using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class playerCamera : cameraControllerManager
{
	public bool cameraCanBeUsed;
	public Camera mainCamera;
	public Transform mainCameraTransform;
	public Transform pivotCameraTransform;

	public Transform pivotLeanTransform;
	public Transform pivotLeanChildTransform;

	public Transform playerCameraTransform;

	public Transform mainCameraParent;

	public string currentStateName;
	public string defaultStateName;
	public string defaultThirdPersonStateName = "Third Person";
	public string defaultFirstPersonStateName = "First Person";

	public string defaultThirdPersonCrouchStateName = "Third Person Crouch";
	public string defaultFirstPersonCrouchStateName = "First Person Crouch";

	public string defaultThirdPersonAimRightStateName = "Aim Right";
	public string defaultThirdPersonAimLeftStateName = "Aim Left";

	public string defaultMoveCameraAwayStateName = "Move Away";

	public string defaultLockedCameraStateName = "Locked Camera";

	public string defaultAICameraStateName = "AI Camera";

	public string defaultVehiclePassengerStateName = "Vehicle Passenger";

	string previousFreeCameraStateName = "";

	public bool useCustomThirdPersonAimActive;
	string customDefaultThirdPersonAimRightStateName;
	string customDefaultThirdPersonAimLeftStateName;

	public float firstPersonVerticalRotationSpeed = 5;
	public float firstPersonHorizontalRotationSpeed = 5;
	public float thirdPersonVerticalRotationSpeed = 5;
	public float thirdPersonHorizontalRotationSpeed = 5;

	public float currentVerticalRotationSpeed;
	public float currentHorizontalRotationSpeed;

	float originalFirstPersonVerticalRotationSpeed;
	float originalFirstPersonHorizontalRotationSpeed;
	float originalThirdPersonVerticalRotationSpeed;
	float originalThirdPersonHorizontalRotationSpeed;

	public float smoothBetweenState;
	public float maxCheckDist = 0.1f;
	public float movementLerpSpeed = 5;
	public float zoomSpeed = 120;
	public float fovChangeSpeed;

	public float firstPersonVerticalRotationSpeedZoomIn = 2.5f;
	public float firstPersonHorizontalRotationSpeedZoomIn = 2.5f;
	public float thirdPersonVerticalRotationSpeedZoomIn = 2.5f;
	public float thirdPersonHorizontalRotationSpeedZoomIn = 2.5f;

	public bool useEventsOnCameraStateChange;
	public List<cameraStateEventInfo> cameraStateEventInfoList = new List<cameraStateEventInfo> ();

	public List<cameraStateInfo> playerCameraStates = new List<cameraStateInfo> ();

	public cameraSettings settings = new cameraSettings ();

	public bool onGround;
	public bool aimingInThirdPerson;
	public bool crouching;
	public bool firstPersonActive;
	public bool usingZoomOn;
	public bool usingZoomOff;
	public bool cameraCanRotate = true;

	public GameObject playerControllerGameObject;
	public typeOfCamera cameraType;

	public bool cameraCurrentlyLocked;

	public bool changeCameraViewEnabled = true;
	bool originalChangeCameraViewEnabled;

	bool pausePlayerCameraViewChange;

	public enum typeOfCamera
	{
		free,
		locked
	}

	public Vector2 lookAngle;
	public cameraStateInfo currentState;
	public cameraStateInfo lerpState;
	public bool usedByAI;

	public enum sideToAim
	{
		Right = 1,
		Left = -1,
	}

	public sideToAim aimSide;

	public float lastTimeInputUsedWhenLookInPlayerDirectionActive;

	public bool adjustCameraToPreviousCharacterDirectionActive;
	public bool useSmoothCameraTransitionBetweenCharacters;
	public float smoothCameraTransitionBetweenCharactersSpeed;

	public bool setNewCharacterAlwaysInThirdPerson;
	public bool setNewCharacterAlwaysInFirstPerson;
	public bool setNewCharacterAlwaysInPreviousCharacterView;

	GameObject previousCharacterControl;
	Vector3 previousCharacterControlCameraPosition;
	Quaternion previousCharacterControlCameraRotation;
	bool previousCharacterControlInFirstPersonView;

	public float horizontalInput;
	public float verticalInput;

	public bool playerNavMeshEnabled;
	bool touchPlatform;
	Touch currentTouch;
	Vector2 playerLookAngle;

	//Look at target variables
	public Transform lookAtTargetTransform;
	public bool lookAtTargetEnabled;
	public bool canActivateLookAtTargetEnabled = true;
	public Transform targetToLook;
	public bool lookingAtTarget;

	public float lookAtTargetSpeed;
	public float lookCloserAtTargetSpeed = 3;
	public float lookAtTargetSpeed2_5dView = 5;
	public float lookAtTargetSpeedOthersLockedViews = 5;

	public float maxDistanceToFindTarget;
	public bool useLookTargetIcon;
	public GameObject lookAtTargetIcon;
	public RectTransform lookAtTargetIconRectTransform;
	public GameObject lookAtTargetRegularIconGameObject;
	public GameObject lookAtTargetIconWhenNotAiming;

	bool originalUseLookTargetIconValue;

	public List<string> tagToLookList = new List<string> ();
	public LayerMask layerToLook;
	public bool lookOnlyIfTargetOnScreen;
	public bool checkObstaclesToTarget;
	public bool getClosestToCameraCenter;
	public bool useMaxDistanceToCameraCenter;
	public float maxDistanceToCameraCenter;
	public bool useTimeToStopAimAssist;
	public float timeToStopAimAssist;

	public bool useTimeToStopAimAssistLockedCamera;
	public float timeToStopAimAssistLockedCamera;

	public bool lookingAtPoint;
	public Vector3 pointTargetPosition;
	float originalLookAtTargetSpeed;
	float originalMaxDistanceToFindTarget;

	public bool searchPointToLookComponents;
	public LayerMask pointToLookComponentsLayer;

	public bool lookAtBodyPartsOnCharacters;
	public List<string> bodyPartsToLook = new List<string> ();
	Vector3 lookTargetPosition;

	bool originalLookAtBodyPartsOnCharactersValue;

	public bool useObjectToGrabFoundShader;
	public Shader objectToGrabFoundShader;
	public float shaderOutlineWidth;
	public Color shaderOutlineColor;

	outlineObjectSystem currentOutlineObjectSystem;

	public bool pauseOutlineShaderChecking;

	public List<Camera> cameraList = new List<Camera> ();

	bool searchingToTargetOnLockedCamera;

	public bool canActiveLookAtTargetOnLockedCamera;
	bool activeLookAtTargetOnLockedCamera;

	public bool canChangeTargetToLookWithCameraAxis;
	public float minimumCameraDragToChangeTargetToLook = 1;
	public float waitTimeToNextChangeTargetToLook = 0.5f;
	public bool useOnlyHorizontalCameraDragValue;

	//Aim assist variables
	float lastTimeAimAsisst;
	bool usingAutoCameraRotation;
	bool lookintAtTargetByInput;

	Vector3 targetPosition;
	Vector3 screenPoint;
	Transform placeToShoot;
	bool targetOnScreen;

	//Custom editor variables
	public bool showSettings;
	public bool showLookTargetSettings;
	public bool showElements;

	//2.5d variables
	public bool using2_5ViewActive;

	Vector2 moveCameraLimitsX2_5d;
	Vector2 moveCameraLimitsY2_5d;

	bool moveInXAxisOn2_5d;
	Vector3 originalLockedCameraPivotPosition;

	public bool clampAimDirections;
	public int numberOfAimDirections = 8;
	public float minDistanceToCenterClampAim = 1.2f;

	bool useCircleCameraLimit2_5d;
	float circleCameraLimit2_5d;
	float originalCircleCameraLimit2_5d;

	//Top down variables
	public bool useTopDownView;
	Vector2 moveCameraLimitsXTopDown;
	Vector2 moveCameraLimitsYTopDown;

	bool useCircleCameraLimitTopDown;
	float circleCameraLimitTopDown;
	float originalCircleCameraLimitTopDown;

	public bool setLookDirection;
	Vector3 currentCameraMovementPosition;
	public Vector3 targetToFollowForward;

	public bool regularMovementOnBulletTime;

	bool fieldOfViewChanging;
	float AccelerometerUpdateInterval = 0.01f;
	float LowPassKernelWidthInSeconds = 0.001f;
	float lastTimeMoved;
	bool adjustPivotAngle;

	bool horizontalCameraLimitActiveOnGround = true;
	bool horizontalCameraLimitActiveOnAir = true;

	GameObject currentDetectedSurface;
	GameObject currentTargetDetected;
	Transform currentTargetToAim;

	bool isMoving;
	RaycastHit hit;
	Vector3 lowPassValue = Vector3.zero;
	Vector2 acelerationAxis;

	Matrix4x4 calibrationMatrix;

	Transform putCameraOutsideOfPivotTransform;

	bool previouslyOnFreeCamera;

	public playerInputManager playerInput;
	public playerController playerControllerManager;
	public otherPowers powersManager;
	public gravitySystem gravityManager;
	public headBob headBobManager;
	public grabObjects grabObjectsManager;
	public scannerSystem scannerManager;
	public playerWeaponsManager weaponsManager;
	public playerNavMeshSystem playerNavMeshManager;
	public characterStateIconSystem characterStateIconManager;
	public Collider mainCollider;
	public cameraCaptureSystem mainCameraCaptureSystem;

	public Transform hipsTransform;

	Coroutine changeFovCoroutine;
	public Transform targetToFollow;

	Coroutine vehicleCameraMovementCoroutine;

	Coroutine adjustCameraCharacterSmoothlyCoroutine;

	bool rotatingLockedCameraFixedRotationAmountActive;

	Coroutine rotateLockedCameraFixedAmountCoroutine;

	bool smoothFollow;
	bool smoothReturn;
	bool smoothGo;
	bool dead;

	//AI variables
	Vector3 navMeshCurrentLookPos;

	//Locked camera variables

	bool lockedCameraChanged;
	bool lockedCameraCanFollow;

	bool followFixedCameraPosition;

	Transform currentLockedCameraAxisTransform;
	Transform previousLockedCameraAxisTransform;
	lockedCameraSystem.cameraAxis currentLockedCameraAxisInfo;
	lockedCameraSystem.cameraAxis previousLockedCameraAxisInfo;
	Coroutine lockedCameraCoroutine;
	Vector3 lockedCameraFollowPlayerPositionVelocity = Vector3.zero;
	bool lockedCameraMoving;

	bool inputNotPressed;

	Vector2 currentLockedLoonAngle;
	Quaternion currentLockedCameraRotation;
	Quaternion currentLockedPivotRotation;
	bool usingLockedZoomOn;
	float lastTimeLockedSpringRotation;
	float lastTimeLockedSpringMovement;
	Vector3 currentLockedCameraMovementPosition;
	Vector3 currentLockedMoveCameraPosition;
	Vector2 currentLockedLimitLookAngle;

	public Transform lockedCameraElementsParent;
	public Transform lockedMainCameraTransform;
	public Transform lockedCameraAxis;
	public Transform lockedCameraPosition;
	public Transform lockedCameraPivot;

	public Transform lookCameraParent;
	public Transform lookCameraPivot;
	public Transform lookCameraDirection;

	public Transform clampAimDirectionTransform;

	public Transform lookDirectionTransform;

	public Transform auxLockedCameraAxis;

	public RectTransform currentLockedCameraCursor;
	public Vector2 currentLockedCameraCursorSize;

	public bool useLayerToSearchTargets;

	float horizontaLimit;
	float verticalLimit;

	float newVerticalPosition;
	float newVerticalPositionVelocity;

	float newHorizontalPosition;
	float newHorizontalPositionVelocity;

	//Locked Camera Limits Variables
	public bool useCameraLimit;
	public Vector3 currentCameraLimitPosition;

	public bool useWidthLimit;
	public float widthLimitRight;
	public float widthLimitLeft;
	public bool useHeightLimit;
	public float heightLimitUpper;
	public float heightLimitLower;

	public bool useDepthLimit;
	public float depthLimitFront;
	public float depthLimitBackward;

	public setTransparentSurfaces setTransparentSurfacesManager;

	//Camera noise variables
	Vector2 shotCameraNoise;
	bool addNoiseToCamera;

	public bool useSmoothCameraRotation;
	public bool useSmoothCameraRotationThirdPerson;
	public float smoothCameraRotationSpeedVerticalThirdPerson = 10;
	public float smoothCameraRotationSpeedHorizontalThirdPerson = 10;
	public bool useSmoothCameraRotationFirstPerson;
	public float smoothCameraRotationSpeedVerticalFirstPerson = 10;
	public float smoothCameraRotationSpeedHorizontalFirstPerson = 10;

	float currentSmoothCameraRotationSpeedVertical;
	float currentSmoothCameraRotationSpeedHorizontal;
	Quaternion currentPivotRotation;

	Quaternion currentCameraRotation;

	float currentCameraUpRotation;

	public cameraRotationType cameraRotationMode;

	public enum cameraRotationType
	{
		vertical,
		horizontal,
		free
	}

	float currentDeltaTime;
	float currentUpdateDeltaTime;
	float currentFixedUpdateDeltaTime;
	float currentLateUpdateDeltaTime;

	float currentScaleTime;

	Vector2 axisValues;

	bool usingPlayerNavMeshPreviously;

	Vector3 cameraInitialPosition;
	Vector3 offsetInitialPosition;

	public bool zeroGravityModeOn;
	float forwardRotationAngle;
	float targetForwardRotationAngle;
	Quaternion currentForwardRotation;
	public bool canRotateForwardOnZeroGravityModeOn;
	public float rotateForwardOnZeroGravitySpeed;

	public bool freeFloatingModeOn;

	Coroutine lookAtTargetEnabledCoroutine;
	Coroutine fixPlayerZPositionCoroutine;
	Coroutine cameraFovStartAndEndCoroutine;
	Coroutine zoomStateDurationCoroutine;

	List<Transform> targetsListToLookTransform = new List<Transform> ();
	int currentTargetToLookIndex;

	bool driving;

	public bool useEventOnMovingLockedCamera;
	public UnityEvent eventOnStartMovingLockedCamera;
	public UnityEvent eventOnKeepMovingLockedCamera;
	public UnityEvent eventOnStopMovingLockedCamera;
	public bool useEventOnFreeCamereToo;

	bool movingCameraState;
	bool movingCameraPrevioslyState;

	//Camera Bound variables
	Vector3 horizontalOffsetOnSide;
	Vector3 horizontalOffsetOnFaceSideSpeed;

	Vector3 horizontalOffsetOnSideOnMoving;
	Vector3 horizontalOffsetOnFaceSideOnMovingSpeed;

	Vector3 verticalOffsetOnMove;
	Vector3 verticalOffsetOnMoveSpeed;

	public FocusArea focusArea;

	public Vector3 focusTargetPosition;

	bool playerAiming;
	bool playerAimingPreviously;
	float lastTimeAiming;

	float originalMaxDistanceToCameraCenterValue;
	bool originalUsemaxDistanceToCameraCenterValue;

	public bool resetCameraRotationAfterTime;
	public float timeToResetCameraRotation;
	public float resetCameraRotationSpeed;
	float lastTimeCameraRotated;
	bool resetingCameraActive;

	bool resetVerticalCameraRotationActive;

	public bool setExtraCameraRotationEnabled = true;
	public float extraCameraRotationAmount = 180;
	public float extraCameraRotationSpeed = 3;
	public bool useCameraForwardDirection = true;

	Coroutine extraRotationCoroutine;

	Coroutine adjustLockedCameraAxisPositionOnGravityChangeCoroutine;

	bool usingSetTransparentSurfacesPreviously;

	bool rotatingLockedCameraToRight;
	bool rotatingLockedCameraToLeft;

	float lockedCameraRotationDirection;
	bool selectingTargetToLookWithMouseActive;

	float currentAxisValuesMagnitude;
	float lastTimeChangeCurrentTargetToLook;
	float currentLockedCameraAngle;

	public UnityEvent setThirdPersonInEditorEvent;
	public UnityEvent setFirstPersonInEditorEvent;

	public bool useEventOnThirdFirstPersonViewChange;
	public UnityEvent setThirdPersonEvent;
	public UnityEvent setFirstPersonEvent;

	public bool moveCameraPositionWithMouseWheelActive;
	public float moveCameraPositionBackwardWithMouseWheelSpeed = 0.3f;
	public float moveCameraPositionForwardWithMouseWheelSpeed = 0.3f;
	public float minDistanceToChangeToFirstPerson = 0.1f;
	public float maxExtraDistanceOnThirdPerson = 1;

	public bool useCameraMouseWheelStates;
	bool cameraPositionMouseWheelEnabled = true;
	int currentMouseWheelStateIndex;

	bool originalMoveCameraPositionWithMouseWheelActive;

	public List<cameraMouseWheelStates> cameraMouseWheelStatesList = new List<cameraMouseWheelStates> ();

	public GameObject lockedCameraSystemPrefab;
	public GameObject lockedCameraLimitSystemPrefab;

	public List<lockedCameraPrefabsTypes> lockedCameraPrefabsTypesList = new List<lockedCameraPrefabsTypes> ();

	public List<cameraNoiseState> cameraNoiseStateList = new List<cameraNoiseState> ();

	cameraNoiseState currentCameraNoiseState;

	bool cameraNoiseActive;

	string currentCameraShakeStateName;

	public bool leanActive;
	public bool leanToRightActive;
	public bool leanToLeftActive;

	public bool leanInputEnabled = true;

	public bool useRemoteEventsOnLockOn;
	public List<string> remoteEventOnLockOnStart = new List<string> ();
	public List<string> remoteEventOnLockOnEnd = new List<string> ();

	public bool useEventsOnLockOn;
	public UnityEvent eventOnLockOnStart;
	public UnityEvent eventOnLockOnEnd;

	float currentLeanAmount;
	float currentLeanSpeed;

	bool checkToRemoveMainCameraPivotFromLeanPivot;

	public Vector2 mainCanvasSizeDelta;
	public Vector2 halfMainCanvasSizeDelta;
	public bool usingScreenSpaceCamera;

	RectTransform iconRectTransform;
	Vector3 iconPositionViewPoint;
	Vector2 iconPosition2d;

	Quaternion pivotRotation;

	int closestWaypointIndex;

	Transform closestWaypoint;

	public float extraCameraCollisionDistance;

	public bool cameraCollisionAlwaysActive = true;

	bool previouslyInFirstPerson;

	public bool cameraRotationInputEnabled = true;
	public bool cameraActionsInputEnabled = true;

	public bool updateReticleActiveState;
	public bool externalReticleCurrentlyActive;
	public bool cameraReticleCurrentlyActive;
	public bool useCameraReticleThirdPerson;
	public bool useCameraReticleFirstPerson;
	public GameObject cameraReticleGameObject;
	public GameObject mainCameraReticleGameObject;


	public RectTransform mainCustomReticleParent;
	public List<customReticleInfo> customReticleInfoList = new List<customReticleInfo> ();

	Vector2 currentYLimits;

	public bool activateStrafeModeIfNoTargetsToLookFound;
	bool strafeModeActivateFromNoTargetsFoundActive;

	public LayerMask layerToCheckPossibleTargetsBelowCursor;

	Vector2 currentNoiseValues;
	float tick;

	bool useMouseInputToRotateCameraHorizontally;

	Vector3 vector3Zero = Vector3.zero;
	Quaternion quaternionIdentity = Quaternion.identity;

	public string temporaLockedCameraPrefabNmeToPlaceOnScene;

	public bool showDebugPrint;

	bool scannerLocated;

	public bool useSmoothCameraFollow;
	public float smoothCameraFollowSpeed;
	public float smoothCameraFollowSpeedOnAim;
	public float smoothCameraFollowMaxDistance;
	public float smoothCameraFollowMaxDistanceSpeed;
	Vector3 cameraVelocity;

	bool originalUseSmoothCameraFollowValue;

	float originalSmoothCameraFollowSpeed;

	float customPivotOffset;

	float extraPivotOffset;

	bool previouslyAimingThirdPerson;
	bool currentlyAimingThirdPerson;

	bool resetRotationLookAngleYActive;
	public bool useUnscaledTimeOnBulletTime;


	public bool useMultipleTargetFov;
	public float multipleTargetsMinFov = 20;
	public float multipleTargetsMaxFov = 70;

	public float multipleTargetsFovSpeed = 40;

	float currentMultipleTargetsFov;

	public bool useMultipleTargetsYPosition;
	public float multipleTargetsYPositionSpeed;
	public float multipleTargetsMaxHeight = 30;

	public float multipleTargetsHeightMultiplier = 0.5f;

	public float maxMultipleTargetHeight = 30;
	public float minMultipleTargetHeight = -1;

	public bool followingMultipleTargets;

	public Vector3 extraPositionOffset;

	public List<Transform> multipleTargetsToFollowList = new List<Transform> ();

	float multipleTargetsCurrentYPosition;

	bool collisionSurfaceFound;
	float distanceToCamPositionOffset;
	Vector3 directionFromPivotToCamera;
	float collisionDistance;

	Vector3 pivotCameraPosition;

	Vector3 mainCameraPosition;
	Vector3 mainCameraNewPosition;
	Vector3 targetCameraPosition;
	Vector3 cameraNewPosition;

	Vector3 currentCamPositionOffset;

	public bool usePivotCameraCollisionEnabled;
	public float pivotCameraCollisionHeightOffset = 0.7f;

	bool ignorePivotCameraCollisionActive;

	bool pivotCollisionSurfaceFound;

	Vector3 cameraTransformPosition;
	Vector3 currentPivotPositionOffset;

	Vector3 directionFromMainPositionToPivot;

	float distanceToPivotPositionOffset;

	RaycastHit pivotHit;

	float pivotCollisionDistance;

	bool mainCameraAssigned;


	void Awake ()
	{ 
		mainCameraAssigned = mainCamera != null;

		if (!mainCameraAssigned) {
			mainCamera = Camera.main;

			mainCameraAssigned = true;

			setNewMainCameraOnMainParent ();
		}

		if (playerCameraTransform == null) {
			playerCameraTransform = transform;
		}

		for (int i = 0; i < playerCameraStates.Count; i++) {
			playerCameraStates [i].originalCamPositionOffset = playerCameraStates [i].camPositionOffset;
		}
		
		//if the game doesn't starts with the first person view, get the original camera position and other parameters for the camera collision system and 
		//movement ranges
		if (!firstPersonActive) {
			//check if the player uses animator in first person
			playerControllerManager.checkAnimatorIsEnabled (false);		
		} else {
			//check if the player uses animator in first person
			playerControllerManager.checkAnimatorIsEnabled (true);			
		}

		originalThirdPersonVerticalRotationSpeed = thirdPersonVerticalRotationSpeed;
		originalThirdPersonHorizontalRotationSpeed = thirdPersonHorizontalRotationSpeed;
		originalFirstPersonVerticalRotationSpeed = firstPersonVerticalRotationSpeed;
		originalFirstPersonHorizontalRotationSpeed = firstPersonHorizontalRotationSpeed;

		//get the input manager to get every key or touch press

		cameraInitialPosition = mainCameraTransform.localPosition;

		defaultStateName = currentStateName;

		//set the camera state when the game starts
		setCameraState (currentStateName);

		currentState = new cameraStateInfo (lerpState);

		offsetInitialPosition = currentState.camPositionOffset;

		mainCameraTransform.localPosition = currentState.camPositionOffset;
		pivotCameraTransform.localPosition = currentState.pivotPositionOffset;

		if (currentState.pivotRotationOffset != 0) {
			lookAngle.y = currentState.pivotRotationOffset;
		}

		targetToFollow = playerControllerGameObject.transform;

		if (lookAtTargetTransform != null) {
			lookAtTargetTransform.SetParent (null);
		}

		touchPlatform = touchJoystick.checkTouchPlatform ();

		originalLookAtTargetSpeed = lookAtTargetSpeed;

		originalMaxDistanceToFindTarget = maxDistanceToFindTarget;

		if (lockedCameraElementsParent != null) {
			lockedCameraElementsParent.SetParent (null);
			lockedCameraElementsParent.position = vector3Zero;

			lockedCameraElementsParent.rotation = quaternionIdentity;
		}

		originalMaxDistanceToCameraCenterValue = maxDistanceToCameraCenter;
		originalUsemaxDistanceToCameraCenterValue = useMaxDistanceToCameraCenter;

		checkCurrentRotationSpeed ();

		originalMoveCameraPositionWithMouseWheelActive = moveCameraPositionWithMouseWheelActive;

		if (moveCameraPositionWithMouseWheelActive) {
			for (int i = 0; i < cameraMouseWheelStatesList.Count; i++) {
				if (cameraMouseWheelStatesList [i].isCurrentCameraState) {
					currentMouseWheelStateIndex = i;
				}
			}
		}

		if (lookAtTargetIcon != null) {
			lookAtTargetIconRectTransform = lookAtTargetIcon.GetComponent<RectTransform> ();
		}

		originalChangeCameraViewEnabled = changeCameraViewEnabled;

		originalLookAtBodyPartsOnCharactersValue = lookAtBodyPartsOnCharacters;

		originalUseLookTargetIconValue = useLookTargetIcon;

		aimSide = sideToAim.Right;

		setAimModeSide (false);

		originalUseSmoothCameraFollowValue = useSmoothCameraFollow;

		originalSmoothCameraFollowSpeed = smoothCameraFollowSpeed;

		scannerLocated = scannerManager != null;
	}

	void Update ()
	{
		if (!usedByAI) {
			if (!lookingAtTarget) {
				if (cameraCanRotate && !gravityManager.isCharacterRotatingToSurface ()) {
					axisValues = playerInput.getPlayerMouseAxis ();
				}
			}
		}

		if (usedByAI) {
			if (targetToFollow) {
				playerCameraTransform.position = targetToFollow.position;
			}

			return;
		}

		checkCurrentRotationSpeed ();

		currentUpdateDeltaTime = getCurrentDeltaTime ();

		playerAiming = playerControllerManager.isPlayerAiming ();

		if (playerAiming != playerAimingPreviously) {
			playerAimingPreviously = playerAiming;
			lastTimeAiming = Time.time;
		}

		if (playerAiming) {
			if (firstPersonActive) {
				setLastTimeMoved ();
			}
		}

		if (isCameraTypeFree ()) {
			if (cameraCanBeUsed) {
				if (!dead) {
					//shake the camera if the player is moving in the air or accelerating on it
					if (settings.enableShakeCamera && settings.shakingCameraActive) {
						if (settings.accelerateShaking) {
							headBobManager.setState ("High Shaking");
						} else {
							headBobManager.setState (currentCameraShakeStateName);
						}
					}
				}
			}

			//the camera follows the player position
			//if smoothfollow is false, it means that the player is alive
			if (!smoothFollow) {
				//smoothreturn is used to move the camera from the hips to the player controller position smoothly, to avoid change their positions quickly
				if (smoothReturn) {
					Vector3 positionToFollow = targetToFollow.position;

					float speed = 1;
					float distance = GKC_Utils.distance (playerCameraTransform.position, positionToFollow);
					if (distance > 1) {
						speed = distance;
					}

					playerCameraTransform.position = Vector3.MoveTowards (playerCameraTransform.position, positionToFollow, currentUpdateDeltaTime * speed);

					if (playerCameraTransform.position == positionToFollow) {
						smoothReturn = false;
					}
				} else {
					if (!useSmoothCameraFollow || firstPersonActive) {
						//in this state the player is playing normally
						playerCameraTransform.position = targetToFollow.position +
						playerCameraTransform.right * currentState.pivotParentPositionOffset.x +
						playerCameraTransform.up * currentState.pivotParentPositionOffset.y +
						playerCameraTransform.forward * currentState.pivotParentPositionOffset.z;
					}
				}
			} else {
				Vector3 positionToFollow = targetToFollow.position;

				//else follow the ragdoll
				//in this state the player has dead, he cannot move, and the camera follows the skeleton, until the player chooses play again
				if (smoothGo) {
					float speed = 1;
					float distance = GKC_Utils.distance (playerCameraTransform.position, positionToFollow);
					if (distance > 1) {
						speed = distance;
					}

					playerCameraTransform.position = Vector3.MoveTowards (playerCameraTransform.position, positionToFollow - Vector3.up / 1.5f, currentUpdateDeltaTime * speed);

					if (playerCameraTransform.position == positionToFollow - Vector3.up / 1.5f) {
						smoothGo = false;
					}
				} else {
					playerCameraTransform.position = positionToFollow - Vector3.up / 1.5f;
				}
			}
		} else {
			//if current camera mode is locked, set its values according to every fixed camera trigger configuration
			checkCameraPosition ();

			playerCameraTransform.position = targetToFollow.position;

			if (lockedCameraChanged) {
				if (currentLockedCameraAxisInfo.axis != previousLockedCameraAxisTransform) {
					if (!playerControllerManager.isPlayerMoving (0.6f) && !playerControllerManager.isPlayerUsingInput ()) {
						inputNotPressed = true;
					}

					if ((inputNotPressed && (playerControllerManager.isPlayerUsingInput () || !playerControllerManager.isPlayerMoving (0))) ||
					    !currentLockedCameraAxisInfo.changeLockedCameraDirectionOnInputReleased) {

						setCurrentAxisTransformValues (lockedCameraAxis);

						previousLockedCameraAxisTransform = currentLockedCameraAxisInfo.axis;

						lockedCameraChanged = false;
						lockedCameraCanFollow = true;

						inputNotPressed = false;
					}
				} else {
					lockedCameraChanged = false;
				}
			}

			//look at player position
			if (!lockedCameraMoving && currentLockedCameraAxisInfo.lookAtPlayerPosition) {

				calculateLockedCameraLookAtPlayerPosition ();

				lockedCameraPosition.localRotation = Quaternion.Slerp (lockedCameraPosition.localRotation, 
					Quaternion.Euler (new Vector3 (currentLockedLimitLookAngle.x, 0, 0)), currentUpdateDeltaTime * currentLockedCameraAxisInfo.lookAtPlayerPositionSpeed);

				lockedCameraAxis.localRotation = Quaternion.Slerp (lockedCameraAxis.localRotation, 
					Quaternion.Euler (new Vector3 (0, currentLockedLimitLookAngle.y, 0)),	currentUpdateDeltaTime * currentLockedCameraAxisInfo.lookAtPlayerPositionSpeed);

				if (lockedCameraCanFollow) {
					setCurrentAxisTransformValues (lockedCameraAxis);
				}
			}

			//rotate camera with mouse using a range
			if (!lockedCameraMoving && currentLockedCameraAxisInfo.cameraCanRotate && cameraCanBeUsed && !playerAiming) {
				Vector2 currentAxisValues = playerInput.getPlayerMouseAxis ();
				float horizontalMouse = currentAxisValues.x;
				float verticalMouse = currentAxisValues.y;

				currentLockedLoonAngle.x += horizontalMouse * currentHorizontalRotationSpeed;
				currentLockedLoonAngle.y -= verticalMouse * currentVerticalRotationSpeed;

				currentLockedLoonAngle.x = Mathf.Clamp (currentLockedLoonAngle.x, currentLockedCameraAxisInfo.rangeAngleY.x, currentLockedCameraAxisInfo.rangeAngleY.y);

				currentLockedLoonAngle.y = Mathf.Clamp (currentLockedLoonAngle.y, currentLockedCameraAxisInfo.rangeAngleX.x, currentLockedCameraAxisInfo.rangeAngleX.y);

				if (currentLockedCameraAxisInfo.smoothRotation) {

					currentLockedCameraRotation = Quaternion.Euler (currentLockedCameraAxisInfo.originalCameraRotationX + currentLockedLoonAngle.y, 0, 0);

					currentLockedPivotRotation = Quaternion.Euler (0, currentLockedCameraAxisInfo.originalPivotRotationY + currentLockedLoonAngle.x, 0);

					if (currentLockedCameraAxisInfo.useSpringRotation) {
						if (horizontalMouse != 0 || verticalMouse != 0) {
							lastTimeLockedSpringRotation = Time.time;

						}
						if (Time.time > lastTimeLockedSpringRotation + currentLockedCameraAxisInfo.springRotationDelay) {
							currentLockedCameraRotation = Quaternion.Euler (currentLockedCameraAxisInfo.originalCameraRotationX, 0, 0);
							currentLockedPivotRotation = Quaternion.Euler (0, currentLockedCameraAxisInfo.originalPivotRotationY, 0);
							currentLockedLoonAngle = Vector2.zero;
						}
					}

					lockedCameraPosition.localRotation = Quaternion.Slerp (lockedCameraPosition.localRotation, currentLockedCameraRotation, 
						currentUpdateDeltaTime * currentLockedCameraAxisInfo.rotationSpeed);

					lockedCameraAxis.localRotation = Quaternion.Slerp (lockedCameraAxis.localRotation, currentLockedPivotRotation, 
						currentUpdateDeltaTime * currentLockedCameraAxisInfo.rotationSpeed);

				} else {
					lockedCameraPosition.localRotation = Quaternion.Euler (currentLockedCameraAxisInfo.originalCameraRotationX + currentLockedLoonAngle.y, 0, 0);

					lockedCameraAxis.localRotation = Quaternion.Euler (0, currentLockedCameraAxisInfo.originalPivotRotationY + currentLockedLoonAngle.x, 0);
				}
			}

			//move camera up, down, right and left
			if (!lockedCameraMoving && currentLockedCameraAxisInfo.canMoveCamera && cameraCanBeUsed && !playerAiming) {
				Vector2 currentAxisValues = playerInput.getPlayerMouseAxis ();

				float horizontalMouse = currentAxisValues.x;
				float verticalMouse = currentAxisValues.y;

				currentLockedMoveCameraPosition.x += horizontalMouse * currentLockedCameraAxisInfo.moveCameraSpeed;
				currentLockedMoveCameraPosition.y += verticalMouse * currentLockedCameraAxisInfo.moveCameraSpeed;

				currentLockedMoveCameraPosition.x = Mathf.Clamp (currentLockedMoveCameraPosition.x, currentLockedCameraAxisInfo.moveCameraLimitsX.x, currentLockedCameraAxisInfo.moveCameraLimitsX.y);

				currentLockedMoveCameraPosition.y = Mathf.Clamp (currentLockedMoveCameraPosition.y, currentLockedCameraAxisInfo.moveCameraLimitsY.x, currentLockedCameraAxisInfo.moveCameraLimitsY.y);

				Vector3 moveInput = currentLockedMoveCameraPosition.x * Vector3.right +	currentLockedMoveCameraPosition.y * Vector3.up;	

				if (currentLockedCameraAxisInfo.smoothCameraMovement) {
					currentLockedCameraMovementPosition = currentLockedCameraAxisInfo.originalCameraAxisLocalPosition + moveInput;

					if (currentLockedCameraAxisInfo.useSpringMovement) {
						if (horizontalMouse != 0 || verticalMouse != 0) {
							lastTimeLockedSpringMovement = Time.time;

						}
						if (Time.time > lastTimeLockedSpringMovement + currentLockedCameraAxisInfo.springMovementDelay) {
							currentLockedCameraMovementPosition = currentLockedCameraAxisInfo.originalCameraAxisLocalPosition;
							currentLockedMoveCameraPosition = vector3Zero;
						}
					}

					lockedCameraAxis.localPosition = Vector3.MoveTowards (lockedCameraAxis.localPosition, currentLockedCameraMovementPosition, 
						currentUpdateDeltaTime * currentLockedCameraAxisInfo.smoothCameraSpeed);
				} else {
					lockedCameraAxis.localPosition = currentLockedCameraAxisInfo.originalCameraAxisLocalPosition + moveInput;
				}
			}

			if (useEventOnMovingLockedCamera) {
				checkEventOnMoveCamera (playerInput.getPlayerMouseAxis ());
			}

			if (!lockedCameraMoving && cameraCanBeUsed && currentLockedCameraAxisInfo.canRotateCameraHorizontally) {

				if (!currentLockedCameraAxisInfo.useFixedRotationAmount || !rotatingLockedCameraFixedRotationAmountActive) {
					float currentRotationValue = 0;

					if (currentLockedCameraAxisInfo.useMouseInputToRotateCameraHorizontally || useMouseInputToRotateCameraHorizontally) {
						if (!isPlayerAiming ()) {
							currentRotationValue = currentUpdateDeltaTime * currentLockedCameraAxisInfo.horizontalCameraRotationSpeed * playerInput.getPlayerMouseAxis ().x;
						}
					} else {
						if (rotatingLockedCameraToLeft || rotatingLockedCameraToRight) {
							currentRotationValue = currentUpdateDeltaTime * currentLockedCameraAxisInfo.horizontalCameraRotationSpeed * lockedCameraRotationDirection;
						}
					}

					lockedMainCameraTransform.Rotate (0, currentRotationValue, 0);
					lockedCameraPivot.Rotate (0, currentRotationValue, 0);
				}
			}

			if (!lockedCameraMoving && currentLockedCameraAxisInfo.useDistanceToTransformToMoveCameraCloserOrFarther) {
				Vector3 originalCameraPosition = currentLockedCameraAxisInfo.originalCameraLocalPosition;

				float distanceToPlayer = GKC_Utils.distance (currentLockedCameraAxisInfo.transformToCalculateDistance.position, targetToFollow.position);

				distanceToPlayer *= currentLockedCameraAxisInfo.cameraDistanceMultiplier;

				if (currentLockedCameraAxisInfo.useClampCameraBackwardDirection) {
					distanceToPlayer = Mathf.Clamp (distanceToPlayer, 0, currentLockedCameraAxisInfo.clampCameraBackwardDirection);
				}

				Vector3 newPosition = (Vector3.forward * distanceToPlayer);

				if (currentLockedCameraAxisInfo.useDistanceDirectlyProportional) {
					newPosition = -newPosition;
				}

				if (currentLockedCameraAxisInfo.useClampCameraForwardDirection) {
					if (newPosition.z > 0) {
						newPosition = vector3Zero;
					}
				}

				lockedCameraPosition.localPosition = Vector3.Lerp (lockedCameraPosition.localPosition, originalCameraPosition + newPosition, currentUpdateDeltaTime * currentLockedCameraAxisInfo.moveCameraCloserOrFartherSpeed);
			}

			if (!lockedCameraMoving && currentLockedCameraAxisInfo.useZoomByMovingCamera && currentLockedCameraAxisInfo.canUseZoom) {
				Vector3 originalCameraPosition = currentLockedCameraAxisInfo.originalCameraLocalPosition;

				lockedCameraZoomMovingCameraValue = Mathf.Clamp (lockedCameraZoomMovingCameraValue, currentLockedCameraAxisInfo.zoomCameraOffsets.x, currentLockedCameraAxisInfo.zoomCameraOffsets.y); 

				Vector3 newPosition = (currentLockedCameraAxisInfo.originalCameraForward * lockedCameraZoomMovingCameraValue);

				if (currentLockedCameraAxisInfo.zoomByMovingCameraDirectlyProportional) {
					newPosition = -newPosition;
				}
					
				lockedCameraPosition.localPosition = 
					Vector3.Lerp (lockedCameraPosition.localPosition, 
					originalCameraPosition + newPosition, 
					currentUpdateDeltaTime * currentLockedCameraAxisInfo.zoomByMovingCameraSpeed);
			}

			//aim weapon
			if (playerAiming) {
				if (currentLockedCameraCursor != null) {
					float horizontalMouse = 0;
					float verticalMouse = 0;

					if (cameraCanBeUsed && !lookingAtFixedTarget) {
						Vector2 currentAxisValues = playerInput.getPlayerMouseAxis ();

						horizontalMouse = currentAxisValues.x;
						verticalMouse = currentAxisValues.y;
					}

					//if the player is on 2.5d view, set the cursor position on screen where the player will aim
					if (using2_5ViewActive) {
						if (Time.time < lastTimeAiming + 0.01f) {
							return;
						}

						if (!setLookDirection) {
							moveCameraLimitsX2_5d = currentLockedCameraAxisInfo.moveCameraLimitsX2_5d;
							moveCameraLimitsY2_5d = currentLockedCameraAxisInfo.moveCameraLimitsY2_5d;

							useCircleCameraLimit2_5d = currentLockedCameraAxisInfo.useCircleCameraLimit2_5d;
							circleCameraLimit2_5d = currentLockedCameraAxisInfo.circleCameraLimit2_5d;
							originalCircleCameraLimit2_5d = circleCameraLimit2_5d;

							if (targetToLook != null) {
								Vector3 worldPosition = targetToLook.position;

								if (moveInXAxisOn2_5d) {
									lookCameraDirection.position = new Vector3 (worldPosition.x, worldPosition.y, lookCameraDirection.position.z);
								} else {
									lookCameraDirection.position = new Vector3 (lookCameraDirection.position.x, worldPosition.y, worldPosition.z);
								}
							} else {

								//Check the rotation of the player in his local Y axis to check the closest direction to look
								bool lookingAtRight = false;
								float lookingDirectionAngle = 0;

								if (moveInXAxisOn2_5d) {
									lookingDirectionAngle = Vector3.Dot (targetToFollow.forward, lookCameraPivot.right); 
								} else {
									lookingDirectionAngle = Vector3.Dot (targetToFollow.forward, lookCameraPivot.forward); 
								}

								if (lookingDirectionAngle > 0) {
									lookingAtRight = true;
								}

								//The player will look in the right direction of the screen
								if (lookingAtRight) {
									if (moveInXAxisOn2_5d) {
										lookCameraDirection.localPosition = new Vector3 (moveCameraLimitsX2_5d.y, 0, 0);
									} else {
										lookCameraDirection.localPosition = new Vector3 (0, 0, moveCameraLimitsX2_5d.y);
									}
									currentCameraMovementPosition.x = moveCameraLimitsX2_5d.y;
								} 

								//else the player will look in the left direction
								else {
									if (moveInXAxisOn2_5d) {
										lookCameraDirection.localPosition = new Vector3 (moveCameraLimitsX2_5d.x, 0, 0);
									} else {
										lookCameraDirection.localPosition = new Vector3 (0, 0, moveCameraLimitsX2_5d.x);
									}
									currentCameraMovementPosition.x = moveCameraLimitsX2_5d.x;
								}

								if (moveInXAxisOn2_5d) {
									lookCameraDirection.localPosition = new Vector3 (currentCameraMovementPosition.x, 0, lookCameraDirection.localPosition.z);
								} else {
									lookCameraDirection.localPosition = new Vector3 (lookCameraDirection.localPosition.x, 0, currentCameraMovementPosition.x);
								}
							}

							setLookDirection = true;

							pivotCameraTransform.localRotation = quaternionIdentity;
							playerCameraTransform.rotation = targetToFollow.rotation;

							//set the transform position and rotation to follow the lookCameraDirection direction only in the local Y axis, to get the correct direction to look to right or left
							lookDirectionTransform.localRotation = Quaternion.Euler (getLookDirectionTransformRotationValue (playerCameraTransform.forward));

							lookAngle = Vector2.zero;

							clampAimDirectionTransform.localPosition = lookCameraDirection.localPosition;
						}


						//if the camera is following a target, set that direction to the camera to aim directly at that object
						if (targetToLook != null) {
							Vector3 worldPosition = targetToLook.position;

							if (moveInXAxisOn2_5d) {
								lookCameraDirection.position = new Vector3 (worldPosition.x, worldPosition.y, lookCameraDirection.position.z);
							} else {
								lookCameraDirection.position = new Vector3 (lookCameraDirection.position.x, worldPosition.y, worldPosition.z);
							}
						} else {
							//else, the player is aiming freely on the screen
							if (moveInXAxisOn2_5d) {
								currentCameraMovementPosition = horizontalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.right;
							} else {
								currentCameraMovementPosition = horizontalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.forward;
							}

							if (currentLockedCameraAxisInfo.reverseMovementDirection) {
								currentCameraMovementPosition = -currentCameraMovementPosition;
							}

							currentCameraMovementPosition += verticalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.up;

							lookCameraDirection.Translate (lookCameraDirection.InverseTransformDirection (currentCameraMovementPosition));

							//clamp aim direction in 8, 4 or 2 directions
							if (clampAimDirections) {
								bool lookingAtRight = false;
								float lookingDirectionAngle = 0;

								if (moveInXAxisOn2_5d) {
									lookingDirectionAngle = Vector3.Dot (lookDirectionTransform.forward, lookCameraPivot.right); 
								} else {
									lookingDirectionAngle = Vector3.Dot (lookDirectionTransform.forward, lookCameraPivot.forward); 
								}

								if (lookingDirectionAngle > 0) {
									lookingAtRight = true;
								}

								float targetHorizontalValue = 0;

								Vector3 currentDirectionToLook = lookCameraDirection.position - lookDirectionTransform.position;

								if (lookingAtRight) {
									if (moveInXAxisOn2_5d) {
										targetHorizontalValue = Vector3.SignedAngle (currentDirectionToLook, lookCameraPivot.right, lookCameraPivot.forward);
									} else {
										targetHorizontalValue = -Vector3.SignedAngle (currentDirectionToLook, lookCameraPivot.forward, lookCameraPivot.right);
									}
								} else {
									if (moveInXAxisOn2_5d) {
										targetHorizontalValue = Vector3.SignedAngle (currentDirectionToLook, -lookCameraPivot.right, -lookCameraPivot.forward);
									} else {
										targetHorizontalValue = -Vector3.SignedAngle (currentDirectionToLook, -lookCameraPivot.forward, -lookCameraPivot.right);
									}
								}

								Vector2 new2DPosition = Vector2.zero;

								float distanceToCenter = GKC_Utils.distance (lookCameraDirection.localPosition, vector3Zero);

								//print (lookingAtRight + " " + targetHorizontalValue + " " + distanceToCenter);

								if (numberOfAimDirections == 8) {
									if (targetHorizontalValue > 0) {
										if (targetHorizontalValue < 30 || distanceToCenter < minDistanceToCenterClampAim) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.x);
											}
										} else if (targetHorizontalValue > 30 && targetHorizontalValue < 60) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (moveCameraLimitsY2_5d.x, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (moveCameraLimitsY2_5d.x, moveCameraLimitsX2_5d.x);
											}
										} else {
											new2DPosition = new Vector2 (moveCameraLimitsY2_5d.x, 0);
										}
									} else {
										if (targetHorizontalValue > -30 || distanceToCenter < minDistanceToCenterClampAim) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.x);
											}
										} else if (targetHorizontalValue < -30 && targetHorizontalValue > -60) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (moveCameraLimitsY2_5d.y, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (moveCameraLimitsY2_5d.y, moveCameraLimitsX2_5d.x);
											}
										} else {
											new2DPosition = new Vector2 (moveCameraLimitsY2_5d.y, 0);
										}
									}
								} else if (numberOfAimDirections == 4) {
									if (targetHorizontalValue > 0) {
										if (targetHorizontalValue < 45 || distanceToCenter < minDistanceToCenterClampAim) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.x);
											}
										} else {
											new2DPosition = new Vector2 (moveCameraLimitsY2_5d.x, 0);
										}
									} else {
										if (targetHorizontalValue > -45 || distanceToCenter < minDistanceToCenterClampAim) {
											if (lookingAtRight) {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.y);
											} else {
												new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.x);
											}
										} else {
											new2DPosition = new Vector2 (moveCameraLimitsY2_5d.y, 0);
										}
									}
								} else if (numberOfAimDirections == 2) {
									if (lookingAtRight) {
										new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.y);
									} else {
										new2DPosition = new Vector2 (0, moveCameraLimitsX2_5d.x);
									}
								}

								if (moveInXAxisOn2_5d) {
									clampAimDirectionTransform.localPosition = new Vector3 (new2DPosition.y, new2DPosition.x, lookCameraDirection.localPosition.z);
								} else {
									clampAimDirectionTransform.localPosition = new Vector3 (lookCameraDirection.localPosition.x, new2DPosition.x, new2DPosition.y);
								}
							}
						}

						//clamp the aim position to the limits of the current camera
						Vector3 newCameraPosition = lookCameraDirection.localPosition;

						float posX = 0;
						float posY = 0;
						float posZ = 0;

						if (useCircleCameraLimit2_5d) {
							if (currentLockedCameraAxisInfo.scaleCircleCameraLimitWithDistanceToCamera2_5d) {
								float distanceOfCamera = lockedCameraZoomMovingCameraValue * 0.5f;

								circleCameraLimit2_5d = originalCircleCameraLimit2_5d + distanceOfCamera;

								if (currentLockedCameraAxisInfo.circleCameraLimitScaleClamp2_5d != Vector2.zero) {
									circleCameraLimit2_5d = Mathf.Clamp (circleCameraLimit2_5d, 
										currentLockedCameraAxisInfo.circleCameraLimitScaleClamp2_5d.x, currentLockedCameraAxisInfo.circleCameraLimitScaleClamp2_5d.y);
								}

							}

							Vector3 newCirclePosition = Vector3.ClampMagnitude (newCameraPosition, circleCameraLimit2_5d);



							if (currentLockedCameraAxisInfo.moveReticleInFixedCircleRotation2_5d) {
								Vector2 positionOnCircle = Vector2.zero;

								if (moveInXAxisOn2_5d) {
									positionOnCircle = new Vector2 (newCirclePosition.x, newCirclePosition.y);
								} else {
									positionOnCircle = new Vector2 (newCirclePosition.y, newCirclePosition.z);
								}
							
								positionOnCircle.Normalize ();

								positionOnCircle *= circleCameraLimit2_5d;

								if (moveInXAxisOn2_5d) {
									posX = positionOnCircle.x;
									posY = positionOnCircle.y;
								} else {
									posY = positionOnCircle.x;
									posZ = positionOnCircle.y;
								}
							} else {
								posY = newCirclePosition.y;
								posX = newCirclePosition.x;
								posZ = newCirclePosition.z;
							}
						} else {
							posY = Mathf.Clamp (newCameraPosition.y, moveCameraLimitsY2_5d.x, moveCameraLimitsY2_5d.y);

							if (moveInXAxisOn2_5d) {
								posX = Mathf.Clamp (newCameraPosition.x, moveCameraLimitsX2_5d.x, moveCameraLimitsX2_5d.y);
							} else {
								posZ = Mathf.Clamp (newCameraPosition.z, moveCameraLimitsX2_5d.x, moveCameraLimitsX2_5d.y);
							}
						}

						if (moveInXAxisOn2_5d) {
							lookCameraDirection.localPosition = new Vector3 (posX, posY, newCameraPosition.z);
						} else {
							lookCameraDirection.localPosition = new Vector3 (newCameraPosition.x, posY, posZ);
						}

						if (clampAimDirections && targetToLook == null) {
							pointTargetPosition = clampAimDirectionTransform.position;
						} else {
							pointTargetPosition = lookCameraDirection.position;
						}

						//set the position to the UI icon showing the position where teh player aims
						if (usingScreenSpaceCamera) {
							currentLockedCameraCursor.anchoredPosition = getIconPosition (pointTargetPosition);
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (pointTargetPosition);
							currentLockedCameraCursor.transform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
						}


						//set the transform position and rotation to follow the lookCameraDirection direction only in the local Y axis, to get the correct direction to look to right or left
						Vector3 newDirectionToLook = lookCameraDirection.position - lookDirectionTransform.position;

						Vector3 newLookDirectionTransformRotation = getLookDirectionTransformRotationValue (newDirectionToLook);

						lookDirectionTransform.localRotation = Quaternion.Lerp (lookDirectionTransform.localRotation, Quaternion.Euler (newLookDirectionTransformRotation), getCurrentDeltaTime () * 5);
					} 

					//else, the player is on top down view or in point and click mode, so check the cursor position to aim
					else {
						//the current view is top down
						if (useTopDownView) {
							//							if (Time.time < lastTimeAiming + 0.01f) {
							//								return;
							//							}

							if (currentLockedCameraAxisInfo.useCustomPivotHeightOffset) {
								customPivotOffset = currentLockedCameraAxisInfo.customPivotOffset;

								if (currentLockedCameraAxisInfo.useOldSchoolAim) {
									float offsetTarget = 0;

									Vector2 temporalMovementAxis = playerInput.getPlayerRawMovementAxis ();

									if (temporalMovementAxis.y > 0) {
										offsetTarget = currentLockedCameraAxisInfo.aimOffsetUp;
									} else if (temporalMovementAxis.y < 0) {
										offsetTarget = currentLockedCameraAxisInfo.aimOffsetDown;
									}

									extraPivotOffset = Mathf.Lerp (extraPivotOffset, offsetTarget, getCurrentDeltaTime () *
									currentLockedCameraAxisInfo.aimOffsetChangeSpeed);

									customPivotOffset += extraPivotOffset;
								}

								lookCameraPivot.localPosition = new Vector3 (0, customPivotOffset, 0);
							}

							lookCameraParent.localRotation = lockedCameraAxis.localRotation;

							if (!setLookDirection) {
								moveCameraLimitsXTopDown = currentLockedCameraAxisInfo.moveCameraLimitsXTopDown;
								moveCameraLimitsYTopDown = currentLockedCameraAxisInfo.moveCameraLimitsYTopDown;

								useCircleCameraLimitTopDown = currentLockedCameraAxisInfo.useCircleCameraLimitTopDown;
								circleCameraLimitTopDown = currentLockedCameraAxisInfo.circleCameraLimitTopDown;
								originalCircleCameraLimitTopDown = circleCameraLimitTopDown;

								//								if (targetToLook) {
								//									print (targetToLook.name);
								//								}

								playerCameraTransform.rotation = targetToFollow.rotation;

								if (targetToLook != null) {
									Vector3 worldPosition = mainCamera.ScreenToWorldPoint (currentLockedCameraCursor.position);
									lookCameraDirection.position = new Vector3 (worldPosition.x, lookCameraDirection.position.y, worldPosition.z);
								} else {
									if (currentLockedCameraAxisInfo.use8DiagonalAim) {
										float currentPlayerRotationY = targetToFollow.eulerAngles.y + currentLockedCameraAxisInfo.extraTopDownYRotation;
									
										if (currentPlayerRotationY > 180) {
											currentPlayerRotationY -= 360;
										}

										float currentPlayerRotationYABS = Mathf.Abs (currentPlayerRotationY);

										//check the current forward direction in Y axis to aim to the closes direction in an angle diviced in 8 directions, so every angles is 360/8=45
										if (currentPlayerRotationYABS < 45) {
											if (currentPlayerRotationYABS > 22.5f) {
												if (currentPlayerRotationY > 0) {
													currentCameraMovementPosition.x = moveCameraLimitsXTopDown.y;
												} else {
													currentCameraMovementPosition.x = moveCameraLimitsXTopDown.x;
												}
												currentCameraMovementPosition.y = moveCameraLimitsYTopDown.y;
											} else {
												currentCameraMovementPosition.x = 0;
												currentCameraMovementPosition.y = moveCameraLimitsYTopDown.y;
											}
										} else if (currentPlayerRotationYABS > 45 && currentPlayerRotationYABS < 135) {
											if (currentPlayerRotationY > 0) {
												currentCameraMovementPosition.x = moveCameraLimitsXTopDown.y;
											} else {
												currentCameraMovementPosition.x = moveCameraLimitsXTopDown.x;
											}
											currentCameraMovementPosition.y = 0;
										} else if (currentPlayerRotationYABS > 135) {
											if (currentPlayerRotationYABS < 157.5f) {
												if (currentPlayerRotationY > 0) {
													currentCameraMovementPosition.x = moveCameraLimitsXTopDown.y;
												} else {
													currentCameraMovementPosition.x = moveCameraLimitsXTopDown.x;
												}
												currentCameraMovementPosition.y = moveCameraLimitsYTopDown.x;
											} else {
												currentCameraMovementPosition.x = 0;
												currentCameraMovementPosition.y = moveCameraLimitsYTopDown.x;
											}
										}

										lookCameraDirection.localPosition = new Vector3 (currentCameraMovementPosition.x / 2, lookCameraDirection.localPosition.y, currentCameraMovementPosition.y / 2);
									} else {
										currentCameraMovementPosition = targetToFollow.position + targetToFollowForward * 4;
										lookCameraDirection.position = new Vector3 (currentCameraMovementPosition.x, lookCameraDirection.position.y, currentCameraMovementPosition.z);
									}
								}

								setLookDirection = true;
							}

							if (targetToLook != null) {
								Vector3 worldPosition = targetToLook.position;
								lookCameraDirection.position = new Vector3 (worldPosition.x, lookCameraDirection.position.y, worldPosition.z);
							} else {
								currentCameraMovementPosition = 
									horizontalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.right +
								verticalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.forward;	

								lookCameraDirection.Translate (currentCameraMovementPosition);
							}

							Vector3 newCameraPosition = lookCameraDirection.localPosition;

							float posX, posZ;

							if (useCircleCameraLimitTopDown) {
								if (currentLockedCameraAxisInfo.scaleCircleCameraLimitWithDistanceToCamera) {
									float distanceOfCamera = lockedCameraZoomMovingCameraValue * 0.5f;
							
									circleCameraLimitTopDown = originalCircleCameraLimitTopDown + distanceOfCamera;

									if (currentLockedCameraAxisInfo.circleCameraLimitScaleClamp != Vector2.zero) {
										circleCameraLimitTopDown = Mathf.Clamp (circleCameraLimitTopDown, 
											currentLockedCameraAxisInfo.circleCameraLimitScaleClamp.x, currentLockedCameraAxisInfo.circleCameraLimitScaleClamp.y);
									}
								}

								Vector3 newCirclePosition = Vector3.ClampMagnitude (newCameraPosition, circleCameraLimitTopDown);

								if (currentLockedCameraAxisInfo.moveReticleInFixedCircleRotationTopDown) {
									Vector2 positionOnCircle = new Vector2 (newCirclePosition.x, newCirclePosition.z);

									positionOnCircle.Normalize ();

									positionOnCircle *= originalCircleCameraLimitTopDown;

									posX = positionOnCircle.x;
									posZ = positionOnCircle.y;
								} else {
									posX = newCirclePosition.x;
									posZ = newCirclePosition.z;
								}
							} else {
								posX = Mathf.Clamp (newCameraPosition.x, moveCameraLimitsXTopDown.x, moveCameraLimitsXTopDown.y);
								posZ = Mathf.Clamp (newCameraPosition.z, moveCameraLimitsYTopDown.x, moveCameraLimitsYTopDown.y);
							}
								
							lookCameraDirection.localPosition = new Vector3 (posX, newCameraPosition.y, posZ);

							pointTargetPosition = lookCameraDirection.position;

							if (currentLockedCameraAxisInfo.reticleOffset > 0) {
								pointTargetPosition -= Vector3.up * currentLockedCameraAxisInfo.reticleOffset;
							}

							if (usingScreenSpaceCamera) {
								currentLockedCameraCursor.anchoredPosition = getIconPosition (pointTargetPosition);
							} else {
								screenPoint = mainCamera.WorldToScreenPoint (pointTargetPosition);
								currentLockedCameraCursor.transform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
							}

							if (currentLockedCameraAxisInfo.checkPossibleTargetsBelowCursor) {
								//check objects below the current camera cursor on the screen to check possible targets to aim, getting their proper place to shoot position
								Ray newRay = mainCamera.ScreenPointToRay (currentLockedCameraCursor.position);

								if (Physics.Raycast (newRay, out hit, Mathf.Infinity, layerToCheckPossibleTargetsBelowCursor)) {
									currentDetectedSurface = hit.collider.gameObject;
								} else {
									currentDetectedSurface = null;
								}

								if (currentTargetDetected != currentDetectedSurface) {
									currentTargetDetected = currentDetectedSurface;

									if (currentTargetDetected != null) {
										currentTargetToAim = applyDamage.getPlaceToShoot (currentTargetDetected);
										//										if (currentTargetToAim) {
										//											print (currentTargetToAim.name);
										//										}
									} else {
										currentTargetToAim = null;
									}
								}

								if (currentTargetToAim != null) {
									pointTargetPosition = currentTargetToAim.position;
								}
							}
						} 

						//the player is on point and click camera type
						else {
							if (playerNavMeshEnabled) {
								if (cameraCanBeUsed) {
									int touchCount = Input.touchCount;

									if (!touchPlatform) {
										touchCount++;
									}

									for (int i = 0; i < touchCount; i++) {
										if (!touchPlatform) {
											currentTouch = touchJoystick.convertMouseIntoFinger ();
										} else {
											currentTouch = Input.GetTouch (i);
										}

										if (touchPlatform) {
											if (currentTouch.phase == TouchPhase.Began) {
												currentLockedCameraCursor.position = currentTouch.position;
											}
										} else {
											currentLockedCameraCursor.position = currentTouch.position;
										}
									}
								}
							} else {
								if (!setLookDirection) {
									setLookDirection = true;
									playerCameraTransform.rotation = targetToFollow.rotation;

									Vector3 positionToFollow = targetToFollow.position;

									if (targetToLook == null) {
										bool surfaceFound = false;
										bool surfaceFoundOnScreen = false;
										Vector2 screenPos = Vector2.zero;

										if (Physics.Raycast (positionToFollow + targetToFollow.up, targetToFollow.forward, out hit, Mathf.Infinity, layerToLook)) {
											surfaceFound = true;

											screenPos = mainCamera.WorldToScreenPoint (hit.point);
											if (screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height) {
												surfaceFoundOnScreen = true;
											}
										}

										if (!surfaceFound || !surfaceFoundOnScreen) {
											screenPos = mainCamera.WorldToScreenPoint (positionToFollow + targetToFollow.forward * 3);

											if (settings.showCameraGizmo) {
												Debug.DrawLine (positionToFollow, positionToFollow + targetToFollow.forward * 3, Color.white, 5);
											}
										}

										if (currentLockedCameraCursor != null) {
											currentLockedCameraCursor.position = screenPos;
										}

										lookAngle = Vector2.zero;

										pivotCameraTransform.localRotation = quaternionIdentity;
										playerCameraTransform.rotation = targetToFollow.rotation;
									}
								}

								currentLockedCameraCursor.Translate (new Vector3 (horizontalMouse, verticalMouse, 0) * currentLockedCameraAxisInfo.moveCameraCursorSpeed);
							}

							Vector3 newCameraPosition = currentLockedCameraCursor.position;
							newCameraPosition.x = Mathf.Clamp (newCameraPosition.x, currentLockedCameraCursorSize.x, horizontaLimit);
							newCameraPosition.y = Mathf.Clamp (newCameraPosition.y, currentLockedCameraCursorSize.y, verticalLimit);
							currentLockedCameraCursor.position = new Vector3 (newCameraPosition.x, newCameraPosition.y, 0);

							Ray newRay = mainCamera.ScreenPointToRay (currentLockedCameraCursor.position);

							if (Physics.Raycast (newRay, out hit, Mathf.Infinity, layerToLook)) {
								pointTargetPosition = hit.point;
							} else {
								print ("look at screen point in work position");
							}
						}
					}
				}
			} else {
				if (setLookDirection) {
					if (using2_5ViewActive) {
						lookCameraDirection.localPosition = vector3Zero;

						playerCameraTransform.rotation = targetToFollow.rotation;
					}

					if (useTopDownView) {
						if (currentLockedCameraAxisInfo.showCameraCursorWhenNotAiming) {
							settingShowCameraCursorWhenNotAimingBackToActive = true;

							activateShowCameraCursorWhenNotAimingState ();

							settingShowCameraCursorWhenNotAimingBackToActive = false;
						} else {
							lookCameraDirection.localPosition = new Vector3 (0, lookCameraDirection.localPosition.y, 0);
						}
					}
				}

				currentCameraMovementPosition = vector3Zero;
				setLookDirection = false;

				if (!lockedCameraMoving) {
					if (using2_5ViewActive) {
						playerCameraTransform.rotation = targetToFollow.rotation;
					}

					if (useTopDownView) {
						if (currentLockedCameraAxisInfo.showCameraCursorWhenNotAiming && currentLockedCameraCursor) {

							Vector2 currentAxisValues = playerInput.getPlayerMouseAxis ();
							float horizontalMouse = currentAxisValues.x;
							float verticalMouse = currentAxisValues.y;

							lookCameraParent.localRotation = lockedCameraAxis.localRotation;

							if (targetToLook != null) {
								Vector3 worldPosition = targetToLook.position;
								lookCameraDirection.position = new Vector3 (worldPosition.x, lookCameraDirection.position.y, worldPosition.z);
							} else {
								currentCameraMovementPosition = 
									horizontalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.right +
								verticalMouse * currentLockedCameraAxisInfo.moveCameraCursorSpeed * Vector3.forward;	

								lookCameraDirection.Translate (currentCameraMovementPosition);
							}

							Vector3 newCameraPosition = lookCameraDirection.localPosition;
							float posX, posZ;

							posX = Mathf.Clamp (newCameraPosition.x, moveCameraLimitsXTopDown.x, moveCameraLimitsXTopDown.y);
							posZ = Mathf.Clamp (newCameraPosition.z, moveCameraLimitsYTopDown.x, moveCameraLimitsYTopDown.y);

							lookCameraDirection.localPosition = new Vector3 (posX, newCameraPosition.y, posZ);

							pointTargetPosition = lookCameraDirection.position;

							if (usingScreenSpaceCamera) {
								currentLockedCameraCursor.anchoredPosition = getIconPosition (pointTargetPosition);
							} else {
								screenPoint = mainCamera.WorldToScreenPoint (pointTargetPosition);
								currentLockedCameraCursor.transform.position = screenPoint;
							}

							if (currentLockedCameraAxisInfo.checkPossibleTargetsBelowCursor) {
								//check objects below the current camera cursor on the screen to check possible targets to aim, getting their proper place to shoot position
								Ray newRay = mainCamera.ScreenPointToRay (currentLockedCameraCursor.position);

								if (Physics.Raycast (newRay, out hit, Mathf.Infinity, layerToCheckPossibleTargetsBelowCursor)) {
									currentDetectedSurface = hit.collider.gameObject;
								} else {
									currentDetectedSurface = null;
								}

								if (currentTargetDetected != currentDetectedSurface) {
									currentTargetDetected = currentDetectedSurface;

									if (currentTargetDetected != null) {
										currentTargetToAim = applyDamage.getPlaceToShoot (currentTargetDetected);
										//										if (currentTargetToAim) {
										//											print (currentTargetToAim.name);
										//										}
									} else {
										currentTargetToAim = null;
									}
								}

								if (currentTargetToAim != null) {
									pointTargetPosition = currentTargetToAim.position;
								}
							}
						} else {
							if (playerControllerManager.isPlayerUsingMeleeWeapons ()) {
								if (playerControllerManager.isStrafeModeActive ()) {
									bool setPlayerCameraRotation = true;

									if (!currentLockedCameraAxisInfo.showCameraCursorWhenNotAiming) {
										if (lookingAtTarget && targetToLook != null) {
											useMouseInputToRotateCameraHorizontally = false;

											Vector3 worldPosition = targetToLook.position;
											lookCameraDirection.position = new Vector3 (worldPosition.x, lookCameraDirection.position.y, worldPosition.z);

											setPlayerCameraRotation = false;
										} else {
											useMouseInputToRotateCameraHorizontally = true;
										}
									}

									if (setPlayerCameraRotation) {
										playerCameraTransform.rotation = lockedMainCameraTransform.rotation;
									}
								} else {
									playerCameraTransform.rotation = targetToFollow.rotation;
								}
							} else {
								if (!lookingAtTarget) {
									playerCameraTransform.rotation = targetToFollow.rotation;
								}
							}
						}
					}
				}

				targetToFollowForward = targetToFollow.forward;
			}
		}
	}

	public Vector3 getLookDirectionTransformRotationValue (Vector3 forwardDirection)
	{
		float lookDirectionTransformRotationAngle = 0;

		if (moveInXAxisOn2_5d) {
			lookDirectionTransformRotationAngle = Vector3.Dot (forwardDirection, lookCameraDirection.right); 
		} else {
			lookDirectionTransformRotationAngle = Vector3.Dot (forwardDirection, lookCameraDirection.forward); 
		}

		Vector3 lookDirectionTransformRotation = vector3Zero;

		if (lookDirectionTransformRotationAngle < 0) {
			if (moveInXAxisOn2_5d) {
				lookDirectionTransformRotation = Vector3.up * (-90);
			} else {
				lookDirectionTransformRotation = Vector3.up * (-180);
			}
		} else {
			if (moveInXAxisOn2_5d) {
				lookDirectionTransformRotation = Vector3.up * (90);
			}
		}

		return lookDirectionTransformRotation;
	}

	public void checkEventOnMoveCamera (Vector2 currentAxisValues)
	{
		if (currentAxisValues.magnitude != 0) {
			movingCameraState = true;
			eventOnKeepMovingLockedCamera.Invoke ();
		} else {
			movingCameraState = false;
		}

		if (movingCameraState != movingCameraPrevioslyState) {
			movingCameraPrevioslyState = movingCameraState;

			if (movingCameraPrevioslyState) {
				eventOnStartMovingLockedCamera.Invoke ();
			} else {
				eventOnStopMovingLockedCamera.Invoke ();
			}
		}
	}

	public Transform getCurrentLookDirection2_5d ()
	{
		return lookCameraDirection;
	}

	public Transform getCurrentLookDirectionTopDown ()
	{
		return lookCameraDirection;
	}

	public string getDefaultThirdPersonStateName ()
	{
		return defaultThirdPersonStateName;
	}

	public string getDefaultFirstPersonStateName ()
	{
		return defaultFirstPersonStateName;
	}

	public string getDefaultVehiclePassengerStateName ()
	{
		return defaultVehiclePassengerStateName;
	}

	public void checkUpdateReticleActiveState (bool state)
	{
		if (updateReticleActiveState) {
			externalReticleCurrentlyActive = state;

			cameraReticleCurrentlyActive = !externalReticleCurrentlyActive;

			updateReticleActive ();
		}
	}

	public void updateReticleActive ()
	{
		if (updateReticleActiveState) {
			if (isCameraTypeFree ()) {
				if (firstPersonActive) {
					if (useCameraReticleFirstPerson) {
						if (cameraReticleGameObject.activeSelf != cameraReticleCurrentlyActive) {
							cameraReticleGameObject.SetActive (cameraReticleCurrentlyActive);
						}
					} else {
						if (cameraReticleGameObject.activeSelf) {
							cameraReticleGameObject.SetActive (false);
						}
					}
				} else {
					if (useCameraReticleThirdPerson) {
						if (cameraReticleGameObject.activeSelf != cameraReticleCurrentlyActive) {
							cameraReticleGameObject.SetActive (cameraReticleCurrentlyActive);
						}
					} else {
						if (cameraReticleGameObject.activeSelf) {
							cameraReticleGameObject.SetActive (false);
						}
					}
				}
			} else {
				if (cameraReticleGameObject.activeSelf) {
					cameraReticleGameObject.SetActive (false);
				}
			}
		}
	}

	public void enableOrDisableMainCameraReticle (bool state)
	{
		if (mainCameraReticleGameObject.activeSelf != state) {
			mainCameraReticleGameObject.SetActive (state);
		}
	}

	public void enableCustomReticle (string reticleName)
	{
		enableOrDisableCustomReticle (true, reticleName);
	}

	public void disableCustomReticle (string reticleName)
	{
		enableOrDisableCustomReticle (false, reticleName);
	}

	public void disableAllCustomReticle ()
	{
		enableOrDisableCustomReticle (false, "");
	}

	public void enableOrDisableCustomReticle (bool state, string reticleName)
	{
		if (state) {
			if (isCameraTypeFree ()) {
				mainCustomReticleParent.SetParent (lookAtTargetIconRectTransform.parent);
			} else {
				mainCustomReticleParent.SetParent (lookAtTargetIconRectTransform);
			}

			mainCustomReticleParent.anchoredPosition = Vector2.zero;
		}

		if (reticleName == "" && !state) {
			for (int i = 0; i < customReticleInfoList.Count; i++) {
				if (customReticleInfoList [i].customReticleObject.activeSelf != false) {
					customReticleInfoList [i].customReticleObject.SetActive (false);
					customReticleInfoList [i].isCurrentReticle = false;
				}
			}

			checkUpdateReticleActiveState (false);

			return;
		}

		int currentIndex = -1;

		currentIndex = customReticleInfoList.FindIndex (s => s.Name == reticleName);

		if (currentIndex > -1) {
			if (customReticleInfoList [currentIndex].customReticleObject.activeSelf != state) {
				customReticleInfoList [currentIndex].customReticleObject.SetActive (state);
				customReticleInfoList [currentIndex].isCurrentReticle = true;
			}
		
			for (int i = 0; i < customReticleInfoList.Count; i++) {
				if (i != currentIndex) {
					if (customReticleInfoList [i].customReticleObject.activeSelf != false) {
						customReticleInfoList [i].customReticleObject.SetActive (false);
						customReticleInfoList [i].isCurrentReticle = false;
					}
				}
			}

			if (state) {
				checkUpdateReticleActiveState (true);
			} else {
				checkUpdateReticleActiveState (false);
			}
		}
	}

	public void setCurrentLockedCameraCursor (RectTransform currentCursor)
	{
		if (currentCursor != null) {
			currentLockedCameraCursor = currentCursor;
			currentLockedCameraCursorSize = currentLockedCameraCursor.sizeDelta;

			Vector2 currentResolution = GKC_Utils.getScreenResolution ();

			horizontaLimit = currentResolution.x - currentLockedCameraCursorSize.x;
			verticalLimit = currentResolution.y - currentLockedCameraCursorSize.y;
		} else {
			if (currentLockedCameraCursor != null) {
				currentLockedCameraCursor.anchoredPosition = Vector2.zero;
				currentLockedCameraCursor = null;
			}
		}
	}

	public int sortTargetsListToLookTransformByDistance (Transform a, Transform b)
	{
		Vector2 centerScreen = getScreenCenter ();

		Vector3	newScreenPoint = vector3Zero;

		if (usingScreenSpaceCamera) {
			newScreenPoint = mainCamera.WorldToViewportPoint (a.position);
		} else {
			newScreenPoint = mainCamera.WorldToScreenPoint (a.position);
		}

		float newDistanceX = GKC_Utils.distance (new Vector2 (newScreenPoint.x, newScreenPoint.y), centerScreen);

		if (usingScreenSpaceCamera) {
			newScreenPoint = mainCamera.WorldToViewportPoint (b.position);
		} else {
			newScreenPoint = mainCamera.WorldToScreenPoint (b.position);
		}

		float newDistanceY = GKC_Utils.distance (new Vector2 (newScreenPoint.x, newScreenPoint.y), centerScreen);

		return newDistanceX.CompareTo (newDistanceY);
	}

	public void checkChangeCurrentTargetToLook ()
	{
		if (canChangeTargetToLookWithCameraAxis && cameraCanBeUsed) {
			axisValues = playerInput.getPlayerMouseAxis ();

			currentAxisValuesMagnitude = Math.Abs (axisValues.magnitude);

			if (currentAxisValuesMagnitude > minimumCameraDragToChangeTargetToLook) {
				if (!selectingTargetToLookWithMouseActive) {

					float closestAngle = 360;

					float closestDistance = Mathf.Infinity;

					int targetToLookIndex = -1;

					//print (axisValues + "\n\n\n");

					List<Transform> newTargetsListToLookTransform = new List<Transform> ();

					List<Transform> placeToShootParent = new List<Transform> ();

					for (int i = 0; i < targetsListToLookTransform.Count; i++) {
						if (targetsListToLookTransform [i] != null) {
							Transform newPlaceToShoot = applyDamage.getPlaceToShoot (targetsListToLookTransform [i].gameObject);

							if (targetToLook != newPlaceToShoot) {
								if (newPlaceToShoot == null) {
									newPlaceToShoot = targetsListToLookTransform [i];
								}

								placeToShootParent.Add (targetsListToLookTransform [i]);

								newTargetsListToLookTransform.Add (newPlaceToShoot);
							}
						}
					}

					newTargetsListToLookTransform.Sort (sortTargetsListToLookTransformByDistance);

					placeToShootParent.Sort (sortTargetsListToLookTransformByDistance);

					Vector2 centerScreen = getScreenCenter ();

					for (int i = 0; i < newTargetsListToLookTransform.Count; i++) {

						Transform newPlaceToShoot = newTargetsListToLookTransform [i];

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (newPlaceToShoot.position);
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (newPlaceToShoot.position);
						}

						if (useOnlyHorizontalCameraDragValue) {
							Vector2 targeDirection = new Vector2 (screenPoint.x, screenPoint.y) - new Vector2 (centerScreen.x, centerScreen.y);

							float newAngle = Vector2.Dot (Vector2.right, targeDirection);

							if ((newAngle > 0 && axisValues.x > 0) || (newAngle < 0 && axisValues.x < 0)) {

								float newDistance = GKC_Utils.distance (new Vector2 (screenPoint.x, 0), new Vector2 (centerScreen.x, 0));

								if (newDistance < closestDistance) {
									closestDistance = newDistance;
									targetToLookIndex = i;
								}
							}
						} else {
							float newAngle = Vector2.Angle (axisValues, (new Vector2 (screenPoint.x, screenPoint.y) - centerScreen));
							//print (screenPoint + " " + newPlaceToShoot.name + " " + newPlaceToShoot.name + " " + newAngle);

							if (newAngle < closestAngle) {
								closestAngle = newAngle;

								targetToLookIndex = i;
							}
						}
					}

					if (newTargetsListToLookTransform.Count > 0 && targetToLookIndex > -1) {
						checkRemoveEventOnLockOnEnd ();

						currentTargetToLookIndex = targetToLookIndex;

						//print ("new " + newTargetsListToLookTransform [currentTargetToLookIndex].name);

						checkTargetToLookShader (placeToShootParent [currentTargetToLookIndex]);

						pauseOutlineShaderChecking = true;

						setLookAtTargetState (true, newTargetsListToLookTransform [currentTargetToLookIndex]);

						lastTimeChangeCurrentTargetToLook = Time.time;

						pauseOutlineShaderChecking = false;

						checkRemoveEventOnLockedOnStart (placeToShootParent [currentTargetToLookIndex]);

					} else {
						setLookAtTargetState (true, null);
					}

					selectingTargetToLookWithMouseActive = true;
				}
			} else {
				if (selectingTargetToLookWithMouseActive) {
					if (currentAxisValuesMagnitude < 0.09f && Time.time > lastTimeChangeCurrentTargetToLook + waitTimeToNextChangeTargetToLook) {
						selectingTargetToLookWithMouseActive = false;
					}
				}
			}
		}
	}

	void LateUpdate ()
	{
		currentLateUpdateDeltaTime = getCurrentDeltaTime ();

		//convert the mouse input in the tilt angle for the camera or the input from the touch screen depending of the settings
		if (!usedByAI) {
			if (isCameraTypeFree ()) {
				if (cameraCanBeUsed) {
					if (!dead) {
	
						updatePivotTransformPosition ();

						checkCameraPosition ();

						if (currentState.leanEnabled && (leanActive || checkToRemoveMainCameraPivotFromLeanPivot)) {
							if (pivotLeanTransform != null) {
								if (leanToRightActive) {
									currentLeanAmount = -currentState.maxLeanAngle;
								} else if (leanToLeftActive) {
									currentLeanAmount = currentState.maxLeanAngle;
								} else {
									currentLeanAmount = 0;
								}

								currentLeanSpeed = currentState.leanSpeed;
							} else {
								currentLeanAmount = 0;

								currentLeanSpeed = currentState.leanResetSpeed;
							}

							if (currentLeanAmount != 0) {
								if (leanToRightActive) {
									if (Physics.Raycast (playerCameraTransform.position + currentState.pivotPositionOffset, mainCameraTransform.right, out hit, currentState.leanRaycastDistance, settings.layer)) {
										currentLeanAmount = -currentState.leanAngleOnSurfaceFound;
									}
								} else if (leanToLeftActive) {
									if (Physics.Raycast (playerCameraTransform.position + currentState.pivotPositionOffset, -mainCameraTransform.right, out hit, currentState.leanRaycastDistance, settings.layer)) {
										currentLeanAmount = currentState.leanAngleOnSurfaceFound;
									}
								}
							}

							if (!leanActive) {
								currentLeanAmount = 0;
							}

							pivotLeanTransform.localRotation = 
								Quaternion.Lerp (pivotLeanTransform.localRotation, Quaternion.Euler (new Vector3 (0, 0, currentLeanAmount)), currentUpdateDeltaTime * currentLeanSpeed);

							if (!leanActive) {

								if (checkToRemoveMainCameraPivotFromLeanPivot && pivotLeanTransform.localEulerAngles == vector3Zero) {
									removeMainCameraPivotFromLeanPivot ();

									checkToRemoveMainCameraPivotFromLeanPivot = false;
								}
							}
						}
					}
				}
			}

			if (lookingAtTarget) {
				usingAutoCameraRotation = true;

				if (targetToLook != null && isCameraTypeFree ()) {
					checkChangeCurrentTargetToLook ();
				}
			} else { 
				usingAutoCameraRotation = false;

				if (cameraCanRotate && !gravityManager.isCharacterRotatingToSurface ()) {

					horizontalInput = axisValues.x;
					verticalInput = axisValues.y;

					if (horizontalInput != 0 || verticalInput != 0) {
						setLastTimeCameraRotated ();

						resetingCameraActive = false;
					}

					if (playerControllerManager.isPlayerMoving (0)) {
						setLastTimeCameraRotated ();
					}

					if (useEventOnFreeCamereToo) {
						checkEventOnMoveCamera (playerInput.getPlayerMouseAxis ());
					}
				} 
			}

			if (playerNavMeshEnabled && !playerAiming) {
				usingAutoCameraRotation = true;
			}
		} else {
			usingAutoCameraRotation = true;
		}

		if (usingAutoCameraRotation) {

			setLastTimeCameraRotated ();

			if (lookingAtTarget) {
				lookAtTarget ();

				if (!lookintAtTargetByInput) {
					if (useTimeToStopAimAssist) {
						if (Time.time > lastTimeAimAsisst + timeToStopAimAssist) {
							setLookAtTargetState (false, null);
						}
					}
				} else {
					if (!isCameraTypeFree ()) {
						if (useTimeToStopAimAssistLockedCamera && targetToLook != null && isPlayerAiming ()) {
							if (Time.time > lastTimeAimAsisst + timeToStopAimAssistLockedCamera) {
								setTargetToLook (null);

								checkTargetToLookShader (null);
							}
						}
					}
				}
			}

			//get horizontal input direction for the camera when the lock on mode is active
			Vector3 forward = navMeshCurrentLookPos;
			float targetHorizontalValue = 0;

			float newAngle = Vector3.Angle (forward, playerCameraTransform.forward);

			if (newAngle >= 1) {

				if (using2_5ViewActive) {

					targetHorizontalValue = Vector3.SignedAngle (lookDirectionTransform.forward, playerCameraTransform.forward, playerCameraTransform.up);

					targetHorizontalValue = -targetHorizontalValue * Mathf.Deg2Rad;

					targetHorizontalValue = Mathf.Clamp (targetHorizontalValue, -1, 1); 
				} else {
					Vector3 lookDelta = vector3Zero;
					forward = forward.normalized;

					lookDelta = playerCameraTransform.InverseTransformDirection (forward);

					if (lookDelta.magnitude > 1) {
						lookDelta.Normalize ();
					}

					targetHorizontalValue = lookDelta.x;
				}
			}

			//get vertical input direction for the camera when the lock on mode is active
			Vector3 forwardY = navMeshCurrentLookPos;
			Vector3 lookDeltaY = vector3Zero;
			forwardY = forwardY.normalized;

			lookDeltaY = pivotCameraTransform.InverseTransformDirection (forwardY);

			if (lookDeltaY.magnitude > 1) {
				lookDeltaY.Normalize ();
			}

			float targetVerticalValue = lookDeltaY.y;

			horizontalInput = targetHorizontalValue;
			verticalInput = targetVerticalValue;

			if (lookingAtTarget) {
				if (using2_5ViewActive) {
					verticalInput *= lookAtTargetSpeed2_5dView;
					horizontalInput *= lookAtTargetSpeed2_5dView;
				} else {
					if (isCameraTypeFree ()) {
						currentLockedCameraAngle = Vector3.Angle (mainCameraTransform.forward, navMeshCurrentLookPos);

						if (currentLockedCameraAngle > 0.5f) {
							verticalInput *= lookAtTargetSpeed;
							horizontalInput *= lookAtTargetSpeed;
						} else {
							verticalInput *= lookCloserAtTargetSpeed;
							horizontalInput *= lookCloserAtTargetSpeed;
						}
					} else {
						verticalInput *= lookAtTargetSpeedOthersLockedViews;
						horizontalInput *= lookAtTargetSpeedOthersLockedViews;
					}
				}
			}
		}

		if (!cameraCanRotate || !cameraCanBeUsed || !cameraRotationInputEnabled) {
			horizontalInput = 0;
			verticalInput = 0;
		}

		isMoving = Mathf.Abs (horizontalInput) > 0.1f || Mathf.Abs (verticalInput) > 0.1f;

		if (isMoving) {
			setLastTimeMoved ();
		}

		//if the use of the accelerometer is enabled, check the rotation of the device, to add its rotation to the x and y values, to roate the camera
		if (settings.useAcelerometer && !usedByAI && playerInput.isUsingTouchControls () && (playerAiming || playerControllerManager.isGravityPowerActive ())) {
			//x rotates y camera axis
			acelerationAxis.x = Input.acceleration.x;

			horizontalInput += acelerationAxis.x * playerInput.rightTouchSensitivity;

			//y rotates x camera axis
			acelerationAxis.y = lowpass ().z;
			verticalInput += acelerationAxis.y * playerInput.rightTouchSensitivity;

			//accelerometer axis in left landscape
			//z righ phone
			//y up phone
			//x out phone
		}

		clampLookAngle (currentLateUpdateDeltaTime);

		if (!isCameraTypeFree ()) {
			if (driving) {
				followPlayerPositionOnLockedCamera (currentLateUpdateDeltaTime);
			}

			if (followFixedCameraPosition) {
				lockedCameraAxis.position = currentLockedCameraAxisTransform.position;
			}
		}
	}

	public void clampLookAngle (float deltaTimeToUse)
	{
		if (cameraCanRotate && Time.deltaTime != 0 && cameraCanBeUsed) {
			//add the values from the input to the angle applied to the camera

			lookAngle.x = horizontalInput * currentHorizontalRotationSpeed;
			lookAngle.y -= verticalInput * currentVerticalRotationSpeed;

			if (zeroGravityModeOn) {
				playerLookAngle.x = horizontalInput * currentHorizontalRotationSpeed;
				playerLookAngle.y = verticalInput * currentVerticalRotationSpeed;

				if (!onGround) {
					lookAngle.y = 0;
				}

				forwardRotationAngle = Mathf.Lerp (forwardRotationAngle, targetForwardRotationAngle, (currentVerticalRotationSpeed + currentHorizontalRotationSpeed) / 2);
			}
		} else {
			lookAngle.x = 0;
		}

		currentYLimits = currentState.yLimits;

		if (currentState.useYLimitsOnLookAtTargetActive) {
			if (lookingAtTarget) {
				currentYLimits = currentState.YLimitsOnLookAtTargetActive;
			}
		}

		//when the player is in ground after a jump or a fall, if the camera rotation is higher than the limits, it is returned to a valid rotation
		if (onGround) {
			if (adjustPivotAngle) {
				if (lookAngle.y < currentYLimits.x) {
					lookAngle.y += deltaTimeToUse * 250;
				}

				if (lookAngle.y > currentYLimits.y) {
					lookAngle.y -= deltaTimeToUse * 250;
				} else if (lookAngle.y > currentYLimits.x && lookAngle.y < currentYLimits.y) {
					adjustPivotAngle = false;
				}
			} else {
				if (horizontalCameraLimitActiveOnGround) {
					lookAngle.y = Mathf.Clamp (lookAngle.y, currentYLimits.x, currentYLimits.y);
				} else {
					lookAngle.y = Mathf.Clamp (lookAngle.y, -85, 85);
				}

				if (lookAngle.y > 360 || lookAngle.y < -360) {
					lookAngle.y = 0;
				}
			}
		}

		//restart the rotation to avoid acumulate a high value in the x axis
		else {
			if (horizontalCameraLimitActiveOnAir && !zeroGravityModeOn) {
				lookAngle.y = Mathf.Clamp (lookAngle.y, currentYLimits.x, currentYLimits.y);
			} else {
				resetRotationLookAngleYActive = true;

				if (!horizontalCameraLimitActiveOnAir && !zeroGravityModeOn) {
					if (!firstPersonActive) {
						if (powersManager.isAimingPowerInThirdPerson () || weaponsManager.isUsingWeapons ()) {
							currentlyAimingThirdPerson = true;

							if (currentlyAimingThirdPerson != previouslyAimingThirdPerson) {
								previouslyAimingThirdPerson = currentlyAimingThirdPerson;

								adjustPivotAngle = true;
							}

							if (adjustPivotAngle) {
								if (lookAngle.y < currentYLimits.x) {
									lookAngle.y += deltaTimeToUse * 250;
								}

								if (lookAngle.y > currentYLimits.y) {
									lookAngle.y -= deltaTimeToUse * 250;
								} else if (lookAngle.y > currentYLimits.x && lookAngle.y < currentYLimits.y) {
									adjustPivotAngle = false;
								}

								resetRotationLookAngleYActive = false;
							} else {
								lookAngle.y = Mathf.Clamp (lookAngle.y, currentYLimits.x, currentYLimits.y);
							}
						} else {
							currentlyAimingThirdPerson = false;
							previouslyAimingThirdPerson = false;
						}
					}
				} else {
					currentlyAimingThirdPerson = false;
					previouslyAimingThirdPerson = false;
				}

				if (resetRotationLookAngleYActive) {
					if (lookAngle.y > 360 || lookAngle.y < -360) {
						lookAngle.y = 0;
					}
				}
			}
		}
	}

	public void checkCurrentRotationSpeed ()
	{
		if (firstPersonActive) {
			currentVerticalRotationSpeed = firstPersonVerticalRotationSpeed;
			currentHorizontalRotationSpeed = firstPersonHorizontalRotationSpeed;
		} else {
			currentVerticalRotationSpeed = thirdPersonVerticalRotationSpeed;
			currentHorizontalRotationSpeed = thirdPersonHorizontalRotationSpeed;
		}
	}

	public void sethorizontalCameraLimitActiveOnAirState (bool state)
	{
		horizontalCameraLimitActiveOnAir = state;
	}

	public float getCurrentDeltaTime ()
	{
		currentDeltaTime = Time.deltaTime;

		if (regularMovementOnBulletTime) {
			currentScaleTime = GKC_Utils.getCurrentDeltaTime ();

			if (useUnscaledTimeOnBulletTime) {
				if (currentScaleTime < 1) {
					currentDeltaTime = Time.unscaledDeltaTime;
				}
			} else {
				currentDeltaTime *= currentScaleTime;
			}
		}

		return currentDeltaTime;
	}

	public void setShotCameraNoise (Vector2 noiseAmount)
	{
		shotCameraNoise = noiseAmount;
		addNoiseToCamera = true;
	}

	void FixedUpdate ()
	{
		currentFixedUpdateDeltaTime = getCurrentDeltaTime ();

		if (cameraCanBeUsed) {

			if (addNoiseToCamera) {
				horizontalInput = shotCameraNoise.x;
				verticalInput = shotCameraNoise.y;

				addNoiseToCamera = false;
				clampLookAngle (currentFixedUpdateDeltaTime);
			}

			if (cameraNoiseActive) {
				currentNoiseValues.x = Mathf.PerlinNoise (tick, 0) - 0.5f;
				currentNoiseValues.y = Mathf.PerlinNoise (0, tick) - 0.5f;

				tick += getCurrentDeltaTime () * currentCameraNoiseState.roughness * 1;

				currentCameraNoiseState.currentNoise.x = currentCameraNoiseState.noiseAmount.x * currentNoiseValues.x * currentCameraNoiseState.noiseSpeed.x * getCurrentDeltaTime ();
				currentCameraNoiseState.currentNoise.y = currentCameraNoiseState.noiseAmount.y * currentNoiseValues.y * currentCameraNoiseState.noiseSpeed.y * getCurrentDeltaTime ();

				//				currentCameraNoiseState.currentNoise.y = Mathf.Cos (Time.time * currentCameraNoiseState.noiseSpeed.y) * currentCameraNoiseState.noiseAmount.y;

				horizontalInput += currentCameraNoiseState.currentNoise.x;
				verticalInput += currentCameraNoiseState.currentNoise.y;

				clampLookAngle (currentFixedUpdateDeltaTime);
				//				setLastTimeCameraRotated ();
			}

			//apply rotation to camera according to input and the state on the player
			if (useSmoothCameraRotation && ((firstPersonActive && useSmoothCameraRotationFirstPerson) || (!firstPersonActive && useSmoothCameraRotationThirdPerson))) {
				if (firstPersonActive) {
					if (useSmoothCameraRotationFirstPerson) {
						currentSmoothCameraRotationSpeedVertical = smoothCameraRotationSpeedVerticalFirstPerson;
						currentSmoothCameraRotationSpeedHorizontal = smoothCameraRotationSpeedHorizontalFirstPerson;
					}
				} else {
					if (useSmoothCameraRotationThirdPerson) {
						currentSmoothCameraRotationSpeedVertical = smoothCameraRotationSpeedVerticalThirdPerson;
						currentSmoothCameraRotationSpeedHorizontal = smoothCameraRotationSpeedHorizontalThirdPerson;
					}
				}

				if (resetCameraRotationAfterTime && !firstPersonActive && isCameraTypeFree ()) {
					if (!resetingCameraActive && Time.time > timeToResetCameraRotation + lastTimeCameraRotated) {
						resetingCameraActive = true;
					}
				}

				if (resetingCameraActive) {
					lookInPlayerDirection (resetCameraRotationSpeed, true, true);
				} else {
					if (resetVerticalCameraRotationActive) {
						lookAngle.y = Mathf.Lerp (lookAngle.y, 0, currentVerticalRotationSpeed);

						if (Math.Abs (lookAngle.y) < 0.05f) {
							resetVerticalCameraRotationActive = false;
						}
					}

					currentPivotRotation = Quaternion.Euler (lookAngle.y, 0, 0);

					if (useTopDownView && !isCameraTypeFree ()) {
						if (!currentLockedCameraAxisInfo.useCustomPivotHeightOffset && currentTargetToAim == null) {
							currentPivotRotation = quaternionIdentity;

							lookAngle.y = 0;
						}
					}

					pivotCameraTransform.localRotation = 
						Quaternion.Lerp (pivotCameraTransform.localRotation, currentPivotRotation, currentSmoothCameraRotationSpeedVertical * currentFixedUpdateDeltaTime);

					if (!zeroGravityModeOn || onGround) {
						bool useCameraInputToRotate = false;

						if (currentState.lookInPlayerDirection && isCameraTypeFree ()) {
							if (currentState.allowRotationWithInput) {
								if (Math.Abs (axisValues.magnitude) > 0.01f) {
									lastTimeInputUsedWhenLookInPlayerDirectionActive = Time.time;
								}

								if (Time.time < lastTimeInputUsedWhenLookInPlayerDirectionActive + currentState.timeToResetRotationAfterInput) {
									useCameraInputToRotate = true;
								}
							}

							if (!useCameraInputToRotate) {
								lookInPlayerDirection (currentState.lookInPlayerDirectionSpeed, true, false);
							}

						} else {
							useCameraInputToRotate = true;
						}

						if (useCameraInputToRotate) {
							currentCameraUpRotation = Mathf.Lerp (currentCameraUpRotation, lookAngle.x, currentSmoothCameraRotationSpeedHorizontal * currentFixedUpdateDeltaTime);

							if (Math.Abs (currentCameraUpRotation) > 0.001f) {
								playerCameraTransform.Rotate (0, currentCameraUpRotation, 0);
							}
						}
					} else {
						currentCameraRotation = 
							Quaternion.Lerp (currentCameraRotation, Quaternion.Euler (-playerLookAngle.y, lookAngle.x, 0), currentSmoothCameraRotationSpeedHorizontal * currentFixedUpdateDeltaTime);

						if (canRotateForwardOnZeroGravityModeOn) {
							currentForwardRotation = 
								Quaternion.Lerp (currentForwardRotation, Quaternion.Euler (0, 0, forwardRotationAngle), rotateForwardOnZeroGravitySpeed * currentFixedUpdateDeltaTime);

							currentCameraRotation *= currentForwardRotation;
						}

						playerCameraTransform.Rotate (currentCameraRotation.eulerAngles);
					}


//					Vector3 lookForward = lookAtTargetTransform.TransformDirection (Vector3.forward);
//
//					float angle = Vector3.SignedAngle (pivotCameraTransform.forward, lookForward, pivotCameraTransform.right);
//
//					//					print (angle + "  " + lookRight);
//					currentPivotRotation = Quaternion.Euler (angle, 0, 0);
//
//					pivotCameraTransform.localRotation = 
//						Quaternion.Lerp (pivotCameraTransform.localRotation, currentPivotRotation, currentSmoothCameraRotationSpeedVertical * currentFixedUpdateDeltaTime);
//					
//
//					lookForward = lookForward - playerCameraTransform.up * playerCameraTransform.InverseTransformDirection (lookForward).y;
//					lookForward = lookForward.normalized;
//
//					Quaternion targerRotation = Quaternion.LookRotation (lookForward);
//
//					currentCameraRotation = 
//						Quaternion.Lerp (currentCameraRotation, targerRotation, currentSmoothCameraRotationSpeedHorizontal * currentFixedUpdateDeltaTime);
//
//					playerCameraTransform.rotation = currentCameraRotation;
				}

			} else {
				//apply the rotation to the X axis of the pivot
				pivotCameraTransform.localRotation = Quaternion.Euler (lookAngle.y, 0, 0);

				//apply the rotation to the Y axis of the camera
				if (Mathf.Abs (lookAngle.x) > 0.0001f) {
					playerCameraTransform.Rotate (0, lookAngle.x, 0);
				}
			}
	
			if (usedByAI) {
				slerpCameraState (currentState, lerpState, 0);
			} else {
				slerpCameraState (currentState, lerpState, smoothBetweenState);
			}
		}

		if (!isCameraTypeFree ()) {
			if (!driving) {
				followPlayerPositionOnLockedCamera (currentFixedUpdateDeltaTime);
			}
		} else {
			if (useSmoothCameraFollow && !firstPersonActive) {
				Vector3 positionToFollow = targetToFollow.position;

				float speed = smoothCameraFollowSpeed;

				if (playerAiming) {
					speed = smoothCameraFollowSpeedOnAim;
				}

				float distance = GKC_Utils.distance (playerCameraTransform.position, positionToFollow);

				if (distance > smoothCameraFollowMaxDistance) {
					speed *= smoothCameraFollowMaxDistanceSpeed;
				}
			
				playerCameraTransform.position = Vector3.SmoothDamp (playerCameraTransform.position, positionToFollow, ref cameraVelocity, speed);
			}
		}
	}

	public bool isCurrentCameraLookInPlayerDirection ()
	{
		return currentState.lookInPlayerDirection;
	}

	public void lookInPlayerDirection (float rotationSpeedToUse, bool rotatePlayerCameraTransform, bool rotatePivotCameraTransform)
	{
		if (rotatePivotCameraTransform) {
			Quaternion targetRotation = quaternionIdentity;

			if (currentState.pivotRotationOffset != 0) {
				targetRotation = Quaternion.Euler (new Vector3 (currentState.pivotRotationOffset, 0, 0));
			}

			pivotCameraTransform.localRotation = Quaternion.Lerp (pivotCameraTransform.localRotation, targetRotation, rotationSpeedToUse * currentFixedUpdateDeltaTime);
		}

		if (rotatePlayerCameraTransform) {
			playerCameraTransform.rotation = Quaternion.Lerp (playerCameraTransform.rotation, playerControllerGameObject.transform.rotation, rotationSpeedToUse * currentFixedUpdateDeltaTime);
		}

		lookAngle.y = pivotCameraTransform.localEulerAngles.x;

		if (lookAngle.y > 180) {
			lookAngle.y -= 360;
		}

		float pivotCameraTransformAngle = Vector3.SignedAngle (pivotCameraTransform.forward, playerCameraTransform.forward, playerCameraTransform.right);

		float cameraTransformAngle = Vector3.SignedAngle (playerCameraTransform.forward, playerControllerGameObject.transform.forward, playerCameraTransform.up);

		if (Math.Abs (pivotCameraTransformAngle) < 1 && Math.Abs (cameraTransformAngle) < 1) {
			resetingCameraActive = false;

			setLastTimeCameraRotated ();
		}

		if (playerControllerManager.isPlayerUsingInput ()) {
			resetingCameraActive = false;

			setLastTimeCameraRotated ();
		}
	}

	public Vector3 projectOnSegment (Vector3 v1, Vector3 v2, Vector3 playerPosition)
	{
		Vector3 v1ToPos = playerPosition - v1;
		Vector3 segDirection = (v2 - v1).normalized;

		float DistanceFromV1 = Vector3.Dot (segDirection, v1ToPos);

		if (DistanceFromV1 < 0.0f) {
			return v1;
		} else if (DistanceFromV1 * DistanceFromV1 > (v2 - v1).sqrMagnitude) {
			return v2;
		} else {
			Vector3 fromV1 = segDirection * DistanceFromV1;

			return v1 + fromV1;
		}
	}

	public void followPlayerPositionOnLockedCamera (float deltaTimeToUse)
	{
		//follow player position
		if (currentLockedCameraAxisInfo.followPlayerPosition) {
			Vector3 targetPosition = vector3Zero;

			if (currentLockedCameraAxisInfo.useWaypoints) {
				Vector3 currentPosition = targetToFollow.position;

				if (currentLockedCameraAxisInfo.useSpline) {
					BezierSpline lockedCameraSpline = currentLockedCameraAxisInfo.mainSpline;
//
//					Transform splineTransform = lockedCameraSpline.transform;
//						
//					float distanceFromSplineTransform = GKC_Utils.distance (currentPosition, splineTransform.position);
//
//					float totalSplineDistance = GKC_Utils.distance (splineTransform.position, lockedCameraSpline.GetPoint (1));
//
//					float totalSplineProgress = (100 * distanceFromSplineTransform) / totalSplineDistance;

//					print (distanceFromSplineTransform + "  " + totalSplineDistance + " " + totalSplineProgress);

//					targetPosition = lockedCameraSpline.GetPoint (totalSplineProgress / 100);

					targetPosition = lockedCameraSpline.FindNearestPointTo (currentPosition, currentLockedCameraAxisInfo.splineAccuracy, 30);

				} else {
					int numberOfWaypoints = currentLockedCameraAxisInfo.waypointList.Count;

					closestWaypointIndex = -1;
					float currentDistance = 0.0f;

					for (int i = 0; i < currentLockedCameraAxisInfo.waypointList.Count; i++) {
						float sqrDistance = (currentLockedCameraAxisInfo.waypointList [i].position - currentPosition).sqrMagnitude;

						if (currentDistance == 0.0f || sqrDistance < currentDistance) {
							currentDistance = sqrDistance;
							closestWaypointIndex = i;
						}
					}

					if (closestWaypointIndex > -1) {
						closestWaypoint = currentLockedCameraAxisInfo.waypointList [closestWaypointIndex];

						if (closestWaypointIndex == 0) {
							Vector3 position1 = currentLockedCameraAxisInfo.waypointList [0].position;
							Vector3 position2 = currentLockedCameraAxisInfo.waypointList [1].position;

							targetPosition = projectOnSegment (position1, position2, currentPosition);
						} else if (closestWaypointIndex == numberOfWaypoints - 1) {
							Vector3 position1 = currentLockedCameraAxisInfo.waypointList [numberOfWaypoints - 1].position;
							Vector3 position2 = currentLockedCameraAxisInfo.waypointList [numberOfWaypoints - 2].position;

							targetPosition = projectOnSegment (position1, position2, currentPosition);
						} else {
							Vector3 previousWaypointPosition = currentLockedCameraAxisInfo.waypointList [closestWaypointIndex - 1].position;
							Vector3 nextWaypointPosition = currentLockedCameraAxisInfo.waypointList [closestWaypointIndex + 1].position;

							Vector3 currentWaypointPosition = closestWaypoint.position;

							Vector3 LeftSeg = projectOnSegment (previousWaypointPosition, currentWaypointPosition, currentPosition);

							Vector3 RightSeg = projectOnSegment (nextWaypointPosition, currentWaypointPosition, currentPosition);

							if (settings.showCameraGizmo) {
								Debug.DrawLine (currentPosition, LeftSeg, Color.red);
								Debug.DrawLine (currentPosition, RightSeg, Color.blue);
							}

							if ((currentPosition - LeftSeg).sqrMagnitude <= (currentPosition - RightSeg).sqrMagnitude) {
								targetPosition = LeftSeg;
							} else {
								targetPosition = RightSeg;
							}
						}
					} else {
						targetPosition = currentPosition;

						print ("WARNING: Closest position of the player to the waypoint list not found, make sure it is properly configured");
					}
				}
			} else {
				//check if the current camera has a limit in the width, height or depth axis
				if (useCameraLimit) {
					lookCameraParent.position = targetToFollow.position;
				} else {
					lookCameraParent.localPosition = vector3Zero;
				}

				targetPosition = getPositionToLimit (true);
			}

			//set locked pivot position smoothly
			if (currentLockedCameraAxisInfo.followPlayerPositionSmoothly) {
				if (currentLockedCameraAxisInfo.useLerpToFollowPlayerPosition) {
					lockedCameraPivot.position = Vector3.MoveTowards (lockedCameraPivot.position, targetPosition, deltaTimeToUse * currentLockedCameraAxisInfo.followPlayerPositionSpeed);
				} else {
					if (currentLockedCameraAxisInfo.useSeparatedVerticalHorizontalSpeed) {
						newVerticalPosition = Mathf.SmoothDamp (newVerticalPosition, targetPosition.y, ref newVerticalPositionVelocity, currentLockedCameraAxisInfo.verticalFollowPlayerPositionSpeed);

						if (moveInXAxisOn2_5d) {
							newHorizontalPosition = 
								Mathf.SmoothDamp (newHorizontalPosition, targetPosition.x, ref newHorizontalPositionVelocity, currentLockedCameraAxisInfo.horizontalFollowPlayerPositionSpeed);

							lockedCameraPivot.position = new Vector3 (newHorizontalPosition, newVerticalPosition, lockedCameraPivot.position.z);
						} else {
							newHorizontalPosition = 
								Mathf.SmoothDamp (newHorizontalPosition, targetPosition.z, ref newHorizontalPositionVelocity, currentLockedCameraAxisInfo.horizontalFollowPlayerPositionSpeed);

							lockedCameraPivot.position = new Vector3 (lockedCameraPivot.position.x, newVerticalPosition, newHorizontalPosition);
						}
					} else {
						lockedCameraPivot.position = Vector3.SmoothDamp (lockedCameraPivot.position, 
							targetPosition, ref lockedCameraFollowPlayerPositionVelocity, currentLockedCameraAxisInfo.followPlayerPositionSpeed);
					}
				}

				lockedMainCameraTransform.position = lockedCameraPivot.position;
			} else {
				lockedCameraPivot.position = targetPosition;
				lockedMainCameraTransform.position = targetPosition;
			}
		}
	}

	public float getHorizontalInput ()
	{
		return horizontalInput;
	}

	public float getVerticalInput ()
	{
		return verticalInput;
	}

	public void setCanvasInfo (Vector2 newMainCanvasSizeDelta, bool newUsingScreenSpaceCamera, bool creatingCharactersOnEditor)
	{
		mainCanvasSizeDelta = newMainCanvasSizeDelta;
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = newUsingScreenSpaceCamera;

		if (creatingCharactersOnEditor) {
			updateComponent ();
		}
	}

	public void setFollowingMultipleTargetsState (bool state, bool activatingFunctionFromEditor)
	{
		followingMultipleTargets = state;

		if (activatingFunctionFromEditor) {
			updateComponent ();
		}
	}

	public void setMultipleTargetsToFollowList (List<Transform> newMultipleTargetsToFollowList, bool activatingFunctionFromEditor)
	{
		if (newMultipleTargetsToFollowList != null && newMultipleTargetsToFollowList.Count > 0) {
			multipleTargetsToFollowList = newMultipleTargetsToFollowList;
		} else {
			multipleTargetsToFollowList.Clear ();
		}

		if (activatingFunctionFromEditor) {
			updateComponent ();
		}
	}

	public void setextraPositionOffsetValue (Vector3 newOffset)
	{
		extraPositionOffset = newOffset;
	}

	public Vector3 getPositionToLimit (bool calculateOnRunTime)
	{
		Vector3 newPosition = playerCameraTransform.position;

		if (followingMultipleTargets) {
			if (multipleTargetsToFollowList.Count == 1) {
				newPosition = multipleTargetsToFollowList [0].position;
			} else if (multipleTargetsToFollowList.Count >= 2) {
				var bounds = new Bounds (multipleTargetsToFollowList [0].position, vector3Zero);

				for (int i = 0; i < multipleTargetsToFollowList.Count; i++) {
					if (multipleTargetsToFollowList [i] != null) {
						bounds.Encapsulate (multipleTargetsToFollowList [i].position);
					}
				}

				newPosition = bounds.center;

				if (useTopDownView) {
					currentMultipleTargetsFov = bounds.size.x;
				}

				if (using2_5ViewActive) {
					if (moveInXAxisOn2_5d) {
						currentMultipleTargetsFov = bounds.size.x;
					} else {
						currentMultipleTargetsFov = bounds.size.z;
					}
				}

				if (!useMultipleTargetFov) {
					Vector2 boundsPosition = new Vector2 (bounds.size.x, bounds.size.z);

					newPosition.y = bounds.center.y + (boundsPosition.magnitude / 2) * multipleTargetsHeightMultiplier;

					newPosition.y = Mathf.Clamp (newPosition.y, minMultipleTargetHeight, maxMultipleTargetHeight);
				}
			}

			if (useMultipleTargetFov) {
				mainCamera.fieldOfView = Mathf.Lerp (multipleTargetsMinFov, multipleTargetsMaxFov, currentMultipleTargetsFov / multipleTargetsFovSpeed);
			}

			if (using2_5ViewActive) {
				if (moveInXAxisOn2_5d) {
					multipleTargetsCurrentYPosition = newPosition.z;
					newPosition.z = Mathf.Lerp (multipleTargetsCurrentYPosition, multipleTargetsCurrentYPosition + multipleTargetsMaxHeight, currentMultipleTargetsFov / multipleTargetsYPositionSpeed);
				} else {
					multipleTargetsCurrentYPosition = newPosition.x;
					newPosition.x = Mathf.Lerp (multipleTargetsCurrentYPosition, multipleTargetsCurrentYPosition + multipleTargetsMaxHeight, currentMultipleTargetsFov / multipleTargetsYPositionSpeed);
				}
			}

			if (useTopDownView) {
				multipleTargetsCurrentYPosition = newPosition.y;
				newPosition.y = Mathf.Lerp (multipleTargetsCurrentYPosition, multipleTargetsCurrentYPosition + multipleTargetsMaxHeight, currentMultipleTargetsFov / multipleTargetsYPositionSpeed);
			}
		} else {
			newPosition += extraPositionOffset;
		}

		//if the calculation of the position is made on update, check if the camera has an offset, that can be applied only when the player moves or also, when he moves
		if (calculateOnRunTime) {

			if (currentLockedCameraAxisInfo.useBoundToFollowPlayer) {
				newPosition = calculateBoundPosition (true);
			}

			//if the player is on 2.5d view
			if (using2_5ViewActive) {
				if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSide || currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSideOnMoving) {
					//check if the player is using the input
					bool playerIsUsingInput = playerControllerManager.isPlayerMovingHorizontal (currentLockedCameraAxisInfo.inputToleranceOnFaceSide);
					bool playerIsMoving = playerControllerManager.isPlayerMovingHorizontal (currentLockedCameraAxisInfo.inputToleranceOnFaceSideOnMoving);

					bool lookToRight = false;

					//Check the rotation of the player in his local Y axis to check the closest direction to look
					float currentPlayerRotationY = 0;

					Vector3 newOffset = vector3Zero;
					Vector3 newOffsetOnMoving = vector3Zero;

					//check the axis where he is moving, on XY or YZ
					if (moveInXAxisOn2_5d) {
						currentPlayerRotationY = Vector3.SignedAngle (targetToFollow.forward, lockedCameraPivot.right, playerCameraTransform.up);
					} else {
						currentPlayerRotationY = Vector3.SignedAngle (targetToFollow.forward, lockedCameraPivot.forward, playerCameraTransform.up);
					}

					//check if the player is moving to the left or to the right
					if (moveInXAxisOn2_5d) {
						if (Math.Abs (currentPlayerRotationY) < 90) {
							lookToRight = true;
						}
					} else {
						if (Math.Abs (currentPlayerRotationY) <= 90) {
							lookToRight = true;
						}
					}

					//add the offset to left and right according to the direction where the player is moving
					if (moveInXAxisOn2_5d) {
						if (playerIsMoving) {
							if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSideOnMoving) {
								if (lookToRight) {
									newOffsetOnMoving = Vector3.right * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMoving;
								} else {
									newOffsetOnMoving = -Vector3.right * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMoving;
								}
							}
						} 

						if (!playerIsUsingInput) {
							if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSide) {
								if (lookToRight) {
									newOffset = Vector3.right * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSide;
								} else {
									newOffset = -Vector3.right * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSide;
								}
							}
						}
					} else {
						if (playerIsMoving) {
							if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSideOnMoving) {
								if (lookToRight) {
									newOffsetOnMoving = Vector3.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMoving;
								} else {
									newOffsetOnMoving = -Vector3.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMoving;
								}
							}
						} 

						if (!playerIsUsingInput) {
							if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSide) {
								if (lookToRight) {
									newOffset = Vector3.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSide;
								} else {
									newOffset = -Vector3.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSide;
								}
							}
						}
					}

					//add this offset to the current camera position
					horizontalOffsetOnSide = Vector3.SmoothDamp (horizontalOffsetOnSide, newOffset, ref horizontalOffsetOnFaceSideSpeed, currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideSpeed);

					horizontalOffsetOnSideOnMoving = Vector3.SmoothDamp (horizontalOffsetOnSideOnMoving, newOffsetOnMoving,
						ref horizontalOffsetOnFaceSideOnMovingSpeed, currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMovingSpeed);

					newPosition += horizontalOffsetOnSide + horizontalOffsetOnSideOnMoving;
				}

				//check to add vertical offset to the camera according to vertical input on 2.5d
				if (currentLockedCameraAxisInfo.use2_5dVerticalOffsetOnMove) {
					float newVerticalInput = playerControllerManager.getRawAxisValues ().y;

					Vector3 newOffsetOnMoving = vector3Zero;

					if (newVerticalInput > 0) {
						newOffsetOnMoving = Vector3.up * currentLockedCameraAxisInfo.verticalTopOffsetOnMove;
					} else if (newVerticalInput < 0) {
						newOffsetOnMoving = -Vector3.up * currentLockedCameraAxisInfo.verticalBottomOffsetOnMove;
					}

					verticalOffsetOnMove = Vector3.SmoothDamp (verticalOffsetOnMove, newOffsetOnMoving, ref verticalOffsetOnMoveSpeed, currentLockedCameraAxisInfo.verticalOffsetOnMoveSpeed);

					newPosition += verticalOffsetOnMove;
				}
			}

			//the player is on a top down view or similar, like isometric
			if (useTopDownView) {
				if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSide || currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSideOnMoving) {
					//check if the player is using the input
					bool playerIsUsingInput = playerControllerManager.isPlayerMoving (currentLockedCameraAxisInfo.inputToleranceOnFaceSide);
					bool playerIsMoving = playerControllerManager.isPlayerMoving (currentLockedCameraAxisInfo.inputToleranceOnFaceSideOnMoving);

					Vector3 newOffset = vector3Zero;
					Vector3 newOffsetOnMoving = vector3Zero;

					//add the offset to the camera, setting the direction of this offset as the forward direction of the player
					if (playerIsMoving) {
						if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSideOnMoving) {
							newOffsetOnMoving = targetToFollow.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMoving;
						}
					} 

					if (!playerIsUsingInput) {
						if (currentLockedCameraAxisInfo.useHorizontalOffsetOnFaceSide) {
							newOffset = targetToFollow.forward * currentLockedCameraAxisInfo.horizontalOffsetOnFaceSide;
						}
					}

					//add the offset to the camera position
					horizontalOffsetOnSide = Vector3.SmoothDamp (horizontalOffsetOnSide, newOffset, ref horizontalOffsetOnFaceSideSpeed, currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideSpeed);

					horizontalOffsetOnSideOnMoving = Vector3.SmoothDamp (horizontalOffsetOnSideOnMoving, newOffsetOnMoving, 
						ref horizontalOffsetOnFaceSideOnMovingSpeed, currentLockedCameraAxisInfo.horizontalOffsetOnFaceSideOnMovingSpeed);

					newPosition += horizontalOffsetOnSide + horizontalOffsetOnSideOnMoving;
				}
			}
		} else {
			calculateBoundPosition (false);
		}

		if (useCameraLimit) {
			if (useHeightLimit) {
				newPosition.y = Mathf.Clamp (newPosition.y, currentCameraLimitPosition.y - heightLimitLower - lockedCameraAxis.localPosition.y,
					currentCameraLimitPosition.y + heightLimitUpper - lockedCameraAxis.localPosition.y);
			}

			if (currentLockedCameraAxisInfo.moveInXAxisOn2_5d) {
				if (useWidthLimit) {
					newPosition.x = Mathf.Clamp (newPosition.x, currentCameraLimitPosition.x - widthLimitLeft - lockedCameraAxis.localPosition.x, 
						currentCameraLimitPosition.x + widthLimitRight - lockedCameraAxis.localPosition.x);
				}

				if (useDepthLimit) {
					newPosition.z = Mathf.Clamp (newPosition.z, currentCameraLimitPosition.z - depthLimitBackward, currentCameraLimitPosition.z + depthLimitFront);
				}
			} else {
				if (useWidthLimit) {
					newPosition.z = Mathf.Clamp (newPosition.z, currentCameraLimitPosition.z - widthLimitLeft, currentCameraLimitPosition.z + widthLimitRight);
				}

				if (useDepthLimit) {
					newPosition.x = Mathf.Clamp (newPosition.x, currentCameraLimitPosition.x - depthLimitBackward, currentCameraLimitPosition.x + depthLimitFront);
				}
			}
		}

		return newPosition;
	}

	public Vector3 calculateBoundPosition (bool calculateOnRunTime)
	{
		if (!calculateOnRunTime) {
			focusArea = new FocusArea (mainCollider.bounds, currentLockedCameraAxisInfo.heightBoundTop,
				currentLockedCameraAxisInfo.widthBoundRight, currentLockedCameraAxisInfo.widthBoundLeft,
				currentLockedCameraAxisInfo.depthBoundFront, currentLockedCameraAxisInfo.depthBoundBackward);
		}

		focusArea.Update (mainCollider.bounds);

		focusTargetPosition = focusArea.centre +
		Vector3.right * currentLockedCameraAxisInfo.boundOffset.x +
		Vector3.up * currentLockedCameraAxisInfo.boundOffset.y +
		Vector3.forward * currentLockedCameraAxisInfo.boundOffset.z;

		return focusTargetPosition;
	}

	public void setIgnorePivotCameraCollisionActiveState (bool state)
	{
		ignorePivotCameraCollisionActive = state;
	}

	void updatePivotTransformPosition ()
	{
		if (usePivotCameraCollisionEnabled) {
			pivotCollisionSurfaceFound = false;

			if (!ignorePivotCameraCollisionActive) {
				Vector3 currentPivotPositionCenter = vector3Zero;

				if (pivotCameraCollisionHeightOffset > 0) {
					currentPivotPositionCenter += playerCameraTransform.up * pivotCameraCollisionHeightOffset;
				}

				cameraTransformPosition = playerCameraTransform.position + currentPivotPositionCenter;
				currentPivotPositionOffset = currentState.pivotPositionOffset;

				directionFromMainPositionToPivot = pivotCameraTransform.position - cameraTransformPosition;
				distanceToPivotPositionOffset = GKC_Utils.distance (currentPivotPositionOffset, currentPivotPositionCenter);

				if (cameraCollisionAlwaysActive || currentState.cameraCollisionActive) {
					if (Physics.SphereCast (cameraTransformPosition, maxCheckDist, directionFromMainPositionToPivot,
						    out pivotHit, distanceToPivotPositionOffset + extraCameraCollisionDistance, settings.layer)) {

						if (pivotHit.rigidbody == null) {
							pivotCollisionDistance = pivotHit.distance - 0.05f;

							pivotCameraTransform.localPosition = 
						new Vector3 (currentPivotPositionOffset.x, pivotCollisionDistance, currentPivotPositionOffset.z);

							pivotCollisionSurfaceFound = true;
						}
					}
				}
			}
		} else {
			currentPivotPositionOffset = currentState.pivotPositionOffset;
		}

		if (!pivotCollisionSurfaceFound) {
			pivotCameraTransform.localPosition = 
				Vector3.Lerp (pivotCameraTransform.localPosition, currentPivotPositionOffset, currentUpdateDeltaTime * movementLerpSpeed);
		}
	}

	public void checkCameraPosition ()
	{
		collisionSurfaceFound = false;

		pivotCameraPosition = pivotCameraTransform.position;
		currentCamPositionOffset = currentState.camPositionOffset;

		directionFromPivotToCamera = mainCameraTransform.position - pivotCameraPosition;
		distanceToCamPositionOffset = GKC_Utils.distance (currentCamPositionOffset, vector3Zero);

		if (cameraCollisionAlwaysActive || currentState.cameraCollisionActive) {
			if (Physics.SphereCast (pivotCameraPosition, maxCheckDist, directionFromPivotToCamera, 
				    out hit, distanceToCamPositionOffset + extraCameraCollisionDistance, settings.layer)) {

				directionFromPivotToCamera = directionFromPivotToCamera.normalized;

				collisionDistance = hit.distance;

				if (settings.showCameraGizmo) {
					Debug.DrawLine (pivotCameraPosition, pivotCameraPosition + (directionFromPivotToCamera * collisionDistance), Color.green);
				}

				targetCameraPosition = pivotCameraPosition + (directionFromPivotToCamera * collisionDistance);

				//targetCameraPosition = targetCameraPosition - playerCameraTransform.up * playerCameraTransform.InverseTransformDirection (targetCameraPosition).y;
				//targetCameraPosition += (currentState.pivotPositionOffset.y + currentCamPositionOffset.y) * playerCameraTransform.up;

				//mainCameraTransform.position = targetCameraPosition;

				cameraNewPosition = pivotCameraTransform.InverseTransformPoint (targetCameraPosition);

				mainCameraTransform.localPosition = 
					new Vector3 (cameraNewPosition.x, currentCamPositionOffset.y, cameraNewPosition.z);

				//print (mainCameraTransform.localPosition);

				collisionSurfaceFound = true;
			}
		}

		if (!collisionSurfaceFound) {
			if (settings.showCameraGizmo) {
				Debug.DrawLine (pivotCameraPosition, pivotCameraPosition + (directionFromPivotToCamera.normalized * distanceToCamPositionOffset), Color.red);
			}

			mainCameraPosition = mainCameraTransform.localPosition;

			mainCameraNewPosition = Vector3.Lerp (mainCameraPosition, currentCamPositionOffset, getCurrentDeltaTime () * movementLerpSpeed);

			mainCameraTransform.localPosition = mainCameraNewPosition;
		}
	}

	public void setCameraToFreeOrLocked (typeOfCamera state, lockedCameraSystem.cameraAxis lockedCameraInfo)
	{
		if (state == typeOfCamera.free) {
			if (cameraType != state) {
				if (rotatingLockedCameraFixedRotationAmountActive) {
					stopRotateLockedCameraFixedRotationAmount ();
				}

				if (driving) {
					cameraType = state;

					cameraCurrentlyLocked = false;

					previousLockedCameraAxisTransform = null;
					playerControllerManager.setLockedCameraState (false, false, false, true);

					if (usingPlayerNavMeshPreviously) {
						playerNavMeshManager.setPlayerNavMeshEnabledState (false);
					}

					usingPlayerNavMeshPreviously = false;

					//check if the player is driving and set the locked camera properly
					vehicleHUDManager currentVehicleHUDManager = playerControllerManager.getCurrentVehicle ().GetComponent<vehicleHUDManager> ();

					if (currentVehicleHUDManager != null) {
						currentVehicleHUDManager.setPlayerCameraParentAndPosition (mainCamera.transform, this);
					}

					if (currentVehicleHUDManager.getCurrentDriver () == playerControllerGameObject) {
						setCameraState (previousFreeCameraStateName);
					} else {
						setCameraState (defaultVehiclePassengerStateName);
					}
				} else {
					currentLockedCameraAxisInfo = lockedCameraInfo;

					string auxPreviousFreeCameraStateName = previousFreeCameraStateName;

					if (showDebugPrint) {
						print ("previous free camera state " + previousFreeCameraStateName);
					}

					if (!previousFreeCameraStateName.Equals ("")) {

						setCameraState (previousFreeCameraStateName);
						resetCurrentCameraStateAtOnce ();

						configureCameraAndPivotPositionAtOnce ();

						previousFreeCameraStateName = "";
					}

					playerCameraTransform.eulerAngles = new Vector3 (playerCameraTransform.eulerAngles.x, lockedCameraInfo.axis.eulerAngles.y, playerCameraTransform.eulerAngles.z);
					pivotCameraTransform.eulerAngles = new Vector3 (lockedCameraInfo.axis.localEulerAngles.x, pivotCameraTransform.eulerAngles.y, pivotCameraTransform.eulerAngles.z);
					mainCamera.transform.SetParent (mainCameraTransform);

					if (currentLockedCameraAxisInfo.smoothCameraTransition) {
						lockedCameraMovement (false);

						if (previousLockedCameraAxisInfo.useDifferentCameraFov || usingLockedZoomOn) {
							setMainCameraFov (currentState.initialFovValue, zoomSpeed);
						}
					} else {
						stopLockedCameraMovementCoroutine ();

						cameraType = state;

						cameraCurrentlyLocked = false;

						mainCamera.transform.localPosition = vector3Zero;
						mainCamera.transform.localRotation = quaternionIdentity;

						if (previousLockedCameraAxisInfo.useDifferentCameraFov || usingLockedZoomOn) {
							mainCamera.fieldOfView = currentState.initialFovValue;
						}
					}

					lookAngle = Vector2.zero;
					previousLockedCameraAxisTransform = null;
					playerControllerManager.setLockedCameraState (false, false, false, true);

					if (usingPlayerNavMeshPreviously) {
						playerNavMeshManager.setPlayerNavMeshEnabledState (false);
					}

					usingPlayerNavMeshPreviously = false;

					if (weaponsManager.isAimingWeapons ()) {
						weaponsManager.inputAimWeapon ();

						if (!auxPreviousFreeCameraStateName.Equals ("")) {
							setCameraState (auxPreviousFreeCameraStateName);
						}
					}

					if (previouslyInFirstPerson) {
						changeCameraToThirdOrFirstView ();
					}
				}

				setLookAtTargetState (false, null);

				//check the unity events on enter and exit
				callLockedCameraEventOnEnter (lockedCameraInfo);

				if (previousLockedCameraAxisInfo != null) {
					callLockedCameraEventOnExit (previousLockedCameraAxisInfo);
				}

				if (previousLockedCameraAxisInfo.changeRootMotionActive) {
					playerControllerManager.setOriginalUseRootMotionActiveState ();
				}

				previousLockedCameraAxisInfo = null;

				useCameraLimit = false;

				if (usingSetTransparentSurfacesPreviously) {
					setTransparentSurfacesManager.setCheckSurfaceActiveState (false);
				}

				setTransparentSurfacesManager.setLockedCameraActiveState (false);

				enableOrDisableMainCameraReticle (true);

				lockedCameraZoomMovingCameraValue = 0;

				playerControllerManager.setDeactivateRootMotionOnStrafeActiveOnLockedViewState (false);

				playerControllerManager.setOriginalCanMoveWhileAimLockedCameraValue ();
			}
		} 

		if (state == typeOfCamera.locked) {
			//assign the new locked camera info

			currentLockedCameraAxisInfo = lockedCameraInfo;

			if (temporalCameraViewToLockedCameraActive) {
				return;
			}

			currentLockedCameraAxisTransform = currentLockedCameraAxisInfo.axis;

			followFixedCameraPosition = currentLockedCameraAxisInfo.followFixedCameraPosition;

			bool newCameraFound = false;

			if (previousLockedCameraAxisInfo == null || previousLockedCameraAxisInfo != currentLockedCameraAxisInfo) {
				newCameraFound = true;
			}

//			print ("New locked camera found: " + newCameraFound + " " + currentLockedCameraAxisInfo.name);

			//if a new camera is found, adjust the position of the locked camera on the player
			if (!newCameraFound) {
				return;
			}

			if (rotatingLockedCameraFixedRotationAmountActive) {
				stopRotateLockedCameraFixedRotationAmount ();
			}

			mainCamera.transform.SetParent (null);

			//set the position and rotations of the new locked camera transform to the previous locked transform elements of the player
			lockedCameraPosition.localPosition = currentLockedCameraAxisInfo.cameraPosition.localPosition;
			lockedCameraPosition.localRotation = currentLockedCameraAxisInfo.cameraPosition.localRotation;

			lockedCameraAxis.localPosition = currentLockedCameraAxisInfo.axis.localPosition;
			lockedCameraAxis.localRotation = currentLockedCameraAxisInfo.axis.localRotation;

			Vector3 targetToFollowPosition = targetToFollow.position;

			//if the is a locked camera pivot, it means the camera follows the player position on this locked view
			if (currentLockedCameraAxisInfo.lockedCameraPivot) {
				if (activatingLockedCameraByInputActive && currentLockedCameraAxisInfo.followPlayerPosition) {
					lockedCameraPivot.position = targetToFollowPosition;
				} else {
					lockedCameraPivot.rotation = currentLockedCameraAxisInfo.lockedCameraPivot.rotation;

					//place the new camera found in the current position of the player to have a smoother transition between cameras that are following the player positition
					if (currentLockedCameraAxisInfo.useZeroCameraTransition) {
						lockedCameraPivot.position = targetToFollowPosition;

						lockedCameraPivot.position = getPositionToLimit (false);
					} else {
						lockedCameraPivot.position = currentLockedCameraAxisInfo.lockedCameraPivot.position;
					}
				}
			} else {
				if (activatingLockedCameraByInputActive && currentLockedCameraAxisInfo.followPlayerPosition) {
					lockedCameraPivot.position = targetToFollowPosition;
				} else {
					//else, the camera will stay in a fixed position

					lockedCameraAxis.position = currentLockedCameraAxisInfo.axis.position;
					lockedCameraAxis.rotation = currentLockedCameraAxisInfo.axis.rotation;

					lockedCameraPosition.position = currentLockedCameraAxisInfo.cameraPosition.position;
					lockedCameraPosition.rotation = currentLockedCameraAxisInfo.cameraPosition.rotation;
				}
			}

			lookCameraParent.localPosition = vector3Zero;
			lookCameraParent.localRotation = quaternionIdentity;

			if (!cameraCurrentlyLocked) {
				previousLockedCameraAxisTransform = getCameraTransform ();

				previouslyInFirstPerson = isFirstPersonActive ();

				if (previouslyInFirstPerson) {
					changeCameraToThirdOrFirstView ();
				}

				headBobManager.stopAllHeadbobMovements ();

				stopShakeCamera ();

				cameraCurrentlyLocked = true;
			}

			if (previousLockedCameraAxisTransform != null) {
				setCurrentAxisTransformValues (previousLockedCameraAxisTransform);
			}

			lockedCameraCanFollow = false;

			//2.5d camera setting
			if (currentLockedCameraAxisInfo.use2_5dView) {

				lookCameraPivot.localPosition = currentLockedCameraAxisInfo.pivot2_5d.localPosition;
				lookCameraPivot.localRotation = currentLockedCameraAxisInfo.pivot2_5d.localRotation;

				lookCameraDirection.localPosition = currentLockedCameraAxisInfo.lookDirection2_5d.localPosition;
				lookCameraDirection.localRotation = currentLockedCameraAxisInfo.lookDirection2_5d.localRotation;

				playerControllerManager.set3dOr2_5dWorldType (false);
				using2_5ViewActive = true;

				originalLockedCameraPivotPosition = currentLockedCameraAxisInfo.originalLockedCameraPivotPosition;
				moveInXAxisOn2_5d = currentLockedCameraAxisInfo.moveInXAxisOn2_5d;

				if (currentLockedCameraAxisInfo.useDefaultZValue2_5d) {
					movePlayerToDefaultHorizontalValue2_5d ();
				}

				clampAimDirections = currentLockedCameraAxisInfo.clampAimDirections;
				numberOfAimDirections = (int)currentLockedCameraAxisInfo.numberOfAimDirections;
			} else {
				playerControllerManager.set3dOr2_5dWorldType (true);
				using2_5ViewActive = false;
			}

			horizontalCameraLimitActiveOnGround = !using2_5ViewActive;

			//point and click setting

			if (currentLockedCameraAxisInfo.usePointAndClickSystem) {
				if (!usingPlayerNavMeshPreviously) {
					playerNavMeshManager.setPlayerNavMeshEnabledState (true);
				}

				usingPlayerNavMeshPreviously = true;
			} else {
				if (usingPlayerNavMeshPreviously) {
					playerNavMeshManager.setPlayerNavMeshEnabledState (false);
				}

				usingPlayerNavMeshPreviously = false;
			}

			//top down setting
			if (currentLockedCameraAxisInfo.useTopDownView) {

				lookCameraPivot.localPosition = currentLockedCameraAxisInfo.topDownPivot.localPosition;
				lookCameraPivot.localRotation = currentLockedCameraAxisInfo.topDownPivot.localRotation;

				lookCameraDirection.localPosition = currentLockedCameraAxisInfo.topDownLookDirection.localPosition;
				lookCameraDirection.localRotation = currentLockedCameraAxisInfo.topDownLookDirection.localRotation;

				useTopDownView = true;
			} else {
				useTopDownView = false;
			}

			if (useTopDownView) {
				if (currentLockedCameraAxisInfo.showCameraCursorWhenNotAiming) {
					setLookDirection = true;
				}
			} else {
				setLookAtTargetState (false, null);
			}

			useMouseInputToRotateCameraHorizontally = false;

			previouslyOnFreeCamera = cameraType == typeOfCamera.free;

//			print ("previously on free camera: " + previouslyOnFreeCamera);

			cameraType = state;

			mainCamera.transform.SetParent (lockedCameraPosition);

			if (currentLockedCameraAxisInfo.smoothCameraTransition) {
				lockedCameraMovement (true);

				if (currentLockedCameraAxisInfo.useDifferentCameraFov) {
					setMainCameraFov (currentLockedCameraAxisInfo.fovValue, zoomSpeed);
				}

				if (previousLockedCameraAxisInfo != null) {
					if (usingLockedZoomOn) {
						setMainCameraFov (currentState.initialFovValue, zoomSpeed);
					}
				}
			} else {
				stopLockedCameraMovementCoroutine ();

				mainCamera.transform.localPosition = vector3Zero;
				mainCamera.transform.localRotation = quaternionIdentity;

				lockedCameraChanged = true;

				if (currentLockedCameraAxisInfo.useDifferentCameraFov) {
					mainCamera.fieldOfView = currentLockedCameraAxisInfo.fovValue;
				}

				if (previousLockedCameraAxisInfo != null) {
					if (usingLockedZoomOn) {
						mainCamera.fieldOfView = currentState.initialFovValue;
					}
				}
			}

			playerControllerManager.setLockedCameraState (true, currentLockedCameraAxisInfo.useTankControls, 
				currentLockedCameraAxisInfo.useRelativeMovementToLockedCamera, currentLockedCameraAxisInfo.playerCanMoveOnAimInTankMode);

			if (previousLockedCameraAxisInfo != null) {
				if (usingLockedZoomOn || previousLockedCameraAxisInfo.cameraCanRotate || previousLockedCameraAxisInfo.canMoveCamera) {

					//Reset locked camera values on this player camera
					currentLockedLoonAngle = Vector2.zero;
					currentLockedCameraRotation = quaternionIdentity;
					currentLockedPivotRotation = quaternionIdentity;
					lastTimeLockedSpringRotation = 0;
					lastTimeLockedSpringMovement = 0;
					currentLockedCameraMovementPosition = vector3Zero;
					currentLockedMoveCameraPosition = vector3Zero;

					usingLockedZoomOn = false;
				}

				if (previousLockedCameraAxisInfo.changeRootMotionActive) {
					playerControllerManager.setOriginalUseRootMotionActiveState ();
				}
			}

			//Assign the previous locked camera and call the event on exit
			if (previousLockedCameraAxisInfo != currentLockedCameraAxisInfo) {
				previousLockedCameraAxisInfo = currentLockedCameraAxisInfo;

				//check the unity events on exit
				callLockedCameraEventOnExit (previousLockedCameraAxisInfo);
			}

			//check the unity events on enter
			callLockedCameraEventOnEnter (currentLockedCameraAxisInfo);

			//set the current vertical and horizontal position of the camera in case the following speed is separated into these two values, so the position is calculated starting at that point
			if (currentLockedCameraAxisInfo.lockedCameraPivot) {
				newVerticalPosition = lockedCameraPivot.position.y;

				if (moveInXAxisOn2_5d) {
					newHorizontalPosition = lockedCameraPivot.position.x;
				} else {
					newHorizontalPosition = lockedCameraPivot.position.z;
				}
			}

			if (currentLockedCameraAxisInfo.lookAtPlayerPosition) {
				calculateLockedCameraLookAtPlayerPosition ();

				lockedCameraPosition.localRotation = Quaternion.Euler (new Vector3 (currentLockedLimitLookAngle.x, 0, 0));

				lockedCameraAxis.localRotation = Quaternion.Euler (new Vector3 (0, currentLockedLimitLookAngle.y, 0));
			}

			horizontalOffsetOnSide = vector3Zero;

			horizontalOffsetOnSideOnMoving = vector3Zero;

			verticalOffsetOnMove = vector3Zero;

			if (currentLockedCameraAxisInfo.useTransparetSurfaceSystem) {
				setTransparentSurfacesManager.setCheckSurfaceActiveState (true);
			} else {
				setTransparentSurfacesManager.setCheckSurfaceActiveState (false);
			}

			setTransparentSurfacesManager.setLockedCameraActiveState (true);

			usingSetTransparentSurfacesPreviously = currentLockedCameraAxisInfo.useTransparetSurfaceSystem;

			if (previousFreeCameraStateName.Equals ("")) {

				if (previouslyInFirstPerson) {
					previousFreeCameraStateName = defaultFirstPersonStateName;
				} else {
					previousFreeCameraStateName = defaultThirdPersonStateName;
				}

				setCameraState (defaultLockedCameraStateName);
				resetCurrentCameraStateAtOnce ();
			}

			if (currentLockedCameraAxisInfo.changeRootMotionActive) {
				playerControllerManager.setUseRootMotionActiveState (currentLockedCameraAxisInfo.useRootMotionActive);
			}

			if (previouslyOnFreeCamera) {
				if (!previouslyInFirstPerson && weaponsManager.isAimingWeapons ()) {
					weaponsManager.inputAimWeapon ();
				}
			}

			enableOrDisableMainCameraReticle (false);

			lockedCameraZoomMovingCameraValue = 0;

			if (currentLockedCameraAxisInfo.disablePreviousCameraLimitSystem) {
				useCameraLimit = false;
			}

			if (currentLockedCameraAxisInfo.setDeactivateRootMotionOnStrafeActiveOnLockedViewValue) {
				playerControllerManager.setDeactivateRootMotionOnStrafeActiveOnLockedViewState (currentLockedCameraAxisInfo.deactivateRootMotionOnStrafeActiveOnLockedView);
			}

			if (currentLockedCameraAxisInfo.setCanMoveWhileAimLockedCameraState) {
				playerControllerManager.setCanMoveWhileAimLockedCameraValue (currentLockedCameraAxisInfo.canMoveWhileAimLockedCamera);
			}

			if (currentLockedCameraAxisInfo.putCameraOutsideOfPivot) {
				if (putCameraOutsideOfPivotTransform == null) {
					GameObject newputCameraOutsideOfPivot = new GameObject ();

					newputCameraOutsideOfPivot.name = "Put Camera Outside Of Pivot Transform";

					putCameraOutsideOfPivotTransform = newputCameraOutsideOfPivot.transform;
				}

				putCameraOutsideOfPivotTransform.position = mainCamera.transform.position;
				putCameraOutsideOfPivotTransform.rotation = mainCamera.transform.rotation;

				mainCamera.transform.SetParent (putCameraOutsideOfPivotTransform);
			}
		}
	}

	public void setClampAimDirectionsState (bool state)
	{
		clampAimDirections = state;
	}

	public void calculateLockedCameraLookAtPlayerPosition ()
	{
		Vector3 cameraAxisPosition = lockedCameraPosition.position;

		if (currentLockedCameraAxisInfo.usePositionOffset) {
			cameraAxisPosition += lockedCameraAxis.transform.right * currentLockedCameraAxisInfo.positionOffset.x;
			cameraAxisPosition += lockedCameraAxis.transform.up * currentLockedCameraAxisInfo.positionOffset.y;
			cameraAxisPosition += lockedCameraAxis.transform.forward * currentLockedCameraAxisInfo.positionOffset.z;
		}

		Vector3 currentTargetToLookPosition = targetToFollow.position;

		if (currentLockedCameraAxisInfo.lookAtCustomTarget) {
			if (currentLockedCameraAxisInfo.lookAtCustomMultipleTargets) {
				List<Transform> lookAtCustomMultipleTargetsList = currentLockedCameraAxisInfo.lookAtCustomMultipleTargetsList;

				int customMultipleTargetsListCount = lookAtCustomMultipleTargetsList.Count;

				var bounds = new Bounds (lookAtCustomMultipleTargetsList [0].position, vector3Zero);

				for (int i = 0; i < customMultipleTargetsListCount; i++) {
					if (lookAtCustomMultipleTargetsList [i] != null) {
						bounds.Encapsulate (lookAtCustomMultipleTargetsList [i].position);
					}
				}

				currentTargetToLookPosition = bounds.center;
			} else {
				currentTargetToLookPosition = currentLockedCameraAxisInfo.customTargetToLook.position;
			}
		}

		Vector3 lookPos = currentTargetToLookPosition - cameraAxisPosition;
		Quaternion rotation = Quaternion.LookRotation (lookPos);
		Vector3 rotatioEuler = rotation.eulerAngles;

		float lockedCameraPivotY = lockedCameraPivot.localEulerAngles.y;
		currentLockedLimitLookAngle.x = rotatioEuler.x;
		currentLockedLimitLookAngle.y = rotatioEuler.y - lockedCameraPivotY;

		if (currentLockedCameraAxisInfo.useRotationLimits) {
			if (currentLockedLimitLookAngle.x > 180) {
				currentLockedLimitLookAngle.x -= 360;				
				currentLockedLimitLookAngle.x = Mathf.Clamp (currentLockedLimitLookAngle.x, currentLockedCameraAxisInfo.rotationLimitsX.x, 0);
			} else {
				currentLockedLimitLookAngle.x = Mathf.Clamp (currentLockedLimitLookAngle.x, currentLockedCameraAxisInfo.rotationLimitsX.x, currentLockedCameraAxisInfo.rotationLimitsX.y);
			}

			currentLockedLimitLookAngle.y = Mathf.Clamp (currentLockedLimitLookAngle.y, currentLockedCameraAxisInfo.rotationLimitsY.x, currentLockedCameraAxisInfo.rotationLimitsY.y);
		} 
	}

	public void setCameraLimit (bool useCameraLimitValue, bool useWidthLimitValue, float newWidthLimitRight, float newWidthLimitLeft, bool useHeightLimitValue, float newHeightLimitUpper,
	                            float newHeightLimitLower, Vector3 newCameraLimitPosition, bool depthLimitEnabled, float newDepthLimitFront, float newDepthLimitBackward)
	{
		useCameraLimit = useCameraLimitValue;

		currentCameraLimitPosition = newCameraLimitPosition;

		useWidthLimit = useWidthLimitValue;
		widthLimitRight = newWidthLimitRight;
		widthLimitLeft = newWidthLimitLeft;

		useHeightLimit = useHeightLimitValue;
		heightLimitUpper = newHeightLimitUpper;
		heightLimitLower = newHeightLimitLower;

		useDepthLimit = depthLimitEnabled;
		depthLimitFront = newDepthLimitFront;
		depthLimitBackward = newDepthLimitBackward;
	}

	public void setNewCameraForwardPosition (float newCameraForwardPosition)
	{
		mainCamera.transform.SetParent (null);

		Vector3 originalCameraAxisLocalPosition = currentLockedCameraAxisInfo.originalCameraAxisLocalPosition;

		if (moveInXAxisOn2_5d) {
			lockedCameraAxis.localPosition = new Vector3 (lockedCameraAxis.localPosition.x, lockedCameraAxis.localPosition.y, newCameraForwardPosition);
			currentLockedCameraAxisInfo.originalCameraAxisLocalPosition = new Vector3 (originalCameraAxisLocalPosition.x, originalCameraAxisLocalPosition.y, newCameraForwardPosition);
		} else {
			lockedCameraAxis.localPosition = new Vector3 (newCameraForwardPosition, lockedCameraAxis.localPosition.y, lockedCameraAxis.localPosition.z);
			currentLockedCameraAxisInfo.originalCameraAxisLocalPosition = new Vector3 (newCameraForwardPosition, originalCameraAxisLocalPosition.y, originalCameraAxisLocalPosition.z);
		}

		mainCamera.transform.SetParent (lockedCameraPosition);

		lockedCameraMovement (true);
	}

	public bool isTopdownViewEnabled ()
	{
		return useTopDownView;
	}

	public override bool is2_5ViewActive ()
	{
		return using2_5ViewActive;
	}

	public void callLockedCameraEventOnEnter (lockedCameraSystem.cameraAxis cameraAxisToCheck)
	{
		if (cameraAxisToCheck.useUnityEvent && cameraAxisToCheck.useUnityEventOnEnter) {
			if (cameraAxisToCheck.unityEventOnEnter.GetPersistentEventCount () > 0) {
				cameraAxisToCheck.unityEventOnEnter.Invoke ();
			}
		}
	}

	public void callLockedCameraEventOnExit (lockedCameraSystem.cameraAxis cameraAxisToCheck)
	{
		if (cameraAxisToCheck.useUnityEvent && cameraAxisToCheck.useUnityEventOnExit) {
			if (cameraAxisToCheck.unityEventOnExit.GetPersistentEventCount () > 0) {
				cameraAxisToCheck.unityEventOnExit.Invoke ();
			}
		}
	}

	public override bool isMoveInXAxisOn2_5d ()
	{
		return moveInXAxisOn2_5d;
	}

	public override Vector3 getOriginalLockedCameraPivotPosition ()
	{
		return originalLockedCameraPivotPosition;
	}

	public Ray getCameraRaycastDirection ()
	{
		if (!isCameraTypeFree ()) {
			if (currentLockedCameraCursor != null) {
				return mainCamera.ScreenPointToRay (currentLockedCameraCursor.position);
			}
		}

		Ray newRay = new Ray ();
		newRay.origin = mainCameraTransform.position;
		newRay.direction = mainCameraTransform.TransformDirection (Vector3.forward);

		return newRay;
	}

	public void setManualAimStateOnLockedCamera (bool state)
	{
		if (!isCameraTypeFree ()) {
			setManualAimState (state);
		}
	}

	public void setManualAimState (bool state)
	{
		if (state) {
			setCurrentLockedCameraCursor (weaponsManager.cursorRectTransform);
			setMaxDistanceToCameraCenter (true, 100);

			if (!isCameraTypeFree ()) {
				setLookAtTargetOnLockedCameraState ();
			}

			setLookAtTargetState (true, null);
		} else {
			setCurrentLockedCameraCursor (null);
			setMaxDistanceToCameraCenter (true, 100);
			setLookAtTargetState (false, null);
		}

		playerControllerManager.enableOrDisableAiminig (state);		
	}

	//Adjust the player to a fixed axis position for the 2.5d view
	public void movePlayerToDefaultHorizontalValue2_5d ()
	{
		Vector3 positionToFollow = targetToFollow.position;

		if (moveInXAxisOn2_5d) {
			float forwardDifference = Math.Abs (Math.Abs (positionToFollow.z) - Math.Abs (originalLockedCameraPivotPosition.z));

			if (positionToFollow.z == originalLockedCameraPivotPosition.z || forwardDifference < 0.01f) {
				return;
			}
		} else {
			float rightDifference = Math.Abs (Math.Abs (positionToFollow.x) - Math.Abs (originalLockedCameraPivotPosition.x));

			if (positionToFollow.x == originalLockedCameraPivotPosition.x || rightDifference < 0.01f) {
				return;
			}
		}

		if (fixPlayerZPositionCoroutine != null) {
			StopCoroutine (fixPlayerZPositionCoroutine);
		}

		fixPlayerZPositionCoroutine = StartCoroutine (movePlayerToDefaultHorizontalValue2_5dCoroutine ());
	}

	IEnumerator movePlayerToDefaultHorizontalValue2_5dCoroutine ()
	{
//		playerControllerManager.changeScriptState (false);

		playerControllerManager.setCanMoveState (false);

		playerControllerManager.resetPlayerControllerInput ();

		playerControllerManager.resetOtherInputFields ();

		Vector3 targetPosition = vector3Zero;

		Vector3 currentPosition = targetToFollow.position;

		if (moveInXAxisOn2_5d) {
			targetPosition = new Vector3 (currentPosition.x, currentPosition.y, originalLockedCameraPivotPosition.z);
		} else {
			targetPosition = new Vector3 (originalLockedCameraPivotPosition.x, currentPosition.y, currentPosition.z);
		}

		float dist = GKC_Utils.distance (currentPosition, targetPosition);

		float duration = dist / currentLockedCameraAxisInfo.adjustPlayerPositionToFixed2_5dPosition;

		float t = 0;

		float movementTimer = 0;

		bool targetReached = false;

		float positionDifference = 0;
		float angleDifference = 0;

		if (currentLockedCameraAxisInfo.rotatePlayerToward2dCameraOnTriggerEnter) {
			Vector3 lockedCameraDirection = currentLockedCameraAxisInfo.axis.position - targetToFollow.position;

			lockedCameraDirection = lockedCameraDirection / lockedCameraDirection.magnitude;

			float rotationAngle = Vector3.SignedAngle (targetToFollow.forward, lockedCameraDirection, targetToFollow.up);

			if (moveInXAxisOn2_5d) {
				if (rotationAngle < 0) {
					rotationAngle = -90;
				} else {
					rotationAngle = 90;
				}
			} else {
				if (rotationAngle < 0) {
					rotationAngle = 180;
				} else {
					rotationAngle = -180;
				}
			}

			Vector3 targetRotationEuler = targetToFollow.up * rotationAngle;

			Quaternion targetRotation = Quaternion.Euler (targetRotationEuler);

			Quaternion currentRotation = targetToFollow.rotation;

			while (!targetReached) {
				t += getCurrentDeltaTime () / duration; 

				targetToFollow.position = Vector3.Lerp (currentPosition, targetPosition, t);

				targetToFollow.rotation = Quaternion.Lerp (currentRotation, targetRotation, t);

				movementTimer += getCurrentDeltaTime ();

				angleDifference = Quaternion.Angle (targetToFollow.rotation, targetRotation);

				positionDifference = GKC_Utils.distance (targetToFollow.position, targetPosition);

				if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 2)) {
					targetReached = true;
				}

				yield return null;
			}
		} else {
			while (!targetReached) {
				t += getCurrentDeltaTime () / duration; 

				targetToFollow.position = Vector3.Lerp (currentPosition, targetPosition, t);

				movementTimer += getCurrentDeltaTime ();

				positionDifference = GKC_Utils.distance (targetToFollow.position, targetPosition);

				if (positionDifference < 0.01f || movementTimer > (duration + 1)) {
					targetReached = true;
				}

				yield return null;
			}
		}

		if (lockedCameraMoving) {
			targetReached = false;

			while (!targetReached) {
				if (!lockedCameraMoving) {
					targetReached = true;
				}

				yield return null;
			}
		} else {
			yield return new WaitForSeconds (0.6f);
		}

//		playerControllerManager.changeScriptState (true);

		playerControllerManager.setCanMoveState (true);
	}

	public void stopLockedCameraMovementCoroutine ()
	{
		lockedCameraMoving = false;

		if (lockedCameraCoroutine != null) {
			StopCoroutine (lockedCameraCoroutine);
		}
	}

	public void lockedCameraMovement (bool isBeingSetToLocked)
	{
		if (rotatingLockedCameraFixedRotationAmountActive) {
			stopRotateLockedCameraFixedRotationAmount ();
		}

		stopLockedCameraMovementCoroutine ();

		lockedCameraCoroutine = StartCoroutine (lockedCameraMovementCoroutine (isBeingSetToLocked));
	}

	//move the camera from its position in player camera to a fix position
	IEnumerator lockedCameraMovementCoroutine (bool isBeingSetToLocked)
	{
		lockedCameraMoving = true;

		float i = 0;
		//store the current rotation of the camera
		Quaternion currentQ = mainCamera.transform.localRotation;
		//store the current position of the camera
		Vector3 currentPos = mainCamera.transform.localPosition;

		//translate position and rotation camera
		while (i < 1) {
			i += getCurrentDeltaTime () * currentLockedCameraAxisInfo.cameraTransitionSpeed;

			mainCamera.transform.localRotation = Quaternion.Lerp (currentQ, quaternionIdentity, i);

			mainCamera.transform.localPosition = Vector3.Lerp (currentPos, vector3Zero, i);

			yield return null;
		}

		if (isBeingSetToLocked) {
			lockedCameraChanged = true;
		} else {
			cameraType = typeOfCamera.free;
			cameraCurrentlyLocked = false;
		}

		lockedCameraMoving = false;
	}

	public void setCurrentAxisTransformValues (Transform newValues)
	{
		lockedMainCameraTransform.position = newValues.position;
		lockedMainCameraTransform.eulerAngles = new Vector3 (0, newValues.eulerAngles.y, 0);
	}

	public override Transform getLockedCameraTransform ()
	{
		return lockedMainCameraTransform;
	}

	public void setLockedMainCameraTransformRotation (Vector3 normal)
	{
		if (!cameraCurrentlyLocked) {
			return;
		}

//		print (normal);

		Vector3 previousLockedCameraAxisPosition = vector3Zero;
		Vector3 lockedCameraAxisLocalPosition = vector3Zero;
		Quaternion previousLockedCameraPivotRotation = lockedCameraPivot.rotation;

		if (moveInXAxisOn2_5d) {
			Quaternion targetRotation = Quaternion.LookRotation (Vector3.forward, normal);
			float rotationAmount = targetRotation.eulerAngles.z;

			lockedCameraPivot.eulerAngles = new Vector3 (0, 0, rotationAmount);
		} else {
			Quaternion targetRotation = Quaternion.LookRotation (Vector3.right, normal);
			float rotationAmount = targetRotation.eulerAngles.z;

			lockedCameraPivot.eulerAngles = new Vector3 (rotationAmount, 0, 0);
		}

//		print (lockedCameraPivot.eulerAngles);

		float lookCameraParentTargetAngle = 0;
		float lockedMainCameraTransformTargetAngle = 0;

		if (normal != Vector3.up) {
			float currentUpAngle = 0;
			float currentRightAngle = 0;

			if (moveInXAxisOn2_5d) {
				currentUpAngle = Vector3.SignedAngle (normal, Vector3.up, Vector3.forward);
				currentRightAngle = Vector3.SignedAngle (normal, Vector3.right, Vector3.forward);
			} else {
				currentUpAngle = Vector3.SignedAngle (normal, Vector3.up, Vector3.right);
				currentRightAngle = Vector3.SignedAngle (normal, Vector3.right, Vector3.right);
			}

			//print (currentUpAngle + " " + currentRightAngle);

			lockedMainCameraTransformTargetAngle = currentUpAngle;
			lookCameraParentTargetAngle = currentUpAngle;

			if (currentUpAngle < 0) {
				lockedMainCameraTransformTargetAngle = -currentUpAngle;
				lookCameraParentTargetAngle = -currentUpAngle;
			}

			if (currentUpAngle < -90) {
				lockedMainCameraTransformTargetAngle = currentUpAngle + 90;
			}

			if (currentUpAngle > 175 && currentUpAngle < 185) {
				lockedMainCameraTransformTargetAngle = 0;
			}

			if (currentUpAngle > 90 && currentUpAngle < 175) {
				lockedMainCameraTransformTargetAngle = 90 - currentRightAngle;
				lookCameraParentTargetAngle = 360 - currentUpAngle;
			}

			if (currentUpAngle > 85 && currentUpAngle < 95) {
				lookCameraParentTargetAngle = -currentUpAngle;
			}

			if (currentUpAngle > 0 && currentUpAngle < 85) {
				lockedMainCameraTransformTargetAngle = -currentUpAngle;
				lookCameraParentTargetAngle = -currentUpAngle;
			}

		}

		lockedMainCameraTransform.eulerAngles = new Vector3 (lockedMainCameraTransform.eulerAngles.x, lockedMainCameraTransform.eulerAngles.y, lockedMainCameraTransformTargetAngle);

		if (moveInXAxisOn2_5d) {
			lookCameraParent.localRotation = Quaternion.Euler (new Vector3 (0, 0, lookCameraParentTargetAngle));
		} else {
			lookCameraParent.localRotation = Quaternion.Euler (new Vector3 (lookCameraParentTargetAngle, 0, 0));
		}

		auxLockedCameraAxis.localPosition = currentLockedCameraAxisInfo.originalCameraAxisLocalPosition;

		previousLockedCameraAxisPosition = auxLockedCameraAxis.position;

		lockedCameraPivot.rotation = previousLockedCameraPivotRotation;

		auxLockedCameraAxis.position = previousLockedCameraAxisPosition;

		lockedCameraAxisLocalPosition = auxLockedCameraAxis.localPosition;

		if (moveInXAxisOn2_5d) {
			setLockedCameraAxisPositionOnGravityChange (
				new Vector3 (lockedCameraAxisLocalPosition.x, lockedCameraAxisLocalPosition.y, currentLockedCameraAxisInfo.originalCameraAxisLocalPosition.z));
		} else {
			setLockedCameraAxisPositionOnGravityChange (
				new Vector3 (currentLockedCameraAxisInfo.originalCameraAxisLocalPosition.x, lockedCameraAxisLocalPosition.y, lockedCameraAxisLocalPosition.z));
		}
	}

	public void setLockedCameraAxisPositionOnGravityChange (Vector3 newPosition)
	{
		if (adjustLockedCameraAxisPositionOnGravityChangeCoroutine != null) {
			StopCoroutine (adjustLockedCameraAxisPositionOnGravityChangeCoroutine);
		}

		adjustLockedCameraAxisPositionOnGravityChangeCoroutine = StartCoroutine (setLockedCameraAxisPositionOnGravityChangeCoroutine (newPosition));
	}

	//move the camera from its position in player camera to a fix position
	IEnumerator setLockedCameraAxisPositionOnGravityChangeCoroutine (Vector3 newPosition)
	{
		float i = 0;

		Vector3 currentPos = lockedCameraAxis.localPosition;

		while (i < 1) {
			i += getCurrentDeltaTime () * currentLockedCameraAxisInfo.cameraTransitionSpeed;

			lockedCameraAxis.localPosition = Vector3.Lerp (lockedCameraAxis.localPosition, newPosition, i);

			yield return null;
		}
	}

	public override void setPlayerAndCameraParent (Transform newParent)
	{
		playerControllerGameObject.transform.SetParent (newParent);
		playerCameraTransform.SetParent (newParent);

		if (newParent == null && gravityManager.getCurrentNormal () == Vector3.up) {
			playerControllerGameObject.transform.rotation = Quaternion.Euler (Vector3.Scale (gravityManager.getCurrentNormal (), playerControllerGameObject.transform.eulerAngles));
			playerCameraTransform.rotation = Quaternion.Euler (Vector3.Scale (gravityManager.getCurrentNormal (), playerCameraTransform.eulerAngles));
		}

		if (!isCameraTypeFree () && currentLockedCameraAxisInfo.followPlayerPosition) {
			if (newParent != null) {
				lockedCameraPivot.SetParent (newParent);
			} else {
				lockedCameraPivot.SetParent (lockedCameraElementsParent);
			}
		}
	}

	public Transform getCurrentLockedCameraTransform ()
	{
		return lockedCameraPosition;
	}

	public Transform getCurrentLockedCameraAxis ()
	{
		return lockedCameraAxis;
	}

	public Transform getCameraTransform ()
	{
		return mainCameraTransform;
	}

	public Camera getMainCamera ()
	{
		if (!mainCameraAssigned) {
			mainCameraAssigned = mainCamera != null;

			if (!mainCameraAssigned) {
				mainCamera = Camera.main;

				mainCameraAssigned = true;

				setNewMainCameraOnMainParent ();
			}
		}

		return mainCamera;
	}

	public void setNewMainCameraOnMainParent ()
	{
		mainCamera.transform.SetParent (mainCameraParent);

		mainCamera.transform.localPosition = Vector3.zero;
		mainCamera.transform.localEulerAngles = Vector3.zero;
	}

	public Transform getPivotCameraTransform ()
	{
		return pivotCameraTransform;
	}

	public void setLookAtBodyPartsOnCharactersState (bool state)
	{
		lookAtBodyPartsOnCharacters = state;
	}

	public void setOriginalLookAtBodyPartsOnCharactersState ()
	{
		setLookAtBodyPartsOnCharactersState (originalLookAtBodyPartsOnCharactersValue);
	}

	//Aim assist functions, and get the closest object to look
	public bool getClosestTargetToLook ()
	{
		bool targetFound = false;

		targetsListToLookTransform.Clear ();
		List<Collider> targetsListCollider = new List<Collider> ();

		List<GameObject> targetist = new List<GameObject> ();
		List<GameObject> fullTargetList = new List<GameObject> ();

		if (useLayerToSearchTargets) {
			targetsListCollider.AddRange (Physics.OverlapSphere (playerCameraTransform.position, maxDistanceToFindTarget, layerToLook));

			for (int i = 0; i < targetsListCollider.Count; i++) {
				fullTargetList.Add (targetsListCollider [i].gameObject);
			}
		} else {
			for (int i = 0; i < tagToLookList.Count; i++) {
				GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (tagToLookList [i]);
				targetist.AddRange (enemiesList);
			}

			for (int i = 0; i < targetist.Count; i++) {	
				float distance = GKC_Utils.distance (targetist [i].transform.position, playerCameraTransform.position);

				if (distance < maxDistanceToFindTarget) {
					fullTargetList.Add (targetist [i]);
				}
			}
		}

		if (fullTargetList.Contains (playerControllerGameObject)) {
			fullTargetList.Remove (playerControllerGameObject);
		}

		List<GameObject> pointToLookComponentList = new List<GameObject> ();

		if (searchPointToLookComponents) {
			targetsListCollider.Clear ();

			targetsListCollider.AddRange (Physics.OverlapSphere (playerCameraTransform.position, maxDistanceToFindTarget, pointToLookComponentsLayer));

			for (int i = 0; i < targetsListCollider.Count; i++) {
				if (targetsListCollider [i].isTrigger) {
					pointToLook currentPointToLook = targetsListCollider [i].GetComponent<pointToLook> ();

					if (currentPointToLook != null) {
						GameObject currenTargetToLook = currentPointToLook.getPointToLookTransform ().gameObject;

						fullTargetList.Add (currenTargetToLook);

						pointToLookComponentList.Add (currenTargetToLook);
					}
				}
			}
		}

		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		for (int i = 0; i < fullTargetList.Count; i++) {
			if (fullTargetList [i] != null) {
				GameObject currentTarget = fullTargetList [i];

				if (tagToLookList.Contains (currentTarget.tag) || pointToLookComponentList.Contains (currentTarget)) {
					bool objectVisible = false;
					bool obstacleDetected = false;

					Vector3 targetPosition = currentTarget.transform.position;

					if (lookOnlyIfTargetOnScreen) {
						Transform currentTargetPlaceToShoot = applyDamage.getPlaceToShoot (currentTarget);

						if (currentTargetPlaceToShoot != null) {
							targetPosition = currentTargetPlaceToShoot.position;
						}

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (targetPosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (targetPosition);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
						}

						//the target is visible in the screen
						if (targetOnScreen) {
							objectVisible = true;
						}
					} else {
						objectVisible = true;
					}

					if (objectVisible && checkObstaclesToTarget) {
						//for every target in front of the camera, use a raycast, if it finds an obstacle between the target and the camera, the target is removed from the list
						Vector3 temporaltargetPosition = targetPosition;

						Transform temporalPlaceToShoot = applyDamage.getPlaceToShoot (currentTarget);

						if (temporalPlaceToShoot != null) {
							temporaltargetPosition = temporalPlaceToShoot.position;
						}

						Vector3 direction = temporaltargetPosition - mainCameraTransform.position;

						direction = direction / direction.magnitude;

						float distance = GKC_Utils.distance (temporaltargetPosition, mainCameraTransform.position);

						if (Physics.Raycast (temporaltargetPosition, -direction, out hit, distance, layerToLook)) {
							obstacleDetected = true;

							if (settings.showCameraGizmo) {
								Debug.DrawLine (temporaltargetPosition, hit.point, Color.white, 4);
							}

							if (showDebugPrint) {
								print ("obstacle detected " + hit.collider.name + " " + currentTarget.name);
							}
						}
					} 

					if (objectVisible && !obstacleDetected) {
						targetsListToLookTransform.Add (currentTarget.transform);
					}
				} 
			}
		}

		//finally, get the target closest to the player
		float minDistance = Mathf.Infinity;

		Vector3 centerScreen = getScreenCenter ();
	
		for (int i = 0; i < targetsListToLookTransform.Count; i++) {

			//find closes element to center screen
			if (getClosestToCameraCenter) {
				targetPosition = targetsListToLookTransform [i].position;

				placeToShoot = applyDamage.getPlaceToShoot (targetsListToLookTransform [i].gameObject);

				if (placeToShoot != null) {
					targetPosition = placeToShoot.position;
				}

				screenPoint = mainCamera.WorldToScreenPoint (targetPosition);

//				print (screenPoint + " " + centerScreen);

				float currentDistance = GKC_Utils.distance (screenPoint, centerScreen);

				bool canBeChecked = false;

				if (useMaxDistanceToCameraCenter && !lookAtBodyPartsOnCharacters) {
					if (currentDistance < maxDistanceToCameraCenter) {
						canBeChecked = true;
					}
				} else {
					canBeChecked = true;
				}

				//print (currentDistance +" "+ canBeChecked);

				if (canBeChecked) {
//					print (currentDistance + " " + minDistance + " " + targetsListToLookTransform [i].name);

					if (currentDistance < minDistance) {
						minDistance = currentDistance;

						setTargetToLook (targetsListToLookTransform [i]);

						targetFound = true;
					}
				}
			} else {
				float currentDistance = GKC_Utils.distance (targetsListToLookTransform [i].position, playerCameraTransform.position);

				if (currentDistance < minDistance) {
					minDistance = currentDistance;

					setTargetToLook (targetsListToLookTransform [i]);

					targetFound = true;
				}
			}
		}

		if (targetFound) {
			checkRemoveEventOnLockedOnStart (targetToLook);

			//			print (targetToLook.name);

			checkTargetToLookShader (targetToLook);

			//print (targetToLook.name);

			bool bodyPartFound = false;

			if (lookAtBodyPartsOnCharacters) {

				Transform bodyPartToLook = targetToLook;

				List<health.weakSpot> characterWeakSpotList = applyDamage.getCharacterWeakSpotList (targetToLook.gameObject);

				if (characterWeakSpotList != null) {
					
					minDistance = Mathf.Infinity;

					for (int i = 0; i < characterWeakSpotList.Count; i++) {
						if (bodyPartsToLook.Contains (characterWeakSpotList [i].name)) {

							if (characterWeakSpotList [i].spotTransform != null) {

								screenPoint = mainCamera.WorldToScreenPoint (characterWeakSpotList [i].spotTransform.position);

								float currentDistance = GKC_Utils.distance (screenPoint, centerScreen);

								//print ("distance to body part " + currentDistance + " "+useMaxDistanceToCameraCenter);
								bool canBeChecked = false;

								if (useMaxDistanceToCameraCenter) {
									if (currentDistance < maxDistanceToCameraCenter) {
										canBeChecked = true;
									}
								} else {
									canBeChecked = true;
								}

								if (canBeChecked) {
									if (currentDistance < minDistance) {
										minDistance = currentDistance;
										bodyPartToLook = characterWeakSpotList [i].spotTransform;

										bodyPartFound = true;
									}
								}
							}
						}
					}

					//print (bodyPartToLook.name);
					setTargetToLook (bodyPartToLook);
				}
			} 

			if (!bodyPartFound) {
				placeToShoot = applyDamage.getPlaceToShoot (targetToLook.gameObject);

				if (placeToShoot != null) {
					setTargetToLook (placeToShoot);
				}

				//check if the object to check is too far from screen center in case the llok at body parts on characters is active and no body part is found or is close enough to screen center
				if (lookAtBodyPartsOnCharacters && useMaxDistanceToCameraCenter && getClosestToCameraCenter) {
					screenPoint = mainCamera.WorldToScreenPoint (targetToLook.position);

					float currentDistance = GKC_Utils.distance (screenPoint, centerScreen);

					if (currentDistance > maxDistanceToCameraCenter) {
						setTargetToLook (null);
						targetFound = false;
						//print ("cancel look at target");
					}
				}
			}
		}

		return targetFound;
	}

	Transform previousTargetToLook;

	//update look at target position
	void lookAtTarget ()
	{
		bool lookingTarget = false;
		lookTargetPosition = vector3Zero;

		//looking at a target in the scene, like an enemy, a vehicle, any element inside the tags to look list
		if (targetToLook != null) {
			lookingTarget = true;
			lookTargetPosition = targetToLook.position;

			lookAtTargetTransform.position = pivotCameraTransform.position
			+ pivotCameraTransform.transform.right * currentState.camPositionOffset.x
			+ pivotCameraTransform.transform.up * currentState.camPositionOffset.y;
		} 

		//look to a position created in the locked camera, like the position where the player is aiming right now
		if (lookingAtPoint) {
			lookingTarget = true;
			lookTargetPosition = pointTargetPosition;

			lookAtTargetTransform.position = pivotCameraTransform.position;
		}

		if (settings.showCameraGizmo) {
			if (lookingTarget) {
				Debug.DrawLine (mainCameraTransform.position, lookTargetPosition, Color.green);
			}
		}

		Vector3 targetDir = lookTargetPosition - lookAtTargetTransform.position;
		Quaternion targetRotation = Quaternion.LookRotation (targetDir, playerCameraTransform.up);

		lookAtTargetTransform.rotation = targetRotation;

		navMeshCurrentLookPos = lookAtTargetTransform.forward;

		if (useLookTargetIcon) {

			if (usingScreenSpaceCamera) {
				screenPoint = mainCamera.WorldToViewportPoint (lookTargetPosition);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
			} else {
				screenPoint = mainCamera.WorldToScreenPoint (lookTargetPosition);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
			}

			if (targetOnScreen) {
				if (usingScreenSpaceCamera) {
					lookAtTargetIconRectTransform.anchoredPosition = 
						new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);
				} else {
					lookAtTargetIcon.transform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
				}
			} 
		}

		if (!lookingTarget) {
			setLookAtTargetState (false, null);
		}
	}

	public void setLookAtTargetStateInput (bool state)
	{
		//the player is in locked camera mode, so search if there is a target list stored to aim to the next target
		if (searchingToTargetOnLockedCamera) {
			currentTargetToLookIndex++;

			for (int i = 0; i < targetsListToLookTransform.Count; i++) {
				if (targetsListToLookTransform [i] == null) {
					targetsListToLookTransform.RemoveAt (i);
					i = 0;
				}
			}

			if (currentTargetToLookIndex >= targetsListToLookTransform.Count) {
				currentTargetToLookIndex = 0;
			}

			if (targetsListToLookTransform.Count > 0) {
				print ("new " + targetsListToLookTransform [currentTargetToLookIndex].name);
				setLookAtTargetState (true, targetsListToLookTransform [currentTargetToLookIndex]);
			} else {
				setLookAtTargetState (true, null);
			}
		} 
		//else, set the regular look at target mode
		else {
			setLookAtTargetStateManual (state, null);
		}
	}

	public void checkIfPlayerIsLookingAtDeadTarget (bool state)
	{
		if (isCameraTypeFree ()) {
			setLookAtTargetStateManual (state, null);
		} else {
			if (currentLockedCameraAxisInfo.showCameraCursorWhenNotAiming) {

				settingShowCameraCursorWhenNotAimingBackToActive = true;

				activateShowCameraCursorWhenNotAimingState ();

				settingShowCameraCursorWhenNotAimingBackToActive = false;
			} else {
				setLookAtTargetStateManual (state, null);
			}
		}
	}

	public void setLookAtTargetStateManual (bool state, Transform objectToLook)
	{
		setLookAtTargetState (state, objectToLook);

		lookintAtTargetByInput = state;
	}

	//look at target
	public void setLookAtTargetState (bool state, Transform objectToLook)
	{
		if (!lookAtTargetEnabled) {
			return;
		}

		Quaternion targetRotation = Quaternion.LookRotation (mainCameraTransform.forward, playerCameraTransform.up);
		lookAtTargetTransform.rotation = targetRotation;

		lookingAtTarget = state;
		lastTimeAimAsisst = Time.time;

		if (!lookingAtTarget) {
			searchingToTargetOnLockedCamera = false;
		}

		if (isCameraTypeFree ()) {
			lookingAtPoint = false;
		} else {
//			print ("LOCK-ON " + state);
			lookingAtPoint = state;

			lookintAtTargetByInput = state;
		}

		//check if the player is searching to look at a possible target
		if (lookingAtTarget && !lookingAtPoint) {
			if (objectToLook != null) {
				setTargetToLook (objectToLook);

				checkTargetToLookShader (targetToLook);
			} else if (!getClosestTargetToLook ()) {
				setLookAtTargetState (false, null);

				setTargetToLook (null);

				checkTargetToLookShader (null);
			}
		} else {
			//if the player is currently on locked mode, check to aim to the closest enemy
			if (searchingToTargetOnLockedCamera || activeLookAtTargetOnLockedCamera) {
				if (activeLookAtTargetOnLockedCamera) {
					lookingAtPoint = false;
					lookingAtFixedTarget = true;
				} else {
					lookingAtFixedTarget = false;
				}

				if (objectToLook != null) {
					setTargetToLook (objectToLook);

					checkTargetToLookShader (targetToLook);

					placeToShoot = applyDamage.getPlaceToShoot (targetToLook.gameObject);

					if (placeToShoot != null) {
						setTargetToLook (placeToShoot);
					}
				} else {
					checkTargetToLookShader (null);
				}

				if (currentLockedCameraAxisInfo.useAimAssist) {
					//if no target is found, then set the state to just aim on the screen by moving the mouse
					if ((objectToLook == null && !getClosestTargetToLookOnLockedCamera ()) || settingShowCameraCursorWhenNotAimingBackToActive) {
						if (activeLookAtTargetOnLockedCamera) {
							lookingAtTarget = false;
							lookingAtPoint = false;

							setTargetToLook (null);

							checkTargetToLookShader (null);

							lookingAtFixedTarget = false;
							lookintAtTargetByInput = false;
							activeLookAtTargetOnLockedCamera = false;

							return;
						} else {
							lookingAtFixedTarget = false;
							lookingAtPoint = true;

							setTargetToLook (null);

							checkTargetToLookShader (null);
						}
					} else {
						//else, it means a target to aim was found

						if (usingScreenSpaceCamera) {
							currentLockedCameraCursor.anchoredPosition = getIconPosition (targetToLook.position);
						} else {
							Vector3 screenPos = mainCamera.WorldToScreenPoint (targetToLook.position);

							if (currentLockedCameraCursor != null) {
								currentLockedCameraCursor.transform.position = screenPos;
							}
						}
					}
				} else {
					lookingAtFixedTarget = false;
					lookingAtPoint = true;

					setTargetToLook (null);

					checkTargetToLookShader (null);
				}
			} else {
				lookingAtFixedTarget = false;

				checkTargetToLookShader (null);
			}
		}

		if (!lookingAtTarget) {
			setMaxDistanceToCameraCenter (originalUsemaxDistanceToCameraCenterValue, originalMaxDistanceToCameraCenterValue);

			checkRemoveEventOnLockOnEnd ();
		}

		if (useLookTargetIcon) {
			if (lookAtTargetIcon.activeSelf != lookingAtTarget) {
				lookAtTargetIcon.SetActive (lookingAtTarget);
			}

			if (lookAtTargetRegularIconGameObject.activeSelf != (!activateIconWhenNotAiming)) {
				lookAtTargetRegularIconGameObject.SetActive (!activateIconWhenNotAiming);
			}

			if (lookAtTargetIconWhenNotAiming.activeSelf != activateIconWhenNotAiming) {
				lookAtTargetIconWhenNotAiming.SetActive (activateIconWhenNotAiming);
			}
		}

		activateIconWhenNotAiming = false;
	}

	bool activateIconWhenNotAiming;

	public void setUseLookTargetIconState (bool state)
	{
		useLookTargetIcon = state;
	}

	public void setOriginalUseLookTargetIconValue ()
	{
		setUseLookTargetIconState (originalUseLookTargetIconValue);
	}

	public bool lookingAtFixedTarget;

	public override bool isPlayerLookingAtTarget ()
	{
		return lookingAtTarget;
	}

	public override bool istargetToLookLocated ()
	{
		return targetToLook != null;
	}

	public void setLookAtTargetOnLockedCameraState ()
	{
		searchingToTargetOnLockedCamera = true;

//		print ("locked");
	}

	public bool getClosestTargetToLookOnLockedCamera ()
	{
		bool targetFound = false;

		currentTargetToLookIndex = 0;

		targetsListToLookTransform.Clear ();

		List<Collider> targetsListCollider = new List<Collider> ();

		List<GameObject> targetist = new List<GameObject> ();
		List<GameObject> fullTargetList = new List<GameObject> ();

		if (useLayerToSearchTargets) {
			targetsListCollider.AddRange (Physics.OverlapSphere (playerCameraTransform.position, maxDistanceToFindTarget, layerToLook));

			for (int i = 0; i < targetsListCollider.Count; i++) {
				fullTargetList.Add (targetsListCollider [i].gameObject);
			}
		} else {
			for (int i = 0; i < tagToLookList.Count; i++) {
				GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (tagToLookList [i]);
				targetist.AddRange (enemiesList);
			}

			for (int i = 0; i < targetist.Count; i++) {	
				float distance = GKC_Utils.distance (targetist [i].transform.position, playerCameraTransform.position);

				if (distance < maxDistanceToFindTarget) {
					fullTargetList.Add (targetist [i]);
				}
			}
		}

		if (fullTargetList.Contains (playerControllerGameObject)) {
			fullTargetList.Remove (playerControllerGameObject);
		}

		List<GameObject> pointToLookComponentList = new List<GameObject> ();

		if (searchPointToLookComponents) {
			targetsListCollider.Clear ();

			targetsListCollider.AddRange (Physics.OverlapSphere (playerCameraTransform.position, maxDistanceToFindTarget, pointToLookComponentsLayer));

			for (int i = 0; i < targetsListCollider.Count; i++) {
				if (targetsListCollider [i].isTrigger) {
					pointToLook currentPointToLook = targetsListCollider [i].GetComponent<pointToLook> ();

					if (currentPointToLook != null) {
						GameObject currenTargetToLook = currentPointToLook.getPointToLookTransform ().gameObject;

						fullTargetList.Add (currenTargetToLook);

						pointToLookComponentList.Add (currenTargetToLook);
					}
				}
			}
		}

		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		for (int i = 0; i < fullTargetList.Count; i++) {
			if (fullTargetList [i] != null) {
				GameObject currentTarget = fullTargetList [i];

				if (tagToLookList.Contains (currentTarget.tag) || pointToLookComponentList.Contains (currentTarget)) {

					bool obstacleDetected = false;

					//for every target in front of the camera, use a raycast, if it finds an obstacle between the target and the camera, the target is removed from the list
					Vector3 originPosition = playerCameraTransform.position + playerCameraTransform.up * 0.5f;
					Vector3 targetPosition = currentTarget.transform.position + currentTarget.transform.up * 0.5f;

					Vector3 direction = targetPosition - originPosition;
					direction = direction / direction.magnitude;

					float distance = GKC_Utils.distance (targetPosition, originPosition);

					if (settings.showCameraGizmo) {
						Debug.DrawLine (originPosition, originPosition + direction * distance, Color.black, 4);
					}

					if (Physics.Raycast (originPosition, direction, out hit, distance, layerToLook)) {
						if (settings.showCameraGizmo) {
							Debug.DrawLine (targetPosition, hit.point, Color.cyan, 4);
						}

						if (hit.collider.gameObject != currentTarget) {
							obstacleDetected = true;
						}
					}

					bool objectVisible = false;

					if (currentLockedCameraAxisInfo.lookOnlyIfTargetOnScreen) {

						if (usingScreenSpaceCamera) {
							screenPoint = mainCamera.WorldToViewportPoint (currentTarget.transform.position);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
						} else {
							screenPoint = mainCamera.WorldToScreenPoint (currentTarget.transform.position);
							targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
						}

						//the target is visible in the screen
						if (targetOnScreen) {
							//check if the target is visible on the current locked camera
							if (currentLockedCameraAxisInfo.lookOnlyIfTargetVisible) {
								originPosition = mainCamera.transform.position;
								targetPosition = currentTarget.transform.position + currentTarget.transform.up * 0.5f;

								direction = targetPosition - originPosition;
								direction = direction / direction.magnitude;

								distance = GKC_Utils.distance (targetPosition, originPosition);

								if (Physics.Raycast (originPosition, direction, out hit, distance, layerToLook)) {
									if (settings.showCameraGizmo) {
										Debug.DrawLine (targetPosition, hit.point, Color.cyan, 4);
									}

									if (hit.collider.gameObject == currentTarget) {
										objectVisible = true;
									}
								}
							} else {
								objectVisible = true;
							}
						}
					} else {
						objectVisible = true;
					}

					if (objectVisible && !obstacleDetected) {
						targetsListToLookTransform.Add (currentTarget.transform);
					}
				}
			}
		}

		//finally, get the target closest to the player
		float minDistance = Mathf.Infinity;

		for (int i = 0; i < targetsListToLookTransform.Count; i++) {
			float currentDistance = GKC_Utils.distance (targetsListToLookTransform [i].position, playerCameraTransform.position);

			if (currentDistance < minDistance) {
				minDistance = currentDistance;
				setTargetToLook (targetsListToLookTransform [i]);

				targetFound = true;
			}
		}

		targetsListToLookTransform.Sort (delegate(Transform a, Transform b) {
			return Vector3.Distance (playerCameraTransform.position, a.position)
				.CompareTo (
				Vector3.Distance (playerCameraTransform.position, b.position));
		});

		if (targetFound) {
			//			bool bodyPartFound = false;
			//			if (lookAtBodyPartsOnCharacters) {
			//
			//				Transform bodyPartToLook = targetToLook;
			//
			//				List<health.weakSpot> characterWeakSpotList = applyDamage.getCharacterWeakSpotList (targetToLook.gameObject);
			//
			//				if (characterWeakSpotList != null) {
			//					minDistance = Mathf.Infinity;
			//					for (int i = 0; i < characterWeakSpotList.Count; i++) {
			//						if (bodyPartsToLook.Contains (characterWeakSpotList [i].name)) {
			//
			//							screenPoint = mainCamera.WorldToScreenPoint (characterWeakSpotList [i].spotTransform.position);
			//							float currentDistance = Utils.distance (screenPoint, centerScreen);
			//							if (currentDistance < minDistance) {
			//								minDistance = currentDistance;
			//								bodyPartToLook = characterWeakSpotList [i].spotTransform;
			//							}
			//						}
			//					}
			//
			//					bodyPartFound = true;
			//
			//					targetToLook = bodyPartToLook;
			//				}
			//			} 
			//
			//			if (!bodyPartFound) {

			checkTargetToLookShader (targetToLook);

			placeToShoot = applyDamage.getPlaceToShoot (targetToLook.gameObject);

			if (placeToShoot != null) {
				setTargetToLook (placeToShoot);
			}
		}

		return targetFound;
	}

	public void checkRemoveEventOnLockedOnStart (Transform objectToCheck)
	{
		if (useRemoteEventsOnLockOn) {
			if (previousTargetToLook != objectToCheck) {
				previousTargetToLook = objectToCheck;

				remoteEventSystem currentRemoteEventSystem = objectToCheck.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventOnLockOnStart.Count; i++) {
						currentRemoteEventSystem.callRemoteEvent (remoteEventOnLockOnStart [i]);
					}
				}
			}
		}
	}

	public void checkRemoveEventOnLockOnEnd ()
	{
		if (useRemoteEventsOnLockOn) {
			if (previousTargetToLook != null) {
				remoteEventSystem currentRemoteEventSystem = previousTargetToLook.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventOnLockOnEnd.Count; i++) {
						currentRemoteEventSystem.callRemoteEvent (remoteEventOnLockOnEnd [i]);
					}
				}

				previousTargetToLook = null;
			}
		}
	}

	public void checkEventOnLockedOnStart ()
	{
		if (useEventsOnLockOn) {
			eventOnLockOnStart.Invoke ();
		}
	}

	public void checkEventOnLockOnEnd ()
	{
		if (useEventsOnLockOn) {
			eventOnLockOnEnd.Invoke ();
		}
	}

	public void setTargetToLook (Transform newTarget)
	{
		targetToLook = newTarget;
	}

	public override Transform getCurrentTargetToLook ()
	{
		return targetToLook;
	}

	public GameObject getPlayerControllerGameObject ()
	{
		return playerControllerGameObject;
	}

	public void setHipsTransform (Transform newHips)
	{
		hipsTransform = newHips;

		updateComponent ();
	}

	public void updateHipsTransformInGame (Transform newHips)
	{
		hipsTransform = newHips;
	}

	public void checkTargetToLookShader (Transform newObjectFound)
	{
		if (pauseOutlineShaderChecking) {
			return;
		}

		if (useObjectToGrabFoundShader) {
			checkIfSetOriginalShaderToPreviousObjectToGrabFound ();

			if (newObjectFound != null) {
				checkIfSetNewShaderToObjectToGrabFound (newObjectFound.gameObject);
			}
		}
	}

	public void checkIfSetNewShaderToObjectToGrabFound (GameObject objectToCheck)
	{
		if (useObjectToGrabFoundShader) {
			currentOutlineObjectSystem = objectToCheck.GetComponentInChildren<outlineObjectSystem> ();

			if (currentOutlineObjectSystem != null) {
				currentOutlineObjectSystem.setOutlineState (true, objectToGrabFoundShader, shaderOutlineWidth, shaderOutlineColor, playerControllerManager);
			}
		}
	}

	public void checkIfSetOriginalShaderToPreviousObjectToGrabFound ()
	{
		if (useObjectToGrabFoundShader && currentOutlineObjectSystem != null) {

			currentOutlineObjectSystem.setOutlineState (false, null, 0, Color.white, playerControllerManager);

			currentOutlineObjectSystem = null;
		}
	}


	public void setLookAtTargetEnabledState (bool state)
	{
		lookAtTargetEnabled = state;
	}

	public void setLookAtTargetEnabledStateDuration (bool currentState, float duration, bool nextState)
	{
		if (lookAtTargetEnabledCoroutine != null) {
			StopCoroutine (lookAtTargetEnabledCoroutine);
		}

		lookAtTargetEnabledCoroutine = StartCoroutine (setLookAtTargetEnabledStateDurationCoroutine (currentState, duration, nextState));
	}

	IEnumerator setLookAtTargetEnabledStateDurationCoroutine (bool currentState, float duration, bool nextState)
	{
		lookAtTargetEnabled = currentState;

		yield return new WaitForSeconds (duration);

		lookAtTargetEnabled = true;

		setLookAtTargetStateInput (false);

		lookAtTargetEnabled = nextState;

		if (lookAtTargetEnabled) {
			setOriginallookAtTargetSpeedValue ();
		}
	}

	public void setLookAtTargetSpeedValue (float newValue)
	{
		lookAtTargetSpeed = newValue;
	}

	public void setOriginallookAtTargetSpeedValue ()
	{
		lookAtTargetSpeed = originalLookAtTargetSpeed;
	}

	public void setMaxDistanceToFindTargetValue (float newValue)
	{
		maxDistanceToFindTarget = newValue;
	}

	public void setOriginalmaxDistanceToFindTargetValue ()
	{
		maxDistanceToFindTarget = originalMaxDistanceToFindTarget;
	}

	public void setMaxDistanceToCameraCenter (bool useMaxDistance, float maxDistance)
	{
		useMaxDistanceToCameraCenter = useMaxDistance;
		maxDistanceToCameraCenter = maxDistance;
	}

	public void death (bool state, bool followHips)
	{
		dead = state;

		headBobManager.playerAliveOrDead (dead);

		if (!firstPersonActive) {
			if (state) {
				if (followHips) {
					smoothFollow = true;

					smoothReturn = false;

					smoothGo = true;
					//this is for the ragdoll, it gets the hips of the player, which is the hips and the parent of the ragdoll
					//the hips is the object that the camera will follow when the player dies, because when this happens, the body of the player is out of the player controller
					//because, while the skeleton of the model will move by the gravity, player controller will not move of its position, due to player has dead
					targetToFollow = hipsTransform;
				}
			} else {
				smoothFollow = false;

				smoothReturn = true;

				smoothGo = false;

				targetToFollow = playerControllerGameObject.transform;
			}
		}
	}

	public void slerpCameraState (cameraStateInfo to, cameraStateInfo from, float lerpSpeed)
	{
		to.assignCameraStateValues (from, lerpSpeed);
	}

	public void slerpCameraStateAtOnce (cameraStateInfo to, cameraStateInfo from)
	{
		to.assignCameraStateValues (from, 0);
	}

	public void setTargetToFollow (Transform newTargetToFollow)
	{
		targetToFollow = newTargetToFollow;
		playerCameraTransform.position = targetToFollow.position;
	}

	public void setOriginalTargetToFollow ()
	{
		setTargetToFollow (playerControllerGameObject.transform);
	}

	public void setCameraStateOnlyOnThirdPerson (string stateName)
	{
		if (!isFirstPersonActive () && isCameraTypeFree ()) {
			setCameraState (stateName);
		}
	}

	public void setDefaultCameraStateOnlyOnThirdPerson ()
	{
		setCameraStateOnlyOnThirdPerson (defaultThirdPersonStateName);
	}

	public void setCameraState (string stateName)
	{
		int cameraStateEventInfoIndex = -1;

		if (useEventsOnCameraStateChange) {
			if (lerpState != null) {
				cameraStateEventInfoIndex = cameraStateEventInfoList.FindIndex (s => s.Name.ToLower () == lerpState.Name.ToLower ());

				if (cameraStateEventInfoIndex > -1) {
					cameraStateEventInfoList [cameraStateEventInfoIndex].eventOnCameraEnd.Invoke ();
				}
			}
		}

		for (int i = 0; i < playerCameraStates.Count; i++) {
			if (playerCameraStates [i].Name.Equals (stateName)) {
				cameraStateInfo newState = new cameraStateInfo (playerCameraStates [i]);

				lerpState = newState;

				currentStateName = stateName;

				if (moveCameraPositionWithMouseWheelActive && useCameraMouseWheelStates) {
					updateCurrentMouseWheelCameraStateByDistance ();
				}

				if (leanActive) {
					if (lerpState.leanEnabled) {
						removeMainCameraPivotFromLeanPivot ();

						if (leanToRightActive) {
							enableOrDisableRightLean (true);
						} else if (leanToLeftActive) {
							enableOrDisableLeftLean (true);
						}
					} else {
						disableLean ();
					}
				}
			}
		}

		if (useEventsOnCameraStateChange) {
			cameraStateEventInfoIndex = cameraStateEventInfoList.FindIndex (s => s.Name == stateName);

			if (cameraStateEventInfoIndex > -1) {
				cameraStateEventInfoList [cameraStateEventInfoIndex].eventOnCameraStart.Invoke ();
			}
		}


		if (mainCamera.fieldOfView != lerpState.initialFovValue && isCameraTypeFree ()) {
			if (usingZoomOn) {
				setZoom (false);
			} else {
				setMainCameraFov (lerpState.initialFovValue, lerpState.fovTransitionSpeed);
			}
		}
	}

	public void setDefaultThirdPersonStateName (string newName)
	{
		defaultThirdPersonStateName = newName;
	}

	public void setDefaultFirstPersonStateName (string newName)
	{
		defaultFirstPersonStateName = newName;
	}

	public string getCurrentStateName ()
	{
		return currentStateName;
	}

	public void setCameraPositionMouseWheelEnabledState (bool state)
	{
		cameraPositionMouseWheelEnabled = state;
	}

	//if the player crouchs, move down the pivot
	public override void crouch (bool isCrouching)
	{
		//check if the camera has been moved away from the player, then the camera moves from its position to the crouch position
		//else the pivot also is moved, but with other parameters
		if (isCrouching) {
			if (isCameraTypeFree ()) {
				if (firstPersonActive) {
					setCameraState (defaultFirstPersonCrouchStateName);
				} else {
					setCameraState (defaultThirdPersonCrouchStateName);
				}
			}

			crouching = true;
		} else {
			if (isCameraTypeFree ()) {
				if (firstPersonActive) {
					setCameraState (defaultFirstPersonStateName);
				} else {
					setCameraState (defaultThirdPersonStateName);
				}
			}

			crouching = false;
		}
	}

	//move away the camera
	public void moveAwayCamera ()
	{
		//check that the player is not in the first person mode or the aim mode
		if (isCameraTypeFree () && !aimingInThirdPerson && !firstPersonActive && !playerControllerManager.isPlayerOnZeroGravityMode ()) {
			bool canMoveAway = false;

			//if the player is crouched, the pivot is also moved, the player get up, but with other parameters
			if (playerControllerManager.isCrouching ()) {
				playerControllerManager.crouch ();
			}

			//if the player can not get up due the place where he is, stops the move away action of the pivot
			if (!playerControllerManager.isCrouching ()) {
				canMoveAway = true;
			}

			if (canMoveAway) {
				if (currentStateName.Equals (defaultMoveCameraAwayStateName)) {
					setCameraState (defaultThirdPersonStateName);
				} else {
					setCameraState (defaultMoveCameraAwayStateName);
				}
			}
		}
	}

	public bool isCurrentCameraStateHeadTrackActive ()
	{
		return currentState.headTrackActive;
	}

	public void setMainCameraFov (float targetValue, float speed)
	{
		if (cameraFovStartAndEndCoroutine != null) {
			StopCoroutine (cameraFovStartAndEndCoroutine);
		}

		stopFovChangeCoroutine ();

		changeFovCoroutine = StartCoroutine (changeFovValue (targetValue, speed));
	}

	public void stopFovChangeCoroutine ()
	{
		if (changeFovCoroutine != null) {
			StopCoroutine (changeFovCoroutine);
		}
	}

	public IEnumerator changeFovValue (float targetValue, float speed)
	{
		fieldOfViewChanging = true;

		while (mainCamera.fieldOfView != targetValue) {
			mainCamera.fieldOfView = Mathf.MoveTowards (mainCamera.fieldOfView, targetValue, getCurrentDeltaTime () * speed);

			yield return null;
		}

		fieldOfViewChanging = false;
	}

	public override void setMainCameraFovStartAndEnd (float startTargetValue, float endTargetValue, float speed)
	{
		stopCameraFovStartAndEndCoroutine ();

		cameraFovStartAndEndCoroutine = StartCoroutine (changeFovValueStartAndEnd (startTargetValue, endTargetValue, speed));
	}

	public void stopCameraFovStartAndEndCoroutine ()
	{
		if (cameraFovStartAndEndCoroutine != null) {
			StopCoroutine (cameraFovStartAndEndCoroutine);
		}
	}

	public IEnumerator changeFovValueStartAndEnd (float startTargetValue, float endTargetValue, float speed)
	{
		while (mainCamera.fieldOfView != startTargetValue) {
			mainCamera.fieldOfView = Mathf.MoveTowards (mainCamera.fieldOfView, startTargetValue, getCurrentDeltaTime () * speed);

			yield return null;
		}

		while (mainCamera.fieldOfView != endTargetValue) {
			mainCamera.fieldOfView = Mathf.MoveTowards (mainCamera.fieldOfView, endTargetValue, getCurrentDeltaTime () * speed);

			yield return null;
		}
	}

	public bool isFOVChanging ()
	{
		return fieldOfViewChanging;
	}

	public void quickChangeFovValue (float targetValue)
	{
		mainCamera.fieldOfView = targetValue;
	}

	public void setOriginalCameraFov ()
	{
		setMainCameraFov (currentState.initialFovValue, zoomSpeed);
	}

	public float getMainCameraCurrentFov ()
	{
		return mainCamera.fieldOfView;
	}

	//set the zoom state
	public void setZoom (bool state)
	{
		if (!playerControllerManager.aimingInFirstPerson) {
			//to the fieldofview of the camera, it is added of substracted the zoomvalue
			usingZoomOn = state;

			float targetFov = currentState.minFovValue;
			float verticalRotationSpeedTarget = thirdPersonVerticalRotationSpeedZoomIn;
			float horizontalRotationSpeedTarget = thirdPersonHorizontalRotationSpeedZoomIn;

			if (firstPersonActive) {
				verticalRotationSpeedTarget = firstPersonVerticalRotationSpeedZoomIn;
				horizontalRotationSpeedTarget = firstPersonHorizontalRotationSpeedZoomIn;
			}

			if (!usingZoomOn) {
				if (firstPersonActive) {
					verticalRotationSpeedTarget = originalFirstPersonVerticalRotationSpeed;
					horizontalRotationSpeedTarget = originalFirstPersonHorizontalRotationSpeed;
				} else {
					verticalRotationSpeedTarget = originalThirdPersonVerticalRotationSpeed;
					horizontalRotationSpeedTarget = originalThirdPersonHorizontalRotationSpeed;
				}

				targetFov = currentState.initialFovValue;
			}

			setMainCameraFov (targetFov, zoomSpeed);
			//also, change the sensibility of the camera when the zoom is on or off, to control the camera properly
			changeRotationSpeedValue (verticalRotationSpeedTarget, horizontalRotationSpeedTarget);

			setScannerManagerState (state);

			if (weaponsManager.carryingWeaponInFirstPerson) {
				weaponsManager.changeWeaponsCameraFov (usingZoomOn, targetFov, zoomSpeed);
			}
		}
	}

	public void setZoomStateDuration (bool currentState, float duration, bool nextState)
	{
		if (zoomStateDurationCoroutine != null) {
			StopCoroutine (zoomStateDurationCoroutine);
		}

		zoomStateDurationCoroutine = StartCoroutine (setZoomStateDurationCoroutine (currentState, duration, nextState));
	}

	IEnumerator setZoomStateDurationCoroutine (bool currentState, float duration, bool nextState)
	{
		settings.zoomEnabled = currentState;

		yield return new WaitForSeconds (duration);

		setZoom (false);

		settings.zoomEnabled = nextState;
	}

	public void setZoomEnabledState (bool state)
	{
		settings.zoomEnabled = state;
	}

	public void disableZoom ()
	{
		if (usingZoomOn) {
			usingZoomOn = false;

			setOriginalRotationSpeed ();

			setScannerManagerState (false);
		}
	}

	public bool isUsingZoom ()
	{
		return usingZoomOn;
	}

	public void setUsingZoomOnValue (bool value)
	{
		usingZoomOn = value;

		setScannerManagerState (value);
	}

	public void setScannerManagerState (bool state)
	{
		if (scannerLocated) {
			scannerManager.enableOrDisableScannerZoom (state);
		}
	}

	public void changeRotationSpeedValue (float newVerticalRotationValue, float newHorizontalRotationValue)
	{
		if (firstPersonActive) {
			firstPersonVerticalRotationSpeed = newVerticalRotationValue;
			firstPersonHorizontalRotationSpeed = newHorizontalRotationValue;
		} else {
			thirdPersonVerticalRotationSpeed = newVerticalRotationValue;
			thirdPersonHorizontalRotationSpeed = newHorizontalRotationValue;
		}
	}

	public void setOriginalRotationSpeed ()
	{
		firstPersonVerticalRotationSpeed = originalFirstPersonVerticalRotationSpeed;
		firstPersonHorizontalRotationSpeed = originalFirstPersonHorizontalRotationSpeed;
		thirdPersonVerticalRotationSpeed = originalThirdPersonVerticalRotationSpeed;
		thirdPersonHorizontalRotationSpeed = originalThirdPersonHorizontalRotationSpeed;
	}

	//move away the camera when the player accelerates his movement velocity in the air, if the power of gravity is activated
	//once the player release shift, find a surface or stop in the air, the camera backs to its position
	//it is just to give the feeling of velocity
	public override void changeCameraFov (bool state)
	{
		if (pauseChangeCameraFovActive) {
			return;
		}

		if (settings.enableMoveAwayInAir) {
			if (playerControllerManager.aimingInFirstPerson || weaponsManager.isPlayerRunningWithNewFov ()) {
				return;
			}

			//print ("disable zoom when land on ground");
			usingZoomOff = state;

			float targetFov = currentState.maxFovValue;
			float targetSpeed = fovChangeSpeed;

			if (!usingZoomOff) {
				targetFov = currentState.initialFovValue;
			}

			if (usingZoomOn) {
				targetSpeed = zoomSpeed;

				if (weaponsManager.carryingWeaponInFirstPerson) {
					weaponsManager.changeWeaponsCameraFov (false, targetFov, targetSpeed);
				}
			}

			setMainCameraFov (targetFov, targetSpeed);

			disableZoom ();
		}
	}

	bool pauseChangeCameraFovActive;

	public void setPauseChangeCameraFovActiveState (bool state)
	{
		pauseChangeCameraFovActive = state;
	}

	//enable or disable the aim mode
	public void activateAiming ()
	{
		aimingInThirdPerson = true;

		if (isCameraTypeFree ()) {
			if (aimSide == sideToAim.Right) {
				if (useCustomThirdPersonAimActive) {
					setCameraState (customDefaultThirdPersonAimRightStateName);
				} else {
					setCameraState (defaultThirdPersonAimRightStateName);
				}
			} else {
				if (useCustomThirdPersonAimActive) {
					setCameraState (customDefaultThirdPersonAimLeftStateName);
				} else {
					setCameraState (defaultThirdPersonAimLeftStateName);
				}
			}
		} else {
			setCameraState (defaultLockedCameraStateName);

			if (!useCustomThirdPersonAimActivePaused) {
				useCustomThirdPersonAimActive = false;
			}
		}

		calibrateAccelerometer ();
	}

	public void deactivateAiming ()
	{
		aimingInThirdPerson = false;

		if (!useCustomThirdPersonAimActivePaused) {
			useCustomThirdPersonAimActive = false;
		}

		if (isCameraTypeFree ()) {
			setCameraState (defaultThirdPersonStateName);
		} else {
			setCameraState (defaultLockedCameraStateName);
		}
	}

	//change the aim side to left or right
	public void changeAimSide (int value)
	{
		if (value == 1) {
			if (useCustomThirdPersonAimActive) {
				setCameraState (customDefaultThirdPersonAimRightStateName);
			} else {
				setCameraState (defaultThirdPersonAimRightStateName);
			}
		} else {
			if (useCustomThirdPersonAimActive) {
				setCameraState (customDefaultThirdPersonAimLeftStateName);
			} else {
				setCameraState (defaultThirdPersonAimLeftStateName);
			}
		}
	}

	bool useCustomThirdPersonAimActivePaused;

	public void setUseCustomThirdPersonAimActivePausedState (bool state)
	{
		useCustomThirdPersonAimActivePaused = state;
	}

	public bool isUseCustomThirdPersonAimActivePaused ()
	{
		return useCustomThirdPersonAimActivePaused;
	}

	public void setUseCustomThirdPersonAimActiveState (bool state, string newCustomDefaultThirdPersonAimRightStateName, string newCustomDefaultThirdPersonAimLeftStateName)
	{
		useCustomThirdPersonAimActive = state;

		if (useCustomThirdPersonAimActive) {
			customDefaultThirdPersonAimRightStateName = newCustomDefaultThirdPersonAimRightStateName;
			customDefaultThirdPersonAimLeftStateName = newCustomDefaultThirdPersonAimLeftStateName;

			if (customDefaultThirdPersonAimRightStateName.Equals ("")) {
				customDefaultThirdPersonAimRightStateName = defaultThirdPersonAimRightStateName;
			}

			if (customDefaultThirdPersonAimLeftStateName.Equals ("")) {
				customDefaultThirdPersonAimLeftStateName = defaultThirdPersonAimLeftStateName;
			}
		}
	}

	//if the player is in the air, the camera can rotate 360 degrees, unlike when the player is in the ground where the rotation in x and y is limited
	public void onGroundOrOnAir (bool state)
	{
		onGround = state;

		if (!onGround) {
			adjustPivotAngle = true;
		}
	}

	void setShakingCameraActiveState (bool state)
	{
		if (state) {
			if (currentState.ignoreCameraShakeOnRunState) {
				return;
			}
		}

		settings.shakingCameraActive = state;
	}

	public override void setShakeCameraState (bool state, string stateName)
	{
		setShakingCameraActiveState (state);

		currentCameraShakeStateName = stateName;
	}

	//set the shake of the camera when the player moves in the air
	public void startShakeCamera ()
	{
		setShakingCameraActiveState (true);

		currentCameraShakeStateName = "Shaking";
	}

	public override void stopShakeCamera ()
	{
		setShakingCameraActiveState (false);

		settings.accelerateShaking = false;

		headBobManager.setDefaultState ();
	}

	public bool isCameraShaking ()
	{
		return settings.shakingCameraActive;
	}

	//now this funcion is here so it can be called by keyboard or touch button
	public void accelerateShake (bool value)
	{
		settings.accelerateShaking = value;
	}


	//FUNCTIONS TO CHANGE THE CAMERA VIEW FROM FIRST TO THIRD AND VICEVERSA, TRUE = FIRST PERSON, FALSE = FIRST PERSON
	public void changeCameraToThirdOrFirstView (bool state)
	{
		if (state) {
			if (!firstPersonActive) {
				changeCameraToThirdOrFirstView ();
			}
		} else {
			if (firstPersonActive) {
				changeCameraToThirdOrFirstView ();
			}
		}
	}

	public void changeCameraToThirdOrFirstView ()
	{
		//in the normal mode, change camera from third to first and viceversa
		if (!playerControllerManager.isPlayerAimingInThirdPerson () && (!scannerLocated || !scannerManager.isScannerActivated ())) {
			changeCameraView ();

			if (!firstPersonActive) {
				if (scannerLocated) {
					if (scannerManager.isScannerActivated ()) {
						scannerManager.disableScanner ();
					}
				}
			}
		}
	}

	public void changeCameraView ()
	{
		firstPersonActive = !firstPersonActive;

		if (firstPersonActive) {
			activateFirstPersonCamera ();
		} else {
			deactivateFirstPersonCamera ();
		}

		setFirstOrThirdHeadBobView (firstPersonActive);

		playerControllerManager.setLastTimeMoved ();

		powersManager.changeCameraToThirdOrFirstView (firstPersonActive);

		gravityManager.changeCameraView (firstPersonActive);

		weaponsManager.setCurrentWeaponsParent (firstPersonActive);
	}

	public void changeTypeView ()
	{
		//in the aim mode, the player can choose which side to aim, left or right
		changeAimSide ();

		changeCameraToThirdOrFirstView ();
	}

	void changeAimSide ()
	{
		if (changeCameraSideActive &&
		    playerControllerManager.isPlayerAimingInThirdPerson () &&
		    !checkToKeepAfterAimingFromShooting &&
		    !aimingFromShooting &&
		    !firstPersonActive) {

			setAimModeSide (true);
		}
	}

	public void setAimModeSide (bool state)
	{
		int value;

		if (state) {
			value = (int)aimSide * (-1);
		} else {
			value = (int)aimSide;
		}

		//change to the right side, enabling the right arm
		if (value == 1) {
			aimSide = sideToAim.Right;
		}
		//change to the left side, enabling the left arm
		else {
			aimSide = sideToAim.Left;
		}

		if (state) {
			changeAimSide (value);
		}

		powersManager.setAimModeSide (value == 1);
	}

	//set first and third person camera position
	public void activateFirstPersonCamera ()
	{
		firstPersonActive = true;

		if (crouching) {
			setCameraState (defaultFirstPersonCrouchStateName);
		} else {
			setCameraState (defaultFirstPersonStateName);
		}

		updatePlayerStatesToView ();

		updateReticleActive ();
	}

	public void deactivateFirstPersonCamera ()
	{
		firstPersonActive = false;

		if (crouching) {
			setCameraState (defaultThirdPersonCrouchStateName);
		} else {
			setCameraState (defaultThirdPersonStateName);
		}

		updatePlayerStatesToView ();

		updateReticleActive ();
	}

	public void activateFirstPersonCameraEditor ()
	{
		changingCameraViewOnEditor = true;

		activateFirstPersonCamera ();

		changingCameraViewOnEditor = false;
	}

	public void deactivateFirstPersonCameraEditor ()
	{
		changingCameraViewOnEditor = true;

		deactivateFirstPersonCamera ();

		changingCameraViewOnEditor = false;
	}

	bool changingCameraViewOnEditor;

	public void updatePlayerStatesToView ()
	{
		//check if in first person the animator is used, else the third person is enabled, so enable again the animator if it was disabled 
		playerControllerManager.checkAnimatorIsEnabled (firstPersonActive);	

		checkCharacterStateIconForViewChange ();

		playerControllerManager.setFootStepManagerState (firstPersonActive);

		bool isPlayerOnGround = false;

		if (!changingCameraViewOnEditor) {
			if (grabObjectsManager != null) {
				grabObjectsManager.checkPhysicalObjectGrabbedPosition (firstPersonActive);
			}

			isPlayerOnGround = playerControllerManager.checkIfPlayerOnGroundWithRaycast ();

			playerControllerManager.setOnGroundState (isPlayerOnGround);

			playerControllerManager.setOnGroundAnimatorIDValueWithoutCheck (isPlayerOnGround);
		}

		playerControllerManager.setFirstPersonViewActiveState (firstPersonActive);


		playerControllerManager.setCharacterMeshGameObjectState (!firstPersonActive);

		if (useEventOnThirdFirstPersonViewChange) {
			if (firstPersonActive) {
				setFirstPersonEvent.Invoke ();
			} else {
				setThirdPersonEvent.Invoke ();	
			}
		}

		if (!useCustomThirdPersonAimActivePaused) {
			useCustomThirdPersonAimActive = false;
		}

		if (!changingCameraViewOnEditor) {
			playerControllerManager.setOnGroundState (isPlayerOnGround);

			playerControllerManager.setOnGroundAnimatorIDValueWithoutCheck (isPlayerOnGround);
		}
	}

	public bool isChangeCameraViewEnabled ()
	{
		return changeCameraViewEnabled && !pausePlayerCameraViewChange;
	}

	public bool isPausePlayerCameraViewChangeActive ()
	{
		return pausePlayerCameraViewChange;
	}

	public void enableOrDisableAllChangeCameraEnabledView (bool state)
	{
		enableOrDisableChangeCameraView (state);

		setMoveCameraPositionWithMouseWheelActiveState (state);
	}

	public void setOriginalAllChangeCameraViewEnabledState ()
	{
		setOriginalchangeCameraViewEnabledValue ();

		setOriginalMoveCameraPositionWithMouseWheelActiveState ();
	}

	public void enableOrDisableChangeCameraView (bool state)
	{
		changeCameraViewEnabled = state;
	}

	public void setOriginalchangeCameraViewEnabledValue ()
	{
		enableOrDisableChangeCameraView (originalChangeCameraViewEnabled);
	}

	public void setPausePlayerCameraViewChangeState (bool state)
	{
		pausePlayerCameraViewChange = state;
	}

	public void checkCharacterStateIconForViewChange ()
	{
		if (characterStateIconManager != null) {
			characterStateIconManager.checkCharacterStateIconForViewChange ();
		}
	}

	//stop the camera rotation or the camera collision detection
	public void changeCameraRotationState (bool state)
	{
		cameraCanRotate = state;
	}

	public void pauseOrPlayCamera (bool state)
	{
		if (state) {
			setLastTimeCameraRotated ();
		}

		cameraCanBeUsed = state;
	}

	public bool cameraCanBeUsedActive ()
	{
		return cameraCanBeUsed;
	}

	public void setCameraRotationInputEnabled (bool state)
	{
		cameraRotationInputEnabled = state;
	}

	public void enableCameraRotationInput ()
	{
		cameraRotationInputEnabled = true;
	}

	public void disableCameraRotationInput ()
	{
		cameraRotationInputEnabled = false;
	}

	public void setCameraActionsInputEnabledState (bool state)
	{
		cameraActionsInputEnabled = state;
	}

	public void enableCameraActionsInput ()
	{
		cameraActionsInputEnabled = true;
	}

	public void disableCameraActionsInput ()
	{
		cameraActionsInputEnabled = false;
	}

	//calibrate the initial accelerometer input according to how the player is holding the touch device
	public void calibrateAccelerometer ()
	{
		if (settings.useAcelerometer) {
			Vector3 wantedDeadZone = Input.acceleration;
			Quaternion rotateQuaternion = Quaternion.FromToRotation (new Vector3 (1, 0, 0), wantedDeadZone);
			//create identity matrix
			Matrix4x4 matrix = Matrix4x4.TRS (vector3Zero, rotateQuaternion, Vector3.one);
			//get the inverse of the matrix
			calibrationMatrix = matrix.inverse;
		}
	}

	//get the accelerometer value, taking in account that the device is holing in left scape mode, with the home button in the right side
	Vector3 getAccelerometer (Vector3 accelerator)
	{
		Vector3 accel = calibrationMatrix.MultiplyVector (accelerator);

		return accel;
	}

	//get the accelerometer value more smoothly
	Vector3 lowpass ()
	{
		float LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds; // tweakable
		lowPassValue = Vector3.Lerp (lowPassValue, getAccelerometer (Input.acceleration), LowPassFilterFactor);

		return lowPassValue;
	}

	//get the scroller value in the touch options menu if the player enables or disables the accelerometer
	public void setUseAcelerometerState (bool state)
	{
		settings.useAcelerometer = state;
	}

	public bool isCameraRotating ()
	{
		return isMoving;
	}

	public bool isPlayerAiming ()
	{
		return playerAiming;
	}

	public void setLastTimeMoved ()
	{
		lastTimeMoved = Time.time;
	}

	public override float getLastTimeMoved ()
	{
		return lastTimeMoved;
	}

	public void setLastTimeCameraRotated ()
	{
		lastTimeCameraRotated = Time.time;
	}

	public void playOrPauseHeadBob (bool state)
	{
		headBobManager.playOrPauseHeadBob (state);
	}

	public void stopAllHeadbobMovements ()
	{
		headBobManager.stopAllHeadbobMovements ();
	}

	public void pauseHeadBodWithDelay (float delayAmount)
	{
		headBobManager.pauseHeadBodWithDelay (delayAmount);
	}

	public headBob getHeadBobManager ()
	{
		return headBobManager;
	}

	public void setFirstOrThirdHeadBobView (bool state)
	{
		headBobManager.setFirstOrThirdHeadBobView (state);
	}

	public void setHeadbobState (string headBobStateName)
	{
		headBobManager.setState (headBobStateName);
	}

	public void setDefaultHeadbobState ()
	{
		headBobManager.setDefaultState ();
	}

	public override float getOriginalCameraFov ()
	{
		return currentState.initialFovValue;
	}

	public bool isCameraPlacedInFirstPerson ()
	{
		float currentCameraDistance = GKC_Utils.distance (mainCameraTransform.position, pivotCameraTransform.position);

		if (currentCameraDistance < 0.01f) {
			return true;
		} else {
			return false;
		}
	}

	public override bool isFirstPersonActive ()
	{
		return firstPersonActive;
	}

	public bool isCameraTypeFree ()
	{
		return cameraType == typeOfCamera.free;
	}

	public void setLookAngleValue (Vector2 newValue)
	{
		lookAngle = newValue;
	}

	public void resetMainCameraTransformLocalPosition ()
	{
		mainCameraTransform.localPosition = lerpState.camPositionOffset;
	}

	public void resetPivotCameraTransformLocalPosition ()
	{
		pivotCameraTransform.localPosition = lerpState.pivotPositionOffset;
	}

	public void resetCurrentCameraStateAtOnce ()
	{
		slerpCameraStateAtOnce (currentState, lerpState);
	}

	public void configureCurrentLerpState (Vector3 pivotOffset, Vector3 cameraOffset)
	{
		lerpState.camPositionOffset = cameraOffset;
		lerpState.pivotPositionOffset = pivotOffset;
	}

	public void configureCameraAndPivotPositionAtOnce ()
	{
		pivotCameraTransform.localPosition = currentState.pivotPositionOffset;
		mainCameraTransform.localPosition = currentState.camPositionOffset;
	}

	public void setCurrentCameraUpRotationValue (float newRotation)
	{
		currentCameraUpRotation = newRotation;
	}

	//AI Input
	public void Rotate (Vector3 currentLookPost)
	{
		navMeshCurrentLookPos = currentLookPost;
	}

	public void setResetVerticalCameraRotationActiveState (bool state)
	{
		resetVerticalCameraRotationActive = state;
	}

	public void setPlayerNavMeshEnabledState (bool state)
	{
		playerNavMeshEnabled = state;
	}

	public void startOverride ()
	{
		overrideTurretControlState (true);
	}

	public void stopOverride ()
	{
		overrideTurretControlState (false);
	}

	public void overrideTurretControlState (bool state)
	{
		usedByAI = !state;

		if (usedByAI) {
			setOverrideCameraPosition (usedByAI);
		}
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	public void enableOrDisableMainCamera (bool state)
	{
		mainCamera.enabled = state;
	}

	public void enableAICamera ()
	{
		enableOrDisableAICamera (true);
	}

	public void disableAICamera ()
	{
		enableOrDisableAICamera (false);
	}

	public void enableOrDisableAICamera (bool state)
	{
		if (state && isFirstPersonActive ()) {
			changeCameraToThirdOrFirstView (false);
		}

		mainCamera.enabled = !state;

		enableOrDisableCameraAudioListener (!state);

		if (state) {
			setCameraState (defaultAICameraStateName);

			resetCurrentCameraStateAtOnce ();

			configureCameraAndPivotPositionAtOnce ();
		} else {
			//			setCameraState (defaultThirdPersonStateName);
			if (setNewCharacterAlwaysInPreviousCharacterView) {
				if (previousCharacterControlInFirstPersonView) {
					if (!isFirstPersonActive ()) {
						changeCameraToThirdOrFirstView (true);
					}
				} else {
					if (isFirstPersonActive ()) {
						changeCameraToThirdOrFirstView (false);
					} else {
						setCameraState (defaultThirdPersonStateName);
					}
				}
			} else {
				if (setNewCharacterAlwaysInThirdPerson) {
					if (isFirstPersonActive ()) {
						changeCameraToThirdOrFirstView (false);
					} else {
						setCameraState (defaultThirdPersonStateName);
					}
				}

				if (setNewCharacterAlwaysInFirstPerson) {
					if (!isFirstPersonActive ()) {
						changeCameraToThirdOrFirstView (true);
					}
				}
			}

			if (adjustCameraToPreviousCharacterDirectionActive) {
				if (previousCharacterControl != null && previousCharacterControl != playerControllerGameObject) {
					adjustCameraToPreviousCharacter ();
				} 
			}

			if (useSmoothCameraTransitionBetweenCharacters) {
				if (previousCharacterControl != null && previousCharacterControl != playerControllerGameObject) {

					adjustCameraToPreviousCharacterSmoothly ();
				}
			}
		}
	}

	public void enableOrDisableCameraAudioListener (bool state)
	{
		AudioListener currentAudioListener = mainCamera.GetComponent<AudioListener> ();

		if (currentAudioListener != null) {
			currentAudioListener.enabled = state;
		}
	}

	public Vector2 getScreenCenter ()
	{
		if (usingScreenSpaceCamera) {

			float centerX = Screen.width / 2;

			centerX *= mainCamera.rect.width;

			centerX += Screen.width * mainCamera.rect.x;


			float centerY = Screen.height / 2;

			centerY *= mainCamera.rect.height;

			centerY += Screen.height * mainCamera.rect.y;

//			print ("screen center values " + mainCamera.rect.x + " " + mainCamera.rect.y + " " + Screen.width + " " + Screen.height);
//
//			print (centerX + " " + centerY);

			return new Vector2 (centerX, centerY);
		} else {
			return new Vector2 (Screen.width / 2, Screen.height / 2);
		}
	}

	public void adjustCameraToPreviousCharacter ()
	{
		pauseOrPlayCamera (false);

		changeCameraRotationState (false);

		playerCamera previousCharacterPlayerCameraManager = previousCharacterControl.GetComponent<playerController> ().getPlayerCameraManager ();

		Quaternion newRotation = quaternionIdentity;
		playerCameraTransform.rotation = previousCharacterPlayerCameraManager.transform.rotation;
		newRotation = previousCharacterPlayerCameraManager.getPivotCameraTransform ().localRotation;

		pivotCameraTransform.localRotation = newRotation;

		float newLookAngleValue = newRotation.eulerAngles.x;
		if (newLookAngleValue > 180) {
			newLookAngleValue -= 360;
		}

		setLookAngleValue (new Vector2 (0, newLookAngleValue));

		if (!useSmoothCameraTransitionBetweenCharacters) {
			resetCurrentCameraStateAtOnce ();

			configureCameraAndPivotPositionAtOnce ();
		}

		changeCameraRotationState (true);

		pauseOrPlayCamera (true);
	}

	public void setPreviousCharacterControl (GameObject newPpreviousCharacterControl, Vector3 cameraPosition, Quaternion cameraRotation, bool previousCharacterControlInFirstPersonViewValue)
	{
		previousCharacterControl = newPpreviousCharacterControl;
		previousCharacterControlCameraPosition = cameraPosition;
		previousCharacterControlCameraRotation = cameraRotation;
		previousCharacterControlInFirstPersonView = previousCharacterControlInFirstPersonViewValue;
	}

	public void adjustCameraToPreviousCharacterSmoothly ()
	{
		if (adjustCameraCharacterSmoothlyCoroutine != null) {
			StopCoroutine (adjustCameraCharacterSmoothlyCoroutine);
		}

		adjustCameraCharacterSmoothlyCoroutine = StartCoroutine (adjustCameraToPreviousCharacterSmoothlyCoroutine ());
	}

	IEnumerator adjustCameraToPreviousCharacterSmoothlyCoroutine ()
	{
		pauseOrPlayCamera (false);

		changeCameraRotationState (false);

		mainCamera.transform.SetParent (null);

		resetCurrentCameraStateAtOnce ();

		configureCameraAndPivotPositionAtOnce ();

		mainCamera.transform.SetParent (mainCameraTransform);

		mainCamera.transform.position = previousCharacterControlCameraPosition;
		mainCamera.transform.rotation = previousCharacterControlCameraRotation;

		bool targetReached = false;

		float angleDifference = 0;

		float movementTimer = 0;

		float dist = GKC_Utils.distance (mainCamera.transform.localPosition, vector3Zero);

		float duration = dist / smoothCameraTransitionBetweenCharactersSpeed;

		float t = 0;

		Vector3 targetPosition = vector3Zero;
		Quaternion targetRotation = quaternionIdentity;

		while (!targetReached) {
			float deltaTimeValue = getCurrentDeltaTime ();

			t += deltaTimeValue / duration; 

			mainCamera.transform.localPosition = Vector3.Slerp (mainCamera.transform.localPosition, targetPosition, t);
			mainCamera.transform.localRotation = Quaternion.Slerp (mainCamera.transform.localRotation, targetRotation, t);

			angleDifference = Quaternion.Angle (mainCamera.transform.localRotation, targetRotation);

			movementTimer += deltaTimeValue;

			if ((GKC_Utils.distance (mainCamera.transform.localPosition, targetPosition) < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
				targetReached = true;
			}

			yield return null;
		}

		changeCameraRotationState (true);

		pauseOrPlayCamera (true);
	}

	public void setOverrideCameraPosition (bool state)
	{
		int defaultStateIndex = -1;

		for (int i = 0; i < playerCameraStates.Count; i++) {
			if (playerCameraStates [i].Name.Equals (defaultStateName)) {
				defaultStateIndex = i;
			}
		}

		if (state) {
			playerCameraStates [defaultStateIndex].camPositionOffset = offsetInitialPosition; 
		} else {
			playerCameraStates [defaultStateIndex].camPositionOffset = cameraInitialPosition;
		}

		setCameraState (defaultStateName);

		mainCameraTransform.localPosition = playerCameraStates [defaultStateIndex].camPositionOffset;
	}

	public void setExtraRotationToCamera ()
	{
		if (extraRotationCoroutine != null) {
			StopCoroutine (extraRotationCoroutine);
		}

		extraRotationCoroutine = StartCoroutine (setExtraRotationToCameraCoroutine ());
	}

	IEnumerator setExtraRotationToCameraCoroutine ()
	{
		pauseOrPlayCamera (false);

		changeCameraRotationState (false);

		bool targetReached = false;

		float t = 0;

		float movementTimer = 0;

		Vector3 targetEuler = vector3Zero;

		Vector3 targetDirection = vector3Zero;

		if (useCameraForwardDirection) {
			targetEuler = playerCameraTransform.eulerAngles + (playerCameraTransform.up * extraCameraRotationAmount);

			targetDirection = Quaternion.AngleAxis (extraCameraRotationAmount, playerCameraTransform.up) * playerCameraTransform.forward;
		} else {
			targetEuler = playerControllerGameObject.transform.eulerAngles + (playerControllerGameObject.transform.up * extraCameraRotationAmount);

			targetDirection = Quaternion.AngleAxis (extraCameraRotationAmount, playerCameraTransform.up) * playerControllerGameObject.transform.forward;
		}

		Quaternion targetRotation = Quaternion.Euler (targetEuler);

		lookAngle.y = 0;

		while (!targetReached) {
			float deltaTimeValue = getCurrentDeltaTime ();

			t += deltaTimeValue * extraCameraRotationSpeed; 

			pivotCameraTransform.localRotation = Quaternion.Slerp (pivotCameraTransform.localRotation, quaternionIdentity, t);
			playerCameraTransform.rotation = Quaternion.Slerp (playerCameraTransform.rotation, targetRotation, t);

			float pivotCameraTransformAngle = Vector3.SignedAngle (pivotCameraTransform.forward, playerCameraTransform.forward, playerCameraTransform.right);

			float cameraTransformAngle = Vector3.SignedAngle (playerCameraTransform.forward, targetDirection, playerCameraTransform.up);

			if ((Math.Abs (pivotCameraTransformAngle) < 1 && Math.Abs (cameraTransformAngle) < 1) || movementTimer > 4) {
				resetingCameraActive = false;

				targetReached = true;
			}

			setLastTimeCameraRotated ();

			movementTimer += deltaTimeValue;

			yield return null;
		}

		changeCameraRotationState (true);

		pauseOrPlayCamera (true);
	}

	public void setZeroGravityModeOnState (bool state)
	{
		zeroGravityModeOn = state;
	}

	public void setfreeFloatingModeOnState (bool state)
	{
		freeFloatingModeOn = state;
	}

	public void setForwardRotationValue (float value)
	{
		targetForwardRotationAngle = value;
	}

	public Vector2 getIconPosition (Vector3 worldObjectPosition)
	{
		iconPositionViewPoint = mainCamera.WorldToViewportPoint (worldObjectPosition);
		iconPosition2d = new Vector2 ((iconPositionViewPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (iconPositionViewPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

		return iconPosition2d;
	}

	public void setLockedCameraRotationActiveToLeft (bool holdingButton)
	{
		if (currentLockedCameraAxisInfo.useFixedRotationAmount) {
			if (!rotatingLockedCameraFixedRotationAmountActive) {
				stopRotateLockedCameraFixedRotationAmount ();

				rotateLockedCameraFixedAmountCoroutine = StartCoroutine (rotateLockedCameraFixedRotationAmount (false));
			}
		} else {
			rotatingLockedCameraToLeft = holdingButton;
			lockedCameraRotationDirection = -1;

			if (!rotatingLockedCameraToLeft && rotatingLockedCameraToRight) {
				lockedCameraRotationDirection = 1;
			}
		}
	}

	public void setLockedCameraRotationActiveToRight (bool holdingButton)
	{
		if (currentLockedCameraAxisInfo.useFixedRotationAmount) {
			if (!rotatingLockedCameraFixedRotationAmountActive) {
				stopRotateLockedCameraFixedRotationAmount ();

				rotateLockedCameraFixedAmountCoroutine = StartCoroutine (rotateLockedCameraFixedRotationAmount (true));
			}
		} else {
			rotatingLockedCameraToRight = holdingButton;
			lockedCameraRotationDirection = 1;

			if (!rotatingLockedCameraToRight && rotatingLockedCameraToLeft) {
				lockedCameraRotationDirection = -1;
			}
		}
	}

	public void stopRotateLockedCameraFixedRotationAmount ()
	{
		rotatingLockedCameraFixedRotationAmountActive = false;

		if (rotateLockedCameraFixedAmountCoroutine != null) {
			StopCoroutine (rotateLockedCameraFixedAmountCoroutine);
		}
	}

	IEnumerator rotateLockedCameraFixedRotationAmount (bool rotateToRight)
	{
		rotatingLockedCameraFixedRotationAmountActive = true;

		float rotationAmount = currentLockedCameraAxisInfo.fixedRotationAmountToLeft;

		if (rotateToRight) {
			rotationAmount = currentLockedCameraAxisInfo.fixedRotationAmountToRight;
		}

		Vector3 targetRotationEuler = lockedMainCameraTransform.up * rotationAmount;

		targetRotationEuler += lockedMainCameraTransform.eulerAngles;

		Quaternion targetRotation = Quaternion.Euler (targetRotationEuler);

		bool targetReached = false;

		float t = 0;

		float movementTimer = 0;

		float angleDifference = 0;

		float duration = Math.Abs (Quaternion.Angle (lockedMainCameraTransform.rotation, targetRotation)) / currentLockedCameraAxisInfo.fixedRotationAmountSpeed;

		while (!targetReached) {
			float deltaTimeValue = getCurrentDeltaTime ();

			t += deltaTimeValue / duration; 

			lockedMainCameraTransform.rotation = Quaternion.Lerp (lockedMainCameraTransform.rotation, targetRotation, t);
			lockedCameraPivot.rotation = Quaternion.Lerp (lockedCameraPivot.rotation, targetRotation, t);

			movementTimer += deltaTimeValue;

			angleDifference = Quaternion.Angle (lockedMainCameraTransform.rotation, targetRotation);

			if (angleDifference < 0.2 || movementTimer > (duration + 2)) {
				targetReached = true;
			}

			yield return null;
		}

		rotatingLockedCameraFixedRotationAmountActive = false;
	}

	public void setCanActivateLookAtTargetEnabledState (bool state)
	{
		canActivateLookAtTargetEnabled = state;
	}

	public void setMoveCameraPositionWithMouseWheelActiveState (bool state)
	{
		moveCameraPositionWithMouseWheelActive = state;
	}

	public void setOriginalMoveCameraPositionWithMouseWheelActiveState ()
	{
		setMoveCameraPositionWithMouseWheelActiveState (originalMoveCameraPositionWithMouseWheelActive);
	}


	public bool reverseMouseWheelDirectionEnabled;

	public void moveCameraCloserOrFartherFromPlayer (bool movingDirection)
	{
		if (moveCameraPositionWithMouseWheelActive && cameraPositionMouseWheelEnabled) {
			if (isCameraTypeFree ()) {
				if (cameraCanBeUsed) {

					if (grabObjectsManager != null) {
						if (grabObjectsManager.isPauseCameraMouseWheelWhileObjectGrabbedActive () && grabObjectsManager.isGrabbedObject ()) {
							if (!grabObjectsManager.isCarryingPhysicalObject ()) {
								return;
							}
						} else {
							grabObjectsManager.dropObject ();
						}
					}

					if (reverseMouseWheelDirectionEnabled) {
						movingDirection = !movingDirection;
					}

					if (useCameraMouseWheelStates) {
						if (movingDirection) {
							lerpState.camPositionOffset.z -= moveCameraPositionBackwardWithMouseWheelSpeed;
							lerpState.camPositionOffset.z = Mathf.Clamp (lerpState.camPositionOffset.z, lerpState.originalCamPositionOffset.z - maxExtraDistanceOnThirdPerson, 0);

						} else {
							lerpState.camPositionOffset.z += moveCameraPositionForwardWithMouseWheelSpeed;
							lerpState.camPositionOffset.z = Mathf.Clamp (lerpState.camPositionOffset.z, lerpState.originalCamPositionOffset.z - maxExtraDistanceOnThirdPerson, 0);
						}

						float currentCameraDistance = Math.Abs (lerpState.camPositionOffset.z);

						int currentWheelMouseStateIndex = -1;

						bool mouseWheelCameraChangedCorrectly = false;

						for (int i = 0; i < cameraMouseWheelStatesList.Count; i++) {
							if (currentWheelMouseStateIndex == -1) {
								if (currentCameraDistance >= cameraMouseWheelStatesList [i].cameraDistanceRange.x && currentCameraDistance <= cameraMouseWheelStatesList [i].cameraDistanceRange.y) {
									if (!cameraMouseWheelStatesList [i].isCurrentCameraState) {
										if (updateCurrentMousWheelCameraState (i)) {

											currentWheelMouseStateIndex = i;

											mouseWheelCameraChangedCorrectly = true;
										}
									} else {
										if (cameraMouseWheelStatesList [i].changeCameraIfDistanceChanged) {
											if (currentCameraDistance >= cameraMouseWheelStatesList [i].minCameraDistanceToChange) {
												

												if (cameraMouseWheelStatesList [i].changeToAboveCameraState) {
													if (updateCurrentMousWheelCameraState (i - 1)) {

														currentWheelMouseStateIndex = (i - 1);

														mouseWheelCameraChangedCorrectly = true;
													}
												} else {
													if (updateCurrentMousWheelCameraState (i + 1)) {

														currentWheelMouseStateIndex = (i + 1);

														mouseWheelCameraChangedCorrectly = true;
													}
												}

												if (mouseWheelCameraChangedCorrectly) {
													cameraMouseWheelStatesList [i].isCurrentCameraState = false;
												}
											}
										}
									}
								} else {
									if (mouseWheelCameraChangedCorrectly) {
										cameraMouseWheelStatesList [i].isCurrentCameraState = false;
									}
								}
							}
						}

						if (currentWheelMouseStateIndex != -1) {
							cameraMouseWheelStatesList [currentWheelMouseStateIndex].isCurrentCameraState = true;
						}
					} else {
						if (movingDirection) {
							lerpState.camPositionOffset.z -= moveCameraPositionBackwardWithMouseWheelSpeed;
							lerpState.camPositionOffset.z = Mathf.Clamp (lerpState.camPositionOffset.z, lerpState.originalCamPositionOffset.z - maxExtraDistanceOnThirdPerson, 0);

							if (firstPersonActive) {
								changeCameraToThirdOrFirstView ();
							}
						} else {
							lerpState.camPositionOffset.z += moveCameraPositionForwardWithMouseWheelSpeed;
							lerpState.camPositionOffset.z = Mathf.Clamp (lerpState.camPositionOffset.z, lerpState.originalCamPositionOffset.z, 0);

							float currentCameraDistance = Math.Abs (lerpState.camPositionOffset.z);

							if (!firstPersonActive && changeCameraViewEnabled && !pausePlayerCameraViewChange) {
								if (currentCameraDistance <= minDistanceToChangeToFirstPerson) {
									changeCameraToThirdOrFirstView ();
								}
							}
						}
					}
				}
			}
		}
	}

	public void disableStrafeModeActivateFromNoTargetsFoundActive ()
	{
		strafeModeActivateFromNoTargetsFoundActive = false;
	}

	public bool playerIsBusy ()
	{
		if (playerControllerManager.isUsingDevice ()) {
			return true;
		}

		if (playerControllerManager.isUsingSubMenu ()) {
			return true;
		}

		if (playerControllerManager.isPlayerMenuActive ()) {
			return true;
		}

		return false;
	}

	public bool changeCameraSideActive = true;

	bool checkToKeepAfterAimingFromShooting;

	public void setCheckToKeepAfterAimingFromShootingState (bool state)
	{
		checkToKeepAfterAimingFromShooting = state;
	}

	bool aimingFromShooting;

	public void setAimingFromShootingState (bool state)
	{
		aimingFromShooting = state;
	}

	public bool isTemporalCameraViewToLockedCameraActive ()
	{
		return temporalCameraViewToLockedCameraActive;
	}

	bool temporalCameraViewToLockedCameraActive;

	bool activatingLockedCameraByInputActive;

	//CALL INPUT FUNCTIONS
	public void inputChangeCameraView ()
	{
		if (!playerIsBusy () && playerControllerManager.canPlayerMove ()) {
			bool canChangeCameraOnLockedView = false;

			if (changeCameraViewEnabled) {
				if (weaponsManager.isEditinWeaponAttachments ()) {
					return;
				}

				if (isCameraTypeFree () && !temporalCameraViewToLockedCameraActive) {
					changeTypeView ();
				} else {
					canChangeCameraOnLockedView = true;
				}
			} else {
				if (!isCameraTypeFree () || temporalCameraViewToLockedCameraActive) {
					canChangeCameraOnLockedView = true;
				}

				if (isCameraTypeFree () && !temporalCameraViewToLockedCameraActive) {
					changeAimSide ();
				}
			}

			if (canChangeCameraOnLockedView) {
				if (currentLockedCameraAxisInfo != null) {
					if (currentLockedCameraAxisInfo.canChangeCameraToFreeViewByInput) {

						if (!temporalCameraViewToLockedCameraActive) {
							setCameraToFreeOrLocked (typeOfCamera.free, currentLockedCameraAxisInfo);

							temporalCameraViewToLockedCameraActive = true;
						} else {
							temporalCameraViewToLockedCameraActive = false;

							activatingLockedCameraByInputActive = true;

							setCameraToFreeOrLocked (typeOfCamera.locked, currentLockedCameraAxisInfo);

							activatingLockedCameraByInputActive = false;
						}
					}
				}
			}
		}
	}

	public void inputToogleCameraRotationInputState ()
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		cameraRotationInputEnabled = !cameraRotationInputEnabled;
	}

	public void inputMoveCamerAway ()
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (isCameraTypeFree ()) {
			if (cameraCanBeUsed) {
				if (settings.moveAwayCameraEnabled) {
					moveAwayCamera ();
				}
			}
		}
	}

	public void inputLookAtTarget ()
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (lookAtTargetEnabled && canActivateLookAtTargetEnabled && cameraCanBeUsed) {
			bool freeCameraActive = isCameraTypeFree ();

			if (freeCameraActive) {
				setLookAtTargetStateInput (!lookingAtTarget);

				return;
			} 


			if (!freeCameraActive) {
				if (canActiveLookAtTargetOnLockedCamera && !using2_5ViewActive && currentLockedCameraAxisInfo.canUseManualLockOn) {
					if (searchingToTargetOnLockedCamera) {
						if (targetToLook != null && currentLockedCameraAxisInfo.showCameraCursorWhenNotAiming) {

							settingShowCameraCursorWhenNotAimingBackToActive = true;

//							print ("back to show camera cursor");
							activateShowCameraCursorWhenNotAimingState ();


							settingShowCameraCursorWhenNotAimingBackToActive = false;
						} else {
		
							setLookAtTargetStateInput (true);

							if (canActiveLookAtTargetOnLockedCamera) {
								searchingToTargetOnLockedCamera = false;
							}

							if (currentLockedCameraAxisInfo.showCameraCursorWhenNotAiming) {
								bool resumeShowCameraCursorWhenNotAiming = false;

								if (targetToLook == null) {
									resumeShowCameraCursorWhenNotAiming = true;
								}

								if (resumeShowCameraCursorWhenNotAiming) {
									activateShowCameraCursorWhenNotAimingState ();
								} else {
									setLookAtTargetOnLockedCameraState ();
								}
							} 
						}
					} else {
						if (targetToLook == null) {
							activeLookAtTargetOnLockedCamera = true;
						} else {
							activeLookAtTargetOnLockedCamera = false;
						}

						if (currentLockedCameraAxisInfo.showCameraCursorWhenNotAiming) {
							bool resumeShowCameraCursorWhenNotAiming = false;

							if (activeLookAtTargetOnLockedCamera) {
								setLookAtTargetStateInput (true);
							} else {
	
								resumeShowCameraCursorWhenNotAiming = true;
							}

							activeLookAtTargetOnLockedCamera = false;

							if (targetToLook == null) {
								resumeShowCameraCursorWhenNotAiming = true;
							}

							if (resumeShowCameraCursorWhenNotAiming) {
								activateShowCameraCursorWhenNotAimingState ();
							}
						} else {
							if (activeLookAtTargetOnLockedCamera) {
								setLookAtTargetStateInput (!lookingAtTarget);
							} else {
								setTargetToLook (null);

								checkTargetToLookShader (null);
							}
						}
					}
				}
			}
		}
	}

	bool settingShowCameraCursorWhenNotAimingBackToActive;

	public void activateShowCameraCursorWhenNotAimingState ()
	{
		activateIconWhenNotAiming = true;

		setCurrentLockedCameraCursor (weaponsManager.cursorRectTransform);

		setLookAtTargetOnLockedCameraState ();

		setLookAtTargetState (true, null);

		moveCameraLimitsXTopDown = new Vector2 (-9, 9);
		moveCameraLimitsYTopDown = new Vector2 (-7, 7);
	}

	public void inputCheckActivateStrafeModeIfNoTargetsToLookFound ()
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (!isCameraTypeFree ()) {
			return;
		}

		if (isFirstPersonActive ()) {
			return;
		}

		if (activateStrafeModeIfNoTargetsToLookFound) {
			if (!strafeModeActivateFromNoTargetsFoundActive) {
				if (targetToLook == null) {
					strafeModeActivateFromNoTargetsFoundActive = true;

					playerControllerManager.activateOrDeactivateStrafeMode (true);
				}
			} else {
				strafeModeActivateFromNoTargetsFoundActive = false;

				playerControllerManager.activateOrDeactivateStrafeMode (false);
			}
		}
	}

	public void inputZoom ()
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (isCameraTypeFree ()) {
			if (cameraCanBeUsed) {
				if (settings.zoomEnabled) {
					setZoom (!usingZoomOn);
				}
			}
		} else if (!isCameraTypeFree ()) {
			if (currentLockedCameraAxisInfo.canUseZoom) {
				if (!currentLockedCameraAxisInfo.useZoomByMovingCamera) {
					usingLockedZoomOn = !usingLockedZoomOn;

					float targetFov = currentLockedCameraAxisInfo.zoomValue;

					if (!usingLockedZoomOn) {
						targetFov = currentState.initialFovValue;
					}

					setMainCameraFov (targetFov, zoomSpeed);
				}
			}
		}
	}

	public void inputMoveSetRotateToLeftState (bool keyDown)
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (isCameraTypeFree ()) {
			if (cameraCanBeUsed) {
				if (zeroGravityModeOn && canRotateForwardOnZeroGravityModeOn) {
					if (keyDown) {
						setForwardRotationValue (1);
					} else {
						setForwardRotationValue (0);
					}
				}
			}
		}
	}

	public void inputMoveSetRotateToRightState (bool keyDown)
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (isCameraTypeFree ()) {
			if (cameraCanBeUsed) {
				if (zeroGravityModeOn && canRotateForwardOnZeroGravityModeOn) {
					if (keyDown) {
						setForwardRotationValue (-1);
					} else {
						setForwardRotationValue (0);
					}
				}
			}
		}
	}

	public void inputResetCameraRotation ()
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		resetCameraRotation ();
	}

	public void inputRotateLockedCameraToRight (bool holdingButton)
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (!isCameraTypeFree ()) {
			setLockedCameraRotationActiveToRight (holdingButton);
		}
	}

	public void inputRotateLockedCameraToLeft (bool holdingButton)
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (!isCameraTypeFree ()) {
			setLockedCameraRotationActiveToLeft (holdingButton);
		}
	}

	public void inputMoveCameraCloserOrFartherFromPlayer (bool movingDirection)
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		moveCameraCloserOrFartherFromPlayer (movingDirection);
	}

	public void inputEnableOrDisableRightLean (bool state)
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (!leanInputEnabled) {
			return;
		}

		enableOrDisableRightLean (state);
	}

	public void inputEnableOrDisableLeftLean (bool state)
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (!leanInputEnabled) {
			return;
		}

		enableOrDisableLeftLean (state);
	}

	public void inputSetExtraCameraRotation ()
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (!setExtraCameraRotationEnabled) {
			return;
		}

		if (isCameraTypeFree () && cameraCanBeUsed) {
			setExtraRotationToCamera ();
		}
	}

	float lockedCameraZoomMovingCameraValue;

	public void inputMoveLockedCameraZoomOn ()
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		inputMoveLockedCameraZoom (true);
	}

	public void inputMoveLockedCameraZoomOff ()
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		inputMoveLockedCameraZoom (false);
	}

	public void inputMoveLockedCameraZoom (bool moveForward)
	{
		if (!cameraActionsInputEnabled) {
			return;
		}

		if (isCameraTypeFree ()) {
			return;
		}

		if (moveForward) {
			lockedCameraZoomMovingCameraValue += currentLockedCameraAxisInfo.zoomByMovingCameraAmount;
		} else {
			lockedCameraZoomMovingCameraValue -= currentLockedCameraAxisInfo.zoomByMovingCameraAmount;
		}
	}
	//END OF CALL INPUT FUNCTIONS


	public void enableOrDisableRightLean (bool state)
	{
		if (currentState.leanEnabled || !state) {
			leanActive = state;

			leanToRightActive = true;

			leanToLeftActive = false;

			if (leanActive) {
				setLeanState ();
			} else {
				checkToRemoveMainCameraPivotFromLeanPivot = true;
			}
		}
	}

	public void enableOrDisableLeftLean (bool state)
	{
		if (currentState.leanEnabled || !state) {
			leanActive = state;

			leanToRightActive = false;

			leanToLeftActive = true;

			if (leanActive) {
				setLeanState ();
			} else {
				checkToRemoveMainCameraPivotFromLeanPivot = true;
			}
		}
	}

	public void enableOrDisableLeanInput (bool state)
	{
		leanInputEnabled = state;
	}

	public void disableLean ()
	{
		leanActive = false;

		checkToRemoveMainCameraPivotFromLeanPivot = true;
	}

	public void setLeanState ()
	{
		pivotLeanTransform.localPosition = new Vector3 (0, lerpState.leanHeight, 0);
		pivotLeanChildTransform.localPosition = new Vector3 (0, -lerpState.leanHeight, 0);

		pivotCameraTransform.SetParent (pivotLeanChildTransform);
	}

	public void removeMainCameraPivotFromLeanPivot ()
	{
		pivotCameraTransform.SetParent (playerCameraTransform);
	}

	public bool updateCurrentMousWheelCameraState (int newIndex)
	{
		cameraMouseWheelStates currentCameraMouseWheelStates = cameraMouseWheelStatesList [newIndex];

		if ((changeCameraViewEnabled && !pausePlayerCameraViewChange) || !currentCameraMouseWheelStates.isFirstPerson) {
			if (newIndex < currentMouseWheelStateIndex) {
				currentCameraMouseWheelStates.eventFromBelowCameraState.Invoke ();
			} else {
				currentCameraMouseWheelStates.eventFromAboveCameraState.Invoke ();
			}

			if (newIndex < currentMouseWheelStateIndex) {
				if (currentCameraMouseWheelStates.changeCameraFromBelowStateWithName) {
					setCameraState (currentCameraMouseWheelStates.Name);
				}
			} else {
				if (currentCameraMouseWheelStates.changeCameraFromAboveStateWithName) {
					setCameraState (currentCameraMouseWheelStates.Name);
				}
			}

			currentCameraMouseWheelStates.isCurrentCameraState = true;
			currentMouseWheelStateIndex = newIndex;

			return true;
		} 

		return false;
	}

	public void updateCurrentMouseWheelCameraStateByDistance ()
	{
		float currentCameraDistance = Math.Abs (lerpState.camPositionOffset.z);

		for (int i = 0; i < cameraMouseWheelStatesList.Count; i++) {
			if (currentCameraDistance >= cameraMouseWheelStatesList [i].cameraDistanceRange.x && currentCameraDistance <= cameraMouseWheelStatesList [i].cameraDistanceRange.y) {
				cameraMouseWheelStatesList [i].isCurrentCameraState = true;
				currentMouseWheelStateIndex = i;
			} else {
				cameraMouseWheelStatesList [i].isCurrentCameraState = false;
			}
		}
	}

	public void resetCameraRotation ()
	{
		if (isCameraTypeFree () && !firstPersonActive && cameraCanBeUsed) {
			resetingCameraActive = true;
		}
	}

	public override void setDrivingState (bool state)
	{
		driving = state;
	}

	public void setVehicleCameraMovementCoroutine (Coroutine cameraMovement)
	{
		vehicleCameraMovementCoroutine = cameraMovement;
	}

	public Coroutine getVehicleCameraMovementCoroutine ()
	{
		return vehicleCameraMovementCoroutine;
	}

	public List<Camera> getCameraList ()
	{
		return cameraList;
	}

	public void updateCameraList ()
	{
		for (int i = 0; i < cameraList.Count; i++) {
			if (cameraList [i].enabled) {
				cameraList [i].enabled = false;
				cameraList [i].enabled = true;
			}
		}
	}

	public Vector2 getMainCanvasSizeDelta ()
	{
		return mainCanvasSizeDelta;
	}

	public bool isUsingScreenSpaceCamera ()
	{
		return usingScreenSpaceCamera;
	}

	public void setFirstOrThirdPersonInEditor (bool state)
	{
		if (state) {
			setFirstPersonInEditor ();
		} else {
			setThirdPersonInEditor ();
		}
	}

	public void setThirdPersonInEditor ()
	{
		setThirdPersonInEditorEvent.Invoke ();
	}

	public void setFirstPersonInEditor ()
	{
		setFirstPersonInEditorEvent.Invoke ();
	}

	public void toggleViewOnEditor ()
	{
		setFirstOrThirdPersonInEditor (!firstPersonActive);
	}

	public void setCameraNoiseState (string cameraNoiseName)
	{
		for (int i = 0; i < cameraNoiseStateList.Count; i++) {
			if (cameraNoiseStateList [i].Name.Equals (cameraNoiseName)) {
				tick = UnityEngine.Random.Range (-100, 100);

				currentCameraNoiseState = cameraNoiseStateList [i];

				cameraNoiseActive = true;
			}
		}
	}

	public void disableCameraNoiseState ()
	{
		cameraNoiseActive = false;
	}

	public void setResetCameraRotationAfterTimeState (bool state)
	{
		resetCameraRotationAfterTime = state;
	}

	public void setUseSmoothCameraFollowState (bool state)
	{
		useSmoothCameraFollow = state;
	}

	public void setOriginalUseSmoothCameraFollowState ()
	{
		setUseSmoothCameraFollowState (originalUseSmoothCameraFollowValue);
	}

	Coroutine useSmoothCameraFollowCoroutine;

	public void activateUseSmoothCameraFollowStateDuration (float duration)
	{
		if (useSmoothCameraFollowCoroutine != null) {
			StopCoroutine (useSmoothCameraFollowCoroutine);
		}

		smoothCameraFollowSpeed = originalSmoothCameraFollowSpeed;

		useSmoothCameraFollowCoroutine = StartCoroutine (activateUseSmoothCameraFollowStateDurationCoroutine (duration));
	}

	IEnumerator activateUseSmoothCameraFollowStateDurationCoroutine (float duration)
	{
		setUseSmoothCameraFollowState (true);

		yield return new WaitForSeconds (duration);

		bool targetReached = false;

		while (!targetReached) {
			smoothCameraFollowSpeed -= getCurrentDeltaTime (); 

			if (smoothCameraFollowSpeed <= 0) {
				targetReached = true;
			}
		
			yield return null;
		}

		smoothCameraFollowSpeed = originalSmoothCameraFollowSpeed;

		setOriginalUseSmoothCameraFollowState ();
	}


	public void takeCapture ()
	{
		if (mainCameraCaptureSystem != null) {
			mainCameraCaptureSystem.takeCapture (mainCamera);
		}
	}

	public void addNewLockedCameraSystemToLevel ()
	{
		GameObject newLockedCameraSystem = (GameObject)Instantiate (lockedCameraSystemPrefab, vector3Zero, quaternionIdentity);
		newLockedCameraSystem.name = lockedCameraSystemPrefab.name;

		GKC_Utils.setActiveGameObjectInEditor (newLockedCameraSystem);
	}

	public void addNewLockedCameraLimitSystemToLevel ()
	{
		GameObject newLockedCameraLimitSystem = (GameObject)Instantiate (lockedCameraLimitSystemPrefab, vector3Zero, quaternionIdentity);
		newLockedCameraLimitSystem.name = lockedCameraLimitSystemPrefab.name;

		GKC_Utils.setActiveGameObjectInEditor (newLockedCameraLimitSystem);
	}

	public void addNewLockedCameraPrefabTypeLevel (int cameraIndex)
	{
		if (cameraIndex <= lockedCameraPrefabsTypesList.Count && lockedCameraPrefabsTypesList [cameraIndex].lockedCameraPrefab != null) {
			GameObject newLockedCameraSystem = (GameObject)Instantiate (lockedCameraPrefabsTypesList [cameraIndex].lockedCameraPrefab, vector3Zero, quaternionIdentity);
			newLockedCameraSystem.name = lockedCameraPrefabsTypesList [cameraIndex].lockedCameraPrefab.name;

			GKC_Utils.setActiveGameObjectInEditor (newLockedCameraSystem);
		} else {
			print ("WARNING: prefab of the selected camera doesn't exist or is not configured on this list, make sure a prefab is assigned");
		}
	}

	public void setTemporalLockedCameraPrefabNameToPlaceOnSceneFromEditor (string newLockedCameraPrefabName)
	{
		temporaLockedCameraPrefabNmeToPlaceOnScene = newLockedCameraPrefabName;

		updateComponent ();
	}

	public void setTemporalLockedCameraPrefabToPlaceOnSceneFromEditor ()
	{
		int cameraIndex = lockedCameraPrefabsTypesList.FindIndex (s => s.Name.ToLower () == temporaLockedCameraPrefabNmeToPlaceOnScene.ToLower ());

		if (cameraIndex > -1) {
			addNewLockedCameraPrefabTypeLevel (cameraIndex);

			GameObject newLockedCameraSystem = GKC_Utils.getActiveGameObjectInEditor ();

			if (newLockedCameraSystem != null) {
				lockedCameraSystem currentLockedCameraSystem = newLockedCameraSystem.GetComponent<lockedCameraSystem> ();
			
				if (currentLockedCameraSystem != null) {
					currentLockedCameraSystem.assignPlayerToStartOnLockedCamera (playerControllerGameObject);

					print ("Placing Locked Camera Prefab on Scene: " + currentLockedCameraSystem.gameObject.name);
				}
			}
		}
	}

	public void updateCameraStateValuesOnEditor (int stateIndex)
	{
		cameraStateInfo newState = new cameraStateInfo (playerCameraStates [stateIndex]);

		lerpState = newState;
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	//draw the lines of the pivot camera in the editor
	void OnDrawGizmos ()
	{
		if (!settings.showCameraGizmo) {
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
		if (settings.showCameraGizmo) {
			for (int i = 0; i < playerCameraStates.Count; i++) {
				if (playerCameraStates [i].showGizmo) {
					Gizmos.color = playerCameraStates [i].gizmoColor;

					Vector3 currentPivotOffset = playerCameraStates [i].pivotPositionOffset;

					Vector3 pivotPosition = playerCameraTransform.position
					                        + playerCameraTransform.right * currentPivotOffset.x
					                        + playerCameraTransform.up * currentPivotOffset.y
					                        + playerCameraTransform.forward * currentPivotOffset.z;

					Vector3 currentCameraOffset = playerCameraStates [i].camPositionOffset;
					Vector3 cameraPosition = pivotPosition
					                         + pivotCameraTransform.right * currentCameraOffset.x
					                         + pivotCameraTransform.up * currentCameraOffset.y
					                         + pivotCameraTransform.forward * currentCameraOffset.z;

					Gizmos.DrawSphere (cameraPosition, settings.gizmoRadius);
					Gizmos.color = playerCameraStates [i].gizmoColor;
					Gizmos.DrawSphere (pivotPosition, settings.gizmoRadius);

					Gizmos.color = settings.linesColor;
					Gizmos.DrawLine (cameraPosition, pivotPosition);
					Gizmos.DrawLine (pivotPosition, playerCameraTransform.position);
				}
			}
		}
			
		if (!isCameraTypeFree ()) {
			if (currentLockedCameraAxisInfo != null) {
				if (closestWaypoint != null) {
					Gizmos.color = Color.green;
					Gizmos.DrawWireSphere (closestWaypoint.position, 1.1f);
				}

				if (useCameraLimit) {
					Gizmos.color = Color.red;

					Gizmos.DrawWireSphere (currentCameraLimitPosition, 1);
				}

				if (currentLockedCameraAxisInfo.lockedCameraPivot != null) {
					Gizmos.color = Color.green;
					Vector3 pivotPosition = lockedCameraPivot.position + lockedCameraPivot.up * lockedCameraAxis.localPosition.y;

					Gizmos.DrawLine (lockedCameraAxis.position, pivotPosition);

					Gizmos.DrawLine (pivotPosition, lockedCameraPivot.position);

					GKC_Utils.drawGizmoArrow (lockedCameraPosition.position, lockedCameraPosition.forward * 1, Color.green, 0.5f, 10);

					Gizmos.color = Color.yellow;
					Gizmos.DrawLine (lookCameraParent.position, lookCameraPivot.position);
					Gizmos.DrawLine (lookCameraPivot.position, lookCameraDirection.position);
				}

				if (currentLockedCameraAxisInfo.useBoundToFollowPlayer) {
					//lockedCameraSystem.drawBoundGizmo (focusArea.centre, currentLockedCameraAxisInfo, new Color (1, 0, 0, .5f));

					Gizmos.color = new Color (1, 0, 0, .5f);

					float height = (currentLockedCameraAxisInfo.heightBoundTop);
					float width = (currentLockedCameraAxisInfo.widthBoundRight + currentLockedCameraAxisInfo.widthBoundLeft);
					float depth = (currentLockedCameraAxisInfo.depthBoundFront + currentLockedCameraAxisInfo.depthBoundBackward);

					Gizmos.DrawCube (focusArea.centre, new Vector3 (width, height, depth));
				}
			}
		}
	}

	//a group of parameters to configure the shake of the camera
	[System.Serializable]
	public class cameraSettings
	{
		public LayerMask layer;
		public bool useAcelerometer;
		public bool zoomEnabled;
		public bool moveAwayCameraEnabled;
		public bool enableMoveAwayInAir = true;
		public bool enableShakeCamera = true;
		public bool showCameraGizmo = true;
		public float gizmoRadius;
		public Color gizmoLabelColor;
		public Color linesColor;
		[HideInInspector] public bool shakingCameraActive = false;
		[HideInInspector] public bool accelerateShaking = false;
	}

	[System.Serializable]
	public struct FocusArea
	{
		public Vector3 centre;
		public Vector3 velocity;
		public float left, right;
		public float top, bottom;
		public float front, backward;

		public FocusArea (Bounds targetBounds, float heightBoundTop, float widthBoundRight, float widthBoundLeft, float depthBoundFront, float depthBoundBackward)
		{
			left = targetBounds.center.x - widthBoundLeft;
			right = targetBounds.center.x + widthBoundRight;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + heightBoundTop;

			front = targetBounds.center.z + depthBoundFront;
			backward = targetBounds.center.z - depthBoundBackward;

			velocity = Vector3.zero;
			centre = new Vector3 ((left + right) / 2, (top + bottom) / 2, (front + backward) / 2);
		}

		public void Update (Bounds targetBounds)
		{
			float shiftX = 0;

			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}

			left += shiftX;
			right += shiftX;

			float shiftY = 0;

			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}

			top += shiftY;
			bottom += shiftY;

			float shiftZ = 0;

			if (targetBounds.min.z < backward) {
				shiftZ = targetBounds.min.z - backward;
			} else if (targetBounds.max.z > front) {
				shiftZ = targetBounds.max.z - front;
			}

			front += shiftZ;
			backward += shiftZ;

			centre = new Vector3 ((left + right) / 2, (top + bottom) / 2, (front + backward) / 2);
			velocity = new Vector3 (shiftX, shiftY, shiftZ);
		}
	}

	[System.Serializable]
	public class cameraMouseWheelStates
	{
		public string Name;
		public Vector2 cameraDistanceRange;

		public bool changeCameraIfDistanceChanged;
		public float minCameraDistanceToChange;
		public bool changeToAboveCameraState;

		public bool changeCameraFromAboveStateWithName;
		public bool changeCameraFromBelowStateWithName;

		public bool isFirstPerson;

		public bool isCurrentCameraState;
		public UnityEvent eventFromAboveCameraState;
		public UnityEvent eventFromBelowCameraState;
	}

	[System.Serializable]
	public class lockedCameraPrefabsTypes
	{
		public string Name;
		public GameObject lockedCameraPrefab;
	}

	[System.Serializable]
	public class cameraNoiseState
	{
		public string Name;
		public Vector2 noiseAmount;
		public Vector2 noiseSpeed;

		public Vector2 currentNoise;

		public float roughness;
	}

	[System.Serializable]
	public class cameraStateEventInfo
	{
		public string Name;
		public UnityEvent eventOnCameraStart;
		public UnityEvent eventOnCameraEnd;
	}

	[System.Serializable]
	public class customReticleInfo
	{
		public string Name;
		public GameObject customReticleObject;
		public bool isCurrentReticle;
	}
}