using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(remoteEventSystem))]
public class remoteEventSystemEditor : Editor
{
	SerializedProperty remoteEventsEnabled;
	SerializedProperty eventInfoList;

	SerializedProperty activateRemoteEventsOnStart;

	SerializedProperty remoteEventsOnStartList;

	SerializedProperty currentArrayElement;

	SerializedProperty showDebugPrint;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		remoteEventsEnabled = serializedObject.FindProperty ("remoteEventsEnabled");
		eventInfoList = serializedObject.FindProperty ("eventInfoList");

		activateRemoteEventsOnStart = serializedObject.FindProperty ("activateRemoteEventsOnStart");

		remoteEventsOnStartList = serializedObject.FindProperty ("remoteEventsOnStartList");

		showDebugPrint = serializedObject.FindProperty ("showDebugPrint");
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical (GUILayout.Height (50));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (remoteEventsEnabled);
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Remote Events On Start Settings", "window");
		EditorGUILayout.PropertyField (activateRemoteEventsOnStart);
		if (activateRemoteEventsOnStart.boolValue) {

			EditorGUILayout.Space ();

			showSimpleList (remoteEventsOnStartList, "Remote Events On Start List");
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Info List", "window");
		showEventInfoList (eventInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (showDebugPrint);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showEventInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Event Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			int currentArraySize = list.arraySize;

			GUILayout.Label ("Number of Events: " + currentArraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Event")) {
				list.arraySize++;

				currentArraySize = list.arraySize;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;

				currentArraySize = list.arraySize;
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

			for (int i = 0; i < currentArraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						showEventInfoListElement (currentArrayElement);
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

					currentArraySize = list.arraySize;
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

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableEventAfterActivation"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRegularEvent"));
		if (list.FindPropertyRelative ("useRegularEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActive"));
		}

		EditorGUILayout.Space ();
	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAmountOnEvent"));
		if (list.FindPropertyRelative ("useAmountOnEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActiveAmount"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBoolOnEvent"));
		if (list.FindPropertyRelative ("useBoolOnEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActiveBool"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useGameObjectOnEvent"));
		if (list.FindPropertyRelative ("useGameObjectOnEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActiveGameObject"));
		}

		EditorGUILayout.Space ();
	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useTransformOnEvent"));
		if (list.FindPropertyRelative ("useTransformOnEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToActiveTransform"));
		}

		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list, string infoText)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + infoText, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label (infoText + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
			}
			GUILayout.EndHorizontal ();

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