using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(eventInfoSystem))]
public class eventInfoSystemEditor : Editor
{
	SerializedProperty eventInfoEnabled;
	SerializedProperty useDelayEnabled;
	SerializedProperty useAccumulativeDelay;
	SerializedProperty mainRemoteEventSystem;
	SerializedProperty eventInProcess;
	SerializedProperty eventInfoList;

	eventInfoSystem manager;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		eventInfoEnabled = serializedObject.FindProperty ("eventInfoEnabled");
		useDelayEnabled = serializedObject.FindProperty ("useDelayEnabled");
		useAccumulativeDelay = serializedObject.FindProperty ("useAccumulativeDelay");
		mainRemoteEventSystem = serializedObject.FindProperty ("mainRemoteEventSystem");
		eventInProcess = serializedObject.FindProperty ("eventInProcess");
		eventInfoList = serializedObject.FindProperty ("eventInfoList");

		manager = (eventInfoSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (50));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (eventInfoEnabled);
		EditorGUILayout.PropertyField (useDelayEnabled);

		if (useDelayEnabled.boolValue) {
			EditorGUILayout.PropertyField (useAccumulativeDelay);
		}
		EditorGUILayout.PropertyField (mainRemoteEventSystem);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Info State", "window");
		EditorGUILayout.PropertyField (eventInProcess);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Info List", "window");
		showEventInfoList (eventInfoList);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showEventInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Events: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Event")) {
				manager.addNewEvent ();
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
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showEventInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showEventInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		if (useDelayEnabled.boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToActivate"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToUse"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEvent"));
		if (list.FindPropertyRelative ("useRemoteEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remoteEventName"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventTriggered"));
		GUILayout.EndVertical ();
	}
}
#endif