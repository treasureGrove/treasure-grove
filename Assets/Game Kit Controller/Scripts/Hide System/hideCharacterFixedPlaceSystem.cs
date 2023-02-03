using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class hideCharacterFixedPlaceSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool cameraCanRotate = true;
	public float rotationSpeed = 10;
	public Vector2 rangeAngleX = new Vector2 (-90, 90);
	public Vector2 rangeAngleY = new Vector2 (-90, 90);
	public bool useSpringRotation;
	public float springRotationDelay = 1;
	public float smoothCameraRotationSpeed = 5;

	public bool cameraCanMove = true;
	public float moveCameraSpeed = 10;
	public float smoothMoveCameraSpeed = 5;
	public Vector2 moveCameraLimitsX = new Vector2 (-2, 2);
	public Vector2 moveCameraLimitsY = new Vector2 (-2, 2);

	public bool useSpringMovement;
	public float springMovementDelay = 1;

	public bool checkIfDetectedWhileHidden;

	public bool setHiddenFov;
	public float hiddenFov = 20;
	public bool zoomEnabled;
	public float zoomSpeed = 10;
	public float maxZoom = 10;
	public float minZoom = 90;

	public Transform cameraTransform;
	public Transform pivotTransform;
	public Transform cameraPositionTransform;

	public float hideEventDelay;
	public float visibleEventDelay;

	public bool canResetCameraRotation = true;
	public bool canResetCameraPosition = true;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool useCharacterStateIcon = true;
	public string visibleCharacterStateName = "Visible";
	public string notVisibleCharacterStateName = "Not Visible";

	public bool activateActionScreen = true;
	public string actionScreenName = "Use Hide System";

	public bool useMessageWhenUnableToHide;
	[TextArea (1, 10)] public string unableToHideMessage;
	public float showMessageTime;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent hidenEvent = new UnityEvent ();
	public UnityEvent visbleEvent = new UnityEvent ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool hidingCharacter;

	[Space]
	[Header ("Components")]
	[Space]

	public moveCameraToDevice moveCameraManager;
	public electronicDevice electronicDeviceManager;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor;
	public float arcGizmoRadius;
	public float gizmoArrowLength = 1;
	public float gizmoArrowAngle = 20;
	public Color gizmoArrowColor = Color.white;

	float originalCameraRotationX;
	float originalPivotRotationY;
	Quaternion currentCameraRotation;
	Quaternion currentPivotRotation;
	GameObject currentPlayer;

	Vector2 currentLookAngle;
	float lastTimeSpringRotation;

	playerController playerControllerManager;
	playerCamera playerCameraManager;

	playerInputManager playerInput;
	playerComponentsManager mainPlayerComponentsManager;
	playerStatesManager statesManager;

	characterFactionManager currentfactionManager;
	usingDevicesSystem devicesManager;

	Collider currentPlayerCollider;

	hideCharacterFixedPlaceSystemPlayerManagement hideCharacterFixedPlaceSystemPlayerManager;

	Vector3 currentMoveCameraPosition;
	Vector3 originalCameraLocalPosition;
	Vector3 currentCameraMovementPosition;
	float lastTimeSpringMovement;
	bool characterIsDetected;

	float horizontalMouse;
	float verticalMouse;
	float horizontalInput;
	float verticalInput;
	Coroutine eventCoroutine;

	bool increaseZoom;
	bool decreaseZoom;
	float currentFovValue;
	Camera mainCamera;

	float lastTimeMouseWheelUsed;
	bool mouseWheelUsedPreviously;
	Vector2 axisValues;

	bool currentPlayerCameraIsFree;

	bool characterFoundInsideHidePlace;

	void Start ()
	{
		originalCameraRotationX = cameraTransform.localRotation.eulerAngles.x;
		originalPivotRotationY = pivotTransform.localRotation.eulerAngles.y;

		if (electronicDeviceManager == null) {
			electronicDeviceManager = GetComponent<electronicDevice> ();
		}

		if (moveCameraManager == null) {
			moveCameraManager = GetComponent<moveCameraToDevice> ();
		}

		originalCameraLocalPosition = pivotTransform.localPosition;
	}

	void FixedUpdate ()
	{
		if (hidingCharacter) {
			if (checkIfDetectedWhileHidden && !characterFoundInsideHidePlace) {
				characterFoundInsideHidePlace = currentfactionManager.isCharacterDetectedAsEnemyByOtherFaction (currentPlayer);

				if (characterFoundInsideHidePlace) {
					devicesManager.inputActivateDevice ();

					characterFoundInsideHidePlace = false;

					return;
				}
			}

			if (currentPlayerCameraIsFree && !moveCameraManager.isCameraMoving ()) {
				axisValues = playerInput.getPlayerMouseAxis ();
				horizontalMouse = axisValues.x;
				verticalMouse = axisValues.y;

				if (cameraCanRotate) {
					currentLookAngle.x += horizontalMouse * rotationSpeed;
					currentLookAngle.y -= verticalMouse * rotationSpeed;

					currentLookAngle.x = Mathf.Clamp (currentLookAngle.x, rangeAngleY.x, rangeAngleY.y);

					currentLookAngle.y = Mathf.Clamp (currentLookAngle.y, rangeAngleX.x, rangeAngleX.y);

					currentCameraRotation = Quaternion.Euler (originalCameraRotationX + currentLookAngle.y, 0, 0);

					currentPivotRotation = Quaternion.Euler (0, originalPivotRotationY + currentLookAngle.x, 0);
				}

				if (useSpringRotation && canResetCameraRotation) {
					if (horizontalMouse != 0 || verticalMouse != 0) {
						lastTimeSpringRotation = Time.time;
					}

					if (Time.time > lastTimeSpringRotation + springRotationDelay) {
						currentCameraRotation = Quaternion.Euler (originalCameraRotationX, 0, 0);
						currentPivotRotation = Quaternion.Euler (0, originalPivotRotationY, 0);
						currentLookAngle = Vector2.zero;
					}
				}

				cameraTransform.localRotation = Quaternion.Lerp (cameraTransform.localRotation, currentCameraRotation, Time.deltaTime * smoothCameraRotationSpeed);

				pivotTransform.localRotation = Quaternion.Lerp (pivotTransform.localRotation, currentPivotRotation, Time.deltaTime * smoothCameraRotationSpeed);

				axisValues = playerInput.getPlayerMovementAxis ();
				horizontalInput = axisValues.x;
				verticalInput = axisValues.y;

				if (cameraCanMove) {
					currentMoveCameraPosition.x += horizontalInput * moveCameraSpeed;
					currentMoveCameraPosition.y += verticalInput * moveCameraSpeed;

					currentMoveCameraPosition.x = Mathf.Clamp (currentMoveCameraPosition.x, moveCameraLimitsX.x, moveCameraLimitsX.y);

					currentMoveCameraPosition.y = Mathf.Clamp (currentMoveCameraPosition.y, moveCameraLimitsY.x, moveCameraLimitsY.y);

					Vector3 moveInput = currentMoveCameraPosition.x * Vector3.right + currentMoveCameraPosition.y * Vector3.up;	

					currentCameraMovementPosition = originalCameraLocalPosition + moveInput;
				}

				if (useSpringMovement && canResetCameraPosition) {
					if (horizontalInput != 0 || verticalInput != 0) {
						lastTimeSpringMovement = Time.time;
					}

					if (Time.time > lastTimeSpringMovement + springMovementDelay) {
						currentCameraMovementPosition = originalCameraLocalPosition;
						currentMoveCameraPosition = Vector3.zero;
					}
				}

				pivotTransform.localPosition = Vector3.MoveTowards (pivotTransform.localPosition, currentCameraMovementPosition, Time.deltaTime * smoothMoveCameraSpeed);	

				if (zoomEnabled) {

					if (mouseWheelUsedPreviously && Time.time > lastTimeMouseWheelUsed + 0.1f) {
						increaseZoom = false;
						decreaseZoom = false;
						mouseWheelUsedPreviously = false;
					}
		
					if (increaseZoom) {
						currentFovValue -= Time.deltaTime * zoomSpeed;
					}

					if (decreaseZoom) {
						currentFovValue += Time.deltaTime * zoomSpeed;
					}

					currentFovValue = Mathf.Clamp (currentFovValue, maxZoom, minZoom);

					if (playerCameraManager.isFOVChanging ()) {
						currentFovValue = mainCamera.fieldOfView;
					} else {
						mainCamera.fieldOfView = currentFovValue;
					}
				}
			}
		}
	}

	public void enableOrDisableHideState ()
	{
		if (characterIsDetected) {
			return;
		}

		hidingCharacter = !hidingCharacter;

		if (hidingCharacter) {
			setCurrentPlayer (electronicDeviceManager.getCurrentPlayer ());
		}

		if (playerCameraManager.isCameraTypeFree ()) {
			if (hidingCharacter) {
				if (setHiddenFov) {
					playerCameraManager.setMainCameraFov (hiddenFov, zoomSpeed);
				}
			} else {
				playerCameraManager.setOriginalCameraFov ();
			}

			currentPlayerCameraIsFree = true;
		} else {
			currentPlayerCameraIsFree = false;
		}
			
		playerControllerManager.setVisibleToAIState (!hidingCharacter);

		if (hidingCharacter) {
			statesManager.checkPlayerStates ();
		}

		if (!checkIfDetectedWhileHidden) {
			currentPlayerCollider.enabled = !hidingCharacter;
		}

		setCharacterState ();

		callUnityEvents ();

		resetCameraInputValues ();

		if (hideCharacterFixedPlaceSystemPlayerManager) {
			hideCharacterFixedPlaceSystemPlayerManager.setPlayerHidingState (hidingCharacter);
		}

		if (activateActionScreen) {
			playerInput.enableOrDisableActionScreen (actionScreenName, hidingCharacter);
		}
	}

	public void setCharacterState ()
	{
		if (useCharacterStateIcon) {
			if (hidingCharacter) {
				playerControllerManager.setCharacterStateIcon (notVisibleCharacterStateName);
			} else {
				playerControllerManager.setCharacterStateIcon (visibleCharacterStateName);
			}
		}
	}

	public void callUnityEvents ()
	{
		if (eventCoroutine != null) {
			StopCoroutine (eventCoroutine);
		}

		eventCoroutine = StartCoroutine (callUnityEventsCoroutine ());
	}

	IEnumerator callUnityEventsCoroutine ()
	{
		if (hidingCharacter) {
			yield return new WaitForSeconds (hideEventDelay);

			if (hidenEvent.GetPersistentEventCount () > 0) {
				hidenEvent.Invoke ();
			}
		} else {
			yield return new WaitForSeconds (visibleEventDelay);

			if (visbleEvent.GetPersistentEventCount () > 0) {
				visbleEvent.Invoke ();
			}
		}
	}

	public void resetCameraInputValues ()
	{
		//reset input values and camera and pivot position and rotation
		horizontalMouse = 0;
		verticalMouse = 0;
		currentLookAngle = Vector2.zero;
		currentCameraRotation = Quaternion.identity;
		currentPivotRotation = Quaternion.identity;
		cameraTransform.localRotation = Quaternion.identity;
		pivotTransform.localRotation = Quaternion.identity;

		horizontalInput = 0;
		verticalInput = 0;
		currentMoveCameraPosition = Vector3.zero;
		currentCameraMovementPosition = Vector3.zero;
		pivotTransform.localPosition = Vector3.zero;
	}

	public void changeResetCameraTransfrom ()
	{
		canResetCameraRotation = !canResetCameraRotation;
		canResetCameraPosition = !canResetCameraPosition;
		lastTimeSpringRotation = 0;
		lastTimeSpringMovement = 0;
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer != null) {
			mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			playerControllerManager = mainPlayerComponentsManager.getPlayerController ();

			currentPlayerCollider = playerControllerManager.getMainCollider ();

			statesManager = mainPlayerComponentsManager.getPlayerStatesManager ();

			currentfactionManager = mainPlayerComponentsManager.getCharacterFactionManager ();

			devicesManager = mainPlayerComponentsManager.getUsingDevicesSystem ();

			playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

			mainCamera = playerCameraManager.getMainCamera ();
			currentFovValue = mainCamera.fieldOfView;

			playerInput = mainPlayerComponentsManager.getPlayerInputManager ();

			hideCharacterFixedPlaceSystemPlayerManager = mainPlayerComponentsManager.getHideCharacterFixedPlaceSystemPlayerManagement ();

			hideCharacterFixedPlaceSystemPlayerManager.setcurrentFixedHideSystem (this);
		}
	}

	public void checkIfDetected ()
	{
		if (currentPlayer == null) {
			setCurrentPlayer (electronicDeviceManager.getCurrentPlayer ());
		}
			
		//check if the current character is being searching by an enemy when the character is trying to hide
		if (currentPlayer != null && !hidingCharacter) {
			characterIsDetected = currentfactionManager.isCharacterDetectedAsEnemyByOtherFaction (currentPlayer);

			//print ("characterIsDetected " + characterIsDetected);
			if (characterIsDetected) {
				electronicDeviceManager.setDeviceCanBeUsedState (false);
			} else {
				electronicDeviceManager.setDeviceCanBeUsedState (true);
			}
		}
	}

	public void showUnableToHideMessage ()
	{
		//show a message that he can't be hidden until none enemy is searching him
		if (useMessageWhenUnableToHide && currentPlayer != null) {
			devicesManager.checkShowObjectMessage (unableToHideMessage, showMessageTime);
		}
	}

	//GET INPUT FUNCTIONS
	public void inputResetCameraTransform ()
	{
		if (hidingCharacter && !moveCameraManager.isCameraMoving ()) {
			changeResetCameraTransfrom ();
		}
	}

	public void inputSetIncreaseZoomStateByButton (bool state)
	{
		if (hidingCharacter && !moveCameraManager.isCameraMoving () && zoomEnabled) {
			if (state) {
				increaseZoom = true;
			} else {
				increaseZoom = false;
			}
		}
	}

	public void inputSetDecreaseZoomStateByButton (bool state)
	{
		if (hidingCharacter && !moveCameraManager.isCameraMoving () && zoomEnabled) {
			if (state) {
				decreaseZoom = true;
			} else {
				decreaseZoom = false;
			}
		}
	}

	public void inputSetZoomValueByMouseWheel (bool state)
	{
		if (hidingCharacter && !moveCameraManager.isCameraMoving () && zoomEnabled) {
			if (state) {
				lastTimeMouseWheelUsed = Time.time;
				increaseZoom = true;
				mouseWheelUsedPreviously = true;
			} else {
				lastTimeMouseWheelUsed = Time.time;
				decreaseZoom = true;
				mouseWheelUsedPreviously = true;
			}
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
			Gizmos.color = gizmoColor;

			GKC_Utils.drawGizmoArrow (cameraTransform.position, cameraTransform.right * Mathf.Abs (moveCameraLimitsX.x), gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);

			GKC_Utils.drawGizmoArrow (cameraTransform.position, -cameraTransform.right * moveCameraLimitsX.y, gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);

			GKC_Utils.drawGizmoArrow (cameraTransform.position, cameraTransform.up * moveCameraLimitsY.y, gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);

			GKC_Utils.drawGizmoArrow (cameraTransform.position, -cameraTransform.up * Mathf.Abs (moveCameraLimitsY.x), gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);

			Gizmos.color = Color.white;

			Gizmos.DrawLine (cameraTransform.position + cameraTransform.right * Mathf.Abs (moveCameraLimitsX.x), 
				cameraTransform.position + cameraTransform.right * Mathf.Abs (moveCameraLimitsX.x) + cameraTransform.up * moveCameraLimitsY.y);
			Gizmos.DrawLine (cameraTransform.position + cameraTransform.right * Mathf.Abs (moveCameraLimitsX.x) + cameraTransform.up * moveCameraLimitsY.y, 
				cameraTransform.position + cameraTransform.up * moveCameraLimitsY.y);

			Gizmos.DrawLine (cameraTransform.position - cameraTransform.right * moveCameraLimitsX.y, 
				cameraTransform.position - cameraTransform.right * moveCameraLimitsX.y + cameraTransform.up * moveCameraLimitsY.y);
			Gizmos.DrawLine (cameraTransform.position - cameraTransform.right * moveCameraLimitsX.y + cameraTransform.up * moveCameraLimitsY.y, 
				cameraTransform.position + cameraTransform.up * moveCameraLimitsY.y);

			Gizmos.DrawLine (cameraTransform.position - cameraTransform.right * moveCameraLimitsX.y, 
				cameraTransform.position - cameraTransform.right * moveCameraLimitsX.y + cameraTransform.up * moveCameraLimitsY.x);
			Gizmos.DrawLine (cameraTransform.position - cameraTransform.right * moveCameraLimitsX.y + cameraTransform.up * moveCameraLimitsY.x, 
				cameraTransform.position + cameraTransform.up * moveCameraLimitsY.x);

			Gizmos.DrawLine (cameraTransform.position + cameraTransform.right * Mathf.Abs (moveCameraLimitsX.x), 
				cameraTransform.position + cameraTransform.right * Mathf.Abs (moveCameraLimitsX.x) + cameraTransform.up * moveCameraLimitsY.x);
			Gizmos.DrawLine (cameraTransform.position + cameraTransform.right * Mathf.Abs (moveCameraLimitsX.x) + cameraTransform.up * moveCameraLimitsY.x, 
				cameraTransform.position + cameraTransform.up * moveCameraLimitsY.x);
		}
	}
}
