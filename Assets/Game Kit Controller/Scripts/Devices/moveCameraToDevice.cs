using UnityEngine;
using System.Collections;

public class moveCameraToDevice : MonoBehaviour
{
	public bool cameraMovementActive = true;
	public GameObject cameraPosition;
	public bool smoothCameraMovement = true;

	public bool useFixedLerpMovement = true;
	public float fixedLerpMovementSpeed = 2;

	public float cameraMovementSpeedThirdPerson = 1;
	public float cameraMovementSpeedFirstPerson = 0.2f;

	public bool secondMoveCameraToDevice;

	public bool unlockCursor = true;

	public bool ignoreHideCursorOnClick;

	public bool setNewMouseCursorControllerSpeed;
	public float newMouseCursroControllerSpeed;

	public bool disablePlayerMeshGameObject = true;
	public bool enablePlayerMeshGameObjectIfFirstPersonActive;

	public bool disableWeaponsCamera;
	public bool keepWeaponsIfCarrying;
	public bool drawWeaponsIfPreviouslyCarrying;
	public bool keepOnlyIfPlayerIsOnFirstPerson;
	public bool disableWeaponsDirectlyOnStart;
	bool carryingWeaponsPreviously;
	bool firstPersonActive;

	public bool carryWeaponOnLowerPositionActive;

	public bool setPlayerCameraRotationOnExit;
	public Transform playerPivotTransformThirdPerson;
	public Transform playerCameraTransformThirdPerson;
	public Transform playerPivotTransformFirstPerson;
	public Transform playerCameraTransformFirstPerson;

	public bool alignPlayerWithCameraPositionOnStartUseDevice;

	public bool alignPlayerWithCameraPositionOnStopUseDevice;

	public bool alignPlayerWithCameraRotationOnStartUseDevice;

	public bool alignPlayerWithCameraRotationOnStopUseDevice;

	public Transform customAlignPlayerTransform;

	public bool resetPlayerCameraDirection;

	public bool disableSecondaryPlayerHUD = true;
	public bool disableAllPlayerHUD;
	public bool disableTouchControls;

	public bool disableInteractionTouchButtonOnUsingDevice;

	public bool showGizmo;
	public float gizmoRadius = 0.1f;
	public Color gizmoLabelColor = Color.black;
	public float gizmoArrowLength = 0.3f;
	public float gizmoArrowLineLength = 0.5f;
	public float gizmoArrowAngle = 20;
	public Color gizmoArrowColor = Color.white;

	Transform cameraParentTransform;
	Vector3 mainCameraTargetPosition;
	Quaternion mainCameraTargetRotation;
	Coroutine cameraState;
	bool deviceEnabled;
	Camera mainCamera;
	menuPause pauseManager;
	GameObject currentPlayer;
	playerController currentPlayerControllerManager;
	playerWeaponsManager weaponsManager;
	usingDevicesSystem usingDevicesManager;
	headBob headBobManager;
	grabObjects grabObjectsManager;
	footStepManager stepManager;
	playerCamera playerCameraManager;
	headTrack headTrackManager;

	playerComponentsManager mainPlayerComponentsManager;

	bool previouslyIconButtonActive;

	bool movingCamera;

	Coroutine headTrackTargetCoroutine;
	Transform headTrackTargeTransform;

	//this function was placed in computer device, but now it can be added to any type of device when the player is using it,
	//to move the camera position and rotation in front of the device and place it again in its regular place when the player stops using the device

	void Start ()
	{
		mainCameraTargetRotation = Quaternion.identity;
		mainCameraTargetPosition = Vector3.zero;
	}

	public bool ignoreMoveCameraFunctionEnabled;

	//activate the device
	public void moveCamera (bool state)
	{
		if (ignoreMoveCameraFunctionEnabled) {
			return;
		}

		if (deviceEnabled == state && !secondMoveCameraToDevice) {
			return;
		}

		deviceEnabled = state;

		if (deviceEnabled) {
			headBobManager.stopAllHeadbobMovements ();
		}

		headBobManager.playOrPauseHeadBob (!state);

		//if the player is using the computer, disable the player controller, the camera, and set the parent of the camera inside the computer, 
		//to move to its view position
		
		if (cameraPosition == null) {
			cameraPosition = gameObject;
		}

		if (deviceEnabled) {		
			if (currentPlayerControllerManager.isPlayerRunning ()) {
				currentPlayerControllerManager.stopRun ();
			}

			if (!secondMoveCameraToDevice) {
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

				if (disableWeaponsCamera) {
					if (weaponsManager.carryingWeaponInFirstPerson) {
						weaponsManager.weaponsCamera.gameObject.SetActive (false);
					}
				}

				pauseManager.usingDeviceState (deviceEnabled);
				currentPlayerControllerManager.changeScriptState (!deviceEnabled);

				if (disablePlayerMeshGameObject) {
					currentPlayerControllerManager.getGravityCenter ().gameObject.SetActive (!deviceEnabled);
				}

				if (enablePlayerMeshGameObjectIfFirstPersonActive) {
					if (firstPersonActive) {
						currentPlayerControllerManager.setCharacterMeshGameObjectState (true);
					}
				}

				stepManager.enableOrDisableFootStepsComponents (!deviceEnabled);

				if (unlockCursor) {
					pauseManager.showOrHideCursor (deviceEnabled);
				}

				pauseManager.changeCameraState (!deviceEnabled);
			}

			grabObjectsManager.checkToDropObjectIfNotPhysicalWeaponElseKeepWeapon ();

			previouslyIconButtonActive = usingDevicesManager.getCurrentIconButtonState ();

			if (disableInteractionTouchButtonOnUsingDevice) {
				usingDevicesManager.setIconButtonCanBeShownState (false);
			}

			if (cameraMovementActive) {
				if (cameraParentTransform == null) {
					cameraParentTransform = mainCamera.transform.parent;
					mainCamera.transform.SetParent (cameraPosition.transform);
				}
			}

			if (alignPlayerWithCameraPositionOnStartUseDevice) {
				Vector3 playerTargetPosition = Vector3.zero;

				if (customAlignPlayerTransform != null) {
					playerTargetPosition =
						new Vector3 (customAlignPlayerTransform.position.x, currentPlayer.transform.position.y, customAlignPlayerTransform.position.z);
				} else {
					playerTargetPosition =
						new Vector3 (cameraPosition.transform.position.x, currentPlayer.transform.position.y, cameraPosition.transform.position.z);
				}

				currentPlayer.transform.position = playerTargetPosition;

				playerCameraManager.transform.position = currentPlayer.transform.position;
			}

			if (alignPlayerWithCameraRotationOnStartUseDevice) {
				Vector3 playerTargetRotation = Vector3.zero;

				if (customAlignPlayerTransform != null) {
					playerTargetRotation =
						new Vector3 (currentPlayer.transform.eulerAngles.x, customAlignPlayerTransform.eulerAngles.y, currentPlayer.transform.eulerAngles.z);
				} else {
					playerTargetRotation =
						new Vector3 (currentPlayer.transform.eulerAngles.x, cameraPosition.transform.eulerAngles.y, currentPlayer.transform.eulerAngles.z);
				}

				currentPlayer.transform.eulerAngles = playerTargetRotation;

				playerCameraManager.transform.eulerAngles = currentPlayer.transform.eulerAngles;
			}

			if (resetPlayerCameraDirection) {
				playerCameraManager.setLookAngleValue (Vector2.zero);

				playerCameraManager.resetCurrentCameraStateAtOnce ();

				playerCameraManager.getPivotCameraTransform ().localRotation = Quaternion.identity;
			}
		} else {

			//set player camera rotation when the player stops using the device
			if (setPlayerCameraRotationOnExit) {
				bool isFirstPersonActive = playerCameraManager.isFirstPersonActive ();

				Vector3 pivotCameraRotation = Vector3.zero;
				Vector3 cameraRotation = Vector3.zero;

				if (isFirstPersonActive) {
					cameraRotation = playerCameraTransformFirstPerson.eulerAngles;
					pivotCameraRotation = playerPivotTransformFirstPerson.localEulerAngles;
				} else {
					cameraRotation = playerCameraTransformThirdPerson.eulerAngles;
					pivotCameraRotation = playerPivotTransformThirdPerson.localEulerAngles;
				}
					
				playerCameraManager.transform.eulerAngles = cameraRotation;
				playerCameraManager.getPivotCameraTransform ().localEulerAngles = pivotCameraRotation;

				float newLookAngleValue = pivotCameraRotation.x;
				if (newLookAngleValue > 180) {
					newLookAngleValue -= 360;
				}
				playerCameraManager.setLookAngleValue (new Vector2 (0, newLookAngleValue));
				playerCameraManager.setCurrentCameraUpRotationValue (0);
			}

			//if the player disconnect the computer, then enabled of its components and set the camera to its previous position inside the player
			if (!secondMoveCameraToDevice) {
				//make the mouse cursor visible according to the action of the player
				currentPlayerControllerManager.setUsingDeviceState (deviceEnabled);

				pauseManager.usingDeviceState (deviceEnabled);
				currentPlayerControllerManager.changeScriptState (!deviceEnabled);

				if (disableWeaponsCamera) {
					if (weaponsManager.carryingWeaponInFirstPerson) {
						weaponsManager.weaponsCamera.gameObject.SetActive (true);
					}
				}

				if (disablePlayerMeshGameObject) {
					currentPlayerControllerManager.getGravityCenter ().gameObject.SetActive (!deviceEnabled);
				}

				if (enablePlayerMeshGameObjectIfFirstPersonActive) {
					if (firstPersonActive) {
						currentPlayerControllerManager.setCharacterMeshGameObjectState (false);
					}
				}

				weaponsManager.setUsingDeviceState (deviceEnabled);

				if (keepWeaponsIfCarrying) {
					if (!keepOnlyIfPlayerIsOnFirstPerson || firstPersonActive) {
						if (drawWeaponsIfPreviouslyCarrying && carryingWeaponsPreviously) {
							weaponsManager.checkIfDrawSingleOrDualWeapon ();
						}
					}
				}

				stepManager.enableOrDisableFootStepsWithDelay (!deviceEnabled, 0);

				if (unlockCursor) {
					pauseManager.showOrHideCursor (deviceEnabled);
				}

				pauseManager.changeCameraState (!deviceEnabled);
			}

			if (cameraMovementActive) {
				if (alignPlayerWithCameraPositionOnStopUseDevice) {
					Vector3 playerTargetPosition = Vector3.zero;

					if (customAlignPlayerTransform != null) {
						playerTargetPosition =
							new Vector3 (customAlignPlayerTransform.position.x, currentPlayer.transform.position.y, customAlignPlayerTransform.position.z);

						currentPlayer.transform.position = playerTargetPosition;
					} else {
					
						float xPosition = currentPlayer.transform.InverseTransformPoint (mainCamera.transform.position).x;
						float zPosition = currentPlayer.transform.InverseTransformPoint (mainCamera.transform.position).z;

						currentPlayer.transform.position += currentPlayer.transform.right * xPosition + currentPlayer.transform.forward * zPosition;
					}

					playerCameraManager.transform.position = currentPlayer.transform.position;
				}

				if (alignPlayerWithCameraRotationOnStopUseDevice) {
					Vector3 playerTargetRotation = Vector3.zero;

					if (customAlignPlayerTransform != null) {
						playerTargetRotation =
							new Vector3 (currentPlayer.transform.eulerAngles.x, customAlignPlayerTransform.eulerAngles.y, currentPlayer.transform.eulerAngles.z);
					} else {
						playerTargetRotation =
							new Vector3 (currentPlayer.transform.eulerAngles.x, cameraPosition.transform.eulerAngles.y, currentPlayer.transform.eulerAngles.z);
					}

					currentPlayer.transform.eulerAngles = playerTargetRotation;

					playerCameraManager.transform.rotation = currentPlayer.transform.rotation;
				}

				if (cameraParentTransform != null) {
					mainCamera.transform.SetParent (cameraParentTransform);
					cameraParentTransform = null;
				}
			}

			usingDevicesManager.setIconButtonCanBeShownState (previouslyIconButtonActive);

			usingDevicesManager.checkIfRemoveDeviceFromList ();
		}
	
		pauseManager.enableOrDisableDynamicElementsOnScreen (!deviceEnabled);

		if (disableAllPlayerHUD) {
			pauseManager.enableOrDisablePlayerHUD (!deviceEnabled);
		} else {
			if (disableSecondaryPlayerHUD) {
				pauseManager.enableOrDisableSecondaryPlayerHUD (!deviceEnabled);
			}
		}

		if (disableTouchControls) {
			if (pauseManager.isUsingTouchControls ()) {
				pauseManager.enableOrDisableTouchControlsExternally (!deviceEnabled);
			}
		}

		if (cameraMovementActive) {
			if (smoothCameraMovement) {
				
				//stop the coroutine to translate the camera and call it again
				if (cameraState != null) {
					StopCoroutine (cameraState);
				}

				cameraState = StartCoroutine (adjustCamera ());

				if (headTrackManager.useHeadTrackTarget) {
					headTrackTargeTransform = headTrackManager.getHeadTrackTargetTransform ();

					if (headTrackTargetCoroutine != null) {
						StopCoroutine (headTrackTargetCoroutine);
					}

					headTrackTargetCoroutine = StartCoroutine (adjustHeadTrackTarget ());
				}
			} else {
				mainCamera.transform.localRotation = mainCameraTargetRotation;
				mainCamera.transform.localPosition = mainCameraTargetPosition;

				if (headTrackManager.useHeadTrackTarget) {
					headTrackTargeTransform = headTrackManager.getHeadTrackTargetTransform ();

					if (deviceEnabled) {
						headTrackTargeTransform.SetParent (cameraPosition.transform);
						headTrackTargeTransform.localPosition = mainCameraTargetPosition;
					} else {
						headTrackTargeTransform.SetParent (headTrackManager.getHeadTrackTargetParent ());
						headTrackTargeTransform.localPosition = headTrackManager.getOriginalHeadTrackTargetPosition ();
					}
				}
			}
		}

		if (unlockCursor) {
			pauseManager.showOrHideMouseCursorController (deviceEnabled);

			if (setNewMouseCursorControllerSpeed) {
				if (deviceEnabled) {
					pauseManager.setMouseCursorControllerSpeedOnGameValue (newMouseCursroControllerSpeed);
				} else {
					pauseManager.setOriginalMouseCursorControllerSpeedOnGameValue ();
				}
			}

			if (ignoreHideCursorOnClick) {
				pauseManager.setIgnoreHideCursorOnClickActiveState (deviceEnabled);
			}
		}

		pauseManager.checkEnableOrDisableTouchZoneList (!deviceEnabled);
	}

	//move the camera from its position in player camera to a fix position for a proper looking of the computer and vice versa
	IEnumerator adjustCamera ()
	{
		movingCamera = true;

		Transform mainCameraTransform = mainCamera.transform;

		if (useFixedLerpMovement) {
			float i = 0;
			//store the current rotation of the camera
			Quaternion currentQ = mainCameraTransform.localRotation;
			//store the current position of the camera
			Vector3 currentPos = mainCameraTransform.localPosition;

			//translate position and rotation camera
			while (i < 1) {
				i += Time.deltaTime * fixedLerpMovementSpeed;

				mainCameraTransform.localRotation = Quaternion.Lerp (currentQ, mainCameraTargetRotation, i);
				mainCameraTransform.localPosition = Vector3.Lerp (currentPos, mainCameraTargetPosition, i);

				yield return null;
			}

		} else {
			bool isFirstPersonActive = playerCameraManager.isFirstPersonActive ();

			float currentCameraMovementSpeed = cameraMovementSpeedThirdPerson;

			if (isFirstPersonActive) {
				currentCameraMovementSpeed = cameraMovementSpeedFirstPerson;
			}

			float dist = GKC_Utils.distance (mainCameraTransform.localPosition, mainCameraTargetPosition);

			float duration = dist / currentCameraMovementSpeed;

			float t = 0;

			float movementTimer = 0;

			bool targetReached = false;

			float angleDifference = 0;

			float positionDifference = 0;

			while (!targetReached) {
				t += Time.deltaTime / duration; 
				mainCameraTransform.localPosition = Vector3.Lerp (mainCameraTransform.localPosition, mainCameraTargetPosition, t);
				mainCameraTransform.localRotation = Quaternion.Lerp (mainCameraTransform.localRotation, mainCameraTargetRotation, t);

				angleDifference = Quaternion.Angle (mainCameraTransform.localRotation, mainCameraTargetRotation);

				positionDifference = GKC_Utils.distance (mainCameraTransform.localPosition, mainCameraTargetPosition);

				movementTimer += Time.deltaTime;

				if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
					targetReached = true;
				}

				yield return null;
			}
		}

		movingCamera = false;
	}

	//move the camera from its position in player camera to a fix position for a proper looking of the computer and vice versa
	IEnumerator adjustHeadTrackTarget ()
	{
		Vector3 targetPosition = mainCameraTargetPosition;
		Quaternion targeRotation = Quaternion.identity;

		headTrackTargeTransform.SetParent (cameraPosition.transform);

		if (!deviceEnabled) {
			targetPosition = headTrackManager.getOriginalHeadTrackTargetPosition ();
			headTrackTargeTransform.SetParent (headTrackManager.getHeadTrackTargetParent ());
		}

		if (useFixedLerpMovement) {
			float i = 0;
			//store the current rotation of the camera
			Quaternion currentQ = headTrackTargeTransform.localRotation;
			//store the current position of the camera
			Vector3 currentPos = headTrackTargeTransform.localPosition;

			//translate position and rotation camera
			while (i < 1) {
				i += Time.deltaTime * fixedLerpMovementSpeed;

				headTrackTargeTransform.localRotation = Quaternion.Lerp (currentQ, targeRotation, i);
				headTrackTargeTransform.localPosition = Vector3.Lerp (currentPos, targetPosition, i);

				yield return null;
			}
		} else {
			bool isFirstPersonActive = playerCameraManager.isFirstPersonActive ();

			float currentCameraMovementSpeed = cameraMovementSpeedThirdPerson;

			if (isFirstPersonActive) {
				currentCameraMovementSpeed = cameraMovementSpeedFirstPerson;
			}

			float dist = GKC_Utils.distance (headTrackTargeTransform.localPosition, targetPosition);

			float duration = dist / currentCameraMovementSpeed;

			float t = 0;

			float movementTimer = 0;

			bool targetReached = false;

			float angleDifference = 0;

			float positionDifference = 0;

			while (!targetReached) {
				t += Time.deltaTime / duration; 

				headTrackTargeTransform.localPosition = Vector3.Lerp (headTrackTargeTransform.localPosition, targetPosition, t);
				headTrackTargeTransform.localRotation = Quaternion.Lerp (headTrackTargeTransform.localRotation, targeRotation, t);

				angleDifference = Quaternion.Angle (headTrackTargeTransform.localRotation, targeRotation);

				positionDifference = GKC_Utils.distance (headTrackTargeTransform.localPosition, targetPosition);

				movementTimer += Time.deltaTime;

				if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
					targetReached = true;
				}

				yield return null;
			}
		}
	}

	public void hasSecondMoveCameraToDevice ()
	{
		secondMoveCameraToDevice = true;
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer != null) {
			mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			currentPlayerControllerManager = mainPlayerComponentsManager.getPlayerController ();

			playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

			mainCamera = playerCameraManager.getMainCamera ();

			usingDevicesManager = mainPlayerComponentsManager.getUsingDevicesSystem ();

			headBobManager = mainPlayerComponentsManager.getHeadBob ();

			grabObjectsManager = mainPlayerComponentsManager.getGrabObjects ();

			weaponsManager = mainPlayerComponentsManager.getPlayerWeaponsManager ();

			stepManager = mainPlayerComponentsManager.getFootStepManager ();

			headTrackManager = mainPlayerComponentsManager.getHeadTrack ();

			pauseManager = mainPlayerComponentsManager.getPauseManager ();
		}
	}

	public void enableFreeInteractionState ()
	{
		if (carryWeaponOnLowerPositionActive) {
			weaponsManager.setCarryWeaponInLowerPositionActiveState (true);

			grabObjectsManager.enableOrDisableGeneralCursorFromExternalComponent (false);
		}
	}

	public void disableFreeInteractionState ()
	{
		if (carryWeaponOnLowerPositionActive) {
			weaponsManager.setCarryWeaponInLowerPositionActiveState (false);

			grabObjectsManager.enableOrDisableGeneralCursorFromExternalComponent (true);
		}
	}

	public void stopMovement ()
	{
		if (cameraState != null) {
			StopCoroutine (cameraState);
		}

		deviceEnabled = false;
	}

	public bool isCameraMoving ()
	{
		return movingCamera;
	}

	public void setCurrentPlayerUseDeviceButtonEnabledState (bool state)
	{
		if (usingDevicesManager != null) {
			usingDevicesManager.setUseDeviceButtonEnabledState (state);
		}
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
			if (cameraPosition != null) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (cameraPosition.transform.position, gizmoRadius);
				Gizmos.color = Color.white;
				Gizmos.DrawLine (cameraPosition.transform.position, transform.position);

				Gizmos.color = Color.green;
				GKC_Utils.drawGizmoArrow (cameraPosition.transform.position, cameraPosition.transform.forward * gizmoArrowLineLength, gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);
			}
		}
	}
}