using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class saveVendorInfo : saveGameInfo
{
	public vendorSystem mainVendorSystem;

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

		if (mainVendorSystem == null) {
			return;
		}

		if (!mainVendorSystem.saveCurrentVendorInventoryToSaveFile) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving vendor");
		}


		bool saveLocated = false;
		bool vendorLocated = false;

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

			print ("VENDOR SYSTEM");
			print ("Number of objects: " + vendorToSave.inventoryObjectList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
		}

		if (saveLocated) {
			if (newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count > 0) {
				vendorLocated = true;
				listIndex = 0;
			}
		}

		//if the save is located, check if the vendor exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (vendorLocated) {
				infoListToSave [saveSlotIndex].playerInventoryList [listIndex].inventoryObjectList = vendorToSave.inventoryObjectList;
			} else {
				infoListToSave [saveSlotIndex].playerInventoryList.Add (vendorToSave);
			}
		} else {
			newPersistanceInventoryListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Add (vendorToSave);
			infoListToSave.Add (newPersistanceInventoryListBySaveSlotInfo);
		}

		if (showDebugInfo) {
			
			print ("\n\n");

			for (int j = 0; j < vendorToSave.inventoryObjectList.Count; j++) {
				persistanceInventoryObjectInfo currentInfo = vendorToSave.inventoryObjectList [j];

				print ("Object Name: " + currentInfo.Name + " Amount: " + currentInfo.amount);
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

		if (mainVendorSystem == null) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Loading vendor");
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

			int vendorIndex = 0;

			if (newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count > 0) {
				persistanceInventoryListByPlayerInfo vendorListToLoad = newPersistanceInventoryListBySaveSlotInfo.playerInventoryList [vendorIndex];

				int inventoryObjectListCount = vendorListToLoad.inventoryObjectList.Count;

				for (int j = 0; j < inventoryObjectListCount; j++) {

					inventoryListElement newInventoryListElement = new inventoryListElement ();

					persistanceInventoryObjectInfo currentInventoryInfo = vendorListToLoad.inventoryObjectList [j];

					newInventoryListElement.Name = currentInventoryInfo.Name;
					newInventoryListElement.amount = currentInventoryInfo.amount;
					newInventoryListElement.infiniteAmount = currentInventoryInfo.infiniteAmount;
					newInventoryListElement.inventoryObjectName = currentInventoryInfo.inventoryObjectName;

					newInventoryListElement.categoryIndex = currentInventoryInfo.categoryIndex;
					newInventoryListElement.elementIndex = currentInventoryInfo.elementIndex;

					newInventoryListElement.vendorPrice = currentInventoryInfo.vendorPrice;
					newInventoryListElement.sellPrice = currentInventoryInfo.sellPrice;

					newInventoryListElement.useMinLevelToBuy = currentInventoryInfo.useMinLevelToBuy;
					newInventoryListElement.minLevelToBuy = currentInventoryInfo.minLevelToBuy;

					newInventoryListElement.spawnObject = currentInventoryInfo.spawnObject;

					persistanceInfoList.Add (newInventoryListElement);
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("VENDOR SYSTEM");

			print ("Vendor Loaded in Save Number " + saveNumberToLoad);
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
	
		List<inventoryInfo> vendorInventoryList = mainVendorSystem.vendorInventoryList;

		if (showDebugInfo) {
			print ("\n\n");
		}

		int vendorInventoryListCount = vendorInventoryList.Count;

		for (int k = 0; k < vendorInventoryListCount; k++) {
			persistanceInventoryObjectInfo newPersistanceInventoryObjectInfo = new persistanceInventoryObjectInfo ();

			inventoryInfo currentInventoryInfo = vendorInventoryList [k];

			newPersistanceInventoryObjectInfo.Name = currentInventoryInfo.Name;
			newPersistanceInventoryObjectInfo.amount = currentInventoryInfo.amount;
			newPersistanceInventoryObjectInfo.infiniteAmount = currentInventoryInfo.infiniteAmount;
			newPersistanceInventoryObjectInfo.inventoryObjectName = currentInventoryInfo.Name;

			newPersistanceInventoryObjectInfo.elementIndex = currentInventoryInfo.elementIndex;
			newPersistanceInventoryObjectInfo.categoryIndex = currentInventoryInfo.categoryIndex;

			newPersistanceInventoryObjectInfo.vendorPrice = currentInventoryInfo.vendorPrice;
			newPersistanceInventoryObjectInfo.sellPrice = currentInventoryInfo.sellPrice;

			newPersistanceInventoryObjectInfo.useMinLevelToBuy = currentInventoryInfo.useMinLevelToBuy;
			newPersistanceInventoryObjectInfo.minLevelToBuy = currentInventoryInfo.minLevelToBuy;

			newPersistanceInventoryObjectInfo.spawnObject = currentInventoryInfo.spawnObject;

			if (showDebugInfo) {
				print (newPersistanceInventoryObjectInfo.Name + " " + newPersistanceInventoryObjectInfo.amount);
			}

			newPersistanceInventoryObjectInfoList.Add (newPersistanceInventoryObjectInfo);
		}	

		newPersistanceInventoryListByPlayerInfo.inventoryObjectList = newPersistanceInventoryObjectInfoList;

		return newPersistanceInventoryListByPlayerInfo;
	}


	void loadInfoOnMainComponent ()
	{
		if (persistanceInfoList != null && persistanceInfoList.Count > 0) {
			mainVendorSystem.setNewInventoryListManagerList (persistanceInfoList);
		}
	}


	void getMainManager ()
	{
		if (mainVendorSystem == null) {
			mainVendorSystem = FindObjectOfType<vendorSystem> ();
		}
	}


	public void saveGameContentFromEditor (int currentSaveNumber, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		getMainManager ();

		if (mainVendorSystem == null) {
			return;
		}

		bool saveLocated = false;
		bool vendorLocated = false;

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
			print ("VENDOR SYSTEM");

			print ("Number of objects: " + vendorToSave.inventoryObjectList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
		}

		if (saveLocated) {
			if (newPersistanceInventoryListBySaveSlotInfo.playerInventoryList.Count > 0) {
				vendorLocated = true;
				listIndex = 0;
			}
		}

		//if the save is located, check if the vendor exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (vendorLocated) {
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
	
		List<inventoryListElement> inventoryListManagerList = mainVendorSystem.inventoryListManagerList;

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
	
			newPersistanceInventoryObjectInfo.vendorPrice = currentElement.vendorPrice;
			newPersistanceInventoryObjectInfo.sellPrice = currentElement.sellPrice;
	
			newPersistanceInventoryObjectInfo.useMinLevelToBuy = currentElement.useMinLevelToBuy;
			newPersistanceInventoryObjectInfo.minLevelToBuy = currentElement.minLevelToBuy;
	
			newPersistanceInventoryObjectInfo.spawnObject = currentElement.spawnObject;
	
			if (showDebugInfo) {
				print (newPersistanceInventoryObjectInfo.Name + " " + newPersistanceInventoryObjectInfo.amount);
			}

			newPersistanceInventoryObjectInfoList.Add (newPersistanceInventoryObjectInfo);
		}	
	
		newPersistanceInventoryListByPlayerInfo.inventoryObjectList = newPersistanceInventoryObjectInfoList;
	
		return newPersistanceInventoryListByPlayerInfo;
	}
}