using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using GKC.Localization;

public class objectiveLogSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool objectiveMenuActive = true;
	public bool objectiveMenuOpened;

	public bool objectiveInProcess;

	public bool checkMinLevelOnMissions;

	public Color buttonUsable;
	public Color buttonNotUsable;

	public bool saveCurrentPlayerMissionsToSaveFile;

	public string mainMissionManagerName = "Mission Manager";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public List<objectiveSlotInfo> objectiveSlotInfoList = new List<objectiveSlotInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventIfSystemDisabled;
	public UnityEvent eventIfSystemDisabled;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject objectiveMenuGameObject;
	public GameObject objectiveSlotPrefab;

	public Text objectiveNameText;
	public Text objectiveDescriptionText;
	public Text objectiveFullDescriptionText;
	public Text objectiveRewardsText;

	public Image activeObjectiveButtonImage;
	public Image cancelObjectiveButtonImage;

	public Transform objectiveListContent;

	public GameObject playerControllerGameObject;
	public menuPause pauseManager;
	public playerController playerControllerManager;

	public objectiveManager mainObjectiveManager;

	objectiveSlotInfo currentObjectiveSlot;
	objectiveEventSystem currentObjectiveEventManager;

	public void initializeMissionValues ()
	{
		//Load the missions saved previously with those missions found by the player or activated in some way, setting their state or complete or not complete
		if (!objectiveMenuActive) {
			checkEventOnSystemDisabled ();

			return;
		}

		//Search for an objectives manager in the level, if no one is present, add one
		if (mainObjectiveManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainMissionManagerName, typeof(objectiveManager));

			mainObjectiveManager = FindObjectOfType<objectiveManager> ();
		}

		updateObjectiveTextContent ("", "", "", "");
	}

	public void checkEventOnSystemDisabled ()
	{
		if (useEventIfSystemDisabled) {
			eventIfSystemDisabled.Invoke ();
		}
	}

	public void setButtonsColor (bool activeObjectiveColor, bool cancelObjectiveColor)
	{
		if (activeObjectiveColor) {
			activeObjectiveButtonImage.color = buttonUsable;
		} else {
			activeObjectiveButtonImage.color = buttonNotUsable;
		}

		if (cancelObjectiveColor) {
			cancelObjectiveButtonImage.color = buttonUsable;
		} else {
			cancelObjectiveButtonImage.color = buttonNotUsable;
		}
	}

	public void activeObjective ()
	{
		if (currentObjectiveSlot != null && currentObjectiveSlot.objectiveEventManager != null) {
			if (!currentObjectiveSlot.objectiveEventManager.isObjectiveComplete () && !currentObjectiveSlot.objectiveEventManager.isObjectiveInProcess ()) {
				currentObjectiveSlot.objectiveEventManager.startObjective ();

				if (!currentObjectiveSlot.currentObjectiveIcon.activeSelf) {
					currentObjectiveSlot.currentObjectiveIcon.SetActive (true);
				}

				setButtonsColor (false, true);

				currentObjectiveEventManager = currentObjectiveSlot.objectiveEventManager;
			}
		}
	}

	public void cancelObjective ()
	{
		if (currentObjectiveSlot != null && currentObjectiveSlot.objectiveEventManager != null) {
			if (!currentObjectiveSlot.objectiveEventManager.isObjectiveComplete ()) {
				currentObjectiveSlot.objectiveEventManager.stopObjective ();

				if (currentObjectiveSlot.currentObjectiveIcon.activeSelf) {
					currentObjectiveSlot.currentObjectiveIcon.SetActive (false);
				}

				setButtonsColor (true, false);
			}
		}
	}

	public void objectiveComplete (objectiveEventSystem currentObjectiveEventSystem)
	{
		int objectiveSlotInfoListCount = objectiveSlotInfoList.Count;

		for (int i = 0; i < objectiveSlotInfoListCount; i++) {
			objectiveSlotInfo currentObjectiveSlotInfo = objectiveSlotInfoList [i];
				
			if (currentObjectiveSlotInfo.objectiveEventManager == currentObjectiveEventSystem) {
				updateObjectiveCompleteSlotInfo (i);

				objectiveInProcess = false;
			}
		}
	}

	public void updateObjectiveCompleteSlotInfo (int objectiveSlotIndex)
	{
		objectiveSlotInfo currentObjectiveSlotInfo = objectiveSlotInfoList [objectiveSlotIndex];

		bool enableObjectiveSlotResult = false;

		if (currentObjectiveSlotInfo.addObjectiveToPlayerLogSystem) {
			if (currentObjectiveSlotInfo.disableObjectivePanelOnMissionComplete) {
				enableObjectiveSlotResult = false;
			} else {
				if (currentObjectiveSlotInfo.currentObjectiveIcon.activeSelf) {
					currentObjectiveSlotInfo.currentObjectiveIcon.SetActive (false);
				}

				if (!currentObjectiveSlotInfo.objectiveCompletePanel.activeSelf) {
					currentObjectiveSlotInfo.objectiveCompletePanel.SetActive (true);
				}

				if (!currentObjectiveSlotInfo.objectiveCompleteText.activeSelf) {
					currentObjectiveSlotInfo.objectiveCompleteText.SetActive (true);
				}

				enableObjectiveSlotResult = true;
			}
		} else {
			enableObjectiveSlotResult = false;
		}

		if (currentObjectiveSlotInfo.objectiveSlotGameObject.activeSelf != enableObjectiveSlotResult) {
			currentObjectiveSlotInfo.objectiveSlotGameObject.SetActive (enableObjectiveSlotResult);
		}

		currentObjectiveSlotInfo.missionInProcess = false;

		currentObjectiveSlotInfo.missionComplete = true;
	}

	public void updateSubObjectiveCompleteListSlotInfo (int objectiveSlotIndex, List<bool> subObjectiveCompleteList)
	{
		objectiveSlotInfo currentObjectiveSlotInfo = objectiveSlotInfoList [objectiveSlotIndex];

		currentObjectiveSlotInfo.subObjectiveCompleteList = subObjectiveCompleteList;
	}

	public void activeObjective (objectiveEventSystem currentObjectiveEventSystem)
	{
		int objectiveSlotInfoListCount = objectiveSlotInfoList.Count;

		for (int i = 0; i < objectiveSlotInfoListCount; i++) {
			objectiveSlotInfo currentObjectiveSlotInfo = objectiveSlotInfoList [i];

			if (currentObjectiveSlotInfo.objectiveEventManager == currentObjectiveEventSystem) {

				if (!currentObjectiveSlotInfo.currentObjectiveIcon.activeSelf) {
					currentObjectiveSlotInfo.currentObjectiveIcon.SetActive (true);
				}

				currentObjectiveEventManager = currentObjectiveSlotInfo.objectiveEventManager;

				currentObjectiveSlotInfo.missionInProcess = true;

				objectiveInProcess = true;
			}
		}
	}

	public void cancelObjective (objectiveEventSystem currentObjectiveEventSystem)
	{
		int objectiveSlotInfoListCount = objectiveSlotInfoList.Count;

		for (int i = 0; i < objectiveSlotInfoListCount; i++) {
			objectiveSlotInfo currentObjectiveSlotInfo = objectiveSlotInfoList [i];

			if (currentObjectiveSlotInfo.objectiveEventManager == currentObjectiveEventSystem) {
				if (currentObjectiveSlotInfo.currentObjectiveIcon.activeSelf) {
					currentObjectiveSlotInfo.currentObjectiveIcon.SetActive (false);
				}

				objectiveInProcess = false;

				currentObjectiveSlotInfo.missionInProcess = false;
			}
		}
	}

	public void cancelPreviousObjective ()
	{
		if (currentObjectiveEventManager != null) {
			if (currentObjectiveEventManager.objectiveInProcess) {
				currentObjectiveEventManager.cancelPreviousObjective ();
			}
		}
	}

	public objectiveEventSystem getCurrentObjectiveEventSystem ()
	{
		return currentObjectiveEventManager;
	}

	public void showObjectiveInformation (GameObject objectiveSlot)
	{
		if (currentObjectiveSlot != null && currentObjectiveSlot.selectedObjectiveIcon.activeSelf) {
			currentObjectiveSlot.selectedObjectiveIcon.SetActive (false);
		}

		int objectiveSlotInfoListCount = objectiveSlotInfoList.Count;

		for (int i = 0; i < objectiveSlotInfoListCount; i++) {
			objectiveSlotInfo currentObjectiveSlotInfo = objectiveSlotInfoList [i];

			if (currentObjectiveSlotInfo.objectiveSlotGameObject == objectiveSlot) {
				currentObjectiveSlot = currentObjectiveSlotInfo;

				updateObjectiveTextContent (currentObjectiveSlot.objectiveName, currentObjectiveSlot.objectiveDescription,
					currentObjectiveSlot.objectiveFullDescription, currentObjectiveSlot.objectiveRewards);

				if (!currentObjectiveSlot.selectedObjectiveIcon.activeSelf) {
					currentObjectiveSlot.selectedObjectiveIcon.SetActive (true);
				}

				if (currentObjectiveSlot.objectiveEventManager != null) {
					if (!currentObjectiveSlot.objectiveEventManager.isObjectiveComplete ()) {
						if (currentObjectiveSlot.objectiveEventManager.objectiveInProcess) {
							setButtonsColor (false, true);
						} else {
							setButtonsColor (true, false);
						}
					} else {
						setButtonsColor (false, false);
					}
				} else {
					setButtonsColor (false, false);
				}

				return;
			}
		}

		setButtonsColor (false, false);
	}

	public void updateUIElements ()
	{
		if (objectiveInProcess) {
			currentObjectiveEventManager.updateUIElements ();
		}

		int objectiveSlotInfoListCount = objectiveSlotInfoList.Count;

		for (int i = 0; i < objectiveSlotInfoListCount; i++) {
			objectiveSlotInfo currentObjectiveSlotInfo = objectiveSlotInfoList [i];

			string objectiveName = currentObjectiveSlotInfo.objectiveName;

			string objectiveLocation = currentObjectiveSlotInfo.objectiveLocation;

			if (gameLanguageSelector.isCheckLanguageActive ()) {
				objectiveName = missionLocalizationManager.GetLocalizedValue (objectiveName);

				objectiveLocation = missionLocalizationManager.GetLocalizedValue (objectiveLocation);
			}

			if (currentObjectiveSlotInfo.objectiveMenuIconElementManager != null) {
				currentObjectiveSlotInfo.objectiveMenuIconElementManager.objectiveNameText.text = objectiveName;

				currentObjectiveSlotInfo.objectiveMenuIconElementManager.objectiveLocationText.text = objectiveLocation;
			}
		}
	}

	public void updateObjectiveTextContent (string objectiveName, string objectiveDescription, string objectiveFullDescription, string objectiveRewards)
	{
		if (gameLanguageSelector.isCheckLanguageActive ()) {
			objectiveName = missionLocalizationManager.GetLocalizedValue (objectiveName);

			objectiveDescription = missionLocalizationManager.GetLocalizedValue (objectiveDescription);

			objectiveFullDescription = missionLocalizationManager.GetLocalizedValue (objectiveFullDescription);

			objectiveRewards = missionLocalizationManager.GetLocalizedValue (objectiveRewards);
		}

		objectiveNameText.text = objectiveName;

		objectiveDescriptionText.text = objectiveDescription;

		objectiveFullDescriptionText.text = objectiveFullDescription;

		objectiveRewardsText.text = objectiveRewards;
	}

	public void addObjective (string objectiveName, string objectiveDescription, string objectiveFullDescription, string objectiveLocation, 
	                          string objectiveRewards, objectiveEventSystem currentObjectiveEventSystem, bool addObjectiveToPlayerLogSystem)
	{
		bool addNewObjectivePanel = true;

		objectiveSlotInfo newObjectiveSlotInfo = new objectiveSlotInfo ();

		if (currentObjectiveEventSystem != null) {
			int objectiveSlotInfoListCount = objectiveSlotInfoList.Count;

			for (int i = 0; i < objectiveSlotInfoListCount; i++) {
				if (objectiveSlotInfoList [i].objectiveEventManager != null) {
					if (objectiveSlotInfoList [i].objectiveEventManager == currentObjectiveEventSystem) {
						return;
					}
				} else {
					if (objectiveSlotInfoList [i].missionID == currentObjectiveEventSystem.missionID &&
					    objectiveSlotInfoList [i].missionScene == currentObjectiveEventSystem.missionScene) {

						if (objectiveSlotInfoList [i].missionComplete) {
							return;
						}

						newObjectiveSlotInfo = objectiveSlotInfoList [i];

						addNewObjectivePanel = false;
					}
				}
			}
		}

		if (addNewObjectivePanel) {
			GameObject newObjectiveSlotPrefab = (GameObject)Instantiate (objectiveSlotPrefab, objectiveSlotPrefab.transform.position, Quaternion.identity, objectiveListContent);

			if (!newObjectiveSlotPrefab.activeSelf) {
				newObjectiveSlotPrefab.SetActive (true);
			}

			newObjectiveSlotPrefab.transform.localScale = Vector3.one;
			newObjectiveSlotPrefab.transform.localPosition = Vector3.zero;

			objectiveMenuIconElement currentobjectiveMenuIconElement = newObjectiveSlotPrefab.GetComponent<objectiveMenuIconElement> ();

			if (gameLanguageSelector.isCheckLanguageActive ()) {
				objectiveName = missionLocalizationManager.GetLocalizedValue (objectiveName);

				objectiveLocation = missionLocalizationManager.GetLocalizedValue (objectiveLocation);
			}

			currentobjectiveMenuIconElement.objectiveNameText.text = objectiveName;
			currentobjectiveMenuIconElement.objectiveLocationText.text = objectiveLocation;

			newObjectiveSlotInfo.objectiveMenuIconElementManager = currentobjectiveMenuIconElement;

			newObjectiveSlotInfo.objectiveSlotGameObject = newObjectiveSlotPrefab;
			newObjectiveSlotInfo.objectiveName = objectiveName;
			newObjectiveSlotInfo.objectiveLocation = objectiveLocation;
			newObjectiveSlotInfo.objectiveRewards = objectiveRewards;
			newObjectiveSlotInfo.objectiveDescription = objectiveDescription;
			newObjectiveSlotInfo.objectiveFullDescription = objectiveFullDescription;

			newObjectiveSlotInfo.currentObjectiveIcon = currentobjectiveMenuIconElement.currentObjectiveIcon;
			newObjectiveSlotInfo.objectiveCompletePanel = currentobjectiveMenuIconElement.objectiveCompletePanel;
			newObjectiveSlotInfo.selectedObjectiveIcon = currentobjectiveMenuIconElement.selectedObjectiveIcon;
			newObjectiveSlotInfo.objectiveCompleteText = currentobjectiveMenuIconElement.objectiveCompleteText;

			newObjectiveSlotInfo.addObjectiveToPlayerLogSystem = addObjectiveToPlayerLogSystem;
		}

		if (currentObjectiveEventSystem != null) {				
			newObjectiveSlotInfo.missionID = currentObjectiveEventSystem.missionID;

			newObjectiveSlotInfo.disableObjectivePanelOnMissionComplete = currentObjectiveEventSystem.disableObjectivePanelOnMissionComplete;

			newObjectiveSlotInfo.missionScene = currentObjectiveEventSystem.missionScene;
			newObjectiveSlotInfo.objectiveEventManager = currentObjectiveEventSystem;
			newObjectiveSlotInfo.missionAccepted = true;

			currentObjectiveEventSystem.setMissionAcceptedState (true);

			if (showDebugPrint) {
				print (currentObjectiveEventSystem.objectiveInfoList.Count + " " + currentObjectiveEventSystem.generalObjectiveName);
			}
				
			for (int i = 0; i < currentObjectiveEventSystem.objectiveInfoList.Count; i++) {
				bool subObjectiveComplete = currentObjectiveEventSystem.objectiveInfoList [i].subObjectiveComplete;

				newObjectiveSlotInfo.subObjectiveCompleteList.Add (subObjectiveComplete);
			}
		}

		if (!addObjectiveToPlayerLogSystem) {
			if (newObjectiveSlotInfo.objectiveSlotGameObject.activeSelf) {
				newObjectiveSlotInfo.objectiveSlotGameObject.SetActive (false);
			}
		}
			
		if (addNewObjectivePanel) {
			objectiveSlotInfoList.Add (newObjectiveSlotInfo);
		}

		if (showDebugPrint) {
			print ("Activating mission " + objectiveName);
		}
	}

	public void openOrCloseObjectiveMenu (bool state)
	{
		if ((!playerControllerManager.isPlayerMenuActive () || objectiveMenuOpened) && (!playerControllerManager.isUsingDevice () || playerControllerManager.isPlayerDriving ()) && !pauseManager.isGamePaused ()) {
			objectiveMenuOpened = state;

			pauseManager.openOrClosePlayerMenu (objectiveMenuOpened, objectiveMenuGameObject.transform, true);

			if (objectiveMenuGameObject.activeSelf != objectiveMenuOpened) {
				objectiveMenuGameObject.SetActive (objectiveMenuOpened);
			}

			pauseManager.setIngameMenuOpenedState ("Objective Log System", objectiveMenuOpened, true);

			pauseManager.enableOrDisablePlayerMenu (objectiveMenuOpened, true, false);

			if (currentObjectiveSlot != null && currentObjectiveSlot.selectedObjectiveIcon.activeSelf) {
				currentObjectiveSlot.selectedObjectiveIcon.SetActive (false);
			}

			currentObjectiveSlot = null;

			setButtonsColor (false, false);

			updateObjectiveTextContent ("", "", "", "");
		}
	}

	public void checkOpenOrCloseObjectiveMenuFromTouch ()
	{
		if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
			return;
		}

		openOrCLoseObjectiveMenuFromTouch ();
	}

	public void openOrCLoseObjectiveMenuFromTouch ()
	{
		openOrCloseObjectiveMenu (!objectiveMenuOpened);
	}

	public void inputOpenOrCloseObjectiveMenu ()
	{
		if (objectiveMenuActive) {
			if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
				return;
			}

			openOrCloseObjectiveMenu (!objectiveMenuOpened);
		}
	}

	public bool isObjectiveInProcess ()
	{
		return objectiveInProcess;
	}

	public void setObtainedRewardState (int missionID, int missionScene, bool state)
	{
		int objectiveSlotInfoListCount = objectiveSlotInfoList.Count;

		for (int k = 0; k < objectiveSlotInfoListCount; k++) {
			objectiveSlotInfo currentObjectiveSlotInfo = objectiveSlotInfoList [k];

			if (currentObjectiveSlotInfo.missionID == missionID && currentObjectiveSlotInfo.missionScene == missionScene) {
				currentObjectiveSlotInfo.rewardObtained = state;
			}
		}
	}

	public void setSubObjectiveCompleteState (int missionID, int missionScene, int subObjectiveIndex, bool state)
	{
		int objectiveSlotInfoListCount = objectiveSlotInfoList.Count;

		for (int k = 0; k < objectiveSlotInfoListCount; k++) {
			objectiveSlotInfo currentObjectiveSlotInfo = objectiveSlotInfoList [k];

			if (currentObjectiveSlotInfo.missionID == missionID && currentObjectiveSlotInfo.missionScene == missionScene) {
				if (showDebugPrint) {
					print (objectiveSlotInfoList [k].subObjectiveCompleteList.Count + " " + subObjectiveIndex);
				}

				if (currentObjectiveSlotInfo.subObjectiveCompleteList.Count > subObjectiveIndex) {
					currentObjectiveSlotInfo.subObjectiveCompleteList [subObjectiveIndex] = state;
				}
			}
		}
	}

	public void setObjectiveMenuActiveState (bool state)
	{
		objectiveMenuActive = state;
	}

	[System.Serializable]
	public class objectiveSlotInfo
	{
		public GameObject objectiveSlotGameObject;
		public string objectiveName;
		public string objectiveDescription;
		public string objectiveFullDescription;
		public string objectiveLocation;
		public string objectiveRewards;

		public GameObject currentObjectiveIcon;
		public GameObject objectiveCompletePanel;
		public GameObject selectedObjectiveIcon;
		public GameObject objectiveCompleteText;
		public GameObject objectiveAcceptedText;

		public bool disableObjectivePanelOnMissionComplete;

		public bool addObjectiveToPlayerLogSystem;

		public bool slotSelectedByPlayer;

		public bool missionComplete;
		public bool missionInProcess;
		public bool rewardObtained;
		public bool missionAccepted;

		public int missionScene;
		public int missionID;

		public objectiveMenuIconElement objectiveMenuIconElementManager;

		public objectiveEventSystem objectiveEventManager;

		public List<bool> subObjectiveCompleteList = new List<bool> ();
	}
}
