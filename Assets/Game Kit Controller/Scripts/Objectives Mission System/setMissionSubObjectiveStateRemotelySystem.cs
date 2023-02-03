using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setMissionSubObjectiveStateRemotelySystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool setMissionInfoEnabled = true;

	public int missionID;

	public int missionScene = -1;

	public string subObjectiveName;

	public bool useObjectiveCounterInsteadOfList;

	public void addSubObjectiveCompleteRemotely (string customSubObjectiveName)
	{
		sendMissionInfo (customSubObjectiveName);
	}

	public void addSubObjectiveCompleteRemotely ()
	{
		sendMissionInfo (subObjectiveName);
	}

	public void sendMissionInfo (string newSubObjectiveName)
	{
		if (!setMissionInfoEnabled) {
			return;
		}

		objectiveManager mainObjectiveManager = FindObjectOfType<objectiveManager> ();

		if (mainObjectiveManager != null) {
			if (useObjectiveCounterInsteadOfList) {
				mainObjectiveManager.increaseObjectiveCounterRemotely (missionScene, missionID);
			} else {
				mainObjectiveManager.addSubObjectiveCompleteRemotely (newSubObjectiveName, missionScene, missionID);
			}
		}
	}

	public void setMissionID (int newValue)
	{
		missionID = newValue;
	}

	public void setSubObjectiveName (string newValue)
	{
		subObjectiveName = newValue;
	}

	public void setMissionInfoEnabledState (bool state)
	{
		setMissionInfoEnabled = state;
	}

	public void setMissionInfoEnabledStateFromEditor (bool state)
	{
		setMissionInfoEnabledState (state);

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("update mission state remotely state ", gameObject);
	}
}
