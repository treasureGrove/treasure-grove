using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(selectObjectManager))]
public class selectObjectManagerEditor : Editor
{
	SerializedProperty objectToSelectInfoList;
	SerializedProperty explanation;

	SerializedProperty objectSearchResultList;

	SerializedProperty objectSearcherName;

	SerializedProperty searchObjectsActive;

	selectObjectManager manager;

	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		objectToSelectInfoList = serializedObject.FindProperty ("objectToSelectInfoList");
		explanation = serializedObject.FindProperty ("explanation");

		objectSearchResultList = serializedObject.FindProperty ("objectSearchResultList");

		objectSearcherName = serializedObject.FindProperty ("objectSearcherName");

		searchObjectsActive = serializedObject.FindProperty ("searchObjectsActive");

		manager = (selectObjectManager)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Information", "window");
		EditorGUILayout.PropertyField (explanation);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object To Select Info List", "window");
		showObjectToSelectInfoList (objectToSelectInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object Searcher", "window");
		EditorGUILayout.PropertyField (objectSearcherName, new GUIContent ("Name To Search"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Search Objects By Name")) {
			manager.showObjectsBySearchName ();

			if (objectSearchResultList.arraySize > 0) {
				objectSearchResultList.isExpanded = true;
			}
		}

		if (searchObjectsActive.boolValue) {
			if (GUILayout.Button ("Clear Results")) {
				manager.clearObjectsSearcResultList ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Object To Search Result List", "window");
			showObjectSearchResultList (objectSearchResultList);
			GUILayout.EndVertical ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showObjectToSelectInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Elements: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
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
						showObjectToSelectInfoListElement (list.GetArrayElementAtIndex (i));
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

				if (GUILayout.Button ("o", buttonStyle)) {
					if (!Application.isPlaying) {
						manager.selectObjectByIndex (i);

						return;
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

	void showObjectToSelectInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToSelect"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useComponentSelection"));

		if (list.FindPropertyRelative ("useComponentSelection").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("componentSelectionCommand"));
		}
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}


	void showObjectSearchResultList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		GUILayout.Label ("Number of Results: " + list.arraySize);

		EditorGUILayout.Space ();
		
		for (int i = 0; i < list.arraySize; i++) {
			expanded = false;
			GUILayout.BeginHorizontal ();
			GUILayout.BeginHorizontal ("box");

			EditorGUILayout.Space ();

			if (i < list.arraySize && i >= 0) {
				EditorGUILayout.BeginVertical ();

				GUILayout.Label (list.GetArrayElementAtIndex (i).stringValue, EditorStyles.boldLabel, GUILayout.MaxWidth (200));

				EditorGUILayout.Space ();

				GUILayout.EndVertical ();
			}
			GUILayout.EndHorizontal ();
			if (expanded) {
				GUILayout.BeginVertical ();
			} else {
				GUILayout.BeginHorizontal ();
			}
		
			if (GUILayout.Button ("o", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.selectObjectByName (list.GetArrayElementAtIndex (i).stringValue);

					return;
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
#endif