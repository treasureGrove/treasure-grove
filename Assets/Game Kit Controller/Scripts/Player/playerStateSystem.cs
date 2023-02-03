using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerStateSystem : MonoBehaviour
{
	public bool playerStatesEnabled = true;

	public bool activateStateOnStart;
	public string stateToActivateOnStart;

	public List<playerStateInfo> playerStateInfoList = new List<playerStateInfo> ();

	playerStateInfo currentPlayerStateInfo;

	public string currentStateName;

	void Awake ()
	{
		if (playerStatesEnabled && activateStateOnStart) {
			setPlayerState (stateToActivateOnStart);
		}
	}

	public void setPlayerState (string stateName)
	{
		if (!playerStatesEnabled) {
			return;
		}
	
		int stateToUseIndex = playerStateInfoList.FindIndex (s => s.Name == stateName);

		if (stateToUseIndex > -1) {
			if (!playerStateInfoList [stateToUseIndex].stateEnabled) {
				print ("State " + playerStateInfoList [stateToUseIndex].Name + " not enabled");

				return;
			} else {
				if (!checkIfStateCanChange (stateToUseIndex)) {
					return;
				}
			}

			for (int i = 0; i < playerStateInfoList.Count; i++) {
				if (playerStateInfoList [i].stateEnabled) {
					if (i != stateToUseIndex) {
			
						if (playerStateInfoList [i].stateActive) {
							playerStateInfoList [i].eventOnStateEnd.Invoke ();

							if (playerStateInfoList [i].stateDurationCoroutine != null) {
								StopCoroutine (playerStateInfoList [i].stateDurationCoroutine);

								playerStateInfoList [i].useTemporalStateDuration = false;
							}
						}

						playerStateInfoList [i].stateActive = false;
					}
				}
			}

			currentPlayerStateInfo = playerStateInfoList [stateToUseIndex];

			currentStateName = currentPlayerStateInfo.Name;

			currentPlayerStateInfo.stateActive = true;

			currentPlayerStateInfo.eventOnStateStart.Invoke ();

			if (currentPlayerStateInfo.stateDurationCoroutine != null) {
				StopCoroutine (currentPlayerStateInfo.stateDurationCoroutine);
			}

			if (currentPlayerStateInfo.useTemporalStateDuration) {
				
				currentPlayerStateInfo.stateDurationCoroutine = StartCoroutine (activateStateCoroutine (currentPlayerStateInfo, currentPlayerStateInfo.temporalStateDuration));
			} else {
				if (currentPlayerStateInfo.useStateDuration) {

					currentPlayerStateInfo.stateDurationCoroutine = StartCoroutine (activateStateCoroutine (currentPlayerStateInfo, currentPlayerStateInfo.stateDuration));
				}
			}

//			print ("State " + currentPlayerStateInfo.Name + " set as current state active");
		}
	}

	public bool checkIfStateCanChange (int stateIndexToCheck)
	{
		for (int i = 0; i < playerStateInfoList.Count; i++) {
			if (i != stateIndexToCheck && playerStateInfoList [i].stateEnabled && playerStateInfoList [i].stateActive &&
			    (!playerStateInfoList [i].canBeInterruptedWhileActive && playerStateInfoList [i].statePriority < playerStateInfoList [stateIndexToCheck].statePriority)) {
				print ("State " + playerStateInfoList [i].Name + " can't be interrupted or has a higher priority");

				return false;
			}
		}

		return true;
	}

	IEnumerator activateStateCoroutine (playerStateInfo newState, float stateDuration)
	{
		yield return new WaitForSeconds (stateDuration);
	
		newState.stateActive = false;

		newState.eventOnStateEnd.Invoke ();

		newState.useTemporalStateDuration = false;
	}

	public void setUseTemporalDurationOnState (string stateName)
	{
		int indexOfSeparator = stateName.IndexOf ("_");

		string newStateName = stateName.Substring (0, indexOfSeparator);

		int stateDurationLength = stateName.Length - newStateName.Length - 1;

		string newStateDurationString = stateName.Substring (indexOfSeparator + 1, stateDurationLength);

		float newDuration = float.Parse (newStateDurationString);

//		print (newStateName + " " + newDuration);

		int stateToUseIndex = playerStateInfoList.FindIndex (s => s.Name == newStateName);

		if (stateToUseIndex > -1) {
			playerStateInfo newPlayerStateInfo = playerStateInfoList [stateToUseIndex];

			newPlayerStateInfo.useTemporalStateDuration = true;
			newPlayerStateInfo.temporalStateDuration = newDuration;
		}
	}
		
	public void setStateEnabledState (bool state, string stateName)
	{
		for (int i = 0; i < playerStateInfoList.Count; i++) {
			if (playerStateInfoList [i].Name.Equals (stateName)) {
				playerStateInfoList [i].stateEnabled = state;

				return;
			}
		}
	}

	public void setStateEnabledState (string stateName)
	{
		setStateEnabledState (true, stateName);
	}

	public void setStateDisabledState (string stateName)
	{
		setStateEnabledState (false, stateName);
	}

	public void setStateEnabledStateFromEditor (string stateName)
	{
		setStateEnabledState (true, stateName);

		updateComponent ();
	}

	public void setStateDisabledStateFromEditor (string stateName)
	{
		setStateEnabledState (false, stateName);
	
		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Player States System " + gameObject.name, gameObject);
	}

	[System.Serializable]
	public class playerStateInfo
	{
		public string Name;
		public bool stateEnabled = true;
		public bool stateActive;
		public int statePriority;
		public bool canBeInterruptedWhileActive = true;
		public UnityEvent eventOnStateStart;
		public UnityEvent eventOnStateEnd;
		public bool useStateDuration;
		public float stateDuration;
		public Coroutine stateDurationCoroutine;

		public bool useTemporalStateDuration;
		public float temporalStateDuration;
	}
}
