using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class saveMeleeWeaponsInfo : saveGameInfo
{
	public meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager;

	List<persistanceMeleeInfo> persistanceInfoList;

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
		if (mainMeleeWeaponsGrabbedManager == null) {
			return;
		}

		if (!mainMeleeWeaponsGrabbedManager.meleeWeaponsGrabbedManagerEnabled) {
			return;
		}

		if (!mainMeleeWeaponsGrabbedManager.saveCurrentMeleeWeaponListToSaveFile) {
			return;
		}

		if (mainMeleeWeaponsGrabbedManager.storePickedWeaponsOnInventory) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving melee weapons");
		}

		bool saveLocated = false;
		bool playerLocated = false;

		int saveSlotIndex = -1;
		int listIndex = -1;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		persistanceMeleeWeaponInfo meleeWeaponsToSave = getPersistanceList (playerID, showDebugInfo);

		persistanceMeleeWeaponListBySaveSlotInfo newPersistanceMeleeWeaponListBySaveSlotInfo = new persistanceMeleeWeaponListBySaveSlotInfo ();

		List<persistanceMeleeWeaponListBySaveSlotInfo> infoListToSave = new List<persistanceMeleeWeaponListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);

			infoListToSave = currentData as List<persistanceMeleeWeaponListBySaveSlotInfo>;

			file.Close ();	
		}

		int infoListToSaveCount = infoListToSave.Count;

		for (int j = 0; j < infoListToSaveCount; j++) {
			if (infoListToSave [j].saveNumber == currentSaveNumber) {
				newPersistanceMeleeWeaponListBySaveSlotInfo = infoListToSave [j];

				saveLocated = true;
				saveSlotIndex = j;
			}
		}

		if (saveLocated) {
			int meleeWeaponListCount = newPersistanceMeleeWeaponListBySaveSlotInfo.meleeWeaponList.Count; 

			for (int j = 0; j < meleeWeaponListCount; j++) {
				if (newPersistanceMeleeWeaponListBySaveSlotInfo.meleeWeaponList [j].playerID == meleeWeaponsToSave.playerID) {
					playerLocated = true;
					listIndex = j;
				}
			}
		}

		//if the save is located, check if the player id exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLocated) {
				infoListToSave [saveSlotIndex].meleeWeaponList [listIndex].meleeList = meleeWeaponsToSave.meleeList;
			} else {
				infoListToSave [saveSlotIndex].meleeWeaponList.Add (meleeWeaponsToSave);
			}
		} else {
			newPersistanceMeleeWeaponListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistanceMeleeWeaponListBySaveSlotInfo.meleeWeaponList.Add (meleeWeaponsToSave);

			infoListToSave.Add (newPersistanceMeleeWeaponListBySaveSlotInfo);
		}

		bf = new BinaryFormatter ();
		file = File.Open (currentSaveDataPath, FileMode.OpenOrCreate); 
		bf.Serialize (file, infoListToSave);

		file.Close ();

		if (showDebugInfo) {
			print ("\n\n");

			print ("Melee Weapons Saved in Save Number " + currentSaveNumber);
		}
	}

	public void loadGameContent (int saveNumberToLoad, int playerID, string currentSaveDataPath, bool showDebugInfo)
	{
		if (mainMeleeWeaponsGrabbedManager == null) {
			return;
		}

		if (!mainMeleeWeaponsGrabbedManager.meleeWeaponsGrabbedManagerEnabled) {
			return;
		}

		if (mainMeleeWeaponsGrabbedManager.storePickedWeaponsOnInventory) {
			return;
		}

		initializeValues ();

		if (showDebugInfo) {
			print ("\n\n");

			print ("Loading melee weapons");
		}

		//need to store and check the current slot saved and the player which is saving, to get that concrete info
		persistanceInfoList = new List<persistanceMeleeInfo> ();

		List<persistanceMeleeWeaponListBySaveSlotInfo> infoListToLoad = new List<persistanceMeleeWeaponListBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);

			infoListToLoad = currentData as List<persistanceMeleeWeaponListBySaveSlotInfo>;

			file.Close ();	
		}

		if (saveNumberToLoad > -1) {
			persistanceMeleeWeaponListBySaveSlotInfo newPersistanceMeleeWeaponListBySaveSlotInfo = new persistanceMeleeWeaponListBySaveSlotInfo ();

			int infoListToLoadCount = infoListToLoad.Count;

			for (int j = 0; j < infoListToLoadCount; j++) {

				if (infoListToLoad [j].saveNumber == saveNumberToLoad) {
					newPersistanceMeleeWeaponListBySaveSlotInfo = infoListToLoad [j];
				}
			}

			int listIndex = -1;

			int meleeWeaponListCount = newPersistanceMeleeWeaponListBySaveSlotInfo.meleeWeaponList.Count;

			for (int j = 0; j < meleeWeaponListCount; j++) {

				if (newPersistanceMeleeWeaponListBySaveSlotInfo.meleeWeaponList [j].playerID == playerID) {
					listIndex = j;
				}
			}

			if (listIndex > -1) {
				persistanceInfoList.AddRange (newPersistanceMeleeWeaponListBySaveSlotInfo.meleeWeaponList [listIndex].meleeList);
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Melee Weapons Loaded in Save Number " + saveNumberToLoad);
			print ("Number of Melee Weapons: " + persistanceInfoList.Count);
		}

		loadInfoOnMainComponent ();
	}


	persistanceMeleeWeaponInfo getPersistanceList (int playerID, bool showDebugInfo)
	{
		persistanceMeleeWeaponInfo newPersistanceMeleeWeaponInfo = new persistanceMeleeWeaponInfo ();

		newPersistanceMeleeWeaponInfo.playerID = playerID;

		List<persistanceMeleeInfo> newPersistanceMeleeInfoList = new List<persistanceMeleeInfo> ();

		List<meleeWeaponsGrabbedManager.meleeWeaponGrabbedInfo> meleeWeaponGrabbedInfoList = mainMeleeWeaponsGrabbedManager.meleeWeaponGrabbedInfoList;

		int meleeWeaponGrabbedInfoListCount = meleeWeaponGrabbedInfoList.Count;

		for (int k = 0; k < meleeWeaponGrabbedInfoListCount; k++) {
			persistanceMeleeInfo newPersistanceMeleeInfo = new persistanceMeleeInfo ();

			meleeWeaponsGrabbedManager.meleeWeaponGrabbedInfo currentWeapon = meleeWeaponGrabbedInfoList [k];

			newPersistanceMeleeInfo.weaponActiveIndex = currentWeapon.weaponPrefabIndex;
			newPersistanceMeleeInfo.isCurrentWeapon = currentWeapon.isCurrentWeapon;

			newPersistanceMeleeInfoList.Add (newPersistanceMeleeInfo);

			if (showDebugInfo) {
				print ("Weapon " + currentWeapon.Name + " " + currentWeapon.weaponPrefabIndex + " " + currentWeapon.isCurrentWeapon);
			}
		}	

		newPersistanceMeleeWeaponInfo.meleeList = newPersistanceMeleeInfoList;

		return newPersistanceMeleeWeaponInfo;
	}


	void loadInfoOnMainComponent ()
	{
		valuesInitializedOnLoad = true;

		initializeValues ();

		if (persistanceInfoList != null && persistanceInfoList.Count > 0) {

			List<meleeWeaponsGrabbedManager.meleeWeaponPrefabInfo> meleeWeaponPrefabInfoList = mainMeleeWeaponsGrabbedManager.meleeWeaponPrefabInfoList;

			List<meleeWeaponsGrabbedManager.meleeWeaponGrabbedInfo> meleeWeaponGrabbedInfoList = mainMeleeWeaponsGrabbedManager.meleeWeaponGrabbedInfoList;

			int persistanceInfoListCount = persistanceInfoList.Count;

			for (int i = 0; i < persistanceInfoListCount; i++) {

				persistanceMeleeInfo currentPersistanceMeleeInfo = persistanceInfoList [i];

				meleeWeaponsGrabbedManager.meleeWeaponPrefabInfo currentMeleeWeaponPrefabInfo = meleeWeaponPrefabInfoList [currentPersistanceMeleeInfo.weaponActiveIndex];

				if (currentMeleeWeaponPrefabInfo != null) {
					meleeWeaponsGrabbedManager.meleeWeaponGrabbedInfo newMeleeWeaponGrabbedInfo = new meleeWeaponsGrabbedManager.meleeWeaponGrabbedInfo ();

					newMeleeWeaponGrabbedInfo.Name = currentMeleeWeaponPrefabInfo.Name;

					newMeleeWeaponGrabbedInfo.weaponPrefabIndex = currentMeleeWeaponPrefabInfo.weaponPrefabIndex;

					meleeWeaponGrabbedInfoList.Add (newMeleeWeaponGrabbedInfo);

					if (currentPersistanceMeleeInfo.isCurrentWeapon) {
						mainMeleeWeaponsGrabbedManager.currentWeaponIndex = currentMeleeWeaponPrefabInfo.weaponPrefabIndex;
					}
				}
			}

			if (!mainMeleeWeaponsGrabbedManager.isGrabObjectsEnabled ()) {
				return;
			}
						
			int meleeWeaponGrabbedInfoListCount = meleeWeaponGrabbedInfoList.Count;

			for (int k = 0; k < meleeWeaponGrabbedInfoListCount; k++) {
				meleeWeaponsGrabbedManager.meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [k];

				currentMeleeWeaponGrabbedInfo.weaponInstantiated = true;

				meleeWeaponsGrabbedManager.meleeWeaponPrefabInfo currentMeleeWeaponPrefabInfo = mainMeleeWeaponsGrabbedManager.getWeaponPrefabByName (currentMeleeWeaponGrabbedInfo.Name);

				if (currentMeleeWeaponPrefabInfo != null) {
					currentMeleeWeaponGrabbedInfo.weaponStored = (GameObject)Instantiate (currentMeleeWeaponPrefabInfo.weaponPrefab, Vector3.up * 1000, Quaternion.identity);

					currentMeleeWeaponGrabbedInfo.weaponPrefabIndex = currentMeleeWeaponPrefabInfo.weaponPrefabIndex;

					mainMeleeWeaponsGrabbedManager.checkObjectMeshToEnableOrDisable (!currentMeleeWeaponGrabbedInfo.hideWeaponMeshWhenNotUsed, currentMeleeWeaponGrabbedInfo);

					currentMeleeWeaponGrabbedInfo.weaponStored.SetActive (false);
				}
			}

			mainMeleeWeaponsGrabbedManager.isLoadingGame = true;
		}

		mainMeleeWeaponsGrabbedManager.initializeMeleeManagerValues ();
	}

	void initializeValues ()
	{
		mainMeleeWeaponsGrabbedManager.isLoadingGame = false;

		if (!valuesInitializedOnLoad) {
			mainMeleeWeaponsGrabbedManager.initializeMeleeManagerValues ();
		}
	}
}