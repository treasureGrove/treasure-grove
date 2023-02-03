using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class pauseOrResumePlayerControllerAndCameraSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool searchPlayerOnSceneIfNotAssigned = true;
	public bool assignPlayerManually;
	public GameObject currentPlayer;
	public GameObject playerCameraGameObject;

	public bool unlockCursor;

	public bool resumePlayerAfterDelay;
	public float delayToResumePlayer;

	public bool activatePlayerMeshModel = true;

	public bool pauseEscapeMenu;

	[Space]
	[Header ("Camera Settings")]
	[Space]

	public bool cameraIsMoved;

	public float resetCameraPositionSpeed;
	public bool setCameraDirectionAtEnd;
	public Transform cameraDirection;
	public Transform pivotDirection;

	[Space]

	public bool setNewCameraParent;
	public Transform newCameraParent;

	[Space]
	[Header ("HUD Settings")]
	[Space]

	public bool disableSecondaryPlayerHUD;
	public bool disableAllPlayerHUD = true;
	public bool disableTouchControls = true;
	public bool disableDynamiUIElements;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool playerComponentsPaused;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnPause;
	public UnityEvent eventOnPause;
	public eventParameters.eventToCallWithGameObject eventToSendCamera;
	public bool useEventOnResume;
	public UnityEvent eventOnResume;

	playerController playerControllerManager;
	playerCamera playerCameraManager;
	headBob headBobManager;
	playerStatesManager statesManager;
	menuPause pauseManager;
	playerComponentsManager mainPlayerComponentsManager;
	playerInputManager playerInput;

	headTrack mainHeadTrack;

	Transform previousCameraParent;
	Vector3 previousCameraPosition;
	Transform mainCamera;

	Coroutine resetCameraPositionCoroutine;

	bool playerIsDriving;

	bool usingDevicePreviously;

	bool headScaleChanged;

	Coroutine resumePlayerCoroutine;

	bool playerAssignedProperly;

	void Start ()
	{
		if (assignPlayerManually) {
			getCurrentPlayer (currentPlayer);
		}
	}

	public void getCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer == null) {
			return;
		}

		mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			playerControllerManager = mainPlayerComponentsManager.getPlayerController ();

			playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

			playerCameraGameObject = playerCameraManager.gameObject;

			mainCamera = playerCameraManager.getMainCamera ().transform;

			headBobManager = mainPlayerComponentsManager.getHeadBob ();

			statesManager = mainPlayerComponentsManager.getPlayerStatesManager ();

			pauseManager = mainPlayerComponentsManager.getPauseManager ();

			playerInput = mainPlayerComponentsManager.getPlayerInputManager ();

			mainHeadTrack = mainPlayerComponentsManager.getHeadTrack ();

			playerAssignedProperly = true;
		}
	}

	public void pauseOrPlayPlayerComponents (bool state)
	{
		if (currentPlayer == null || !playerAssignedProperly) {

			findPlayerOnScene ();

			if (currentPlayer == null) {
				print ("WARNING: no player controller has been assigned to the mission." +
				" Make sure to use a trigger to activate the mission or assign the player manually");

				return;
			}
		}

		if (playerComponentsPaused == state) {
			if (playerComponentsPaused) {
				print ("Trying to pause the player when it is already paused, avoiding function call to keep the player in same state");
			} else {
				print ("Trying to resume the player when it is already resumed, avoiding function call to keep the player in same state");
			}

			return;
		}

		playerComponentsPaused = state;

		playerIsDriving = playerControllerManager.isPlayerDriving ();

		playerInput.setAvoidInputActiveState (state);

		if (playerComponentsPaused) {
			usingDevicePreviously = playerControllerManager.isUsingDevice ();
		}

		if (!playerIsDriving) {
			if (!usingDevicePreviously) {
				playerControllerManager.smoothChangeScriptState (!state);

				mainHeadTrack.setSmoothHeadTrackDisableState (state);

				playerControllerManager.setHeadTrackCanBeUsedState (!state);

				playerControllerManager.setUsingDeviceState (state);

				if (playerComponentsPaused) {
					headBobManager.stopAllHeadbobMovements ();
				}
				
				headBobManager.playOrPauseHeadBob (!state);

				statesManager.checkPlayerStates (false, true, false, true, false, false, true, true);

				playerCameraManager.pauseOrPlayCamera (!state);
			}
		}

		if (playerIsDriving) {
			if (playerComponentsPaused) {
				headScaleChanged = playerControllerManager.isHeadScaleChanged ();

				if (headScaleChanged) {
					playerControllerManager.changeHeadScale (false);
				}
			} else {
				if (headScaleChanged) {
					playerControllerManager.changeHeadScale (true);
				}
			}
		} else {
			checkCharacterMesh (state);
		}

		if (unlockCursor) {
			pauseManager.showOrHideCursor (playerComponentsPaused);
			pauseManager.usingDeviceState (playerComponentsPaused);
			pauseManager.usingSubMenuState (playerComponentsPaused);
		}

		if (playerIsDriving) {
			if (disableAllPlayerHUD) {
				pauseManager.enableOrDisableVehicleHUD (!playerComponentsPaused);
			}
		} else {
			if (!usingDevicePreviously) {
				if (disableAllPlayerHUD) {
					pauseManager.enableOrDisablePlayerHUD (!playerComponentsPaused);
				} else {
					if (disableSecondaryPlayerHUD) {
						pauseManager.enableOrDisableSecondaryPlayerHUD (!playerComponentsPaused);
					}
				}
			}
		}

		if (disableDynamiUIElements) {
			pauseManager.enableOrDisableDynamicElementsOnScreen (!playerComponentsPaused);
		}

		if (disableTouchControls) {
			if (!usingDevicePreviously) {
				if (pauseManager.isUsingTouchControls ()) {
					pauseManager.enableOrDisableTouchControlsExternally (!playerComponentsPaused);
				}
			}
		}

		if (!usingDevicePreviously) {
			pauseManager.enableOrDisableDynamicElementsOnScreen (!playerComponentsPaused);
		}

		playerInput.setInputPausedForExternalComponentsState (playerComponentsPaused);

		if (playerComponentsPaused) {
			if (useEventOnPause) {
				eventToSendCamera.Invoke (mainCamera.gameObject);
				eventOnPause.Invoke ();
			}
			if (cameraIsMoved) {
				previousCameraParent = mainCamera.parent;
				previousCameraPosition = mainCamera.localPosition;

				if (setNewCameraParent) {
					mainCamera.SetParent (newCameraParent);
				} else {
					mainCamera.SetParent (null);
				}
			}
		} else {
			if (useEventOnResume) {
				eventOnResume.Invoke ();
			}
		}

		stopResumePlayerAfterTimeDelay ();

		if (state) {
			if (resumePlayerAfterDelay) {
				resumePlayerAfterTimeDelay ();
			}
		}

		if (pauseEscapeMenu) {
			pauseManager.setPauseGameInputPausedState (state);
		}
	}

	public void checkCharacterMesh (bool state)
	{
		if (activatePlayerMeshModel) {
			bool firstCameraEnabled = playerCameraManager.isFirstPersonActive ();

			if (firstCameraEnabled) {
				playerControllerManager.setCharacterMeshGameObjectState (state);
			}

			if (usingDevicePreviously) {
				playerControllerManager.getGravityCenter ().gameObject.SetActive (state);
			}
		}
	}

	public void resetCameraPosition ()
	{
		if (!cameraIsMoved) {
			return;
		}

		if (resetCameraPositionCoroutine != null) {
			StopCoroutine (resetCameraPositionCoroutine);
		}

		resetCameraPositionCoroutine = StartCoroutine (resetCameraCoroutine ());
	}

	IEnumerator resetCameraCoroutine ()
	{
		setCameraDirection ();

		mainCamera.SetParent (previousCameraParent);

		Vector3	targetPosition = previousCameraPosition;
		Quaternion targeRotation = Quaternion.identity;

		Vector3	worldTargetPosition = previousCameraParent.position;
		float dist = GKC_Utils.distance (mainCamera.position, worldTargetPosition);
		float duration = dist / resetCameraPositionSpeed;
		float t = 0;
			
		float movementTimer = 0;

		bool targetReached = false;

		float angleDifference = 0;

		float currentDistance = 0;

		while (!targetReached) {
			t += Time.deltaTime / duration; 

			mainCamera.localPosition = Vector3.Lerp (mainCamera.localPosition, targetPosition, t);
			mainCamera.localRotation = Quaternion.Lerp (mainCamera.localRotation, targeRotation, t);

			angleDifference = Quaternion.Angle (mainCamera.localRotation, targeRotation);

			currentDistance = GKC_Utils.distance (mainCamera.localPosition, targetPosition);

			movementTimer += Time.deltaTime;

			if ((currentDistance < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
				targetReached = true;
			}

			yield return null;
		}

		pauseOrPlayPlayerComponents (false);
	}

	public void setCameraDirection ()
	{
		if (setCameraDirectionAtEnd) {
			playerCameraGameObject.transform.rotation = cameraDirection.rotation;

			Quaternion newCameraRotation = pivotDirection.localRotation;

			playerCameraManager.getPivotCameraTransform ().localRotation = newCameraRotation;

			float newLookAngleValue = newCameraRotation.eulerAngles.x;

			if (newLookAngleValue > 180) {
				newLookAngleValue -= 360;
			}

			playerCameraManager.setLookAngleValue (new Vector2 (0, newLookAngleValue));
		}
	}

	public void resumePlayerAfterTimeDelay ()
	{
		stopResumePlayerAfterTimeDelay ();

		resumePlayerCoroutine = StartCoroutine (resumePlayerAfterTimeDelayCoroutine ());
	}

	public void stopResumePlayerAfterTimeDelay ()
	{
		if (resumePlayerCoroutine != null) {
			StopCoroutine (resumePlayerCoroutine);
		}
	}

	IEnumerator resumePlayerAfterTimeDelayCoroutine ()
	{
		yield return new WaitForSeconds (delayToResumePlayer);

		pauseOrPlayPlayerComponents (false);
	}

	public void findPlayerOnScene ()
	{
		if (searchPlayerOnSceneIfNotAssigned) {
			getCurrentPlayer (GKC_Utils.findMainPlayerOnScene ());
		}
	}
}
