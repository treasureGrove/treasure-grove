using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(objectiveEventSystem))]
public class objectiveEventSystemEditor : Editor
{
	SerializedProperty missionID;
	SerializedProperty missionScene;
	SerializedProperty objectiveInfoList;

	SerializedProperty useObjectiveCounterInsteadOfList;
	SerializedProperty objectiveCounterToComplete;

	SerializedProperty showObjectiveName;
	SerializedProperty generalObjectiveName;
	SerializedProperty showObjectiveDescription;
	SerializedProperty generalObjectiveDescription;
	SerializedProperty objectiveFullDescription;
	SerializedProperty objectiveLocaltion;
	SerializedProperty objectiveRewards;
	SerializedProperty hideObjectivePanelsAfterXTime;
	SerializedProperty objectivesFollowsOrder;
	SerializedProperty addObjectiveToPlayerLogSystem;
	SerializedProperty canCancelPreviousMissionToStartNewOne;
	SerializedProperty showMissionAcceptedPanel;
	SerializedProperty showMissionCompletePanel;
	SerializedProperty delayToDisableMissionPanel;
	SerializedProperty addMissionToLogIfMissionStartsAndNotBeingInProcess;
	SerializedProperty showAmountOfSubObjectivesComplete;
	SerializedProperty searchPlayerOnSceneIfNotAssigned;
	SerializedProperty disableObjectivePanelOnMissionComplete;
	SerializedProperty useTimeLimit;
	SerializedProperty timerSpeed;
	SerializedProperty minutesToComplete;
	SerializedProperty secondsToComplete;
	SerializedProperty secondSoundTimerLowerThan;
	SerializedProperty secondTimerSound;
	SerializedProperty timeToHideObjectivePanel;
	SerializedProperty eventWhenObjectiveComplete;
	SerializedProperty eventWhenObjectiveCompleteSendPlayer;
	SerializedProperty callEventWhenObjectiveNotComplete;
	SerializedProperty eventWhenObjectiveNotComplete;
	SerializedProperty useEventOnObjectiveStart;
	SerializedProperty eventOnObjectiveStart;
	SerializedProperty useEventObjectiveCompleteReward;
	SerializedProperty giveRewardOnObjectiveComplete;
	SerializedProperty eventObjectiveCompleteReward;
	SerializedProperty useMinPlayerLevel;
	SerializedProperty minPlayerLevel;
	SerializedProperty eventOnNotEnoughLevel;
	SerializedProperty enableAllMapObjectInformationAtOnce;
	SerializedProperty enableAllMapObjectInformationOnTime;
	SerializedProperty timeToEnableAllMapObjectInformation;
	SerializedProperty useExtraListMapObjectInformation;
	SerializedProperty extraListMapObjectInformation;
	SerializedProperty useSoundOnSubObjectiveComplete;
	SerializedProperty soundOnSubObjectiveComplete;
	SerializedProperty useSoundOnObjectiveNotComplete;
	SerializedProperty soundOnObjectiveNotComplete;
	SerializedProperty setCurrentPlayerManually;
	SerializedProperty currentPlayerToConfigure;
	SerializedProperty objectiveInProcess;
	SerializedProperty objectiveComplete;
	SerializedProperty missionAccepted;
	SerializedProperty rewardsObtained;
	SerializedProperty numberOfObjectives;
	SerializedProperty currentSubObjectiveIndex;

	SerializedProperty saveGameOnMissionComplete;

	SerializedProperty updateSubobjectivesCompleteOnLoadGame;

	SerializedProperty resumeMissionOnLoadGameIfNotComplete;

	SerializedProperty resetSubobjectivesIfCancellingMission;

	SerializedProperty useEventWhenLoadingGameAndObjectiveComplete;
	SerializedProperty eventWhenLoadingGameAndObjectiveComplete;

	objectiveEventSystem manager;

	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		missionID = serializedObject.FindProperty ("missionID");
		missionScene = serializedObject.FindProperty ("missionScene");
		objectiveInfoList = serializedObject.FindProperty ("objectiveInfoList");

		useObjectiveCounterInsteadOfList = serializedObject.FindProperty ("useObjectiveCounterInsteadOfList");
		objectiveCounterToComplete = serializedObject.FindProperty ("objectiveCounterToComplete");

		showObjectiveName = serializedObject.FindProperty ("showObjectiveName");
		generalObjectiveName = serializedObject.FindProperty ("generalObjectiveName");
		showObjectiveDescription = serializedObject.FindProperty ("showObjectiveDescription");
		generalObjectiveDescription = serializedObject.FindProperty ("generalObjectiveDescription");
		objectiveFullDescription = serializedObject.FindProperty ("objectiveFullDescription");
		objectiveLocaltion = serializedObject.FindProperty ("objectiveLocaltion");
		objectiveRewards = serializedObject.FindProperty ("objectiveRewards");
		hideObjectivePanelsAfterXTime = serializedObject.FindProperty ("hideObjectivePanelsAfterXTime");
		objectivesFollowsOrder = serializedObject.FindProperty ("objectivesFollowsOrder");
		addObjectiveToPlayerLogSystem = serializedObject.FindProperty ("addObjectiveToPlayerLogSystem");
		canCancelPreviousMissionToStartNewOne = serializedObject.FindProperty ("canCancelPreviousMissionToStartNewOne");
		showMissionAcceptedPanel = serializedObject.FindProperty ("showMissionAcceptedPanel");
		showMissionCompletePanel = serializedObject.FindProperty ("showMissionCompletePanel");
		delayToDisableMissionPanel = serializedObject.FindProperty ("delayToDisableMissionPanel");
		addMissionToLogIfMissionStartsAndNotBeingInProcess = serializedObject.FindProperty ("addMissionToLogIfMissionStartsAndNotBeingInProcess");
		showAmountOfSubObjectivesComplete = serializedObject.FindProperty ("showAmountOfSubObjectivesComplete");
		searchPlayerOnSceneIfNotAssigned = serializedObject.FindProperty ("searchPlayerOnSceneIfNotAssigned");
		disableObjectivePanelOnMissionComplete = serializedObject.FindProperty ("disableObjectivePanelOnMissionComplete");
		useTimeLimit = serializedObject.FindProperty ("useTimeLimit");
		timerSpeed = serializedObject.FindProperty ("timerSpeed");
		minutesToComplete = serializedObject.FindProperty ("minutesToComplete");
		secondsToComplete = serializedObject.FindProperty ("secondsToComplete");
		secondSoundTimerLowerThan = serializedObject.FindProperty ("secondSoundTimerLowerThan");
		secondTimerSound = serializedObject.FindProperty ("secondTimerSound");
		timeToHideObjectivePanel = serializedObject.FindProperty ("timeToHideObjectivePanel");
		eventWhenObjectiveComplete = serializedObject.FindProperty ("eventWhenObjectiveComplete");
		eventWhenObjectiveCompleteSendPlayer = serializedObject.FindProperty ("eventWhenObjectiveCompleteSendPlayer");
		callEventWhenObjectiveNotComplete = serializedObject.FindProperty ("callEventWhenObjectiveNotComplete");
		eventWhenObjectiveNotComplete = serializedObject.FindProperty ("eventWhenObjectiveNotComplete");
		useEventOnObjectiveStart = serializedObject.FindProperty ("useEventOnObjectiveStart");
		eventOnObjectiveStart = serializedObject.FindProperty ("eventOnObjectiveStart");
		useEventObjectiveCompleteReward = serializedObject.FindProperty ("useEventObjectiveCompleteReward");
		giveRewardOnObjectiveComplete = serializedObject.FindProperty ("giveRewardOnObjectiveComplete");
		eventObjectiveCompleteReward = serializedObject.FindProperty ("eventObjectiveCompleteReward");
		useMinPlayerLevel = serializedObject.FindProperty ("useMinPlayerLevel");
		minPlayerLevel = serializedObject.FindProperty ("minPlayerLevel");
		eventOnNotEnoughLevel = serializedObject.FindProperty ("eventOnNotEnoughLevel");
		enableAllMapObjectInformationAtOnce = serializedObject.FindProperty ("enableAllMapObjectInformationAtOnce");
		enableAllMapObjectInformationOnTime = serializedObject.FindProperty ("enableAllMapObjectInformationOnTime");
		timeToEnableAllMapObjectInformation = serializedObject.FindProperty ("timeToEnableAllMapObjectInformation");
		useExtraListMapObjectInformation = serializedObject.FindProperty ("useExtraListMapObjectInformation");
		extraListMapObjectInformation = serializedObject.FindProperty ("extraListMapObjectInformation");
		useSoundOnSubObjectiveComplete = serializedObject.FindProperty ("useSoundOnSubObjectiveComplete");
		soundOnSubObjectiveComplete = serializedObject.FindProperty ("soundOnSubObjectiveComplete");
		useSoundOnObjectiveNotComplete = serializedObject.FindProperty ("useSoundOnObjectiveNotComplete");
		soundOnObjectiveNotComplete = serializedObject.FindProperty ("soundOnObjectiveNotComplete");
		setCurrentPlayerManually = serializedObject.FindProperty ("setCurrentPlayerManually");
		currentPlayerToConfigure = serializedObject.FindProperty ("currentPlayerToConfigure");
		objectiveInProcess = serializedObject.FindProperty ("objectiveInProcess");
		objectiveComplete = serializedObject.FindProperty ("objectiveComplete");
		missionAccepted = serializedObject.FindProperty ("missionAccepted");
		rewardsObtained = serializedObject.FindProperty ("rewardsObtained");
		numberOfObjectives = serializedObject.FindProperty ("numberOfObjectives");
		currentSubObjectiveIndex = serializedObject.FindProperty ("currentSubObjectiveIndex");

		saveGameOnMissionComplete = serializedObject.FindProperty ("saveGameOnMissionComplete");

		updateSubobjectivesCompleteOnLoadGame = serializedObject.FindProperty ("updateSubobjectivesCompleteOnLoadGame");

		resumeMissionOnLoadGameIfNotComplete = serializedObject.FindProperty ("resumeMissionOnLoadGameIfNotComplete");

		resetSubobjectivesIfCancellingMission = serializedObject.FindProperty ("resetSubobjectivesIfCancellingMission");

		useEventWhenLoadingGameAndObjectiveComplete = serializedObject.FindProperty ("useEventWhenLoadingGameAndObjectiveComplete");
		eventWhenLoadingGameAndObjectiveComplete = serializedObject.FindProperty ("eventWhenLoadingGameAndObjectiveComplete");

		manager = (objectiveEventSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Info Settings", "window");
		EditorGUILayout.PropertyField (missionID);	
		EditorGUILayout.PropertyField (missionScene);	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objective Info List", "window");
		showObjectiveInfoList (objectiveInfoList);

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useObjectiveCounterInsteadOfList);	
		if (useObjectiveCounterInsteadOfList.boolValue) {
			EditorGUILayout.PropertyField (objectiveCounterToComplete);	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (showObjectiveName);
		EditorGUILayout.PropertyField (generalObjectiveName);
		EditorGUILayout.PropertyField (showObjectiveDescription);
		EditorGUILayout.PropertyField (generalObjectiveDescription);
		EditorGUILayout.PropertyField (objectiveFullDescription);

		EditorGUILayout.PropertyField (objectiveLocaltion);

		EditorGUILayout.PropertyField (objectiveRewards);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (hideObjectivePanelsAfterXTime);
		if (hideObjectivePanelsAfterXTime.boolValue) {
			EditorGUILayout.PropertyField (timeToHideObjectivePanel);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (objectivesFollowsOrder);
		EditorGUILayout.PropertyField (addObjectiveToPlayerLogSystem);
		EditorGUILayout.PropertyField (canCancelPreviousMissionToStartNewOne);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (showMissionAcceptedPanel);
		EditorGUILayout.PropertyField (showMissionCompletePanel);
		if (showMissionCompletePanel.boolValue) {
			EditorGUILayout.PropertyField (delayToDisableMissionPanel);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (addMissionToLogIfMissionStartsAndNotBeingInProcess);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (showAmountOfSubObjectivesComplete);
		EditorGUILayout.PropertyField (searchPlayerOnSceneIfNotAssigned);
		EditorGUILayout.PropertyField (disableObjectivePanelOnMissionComplete);

		EditorGUILayout.Space ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Save/Load Mission Settings", "window");
		EditorGUILayout.PropertyField (saveGameOnMissionComplete);

		EditorGUILayout.PropertyField (updateSubobjectivesCompleteOnLoadGame);

		EditorGUILayout.PropertyField (resumeMissionOnLoadGameIfNotComplete);
		EditorGUILayout.PropertyField (resetSubobjectivesIfCancellingMission);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Time Settings", "window");
		EditorGUILayout.PropertyField (useTimeLimit);
		if (useTimeLimit.boolValue) {
			EditorGUILayout.PropertyField (timerSpeed);
			EditorGUILayout.PropertyField (minutesToComplete);
			EditorGUILayout.PropertyField (secondsToComplete);
			EditorGUILayout.PropertyField (secondSoundTimerLowerThan);
			EditorGUILayout.PropertyField (secondTimerSound);
			EditorGUILayout.PropertyField (timeToHideObjectivePanel);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objective Complete Event Settings", "window");
		EditorGUILayout.PropertyField (eventWhenObjectiveComplete);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (eventWhenObjectiveCompleteSendPlayer);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objective Not Complete Event Settings", "window");
		EditorGUILayout.PropertyField (callEventWhenObjectiveNotComplete);
		if (callEventWhenObjectiveNotComplete.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventWhenObjectiveNotComplete);

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objective Start Event Settings", "window");
		EditorGUILayout.PropertyField (useEventOnObjectiveStart);
		if (useEventOnObjectiveStart.boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnObjectiveStart);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Reward Event Settings", "window");
		EditorGUILayout.PropertyField (useEventObjectiveCompleteReward);
		if (useEventObjectiveCompleteReward.boolValue) {
			EditorGUILayout.PropertyField (giveRewardOnObjectiveComplete);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventObjectiveCompleteReward);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Load Game & Objective Complete Event Settings", "window");
		EditorGUILayout.PropertyField (useEventWhenLoadingGameAndObjectiveComplete);
		if (useEventWhenLoadingGameAndObjectiveComplete.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventWhenLoadingGameAndObjectiveComplete);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Mission Level Settings", "window");
		EditorGUILayout.PropertyField (useMinPlayerLevel);
		if (useMinPlayerLevel.boolValue) {
			EditorGUILayout.PropertyField (minPlayerLevel);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnNotEnoughLevel);
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	
		GUILayout.BeginVertical ("Map Object Information Settings", "window");
		EditorGUILayout.PropertyField (enableAllMapObjectInformationAtOnce);
		EditorGUILayout.PropertyField (enableAllMapObjectInformationOnTime);
		if (enableAllMapObjectInformationOnTime.boolValue) {
			EditorGUILayout.PropertyField (timeToEnableAllMapObjectInformation);
		}
		EditorGUILayout.PropertyField (useExtraListMapObjectInformation);
		if (useExtraListMapObjectInformation.boolValue) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Objective Info List", "window");
			showExtraListMapObjectInformation (extraListMapObjectInformation);
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sounds Settings", "window");
		EditorGUILayout.PropertyField (useSoundOnSubObjectiveComplete, new GUIContent ("Use Sound On Objective Complete", null, ""));
		if (useSoundOnSubObjectiveComplete.boolValue) {
			EditorGUILayout.PropertyField (soundOnSubObjectiveComplete, new GUIContent ("Sound On Objective Complete", null, ""));
		}
		EditorGUILayout.PropertyField (useSoundOnObjectiveNotComplete);
		if (useSoundOnObjectiveNotComplete.boolValue) {
			EditorGUILayout.PropertyField (soundOnObjectiveNotComplete);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Settings", "window");
		EditorGUILayout.PropertyField (setCurrentPlayerManually);
		if (setCurrentPlayerManually.boolValue) {
			EditorGUILayout.PropertyField (currentPlayerToConfigure);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current State", "window");
		GUILayout.Label ("Mission In Process\t" + objectiveInProcess.boolValue);
		GUILayout.Label ("Mission Complete\t" + objectiveComplete.boolValue);
		GUILayout.Label ("Mission Accepted\t" + missionAccepted.boolValue);
		GUILayout.Label ("Rewards Obtained\t" + rewardsObtained.boolValue);
		EditorGUILayout.PropertyField (numberOfObjectives);
		EditorGUILayout.PropertyField (currentSubObjectiveIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Options", "window");
		if (GUILayout.Button ("Set Objective As Complete")) {
			if (Application.isPlaying) {
				manager.setObjectiveCompleteFromEditor ();
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}

	void showObjectiveInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Objective Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objectives: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Objective")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showObjectiveInfoListElement (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showObjectiveInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectiveName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectiveDescription"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectiveEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMapObjectInformation"));
		if (list.FindPropertyRelative ("useMapObjectInformation").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentMapObjectInformation"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("giveExtraTime"));
		if (list.FindPropertyRelative ("giveExtraTime").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraTime"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setObjectiveNameOnScreen"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setObjectiveDescriptionOnScreen"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sounds Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSoundOnSubObjectiveComplete"));
		if (list.FindPropertyRelative ("useSoundOnSubObjectiveComplete").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("soundOnSubObjectiveComplete"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnSubObjectiveComplete"));
		if (list.FindPropertyRelative ("useEventOnSubObjectiveComplete").boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnSubObjectiveComplete"));	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Options", "window");

		GUILayout.BeginVertical ("Sub Objective State (DEBUG)", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("subObjectiveComplete"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Sub Objective As Complete")) {
			if (Application.isPlaying) {
				manager.addSubObjectiveCompleteFromEditor (list.FindPropertyRelative ("Name").stringValue);
			}
		}
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}

	void showExtraListMapObjectInformation (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Extra List Map Object Information", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Map Objects: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Map Object")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif