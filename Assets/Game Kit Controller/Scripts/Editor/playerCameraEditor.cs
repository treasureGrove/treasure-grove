using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerCamera))]
public class playerCameraEditor : Editor
{
	SerializedProperty playerCameraTransform;
	SerializedProperty mainCamera;
	SerializedProperty mainCameraTransform;
	SerializedProperty pivotCameraTransform;
	SerializedProperty pivotLeanTransform;
	SerializedProperty pivotLeanChildTransform;

	SerializedProperty mainCameraParent;

	SerializedProperty hipsTransform;

	SerializedProperty cameraRotationInputEnabled;
	SerializedProperty thirdPersonVerticalRotationSpeed;
	SerializedProperty thirdPersonHorizontalRotationSpeed;
	SerializedProperty firstPersonVerticalRotationSpeed;
	SerializedProperty firstPersonHorizontalRotationSpeed;
	SerializedProperty smoothBetweenState;
	SerializedProperty maxCheckDist;
	SerializedProperty movementLerpSpeed;

	SerializedProperty regularMovementOnBulletTime;
	SerializedProperty useUnscaledTimeOnBulletTime;


	SerializedProperty extraCameraCollisionDistance;
	SerializedProperty cameraCollisionAlwaysActive;

	SerializedProperty usePivotCameraCollisionEnabled;
	SerializedProperty pivotCameraCollisionHeightOffset;

	SerializedProperty changeCameraViewEnabled;
	SerializedProperty aimSide;
	SerializedProperty changeCameraSideActive;

	SerializedProperty useSmoothCameraRotation;
	SerializedProperty useSmoothCameraRotationThirdPerson;
	SerializedProperty smoothCameraRotationSpeedVerticalThirdPerson;
	SerializedProperty smoothCameraRotationSpeedHorizontalThirdPerson;
	SerializedProperty useSmoothCameraRotationFirstPerson;
	SerializedProperty smoothCameraRotationSpeedVerticalFirstPerson;
	SerializedProperty smoothCameraRotationSpeedHorizontalFirstPerson;

	SerializedProperty zoomSpeed;
	SerializedProperty fovChangeSpeed;
	SerializedProperty thirdPersonVerticalRotationSpeedZoomIn;
	SerializedProperty thirdPersonHorizontalRotationSpeedZoomIn;
	SerializedProperty firstPersonVerticalRotationSpeedZoomIn;
	SerializedProperty firstPersonHorizontalRotationSpeedZoomIn;
	SerializedProperty moveCameraPositionWithMouseWheelActive;
	SerializedProperty moveCameraPositionForwardWithMouseWheelSpeed;
	SerializedProperty moveCameraPositionBackwardWithMouseWheelSpeed;
	SerializedProperty maxExtraDistanceOnThirdPerson;
	SerializedProperty useCameraMouseWheelStates;
	SerializedProperty cameraMouseWheelStatesList;

	SerializedProperty reverseMouseWheelDirectionEnabled;

	SerializedProperty minDistanceToChangeToFirstPerson;
	SerializedProperty using2_5ViewActive;
	SerializedProperty cameraType;
	SerializedProperty useCameraLimit;
	SerializedProperty useWidthLimit;
	SerializedProperty widthLimitRight;
	SerializedProperty widthLimitLeft;
	SerializedProperty useHeightLimit;
	SerializedProperty heightLimitUpper;
	SerializedProperty heightLimitLower;
	SerializedProperty useDepthLimit;
	SerializedProperty depthLimitFront;
	SerializedProperty depthLimitBackward;
	SerializedProperty currentCameraLimitPosition;
	SerializedProperty clampAimDirections;
	SerializedProperty numberOfAimDirections;
	SerializedProperty minDistanceToCenterClampAim;
	SerializedProperty canRotateForwardOnZeroGravityModeOn;
	SerializedProperty rotateForwardOnZeroGravitySpeed;
	SerializedProperty resetCameraRotationAfterTime;
	SerializedProperty timeToResetCameraRotation;
	SerializedProperty resetCameraRotationSpeed;
	SerializedProperty setExtraCameraRotationEnabled;
	SerializedProperty extraCameraRotationAmount;
	SerializedProperty extraCameraRotationSpeed;
	SerializedProperty useCameraForwardDirection;
	SerializedProperty currentStateName;
	SerializedProperty defaultThirdPersonStateName;
	SerializedProperty defaultFirstPersonStateName;
	SerializedProperty defaultThirdPersonCrouchStateName;
	SerializedProperty defaultFirstPersonCrouchStateName;
	SerializedProperty defaultThirdPersonAimRightStateName;
	SerializedProperty defaultThirdPersonAimLeftStateName;
	SerializedProperty defaultMoveCameraAwayStateName;
	SerializedProperty defaultLockedCameraStateName;
	SerializedProperty defaultAICameraStateName;
	SerializedProperty defaultVehiclePassengerStateName;
	SerializedProperty playerCameraStates;
	SerializedProperty useEventsOnCameraStateChange;
	SerializedProperty cameraStateEventInfoList;
	SerializedProperty onGround;
	SerializedProperty aimingInThirdPerson;
	SerializedProperty crouching;
	SerializedProperty leanActive;
	SerializedProperty leanToRightActive;
	SerializedProperty firstPersonActive;
	SerializedProperty usingZoomOn;
	SerializedProperty usingZoomOff;
	SerializedProperty cameraCanRotate;
	SerializedProperty cameraCanBeUsed;
	SerializedProperty lookingAtTarget;
	SerializedProperty lookingAtPoint;
	SerializedProperty useTopDownView;
	SerializedProperty horizontalInput;
	SerializedProperty verticalInput;
	SerializedProperty lookAngle;
	SerializedProperty playerNavMeshEnabled;
	SerializedProperty followingMultipleTargets;
	SerializedProperty useCustomThirdPersonAimActive;
	SerializedProperty lockedCameraSystemPrefab;
	SerializedProperty lockedCameraLimitSystemPrefab;
	SerializedProperty lockedCameraPrefabsTypesList;
	SerializedProperty cameraNoiseStateList;
	SerializedProperty usedByAI;
	SerializedProperty showLookTargetSettings;
	SerializedProperty showSettings;
	SerializedProperty layer;
	SerializedProperty useAcelerometer;
	SerializedProperty zoomEnabled;
	SerializedProperty moveAwayCameraEnabled;
	SerializedProperty enableMoveAwayInAir;
	SerializedProperty enableShakeCamera;
	SerializedProperty showElements;
	SerializedProperty lockedCameraElementsParent;
	SerializedProperty lockedMainCameraTransform;
	SerializedProperty lockedCameraAxis;
	SerializedProperty lockedCameraPosition;
	SerializedProperty lockedCameraPivot;
	SerializedProperty lookCameraParent;
	SerializedProperty lookCameraPivot;
	SerializedProperty lookCameraDirection;
	SerializedProperty clampAimDirectionTransform;
	SerializedProperty lookDirectionTransform;
	SerializedProperty auxLockedCameraAxis;
	SerializedProperty setTransparentSurfacesManager;
	SerializedProperty playerControllerGameObject;
	SerializedProperty playerInput;
	SerializedProperty playerControllerManager;
	SerializedProperty powersManager;
	SerializedProperty gravityManager;
	SerializedProperty headBobManager;
	SerializedProperty grabObjectsManager;
	SerializedProperty scannerManager;
	SerializedProperty weaponsManager;
	SerializedProperty playerNavMeshManager;
	SerializedProperty characterStateIconManager;

	SerializedProperty mainCollider;

	SerializedProperty mainCameraCaptureSystem;

	SerializedProperty targetToFollow;
	SerializedProperty useEventOnMovingLockedCamera;
	SerializedProperty useEventOnFreeCamera;
	SerializedProperty eventOnStartMovingLockedCamera;
	SerializedProperty eventOnKeepMovingLockedCamera;
	SerializedProperty eventOnStopMovingLockedCamera;
	SerializedProperty useEventOnThirdFirstPersonViewChange;
	SerializedProperty setFirstPersonEvent;
	SerializedProperty setThirdPersonEvent;
	SerializedProperty setThirdPersonInEditorEvent;
	SerializedProperty setFirstPersonInEditorEvent;
	SerializedProperty cameraList;

	SerializedProperty updateReticleActiveState;
	SerializedProperty useCameraReticleThirdPerson;
	SerializedProperty useCameraReticleFirstPerson;
	SerializedProperty cameraReticleGameObject;
	SerializedProperty mainCameraReticleGameObject;
	SerializedProperty cameraReticleCurrentlyActive;
	SerializedProperty externalReticleCurrentlyActive;

	SerializedProperty mainCustomReticleParent;
	SerializedProperty customReticleInfoList;

	SerializedProperty adjustCameraToPreviousCharacterDirectionActive;
	SerializedProperty useSmoothCameraTransitionBetweenCharacters;
	SerializedProperty smoothCameraTransitionBetweenCharactersSpeed;
	SerializedProperty setNewCharacterAlwaysInPreviousCharacterView;
	SerializedProperty setNewCharacterAlwaysInThirdPerson;
	SerializedProperty setNewCharacterAlwaysInFirstPerson;

	SerializedProperty useMultipleTargetFov;
	SerializedProperty multipleTargetsMinFov;
	SerializedProperty multipleTargetsMaxFov;
	SerializedProperty multipleTargetsFovSpeed;
	SerializedProperty useMultipleTargetsYPosition;
	SerializedProperty multipleTargetsYPositionSpeed;
	SerializedProperty multipleTargetsMaxHeight;
	SerializedProperty multipleTargetsHeightMultiplier;

	SerializedProperty maxMultipleTargetHeight;
	SerializedProperty minMultipleTargetHeight;

	SerializedProperty showCameraGizmo;
	SerializedProperty gizmoRadius;
	SerializedProperty gizmoLabelColor;
	SerializedProperty linesColor;
	SerializedProperty lookAtTargetEnabled;
	SerializedProperty canActivateLookAtTargetEnabled;
	SerializedProperty canActiveLookAtTargetOnLockedCamera;
	SerializedProperty lookAtTargetTransform;
	SerializedProperty targetToLook;
	SerializedProperty maxDistanceToFindTarget;
	SerializedProperty useLayerToSearchTargets;
	SerializedProperty layerToLook;
	SerializedProperty lookOnlyIfTargetOnScreen;
	SerializedProperty checkObstaclesToTarget;
	SerializedProperty getClosestToCameraCenter;
	SerializedProperty useMaxDistanceToCameraCenter;
	SerializedProperty maxDistanceToCameraCenter;
	SerializedProperty activateStrafeModeIfNoTargetsToLookFound;
	SerializedProperty searchPointToLookComponents;
	SerializedProperty pointToLookComponentsLayer;
	SerializedProperty useTimeToStopAimAssist;
	SerializedProperty timeToStopAimAssist;
	SerializedProperty useTimeToStopAimAssistLockedCamera;
	SerializedProperty timeToStopAimAssistLockedCamera;
	SerializedProperty canChangeTargetToLookWithCameraAxis;
	SerializedProperty minimumCameraDragToChangeTargetToLook;
	SerializedProperty waitTimeToNextChangeTargetToLook;
	SerializedProperty useOnlyHorizontalCameraDragValue;
	SerializedProperty lookAtTargetSpeed;
	SerializedProperty lookCloserAtTargetSpeed;
	SerializedProperty lookAtTargetSpeed2_5dView;
	SerializedProperty lookAtTargetSpeedOthersLockedViews;
	SerializedProperty useLookTargetIcon;
	SerializedProperty lookAtTargetIcon;
	SerializedProperty lookAtTargetRegularIconGameObject;
	SerializedProperty lookAtTargetIconWhenNotAiming;
	SerializedProperty tagToLookList;
	SerializedProperty lookAtBodyPartsOnCharacters;
	SerializedProperty bodyPartsToLook;
	SerializedProperty useObjectToGrabFoundShader;
	SerializedProperty objectToGrabFoundShader;
	SerializedProperty shaderOutlineWidth;
	SerializedProperty shaderOutlineColor;
	SerializedProperty useRemoteEventsOnLockOn;
	SerializedProperty remoteEventOnLockOnStart;
	SerializedProperty remoteEventOnLockOnEnd;
	SerializedProperty useEventsOnLockOn;
	SerializedProperty eventOnLockOnStart;
	SerializedProperty eventOnLockOnEnd;

	SerializedProperty layerToCheckPossibleTargetsBelowCursor;

	SerializedProperty useSmoothCameraFollow;
	SerializedProperty smoothCameraFollowSpeed;
	SerializedProperty smoothCameraFollowSpeedOnAim;
	SerializedProperty smoothCameraFollowMaxDistance;
	SerializedProperty smoothCameraFollowMaxDistanceSpeed;

	SerializedProperty showDebugPrint;

	playerCamera camera;
	public string currentCamera;
	bool checkCamera;
	Color defBackgroundColor;
	GUIStyle style = new GUIStyle ();
	bool expanded;

	string currentButtonString;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		playerCameraTransform = serializedObject.FindProperty ("playerCameraTransform");
		mainCamera = serializedObject.FindProperty ("mainCamera");
		mainCameraTransform = serializedObject.FindProperty ("mainCameraTransform");
		pivotCameraTransform = serializedObject.FindProperty ("pivotCameraTransform");
		pivotLeanTransform = serializedObject.FindProperty ("pivotLeanTransform");
		pivotLeanChildTransform = serializedObject.FindProperty ("pivotLeanChildTransform");

		mainCameraParent = serializedObject.FindProperty ("mainCameraParent");

		hipsTransform = serializedObject.FindProperty ("hipsTransform");

		cameraRotationInputEnabled = serializedObject.FindProperty ("cameraRotationInputEnabled");
		thirdPersonVerticalRotationSpeed = serializedObject.FindProperty ("thirdPersonVerticalRotationSpeed");
		thirdPersonHorizontalRotationSpeed = serializedObject.FindProperty ("thirdPersonHorizontalRotationSpeed");
		firstPersonVerticalRotationSpeed = serializedObject.FindProperty ("firstPersonVerticalRotationSpeed");
		firstPersonHorizontalRotationSpeed = serializedObject.FindProperty ("firstPersonHorizontalRotationSpeed");
		smoothBetweenState = serializedObject.FindProperty ("smoothBetweenState");
		maxCheckDist = serializedObject.FindProperty ("maxCheckDist");
		movementLerpSpeed = serializedObject.FindProperty ("movementLerpSpeed");

		regularMovementOnBulletTime = serializedObject.FindProperty ("regularMovementOnBulletTime");
		useUnscaledTimeOnBulletTime = serializedObject.FindProperty ("useUnscaledTimeOnBulletTime");


		extraCameraCollisionDistance = serializedObject.FindProperty ("extraCameraCollisionDistance");
		cameraCollisionAlwaysActive = serializedObject.FindProperty ("cameraCollisionAlwaysActive");

		usePivotCameraCollisionEnabled = serializedObject.FindProperty ("usePivotCameraCollisionEnabled");
		pivotCameraCollisionHeightOffset = serializedObject.FindProperty ("pivotCameraCollisionHeightOffset");

		changeCameraViewEnabled = serializedObject.FindProperty ("changeCameraViewEnabled");
		aimSide = serializedObject.FindProperty ("aimSide");
		changeCameraSideActive = serializedObject.FindProperty ("changeCameraSideActive");
		useSmoothCameraRotation = serializedObject.FindProperty ("useSmoothCameraRotation");
		useSmoothCameraRotationThirdPerson = serializedObject.FindProperty ("useSmoothCameraRotationThirdPerson");
		smoothCameraRotationSpeedVerticalThirdPerson = serializedObject.FindProperty ("smoothCameraRotationSpeedVerticalThirdPerson");
		smoothCameraRotationSpeedHorizontalThirdPerson = serializedObject.FindProperty ("smoothCameraRotationSpeedHorizontalThirdPerson");
		useSmoothCameraRotationFirstPerson = serializedObject.FindProperty ("useSmoothCameraRotationFirstPerson");
		smoothCameraRotationSpeedVerticalFirstPerson = serializedObject.FindProperty ("smoothCameraRotationSpeedVerticalFirstPerson");
		smoothCameraRotationSpeedHorizontalFirstPerson = serializedObject.FindProperty ("smoothCameraRotationSpeedHorizontalFirstPerson");
		zoomSpeed = serializedObject.FindProperty ("zoomSpeed");
		fovChangeSpeed = serializedObject.FindProperty ("fovChangeSpeed");
		thirdPersonVerticalRotationSpeedZoomIn = serializedObject.FindProperty ("thirdPersonVerticalRotationSpeedZoomIn");
		thirdPersonHorizontalRotationSpeedZoomIn = serializedObject.FindProperty ("thirdPersonHorizontalRotationSpeedZoomIn");
		firstPersonVerticalRotationSpeedZoomIn = serializedObject.FindProperty ("firstPersonVerticalRotationSpeedZoomIn");
		firstPersonHorizontalRotationSpeedZoomIn = serializedObject.FindProperty ("firstPersonHorizontalRotationSpeedZoomIn");
		moveCameraPositionWithMouseWheelActive = serializedObject.FindProperty ("moveCameraPositionWithMouseWheelActive");
		moveCameraPositionForwardWithMouseWheelSpeed = serializedObject.FindProperty ("moveCameraPositionForwardWithMouseWheelSpeed");
		moveCameraPositionBackwardWithMouseWheelSpeed = serializedObject.FindProperty ("moveCameraPositionBackwardWithMouseWheelSpeed");
		maxExtraDistanceOnThirdPerson = serializedObject.FindProperty ("maxExtraDistanceOnThirdPerson");
		useCameraMouseWheelStates = serializedObject.FindProperty ("useCameraMouseWheelStates");
		cameraMouseWheelStatesList = serializedObject.FindProperty ("cameraMouseWheelStatesList");

		reverseMouseWheelDirectionEnabled = serializedObject.FindProperty ("reverseMouseWheelDirectionEnabled");

		minDistanceToChangeToFirstPerson = serializedObject.FindProperty ("minDistanceToChangeToFirstPerson");
		using2_5ViewActive = serializedObject.FindProperty ("using2_5ViewActive");
		cameraType = serializedObject.FindProperty ("cameraType");
		useCameraLimit = serializedObject.FindProperty ("useCameraLimit");
		useWidthLimit = serializedObject.FindProperty ("useWidthLimit");
		widthLimitRight = serializedObject.FindProperty ("widthLimitRight");
		widthLimitLeft = serializedObject.FindProperty ("widthLimitLeft");
		useHeightLimit = serializedObject.FindProperty ("useHeightLimit");
		heightLimitUpper = serializedObject.FindProperty ("heightLimitUpper");
		heightLimitLower = serializedObject.FindProperty ("heightLimitLower");
		useDepthLimit = serializedObject.FindProperty ("useDepthLimit");
		depthLimitFront = serializedObject.FindProperty ("depthLimitFront");
		depthLimitBackward = serializedObject.FindProperty ("depthLimitBackward");
		currentCameraLimitPosition = serializedObject.FindProperty ("currentCameraLimitPosition");
		clampAimDirections = serializedObject.FindProperty ("clampAimDirections");
		numberOfAimDirections = serializedObject.FindProperty ("numberOfAimDirections");
		minDistanceToCenterClampAim = serializedObject.FindProperty ("minDistanceToCenterClampAim");
		canRotateForwardOnZeroGravityModeOn = serializedObject.FindProperty ("canRotateForwardOnZeroGravityModeOn");
		rotateForwardOnZeroGravitySpeed = serializedObject.FindProperty ("rotateForwardOnZeroGravitySpeed");
		resetCameraRotationAfterTime = serializedObject.FindProperty ("resetCameraRotationAfterTime");
		timeToResetCameraRotation = serializedObject.FindProperty ("timeToResetCameraRotation");
		resetCameraRotationSpeed = serializedObject.FindProperty ("resetCameraRotationSpeed");
		setExtraCameraRotationEnabled = serializedObject.FindProperty ("setExtraCameraRotationEnabled");
		extraCameraRotationAmount = serializedObject.FindProperty ("extraCameraRotationAmount");
		extraCameraRotationSpeed = serializedObject.FindProperty ("extraCameraRotationSpeed");
		useCameraForwardDirection = serializedObject.FindProperty ("useCameraForwardDirection");
		currentStateName = serializedObject.FindProperty ("currentStateName");
		defaultThirdPersonStateName = serializedObject.FindProperty ("defaultThirdPersonStateName");
		defaultFirstPersonStateName = serializedObject.FindProperty ("defaultFirstPersonStateName");
		defaultThirdPersonCrouchStateName = serializedObject.FindProperty ("defaultThirdPersonCrouchStateName");
		defaultFirstPersonCrouchStateName = serializedObject.FindProperty ("defaultFirstPersonCrouchStateName");
		defaultThirdPersonAimRightStateName = serializedObject.FindProperty ("defaultThirdPersonAimRightStateName");
		defaultThirdPersonAimLeftStateName = serializedObject.FindProperty ("defaultThirdPersonAimLeftStateName");
		defaultMoveCameraAwayStateName = serializedObject.FindProperty ("defaultMoveCameraAwayStateName");
		defaultLockedCameraStateName = serializedObject.FindProperty ("defaultLockedCameraStateName");
		defaultAICameraStateName = serializedObject.FindProperty ("defaultAICameraStateName");
		defaultVehiclePassengerStateName = serializedObject.FindProperty ("defaultVehiclePassengerStateName");
		playerCameraStates = serializedObject.FindProperty ("playerCameraStates");
		useEventsOnCameraStateChange = serializedObject.FindProperty ("useEventsOnCameraStateChange");
		cameraStateEventInfoList = serializedObject.FindProperty ("cameraStateEventInfoList");
		onGround = serializedObject.FindProperty ("onGround");
		aimingInThirdPerson = serializedObject.FindProperty ("aimingInThirdPerson");
		crouching = serializedObject.FindProperty ("crouching");
		leanActive = serializedObject.FindProperty ("leanActive");
		leanToRightActive = serializedObject.FindProperty ("leanToRightActive");
		firstPersonActive = serializedObject.FindProperty ("firstPersonActive");
		usingZoomOn = serializedObject.FindProperty ("usingZoomOn");
		usingZoomOff = serializedObject.FindProperty ("usingZoomOff");
		cameraCanRotate = serializedObject.FindProperty ("cameraCanRotate");
		cameraCanBeUsed = serializedObject.FindProperty ("cameraCanBeUsed");
		lookingAtTarget = serializedObject.FindProperty ("lookingAtTarget");
		lookingAtPoint = serializedObject.FindProperty ("lookingAtPoint");
		useTopDownView = serializedObject.FindProperty ("useTopDownView");
		horizontalInput = serializedObject.FindProperty ("horizontalInput");
		verticalInput = serializedObject.FindProperty ("verticalInput");
		lookAngle = serializedObject.FindProperty ("lookAngle");
		playerNavMeshEnabled = serializedObject.FindProperty ("playerNavMeshEnabled");
		followingMultipleTargets = serializedObject.FindProperty ("followingMultipleTargets");
		useCustomThirdPersonAimActive = serializedObject.FindProperty ("useCustomThirdPersonAimActive");
		lockedCameraSystemPrefab = serializedObject.FindProperty ("lockedCameraSystemPrefab");
		lockedCameraLimitSystemPrefab = serializedObject.FindProperty ("lockedCameraLimitSystemPrefab");
		lockedCameraPrefabsTypesList = serializedObject.FindProperty ("lockedCameraPrefabsTypesList");
		cameraNoiseStateList = serializedObject.FindProperty ("cameraNoiseStateList");
		usedByAI = serializedObject.FindProperty ("usedByAI");
		showLookTargetSettings = serializedObject.FindProperty ("showLookTargetSettings");
		showSettings = serializedObject.FindProperty ("showSettings");
		layer = serializedObject.FindProperty ("settings.layer");
		useAcelerometer = serializedObject.FindProperty ("settings.useAcelerometer");
		zoomEnabled = serializedObject.FindProperty ("settings.zoomEnabled");
		moveAwayCameraEnabled = serializedObject.FindProperty ("settings.moveAwayCameraEnabled");
		enableMoveAwayInAir = serializedObject.FindProperty ("settings.enableMoveAwayInAir");
		enableShakeCamera = serializedObject.FindProperty ("settings.enableShakeCamera");
		showElements = serializedObject.FindProperty ("showElements");
		lockedCameraElementsParent = serializedObject.FindProperty ("lockedCameraElementsParent");
		lockedMainCameraTransform = serializedObject.FindProperty ("lockedMainCameraTransform");
		lockedCameraAxis = serializedObject.FindProperty ("lockedCameraAxis");
		lockedCameraPosition = serializedObject.FindProperty ("lockedCameraPosition");
		lockedCameraPivot = serializedObject.FindProperty ("lockedCameraPivot");
		lookCameraParent = serializedObject.FindProperty ("lookCameraParent");
		lookCameraPivot = serializedObject.FindProperty ("lookCameraPivot");
		lookCameraDirection = serializedObject.FindProperty ("lookCameraDirection");
		clampAimDirectionTransform = serializedObject.FindProperty ("clampAimDirectionTransform");
		lookDirectionTransform = serializedObject.FindProperty ("lookDirectionTransform");
		auxLockedCameraAxis = serializedObject.FindProperty ("auxLockedCameraAxis");
		setTransparentSurfacesManager = serializedObject.FindProperty ("setTransparentSurfacesManager");
		playerControllerGameObject = serializedObject.FindProperty ("playerControllerGameObject");
		playerInput = serializedObject.FindProperty ("playerInput");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		powersManager = serializedObject.FindProperty ("powersManager");
		gravityManager = serializedObject.FindProperty ("gravityManager");
		headBobManager = serializedObject.FindProperty ("headBobManager");
		grabObjectsManager = serializedObject.FindProperty ("grabObjectsManager");
		scannerManager = serializedObject.FindProperty ("scannerManager");
		weaponsManager = serializedObject.FindProperty ("weaponsManager");
		playerNavMeshManager = serializedObject.FindProperty ("playerNavMeshManager");
		characterStateIconManager = serializedObject.FindProperty ("characterStateIconManager");

		mainCollider = serializedObject.FindProperty ("mainCollider");

		mainCameraCaptureSystem = serializedObject.FindProperty ("mainCameraCaptureSystem");

		targetToFollow = serializedObject.FindProperty ("targetToFollow");
		useEventOnMovingLockedCamera = serializedObject.FindProperty ("useEventOnMovingLockedCamera");
		useEventOnFreeCamera = serializedObject.FindProperty ("useEventOnFreeCamereToo");
		eventOnStartMovingLockedCamera = serializedObject.FindProperty ("eventOnStartMovingLockedCamera");
		eventOnKeepMovingLockedCamera = serializedObject.FindProperty ("eventOnKeepMovingLockedCamera");
		eventOnStopMovingLockedCamera = serializedObject.FindProperty ("eventOnStopMovingLockedCamera");
		useEventOnThirdFirstPersonViewChange = serializedObject.FindProperty ("useEventOnThirdFirstPersonViewChange");
		setFirstPersonEvent = serializedObject.FindProperty ("setFirstPersonEvent");
		setThirdPersonEvent = serializedObject.FindProperty ("setThirdPersonEvent");
		setThirdPersonInEditorEvent = serializedObject.FindProperty ("setThirdPersonInEditorEvent");
		setFirstPersonInEditorEvent = serializedObject.FindProperty ("setFirstPersonInEditorEvent");
		cameraList = serializedObject.FindProperty ("cameraList");

		updateReticleActiveState = serializedObject.FindProperty ("updateReticleActiveState");
		useCameraReticleThirdPerson = serializedObject.FindProperty ("useCameraReticleThirdPerson");
		useCameraReticleFirstPerson = serializedObject.FindProperty ("useCameraReticleFirstPerson");
		cameraReticleGameObject = serializedObject.FindProperty ("cameraReticleGameObject");
		mainCameraReticleGameObject = serializedObject.FindProperty ("mainCameraReticleGameObject");
		cameraReticleCurrentlyActive = serializedObject.FindProperty ("cameraReticleCurrentlyActive");
		externalReticleCurrentlyActive = serializedObject.FindProperty ("externalReticleCurrentlyActive");

		mainCustomReticleParent = serializedObject.FindProperty ("mainCustomReticleParent");
		customReticleInfoList = serializedObject.FindProperty ("customReticleInfoList");

		adjustCameraToPreviousCharacterDirectionActive = serializedObject.FindProperty ("adjustCameraToPreviousCharacterDirectionActive");
		useSmoothCameraTransitionBetweenCharacters = serializedObject.FindProperty ("useSmoothCameraTransitionBetweenCharacters");
		smoothCameraTransitionBetweenCharactersSpeed = serializedObject.FindProperty ("smoothCameraTransitionBetweenCharactersSpeed");
		setNewCharacterAlwaysInPreviousCharacterView = serializedObject.FindProperty ("setNewCharacterAlwaysInPreviousCharacterView");
		setNewCharacterAlwaysInThirdPerson = serializedObject.FindProperty ("setNewCharacterAlwaysInThirdPerson");
		setNewCharacterAlwaysInFirstPerson = serializedObject.FindProperty ("setNewCharacterAlwaysInFirstPerson");

		useMultipleTargetFov = serializedObject.FindProperty ("useMultipleTargetFov");
		multipleTargetsMinFov = serializedObject.FindProperty ("multipleTargetsMinFov");
		multipleTargetsMaxFov = serializedObject.FindProperty ("multipleTargetsMaxFov");
		multipleTargetsFovSpeed = serializedObject.FindProperty ("multipleTargetsFovSpeed");
		useMultipleTargetsYPosition = serializedObject.FindProperty ("useMultipleTargetsYPosition");
		multipleTargetsYPositionSpeed = serializedObject.FindProperty ("multipleTargetsYPositionSpeed");
		multipleTargetsMaxHeight = serializedObject.FindProperty ("multipleTargetsMaxHeight");

		multipleTargetsHeightMultiplier = serializedObject.FindProperty ("multipleTargetsHeightMultiplier");

		maxMultipleTargetHeight = serializedObject.FindProperty ("maxMultipleTargetHeight");
		minMultipleTargetHeight = serializedObject.FindProperty ("minMultipleTargetHeight");

		showCameraGizmo = serializedObject.FindProperty ("settings.showCameraGizmo");
		gizmoRadius = serializedObject.FindProperty ("settings.gizmoRadius");
		gizmoLabelColor = serializedObject.FindProperty ("settings.gizmoLabelColor");
		linesColor = serializedObject.FindProperty ("settings.linesColor");
		lookAtTargetEnabled = serializedObject.FindProperty ("lookAtTargetEnabled");
		canActivateLookAtTargetEnabled = serializedObject.FindProperty ("canActivateLookAtTargetEnabled");
		canActiveLookAtTargetOnLockedCamera = serializedObject.FindProperty ("canActiveLookAtTargetOnLockedCamera");
		lookAtTargetTransform = serializedObject.FindProperty ("lookAtTargetTransform");
		targetToLook = serializedObject.FindProperty ("targetToLook");
		maxDistanceToFindTarget = serializedObject.FindProperty ("maxDistanceToFindTarget");
		useLayerToSearchTargets = serializedObject.FindProperty ("useLayerToSearchTargets");
		layerToLook = serializedObject.FindProperty ("layerToLook");
		lookOnlyIfTargetOnScreen = serializedObject.FindProperty ("lookOnlyIfTargetOnScreen");
		checkObstaclesToTarget = serializedObject.FindProperty ("checkObstaclesToTarget");
		getClosestToCameraCenter = serializedObject.FindProperty ("getClosestToCameraCenter");
		useMaxDistanceToCameraCenter = serializedObject.FindProperty ("useMaxDistanceToCameraCenter");
		maxDistanceToCameraCenter = serializedObject.FindProperty ("maxDistanceToCameraCenter");
		activateStrafeModeIfNoTargetsToLookFound = serializedObject.FindProperty ("activateStrafeModeIfNoTargetsToLookFound");
		searchPointToLookComponents = serializedObject.FindProperty ("searchPointToLookComponents");
		pointToLookComponentsLayer = serializedObject.FindProperty ("pointToLookComponentsLayer");
		useTimeToStopAimAssist = serializedObject.FindProperty ("useTimeToStopAimAssist");
		timeToStopAimAssist = serializedObject.FindProperty ("timeToStopAimAssist");
		useTimeToStopAimAssistLockedCamera = serializedObject.FindProperty ("useTimeToStopAimAssistLockedCamera");
		timeToStopAimAssistLockedCamera = serializedObject.FindProperty ("timeToStopAimAssistLockedCamera");
		canChangeTargetToLookWithCameraAxis = serializedObject.FindProperty ("canChangeTargetToLookWithCameraAxis");
		minimumCameraDragToChangeTargetToLook = serializedObject.FindProperty ("minimumCameraDragToChangeTargetToLook");
		waitTimeToNextChangeTargetToLook = serializedObject.FindProperty ("waitTimeToNextChangeTargetToLook");
		useOnlyHorizontalCameraDragValue = serializedObject.FindProperty ("useOnlyHorizontalCameraDragValue");
		lookAtTargetSpeed = serializedObject.FindProperty ("lookAtTargetSpeed");
		lookCloserAtTargetSpeed = serializedObject.FindProperty ("lookCloserAtTargetSpeed");
		lookAtTargetSpeed2_5dView = serializedObject.FindProperty ("lookAtTargetSpeed2_5dView");
		lookAtTargetSpeedOthersLockedViews = serializedObject.FindProperty ("lookAtTargetSpeedOthersLockedViews");
		useLookTargetIcon = serializedObject.FindProperty ("useLookTargetIcon");
		lookAtTargetIcon = serializedObject.FindProperty ("lookAtTargetIcon");
		lookAtTargetRegularIconGameObject = serializedObject.FindProperty ("lookAtTargetRegularIconGameObject");
		lookAtTargetIconWhenNotAiming = serializedObject.FindProperty ("lookAtTargetIconWhenNotAiming");
		tagToLookList = serializedObject.FindProperty ("tagToLookList");
		lookAtBodyPartsOnCharacters = serializedObject.FindProperty ("lookAtBodyPartsOnCharacters");
		bodyPartsToLook = serializedObject.FindProperty ("bodyPartsToLook");
		useObjectToGrabFoundShader = serializedObject.FindProperty ("useObjectToGrabFoundShader");
		objectToGrabFoundShader = serializedObject.FindProperty ("objectToGrabFoundShader");
		shaderOutlineWidth = serializedObject.FindProperty ("shaderOutlineWidth");
		shaderOutlineColor = serializedObject.FindProperty ("shaderOutlineColor");
		useRemoteEventsOnLockOn = serializedObject.FindProperty ("useRemoteEventsOnLockOn");
		remoteEventOnLockOnStart = serializedObject.FindProperty ("remoteEventOnLockOnStart");
		remoteEventOnLockOnEnd = serializedObject.FindProperty ("remoteEventOnLockOnEnd");
		useEventsOnLockOn = serializedObject.FindProperty ("useEventsOnLockOn");
		eventOnLockOnStart = serializedObject.FindProperty ("eventOnLockOnStart");
		eventOnLockOnEnd = serializedObject.FindProperty ("eventOnLockOnEnd");

		layerToCheckPossibleTargetsBelowCursor = serializedObject.FindProperty ("layerToCheckPossibleTargetsBelowCursor");

		useSmoothCameraFollow = serializedObject.FindProperty ("useSmoothCameraFollow");
		smoothCameraFollowSpeed = serializedObject.FindProperty ("smoothCameraFollowSpeed");
		smoothCameraFollowSpeedOnAim = serializedObject.FindProperty ("smoothCameraFollowSpeedOnAim");
		smoothCameraFollowMaxDistance = serializedObject.FindProperty ("smoothCameraFollowMaxDistance");
		smoothCameraFollowMaxDistanceSpeed = serializedObject.FindProperty ("smoothCameraFollowMaxDistanceSpeed");

		showDebugPrint = serializedObject.FindProperty ("showDebugPrint");

		camera = (playerCamera)target;
	}

	void OnSceneGUI ()
	{   
		if (camera.settings.showCameraGizmo) {
			style.alignment = TextAnchor.MiddleCenter;

			if (camera.gameObject == Selection.activeGameObject) {
				for (int i = 0; i < camera.playerCameraStates.Count; i++) {
					if (camera.playerCameraStates [i].showGizmo) {
						style.normal.textColor = camera.settings.gizmoLabelColor;

						Handles.Label (camera.gameObject.transform.position + camera.playerCameraStates [i].pivotPositionOffset
						+ camera.playerCameraStates [i].camPositionOffset, camera.playerCameraStates [i].Name, style);						
					}
				}
			}    
		}
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Camera Components", "window");
		EditorGUILayout.PropertyField (playerCameraTransform);
		EditorGUILayout.PropertyField (mainCamera);
		EditorGUILayout.PropertyField (mainCameraTransform);
		EditorGUILayout.PropertyField (pivotCameraTransform);
		EditorGUILayout.PropertyField (pivotLeanTransform);
		EditorGUILayout.PropertyField (pivotLeanChildTransform);

		EditorGUILayout.PropertyField (mainCameraParent);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (cameraRotationInputEnabled);
		EditorGUILayout.PropertyField (thirdPersonVerticalRotationSpeed);
		EditorGUILayout.PropertyField (thirdPersonHorizontalRotationSpeed);
		EditorGUILayout.PropertyField (firstPersonVerticalRotationSpeed);
		EditorGUILayout.PropertyField (firstPersonHorizontalRotationSpeed);

		EditorGUILayout.PropertyField (smoothBetweenState);
		EditorGUILayout.PropertyField (maxCheckDist);
		EditorGUILayout.PropertyField (movementLerpSpeed);

		EditorGUILayout.PropertyField (extraCameraCollisionDistance);
		EditorGUILayout.PropertyField (cameraCollisionAlwaysActive);

		EditorGUILayout.PropertyField (usePivotCameraCollisionEnabled);
		if (usePivotCameraCollisionEnabled.boolValue) {
			EditorGUILayout.PropertyField (pivotCameraCollisionHeightOffset);
		}

		EditorGUILayout.PropertyField (changeCameraViewEnabled);
		EditorGUILayout.PropertyField (aimSide);
		EditorGUILayout.PropertyField (changeCameraSideActive);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Camera Bullet Time Settings", "window");
		EditorGUILayout.PropertyField (regularMovementOnBulletTime);
		EditorGUILayout.PropertyField (useUnscaledTimeOnBulletTime);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Follow Position Settings", "window");

		EditorGUILayout.PropertyField (useSmoothCameraFollow);
		if (useSmoothCameraFollow.boolValue) {
			EditorGUILayout.PropertyField (smoothCameraFollowSpeed);
			EditorGUILayout.PropertyField (smoothCameraFollowSpeedOnAim);
			EditorGUILayout.PropertyField (smoothCameraFollowMaxDistance);
			EditorGUILayout.PropertyField (smoothCameraFollowMaxDistanceSpeed);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Smooth Camera Rotation Settings", "window");
		EditorGUILayout.PropertyField (useSmoothCameraRotation);
		if (useSmoothCameraRotation.boolValue) {
			EditorGUILayout.PropertyField (useSmoothCameraRotationThirdPerson);
			if (useSmoothCameraRotationThirdPerson.boolValue) {
				EditorGUILayout.PropertyField (smoothCameraRotationSpeedVerticalThirdPerson);
				EditorGUILayout.PropertyField (smoothCameraRotationSpeedHorizontalThirdPerson);
			}
			EditorGUILayout.PropertyField (useSmoothCameraRotationFirstPerson);
			if (useSmoothCameraRotationFirstPerson.boolValue) {
				EditorGUILayout.PropertyField (smoothCameraRotationSpeedVerticalFirstPerson);
				EditorGUILayout.PropertyField (smoothCameraRotationSpeedHorizontalFirstPerson);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zoom Settings", "window");
		EditorGUILayout.PropertyField (zoomSpeed);
		EditorGUILayout.PropertyField (fovChangeSpeed);

		EditorGUILayout.PropertyField (thirdPersonVerticalRotationSpeedZoomIn);
		EditorGUILayout.PropertyField (thirdPersonHorizontalRotationSpeedZoomIn);
		EditorGUILayout.PropertyField (firstPersonVerticalRotationSpeedZoomIn);
		EditorGUILayout.PropertyField (firstPersonHorizontalRotationSpeedZoomIn);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Mouse Wheel Settings", "window");
		EditorGUILayout.PropertyField (moveCameraPositionWithMouseWheelActive);
		if (moveCameraPositionWithMouseWheelActive.boolValue) {
			EditorGUILayout.PropertyField (moveCameraPositionForwardWithMouseWheelSpeed);
			EditorGUILayout.PropertyField (moveCameraPositionBackwardWithMouseWheelSpeed);

			EditorGUILayout.PropertyField (reverseMouseWheelDirectionEnabled);

			EditorGUILayout.PropertyField (maxExtraDistanceOnThirdPerson);
			EditorGUILayout.PropertyField (useCameraMouseWheelStates);
			if (useCameraMouseWheelStates.boolValue) {
				
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Mouse Wheel Settings", "window");
				showCameraMouseWheelStatesList (cameraMouseWheelStatesList);
				GUILayout.EndVertical ();
			} else {
				EditorGUILayout.PropertyField (minDistanceToChangeToFirstPerson);
			}
		}
		GUILayout.EndVertical ();

		if (using2_5ViewActive.boolValue || cameraType.enumValueIndex == 1) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("2.5d Camera Settings", "window");

			EditorGUILayout.PropertyField (useCameraLimit);
			if (useCameraLimit.boolValue) {

				EditorGUILayout.PropertyField (useWidthLimit);
				if (useWidthLimit.boolValue) {
					EditorGUILayout.PropertyField (widthLimitRight);
					EditorGUILayout.PropertyField (widthLimitLeft);
				}

				EditorGUILayout.PropertyField (useHeightLimit);
				if (useHeightLimit.boolValue) {
					EditorGUILayout.PropertyField (heightLimitUpper);
					EditorGUILayout.PropertyField (heightLimitLower);
				}

				EditorGUILayout.PropertyField (useDepthLimit);
				if (useDepthLimit.boolValue) {
					EditorGUILayout.PropertyField (depthLimitFront);
					EditorGUILayout.PropertyField (depthLimitBackward);
				}

				EditorGUILayout.PropertyField (currentCameraLimitPosition);
			}
			EditorGUILayout.PropertyField (clampAimDirections);
			EditorGUILayout.PropertyField (numberOfAimDirections);
			EditorGUILayout.PropertyField (minDistanceToCenterClampAim);
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zero Gravity Camera Settings", "window");
		EditorGUILayout.PropertyField (canRotateForwardOnZeroGravityModeOn);
		EditorGUILayout.PropertyField (rotateForwardOnZeroGravitySpeed);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Reset Camera Rotation Settings", "window");
		EditorGUILayout.PropertyField (resetCameraRotationAfterTime);
		if (resetCameraRotationAfterTime.boolValue) {
			EditorGUILayout.PropertyField (timeToResetCameraRotation);
		}
		EditorGUILayout.PropertyField (resetCameraRotationSpeed);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Set Extra Camera Rotation Settings", "window");
		EditorGUILayout.PropertyField (setExtraCameraRotationEnabled);
		if (setExtraCameraRotationEnabled.boolValue) {
			EditorGUILayout.PropertyField (extraCameraRotationAmount);
			EditorGUILayout.PropertyField (extraCameraRotationSpeed);
			EditorGUILayout.PropertyField (useCameraForwardDirection);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Player Camera States", "window");
		EditorGUILayout.PropertyField (currentStateName);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (defaultThirdPersonStateName);
		EditorGUILayout.PropertyField (defaultFirstPersonStateName);
		EditorGUILayout.PropertyField (defaultThirdPersonCrouchStateName);
		EditorGUILayout.PropertyField (defaultFirstPersonCrouchStateName);
		EditorGUILayout.PropertyField (defaultThirdPersonAimRightStateName);
		EditorGUILayout.PropertyField (defaultThirdPersonAimLeftStateName);
		EditorGUILayout.PropertyField (defaultMoveCameraAwayStateName);
		EditorGUILayout.PropertyField (defaultLockedCameraStateName);
		EditorGUILayout.PropertyField (defaultAICameraStateName);
		EditorGUILayout.PropertyField (defaultVehiclePassengerStateName);

		EditorGUILayout.Space ();

		showPlayerCameraStates (playerCameraStates);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Camera States Events Settings", "window");
		EditorGUILayout.PropertyField (useEventsOnCameraStateChange);
		if (useEventsOnCameraStateChange.boolValue) {

			EditorGUILayout.Space ();

			showCameraStateEventInfoList (cameraStateEventInfoList);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current Camera States", "window");
		GUILayout.Label ("Camera Mode\t\t" + camera.cameraType);
		GUILayout.Label ("On Ground\t\t\t" + onGround.boolValue);
		GUILayout.Label ("Aiming 3rd Person\t\t" + aimingInThirdPerson.boolValue);
		GUILayout.Label ("Crouching\t\t\t" + crouching.boolValue);
		GUILayout.Label ("Lean Active \t\t" + leanActive.boolValue);
		GUILayout.Label ("Lean To Right \t\t" + leanToRightActive.boolValue);
		GUILayout.Label ("First Person Active\t\t" + firstPersonActive.boolValue);
		GUILayout.Label ("Using Zoom On\t\t" + usingZoomOn.boolValue);
		GUILayout.Label ("Using Zoom Off\t\t" + usingZoomOff.boolValue);
		GUILayout.Label ("Camera Can Rotate\t\t" + cameraCanRotate.boolValue);
		GUILayout.Label ("Camera Can Be Used\t\t" + cameraCanBeUsed.boolValue);
		GUILayout.Label ("Looking At Target\t\t" + lookingAtTarget.boolValue);
		GUILayout.Label ("Looking At Point\t\t" + lookingAtPoint.boolValue);
		GUILayout.Label ("Using 2.5d View\t\t" + using2_5ViewActive.boolValue);
		GUILayout.Label ("Using Top Down View\t\t" + useTopDownView.boolValue);
		GUILayout.Label ("Horizontal Input\t\t" + horizontalInput.floatValue);
		GUILayout.Label ("Vertical Input\t\t" + verticalInput.floatValue);
		GUILayout.Label ("Look Angle\t\t\t" + lookAngle.vector2Value);
		GUILayout.Label ("Navmesh Active\t\t" + playerNavMeshEnabled.boolValue);
		GUILayout.Label ("Following Multiple Targets\t" + followingMultipleTargets.boolValue);
		GUILayout.Label ("Custom Aim Active\t" + useCustomThirdPersonAimActive.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Locked Camera Elements", "window");
		EditorGUILayout.PropertyField (lockedCameraSystemPrefab);
		EditorGUILayout.PropertyField (lockedCameraLimitSystemPrefab);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add New Locked Camera System")) {
			camera.addNewLockedCameraSystemToLevel ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add New Locked Camera Limit System")) {
			camera.addNewLockedCameraLimitSystemToLevel ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Locked Camera Prefabs List", "window");
		showLockedCameraPrefabsTypesList (lockedCameraPrefabsTypesList);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Noise List", "window");
		showCameraNoiseStateList (cameraNoiseStateList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (usedByAI);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (showSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Settings")) {
			showSettings.boolValue = !showSettings.boolValue;
		}
		if (showLookTargetSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Look At Target Settings")) {
			showLookTargetSettings.boolValue = !showLookTargetSettings.boolValue;
		}

		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndVertical ();

		if (showSettings.boolValue) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Basic Camera Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window");
			EditorGUILayout.PropertyField (layer, new GUIContent ("Collision Layer"));
			EditorGUILayout.PropertyField (useAcelerometer);
			EditorGUILayout.PropertyField (zoomEnabled);
			EditorGUILayout.PropertyField (moveAwayCameraEnabled);
			EditorGUILayout.PropertyField (enableMoveAwayInAir);
			EditorGUILayout.PropertyField (enableShakeCamera);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			defBackgroundColor = GUI.backgroundColor;
			EditorGUILayout.BeginVertical ();
			if (showElements.boolValue) {
				GUI.backgroundColor = Color.gray;
				currentButtonString = "Hide Camera Components";
			} else {
				GUI.backgroundColor = defBackgroundColor;
				currentButtonString = "Show Camera Components";
			}

			if (GUILayout.Button (currentButtonString)) {
				showElements.boolValue = !showElements.boolValue;
			}

			GUI.backgroundColor = defBackgroundColor;
			EditorGUILayout.EndVertical ();

			if (showElements.boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Locked Camera Elements", "window");
				EditorGUILayout.PropertyField (lockedCameraElementsParent);
				EditorGUILayout.PropertyField (lockedMainCameraTransform);
				EditorGUILayout.PropertyField (lockedCameraAxis);
				EditorGUILayout.PropertyField (lockedCameraPosition);
				EditorGUILayout.PropertyField (lockedCameraPivot);
				EditorGUILayout.PropertyField (lookCameraParent);
				EditorGUILayout.PropertyField (lookCameraPivot);
				EditorGUILayout.PropertyField (lookCameraDirection);
				EditorGUILayout.PropertyField (clampAimDirectionTransform);
				EditorGUILayout.PropertyField (lookDirectionTransform);
				EditorGUILayout.PropertyField (auxLockedCameraAxis);
				EditorGUILayout.PropertyField (setTransparentSurfacesManager);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Player Camera Elements", "window");
				EditorGUILayout.PropertyField (playerControllerGameObject);
				EditorGUILayout.PropertyField (playerInput);
				EditorGUILayout.PropertyField (playerControllerManager);
				EditorGUILayout.PropertyField (powersManager);
				EditorGUILayout.PropertyField (gravityManager);
				EditorGUILayout.PropertyField (headBobManager);
				EditorGUILayout.PropertyField (grabObjectsManager);
				EditorGUILayout.PropertyField (scannerManager);
				EditorGUILayout.PropertyField (weaponsManager);
				EditorGUILayout.PropertyField (playerNavMeshManager);
				EditorGUILayout.PropertyField (characterStateIconManager);

				EditorGUILayout.PropertyField (mainCollider);
				EditorGUILayout.PropertyField (mainCameraCaptureSystem);

				EditorGUILayout.PropertyField (targetToFollow);
				EditorGUILayout.PropertyField (hipsTransform);
				GUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Event On Moving Locked Camera Settings", "window");
			EditorGUILayout.PropertyField (useEventOnMovingLockedCamera);
			EditorGUILayout.PropertyField (useEventOnFreeCamera);
			if (useEventOnMovingLockedCamera.boolValue || useEventOnFreeCamera.boolValue) {
				EditorGUILayout.PropertyField (eventOnStartMovingLockedCamera);
				EditorGUILayout.PropertyField (eventOnKeepMovingLockedCamera);
				EditorGUILayout.PropertyField (eventOnStopMovingLockedCamera);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Event On Third/First Person View Change Settings", "window");
			EditorGUILayout.PropertyField (useEventOnThirdFirstPersonViewChange);
			if (useEventOnThirdFirstPersonViewChange.boolValue) {
				EditorGUILayout.PropertyField (setFirstPersonEvent);
				EditorGUILayout.PropertyField (setThirdPersonEvent);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Change Third/First Person View In Editor Events Settings", "window");
			EditorGUILayout.PropertyField (setThirdPersonInEditorEvent);
			EditorGUILayout.PropertyField (setFirstPersonInEditorEvent);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Camera List Settings", "window");
			showSimpleList (cameraList, "Camera List");
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Use Camera Reticle Settings", "window");
			EditorGUILayout.PropertyField (updateReticleActiveState);
			if (updateReticleActiveState.boolValue) {
				EditorGUILayout.PropertyField (useCameraReticleThirdPerson);
				EditorGUILayout.PropertyField (useCameraReticleFirstPerson);
				EditorGUILayout.PropertyField (cameraReticleGameObject);
				EditorGUILayout.PropertyField (mainCameraReticleGameObject);

				EditorGUILayout.Space ();

				GUILayout.Label ("Camera Reticle Active\t\t" + cameraReticleCurrentlyActive.boolValue);
				GUILayout.Label ("External Reticle Active\t" + externalReticleCurrentlyActive.boolValue);
			}

			EditorGUILayout.Space ();
			GUILayout.BeginVertical ("Use Camera Reticle Settings", "window");

			EditorGUILayout.PropertyField (mainCustomReticleParent);

			EditorGUILayout.Space ();

			showCustomReticleInfoList (customReticleInfoList);
			GUILayout.EndVertical ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Adjust Camera To Previous Character View Settings", "window");
			EditorGUILayout.PropertyField (adjustCameraToPreviousCharacterDirectionActive);
			EditorGUILayout.PropertyField (useSmoothCameraTransitionBetweenCharacters);	
			EditorGUILayout.PropertyField (smoothCameraTransitionBetweenCharactersSpeed);	

			EditorGUILayout.PropertyField (setNewCharacterAlwaysInPreviousCharacterView);	
			if (!setNewCharacterAlwaysInPreviousCharacterView.boolValue) {
				EditorGUILayout.PropertyField (setNewCharacterAlwaysInThirdPerson);
				EditorGUILayout.PropertyField (setNewCharacterAlwaysInFirstPerson);	
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Local Multiplayer Single Camera Settings", "window");
			EditorGUILayout.PropertyField (useMultipleTargetFov);
			if (useMultipleTargetFov.boolValue) {
				EditorGUILayout.PropertyField (multipleTargetsMinFov);
				EditorGUILayout.PropertyField (multipleTargetsMaxFov);	
				EditorGUILayout.PropertyField (multipleTargetsFovSpeed);	
			}

			EditorGUILayout.PropertyField (useMultipleTargetsYPosition);
			if (useMultipleTargetsYPosition.boolValue) {
				EditorGUILayout.PropertyField (multipleTargetsYPositionSpeed);
				EditorGUILayout.PropertyField (multipleTargetsMaxHeight);	
				EditorGUILayout.PropertyField (multipleTargetsHeightMultiplier);	
				EditorGUILayout.PropertyField (maxMultipleTargetHeight);	
				EditorGUILayout.PropertyField (minMultipleTargetHeight);	
			}

			GUILayout.EndVertical ();
		
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window");
			EditorGUILayout.PropertyField (showCameraGizmo);
			if (showCameraGizmo.boolValue) {
				EditorGUILayout.PropertyField (gizmoRadius);
				EditorGUILayout.PropertyField (gizmoLabelColor);
				EditorGUILayout.PropertyField (linesColor);
			}
			EditorGUILayout.PropertyField (showDebugPrint);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showLookTargetSettings.boolValue) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Look At Target Camera Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Look At Target Settings", "window");
			EditorGUILayout.PropertyField (lookAtTargetEnabled);
			if (lookAtTargetEnabled.boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Main Settings", "window");
				EditorGUILayout.PropertyField (canActivateLookAtTargetEnabled);
				EditorGUILayout.PropertyField (canActiveLookAtTargetOnLockedCamera);

				EditorGUILayout.PropertyField (lookAtTargetTransform);
				EditorGUILayout.PropertyField (targetToLook);

				EditorGUILayout.PropertyField (maxDistanceToFindTarget);

				EditorGUILayout.PropertyField (useLayerToSearchTargets);	
				if (useLayerToSearchTargets.boolValue) {
					EditorGUILayout.PropertyField (layerToLook);
				}
				EditorGUILayout.PropertyField (lookOnlyIfTargetOnScreen);
				EditorGUILayout.PropertyField (checkObstaclesToTarget);
				EditorGUILayout.PropertyField (getClosestToCameraCenter);
				if (getClosestToCameraCenter.boolValue) {
					EditorGUILayout.PropertyField (useMaxDistanceToCameraCenter);
					if (useMaxDistanceToCameraCenter.boolValue) {
						EditorGUILayout.PropertyField (maxDistanceToCameraCenter);
					}
				}

				EditorGUILayout.PropertyField (activateStrafeModeIfNoTargetsToLookFound);
				EditorGUILayout.PropertyField (layerToCheckPossibleTargetsBelowCursor);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Search Point To Look Objects Settings", "window");
				EditorGUILayout.PropertyField (searchPointToLookComponents);
				if (searchPointToLookComponents.boolValue) {
					EditorGUILayout.PropertyField (pointToLookComponentsLayer);
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Stop Look At Target Aim Assist Settings", "window");
				EditorGUILayout.PropertyField (useTimeToStopAimAssist);
				if (useTimeToStopAimAssist.boolValue) {
					EditorGUILayout.PropertyField (timeToStopAimAssist);
				}
				EditorGUILayout.PropertyField (useTimeToStopAimAssistLockedCamera);
				if (useTimeToStopAimAssistLockedCamera.boolValue) {
					EditorGUILayout.PropertyField (timeToStopAimAssistLockedCamera);
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Change Between Target Settings", "window");
				EditorGUILayout.PropertyField (canChangeTargetToLookWithCameraAxis);
				if (canChangeTargetToLookWithCameraAxis.boolValue) {
					EditorGUILayout.PropertyField (minimumCameraDragToChangeTargetToLook);
					EditorGUILayout.PropertyField (waitTimeToNextChangeTargetToLook);
					EditorGUILayout.PropertyField (useOnlyHorizontalCameraDragValue);
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Look At Target Speed Settings", "window");
				EditorGUILayout.PropertyField (lookAtTargetSpeed);
				EditorGUILayout.PropertyField (lookCloserAtTargetSpeed);
				EditorGUILayout.PropertyField (lookAtTargetSpeed2_5dView);
				EditorGUILayout.PropertyField (lookAtTargetSpeedOthersLockedViews);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Icon Settings", "window");
				EditorGUILayout.PropertyField (useLookTargetIcon);
				if (useLookTargetIcon.boolValue) {
					EditorGUILayout.PropertyField (lookAtTargetIcon);
					EditorGUILayout.PropertyField (lookAtTargetRegularIconGameObject);
					EditorGUILayout.PropertyField (lookAtTargetIconWhenNotAiming);
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Tag To Look List", "window");
				showSimpleList (tagToLookList, "Tag To Look List");
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Look At Characters Body Parts Settings", "window");
				EditorGUILayout.PropertyField (lookAtBodyPartsOnCharacters);

				if (lookAtBodyPartsOnCharacters.boolValue) {
					
					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Body Parts To Look List", "window");
					showSimpleList (bodyPartsToLook, "Body Parts To Look List");
					GUILayout.EndVertical ();
				}

				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Look At Characters Body Parts Settings", "window");
				EditorGUILayout.PropertyField (useObjectToGrabFoundShader);

				if (useObjectToGrabFoundShader.boolValue) {
					EditorGUILayout.PropertyField (objectToGrabFoundShader);
					EditorGUILayout.PropertyField (shaderOutlineWidth);
					EditorGUILayout.PropertyField (shaderOutlineColor);
				}

				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Remote Events On Lock On Settings", "window");
				EditorGUILayout.PropertyField (useRemoteEventsOnLockOn);
				if (useRemoteEventsOnLockOn.boolValue) {
					
					showSimpleList (remoteEventOnLockOnStart, "Remote Events On Lock Start");

					EditorGUILayout.Space ();

					showSimpleList (remoteEventOnLockOnEnd, "Remote Events On Lock End");
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Events On Lock On Settings", "window");
				EditorGUILayout.PropertyField (useEventsOnLockOn);
				if (useEventsOnLockOn.boolValue) {

					EditorGUILayout.PropertyField (eventOnLockOnStart);

					EditorGUILayout.Space ();

					EditorGUILayout.PropertyField (eventOnLockOnEnd);
				}
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}

		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
		EditorGUILayout.Space ();

		if (!Application.isPlaying) {
			//set in the inspector the current camera type
			if (!checkCamera) {
				if (camera.firstPersonActive) {
					currentCamera = "FIRST PERSON";
				} else {
					currentCamera = "THIRD PERSON";
				}
				checkCamera = true;
			}

			GUILayout.Label ("Current Camera: " + currentCamera);
			if (GUILayout.Button ("Set First Person")) {
				camera.setFirstPersonInEditor ();
				currentCamera = "FIRST PERSON";
			}

			if (GUILayout.Button ("Set Third Person")) {
				camera.setThirdPersonInEditor ();
				currentCamera = "THIRD PERSON";
			}
		}
	}

	void showCameraStateElementInfo (SerializedProperty list, int stateIndex)
	{
		GUILayout.BeginVertical ();
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("camPositionOffset"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pivotPositionOffset"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pivotParentPositionOffset"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraCollisionActive"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pivotRotationOffset"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreCameraShakeOnRunState"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Rotation Limit Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("yLimits"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useYLimitsOnLookAtTargetActive"));
		if (list.FindPropertyRelative ("useYLimitsOnLookAtTargetActive").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("YLimitsOnLookAtTargetActive"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("FOV Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("initialFovValue"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("fovTransitionSpeed"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxFovValue"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("minFovValue"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Look In Player Direction Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookInPlayerDirection"));
		if (list.FindPropertyRelative ("lookInPlayerDirection").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookInPlayerDirectionSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("allowRotationWithInput"));
			if (list.FindPropertyRelative ("allowRotationWithInput").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("timeToResetRotationAfterInput"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Lean Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("leanEnabled"));
		if (list.FindPropertyRelative ("leanEnabled").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxLeanAngle"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("leanSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("leanResetSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("leanRaycastDistance"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("leanAngleOnSurfaceFound"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("leanHeight"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Head Track Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("headTrackActive"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showGizmo"));
		if (list.FindPropertyRelative ("showGizmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("gizmoColor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Camera State Values (Debug)")) {
			if (Application.isPlaying) {
				camera.updateCameraStateValuesOnEditor (stateIndex);
			}
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showPlayerCameraStates (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Player Camera States", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of States: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showCameraStateElementInfo (list.GetArrayElementAtIndex (i), i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}

				EditorGUILayout.Space ();

				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Amount: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}

	void showCameraMouseWheelStatesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Camera Mouse Wheel States List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of States: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showCameraMouseWheelStatesListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showCameraMouseWheelStatesListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isFirstPerson"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraDistanceRange"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentCameraState"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeCameraFromAboveStateWithName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeCameraFromBelowStateWithName"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeCameraIfDistanceChanged"));
		if (list.FindPropertyRelative ("changeCameraIfDistanceChanged").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("minCameraDistanceToChange"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeToAboveCameraState"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventFromAboveCameraState"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventFromBelowCameraState"));

		GUILayout.EndVertical ();
	}

	void showLockedCameraPrefabsTypesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Locked Camera Prefabs List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Prefabs: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Prefab")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showLockedCameraPrefabsTypesListElement (list.GetArrayElementAtIndex (i), i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("Add")) {
					camera.addNewLockedCameraPrefabTypeLevel (i);
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showLockedCameraPrefabsTypesListElement (SerializedProperty list, int lockedCameraIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("lockedCameraPrefab"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Camera To Scene")) {
			camera.addNewLockedCameraPrefabTypeLevel (lockedCameraIndex);
		}
		GUILayout.EndVertical ();
	}

	void showCameraNoiseStateList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Camera Noise State List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of States: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showCameraNoiseStateListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showCameraNoiseStateListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseAmount"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("roughness"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Test Noise")) {
			if (Application.isPlaying) {
				camera.setCameraNoiseState (list.FindPropertyRelative ("Name").stringValue);
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Stop Noise")) {
			if (Application.isPlaying) {
				camera.disableCameraNoiseState ();
			}
		}
	
		GUILayout.EndVertical ();
	}

	void showCameraStateEventInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Camera State Event Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of States: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showCameraStateEventInfoListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showCameraStateEventInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnCameraStart"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnCameraEnd"));
		GUILayout.EndVertical ();
	}

	void showCustomReticleInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Custom Reticle Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Reticles: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Reticle")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showCustomReticleInfoListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showCustomReticleInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("customReticleObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentReticle"));
		GUILayout.EndVertical ();
	}
}
#endif