using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class simpleWeaponSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public weaponInfo weaponSettings;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public AudioClip outOfAmmo;
	public GameObject weaponProjectile;
	public LayerMask layer;

	public bool projectilesPoolEnabled = true;

	public int maxAmountOfPoolElementsOnWeapon = 30;

	public LayerMask targetToDamageLayer;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool sendProjectileEventOnFire;
	public eventParameters.eventToCallWithGameObject projectileEventOnFire;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameList = new List<string> ();

	public bool useRemoteEventOnObjectsFoundOnExplosion;
	public string remoteEventNameOnExplosion;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool reloading;
	public bool usingWeapon;

	public bool externalProjectileToFireAssigned;
	public GameObject externalProjectileToFire;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform mainCameraTransform;

	public AudioSource weaponsEffectsSource;

	public Camera mainCamera;

	public Animation weaponAnimation;

	public Collider playerCollider;

	public GameObject playerControllerGameObject;
	public GameObject playerCameraGameObject;
	public playerWeaponsManager mainPlayerWeaponsManager;

	public string mainDecalManagerName = "Decal Manager";

	public string mainNoiseMeshManagerName = "Noise Mesh Manager";

	public string[] impactDecalList;
	public int impactDecalIndex;
	public string impactDecalName;
	public bool getImpactListEveryFrame;

	decalManager impactDecalManager;

	List<GameObject> shells = new List<GameObject> ();
	float destroyShellsTimer = 0;
	RaycastHit hit;
	RaycastHit hitCamera;
	RaycastHit hitWeapon;
	float lastShoot;

	bool animationForwardPlayed;
	bool animationBackPlayed;

	bool shellCreated;

	bool weaponHasAnimation;

	Vector3 forceDirection;
	GameObject closestEnemy;

	float weaponSpeed = 1;
	float originalWeaponSpeed;

	Vector3 aimedZone;
	bool objectiveFound;

	bool usingSight;

	Coroutine muzzleFlashCoroutine;

	bool usingScreenSpaceCamera;
	bool targetOnScreen;
	Vector3 screenPoint;

	Rigidbody currentProjectileRigidbody;

	GameObject currentProjectile;

	projectileInfo newProjectileInfo;

	float screenWidth;
	float screenHeight;

	Transform currentProjectilePosition;

	projectileSystem currentProjectileSystem;

	List<projectileSystem> lastProjectilesSystemListFired = new List<projectileSystem> ();

	GameObject newShellClone;
	weaponShellSystem newWeaponShellSystem;
	AudioClip newClipToShell;
	GameObject currentShellToRemove;
	bool shellsActive;
	bool shellsOnSceneToRemove;

	GameObject newMuzzleParticlesClone;
	Coroutine reloadCoroutine;

	noiseMeshSystem noiseMeshManager;

	bool noiseMeshManagerFound;

	float currentRaycastDistance;

	bool surfaceFound;
	RaycastHit surfaceFoundHit;

	Coroutine holdShootWeaponCoroutine;

	bool holdShootActive;


	void Start ()
	{
		weaponSettings.ammoPerClip = weaponSettings.clipSize;

		if (weaponSettings.startWithEmptyClip) {
			weaponSettings.clipSize = 0;
		}

		if (weaponSettings.animation != "") {
			weaponHasAnimation = true;
			weaponAnimation [weaponSettings.animation].speed = weaponSettings.animationSpeed; 
		} else {
			shellCreated = true;
		}

		originalWeaponSpeed = weaponSpeed;

		weaponSettings.auxRemainAmmo = weaponSettings.remainAmmo;

		if (mainPlayerWeaponsManager != null) {
			usingScreenSpaceCamera = mainPlayerWeaponsManager.isUsingScreenSpaceCamera ();
		}

		currentRaycastDistance = Mathf.Infinity;
	}

	void Update ()
	{
		if (!shellCreated && ((weaponHasAnimation && animationForwardPlayed && !weaponAnimation.IsPlaying (weaponSettings.animation)) || !weaponHasAnimation)) {
			createShells ();
		}

		if (!reloading) {
			if (weaponHasAnimation) {
				if (weaponSettings.clipSize > 0) {
					if (weaponSettings.playAnimationBackward) {
						if (animationForwardPlayed && !weaponAnimation.IsPlaying (weaponSettings.animation)) {
							animationForwardPlayed = false;
							animationBackPlayed = true;

							weaponAnimation [weaponSettings.animation].speed = -weaponSettings.animationSpeed; 

							weaponAnimation [weaponSettings.animation].time = weaponAnimation [weaponSettings.animation].length;

							weaponAnimation.Play (weaponSettings.animation);
						}
						if (animationBackPlayed && !weaponAnimation.IsPlaying (weaponSettings.animation)) {
							animationBackPlayed = false;
						}
					} else {
						animationForwardPlayed = false;
						animationBackPlayed = false;
					}
				} else if ((weaponSettings.remainAmmo > 0 || weaponSettings.infiniteAmmo) && weaponSettings.autoReloadWhenClipEmpty) {
					reloadWeapon (weaponSettings.reloadDelayThirdPerson);
				}
			} else {
				if (weaponSettings.clipSize == 0) {
					if ((weaponSettings.remainAmmo > 0 || weaponSettings.infiniteAmmo) && weaponSettings.autoReloadWhenClipEmpty) {
						reloadWeapon (weaponSettings.reloadDelayThirdPerson);
					}
				}
			}
		} 

		checkDroppedShellsRemoval ();
	}

	public void shootWeaponExternally ()
	{
		inputShootWeaponOnPressDown ();

		inputShootWeaponOnPressUp ();
	}

	void setHoldShootWeaponState (bool state)
	{
		stopUpdateHoldShootWeaponCoroutine ();

		if (state) {
			holdShootWeaponCoroutine = StartCoroutine (updateHoldShootWeaponCoroutine ());

			holdShootActive = true;
		}
	}

	public void stopUpdateHoldShootWeaponCoroutine ()
	{
		if (holdShootWeaponCoroutine != null) {
			StopCoroutine (holdShootWeaponCoroutine);
		}

		if (holdShootActive) {
			shootWeapon (false);
		}

		holdShootActive = false;
	}

	IEnumerator updateHoldShootWeaponCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			shootWeapon (true);
		}
	}


	public void inputHoldOrReleaseShootWeapon (bool holdingButton)
	{
		if (holdingButton) {
			setHoldShootWeaponState (true);
		} else {
			setHoldShootWeaponState (false);
		}
	}

	public void inputShootWeaponOnPressDown ()
	{
		shootWeapon (true);
	}

	public void inputShootWeaponOnPressUp ()
	{
		shootWeapon (false);
	}

	public void inputShootWeaponOnPress ()
	{
		if (weaponSettings.automatic) {
			shootWeapon (true);
		}
	}

	public void shootWeapon (bool state)
	{
		if (state) {
			shootWeapon (!mainPlayerWeaponsManager.isFirstPersonActive (), state);
		} else {
			shootWeapon (!mainPlayerWeaponsManager.isFirstPersonActive (), state);
		}
	}

	public void addExternalProjectileToFire (GameObject newProjectile)
	{
		externalProjectileToFire = newProjectile;

		externalProjectileToFireAssigned = externalProjectileToFire != null;
	}

	public void removeExternalProjectileToFire (GameObject newProjectile)
	{
		if (externalProjectileToFire != null && externalProjectileToFire == newProjectile) {
			addExternalProjectileToFire (null);
		}
	}

	public void sendRemoteEventToExternalProjectile (string remoteEventName)
	{
		if (externalProjectileToFire != null) {
			externalProjectileToFire.GetComponent<projectileSystem> ().checkRemoteEventOnProjectile (remoteEventName);
		}
	}
		
	//fire the current weapon
	public void shootWeapon (bool isThirdPersonView, bool shootAtKeyDown)
	{
		checkWeaponAbility (shootAtKeyDown);

		//if the weapon system is active and the clip size higher than 0
		if (weaponSettings.clipSize > 0) {
			//else, fire the current weapon according to the fire rate
			if (Time.time > lastShoot + weaponSettings.fireRate) {

				if ((animationForwardPlayed || animationBackPlayed) && weaponHasAnimation) {
					return;
				}

				checkWeaponShootNoise ();

				//play the fire sound
				playWeaponSoundEffect (true);

				//enable the muzzle flash light
				enableMuzzleFlashLight ();

				Vector3 mainCameraPosition = mainCameraTransform.position;

				//create the muzzle flash
				createMuzzleFlash ();

				bool weaponCrossingSurface = false;

				if (!isThirdPersonView) {
					if (weaponSettings.checkCrossingSurfacesOnCameraDirection) {
						if (weaponSettings.projectilePosition.Count > 0) {
							if (Physics.Raycast (mainCameraPosition, mainCameraTransform.TransformDirection (Vector3.forward), out hitCamera, currentRaycastDistance, layer) &&
							    Physics.Raycast (weaponSettings.projectilePosition [0].position, mainCameraTransform.TransformDirection (Vector3.forward), out hitWeapon, currentRaycastDistance, layer)) {
								if (hitCamera.collider != hitWeapon.collider) {
									//print ("too close surface");
									weaponCrossingSurface = true;
								} 
							}
						}
					}
				}

				//play the fire animation
				if (weaponHasAnimation) {
					weaponAnimation [weaponSettings.animation].speed = weaponSettings.animationSpeed;

					weaponAnimation.Play (weaponSettings.animation);

					animationForwardPlayed = true;

					if (weaponSettings.cockSound != null) {
						playSound (weaponSettings.cockSound);
					}
				} 

				shellCreated = false;

				Vector3 cameraDirection = mainCameraTransform.TransformDirection (Vector3.forward);

				lastProjectilesSystemListFired.Clear ();

				//every weapon can shoot 1 or more projectiles at the same time, so for every projectile position to instantiate
				for (int j = 0; j < weaponSettings.projectilePosition.Count; j++) {
					currentProjectilePosition = weaponSettings.projectilePosition [j];

					for (int l = 0; l < weaponSettings.projectilesPerShoot; l++) {

						surfaceFound = false;

						//create the projectile
						if (externalProjectileToFireAssigned) {
							if (externalProjectileToFire != null) {
								externalProjectileToFire.transform.position = currentProjectilePosition.position;

								externalProjectileToFire.transform.rotation = currentProjectilePosition.rotation;

								currentProjectile = externalProjectileToFire;
							} else {
								externalProjectileToFireAssigned = false;
							}
						}

						if (!externalProjectileToFireAssigned) {
							if (projectilesPoolEnabled) {
								currentProjectile = GKC_PoolingSystem.Spawn (weaponProjectile, currentProjectilePosition.position, mainCameraTransform.rotation, maxAmountOfPoolElementsOnWeapon);
							} else {
								currentProjectile = (GameObject)Instantiate (weaponProjectile, currentProjectilePosition.position, mainCameraTransform.rotation);
							}
						}

						//set the info in the projectile, like the damage, the type of projectile, bullet or missile, etc...
						currentProjectileSystem = currentProjectile.GetComponent<projectileSystem> ();

						if (externalProjectileToFireAssigned) {
							currentProjectileSystem.checkEventOnProjectileFiredExternally ();

							externalProjectileToFire = null;

							externalProjectileToFireAssigned = false;
						}

						currentProjectileSystem.checkToResetProjectile ();

						lastProjectilesSystemListFired.Add (currentProjectileSystem);

						if (!weaponSettings.launchProjectile) {
							//set its direction in the weapon forward or the camera forward according to if the weapon is aimed correctly or not
							if (!weaponCrossingSurface) {
								if (weaponSettings.fireWeaponForward) {
									currentProjectile.transform.rotation = currentProjectilePosition.rotation;
								} else {

									if (Physics.Raycast (mainCameraPosition, cameraDirection, out hit, currentRaycastDistance, layer)) {
										if (!hit.collider.isTrigger) {
											if (hit.collider != playerCollider) {
												//Debug.DrawLine (currentProjectilePosition.position, hit.point, Color.red, 2);

												currentProjectile.transform.LookAt (hit.point);

												surfaceFound = true;
												surfaceFoundHit = hit;
											} else {
												if (Physics.Raycast (hit.point + cameraDirection * 0.2f, cameraDirection, out hit, currentRaycastDistance, layer)) {
													currentProjectile.transform.LookAt (hit.point);

													surfaceFound = true;
													surfaceFoundHit = hit;
												}
											}
										}
									}
								}
							}
						}

						if (weaponSettings.launchProjectile) {
							if (currentProjectileSystem != null) {
								currentProjectileRigidbody = currentProjectileSystem.getProjectileRigibody ();
							}

							if (currentProjectileRigidbody == null) {
								currentProjectileRigidbody = currentProjectile.GetComponent<Rigidbody> ();
							}

							currentProjectileRigidbody.isKinematic = true;
							// If the vehicle has a gravity control component, and the current gravity is not the regular one,
							// add an artificial gravity component to the projectile like this, it can make a parable in any
							// surface and direction, setting its gravity in the same of the vehicle.

							Vector3 currentNormal = mainPlayerWeaponsManager.getCurrentNormal ();

							if (currentNormal != Vector3.up) {
								currentProjectile.AddComponent<artificialObjectGravity> ().setCurrentGravity (-currentNormal);
							}

							if (weaponSettings.useParableSpeed) {
								//get the ray hit point where the projectile will fall

								//get the ray hit point where the projectile will fall
								if (surfaceFound) {
									aimedZone = surfaceFoundHit.point;

									objectiveFound = true;
								} else {
									if (Physics.Raycast (mainCameraPosition, cameraDirection, out hit, currentRaycastDistance, layer)) {
										if (hit.collider != playerCollider) {
											aimedZone = hit.point;

											objectiveFound = true;
										} else {
											if (Physics.Raycast (hit.point + cameraDirection * 0.2f, cameraDirection, out hit, currentRaycastDistance, layer)) {
												aimedZone = hit.point;

												objectiveFound = true;
											}
										}
									} else {
										objectiveFound = false;
									}
								}
							}

							launchCurrentProjectile (currentProjectile, currentProjectileRigidbody, cameraDirection);
						}

						//add spread to the projectile
						Vector3 spreadAmount = Vector3.zero;

						if (weaponSettings.useProjectileSpread) {
							spreadAmount = setProjectileSpread ();
							currentProjectile.transform.Rotate (spreadAmount);
						}

						currentProjectileSystem.setProjectileInfo (setProjectileInfo ());

						if (weaponSettings.isSeeker) {
							closestEnemy = setSeekerProjectileInfo (currentProjectilePosition.position);

							if (closestEnemy != null) {
								currentProjectileSystem.setEnemy (closestEnemy);
							}
						}

						bool projectileFiredByRaycast = false;

						bool projectileDestroyed = false;

						//if the weapon shoots setting directly the projectile in the hit point, place the current projectile in the hit point position
						if (weaponSettings.useRayCastShoot || weaponCrossingSurface) {
							Vector3 forwardDirection = mainCameraTransform.TransformDirection (Vector3.forward);
							Vector3 forwardPosition = mainCameraPosition;

							if (weaponSettings.fireWeaponForward && !weaponCrossingSurface) {
								forwardDirection = transform.forward;
								forwardPosition = currentProjectilePosition.position;
							}

							if (spreadAmount.magnitude != 0) {
								forwardDirection = Quaternion.Euler (spreadAmount) * forwardDirection;
							}
								
							if (Physics.Raycast (forwardPosition, forwardDirection, out hit, currentRaycastDistance, layer)) {
								if (hit.collider != playerCollider) {
									if (weaponSettings.useFakeProjectileTrails) {
										currentProjectileSystem.creatFakeProjectileTrail (hit.point);
									}

									currentProjectileSystem.rayCastShoot (hit.collider, hit.point, forwardDirection);

									projectileFiredByRaycast = true;
								} else {
									if (Physics.Raycast (hit.point + forwardDirection * 0.2f, forwardDirection, out hit, currentRaycastDistance, layer)) {

										if (weaponSettings.useFakeProjectileTrails) {
											currentProjectileSystem.creatFakeProjectileTrail (hit.point);
										}

										currentProjectileSystem.rayCastShoot (hit.collider, hit.point, forwardDirection);

										projectileFiredByRaycast = true;
									}
								}
								//print ("same object fired: " + hit.collider.name);
							} else {
								currentProjectileSystem.initializeProjectile ();

								currentProjectileSystem.destroyProjectile ();

								projectileDestroyed = true;
							}
						}

						if (!projectileFiredByRaycast && !projectileDestroyed) {
							currentProjectileSystem.initializeProjectile ();
						}
					}

					if (!weaponSettings.infiniteAmmo) {
						useAmmo ();
					}

					lastShoot = Time.time;
					destroyShellsTimer = 0;

					checkSendProjectileOnEventFire ();
				}

				if (weaponSettings.weaponWithAbility) {
					lastShoot = Time.time;
					destroyShellsTimer = 0;
				}
			}
		} 
		//else, the clip in the weapon is over, so check if there is remaining ammo
		else {
			if (weaponSettings.remainAmmo == 0 && !weaponSettings.infiniteAmmo) {
				playWeaponSoundEffect (false);
			}
		}
	}

	public void checkWeaponAbility (bool keyDown)
	{
		if (weaponSettings.weaponWithAbility) {
			if (keyDown) {
				if (weaponSettings.useDownButton) {
					if (weaponSettings.downButtonAction.GetPersistentEventCount () > 0) {
						weaponSettings.downButtonAction.Invoke ();
					}
				}
			} else {
				if (weaponSettings.useUpButton) {
					if (weaponSettings.upButtonAction.GetPersistentEventCount () > 0) {
						weaponSettings.upButtonAction.Invoke ();
					}
				}
			}
		}
	}

	public projectileInfo setProjectileInfo ()
	{
		newProjectileInfo = new projectileInfo ();

		newProjectileInfo.isHommingProjectile = weaponSettings.isHommingProjectile;

		newProjectileInfo.isSeeker = weaponSettings.isSeeker;
		newProjectileInfo.targetOnScreenForSeeker = weaponSettings.targetOnScreenForSeeker;

		newProjectileInfo.waitTimeToSearchTarget = weaponSettings.waitTimeToSearchTarget;
		newProjectileInfo.useRayCastShoot = weaponSettings.useRayCastShoot;

		newProjectileInfo.useRaycastShootDelay = weaponSettings.useRaycastShootDelay;
		newProjectileInfo.raycastShootDelay = weaponSettings.raycastShootDelay;
		newProjectileInfo.getDelayWithDistance = weaponSettings.getDelayWithDistance;
		newProjectileInfo.delayWithDistanceSpeed = weaponSettings.delayWithDistanceSpeed;
		newProjectileInfo.maxDelayWithDistance = weaponSettings.maxDelayWithDistance;

		newProjectileInfo.useFakeProjectileTrails = weaponSettings.useFakeProjectileTrails;

		newProjectileInfo.useRaycastCheckingOnRigidbody = weaponSettings.useRaycastCheckingOnRigidbody;
		newProjectileInfo.customRaycastCheckingRate = weaponSettings.customRaycastCheckingRate;
		newProjectileInfo.customRaycastCheckingDistance = weaponSettings.customRaycastCheckingDistance;

		newProjectileInfo.projectileDamage = weaponSettings.projectileDamage;
		newProjectileInfo.projectileSpeed = weaponSettings.projectileSpeed;

		newProjectileInfo.impactForceApplied = weaponSettings.impactForceApplied;
		newProjectileInfo.forceMode = weaponSettings.forceMode;
		newProjectileInfo.applyImpactForceToVehicles = weaponSettings.applyImpactForceToVehicles;
		newProjectileInfo.impactForceToVehiclesMultiplier = weaponSettings.impactForceToVehiclesMultiplier;

		newProjectileInfo.projectileWithAbility = weaponSettings.projectileWithAbility;

		newProjectileInfo.impactSoundEffect = weaponSettings.impactSoundEffect;

		newProjectileInfo.scorch = weaponSettings.scorch;
		newProjectileInfo.scorchRayCastDistance = weaponSettings.scorchRayCastDistance;

		newProjectileInfo.owner = playerControllerGameObject;

		newProjectileInfo.projectileParticles = weaponSettings.projectileParticles;
		newProjectileInfo.impactParticles = weaponSettings.impactParticles;

		newProjectileInfo.isExplosive = weaponSettings.isExplosive;
		newProjectileInfo.isImplosive = weaponSettings.isImplosive;
		newProjectileInfo.useExplosionDelay = weaponSettings.useExplosionDelay;
		newProjectileInfo.explosionDelay = weaponSettings.explosionDelay;
		newProjectileInfo.explosionForce = weaponSettings.explosionForce;
		newProjectileInfo.explosionRadius = weaponSettings.explosionRadius;
		newProjectileInfo.explosionDamage = weaponSettings.explosionDamage;
		newProjectileInfo.pushCharacters = weaponSettings.pushCharacters;
		newProjectileInfo.canDamageProjectileOwner = weaponSettings.canDamageProjectileOwner;
		newProjectileInfo.applyExplosionForceToVehicles = weaponSettings.applyExplosionForceToVehicles;
		newProjectileInfo.explosionForceToVehiclesMultiplier = weaponSettings.explosionForceToVehiclesMultiplier;

		newProjectileInfo.killInOneShot = weaponSettings.killInOneShot;

		newProjectileInfo.useDisableTimer = weaponSettings.useDisableTimer;
		newProjectileInfo.noImpactDisableTimer = weaponSettings.noImpactDisableTimer;
		newProjectileInfo.impactDisableTimer = weaponSettings.impactDisableTimer;

		newProjectileInfo.targetToDamageLayer = targetToDamageLayer;
		newProjectileInfo.targetForScorchLayer = mainPlayerWeaponsManager.targetForScorchLayer;

		newProjectileInfo.impactDecalIndex = impactDecalIndex;

		newProjectileInfo.launchProjectile = weaponSettings.launchProjectile;

		newProjectileInfo.adhereToSurface = weaponSettings.adhereToSurface;
		newProjectileInfo.adhereToLimbs = weaponSettings.adhereToLimbs;
		newProjectileInfo.ignoreSetProjectilePositionOnImpact = weaponSettings.ignoreSetProjectilePositionOnImpact;

		newProjectileInfo.useGravityOnLaunch = weaponSettings.useGravityOnLaunch;
		newProjectileInfo.useGraivtyOnImpact = weaponSettings.useGraivtyOnImpact;

		newProjectileInfo.breakThroughObjects = weaponSettings.breakThroughObjects;
		newProjectileInfo.infiniteNumberOfImpacts = weaponSettings.infiniteNumberOfImpacts;
		newProjectileInfo.numberOfImpacts = weaponSettings.numberOfImpacts;
		newProjectileInfo.canDamageSameObjectMultipleTimes = weaponSettings.canDamageSameObjectMultipleTimes;
		newProjectileInfo.forwardDirection = mainCameraTransform.forward;

		newProjectileInfo.damageTargetOverTime = weaponSettings.damageTargetOverTime;
		newProjectileInfo.damageOverTimeDelay = weaponSettings.damageOverTimeDelay;
		newProjectileInfo.damageOverTimeDuration = weaponSettings.damageOverTimeDuration;
		newProjectileInfo.damageOverTimeAmount = weaponSettings.damageOverTimeAmount;
		newProjectileInfo.damageOverTimeRate = weaponSettings.damageOverTimeRate;
		newProjectileInfo.damageOverTimeToDeath = weaponSettings.damageOverTimeToDeath;
		newProjectileInfo.removeDamageOverTimeState = weaponSettings.removeDamageOverTimeState;

		newProjectileInfo.sedateCharacters = weaponSettings.sedateCharacters;
		newProjectileInfo.sedateDelay = weaponSettings.sedateDelay;
		newProjectileInfo.useWeakSpotToReduceDelay = weaponSettings.useWeakSpotToReduceDelay;
		newProjectileInfo.sedateDuration = weaponSettings.sedateDuration;
		newProjectileInfo.sedateUntilReceiveDamage = weaponSettings.sedateUntilReceiveDamage;

		newProjectileInfo.pushCharacter = weaponSettings.pushCharacter;
		newProjectileInfo.pushCharacterForce = weaponSettings.pushCharacterForce;
		newProjectileInfo.pushCharacterRagdollForce = weaponSettings.pushCharacterRagdollForce;

		newProjectileInfo.setProjectileMeshRotationToFireRotation = weaponSettings.setProjectileMeshRotationToFireRotation;

		newProjectileInfo.useRemoteEventOnObjectsFound = useRemoteEventOnObjectsFound;
		newProjectileInfo.remoteEventNameList = remoteEventNameList;

		newProjectileInfo.useRemoteEventOnObjectsFoundOnExplosion = useRemoteEventOnObjectsFoundOnExplosion;
		newProjectileInfo.remoteEventNameOnExplosion = remoteEventNameOnExplosion;

		newProjectileInfo.ignoreShield = weaponSettings.ignoreShield;

		newProjectileInfo.canActivateReactionSystemTemporally = weaponSettings.canActivateReactionSystemTemporally;
		newProjectileInfo.damageReactionID = weaponSettings.damageReactionID;

		newProjectileInfo.damageTypeID = weaponSettings.damageTypeID;

		newProjectileInfo.projectilesPoolEnabled = projectilesPoolEnabled;

		newProjectileInfo.maxAmountOfPoolElementsOnWeapon = maxAmountOfPoolElementsOnWeapon;

		newProjectileInfo.projectileCanBeDeflected = weaponSettings.projectileCanBeDeflected;

		return newProjectileInfo;
	}

	public GameObject setSeekerProjectileInfo (Vector3 shootPosition)
	{
		return applyDamage.setSeekerProjectileInfo (shootPosition, weaponSettings.tagToLocate, usingScreenSpaceCamera, 
			weaponSettings.targetOnScreenForSeeker, mainCamera, targetToDamageLayer, transform.position, false, null);
	}

	public Vector3 setProjectileSpread ()
	{
		float spreadAmount = weaponSettings.thirdPersonSpreadAmountAiming;

		if (spreadAmount > 0) {
			Vector3 randomSpread = Vector3.zero;
			randomSpread.x = Random.Range (-spreadAmount, spreadAmount);
			randomSpread.y = Random.Range (-spreadAmount, spreadAmount);
			randomSpread.z = Random.Range (-spreadAmount, spreadAmount);
			return randomSpread;
		}

		return Vector3.zero;
	}

	void createShells ()
	{
		if (weaponSettings.shell != null) {
			for (int j = 0; j < weaponSettings.shellPosition.Count; j++) {
				//if the current weapon drops shells, create them

				if (projectilesPoolEnabled) {
					newShellClone = GKC_PoolingSystem.Spawn (weaponSettings.shell, weaponSettings.shellPosition [j].position, weaponSettings.shellPosition [j].rotation, maxAmountOfPoolElementsOnWeapon);
				} else {
					newShellClone = (GameObject)Instantiate (weaponSettings.shell, weaponSettings.shellPosition [j].position, weaponSettings.shellPosition [j].rotation);
				}

				newWeaponShellSystem = newShellClone.GetComponent<weaponShellSystem> ();

				if (weaponSettings.shellDropSoundList.Count > 0) {
					newClipToShell = weaponSettings.shellDropSoundList [Random.Range (0, weaponSettings.shellDropSoundList.Count - 1)];
				}

				newWeaponShellSystem.setShellValues (weaponSettings.shellPosition [j].right * (weaponSettings.shellEjectionForce * GKC_Utils.getCurrentScaleTime ()), playerCollider, newClipToShell);

				if (weaponSettings.removeDroppedShellsAfterTime) {
					shells.Add (newShellClone);

					if (shells.Count > weaponSettings.maxAmountOfShellsBeforeRemoveThem) {
						currentShellToRemove = shells [0];

						shells.RemoveAt (0);

						if (projectilesPoolEnabled) {
							GKC_PoolingSystem.Despawn (currentShellToRemove);
						} else {
							Destroy (currentShellToRemove);
						}
					}

					shellsActive = true;

					shellsOnSceneToRemove = true;
				}
			}
		}
	}

	public void checkDroppedShellsRemoval ()
	{
		//if the amount of shells from the projectiles is higher than 0, check the time to remove then
		if (shellsOnSceneToRemove && weaponSettings.removeDroppedShellsAfterTime) {
			if (shellsActive) {
				destroyShellsTimer += Time.deltaTime;

				if (destroyShellsTimer > 3) {
					for (int i = 0; i < shells.Count; i++) {
						if (projectilesPoolEnabled) {
							GKC_PoolingSystem.Despawn (shells [i]);
						} else {
							Destroy (shells [i]);
						}
					}

					shells.Clear ();

					destroyShellsTimer = 0;

					shellsOnSceneToRemove = false;

					shellsActive = false;
				}
			}
		}
	}

	//play the fire sound or the empty clip sound
	void playWeaponSoundEffect (bool hasAmmo)
	{
		if (weaponsEffectsSource != null) {
			if (hasAmmo) {
				if (weaponSettings.shootSoundEffect != null) {
					weaponsEffectsSource.clip = weaponSettings.shootSoundEffect;

					weaponsEffectsSource.pitch = weaponSpeed;
					weaponsEffectsSource.Play ();
				}
			} else {
				if (Time.time > lastShoot + weaponSettings.fireRate) {
					weaponsEffectsSource.pitch = weaponSpeed;

					GKC_Utils.checkAudioSourcePitch (weaponsEffectsSource);

					weaponsEffectsSource.PlayOneShot (outOfAmmo);
					lastShoot = Time.time;
				}
			}
		} 

//		else {
//			print ("WARNING: no audio source attached on " + gameObject.name + " weapon");
//		}
	}

	public void playSound (AudioClip clipSound)
	{
		if (weaponsEffectsSource != null) {
			GKC_Utils.checkAudioSourcePitch (weaponsEffectsSource);

			weaponsEffectsSource.PlayOneShot (clipSound);
		}
	}

	//create the muzzle flash particles if the weapon has it
	void createMuzzleFlash ()
	{
		if (weaponSettings.shootParticles != null) {
			for (int j = 0; j < weaponSettings.projectilePosition.Count; j++) {
				currentProjectilePosition = weaponSettings.projectilePosition [j];

				if (projectilesPoolEnabled) {
					newMuzzleParticlesClone = GKC_PoolingSystem.Spawn (weaponSettings.shootParticles, currentProjectilePosition.position, currentProjectilePosition.rotation, maxAmountOfPoolElementsOnWeapon);
				} else {
					newMuzzleParticlesClone = (GameObject)Instantiate (weaponSettings.shootParticles, currentProjectilePosition.position, currentProjectilePosition.rotation);
				}

				if (weaponSettings.setShootParticlesLayerOnFirstPerson) {
					if (mainPlayerWeaponsManager.isFirstPersonActive ()) {
						mainPlayerWeaponsManager.setWeaponPartLayer (newMuzzleParticlesClone);
					}
				}

				if (projectilesPoolEnabled) {

				} else {
					Destroy (newMuzzleParticlesClone, 1);	
				}

				newMuzzleParticlesClone.transform.SetParent (currentProjectilePosition);
			}
		}
	}

	//	//decrease the amount of ammo in the clip
	public void useAmmo ()
	{
		weaponSettings.clipSize--;
	}

	public void useAmmo (int amount)
	{
		if (amount > weaponSettings.clipSize) {
			amount = weaponSettings.clipSize;
		}

		weaponSettings.clipSize -= amount;
	}

	/// <summary>
	/// Checks and handles the amount of remaining ammo.
	/// </summary>
	void checkRemainingAmmo ()
	{
		// If the weapon has not infinite ammo
		if (!weaponSettings.infiniteAmmo) {
			//the clip is empty
			if (weaponSettings.clipSize == 0) {
				//if the remaining ammo is lower that the ammo per clip, set the final projectiles in the clip 
				if (weaponSettings.remainAmmo < weaponSettings.ammoPerClip) {
					weaponSettings.clipSize = weaponSettings.remainAmmo;
				} 
				//else, refill it
				else {
					weaponSettings.clipSize = weaponSettings.ammoPerClip;
				}

				//if the remaining ammo is higher than 0, remove the current projectiles added in the clip
				if (weaponSettings.remainAmmo > 0) {
					weaponSettings.remainAmmo -= weaponSettings.clipSize;
				} 
			} 
			//the clip has some bullets in it yet
			else {
				int usedAmmo = 0;

				if (weaponSettings.removePreviousAmmoOnClip) {
					weaponSettings.clipSize = 0;

					if (weaponSettings.remainAmmo < (weaponSettings.ammoPerClip)) {
						usedAmmo = weaponSettings.remainAmmo;
					} else {
						usedAmmo = weaponSettings.ammoPerClip;
					}
				} else {
					if (weaponSettings.remainAmmo < (weaponSettings.ammoPerClip - weaponSettings.clipSize)) {
						usedAmmo = weaponSettings.remainAmmo;
					} else {
						usedAmmo = weaponSettings.ammoPerClip - weaponSettings.clipSize;
					}
				}

				weaponSettings.remainAmmo -= usedAmmo;
				weaponSettings.clipSize += usedAmmo;
			}
		} else {
			// else, the weapon has infinite ammo, so refill it
			weaponSettings.clipSize = weaponSettings.ammoPerClip;
		}

		weaponSettings.auxRemainAmmo = weaponSettings.remainAmmo;
	}

	public void reloadWeapon (float waitTimeAmount)
	{
		if (reloadCoroutine != null) {
			StopCoroutine (reloadCoroutine);
		}

		reloadCoroutine = StartCoroutine (waitToReload (waitTimeAmount));
	}

	/// <summary>
	/// Adds a delay when reloading weapons for the given amount of seconds.
	/// </summary>
	/// <param name="waitTimeAmount">Number of seconds to wait before reloading.</param>
	IEnumerator waitToReload (float waitTimeAmount)
	{
		//print ("reload");
		// If the remaining ammo is higher than 0 or infinite
		if (weaponSettings.remainAmmo > 0 || weaponSettings.infiniteAmmo) {
			// Reload
			reloading = true;

			// Play the reload sound
			if (weaponSettings.reloadSoundEffect != null) {
				playSound (weaponSettings.reloadSoundEffect);
			}

			if (weaponSettings.dropClipWhenReload) {
				GameObject newClipToDrop = (GameObject)Instantiate (weaponSettings.clipModel, weaponSettings.positionToDropClip.position, weaponSettings.positionToDropClip.rotation);
				Collider newClipCollider = newClipToDrop.GetComponent<Collider> ();

				if (newClipCollider != null) {
					Physics.IgnoreCollision (playerCollider, newClipCollider);
				}
			}

			// Wait an amount of time
			yield return new WaitForSeconds (waitTimeAmount);

			// Check the ammo values
			checkRemainingAmmo ();

			// Stop reload
			reloading = false;
		} else {
			// else, the weapon is out of ammo, play the empty weapon sound
			playWeaponSoundEffect (false);
		}

		yield return null;
	}

	public void manualReload ()
	{
		if (!reloading) {
			if (weaponSettings.clipSize < weaponSettings.ammoPerClip) {
				reloadWeapon (weaponSettings.reloadDelayThirdPerson);
			}
		}
	}

	public bool canIncreaseRemainAmmo ()
	{
		if (weaponSettings.auxRemainAmmo < weaponSettings.ammoLimit) {
			return true;
		} else {
			return false;
		}
	}

	public bool hasMaximumAmmoAmount ()
	{
		if (weaponSettings.useAmmoLimit) {
			if (weaponSettings.remainAmmo >= weaponSettings.ammoLimit) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	public bool hasAmmoLimit ()
	{
		return weaponSettings.useAmmoLimit;
	}

	public int ammoAmountToMaximumLimit ()
	{
		return weaponSettings.ammoLimit - weaponSettings.auxRemainAmmo;
	}

	//the vehicle has used an ammo pickup, so increase the correct weapon by name
	public void getAmmo (int amount)
	{
		bool empty = false;

		if (weaponSettings.remainAmmo == 0 && weaponSettings.clipSize == 0) {
			empty = true;
		}

		weaponSettings.remainAmmo += amount;

		if (empty) {
			if (weaponSettings.autoReloadWhenClipEmpty) {
				manualReload ();
			}
		}

		weaponSettings.auxRemainAmmo = weaponSettings.remainAmmo;
	}

	public void addAuxRemainAmmo (int amount)
	{
		weaponSettings.auxRemainAmmo += amount;
	}

	public bool hasAnyAmmo ()
	{
		if (weaponSettings.remainAmmo > 0 || weaponSettings.clipSize > 0 || weaponSettings.infiniteAmmo) {
			return true;
		}

		return false;
	}

	public void getAndUpdateAmmo (int amount)
	{
		getAmmo (amount);
	}

	public bool remainAmmoInClip ()
	{
		return weaponSettings.clipSize > 0;
	}

	public void checkSendProjectileOnEventFire ()
	{
		if (sendProjectileEventOnFire) {
			projectileEventOnFire.Invoke (currentProjectile);
		}
	}

	public void launchCurrentProjectile (GameObject newProjectile, Rigidbody projectileRigidbody, Vector3 cameraDirection)
	{
		//launch the projectile according to the velocity calculated according to the hit point of a raycast from the camera position
		projectileRigidbody.isKinematic = false;

		if (weaponSettings.useParableSpeed) {
			Vector3 newVel = getParableSpeed (newProjectile.transform.position, aimedZone, cameraDirection);

			if (newVel == -Vector3.one) {
				newVel = newProjectile.transform.forward * 100;
			}

			projectileRigidbody.AddForce (newVel, ForceMode.VelocityChange);
		} else {
			projectileRigidbody.AddForce (weaponSettings.parableDirectionTransform.forward * weaponSettings.projectileSpeed, ForceMode.Impulse);
		}
	}

	//calculate the speed applied to the launched projectile to make a parable according to a hit point
	Vector3 getParableSpeed (Vector3 origin, Vector3 target, Vector3 cameraDirection)
	{
		//if a hit point is not found, return
		if (!objectiveFound) {
			if (weaponSettings.useMaxDistanceWhenNoSurfaceFound) {
				target = origin + cameraDirection * weaponSettings.maxDistanceWhenNoSurfaceFound;
			} else {
				return -Vector3.one;
			}
		}

		// Get the distance between positions
		Vector3 toTarget = target - origin;
		Vector3 toTargetXZ = toTarget;

		// Remove the Y axis value
		toTargetXZ -= playerCameraGameObject.transform.InverseTransformDirection (toTargetXZ).y * playerCameraGameObject.transform.up;
		float y = playerCameraGameObject.transform.InverseTransformDirection (toTarget).y;
		float xz = toTargetXZ.magnitude;

		// Get the velocity according to distance and gravity
		float t = GKC_Utils.distance (origin, target) / 20;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;

		// Create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;        

		// Get direction of xz but with magnitude 1
		result *= v0xz;                           

		// Set magnitude of xz to v0xz (starting speed in xz plane), setting the local Y value
		result -= playerCameraGameObject.transform.InverseTransformDirection (result).y * playerCameraGameObject.transform.up;
		result += playerCameraGameObject.transform.up * v0y;

		return result;
	}

	public void checkParableTrayectory (bool usingThirdPerson, bool usingFirstPerson)
	{
		//enable or disable the parable linerenderer
		if (((usingThirdPerson && weaponSettings.activateLaunchParableThirdPerson) ||
		    (usingFirstPerson && weaponSettings.activateLaunchParableFirstPerson)) &&
		    weaponSettings.launchProjectile) {
		}
	}

	public void getImpactListInfo ()
	{
		if (impactDecalManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainDecalManagerName, typeof(decalManager));

			impactDecalManager = FindObjectOfType<decalManager> ();
		} 

		if (impactDecalManager != null) {
			
			impactDecalList = new string[impactDecalManager.impactListInfo.Count + 1];

			for (int i = 0; i < impactDecalManager.impactListInfo.Count; i++) {
				string name = impactDecalManager.impactListInfo [i].name;
				impactDecalList [i] = name;
			}
		}
	}

	public void setReducedVelocity (float multiplierValue)
	{
		weaponSpeed *= multiplierValue;
	}

	public void setNormalVelocity ()
	{
		weaponSpeed = originalWeaponSpeed;
	}

	public string getWeaponSystemName ()
	{
		return weaponSettings.Name;
	}

	public int getWeaponClipSize ()
	{
		return weaponSettings.clipSize;
	}

	public string getCurrentAmmoText ()
	{
		if (!weaponSettings.infiniteAmmo) {
			return weaponSettings.clipSize + "/" + weaponSettings.remainAmmo;
		} else {
			return weaponSettings.clipSize + "/" + "Inf";
		}
	}

	public void enableMuzzleFlashLight ()
	{
		if (!weaponSettings.useMuzzleFlash) {
			return;
		}

		if (muzzleFlashCoroutine != null) {
			StopCoroutine (muzzleFlashCoroutine);
		}

		muzzleFlashCoroutine = StartCoroutine (enableMuzzleFlashCoroutine ());
	}

	IEnumerator enableMuzzleFlashCoroutine ()
	{
		weaponSettings.muzzleFlahsLight.gameObject.SetActive (true);

		yield return new WaitForSeconds (weaponSettings.muzzleFlahsDuration);

		weaponSettings.muzzleFlahsLight.gameObject.SetActive (false);

		yield return null;
	}

	public void checkWeaponShootNoise ()
	{
		if (weaponSettings.useNoise) {
			if (weaponSettings.useNoiseDetection) {
				applyDamage.sendNoiseSignal (weaponSettings.noiseRadius, playerControllerGameObject.transform.position,
					weaponSettings.noiseDetectionLayer, weaponSettings.noiseDecibels, weaponSettings.forceNoiseDetection,
					weaponSettings.showNoiseDetectionGizmo, weaponSettings.noiseID);
			}

			if (weaponSettings.useNoiseMesh) {
				if (!noiseMeshManagerFound) {
					if (noiseMeshManager == null) {
						GKC_Utils.instantiateMainManagerOnSceneWithType (mainNoiseMeshManagerName, typeof(noiseMeshSystem));

						noiseMeshManager = FindObjectOfType<noiseMeshSystem> ();

						if (noiseMeshManager != null) {
							noiseMeshManagerFound = true;
						}
					}
				}

				if (noiseMeshManagerFound) {
					noiseMeshManager.addNoiseMesh (weaponSettings.noiseRadius, playerControllerGameObject.transform.position + Vector3.up, weaponSettings.noiseExpandSpeed);
				}
			}
		}
	}

	public projectileSystem getCurrentProjectileSystem ()
	{
		return currentProjectileSystem;
	}

	public List<projectileSystem> getLastProjectilesSystemListFired ()
	{
		return lastProjectilesSystemListFired;
	}


	public void setCustomprojectilePosition (List<Transform> customProjectilePosition)
	{
		weaponSettings.projectilePosition = customProjectilePosition;
	}

	void OnDrawGizmosSelected ()
	{
		if (!Application.isPlaying && getImpactListEveryFrame) {
			getImpactListInfo ();
		}
	}
}