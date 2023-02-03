using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class lockedCameraSystem : MonoBehaviour
{
	public LayerMask layerToPlaceNewCamera;
	public bool useRaycastToPlaceCameras;
	public Vector3 eventTriggerScale;
	public List<cameraElement> lockedCameraList = new List<cameraElement> ();

	public bool showGizmo;
	public bool showGizmoOnPlaying;
	public Color gizmoColor;
	public Color gizmoLabelColor;

	public float gizmoRadius = 0.3f;
	public float gizmoArrowLength = 1.4f;
	public float gizmoArrowLineLength = 3.4f;
	public float gizmoArrowAngle = 20;

	public Color gizmoArrowColor;
	public GameObject cameraTransformPrefab;
	public GameObject cameraTriggerPrefab;
	public bool useFreeHandle;
	public float handleRadius = 0.1f;
	public Color handleGizmoColor;

	public Color boundColor = Color.red;

	public bool showDoPositionHandles;
	public bool showButtons = true;
	public float buttonsSize = 0.1f;

	public bool showFloorTriggersCubes;
	public Color floorTriggersCubesColor;

	public bool showWaypointList;
	public Color waypointListColor;

	playerCamera playerCameraManager;
	GameObject currentPlayer;

	void Start ()
	{
		cameraElement currentCameraElement;
		cameraAxis currentCameraAxis;

		for (int i = 0; i < lockedCameraList.Count; i++) {
			currentCameraElement = lockedCameraList [i];

			for (int j = 0; j < currentCameraElement.cameraTransformList.Count; j++) {

				currentCameraAxis = currentCameraElement.cameraTransformList [j];
				
				if (currentCameraAxis.axis != null) {
					currentCameraAxis.originalPivotRotationY = currentCameraAxis.axis.localEulerAngles.y;
				} else {
					print ("WARNING: camera axis on " + currentCameraElement.name + " " + currentCameraAxis.name + " not found, check the locked camera" +
					"system inspector");
				}

				if (currentCameraAxis.cameraPosition != null) {
					currentCameraAxis.originalCameraRotationX = currentCameraAxis.cameraPosition.localEulerAngles.x;
					currentCameraAxis.originalCameraLocalPosition = currentCameraAxis.cameraPosition.localPosition;
					currentCameraAxis.originalCameraForward = currentCameraAxis.axis.InverseTransformDirection (currentCameraAxis.cameraPosition.forward);
				} else {
					print ("WARNING: camera position on " + currentCameraElement.name + " " + currentCameraAxis.name + " not found, check the locked camera" +
					"system inspector");
				}

				if (currentCameraAxis.axis != null) {
					currentCameraAxis.originalCameraAxisLocalPosition = currentCameraAxis.axis.localPosition;
				} else {
					print ("WARNING: camera axis on " + currentCameraElement.name + " " + currentCameraAxis.name + " not found, check the locked camera" +
					"system inspector");
				}

				if (currentCameraAxis.lockedCameraPivot != null) {
					currentCameraAxis.originalLockedCameraPivotPosition = currentCameraAxis.lockedCameraPivot.position;
				}
			}
		}

		StartCoroutine (checkCamerasCoroutine ());
	}

	IEnumerator checkCamerasCoroutine ()
	{
		yield return new WaitForSeconds (0.01f);
	
		checkStartGameWithView ();
	}

	void checkStartGameWithView ()
	{
		cameraElement currentCameraElement;
		cameraAxis currentCameraAxis;

		for (int i = 0; i < lockedCameraList.Count; i++) {

			currentCameraElement = lockedCameraList [i];

			for (int j = 0; j < currentCameraElement.cameraTransformList.Count; j++) {
				currentCameraAxis = currentCameraElement.cameraTransformList [j];

				if (currentCameraAxis.startGameWithThisView) {

					if (!currentCameraAxis.smoothTransitionAtStart) {
						currentCameraAxis.previoslyUseSmoohtTransitionState = currentCameraAxis.smoothCameraTransition;

						currentCameraAxis.smoothCameraTransition = false;
					}

					if (currentCameraAxis.useMultiplePlayers) {
						for (int k = 0; k < currentCameraAxis.playerForViewList.Count; k++) {
							if (currentCameraAxis.playerForViewList [k] != null) {
								getPlayerGameObject (currentCameraAxis.playerForViewList [k]);
								setCameraTransform (currentCameraAxis.cameraPosition.gameObject);
							}
						}
					} else {
						if (currentCameraAxis.playerForView != null) {
							getPlayerGameObject (currentCameraAxis.playerForView);
							setCameraTransform (currentCameraAxis.cameraPosition.gameObject);
						}
					}

					if (!currentCameraAxis.smoothTransitionAtStart) {
						currentCameraAxis.smoothCameraTransition = currentCameraAxis.previoslyUseSmoohtTransitionState;
					}
				}
			}
		}
	}

	public void setCameraTransform (GameObject cameraTransform)
	{
		if (playerCameraManager == null) {
			return;
		}

		if (playerCameraManager != null && playerCameraManager.getCurrentLockedCameraTransform () == cameraTransform.transform) {
			return;
		}

		cameraElement currentCameraElement;
		cameraAxis currentCameraAxis;

		for (int i = 0; i < lockedCameraList.Count; i++) {

			currentCameraElement = lockedCameraList [i];

			for (int j = 0; j < lockedCameraList [i].cameraTransformList.Count; j++) {

				currentCameraAxis = currentCameraElement.cameraTransformList [j];

				if (currentCameraAxis.cameraPosition == cameraTransform.transform) {
					if (currentCameraElement.setCameraToFree) {
						playerCameraManager.setCameraToFreeOrLocked (playerCamera.typeOfCamera.free, currentCameraAxis);
					} else {
						playerCameraManager.setCameraToFreeOrLocked (playerCamera.typeOfCamera.locked, currentCameraAxis);

						if (currentCameraAxis.useCameraLimitSystem) {
							currentCameraAxis.mainCamera2_5dZoneLimitSystem.setCameraLimitOnPlayer (playerCameraManager.getPlayerControllerGameObject ());
						}
					}

					currentCameraElement.isCurrentCameraTransform = true;
				} else {
					currentCameraElement.isCurrentCameraTransform = false;
				}
			}
		}
	}

	public void findPlayerOnScene ()
	{
		getPlayerGameObject (GKC_Utils.findMainPlayerOnScene ());
	}

	public void getPlayerGameObject (GameObject objectToCheck)
	{
		if (!objectToCheck) {
			return;
		}

		currentPlayer = objectToCheck;

		playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();
		} else {
//			print (objectToCheck.name);

			vehicleHUDManager currentvehicleHUDManager = applyDamage.getVehicleHUDManager (objectToCheck);

			if (currentvehicleHUDManager != null && currentvehicleHUDManager.isVehicleBeingDriven ()) {
				currentPlayer = currentvehicleHUDManager.getCurrentDriver ();

				if (currentPlayer != null) {
					mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

					if (mainPlayerComponentsManager != null) {
						playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();
					}
				}
			}
		}
	}

	public void setLockedCameraToEditorCameraPosition (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

		if (currentCameraEditor != null) {
			Vector3 editorCameraPosition = currentCameraEditor.transform.position;
			Quaternion editorCameraRotation = currentCameraEditor.transform.rotation;
			Vector3 editorCameraEuler = editorCameraRotation.eulerAngles;

			cameraAxis currentCameraAxis = lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];

			Transform axisTransform = currentCameraAxis.axis;

			Transform cameraTransform = currentCameraAxis.cameraPosition;

			axisTransform.position = editorCameraPosition;

			axisTransform.rotation = Quaternion.Euler (new Vector3 (0, editorCameraEuler.y, 0));

			cameraTransform.localRotation = Quaternion.Euler (new Vector3 (editorCameraEuler.x, 0, 0));

			if (currentCameraAxis.lockedCameraPivot != null) {
				Transform currentLockedCameraPivot = currentCameraAxis.lockedCameraPivot;
		
				axisTransform.SetParent (null);

				currentLockedCameraPivot.position = currentCameraAxis.axis.position;
				currentLockedCameraPivot.rotation = currentCameraAxis.axis.rotation;

				currentCameraAxis.axis.SetParent (currentLockedCameraPivot);
			}

			if (currentCameraAxis.followPlayerPosition && currentCameraAxis.useTopDownView) {
				if (currentCameraAxis.lockedCameraPivot != null) {
					Transform currentLockedCameraPivot = currentCameraAxis.lockedCameraPivot;

					Vector3 axisTransformTargetPosition = currentLockedCameraPivot.position;

					Vector3 axisTransformTargetEuler = currentLockedCameraPivot.eulerAngles;

					currentLockedCameraPivot.localPosition = Vector3.zero;
					currentLockedCameraPivot.localRotation = Quaternion.identity;

					axisTransform.position = editorCameraPosition;

					axisTransform.eulerAngles = axisTransformTargetEuler;

					cameraTransform.localPosition = Vector3.zero;
				}
			} 
		}
	}

	public void setEditorCameraPositionToLockedCamera (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		Transform lockedCameraTransform = lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex].cameraPosition;

		setEditorCameraPosition (lockedCameraTransform);
	}

	public void duplicateLockedCamera (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		addNewCameraTransformElememnt (lockedCameraListIndex);

		int currentCameraElementIndex = lockedCameraList [lockedCameraListIndex].cameraTransformList.Count - 1;

		cameraAxis currentCameraElement = lockedCameraList [lockedCameraListIndex].cameraTransformList [currentCameraElementIndex];

		cameraAxis currentCameraElementToDuplicate = lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];

		currentCameraElement.cameraParentTransform.position = currentCameraElementToDuplicate.cameraParentTransform.position;
		currentCameraElement.axis.position = currentCameraElementToDuplicate.axis.position;

		currentCameraElement.cameraPosition.position = currentCameraElementToDuplicate.cameraPosition.position;
		currentCameraElement.cameraTriggerParentTransform.position = currentCameraElementToDuplicate.cameraTriggerParentTransform.position;

		for (int i = 0; i < currentCameraElementToDuplicate.triggerTransform.Count; i++) {
			if (currentCameraElement.triggerTransform.Count < i) {
				addCameraTrigger (lockedCameraListIndex, currentCameraElementIndex);
			}

			currentCameraElement.triggerTransform [i].position = currentCameraElementToDuplicate.triggerTransform [i].position;
			currentCameraElement.triggerTransform [i].localScale = currentCameraElementToDuplicate.triggerTransform [i].localScale;
		}

		updateComponent ();
	}

	public void assignPlayerToStartOnLockedCamera (GameObject newPlayer)
	{
		if (lockedCameraList.Count > 0) {

			cameraElement currentCameraElement = lockedCameraList [0];

			if (currentCameraElement.cameraTransformList.Count > 0) {
				cameraAxis currentCameraAxis = currentCameraElement.cameraTransformList [0];

				currentCameraAxis.startGameWithThisView = true;

				currentCameraAxis.playerForView = newPlayer;

				updateComponent ();
			}
		}
	}

	public void addLookAtCustomMultipleTargetsList (Transform newTransform)
	{
		addOrRemoveLookAtCustomMultipleTargetsList (newTransform, true);
	}

	public void removeLookAtCustomMultipleTargetsList (Transform newTransform)
	{
		addOrRemoveLookAtCustomMultipleTargetsList (newTransform, false);
	}

	public void addOrRemoveLookAtCustomMultipleTargetsList (Transform newTransform, bool state)
	{
		if (lockedCameraList.Count > 0) {

			cameraElement currentCameraElement = lockedCameraList [0];

			if (currentCameraElement.cameraTransformList.Count > 0) {
				cameraAxis currentCameraAxis = currentCameraElement.cameraTransformList [0];

				if (state) {
					if (!currentCameraAxis.lookAtCustomMultipleTargetsList.Contains (newTransform)) {
						currentCameraAxis.lookAtCustomMultipleTargetsList.Add (newTransform);
					}
				} else {
					if (currentCameraAxis.lookAtCustomMultipleTargetsList.Contains (newTransform)) {
						currentCameraAxis.lookAtCustomMultipleTargetsList.Remove (newTransform);
					}
				}
			}
		}
	}

	public static void setEditorCameraPosition (Transform transformToUse)
	{
		GKC_Utils.alignViewToObject (transformToUse);
	}

	public void addNewCameraTransformElememnt (int index)
	{
		Vector3 positionToInstantiate = transform.position;
		Quaternion rotationToInstantiate = Quaternion.identity;
	
		cameraElement currentCameraElement = lockedCameraList [index];

		int newCameraTransformElementIndex = currentCameraElement.cameraTransformList.Count + 1;

		if (currentCameraElement.cameraTransformList.Count > 0 && !useRaycastToPlaceCameras) {
			positionToInstantiate = currentCameraElement.cameraTransformList [currentCameraElement.cameraTransformList.Count - 1].cameraParentTransform.position;
			rotationToInstantiate = currentCameraElement.cameraTransformList [currentCameraElement.cameraTransformList.Count - 1].cameraParentTransform.rotation;
		} else {
			Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

			if (currentCameraEditor != null) {
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;

				RaycastHit hit;

				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceNewCamera)) {
					positionToInstantiate = hit.point + Vector3.up * 0.1f;
				}
			}
		}

		GameObject newCameraTransformElement = (GameObject)Instantiate (cameraTransformPrefab, positionToInstantiate, rotationToInstantiate);
		cameraAxisComponentsInfo newCameraAxisComponentsInfo = newCameraTransformElement.GetComponent<cameraAxisComponentsInfo> ();

		newCameraTransformElement.name = "Locked Camera Position " + newCameraTransformElementIndex;
		newCameraTransformElement.transform.SetParent (currentCameraElement.parentList);

		cameraAxis newCameraAxis = new cameraAxis ();

		newCameraAxis.name = "Camera " + (currentCameraElement.cameraTransformList.Count + 1);
		newCameraAxis.axis = newCameraAxisComponentsInfo.cameraAxisInfo.axis;
		newCameraAxis.cameraPosition = newCameraAxisComponentsInfo.cameraAxisInfo.cameraPosition;
		newCameraAxis.cameraTriggerParentTransform = newCameraAxisComponentsInfo.cameraAxisInfo.cameraTriggerParentTransform;
		newCameraAxis.triggerTransform = newCameraAxisComponentsInfo.cameraAxisInfo.triggerTransform;
		newCameraAxis.cameraParentTransform = newCameraAxisComponentsInfo.cameraAxisInfo.cameraParentTransform;

		currentCameraElement.cameraTransformList.Add (newCameraAxis);

		eventTriggerSystem cameraEventTriggerSystem = newCameraTransformElement.GetComponentInChildren<eventTriggerSystem> ();

		cameraEventTriggerSystem.eventList [0].objectToCall = gameObject;

		newCameraTransformElement.GetComponentInChildren<BoxCollider> ().transform.localScale = eventTriggerScale;

		DestroyImmediate (newCameraTransformElement.GetComponent<cameraAxisComponentsInfo> ());

		updateComponent ();
	}

	public void removeNewCameraTransformElememnt (int lockedCameraListIndex, int cameraTransformListIndex, bool updateInspector)
	{
		cameraElement currentCameraElement = lockedCameraList [lockedCameraListIndex];
		cameraAxis currentCameraAxis = currentCameraElement.cameraTransformList [cameraTransformListIndex];

		if (currentCameraAxis.cameraParentTransform != null) {
			DestroyImmediate (currentCameraAxis.cameraParentTransform.gameObject);
		}

		if (updateInspector) {
			currentCameraElement.cameraTransformList.RemoveAt (cameraTransformListIndex);

			updateComponent ();
		}
	}

	public void removeAllCameraTransformElements (int index)
	{
		for (int i = 0; i < lockedCameraList [index].cameraTransformList.Count; i++) {
			removeNewCameraTransformElememnt (index, i, false);
		}

		lockedCameraList [index].cameraTransformList.Clear ();

		updateComponent ();
	}

	public void removeAllCameraZones ()
	{
		for (int i = 0; i < lockedCameraList.Count; i++) {
			removeAllCameraTransformElements (i);
		}

		for (int i = 0; i < lockedCameraList.Count; i++) {
			removeCameraZone (i, false);
		}

		lockedCameraList.Clear ();
	}

	public void addNewCameraZone ()
	{
		GameObject newCameraZone = new GameObject ();

		newCameraZone.name = "New Camera Zone";

		cameraElement newCameraElement = new cameraElement ();

		newCameraElement.name = "New Camera Zone";
		newCameraElement.parentList = newCameraZone.transform;
		newCameraZone.transform.SetParent (transform);

		lockedCameraList.Add (newCameraElement);

		updateComponent ();
	}

	public void removeCameraZone (int index, bool updateInspector)
	{
		removeAllCameraTransformElements (index);

		if (lockedCameraList [index].parentList != null) {
			DestroyImmediate (lockedCameraList [index].parentList.gameObject);
		}

		if (updateInspector) {
			lockedCameraList.RemoveAt (index);

			updateComponent ();
		}
	}

	public void addCameraTrigger (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		Vector3 positionToInstantiate = transform.position;

		Quaternion rotationToInstantiate = Quaternion.identity;

		cameraAxis currentCameraAxis = lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];

		Vector3 cameraTriggerScale = Vector3.one;

		if (currentCameraAxis.triggerTransform.Count > 0) {
			positionToInstantiate = currentCameraAxis.triggerTransform [currentCameraAxis.triggerTransform.Count - 1].position;
			rotationToInstantiate = currentCameraAxis.triggerTransform [currentCameraAxis.triggerTransform.Count - 1].rotation;
			cameraTriggerScale = currentCameraAxis.triggerTransform [currentCameraAxis.triggerTransform.Count - 1].localScale;
		}

		GameObject newCameraTrigger = (GameObject)Instantiate (cameraTriggerPrefab, positionToInstantiate, rotationToInstantiate);

		newCameraTrigger.name = "Camera Event Trigger";

		newCameraTrigger.transform.localScale = cameraTriggerScale;
		newCameraTrigger.transform.SetParent (currentCameraAxis.cameraTriggerParentTransform);

		currentCameraAxis.triggerTransform.Add (newCameraTrigger.transform);

		eventTriggerSystem cameraEventTriggerSystem = newCameraTrigger.GetComponentInChildren<eventTriggerSystem> ();

		cameraEventTriggerSystem.eventList [0].objectToCall = gameObject;

		cameraEventTriggerSystem.eventList [0].objectToSend = currentCameraAxis.cameraPosition.gameObject;

		updateComponent ();
	}

	public void removeCameraTrigger (int lockedCameraListIndex, int cameraTransformListIndex, int cameraTriggerIndex, bool updateInspector)
	{
		cameraElement currentCameraElement = lockedCameraList [lockedCameraListIndex];

		cameraAxis currentCameraAxis = currentCameraElement.cameraTransformList [cameraTransformListIndex];

		DestroyImmediate (currentCameraAxis.triggerTransform [cameraTriggerIndex].gameObject);

		if (updateInspector) {
			currentCameraAxis.triggerTransform.RemoveAt (cameraTriggerIndex);

			updateComponent ();
		}
	}

	public void removeAllCameraTriggers (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		cameraElement currentCameraElement = lockedCameraList [lockedCameraListIndex];

		cameraAxis currentCameraAxis = currentCameraElement.cameraTransformList [cameraTransformListIndex];

		for (int i = 0; i < currentCameraAxis.triggerTransform.Count; i++) {
			removeCameraTrigger (lockedCameraListIndex, cameraTransformListIndex, i, false);
		}

		currentCameraAxis.triggerTransform.Clear ();

		updateComponent ();
	}

	public void selectCurrentTrigger (int lockedCameraListIndex, int cameraTransformListIndex, int cameraTriggerIndex)
	{
		GKC_Utils.setActiveGameObjectInEditor (lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex].triggerTransform [cameraTriggerIndex].gameObject);
	}

	public void addWaypoint (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		Vector3 positionToInstantiate = transform.position;
		Quaternion rotationToInstantiate = Quaternion.identity;
		cameraAxis currentCameraAxis = lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];

		if (currentCameraAxis.waypointList.Count > 0 && !currentCameraAxis.useRaycastToPlaceWaypoints) {
			positionToInstantiate = currentCameraAxis.waypointList [currentCameraAxis.waypointList.Count - 1].position;
			rotationToInstantiate = currentCameraAxis.waypointList [currentCameraAxis.waypointList.Count - 1].rotation;
		} else {
			Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

			if (currentCameraEditor != null) {
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;

				RaycastHit hit;

				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceNewCamera)) {
					positionToInstantiate = hit.point + Vector3.up * 0.1f;
				}
			}
		}

		GameObject newWaypoint = new GameObject ();

		newWaypoint.name = "Waypoint " + (currentCameraAxis.waypointList.Count + 1);

		newWaypoint.transform.position = positionToInstantiate;
		newWaypoint.transform.rotation = rotationToInstantiate;

		if (currentCameraAxis.waypointParentTransform == null) {
			GameObject newWaypointParent = new GameObject ();

			newWaypointParent.name = "Waypoint List Parent";

			newWaypointParent.transform.position = transform.position;
			newWaypointParent.transform.rotation = transform.rotation;
			newWaypointParent.transform.SetParent (currentCameraAxis.cameraParentTransform);
			currentCameraAxis.waypointParentTransform = newWaypointParent.transform;
		}

		newWaypoint.transform.SetParent (currentCameraAxis.waypointParentTransform);

		currentCameraAxis.waypointList.Add (newWaypoint.transform);

		updateComponent ();
	}

	public void removeWaypoint (int lockedCameraListIndex, int cameraTransformListIndex, int waypointIndex, bool updateInspector)
	{
		cameraElement currentCameraElement = lockedCameraList [lockedCameraListIndex];

		cameraAxis currentCameraAxis = currentCameraElement.cameraTransformList [cameraTransformListIndex];

		DestroyImmediate (currentCameraAxis.waypointList [waypointIndex].gameObject);

		if (updateInspector) {
			currentCameraAxis.waypointList.RemoveAt (waypointIndex);

			updateComponent ();
		}
	}

	public void removeAllWaypoints (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		cameraElement currentCameraElement = lockedCameraList [lockedCameraListIndex];

		cameraAxis currentCameraAxis = currentCameraElement.cameraTransformList [cameraTransformListIndex];

		for (int i = 0; i < currentCameraAxis.waypointList.Count; i++) {
			removeWaypoint (lockedCameraListIndex, cameraTransformListIndex, i, false);
		}

		currentCameraAxis.waypointList.Clear ();

		updateComponent ();
	}

	public void setGizmosState (bool state)
	{
		for (int i = 0; i < lockedCameraList.Count; i++) {
			lockedCameraList [i].showGizmo = state;
		}
	}

	public void setCameraParentListName (string name, int index)
	{
		lockedCameraList [index].parentList.name = name;
	}

	public void addLockedCameraPivot (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		cameraAxis currentCameraAxis = lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];

		if (currentCameraAxis.lockedCameraPivot == null) {
			GameObject newLockedCameraPivot = new GameObject ();

			newLockedCameraPivot.name = "Camera Pivot";

			newLockedCameraPivot.transform.SetParent (currentCameraAxis.cameraParentTransform);
			newLockedCameraPivot.transform.position = currentCameraAxis.axis.position;
			newLockedCameraPivot.transform.rotation = currentCameraAxis.axis.rotation;

			currentCameraAxis.lockedCameraPivot = newLockedCameraPivot.transform;

			currentCameraAxis.axis.SetParent (newLockedCameraPivot.transform);

			updateComponent ();
		}
	}

	public void addLockedCameraPivot2_5d (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		cameraAxis currentCameraAxis = lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];

		if (currentCameraAxis.lockedCameraPivot != null) {
			GameObject newLockedCameraPivot2_5d = new GameObject ();

			newLockedCameraPivot2_5d.name = "2.5d Pivot";

			newLockedCameraPivot2_5d.transform.SetParent (currentCameraAxis.lockedCameraPivot);

			newLockedCameraPivot2_5d.transform.localPosition = Vector3.zero;
			newLockedCameraPivot2_5d.transform.localRotation = Quaternion.identity;
			currentCameraAxis.pivot2_5d = newLockedCameraPivot2_5d.transform;

			GameObject newLockedCameraLookDirection2_5d = new GameObject ();

			newLockedCameraLookDirection2_5d.name = "2.5d Look Direction";

			newLockedCameraLookDirection2_5d.transform.SetParent (newLockedCameraPivot2_5d.transform);

			newLockedCameraLookDirection2_5d.transform.localPosition = Vector3.zero;
			newLockedCameraLookDirection2_5d.transform.localRotation = Quaternion.identity;
			currentCameraAxis.lookDirection2_5d = newLockedCameraLookDirection2_5d.transform;

			updateComponent ();
		}
	}

	public void addLockedCameraPivotTopDown (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		cameraAxis currentCameraAxis = lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];

		if (currentCameraAxis.lockedCameraPivot != null) {
			GameObject newLockedCameraPivotTopDown = new GameObject ();

			newLockedCameraPivotTopDown.name = "Top Down Pivot";

			newLockedCameraPivotTopDown.transform.SetParent (currentCameraAxis.lockedCameraPivot);

			newLockedCameraPivotTopDown.transform.localPosition = Vector3.zero;
			newLockedCameraPivotTopDown.transform.localRotation = Quaternion.identity;
			currentCameraAxis.topDownPivot = newLockedCameraPivotTopDown.transform;

			GameObject newLockedCameraLookDirectionTopDown = new GameObject ();

			newLockedCameraLookDirectionTopDown.name = "Top Down Look Direction";

			newLockedCameraLookDirectionTopDown.transform.SetParent (newLockedCameraPivotTopDown.transform);

			newLockedCameraLookDirectionTopDown.transform.localPosition = Vector3.zero;
			newLockedCameraLookDirectionTopDown.transform.localRotation = Quaternion.identity;
			currentCameraAxis.topDownLookDirection = newLockedCameraLookDirectionTopDown.transform;

			updateComponent ();
		}
	}

	public void setAllCamerasAsPointAndClickOrFree (bool state)
	{
		for (int i = 0; i < lockedCameraList.Count; i++) {
			for (int j = 0; j < lockedCameraList [i].cameraTransformList.Count; j++) {
				lockedCameraList [i].cameraTransformList [j].usePointAndClickSystem = state;
			}
		}

		updateComponent ();
	}

	public void setCanChangeCameraToFreeViewByInputInAllCameras (bool state)
	{
		for (int i = 0; i < lockedCameraList.Count; i++) {
			for (int j = 0; j < lockedCameraList [i].cameraTransformList.Count; j++) {
				lockedCameraList [i].cameraTransformList [j].canChangeCameraToFreeViewByInput = state;
			}
		}

		updateComponent ();
	}

	public void setAllCamerasAsTankControlsOrRegular (bool state)
	{
		for (int i = 0; i < lockedCameraList.Count; i++) {
			for (int j = 0; j < lockedCameraList [i].cameraTransformList.Count; j++) {
				lockedCameraList [i].cameraTransformList [j].useTankControls = state;
			}
		}

		updateComponent ();
	}

	public void setAllCamerasAsRelativeMovementsOrRegular (bool state)
	{
		for (int i = 0; i < lockedCameraList.Count; i++) {
			for (int j = 0; j < lockedCameraList [i].cameraTransformList.Count; j++) {
				lockedCameraList [i].cameraTransformList [j].useRelativeMovementToLockedCamera = state;
			}
		}

		updateComponent ();
	}

	public void setAllCamerasAsChangeRootMotionEnabledOrDisabled (bool state)
	{
		for (int i = 0; i < lockedCameraList.Count; i++) {
			for (int j = 0; j < lockedCameraList [i].cameraTransformList.Count; j++) {
				lockedCameraList [i].cameraTransformList [j].changeRootMotionActive = state;
			}
		}

		updateComponent ();
	}

	public void setAllCamerasAsUseRootMotionEnabledOrDisabled (bool state)
	{
		for (int i = 0; i < lockedCameraList.Count; i++) {
			for (int j = 0; j < lockedCameraList [i].cameraTransformList.Count; j++) {
				lockedCameraList [i].cameraTransformList [j].useRootMotionActive = state;
			}
		}

		updateComponent ();
	}

	public void setAllCamerasAsUseClampAimDirectionEnabledOrDisabled (bool state)
	{
		for (int i = 0; i < lockedCameraList.Count; i++) {
			for (int j = 0; j < lockedCameraList [i].cameraTransformList.Count; j++) {
				lockedCameraList [i].cameraTransformList [j].clampAimDirections = state;
			}
		}

		updateComponent ();
	}

	public void updateTriggers ()
	{
		cameraElement currentCameraElement;
		cameraAxis currentCameraAxis;

		for (int i = 0; i < lockedCameraList.Count; i++) {

			currentCameraElement = lockedCameraList [i];

			for (int j = 0; j < currentCameraElement.cameraTransformList.Count; j++) {

				currentCameraAxis = currentCameraElement.cameraTransformList [j];

				for (int k = 0; k < currentCameraAxis.triggerTransform.Count; k++) {
					if (currentCameraAxis.triggerTransform [k] != null) {
						eventTriggerSystem currenteventTriggerSystem = currentCameraAxis.triggerTransform [k].GetComponent<eventTriggerSystem> ();
						currenteventTriggerSystem.dontUseDelay = true;


						GKC_Utils.updateComponent (currenteventTriggerSystem);
					}
				}
			}
		}
	}

	public void changeViewToCameraInEditor (int lockedCameraListIndex, int cameraTransformListIndex)
	{
		if (currentPlayer != null) {
			cameraAxis currentCameraAxis = lockedCameraList [lockedCameraListIndex].cameraTransformList [cameraTransformListIndex];

			if (!currentCameraAxis.smoothTransitionAtStart) {
				currentCameraAxis.previoslyUseSmoohtTransitionState = currentCameraAxis.smoothCameraTransition;

				currentCameraAxis.smoothCameraTransition = false;
			}

			setCameraTransform (currentCameraAxis.cameraPosition.gameObject);
		}
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
		if (showGizmo && (!Application.isPlaying || showGizmoOnPlaying)) {
			cameraElement currentCameraElement;
			cameraAxis currentCameraAxis;

			for (int i = 0; i < lockedCameraList.Count; i++) {

				currentCameraElement = lockedCameraList [i];

				if (currentCameraElement.showGizmo) {
					for (int j = 0; j < currentCameraElement.cameraTransformList.Count; j++) {
						Gizmos.color = gizmoColor;

						currentCameraAxis = currentCameraElement.cameraTransformList [j];

						if (currentCameraAxis.axis != null) {
							Gizmos.DrawSphere (currentCameraAxis.axis.position, gizmoRadius);
						}

						for (int k = 0; k < currentCameraAxis.triggerTransform.Count; k++) {
							if (currentCameraAxis.triggerTransform [k] != null) {
								Gizmos.DrawLine (currentCameraAxis.axis.position, currentCameraAxis.triggerTransform [k].position);

								if (showFloorTriggersCubes) {
									Gizmos.color = floorTriggersCubesColor;
									Gizmos.DrawCube (currentCameraAxis.triggerTransform [k].position, currentCameraAxis.triggerTransform [k].localScale);
								}
							}
						}

						if (currentCameraAxis.cameraPosition != null) {
							Gizmos.DrawLine (currentCameraAxis.cameraPosition.position, currentCameraAxis.axis.position);

							GKC_Utils.drawGizmoArrow (currentCameraAxis.cameraPosition.position, 
								currentCameraAxis.cameraPosition.forward * gizmoArrowLineLength, gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);
						}

						if (currentCameraAxis.followPlayerPosition || currentCameraAxis.lookAtPlayerPosition) {
							if (currentCameraAxis.lockedCameraPivot != null) {
								Vector3 pivotPosition = currentCameraAxis.lockedCameraPivot.position +
								                        currentCameraAxis.lockedCameraPivot.up *
								                        currentCameraAxis.axis.localPosition.y;

								Gizmos.DrawLine (currentCameraAxis.axis.position, pivotPosition);
						
								Gizmos.DrawLine (pivotPosition, currentCameraAxis.lockedCameraPivot.position);
							}
						}

						if (currentCameraAxis.useBoundToFollowPlayer && currentCameraAxis.lockedCameraPivot != null) {
							drawBoundGizmo (currentCameraAxis.lockedCameraPivot.position, currentCameraAxis, boundColor);
						}

						if (showWaypointList) {
							for (int k = 0; k < currentCameraAxis.waypointList.Count; k++) {
								if (currentCameraAxis.waypointList [k] != null) {
								
									Gizmos.color = waypointListColor;
									Gizmos.DrawSphere (currentCameraAxis.waypointList [k].position, gizmoRadius);

									if (k + 1 < currentCameraAxis.waypointList.Count) {
										Gizmos.color = Color.white;
										Gizmos.DrawLine (currentCameraAxis.waypointList [k].position, currentCameraAxis.waypointList [k + 1].position);
									}
								}
							}
						}
					}
				}
			}
		}
	}


	public static void drawBoundGizmo (Vector3 currentPosition, cameraAxis currentCameraAxis, Color newBoundColor)
	{
		Vector3 depthFrontPosition = Vector3.zero;
		Vector3 depthBackwardPosition = Vector3.zero;

		bool useMoveInXAxisOn2_5d = currentCameraAxis.moveInXAxisOn2_5d || currentCameraAxis.useTopDownView;

		float heightBoundTop = currentCameraAxis.heightBoundTop;
		float widthBoundRight = currentCameraAxis.widthBoundRight;
		float widthBoundLeft = currentCameraAxis.widthBoundLeft;
		float depthBoundFront = currentCameraAxis.depthBoundFront;
		float depthBoundBackward = currentCameraAxis.depthBoundBackward;

		Vector3 horizontalRightUpperLinePosition = currentPosition + Vector3.up * heightBoundTop;
		Vector3 horizontalLeftUpperLinePosition = currentPosition + Vector3.up * heightBoundTop;
		Vector3 horizontalRightLowerLinePosition = currentPosition;
		Vector3 horizontalLeftLowerLinePosition = currentPosition;
		Vector3 verticalRightUpperLinePosition = currentPosition + Vector3.up * heightBoundTop;
		Vector3 verticalRightLowerLinePosition = currentPosition;
		Vector3 verticalLeftUpperLinePosition = currentPosition + Vector3.up * heightBoundTop;
		Vector3 verticalLefttLowerLinePosition = currentPosition;

		float totalWidth = widthBoundRight + widthBoundLeft;
		float totalheight = heightBoundTop;

		Gizmos.color = newBoundColor;

		if (useMoveInXAxisOn2_5d) {
			horizontalRightUpperLinePosition += Vector3.right * widthBoundRight;
			horizontalLeftUpperLinePosition -= Vector3.right * widthBoundLeft;
			horizontalRightLowerLinePosition += Vector3.right * widthBoundRight;
			horizontalLeftLowerLinePosition -= Vector3.right * widthBoundLeft;
			verticalRightUpperLinePosition += Vector3.right * widthBoundRight;
			verticalRightLowerLinePosition += Vector3.right * widthBoundRight;
			verticalLeftUpperLinePosition -= Vector3.right * widthBoundLeft;
			verticalLefttLowerLinePosition -= Vector3.right * widthBoundLeft;

			depthFrontPosition = currentPosition + Vector3.right * widthBoundRight / 2 - Vector3.right * widthBoundLeft / 2;
			depthFrontPosition += Vector3.up * heightBoundTop / 2;

			Gizmos.DrawCube (depthFrontPosition, new Vector3 (totalWidth, totalheight, 0));
		} else {
			horizontalRightUpperLinePosition += Vector3.forward * widthBoundRight;
			horizontalLeftUpperLinePosition -= Vector3.forward * widthBoundLeft;
			horizontalRightLowerLinePosition += Vector3.forward * widthBoundRight;
			horizontalLeftLowerLinePosition -= Vector3.forward * widthBoundLeft;
			verticalRightUpperLinePosition += Vector3.forward * widthBoundRight;
			verticalRightLowerLinePosition += Vector3.forward * widthBoundRight;
			verticalLeftUpperLinePosition -= Vector3.forward * widthBoundLeft;
			verticalLefttLowerLinePosition -= Vector3.forward * widthBoundLeft;

			depthFrontPosition = currentPosition + Vector3.forward * widthBoundRight / 2 - Vector3.forward * widthBoundLeft / 2;
			depthFrontPosition += Vector3.up * heightBoundTop / 2;

			Gizmos.DrawCube (depthFrontPosition, new Vector3 (0, totalheight, totalWidth));
		}

		Gizmos.color = Color.white;

		Gizmos.DrawLine (horizontalRightUpperLinePosition, horizontalLeftUpperLinePosition);
		Gizmos.DrawLine (horizontalRightLowerLinePosition, horizontalLeftLowerLinePosition);
	
		Gizmos.DrawLine (verticalRightUpperLinePosition, verticalRightLowerLinePosition);
		Gizmos.DrawLine (verticalLeftUpperLinePosition, verticalLefttLowerLinePosition);

		if (depthBoundFront != 0 || depthBoundBackward != 0) {
			Gizmos.color = newBoundColor / 2;

			if (useMoveInXAxisOn2_5d) {
				depthFrontPosition = currentPosition + Vector3.forward * depthBoundFront / 2 + Vector3.right * widthBoundRight / 2 - Vector3.right * widthBoundLeft / 2;
				depthFrontPosition += Vector3.up * heightBoundTop / 2;

				Gizmos.DrawCube (depthFrontPosition, new Vector3 (totalWidth, totalheight, depthBoundFront));

				depthBackwardPosition = currentPosition - Vector3.forward * depthBoundBackward / 2 + Vector3.right * widthBoundRight / 2 - Vector3.right * widthBoundLeft / 2;
				depthBackwardPosition += Vector3.up * heightBoundTop / 2;

				Gizmos.DrawCube (depthBackwardPosition, new Vector3 (totalWidth, totalheight, depthBoundBackward));
			} else {

				depthFrontPosition = currentPosition + Vector3.right * depthBoundFront / 2 + Vector3.forward * widthBoundRight / 2 - Vector3.forward * widthBoundLeft / 2;
				depthFrontPosition += Vector3.up * heightBoundTop / 2;

				Gizmos.DrawCube (depthFrontPosition, new Vector3 (depthBoundFront, totalheight, totalWidth));

				depthBackwardPosition = currentPosition - Vector3.right * depthBoundBackward / 2 + Vector3.forward * widthBoundRight / 2 - Vector3.forward * widthBoundLeft / 2;
				depthBackwardPosition += Vector3.up * heightBoundTop / 2;

				Gizmos.DrawCube (depthBackwardPosition, new Vector3 (depthBoundBackward, totalheight, totalWidth));
			}
		}
	}

	[System.Serializable]
	public class cameraElement
	{
		public string name;
		public Transform parentList;
		public List<cameraAxis> cameraTransformList = new List<cameraAxis> ();
		public bool isCurrentCameraTransform;
		public bool setCameraToFree;
		public bool showGizmo = true;
	}

	[System.Serializable]
	public class cameraAxis
	{
		public string name;
		public Transform axis;
		public Transform cameraPosition;

		public bool useDifferentCameraFov;
		public float fovValue;

		public Transform cameraTriggerParentTransform;
		public List<Transform> triggerTransform = new List <Transform> ();

		public bool followPlayerPosition;
		public bool putCameraOutsideOfPivot;
		public bool followPlayerPositionSmoothly;
		public float followPlayerPositionSpeed;
		public bool useLerpToFollowPlayerPosition;

		public bool useSeparatedVerticalHorizontalSpeed;
		public float verticalFollowPlayerPositionSpeed;
		public float horizontalFollowPlayerPositionSpeed;

		public bool lookAtPlayerPosition;
		public float lookAtPlayerPositionSpeed;
		public Transform lockedCameraPivot;

		public bool lookAtCustomTarget;
		public Transform customTargetToLook;
		public bool lookAtCustomMultipleTargets;
		public List<Transform> lookAtCustomMultipleTargetsList = new List<Transform> ();

		public bool useRotationLimits;
		public Vector2 rotationLimitsX = new Vector2 (-90, 90);
		public Vector2 rotationLimitsY = new Vector2 (-90, 90);

		public bool usePositionOffset;
		public Vector3 positionOffset;

		public bool smoothCameraTransition;
		public float cameraTransitionSpeed;
		public Transform cameraParentTransform;

		public bool startGameWithThisView;
		public GameObject playerForView;
		public bool useMultiplePlayers;
		public List<GameObject> playerForViewList = new List<GameObject> ();
		public bool smoothTransitionAtStart;
		public bool previoslyUseSmoohtTransitionState;

		public bool cameraCanRotate;

		public Vector2 rangeAngleX = new Vector2 (-90, 90);
		public Vector2 rangeAngleY = new Vector2 (-90, 90);

		public bool useSpringRotation;
		public float springRotationDelay;

		public bool smoothRotation;
		public float rotationSpeed;

		public float originalPivotRotationY;
		public float originalCameraRotationX;

		public bool canUseZoom;
		public float zoomValue;

		public bool useZoomByMovingCamera;
		public Vector2 zoomCameraOffsets;
		public float zoomByMovingCameraSpeed;
		public float zoomByMovingCameraAmount = 1;
		public bool zoomByMovingCameraDirectlyProportional = true;

		public bool canMoveCamera;

		public float moveCameraSpeed;
		public Vector2 moveCameraLimitsX = new Vector2 (-2, 2);
		public Vector2 moveCameraLimitsY = new Vector2 (-2, 2);
		public bool smoothCameraMovement;
		public float smoothCameraSpeed;
		public bool useSpringMovement;
		public float springMovementDelay;

		public Vector3 originalCameraAxisLocalPosition;
		public Vector3 originalLockedCameraPivotPosition;
		public Vector3 originalCameraLocalPosition;

		public Vector3 originalCameraForward;

		public bool followFixedCameraPosition;

		public bool changeLockedCameraDirectionOnInputReleased = true;

		//Waypoints variables
		public bool useWaypoints;
		public bool useRaycastToPlaceWaypoints;

		public List<Transform> waypointList = new List<Transform> ();
		public Transform waypointParentTransform;

		public bool useSpline;
		public BezierSpline mainSpline;

		public float splineAccuracy = 100;

		public bool useZeroCameraTransition;

		public bool useHorizontalOffsetOnFaceSide;
		public float horizontalOffsetOnFaceSide;
		public float inputToleranceOnFaceSide;
		public float horizontalOffsetOnFaceSideSpeed;

		public bool useHorizontalOffsetOnFaceSideOnMoving;
		public float horizontalOffsetOnFaceSideOnMoving;
		public float inputToleranceOnFaceSideOnMoving;
		public float horizontalOffsetOnFaceSideOnMovingSpeed;
	
		public bool useBoundToFollowPlayer;
		public float heightBoundTop;
		public float widthBoundRight;
		public float widthBoundLeft;
		public float depthBoundFront;
		public float depthBoundBackward;
		public Vector3 boundOffset;

		public bool useTankControls;

		public bool playerCanMoveOnAimInTankMode = true;
	
		//2.5d variables
		public bool use2_5dView;
		public Transform pivot2_5d;
		public Transform lookDirection2_5d;
		public bool useDefaultZValue2_5d;
		public bool moveInXAxisOn2_5d;

		public bool reverseMovementDirection;

		public Vector2 moveCameraLimitsX2_5d = new Vector2 (-3, 3);
		public Vector2 moveCameraLimitsY2_5d = new Vector2 (-3, 3);

		public bool use2_5dVerticalOffsetOnMove;
		public float verticalTopOffsetOnMove;
		public float verticalBottomOffsetOnMove;
		public float verticalOffsetOnMoveSpeed;

		public bool clampAimDirections;
		public numberOfAimDirection numberOfAimDirections = numberOfAimDirection.eight;

		public enum numberOfAimDirection
		{
			eight = 8,
			four = 4,
			two = 2
		}

		public bool useCircleCameraLimit2_5d;
		public float circleCameraLimit2_5d;
		public bool scaleCircleCameraLimitWithDistanceToCamera2_5d;
		public Vector2 circleCameraLimitScaleClamp2_5d;

		public float adjustPlayerPositionToFixed2_5dPosition = 5;

		public bool moveReticleInFixedCircleRotation2_5d = true;

		//Top down variables
		public bool useTopDownView;
		public Transform topDownPivot;
		public Transform topDownLookDirection;

		public float extraTopDownYRotation;

		public Vector2 moveCameraLimitsXTopDown = new Vector2 (-4, 4);
		public Vector2 moveCameraLimitsYTopDown = new Vector2 (-3, 3);
		public bool use8DiagonalAim;
		public bool useRelativeMovementToLockedCamera = true;

		public bool useCircleCameraLimitTopDown;
		public float circleCameraLimitTopDown;
		public bool scaleCircleCameraLimitWithDistanceToCamera;
		public Vector2 circleCameraLimitScaleClamp;
		public bool moveReticleInFixedCircleRotationTopDown = true;

		public bool changeRootMotionActive;
		public bool useRootMotionActive = true;

		public bool canRotateCameraHorizontally;
		public float horizontalCameraRotationSpeed;
		public bool useMouseInputToRotateCameraHorizontally;
		public bool useFixedRotationAmount;
		public float fixedRotationAmountToRight;
		public float fixedRotationAmountToLeft;
		public float fixedRotationAmountSpeed;

		public bool checkPossibleTargetsBelowCursor = true;

		public bool showCameraCursorWhenNotAiming;

		public bool useCustomPivotHeightOffset;
		public float customPivotOffset = 1.52f;

		public float reticleOffset;

		//Look at target variables
		public bool useAimAssist = true;
		public bool lookOnlyIfTargetOnScreen;
		public bool lookOnlyIfTargetVisible;
		public float moveCameraCursorSpeed = 0.1f;

		public bool canUseManualLockOn;

		public bool setDeactivateRootMotionOnStrafeActiveOnLockedViewValue;
		public bool deactivateRootMotionOnStrafeActiveOnLockedView;

		public bool useOldSchoolAim;
		public float aimOffsetUp;
		public float aimOffsetDown;
		public float aimOffsetChangeSpeed;

		public bool setCanMoveWhileAimLockedCameraState;
		public bool canMoveWhileAimLockedCamera;

		//Point and click varaibles
		public bool usePointAndClickSystem;

		//Event variables
		public bool useUnityEvent;
		public bool useUnityEventOnEnter;
		public UnityEvent unityEventOnEnter;
		public bool useUnityEventOnExit;
		public UnityEvent unityEventOnExit;

		public bool useTransparetSurfaceSystem;

		public bool useDistanceToTransformToMoveCameraCloserOrFarther;
		public Transform transformToCalculateDistance;
		public float moveCameraCloserOrFartherSpeed;
		public bool useDistanceDirectlyProportional;

		public bool useClampCameraForwardDirection;

		public bool useClampCameraBackwardDirection;
		public float clampCameraBackwardDirection;

		public float cameraDistanceMultiplier = 1;

		public bool useCameraLimitSystem;
		public camera2_5dZoneLimitSystem mainCamera2_5dZoneLimitSystem;
		public bool disablePreviousCameraLimitSystem;

		public bool canChangeCameraToFreeViewByInput;

		public bool rotatePlayerToward2dCameraOnTriggerEnter;
	}
}