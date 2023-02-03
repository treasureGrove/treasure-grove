using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerStateSystem))]
public class playerStateSystemEditor : Editor
{
	SerializedProperty playerStatesEnabled;
	SerializedProperty currentStateName;
	SerializedProperty playerStateInfoList;

	SerializedProperty activateStateOnStart;
	SerializedProperty stateToActivateOnStart;

	playerStateSystem manager;

	bool expanded = false;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		playerStatesEnabled = serializedObject.FindProperty ("playerStatesEnabled");
		currentStateName = serializedObject.FindProperty ("currentStateName");
		playerStateInfoList = serializedObject.FindProperty ("playerStateInfoList");

		activateStateOnStart = serializedObject.FindProperty ("activateStateOnStart");
		stateToActivateOnStart = serializedObject.FindProperty ("stateToActivateOnStart");

		manager = (playerStateSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (playerStatesEnabled);

		EditorGUILayout.PropertyField (activateStateOnStart);
		if (activateStateOnStart.boolValue) {
			EditorGUILayout.PropertyField (stateToActivateOnStart);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("States", "window");
		GUILayout.Label ("Current State Name \t\t" + currentStateName.stringValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Screen Actions List", "window");
		showPlayerStateInfoList (playerStateInfoList);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showPlayerStateInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.Label ("Number Of States: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
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
						showPlayerStateInfoListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
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
					GUILayout.EndVertical ();
				} else {
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
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showPlayerStateInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stateEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stateActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("statePriority"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeInterruptedWhileActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStateDuration"));
		if (list.FindPropertyRelative ("useStateDuration").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("stateDuration"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnStateStart"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnStateEnd"));

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useTemporalStateDuration"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("temporalStateDuration"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		if (GUILayout.Button ("Activate State")) {
			if (Application.isPlaying) {
				manager.setPlayerState (list.FindPropertyRelative ("Name").stringValue);
			}
		}
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}
}
#endif