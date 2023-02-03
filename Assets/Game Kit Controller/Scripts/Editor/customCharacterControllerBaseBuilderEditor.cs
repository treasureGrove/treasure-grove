using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(customCharacterControllerBaseBuilder))]
public class customCharacterControllerBaseBuilderEditor : Editor
{
	SerializedProperty mainCustomCharacterControllerBase;
	SerializedProperty settingsInfoCategoryList;

	customCharacterControllerBaseBuilder manager;

	bool expanded;

	void OnEnable ()
	{
		mainCustomCharacterControllerBase = serializedObject.FindProperty ("mainCustomCharacterControllerBase");

		settingsInfoCategoryList = serializedObject.FindProperty ("settingsInfoCategoryList");

		manager = (customCharacterControllerBaseBuilder)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");

		EditorGUILayout.PropertyField (mainCustomCharacterControllerBase);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Settings List", "window");

		showSettingsInfoCategoryList (settingsInfoCategoryList);

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("\n APPLY CURRENT SETTINGS \n")) {
			manager.adjustSettingsFromEditor ();
		}

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showSettingsInfoCategoryList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Setting Info Category List")) {
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
				expanded = false;

				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						showSettingsInfoCategoryListElement (currentArrayElement);
						expanded = true;
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

	void showSettingsInfoCategoryListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.Space ();

		showSettingsInfoList (list.FindPropertyRelative ("settingsInfoList"));
		GUILayout.EndVertical ();
	}

	void showSettingsInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Settings Info List")) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Settings: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Settings")) {
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

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						showSettingsInfoListElement (currentArrayElement);
						expanded = true;
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

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();
	}

	void showSettingsInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("settingEnabled"));
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Bool Values Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBoolState"));
		if (list.FindPropertyRelative ("useBoolState").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("boolState"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSetBoolState"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Float Values Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFloatValue"));
		if (list.FindPropertyRelative ("useFloatValue").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("floatValue"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSetFloatValue"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("String Values Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStringValue"));
		if (list.FindPropertyRelative ("useStringValue").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("stringValue"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSetStringValue"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space (); 

		GUILayout.BeginVertical ("Vector3 Values Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useVector3Value"));
		if (list.FindPropertyRelative ("useVector3Value").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("vector3Value"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSetVector3Value"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space (); 

		GUILayout.BeginVertical ("Regular Values Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRegularValue"));
		if (list.FindPropertyRelative ("useRegularValue").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("regularValue"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToEnableActiveValue"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToDisableActiveValue"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space (); 

		GUILayout.BeginVertical ("Field Explanation Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFieldExplanation"));
		if (list.FindPropertyRelative ("useFieldExplanation").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("fieldExplanation"));
		}
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}
}
#endif