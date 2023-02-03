using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class vehicleBuilder : MonoBehaviour
{
	public string vehicleName;

	public string prefabsPath = "Assets/Game Kit Controller/Prefabs/Vehicles/Vehicles/";

	public LayerMask layerToPlaceVehicle;
	public float placeVehicleOffset = 2;

	public bool showGizmo;
	public bool showGizmoLabel;
	public Color gizmoLabelColor = Color.black;
	public float gizmoRadius;
	public bool useHandle;

	public GameObject vehicle;
	public GameObject vehicleCamera;

	public Transform vehicleViewTransform;

	public vehicleHUDManager mainVehicleHUDManager;

	public List<vehiclePartInfo> vehiclePartInfoList = new List<vehiclePartInfo> ();

	public List<vehicleSettingsInfo> vehicleSettingsInfoList = new List<vehicleSettingsInfo> ();

	vehiclePartInfo currentVehiclePartInfo;

	vehicleSettingsInfo currentVehicleSettingsInfo;

	public bool placeVehicleInScene;

	[TextArea (5, 15)] public string buildVehicleExplanation = "IMPORTANT: once you create a new vehicle, add the colliders you need on the vehicle, " +
	                                                           "set those colliders to the layer 'vehicle' and attach a vehicle damage receiver component. \n\nFinally, go to vehicle hud manager component " +
	                                                           "and press UPDATE VEHICLE PARTS button";

	public void buildVehicle ()
	{
		adjustVehicleParts ();

		completeVehicleBuild ();

		resetTemporalVehicleParts ();

		adjustVehicleSettings ();

		updateComponent ();
	}

	public void adjustVehicleParts ()
	{
		for (int i = 0; i < vehiclePartInfoList.Count; i++) { 
			currentVehiclePartInfo = vehiclePartInfoList [i];

			if (currentVehiclePartInfo.useVehiclePart) {
				if (currentVehiclePartInfo.newVehicleMesh) {

					if (currentVehiclePartInfo.currentVehicleMesh) {
						DestroyImmediate (currentVehiclePartInfo.currentVehicleMesh);
					}

					for (int j = 0; j < currentVehiclePartInfo.extraVehiclePartMeshesList.Count; j++) { 
						DestroyImmediate (currentVehiclePartInfo.extraVehiclePartMeshesList [j]);
					}

					currentVehiclePartInfo.extraVehiclePartMeshesList.Clear ();

					if (currentVehiclePartInfo.temporalVehicleMeshInstantiated) {

						currentVehiclePartInfo.currentVehicleMesh = currentVehiclePartInfo.temporalVehicleMesh;
					} else {
						GameObject newVehiclePart = (GameObject)Instantiate (currentVehiclePartInfo.newVehicleMesh, Vector3.zero, Quaternion.identity);

						Transform newVehiclePartTransform = newVehiclePart.transform;

						newVehiclePartTransform.SetParent (currentVehiclePartInfo.vehicleMeshParent);
						newVehiclePartTransform.localPosition = Vector3.zero;
						newVehiclePartTransform.localRotation = Quaternion.identity;
			
						currentVehiclePartInfo.currentVehicleMesh = newVehiclePart;
					}
				}
			} else {
				DestroyImmediate (currentVehiclePartInfo.currentVehicleMesh);

				for (int j = 0; j < currentVehiclePartInfo.extraVehiclePartMeshesList.Count; j++) { 
					DestroyImmediate (currentVehiclePartInfo.extraVehiclePartMeshesList [j]);
				}

				currentVehiclePartInfo.extraVehiclePartMeshesList.Clear ();
			}

			currentVehiclePartInfo.newVehicleMesh = null;

			if (currentVehiclePartInfo.useVehiclePart) {
				if (currentVehiclePartInfo.useEventOnUseVehiclePartEnabled) {
					currentVehiclePartInfo.eventOnUseVehiclePartEnabled.Invoke ();
				}
			} else {
				if (currentVehiclePartInfo.useEventOnUseVehiclePartDisabled) {
					currentVehiclePartInfo.eventOnUseVehiclePartDisabled.Invoke ();
				}
			}
		}
	}

	public void adjustVehicleSettings ()
	{
		for (int i = 0; i < vehicleSettingsInfoList.Count; i++) { 
			currentVehicleSettingsInfo = vehicleSettingsInfoList [i];

			if (currentVehicleSettingsInfo.useBoolState) {
				currentVehicleSettingsInfo.eventToSetBoolState.Invoke (currentVehicleSettingsInfo.boolState);
			}

			if (currentVehicleSettingsInfo.useFloatValue) {
				currentVehicleSettingsInfo.eventToSetFloatValue.Invoke (currentVehicleSettingsInfo.floatValue);
			}
		}
	}

	public void adjustVehicleSettingsFromEditor ()
	{
		adjustVehicleSettings ();
	}

	public void completeVehicleBuild ()
	{
		mainVehicleHUDManager.setVehicleName (vehicleName);

		vehicle.name = vehicleName + " Controller";
		vehicleCamera.name = vehicleName + " Camera";

		gameObject.name = vehicleName;

		mainVehicleHUDManager.setVehicleParts ();

		print ("New Vehicle Build: " + mainVehicleHUDManager.getVehicleName ());

		if (placeVehicleInScene) {
			placeVehicleInCameraPosition ();
		}
	}

	public void resetTemporalVehicleParts ()
	{
		for (int i = 0; i < vehiclePartInfoList.Count; i++) { 
			currentVehiclePartInfo = vehiclePartInfoList [i];

			currentVehiclePartInfo.temporalVehicleMesh = null;
			currentVehiclePartInfo.temporalVehicleMeshInstantiated = false;
			currentVehiclePartInfo.temporalNewVehicleMesh = null;

			currentVehiclePartInfo.newVehicleMeshPositionOffset = Vector3.zero;
			currentVehiclePartInfo.newVehicleMeshEulerOffset = Vector3.zero;
		}
	}

	public void removeTemporalVehicleParts ()
	{
		for (int i = 0; i < vehiclePartInfoList.Count; i++) { 
			currentVehiclePartInfo = vehiclePartInfoList [i];

			if (currentVehiclePartInfo.temporalVehicleMeshInstantiated) {
				DestroyImmediate (currentVehiclePartInfo.temporalVehicleMesh);

				currentVehiclePartInfo.temporalVehicleMesh = null;

				currentVehiclePartInfo.temporalNewVehicleMesh = null;
			}

			currentVehiclePartInfo.temporalVehicleMeshInstantiated = false;

			currentVehiclePartInfo.newVehicleMeshPositionOffset = Vector3.zero;
			currentVehiclePartInfo.newVehicleMeshEulerOffset = Vector3.zero;

			if (currentVehiclePartInfo.currentVehicleMesh) {
				currentVehiclePartInfo.currentVehicleMesh.SetActive (true);
			}

			currentVehiclePartInfo.newVehicleMesh = null;

			currentVehiclePartInfo.useVehiclePart = true;

			if (currentVehiclePartInfo.currentVehicleMesh) {
				currentVehiclePartInfo.currentVehicleMesh.SetActive (true);
			}
				
			for (int j = 0; j < currentVehiclePartInfo.extraVehiclePartMeshesList.Count; j++) { 
				if (currentVehiclePartInfo.extraVehiclePartMeshesList [j]) {
					currentVehiclePartInfo.extraVehiclePartMeshesList [j].SetActive (true);
				}
			}
		}
	}

	public void getMeshParentTransformValues ()
	{
		for (int i = 0; i < vehiclePartInfoList.Count; i++) { 
			currentVehiclePartInfo = vehiclePartInfoList [i];

			if (currentVehiclePartInfo.vehicleMeshParent) {
				currentVehiclePartInfo.originalMeshParentPosition = currentVehiclePartInfo.vehicleMeshParent.localPosition;
				currentVehiclePartInfo.originalMeshParentEulerAngles = currentVehiclePartInfo.vehicleMeshParent.localEulerAngles;
			}
		}
	}

	public void placeVehicleInCameraPosition ()
	{
		Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

		if (currentCameraEditor != null) {
			Vector3 editorCameraPosition = currentCameraEditor.transform.position;
			Vector3 editorCameraForward = currentCameraEditor.transform.forward;
			RaycastHit hit;

			if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceVehicle)) {
				transform.position = hit.point + (Vector3.up * placeVehicleOffset);
			}
		}
	}

	public void alignViewWithVehicleCameraPosition ()
	{
		setEditorCameraPosition (vehicleViewTransform);
	}

	public void setEditorCameraPosition (Transform transformToUse)
	{
		GKC_Utils.alignViewToObject (transformToUse);
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
			for (int i = 0; i < vehiclePartInfoList.Count; i++) {
				if (vehiclePartInfoList [i].objectAlwaysUsed && vehiclePartInfoList [i].currentVehicleMesh) {
					Gizmos.color = Color.yellow;

					Gizmos.DrawSphere (vehiclePartInfoList [i].currentVehicleMesh.transform.position, gizmoRadius);
				}
			}
		}
	}

	[System.Serializable]
	public class vehicleSettingsInfo
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
	public class vehiclePartInfo
	{
		public string Name;

		public bool useVehiclePart = true;

		public bool moveMeshParentInsteadOfPart;

		public Transform vehicleMeshParent;
		public GameObject currentVehicleMesh;
		public GameObject newVehicleMesh;

		public bool objectAlwaysUsed;

		public List<GameObject> extraVehiclePartMeshesList = new List<GameObject> ();

		public bool temporalVehicleMeshInstantiated;
		public GameObject temporalVehicleMesh;

		public GameObject temporalNewVehicleMesh;

		public bool useEventOnUseVehiclePartEnabled;
		public UnityEvent eventOnUseVehiclePartEnabled;
		public bool useEventOnUseVehiclePartDisabled;
		public UnityEvent eventOnUseVehiclePartDisabled;

		public Vector3 newVehicleMeshPositionOffset;
		public Vector3 newVehicleMeshEulerOffset;

		public Vector3 originalMeshParentPosition;
		public Vector3 originalMeshParentEulerAngles;

		public bool expandElement;

		public bool showHandleTool = true;
	}
}
