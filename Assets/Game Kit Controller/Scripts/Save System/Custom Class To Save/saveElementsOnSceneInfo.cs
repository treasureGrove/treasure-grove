using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class saveElementsOnSceneInfo : saveGameInfo
{
	public elementsOnSceneSystem mainElementsOnSceneSystem;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<persistanceElementOnSceneInfo> persistanceInfoList;

	public persistancePlayerElementOnSceneInfo temporalPersistancePlayerElementOnSceneInfo;

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

		if (mainElementsOnSceneSystem == null) {
			return;
		}

		if (!mainElementsOnSceneSystem.saveCurrentPlayerElementsOnSceneToSaveFile) {
			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Saving elements on scene");
		}

		bool saveLocated = false;
		bool playerLocated = false;

		int saveSlotIndex = -1;
		int listIndex = -1;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		persistancePlayerElementOnSceneInfo elementsOnSceneToSave = getPersistanceList (playerID, showDebugInfo);

		persistanceElementsOnSceneBySaveSlotInfo newPersistancePlayerElementsOnSceneListBySaveSlotInfo = new persistanceElementsOnSceneBySaveSlotInfo ();

		List<persistanceElementsOnSceneBySaveSlotInfo> infoListToSave = new List<persistanceElementsOnSceneBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			bf = new BinaryFormatter ();
			file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToSave = currentData as List<persistanceElementsOnSceneBySaveSlotInfo>;

			file.Close ();	
		}

		int infoListToSaveCount = infoListToSave.Count;

		for (int j = 0; j < infoListToSaveCount; j++) {
			if (infoListToSave [j].saveNumber == currentSaveNumber) {
				newPersistancePlayerElementsOnSceneListBySaveSlotInfo = infoListToSave [j];
				saveLocated = true;
				saveSlotIndex = j;
			}
		}

		if (saveLocated) {
			int playerElementsOnSceneListCount = newPersistancePlayerElementsOnSceneListBySaveSlotInfo.playerElementsOnSceneList.Count;

			for (int j = 0; j < playerElementsOnSceneListCount; j++) {
				if (newPersistancePlayerElementsOnSceneListBySaveSlotInfo.playerElementsOnSceneList [j].playerID == elementsOnSceneToSave.playerID) {
					playerLocated = true;
					listIndex = j;
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("EXTRA INFO\n");
			print ("Number of elements on scene: " + elementsOnSceneToSave.elementOnSceneList.Count);
			print ("Current Save Number " + currentSaveNumber);
			print ("Save Located " + saveLocated);
			print ("Player Located " + playerLocated);
			print ("Player ID " + elementsOnSceneToSave.playerID);

			print ("Scene Index " + menuPause.getCurrentActiveSceneIndex ());
		}

		int currentSceneIndex = menuPause.getCurrentActiveSceneIndex ();

		//if the save is located, check if the player id exists
		if (saveLocated) {
			//if player id exists, overwrite it
			if (playerLocated) {
				int elementOnSceneListCount = infoListToSave [saveSlotIndex].playerElementsOnSceneList [listIndex].elementOnSceneList.Count;

				for (int i = elementOnSceneListCount - 1; i >= 0; i--) {
					if (currentSceneIndex == infoListToSave [saveSlotIndex].playerElementsOnSceneList [listIndex].elementOnSceneList [i].elementScene) {
						infoListToSave [saveSlotIndex].playerElementsOnSceneList [listIndex].elementOnSceneList.RemoveAt (i);
					}
				}

				infoListToSave [saveSlotIndex].playerElementsOnSceneList [listIndex].elementOnSceneList.AddRange (elementsOnSceneToSave.elementOnSceneList);
			} else {
				infoListToSave [saveSlotIndex].playerElementsOnSceneList.Add (elementsOnSceneToSave);
			}
		} else {
			newPersistancePlayerElementsOnSceneListBySaveSlotInfo.saveNumber = currentSaveNumber;
			newPersistancePlayerElementsOnSceneListBySaveSlotInfo.playerElementsOnSceneList.Add (elementsOnSceneToSave);
			infoListToSave.Add (newPersistancePlayerElementsOnSceneListBySaveSlotInfo);
		}

		if (showDebugInfo) {
			print ("\n\n");

			for (int j = 0; j < elementsOnSceneToSave.elementOnSceneList.Count; j++) {
				persistanceElementOnSceneInfo currentPersistanceElementsOnSceneInfo = elementsOnSceneToSave.elementOnSceneList [j];

				if (currentPersistanceElementsOnSceneInfo.elementScene == currentSceneIndex) {
					showDetailedDebugInfo (currentPersistanceElementsOnSceneInfo);
				}
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

		if (mainElementsOnSceneSystem == null) {
			return;
		}

		if (!mainElementsOnSceneSystem.saveCurrentPlayerElementsOnSceneToSaveFile) {
			initializeValues ();

			return;
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Loading elements on scene");
		}

		//need to store and check the current slot saved and the player which is saving, to get that concrete info
		persistanceInfoList = new List<persistanceElementOnSceneInfo> ();

		List<persistanceElementsOnSceneBySaveSlotInfo> infoListToLoad = new List<persistanceElementsOnSceneBySaveSlotInfo> ();

		if (File.Exists (currentSaveDataPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath, FileMode.Open);
			object currentData = bf.Deserialize (file);
			infoListToLoad = currentData as List<persistanceElementsOnSceneBySaveSlotInfo>;

			file.Close ();	
		}

		if (saveNumberToLoad > -1) {
			persistanceElementsOnSceneBySaveSlotInfo newPersistancePlayerElementsOnSceneListBySaveSlotInfo = new persistanceElementsOnSceneBySaveSlotInfo ();

			int infoListToLoadCount = infoListToLoad.Count;

			for (int j = 0; j < infoListToLoadCount; j++) {

				if (infoListToLoad [j].saveNumber == saveNumberToLoad) {
					newPersistancePlayerElementsOnSceneListBySaveSlotInfo = infoListToLoad [j];
				}
			}

			int listIndex = -1;

			int playerElementsOnSceneListCount = newPersistancePlayerElementsOnSceneListBySaveSlotInfo.playerElementsOnSceneList.Count;

			for (int j = 0; j < playerElementsOnSceneListCount; j++) {

				if (newPersistancePlayerElementsOnSceneListBySaveSlotInfo.playerElementsOnSceneList [j].playerID == playerID) {
					listIndex = j;
				}
			}

			if (listIndex > -1) {
				int currentSceneIndex = menuPause.getCurrentActiveSceneIndex ();

				temporalPersistancePlayerElementOnSceneInfo = newPersistancePlayerElementsOnSceneListBySaveSlotInfo.playerElementsOnSceneList [listIndex];

				int elementOnSceneListCount = temporalPersistancePlayerElementOnSceneInfo.elementOnSceneList.Count;

				if (showDebugInfo) {
					print ("\n\n");

					print ("Total number of elements saved in game " + elementOnSceneListCount);
				}

				for (int j = 0; j < elementOnSceneListCount; j++) {
					if (currentSceneIndex == temporalPersistancePlayerElementOnSceneInfo.elementOnSceneList [j].elementScene) {
						persistanceInfoList.Add (temporalPersistancePlayerElementOnSceneInfo.elementOnSceneList [j]);
					}
				}
			}
		}

		if (showDebugInfo) {
			print ("\n\n");

			print ("Elements Loaded in Save Number " + saveNumberToLoad);
			print ("Number of Elements: " + persistanceInfoList.Count);

			print ("Scene Index " + menuPause.getCurrentActiveSceneIndex ());

			print ("\n\n");

			for (int j = 0; j < persistanceInfoList.Count; j++) {
				print ("\n\n");

				persistanceElementOnSceneInfo currentPersistanceElementsOnSceneInfo = persistanceInfoList [j];

				showDetailedDebugInfo (currentPersistanceElementsOnSceneInfo);
			}
		}

		showCurrentDebugInfo = showDebugInfo;

		loadInfoOnMainComponent ();
	}

	public persistancePlayerElementOnSceneInfo getPersistanceList (int playerID, bool showDebugInfo)
	{
		persistancePlayerElementOnSceneInfo newElementsOnSceneList = new persistancePlayerElementOnSceneInfo ();

		newElementsOnSceneList.playerID = playerID;

		List<persistanceElementOnSceneInfo> newPersistanceElementsOnSceneInfoList = new List<persistanceElementOnSceneInfo> ();

		mainElementsOnSceneSystem.mainElementOnSceneManager.checkForInstantiatedElementsOnSceneOnSave ();

		List<elementOnScene> elementOnSceneList = mainElementsOnSceneSystem.mainElementOnSceneManager.elementOnSceneList;

		int elementOnSceneListCount = elementOnSceneList.Count;

		for (int k = 0; k < elementOnSceneListCount; k++) {
			persistanceElementOnSceneInfo newPersistanceElementsOnSceneInfo = new persistanceElementOnSceneInfo ();

			elementOnScene currentSlotInfo = elementOnSceneList [k];

			if (currentSlotInfo != null && currentSlotInfo.isSaveElementEnabled ()) {
				newPersistanceElementsOnSceneInfo.elementScene = currentSlotInfo.elementScene;
				newPersistanceElementsOnSceneInfo.elementID = currentSlotInfo.elementID;

				newPersistanceElementsOnSceneInfo.elementActiveState = currentSlotInfo.elementActiveState;

				if (currentSlotInfo.useStats) {

					currentSlotInfo.checkEventOnStatsSave ();

					newPersistanceElementsOnSceneInfo.useStats = true;

					for (int j = 0; j < currentSlotInfo.statInfoList.Count; j++) {
						if (currentSlotInfo.statInfoList [j].statIsAmount) {
							newPersistanceElementsOnSceneInfo.floatValueStatList.Add (currentSlotInfo.statInfoList [j].currentFloatValue);
						} else {
							newPersistanceElementsOnSceneInfo.boolValueStatList.Add (currentSlotInfo.statInfoList [j].currentBoolState);
						}
					}
				}

				newPersistanceElementsOnSceneInfo.savePositionValues = currentSlotInfo.savePositionValues;

				Vector3 currentPosition = currentSlotInfo.getElementPosition ();

				newPersistanceElementsOnSceneInfo.positionX = currentPosition.x;
				newPersistanceElementsOnSceneInfo.positionY = currentPosition.y;
				newPersistanceElementsOnSceneInfo.positionZ = currentPosition.z;	

				newPersistanceElementsOnSceneInfo.saveRotationValues = currentSlotInfo.saveRotationValues;

				Vector3 currentRotation = currentSlotInfo.getElementRotation ();

				newPersistanceElementsOnSceneInfo.rotationX = currentRotation.x;
				newPersistanceElementsOnSceneInfo.rotationY = currentRotation.y;
				newPersistanceElementsOnSceneInfo.rotationZ = currentRotation.z; 

				newPersistanceElementsOnSceneInfo.elementPrefabID = currentSlotInfo.elementPrefabID; 

				newPersistanceElementsOnSceneInfo.useElementPrefabID = currentSlotInfo.useElementPrefabID; 

//				if (showDebugInfo) {
//					showDetailedDebugInfo (newPersistanceElementsOnSceneInfo);
//				}

				newPersistanceElementsOnSceneInfoList.Add (newPersistanceElementsOnSceneInfo);
			}
		}	


		List<elementOnSceneManager.temporalElementOnSceneInfo> temporalElementOnSceneList = mainElementsOnSceneSystem.mainElementOnSceneManager.temporalElementOnSceneInfoList;

		int temporalElementOnSceneListCount = temporalElementOnSceneList.Count;

		for (int k = 0; k < temporalElementOnSceneListCount; k++) {
			int elementIndex = getElementOnSceneIndex (newPersistanceElementsOnSceneInfoList, temporalElementOnSceneList [k].elementID,
				                   temporalElementOnSceneList [k].elementScene);

			if (elementIndex > -1) {
				newPersistanceElementsOnSceneInfoList [elementIndex].elementActiveState = temporalElementOnSceneList [k].elementActiveState;
			} else {
				persistanceElementOnSceneInfo newPersistanceElementsOnSceneInfo = new persistanceElementOnSceneInfo ();

				newPersistanceElementsOnSceneInfo.elementScene = temporalElementOnSceneList [k].elementScene;
				newPersistanceElementsOnSceneInfo.elementID = temporalElementOnSceneList [k].elementID;

				newPersistanceElementsOnSceneInfo.elementActiveState = temporalElementOnSceneList [k].elementActiveState;

				newPersistanceElementsOnSceneInfo.useElementPrefabID = true;

				newPersistanceElementsOnSceneInfoList.Add (newPersistanceElementsOnSceneInfo);
			}
		}	
			
		newElementsOnSceneList.elementOnSceneList = newPersistanceElementsOnSceneInfoList;

		return newElementsOnSceneList;
	}

	int getElementOnSceneIndex (List<persistanceElementOnSceneInfo> newPersistanceElementsOnSceneInfoList, int elementID, int elementScene)
	{
		int newPersistanceElementsOnSceneInfoListCount = newPersistanceElementsOnSceneInfoList.Count;

		for (int j = 0; j < newPersistanceElementsOnSceneInfoListCount; j++) {
			if (newPersistanceElementsOnSceneInfoList [j].elementID == elementID &&
			    newPersistanceElementsOnSceneInfoList [j].elementScene == elementScene) {
				return j;
			}
		}

		return -1;
	}

	void loadInfoOnMainComponent ()
	{
		initializeValues ();

		if (persistanceInfoList != null && persistanceInfoList.Count > 0) {

			int persistanceInfoListCount = persistanceInfoList.Count;

			for (int k = 0; k < persistanceInfoListCount; k++) {

				persistanceElementOnSceneInfo currentPersistanceElementsOnSceneInfo = persistanceInfoList [k];

				elementOnScene currentElementOnScene = 
					mainElementsOnSceneSystem.mainElementOnSceneManager.getElementOnSceneInfo (currentPersistanceElementsOnSceneInfo.elementID, currentPersistanceElementsOnSceneInfo.elementScene);
				
				if (currentElementOnScene == null) {
					if (mainElementsOnSceneSystem.mainElementOnSceneManager.useElementsOnSceneData) {
						if (currentPersistanceElementsOnSceneInfo.useElementPrefabID) {
							if (currentPersistanceElementsOnSceneInfo.elementActiveState) {
								int elementPrefabID = currentPersistanceElementsOnSceneInfo.elementPrefabID;

								GameObject elementPrefab = mainElementsOnSceneSystem.mainElementOnSceneManager.mainElementsOnSceneData.getElementScenePrefabById (elementPrefabID);

								if (elementPrefab != null) {
									GameObject newObject = Instantiate (elementPrefab);

									newObject.name = elementPrefab.name;

									currentElementOnScene = newObject.GetComponentInChildren<elementOnScene> ();

									currentElementOnScene.saveElementEnabled = true;

									mainElementsOnSceneSystem.mainElementOnSceneManager.setNewInstantiatedElementOnSceneManagerIngameWithInfo (currentElementOnScene);

									if (showCurrentDebugInfo) {
										print ("INSTANTIATING ELEMENT ON SCENE FROM PREFAB " + newObject.name + " " + currentElementOnScene.elementID);
									}
								} else {
									if (showCurrentDebugInfo) {
										print ("INSTANTIATING ERROR ON ELEMENT ON SCENE FROM PREFAB ID" + elementPrefabID);
									}
								}
							}
						} 
					}
				}

				if (showCurrentDebugInfo) {
					if (currentElementOnScene != null) {
						print ("Element On Scene found " + currentElementOnScene.elementID);
					} else {
						print ("Element On Scene Not Found");
					}
				}

				if (currentElementOnScene != null) {
					if (showCurrentDebugInfo) {
						showDetailedDebugInfo (currentPersistanceElementsOnSceneInfo);
					}

					if (currentElementOnScene.saveElementEnabled) {
						if (currentElementOnScene.savePositionValues) {
							Vector3 newPosition = new Vector3 (currentPersistanceElementsOnSceneInfo.positionX, 
								                      currentPersistanceElementsOnSceneInfo.positionY,
								                      currentPersistanceElementsOnSceneInfo.positionZ);

							currentElementOnScene.setNewPositionValues (newPosition);
						}

						if (currentElementOnScene.saveRotationValues) {
							Vector3 newRotation = new Vector3 (currentPersistanceElementsOnSceneInfo.rotationX, 
								                      currentPersistanceElementsOnSceneInfo.rotationY,
								                      currentPersistanceElementsOnSceneInfo.rotationZ);

							currentElementOnScene.setNewRotationValues (newRotation);
						}

						currentElementOnScene.setElementActiveState (currentPersistanceElementsOnSceneInfo.elementActiveState);

						currentElementOnScene.checkStateOnLoad ();

						if (currentPersistanceElementsOnSceneInfo.elementActiveState) {
							bool canCheckStats = true;

							if (mainElementsOnSceneSystem.mainElementOnSceneManager.ignoreLoadStatsOnObjectIDList) {
								if (mainElementsOnSceneSystem.mainElementOnSceneManager.ignoreLoadStatsOnObjectIDListInfo.Contains (currentElementOnScene.elementID)) {
									canCheckStats = false;
								}
							}

							if (mainElementsOnSceneSystem.mainElementOnSceneManager.ignoreLoadStatsOnObjectPrefabIDList) {
								if (mainElementsOnSceneSystem.mainElementOnSceneManager.ignoreLoadStatsOnObjectPrefabIDListInfo.Contains (currentElementOnScene.elementPrefabID)) {
									canCheckStats = false;
								}
							}

							if (currentElementOnScene.useStats && canCheckStats) {
								int boolValueStatListCount = 0;
								int floatValueStatListCount = 0;

								for (int j = 0; j < currentElementOnScene.statInfoList.Count; j++) {
									if (currentElementOnScene.statInfoList [j].statIsAmount) {
										if (floatValueStatListCount < currentPersistanceElementsOnSceneInfo.floatValueStatList.Count) {
											currentElementOnScene.statInfoList [j].currentFloatValue = currentPersistanceElementsOnSceneInfo.floatValueStatList [floatValueStatListCount];
										}

										floatValueStatListCount++;
									} else {
										if (boolValueStatListCount < currentPersistanceElementsOnSceneInfo.boolValueStatList.Count) {
											currentElementOnScene.statInfoList [j].currentBoolState = currentPersistanceElementsOnSceneInfo.boolValueStatList [boolValueStatListCount];
										}

										boolValueStatListCount++;
									}
								}

								currentElementOnScene.setStatsOnLoad ();
							}
						}
					}
				}
			}
		}
	}

	void getMainManager ()
	{
		if (mainElementsOnSceneSystem == null) {
			mainElementsOnSceneSystem = FindObjectOfType<elementsOnSceneSystem> ();
		}
	}

	void initializeValues ()
	{
		mainElementsOnSceneSystem.initializeValues ();
	}

	void showDetailedDebugInfo (persistanceElementOnSceneInfo currentPersistanceElementsOnSceneInfo)
	{
		print ("Element Scene " + currentPersistanceElementsOnSceneInfo.elementScene);
		print ("Element ID " + currentPersistanceElementsOnSceneInfo.elementID);
		print ("Element Active State " + currentPersistanceElementsOnSceneInfo.elementActiveState);

		print ("Element Save Position Values " + currentPersistanceElementsOnSceneInfo.savePositionValues);
		print ("Element Position X-Y-Z " + currentPersistanceElementsOnSceneInfo.positionX + " " +
		currentPersistanceElementsOnSceneInfo.positionY + " " +
		currentPersistanceElementsOnSceneInfo.positionZ);
		
		print ("Element Save Rotation Values " + currentPersistanceElementsOnSceneInfo.saveRotationValues);
		print ("Element Rotation X-Y-Z " + currentPersistanceElementsOnSceneInfo.rotationX + " " +
		currentPersistanceElementsOnSceneInfo.rotationY + " " +
		currentPersistanceElementsOnSceneInfo.rotationZ);
		
		print ("Element Prefab ID " + currentPersistanceElementsOnSceneInfo.elementPrefabID);

		print ("\n\n");
	}

	public void setStatsSearchingByInfo (int currentElementScene, int currentElementID, elementOnScene currentElementOnScene)
	{
		if (temporalPersistancePlayerElementOnSceneInfo != null) {
			print ("searching stats on " + currentElementScene + " " + currentElementID + " " + currentElementOnScene.gameObject.name);

			int elementOnSceneListCount = temporalPersistancePlayerElementOnSceneInfo.elementOnSceneList.Count;

			for (int k = 0; k < elementOnSceneListCount; k++) {
				if (currentElementScene == temporalPersistancePlayerElementOnSceneInfo.elementOnSceneList [k].elementScene) {

					persistanceElementOnSceneInfo currentPersistanceElementsOnSceneInfo = temporalPersistancePlayerElementOnSceneInfo.elementOnSceneList [k];

					if (currentPersistanceElementsOnSceneInfo.elementScene == currentElementScene && currentPersistanceElementsOnSceneInfo.elementID == currentElementID) {
						bool canCheckStats = true;

						if (mainElementsOnSceneSystem.mainElementOnSceneManager.ignoreLoadStatsOnObjectIDList) {
							if (mainElementsOnSceneSystem.mainElementOnSceneManager.ignoreLoadStatsOnObjectIDListInfo.Contains (currentElementOnScene.elementID)) {
								canCheckStats = false;
							}
						}

						if (mainElementsOnSceneSystem.mainElementOnSceneManager.ignoreLoadStatsOnObjectPrefabIDList) {
							if (mainElementsOnSceneSystem.mainElementOnSceneManager.ignoreLoadStatsOnObjectPrefabIDListInfo.Contains (currentElementOnScene.elementPrefabID)) {
								canCheckStats = false;
							}
						}

						if (currentElementOnScene.useStats && canCheckStats) {
							int boolValueStatListCount = 0;
							int floatValueStatListCount = 0;

							for (int j = 0; j < currentElementOnScene.statInfoList.Count; j++) {
								if (currentElementOnScene.statInfoList [j].statIsAmount) {
									currentElementOnScene.statInfoList [j].currentFloatValue = currentPersistanceElementsOnSceneInfo.floatValueStatList [floatValueStatListCount];

									floatValueStatListCount++;
								} else {
									currentElementOnScene.statInfoList [j].currentBoolState = currentPersistanceElementsOnSceneInfo.boolValueStatList [boolValueStatListCount];

									boolValueStatListCount++;
								}
							}

							currentElementOnScene.setStatsOnLoad ();

							return;
						}
					}
				}
			}
		}
	}
}