using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerController))]
public class playerControllerEditor : Editor
{
	SerializedProperty jumpPower;
	SerializedProperty airSpeed;
	SerializedProperty airControl;

	SerializedProperty ignorePlayerExtraRotationOnAirEnabled;

	SerializedProperty stationaryTurnSpeed;
	SerializedProperty movingTurnSpeed;
	SerializedProperty autoTurnSpeed;
	SerializedProperty aimTurnSpeed;
	SerializedProperty thresholdAngleDifference;

	SerializedProperty useTurnSpeedOnAim;

	SerializedProperty capsuleHeightOnCrouch;
	SerializedProperty regularMovementOnBulletTime;

	SerializedProperty useMoveInpuptMagnitedeToForwardAmount;

	SerializedProperty baseLayerIndex;
	SerializedProperty inputHorizontalLerpSpeed;
	SerializedProperty inputVerticalLerpSpeed;
	SerializedProperty inputStrafeHorizontalLerpSpeed;
	SerializedProperty inputStrafeVerticalLerpSpeed;
	SerializedProperty animatorForwardInputLerpSpeed;
	SerializedProperty animatorTurnInputLerpSpeed;

	SerializedProperty animSpeedMultiplier;

	SerializedProperty overrideAnimationSpeedActive;

	SerializedProperty characterRadius;

	SerializedProperty minTimeToSetIdleOnStrafe;
	SerializedProperty currentIdleID;
	SerializedProperty currentStrafeID;
	SerializedProperty currentAirID;
	SerializedProperty currentCrouchID;

	SerializedProperty inputTankControlsHorizontalLerpSpeed;
	SerializedProperty inputTankControlsVerticalLerpSpeed;
	SerializedProperty inputTankControlsHorizontalStrafeLerpSpeed;
	SerializedProperty inputTankControlsVerticalStrafeLerpSpeed;
	SerializedProperty playerStatusID;
	SerializedProperty playerStatusIDLerpSpeed;


	SerializedProperty layer;
	SerializedProperty rayDistance;

	SerializedProperty useSphereRaycastForGroundDetection;
	
	SerializedProperty sphereCastRadius;
	SerializedProperty maxDistanceSphereCast;
	SerializedProperty sphereCastOffset;

	SerializedProperty maxExtraRaycastDistanceToGroundDetection;

	//
	//	SerializedProperty useCaspsuleCastForGroundDetection;
	//
	//	SerializedProperty capsuleCastRadius;
	//	SerializedProperty capsuleCastDistance;
	//	SerializedProperty caspsuleCastOffset;


	SerializedProperty walkSpeed;
	SerializedProperty increaseWalkSpeedEnabled;
	SerializedProperty increaseWalkSpeedValue;
	SerializedProperty holdButtonToKeepIncreasedWalkSpeed;
	SerializedProperty walkRunTip;
	SerializedProperty sprintEnabled;
	SerializedProperty changeCameraFovOnSprint;
	SerializedProperty shakeCameraOnSprintThirdPerson;
	SerializedProperty sprintThirdPersonCameraShakeName;
	SerializedProperty useSecondarySprintValues;
	SerializedProperty sprintVelocity;
	SerializedProperty sprintJumpPower;
	SerializedProperty sprintAirSpeed;
	SerializedProperty sprintAirControl;

	SerializedProperty runOnCrouchEnabled;

	SerializedProperty regularGroundAdherence;
	SerializedProperty slopesGroundAdherenceUp;
	SerializedProperty slopesGroundAdherenceDown;
	SerializedProperty maxRayDistanceRange;
	SerializedProperty maxSlopeRayDistance;
	SerializedProperty maxStairsRayDistance;
	SerializedProperty useMaxWalkSurfaceAngle;
	SerializedProperty maxWalkSurfaceAngle;
	SerializedProperty useMaxDistanceToCheckSurfaceAngle;
	SerializedProperty maxDistanceToCheckSurfaceAngle;

	SerializedProperty useMaxSlopeInclination;
	SerializedProperty maxSlopeInclination;
	SerializedProperty minSlopeInclination;

	SerializedProperty noAnimatorSpeed;
	SerializedProperty noAnimatorWalkMovementSpeed;
	SerializedProperty noAnimatorRunMovementSpeed;
	SerializedProperty noAnimatorCrouchMovementSpeed;
	SerializedProperty noAnimatorRunCrouchMovementSpeed;
	SerializedProperty noAnimatorStrafeMovementSpeed;
	SerializedProperty noAnimatorCanRun;
	SerializedProperty noAnimatorWalkBackwardMovementSpeed;
	SerializedProperty noAnimatorRunBackwardMovementSpeed;
	SerializedProperty noAnimatorCrouchBackwardMovementSpeed;
	SerializedProperty noAnimatorRunCrouchBackwardMovementSpeed;
	SerializedProperty noAnimatorStrafeBackwardMovementSpeed;
	SerializedProperty noAnimatorCanRunBackwards;
	SerializedProperty noAnimatorAirSpeed;
	SerializedProperty maxVelocityChange;
	SerializedProperty noAnimatorMovementSpeedMultiplier;
	SerializedProperty noAnimatorSlopesGroundAdherenceUp;
	SerializedProperty noAnimatorSlopesGroundAdherenceDown;
	SerializedProperty noAnimatorStairsGroundAdherence;

	SerializedProperty currentVelocity;
	SerializedProperty axisValues;
	//	SerializedProperty noAnimatorCurrentMovementSpeed;
	//	SerializedProperty currentVelocityChangeMagnitude;
	//	SerializedProperty characterVelocity;
	//	SerializedProperty moveInput;

	SerializedProperty useRootMotionActive;

	SerializedProperty rootMotionCurrentlyActive;

	SerializedProperty usingGenericModelActive;

	SerializedProperty noRootLerpSpeed;
	SerializedProperty noRootWalkMovementSpeed;
	SerializedProperty noRootRunMovementSpeed;
	SerializedProperty noRootSprintMovementSpeed;
	SerializedProperty noRootCrouchMovementSpeed;
	SerializedProperty noRootWalkStrafeMovementSpeed;
	SerializedProperty noRootRunStrafeMovementSpeed;

	SerializedProperty disableRootMotionTemporalyOnLandThirdPerson;
	SerializedProperty durationDisableRootMotionTemporalyOnLandThirdPerson;
	SerializedProperty waitToDisableRootMotionTemporalyOnLandThirdPerson;

	SerializedProperty useEventOnSprint;
	SerializedProperty eventOnStartSprint;
	SerializedProperty eventOnStopSprint;

	SerializedProperty lookAlwaysInCameraDirection;
	SerializedProperty lookInCameraDirectionIfLookingAtTarget;
	SerializedProperty lookOnlyIfMoving;
	SerializedProperty disableLookAlwaysInCameraDirectionIfIncreaseWalkSpeedActive;
	SerializedProperty defaultStrafeWalkSpeed;
	SerializedProperty defaultStrafeRunSpeed;
	SerializedProperty rotateDirectlyTowardCameraOnStrafe;
	SerializedProperty strafeLerpSpeed;

	SerializedProperty lookInCameraDirectionOnCrouchState;

	SerializedProperty characterMeshGameObject;
	SerializedProperty extraCharacterMeshGameObject;

	SerializedProperty checkCharacterMeshIfGeneratedOnStart;

	SerializedProperty objectToCheckCharacterIfGeneratedOnStart;

	SerializedProperty characterMeshesListToDisableOnEvent;

	SerializedProperty useEventsOnEnableDisableCharacterMeshes;
	SerializedProperty eventOnEnableCharacterMeshes;
	SerializedProperty eventOnDisableCharacterMeshes;
	SerializedProperty eventOnEnableCharacterMeshesOnEditor;
	SerializedProperty eventOnDisableCharacterMeshesOnEditor;

	SerializedProperty canMoveWhileAimFirstPerson;
	SerializedProperty canMoveWhileAimThirdPerson;
	SerializedProperty stairsMinValue;
	SerializedProperty stairsMaxValue;
	SerializedProperty stairsGroundAdherence;
	SerializedProperty checkStairsWithInclination;
	SerializedProperty minStairInclination;
	SerializedProperty maxStairInclination;
	SerializedProperty checkForStairAdherenceSystem;
	SerializedProperty currentStairAdherenceSystemMinValue;
	SerializedProperty currentStairAdherenceSystemMaxValue;
	SerializedProperty currentStairAdherenceSystemAdherenceValue;
	SerializedProperty lockedPlayerMovement;
	SerializedProperty tankModeRotationSpeed;
	SerializedProperty canMoveWhileAimLockedCamera;
	SerializedProperty crouchVerticalInput2_5dEnabled;

	SerializedProperty gravityMultiplier;
	SerializedProperty gravityForce;

	SerializedProperty useMaxFallSpeed;
	SerializedProperty maxFallSpeed;

	SerializedProperty zeroFrictionMaterial;
	SerializedProperty highFrictionMaterial;

	SerializedProperty enabledRegularJump;
	SerializedProperty enabledDoubleJump;
	SerializedProperty maxNumberJumpsInAir;
	SerializedProperty holdJumpSlowDownFallEnabled;
	SerializedProperty slowDownGravityMultiplier;
	SerializedProperty readyToJumpTime;

	SerializedProperty useNewDoubleJumpPower;
	SerializedProperty doubleJumpPower;

	SerializedProperty removeVerticalSpeedOnDoubleJump;

	SerializedProperty addJumpForceWhileButtonPressed;
	SerializedProperty addJumpForceDuration;
	SerializedProperty jumpForceAmontWhileButtonPressed;
	SerializedProperty useJumpForceOnAirJumpEnabled;
	SerializedProperty minWaitToAddJumpForceAfterButtonPressed;

	SerializedProperty stopCrouchOnJumpEnabled;

	SerializedProperty useEventOnJump;
	SerializedProperty eventOnJump;

	SerializedProperty fallDamageEnabled;
	SerializedProperty maxTimeInAirBeforeGettingDamage;
	SerializedProperty fallingDamageMultiplier;

	SerializedProperty minPlayerVelocityToApplyDamageThirdPerson;
	SerializedProperty minPlayerVelocityToApplyDamageFirstPerson;
	SerializedProperty callEventOnFallDamage;
	SerializedProperty minTimeOnAirToUseEvent;
	SerializedProperty eventOnFallDamage;

	SerializedProperty ignoreShieldOnFallDamage;

	SerializedProperty damageTypeIDOnFallDamage;

	SerializedProperty applyFallDamageEnabled;

	SerializedProperty callEventOnlyIfPlayerAlive;

	SerializedProperty useLandMark;
	SerializedProperty maxLandDistance;
	SerializedProperty minDistanceShowLandMark;
	SerializedProperty landMark;
	SerializedProperty landMark1;
	SerializedProperty landMark2;
	SerializedProperty canUseSphereMode;
	SerializedProperty canGetOnVehicles;
	SerializedProperty canDrive;

	SerializedProperty canCrouchWhenUsingWeaponsOnThirdPerson;
	SerializedProperty getUpIfJumpOnCrouchInThirdPerson;
	SerializedProperty getUpIfJumpOnCrouchInFirstPerson;
	SerializedProperty useAutoCrouch;
	SerializedProperty layerToCrouch;
	SerializedProperty raycastDistanceToAutoCrouch;
	SerializedProperty autoCrouchRayPosition;
	SerializedProperty secondRaycastOffset;
	SerializedProperty useObstacleDetectionToAvoidMovement;
	SerializedProperty obstacleDetectionToAvoidMovementRaycastDistance;
	SerializedProperty obstacleDetectionRaycastDistanceRightAndLeft;
	SerializedProperty obstacleDetectionRaycastHeightOffset;
	SerializedProperty obstacleDetectionToAvoidMovementLayermask;

	SerializedProperty forwardAnimatorName;
	SerializedProperty turnAnimatorName;
	SerializedProperty horizontalAnimatorName;
	SerializedProperty verticalAnimatorName;
	SerializedProperty horizontalStrafeAnimatorName;
	SerializedProperty verticalStrafeAnimatorName;
	SerializedProperty onGroundAnimatorName;
	SerializedProperty crouchAnimatorName;
	SerializedProperty movingAnimatorName;
	SerializedProperty jumpAnimatorName;
	SerializedProperty jumpLegAnimatorName;
	SerializedProperty strafeModeActiveAnimatorName;
	SerializedProperty movementInputActiveAnimatorName;
	SerializedProperty movementRelativeToCameraAnimatorName;
	SerializedProperty movementIDAnimatorName;
	SerializedProperty playerModeIDAnimatorName;
	SerializedProperty movementSpeedAnimatorName;
	SerializedProperty lastTimeInputPressedAnimatorName;
	SerializedProperty carryingWeaponAnimatorName;
	SerializedProperty aimingModeActiveAnimatorName;
	SerializedProperty playerStatusIDAnimatorName;
	SerializedProperty idleIDAnimatorName;
	SerializedProperty strafeIDAnimatorName;

	SerializedProperty crouchIDAnimatorName;

	SerializedProperty airIDAnimatorName;

	SerializedProperty airSpeedAnimatorName;

	SerializedProperty quickTurnRightDirectionName;
	SerializedProperty quickTurnLeftDirectionName;
	SerializedProperty quickTurnDirectionIDSpeedName;

	SerializedProperty shieldActiveAnimatorName;

	SerializedProperty playerCameraGameObject;
	SerializedProperty playerCameraTransform;

	SerializedProperty playerCameraManager;
	SerializedProperty headBobManager;
	SerializedProperty playerInput;

	SerializedProperty playerTransform;

	SerializedProperty weaponsManager;
	SerializedProperty healthManager;
	SerializedProperty IKSystemManager;
	SerializedProperty mainCameraTransform;
	SerializedProperty animator;
	SerializedProperty stepManager;
	SerializedProperty mainRigidbody;
	SerializedProperty gravityManager;
	SerializedProperty characterStateIconManager;
	SerializedProperty capsule;
	SerializedProperty mainCollider;
	SerializedProperty AIElements;
	SerializedProperty mainPlayerActionSystem;

	SerializedProperty showDebugPrint;

	SerializedProperty showGizmo;
	SerializedProperty gizmoColor;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;

	SerializedProperty crouchSlidingEnabled;
	SerializedProperty noAnimatorCrouchSlidingSpeed;
	SerializedProperty noAnimatorCrouchSlidingDuration;
	SerializedProperty noAnimatorCrouchSlidingLerpSpeed;
	SerializedProperty noAnimatorCrouchSlidingLerpDelay;
	SerializedProperty getUpAfterCrouchSlidingEnd;
	SerializedProperty keepCrouchSlidingOnInclinatedSurface;
	SerializedProperty minInclinitaonSurfaceAngleToCrouchcSliding;
	SerializedProperty crouchSlidingOnAirEnabled;
	SerializedProperty noAnimatorCrouchSlidingOnAirSpeed;
	SerializedProperty useNoAnimatorCrouchSlidingDurationOnAir;
	SerializedProperty noAnimatorCrouchSlidingDurationOnAir;
	SerializedProperty eventOnCrouchSlidingStart;
	SerializedProperty eventOnCrouchSlidingEnd;

	SerializedProperty useCrouchSlidingOnThirdPersonEnabled;

	SerializedProperty eventOnCrouchSlidingThirdPersonStart;
	SerializedProperty eventOnCrouchSlidingThirdPersonEnd;

	SerializedProperty airDashEnabled;
	SerializedProperty airDashForce;
	SerializedProperty airDashColdDown;
	SerializedProperty pauseGravityForce;
	SerializedProperty gravityForcePausedTime;
	SerializedProperty resetGravityForceOnDash;
	SerializedProperty useDashLimit;
	SerializedProperty dashLimit;
	SerializedProperty changeCameraFovOnDash;
	SerializedProperty cameraFovOnDash;
	SerializedProperty cameraFovOnDashSpeed;

	SerializedProperty useEventsOnAirDash;
	SerializedProperty eventOnAirDashThirdPerson;
	SerializedProperty eventOnAirDashFirstPerson;


	SerializedProperty zeroGravityMovementSpeed;
	SerializedProperty zeroGravityControlSpeed;
	SerializedProperty zeroGravityLookCameraSpeed;
	SerializedProperty useGravityDirectionLandMark;
	SerializedProperty forwardSurfaceRayPosition;
	SerializedProperty maxDistanceToAdjust;
	SerializedProperty pauseCheckOnGroundStateZG;
	SerializedProperty pushPlayerWhenZeroGravityModeIsEnabled;
	SerializedProperty pushZeroGravityEnabledAmount;
	SerializedProperty canMoveVerticallyOnZeroGravity;
	SerializedProperty canMoveVerticallyAndHorizontalZG;
	SerializedProperty zeroGravitySpeedMultiplier;
	SerializedProperty zeroGravityModeVerticalSpeed;
	SerializedProperty freeFloatingMovementSpeed;
	SerializedProperty freeFloatingControlSpeed;
	SerializedProperty pauseCheckOnGroundStateFF;
	SerializedProperty canMoveVerticallyOnFreeFloating;
	SerializedProperty canMoveVerticallyAndHorizontalFF;
	SerializedProperty freeFloatingSpeedMultiplier;
	SerializedProperty pushFreeFloatingModeEnabledAmount;
	SerializedProperty freeFloatingModeVerticalSpeed;
	SerializedProperty useMaxAngleToCheckOnGroundStateZGFF;
	SerializedProperty maxAngleToChekOnGroundStateZGFF;

	SerializedProperty currentExternalControllerBehavior;

	SerializedProperty externalControllBehaviorActive;

	SerializedProperty mainExternalControllerBehaviorManager;

	SerializedProperty playerOnGround;
	SerializedProperty isMoving;

	SerializedProperty movingOnPlatformActive;

	SerializedProperty jump;
	SerializedProperty running;
	SerializedProperty crouching;
	SerializedProperty canMove;

	SerializedProperty moveIputPaused;

	SerializedProperty canMoveAI;

	SerializedProperty slowingFall;
	SerializedProperty isDead;
	SerializedProperty playerSetAsChildOfParent;
	SerializedProperty useRelativeMovementToLockedCamera;
	SerializedProperty ladderFound;
	SerializedProperty ignoreCameraDirectionOnMovement;
	SerializedProperty strafeModeActive;

	SerializedProperty actionActive;
	SerializedProperty adhereToGround;
	SerializedProperty distanceToGround;
	SerializedProperty hitAngle;
	SerializedProperty slopeFound;
	SerializedProperty movingOnSlopeUp;
	SerializedProperty movingOnSlopeDown;
	SerializedProperty stairsFound;
	SerializedProperty stairAdherenceSystemDetected;
	SerializedProperty wallRunningActive;
	SerializedProperty crouchSlidingActive;
	SerializedProperty aimingInThirdPerson;
	SerializedProperty aimingInFirstPerson;
	SerializedProperty playerIsAiming;

	SerializedProperty usingFreeFireMode;
	SerializedProperty lookInCameraDirectionOnFreeFireActive;
	SerializedProperty playerUsingMeleeWeapons;
	SerializedProperty jetPackEquiped;
	SerializedProperty usingJetpack;
	SerializedProperty sphereModeActive;
	SerializedProperty flyModeActive;
	SerializedProperty swimModeActive;

	SerializedProperty usingCloseCombatActive;
	SerializedProperty closeCombatAttackInProcess;

	SerializedProperty lockedCameraActive;
	SerializedProperty lookInCameraDirectionActive;
	SerializedProperty firstPersonActive;
	SerializedProperty usingDevice;
	SerializedProperty visibleToAI;
	SerializedProperty stealthModeActive;
	SerializedProperty playerNavMeshEnabled;
	SerializedProperty ignoreExternalActionsActiveState;
	SerializedProperty usingSubMenu;
	SerializedProperty playerMenuActive;
	SerializedProperty gamePaused;
	SerializedProperty driving;
	SerializedProperty drivingRemotely;
	SerializedProperty overridingElement;
	SerializedProperty currentVehicleName;

	SerializedProperty gravityPowerActive;
	SerializedProperty gravityForcePaused;
	SerializedProperty zeroGravityModeOn;
	SerializedProperty freeFloatingModeOn;
	SerializedProperty currentNormal;
	SerializedProperty checkOnGroundStatePausedFFOrZG;
	SerializedProperty checkOnGroundStatePaused;
	SerializedProperty playerID;
	SerializedProperty usedByAI;

	SerializedProperty updateHeadbobState;

	SerializedProperty checkQuickMovementTurn180DegreesOnRun;
	SerializedProperty minDelayToActivateQuickMovementTurn180OnRun;
	SerializedProperty quickTurnMovementDurationWalking;
	SerializedProperty quickTurnMovementDurationRunning;
	SerializedProperty quickTurnMovementDurationSprinting;
	SerializedProperty quickTurnMovementRotationSpeed;

	SerializedProperty wallRunnigEnabled;
	SerializedProperty wallRunningExternalControllerBehavior;

	SerializedProperty updateFootStepStateActive;

	SerializedProperty useEventsOnWalk;
	SerializedProperty eventOnWalkStart;
	SerializedProperty eventOnWalkEnd;

	SerializedProperty useEventsOnRun;
	SerializedProperty eventOnRunStart;
	SerializedProperty eventOnRunEnd;

	playerController manager;

	GUIStyle style = new GUIStyle ();

	GUIStyle sectionStyle = new GUIStyle ();

	GUIStyle buttonStyle = new GUIStyle ();

	Color buttonColor;

	string currentButtonString;

	string[] mainTabsOptions = {
		"First Person",
		"Advanced",
		"Animation ID Values",
		"Abilities",
		"Debug State",
		"Components",
		"Show All",
		"Hide All"
	};

	int mainTabIndex = -1;

	string[] abilitiesTabsOptions = {
		"Crouch Slide",
		"Wall Running",
		"Air Dash",
		"Zero Gravity",
		"Free Floating Mode",
		"Show All",
		"Hide All"
	};

	int abilitiesTabIndex = -1;

	void OnEnable ()
	{
		jumpPower = serializedObject.FindProperty ("jumpPower");
		airSpeed = serializedObject.FindProperty ("airSpeed");
		airControl = serializedObject.FindProperty ("airControl");

		ignorePlayerExtraRotationOnAirEnabled = serializedObject.FindProperty ("ignorePlayerExtraRotationOnAirEnabled");

		stationaryTurnSpeed = serializedObject.FindProperty ("stationaryTurnSpeed");
		movingTurnSpeed = serializedObject.FindProperty ("movingTurnSpeed");
		autoTurnSpeed = serializedObject.FindProperty ("autoTurnSpeed");
		aimTurnSpeed = serializedObject.FindProperty ("aimTurnSpeed");
		thresholdAngleDifference = serializedObject.FindProperty ("thresholdAngleDifference");

		useTurnSpeedOnAim = serializedObject.FindProperty ("useTurnSpeedOnAim");

		capsuleHeightOnCrouch = serializedObject.FindProperty ("capsuleHeightOnCrouch");
		regularMovementOnBulletTime = serializedObject.FindProperty ("regularMovementOnBulletTime");

		useMoveInpuptMagnitedeToForwardAmount = serializedObject.FindProperty ("useMoveInpuptMagnitedeToForwardAmount");

		baseLayerIndex = serializedObject.FindProperty ("baseLayerIndex");
		inputHorizontalLerpSpeed = serializedObject.FindProperty ("inputHorizontalLerpSpeed");
		inputVerticalLerpSpeed = serializedObject.FindProperty ("inputVerticalLerpSpeed");
		inputStrafeHorizontalLerpSpeed = serializedObject.FindProperty ("inputStrafeHorizontalLerpSpeed");
		inputStrafeVerticalLerpSpeed = serializedObject.FindProperty ("inputStrafeVerticalLerpSpeed");
		animatorForwardInputLerpSpeed = serializedObject.FindProperty ("animatorForwardInputLerpSpeed");
		animatorTurnInputLerpSpeed = serializedObject.FindProperty ("animatorTurnInputLerpSpeed");

		animSpeedMultiplier = serializedObject.FindProperty ("animSpeedMultiplier");

		overrideAnimationSpeedActive = serializedObject.FindProperty ("overrideAnimationSpeedActive");

		characterRadius = serializedObject.FindProperty ("characterRadius");

		minTimeToSetIdleOnStrafe = serializedObject.FindProperty ("minTimeToSetIdleOnStrafe");
		currentIdleID = serializedObject.FindProperty ("currentIdleID");
		currentStrafeID = serializedObject.FindProperty ("currentStrafeID");
		currentAirID = serializedObject.FindProperty ("currentAirID");
		currentCrouchID = serializedObject.FindProperty ("currentCrouchID");


		inputTankControlsHorizontalLerpSpeed = serializedObject.FindProperty ("inputTankControlsHorizontalLerpSpeed");
		inputTankControlsVerticalLerpSpeed = serializedObject.FindProperty ("inputTankControlsVerticalLerpSpeed");
		inputTankControlsHorizontalStrafeLerpSpeed = serializedObject.FindProperty ("inputTankControlsHorizontalStrafeLerpSpeed");
		inputTankControlsVerticalStrafeLerpSpeed = serializedObject.FindProperty ("inputTankControlsVerticalStrafeLerpSpeed");
		playerStatusID = serializedObject.FindProperty ("playerStatusID");
		playerStatusIDLerpSpeed = serializedObject.FindProperty ("playerStatusIDLerpSpeed");

		layer = serializedObject.FindProperty ("layer");
		rayDistance = serializedObject.FindProperty ("rayDistance");

		useSphereRaycastForGroundDetection = serializedObject.FindProperty ("useSphereRaycastForGroundDetection");
		sphereCastRadius = serializedObject.FindProperty ("sphereCastRadius");
		maxDistanceSphereCast = serializedObject.FindProperty ("maxDistanceSphereCast");
		sphereCastOffset = serializedObject.FindProperty ("sphereCastOffset");

		maxExtraRaycastDistanceToGroundDetection = serializedObject.FindProperty ("maxExtraRaycastDistanceToGroundDetection");
//
//		useCaspsuleCastForGroundDetection = serializedObject.FindProperty ("useCaspsuleCastForGroundDetection");
//		capsuleCastRadius = serializedObject.FindProperty ("capsuleCastRadius");
//		capsuleCastDistance = serializedObject.FindProperty ("capsuleCastDistance");
//		caspsuleCastOffset = serializedObject.FindProperty ("caspsuleCastOffset");
//

		walkSpeed = serializedObject.FindProperty ("walkSpeed");
		increaseWalkSpeedEnabled = serializedObject.FindProperty ("increaseWalkSpeedEnabled");
		increaseWalkSpeedValue = serializedObject.FindProperty ("increaseWalkSpeedValue");
		holdButtonToKeepIncreasedWalkSpeed = serializedObject.FindProperty ("holdButtonToKeepIncreasedWalkSpeed");
		walkRunTip = serializedObject.FindProperty ("walkRunTip");
		sprintEnabled = serializedObject.FindProperty ("sprintEnabled");
		changeCameraFovOnSprint = serializedObject.FindProperty ("changeCameraFovOnSprint");
		shakeCameraOnSprintThirdPerson = serializedObject.FindProperty ("shakeCameraOnSprintThirdPerson");
		sprintThirdPersonCameraShakeName = serializedObject.FindProperty ("sprintThirdPersonCameraShakeName");
		useSecondarySprintValues = serializedObject.FindProperty ("useSecondarySprintValues");
		sprintVelocity = serializedObject.FindProperty ("sprintVelocity");
		sprintJumpPower = serializedObject.FindProperty ("sprintJumpPower");
		sprintAirSpeed = serializedObject.FindProperty ("sprintAirSpeed");
		sprintAirControl = serializedObject.FindProperty ("sprintAirControl");

		runOnCrouchEnabled = serializedObject.FindProperty ("runOnCrouchEnabled");

		regularGroundAdherence = serializedObject.FindProperty ("regularGroundAdherence");
		slopesGroundAdherenceUp = serializedObject.FindProperty ("slopesGroundAdherenceUp");
		slopesGroundAdherenceDown = serializedObject.FindProperty ("slopesGroundAdherenceDown");
		maxRayDistanceRange = serializedObject.FindProperty ("maxRayDistanceRange");
		maxSlopeRayDistance = serializedObject.FindProperty ("maxSlopeRayDistance");
		maxStairsRayDistance = serializedObject.FindProperty ("maxStairsRayDistance");
		useMaxWalkSurfaceAngle = serializedObject.FindProperty ("useMaxWalkSurfaceAngle");
		maxWalkSurfaceAngle = serializedObject.FindProperty ("maxWalkSurfaceAngle");
		useMaxDistanceToCheckSurfaceAngle = serializedObject.FindProperty ("useMaxDistanceToCheckSurfaceAngle");
		maxDistanceToCheckSurfaceAngle = serializedObject.FindProperty ("maxDistanceToCheckSurfaceAngle");

		useMaxSlopeInclination = serializedObject.FindProperty ("useMaxSlopeInclination");
		maxSlopeInclination = serializedObject.FindProperty ("maxSlopeInclination");
		minSlopeInclination = serializedObject.FindProperty ("minSlopeInclination");

		noAnimatorSpeed = serializedObject.FindProperty ("noAnimatorSpeed");
		noAnimatorWalkMovementSpeed = serializedObject.FindProperty ("noAnimatorWalkMovementSpeed");
		noAnimatorRunMovementSpeed = serializedObject.FindProperty ("noAnimatorRunMovementSpeed");
		noAnimatorCrouchMovementSpeed = serializedObject.FindProperty ("noAnimatorCrouchMovementSpeed");
		noAnimatorRunCrouchMovementSpeed = serializedObject.FindProperty ("noAnimatorRunCrouchMovementSpeed");
		noAnimatorStrafeMovementSpeed = serializedObject.FindProperty ("noAnimatorStrafeMovementSpeed");
		noAnimatorCanRun = serializedObject.FindProperty ("noAnimatorCanRun");
		noAnimatorWalkBackwardMovementSpeed = serializedObject.FindProperty ("noAnimatorWalkBackwardMovementSpeed");
		noAnimatorRunBackwardMovementSpeed = serializedObject.FindProperty ("noAnimatorRunBackwardMovementSpeed");
		noAnimatorCrouchBackwardMovementSpeed = serializedObject.FindProperty ("noAnimatorCrouchBackwardMovementSpeed");
		noAnimatorRunCrouchBackwardMovementSpeed = serializedObject.FindProperty ("noAnimatorRunCrouchBackwardMovementSpeed");
		noAnimatorStrafeBackwardMovementSpeed = serializedObject.FindProperty ("noAnimatorStrafeBackwardMovementSpeed");
		noAnimatorCanRunBackwards = serializedObject.FindProperty ("noAnimatorCanRunBackwards");
		noAnimatorAirSpeed = serializedObject.FindProperty ("noAnimatorAirSpeed");
		maxVelocityChange = serializedObject.FindProperty ("maxVelocityChange");
		noAnimatorMovementSpeedMultiplier = serializedObject.FindProperty ("noAnimatorMovementSpeedMultiplier");
		noAnimatorSlopesGroundAdherenceUp = serializedObject.FindProperty ("noAnimatorSlopesGroundAdherenceUp");
		noAnimatorSlopesGroundAdherenceDown = serializedObject.FindProperty ("noAnimatorSlopesGroundAdherenceDown");
		noAnimatorStairsGroundAdherence = serializedObject.FindProperty ("noAnimatorStairsGroundAdherence");

		currentVelocity = serializedObject.FindProperty ("currentVelocity");
		axisValues = serializedObject.FindProperty ("axisValues");
//		noAnimatorCurrentMovementSpeed = serializedObject.FindProperty ("noAnimatorCurrentMovementSpeed");
//		currentVelocityChangeMagnitude = serializedObject.FindProperty ("currentVelocityChangeMagnitude");
//		characterVelocity = serializedObject.FindProperty ("characterVelocity");
//		moveInput = serializedObject.FindProperty ("moveInput");

		useRootMotionActive = serializedObject.FindProperty ("useRootMotionActive");

		rootMotionCurrentlyActive = serializedObject.FindProperty ("rootMotionCurrentlyActive");

		usingGenericModelActive = serializedObject.FindProperty ("usingGenericModelActive");

		noRootLerpSpeed = serializedObject.FindProperty ("noRootLerpSpeed");
		noRootWalkMovementSpeed = serializedObject.FindProperty ("noRootWalkMovementSpeed");
		noRootRunMovementSpeed = serializedObject.FindProperty ("noRootRunMovementSpeed");
		noRootSprintMovementSpeed = serializedObject.FindProperty ("noRootSprintMovementSpeed");
		noRootCrouchMovementSpeed = serializedObject.FindProperty ("noRootCrouchMovementSpeed");
		noRootWalkStrafeMovementSpeed = serializedObject.FindProperty ("noRootWalkStrafeMovementSpeed");
		noRootRunStrafeMovementSpeed = serializedObject.FindProperty ("noRootRunStrafeMovementSpeed");

		disableRootMotionTemporalyOnLandThirdPerson = serializedObject.FindProperty ("disableRootMotionTemporalyOnLandThirdPerson");
		durationDisableRootMotionTemporalyOnLandThirdPerson = serializedObject.FindProperty ("durationDisableRootMotionTemporalyOnLandThirdPerson");
		waitToDisableRootMotionTemporalyOnLandThirdPerson = serializedObject.FindProperty ("waitToDisableRootMotionTemporalyOnLandThirdPerson");

		useEventOnSprint = serializedObject.FindProperty ("useEventOnSprint");
		eventOnStartSprint = serializedObject.FindProperty ("eventOnStartSprint");
		eventOnStopSprint = serializedObject.FindProperty ("eventOnStopSprint");

		lookAlwaysInCameraDirection = serializedObject.FindProperty ("lookAlwaysInCameraDirection");
		lookInCameraDirectionIfLookingAtTarget = serializedObject.FindProperty ("lookInCameraDirectionIfLookingAtTarget");
		lookOnlyIfMoving = serializedObject.FindProperty ("lookOnlyIfMoving");
		disableLookAlwaysInCameraDirectionIfIncreaseWalkSpeedActive = serializedObject.FindProperty ("disableLookAlwaysInCameraDirectionIfIncreaseWalkSpeedActive");
		defaultStrafeWalkSpeed = serializedObject.FindProperty ("defaultStrafeWalkSpeed");
		defaultStrafeRunSpeed = serializedObject.FindProperty ("defaultStrafeRunSpeed");
		rotateDirectlyTowardCameraOnStrafe = serializedObject.FindProperty ("rotateDirectlyTowardCameraOnStrafe");
		strafeLerpSpeed = serializedObject.FindProperty ("strafeLerpSpeed");

		lookInCameraDirectionOnCrouchState = serializedObject.FindProperty ("lookInCameraDirectionOnCrouchState");

		characterMeshGameObject = serializedObject.FindProperty ("characterMeshGameObject");
		extraCharacterMeshGameObject = serializedObject.FindProperty ("extraCharacterMeshGameObject");

		checkCharacterMeshIfGeneratedOnStart = serializedObject.FindProperty ("checkCharacterMeshIfGeneratedOnStart");

		objectToCheckCharacterIfGeneratedOnStart = serializedObject.FindProperty ("objectToCheckCharacterIfGeneratedOnStart");

		characterMeshesListToDisableOnEvent = serializedObject.FindProperty ("characterMeshesListToDisableOnEvent");

		useEventsOnEnableDisableCharacterMeshes = serializedObject.FindProperty ("useEventsOnEnableDisableCharacterMeshes");
		eventOnEnableCharacterMeshes = serializedObject.FindProperty ("eventOnEnableCharacterMeshes");
		eventOnDisableCharacterMeshes = serializedObject.FindProperty ("eventOnDisableCharacterMeshes");
		eventOnEnableCharacterMeshesOnEditor = serializedObject.FindProperty ("eventOnEnableCharacterMeshesOnEditor");
		eventOnDisableCharacterMeshesOnEditor = serializedObject.FindProperty ("eventOnDisableCharacterMeshesOnEditor");

		canMoveWhileAimFirstPerson = serializedObject.FindProperty ("canMoveWhileAimFirstPerson");
		canMoveWhileAimThirdPerson = serializedObject.FindProperty ("canMoveWhileAimThirdPerson");
		stairsMinValue = serializedObject.FindProperty ("stairsMinValue");
		stairsMaxValue = serializedObject.FindProperty ("stairsMaxValue");
		stairsGroundAdherence = serializedObject.FindProperty ("stairsGroundAdherence");
		checkStairsWithInclination = serializedObject.FindProperty ("checkStairsWithInclination");
		minStairInclination = serializedObject.FindProperty ("minStairInclination");
		maxStairInclination = serializedObject.FindProperty ("maxStairInclination");
		checkForStairAdherenceSystem = serializedObject.FindProperty ("checkForStairAdherenceSystem");
		currentStairAdherenceSystemMinValue = serializedObject.FindProperty ("currentStairAdherenceSystemMinValue");
		currentStairAdherenceSystemMaxValue = serializedObject.FindProperty ("currentStairAdherenceSystemMaxValue");
		currentStairAdherenceSystemAdherenceValue = serializedObject.FindProperty ("currentStairAdherenceSystemAdherenceValue");
		lockedPlayerMovement = serializedObject.FindProperty ("lockedPlayerMovement");
		tankModeRotationSpeed = serializedObject.FindProperty ("tankModeRotationSpeed");
		canMoveWhileAimLockedCamera = serializedObject.FindProperty ("canMoveWhileAimLockedCamera");
		crouchVerticalInput2_5dEnabled = serializedObject.FindProperty ("crouchVerticalInput2_5dEnabled");

		gravityMultiplier = serializedObject.FindProperty ("gravityMultiplier");
		gravityForce = serializedObject.FindProperty ("gravityForce");

		useMaxFallSpeed = serializedObject.FindProperty ("useMaxFallSpeed");
		maxFallSpeed = serializedObject.FindProperty ("maxFallSpeed");

		zeroFrictionMaterial = serializedObject.FindProperty ("zeroFrictionMaterial");
		highFrictionMaterial = serializedObject.FindProperty ("highFrictionMaterial");
		enabledRegularJump = serializedObject.FindProperty ("enabledRegularJump");
		enabledDoubleJump = serializedObject.FindProperty ("enabledDoubleJump");
		maxNumberJumpsInAir = serializedObject.FindProperty ("maxNumberJumpsInAir");
		holdJumpSlowDownFallEnabled = serializedObject.FindProperty ("holdJumpSlowDownFallEnabled");
		slowDownGravityMultiplier = serializedObject.FindProperty ("slowDownGravityMultiplier");
		readyToJumpTime = serializedObject.FindProperty ("readyToJumpTime");

		useNewDoubleJumpPower = serializedObject.FindProperty ("useNewDoubleJumpPower");
		doubleJumpPower = serializedObject.FindProperty ("doubleJumpPower");

		removeVerticalSpeedOnDoubleJump = serializedObject.FindProperty ("removeVerticalSpeedOnDoubleJump");

		addJumpForceWhileButtonPressed = serializedObject.FindProperty ("addJumpForceWhileButtonPressed");
		addJumpForceDuration = serializedObject.FindProperty ("addJumpForceDuration");
		jumpForceAmontWhileButtonPressed = serializedObject.FindProperty ("jumpForceAmontWhileButtonPressed");
		useJumpForceOnAirJumpEnabled = serializedObject.FindProperty ("useJumpForceOnAirJumpEnabled");
		minWaitToAddJumpForceAfterButtonPressed = serializedObject.FindProperty ("minWaitToAddJumpForceAfterButtonPressed");

		stopCrouchOnJumpEnabled = serializedObject.FindProperty ("stopCrouchOnJumpEnabled");

		useEventOnJump = serializedObject.FindProperty ("useEventOnJump");
		eventOnJump = serializedObject.FindProperty ("eventOnJump");

		fallDamageEnabled = serializedObject.FindProperty ("fallDamageEnabled");
		maxTimeInAirBeforeGettingDamage = serializedObject.FindProperty ("maxTimeInAirBeforeGettingDamage");
		fallingDamageMultiplier = serializedObject.FindProperty ("fallingDamageMultiplier");
		minPlayerVelocityToApplyDamageThirdPerson = serializedObject.FindProperty ("minPlayerVelocityToApplyDamageThirdPerson");
		minPlayerVelocityToApplyDamageFirstPerson = serializedObject.FindProperty ("minPlayerVelocityToApplyDamageFirstPerson");

		callEventOnFallDamage = serializedObject.FindProperty ("callEventOnFallDamage");
		minTimeOnAirToUseEvent = serializedObject.FindProperty ("minTimeOnAirToUseEvent");
		eventOnFallDamage = serializedObject.FindProperty ("eventOnFallDamage");
		useLandMark = serializedObject.FindProperty ("useLandMark");
		maxLandDistance = serializedObject.FindProperty ("maxLandDistance");
		 
		ignoreShieldOnFallDamage = serializedObject.FindProperty ("ignoreShieldOnFallDamage");

		damageTypeIDOnFallDamage = serializedObject.FindProperty ("damageTypeIDOnFallDamage");

		applyFallDamageEnabled = serializedObject.FindProperty ("applyFallDamageEnabled");

		callEventOnlyIfPlayerAlive = serializedObject.FindProperty ("callEventOnlyIfPlayerAlive");

		minDistanceShowLandMark = serializedObject.FindProperty ("minDistanceShowLandMark");

		landMark = serializedObject.FindProperty ("landMark");
		landMark1 = serializedObject.FindProperty ("landMark1");
		landMark2 = serializedObject.FindProperty ("landMark2");
		canUseSphereMode = serializedObject.FindProperty ("canUseSphereMode");
		canGetOnVehicles = serializedObject.FindProperty ("canGetOnVehicles");
		canDrive = serializedObject.FindProperty ("canDrive");

		canCrouchWhenUsingWeaponsOnThirdPerson = serializedObject.FindProperty ("canCrouchWhenUsingWeaponsOnThirdPerson");

		getUpIfJumpOnCrouchInThirdPerson = serializedObject.FindProperty ("getUpIfJumpOnCrouchInThirdPerson");
		getUpIfJumpOnCrouchInFirstPerson = serializedObject.FindProperty ("getUpIfJumpOnCrouchInFirstPerson");
		useAutoCrouch = serializedObject.FindProperty ("useAutoCrouch");
		layerToCrouch = serializedObject.FindProperty ("layerToCrouch");
		raycastDistanceToAutoCrouch = serializedObject.FindProperty ("raycastDistanceToAutoCrouch");
		autoCrouchRayPosition = serializedObject.FindProperty ("autoCrouchRayPosition");
		secondRaycastOffset = serializedObject.FindProperty ("secondRaycastOffset");
		useObstacleDetectionToAvoidMovement = serializedObject.FindProperty ("useObstacleDetectionToAvoidMovement");
		obstacleDetectionToAvoidMovementRaycastDistance = serializedObject.FindProperty ("obstacleDetectionToAvoidMovementRaycastDistance");
		obstacleDetectionRaycastDistanceRightAndLeft = serializedObject.FindProperty ("obstacleDetectionRaycastDistanceRightAndLeft");
		obstacleDetectionRaycastHeightOffset = serializedObject.FindProperty ("obstacleDetectionRaycastHeightOffset");
		obstacleDetectionToAvoidMovementLayermask = serializedObject.FindProperty ("obstacleDetectionToAvoidMovementLayermask");

		forwardAnimatorName = serializedObject.FindProperty ("forwardAnimatorName");
		turnAnimatorName = serializedObject.FindProperty ("turnAnimatorName");
		horizontalAnimatorName = serializedObject.FindProperty ("horizontalAnimatorName");
		verticalAnimatorName = serializedObject.FindProperty ("verticalAnimatorName");
		horizontalStrafeAnimatorName = serializedObject.FindProperty ("horizontalStrafeAnimatorName");
		verticalStrafeAnimatorName = serializedObject.FindProperty ("verticalStrafeAnimatorName");
		onGroundAnimatorName = serializedObject.FindProperty ("onGroundAnimatorName");
		crouchAnimatorName = serializedObject.FindProperty ("crouchAnimatorName");
		movingAnimatorName = serializedObject.FindProperty ("movingAnimatorName");
		jumpAnimatorName = serializedObject.FindProperty ("jumpAnimatorName");
		jumpLegAnimatorName = serializedObject.FindProperty ("jumpLegAnimatorName");
		strafeModeActiveAnimatorName = serializedObject.FindProperty ("strafeModeActiveAnimatorName");
		movementInputActiveAnimatorName = serializedObject.FindProperty ("movementInputActiveAnimatorName");
		movementRelativeToCameraAnimatorName = serializedObject.FindProperty ("movementRelativeToCameraAnimatorName");
		movementIDAnimatorName = serializedObject.FindProperty ("movementIDAnimatorName");
		playerModeIDAnimatorName = serializedObject.FindProperty ("playerModeIDAnimatorName");
		movementSpeedAnimatorName = serializedObject.FindProperty ("movementSpeedAnimatorName");
		lastTimeInputPressedAnimatorName = serializedObject.FindProperty ("lastTimeInputPressedAnimatorName");
		carryingWeaponAnimatorName = serializedObject.FindProperty ("carryingWeaponAnimatorName");
		aimingModeActiveAnimatorName = serializedObject.FindProperty ("aimingModeActiveAnimatorName");
		playerStatusIDAnimatorName = serializedObject.FindProperty ("playerStatusIDAnimatorName");
		idleIDAnimatorName = serializedObject.FindProperty ("idleIDAnimatorName");
		strafeIDAnimatorName = serializedObject.FindProperty ("strafeIDAnimatorName");

		crouchIDAnimatorName = serializedObject.FindProperty ("crouchIDAnimatorName");

		airIDAnimatorName = serializedObject.FindProperty ("airIDAnimatorName");

		airSpeedAnimatorName = serializedObject.FindProperty ("airSpeedAnimatorName");

		quickTurnRightDirectionName = serializedObject.FindProperty ("quickTurnRightDirectionName");
		quickTurnLeftDirectionName = serializedObject.FindProperty ("quickTurnLeftDirectionName");

		quickTurnDirectionIDSpeedName = serializedObject.FindProperty ("quickTurnDirectionIDSpeedName");

		shieldActiveAnimatorName = serializedObject.FindProperty ("shieldActiveAnimatorName");

		playerCameraGameObject = serializedObject.FindProperty ("playerCameraGameObject");
		playerCameraTransform = serializedObject.FindProperty ("playerCameraTransform");

		playerCameraManager = serializedObject.FindProperty ("playerCameraManager");
		headBobManager = serializedObject.FindProperty ("headBobManager");
		playerInput = serializedObject.FindProperty ("playerInput");

		playerTransform = serializedObject.FindProperty ("playerTransform");

		weaponsManager = serializedObject.FindProperty ("weaponsManager");
		healthManager = serializedObject.FindProperty ("healthManager");
		IKSystemManager = serializedObject.FindProperty ("IKSystemManager");
		mainCameraTransform = serializedObject.FindProperty ("mainCameraTransform");
		animator = serializedObject.FindProperty ("animator");
		stepManager = serializedObject.FindProperty ("stepManager");
		mainRigidbody = serializedObject.FindProperty ("mainRigidbody");
		gravityManager = serializedObject.FindProperty ("gravityManager");
		characterStateIconManager = serializedObject.FindProperty ("characterStateIconManager");
		capsule = serializedObject.FindProperty ("capsule");
		mainCollider = serializedObject.FindProperty ("mainCollider");
		AIElements = serializedObject.FindProperty ("AIElements");
		mainPlayerActionSystem = serializedObject.FindProperty ("mainPlayerActionSystem");

		showDebugPrint = serializedObject.FindProperty ("showDebugPrint");

		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoColor = serializedObject.FindProperty ("gizmoColor");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
	
		crouchSlidingEnabled = serializedObject.FindProperty ("crouchSlidingEnabled");
		noAnimatorCrouchSlidingSpeed = serializedObject.FindProperty ("noAnimatorCrouchSlidingSpeed");
		noAnimatorCrouchSlidingDuration = serializedObject.FindProperty ("noAnimatorCrouchSlidingDuration");
		noAnimatorCrouchSlidingLerpSpeed = serializedObject.FindProperty ("noAnimatorCrouchSlidingLerpSpeed");
		noAnimatorCrouchSlidingLerpDelay = serializedObject.FindProperty ("noAnimatorCrouchSlidingLerpDelay");
		getUpAfterCrouchSlidingEnd = serializedObject.FindProperty ("getUpAfterCrouchSlidingEnd");
		keepCrouchSlidingOnInclinatedSurface = serializedObject.FindProperty ("keepCrouchSlidingOnInclinatedSurface");
		minInclinitaonSurfaceAngleToCrouchcSliding = serializedObject.FindProperty ("minInclinitaonSurfaceAngleToCrouchcSliding");
		crouchSlidingOnAirEnabled = serializedObject.FindProperty ("crouchSlidingOnAirEnabled");
		noAnimatorCrouchSlidingOnAirSpeed = serializedObject.FindProperty ("noAnimatorCrouchSlidingOnAirSpeed");
		useNoAnimatorCrouchSlidingDurationOnAir = serializedObject.FindProperty ("useNoAnimatorCrouchSlidingDurationOnAir");
		noAnimatorCrouchSlidingDurationOnAir = serializedObject.FindProperty ("noAnimatorCrouchSlidingDurationOnAir");
		eventOnCrouchSlidingStart = serializedObject.FindProperty ("eventOnCrouchSlidingStart");
		eventOnCrouchSlidingEnd = serializedObject.FindProperty ("eventOnCrouchSlidingEnd");

		useCrouchSlidingOnThirdPersonEnabled = serializedObject.FindProperty ("useCrouchSlidingOnThirdPersonEnabled");

		eventOnCrouchSlidingThirdPersonStart = serializedObject.FindProperty ("eventOnCrouchSlidingThirdPersonStart");
		eventOnCrouchSlidingThirdPersonEnd = serializedObject.FindProperty ("eventOnCrouchSlidingThirdPersonEnd");

		airDashEnabled = serializedObject.FindProperty ("airDashEnabled");
		airDashForce = serializedObject.FindProperty ("airDashForce");
		airDashColdDown = serializedObject.FindProperty ("airDashColdDown");
		pauseGravityForce = serializedObject.FindProperty ("pauseGravityForce");
		gravityForcePausedTime = serializedObject.FindProperty ("gravityForcePausedTime");
		resetGravityForceOnDash = serializedObject.FindProperty ("resetGravityForceOnDash");
		useDashLimit = serializedObject.FindProperty ("useDashLimit");
		dashLimit = serializedObject.FindProperty ("dashLimit");
		changeCameraFovOnDash = serializedObject.FindProperty ("changeCameraFovOnDash");
		cameraFovOnDash = serializedObject.FindProperty ("cameraFovOnDash");
		cameraFovOnDashSpeed = serializedObject.FindProperty ("cameraFovOnDashSpeed");

		useEventsOnAirDash = serializedObject.FindProperty ("useEventsOnAirDash");
		eventOnAirDashThirdPerson = serializedObject.FindProperty ("eventOnAirDashThirdPerson");
		eventOnAirDashFirstPerson = serializedObject.FindProperty ("eventOnAirDashFirstPerson");

		zeroGravityMovementSpeed = serializedObject.FindProperty ("zeroGravityMovementSpeed");
		zeroGravityControlSpeed = serializedObject.FindProperty ("zeroGravityControlSpeed");
		zeroGravityLookCameraSpeed = serializedObject.FindProperty ("zeroGravityLookCameraSpeed");
		useGravityDirectionLandMark = serializedObject.FindProperty ("useGravityDirectionLandMark");
		forwardSurfaceRayPosition = serializedObject.FindProperty ("forwardSurfaceRayPosition");
		maxDistanceToAdjust = serializedObject.FindProperty ("maxDistanceToAdjust");
		pauseCheckOnGroundStateZG = serializedObject.FindProperty ("pauseCheckOnGroundStateZG");
		pushPlayerWhenZeroGravityModeIsEnabled = serializedObject.FindProperty ("pushPlayerWhenZeroGravityModeIsEnabled");
		pushZeroGravityEnabledAmount = serializedObject.FindProperty ("pushZeroGravityEnabledAmount");
		canMoveVerticallyOnZeroGravity = serializedObject.FindProperty ("canMoveVerticallyOnZeroGravity");
		canMoveVerticallyAndHorizontalZG = serializedObject.FindProperty ("canMoveVerticallyAndHorizontalZG");
		zeroGravitySpeedMultiplier = serializedObject.FindProperty ("zeroGravitySpeedMultiplier");
		zeroGravityModeVerticalSpeed = serializedObject.FindProperty ("zeroGravityModeVerticalSpeed");
		freeFloatingMovementSpeed = serializedObject.FindProperty ("freeFloatingMovementSpeed");
		freeFloatingControlSpeed = serializedObject.FindProperty ("freeFloatingControlSpeed");
		pauseCheckOnGroundStateFF = serializedObject.FindProperty ("pauseCheckOnGroundStateFF");
		canMoveVerticallyOnFreeFloating = serializedObject.FindProperty ("canMoveVerticallyOnFreeFloating");
		canMoveVerticallyAndHorizontalFF = serializedObject.FindProperty ("canMoveVerticallyAndHorizontalFF");
		freeFloatingSpeedMultiplier = serializedObject.FindProperty ("freeFloatingSpeedMultiplier");
		pushFreeFloatingModeEnabledAmount = serializedObject.FindProperty ("pushFreeFloatingModeEnabledAmount");
		freeFloatingModeVerticalSpeed = serializedObject.FindProperty ("freeFloatingModeVerticalSpeed");
		useMaxAngleToCheckOnGroundStateZGFF = serializedObject.FindProperty ("useMaxAngleToCheckOnGroundStateZGFF");
		maxAngleToChekOnGroundStateZGFF = serializedObject.FindProperty ("maxAngleToChekOnGroundStateZGFF");

		currentExternalControllerBehavior = serializedObject.FindProperty ("currentExternalControllerBehavior");

		externalControllBehaviorActive = serializedObject.FindProperty ("externalControllBehaviorActive");

		mainExternalControllerBehaviorManager = serializedObject.FindProperty ("mainExternalControllerBehaviorManager");

		playerOnGround = serializedObject.FindProperty ("playerOnGround");
		isMoving = serializedObject.FindProperty ("isMoving");

		movingOnPlatformActive = serializedObject.FindProperty ("movingOnPlatformActive");

		jump = serializedObject.FindProperty ("jump");
		running = serializedObject.FindProperty ("running");
		crouching = serializedObject.FindProperty ("crouching");
		canMove = serializedObject.FindProperty ("canMove");

		moveIputPaused = serializedObject.FindProperty ("moveIputPaused");

		canMoveAI = serializedObject.FindProperty ("canMoveAI");

		slowingFall = serializedObject.FindProperty ("slowingFall");
		isDead = serializedObject.FindProperty ("isDead");
		playerSetAsChildOfParent = serializedObject.FindProperty ("playerSetAsChildOfParent");
		useRelativeMovementToLockedCamera = serializedObject.FindProperty ("useRelativeMovementToLockedCamera");
		ladderFound = serializedObject.FindProperty ("ladderFound");
		ignoreCameraDirectionOnMovement = serializedObject.FindProperty ("ignoreCameraDirectionOnMovement");
		strafeModeActive = serializedObject.FindProperty ("strafeModeActive");

		actionActive = serializedObject.FindProperty ("actionActive");
		adhereToGround = serializedObject.FindProperty ("adhereToGround");
		distanceToGround = serializedObject.FindProperty ("distanceToGround");
		hitAngle = serializedObject.FindProperty ("hitAngle");
		slopeFound = serializedObject.FindProperty ("slopeFound");
		movingOnSlopeUp = serializedObject.FindProperty ("movingOnSlopeUp");
		movingOnSlopeDown = serializedObject.FindProperty ("movingOnSlopeDown");
		stairsFound = serializedObject.FindProperty ("stairsFound");
		stairAdherenceSystemDetected = serializedObject.FindProperty ("stairAdherenceSystemDetected");
		wallRunningActive = serializedObject.FindProperty ("wallRunningActive");
		crouchSlidingActive = serializedObject.FindProperty ("crouchSlidingActive");

		aimingInThirdPerson = serializedObject.FindProperty ("aimingInThirdPerson");
		aimingInFirstPerson = serializedObject.FindProperty ("aimingInFirstPerson");

		playerIsAiming = serializedObject.FindProperty ("playerIsAiming");

		usingFreeFireMode = serializedObject.FindProperty ("usingFreeFireMode");
		lookInCameraDirectionOnFreeFireActive = serializedObject.FindProperty ("lookInCameraDirectionOnFreeFireActive");
		playerUsingMeleeWeapons = serializedObject.FindProperty ("playerUsingMeleeWeapons");

		jetPackEquiped = serializedObject.FindProperty ("jetPackEquiped");
		usingJetpack = serializedObject.FindProperty ("usingJetpack");
		sphereModeActive = serializedObject.FindProperty ("sphereModeActive");
		flyModeActive = serializedObject.FindProperty ("flyModeActive");

		swimModeActive = serializedObject.FindProperty ("swimModeActive");

		usingCloseCombatActive = serializedObject.FindProperty ("usingCloseCombatActive");
		closeCombatAttackInProcess = serializedObject.FindProperty ("closeCombatAttackInProcess");

		lockedCameraActive = serializedObject.FindProperty ("lockedCameraActive");
		lookInCameraDirectionActive = serializedObject.FindProperty ("lookInCameraDirectionActive");
		firstPersonActive = serializedObject.FindProperty ("firstPersonActive");
		usingDevice = serializedObject.FindProperty ("usingDevice");
		visibleToAI = serializedObject.FindProperty ("visibleToAI");
		stealthModeActive = serializedObject.FindProperty ("stealthModeActive");
		playerNavMeshEnabled = serializedObject.FindProperty ("playerNavMeshEnabled");
		ignoreExternalActionsActiveState = serializedObject.FindProperty ("ignoreExternalActionsActiveState");
		usingSubMenu = serializedObject.FindProperty ("usingSubMenu");
		playerMenuActive = serializedObject.FindProperty ("playerMenuActive");
		gamePaused = serializedObject.FindProperty ("gamePaused");
		driving = serializedObject.FindProperty ("driving");
		drivingRemotely = serializedObject.FindProperty ("drivingRemotely");
		overridingElement = serializedObject.FindProperty ("overridingElement");
		currentVehicleName = serializedObject.FindProperty ("currentVehicleName");

		gravityPowerActive = serializedObject.FindProperty ("gravityPowerActive");
		gravityForcePaused = serializedObject.FindProperty ("gravityForcePaused");
		zeroGravityModeOn = serializedObject.FindProperty ("zeroGravityModeOn");
		freeFloatingModeOn = serializedObject.FindProperty ("freeFloatingModeOn");
		currentNormal = serializedObject.FindProperty ("currentNormal");
		checkOnGroundStatePausedFFOrZG = serializedObject.FindProperty ("checkOnGroundStatePausedFFOrZG");
		checkOnGroundStatePaused = serializedObject.FindProperty ("checkOnGroundStatePaused");
		playerID = serializedObject.FindProperty ("playerID");
		usedByAI = serializedObject.FindProperty ("usedByAI");

		updateHeadbobState = serializedObject.FindProperty ("updateHeadbobState");

		checkQuickMovementTurn180DegreesOnRun = serializedObject.FindProperty ("checkQuickMovementTurn180DegreesOnRun");
		minDelayToActivateQuickMovementTurn180OnRun = serializedObject.FindProperty ("minDelayToActivateQuickMovementTurn180OnRun");
		quickTurnMovementDurationWalking = serializedObject.FindProperty ("quickTurnMovementDurationWalking");
		quickTurnMovementDurationRunning = serializedObject.FindProperty ("quickTurnMovementDurationRunning");
		quickTurnMovementDurationSprinting = serializedObject.FindProperty ("quickTurnMovementDurationSprinting");
		quickTurnMovementRotationSpeed = serializedObject.FindProperty ("quickTurnMovementRotationSpeed");

		wallRunnigEnabled = serializedObject.FindProperty ("wallRunnigEnabled");
		wallRunningExternalControllerBehavior = serializedObject.FindProperty ("wallRunningExternalControllerBehavior");

		updateFootStepStateActive = serializedObject.FindProperty ("updateFootStepStateActive");

		useEventsOnWalk = serializedObject.FindProperty ("useEventsOnWalk");
		eventOnWalkStart = serializedObject.FindProperty ("eventOnWalkStart");
		eventOnWalkEnd = serializedObject.FindProperty ("eventOnWalkEnd");

		useEventsOnRun = serializedObject.FindProperty ("useEventsOnRun");
		eventOnRunStart = serializedObject.FindProperty ("eventOnRunStart");
		eventOnRunEnd = serializedObject.FindProperty ("eventOnRunEnd");

		manager = (playerController)target;
	}

	void OnSceneGUI ()
	{   
		if (manager.showGizmo) {
			if (manager.useAutoCrouch) {
				style.normal.textColor = manager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;
				Handles.Label (manager.autoCrouchRayPosition.position, "Auto Crouch \n raycast", style);	
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

		GUILayout.BeginVertical ("Movement Settings", "window");
		EditorGUILayout.PropertyField (jumpPower);
		EditorGUILayout.PropertyField (airSpeed);
		EditorGUILayout.PropertyField (airControl);

		EditorGUILayout.PropertyField (ignorePlayerExtraRotationOnAirEnabled);

		EditorGUILayout.PropertyField (stationaryTurnSpeed);
		EditorGUILayout.PropertyField (movingTurnSpeed);

		EditorGUILayout.PropertyField (useTurnSpeedOnAim);

		EditorGUILayout.PropertyField (autoTurnSpeed);
		EditorGUILayout.PropertyField (aimTurnSpeed);
		EditorGUILayout.PropertyField (thresholdAngleDifference);
		EditorGUILayout.PropertyField (regularMovementOnBulletTime);	
		EditorGUILayout.PropertyField (useMoveInpuptMagnitedeToForwardAmount);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Animator Settings", "window");
		EditorGUILayout.PropertyField (baseLayerIndex);	
		EditorGUILayout.PropertyField (inputHorizontalLerpSpeed);
		EditorGUILayout.PropertyField (inputVerticalLerpSpeed);
		EditorGUILayout.PropertyField (inputStrafeHorizontalLerpSpeed);
		EditorGUILayout.PropertyField (inputStrafeVerticalLerpSpeed);
		EditorGUILayout.PropertyField (animatorForwardInputLerpSpeed);
		EditorGUILayout.PropertyField (animatorTurnInputLerpSpeed);
		EditorGUILayout.PropertyField (animSpeedMultiplier);
		EditorGUILayout.PropertyField (minTimeToSetIdleOnStrafe);
		EditorGUILayout.PropertyField (overrideAnimationSpeedActive);

		EditorGUILayout.PropertyField (characterRadius);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Animator ID State", "window");
		EditorGUILayout.PropertyField (currentIdleID);	
		EditorGUILayout.PropertyField (currentStrafeID);	
		EditorGUILayout.PropertyField (currentAirID);	
		EditorGUILayout.PropertyField (currentCrouchID);	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Tank Controls Animator Settings", "window");
		EditorGUILayout.PropertyField (inputTankControlsHorizontalLerpSpeed);
		EditorGUILayout.PropertyField (inputTankControlsVerticalLerpSpeed);
		EditorGUILayout.PropertyField (inputTankControlsHorizontalStrafeLerpSpeed);
		EditorGUILayout.PropertyField (inputTankControlsVerticalStrafeLerpSpeed);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Status ID Settings", "window");
		EditorGUILayout.PropertyField (playerStatusID);
		EditorGUILayout.PropertyField (playerStatusIDLerpSpeed);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ground Detection Settings", "window");
		EditorGUILayout.PropertyField (layer);
		EditorGUILayout.PropertyField (rayDistance);
		EditorGUILayout.PropertyField (updateFootStepStateActive);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useSphereRaycastForGroundDetection);
		if (useSphereRaycastForGroundDetection.boolValue) {
			EditorGUILayout.PropertyField (sphereCastRadius);

			EditorGUILayout.PropertyField (maxDistanceSphereCast);
			EditorGUILayout.PropertyField (sphereCastOffset);
		}

		EditorGUILayout.PropertyField (maxExtraRaycastDistanceToGroundDetection);

//		EditorGUILayout.Space ();
//
//		EditorGUILayout.PropertyField (useCaspsuleCastForGroundDetection);
//		if (useCaspsuleCastForGroundDetection.boolValue) {
//			EditorGUILayout.PropertyField (capsuleCastRadius);
//
//			EditorGUILayout.PropertyField (capsuleCastDistance);
//			EditorGUILayout.PropertyField (caspsuleCastOffset);
//		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Walk/Run/Sprint Settings", "window");
		EditorGUILayout.PropertyField (walkSpeed);
		EditorGUILayout.PropertyField (increaseWalkSpeedEnabled);
		if (increaseWalkSpeedEnabled.boolValue) {
			EditorGUILayout.PropertyField (increaseWalkSpeedValue);
			EditorGUILayout.PropertyField (holdButtonToKeepIncreasedWalkSpeed);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (walkRunTip);

		EditorGUILayout.Space ();

		//		GUILayout.BeginVertical ("Walk To Run Acceleration Settings", "window");
		//		EditorGUILayout.PropertyField (list.FindProperty ("useWalkToRunAccelerationChange"));
		//		if (list.FindProperty ("useWalkToRunAccelerationChange").boolValue) {
		//			EditorGUILayout.PropertyField (list.FindProperty ("walkToRunLerSpeed"));
		//			EditorGUILayout.PropertyField (list.FindProperty ("walkToRunAccelerationSpeed"));
		//			EditorGUILayout.PropertyField (list.FindProperty ("walkToRunDecelerationSpeed"));
		//		}
		//		GUILayout.EndVertical ();
		//
		//		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sprint Settings", "window");
		EditorGUILayout.PropertyField (sprintEnabled);
		if (sprintEnabled.boolValue) {
			EditorGUILayout.PropertyField (changeCameraFovOnSprint);
			EditorGUILayout.PropertyField (shakeCameraOnSprintThirdPerson);
			EditorGUILayout.PropertyField (sprintThirdPersonCameraShakeName);

			EditorGUILayout.PropertyField (useSecondarySprintValues);
			if (useSecondarySprintValues.boolValue) {
				EditorGUILayout.PropertyField (sprintVelocity);
				EditorGUILayout.PropertyField (sprintJumpPower);
				EditorGUILayout.PropertyField (sprintAirSpeed);
				EditorGUILayout.PropertyField (sprintAirControl);
			}

			EditorGUILayout.PropertyField (runOnCrouchEnabled);
		}

		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ground Adherence Settings", "window");
		EditorGUILayout.PropertyField (regularGroundAdherence);	

		EditorGUILayout.PropertyField (slopesGroundAdherenceUp);	
		EditorGUILayout.PropertyField (slopesGroundAdherenceDown);

		EditorGUILayout.PropertyField (maxRayDistanceRange);
		EditorGUILayout.PropertyField (maxSlopeRayDistance);
		EditorGUILayout.PropertyField (maxStairsRayDistance);
		EditorGUILayout.PropertyField (useMaxWalkSurfaceAngle);	
		if (useMaxWalkSurfaceAngle.boolValue) {
			EditorGUILayout.PropertyField (maxWalkSurfaceAngle);	
			EditorGUILayout.PropertyField (useMaxDistanceToCheckSurfaceAngle);	
			if (useMaxDistanceToCheckSurfaceAngle.boolValue) {
				EditorGUILayout.PropertyField (maxDistanceToCheckSurfaceAngle);
			}
		}

		EditorGUILayout.PropertyField (useMaxSlopeInclination);	
		if (useMaxSlopeInclination.boolValue) {
			EditorGUILayout.PropertyField (maxSlopeInclination);	
			EditorGUILayout.PropertyField (minSlopeInclination);	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Root Motion Settings", "window");
		EditorGUILayout.PropertyField (useRootMotionActive);
		if (!useRootMotionActive.boolValue) {
			EditorGUILayout.PropertyField (noRootLerpSpeed);
			EditorGUILayout.PropertyField (noRootWalkMovementSpeed);
			EditorGUILayout.PropertyField (noRootRunMovementSpeed);
			EditorGUILayout.PropertyField (noRootSprintMovementSpeed);
			EditorGUILayout.PropertyField (noRootCrouchMovementSpeed);
			EditorGUILayout.PropertyField (noRootWalkStrafeMovementSpeed);
			EditorGUILayout.PropertyField (noRootRunStrafeMovementSpeed);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (disableRootMotionTemporalyOnLandThirdPerson);
		if (disableRootMotionTemporalyOnLandThirdPerson.boolValue) {
			EditorGUILayout.PropertyField (durationDisableRootMotionTemporalyOnLandThirdPerson);
			EditorGUILayout.PropertyField (waitToDisableRootMotionTemporalyOnLandThirdPerson);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player ID Settings", "window");
		EditorGUILayout.PropertyField (playerID);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (usedByAI);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		mainTabIndex = GUILayout.SelectionGrid (mainTabIndex, mainTabsOptions, 3);

		if (mainTabIndex >= 0 && mainTabIndex < mainTabsOptions.Length) {
			switch (mainTabsOptions [mainTabIndex]) {
			case "First Person":
				showFirstPersonSettings ();

				break;
			
			case "Advanced":
				showAdvancedSettings ();

				break;

			case "Abilities":
				showAbilitiesSettings ();

				break;

			case "Components":
				showCharacterComponents ();

				break;

			case "Animation ID Values":
				showAnimatorID ();

				break;

			case "Debug State":
				showDebugSettings ();

				break;
		
			case "Show All":

				showFirstPersonSettings ();

				showAdvancedSettings ();

				showAbilitiesSettings ();

				showCharacterComponents ();

				showAnimatorID ();

				showDebugSettings ();

				break;

			case "Hide All":
				mainTabIndex = -1;

				break;

			default: 

				break;
			}
		}

		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showSectionTitle (string sectionTitle)
	{
		sectionStyle.fontStyle = FontStyle.Bold;
		sectionStyle.fontSize = 30;
		sectionStyle.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.LabelField (sectionTitle, sectionStyle);

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
	}

	void showFirstPersonSettings ()
	{
		showSectionTitle ("FIRST PERSON SETTINGS");

		GUILayout.BeginVertical ("First Person No Animator Settings", "window");

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (noAnimatorSpeed);	
		EditorGUILayout.PropertyField (noAnimatorWalkMovementSpeed);	
		if (noAnimatorCanRun.boolValue) {
			EditorGUILayout.PropertyField (noAnimatorRunMovementSpeed);	
		}
		EditorGUILayout.PropertyField (noAnimatorCrouchMovementSpeed);
		EditorGUILayout.PropertyField (noAnimatorRunCrouchMovementSpeed);
		EditorGUILayout.PropertyField (noAnimatorStrafeMovementSpeed);
		EditorGUILayout.PropertyField (noAnimatorCanRun);
		EditorGUILayout.PropertyField (noAnimatorWalkBackwardMovementSpeed);	
		if (noAnimatorCanRunBackwards.boolValue) {
			EditorGUILayout.PropertyField (noAnimatorRunBackwardMovementSpeed);	
		}
		EditorGUILayout.PropertyField (noAnimatorCrouchBackwardMovementSpeed);
		EditorGUILayout.PropertyField (noAnimatorRunCrouchBackwardMovementSpeed);	

		EditorGUILayout.PropertyField (noAnimatorStrafeBackwardMovementSpeed);
		EditorGUILayout.PropertyField (noAnimatorCanRunBackwards);	
		EditorGUILayout.PropertyField (noAnimatorAirSpeed);	
		EditorGUILayout.PropertyField (maxVelocityChange);
		EditorGUILayout.PropertyField (noAnimatorMovementSpeedMultiplier);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ground Adherence Settings", "window");
		EditorGUILayout.PropertyField (noAnimatorSlopesGroundAdherenceUp);
		EditorGUILayout.PropertyField (noAnimatorSlopesGroundAdherenceDown);
		EditorGUILayout.PropertyField (noAnimatorStairsGroundAdherence);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showAdvancedSettings ()
	{
		showSectionTitle ("ADVANCED SETTINGS");

		//		GUILayout.BeginVertical ("Speed State", "window");

		//		GUILayout.Label ("Current Speed\t" + noAnimatorCurrentMovementSpeed.floatValue);
		//		GUILayout.Label ("Current Float Speed\t" + currentVelocityChangeMagnitude.floatValue);
		//		GUILayout.Label ("Character Velocity\t" + characterVelocity.vector3Value);
		//		GUILayout.Label ("Move Input\t" + moveInput.vector3Value);
		//
		//		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sprint Settings", "window");
		if (sprintEnabled.boolValue) {
			EditorGUILayout.PropertyField (useEventOnSprint);

			EditorGUILayout.Space ();

			if (useEventOnSprint.boolValue) {
				EditorGUILayout.PropertyField (eventOnStartSprint);
				EditorGUILayout.PropertyField (eventOnStopSprint);
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Look In Camera Direction Settings", "window");
		EditorGUILayout.PropertyField (lookAlwaysInCameraDirection);	
		EditorGUILayout.PropertyField (lookInCameraDirectionIfLookingAtTarget);	
		if (lookAlwaysInCameraDirection.boolValue || lookInCameraDirectionIfLookingAtTarget.boolValue) {
			EditorGUILayout.PropertyField (lookOnlyIfMoving);

			EditorGUILayout.PropertyField (disableLookAlwaysInCameraDirectionIfIncreaseWalkSpeedActive);

			EditorGUILayout.PropertyField (lookInCameraDirectionOnCrouchState);
		}

		EditorGUILayout.PropertyField (defaultStrafeWalkSpeed);
		EditorGUILayout.PropertyField (defaultStrafeRunSpeed);
		EditorGUILayout.PropertyField (rotateDirectlyTowardCameraOnStrafe);
		EditorGUILayout.PropertyField (strafeLerpSpeed);
		EditorGUILayout.PropertyField (updateHeadbobState);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Quick Movement Turn Settings", "window");
		EditorGUILayout.PropertyField (checkQuickMovementTurn180DegreesOnRun);
		if (checkQuickMovementTurn180DegreesOnRun.boolValue) {
			EditorGUILayout.PropertyField (minDelayToActivateQuickMovementTurn180OnRun);
			EditorGUILayout.PropertyField (quickTurnMovementDurationWalking);
			EditorGUILayout.PropertyField (quickTurnMovementDurationRunning);
			EditorGUILayout.PropertyField (quickTurnMovementDurationSprinting);
			EditorGUILayout.PropertyField (quickTurnMovementRotationSpeed);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Character Mesh Settings", "window");
		EditorGUILayout.PropertyField (characterMeshGameObject);	

		EditorGUILayout.Space ();

		showSimpleList (extraCharacterMeshGameObject);	

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (characterMeshesListToDisableOnEvent);	

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (checkCharacterMeshIfGeneratedOnStart);	
		if (checkCharacterMeshIfGeneratedOnStart.boolValue) {
			EditorGUILayout.PropertyField (objectToCheckCharacterIfGeneratedOnStart);	
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventsOnEnableDisableCharacterMeshes);	
		if (useEventsOnEnableDisableCharacterMeshes.boolValue) {
			
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnEnableCharacterMeshes);	
			EditorGUILayout.PropertyField (eventOnDisableCharacterMeshes);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnEnableCharacterMeshesOnEditor);	
			EditorGUILayout.PropertyField (eventOnDisableCharacterMeshesOnEditor);	
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Move While Aim Settings", "window");
		EditorGUILayout.PropertyField (canMoveWhileAimFirstPerson);
		EditorGUILayout.PropertyField (canMoveWhileAimThirdPerson);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Stairs Settings", "window");
		EditorGUILayout.PropertyField (stairsMinValue);
		EditorGUILayout.PropertyField (stairsMaxValue);
		EditorGUILayout.PropertyField (stairsGroundAdherence);	

		EditorGUILayout.PropertyField (checkStairsWithInclination);
		if (checkStairsWithInclination.boolValue) {
			EditorGUILayout.PropertyField (minStairInclination);
			EditorGUILayout.PropertyField (maxStairInclination);	
		}

		EditorGUILayout.PropertyField (checkForStairAdherenceSystem);
		if (checkForStairAdherenceSystem.boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Current Stairs Adherence (DEBUG)", "window");
			EditorGUILayout.PropertyField (currentStairAdherenceSystemMinValue);	
			EditorGUILayout.PropertyField (currentStairAdherenceSystemMaxValue);
			EditorGUILayout.PropertyField (currentStairAdherenceSystemAdherenceValue);
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Locked Camera Settings", "window");
		EditorGUILayout.PropertyField (lockedPlayerMovement);
		if (lockedPlayerMovement.enumValueIndex == 0) {
			EditorGUILayout.PropertyField (tankModeRotationSpeed);
			EditorGUILayout.PropertyField (canMoveWhileAimLockedCamera);
		}

		EditorGUILayout.PropertyField (crouchVerticalInput2_5dEnabled);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gravity Settings", "window");
		EditorGUILayout.PropertyField (gravityMultiplier);
		EditorGUILayout.PropertyField (gravityForce);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useMaxFallSpeed);
		if (useMaxFallSpeed.boolValue) {
			EditorGUILayout.PropertyField (maxFallSpeed);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Physics Settings", "window");
		EditorGUILayout.PropertyField (zeroFrictionMaterial);
		EditorGUILayout.PropertyField (highFrictionMaterial);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Jump Settings", "window");
		EditorGUILayout.PropertyField (enabledRegularJump);
		EditorGUILayout.PropertyField (holdJumpSlowDownFallEnabled);
		if (holdJumpSlowDownFallEnabled.boolValue) {
			EditorGUILayout.PropertyField (slowDownGravityMultiplier);
		}
		EditorGUILayout.PropertyField (readyToJumpTime);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (addJumpForceWhileButtonPressed);
		if (addJumpForceWhileButtonPressed.boolValue) {
			EditorGUILayout.PropertyField (addJumpForceDuration);
			EditorGUILayout.PropertyField (jumpForceAmontWhileButtonPressed);
			EditorGUILayout.PropertyField (minWaitToAddJumpForceAfterButtonPressed);
			EditorGUILayout.PropertyField (useJumpForceOnAirJumpEnabled);
		}

		EditorGUILayout.PropertyField (stopCrouchOnJumpEnabled);
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventOnJump);
		if (useEventOnJump.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnJump);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Double Jump Settings", "window");

		EditorGUILayout.PropertyField (enabledDoubleJump);
		if (enabledDoubleJump.boolValue) {
			EditorGUILayout.PropertyField (maxNumberJumpsInAir);
		}
		EditorGUILayout.PropertyField (useNewDoubleJumpPower);
		if (useNewDoubleJumpPower.boolValue) {
			EditorGUILayout.PropertyField (doubleJumpPower);
		}
		EditorGUILayout.PropertyField (removeVerticalSpeedOnDoubleJump);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Fall Damage Settings", "window");
		EditorGUILayout.PropertyField (fallDamageEnabled);
		if (fallDamageEnabled.boolValue) {
			EditorGUILayout.PropertyField (maxTimeInAirBeforeGettingDamage);
			EditorGUILayout.PropertyField (fallingDamageMultiplier);				 
			EditorGUILayout.PropertyField (minPlayerVelocityToApplyDamageThirdPerson);
			EditorGUILayout.PropertyField (minPlayerVelocityToApplyDamageFirstPerson);
			EditorGUILayout.PropertyField (ignoreShieldOnFallDamage);
			EditorGUILayout.PropertyField (damageTypeIDOnFallDamage);
			EditorGUILayout.PropertyField (applyFallDamageEnabled);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (callEventOnFallDamage);
			if (callEventOnFallDamage.boolValue) {
				EditorGUILayout.PropertyField (minTimeOnAirToUseEvent);
				EditorGUILayout.PropertyField (callEventOnlyIfPlayerAlive);

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventOnFallDamage);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Land Mark Settings", "window");
		EditorGUILayout.PropertyField (useLandMark);
		if (useLandMark.boolValue) {
			EditorGUILayout.PropertyField (maxLandDistance);
			EditorGUILayout.PropertyField (minDistanceShowLandMark);
			EditorGUILayout.PropertyField (landMark);
			EditorGUILayout.PropertyField (landMark1);
			EditorGUILayout.PropertyField (landMark2);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Modes Settings", "window");
		EditorGUILayout.PropertyField (canUseSphereMode);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Settings", "window");
		EditorGUILayout.PropertyField (canGetOnVehicles);
		EditorGUILayout.PropertyField (canDrive);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Crouch Settings", "window");
		EditorGUILayout.PropertyField (capsuleHeightOnCrouch);
		EditorGUILayout.PropertyField (getUpIfJumpOnCrouchInThirdPerson);
		EditorGUILayout.PropertyField (getUpIfJumpOnCrouchInFirstPerson);
		EditorGUILayout.PropertyField (useAutoCrouch);
		if (useAutoCrouch.boolValue) {
			EditorGUILayout.PropertyField (layerToCrouch);
			EditorGUILayout.PropertyField (raycastDistanceToAutoCrouch);
			EditorGUILayout.PropertyField (autoCrouchRayPosition);
			EditorGUILayout.PropertyField (secondRaycastOffset);

		}
		EditorGUILayout.PropertyField (canCrouchWhenUsingWeaponsOnThirdPerson);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Obstacle Detection To Avoid Movement Settings", "window");
		EditorGUILayout.PropertyField (useObstacleDetectionToAvoidMovement);
		if (useObstacleDetectionToAvoidMovement.boolValue) {
			EditorGUILayout.PropertyField (obstacleDetectionToAvoidMovementRaycastDistance);
			EditorGUILayout.PropertyField (obstacleDetectionRaycastDistanceRightAndLeft);
			EditorGUILayout.PropertyField (obstacleDetectionRaycastHeightOffset);
			EditorGUILayout.PropertyField (obstacleDetectionToAvoidMovementLayermask);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events On Walk/Run Settings", "window");
		EditorGUILayout.PropertyField (useEventsOnWalk);
		if (useEventsOnWalk.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnWalkStart);
			EditorGUILayout.PropertyField (eventOnWalkEnd);
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventsOnRun);
		if (useEventsOnRun.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnRunStart);
			EditorGUILayout.PropertyField (eventOnRunEnd);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showAnimatorID ()
	{
		showSectionTitle ("ANIMATION ID Values");

		GUILayout.BeginVertical ("Animator Fields Settings", "window");
		EditorGUILayout.PropertyField (forwardAnimatorName);
		EditorGUILayout.PropertyField (turnAnimatorName);

		EditorGUILayout.PropertyField (horizontalAnimatorName);
		EditorGUILayout.PropertyField (verticalAnimatorName);

		EditorGUILayout.PropertyField (horizontalStrafeAnimatorName);
		EditorGUILayout.PropertyField (verticalStrafeAnimatorName);

		EditorGUILayout.PropertyField (onGroundAnimatorName);
		EditorGUILayout.PropertyField (crouchAnimatorName);
		EditorGUILayout.PropertyField (movingAnimatorName);
		EditorGUILayout.PropertyField (jumpAnimatorName);
		EditorGUILayout.PropertyField (jumpLegAnimatorName);

		EditorGUILayout.PropertyField (strafeModeActiveAnimatorName);
		EditorGUILayout.PropertyField (movementInputActiveAnimatorName);
		EditorGUILayout.PropertyField (movementRelativeToCameraAnimatorName);
		EditorGUILayout.PropertyField (movementIDAnimatorName);
		EditorGUILayout.PropertyField (playerModeIDAnimatorName);
		EditorGUILayout.PropertyField (movementSpeedAnimatorName);
		EditorGUILayout.PropertyField (lastTimeInputPressedAnimatorName);

		EditorGUILayout.PropertyField (carryingWeaponAnimatorName);
		EditorGUILayout.PropertyField (aimingModeActiveAnimatorName);

		EditorGUILayout.PropertyField (playerStatusIDAnimatorName);
		EditorGUILayout.PropertyField (idleIDAnimatorName);
		EditorGUILayout.PropertyField (strafeIDAnimatorName);

		EditorGUILayout.PropertyField (crouchIDAnimatorName);

		EditorGUILayout.PropertyField (airIDAnimatorName);

		EditorGUILayout.PropertyField (airSpeedAnimatorName);	

		EditorGUILayout.PropertyField (quickTurnRightDirectionName);	
		EditorGUILayout.PropertyField (quickTurnLeftDirectionName);	
		EditorGUILayout.PropertyField (quickTurnDirectionIDSpeedName);

		EditorGUILayout.PropertyField (shieldActiveAnimatorName);

		GUILayout.EndVertical ();
	}

	void showCharacterComponents ()
	{
		showSectionTitle ("CHARACTER COMPONENTS");

		GUILayout.BeginVertical ("Characters Elements", "window");
		EditorGUILayout.PropertyField (playerCameraGameObject);
		EditorGUILayout.PropertyField (playerCameraTransform);
		EditorGUILayout.PropertyField (playerCameraManager);

		EditorGUILayout.PropertyField (headBobManager);
		EditorGUILayout.PropertyField (playerInput);

		EditorGUILayout.PropertyField (playerTransform);

		EditorGUILayout.PropertyField (weaponsManager);
		EditorGUILayout.PropertyField (healthManager);
		EditorGUILayout.PropertyField (IKSystemManager);
		EditorGUILayout.PropertyField (mainCameraTransform);
		EditorGUILayout.PropertyField (animator);
		EditorGUILayout.PropertyField (stepManager);
		EditorGUILayout.PropertyField (mainRigidbody);
		EditorGUILayout.PropertyField (gravityManager);
		EditorGUILayout.PropertyField (characterStateIconManager);
		EditorGUILayout.PropertyField (capsule);
		EditorGUILayout.PropertyField (mainCollider);
		EditorGUILayout.PropertyField (AIElements);
		EditorGUILayout.PropertyField (mainPlayerActionSystem);

		EditorGUILayout.PropertyField (mainExternalControllerBehaviorManager);
		GUILayout.EndVertical ();
	}

	void showAbilitiesSettings ()
	{
		showSectionTitle ("ABILITIES SETTINGS");

		abilitiesTabIndex = GUILayout.SelectionGrid (abilitiesTabIndex, abilitiesTabsOptions, 3);

		if (abilitiesTabIndex >= 0 && abilitiesTabIndex < abilitiesTabsOptions.Length) {
			
			switch (abilitiesTabsOptions [abilitiesTabIndex]) {
			case "Crouch Slide":
				showCrouchSlideSettings ();

				break;

			case "Wall Running":
				showWallRunningSettings ();

				break;

			case "Air Dash":
				showAirDashSettings ();

				break;

			case "Zero Gravity":
				showZeroGravitySettings ();

				break;

			case "Free Floating Mode":
				showFreeFloatingModeSettings ();

				break;

			case "Show All":
				showCrouchSlideSettings ();

				showWallRunningSettings ();

				showAirDashSettings ();

				showZeroGravitySettings ();

				showFreeFloatingModeSettings ();

				break;

			case "Hide All":
				abilitiesTabIndex = -1;

				break;

			default: 

				break;
			}
		}
	}

	void showCrouchSlideSettings ()
	{
		showSectionTitle ("CROUCH SLIDING SETTINGS");

		GUILayout.BeginVertical ("Crouch Sliding Settings", "window");
		EditorGUILayout.PropertyField (crouchSlidingEnabled);	
		if (crouchSlidingEnabled.boolValue) {
			EditorGUILayout.PropertyField (noAnimatorCrouchSlidingSpeed);
			EditorGUILayout.PropertyField (noAnimatorCrouchSlidingDuration);

			EditorGUILayout.PropertyField (noAnimatorCrouchSlidingLerpSpeed);
			EditorGUILayout.PropertyField (noAnimatorCrouchSlidingLerpDelay);

			EditorGUILayout.PropertyField (getUpAfterCrouchSlidingEnd);

			EditorGUILayout.PropertyField (keepCrouchSlidingOnInclinatedSurface);	
			if (keepCrouchSlidingOnInclinatedSurface.boolValue) {
				EditorGUILayout.PropertyField (minInclinitaonSurfaceAngleToCrouchcSliding);
			}

			EditorGUILayout.PropertyField (crouchSlidingOnAirEnabled);	
			if (crouchSlidingOnAirEnabled.boolValue) {
				EditorGUILayout.PropertyField (noAnimatorCrouchSlidingOnAirSpeed);
				EditorGUILayout.PropertyField (useNoAnimatorCrouchSlidingDurationOnAir);
				if (useNoAnimatorCrouchSlidingDurationOnAir.boolValue) {
					EditorGUILayout.PropertyField (noAnimatorCrouchSlidingDurationOnAir);
				}
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnCrouchSlidingStart);
			EditorGUILayout.PropertyField (eventOnCrouchSlidingEnd);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useCrouchSlidingOnThirdPersonEnabled);

			if (useCrouchSlidingOnThirdPersonEnabled.boolValue) {
				EditorGUILayout.PropertyField (eventOnCrouchSlidingThirdPersonStart);	
				EditorGUILayout.PropertyField (eventOnCrouchSlidingThirdPersonEnd);
			}
		}
		GUILayout.EndVertical ();
	}

	void showWallRunningSettings ()
	{

		showSectionTitle ("WALL RUNNING SETTINGS");

		GUILayout.BeginVertical ("Wall Running Settings", "window");

		EditorGUILayout.PropertyField (wallRunnigEnabled);
		EditorGUILayout.PropertyField (wallRunningExternalControllerBehavior);
			
		GUILayout.EndVertical ();
	}

	void showAirDashSettings ()
	{
		showSectionTitle ("AIR DASH SETTINGS");

		GUILayout.BeginVertical ("Air Dash Settings", "window");
		EditorGUILayout.PropertyField (airDashEnabled);
		if (airDashEnabled.boolValue) {
			EditorGUILayout.PropertyField (airDashForce);
			EditorGUILayout.PropertyField (airDashColdDown);
			EditorGUILayout.PropertyField (pauseGravityForce);
			if (pauseGravityForce.boolValue) {
				EditorGUILayout.PropertyField (gravityForcePausedTime);
			}
			EditorGUILayout.PropertyField (resetGravityForceOnDash);
			EditorGUILayout.PropertyField (useDashLimit);
			if (useDashLimit.boolValue) {
				EditorGUILayout.PropertyField (dashLimit);
			}
			EditorGUILayout.PropertyField (changeCameraFovOnDash);
			if (changeCameraFovOnDash.boolValue) {
				EditorGUILayout.PropertyField (cameraFovOnDash);
				EditorGUILayout.PropertyField (cameraFovOnDashSpeed);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useEventsOnAirDash);
			if (useEventsOnAirDash.boolValue) {
				EditorGUILayout.PropertyField (eventOnAirDashThirdPerson);
				EditorGUILayout.PropertyField (eventOnAirDashFirstPerson);
			}
		}
		GUILayout.EndVertical ();
	}

	void showZeroGravitySettings ()
	{
		showSectionTitle ("ZERO GRAVITY SETTINGS");

		GUILayout.BeginVertical ("Zero Gravity Mode Settings", "window");
		EditorGUILayout.PropertyField (zeroGravityMovementSpeed);
		EditorGUILayout.PropertyField (zeroGravityControlSpeed);
		EditorGUILayout.PropertyField (zeroGravityLookCameraSpeed);
		EditorGUILayout.PropertyField (useGravityDirectionLandMark);
		if (useGravityDirectionLandMark.boolValue) {
			EditorGUILayout.PropertyField (forwardSurfaceRayPosition);
			EditorGUILayout.PropertyField (maxDistanceToAdjust);
		}
		EditorGUILayout.PropertyField (pauseCheckOnGroundStateZG);
		EditorGUILayout.PropertyField (pushPlayerWhenZeroGravityModeIsEnabled);
		if (pushPlayerWhenZeroGravityModeIsEnabled.boolValue) {
			EditorGUILayout.PropertyField (pushZeroGravityEnabledAmount);
		}
		EditorGUILayout.PropertyField (canMoveVerticallyOnZeroGravity);
		EditorGUILayout.PropertyField (canMoveVerticallyAndHorizontalZG);
		EditorGUILayout.PropertyField (zeroGravitySpeedMultiplier);
		EditorGUILayout.PropertyField (zeroGravityModeVerticalSpeed);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zero Gravity And Free Floating Mode Settings", "window");
		EditorGUILayout.PropertyField (useMaxAngleToCheckOnGroundStateZGFF);
		if (useMaxAngleToCheckOnGroundStateZGFF.boolValue) {
			EditorGUILayout.PropertyField (maxAngleToChekOnGroundStateZGFF);
		}
		GUILayout.EndVertical ();
	}

	void showFreeFloatingModeSettings ()
	{
		showSectionTitle ("FREE FLOATING MODE SETTINGS");

		GUILayout.BeginVertical ("Free Floating Mode Settings", "window");
		EditorGUILayout.PropertyField (freeFloatingMovementSpeed);
		EditorGUILayout.PropertyField (freeFloatingControlSpeed);

		EditorGUILayout.PropertyField (pauseCheckOnGroundStateFF);
		EditorGUILayout.PropertyField (canMoveVerticallyOnFreeFloating);
		EditorGUILayout.PropertyField (canMoveVerticallyAndHorizontalFF);
		EditorGUILayout.PropertyField (freeFloatingSpeedMultiplier);
		EditorGUILayout.PropertyField (pushFreeFloatingModeEnabledAmount);
		EditorGUILayout.PropertyField (freeFloatingModeVerticalSpeed);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zero Gravity And Free Floating Mode Settings", "window");
		EditorGUILayout.PropertyField (useMaxAngleToCheckOnGroundStateZGFF);
		if (useMaxAngleToCheckOnGroundStateZGFF.boolValue) {
			EditorGUILayout.PropertyField (maxAngleToChekOnGroundStateZGFF);
		}
		GUILayout.EndVertical ();
	}

	void showDebugSettings ()
	{
		showSectionTitle ("DEBUG SETTINGS");

		GUILayout.BeginVertical ("Player State", "window");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Controller State", "window");
		GUILayout.Label ("On Ground \t" + playerOnGround.boolValue);
		GUILayout.Label ("Is Moving \t\t" + isMoving.boolValue);
		GUILayout.Label ("Jumping \t\t" + jump.boolValue);
		GUILayout.Label ("Running \t\t" + running.boolValue);
		GUILayout.Label ("Crouching \t\t" + crouching.boolValue);
		GUILayout.Label ("Can Move \t\t" + canMove.boolValue);
		GUILayout.Label ("Move Input Paused\t" + moveIputPaused.boolValue);
		GUILayout.Label ("Slowing Fall \t" + slowingFall.boolValue);
		GUILayout.Label ("Is Dead \t\t" + isDead.boolValue);
		GUILayout.Label ("Child Of Parent \t" + playerSetAsChildOfParent.boolValue);
		GUILayout.Label ("Relative Movement \t" + useRelativeMovementToLockedCamera.boolValue);
		GUILayout.Label ("Ladder Found \t" + ladderFound.boolValue);
		GUILayout.Label ("Ignore Camera Dir \t" + ignoreCameraDirectionOnMovement.boolValue);
		GUILayout.Label ("Strafe Active \t" + strafeModeActive.boolValue);
		GUILayout.Label ("Footsteps State \t" + manager.currentFootStepState);
		GUILayout.Label ("Head Bob State \t" + manager.currentHeadbobState);
		GUILayout.Label ("Action Active \t" + actionActive.boolValue);
		GUILayout.Label ("Root Motion Active \t" + rootMotionCurrentlyActive.boolValue);
		GUILayout.Label ("Generic Model Active \t" + usingGenericModelActive.boolValue);

		GUILayout.Label ("Rigidbody Velocity\t" + currentVelocity.vector3Value);
		GUILayout.Label ("Axis Values\t" + axisValues.vector2Value);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ground Adherence State", "window");
		GUILayout.Label ("Adhere To Ground \t" + adhereToGround.boolValue);
		GUILayout.Label ("Distance To Ground \t" + distanceToGround.floatValue);
		GUILayout.Label ("Surface Angle \t" + hitAngle.floatValue);
		EditorGUILayout.Space ();

		GUILayout.Label ("Slope Found \t" + slopeFound.boolValue);
		GUILayout.Label ("Slope Up \t\t" + movingOnSlopeUp.boolValue);
		GUILayout.Label ("Slope Down \t" + movingOnSlopeDown.boolValue);

		EditorGUILayout.Space ();

		GUILayout.Label ("Stairs Found \t" + stairsFound.boolValue);
		GUILayout.Label ("Stair System Found \t" + stairAdherenceSystemDetected.boolValue);

		EditorGUILayout.Space ();

		GUILayout.Label ("Wall Running \t" + wallRunningActive.boolValue);
		GUILayout.Label ("Crouch Slide Active \t" + crouchSlidingActive.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapons And Powers State", "window");
		GUILayout.Label ("Aiming In 3rd Person \t" + aimingInThirdPerson.boolValue);
		GUILayout.Label ("Aiming In 1st Person \t" + aimingInFirstPerson.boolValue);
		GUILayout.Label ("Player Is Aiming \t" + playerIsAiming.boolValue);
		GUILayout.Label ("Using Free Fire Mode \t" + usingFreeFireMode.boolValue);
		GUILayout.Label ("Look Camera Direction Free Fire Active \t" + lookInCameraDirectionOnFreeFireActive.boolValue);
		GUILayout.Label ("Using Melee Weapons \t" + playerUsingMeleeWeapons.boolValue);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Mode State", "window");
		GUILayout.Label ("Jet Pack Equipped \t" + jetPackEquiped.boolValue);
		GUILayout.Label ("Using Jet Pack \t" + usingJetpack.boolValue);
		GUILayout.Label ("Sphere Mode Active \t" + sphereModeActive.boolValue);
		GUILayout.Label ("Fly Mode Active \t" + flyModeActive.boolValue);
		GUILayout.Label ("Swim Mode Active \t" + swimModeActive.boolValue);
		GUILayout.Label ("Combat Mode Active \t" + usingCloseCombatActive.boolValue);
		GUILayout.Label ("Close Combat Attack Active\t" + closeCombatAttackInProcess.boolValue);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("External Controller Behavior State", "window");
		GUILayout.Label ("External Controller Active \t" + externalControllBehaviorActive.boolValue);
		EditorGUILayout.PropertyField (currentExternalControllerBehavior);
	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera State", "window");
		GUILayout.Label ("Camera Locked \t" + lockedCameraActive.boolValue);
		GUILayout.Label ("Look Camera Direction Active \t" + lookInCameraDirectionActive.boolValue);
		GUILayout.Label ("First Person Active \t" + firstPersonActive.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Elements State", "window");
		GUILayout.Label ("Using Device \t" + usingDevice.boolValue);
		GUILayout.Label ("Visible To AI \t" + visibleToAI.boolValue);
		GUILayout.Label ("Stealth Active \t" + stealthModeActive.boolValue);
		GUILayout.Label ("NavMesh Enabled \t" + playerNavMeshEnabled.boolValue);
		GUILayout.Label ("Ignore External Actions \t" + ignoreExternalActionsActiveState.boolValue);
		GUILayout.Label ("Moving On Platform \t" + movingOnPlatformActive.boolValue);
		GUILayout.Label ("AI Can Move \t" + canMoveAI.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Menu Elements State", "window");
		GUILayout.Label ("Using SubMenu \t" + usingSubMenu.boolValue);
		GUILayout.Label ("Player Menu Active \t" + playerMenuActive.boolValue);
		GUILayout.Label ("Game Paused \t" + gamePaused.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle State", "window");
		GUILayout.Label ("Is Driving \t\t" + driving.boolValue);
		GUILayout.Label ("Driving Remotely \t" + drivingRemotely.boolValue);
		GUILayout.Label ("Overriding Element \t" + overridingElement.boolValue);
		GUILayout.Label ("Vehicle Name \t\t" + currentVehicleName.stringValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gravity State", "window");
		GUILayout.Label ("Gravity Force Paused \t" + gravityForcePaused.boolValue);
		GUILayout.Label ("Zero Gravity On \t" + zeroGravityModeOn.boolValue);
		GUILayout.Label ("Free Floating On \t" + freeFloatingModeOn.boolValue);
		GUILayout.Label ("Current Normal \t" + currentNormal.vector3Value);
		GUILayout.Label ("Check Ground Paused FF ZG \t" + checkOnGroundStatePausedFFOrZG.boolValue);
		GUILayout.Label ("Check Ground Paused \t\t" + checkOnGroundStatePaused.boolValue);
		GUILayout.Label ("Gravity Power Active \t" + gravityPowerActive.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showDebugPrint);
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoColor);
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (gizmoRadius);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
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
}
#endif