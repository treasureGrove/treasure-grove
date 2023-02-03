using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class weaponSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public weaponInfo weaponSettings;
	public AudioClip outOfAmmo;
	public GameObject weaponProjectile;
	public LayerMask layer;

	public bool projectilesPoolEnabled = true;

	public int maxAmountOfPoolElementsOnWeapon = 30;

	public string mainDecalManagerName = "Decal Manager";

	public string mainNoiseMeshManagerName = "Noise Mesh Manager";

	public string[] impactDecalList;
	public int impactDecalIndex;
	public string impactDecalName;
	public bool getImpactListEveryFrame;
	decalManager impactDecalManager;

	public bool canMarkTargets;
	public List<string> tagListToMarkTargets = new List<string> ();
	public LayerMask markTargetsLayer;
	public string markTargetName;
	public float maxDistanceToMarkTargets;
	public bool canMarkTargetsOnFirstPerson;
	public bool canMarkTargetsOnThirdPerson;
	public bool aimOnFirstPersonToMarkTarget;
	public bool useMarkTargetSound;
	public AudioClip markTargetSound;

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
	public bool carryingWeaponInThirdPerson;
	public bool carryingWeaponInFirstPerson;
	public bool aimingInThirdPerson;
	public bool aimingInFirstPerson;
	public bool showSettings;

	public bool usingWeapon;

	public bool useAmmoFromMainPlayerWeaponSystem;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject playerControllerGameObject;
	public GameObject playerCameraGameObject;

	public IKWeaponSystem IKWeaponManager;
	public Transform mainCameraTransform;

	public playerWeaponsManager weaponsManager;

	public AudioSource weaponsEffectsSource;

	public Camera mainCamera;

	public Animation weaponAnimation;

	public launchTrayectory parable;

	public Collider playerCollider;

	public playerWeaponSystem mainPlayerWeaponSystem;

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
	bool aimingHommingProjectile;

	List<GameObject> locatedEnemies = new List<GameObject> ();

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

	float screenWidth;
	float screenHeight;

	Transform currentProjectilePosition;
	GameObject newProjectileGameObject;
	projectileSystem currentProjectileSystem;

	bool shootingBurst;
	int currentBurstAmount;

	GameObject newShellClone;
	weaponShellSystem newWeaponShellSystem;
	AudioClip newClipToShell;
	GameObject currentShellToRemove;
	bool shellsActive;
	bool shellsOnSceneToRemove;

	GameObject newMuzzleParticlesClone;
	Coroutine reloadCoroutine;

	noiseMeshSystem noiseMeshManager;

	projectileInfo newProjectileInfo;

	bool noiseMeshManagerFound;

	void Start ()
	{
		if (useAmmoFromMainPlayerWeaponSystem) {
			if (mainPlayerWeaponSystem == null) {
				useAmmoFromMainPlayerWeaponSystem = false;
			}
		}

		weaponSettings.ammoPerClip = getClipSize ();

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

		weaponSettings.auxRemainAmmo = getRemainAmmo ();

		usingScreenSpaceCamera = weaponsManager.isUsingScreenSpaceCamera ();
	}

	void Update ()
	{
		if (aimingInThirdPerson || carryingWeaponInFirstPerson) {
			if (!shellCreated && ((weaponHasAnimation && animationForwardPlayed && !weaponAnimation.IsPlaying (weaponSettings.animation)) || !weaponHasAnimation)) {
				createShells ();
			}

			if (!reloading) {
				if (weaponHasAnimation) {
					if (getClipSize () > 0) {
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
					} else if ((getRemainAmmo () > 0 || infiniteAmmoActive ()) && weaponSettings.autoReloadWhenClipEmpty) {
						reloadWeapon (weaponSettings.reloadTime);
					}
				} else {
					if (getClipSize () == 0) {
						if ((getRemainAmmo () > 0 || infiniteAmmoActive ()) && weaponSettings.autoReloadWhenClipEmpty) {
							reloadWeapon (weaponSettings.reloadTime);
						}
					}
				}
			} 

			if (aimingHommingProjectile) {
				//while the number of located enemies is lowers that the max enemies amount, then
				if (locatedEnemies.Count < weaponSettings.projectilePosition.Count) {
					//uses a ray to detect enemies, to locked them
					if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, weaponsManager.targetToDamageLayer)) {
						GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);
						if (target != null && target != playerControllerGameObject) {
							if (weaponSettings.tagToLocate.Contains (target.tag)) {
								GameObject placeToShoot = applyDamage.getPlaceToShootGameObject (target);
								if (!locatedEnemies.Contains (placeToShoot)) {
									//if an enemy is detected, add it to the list of located enemies and instantiated an icon in screen to follow the enemy
									locatedEnemies.Add (placeToShoot);

									weaponsManager.addElementToPlayerList (placeToShoot, false, false, 0, true, false, 
										false, false, weaponSettings.locatedEnemyIconName, false, Color.white, true, -1, 0, false);
								}
							}
						}
					}
				}
			}

			if (shootingBurst) {
				if (Time.time > lastShoot + weaponSettings.fireRate) {
					if ((!animationForwardPlayed && !animationBackPlayed && weaponHasAnimation) || !weaponHasAnimation) {
						currentBurstAmount--;
						if (currentBurstAmount == 0) {
							shootWeapon (!weaponsManager.isFirstPersonActive (), false);
							shootingBurst = false;
						} else {
							shootWeapon (!weaponsManager.isFirstPersonActive (), true);
						}
					}
				}
			}
		}

		checkDroppedShellsRemoval ();
	}

	bool infiniteAmmoActive ()
	{
		return weaponSettings.infiniteAmmo;
	}

	int getRemainAmmo ()
	{
		if (useAmmoFromMainPlayerWeaponSystem) {
			return mainPlayerWeaponSystem.weaponSettings.remainAmmo;
		} else {
			return weaponSettings.remainAmmo;
		}
	}

	int getClipSize ()
	{
		if (useAmmoFromMainPlayerWeaponSystem) {
			return mainPlayerWeaponSystem.weaponSettings.clipSize;
		} else {
			return weaponSettings.clipSize;
		}
	}

	public void setWeaponCarryState (bool thirdPersonCarry, bool firstPersonCarry)
	{
		carryingWeaponInThirdPerson = thirdPersonCarry;
		carryingWeaponInFirstPerson = firstPersonCarry;

		checkParableTrayectory (false, carryingWeaponInFirstPerson);

		checkWeaponAbility (false);

		//functions called when the player draws or keep the weapon in any view
		if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
			if (weaponSettings.useStartDrawAction) {
				if (weaponSettings.startDrawAction.GetPersistentEventCount () > 0) {
					weaponSettings.startDrawAction.Invoke ();
				}
			}
		} else {
			if (weaponSettings.useStopDrawAction) {
				if (weaponSettings.stopDrawAction.GetPersistentEventCount () > 0) {
					weaponSettings.stopDrawAction.Invoke ();
				}
			}
		}
			
		if (!weaponsManager.isFirstPersonActive ()) {
			if (carryingWeaponInThirdPerson) {
				if (weaponSettings.useStartDrawActionThirdPerson) {
					if (weaponSettings.startDrawActionThirdPerson.GetPersistentEventCount () > 0) {
						weaponSettings.startDrawActionThirdPerson.Invoke ();
					}
				}
			} else {
				if (weaponSettings.useStopDrawActionThirdPerson) {
					if (weaponSettings.stopDrawActionThirdPerson.GetPersistentEventCount () > 0) {
						weaponSettings.stopDrawActionThirdPerson.Invoke ();
					}
				}
			}
		}

		if (weaponsManager.isFirstPersonActive ()) {
			if (carryingWeaponInFirstPerson) {
				if (weaponSettings.useStartDrawActionFirstPerson) {
					if (weaponSettings.startDrawActionFirstPerson.GetPersistentEventCount () > 0) {
						weaponSettings.startDrawActionFirstPerson.Invoke ();
					}
				}
			} else {
				if (weaponSettings.useStopDrawActionFirstPerson) {
					if (weaponSettings.stopDrawActionFirstPerson.GetPersistentEventCount () > 0) {
						weaponSettings.stopDrawActionFirstPerson.Invoke ();
					}
				}
			}
		}
	}

	public void setWeaponAimState (bool thirdPersonAim, bool firstPersonAim)
	{
		aimingInThirdPerson = thirdPersonAim;
		aimingInFirstPerson = firstPersonAim;

		if ((aimingInThirdPerson || aimingInFirstPerson) && getClipSize () == 0) {
			if (weaponSettings.autoReloadWhenClipEmpty) {
				manualReload ();
			}
		}

		if (carryingWeaponInFirstPerson) {
			checkParableTrayectory (false, !aimingInFirstPerson);
		}

		if (carryingWeaponInThirdPerson) {
			checkParableTrayectory (aimingInThirdPerson, false);
			checkWeaponAbility (false);
		}

		if (!aimingInFirstPerson && !aimingInThirdPerson) {
			if (locatedEnemies.Count > 0) {
				aimingHommingProjectile = false;
				//remove the icons in the screen
				removeLocatedEnemiesIcons ();
			}
		}

		//functions called when the player aims or stop to aim the weapon in any view
		if (!weaponsManager.isFirstPersonActive ()) {
			if (aimingInThirdPerson) {
				if (weaponSettings.useStartAimActionThirdPerson) {
					if (weaponSettings.startAimActionThirdPerson.GetPersistentEventCount () > 0) {
						weaponSettings.startAimActionThirdPerson.Invoke ();
					}
				}
			} else {
				if (weaponSettings.useStopAimActionThirdPerson) {
					if (weaponSettings.stopAimActionThirdPerson.GetPersistentEventCount () > 0) {
						weaponSettings.stopAimActionThirdPerson.Invoke ();
					}
				}
			}
		}

		if (weaponsManager.isFirstPersonActive ()) {
			if (aimingInFirstPerson) {
				if (weaponSettings.useStartAimActionFirstPerson) {
					if (weaponSettings.startAimActionFirstPerson.GetPersistentEventCount () > 0) {
						weaponSettings.startAimActionFirstPerson.Invoke ();
					}
				}
			} else {
				if (weaponSettings.useStopAimActionFirstPerson) {
					if (weaponSettings.stopAimActionFirstPerson.GetPersistentEventCount () > 0) {
						weaponSettings.stopAimActionFirstPerson.Invoke ();
					}
				}
			}
		}
	}

	public void setUsingWeaponState (bool state)
	{
		usingWeapon = state;

		if (weaponSettings.showWeaponIconInHUD) {
			weaponsManager.setAttachmentPanelState (usingWeapon);

			weaponsManager.setAttachmentIcon (weaponSettings.weaponIconHUD);

			weaponsManager.setAttachmentPanelAmmoText (getClipSize ().ToString ());

			if (usingWeapon) {
				weaponSettings.clipSizeText = weaponsManager.attachmentAmmoText;
			}
		}
	}

	public void checkCarryAndAimStates ()
	{
		if (weaponsManager.isCarryingWeaponInThirdPerson () != carryingWeaponInThirdPerson || weaponsManager.carryingWeaponInFirstPerson != carryingWeaponInFirstPerson) {
			setWeaponCarryState (weaponsManager.isCarryingWeaponInThirdPerson (), weaponsManager.carryingWeaponInFirstPerson);
		}

		if (weaponsManager.isAimingInThirdPerson () != aimingInThirdPerson || weaponsManager.aimingInFirstPerson != aimingInFirstPerson) {
			setWeaponAimState (weaponsManager.isAimingInThirdPerson (), weaponsManager.aimingInFirstPerson);
		}
	}

	public void inputShootWeaponOnPressDown ()
	{
		if (weaponsManager.canUseWeaponsInput ()) {
			checkCarryAndAimStates ();

			if (aimingInThirdPerson || carryingWeaponInFirstPerson) {
				if (weaponsManager.isCursorLocked ()) {
					if (weaponSettings.automatic) {
						if (weaponSettings.useBurst) {
							shootWeapon (true);
						}
					} else {
						shootWeapon (true);
					}
				}
			}
		}
	}

	public void inputShootWeaponOnPressUp ()
	{
		if (weaponsManager.canUseWeaponsInput ()) {
			checkCarryAndAimStates ();

			if (aimingInThirdPerson || carryingWeaponInFirstPerson) {
				if (weaponsManager.isCursorLocked ()) {
					shootWeapon (false);
				}
			}
		}
	}

	public void inputShootWeaponOnPress ()
	{
		if (weaponsManager.canUseWeaponsInput ()) {
			checkCarryAndAimStates ();

			if (aimingInThirdPerson || carryingWeaponInFirstPerson) {
				if (weaponsManager.isCursorLocked ()) {
					if (weaponSettings.automatic) {
						if (!weaponSettings.useBurst) {
							shootWeapon (true);
						}
					}
				}
			}
		}
	}

	public void shootWeapon (bool state)
	{
		if (!weaponsManager.playerIsBusy ()) {
			if (state) {
				if (!shootingBurst) {

					if (IKWeaponManager.isCursorHidden ()) {
						weaponsManager.enableOrDisableGeneralWeaponCursor (true);
						IKWeaponManager.setCursorHiddenState (false);
					}

					weaponsManager.disablePlayerRunningState ();

					if (!reloading && getWeaponClipSize () > 0) {
						weaponsManager.setShootingState (true);
					} else {
						weaponsManager.setShootingState (false);
					}

					shootWeapon (!weaponsManager.isFirstPersonActive (), state);
					weaponsManager.setLastTimeFired ();
				}
			} else {
				weaponsManager.setShootingState (false);
				shootWeapon (!weaponsManager.isFirstPersonActive (), state);
			}
		}
	}


	// Fire the current weapon
	public void shootWeapon (bool isThirdPersonView, bool shootAtKeyDown)
	{
		if (!IKWeaponManager.weaponCanFire ()) {
			return;
		}

		if (IKWeaponManager.moving) {
			return;
		}

		if (reloading) {
			return;
		}

		checkWeaponAbility (shootAtKeyDown);

		// If the weapon system is active and the clip size higher than 0
		if (getClipSize () > 0) {
			// else, fire the current weapon according to the fire rate
			if (Time.time > lastShoot + weaponSettings.fireRate) {

				// If the player fires a weapon, set the visible to AI state to true, this will change if the player is using a silencer
				if (weaponSettings.shootAProjectile || weaponSettings.launchProjectile) {
					weaponsManager.setVisibleToAIState (true);
				}

				// If the current projectile is homing type, check when the shoot button is pressed and release
				if ((weaponSettings.isHommingProjectile && shootAtKeyDown)) {
					aimingHommingProjectile = true;
					//print ("1 "+ shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHommingProjectile);
					return;
				}

				if ((weaponSettings.isHommingProjectile && !shootAtKeyDown && locatedEnemies.Count <= 0) ||
				    (!weaponSettings.isHommingProjectile && !shootAtKeyDown)) {
					aimingHommingProjectile = false;
					//print ("2 "+shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHommingProjectile);
					return;
				}

				if ((animationForwardPlayed || animationBackPlayed) && weaponHasAnimation) {
					return;
				}


				if (weaponSettings.automatic && weaponSettings.useBurst) {
					if (!shootingBurst && weaponSettings.burstAmount > 0) {
						shootingBurst = true;
						currentBurstAmount = weaponSettings.burstAmount;
					}
				}

				// Camera shake
				if (IKWeaponManager.sameValueBothViews) {
					weaponsManager.setHeadBobShotShakeState (IKWeaponManager.thirdPersonshotShakeInfo);
				} else {
					if (weaponsManager.carryingWeaponInFirstPerson && IKWeaponManager.useShotShakeInFirstPerson) {
						weaponsManager.setHeadBobShotShakeState (IKWeaponManager.firstPersonshotShakeInfo);
					}

					if (weaponsManager.isCarryingWeaponInThirdPerson () && IKWeaponManager.useShotShakeInThirdPerson) {
						weaponsManager.setHeadBobShotShakeState (IKWeaponManager.thirdPersonshotShakeInfo);
					}
				}

				// Recoil
				IKWeaponManager.startRecoil (isThirdPersonView);

				IKWeaponManager.setLastTimeMoved ();

				if (weaponSettings.shakeUpperBodyWhileShooting) {
					weaponsManager.checkShakeUpperBodyRotationCoroutine (weaponSettings.shakeAmount, weaponSettings.shakeSpeed);
				}

				//check shot camera noise
				IKWeaponManager.setShotCameraNoise ();

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
							if (Physics.Raycast (mainCameraPosition, mainCameraTransform.TransformDirection (Vector3.forward), out hitCamera, Mathf.Infinity, layer) &&
							    Physics.Raycast (weaponSettings.projectilePosition [0].position, mainCameraTransform.TransformDirection (Vector3.forward), out hitWeapon, Mathf.Infinity, layer)) {
								if (hitCamera.collider != hitWeapon.collider) {
									//print ("too close surface");
									weaponCrossingSurface = true;
								} 
							}
						}
					}
				}

				// Play the fire animation
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
				// Every weapon can shoot 1 or more projectiles at the same time, so for every projectile position to instantiate
				for (int j = 0; j < weaponSettings.projectilePosition.Count; j++) {
					currentProjectilePosition = weaponSettings.projectilePosition [j];

					for (int l = 0; l < weaponSettings.projectilesPerShoot; l++) {
						// Create the projectile
						if (projectilesPoolEnabled) {
							newProjectileGameObject = GKC_PoolingSystem.Spawn (weaponProjectile, currentProjectilePosition.position, mainCameraTransform.rotation, maxAmountOfPoolElementsOnWeapon);
						} else {
							newProjectileGameObject = (GameObject)Instantiate (weaponProjectile, currentProjectilePosition.position, mainCameraTransform.rotation);
						}

						// Set the info in the projectile, like the damage, the type of projectile, bullet or missile, etc...
						currentProjectileSystem = newProjectileGameObject.GetComponent<projectileSystem> ();

						currentProjectileSystem.checkToResetProjectile ();

						if (!weaponSettings.launchProjectile) {
							// Set its direction in the weapon forward or the camera forward according to if the weapon is aimed correctly or not
							if (!weaponCrossingSurface) {
								if (Physics.Raycast (mainCameraPosition, cameraDirection, out hit, Mathf.Infinity, layer)
								    && !weaponSettings.fireWeaponForward) {
									if (!hit.collider.isTrigger) {
										//Debug.DrawLine (weaponSettings.projectilePosition [j].position, hit.point, Color.red, 2);
										newProjectileGameObject.transform.LookAt (hit.point);
									}
								}
							}
						}

						if (weaponSettings.launchProjectile) {
							if (currentProjectileSystem != null) {
								currentProjectileRigidbody = currentProjectileSystem.getProjectileRigibody ();
							}

							if (currentProjectileRigidbody == null) {
								currentProjectileRigidbody = newProjectileGameObject.GetComponent<Rigidbody> ();
							}

							currentProjectileRigidbody.isKinematic = true;
							// If the vehicle has a gravity control component, and the current gravity is not the regular one,
							// add an artificial gravity component to the projectile like this, it can make a parable in any
							// surface and direction, setting its gravity in the same of the vehicle.

							Vector3 currentNormal = weaponsManager.getCurrentNormal ();

							if (currentNormal != Vector3.up) {
								newProjectileGameObject.AddComponent<artificialObjectGravity> ().setCurrentGravity (-currentNormal);
							}

							if (weaponSettings.useParableSpeed) {
								// Get the ray hit point where the projectile will fall
								if (Physics.Raycast (mainCameraPosition, cameraDirection, out hit, Mathf.Infinity, layer)) {
									aimedZone = hit.point;
									objectiveFound = true;
								} else {
									objectiveFound = false;
								}
							}

							launchCurrentProjectile (newProjectileGameObject, currentProjectileRigidbody, cameraDirection);
						}

						//add spread to the projectile
						Vector3 spreadAmount = Vector3.zero;
						if (weaponSettings.useProjectileSpread) {
							spreadAmount = setProjectileSpread ();
							newProjectileGameObject.transform.Rotate (spreadAmount);
						}

						//set the info in the projectile, like the damage, the type of projectile, bullet or missile, etc...
						currentProjectileSystem.setProjectileInfo (setProjectileInfo ());

						if (weaponSettings.isSeeker) {
							closestEnemy = setSeekerProjectileInfo (currentProjectilePosition.position);

							if (closestEnemy != null) {
								currentProjectileSystem.setEnemy (closestEnemy);
							}
						}

						//if the homing projectiles are being using, then
						if (aimingHommingProjectile) {
							//if the button to shoot is released, shoot a homing projectile for every located enemy
							//check that the located enemies are higher that 0
							if (locatedEnemies.Count > 0) {
								//shoot the missiles
								if (j < locatedEnemies.Count) {
									currentProjectileSystem.setEnemy (locatedEnemies [j]);
								}
							}
						}

						bool projectileFiredByRaycast = false;

						bool projectileDestroyed = false;

						// If the weapon shoots setting directly the projectile in the hit point, place the current projectile in the hit point position
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

							if (Physics.Raycast (forwardPosition, forwardDirection, out hit, Mathf.Infinity, layer)) {
								currentProjectileSystem.rayCastShoot (hit.collider, hit.point, forwardDirection);

								projectileFiredByRaycast = true;
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

					useAmmo ();

					lastShoot = Time.time;
					destroyShellsTimer = 0;
				}

				if (weaponSettings.weaponWithAbility) {
					lastShoot = Time.time;
					destroyShellsTimer = 0;
				}

				if (weaponSettings.applyForceAtShoot) {
					forceDirection = (mainCameraTransform.right * weaponSettings.forceDirection.x +
					mainCameraTransform.up * weaponSettings.forceDirection.y +
					mainCameraTransform.forward * weaponSettings.forceDirection.z) * weaponSettings.forceAmount;
					weaponsManager.externalForce (forceDirection);
				}

				if (weaponSettings.isHommingProjectile && !shootAtKeyDown && aimingHommingProjectile) {
					//if the button to shoot is released, shoot a homing projectile for every located enemy
					//check that the located enemies are higher that 0
					if (locatedEnemies.Count > 0) {
						//remove the icons in the screen
						removeLocatedEnemiesIcons ();
					}
					aimingHommingProjectile = false;
				}
			}
		} 
		//else, the clip in the weapon is over, so check if there is remaining ammo
		else {
			if (getRemainAmmo () == 0 && !infiniteAmmoActive ()) {
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

	// remove the located enemies icons
	public void removeLocatedEnemiesIcons ()
	{
		for (int i = 0; i < locatedEnemies.Count; i++) {
			weaponsManager.removeLocatedEnemiesIcons (locatedEnemies [i]);
		}

		locatedEnemies.Clear ();
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

		newProjectileInfo.projectileDamage = (weaponSettings.projectileDamage + weaponsManager.getExtraDamageStat ()) * weaponsManager.getDamageMultiplierStat ();
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

		newProjectileInfo.targetToDamageLayer = weaponsManager.targetToDamageLayer;
		newProjectileInfo.targetForScorchLayer = weaponsManager.targetForScorchLayer;

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
		if (mainCamera == null) {
			mainCamera = weaponsManager.getMainCamera ();
		}

		return applyDamage.setSeekerProjectileInfo (shootPosition, weaponSettings.tagToLocate, usingScreenSpaceCamera, 
			weaponSettings.targetOnScreenForSeeker, mainCamera, weaponsManager.targetToDamageLayer, transform.position, false, null);
	}

	public Vector3 setProjectileSpread ()
	{
		float spreadAmount = 0;
		if (carryingWeaponInFirstPerson) {
			spreadAmount = weaponSettings.spreadAmount;
		}

		if (carryingWeaponInThirdPerson) {
			if (weaponSettings.sameSpreadInThirdPerson) {
				spreadAmount = weaponSettings.spreadAmount;
			} else {
				spreadAmount = weaponSettings.thirdPersonSpreadAmount;
			}
		}

		if (aimingInFirstPerson) {
			//print ("aiming");
			if (weaponSettings.useSpreadAming) {
				if (weaponSettings.useLowerSpreadAiming) {
					spreadAmount = weaponSettings.lowerSpreadAmount;
					//print ("lower spread");
				} else {
					//print ("same spread");
				}
			} else {
				//print ("no spread");
				spreadAmount = 0;
			}
		} else {
			//print ("no aiming");
		}

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
					if (weaponsManager.isFirstPersonActive ()) {
						weaponsManager.setWeaponPartLayer (newMuzzleParticlesClone);
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

	//decrease the amount of ammo in the clip
	public void useAmmo ()
	{
		if (useAmmoFromMainPlayerWeaponSystem) {
			mainPlayerWeaponSystem.useAmmo ();
		} else {
			weaponSettings.clipSize--;
			//update hud ammo info
			weaponsManager.setAttachmentPanelAmmoText (getClipSize ().ToString ());
		}
	}

	public void useAmmo (int amount)
	{
		if (useAmmoFromMainPlayerWeaponSystem) {
			mainPlayerWeaponSystem.useAmmo (amount);
		} else {
			if (amount > getClipSize ()) {
				amount = getClipSize ();
			}

			weaponSettings.clipSize -= amount;
			//update hud ammo info
			weaponsManager.setAttachmentPanelAmmoText (getClipSize ().ToString ());
		}
	}

	/// <summary>
	/// Checks and handles the amount of remaining ammo.
	/// </summary>
	void checkRemainingAmmo ()
	{
		// if the weapon doesn't have infinite ammo
		if (!infiniteAmmoActive ()) {
			// and the clip is empty
			if (getClipSize () == 0) {
				// if the remaining ammo is lower that the ammo per clip, set the final projectiles in the clip 
				if (getRemainAmmo () < weaponSettings.ammoPerClip) {
					weaponSettings.clipSize = getRemainAmmo ();
				} 
				// else, refill it
				else {
					weaponSettings.clipSize = weaponSettings.ammoPerClip;
				}

				// if the remaining ammo is higher than 0, remove the current projectiles added in the clip
				if (getRemainAmmo () > 0) {
					if (useAmmoFromMainPlayerWeaponSystem) {
						mainPlayerWeaponSystem.useRemainAmmo (getClipSize ());
					} else {
						weaponSettings.remainAmmo -= getClipSize ();
					}
				}
			} 
			// the clip has some bullets in it yet
			else {
				int usedAmmo = 0;

				if (weaponSettings.removePreviousAmmoOnClip) {
					weaponSettings.clipSize = 0;

					if (getRemainAmmo () < (weaponSettings.ammoPerClip)) {
						usedAmmo = weaponSettings.remainAmmo;
					} else {
						usedAmmo = weaponSettings.ammoPerClip;
					}
				} else {
					if (getRemainAmmo () < (weaponSettings.ammoPerClip - getClipSize ())) {
						usedAmmo = weaponSettings.remainAmmo;
					} else {
						usedAmmo = weaponSettings.ammoPerClip - getClipSize ();
					}
				}
				if (useAmmoFromMainPlayerWeaponSystem) {
					mainPlayerWeaponSystem.useRemainAmmo (usedAmmo);
				} else {
					weaponSettings.remainAmmo -= usedAmmo;
				}

				weaponSettings.clipSize += usedAmmo;
			}
		} else {
			// else, the weapon has infinite ammo, so refill it
			weaponSettings.clipSize = weaponSettings.ammoPerClip;
		}

		weaponsManager.setAttachmentPanelAmmoText (getClipSize ().ToString ());

		weaponSettings.auxRemainAmmo = getRemainAmmo ();
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
		//if the remaining ammo is higher than 0 or infinite
		shootingBurst = false;

		if (getRemainAmmo () > 0 || infiniteAmmoActive ()) {
			//reload
			reloading = true;
			//play the reload sound
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
			if (getClipSize () < weaponSettings.ammoPerClip) {
				reloadWeapon (weaponSettings.reloadTime);
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
			if (getRemainAmmo () >= weaponSettings.ammoLimit) {
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

		if (getRemainAmmo () == 0 && getClipSize () == 0) {
			empty = true;
		}

		weaponSettings.remainAmmo += amount;

		if (empty && (carryingWeaponInFirstPerson || aimingInThirdPerson)) {
			if (weaponSettings.autoReloadWhenClipEmpty) {
				manualReload ();
			}
		}

		weaponSettings.auxRemainAmmo = getRemainAmmo ();

	}

	public void addAuxRemainAmmo (int amount)
	{
		weaponSettings.auxRemainAmmo += amount;
	}

	public bool hasAnyAmmo ()
	{
		if (getRemainAmmo () > 0 || getClipSize () > 0 || infiniteAmmoActive ()) {
			return true;
		}

		return false;
	}

	public void getAndUpdateAmmo (int amount)
	{
		getAmmo (amount);

		weaponsManager.setAttachmentPanelAmmoText (getClipSize ().ToString ());
	}

	public bool remainAmmoInClip ()
	{
		return getClipSize () > 0;
	}

	public void launchCurrentProjectile (GameObject currentProjectile, Rigidbody projectileRigidbody, Vector3 cameraDirection)
	{
		//launch the projectile according to the velocity calculated according to the hit point of a raycast from the camera position
		projectileRigidbody.isKinematic = false;

		if (weaponSettings.useParableSpeed) {
			Vector3 newVel = getParableSpeed (currentProjectile.transform.position, aimedZone, cameraDirection);

			if (newVel == -Vector3.one) {
				newVel = currentProjectile.transform.forward * 100;
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

			if (parable != null) {
				parable.changeParableState (true);

				if (weaponSettings.projectilePosition.Count > 0) {
					parable.shootPosition = weaponSettings.projectilePosition [0];
				}
			}
		} else {
			if (parable != null) {
				parable.changeParableState (false);
			}
		}
	}

	public void getWeaponComponents (playerWeaponsManager newPlayerWeaponsManager)
	{
		playerControllerGameObject = newPlayerWeaponsManager.gameObject;

		weaponsManager = newPlayerWeaponsManager;

		playerCameraGameObject = weaponsManager.playerCameraManager.gameObject;
			
		mainCameraTransform = weaponsManager.getMainCameraTransform ();

		mainCamera = weaponsManager.getMainCamera ();

		weaponsEffectsSource = GetComponent<AudioSource> ();

		weaponAnimation = GetComponent<Animation> ();

		parable = GetComponentInChildren<launchTrayectory> ();

		playerCollider = playerControllerGameObject.GetComponent<Collider> ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
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

			updateComponent ();
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
		return getClipSize ();
	}

	public string getCurrentAmmoText ()
	{
		if (!infiniteAmmoActive ()) {
			return getClipSize () + "/" + getRemainAmmo ();
		} else {
			return getClipSize () + "/" + "Inf";
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
					weaponSettings.noiseDetectionLayer, weaponSettings.noiseDecibels,
					weaponSettings.forceNoiseDetection, weaponSettings.showNoiseDetectionGizmo, weaponSettings.noiseID);
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

	void OnDrawGizmosSelected ()
	{
		if (!Application.isPlaying && getImpactListEveryFrame) {
			getImpactListInfo ();
		}
	}

	[System.Serializable]
	public class weaponInfo
	{
		public string Name;

		[Space]
		[Space]

		public bool useRayCastShoot;
		public bool fireWeaponForward;

		public bool useRaycastShootDelay;
		public float raycastShootDelay;
		public bool getDelayWithDistance;
		public float delayWithDistanceSpeed;
		public float maxDelayWithDistance;

		public bool useFakeProjectileTrails;

		public bool useRaycastCheckingOnRigidbody;

		public float customRaycastCheckingRate;

		public float customRaycastCheckingDistance = 0.1f;

		[Space]
		[Space]

		public int ammoPerClip;
		public bool removePreviousAmmoOnClip;
		public bool infiniteAmmo;
		public int remainAmmo;
		public int clipSize;
		public bool dropClipWhenReload;
		public Transform positionToDropClip;
		public GameObject clipModel;
		public bool startWithEmptyClip;
		public bool autoReloadWhenClipEmpty = true;
		public bool useAmmoLimit;
		public int ammoLimit;
		public int auxRemainAmmo;

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
		public float reloadTime;
		public float projectileDamage;
		public float projectileSpeed;
		public int projectilesPerShoot;

		public bool useProjectileSpread;
		public float spreadAmount;
		public bool sameSpreadInThirdPerson;
		public float thirdPersonSpreadAmount;
		public bool useSpreadAming;
		public bool useLowerSpreadAiming;
		public float lowerSpreadAmount;

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

		public bool shakeUpperBodyWhileShooting;
		public float shakeAmount;
		public float shakeSpeed;

		public bool useMuzzleFlash;
		public Light muzzleFlahsLight;
		public float muzzleFlahsDuration;

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

		public bool useNoise;
		public float noiseRadius;
		public float noiseExpandSpeed;
		public bool useNoiseDetection;
		public LayerMask noiseDetectionLayer;
		public bool showNoiseDetectionGizmo;
		[Range (0, 2)] public float noiseDecibels = 1;

		public int noiseID = -1;

		public bool forceNoiseDetection;

		public bool useNoiseMesh = true;

		public bool projectileCanBeDeflected = true;

		[Space]
		[Space]

		public bool autoShootOnTag;
		public LayerMask layerToAutoShoot;
		public List<string> autoShootTagList = new List<string> ();
		public float maxDistanceToRaycast;
		public bool shootAtLayerToo;

		[Space]
		[Space]

		public GameObject weaponMesh;
		public string animation;
		public float animationSpeed = 1;
		public bool playAnimationBackward = true;

		public GameObject scorch;
		public float scorchRayCastDistance;

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

		public AudioClip reloadSoundEffect;
		public AudioClip cockSound;
		public AudioClip shootSoundEffect;
		public AudioClip impactSoundEffect;

		public bool setShootParticlesLayerOnFirstPerson;

		public GameObject shootParticles;
		public GameObject projectileParticles;
		public GameObject impactParticles;

		[Space]
		[Space]

		public Texture weaponIconHUD;
		public bool showWeaponNameInHUD = true;
		public bool showWeaponIconInHUD = true;
		public bool showWeaponAmmoSliderInHUD = true;
		public bool showWeaponAmmoTextInHUD = true;

		public Text clipSizeText;

		[Space]
		[Space]

		public bool useDownButton;
		public UnityEvent downButtonAction;
		public bool useHoldButton;
		public UnityEvent holdButtonAction;
		public bool useUpButton;
		public UnityEvent upButtonAction;

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
	}
}