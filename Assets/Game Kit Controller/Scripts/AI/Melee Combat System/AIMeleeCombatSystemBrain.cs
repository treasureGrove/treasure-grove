using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIMeleeCombatSystemBrain : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool combatSystemEnabled = true;

	public bool makeFullCombos;
	public bool useWaitTimeBetweenFullCombos;
	public float minRandomTimeBetweenFullCombos;
	public float maxRandomTimeBetweenFullCombos;

	[Space]
	[Header ("Drop Weapon Settings")]
	[Space]

	public bool canDropWeaponExternallyEnabled = true;

	public bool drawRandomWeaponOnWeaponDrop;

	[Space]
	[Header ("Attack Types Settings")]
	[Space]

	public bool useAlwaysSameAttackType;
	public string sameAttackTypeName;
	public bool alternateAttackTypes;

	public bool checkIfAttackInProcessBeforeCallingNextAttack = true;

	[Space]
	[Header ("Random Time Attack Settings")]
	[Space]

	public bool useRandomAttack;
	public float minTimeBetweenAttacks;
	public bool useRandomTimeBetweenAttacks;
	public float minRandomTimeBetweenAttacks;
	public float maxRandomTimeBetweenAttacks;


	[Space]
	[Header ("Attack Types Settings")]
	[Space]

	public bool attackEnabled;

	public List<string> meleeAttackTypes = new List<string> ();

	public List<int> meleeAttackTypesAmount = new List<int> ();

	[Space]
	[Header ("Block Attack Settings")]
	[Space]

	public bool blockEnabled;

	public Vector2 randomBlockWaitTime;
	public Vector2 randomBlockDuration;

	[Space]
	[Header ("Roll/Dodge Settings")]
	[Space]

	public bool rollEnabled;

	public Vector2 randomRollWaitTime;

	public float minWaitTimeAfterRollActive = 1.3f;

	public List<Vector2> rollMovementDirectionList = new List<Vector2> ();

	[Space]
	[Header ("Random Walk Settings")]
	[Space]

	public bool randomWalkEnabled;

	public Vector2 randomWalkWaitTime;
	public Vector2 randomWalkDuration;
	public Vector2 randomWalkRadius;

	[Space]
	[Header ("Melee Weapon At Distance Settings")]
	[Space]

	public float meleeAttackAtDistanceRate;
	public UnityEvent eventToStartAimMeleeWeapon;
	public UnityEvent eventToStopAimMeleeWeapon;

	public UnityEvent eventToMeleeAttackAtDistance;

	public float minWaitTimeToAimMeleeWeaponAfterDraw = 2;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool checkCurrentTargetState;

	public bool activateBlockIfTargetAttack;
	public float probabilityToActiveBlockIfTargetAttack = 100;
	public float reactionTimeToActiveBlockIfTargetAttack;
	public Vector2 randomBlockDurationIfTargetAttack;
	public Vector2 randomWaitTimeToActivateBlockIfTargetAttack;

	public bool useEventToCancelAttackToActivateAutoBlock;
	public UnityEvent eventToCancelAttackToActivateBlock;

	[Space]

	public bool changeWaitToActivateAttackEnabled = true;

	public bool activateRandomWalkIfWaitToActivateAttackActive;

	[Space]
	[Header ("Search Weapons On Scene Settings")]
	[Space]

	public bool searchWeaponsPickupsOnLevelIfNoWeaponsAvailable;

	public bool useMaxDistanceToGetPickupWeapon;
	public float maxDistanceToGetPickupWeapon;

	[Space]

	public bool useEventOnNoWeaponToPickFromScene;
	public UnityEvent eventOnNoWeaponToPickFromScene;

	[Space]

	public bool useEventsOnNoWeaponsAvailable;
	public bool useEventsOnNoWeaponsAvailableAfterCheckWeaponsOnScene;
	public UnityEvent eventOnNoWeaponsAvailable;

	[Space]
	[Header ("Steal Weapons From Targets Settings")]
	[Space]

	public bool stealWeaponFromCurrentTarget;

	[Range (0, 100)] public float probabilityToStealWeapon = 0;

	public bool stealWeaponOnlyIfNotPickupsLocated;

	public bool useMaxDistanceToStealWeapon;
	public float maxDistanceToGetStealWeapon;

	[Space]

	public bool useEventOnSteal;
	public UnityEvent eventOnStealWeapon;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool showAttackDebugPrint;

	public bool weaponEquiped;

	public bool combatSystemActive;

	public bool waitingForAttackActive;
	float currentRandomTimeToAttack;

	public bool waitingForFullComboComplete;
	float currentRandomTimeToFullCombo;

	public bool walkActive;
	public bool waitingWalkActive;

	public bool blockActive;
	public bool waitingBlockActive;

	public bool waitingRollActive;

	public bool canUseAttackActive;

	public bool attackStatePaused;

	public bool insideMinDistanceToMelee;

	public int currentActionPriority = 1;

	public bool currentMeleeWeaponAtDistance;

	public bool aimingMeleeWeapon;

	public bool rollPaused;

	public bool currentTargetAssigned;

	public bool behaviorStatesPaused;

	public string currentComboNameSelected;

	public bool searchingWeapon;
	public bool characterHasWeapons;

	public bool waitToActivateAttackActive;

	public GameObject currentWeaponToGet;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnCombatActive;
	public UnityEvent eventOnCombatActive;
	public UnityEvent eventOnCombatDeactivate;

	[Space]
	[Header ("Components")]
	[Space]

	public grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;
	public dashSystem mainDashSystem;
	public findObjectivesSystem mainFindObjectivesSystem;

	public AINavMesh mainAINavmeshManager;

	float lastTimeAttack;

	int currentAttackTypeIndex;

	bool weaponInfoStored;

	int currentAttackIndex;

	int currentAttackTypeToAlternateIndex;


	float lastTimeBlockActive;

	float currentBlockWaitTime;

	float currentBlockDuration;

	float lastTimeBlockWaitActive;


	float lastTimeRollActive;

	float lastTimeWaitRollActive;

	float currentRollWaitTime;


	float lastTimeWaitWalkActive;
	float currentWalkTime;
	float lastTimeWalkActive;

	float currentWalkDuration;
	float currentWalkRadius;

	GameObject currentTarget;
	GameObject previousTarget;

	grabbedObjectMeleeAttackSystem targetGrabbedObjectMeleeAttackSystem;

	bool targetCurrentlyAttacking;
	bool targetPreviouslAttacking;

	bool waitingToActivateBlockFromAttack;
	float lastTimeWaitingToActivateBlockFromAttack;

	float currentWaitTimeToActivateBlockIfTargetAttack;

	float lastTimeBlockActiveFromAttack;

	bool rollCoolDownActive;

	float currentPauseAttackStateDuration;
	float lastTimeAttackPauseWithDuration;

	bool checkIfDrawMeleeWeaponActive;

	float randomWaitTime;

	float lastTimeMeleeAttackAtDistance;

	bool checkIfSearchWeapon;

	bool originalRandomWalkEnabled;

	float lastTimeWeaponPicked = 0;

	string currentWeaponNameToPick;


	void Start ()
	{
		originalRandomWalkEnabled = randomWalkEnabled;
	}


	public void updateAI ()
	{
		if (combatSystemActive) {
			if (!weaponInfoStored) {
				if (mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
					meleeAttackTypesAmount = mainGrabbedObjectMeleeAttackSystem.getMeleeAttackTypesAmount ();

					weaponInfoStored = true;

					weaponEquiped = true;
				}
			}

			if (walkActive) {
				if (Time.time > lastTimeWalkActive + currentWalkDuration || mainFindObjectivesSystem.getRemainingDistanceToTarget () < 0.5f) {
					resetRandomWalkState ();
				}
			}

			if (searchingWeapon) {
				bool weaponReached = false;

				if (currentWeaponToGet != null) {
					if (GKC_Utils.distance (mainAINavmeshManager.transform.position, currentWeaponToGet.transform.position) < 1) {
						meleeWeaponPickup currentWeaponPickup = currentWeaponToGet.GetComponent<meleeWeaponPickup> ();

						if (currentWeaponPickup != null) {
							simpleActionButton currentSimpleActionButton = currentWeaponToGet.GetComponentInChildren<simpleActionButton> ();

							if (currentSimpleActionButton != null) {
								mainFindObjectivesSystem.takeObject (currentSimpleActionButton.gameObject);
							}

							weaponReached = true;
						} else {
							grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = 
								currentWeaponToGet.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

							if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
								GKC_Utils.grabPhysicalObjectExternally (mainFindObjectivesSystem.gameObject, currentWeaponToGet);

								weaponReached = true;
							}
						}
					}
				} else {
					weaponReached = true;
				}

				if (weaponReached) {
					characterHasWeapons = mainGrabbedObjectMeleeAttackSystem.getCurrentNumberOfWeaponsAvailable () > 0;

					mainFindObjectivesSystem.setSearchigWeaponState (false);

					searchingWeapon = false;

					checkIfSearchWeapon = false;

					currentWeaponToGet = null;

					mainFindObjectivesSystem.setIgnoreVisionRangeActiveState (true);

					mainFindObjectivesSystem.resetAITargets ();

					mainFindObjectivesSystem.setIgnoreVisionRangeActiveState (false);

					mainGrabbedObjectMeleeAttackSystem.repairObjectFully (currentWeaponNameToPick);

					lastTimeWeaponPicked = Time.time;

					if (showDebugPrint) {
						print ("Weapon picked, resuming state on AI");
					}
				}
			}

			if (checkCurrentTargetState) {
				if (!currentTargetAssigned || currentTarget != previousTarget) {
					currentTarget = mainFindObjectivesSystem.getCurrentTargetToAttack ();

					if (currentTarget != null) {
						previousTarget = currentTarget;

						playerComponentsManager currentPlayerComponentsManager = currentTarget.GetComponent<playerComponentsManager> ();

						if (currentPlayerComponentsManager != null) {
							targetGrabbedObjectMeleeAttackSystem = currentPlayerComponentsManager.getGrabbedObjectMeleeAttackSystem ();
						
							currentTargetAssigned = true;
						}

					} else {
						currentTargetAssigned = false;
						previousTarget = null;
					}
				}

				if (currentTargetAssigned) {
					targetCurrentlyAttacking = targetGrabbedObjectMeleeAttackSystem.isAttackInProcess ();

					if (activateBlockIfTargetAttack) {
						if (targetCurrentlyAttacking != targetPreviouslAttacking) {
							targetPreviouslAttacking = targetCurrentlyAttacking;
						} 

						if (currentWaitTimeToActivateBlockIfTargetAttack > 0) {
							if (Time.time > lastTimeBlockActiveFromAttack + currentWaitTimeToActivateBlockIfTargetAttack &&
							    Time.time > lastTimeBlockActive + currentBlockDuration) {
								waitingToActivateBlockFromAttack = false;

								currentWaitTimeToActivateBlockIfTargetAttack = 0;

								if (showDebugPrint) {
									print ("can block again");
								}

								lastTimeWaitingToActivateBlockFromAttack = 0;
							}
						}

						if (targetCurrentlyAttacking) {
							if (currentWaitTimeToActivateBlockIfTargetAttack == 0 && lastTimeWaitingToActivateBlockFromAttack == 0) {
								float probablityToActivateBlock = Random.Range (0, 100);

								if (probabilityToActiveBlockIfTargetAttack >= probablityToActivateBlock) {
									waitingToActivateBlockFromAttack = true;
									lastTimeWaitingToActivateBlockFromAttack = Time.time;

									lastTimeBlockActiveFromAttack = 0;
								}

								if (showDebugPrint) {
									print ("target attacking");
								}
							}
						}

						if (waitingToActivateBlockFromAttack) {
							if (Time.time > reactionTimeToActiveBlockIfTargetAttack + lastTimeWaitingToActivateBlockFromAttack) {
								if (showDebugPrint) {
									print ("check to activate block");
								}

								bool canActivateBlock = true;

								if (rollEnabled) {
									if (Time.time < lastTimeRollActive + 1) {
										canActivateBlock = false;

										if (showDebugPrint) {
											print ("1");
										}
									}
								}

								if (lastTimeAttack > 0 && Time.time < lastTimeAttack + 0.2f) {
									if (useEventToCancelAttackToActivateAutoBlock) {
										if (showDebugPrint) {
											print ("cancel attack to activate auto block");
										}

										eventToCancelAttackToActivateBlock.Invoke ();
									} else {
										canActivateBlock = false;

										if (showDebugPrint) {
											print ("2");
										}
									}
								}

								if (!targetCurrentlyAttacking) {
									canActivateBlock = false;

									if (showDebugPrint) {
										print ("3");
									}
								}

								if (!insideMinDistanceToMelee) {
									if (mainFindObjectivesSystem.getDistanceToTarget () > 4) {
										canActivateBlock = false;

										if (showDebugPrint) {
											print ("4");
										}
									}
								}

								if (canActivateBlock) {
									if (showDebugPrint) {
										print ("block can be activated");
									}

									if (mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ()) {
										if (useEventToCancelAttackToActivateAutoBlock) {
											if (showDebugPrint) {
												print ("cancel attack to activate auto block");
											}

											eventToCancelAttackToActivateBlock.Invoke ();
										} 
									}

									resetStates ();

									currentBlockDuration = Random.Range (randomBlockDurationIfTargetAttack.x, randomBlockDurationIfTargetAttack.y);

									blockActive = true;

									mainGrabbedObjectMeleeAttackSystem.inputActivateBlock ();

									lastTimeBlockActiveFromAttack = Time.time;

									lastTimeBlockActive = Time.time;

									waitingBlockActive = true;

									currentWaitTimeToActivateBlockIfTargetAttack = Random.Range (randomWaitTimeToActivateBlockIfTargetAttack.x, randomWaitTimeToActivateBlockIfTargetAttack.y);
								} else {
									if (showDebugPrint) {
										print ("block can't be activated");
									}

									currentWaitTimeToActivateBlockIfTargetAttack = 0;

									lastTimeWaitingToActivateBlockFromAttack = 0;
								}

								waitingToActivateBlockFromAttack = false;
							}
						}

						if (!blockEnabled && blockActive) {
							updateBlockState ();
						}
					}

					if (targetGrabbedObjectMeleeAttackSystem.isBlockActive ()) {

					}
				}
			}
		}
	}

	public void resetRandomWalkState ()
	{
		mainFindObjectivesSystem.setRandomWalkPositionState (0);

		waitingWalkActive = false;

		walkActive = false;

		lastTimeWalkActive = Time.time;
	}

	public void resetBlockState ()
	{
		waitingBlockActive = false;

		if (blockActive) {
			mainGrabbedObjectMeleeAttackSystem.inputDeactivateBlock ();
		}

		blockActive = false;

		lastTimeBlockActive = Time.time;
	}

	public void resetRollState ()
	{
		waitingRollActive = false;

		lastTimeRollActive = Time.time;
	}

	public void dropWeapon ()
	{
		if (!combatSystemActive) {
			return;
		}

		if (!weaponEquiped) {
			return;
		}

		if (mainFindObjectivesSystem.isAIPaused ()) {
			return;
		}

		if (behaviorStatesPaused) {
			return;
		}

		if (!canDropWeaponExternallyEnabled) {
			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ()) {
			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.isActionActive ()) {
			return;
		}


		string previousWeaponName = mainGrabbedObjectMeleeAttackSystem.getCurrentMeleeWeaponName ();

		if (mainGrabbedObjectMeleeAttackSystem.dropMeleeWeaponsExternally ()) {

			if (aimingMeleeWeapon) {
				setAimMeleeWeaponState (false);
			}

			resetAttackState ();

			updateWeaponsAvailableState ();

			weaponEquiped = characterHasWeapons;

			if (characterHasWeapons) {
				if (drawRandomWeaponOnWeaponDrop) {
					mainGrabbedObjectMeleeAttackSystem.drawRandomWeaponFromPrefabList (previousWeaponName);
				} else {
					mainGrabbedObjectMeleeAttackSystem.drawNextWeaponAvailable ();
				}
			}
		}
	}

	public void resetAttackState ()
	{
		
	}

	public void resetStates ()
	{
		resetRandomWalkState ();

		resetBlockState ();

		resetRollState ();

		resetAttackState ();

		currentActionPriority = Random.Range (1, 3);
	}

	public void checkIfResetStatsOnRandomWalk ()
	{
		if (walkActive) {
			resetStates ();
		}
	}

	public void checkBlockState ()
	{
		if (blockEnabled) {
			updateBlockState ();
		}
	}

	public void updateBlockState ()
	{
		if (rollEnabled) {
			if (Time.time < lastTimeRollActive + 1) {
				return;
			}
		}

		if (walkActive) {
			return;
		}

//		print (mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ());

		if (mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ()) {
			return;
		}

		if (waitingRollActive) {
			return;
		}

		if (!insideMinDistanceToMelee) {
			if (blockActive) {
				resetBlockState ();

				lastTimeBlockActive = 0;
			}

			return;
		}

		if (waitingBlockActive) {
			if (blockActive) {
				if (Time.time > lastTimeBlockActive + currentBlockDuration) {
					resetBlockState ();
				}
			} else {
				if (Time.time > lastTimeBlockWaitActive + currentBlockWaitTime) {
					blockActive = true;

					mainGrabbedObjectMeleeAttackSystem.inputActivateBlock ();

					lastTimeBlockActive = Time.time;
				}
			}
		} else {
			if (Time.time > lastTimeBlockActive + randomWaitTime && currentActionPriority == 1) { 
				currentBlockWaitTime = Random.Range (randomBlockWaitTime.x, randomBlockWaitTime.y);

				currentBlockDuration = Random.Range (randomBlockDuration.x, randomBlockDuration.y);

				lastTimeBlockWaitActive = Time.time;

				waitingBlockActive = true;

				blockActive = false;

				randomWaitTime = Random.Range (0.1f, 0.5f);

				currentActionPriority = Random.Range (1, 3);
			}
		}
	}

	public void checkRollState ()
	{
		if (rollEnabled) {
			
			if (blockActive) {
				return;
			}

			if (walkActive) {
				return;
			}

			if (mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ()) {
				return;
			}

			if (waitingBlockActive) {
				return;
			}

			if (!insideMinDistanceToMelee) {
				resetRollState ();

				lastTimeRollActive = 0;

				return;
			}

			if (waitingRollActive) {
				if (Time.time > lastTimeWaitRollActive + currentRollWaitTime) {

					int randomRollMovementDirection = Random.Range (0, rollMovementDirectionList.Count);

					mainDashSystem.activateDashStateWithCustomDirection (rollMovementDirectionList [randomRollMovementDirection]);

					resetRollState ();
				}
			} else {
				if (Time.time > lastTimeRollActive + randomWaitTime && currentActionPriority == 2) {
					currentRollWaitTime = Random.Range (randomRollWaitTime.x, randomRollWaitTime.y);

					lastTimeWaitRollActive = Time.time;

					waitingRollActive = true;

					randomWaitTime = Random.Range (0.1f, 0.5f);

					currentActionPriority = Random.Range (1, 3);
				}
			}
		}
	}

	public void checkWalkState ()
	{
		if (randomWalkEnabled) {

			if (blockActive) {
				return;
			}

			if (mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ()) {
				return;
			}

			rollCoolDownActive = Time.time < lastTimeRollActive + 0.7f;

			if (rollCoolDownActive) {
				return;
			}

			if (waitingWalkActive) {
				if (!walkActive) {
						
					if (Time.time > lastTimeWaitWalkActive + currentWalkTime) {
						mainFindObjectivesSystem.setRandomWalkPositionState (currentWalkRadius);
						
						lastTimeWalkActive = Time.time;

						walkActive = true;
					}
				}
			} else {
				currentWalkTime = Random.Range (randomWalkWaitTime.x, randomWalkWaitTime.y);

				lastTimeWaitWalkActive = Time.time;

				waitingWalkActive = true;

				currentWalkDuration = Random.Range (randomWalkDuration.x, randomWalkDuration.y);

				currentWalkRadius = Random.Range (randomWalkRadius.x, randomWalkRadius.y);

				walkActive = false;
			}
		}
	}

	public void updateInsideMinDistance (bool newInsideMinDistanceToAttack)
	{
		insideMinDistanceToMelee = newInsideMinDistanceToAttack;

		if (insideMinDistanceToMelee) {
			if (checkIfDrawMeleeWeaponActive) {
				if (!mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
					mainGrabbedObjectMeleeAttackSystem.drawOrKeepMeleeWeapon ();

					weaponEquiped = true;
				}

				checkIfDrawMeleeWeaponActive = false;
			}
		} else {
			if (currentMeleeWeaponAtDistance) {
				if (aimingMeleeWeapon) {
					setAimMeleeWeaponState (false);
				}
			}
		}
	}

	public void updateMainMeleeBehavior ()
	{
		if (!combatSystemActive) {
			return;
		}

		if (behaviorStatesPaused) {
			return;
		}

		if (mainFindObjectivesSystem.isAIPaused ()) {
			return;
		}

		checkWalkState ();

		if (walkActive) {
			return;
		}

		checkBlockState ();

		if (blockEnabled) {
			if (rollEnabled) {
				if (Time.time < lastTimeRollActive + 0.3f) {
					return;
				}
			}
		}

		if (blockActive) {
			return;
		}

		if (blockEnabled) {
			if (rollEnabled) {
				if (Time.time < lastTimeBlockActive + 0.3f) {
					return;
				}
			}
		}

		checkRollState ();

		if (rollEnabled) {
			if (Time.time < lastTimeRollActive + minWaitTimeAfterRollActive) {
				return;
			}
		}
	
		if (!insideMinDistanceToMelee || !canUseAttackActive) {
			return;
		}

		if (currentPauseAttackStateDuration > 0) {
			if (Time.time > currentPauseAttackStateDuration + lastTimeAttackPauseWithDuration) {

				attackStatePaused = false;

				currentPauseAttackStateDuration = 0;
			} else {
				return;
			}
		}

		if (characterHasWeapons) {

			if (waitToActivateAttackActive) {
				if (activateRandomWalkIfWaitToActivateAttackActive) {
					if (!walkActive) {
						currentWalkTime = Random.Range (randomWalkWaitTime.x, randomWalkWaitTime.y);

						currentWalkDuration = Random.Range (randomWalkDuration.x, randomWalkDuration.y);

						currentWalkRadius = Random.Range (randomWalkRadius.x, randomWalkRadius.y);

						mainFindObjectivesSystem.setRandomWalkPositionState (currentWalkRadius);

						lastTimeWalkActive = Time.time;

						walkActive = true;
					}
				}

				return;
			}
			
			checkAttackState ();
		} else {
			checkNoMeleeWeaponsAvailableState ();
		}
	}

	public void checkNoMeleeWeaponsAvailableState ()
	{
		if (!combatSystemActive) {
			return;
		}
	
		if (!searchingWeapon && !checkIfSearchWeapon) {
			if (lastTimeWeaponPicked > 0) {
				if (Time.time < lastTimeWeaponPicked + 0.75f) {
					if (showDebugPrint) {
						print ("NO; WAITING TO WEAPON PICKED\n\n");
					}

					return;
				} else {
					lastTimeWeaponPicked = 0;
				}
			}

			characterHasWeapons = mainGrabbedObjectMeleeAttackSystem.getCurrentNumberOfWeaponsAvailable () > 0;

			//seach for the closest weapon
			if (!characterHasWeapons) {

				if (!stealWeaponOnlyIfNotPickupsLocated) {
					if (checkIfStealWeaponFromCurrentTarget ()) {
						return;
					}
				}

				if (!useEventsOnNoWeaponsAvailableAfterCheckWeaponsOnScene) {
					checkEventOnNoWeaponsAvailable ();
				}

				bool weaponFound = false;

				if (searchWeaponsPickupsOnLevelIfNoWeaponsAvailable) {
					checkIfSearchWeapon = true;

					if (showDebugPrint) {
						print ("checking if weapons found on scene");
					}

					meleeWeaponPickup[] weaponPickupList = FindObjectsOfType (typeof(meleeWeaponPickup)) as meleeWeaponPickup[];

					List<GameObject> weaponObjectsDetected = new List<GameObject> ();

					if (weaponPickupList.Length > 0) {
						for (int i = 0; i < weaponPickupList.Length; i++) {
							bool checkObjectResult = true;

							if (useMaxDistanceToGetPickupWeapon) {
								float distance = GKC_Utils.distance (weaponPickupList [i].transform.position, mainAINavmeshManager.transform.position);

								if (distance > maxDistanceToGetPickupWeapon) {
									checkObjectResult = false;
								}
							}

							if (checkObjectResult) {
								if (weaponPickupList [i].gameObject.activeSelf && weaponPickupList [i].gameObject.activeInHierarchy) {
									weaponObjectsDetected.Add (weaponPickupList [i].gameObject);
								}
							}
						}
					}

					if (weaponObjectsDetected.Count == 0) {
						if (showDebugPrint) {
							print ("no pickups detected, searching for physical objects");
						}

						grabPhysicalObjectMeleeAttackSystem[] grabPhysicalObjectMeleeAttackSystemList = FindObjectsOfType (typeof(grabPhysicalObjectMeleeAttackSystem)) as grabPhysicalObjectMeleeAttackSystem[];

						for (int i = 0; i < grabPhysicalObjectMeleeAttackSystemList.Length; i++) {
							bool checkObjectResult = true;

							if (useMaxDistanceToGetPickupWeapon) {
								float distance = GKC_Utils.distance (grabPhysicalObjectMeleeAttackSystemList [i].transform.position, mainAINavmeshManager.transform.position);

								if (distance > maxDistanceToGetPickupWeapon) {
									checkObjectResult = false;
								}
							}

							if (checkObjectResult) {
								if (!grabPhysicalObjectMeleeAttackSystemList [i].isWeaponCarriedByCharacter () &&
								    grabPhysicalObjectMeleeAttackSystemList [i].gameObject.activeSelf &&
								    grabPhysicalObjectMeleeAttackSystemList [i].gameObject.activeInHierarchy) {

									float lastTimeWeaponDropped = grabPhysicalObjectMeleeAttackSystemList [i].getLastTimeWeaponDropped ();

									if (lastTimeWeaponDropped == 0 || Time.time > lastTimeWeaponDropped + 1) {
										weaponObjectsDetected.Add (grabPhysicalObjectMeleeAttackSystemList [i].gameObject);
									}
								}
							}
						}
					}
						
					if (weaponObjectsDetected.Count > 0) {
						if (showDebugPrint) {
							print ("weapons found on scene");
						}

						string currentWeaponName = "";

						for (int i = 0; i < weaponObjectsDetected.Count; i++) {
							if (!weaponFound) {
								currentWeaponName = "";

								meleeWeaponPickup currentWeaponPickup = weaponObjectsDetected [i].GetComponent<meleeWeaponPickup> ();

								if (currentWeaponPickup != null) {
									currentWeaponName = currentWeaponPickup.weaponName;
								} else {
									grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = 
										weaponObjectsDetected [i].GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

									if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
										currentWeaponName = currentGrabPhysicalObjectMeleeAttackSystem.getWeaponName ();
									}
								}

								currentWeaponNameToPick = currentWeaponName;

								if (showDebugPrint) {
									print ("checking if weapon with name " + currentWeaponName + " can be used");
								}

								if (mainGrabbedObjectMeleeAttackSystem.checkIfCanUseMeleeWeaponPrefabByName (currentWeaponName)) {
									if (mainAINavmeshManager.updateCurrentNavMeshPath (weaponObjectsDetected [i].transform.position)) {
										mainFindObjectivesSystem.setSearchigWeaponState (true);

										currentWeaponToGet = weaponObjectsDetected [i];

										mainAINavmeshManager.setTarget (weaponObjectsDetected [i].transform);

										mainAINavmeshManager.setTargetType (false, true);

										weaponFound = true;

										mainAINavmeshManager.lookAtTaget (false);

										if (showDebugPrint) {
											print ("weapon to use located " + currentWeaponName + ", setting searching weapon state on AI");
										}

										searchingWeapon = true;
									} else {
										if (showDebugPrint) {
											print ("weapon to use located " + currentWeaponName + ", but can't be reached");
										}
									}
								} else {
									if (showDebugPrint) {
										print ("weapon " + currentWeaponName + " can't be used");
									}
								}
							}
						}
					} else {
						if (showDebugPrint) {
							print ("no pickups or physical objects found");
						}
					}

					if (!weaponFound) {
						if (stealWeaponOnlyIfNotPickupsLocated) {
							if (checkIfStealWeaponFromCurrentTarget ()) {
								return;
							}
						}

						if (useEventOnNoWeaponToPickFromScene) {
							eventOnNoWeaponToPickFromScene.Invoke ();
						}
					}
				}

				if (!weaponFound) {
					if (stealWeaponOnlyIfNotPickupsLocated) {
						if (checkIfStealWeaponFromCurrentTarget ()) {
							return;
						}
					}

					if (showDebugPrint) {
						print ("no weapons located or out of range, activating no weapons detected mode");
					}
						
					if (useEventsOnNoWeaponsAvailableAfterCheckWeaponsOnScene) {
						checkEventOnNoWeaponsAvailable ();
					}
				}

				//it will need to check if the weapon can be seen by the character and if it is can be reached by the navmesh
			}

			mainFindObjectivesSystem.setLookingAtTargetPositionState (false);
		}
	}

	void checkEventOnNoWeaponsAvailable ()
	{
		if (useEventsOnNoWeaponsAvailable) {
			eventOnNoWeaponsAvailable.Invoke ();
		}
	}

	bool checkIfStealWeaponFromCurrentTarget ()
	{
		if (stealWeaponFromCurrentTarget) {
			if (showDebugPrint) {
				print ("Checking probability to steal weapon");
			}

			float currentProbability = Random.Range (0, 100);

			if (currentProbability < probabilityToStealWeapon) {
				if (showDebugPrint) {
					print ("AI can check if steal object, checking target state");
				}

				GameObject currentTarget = mainFindObjectivesSystem.getCurrentTargetToAttack ();

				if (currentTarget != null) {
					if (useMaxDistanceToStealWeapon) {
						float distance = GKC_Utils.distance (currentTarget.transform.position, mainAINavmeshManager.transform.position);

						if (distance > maxDistanceToGetStealWeapon) {
							return false;
						}
					}

					playerComponentsManager currentPlayerComponentsManager = currentTarget.GetComponent<playerComponentsManager> ();

					if (currentPlayerComponentsManager != null) {
						playerController currentPlayerController = currentPlayerComponentsManager.getPlayerController ();

						if (currentPlayerController.isPlayerUsingMeleeWeapons ()) {
							if (showDebugPrint) {
								print ("target is using melee weapons, checking weapon it self");
							}

							grabbedObjectMeleeAttackSystem currentGrabbedObjectMeleeAttackSystem = currentPlayerComponentsManager.getGrabbedObjectMeleeAttackSystem ();

							if (currentGrabbedObjectMeleeAttackSystem.canWeaponsBeStolenFromCharacter ()) {
								string currentWeaponName = currentGrabbedObjectMeleeAttackSystem.getCurrentMeleeWeaponName ();

								if (mainGrabbedObjectMeleeAttackSystem.checkIfCanUseMeleeWeaponPrefabByName (currentWeaponName)) {
									if (currentPlayerController.isCharacterUsedByAI ()) {
										currentGrabbedObjectMeleeAttackSystem.dropMeleeWeaponsExternallyWithoutResultAndDestroyIt ();
									} else {
										inventoryManager currentInventoryManager = currentPlayerComponentsManager.getInventoryManager ();

										if (currentInventoryManager != null) {
											currentInventoryManager.removeObjectAmountFromInventoryByName (currentWeaponName, 1);
										}
									}

									mainGrabbedObjectMeleeAttackSystem.mainMeleeWeaponsGrabbedManager.checkMeleeWeaponToUse (currentWeaponName, false);

									currentGrabbedObjectMeleeAttackSystem.mainMeleeWeaponsGrabbedManager.checkEventOnWeaponStolen ();

									if (showDebugPrint) {
										print ("weapon stolen from target");
									}

									if (useEventOnSteal) {
										eventOnStealWeapon.Invoke ();
									}

									return true;
								} else {
									if (showDebugPrint) {
										print ("AI can't use weapon from target");
									}
								}
							}
						} else {
							if (showDebugPrint) {
								print ("target is not using melee weapons");
							}
						}
					}
				}
			}
		}

		return false;
	}

	public void checkAttackState ()
	{
		if (!attackEnabled) {
			return;
		}

		if (currentMeleeWeaponAtDistance) {
			if (!aimingMeleeWeapon) {
				float lastTimeDrawMeleeWeapon = mainGrabbedObjectMeleeAttackSystem.getLastTimeDrawMeleeWeapon ();
			
				if (lastTimeDrawMeleeWeapon > -1 && Time.time > lastTimeDrawMeleeWeapon + minWaitTimeToAimMeleeWeaponAfterDraw) {
					if (showDebugPrint) {
						print ("AIM AFTER DRAW");
					}

					setAimMeleeWeaponState (true);
				}
			}

			if (Time.time > meleeAttackAtDistanceRate + lastTimeMeleeAttackAtDistance) {
				eventToMeleeAttackAtDistance.Invoke ();

				lastTimeMeleeAttackAtDistance = Time.time;
			}


			return;
		}

		if (currentRandomTimeToFullCombo > 0) {
			if (Time.time < currentRandomTimeToFullCombo + lastTimeAttack) {
				return;
			} else {
				currentRandomTimeToFullCombo = 0;
			}
		}

		bool canDoAttack = false;

		if (useRandomTimeBetweenAttacks) {
			if (!waitingForAttackActive) {
				currentRandomTimeToAttack = Random.Range (minRandomTimeBetweenAttacks, maxRandomTimeBetweenAttacks);

				waitingForAttackActive = true;
			}
		} else {
			currentRandomTimeToAttack = minTimeBetweenAttacks;
		}

		if (Time.time > currentRandomTimeToAttack + lastTimeAttack) {
			canDoAttack = true;
		}

		if (makeFullCombos) {
			if (!waitingForFullComboComplete) {
				waitingForFullComboComplete = true;

				currentAttackTypeIndex = Random.Range (0, meleeAttackTypes.Count);

				currentComboNameSelected = meleeAttackTypes [currentAttackTypeIndex];

				currentAttackIndex = 0;
			}
		}

		if (canDoAttack) {
			if (checkIfAttackInProcessBeforeCallingNextAttack) {
				if (!mainGrabbedObjectMeleeAttackSystem.canActivateAttack ()) {
					if (showAttackDebugPrint) {
						print ("attack in process, cancelling check of attack");
					}

					return;
				}
			}

			if (waitingForFullComboComplete) {
				if (currentAttackIndex >= meleeAttackTypesAmount [currentAttackTypeIndex]) {
					waitingForFullComboComplete = false;

					if (useWaitTimeBetweenFullCombos) {
						currentRandomTimeToFullCombo = Random.Range (minRandomTimeBetweenFullCombos, maxRandomTimeBetweenFullCombos);
					}
				} else if (!mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ()) {
					useAttack (currentComboNameSelected);

					currentAttackIndex++;

					lastTimeAttack = Time.time;
				}
			} else {
				if (useRandomAttack) {
					useAttack (meleeAttackTypes [Random.Range (0, meleeAttackTypes.Count)]);

					lastTimeAttack = Time.time;
				} else if (useAlwaysSameAttackType) {
					useAttack (sameAttackTypeName);

					lastTimeAttack = Time.time;

				} else if (alternateAttackTypes) {
					useAttack (meleeAttackTypes [currentAttackTypeToAlternateIndex]);

					currentAttackTypeToAlternateIndex++;

					if (currentAttackTypeToAlternateIndex == meleeAttackTypes.Count) {
						currentAttackTypeToAlternateIndex = 0;
					}

					lastTimeAttack = Time.time;
				}
			}

			if (useRandomTimeBetweenAttacks) {
				waitingForAttackActive = false;
			}
		}
	}

	public void useAttack (string attackType)
	{
		if (showAttackDebugPrint) {
			print ("use attack" + attackType);
		}
			
		mainGrabbedObjectMeleeAttackSystem.activateGrabbedObjectMeleeAttack (attackType);
	}

	public void updateMainMeleeAttack (bool newCanUseAttackActiveState)
	{
		canUseAttackActive = newCanUseAttackActiveState;
	}

	public void setCombatSystemActiveState (bool state)
	{
		if (!combatSystemEnabled) {
			return;
		}

		combatSystemActive = state;

		checkEventsOnCombatStateChange (combatSystemActive);

		if (combatSystemActive) {
			currentActionPriority = Random.Range (1, 3);
		}
	}

	void checkEventsOnCombatStateChange (bool state)
	{
		if (useEventsOnCombatActive) {
			if (state) {
				eventOnCombatActive.Invoke ();
			} else {
				eventOnCombatDeactivate.Invoke ();
			}
		}
	}

	public void pauseAttackDuringXTime (float newDuration)
	{
		currentPauseAttackStateDuration = newDuration;

		lastTimeAttackPauseWithDuration = Time.time;

		attackStatePaused = true;

		if (showAttackDebugPrint) {
			print ("Pause attack");
		}
	}

	public void setWaitToActivateAttackActiveState (bool state)
	{
		if (!changeWaitToActivateAttackEnabled) {
			return;
		}

		waitToActivateAttackActive = state;

		if (activateRandomWalkIfWaitToActivateAttackActive) {
			if (state) {
				currentWalkTime = Random.Range (randomWalkWaitTime.x, randomWalkWaitTime.y);

				currentWalkDuration = Random.Range (randomWalkDuration.x, randomWalkDuration.y);

				currentWalkRadius = Random.Range (randomWalkRadius.x, randomWalkRadius.y);

				mainFindObjectivesSystem.setRandomWalkPositionState (currentWalkRadius);

				lastTimeWalkActive = Time.time;

				walkActive = true;
			} else {
				if (walkActive) {
					resetRandomWalkState ();
				}
			}
		}
	}

	public void setUseRandomWalkEnabledState (bool state)
	{
		randomWalkEnabled = state;
	}

	public void setOriginalUseRandomWalkEnabledState ()
	{
		setUseRandomWalkEnabledState (originalRandomWalkEnabled);
	}

	public void resetBehaviorStates ()
	{
		resetStates ();

		waitingForAttackActive = false;

		waitingForFullComboComplete = false;

		currentRandomTimeToFullCombo = 0;

		checkIfDrawMeleeWeaponActive = true;

		if (currentMeleeWeaponAtDistance) {
			setAimMeleeWeaponState (false);
		}

		insideMinDistanceToMelee = false;

		aimingMeleeWeapon = false;
	}

	public void setCurrentMeleeWeaponAtDistanceState (bool state)
	{
		currentMeleeWeaponAtDistance = state;

		if (!currentMeleeWeaponAtDistance) {
			setAimMeleeWeaponState (false);
		}
	}

	void setAimMeleeWeaponState (bool state)
	{
		if (state) {
			if (!aimingMeleeWeapon) {
				lastTimeMeleeAttackAtDistance = Time.time;

				eventToStartAimMeleeWeapon.Invoke ();

				aimingMeleeWeapon = true;
			}
		} else {
			if (aimingMeleeWeapon) {
				eventToStopAimMeleeWeapon.Invoke ();

				aimingMeleeWeapon = false;
			}
		}
	}

	public void setPauseRollState (bool state)
	{
		rollPaused = state;

		if (rollPaused) {
			resetRollState ();

			lastTimeRollActive = 0;
		}
	}

	public void checkIfDrawMeleeWeaponAfterResumingAI ()
	{
		if (combatSystemActive) {
			if (checkIfDrawMeleeWeaponActive) {
				if (!mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
					mainGrabbedObjectMeleeAttackSystem.drawOrKeepMeleeWeapon ();

					weaponEquiped = true;
				}

				checkIfDrawMeleeWeaponActive = false;
			}
		}
	}

	public void updateWeaponsAvailableState ()
	{
		characterHasWeapons = mainGrabbedObjectMeleeAttackSystem.getCurrentNumberOfWeaponsAvailable () > 0;
	}

	public void setBehaviorStatesPausedState (bool state)
	{
		behaviorStatesPaused = state;
	}

	public void setSameAttackTypeNameValue (string newValue)
	{
		sameAttackTypeName = newValue;
	}

	public bool isWeaponEquiped ()
	{
		return weaponEquiped;
	}

	public bool isAIBehaviorAttackInProcess ()
	{
		return mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ();
	}
}
