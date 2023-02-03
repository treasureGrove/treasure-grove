using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class objectiveStationSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useCharacterObjectivePanel;

	public int currentCharacterObjectiveIndex;

	public bool startMissionAfterPressingClosingStation;
	public bool useDelayToStartMission;
	public float delayToStartMission;

	[Space]
	[Header ("Objective Info List Settings")]
	[Space]

	public List<objectiveInfo> objectiveInfoList = new List<objectiveInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool usingObjectiveStation;

	public bool firstTimeMissionsStateChecked;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnMissionAvailables;
	public UnityEvent eventOnAllMissionsComplete;

	[Space]
	[Header ("Explanation")]
	[Space]

	[TextArea (5, 20)]public string explanation = "This is a station system which " +
	                                              "can be used as a physical mission board or just or just an invisible element to configure different missions which are maybe activate by dialog or trigger." +
	                                              "\n\n" + "" + "Each station has a list of missions, which are the one which contains the missions to achieve, tasks, and rewards and events to activate.";

	GameObject currentPlayer;

	playerComponentsManager mainPlayerComponentsManager;

	objectiveStationUISystem currentObjectiveStationUISystem;

	//Check the mission state, to see if they are already complete and with the reward obtained (in case there is a reward to obtain)
	public void checkMissionsState ()
	{
		for (int i = 0; i < objectiveInfoList.Count; i++) {

			if (objectiveInfoList [i].mainObjectiveEventSystem != null) {
				if (!objectiveInfoList [i].mainObjectiveEventSystem.isObjectiveComplete () || !objectiveInfoList [i].mainObjectiveEventSystem.isRewardsObtained ()) {
					eventOnMissionAvailables.Invoke ();

					firstTimeMissionsStateChecked = true;

					return;
				} else {
					if (!firstTimeMissionsStateChecked) {
						currentCharacterObjectiveIndex = i;
					}
				}
			} else {
				print ("WARNING: There is a list of missions configured in this mission station but the element is empty or missing! " +
				       "Please make sure the mission system is configured properly.");
			}
		}

		firstTimeMissionsStateChecked = true;

		eventOnAllMissionsComplete.Invoke ();
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer != null) {
			mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			currentObjectiveStationUISystem = mainPlayerComponentsManager.getObjectiveStationUISystem ();
		}
	}

	//Enable or disable the mission station, sending the missions configured on it to the player UI system
	public void activateObjectiveStation ()
	{
		usingObjectiveStation = !usingObjectiveStation;

		if (usingObjectiveStation) {
			currentObjectiveStationUISystem.setCurrentObjectiveStationSystem (this);
		} else {
			checkMissionsState ();
		}

		currentObjectiveStationUISystem.openOrCloseObjectiveStationMenu (usingObjectiveStation);
	}

	//Set if the current station system is being used or not
	public void setUsingObjectiveStationState (bool state)
	{
		usingObjectiveStation = state;
	}

	//Get the mission list configured in this station
	public List<objectiveInfo> getObjectiveInfoList ()
	{
		return objectiveInfoList;
	}

	//Increase the current mission index, used usually on character mission, when the player completes a mission, so the next mission available can be obtained
	public void increaseCurrentCharacterObjectiveIndex ()
	{
		currentCharacterObjectiveIndex++;

		if (currentCharacterObjectiveIndex >= objectiveInfoList.Count) {
			currentCharacterObjectiveIndex = objectiveInfoList.Count - 1;
		}
	}

	public void setCurrentCharacterObjectiveIndex (int newIndex)
	{
		currentCharacterObjectiveIndex = newIndex;

		if (currentCharacterObjectiveIndex >= objectiveInfoList.Count) {
			currentCharacterObjectiveIndex = objectiveInfoList.Count - 1;
		}
	}

	//Get the current mission system index
	public int getCurrentCharacterObjectiveIndex ()
	{
		return currentCharacterObjectiveIndex;
	}

	//Check if there are more missions available on this station
	public bool isThereMissionsAvailableOnStation (int indexMissionToCheck)
	{
		if (currentCharacterObjectiveIndex <= indexMissionToCheck) {
			for (int i = 0; i < objectiveInfoList.Count; i++) {
				if (objectiveInfoList [i].mainObjectiveEventSystem != null) {
					if (!objectiveInfoList [i].mainObjectiveEventSystem.isObjectiveComplete ()) {
//						print ("more missions");

						return true;
					}
				}
			}
		}

//		print ("no missions");

		return false;
	}

	public void getAllMissionsSystemOnLevel ()
	{
		//Search all the station systems on the level, so they can be managed here
		objectiveInfoList.Clear ();

		objectiveEventSystem[] newObjectiveEventSystemList = FindObjectsOfType<objectiveEventSystem> ();

		foreach (objectiveEventSystem currentObjectiveEventSystem in newObjectiveEventSystemList) {

			objectiveInfo newObjectiveInfo = new objectiveInfo ();

			newObjectiveInfo.Name = currentObjectiveEventSystem.generalObjectiveName;
			newObjectiveInfo.mainObjectiveEventSystem = currentObjectiveEventSystem;

			objectiveInfoList.Add (newObjectiveInfo);
		}

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Main Station Manager info", gameObject);
	}

	[System.Serializable]
	public class objectiveInfo
	{
		public string Name;

		public objectiveEventSystem mainObjectiveEventSystem;
	}
}
