using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class bowSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int regularBowMovementID;
	public int loadedBowMovementID;

	public bool activateStrafeOnlyWhenAimingBow;

	public bool useDrawHolsterActionAnimations = true;
	public string drawBowActionName = "Draw Bow";
	public string holsterBowActionName = "Holster Bow";

	public string loadArrowActionName = "Load Arrow";
	public string fireArrowActionName = "Fire Arrow";

	public float extraUpperBodyRotation = 90;

	public float targetRotation;

	public float minTimeToShootArrows = 2;

	public float minTimeToAimBow = 1.5f;

	public bool arrowsManagedByInventory;

	public bool activateBowCollisionWhenAiming;

	public bool stopAimBowOnLanding;

	[Space]
	[Header ("Third Person Camera State Settings")]
	[Space]

	public bool setNewCameraStateOnThirdPerson;

	public string newCameraStateOnThirdPerson;

	[Space]
	[Header ("Fire Bow Without Aim Settings")]
	[Space]

	public bool canFireBowWithoutAimEnabled;

	public float delayToFireBowWithoutAim;

	public bool autoFireArrowsWithoutAim;
	public float waitTimeToStopAutoFire = 0.5f;

	[Space]
	[Header ("Bow Weapon Types List")]
	[Space]

	public List<bowWeaponSystemInfo> bowWeaponSystemInfoList = new List<bowWeaponSystemInfo> ();

	[Space]

	public int currentBowWeaponID;

	[Space]
	[Header ("Bullet Time Settings")]
	[Space]

	public bool activateBulletTimeOnAir;
	public float bulletTimeScale;
	public float animationSpeedOnBulletTime = 1;

	public bool reduceGravityMultiplerOnBulletTimeOnAir;
	public float gravityMultiplerOnBulletTimeOnAir;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool setNewUpperBodyRotationValues;

	public float horizontalBendingMultiplier = 1;
	public float verticalBendingMultiplier = 1;

	public bool canMoveWhileUsingBowEnabled = true;

	public float cancelBowStateOnEmptyArrowsWaitDelay = 0.5f;

	[Space]
	[Header ("Durability Settings")]
	[Space]

	public bool ignoreDurability;
	public float generalAttackDurabilityMultiplier = 1;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool bowActive;

	public bool bowLoadedActive;

	public bool arrowIsFired;

	public bool aimingBowActive;

	public bool arrowsAvailableToFire = true;

	public bool checkArrowsOnInventory;

	public bool aimInputPressedDown;

	public bool bulletTimeOnAirActive;

	public int currentMultipleArrowTypeIndex;

	public bool aimInputPressedOnFireArrowWithoutAim;

	public bool fireArrowWithoutAimActive;

	public bool cancelBowCoroutineActive;

	public float lastTimePullBow;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnBowEnabled;
	public UnityEvent eventOnBowDisabled;

	public UnityEvent eventOnStartAim;
	public UnityEvent eventOnEndAim;

	public UnityEvent eventOnFireArrow;

	[Space]
	[Header ("Components")]
	[Space]

	public playerActionSystem mainPlayerActionSystem;
	public playerController mainPlayerController;
	public playerCamera mainPlayerCamera;

	public upperBodyRotationSystem mainUpperBodyRotationSystem;

	public headTrack mainHeadTrack;

	public simpleWeaponSystem currentSimpleWeaponSystem;

	public Transform positionToPlaceArrow;
	public Transform arrowPositionReference;

	public grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;

	public bowHolderSystem currentBowHolderSystem;

	public GameObject arrowMeshPrefab;
	public GameObject arrowMesh;

	public arrowManager mainArrowManager;

	float lastTimeShootArrow = 0;

	bowWeaponSystemInfo currentBowWeaponSystemInfo;

	float lastTimeAimBow;

	Coroutine checOnGrounCoroutine;

	Coroutine changeExtraRotation;

	Coroutine updateBowCoroutine;

	bool onGroundState;

	string previousCameraState = "";

	float lastTimeCheckFireArrowWithoutAim;

	bool pauseMoveInputActive;

	Coroutine cancelBowStateCoroutine;


	public void stopUpdateBowStateCoroutine ()
	{
		if (updateBowCoroutine != null) {
			StopCoroutine (updateBowCoroutine);
		}

		onGroundState = false;
	}

	IEnumerator updateBowStateCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			if (onGroundState) {
				if (!mainPlayerController.isPlayerOnGround ()) {				
					onGroundState = false;
				}

				if (!canMoveWhileUsingBowEnabled) {
					if (aimingBowActive || fireArrowWithoutAimActive) {
						if (!pauseMoveInputActive) {
							mainPlayerController.setMoveInputPausedState (true);
							mainPlayerController.resetPlayerControllerInput ();

							pauseMoveInputActive = true;
						}
					} else {
						if (pauseMoveInputActive) {
							mainPlayerController.setMoveInputPausedState (false);

							pauseMoveInputActive = false;
						}
					}
				}

				if (onGroundState) {
					if (mainPlayerController.isCrouching ()) {
						cancelBowState ();

						aimInputPressedDown = false;
					}
				}
			} else {
				if (mainPlayerController.isPlayerOnGround ()) {
					onGroundState = true;

					if (stopAimBowOnLanding) {
						if (aimingBowActive) {
							cancelBowState ();

							aimInputPressedDown = false;
						}
					}
				}

				if (pauseMoveInputActive) {
					mainPlayerController.setMoveInputPausedState (false);

					pauseMoveInputActive = false;
				}
			}

			if (fireArrowWithoutAimActive) {
				if (Time.time > lastTimeCheckFireArrowWithoutAim + delayToFireBowWithoutAim) {
					inputFireArrow ();

					if (!autoFireArrowsWithoutAim) {
						stopAimAfterFiringArrowWithoutAimActive = true;
					}

					lastTimeCheckFireArrowWithoutAim = Time.time;
				} else {
					if (stopAimAfterFiringArrowWithoutAimActive) {
						if (Time.time > lastTimeCheckFireArrowWithoutAim + waitTimeToStopAutoFire) {
							fireArrowWithoutAimActive = false;

							setAimState (false);

							stopAimAfterFiringArrowWithoutAimActive = false;
						}
					}
				}
			}
		}
	}

	bool stopAimAfterFiringArrowWithoutAimActive;

	public void setBowLoadedState (bool state)
	{
		bowLoadedActive = state;

		if (bowLoadedActive) {
			mainPlayerController.setCurrentStrafeIDValue (loadedBowMovementID);

			if (activateStrafeOnlyWhenAimingBow) {
				mainPlayerController.activateOrDeactivateStrafeMode (true);
			}

			mainPlayerActionSystem.activateCustomAction (loadArrowActionName);

			arrowIsFired = false;

			checkSpawnArrowInPlayerHand ();
		} else {
			mainPlayerController.setCurrentStrafeIDValue (regularBowMovementID);

			if (activateStrafeOnlyWhenAimingBow) {
				mainPlayerController.activateOrDeactivateStrafeMode (false);
			}

			setPositionToPlaceArrowActiveState (false);

			fireArrowWithoutAimActive = false;
		}

		mainUpperBodyRotationSystem.enableOrDisableIKUpperBody (bowLoadedActive);

		checkUpperBodyRotationSystemValues (bowLoadedActive);

		mainHeadTrack.setHeadTrackActiveWhileAimingState (bowLoadedActive);

		mainPlayerController.enableOrDisableAiminig (bowLoadedActive);		

		checkSetExtraRotationCoroutine (bowLoadedActive);

		setAimBowState (bowLoadedActive);

		stopCancelBowStateCoroutine ();

		cancelBowCoroutineActive = false;
	}

	void checkUpperBodyRotationSystemValues (bool state)
	{
		if (setNewUpperBodyRotationValues) {
			if (state) {
				mainUpperBodyRotationSystem.setHorizontalBendingMultiplierValue (horizontalBendingMultiplier);

				mainUpperBodyRotationSystem.setVerticalBendingMultiplierValue (verticalBendingMultiplier);
			} else {
				mainUpperBodyRotationSystem.setOriginalHorizontalBendingMultiplier ();

				mainUpperBodyRotationSystem.setOriginalVerticalBendingMultiplier ();
			}
		}
	}

	public void fireLoadedArrow ()
	{
		if (bowActive && bowLoadedActive) {
			mainPlayerActionSystem.activateCustomAction (fireArrowActionName);

			arrowIsFired = true;

			lastTimeShootArrow = Time.unscaledTime;

			eventOnFireArrow.Invoke ();

//			setPullBowState (false);

			if (arrowsManagedByInventory) {
				if (checkArrowsOnInventory) {
					mainArrowManager.useArrowFromInventory (1);

					checkIfArrowsFoundOnInventoryToCancelBowState ();
				}
			}

			mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnAttackOnCurrentWeapon (ignoreDurability, generalAttackDurabilityMultiplier);
		}
	}

	public void checkIfArrowsFoundOnInventoryToCancelBowState ()
	{
		if (arrowsManagedByInventory) {
			if (checkArrowsOnInventory) {
				mainArrowManager.checkIfArrowsFoundOnInventory ();

				if (!arrowsAvailableToFire) {
//					cancelBowState ();

					stopCancelBowStateCoroutine ();

					cancelBowStateCoroutine = StartCoroutine (activatCancelBowStateCoroutine ());
				}
			}
		}
	}

	void stopCancelBowStateCoroutine ()
	{
		if (cancelBowStateCoroutine != null) {
			StopCoroutine (cancelBowStateCoroutine);
		}

		cancelBowCoroutineActive = false;
	}

	IEnumerator activatCancelBowStateCoroutine ()
	{
		cancelBowCoroutineActive = true;

		yield return new WaitForSecondsRealtime (cancelBowStateOnEmptyArrowsWaitDelay);

		cancelBowCoroutineActive = false;

		cancelBowState ();
	}

	public void activateLoadBowAction ()
	{
		if (!aimingBowActive) {
			if (showDebugPrint) {
				print ("no aiming active, cancelling load bow action");
			}

			return;
		}

		if (arrowsManagedByInventory) {
			if (checkArrowsOnInventory) {
				mainArrowManager.checkIfArrowsFoundOnInventory ();

				if (!arrowsAvailableToFire) {

					return;
				}
			}
		}

		mainPlayerActionSystem.activateCustomAction (loadArrowActionName);
	}

	public void setAimBowState (bool state)
	{
		if (bowActive) {
			aimingBowActive = state;

			if (aimingBowActive) {
				eventOnStartAim.Invoke ();

				checkBowEventOnStartAim ();
			} else {
				eventOnEndAim.Invoke ();

				checkBowEventOnEndAim ();
			}

			if (!fireArrowWithoutAimActive) {
				checkCameraState (state);
			}

			if (activateBowCollisionWhenAiming) {
				mainGrabbedObjectMeleeAttackSystem.setGrabbedObjectClonnedColliderEnabledState (aimingBowActive);
			}

			if (activateBulletTimeOnAir) {
				if (aimingBowActive) {
					if (!mainPlayerController.isPlayerOnGround ()) {
						GKC_Utils.activateTimeBulletXSeconds (0, bulletTimeScale);

						bulletTimeOnAirActive = true;

						stopCheckIfPlayerOnGroundCoroutine ();

						checOnGrounCoroutine = StartCoroutine (checkIfPlayerOnGroundCoroutine (true));

						if (animationSpeedOnBulletTime != 1) {
							mainPlayerController.setReducedVelocity (animationSpeedOnBulletTime);
						}
					}
				} else {
					if (bulletTimeOnAirActive) {
						GKC_Utils.activateTimeBulletXSeconds (0, 1);

						bulletTimeOnAirActive = false;

						if (animationSpeedOnBulletTime != 1) {
							mainPlayerController.setNormalVelocity ();
						}

						if (reduceGravityMultiplerOnBulletTimeOnAir) {
							mainPlayerController.setGravityMultiplierValue (true, 0);
						}
					}
				}
			} else {
				stopCheckIfPlayerOnGroundCoroutine ();

				checOnGrounCoroutine = StartCoroutine (checkIfPlayerOnGroundCoroutine (false));
			}
		}
	}

	public void setBowActiveState (bool state)
	{
		if (bowActive == state) {
			return;
		}

		bowActive = state;

		if (bowActive) {
			if (useDrawHolsterActionAnimations) {
				if (Time.unscaledTime > mainGrabbedObjectMeleeAttackSystem.getLastTimeObjectReturn () + 0.7f &&
				    !mainGrabbedObjectMeleeAttackSystem.isCurrentWeaponThrown ()) {

					mainPlayerActionSystem.activateCustomAction (drawBowActionName);
				}
			}

			eventOnBowEnabled.Invoke ();

			Transform currentGrabbedObjectTransform = mainGrabbedObjectMeleeAttackSystem.getCurrentGrabbedObjectTransform ();

			if (currentGrabbedObjectTransform == null) {

				return;
			}

			currentBowHolderSystem = currentGrabbedObjectTransform.GetComponent<bowHolderSystem> ();

			currentBowWeaponID = currentBowHolderSystem.getBowWeaponID ();

			setCurrentBowWeaponSystemByID (currentBowWeaponID);
		
			if (arrowsManagedByInventory) {
				if (checkArrowsOnInventory) {
					mainArrowManager.updateArrowAmountText ();

					mainArrowManager.checkIfArrowsFoundOnInventory ();
				}
			}

			currentMultipleArrowTypeIndex = 0;
		} else {
			bool canPlayHolsterAction = true;

			if (mainPlayerController.isActionActive ()) {
				if (!mainPlayerController.canPlayerMove ()) {
					canPlayHolsterAction = false;
				}
			} else {
				if (!mainPlayerController.canPlayerMove ()) {
					canPlayHolsterAction = false;
				}
			}

//			print (canPlayHolsterAction);

			if (canPlayHolsterAction) {
				if (useDrawHolsterActionAnimations) {
					if (Time.unscaledTime > mainGrabbedObjectMeleeAttackSystem.getLastTimeObjectThrown () + 0.7f &&
					    !mainGrabbedObjectMeleeAttackSystem.isCurrentWeaponThrown ()) {

						mainPlayerActionSystem.activateCustomAction (holsterBowActionName);
					}
				}
			}

			eventOnBowDisabled.Invoke ();

			if (bowLoadedActive) {
				if (arrowIsFired) {
					mainPlayerActionSystem.stopCustomAction (loadArrowActionName);
				}

				mainUpperBodyRotationSystem.enableOrDisableIKUpperBody (false);

				checkUpperBodyRotationSystemValues (false);

				mainHeadTrack.setHeadTrackActiveWhileAimingState (false);

				mainPlayerController.enableOrDisableAiminig (false);		

				checkSetExtraRotationCoroutine (false);

				if (aimingBowActive && activateBowCollisionWhenAiming) {
					mainGrabbedObjectMeleeAttackSystem.setGrabbedObjectClonnedColliderEnabledState (false);
				}

				if (activateBulletTimeOnAir) {
					if (bulletTimeOnAirActive) {
						GKC_Utils.activateTimeBulletXSeconds (0, 1);

						bulletTimeOnAirActive = false;

						stopCheckIfPlayerOnGroundCoroutine ();

						if (animationSpeedOnBulletTime != 1) {
							mainPlayerController.setNormalVelocity ();
						}

						if (reduceGravityMultiplerOnBulletTimeOnAir) {
							mainPlayerController.setGravityMultiplierValue (true, 0);
						}
					}
				}

				aimingBowActive = false;

				bowLoadedActive = false;

				if (activateStrafeOnlyWhenAimingBow) {
					mainPlayerController.activateOrDeactivateStrafeMode (false);
				}

				eventOnEndAim.Invoke ();

				checkBowEventOnEndAim ();

				checkCameraState (false);

				if (lastTimePullBow == 0 || !mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
					if (showDebugPrint) {
						print ("cancel bow action");
					}

					checkEventOnCancelBowAction ();
				}
			}

			setPositionToPlaceArrowActiveState (false);

			currentBowHolderSystem = null;

			if (arrowsManagedByInventory) {
				if (checkArrowsOnInventory) {
					mainArrowManager.enableOrDisableArrowInfoPanel (bowActive);
				}
			}

			if (currentSimpleWeaponSystem != null) {
				currentSimpleWeaponSystem.enabled = false;
			}
		}

		aimInputPressedDown = false;

		stopUpdateBowStateCoroutine ();

		if (bowActive) {
			updateBowCoroutine = StartCoroutine (updateBowStateCoroutine ());
		}

		if (!bowActive) {
			if (pauseMoveInputActive) {
				mainPlayerController.setMoveInputPausedState (false);

				pauseMoveInputActive = false;
			}
		}
	}

	public void cancelBowState ()
	{
		if (bowLoadedActive) {
			setBowLoadedState (false);
		}
	}

	//if the player is in aim mode, enable the upper body to rotate with the camera movement
	public void checkSetExtraRotationCoroutine (bool state)
	{
		if (changeExtraRotation != null) {
			StopCoroutine (changeExtraRotation);
		}

		changeExtraRotation = StartCoroutine (setExtraRotation (state));
	}

	IEnumerator setExtraRotation (bool state)
	{
		if (extraUpperBodyRotation != 0 || (!state && targetRotation != extraUpperBodyRotation)) {
			for (float t = 0; t < 1;) {
				t += Time.unscaledDeltaTime;

				if (state) {
					targetRotation = Mathf.Lerp (targetRotation, extraUpperBodyRotation, t);
				} else {
					targetRotation = Mathf.Lerp (targetRotation, 0, t);
				}

				mainUpperBodyRotationSystem.setCurrentBodyRotation (targetRotation);

				yield return null;
			}
		}
	}

	public void stopCheckIfPlayerOnGroundCoroutine ()
	{
		if (checOnGrounCoroutine != null) {
			StopCoroutine (checOnGrounCoroutine);
		}
	}

	IEnumerator checkIfPlayerOnGroundCoroutine (bool checkBulletTime)
	{
		bool targetReached = false;

		bool checkGravityMultiplier = false;

		float lastTimeGravityMultiplierChecked = Time.unscaledTime;

		while (!targetReached) {

			if (checkBulletTime) {
				if (reduceGravityMultiplerOnBulletTimeOnAir) {
					if (!checkGravityMultiplier) {
						if (Time.unscaledTime > lastTimeGravityMultiplierChecked + 0.3f) {
							if (reduceGravityMultiplerOnBulletTimeOnAir) {
								mainPlayerController.setGravityMultiplierValue (false, gravityMultiplerOnBulletTimeOnAir);

								checkGravityMultiplier = true;
							} 
						}
					}
				}
			}

			if (mainPlayerController.isPlayerOnGround ()) {
				targetReached = true;
			}

			yield return null;
		}

		if (checkBulletTime) {
			if (activateBulletTimeOnAir) {
				if (bulletTimeOnAirActive) {
					GKC_Utils.activateTimeBulletXSeconds (0, 1);

					bulletTimeOnAirActive = false;

					if (animationSpeedOnBulletTime != 1) {
						mainPlayerController.setNormalVelocity ();
					}

					if (reduceGravityMultiplerOnBulletTimeOnAir) {
						mainPlayerController.setGravityMultiplierValue (true, 0);
					}
				}
			}
		}
	}

	public void checkSpawnArrowInPlayerHand ()
	{
		if (arrowMesh == null) {
			arrowMesh = (GameObject)Instantiate (arrowMeshPrefab);
			arrowMesh.transform.SetParent (positionToPlaceArrow);
			arrowMesh.transform.localPosition = arrowPositionReference.localPosition;
			arrowMesh.transform.localRotation = arrowPositionReference.localRotation;
		}
	}

	public void checkBowHolderLoadArrow ()
	{
		if (!aimingBowActive) {
			if (showDebugPrint) {
				print ("trying to load arrow when the bow is not being aimed, cancelling");
			}

			setPositionToPlaceArrowActiveState (false);

			return;
		}

		getCurrentBowHolderSystem ();

		if (currentBowHolderSystem != null) {
			currentBowHolderSystem.checkEventOnLoadArrow ();
		}
	}

	public void checkBowHolderFireArrow ()
	{
		getCurrentBowHolderSystem ();

		if (currentBowHolderSystem != null) {
			currentBowHolderSystem.checkEventOnFireArrow ();
		}
	}

	public void checkBowEventOnStartAim ()
	{
		getCurrentBowHolderSystem ();

		if (currentBowHolderSystem != null) {
			currentBowHolderSystem.checkEventOnStartAim ();
		}
	}

	public void checkBowEventOnEndAim ()
	{
		getCurrentBowHolderSystem ();

		if (currentBowHolderSystem != null) {
			currentBowHolderSystem.checkEventOnEndAim ();
		}
	}

	public void checkEventOnCancelBowAction ()
	{
		getCurrentBowHolderSystem ();

		if (currentBowHolderSystem != null) {
			currentBowHolderSystem.checkEventOnCancelBowAction ();
		}
	}

	void getCurrentBowHolderSystem ()
	{
		if (currentBowHolderSystem == null) {
			currentBowHolderSystem = mainPlayerController.GetComponentInChildren<bowHolderSystem> ();
		}
	}

	public void setCurrentBowWeaponSystemByID (int bowWeaponID)
	{
		for (int i = 0; i < bowWeaponSystemInfoList.Count; i++) {
			if (bowWeaponSystemInfoList [i].bowWeaponID == bowWeaponID) {
				currentBowWeaponSystemInfo = bowWeaponSystemInfoList [i];

				currentBowWeaponSystemInfo.isCurrentBowWeapon = true;

				currentSimpleWeaponSystem = currentBowWeaponSystemInfo.mainSimpleWeaponSystem;

				currentSimpleWeaponSystem.enabled = true;

				currentBowWeaponID = bowWeaponID;

				mainArrowManager.enableOrDisableArrowInfoPanel (currentBowWeaponSystemInfo.showArrowTypeIcon);

				mainArrowManager.setArrowTypeIcon (currentBowWeaponSystemInfo.bowIcon, !currentBowWeaponSystemInfo.checkArrowsOnInventory);

				if (arrowsManagedByInventory) {
					checkArrowsOnInventory = currentBowWeaponSystemInfo.checkArrowsOnInventory;

					if (checkArrowsOnInventory) {
						mainArrowManager.setCurrentArrowInventoryObjectName (currentBowWeaponSystemInfo.arrowInventoryObjectName);
					} 
				}

				if (currentBowWeaponSystemInfo.useEventOnBowSelected) {
					currentBowWeaponSystemInfo.eventOnBowSelected.Invoke ();
				}
			} else {
				if (bowWeaponSystemInfoList [i].isCurrentBowWeapon) {
					if (bowWeaponSystemInfoList [i].useEventOnBowSelected) {
						bowWeaponSystemInfoList [i].eventOnBowUnselected.Invoke ();
					}

					bowWeaponSystemInfoList [i].mainSimpleWeaponSystem.enabled = false;
				}

				bowWeaponSystemInfoList [i].isCurrentBowWeapon = false;
			}
		}
	}

	public void fireCurrentBowWeapon ()
	{
		if (bowActive) {
			currentSimpleWeaponSystem.inputShootWeaponOnPressDown ();

			currentSimpleWeaponSystem.inputShootWeaponOnPressUp ();
		}
	}

	public void setPullBowState (bool state)
	{
		if (currentBowWeaponSystemInfo != null && currentBowWeaponSystemInfo.usePullBowMultipliers) {
			if (state) {
				if (showDebugPrint) {
					print ("start to pull");
				}

				lastTimePullBow = Time.unscaledTime;

				if (showDebugPrint) {
					print ("set pull bow state true");
				}
			} else {
				if (lastTimePullBow > 0 && currentSimpleWeaponSystem != null) {
					float timeTime = Time.unscaledTime;

					if (showDebugPrint) {
						print ("time " + (timeTime - lastTimePullBow));
					}

					float totalPullBowForce = (timeTime - lastTimePullBow) / currentBowWeaponSystemInfo.pullBowForceRateMultiplier;

					totalPullBowForce = currentBowWeaponSystemInfo.initialPullBowForceRateMultiplier - totalPullBowForce;

					if (totalPullBowForce <= 0) {
						totalPullBowForce = 1;
					}


					float totalPullBowDamage = (timeTime - lastTimePullBow) * currentBowWeaponSystemInfo.pullBowDamageRateMultiplier;

					totalPullBowDamage += 1;

					totalPullBowDamage = Mathf.Clamp (totalPullBowDamage, 1, currentBowWeaponSystemInfo.maxPullBowDamageRateMultiplier);

					if (showDebugPrint) {
						print (totalPullBowForce + " " + totalPullBowDamage);
					}

					List<projectileSystem> lastProjectilesSystemListFired = currentSimpleWeaponSystem.getLastProjectilesSystemListFired ();

					if (lastProjectilesSystemListFired.Count > 0) {
						for (int i = 0; i < lastProjectilesSystemListFired.Count; i++) {

							if (lastProjectilesSystemListFired [i] != null) {
								if (showDebugPrint) {
									print ("setting projectile info");
								}

								lastProjectilesSystemListFired [i].setProjectileDamageMultiplier (totalPullBowDamage);

								arrowProjectile currentArrowProjectile = lastProjectilesSystemListFired [i].gameObject.GetComponent<arrowProjectile> ();

								if (currentArrowProjectile != null) {

									currentArrowProjectile.setNewArrowDownForce (totalPullBowForce);

									if (currentBowWeaponSystemInfo.checkSurfaceTypeDetected) {
										currentArrowProjectile.setArrowSurfaceTypeInfoList (currentBowWeaponSystemInfo.arrowSurfaceTypeInfoList);
									}
								}
							}
						}
					}
				}

				lastTimePullBow = 0;
			}
		} else {
			if (state) {
				lastTimePullBow = Time.unscaledTime;
			} else {
				lastTimePullBow = 0;
			}
		}
	}

	public void setArrowsAvailableToFireState (bool state)
	{
		arrowsAvailableToFire = state;
	}

	public void setPositionToPlaceArrowActiveState (bool state)
	{
		if (positionToPlaceArrow.gameObject.activeSelf != state) {
			positionToPlaceArrow.gameObject.SetActive (state);
		}
	}

	public bool canUseWeaponsInput ()
	{
		if (mainPlayerController.canPlayerMove () && !mainPlayerController.playerIsBusy ()) {
			return true;
		}

		return false;
	}

	void checkCameraState (bool state)
	{
		bool isFirstPersonActive = mainPlayerController.isPlayerOnFirstPerson ();

		if (setNewCameraStateOnThirdPerson && !isFirstPersonActive) {
			if (mainPlayerCamera != null) {
				if (state) {
					previousCameraState = mainPlayerCamera.getCurrentStateName ();

					mainPlayerCamera.setCameraStateOnlyOnThirdPerson (newCameraStateOnThirdPerson);
				} else {

					if (previousCameraState != "") {
						if (previousCameraState != newCameraStateOnThirdPerson) {
							mainPlayerCamera.setCameraStateOnlyOnThirdPerson (previousCameraState);
						}

						previousCameraState = "";
					}
				}
			}
		}
	}

	void setAimState (bool state)
	{
		if (bowActive) {
			if (showDebugPrint) {
				print ("input aim bow " + state + " " + bowActive);
			}

			if (!canUseWeaponsInput ()) {
				if (state) {
					return;
				} else {
					if (!aimInputPressedDown) {
						print ("no input down pressed before, cancelling");

						return;
					}
				}
			}

			if (!bowLoadedActive && state) {
				if (arrowsManagedByInventory) {
					mainArrowManager.checkIfArrowsFoundOnInventory ();

					if (!arrowsAvailableToFire) {
						if (currentSimpleWeaponSystem != null && bowWeaponSystemInfoList [currentBowWeaponID].checkArrowsOnInventory) {
							return;
						}
					}
				}
			} else {
				if (!state && !bowLoadedActive) {
					if (arrowsManagedByInventory) {
						mainArrowManager.checkIfArrowsFoundOnInventory ();

						if (!arrowsAvailableToFire) {
							if (showDebugPrint) {
								print ("no arrows available when releasing the aim button");
							}

							return;
						}
					}
				}
			}

			aimInputPressedDown = state;

			bool bowLoadedButNotFired = bowLoadedActive && !arrowIsFired;

			if (bowLoadedButNotFired) {
				if (showDebugPrint) {
					print ("bow loaded but not fired");
				}

				mainPlayerActionSystem.stopCustomAction (loadArrowActionName);
			}

			setBowLoadedState (state);

			lastTimeAimBow = Time.unscaledTime;

			lastTimePullBow = 0;

			if (!state) {
				setPositionToPlaceArrowActiveState (false);
			}

			if (showDebugPrint) {
				print ("aim " + state);
			}
		}
	}

	//INPUT FUNCTIONS
	public void inputSetAimBowState (bool state)
	{
		if (state) {
			if (mainPlayerController.isCrouching ()) {
				mainPlayerController.crouch ();

				if (mainPlayerController.isCrouching ()) {
					return;
				}
			}
		}

		if (fireArrowWithoutAimActive) {
//			setAimState (false);		
//
//			setAimState (true);

			checkCameraState (true);

			if (state) {
				aimInputPressedOnFireArrowWithoutAim = true;
			}
		} else {
			setAimState (state);		
		}

		fireArrowWithoutAimActive = false;

		stopAimAfterFiringArrowWithoutAimActive = false;

		if (!state) {
			aimInputPressedOnFireArrowWithoutAim = false;
		}
	}

	public void inputFireArrow ()
	{
		if (bowActive) {
			if (!canUseWeaponsInput ()) {
				return;
			}

			if (Time.unscaledTime > minTimeToShootArrows + lastTimeShootArrow &&
			    Time.unscaledTime > lastTimeAimBow + minTimeToAimBow && lastTimePullBow > 0 && bowLoadedActive) {

				fireLoadedArrow ();

				if (showDebugPrint) {
					print ("fire");
				}

				print ("bowLoadedActive " + bowLoadedActive);

				return;
			} 

			if (canFireBowWithoutAimEnabled && !bowLoadedActive) {
				fireArrowWithoutAimActive = true;

				lastTimeCheckFireArrowWithoutAim = Time.time;

				setAimState (true);
			}
		}
	}

	public void inputStopFireArrowWithoutAim ()
	{
		if (bowActive) {
			if (fireArrowWithoutAimActive) {
				if (!aimInputPressedOnFireArrowWithoutAim) {
					setAimState (false);
				}

				lastTimeCheckFireArrowWithoutAim = 0;

				fireArrowWithoutAimActive = false;

				stopAimAfterFiringArrowWithoutAimActive = false;
			}
		}
	}

	public void inputChangeArrowType ()
	{
		if (bowActive) {
			if (!canUseWeaponsInput ()) {
				return;
			}

			if (aimingBowActive) {
				return;
			}

			if (currentBowHolderSystem.canUseMultipleArrowType) {

				currentMultipleArrowTypeIndex++;

				if (currentMultipleArrowTypeIndex >= currentBowHolderSystem.bowWeaponIDList.Count) {
					currentMultipleArrowTypeIndex = 0;
				}

				setCurrentBowWeaponSystemByID (currentBowHolderSystem.bowWeaponIDList [currentMultipleArrowTypeIndex]);

				if (arrowsManagedByInventory) {
					if (checkArrowsOnInventory) {
						mainArrowManager.updateArrowAmountText ();

						mainArrowManager.checkIfArrowsFoundOnInventory ();
					}
				}
			}
		}
	}

	[System.Serializable]
	public class bowWeaponSystemInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;

		public int bowWeaponID;

		public bool isCurrentBowWeapon;

		[Space]
		[Header ("Inventory Settings")]
		[Space]

		public bool checkArrowsOnInventory;

		public string arrowInventoryObjectName;

		[Space]
		[Header ("Surface Settings")]
		[Space]

		public bool checkSurfaceTypeDetected;

		public List<arrowSurfaceTypeInfo> arrowSurfaceTypeInfoList = new List<arrowSurfaceTypeInfo> ();

		[Space]
		[Header ("Other Settings")]
		[Space]

		public simpleWeaponSystem mainSimpleWeaponSystem;

		public bool showArrowTypeIcon = true;

		public Texture bowIcon;

		[Space]
		[Header ("Arrow Force And Damage Settings")]
		[Space]

		public bool usePullBowMultipliers;

		public float pullBowForceRateMultiplier = 0.1f;

		public float initialPullBowForceRateMultiplier = 30;

		public float pullBowDamageRateMultiplier = 0.1f;

		public float maxPullBowDamageRateMultiplier = 10;

		[Space]
		[Header ("Events Settings")]
		[Space]
	
		public bool useEventOnBowSelected;

		public UnityEvent eventOnBowSelected;

		public UnityEvent eventOnBowUnselected;
	}

	[System.Serializable]
	public class arrowSurfaceTypeInfo
	{
		public string Name;

		public bool isObstacle;

		public bool arrowBounceOnSurface;

		public bool dropArrowPickupOnBounce;

		public bool addExtraForceOnBounce;

		public float extraForceOnBounce;
	}
}
