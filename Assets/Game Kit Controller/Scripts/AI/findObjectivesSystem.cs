using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class findObjectivesSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask layerMask;
	public LayerMask layerToCheckTargets;
	public AIAttackType attackType;

	public float minDistanceToLookAtTarget = 6;

	public float extraFieldOfViewRadiusOnSpot;
	public bool avoidEnemies;

	public float lookAtTargetSpeed = 10;

	public float delayToEnableViewTriggerAfterTargetLost = 1;

	public bool useMaxDistanceRangeToDetectTargets;
	public float maxDistanceRangeToDetectTargets;

	public bool avoidLoseTrackOfDetectedTargets;

	public bool ignoreTriggersOnTargetDetection = true;

	[Space]
	[Header ("Attack On Place Settings")]
	[Space]

	public bool attackAlwaysOnPlace;

	public float maxRaycastDistanceToAlwaysAttackOnPlace = 200;

	public float minDistanceToAttackOnPlace = 3;
	public float maxDistanceToAttackOnPlace = 8;

	public float maxRaycastDistanceToAttackOnPlace = 7;

	[Space]
	[Header ("Checking Threat Settings")]
	[Space]

	public float timeToCheckSuspect;

	public bool checkThreatsEnabled = true;

	public bool pauseMovementWhenCheckingThreats;

	[Space]
	[Header ("Weapons Settings")]
	[Space]

	public float minDistanceToDraw;
	public float minDistanceToAim;
	public float minDistanceToShoot;

	public float minDistanceToEnemyUsingWeapons = 2;

	public AIBehaviorInfo mainWeaponAIBehavior;

	[Space]
	[Header ("Powers Settings")]
	[Space]

	public float minDistanceToAimPower = 5;
	public float minDistanceToShootPower = 4;

	public float minDistanceToEnemyUsingPowers = 2;

	public AIBehaviorInfo mainPowerAIBehavior;

	[Space]
	[Header ("Close Combat Settings")]
	[Space]

	public float minDistanceToCloseCombat = 1.5f;

	public float minDistanceToEnemyUsingCloseCombat = 1;

	public AIBehaviorInfo mainCloseCombatAIBehavior;

	[Space]
	[Header ("Melee Combat Settings")]
	[Space]

	public float minDistanceToMelee = 1.5f;

	public float minDistanceToEnemyUsingMelee = 1.5f;

	public float minDistanceToMeleeAtDistance = 4.1f;

	public float minDistanceToEnemyUsingMeleeAtDistance = 4;

	public AIBehaviorInfo mainMeleAIBehavior;

	[Space]
	[Header ("AI Abilities Behavior State")]
	[Space]

	public bool useAbilitiesBehavior;

	public AIBehaviorInfo abilitiesAIBehavior;

	[Space]
	[Header ("Vision Range Settings")]
	[Space]

	public bool useVisionRange = true;
	public float visionRange = 90;

	public bool allowDetectionWhenTooCloseEvenNotVisible;
	public float minDistanceToAdquireTarget = 2;
	public bool ignoreVisionRangeWhenGetUp;
	public float timeToIgnoreVisionRange = 1;

	public bool useMinAngleToTargetToAttack = true;
	public float minAngleToTargetToAttack = 60;

	public bool useMaxTimeToLoseTargetIfNotVisible;
	public float maxTimeToLoseTargetIfNotVisible = 4;

	public bool searchEnemiesAroundOnGetUpEnabled = true;

	[Space]
	[Header ("Hearing Settings")]
	[Space]

	public bool canHearNoises = true;
	public bool checkNoiseDecibels;
	[Range (0, 2)]public float decibelsDetectionAmount = 0.1f;
	public LayerMask layerToCheckNoises;
	public float raycastDistanceToCheckNoises = 2;
	public float timeToCheckNoise = 4;

	public bool useMaxHearDistance;
	public float maxHearDistance;

	public bool checkNoisesEvenIfPartnerFound = true;

	[Space]

	public bool useEventOnStartCheckNoise;
	public UnityEvent eventOnStartCheckNoise;

	[Space]

	public bool useNoiseReactionInfo;
	public bool checkRegularNoiseStateIfReactionInfoNotFound = true;
	public noiseReactionData mainNoiseReactionData;

	[Space]
	[Header ("Hidden Target Visibility Settings")]
	[Space]

	public bool canSeeHiddenTargets;
	public bool canSeeTargetsOnStealthMode;

	[Space]
	[Header ("Partner Settings")]
	[Space]

	public bool followPartnerOnTrigger = true;
	public string factionToFollowAsPartner = "Player";
	public bool removePartnerIfTooFarFromAI;
	public float minDistanceToRemovePartner;

	public bool removePartnerIfDead;

	public bool removePartnerAfterDelay;
	public float delayToRemoveCurrentPartner;

	[Space]

	public bool useEventOnPartnerFound;
	public UnityEvent eventOnPartnerFound;

	[Space]
	[Header ("Wander Settings")]
	[Space]

	public bool wanderEnabled;
	public Vector2 delaySecondsToWanderToRandomPositionRange;
	public Vector2 wanderPositionRange;
	public bool useMaxDistanceToWanderFromInitialPosition;
	public float maxDistanceToWanderFromInitialPosition;
	public bool wanderAlwaysFartherFromInitialPosition;
	public float maxHeightToWander;
	public LayerMask layerMaskToRaycastToWander;

	public bool useCustomMovementSpeedOnWonder;
	public bool useRandomMovementSpeedOnWander;
	public Vector2 randomRangeMovementSpeedOnWander;
	public float movementSpeedOnWander;

	[Space]

	public bool useEventOnWanderPositionReached;
	public UnityEvent eventOnWanderPositionReached;

	public bool useEventsOnInterruptWanderState;
	public UnityEvent eventsOnInterruptWanderState;

	[Space]
	[Header ("Wander States Settings")]
	[Space]

	public bool useEventsOnWanderingInfoListOnPositionReached;

	[Space]

	public  List<wanderingInfo> wanderingInfoList = new List<wanderingInfo> ();

	[Space]
	[Header ("Return To Position After Action Settings")]
	[Space]

	public bool returnToPositionWhenNoiseChecked;
	public Transform positionToReturnAfterNoiseChecked;
	public bool adjustPlayerRotationAfterNoiseChecked;

	public bool returnToPositionWhenPartnerRemoved;
	public Transform positionToReturnAfterPartnerRemoved;
	public bool adjustPlayerRotationAfterPlayerRemoved;

	public bool returnToPositionAfterNoEnemiesFound;
	public Transform positionToReturnAfterNoEnemiesFound;
	public bool adjustPlayerRotationAfterNoEnemiesFound;

	[Space]
	[Header ("Alert About Target Settings")]
	[Space]

	public bool alertFactionOnSpotted;
	public float alertFactionRadius = 10;
	public bool alertFactionFromPlayerOnDeath;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool checkRaycastToViewTarget = true;
	public LayerMask layerToCheckRaycastToViewTarget;

	public string surprisedCharacterStateName = "Surprised";
	public string wonderingCharacterStateName = "Wondering";

	public float minDistanceToTargetToUseStrafeMode;

	public bool useDelayToResumeActivityAfterOnSpottedDisable;
	public float delayToResumeActivityAfterOnSpottedDisable;

	public bool resetCameraRotationOnStart;

	[Space]
	[Header ("Send Event To Player Detected Settings")]
	[Space]

	public bool addAndRemoveEnemyAroundToDetectedPlayers;

	public string sendCharacterAroundToAddName = "Add Character Around";
	public string sendCharacterAroundToRemoveName = "Remove Character Around";

	public bool sendRemoveEnemyAroundOnDeath;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnSpotted;
	public UnityEvent eventOnSpotted;
	public bool useEventOnNoEnemies;
	public UnityEvent eventOnNoEnemies;

	public bool useEventOnAnyTargetDetected;
	public UnityEvent eventOnAnyTargetDetected;

	[Space]
	[Header ("AI State and Debug")]
	[Space]

	public bool showDebugPrint;

	public bool onSpotted;
	public bool runningAway;

	public bool runningAwayFromPosition;

	public bool AIPaused;

	public bool currentEnemyInfoStored;

	public bool searchingWeapon;

	public bool seeingCurrentTarget;

	public bool lookingAtTargetPosition;

	public bool AIDead;

	public bool targetIsVisible;

	public bool checkingNoiseActive;

	public GameObject targetToAttack;

	public Transform placeToShoot;

	public GameObject partner;
	public bool partnerFound;

	public playerController currentPlayerController;

	public bool returningToPositionActive;
	public bool adjustRotationOnReturnToPositionActive;

	public bool AICanSeeCurrentTarget;
	public bool AICanDetectCurrentTarget;

	public bool checkIfReturnCompleteActive;

	public bool wanderStatePaused;

	public bool wanderStateActive;

	public bool strafeModeActive;
	public bool strafeCurrentlyActive;

	public bool randomWalkPositionActive;

	public bool currentMeleeWeaponAtDistance;

	public bool resetVerticalCameraRotationActive;

	public bool ignoreVisionRangeActive;

	[Space]
	[Header ("Target Type Info")]
	[Space]

	public bool targetIsCharacter;
	public bool targetIsVehicle;
	public bool targetIsOtherType;

	[Space]
	[Header ("AI Vehicle Info")]
	[Space]

	public bool characterOnVehicle;

	public vehicleHUDManager currentVehicleHUDManager;
	public vehicleHUDManager currentVehicleToAttack;
	public GameObject currentVehicle;

	[Space]
	[Header ("AI Behavior State")]
	[Space]

	public bool currentAIBehaviorAssigned;

	public AIBehaviorInfo currentAIBehavior;

	[Space]
	[Header ("Checking Attack State")]
	[Space]

	public float currentDistanceToTarget;
	public float currentPathDistanceToTarget;
	public float distanceDifference;
	public float currentAngleWithTarget;

	public bool attackTargetDirectly;

	public bool canUseAttackActive;
	public bool insideMinDistanceToAttack;
	public bool minRangeToActivateAttackBehaviorActive;

	public bool canAttackOnMinimumAngle;
	public bool checkingIfTargetVisibleResult;

	[Space]
	[Header ("Checking Threat State")]
	[Space]

	public bool checkingThreatVisible;
	public bool checkingThreatNotVisible;
	public bool threatInfoStored;
	public float timeToCheckThreat;
	public bool seeingCurrentThreath;
	public GameObject posibleThreat;
	public bool possibleThreatDetected;

	[Space]
	[Header ("Current Enemies/Targets Detected")]
	[Space]

	public List<GameObject> enemies = new List<GameObject> ();
	public List<GameObject> notEnemies = new List<GameObject> ();

	public List<GameObject> fullEnemyList = new List<GameObject> ();

	public List<enemyInfo> enemyInfoList = new List<enemyInfo> ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.red;
	public float visionRangeGizmoRadius = 2;

	[Space]
	[Header ("Main AI Patrol Component")]
	[Space]

	public AIPatrolSystem AIPatrolManager;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform rayCastPosition;
	public SphereCollider fovTrigger;

	public AIViewTriggerSystem mainAIViewTriggerSystem;
	public checkCollisionType viewTrigger;
	public AINavMesh AINavMeshManager;
	public characterFactionManager factionManager;

	public usingDevicesSystem usingDevicesManager;
	public playerController playerControllerManager;
	public GameObject AIGameObject;

	public enum AIAttackType
	{
		none,
		weapons,
		closeCombat,
		powers,
		melee
	}

	RaycastHit hit;
	Ray newRay;

	float originalFOVRadius;

	float speedMultiplier = 1;
	float timeWithoutThreatFound;

	bool moveBack;

	float currentDistance;

	float originalMinDistanceToDraw;
	float originalMinDistanceToAim;
	float originalMinDistanceToShoot;

	float originalMinDistanceToAimPower;
	float originalMinDistanceToShootPower;

	float originalMinDistanceToCloseCombat;

	float originalMinDistanceToMelee;

	vehicleHUDManager currentVehicleHUDManagerToGetOn;
	characterFactionManager characterFactionToCheck;
	playerController playerControllerPartner;

	GameObject previousEnemy;
	bool previousEnemyAssigned;

	bool onGroundCheck;

	bool hasBeenAttacked;
	Vector3 targetDirection;
	Vector3 targetDirectionAngle;
	bool enemyAlertActive;

	Vector3 currentPosition;
	Vector3 currentEnemyPosition;

	Transform emptyTargetPositionTransform;

	float lastTimeSpotted;

	bool targetIsDriving;

	Transform AITransform;

	Vector3 targetToCheckDirection;
	Transform temporalPlaceToShoot;
	healthManagement temporalHealthManagementToCheck;

	float currentTimeCheckingNoise;

	float currentFovRadius;

	enemyInfo currentEnemyInfo;

	float minimumDistance;
	int currentTargetIndex;

	Transform currentEnemyTransform;

	Coroutine viewTriggerCoroutine;

	bool AIParentChecked;

	Transform AIParent;

	bool checkingIfCanMoveAfterGettingUp;

	Transform currentPositionToReturn;

	float lastTimeWanderPositionCalculated;

	bool waitingToCalculateNextWanderPosition;

	float currentWaitTimeToCalculateNextWanderPosition;

	float lastWanderPositionDistance;

	bool checkingIfWanderPositionReached;

	bool targetDetectionActiveOnPausedAIState;

	GameObject objectToCheck;

	bool checkTargetsOnTriggerPaused;

	float checkTargetsOnTriggerPausedDuration;
	float lastTimeCheckTargetsOnTriggerPaused;

	bool anyTargetToReach;

	bool strafeModeOnControllerActiveState;

	Coroutine regularActivityCoroutine;

	float lastTimeEnemyVisible;

	wanderingInfo currentWanderingInfo;

	bool wanderingInfoAssigned;

	Vector3 Vector3Zero = Vector3.zero;

	Vector3 Vector3Up = Vector3.up;

	float originalRayCastPosition;

	float lastTimePartnerFound;

	bool originalWanderEnabled;

	int currentEnemiesCount;

	GameObject temporalEnemy;
	float longDistance = 100000;

	Vector3 lastPositionBeforeRunningAwayFromPosition;

	float distanceFromRunningAwayPosition;

	float lastTimePaused;
	bool gettingUpActive;


	void Start ()
	{
		if (AIGameObject == null) {
			AIGameObject = gameObject;
		}

		AITransform = AIGameObject.transform;

		originalFOVRadius = fovTrigger.radius;

		originalMinDistanceToDraw = minDistanceToDraw;
		originalMinDistanceToAim = minDistanceToAim;
		originalMinDistanceToShoot = minDistanceToShoot;

		originalMinDistanceToAimPower = minDistanceToAimPower;
		originalMinDistanceToShootPower = minDistanceToShootPower;

		originalMinDistanceToCloseCombat = minDistanceToCloseCombat;

		originalMinDistanceToMelee = minDistanceToMelee;

		originalRayCastPosition = rayCastPosition.localPosition.y;

		AIParent = AITransform.parent;

		GameObject newEmptyTarget = new GameObject ();
		newEmptyTarget.name = "Empty Target Position Transform";
		emptyTargetPositionTransform = newEmptyTarget.transform;
		emptyTargetPositionTransform.SetParent (AIParent);

		if (wanderEnabled && AIPatrolManager != null) {
			if (!AIPatrolManager.isPatrolPaused () && AIPatrolManager.gameObject.activeSelf) {
				wanderEnabled = false;
			}
		}

		originalWanderEnabled = wanderEnabled;
	}

	void Update ()
	{
		checkPartnerState ();

		if (!AIPaused) {
			if (currentAIBehaviorAssigned) {
				currentAIBehavior.updateAI ();
			}

			if (useAbilitiesBehavior) {
				abilitiesAIBehavior.updateAI ();

				abilitiesAIBehavior.updateAIBehaviorState ();
		
				abilitiesAIBehavior.updateAIAttackState (true);
			}

			if (checkTargetsOnTriggerPaused) {
				if (Time.time > lastTimeCheckTargetsOnTriggerPaused + checkTargetsOnTriggerPausedDuration) {
					checkTargetsOnTriggerPaused = false;

					forceFovTriggerToDetectAnythingAround ();
				}
			}

			if (checkingIfCanMoveAfterGettingUp) {
				if (playerControllerManager.canPlayerMove ()) {
					checkingIfCanMoveAfterGettingUp = false;

					if (currentAIBehaviorAssigned) {
						currentAIBehavior.checkIfDrawWeaponsWhenResumingAI ();
					}
				}
			}

			AINavMeshManager.setOnGroundState (playerControllerManager.isPlayerOnGround ());

			if (playerControllerManager.isPlayerOnGround ()) {
				if (!onGroundCheck) {
					onGroundCheck = true;
				}
			} else {
				if (onGroundCheck) {
					onGroundCheck = false;
				}
			}

			if (gettingUpActive && ignoreVisionRangeActive) {
				if (Time.time > lastTimePaused + timeToIgnoreVisionRange) {
					ignoreVisionRangeActive = false;

					gettingUpActive = false;
				}
			}

			if (!checkingIfCanMoveAfterGettingUp) {
				if (!searchingWeapon) {
					closestTarget ();

					if (partnerFound) {
						if (removePartnerIfTooFarFromAI) {
							if (GKC_Utils.distance (AITransform.position, partner.transform.position) > minDistanceToRemovePartner) {
								removePartner ();

								checkIfResumeAfterRemovingPartner ();
							}
						}

						if (removePartnerAfterDelay) {
							if (Time.time > lastTimePartnerFound + delayToRemoveCurrentPartner) {
								removePartner ();

								checkIfResumeAfterRemovingPartner ();

								if (originalWanderEnabled) {
									setWanderEnabledState (true);
								}
							}
						}
					}
				}

				if (attackType != AIAttackType.none) {
					if (!searchingWeapon) {
						checkTargetToAttack ();
					}
				} else {
					if (avoidEnemies) {
						checkTargetToRunAway ();
					}
				}

				if (adjustRotationOnReturnToPositionActive) {
					if (returningToPositionActive) {
						if (!AINavMeshManager.isFollowingTarget () && !AINavMeshManager.anyTargetToReach ()) {
							playerControllerManager.rotateCharacterTowardDirection (currentPositionToReturn, 6, 0.05f, 1);

							returningToPositionActive = false;
						}
					}
				}

				if (!wanderStatePaused && !checkingNoiseActive && !runningAwayFromPosition) {
					checkWanderState ();
				}
			}

			checkAIParent ();

			if (resetVerticalCameraRotationActive) {
				resetRayCastPositionLocalRotation ();

				float angleDifference = Quaternion.Angle (rayCastPosition.localRotation, Quaternion.identity);
	
				if (angleDifference < 0.2f) {
					resetVerticalCameraRotationActive = false;
				}
			}
		} else {
			if (targetDetectionActiveOnPausedAIState) {
				if (!checkingIfCanMoveAfterGettingUp) {
					if (!searchingWeapon) {
						closestTarget ();
					}
				}
			}
		}
	}

	public void checkWanderState ()
	{
		if (wanderEnabled) {
			if (!onSpotted) {
				if (partnerFound) {
					if (wanderStateActive) {

						disableWanderValues ();

						wanderStateActive = false;
					}

					wanderEnabled = false;
				} else {
					if (!waitingToCalculateNextWanderPosition && !AINavMeshManager.isFollowingTarget ()) {
						lastTimeWanderPositionCalculated = Time.time;

						currentWaitTimeToCalculateNextWanderPosition = Random.Range (delaySecondsToWanderToRandomPositionRange.x, delaySecondsToWanderToRandomPositionRange.y);

						waitingToCalculateNextWanderPosition = true;

						wanderStateActive = true;

						if (checkingIfWanderPositionReached) {
							if (useEventOnWanderPositionReached) {
								eventOnWanderPositionReached.Invoke ();
							}

							if (useEventsOnWanderingInfoListOnPositionReached) {
								if (!wanderingInfoAssigned) {
									for (int i = 0; i < wanderingInfoList.Count; i++) {
										if (wanderingInfoList [i].isCurrentState) {
											currentWanderingInfo = wanderingInfoList [i];

											wanderingInfoAssigned = true;
										}
									}
								} 

								if (wanderingInfoAssigned) {
									currentWanderingInfo.eventOnWanderPositionReached.Invoke ();
								}
							}
						}
					} else {
						if (waitingToCalculateNextWanderPosition) {
							if (Time.time > lastTimeWanderPositionCalculated + currentWaitTimeToCalculateNextWanderPosition) {
								bool wanderTargetCalculated = false;

								int destinationCheckCounter = 0;

								while (!wanderTargetCalculated) {
									float currentRangeToNextWonderPosition = Random.Range (wanderPositionRange.x, wanderPositionRange.y);

									//								print ("new range " + currentRangeToNextWonderPosition);

									Vector2 circlePosition = Random.insideUnitCircle * currentRangeToNextWonderPosition;

									Vector3 newCirclePosition = new Vector3 (circlePosition.x, 0, circlePosition.y);

									Vector3 wonderTargetPosition = AITransform.position + newCirclePosition;

									if (useMaxDistanceToWanderFromInitialPosition) {
										newCirclePosition = Vector3.ClampMagnitude (newCirclePosition, maxDistanceToWanderFromInitialPosition);

										wonderTargetPosition = AIParent.position + newCirclePosition;
									} 

									wonderTargetPosition += Vector3Up * 2;

									bool ignoreRaycastCheck = false;

									if (wanderAlwaysFartherFromInitialPosition) {
										float currentDistanceFromInitialPosition = GKC_Utils.distance (AITransform.position, AIParent.position);

										if (currentDistanceFromInitialPosition < lastWanderPositionDistance) {
											ignoreRaycastCheck = true;
											//										print ("distance too short, calculating again");
										}
									}

									if (!ignoreRaycastCheck) {
										if (Physics.Raycast (wonderTargetPosition, -Vector3Up, out hit, longDistance, layerMaskToRaycastToWander)) {
											if (hit.rigidbody != null) {
												Physics.Raycast (hit.point, -Vector3Up, out hit, longDistance, layerMaskToRaycastToWander);

											}

											if (AINavMeshManager.updateCurrentNavMeshPath (hit.point)) {

												wanderTargetCalculated = true;

												waitingToCalculateNextWanderPosition = false;

												lastWanderPositionDistance = GKC_Utils.distance (hit.point, AIParent.position);

												//											print ("position to wander found, moving with distance " + (GKC_Utils.distance (wonderTargetPosition, AIParent.position)));

												emptyTargetPositionTransform.position = hit.point + hit.normal * 0.3f;

												AINavMeshManager.setTarget (emptyTargetPositionTransform);

												AINavMeshManager.setTargetType (false, true);

												AINavMeshManager.setPatrolState (false);

												if (useCustomMovementSpeedOnWonder) {
													float newMovementSpeed = movementSpeedOnWander;

													if (useRandomMovementSpeedOnWander) {
														newMovementSpeed = Random.Range (randomRangeMovementSpeedOnWander.x, randomRangeMovementSpeedOnWander.y);
													}

													AINavMeshManager.enableCustomNavMeshSpeed (newMovementSpeed);
												} else {
													AINavMeshManager.disableCustomNavMeshSpeed ();
												}

												checkingIfWanderPositionReached = true;
											}
										}
									}

									destinationCheckCounter++;

									if (destinationCheckCounter >= 100) {
										//									print ("too much destination checks, stopping the check");

										wanderTargetCalculated = true;

										waitingToCalculateNextWanderPosition = false;

									}
								}
							}
						}
					}
				}
			} else {
				if (wanderStateActive) {

					disableWanderValues ();

					wanderStateActive = false;
				}
			}
		}
	}

	public void disableWanderValues ()
	{
		waitingToCalculateNextWanderPosition = false;

		checkingIfWanderPositionReached = false;

		wanderStateActive = false;

		lastTimeWanderPositionCalculated = Time.time;

		if (useCustomMovementSpeedOnWonder) {
			AINavMeshManager.disableCustomNavMeshSpeed ();
		}
	}

	public void checkIfInterruptWanderState ()
	{
		if (wanderEnabled) {
			if (wanderStateActive) {

				disableWanderValues ();

				wanderStateActive = false;
			}

			checkIfUseEventsOnInterruptWanderState ();
		}
	}

	public void disableWanderState ()
	{
		if (wanderEnabled) {
			checkIfInterruptWanderState ();

			wanderEnabled = false;
		}
	}

	public void setWanderEnabledState (bool state)
	{
		wanderEnabled = state;
	}

	public void checkIfUseEventsOnInterruptWanderState ()
	{
		if (useEventsOnInterruptWanderState) {
			eventsOnInterruptWanderState.Invoke ();
		}
	}

	public void setWanderStatePaused (bool state)
	{
		wanderStatePaused = state;

		print (state);
	}

	public void setWanderingInfoState (string nameToSearch)
	{
		for (int i = 0; i < wanderingInfoList.Count; i++) {
			if (wanderingInfoList [i].Name.Equals (nameToSearch)) {
				currentWanderingInfo = wanderingInfoList [i];

				wanderingInfoList [i].isCurrentState = true;

				wanderingInfoAssigned = true;
			} else {
				wanderingInfoList [i].isCurrentState = false;
			}
		}
	}

	public void checkTargetToAttack ()
	{
		if (onSpotted) {

			if (!lookingAtTargetPosition && !randomWalkPositionActive) {
				lootAtDirection (AINavMeshManager.getCurrentMovementDirection ());
			}

			if (targetToAttack != null) {

				currentPathDistanceToTarget = AINavMeshManager.getRemainingDistanceToTarget ();

				currentDistanceToTarget = GKC_Utils.distance (targetToAttack.transform.position, AITransform.position);

				if (currentPathDistanceToTarget < 0) {
					currentPathDistanceToTarget = currentDistanceToTarget;
				}

				attackTargetDirectly = false;

				targetIsVisible = false;

				if (attackType == AIAttackType.weapons || attackType == AIAttackType.powers) {
					distanceDifference = Mathf.Abs (currentDistanceToTarget - currentPathDistanceToTarget);

					bool checkAttackOnPlace = false;

					if (attackAlwaysOnPlace) {
						checkAttackOnPlace = true;
					}

					if (!checkAttackOnPlace) {
						if ((currentDistanceToTarget < currentPathDistanceToTarget && distanceDifference > minDistanceToAttackOnPlace &&
						    distanceDifference < maxDistanceToAttackOnPlace) ||
						    !AINavMeshManager.getCanReachCurrentTargetValue ()) {
							checkAttackOnPlace = true;
						}
					}

					if (checkAttackOnPlace) {
						if (attackAlwaysOnPlace) {
							targetIsVisible = checkIfTargetVisible (maxRaycastDistanceToAlwaysAttackOnPlace);
						} else {
							targetIsVisible = checkIfTargetVisible (maxRaycastDistanceToAttackOnPlace);
						}

						if (targetIsVisible) {
							AINavMeshManager.setPauseMoveNavMeshState (true);

							attackTargetDirectly = true;

							setAttackMode (attackTargetDirectly, true);
						} else {
							AINavMeshManager.setPauseMoveNavMeshState (false);
						}
					} else {
						AINavMeshManager.setPauseMoveNavMeshState (false);
					}
				}

				if (targetIsVisible || checkIfTargetVisible (currentFovRadius)) {
					if (currentPlayerController != null) {
						AICanDetectCurrentTarget = !currentPlayerController.isCharacterInStealthMode () || canSeeTargetsOnStealthMode;

						if (!AICanDetectCurrentTarget) {
							GameObject currentTarget = targetToAttack;

							enemyLost (targetToAttack);

							if (enemies.Count == 0) {
								checkPossibleThreatValues (currentTarget);
							}

							return;
						}
					}
				}

				if (!attackTargetDirectly) {
					if (currentPathDistanceToTarget < minDistanceToLookAtTarget) {
						//print ("attack on close distance");
						if (targetIsVisible || checkIfTargetVisible (0)) {
							setAttackMode (false, true);
						} else {
							if (onSpotted) {
								setAttackMode (false, false);
							}
						}
					} else {
						if (currentAIBehaviorAssigned) {
							currentAIBehavior.stopAim ();

							if (insideMinDistanceToAttack) {
								resetBehaviorStates ();

								insideMinDistanceToAttack = false;
							}
						}
							
						lookingAtTargetPosition = false;
					}
				}

				checkStrafeMode (true);
			}
		}

		//if the turret detects a target, it will check if it is an enemy, and this will take 2 seconds, while the enemy choose to leave or stay in the place
		else if (checkingThreatVisible || checkingThreatNotVisible) {
			checkCurrentThreat ();
		} else {
			//NEW PARTS FOR THE STRAFE MODE ON THE AI, NOT FINISHED YET

			anyTargetToReach = AINavMeshManager.anyTargetToReach ();

			if (!anyTargetToReach) {
				if (!checkingThreatVisible && !checkingThreatNotVisible) {
					if (checkingNoiseActive) {
						currentTimeCheckingNoise += Time.deltaTime;

						if (timeToCheckNoise < currentTimeCheckingNoise) {
							checkIfResumeAfterCheckingNoise ();
						}
					} else {
						if (returnToPositionAfterNoEnemiesFound && Time.time > lastTimeSpotted + 1 && checkIfReturnCompleteActive) {
							checkIfResumeAfterNoEnemiesFound ();
						}
					}
				}
			}

			checkStrafeMode (false);
		}
	}

	void checkStrafeMode (bool state)
	{
		if (AINavMeshManager.anyTargetToReach ()) {
			if (strafeModeActive) {
				if (playerControllerManager.isStrafeModeActive () && AINavMeshManager.isFollowingTarget ()) {
					if (state || randomWalkPositionActive) {
						lootAtTarget (placeToShoot);
					} else {
						Vector3 currentLookDirection = AINavMeshManager.getCurrentMovementDirection ();

						lootAtDirection (currentLookDirection);
					}

					if (!strafeCurrentlyActive) {
						AINavMeshManager.lookAtTaget (true);

						AINavMeshManager.setStrafeModeActiveState (true);

						strafeCurrentlyActive = true;
					}
				} else {
					if (state) {
						if (!strafeCurrentlyActive) {
							if (AINavMeshManager.isFollowingTarget () && !playerControllerManager.isStrafeModeActive ()) {
								playerControllerManager.activateOrDeactivateStrafeMode (true);
							}
						}
					}
				}

				if (strafeCurrentlyActive) {
					if (state) {
						if (randomWalkPositionActive) {
							if (!strafeModeOnControllerActiveState) {
								playerControllerManager.activateOrDeactivateStrafeMode (true);

								strafeModeOnControllerActiveState = true;
							}
						} else {
							float distanceToTarget = AINavMeshManager.getRemainingDistanceToTarget ();

							if (distanceToTarget < minDistanceToTargetToUseStrafeMode) {
								if (!strafeModeOnControllerActiveState) {
									playerControllerManager.activateOrDeactivateStrafeMode (true);

									strafeModeOnControllerActiveState = true;
								}
							} else {
								if (strafeModeOnControllerActiveState) {
									playerControllerManager.activateOrDeactivateStrafeMode (false);

									strafeModeOnControllerActiveState = false;
								}
							}
						}
					} else {
						if (partnerFound) {
							if (strafeModeOnControllerActiveState) {
								playerControllerManager.activateOrDeactivateStrafeMode (false);

								AINavMeshManager.lookAtTaget (false);

								AINavMeshManager.setStrafeModeActiveState (false);

								strafeCurrentlyActive = false;

								strafeModeOnControllerActiveState = false;
							}
						} else {
							if (!strafeModeOnControllerActiveState) {
								playerControllerManager.activateOrDeactivateStrafeMode (true);

								strafeModeOnControllerActiveState = true;
							}
						}
					}
				}
			} else {
				if (strafeCurrentlyActive) {
					AINavMeshManager.setStrafeModeActiveState (false);

					strafeCurrentlyActive = false;

					strafeModeOnControllerActiveState = false;
				}
			}
		} else {
			if (strafeModeActive) {
				if (playerControllerManager.isStrafeModeActive ()) {
					if (strafeCurrentlyActive) {
						AINavMeshManager.lookAtTaget (false);

						strafeCurrentlyActive = false;

						emptyTargetPositionTransform.eulerAngles = AITransform.eulerAngles;
					}

					lootAtDirection (emptyTargetPositionTransform.forward);

					//					print ("disable strafe");
				}
			} else {
				if (strafeCurrentlyActive) {
					AINavMeshManager.lookAtTaget (false);

					AINavMeshManager.setStrafeModeActiveState (false);

					strafeCurrentlyActive = false;

					strafeModeOnControllerActiveState = false;
				}
			}
		}
	}

	public void setStrafeModeActive (bool state)
	{
		strafeModeActive = state;
	}

	public void updateStrafeModeActiveState ()
	{

	}

	public void setRandomWalkPositionState (float newRandomWalkRadius)
	{
		randomWalkPositionActive = newRandomWalkRadius != 0;

		AINavMeshManager.setCalculateMoveValueWithDirectionToTargetActiveState (randomWalkPositionActive);

		if (randomWalkPositionActive) {
			bool randomWalkPositionFound = false;

			int destinationCheckCounter = 0;

			while (!randomWalkPositionFound) {
				Vector2 circlePosition = Random.insideUnitCircle * newRandomWalkRadius;

				Vector3 newCirclePosition = new Vector3 (circlePosition.x, 0, circlePosition.y);

				Vector3 wonderTargetPosition = AITransform.position + newCirclePosition;

				wonderTargetPosition += Vector3Up * 2;

				bool ignoreCheckRaycast = false;

				float dot = Vector3.Dot (AITransform.forward, (wonderTargetPosition - AITransform.position).normalized);

				if (dot > 0) {
					ignoreCheckRaycast = true;
				}

				if (!ignoreCheckRaycast) {
					if (Physics.Raycast (wonderTargetPosition, -Vector3Up, out hit, longDistance, layerMaskToRaycastToWander)) {
						if (AINavMeshManager.updateCurrentNavMeshPath (hit.point)) {

							randomWalkPositionFound = true;

							emptyTargetPositionTransform.position = hit.point + hit.normal * 0.3f;

							AINavMeshManager.setTarget (emptyTargetPositionTransform);
						}
					}
				}

				destinationCheckCounter++;

				if (destinationCheckCounter >= 100) {
					randomWalkPositionFound = true;
				}
			}
		} else {
			if (onSpotted) {
				if (targetToAttack != null) {
					AINavMeshManager.setTarget (targetToAttack.transform);
				}
			}
		}
	}

	public void checkCurrentThreat ()
	{
		if (possibleThreatDetected && posibleThreat != null) {
			if (placeToShoot == null) {
				//every object with a health component, has a place to be shoot, to avoid that a enemy shoots the player in his foot, so to center the shoot
				//it is used the gameObject placetoshoot in the health script
				if (applyDamage.checkIfDead (posibleThreat)) {
					cancelCheckPossibleThreatValues (posibleThreat);

					return;
				} else {
					placeToShoot = applyDamage.getPlaceToShoot (posibleThreat);
				}
			}

			if (checkingThreatVisible) {
				if (!threatInfoStored) {
					currentPlayerController = posibleThreat.GetComponent<playerController> ();

					if (currentPlayerController != null) {
						AICanSeeCurrentTarget = currentPlayerController.isCharacterVisibleToAI () || canSeeHiddenTargets || canSeeTargetsOnStealthMode;

						if (AICanSeeCurrentTarget) {
							playerControllerManager.setCharacterStateIcon (wonderingCharacterStateName);
						} else {
							AICanDetectCurrentTarget = !currentPlayerController.isCharacterInStealthMode () || canSeeTargetsOnStealthMode;

							if (!AICanDetectCurrentTarget) {
								checkingThreatVisible = false;
								threatInfoStored = false;
								checkingThreatNotVisible = true;

								return;
							}
						}
					} else {
						playerControllerManager.setCharacterStateIcon (wonderingCharacterStateName);
					}

					//check if the current threat is hidden or not, to pause the patrol
					if (AIPatrolManager != null && !wanderEnabled) {
						if (!AIPatrolManager.isPatrolPaused () && !AINavMeshManager.isPatrolPaused ()) {
							AICanSeeCurrentTarget = (currentPlayerController == null);

							if (!AICanSeeCurrentTarget) {
								AICanSeeCurrentTarget = currentPlayerController.isCharacterVisibleToAI () || canSeeHiddenTargets || canSeeTargetsOnStealthMode;
							}

							if (AICanSeeCurrentTarget) {
								AINavMeshManager.setPatrolPauseState (true);
							}
						}
					} else {
						if (pauseMovementWhenCheckingThreats) {
							AICanSeeCurrentTarget = (currentPlayerController == null);

							if (!AICanSeeCurrentTarget) {
								AICanSeeCurrentTarget = currentPlayerController.isCharacterVisibleToAI () || canSeeHiddenTargets || canSeeTargetsOnStealthMode;
							}

							if (AICanSeeCurrentTarget) {
								AINavMeshManager.setPauseMoveNavMeshState (true);

								if (wanderEnabled) {
									checkIfInterruptWanderState ();
								}
							}
						}
					}

					threatInfoStored = true;
				} 

				if (placeToShoot != null) {
					//look at the target position
					lootAtTarget (placeToShoot);
				}
			} else {
				if (checkingThreatNotVisible) {
					if (placeToShoot != null) {
						//look at the target position
						lootAtTarget (placeToShoot);
					}
				}
			}

			seeingCurrentThreath = false;

			//uses a raycast to check the posible threat
			if (Physics.Raycast (rayCastPosition.position, rayCastPosition.forward, out hit, longDistance, layerMask)) {
				if (hit.collider.gameObject == posibleThreat || hit.collider.gameObject.transform.IsChildOf (posibleThreat.transform)) {

					if (checkingThreatVisible) {
						AICanSeeCurrentTarget = (currentPlayerController == null);

						if (!AICanSeeCurrentTarget) {
							AICanSeeCurrentTarget = currentPlayerController.isCharacterVisibleToAI () || canSeeHiddenTargets || canSeeTargetsOnStealthMode;
						}

						if (AICanSeeCurrentTarget) {
							timeToCheckThreat += Time.deltaTime * speedMultiplier;
						}
					}

					if (checkingThreatNotVisible) {
						checkingThreatVisible = true;
					}

					seeingCurrentThreath = true;
				} else {
					if (checkingThreatVisible) {
						timeWithoutThreatFound += Time.deltaTime * speedMultiplier;
					}
				}

				//when the turret look at the target for a while, it will open fire 
				if (timeToCheckThreat > timeToCheckSuspect) {
					timeToCheckThreat = 0;

					checkingThreatNotVisible = false;

					threatInfoStored = false;

					addEnemy (posibleThreat);

					posibleThreat = null;

					possibleThreatDetected = false;

					AINavMeshManager.setPatrolPauseState (true);

					playerControllerManager.disableCharacterStateIcon ();

					resetCheckingNoiseState ();

					if (pauseMovementWhenCheckingThreats) {
						AINavMeshManager.setPauseMoveNavMeshState (false);
					}
				}

				if (checkingThreatVisible) {
					if (timeWithoutThreatFound > timeToCheckSuspect) {
						resetCheckThreatValues ();
					}
				}
			}

			//check if the target enters or exits on stealth mode
			if (!checkingThreatNotVisible) {
				if (currentPlayerController != null) {
					AICanDetectCurrentTarget = !currentPlayerController.isCharacterInStealthMode () || canSeeTargetsOnStealthMode;

					if (!AICanDetectCurrentTarget) {
						checkingThreatNotVisible = true;	
						checkingThreatVisible = false;
						threatInfoStored = false;

						playerControllerManager.disableCharacterStateIcon ();

						print ("target lost after enter on stealth mode");
					}
				}
			} else {
				if (currentPlayerController != null) {
					AICanDetectCurrentTarget = !currentPlayerController.isCharacterInStealthMode () || canSeeTargetsOnStealthMode;

					if (AICanDetectCurrentTarget) {

						resetCheckThreatValues ();

						print ("target found again after being on stealth mode");
					}
				}
			}
		}
	}

	public void checkTargetToRunAway ()
	{
		if (onSpotted) {

			if (!lookingAtTargetPosition) {
				lootAtDirection (AINavMeshManager.getCurrentMovementDirection ());
			}

			if (targetToAttack != null) {
				targetIsVisible = checkIfTargetVisible (currentFovRadius);

				if (targetIsVisible) {
					if (currentPlayerController != null) {
						AICanDetectCurrentTarget = !currentPlayerController.isCharacterInStealthMode () || canSeeTargetsOnStealthMode;

						if (!AICanDetectCurrentTarget) {
							GameObject currentTarget = targetToAttack;

							enemyLost (targetToAttack);

							if (enemies.Count == 0) {
								checkPossibleThreatValues (currentTarget);
							}

							return;
						}
					}
				}
			}
		}

		//if the turret detects a target, it will check if it is an enemy, and this will take 2 seconds, while the enemy choose to leave or stay in the place
		else if (checkingThreatVisible || checkingThreatNotVisible) {
			checkCurrentThreat ();
		}
	}

	public bool checkIfTargetVisible (float raycastDistance)
	{
		Vector3 raycastDirection = placeToShoot.position - rayCastPosition.position;

		if (raycastDistance == 0) {
			raycastDistance = longDistance;
		}

		if (Physics.Raycast (rayCastPosition.position, raycastDirection, out hit, raycastDistance, layerMask)) {
			if (hit.collider.gameObject == targetToAttack || hit.collider.gameObject.transform.IsChildOf (targetToAttack.transform)) {
				checkingIfTargetVisibleResult = true;

				Debug.DrawLine (rayCastPosition.position, hit.collider.transform.position, Color.green);

				return true;
			}
		} else {
			Debug.DrawRay (rayCastPosition.position, rayCastPosition.forward * 3, Color.black);
		}

		checkingIfTargetVisibleResult = false;

		return false;
	}

	public bool checkIfMinimumAngleToAttack ()
	{
		if (targetToAttack != null) {
			if (!useMinAngleToTargetToAttack || targetIsVehicle) {
				return true;
			}

			targetDirectionAngle = targetToAttack.transform.position - AITransform.position;

			targetDirectionAngle -= AITransform.up * AITransform.InverseTransformDirection (targetDirectionAngle).y;

			currentAngleWithTarget = Vector3.SignedAngle (AITransform.forward, targetDirectionAngle, AITransform.up);

			if (Mathf.Abs (currentAngleWithTarget) < minAngleToTargetToAttack / 2) { 
				canAttackOnMinimumAngle = true;

				return true;
			}
		}

		canAttackOnMinimumAngle = false;

		return false;
	}

	public void resetCheckThreatValues ()
	{
		placeToShoot = null;

		posibleThreat = null;

		possibleThreatDetected = false;

		checkingThreatVisible = false;
		checkingThreatNotVisible = false;

		threatInfoStored = false;
		timeToCheckThreat = 0;
		timeWithoutThreatFound = 0;

		AINavMeshManager.setPatrolPauseState (false);

		if (pauseMovementWhenCheckingThreats) {
			AINavMeshManager.setPauseMoveNavMeshState (false);
		}

		playerControllerManager.disableCharacterStateIcon ();
	}

	public void setLookingAtTargetPositionState (bool state)
	{
		lookingAtTargetPosition = state;
	}

	public bool isLookingAtTargetPosition ()
	{
		return lookingAtTargetPosition;
	}

	//follow the enemy position, to rotate torwards his direction
	void lootAtTarget (Transform objective)
	{
		if (objective != null) {
			if (showGizmo) {
				Debug.DrawRay (rayCastPosition.position, rayCastPosition.forward * 2, Color.yellow);
			}
				
			Vector3 targetDir = objective.position - rayCastPosition.position;
			Quaternion targetRotation = Quaternion.LookRotation (targetDir, AITransform.up);

			rayCastPosition.rotation = Quaternion.Slerp (rayCastPosition.rotation, targetRotation, lookAtTargetSpeed * Time.deltaTime);
		}
	}

	public void lookAtCurrentPlaceToShoot ()
	{
		lootAtTarget (placeToShoot);
	}

	void lootAtDirection (Vector3 direction)
	{
		if (direction != Vector3Zero) {
			if (showGizmo) {
				Debug.DrawRay (rayCastPosition.position, rayCastPosition.forward * 2, Color.green);
			}

			Quaternion targetRotation = Quaternion.LookRotation (direction, AITransform.up);

			rayCastPosition.rotation = Quaternion.Slerp (rayCastPosition.rotation, targetRotation, lookAtTargetSpeed * Time.deltaTime);
		}
	}

	void resetLookAtDirection ()
	{
		rayCastPosition.rotation = Quaternion.Slerp (rayCastPosition.rotation, Quaternion.identity, lookAtTargetSpeed * Time.deltaTime);
	}

	void resetRayCastPositionLocalRotation ()
	{
		rayCastPosition.localRotation = Quaternion.Slerp (rayCastPosition.localRotation, Quaternion.identity, lookAtTargetSpeed * Time.deltaTime);
	}

	public void setRayCastPositionOffsetValue (float offsetValue)
	{
		rayCastPosition.localPosition = Vector3.up * offsetValue;
	}

	public void setOriginalRayCastPositionOffsetValue ()
	{
		setRayCastPositionOffsetValue (originalRayCastPosition);
	}

	public bool checkCharacterFaction (GameObject character, bool damageReceived)
	{
		if (character == null) {
			return false;
		}

		//print (gameObject.name + " "+ character.name + " " + damageReceived);
		if (fullEnemyList.Contains (character)) {
			return true;
		}

		characterFactionToCheck = character.GetComponent<characterFactionManager> ();

		if (characterFactionToCheck != null) {
			bool isEnemy = false;

			if (damageReceived) {
				isEnemy = factionManager.isAttackerEnemy (characterFactionToCheck.getFactionName ());
			} else {
				isEnemy = factionManager.isCharacterEnemy (characterFactionToCheck.getFactionName ());
			}

			if (isEnemy) {
				return true;
			}
		} else {
			currentVehicleHUDManager = applyDamage.getVehicleHUDManager (character);

			if (currentVehicleHUDManager != null) {
				if (currentVehicleHUDManager.isVehicleBeingDriven ()) {
					GameObject currentDriver = currentVehicleHUDManager.getCurrentDriver ();

					//print (currentDriver.name);
					if (currentDriver == null) {
						return false;
					}

					characterFactionToCheck = currentDriver.GetComponent<characterFactionManager> ();

					if (characterFactionToCheck != null) {
						bool isEnemy = false;

						if (damageReceived) {
							isEnemy = factionManager.isAttackerEnemy (characterFactionToCheck.getFactionName ());
						} else {
							isEnemy = factionManager.isCharacterEnemy (characterFactionToCheck.getFactionName ());
						}

						if (isEnemy) {
							addEnemy (currentDriver);

							targetIsDriving = true;

							return true;
						}
					} 
				}
			}
		}

		return false;
	}

	//check if the object which has collided with the viewTrigger (the capsule collider in the head of the turret) is an enemy checking the tag of that object
	public void checkSuspect (GameObject currentSuspect)
	{
		if (!checkThreatsEnabled) {
			return;
		}

		checkPossibleThreatValues (currentSuspect);
	}

	void checkPossibleThreatValues (GameObject currentSuspect)
	{
		if (currentSuspect == null) {
			return;
		}

		if (canCheckSuspect (currentSuspect.layer)) {
			if (!onSpotted && posibleThreat == null && checkCharacterFaction (currentSuspect, false)) {
				if (targetIsDriving || applyDamage.isVehicle (currentSuspect)) {
					targetIsDriving = false;
					targetIsVehicle = true;
				}

				temporalPlaceToShoot = null;
				temporalHealthManagementToCheck = null;

				if (!checkRaycastToViewTarget || checkIfTargetIsPhysicallyVisible (currentSuspect, true)) {
					posibleThreat = currentSuspect;
					checkingThreatVisible = true;
				} else {
					posibleThreat = currentSuspect;
					checkingThreatNotVisible = true;
				}

				possibleThreatDetected = posibleThreat != null;

				currentTimeCheckingNoise = 0;

				if (showDebugPrint) {
					print ("checking possible threat " + posibleThreat.name);

					print (checkingThreatVisible + "  " + checkingThreatNotVisible);

					print ("\n \n \n");
				}
			}
		}
	}

	public void cancelCheckSuspect (GameObject currentSuspect)
	{
		if (!checkThreatsEnabled) {
			return;
		}

		cancelCheckPossibleThreatValues (currentSuspect);
	}

	void cancelCheckPossibleThreatValues (GameObject currentSuspect)
	{
		if (!onSpotted && possibleThreatDetected && canCheckSuspect (currentSuspect.layer) && checkCharacterFaction (currentSuspect, false)) {
			resetCheckThreatValues ();
		}
	}

	//the sphere collider with the trigger of the turret has detected an enemy, so it is added to the list of enemies
	void enemyDetected (GameObject col)
	{
		if (checkCharacterFaction (col, false)) {
			addEnemy (col.gameObject);
		}
	}

	//one of the enemies has left, so it is removed from the enemies list
	void enemyLost (GameObject enemyToRemove)
	{
		//if (onSpotted) {
		removeEnemy (enemyToRemove);
		//}
	}

	void enemyAlert (GameObject target)
	{
		if (onSpotted) {
			return;
		}

		//		print ("giving alert of target " + target.name + " to AI " + AITransform.name);

		enemyDetected (target);

		enemyAlertActive = true;
	}

	//if anyone shoot this character, increase its field of view to search any enemy close to it
	public void checkShootOrigin (GameObject attacker)
	{
		if (!onSpotted) {

			if (!enabled) {
				return;
			}

			if (useMaxDistanceRangeToDetectTargets) {
				if (GKC_Utils.distance (attacker.transform.position, AITransform.position) > maxDistanceRangeToDetectTargets) {
					if (showDebugPrint) {
						print ("attacker too far to be detected");
					}

					return;
				}
			}

			if (checkCharacterFaction (attacker, true)) {
				addEnemy (attacker);

				factionManager.addDetectedEnemyFromFaction (attacker);

				hasBeenAttacked = true;
			}
		}
	}

	//add an enemy to the list, checking that that enemy is not already in the list
	void addEnemy (GameObject enemyToAdd)
	{
		if (!enemies.Contains (enemyToAdd)) {

			if (enemies.Count == 0) {
				if (useEventOnAnyTargetDetected) {
					eventOnAnyTargetDetected.Invoke ();
				}
			}

			GameObject detectedTarget = applyDamage.getCharacterOrVehicle (enemyToAdd);

			if (detectedTarget != null) {
				if (enemies.Contains (detectedTarget)) {

					int enemyIndex = enemyInfoList.FindIndex (s => s.enemyGameObject == detectedTarget);

					if (enemyIndex > -1) {
						enemyInfoList [enemyIndex].vehicleParts.Add (enemyToAdd);
					}

					return;
				}
			} else {
				detectedTarget = enemyToAdd;
			}

			//			print ("adding " + detectedTarget.name + " to character " + name);

			enemies.Add (detectedTarget);

			enemyInfo newEnemyInfo = new enemyInfo ();

			newEnemyInfo.enemyGameObject = detectedTarget;
			newEnemyInfo.enemyTransform = newEnemyInfo.enemyGameObject.transform;

			newEnemyInfo.Name = newEnemyInfo.enemyTransform.name;

			newEnemyInfo.enemeyHealthManagement = applyDamage.getHealthManagement (newEnemyInfo.enemyGameObject);
			newEnemyInfo.placeToShoot = applyDamage.getPlaceToShoot (newEnemyInfo.enemyGameObject);
			newEnemyInfo.isCharacter = applyDamage.isCharacter (newEnemyInfo.enemyGameObject);
			newEnemyInfo.isVehicle = applyDamage.isVehicle (newEnemyInfo.enemyGameObject);
			newEnemyInfo.isOtherType = (!newEnemyInfo.isCharacter && newEnemyInfo.isVehicle);

			newEnemyInfo.lastTimeEnemyVisible = Time.time;

			if (newEnemyInfo.isVehicle) {
				newEnemyInfo.vehicleParts.Add (enemyToAdd);
			}

			if (newEnemyInfo.isCharacter) {
				newEnemyInfo.enemyPlayerController = applyDamage.getPlayerControllerComponent (newEnemyInfo.enemyGameObject);

				if (addAndRemoveEnemyAroundToDetectedPlayers) {
					if (newEnemyInfo.enemyPlayerController != null) {
						applyDamage.sendCharacterAroundToAdd (newEnemyInfo.enemyGameObject, AITransform, sendCharacterAroundToAddName);
					}
				}

				if (newEnemyInfo.enemyPlayerController != null) {
					newEnemyInfo.characterRadius = newEnemyInfo.enemyPlayerController.getCharacterRadius ();
				}
			}

			enemyInfoList.Add (newEnemyInfo);

			if (!fullEnemyList.Contains (detectedTarget)) {
				fullEnemyList.Add (detectedTarget);
			}

			if (partnerFound && partner != null) {
				if (fullEnemyList.Contains (partner)) {

					removePartner ();

					getOffFromVehicle ();
				}
			}
		}
	}

	//remove an enemy from the list
	void removeEnemy (GameObject enemyToRemove)
	{
		int enemyIndex = enemyInfoList.FindIndex (s => s.enemyGameObject == enemyToRemove);

		//		print ("removing target " + enemyToRemove.name + " from character " + name + " " + enemyIndex);

		if (enemyIndex > -1) {
			//remove this enemy from the faction system detected enemies for the faction of this character
			factionManager.removeDetectedEnemyFromFaction (enemyToRemove);

			if (addAndRemoveEnemyAroundToDetectedPlayers) {
				if (enemyInfoList [enemyIndex].isCharacter) {
					if (enemyInfoList [enemyIndex].enemyPlayerController != null) {
					
						applyDamage.sendCharacterAroundToRemove (enemyInfoList [enemyIndex].enemyGameObject, AITransform, sendCharacterAroundToRemoveName);
					}
				}
			}

			enemies.Remove (enemyToRemove);
			enemyInfoList.RemoveAt (enemyIndex);
		} else {
			GameObject detectedTarget = applyDamage.getCharacterOrVehicle (enemyToRemove);

			if (detectedTarget != null) {
				enemyIndex = enemyInfoList.FindIndex (s => s.enemyGameObject == detectedTarget);

				if (enemyIndex > -1) {
					enemyInfo enemyInfoToCheck = enemyInfoList [enemyIndex];

					if (enemyInfoToCheck.isVehicle) {
						if (enemyInfoToCheck.vehicleParts.Contains (enemyToRemove)) {
							enemyInfoToCheck.vehicleParts.Remove (enemyToRemove);

							if (enemyInfoToCheck.vehicleParts.Count == 0) {
								enemyToRemove = enemyInfoToCheck.enemyGameObject;

								factionManager.removeDetectedEnemyFromFaction (enemyToRemove);

								enemies.Remove (enemyToRemove);
								enemyInfoList.RemoveAt (enemyIndex);
							}
						}
					} else {
						if (addAndRemoveEnemyAroundToDetectedPlayers) {
							if (enemyInfoToCheck.isCharacter) {
								if (enemyInfoToCheck.enemyPlayerController != null) {
								
									applyDamage.sendCharacterAroundToRemove (enemyInfoToCheck.enemyGameObject, AITransform, sendCharacterAroundToRemoveName);
								}
							}
						} 
					}
				}
			}
		}
	}

	void addNotEnemey (GameObject notEnemy)
	{
		if (!notEnemies.Contains (notEnemy)) {
			characterFactionToCheck = notEnemy.GetComponent<characterFactionManager> ();

			if (characterFactionToCheck != null) {
				notEnemies.Add (notEnemy);
			}
		}
	}

	void removeNotEnemy (GameObject notEnemy)
	{
		if (notEnemies.Contains (notEnemy)) {
			notEnemies.Remove (notEnemy);
		}
	}

	//when there is one enemy or more, check which is the closest to shoot it.
	void closestTarget ()
	{
		currentEnemiesCount = enemies.Count;

		if (currentEnemiesCount > 0) {
			currentPosition = AITransform.position;

			minimumDistance = longDistance;
			currentTargetIndex = -1;

			for (int i = 0; i < currentEnemiesCount; i++) {
				temporalEnemy = enemies [i];

				if (temporalEnemy != null) {
					currentDistance = GKC_Utils.distance (temporalEnemy.transform.position, currentPosition);

					if (currentDistance < minimumDistance) {
						minimumDistance = currentDistance;
						currentTargetIndex = i;
					}
				} else {
					enemies.RemoveAt (i);
					enemyInfoList.RemoveAt (i);

					i = 0;

					currentEnemiesCount = enemies.Count;
				}
			}

			if (currentTargetIndex < 0) {
				return;
			}

			targetToAttack = enemies [currentTargetIndex];

			if (previousEnemy != targetToAttack) {
				previousEnemy = targetToAttack;

				currentEnemyInfoStored = false;

				lastTimeEnemyVisible = enemyInfoList [currentTargetIndex].lastTimeEnemyVisible;

				previousEnemyAssigned = previousEnemy != null;
			}

			if (!currentEnemyInfoStored || placeToShoot == null) {
				currentEnemyInfo = enemyInfoList [currentTargetIndex];

				currentEnemyTransform = currentEnemyInfo.enemyTransform;

				placeToShoot = currentEnemyInfo.placeToShoot;

				currentEnemyInfoStored = false;
			}

			currentEnemyPosition = currentEnemyTransform.position;

			if (currentEnemyInfo.enemeyHealthManagement.checkIfDeadWithHealthManagement ()) {
				removeEnemy (targetToAttack);

				return;
			}

			if (!currentEnemyInfoStored) {
				targetIsCharacter = currentEnemyInfo.isCharacter;
				targetIsVehicle = currentEnemyInfo.isVehicle;
				targetIsOtherType = currentEnemyInfo.isOtherType;

				if (targetIsCharacter) {
					currentPlayerController = currentEnemyInfo.enemyPlayerController;
				} 

				if (targetIsVehicle) {
					currentVehicleToAttack = (vehicleHUDManager)currentEnemyInfo.enemeyHealthManagement;
				}
			}

			if (targetIsCharacter) {
				if (currentPlayerController != null && currentPlayerController.isPlayerDriving ()) {
					GameObject vehicleToCheck = currentPlayerController.getCurrentVehicle ();

					if (!onSpotted) {
						vehicleToCheck = null;
					}

					removeEnemy (targetToAttack);

					if (vehicleToCheck != null && !enemies.Contains (vehicleToCheck)) {
						//						print ("player entering in vehicle");

						vehicleHUDManager temporalVehicleHUDManager = vehicleToCheck.GetComponent<vehicleHUDManager> ();

						if (temporalVehicleHUDManager != null) {
							checkTriggerInfo (temporalVehicleHUDManager.getVehicleCollider (), true);
						}
					}

					return;
				}
			} 

			if (targetIsVehicle) {

				if (!currentVehicleToAttack.isVehicleBeingDriven ()) {

					removeEnemy (targetToAttack);

					return;
				}
			}

			if (!currentEnemyInfoStored) {
				if (targetIsCharacter || targetIsOtherType) {

					float characterRadius = currentEnemyInfo.characterRadius;

					if (characterRadius > 0) {
						setExtraMinDistanceState (true, characterRadius);

						AINavMeshManager.setExtraMinDistanceState (true, characterRadius);
					} else {
						setExtraMinDistanceState (false, 0);

						AINavMeshManager.setExtraMinDistanceState (false, 0);
					}
				} 

				if (targetIsVehicle) {
					float distanceToConfigure = currentVehicleToAttack.getVehicleRadius ();

					if (attackType != AIAttackType.weapons && attackType != AIAttackType.powers) {
						distanceToConfigure /= 2;
					}

					setExtraMinDistanceState (true, distanceToConfigure);

					AINavMeshManager.setExtraMinDistanceState (true, distanceToConfigure);

					if (minDistanceToCloseCombat < AINavMeshManager.minDistanceToEnemy) {
						minDistanceToCloseCombat = AINavMeshManager.minDistanceToEnemy + 0.1f;
					}

					if (minDistanceToMelee < AINavMeshManager.minDistanceToEnemy) {
						minDistanceToMelee = AINavMeshManager.minDistanceToEnemy + 0.1f;
					}
				}
			}

			if (!onSpotted) {
				//the player can hack the turrets, but for that he has to crouch, so he can reach the back of the turret and activate the panel
				// if the player fails in the hacking or he gets up, the turret will detect the player and will start to fire him
				//check if the player fails or get up
				seeingCurrentTarget = false;

				if (checkRaycastToViewTarget) {
					seeingCurrentTarget = checkIfTargetIsPhysicallyVisible (targetToAttack, false);
				} else {
					seeingCurrentTarget = true;
				}

				if (enemyAlertActive) {
					seeingCurrentTarget = true;
				}

				if (seeingCurrentTarget) {
					lastTimeEnemyVisible = Time.time;

					//if an enemy is inside the trigger, check its position with respect the AI, if the target is in the vision range, adquire it as target
					targetDirection = currentEnemyPosition - currentPosition;

					float angleWithTarget = Vector3.SignedAngle (AITransform.forward, targetDirection, currentEnemyTransform.up);

					bool checkResult = false;

					if (!useVisionRange || Mathf.Abs (angleWithTarget) < visionRange / 2) {
						checkResult = true;
					}

					if (hasBeenAttacked) {
						checkResult = true;
					}

					if (enemyAlertActive) {
						checkResult = true;
					}

					if (ignoreVisionRangeActive) {
						checkResult = true;
					}

					if (checkResult) {
						if (currentPlayerController != null) {
							if ((checkingThreatVisible || posibleThreat == null) || hasBeenAttacked || enemyAlertActive) {

								AICanSeeCurrentTarget = currentPlayerController.isCharacterVisibleToAI () || canSeeHiddenTargets || canSeeTargetsOnStealthMode;

								if (AICanSeeCurrentTarget) {

									hasBeenAttacked = false;
									enemyAlertActive = false;

									targetAdquired ();
								}
							}
						} else {
							//else, the target is a friend of the player, so shoot him
							targetAdquired ();
						}
					} else {
						//else check the distance, if the target is too close, adquire it as target too
						float distanceToTarget = GKC_Utils.distance (currentEnemyPosition, currentPosition);

						if (distanceToTarget < minDistanceToAdquireTarget) {
							if (currentPlayerController != null) {
								AICanSeeCurrentTarget = currentPlayerController.isCharacterVisibleToAI () || canSeeHiddenTargets || canSeeTargetsOnStealthMode;

								AICanDetectCurrentTarget = !currentPlayerController.isCharacterInStealthMode () || canSeeTargetsOnStealthMode;

								if (AICanSeeCurrentTarget ||
								    (allowDetectionWhenTooCloseEvenNotVisible && AICanDetectCurrentTarget)) {
									targetAdquired ();
								}
							} else {
								targetAdquired ();
							}
						}
						//print ("out of range of vision");
					}
				} else {
					if (useMaxTimeToLoseTargetIfNotVisible && lastTimeEnemyVisible > 0) {
						if (Time.time > lastTimeEnemyVisible + maxTimeToLoseTargetIfNotVisible) {
							enemyLost (targetToAttack);

							lastTimeEnemyVisible = 0;

							return;
						}
					}
				}
			}

			if (onSpotted) {
				if (!currentEnemyInfoStored) {
					if (avoidEnemies) {
						AINavMeshManager.avoidTarget (currentEnemyTransform);

						AINavMeshManager.setAvoidTargetState (true);

						runningAway = true;

						checkEventOnSpottedStateChange ();
					} else {
						AINavMeshManager.setTarget (currentEnemyTransform);

						AINavMeshManager.setTargetType (false, false);
					}

					AINavMeshManager.setPatrolState (false);
				}
			}

			currentEnemyInfoStored = true;
		} 

		//if there are no enemies
		else {
			if (onSpotted || runningAway) {
				removeTargetInfo ();
			} else {
				if (previousEnemyAssigned) {
					if (previousEnemy != null) {
						previousEnemy = null;
					}

					previousEnemyAssigned = false;
				}

				if (runningAwayFromPosition) {
					if (GKC_Utils.distance (AITransform.position, lastPositionBeforeRunningAwayFromPosition) > distanceFromRunningAwayPosition) {
						removeTargetInfo ();

						runningAwayFromPosition = false;

						if (showDebugPrint) {
							print ("finishing running away from position state ");
						}
					}
				}
			}
		}
	}

	public void removeTargetInfo ()
	{
		if (showDebugPrint) {
			print ("remove target info");
		}

		if (onSpotted) {
			checkIfReturnCompleteActive = true;

			resetVerticalCameraRotationActive = true;
		}

		placeToShoot = null;
		targetToAttack = null;
		previousEnemy = null;
		onSpotted = false;

		previousEnemyAssigned = false;

		currentEnemyInfoStored = false;

		lookingAtTargetPosition = false;

		lastTimeSpotted = Time.time;

		checkEventOnSpottedStateChange ();

		fovTrigger.radius = originalFOVRadius;
		currentFovRadius = originalFOVRadius;

		enableViewTrigger ();

		AINavMeshManager.removeTarget ();

		if (avoidEnemies) {
			AINavMeshManager.setAvoidTargetState (false);
			runningAway = false;
		}

//		runningAwayFromPosition = false;
	
		if (useDelayToResumeActivityAfterOnSpottedDisable) {
			checkResumeRegularActivity ();
		} else {
			resumeRegularActivity ();
		}
	}

	public void stopResumeRegularActivityCoroutine ()
	{
		if (regularActivityCoroutine != null) {
			StopCoroutine (regularActivityCoroutine);
		}
	}

	public void checkResumeRegularActivity ()
	{
		stopResumeRegularActivityCoroutine ();

		regularActivityCoroutine = StartCoroutine (resumeRegularActivityCoroutine ());
	}

	IEnumerator resumeRegularActivityCoroutine ()
	{
		yield return new WaitForSeconds (delayToResumeActivityAfterOnSpottedDisable);

		resumeRegularActivity ();
	}

	public void resumeRegularActivity ()
	{
		//stop the character from look towards his target
		AINavMeshManager.lookAtTaget (false);

		if (partnerFound) {
			AINavMeshManager.setTarget (partner.transform);

			AINavMeshManager.setPatrolState (false);

			AINavMeshManager.setTargetType (true, false);
		} else {
			if (AIPatrolManager != null && !wanderEnabled) {
				if (!AIPatrolManager.isPatrolPaused ()) {

					AINavMeshManager.setPatrolPauseState (false);

					AIPatrolManager.setClosestWayPoint ();

					AIPatrolManager.setReturningToPatrolState (true);
				}
			}
		}

		resetCheckThreatValues ();
	}

	public void stopEnableViewTriggerCoroutine ()
	{
		if (viewTriggerCoroutine != null) {
			StopCoroutine (viewTriggerCoroutine);
		}
	}

	public void enableViewTrigger ()
	{
		stopEnableViewTriggerCoroutine ();

		viewTriggerCoroutine = StartCoroutine (enableViewTriggerCoroutine ());
	}

	IEnumerator enableViewTriggerCoroutine ()
	{
		yield return new WaitForSeconds (delayToEnableViewTriggerAfterTargetLost);

		if (viewTrigger != null) {
			if (!viewTrigger.gameObject.activeSelf) {
				viewTrigger.gameObject.SetActive (true);
			}
		}

		if (mainAIViewTriggerSystem != null) {
			if (!mainAIViewTriggerSystem.gameObject.activeSelf) {
				mainAIViewTriggerSystem.gameObject.SetActive (true);
			}
		}
	}

	public void checkIfResumeAfterNoEnemiesFound ()
	{
		if (returnToPositionAfterNoEnemiesFound) {
			currentPositionToReturn = positionToReturnAfterNoEnemiesFound;

			AINavMeshManager.setTarget (positionToReturnAfterNoEnemiesFound);

			AINavMeshManager.setTargetType (false, true);

			adjustRotationOnReturnToPositionActive = adjustPlayerRotationAfterNoEnemiesFound;

			returningToPositionActive = true;

			checkIfReturnCompleteActive = false;
		}
	}

	public void checkIfResumeAfterCheckingNoise ()
	{
		if (!partnerFound) {
			if (AIPatrolManager != null && !wanderEnabled) {
				if (!AIPatrolManager.isPatrolPaused ()) {
					AINavMeshManager.setPatrolPauseState (false);

					AIPatrolManager.setClosestWayPoint ();

					AINavMeshManager.setPatrolState (true);

					AIPatrolManager.setReturningToPatrolState (true);
				}
			}
		} else {
			AINavMeshManager.setTarget (partner.transform);

			AINavMeshManager.setPatrolState (false);

			AINavMeshManager.setTargetType (true, false);
		}

		playerControllerManager.disableCharacterStateIcon ();

		checkingNoiseActive = false;

		if (returnToPositionWhenNoiseChecked && !partnerFound) {
			currentPositionToReturn = positionToReturnAfterNoiseChecked;

			AINavMeshManager.setTarget (positionToReturnAfterNoiseChecked);

			AINavMeshManager.setTargetType (false, true);

			adjustRotationOnReturnToPositionActive = adjustPlayerRotationAfterNoiseChecked;

			returningToPositionActive = true;
		}
	}

	public void checkIfResumeAfterRemovingPartner ()
	{
		if (AIPatrolManager != null && !wanderEnabled) {
			if (!AIPatrolManager.isPatrolPaused ()) {
				AINavMeshManager.setPatrolPauseState (false);

				AIPatrolManager.setClosestWayPoint ();

				AINavMeshManager.setPatrolState (true);

				AIPatrolManager.setReturningToPatrolState (true);

				AINavMeshManager.setTargetType (false, true);
			}
		} else {
			if (returnToPositionWhenPartnerRemoved) {
				currentPositionToReturn = positionToReturnAfterPartnerRemoved;

				AINavMeshManager.setTarget (positionToReturnAfterPartnerRemoved);

				AINavMeshManager.setTargetType (false, true);

				adjustRotationOnReturnToPositionActive = adjustPlayerRotationAfterPlayerRemoved;

				returningToPositionActive = true;
			}
		}
	}

	public void resetCheckingNoiseState ()
	{
		checkingNoiseActive = false;

		if (adjustPlayerRotationAfterNoiseChecked || adjustPlayerRotationAfterNoEnemiesFound || adjustPlayerRotationAfterPlayerRemoved) {
			playerControllerManager.stopRotateCharacterTowardDirectionCoroutine ();
		}
	}

	public void setIgnoreVisionRangeActiveState (bool state)
	{
		ignoreVisionRangeActive = state;
	}

	public void setAvoidEnemiesState (bool state)
	{
		avoidEnemies = state;
	}

	public bool checkIfTargetIsPhysicallyVisible (GameObject targetToCheck, bool checkingSuspect)
	{
		if (placeToShoot != null) {
			targetToCheckDirection = placeToShoot.position - rayCastPosition.position;
		} else {
			if (temporalPlaceToShoot == null) {
				temporalPlaceToShoot = applyDamage.getPlaceToShoot (targetToCheck);
			}

			if (temporalPlaceToShoot != null) {
				targetToCheckDirection = temporalPlaceToShoot.position - rayCastPosition.position;
			} else {
				targetToCheckDirection = (targetToCheck.transform.position + targetToCheck.transform.up) - rayCastPosition.position;
			}
		}

		if (targetIsVehicle) {
			if (checkingSuspect) {
				if (temporalHealthManagementToCheck == null) {
					temporalHealthManagementToCheck = applyDamage.getHealthManagement (targetToCheck);
				}
			} else {
				temporalHealthManagementToCheck = currentVehicleToAttack;
			}
		}

		if (Physics.Raycast (rayCastPosition.position, targetToCheckDirection, out hit, longDistance, layerToCheckRaycastToViewTarget)) {
			//print (hit.collider.gameObject.name + " " + targetIsVehicle);
			//print (hit.collider.name + " " + hit.transform.IsChildOf (targetToCheck.transform) + " " + targetToCheck.name);

			bool targetVisibilityConfirmed = false;

			if (targetIsVehicle) {
				if (temporalHealthManagementToCheck != null) {
					if (temporalHealthManagementToCheck.gameObject == targetToCheck && checkingSuspect) {
						targetVisibilityConfirmed = true;
					}
				 
					if (temporalHealthManagementToCheck.checkIfDetectSurfaceBelongToVehicleWithHealthManagement (hit.collider)) {
						targetVisibilityConfirmed = true;
					}
				}
			} else {
				if (hit.collider.gameObject == targetToCheck) {
					targetVisibilityConfirmed = true;
				}

				if (hit.transform.IsChildOf (targetToCheck.transform)) {
					targetVisibilityConfirmed = true;
				}
			}
				
			if (targetVisibilityConfirmed) {

				if (showGizmo) {
					Debug.DrawRay (rayCastPosition.position, targetToCheckDirection, Color.green, 2);
				}

				return true;
			} else {
				if (showGizmo) {
					Debug.DrawRay (rayCastPosition.position, targetToCheckDirection, Color.red, 2);
				}

				return false;
			}
		} else {
			if (showGizmo) {
				Debug.DrawRay (rayCastPosition.position, targetToCheckDirection, Color.black, 2);
			}
		}

		return false;
	}

	public void setAttackMode (bool attackTargetDirectly, bool targetCurrentlyVisible)
	{
		if (AIDead) {
			return;
		}
			
		if (attackType == AIAttackType.weapons) {
			if (currentPathDistanceToTarget <= minDistanceToDraw) {

				minRangeToActivateAttackBehaviorActive = true;

				insideMinDistanceToAttack = true;

			} else {
				if (minRangeToActivateAttackBehaviorActive) {
					if (currentPathDistanceToTarget > minDistanceToDraw + 2) {

						minRangeToActivateAttackBehaviorActive = false;

						insideMinDistanceToAttack = false;
					}

					if (currentPathDistanceToTarget > minDistanceToDraw + 0.3f) {

						insideMinDistanceToAttack = false;
					}
				}
			}

			if (minRangeToActivateAttackBehaviorActive) {

				canUseAttackActive = checkIfMinimumAngleToAttack ();

				if (!targetCurrentlyVisible) {
					canUseAttackActive = false;

					AINavMeshManager.enableHalfMinDistance ();
				}

			} else {
				canUseAttackActive = false;
			}

			updateFireWeaponsBehavior ();

			updateFireWeaponsAttack ();

			updateFireWeaponsInsideMinRange ();
		} else if (attackType == AIAttackType.powers) {
			if (currentPathDistanceToTarget <= minDistanceToDraw) {

				minRangeToActivateAttackBehaviorActive = true;

				insideMinDistanceToAttack = true;

			} else {
				if (minRangeToActivateAttackBehaviorActive) {
					if (currentPathDistanceToTarget > minDistanceToDraw + 2) {

						minRangeToActivateAttackBehaviorActive = false;

						insideMinDistanceToAttack = false;
					}

					if (currentPathDistanceToTarget > minDistanceToDraw + 0.3f) {

						insideMinDistanceToAttack = false;
					}
				}
			}

			if (minRangeToActivateAttackBehaviorActive) {

				canUseAttackActive = checkIfMinimumAngleToAttack ();

				if (!targetCurrentlyVisible) {
					canUseAttackActive = false;

					AINavMeshManager.enableHalfMinDistance ();
				}

			} else {
				canUseAttackActive = false;
			}

			updatePowersBehavior ();

			updatePowersAttack ();

			updatePowersInsideMinRange ();
		} else if (attackType == AIAttackType.closeCombat) {
			float currentMinDistance = minDistanceToCloseCombat;

			if (AINavMeshManager.rangeToTargetReached) {
				currentMinDistance += AINavMeshManager.currentDistanceToTargetOffset;
			}

			if (currentPathDistanceToTarget <= currentMinDistance) {

				minRangeToActivateAttackBehaviorActive = true;

				insideMinDistanceToAttack = true;

			} else {
				if (minRangeToActivateAttackBehaviorActive) {
					if (currentPathDistanceToTarget > minDistanceToCloseCombat + 2) {

						minRangeToActivateAttackBehaviorActive = false;
					}
				} else {
					lookingAtTargetPosition = false;
				}

				insideMinDistanceToAttack = false;
			}

			if (minRangeToActivateAttackBehaviorActive) {
				lookingAtTargetPosition = true;

				lootAtTarget (placeToShoot);

				canUseAttackActive = checkIfMinimumAngleToAttack ();

				if (!targetCurrentlyVisible) {
					canUseAttackActive = false;

					AINavMeshManager.enableHalfMinDistance ();
				}

			} else {
				canUseAttackActive = false;
			}
				
			updateCloseCombatBehavior ();

			updateCloseCombatAttack ();

			updateCloseCombatInsideMinRange ();
		} else if (attackType == AIAttackType.melee) {
			float currentMinDistance = minDistanceToMelee;

			if (AINavMeshManager.rangeToTargetReached) {
				currentMinDistance += AINavMeshManager.currentDistanceToTargetOffset;
			}

			if (currentPathDistanceToTarget <= currentMinDistance) {
				minRangeToActivateAttackBehaviorActive = true;

				insideMinDistanceToAttack = true;

			} else {
				if (minRangeToActivateAttackBehaviorActive) {
					if (currentPathDistanceToTarget > minDistanceToMelee + 2) {

						minRangeToActivateAttackBehaviorActive = false;

						insideMinDistanceToAttack = false;
					}

					if (currentPathDistanceToTarget > minDistanceToMelee + 0.3f) {

						insideMinDistanceToAttack = false;
					}
				} else {
					lookingAtTargetPosition = false;
				}
			}

			if (minRangeToActivateAttackBehaviorActive) {
				lookingAtTargetPosition = true;

				lootAtTarget (placeToShoot);

				canUseAttackActive = checkIfMinimumAngleToAttack ();

				if (!targetCurrentlyVisible) {
					canUseAttackActive = false;

					AINavMeshManager.enableHalfMinDistance ();
				}

			} else {
				canUseAttackActive = false;
			}

			updateMeleeBehavior ();

			updateMeleeAttack ();

			updateMeleeInsideMinRange ();
		}
	}

	public void checkIfAIDetectsTargetOnRangeOutOfViewWithProbability (int probablityAmount)
	{
		if (targetToAttack == null) {
			return;
		}

		int randomValue = Random.Range (0, 100);

		if (showDebugPrint) {
			print ("Obtained probability to detect targets " + randomValue + "  " + probablityAmount);
		}

		if (randomValue < probablityAmount) {
			closestTarget ();

			targetAdquired ();
		}
	}

	public void targetAdquired ()
	{
		if (showDebugPrint) {
			print ("Target acquired");
		}

		stopResumeRegularActivityCoroutine ();

		resetVerticalCameraRotationActive = false;

		onSpotted = true;

		checkEventOnSpottedStateChange ();

		float newFovTriggerRadius = GKC_Utils.distance (targetToAttack.transform.position, AITransform.position) + extraFieldOfViewRadiusOnSpot;

		if (avoidEnemies) {
			newFovTriggerRadius += AINavMeshManager.minDistanceToRunFromTarget;
		}

		fovTrigger.radius = newFovTriggerRadius;

		currentFovRadius = fovTrigger.radius;

		stopEnableViewTriggerCoroutine ();

		if (viewTrigger != null && viewTrigger.gameObject.activeSelf) {
			viewTrigger.gameObject.SetActive (false);
		}

		if (mainAIViewTriggerSystem != null && mainAIViewTriggerSystem.gameObject.activeSelf) {
			mainAIViewTriggerSystem.gameObject.SetActive (false);
		}

		//make the character to look towards his target
		AINavMeshManager.lookAtTaget (true);

		//send this enemy to faction system for the detected enemies list
		factionManager.addDetectedEnemyFromFaction (targetToAttack);

		playerControllerManager.setCharacterStateIcon (surprisedCharacterStateName);

		if (alertFactionOnSpotted) {
			alertAIFactionFromTargetToAttack (targetToAttack);
		}

		resetCheckingNoiseState ();

		currentEnemyInfoStored = false;

		checkIfReturnCompleteActive = false;

		runningAwayFromPosition = false;

		if (pauseMovementWhenCheckingThreats) {
			AINavMeshManager.setPauseMoveNavMeshState (false);
		}
	}

	public void alertAIFactionFromTargetToAttack (GameObject newTarget)
	{
		factionManager.alertFactionOnSpotted (alertFactionRadius, newTarget, AITransform.position);
	}

	public void alertAIFactionToSearchAndAttackPlayer ()
	{
		alertAIFactionFromTargetToAttack (GKC_Utils.findMainPlayerOnScene ());
	}

	public void alertAIFactionToSearchAndAttackPlayerOnDeath ()
	{
		if (alertFactionFromPlayerOnDeath) {
			alertAIFactionToSearchAndAttackPlayer ();
		}
	}

	bool behaviorStatesPaused;

	public void setBehaviorStatesPausedState (bool state)
	{
		behaviorStatesPaused = state;

		if (currentAIBehaviorAssigned) {
			currentAIBehavior.setBehaviorStatesPausedState (behaviorStatesPaused);
		}
	}

	public void setWaitToActivateAttackActiveState (bool state)
	{
		if (currentAIBehaviorAssigned) {
			currentAIBehavior.setWaitToActivateAttackActiveState (state);

			if (useAbilitiesBehavior) {
				abilitiesAIBehavior.setWaitToActivateAttackActiveState (state);
			}
		}
	}

	public void setUseRandomWalkEnabledState (bool state)
	{
		if (currentAIBehaviorAssigned) {
			currentAIBehavior.setUseRandomWalkEnabledState (state);
		}
	}

	public void setOriginalUseRandomWalkEnabledState ()
	{
		if (currentAIBehaviorAssigned) {
			currentAIBehavior.setOriginalUseRandomWalkEnabledState ();
		}
	}

	//Functions to use the powers
	public void updatePowersBehavior ()
	{
		mainPowerAIBehavior.updateAIBehaviorState ();
	}

	public void updatePowersAttack ()
	{
		mainPowerAIBehavior.updateAIAttackState (canUseAttackActive);
	}

	public void updatePowersInsideMinRange ()
	{
		mainPowerAIBehavior.updateInsideRangeDistance (insideMinDistanceToAttack);
	}


	//Functions to use the weapons
	public void updateFireWeaponsBehavior ()
	{
		mainWeaponAIBehavior.updateAIBehaviorState ();
	}

	public void updateFireWeaponsAttack ()
	{
		mainWeaponAIBehavior.updateAIAttackState (canUseAttackActive);
	}

	public void updateFireWeaponsInsideMinRange ()
	{
		mainWeaponAIBehavior.updateInsideRangeDistance (insideMinDistanceToAttack);
	}




	//Functions to use combat
	public void updateCloseCombatBehavior ()
	{
		mainCloseCombatAIBehavior.updateAIBehaviorState ();
	}

	public void updateCloseCombatAttack ()
	{
		mainCloseCombatAIBehavior.updateAIAttackState (canUseAttackActive);
	}

	public void updateCloseCombatInsideMinRange ()
	{
		mainCloseCombatAIBehavior.updateInsideRangeDistance (insideMinDistanceToAttack);
	}


	//Functions to use melee combat
	public void updateMeleeBehavior ()
	{
		mainMeleAIBehavior.updateAIBehaviorState ();
	}

	public void updateMeleeAttack ()
	{
		mainMeleAIBehavior.updateAIAttackState (canUseAttackActive);
	}

	public void updateMeleeInsideMinRange ()
	{
		mainMeleAIBehavior.updateInsideRangeDistance (insideMinDistanceToAttack);
	}


	public void resetBehaviorStates ()
	{
		if (mainWeaponAIBehavior != null) {
			mainWeaponAIBehavior.resetBehaviorStates ();
		}

		if (mainPowerAIBehavior != null) {
			mainPowerAIBehavior.resetBehaviorStates ();
		}

		if (mainCloseCombatAIBehavior != null) {
			mainCloseCombatAIBehavior.resetBehaviorStates ();
		}

		if (mainMeleAIBehavior != null) {
			mainMeleAIBehavior.resetBehaviorStates ();
		}
	}

	public void pauseAction (bool state)
	{
		AIPaused = state;

		AIDead = playerControllerManager.checkIfPlayerDeadFromHealthComponent ();

		if (AIDead) {
			if (AIPatrolManager != null) {
				AIPatrolManager.pauseOrPlayPatrol (true);
			}

			if (addAndRemoveEnemyAroundToDetectedPlayers && sendRemoveEnemyAroundOnDeath && enemies.Count > 0) {
				for (int i = 0; i < enemies.Count; i++) {
					if (enemies [i] != null) {
						applyDamage.sendCharacterAroundToRemove (enemies [i], AITransform, sendCharacterAroundToRemoveName);
					}
				}
			}
		} else {
			resetAttackState ();
		}

		if (ignoreVisionRangeWhenGetUp) {
			if (!playerControllerManager.isActionActive ()) {
				ignoreVisionRangeActive = true;
				lastTimePaused = Time.time;
				gettingUpActive = true;
			}
		}

		if (AIPaused) {
			runningAwayFromPosition = false;
		} else {
			checkingIfCanMoveAfterGettingUp = true;
		}

		disableWanderValues ();

		targetDetectionActiveOnPausedAIState = false;

		checkTargetsOnTriggerPaused = false;

		resetBehaviorStates ();
	}

	public bool isAIPaused ()
	{
		return AIPaused;
	}

	public void setTargetDetectionActiveOnPausedAIState (bool state)
	{
		targetDetectionActiveOnPausedAIState = state;
	}

	public void checkObject (GameObject objectToCheck)
	{
		Collider currentCollider = objectToCheck.GetComponent<Collider> ();

		if (currentCollider != null) {
			OnTriggerEnter (currentCollider);
		}
	}

	public void setObjectOutOfRange (GameObject objectToCheck)
	{
		Collider currentCollider = objectToCheck.GetComponent<Collider> ();

		if (currentCollider != null) {
			OnTriggerExit (currentCollider);
		}
	}

	public void setRemovePartnerAfterDelayState (bool state)
	{
		removePartnerAfterDelay = state;
	}

	public void setDelayToRemoveCurrentPartnerValue (float newValue)
	{
		delayToRemoveCurrentPartner = newValue;
	}

	public void addPlayerAsPartner (GameObject objectToCheck)
	{
		followPartnerOnTrigger = true;

		checkObject (objectToCheck);
	}

	public void removeCurrentPartner ()
	{
		followPartnerOnTrigger = false;

		removePartner ();
	}

	public bool isPartnerFound ()
	{
		return partnerFound;
	}

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public bool canCheckSuspect (int suspectLayer)
	{
		if ((1 << suspectLayer & layerToCheckTargets.value) == 1 << suspectLayer) {
			return true;
		}

		return false;
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (ignoreTriggersOnTargetDetection && col.isTrigger) {
			return;
		}

		if (avoidLoseTrackOfDetectedTargets) {
			if (!isEnter) {
				return;
			}
		}

		if (canCheckSuspect (col.gameObject.layer)) {

			if (isEnter) {

				if (checkTargetsOnTriggerPaused) {
					return;
				}

				if (!AIPaused || targetDetectionActiveOnPausedAIState) {

					if (checkCharacterFaction (col.gameObject, false)) {
						addEnemy (col.gameObject);
					} else {
						addNotEnemey (col.gameObject);

						objectToCheck = col.gameObject;

						if (currentVehicleHUDManager != null) {
							if (currentVehicleHUDManager.isVehicleBeingDriven ()) {
								GameObject currentDriver = currentVehicleHUDManager.getCurrentDriver ();

								if (currentDriver != null) {
									objectToCheck = currentDriver;
								}
							}
						}

						if (showDebugPrint) {
							print ("checking target " + objectToCheck.name);

							print (factionManager.checkIfCharacterBelongsToFaction (factionToFollowAsPartner, objectToCheck));

							print (checkCharacterFaction (objectToCheck, false));

							print ("\n \n \n");
						}

						if (factionManager.checkIfCharacterBelongsToFaction (factionToFollowAsPartner, objectToCheck) && !partnerFound) {
							if (!checkCharacterFaction (objectToCheck, false)) {
								if (followPartnerOnTrigger) {
									if (useEventOnPartnerFound && !onSpotted) {
										eventOnPartnerFound.Invoke ();
									}

									partner = objectToCheck;

									playerControllerPartner = partner.GetComponent<playerController> ();

									if (onSpotted) {
										AINavMeshManager.setPartnerFoundInfo (partner.transform);
									} else {
										AINavMeshManager.partnerFound (partner.transform);

										AINavMeshManager.setTargetType (true, false);
									}

									partnerFound = true;

									lastTimePartnerFound = Time.time;
								}
							} else {
								removeNotEnemy (objectToCheck);

								enemyDetected (objectToCheck);
							}
						}
					}
				}
			} else {
				if (checkCharacterFaction (col.gameObject, false)) {
					enemyLost (col.gameObject);
				} else {
					removeNotEnemy (col.gameObject);
				}
			}
		}
	}

	public void removePartner ()
	{
		partnerFound = false; 

		AINavMeshManager.removePartner ();

		partner = null;

		playerControllerPartner = null;

		lastTimePartnerFound = 0;

		//		print ("remove partner");
	}

	public void checkNoisePosition (Vector3 noisePosition, float noiseDecibels, bool forceNoiseDetection, int noiseID)
	{
		if (!canHearNoises && !forceNoiseDetection) {
			return;
		}

		if (checkNoiseDecibels && noiseDecibels < decibelsDetectionAmount && !forceNoiseDetection) {
			return;
		}

		if (!onSpotted &&
		    !checkingThreatVisible &&
		    !checkingThreatNotVisible &&
		    (!partnerFound || checkNoisesEvenIfPartnerFound) &&
		    Time.time > lastTimeSpotted + 2) {

			bool currentUseMaxHearDistance = useMaxHearDistance;
			float currentMaxHearDistance = maxHearDistance;

			if (useNoiseReactionInfo) {
				if (noiseID > -1) {
					int noiseReactionIndex = mainNoiseReactionData.noiseReactionInfoList.FindIndex (s => s.ID == noiseID);

					if (noiseReactionIndex > -1) {
						noiseReactionInfo currentNoiseReactionInfo = mainNoiseReactionData.noiseReactionInfoList [noiseReactionIndex];
					
						if (currentNoiseReactionInfo.noiseReactionEnabled) {

							if (showDebugPrint) {
								print ("custom noise reaction activated " + currentNoiseReactionInfo.Name);
							}

							currentUseMaxHearDistance = currentNoiseReactionInfo.useMaxNoiseDistance;
							currentMaxHearDistance = currentNoiseReactionInfo.maxNoiseDistance;

							if (currentNoiseReactionInfo.useRemoteEventOnNoise) {
								GKC_Utils.activateRemoteEvent (currentNoiseReactionInfo.remoteEventOnNoise, AITransform.gameObject);
							}

							if (currentNoiseReactionInfo.runAwayFromNoiseOrigin) {
//								GameObject currentPlayer = GKC_Utils.findMainPlayerOnScene ();
//
//								if (currentPlayer != null) {
//									checkShootOrigin (currentPlayer);
//								}

								Vector3 currentNoisePosition = getPositionFromRaycastAll (noisePosition, currentUseMaxHearDistance, currentMaxHearDistance);

								if (currentNoisePosition == Vector3.zero) {
									currentNoisePosition = noisePosition;
								}

								emptyTargetPositionTransform.position = currentNoisePosition;

								AINavMeshManager.avoidTarget (emptyTargetPositionTransform);

								AINavMeshManager.setAvoidTargetState (true);

								AINavMeshManager.setPatrolState (false);

								runningAwayFromPosition = true;

								lastPositionBeforeRunningAwayFromPosition = AITransform.position;

								distanceFromRunningAwayPosition = 
									GKC_Utils.distance (emptyTargetPositionTransform.position, AITransform.position) +
								extraFieldOfViewRadiusOnSpot;

								distanceFromRunningAwayPosition += AINavMeshManager.minDistanceToRunFromTarget;

								checkIfInterruptWanderState ();
							}

							if (showDebugPrint) {
								print (currentNoiseReactionInfo.Name);
							}
								
							bool setNoisePosition = true;

							if (!currentNoiseReactionInfo.checkNoiseOrigin) {
								setNoisePosition = false;
							}

							if (!setNoisePosition) {
								return;
							}
						}
					}
				} else {
					if (!checkRegularNoiseStateIfReactionInfoNotFound) {
						return;
					}
				}
			}

			Vector3 newNoisePosition = getPositionFromRaycastAll (noisePosition, currentUseMaxHearDistance, currentMaxHearDistance);

			if (newNoisePosition != Vector3.zero) {

				if (useEventOnStartCheckNoise) {
					eventOnStartCheckNoise.Invoke ();
				}

				emptyTargetPositionTransform.position = newNoisePosition;

				AINavMeshManager.setTarget (emptyTargetPositionTransform);

				AINavMeshManager.setTargetType (false, true);

				AINavMeshManager.setPatrolState (false);

				AINavMeshManager.setPatrolPauseState (true);

				currentTimeCheckingNoise = 0;

				checkingNoiseActive = true;

				playerControllerManager.setCharacterStateIcon (wonderingCharacterStateName);
					
			}
		}
	}

	Vector3 getPositionFromRaycastAll (Vector3 originPosition, bool useMaxDistance, float maxDistanceToCheck)
	{
		newRay = new Ray ();

		newRay.direction = -Vector3Up;
		newRay.origin = originPosition + Vector3Up;

		RaycastHit[] hits = Physics.RaycastAll (newRay, raycastDistanceToCheckNoises, layerToCheckNoises);
		System.Array.Sort (hits, (x, y) => x.distance.CompareTo (y.distance));

		bool canCheckPosition = true;

		foreach (RaycastHit rh in hits) {
			if (useMaxDistance) {
				canCheckPosition = true;

				float distanceToCheck = GKC_Utils.distance (AITransform.position, hit.point);

				if (distanceToCheck > maxDistanceToCheck) {
					canCheckPosition = false;
				}
			}

			if (canCheckPosition) {
				Rigidbody currenRigidbodyFound = applyDamage.applyForce (rh.collider.gameObject);

				if (currenRigidbodyFound == null) {
					return rh.point + rh.normal * 0.3f;
				}
			}
		}

		return Vector3.zero;
	}

	public void resetAttackState ()
	{
		if (mainWeaponAIBehavior != null) {
			mainWeaponAIBehavior.resetAttackState ();
		}

		if (mainPowerAIBehavior != null) {
			mainPowerAIBehavior.resetAttackState ();
		}

		if (mainCloseCombatAIBehavior != null) {
			mainCloseCombatAIBehavior.resetAttackState ();
		}

		if (mainMeleAIBehavior != null) {
			mainMeleAIBehavior.resetAttackState ();
		}
	}

	public void setExtraMinDistanceState (bool state, float extraDistance)
	{
		if (state) {
			minDistanceToDraw =	originalMinDistanceToDraw + extraDistance;
			minDistanceToAim = originalMinDistanceToAim + extraDistance;
			minDistanceToShoot = originalMinDistanceToShoot + extraDistance;

			minDistanceToAimPower = originalMinDistanceToAimPower + extraDistance;

			minDistanceToShootPower = originalMinDistanceToShootPower + extraDistance;

			minDistanceToCloseCombat = originalMinDistanceToCloseCombat + extraDistance;

			if (currentMeleeWeaponAtDistance) {
				minDistanceToMelee = minDistanceToMeleeAtDistance + extraDistance;
			} else {
				minDistanceToMelee = originalMinDistanceToMelee + extraDistance;
			}
		} else {
			minDistanceToDraw =	originalMinDistanceToDraw;
			minDistanceToAim = originalMinDistanceToAim;
			minDistanceToShoot = originalMinDistanceToShoot;

			minDistanceToAimPower = originalMinDistanceToAimPower;

			minDistanceToShootPower = originalMinDistanceToShootPower;

			minDistanceToCloseCombat = originalMinDistanceToCloseCombat;

			if (currentMeleeWeaponAtDistance) {
				minDistanceToMelee = minDistanceToMeleeAtDistance;
			} else {
				minDistanceToMelee = originalMinDistanceToMelee;
			}
		}
	}

	public void setNewMinDistanceToCloseCombat (float newValue)
	{
		minDistanceToCloseCombat = newValue;

		originalMinDistanceToCloseCombat = newValue;
	}

	public void setNewMinDistanceToEnemyUsingCloseCombat (float newValue)
	{
		minDistanceToEnemyUsingCloseCombat = newValue;
	}

	public void setNewMinDistanceToMelee (float newValue)
	{
		minDistanceToMelee = newValue;

		originalMinDistanceToMelee = newValue;
	}

	public void checkCharactersAroundAI ()
	{
		for (int i = 0; i < notEnemies.Count; i++) {
			enemyDetected (notEnemies [i]);
		}
	}

	public bool isSearchingWeapon ()
	{
		return searchingWeapon;
	}

	public void setSearchigWeaponState (bool state)
	{
		searchingWeapon = state;
	}

	public float getDistanceToTarget ()
	{
		if (onSpotted && targetToAttack != null) {
			return GKC_Utils.distance (targetToAttack.transform.position, AITransform.position);
		}

		return -1;
	}

	public GameObject getCurrentTargetToAttack ()
	{
		return targetToAttack;
	}

	public bool isOnSpotted ()
	{
		return onSpotted;
	}

	public float getRemainingDistanceToTarget ()
	{
		return AINavMeshManager.getRemainingDistanceToTarget ();
	}

	public void checkEventOnSpottedStateChange ()
	{
		if (onSpotted) {
			if (useEventOnSpotted) {
				eventOnSpotted.Invoke ();
			}
		} else {
			if (useEventOnNoEnemies) {
				eventOnNoEnemies.Invoke ();
			}

			resetBehaviorStates ();
		}
	}

	public void searchEnemiesAround ()
	{
		enemies.Clear ();

		notEnemies.Clear ();

		enemyInfoList.Clear ();

		closestTarget ();

		if (searchEnemiesAroundOnGetUpEnabled) {
			forceFovTriggerToDetectAnythingAround ();
		}
	}

	public void resetAITargets ()
	{
		if (onSpotted || runningAway || enemies.Count > 0) {
			removeTargetInfo ();
		}

		resetCheckThreatValues ();

		clearFullEnemiesList ();

		searchEnemiesAround ();
	}

	public void clearFullEnemiesList ()
	{
		fullEnemyList.Clear ();
	}

	public void removeCharacterAsTargetOnSameFaction ()
	{
		factionManager.removeCharacterAsTargetOnSameFaction (AITransform.gameObject);
	}

	public void sendSignalToRemoveCharacterAsTarget (GameObject objectToRemove)
	{
		removeEnemy (objectToRemove);

		if (fullEnemyList.Contains (objectToRemove)) {
			fullEnemyList.Remove (objectToRemove);
		}

		if (posibleThreat != null && posibleThreat == objectToRemove) {
			resetCheckThreatValues ();
		}
	}

	public void forceFovTriggerToDetectAnythingAround ()
	{
		fovTrigger.enabled = false;

		fovTrigger.enabled = true;
	}

	public void updateWeaponsAvailableValue ()
	{
		if (currentAIBehaviorAssigned) {
			currentAIBehavior.updateWeaponsAvailableState ();
		}
	}

	public void setWeaponsAttackMode ()
	{
		resetAttackState ();

		attackType = AIAttackType.weapons;

		AINavMeshManager.setDistanceToEnemy (minDistanceToEnemyUsingWeapons);

		currentAIBehavior = mainWeaponAIBehavior;

		currentAIBehaviorAssigned = true;
	}

	public void setPowersAttackMode ()
	{
		resetAttackState ();

		attackType = AIAttackType.powers;

		AINavMeshManager.setDistanceToEnemy (minDistanceToEnemyUsingPowers);

		currentAIBehavior = mainPowerAIBehavior;

		currentAIBehaviorAssigned = true;
	}

	public void setCloseCombatAttackMode ()
	{
		resetAttackState ();

		attackType = AIAttackType.closeCombat;

		AINavMeshManager.setDistanceToEnemy (minDistanceToEnemyUsingCloseCombat);

		currentAIBehavior = mainCloseCombatAIBehavior;

		currentAIBehaviorAssigned = true;
	}

	public void setMeleeAttackMode ()
	{
		resetAttackState ();

		attackType = AIAttackType.melee;

		AINavMeshManager.setDistanceToEnemy (minDistanceToEnemyUsingMelee);

		currentAIBehavior = mainMeleAIBehavior;

		currentAIBehaviorAssigned = true;
	}

	public void setMeleeAttackModeAtDistanceState (bool state)
	{
		currentMeleeWeaponAtDistance = state;

		if (currentMeleeWeaponAtDistance) {
			AINavMeshManager.setDistanceToEnemy (minDistanceToEnemyUsingMeleeAtDistance);

			minDistanceToMelee = minDistanceToMeleeAtDistance;
		} else {
			AINavMeshManager.setDistanceToEnemy (minDistanceToEnemyUsingMelee);

			minDistanceToMelee = originalMinDistanceToMelee;
		}
	}

	public void disableAttackMode ()
	{
		resetAttackState ();

		attackType = AIAttackType.none;
	}

	public void dropWeapon ()
	{
		if (currentAIBehaviorAssigned && currentAIBehavior.carryingWeapon () && !AIPaused) {
			currentAIBehavior.dropWeapon ();
		}
	}

	public void checkPartnerState ()
	{
		if (partnerFound) {
			if (playerControllerPartner != null) {
				if (!AINavMeshManager.isCharacterWaiting ()) {
					if (playerControllerPartner.isPlayerDriving ()) {
						if (!characterOnVehicle && !AINavMeshManager.isCharacterAttacking ()) {
							if (currentVehicle == null) {

								currentVehicle = playerControllerPartner.getCurrentVehicle ();

								currentVehicleHUDManagerToGetOn = currentVehicle.GetComponent<vehicleHUDManager> ();

								setExtraMinDistanceState (true, currentVehicleHUDManagerToGetOn.getVehicleRadius ());

								AINavMeshManager.setExtraMinDistanceState (true, currentVehicleHUDManagerToGetOn.getVehicleRadius ());

								addNotEnemey (playerControllerPartner.gameObject);
							}

							if (!AINavMeshManager.isFollowingTarget () && !currentVehicleHUDManagerToGetOn.isVehicleFull ()) {

								if (playerControllerManager.canCharacterGetOnVehicles ()) {
									if (usingDevicesManager != null) {
										characterOnVehicle = true;

										AINavMeshManager.pauseAI (true);

										usingDevicesManager.addDeviceToList (currentVehicle);

										usingDevicesManager.getclosestDevice (false, false);

										usingDevicesManager.setCurrentVehicle (currentVehicle);

										usingDevicesManager.useCurrentDevice (currentVehicle);
									}
								}
							}
						} else {
							if (AINavMeshManager.isCharacterAttacking ()) {
								getOffFromVehicle ();
							}
						}
					} else {
						getOffFromVehicle ();
					}
				}

				if (removePartnerIfDead) {
					if (playerControllerPartner.isPlayerDead ()) {
						removePartner ();
					}
				}
			}
		}
	}

	public void removeStateOnVehicle ()
	{
		currentVehicle = null;

		characterOnVehicle = false;

		setExtraMinDistanceState (false, 0);

		AINavMeshManager.setExtraMinDistanceState (false, 0);
	}

	public void setCheckTargetsOnTriggerPausedState (bool state)
	{
		checkTargetsOnTriggerPaused = state;
	}

	public void setPauseTargetDetectionOnTriggerDuration (float newDuration)
	{
		checkTargetsOnTriggerPausedDuration = newDuration;

		lastTimeCheckTargetsOnTriggerPaused = Time.time;
	}

	public void getOffFromVehicle ()
	{
		if (characterOnVehicle) {
			if (playerControllerManager.canCharacterGetOnVehicles ()) {
				usingDevicesManager.useDevice ();

				AINavMeshManager.pauseAI (false);
			}

			removeStateOnVehicle ();
		}
	}

	public void takeObject (GameObject newObject)
	{
		if (usingDevicesManager != null) {
			usingDevicesManager.addDeviceToList (newObject);

			usingDevicesManager.getclosestDevice (false, false);
		
			usingDevicesManager.useCurrentDevice (newObject);
		}
	}

	void checkAIParent ()
	{
		if (!AIParentChecked) {
			if (AIParent != null) {
				AITransform.SetParent (null);

				GameObject playerCameraGameObject = playerControllerManager.getPlayerCameraGameObject ();

				playerCameraGameObject.transform.SetParent (null);

				AIParent.rotation = Quaternion.identity;

				AITransform.SetParent (AIParent);

				playerCameraGameObject.transform.SetParent (AIParent);

				if (resetCameraRotationOnStart) {
					playerCameraGameObject.transform.rotation = Quaternion.identity;
				}

				AIParentChecked = true;
			}
		}
	}

	public void setUseAbilitiesBehaviorState (bool state)
	{
		useAbilitiesBehavior = state;

		if (abilitiesAIBehavior != null) {
			abilitiesAIBehavior.setSystemActiveState (state);
		}
	}

	public bool isAIBehaviorAttackInProcess ()
	{
		if (currentAIBehaviorAssigned) {
			return currentAIBehavior.isAIBehaviorAttackInProcess ();
		}

		return false;
	}

	public void setExtraFieldOfViewRadiusOnSpotFromEditor (float newValue)
	{
		extraFieldOfViewRadiusOnSpot = newValue;

		updateComponent ();
	}

	public void setAvoidEnemiesStateFromEditor (bool state)
	{
		avoidEnemies = state;

		updateComponent ();
	}

	public void setVisonRangeValueFromEditor (float newValue)
	{
		visionRange = newValue;

		updateComponent ();
	}

	public void setUseVisionRangeValueFromEditor (bool state)
	{
		useVisionRange = state;

		updateComponent ();
	}

	public void setCanHearNoiseStateFromEditor (bool state)
	{
		canHearNoises = state;

		updateComponent ();
	}

	public void setWanderEnabledStateFromEditor (bool state)
	{
		wanderEnabled = state;

		updateComponent ();
	}

	public void setFollowPartnerOnTriggerFromEditor (bool state)
	{
		followPartnerOnTrigger = state;

		updateComponent ();
	}

	public void setFollowPartnerOnTriggerState (bool state)
	{
		followPartnerOnTrigger = state;
	}

	public void setUseNoiseReactionInfoState (bool state)
	{
		useNoiseReactionInfo = state;
	}

	public void setUseNoiseReactionInfoStateFromEditor (bool state)
	{
		setUseNoiseReactionInfoState (state);

		updateComponent ();
	}

	public void setUseAbilitiesBehaviorStateFromEditor (bool state)
	{
		useAbilitiesBehavior = state;

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
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
			Gizmos.color = gizmoColor;

			Gizmos.DrawWireSphere (transform.position, alertFactionRadius);
		}
	}

	[System.Serializable]
	public class enemyInfo
	{
		public string Name;
		public GameObject enemyGameObject;
		public Transform enemyTransform;
		public Transform placeToShoot;
		public healthManagement enemeyHealthManagement;
		public playerController enemyPlayerController;

		public float lastTimeEnemyVisible;

		public float characterRadius;
		public bool isCharacter;
		public bool isVehicle;
		public bool isOtherType;

		public List<GameObject> vehicleParts = new List<GameObject> ();
	}

	[System.Serializable]
	public class wanderingInfo
	{
		public string Name;

		public bool isCurrentState;

		public UnityEvent eventOnWanderPositionReached;
	}
}