using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GKC.Localization;

public class objectiveEventSystem : MonoBehaviour
{
	public int missionID;
	public int missionScene;

	public bool missionAccepted;

	public bool rewardsObtained;

	public bool objectiveInProcess;
	public bool objectiveComplete;

	public bool useMinPlayerLevel;
	public int minPlayerLevel;
	public UnityEvent eventOnNotEnoughLevel;

	public List<objectiveInfo> objectiveInfoList = new List<objectiveInfo> ();

	public bool useObjectiveCounterInsteadOfList;
	public int objectiveCounterToComplete;

	public bool showObjectiveName;
	public string generalObjectiveName;

	public bool showObjectiveDescription;
	[TextArea (1, 10)] public string generalObjectiveDescription;
	[TextArea (1, 10)] public string objectiveFullDescription;
	[TextArea (1, 10)] public string objectiveLocaltion;

	[TextArea (2, 20)] public string objectiveRewards;

	public bool hideObjectivePanelsAfterXTime;
	public float timeToHideObjectivePanel;

	public bool objectivesFollowsOrder;
	public int currentSubObjectiveIndex;

	public bool useTimeLimit;
	public float timerSpeed;
	[Range (0, 60)] public float minutesToComplete;
	[Range (0, 60)] public float secondsToComplete;
	public float secondSoundTimerLowerThan;
	public AudioClip secondTimerSound;

	public bool addObjectiveToPlayerLogSystem = true;

	public UnityEvent eventWhenObjectiveComplete;
	public bool callEventWhenObjectiveNotComplete;
	public UnityEvent eventWhenObjectiveNotComplete;

	public bool useEventOnObjectiveStart;
	public UnityEvent eventOnObjectiveStart;

	public bool useEventWhenLoadingGameAndObjectiveComplete;
	public UnityEvent eventWhenLoadingGameAndObjectiveComplete;

	public bool useExtraListMapObjectInformation;
	public List<mapObjectInformation> extraListMapObjectInformation = new List<mapObjectInformation> ();

	public bool useSoundOnSubObjectiveComplete;
	public AudioClip soundOnSubObjectiveComplete;

	public bool useSoundOnObjectiveNotComplete;
	public AudioClip soundOnObjectiveNotComplete;

	public bool enableAllMapObjectInformationAtOnce;

	public bool enableAllMapObjectInformationOnTime;
	public float timeToEnableAllMapObjectInformation;

	public int numberOfObjectives;

	public bool setCurrentPlayerManually;
	public GameObject currentPlayerToConfigure;

	public bool searchPlayerOnSceneIfNotAssigned = true;

	public bool canCancelPreviousMissionToStartNewOne;

	public eventParameters.eventToCallWithGameObject eventWhenObjectiveCompleteSendPlayer;

	public bool useEventObjectiveCompleteReward;
	public bool giveRewardOnObjectiveComplete;
	public eventParameters.eventToCallWithGameObject eventObjectiveCompleteReward;

	public bool showMissionAcceptedPanel;

	public bool showMissionCompletePanel;
	public float delayToDisableMissionPanel = 3;

	public bool addMissionToLogIfMissionStartsAndNotBeingInProcess;

	public bool showAmountOfSubObjectivesComplete;

	public bool disableObjectivePanelOnMissionComplete;

	public bool saveGameOnMissionComplete;

	public bool updateSubobjectivesCompleteOnLoadGame = true;

	public bool resumeMissionOnLoadGameIfNotComplete;

	public bool resetSubobjectivesIfCancellingMission = true;


	public bool showDebugPrint;


	float lastTimeObjectiveInProcess;

	float totalSecondsTimer;

	AudioSource mainAudioSource;

	Text currentObjectiveNameText;
	Text currentObjectiveDescriptionText;

	Text screenTimerText;
	showGameInfoHud gameInfoHudManager;
	GameObject currentPlayer;

	GameObject objectiveInfoPanel;

	objectiveLogSystem currentObjectiveLogSystem;
	playerComponentsManager mainPlayerComponentsManager;
	playerExperienceSystem mainPlayerExperienceSystem;

	bool cancellingMisionActive;

	bool playerAssignedProperly;

	void Start ()
	{
		initializeNumberOfObjectives ();

		if (setCurrentPlayerManually) {
			setCurrentPlayer (currentPlayerToConfigure);
		}
	}

	public void initializeNumberOfObjectives ()
	{
		numberOfObjectives = 0;

		if (useObjectiveCounterInsteadOfList) {
			numberOfObjectives = objectiveCounterToComplete;
		} else {
			for (int i = 0; i < objectiveInfoList.Count; i++) {
				if (objectiveInfoList [i].objectiveEnabled) {
					numberOfObjectives++;
				}
			}
		}
	}

	void Update ()
	{
		if (objectiveInProcess) {
			if (useTimeLimit) {
				totalSecondsTimer -= Time.deltaTime * timerSpeed;
				screenTimerText.text = convertSeconds ();

				if (secondTimerSound) {
					if (totalSecondsTimer - 1 <= secondSoundTimerLowerThan && totalSecondsTimer % 1 < 0.1f) {
						playAudioSourceShoot (secondTimerSound);
					}
				}

				if (totalSecondsTimer <= 0) {
					stopObjective ();
				}
			}

			if (enableAllMapObjectInformationOnTime) {
				if (lastTimeObjectiveInProcess > 0 && Time.time > lastTimeObjectiveInProcess + timeToEnableAllMapObjectInformation) {
					for (int i = 0; i < objectiveInfoList.Count; i++) {
						if (objectiveInfoList [i].useMapObjectInformation && objectiveInfoList [i].objectiveEnabled) {
							if (objectiveInfoList [i].currentMapObjectInformation != null) {
								objectiveInfoList [i].currentMapObjectInformation.createMapIconInfo ();
							}
						}

						lastTimeObjectiveInProcess = 0;
					}
				}
			}
		}
	}

	public string convertSeconds ()
	{
		int minutes = Mathf.FloorToInt (totalSecondsTimer / 60F);
		int seconds = Mathf.FloorToInt (totalSecondsTimer - minutes * 60);

		return string.Format ("{0:00}:{1:00}", minutes, seconds);
	}

	public void increaseObjectiveCounter ()
	{
		if (useObjectiveCounterInsteadOfList) {
			if (!checkIfMissionCanBeStarted ()) {
				return;
			}

			if (objectiveInfoList.Count >= 1) {
				
				objectiveInfo currentObjectiveInfo = objectiveInfoList [0];

				if (!currentObjectiveInfo.subObjectiveComplete) {
						
					if (currentObjectiveInfo.useSoundOnSubObjectiveComplete) {
						playAudioSourceShoot (currentObjectiveInfo.soundOnSubObjectiveComplete);
					}

					currentSubObjectiveIndex++;

					if (currentObjectiveInfo.useEventOnSubObjectiveComplete) {
						currentObjectiveInfo.eventOnSubObjectiveComplete.Invoke ();
					}
						
					if (currentSubObjectiveIndex == numberOfObjectives) {
						currentObjectiveInfo.subObjectiveComplete = true;

						if (currentObjectiveLogSystem != null) {
							currentObjectiveLogSystem.setSubObjectiveCompleteState (missionID, missionScene, 0, true);
						}

						setObjectiveComplete ();
					} else {
						if (useTimeLimit && currentObjectiveInfo.giveExtraTime) {
							totalSecondsTimer += currentObjectiveInfo.extraTime;
						}
					}

					updateUIElements ();

					return;
				}
			}
		}
	}

	bool checkIfMissionCanBeStarted ()
	{
		if (!objectiveInProcess) {
			if (objectiveComplete) {
				if (showDebugPrint) {
					print ("mission already complete, avoiding to set sub objective complete " + generalObjectiveName);
				}

				return false;
			}

			if (addMissionToLogIfMissionStartsAndNotBeingInProcess) {

				findPlayerOnScene ();

				if ((setCurrentPlayerManually && currentPlayerToConfigure != null) || playerAssignedProperly) {
					if (objectiveInfoList.Count > 1 || objectiveCounterToComplete > 1) {
						startObjective ();
					} else {
						if (canCancelPreviousMissionToStartNewOne) {
							if (currentObjectiveLogSystem != null) {
								currentObjectiveLogSystem.cancelPreviousObjective ();
							}
						}

						objectiveInProcess = true;

						if (useEventOnObjectiveStart) {
							eventOnObjectiveStart.Invoke ();
						}

						if (currentObjectiveLogSystem != null) {
							currentObjectiveLogSystem.addObjective (generalObjectiveName, generalObjectiveDescription, objectiveFullDescription, 
								objectiveLocaltion, objectiveRewards, this, addObjectiveToPlayerLogSystem);

							currentObjectiveLogSystem.activeObjective (this);
						}

						lastTimeObjectiveInProcess = Time.time;
					}
				} else {

					print ("WARNING: If you configure the mission " + generalObjectiveName + " with the parameter add Mission To Log If Mission Starts And Not Being In Process, make sure" +
					" to assign the player manually, so the info can be managed properly");

					return false;
				}
			} else {
				return false;
			}
		}

		return true;
	}

	public void addSubObjectiveComplete (string subObjectiveName)
	{
		if (!checkIfMissionCanBeStarted ()) {
			return;
		}

		for (int i = 0; i < objectiveInfoList.Count; i++) {
			objectiveInfo currentObjectiveInfo = objectiveInfoList [i];

			if (currentObjectiveInfo.Name.Equals (subObjectiveName) && !currentObjectiveInfo.subObjectiveComplete) {
				currentObjectiveInfo.subObjectiveComplete = true;

				if (currentObjectiveLogSystem != null) {
					currentObjectiveLogSystem.setSubObjectiveCompleteState (missionID, missionScene, i, true);
				}

				if (objectivesFollowsOrder) {
					if (i != currentSubObjectiveIndex) {
						
						stopObjective ();

						return;
					}
				}

				if (currentObjectiveInfo.useSoundOnSubObjectiveComplete) {
					playAudioSourceShoot (currentObjectiveInfo.soundOnSubObjectiveComplete);
				}

				currentSubObjectiveIndex++;

				if (currentObjectiveInfo.useEventOnSubObjectiveComplete) {
					currentObjectiveInfo.eventOnSubObjectiveComplete.Invoke ();
				}

				if (showDebugPrint) {
					print (currentSubObjectiveIndex + " " + numberOfObjectives);
				}

				if (currentSubObjectiveIndex == numberOfObjectives) {
					setObjectiveComplete ();
				} else {
					if (useTimeLimit && currentObjectiveInfo.giveExtraTime) {
						totalSecondsTimer += currentObjectiveInfo.extraTime;
					}

					if (i + 1 < objectiveInfoList.Count) {
						currentObjectiveInfo = objectiveInfoList [i + 1];

						if (!enableAllMapObjectInformationAtOnce) {
							if (currentObjectiveInfo.useMapObjectInformation && currentObjectiveInfo.objectiveEnabled) {
								if (currentObjectiveInfo.currentMapObjectInformation != null) {
									currentObjectiveInfo.currentMapObjectInformation.createMapIconInfo ();
								}
							}
						}

						if (currentObjectiveInfo.setObjectiveNameOnScreen) {
							if (currentObjectiveNameText != null) {

								string textContent = currentObjectiveInfo.objectiveName;

								if (gameLanguageSelector.isCheckLanguageActive ()) {
									if (textContent != null && textContent != "") {
										textContent = missionLocalizationManager.GetLocalizedValue (textContent);
									}
								}

								currentObjectiveNameText.text = textContent;

								currentObjectiveNameTextContent = textContent;
							}
						}

						if (currentObjectiveInfo.setObjectiveDescriptionOnScreen) {
							if (currentObjectiveDescriptionText != null) {
								string textContent = currentObjectiveInfo.objectiveDescription;

								if (gameLanguageSelector.isCheckLanguageActive ()) {
									if (textContent != null && textContent != "") {
										textContent = missionLocalizationManager.GetLocalizedValue (textContent);
									}
								}

								checkShowAmountOfSubObjectivesComplete (textContent);

								currentObjectiveDescriptionTextContent = textContent;
							}
						}
					}
				}

				updateUIElements ();

				return;
			}
		}
	}

	public void decreaseObjectiveCounter ()
	{
		if (!objectiveInProcess) {
			return;
		}

		if (objectiveInfoList.Count >= 1) {

			objectiveInfo currentObjectiveInfo = objectiveInfoList [0];
		
			if (currentObjectiveInfo.subObjectiveComplete) {
				currentObjectiveInfo.subObjectiveComplete = false;

				if (currentObjectiveLogSystem != null) {
					currentObjectiveLogSystem.setSubObjectiveCompleteState (missionID, missionScene, 0, false);
				}
					
				checkInCallEventWhenObjectiveNotComplete ();

				currentSubObjectiveIndex--;

				return;
			}
		}
	}

	public void removeSubObjectiveComplete (string subObjectiveName)
	{
		if (!objectiveInProcess) {
			return;
		}

		for (int i = 0; i < objectiveInfoList.Count; i++) {
			if (objectiveInfoList [i].Name.Equals (subObjectiveName) && objectiveInfoList [i].subObjectiveComplete) {
				objectiveInfoList [i].subObjectiveComplete = false;

				if (currentObjectiveLogSystem != null) {
					currentObjectiveLogSystem.setSubObjectiveCompleteState (missionID, missionScene, i, false);
				}

				if (objectivesFollowsOrder) {
					if (i != currentSubObjectiveIndex) {

						stopObjective ();
						return;
					}
				} else {
					checkInCallEventWhenObjectiveNotComplete ();
				}

				currentSubObjectiveIndex--;

				return;
			}
		}
	}

	public void findPlayerOnScene ()
	{
		if (searchPlayerOnSceneIfNotAssigned) {
			setCurrentPlayer (GKC_Utils.findMainPlayerOnScene ());
		}
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		if (showDebugPrint) {
			print (newPlayer.name);
		}

		currentPlayer = newPlayer;

		if (currentPlayer != null) {

			mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				currentObjectiveLogSystem = mainPlayerComponentsManager.getObjectiveLogSystem ();

				gameInfoHudManager = mainPlayerComponentsManager.getGameInfoHudManager ();

				mainPlayerExperienceSystem = mainPlayerComponentsManager.getPlayerExperienceSystem ();

				playerAssignedProperly = true;
			}
		}
	}

	public void startObjective ()
	{
		if (currentPlayer == null || !playerAssignedProperly) {

			findPlayerOnScene ();

			if (currentPlayer == null) {
				print ("WARNING: no player controller has been assigned to the mission." +
				" Make sure to use a trigger to activate the mission or assign the player manually");

				return;
			}
		}

		bool objectiveAlreadyInProcess = objectiveInProcess;

		if (objectiveInProcess && !canCancelPreviousMissionToStartNewOne) {
			return;
		}

		if (objectiveComplete) {
			return;
		}

		bool objectiveCanBeAdded = false;

		if (currentObjectiveLogSystem != null && currentObjectiveLogSystem.checkMinLevelOnMissions) {
			if (!useMinPlayerLevel) {
				objectiveCanBeAdded = true;
			} else {
				if (mainPlayerExperienceSystem) {
					if (mainPlayerExperienceSystem.getCurrentLevel () >= minPlayerLevel) {
						objectiveCanBeAdded = true;
					}
				}
			}
		} else {
			objectiveCanBeAdded = true;
		}

		if (!objectiveCanBeAdded) {
			eventOnNotEnoughLevel.Invoke ();
			return;
		}

		if (canCancelPreviousMissionToStartNewOne) {
			if (currentObjectiveLogSystem != null) {
				currentObjectiveLogSystem.cancelPreviousObjective ();
			}
		}

		if (hideObjectivePanelsAfterXTime) {
			disableObjectivePanelsAfterXTime ();
		}
			
		objectiveInProcess = true;

		if (useEventOnObjectiveStart) {
			eventOnObjectiveStart.Invoke ();
		}

		if (currentObjectiveLogSystem != null) {
			currentObjectiveLogSystem.addObjective (generalObjectiveName, generalObjectiveDescription, objectiveFullDescription, objectiveLocaltion,
				objectiveRewards, this, addObjectiveToPlayerLogSystem);

			currentObjectiveLogSystem.activeObjective (this);
		}

		if (showMissionAcceptedPanel && !objectiveAlreadyInProcess) {
			objectiveStationUISystem mainObjectiveStationUISystem = mainPlayerComponentsManager.getObjectiveStationUISystem ();

			if (mainObjectiveStationUISystem != null) {
				mainObjectiveStationUISystem.showMissionAcceptedMessage (generalObjectiveName);
			}
		}

		lastTimeObjectiveInProcess = Time.time;

		setMissionInfoOnHUd ();

		setMissionInfoOnMap ();
	}

	string currentObjectiveNameTextContent;
	string currentObjectiveDescriptionTextContent;

	public void updateUIElements ()
	{
		if (objectiveInProcess) {
			string objectiveContent = currentObjectiveNameTextContent;
			string descriptionContent = currentObjectiveDescriptionTextContent;

			if (gameLanguageSelector.isCheckLanguageActive ()) {
				if (objectiveContent != null && objectiveContent != "") {
					objectiveContent = missionLocalizationManager.GetLocalizedValue (objectiveContent);
				}

				if (descriptionContent != null && descriptionContent != "") {
					descriptionContent = missionLocalizationManager.GetLocalizedValue (descriptionContent);
				}
			}

			if (currentObjectiveNameText != null) {
				currentObjectiveNameText.text = objectiveContent;
			}

			checkShowAmountOfSubObjectivesComplete (descriptionContent);
		}
	}

	public void setMissionInfoOnHUd ()
	{
		if (gameInfoHudManager != null) {
			if (useTimeLimit) {
				totalSecondsTimer = secondsToComplete + minutesToComplete * 60;

				if (screenTimerText == null) {
					GameObject currenObjectFound = gameInfoHudManager.getHudElement ("Timer", "Timer Text"); 

					if (currenObjectFound != null) {
						screenTimerText = currenObjectFound.GetComponent<Text> ();
					}
				} 

				if (screenTimerText != null) {
					if (!screenTimerText.gameObject.activeSelf) {
						screenTimerText.gameObject.SetActive (true);
					}
				}
			}

			if (showObjectiveName) {
				if (currentObjectiveNameText == null) {
					GameObject currenObjectFound = gameInfoHudManager.getHudElement ("Objective Info", "Objective Name Text");

					if (currenObjectFound != null) {
						currentObjectiveNameText = currenObjectFound.GetComponent<Text> ();
					}
				} 

				if (currentObjectiveNameText != null) {
					if (!currentObjectiveNameText.gameObject.activeSelf) {
						currentObjectiveNameText.gameObject.SetActive (true);
					}

					string textContent = "";

					if (objectiveInfoList.Count > 0 && objectiveInfoList [0].setObjectiveNameOnScreen) {
						textContent = objectiveInfoList [0].objectiveName;
					} else {
						textContent = generalObjectiveName;
					}

					if (gameLanguageSelector.isCheckLanguageActive ()) {
						if (textContent != null && textContent != "") {
							textContent = missionLocalizationManager.GetLocalizedValue (textContent);
						}
					}

					currentObjectiveNameText.text = textContent;

					currentObjectiveNameTextContent = textContent;
				}
			}

			if (showObjectiveDescription) {
				if (currentObjectiveDescriptionText == null) {
					GameObject currenObjectFound = gameInfoHudManager.getHudElement ("Objective Info", "Objective Description Text");

					if (currenObjectFound != null) {
						currentObjectiveDescriptionText = currenObjectFound.GetComponent<Text> ();
					}
				} 

				if (currentObjectiveDescriptionText != null) {
					if (!currentObjectiveDescriptionText.gameObject.activeSelf) {
						currentObjectiveDescriptionText.gameObject.SetActive (true);
					}

					string textContent = "";

					if (objectiveInfoList.Count > 0 && objectiveInfoList [0].setObjectiveDescriptionOnScreen) {
						textContent = objectiveInfoList [0].objectiveDescription;
					} else {
						textContent = generalObjectiveDescription;
					}
						
					if (gameLanguageSelector.isCheckLanguageActive ()) {
						if (textContent != null && textContent != "") {
							textContent = missionLocalizationManager.GetLocalizedValue (textContent);
						}
					}
						
					currentObjectiveDescriptionTextContent = textContent;

					updateUIElements ();
				}
			}

			if (showObjectiveName || showObjectiveDescription) {
				if (objectiveInfoPanel == null) {
					objectiveInfoPanel = gameInfoHudManager.getHudElement ("Objective Info", "Objective Info Panel");
				} 

				if (objectiveInfoPanel != null) {
					if (!objectiveInfoPanel.activeSelf) {
						objectiveInfoPanel.SetActive (true);
					}
				}
			}
		}

		if (currentPlayer != null) {
			playerStatesManager currentPlayerStatesManager = currentPlayer.GetComponent<playerStatesManager> ();

			if (currentPlayerStatesManager != null) {
				mainAudioSource = currentPlayerStatesManager.getAudioSourceElement ("Timer Audio Source");
			}
		}
	}

	public void checkShowAmountOfSubObjectivesComplete (string newTextContent)
	{
		if (currentObjectiveDescriptionText != null) {
			if (showAmountOfSubObjectivesComplete) {
				currentObjectiveDescriptionText.text = newTextContent + " (" + currentSubObjectiveIndex + "/" + numberOfObjectives + ")";
			} else {
				currentObjectiveDescriptionText.text = newTextContent;
			}
		}
	}

	public void setMissionInfoOnMap ()
	{
		bool firstMapObjectInformationEnabled = false;

		for (int i = 0; i < objectiveInfoList.Count; i++) {
			if (enableAllMapObjectInformationAtOnce || !firstMapObjectInformationEnabled) {
				if (objectiveInfoList [i].useMapObjectInformation && objectiveInfoList [i].objectiveEnabled) {
					
					if (objectiveInfoList [i].currentMapObjectInformation != null) {
						objectiveInfoList [i].currentMapObjectInformation.createMapIconInfo ();
						firstMapObjectInformationEnabled = true;
					}
				}
			}
		}

		if (useExtraListMapObjectInformation) {
			for (int i = 0; i < extraListMapObjectInformation.Count; i++) {
				if (extraListMapObjectInformation [i] != null) {
					extraListMapObjectInformation [i].createMapIconInfo ();
				}
			}
		}
	}

	public void addObjectiveToPlayerLogMenu ()
	{
		if (currentObjectiveLogSystem != null) {
			currentObjectiveLogSystem.addObjective (generalObjectiveName, generalObjectiveDescription, objectiveFullDescription, 
				objectiveLocaltion, objectiveRewards, this, addObjectiveToPlayerLogSystem);
		}
	}

	public void resetAllSubObjectives ()
	{
		for (int i = 0; i < objectiveInfoList.Count; i++) {
			objectiveInfoList [i].subObjectiveComplete = false;

			if (currentObjectiveLogSystem != null) {
				currentObjectiveLogSystem.setSubObjectiveCompleteState (missionID, missionScene, i, false);
			}
		}
	}

	public void setSubObjectiveCompleteListState (List<bool> subObjectiveCompleteList)
	{
		currentSubObjectiveIndex = 0;

		for (int i = 0; i < objectiveInfoList.Count; i++) {
			objectiveInfoList [i].subObjectiveComplete = subObjectiveCompleteList [i];

			if (subObjectiveCompleteList [i]) {
				currentSubObjectiveIndex++;
			}
		}
	}

	public void cancelPreviousObjective ()
	{
		if (showDebugPrint) {
			print ("-------------------------------------------------------cancelling mission " + generalObjectiveName);
		}

		cancellingMisionActive = true;

		stopObjective ();

		cancellingMisionActive = false;
	}

	public void stopObjective ()
	{
		objectiveInProcess = false;

		if (useTimeLimit) {
			if (screenTimerText.gameObject.activeSelf) {
				screenTimerText.gameObject.SetActive (false);
			}
		}

		if (useSoundOnObjectiveNotComplete && !cancellingMisionActive) {
			playAudioSourceShoot (soundOnObjectiveNotComplete);
		}

		if (resetSubobjectivesIfCancellingMission) {
			resetAllSubObjectives ();

			currentSubObjectiveIndex = 0;
		}

		for (int i = 0; i < objectiveInfoList.Count; i++) {
			if (objectiveInfoList [i].currentMapObjectInformation != null) {
				objectiveInfoList [i].currentMapObjectInformation.removeMapObject ();
			}
		}

		disableHUDElements ();

		if (currentObjectiveLogSystem != null) {
			currentObjectiveLogSystem.cancelObjective (this);
		}

		if (useExtraListMapObjectInformation) {
			for (int i = 0; i < extraListMapObjectInformation.Count; i++) {
				if (extraListMapObjectInformation [i] != null) {
					extraListMapObjectInformation [i].removeMapObject ();
				}
			}
		}

		checkInCallEventWhenObjectiveNotComplete ();
	}

	public void setObjectiveComplete ()
	{
		objectiveInProcess = false;

		objectiveComplete = true;

		disableHUDElements ();

		eventWhenObjectiveComplete.Invoke ();

		if (giveRewardOnObjectiveComplete) {
			giveRewardToPlayer ();
		}

		eventWhenObjectiveCompleteSendPlayer.Invoke (currentPlayer);

		if (useSoundOnSubObjectiveComplete) {
			playAudioSourceShoot (soundOnSubObjectiveComplete);
		}

		if (currentObjectiveLogSystem != null) {
			currentObjectiveLogSystem.objectiveComplete (this);
		}

		if (useExtraListMapObjectInformation) {
			for (int i = 0; i < extraListMapObjectInformation.Count; i++) {
				if (extraListMapObjectInformation [i] != null) {
					extraListMapObjectInformation [i].removeMapObject ();
				}
			}
		}

		if (showMissionCompletePanel) {
			objectiveStationUISystem mainObjectiveStationUISystem = mainPlayerComponentsManager.getObjectiveStationUISystem ();

			if (mainObjectiveStationUISystem != null) {
				mainObjectiveStationUISystem.showMissionCompleteMessageTemporarily (delayToDisableMissionPanel);
			}
		}

		if (saveGameOnMissionComplete) {
			saveGameSystem currentSaveGameSystem = mainPlayerComponentsManager.getSaveGameSystem ();

			if (currentSaveGameSystem != null) {
				currentSaveGameSystem.saveGameWhenReturningHomeMenu ();
			}
		}
	}

	public void setObjectiveAsCompleteOnLoad (bool rewardsObtainedValue)
	{
		objectiveInProcess = false;

		objectiveComplete = true;

		disableHUDElements ();

		if (currentObjectiveLogSystem != null) {
			currentObjectiveLogSystem.objectiveComplete (this);
		}

		if (useExtraListMapObjectInformation) {
			for (int i = 0; i < extraListMapObjectInformation.Count; i++) {
				if (extraListMapObjectInformation [i] != null) {
					extraListMapObjectInformation [i].removeMapObject ();
				}
			}
		}

		rewardsObtained = rewardsObtainedValue;

		if (rewardsObtained) {
			if (currentObjectiveLogSystem != null) {
				currentObjectiveLogSystem.setObtainedRewardState (missionID, missionScene, true);
			}
		}

		if (useEventWhenLoadingGameAndObjectiveComplete) {
			eventWhenLoadingGameAndObjectiveComplete.Invoke ();
		}
	}

	public void giveRewardToPlayer ()
	{
		if (rewardsObtained) {
			return;
		}

		if (useEventObjectiveCompleteReward) {
			eventObjectiveCompleteReward.Invoke (currentPlayer);
		}

		rewardsObtained = true;

		if (currentObjectiveLogSystem != null) {
			currentObjectiveLogSystem.setObtainedRewardState (missionID, missionScene, true);
		}
	}

	public bool isRewardsObtained ()
	{
		return rewardsObtained;
	}

	public void setRewardsObtanedState (bool state)
	{
		rewardsObtained = state;
	}

	public bool isMissionAccepted ()
	{
		return missionAccepted;
	}

	public bool isObjectiveInProcess ()
	{
		return objectiveInProcess;
	}

	public void setMissionAcceptedState (bool state)
	{
		missionAccepted = state;
	}

	public void checkInCallEventWhenObjectiveNotComplete ()
	{
		if (callEventWhenObjectiveNotComplete) {
			eventWhenObjectiveNotComplete.Invoke ();
		}
	}

	public void disableHUDElements ()
	{
		if (useTimeLimit) {
			if (screenTimerText != null && screenTimerText.gameObject.activeSelf) {
				screenTimerText.gameObject.SetActive (false);
			}
		}

		if (currentObjectiveNameText != null && currentObjectiveNameText.gameObject.activeSelf) {
			currentObjectiveNameText.gameObject.SetActive (false);
		}

		if (currentObjectiveDescriptionText != null && currentObjectiveDescriptionText.gameObject.activeSelf) {
			currentObjectiveDescriptionText.gameObject.SetActive (false);
		}

		if (objectiveInfoPanel != null && objectiveInfoPanel.activeSelf) {
			objectiveInfoPanel.SetActive (false);
		}
	}

	public void playAudioSourceShoot (AudioClip clip)
	{
		if (mainAudioSource != null) {
			mainAudioSource.PlayOneShot (clip);
		}
	}

	Coroutine disableObjectivePanelCoroutine;

	public void disableObjectivePanelsAfterXTime ()
	{
		stopDisableObjectivePanelCoroutine ();

		disableObjectivePanelCoroutine = StartCoroutine (disableObjectivePanelsAfterXTimeCoroutine ());
	}

	public void stopDisableObjectivePanelCoroutine ()
	{
		if (disableObjectivePanelCoroutine != null) {
			StopCoroutine (disableObjectivePanelCoroutine);
		}
	}

	IEnumerator disableObjectivePanelsAfterXTimeCoroutine ()
	{
		yield return new WaitForSeconds (timeToHideObjectivePanel);

		if (currentObjectiveNameText != null) {
			if (currentObjectiveNameText.gameObject.activeSelf) {
				currentObjectiveNameText.gameObject.SetActive (false);
			}
		}
	
		if (showObjectiveDescription) {
			if (currentObjectiveDescriptionText != null) {
				if (currentObjectiveDescriptionText.gameObject.activeSelf) {
					currentObjectiveDescriptionText.gameObject.SetActive (false);
				}
			}
		}

		if (showObjectiveName || showObjectiveDescription) {
			if (objectiveInfoPanel != null) {
				if (objectiveInfoPanel.activeSelf) {
					objectiveInfoPanel.SetActive (false);
				}
			}
		}
	}

	public void addSubObjectiveCompleteFromEditor (string subObjectiveName)
	{
		addSubObjectiveComplete (subObjectiveName);
	}

	public void setObjectiveCompleteFromEditor ()
	{
		setObjectiveComplete ();
	}

	public bool isObjectiveComplete ()
	{
		return objectiveComplete;
	}

	public void assignIDToMission (int newID)
	{
		missionID = newID;

		updateComponent ();
	}

	public void assignMissionScene (int newMissionScene)
	{
		missionScene = newMissionScene;

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class objectiveInfo
	{
		public string Name;
		public string objectiveName;
		[TextArea (1, 10)] public string objectiveDescription;

		public bool objectiveEnabled = true;

		public bool useMapObjectInformation;
		public mapObjectInformation currentMapObjectInformation;

		public bool giveExtraTime;
		public float extraTime;

		public bool setObjectiveNameOnScreen;
		public bool setObjectiveDescriptionOnScreen;

		public bool subObjectiveComplete;

		public bool useSoundOnSubObjectiveComplete;
		public AudioClip soundOnSubObjectiveComplete;

		public bool useEventOnSubObjectiveComplete;
		public UnityEvent eventOnSubObjectiveComplete;
	}
}
