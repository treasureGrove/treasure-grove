using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class craftingPlacementSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool placementEnabled = true;

	public LayerMask layerToPlaceObjects;

	public LayerMask layerToTakeObjectsBack;

	public float maxRaycastDistance;

	public bool checkForObstacles;

	public LayerMask layerForObstacles;

	public float hitPointOffset;

	[Space]
	[Header ("Input To Pause On Placement Active Settings")]
	[Space]

	public List<playerActionSystem.inputToPauseOnActionIfo> customInputToPauseOnActionInfoList = new List<playerActionSystem.inputToPauseOnActionIfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool placementActive;

	public GameObject currentObjectToPlaceMesh;

	public float currentObjectRighRotation;

	public float currentObjectForwardRotation;

	public bool objectTakenBack;
	public string lastObjectTakenBackName;

	public bool placingObjectPreviouslyTaken;

	public LayerMask currentLayerMaskToPlaceObject;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnEnablePlacement;
	public UnityEvent eventOnDisablePlacement;

	[Space]
	[Header ("Components")]
	[Space]

	public craftingSystem mainCraftingSystem;

	public Transform mainCameraTransform;

	public Collider mainPlayerCollider;

	public playerInputManager mainPlayerInputManager;

	public GameObject playerControllerGameObject;


	RaycastHit hit;

	Coroutine updateCoroutine;

	bool currentObjectAssigned;

	Bounds currentBounds;

	Vector3 objectToPlacePositionOffset;

	bool attachObjectToSurfaceActive;

	bool useCustomLayerMaskToPlaceObject;

	bool objectCanBeRotatedOnYAxis;
	bool objectCanBeRotatedOnXAxis;


	craftingSocket outputSocket;

	craftingSocket inputSocket;

	bool connectingStationsActive;

	craftingStationSystem firstStationToConnectSelected;
	craftingStationSystem secondStationToConnectSelected;


	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForSecondsRealtime (0.0001f);

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}


	void updateSystem ()
	{
		if (currentObjectAssigned) {
			bool surfaceFound = false;

			Vector3 raycastPosition = mainCameraTransform.position;
			Vector3 raycastDirection = mainCameraTransform.forward;

			if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxRaycastDistance, currentLayerMaskToPlaceObject)) {
				if (hit.collider != mainPlayerCollider) {
					surfaceFound = true;
				} else {
					raycastPosition = hit.point + raycastDirection * 0.2f;

					if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxRaycastDistance, currentLayerMaskToPlaceObject)) {
						surfaceFound = true;
					}
				}
			} 

			if (surfaceFound) {
				currentObjectToPlaceMesh.transform.position = hit.point;

				currentObjectToPlaceMesh.transform.position += hit.normal * hitPointOffset + objectToPlacePositionOffset;

//				(hit.normal * (hitPointOffset + currentBounds.extents.y / 2));
//				currentObjectToPlaceMesh.transform.position += Vector3.Cross (hit.normal, currentBounds.center);

				if (!currentObjectToPlaceMesh.gameObject.activeSelf) {
					currentObjectToPlaceMesh.gameObject.SetActive (true);
				}

			} else {
				if (currentObjectToPlaceMesh.gameObject.activeSelf) {
					currentObjectToPlaceMesh.gameObject.SetActive (false);
				}
			}
		}

		if (connectingStationsActive) {
			if (outputSocket != null) {
				Vector3 raycastPosition = mainCameraTransform.position + mainCameraTransform.forward * 4;

				outputSocket.setLineRendererTargetPosition (raycastPosition);
			}
		}
	}

	public void confirmToPlaceObject ()
	{
		if (!currentObjectAssigned) {
			return;
		}

		string currentObjectName = mainCraftingSystem.getCurrentObjectSelectedName ();

		if (placingObjectPreviouslyTaken) {
			currentObjectName = lastObjectTakenBackName;
		}

		mainCraftingSystem.removeObjectAmountFromInventoryByName (currentObjectName, 1);

		GameObject currentObjectToPlace = null;

		if (placingObjectPreviouslyTaken) {
			currentObjectToPlace = mainCraftingSystem.getCurrentObjectToPlaceByName (currentObjectName);
		} else {
			currentObjectToPlace = mainCraftingSystem.getCurrentObjectToPlace ();
		}

		if (currentObjectToPlace != null) {
			GameObject newObject = Instantiate (currentObjectToPlace, null);

			newObject.transform.position = currentObjectToPlaceMesh.transform.position;
			newObject.transform.rotation = currentObjectToPlaceMesh.transform.rotation;

			craftedObjectPlacedHelper currentCraftedObjectPlacedHelper = newObject.GetComponent<craftedObjectPlacedHelper> ();

			if (currentCraftedObjectPlacedHelper == null) {
				currentCraftedObjectPlacedHelper = newObject.AddComponent<craftedObjectPlacedHelper> ();

				currentCraftedObjectPlacedHelper.mainGameObject = newObject;
				currentCraftedObjectPlacedHelper.objectName = currentObjectName;
			}

			if (currentCraftedObjectPlacedHelper != null) {
				currentCraftedObjectPlacedHelper.checkEventsOnStateChange (true);

				currentCraftedObjectPlacedHelper.checkEventsOnStateChangeWithPlayer (true, playerControllerGameObject);

				currentCraftedObjectPlacedHelper.setObjectPlacedState (true);
			}

			if (attachObjectToSurfaceActive) {
				LayerMask layerMaskToAttachObject = mainCraftingSystem.getCurrentObjectLayerMaskToAttachObjectByName (currentObjectName);

				if (layerMaskToAttachObject != null) {
					bool surfaceFound = false;

					Vector3 raycastPosition = mainCameraTransform.position;
					Vector3 raycastDirection = mainCameraTransform.forward;

					if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxRaycastDistance, layerMaskToAttachObject)) {
						if (hit.collider != mainPlayerCollider) {
							surfaceFound = true;
						} else {
							raycastPosition = hit.point + raycastDirection * 0.2f;

							if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxRaycastDistance, layerMaskToAttachObject)) {
								surfaceFound = true;
							}
						}
					} 

					if (surfaceFound) {
						if (!applyDamage.setObjectParentOnDetectedSurface (newObject.transform, true, hit.collider.transform, true)) {
							newObject.transform.SetParent (hit.collider.transform);

							Collider objectCollider = newObject.GetComponent<Collider> ();

							if (objectCollider != null) {
								if (newObject.transform.parent != null) {
									Collider surfaceCollider = newObject.transform.parent.GetComponent<Collider> ();

									if (surfaceCollider != null) {
										Physics.IgnoreCollision (objectCollider, surfaceCollider, true);
									}
								}
							}
						}

						Rigidbody objectRigidbody = newObject.GetComponent<Rigidbody> ();

						if (objectRigidbody != null) {
							objectRigidbody.isKinematic = true;
						}
					}
				}
			}
		}

		stopUpdateCoroutine ();

		if (currentObjectToPlaceMesh != null) {
			Destroy (currentObjectToPlaceMesh);

			currentObjectAssigned = false;
		}
	}

	public void setCurrentObjectToPlaceMesh (GameObject newObject)
	{
		if (newObject == null) {
			return;
		}

		currentObjectToPlaceMesh = Instantiate (newObject, null);

		string currentObjectName = mainCraftingSystem.getCurrentObjectSelectedName ();

		objectToPlacePositionOffset = mainCraftingSystem.getCurrentObjectToPlacePositionOffsetByName (currentObjectName);

		currentObjectAssigned = currentObjectToPlaceMesh != null;

		currentLayerMaskToPlaceObject = layerToPlaceObjects;

		useCustomLayerMaskToPlaceObject = mainCraftingSystem.checkIfCurrentObjectToPlaceUseCustomLayerMaskByName (currentObjectName);

		if (useCustomLayerMaskToPlaceObject) {
			currentLayerMaskToPlaceObject = mainCraftingSystem.getCurrentObjectCustomLayerMaskToPlaceObjectByName (currentObjectName);
		}

		mainCraftingSystem.getCurrentObjectCanBeRotatedValuesByName (currentObjectName, ref objectCanBeRotatedOnYAxis, ref objectCanBeRotatedOnXAxis);
		 

		if (currentObjectAssigned) {

			currentBounds = new Bounds (Vector3.zero, Vector3.one);

			Component[] components = currentObjectToPlaceMesh.GetComponentsInChildren (typeof(Renderer));
			foreach (Renderer child in components) {
				if (child != null) {
					currentBounds.Encapsulate (child.bounds);
				}
			}
		}
	}


	public void setPlacementActiveState (bool state)
	{
		if (!placementEnabled) {
			return;
		}

		if (placementActive == state) {
			return;
		}

		placementActive = state;

		checkEventsOnStateChange (placementActive);

		currentObjectRighRotation = 0;

		currentObjectForwardRotation = 0;

		checkInputListToPauseDuringAction (placementActive);

		objectTakenBack = false;

		placingObjectPreviouslyTaken = false;

		if (placementActive) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());

		} else {
			stopUpdateCoroutine ();

			if (currentObjectToPlaceMesh != null) {
				Destroy (currentObjectToPlaceMesh);

				currentObjectAssigned = false;
			}
		}

		craftingSocket[] craftingSocketList = FindObjectsOfType<craftingSocket> ();

		foreach (craftingSocket currentCraftingSocket in craftingSocketList) {
			if (placementActive) {
				currentCraftingSocket.checkIfSocketAssigned ();

				currentCraftingSocket.enableLineRendererIfSocketAssigned ();

				currentCraftingSocket.updateLinerenderPositions ();
			} else {
				currentCraftingSocket.enableOrDisableLineRenderer (false);
			}
		}

		firstStationToConnectSelected = null;
		secondStationToConnectSelected = null;

		connectingStationsActive = false;
	}

	void checkEventsOnStateChange (bool state)
	{
		if (state) {
			eventOnEnablePlacement.Invoke ();
		} else {
			eventOnDisablePlacement.Invoke ();
		}
	}

	public void checkInputListToPauseDuringAction (bool state)
	{
		for (int i = 0; i < customInputToPauseOnActionInfoList.Count; i++) {
			if (state) {
				customInputToPauseOnActionInfoList [i].previousActiveState = mainPlayerInputManager.setPlayerInputMultiAxesStateAndGetPreviousState (false, customInputToPauseOnActionInfoList [i].inputName);
			} else {
				if (customInputToPauseOnActionInfoList [i].previousActiveState) {
					mainPlayerInputManager.setPlayerInputMultiAxesState (customInputToPauseOnActionInfoList [i].previousActiveState, customInputToPauseOnActionInfoList [i].inputName);
				}
			}
		}
	}

	//Input Functions
	public void inputRotateObjectToRightOrLeft (bool state)
	{
		if (!placementEnabled) {
			return;
		}

		if (!placementActive) {
			return;
		}

		if (!currentObjectAssigned) {
			return;
		}

		if (state) {
			currentObjectRighRotation += 90;
		} else {
			currentObjectRighRotation -= 90;
		}

		if (currentObjectRighRotation < 0) {
			currentObjectRighRotation = 270;
		}

		if (currentObjectRighRotation > 360) {
			currentObjectRighRotation = 0;
		}

		if (currentObjectToPlaceMesh != null) {
			currentObjectToPlaceMesh.transform.eulerAngles = Vector3.up * currentObjectRighRotation;
		}
	}

	public void inputRotateObjectToUpOrDown (bool state)
	{
		if (!placementEnabled) {
			return;
		}

		if (!placementActive) {
			return;
		}

		if (!currentObjectAssigned) {
			return;
		}

		if (state) {
			currentObjectForwardRotation += 90;
		} else {
			currentObjectForwardRotation -= 90;
		}

		if (currentObjectForwardRotation < 0) {
			currentObjectForwardRotation = 270;
		}

		if (currentObjectForwardRotation > 360) {
			currentObjectForwardRotation = 0;
		}

		if (currentObjectToPlaceMesh != null) {
			currentObjectToPlaceMesh.transform.eulerAngles = Vector3.right * currentObjectForwardRotation;
		}
	}

	public void inputConfirmToPlaceObject ()
	{
		if (!placementEnabled) {
			return;
		}

		if (!placementActive) {
			return;
		}

		if (objectTakenBack) {
			GameObject currentObjectMesh = mainCraftingSystem.getInventoryMeshByName (lastObjectTakenBackName);

			if (currentObjectMesh != null) {
				setCurrentObjectToPlaceMesh (currentObjectMesh);
			} 

			placingObjectPreviouslyTaken = true;

			objectTakenBack = false;

			return;
		}

		if (!currentObjectAssigned) {
			return;
		}

		confirmToPlaceObject ();
	}

	public void inputConfirmToTakeObjectBackToInventory ()
	{
		if (!placementEnabled) {
			return;
		}

		if (!placementActive) {
			return;
		}

		bool surfaceFound = false;

		if (!currentObjectAssigned) {
			currentLayerMaskToPlaceObject = layerToTakeObjectsBack;
		}

		Vector3 raycastPosition = mainCameraTransform.position;
		Vector3 raycastDirection = mainCameraTransform.forward;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxRaycastDistance, currentLayerMaskToPlaceObject)) {
			if (hit.collider != mainPlayerCollider) {
				surfaceFound = true;
			} else {
				raycastPosition = hit.point + raycastDirection * 0.2f;

				if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxRaycastDistance, currentLayerMaskToPlaceObject)) {
					surfaceFound = true;
				}
			}
		} 

		if (showDebugPrint) {
			print ("surfaceFound result " + surfaceFound);
		}

		if (surfaceFound) {
			craftedObjectPlacedHelper currentCraftedObjectPlacedHelper = hit.collider.GetComponent<craftedObjectPlacedHelper> ();

			if (currentCraftedObjectPlacedHelper != null) {
				checkObjectToTakeBack (currentCraftedObjectPlacedHelper);

				return;
			}

			healthManagement currentHealthManagement = hit.collider.GetComponent<healthManagement> ();

			if (currentHealthManagement != null) {
				GameObject currentMainObject = currentHealthManagement.getCharacterOrVehicleWithHealthManagement ();

				if (currentMainObject != null) {
					currentCraftedObjectPlacedHelper = currentMainObject.GetComponent<craftedObjectPlacedHelper> ();

					if (currentCraftedObjectPlacedHelper != null) {
						checkObjectToTakeBack (currentCraftedObjectPlacedHelper);
					}
				}
			}
		}
	}

	void checkObjectToTakeBack (craftedObjectPlacedHelper currentCraftedObjectPlacedHelper)
	{
		if (!currentCraftedObjectPlacedHelper.isObjectCanBeTakenBackToInventoryEnabled ()) {
			return;
		}

		if (!currentCraftedObjectPlacedHelper.isObjectPlaced ()) {
			return;
		}

		if (showDebugPrint) {
			print ("crafted object located");
		}

		string objectName = currentCraftedObjectPlacedHelper.getObjectName ();

		mainCraftingSystem.giveInventoryObjectToCharacter (objectName, 1);

		currentCraftedObjectPlacedHelper.checkEventsOnStateChange (false);

		currentCraftedObjectPlacedHelper.checkEventsOnStateChangeWithPlayer (false, playerControllerGameObject);

		currentCraftedObjectPlacedHelper.checkStateOnTakeObjectBack ();

		GameObject currentDetectedObject = currentCraftedObjectPlacedHelper.getObject ();

		if (currentDetectedObject != null) {
			checkToRemoveStation (currentDetectedObject);

			Destroy (currentDetectedObject);
		}

		objectTakenBack = true;

		lastObjectTakenBackName = objectName;
	}

	public void inputCancelPlaceObject ()
	{
		if (!placementEnabled) {
			return;
		}

		if (!placementActive) {
			return;
		}

		if (currentObjectAssigned) {
			stopUpdateCoroutine ();

			if (currentObjectToPlaceMesh != null) {
				Destroy (currentObjectToPlaceMesh);

				currentObjectAssigned = false;
			}

			return;
		}


		mainCraftingSystem.setPlacementActiveState (false);
	}

	public void inputConfirmToAttachObjectToSurfaceDetected ()
	{
		if (!objectTakenBack) {
			attachObjectToSurfaceActive = true;

			inputConfirmToPlaceObject ();

			attachObjectToSurfaceActive = false;
		} else {
			inputConfirmToPlaceObject ();
		}
	}

	public void inputCheckConnectStationInput ()
	{
		if (!placementEnabled) {
			return;
		}

		if (!placementActive) {
			return;
		}

		bool surfaceFound = false;

		Vector3 raycastPosition = mainCameraTransform.position;
		Vector3 raycastDirection = mainCameraTransform.forward;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxRaycastDistance, layerToPlaceObjects)) {
			if (hit.collider != mainPlayerCollider) {
				surfaceFound = true;
			} else {
				raycastPosition = hit.point + raycastDirection * 0.2f;

				if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxRaycastDistance, layerToPlaceObjects)) {
					surfaceFound = true;
				}
			}
		} 

		if (showDebugPrint) {
			print ("surfaceFound result " + surfaceFound);
		}

		if (surfaceFound) {
			craftedObjectPlacedHelper currentCraftedObjectPlacedHelper = hit.collider.GetComponent<craftedObjectPlacedHelper> ();

			if (currentCraftedObjectPlacedHelper != null) {
				GameObject craftingStationDetected = currentCraftedObjectPlacedHelper.getObject ();

				if (craftingStationDetected != null) {
					if (firstStationToConnectSelected == null) {
						firstStationToConnectSelected = craftingStationDetected.GetComponent<craftingStationSystem> ();

						if (firstStationToConnectSelected != null) {

							if (showDebugPrint) {
								print ("first station detected");
							}

							outputSocket = firstStationToConnectSelected.outputSocket;

							outputSocket.enableOrDisableLineRenderer (true);

							connectingStationsActive = true;

							return;
						}
					}

					if (secondStationToConnectSelected == null) {
						secondStationToConnectSelected = craftingStationDetected.GetComponent<craftingStationSystem> ();

						if (secondStationToConnectSelected != null) {
							if (showDebugPrint) {
								print ("second station detected");
							}

							bool connectResult = false;

							if (firstStationToConnectSelected.stationToConnectNameList.Contains (secondStationToConnectSelected.getStationName ()) ||
							    secondStationToConnectSelected.stationToConnectNameList.Contains (firstStationToConnectSelected.getStationName ())) {
								connectResult = true;
							}

							if (showDebugPrint) {
								print ("connectResult value is " + connectResult);
							}

							if (connectResult) {
								outputSocket = firstStationToConnectSelected.outputSocket;

								inputSocket = secondStationToConnectSelected.inputSocket;

								if (outputSocket.currentCraftingStationSystemAssigned != null) {
									firstStationToConnectSelected.checkStateOnRemoveOuput ();

									secondStationToConnectSelected.checkStateOnRemoveInput ();

									outputSocket.enableOrDisableLineRenderer (false);

									outputSocket.assignCraftingStationSystem (null);
								}


								outputSocket.assignCraftingStationSystem (secondStationToConnectSelected);

								outputSocket.enableLineRendererIfSocketAssigned ();

								outputSocket.setLineRendererTargetPosition (inputSocket.transform);

								firstStationToConnectSelected.checkStateOnSetOuput ();

								secondStationToConnectSelected.checkStateOnSetInput ();

								firstStationToConnectSelected.addCraftingStationSystem (secondStationToConnectSelected);

								secondStationToConnectSelected.addCraftingStationSystem (firstStationToConnectSelected);
							}


							firstStationToConnectSelected = null;
							secondStationToConnectSelected = null;

							connectingStationsActive = false;

							if (showDebugPrint) {
								print ("stations connected");
							}
						}
					}
				}
			}
		}
	}

	public void inputCheckRemoveStationInput ()
	{
		if (!placementEnabled) {
			return;
		}

		if (!placementActive) {
			return;
		}

		if (connectingStationsActive) {
			if (outputSocket != null) {

				outputSocket.enableOrDisableLineRenderer (false);
			}

			outputSocket = null;

			connectingStationsActive = true;

			firstStationToConnectSelected = null;
			secondStationToConnectSelected = null;

			connectingStationsActive = false;

			return;
		}

		bool surfaceFound = false;

		Vector3 raycastPosition = mainCameraTransform.position;
		Vector3 raycastDirection = mainCameraTransform.forward;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxRaycastDistance, layerToPlaceObjects)) {
			if (hit.collider != mainPlayerCollider) {
				surfaceFound = true;
			} else {
				raycastPosition = hit.point + raycastDirection * 0.2f;

				if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxRaycastDistance, layerToPlaceObjects)) {
					surfaceFound = true;
				}
			}
		} 

		if (showDebugPrint) {
			print ("surfaceFound result " + surfaceFound);
		}

		if (surfaceFound) {
			checkToRemoveStation (hit.collider.gameObject);
		}
	}

	void checkToRemoveStation (GameObject objectToCheck)
	{
		craftedObjectPlacedHelper currentCraftedObjectPlacedHelper = objectToCheck.GetComponent<craftedObjectPlacedHelper> ();

		if (currentCraftedObjectPlacedHelper != null) {
			GameObject craftingStationDetected = currentCraftedObjectPlacedHelper.getObject ();

			if (craftingStationDetected != null) {

				firstStationToConnectSelected = craftingStationDetected.GetComponent<craftingStationSystem> ();

				if (firstStationToConnectSelected != null) {
					bool checkStateOnRemoveInputFound = false;

					for (int i = 0; i < firstStationToConnectSelected.craftingStationSystemConnectedList.Count; i++) {
						craftingStationSystem currentCraftingStationSystem = firstStationToConnectSelected.craftingStationSystemConnectedList [i];

						print ("station to check " + currentCraftingStationSystem.stationName);

						if (currentCraftingStationSystem.craftingStationSystemConnectedList.Contains (firstStationToConnectSelected)) {
	
							print ("contains this station");

							outputSocket = currentCraftingStationSystem.outputSocket;

							if (outputSocket.currentCraftingStationSystemAssigned == firstStationToConnectSelected) {
								print ("removing");

								currentCraftingStationSystem.checkStateOnRemoveOuput ();

								currentCraftingStationSystem.removeCraftingStationSystem (firstStationToConnectSelected);
								
								outputSocket.enableOrDisableLineRenderer (false);

								outputSocket.assignCraftingStationSystem (null);

								if (!checkStateOnRemoveInputFound) {
									firstStationToConnectSelected.checkStateOnRemoveInput ();

									checkStateOnRemoveInputFound = false;
								}
							}
						}
					}

					outputSocket = firstStationToConnectSelected.outputSocket;

					if (outputSocket.currentCraftingStationSystemAssigned != null) {
						firstStationToConnectSelected.checkStateOnRemoveOuput ();

						secondStationToConnectSelected = outputSocket.currentCraftingStationSystemAssigned;

						secondStationToConnectSelected.checkStateOnRemoveInput ();

						outputSocket.enableOrDisableLineRenderer (false);

						outputSocket.assignCraftingStationSystem (null);
					}

					firstStationToConnectSelected.clearCraftingStationSystemList ();

					firstStationToConnectSelected = null;
					secondStationToConnectSelected = null;
				}
			}
		}
	}
}
