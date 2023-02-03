using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerSkillsUISystem))]
public class playerSkillsUISystemEditor : Editor
{
	SerializedProperty showLockedSkillName;
	SerializedProperty lockedSkillNameToShow;
	SerializedProperty showLockedSkillDescription;
	SerializedProperty lockedSkillDescriptionToShow;

	SerializedProperty useDoublePressToShowIncreaseSkillButtonEnabled;

	SerializedProperty skillsMenuOpened;
	SerializedProperty skillsMenuPanel;
	SerializedProperty currentSkillCategoryNameText;
	SerializedProperty currentSkillNameText;
	SerializedProperty currentSkillDescriptionText;
	SerializedProperty currentSkillLevelDescriptionText;
	SerializedProperty currentSkillPointsText;
	SerializedProperty requiredSkillPointsText;
	SerializedProperty confirmUseSkillPointsPanel;
	SerializedProperty playerExperienceManager;
	SerializedProperty playerSkillsManager;
	SerializedProperty eventOnSkillMenuOpened;
	SerializedProperty eventOnSkillMenuClosed;
	SerializedProperty eventOnSkillPointsUsed;
	SerializedProperty eventOnNotEnoughSkillPoints;
	SerializedProperty skillUICategoryInfoList;

	SerializedProperty categoryScrollBar;
	SerializedProperty categoryListContent;
	SerializedProperty categoryScrollRect;

	SerializedProperty showDebugPrint;

	playerSkillsUISystem manager;

	Color defBackgroundColor;
	bool showEventSettings;

	bool showElementSettings;

	string buttonMessage;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		showLockedSkillName = serializedObject.FindProperty ("showLockedSkillName");
		lockedSkillNameToShow = serializedObject.FindProperty ("lockedSkillNameToShow");
		showLockedSkillDescription = serializedObject.FindProperty ("showLockedSkillDescription");
		lockedSkillDescriptionToShow = serializedObject.FindProperty ("lockedSkillDescriptionToShow");

		useDoublePressToShowIncreaseSkillButtonEnabled = serializedObject.FindProperty ("useDoublePressToShowIncreaseSkillButtonEnabled");

		skillsMenuOpened = serializedObject.FindProperty ("skillsMenuOpened");
		skillsMenuPanel = serializedObject.FindProperty ("skillsMenuPanel");
		currentSkillCategoryNameText = serializedObject.FindProperty ("currentSkillCategoryNameText");
		currentSkillNameText = serializedObject.FindProperty ("currentSkillNameText");
		currentSkillDescriptionText = serializedObject.FindProperty ("currentSkillDescriptionText");
		currentSkillLevelDescriptionText = serializedObject.FindProperty ("currentSkillLevelDescriptionText");
		currentSkillPointsText = serializedObject.FindProperty ("currentSkillPointsText");
		requiredSkillPointsText = serializedObject.FindProperty ("requiredSkillPointsText");
		confirmUseSkillPointsPanel = serializedObject.FindProperty ("confirmUseSkillPointsPanel");
		playerExperienceManager = serializedObject.FindProperty ("playerExperienceManager");
		playerSkillsManager = serializedObject.FindProperty ("playerSkillsManager");
		eventOnSkillMenuOpened = serializedObject.FindProperty ("eventOnSkillMenuOpened");
		eventOnSkillMenuClosed = serializedObject.FindProperty ("eventOnSkillMenuClosed");
		eventOnSkillPointsUsed = serializedObject.FindProperty ("eventOnSkillPointsUsed");
		eventOnNotEnoughSkillPoints = serializedObject.FindProperty ("eventOnNotEnoughSkillPoints");
		skillUICategoryInfoList = serializedObject.FindProperty ("skillUICategoryInfoList");

		categoryScrollBar = serializedObject.FindProperty ("categoryScrollBar");
		categoryListContent = serializedObject.FindProperty ("categoryListContent");
		categoryScrollRect = serializedObject.FindProperty ("categoryScrollRect");

		showDebugPrint = serializedObject.FindProperty ("showDebugPrint");

		manager = (playerSkillsUISystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (showLockedSkillName);
		if (!showLockedSkillName.boolValue) {
			EditorGUILayout.PropertyField (lockedSkillNameToShow);
		}
		EditorGUILayout.PropertyField (showLockedSkillDescription);
		if (!showLockedSkillDescription.boolValue) {
			EditorGUILayout.PropertyField (lockedSkillDescriptionToShow);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useDoublePressToShowIncreaseSkillButtonEnabled);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Skill UI State Debug", "window");
		GUILayout.Label ("Menu Opened\t\t " + skillsMenuOpened.boolValue);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (showDebugPrint);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Elements", "window");
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

			EditorGUILayout.PropertyField (skillsMenuPanel);
			EditorGUILayout.PropertyField (currentSkillCategoryNameText);
			EditorGUILayout.PropertyField (currentSkillNameText);
			EditorGUILayout.PropertyField (currentSkillDescriptionText);
			EditorGUILayout.PropertyField (currentSkillLevelDescriptionText);
			EditorGUILayout.PropertyField (currentSkillPointsText);
			EditorGUILayout.PropertyField (requiredSkillPointsText);
			EditorGUILayout.PropertyField (confirmUseSkillPointsPanel);
			EditorGUILayout.PropertyField (playerExperienceManager);
			EditorGUILayout.PropertyField (playerSkillsManager);

			EditorGUILayout.PropertyField (categoryScrollBar);
			EditorGUILayout.PropertyField (categoryListContent);
			EditorGUILayout.PropertyField (categoryScrollRect);			 
		}
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

			EditorGUILayout.PropertyField (eventOnSkillMenuOpened);
			EditorGUILayout.PropertyField (eventOnSkillMenuClosed);
			EditorGUILayout.PropertyField (eventOnSkillPointsUsed);
			EditorGUILayout.PropertyField (eventOnNotEnoughSkillPoints);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Skill Category List", "window");
		showSkillUICategoryInfoList (skillUICategoryInfoList);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Assign Skills To Slots")) {
			manager.assignSkillsToSlots ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showSkillUICategoryInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Categories: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Category")) {
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showSkillUICategoryInfoListElement (list.GetArrayElementAtIndex (i), i);
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

	void showSkillUICategoryInfoListElement (SerializedProperty list, int listIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("categorySlot"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("categorySkillPanel"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Skill List", "window");
		showSkillUIInfoList (list.FindPropertyRelative ("skillUIInfoList"), listIndex);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showSkillUIInfoList (SerializedProperty list, int listIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Skills: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Skill")) {
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

			if (GUILayout.Button ("Assign Skills Names To Slots")) {
				manager.assignSkillsNamesToSlots (listIndex);
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Activate All Skills")) {
				if (Application.isPlaying) {
					manager.activateAllSkills (listIndex);
				}
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showSkillUIInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showSkillUIInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("mainSkillSlotPanel"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("categorySkillIndex"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("skillIndex")); 

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("unlockOtherSkillSlots"));

		if (list.FindPropertyRelative ("unlockOtherSkillSlots").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("unlockWhenCurrentSlotIsComplete"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMinSkillLevelToUnlock"));
			if (list.FindPropertyRelative ("useMinSkillLevelToUnlock").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("minSkillLevelToUnlock"));
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Skill List To Unlock", "window");
			showSimpleList (list.FindPropertyRelative ("skillNameListToUnlock"));
			GUILayout.EndVertical ();

		}
		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Amount: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.BeginHorizontal ();
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
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}
}
#endif