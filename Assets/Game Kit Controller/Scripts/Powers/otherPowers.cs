using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class otherPowers : MonoBehaviour
{
	public int choosedPower = 0;

	public int initialPowerIndex = 0;

	public LayerMask layerToDamage;

	public powersSettings settings = new powersSettings ();
	public aimSettings aimsettings = new aimSettings ();
	public shootSettings shootsettings = new shootSettings ();
	public bool usingPowers = false;

	public float auxPowerAmount;
	public bool showSettings;
	public bool showAimSettings;
	public bool showShootSettings;

	public bool usedByAI;
	public bool aimingInThirdPerson;
	public bool aimingInFirstPerson;
	public bool shooting;

	public bool infinitePower;
	public bool regeneratePower;
	public bool constantRegenerate;
	public float regenerateSpeed = 0;
	public float regenerateTime;
	public float regenerateAmount;

	public bool useAimAssistInThirdPerson;
	public bool useMaxDistanceToCameraCenterAimAssist;
	public float maxDistanceToCameraCenterAimAssist;

	public bool useAimAssistInLockedCamera = true;

	public LayerMask targetForScorchLayer;

	public bool powersModeActive;

	public bool canFirePowersWithoutAiming;
	public bool useAimCameraOnFreeFireMode;
	bool aimingPowerFromShooting;
	public bool usingFreeFireMode;

	public bool checkToKeepPowersAfterAimingPowerFromShooting;

	public float timeToStopAimAfterStopFiring = 0.85f;

	public bool aimModeInputPressed;
	public bool weaponAimedFromFiringActive;

	bool checkToKeepPowersAfterAimingPowerFromShooting2_5d;

	public bool projectilesPoolEnabled = true;

	public int maxAmountOfPoolElementsOnWeapon = 30;

	public bool headLookWhenAiming;
	public float headLookSpeed;
	public Transform headLookTarget;

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	public weaponRotationPointInfo rotationPointInfo;
	public bool usePowerRotationPoint;
	public Transform powerRotationPoint;

	public AudioSource shootZoneAudioSource;
	public Transform mainCameraTransform;
	public Camera mainCamera;
	public upperBodyRotationSystem upperBodyRotationManager;
	public IKSystem IKSystemManager;

	public playerCamera playerCameraManager;
	public headBob headBobManager;
	public playerWeaponsManager weaponsManager;

	public playerController playerControllerManager;
	public grabObjects grabObjectsManager;

	public headTrack headTrackManager;
	public decalManager impactDecalManager;

	public playerInputManager playerInput;
	public powersListManager powersManager;
	public Collider playerCollider;
	public RectTransform cursorRectTransform;

	public string mainDecalManagerName = "Decal Manager";

	public launchTrayectory parable;

	public bool changePowersWithNumberKeysActive = true;
	public bool changePowersWithMouseWheelActive = true;
	public bool changePowersWithKeysActive = true;

	public bool useLowerRotationSpeedAimedThirdPerson;
	public float verticalRotationSpeedAimedInThirdPerson = 4;
	public float horizontalRotationSpeedAimedInThirdPerson = 4;

	public bool runWhenAimingPowerInThirdPerson;
	public bool stopRunIfPreviouslyNotRunning;
	bool runningPreviouslyAiming;

	public GameObject powersInfoPanel;

	public string currentPowerName;

	Powers currentPower;

	GameObject currentProjectile;

	public int amountPowersEnabled;

	public playerStatsSystem playerStatsManager;
	public string energyStatName = "Current Energy";
	bool hasPlayerStatsManager;

	public bool actionActive;

	public bool firstPersonActive;
	public bool canMove;
	public bool touching;

	public bool showElements;

	Vector3 swipeStartPos;

	List<GameObject> locatedEnemies = new List<GameObject> ();

	RaycastHit hit;

	float powerSelectionTimer;

	float lastTimeUsed;
	float lastTimeFired;

	bool selection;

	bool playerIsDead;

	bool homingProjectiles;
	bool touchPlatform;

	Touch currentTouch;

	GameObject closestEnemy;

	public string[] impactDecalList;

	GameObject parableGameObject;
	bool objectiveFound;
	Vector3 aimedZone;
	Vector3 forceDirection;

	Coroutine muzzleFlashCoroutine;

	bool playerCurrentlyBusy;

	bool startInitialized;

	bool usingScreenSpaceCamera;
	bool targetOnScreen;
	Vector3 screenPoint;

	bool grabObjectsAttached;

	float lastShoot;
	float minimumFireRate = 0.2f;

	bool shootingBurst;
	int currentBurstAmount;

	bool fingerPressingTouchPanel;

	float screenWidth;
	float screenHeight;

	bool powersManagerLocated;

	GameObject shootParticles;

	bool autoShootOnTagActive;

	GameObject previousTargetDetectedOnAutoShootOnTag;
	GameObject currentTargetDetectedOnAutoShootOnTag;

	bool otherPowersInputPaused;

	bool ignoreDeactivateAiming;


	public bool useFreeAimMode;
	public bool freeAimModeeActive;
	public Transform armsPivotRotationTransform;
	public float armsPivotRotationSpeed = 10;
	public float armsPivotClampRotation = 60;
	public float armsPivotRotationMultiplier = 1;

	public float freeAimModLookInOppositeDirectionExtraRange = 10;
	public int freeAimModePlayerStatusID = -1;

	float currentArmsPivotRotation;

	bool checkToLookAtLeft;
	bool checkToLookAtRight = true;

	int previousPlayerStatusID = -100;


	void Awake ()
	{
		choosedPower = initialPowerIndex;

		currentPower = shootsettings.powersList [initialPowerIndex];

		currentPowerName = currentPower.Name;

		if (powersManager != null) {
			powersManagerLocated = true;
		}
	}

	void Start ()
	{
		canMove = !playerIsDead && playerControllerManager.canPlayerMove ();

		//set the texture of the current selected power
		if (shootsettings.selectedPowerHud != null) {
			shootsettings.selectedPowerHud.texture = currentPower.texture;
		}

		//by default the aim mode stays in the right side of the player, but it is checked in the start
		setAimModeSide (true);

		firstPersonActive = playerCameraManager.isFirstPersonActive ();

		//set the amount of current powers enabled
		updateAmountPowersEnabled ();

		//check if the platform is a touch device or not
		touchPlatform = touchJoystick.checkTouchPlatform ();

		if (shootsettings.maxPowerAmount == 0) {
			shootsettings.maxPowerAmount = shootsettings.powerAmount;
		}

		//set the value of energy avaliable at the beginning of the game
		if (settings.powerBar != null) {
			settings.powerBar.maxValue = shootsettings.maxPowerAmount;
			settings.powerBar.value = shootsettings.powerAmount;

			if (settings.powerBarText != null) {
				settings.powerBarText.text = shootsettings.powerAmount.ToString ();
			}
		}

		//store the max amount of energy in axiliar variables, used for the pick ups to check that the player doesn't use more pickups that the neccessary
		auxPowerAmount = shootsettings.powerAmount;

		//get the parable launcher in case the weapons has it
		if (parable != null) {
			parableGameObject = parable.gameObject;
		}

		if (!usedByAI) {
			setFirstPowerAvailable ();

			setSelectedPowerIconState (false);
		}

		usingScreenSpaceCamera = playerCameraManager.isUsingScreenSpaceCamera ();

		if (playerStatsManager != null) {
			hasPlayerStatsManager = true;
		}

		if (firstPersonActive) {
			shootsettings.shootZone.SetParent (mainCameraTransform.transform);
			shootsettings.shootZone.localPosition = shootsettings.firstPersonShootPosition.localPosition;
			shootsettings.shootZone.localRotation = Quaternion.identity;
		} else {
			aimsettings.handActive = aimsettings.rightHand;
			shootsettings.shootZone.SetParent (aimsettings.handActive.transform);
			shootsettings.shootZone.localPosition = Vector3.zero;
			shootsettings.shootZone.localRotation = Quaternion.identity;
		}

		grabObjectsAttached = grabObjectsManager != null;
	}

	void Update ()
	{
		if (!startInitialized) {
			startInitialized = true;
		}

		canMove = !playerIsDead && playerControllerManager.canPlayerMove ();

		if (!usedByAI || powersModeActive) {
			//the power is regenerated if the player is not using it
			if (regeneratePower && canMove && !infinitePower) {
				if (constantRegenerate) {
					if (regenerateSpeed > 0 && shootsettings.powerAmount < shootsettings.maxPowerAmount) {
						if (Time.time > lastTimeUsed + regenerateTime) {
							getEnergy (regenerateSpeed * Time.deltaTime);
						}
					}
				} else {
					if (shootsettings.powerAmount < shootsettings.maxPowerAmount) {
						if (Time.time > lastTimeUsed + regenerateTime) {
							getEnergy (regenerateAmount);
							lastTimeUsed = Time.time;
						}
					}
				}
			}
		}

		firstPersonActive = playerCameraManager.isFirstPersonActive ();

		playerCurrentlyBusy = playerIsBusy ();

		//check that the player is not using a device, so all the key input can be checked
		if (!playerCurrentlyBusy) {
			if (!usedByAI) {
				if (powersModeActive) {
					if (changePowersWithNumberKeysActive && !playerInput.isUsingTouchControls ()) {
						//check if any keyboard number is preseed, and in that case, check which of it and if a power has that number associated
						int currentNumberInput = playerInput.checkNumberInput (shootsettings.powersSlotsAmount + 1);

						if (currentNumberInput > -1) {
							for (int k = 0; k < shootsettings.powersList.Count; k++) {
								if (shootsettings.powersList [k].numberKey == currentNumberInput && choosedPower != k) {
									if (shootsettings.powersList [k].powerEnabled) {
										choosedPower = k;

										powerChanged ();
									}
								}
							}
						}
					}

					if (isAimingPower ()) {
						checkAutoShootOnTag ();

						if (autoShootOnTagActive) {
							shootPower (true);
						}
					} else {
						if (autoShootOnTagActive) {
							resetAutoShootValues ();

							autoShootOnTagActive = false;
						}
					}

					if (useFreeAimMode) {
						float targetRotation = 0;

						if (!firstPersonActive) {
							if (checkToKeepPowersAfterAimingPowerFromShooting || weaponAimedFromFiringActive) {
								if (!freeAimModeeActive) {
									playerControllerManager.setIgnoreLookInCameraDirectionOnFreeFireActiveState (true);

									playerControllerManager.setIgnoreLookInCameraDirectionValue (true);

									headTrackManager.setHeadTrackActiveWhileAimingState (true);

									upperBodyRotationManager.enableOrDisableIKUpperBody (false);

									playerControllerManager.setIgnoreStrafeModeInputCheckActiveState (true);

									previousPlayerStatusID = playerControllerManager.getPlayerStatusID ();

									playerControllerManager.setPlayerStatusIDValue (freeAimModePlayerStatusID);

									freeAimModeeActive = true;
								}

								float armsAngle = Vector3.SignedAngle (transform.forward, playerCameraManager.transform.forward, transform.up);

								float ABSArmsAngle = Mathf.Abs (armsAngle);

								if (ABSArmsAngle > 45) {
									if (ABSArmsAngle > 70) {
										if (checkToLookAtRight) {
											if (armsAngle < 0) {
												if (ABSArmsAngle < 180 - freeAimModLookInOppositeDirectionExtraRange) {
													checkToLookAtLeft = true;

													checkToLookAtRight = false;
												}
											}
										} else {
											if (checkToLookAtLeft) {
												if (armsAngle > 0) {
													if (ABSArmsAngle < 180 - freeAimModLookInOppositeDirectionExtraRange) {
														checkToLookAtLeft = false;

														checkToLookAtRight = true;
													}
												}
											}
										}

										if (checkToLookAtRight) {
											if (armsAngle < 0) {
												targetRotation = -armsAngle;
											} else {
												targetRotation = armsAngle;
											}
										} 

										if (checkToLookAtLeft) {
											if (armsAngle > 0) {
												targetRotation = -armsAngle;
											} else {
												targetRotation = armsAngle;
											}
										}

										if (checkToLookAtRight) {
											if (!usingRightArm) {
												setAimModeSide (true);
											}
										} else {
											if (usingRightArm) {
												setAimModeSide (false);
											}
										}
									} else {
										targetRotation = armsAngle;
									}
								} else {
									if (!usingRightArm) {
										setAimModeSide (true);
									}

									targetRotation = armsAngle;
								}
							} else {
								if (freeAimModeeActive) {
									disableFreeAimArmsState ();
								}
							}
						}

						targetRotation *= armsPivotRotationMultiplier;

						targetRotation = Mathf.Clamp (targetRotation, -armsPivotClampRotation, armsPivotClampRotation);

						currentArmsPivotRotation = Mathf.Lerp (currentArmsPivotRotation, targetRotation, Time.deltaTime * armsPivotRotationSpeed);

						armsPivotRotationTransform.localEulerAngles = new Vector3 (0, 0, currentArmsPivotRotation);
					}
				}

				//if the wheel of the mouse rotates, the selected power is showed in the center of the screen a few seconds, and also changed in the hud
				if (selection) {
					powerSelectionTimer -= Time.deltaTime;

					if (powerSelectionTimer < 0) {
						powerSelectionTimer = 0.5f;
						selection = false;

						setSelectedPowerIconState (false);
					}
				}

				//if the touch controls are enabled, activate the swipe option
				if (playerInput.isUsingTouchControls ()) {
					//select the power by swiping the finger in the left corner of the screen, above the selected power icon
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

						//get the start position of the swipe
						if (fingerPressingTouchPanel && !touching) {
							swipeStartPos = currentTouch.position;
							touching = true;
						}

						//and the final position, and get the direction, to change to the previous or the next power
						if (currentTouch.phase == TouchPhase.Ended) {
							if (touching) {
								float swipeDistHorizontal = (new Vector3 (currentTouch.position.x, 0, 0) - new Vector3 (swipeStartPos.x, 0, 0)).magnitude;

								if (swipeDistHorizontal > shootsettings.minSwipeDist) {
									float swipeValue = Mathf.Sign (currentTouch.position.x - swipeStartPos.x);

									if (swipeValue > 0) {
										//right swipe
										choosePreviousPower ();
									} else if (swipeValue < 0) {
										//left swipe
										chooseNextPower ();
									}
								}

								touching = false;
							}

							fingerPressingTouchPanel = false;
						}
					}
				} else if (powersManagerLocated && powersManager.editingPowers) {
					//if the player is editing the power list using the power manager, disable the swipe checking
					touching = false;

					return;
				}
			}

			if (powersModeActive) {
				if (shootingBurst) {
					if (Time.time > lastShoot + currentPower.fireRate) {
						currentBurstAmount--;

						if (currentBurstAmount == 0) {
							powerShoot (false);
							shootingBurst = false;
						} else {
							powerShoot (true);
						}
					}
				}
			}

			//if the homing projectiles are being using, then
			if (homingProjectiles) {
				//while the number of located enemies is lowers that the max enemies amount, then
				if (locatedEnemies.Count < currentPower.homingProjectilesMaxAmount) {
					//uses a ray to detect enemies, to locked them
					bool surfaceFound = false;

					if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
						if (hit.collider != playerCollider) {
							surfaceFound = true;
						} else {
							if (Physics.Raycast (hit.point + mainCameraTransform.forward * 0.2f, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
								surfaceFound = true;
							}
						}
					}

					if (surfaceFound) {
						GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);

						if (target != null && target != gameObject) {
							if (currentPower.tagToLocate.Contains (target.tag)) {
								GameObject placeToShoot = applyDamage.getPlaceToShootGameObject (target);

								if (placeToShoot != null) {

									if (!locatedEnemies.Contains (placeToShoot)) {
										//if an enemy is detected, add it to the list of located enemies and instantiated an icon in screen to follow the enemy
										locatedEnemies.Add (placeToShoot);

										GKC_Utils.addElementToPlayerScreenObjectivesManager (gameObject, placeToShoot.gameObject, currentPower.locatedEnemyIconName);
									}
								}
							}
						}
					}
				}
			}
		}

		if (!firstPersonActive) {
			if (aimingInThirdPerson && (checkToKeepPowersAfterAimingPowerFromShooting || checkToKeepPowersAfterAimingPowerFromShooting2_5d)) {
				if (Time.time > lastTimeFired + timeToStopAimAfterStopFiring) {

					disableFreeFireModeAfterStopFiring ();
				}
			}
		}
	}

	void disableFreeAimArmsState ()
	{
		playerControllerManager.setIgnoreLookInCameraDirectionOnFreeFireActiveState (false);

		playerControllerManager.setIgnoreLookInCameraDirectionValue (false);

		headTrackManager.setHeadTrackActiveWhileAimingState (false);

		if (!usingRightArm) {
			setAimModeSide (true);
		}

		if (previousPlayerStatusID != -100) {
			playerControllerManager.setPlayerStatusIDValue (previousPlayerStatusID);

			previousPlayerStatusID = -100;
		}

		freeAimModeeActive = false;
	}

	public void setTouchingMenuPanelState (bool state)
	{
		fingerPressingTouchPanel = state;
	}

	public void disableFreeFireModeAfterStopFiring ()
	{
		headTrackManager.setOriginalCameraBodyWeightValue ();

		if (checkToKeepPowersAfterAimingPowerFromShooting) {
			playerCameraManager.setCheckToKeepAfterAimingFromShootingState (false);
		}

		checkToKeepPowersAfterAimingPowerFromShooting = false;

		checkToKeepPowersAfterAimingPowerFromShooting2_5d = false;

		playerControllerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

		aimModeInputPressed = false;

		useAimMode ();

		if (freeAimModeeActive) {
			disableFreeAimArmsState ();
		}
	}

	public void resetPowerFiringAndAimingIfPlayerDisabled ()
	{
		if (powersModeActive) {
			disableFreeFireModeAfterStopFiring ();

			aimOrKeepPowerInThirdPerson (false);

			disableFreeFireModeState ();

			shootPower (false);

			if (!firstPersonActive) {
				deactivateAimMode ();
			}

			IKSystemManager.disableArmsState ();
		}
	}

	//check if the player is using a device or using a game submen
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

	bool canUseInput ()
	{
		if (playerControllerManager.iscloseCombatAttackInProcess ()) {
			return false;
		}

		return true;
	}

	public void useAimMode ()
	{
		if (canMove && settings.aimModeEnabled && !firstPersonActive && !playerControllerManager.isSphereModeActive ()) {
			
			//check if the player is crouched, to prevent that the player enables the aim mode in a place where he can not get up
			if (playerControllerManager.isCrouching ()) {
				playerControllerManager.crouch ();
			}

			//if the player can get up, or was not crouched, allow to enable or disable the aim mode
			if (!playerControllerManager.isCrouching ()) {
				if ((!aimingPowerFromShooting && !checkToKeepPowersAfterAimingPowerFromShooting) || !aimingInThirdPerson) {
					//print ("aimorkeep");
					aimOrKeepPower ();
				}

				//print ("aimModeInputPressed " + aimModeInputPressed);
				if (aimModeInputPressed && (checkToKeepPowersAfterAimingPowerFromShooting || usingFreeFireMode)) {
					//print ("disable");
					disableFreeFireModeState ();
				}

				if (isAimingPower ()) {
					//print ("activateaimmode");
					activateAimMode ();
				} else {
					deactivateAimMode ();
				}

				if (runWhenAimingPowerInThirdPerson) {
					if (!aimingPowerFromShooting) {
						if (aimingInThirdPerson) {
							runningPreviouslyAiming = playerControllerManager.isPlayerRunning ();

							if (!runningPreviouslyAiming) {
								playerControllerManager.checkIfCanRun ();
							}
						} else {
							if (stopRunIfPreviouslyNotRunning) {
								if (!runningPreviouslyAiming) {
									playerControllerManager.stopRun ();
								}
							}
						}
					}
				}
			}
		}
	}

	public void aimOrKeepPower ()
	{
		if (firstPersonActive) {
			aimOrKeepPowerInFirstPerson (!aimingInFirstPerson);
		} else {
			aimOrKeepPowerInThirdPerson (!aimingInThirdPerson);
		}
	}

	public void aimOrKeepPowerInThirdPerson (bool state)
	{
		aimingInThirdPerson = state;

		playerControllerManager.setAimingPowersState (state);

		if (!state) {
			weaponAimedFromFiringActive = false;
		}
	}

	public void aimOrKeepPowerInFirstPerson (bool state)
	{
		aimingInFirstPerson = state;

		playerControllerManager.setAimingPowersState (state);
	}

	public bool isAimingPower ()
	{
		return aimingInFirstPerson || aimingInThirdPerson;
	}

	public bool isAimingPowerInFirstPerson ()
	{
		return aimingInFirstPerson;
	}

	public bool isAimingPowerInThirdPerson ()
	{
		return aimingInThirdPerson;
	}

	public void keepPower ()
	{
		aimOrKeepPowerInFirstPerson (false);

		aimOrKeepPowerInThirdPerson (false);
	}

	public void setAimOrKeepPowerState (bool firstPersonState, bool thirdPersonState)
	{
		aimOrKeepPowerInFirstPerson (firstPersonState);

		aimOrKeepPowerInThirdPerson (thirdPersonState);
	}

	public projectileInfo setPowerProjectileInfo (Powers selectedPower)
	{
		projectileInfo newProjectile = new projectileInfo ();

		newProjectile.isHommingProjectile = selectedPower.isHommingProjectile;

		newProjectile.isSeeker = selectedPower.isSeeker;
		newProjectile.targetOnScreenForSeeker = selectedPower.targetOnScreenForSeeker;

		newProjectile.waitTimeToSearchTarget = selectedPower.waitTimeToSearchTarget;

		newProjectile.useRayCastShoot = selectedPower.useRayCastShoot;

		newProjectile.useRaycastCheckingOnRigidbody = selectedPower.useRaycastCheckingOnRigidbody;
		newProjectile.customRaycastCheckingRate = selectedPower.customRaycastCheckingRate;
		newProjectile.customRaycastCheckingDistance = selectedPower.customRaycastCheckingDistance;

		newProjectile.projectileDamage = selectedPower.projectileDamage;
		newProjectile.projectileSpeed = selectedPower.projectileSpeed;

		newProjectile.impactForceApplied = selectedPower.impactForceApplied;
		newProjectile.forceMode = selectedPower.forceMode;
		newProjectile.applyImpactForceToVehicles = selectedPower.applyImpactForceToVehicles;
		newProjectile.impactForceToVehiclesMultiplier = selectedPower.impactForceToVehiclesMultiplier;

		newProjectile.projectileWithAbility = selectedPower.projectileWithAbility;

		newProjectile.impactSoundEffect = selectedPower.impactSoundEffect;

		newProjectile.scorch = selectedPower.scorch;
		newProjectile.scorchRayCastDistance = selectedPower.scorchRayCastDistance;

		newProjectile.owner = gameObject;

		newProjectile.projectileParticles = selectedPower.projectileParticles;
		newProjectile.impactParticles = selectedPower.impactParticles;

		newProjectile.isExplosive = selectedPower.isExplosive;
		newProjectile.isImplosive = selectedPower.isImplosive;
		newProjectile.useExplosionDelay = selectedPower.useExplosionDelay;
		newProjectile.explosionDelay = selectedPower.explosionDelay;
		newProjectile.explosionForce = selectedPower.explosionForce;
		newProjectile.explosionRadius = selectedPower.explosionRadius;
		newProjectile.explosionDamage = selectedPower.explosionDamage;
		newProjectile.pushCharacters = selectedPower.pushCharacters;
		newProjectile.canDamageProjectileOwner = selectedPower.canDamageProjectileOwner;
		newProjectile.applyExplosionForceToVehicles = selectedPower.applyExplosionForceToVehicles;
		newProjectile.explosionForceToVehiclesMultiplier = selectedPower.explosionForceToVehiclesMultiplier;

		newProjectile.killInOneShot = selectedPower.killInOneShot;

		newProjectile.useDisableTimer = selectedPower.useDisableTimer;
		newProjectile.noImpactDisableTimer = selectedPower.noImpactDisableTimer;
		newProjectile.impactDisableTimer = selectedPower.impactDisableTimer;

		newProjectile.targetToDamageLayer = shootsettings.targetToDamageLayer;
		newProjectile.targetForScorchLayer = targetForScorchLayer;

		newProjectile.useCustomIgnoreTags = shootsettings.useCustomIgnoreTags;
		newProjectile.customTagsToIgnoreList = shootsettings.customTagsToIgnoreList;  

		newProjectile.impactDecalIndex = selectedPower.impactDecalIndex;

		newProjectile.launchProjectile = selectedPower.launchProjectile;

		newProjectile.adhereToSurface = selectedPower.adhereToSurface;
		newProjectile.adhereToLimbs = selectedPower.adhereToLimbs;
		newProjectile.ignoreSetProjectilePositionOnImpact = selectedPower.ignoreSetProjectilePositionOnImpact;

		newProjectile.useGravityOnLaunch = selectedPower.useGravityOnLaunch;
		newProjectile.useGraivtyOnImpact = selectedPower.useGraivtyOnImpact;

		newProjectile.breakThroughObjects = selectedPower.breakThroughObjects;
		newProjectile.infiniteNumberOfImpacts = selectedPower.infiniteNumberOfImpacts;
		newProjectile.numberOfImpacts = selectedPower.numberOfImpacts;
		newProjectile.canDamageSameObjectMultipleTimes = selectedPower.canDamageSameObjectMultipleTimes;
		newProjectile.forwardDirection = mainCameraTransform.forward;

		newProjectile.damageTargetOverTime = selectedPower.damageTargetOverTime;
		newProjectile.damageOverTimeDelay = selectedPower.damageOverTimeDelay;
		newProjectile.damageOverTimeDuration = selectedPower.damageOverTimeDuration;
		newProjectile.damageOverTimeAmount = selectedPower.damageOverTimeAmount;
		newProjectile.damageOverTimeRate = selectedPower.damageOverTimeRate;
		newProjectile.damageOverTimeToDeath = selectedPower.damageOverTimeToDeath;
		newProjectile.removeDamageOverTimeState = selectedPower.removeDamageOverTimeState;

		newProjectile.sedateCharacters = selectedPower.sedateCharacters;
		newProjectile.sedateDelay = selectedPower.sedateDelay;
		newProjectile.useWeakSpotToReduceDelay = selectedPower.useWeakSpotToReduceDelay;
		newProjectile.sedateDuration = selectedPower.sedateDuration;
		newProjectile.sedateUntilReceiveDamage = selectedPower.sedateUntilReceiveDamage;

		newProjectile.pushCharacter = selectedPower.pushCharacter;
		newProjectile.pushCharacterForce = selectedPower.pushCharacterForce;
		newProjectile.pushCharacterRagdollForce = selectedPower.pushCharacterRagdollForce;

		newProjectile.useRemoteEventOnObjectsFound = selectedPower.useRemoteEventOnObjectsFound;
		newProjectile.remoteEventNameList = selectedPower.remoteEventNameList;

		newProjectile.useRemoteEventOnObjectsFoundOnExplosion = selectedPower.useRemoteEventOnObjectsFoundOnExplosion;
		newProjectile.remoteEventNameOnExplosion = selectedPower.remoteEventNameOnExplosion;

		newProjectile.ignoreShield = selectedPower.ignoreShield;
		newProjectile.canActivateReactionSystemTemporally = selectedPower.canActivateReactionSystemTemporally;
		newProjectile.damageReactionID = selectedPower.damageReactionID;

		newProjectile.damageTypeID = selectedPower.damageTypeID;

		newProjectile.projectilesPoolEnabled = projectilesPoolEnabled;

		newProjectile.maxAmountOfPoolElementsOnWeapon = maxAmountOfPoolElementsOnWeapon;

		newProjectile.projectileCanBeDeflected = selectedPower.projectileCanBeDeflected;
	
		return newProjectile;
	}

	public void enableMuzzleFlashLight ()
	{
		if (!currentPower.useMuzzleFlash) {
			return;
		}

		if (muzzleFlashCoroutine != null) {
			StopCoroutine (muzzleFlashCoroutine);
		}

		muzzleFlashCoroutine = StartCoroutine (enableMuzzleFlashCoroutine ());
	}

	IEnumerator enableMuzzleFlashCoroutine ()
	{
		currentPower.muzzleFlahsLight.gameObject.SetActive (true);

		yield return new WaitForSeconds (currentPower.muzzleFlahsDuration);

		currentPower.muzzleFlahsLight.gameObject.SetActive (false);

		yield return null;
	}

	//use the remaining power of the player, to use any of his powers
	public void usePowerBar (float amount)
	{
		if (infinitePower) {
			return;
		}

		shootsettings.powerAmount -= amount;

		auxPowerAmount = shootsettings.powerAmount;

		lastTimeUsed = Time.time;

		updateSlider (shootsettings.powerAmount);
	}

	public bool isThereEnergy ()
	{
		if (shootsettings.powerAmount > 0 || infinitePower) {
			return true;
		}

		return false;
	}

	public void updateSlider (float value)
	{
		if (settings.powerBar != null) {
			settings.powerBar.value = value;

			if (settings.powerBarText != null) {
				settings.powerBarText.text = value.ToString ("0");
			}

			if (hasPlayerStatsManager) {
				playerStatsManager.updateStatValue (energyStatName, shootsettings.powerAmount);
			}
		}
	}

	//if the player pick a enegy object, increase his energy value
	public void getEnergy (float amount)
	{
		if (!playerIsDead) {
			shootsettings.powerAmount += amount;

			//check that the energy amount is not higher that the energy max value of the slider
			if (shootsettings.powerAmount >= shootsettings.maxPowerAmount) {
				shootsettings.powerAmount = shootsettings.maxPowerAmount;
			}

			updateSlider (shootsettings.powerAmount);
		}

		auxPowerAmount = shootsettings.powerAmount;
	}

	public void removeEnergy (float amount)
	{
		shootsettings.powerAmount -= amount;

		//check that the energy amount is not higher that the energy max value of the slider
		if (shootsettings.powerAmount < 0) {
			shootsettings.powerAmount = 0;
		}

		updateSlider (shootsettings.powerAmount);

		auxPowerAmount = shootsettings.powerAmount;
	}

	//energy management
	public float getCurrentEnergyAmount ()
	{
		return shootsettings.powerAmount;
	}

	public float getMaxEnergyAmount ()
	{
		return shootsettings.maxPowerAmount;
	}

	public float getAuxEnergyAmount ()
	{
		return auxPowerAmount;
	}

	public void addAuxEnergyAmount (float amount)
	{
		auxPowerAmount += amount;
	}

	public float getEnergyAmountToLimit ()
	{
		return shootsettings.maxPowerAmount - auxPowerAmount;
	}

	public void increaseMaxPowerAmount (float newAmount)
	{
		shootsettings.maxPowerAmount += newAmount;

		updateSliderMaxValue (shootsettings.maxPowerAmount);
	}

	public void setPowerAmountOnMaxValue ()
	{
		getEnergy (shootsettings.maxPowerAmount - shootsettings.powerAmount);
	}

	public void updateSliderMaxValue (float newMaxValue)
	{
		if (settings.powerBar != null) {
			settings.powerBar.maxValue = newMaxValue;
		}
	}

	public void initializePowerAmount (float newValue)
	{
		shootsettings.powerAmount = newValue;
	}

	public void initializeMaxPowerAmount (float newValue)
	{
		shootsettings.maxPowerAmount = newValue;
	}

	public void initializeRegenerateSpeed (float newValue)
	{
		regenerateSpeed = newValue;
	}

	public void increaseRegenerateSpeed (float newValue)
	{
		regenerateSpeed += newValue;
	}

	//remove the localte enemies icons
	void removeLocatedEnemiesIcons ()
	{
		if (locatedEnemies.Count > 0) {
			for (int i = 0; i < locatedEnemies.Count; i++) {
				if (locatedEnemies [i] != null) {
					GKC_Utils.removeElementToPlayerScreenObjectivesManager (gameObject, locatedEnemies [i]);
				}
			}

			locatedEnemies.Clear ();
		}
	}

	//set the choosed power value in the next, changing the type of shoot action
	public void chooseNextPower ()
	{
		//if the wheel mouse or the change power button have been used and the powers can be changed, then
		if (amountPowersEnabled > 1 && settings.changePowersEnabled) {
			//increase the index
			int max = 0;
			int currentPowerIndex = currentPower.numberKey;
			currentPowerIndex++;

			//if the index is higher than the current powers slots, reset the index
			if (currentPowerIndex > shootsettings.powersSlotsAmount) {
				currentPowerIndex = 1;
			}

			bool exit = false;
			while (!exit) {
				//get which is the next power in the list, checking that it is enabled
				for (int k = 0; k < shootsettings.powersList.Count; k++) {
					if (shootsettings.powersList [k].powerEnabled && shootsettings.powersList [k].numberKey == currentPowerIndex) {
						choosedPower = k;
						exit = true;
					}
				}

				max++;
				if (max > 100) {
					//print ("forward error in index");
					return;
				}

				//set the current power
				currentPowerIndex++;
				if (currentPowerIndex > shootsettings.powersSlotsAmount) {
					currentPowerIndex = 1;
				}
			}

			//enable the power icon in the center of the screen
			powerChanged ();
		}
	}

	//set the choosed power value in the previous, changing the type of shoot action
	public void choosePreviousPower ()
	{
		//if the wheel mouse or the change power button have been used and the powers can be changed, then
		if (amountPowersEnabled > 1 && settings.changePowersEnabled) {
			//decrease the index
			int max = 0;
			int currentPowerIndex = currentPower.numberKey;
			currentPowerIndex--;

			//if the index is lower than 0, reset the index
			if (currentPowerIndex < 1) {
				currentPowerIndex = shootsettings.powersSlotsAmount;
			}

			bool exit = false;
			while (!exit) {
				//get which is the next power in the list, checking that it is enabled
				for (int k = shootsettings.powersList.Count - 1; k >= 0; k--) {
					if (shootsettings.powersList [k].powerEnabled && shootsettings.powersList [k].numberKey == currentPowerIndex) {
						choosedPower = k;
						exit = true;
					}
				}

				max++;
				if (max > 100) {
					//print ("backward error in index");
					return;
				}

				//set the current power
				currentPowerIndex--;
				if (currentPowerIndex < 1) {
					currentPowerIndex = shootsettings.powersSlotsAmount;
				}
			}

			//enable the power icon in the center of the screen
			powerChanged ();
		}
	}

	//every time that a power is selected, the icon of the power is showed in the center of the screen
	//and changed if the upper left corner of the screen
	void powerChanged ()
	{
		if (settings.changePowersEnabled) {
			selection = true;
			powerSelectionTimer = 0.5f;

			if (grabObjectsAttached) {
				grabObjectsManager.checkIfDropObjectIfNotPhysical (false);
			}

			currentPower = shootsettings.powersList [choosedPower];

			currentPowerName = currentPower.Name;

			if (shootsettings.selectedPowerHud != null) {
				shootsettings.selectedPowerHud.texture = currentPower.texture;
				shootsettings.selectedPowerIcon.texture = currentPower.texture;
			}

			checkCustomReticle ();

			setSelectedPowerIconState (true);

			removeLocatedEnemiesIcons ();

			checkParableTrayectory (true);
		}
	}

	public void checkParableTrayectory (bool parableState)
	{
		if (currentPower == null) {
			return;
		}

		//enable or disable the parable linerenderer
		if (((currentPower.activateLaunchParableThirdPerson && aimingInThirdPerson) ||
		    (currentPower.activateLaunchParableFirstPerson && aimingInFirstPerson)) && parableState && currentPower.launchProjectile) {
			if (parable != null) {
				parableGameObject.transform.position = shootsettings.shootZone.position;
				parable.shootPosition = shootsettings.shootZone;

				parable.changeParableState (true);
			}
		} else {
			if (parable != null) {
				parable.changeParableState (false);
			}
		}
	}
		
	//if the player edits the current powers in the wheel, when a power is changed of place, removed, or added, change its key number to change
	//and the order in the power list
	public void changePowerState (Powers powerToCheck, int numberKey, bool value, int index)
	{
		//change the state of the power sent
		powerToCheck.numberKey = numberKey;
	
		powerToCheck.powerAssigned = value;

		//increase or decrease the amount of powers enabled
		amountPowersEnabled += index;

		//if the current power is removed, select the previous
		if (amountPowersEnabled > 0 && !value && currentPower.Name.Equals (powerToCheck.Name)) {
			//decrease the index
			int max = 0;
			int currentPowerIndex = currentPower.numberKey;
			currentPowerIndex--;

			//if the index is lower than 0, reset the index
			if (currentPowerIndex < 1) {
				currentPowerIndex = shootsettings.powersSlotsAmount;
			}

			bool exit = false;
			while (!exit) {
				//get which is the next power in the list, checking that it is enabled
				for (int k = shootsettings.powersList.Count - 1; k >= 0; k--) {
					if (shootsettings.powersList [k].powerEnabled && shootsettings.powersList [k].numberKey == currentPowerIndex) {
						choosedPower = k;
						exit = true;
					}
				}

				max++;
				if (max > 100) {
					//print ("backward error in index");
					return;
				}

				//set the current power
				currentPowerIndex--;
				if (currentPowerIndex < 1) {
					currentPowerIndex = shootsettings.powersSlotsAmount;
				}
			}

			//enable the power icon in the center of the screen
			powerChanged ();
		}

		//if all the powers are disabled, disable the icon in the upper left corner of the screen
		if (amountPowersEnabled == 0) {
			shootsettings.selectedPowerHud.texture = null;

			if (shootsettings.selectedPowerHud.gameObject.activeSelf) {
				shootsettings.selectedPowerHud.gameObject.SetActive (false);
			}

			shootsettings.selectedPowerIcon.texture = null;
		} 

		//if only a power still enabled and the power is not selected, search and set it.
		else if (amountPowersEnabled == 1) {
			for (int k = 0; k < shootsettings.powersList.Count; k++) {
				if (shootsettings.powersList [k].powerEnabled) {
					choosedPower = k;

					if (!shootsettings.selectedPowerHud.gameObject.activeSelf) {
						shootsettings.selectedPowerHud.gameObject.SetActive (true);
					}

					shootsettings.selectedPowerHud.texture = currentPower.texture;

					currentPower = shootsettings.powersList [choosedPower];

					currentPowerName = currentPower.Name;
				}
			}
		}
	}

	//if the player selects a power using the wheel and the mouse, set the power closed to the mouse
	public void setPower (Powers power)
	{
		for (int k = 0; k < shootsettings.powersList.Count; k++) {
			if (shootsettings.powersList [k].powerEnabled && shootsettings.powersList [k].Name.Equals (power.Name)) {
				choosedPower = k;
				currentPower = shootsettings.powersList [choosedPower];

				currentPowerName = currentPower.Name;

				if (!shootsettings.selectedPowerHud.gameObject.activeSelf) {
					shootsettings.selectedPowerHud.gameObject.SetActive (true);
				}

				shootsettings.selectedPowerHud.texture = currentPower.texture;
			}
		}
	}

	public void shootPower (bool state)
	{
		if (state) {
			if (!shootingBurst) {
				if (shootsettings.powerAmount > 0) {
					shooting = true;
				} else {
					shooting = false;
				}

				powerShoot (state);

				setLastTimeFired ();
			}
		} else {
			shooting = false;

			powerShoot (state);
		}
	}

	//when the player is in aim mode, and press shoot, it is checked which power is selected, to create a bullet, push objects, etc...
	public void powerShoot (bool shootAtKeyDown)
	{
		if (((currentPower.useFireRate || currentPower.automatic) && Time.time < lastShoot + currentPower.fireRate) ||
		    (!currentPower.useFireRate && !currentPower.automatic && Time.time < lastShoot + minimumFireRate)) {
			return;
		} else {
			lastShoot = Time.time;
		}

		if ((isAimingPower () || firstPersonActive) && settings.shootEnabled && canMove) {
			if (shootsettings.powerAmount >= currentPower.amountPowerNeeded && amountPowersEnabled > 0 &&
			    (!grabObjectsAttached || !grabObjectsManager.isGrabbedObject () || grabObjectsManager.isCarryingMeleeWeapon ())) {

				checkPowerAbility (shootAtKeyDown);

				//If the current projectile is homming type, check when the shoot button is pressed and release
				if ((currentPower.isHommingProjectile && shootAtKeyDown)) {
					homingProjectiles = true;
					//print ("1 "+ shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHommingProjectile);
					return;
				}
					
				if ((currentPower.isHommingProjectile && !shootAtKeyDown && locatedEnemies.Count <= 0) ||
				    (!currentPower.isHommingProjectile && !shootAtKeyDown)) {
					homingProjectiles = false;
					//print ("2 "+shootAtKeyDown + " " + locatedEnemiesIcons.Count + " " + aimingHommingProjectile);
					return;
				}

				if (currentPower.automatic && currentPower.useBurst) {
					if (!shootingBurst && currentPower.burstAmount > 0) {
						shootingBurst = true;
						currentBurstAmount = currentPower.burstAmount;
					}
				}

				shootZoneAudioSource.PlayOneShot (currentPower.shootSoundEffect);
				//every power uses a certain amount of the power bar	

				usePowerBar (currentPower.amountPowerNeeded);

				if (!usedByAI) {
					checkPowerShake ();
				}

				if (currentPower.useEventToCall) {
					currentPower.eventToCall.Invoke ();

					return;
				}

				bool isLaunchingHomingProjectiles = (currentPower.isHommingProjectile && !shootAtKeyDown);
					
				if (!isLaunchingHomingProjectiles && !currentPower.powerWithAbility) {
					
					//if the player shoots, instantate the bullet and set its direction, velocity, etc...
					createShootParticles ();

					//use a raycast to check if there is any collider in the forward of the camera
					//if hit exits, then rotate the bullet in that direction, else launch the bullet in the camera direction

					if (projectilesPoolEnabled) {
						currentProjectile = GKC_PoolingSystem.Spawn (currentPower.projectile, shootsettings.shootZone.position, mainCameraTransform.rotation, maxAmountOfPoolElementsOnWeapon);
					} else {
						currentProjectile = (GameObject)Instantiate (currentPower.projectile, shootsettings.shootZone.position, mainCameraTransform.rotation);
					}

					Vector3 cameraDirection = mainCameraTransform.TransformDirection (Vector3.forward);

					bool armCrossingSurface = false;

					if (aimingInFirstPerson) {
						RaycastHit hitCamera;
						RaycastHit hitPower;

						if (Physics.Raycast (mainCameraTransform.position, cameraDirection, out hitCamera, Mathf.Infinity, layerToDamage)
						    && Physics.Raycast (shootsettings.shootZone.position, cameraDirection, out hitPower, Mathf.Infinity, layerToDamage)) {
							if (hitCamera.collider != hitPower.collider) {
								armCrossingSurface = true;
								//print ("crossing surface");
							} 
						}
					}

					if (!currentPower.launchProjectile) {
						if (!armCrossingSurface) {
							bool surfaceFound = false;

							if (Physics.Raycast (mainCameraTransform.position, cameraDirection, out hit, Mathf.Infinity, layerToDamage)) {
								if (hit.collider != playerCollider) {
									surfaceFound = true;
								} else {
									if (Physics.Raycast (hit.point + cameraDirection * 0.2f, cameraDirection, out hit, Mathf.Infinity, layerToDamage)) {
										surfaceFound = true;
									}
								}
							}

							if (surfaceFound) {
								//Debug.DrawRay (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward) * hit.distance, Color.red);
								currentProjectile.transform.LookAt (hit.point);
							}
						}
					}

					if (currentPower.launchProjectile) {
						Rigidbody projectileRigdibody = currentProjectile.GetComponent<Rigidbody> ();

						projectileRigdibody.isKinematic = true;

						//if the vehicle has a gravity control component, and the current gravity is not the regular one, add an artifical gravity component to the projectile
						//like this, it can make a parable in any surface and direction, setting its gravity in the same of the vehicle

						Vector3 currentNormal = playerControllerManager.getCurrentNormal ();

						if (currentNormal != Vector3.up) {
							currentProjectile.AddComponent<artificialObjectGravity> ().setCurrentGravity (-currentNormal);
						}

						if (currentPower.useParableSpeed) {
							//get the ray hit point where the projectile will fall
							bool surfaceFound = false;
							if (Physics.Raycast (mainCameraTransform.position, cameraDirection, out hit, Mathf.Infinity, layerToDamage)) {
								if (hit.collider != playerCollider) {
									surfaceFound = true;
								} else {
									if (Physics.Raycast (hit.point + cameraDirection * 0.2f, cameraDirection, out hit, Mathf.Infinity, layerToDamage)) {
										surfaceFound = true;
									}
								}
							}

							if (surfaceFound) {
								aimedZone = hit.point;
								objectiveFound = true;
							} else {
								objectiveFound = false;
							}
						}

						launchCurrentProjectile (projectileRigdibody, cameraDirection);
					}
						
					//add spread to the projectile
					Vector3 spreadAmount = Vector3.zero;
					if (currentPower.useProjectileSpread) {
						spreadAmount = setProjectileSpread ();
						currentProjectile.transform.Rotate (spreadAmount);
					}

					projectileSystem currentProjectileSystem = currentProjectile.GetComponent<projectileSystem> ();

					currentProjectileSystem.checkToResetProjectile ();
				
					currentProjectileSystem.setProjectileInfo (setPowerProjectileInfo (currentPower));

					bool projectileFiredByRaycast = false;

					bool projectileDestroyed = false;

					if (currentPower.useRayCastShoot || armCrossingSurface) {
						Vector3 forwardDirection = cameraDirection;

						if (spreadAmount.magnitude != 0) {
							forwardDirection = Quaternion.Euler (spreadAmount) * forwardDirection;
						}

						bool surfaceFound = false;

						if (Physics.Raycast (mainCameraTransform.position, forwardDirection, out hit, Mathf.Infinity, layerToDamage)) {
							if (hit.collider != playerCollider) {
								surfaceFound = true;
							} else {
								if (Physics.Raycast (hit.point + forwardDirection * 0.2f, forwardDirection, out hit, Mathf.Infinity, layerToDamage)) {
									surfaceFound = true;
								}
							}
						}

						if (surfaceFound) {
							currentProjectileSystem.rayCastShoot (hit.collider, hit.point, forwardDirection);

							projectileFiredByRaycast = true;
						} else {
							currentProjectileSystem.destroyProjectile ();

							projectileDestroyed = true;
						}
					}

					if (currentPower.isSeeker) {
						closestEnemy = setSeekerProjectileInfo ();

						if (closestEnemy != null) {
							currentProjectileSystem.setEnemy (closestEnemy);
						}
					}

					if (!projectileFiredByRaycast && !projectileDestroyed) {
						currentProjectileSystem.initializeProjectile ();
					}

					if (currentPower.applyForceAtShoot) {
						forceDirection = (mainCameraTransform.right * currentPower.forceDirection.x +
						mainCameraTransform.up * currentPower.forceDirection.y +
						mainCameraTransform.forward * currentPower.forceDirection.z) * currentPower.forceAmount;
						playerControllerManager.externalForce (forceDirection);
					}
				}

				activatePowerHandRecoil ();
			}

			// if the player is holding an object in the aim mode (not many in the normal mode) and press left button of the mouse
			//the gravity of this object is changed, sending the object in the camera direction, and the normal of the first surface that it touchs
			//will be its new gravity
			//to enable previous gravity of that object, grab again and change its gravity again, but this time aim to the actual ground with normal (0,1,0)
			if (grabObjectsAttached) {
				if (grabObjectsManager.isGrabbedObject () && !grabObjectsManager.isCarryingMeleeWeapon ()) {
					grabObjectsManager.checkGrabbedObjectAction ();
				}
			}
		}
	}

	public bool canPlayerMove ()
	{
		return canMove;
	}

	public bool isActionActiveInPlayer ()
	{
		return playerControllerManager.isActionActive ();
	}

	public void checkPowerAbility (bool keyDown)
	{
		if (currentPower.powerWithAbility) {
			if (keyDown) {
				if (currentPower.useDownButton) {
					currentPower.downButtonAction.Invoke ();
				}
			} else {
				if (currentPower.useUpButton) {
					currentPower.upButtonAction.Invoke ();
				}
			}
		}
	}

	public void activateSecondaryAction ()
	{
		if (currentPower.useSecondaryAction) {
			currentPower.secondaryAction.Invoke ();
		}
	}

	public void launchCurrentProjectile (Rigidbody projectileRigdibody, Vector3 cameraDirection)
	{
		//launch the projectile according to the velocity calculated according to the hit point of a raycast from the camera position
		projectileRigdibody.isKinematic = false;

		if (currentPower.useParableSpeed) {
			Vector3 newVel = getParableSpeed (currentProjectile.transform.position, aimedZone, cameraDirection);

			if (newVel == -Vector3.one) {
				newVel = currentProjectile.transform.forward * 100;
			}
			projectileRigdibody.AddForce (newVel, ForceMode.VelocityChange);
		} else {
			projectileRigdibody.AddForce (currentPower.parableDirectionTransform.forward * currentPower.projectileSpeed, ForceMode.Impulse);
		}
	}

	//calculate the speed applied to the launched projectile to make a parable according to a hit point
	Vector3 getParableSpeed (Vector3 origin, Vector3 target, Vector3 cameraDirection)
	{
		//if a hit point is not found, return
		if (!objectiveFound) {
			if (currentPower.useMaxDistanceWhenNoSurfaceFound) {
				target = origin + cameraDirection * currentPower.maxDistanceWhenNoSurfaceFound;
			} else {
				return -Vector3.one;
			}
		}

		//get the distance between positions
		Vector3 toTarget = target - origin;
		Vector3 toTargetXZ = toTarget;

		//remove the Y axis value
		toTargetXZ -= transform.InverseTransformDirection (toTargetXZ).y * transform.up;
		float y = transform.InverseTransformDirection (toTarget).y;
		float xz = toTargetXZ.magnitude;

		//get the velocity accoring to distance ang gravity
		float t = GKC_Utils.distance (origin, target) / 20;
		float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
		float v0xz = xz / t;

		//create result vector for calculated starting speeds
		Vector3 result = toTargetXZ.normalized;   

		//get direction of xz but with magnitude 1
		result *= v0xz;                     

		// set magnitude of xz to v0xz (starting speed in xz plane), setting the local Y value
		result -= transform.InverseTransformDirection (result).y * transform.up;
		result += transform.up * v0y;

		return result;
	}

	public Vector3 setProjectileSpread ()
	{
		float spreadAmount = currentPower.spreadAmount;

		Vector3 randomSpread = Vector3.zero;

		randomSpread.x = Random.Range (-spreadAmount, spreadAmount);
		randomSpread.y = Random.Range (-spreadAmount, spreadAmount);
		randomSpread.z = Random.Range (-spreadAmount, spreadAmount);

		return randomSpread;
	}

	//shoot to any object with the tag configured in the inspector, in case the option is enabled
	public void checkAutoShootOnTag ()
	{
		if (currentPower.autoShootOnTag) {
			Vector3 raycastPosition = mainCameraTransform.position;
			Vector3 raycastDirection = mainCameraTransform.TransformDirection (Vector3.forward);

			if (Physics.Raycast (raycastPosition, raycastDirection, out hit, currentPower.maxDistanceToRaycast, currentPower.layerToAutoShoot)) {

				currentTargetDetectedOnAutoShootOnTag = hit.collider.gameObject;

				if (previousTargetDetectedOnAutoShootOnTag == null || previousTargetDetectedOnAutoShootOnTag != currentTargetDetectedOnAutoShootOnTag) {

					previousTargetDetectedOnAutoShootOnTag = currentTargetDetectedOnAutoShootOnTag;

					GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);

					if (target == null) {
						target = hit.collider.gameObject;
					}

					if (currentPower.autoShootTagList.Contains (target.tag) ||
					    (currentPower.shootAtLayerToo &&
					    (1 << target.layer & currentPower.layerToAutoShoot.value) == 1 << target.layer)) {
						autoShootOnTagActive = true;

					} else {
						if (autoShootOnTagActive) {
							resetAutoShootValues ();

							autoShootOnTagActive = false;
						}
					}
				}
			} else {
				resetAutoShootValues ();
			}
		} else {
			resetAutoShootValues ();
		}
	}

	void resetAutoShootValues ()
	{
		if (autoShootOnTagActive) {
			shootPower (false);

			previousTargetDetectedOnAutoShootOnTag = null;
			currentTargetDetectedOnAutoShootOnTag = null;

			autoShootOnTagActive = false;
		}
	}

	public GameObject setSeekerProjectileInfo ()
	{
		if (mainCamera == null) {
			mainCamera = playerCameraManager.getMainCamera ();
		}

		return applyDamage.setSeekerProjectileInfo (shootsettings.shootZone.position, currentPower.tagToLocate, usingScreenSpaceCamera, 
			currentPower.targetOnScreenForSeeker, mainCamera, weaponsManager.targetToDamageLayer, transform.position, true, playerCollider);
	}

	public void createShootParticles ()
	{
		if (currentPower.shootParticles != null) {
			if (projectilesPoolEnabled) {
				shootParticles = GKC_PoolingSystem.Spawn (currentPower.shootParticles, shootsettings.shootZone.position, Quaternion.LookRotation (mainCameraTransform.forward), maxAmountOfPoolElementsOnWeapon);
			} else {
				shootParticles = (GameObject)Instantiate (currentPower.shootParticles, shootsettings.shootZone.position, Quaternion.LookRotation (mainCameraTransform.forward));
			}

			shootParticles.transform.position = shootsettings.shootZone.position;

			//shootParticles.transform.SetParent (shootsettings.shootZone);
		}
	}

	public void setLastTimeFired ()
	{
		lastTimeFired = Time.time;

		playerControllerManager.setLastTimeFiring ();
	}

	public float getLastTimeFired ()
	{
		return lastTimeFired;
	}

	//if the player dies, check if the player was aiming, grabbing and object, etc... and disable any necessary parameter
	public void death (bool state)
	{
		playerIsDead = state;

		if (state) {
			//check that the player is not in first person view to disable the aim mode
			if (!firstPersonActive) {
				deactivateAimMode ();
			}
		}
	}

	//functions to enable or disable the aim mode
	public void activateAimMode ()
	{
		if (grabObjectsAttached) {
			grabObjectsManager.checkIfDropObjectIfPhysical ();
		}

		if (!canFirePowersWithoutAiming || !aimingPowerFromShooting || (canFirePowersWithoutAiming && useAimCameraOnFreeFireMode)) {
			playerCameraManager.activateAiming (); 	
		}

		if (headLookWhenAiming) {
			IKSystemManager.setHeadLookState (true, headLookSpeed, headLookTarget);
		}

		playerControllerManager.enableOrDisableAiminig (true);		

		setUsingPowersState (true);

		enableOrDisablePowerCursor (true);

		//enable the grab objects mode in aim mode
		if (grabObjectsAttached) {
			grabObjectsManager.setAimingState (true);
		}

		IKSystemManager.changeArmState (1);

		if (!firstPersonActive && usePowerRotationPoint) {
			upperBodyRotationManager.setCurrentWeaponRotationPoint (powerRotationPoint, rotationPointInfo, -1);
			upperBodyRotationManager.setUsingWeaponRotationPointState (true);
		}

		upperBodyRotationManager.enableOrDisableIKUpperBody (true);

		if (useAimAssistInThirdPerson) {
			playerCameraManager.setCurrentLockedCameraCursor (cursorRectTransform);
			playerCameraManager.setMaxDistanceToCameraCenter (useMaxDistanceToCameraCenterAimAssist, maxDistanceToCameraCenterAimAssist);

			if (!playerCameraManager.isCameraTypeFree ()) {
				if (useAimAssistInLockedCamera) {
					playerCameraManager.setLookAtTargetOnLockedCameraState ();
				}
			}

			playerCameraManager.setLookAtTargetState (true, null);

			playerCameraManager.disableStrafeModeActivateFromNoTargetsFoundActive ();
		}

		//enable the parable linerenderer
		checkParableTrayectory (true);

		if (!usingFreeFireMode) {
			if (useLowerRotationSpeedAimedThirdPerson) {
				playerCameraManager.changeRotationSpeedValue (verticalRotationSpeedAimedInThirdPerson, horizontalRotationSpeedAimedInThirdPerson);
			}
		}
	}

	public void deactivateAimMode ()
	{
		if (headLookWhenAiming) {
			IKSystemManager.setHeadLookState (false, headLookSpeed, headLookTarget);
		}

		if (!ignoreDeactivateAiming) {
			playerCameraManager.deactivateAiming ();
		}

		playerControllerManager.enableOrDisableAiminig (false);

		setUsingPowersState (false);

		enableOrDisablePowerCursor (false);

		//disable the grab objects mode in aim mode, and drop any object that the player has grabbed
		if (grabObjectsAttached) {
			grabObjectsManager.checkIfDropObjectIfNotPhysical (true);
		}
			
		IKSystemManager.changeArmState (0);

		upperBodyRotationManager.enableOrDisableIKUpperBody (false);

		keepPower ();

		if (useAimAssistInThirdPerson) {
			playerCameraManager.setCurrentLockedCameraCursor (null);
			playerCameraManager.setMaxDistanceToCameraCenter (useMaxDistanceToCameraCenterAimAssist, maxDistanceToCameraCenterAimAssist);
			playerCameraManager.setLookAtTargetState (false, null);
		}

		//disable the parable linerenderer
		checkParableTrayectory (false);

		if (locatedEnemies.Count > 0) {
			homingProjectiles = false;
			//remove the icons in the screen
			removeLocatedEnemiesIcons ();
		}

		if (useLowerRotationSpeedAimedThirdPerson) {
			playerCameraManager.setOriginalRotationSpeed ();
		}
	}

	public void enableOrDisableIKOnPowersDuringAction (bool state)
	{
		actionActive = !state;

		upperBodyRotationManager.enableOrDisableIKUpperBody (!actionActive);

		IKSystemManager.changeArmState (actionActive ? 0 : 1);
	}

	public void setActionActiveState (bool state)
	{
		actionActive = state;
	}

	public void fireHomingProjectiles ()
	{
		//check that the located enemies are higher that 0
		if (locatedEnemies.Count > 0) {
			
			createShootParticles ();

			Quaternion targetRotation = mainCameraTransform.rotation;

			Vector3 targetPosition = shootsettings.shootZone.position;

			//shoot the missiles
			for (int i = 0; i < locatedEnemies.Count; i++) {
				if (projectilesPoolEnabled) {
					currentProjectile = GKC_PoolingSystem.Spawn (currentPower.projectile, targetPosition, targetRotation, maxAmountOfPoolElementsOnWeapon);
				} else {
					currentProjectile = (GameObject)Instantiate (currentPower.projectile, targetPosition, targetRotation);
				}

				projectileSystem currentProjectileSystem = currentProjectile.GetComponent<projectileSystem> ();

				currentProjectileSystem.checkToResetProjectile ();

				currentProjectileSystem.setProjectileInfo (setPowerProjectileInfo (currentPower));

				currentProjectileSystem.setEnemy (locatedEnemies [i]);

				bool projectileFiredByRaycast = false;

				if (currentPower.useRayCastShoot) {
					Vector3 forwardDirection = mainCameraTransform.TransformDirection (Vector3.forward);
					Vector3 forwardPositon = mainCameraTransform.position;

					bool surfaceFound = false;
					if (Physics.Raycast (forwardPositon, forwardDirection, out hit, Mathf.Infinity, layerToDamage)) {
						if (hit.collider != playerCollider) {
							surfaceFound = true;
						} else {
							if (Physics.Raycast (hit.point + forwardDirection * 0.2f, forwardDirection, out hit, Mathf.Infinity, layerToDamage)) {
								surfaceFound = true;
							}
						}
					}

					if (surfaceFound) {
						currentProjectileSystem.rayCastShoot (hit.collider, hit.point, forwardDirection);

						projectileFiredByRaycast = true;
					}
				}

				if (!projectileFiredByRaycast) {
					currentProjectileSystem.initializeProjectile ();
				}
			}

			//remove the icons in the screen
			removeLocatedEnemiesIcons ();

			//decrease the value of the power bar
			shootZoneAudioSource.PlayOneShot (currentPower.shootSoundEffect);
			usePowerBar (currentPower.amountPowerNeeded);

			activatePowerHandRecoil ();
		}

		homingProjectiles = false;
	}

	public void activatePowerHandRecoil ()
	{
		if (currentPower.useRecoil) {
			IKSystemManager.startRecoil (currentPower.recoilSpeed, currentPower.recoilAmount);
		}
	}

	public void enableOrDisablePowerCursor (bool state)
	{
		//show a cursor in the center of the screen to aim when the player is going to launch some objects
		if (settings.cursor != null) {
			if (settings.cursor.activeSelf != state) {
				settings.cursor.SetActive (state);
			}

			if (state) {
				checkCustomReticle ();
			} else {
				if (cursorRectTransform) {
					cursorRectTransform.localPosition = Vector3.zero;
				}
			}

			playerCameraManager.checkUpdateReticleActiveState (state);
		}
	}

	public void checkCustomReticle ()
	{
		if (settings.cursor != null) {
			if (settings.customReticle != null && settings.customReticle.activeSelf != currentPower.useCustomReticle) {
				settings.customReticle.SetActive (currentPower.useCustomReticle);
			}

			if (settings.regularReticle != null && settings.regularReticle.activeSelf != (!currentPower.useCustomReticle)) {
				settings.regularReticle.SetActive (!currentPower.useCustomReticle);
			}

			if (currentPower.useCustomReticle) {
				if (settings.customReticleImage != null) {
					settings.customReticleImage.texture = currentPower.customReticle;
				}
			} 
		}
	}

	public void changePlayerMode (bool state)
	{
		enableOrDisablePowerCursor (state);

		checkParableTrayectory (state);
	}

	public void changeCameraToThirdOrFirstView (bool isFirstPersonActive)
	{
		//in the normal mode, change camera from third to first and viceversa
		ignoreDeactivateAiming = true;

		deactivateAimMode ();

		ignoreDeactivateAiming = false;

		if (isFirstPersonActive) {
			//change the place where the projectiles are instantiated to a place below the camera
			shootsettings.shootZone.SetParent (mainCameraTransform.transform);
			shootsettings.shootZone.localPosition = shootsettings.firstPersonShootPosition.localPosition;
		} else {
			//change the place where the projectiles are instantiated back to the hand of the player
			shootsettings.shootZone.SetParent (aimsettings.handActive.transform);
			shootsettings.shootZone.localPosition = Vector3.zero;
		}

		shootsettings.shootZone.localRotation = Quaternion.identity;

		setAimOrKeepPowerState (isFirstPersonActive, false);

		if (powersModeActive) {
			checkParableTrayectory (true);

			if (isFirstPersonActive) {
				enableOrDisablePowerCursor (true);
			}

			setUsingPowersState (isFirstPersonActive);
		} else {
			setUsingPowersState (false);
		}
	}

	bool usingRightArm;

	//in the aim mode, the player can change the side to aim, left or right, moving the camera and changing the arm,
	//to configure the gameplay with the style of the player
	public void setAimModeSide (bool state)
	{
		//change to the right side, enabling the right arm
		if (state) {
			aimsettings.handActive = aimsettings.rightHand;

			IKSystemManager.changeArmSide (true);
		}
		//change to the left side, enabling the left arm
		else {
			aimsettings.handActive = aimsettings.leftHand;

			IKSystemManager.changeArmSide (false);
		}

		usingRightArm = state;

		if (!playerCameraManager.isFirstPersonActive ()) {
			shootsettings.shootZone.SetParent (aimsettings.handActive.transform);
			shootsettings.shootZone.localPosition = Vector3.zero;
			shootsettings.shootZone.localRotation = Quaternion.identity;
		}
	}

	public bool isUsingPowers ()
	{
		return usingPowers;
	}

	public void setUsingPowersState (bool state)
	{
		usingPowers = state;

		playerControllerManager.setUsingPowersState (state);
	}

	public void checkPowerShake ()
	{
		if (!currentPower.useShake) {
			return;
		}

		if (currentPower.sameValueBothViews) {
			headBobManager.setShotShakeState (currentPower.thirdPersonShakeInfo);
		} else {
			if (firstPersonActive && currentPower.useShakeInFirstPerson) {
				headBobManager.setShotShakeState (currentPower.firstPersonShakeInfo);
			}

			if (!firstPersonActive && currentPower.useShakeInThirdPerson) {
				headBobManager.setShotShakeState (currentPower.thirdPersonShakeInfo);
			}
		}
	}

	public void setPowersModeState (bool state)
	{
		powersModeActive = state;

		if (!powersModeActive && startInitialized) {
			disableFreeFireModeState ();
		}

		checkEventsOnStateChange (powersModeActive);

		enableOrDisablePowersInfoPanel (powersModeActive);
	}

	public void enableOrDisablePowersInfoPanel (bool state)
	{
		if (powersInfoPanel != null) {
			if (powersInfoPanel.activeSelf != state) {
				powersInfoPanel.SetActive (state);
			}
		}
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				evenOnStateEnabled.Invoke ();
			} else {
				eventOnStateDisabled.Invoke ();
			}
		}
	}

	public void checkIfChangePlayerMode ()
	{
		if (isUsingPowers ()) {
			changePlayerMode (true);
		}
	}

	public void checkIfChangePlayerModeAccordingToViewType ()
	{
		if (playerCameraManager.isFirstPersonActive ()) {
			changePlayerMode (true);
		} else {
			changePlayerMode (false);
		}
	}

	public bool isPowersModeActive ()
	{
		return powersModeActive;
	}

	public void disableFreeFireModeState ()
	{
		if (aimingPowerFromShooting) {
			playerCameraManager.setAimingFromShootingState (false);
		}

		aimingPowerFromShooting = false;

		if (checkToKeepPowersAfterAimingPowerFromShooting) {
			playerCameraManager.setCheckToKeepAfterAimingFromShootingState (false);
		}

		checkToKeepPowersAfterAimingPowerFromShooting = false;

		checkToKeepPowersAfterAimingPowerFromShooting2_5d = false;

		playerControllerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

		playerControllerManager.setUsingFreeFireModeState (false);

		usingFreeFireMode = false;

		weaponAimedFromFiringActive = false;
	}

	public void setOtherPowersInputPausedState (bool state)
	{
		otherPowersInputPaused = state;

		if (otherPowersInputPaused) {
			if (shooting) {
				shootPower (false);
			}
		}
	}


	//CALL INPUT FUNCTIONS
	public void inputNextOrPreviousPowerByButton (bool setNextPower)
	{
		if (!changePowersWithKeysActive) {
			return;
		}

		if (otherPowersInputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		//select the power using the mouse wheel or the change power buttons
		if (!playerCurrentlyBusy && powersModeActive) {
			if (setNextPower) {
				chooseNextPower ();
			} else {
				choosePreviousPower ();
			}
		}
	}

	public void inputNextOrPreviousPowerByMouse (bool setNextPower)
	{
		if (!changePowersWithMouseWheelActive) {
			return;
		}

		if (otherPowersInputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (!playerCurrentlyBusy && powersModeActive) {
			if (!grabObjectsAttached || ((!grabObjectsManager.canUseZoomWhileGrabbed && grabObjectsManager.isGrabbedObject ()) || !grabObjectsManager.isGrabbedObject ())) {
				if (setNextPower) {
					chooseNextPower ();
				} else {
					choosePreviousPower ();
				}
			}
		}
	}

	public void inputHoldOrReleaseShootPower (bool holdingButton)
	{
		if (otherPowersInputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		//according to the selected power, when the left button of the mouse is pressed, that power is activated
		if (!playerCurrentlyBusy && powersModeActive && !playerIsDead && playerControllerManager.canPlayerMove ()) {
			if (holdingButton) {

				if (!playerControllerManager.isPlayerNavMeshEnabled ()) {
					if (canFirePowersWithoutAiming && !firstPersonActive) {
						if (playerControllerManager.isPlayerMovingOn3dWorld ()) {
							if (!aimingInThirdPerson || checkToKeepPowersAfterAimingPowerFromShooting) {
								playerControllerManager.setUsingFreeFireModeState (true);

								usingFreeFireMode = true;

								aimingPowerFromShooting = true;

								playerCameraManager.setAimingFromShootingState (true);

								weaponAimedFromFiringActive = true;

								if (checkToKeepPowersAfterAimingPowerFromShooting) {
									playerCameraManager.setCheckToKeepAfterAimingFromShootingState (false);
								}

								checkToKeepPowersAfterAimingPowerFromShooting = false;
							}

							if (!aimingInThirdPerson) {
								playerControllerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

								aimModeInputPressed = false;

								useAimMode ();
							}
						} else {
							if (!aimingInThirdPerson) {
								checkToKeepPowersAfterAimingPowerFromShooting2_5d = true;
								useAimMode ();
							}
						}
					}
				}

				if (currentPower.automatic) {
					if (currentPower.useBurst) {
						shootPower (true);
					}
				} else {
					shootPower (true);
				}
			} else {
				shootPower (false);

				if (homingProjectiles) {
					//if the button to shoot is released, shoot a homing projectile for every located enemy
					fireHomingProjectiles ();
				}

				if (playerControllerManager.isPlayerMovingOn3dWorld ()) {
					if (aimingInThirdPerson && canFirePowersWithoutAiming && aimingPowerFromShooting && !firstPersonActive) {
						checkToKeepPowersAfterAimingPowerFromShooting = true;

						playerCameraManager.setCheckToKeepAfterAimingFromShootingState (true);

						playerControllerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (true);
					}
				}

				if (aimingPowerFromShooting) {
					playerCameraManager.setAimingFromShootingState (false);
				}

				aimingPowerFromShooting = false;

				playerControllerManager.setUsingFreeFireModeState (false);

				usingFreeFireMode = false;
			}
		}
	}

	public void inputHoldShootPower ()
	{
		if (otherPowersInputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (!playerCurrentlyBusy && powersModeActive && !playerIsDead && playerControllerManager.canPlayerMove ()) {

			if (!playerControllerManager.isPlayerNavMeshEnabled ()) {
				if (canFirePowersWithoutAiming && !firstPersonActive) {
					if (playerControllerManager.isPlayerMovingOn3dWorld ()) {
						if (!aimingInThirdPerson || checkToKeepPowersAfterAimingPowerFromShooting) {
							playerControllerManager.setUsingFreeFireModeState (true);

							usingFreeFireMode = true;

							aimingPowerFromShooting = true;

							playerCameraManager.setAimingFromShootingState (true);

							weaponAimedFromFiringActive = true;

							if (checkToKeepPowersAfterAimingPowerFromShooting) {
								playerCameraManager.setCheckToKeepAfterAimingFromShootingState (false);
							}

							checkToKeepPowersAfterAimingPowerFromShooting = false;
						}

						if (!aimingInThirdPerson) {

							playerControllerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

							aimModeInputPressed = false;

							useAimMode ();
						}
					} else {
						if (!aimingInThirdPerson) {
							checkToKeepPowersAfterAimingPowerFromShooting2_5d = true;

							useAimMode ();
						}
					}
				}
			}

			if (currentPower.automatic) {
				if (!currentPower.useBurst) {
					shootPower (true);
				}
			}
		}
	}

	public void inputSetAimPowerState (bool state)
	{
		if (otherPowersInputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (!playerCurrentlyBusy && powersModeActive) {
			aimModeInputPressed = state;

			useAimMode ();
		}
	}

	public void inputAimPower ()
	{
		if (otherPowersInputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		//activate or deactivate the aim mode, checking that the gravity power is active and nither the first person mode
		if (!playerCurrentlyBusy && powersModeActive) {
			aimModeInputPressed = !aimModeInputPressed;
		
			useAimMode ();
		}
	}

	public void inputActivateSecondaryAction ()
	{
		if (otherPowersInputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (!playerCurrentlyBusy && powersModeActive) {
			activateSecondaryAction ();
		}
	}

	//Set abilities and powers enabled and disable state, so they can be unlocked if the player activates them or disable if the situation needs it
	public void setRunEnabledState (bool state)
	{
		playerControllerManager.setSprintEnabledState (state);
	}

	public void setShootEnabledState (bool state)
	{
		settings.shootEnabled = state;
	}

	public void setCurrentPowerByName (string powerName)
	{
		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			if (shootsettings.powersList [i].powerEnabled && shootsettings.powersList [i].Name.Equals (powerName)) {
				choosedPower = i;

				powerChanged ();

				setSelectedPowerIconState (false);

				return;
			}
		}
	}

	public void shootPowerExternally ()
	{
		aimingInThirdPerson = true;

		shootPower (true);

		shootPower (false);

		aimingInThirdPerson = false;
	}

	public void shootPowerExternally (string powerName)
	{
		setCurrentPowerByName (powerName);

		shootPowerExternally ();
	}

	public void resetLastShoot ()
	{
		lastShoot = 0;
	}

	public void enableRegularPowerListElement (string powerName)
	{
		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			if (shootsettings.powersList [i].Name.Equals (powerName)) {
				if (!shootsettings.powersList [i].powerEnabled) {
					shootsettings.powersList [i].powerEnabled = true;

					if (powersManagerLocated) {
						powersManager.enableOrDisablePowerSlot (powerName, true);
					}

					updateAmountPowersEnabled ();

					choosedPower = i;

					powerChanged ();

					if (!shootsettings.selectedPowerHud.gameObject.activeSelf) {
						shootsettings.selectedPowerHud.gameObject.SetActive (true);
					}

					return;
				}
			}
		}
	}

	public void disableRegularPowerListElement (string powerName)
	{
		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			if (shootsettings.powersList [i].Name.Equals (powerName)) {
				if (shootsettings.powersList [i].powerEnabled) {
					shootsettings.powersList [i].powerEnabled = false;

					if (powersManagerLocated) {
						powersManager.enableOrDisablePowerSlot (powerName, false);
					}

					updateAmountPowersEnabled ();

					return;
				}
			}
		}
	}

	public void updateAmountPowersEnabled ()
	{
		amountPowersEnabled = 0;

		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			if (shootsettings.powersList [i].powerEnabled) {
				if (amountPowersEnabled + 1 <= shootsettings.powersSlotsAmount) {
					amountPowersEnabled++;
				}
			}
		}
	}

	public int getNumberOfPowersAvailable ()
	{
		int numberOfPowersAvailable = 0;

		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			if (shootsettings.powersList [i].powerEnabled) {
				numberOfPowersAvailable++;
			}
		}

		return numberOfPowersAvailable;
	}

	public void setFirstPowerAvailable ()
	{
		if (amountPowersEnabled > 0) {
			for (int i = 0; i < shootsettings.powersList.Count; i++) {
				if (shootsettings.powersList [i].powerEnabled) {
					choosedPower = i;
		
					powerChanged ();

					return;
				}
			}
		} else {
			shootsettings.selectedPowerHud.texture = null;

			if (shootsettings.selectedPowerHud.gameObject.activeSelf) {
				shootsettings.selectedPowerHud.gameObject.SetActive (false);
			}

			shootsettings.selectedPowerIcon.texture = null;
		}
	}

	public void setSelectedPowerIconState (bool state)
	{
		if (shootsettings.selectedPowerIcon != null && shootsettings.selectedPowerIcon.gameObject.activeSelf != state) {
			shootsettings.selectedPowerIcon.gameObject.SetActive (state);
		}
	}

	public Powers getCurrentPower ()
	{
		return currentPower;
	}

	public int getPowerIndexByNumberKey (int numberKeyToSearch)
	{
		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			if (shootsettings.powersList [i].powerEnabled && shootsettings.powersList [i].numberKey == numberKeyToSearch) {
				return i;
			}
		}

		return -1;
	}

	public void enableOrDisableAllPowers (bool state)
	{
		for (int i = 0; i < shootsettings.powersList.Count; i++) {
			shootsettings.powersList [i].powerEnabled = state;
		}

		updateComponent ();
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
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

	[System.Serializable]
	public class powersSettings
	{
		public bool aimModeEnabled;

		public bool shootEnabled;
		public bool changePowersEnabled;

		public GameObject cursor;
		public GameObject regularReticle;
		public GameObject customReticle;
		public RawImage customReticleImage;

		public Slider powerBar;
		public Text powerBarText;
	}

	[System.Serializable]
	public class aimSettings
	{
		public GameObject leftHand;
		public GameObject rightHand;
		public GameObject handActive;
	}

	[System.Serializable]
	public class shootSettings
	{
		public List<Powers> powersList = new List<Powers> ();

		public bool autoShootOnTag;
		public LayerMask layerToAutoShoot;
		public List<string> autoShootTagList = new List<string> ();
		public float maxDistanceToRaycast;
		public bool shootAtLayerToo;

		public LayerMask targetToDamageLayer;

		public bool useCustomIgnoreTags;
		public List<string> customTagsToIgnoreList = new List<string> ();

		public float powerAmount;
		public float maxPowerAmount;
		public int powersSlotsAmount;

		public int homingProjectilesMaxAmount;

		public RawImage selectedPowerIcon;
		public RawImage selectedPowerHud;
		public Transform shootZone;
		public Transform firstPersonShootPosition;

		public float minSwipeDist = 20;
	}

	[System.Serializable]
	public class Powers
	{
		public string Name;
		public GameObject projectile;
		public int numberKey;
		public bool useRayCastShoot;

		public bool useCustomReticle;
		public Texture customReticle;

		public bool useRaycastCheckingOnRigidbody;

		public float customRaycastCheckingRate;

		public float customRaycastCheckingDistance = 0.1f;

		public Texture texture;
		public bool powerEnabled = true;
		public bool powerAssigned = true;
		public float amountPowerNeeded;

		public bool useRecoil = true;
		public float recoilSpeed = 10;
		public float recoilAmount = 0.1f;

		public bool shootAProjectile;
		public bool launchProjectile;

		public bool projectileWithAbility;

		public bool powerWithAbility;
		public bool useDownButton;
		public UnityEvent downButtonAction;
		public bool useHoldButton;
		public UnityEvent holdButtonAction;
		public bool useUpButton;
		public UnityEvent upButtonAction;

		public bool useSecondaryAction;
		public UnityEvent secondaryAction;

		public bool automatic;

		public bool useBurst;
		public int burstAmount;

		public bool useFireRate;
		public float fireRate;
		public float projectileDamage;
		public float projectileSpeed;

		public bool useProjectileSpread;
		public float spreadAmount;

		public bool isExplosive;
		public bool isImplosive;
		public float explosionForce;
		public float explosionRadius;
		public bool useExplosionDelay;
		public float explosionDelay;
		public float explosionDamage;
		public bool pushCharacters;
		public bool canDamageProjectileOwner;
		public bool applyExplosionForceToVehicles = true;
		public float explosionForceToVehiclesMultiplier = 0.2f;

		public GameObject scorch;
		public float scorchRayCastDistance;

		public bool useEventToCall;
		public UnityEvent eventToCall;

		public bool autoShootOnTag;
		public LayerMask layerToAutoShoot;
		public List<string> autoShootTagList = new List<string> ();
		public float maxDistanceToRaycast;
		public bool shootAtLayerToo;

		public bool applyForceAtShoot;
		public Vector3 forceDirection;
		public float forceAmount;

		public bool isHommingProjectile;
		public bool isSeeker;
		public bool targetOnScreenForSeeker = true;
		public float waitTimeToSearchTarget;
		public int homingProjectilesMaxAmount;

		public float impactForceApplied;
		public ForceMode forceMode;
		public bool applyImpactForceToVehicles;
		public float impactForceToVehiclesMultiplier = 1;

		public float forceMassMultiplier = 1;

		public AudioClip shootSoundEffect;
		public AudioClip impactSoundEffect;

		public GameObject shootParticles;
		public GameObject projectileParticles;
		public GameObject impactParticles;

		public bool killInOneShot;

		public bool useDisableTimer;
		public float noImpactDisableTimer;
		public float impactDisableTimer;

		public List<string> tagToLocate = new List<string> ();
		public string locatedEnemyIconName = "Homing Located Enemy";

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

		public bool breakThroughObjects;
		public bool infiniteNumberOfImpacts;
		public int numberOfImpacts;
		public bool canDamageSameObjectMultipleTimes;

		public int impactDecalIndex;
		public string impactDecalName;

		public bool useShake;
		public bool useShakeInFirstPerson;
		public bool useShakeInThirdPerson;
		public bool sameValueBothViews;
		public IKWeaponSystem.weaponShotShakeInfo thirdPersonShakeInfo;
		public IKWeaponSystem.weaponShotShakeInfo firstPersonShakeInfo;
		public bool showShakeSettings;

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

		public bool useRemoteEventOnObjectsFound;
		public List<string> remoteEventNameList = new List<string> ();

		public bool useRemoteEventOnObjectsFoundOnExplosion;
		public string remoteEventNameOnExplosion;

		public bool ignoreShield;

		public bool canActivateReactionSystemTemporally;
		public int damageReactionID = -1;

		public int damageTypeID = -1;

		public bool projectileCanBeDeflected = true;
	}

	[System.Serializable]
	public class powerInfo
	{
		public string Name;
		public UnityEvent eventToEnable;
		public UnityEvent eventToDisable;
	}
}