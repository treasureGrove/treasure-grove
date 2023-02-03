using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class weaponInfo
{
	public string Name;
	public int numberKey;

	[Space]
	[Space]

	public bool useRayCastShoot;
	public bool useInfiniteRaycastDistance = true;
	public float maxRaycastDistance = 200;

	public bool checkIfSurfacesCloseToWeapon;
	public float minDistanceToCheck = 2;

	public bool useRaycastShootDelay;
	public float raycastShootDelay;
	public bool getDelayWithDistance;
	public float delayWithDistanceSpeed;
	public float maxDelayWithDistance;

	public bool useFakeProjectileTrails;

	public bool fireWeaponForward;

	public bool useRaycastCheckingOnRigidbody;

	public float customRaycastCheckingRate;

	public float customRaycastCheckingDistance = 0.1f;

	[Space]
	[Space]

	public bool weaponUsesAmmo = true;
	public int ammoPerClip;
	public bool removePreviousAmmoOnClip;
	public bool infiniteAmmo;

	public bool useRemainAmmoFromInventory;

	public int remainAmmo;
	public int clipSize;

	public bool startWithEmptyClip;

	public bool useAmmoLimit;
	public int ammoLimit;
	public int auxRemainAmmo;

	public string ammoName;

	public bool ignoreAnyWaitTimeOnReload;

	[Space]
	[Space]

	public bool shootAProjectile;
	public bool launchProjectile;

	public bool projectileWithAbility;

	public bool weaponWithAbility;

	public bool setProjectileMeshRotationToFireRotation;

	public bool ignoreShield;

	public bool canActivateReactionSystemTemporally;

	public int damageReactionID = -1;

	public int damageTypeID = -1;

	[Space]
	[Space]

	public bool automatic;

	public bool useBurst;
	public int burstAmount;

	public float fireRate;

	public float projectileDamage;
	public float projectileSpeed;
	public int projectilesPerShoot;

	public bool useProjectileSpread;
	public float thirdPersonSpreadAmountAiming;
	public float thirdPersonSpreadAmountNoAiming;
	public float firstPersonSpreadAmountAiming;
	public float firstPersonSpreadAmountNoAiming;

	[Space]
	[Space]

	public bool isImplosive;
	public bool isExplosive;
	public float explosionForce;
	public float explosionRadius;
	public bool useExplosionDelay;
	public float explosionDelay;
	public float explosionDamage;
	public bool pushCharacters;
	public bool canDamageProjectileOwner = true;
	public bool applyExplosionForceToVehicles = true;
	public float explosionForceToVehiclesMultiplier = 0.2f;

	[Space]
	[Space]

	public List<Transform> projectilePosition = new List<Transform> ();

	public bool checkCrossingSurfacesOnCameraDirection = true;

	public bool applyForceAtShoot;
	public Vector3 forceDirection;
	public float forceAmount;

	public bool isHommingProjectile;
	public bool isSeeker;
	public bool targetOnScreenForSeeker = true;
	public float waitTimeToSearchTarget;

	public float impactForceApplied;
	public ForceMode forceMode;
	public bool applyImpactForceToVehicles;
	public float impactForceToVehiclesMultiplier = 1;

	public float forceMassMultiplier = 1;

	public bool killInOneShot;

	public bool useDisableTimer;
	public float noImpactDisableTimer;
	public float impactDisableTimer;

	public string locatedEnemyIconName = "Homing Located Enemy";
	public List<string> tagToLocate = new List<string> ();

	public bool activateLaunchParableThirdPerson;
	public bool activateLaunchParableFirstPerson;
	public bool useParableSpeed;
	public Transform parableDirectionTransform;
	public bool useMaxDistanceWhenNoSurfaceFound;
	public float maxDistanceWhenNoSurfaceFound;

	public bool checkGravitySystemOnProjectile = true;
	public bool checkCircumnavigationValuesOnProjectile;
	public float gravityForceForCircumnavigationOnProjectile = 9.8f;

	public bool adhereToSurface;
	public bool adhereToLimbs;

	public bool ignoreSetProjectilePositionOnImpact;

	public bool useGravityOnLaunch;
	public bool useGraivtyOnImpact;

	[Space]
	[Space]

	public bool breakThroughObjects;
	public bool infiniteNumberOfImpacts;
	public int numberOfImpacts;
	public bool canDamageSameObjectMultipleTimes;

	public bool damageTargetOverTime;
	public float damageOverTimeDelay;
	public float damageOverTimeDuration;
	public float damageOverTimeAmount;
	public float damageOverTimeRate;
	public bool damageOverTimeToDeath;
	public bool removeDamageOverTimeState;

	public bool sedateCharacters;
	public float sedateDelay;
	public bool useWeakSpotToReduceDelay;
	public bool sedateUntilReceiveDamage;
	public float sedateDuration;

	public bool pushCharacter;
	public float pushCharacterForce;
	public float pushCharacterRagdollForce;

	[Space]
	[Space]

	public bool useNoise;
	public float noiseRadius;
	public float noiseExpandSpeed;
	public bool useNoiseDetection;
	public LayerMask noiseDetectionLayer;
	public bool showNoiseDetectionGizmo;

	public int noiseID = -1;

	public bool useNoiseMesh = true;

	[Range (0, 2)] public float noiseDecibels = 1;

	public bool forceNoiseDetection;

	[Space]
	[Space]

	public bool autoShootOnTag;
	public LayerMask layerToAutoShoot;
	public List<string> autoShootTagList = new List<string> ();
	public float maxDistanceToRaycast;
	public bool shootAtLayerToo;

	public bool avoidShootAtTag;
	public LayerMask layertToAvoidShoot;
	public List<string> avoidShootTagList = new List<string> ();
	public float avoidShootMaxDistanceToRaycast;
	public bool avoidShootAtLayerToo;
	public bool useLowerPositionOnAvoidShoot;

	[Space]
	[Space]

	public GameObject weaponMesh;
	public Transform weaponParent;

	public bool useFireAnimation = true;
	public string animation;
	public float animationSpeed = 1;

	public bool useReloadAnimation;

	public bool useReloadOnThirdPerson = true;
	public bool useReloadOnFirstPerson = true;

	public string reloadAnimationName;
	public float reloadAnimationSpeed;

	public bool playAnimationBackward = true;

	public bool dropClipWhenReload;

	public bool dropClipWhenReloadFirstPerson = true;
	public bool dropClipWhenReloadThirdPerson = true;

	public Transform positionToDropClip;
	public GameObject clipModel;
	public bool autoReloadWhenClipEmpty = true;
	public float delayDropClipWhenReloadThirdPerson;
	public float delayDropClipWhenReloadFirstPerson;

	public Transform magazineInHandTransform;
	public GameObject magazineInHandGameObject;

	public GameObject scorch;
	public float scorchRayCastDistance;

	[Space]
	[Space]

	public float reloadTimeThirdPerson = 1;
	public float reloadTimeFirstPerson = 1;

	public bool useReloadDelayThirdPerson;
	public float reloadDelayThirdPerson;

	public bool useReloadDelayFirstPerson;
	public float reloadDelayFirstPerson;

	public bool usePreReloadDelayThirdPerson;
	public float preReloadDelayThirdPerson;

	public bool usePreReloadDelayFirstPerson;
	public float preReloadDelayFirstPerson;

	public bool shakeUpperBodyWhileShooting;
	public float shakeAmount;
	public float shakeSpeed;

	public bool shakeUpperBodyShootingDualWeapons;
	public float dualWeaponShakeAmount;
	public float dualWeaponShakeSpeed;

	[Space]
	[Space]

	public GameObject shell;
	public List<Transform> shellPosition = new List<Transform> ();
	public float shellEjectionForce = 100;
	public List<AudioClip> shellDropSoundList = new List<AudioClip> ();
	public bool useShellDelay;
	public float shellDelayThirdPerson;
	public float shellDelayFirsPerson;
	public bool createShellsOnReload;
	public bool checkToCreateShellsIfNoRemainAmmo;
	public bool removeDroppedShellsAfterTime = true;
	public float createShellsOnReloadDelayThirdPerson;
	public float createShellsOnReloadDelayFirstPerson;
	public int maxAmountOfShellsBeforeRemoveThem = 15;

	[Space]
	[Space]

	public bool useSoundOnDrawKeepWeapon;
	public AudioClip drawWeaponSound;
	public AudioClip keepWeaponSound;

	public bool useMuzzleFlash;
	public Light muzzleFlahsLight;
	public float muzzleFlahsDuration;

	public AudioClip reloadSoundEffect;
	public AudioClip cockSound;
	public AudioClip shootSoundEffect;
	public AudioClip impactSoundEffect;
	public AudioClip silencerShootEffect;

	public bool useSoundsPool;
	public int maxSoundsPoolAmount;
	public GameObject weaponEffectSourcePrefab;
	public Transform weaponEffectSourceParent;

	public bool setShootParticlesLayerOnFirstPerson;

	public GameObject shootParticles;
	public GameObject projectileParticles;
	public GameObject impactParticles;

	[Space]
	[Space]

	public Texture weaponIcon;

	public Texture weaponInventorySlotIcon;

	public Texture weaponIConHUD;
	public bool showWeaponNameInHUD = true;
	public bool showWeaponIconInHUD = true;
	public bool showWeaponAmmoSliderInHUD = true;
	public bool showWeaponAmmoTextInHUD = true;

	public bool useReticleThirdPerson;
	public bool useAimReticleThirdPerson = true;
	public bool useReticleFirstPerson = true;
	public bool useAimReticleFirstPerson = true;

	public bool useCustomReticle;
	public Texture regularCustomReticle;

	public bool useAimCustomReticle;
	public Texture aimCustomReticle;

	public bool sniperSightThirdPersonEnabled;
	public bool sniperSightFirstPersonEnabled;

	[Space]
	[Space]

	public bool useCanvasHUD = true;
	public Text clipSizeText;
	public Text remainAmmoText;
	public GameObject HUD;
	public GameObject ammoInfoHUD;

	public bool useHUD;
	public bool changeHUDPosition;
	public bool disableHUDInFirstPersonAim;
	public Transform HUDTransformInThirdPerson;
	public Transform HUDTransformInFirstPerson;

	public bool useHUDDualWeaponThirdPerson;
	public bool useHUDDualWeaponFirstPerson;

	public bool changeHUDPositionDualWeapon;
	public Transform HUDRightHandTransformThirdPerson;
	public Transform HUDLeftHandTransformThirdPerson;
	public Transform HUDRightHandTransformFirstPerson;
	public Transform HUDLeftHandTransformFirstPerson;

	[Space]
	[Space]

	public bool useDownButton;
	public UnityEvent downButtonAction;
	public bool useHoldButton;
	public UnityEvent holdButtonAction;
	public bool useUpButton;
	public UnityEvent upButtonAction;

	public bool useEventOnFireWeapon;
	public UnityEvent eventOnFireWeapon;

	public bool useStartDrawAction;
	public UnityEvent startDrawAction;

	public bool useStopDrawAction;
	public UnityEvent stopDrawAction;

	public bool useStartDrawActionThirdPerson;
	public UnityEvent startDrawActionThirdPerson;

	public bool useStopDrawActionThirdPerson;
	public UnityEvent stopDrawActionThirdPerson;

	public bool useStartDrawActionFirstPerson;
	public UnityEvent startDrawActionFirstPerson;

	public bool useStopDrawActionFirstPerson;
	public UnityEvent stopDrawActionFirstPerson;

	public bool useStartAimActionThirdPerson;
	public UnityEvent startAimActionThirdPerson;

	public bool useStopAimActionThirdPerson;
	public UnityEvent stopAimActionThirdPerson;

	public bool useStartAimActionFirstPerson;
	public UnityEvent startAimActionFirstPerson;

	public bool useStopAimActionFirstPerson;
	public UnityEvent stopAimActionFirstPerson;

	public bool showDrawAimFunctionSettings;

	public bool useSecondaryAction;
	public UnityEvent secondaryAction;

	public bool useSecondaryActionOnDownPress;
	public UnityEvent secondaryActionOnDownPress;
	public bool useSecondaryActionOnUpPress;
	public UnityEvent secondaryActionOnUpPress;
	public bool useSecondaryActionOnHoldPress;
	public UnityEvent secondaryActionOnHoldPress;

	public bool useSecondaryActionsOnlyAimingThirdPerson;

	public bool useForwardActionEvent;
	public UnityEvent forwardActionEvent;
	public bool useBackwardActionEvent;
	public UnityEvent backwardActionEvent;

	public bool useReloadEvent;
	public bool useReloadEventThirdPerson;
	public bool useReloadEventFirstPerson;
	public UnityEvent reloadSingleWeaponThirdPersonEvent;
	public UnityEvent reloadDualWeaponThirdPersonEvent;
	public UnityEvent reloadSingleWeaponFirstPersonEvent;
	public UnityEvent reloadDualWeaponFirstPersonEvent;

	public bool useEventOnWeaponActivated;
	public UnityEvent eventOnWeaponActivated;
	public bool useEventOnWeaponDeactivated;
	public UnityEvent eventOnWeaponDeactivated;

	public bool allowDamageForProjectileOwner;

	public bool projectileCanBeDeflected = true;
}