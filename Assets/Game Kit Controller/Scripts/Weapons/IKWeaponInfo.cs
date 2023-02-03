using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class IKWeaponInfo
{
	public GameObject weapon;
	public Transform aimPosition;
	public Transform walkPosition;
	public Transform keepPosition;
	public Transform aimRecoilPosition;
	public Transform walkRecoilPosition;

	public bool checkSurfaceCollision;
	public float collisionRayDistance = 0.54f;
	public float collisionMovementSpeed = 10;
	public Transform surfaceCollisionPosition;
	public Transform surfaceCollisionRayPosition;
	public bool hideCursorOnCollision;

	public Transform lowerWeaponPosition;

	public bool useSwayInfo = true;
	public bool useRunPosition;
	public float runMovementSpeed = 10;
	public Transform runPosition;
	public bool hideCursorOnRun;
	public bool useRunSwayInfo;

	public bool useJumpPositions;
	public Transform jumpStartPosition;
	public Transform jumpEndPosition;
	public float jumpStartMovementSpeed = 3;
	public float jumpEndtMovementSpeed = 3;
	public float resetJumpMovementSped = 3;
	public float delayAtJumpEnd = 0.4f;

	public bool useReloadMovement;
	public BezierSpline reloadSpline;
	public float reloadDuration = 1;
	public float reloadLookDirectionSpeed = 11;

	public bool useNewFovOnRun;
	public float changeFovSpeed;
	public float newFovOnRun;

	public bool useMeleeAttack;
	public bool useCustomMeleeAttackLayer;
	public LayerMask customMeleeAttackLayer;
	public bool useCapsuleRaycast;
	public Transform meleeAttackPosition;
	public float meleeAttackStartMovementSpeed;
	public float meleeAttackEndMovementSpeed;
	public float meleeAttackEndDelay;
	public Transform meleeAttackRaycastPosition;
	public float meleeAttackRaycastDistance;
	public float meleeAttackRaycastRadius;
	public float meleeAttackDamageAmount;
	public bool meleeAttackIgnoreShield;
	public bool useMeleeAttackShakeInfo;
	public bool applyMeleeAtackForce;
	public float meleeAttackForceAmount;
	public ForceMode meleeAttackForceMode;
	public bool meleeAttackApplyForceToVehicles;
	public float meleAttackForceToVehicles;
	public bool useMeleeAttackSound;
	public AudioClip meleeAttackSurfaceSound;
	public AudioClip meleeAttackAirSound;
	public bool useMeleeAttackParticles;
	public GameObject meleeAttackParticles;
	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	public int damageTypeID = -1;

	public bool useActionSystemForMeleeAttack;
	public string meleeAttackActionName;

	public bool hasAttachments;
	public Transform editAttachmentPosition;
	public Transform attachmentCameraPosition;

	public GameObject leftHandMesh;
	public GameObject extraLeftHandMesh;

	public Transform leftHandEditPosition;

	public Transform leftHandParent;
	public Transform extraLeftHandMeshParent;

	public float editAttachmentMovementSpeed = 2;
	public float editAttachmentHandSpeed = 6;
	public bool usingSecondPositionForHand;
	public Transform secondPositionForHand;

	public bool useSightPosition;
	public Transform sightPosition;
	public Transform sightRecoilPosition;

	public bool canRunWhileCarrying = true;
	public bool canRunWhileAiming = true;
	public bool useNewCarrySpeed;
	public float newCarrySpeed;
	public bool useNewAimSpeed;
	public float newAimSpeed;

	public bool deactivateIKIfNotAiming;
	public bool placeWeaponOnWalkPositionBeforeDeactivateIK = true;
	public List<Transform> deactivateIKDrawPath = new List<Transform> ();
	public Transform weaponPositionInHandForDeactivateIK;

	public float drawWeaponMovementSpeed = 2;
	public float aimMovementSpeed;
	public bool weaponHasRecoil = true;
	public float recoilSpeed = 5;
	public float endRecoilSpeed = 4;
	public bool useExtraRandomRecoil;
	public bool useExtraRandomRecoilPosition = true;
	public Vector3 extraRandomRecoilPosition;
	public bool useExtraRandomRecoilRotation = true;
	public Vector3 extraRandomRecoilRotation;

	public bool useQuickDrawKeepWeapon;

	public bool useBezierCurve;
	public BezierSpline spline;
	public float bezierDuration = 0.5f;
	public float lookDirectionSpeed = 11;

	public List<Transform> keepPath = new List<Transform> ();
	public List<IKWeaponsPosition> handsInfo = new List<IKWeaponsPosition> ();
	public bool handsInPosition;

	public bool useWeaponRotationPoint;
	public Transform weaponRotationPoint;
	public Transform weaponRotationPointHolder;
	public Transform weaponRotationPointHeadLookTarget;
	public weaponRotationPointInfo rotationPointInfo;
	public Transform weaponPivotPoint;

	public bool useCrouchPosition;
	public Transform crouchPosition;
	public Transform crouchRecoilPosition;
	public float crouchMovementSpeed = 5;

	public bool useHandle;
	[Range (0, 0.1f)] public float handleRadius;
	public Color handleGizmoColor;

	public bool usePositionHandle = true;

	public bool dualWeaponActive;

	public bool usedOnRightHand;

	public bool canBeUsedAsDualWeapon;
	public dualWeaponInfo rightHandDualWeaopnInfo;
	public dualWeaponInfo leftHandDualWeaponInfo;

	public float changeOneOrTwoHandWieldSpeed = 5;

	public bool usingWeaponAsOneHandWield;

	public bool showSingleWeaponSettings;

	public bool showDualWeaponSettings;

	public bool useCustomThirdPersonAimActive;
	public string customDefaultThirdPersonAimRightStateName;
	public string customDefaultThirdPersonAimLeftStateName;

	public bool useNewIdleID;
	public int newIdleID;

	public bool useDrawKeepWeaponAnimation;
	public string drawWeaponActionName;
	public string keepWeaponActionName;

	public float delayToPlaceWeaponOnHandOnDrawAnimation = 1;
	public float delayToPlaceWeaponOnKeepAnimation = 1;
}

[System.Serializable]
public class dualWeaponInfo
{
	public Transform aimPosition;
	public Transform aimRecoilPosition;

	public Transform walkPosition;
	public Transform walkRecoilPosition;

	public Transform keepPosition;

	public Transform crouchPosition;
	public Transform crouchRecoilPosition;

	public float collisionRayDistance = 0.54f;
	public Transform surfaceCollisionPosition;
	public Transform surfaceCollisionRayPosition;

	public Transform lowerWeaponPosition;
	public Transform runPosition;

	public Transform jumpStartPosition;
	public Transform jumpEndPosition;

	public Transform meleeAttackPosition;
	public Transform meleeAttackRaycastPosition;

	public GameObject firstPersonHandMesh;

	public Transform editAttachmentPosition;
	public Transform attachmentCameraPosition;

	public bool deactivateIKIfNotAiming;
	public List<Transform> deactivateIKDrawPath = new List<Transform> ();
	public Transform weaponPositionInHandForDeactivateIK;
	public bool placeWeaponOnWalkPositionBeforeDeactivateIK = true;

	public bool useBezierCurve;
	public BezierSpline spline;
	public float bezierDuration = 0.5f;
	public float lookDirectionSpeed = 11;

	public List<Transform> keepPath = new List<Transform> ();
	public List<IKWeaponsPosition> handsInfo = new List<IKWeaponsPosition> ();
	public bool handsInPosition;

	public bool useQuickDrawKeepWeapon;

	public bool placeWeaponOnKeepPositionSideBeforeDraw;

	public bool useReloadMovement;
	public BezierSpline reloadSpline;
	public float reloadDuration = 1;
	public float reloadLookDirectionSpeed = 11;
}

[System.Serializable]
public class IKWeaponsPosition
{
	public string Name;
	public Transform handTransform;
	public AvatarIKGoal limb;
	public Transform position;
	public float HandIKWeight;
	public float targetValue;
	public float handMovementSpeed = 2;
	public Transform waypointFollower;
	public List<Transform> wayPoints = new List<Transform> ();
	public bool handInPositionToAim;
	public Transform transformFollowByHand;
	public bool usedToDrawWeapon;
	public IKWeaponsPositionElbow elbowInfo;
	public bool showElbowInfo;
	public Coroutine handMovementCoroutine;
	public bool handUsedInWeapon = true;

	public bool ignoreIKWeight;

	public bool useBezierCurve;
	public BezierSpline spline;
	public float bezierDuration = 0.5f;
	public float lookDirectionSpeed = 11;

	public bool useGrabbingHandID;
	public int grabbingHandID = 1;
}

[System.Serializable]
public class IKWeaponsPositionElbow
{
	public string Name;
	public AvatarIKHint elbow;
	public Transform position;
	public float elbowIKWeight;
	public float targetValue;
	public Transform elbowOriginalPosition;
}

[System.Serializable]
public class weaponRotationPointInfo
{
	public float rotationUpPointAmountMultiplier = 1.5f;
	public float rotationDownPointAmountMultiplier = 1.5f;
	public float rotationPointSpeed = 20;
	public bool useRotationUpClamp = true;
	public float rotationUpClampAmount = 50;
	public bool useRotationDownClamp = true;
	public float rotationDownClamp = 31;
}