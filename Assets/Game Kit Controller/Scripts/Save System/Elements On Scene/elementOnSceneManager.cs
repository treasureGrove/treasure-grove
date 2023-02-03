using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elementOnSceneManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int elementsScene;

	[Space]
	[Header ("Elements On Scene Prefabs Settings")]
	[Space]

	public bool useElementsOnSceneData;

	public elementsOnSceneData mainElementsOnSceneData;

	[Space]
	[Header ("Elements On Scene Prefabs Settings")]
	[Space]

	public bool ignoreLoadStatsOnObjectIDList;
	public List<int> ignoreLoadStatsOnObjectIDListInfo = new List<int> ();

	[Space]

	public bool ignoreLoadStatsOnObjectPrefabIDList;
	public List<int> ignoreLoadStatsOnObjectPrefabIDListInfo = new List<int> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public int currentElementID = 0;

	public List<elementOnScene> elementOnSceneList = new List<elementOnScene> ();

	[Space]
	[Space]

	public List<temporalElementOnSceneInfo> temporalElementOnSceneInfoList = new List<temporalElementOnSceneInfo> ();


	//Possible cases of objects on scene to save its info:
	//-Pickups:
	//When placed on scene, the regular scene manager will get its info on the editor
	//When picked, they send a signal to be removed, by setting its info on a temporal list, as the original object is destroyed
	//the system should check if the object picked was an original element on the scene or it was instantiated as a new object from a previous save
	//so in that case, that element info is removed from the list, as it is not needed to be loaded or taken into account in next load/save
	//When spawned, they are stored as regular object info when the game is saved, increasing the ID value and adding them as new info elements on the
	//main list, so when the game loads, it searches their prefab ID to instantiate a copy of each one, adding them as new objects info on the list

	//-Vehicles:
	//When placed on scene, the regular scene manager will get its info on the editor
	//When moving to new scenes, they are disabled from the main list, so the original vehicle is disabled from scene in that case
	//in that case, the vehicle is checked on the new scene, instantiated and added on the main info list
	//When destroyed, they are disabled from the main list, so the original vehicle is disabled from scene in that case
	//When spawned, they are stored as regular object info when the game is saved, increasing the ID value and adding them as new info elements on the
	//main list, so when the game loads, it searches their prefab ID to instantiate a copy of each one, adding them as new objects info on the list

	//-AI:
	//When placed on scene, the regular scene manager will get its info on the editor
	//When destroyed, they are disabled from the main list, so the original AI is disabled from scene in that case
	//When spawned, they are stored as regular object info when the game is saved, increasing the ID value and adding them as new info elements on the
	//main list, so when the game loads, it searches their prefab ID to instantiate a copy of each one, adding them as new objects info on the list


	public void checkForInstantiatedElementsOnSceneOnSave ()
	{
		elementOnScene[] newElementOnSceneList = FindObjectsOfType<elementOnScene> ();

		foreach (elementOnScene currentElementOnScene in newElementOnSceneList) {
			if (!elementOnSceneList.Contains (currentElementOnScene)) {
				setNewInstantiatedElementOnSceneManagerIngameWithInfo (currentElementOnScene);
			}
		}
	}

	public void setNewInstantiatedElementOnSceneManagerIngame (elementOnScene newElementOnScene)
	{
		currentElementID++;

		newElementOnScene.setSaveElementEnabledState (true);

		newElementOnScene.setElementID (currentElementID);

		newElementOnScene.setElementScene (elementsScene);

		newElementOnScene.setUseElementPrefabIDState (true);

		elementOnSceneList.Add (newElementOnScene);
	}

	public void setNewInstantiatedElementOnSceneManagerIngameWithInfo (elementOnScene newElementOnScene)
	{
		currentElementID++;

		newElementOnScene.setSaveElementEnabledState (true);

		newElementOnScene.setElementActiveState (true);

		newElementOnScene.setElementID (currentElementID);

		newElementOnScene.setElementScene (elementsScene);

		if (useElementsOnSceneData) {
			int elementPrefabID = mainElementsOnSceneData.getElementScenePrefabIDByName (newElementOnScene.gameObject.name);

			if (elementPrefabID > -1) {
				newElementOnScene.setElementPrefabIDValue (elementPrefabID);

				newElementOnScene.setUseElementPrefabIDState (true);
			}
		}

		elementOnSceneList.Add (newElementOnScene);
	}

	public void setTemporalElementActiveState (int elementID, int elementScene, bool elementActiveState)
	{
		temporalElementOnSceneInfo newTemporalElementOnSceneInfo = new temporalElementOnSceneInfo ();

		newTemporalElementOnSceneInfo.elementID = elementID;

		newTemporalElementOnSceneInfo.elementScene = elementScene;

		newTemporalElementOnSceneInfo.elementActiveState = elementActiveState;

		temporalElementOnSceneInfoList.Add (newTemporalElementOnSceneInfo);
	}

	public void removeElementFromSceneList (elementOnScene newElementOnScene)
	{
		if (elementOnSceneList.Contains (newElementOnScene)) {
			elementOnSceneList.Remove (newElementOnScene);
		}
	}

	public elementOnScene getElementOnSceneInfo (int elementID, int elementScene)
	{
		//Get each elemento on scene configured,searching by ID
		if (elementOnSceneList.Count == 0) {
			getAllElementsOnSceneOnLevel ();
		}

		int elementOnSceneListCount = elementOnSceneList.Count;

		//Return the element on scene currently found by ID
		for (int i = 0; i < elementOnSceneListCount; i++) {
			elementOnScene currentElementOnScene = elementOnSceneList [i];
	
			if (currentElementOnScene != null && currentElementOnScene.isSaveElementEnabled ()) {
				if (currentElementOnScene.elementScene == elementScene && currentElementOnScene.elementID == elementID) {

					return currentElementOnScene;
				}
			}
		}

		return null;
	}

	public void addNewElementOnScene (elementOnScene newElementOnScene)
	{
		currentElementID++;

		newElementOnScene.setElementID (currentElementID);

		newElementOnScene.setElementScene (elementsScene);

		elementOnSceneList.Add (newElementOnScene);
	}

	public void getAllElementsOnSceneOnLevel ()
	{
		//Search all the station systems on the level, so they can be managed here
		elementOnSceneList.Clear ();

		elementOnScene[] newElementOnSceneList = FindObjectsOfType<elementOnScene> ();

		foreach (elementOnScene currentElementOnScene in newElementOnSceneList) {
			if (!elementOnSceneList.Contains (currentElementOnScene)) {
				elementOnSceneList.Add (currentElementOnScene);
			}
		}
	}

	public void setStatsSearchingByInfo (int currentElementScene, int currentElementID, elementOnScene currentElementOnScene)
	{
		saveElementsOnSceneInfo mainSaveElementsOnSceneInfo = FindObjectOfType<saveElementsOnSceneInfo> ();

		if (mainSaveElementsOnSceneInfo != null) {

			mainSaveElementsOnSceneInfo.setStatsSearchingByInfo (currentElementScene, currentElementID, currentElementOnScene);
		}
	}

	//EDITOR FUNCTIONS
	public void getAllElementsOnSceneOnLevelAndAssignInfoToAllElements ()
	{
		//Seach all station systems on the level and assign an ID to each one
		getAllElementsOnSceneOnLevel ();

		currentElementID = 0;

		int elementOnSceneListCount = elementOnSceneList.Count;

		for (int i = 0; i < elementOnSceneListCount; i++) {
			
			if (elementOnSceneList [i] != null) {
				elementOnSceneList [i].setElementActiveState (elementOnSceneList [i].gameObject.activeSelf);

				elementOnSceneList [i].setElementID (currentElementID);

				elementOnSceneList [i].setElementScene (elementsScene);

				elementOnSceneList [i].setObjectOriginallyOnSceneState (true);

				GKC_Utils.updateComponent (elementOnSceneList [i]);

				currentElementID++;
			}
		}

		updateComponent ();
	}

	public void setEnableStateOnAllElementsOnScene (bool state)
	{
		//Seach all station systems on the level and assign an ID to each one
		getAllElementsOnSceneOnLevel ();

		int elementOnSceneListCount = elementOnSceneList.Count;

		for (int i = 0; i < elementOnSceneListCount; i++) {

			if (elementOnSceneList [i] != null) {
				elementOnSceneList [i].setSaveElementEnabledState (state);

				GKC_Utils.updateComponent (elementOnSceneList [i]);
			}
		}

		updateComponent ();
	}

	public void setIDOnElementsOnScenePrefabs ()
	{
		if (useElementsOnSceneData) {
			int elementPrefabIDCount = 0;

			for (int i = 0; i < mainElementsOnSceneData.elementsOnSceneInfoList.Count; i++) {
				for (int j = 0; j < mainElementsOnSceneData.elementsOnSceneInfoList [i].elementOnSceneInfoList.Count; j++) {
					mainElementsOnSceneData.elementsOnSceneInfoList [i].elementOnSceneInfoList [j].elementPrefabID = elementPrefabIDCount;

					GameObject elementPrefab = mainElementsOnSceneData.elementsOnSceneInfoList [i].elementOnSceneInfoList [j].elementPrefab;

					if (elementPrefab != null) {
						elementOnScene currentElementOnScene = elementPrefab.GetComponentInChildren<elementOnScene> ();

						if (currentElementOnScene != null) {
							currentElementOnScene.setElementPrefabIDValue (elementPrefabIDCount);
						}
					}

					elementPrefabIDCount++;
				}
			}
		}

		updateComponent ();

		GKC_Utils.refreshAssetDatabase ();
	}

	public void setIDOnElementsOnScenePrefabsLocatedOnScene ()
	{
		if (!useElementsOnSceneData) {
			return;
		}

		elementOnScene[] newElementOnSceneList = FindObjectsOfType<elementOnScene> ();

		foreach (elementOnScene currentElementOnScene in newElementOnSceneList) {
			
			int elementPrefabID = mainElementsOnSceneData.getElementScenePrefabIDByName (currentElementOnScene.gameObject.name);

			if (elementPrefabID > -1) {
				currentElementOnScene.setElementPrefabIDValue (elementPrefabID);
			}
		}

		updateComponent ();
	}

	public void getAllElementsOnSceneOnLevelByEditor ()
	{
		//Search all station systems on the level and assign them here by the editor
		getAllElementsOnSceneOnLevel ();

		updateComponent ();
	}

	public void clearElementsOnSceneList ()
	{
		elementOnSceneList.Clear ();

		updateComponent ();
	}

	public void addSingleElementOnSceneToManager (elementOnScene newElementOnScene)
	{
		if (!elementOnSceneList.Contains (newElementOnScene)) {
			newElementOnScene.setElementActiveState (newElementOnScene.gameObject.activeSelf);

			newElementOnScene.setElementID (currentElementID);

			newElementOnScene.setElementScene (elementsScene);

			newElementOnScene.setObjectOriginallyOnSceneState (true);

			GKC_Utils.updateComponent (newElementOnScene);

			currentElementID++;

			elementOnSceneList.Add (newElementOnScene);

			updateComponent ();
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Main Elements On Scene Manager info", gameObject);
	}

	[System.Serializable]
	public class temporalElementOnSceneInfo
	{
		public int elementScene;
		public int elementID;

		public bool elementActiveState;

		public bool useElementPrefabID;

		public int elementPrefabID;
	}
}
