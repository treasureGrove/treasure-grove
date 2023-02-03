using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistancePlayerMissionsListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistancePlayerMissionInfo> playerMissionsList = new List<persistancePlayerMissionInfo> ();
}

[System.Serializable]
public class persistancePlayerMissionInfo
{
	public int playerID;
	public List<persistanceMissionInfo> missionsList = new List<persistanceMissionInfo> ();
}

[System.Serializable]
public class persistanceMissionInfo
{
	public int missionScene;
	public int missionID;

	public bool disableObjectivePanelOnMissionComplete;

	public bool addObjectiveToPlayerLogSystem;

	public bool missionComplete;
	public bool missionInProcess;
	public bool rewardObtained;
	public bool missionAccepted;

	public string objectiveName;
	public string objectiveDescription;
	public string objectiveFullDescription;
	public string objectiveLocation;
	public string objectiveRewards;

	public List<bool> subObjectiveCompleteList = new List<bool> ();

	public persistanceMissionInfo ()
	{

	}
}