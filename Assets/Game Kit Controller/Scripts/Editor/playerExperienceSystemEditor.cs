using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerExperienceSystem))]
public class playerExperienceSystemEditor : Editor
{
	SerializedProperty experienceSystemEnabled;

	SerializedProperty XPExtraString;

	SerializedProperty currentLevelExperienceAmount;
	SerializedProperty currentLevelExperienceToNextLevel;
	SerializedProperty totalExperienceAmount;
	SerializedProperty totalExperiencePoints;
	SerializedProperty currentLevel;
	SerializedProperty totalExperienceAmountLimit;
	SerializedProperty experienceTextMovementSpeed;
	SerializedProperty experienceTextMovementDelay;
	SerializedProperty timeToShowLevelUpPanel;
	SerializedProperty experienceAmountTextOffset;
	SerializedProperty useMaxLevel;
	SerializedProperty maxLevel;
	SerializedProperty hideExperienceSliderAfterTime;
	SerializedProperty timeToHideExperienceSlider;
	SerializedProperty experienceMenuActive;
	SerializedProperty currentExperienceMultiplier;
	SerializedProperty experienceMultiplerEnabled;

	SerializedProperty extraTextOnLevelNumber;
	SerializedProperty extraTextOnNewLevelNumber;


	SerializedProperty totalExperiencePointsName;
	SerializedProperty totalExperienceAmountName;
	SerializedProperty currentLevelExperienceAmountName;
	SerializedProperty currentLevelExperienceToNextLevelName;
	SerializedProperty currentLevelName;
	SerializedProperty setLevelManually;
	SerializedProperty experienceMenuOpened;
	SerializedProperty playerSkillManager;
	SerializedProperty playerStatsManager;
	SerializedProperty mainCamera;
	SerializedProperty pauseManager;
	SerializedProperty levelUpAudioSource;
	SerializedProperty levelUpAudioClip;
	SerializedProperty currentLevelText;
	SerializedProperty levelUpPanel;
	SerializedProperty levelUpText;
	SerializedProperty experienceTextTargetPosition;
	SerializedProperty experienceSliderTransform;
	SerializedProperty experienceSlider;
	SerializedProperty experienceSliderPanel;
	SerializedProperty playerControllerManager;
	SerializedProperty mainPlayerCamera;
	SerializedProperty experienceMenuGameObject;
	SerializedProperty experienceObtaniedTextPrefab;
	SerializedProperty experienceObtainedTextParent;
	SerializedProperty experienceMultiplierTextPanel;
	SerializedProperty experienceMultiplerText;
	SerializedProperty experienceSliderPanelPositionInsideMenu;
	SerializedProperty experienceSliderPanelPositionOutsideMenu;
	SerializedProperty statsMenuPanel;
	SerializedProperty eventOnExperienceMultiplerEnabled;
	SerializedProperty eventOnExperienceMultiplerDisabled;
	SerializedProperty eventOnExperienceWithoutTextPosition;
	SerializedProperty eventOnExperienceMenuOpened;
	SerializedProperty eventOnExperienceMenuClosed;
	SerializedProperty useEventIfSystemDisabled;
	SerializedProperty eventIfSystemDisabled;
	SerializedProperty levelInfoList;
	SerializedProperty statUIInfoList;

	SerializedProperty callEventOnLevelReachedOnLoadEnabled;

	bool expanded;

	Color defBackgroundColor;
	bool showEventSettings;

	bool showElementSettings;

	string buttonMessage;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		experienceSystemEnabled = serializedObject.FindProperty ("experienceSystemEnabled");

		XPExtraString = serializedObject.FindProperty ("XPExtraString");

		currentLevelExperienceAmount = serializedObject.FindProperty ("currentLevelExperienceAmount");
		currentLevelExperienceToNextLevel = serializedObject.FindProperty ("currentLevelExperienceToNextLevel");
		totalExperienceAmount = serializedObject.FindProperty ("totalExperienceAmount");
		totalExperiencePoints = serializedObject.FindProperty ("totalExperiencePoints");
		currentLevel = serializedObject.FindProperty ("currentLevel");
		totalExperienceAmountLimit = serializedObject.FindProperty ("totalExperienceAmountLimit");
		experienceTextMovementSpeed = serializedObject.FindProperty ("experienceTextMovementSpeed");
		experienceTextMovementDelay = serializedObject.FindProperty ("experienceTextMovementDelay");
		timeToShowLevelUpPanel = serializedObject.FindProperty ("experienceAmountTextOffset");
		experienceAmountTextOffset = serializedObject.FindProperty ("experienceAmountTextOffset");
		useMaxLevel = serializedObject.FindProperty ("useMaxLevel");
		maxLevel = serializedObject.FindProperty ("maxLevel");
		hideExperienceSliderAfterTime = serializedObject.FindProperty ("hideExperienceSliderAfterTime");
		timeToHideExperienceSlider = serializedObject.FindProperty ("timeToHideExperienceSlider");
		experienceMenuActive = serializedObject.FindProperty ("experienceMenuActive");
		currentExperienceMultiplier = serializedObject.FindProperty ("currentExperienceMultiplier");
		experienceMultiplerEnabled = serializedObject.FindProperty ("experienceMultiplerEnabled");

		extraTextOnLevelNumber = serializedObject.FindProperty ("extraTextOnLevelNumber");
		extraTextOnNewLevelNumber = serializedObject.FindProperty ("extraTextOnNewLevelNumber");

		totalExperiencePointsName = serializedObject.FindProperty ("totalExperiencePointsName");
		totalExperienceAmountName = serializedObject.FindProperty ("totalExperienceAmountName");
		currentLevelExperienceAmountName = serializedObject.FindProperty ("currentLevelExperienceAmountName");
		currentLevelExperienceToNextLevelName = serializedObject.FindProperty ("currentLevelExperienceToNextLevelName");
		currentLevelName = serializedObject.FindProperty ("currentLevelName");
		setLevelManually = serializedObject.FindProperty ("setLevelManually");
		experienceMenuOpened = serializedObject.FindProperty ("experienceMenuOpened");
		playerSkillManager = serializedObject.FindProperty ("playerSkillManager");
		playerStatsManager = serializedObject.FindProperty ("playerStatsManager");
		mainCamera = serializedObject.FindProperty ("mainCamera");
		pauseManager = serializedObject.FindProperty ("pauseManager");
		levelUpAudioSource = serializedObject.FindProperty ("levelUpAudioSource");
		levelUpAudioClip = serializedObject.FindProperty ("levelUpAudioClip");
		currentLevelText = serializedObject.FindProperty ("currentLevelText");
		levelUpPanel = serializedObject.FindProperty ("levelUpPanel");
		levelUpText = serializedObject.FindProperty ("levelUpText");
		experienceTextTargetPosition = serializedObject.FindProperty ("experienceTextTargetPosition");
		experienceSliderTransform = serializedObject.FindProperty ("experienceSliderTransform");
		experienceSlider = serializedObject.FindProperty ("experienceSlider");
		experienceSliderPanel = serializedObject.FindProperty ("experienceSliderPanel");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		mainPlayerCamera = serializedObject.FindProperty ("mainPlayerCamera");
		experienceMenuGameObject = serializedObject.FindProperty ("experienceMenuGameObject");
		experienceObtaniedTextPrefab = serializedObject.FindProperty ("experienceObtaniedTextPrefab");
		experienceObtainedTextParent = serializedObject.FindProperty ("experienceObtainedTextParent");
		experienceMultiplierTextPanel = serializedObject.FindProperty ("experienceMultiplierTextPanel");
		experienceMultiplerText = serializedObject.FindProperty ("experienceMultiplerText");
		experienceSliderPanelPositionInsideMenu = serializedObject.FindProperty ("experienceSliderPanelPositionInsideMenu");
		experienceSliderPanelPositionOutsideMenu = serializedObject.FindProperty ("experienceSliderPanelPositionOutsideMenu");
		statsMenuPanel = serializedObject.FindProperty ("statsMenuPanel");
		eventOnExperienceMultiplerEnabled = serializedObject.FindProperty ("eventOnExperienceMultiplerEnabled");
		eventOnExperienceMultiplerDisabled = serializedObject.FindProperty ("eventOnExperienceMultiplerDisabled");
		eventOnExperienceWithoutTextPosition = serializedObject.FindProperty ("eventOnExperienceWithoutTextPosition");
		eventOnExperienceMenuOpened = serializedObject.FindProperty ("eventOnExperienceMenuOpened");
		eventOnExperienceMenuClosed = serializedObject.FindProperty ("eventOnExperienceMenuClosed");
		useEventIfSystemDisabled = serializedObject.FindProperty ("useEventIfSystemDisabled");
		eventIfSystemDisabled = serializedObject.FindProperty ("eventIfSystemDisabled");
		levelInfoList = serializedObject.FindProperty ("levelInfoList");
		statUIInfoList = serializedObject.FindProperty ("statUIInfoList");

		callEventOnLevelReachedOnLoadEnabled = serializedObject.FindProperty ("callEventOnLevelReachedOnLoadEnabled");
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (experienceSystemEnabled);
		EditorGUILayout.PropertyField (currentLevelExperienceAmount);
		EditorGUILayout.PropertyField (currentLevelExperienceToNextLevel);
		EditorGUILayout.PropertyField (totalExperienceAmount);
		EditorGUILayout.PropertyField (totalExperiencePoints);
		EditorGUILayout.PropertyField (currentLevel);

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (totalExperienceAmountLimit);
		EditorGUILayout.PropertyField (experienceTextMovementSpeed);
		EditorGUILayout.PropertyField (experienceTextMovementDelay);

		EditorGUILayout.PropertyField (timeToShowLevelUpPanel);
		EditorGUILayout.PropertyField (experienceAmountTextOffset);
		EditorGUILayout.PropertyField (useMaxLevel);
		EditorGUILayout.PropertyField (maxLevel);
		EditorGUILayout.PropertyField (hideExperienceSliderAfterTime);
		EditorGUILayout.PropertyField (timeToHideExperienceSlider);
		EditorGUILayout.PropertyField (experienceMenuActive);

		EditorGUILayout.PropertyField (callEventOnLevelReachedOnLoadEnabled);

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (currentExperienceMultiplier);
		EditorGUILayout.PropertyField (experienceMultiplerEnabled);

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (extraTextOnLevelNumber);
		EditorGUILayout.PropertyField (extraTextOnNewLevelNumber);

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (totalExperiencePointsName);
		EditorGUILayout.PropertyField (totalExperienceAmountName);
		EditorGUILayout.PropertyField (currentLevelExperienceAmountName);
		EditorGUILayout.PropertyField (currentLevelExperienceToNextLevelName);
		EditorGUILayout.PropertyField (currentLevelName);
		EditorGUILayout.PropertyField (setLevelManually);

		EditorGUILayout.PropertyField (XPExtraString);	

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Experience UI State", "window");
		GUILayout.Label ("Menu Opened\t\t " + experienceMenuOpened.boolValue);
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Level Info List", "window");
		showLevelInfoList (levelInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Stat UI Info List", "window");
		showStatUIInfoList (statUIInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showEventSettings) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Event Settings";
		} else {
			GUI.backgroundColor = defBackgroundColor;
			buttonMessage = "Show Event Settings";
		}
		if (GUILayout.Button (buttonMessage)) {
			showEventSettings = !showEventSettings;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		if (showEventSettings) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnExperienceMultiplerEnabled);
			EditorGUILayout.PropertyField (eventOnExperienceMultiplerDisabled);
			EditorGUILayout.PropertyField (eventOnExperienceWithoutTextPosition);
			EditorGUILayout.PropertyField (eventOnExperienceMenuOpened);
			EditorGUILayout.PropertyField (eventOnExperienceMenuClosed);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useEventIfSystemDisabled);
			if (useEventIfSystemDisabled.boolValue) {
				EditorGUILayout.PropertyField (eventIfSystemDisabled);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Component Elements", "window");
		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showElementSettings) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Element Settings";
		} else {
			GUI.backgroundColor = defBackgroundColor;
			buttonMessage = "Show Element Settings";
		}
		if (GUILayout.Button (buttonMessage)) {
			showElementSettings = !showElementSettings;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		if (showElementSettings) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (playerSkillManager);
			EditorGUILayout.PropertyField (playerStatsManager);
			EditorGUILayout.PropertyField (mainCamera);
			EditorGUILayout.PropertyField (pauseManager);

			EditorGUILayout.PropertyField (levelUpAudioSource);
			EditorGUILayout.PropertyField (levelUpAudioClip);
			EditorGUILayout.PropertyField (currentLevelText);
			EditorGUILayout.PropertyField (levelUpPanel);
			EditorGUILayout.PropertyField (levelUpText);
			EditorGUILayout.PropertyField (experienceTextTargetPosition);
			EditorGUILayout.PropertyField (experienceSliderTransform);
			EditorGUILayout.PropertyField (experienceSlider);
			EditorGUILayout.PropertyField (experienceSliderPanel);

			EditorGUILayout.PropertyField (playerControllerManager);
			EditorGUILayout.PropertyField (mainPlayerCamera);
			EditorGUILayout.PropertyField (experienceMenuGameObject);
			EditorGUILayout.PropertyField (experienceObtaniedTextPrefab);
			EditorGUILayout.PropertyField (experienceObtainedTextParent);

			EditorGUILayout.PropertyField (experienceMultiplierTextPanel);
			EditorGUILayout.PropertyField (experienceMultiplerText);

			EditorGUILayout.PropertyField (experienceSliderPanelPositionInsideMenu);
			EditorGUILayout.PropertyField (experienceSliderPanelPositionOutsideMenu);
			EditorGUILayout.PropertyField (statsMenuPanel);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showLevelInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Levels: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Level")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
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
						showLevelInfoListElement (list.GetArrayElementAtIndex (i));
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
		GUILayout.EndVertical ();
	}

	void showLevelInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		list.FindPropertyRelative ("Name").stringValue = "Level " + list.FindPropertyRelative ("levelNumber").intValue;

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("levelNumber"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("experienceRequiredToNextLevel"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("experiencePointsAmount"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("levelUnlocked"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("callEventOnLevelReachedOnLoad"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventsOnLevelReached"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Stats List", "window");
		showStatInfoList (list.FindPropertyRelative ("statInfoList"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showStatInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Stats: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Stat")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
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
						showStatInfoListElement (list.GetArrayElementAtIndex (i));
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
		GUILayout.EndVertical ();
	}

	void showStatInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("statIsAmount"));
		if (list.FindPropertyRelative ("statIsAmount").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("statExtraValue"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRandomRange")); 
			if (list.FindPropertyRelative ("useRandomRange").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("randomRange"));
			}
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newBoolState"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("unlockSkill"));
		if (list.FindPropertyRelative ("unlockSkill").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("skillNameToUnlock"));
		}

		GUILayout.EndVertical ();
	}

	void showStatUIInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Stats: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Stat")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
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
						showStatUIInfoListElement (list.GetArrayElementAtIndex (i));
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
		GUILayout.EndVertical ();
	}

	void showStatUIInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("statAmountText"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraTextAtStart"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraTextAtEnd"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("statIsAmount"));

		GUILayout.EndVertical ();
	}

}
#endif