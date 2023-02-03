using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerSkillsSystem))]
public class playerSkillsSystemEditor : Editor
{
	SerializedProperty playerSkillsActive;
	SerializedProperty initializeSkillsValuesAtStartActive;
	SerializedProperty initializeSkillsOnlyWhenLoadingGame;
	SerializedProperty saveCurrentPlayerSkillsToSaveFile;
	SerializedProperty skillCategoryInfoList;

	SerializedProperty initializeValuesWhenNotLoadingFromTemplate;

	SerializedProperty mainSkillSettingsTemplate;

	playerSkillsSystem manager;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		playerSkillsActive = serializedObject.FindProperty ("playerSkillsActive");
		initializeSkillsValuesAtStartActive = serializedObject.FindProperty ("initializeSkillsValuesAtStartActive");
		initializeSkillsOnlyWhenLoadingGame = serializedObject.FindProperty ("initializeSkillsOnlyWhenLoadingGame");
		saveCurrentPlayerSkillsToSaveFile = serializedObject.FindProperty ("saveCurrentPlayerSkillsToSaveFile");
		skillCategoryInfoList = serializedObject.FindProperty ("skillCategoryInfoList");

		initializeValuesWhenNotLoadingFromTemplate = serializedObject.FindProperty ("initializeValuesWhenNotLoadingFromTemplate");

		mainSkillSettingsTemplate = serializedObject.FindProperty ("mainSkillSettingsTemplate");
	
		manager = (playerSkillsSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (playerSkillsActive);
		EditorGUILayout.PropertyField (initializeSkillsValuesAtStartActive);
		EditorGUILayout.PropertyField (initializeSkillsOnlyWhenLoadingGame);
		EditorGUILayout.PropertyField (saveCurrentPlayerSkillsToSaveFile);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Skill Settings Template", "window");
		EditorGUILayout.PropertyField (initializeValuesWhenNotLoadingFromTemplate);
		EditorGUILayout.PropertyField (mainSkillSettingsTemplate);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Save Settings To Template")) {
			manager.saveSettingsToTemplate ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Load Settings From Template")) {
			manager.loadSettingsFromTemplate (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Skills Template Complete")) {
			manager.setAllSkillsCompleteStateOnTemplate (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Skills Template Not Complete")) {
			manager.setAllSkillsCompleteStateOnTemplate (false);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Skill Categories List", "window");
		showSkillCategoryInfoList (skillCategoryInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showSkillCategoryInfoList (SerializedProperty list)
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All")) {
				manager.enableAllSkillsOnEditor ();
			}
			if (GUILayout.Button ("Disable All")) {
				manager.disableAllSkillsOnEditor ();
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
						showSkillCategoryInfoListElement (list.GetArrayElementAtIndex (i), i);
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

	void showSkillCategoryInfoListElement (SerializedProperty list, int categoryIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Skill Info List", "window");
		showSkillInfoList (list.FindPropertyRelative ("skillInfoList"), categoryIndex);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showSkillInfoList (SerializedProperty list, int categoryIndex)
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All")) {
				manager.enableSkillsOnEditor (categoryIndex);
			}
			if (GUILayout.Button ("Disable All")) {
				manager.disableSkillsOnEditor (categoryIndex);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Activate All")) {
				manager.activateSkillsOnEditor (categoryIndex);
			}
			if (GUILayout.Button ("Deactivate All")) {
				manager.deactivateSkillsOnEditor (categoryIndex);
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
						showSkillInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showSkillInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("skillEnabled"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("neededSkillPoints"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("skillUnlocked"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("skillActive"));

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("skillDescription"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("skillComplete"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Float Value Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFloatValue"));
		if (list.FindPropertyRelative ("useFloatValue").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentValue"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("valueToConfigure"));

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.Label ("Events Settings");

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToInitializeSkill"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToIncreaseSkill"));
		} 
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Bool Value Settings", "window");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBoolValue"));
		if (list.FindPropertyRelative ("useBoolValue").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentBoolState"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("boolStateToConfigure"));

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.Label ("Events Settings");

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useTwoEventsForActiveAndNotActive"));

			EditorGUILayout.Space ();

			if (list.FindPropertyRelative ("useTwoEventsForActiveAndNotActive").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToInitializeSkillActive"));

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToInitializeSkillNotActive"));
			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToInitializeBoolSkill"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActivateBoolSkill"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Skill Level Value Settings", "window");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSkillLevel"));
		if (list.FindPropertyRelative ("useSkillLevel").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentSkillLevel"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Skill Level Info List", "window");
			showSkillLevelInfoList (list.FindPropertyRelative ("skillLevelInfoList"));
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showSkillLevelInfoList (SerializedProperty list)
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showSkillLevelInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showSkillLevelInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("skillLevelDescription"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("neededSkillPoints"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFloatValue"));
		if (list.FindPropertyRelative ("useFloatValue").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentValue"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToInitializeSkill"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToIncreaseSkill"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBoolValue"));
		if (list.FindPropertyRelative ("useBoolValue").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentBoolState"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToInitializeBoolSkill"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActivateBoolSkill"));
		}

		GUILayout.EndVertical ();
	}
}
#endif