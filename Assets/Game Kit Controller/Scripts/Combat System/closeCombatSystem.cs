using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;

[System.Serializable]
public class closeCombatSystem : MonoBehaviour
{
	public bool combatSystemEnabled;
	public LayerMask layerMaskToDamage;

	public bool useCustomIgnoreTags;
	public List<string> customTagsToIgnoreList = new List<string> ();

	public float addForceMultiplier;

	public bool setCanActivateReactionSystemTemporallyState;
	public bool canActivateReactionSystemTemporally;

	float defaultArmHitDamage = 10;
	float defaultLegHitDamage = 15;

	public string attackIDAnimatorName = "Attack ID";

	public List<combatTypeInfo> combatTypeInfoList = new List<combatTypeInfo> ();

	public List<combatLimbInfo> combatLimbList = new List<combatLimbInfo> ();

	public bool currentPlayerMode;

	public bool useAnimationPercentageDuration = true;
	public bool useAnimationPercentageOver100;

	public bool setCombatIdleID;
	public int combatIdleID = 1;

	public int combatTypeID = 0;

	public enum colliderPlace
	{
		leg,
		arm,
		both,
		right_leg,
		left_leg,
		right_arm,
		left_arm,
		both_legs,
		both_arms,
		in_front
	}

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;


	public bool useStrafeMode;
	public int strafeIDUsed;
	public bool setPreviousStrafeModeOnDisableMode;

	bool previousStrafeMode;
	int previousStrafeID = -1;

	public bool activateStrafeModeOnLockOnTargetActive;

	public bool toggleStrafeModeIfRunningActive;

	public headTrack mainHeadTrack;
	public playerController playerControllerManager;
	public Animator animator;

	public bool useMatchTargetSystemOnAttack;
	public bool ignoreAttackSettingToMatchTarget;
	public bool useMainMatchPositionOffset;
	public float mainMatchPositionOffset;
	public matchPlayerToTargetSystem mainMatchPlayerToTargetSystem;


	public GameObject hitCombatPrefab;

	public float timerCombat = 0;

	public bool combatPlaying;

	float lastTimeComboAttack;

	public string currentComboInProcessName;
	public string previousComboInProcessName;

	public bool checkSurfaceInfoEnabled = true;

	public string surfaceInfoOnMeleeAttackNameForNotFound = "Regular";
	public string surfaceInfoOnMeleeAttackNameForSwingOnAir = "Swing On Air";
	public List<grabbedObjectMeleeAttackSystem.surfaceInfoOnMeleeAttack> surfaceInfoOnMeleeAttackList = 
		new List<grabbedObjectMeleeAttackSystem.surfaceInfoOnMeleeAttack> ();

	public AudioSource mainAudioSource;


	//Block Settings
	public bool blockEnabled;

	public bool blockActive;

	public float generalBlockProtectionMultiplier = 1;

	public string cancelBlockReactionStateName = "Disable Has Exit Time State";

	public string mainMeleeCombatBlockInputName = "Block Attack";

	public string blockActionName = "Block Close Combat";

	public int blockID;

	public bool blockActivePreviously;

	public healthManagement mainHealth;
	public playerInputManager playerInput;

	public bool useMaxBlockRangeAngle;
	public float maxBlockRangeAngle;

	public float blockDamageProtectionAmount;

	public float reducedBlockDamageProtectionAmount;

	public bool reducedBlockDamageProtectionActive;

	public bool canCancelBlockToStartAttackActive;

	public bool blockInputPaused;

	public bool useEventsOnBlockDamage;
	public UnityEvent eventOnBlockActivate;
	public UnityEvent eventOnBlockDeactivate;

	public bool useEventsOnAttackCantBeBlocked;
	public UnityEvent eventOnAttackCantBeBlocked;

	public bool attackInputPaused;

	public bool useStaminaOnAttackEnabled;
	public bool attackInputPausedForStamina;
	public string attackStaminaStateName = "Close Combat";
	public float generalStaminaUseMultiplier = 1;
	public staminaSystem mainStaminaSystem;

	bool staminaSystemLocated;

	public bool playerOnGroundToActivateAttack = true;

	public bool ignoreGetHealthFromDamagingObjects = true;

	public bool getHealthFromBlocks;
	public float healthAmountFromBlocks = 1;

	public bool getHealthFromPerfectBlocks;
	public float healthAmountFromPerfectBlocks = 1;

	//Gizmo Settings
	public bool showDebugPrint;

	public bool showGizmo;
	public Color gizmoColor;
	public float gizmoRadius;
	public Color gizmoLabelColor;

	int attackIDAnimatorID;

	public bool canMoveStatePaused;

	public bool rootMotionStateChanged;

	public bool headTrackStateChanged;

	public bool headTrackLookInOppositeDirectionStateChanged;

	bool combatModeActivatedTemporally;

	public bool canUseAttacksWithoutCombatActiveEnabled;

	public bool hideMeleeWeaponsOnExternalCombat = true;
	public bool hideFireWeaponsOnExternalCombat;

	bool carryingWeaponsPreviously;


	Coroutine damageTriggerCoroutine;

	damageTriggerActiveInfo currentTriggerInfo;

	Coroutine eventInfoListCoroutine;

	List<int> limbIndexListOnCurrentAttack = new List<int> ();

	Coroutine resetAttackIDCoroutine;

	bool currentAttackCanBeBlocked;

	Coroutine pauseMeleeAttackInputCoroutine;
	Coroutine pauseBlockInputCoroutine;

	public bool activateCloseCombatSystemAtStart;

	//Editor variables
	public bool showMovementSettings;
	public bool showMatchPositionSettings;
	public bool showBlockSettings;
	public bool showSurfaceSettings;
	public bool showCombatLimbListSettings;
	public bool showAttackListSettings;

	public bool showEventsSettings;

	public bool showDebugSettings;
	public bool showAllSettings;
	public bool showComponents;

	bool checkIfActivateCloseCombatSystemAtStart;

	bool gravityChangedActive;
	public bool checkExternalControllerBehaviorActiveList;

	public List<string> externalControllerBehaviorNameListToUseCloseCombat;

	public bool usingAttacksOnAir;

	void Start ()
	{
		attackIDAnimatorID = Animator.StringToHash (attackIDAnimatorName);

		if (useStaminaOnAttackEnabled) {
			staminaSystemLocated = mainStaminaSystem != null;
		}
	}

	void Update ()
	{
		if (timerCombat > 0) {
			timerCombat -= Time.deltaTime;

			if (timerCombat < 0) {
				disableCombat ();
			}
		}

		if (!checkIfActivateCloseCombatSystemAtStart) {
			if (activateCloseCombatSystemAtStart) {
				setCurrentPlayerMode (true);
			}
				
			checkIfActivateCloseCombatSystemAtStart = true;
		}
	}

	public void disableCurrentAttackInProcess ()
	{
		stopAllAttacks ();
	}

	float currentWaitTimeToNextAttack = 0;

	public bool canActivateAttack ()
	{
		if (currentWaitTimeToNextAttack == 0) {
			return true;
		} else {
			if (Time.time > lastTimeComboAttack + currentWaitTimeToNextAttack) {
				return true;
			} else {
				return false;
			}
		}
	}

	public void useAttack (string attackName)
	{
		if (!canUseCombat ()) {
			return;
		}

		bool canActivateAttackOnAir = false;

		bool isPlayerOnGround = playerControllerManager.isPlayerOnGround ();

		if (isPlayerOnGround) {
			if (usingAttacksOnAir) {

				usingAttacksOnAir = false;
			}

		} else {
			if (playerOnGroundToActivateAttack) {
				return;
			} else {
				canActivateAttackOnAir = true;

				if (!usingAttacksOnAir) {


					usingAttacksOnAir = true;
				}
			}
		}

		if (showDebugPrint) {
			print ("\n\n");

			print (attackName);
		}

		int combatTypeInfoListCount = combatTypeInfoList.Count;

		for (int i = 0; i < combatTypeInfoListCount; i++) {

			bool canPlayAttack = true;

			combatTypeInfo currentCombatTypeInfo = combatTypeInfoList [i];

			if (combatTypeID == currentCombatTypeInfo.combatTypeID) {

				if (isPlayerOnGround) {
					if (currentCombatTypeInfo.attacksUsedOnAir) {
						canPlayAttack = false;
					}
				} else {
					if (!currentCombatTypeInfo.attacksUsedOnAir) {
						canPlayAttack = false;
					}

					if (!canActivateAttackOnAir) {
						canPlayAttack = false;
					}
				}

				if (canPlayAttack && currentCombatTypeInfo.Name.Equals (attackName)) {
					currentComboInProcessName = attackName;

					//Check changes between previous and new combo
					if (currentComboInProcessName != previousComboInProcessName && currentComboInProcessName != "" && previousComboInProcessName != "") {

						if (showDebugPrint) {
							print ("changing from " + previousComboInProcessName + " to " + currentComboInProcessName);
						}

						combatTypeInfo previousCombatTypeInfo = getCombatTypeInfoByName (previousComboInProcessName);

						bool cancelCombo = false;

						if (!previousCombatTypeInfo.canChangeOfComboWhenPreviousNotComplete) {
							if (previousCombatTypeInfo.comboCounter < previousCombatTypeInfo.combatAttackInfoList.Count ||
							    previousCombatTypeInfo.attackActive) {

								if (showDebugPrint) {
									print ("trying to change of combo when current is not complete");
								}

								cancelCombo = true;
							}
						}

						if (previousCombatTypeInfo.useMinTimeToChangeCombo) {
							if (showDebugPrint) {
								print ("checking if combo change is possible");
							}

							float minTimeToPlayNextAttack = previousCombatTypeInfo.combatAttackInfoList [previousCombatTypeInfo.comboCounter].minTimeToPlayNextAttack;

							if (Time.time < lastTimeComboAttack + minTimeToPlayNextAttack) {
								if (showDebugPrint) {
									print ("change of combo is not possible yet for time");
								}

								cancelCombo = true;

								currentWaitTimeToNextAttack = 0;
							} else {
								currentWaitTimeToNextAttack = minTimeToPlayNextAttack;
							}
						}

//						if (canActivateAttackOnAir) {
//							if (currentCombatTypeInfo.currentAttackIndex < currentCombatTypeInfo.combatAttackInfoList.Count) {
//								if (currentCombatTypeInfo.attackActive) {
//
//									combatAttackInfo currentCombatAttackInfo = currentCombatTypeInfo.combatAttackInfoList [currentCombatTypeInfo.comboCounter];
//								
//									if (currentCombatAttackInfo.playerOnGroundToActivateAttack) {
//										cancelCombo = true;
//									}
//								}
//							}
//						}

						if (cancelCombo) {
							if (showDebugPrint) {
								print ("combo cancelled");
							}

							currentComboInProcessName = previousComboInProcessName;

							return;
						}
					}

					previousComboInProcessName = currentComboInProcessName;

					if (currentCombatTypeInfo.waitingToStartAttackActive) {
						if (Time.time < currentCombatTypeInfo.lastTimeFullCombo + currentCombatTypeInfo.delayToStartAttackAfterFullCombo) {
							return;
						} else {
							currentCombatTypeInfo.waitingToStartAttackActive = false;
						}
					}
	
					//Check the info of the current attack to make from the selected combo
					if (currentCombatTypeInfo.currentAttackIndex < currentCombatTypeInfo.combatAttackInfoList.Count) {
						string debugInfo = combatTypeInfoList [i].Name;

						bool newAttackPerformedCorrectly = false;

						if (currentCombatTypeInfo.attackActive) {

							combatAttackInfo currentCombatAttackInfo = currentCombatTypeInfo.combatAttackInfoList [currentCombatTypeInfo.comboCounter];

							if (showDebugPrint) {
								print ("Checking info of the combo counter of " + currentCombatAttackInfo.Name);
							}

							float minTimeToPlayNextAttack = currentCombatAttackInfo.minTimeToPlayNextAttack;

							if (Time.time > lastTimeComboAttack + minTimeToPlayNextAttack) {
								if (!currentCombatAttackInfo.playingNextAttack) {
									if (showDebugPrint) {
										print ("can play next attack");
									}

									currentCombatAttackInfo.playingNextAttack = true;

									currentCombatTypeInfo.comboCounter++;

									if (currentCombatTypeInfo.comboCounter < currentCombatTypeInfo.combatAttackInfoList.Count) {

										stopResetAttackIdOnAnimatorCoroutine ();

										resetAttackIDAnimatorID ();

										combatAttackInfo nextAttackInfo = currentCombatTypeInfo.combatAttackInfoList [currentCombatTypeInfo.comboCounter];

										if (nextAttackInfo.useAttackID) {

											animator.SetInteger (attackIDAnimatorID, nextAttackInfo.attackID);

											resetAttackIdOnAnimator ();
										} else {
											string nextAttackName = nextAttackInfo.Name;
											animator.CrossFadeInFixedTime (nextAttackName, 0.1f);
										}

										if (showDebugPrint) {
											print ("next attack " + nextAttackInfo.Name);
										}

										lastTimeComboAttack = Time.time;

										newAttackPerformedCorrectly = true;

										setMoveInputPausedState (!nextAttackInfo.canMoveWhileAttackActive);

										setRootMotionState (nextAttackInfo.useRootMotionOnAttack);

										setHeadTrackState (nextAttackInfo.pauseHeadTrackOnAttack);

										setHeadTrackLookInOppositeDirection (nextAttackInfo.pauseHeadTrackLookInOppositeDirection);

										if (nextAttackInfo.disableGravityOnAttacks) {
											disablePlayerGravityDuringAttack (true);
										}

										currentWaitTimeToNextAttack = minTimeToPlayNextAttack;
									}
								}
							} else {
								if (currentCombatAttackInfo.resetComboIfIncorrectMinTime) {
									if (showDebugPrint) {
										print ("incorrect timing, resetting combo when animation ends");
									}

									currentCombatTypeInfo.resetingCombo = true;

									currentWaitTimeToNextAttack = 0;
								}
							}
						} else {
							currentCombatTypeInfo.attackActive = true;

							combatAttackInfo currentCombatAttackInfo = currentCombatTypeInfo.combatAttackInfoList [0];

							stopResetAttackIdOnAnimatorCoroutine ();

							resetAttackIDAnimatorID ();

							if (currentCombatAttackInfo.useAttackID) {
								animator.SetInteger (attackIDAnimatorID, currentCombatAttackInfo.attackID);

								resetAttackIdOnAnimator ();
							} else {
								animator.CrossFadeInFixedTime (currentCombatAttackInfo.Name, 0.1f);
							}

							if (showDebugPrint) {
								print (" attack " + currentCombatAttackInfo.Name);
							}

							lastTimeComboAttack = Time.time;

							newAttackPerformedCorrectly = true;

							setMoveInputPausedState (!currentCombatAttackInfo.canMoveWhileAttackActive);

							setRootMotionState (currentCombatAttackInfo.useRootMotionOnAttack);

							setHeadTrackState (currentCombatAttackInfo.pauseHeadTrackOnAttack);

							setHeadTrackLookInOppositeDirection (currentCombatAttackInfo.pauseHeadTrackLookInOppositeDirection);

							if (currentCombatAttackInfo.disableGravityOnAttacks) {
								disablePlayerGravityDuringAttack (true);
							}
						}

						combatPlaying = true;

						playerControllerManager.setCloseCombatAttackInProcessState (true);

						resetAttacksIndex (i);

						bool increaseAttackIndex = false;
							
						if (currentCombatTypeInfo.increaseAttackIndexOnlyOnAttackPerformedCorrectly) {
							if (newAttackPerformedCorrectly) {
								increaseAttackIndex = true;
							}
						} else {
							increaseAttackIndex = true;
						}

						if (increaseAttackIndex) {
							// add to the timer the time of the animation
							timerCombat += (currentCombatTypeInfo.combatAttackInfoList [currentCombatTypeInfo.currentAttackIndex].attackDuration /
							currentCombatTypeInfo.combatAttackInfoList [currentCombatTypeInfo.currentAttackIndex].animationSpeed);

							currentCombatTypeInfo.currentAttackIndex++;
						}

						if (showDebugPrint) {
							print (debugInfo + " performed correctly " + newAttackPerformedCorrectly + " increase index " + increaseAttackIndex);
						}

					} else {
						if (currentCombatTypeInfo.useDelayToStartAttackAfterFullCombo) {
							currentCombatTypeInfo.waitingToStartAttackActive = true;

							currentCombatTypeInfo.lastTimeFullCombo = Time.time;
						}
					}

					return;
				}
			}
		}
	}

	public combatTypeInfo getCombatTypeInfoByName (string comboName)
	{
		for (int i = 0; i < combatTypeInfoList.Count; i++) {
			combatTypeInfo currentCombatTypeInfo = combatTypeInfoList [i];

			if (combatTypeID == currentCombatTypeInfo.combatTypeID) {
				if (currentCombatTypeInfo.Name.Equals (comboName)) {
					return currentCombatTypeInfo;
				}
			}
		}

		return null;
	}

	public void setAttackStartState (string attackName)
	{
		int combatTypeInfoListCount = combatTypeInfoList.Count;

		for (int i = 0; i < combatTypeInfoListCount; i++) {
			combatTypeInfo currentCombatTypeInfo = combatTypeInfoList [i];

			if (combatTypeID == currentCombatTypeInfo.combatTypeID) {

				for (int j = 0; j < currentCombatTypeInfo.combatAttackInfoList.Count; j++) {

					combatAttackInfo currentCombatAttackInfo = currentCombatTypeInfo.combatAttackInfoList [j];

					if (currentCombatAttackInfo.Name.Equals (attackName)) {

						if (currentCombatAttackInfo.useEventOnAttackStart) {

							if (currentCombatAttackInfo.useDelayOnEventOnAttackStart) {
								callEventWithDelay (currentCombatAttackInfo, true);
							} else {
								currentCombatAttackInfo.eventOnAttackStart.Invoke ();
							}
						}

						if (showDebugPrint) {
							print ("starting " + attackName);
						}

						changeCollidersState (true, currentCombatAttackInfo.limbForAttack, i, j);

						checkEventInfoList (currentCombatAttackInfo);

						if (currentCombatAttackInfo.useExtraDamageMultiplier) {
							setCollidersExtraDamageValue (currentCombatAttackInfo.extraDamageMultiplier);
						} else {
							setCollidersExtraDamageValue (1);
						}

						if (useMatchTargetSystemOnAttack && !usingAttacksOnAir) {

							if (currentCombatAttackInfo.useMatchPositionSystem || ignoreAttackSettingToMatchTarget) {
								float currentMatchOffset = mainMatchPositionOffset;

								if (!useMainMatchPositionOffset) {
									currentMatchOffset = currentCombatAttackInfo.matchPositionOffset;
								}

								mainMatchPlayerToTargetSystem.activateMatchPosition (currentMatchOffset);
							}
						}

						if (useStaminaOnAttackEnabled) {
							if (currentCombatAttackInfo.useStaminaOnAttack) {
								if (staminaSystemLocated) {
									mainStaminaSystem.activeStaminaStateWithCustomAmount (attackStaminaStateName, 
										currentCombatAttackInfo.staminaUsedOnAttack * generalStaminaUseMultiplier, 
										currentCombatAttackInfo.customRefillStaminaDelayAfterUse);	
								}
							}
						}

						return;
					}
				}
			}
		}
	}

	public void setGeneralStaminaUseMultiplierValue (float newValue)
	{
		generalStaminaUseMultiplier = newValue;
	}

	public void setAttackInputPausedForStaminaState (bool state)
	{
		attackInputPausedForStamina = state;
	}

	public void setAttackEndState (string attackName)
	{
		int combatTypeInfoListCount = combatTypeInfoList.Count;

		for (int i = 0; i < combatTypeInfoListCount; i++) {
			combatTypeInfo currentCombatTypeInfo = combatTypeInfoList [i];

			if (combatTypeID == currentCombatTypeInfo.combatTypeID) {

				for (int j = 0; j < currentCombatTypeInfo.combatAttackInfoList.Count; j++) {

					combatAttackInfo currentCombatAttackInfo = currentCombatTypeInfo.combatAttackInfoList [j];

					if (currentCombatAttackInfo.Name.Equals (attackName)) {
						if (showDebugPrint) {
							print ("end " + attackName);
						}

						if (currentCombatAttackInfo.useEventOnAttackEnd) {
							if (currentCombatAttackInfo.useDelayOnEventOnAttackEnd) {
								callEventWithDelay (currentCombatAttackInfo, false);
							} else {
								currentCombatAttackInfo.eventOnAttackEnd.Invoke ();
							}
						}

						if (currentCombatTypeInfo.resetingCombo) {
							if (showDebugPrint) {
								print ("reset combo");
							}

							disableCombat ();

							return;
						}

						if ((currentCombatTypeInfo.currentAttackIndex - 1) > currentCombatTypeInfo.comboCounter || currentCombatAttackInfo.playingNextAttack) {
							if (currentCombatTypeInfo.comboCounter <= j) {
								currentCombatAttackInfo.playingNextAttack = false;

								currentCombatTypeInfo.comboCounter++;

								if (currentCombatTypeInfo.comboCounter < currentCombatTypeInfo.combatAttackInfoList.Count) {
									combatAttackInfo attackToUse = currentCombatTypeInfo.combatAttackInfoList [currentCombatTypeInfo.comboCounter];

									stopResetAttackIdOnAnimatorCoroutine ();

									resetAttackIDAnimatorID ();

									if (attackToUse.useAttackID) {
										animator.SetInteger (attackIDAnimatorID, attackToUse.attackID);

										resetAttackIdOnAnimator ();
									} else {
										animator.CrossFadeInFixedTime (attackToUse.Name, 0.1f);
									}

									lastTimeComboAttack = Time.time;

									if (showDebugPrint) {
										print ("next attack " + attackToUse.Name);
									}
										
									setMoveInputPausedState (!attackToUse.canMoveWhileAttackActive);

									setRootMotionState (currentCombatAttackInfo.useRootMotionOnAttack);

									setHeadTrackState (currentCombatAttackInfo.pauseHeadTrackOnAttack);

									setHeadTrackLookInOppositeDirection (currentCombatAttackInfo.pauseHeadTrackLookInOppositeDirection);

									if (currentCombatAttackInfo.disableGravityOnAttacks) {
										disablePlayerGravityDuringAttack (true);
									}
								}
							} else {
								if (showDebugPrint) {
									print ("combo counter higher than attack index " + currentCombatTypeInfo.comboCounter);
								}
							}
						} else {
							if (currentCombatTypeInfo.attackActive) {
								if (showDebugPrint) {
									print ("combo is over");
								}

								disableCombat ();
							}
						}

						return;
					}
				}
			}
		}
	}

	public void resetAttacksIndex (int combatTypeIndexToIgnore)
	{
		bool otherAttacksTypeActive = false;

		int combatTypeInfoListCount = combatTypeInfoList.Count;

		for (int i = 0; i < combatTypeInfoListCount; i++) {
			combatTypeInfo currentCombatTypeInfo = combatTypeInfoList [i];

			if (combatTypeID == currentCombatTypeInfo.combatTypeID) {
				if (i != combatTypeIndexToIgnore && currentCombatTypeInfo.attackActive) {

					currentCombatTypeInfo.attackActive = false;

					currentCombatTypeInfo.resetingCombo = false;

					currentCombatTypeInfo.currentAttackIndex = 0;

					currentCombatTypeInfo.comboCounter = 0;

					for (int j = 0; j < currentCombatTypeInfo.combatAttackInfoList.Count; j++) {
						currentCombatTypeInfo.combatAttackInfoList [j].playingNextAttack = false;
					}

					otherAttacksTypeActive = true;
				}
			}
		}

		if (otherAttacksTypeActive) {
			timerCombat = 0;
		}
	}

	public void setDamageDetectedOnTriggerById (int newId)
	{
		combatLimbInfo currentcombatLimbInfo = combatLimbList [newId];

		checkEventOnDamage (currentcombatLimbInfo.hitCombatManager.currentCombatTypeIndex, 
			currentcombatLimbInfo.hitCombatManager.currentAttackIndex, 
			currentcombatLimbInfo.hitCombatManager.getLastSurfaceDetected (), 
			newId);
	}

	public void checkEventOnDamage (int combatTypeIndex, int attackIndex, GameObject objectDetected, int combatLimbID)
	{
		if (combatTypeInfoList.Count > combatTypeIndex) { 

			combatTypeInfo currentCombatTypeInfo = combatTypeInfoList [combatTypeIndex];

			if (currentCombatTypeInfo.combatAttackInfoList.Count > attackIndex) {
				combatAttackInfo currentCombatAttackInfo = currentCombatTypeInfo.combatAttackInfoList [attackIndex];

				if (currentCombatAttackInfo != null) {
					if (currentCombatAttackInfo.useEventOnDamage) {
						currentCombatAttackInfo.eventOnDamage.Invoke ();
					}
					
					if (currentCombatAttackInfo.useRemoteEvent) {
						bool useRemoteEvents = false;

						if (objectDetected != null) {
							if (currentCombatAttackInfo.checkObjectsToUseRemoteEventsOnDamage) {
								if ((1 << objectDetected.layer & currentCombatAttackInfo.layerToUseRemoteEventsOnDamage.value) == 1 << objectDetected.layer) {
									useRemoteEvents = true;
								}
							} else {
								useRemoteEvents = true;
							}

							if (useRemoteEvents) {
								remoteEventSystem currentRemoteEventSystem = objectDetected.GetComponent<remoteEventSystem> ();

								if (currentRemoteEventSystem != null) {
									for (int j = 0; j < currentCombatAttackInfo.remoteEventNameList.Count; j++) {
										currentRemoteEventSystem.callRemoteEvent (currentCombatAttackInfo.remoteEventNameList [j]);
									}
								}
							}
						}
					}

					if (objectDetected != null) {
						checkSurfaceFoundOnAttack (true, objectDetected, combatLimbID);

						if (!ignoreGetHealthFromDamagingObjects) {
							if (currentCombatAttackInfo.getHealthFromDamagingObjects) {
								float totalHealthAmount = combatLimbList [combatLimbID].hitCombatManager.getLastDamageApplied ()
								                          * currentCombatAttackInfo.healthFromDamagingObjectsMultiplier;

								if (totalHealthAmount > 0 && !mainHealth.checkIfMaxHealthWithHealthManagement ()) {

									mainHealth.setHealWithHealthManagement (totalHealthAmount);
								}
							}
						}
					}
				}
			}
		}
	}

	public void resetAttackIdOnAnimator ()
	{
		stopResetAttackIdOnAnimatorCoroutine ();

		resetAttackIDCoroutine = StartCoroutine (resetAttackIdOnAnimatorCoroutine ());
	}

	void stopResetAttackIdOnAnimatorCoroutine ()
	{
		if (resetAttackIDCoroutine != null) {
			StopCoroutine (resetAttackIDCoroutine);
		}
	}

	IEnumerator resetAttackIdOnAnimatorCoroutine ()
	{	
		yield return new WaitForSeconds (0.1f);

		resetAttackIDAnimatorID ();
	}

	public void callEventWithDelay (combatAttackInfo currentCombatAttackInfoEvent, bool playAtStart)
	{
		if (currentCombatAttackInfoEvent.delayCoroutine != null) {
			StopCoroutine (currentCombatAttackInfoEvent.delayCoroutine);
		}
	
		currentCombatAttackInfoEvent.delayCoroutine = StartCoroutine (callEventWithDelayCoroutine (currentCombatAttackInfoEvent, playAtStart));
	}

	IEnumerator callEventWithDelayCoroutine (combatAttackInfo currentCombatAttackInfoEvent, bool playAtStart)
	{	
		if (playAtStart) {
			yield return new WaitForSeconds (currentCombatAttackInfoEvent.delayOnEventOnAttackStart);

			currentCombatAttackInfoEvent.eventOnAttackStart.Invoke ();
		} else {
			yield return new WaitForSeconds (currentCombatAttackInfoEvent.delayOnEventOnAttackEnd);

			currentCombatAttackInfoEvent.eventOnAttackStart.Invoke ();
		}
	}

	bool canUseCombat ()
	{
		if (currentPlayerMode) { 
			bool canUseCombatResult = true;

			if (!combatSystemEnabled) {
				canUseCombatResult = false;
			}

			if (playerControllerManager.isGravityPowerActive ()) {
				canUseCombatResult = false;
			}

			if (playerControllerManager.isPlayerUsingPowers ()) {
				canUseCombatResult = false;
			}

			if (playerControllerManager.isPlayerUsingWeapons () && !combatModeActivatedTemporally) {
				canUseCombatResult = false;
			}

			if (!canUseInput ()) {
				canUseCombatResult = false;
			}

			if (playerControllerManager.isPlayerNavMeshEnabled ()) {
				canUseCombatResult = false;
			}

			if (playerControllerManager.isExternalControllBehaviorActive ()) {
				if (checkExternalControllerBehaviorActiveList) {
					if (!externalControllerBehaviorNameListToUseCloseCombat.Contains (playerControllerManager.getCurrentExternalControllerBehaviorName ())) {
						canUseCombatResult = false;

						if (showDebugPrint) {
							print ("External Controller behavior active not included on the behavior " +
							"list " + playerControllerManager.getCurrentExternalControllerBehaviorName () + ", avoid to use close combat");
						}
					}
				} else {
					canUseCombatResult = false;

					if (showDebugPrint) {
						print ("External Controller behavior active, avoid to use close combat");
					}
				}
			}

			return canUseCombatResult;
		}

		return false;
	}

	bool canUseInput ()
	{
		if (playerControllerManager.isUsingDevice ()) {
			return false;
		}

		if (playerControllerManager.isPlayerDead ()) {
			return false;
		}

		if (playerControllerManager.isPlayerMenuActive ()) {
			return false;
		}

		return true;
	}

	public void activateAttackWithoutCombatActive (string attackName)
	{
		if (!canUseAttacksWithoutCombatActiveEnabled) {
			return;
		}

		if (currentPlayerMode && !combatModeActivatedTemporally) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (playerControllerManager.isActionActive ()) {
			return;
		}

		if (playerControllerManager.isIgnoreExternalActionsActiveState ()) {
			return;
		}

		combatModeActivatedTemporally = true;

		currentPlayerMode = true;

		enableOrDisableCloseCombatTriggers (true);

		playerControllerManager.setUsingCloseCombatActiveState (currentPlayerMode);

		inputAttack (attackName);


		if (!combatPlaying) {
			print ("attack wasn't able to be activated, cancelling temporal state");

			disableCombatModeActivatedTemporallyValues ();
		} else {
			carryingWeaponsPreviously = GKC_Utils.enableOrDisableIKOnWeaponsDuringAction (playerControllerManager.gameObject, false);

			if (hideFireWeaponsOnExternalCombat) {
				GKC_Utils.enableOrDisableFireWeaponMeshActiveState (playerControllerManager.gameObject, false);
			}
				
			if (hideMeleeWeaponsOnExternalCombat) {
				GKC_Utils.enableOrDisableMeleeWeaponMeshActiveState (playerControllerManager.gameObject, false);
			}
		}
	}

	public void setCurrentPlayerMode (bool state)
	{
		if (currentPlayerMode == state) {
			return;
		}

		currentPlayerMode = state;

		playerControllerManager.setUsingCloseCombatActiveState (currentPlayerMode);

		checkEventsOnStateChange (currentPlayerMode);

		if (currentPlayerMode) {
			enableOrDisableCloseCombatTriggers (true);

			if (setCombatIdleID) {
				playerControllerManager.setCurrentIdleIDValue (combatIdleID);
			}
		} else {
			if (timerCombat <= 0) {
				enableOrDisableCloseCombatTriggers (false);
			}

			if (setCombatIdleID) {
				playerControllerManager.setCurrentIdleIDValue (0);
			}
		}

		if (toggleStrafeModeIfRunningActive) {
			playerControllerManager.setDisableStrafeModeExternallyIfIncreaseWalkSpeedActiveState (currentPlayerMode);
		}

		if (currentPlayerMode) {
			previousStrafeMode = playerControllerManager.isStrafeModeActive ();

			if (previousStrafeID == -1) {
				previousStrafeID = playerControllerManager.getCurrentStrafeIDValue ();
			}
		
			if (useStrafeMode) {
				playerControllerManager.activateOrDeactivateStrafeMode (useStrafeMode);

				playerControllerManager.setCurrentStrafeIDValue (strafeIDUsed);
			}

		} else {
			if (useStrafeMode) {
				if (setPreviousStrafeModeOnDisableMode) {
					playerControllerManager.activateOrDeactivateStrafeMode (previousStrafeMode);
				} else {
					playerControllerManager.setOriginalLookAlwaysInCameraDirectionState ();
				}
		
				if (previousStrafeID != -1) {
					playerControllerManager.setCurrentStrafeIDValue (previousStrafeID);
				}
			}

			previousStrafeMode = false;

			previousStrafeID = -1;
		}

		if (!currentPlayerMode) {
			if (blockActive) {
				blockActivePreviously = false;

				disableBlockState ();
			}

			blockActive = false;

			if (attackInputPaused) {
				stopDisableMeleeAttackInputPausedStateWithDurationCoroutine ();

				attackInputPaused = false;
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

	//the combo has finished, so disable the combat mode
	void disableCombat ()
	{
		resetAttacksIndex (-1);

		timerCombat = 0;

		changeCollidersState (false, colliderPlace.both, -1, -1);
	
		combatPlaying = false;

		previousComboInProcessName = "";
		currentComboInProcessName = "";

		resetAttackIDAnimatorID ();

		stopResetAttackIdOnAnimatorCoroutine ();

		if (canMoveStatePaused) {
			setMoveInputPausedState (false);
		}

		if (rootMotionStateChanged) {
			setRootMotionState (false);
		}

		if (headTrackStateChanged) {
			setHeadTrackState (false);
		}

		if (headTrackLookInOppositeDirectionStateChanged) {
			setHeadTrackLookInOppositeDirection (false);
		}

		if (!currentPlayerMode) {
			enableOrDisableCloseCombatTriggers (false);
		}

		if (gravityChangedActive) {
			disablePlayerGravityDuringAttack (false);
		}

		blockActivePreviously = false;

		currentWaitTimeToNextAttack = 0;

		if (combatModeActivatedTemporally) {

			disableCombatModeActivatedTemporallyValues ();
		}

		playerControllerManager.setCloseCombatAttackInProcessState (false);
	}

	void disableCombatModeActivatedTemporallyValues ()
	{
		if (combatModeActivatedTemporally) {
			currentPlayerMode = false;

			enableOrDisableCloseCombatTriggers (false);

			playerControllerManager.setUsingCloseCombatActiveState (false);

			combatModeActivatedTemporally = false;

			if (carryingWeaponsPreviously) {
				GKC_Utils.enableOrDisableIKOnWeaponsDuringAction (playerControllerManager.gameObject, true);

				carryingWeaponsPreviously = false;
			}

			if (hideMeleeWeaponsOnExternalCombat) {
				GKC_Utils.enableOrDisableMeleeWeaponMeshActiveState (playerControllerManager.gameObject, true);
			}

			if (hideFireWeaponsOnExternalCombat) {
				GKC_Utils.enableOrDisableFireWeaponMeshActiveState (playerControllerManager.gameObject, true);
			}
		}
	}

	public bool isAttackInProcess ()
	{
		return combatPlaying;
	}

	void setMoveInputPausedState (bool state)
	{
		if (canMoveStatePaused != state) {
			playerControllerManager.setMoveInputPausedState (state);

			if (state) {
				playerControllerManager.resetPlayerControllerInput ();
			}
		}

		canMoveStatePaused = state;
	}

	void setRootMotionState (bool state)
	{
		if (rootMotionStateChanged != state) {
			playerControllerManager.setApplyRootMotionAlwaysActiveState (state);

			playerControllerManager.setActionActiveWithMovementState (state);
		}

		rootMotionStateChanged = state;
	}

	void setHeadTrackState (bool state)
	{
		if (headTrackStateChanged != state) {
			playerControllerManager.setHeadTrackCanBeUsedState (!state);
		}

		headTrackStateChanged = state;
	}

	void setHeadTrackLookInOppositeDirection (bool state)
	{
		if (headTrackLookInOppositeDirectionStateChanged != state) {
			if (mainHeadTrack != null) {
				mainHeadTrack.setLookInOppositeDirectionOutOfRangeValue (!state);
			}
		}

		headTrackLookInOppositeDirectionStateChanged = state;
	}

	void resetAttackIDAnimatorID ()
	{
		animator.SetInteger (attackIDAnimatorID, 0);
	}

	void disablePlayerGravityDuringAttack (bool state)
	{
		if (state) {
			if (!gravityChangedActive) {
				playerControllerManager.setGravityForcePuase (true);

				playerControllerManager.setCheckOnGroungPausedState (true);

				playerControllerManager.setPlayerVelocityToZero ();
			}
		} else {
			if (gravityChangedActive) {
				playerControllerManager.setGravityForcePuase (false);

				playerControllerManager.setCheckOnGroungPausedState (false);

				playerControllerManager.setLastTimeFalling ();
			}
		}

		gravityChangedActive = state;
	}

	//disable or enable the triggers in the hands and foot of the player, to damage the enemy when they touch it
	void changeCollidersState (bool state, colliderPlace place, int combatTypeIndex, int attackIndex)
	{
		//check what colliders have to be activated or deactivated, the hands or the foot, to damage the enemy with 
		//the correct triggers according to the type of combo
		stopCheckEventInfoList ();

		stopActivateDamageTriggerCoroutine ();

		for (int i = 0; i < combatLimbList.Count; i++) {
			hitCombat temporalHitCombat = combatLimbList [i].hitCombatManager;

			temporalHitCombat.setCurrentState (false);
		
			temporalHitCombat.setIgnoreDetectedObjectsOnListState (false);

			temporalHitCombat.setNewSphereColliderTriggerRadius (combatLimbList [i].originalTriggerRadius);
	
			temporalHitCombat.setCustomDamageCanBeBlockedState (currentAttackCanBeBlocked);

			if (setCanActivateReactionSystemTemporallyState) {
				temporalHitCombat.setCanActivateReactionSystemTemporallyState (canActivateReactionSystemTemporally);
			}
		}

		if (state) {

			combatAttackInfo currentCombatAttackInfo = combatTypeInfoList [combatTypeIndex].combatAttackInfoList [attackIndex];

			bool useDamageTriggerActiveInfo = currentCombatAttackInfo.useDamageTriggerActiveInfo;

			limbIndexListOnCurrentAttack.Clear ();

			for (int i = 0; i < combatLimbList.Count; i++) {
				combatLimbInfo currentCombatLimbInfo = combatLimbList [i];
				colliderPlace currentColliderPlace = currentCombatLimbInfo.limbType;

				if (state) {
					bool canUseLimb = false;

					if (place == currentColliderPlace) {
						canUseLimb = true;
					}

					if (place == colliderPlace.both) {
						canUseLimb = true;
					}
				
					if (place == colliderPlace.both_arms && (currentColliderPlace == colliderPlace.right_arm || currentColliderPlace == colliderPlace.left_arm)) {
						canUseLimb = true;
					}

					if (place == colliderPlace.both_legs && (currentColliderPlace == colliderPlace.right_leg || currentColliderPlace == colliderPlace.left_leg)) {
						canUseLimb = true;
					}

					if (place == colliderPlace.in_front && i == combatLimbList.Count - 1) {
						canUseLimb = true;
					}

					if (canUseLimb) {
						if (!useDamageTriggerActiveInfo) {
							currentCombatLimbInfo.hitCombatManager.setCurrentState (true);

							currentCombatLimbInfo.hitCombatManager.setMainColliderEnabledState (true);

							if (setCanActivateReactionSystemTemporallyState && canActivateReactionSystemTemporally && currentCombatAttackInfo.ignoreActivateReactionSystem) {
								currentCombatLimbInfo.hitCombatManager.setCanActivateReactionSystemTemporallyState (false);
							}
								
							currentCombatLimbInfo.hitCombatManager.setNewDamageReactionID (currentCombatAttackInfo.damageReactionID);
						}

						limbIndexListOnCurrentAttack.Add (i);

						currentCombatLimbInfo.hitCombatManager.setCurrentAttackInfoIndex (combatTypeIndex, attackIndex);
					}
				}
			}

			if (useDamageTriggerActiveInfo) {
				damageTriggerCoroutine = StartCoroutine (activateDamageTriggerCoroutine (combatTypeIndex, attackIndex));
			}
		}
	}

	public void stopActivateDamageTriggerCoroutine ()
	{
		if (damageTriggerCoroutine != null) {
			StopCoroutine (damageTriggerCoroutine);
		}
	}

	IEnumerator activateDamageTriggerCoroutine (int combatTypeIndex, int attackIndex)
	{
		if (blockActive) {
			blockActivePreviously = true;

			disableBlockState ();
		}


		if (canCancelBlockToStartAttackActive) {
			if (showDebugPrint) {
				print ("cancel block");
			}

			animator.SetBool (cancelBlockReactionStateName, true);

			yield return new WaitForSeconds (0.3f);

			animator.SetBool (cancelBlockReactionStateName, false);

			//			checkDisableHasExitTimeAnimator ();

			canCancelBlockToStartAttackActive = false;
		}


		combatAttackInfo currentCombatAttackInfo = combatTypeInfoList [combatTypeIndex].combatAttackInfoList [attackIndex];

		currentAttackCanBeBlocked = true;

		if (currentCombatAttackInfo.attackCantBeBlocked) {
			currentAttackCanBeBlocked = false;
		}

		if (!currentAttackCanBeBlocked) {
			if (useEventsOnAttackCantBeBlocked) {
				eventOnAttackCantBeBlocked.Invoke ();
			}
		}


//		print ("start attack " + currentCombatAttackInfo.Name);

		int numberOfEvents = currentCombatAttackInfo.damageTriggerActiveInfoList.Count;

		int numberOfEventsTriggered = 0;

		float timer = Time.time;

		bool allEventsTriggered = false;

		List<damageTriggerActiveInfo> damageTriggerActiveInfoList = currentCombatAttackInfo.damageTriggerActiveInfoList;

		for (int i = 0; i < damageTriggerActiveInfoList.Count; i++) {

			currentTriggerInfo = damageTriggerActiveInfoList [i];

			currentTriggerInfo.delayTriggered = false;

			if (useAnimationPercentageDuration) {
				float currentDelay = currentTriggerInfo.delayToActiveTrigger;

				if (currentTriggerInfo.delayToActiveTrigger > 1) {
					if (showDebugPrint) {
						print ("ERRRORORORROOROROR: DELAY IS HIGHER THAN 1 FIXXXXXXXXXXX----------------------------------------" +
						".............................................");
					}
				}

				if (useAnimationPercentageOver100) {
					currentDelay /= 100;
				}

				currentTriggerInfo.calculatedPercentageAttackDuration = 
					(currentCombatAttackInfo.attackDuration / currentCombatAttackInfo.animationSpeed) * currentDelay;
			}
		}

		bool canActivateCurrentEvent = false;

		while (!allEventsTriggered) {

			for (int i = 0; i < damageTriggerActiveInfoList.Count; i++) {
				currentTriggerInfo = damageTriggerActiveInfoList [i];

				if (!currentTriggerInfo.delayTriggered) {
					//						print (currentTriggerInfo.delayToActiveTrigger + " " + i);

					canActivateCurrentEvent = false;

					if (useAnimationPercentageDuration) {
						if (Time.time > timer + currentTriggerInfo.calculatedPercentageAttackDuration) {
							canActivateCurrentEvent = true;
						}
					} else {
						if (Time.time > timer + currentTriggerInfo.delayToActiveTrigger) {
							canActivateCurrentEvent = true;
						}
					}

//						print (currentTriggerInfo.delayToActiveTrigger + " " + i);

					if (canActivateCurrentEvent) {

						bool activateDamageTrigger = currentTriggerInfo.activateDamageTrigger;

						for (int j = 0; j < limbIndexListOnCurrentAttack.Count; j++) {

							hitCombat temporalhitCombat = combatLimbList [limbIndexListOnCurrentAttack [j]].hitCombatManager;

							temporalhitCombat.setCurrentState (activateDamageTrigger);

							if (activateDamageTrigger) {
								temporalhitCombat.setMainColliderEnabledState (false);
								temporalhitCombat.setMainColliderEnabledState (true);

								if (currentCombatAttackInfo.ignoreForcesToApplyOnAttack) {
									temporalhitCombat.setIgnoreForcesToApplyOnAttackActiveState (true);
								}

								if (setCanActivateReactionSystemTemporallyState && canActivateReactionSystemTemporally && currentCombatAttackInfo.ignoreActivateReactionSystem) {
									temporalhitCombat.setCanActivateReactionSystemTemporallyState (false);
								}

								temporalhitCombat.setNewDamageReactionID (currentCombatAttackInfo.damageReactionID);

								if (currentCombatAttackInfo.ignoreStoreDetectedObjectOnList) {
									temporalhitCombat.setIgnoreDetectedObjectsOnListState (true);
								}
							}

							if (activateDamageTrigger) {
								temporalhitCombat.setUseCastAllDamageDetectionState (currentTriggerInfo.useCastAllDamageDetection);
							}

							if (activateDamageTrigger) {
								checkSurfaceFoundOnAttack (false, null, -1);
							}
						
							if (currentTriggerInfo.setNewTriggerRadius) {
//								if (currentTriggerInfo.setOriginalRadius) {
//									temporalhitCombat.setNewSphereColliderTriggerRadius (currentTriggerInfo.originalRadius);
//								} else {
								temporalhitCombat.setNewSphereColliderTriggerRadius (currentTriggerInfo.newTriggerRadius);
//								}
							}
						}

						numberOfEventsTriggered++;

						currentTriggerInfo.delayTriggered = true;

						if (numberOfEvents == numberOfEventsTriggered) {
							allEventsTriggered = true;

//							print ("end of attack " + currentCombatAttackInfo.Name);
						}
					}
				}
			}

			yield return null;
		}

		if (blockActivePreviously) {
			if (!blockInputPaused) {
				blockActivePreviously = false;

				yield return new WaitForSeconds (0.3f);

				setBlockActiveState (true);
			}
		}
	}

	void setCollidersExtraDamageValue (float extraDamageValue)
	{
		for (int i = 0; i < combatLimbList.Count; i++) {
			combatLimbList [i].hitCombatManager.setCurrentExtraDamageValue (extraDamageValue);
		}
	}

	public void enableOrDisableTriggers (bool value)
	{
		for (int i = 0; i < combatLimbList.Count; i++) {
			if (combatLimbList [i].trigger) {
				combatLimbList [i].trigger.enabled = value;
			}
		}
	}

	public void stopCheckEventInfoList ()
	{
		if (eventInfoListCoroutine != null) {
			StopCoroutine (eventInfoListCoroutine);
		}
	}

	public void checkEventInfoList (combatAttackInfo currentCombatAttackInfo)
	{
		if (currentCombatAttackInfo != null && currentCombatAttackInfo.useEventInfoList) {

			stopCheckEventInfoList ();

			eventInfoListCoroutine = StartCoroutine (checkEventInfoListCoroutine (currentCombatAttackInfo));
		}
	}

	IEnumerator checkEventInfoListCoroutine (combatAttackInfo currentCombatAttackInfo)
	{
		List<eventInfo> eventInfoList = currentCombatAttackInfo.eventInfoList;

		for (int i = 0; i < eventInfoList.Count; i++) {
			eventInfoList [i].eventTriggered = false;
		}

		eventInfo currentEventInfo;

		if (currentCombatAttackInfo.useAccumulativeDelay) {
			for (int i = 0; i < eventInfoList.Count; i++) {

				currentEventInfo = eventInfoList [i];

				yield return new WaitForSeconds (currentEventInfo.delayToActivate);

				currentEventInfo.eventToUse.Invoke ();

				if (currentEventInfo.sendCurrentPlayerOnEvent) {
					currentEventInfo.eventToSendCurrentPlayer.Invoke (gameObject);
				}

				if (currentCombatAttackInfo == null) {
					i = eventInfoList.Count - 1;
				}
			}
		} else {
			int numberOfEvents = eventInfoList.Count;

			int numberOfEventsTriggered = 0;

			float timer = Time.time;

			bool allEventsTriggered = false;

			while (!allEventsTriggered) {
				if (currentCombatAttackInfo == null) {
					allEventsTriggered = true;
				} else {
					for (int i = 0; i < eventInfoList.Count; i++) {
						currentEventInfo = eventInfoList [i];

						if (!currentEventInfo.eventTriggered) {
							if (Time.time > timer + currentEventInfo.delayToActivate) {
								currentEventInfo.eventToUse.Invoke ();

								if (currentEventInfo.sendCurrentPlayerOnEvent) {
									currentEventInfo.eventToSendCurrentPlayer.Invoke (gameObject);
								}

								currentEventInfo.eventTriggered = true;

								numberOfEventsTriggered++;

								if (numberOfEvents == numberOfEventsTriggered) {
									allEventsTriggered = true;
								}
							}
						}
					}
				}

				yield return null;
			}
		}
	}

	public void stopAllAttacks ()
	{
		if (timerCombat <= 0) {
			return;
		}

		stopActivateDamageTriggerCoroutine ();

		stopCheckEventInfoList ();

		stopResetAttackIdOnAnimatorCoroutine ();

		disableCombat ();
	}

	public void checkLookAtTargetActiveState ()
	{
		if (currentPlayerMode) {
			bool lookingAtTarget = playerControllerManager.isPlayerLookingAtTarget ();

			if (lookingAtTarget) {
				if (activateStrafeModeOnLockOnTargetActive) {
					playerControllerManager.activateOrDeactivateStrafeMode (true);
				}
			} else {
				if (activateStrafeModeOnLockOnTargetActive) {
					playerControllerManager.activateOrDeactivateStrafeMode (false);
				}
			}
		}
	}

	public void checkLookAtTargetDeactivateState ()
	{
		if (currentPlayerMode) {
			if (activateStrafeModeOnLockOnTargetActive) {
				playerControllerManager.activateOrDeactivateStrafeMode (false);
			}
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputAttack (string attackName)
	{
		if (attackInputPaused) {
			return;
		}

		if (useStaminaOnAttackEnabled) {
			if (attackInputPausedForStamina && generalStaminaUseMultiplier > 0) {
				if (showDebugPrint) {
					print ("Not enough stamina");
				}

				return;
			}
		}

		useAttack (attackName);
	}

	public void inputSetBlockState (bool state)
	{
		if (!canUseCombat ()) {
			return;
		}

		setBlockStateWithoutInputCheck (state);
	}

	public void setBlockStateWithoutInputCheck (bool state)
	{
		if (!blockEnabled) {
			return;
		}

		if (blockInputPaused) {
			return;
		}

		if (!currentPlayerMode) {
			blockActivePreviously = false;

			return;
		}

		if (state) {
			if (combatPlaying) {
				blockActivePreviously = true;

				return;
			}
		} else {
			if (combatPlaying) {
				blockActivePreviously = false;

				return;
			}
		}

		blockActive = state;

		if (blockActive) {

		} else {

		}

		setBlockActiveState (blockActive);
	}
	//END OF INPUT FUNCTIONS


	//START OF BLOCK FUNCTIONS
	public void setBlockActiveState (bool state)
	{
		if (state) {
			if (blockEnabled) {
				playerControllerManager.activateCustomAction (blockActionName);

				mainHealth.setBlockDamageActiveState (true);

				if (reducedBlockDamageProtectionActive) {
					mainHealth.setBlockDamageProtectionAmount (reducedBlockDamageProtectionAmount * generalBlockProtectionMultiplier);
				} else {
					mainHealth.setBlockDamageProtectionAmount (blockDamageProtectionAmount * generalBlockProtectionMultiplier);
				}

				mainHealth.setBlockDamageRangleAngleState (useMaxBlockRangeAngle, maxBlockRangeAngle);

				mainHealth.setHitReactionBlockIDValue (blockID);

				blockActive = true;

				checkEventsOnBlockDamage (true);
			}
		} else {
			if (blockEnabled) {
				disableBlockState ();
			}
		}
	}

	void disableBlockState ()
	{
		playerControllerManager.stopCustomAction (blockActionName);

		mainHealth.setBlockDamageActiveState (false);

		blockActive = false;

		checkEventsOnBlockDamage (false);

		mainHealth.setHitReactionBlockIDValue (-1);
	}

	public void checkEventsOnBlockDamage (bool state)
	{
		if (useEventsOnBlockDamage) {
			if (state) {
				eventOnBlockActivate.Invoke ();
			} else {
				eventOnBlockDeactivate.Invoke ();	
			}
		}
	}

	//CALLED BY THE STAMINA SYSTEM
	public void updateRegularBlockDamageProtectionValue ()
	{
		setBlockDamageProtectionValue (false);
	}

	public void updateReducedBlockDamageProtectionValue ()
	{
		setBlockDamageProtectionValue (true);
	}

	public void setBlockDamageProtectionValue (bool state)
	{
		reducedBlockDamageProtectionActive = state;

		if (blockActive) {
			if (!currentPlayerMode) {
				return;
			}

			if (blockEnabled) {
				if (reducedBlockDamageProtectionActive) {
					mainHealth.setBlockDamageProtectionAmount (reducedBlockDamageProtectionAmount * generalBlockProtectionMultiplier);
				} else {
					mainHealth.setBlockDamageProtectionAmount (blockDamageProtectionAmount * generalBlockProtectionMultiplier);
				}
			}
		}
	}

	public void setMeleeAttackInputPausedState (bool state)
	{
		attackInputPaused = state;
	}

	//Used to pause/resume the attack and block input
	public void disableMeleeAttackInputPausedStateWithDuration (float pauseDuration)
	{
		stopDisableMeleeAttackInputPausedStateWithDurationCoroutine ();

		pauseMeleeAttackInputCoroutine = StartCoroutine (disableMeleeAttackInputPausedStateWithDurationCoroutine (pauseDuration));
	}

	void stopDisableMeleeAttackInputPausedStateWithDurationCoroutine ()
	{
		if (pauseMeleeAttackInputCoroutine != null) {
			StopCoroutine (pauseMeleeAttackInputCoroutine);
		}
	}

	IEnumerator disableMeleeAttackInputPausedStateWithDurationCoroutine (float pauseDuration)
	{
		attackInputPaused = true;

		yield return new WaitForSeconds (pauseDuration);

		attackInputPaused = false;
	}

	public void checkIfBlockActive ()
	{
		if (blockActive) {
			setBlockActiveState (true);
		}
	}

	public void disableBlockInputPausedStateWithDuration (float pauseDuration)
	{
		stopDisableBlockInputPausedStateWithDurationCoroutine ();

		pauseBlockInputCoroutine = StartCoroutine (disableBlockInputPausedStateWithDurationCoroutine (pauseDuration));
	}

	void stopDisableBlockInputPausedStateWithDurationCoroutine ()
	{
		if (pauseBlockInputCoroutine != null) {
			StopCoroutine (pauseBlockInputCoroutine);
		}
	}

	IEnumerator disableBlockInputPausedStateWithDurationCoroutine (float pauseDuration)
	{
		blockInputPaused = true;

		yield return new WaitForSeconds (pauseDuration);

		blockInputPaused = false;

		if (blockActivePreviously) {
			blockActivePreviously = false;

			yield return new WaitForSeconds (0.3f);

			setBlockActiveState (true);
		}
	}

	public void setCanCancelBlockToStartAttackActiveState (bool state)
	{
		canCancelBlockToStartAttackActive = state;
	}

	//CALLED ON DODGE/ROLL ACTION SYSTEM
	public void checkIfBlockInputIsCurrentlyInUse ()
	{
		if (playerInput.isKeyboardButtonPressed (mainMeleeCombatBlockInputName, mainMeleeCombatBlockInputName)) {
			if (showDebugPrint) {
				print ("block is being pressed");
			}
		} else {
			if (showDebugPrint) {
				print ("block is not being pressed, disabling block.");
			}

			disableBlockStateInProcess ();
		}
	}

	public void disableBlockStateInProcess ()
	{
		if (blockActive) {
			blockActivePreviously = false;

			setBlockActiveState (false);
		}
	}

	public void checkOnAttackBlocked ()
	{
		if (blockActive) {
			if (getHealthFromBlocks) {
				float totalHealthAmount = healthAmountFromBlocks;

				if (totalHealthAmount > 0 && !mainHealth.checkIfMaxHealthWithHealthManagement ()) {

					mainHealth.setHealWithHealthManagement (totalHealthAmount);
				}
			}
		}
	}

	public void checkOnAttackBlockedPerfectly ()
	{
		if (blockActive) {
			if (getHealthFromPerfectBlocks) {
				float totalHealthAmount = healthAmountFromPerfectBlocks;

				if (totalHealthAmount > 0 && !mainHealth.checkIfMaxHealthWithHealthManagement ()) {

					mainHealth.setHealWithHealthManagement (totalHealthAmount);
				}
			}
		}
	}
	//END OF BLOCK FUNCTIONS

	//CHECK SURFACE ON ATTACK FUNCTIONS
	public void setNoDamageDetectedOnTriggerById (GameObject objectDetected)
	{
		checkSurfaceFoundOnAttack (true, objectDetected, -1);
	}

	public void checkSurfaceFoundOnAttack (bool surfaceLocated, GameObject lastSurfaceDetected, int combatLimbID)
	{
		if (!checkSurfaceInfoEnabled) {
			return;
		}

		string surfaceName = surfaceInfoOnMeleeAttackNameForSwingOnAir;

		Vector3 attackPosition = Vector3.zero;
		Vector3 attackNormal = Vector3.zero;

		RaycastHit hit = new RaycastHit ();

		if (surfaceLocated) {
			GameObject surfaceFound = null;

			if (lastSurfaceDetected != null && combatLimbID > -1) {
				Collider lastSurfaceDetectedCollider = lastSurfaceDetected.GetComponent<Collider> ();

				Vector3 limbPosition = combatLimbList [combatLimbID].limb.transform.position;

				Vector3 raycastPositionTarget = lastSurfaceDetectedCollider.ClosestPointOnBounds (limbPosition);

				if (!surfaceFound) {			
					Vector3 raycastPosition = limbPosition;

					Vector3 raycastDirection = raycastPositionTarget - raycastPosition;

					raycastDirection = raycastDirection / raycastDirection.magnitude;

					float currentRaycastDistance = GKC_Utils.distance (raycastPosition, raycastPositionTarget);

					currentRaycastDistance += 0.5f;

					Debug.DrawLine (raycastPosition, raycastPositionTarget, Color.black, 6);

					Debug.DrawLine (raycastPosition, raycastPosition + raycastDirection, Color.red, 4);

					Debug.DrawLine (raycastPosition + raycastDirection, 
						raycastPosition + (raycastDirection * 0.5f), Color.white, 4);

					Debug.DrawLine (raycastPositionTarget, raycastPositionTarget - (raycastDirection * 0.5f), Color.yellow, 4);

					if (Physics.Raycast (raycastPosition, raycastDirection, out hit, currentRaycastDistance, layerMaskToDamage)) {
						if (showDebugPrint) {
							print ("detected " + lastSurfaceDetected.name + " and raycast " + hit.collider.gameObject.name);
						}

						if (hit.collider.gameObject != playerControllerManager.gameObject) {
							surfaceFound = hit.collider.gameObject;
						}
					} else {
						if (showDebugPrint) {
							print ("detected " + lastSurfaceDetected.name + " no raycast found");
						}
					}
				}
			}

			if (surfaceFound != null) {
				if (showDebugPrint) {
					print ("SURFACE FOUND " + surfaceFound.name);
				}

				attackPosition = hit.point;
				attackNormal = hit.normal;
			} else {
				if (showDebugPrint) {
					print ("SURFACE NOT FOUND BY RAYCAST!!!!!!!!!!");
				}
			}

			if (lastSurfaceDetected != null) {
				meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = lastSurfaceDetected.GetComponent<meleeAttackSurfaceInfo> ();

				if (currentMeleeAttackSurfaceInfo != null) {
					if (!currentMeleeAttackSurfaceInfo.isSurfaceEnabled ()) {
						surfaceLocated = false;
					} else {

						surfaceName = currentMeleeAttackSurfaceInfo.getSurfaceName ();
					}
				} else {
					GameObject currentCharacter = applyDamage.getCharacterOrVehicle (lastSurfaceDetected);

					if (currentCharacter != null) {
						currentMeleeAttackSurfaceInfo = currentCharacter.GetComponent<meleeAttackSurfaceInfo> ();

						if (currentMeleeAttackSurfaceInfo != null) {
							if (!currentMeleeAttackSurfaceInfo.isSurfaceEnabled ()) {
								surfaceLocated = false;
							} else {
								surfaceName = currentMeleeAttackSurfaceInfo.getSurfaceName ();
							}
						}
					} else {
						surfaceLocated = false;
					}

					if (!surfaceLocated) {
						return;
					}
				}
			} else {
				if (showDebugPrint) {
					print ("SURFACE NOT FOUND BY TRIGGER!!!!!!!!!!");
				}
			}
		}

		bool ignoreBounceEvent = false;
		if (!currentAttackCanBeBlocked) {
			ignoreBounceEvent = true;
		}

		checkSurfaceFoundOnAttackToProcess (surfaceName, surfaceLocated, attackPosition, attackNormal, ignoreBounceEvent);
	}

	float lastTimeSurfaceAudioPlayed;
	int lastSurfaceDetecetedIndex = -1;

	public void checkSurfaceFoundOnAttackToProcess (string surfaceName, bool surfaceLocated, Vector3 attackPosition, Vector3 attackNormal, bool ignoreBounceEvent)
	{
		for (int i = 0; i < surfaceInfoOnMeleeAttackList.Count; i++) {
			grabbedObjectMeleeAttackSystem.surfaceInfoOnMeleeAttack currentSurfaceInfo = surfaceInfoOnMeleeAttackList [i];

			if (surfaceName.Equals (currentSurfaceInfo.surfaceName)) {

				int soundIndex = 0;

				if (currentSurfaceInfo.useSoundsListOnOrder) {
					currentSurfaceInfo.currentSoundIndex++;

					if (currentSurfaceInfo.currentSoundIndex >= currentSurfaceInfo.soundsList.Count) {
						currentSurfaceInfo.currentSoundIndex = 0;
					}

					soundIndex = currentSurfaceInfo.currentSoundIndex;
				} else {
					soundIndex = Random.Range (0, currentSurfaceInfo.soundsList.Count);
				}

				bool soundCanBePlayed = false;

				if (Time.time > lastTimeSurfaceAudioPlayed + 0.5f) {
					soundCanBePlayed = true;
				}

				if (lastSurfaceDetecetedIndex == -1 || lastSurfaceDetecetedIndex != i) {
					soundCanBePlayed = true;
				}

				if (soundCanBePlayed) {
//					print (currentSurfaceInfo.surfaceName);

					mainAudioSource.PlayOneShot (currentSurfaceInfo.soundsList [soundIndex]);

					lastTimeSurfaceAudioPlayed = Time.time;

					lastSurfaceDetecetedIndex = i;
				}

				if (surfaceLocated) {
					if (!ignoreBounceEvent) {
						if (currentSurfaceInfo.surfaceActivatesBounceOnCharacter) {
							currentSurfaceInfo.eventOnBounceCharacter.Invoke ();

							if (currentSurfaceInfo.stopAttackOnBounce) {

								stopAllAttacks ();
							}
						}
					}

					if (currentSurfaceInfo.useParticlesOnSurface && attackPosition != Vector3.zero) {
						GameObject newParticles = (GameObject)Instantiate (currentSurfaceInfo.particlesOnSurface, Vector3.zero, Quaternion.identity);
						newParticles.transform.position = attackPosition;
						newParticles.transform.LookAt (attackPosition + attackNormal * 3);
					}
				}

				//				print ("surface type detected " + surfaceName);

				return;
			}
		}
	}

	public void setCombatTypeID (int newValue)
	{
		combatTypeID = newValue;
	}

	public int getCombatTypeID ()
	{
		return combatTypeID;
	}

	Transform newDamageTriggerParent;

	public void setNewParentForDamageTriggers (Transform newParent)
	{
		newDamageTriggerParent = newParent;
			
		if (newDamageTriggerParent == null) {
			return;
		}

		for (int i = 0; i < combatLimbList.Count; i++) {
			hitCombat hitCombatToCheck = combatLimbList [i].hitCombatManager;

			hitCombatToCheck.transform.SetParent (newDamageTriggerParent);

			hitCombatToCheck.transform.localPosition = Vector3.zero;

			Rigidbody newRigidbody = hitCombatToCheck.gameObject.GetComponent<Rigidbody> ();

			if (newRigidbody == null) {
				newRigidbody = hitCombatToCheck.gameObject.AddComponent<Rigidbody> ();
			}

			newRigidbody.isKinematic = true;
		}
	}

	public void setOriginalParentForDamageTriggers ()
	{
		if (newDamageTriggerParent == null) {
			return;
		}

		for (int i = 0; i < combatLimbList.Count; i++) {
			hitCombat hitCombatToCheck = combatLimbList [i].hitCombatManager;

			if (combatLimbList [i].originalParent != null) {
				hitCombatToCheck.transform.SetParent (combatLimbList [i].originalParent);

				hitCombatToCheck.transform.localPosition = combatLimbList [i].originalLocalPosition;

				hitCombatToCheck.setNewSphereColliderTriggerRadius (combatLimbList [i].originalTriggerRadius);

				Rigidbody newRigidbody = hitCombatToCheck.gameObject.GetComponent<Rigidbody> ();

				if (newRigidbody != null) {
					Destroy (newRigidbody);
				}
			}
		}

		newDamageTriggerParent = null;
	}


	//EDITOR FUNCTIONS
	public void getCombatPrefabs (GameObject combatPrefab)
	{
		hitCombatPrefab = combatPrefab;

		updateComponent ();
	}

	public void addCombatPlace (string name, float hitDamage, GameObject limb, colliderPlace limbType)
	{
		combatLimbInfo newLimb = new combatLimbInfo ();
		newLimb.name = name;
		newLimb.hitDamage = hitDamage;
		newLimb.limb = limb;
		newLimb.limbType = limbType;
		newLimb.hitCombatManager = limb.GetComponent<hitCombat> ();
		newLimb.trigger = limb.GetComponent<Collider> ();
		combatLimbList.Add (newLimb);

		updateComponent ();
	}

	public void assignBasicCombatTriggers ()
	{
		if (hitCombatPrefab) {
			animator = transform.GetChild (0).GetComponentInChildren<Animator> ();

			if (animator != null) {

				combatLimbList.Clear ();

				//another list of bones, to the triggers in hands and feet for the combat
				Transform[] hitCombatPositions = new Transform[] {
					animator.GetBoneTransform (HumanBodyBones.LeftFoot),
					animator.GetBoneTransform (HumanBodyBones.RightFoot),
					animator.GetBoneTransform (HumanBodyBones.LeftHand),
					animator.GetBoneTransform (HumanBodyBones.RightHand)
				};

				for (int i = 0; i < hitCombatPositions.Length; i++) {
					GameObject hitCombatClone = (GameObject)Instantiate (hitCombatPrefab, Vector3.zero, Quaternion.identity);

					hitCombatClone.transform.SetParent (hitCombatPositions [i]);
					hitCombatClone.transform.localPosition = Vector3.zero;
					hitCombatClone.transform.localRotation = Quaternion.identity;
					string name = hitCombatPositions [i].gameObject.name;

					hitCombat currentHitCombat = hitCombatClone.GetComponent<hitCombat> ();

					currentHitCombat.setMainColliderEnabledState (false);

					string hitCombatTriggerName = "";

					if (hitCombatPositions [i] == animator.GetBoneTransform (HumanBodyBones.LeftFoot) || hitCombatPositions [i] == animator.GetBoneTransform (HumanBodyBones.RightFoot)) {
						colliderPlace newColliderPlace = colliderPlace.right_leg;

						if (hitCombatPositions [i] == animator.GetBoneTransform (HumanBodyBones.LeftFoot)) {
							newColliderPlace = colliderPlace.left_leg;

							hitCombatTriggerName = "Hit Combat Left Foot";
						} else {
							hitCombatTriggerName = "Hit Combat Right Foot";
						}

						addCombatPlace (name, defaultLegHitDamage, hitCombatClone, newColliderPlace);

						currentHitCombat.hitDamage = defaultLegHitDamage;
					} else {
						colliderPlace newColliderPlace = colliderPlace.right_arm;

						if (hitCombatPositions [i] == animator.GetBoneTransform (HumanBodyBones.LeftHand)) {
							newColliderPlace = colliderPlace.left_arm;

							hitCombatTriggerName = "Hit Combat Left Hand";
						} else {
							hitCombatTriggerName = "Hit Combat Right Hand";
						}

						addCombatPlace (name, defaultArmHitDamage, hitCombatClone, newColliderPlace);

						currentHitCombat.hitDamage = defaultArmHitDamage;
					}

					hitCombatClone.name = hitCombatTriggerName;
				}
			} else {
				print ("Animator not found in character, make sure it has one assigned");
			}

			animator = GetComponentInChildren<Animator> ();

			udpateHitCombatInfo ();

			updateComponent ();
		} else {
			print ("Assign the hit combat prefab to create the combat limbs");
		}
	}

	public void udpateHitCombatInfo ()
	{
		for (int i = 0; i < combatLimbList.Count; i++) {
			hitCombat hitCombatToCheck = combatLimbList [i].hitCombatManager;

			hitCombatToCheck.getOwner (gameObject);

			hitCombatToCheck.layerMask = layerMaskToDamage;

			hitCombatToCheck.hitDamage = combatLimbList [i].hitDamage;

			hitCombatToCheck.setAddForceMultiplierValue (addForceMultiplier);

			hitCombatToCheck.setTriggerIdOnEditor (i);

			hitCombatToCheck.setNewSphereColliderTriggerRadius (combatLimbList [i].originalTriggerRadius);

			combatLimbList [i].originalParent = hitCombatToCheck.transform.parent;

			combatLimbList [i].originalLocalPosition = hitCombatToCheck.transform.localPosition;

			if (useCustomIgnoreTags) {
				hitCombatToCheck.setCustomTagsToIgnore (customTagsToIgnoreList);
			} else {
				hitCombatToCheck.setCustomTagsToIgnore (null);
			}

			hitCombatToCheck.updateComponent ();
		}

		updateComponent ();

		print ("Info in hit triggers for combat updated");
	}

	public void removeLimbParts ()
	{
		for (int i = 0; i < combatLimbList.Count; i++) {
			if (combatLimbList [i].limb != null) {
				DestroyImmediate (combatLimbList [i].limb);
			}
		}

		combatLimbList.Clear ();

		Component[] components = GetComponentsInChildren (typeof(hitCombat));
		foreach (Component c in components) {
			DestroyImmediate (c.gameObject);
		}

		updateComponent ();
	}

	public void enableOrDisableCloseCombatTriggers (bool state)
	{
		for (int i = 0; i < combatLimbList.Count; i++) {
			combatLimbList [i].hitCombatManager.setMainColliderEnabledState (state);
		}
	}

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
			for (int i = 0; i < combatLimbList.Count; i++) {
				if (combatLimbList [i].limb != null) {
					Gizmos.color = gizmoColor;
					Gizmos.DrawWireSphere (combatLimbList [i].limb.transform.position, gizmoRadius);
				}
			}
		}
	}

	public void setCustomIgnoreTagsForCharacterFromEditor ()
	{
		if (playerControllerManager != null) {
			useCustomIgnoreTags = true;

			customTagsToIgnoreList.Clear ();

			customTagsToIgnoreList.Add (playerControllerManager.gameObject.tag);

			updateComponent ();
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update close combat info", gameObject);
	}

	[System.Serializable]
	public class combatLimbInfo
	{
		public string name;
		public float hitDamage;
		public GameObject limb;
		public colliderPlace limbType;
		public hitCombat hitCombatManager;
		public Collider trigger;
		public float originalTriggerRadius = 0.15f;
		public Transform originalParent;
		public Vector3 originalLocalPosition;
	}

	[System.Serializable]
	public class combatTypeInfo
	{
		public string Name;

		public string combatTypeDescription;

		public int combatTypeID = 0;
	
		public bool attacksUsedOnAir;

		public List<combatAttackInfo> combatAttackInfoList = new List<combatAttackInfo> ();

		public bool attackActive;

		public bool useDelayToStartAttackAfterFullCombo;
		public float delayToStartAttackAfterFullCombo;

		public bool useDelayToStartSameAttack;

		public bool useMinTimeToChangeCombo;

		public bool canChangeOfComboWhenPreviousNotComplete = true;

		public bool increaseAttackIndexOnlyOnAttackPerformedCorrectly;

		public float lastTimeFullCombo;

		public bool waitingToStartAttackActive;

		public int currentAttackIndex;

		public int comboCounter;

		public bool resetingCombo;
	}

	[System.Serializable]
	public class combatAttackInfo
	{
		public string Name;

		public bool useAttackID;
		public int attackID;

		public colliderPlace limbForAttack;

		public float attackDuration;

		public float animationSpeed = 1;

		public float minTimeToPlayNextAttack;
		public bool resetComboIfIncorrectMinTime;

		public bool playingNextAttack;

		public bool useExtraDamageMultiplier;
		public float extraDamageMultiplier;

		public bool canMoveWhileAttackActive;
		public bool useRootMotionOnAttack;
		public bool pauseHeadTrackOnAttack;

		public bool pauseHeadTrackLookInOppositeDirection;

		public bool disableGravityOnAttacks;

		public bool useEventOnAttackStart;
		public UnityEvent eventOnAttackStart;


		public bool useEventOnAttackEnd;
		public UnityEvent eventOnAttackEnd;

		public bool useDelayOnEventOnAttackStart;
		public float delayOnEventOnAttackStart;

		public bool useDelayOnEventOnAttackEnd;
		public float delayOnEventOnAttackEnd;


		public bool useEventOnDamage;
		public UnityEvent eventOnDamage;

		public bool checkObjectsToUseRemoteEventsOnDamage;
		public LayerMask layerToUseRemoteEventsOnDamage;

		public bool useRemoteEvent;
		public List<string> remoteEventNameList;

		public Coroutine delayCoroutine;

		public bool useDamageTriggerActiveInfo;

		public List<damageTriggerActiveInfo> damageTriggerActiveInfoList = new List<damageTriggerActiveInfo> ();
	
		public bool useEventInfoList;

		public bool useAccumulativeDelay;
		public Coroutine eventInfoListCoroutine;
		public List<eventInfo> eventInfoList = new List<eventInfo> ();

		public bool ignoreActivateReactionSystem;

		public int damageReactionID = -1;

		public bool useMatchPositionSystem = true;
		public float matchPositionOffset = 1.6f;

		public bool attackCantBeBlocked;

		public bool ignoreStoreDetectedObjectOnList;

		public bool ignoreForcesToApplyOnAttack;

		public bool useStaminaOnAttack;
		public float staminaUsedOnAttack;
		public float customRefillStaminaDelayAfterUse;

		public bool getHealthFromDamagingObjects;
		public float healthFromDamagingObjectsMultiplier = 1;
	}

	[System.Serializable]
	public class damageTriggerActiveInfo
	{
		public float delayToActiveTrigger;
		public bool activateDamageTrigger = true;
		public bool delayTriggered;

		public float calculatedPercentageAttackDuration;

		public bool setNewTriggerRadius;
		public float newTriggerRadius;
		public bool setOriginalRadius = true;
		public float originalRadius = 0.07f;

		public bool useCastAllDamageDetection;
	}

	[System.Serializable]
	public class eventInfo
	{
		public float delayToActivate;

		public UnityEvent eventToUse;

		public bool eventTriggered;

		public bool sendCurrentPlayerOnEvent;
		public eventParameters.eventToCallWithGameObject eventToSendCurrentPlayer;
	}
}