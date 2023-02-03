using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponTransformInfoSystem : MonoBehaviour
{
	public List<weaponsTransformInfo> weaponsTransformInfoList = new List<weaponsTransformInfo> ();

	public weaponTransformData mainWeaponTransformData;

	public int weaponID;

	public string newListToAddName;
	public Transform newListToAddParent;

	public string currentListName;

	public int numberOfElements;

	public Transform mainWeaponTransform;

	public List<Transform> mainWeaponPositionsParentThirdPerson = new List<Transform> ();

	public List<Transform> mainWeaponPositionChildsThirdPerson = new List<Transform> ();

	public Vector3 currentOffsetParentThirdPerson;

	public List<Transform> mainWeaponPositionsParentFirstPerson = new List<Transform> ();

	public List<Transform> mainWeaponPositionChildsFirstPerson = new List<Transform> ();

	public Vector3 currentOffsetParentFirstPerson;

	public void getMainWeaponPositionChildsThirdPerson ()
	{
		mainWeaponPositionChildsThirdPerson.Clear ();

		for (int i = 0; i < mainWeaponPositionsParentThirdPerson.Count; i++) {
			if (mainWeaponPositionsParentThirdPerson [i] != null) {
				Component[] components = mainWeaponPositionsParentThirdPerson [i].GetComponentsInChildren (typeof(Transform));

				foreach (Transform child in components) {
					if (!mainWeaponPositionsParentThirdPerson.Contains (child)) {
						print (child.name);

						mainWeaponPositionChildsThirdPerson.Add (child);
					}
				}
			}
		}

		updateComponent ();
	}

	public void setCurrentOffsetParentOnThirdPerson ()
	{
		for (int i = 0; i < mainWeaponPositionChildsThirdPerson.Count; i++) {
			if (mainWeaponPositionChildsThirdPerson [i] != null) {
				mainWeaponPositionChildsThirdPerson [i].position += Vector3.right * currentOffsetParentThirdPerson.x;
				mainWeaponPositionChildsThirdPerson [i].position += Vector3.up * currentOffsetParentThirdPerson.y;
				mainWeaponPositionChildsThirdPerson [i].position += Vector3.forward * currentOffsetParentThirdPerson.z;
			}
		}

		checkMainWeaponTransformNull ();

		mainWeaponTransform.position += Vector3.right * currentOffsetParentThirdPerson.x;
		mainWeaponTransform.position += Vector3.up * currentOffsetParentThirdPerson.y;
		mainWeaponTransform.position += Vector3.forward * currentOffsetParentThirdPerson.z;

		updateComponent ();
	}

	void checkMainWeaponTransformNull ()
	{
		if (mainWeaponTransform == null) {
			playerWeaponSystem currentPlayerWeaponSystem = GetComponentInChildren<playerWeaponSystem> ();

			if (currentPlayerWeaponSystem != null) {
				mainWeaponTransform = currentPlayerWeaponSystem.transform;
			}
		}

	}

	public void getMainWeaponPositionChildsFirstPerson ()
	{
		mainWeaponPositionChildsFirstPerson.Clear ();

		for (int i = 0; i < mainWeaponPositionsParentFirstPerson.Count; i++) {
			if (mainWeaponPositionsParentFirstPerson [i] != null) {
				Component[] components = mainWeaponPositionsParentFirstPerson [i].GetComponentsInChildren (typeof(Transform));

				foreach (Transform child in components) {
					if (!mainWeaponPositionsParentFirstPerson.Contains (child)) {
						print (child.name);

						mainWeaponPositionChildsFirstPerson.Add (child);
					}
				}
			}
		}

		updateComponent ();
	}

	public void setCurrentOffsetParentOnFirstPerson ()
	{
		for (int i = 0; i < mainWeaponPositionChildsFirstPerson.Count; i++) {
			if (mainWeaponPositionChildsFirstPerson [i] != null) {
				mainWeaponPositionChildsFirstPerson [i].position += Vector3.right * currentOffsetParentFirstPerson.x;
				mainWeaponPositionChildsFirstPerson [i].position += Vector3.up * currentOffsetParentFirstPerson.y;
				mainWeaponPositionChildsFirstPerson [i].position += Vector3.forward * currentOffsetParentFirstPerson.z;
			}
		}

		updateComponent ();
	}

	public void adjustDrawKeepPositionToWeaponPosition ()
	{
		IKWeaponSystem currentIKWeaponSystem = GetComponent<IKWeaponSystem> ();

		if (currentIKWeaponSystem != null) {
			checkMainWeaponTransformNull ();

			Vector3 weaponPosition = mainWeaponTransform.position;
			Quaternion weaponRotation = mainWeaponTransform.rotation;

			currentIKWeaponSystem.thirdPersonWeaponInfo.keepPosition.position = weaponPosition;
			currentIKWeaponSystem.thirdPersonWeaponInfo.keepPosition.rotation = weaponRotation;

			currentIKWeaponSystem.firstPersonWeaponInfo.keepPosition.position = weaponPosition;
			currentIKWeaponSystem.firstPersonWeaponInfo.keepPosition.rotation = weaponRotation;
		}

		GKC_Utils.updateDirtyScene ("Adjust Draw Keep Position To Weapon Position ", gameObject);

		updateComponent ();
	}

	public void copyTransformValuesToBuffer ()
	{
		if (weaponID != mainWeaponTransformData.weaponID) {
			mainWeaponTransformData.objectsTransformInfoList.Clear ();
			mainWeaponTransformData.weaponID = weaponID;
		}

		for (int i = 0; i < weaponsTransformInfoList.Count; i++) {
			currentListName = weaponsTransformInfoList [i].Name;
		
			int currentListIndex = getListIndex (currentListName);

			objectsTransformInfo newObjectsTransformInfo = new objectsTransformInfo ();

			if (currentListIndex > -1) {
				newObjectsTransformInfo = mainWeaponTransformData.objectsTransformInfoList [currentListIndex];
				newObjectsTransformInfo.objectTransformInfoList.Clear ();
			} else {
				newObjectsTransformInfo.Name = currentListName;
			}
				
			for (int k = 0; k < weaponsTransformInfoList [i].objectTransformInfoList.Count; k++) {
				objectTransformInfo newObjectTransformInfo = new objectTransformInfo {
					objectPosition = weaponsTransformInfoList [i].objectTransformInfoList [k].localPosition,
					objectRotation = weaponsTransformInfoList [i].objectTransformInfoList [k].localRotation
				};

				print ("Copied transform values of " + weaponsTransformInfoList [i].objectTransformInfoList [k].name + ": " +
				weaponsTransformInfoList [i].objectTransformInfoList [k].localPosition.ToString ("F7") + "_" +
				weaponsTransformInfoList [i].objectTransformInfoList [k].localEulerAngles.ToString ("F7"));

				newObjectsTransformInfo.objectTransformInfoList.Add (newObjectTransformInfo);
			}

			if (currentListIndex == -1) {
				mainWeaponTransformData.objectsTransformInfoList.Add (newObjectsTransformInfo);
			}
		}
	}

	public int getListIndex (string listName)
	{
		for (int k = 0; k < mainWeaponTransformData.objectsTransformInfoList.Count; k++) {
			if (mainWeaponTransformData.objectsTransformInfoList [k].Name.Equals (listName)) {
				return k;
			}
		}

		return -1;
	}

	public void pasteTransformValuesToBuffer ()
	{
		if (weaponID != mainWeaponTransformData.weaponID) {
			print ("WARNING: You are trying to use the stored positions from another weapon, " +
			"make sure to make a note of the positions of this weapon before pasting the new values.");
			return;
		}

		numberOfElements = 0;

		for (int i = 0; i < weaponsTransformInfoList.Count; i++) {
			currentListName = weaponsTransformInfoList [i].Name;

			int currentListIndex = getListIndex (currentListName);

			if (currentListIndex > -1) {
				objectsTransformInfo newObjectsTransformInfo = mainWeaponTransformData.objectsTransformInfoList [currentListIndex];
		
				for (int k = 0; k < newObjectsTransformInfo.objectTransformInfoList.Count; k++) {

					objectTransformInfo currentObjectTransformInfo = newObjectsTransformInfo.objectTransformInfoList [k];

					weaponsTransformInfoList [i].objectTransformInfoList [k].localPosition = currentObjectTransformInfo.objectPosition;
					weaponsTransformInfoList [i].objectTransformInfoList [k].localRotation = currentObjectTransformInfo.objectRotation;

					print ("Pasted transform values to " + weaponsTransformInfoList [i].objectTransformInfoList [k].name + ": " +
					currentObjectTransformInfo.objectPosition.ToString ("F7") + "_" +
					currentObjectTransformInfo.objectRotation.eulerAngles.ToString ("F7"));

					GKC_Utils.updateComponent (weaponsTransformInfoList [i].objectTransformInfoList [k]);

					numberOfElements++;
				}
			}
		}

		IKWeaponSystem currentIKWeaponSystem = GetComponent<IKWeaponSystem> ();

		if (currentIKWeaponSystem != null) {
			GKC_Utils.updateComponent (currentIKWeaponSystem);
		}

		GKC_Utils.updateDirtyScene ("Update Weapon Positions " + weaponID, gameObject);

		updateComponent ();
	}

	public void cleanPositionsOnScriptable ()
	{
		mainWeaponTransformData.weaponID = -1;

		mainWeaponTransformData.objectsTransformInfoList.Clear ();

		updateComponent ();
	}

	public void addNewList ()
	{
		if (newListToAddParent != null) {
			if (newListToAddName.Equals ("")) {
				newListToAddName = newListToAddParent.name;

				print ("No name for the list used, using the name of the parent, called " + newListToAddName);
			}

			for (int i = 0; i < weaponsTransformInfoList.Count; i++) {
				if (weaponsTransformInfoList [i].Name.Equals (newListToAddName)) {
					print ("WARNING: A list called '" + newListToAddName + "' already exists! " +
					"Configure a new list name and set a parent to get the child transform data.");

					return;
				}
			}

			weaponsTransformInfo newWeaponsTransformInfo = new weaponsTransformInfo { Name = newListToAddName };

			Component[] components = newListToAddParent.GetComponentsInChildren (typeof(Transform));
			foreach (Transform child in components) {
				if (child != newListToAddParent) {
					newWeaponsTransformInfo.objectTransformInfoList.Add (child);
				}
			}
			
			newListToAddName = "";
			newListToAddParent = null;
			weaponsTransformInfoList.Add (newWeaponsTransformInfo);

			updateComponent ();
		} else {
			print ("WARNING: Please, configure a list name and set a parent to get it's child transform data.");
		}
	}

	public void addNewEmptyList ()
	{
		weaponsTransformInfo newWeaponsTransformInfo = new weaponsTransformInfo { Name = "New List" };
		
		weaponsTransformInfoList.Add (newWeaponsTransformInfo);

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class weaponsTransformInfo
	{
		public string Name;
		public List<Transform> objectTransformInfoList = new List<Transform> ();
	}
}
