using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class savePlayerBlendshapesInfo : saveGameInfo
{
	public characterCustomizationBlendshapesHelper mainCharacterCustomizationBlendshapesHelper;

	List<persistanceBlendshapesInfo> persistanceInfoList;

	List<string> accessoriesList = new List<string> ();

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
		if (mainCharacterCustomizationBlendshapesHelper == null) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving player blendshapes");
		}

		bool saveLocated = false;
		bool playerLocated = false;

		int saveSlotIndex = -1;
		int listIndex = -1;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		persistancePlayerBlendshapesInfo blendShapeListToSave = getPersistanceList (playerID, showDebugInfo);

		persistanceBlendshapesListBySaveSlotInfo newPersistanceListBySaveSlotInfo = new persistanceBlendshapesListBySaveSlotInfo ();

		List<persistanceBlendshapesListBySaveSlotInfo> infoListToSave = new List<persistanceBlendshapesListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToSave = currentData as List<persistanceBlendshapesListBySaveSlotInfo>;

			file.Close ();	
		}

		if (showDebugInfo) {
			print ("Number of save list for player blendshapes " + infoListToSave.Count);
		}

		int infoListToSaveCount = infoListToSave.Count;

		for (int j = 0; j < infoListToSaveCount; j++) {
			if (saveSlotIndex == -1) {
				persistanceBlendshapesListBySaveSlotInfo currentSlot = infoListToSave [j];

				if (ignoreSaveNumberOnSaveLoadInfo || currentSlot.saveNumber == currentSaveNumber) {
					newPersistanceListBySaveSlotInfo = currentSlot;
					saveLocated = true;
					saveSlotIndex = j;
				}
			}
		}

		if (saveLocated) {
			int listCount = newPersistanceListBySaveSlotInfo.playerBlendshapesList.Count;

			int playerIDToSearch = blendShapeListToSave.playerID;

			for (int j = 0; j < listCount; j++) {
				if (listIndex == -1) {
					if (ignorePlayerIDOnSaveLoadInfo ||
					    newPersistanceListBySaveSlotInfo.playerBlendshapesList [j].playerID == playerIDToSearch) {
						playerLocated = true;
						listIndex = j;
					}
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("EXTRA INFO\n");
			print ("PLAYER BLENDSHAPES");
			print ("Number of blendshpaes: " + blendShapeListToSave.blendshapesList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
			print ("Player Located " + playerLocated);

			print ("Save List Index " + listIndex);
		}

		//if the save is located, check if the player id exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLocated) {
				infoListToSave [saveSlotIndex].playerBlendshapesList [listIndex] = blendShapeListToSave;
			} else {
				infoListToSave [saveSlotIndex].playerBlendshapesList.Add (blendShapeListToSave);
			}
		} else {
			newPersistanceListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistanceListBySaveSlotInfo.playerBlendshapesList.Add (blendShapeListToSave);

			infoListToSave.Add (newPersistanceListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Open (currentSaveDataPath, FileMode.OpenOrCreate); 
		bf.Serialize (file, infoListToSave);

		file.Close ();
	}

	public void loadGameContent (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		if (mainCharacterCustomizationBlendshapesHelper == null) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Loading player blendshapes with PLAYER ID " + playerID);
		}
			
		persistanceInfoList = new List<persistanceBlendshapesInfo> ();

		List<persistanceBlendshapesListBySaveSlotInfo> infoListToLoad = new List<persistanceBlendshapesListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToLoad = currentData as List<persistanceBlendshapesListBySaveSlotInfo>;

			file.Close ();	
		}

		if (saveNumberToLoad > -1) {

			if (showDebugInfo) {
				print (infoListToLoad.Count);
			}

			persistanceBlendshapesListBySaveSlotInfo newPersistanceListBySaveSlotInfo = new persistanceBlendshapesListBySaveSlotInfo ();

			int infoListToLoadCount = infoListToLoad.Count;

			bool slotLocated = false;

			for (int j = 0; j < infoListToLoadCount; j++) {
				if (!slotLocated) {
					persistanceBlendshapesListBySaveSlotInfo currentSlot = infoListToLoad [j];

					bool checkSlot = false;

					if (ignoreSaveNumberOnSaveLoadInfo) {
						checkSlot = true;
					}

					if (saveNumberToIgnoreSaveLoadInfo == saveNumberToLoad && saveNumberToIgnoreSaveLoadInfo > -1) {
						checkSlot = true;
					}

					if (currentSlot.saveNumber == saveNumberToLoad) {
						checkSlot = true;
					}

					if (checkSlot) {
						newPersistanceListBySaveSlotInfo = currentSlot;

						slotLocated = true;
					}
				}
			}

			int listIndex = -1;

			int listCount = newPersistanceListBySaveSlotInfo.playerBlendshapesList.Count;

			for (int j = 0; j < listCount; j++) {
				if (listIndex == -1) {
					bool checkSlot = false;

					if (ignorePlayerIDOnSaveLoadInfo) {
						checkSlot = true;
					}

					if (saveNumberToIgnoreSaveLoadInfo == saveNumberToLoad && saveNumberToIgnoreSaveLoadInfo > -1) {
						checkSlot = true;
					}

					if (newPersistanceListBySaveSlotInfo.playerBlendshapesList [j].playerID == playerID) {
						checkSlot = true;
					}

					if (checkSlot) {
						listIndex = j;
					}
				}
			}

			if (listIndex > -1) {
				persistancePlayerBlendshapesInfo blendshapeListToLoad = newPersistanceListBySaveSlotInfo.playerBlendshapesList [listIndex];

				persistanceInfoList.AddRange (newPersistanceListBySaveSlotInfo.playerBlendshapesList [listIndex].blendshapesList);

				accessoriesList.AddRange (newPersistanceListBySaveSlotInfo.playerBlendshapesList [listIndex].accessoriesList);
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("PLAYER BLENDSHAPES");

			print ("Blendshapes Loaded in Save Number " + saveNumberToLoad);
			print ("Number of objects: " + persistanceInfoList.Count);

			for (int j = 0; j < persistanceInfoList.Count; j++) {
				persistanceBlendshapesInfo currentPersistanceBlendshapesInfo = persistanceInfoList [j];

				print (currentPersistanceBlendshapesInfo.Name + " " + currentPersistanceBlendshapesInfo.blendShapeValue);
			}
		}

		loadInfoOnMainComponent ();
	}


	public persistancePlayerBlendshapesInfo getPersistanceList (int playerID, bool showDebugInfo)
	{
		persistancePlayerBlendshapesInfo newPersistancePlayerBlendshapesInfo = new persistancePlayerBlendshapesInfo ();

		newPersistancePlayerBlendshapesInfo.playerID = playerID;

		List<persistanceBlendshapesInfo> newPersistanceBlendshapesInfoList = new List<persistanceBlendshapesInfo> ();

		List<characterCustomizationManager.temporalBlendshapeInfo> temporalBlendshapeInfoList = mainCharacterCustomizationBlendshapesHelper.temporalBlendshapeInfoList;

		int temporalBlendshapeInfoListCount = temporalBlendshapeInfoList.Count;

		for (int k = 0; k < temporalBlendshapeInfoListCount; k++) {
			persistanceBlendshapesInfo newPersistanceBlendshapesInfo = new persistanceBlendshapesInfo ();

			characterCustomizationManager.temporalBlendshapeInfo currentBlendshape = temporalBlendshapeInfoList [k];

			newPersistanceBlendshapesInfo.Name = currentBlendshape.Name;

			newPersistanceBlendshapesInfo.blendShapeValue = currentBlendshape.blendShapeValue;

			newPersistanceBlendshapesInfoList.Add (newPersistanceBlendshapesInfo);
		}	

		newPersistancePlayerBlendshapesInfo.blendshapesList = newPersistanceBlendshapesInfoList;

		newPersistancePlayerBlendshapesInfo.accessoriesList = mainCharacterCustomizationBlendshapesHelper.temporalCurrentAccessoriesList;

		return newPersistancePlayerBlendshapesInfo;
	}


	void loadInfoOnMainComponent ()
	{
		initializeValues ();

		if (persistanceInfoList != null && persistanceInfoList.Count > 0) {

			int persistanceInfoListCount = persistanceInfoList.Count;

			for (int i = 0; i < persistanceInfoListCount; i++) {

				persistanceBlendshapesInfo currentPersistanceBlendshapesInfo = persistanceInfoList [i];

				characterCustomizationManager.temporalBlendshapeInfo newBlendshape = new characterCustomizationManager.temporalBlendshapeInfo ();

				newBlendshape.Name = currentPersistanceBlendshapesInfo.Name;
				newBlendshape.blendShapeValue = currentPersistanceBlendshapesInfo.blendShapeValue;

				mainCharacterCustomizationBlendshapesHelper.temporalBlendshapeInfoList.Add (newBlendshape);
			}

			mainCharacterCustomizationBlendshapesHelper.temporalCurrentAccessoriesList.AddRange (accessoriesList);

			mainCharacterCustomizationBlendshapesHelper.setBlendshapeList ();
		}
	}

	void initializeValues ()
	{
		mainCharacterCustomizationBlendshapesHelper.checkMainCHaracterCustomizatationManager ();
	}
}