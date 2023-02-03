using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIAbilitiesSystemBrain : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool systemEnabled = true;

	public float currentMinDistanceToAttack = 7;

	[Space]
	[Header ("Attack Settings")]
	[Space]

	public bool attackEnabled;

	public Vector2 randomAttackRangeRate;

	[Space]
	[Header ("Abilities Settings")]
	[Space]

	public bool changeAbilityAfterTimeEnabled;

	public bool changeAbilityRandomly;

	public Vector2 randomChangeAbilityRate;

	[Space]
	[Header ("Abilities List")]
	[Space]

	public int currentAbilityIndex;

	[Space]

	public List<AIAbilityInfo> AIAbilityInfoList = new List<AIAbilityInfo> ();

	[Space]
	[Space]

	public int currentAbilityStateIndex;

	[Space]

	public List<AIAbilityStatesInfo> AIAbilityStatesInfoList = new List<AIAbilityStatesInfo> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool changeWaitToActivateAttackEnabled = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool systemActive;

	public bool waitingForAttackActive;
	float currentRandomTimeToAttack;

	public bool canUseAttackActive;

	public bool attackStatePaused;

	public bool insideMinDistanceToAttack;

	public float currentAttackRate;

	public bool abilityInProcess;

	public bool onSpotted;

	public bool checkingToChangeAbilityActive;

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

	public findObjectivesSystem mainFindObjectivesSystem;
	public AINavMesh mainAINavmeshManager;
	public playerAbilitiesSystem mainPlayerAbilitiesSystem;
	public playerController mainPlayerController;

	float lastTimeAttack;

	int currentAttackTypeIndex;

	int currentAttackIndex;

	int currentAttackTypeToAlternateIndex;

	float currentPauseAttackStateDuration;
	float lastTimeAttackPauseWithDuration;

	float randomWaitTime;

	float lastTimeAttackActivated;

	float currentPathDistanceToTarget;

	bool AIPaused;

	AIAbilityInfo currentAIAbilityInfo;

	Coroutine abilityCoroutine;
	Coroutine pauseMainBehaviorCoroutine;

	float lastTimeChangeAbility;

	float timeToChangeAbility;

	AIAbilityStatesInfo currentAIAbilityStatesInfo;

	void Start ()
	{
		if (systemActive) {
			systemActive = false;

			setSystemActiveState (true);
		}
	}

	public void updateAI ()
	{
		if (systemActive) {
			AIPaused = mainFindObjectivesSystem.isAIPaused ();

			if (!AIPaused) {
				
			}
		}
	}

	public void resetStates ()
	{


	}

	public void updateInsideMinDistance (bool newInsideMinDistanceToAttack)
	{
		insideMinDistanceToAttack = newInsideMinDistanceToAttack;

		if (insideMinDistanceToAttack) {

		} else {
			
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

		currentPathDistanceToTarget = mainFindObjectivesSystem.currentPathDistanceToTarget;

		checkAttackState ();
	}

	public void checkAttackState ()
	{
		if (!attackEnabled) {
			return;
		}

		insideMinDistanceToAttack = mainFindObjectivesSystem.insideMinDistanceToAttack;

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

		if (waitToActivateAttackActive) {
			return;
		}


		if (!canUseAttackActive) {
			return;
		}

		if (mainFindObjectivesSystem.isOnSpotted ()) {
			if (!onSpotted) {
				lastTimeAttackActivated = Time.time;

				onSpotted = true;
			}
		} else {
			if (onSpotted) {

				onSpotted = false;

				checkingToChangeAbilityActive = false;
			}
		}
			
		if (onSpotted) {
			if (changeAbilityAfterTimeEnabled && !abilityInProcess) {
				if (!checkingToChangeAbilityActive) {
					lastTimeChangeAbility = Time.time;

					checkingToChangeAbilityActive = true;

					timeToChangeAbility = Random.Range (randomChangeAbilityRate.x, randomChangeAbilityRate.y);
				} else {
					if (Time.time > lastTimeChangeAbility + timeToChangeAbility) {
						setNextAbility ();
					}
				}
			}

			if (Time.time > currentAttackRate + lastTimeAttackActivated && !abilityInProcess) {
				if (currentPathDistanceToTarget <= currentMinDistanceToAttack &&
				    mainFindObjectivesSystem.checkIfMinimumAngleToAttack () &&
				    !mainPlayerController.isActionActive () &&
				    mainPlayerController.canPlayerMove ()) {

					bool canActivateAbilityResult = true;

					if (currentAIAbilityInfo.useCustomAbilityDistance) {
						if (currentPathDistanceToTarget > currentAIAbilityInfo.customAbilityDistance) {
							canActivateAbilityResult = false;
						}
					}

					if (canActivateAbilityResult) {
						if (mainFindObjectivesSystem.isAIBehaviorAttackInProcess ()) {
							if (showDebugPrint) {
								print ("attack in process in current main behavior, cancelling ability action");
							}

							canActivateAbilityResult = false;
						}
					}

					if (canActivateAbilityResult) {
						attackTarget ();

						if (showDebugPrint) {
							print ("activate ability attack " + currentAttackRate);
						}

						if (currentAIAbilityInfo.useCustomRandomAttackRangeRate) {
							currentAttackRate = Random.Range (currentAIAbilityInfo.customRandomAttackRangeRate.x, currentAIAbilityInfo.customRandomAttackRangeRate.y);
						} else {
							currentAttackRate = Random.Range (randomAttackRangeRate.x, randomAttackRangeRate.y);
						}
					}
				}
			}
		}
	}

	public void setChangeAbilityAfterTimeEnabledState (bool state)
	{
		changeAbilityAfterTimeEnabled = state;
	}

	public void setNextAbility ()
	{
		bool newAbilityIndexFound = false;

		int newAbilityIndex = -1;

		if (changeAbilityRandomly) {
			while (!newAbilityIndexFound) {

				newAbilityIndex = Random.Range (0, AIAbilityInfoList.Count);

				if (newAbilityIndex != currentAbilityIndex) {
					if (AIAbilityInfoList [newAbilityIndex].useAbilityEnabled) {
						newAbilityIndexFound = true;
					}
				}
			}
		} else {
			newAbilityIndex = currentAbilityIndex;

			while (!newAbilityIndexFound) {
				newAbilityIndex++;

				if (newAbilityIndex >= AIAbilityInfoList.Count) {
					newAbilityIndex = 0;
				}

				if (AIAbilityInfoList [newAbilityIndex].useAbilityEnabled) {
					newAbilityIndexFound = true;
				}
			}
		}

		if (newAbilityIndex > -1) {
			setNewAbilityByName (AIAbilityInfoList [newAbilityIndex].Name);

			if (showDebugPrint) {
				print ("changing ability to " + AIAbilityInfoList [newAbilityIndex].Name);
			}
		}

		checkingToChangeAbilityActive = false;
	}

	public void updateAIAttackState (bool newCanUseAttackActiveState)
	{
		canUseAttackActive = newCanUseAttackActiveState;
	}

	public void setSystemEnabledState (bool state)
	{
		if (systemEnabled == state) {
			return;
		}

		systemEnabled = state;

		setSystemActiveState (systemEnabled);
	}

	public void setSystemActiveState (bool state)
	{
		if (!systemEnabled) {
			return;
		}

		if (systemActive == state) {
			return;
		}

		systemActive = state;

		if (systemActive) {
			lastTimeAttackActivated = Time.time;

			setNewAbilityByName (AIAbilityInfoList [currentAbilityIndex].Name);

			setNewAbilityStateByName (AIAbilityStatesInfoList [currentAbilityStateIndex].Name);

			if (currentAIAbilityInfo.useCustomRandomAttackRangeRate) {
				currentAttackRate = Random.Range (currentAIAbilityInfo.customRandomAttackRangeRate.x, currentAIAbilityInfo.customRandomAttackRangeRate.y);
			} else {
				currentAttackRate = Random.Range (randomAttackRangeRate.x, randomAttackRangeRate.y);
			}

		}

		onSpotted = false;

		checkingToChangeAbilityActive = false;

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
	}

	public void resetBehaviorStates ()
	{
		resetStates ();

		waitingForAttackActive = false;



		insideMinDistanceToAttack = false;
	}

	public void attackTarget ()
	{
		if (showDebugPrint) {
			print ("activate ability attack");
		}

		stopPauseMainAttackBehaviorCoroutine ();

		pauseMainBehaviorCoroutine = StartCoroutine (pauseMainAttackBehaviorCoroutine ());

		abilityInProcess = true;

		if (currentAIAbilityInfo.abilityInputHoldType || currentAIAbilityInfo.abilityInputToggleType) {
			stopAbilityInputHoldCoroutine ();

			abilityCoroutine = StartCoroutine (abilityInputHoldCoroutine ());
		} else {
			if (currentAIAbilityInfo.sendSignalToActivateAbilityEnabled) {
				mainPlayerAbilitiesSystem.inputSelectAndPressDownNewAbility (currentAIAbilityInfo.Name);
			}

			if (currentAIAbilityInfo.useEventsOnAbility) {
				currentAIAbilityInfo.eventOnAbilityStart.Invoke ();
			}
		}
	}

	void stopAbilityInputHoldCoroutine ()
	{
		if (abilityCoroutine != null) {
			StopCoroutine (abilityCoroutine);
		}
	}

	IEnumerator abilityInputHoldCoroutine ()
	{
		if (currentAIAbilityInfo.sendSignalToActivateAbilityEnabled) {
			mainPlayerAbilitiesSystem.inputSelectAndPressDownNewSeparatedAbility (currentAIAbilityInfo.Name);
		}

		if (currentAIAbilityInfo.useEventsOnAbility) {
			currentAIAbilityInfo.eventOnAbilityStart.Invoke ();
		}

		yield return new WaitForSeconds (currentAIAbilityInfo.timeToHoldAndReleaseAbilityInput);

		if (currentAIAbilityInfo.sendSignalToActivateAbilityEnabled) {
			if (currentAIAbilityInfo.abilityInputHoldType) {
				mainPlayerAbilitiesSystem.inputSelectAndPressUpNewSeparatedAbility ();
			}

			if (currentAIAbilityInfo.abilityInputToggleType) {
				mainPlayerAbilitiesSystem.inputSelectAndPressDownNewSeparatedAbility (currentAIAbilityInfo.Name);
			}
		}

		if (currentAIAbilityInfo.useEventsOnAbility) {
			currentAIAbilityInfo.eventOnAbilityEnd.Invoke ();
		}

		yield return null;
	}

	void stopPauseMainAttackBehaviorCoroutine ()
	{
		if (pauseMainBehaviorCoroutine != null) {
			StopCoroutine (pauseMainBehaviorCoroutine);
		}

		mainFindObjectivesSystem.setBehaviorStatesPausedState (false);

		lastTimeAttackActivated = Time.time;

		abilityInProcess = false;
	}

	IEnumerator pauseMainAttackBehaviorCoroutine ()
	{
		mainFindObjectivesSystem.setBehaviorStatesPausedState (true);

		yield return new WaitForSeconds (currentAIAbilityInfo.abilityDuration);

		mainFindObjectivesSystem.setBehaviorStatesPausedState (false);

		abilityInProcess = false;

		lastTimeAttackActivated = Time.time;

		yield return null;
	}

	public void resetAttackState ()
	{
		
	}


	public void disableOnSpottedState ()
	{

	}

	public void setNewAbilityByName (string abilityName)
	{
		if (!systemEnabled) {
			return;
		}

		int newAbilityIndex = AIAbilityInfoList.FindIndex (s => s.Name.Equals (abilityName));

		if (newAbilityIndex > -1) {
			if (currentAIAbilityInfo != null) {
				currentAIAbilityInfo.isCurrentAbility = false;
			}

			currentAIAbilityInfo = AIAbilityInfoList [newAbilityIndex];

			currentAIAbilityInfo.isCurrentAbility = true;

			currentAbilityIndex = newAbilityIndex;
		}
	}

	public void setNewMinDistanceToAttack (float newValue)
	{
		currentMinDistanceToAttack = newValue;
	}

	public void setNewAbilityStateByName (string abilityStateName)
	{
		if (!systemEnabled) {
			return;
		}

		int newAbilityStateIndex = AIAbilityStatesInfoList.FindIndex (s => s.Name.Equals (abilityStateName));

		if (newAbilityStateIndex > -1) {
			if (currentAIAbilityStatesInfo != null) {
				currentAIAbilityStatesInfo.isCurrentState = false;
			}

			currentAIAbilityStatesInfo = AIAbilityStatesInfoList [newAbilityStateIndex];

			currentAIAbilityStatesInfo.isCurrentState = true;

			currentMinDistanceToAttack = currentAIAbilityStatesInfo.minDistanceToAttack;

			currentAbilityStateIndex = newAbilityStateIndex;
		}
	}

	[System.Serializable]
	public class AIAbilityInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;

		public bool useAbilityEnabled = true;

		public bool isCurrentAbility;

		public bool sendSignalToActivateAbilityEnabled = true;

		[Space]
		[Header ("Distance Settings")]
		[Space]

		public bool useCustomAbilityDistance;
		public float customAbilityDistance;

		[Space]
		[Header ("Ability Settings")]
		[Space]

		public bool abilityInputHoldType;

		public bool abilityInputToggleType;

		public float timeToHoldAndReleaseAbilityInput;

		public float abilityDuration;

		[Space]
		[Header ("Custom Random Attack Range Settings")]
		[Space]

		public bool useCustomRandomAttackRangeRate;

		public Vector2 customRandomAttackRangeRate;

		[Space]
		[Header ("Events Settings")]
		[Space]

		public bool useEventsOnAbility;

		public UnityEvent eventOnAbilityStart;

		public UnityEvent eventOnAbilityEnd;
	}

	[System.Serializable]
	public class AIAbilityStatesInfo
	{
		public string Name;

		public float minDistanceToAttack = 7;

		public bool isCurrentState;
	}
}
