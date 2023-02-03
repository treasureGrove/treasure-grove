using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class playerController : MonoBehaviour
{
	[Header ("Movement Settings")]
	[Space]

	[Tooltip ("Sets how high/far the player will jump under normal circumstances.")]
	public float jumpPower = 12;

	public float airSpeed = 6;
	public float airControl = 2;

	public bool ignorePlayerExtraRotationOnAirEnabled;

	public float stationaryTurnSpeed = 180;
	public float movingTurnSpeed = 200;

	public bool useTurnSpeedOnAim = true;

	public float autoTurnSpeed = 2;
	public float aimTurnSpeed = 10;

	public float thresholdAngleDifference = 5;

	public bool regularMovementOnBulletTime;

	public bool useMoveInpuptMagnitedeToForwardAmount;

	[Space]
	[Header ("Animator Settings")]
	[Space]

	public int baseLayerIndex = 0;

	public float inputHorizontalLerpSpeed = 0.03f;
	public float inputVerticalLerpSpeed = 0.03f;
	public float inputStrafeHorizontalLerpSpeed = 0.3f;
	public float inputStrafeVerticalLerpSpeed = 0.3f;

	public float animatorForwardInputLerpSpeed = 0.1f;
	public float animatorTurnInputLerpSpeed = 0.1f;

	public float moveSpeedMultiplier = 1;
	public float animSpeedMultiplier = 1;

	public float minTimeToSetIdleOnStrafe = 0.5f;

	public bool overrideAnimationSpeedActive;

	public float characterRadius;

	public bool usingAnimator = true;

	[Space]
	[Header ("Animator ID State")]
	[Space]

	public int currentIdleID;

	public int currentStrafeID;

	public int currentAirID;

	public int currentCrouchID;

	public int currentAirSpeed = 1;

	public int currentShieldActive;

	[Space]
	[Header ("Tank Controls Animator Settings")]
	[Space]

	public float inputTankControlsHorizontalLerpSpeed = 0.3f;
	public float inputTankControlsVerticalLerpSpeed = 0.3f;
	public float inputTankControlsHorizontalStrafeLerpSpeed = 0.06f;
	public float inputTankControlsVerticalStrafeLerpSpeed = 0.06f;


	[Space]
	[Header ("Player Status ID Settings")]
	[Space]

	public int playerStatusID;
	public float playerStatusIDLerpSpeed = 0.1f;

	[Space]
	[Header ("Ground Detection Settings")]
	[Space]

	public LayerMask layer;
	public float rayDistance;
	public bool updateFootStepStateActive = true;

	public bool useSphereRaycastForGroundDetection;

	public float sphereCastRadius = 0.4f;
	public float maxDistanceSphereCast = 0.5f;

	public float sphereCastOffset;

	public float maxExtraRaycastDistanceToGroundDetection = 0.4f;

	[Space]
	[Header ("Walk/Run Sprint Settings")]
	[Space]

	[Range (0, 1)]public float walkSpeed = 1;

	public bool increaseWalkSpeedEnabled;
	public float increaseWalkSpeedValue = 1;
	public bool holdButtonToKeepIncreasedWalkSpeed;

	[TextArea (3, 8)] public string walkRunTip = "Set 'Sprint Enabled' field to false and change 'Walk Speed' to 0.5 to start the " +
	                                             "game walking and toggle to run. Or set 'Increase Walk Speed Value' to 0.5 to vice versa.";

	[Space]
	[Header ("Sprint Settings")]
	[Space]

	public bool sprintEnabled = true;
	public bool changeCameraFovOnSprint = true;
	public bool shakeCameraOnSprintThirdPerson = true;
	public string sprintThirdPersonCameraShakeName = "Sprint Shake";

	public bool canRunThirdPersonActive = true;

	public bool useSecondarySprintValues = true;
	public float sprintVelocity = 1;
	public float sprintJumpPower = 15;
	public float sprintAirSpeed = 20;
	public float sprintAirControl = 4;

	public bool runOnCrouchEnabled = true;

	[Space]
	[Header ("Ground Adherence Settings")]
	[Space]

	public float regularGroundAdherence = 5;
	public float slopesGroundAdherenceUp = 2;
	public float slopesGroundAdherenceDown = 4;

	[Range (0, 1)] public float maxRayDistanceRange = 0.5f;
	[Range (0, 1)] public float maxSlopeRayDistance = 0.5f;
	[Range (0, 1)] public float maxStairsRayDistance = 0.5f;

	public bool useMaxWalkSurfaceAngle;
	public float maxWalkSurfaceAngle;

	public bool useMaxDistanceToCheckSurfaceAngle;
	public float maxDistanceToCheckSurfaceAngle;

	public bool useMaxSlopeInclination = true;
	public float maxSlopeInclination = 80;
	public float minSlopeInclination = -80;


	[Space]
	[Header ("Root Motion Settings")]
	[Space]

	public bool useRootMotionActive = true;
	public float noRootLerpSpeed;
	public float noRootWalkMovementSpeed;
	public float noRootRunMovementSpeed;
	public float noRootSprintMovementSpeed;
	public float noRootCrouchMovementSpeed;
	public float noRootWalkStrafeMovementSpeed;
	public float noRootRunStrafeMovementSpeed;

	public bool disableRootMotionTemporalyOnLandThirdPerson;
	public float durationDisableRootMotionTemporalyOnLandThirdPerson;
	public float waitToDisableRootMotionTemporalyOnLandThirdPerson;

	[Space]
	[Header ("Player ID Settings")]
	[Space]


	public int playerID;

	[Space]
	[Header ("AI Settings")]
	[Space]

	public bool usedByAI;

	[Space]
	[Header ("First Person Settings")]
	[Space]

	public float noAnimatorSpeed;
	public float noAnimatorWalkMovementSpeed;
	public float noAnimatorRunMovementSpeed;
	public float noAnimatorCrouchMovementSpeed;
	public float noAnimatorRunCrouchMovementSpeed;
	public float noAnimatorStrafeMovementSpeed;
	public bool noAnimatorCanRun;
	public float noAnimatorWalkBackwardMovementSpeed;
	public float noAnimatorRunBackwardMovementSpeed;
	public float noAnimatorCrouchBackwardMovementSpeed;
	public float noAnimatorRunCrouchBackwardMovementSpeed;
	public float noAnimatorStrafeBackwardMovementSpeed;

	public bool noAnimatorCanRunBackwards;
	public float noAnimatorAirSpeed = 11;

	public float maxVelocityChange;

	public float noAnimatorMovementSpeedMultiplier = 1;

	public float noAnimatorSlopesGroundAdherenceUp = 1;
	public float noAnimatorSlopesGroundAdherenceDown = 8;

	public float noAnimatorStairsGroundAdherence = 2;

	[Space]
	[Header ("Advanced Settings")]
	[Space]

	[Space]
	[Header ("Sprint Settings")]
	[Space]

	public bool useEventOnSprint;
	public UnityEvent eventOnStartSprint;
	public UnityEvent eventOnStopSprint;

	[Space]
	[Header ("Look In Camera Direction Settings")]
	[Space]

	public bool lookAlwaysInCameraDirection;
	public bool lookInCameraDirectionIfLookingAtTarget;
	public bool lookOnlyIfMoving;

	public bool lookInCameraDirectionOnCrouchState;


	public bool disableLookAlwaysInCameraDirectionIfIncreaseWalkSpeedActive;

	public float defaultStrafeWalkSpeed = 1;
	public float defaultStrafeRunSpeed = 2;
	public bool rotateDirectlyTowardCameraOnStrafe;
	public float strafeLerpSpeed = 0.1f;

	public bool updateHeadbobState;

	[Space]
	[Header ("Quick Movement Turn Settings")]
	[Space]

	public bool checkQuickMovementTurn180DegreesOnRun;
	public float minDelayToActivateQuickMovementTurn180OnRun = 2;
	public float quickTurnMovementDurationWalking = 1;
	public float quickTurnMovementDurationRunning = 0.8f;
	public float quickTurnMovementDurationSprinting = 2.1f;

	public float quickTurnMovementRotationSpeed = 2;

	[Space]
	[Header ("Character Mesh Settings")]
	[Space]

	public GameObject characterMeshGameObject;

	public List<GameObject> extraCharacterMeshGameObject = new List<GameObject> ();

	public eventParameters.eventToCallWithBool characterMeshesListToDisableOnEvent;

	public bool checkCharacterMeshIfGeneratedOnStart;

	public GameObject objectToCheckCharacterIfGeneratedOnStart;

	[Space]
	[Header ("Move While Aim Settings")]
	[Space]

	public bool canMoveWhileAimFirstPerson = true;
	public bool canMoveWhileAimThirdPerson = true;

	[Space]
	[Header ("Stairs Settings")]
	[Space]

	public float stairsMinValue = 0.2f;
	public float stairsMaxValue = 0.25f;

	public float stairsGroundAdherence = 8;

	public bool checkStairsWithInclination;
	public float minStairInclination = -10;
	public float maxStairInclination = 10;

	public bool checkForStairAdherenceSystem;

	public float currentStairAdherenceSystemMaxValue;
	public float currentStairAdherenceSystemMinValue;
	public float currentStairAdherenceSystemAdherenceValue;

	[Space]
	[Header ("Locked Camera Settings")]
	[Space]

	public lockedPlayerMovementMode lockedPlayerMovement;

	[Range (0, 1)] public float tankModeRotationSpeed;
	public bool canMoveWhileAimLockedCamera = true;

	public bool crouchVerticalInput2_5dEnabled;

	[Space]
	[Header ("Gravity Settings")]
	[Space]

	public float gravityMultiplier = 2;
	public float gravityForce = -9.8f;

	public bool useMaxFallSpeed;
	public float maxFallSpeed;

	[Space]
	[Header ("Physics Settings")]
	[Space]

	public PhysicMaterial zeroFrictionMaterial;
	public PhysicMaterial highFrictionMaterial;

	[Space]
	[Header ("Jump Settings")]
	[Space]

	public bool enabledRegularJump;
	public bool enabledDoubleJump;
	public int maxNumberJumpsInAir;

	public bool useNewDoubleJumpPower;
	public float doubleJumpPower = 10;

	public bool removeVerticalSpeedOnDoubleJump;

	public bool holdJumpSlowDownFallEnabled = true;

	public float slowDownGravityMultiplier;

	public float readyToJumpTime = 0.25f;

	public bool addJumpForceWhileButtonPressed;
	public float addJumpForceDuration;
	public float jumpForceAmontWhileButtonPressed;

	public float minWaitToAddJumpForceAfterButtonPressed;
	public bool useJumpForceOnAirJumpEnabled;

	public bool useEventOnJump;
	public UnityEvent eventOnJump;

	public bool stopCrouchOnJumpEnabled;

	[Space]
	[Header ("Fall Damage Settings")]
	[Space]

	public bool fallDamageEnabled;
	[Tooltip ("The time a player can be in the air and falling before they may receive damage.")]
	public float maxTimeInAirBeforeGettingDamage = 4;

	public float fallingDamageMultiplier = 0.2f;

	public float minPlayerVelocityToApplyDamageThirdPerson = 15;
	public float minPlayerVelocityToApplyDamageFirstPerson = 10;

	public bool ignoreShieldOnFallDamage;

	public int damageTypeIDOnFallDamage = -1;

	public bool applyFallDamageEnabled = true;

	bool fallDamageCheckOnHealthPaused;

	public bool callEventOnFallDamage;

	public float minTimeOnAirToUseEvent;

	public bool callEventOnlyIfPlayerAlive;
	public UnityEvent eventOnFallDamage;

	[Space]
	[Header ("Land Mark Settings")]
	[Space]

	public bool useLandMark;
	public float maxLandDistance;
	public float minDistanceShowLandMark;
	public GameObject landMark;
	public Transform landMark1;
	public Transform landMark2;

	[Space]
	[Header ("Player Modes Settings")]
	[Space]

	public bool canUseSphereMode;

	[Space]
	[Header ("Vehicle Settings")]
	[Space]

	public bool canGetOnVehicles;
	public bool canDrive;

	[Space]
	[Header ("Crouch Settings")]
	[Space]

	public float capsuleHeightOnCrouch = 1;

	public bool getUpIfJumpOnCrouchInThirdPerson;
	public bool getUpIfJumpOnCrouchInFirstPerson;

	public bool useAutoCrouch;

	public LayerMask layerToCrouch;

	public float raycastDistanceToAutoCrouch;
	public Transform autoCrouchRayPosition;
	public Vector3 secondRaycastOffset;

	public bool canCrouchWhenUsingWeaponsOnThirdPerson = true;

	[Space]
	[Header ("Obstacle Detection To Avoid Movement Settings")]
	[Space]

	public bool useObstacleDetectionToAvoidMovement;
	public float obstacleDetectionToAvoidMovementRaycastDistance = 0.5f;
	public float obstacleDetectionRaycastDistanceRightAndLeft = 0.1f;
	public float obstacleDetectionRaycastHeightOffset;

	public LayerMask obstacleDetectionToAvoidMovementLayermask;

	[Space]
	[Header ("Abilities Settings")]
	[Space]

	[Space]
	[Header ("Crouch Sliding Settings")]
	[Space]

	public bool crouchSlidingEnabled;
	public float noAnimatorCrouchSlidingSpeed;

	public float noAnimatorCrouchSlidingDuration;

	public float noAnimatorCrouchSlidingLerpSpeed;
	public float noAnimatorCrouchSlidingLerpDelay;

	public bool getUpAfterCrouchSlidingEnd;

	public bool keepCrouchSlidingOnInclinatedSurface;
	public float minInclinitaonSurfaceAngleToCrouchcSliding;

	public bool crouchSlidingOnAirEnabled;

	public float noAnimatorCrouchSlidingOnAirSpeed;

	public bool useNoAnimatorCrouchSlidingDurationOnAir;
	public float noAnimatorCrouchSlidingDurationOnAir;

	public UnityEvent eventOnCrouchSlidingStart;
	public UnityEvent eventOnCrouchSlidingEnd;

	public bool useCrouchSlidingOnThirdPersonEnabled;

	public UnityEvent eventOnCrouchSlidingThirdPersonStart;
	public UnityEvent eventOnCrouchSlidingThirdPersonEnd;

	bool slidindOnThirdPersonActive;

	[Space]
	[Header ("Wall Running Settings")]
	[Space]

	public bool wallRunnigEnabled;

	public externalControllerBehavior wallRunningExternalControllerBehavior;

	[Space]
	[Header ("Air Dash Settings")]
	[Space]

	public bool airDashEnabled;
	public float airDashForce = 20;
	public float airDashColdDown = 0.5f;
	public bool pauseGravityForce;
	public float gravityForcePausedTime = 1;
	public bool resetGravityForceOnDash;
	public bool useDashLimit;
	public int dashLimit;
	public bool changeCameraFovOnDash;
	public float cameraFovOnDash;
	public float cameraFovOnDashSpeed;

	[Space]

	public bool useEventsOnAirDash;
	public UnityEvent eventOnAirDashThirdPerson;
	public UnityEvent eventOnAirDashFirstPerson;

	[Space]
	[Header ("Zero Gravity Settings")]
	[Space]

	public float zeroGravityMovementSpeed;
	public float zeroGravityControlSpeed;
	public float zeroGravityLookCameraSpeed;
	public bool useGravityDirectionLandMark;
	public Transform forwardSurfaceRayPosition;
	public float maxDistanceToAdjust;
	public bool pauseCheckOnGroundStateZG;
	public bool pushPlayerWhenZeroGravityModeIsEnabled;
	public float pushZeroGravityEnabledAmount;
	public bool canMoveVerticallyOnZeroGravity;
	public bool canMoveVerticallyAndHorizontalZG;
	public float zeroGravitySpeedMultiplier = 1.4f;
	float currentZeroGravitySpeedMultiplier = 1;
	public float zeroGravityModeVerticalSpeed = 2;

	public bool useMaxAngleToCheckOnGroundStateZGFF;
	public float maxAngleToChekOnGroundStateZGFF;

	[Space]
	[Header ("Free Floating Mode Settings")]
	[Space]

	public float freeFloatingMovementSpeed;
	public float freeFloatingControlSpeed;
	public bool pauseCheckOnGroundStateFF;
	public bool canMoveVerticallyOnFreeFloating;
	public bool canMoveVerticallyAndHorizontalFF;
	public float freeFloatingSpeedMultiplier = 1.4f;
	public float pushFreeFloatingModeEnabledAmount;
	public float freeFloatingModeVerticalSpeed = 2;

	[Space]
	[Header ("Debug")]
	[Space]

	[Space]
	[Header ("Controller State")]
	[Space]

	public bool playerOnGround;
	public bool isMoving;

	public Vector3 moveInput;

	public bool jump;
	public bool running;
	public bool crouching = false;
	public bool canMove = true;
	public bool slowingFall;

	public bool isDead;

	float lastTimeResurrect;

	public bool playerSetAsChildOfParent;

	Transform currentTemporalPlayerParent;


	public bool useRelativeMovementToLockedCamera;
	public bool ladderFound;

	public bool ignoreCameraDirectionOnMovement;
	public bool strafeModeActive;

	public footStepState currentFootStepState;

	public headbobState currentHeadbobState;
	public bool actionActive;
	public bool rootMotionCurrentlyActive;

	public bool usingGenericModelActive;

	public bool canMoveAI = true;

	[Space]
	[Header ("Ground Adherence State")]
	[Space]

	public bool adhereToGround;

	public float distanceToGround;
	public float hitAngle;

	public bool slopeFound;
	public bool movingOnSlopeUp;
	public bool movingOnSlopeDown;

	public bool stairsFound;
	public bool stairAdherenceSystemDetected;
	public bool wallRunningActive;
	public bool crouchSlidingActive;

	[Space]
	[Header ("Weapons And Powers State")]
	[Space]

	public bool playerIsAiming;
	public bool aimingInThirdPerson;
	public bool aimingInFirstPerson;
	public bool usingFreeFireMode;
	public bool lookInCameraDirectionOnFreeFireActive;

	public bool playerUsingMeleeWeapons;

	bool playerMeleeWeaponThrown;

	[Space]
	[Header ("Mode State")]
	[Space]

	public bool jetPackEquiped;
	public bool usingJetpack;

	public bool flyModeActive;

	public bool swimModeActive;
	public bool sphereModeActive;

	public bool enableExternalForceOnSwimMode;

	public bool enableExternalForceOnFlyMode;

	[Space]
	[Header ("Camera State")]
	[Space]

	public bool lockedCameraActive;
	public bool lookInCameraDirectionActive;
	public bool firstPersonActive;

	[Space]
	[Header ("Other Elements State")]
	[Space]

	public bool usingDevice;

	public bool visibleToAI = true;
	public bool stealthModeActive;
	public bool playerNavMeshEnabled;
	public bool ignoreExternalActionsActiveState;

	public bool movingOnPlatformActive;

	[Space]
	[Header ("Menu Elements State")]
	[Space]

	public bool usingSubMenu;
	public bool playerMenuActive;
	public bool gamePaused;

	[Space]
	[Header ("Vehicle State")]
	[Space]

	public bool driving;
	public bool drivingRemotely;
	public bool overridingElement;
	public string currentVehicleName;

	[Space]
	[Header ("Gravity State")]
	[Space]

	public bool gravityForcePaused;
	public bool zeroGravityModeOn;
	public bool freeFloatingModeOn;
	public Vector3 currentNormal = new Vector3 (0, 1, 0);
	public bool checkOnGroundStatePausedFFOrZG;
	public bool checkOnGroundStatePaused;

	[Space]
	[Header ("Gizmo State")]
	[Space]

	public bool showDebugPrint;
	public bool showGizmo;
	public Color gizmoColor;
	public Color gizmoLabelColor;
	public float gizmoRadius;

	//ANIMATOR VALUES
	[Space]
	[Header ("ANIMATOR ID VALUES")]
	[Space]

	public string forwardAnimatorName = "Forward";
	public string turnAnimatorName = "Turn";

	public string horizontalAnimatorName = "Horizontal";
	public string verticalAnimatorName = "Vertical";

	public string horizontalStrafeAnimatorName = "Horizontal Strafe";
	public string verticalStrafeAnimatorName = "Vertical Strafe";

	public string onGroundAnimatorName = "OnGround";
	public string crouchAnimatorName = "Crouch";
	public string movingAnimatorName = "Moving";
	public string jumpAnimatorName = "Jump";
	public string jumpLegAnimatorName = "JumpLeg";

	public string strafeModeActiveAnimatorName = "Strafe Mode Active";
	public string movementInputActiveAnimatorName = "Movement Input Active";
	public string movementRelativeToCameraAnimatorName = "Movement Relative To Camera";
	public string movementIDAnimatorName = "Movement ID";
	public string playerModeIDAnimatorName = "Player Mode ID";
	public string movementSpeedAnimatorName = "Movement Speed";
	public string lastTimeInputPressedAnimatorName = "Last Time Input Pressed";

	public string carryingWeaponAnimatorName = "Carrying Weapon";
	public string aimingModeActiveAnimatorName = "Aiming Mode Active";

	public string playerStatusIDAnimatorName = "Player Status ID";

	public string idleIDAnimatorName = "Idle ID";

	public string strafeIDAnimatorName = "Strafe ID";

	public string crouchIDAnimatorName = "Crouch ID";

	public string airIDAnimatorName = "Air ID";

	public string airSpeedAnimatorName = "Air Speed";

	public string quickTurnRightDirectionName = "Quick Turn Right Direction";
	public string quickTurnLeftDirectionName = "Quick Turn Left Direction";
	public string quickTurnDirectionIDSpeedName = "Quick Turn Direction ID";

	public string shieldActiveAnimatorName = "Shield Active";

	public string inputAmountName = "Input Amount";

	[Space]
	[Header ("Character Components")]
	[Space]

	//Player components
	public headBob headBobManager;
	public playerInputManager playerInput;
	public playerWeaponsManager weaponsManager;
	public healthManagement healthManager;
	public IKSystem IKSystemManager;

	public Transform playerTransform;

	//Camera variables
	public GameObject playerCameraGameObject;
	public Transform playerCameraTransform;
	public Transform mainCameraTransform;
	public cameraControllerManager playerCameraManager;
	public Animator animator;
	public footStepManager stepManager;
	public Rigidbody mainRigidbody;
	public gravityObjectManager gravityManager;
	public characterStateIconSystem characterStateIconManager;
	public CapsuleCollider capsule;
	public Collider mainCollider;

	public GameObject AIElements;

	public playerActionSystem mainPlayerActionSystem;

	public bool useEventsOnWalk;
	public UnityEvent eventOnWalkStart;
	public UnityEvent eventOnWalkEnd;

	public bool useEventsOnRun;
	public UnityEvent eventOnRunStart;
	public UnityEvent eventOnRunEnd;


	public bool useEventsOnEnableDisableCharacterMeshes;
	public UnityEvent eventOnEnableCharacterMeshes;
	public UnityEvent eventOnDisableCharacterMeshes;
	public UnityEvent eventOnEnableCharacterMeshesOnEditor;
	public UnityEvent eventOnDisableCharacterMeshesOnEditor;

	bool playeCurrentlyUsingInput;

	int forwardAnimatorID;
	int turnAnimatorID;
	int horizontalAnimatorID;
	int verticalAnimatorID;

	int horizontalStrafeAnimatorID;
	int verticalStrafeAnimatorID;

	int onGroundAnimatorID;
	int crouchAnimatorID;
	int movingAnimatorID;
	int jumpAnimatorID;
	int jumpLegAnimatorID;

	int strafeModeActiveAnimatorID;
	int movementInputActiveAnimatorID;
	int movementRelativeToCameraAnimatorID;
	int movementIDAnimatorID;
	int playerModeIDAnimatorID;
	int movementSpeedAnimatorID;
	int lastTimeInputPressedAnimatorID;

	int carryingWeaponAnimatorID;
	int aimingModeActiveAnimatorID;

	int playerStatusIDAnimatorID;

	int idleIDAnimatorID;

	int strafeIDAnimatorID;

	int crouchIDAnimatorID;

	int airIDAnimatorID;

	int airSpeedAnimatorID;

	int quickTurnRightDirectionID;
	int quickTurnLeftDirectionID;
	int quickTurnDirectionIDSpeedID;

	int shieldActiveAnimatorID;

	int inputAmountID;

	Vector3 vector3Zero = Vector3.zero;
	Quaternion quaternionIdentity = Quaternion.identity;

	Vector3 Vector3Up = Vector3.up;

	Vector3 playerTransformUp;
	Vector3 playerTransformForward;
	Vector3 playerTransformRight;

	float half = 0.5f;


	//OTHER FIELDS
	[HideInInspector] public bool canMoveVertically;
	[HideInInspector] public bool canMoveVerticallyAndHorizontal;
	[HideInInspector] public bool movingVertically;

	float currentFreeFloatingSpeedMultiplier = 1;

	float originalStationaryTurnSpeed;

	float originalWalkSpeedValue;

	bool increaseWalkSpeedActive;

	float currentHorizontalInputLerpSpeed;
	float currentVerticalInputLerpSpeed;
	float currentInputStrafeHorizontalLerpSpeed;
	float currentInputStrafeVerticalLerpSpeed;

	//	public bool useWalkToRunAccelerationChange;
	//	public float walkToRunAccelerationSpeed = 0.6f;
	//	float currentWalkSpeedTarget;

	bool sprintCanBeUsed = true;

	//Input variables
	[HideInInspector] public float horizontalInput;
	[HideInInspector] public float verticalInput;

	[HideInInspector] public Vector2 axisValues;
	[HideInInspector] public Vector2 rawAxisValues;

	Vector3 secondaryMoveInput;
	Vector3 secondaryMove;
	Vector3 stairsMoveInput;
	Vector3 airMove;

	[HideInInspector] public Vector3 currentMoveInput;

	Vector3 lockedCameraMoveInput;
	Vector3 currentLockedCameraInput;
	Vector3 lockedMove;

	Vector3 currentAdherenceInput;

	bool overrideOnGroundAnimatorValueActive;
	float overrideOnGroundDuration;
	float lastTimeOverrideOnGroundAnimator;

	bool playerOnGroundAnimatorStateOnOverrideOnGround = true;

	bool playerOnGroundAnimatorStateOnOverrideOnGroundWithTime = true;

	[HideInInspector] public bool playerActionsInputEnabled = true;

	//Fall damage variables

	bool fallDamageCheckPaused;

	[HideInInspector] public float lastTimeFalling;

	float currentLastTimeFalling;
	float currentFallSpeed;

	bool checkFallState;

	bool checkFallStatePaused;

	//Stairs variables

	float currentStairMaxValue;
	float currentStairMinValue;
	float currentStairAdherenceValue;

	GameObject currentDetectedSurface;
	GameObject currentTargetDetected;

	bool stairInclinationDetected;

	//Animator variables

	[HideInInspector] public bool usingAnimatorInFirstMode;
	float originalAnimationSpeed;

	[HideInInspector] public bool animatorSpeedUpdatePaused;

	[HideInInspector] public bool useFirstPersonPhysicsInThirdPersonActive;

	[HideInInspector] public bool lookInCameraDirectionOnFreeFire = true;

	[HideInInspector] public float currentNoRootMovementSpeed;
	[HideInInspector] public float lerpNoRootMovementSpeed;

	[HideInInspector] public bool originalSetUseRootMotionActiveState;

	float originalNoRootWalkMovementSpeed;
	float originalNoRootRunMovementSpeed;
	float originalNoRootSprintMovementSpeed;
	float originalNoRootCrouchMovementSpeed;
	float originalNoRootWalkStrafeMovementSpeed;
	float originalNoRootRunStrafeMovementSpeed;

	bool rootMotionDisabledTemporaly;

	bool checkingDisableRootMotionTemporaly;

	float lastTimeRootMotionDisabledTemporaly;

	[HideInInspector] public Vector3 characterVelocity;

	float lastTimeInput;
	float lastTimePlayerUserInput;

	bool lastTimeInputPressed;
	bool previousValueLastTimeInputPressed;

	bool previousValueOnGroundAnimatorState;
	bool onGroundAnimatorState;

	bool movementRelativeToCameraAnimatorState;
	bool previousValueMovementRelativeToCameraState;

	bool carryingWeaponAnimatorState;
	bool previousValueCarryingWeaponAnimatorState;

	bool aimingModeActiveAnimatorState;
	bool previousValueAimingModeActiveAnimatorState;

	bool tankModeActive;
	bool scrollModeActive;
	float currentMovementID;
	float currentPlayerModeID;
	float currentMovementSpeed;
	float currentHorizontalValue;
	float currentVerticalValue;

	[HideInInspector] public float noAnimatorCurrentMovementSpeed;

	[HideInInspector] public bool crouchSlidingOnAirActive;

	//States variables
	public bool gravityPowerActive;
	[HideInInspector] public bool choosingGravityDirection;

	[HideInInspector] public bool headBobPaused;

	[HideInInspector] public bool canRagdollMove = true;

	public bool usingCloseCombatActive;
	public bool closeCombatAttackInProcess;

	bool jumpLegExternallyActiveState;

	bool movingAnimator;
	bool previousValueMovingAnimator;

	public bool moveIputPaused;

	//Physics variables
	[HideInInspector] public Vector3 currentVelocity;
	RaycastHit hit;
	Ray crouchRay;

	RaycastHit temporalHit;

	RaycastHit sphereRaycastHit = new RaycastHit ();

	bool highFrictionMaterialActive;

	RaycastHit adherenceHit;

	bool physicMaterialAssigmentPaused;

	float currentNoAnimatorSlidingCrouchSpeed;

	float originalNoAnimatorMovementSpeedMultiplier;

	Vector3 noAnimatorCurrentForce;
	Vector3 velocityChange;
	[HideInInspector] public float currentVelocityChangeMagnitude;

	float lastFrameAirVelocity;

	float originalNoAnimWalkMovementSpeed;
	float originalNoAnimRunMovementSpeed;
	float originalNoAnimCrouchMovementSpeed;
	float originalNoAnimStrafeMovementSpeed;
	float originalNoAnimWalkBackwardMovementSpeed;
	float originalNoAnimRunBackwardMovementSpeed;
	float originalNoAnimCrouchBackwardMovementSpeed;
	float originalNoAnimStrafeBackwardMovementSpeed;
	bool originalNoAnimCanRun;

	//Ground variables
	bool onGroundChecked;
	float currentGroundAdherence;
	Transform currentSurfaceBelowPlayer;


	Vector3 hitPoint;
	Vector3 mainHitPoint;
	float mainHitAngle;

	Vector3 rayPos;
	bool canSetGroundState;
	float lastTimeGround;

	//Collider variables
	[HideInInspector] public float originalHeight;
	[HideInInspector] public float originalRadius;

	float currentHeight;

	//Air variables
	float lastTimeAir;

	//Gravity variables
	float originalGravityMultiplier;

	Vector3 extraGravityForce;
	float currentGravityMultiplier;
	float originalGravityForce;

	float lastTimeGravityForcePaused;
	bool unPauseGravityForceActive;

	bool movementSpeedIncreased;

	//Jump variables
	[HideInInspector] public bool jumpInput;
	[HideInInspector] public bool doubleJump;

	float readyToDoubleJumpTime = 0.2f;
	float lastJumpTime;
	float lastDoubleJumpTime;
	int jumpsAmount;
	float currentJumpLeg;
	bool canJumpActive = true;
	float currentJumpAnimatorValue;
	bool jumpInputPaused;

	bool jumpButtonHoldActive;
	float lastTimeJumpButtonHoldActive;

	bool runInputPaused;

	Transform head;
	Vector3 landMarkRayPosition;
	Vector3 landMarkRayDirection;
	float currentMaxLandDistance;
	Vector3 landMarkForwardDirection;
	Quaternion landMarkTargetRotation;

	//Locked camera variables

	[HideInInspector] public bool useTankControls;
	bool playerCanMoveOnAimInTankMode;

	public enum lockedPlayerMovementMode
	{
		world3d,
		world2_5d
	}

	[HideInInspector] public bool checkCameraDirectionFromLockedToFree;
	bool tankModeCurrentlyEnabled;

	bool inputNotPressed;

	//Air dash ability variables

	int currentNumberOfDash;
	float lastTimeDash;

	[HideInInspector] public bool usingAbilityActive;

	float currentDeltaTime;

	float currentUpdateDeltaTime;
	float currentFixedUpdateDeltaTime;

	//Crouch variables

	float lastTimeCrouchSlidingActive;

	bool crouchingOnAir;

	bool previousValueCrouching;

	//Run variables

	float lastTimeMovingOnRun;

	float runButtonPressedTimer;
	float lastTimeRun;

	//AI variables

	Vector3 navMeshCurrentLookPos;
	Vector3 navMeshMoveInput;
	bool lookInCameraDirection;

	bool characterControlOverrideActive;

	bool AIStrafeModeActive;
	bool AIEnableInputMovementOnStrafe;

	//Fly mode variables

	bool turnAndForwardAnimatorValuesPaused;

	//Vehicle variables
	GameObject currentVehicle;

	//Controller variables
	float turnAmount;
	float forwardAmount;
	float runCycleLegOffset = 0.2f;
	float lastTimeMoved;

	bool slowFallExternallyActive;

	bool slowingFallInput;

	//External force variables
	bool externalForceActive;
	Vector3 externalForceValue;

	bool externalForceOnAirActive;
	Vector3 externalForceOnAirValue;
	float externalForceOnAirControlValue;

	float originalMoveSpeedMultiplier;
	float originalJumpPower;
	float originalAirSpeed;
	float originalAirControl;

	float originalNoAnimatorAirSpeed;

	GameObject playerManagersParentGameObject;

	bool checkCharacterMeshIfGeneratedOnStartInitialized;

	Transform currentLockedCameraTransform;

	bool inputCanBeUsed;
	bool characterRotatingToSurface;

	float raycastDistance;
	Vector3 rayPosition;
	//	Vector3 rayDirection;

	float hitPointVerticalPosition;

	float mainHitPointVerticalPosition;

	float upDistance;

	bool checkForSlopes;

	float waitTimeToSlowDown;

	bool previouslyOnGround;

	[HideInInspector] public bool checkOnGroungPaused;

	bool rotatingPlayerIn2_5dToRight;
	bool rotatingPlayerIn2_5dToRLeft;

	bool canRotateOn3dWorld;
	bool canRotateOn2_5dWorld;

	bool applyRootMotionAlwaysActive;

	[HideInInspector] public bool actionActiveWithMovement;

	[HideInInspector] public float lastTimeActionActive;

	[HideInInspector] public bool ignoreCameraDirectionOnStrafeMovement;

	[HideInInspector] public bool addExtraRotationPaused;

	[HideInInspector] public bool deactivateRootMotionOnStrafeActiveOnLockedView;

	bool previousStrafeModeActive;

	Vector3 lastForwardDirection;
	Vector3 lastRigthDirection;

	Coroutine resetInputCoroutine;

	bool playerUsingInput;

	bool movementInputActive;
	bool previousValueMovementInputActive;

	[HideInInspector] public Vector3 currentForwardDirection;
	[HideInInspector] public Vector3 currentRightDirection;

	bool originalLookAlwaysInCameraDirection;
	bool originalLookOnlyIfMoving;

	Vector3 currentPlayerPosition;
	Vector3 currentPlayerUp;

	bool stairStepDetected;

	bool setHeadbodStatesPaused;

	[HideInInspector] public bool pauseAllPlayerDownForces;

	bool toggleWalkRunState;
	float previousWalkSpeedValue = 1;

	bool originalSprintEnabledValue;
	bool originalIncreaseWalkSpeedEnabledValue;

	bool obstacleToAvoidMovementFound;

	Vector3 checkObstacleRaycastPosition;
	Vector3 checkObstacleRaycastDirection;

	bool obstacleDetectedAtRight;
	bool obstacleDetectedAtLeft;

	[HideInInspector] public bool headTrackCanBeUsed = true;

	[HideInInspector] public bool disableStrafeModeExternallyIfIncreaseWalkSpeedActive;

	bool currentStrafeModeExternallyActive;

	public externalControllerBehavior currentExternalControllerBehavior;

	public bool useExternalControllerBehaviorPaused;

	public bool externalControllBehaviorActive;

	bool externalControlBehaviorForAirTypeActive;

	bool playerActionSystemLocated;

	bool weaponsManagerLocated;

	float lastTimeFiring;

	bool aimingPowers;

	bool usingPowers;

	float quickTurnMovementDuration;

	float lastTimeQuickTurnActive;

	bool pausePlayerTransformExtraRotationActive;
	float lastTimeMovementInputPressed;
	bool checkingQuickMovementDirectionActive;
	bool activateExtraRotationInQuickTurnMovement;

	public enum footStepState
	{
		None,
		Air_Landing,
		Running,
		Run_Crouching,
		Crouching,
		Walking,
		Jumping
	}

	public enum headbobState
	{
		None,
		Jump_Start,
		Jump_End,
		Air,
		Run_Crouching,
		Running,
		Crouching,
		Walking,
		Dynamic_Idle,
		Static_Idle
	}

	bool addExtraCharacterVelocityActive;

	Vector3 currentExtraCharacterVelocity;

	bool actionCanHappenOnAir;

	bool enableChangeScriptStateAfterFinishActionState;

	bool disableChangeScriptStateAfterFinishActionState;

	bool allowDownVelocityDuringAction;

	bool groundDetected;

	Ray sphereRaycastRay;

	[HideInInspector] public bool useCaspsuleCastForGroundDetection;

	[HideInInspector] public float capsuleCastRadius;
	[HideInInspector] public float capsuleCastDistance;
	[HideInInspector] public float caspsuleCastOffset;

	//	Vector3 point1;
	//	Vector3 point2;

	Vector3 currentRayOriginPosition;
	Vector3 currentRayTargetPosition;

	float distanceToTarget;

	bool headScaleChanged;

	[HideInInspector] public bool customCharacterControllerActive;

	customCharacterControllerBase currentCustomCharacterControllerBase;

	[HideInInspector] public int regularAirboneID = -1;
	[HideInInspector] public int regularAirID = 0;

	bool regularAirboneIDActivePreviously;

	bool overrideTurnAmountActive;
	float overrideTurnAmount;

	bool overrideMainCameraTransformActive;
	Transform overrideMainCameraTransform;

	bool useForwardDirectionForCameraDirection;

	bool useRightDirectionForCameraDirection;

	bool actionToGetOnVehicleActive;
	bool actionToGetOffFromVehicleActive;

	Coroutine rotateCharacterCoroutine;

	[HideInInspector] public bool checkToKeepWeaponAfterAimingWeaponFromShooting;

	float lastTimeCheckToKeepWeapon;

	float changeAnimSpeedMultiplierDuration;

	Coroutine changeAnimSpeedMultiplierCoroutine;

	bool pauseResetAnimatorStateFOrGroundAnimator;

	GameObject currentTemporaVehicle;

	bool applyAnimatorVelocityWithoutMoving;

	bool disableCrouchState;

	float currentVerticalSpeed;

	bool currentApplyRootMotionValue;

	float currentAnimatorSpeed = -2;
	float previousAnimatorSpeed = -1;

	float currentWalkSpeedOnLockedCamera;

	bool headBobManagerLocated;

	bool originalCanDrive;
	bool originalCanCharacterGetOnVehiclesState;

	bool originalCanMoveWhileAimLockedCamera;

	bool pauseCameraShakeFromGravityActive;

	bool ignoreInputOnAirControlActive;

	bool useMaxFallSpeedExternallyActive;

	float customMaxFallSpeedExternally;

	void initilizeAnimatorIDValues ()
	{
		forwardAnimatorID = Animator.StringToHash (forwardAnimatorName);
		turnAnimatorID = Animator.StringToHash (turnAnimatorName);

		horizontalAnimatorID = Animator.StringToHash (horizontalAnimatorName);
		verticalAnimatorID = Animator.StringToHash (verticalAnimatorName);

		horizontalStrafeAnimatorID = Animator.StringToHash (horizontalStrafeAnimatorName);
		verticalStrafeAnimatorID = Animator.StringToHash (verticalStrafeAnimatorName);

		onGroundAnimatorID = Animator.StringToHash (onGroundAnimatorName);
		crouchAnimatorID = Animator.StringToHash (crouchAnimatorName);
		movingAnimatorID = Animator.StringToHash (movingAnimatorName);
		jumpAnimatorID = Animator.StringToHash (jumpAnimatorName);
		jumpLegAnimatorID = Animator.StringToHash (jumpLegAnimatorName);

		strafeModeActiveAnimatorID = Animator.StringToHash (strafeModeActiveAnimatorName);
		movementInputActiveAnimatorID = Animator.StringToHash (movementInputActiveAnimatorName);
		movementRelativeToCameraAnimatorID = Animator.StringToHash (movementRelativeToCameraAnimatorName);
		movementIDAnimatorID = Animator.StringToHash (movementIDAnimatorName);
		playerModeIDAnimatorID = Animator.StringToHash (playerModeIDAnimatorName);
		movementSpeedAnimatorID = Animator.StringToHash (movementSpeedAnimatorName);
		lastTimeInputPressedAnimatorID = Animator.StringToHash (lastTimeInputPressedAnimatorName);

		carryingWeaponAnimatorID = Animator.StringToHash (carryingWeaponAnimatorName);
		aimingModeActiveAnimatorID = Animator.StringToHash (aimingModeActiveAnimatorName);

		playerStatusIDAnimatorID = Animator.StringToHash (playerStatusIDAnimatorName);

		idleIDAnimatorID = Animator.StringToHash (idleIDAnimatorName);

		strafeIDAnimatorID = Animator.StringToHash (strafeIDAnimatorName);

		crouchIDAnimatorID = Animator.StringToHash (crouchIDAnimatorName);

		airIDAnimatorID = Animator.StringToHash (airIDAnimatorName);

		airSpeedAnimatorID = Animator.StringToHash (airSpeedAnimatorName);

		quickTurnRightDirectionID = Animator.StringToHash (quickTurnRightDirectionName);
		quickTurnLeftDirectionID = Animator.StringToHash (quickTurnLeftDirectionName);
		quickTurnDirectionIDSpeedID = Animator.StringToHash (quickTurnDirectionIDSpeedName);

		shieldActiveAnimatorID = Animator.StringToHash (shieldActiveAnimatorName);

		inputAmountID = Animator.StringToHash (inputAmountName);
	}

	void Awake ()
	{
		initilizeAnimatorIDValues ();

		originalWalkSpeedValue = walkSpeed;
		originalGravityMultiplier = gravityMultiplier;
		currentGravityMultiplier = gravityMultiplier;
		originalGravityForce = gravityForce;

		originalStationaryTurnSpeed = stationaryTurnSpeed;

		if (playerTransform == null) {
			playerTransform = transform;
		}

		if (playerTransform.parent != null) {
			playerManagersParentGameObject = playerTransform.parent.gameObject;
		}

		playerActionSystemLocated = mainPlayerActionSystem != null;

		animator.applyRootMotion = true;

		currentApplyRootMotionValue = true;

		weaponsManagerLocated = weaponsManager != null;

		headBobManagerLocated = headBobManager != null;

		originalCanDrive = canDrive;

		originalCanCharacterGetOnVehiclesState = canGetOnVehicles;

		originalCanMoveWhileAimLockedCamera = canMoveWhileAimLockedCamera;
	}

	void Start ()
	{
		//set the collider center in the correct place
		originalHeight = capsule.height;
		originalRadius = capsule.radius;
		capsule.center = Vector3Up * capsule.height * half;

		currentHeight = originalHeight;

		//get the player camera
		if (playerCameraTransform == null) {
			playerCameraTransform = playerCameraGameObject.transform;
		}

		if (animator.avatar != null && animator.avatar.isHuman) {
			head = animator.GetBoneTransform (HumanBodyBones.Head);
		}

		if (useLandMark) {
			if (landMark.activeSelf) {
				landMark.SetActive (false);
			}

			landMark.transform.SetParent (null);
		}

		setHighFrictionMaterial ();

		originalAnimationSpeed = animSpeedMultiplier;

		//get all the important parameters of player controller
		originalMoveSpeedMultiplier = moveSpeedMultiplier;
		originalJumpPower = jumpPower;
		originalAirSpeed = airSpeed;
		originalAirControl = airControl;

		originalNoAnimatorAirSpeed = noAnimatorAirSpeed;

		//no animator original movement values assignment
		originalNoAnimWalkMovementSpeed = noAnimatorWalkMovementSpeed;
		originalNoAnimRunMovementSpeed = noAnimatorRunMovementSpeed;
		originalNoAnimCrouchMovementSpeed = noAnimatorCrouchMovementSpeed;
		originalNoAnimStrafeMovementSpeed = noAnimatorStrafeMovementSpeed;
		originalNoAnimWalkBackwardMovementSpeed = noAnimatorWalkBackwardMovementSpeed;
		originalNoAnimRunBackwardMovementSpeed = noAnimatorRunBackwardMovementSpeed;
		originalNoAnimCrouchBackwardMovementSpeed = noAnimatorCrouchBackwardMovementSpeed;
		originalNoAnimStrafeBackwardMovementSpeed = noAnimatorStrafeBackwardMovementSpeed;
		originalNoAnimCanRun = noAnimatorCanRun;

		originalSetUseRootMotionActiveState = useRootMotionActive;

		originalNoRootWalkMovementSpeed = noRootWalkMovementSpeed;
		originalNoRootRunMovementSpeed = noRootRunMovementSpeed;
		originalNoRootSprintMovementSpeed = noRootSprintMovementSpeed;
		originalNoRootCrouchMovementSpeed = noRootCrouchMovementSpeed;
		originalNoRootWalkStrafeMovementSpeed = noRootWalkStrafeMovementSpeed;
		originalNoRootRunStrafeMovementSpeed = noRootRunStrafeMovementSpeed;

	
		originalLookAlwaysInCameraDirection = lookAlwaysInCameraDirection;

		originalLookOnlyIfMoving = lookOnlyIfMoving;

		originalSprintEnabledValue = sprintEnabled;

		originalIncreaseWalkSpeedEnabledValue = increaseWalkSpeedEnabled;

		originalNoAnimatorMovementSpeedMultiplier = noAnimatorMovementSpeedMultiplier;

		previousValueOnGroundAnimatorState = !checkIfPlayerOnGroundWithRaycast ();
	}

	void Update ()
	{
		currentUpdateDeltaTime = getCurrentDeltaTime ();

		inputCanBeUsed = canUseInput ();

		playerIsAiming = isPlayerAiming ();

		lookInCameraDirectionOnFreeFireActive = hasToLookInCameraDirectionOnFreeFire ();

		playerUsingInput = isPlayerUsingInput ();

		if (useEventsOnWalk || useEventsOnRun) {
			if (playeCurrentlyUsingInput != playerUsingInput) {
				playeCurrentlyUsingInput = playerUsingInput;

				if (playeCurrentlyUsingInput) {
					if (walkSpeed > 0.5f) {
						if (useEventsOnWalk) {
							eventOnWalkEnd.Invoke ();
						}

						if (useEventsOnRun) {
							eventOnRunStart.Invoke ();
						}
					} else {
						if (useEventsOnRun) {
							eventOnRunEnd.Invoke ();
						}

						if (useEventsOnWalk) {
							eventOnWalkStart.Invoke ();
						}
					}
				} else {
					if (useEventsOnWalk) {
						eventOnWalkEnd.Invoke ();
					}

					if (useEventsOnRun) {
						eventOnRunEnd.Invoke ();
					}
				}
			}
		}

		if (inputCanBeUsed && !playerOnGround && !gravityPowerActive && holdJumpSlowDownFallEnabled) {

			waitTimeToSlowDown = lastJumpTime + 0.2f;

			if (enabledDoubleJump && doubleJump) {
				waitTimeToSlowDown += lastDoubleJumpTime + 1;
			}
		}

		if (lockedCameraActive) {
			if (isPlayerMovingOn2_5dWorld ()) {
				if (playerCameraManager.isMoveInXAxisOn2_5d ()) {
					playerTransform.position = new Vector3 (playerTransform.position.x, playerTransform.position.y, playerCameraManager.getOriginalLockedCameraPivotPosition ().z);
				} else {
					playerTransform.position = new Vector3 (playerCameraManager.getOriginalLockedCameraPivotPosition ().x, playerTransform.position.y, playerTransform.position.z);
				}
			}
		}

		if (gravityForcePaused) {
			if (unPauseGravityForceActive && Time.time > lastTimeGravityForcePaused + gravityForcePausedTime) {
				setGravityForcePuase (false);

				unPauseGravityForceActive = false;
			}
		}

		lookInCameraDirectionActive = hasToLookInCameraDirection () &&
		((!crouching || lookInCameraDirectionOnCrouchState) || (!weaponsManagerLocated || !weaponsManager.isUsingWeapons ()));

		strafeModeActive = ((playerIsAiming && lookInCameraDirectionOnFreeFireActive) || lookInCameraDirectionActive) && playerOnGround;

		if (previousStrafeModeActive != strafeModeActive) {
			previousStrafeModeActive = strafeModeActive;

			if (deactivateRootMotionOnStrafeActiveOnLockedView) {
				setUseRootMotionActiveState (!strafeModeActive);
			}
		}

		if (originalLookAlwaysInCameraDirection && disableLookAlwaysInCameraDirectionIfIncreaseWalkSpeedActive) {
			if (sprintEnabled) {
				if (running) {
					if (originalLookAlwaysInCameraDirection == lookAlwaysInCameraDirection) {
						setLookAlwaysInCameraDirectionState (false);
					} 
				} else {
					if (originalLookAlwaysInCameraDirection != lookAlwaysInCameraDirection) {
						setLookAlwaysInCameraDirectionState (true);
					}
				}
			} else {
				if (walkSpeed == 1) {
					if (originalLookAlwaysInCameraDirection == lookAlwaysInCameraDirection) {
						setLookAlwaysInCameraDirectionState (false);
					} 
				} else {
					if (originalLookAlwaysInCameraDirection != lookAlwaysInCameraDirection) {
						setLookAlwaysInCameraDirectionState (true);
					}
				}
			}
		} 

		if (disableStrafeModeExternallyIfIncreaseWalkSpeedActive) {
			if (sprintEnabled) {
				if (running) {
					if (!currentStrafeModeExternallyActive) {
						setLookAlwaysInCameraDirectionState (false);

						currentStrafeModeExternallyActive = true;
					} 
				} else {
					if (currentStrafeModeExternallyActive) {
						setLookAlwaysInCameraDirectionState (true);

						currentStrafeModeExternallyActive = false;
					}
				}
			} else {
				if (walkSpeed == 1) {
					if (!currentStrafeModeExternallyActive) {
						setLookAlwaysInCameraDirectionState (false);

						currentStrafeModeExternallyActive = true;
					} 
				} else {
					if (currentStrafeModeExternallyActive) {
						setLookAlwaysInCameraDirectionState (true);

						currentStrafeModeExternallyActive = false;
					}
				}
			}
		}

		if (!usedByAI) {
			if (running) {
				//stop the running action if the player is not moving
				if (getMoveInputDirection ().magnitude == 0) {

					if (Time.time > lastTimeMovingOnRun + 0.26f) {
						stopRun ();
					}
				}

				if (isPlayerMoving (0.05f) || isPlayerUsingInput ()) {
					lastTimeMovingOnRun = Time.time;
				} else {
					lastTimeMovingOnRun = 0;
				}
			}
		}
	}

	void FixedUpdate ()
	{
		currentFixedUpdateDeltaTime = getCurrentDeltaTime ();

		playerTransformUp = playerTransform.up;

		playerTransformForward = playerTransform.forward;

		playerTransformRight = playerTransform.right;

		if (jumpButtonHoldActive) {
			if (Time.time > lastTimeJumpButtonHoldActive + addJumpForceDuration) {
				jumpButtonHoldActive = false;
			} else {
				if (Time.time > lastTimeJumpButtonHoldActive + minWaitToAddJumpForceAfterButtonPressed) {
					Vector3 totalExtraJump = jumpForceAmontWhileButtonPressed * mainRigidbody.mass * currentNormal;

					mainRigidbody.AddForce (totalExtraJump);
				}
			}
		}
	
		//convert the input from keyboard or a touch screen into values to move the player, given the camera direction
		if (canMove && !usedByAI && !playerNavMeshEnabled && !moveIputPaused) {
			setMainAxisValues ();

			horizontalInput = axisValues.x;
			verticalInput = axisValues.y;
		} else {
			axisValues = Vector2.zero;
		}

		if (canMove && !moveIputPaused) {
			setMainRawAxisValues ();
		} else {
			rawAxisValues = Vector2.zero;
		}

		if (!usedByAI && !playerNavMeshEnabled) {
			//get the axis of the player camera, to move him properly
			if (lockedCameraActive) {

				currentLockedCameraTransform = playerCameraManager.getLockedCameraTransform ();

				if (isPlayerMovingOn3dWorld ()) {

					//player can move while aiming
					if (!canMoveWhileAimLockedCamera && playerIsAiming) {
						verticalInput = 0;
						horizontalInput = 0;
					}

					//if the player is on tank mode, use his forward and right direction as the input
					if (useTankControls) {
						currentForwardDirection = playerTransformForward;
						currentRightDirection = playerTransformRight;
					} else {
						//else, the player will follow the direction of the current locked camera 

						//if the player is looking at a target, the input direction used will be the player camera
						if (playerCameraManager.isPlayerLookingAtTarget ()) {
							if (!isPlayerOnGround ()) {
								currentForwardDirection = currentLockedCameraTransform.forward;
								currentRightDirection = currentLockedCameraTransform.right;
							} else {
								currentForwardDirection = playerCameraTransform.forward;
								currentRightDirection = mainCameraTransform.right;
							}
						} else {
							//else, he will use the locked camera direction
							currentForwardDirection = currentLockedCameraTransform.forward;
							currentRightDirection = currentLockedCameraTransform.right;
						}
					}
				} else {
					//else, the player is moving in 2.5d camera, so use only the horizontal input

					currentForwardDirection = vector3Zero;
					currentRightDirection = currentLockedCameraTransform.right;
				}
			} else {
				//the player is on free camera mode

				//check if the player can move while aiming on first or third person according to settings
				if (playerIsAiming && !usingFreeFireMode) {
					if ((firstPersonActive && !canMoveWhileAimFirstPerson) || (!firstPersonActive && !canMoveWhileAimThirdPerson)) {
						verticalInput = 0;
						horizontalInput = 0;
					}
				}

				//in other case, the player uses the player camera direction as input
				if (checkCameraDirectionFromLockedToFree) {
					currentLockedCameraTransform = playerCameraManager.getLockedCameraTransform ();
					//keep the latest locked camera direction until the player stops to move

					currentForwardDirection = currentLockedCameraTransform.forward;
					currentRightDirection = currentLockedCameraTransform.right;

					if (!isPlayerMoving (0.6f) && !playerUsingInput) {
						inputNotPressed = true;
					}

					if (inputNotPressed && (playerUsingInput || !isPlayerMoving (0))) {
						checkCameraDirectionFromLockedToFree = false;

						inputNotPressed = false;
					}
				} else {
					currentForwardDirection = playerCameraTransform.forward;
					currentRightDirection = mainCameraTransform.right;

					if (useForwardDirectionForCameraDirection) {
						currentForwardDirection = playerTransform.forward;
					}

					if (useRightDirectionForCameraDirection) {
						currentRightDirection = playerTransform.right;
					}

					if (ignoreCameraDirectionOnMovement) {
						currentForwardDirection = lastForwardDirection;
						currentRightDirection = lastRigthDirection;
					}
				}
			}

			//the camera direccion and input is override by some external function, to make the playe to move toward some direction or position
			if (overrideMainCameraTransformActive) {
				currentForwardDirection = overrideMainCameraTransform.forward;
				currentRightDirection = overrideMainCameraTransform.right;
			}

			moveInput = (verticalInput * currentForwardDirection + horizontalInput * currentRightDirection) * walkSpeed;	

		} else {
			if (playerNavMeshEnabled) {
				currentLockedCameraTransform = playerCameraManager.getLockedCameraTransform ();
			}
				
			if (moveIputPaused ||
			    (actionActive && !useRootMotionActive)) {
				navMeshMoveInput = vector3Zero;
			}

			updateOverrideInputValues (new Vector2 (navMeshMoveInput.x, navMeshMoveInput.z), true);

			setAIMainAxisValues ();

			setMainAIRawAxisValues ();

			if (playerIsAiming || AIStrafeModeActive) {
				if (AIEnableInputMovementOnStrafe) {
					moveInput = playerTransform.InverseTransformDirection (navMeshMoveInput);

					horizontalInput = moveInput.x;
					verticalInput = moveInput.z;
				} else {
					moveInput = navMeshMoveInput;

					horizontalInput = 0;
					verticalInput = moveInput.magnitude;
				}
			} else {
				moveInput = navMeshMoveInput;

				horizontalInput = moveInput.x;
				verticalInput = moveInput.z;
			}

			if (!canMoveAI) {
				moveInput = vector3Zero;

				horizontalInput = 0;
				verticalInput = 0;
			}
		}

		currentMoveInput = moveInput;

		if (useObstacleDetectionToAvoidMovement && playerOnGround) {
			checkObstacleToAvoidMovement ();
		}

		//		if (useWalkToRunAccelerationChange) {
		//			currentWalkSpeedTarget = 1;
		//
		//			if (obstacleToAvoidMovementFound || !isPlayerUsingInput ()) {
		//				currentWalkSpeedTarget = originalWalkSpeedValue;
		//			} 
		//
		//			walkSpeed = Mathf.Lerp (walkSpeed, currentWalkSpeedTarget, Time.deltaTime * walkToRunAccelerationSpeed);
		//		}

		//isMoving is true if the player is moving, else is false
		isMoving = Mathf.Abs (horizontalInput) > 0.1f || Mathf.Abs (verticalInput) > 0.1f;

		if (currentMoveInput.magnitude > 1) {
			currentMoveInput.Normalize ();
		}

		//get the velocity of the rigidbody
		if (!gravityPowerActive) {
			if ((usingAnimator && !useFirstPersonPhysicsInThirdPersonActive) ||
			    (!usingAnimator && !playerOnGround) ||
			    (usingAnimator && useFirstPersonPhysicsInThirdPersonActive && !playerOnGround)) {

				currentVelocity = mainRigidbody.velocity;
			}
		}

		//convert the global movement in local
		getMoveInput ();

		//look in camera direction when the player is aiming
		lookCameraDirection ();

		//add an extra rotation to the player to get a better control of him
		addExtraRotation ();

		//if the animator is used, then
		if (usingAnimator && !useFirstPersonPhysicsInThirdPersonActive) {

			//check when the player is on ground
			checkOnGround (); 

			//update mecanim
			updateAnimator ();
		} else {
			//else, apply force to the player's rigidbody
			if (playerOnGround) {
				if (!isMoving) {
					currentMoveInput = vector3Zero;
				}

				if (running) {
					if (noAnimatorCanRun) {
						if (verticalInput < 0) {
							if (noAnimatorCanRunBackwards) {
								if (crouching) {
									noAnimatorCurrentMovementSpeed = noAnimatorRunCrouchBackwardMovementSpeed;
								} else {
									noAnimatorCurrentMovementSpeed = noAnimatorRunBackwardMovementSpeed;
								}
							} else {
								stopRun ();
							}
						} else {
							if (crouching) {
								if (crouchSlidingActive) {
									noAnimatorCurrentMovementSpeed = noAnimatorCrouchSlidingSpeed;

									if (keepCrouchSlidingOnInclinatedSurface) {
										if (hitAngle > minInclinitaonSurfaceAngleToCrouchcSliding) {
											if (Physics.Raycast (playerTransform.position + (playerTransformForward / 2), -playerTransformUp, out hit, 1, layer)) {
												if (Mathf.Abs (playerTransform.InverseTransformPoint (hit.point).y) > 0.1f) {

													lastTimeCrouchSlidingActive = Time.time;
												}
											}
										}
									}

									if (Time.time > lastTimeCrouchSlidingActive + noAnimatorCrouchSlidingLerpDelay) {
										currentNoAnimatorSlidingCrouchSpeed = 
											Mathf.Lerp (currentNoAnimatorSlidingCrouchSpeed, noAnimatorRunCrouchMovementSpeed, Time.fixedDeltaTime * noAnimatorCrouchSlidingLerpSpeed);

										noAnimatorCurrentMovementSpeed = currentNoAnimatorSlidingCrouchSpeed;
									} else {
										currentNoAnimatorSlidingCrouchSpeed = noAnimatorCrouchSlidingSpeed;
									}

									if (Time.time > lastTimeCrouchSlidingActive + noAnimatorCrouchSlidingDuration) {
										stopCrouchSliding ();

										if (getUpAfterCrouchSlidingEnd && crouching) {
											crouch ();
										}
									}
								} else {
									noAnimatorCurrentMovementSpeed = noAnimatorRunCrouchMovementSpeed;
								}
							} else {
								noAnimatorCurrentMovementSpeed = noAnimatorRunMovementSpeed;
							}
						}
					} else {
						stopRun ();
					}
				} else if (crouching) {
					if (verticalInput >= 0) {
						noAnimatorCurrentMovementSpeed = noAnimatorCrouchMovementSpeed;
					} else {
						noAnimatorCurrentMovementSpeed = noAnimatorCrouchBackwardMovementSpeed;
					}
				} else if (Mathf.Abs (verticalInput) > half && Mathf.Abs (horizontalInput) > half) {
					if (verticalInput >= 0) {
						noAnimatorCurrentMovementSpeed = noAnimatorStrafeMovementSpeed;
					} else {
						noAnimatorCurrentMovementSpeed = noAnimatorStrafeBackwardMovementSpeed;
					}
				} else {
					if (verticalInput >= 0) {
						noAnimatorCurrentMovementSpeed = noAnimatorWalkMovementSpeed;
					} else {
						noAnimatorCurrentMovementSpeed = noAnimatorWalkBackwardMovementSpeed;
					}
				}

				noAnimatorCurrentForce = (noAnimatorSpeed * noAnimatorCurrentMovementSpeed) * currentMoveInput;

				// Subtract the local Y axis velocity of the rigidbody
				noAnimatorCurrentForce = noAnimatorCurrentForce - playerTransformUp * playerTransform.InverseTransformDirection (noAnimatorCurrentForce).y;

				noAnimatorCurrentForce *= noAnimatorMovementSpeedMultiplier;

				velocityChange = noAnimatorCurrentForce - mainRigidbody.velocity;

				velocityChange = Vector3.ClampMagnitude (velocityChange, maxVelocityChange);

				mainRigidbody.AddForce (velocityChange, ForceMode.VelocityChange);
			} else {
				if (!wallRunningActive &&
				    !gravityPowerActive &&
				    !pauseAllPlayerDownForces &&
				    !externalForceOnAirActive &&
				    !externalControlBehaviorForAirTypeActive &&
				    !swimModeActive) {

					if (crouchSlidingOnAirActive) {
						noAnimatorCurrentMovementSpeed = noAnimatorCrouchSlidingOnAirSpeed;

						if (useNoAnimatorCrouchSlidingDurationOnAir && Time.time > lastTimeCrouchSlidingActive + noAnimatorCrouchSlidingDurationOnAir) {
							stopCrouchSliding ();
						}
					} else {
						noAnimatorCurrentMovementSpeed = noAnimatorAirSpeed;
					}

					noAnimatorCurrentForce = currentMoveInput * noAnimatorCurrentMovementSpeed + playerTransform.InverseTransformDirection (currentVelocity).y * playerTransformUp;

					noAnimatorCurrentForce *= noAnimatorMovementSpeedMultiplier;

					velocityChange = noAnimatorCurrentForce - mainRigidbody.velocity;

					velocityChange = Vector3.ClampMagnitude (velocityChange, maxVelocityChange);

					mainRigidbody.AddForce (velocityChange, ForceMode.VelocityChange);
				}

				if (externalForceOnAirActive) {
					//					airMove = externalForceOnAirValue;
					//					currentVelocity = Vector3.Lerp (currentVelocity, airMove, currentFixedUpdateDeltaTime * externalForceOnAirControlValue);

					velocityChange = externalForceOnAirValue - mainRigidbody.velocity;

					velocityChange = Vector3.ClampMagnitude (velocityChange, maxVelocityChange);

					mainRigidbody.AddForce (velocityChange, ForceMode.VelocityChange);

					externalForceOnAirActive = false;
				} 
			}

			if (!gravityPowerActive) {
				currentVelocity = mainRigidbody.velocity;
			}

			currentVelocityChangeMagnitude = mainRigidbody.velocity.magnitude;

			checkOnGround (); 
		}

		//check if the player is on ground or in air
		//also set the friction of the character if he is on the ground or in the air
		if (playerOnGround) {

			onGroundVelocity ();

			if (!onGroundChecked) {
				gravityManager.onGroundOrOnAir (true);

				onGroundChecked = true;

				if (updateHeadbobState && headBobManagerLocated) {
					//send a message to the headbob in the camera, when the player lands from a jump
					if (currentHeadbobState != headbobState.Jump_End) {
						headBobManager.setState ("Jump End");
					
						currentHeadbobState = headbobState.Jump_End;
					}
				}

				if (updateFootStepStateActive) {
					if (currentFootStepState != footStepState.Air_Landing) {
						stepManager.setFootStepState ("Air Landing");

						currentFootStepState = footStepState.Air_Landing;
					}
				}

				//set the number of jumps made by the player since this moment
				jumpsAmount = 0;

				if (weaponsManagerLocated) {
					weaponsManager.setWeaponsJumpEndPositionState (true);
				}

				if (!isPlayerDead () && canRagdollMove) {
					//check the last time since the player is in the air, falling in its gravity direction
					//if the player has been in the air more time than maxTimeInAirBeforeGetDamage and his velocity is higher than 15, then apply damage
					if (fallDamageEnabled && !gravityPowerActive && !fallDamageCheckPaused) {
						currentFallSpeed = Mathf.Abs (mainRigidbody.velocity.magnitude);

						float lastFrameAirVelocityABS = Mathf.Abs (lastFrameAirVelocity);

						//						print ("current fall speed " + currentFallSpeed);

						checkFallState = false;

						if (firstPersonActive) {
							if (currentFallSpeed > minPlayerVelocityToApplyDamageFirstPerson ||
							    lastFrameAirVelocityABS > minPlayerVelocityToApplyDamageFirstPerson) {
								checkFallState = true;
							}
						} else {
							if (currentFallSpeed > minPlayerVelocityToApplyDamageThirdPerson ||
							    lastFrameAirVelocityABS > minPlayerVelocityToApplyDamageThirdPerson) {
								checkFallState = true;
							}
						}

//						print (checkFallStatePaused + " " + applyFallDamageEnabled + " " + fallDamageCheckOnHealthPaused + " " +
//						currentFallSpeed + " " + checkFallState + " " + lastFrameAirVelocityABS);

						if (checkFallStatePaused) {
							checkFallState = false;
						}

						if (checkFallState) {
							currentLastTimeFalling = lastTimeFalling;

							if (applyFallDamageEnabled && !fallDamageCheckOnHealthPaused) {
								if (Time.time > currentLastTimeFalling + maxTimeInAirBeforeGettingDamage) {
									//get the last time since the player is in the air and his velocity, and call the health damage function
									float totalTimeOnAir = Mathf.Abs (Time.time - currentLastTimeFalling);

									//									print ("total time on air " + totalTimeOnAir);

									float damageValue = totalTimeOnAir * currentFallSpeed;

									if (fallingDamageMultiplier != 1) {
										damageValue *= fallingDamageMultiplier;
									}

									float remainingHealthAmount = healthManager.getCurrentHealthAmount ();

									if (ignoreShieldOnFallDamage) {
										damageValue = Mathf.Clamp (damageValue, 0, remainingHealthAmount);
									} else {
										if (healthManager.isUseShieldActive ()) {
											float remainShield = healthManager.getCurrentShieldAmount ();

											damageValue = Mathf.Clamp (damageValue, 0, remainingHealthAmount + remainShield);

											//											print (remainingHealthAmount + " " + remainShield);
										} else {
											damageValue = Mathf.Clamp (damageValue, 0, remainingHealthAmount);
										}
									}

									//									print ("damage to apply on fall " + damageValue);

									if (damageValue > 0) {
										healthManager.setDamageWithHealthManagement (damageValue, -mainRigidbody.velocity.normalized, playerTransform.position + playerTransformUp, 
											gameObject, gameObject, false, false, ignoreShieldOnFallDamage, false, false, false, -2, damageTypeIDOnFallDamage);
									}
								}
							}

//							print ((Time.time > currentLastTimeFalling + minTimeOnAirToUseEvent) + " " + currentLastTimeFalling + " " + minTimeOnAirToUseEvent
//							+ " " + (Time.time - currentLastTimeFalling + minTimeOnAirToUseEvent));

							//call another function when the player receives damage from a long fall
							if (callEventOnFallDamage && (!isPlayerDead () || !callEventOnlyIfPlayerAlive)) {
								if (Time.time > currentLastTimeFalling + minTimeOnAirToUseEvent) {
									eventOnFallDamage.Invoke ();
								}
							}
						}
					}
				}

				if (freeFloatingModeOn || zeroGravityModeOn) {
					setFootStepManagerState (false);
				}

				lastTimeGround = Time.time;

				if (wallRunningActive) {
					setWallRunningActiveState (false);
				}

				if (crouchingOnAir && !crouching) {
					crouching = true;

					crouchingOnAir = false;

					crouchSlidingOnAirActive = false;
				} else {
					if (disableCrouchState && !firstPersonActive) {
						crouching = false;

						disableCrouchState = false;
					}
				}

				if (disableRootMotionTemporalyOnLandThirdPerson) {
					if (!firstPersonActive) {
						if (!checkingDisableRootMotionTemporaly) {
							checkingDisableRootMotionTemporaly = true;
						}
					}
				}

				jumpButtonHoldActive = false;

				if (customCharacterControllerActive) {
					currentCustomCharacterControllerBase.updateOnGroundValue (true);
				}
					
				if (stopCrouchFromAutoJumpActive) {
					crouching = false;

					stopCrouchFromAutoJumpActive = false;
				}
			}

			if (checkingDisableRootMotionTemporaly) {
				if (firstPersonActive) {
					if (rootMotionDisabledTemporaly) {
						checkingDisableRootMotionTemporaly = false;
						rootMotionDisabledTemporaly = false;

						setOriginalUseRootMotionActiveState ();
					}
				} else {
					if (!rootMotionDisabledTemporaly) {
						if (Time.time > lastTimeGround + waitToDisableRootMotionTemporalyOnLandThirdPerson) {
							setUseRootMotionActiveState (false);

							lastTimeRootMotionDisabledTemporaly = Time.time;

							rootMotionDisabledTemporaly = true;
						}
					} else {
						if (Time.time > lastTimeRootMotionDisabledTemporaly + durationDisableRootMotionTemporalyOnLandThirdPerson) {
							checkingDisableRootMotionTemporaly = false;
							rootMotionDisabledTemporaly = false;

							setOriginalUseRootMotionActiveState ();
						}
					}
				}
			}

			if (updateFootStepStateActive) {
				if (playerOnGround) {
					setCurrentFootStepsState ();
				}
			}

			//change the collider material when the player moves and when the player is not moving
			if (!physicMaterialAssigmentPaused) {
				if (currentMoveInput.magnitude == 0) {
					if (!highFrictionMaterialActive) {
						setHighFrictionMaterial ();
					}
				} else {
					if (highFrictionMaterialActive) {
						setZeroFrictionMaterial ();
					}
				}
			}

			//check the headbob state
			if (updateHeadbobState && headBobManagerLocated) {
				if (headBobManager.headBobEnabled && !setHeadbodStatesPaused) {
					setCurrentHeadBobState ();
				}
			}
		}

		//the player is in the air, so
		else {
			//call the air velocity function
			onAirVelocity ();

			if (onGroundChecked) {
				//set in other script this state
				gravityManager.onGroundOrOnAir (false);

				onGroundChecked = false;

				setLastTimeFalling ();

				if (weaponsManagerLocated) {
					weaponsManager.setWeaponsJumpStartPositionState (true);
				}

				currentJumpLeg = 0;

				if (zeroGravityModeOn || freeFloatingModeOn) {
					setCheckOnGrundStatePausedFFOrZGState (true);

					setFootStepManagerState (true);
				}

				if (customCharacterControllerActive) {
					currentCustomCharacterControllerBase.updateOnGroundValue (false);
				}
			}

			if (highFrictionMaterialActive) {
				setZeroFrictionMaterial ();
			}

			if (updateHeadbobState && headBobManagerLocated) {
				if (!wallRunningActive && !externalControlBehaviorForAirTypeActive) {
					if (currentHeadbobState != headbobState.Air) {
						
						headBobManager.setState ("Air");


						currentHeadbobState = headbobState.Air;
					}
			
					if (headBobManager.useDynamicIdle) {
						setLastTimeMoved ();
					}
				}
			}
		}

		if (externalControllBehaviorActive) {
			currentExternalControllerBehavior.updateControllerBehavior ();
		}

		if (customCharacterControllerActive) {
			currentCustomCharacterControllerBase.updateCharacterControllerState ();
		}

		//in case the player is using the gravity power, the update of the rigidbody velocity stops
		if (!gravityPowerActive) {
			if ((usingAnimator && !useFirstPersonPhysicsInThirdPersonActive) ||
			    (!usingAnimator && !playerOnGround) ||
			    (usingAnimator && useFirstPersonPhysicsInThirdPersonActive && !playerOnGround)) {

				if ((useMaxFallSpeed || useMaxFallSpeedExternallyActive) && !playerOnGround) {

					float verticalSpeed = playerTransform.InverseTransformDirection (currentVelocity).y;

					if (verticalSpeed < 0) {

						Vector3 horizontalSpeed = currentVelocity - verticalSpeed * playerTransformUp;

						float currentMaxFallSpeed = -maxFallSpeed;

						if (customMaxFallSpeedExternally != 0) {
							currentMaxFallSpeed = -customMaxFallSpeedExternally;
						}

						float verticalSpeedClamped = Mathf.Clamp (verticalSpeed, currentMaxFallSpeed, 100);

						horizontalSpeed += playerTransformUp * verticalSpeedClamped;

						currentVelocity = horizontalSpeed;
					}
				}

				mainRigidbody.velocity = currentVelocity;
			}

			if (externalForceActive) {
				mainRigidbody.velocity += externalForceValue;

				externalForceActive = false;
			}
		}

		if (useLandMark && !externalControllBehaviorActive) {
			updateLandMark ();
		}
	}

	public void setUseMaxFallSpeedExternallyActiveState (bool state)
	{
		useMaxFallSpeedExternallyActive = state;
	}

	public void setCustomMaxFallSpeedExternally (float newValue)
	{
		customMaxFallSpeedExternally = newValue;
	}

	public virtual void setMainAxisValues ()
	{
		axisValues = playerInput.getPlayerMovementAxis ();
	}

	public virtual void setMainRawAxisValues ()
	{
		rawAxisValues = playerInput.getPlayerRawMovementAxis ();
	}

	public virtual void setCustomAxisValues (Vector2 newValue)
	{
		
	}

	public virtual void setAIMainAxisValues ()
	{
		axisValues = playerInput.getPlayerMovementAxisWithoutCheckingEnabled ();
	}

	public virtual void setMainAIRawAxisValues ()
	{
		rawAxisValues = playerInput.getPlayerRawMovementAxisWithoutCheckingEnabled ();
	}

	public virtual void updateOverrideInputValues (Vector2 inputValues, bool state)
	{
		playerInput.overrideInputValues (inputValues, state);
	}

	public virtual void setNewMainCameraTransform (Transform newTransform)
	{
		mainCameraTransform = newTransform;
	}

	public virtual void setNewPlayerCameraTransform (Transform newTransform)
	{
		playerCameraTransform = newTransform;
	}

	public void setDisableStrafeModeExternallyIfIncreaseWalkSpeedActiveState (bool state)
	{
		disableStrafeModeExternallyIfIncreaseWalkSpeedActive = state;

		currentStrafeModeExternallyActive = false;
	}

	public void checkObstacleToAvoidMovement ()
	{
		checkObstacleRaycastPosition = currentPlayerPosition + (playerTransformUp * ((currentHeight / 2) + obstacleDetectionRaycastHeightOffset));

		if (lockedCameraActive && useRelativeMovementToLockedCamera) {
			checkObstacleRaycastDirection = lockedCameraMoveInput;
		} else {
			checkObstacleRaycastDirection = moveInput;
		}

		checkObstacleRaycastDirection.Normalize ();

		obstacleDetectedAtRight = false;
		obstacleDetectedAtLeft = false;

		checkObstacleRaycastPosition += playerTransformRight * obstacleDetectionRaycastDistanceRightAndLeft;

		if (Physics.Raycast (checkObstacleRaycastPosition, checkObstacleRaycastDirection, out hit, obstacleDetectionToAvoidMovementRaycastDistance, obstacleDetectionToAvoidMovementLayermask)) {
			obstacleDetectedAtRight = true;

			if (showGizmo) {
				Debug.DrawRay (checkObstacleRaycastPosition, checkObstacleRaycastDirection * obstacleDetectionToAvoidMovementRaycastDistance, Color.red);
			}
		} else {
			if (showGizmo) {
				Debug.DrawRay (checkObstacleRaycastPosition, checkObstacleRaycastDirection * obstacleDetectionToAvoidMovementRaycastDistance, Color.green);
			}
		}

		checkObstacleRaycastPosition -= (obstacleDetectionRaycastDistanceRightAndLeft * 2) * playerTransformRight;

		if (Physics.Raycast (checkObstacleRaycastPosition, checkObstacleRaycastDirection, out hit, obstacleDetectionToAvoidMovementRaycastDistance, obstacleDetectionToAvoidMovementLayermask)) {
			obstacleDetectedAtLeft = true;

			if (showGizmo) {
				Debug.DrawRay (checkObstacleRaycastPosition, checkObstacleRaycastDirection * obstacleDetectionToAvoidMovementRaycastDistance, Color.red);
			}
		} else {
			if (showGizmo) {
				Debug.DrawRay (checkObstacleRaycastPosition, checkObstacleRaycastDirection * obstacleDetectionToAvoidMovementRaycastDistance, Color.green);
			}
		}

		if (obstacleDetectedAtRight || obstacleDetectedAtLeft) {

			obstacleToAvoidMovementFound = true;
		} else {

			obstacleToAvoidMovementFound = false;
		}

		if (obstacleToAvoidMovementFound) {
			currentMoveInput = vector3Zero;
		}
	}

	public bool isObstacleToAvoidMovementFound ()
	{
		return obstacleToAvoidMovementFound;
	}

	public Vector3 getCurrentForwardDirection ()
	{
		return currentForwardDirection;
	}

	public Vector3 getCurrentRightDirection ()
	{
		return currentRightDirection;
	}

	//convert the global movement into local movement
	void getMoveInput ()
	{
		Vector3 localMove = playerTransform.InverseTransformDirection (currentMoveInput);

		//get the amount of rotation added to the character mecanim
		if (currentMoveInput.magnitude > 0) {
			turnAmount = Mathf.Atan2 (localMove.x, localMove.z);
		} else {
			turnAmount = Mathf.Atan2 (0, 0);
		}

		//adjust player orientation to lef or right according to the input direction pressed just once
		if (isPlayerMovingOn2_5dWorld ()) {
			if (horizontalInput > 0) {
				rotatingPlayerIn2_5dToRight = true;
				rotatingPlayerIn2_5dToRLeft = false;
			}

			if (horizontalInput < 0) {
				rotatingPlayerIn2_5dToRLeft = true;
				rotatingPlayerIn2_5dToRight = false;
			}

			if (Mathf.Abs (horizontalInput) == 1) {
				rotatingPlayerIn2_5dToRLeft = false;
				rotatingPlayerIn2_5dToRight = false;
			}

			if (rotatingPlayerIn2_5dToRight) {
				float angle = Vector3.SignedAngle (playerTransformForward, currentLockedCameraTransform.right, playerTransformUp);

				if (Mathf.Abs (angle) > 5) { 
					turnAmount = angle * Mathf.Deg2Rad;
				} else {
					turnAmount = 0;
					rotatingPlayerIn2_5dToRight = false;
				}
			}

			if (rotatingPlayerIn2_5dToRLeft) {
				float angle = Vector3.SignedAngle (playerTransformForward, -currentLockedCameraTransform.right, playerTransformUp);

				if (Mathf.Abs (angle) > 5) {
					turnAmount = angle * Mathf.Deg2Rad;
				} else {
					turnAmount = 0;
					rotatingPlayerIn2_5dToRLeft = false;
				}
			}

			if (playerIsAiming) {
				if (horizontalInput != 0) {
					turnAmount = 0;

					rotatingPlayerIn2_5dToRLeft = false;
					rotatingPlayerIn2_5dToRight = false;
				}
			}

			//crouch using vertical input
			if (crouchVerticalInput2_5dEnabled) {
				if ((!crouching && verticalInput < 0) || (crouching && verticalInput > 0)) {
					crouch ();
				}
			}

			//move vertically on free floating mode or zero gravity mode
			if (freeFloatingModeOn || zeroGravityModeOn) {
				if (rawAxisValues.y != 0) {
					movingVertically = true;
				} else {
					movingVertically = false;
				}
			}

			if (!canMove || moveIputPaused) {
				turnAmount = 0;

				rotatingPlayerIn2_5dToRLeft = false;
				rotatingPlayerIn2_5dToRight = false;
			}
		}

		//get the amount of movement in forward direction

		if (useMoveInpuptMagnitedeToForwardAmount) {
			forwardAmount = currentMoveInput.magnitude;
		} else {
			forwardAmount = localMove.z;
		}

		forwardAmount = Mathf.Clamp (forwardAmount, -walkSpeed, walkSpeed);

		if (checkQuickMovementTurn180DegreesOnRun) {
			updatecheckQuickMovementTurn180DegreesOnRun (localMove);
		}
	}

	void updatecheckQuickMovementTurn180DegreesOnRun (Vector3 localMove)
	{
		if (playerIsAiming ||
		    firstPersonActive ||
		    !playerOnGround ||
		    actionActive ||
		    Time.time < lastTimeActionActive + 1) {

			if (checkingQuickMovementDirectionActive || pausePlayerTransformExtraRotationActive) {
				lastTimeMovementInputPressed = 0;

				checkingQuickMovementDirectionActive = false;

				pausePlayerTransformExtraRotationActive = false;

				activateExtraRotationInQuickTurnMovement = false;
			}
		} else {
			if (rawAxisValues != Vector2.zero) {
				if (lastTimeMovementInputPressed == 0 && isPlayerMoving (0.95f)) {
					lastTimeMovementInputPressed = Time.time;

					print ("Starting to check");

					checkingQuickMovementDirectionActive = true;
				}
			} else {
				if (lastTimeMovementInputPressed != 0 && !isPlayerMoving (0.1f)) {
					lastTimeMovementInputPressed = 0;

					print ("Resetting check");

					checkingQuickMovementDirectionActive = false;
				}
			}

			if (checkingQuickMovementDirectionActive && Time.time > lastTimeMovementInputPressed + minDelayToActivateQuickMovementTurn180OnRun) {			
				float movementDirectionTurnAmount = Mathf.Atan2 (localMove.x, localMove.z);

				if (Mathf.Abs (movementDirectionTurnAmount) > 0.95f) {

					if (walkSpeed < 1) {
						animator.SetInteger (quickTurnDirectionIDSpeedID, 0);

						quickTurnMovementDuration = quickTurnMovementDurationWalking;
					} else {
						if (running) {
							animator.SetInteger (quickTurnDirectionIDSpeedID, 2);

							quickTurnMovementDuration = quickTurnMovementDurationRunning;
						} else {
							animator.SetInteger (quickTurnDirectionIDSpeedID, 1);

							quickTurnMovementDuration = quickTurnMovementDurationSprinting;

							activateExtraRotationInQuickTurnMovement = true;
						}
					}

					if (movementDirectionTurnAmount > 0) {
						animator.SetTrigger (quickTurnRightDirectionID);
					} else {
						animator.SetTrigger (quickTurnLeftDirectionID);
					}

					pausePlayerTransformExtraRotationActive = true;

					checkingQuickMovementDirectionActive = false;

					lastTimeQuickTurnActive = Time.time;

					print ("quick change of direction");
				}
			}

			if (pausePlayerTransformExtraRotationActive) {
				if ((Time.time > lastTimeQuickTurnActive + quickTurnMovementDuration) ||
				    (playerInput.getPlayerRawMovementAxisWithoutCheckingEnabled () == Vector2.zero &&
				    Time.time > lastTimeQuickTurnActive + quickTurnMovementDuration / 2)) {

					lastTimeMovementInputPressed = 0;

					checkingQuickMovementDirectionActive = false;

					pausePlayerTransformExtraRotationActive = false;

					activateExtraRotationInQuickTurnMovement = false;
					print ("resume player");
				} else {
					if (activateExtraRotationInQuickTurnMovement) {
						Quaternion quickTurnRotatation = Quaternion.LookRotation (currentMoveInput, playerTransformUp);
						mainRigidbody.rotation = Quaternion.Lerp (mainRigidbody.rotation, quickTurnRotatation, quickTurnMovementRotationSpeed * currentFixedUpdateDeltaTime);
					}
				}
			}
		}
	}

	bool ignoreLookInCameraDirection;
	bool ignoreStrafeModeInputCheckActive;

	public void setIgnoreLookInCameraDirectionValue (bool state)
	{
		ignoreLookInCameraDirection = state;
	}

	public void setIgnoreStrafeModeInputCheckActiveState (bool state)
	{
		ignoreStrafeModeInputCheckActive = state;
	}

	float turnAmountClampValue = 1;

	public void setTurnAmountClampValue (float newValue)
	{
		turnAmountClampValue = newValue;
	}

	bool rotateInCameraDirectionOnAirOnExternalActionActive;

	public void setRotateInCameraDirectionOnAirOnExternalActionActiveState (bool state)
	{
		rotateInCameraDirectionOnAirOnExternalActionActive = state;
	}

	//function used when the player is aim mode, so the character will rotate in the camera direction
	void lookCameraDirection ()
	{
		if (usingAnimator) {
			if (wallRunningActive) {
				updateWallRunningState ();
			} else {
				bool rotateInLookDirectionResult = false;

				if ((((playerIsAiming && lookInCameraDirectionOnFreeFireActive) || lookInCameraDirection || lookInCameraDirectionActive) && playerOnGround) ||

				    (playerIsAiming && !usingFreeFireMode && !playerOnGround)) {

					rotateInLookDirectionResult = true;
				}

				if (ignoreLookInCameraDirection) {
					rotateInLookDirectionResult = false;
				}

				if (rotateInCameraDirectionOnAirOnExternalActionActive) {
					if (!ignoreLookInCameraDirection && !rotateInLookDirectionResult && !playerOnGround) {
						if (playerIsAiming || usingFreeFireMode) {

							rotateInLookDirectionResult = true;
						}
					}
				}

//				print (rotateInLookDirectionResult + " " + ignoreLookInCameraDirection + " " + playerIsAiming);

				if (rotateInLookDirectionResult) {

					bool ZGFFModeOnAir = ((zeroGravityModeOn || freeFloatingModeOn) && !playerOnGround);

					if (!ZGFFModeOnAir) {
						//get the camera direction, getting the local direction in any surface
						Vector3 forward = playerCameraTransform.TransformDirection (Vector3.forward);
						float newAngle = Vector3.Angle (forward, playerTransformForward);

						Vector3 targetDirection = vector3Zero;
						Quaternion targetRotation = quaternionIdentity;

						forward = forward - playerTransformUp * playerTransform.InverseTransformDirection (forward).y;
						forward = forward.normalized;
						targetDirection = forward;

						Quaternion currentRotation = Quaternion.LookRotation (targetDirection, playerTransformUp);
						targetRotation = Quaternion.Lerp (mainRigidbody.rotation, currentRotation, aimTurnSpeed * currentFixedUpdateDeltaTime);

						bool canRotatePlayer = (!lookAlwaysInCameraDirection ||
						                       (lookAlwaysInCameraDirection && !lookOnlyIfMoving) ||
						                       (lookAlwaysInCameraDirection && lookOnlyIfMoving && playerUsingInput) ||
						                       (lookAlwaysInCameraDirection && lookOnlyIfMoving && !playerUsingInput && playerIsAiming));

						if (ignoreCameraDirectionOnStrafeMovement) {
							canRotatePlayer = false;
						}

						if (newAngle >= Mathf.Abs (thresholdAngleDifference)) {
							//if the player is not moving, set the turnamount to rotate him around, setting its turn animation properly
							if (canRotatePlayer) {
								targetRotation = Quaternion.Lerp (mainRigidbody.rotation, currentRotation, autoTurnSpeed * currentFixedUpdateDeltaTime);
								//							mainRigidbody.MoveRotation (targetRotation);

								Vector3 lookDelta = playerTransform.InverseTransformDirection (targetDirection * 100);
								float lookAngle = Mathf.Atan2 (lookDelta.x, lookDelta.z) * Mathf.Rad2Deg;
								turnAmount += lookAngle * .01f * 6;
							}
						} else {
							turnAmount = Mathf.MoveTowards (turnAmount, 0, currentFixedUpdateDeltaTime * 2);
						}

						if (rotateDirectlyTowardCameraOnStrafe) {
							turnAmount = 0;
						}

						if (canRotatePlayer) {
							mainRigidbody.MoveRotation (targetRotation);
							//if (usedByAI) {
							//print ((((playerIsAiming && lookInCameraDirectionOnFreeFireActive) || lookInCameraDirection || lookInCameraDirectionActive) && playerOnGround) + " " +
							//(playerIsAiming && !usingFreeFireMode && !playerOnGround) + "  " + "ROTATION");

							//print (lookInCameraDirection + " " + lookInCameraDirectionActive + "  " + "ROTATION");
							//}
						}
					}
				}

				if ((zeroGravityModeOn || freeFloatingModeOn) && !firstPersonActive && !playerOnGround && !characterRotatingToSurface) {
					if (isPlayerMovingOn3dWorld () || playerIsAiming) {
						if (playerIsAiming) {
							playerTransform.rotation = playerCameraTransform.rotation;
						} else {
							Quaternion targetRotation = Quaternion.Lerp (playerTransform.rotation, playerCameraTransform.rotation, zeroGravityLookCameraSpeed * currentFixedUpdateDeltaTime);
							playerTransform.rotation = targetRotation;
						}
					}
				}
			}
		} else {

			//Check if the wall running function is active
			if (wallRunningActive) {
				updateWallRunningState ();
			} else {
				if (!characterRotatingToSurface && !pausePlayerRotateToCameraDirectionOnFirstPersonActive) {
					playerTransform.rotation = playerCameraTransform.rotation;
				}
			}
		}
	}

	bool pausePlayerRotateToCameraDirectionOnFirstPersonActive;

	public void setPausePlayerRotateToCameraDirectionOnFirstPersonActiveState (bool state)
	{
		pausePlayerRotateToCameraDirectionOnFirstPersonActive = state;
	}

	void addExtraRotation ()
	{
		if (addExtraRotationPaused) {
			return;
		}

		if (usingAnimator && !wallRunningActive) {
			if ((!playerIsAiming || !lookInCameraDirectionOnFreeFireActive) && !lookInCameraDirectionActive) {

				canRotateOn3dWorld = isPlayerMovingOn3dWorld () && ((!freeFloatingModeOn && !zeroGravityModeOn) || playerOnGround);
				//&& !playerCameraManager.isCurrentCameraLookInPlayerDirection ();

				canRotateOn2_5dWorld = isPlayerMovingOn2_5dWorld ();

				if (canRotateOn3dWorld || canRotateOn2_5dWorld) {

					if (pausePlayerTransformExtraRotationActive) {
						return;
					}

					//add an extra rotation to the player to get a smooth movement
					if (lockedCameraActive) {
						float turnSpeed = Mathf.Lerp (stationaryTurnSpeed, movingTurnSpeed, forwardAmount);

						if (useTankControls) {
							turnSpeed *= tankModeRotationSpeed;

							float newRotationValue = horizontalInput * turnSpeed * currentFixedUpdateDeltaTime * 1.5f;

							if (ignorePlayerExtraRotationOnAirEnabled && !playerOnGround) {
								newRotationValue = 0;
							}

							if (Mathf.Abs (newRotationValue) > 0.001f) {
								playerTransform.Rotate (0, newRotationValue, 0);
							}
						} else {
							float newRotationValue = turnAmount * turnSpeed * currentFixedUpdateDeltaTime * 1.5f;

							if (ignorePlayerExtraRotationOnAirEnabled && !playerOnGround) {
								newRotationValue = 0;
							}

							if (Mathf.Abs (newRotationValue) > 0.001f) {
								playerTransform.Rotate (0, newRotationValue, 0);
							}
						}
					} else {
						if (!playerIsAiming || useTurnSpeedOnAim) {
							float turnSpeed = Mathf.Lerp (stationaryTurnSpeed, movingTurnSpeed, forwardAmount);

							float newRotationValue = turnAmount * turnSpeed * currentFixedUpdateDeltaTime * 1.5f;

							if (ignorePlayerExtraRotationOnAirEnabled && !playerOnGround) {
								newRotationValue = 0;
							}

							if (Mathf.Abs (newRotationValue) > 0.001f) {
								playerTransform.Rotate (0, newRotationValue, 0);
							}
						}
					}
				}
			}
		}
	}

	//set the normal of the player every time it is changed in the other script
	public void setCurrentNormalCharacter (Vector3 newNormal)
	{
		currentNormal = newNormal;
	}

	public void setApplyAnimatorVelocityWithoutMovingState (bool state)
	{
		applyAnimatorVelocityWithoutMoving = state;
	}

	//check if the player jumps
	void onGroundVelocity ()
	{
		if (currentMoveInput.magnitude == 0 && !actionActiveWithMovement && (!applyAnimatorVelocityWithoutMoving || actionActive)) {
			currentVelocity = vector3Zero;
		}

		//check when the player is able to jump, according to the timer and the animator state
		bool animationGrounded = false;

		if (usingAnimator && !useFirstPersonPhysicsInThirdPersonActive && !customCharacterControllerActive) {
			animationGrounded = animator.GetCurrentAnimatorStateInfo (baseLayerIndex).IsName ("Grounded");

			if (!animationGrounded) {
				if (playerOnGround && Time.time > lastTimeGround + half) {
					animationGrounded = true;
				}
			}
		} else {
			animationGrounded = playerOnGround;
		}

		//if the player jumps, apply velocity to its rigidbody
		if (jumpInput && Time.time > lastTimeAir + readyToJumpTime && animationGrounded) {	
			if (updateFootStepStateActive) {	
				if (playerOnGround) {
					if (currentFootStepState != footStepState.Jumping) {
						stepManager.setFootStepState ("Jumping");

						currentFootStepState = footStepState.Jumping;
					}
				}
			}

			if (customCharacterControllerActive) {
				currentCustomCharacterControllerBase.activateJumpAnimatorState ();
			} else {
				animator.SetFloat (jumpAnimatorID, 0);
			}

			setOnGroundState (false);

			if (usingAnimator && !useFirstPersonPhysicsInThirdPersonActive) {
				currentVelocity = currentMoveInput * airSpeed;
			} else {
				currentVelocity = currentMoveInput * noAnimatorAirSpeed;
			}

			currentVelocity = currentVelocity + playerTransformUp * jumpPower;

			jump = true;

			lastJumpTime = Time.time;

			//this is used for the headbod, to shake the camera when the player jumps
			if (updateHeadbobState && headBobManagerLocated) {
				if (currentHeadbobState != headbobState.Jump_Start) {
					headBobManager.setState ("Jump Start");

					currentHeadbobState = headbobState.Jump_Start;
				}
			}

			if (weaponsManagerLocated) {
				weaponsManager.setWeaponsJumpStartPositionState (true);
			}

			disableCrouchState = false;

			if (firstPersonActive) {
				if (getUpIfJumpOnCrouchInFirstPerson) {
					disableCrouchState = true;
				}
			} else {
				if (getUpIfJumpOnCrouchInThirdPerson) {
					disableCrouchState = true;
				}
			}

			if (disableCrouchState) {
				if (crouching) {
					setCrouchComponentsState (false);

					if (firstPersonActive) {
						crouching = false;
					}
				}
			}

			if (useEventOnJump) {
				eventOnJump.Invoke ();
			}

			if (stopCrouchOnJumpEnabled) {
				if (crouching) {
					if (playerCanGetUpFromCrouch ()) {
						stopCrouchFromAutoJumpActive = true;

						if (!firstPersonActive) {
							if (weaponsManagerLocated) {
								weaponsManager.enableOrDisableIKOnHands (true);
							}
						}
							
						setCrouchComponentsState (false);

						if (crouchSlidingEnabled && running) {
							if (firstPersonActive) {
								stopCrouchSliding ();
							}
						}
					}
				}
			}
		}

		jumpInput = false;
	}

	bool stopCrouchFromAutoJumpActive;

	//check if the player is in the air falling, applying the gravity force in his local up negative
	void onAirVelocity ()
	{
		if (!gravityPowerActive &&
			
		    !usingJetpack &&

		    (!flyModeActive || enableExternalForceOnFlyMode) &&

		    !characterRotatingToSurface &&
		    
		    (!swimModeActive || enableExternalForceOnSwimMode) &&

		    !externalControlBehaviorForAirTypeActive) {

			//when the player falls, allow him to move to his right, left, forward and backward with WASD

			if (zeroGravityModeOn || freeFloatingModeOn) {

				secondaryMoveInput = currentMoveInput;

				canMoveVertically = (zeroGravityModeOn && canMoveVerticallyOnZeroGravity) || (freeFloatingModeOn && canMoveVerticallyOnFreeFloating);

				canMoveVerticallyAndHorizontal = (zeroGravityModeOn && canMoveVerticallyAndHorizontalZG) || (freeFloatingModeOn && canMoveVerticallyAndHorizontalFF);

				if (movingVertically) {
					if (zeroGravityModeOn) {
						currentVerticalSpeed = zeroGravityModeVerticalSpeed;
					} else {
						currentVerticalSpeed = freeFloatingModeVerticalSpeed;
					}

					if (canMoveVerticallyAndHorizontal) {
						secondaryMoveInput += (verticalInput * currentVerticalSpeed) * playerCameraTransform.up;
					} else {
						secondaryMoveInput = (verticalInput * playerCameraTransform.up + horizontalInput * mainCameraTransform.right) * currentVerticalSpeed;	
					}
				}

				if (externalForceOnAirActive) {
					airMove = externalForceOnAirValue;

					currentVelocity = Vector3.Lerp (currentVelocity, airMove, currentFixedUpdateDeltaTime * externalForceOnAirControlValue);

					externalForceOnAirActive = false;
				} else {
					if (zeroGravityModeOn) {
						secondaryMove = (zeroGravityMovementSpeed * currentZeroGravitySpeedMultiplier) * secondaryMoveInput;

						currentVelocity = Vector3.Lerp (currentVelocity, secondaryMove, currentFixedUpdateDeltaTime * zeroGravityControlSpeed);
					}

					if (freeFloatingModeOn) {
						secondaryMove = (freeFloatingMovementSpeed * currentFreeFloatingSpeedMultiplier) * secondaryMoveInput;

						currentVelocity = Vector3.Lerp (currentVelocity, secondaryMove, currentFixedUpdateDeltaTime * freeFloatingControlSpeed);
					}
				}
			} else {

				if (usingAnimator && !useFirstPersonPhysicsInThirdPersonActive && !wallRunningActive) {
					if (externalForceOnAirActive) {
						airMove = externalForceOnAirValue;

						currentVelocity = Vector3.Lerp (currentVelocity, airMove, currentFixedUpdateDeltaTime * externalForceOnAirControlValue);

						externalForceOnAirActive = false;
					} else {
						if (!ignoreInputOnAirControlActive) {
							airMove = currentMoveInput * airSpeed + playerTransform.InverseTransformDirection (currentVelocity).y * playerTransformUp;

							currentVelocity = Vector3.Lerp (currentVelocity, airMove, currentFixedUpdateDeltaTime * airControl);
						}
					}
				}

				checkIfActivateWallRunningState ();
			}

			//also, apply force in his local negative Y Axis
			if (!playerOnGround) {
				if (slowingFallInput) {
					slowingFall = true;
					slowingFallInput = false;
				}

				if (!gravityForcePaused && !zeroGravityModeOn && !freeFloatingModeOn && !wallRunningActive && !pauseAllPlayerDownForces && !externalControlBehaviorForAirTypeActive) {
					mainRigidbody.AddForce (gravityForce * mainRigidbody.mass * playerTransformUp);

					extraGravityForce = (gravityForce * gravityMultiplier) * playerTransformUp + playerTransformUp * -gravityForce;

					mainRigidbody.AddForce (extraGravityForce);
				}
			}

			//also apply force if the player jumps again
			if (doubleJump) {
				if (usingAnimator && !useFirstPersonPhysicsInThirdPersonActive) {
					currentVelocity += currentMoveInput * airSpeed;
				} else {
					currentVelocity += currentMoveInput * noAnimatorAirSpeed;
				}

				float currentDoubleJumpPower = jumpPower;

				if (useNewDoubleJumpPower) {
					currentDoubleJumpPower = doubleJumpPower;
				}

				if (removeVerticalSpeedOnDoubleJump) {
					float currentVerticalVelocity = playerTransform.InverseTransformDirection (currentVelocity).y;

					currentVelocity -= playerTransform.up * currentVerticalVelocity;
				}

				currentVelocity = currentVelocity + playerTransformUp * currentDoubleJumpPower;

				if (!ladderFound) {
					jumpsAmount++;
				}

				lastDoubleJumpTime = Time.time;

				if (useEventOnJump) {
					eventOnJump.Invoke ();
				}
			}

			doubleJump = false;

		} else {
			if (characterRotatingToSurface) {
				mainRigidbody.velocity = vector3Zero;

				currentVelocity = vector3Zero;
			}
		}

		if (fallDamageEnabled) {
			lastFrameAirVelocity = mainRigidbody.velocity.magnitude;
		}
	}

	public void setApplyRootMotionAlwaysActiveState (bool state)
	{
		applyRootMotionAlwaysActive = state;
	}
		
	//update the animator values
	void updateAnimator ()
	{
		//set the rootMotion according to the player state
		if (!applyRootMotionAlwaysActive) {
			if (useRootMotionActive) {
				if (currentApplyRootMotionValue != playerOnGround) {
					animator.applyRootMotion = playerOnGround;

					currentApplyRootMotionValue = playerOnGround;
				}
			} else {
				if (currentApplyRootMotionValue) {
					animator.applyRootMotion = false;

					currentApplyRootMotionValue = false;
				}
			}
		} else {
			if (!currentApplyRootMotionValue) {
				animator.applyRootMotion = true;

				currentApplyRootMotionValue = true;
			}
		}

		rootMotionCurrentlyActive = animator.applyRootMotion;

		if (running && sprintEnabled && sprintCanBeUsed) {
			forwardAmount *= 2;
		}

		if (customCharacterControllerActive) {
			if (currentCustomCharacterControllerBase.updateForwardAmountInputValueFromPlayerController) {
				currentCustomCharacterControllerBase.updateForwardAmountInputValue (forwardAmount);
			}

			if (currentCustomCharacterControllerBase.updateTurnAmountInputValueFromPlayerController) {
				currentCustomCharacterControllerBase.updateTurnAmountInputValue (turnAmount);
			}

			if (currentCustomCharacterControllerBase.updateUsingInputValueFromPlayerController) {
				currentCustomCharacterControllerBase.updatePlayerUsingInputValue (playerUsingInput);
			}

			updateAnimatorSpeed ();
		} else {
			if (!lockedCameraActive || !isPlayerMovingOn2_5dWorld ()) {
				//if the player is not aiming, set the forward direction
				if (!playerIsAiming || (!lookInCameraDirectionActive && (usingFreeFireMode || (checkToKeepWeaponAfterAimingWeaponFromShooting || Time.time < lastTimeCheckToKeepWeapon + 0.2f)))) {
					animator.SetFloat (forwardAnimatorID, forwardAmount, animatorForwardInputLerpSpeed, currentFixedUpdateDeltaTime);
				} else {
					//else, set its forward to 0, to prevent any issue
					//this value is set to 0, because the player uses another layer of the mecanim to move while he is aiming
					animator.SetFloat (forwardAnimatorID, 0);
				}
			} else {
				animator.SetFloat (forwardAnimatorID, forwardAmount, animatorForwardInputLerpSpeed, currentFixedUpdateDeltaTime);
			}

			if (isTankModeActive ()) {
				if (!playerIsAiming) {
					animator.SetFloat (turnAnimatorID, 0);

					if (tankModeCurrentlyEnabled) {
						tankModeActive = true;
					} else {
						tankModeActive = false;
					}
				} else {
					if (isPlayerMoving (0)) {
						turnAmount = 0;
					}

					if (overrideTurnAmountActive) {
						turnAmount = overrideTurnAmount;
					}

					turnAmount = Mathf.Clamp (turnAmount, -1, 1);

					animator.SetFloat (turnAnimatorID, turnAmount, animatorTurnInputLerpSpeed, currentFixedUpdateDeltaTime);

					if (playerCanMoveOnAimInTankMode) {
						tankModeActive = false;
					}
				}
			} else {
				if (ignoreStrafeModeInputCheckActive) {
					lastTimePlayerUserInput = 0;

					ignoreStrafeModeInputCheckActive = false;

					strafeModeActive = false;
				}

				if (strafeModeActive) {
					if (lookAlwaysInCameraDirection && lookOnlyIfMoving) {
						if (!playerIsAiming) {
							turnAmount = 0;
							lastTimePlayerUserInput = Time.time;
						}
					}

					if (playerUsingInput) {
						turnAmount = 0;
						lastTimePlayerUserInput = Time.time;
					}
				} 

				if (lastTimePlayerUserInput > 0) {
					if (isPlayerMoving (0)) {
						turnAmount = 0;
					} else {
						lastTimePlayerUserInput = 0;
					}
				}

				if (overrideTurnAmountActive) {
					turnAmount = overrideTurnAmount;
				}

				turnAmount = Mathf.Clamp (turnAmount, -turnAmountClampValue, turnAmountClampValue);

				animator.SetFloat (turnAnimatorID, turnAmount, animatorTurnInputLerpSpeed, currentFixedUpdateDeltaTime);

				tankModeActive = false;
			}

			if (Mathf.Abs (horizontalInput) > 0 || Mathf.Abs (verticalInput) > 0) {
				lastTimeInput = Time.time;
			}

			if (useTankControls) {
				lastTimeInputPressed = true;
			} else {
				lastTimeInputPressed = Time.time > lastTimeInput + minTimeToSetIdleOnStrafe;
			}

			if (obstacleToAvoidMovementFound) {
				lastTimeInputPressed = true;
			}

			if (previousValueLastTimeInputPressed != lastTimeInputPressed) {
				previousValueLastTimeInputPressed = lastTimeInputPressed;

				animator.SetBool (lastTimeInputPressedAnimatorID, lastTimeInputPressed);
			}

			if (overrideOnGroundAnimatorValueActive) {
				onGroundAnimatorState = playerOnGroundAnimatorStateOnOverrideOnGround;

				if (playerOnGroundAnimatorStateOnOverrideOnGroundWithTime) {
					if (Time.time > lastTimeOverrideOnGroundAnimator + overrideOnGroundDuration) {
						overrideOnGroundAnimatorValueActive = false;
					}
				}
			} else {
				onGroundAnimatorState = playerOnGround;
			}

			if (previousValueOnGroundAnimatorState != onGroundAnimatorState) {
				previousValueOnGroundAnimatorState = onGroundAnimatorState;

				animator.SetBool (onGroundAnimatorID, onGroundAnimatorState);
			}

			if (previousValueCrouching != crouching) {
				previousValueCrouching = crouching;

				animator.SetBool (crouchAnimatorID, crouching);
			}

			animator.SetBool (strafeModeActiveAnimatorID, strafeModeActive);

			if (lockedCameraActive) {
				if (isPlayerMovingOn2_5dWorld ()) {

					scrollModeActive = true;
					animator.SetBool (strafeModeActiveAnimatorID, false);

				} else {
					scrollModeActive = false;
				}
			}

			movingAnimator = isMoving;

			if (obstacleToAvoidMovementFound) {
				movingAnimator = false;
			}

			if (previousValueMovingAnimator != movingAnimator) {
				previousValueMovingAnimator = movingAnimator;

				animator.SetBool (movingAnimatorID, movingAnimator);
			}

			movementInputActive = playerUsingInput;

			if (obstacleToAvoidMovementFound) {
				movementInputActive = false;
			}

			if (tankModeActive && !playerCanMoveOnAimInTankMode) {
				if (playerIsAiming) {
					movementInputActive = false;
				}
			}

			if (previousValueMovementInputActive != movementInputActive) {
				previousValueMovementInputActive = movementInputActive;

				animator.SetBool (movementInputActiveAnimatorID, movementInputActive);
			}

			movementRelativeToCameraAnimatorState = lockedCameraActive && useRelativeMovementToLockedCamera;

			if (previousValueMovementRelativeToCameraState != movementRelativeToCameraAnimatorState) {
				previousValueMovementRelativeToCameraState = movementRelativeToCameraAnimatorState;

				animator.SetBool (movementRelativeToCameraAnimatorID, movementRelativeToCameraAnimatorState);
			}

			if (playerIsAiming) {
				currentMovementID = 1;
			} else {
				currentMovementID = 0;
			}

			animator.SetFloat (movementIDAnimatorID, currentMovementID, 0.1f, currentFixedUpdateDeltaTime);

			if (!tankModeActive && !scrollModeActive) {
				currentPlayerModeID = 0;
			} else {
				if (tankModeActive) {
					currentPlayerModeID = 1;
				}

				if (scrollModeActive) {
					currentPlayerModeID = 2;
				}
			}

			animator.SetFloat (playerModeIDAnimatorID, currentPlayerModeID, 0.1f, currentFixedUpdateDeltaTime);

			if (weaponsManagerLocated) {
				carryingWeaponAnimatorState = weaponsManager.isUsingWeapons ();
			} else {
				carryingWeaponAnimatorState = false;
			}

			if (previousValueCarryingWeaponAnimatorState != carryingWeaponAnimatorState) {
				previousValueCarryingWeaponAnimatorState = carryingWeaponAnimatorState;

				animator.SetBool (carryingWeaponAnimatorID, carryingWeaponAnimatorState);
			}

			if (lockedCameraActive && useRelativeMovementToLockedCamera) {
				currentWalkSpeedOnLockedCamera = walkSpeed;

				if (isPlayerAiming ()) {
					currentWalkSpeedOnLockedCamera = 1;
				}

				lockedCameraMoveInput = (verticalInput * currentLockedCameraTransform.forward + horizontalInput * currentLockedCameraTransform.right) * currentWalkSpeedOnLockedCamera;	
				lockedMove = playerTransform.InverseTransformDirection (lockedCameraMoveInput);

				currentHorizontalValue = lockedMove.x;
				currentVerticalValue = lockedMove.z;
			} else {

				if (tankModeActive) {
					if (running && sprintEnabled && sprintCanBeUsed) {
						verticalInput *= 2;
					} else {
						if (verticalInput > 0) {
							currentWalkSpeedOnLockedCamera = walkSpeed;

							if (isPlayerAiming ()) {
								currentWalkSpeedOnLockedCamera = 1;
							}

							verticalInput = Mathf.Clamp (verticalInput, 0, currentWalkSpeedOnLockedCamera);
						}
					}
				}

				currentHorizontalValue = horizontalInput;
				currentVerticalValue = verticalInput;
			}

			if (useTankControls) {
				currentHorizontalInputLerpSpeed = inputTankControlsHorizontalLerpSpeed;
				currentVerticalInputLerpSpeed = inputTankControlsVerticalLerpSpeed;
			} else {
				currentHorizontalInputLerpSpeed = inputHorizontalLerpSpeed;
				currentVerticalInputLerpSpeed = inputVerticalLerpSpeed;
			}

			if (flyModeActive || swimModeActive || externalControlBehaviorForAirTypeActive) {
				if (!firstPersonActive && !characterRotatingToSurface) {
					if (isMoving && isPlayerAiming ()) {
						currentHorizontalValue = 0;
						currentVerticalValue = 0;
					}
				}
			}

			animator.SetFloat (horizontalAnimatorID, currentHorizontalValue, currentHorizontalInputLerpSpeed, currentFixedUpdateDeltaTime);
			animator.SetFloat (verticalAnimatorID, currentVerticalValue, currentVerticalInputLerpSpeed, currentFixedUpdateDeltaTime);

			if (obstacleToAvoidMovementFound) {
				currentHorizontalValue = 0;
				currentVerticalValue = 0;
			}

			if (useTankControls) {
				currentInputStrafeHorizontalLerpSpeed = inputTankControlsHorizontalStrafeLerpSpeed;
				currentInputStrafeVerticalLerpSpeed = inputTankControlsVerticalStrafeLerpSpeed;
			} else {
				currentInputStrafeHorizontalLerpSpeed = inputStrafeHorizontalLerpSpeed;
				currentInputStrafeVerticalLerpSpeed = inputStrafeVerticalLerpSpeed;
			}

			if (usedByAI) {
				if (!playerIsAiming && !AIStrafeModeActive) {
					currentHorizontalValue = 0;
					currentVerticalValue = 0;
				}
			}

			animator.SetFloat (horizontalStrafeAnimatorID, currentHorizontalValue, currentInputStrafeHorizontalLerpSpeed, currentFixedUpdateDeltaTime);
			animator.SetFloat (verticalStrafeAnimatorID, currentVerticalValue, currentInputStrafeVerticalLerpSpeed, currentFixedUpdateDeltaTime);

			if (strafeModeActive) {
				if (running) {
					currentMovementSpeed = defaultStrafeRunSpeed;
				} else {
					currentMovementSpeed = defaultStrafeWalkSpeed;
				}
			} else {
				currentMovementSpeed = defaultStrafeWalkSpeed;
			}

			animator.SetFloat (movementSpeedAnimatorID, currentMovementSpeed, strafeLerpSpeed, currentFixedUpdateDeltaTime);

			if (!playerOnGround) {
				//when the player enables the power gravity and he is floating in the air, set this value to 0 to set 
				//the look like floating animation
				if (gravityPowerActive) {
					animator.SetFloat (jumpAnimatorID, 0);
				}
				//else set his jump value as his current rigidbody velocity
				else {
					if (zeroGravityModeOn || freeFloatingModeOn) {
						animator.SetFloat (jumpAnimatorID, 0);
					} else {
						currentJumpAnimatorValue = playerTransform.InverseTransformDirection (mainRigidbody.velocity).y;

						if (jumpLegExternallyActiveState) {
							currentJumpAnimatorValue = -9;
						}

						animator.SetFloat (jumpAnimatorID, currentJumpAnimatorValue, 0.1f, currentFixedUpdateDeltaTime / 2);
					}
				}
			}

			if (usingJetpack || flyModeActive || turnAndForwardAnimatorValuesPaused) {
				animator.SetFloat (forwardAnimatorID, 0);
				animator.SetFloat (turnAnimatorID, 0);
			}

			//this value is used to know in which leg the player has to jump, left of right
			float runCycle = Mathf.Repeat (animator.GetCurrentAnimatorStateInfo (baseLayerIndex).normalizedTime + runCycleLegOffset, 1);
			float jumpLeg = (runCycle < half ? 1 : -1) * forwardAmount;

			if (playerOnGround) {
				animator.SetFloat (jumpLegAnimatorID, jumpLeg);
			} else {
				if (zeroGravityModeOn || freeFloatingModeOn) {
					if (currentJumpLeg == 0) {
						currentJumpLeg = jumpLeg;
					}

					if (zeroGravityModeOn) {
						currentJumpLeg = 1;
					}

					currentJumpLeg = Mathf.Lerp (currentJumpLeg, 1, currentFixedUpdateDeltaTime);

					animator.SetFloat (jumpLegAnimatorID, currentJumpLeg);
				}

				if (gravityPowerActive || jumpLegExternallyActiveState) {
					if ((weaponsManagerLocated && weaponsManager.isUsingWeapons ()) || aimingPowers) {
						currentJumpLeg = Mathf.Lerp (currentJumpLeg, 1, currentFixedUpdateDeltaTime / 2);
					} else {
						currentJumpLeg = Mathf.Lerp (currentJumpLeg, 0, currentFixedUpdateDeltaTime / 2);
					}

					animator.SetFloat (jumpLegAnimatorID, currentJumpLeg, 0.1f, currentFixedUpdateDeltaTime / 2);
				}
			}

			if (playerIsAiming && !usingFreeFireMode && !checkToKeepWeaponAfterAimingWeaponFromShooting) {
				aimingModeActiveAnimatorState = true;
			} else {
				aimingModeActiveAnimatorState = false;
			}

			if (previousValueAimingModeActiveAnimatorState != aimingModeActiveAnimatorState) {
				previousValueAimingModeActiveAnimatorState = aimingModeActiveAnimatorState;

				animator.SetBool (aimingModeActiveAnimatorID, aimingModeActiveAnimatorState);
			}

			updateAnimatorSpeed ();

			animator.SetFloat (playerStatusIDAnimatorID, playerStatusID, playerStatusIDLerpSpeed, currentFixedUpdateDeltaTime);

			animator.SetFloat (idleIDAnimatorID, currentIdleID, 0.1f, currentFixedUpdateDeltaTime);

			animator.SetFloat (strafeIDAnimatorID, currentStrafeID, 0.1f, currentFixedUpdateDeltaTime);

			animator.SetFloat (crouchIDAnimatorID, currentCrouchID, 0.1f, currentFixedUpdateDeltaTime);

			animator.SetFloat (airIDAnimatorID, currentAirID, 0.1f, currentFixedUpdateDeltaTime);

			animator.SetFloat (airSpeedAnimatorID, currentAirSpeed, 0.1f, currentFixedUpdateDeltaTime); 

			animator.SetFloat (shieldActiveAnimatorID, currentShieldActive, 0.1f, currentFixedUpdateDeltaTime);

			if (playerUsingInput) {
				animator.SetFloat (inputAmountID, 1, 0.1f, currentFixedUpdateDeltaTime);
			} else {
				animator.SetFloat (inputAmountID, 0, 0.1f, currentFixedUpdateDeltaTime);
			}
		}

		if (customCharacterControllerActive) {
			currentCustomCharacterControllerBase.updateCharacterControllerAnimator ();
		}
	}

	void updateAnimatorSpeed ()
	{
		if (!animatorSpeedUpdatePaused) {
			//if the player is on ground and moving set the speed of his animator to the properly value
			if (playerOnGround && currentMoveInput.magnitude > 0) {
				currentAnimatorSpeed = animSpeedMultiplier;
			} else {
				currentAnimatorSpeed = 1;
			}
		}

		if (overrideAnimationSpeedActive) {
			currentAnimatorSpeed = animSpeedMultiplier;
		}

		if (regularMovementOnBulletTime) {
			currentAnimatorSpeed *= GKC_Utils.getCurrentDeltaTime ();
		}

		if (currentAnimatorSpeed != previousAnimatorSpeed) {
			previousAnimatorSpeed = currentAnimatorSpeed;

			animator.speed = currentAnimatorSpeed;
		}
	}

	public void setPlayerStatusIDValue (int newValue)
	{
		playerStatusID = newValue;
	}

	public void resetPlayerStatusID ()
	{
		setPlayerStatusIDValue (0);
	}

	public int getPlayerStatusID ()
	{
		return playerStatusID;
	}

	public void setCurrentIdleIDValue (int newValue)
	{
		currentIdleID = newValue;
	}

	public int getCurrentIdleID ()
	{
		return currentIdleID;
	}

	public void setCurrentStrafeIDValue (int newValue)
	{
		currentStrafeID = newValue;
	}

	public int getCurrentStrafeIDValue ()
	{
		return currentStrafeID;
	}

	public void setCurrentCrouchIDValue (int newValue)
	{
		currentCrouchID = newValue;
	}

	public int getCurrentCrouchIDValue ()
	{
		return currentCrouchID;
	}

	public void setCurrentAirIDValue (int newValue)
	{ 
		currentAirID = newValue;
	}

	public void setOriginalAirIDValue ()
	{ 
		setCurrentAirIDValue (regularAirboneID);
	}

	public void setCurrentAirSpeedValue (int newValue)
	{
		currentAirSpeed = newValue;
	}

	public void setCurrentShieldActiveValue (int newValue)
	{
		currentShieldActive = newValue;
	}

	public int getCurrentAirID ()
	{
		return currentAirID;
	}

	public void updatePlayerStatusIDOnAnimator ()
	{
		if (!customCharacterControllerActive) {
			animator.SetFloat (playerStatusIDAnimatorID, playerStatusID);
		}
	}

	public void updateIdleIDOnAnimator ()
	{
		if (!customCharacterControllerActive) {
			animator.SetFloat (idleIDAnimatorID, currentIdleID);
		}
	}

	public void updateStrafeIDOnAnimator ()
	{
		if (!customCharacterControllerActive) {
			animator.SetFloat (strafeIDAnimatorID, currentStrafeID);
		}
	}

	public void updateAirIDOnAnimator ()
	{
		if (!customCharacterControllerActive) {
			animator.SetFloat (airIDAnimatorID, currentAirID);
		}
	}

	public void updateAirIDAnimatorID ()
	{
		if (!customCharacterControllerActive) {
			animator.SetFloat (airIDAnimatorID, currentAirID);
		}
	}

	public void setPlayerOnGroundAnimatorStateOnOverrideOnGround (bool state)
	{
		playerOnGroundAnimatorStateOnOverrideOnGround = state;
	}

	public void setPlayerOnGroundAnimatorStateOnOverrideOnGroundWithTime (bool state)
	{
		playerOnGroundAnimatorStateOnOverrideOnGroundWithTime = state;
	}

	public void setPreviousValueOnGroundAnimatorStateValue (bool state)
	{
		previousValueOnGroundAnimatorState = state;
	}

	public void overrideOnGroundAnimatorValue (float newOverrideOnGroundDuration)
	{
		overrideOnGroundDuration = newOverrideOnGroundDuration;

		overrideOnGroundAnimatorValueActive = true;

		lastTimeOverrideOnGroundAnimator = Time.time;
	}

	public void disableOverrideOnGroundAnimatorValue ()
	{
		overrideOnGroundAnimatorValueActive = false;
	}

	//update the velocity of the player rigidbody
	public void OnAnimatorMove ()
	{
		if (!gravityPowerActive && usingAnimator && !useFirstPersonPhysicsInThirdPersonActive) {
			mainRigidbody.rotation = animator.rootRotation;

			if ((playerOnGround || (actionActive && !actionCanHappenOnAir)) && Time.deltaTime > 0) {
				characterVelocity = vector3Zero;

				if (useRootMotionActive || actionActive) {
					characterVelocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
				} else {
					currentNoRootMovementSpeed = 1;

					if (running && !crouching) {
						if (strafeModeActive) {
							currentNoRootMovementSpeed = noRootRunStrafeMovementSpeed;
						} else {
							currentNoRootMovementSpeed = noRootSprintMovementSpeed;
						}
					} else {
						if (crouching) {
							currentNoRootMovementSpeed = noRootCrouchMovementSpeed;
						} else {
							if (strafeModeActive) {
								currentNoRootMovementSpeed = noRootWalkStrafeMovementSpeed;
							} else {
								if (walkSpeed >= half) {
									if (forwardAmount < half) {
										currentNoRootMovementSpeed = noRootWalkMovementSpeed / walkSpeed;
									} else {
										currentNoRootMovementSpeed = noRootRunMovementSpeed * walkSpeed;
									}
								} else {
									currentNoRootMovementSpeed = noRootWalkMovementSpeed;
								}
							}
						}
					}

					if (!strafeModeActive) {
						currentNoRootMovementSpeed -= currentMovementSpeed * (Mathf.Abs (turnAmount));
					}

					if (Time.time < lastTimeGround + 0.2f) {
						currentNoRootMovementSpeed = 0; 
					}

					lerpNoRootMovementSpeed = Mathf.Lerp (lerpNoRootMovementSpeed, currentNoRootMovementSpeed, Time.deltaTime * noRootRunMovementSpeed);

					if (lockedCameraActive && useRelativeMovementToLockedCamera) {

						characterVelocity = lockedCameraMoveInput * lerpNoRootMovementSpeed;

						characterVelocity = Vector3.ClampMagnitude (characterVelocity, currentNoRootMovementSpeed);
					} else {
						characterVelocity = currentMoveInput * lerpNoRootMovementSpeed;
					}
				}

				mainRigidbody.velocity = characterVelocity;

				if (addExtraCharacterVelocityActive) {
					//					print (currentExtraCharacterVelocity);

					mainRigidbody.velocity += currentExtraCharacterVelocity;
				}

				if (actionActive) {

					if (!allowDownVelocityDuringAction) {
						if (mainRigidbody.velocity.y < 0) {
							mainRigidbody.velocity = new Vector3 (mainRigidbody.velocity.x, 0, mainRigidbody.velocity.z);
						}
					}
				}

				currentVelocityChangeMagnitude = mainRigidbody.velocity.magnitude;
			}
		}

		if (playerActionSystemLocated && mainPlayerActionSystem.actionActive) {
			mainPlayerActionSystem.OnAnimatorMove ();
		}
	}

	public void activateCustomAction (string actionName)
	{
		if (playerActionSystemLocated) {
			mainPlayerActionSystem.activateCustomAction (actionName);
		}
	}

	public void stopCustomAction (string actionName)
	{
		if (playerActionSystemLocated) {
			mainPlayerActionSystem.stopCustomAction (actionName);
		}
	}

	public void stopAllActionsOnActionSystem ()
	{
		if (playerActionSystemLocated) {
			mainPlayerActionSystem.stopAllActions ();
		}
	}

	public string getCurrentActionName ()
	{
		if (playerActionSystemLocated) {
			return mainPlayerActionSystem.getCurrentActionName ();
		}

		return "";
	}

	public void setExtraCharacterVelocity (Vector3 extraValue)
	{
		if (!canRagdollMove) {
			extraValue = vector3Zero;
		}

		currentExtraCharacterVelocity = extraValue;

		addExtraCharacterVelocityActive = (extraValue != vector3Zero);
	}

	public void setActionCanHappenOnAirState (bool state)
	{
		actionCanHappenOnAir = state;
	}

	public void setActionActiveState (bool state)
	{
		actionActive = state;

		//If an action is over but some menu is opened when an action is over, make sure to disable the 
		//player movement once the action is over in case the menu is still opened
		if (!actionActive) {
			if (enableChangeScriptStateAfterFinishActionState) {
				if (!playerIsBusy ()) {
					changeScriptState (true);
				}

				enableChangeScriptStateAfterFinishActionState = false;
			}

			if (disableChangeScriptStateAfterFinishActionState) {
				changeScriptState (false);

				disableChangeScriptStateAfterFinishActionState = false;
			}

			lastTimeActionActive = Time.time;
		}
	}

	public bool isActionActive ()
	{
		return actionActive;
	}

	public string getCurrentActionCategoryActive ()
	{
		if (playerActionSystemLocated) {
			return mainPlayerActionSystem.getCurrentActionCategoryActive ();
		}

		return "";
	}

	public void setCurrentCustomActionCategoryID (int newID)
	{
		if (playerActionSystemLocated) {
			mainPlayerActionSystem.setCurrentCustomActionCategoryID (newID);
		}
	}

	public void setEnableChangeScriptStateAfterFinishActionState (bool state)
	{
		enableChangeScriptStateAfterFinishActionState = state;
	}

	public void setDisableChangeScriptStateAfterFinishActionState (bool state)
	{
		disableChangeScriptStateAfterFinishActionState = state;
	}

	public void setActionActiveWithMovementState (bool state)
	{
		actionActiveWithMovement = state;
	}

	public void setIgnoreCameraDirectionOnMovementState (bool state)
	{
		if (state) {
			lastForwardDirection = playerCameraTransform.forward;
			lastRigthDirection = mainCameraTransform.right;
		}

		ignoreCameraDirectionOnMovement = state;
	}


	public void setIgnoreCameraDirectionOnStrafeMovementState (bool state)
	{
		ignoreCameraDirectionOnStrafeMovement = state;
	}

	public void setUseRootMotionActiveState (bool state)
	{
		useRootMotionActive = state;
	}

	public void setOriginalUseRootMotionActiveState ()
	{
		setUseRootMotionActiveState (originalSetUseRootMotionActiveState);
	}

	public void setNoRootMotionValues (float noRootWalkMovementSpeedValue, float noRootRunMovementSpeedValue, float noRootSprintMovementSpeedValue, 
	                                   float noRootCrouchMovementSpeedValue, float noRootWalkStrafeMovementSpeedValue, float noRootRunStrafeMovementSpeedValue)
	{
		noRootWalkMovementSpeed = noRootWalkMovementSpeedValue;
		noRootRunMovementSpeed = noRootRunMovementSpeedValue;
		noRootSprintMovementSpeed = noRootSprintMovementSpeedValue;
		noRootCrouchMovementSpeed = noRootCrouchMovementSpeedValue;
		noRootWalkStrafeMovementSpeed = noRootWalkStrafeMovementSpeedValue;
		noRootRunStrafeMovementSpeed = noRootRunStrafeMovementSpeedValue;
	}

	public void setOriginalNoRootMotionValues ()
	{
		setNoRootMotionValues (originalNoRootWalkMovementSpeed, originalNoRootRunMovementSpeed, originalNoRootSprintMovementSpeed,
			originalNoRootCrouchMovementSpeed, originalNoRootWalkStrafeMovementSpeed, originalNoRootRunStrafeMovementSpeed);
	}

	public void setDeactivateRootMotionOnStrafeActiveOnLockedViewState (bool state)
	{
		deactivateRootMotionOnStrafeActiveOnLockedView = state;

		previousStrafeModeActive = false;

		if (deactivateRootMotionOnStrafeActiveOnLockedView) {
			if (strafeModeActive) {
				setUseRootMotionActiveState (false);
			}
		} else {
			setOriginalUseRootMotionActiveState ();
		}
	}

	public void setAllowDownVelocityDuringActionState (bool state)
	{
		allowDownVelocityDuringAction = state;
	}

	public void setPlayerControllerMovementValues (float moveSpeedMultiplierValue, float animSpeedMultiplierValue, float jumpPowerValue, float airSpeedValue, float airControlValue)
	{
		setMoveSpeedMultiplierValue (moveSpeedMultiplierValue);

		setAnimSpeedMultiplierValue (animSpeedMultiplierValue);

		setJumpPower (jumpPowerValue);

		airSpeed = airSpeedValue;
		airControl = airControlValue;

		noAnimatorAirSpeed = airSpeedValue;
	}

	public void resetPlayerControllerMovementValues ()
	{
		setMoveSpeedMultiplierValue (originalMoveSpeedMultiplier);

		setAnimSpeedMultiplierValue (originalAnimationSpeed);

		setOriginalJumpPower ();

		airSpeed = originalAirSpeed;
		airControl = originalAirControl;

		noAnimatorAirSpeed = originalNoAnimatorAirSpeed;
	}

	public void setAirSpeedValue (float newAirSpeedValue)
	{
		airSpeed = newAirSpeedValue;
	}

	public void setAirControlValue (float newAirControlValue)
	{
		airControl = newAirControlValue;
	}

	public void setOriginalAirSpeed ()
	{
		setAirSpeedValue (originalAirSpeed);
	}

	public void setOriginalAirControl ()
	{
		setAirControlValue (originalAirControl);
	}

	public void setIgnoreInputOnAirControlActiveState (bool state)
	{
		ignoreInputOnAirControlActive = state;
	}

	public void setAnimatorSpeedUpdatePausedState (bool state)
	{
		animatorSpeedUpdatePaused = state;
	}

	public void setOverrideAnimationSpeedActiveState (bool state)
	{
		overrideAnimationSpeedActive = state;
	}

	public void updateCurrentAnimatorSpeed (float newSpeed)
	{
		currentAnimatorSpeed = newSpeed;
	}

	public void setNoAnimatorMovementSpeedMultiplierValue (float newValue)
	{
		noAnimatorMovementSpeedMultiplier = newValue;
	}

	public void setOriginalNoAnimatorMovementSpeedMultiplierValue ()
	{
		setNoAnimatorMovementSpeedMultiplierValue (originalNoAnimatorMovementSpeedMultiplier);
	}

	public Transform getCurrentSurfaceBelowPlayer ()
	{
		return currentSurfaceBelowPlayer;
	}

	public bool checkIfPlayerOnGroundWithRaycast ()
	{
		if (playerTransform == null) {
			return false;
		}

		if (Physics.Raycast (playerTransform.position + playerTransform.up, -playerTransform.up, 1.3f, layer)) {
			return true;
		}

		return false;
	}

	public void setCheckOnGroungPausedState (bool state)
	{
		checkOnGroungPaused = state;
	}

	//check if the player is in the ground with a raycast
	void checkOnGround ()
	{
		if (checkOnGroungPaused) {
			currentSurfaceBelowPlayer = null;
			return;
		}

		if (pauseAllPlayerDownForces) {
			return;
		}

		if (!gravityPowerActive && !usingJetpack && !flyModeActive && !swimModeActive && !externalControlBehaviorForAirTypeActive) {

			currentPlayerPosition = playerTransform.position;
			currentPlayerUp = playerTransform.up;

			if (playerTransform.InverseTransformDirection (mainRigidbody.velocity).y < jumpPower * .5f) {
				previouslyOnGround = playerOnGround;

				setOnGroundState (false);

				if (jump || slowingFall) {
					if (!gravityForcePaused && !zeroGravityModeOn && !freeFloatingModeOn && !wallRunningActive && !pauseAllPlayerDownForces && !externalControlBehaviorForAirTypeActive) {
						Vector3 totalAirForce = gravityForce * mainRigidbody.mass * currentNormal;

						mainRigidbody.AddForce (totalAirForce);
					}
				}

				//check what it is under the player
				rayPos = currentPlayerPosition + currentPlayerUp;
				hitAngle = 0;
				hitPoint = vector3Zero;

				mainHitPoint = currentPlayerPosition;

				groundDetected = false;

//				bool groundDetectedBySphereCast = false;

//				if (useSphereRaycastForGroundDetection) {
//					sphereRaycastRay = new Ray (currentPlayerPosition - currentPlayerUp * sphereCastOffset, -currentPlayerUp);
//
//					if (Physics.SphereCast (sphereRaycastRay, sphereCastRadius, out hit, maxDistanceSphereCast, layer)) {
//						groundDetected = true;
//					}
//				} else if (useCaspsuleCastForGroundDetection) {
//					currentRayOriginPosition = currentPlayerPosition + currentPlayerUp * caspsuleCastOffset;
//					currentRayTargetPosition = currentPlayerPosition - currentPlayerUp * capsuleCastDistance;
//
//					distanceToTarget = GKC_Utils.distance (currentRayOriginPosition, currentRayTargetPosition);
//					rayDirection = currentRayOriginPosition - currentRayTargetPosition;
//					rayDirection = rayDirection / rayDirection.magnitude;
//
//					point1 = currentRayOriginPosition - rayDirection * capsuleCastRadius;
//					point2 = currentRayTargetPosition + rayDirection * capsuleCastRadius;
//
//					if (Physics.CapsuleCast (point1, point2, capsuleCastRadius, -rayDirection, out hit, maxDistanceSphereCast, layer)) {
//						groundDetected = true;
//					}
//				} else {
//					if (Physics.Raycast (rayPos, -currentPlayerUp, out hit, rayDistance, layer)) {
//						groundDetected = true;
//					}
//				}


				if (Physics.Raycast (rayPos, -currentPlayerUp, out hit, rayDistance, layer)) {
					groundDetected = true;
				}

				if (useSphereRaycastForGroundDetection) {
					if (!groundDetected) {
						if (Physics.Raycast (rayPos, -currentPlayerUp, out temporalHit, rayDistance + 1.5f, layer)) {
							float temporalHitDistance = temporalHit.distance;

							if (temporalHitDistance < rayDistance + 0.2f) {
								hit = temporalHit;

								groundDetected = true;
							}
						}
					}

					if (!groundDetected) {
						sphereRaycastRay = new Ray (currentPlayerPosition - currentPlayerUp * sphereCastOffset, -currentPlayerUp);

						if (Physics.SphereCast (sphereRaycastRay, sphereCastRadius, out sphereRaycastHit, maxDistanceSphereCast, layer)) {
							groundDetected = true;

							hit = sphereRaycastHit;

							if (showGizmo) {
								Debug.DrawRay (hit.point, Vector3.up * 100, Color.black, 10);
							}

							bool nextSurfaceLocated = false;

							Vector3 playerMovementDirection = mainRigidbody.velocity * 5;

							playerMovementDirection.Normalize ();

							Vector3 temporalRaycastPosition = currentPlayerPosition + playerMovementDirection * 0.05f + currentPlayerUp * 0.1f;

							int temporalRaycastCheckAmount = 0;

							while (!nextSurfaceLocated) {
								
								if (Physics.Raycast (temporalRaycastPosition, -currentPlayerUp, out sphereRaycastHit, 0.17f, layer)) {
									hit = sphereRaycastHit;

									nextSurfaceLocated = true;
								} else {
									temporalRaycastPosition += playerMovementDirection * 0.03f;

									temporalRaycastCheckAmount++;

									if (showGizmo) {
										Debug.DrawRay (temporalRaycastPosition, Vector3.up * 100, Color.yellow, 10);
									}
								}

								if (temporalRaycastCheckAmount >= 5) {
									nextSurfaceLocated = true;

									groundDetected = false;
								}
							}
						}
					}

					if (!groundDetected) {
						if (Physics.Raycast (rayPos, -currentPlayerUp, out temporalHit, rayDistance + 1.5f, layer)) {
							float temporalHitDistance = temporalHit.distance;

							if (previouslyOnGround) {
								if (temporalHitDistance < rayDistance + maxExtraRaycastDistanceToGroundDetection) {
									hit = temporalHit;

									groundDetected = true;
								}
							}
						}
					}
				}

				if (groundDetected) {
					//get the angle of the current surface
					hitAngle = Vector3.Angle (currentNormal, hit.normal);

//					print (hitAngle + " " + GKC_Utils.getAngle (currentNormal, hit.normal));

					canSetGroundState = false;
					//check max angle between the ground and the player if he is on zero gravity or free floating mode
					if ((freeFloatingModeOn || zeroGravityModeOn) && !checkOnGroundStatePausedFFOrZG && !previouslyOnGround) {
						if (useMaxAngleToCheckOnGroundStateZGFF) {
							if (hitAngle < maxAngleToChekOnGroundStateZGFF) {
								canSetGroundState = true;
							}
						} else {
							canSetGroundState = true;
						}
					} else {
						canSetGroundState = true;
					}

					if (canSetGroundState) {
						setOnGroundState (true);
					}

					jump = false;

					slowingFall = false;

					slowFallExternallyActive = false;

					hitPoint = hit.point;

					currentSurfaceBelowPlayer = hit.collider.transform;

					//					print (currentSurfaceBelowPlayer.name);

					currentNumberOfDash = 0;

					distanceToGround = hit.distance;

					if (!useSphereRaycastForGroundDetection && !useCaspsuleCastForGroundDetection) {
						distanceToGround -= 1;
					}

					mainHitPoint = hitPoint;
					mainHitAngle = hitAngle;
				} else {
					currentSurfaceBelowPlayer = null;
				}

				if (useMaxWalkSurfaceAngle && !stairStepDetected) {
					if (playerOnGround && hitAngle > maxWalkSurfaceAngle && (!useMaxDistanceToCheckSurfaceAngle || distanceToGround > maxDistanceToCheckSurfaceAngle)) {
						setOnGroundState (false);
					}
				}

				//check if the player has to adhere to the surface or not
				adhereToGround = false;
				if (playerOnGround) {

					movingOnSlopeUp = false;
					movingOnSlopeDown = false;

					//use a raycast to check the distance to the ground in front of the player, 
					//if the ray doesn't find a collider, it means that the player is going down in an inclinated surface, so adhere to that surface
					bool hitInFront = false;
					slopeFound = false;
					stairsFound = false;

					//assign the current input direction, taking into account if the player is moving in free mode or in locked camera with movement relative to the camera direction
					if (lockedCameraActive && useRelativeMovementToLockedCamera) {
						currentAdherenceInput = lockedCameraMoveInput;
					} else {
						currentAdherenceInput = currentMoveInput;
					}

					if (usedByAI) {
						if (Mathf.Abs (currentAdherenceInput.x) > 0.1f) {
							currentAdherenceInput.x = 1 * Mathf.Sign (currentAdherenceInput.x);
						}

						if (Mathf.Abs (currentAdherenceInput.y) > 0.1f) {
							currentAdherenceInput.y = 1 * Mathf.Sign (currentAdherenceInput.y);
						}
					}

					rayPosition = rayPos + (currentAdherenceInput * maxRayDistanceRange);

					//Debug.DrawRay (rayPosition - currentPlayerUp, currentPlayerUp * 10, Color.yellow);

					raycastDistance = rayDistance - 0.2f;

					if (walkSpeed <= half) {
						raycastDistance = rayDistance;
					}

					float hitAngleInFront = 0;

					if (Physics.Raycast (rayPosition, -currentPlayerUp, out hit, raycastDistance, layer)) {
						//Debug.DrawRay (rayPosition, -currentPlayerUp * raycastDistance, Color.red);
						hitInFront = true;

						hitAngleInFront = Vector3.Angle (currentNormal, hit.normal);
					}

					bool ignoringSlopesFromInclination = false;

					//check for slopes while aiming in third person or moving in first person
					if (hitAngle != 0 || hitAngleInFront != 0) {

						checkForSlopes = false;

						if (firstPersonActive) {
							rayPosition = rayPos + (currentAdherenceInput * maxSlopeRayDistance);

							checkForSlopes = true;
						} else {
							if (!playerCameraManager.is2_5ViewActive ()) {
								if ((playerIsAiming && lookInCameraDirectionOnFreeFireActive) || lookInCameraDirectionActive) {

									rayPosition = rayPos + (currentAdherenceInput * maxSlopeRayDistance);

									checkForSlopes = true;
								} else {
									if (useTankControls) {
										if (!isPlayerUsingVerticalInput ()) {
											currentAdherenceInput = vector3Zero;
										}

										rayPosition = rayPos + (currentAdherenceInput * maxSlopeRayDistance);
									} else {
										rayPosition = rayPos + (playerTransformForward * maxSlopeRayDistance);
									}

									checkForSlopes = true;
								}
							} else {
								if (playerIsAiming) {
									rayPosition = rayPos + (currentAdherenceInput * maxSlopeRayDistance);
								} else {
									rayPosition = rayPos + (playerTransformForward * maxSlopeRayDistance);
								}

								checkForSlopes = true;
							}
						}

						if (useMaxSlopeInclination) {
							if (hitAngle < minSlopeInclination || hitAngle > maxSlopeInclination ||
							    hitAngleInFront < minSlopeInclination || hitAngleInFront > maxSlopeInclination) {
								checkForSlopes = false;

								ignoringSlopesFromInclination = true;
							}
						}

						if (checkForSlopes) {
							if (showGizmo) {
								Debug.DrawRay (rayPosition + Vector3Up * 4, -currentPlayerUp * (rayDistance + half) - Vector3Up * 4, Color.green, 3);
							}

							if (Physics.Raycast (rayPosition, -currentPlayerUp, out adherenceHit, (rayDistance + half), layer)) {

								hitPointVerticalPosition = playerTransform.InverseTransformPoint (adherenceHit.point).y;

								mainHitPointVerticalPosition = playerTransform.InverseTransformPoint (mainHitPoint).y;

								adhereToGround = true;

								hitPoint = adherenceHit.point;

								if (usingAnimator && !useFirstPersonPhysicsInThirdPersonActive) {
									if (hitPointVerticalPosition >= mainHitPointVerticalPosition) {
										currentGroundAdherence = slopesGroundAdherenceUp;

										movingOnSlopeUp = true;
									} else {
										currentGroundAdherence = slopesGroundAdherenceDown;

										movingOnSlopeDown = true;
									}
								} else {
									if (hitPointVerticalPosition >= mainHitPointVerticalPosition) {
										currentGroundAdherence = noAnimatorSlopesGroundAdherenceUp;

										movingOnSlopeUp = true;
									} else {
										currentGroundAdherence = noAnimatorSlopesGroundAdherenceDown;

										movingOnSlopeDown = true;
									}
								}

//								if (adherenceHit.rigidbody != null) {
//									currentGroundAdherence = regularGroundAdherence;
//								}

								slopeFound = true;

								//Debug.DrawRay (rayPosition + Vector3Up * 4, -currentPlayerUp * (rayDistance + half) - Vector3Up * 4, Color.green, 3);
							} else {
								//Debug.DrawRay (rayPosition + Vector3Up * 4, -currentPlayerUp * (rayDistance + half) - Vector3Up * 4, Color.yellow, 3);
							}
						}
					}

					//check for stairs below and in front of the player
					stairsMoveInput = (1 / walkSpeed) * currentAdherenceInput;

					rayPosition = rayPos + stairsMoveInput;

					if (Physics.Raycast (rayPosition, -currentPlayerUp, out adherenceHit, raycastDistance, layer)) {

						hitAngle = Vector3.Angle (currentNormal, adherenceHit.normal);  

						//print (hitAngle);

						if (checkStairsWithInclination) {
							if (!stairStepDetected) {
								stairInclinationDetected = false;
							}

							if (hitAngle != 0 && hitAngle >= minStairInclination && hitAngle <= maxStairInclination) {
								stairInclinationDetected = true;
							}
						}
					}

					//walk on stairs
					if (hitInFront && (hitAngle == 0 || stairInclinationDetected) && !ignoringSlopesFromInclination) {

						if (!slopeFound || mainHitAngle == 0 || (mainHitAngle >= minStairInclination && mainHitAngle <= maxStairInclination)) {
							if (checkForStairAdherenceSystem) {
								rayPosition = rayPos + stairsMoveInput;

								//Debug.DrawRay (rayPosition, -currentPlayerUp * raycastDistance, Color.black);

								if (Physics.Raycast (rayPosition, -currentPlayerUp, out adherenceHit, raycastDistance, layer)) {
									currentDetectedSurface = adherenceHit.collider.gameObject;
								} else {
									currentDetectedSurface = null;
								}

								if (currentTargetDetected != currentDetectedSurface) {

									stairAdherenceSystemDetected = false;

									currentTargetDetected = currentDetectedSurface;

									if (currentTargetDetected != null) {
										stairAdherenceSystem currentStairAdherenceSystem = currentTargetDetected.GetComponent<stairAdherenceSystem> ();

										if (currentStairAdherenceSystem != null) {
											if (currentStairAdherenceSystem.modifyStairsValuesOnPlayer) {
												currentStairAdherenceSystem.setStairsValuesOnPlayer (this);
											}

											stairAdherenceSystemDetected = true;
										} else {
											currentStairAdherenceSystem = currentSurfaceBelowPlayer.GetComponent<stairAdherenceSystem> ();

											if (currentStairAdherenceSystem != null) {
												if (currentStairAdherenceSystem.modifyStairsValuesOnPlayer) {
													currentStairAdherenceSystem.setStairsValuesOnPlayer (this);
												}

												stairAdherenceSystemDetected = true;
											}
										}
									}
								}
							}

							if (stairAdherenceSystemDetected) {
								currentStairMaxValue = currentStairAdherenceSystemMaxValue;
								currentStairMinValue = currentStairAdherenceSystemMinValue;
								currentStairAdherenceValue = currentStairAdherenceSystemAdherenceValue;
							} else {
								currentStairMaxValue = stairsMaxValue;
								currentStairMinValue = stairsMinValue;
								currentStairAdherenceValue = stairsGroundAdherence;
							}

							rayPosition = rayPos + (stairsMoveInput * maxStairsRayDistance);

							Vector3 clampRayPosition = rayPosition - currentPlayerPosition;

							clampRayPosition = Vector3.ClampMagnitude (clampRayPosition, 1.1f);

							clampRayPosition = currentPlayerPosition + clampRayPosition;

							if (Physics.Raycast (clampRayPosition, -currentPlayerUp, out adherenceHit, raycastDistance, layer)) {
								if (!stairStepDetected) {
//									&& adherenceHit.rigidbody == null

									float stepPosition = playerTransform.InverseTransformPoint (adherenceHit.point).y;

									if (stepPosition > 0) {
										upDistance = Mathf.Abs (Mathf.Abs (playerTransform.InverseTransformPoint (currentPlayerPosition).y) - Mathf.Abs (stepPosition));

										if (upDistance >= currentStairMinValue && upDistance <= currentStairMaxValue) {
											stairStepDetected = true;	
											//print ("stair step detected");
										}
									}
								}

								if (stairStepDetected) {
									//									print ("climbing step");

									adhereToGround = true;
									hitPoint = adherenceHit.point;

									//Debug.DrawLine (currentPlayerPosition + currentPlayerUp * 2, hitPoint, Color.black, 2);

									if (usingAnimator && !useFirstPersonPhysicsInThirdPersonActive) {
										currentGroundAdherence = currentStairAdherenceValue;

										if (walkSpeed < 1) {
											float extraValue = walkSpeed;
											extraValue += 0.3f;
											extraValue = Mathf.Clamp (extraValue, 0, 1);
											currentGroundAdherence *= extraValue;
										}
									} else {
										currentGroundAdherence = noAnimatorStairsGroundAdherence;
									}

									stairsFound = true;

									//print (playerTransform.InverseTransformPoint (adherenceHit.point).y);

									if (Mathf.Abs (playerTransform.InverseTransformPoint (adherenceHit.point).y) < 0.02f) {
										stairStepDetected = false;

										//										print ("new step reached");
									}
								} 
							} else {
								stairStepDetected = false;
							}
						}
					}

					if (!slopeFound && !stairsFound) {
						adhereToGround = true;
						currentGroundAdherence = regularGroundAdherence;
					}

					//if the player is not moving and the angle of the surface is 0, adhere to it, so if the player jumps for example, the player is correctly
					//placed in the ground, with out a gap between the player's collider and the ground
					bool pressingInput = isPlayerUsingInput ();

					if (useTankControls) {
						pressingInput = isPlayerUsingVerticalInput ();
					}

					if (!pressingInput && hitAngle == 0) {
						adhereToGround = true;
						currentGroundAdherence = regularGroundAdherence;

						if (stairStepDetected) {
							hitPoint = currentPlayerPosition;
						}
					} else if (!pressingInput || (movingOnSlopeUp && hitAngle != 0 && !isPlayerMoving (0.99f)) || (movingOnSlopeDown && hitAngle != 0 && !isPlayerMoving (0.99f))) {
						adhereToGround = true;

						//print (movingOnSlopeUp + " " + movingOnSlopeDown);

						bool sphereCastFound = false;
						if (Physics.SphereCast (currentPlayerPosition + currentPlayerUp, originalRadius, -currentPlayerUp, out adherenceHit, 0.75f, layer)) {

							if (showGizmo) {
								Debug.DrawLine (currentPlayerPosition + currentPlayerUp, adherenceHit.point, Color.black);
							}

							sphereCastFound = true;
						}

						if (movingOnSlopeUp) {
							if (distanceToGround < 0.05f || sphereCastFound) {
								hitPoint = currentPlayerPosition;
							} else {
								hitPoint = mainHitPoint;
							}
						} else if (movingOnSlopeDown) {
							if (pressingInput) {

								if (!isPlayerMoving (0.25f) && (sphereCastFound || distanceToGround < 0.05f)) {
									hitPoint = currentPlayerPosition;
								}
							} else {
								if (distanceToGround < 0.05f || sphereCastFound) {
									if (!isPlayerMoving (0.05f)) {
										hitPoint = currentPlayerPosition;
									}
								} else {
									hitPoint = mainHitPoint;
								}
							}
						}
					}
				}

				if (showGizmo) {
					Debug.DrawLine (currentPlayerPosition + (currentPlayerUp * 4), adherenceHit.point, Color.black);
				}

				//the player has to ahdere to the current surface, so
				if (adhereToGround) {
					//move towards the surface the player's rigidbody 
					mainRigidbody.position = Vector3.MoveTowards (mainRigidbody.position, hitPoint, currentFixedUpdateDeltaTime * currentGroundAdherence);
				
					if (showGizmo) {
						Debug.DrawLine (mainRigidbody.position, hitPoint + playerTransform.up * 4, Color.black);
					}
				}

				if (playerOnGround) {
					if (useAutoCrouch) {
						if (!crouching) {
							if (!Physics.Raycast (autoCrouchRayPosition.position, playerTransformForward, raycastDistanceToAutoCrouch, layerToCrouch)) {

								if (showGizmo) {
									Debug.DrawRay (autoCrouchRayPosition.position, playerTransformForward * raycastDistanceToAutoCrouch, Color.white);
								}

								if (Physics.Raycast (autoCrouchRayPosition.position + secondRaycastOffset, playerTransformForward, raycastDistanceToAutoCrouch, layerToCrouch)) {

									if (showGizmo) {
										Debug.DrawRay (autoCrouchRayPosition.position + secondRaycastOffset, playerTransformForward * raycastDistanceToAutoCrouch, Color.red);
									}

									crouch ();
								} 
							} 
						}
					}
				}
			}

			if (!playerOnGround) {
				lastTimeAir = Time.time;
			}
		} else {
			setOnGroundState (false);
		}
	}

	public bool isPlayerOnGround ()
	{
		return playerOnGround;
	}

	public void setPlayerOnGroundState (bool state)
	{
		playerOnGround = state;
	}

	public void setOnGroundState (bool state)
	{
		playerOnGround = state;

		if (checkOnGroundStatePaused || checkOnGroundStatePausedFFOrZG) {
			playerOnGround = false;
		}
	}

	public bool isStrafeModeActive ()
	{
		return strafeModeActive;
	}

	public void updateStrafeModeActiveState ()
	{
		strafeModeActive = ((playerIsAiming && lookInCameraDirectionOnFreeFireActive) || lookInCameraDirectionActive) && playerOnGround;
	}

	public void updateWallRunningState ()
	{
		if (wallRunnigEnabled) {
			wallRunningExternalControllerBehavior.updateControllerBehavior ();
		}
	}

	public void checkIfActivateWallRunningState ()
	{
		if (wallRunnigEnabled) {
			wallRunningExternalControllerBehavior.checkIfActivateExternalForce ();
		}
	}

	public bool isWallRunningActive ()
	{
		return wallRunningActive;
	}

	public void setWallRunningActiveValue (bool state)
	{
		wallRunningActive = state;
	}

	public bool isExternalControlBehaviorForAirTypeActive ()
	{
		return externalControlBehaviorForAirTypeActive;
	}

	public void setWallRunningActiveState (bool state)
	{
		if (wallRunnigEnabled) {
			wallRunningExternalControllerBehavior.setExternalForceActiveState (state);
		}
	}

	public void setWallRunningImpulseForce (Vector3 forceAmount, bool useCameraDirection)
	{
		if (wallRunnigEnabled) {
			wallRunningExternalControllerBehavior.setExtraImpulseForce (forceAmount, useCameraDirection);
		}
	}

	public void setWallRunningEnabledState (bool state)
	{
		if (wallRunnigEnabled) {
			wallRunningExternalControllerBehavior.setExternalForceEnabledState (state);
		}
	}

	public void setPauseAllPlayerDownForces (bool state)
	{
		pauseAllPlayerDownForces = state;
	}

	public void setLadderFoundState (bool state)
	{
		ladderFound = state;

		if (ladderFound) {
			if (wallRunningActive) {
				setWallRunningActiveState (false);
			}
		} else {
			jumpInput = false;
		}
	}

	public bool isLadderFound ()
	{
		return ladderFound;
	}

	public bool isPauseAllPlayerDownForcesActive ()
	{
		return pauseAllPlayerDownForces;
	}

	public void setAirDashEnabledState (bool state)
	{
		airDashEnabled = state;
	}

	public void doAirDash ()
	{
		if (characterRotatingToSurface) {
			return;
		}

		if (ignoreExternalActionsActiveState) {
			return;
		}

		if (useDashLimit) {
			if (currentNumberOfDash >= dashLimit) {
				return;
			}
		}

		if (airDashEnabled && Time.time > lastTimeDash + airDashColdDown) {

			Vector3 dashDirection = currentMoveInput;

			if (dashDirection == vector3Zero || dashDirection.magnitude < 0.1f) {
				if (firstPersonActive) {
					dashDirection = playerCameraTransform.forward;
				} else {
					dashDirection = playerTransformForward;
				}
			}

			if (resetGravityForceOnDash) {
				mainRigidbody.velocity = vector3Zero;
			}

			mainRigidbody.AddForce (mainRigidbody.mass * airDashForce * dashDirection, ForceMode.Impulse);

			lastTimeDash = Time.time;

			if (pauseGravityForce) {
				setGravityForcePuase (true);

				lastTimeGravityForcePaused = Time.time;

				unPauseGravityForceActive = true;
			}

			if (useDashLimit) {
				currentNumberOfDash++;
			}

			if (changeCameraFovOnDash) {
				playerCameraManager.setMainCameraFovStartAndEnd (cameraFovOnDash, playerCameraManager.getOriginalCameraFov (), cameraFovOnDashSpeed);
			}

			if (useEventsOnAirDash) {
				if (firstPersonActive) {
					eventOnAirDashFirstPerson.Invoke ();
				} else {
					eventOnAirDashThirdPerson.Invoke ();
				}
			}
		}
	}

	public void addExternalForce (Vector3 forceDireciton)
	{
		mainRigidbody.AddForce (forceDireciton * mainRigidbody.mass, ForceMode.Impulse);
	}

	public void setGravityForcePuase (bool state)
	{
		gravityForcePaused = state;
	}

	public bool isGravityForcePaused ()
	{
		return gravityForcePaused;
	}

	public void setRigidbodyVelocityToZero ()
	{
		mainRigidbody.velocity = vector3Zero;
	}

	public float getCurrentDeltaTime ()
	{
		currentDeltaTime = Time.deltaTime;

		if (regularMovementOnBulletTime) {
			currentDeltaTime *= GKC_Utils.getCurrentDeltaTime ();
		}

		return currentDeltaTime;
	}

	public float getCurrentScaleTime ()
	{
		return GKC_Utils.getCurrentDeltaTime ();
	}

	//set the scale of the capsule if the player is crouched
	void scaleCapsule (bool state)
	{
		if (state) {
			capsule.height = capsuleHeightOnCrouch;
			capsule.center = Vector3Up * capsuleHeightOnCrouch / 2;
		} else {
			capsule.height = originalHeight;
			capsule.center = Vector3Up * (originalHeight * half);
		}

		currentHeight = capsule.height;
	}

	public void setPlayerColliderCapsuleScale (float capsuleHeight)
	{
		capsule.height = capsuleHeight;
		capsule.center = Vector3Up * capsuleHeight / 2;

		currentHeight = capsule.height;
	}

	public void setPlayerColliderCapsuleCenter (Vector3 newValue)
	{
		capsule.center = newValue;
	}

	public void setPlayerCapsuleColliderDirection (int newDirection)
	{
		capsule.direction = newDirection;
	}

	public void setPlayerCapsuleColliderRadius (float newValue)
	{
		capsule.radius = newValue;
	}

	public void setOriginalPlayerCapsuleColliderRadius ()
	{
		setPlayerCapsuleColliderRadius (originalRadius);
	}

	public void setOriginalPlayerColliderCapsuleScale ()
	{
		scaleCapsule (false);
	}

	public void setOriginalPlayerColliderCapsuleScaleIfCrouchNotActive ()
	{
		if (!crouching) {
			scaleCapsule (false);
		} else {
			healthManager.changePlaceToShootPosition (true);
		}
	}

	public void setCharacterRadius (float newValue)
	{
		characterRadius = newValue;
	}

	public float getCharacterRadius ()
	{
		return characterRadius;
	}

	public void setPlaceToShootPositionOffset (float newValue)
	{
		if (newValue > 0) {
			healthManager.setPlaceToShootPositionOffset (newValue);
		} else {
			healthManager.changePlaceToShootPosition (false);
		}
	}

	public void setCheckFallStatePausedState (bool state)
	{
		checkFallStatePaused = state;
	}

	public Collider getMainCollider ()
	{
		return mainCollider;
	}

	public void setMainColliderState (bool state)
	{
		if (!canRagdollMove && state) {
			return;
		}

		mainCollider.enabled = state;

//		print ("main collider state " + state);
	}

	public void setMainColliderTriggerState (bool state)
	{
		mainCollider.isTrigger = state;
	}

	public bool isMainColliderEnabled ()
	{
		return mainCollider.enabled;
	}

	public void reactivateColliderIfPossible ()
	{
		if (mainCollider.enabled) {
			setMainColliderState (false);

			setMainColliderState (true);
		}
	}

	public void setGravityMultiplierValue (bool setOriginal, float newValue)
	{
		if (setOriginal) {
			currentGravityMultiplier = originalGravityMultiplier;

			gravityMultiplier = originalGravityMultiplier;
		} else {
			currentGravityMultiplier = newValue;

			gravityMultiplier = newValue;
		}

		//		print (gravityMultiplier);
	}

	public void setGravityMultiplierValueFromExternalFunction (float newValue)
	{
		gravityMultiplier = newValue;
	}

	public float getGravityMultiplier ()
	{
		return gravityMultiplier;
	}

	public void setGravityForceValue (bool setOriginal, float newValue)
	{
		if (setOriginal) {
			gravityForce = originalGravityForce;
		} else {
			gravityForce = newValue;
		}
	}

	public void setOriginalGravityForceValue ()
	{
		setGravityForceValue (true, 0);
	}

	public float getGravityForce ()
	{
		return gravityForce;
	}

	public void setSlowFallExternallyActiveState (bool state)
	{
		slowFallExternallyActive = state;
	}

	public bool isSlowFallExternallyActive ()
	{
		return slowFallExternallyActive;
	}

	//CALL INPUT FUNCTIONS
	public bool canUseInput ()
	{
		if (jetPackEquiped) {
			return false;
		}

		if (flyModeActive) {
			return false;
		}

		if (!canMove) {
			return false;
		}

		if (usedByAI) {
			return false;
		}

		if (playerNavMeshEnabled) {
			return false;
		}

		if (driving) {
			return false;
		}

		if (swimModeActive) {
			return false;
		}

		if (closeCombatAttackInProcess) {
			return false;
		}

		return true;
	}

	public void inputCrouch ()
	{
		if (!playerActionsInputEnabled) {
			return;
		}

		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (isUsingSubMenu () || isPlayerMenuActive ()) {
			return;
		}

		if (!playerCameraManager.isFirstPersonActive ()) {
			if (weaponsManagerLocated && weaponsManager.isUsingWeapons ()) {
				if (weaponsManager.checkIfWeaponStateIsBusy () && !crouching) {
					return;
				}
			}
		}

		//check if the crouch button has been pressed
		crouch ();
	}

	public void inputJump ()
	{
		if (!playerActionsInputEnabled) {
			return;
		}

		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (!canJumpActive) {
			return;
		}

		if (jumpInputPaused) {
			return;
		}

		activateJump ();
	}

	public void inputReleaseJumpButton ()
	{
		if (!playerActionsInputEnabled) {
			return;
		}

		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (!canJumpActive) {
			return;
		}

		releaseJumpButton ();
	}

	public void setJumpsAmountValue (int newValue)
	{
		jumpsAmount = newValue;
	}

	public void activateJump ()
	{			
		setJumpActive (false);
	}

	public void setJumpActive (bool activateJumpOnAir)
	{
		if (wallRunningActive) {
			if (wallRunnigEnabled) {
				wallRunningExternalControllerBehavior.setJumpActiveForExternalForce ();
			}

			return;
		}

		if (externalControllBehaviorActive) {
			if (currentExternalControllerBehavior.externalControllerJumpEnabled &&
			    currentExternalControllerBehavior.behaviorCurrentlyActive) {
				currentExternalControllerBehavior.setJumpActiveForExternalForce ();

				return;
			}
		}

		if (enabledRegularJump) {
			jumpInput = true;

			if (ladderFound || activateJumpOnAir) {
				jumpInput = false;
			}

			if (!playerOnGround && !gravityPowerActive) {
				if ((ladderFound || enabledDoubleJump) && !zeroGravityModeOn && !freeFloatingModeOn) {
					//then check the last time the jump button has been pressed, so the player can jump again  
					//jump again
					if (Time.time > lastJumpTime + readyToDoubleJumpTime && (jumpsAmount < maxNumberJumpsInAir || ladderFound)) {
						doubleJump = true;
					}
				}
			}

			if (activateJumpOnAir) {
				doubleJump = true;
			}

			if (addJumpForceWhileButtonPressed) {
				bool canActivateJumpForceOnButtonPressed = false;

				if (playerOnGround) {
					canActivateJumpForceOnButtonPressed = true;
				} else {
					if (useJumpForceOnAirJumpEnabled) {
						canActivateJumpForceOnButtonPressed = true;
					}
				}

				if (canActivateJumpForceOnButtonPressed) {
					lastTimeJumpButtonHoldActive = Time.time;

					jumpButtonHoldActive = true;
				}
			}
		}
	}

	public void releaseJumpButton ()
	{
		if (addJumpForceWhileButtonPressed) {
			jumpButtonHoldActive = false;
		}
	}

	public void inputStartSlowDownFall ()
	{
		if (!playerActionsInputEnabled) {
			return;
		}

		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (holdJumpSlowDownFallEnabled) {
			if (Time.time > waitTimeToSlowDown) {

				gravityMultiplier = slowDownGravityMultiplier;

				if (!slowingFall) {
					slowingFallInput = true;
				}
			}
		}
	}

	public void inputStopSlowDownFall ()
	{
		if (!playerActionsInputEnabled) {
			return;
		}

		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (holdJumpSlowDownFallEnabled) {
			gravityMultiplier = currentGravityMultiplier;
		}
	}

	public void inputAirDash ()
	{
		if (!playerActionsInputEnabled) {
			return;
		}

		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (airDashEnabled && !gravityPowerActive && !playerOnGround && !gravityManager.isSearchingSurface () && !usingAbilityActive && canMove) {
			doAirDash ();
		}
	}

	public void inputSetMoveVerticallyState (bool state)
	{
		if (!playerActionsInputEnabled) {
			return;
		}

		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (zeroGravityModeOn || freeFloatingModeOn) {
			if (canMoveVertically && isPlayerMovingOn3dWorld ()) {
				movingVertically = state;
			}
		}
	}

	public void inputSetChangeZeroGravityMovementSpeeState (bool state)
	{
		if (!playerActionsInputEnabled) {
			return;
		}

		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (zeroGravityModeOn) {
			changeZeroGravityMovementVelocity (state);
		}
	}

	public void inputSetChangeFreeFloatingMovementSpeeState (bool state)
	{
		if (!playerActionsInputEnabled) {
			return;
		}

		if (!inputCanBeUsed || !enabled) {
			return;
		}

		if (freeFloatingModeOn) {
			changeFreeFloatingMovementVelocity (state);
		}
	}

	public void inputSetIncreaseMovementSpeedState (bool state)
	{
		if (!playerActionsInputEnabled) {
			return;
		}

		if (jetPackEquiped || flyModeActive || swimModeActive || !canMove || usedByAI || playerNavMeshEnabled || driving) {
			return;
		}

		if (!enabled) {
			return;
		}

		if (runInputPaused) {
			return;
		}

		if (increaseWalkSpeedEnabled) {
			if (holdButtonToKeepIncreasedWalkSpeed) {
				setIncreaseMovementSpeedState (state);
			} else {
				if (state) {
					increaseWalkSpeedActive = !increaseWalkSpeedActive;

					setIncreaseMovementSpeedState (increaseWalkSpeedActive);
				}
			}
		}
	}

	public void inputStartToRun ()
	{
		if (runInputPaused) {
			return;
		}

		//check if the player is moving and he is not using the gravity power
		//in that case according to the duration of the press key, the player will only run or run and change his gravity
		//also in the new version of the asset also check if the touch control is being used
		if (!playerIsBusy () && canMove && isSprintEnabled ()) {
			checkIfCanRun ();
		}
	}

	public void inputStopToRun ()
	{
		//if the run button is released, stop the run power, if the power was holding the run button to adhere to other surfaces, else the 
		//run button has to be pressed again to stop the run power
		if (!playerIsBusy () && canMove) {
			checkIfStopRun ();
		}
	}

	public void inputToggleWalkAndRun ()
	{
		if (runInputPaused) {
			return;
		}

		if (!playerIsBusy () && canMove) {
			toggleWalkRunState = !toggleWalkRunState;

			if (toggleWalkRunState) {
				previousWalkSpeedValue = originalWalkSpeedValue;

				increaseWalkSpeedValue = 1;

				originalWalkSpeedValue = half;

				setWalkBydefaultFromEditor ();
			} else {
				originalWalkSpeedValue = previousWalkSpeedValue;

				setWalkSpeedValue (originalWalkSpeedValue);

				setSprintEnabledState (originalSprintEnabledValue);

				increaseWalkSpeedValue = 1;
			}
		}
	}

	//END OF INPUT CALL FUNCTIONS

	public void checkIfCanRun ()
	{
		bool canActivateRunResult = false;

		if (getMoveInputDirection ().magnitude > 0 &&
		    !isGravityPowerActive () &&
		    (isPlayerOnGround () || wallRunningActive || externalControlBehaviorForAirTypeActive) &&
		    !gravityManager.isSearchingSurface ()) {

			canActivateRunResult = true;
		}

		if (running) {
			canActivateRunResult = true;
		}

		if (canActivateRunResult) {
			//avoid to run backwards
			if (playerCanRunNow ()) {
				//check if he can run while crouching
				if (!isCrouching () || (runOnCrouchEnabled && isCrouching ())) {
					//check the amount of time that the button is being pressed
					runButtonPressedTimer += Time.deltaTime;

					if (!running) {
						activateRun ();

						lastTimeRun = runButtonPressedTimer;
					}
				}
			}
		}
	}

	public void activateRun ()
	{
		lastTimeMovingOnRun = Time.time;

		setRunnningState (true);

		if (weaponsManagerLocated) {
			weaponsManager.setRunningState (true);
		}
	}

	public void checkIfStopRun ()
	{
		if (running && (runButtonPressedTimer > 0.5f || runButtonPressedTimer - lastTimeRun > 0.12f)) {
			forceStopRun ();
		}
	}

	public void forceStopRun ()
	{
		stopRun ();

		runButtonPressedTimer = 0;

		lastTimeRun = 0;
	}

	public void stopRun ()
	{
		setRunnningState (false);

		if (weaponsManagerLocated) {
			weaponsManager.setRunningState (false);
		}
	}

	public float getRunButtonPressedTimer ()
	{
		return runButtonPressedTimer;
	}

	public bool playerIsBusy ()
	{
		if (isUsingDevice ()) {
			return true;
		}

		if (isUsingSubMenu ()) {
			return true;
		}

		if (isPlayerMenuActive ()) {
			return true;
		}

		return false;
	}

	public void setSlidindOnThirdPersonActiveState (bool state)
	{
		slidindOnThirdPersonActive = state;
	}

	//check with a sphere cast if the there are any surface too close
	public void crouch ()
	{
		if (slidindOnThirdPersonActive) {
			if (useCrouchSlidingOnThirdPersonEnabled) {

				slidindOnThirdPersonActive = false;

				eventOnCrouchSlidingThirdPersonEnd.Invoke ();

				return;
			}
		}

		if (!gravityPowerActive && checkWeaponsState () && playerOnGround) {
			if (crouchSlidingEnabled) {
				if (!firstPersonActive) {
					if (!crouching) {
						
						if (running) {
							if (useCrouchSlidingOnThirdPersonEnabled && !slidindOnThirdPersonActive) {

								slidindOnThirdPersonActive = true;

								eventOnCrouchSlidingThirdPersonStart.Invoke ();
					
								return;
							}
						}
					}
				}
			}

			crouching = !crouching;

			if (!crouching) {
				//check if there is anything above the player when he is crouched, to prevent he stand up
				//stop the player to get up
				if (!playerCanGetUpFromCrouch ()) {
					crouching = true;
				}
			}

			if (!firstPersonActive) {
				if (weaponsManagerLocated) {
					weaponsManager.enableOrDisableIKOnHands (!crouching);
				}
			}

			//set the pivot position
			setCrouchComponentsState (crouching);

			if (crouchSlidingEnabled && running) {
				if (firstPersonActive) {
					if (crouching) {
						crouchSlidingActive = true;

						lastTimeCrouchSlidingActive = Time.time;

						eventOnCrouchSlidingStart.Invoke ();
					} else {
						stopCrouchSliding ();
					}
				}
			}
		} else if (crouchSlidingOnAirEnabled) {
			if (!gravityPowerActive && checkWeaponsState () && !playerOnGround) {
				if (crouchSlidingEnabled && running) {

					if (firstPersonActive) {

						crouchingOnAir = !crouchingOnAir;

						if (crouchingOnAir) {
							setCrouchComponentsState (true);

							crouchSlidingOnAirActive = true;

							crouchSlidingActive = true;

							lastTimeCrouchSlidingActive = Time.time;

							eventOnCrouchSlidingStart.Invoke ();
						} else {
							stopCrouchSliding ();
						}
					}
				}
			}
		}
	}

	public void checkIfActivateCrouchState ()
	{
		if (!playerCanGetUpFromCrouch ()) {
			crouch ();
		}
	}

	public void setCrouchState (bool state)
	{
		if (crouching) {
			if (!state) {
				crouch ();
			}
		} else {
			if (state) {
				crouch ();
			}
		}
	}

	public void setCrouchComponentsState (bool state)
	{
		playerCameraManager.crouch (state);

		healthManager.changePlaceToShootPosition (state);

		scaleCapsule (state);

		setLastTimeMoved ();
	}

	public void stopCrouchSliding ()
	{
		if (crouchSlidingActive || crouchingOnAir) {
			eventOnCrouchSlidingEnd.Invoke ();

			crouchSlidingActive = false;

			if (crouchingOnAir && playerCanGetUpFromCrouch () && !crouching) {
				setCrouchComponentsState (false);
			}

			crouchingOnAir = false;

			crouchSlidingOnAirActive = false;
		}
	}

	public void setCrouchSlidingEnabledState (bool state)
	{
		crouchSlidingEnabled = state;
	}

	public bool playerCanGetUpFromCrouch ()
	{
		crouchRay = new Ray (mainRigidbody.position + playerTransformUp * (originalRadius * half), playerTransformUp);

		float crouchRayLength = originalHeight - originalRadius * half;

		if (Physics.SphereCast (crouchRay, originalRadius * half, crouchRayLength, layer)) {
			return false;
		}

		return true;
	}

	public bool isCrouching ()
	{
		return crouching;
	}

	public void setChoosingGravityDirectionState (bool state)
	{
		choosingGravityDirection = state;
	}

	public void setGravityPowerActiveState (bool state)
	{
		gravityPowerActive = state;
	}

	public bool isGravityPowerActive ()
	{
		return gravityPowerActive;
	}

	public bool isChoosingGravityDirection ()
	{
		return	choosingGravityDirection;
	}

	public void setUsingCloseCombatActiveState (bool state)
	{
		usingCloseCombatActive = state;
	}

	public bool isUsingCloseCombatActive ()
	{
		return usingCloseCombatActive;
	}

	public bool iscloseCombatAttackInProcess ()
	{
		return closeCombatAttackInProcess;
	}

	public void setCloseCombatAttackInProcessState (bool state)
	{
		closeCombatAttackInProcess = state;
	}

	public void setJumpLegExternallyActiveState (bool state)
	{
		jumpLegExternallyActiveState = state;
	}

	public void setIgnoreExternalActionsActiveState (bool state)
	{
		ignoreExternalActionsActiveState = state;
	}

	public bool isIgnoreExternalActionsActiveState ()
	{
		return ignoreExternalActionsActiveState;
	}

	public void setRunnningState (bool state)
	{
		running = state;

		if (useEventOnSprint) {
			if (running) {
				eventOnStartSprint.Invoke ();
			} else {
				eventOnStopSprint.Invoke ();
			}
		}

		if (useSecondarySprintValues) {
			if (running) {
				setPlayerControllerMovementValues (sprintVelocity, sprintVelocity, sprintJumpPower, sprintAirSpeed, sprintAirControl);
			} else {
				resetPlayerControllerMovementValues ();
			}
		}

		if (sprintEnabled && sprintCanBeUsed) {

			if (lockedCameraActive) {
				setHeadbodStatesPaused = false;
			} else {
				if (isPlayerOnFirstPerson ()) {

					if (updateHeadbobState) {
						if (setHeadbodStatesPaused) {
							setHeadbodStatesPaused = false;

							resetHeadBodState ();

							playerCameraManager.setShakeCameraState (false, "");
						}
					}
				} else {
					if (changeCameraFovOnSprint) {
						changeSprintFovThirdPerson (running);
					}

					if (updateHeadbobState) {
						if (shakeCameraOnSprintThirdPerson) {
							setHeadbodStatesPaused = running;

							playerCameraManager.setShakeCameraState (running, sprintThirdPersonCameraShakeName);

							resetHeadBodState ();
						}
					}
				}
			}
		}

		stopCrouchSliding ();

		if (updateFootStepStateActive) {
			setCurrentFootStepsState ();
		}
	}

	public bool isPlayerRunning ()
	{
		return running;
	}

	public void setSprintEnabledState (bool state)
	{
		sprintEnabled = state;
	}

	public void setSprintCanBeUsedState (bool state)
	{
		sprintCanBeUsed = state;
	}

	public void stopSprint ()
	{
		stopRun ();
	}

	public bool isSprintEnabled ()
	{
		return sprintEnabled && sprintCanBeUsed;
	}

	public void setOriginalSprintEnabledValue ()
	{
		if (!originalSprintEnabledValue) {
			stopSprint ();
		}

		setSprintEnabledState (originalSprintEnabledValue);
	}

	public void setWalkBydefaultFromEditor ()
	{
		setWalkSpeedValue (half);

		setSprintEnabledState (false);
	}

	public void setRunAndWalkToggleBydefaultFromEditor ()
	{
		increaseWalkSpeedValue = half;

		setSprintEnabledState (false);
	}

	public void setStrafeModeFromEditor (bool state)
	{
		//		print ("activate strafe mode");
		setLookAlwaysInCameraDirectionState (state);

		setLookOnlyIfMovingState (false);

		updateComponent ();
	}

	public void enableOrDisableSphereMode (bool state)
	{
		if (canUseSphereMode) {
			sphereModeActive = state;
		}
	}

	public bool isSphereModeActive ()
	{
		return sphereModeActive;
	}

	public void setLastTimeMoved ()
	{
		lastTimeMoved = Time.time;
	}

	public void setLastTimeFalling ()
	{
		lastTimeFalling = Time.time;
	}

	public float getLastTimeFalling ()
	{
		return lastTimeFalling;
	}

	public float getLastJumpTime ()
	{
		return lastJumpTime;
	}

	public float getLastDoubleJumpTime ()
	{
		return lastDoubleJumpTime;
	}

	public void setLastTimeFiring ()
	{
		lastTimeFiring = Time.time;
	}

	public float getLastTimeFiring ()
	{
		return lastTimeFiring;
	}

	public void setAimingPowersState (bool state)
	{
		aimingPowers = state;
	}

	public void setUsingPowersState (bool state)
	{
		usingPowers = state;
	}

	public bool checkWeaponsState ()
	{
		if (playerCameraManager.isFirstPersonActive ()) {
			return true;
		} 

		if (aimingPowers) { 
			return false;
		}

		if (weaponsManagerLocated) {
			if (weaponsManager.isAimingWeapons ()) {
				return false;
			}

			if (!canCrouchWhenUsingWeaponsOnThirdPerson && weaponsManager.isUsingWeapons ()) {
				return false;
			}
		}

		if (crouching) {
			return true;
		}

		return true;
	}

	public void setUsingFreeFireModeState (bool state)
	{
		usingFreeFireMode = state;
	}

	public bool isUsingFreeFireMode ()
	{
		return usingFreeFireMode;
	}

	//set if the animator is enabled or not according to if the usingAnimatorInFirstMode is true or false
	public void checkAnimatorIsEnabled (bool state)
	{
		//if the animator in first person is disabled, then
		if (!usingAnimatorInFirstMode) {
			//the first person mode is enabled, so disable the animator
			if (state) {
				animator.enabled = false;
			}
			//the third person mode is enabled, so enable the animator
			else {
				animator.enabled = true;
			}
		}

		//if the animator is enabled, 
		if (animator.enabled) {
			//check the state of the animator to set the values in the mecanim
			usingAnimator = true;
		} else {
			//disable the functions that set the values in the mecanim, and apply force to the player rigidbody directly, instead of using the animation motion
			usingAnimator = false;
		}

		if (updateFootStepStateActive) {
			//change the type of footsteps, with the triggers in the feet of the player or the raycast checking the surface under the player
			stepManager.changeFootStespType (usingAnimator);
		}
	}

	public void setUseFirstPersonPhysicsInThirdPersonActiveState (bool state)
	{
		useFirstPersonPhysicsInThirdPersonActive = state;
	}

	public void setFootStepManagerState (bool state)
	{
		if (updateFootStepStateActive) {
			if (state) {
				stepManager.enableOrDisableFootStepsComponents (false);
			} else {
				stepManager.enableOrDisableFootStepsWithDelay (true, 0);
			}
		}
	}

	public void setCurrentFootStepsState ()
	{
		if (updateFootStepStateActive) {
			if (!stepManager.isDelayToNewStateActive ()) {
				if (running) {
					if (crouching) {
						if (currentFootStepState != footStepState.Run_Crouching) {
							stepManager.setFootStepState ("Run Crouching");

							currentFootStepState = footStepState.Run_Crouching;
						}
					} else {
						if (currentFootStepState != footStepState.Running) {
							stepManager.setFootStepState ("Running");

							currentFootStepState = footStepState.Running;
						}
					}
				} else {
					if (crouching) {
						if (currentFootStepState != footStepState.Crouching) {
							stepManager.setFootStepState ("Crouching");		

							currentFootStepState = footStepState.Crouching;
						}
					} else {
						if (currentFootStepState != footStepState.Walking) {
							stepManager.setFootStepState ("Walking");

							currentFootStepState = footStepState.Walking;
						}
					}
				}
			}
		}
	}

	public void setCurrentHeadBobState ()
	{
		if (isMoving && !obstacleToAvoidMovementFound) {
			if (running) {
				if (crouching) {
					if (currentHeadbobState != headbobState.Run_Crouching) {
						headBobManager.setState ("Run Crouching");

						currentHeadbobState = headbobState.Run_Crouching;
					}
				} else {
					if (currentHeadbobState != headbobState.Running) {
						headBobManager.setState ("Running");

						currentHeadbobState = headbobState.Running;
					}
				}
			} else {
				if (crouching) {
					if (currentHeadbobState != headbobState.Crouching) {
						headBobManager.setState ("Crouching");

						currentHeadbobState = headbobState.Crouching;
					}
				} else {
					if (currentHeadbobState != headbobState.Walking) {
						headBobManager.setState ("Walking");

						currentHeadbobState = headbobState.Walking;
					}
				}
			}
				
			if (headBobManager.useDynamicIdle) {
				setLastTimeMoved ();
			}
		} else {
			if (headBobManager.useDynamicIdle && canMove && !usingDevice && firstPersonActive && !headBobManager.externalShakingActive) {
				float timeToActiveDynamicIdle = headBobManager.timeToActiveDynamicIdle;

				if (Time.time > lastTimeMoved + timeToActiveDynamicIdle &&
				    Time.time > playerCameraManager.getLastTimeMoved () + timeToActiveDynamicIdle &&
				    Time.time > lastTimeFiring + timeToActiveDynamicIdle &&
				    !headBobPaused) {

					if (currentHeadbobState != headbobState.Dynamic_Idle) {
						headBobManager.setState ("Dynamic Idle");

						currentHeadbobState = headbobState.Dynamic_Idle;
					}
				} else {
					if (currentHeadbobState != headbobState.Static_Idle) {
						headBobManager.setState ("Static Idle");

						currentHeadbobState = headbobState.Static_Idle;
					}
				}
			} else {
				if (currentHeadbobState != headbobState.Static_Idle) {
					headBobManager.setState ("Static Idle");

					currentHeadbobState = headbobState.Static_Idle;
				}
			}
		}
	}

	public void resetHeadBodState ()
	{
		currentHeadbobState = headbobState.None;
	}

	//if the vehicle driven by the player ejects him, add force to his rigidbody
	public void ejectPlayerFromVehicle (float force)
	{
		jumpInput = true;

		mainRigidbody.AddForce (currentNormal * force, ForceMode.Impulse);
	}

	//use a jump platform
	public void useJumpPlatform (Vector3 direction, ForceMode forceMode)
	{
		jumpInput = true;

		if (forceMode == ForceMode.VelocityChange) {
			mainRigidbody.velocity = vector3Zero;
			currentVelocity = vector3Zero;
		}

		mainRigidbody.AddForce (direction, forceMode);
	}

	public void useJumpPlatformWithKeyButton (bool state, float newJumpPower)
	{
		if (state) {
			setJumpPower (newJumpPower);
		} else {
			setOriginalJumpPower ();
		}
	}

	public void setJumpPower (float newValue)
	{
		jumpPower = newValue;
	}

	public void setOriginalJumpPower ()
	{
		setJumpPower (originalJumpPower);
	}

	public Vector3 getCurrentNormal ()
	{
		return gravityManager.getCurrentNormal ();
	}

	public void externalForce (Vector3 direction)
	{
		externalForceActive = true;
		externalForceValue = direction;
	}

	public void setExternalForceOnAir (Vector3 direction, float externalForceOnAirControl)
	{
		externalForceOnAirActive = true;
		externalForceOnAirValue = direction;
		externalForceOnAirControlValue = externalForceOnAirControl;
	}

	public void changeHeadScale (bool state)
	{
		if (drivingRemotely) {
			return;
		}

		headScaleChanged = state;

		if (head != null) {
			if (state) {
				head.localScale = vector3Zero;
			} else {
				head.localScale = Vector3.one;
			}
		}
	}

	public bool isHeadScaleChanged ()
	{
		return headScaleChanged;
	}

	//if the player is driving, set to 0 his movement values and disable the player controller component
	public void setDrivingState (bool state, GameObject vehicle, string vehicleName)
	{
		driving = state;

		if (driving) {
			currentVehicle = vehicle;
			currentVehicleName = vehicleName;
		} else {
			currentVehicle = null;
			currentVehicleName = "";
		}

		if (usingAnimator) {
			if (customCharacterControllerActive) {

			} else {
				animator.SetFloat (forwardAnimatorID, 0);
				animator.SetFloat (turnAnimatorID, 0);
			}
		}

		playerCameraManager.setDrivingState (driving);
	}

	public void setTemporalVehicle (GameObject newVehicle)
	{
		currentTemporaVehicle = newVehicle;
	}

	public GameObject getTemporalVehicle ()
	{
		return currentTemporaVehicle;
	}

	public bool isPlayerRotatingToSurface ()
	{
		return characterRotatingToSurface;
	}

	public void setCharacterRotatingToSurfaceState (bool state)
	{
		characterRotatingToSurface = state;
	}

	public bool isPlayerDriving ()
	{
		return driving;
	}

	public bool isDrivingRemotely ()
	{
		return drivingRemotely;
	}

	public void setDrivingRemotelyState (bool state)
	{
		drivingRemotely = state;
	}

	public bool isOverridingElement ()
	{
		return overridingElement;
	}

	public void setOverridingElementState (bool state)
	{
		overridingElement = state;
	}

	public void setUsingDeviceState (bool state)
	{
		usingDevice = state;

		setLastTimeMoved ();
	}

	public bool isUsingDevice ()
	{
		return usingDevice;
	}

	//Menu States
	public void setUsingSubMenuState (bool state)
	{
		usingSubMenu = state;
	}

	public bool isUsingSubMenu ()
	{
		return usingSubMenu;
	}

	public void setPlayerMenuActiveState (bool state)
	{
		playerMenuActive = state;
	}

	public bool isPlayerMenuActive ()
	{
		return playerMenuActive;
	}

	public void setGamePausedState (bool state)
	{
		gamePaused = state;

		setLastTimeMoved ();
	}

	public bool isGamePaused ()
	{
		return gamePaused;
	}

	public void setHeadBobPausedState (bool state)
	{
		headBobPaused = state;
	}

	public GameObject getCurrentVehicle ()
	{
		return currentVehicle;
	}

	public string getCurrentVehicleName ()
	{
		return currentVehicleName;
	}

	//if it is neccessary, stop any movement from the keyboard or the touch controls in the player controller
	public void changeScriptState (bool state)
	{
		if (usingAnimator) {
			if (customCharacterControllerActive) {

			} else {
				animator.SetFloat (forwardAnimatorID, 0);
				animator.SetFloat (turnAnimatorID, 0);
			}
		}

		isMoving = false;

		setCanMoveState (state);

		resetOtherInputFields ();

		resetPlayerControllerInput ();
	}

	public void removeJumpInputState ()
	{
		jumpInput = false;
	}

	public void resetOtherInputFields ()
	{
		jump = false;

		jumpInput = false;

		doubleJump = false;

		slowingFall = false;

		slowingFallInput = false;

		setHeadbodStatesPaused = false;

		gravityMultiplier = currentGravityMultiplier;

		slowFallExternallyActive = false;
	}

	public void setCanMoveState (bool state)
	{
		canMove = state;

		if (showDebugPrint) {
			print ("setting can move state to " + state);
		}
	}

	public void setCanMoveAIState (bool state)
	{
		canMoveAI = state;

		if (showDebugPrint) {
			print ("setting can move AI state to " + state);
		}
	}

	public void setPlayerVelocityToZero ()
	{
		mainRigidbody.velocity = vector3Zero;
		currentVelocity = vector3Zero;
		velocityChange = vector3Zero;
	}

	public void setCurrentVelocityValue (Vector3 newValue)
	{
		currentVelocity = newValue;
	}

	public void setVelocityChangeValue (Vector3 newValue)
	{
		velocityChange = newValue;
	}

	public void smoothChangeScriptState (bool state)
	{
		setCanMoveState (state);

		if (!state) {
			smoothResetPlayerControllerInput ();
		} else {
			if (resetInputCoroutine != null) {
				StopCoroutine (resetInputCoroutine);
			}
		}
	}

	public bool canPlayerMove ()
	{
		return canMove;
	}

	public void setMoveInputPausedState (bool state)
	{
		moveIputPaused = state;
	}

	public bool canPlayerRagdollMove ()
	{
		return canRagdollMove;
	}

	public void setCanRagdollMoveState (bool state)
	{
		canRagdollMove = state;
	}

	bool ragdollCurrentlyActive;

	public void setRagdollCurrentlyActiveState (bool state)
	{
		ragdollCurrentlyActive = state;
	}

	public bool isRagdollCurrentlyActive ()
	{
		return ragdollCurrentlyActive;
	}

	public void setPlayerActionsInputEnabledState (bool state)
	{
		playerActionsInputEnabled = state;
	}

	public void enablePlayerActionsInput ()
	{
		setPlayerActionsInputEnabledState (true);
	}

	public void disablePlayerActionsInput ()
	{
		setPlayerActionsInputEnabledState (false);
	}

	public void smoothResetPlayerControllerInput ()
	{
		if (resetInputCoroutine != null) {
			StopCoroutine (resetInputCoroutine);
		}

		resetInputCoroutine = StartCoroutine (smoothResetPlayerControllerInputCoroutine ());
	}

	IEnumerator smoothResetPlayerControllerInputCoroutine ()
	{
		float currentTime = 0;

		while (verticalInput != 0 || horizontalInput != 0) {
			currentTime = getCurrentDeltaTime ();

			verticalInput = Mathf.MoveTowards (verticalInput, 0, currentTime * 2);
			horizontalInput = Mathf.MoveTowards (horizontalInput, 0, currentTime * 2);

			yield return null;
		}
	}

	public void enableOrDisablePlayerControllerScript (bool state)
	{
		enabled = state;

		if (!enabled) {
			if (customCharacterControllerActive) {

			} else {
				animator.SetFloat (forwardAnimatorID, 0);
				animator.SetFloat (turnAnimatorID, 0);
				animator.SetFloat (jumpAnimatorID, 0);
				animator.SetFloat (jumpLegAnimatorID, 0);
				animator.SetFloat (horizontalAnimatorID, 0);
				animator.SetFloat (verticalAnimatorID, 0);

				animator.SetBool (movingAnimatorID, false);
			}

			previousValueAimingModeActiveAnimatorState = false;
		}
	}

	public void resetAnimatorState ()
	{
		if (isCrouching ()) {
			crouch ();

			if (customCharacterControllerActive) {

			} else {
				animator.SetBool (crouchAnimatorID, false);
			}
		}

		if (customCharacterControllerActive) {

		} else {
			animator.SetFloat (jumpAnimatorID, 0);
			animator.SetFloat (jumpLegAnimatorID, 0);
			animator.SetFloat (turnAnimatorID, 0);
			animator.SetFloat (forwardAnimatorID, 0);

			if (!pauseResetAnimatorStateFOrGroundAnimator) {
				animator.SetBool (onGroundAnimatorID, true);
			}
		}

		pauseResetAnimatorStateFOrGroundAnimator = false;
	}

	public void setPauseResetAnimatorStateFOrGroundAnimatorState (bool state)
	{
		pauseResetAnimatorStateFOrGroundAnimator = state;
	}

	public void setOnGroundAnimatorIDValue (bool state)
	{
		if (!customCharacterControllerActive) {
			setOnGroundAnimatorIDValueWithoutCheck (state);
		}
	}

	public void setOnGroundAnimatorIDValueWithoutCheck (bool state)
	{
		animator.SetBool (onGroundAnimatorID, state);
	}

	public void equipJetpack (bool state)
	{
		jetPackEquiped = state;
	}

	public void setUsingJetpackState (bool state)
	{
		usingJetpack = state;
	}

	public bool isJetpackEquiped ()
	{
		return jetPackEquiped;
	}

	public bool isUsingJetpack ()
	{
		return usingJetpack;
	}

	public void enableOrDisableFlyingMode (bool state)
	{
		flyModeActive = state;

		if (!swimModeActive) {
			setEnableExternalForceOnFlyModeState (false);
		}
	}

	public bool isFlyingActive ()
	{
		return flyModeActive;
	}

	public void enableOrDisableFlyModeTurbo (bool state)
	{

	}

	public void enableOrDisableSwimMode (bool state)
	{
		swimModeActive = state;

		if (!swimModeActive) {
			setEnableExternalForceOnSwimModeState (false);
		}
	}

	public bool isSwimModeActive ()
	{
		return swimModeActive;
	}

	public void setEnableExternalForceOnSwimModeState (bool state)
	{
		enableExternalForceOnSwimMode = state;
	}

	public void setEnableExternalForceOnFlyModeState (bool state)
	{
		enableExternalForceOnFlyMode = state;
	}

	public void setTurnAndForwardAnimatorValuesPausedState (bool state)
	{
		turnAndForwardAnimatorValuesPaused = state;
	}

	public void setPauseCameraShakeFromGravityActiveState (bool state)
	{
		pauseCameraShakeFromGravityActive = state;
	}

	public bool isPauseCameraShakeFromGravityActive ()
	{
		return pauseCameraShakeFromGravityActive;
	}

	public externalControllerBehaviorManager mainExternalControllerBehaviorManager;

	string currentExternalControllerBehaviorName = "";

	public void setExternalControllerBehavior (externalControllerBehavior newExternalControllerBehavior)
	{
		currentExternalControllerBehavior = newExternalControllerBehavior;

		externalControllBehaviorActive = currentExternalControllerBehavior != null;

		if (newExternalControllerBehavior != null) {
			currentExternalControllerBehaviorName = newExternalControllerBehavior.behaviorName;
		}

		if (mainExternalControllerBehaviorManager != null) {
			if (externalControllBehaviorActive) {
				mainExternalControllerBehaviorManager.setExternalControllerActive (currentExternalControllerBehaviorName);
			} else {
				mainExternalControllerBehaviorManager.setExternalControllerDeactive (currentExternalControllerBehaviorName);
			}
		}

		if (newExternalControllerBehavior == null) {
			currentExternalControllerBehaviorName = "";
		}
	}

	public externalControllerBehavior getCurrentExternalControllerBehavior ()
	{
		return currentExternalControllerBehavior;
	}

	public bool isExternalControllBehaviorActive ()
	{
		return externalControllBehaviorActive;
	}

	public string getCurrentExternalControllerBehaviorName ()
	{
		return currentExternalControllerBehaviorName;
	}

	public void setUseExternalControllerBehaviorPausedState (bool state)
	{
		useExternalControllerBehaviorPaused = state;
	}

	public bool isUseExternalControllerBehaviorPaused ()
	{
		return useExternalControllerBehaviorPaused;
	}

	public void setCustomCharacterControllerActiveState (bool state, customCharacterControllerBase customCharacterControllerBaseInfo)
	{
		customCharacterControllerActive = state;

		currentCustomCharacterControllerBase = customCharacterControllerBaseInfo;
	}

	public bool isCustomCharacterControllerActive ()
	{
		return customCharacterControllerActive;
	}

	public void setExternalControlBehaviorForAirTypeActiveState (bool state)
	{
		externalControlBehaviorForAirTypeActive = state;
	}

	public void enableOrDisableAiminig (bool state)
	{
		if (playerCameraManager.isFirstPersonActive ()) {
			enableOrDisableAimingInFirstPerson (state);

			enableOrDisableAimingInThirdPerson (false);
		} else {
			enableOrDisableAimingInThirdPerson (state);

			enableOrDisableAimingInFirstPerson (false);
		}

		if (!state) {
			lastTimePlayerUserInput = 0;
		}
	}

	public void enableOrDisableAimingInFirstPerson (bool state)
	{
		aimingInFirstPerson = state;
	}

	public void enableOrDisableAimingInThirdPerson (bool state)
	{
		aimingInThirdPerson = state;

		checkOtherStatesOnAirID (state);
	}

	public bool isPlayerAiming ()
	{
		return aimingInFirstPerson || aimingInThirdPerson;
	}

	public bool isPlayerAimingInThirdPerson ()
	{
		return aimingInThirdPerson;
	}

	public bool isPlayerAimingInFirstPerson ()
	{
		return aimingInFirstPerson;
	}

	public void checkOtherStatesOnAirID (bool state)
	{
		if (state) {
			if (!regularAirboneIDActivePreviously) {
				if (currentAirID == regularAirboneID) {
					setCurrentAirIDValue (regularAirID);

					regularAirboneIDActivePreviously = true;
				}
			}
		} else {
			if (regularAirboneIDActivePreviously) {
				if (!isPlayerOnFFOrZeroGravityModeOn ()) {
					setCurrentAirIDValue (regularAirboneID);
				}

				regularAirboneIDActivePreviously = false;
			}
		}
	}

	public bool isLookingInCameraDirection ()
	{
		return ((isPlayerAiming () && hasToLookInCameraDirectionOnFreeFire ()) || lookInCameraDirectionActive || firstPersonActive);
	}

	public playerCamera getPlayerCameraManager ()
	{
		return playerCameraManager as playerCamera;
	}

	public GameObject getPlayerCameraGameObject ()
	{
		return playerCameraGameObject;
	}

	public bool isPlayerOnFirstPerson ()
	{
		return firstPersonActive;
	}

	public void setFirstPersonViewActiveState (bool state)
	{
		firstPersonActive = state;

		resetHeadBodState ();

		if (firstPersonActive) {
			if (setHeadbodStatesPaused) {
				setHeadbodStatesPaused = false;

				playerCameraManager.setShakeCameraState (false, "");
			}
		} else {
			if (currentSurfaceBelowPlayer != null) {
				previousValueOnGroundAnimatorState = false;
			}
		}
	}

	public void stopShakeCamera ()
	{
		playerCameraManager.stopShakeCamera ();
	}

	public bool isPlayerMoving (float movingTolerance)
	{
		if (Mathf.Abs (horizontalInput) > movingTolerance || Mathf.Abs (verticalInput) > movingTolerance) {
			return true;
		}

		return false;
	}

	public bool isPlayerMovingHorizontal (float movingTolerance)
	{
		if (Mathf.Abs (horizontalInput) > movingTolerance) {
			return true;
		}

		return false;
	}

	public bool isPlayerMovingVertical (float movingTolerance)
	{
		if (Mathf.Abs (verticalInput) > movingTolerance) {
			return true;
		}

		return false;
	}

	public bool isPlayerUsingInput ()
	{
		if (rawAxisValues.x != 0 || rawAxisValues.y != 0) {
			return true;
		}

		return false;
	}

	public bool isPlayerUsingVerticalInput ()
	{
		if (rawAxisValues.y != 0) {
			return true;
		}

		return false;
	}

	public bool isPlayerUsingHorizontalInput ()
	{
		if (rawAxisValues.x != 0) {
			return true;
		}

		return false;
	}

	public Vector2 getAxisValues ()
	{
		return axisValues;
	}

	public Vector2 getRawAxisValues ()
	{
		return rawAxisValues;
	}

	public void resetPlayerControllerInput ()
	{
		horizontalInput = 0;
		verticalInput = 0;
	}

	public void overrideMainCameraTransformDirection (Transform newCameraDirection, bool overrideState)
	{
		overrideMainCameraTransform = newCameraDirection;
		overrideMainCameraTransformActive = overrideState;
	}

	public void setUseForwardDirectionForCameraDirectionState (bool state)
	{
		useForwardDirectionForCameraDirection = state;
	}

	public void setUseRightDirectionForCameraDirectionState (bool state)
	{
		useRightDirectionForCameraDirection = state;
	}

	public void setOverrideTurnAmount (float newTurnAmount, bool newOverrideTurnAmountActiveState)
	{
		overrideTurnAmount = newTurnAmount;
		overrideTurnAmountActive = newOverrideTurnAmountActiveState;
	}

	public void setLockedCameraState (bool state, bool useTankControlsValue, bool useRelativeMovementToLockedCameraValue,
	                                  bool playerCanMoveOnAimInTankModeValue)
	{
		lockedCameraActive = state;

		useTankControls = useTankControlsValue;

		playerCanMoveOnAimInTankMode = playerCanMoveOnAimInTankModeValue;

		tankModeCurrentlyEnabled = lockedCameraActive && useTankControls;

		useRelativeMovementToLockedCamera = useRelativeMovementToLockedCameraValue;

		if (!lockedCameraActive) {
			checkCameraDirectionFromLockedToFree = true;
		}
	}

	public bool isLockedCameraStateActive ()
	{
		return lockedCameraActive;
	}

	public bool isTankModeActive ()
	{
		return useTankControls && lockedCameraActive;
	}

	public bool canCharacterGetOnVehicles ()
	{
		return canGetOnVehicles;
	}

	public void setCanCharacterGetOnVehiclesState (bool state)
	{
		canGetOnVehicles = state;
	}

	public void setOriginalCanCharacterGetOnVehiclesState ()
	{
		setCanCharacterGetOnVehiclesState (originalCanCharacterGetOnVehiclesState);
	}

	public bool isActionToGetOnVehicleActive ()
	{
		return actionToGetOnVehicleActive;
	}

	public void setActionToGetOnVehicleActiveState (bool state)
	{
		actionToGetOnVehicleActive = state;
	}

	public bool isActionToGetOffFromVehicleActive ()
	{
		return actionToGetOffFromVehicleActive;
	}

	public void setActionToGetOffFromVehicleActiveState (bool state)
	{
		actionToGetOffFromVehicleActive = state;
	}

	public bool canCharacterDrive ()
	{
		return canDrive;
	}

	public void setCanDriveState (bool state)
	{
		canDrive = state;
	}

	public void setOriginalCanDriveState ()
	{
		setCanDriveState (originalCanDrive);
	}

	public void setCanMoveWhileAimLockedCameraValue (bool state)
	{
		canMoveWhileAimLockedCamera = state;
	}

	public void setOriginalCanMoveWhileAimLockedCameraValue ()
	{
		setCanMoveWhileAimLockedCameraValue (originalCanMoveWhileAimLockedCamera);
	}

	public GameObject getPlayerManagersParentGameObject ()
	{
		return playerManagersParentGameObject;
	}

	public void setPlayerAndCameraParent (Transform newParent)
	{
		playerCameraManager.setPlayerAndCameraParent (newParent);

		if (newParent != null) {
			setPlayerSetAsChildOfParentState (true);
		} else {
			setPlayerSetAsChildOfParentState (false);
		}

		currentTemporalPlayerParent = newParent;
	}

	public Transform getCurrentTemporalPlayerParent ()
	{
		return currentTemporalPlayerParent;
	}

	public void setMovingOnPlatformActiveState (bool state)
	{
		movingOnPlatformActive = state;
	}

	public bool isMovingOnPlatformActive ()
	{
		return movingOnPlatformActive;
	}

	//AI input and functions to use this controller with an AI system
	public void setReducedVelocity (float newValue)
	{
		setAnimSpeedMultiplierValue (newValue);
	}

	public void setNormalVelocity ()
	{
		setAnimSpeedMultiplierValue (originalAnimationSpeed);
	}

	void setAnimSpeedMultiplierValue (float newValue)
	{
		animSpeedMultiplier = newValue;
	}

	public float getAnimSpeedMultiplier ()
	{
		return animSpeedMultiplier;
	}

	void setMoveSpeedMultiplierValue (float newValue)
	{
		moveSpeedMultiplier = newValue;
	}

	public void setNewAnimSpeedMultiplierDuringXTime (float newValue)
	{
		changeAnimSpeedMultiplierDuration = newValue;

		stopSetNewAnimSpeedMultiplierDuringXTimeCoroutine ();

		changeAnimSpeedMultiplierCoroutine = StartCoroutine (setNewAnimSpeedMultiplierDuringXTimeCoroutine ());
	}

	IEnumerator setNewAnimSpeedMultiplierDuringXTimeCoroutine ()
	{
		overrideAnimationSpeedActive = true;

		yield return new WaitForSeconds (changeAnimSpeedMultiplierDuration);

		setNormalVelocity ();

		overrideAnimationSpeedActive = false;

		if (usedByAI) {
			setCanMoveAIState (true);
		}
	}

	void stopSetNewAnimSpeedMultiplierDuringXTimeCoroutine ()
	{
		if (changeAnimSpeedMultiplierCoroutine != null) {
			StopCoroutine (changeAnimSpeedMultiplierCoroutine);
		}

		overrideAnimationSpeedActive = false;
	}

	int pauseCharacterPriority = 0;

	public void setPauseCharacterPriorityValue (int newValue)
	{
		pauseCharacterPriority = newValue;
	}

	public int getPauseCharacterPriorityValue ()
	{
		return pauseCharacterPriority;
	}

	public void changeControlInputType (bool value)
	{
		usedByAI = value;

		if (usedByAI) {
			resetPlayerControllerInput ();
		} else {
			playerInput.overrideInputValues (Vector2.zero, false);
		}
	}

	public void setAIElementsEnabledState (bool state)
	{
		if (AIElements.activeSelf != state) {
			AIElements.SetActive (state);
		}
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;

		if (!usedByAI) {
			updateOverrideInputValues (Vector2.zero, false);
		}
	}

	public void startOverride ()
	{
		overrideCharacterControlState (true);
	}

	public void stopOverride ()
	{
		overrideCharacterControlState (false);
	}

	public void overrideCharacterControlState (bool state)
	{
		usedByAI = !state;

		characterControlOverrideActive = state;

		if (!usedByAI) {
			playerInput.overrideInputValues (Vector2.zero, false);
		}
	}

	public bool isCharacterUsedByAI ()
	{
		return usedByAI;
	}

	public bool isCharacterControlOverrideActive ()
	{
		return characterControlOverrideActive;
	}

	public float getHorizontalInput ()
	{
		return horizontalInput;
	}

	public float getVerticalInput ()
	{
		return verticalInput;
	}

	public Vector3 getMoveInputDirection ()
	{
		return currentMoveInput;
	}

	public void setLookInCameraDirectionState (bool state)
	{
		lookInCameraDirection = state;
	}

	// The Move function is designed to be called from a separate component
	// based on User input, or an AI control script
	public void Move (AINavMeshMoveInfo inputInfo)
	{
		if (inputInfo.moveInput.magnitude > 1) {
			inputInfo.moveInput.Normalize ();
		}

		navMeshMoveInput = inputInfo.moveInput;

		//		crouchInput = inputInfo.crouchInput;

		if (inputInfo.jumpInput) {
			activateJump ();
		}

		lookInCameraDirection = inputInfo.lookAtTarget;

		AIStrafeModeActive = inputInfo.strafeModeActive;

		AIEnableInputMovementOnStrafe = inputInfo.AIEnableInputMovementOnStrafe;
	}

	public void setVisibleToAIState (bool state)
	{
		visibleToAI = state;
	}

	public bool isCharacterInVisibleState ()
	{
		return visibleToAI;
	}

	public bool isCharacterVisibleToAI ()
	{
		return visibleToAI && !stealthModeActive;
	}

	public void setStealthModeActiveState (bool state)
	{
		stealthModeActive = state;
	}

	public bool isCharacterInStealthMode ()
	{
		return stealthModeActive;
	}

	public void setCharacterStateIcon (string stateName)
	{
		if (characterStateIconManager != null) {
			characterStateIconManager.setCharacterStateIcon (stateName);
		}
	}

	public void disableCharacterStateIcon ()
	{
		if (characterStateIconManager != null) {
			characterStateIconManager.disableCharacterStateIcon ();
		}
	}

	public int getPlayerID ()
	{
		return playerID;
	}

	public void setPlayerID (int newID)
	{
		playerID = newID;
	}

	public void setPlayerDeadState (bool state)
	{
		isDead = state;

		if (isDead) {
			if (isPlayerRunning ()) {
				stopRun ();
			}
		
			if (isCrouching ()) {
				crouch ();
			}

			lastTimeResurrect = 0;
		} else {
			lastTimeResurrect = Time.time;
		}
	}

	public bool isPlayerDead ()
	{
		return isDead;
	}

	public float getLastTimeResurrect ()
	{
		return lastTimeResurrect;
	}

	public void setFallDamageCheckPausedState (bool state)
	{
		fallDamageCheckPaused = state;
	}

	public void setFallDamageCheckOnHealthPausedState (bool state)
	{
		fallDamageCheckOnHealthPaused = state;
	}

	public bool checkIfPlayerDeadFromHealthComponent ()
	{
		return healthManager.isDead ();
	}

	public bool isPlayerUsingWeapons ()
	{
		if (weaponsManagerLocated) {
			return weaponsManager.isUsingWeapons ();
		}

		return false;
	}

	public bool isPlayerUsingPowers ()
	{
		return usingPowers;
	}

	public bool isPlayerUsingMeleeWeapons ()
	{
		return playerUsingMeleeWeapons;
	}

	public bool isPlayerMeleeWeaponThrown ()
	{
		return playerMeleeWeaponThrown;
	}

	public void setPlayerMeleeWeaponThrownState (bool state)
	{
		playerMeleeWeaponThrown = state;
	}

	public void setPlayerUsingMeleeWeaponsState (bool state)
	{
		playerUsingMeleeWeapons = state;
	}

	public void setPlayerNavMeshEnabledState (bool state)
	{
		playerNavMeshEnabled = state;

		resetPlayerControllerInput ();

		if (!playerNavMeshEnabled) {
			playerInput.overrideInputValues (Vector2.zero, false);
		}
	}

	public bool isPlayerNavMeshEnabled ()
	{
		return playerNavMeshEnabled;
	}

	public void enableOrDisableDoubleJump (bool state)
	{
		enabledDoubleJump = state;
	}

	public void setMaxNumberJumpsInAir (float amount)
	{
		maxNumberJumpsInAir = (int)amount;
	}

	public void enableOrDisableJump (bool state)
	{
		enabledRegularJump = state;
	}

	public void setcanJumpActiveState (bool state)
	{
		canJumpActive = state;
	}

	public void setJumpInputPausedState (bool state)
	{
		jumpInputPaused = state;
	}

	public void setHoldJumpSlowDownFallEnabledState (bool state)
	{
		holdJumpSlowDownFallEnabled = state;
	}

	public void setRunInputPausedState (bool state)
	{
		runInputPaused = state;
	}

	public void setPhysicMaterialAssigmentPausedState (bool state)
	{
		physicMaterialAssigmentPaused = state;
	}

	public void setUsingAbilityActiveState (bool state)
	{
		usingAbilityActive = state;
	}

	public bool isUsingAbilityActive ()
	{
		return usingAbilityActive;
	}

	public void setHighFrictionMaterial ()
	{
		if (!highFrictionMaterialActive) {
			capsule.material = highFrictionMaterial;
			highFrictionMaterialActive = true;
		}
	}

	public void setZeroFrictionMaterial ()
	{
		if (highFrictionMaterialActive) {
			capsule.material = zeroFrictionMaterial;
			highFrictionMaterialActive = false;
		}
	}

	public bool playerCanRunNow ()
	{
		return (!playerCameraManager.isFirstPersonActive () && canRunThirdPersonActive) ||
		(playerCameraManager.isFirstPersonActive () && (noAnimatorCanRun && (getVerticalInput () > 0 || noAnimatorCanRunBackwards)));
	}

	public void setNoAnimatorWalkMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorWalkMovementSpeed = originalNoAnimWalkMovementSpeed;
		} else {
			noAnimatorWalkMovementSpeed = originalNoAnimWalkMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorRunMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorRunMovementSpeed = originalNoAnimRunMovementSpeed;
		} else {
			noAnimatorRunMovementSpeed = originalNoAnimRunMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorCrouchMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorCrouchMovementSpeed = originalNoAnimCrouchMovementSpeed;
		} else {
			noAnimatorCrouchMovementSpeed = originalNoAnimCrouchMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorStrafeMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorStrafeMovementSpeed = originalNoAnimStrafeMovementSpeed;
		} else {
			noAnimatorStrafeMovementSpeed = originalNoAnimStrafeMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorWalkBackwardMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorWalkBackwardMovementSpeed = originalNoAnimWalkBackwardMovementSpeed;
		} else {
			noAnimatorWalkBackwardMovementSpeed = originalNoAnimWalkBackwardMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorRunBackwardMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorRunBackwardMovementSpeed = originalNoAnimRunBackwardMovementSpeed;
		} else {
			noAnimatorRunBackwardMovementSpeed = originalNoAnimRunBackwardMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorCrouchBackwardMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorCrouchBackwardMovementSpeed = originalNoAnimCrouchBackwardMovementSpeed;
		} else {
			noAnimatorCrouchBackwardMovementSpeed = originalNoAnimCrouchBackwardMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorStrafeBackwardMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorStrafeBackwardMovementSpeed = originalNoAnimStrafeBackwardMovementSpeed;
		} else {
			noAnimatorStrafeBackwardMovementSpeed = originalNoAnimStrafeBackwardMovementSpeed * newValue;
		}
	}

	public void setNoAnimatorCanRunState (bool newState, bool setOriginalValue)
	{
		if (setOriginalValue) {
			noAnimatorCanRun = originalNoAnimCanRun;
		} else {
			noAnimatorCanRun = newState;
		}
	}

	public void setNoAnimatorGeneralMovementSpeed (float newValue, bool setOriginalValue)
	{
		setNoAnimatorWalkMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorRunMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorCrouchMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorStrafeMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorWalkBackwardMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorRunBackwardMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorCrouchBackwardMovementSpeed (newValue, setOriginalValue);
		setNoAnimatorStrafeBackwardMovementSpeed (newValue, setOriginalValue);
	}

	public void setNoAnimatorSpeedValue (float newValue)
	{
		noAnimatorSpeed = newValue;
	}

	public void setAnimatorCanRunState (bool newState, bool setOriginalValue)
	{
		if (setOriginalValue) {
			canRunThirdPersonActive = true;
		} else {
			canRunThirdPersonActive = newState;
		}
	}

	public void setAnimatorGeneralMovementSpeed (float newValue, bool setOriginalValue)
	{
		if (setOriginalValue) {
			setOriginalWalkSpeed ();
		} else {
			setWalkSpeedValue (newValue);
		}
	}

	public bool hasToLookInCameraDirection ()
	{
		//if the player has active the option to look in the camera direction, or to look on that direction when the camera is looking to a target, return true
		//also, check if the player only follows the camera direction while moving or not, and also, check if he is currently on ground state
		return (lookAlwaysInCameraDirection || (lookInCameraDirectionIfLookingAtTarget && playerCameraManager.isPlayerLookingAtTarget ()))
		&& lookInCameraDirectionOnFreeFireActive
		&& playerOnGround;
	}

	bool ignoreLookInCameraDirectionOnFreeFireActive;

	public void setIgnoreLookInCameraDirectionOnFreeFireActiveState (bool state)
	{
		ignoreLookInCameraDirectionOnFreeFireActive = state;
	}

	public bool hasToLookInCameraDirectionOnFreeFire ()
	{
		if (ignoreLookInCameraDirectionOnFreeFireActive) {
			return false;
		}

		return (!usingFreeFireMode && !checkToKeepWeaponAfterAimingWeaponFromShooting) || lookInCameraDirectionOnFreeFire || useRelativeMovementToLockedCamera;
	}

	public bool isPlayerLookingAtTarget ()
	{
		return playerCameraManager.isPlayerLookingAtTarget ();
	}

	public bool istargetToLookLocated ()
	{
		return playerCameraManager.istargetToLookLocated ();
	}

	public Transform getCurrentTargetToLook ()
	{
		return playerCameraManager.getCurrentTargetToLook ();
	}

	public void setCheckToKeepWeaponAfterAimingWeaponFromShootingState (bool state)
	{
		checkToKeepWeaponAfterAimingWeaponFromShooting = state;

		if (!checkToKeepWeaponAfterAimingWeaponFromShooting) {
			lastTimeCheckToKeepWeapon = Time.time;
		}
	}

	public bool isCheckToKeepWeaponAfterAimingWeaponFromShooting ()
	{
		return checkToKeepWeaponAfterAimingWeaponFromShooting;
	}

	public void setLookAlwaysInCameraDirectionState (bool state)
	{
		lookAlwaysInCameraDirection = state;
	}

	public bool isLookAlwaysInCameraDirectionActive ()
	{
		return lookAlwaysInCameraDirection;
	}

	public void setOriginalLookAlwaysInCameraDirectionState ()
	{
		setLookAlwaysInCameraDirectionState (originalLookAlwaysInCameraDirection);
	}

	public void setLookInCameraDirectionIfLookingAtTargetState (bool state)
	{
		lookInCameraDirectionIfLookingAtTarget = state;
	}

	public void setLookOnlyIfMovingState (bool state)
	{
		lookOnlyIfMoving = state;
	}

	public void setOriginalLookOnlyIfMovingState ()
	{
		setLookOnlyIfMovingState (originalLookOnlyIfMoving);
	}

	public void activateOrDeactivateStrafeMode (bool state)
	{
		if (state) {
			activateStrafeMode ();
		} else {
			deactivateStrafeMode ();
		}
	}

	public void activateStrafeMode ()
	{
		//		print ("activate strafe mode");

		setLookAlwaysInCameraDirectionState (true);

		setLookOnlyIfMovingState (false);
	}

	public void deactivateStrafeMode ()
	{
		setOriginalLookAlwaysInCameraDirectionState ();

		setOriginalLookOnlyIfMovingState ();
	}

	public void setZeroGravityModeOnState (bool state)
	{
		zeroGravityModeOn = state;

		if (zeroGravityModeOn) {
			if (pushPlayerWhenZeroGravityModeIsEnabled) {
				useJumpPlatform (playerTransformUp * pushZeroGravityEnabledAmount, ForceMode.Impulse);
			} 

			if (!playerOnGround) {
				setCheckOnGrundStatePausedFFOrZGState (true);
			}
		} else {
			setCheckOnGrundStatePausedFFOrZGState (false);
		}

		movingVertically = false;

		setLastTimeFalling ();

		IKSystemManager.setIKBodyState (zeroGravityModeOn, "Zero Gravity Mode");

		setFootStepManagerState (zeroGravityModeOn);

		checkOtherStatesOnAirID (state);
	}

	public void setPushPlayerWhenZeroGravityModeIsEnabledState (bool state)
	{
		pushPlayerWhenZeroGravityModeIsEnabled = state;
	}

	public bool isPlayerOnZeroGravityMode ()
	{
		return zeroGravityModeOn;
	}

	public void changeZeroGravityMovementVelocity (bool value)
	{
		if (value) {
			currentZeroGravitySpeedMultiplier = zeroGravitySpeedMultiplier;

			playerCameraManager.changeCameraFov (true);
		} else {
			currentZeroGravitySpeedMultiplier = 1;

			playerCameraManager.changeCameraFov (false);
		}

		movementSpeedIncreased = value;
	}

	public void setCheckOnGrundStatePausedFFOrZGState (bool state)
	{
		if (state) {
			if ((zeroGravityModeOn && pauseCheckOnGroundStateZG) || (freeFloatingModeOn && pauseCheckOnGroundStateFF)) {
				checkOnGroundStatePausedFFOrZG = state;
			}
		} else {
			checkOnGroundStatePausedFFOrZG = state;
		}
	}

	public void setcheckOnGroundStatePausedState (bool state)
	{
		checkOnGroundStatePaused = state;
	}

	public bool isPauseCheckOnGroundStateZGActive ()
	{
		return pauseCheckOnGroundStateZG;
	}

	public void setfreeFloatingModeOnState (bool state)
	{
		freeFloatingModeOn = state;

		if (freeFloatingModeOn) {
			useJumpPlatform (playerTransformUp * pushFreeFloatingModeEnabledAmount, ForceMode.Force);

			if (!playerOnGround) {
				setCheckOnGrundStatePausedFFOrZGState (true);
			}
		} else {
			setCheckOnGrundStatePausedFFOrZGState (false);
		}

		setLastTimeFalling ();

		movingVertically = false;

		playerCameraManager.changeCameraFov (false);

		IKSystemManager.setIKBodyState (freeFloatingModeOn, "Free Floating Mode");

		setFootStepManagerState (freeFloatingModeOn);

		checkOtherStatesOnAirID (state);
	}

	public void enableOrDisableIKSystemManagerState (bool state)
	{
		IKSystemManager.enableOrDisableIKSystemManagerState (state);
	}

	public void setIKBodyPausedState (bool state)
	{
		IKSystemManager.setIKBodyPausedState (state);
	}

	public bool isPlayerOnFreeFloatingMode ()
	{
		return freeFloatingModeOn;
	}

	public void changeFreeFloatingMovementVelocity (bool value)
	{
		if (value) {
			currentFreeFloatingSpeedMultiplier = freeFloatingSpeedMultiplier;

			playerCameraManager.changeCameraFov (true);
		} else {
			currentFreeFloatingSpeedMultiplier = 1;

			playerCameraManager.changeCameraFov (false);
		}

		movementSpeedIncreased = value;
	}

	public void changeSprintFovThirdPerson (bool value)
	{
		if (value) {
			playerCameraManager.changeCameraFov (true);
		} else {
			playerCameraManager.changeCameraFov (false);
		}
	}

	public bool isPlayerOnFFOrZeroGravityModeOn ()
	{
		return zeroGravityModeOn || freeFloatingModeOn;
	}

	public bool isMovementSpeedIncreased ()
	{
		return movementSpeedIncreased;
	}

	public float getCharacterHeight ()
	{
		return originalHeight;
	}

	public float getVerticalSpeed ()
	{
		return playerTransform.InverseTransformDirection (mainRigidbody.velocity).y;
	}

	public void setWalkSpeedValue (float newValue)
	{
		walkSpeed = Mathf.Clamp (newValue, 0, 1);
	}

	public void setOriginalWalkSpeed ()
	{
		setWalkSpeedValue (originalWalkSpeedValue);
	}

	public void setWalkByDefaultState (bool state)
	{
		if (state) {
			setWalkSpeedValue (half);
		} else {
			setWalkSpeedValue (1);
		}
	}

	public void setIncreaseMovementSpeedState (bool state)
	{
		if (state) {
			setWalkSpeedValue (increaseWalkSpeedValue);
		} else {
			setWalkSpeedValue (originalWalkSpeedValue);
		}
	}

	public void setIncreaseWalkSpeedValue (float newValue)
	{
		increaseWalkSpeedValue = newValue;

		if (increaseWalkSpeedValue < originalWalkSpeedValue) {
			increaseWalkSpeedActive = true;
		} else {
			increaseWalkSpeedActive = false;
		}
	}

	public void setIncreaseWalkSpeedEnabled (bool state)
	{
		increaseWalkSpeedEnabled = state;

		increaseWalkSpeedActive = false;
	}

	public void setOriginalIncreaseWalkSpeedEnabledValue ()
	{
		setIncreaseWalkSpeedEnabled (originalIncreaseWalkSpeedEnabledValue);
	}

	public void setNewStationaryTurnSpeedValue (float newValue)
	{
		stationaryTurnSpeed = newValue;
	}

	public void setOriginalStationaryTurnSpeed ()
	{
		setNewStationaryTurnSpeedValue (originalStationaryTurnSpeed);
	}

	public void setStairsValues (float stairsMin, float stairsMax, float stairsAdherence)
	{
		currentStairAdherenceSystemMinValue = stairsMin;
		currentStairAdherenceSystemMaxValue = stairsMax;
		currentStairAdherenceSystemAdherenceValue = stairsAdherence;
	}

	public void setCharacterMeshGameObject (GameObject characterMesh)
	{
		characterMeshGameObject = characterMesh;

		updateComponent ();
	}

	bool usingExternalCharacterMeshList;

	public List<GameObject> externalCharacterMeshList = new List<GameObject> ();

	public void setUsingExternalCharacterMeshListState (bool state)
	{
		usingExternalCharacterMeshList = state;
	}

	public void setExternalCharacterMeshList (List<GameObject> newList)
	{
		externalCharacterMeshList = newList;
	}

	public void setCharacterMeshGameObjectState (bool state)
	{
		if (!checkCharacterMeshIfGeneratedOnStartInitialized) {
			if (checkCharacterMeshIfGeneratedOnStart) {

				Transform objectToCheckCharacterIfGeneratedOnStartTransform = objectToCheckCharacterIfGeneratedOnStart.transform;

				int count = objectToCheckCharacterIfGeneratedOnStartTransform.childCount;

				for (int i = 0; i < count; i++) {
					Transform child = objectToCheckCharacterIfGeneratedOnStartTransform.GetChild (i);

					if (child != null) {
						if (child.GetComponent<SkinnedMeshRenderer> () != null) {
							extraCharacterMeshGameObject.Add (child.gameObject);
						}
					}
				}
			}

			checkCharacterMeshIfGeneratedOnStartInitialized = true;
		}

		if (usingExternalCharacterMeshList) {
			if (externalCharacterMeshList.Count > 0) {
				for (int i = 0; i < externalCharacterMeshList.Count; i++) {
					if (externalCharacterMeshList [i] != null && externalCharacterMeshList [i].activeSelf != state) {
						externalCharacterMeshList [i].SetActive (state);
					}
				}
			}
		} else {
			if (characterMeshGameObject != null) {
				if (characterMeshGameObject.activeSelf != state) {
					characterMeshGameObject.SetActive (state);
				}
			}

			if (extraCharacterMeshGameObject.Count > 0) {
				for (int i = 0; i < extraCharacterMeshGameObject.Count; i++) {
					if (extraCharacterMeshGameObject [i] != null && extraCharacterMeshGameObject [i].activeSelf != state) {
						extraCharacterMeshGameObject [i].SetActive (state);
					}
				}
			}
		}

		if (useEventsOnEnableDisableCharacterMeshes) {
			if (Application.isPlaying) {
				if (state) {
					eventOnEnableCharacterMeshes.Invoke ();
				} else {
					eventOnDisableCharacterMeshes.Invoke ();
				}
			} else {
				if (state) {
					eventOnEnableCharacterMeshesOnEditor.Invoke ();
				} else {
					eventOnDisableCharacterMeshesOnEditor.Invoke ();
				}
			}
		}
	}

	public void setCharacterMeshesListToDisableOnEventState (bool state)
	{
		characterMeshesListToDisableOnEvent.Invoke (state);
	}

	public void setUsingGenericModelActiveState (bool state)
	{
		usingGenericModelActive = state;
	}

	public bool isUsingGenericModelActive ()
	{
		return usingGenericModelActive;
	}

	public Transform getGravityCenter ()
	{
		return gravityManager.getGravityCenter ();
	}

	public GameObject getCharacterMeshGameObject ()
	{
		return characterMeshGameObject;
	}

	public Transform getCharacterHumanBone (HumanBodyBones boneToFind)
	{
		return animator.GetBoneTransform (boneToFind);
	}

	public Transform getRightHandTransform ()
	{
		Transform rightHandTransform = getCharacterHumanBone (HumanBodyBones.RightHand);

		if (rightHandTransform != null) {
			return rightHandTransform;
		} else {
			return weaponsManager.getRightHandTransform ();
		}
	}

	public Transform getLeftHandTransform ()
	{
		Transform leftHandTransform = getCharacterHumanBone (HumanBodyBones.LeftHand);

		if (leftHandTransform != null) {
			return leftHandTransform;
		} else {
			return weaponsManager.getLeftHandTransform ();
		}
	}

	public Animator getCharacterAnimator ()
	{
		return animator;
	}

	public void setAnimatorState (bool state)
	{
		animator.enabled = state;
	}

	public void resetAnimator ()
	{
		animator.Rebind ();
		animator.Update (0f);
	}

	public void setAnimatorUnscaledTimeState (bool state)
	{
		if (state) {
			animator.updateMode = AnimatorUpdateMode.UnscaledTime;
		} else {
			animator.updateMode = AnimatorUpdateMode.Normal;
		}
	}

	public void playerCrossFadeInFixedTime (string actionName)
	{
		animator.CrossFadeInFixedTime (actionName, 0.1f);
	}

	public playerInputManager getPlayerInput ()
	{
		return playerInput;
	}

	public void set3dOr2_5dWorldType (bool movingOn3dWorld)
	{
		if (movingOn3dWorld) {
			lockedPlayerMovement = lockedPlayerMovementMode.world3d;
		} else {
			lockedPlayerMovement = lockedPlayerMovementMode.world2_5d;
		}
	}

	public bool isPlayerMovingOn3dWorld ()
	{
		return lockedPlayerMovement == lockedPlayerMovementMode.world3d;
	}

	public bool isPlayerMovingOn2_5dWorld ()
	{
		return lockedPlayerMovement == lockedPlayerMovementMode.world2_5d;
	}

	public bool isPlayerSetAsChildOfParent ()
	{
		return playerSetAsChildOfParent;
	}

	public Rigidbody getRigidbody ()
	{
		return mainRigidbody;
	}

	public void setPlayerSetAsChildOfParentState (bool state)
	{
		playerSetAsChildOfParent = state;
	}

	public void setHeadTrackCanBeUsedState (bool state)
	{
		headTrackCanBeUsed = state;
	}

	public bool canHeadTrackBeUsed ()
	{
		return headTrackCanBeUsed;
	}

	public void setAddExtraRotationPausedState (bool state)
	{
		addExtraRotationPaused = state;
	}

	public void updateLandMark ()
	{
		landMarkRayPosition = playerTransform.position;
		landMarkRayDirection = -playerTransformUp;

		if (gravityManager.isPlayerSearchingGravitySurface ()) {
			landMarkRayPosition = mainCameraTransform.position;
			landMarkRayDirection = mainCameraTransform.forward;
		}

		if (zeroGravityModeOn && useGravityDirectionLandMark) {
			if (firstPersonActive) {
				landMarkRayPosition = mainCameraTransform.position;
				landMarkRayDirection = mainCameraTransform.forward;
			} else {
				landMarkRayPosition = forwardSurfaceRayPosition.position;
				landMarkRayDirection = forwardSurfaceRayPosition.forward;
			}
			currentMaxLandDistance = maxDistanceToAdjust;
		} else {
			currentMaxLandDistance = maxLandDistance;
		}

		if (!playerOnGround && canMove) {
			if (flyModeActive || swimModeActive) {
				if (landMark.activeSelf) {
					landMark.SetActive (false);
				}
			} else {
				if (Physics.Raycast (landMarkRayPosition, landMarkRayDirection, out hit, currentMaxLandDistance, layer)) {

					if (hit.distance >= minDistanceShowLandMark) {
						if (!landMark.activeSelf) {
							landMark.SetActive (true);
						}

						landMark.transform.position = hit.point + hit.normal * 0.08f;

						landMarkForwardDirection = Vector3.Cross (landMark.transform.right, hit.normal);
						landMarkTargetRotation = Quaternion.LookRotation (landMarkForwardDirection, hit.normal);

						landMark.transform.rotation = landMarkTargetRotation;

						landMark1.transform.Rotate (0, 100 * currentUpdateDeltaTime, 0);
						landMark2.transform.Rotate (0, -100 * currentUpdateDeltaTime, 0);
					} else {
						if (landMark.activeSelf) {
							landMark.SetActive (false);
						}
					}
				} else {
					if (zeroGravityModeOn && useGravityDirectionLandMark) {
						if (landMark.activeSelf) {
							landMark.SetActive (false);
						}
					} else {
						if (landMark.activeSelf) {
							landMark.SetActive (false);
						}
					}
				}
			}
		} else {
			if (landMark.activeSelf) {
				landMark.SetActive (false);
			}
		}
	}

	public void setUseLandMarkState (bool state)
	{
		useLandMark = state;
	}

	public void rotateCharacterTowardDirection (Transform directionTransform, float minRotationAngle, float minRotationAmount, float maxRotationAmount)
	{
		stopRotateCharacterTowardDirectionCoroutine ();

		rotateCharacterCoroutine = StartCoroutine (rotateCharacterTowardDirectionCoroutine (directionTransform, minRotationAngle, minRotationAmount, maxRotationAmount));
	}

	public void stopRotateCharacterTowardDirectionCoroutine ()
	{
		if (rotateCharacterCoroutine != null) {
			StopCoroutine (rotateCharacterCoroutine);
		}

		actionActiveWithMovement = false;

		applyRootMotionAlwaysActive = false;

		actionActive = false;

		lastTimeActionActive = Time.time;

		setCanMoveState (true);
	}

	IEnumerator rotateCharacterTowardDirectionCoroutine (Transform directionTransform, float minRotationAngle, float minRotationAmount, float maxRotationAmount)
	{
		float timer = 0;

		bool targetReached = false;

		actionActiveWithMovement = true;

		applyRootMotionAlwaysActive = true;

		actionActive = true;

		setCanMoveState (false);

		resetPlayerControllerInput ();

		resetOtherInputFields ();

		while (!targetReached) {

			float newTurnAmount = 0;

			float angle = Vector3.SignedAngle (playerTransformForward, directionTransform.forward, playerTransformUp);

			if (Mathf.Abs (angle) > minRotationAngle) {
				newTurnAmount = angle * Mathf.Deg2Rad;

				newTurnAmount = Mathf.Clamp (newTurnAmount, -maxRotationAmount, maxRotationAmount);
			} else {
				newTurnAmount = 0;
			}

			float turnAmountToApply = newTurnAmount;
			if (turnAmountToApply < 0) {
				if (turnAmountToApply > -minRotationAmount) {
					turnAmountToApply = -minRotationAmount;
				}
			} else {
				if (turnAmountToApply < minRotationAmount) {
					turnAmountToApply = minRotationAmount;
				}
			}

			setOverrideTurnAmount (turnAmountToApply, true);

			timer += Time.deltaTime;

			//|| timer >= 6

			if (newTurnAmount == 0) {
				targetReached = true;

				setOverrideTurnAmount (0, false);

				//				if (timer >= 6) {
				//					print ("too much time, ending rotation");
				//				}
			}

			yield return null;
		}

		stopRotateCharacterTowardDirectionCoroutine ();
	}

	public void destroyCharacterAtOnce ()
	{
		Destroy (playerManagersParentGameObject);
		Destroy (playerCameraGameObject);
		Destroy (gameObject);
	}

	//	private readonly RaycastHit[] _groundCastResults = new RaycastHit[8];


	//	void OnCollisionStay ()
	//	{
	//		var bounds = mainCollider.bounds;
	//		var extents = bounds.extents;
	//		var radius = extents.x - 0.01f;
	//		Physics.SphereCastNonAlloc (bounds.center, radius, -playerTransform.up,
	//			_groundCastResults, extents.y - radius * half, ~0, QueryTriggerInteraction.Ignore);
	//
	//		if (!_groundCastResults.Any (detectedHit => detectedHit.collider != null && detectedHit.collider != mainCollider)) {
	//			return;
	//		}
	//
	//		for (var i = 0; i < _groundCastResults.Length; i++) {
	//			_groundCastResults [i] = new RaycastHit ();
	//		}
	//
	////		print ("ground");
	////		_isGrounded = true;
	//	}


	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	//Draw gizmos
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
			if (playerTransform == null) {
				playerTransform = transform;
			}

			if (useAutoCrouch) {
				Gizmos.color = gizmoColor;

				Vector3 position = autoCrouchRayPosition.position;
				Gizmos.DrawSphere (position, gizmoRadius);
				Gizmos.color = Color.white;
				Gizmos.DrawLine (position, position + playerTransform.forward * raycastDistanceToAutoCrouch);

				Gizmos.DrawSphere (position + secondRaycastOffset, gizmoRadius);

				Gizmos.DrawLine (position + secondRaycastOffset, position + secondRaycastOffset + playerTransform.forward * raycastDistanceToAutoCrouch);
			}

			if (showGizmo) {
				Debug.DrawRay (playerTransform.position + playerTransform.up, -playerTransform.up * rayDistance, Color.white);

				if (useSphereRaycastForGroundDetection) {
					Gizmos.DrawSphere (currentPlayerPosition - currentPlayerUp * sphereCastOffset, sphereCastRadius);
				}
			}

//			if (useCaspsuleCastForGroundDetection) {
//				GKC_Utils.drawCapsuleGizmo (point1, point2, capsuleCastRadius, Color.white, Color.red, currentRayTargetPosition, rayDirection, distanceToTarget);
//			}
		}
	}
}