using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class moveDeviceToCamera : MonoBehaviour
{
	public GameObject deviceGameObject;
	public float distanceFromCamera;

	public bool rotateObjectOnCameraDirectionEnabled = true;

	public bool smoothCameraMovement = true;

	public bool useFixedLerpMovement = true;
	public float fixedLerpMovementSpeed = 2;

	public float cameraMovementSpeedThirdPerson = 2;
	public float cameraMovementSpeedFirstPerson = 1;

	public bool setNewMouseCursorControllerSpeed;
	public float newMouseCursroControllerSpeed;

	public float maxZoomDistance;
	public float minZoomDistance;
	public float zoomSpeed;

	public string layerToExaminateDevices;

	public bool activateExaminateObjectSystem;

	public bool objectHasActiveRigidbody;

	public bool disablePlayerMeshGameObject = true;

	public bool keepWeaponsIfCarrying;
	public bool drawWeaponsIfPreviouslyCarrying;
	public bool keepOnlyIfPlayerIsOnFirstPerson;
	public bool disableWeaponsDirectlyOnStart;
	bool carryingWeaponsPreviously;
	bool firstPersonActive;

	public Collider deviceTrigger;

	public bool useListOfDisabledObjects;
	public List<GameObject> disabledObjectList = new List<GameObject> ();

	public List<Collider> colliderListToDisable = new List<Collider> ();
	public List<Collider> colliderListButtons = new List<Collider> ();

	public bool ignoreDeviceTriggerEnabled;

	public bool useBlurUIPanel;

	public bool disableSecondaryPlayerHUD = true;
	public bool disableAllPlayerHUD;

	public examineObjectSystem examineObjectManager;
	public Rigidbody mainRigidbody;

	public bool disableInteractionTouchButtonOnUsingDevice = true;

	float originalDistanceFromCamera;

	Vector3 devicePositionTarget;
	Quaternion deviceRotationTarget;

	Transform originalDeviceParentTransform;
	Coroutine cameraState;

	Transform deviceTransform;

	bool deviceEnabled;
	Camera mainCamera;
	menuPause pauseManager;
	GameObject currentPlayer;
	playerController currentPlayerControllerManager;
	playerWeaponsManager weaponsManager;
	usingDevicesSystem usingDevicesManager;

	bool previouslyIconButtonActive;


	Vector3 originalPosition;
	Quaternion originalRotation;
	List<layerInfo> layerList = new List<layerInfo> ();
	bool previouslyActivated;

	headBob headBobManager;
	footStepManager stepManager;

	bool originalKinematicValue;
	bool originalUseGravityValue;
	Collider playerCollider;

	GameObject examineObjectRenderTexturePanel;
	Transform examineObjectBlurPanelParent;

	playerComponentsManager mainPlayerComponentsManager;

	bool ignoreObjectRotation = false;

	public bool hideMouseCursorIfUsingGamepad;
	bool previousCursorVisibleState;


	void Start ()
	{
		if (deviceGameObject == null) {
			deviceGameObject = gameObject;
		}

		deviceTransform = deviceGameObject.transform;

		originalPosition = deviceTransform.localPosition;
		originalRotation = deviceTransform.localRotation;

		originalDeviceParentTransform = deviceTransform.parent;

		setLayerList ();

		originalDistanceFromCamera = distanceFromCamera;

		if (activateExaminateObjectSystem) {
			if (examineObjectManager == null) {
				examineObjectManager = GetComponent<examineObjectSystem> ();
			}
		}

		if (objectHasActiveRigidbody) {
			if (mainRigidbody == null) {
				mainRigidbody = deviceGameObject.GetComponent<Rigidbody> ();
			}
		}
	}


	//activate the device
	public void moveCamera (bool state)
	{
		deviceEnabled = state;

		if (deviceEnabled) {
			headBobManager.stopAllHeadbobMovements ();
		}

		headBobManager.playOrPauseHeadBob (!state);


		//if the player is using the computer, disable the player controller, the camera, and set the parent of the camera inside the computer, 
		//to move to its view position

		if (deviceEnabled) {
			if (currentPlayerControllerManager.isPlayerRunning ()) {
				currentPlayerControllerManager.stopRun ();
			}

			//make the mouse cursor visible according to the action of the player
			currentPlayerControllerManager.setUsingDeviceState (deviceEnabled);

			weaponsManager.setUsingDeviceState (deviceEnabled);

			if (keepWeaponsIfCarrying) {
				firstPersonActive = currentPlayerControllerManager.isPlayerOnFirstPerson ();

				if (!keepOnlyIfPlayerIsOnFirstPerson || firstPersonActive) {
					carryingWeaponsPreviously = weaponsManager.isUsingWeapons ();

					if (carryingWeaponsPreviously) {
						if (disableWeaponsDirectlyOnStart) {
							weaponsManager.checkIfDisableCurrentWeapon ();
						} else {
							weaponsManager.checkIfKeepSingleOrDualWeapon ();
						}
					}
				}
			}

			pauseManager.usingDeviceState (deviceEnabled);
			currentPlayerControllerManager.changeScriptState (!deviceEnabled);

			if (disablePlayerMeshGameObject) {
				currentPlayerControllerManager.getGravityCenter ().gameObject.SetActive (!deviceEnabled);
			}

			stepManager.enableOrDisableFootStepsComponents (!deviceEnabled);

			pauseManager.showOrHideCursor (deviceEnabled);

			if (hideMouseCursorIfUsingGamepad) {
				if (pauseManager.isUsingGamepad ()) {
					previousCursorVisibleState = menuPause.isCursorVisible ();

					menuPause.setCursorVisibleState (false);
				}
			}

			pauseManager.changeCameraState (!deviceEnabled);

			previouslyIconButtonActive = usingDevicesManager.getCurrentIconButtonState ();

			if (disableInteractionTouchButtonOnUsingDevice) {
				usingDevicesManager.setIconButtonCanBeShownState (false);
			}

			distanceFromCamera = originalDistanceFromCamera;

			if (objectHasActiveRigidbody) {
				deviceTransform.SetParent (null);

				originalPosition = deviceTransform.localPosition;
				originalRotation = deviceTransform.localRotation;

				originalDeviceParentTransform = null;
			}

			deviceTransform.SetParent (mainCamera.transform);

			devicePositionTarget = Vector3.zero + Vector3.forward * distanceFromCamera;
			deviceRotationTarget = Quaternion.identity;

			setColliderListState (!deviceEnabled);

			setLayerListState (!deviceEnabled);

			usingDevicesManager.setExamineteDevicesCameraState (deviceEnabled, useBlurUIPanel);

			previouslyActivated = true;

			if (useBlurUIPanel) {
				examineObjectRenderTexturePanel.SetActive (true);
				pauseManager.changeBlurUIPanelValue (true, examineObjectBlurPanelParent, true);
			}
		} else {
			//if the player disconnect the computer, then enabled of its components and set the camera to its previous position inside the player
			//make the mouse cursor visible according to the action of the player
			currentPlayerControllerManager.setUsingDeviceState (deviceEnabled);

			pauseManager.usingDeviceState (deviceEnabled);
			currentPlayerControllerManager.changeScriptState (!deviceEnabled);

			weaponsManager.setUsingDeviceState (deviceEnabled);

			if (keepWeaponsIfCarrying) {
				if (!keepOnlyIfPlayerIsOnFirstPerson || firstPersonActive) {
					if (drawWeaponsIfPreviouslyCarrying && carryingWeaponsPreviously) {
						weaponsManager.checkIfDrawSingleOrDualWeapon ();
					}
				}
			}

			if (disablePlayerMeshGameObject) {
				currentPlayerControllerManager.getGravityCenter ().gameObject.SetActive (!deviceEnabled);
			}

			stepManager.enableOrDisableFootStepsWithDelay (!deviceEnabled, 0.5f);

			if (hideMouseCursorIfUsingGamepad) {
				if (pauseManager.isUsingGamepad ()) {
					menuPause.setCursorVisibleState (previousCursorVisibleState);
				}
			}

			pauseManager.showOrHideCursor (deviceEnabled);

			pauseManager.changeCameraState (!deviceEnabled);

			if (previouslyActivated) {
				usingDevicesManager.setIconButtonCanBeShownState (previouslyIconButtonActive);
			}

			devicePositionTarget = originalPosition;
			deviceRotationTarget = originalRotation;

			deviceTransform.SetParent (originalDeviceParentTransform);

			usingDevicesManager.checkIfRemoveDeviceFromList ();

			if (useBlurUIPanel) {
				pauseManager.changeBlurUIPanelValue (false, examineObjectBlurPanelParent, true);
			}
		}

		pauseManager.enableOrDisableDynamicElementsOnScreen (!deviceEnabled);

		if (disableAllPlayerHUD) {
			pauseManager.enableOrDisablePlayerHUD (!deviceEnabled);
		} else {
			if (disableSecondaryPlayerHUD) {
				pauseManager.enableOrDisableSecondaryPlayerHUD (!deviceEnabled);
			}
		}

		ignoreObjectRotation = false;

		if (deviceEnabled && !rotateObjectOnCameraDirectionEnabled) {
			ignoreObjectRotation = true;
		}

		if (smoothCameraMovement) {
			
			//stop the coroutine to translate the device and call it again
			checkCameraPosition ();
		} else {
			deviceTransform.localRotation = deviceRotationTarget;
			deviceTransform.localPosition = devicePositionTarget;

			if (!deviceEnabled) {
				setColliderListState (!deviceEnabled);

				setLayerListState (!deviceEnabled);

				usingDevicesManager.setExamineteDevicesCameraState (deviceEnabled, useBlurUIPanel);
			}
		}

		if (activateExaminateObjectSystem && examineObjectManager) {
			examineObjectManager.examineDevice ();
		}

		pauseManager.showOrHideMouseCursorController (deviceEnabled);

		if (setNewMouseCursorControllerSpeed) {
			if (deviceEnabled) {
				pauseManager.setMouseCursorControllerSpeedOnGameValue (newMouseCursroControllerSpeed);
			} else {
				pauseManager.setOriginalMouseCursorControllerSpeedOnGameValue ();
			}
		}

		pauseManager.checkEnableOrDisableTouchZoneList (!deviceEnabled);
	}

	public void checkCameraPosition ()
	{
		if (cameraState != null) {
			StopCoroutine (cameraState);
		}

		cameraState = StartCoroutine (adjustCamera ());
	}

	//move the device from its position in the scene to a fix position in player camera for a proper looking
	IEnumerator adjustCamera ()
	{
		if (deviceEnabled) {
			setRigidbodyState (true);
		}

		bool isFirstPersonActive = currentPlayerControllerManager.isPlayerOnFirstPerson ();

		bool rotateObject = true;

		if (ignoreObjectRotation) {
			rotateObject = false;
		}

		if (useFixedLerpMovement) {
			float i = 0;

			Quaternion currentQ = deviceTransform.localRotation;
			Vector3 currentPos = deviceTransform.localPosition;

			//translate position and rotation of the device
			while (i < 1) {
				i += Time.deltaTime * fixedLerpMovementSpeed;

				if (rotateObject) {
					deviceTransform.localRotation = Quaternion.Lerp (currentQ, deviceRotationTarget, i);
				}

				deviceTransform.localPosition = Vector3.Lerp (currentPos, devicePositionTarget, i);

				yield return null;
			}
		} else {
			float currentCameraMovementSpeed = cameraMovementSpeedThirdPerson;

			if (isFirstPersonActive) {
				currentCameraMovementSpeed = cameraMovementSpeedFirstPerson;
			}

			float dist = GKC_Utils.distance (deviceTransform.localPosition, devicePositionTarget);

			float duration = dist / currentCameraMovementSpeed;

			float t = 0;

			float movementTimer = 0;

			bool targetReached = false;

			float angleDifference = 0;

			float positionDifference = 0;

			while (!targetReached) {
				t += Time.deltaTime / duration; 

				deviceTransform.localPosition = Vector3.Slerp (deviceTransform.localPosition, devicePositionTarget, t);

				if (rotateObject) {
					deviceTransform.localRotation = Quaternion.Slerp (deviceTransform.localRotation, deviceRotationTarget, t);
				
					angleDifference = Quaternion.Angle (deviceTransform.localRotation, deviceRotationTarget);
				} else {
					angleDifference = 0;
				}

				positionDifference = GKC_Utils.distance (deviceTransform.localPosition, devicePositionTarget);

				movementTimer += Time.deltaTime;

				if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
					targetReached = true;
				}

				yield return null;
			}
		}

		if (!deviceEnabled) {
			setColliderListState (true);

			setLayerListState (true);

			usingDevicesManager.setExamineteDevicesCameraState (false, useBlurUIPanel);

			setRigidbodyState (false);

			if (useBlurUIPanel) {
				examineObjectRenderTexturePanel.SetActive (false);
			}
		}
	}

	public void setRigidbodyState (bool state)
	{
		if (mainRigidbody != null && objectHasActiveRigidbody) {
			if (state) {
				originalKinematicValue = mainRigidbody.isKinematic;
				originalUseGravityValue = mainRigidbody.useGravity;
				mainRigidbody.useGravity = false;
				mainRigidbody.isKinematic = true;
			} else {
				mainRigidbody.useGravity = originalUseGravityValue;
				mainRigidbody.isKinematic = originalKinematicValue;
			}
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer != null) {
			mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			currentPlayerControllerManager = mainPlayerComponentsManager.getPlayerController ();

			mainCamera = mainPlayerComponentsManager.getPlayerCamera ().getMainCamera ();

			usingDevicesManager = mainPlayerComponentsManager.getUsingDevicesSystem ();

			headBobManager = mainPlayerComponentsManager.getHeadBob ();

			weaponsManager = mainPlayerComponentsManager.getPlayerWeaponsManager ();

			stepManager = mainPlayerComponentsManager.getFootStepManager ();

			playerCollider = currentPlayerControllerManager.getMainCollider ();

			pauseManager = mainPlayerComponentsManager.getPauseManager ();

			if (useBlurUIPanel) {
				examineObjectRenderTexturePanel = usingDevicesManager.getExamineObjectRenderTexturePanel ();
				examineObjectBlurPanelParent = usingDevicesManager.getExamineObjectBlurPanelParent ();
			}
		}
	}

	public void setColliderListState (bool state)
	{
		for (int i = 0; i < colliderListToDisable.Count; i++) {
			if (colliderListToDisable [i] != null) {
				colliderListToDisable [i].enabled = state;

				Physics.IgnoreCollision (playerCollider, colliderListToDisable [i], deviceEnabled);
			}
		}

		for (int i = 0; i < colliderListButtons.Count; i++) {
			Physics.IgnoreCollision (playerCollider, colliderListButtons [i], deviceEnabled);
		}

		if (ignoreDeviceTriggerEnabled && state) {
			deviceTrigger.enabled = false;
		} else {
			deviceTrigger.enabled = state;
		}

	}

	public void setIgnoreDeviceTriggerEnabledState (bool state)
	{
		ignoreDeviceTriggerEnabled = state;
	}

	public void setLayerList ()
	{
		Component[] components = deviceGameObject.GetComponentsInChildren (typeof(Transform));

		foreach (Component c in components) {
			layerInfo newLayerInfo = new layerInfo ();
			newLayerInfo.gameObject = c.gameObject;
			newLayerInfo.layerNumber = c.gameObject.layer;
			layerList.Add (newLayerInfo);
		}

		if (useListOfDisabledObjects) {
			for (int i = 0; i < disabledObjectList.Count; i++) {
				if (disabledObjectList [i] != null) {
					disabledObjectList [i].SetActive (false);
				}
			}
		}
	}

	public void setLayerListState (bool state)
	{
		int layerIndex = LayerMask.NameToLayer (layerToExaminateDevices);

		for (int i = 0; i < layerList.Count; i++) {
			if (layerList [i].gameObject != null) {
				if (state) {
					layerList [i].gameObject.layer = layerList [i].layerNumber;
				} else {
					layerList [i].gameObject.layer = layerIndex;
				}
			}
		}
	}

	public void changeDeviceZoom (bool zoomIn)
	{
		if (zoomIn) {
			distanceFromCamera += Time.deltaTime * zoomSpeed;
		} else {
			distanceFromCamera -= Time.deltaTime * zoomSpeed;
		}

		if (distanceFromCamera > maxZoomDistance) {
			distanceFromCamera = maxZoomDistance;
		}
		if (distanceFromCamera < minZoomDistance) {
			distanceFromCamera = minZoomDistance;
		}

		checkCameraPosition ();

		devicePositionTarget = Vector3.zero + Vector3.forward * distanceFromCamera;
		deviceRotationTarget = transform.localRotation;
	}

	public void resetRotation ()
	{
		devicePositionTarget = transform.localPosition;
		deviceRotationTarget = Quaternion.identity;

		checkCameraPosition ();
	}

	public void resetRotationAndPosition ()
	{
		devicePositionTarget = Vector3.zero + Vector3.forward * originalDistanceFromCamera;
		deviceRotationTarget = Quaternion.identity;

		checkCameraPosition ();
	}

	[System.Serializable]
	public class layerInfo
	{
		public GameObject gameObject;
		public int layerNumber;
	}
}