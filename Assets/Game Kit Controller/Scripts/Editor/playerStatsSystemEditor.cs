using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerStatsSystem))]
public class playerStatsSystemEditor : Editor
{
	SerializedProperty playerStatsActive;
	SerializedProperty initializeStatsValuesAtStartActive;
	SerializedProperty initializeStatsOnlyWhenLoadingGame;
	SerializedProperty saveCurrentPlayerStatsToSaveFile;
	SerializedProperty statInfoList;

	SerializedProperty initializeValuesWhenNotLoadingFromTemplate;

	SerializedProperty mainStatsSettingsTemplate;

	bool expanded;

	playerStatsSystem manager;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		playerStatsActive = serializedObject.FindProperty ("playerStatsActive");
		initializeStatsValuesAtStartActive = serializedObject.FindProperty ("initializeStatsValuesAtStartActive");
		initializeStatsOnlyWhenLoadingGame = serializedObject.FindProperty ("initializeStatsOnlyWhenLoadingGame");
		saveCurrentPlayerStatsToSaveFile = serializedObject.FindProperty ("saveCurrentPlayerStatsToSaveFile");
		statInfoList = serializedObject.FindProperty ("statInfoList");

		initializeValuesWhenNotLoadingFromTemplate = serializedObject.FindProperty ("initializeValuesWhenNotLoadingFromTemplate");
		mainStatsSettingsTemplate = serializedObject.FindProperty ("mainStatsSettingsTemplate");

		manager = (playerStatsSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (playerStatsActive);
		EditorGUILayout.PropertyField (initializeStatsValuesAtStartActive);
		EditorGUILayout.PropertyField (initializeStatsOnlyWhenLoadingGame);
		EditorGUILayout.PropertyField (saveCurrentPlayerStatsToSaveFile);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Stats Settings Template", "window");
		EditorGUILayout.PropertyField (initializeValuesWhenNotLoadingFromTemplate);
		EditorGUILayout.PropertyField (mainStatsSettingsTemplate);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Save Settings To Template")) {
			manager.saveSettingsToTemplate ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Load Settings From Template")) {
			manager.loadSettingsFromTemplate (true);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Stat Info List", "window");
		showStatInfoList (statInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showStatInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("statIsAmount"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("initializeStatWithThisValue"));

		if (list.FindPropertyRelative ("statIsAmount").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentValue"));

			EditorGUILayout.Space ();

			GUILayout.Label ("Max Amount Stat Settings", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMaxAmount"));

			if (list.FindPropertyRelative ("useMaxAmount").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useOtherStatAsMaxAmount"));
				if (list.FindPropertyRelative ("useOtherStatAsMaxAmount").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("otherStatAsMaxAmountName"));
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxAmount"));
				}
			}
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentBoolState"));
		}

		EditorGUILayout.Space ();

		GUILayout.Label ("Custom Stat Settings", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomStatTypeForEvents"));
		if (list.FindPropertyRelative ("useCustomStatTypeForEvents").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("customStatType"));
		} else {

			EditorGUILayout.Space ();

			GUILayout.Label ("Event Settings", EditorStyles.boldLabel);

			EditorGUILayout.Space ();

			if (list.FindPropertyRelative ("statIsAmount").boolValue) {
				EditorGUILayout.Space ();

				if (list.FindPropertyRelative ("initializeStatWithThisValue").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToInitializeStat"));
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToInitializeStatOnComponent"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToIncreaseStat"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToUseStat"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToAddAmount"));


				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventToSendValueOnUpdateState"));
				if (list.FindPropertyRelative ("useEventToSendValueOnUpdateState").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSendValueOnUpdateState"));
				}
			} else {
				if (list.FindPropertyRelative ("initializeStatWithThisValue").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToInitializeBoolStat"));
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToInitializeBoolStatOnComponent"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActivateBoolStat"));
			}
		}
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
}
#endif