using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class savePlayerOptionsInfo : saveGameInfo
{
	[Header ("Custom Settings")]
	[Space]

	public playerOptionsEditorSystem mainPlayerOptionsEditorSystem;

	List<persistanceOptionsInfo> persistanceInfoList;


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
		if (mainPlayerOptionsEditorSystem == null) {
			return;
		}

		if (!mainPlayerOptionsEditorSystem.playerOptionsEditorEnabled) {
			return;
		}

		if (!mainPlayerOptionsEditorSystem.saveCurrentPlayerOptionsToSaveFile) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving player options");
		}


		bool saveLocated = false;
		bool playerLocated = false;

		int saveSlotIndex = -1;
		int listIndex = -1;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		persistancePlayerOptionsInfo playerOptionsToSave = getPersistanceList (playerID, showDebugInfo);

		persistancePlayerOptionsListBySaveSlotInfo newPersistancePlayerOptionsListBySaveSlotInfo = new persistancePlayerOptionsListBySaveSlotInfo ();

		List<persistancePlayerOptionsListBySaveSlotInfo> infoListToSave = new List<persistancePlayerOptionsListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToSave = currentData as List<persistancePlayerOptionsListBySaveSlotInfo>;

			file.Close ();	
		}

		int infoListToSaveCount = infoListToSave.Count;

		for (int j = 0; j < infoListToSaveCount; j++) {
			if (!saveLocated) {
				if (ignoreSaveNumberOnSaveLoadInfo || infoListToSave [j].saveNumber == currentSaveNumber) {
					newPersistancePlayerOptionsListBySaveSlotInfo = infoListToSave [j];
					saveLocated = true;
					saveSlotIndex = j;
				}
			}
		}

		if (saveLocated) {
			int playerOptionsListCount = newPersistancePlayerOptionsListBySaveSlotInfo.playerOptionsList.Count;

			for (int j = 0; j < playerOptionsListCount; j++) {
				if (!playerLocated) {
					if (ignorePlayerIDOnSaveLoadInfo ||
					    newPersistancePlayerOptionsListBySaveSlotInfo.playerOptionsList [j].playerID == playerOptionsToSave.playerID) {
						playerLocated = true;
						listIndex = j;
					}
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("EXTRA INFO\n");
			print ("Number of options: " + playerOptionsToSave.optionsList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
			print ("Player Located " + playerLocated);
			print ("Player ID " + playerOptionsToSave.playerID);

			showDetailedDebugInfo ();
		}

		//if the save is located, check if the player id exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLocated) {
				infoListToSave [saveSlotIndex].playerOptionsList [listIndex].optionsList = playerOptionsToSave.optionsList;
			} else {
				infoListToSave [saveSlotIndex].playerOptionsList.Add (playerOptionsToSave);
			}
		} else {
			newPersistancePlayerOptionsListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistancePlayerOptionsListBySaveSlotInfo.playerOptionsList.Add (playerOptionsToSave);
			infoListToSave.Add (newPersistancePlayerOptionsListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Open (currentSaveDataPath, FileMode.OpenOrCreate); 
		bf.Serialize (file, infoListToSave);

		file.Close ();

		if (saveInfoOnRegularTxtFile) {
			writeDetailedDebugInfo (false);
		}
	}

	public void loadGameContent (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		if (mainPlayerOptionsEditorSystem == null) {
			return;
		}

		if (!mainPlayerOptionsEditorSystem.playerOptionsEditorEnabled) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Loading player options");
		}
			
		persistanceInfoList = new List<persistanceOptionsInfo> ();

		//need to store and check the current slot saved and the player which is saving, to get that concrete info
		List<persistancePlayerOptionsListBySaveSlotInfo> infoListToLoad = new List<persistancePlayerOptionsListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToLoad = currentData as List<persistancePlayerOptionsListBySaveSlotInfo>;

			file.Close ();	
		}
			
		if (saveNumberToLoad > -1) {
			persistancePlayerOptionsListBySaveSlotInfo newPersistancePlayerOptionsListBySaveSlotInfo = new persistancePlayerOptionsListBySaveSlotInfo ();

			int infoListToLoadCount = infoListToLoad.Count;

			bool saveLocated = false;

			for (int j = 0; j < infoListToLoadCount; j++) {

				if (!saveLocated) {

					if (ignoreSaveNumberOnSaveLoadInfo || infoListToLoad [j].saveNumber == saveNumberToLoad) {
						newPersistancePlayerOptionsListBySaveSlotInfo = infoListToLoad [j];

						saveLocated = true;
					}
				}
			}

			int listIndex = -1;

			int playerOptionsListCount = newPersistancePlayerOptionsListBySaveSlotInfo.playerOptionsList.Count;

			bool playerLocated = false;

			for (int j = 0; j < playerOptionsListCount; j++) {
				if (!playerLocated) {
					if (ignorePlayerIDOnSaveLoadInfo || newPersistancePlayerOptionsListBySaveSlotInfo.playerOptionsList [j].playerID == playerID) {
						listIndex = j;

						playerLocated = true;
					}
				}
			}

			if (listIndex > -1) {
				persistanceInfoList.AddRange (newPersistancePlayerOptionsListBySaveSlotInfo.playerOptionsList [listIndex].optionsList);
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Player Options Loaded in Save Number " + saveNumberToLoad);
			print ("Number of Player Options: " + persistanceInfoList.Count);


			showDetailedDebugInfo ();
		}

		loadInfoOnMainComponent ();

		if (saveInfoOnRegularTxtFile) {
			writeDetailedDebugInfo (true);
		}
	}


	public persistancePlayerOptionsInfo getPersistanceList (int playerID, bool showDebugInfo)
	{
		persistancePlayerOptionsInfo newPersistancePlayerOptionsInfo = new persistancePlayerOptionsInfo ();

		newPersistancePlayerOptionsInfo.playerID = playerID;

		List<persistanceOptionsInfo> newPersistanceOptionsInfoList = new List<persistanceOptionsInfo> ();

		List<playerOptionsEditorSystem.optionInfo> optionInfoList = mainPlayerOptionsEditorSystem.optionInfoList;

		int optionInfoListCount = optionInfoList.Count;

		for (int k = 0; k < optionInfoListCount; k++) {
			persistanceOptionsInfo newPersistanceOptionsInfo = new persistanceOptionsInfo ();

			playerOptionsEditorSystem.optionInfo currentOptionInfo = optionInfoList [k];

			newPersistanceOptionsInfo.optionName = currentOptionInfo.Name;

			newPersistanceOptionsInfo.currentScrollBarValue = currentOptionInfo.currentScrollBarValue;
			newPersistanceOptionsInfo.currentSliderValue = currentOptionInfo.currentSliderValue;
			newPersistanceOptionsInfo.currentToggleValue = currentOptionInfo.currentToggleValue;
			newPersistanceOptionsInfo.currentDropDownValue = currentOptionInfo.currentDropDownValue;

			newPersistanceOptionsInfoList.Add (newPersistanceOptionsInfo);
		}	

		newPersistancePlayerOptionsInfo.optionsList = newPersistanceOptionsInfoList;

		return newPersistancePlayerOptionsInfo;
	}


	void loadInfoOnMainComponent ()
	{
		if (persistanceInfoList != null && persistanceInfoList.Count > 0) {

			List<playerOptionsEditorSystem.optionInfo> optionInfoList = mainPlayerOptionsEditorSystem.optionInfoList;

			int persistanceInfoListCount = persistanceInfoList.Count;

			for (int i = 0; i < persistanceInfoListCount; i++) {

				persistanceOptionsInfo currentPersistanceOptionsInfo = persistanceInfoList [i];

				int currentOptionIndex = optionInfoList.FindIndex (s => s.Name.Equals (currentPersistanceOptionsInfo.optionName));

				if (currentOptionIndex > -1) {

					playerOptionsEditorSystem.optionInfo currentOptionInfo = optionInfoList [currentOptionIndex];

					if (currentOptionInfo.useScrollBar) {
						currentOptionInfo.currentScrollBarValue = currentPersistanceOptionsInfo.currentScrollBarValue;
					}

					if (currentOptionInfo.useSlider) {
						currentOptionInfo.currentSliderValue = currentPersistanceOptionsInfo.currentSliderValue;
					}

					if (currentOptionInfo.useToggle) {
						currentOptionInfo.currentToggleValue = currentPersistanceOptionsInfo.currentToggleValue;
					}

					if (currentOptionInfo.useDropDown) {
						currentOptionInfo.currentDropDownValue = currentPersistanceOptionsInfo.currentDropDownValue;
					}
				}
			}

			mainPlayerOptionsEditorSystem.isLoadingGame = true;
		}
	}

	void showDetailedDebugInfo ()
	{
		List<playerOptionsEditorSystem.optionInfo> optionInfoList = mainPlayerOptionsEditorSystem.optionInfoList;

		int persistanceInfoListCount = persistanceInfoList.Count;

		for (int i = 0; i < persistanceInfoListCount; i++) {

			persistanceOptionsInfo currentPersistanceOptionsInfo = persistanceInfoList [i];

			int currentOptionIndex = optionInfoList.FindIndex (s => s.Name.Equals (currentPersistanceOptionsInfo.optionName));

			if (currentOptionIndex > -1) {

				playerOptionsEditorSystem.optionInfo currentOptionInfo = optionInfoList [currentOptionIndex];

				print (currentOptionInfo.Name + " " + currentOptionInfo.useScrollBar + " " + currentPersistanceOptionsInfo.currentScrollBarValue);

				print (currentOptionInfo.Name + " " + currentOptionInfo.useSlider + " " + currentPersistanceOptionsInfo.currentSliderValue);

				print (currentOptionInfo.Name + " " + currentOptionInfo.useToggle + " " + currentPersistanceOptionsInfo.currentToggleValue);

				print (currentOptionInfo.Name + " " + currentOptionInfo.useDropDown + " " + currentPersistanceOptionsInfo.currentDropDownValue);
			}
		}

		print ("\n\n");
	}

	void writeDetailedDebugInfo (bool loadingFile)
	{
		string filePath = GKC_Utils.getMainDataPath ();

		string newContent = "";

		List<playerOptionsEditorSystem.optionInfo> optionInfoList = mainPlayerOptionsEditorSystem.optionInfoList;

		if (loadingFile) {
			int persistanceInfoListCount = persistanceInfoList.Count;

			for (int i = 0; i < persistanceInfoListCount; i++) {

				persistanceOptionsInfo currentPersistanceOptionsInfo = persistanceInfoList [i];

				int currentOptionIndex = optionInfoList.FindIndex (s => s.Name.Equals (currentPersistanceOptionsInfo.optionName));

				if (currentOptionIndex > -1) {

					playerOptionsEditorSystem.optionInfo currentOptionInfo = optionInfoList [currentOptionIndex];

					newContent += currentOptionInfo.Name;

					if (currentOptionInfo.useScrollBar) {
						newContent += " " + currentPersistanceOptionsInfo.currentScrollBarValue + " " + currentOptionInfo.currentScrollBarValue;
					}

					if (currentOptionInfo.useSlider) {
						newContent += " " + currentPersistanceOptionsInfo.currentSliderValue + " " + currentOptionInfo.currentSliderValue;
					}

					if (currentOptionInfo.useToggle) {
						newContent += " " + currentPersistanceOptionsInfo.currentToggleValue + " " + currentOptionInfo.currentToggleValue;
					}

					if (currentOptionInfo.useDropDown) {
						newContent += " " + currentPersistanceOptionsInfo.currentDropDownValue + " " + currentOptionInfo.currentDropDownValue;
					}

					newContent += ("\n\n");
				}
			}
		} else {
			for (int i = 0; i < optionInfoList.Count; i++) {

				playerOptionsEditorSystem.optionInfo currentOptionInfo = optionInfoList [i];
				
				newContent += currentOptionInfo.Name;

				if (currentOptionInfo.useScrollBar) {
					newContent += " " + currentOptionInfo.currentScrollBarValue;
				}

				if (currentOptionInfo.useSlider) {
					newContent += " " + currentOptionInfo.currentSliderValue;
				}

				if (currentOptionInfo.useToggle) {
					newContent += " " + currentOptionInfo.currentToggleValue;
				}

				if (currentOptionInfo.useDropDown) {
					newContent += " " + currentOptionInfo.currentDropDownValue;
				}
			
				newContent += ("\n\n");
			}
		}

		if (loadingFile) {
			filePath += loadInfoOnRegulaFileName + ".txt";
		} else {
			filePath += saveInfoOnRegulaFileName + ".txt";
		}

		File.WriteAllText (filePath, newContent);
	}
}