using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class saveSkillsInfo : saveGameInfo
{
	public playerSkillsSystem mainPlayerSkillsSystem;

	List<persistanceCategorySkillInfo> persistanceInfoList;

	bool valuesInitializedOnLoad;


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

		if (mainPlayerSkillsSystem == null) {
			return;
		}

		if (!mainPlayerSkillsSystem.playerSkillsActive) {
			return;
		}

		if (!mainPlayerSkillsSystem.saveCurrentPlayerSkillsToSaveFile) {
			return;
		}			

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving skills");
		}

		bool saveLocated = false;
		bool playerLocated = false;

		int saveSlotIndex = -1;
		int listIndex = -1;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		persistancePlayerCategorySkillInfo skillsToSave = getPersistanceList (playerID, showDebugInfo);

		persistancePlayerSkillsListBySaveSlotInfo newPersistancePlayerSkillsListBySaveSlotInfo = new persistancePlayerSkillsListBySaveSlotInfo ();

		List<persistancePlayerSkillsListBySaveSlotInfo> infoListToSave = new List<persistancePlayerSkillsListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToSave = currentData as List<persistancePlayerSkillsListBySaveSlotInfo>;

			file.Close ();	
		}

		int infoListToSaveCount = infoListToSave.Count;

		for (int j = 0; j < infoListToSaveCount; j++) {
			if (infoListToSave [j].saveNumber == currentSaveNumber) {
				newPersistancePlayerSkillsListBySaveSlotInfo = infoListToSave [j];
				saveLocated = true;
				saveSlotIndex = j;
			}
		}

		if (saveLocated) {
			int playerSkillsListCount = newPersistancePlayerSkillsListBySaveSlotInfo.playerSkillsList.Count;

			for (int j = 0; j < playerSkillsListCount; j++) {
				if (newPersistancePlayerSkillsListBySaveSlotInfo.playerSkillsList [j].playerID == skillsToSave.playerID) {
					playerLocated = true;
					listIndex = j;
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("EXTRA INFO\n");
			print ("Number of skills: " + skillsToSave.categorySkillsList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
			print ("Player Located " + playerLocated);
			print ("Player ID " + skillsToSave.playerID);
		}

		//if the save is located, check if the player id exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLocated) {
				infoListToSave [saveSlotIndex].playerSkillsList [listIndex].categorySkillsList = skillsToSave.categorySkillsList;
			} else {
				infoListToSave [saveSlotIndex].playerSkillsList.Add (skillsToSave);
			}
		} else {
			newPersistancePlayerSkillsListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistancePlayerSkillsListBySaveSlotInfo.playerSkillsList.Add (skillsToSave);
			infoListToSave.Add (newPersistancePlayerSkillsListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Open (currentSaveDataPath, FileMode.OpenOrCreate); 
		bf.Serialize (file, infoListToSave);

		file.Close ();
	}

	public void loadGameContent (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		getMainManager ();

		if (mainPlayerSkillsSystem == null) {
			return;
		}

		if (!mainPlayerSkillsSystem.playerSkillsActive) {
			return;
		}

		if (!mainPlayerSkillsSystem.saveCurrentPlayerSkillsToSaveFile) {
			initializeValues ();

			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Loading skills");
		}

		//need to store and check the current slot saved and the player which is saving, to get that concrete info
		persistanceInfoList = new List<persistanceCategorySkillInfo> ();

		List<persistancePlayerSkillsListBySaveSlotInfo> infoListToLoad = new List<persistancePlayerSkillsListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToLoad = currentData as List<persistancePlayerSkillsListBySaveSlotInfo>;

			file.Close ();	
		}

		if (saveNumberToLoad > -1) {
			persistancePlayerSkillsListBySaveSlotInfo newPersistancePlayerSkillsListBySaveSlotInfo = new persistancePlayerSkillsListBySaveSlotInfo ();

			int infoListToLoadCount = infoListToLoad.Count;

			for (int j = 0; j < infoListToLoadCount; j++) {

				if (infoListToLoad [j].saveNumber == saveNumberToLoad) {
					newPersistancePlayerSkillsListBySaveSlotInfo = infoListToLoad [j];
				}
			}

			int listIndex = -1;

			int playerSkillsListCount = newPersistancePlayerSkillsListBySaveSlotInfo.playerSkillsList.Count;

			for (int j = 0; j < playerSkillsListCount; j++) {

				if (newPersistancePlayerSkillsListBySaveSlotInfo.playerSkillsList [j].playerID == playerID) {
					listIndex = j;
				}
			}

			if (listIndex > -1) {
				persistanceInfoList.AddRange (newPersistancePlayerSkillsListBySaveSlotInfo.playerSkillsList [listIndex].categorySkillsList);
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Skills Loaded in Save Number " + saveNumberToLoad);
		}

		loadInfoOnMainComponent ();
	}


	public persistancePlayerCategorySkillInfo getPersistanceList (int playerID, bool showDebugInfo)
	{
		persistancePlayerCategorySkillInfo newSkillsList = new persistancePlayerCategorySkillInfo ();

		newSkillsList.playerID = playerID;

		List<persistanceCategorySkillInfo> newPersistanceCategorySkillInfoList = new List<persistanceCategorySkillInfo> ();

		List<playerSkillsSystem.skillCategoryInfo> skillCategoryInfoList = mainPlayerSkillsSystem.skillCategoryInfoList;

		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {

			persistanceCategorySkillInfo newPersistanceCategorySkillInfo = new persistanceCategorySkillInfo ();

			List<persistanceSkillInfo> newPersistanceSkillInfoList = new List<persistanceSkillInfo> ();

			playerSkillsSystem.skillCategoryInfo currentSkillCategory = skillCategoryInfoList [i];

			int skillInfoListCount = currentSkillCategory.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				playerSkillsSystem.skillInfo currentSkillInfo = currentSkillCategory.skillInfoList [k];

				persistanceSkillInfo newpersistanceSkillInfo = new persistanceSkillInfo ();

				newpersistanceSkillInfo.skillUnlocked = currentSkillInfo.skillUnlocked;
				newpersistanceSkillInfo.skillActive = currentSkillInfo.skillActive;
				newpersistanceSkillInfo.skillComplete = currentSkillInfo.skillComplete;
				newpersistanceSkillInfo.currentBoolState = currentSkillInfo.currentBoolState;
				newpersistanceSkillInfo.currentValue = currentSkillInfo.currentValue;
				newpersistanceSkillInfo.currentSkillLevel = currentSkillInfo.currentSkillLevel;

				newPersistanceSkillInfoList.Add (newpersistanceSkillInfo);
			}

			newPersistanceCategorySkillInfo.skillsList = newPersistanceSkillInfoList;

			newPersistanceCategorySkillInfoList.Add (newPersistanceCategorySkillInfo);
		}	

		newSkillsList.categorySkillsList = newPersistanceCategorySkillInfoList;

		return newSkillsList;
	}


	void loadInfoOnMainComponent ()
	{
		valuesInitializedOnLoad = true;

		initializeValues ();

		if (persistanceInfoList != null && persistanceInfoList.Count > 0) {
			List<playerSkillsSystem.skillCategoryInfo> skillCategoryInfoList = mainPlayerSkillsSystem.skillCategoryInfoList;

			int skillCategoryInfoListCount = skillCategoryInfoList.Count;

			for (int i = 0; i < skillCategoryInfoListCount; i++) {

				playerSkillsSystem.skillCategoryInfo currentSkillCategory = skillCategoryInfoList [i];

				int skillInfoListCount = currentSkillCategory.skillInfoList.Count;

				for (int k = 0; k < skillInfoListCount; k++) {

					playerSkillsSystem.skillInfo currentSkillInfo = currentSkillCategory.skillInfoList [k];

					persistanceSkillInfo currentPersistanceSkillInfo = persistanceInfoList [i].skillsList [k];

					currentSkillInfo.skillUnlocked = currentPersistanceSkillInfo.skillUnlocked;
					currentSkillInfo.skillActive = currentPersistanceSkillInfo.skillActive;

					currentSkillInfo.skillComplete = currentPersistanceSkillInfo.skillComplete;
					currentSkillInfo.currentBoolState = currentPersistanceSkillInfo.currentBoolState;

					currentSkillInfo.currentValue = currentPersistanceSkillInfo.currentValue;
					currentSkillInfo.currentSkillLevel = currentPersistanceSkillInfo.currentSkillLevel;
				}
			}

			mainPlayerSkillsSystem.isLoadingGame = true;
		}

		mainPlayerSkillsSystem.initializeSkillsValues ();
	}


	void getMainManager ()
	{
		if (mainPlayerSkillsSystem == null) {
			mainPlayerSkillsSystem = FindObjectOfType<playerSkillsSystem> ();
		}
	}

	void initializeValues ()
	{
		mainPlayerSkillsSystem.isLoadingGame = false;

		if (!valuesInitializedOnLoad) {
			mainPlayerSkillsSystem.initializeSkillsValues ();
		}
	}
}