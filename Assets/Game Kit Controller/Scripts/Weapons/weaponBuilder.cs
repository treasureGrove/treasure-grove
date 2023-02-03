using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class weaponBuilder : MonoBehaviour
{
	public string weaponName;

	public string relativePathUsableWeaponPrefab = "";
	public GameObject usableWeaponPrefab;
	public LayerMask layerToPlaceWeaponPrefab;

	public playerWeaponsManager mainPlayerWeaponsManager;
	public IKWeaponSystem mainIKWeaponSystem;
	public playerWeaponSystem mainPlayerWeaponSystem;

	public Transform weaponParent;
	public GameObject weaponMeshParent;
	public Transform newWeaponMeshParent;

	public Transform weaponViewTransform;

	public bool replaceArmsModel;
	public Transform armsParent;
	public GameObject currentArmsModel;
	public GameObject newArmsModel;

	public List<partInfo> weaponPartInfoList = new List<partInfo> ();

	public List<settingsInfo> weaponSettingsInfoList = new List<settingsInfo> ();

	public List<GameObject> weaponPartsNotUsedOnAIList = new List<GameObject> ();

	public bool showGizmo;
	public bool showGizmoLabel;
	public Color gizmoLabelColor = Color.black;
	public float gizmoRadius;
	public bool useHandle;

	[TextArea (3, 10)] public string buildWeaponExplanation = "Make sure to drop the prefab of the model for the new weapon you want to create" +
	                                                          " in the scene window and assign the parts of that object into the editor.";

	partInfo currentWeaponPartInfo;

	settingsInfo currentWeaponSettingsInfo;

	public bool createWeaponInventoryActive;
	public bool createWeaponAmmoInventoryActive;

	public bool createUsableWeaponPrefabActive;

	public int weaponAmmoAmountValue = 10;

	string previousWeaponName;

	Texture weaponIconTexture;

	string weaponDescription;

	GameObject customAmmoMesh;
	Texture customAmmoIcon;

	string customAmmoDescription;


	public void buildWeapon ()
	{
		adjustWeaponParts ();

		checkArmsToReplace ();

		resetTemporalWeaponParts ();

		adjustWeaponSettings ();

		completeWeaponBuild ();

		checkIfCreateInventoryWeapon ();

		checkIfCreateInventoryWeaponAmmo ();

		checkCreateUsableWeaponPrefab ();

		checkRemoteRegularWeaponPickupPrefab ();

		GKC_Utils.setActiveGameObjectInEditor (gameObject);

		alignViewWithWeaponCameraPosition ();

		updateComponent ();
	}

	public void adjustWeaponParts ()
	{
		for (int i = 0; i < weaponPartInfoList.Count; i++) { 
			currentWeaponPartInfo = weaponPartInfoList [i];

			if (currentWeaponPartInfo.containsIKTransform) {
				for (int j = 0; j < currentWeaponPartInfo.IKPositionsListOnMesh.Count; j++) { 
					currentWeaponPartInfo.IKPositionsListOnMesh [j].SetParent (weaponMeshParent.transform);
				}
			}

			if (!currentWeaponPartInfo.removeWeaponPartIfNoNewMesh) {
				if (currentWeaponPartInfo.newWeaponMesh != null) {

					if (currentWeaponPartInfo.currentWeaponMesh != null) {
						DestroyImmediate (currentWeaponPartInfo.currentWeaponMesh);
					}

					for (int j = 0; j < currentWeaponPartInfo.extraWeaponPartMeshesList.Count; j++) { 
						DestroyImmediate (currentWeaponPartInfo.extraWeaponPartMeshesList [j]);
					}

					currentWeaponPartInfo.extraWeaponPartMeshesList.Clear ();

					if (currentWeaponPartInfo.temporalWeaponMeshInstantiated) {

						currentWeaponPartInfo.currentWeaponMesh = currentWeaponPartInfo.temporalWeaponMesh;
					} else {
						GameObject newWeaponPart = (GameObject)Instantiate (currentWeaponPartInfo.newWeaponMesh, Vector3.zero, Quaternion.identity);

						Transform newWeaponPartTransform = newWeaponPart.transform;

						newWeaponPartTransform.SetParent (currentWeaponPartInfo.weaponMeshParent);
						newWeaponPartTransform.localPosition = Vector3.zero;
						newWeaponPartTransform.localRotation = Quaternion.identity;

						currentWeaponPartInfo.currentWeaponMesh = newWeaponPart;
					}

					if (currentWeaponPartInfo.containsIKTransform) {
						for (int j = 0; j < currentWeaponPartInfo.IKPositionsListOnMesh.Count; j++) { 
							currentWeaponPartInfo.IKPositionsListOnMesh [j].SetParent (currentWeaponPartInfo.currentWeaponMesh.transform);
						}
					}
				}
			} else {
				DestroyImmediate (currentWeaponPartInfo.currentWeaponMesh);

				for (int j = 0; j < currentWeaponPartInfo.extraWeaponPartMeshesList.Count; j++) { 
					DestroyImmediate (currentWeaponPartInfo.extraWeaponPartMeshesList [j]);
				}

				currentWeaponPartInfo.extraWeaponPartMeshesList.Clear ();
			}

			currentWeaponPartInfo.newWeaponMesh = null;

			if (!currentWeaponPartInfo.removeWeaponPartIfNoNewMesh) {
				if (currentWeaponPartInfo.useEventOnUseWeaponPartEnabled) {
					currentWeaponPartInfo.eventOnUseWeaponPartEnabled.Invoke ();
				}
			} else {
				if (currentWeaponPartInfo.useEventOnUseWeaponPartDisabled) {
					currentWeaponPartInfo.eventOnUseWeaponPartDisabled.Invoke ();
				}
			}
		}
	}

	public void checkArmsToReplace ()
	{
		if (replaceArmsModel) {
			if (newArmsModel != null) {
			
				GameObject newArmsGameObject = (GameObject)Instantiate (newArmsModel, Vector3.zero, Quaternion.identity);

				Transform newArmsTransform = newArmsGameObject.transform;

//				newArmsTransform.SetParent (armsParent);

				newArmsTransform.SetParent (weaponMeshParent.transform);
				newArmsTransform.localPosition = Vector3.zero;
				newArmsTransform.localRotation = Quaternion.identity;

				if (currentArmsModel != null) {
					DestroyImmediate (currentArmsModel);
				}

				currentArmsModel = newArmsGameObject;

				mainIKWeaponSystem.setNewFirstPersonArms (currentArmsModel);

				replaceArmsModel = false;
				newArmsModel = null;

				mainPlayerWeaponsManager.setWeaponPartLayerFromCameraView (currentArmsModel, true);
			} else {
				print ("WARNING: no new arms were found, please, drop an arms model into the new arms field to assign it");
			}
		}
	}

	public void toogleArmsMeshActiveState ()
	{
		if (currentArmsModel != null) {
			currentArmsModel.SetActive (!currentArmsModel.activeSelf);
		}
	}

	public void adjustWeaponSettings ()
	{
		for (int i = 0; i < weaponSettingsInfoList.Count; i++) { 
			currentWeaponSettingsInfo = weaponSettingsInfoList [i];

			if (currentWeaponSettingsInfo.useBoolState) {
				currentWeaponSettingsInfo.eventToSetBoolState.Invoke (currentWeaponSettingsInfo.boolState);
			}

			if (currentWeaponSettingsInfo.useFloatValue) {
				currentWeaponSettingsInfo.eventToSetFloatValue.Invoke (currentWeaponSettingsInfo.floatValue);
			}
		}
	}

	public void checkIfCreateInventoryWeapon ()
	{
		if (createWeaponInventoryActive) {
			print ("create inventory weapon");
			mainIKWeaponSystem.createInventoryWeaponExternally (weaponName, weaponDescription, weaponIconTexture);
		}
	}

	public void setCustomAmmoMeshInfo (GameObject newCustomAmmoMesh, Texture newCustomAmmoIcon, string newCustomAmmoDescription)
	{
		customAmmoMesh = newCustomAmmoMesh;
		customAmmoIcon = newCustomAmmoIcon;
		customAmmoDescription = newCustomAmmoDescription;
	}

	public void checkIfCreateInventoryWeaponAmmo ()
	{
		if (createWeaponAmmoInventoryActive) {
			mainIKWeaponSystem.createInventoryWeaponAmmoExternally ((weaponName + " Ammo"), weaponAmmoAmountValue,
				customAmmoMesh, customAmmoIcon, customAmmoDescription);
		}
	}

	public void checkCreateUsableWeaponPrefab ()
	{
		if (createUsableWeaponPrefabActive) {
			createUsableWeaponPrefab ();
		} else {
			usableWeaponPrefab = null;
		}
	}

	public void checkRemoteRegularWeaponPickupPrefab ()
	{
		mainIKWeaponSystem.removeWeaponRegularPickup ();
	}

	public void adjustWeaponSettingsFromEditor ()
	{
		adjustWeaponSettings ();

		GKC_Utils.updateDirtyScene ("Updating Weapon Builder Content", gameObject);
	}

	public void completeWeaponBuild ()
	{
		mainPlayerWeaponSystem.setWeaponSystemName (weaponName);

		gameObject.name = weaponName;

		if (mainPlayerWeaponsManager != null) {
			mainPlayerWeaponsManager.setWeaponList ();

			mainPlayerWeaponsManager.addWeaponToSamePocket (weaponName, previousWeaponName);

			mainPlayerWeaponsManager.setWeaponPartLayerFromCameraView (weaponMeshParent, mainPlayerWeaponsManager.isFirstPersonActive ());
		}

		print ("New Weapon Built: " + mainPlayerWeaponSystem.getWeaponSystemName ());
	}

	public void resetTemporalWeaponParts ()
	{
		for (int i = 0; i < weaponPartInfoList.Count; i++) { 
			currentWeaponPartInfo = weaponPartInfoList [i];

			currentWeaponPartInfo.temporalWeaponMesh = null;
			currentWeaponPartInfo.temporalWeaponMeshInstantiated = false;
			currentWeaponPartInfo.temporalNewWeaponMesh = null;

			currentWeaponPartInfo.newWeaponMeshPositionOffset = Vector3.zero;
			currentWeaponPartInfo.newWeaponMeshEulerOffset = Vector3.zero;
		}
	}

	public void removeTemporalWeaponParts ()
	{
		for (int i = 0; i < weaponPartInfoList.Count; i++) { 
			currentWeaponPartInfo = weaponPartInfoList [i];

			if (currentWeaponPartInfo.temporalWeaponMeshInstantiated) {
				DestroyImmediate (currentWeaponPartInfo.temporalWeaponMesh);

				currentWeaponPartInfo.temporalWeaponMesh = null;

				currentWeaponPartInfo.temporalNewWeaponMesh = null;
			}

			currentWeaponPartInfo.temporalWeaponMeshInstantiated = false;

			currentWeaponPartInfo.newWeaponMeshPositionOffset = Vector3.zero;
			currentWeaponPartInfo.newWeaponMeshEulerOffset = Vector3.zero;

			if (currentWeaponPartInfo.currentWeaponMesh != null) {
				currentWeaponPartInfo.currentWeaponMesh.SetActive (true);
			}

			currentWeaponPartInfo.newWeaponMesh = null;

			currentWeaponPartInfo.removeWeaponPartIfNoNewMesh = false;

			if (currentWeaponPartInfo.currentWeaponMesh != null) {
				currentWeaponPartInfo.currentWeaponMesh.SetActive (true);
			}

			for (int j = 0; j < currentWeaponPartInfo.extraWeaponPartMeshesList.Count; j++) { 
				if (currentWeaponPartInfo.extraWeaponPartMeshesList [j] != null) {
					currentWeaponPartInfo.extraWeaponPartMeshesList [j].SetActive (true);
				}
			}
		}
	}

	public void setCreateWeaponInventoryActiveState (bool state)
	{
		createWeaponInventoryActive = state;

		updateComponent ();
	}

	public void setCreateWeaponAmmoInventoryActiveState (bool state)
	{
		createWeaponAmmoInventoryActive = state;

		updateComponent ();
	}

	public void setWeaponAmmoAmountValue (float ammoAmount)
	{
		weaponAmmoAmountValue = (int)ammoAmount;

		updateComponent ();
	}

	public void setCreateUsableWeaponPrefabActiveState (bool state)
	{
		createUsableWeaponPrefabActive = state;

		updateComponent ();
	}

	public void alignViewWithWeaponCameraPosition ()
	{
		setEditorCameraPosition (weaponViewTransform);
	}

	public void setEditorCameraPosition (Transform transformToUse)
	{
		GKC_Utils.alignViewToObject (transformToUse);
	}

	public void setNewWeaponName (string newWeaponName)
	{
		previousWeaponName = weaponName;

		weaponName = newWeaponName;

		updateComponent ();
	}

	public void setNewWeaponIconTexture (Texture newWeaponIconTexture)
	{
		weaponIconTexture = newWeaponIconTexture;
	}

	public void setNewWeaponDescription (string newValue)
	{
		weaponDescription = newValue;
	}

	public void setWeaponInventorySlotIconInEditor (Texture newTexture)
	{
		mainPlayerWeaponSystem.setWeaponInventorySlotIconInEditor (newTexture);
	}

	public void checkWeaponBuilder (playerWeaponsManager newPlayerWeaponsManager)
	{
		if (newPlayerWeaponsManager != null) {
			mainPlayerWeaponsManager = newPlayerWeaponsManager;
			weaponParent = mainPlayerWeaponsManager.getWeaponsParent ();

			updateComponent ();
		}
	}

	public void checkWeaponsPartsToRemoveOnAI ()
	{
		for (int i = 0; i < weaponPartsNotUsedOnAIList.Count; i++) {
			if (weaponPartsNotUsedOnAIList [i] != null) {
				DestroyImmediate (weaponPartsNotUsedOnAIList [i]);
			}
		}

		weaponPartsNotUsedOnAIList.Clear ();

		updateComponent ();
	}

	public void createUsableWeaponPrefab ()
	{
		relativePathUsableWeaponPrefab = pathInfoValues.getUsableWeaponsPrefabsPath ();

		usableWeaponPrefab = GKC_Utils.createPrefab (relativePathUsableWeaponPrefab, weaponName, gameObject);

		updateComponent ();

		GKC_Utils.updateDirtyScene ("Updating Weapon Builder Content", gameObject);
	}

	public void instantiateUsableWeaponPrefabInScene ()
	{
		relativePathUsableWeaponPrefab = pathInfoValues.getUsableWeaponsPrefabsPath ();

		GKC_Utils.instantiatePrefabInScene (relativePathUsableWeaponPrefab, weaponName, layerToPlaceWeaponPrefab);

		GKC_Utils.updateDirtyScene ("Updating Weapon Builder Content", gameObject);
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			for (int i = 0; i < weaponPartInfoList.Count; i++) {
				if (weaponPartInfoList [i].objectAlwaysUsed && weaponPartInfoList [i].currentWeaponMesh != null) {
					Gizmos.color = Color.yellow;

					Gizmos.DrawSphere (weaponPartInfoList [i].currentWeaponMesh.transform.position, gizmoRadius);
				}
			}
		}
	}

	[System.Serializable]
	public class settingsInfo
	{
		public string Name;
		public bool useBoolState;
		public bool boolState;
		public eventParameters.eventToCallWithBool eventToSetBoolState;
		public bool useFloatValue;
		public float floatValue;
		public eventParameters.eventToCallWithAmount eventToSetFloatValue;

		public bool expandElement;
	}

	[System.Serializable]
	public class partInfo
	{
		public string Name;

		public bool removeWeaponPartIfNoNewMesh;

		public Transform weaponMeshParent;
		public GameObject currentWeaponMesh;
		public GameObject newWeaponMesh;

		public bool objectAlwaysUsed;

		public List<GameObject> extraWeaponPartMeshesList = new List<GameObject> ();

		public bool containsIKTransform;
		public List<Transform> IKPositionsListOnMesh = new List<Transform> ();

		public bool temporalWeaponMeshInstantiated;
		public GameObject temporalWeaponMesh;

		public GameObject temporalNewWeaponMesh;

		public bool useEventOnUseWeaponPartEnabled;
		public UnityEvent eventOnUseWeaponPartEnabled;
		public bool useEventOnUseWeaponPartDisabled;
		public UnityEvent eventOnUseWeaponPartDisabled;

		public Vector3 newWeaponMeshPositionOffset;
		public Vector3 newWeaponMeshEulerOffset;

		public bool expandElement;
	}
}
