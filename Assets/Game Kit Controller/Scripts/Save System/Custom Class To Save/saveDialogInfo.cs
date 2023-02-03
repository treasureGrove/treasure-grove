using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class saveDialogInfo : saveGameInfo
{
	public dialogManager mainDialogManager;

	public string mainDialogManagerName = "Dialog Manager";

	[Space]
	[Header ("Debug")]
	[Space]

	public List<persistanceDialogContentInfo> persistanceInfoList;

	public persistancePlayerDialogContentInfo temporalPersistancePlayerDialogContentInfo;

	public override void saveGame (int saveNumber, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		saveGameContent (saveNumber, playerID, currentSaveDataPath, showDebugInfo);
	}

	public override void loadGame (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		loadGameContent (saveNumberToLoad, playerID, currentSaveDataPath, showDebugInfo);
	}

	public void saveGameContent (int currentSaveNumber, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		getMainManager ();

		if (mainDialogManager == null) {
			return;
		}

		if (!mainDialogManager.dialogSystemEnabled) {
			return;
		}

		if (!mainDialogManager.saveCurrentDialogContentToSaveFile) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving dialog");
		}
			
		bool saveLocated = false;
		bool playerLocated = false;

		int saveSlotIndex = -1;
		int listIndex = -1;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		persistancePlayerDialogContentInfo dialogContentToSave = getPersistanceList (playerID, showDebugInfo);

		persistanceDialogContentListBySaveSlotInfo newPersistanceDialogContentListBySaveSlotInfo = new persistanceDialogContentListBySaveSlotInfo ();

		List<persistanceDialogContentListBySaveSlotInfo> infoListToSave = new List<persistanceDialogContentListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToSave = currentData as List<persistanceDialogContentListBySaveSlotInfo>;

			file.Close ();	
		}

		int infoListToSaveCount = infoListToSave.Count;

		for (int j = 0; j < infoListToSaveCount; j++) {
			if (infoListToSave [j].saveNumber == currentSaveNumber) {
				newPersistanceDialogContentListBySaveSlotInfo = infoListToSave [j];
				saveLocated = true;
				saveSlotIndex = j;
			}
		}

		if (saveLocated) {
			int playerDialogContentListCount = newPersistanceDialogContentListBySaveSlotInfo.playerDialogContentList.Count;

			for (int j = 0; j < playerDialogContentListCount; j++) {
				if (newPersistanceDialogContentListBySaveSlotInfo.playerDialogContentList [j].playerID == dialogContentToSave.playerID) {
					playerLocated = true;
					listIndex = j;
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("EXTRA INFO\n");
			print ("Number of elements on scene: " + dialogContentToSave.dialogContentList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
			print ("Player Located " + playerLocated);
			print ("Player ID " + dialogContentToSave.playerID);

			print ("Scene Index " + menuPause.getCurrentActiveSceneIndex ());
		}

		int currentSceneIndex = menuPause.getCurrentActiveSceneIndex ();

		//if the save is located, check if the player id exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLocated) {
				int elementOnSceneListCount = infoListToSave [saveSlotIndex].playerDialogContentList [listIndex].dialogContentList.Count;

				for (int i = elementOnSceneListCount - 1; i >= 0; i--) {
					if (currentSceneIndex == infoListToSave [saveSlotIndex].playerDialogContentList [listIndex].dialogContentList [i].dialogContentScene) {
						infoListToSave [saveSlotIndex].playerDialogContentList [listIndex].dialogContentList.RemoveAt (i);
					}
				}

				infoListToSave [saveSlotIndex].playerDialogContentList [listIndex].dialogContentList.AddRange (dialogContentToSave.dialogContentList);
			} else {
				infoListToSave [saveSlotIndex].playerDialogContentList.Add (dialogContentToSave);
			}
		} else {
			newPersistanceDialogContentListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistanceDialogContentListBySaveSlotInfo.playerDialogContentList.Add (dialogContentToSave);
			infoListToSave.Add (newPersistanceDialogContentListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Open (currentSaveDataPath, FileMode.OpenOrCreate); 
		bf.Serialize (file, infoListToSave);

		file.Close ();

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving Dialogs ");
			print ("Number of dialog: " + dialogContentToSave.dialogContentList.Count);

			for (int j = 0; j < dialogContentToSave.dialogContentList.Count; j++) {
				persistanceDialogContentInfo currentPersistanceDialogContentInfo = dialogContentToSave.dialogContentList [j];
								
				if (currentPersistanceDialogContentInfo.dialogContentScene == currentSceneIndex) {
					showDetailedDebugInfo (currentPersistanceDialogContentInfo);
				}
			}
		}
	}

	public void loadGameContent (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		getMainManager ();

		if (mainDialogManager == null) {
			return;
		}

		if (!mainDialogManager.dialogSystemEnabled) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Loading dialog");
		}

		persistanceInfoList = new List<persistanceDialogContentInfo> ();

		//need to store and check the current slot saved and the player which is saving, to get that concrete info
		List<persistanceDialogContentListBySaveSlotInfo> infoListToLoad = new List<persistanceDialogContentListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToLoad = currentData as List<persistanceDialogContentListBySaveSlotInfo>;

			file.Close ();	
		}

		if (saveNumberToLoad > -1) {
			persistanceDialogContentListBySaveSlotInfo newPersistanceDialogContentListBySaveSlotInfo = new persistanceDialogContentListBySaveSlotInfo ();

			int infoListToLoadCount = infoListToLoad.Count;

			for (int j = 0; j < infoListToLoadCount; j++) {

				if (infoListToLoad [j].saveNumber == saveNumberToLoad) {
					newPersistanceDialogContentListBySaveSlotInfo = infoListToLoad [j];
				}
			}

			int listIndex = -1;

			int playerDialogContentListCount = newPersistanceDialogContentListBySaveSlotInfo.playerDialogContentList.Count;

			for (int j = 0; j < playerDialogContentListCount; j++) {

				if (newPersistanceDialogContentListBySaveSlotInfo.playerDialogContentList [j].playerID == playerID) {
					listIndex = j;
				}
			}

			if (listIndex > -1) {
				int currentSceneIndex = menuPause.getCurrentActiveSceneIndex ();
					
				temporalPersistancePlayerDialogContentInfo = newPersistanceDialogContentListBySaveSlotInfo.playerDialogContentList [listIndex];

				int elementOnSceneListCount = temporalPersistancePlayerDialogContentInfo.dialogContentList.Count;

				if (showDebugInfo) {
					print ("\n\n");

					print ("Total number of elements saved in game " + elementOnSceneListCount);
				}

				for (int j = 0; j < elementOnSceneListCount; j++) {
					if (currentSceneIndex == temporalPersistancePlayerDialogContentInfo.dialogContentList [j].dialogContentScene) {
						persistanceInfoList.Add (temporalPersistancePlayerDialogContentInfo.dialogContentList [j]);
					}
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Dialogs Loaded in Save Number " + saveNumberToLoad);
			print ("Number of dialog: " + persistanceInfoList.Count);

			print ("Scene Index " + menuPause.getCurrentActiveSceneIndex ());

			print ("\n\n");

			for (int j = 0; j < persistanceInfoList.Count; j++) {
				print ("\n\n");

				persistanceDialogContentInfo currentPersistanceDialogContentInfo = persistanceInfoList [j];

				showDetailedDebugInfo (currentPersistanceDialogContentInfo);
			}
		}

		loadInfoOnMainComponent ();
	}

	public persistancePlayerDialogContentInfo getPersistanceList (int playerID, bool showDebugInfo)
	{
		persistancePlayerDialogContentInfo newPersistancePlayerDialogContentInfo = new persistancePlayerDialogContentInfo ();

		newPersistancePlayerDialogContentInfo.playerID = playerID;

		List<persistanceDialogContentInfo> newPersistanceDialogContentInfoList = new List<persistanceDialogContentInfo> ();

		List<dialogContentSystem> dialogContentSystemList = mainDialogManager.dialogContentSystemList;

		int dialogContentSystemListCount = dialogContentSystemList.Count;

		for (int k = 0; k < dialogContentSystemListCount; k++) {

			dialogContentSystem currentDialogContentSystem = dialogContentSystemList [k];
			
			if (currentDialogContentSystem != null) {
				persistanceDialogContentInfo newPersistanceDialogContentInfo = new persistanceDialogContentInfo ();
					
				newPersistanceDialogContentInfo.dialogContentID = currentDialogContentSystem.dialogContentID;
				newPersistanceDialogContentInfo.dialogContentScene = currentDialogContentSystem.dialogContentScene;
				newPersistanceDialogContentInfo.currentDialogIndex = currentDialogContentSystem.currentDialogIndex;

				newPersistanceDialogContentInfoList.Add (newPersistanceDialogContentInfo);
			}
		}	

		newPersistancePlayerDialogContentInfo.dialogContentList = newPersistanceDialogContentInfoList;

		return newPersistancePlayerDialogContentInfo;
	}


	void loadInfoOnMainComponent ()
	{
		if (persistanceInfoList != null && persistanceInfoList.Count > 0) {
			int persistanceInfoListCount = persistanceInfoList.Count;

			for (int i = 0; i < persistanceInfoListCount; i++) {

				persistanceDialogContentInfo currentPersistanceDialogContentInfo = persistanceInfoList [i];

				dialogContentSystem currentDialogContentSystem = mainDialogManager.getDialogContentSystem (currentPersistanceDialogContentInfo.dialogContentID, 
					                                                 currentPersistanceDialogContentInfo.dialogContentScene);

				if (currentDialogContentSystem != null) {
					currentDialogContentSystem.setCompleteDialogIndex (currentPersistanceDialogContentInfo.currentDialogIndex);
				}
			}
		}
	}

	void showDetailedDebugInfo (persistanceDialogContentInfo currentPersistanceDialogContentInfo)
	{
		print ("Dialog Content ID " + currentPersistanceDialogContentInfo.dialogContentID);
		print ("Dialog Scene " + currentPersistanceDialogContentInfo.dialogContentScene);
		print ("Dialog Index " + currentPersistanceDialogContentInfo.currentDialogIndex);

		print ("\n\n");
	}

	void getMainManager ()
	{
		if (mainDialogManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainDialogManagerName, typeof(dialogManager));

			mainDialogManager = FindObjectOfType<dialogManager> ();
		}
	}
}