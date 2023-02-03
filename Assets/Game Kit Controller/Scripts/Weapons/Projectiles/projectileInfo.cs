using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class projectileInfo
{
	public bool isHommingProjectile;
	public bool isSeeker;
	public bool targetOnScreenForSeeker = true;
	public float waitTimeToSearchTarget;

	public bool useRaycastCheckingOnRigidbody;

	public float customRaycastCheckingRate;

	public float customRaycastCheckingDistance = 0.1f;

	public bool useRayCastShoot;

	public bool useRaycastShootDelay;
	public float raycastShootDelay;
	public bool getDelayWithDistance;
	public float delayWithDistanceSpeed;
	public float maxDelayWithDistance;

	public bool useFakeProjectileTrails;

	public float projectileDamage;
	public float projectileSpeed;
	public float impactForceApplied;
	public ForceMode forceMode;
	public bool applyImpactForceToVehicles;
	public float impactForceToVehiclesMultiplier;

	public float forceMassMultiplier = 1;

	public bool projectileWithAbility;
	public AudioClip impactSoundEffect;
	public GameObject scorch;
	public GameObject target;
	public GameObject owner;
	public GameObject projectileParticles;
	public GameObject impactParticles;

	public bool isExplosive;
	public bool isImplosive;
	public float explosionForce;
	public float explosionRadius;
	public bool useExplosionDelay;
	public float explosionDelay;
	public float explosionDamage;
	public bool pushCharacters;
	public bool canDamageProjectileOwner;
	public bool applyExplosionForceToVehicles;
	public float explosionForceToVehiclesMultiplier;

	public bool killInOneShot;

	public bool useDisableTimer;
	public float noImpactDisableTimer;
	public float impactDisableTimer;

	public bool useCustomIgnoreTags;
	public List<string> customTagsToIgnoreList = new List<string> ();

	public LayerMask targetToDamageLayer;

	public LayerMask targetForScorchLayer;

	public float scorchRayCastDistance;

	public int impactDecalIndex;

	public bool launchProjectile;

	public bool adhereToSurface;
	public bool adhereToLimbs;

	public bool ignoreSetProjectilePositionOnImpact;

	public bool useGravityOnLaunch;
	public bool useGraivtyOnImpact;

	public bool breakThroughObjects;
	public bool infiniteNumberOfImpacts;
	public int numberOfImpacts;
	public bool canDamageSameObjectMultipleTimes;
	public Vector3 forwardDirection;

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

	public bool setProjectileMeshRotationToFireRotation;

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameList = new List<string> ();

	public bool useRemoteEventOnObjectsFoundOnExplosion;
	public string remoteEventNameOnExplosion;

	public bool ignoreShield;

	public bool canActivateReactionSystemTemporally;

	public int damageReactionID = -1;

	public int damageTypeID = -1;

	public bool projectilesPoolEnabled;

	public int maxAmountOfPoolElementsOnWeapon;

	public bool allowDamageForProjectileOwner;

	public bool projectileCanBeDeflected = true;
}