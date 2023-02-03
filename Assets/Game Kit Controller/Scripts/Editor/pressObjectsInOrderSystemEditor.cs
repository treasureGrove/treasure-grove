using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(pressObjectsInOrderSystem))]
public class pressObjectsInOrderSystemEditor : Editor
{
	SerializedProperty allPositionsPressedEvent;
	SerializedProperty useIncorrectOrderSound;
	SerializedProperty incorrectOrderSound;
	SerializedProperty pressObjectsWhileMousePressed;
	SerializedProperty pressObjectsLayer;
	SerializedProperty usingPressedObjectSystem;
	SerializedProperty allPositionsPressed;
	SerializedProperty correctPressedIndex;
	SerializedProperty positionInfoList;

	SerializedProperty mainCamera;
	SerializedProperty mainAudioSource;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		allPositionsPressedEvent = serializedObject.FindProperty ("allPositionsPressedEvent");
		useIncorrectOrderSound = serializedObject.FindProperty ("useIncorrectOrderSound");
		incorrectOrderSound = serializedObject.FindProperty ("incorrectOrderSound");
		pressObjectsWhileMousePressed = serializedObject.FindProperty ("pressObjectsWhileMousePressed");
		pressObjectsLayer = serializedObject.FindProperty ("pressObjectsLayer");
		usingPressedObjectSystem = serializedObject.FindProperty ("usingPressedObjectSystem");
		allPositionsPressed = serializedObject.FindProperty ("allPositionsPressed");
		correctPressedIndex = serializedObject.FindProperty ("correctPressedIndex");
		positionInfoList = serializedObject.FindProperty ("positionInfoList");

		mainCamera = serializedObject.FindProperty ("mainCamera");
		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (useIncorrectOrderSound);
		if (useIncorrectOrderSound.boolValue) {
			EditorGUILayout.PropertyField (incorrectOrderSound);
		}
		EditorGUILayout.PropertyField (pressObjectsWhileMousePressed);
		if (pressObjectsWhileMousePressed.boolValue) {
			EditorGUILayout.PropertyField (pressObjectsLayer);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current State", "window");
		EditorGUILayout.PropertyField (usingPressedObjectSystem);
		EditorGUILayout.PropertyField (allPositionsPressed);
		EditorGUILayout.PropertyField (correctPressedIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events State", "window");
		EditorGUILayout.PropertyField (allPositionsPressedEvent);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Positions List", "window");
		showPositionsList (positionInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (mainCamera);
		EditorGUILayout.PropertyField (mainAudioSource);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showPositionInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePositionEvent"));
		if (list.FindPropertyRelative ("usePositionEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionEvent"));
		}
		GUILayout.EndVertical ();
	}

	void showPositionsList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ();
			GUILayout.Label ("Number Of Positions: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Position")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();

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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + " - " + i), false);

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showPositionInfo (list.GetArrayElementAtIndex (i));
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

}
#endif