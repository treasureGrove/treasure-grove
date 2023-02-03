using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class saveTravelStationInfo : saveGameInfo
{
	public travelStationUISystem mainTravelStationUISystem;

	List<persistanceTravelStationInfo> persistanceInfoList;


	public override void saveGame (int saveNumber, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		saveGameContent (saveNumber, playerID, currentSaveDataPath, showDebugInfo);
	}

	public override void loadGame (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		loadGameContent (saveNumberToLoad, playerID, currentSaveDataPath, showDebugInfo);
	}

	public override void initializeValuesOnComponent ()
	{
		initializeValues ();
	}

	public void saveGameContent (int currentSaveNumber, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		if (mainTravelStationUISystem == null) {
			return;
		}

		if (!mainTravelStationUISystem.travelStationUIActive) {
			return;
		}

		if (!mainTravelStationUISystem.saveCurrentTravelStationToSaveFile) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving travel stations");
		}

		bool saveLocated = false;
		bool playerLocated = false;

		int saveSlotIndex = -1;
		int listIndex = -1;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		persistancePlayerTravelStationInfo traveStationListToSave = getPersistanceList (playerID, showDebugInfo);

		persistanceTravelStationListBySaveSlotInfo newPersistanceTravelStationListBySaveSlotInfo = new persistanceTravelStationListBySaveSlotInfo ();

		List<persistanceTravelStationListBySaveSlotInfo> infoListToSave = new List<persistanceTravelStationListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToSave = currentData as List<persistanceTravelStationListBySaveSlotInfo>;

			file.Close ();	
		}

		int infoListToSaveCount = infoListToSave.Count;

		for (int j = 0; j < infoListToSaveCount; j++) {
			if (infoListToSave [j].saveNumber == currentSaveNumber) {
				newPersistanceTravelStationListBySaveSlotInfo = infoListToSave [j];
				saveLocated = true;
				saveSlotIndex = j;
			}
		}

		if (saveLocated) {
			int playerTravelStationListCount = newPersistanceTravelStationListBySaveSlotInfo.playerTravelStationList.Count;

			for (int j = 0; j < playerTravelStationListCount; j++) {
				if (newPersistanceTravelStationListBySaveSlotInfo.playerTravelStationList [j].playerID == traveStationListToSave.playerID) {
					playerLocated = true;
					listIndex = j;
				}
			}
		}

		//if the save is located, check if the player id exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLocated) {
				infoListToSave [saveSlotIndex].playerTravelStationList [listIndex].travelStationList = traveStationListToSave.travelStationList;
			} else {
				infoListToSave [saveSlotIndex].playerTravelStationList.Add (traveStationListToSave);
			}
		} else {
			newPersistanceTravelStationListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistanceTravelStationListBySaveSlotInfo.playerTravelStationList.Add (traveStationListToSave);
			infoListToSave.Add (newPersistanceTravelStationListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Open (currentSaveDataPath, FileMode.OpenOrCreate); 
		bf.Serialize (file, infoListToSave);

		file.Close ();
	}

	public void loadGameContent (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		if (mainTravelStationUISystem == null) {
			return;
		}

		if (!mainTravelStationUISystem.travelStationUIActive) {
			return;
		}

		if (!mainTravelStationUISystem.saveCurrentTravelStationToSaveFile) {
			initializeValues ();

			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Loading travel stations");
		}

		persistanceInfoList = new List<persistanceTravelStationInfo> ();

		//need to store and check the current slot saved and the player which is saving, to get that concrete info
		List<persistanceTravelStationListBySaveSlotInfo> infoListToLoad = new List<persistanceTravelStationListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToLoad = currentData as List<persistanceTravelStationListBySaveSlotInfo>;

			file.Close ();	
		}

		if (saveNumberToLoad > -1) {
			persistanceTravelStationListBySaveSlotInfo newPersistanceTravelStationListBySaveSlotInfo = new persistanceTravelStationListBySaveSlotInfo ();

			int infoListToLoadCount = infoListToLoad.Count;

			for (int j = 0; j < infoListToLoadCount; j++) {

				if (infoListToLoad [j].saveNumber == saveNumberToLoad) {
					newPersistanceTravelStationListBySaveSlotInfo = infoListToLoad [j];
				}
			}

			int listIndex = -1;

			int playerTravelStationListCount = newPersistanceTravelStationListBySaveSlotInfo.playerTravelStationList.Count;

			for (int j = 0; j < playerTravelStationListCount; j++) {

				if (newPersistanceTravelStationListBySaveSlotInfo.playerTravelStationList [j].playerID == playerID) {
					listIndex = j;
				}
			}

			if (listIndex > -1) {
				persistanceInfoList.AddRange (newPersistanceTravelStationListBySaveSlotInfo.playerTravelStationList [listIndex].travelStationList);
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Travel Stations Loaded in Save Number " + saveNumberToLoad);
			print ("Number of travel station: " + persistanceInfoList.Count);
		}
			
		loadInfoOnMainComponent ();
	}


	persistancePlayerTravelStationInfo getPersistanceList (int playerID, bool showDebugInfo)
	{
		persistancePlayerTravelStationInfo newPersistancePlayerTravelStationInfo = new persistancePlayerTravelStationInfo ();

		newPersistancePlayerTravelStationInfo.playerID = playerID;

		List<persistanceTravelStationInfo> newPersistanceTravelStationInfoList = new List<persistanceTravelStationInfo> ();

		List<travelStationUISystem.foundTravelStationInfo> foundTravelStationInfoList = mainTravelStationUISystem.foundTravelStationInfoList;

		int foundTravelStationInfoListCount = foundTravelStationInfoList.Count;

		for (int k = 0; k < foundTravelStationInfoListCount; k++) {
			persistanceTravelStationInfo newPersistanceTravelStationInfo = new persistanceTravelStationInfo ();

			travelStationUISystem.foundTravelStationInfo currentStationInfo = foundTravelStationInfoList [k];

			newPersistanceTravelStationInfo.sceneNumberToLoad = currentStationInfo.sceneNumberToLoad;
			newPersistanceTravelStationInfo.levelManagerIDToLoad = currentStationInfo.levelManagerIDToLoad;

			newPersistanceTravelStationInfoList.Add (newPersistanceTravelStationInfo);
		}	

		newPersistancePlayerTravelStationInfo.travelStationList = newPersistanceTravelStationInfoList;

		return newPersistancePlayerTravelStationInfo;
	}


	void loadInfoOnMainComponent ()
	{
		initializeValues ();

		if (persistanceInfoList != null && persistanceInfoList.Count > 0) {

			int persistanceInfoListCount = persistanceInfoList.Count;

//			print (persistanceInfoListCount);

			List<travelStationUISystem.foundTravelStationInfo> foundTravelStationInfoList = mainTravelStationUISystem.foundTravelStationInfoList;

			for (int i = 0; i < persistanceInfoListCount; i++) {

				persistanceTravelStationInfo currentPersistanceTravelStationInfo = persistanceInfoList [i];

				travelStationUISystem.foundTravelStationInfo newFoundTravelStationInfo = new travelStationUISystem.foundTravelStationInfo ();

				newFoundTravelStationInfo.sceneNumberToLoad = currentPersistanceTravelStationInfo.sceneNumberToLoad;
				newFoundTravelStationInfo.levelManagerIDToLoad = currentPersistanceTravelStationInfo.levelManagerIDToLoad;

				foundTravelStationInfoList.Add (newFoundTravelStationInfo);
			}
		}
	}

	void initializeValues ()
	{
		mainTravelStationUISystem.initializeTravelStationValues ();
	}
}