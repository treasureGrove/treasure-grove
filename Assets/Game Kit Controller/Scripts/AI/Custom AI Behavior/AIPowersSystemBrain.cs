using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIPowersSystemBrain : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool systemEnabled = true;

	[Space]
	[Header ("Attack Settings")]
	[Space]

	public bool attackEnabled;

	public float attackRate = 0.17f;

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
	[Header ("Other Settings")]
	[Space]

	public bool changeWaitToActivateAttackEnabled = true;

	public bool activateRandomWalkIfWaitToActivateAttackActive;

	public bool checkIfAttackInProcessBeforeCallingNextAttack = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool systemActive;

	public bool aimingPower;

	public bool waitingForAttackActive;
	float currentRandomTimeToAttack;

	public bool walkActive;
	public bool waitingWalkActive;

	public bool waitingRollActive;

	public bool canUseAttackActive;

	public bool attackStatePaused;

	public bool insideMinDistanceToAttack;

	public bool behaviorStatesPaused;

	public bool waitToActivateAttackActive;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnCombatActive;
	public UnityEvent eventOnCombatActive;
	public UnityEvent eventOnCombatDeactivate;

	[Space]
	[Header ("Components")]
	[Space]

	public otherPowers mainOtherPowers;
	public dashSystem mainDashSystem;
	public findObjectivesSystem mainFindObjectivesSystem;
	public AINavMesh mainAINavmeshManager;

	float lastTimeAttack;

	int currentAttackTypeIndex;

	int currentAttackIndex;

	int currentAttackTypeToAlternateIndex;


	float lastTimeRollActive;

	float lastTimeWaitRollActive;

	float currentRollWaitTime;


	float lastTimeWaitWalkActive;
	float currentWalkTime;
	float lastTimeWalkActive;

	float currentWalkDuration;
	float currentWalkRadius;

	bool rollCoolDownActive;

	float currentPauseAttackStateDuration;
	float lastTimeAttackPauseWithDuration;

	float randomWaitTime;

	float lastTimeAttackAtDistance;

	float currentPathDistanceToTarget;
	float minDistanceToAim;
	float minDistanceToShoot;

	bool AIPaused;

	bool cancelCheckAttackState;


	public void updateAI ()
	{
		if (systemActive) {
			AIPaused = mainFindObjectivesSystem.isAIPaused ();

			if (!AIPaused) {
				if (walkActive) {
					if (Time.time > lastTimeWalkActive + currentWalkDuration || mainFindObjectivesSystem.getRemainingDistanceToTarget () < 0.5f) {
						resetRandomWalkState ();
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

	public void resetRollState ()
	{
		waitingRollActive = false;

		lastTimeRollActive = Time.time;
	}

	public void resetStates ()
	{
		resetRandomWalkState ();

		resetRollState ();
	}

	public void checkIfResetStatsOnRandomWalk ()
	{
		if (walkActive) {
			resetStates ();
		}
	}

	public void checkRollState ()
	{
		if (rollEnabled) {

			if (walkActive) {
				return;
			}

			if (!insideMinDistanceToAttack) {
				resetRollState ();

				lastTimeRollActive = 0;

				return;
			}

			if (waitingRollActive) {
				if (Time.time > lastTimeWaitRollActive + currentRollWaitTime) {

					int randomRollMovementDirection = Random.Range (0, rollMovementDirectionList.Count - 1);

					mainDashSystem.activateDashStateWithCustomDirection (rollMovementDirectionList [randomRollMovementDirection]);

					resetRollState ();
				}
			} else {
				if (Time.time > lastTimeRollActive + randomWaitTime) {
					currentRollWaitTime = Random.Range (randomRollWaitTime.x, randomRollWaitTime.y);

					lastTimeWaitRollActive = Time.time;

					waitingRollActive = true;

					randomWaitTime = Random.Range (0.1f, 0.5f);
				}
			}
		}
	}

	public void checkWalkState ()
	{
		if (randomWalkEnabled) {

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
		insideMinDistanceToAttack = newInsideMinDistanceToAttack;

		if (mainFindObjectivesSystem.attackAlwaysOnPlace) {
			insideMinDistanceToAttack = true;
		}

		if (insideMinDistanceToAttack) {
			
		} else {
			if (aimingPower) {
				setAimState (false);
			}
		}
	}

	void updateLookAtTargetIfBehaviorPaused ()
	{
		if (mainFindObjectivesSystem.attackTargetDirectly) {
			mainFindObjectivesSystem.setLookingAtTargetPositionState (true);

			mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();
		} else {
			currentPathDistanceToTarget = mainFindObjectivesSystem.currentPathDistanceToTarget;
			minDistanceToAim = mainFindObjectivesSystem.minDistanceToAim;
			minDistanceToShoot = mainFindObjectivesSystem.minDistanceToShoot; 

			bool useHalfMinDistance = mainAINavmeshManager.useHalfMinDistance;

			if (useHalfMinDistance) {
				mainFindObjectivesSystem.setLookingAtTargetPositionState (false);

				cancelCheckAttackState = true;
			} else {

				if (currentPathDistanceToTarget <= minDistanceToAim) {
					mainFindObjectivesSystem.setLookingAtTargetPositionState (true);

					mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();
				} else {
					if (currentPathDistanceToTarget >= minDistanceToAim + 1.5f) {

						mainFindObjectivesSystem.setLookingAtTargetPositionState (false);

						cancelCheckAttackState = true;
					} else {
						if (mainFindObjectivesSystem.isLookingAtTargetPosition ()) {
							mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();
						}
					}
				}
			}
		}
	}

	public void updateBehavior ()
	{
		if (!systemActive) {
			return;
		}

		if (AIPaused) {
			return;
		}

		if (behaviorStatesPaused) {
			updateLookAtTargetIfBehaviorPaused ();

			return;
		}

		checkWalkState ();

		if (walkActive) {
			return;
		}

		checkRollState ();

		if (rollEnabled) {
			if (Time.time < lastTimeRollActive + minWaitTimeAfterRollActive) {
				return;
			}
		}

		cancelCheckAttackState = false;

		if (mainFindObjectivesSystem.attackTargetDirectly) {
			mainFindObjectivesSystem.setLookingAtTargetPositionState (true);

			mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();

			if (!aimingPower) {
				setAimState (true);
			}

			if (aimingPower) {
				if (mainFindObjectivesSystem.checkIfMinimumAngleToAttack () &&
				    !mainOtherPowers.isActionActiveInPlayer () &&
				    mainOtherPowers.canPlayerMove ()) {
					shootTarget ();
				}
			}
		} else {
			currentPathDistanceToTarget = mainFindObjectivesSystem.currentPathDistanceToTarget;
			minDistanceToAim = mainFindObjectivesSystem.minDistanceToAim;
			minDistanceToShoot = mainFindObjectivesSystem.minDistanceToShoot; 

			bool useHalfMinDistance = mainAINavmeshManager.useHalfMinDistance;

			if (useHalfMinDistance) {
				if (aimingPower) {
					setAimState (false);
				}

				mainFindObjectivesSystem.setLookingAtTargetPositionState (false);

				cancelCheckAttackState = true;
			} else {

				if (currentPathDistanceToTarget <= minDistanceToAim) {
					if (!aimingPower) {
						setAimState (true);
					}

					mainFindObjectivesSystem.setLookingAtTargetPositionState (true);

					mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();
				} else {
					if (currentPathDistanceToTarget >= minDistanceToAim + 1.5f) {
						if (aimingPower) {
							setAimState (false);
						}

						mainFindObjectivesSystem.setLookingAtTargetPositionState (false);

						cancelCheckAttackState = true;
					} else {
						if (mainFindObjectivesSystem.isLookingAtTargetPosition ()) {
							mainFindObjectivesSystem.lookAtCurrentPlaceToShoot ();
						}
					}
				}
			}
		}

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
	}

	public void checkAttackState ()
	{
		if (!attackEnabled) {
			return;
		}

		if (!insideMinDistanceToAttack) {
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


		if (!canUseAttackActive) {
			return;
		}

		if (!aimingPower && !cancelCheckAttackState) {
			setAimState (true);
		}

		if (Time.time > attackRate + lastTimeAttackAtDistance && mainOtherPowers.canPlayerMove ()) {
			if (aimingPower) {
				if (!mainOtherPowers.isAimingPower ()) {
					setAimState (true);
				}

				if (currentPathDistanceToTarget <= minDistanceToShoot &&
				    mainFindObjectivesSystem.checkIfMinimumAngleToAttack () &&
				    !mainOtherPowers.isActionActiveInPlayer ()) {

					shootTarget ();
				}
			}

			lastTimeAttackAtDistance = Time.time;
		}
	}

	public void updateAIAttackState (bool newCanUseAttackActiveState)
	{
		canUseAttackActive = newCanUseAttackActiveState;
	}

	public void setSystemActiveState (bool state)
	{
		if (!systemEnabled) {
			return;
		}

		systemActive = state;

		checkEventsOnCombatStateChange (systemActive);
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

	public void resetBehaviorStates ()
	{
		resetStates ();

		waitingForAttackActive = false;

		setAimState (false);

		insideMinDistanceToAttack = false;
	}

	public void setAimState (bool state)
	{
		if (showDebugPrint) {
			print ("setting aim active state " + state);
		}

		if (state) {
			if (!aimingPower) {
				startAimPower ();

				lastTimeAttackAtDistance = Time.time;
			}
		} else {
			if (aimingPower) {
				stopAimPower ();

				stopShootPower ();
			}
		}

		aimingPower = state;
	}

	public void shootTarget ()
	{
		startShootPower ();
		
		holdShootPower ();
	}

	public void resetAttackState ()
	{
		aimingPower = false;
	}

	public void stopAim ()
	{
		if (aimingPower) {
			setAimState (false);
		}
	}

	public void disableOnSpottedState ()
	{

	}

	public void startAimPower ()
	{
		if (!aimingPower) {
			mainOtherPowers.inputSetAimPowerState (true);
		}
	}

	public void stopAimPower ()
	{
		if (aimingPower) {
			mainOtherPowers.inputSetAimPowerState (false);
		}
	}

	public void startShootPower ()
	{
		mainOtherPowers.inputHoldOrReleaseShootPower (true);
	}

	public void holdShootPower ()
	{
		mainOtherPowers.inputHoldShootPower ();
	}

	public void stopShootPower ()
	{
		mainOtherPowers.inputHoldOrReleaseShootPower (false);
	}

	public void setBehaviorStatesPausedState (bool state)
	{
		behaviorStatesPaused = state;

		if (behaviorStatesPaused) {
			resetAttackState ();
		}
	}
}
