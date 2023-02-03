using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class saveInventoryBankInfo : saveGameInfo
{
	public inventoryBankManager mainInventoryBankManager;

	public string mainInventoryManagerName = "Main Inventory Manager";

	List<inventoryListElement> persistanceInfoList;


	public override void saveGame (int saveNumber, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		saveGameContent (saveNumber, playerID, currentSaveDataPath, showDebugInfo);
	}

	public override void loadGame (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		loadGameContent (saveNumberToLoad, playerID, currentSaveDataPath, showDebugInfo);
	}

	public override void saveGameFromEditor (int saveNumber, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		saveGameContentFromEditor (saveNumber, playerID, currentSaveDataPath, showDebugInfo);
	}

	public void saveGameContent (int currentSaveNumber, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		getMainManager ();

		if (mainInventoryBankManager == null) {
			return;
		}

		if (!mainInventoryBankManager.saveCurrentBankInventoryToSaveFile) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving inventory bank");
		}

		bool saveLocated = false;
		bool playerLocated = false;

		int saveSlotIndex = -1;
		int listIndex = -1;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		persistanceInventoryListByPlayerInfo vendorToSave = getPersistanceList (playerID, showDebugInfo);

		persistanceInventoryListBySaveSlotInfo newPersistanceInventoryListBySaveSlotInfo = new persistanceInventoryListBySaveSlotInfo ();

		List<persistanceInventoryListBySaveSlotInfo> infoListToSave = new List<persistanceInventoryListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToSave = currentData as List<persistanceInventoryListBySaveSlotInfo>;

			file.Close ();	
		}

		int infoListToSaveCount = infoListToSave.Count;

		for (int j = 0; j < infoListToSaveCount; j++) {
			if (saveSlotIndex == -1) {
				persistanceInventoryListBySaveSlotInfo currentSlot = infoListToSave [j];

				if (currentSlot.saveNumber == currentSaveNumber) {
					newPersistanceInventoryListBySaveSlotInfo = currentSlot;
					saveLocated = true;
					saveSlotIndex = j;
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("INVENTORY BANK");
			print ("Number of objects: " + vendorToSave.inventoryObjectList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
		}

		if (saveLocated) {
			if (newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count > 0) {
				playerLocated = true;
				listIndex = 0;
			}
		}

		//if the save is located, check if the bank exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLocated) {
				infoListToSave [saveSlotIndex].playerInventoryList [listIndex].inventoryObjectList = vendorToSave.inventoryObjectList;
			} else {
				infoListToSave [saveSlotIndex].playerInventoryList.Add (vendorToSave);
			}
		} else {
			newPersistanceInventoryListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Add (vendorToSave);
			infoListToSave.Add (newPersistanceInventoryListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Open (currentSaveDataPath, FileMode.OpenOrCreate); 
		bf.Serialize (file, infoListToSave);

		file.Close ();
	}

	public void loadGameContent (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		getMainManager ();

		if (mainInventoryBankManager == null) {
			return;
		}
			
		if (showDebugInfo) {
			print ("\n\n");

			print ("Loading inventory bank");
		}

		//need to store and check the current slot saved and the player which is saving, to get that concrete info
		persistanceInfoList = new List<inventoryListElement> ();

		List<persistanceInventoryListBySaveSlotInfo> infoListToLoad = new List<persistanceInventoryListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToLoad = currentData as List<persistanceInventoryListBySaveSlotInfo>;

			file.Close ();	
		}

		if (saveNumberToLoad > -1) {
			persistanceInventoryListBySaveSlotInfo newPersistanceInventoryListBySaveSlotInfo = new persistanceInventoryListBySaveSlotInfo ();

			int infoListToLoadCount = infoListToLoad.Count;

			bool slotLocated = false;

			for (int j = 0; j < infoListToLoadCount; j++) {
				if (!slotLocated) {
					persistanceInventoryListBySaveSlotInfo currentSlot = infoListToLoad [j];
					
					if (currentSlot.saveNumber == saveNumberToLoad) {
						newPersistanceInventoryListBySaveSlotInfo = currentSlot;

						slotLocated = true;
					}
				}
			}

			int bankInventoryIndex = 0;

			if (newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count > 0) {
				persistanceInventoryListByPlayerInfo bankInventoryListToLoad = newPersistanceInventoryListBySaveSlotInfo.playerInventoryList [bankInventoryIndex];

				int inventoryObjectListCount = bankInventoryListToLoad.inventoryObjectList.Count;

				for (int j = 0; j < inventoryObjectListCount; j++) {
					inventoryListElement newInventoryListElement = new inventoryListElement ();

					persistanceInventoryObjectInfo currentInventorInfo = bankInventoryListToLoad.inventoryObjectList [j];

					newInventoryListElement.Name = currentInventorInfo.Name;
					newInventoryListElement.amount = currentInventorInfo.amount;
					newInventoryListElement.infiniteAmount = currentInventorInfo.infiniteAmount;
					newInventoryListElement.inventoryObjectName = currentInventorInfo.inventoryObjectName;

					newInventoryListElement.categoryIndex = currentInventorInfo.categoryIndex;
					newInventoryListElement.elementIndex = currentInventorInfo.elementIndex;

					persistanceInfoList.Add (newInventoryListElement);
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("INVENTORY BANK");

			print ("Inventory Bank Loaded in Save Number " + saveNumberToLoad);
			print ("Number of objects: " + persistanceInfoList.Count);

			for (int j = 0; j < persistanceInfoList.Count; j++) {
				inventoryListElement currentElement = persistanceInfoList [j];

				print ("Object Name: " + currentElement.Name + " Amount: " + currentElement.amount);
			}
		}

		loadInfoOnMainComponent ();
	}


	public persistanceInventoryListByPlayerInfo getPersistanceList (int playerID, bool showDebugInfo)
	{
		persistanceInventoryListByPlayerInfo newPersistanceInventoryListByPlayerInfo = new persistanceInventoryListByPlayerInfo ();
	
		newPersistanceInventoryListByPlayerInfo.playerID = playerID;

		List<persistanceInventoryObjectInfo> newPersistanceInventoryObjectInfoList = new List<persistanceInventoryObjectInfo> ();

		List<inventoryInfo> bankInventoryList = mainInventoryBankManager.bankInventoryList;

		int bankInventoryListCount = bankInventoryList.Count;

		for (int k = 0; k < bankInventoryListCount; k++) {
			persistanceInventoryObjectInfo newPersistanceInventoryObjectInfo = new persistanceInventoryObjectInfo ();

			inventoryInfo currentInventoryInfo = bankInventoryList [k];

			newPersistanceInventoryObjectInfo.Name = currentInventoryInfo.Name;
			newPersistanceInventoryObjectInfo.amount = currentInventoryInfo.amount;
			newPersistanceInventoryObjectInfo.infiniteAmount = currentInventoryInfo.infiniteAmount;
			newPersistanceInventoryObjectInfo.inventoryObjectName = currentInventoryInfo.Name;

			newPersistanceInventoryObjectInfo.categoryIndex = currentInventoryInfo.categoryIndex;
			newPersistanceInventoryObjectInfo.elementIndex = currentInventoryInfo.elementIndex;

			newPersistanceInventoryObjectInfoList.Add (newPersistanceInventoryObjectInfo);
		}	

		newPersistanceInventoryListByPlayerInfo.inventoryObjectList = newPersistanceInventoryObjectInfoList;

		return newPersistanceInventoryListByPlayerInfo;
	}


	void loadInfoOnMainComponent ()
	{
		if (persistanceInfoList != null && persistanceInfoList.Count > 0) {
			mainInventoryBankManager.setNewInventoryListManagerList (persistanceInfoList);
		}
	}


	void getMainManager ()
	{
		if (mainInventoryBankManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainInventoryManagerName, typeof(inventoryListManager));

			inventoryListManager mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

			mainInventoryBankManager = mainInventoryListManager.getMainInventoryBankManager ();
		}
	}


	public void saveGameContentFromEditor (int currentSaveNumber, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		getMainManager ();

		if (mainInventoryBankManager == null) {
			return;
		}

		bool saveLocated = false;
		bool playerLocated = false;

		int saveSlotIndex = -1;
		int listIndex = -1;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		persistanceInventoryListByPlayerInfo vendorToSave = getPersistanceVendorListElement (playerID, showDebugInfo);

		persistanceInventoryListBySaveSlotInfo newPersistanceInventoryListBySaveSlotInfo = new persistanceInventoryListBySaveSlotInfo ();

		List<persistanceInventoryListBySaveSlotInfo> infoListToSave = new List<persistanceInventoryListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToSave = currentData as List<persistanceInventoryListBySaveSlotInfo>;

			file.Close ();	
		}

		int infoListToSaveCount = infoListToSave.Count;

		for (int j = 0; j < infoListToSaveCount; j++) {
			if (saveSlotIndex == -1) {
				persistanceInventoryListBySaveSlotInfo currentSlot = infoListToSave [j];
				
				if (currentSlot.saveNumber == currentSaveNumber) {
					newPersistanceInventoryListBySaveSlotInfo = currentSlot;
					saveLocated = true;
					saveSlotIndex = j;
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("INVENTORY BANK");

			print ("Number of objects: " + vendorToSave.inventoryObjectList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
		}

		if (saveLocated) {
			if (newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count > 0) {
				playerLocated = true;
				listIndex = 0;
			}
		}

		//if the save is located, check if the bank exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLocated) {
				infoListToSave [saveSlotIndex].playerInventoryList [listIndex].inventoryObjectList = vendorToSave.inventoryObjectList;
			} else {
				infoListToSave [saveSlotIndex].playerInventoryList.Add (vendorToSave);
			}
		} else {
			newPersistanceInventoryListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Add (vendorToSave);
			infoListToSave.Add (newPersistanceInventoryListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Create (currentSaveDataPath); 
		bf.Serialize (file, infoListToSave);
		file.Close ();
	}

	public persistanceInventoryListByPlayerInfo getPersistanceVendorListElement (int playerID, bool showDebugInfo)
	{
		persistanceInventoryListByPlayerInfo newPersistanceInventoryListByPlayerInfo = new persistanceInventoryListByPlayerInfo ();
	
		newPersistanceInventoryListByPlayerInfo.playerID = playerID;

		List<persistanceInventoryObjectInfo> newPersistanceInventoryObjectInfoList = new List<persistanceInventoryObjectInfo> ();

		List<inventoryListElement> inventoryListManagerList = mainInventoryBankManager.inventoryListManagerList;

		int inventoryListManagerListCount = inventoryListManagerList.Count;

		for (int k = 0; k < inventoryListManagerListCount; k++) {
			persistanceInventoryObjectInfo newPersistanceInventoryObjectInfo = new persistanceInventoryObjectInfo ();

			inventoryListElement currentElement = inventoryListManagerList [k];

			newPersistanceInventoryObjectInfo.Name = currentElement.Name;
			newPersistanceInventoryObjectInfo.amount = currentElement.amount;
			newPersistanceInventoryObjectInfo.infiniteAmount = currentElement.infiniteAmount;
			newPersistanceInventoryObjectInfo.inventoryObjectName = currentElement.inventoryObjectName;

			newPersistanceInventoryObjectInfo.elementIndex = currentElement.elementIndex;
			newPersistanceInventoryObjectInfo.categoryIndex = currentElement.categoryIndex;

			if (showDebugInfo) {
				print (newPersistanceInventoryObjectInfo.Name + " " + newPersistanceInventoryObjectInfo.amount);
			}

			newPersistanceInventoryObjectInfoList.Add (newPersistanceInventoryObjectInfo);
		}	

		newPersistanceInventoryListByPlayerInfo.inventoryObjectList = newPersistanceInventoryObjectInfoList;

		return newPersistanceInventoryListByPlayerInfo;
	}
}
