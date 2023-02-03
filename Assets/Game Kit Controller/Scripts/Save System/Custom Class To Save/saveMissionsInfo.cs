using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class saveMissionsInfo : saveGameInfo
{
	public objectiveLogSystem mainObjectiveLogSystem;

	List<persistanceMissionInfo> persistanceInfoList;

	bool showCurrentDebugInfo;

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
		getMainManager ();

		if (mainObjectiveLogSystem == null) {
			return;
		}

		if (!mainObjectiveLogSystem.objectiveMenuActive) {
			return;
		}

		if (!mainObjectiveLogSystem.saveCurrentPlayerMissionsToSaveFile) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving missions");
		}

		bool saveLocated = false;
		bool playerLocated = false;

		int saveSlotIndex = -1;
		int listIndex = -1;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		persistancePlayerMissionInfo missionsToSave = getPersistanceList (playerID, showDebugInfo);

		persistancePlayerMissionsListBySaveSlotInfo newPersistancePlayerMissionsListBySaveSlotInfo = new persistancePlayerMissionsListBySaveSlotInfo ();

		List<persistancePlayerMissionsListBySaveSlotInfo> infoListToSave = new List<persistancePlayerMissionsListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToSave = currentData as List<persistancePlayerMissionsListBySaveSlotInfo>;

			file.Close ();	
		}

		int infoListToSaveCount = infoListToSave.Count;

		for (int j = 0; j < infoListToSaveCount; j++) {
			if (infoListToSave [j].saveNumber == currentSaveNumber) {
				newPersistancePlayerMissionsListBySaveSlotInfo = infoListToSave [j];
				saveLocated = true;
				saveSlotIndex = j;
			}
		}

		if (saveLocated) {
			int playerMissionsListCount = newPersistancePlayerMissionsListBySaveSlotInfo.playerMissionsList.Count;

			for (int j = 0; j < playerMissionsListCount; j++) {
				if (newPersistancePlayerMissionsListBySaveSlotInfo.playerMissionsList [j].playerID == missionsToSave.playerID) {
					playerLocated = true;
					listIndex = j;
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("EXTRA INFO\n");
			print ("Number of missions: " + missionsToSave.missionsList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
			print ("Player Located " + playerLocated);
			print ("Player ID " + missionsToSave.playerID);
		}

		//if the save is located, check if the player id exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLocated) {
				infoListToSave [saveSlotIndex].playerMissionsList [listIndex].missionsList = missionsToSave.missionsList;
			} else {
				infoListToSave [saveSlotIndex].playerMissionsList.Add (missionsToSave);
			}
		} else {
			newPersistancePlayerMissionsListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistancePlayerMissionsListBySaveSlotInfo.playerMissionsList.Add (missionsToSave);
			infoListToSave.Add (newPersistancePlayerMissionsListBySaveSlotInfo);
		}

		if (showDebugInfo) {
			print ("\n\n");

			for (int j = 0; j < missionsToSave.missionsList.Count; j++) {
				persistanceMissionInfo currentPersistanceMissionInfo = missionsToSave.missionsList [j];

				showDetailedDebugInfo (currentPersistanceMissionInfo);
			}
		}

		bf = new BinaryFormatter ();
		file = File.Open (currentSaveDataPath, FileMode.OpenOrCreate); 
		bf.Serialize (file, infoListToSave);

		file.Close ();
	}

	public void loadGameContent (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		getMainManager ();

		if (mainObjectiveLogSystem == null) {
			return;
		}

		if (!mainObjectiveLogSystem.objectiveMenuActive) {
			return;
		}

		if (!mainObjectiveLogSystem.saveCurrentPlayerMissionsToSaveFile) {
			initializeValues ();

			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Loading missions");
		}

		//need to store and check the current slot saved and the player which is saving, to get that concrete info
		persistanceInfoList = new List<persistanceMissionInfo> ();

		List<persistancePlayerMissionsListBySaveSlotInfo> infoListToLoad = new List<persistancePlayerMissionsListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToLoad = currentData as List<persistancePlayerMissionsListBySaveSlotInfo>;

			file.Close ();	
		}

		if (saveNumberToLoad > -1) {
			persistancePlayerMissionsListBySaveSlotInfo newPersistancePlayerMissionsListBySaveSlotInfo = new persistancePlayerMissionsListBySaveSlotInfo ();

			int infoListToLoadCount = infoListToLoad.Count;

			for (int j = 0; j < infoListToLoadCount; j++) {

				if (infoListToLoad [j].saveNumber == saveNumberToLoad) {
					newPersistancePlayerMissionsListBySaveSlotInfo = infoListToLoad [j];
				}
			}

			int listIndex = -1;

			int playerMissionsListCount = newPersistancePlayerMissionsListBySaveSlotInfo.playerMissionsList.Count;

			for (int j = 0; j < playerMissionsListCount; j++) {

				if (newPersistancePlayerMissionsListBySaveSlotInfo.playerMissionsList [j].playerID == playerID) {
					listIndex = j;
				}
			}

			if (listIndex > -1) {
				persistanceInfoList.AddRange (newPersistancePlayerMissionsListBySaveSlotInfo.playerMissionsList [listIndex].missionsList);
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Missions Loaded in Save Number " + saveNumberToLoad);
			print ("Number of missions: " + persistanceInfoList.Count);

			print ("\n\n");

			for (int j = 0; j < persistanceInfoList.Count; j++) {
				print ("\n\n");

				persistanceMissionInfo currentPersistanceMissionInfo = persistanceInfoList [j];

				showDetailedDebugInfo (currentPersistanceMissionInfo);
			}
		}

		showCurrentDebugInfo = showDebugInfo;

		loadInfoOnMainComponent ();
	}


	public persistancePlayerMissionInfo getPersistanceList (int playerID, bool showDebugInfo)
	{
		persistancePlayerMissionInfo newMissionsList = new persistancePlayerMissionInfo ();

		newMissionsList.playerID = playerID;

		List<persistanceMissionInfo> newPersistanceMissionInfoList = new List<persistanceMissionInfo> ();

		List<objectiveLogSystem.objectiveSlotInfo> objectiveSlotInfoList = mainObjectiveLogSystem.objectiveSlotInfoList;

		int objectiveSlotInfoListCount = objectiveSlotInfoList.Count;

		for (int k = 0; k < objectiveSlotInfoListCount; k++) {
			persistanceMissionInfo newPersistanceMissionInfo = new persistanceMissionInfo ();

			objectiveLogSystem.objectiveSlotInfo currentSlotInfo = objectiveSlotInfoList [k];

			newPersistanceMissionInfo.missionScene = currentSlotInfo.missionScene;
			newPersistanceMissionInfo.missionID = currentSlotInfo.missionID;

			newPersistanceMissionInfo.disableObjectivePanelOnMissionComplete = currentSlotInfo.disableObjectivePanelOnMissionComplete;

			newPersistanceMissionInfo.addObjectiveToPlayerLogSystem = currentSlotInfo.addObjectiveToPlayerLogSystem;

			newPersistanceMissionInfo.missionComplete = currentSlotInfo.missionComplete;
			newPersistanceMissionInfo.missionInProcess = currentSlotInfo.missionInProcess;
			newPersistanceMissionInfo.rewardObtained = currentSlotInfo.rewardObtained;	
			newPersistanceMissionInfo.missionAccepted = currentSlotInfo.missionAccepted;

			newPersistanceMissionInfo.objectiveName = currentSlotInfo.objectiveName;
			newPersistanceMissionInfo.objectiveDescription = currentSlotInfo.objectiveDescription;
			newPersistanceMissionInfo.objectiveFullDescription = currentSlotInfo.objectiveFullDescription;
			newPersistanceMissionInfo.objectiveLocation = currentSlotInfo.objectiveLocation;
			newPersistanceMissionInfo.objectiveRewards = currentSlotInfo.objectiveRewards;

			newPersistanceMissionInfo.subObjectiveCompleteList = currentSlotInfo.subObjectiveCompleteList;

			if (showDebugInfo) {
				debugMissionInfo (newPersistanceMissionInfo);
			}

			newPersistanceMissionInfoList.Add (newPersistanceMissionInfo);
		}	

		newMissionsList.missionsList = newPersistanceMissionInfoList;

		return newMissionsList;
	}


	void loadInfoOnMainComponent ()
	{
		initializeValues ();

		if (persistanceInfoList != null && persistanceInfoList.Count > 0) {

			int persistanceInfoListCount = persistanceInfoList.Count;

			for (int k = 0; k < persistanceInfoListCount; k++) {

				persistanceMissionInfo currentPersistanceMissionInfo = persistanceInfoList [k];

				//Seach the objective event system, the mission it self, in the main objective manager, to see the info needed on it
				objectiveEventSystem currentObjectiveEventSystem =
					mainObjectiveLogSystem.mainObjectiveManager.getObjectiveEventSystem (currentPersistanceMissionInfo.missionID, currentPersistanceMissionInfo.missionScene);

				//if the current mission to search is found in the level, assign the info and set its state, including to add the log info on the player's menu
				if (currentObjectiveEventSystem != null) {
					if (showCurrentDebugInfo) {
						debugMissionInfo (currentPersistanceMissionInfo);
					}

					currentObjectiveEventSystem.setCurrentPlayer (mainObjectiveLogSystem.playerControllerGameObject);

					bool updateSubobjectivesCompleteOnMission = currentPersistanceMissionInfo.missionInProcess ||
					                                            currentObjectiveEventSystem.updateSubobjectivesCompleteOnLoadGame;

					if (updateSubobjectivesCompleteOnMission) {
						if (currentPersistanceMissionInfo.subObjectiveCompleteList.Count > 0) {
							currentObjectiveEventSystem.setSubObjectiveCompleteListState (currentPersistanceMissionInfo.subObjectiveCompleteList);
						}

						currentObjectiveEventSystem.initializeNumberOfObjectives ();
					}

					bool resumeMissionOnLoadGameIfNotComplete = !currentPersistanceMissionInfo.missionComplete &&
					                                            (currentObjectiveEventSystem.resumeMissionOnLoadGameIfNotComplete ||
					                                            currentPersistanceMissionInfo.missionInProcess);

					if (resumeMissionOnLoadGameIfNotComplete) {
						currentObjectiveEventSystem.startObjective ();
					} else {
						currentObjectiveEventSystem.addObjectiveToPlayerLogMenu ();

						if (currentPersistanceMissionInfo.missionComplete) {
							currentObjectiveEventSystem.setObjectiveAsCompleteOnLoad (currentPersistanceMissionInfo.rewardObtained);
						}
					}
				} else {

					//If the mission system is not found, it means the mission is assigned in other level, so just assign the info in the player's log
					mainObjectiveLogSystem.addObjective (currentPersistanceMissionInfo.objectiveName, 
						currentPersistanceMissionInfo.objectiveDescription, 
						currentPersistanceMissionInfo.objectiveFullDescription, 
						currentPersistanceMissionInfo.objectiveLocation, 
						currentPersistanceMissionInfo.objectiveRewards, 
						null, 
						currentPersistanceMissionInfo.addObjectiveToPlayerLogSystem);

					objectiveLogSystem.objectiveSlotInfo currentobjectiveSlotInfo = 
						mainObjectiveLogSystem.objectiveSlotInfoList [mainObjectiveLogSystem.objectiveSlotInfoList.Count - 1];

					currentobjectiveSlotInfo.missionScene = currentPersistanceMissionInfo.missionScene;
					currentobjectiveSlotInfo.missionID = currentPersistanceMissionInfo.missionID;

					currentobjectiveSlotInfo.disableObjectivePanelOnMissionComplete = currentPersistanceMissionInfo.disableObjectivePanelOnMissionComplete;

					currentobjectiveSlotInfo.addObjectiveToPlayerLogSystem = currentPersistanceMissionInfo.addObjectiveToPlayerLogSystem;

					currentobjectiveSlotInfo.missionComplete = currentPersistanceMissionInfo.missionComplete;
					currentobjectiveSlotInfo.missionInProcess = currentPersistanceMissionInfo.missionInProcess;
					currentobjectiveSlotInfo.rewardObtained = currentPersistanceMissionInfo.rewardObtained;	
					currentobjectiveSlotInfo.missionAccepted = currentPersistanceMissionInfo.missionAccepted;

					if (currentobjectiveSlotInfo.missionComplete) {
						mainObjectiveLogSystem.updateObjectiveCompleteSlotInfo (mainObjectiveLogSystem.objectiveSlotInfoList.Count - 1);
					} else {
						if (currentPersistanceMissionInfo.subObjectiveCompleteList.Count > 0) {
							mainObjectiveLogSystem.updateSubObjectiveCompleteListSlotInfo (mainObjectiveLogSystem.objectiveSlotInfoList.Count - 1,
								currentPersistanceMissionInfo.subObjectiveCompleteList);
						}
					}
				}

				mainObjectiveLogSystem.updateUIElements ();
			}
		}
	}


	void getMainManager ()
	{
		if (mainObjectiveLogSystem == null) {
			mainObjectiveLogSystem = FindObjectOfType<objectiveLogSystem> ();
		}
	}

	void initializeValues ()
	{
		mainObjectiveLogSystem.initializeMissionValues ();
	}

	public void debugMissionInfo (persistanceMissionInfo missinInfo)
	{
		print (

			missinInfo.missionScene + " " +
			missinInfo.missionID + " " +
			missinInfo.disableObjectivePanelOnMissionComplete + " " +
			missinInfo.addObjectiveToPlayerLogSystem + " " +
			missinInfo.missionComplete + " " +
			missinInfo.missionInProcess + " " +
			missinInfo.rewardObtained + " " +
			missinInfo.missionAccepted + " " +
			missinInfo.objectiveName + " " +
			missinInfo.objectiveDescription + " " +
			missinInfo.objectiveFullDescription + " " +
			missinInfo.objectiveRewards

		);
	}

	void showDetailedDebugInfo (persistanceMissionInfo currentPersistanceMissionInfo)
	{
		print ("Mission Scene " + currentPersistanceMissionInfo.missionScene);
		print ("Mission ID " + currentPersistanceMissionInfo.missionID);
		print ("Disable Panel On Mission Complete " + currentPersistanceMissionInfo.disableObjectivePanelOnMissionComplete);
		print ("Add Objective To Player Log System " + currentPersistanceMissionInfo.addObjectiveToPlayerLogSystem);
		print ("Mission Complete " + currentPersistanceMissionInfo.missionComplete);
		print ("Mission In Process " + currentPersistanceMissionInfo.missionInProcess);
		print ("Mission Reward Obtained " + currentPersistanceMissionInfo.rewardObtained);
		print ("Mission Accepted " + currentPersistanceMissionInfo.missionAccepted);
		print ("Objective Name " + currentPersistanceMissionInfo.objectiveName);
		print ("Objective Description " + currentPersistanceMissionInfo.objectiveDescription);
		print ("Full Description " + currentPersistanceMissionInfo.objectiveFullDescription);
		print ("Objective Location " + currentPersistanceMissionInfo.objectiveLocation);
		print ("Objective Rewards " + currentPersistanceMissionInfo.objectiveRewards);


		print ("Sub Objectives Complete\n");

		for (int j = 0; j < currentPersistanceMissionInfo.subObjectiveCompleteList.Count; j++) {
			print (currentPersistanceMissionInfo.subObjectiveCompleteList [j]);
		}

		print ("\n\n");
	}
}