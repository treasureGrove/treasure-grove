using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceTravelStationListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistancePlayerTravelStationInfo> playerTravelStationList = new List<persistancePlayerTravelStationInfo> ();
}

[System.Serializable]
public class persistancePlayerTravelStationInfo
{
	public int playerID;
	public List<persistanceTravelStationInfo> travelStationList = new List<persistanceTravelStationInfo> ();
}

[System.Serializable]
public class persistanceTravelStationInfo
{
	public int sceneNumberToLoad;

	public int levelManagerIDToLoad;

	public persistanceTravelStationInfo ()
	{

	}
}