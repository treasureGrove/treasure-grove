using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectiveManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int missionScene;

	[Space]
	[Header ("Objective Station System List Settings")]
	[Space]

	public List<objectiveStationSystem> objectiveStationSystemList = new List<objectiveStationSystem> ();

	[Space]
	[Header ("Prefabs List")]
	[Space]

	public LayerMask layerToPlaceObjects;

	public GameObject missionStationPrefab;
	public GameObject missionSystemPrefab;
	public GameObject emptyMissionStationPrefab;
	public GameObject characterMissionSystemPrefab;

	[Space]
	[Header ("Other Elements")]
	[Space]

	[TextArea (5, 20)]public string explanation = "This is the main mission manager, and it contains a list of all the station systems in each level. These station systems " +
	                                              "can be used as a physical mission board or just an invisible element to configure different missions which are maybe activate by dialog or trigger." +
	                                              "\n\n" + "" + "Each station has a list of missions, which are the one which contains the missions to achieve, task, rewards and events to activate.";

	//Main Objective Manager, this component stores all the station systems in each level, managing their states. Each station system can be a physical station to get missions
	//Or used as a hidden mission manager to missions which are obtained by conversations with NPCs, with the dialog system or directly by triggers placed anywhere

	//So this component manages a list of station systems in the level, each one with different missions assigned and configured on them

	objectiveStationSystem currentObjectiveStationSystem;

	objectiveStationSystem.objectiveInfo currentObjectiveInfo;

	void Start ()
	{
		//Check the missions which are not complete yet after loading the current info saved previously
		int objectiveStationSystemListCount = objectiveStationSystemList.Count;

		for (int i = 0; i < objectiveStationSystemListCount; i++) {
			if (objectiveStationSystemList [i] != null) {
				objectiveStationSystemList [i].checkMissionsState ();
			} else {
				print ("WARNING: there is a list of missions stations configured in this main mission manager but the element is empty or missing, please make sure " +
				"the mission station system is configured properly in the list of this main mission manager");
			}
		}
	}

	public objectiveEventSystem getObjectiveEventSystem (int missionID, int missionSceneToSearch)
	{
		//Get each mission configured in each station system, searching by ID
		if (objectiveStationSystemList.Count == 0) {
			getAllStationSystemOnLevel ();
		}

		int objectiveStationSystemListCount = objectiveStationSystemList.Count;

		//Return the mission system currently found by ID
		for (int i = 0; i < objectiveStationSystemListCount; i++) {
			currentObjectiveStationSystem = objectiveStationSystemList [i];

			int objectiveInfoListCount = currentObjectiveStationSystem.objectiveInfoList.Count;

			for (int j = 0; j < objectiveInfoListCount; j++) {

				currentObjectiveInfo = currentObjectiveStationSystem.objectiveInfoList [j];

				if (currentObjectiveInfo != null) {
					if (currentObjectiveInfo.mainObjectiveEventSystem.missionID == missionID &&
					    currentObjectiveInfo.mainObjectiveEventSystem.missionScene == missionSceneToSearch) {

						return currentObjectiveInfo.mainObjectiveEventSystem;
					}
				}
			}
		}

		return null;
	}

	public void getAllStationSystemOnLevel ()
	{
		//Search all the station systems on the level, so they can be managed here
		objectiveStationSystemList.Clear ();

		objectiveStationSystem[] newObjectiveStationSystemList = FindObjectsOfType<objectiveStationSystem> ();

		foreach (objectiveStationSystem currentObjectiveStationSystem in newObjectiveStationSystemList) {
			if (!objectiveStationSystemList.Contains (currentObjectiveStationSystem)) {
				objectiveStationSystemList.Add (currentObjectiveStationSystem);
			}
		}
	}

	public void getAllStationSystemOnLevelAndAssignInfoToAllMissions ()
	{
		//Seach all station systems on the level and assign an ID to each one
		getAllStationSystemOnLevel ();

		int currentMissionID = 0;

		int objectiveStationSystemListCount = objectiveStationSystemList.Count;

		for (int i = 0; i < objectiveStationSystemListCount; i++) {
			for (int j = 0; j < objectiveStationSystemList [i].objectiveInfoList.Count; j++) {
				if (objectiveStationSystemList [i].objectiveInfoList [j].mainObjectiveEventSystem != null) {
					objectiveStationSystemList [i].objectiveInfoList [j].mainObjectiveEventSystem.assignIDToMission (currentMissionID);

					objectiveStationSystemList [i].objectiveInfoList [j].mainObjectiveEventSystem.assignMissionScene (missionScene);

					currentMissionID++;
				}
			}
		}

		updateComponent ();
	}

	public void getAllStationSystemOnLevelByEditor ()
	{
		//Search all station systems on the level and assign them here by the editor
		getAllStationSystemOnLevel ();

		updateComponent ();
	}

	public void clearStationSystemList ()
	{
		objectiveStationSystemList.Clear ();

		updateComponent ();
	}

	public void instantiateMissionStation ()
	{
		instantateObjectOnLevel (missionStationPrefab);
	}

	public void instantiateMissionSystem ()
	{
		instantateObjectOnLevel (missionSystemPrefab);
	}

	public void instantiateEmptyMissionSystem ()
	{
		instantateObjectOnLevel (emptyMissionStationPrefab);
	}

	public void instantiateCharacterMissionSystem ()
	{
		instantateObjectOnLevel (characterMissionSystemPrefab);
	}

	public void instantateObjectOnLevel (GameObject objectToInstantiate)
	{
		Vector3 positionToInstantiate = Vector3.zero;
	
		Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

		if (currentCameraEditor != null) {
			Vector3 editorCameraPosition = currentCameraEditor.transform.position;
			Vector3 editorCameraForward = currentCameraEditor.transform.forward;

			RaycastHit hit;

			if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceObjects)) {
				positionToInstantiate = hit.point + Vector3.up * 0.05f;
			}
		}

		if (objectToInstantiate != null) {

			GameObject newCameraTransformElement = (GameObject)Instantiate (objectToInstantiate, positionToInstantiate, Quaternion.identity);
			newCameraTransformElement.name = objectToInstantiate.name;

		} else {
			print ("WARNING: prefab gameObject is empty, make sure it is assigned correctly");
		}
	}

	public void addSubObjectiveCompleteRemotely	(string subObjectiveName, int missionScene, int missionID)
	{
		int objectiveStationSystemListCount = objectiveStationSystemList.Count;

		for (int i = 0; i < objectiveStationSystemListCount; i++) {
			currentObjectiveStationSystem = objectiveStationSystemList [i];

			int objectiveInfoListCount = currentObjectiveStationSystem.objectiveInfoList.Count;

			for (int j = 0; j < objectiveInfoListCount; j++) {

				currentObjectiveInfo = currentObjectiveStationSystem.objectiveInfoList [j];

				if (currentObjectiveInfo != null) {
					if (currentObjectiveInfo.mainObjectiveEventSystem.missionID == missionID) {

						if (missionScene == -1 || currentObjectiveInfo.mainObjectiveEventSystem.missionScene == missionScene) {
							currentObjectiveInfo.mainObjectiveEventSystem.addSubObjectiveComplete (subObjectiveName);

							return;
						}
					}
				}
			}
		}
	}

	public void increaseObjectiveCounterRemotely (int missionScene, int missionID)
	{
		int objectiveStationSystemListCount = objectiveStationSystemList.Count;

		for (int i = 0; i < objectiveStationSystemListCount; i++) {
			currentObjectiveStationSystem = objectiveStationSystemList [i];

			int objectiveInfoListCount = currentObjectiveStationSystem.objectiveInfoList.Count;

			for (int j = 0; j < objectiveInfoListCount; j++) {

				currentObjectiveInfo = currentObjectiveStationSystem.objectiveInfoList [j];

				if (currentObjectiveInfo != null) {
					if (currentObjectiveInfo.mainObjectiveEventSystem.missionID == missionID) {
						if (missionScene == -1 || currentObjectiveInfo.mainObjectiveEventSystem.missionScene == missionScene) {

							currentObjectiveInfo.mainObjectiveEventSystem.increaseObjectiveCounter ();

							return;
						}
					}
				}
			}
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Main Mission Manager info", gameObject);
	}
}
