using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(externalCameraShakeSystem))]
[CanEditMultipleObjects]
public class externalCameraShakeSystemEditor : Editor
{
	SerializedProperty useShakeListTriggeredByActions;
	SerializedProperty nameList;
	SerializedProperty nameIndex;
	SerializedProperty externalShakeName;
	SerializedProperty setPlayerManually;
	SerializedProperty currentPlayer;
	SerializedProperty shakeTriggeredByActionList;
	SerializedProperty shakeUsingDistance;
	SerializedProperty minDistanceToShake;
	SerializedProperty layer;
	SerializedProperty useShakeEvent;
	SerializedProperty eventAtStart;
	SerializedProperty eventAtEnd;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;

	SerializedProperty mainManagerName;

	externalCameraShakeSystem shakeManager;

	GUIStyle style = new GUIStyle ();

	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		useShakeListTriggeredByActions = serializedObject.FindProperty ("useShakeListTriggeredByActions");
		nameList = serializedObject.FindProperty ("nameList");
		nameIndex = serializedObject.FindProperty ("nameIndex");
		externalShakeName = serializedObject.FindProperty ("externalShakeName");
		setPlayerManually = serializedObject.FindProperty ("setPlayerManually");
		currentPlayer = serializedObject.FindProperty ("currentPlayer");
		shakeTriggeredByActionList = serializedObject.FindProperty ("shakeTriggeredByActionList");
		shakeUsingDistance = serializedObject.FindProperty ("shakeUsingDistance");
		minDistanceToShake = serializedObject.FindProperty ("minDistanceToShake");
		layer = serializedObject.FindProperty ("layer");
		useShakeEvent = serializedObject.FindProperty ("useShakeEvent");
		eventAtStart = serializedObject.FindProperty ("eventAtStart");
		eventAtEnd = serializedObject.FindProperty ("eventAtEnd");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");

		mainManagerName = serializedObject.FindProperty ("mainManagerName");

		shakeManager = (externalCameraShakeSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (shakeManager.showGizmo) {
				
				style.normal.textColor = shakeManager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				Handles.Label (shakeManager.transform.position + shakeManager.transform.up * shakeManager.minDistanceToShake,
					"External Shake: " + shakeManager.externalShakeName, style);
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (mainManagerName);

		EditorGUILayout.Space ();

		if (!useShakeListTriggeredByActions.boolValue) {
			if (nameList.arraySize > 0) {
				nameIndex.intValue = EditorGUILayout.Popup ("External Shake Type", nameIndex.intValue, shakeManager.nameList);
				externalShakeName.stringValue = shakeManager.nameList [nameIndex.intValue];
			} 
		}

		EditorGUILayout.PropertyField (setPlayerManually);
		if (setPlayerManually.boolValue || useShakeListTriggeredByActions.boolValue) {
			EditorGUILayout.PropertyField (currentPlayer);
		}

		EditorGUILayout.PropertyField (useShakeListTriggeredByActions);
		if (useShakeListTriggeredByActions.boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Shake Triggered By Action List", "window");
			showShakeTriggeredByActionList (shakeTriggeredByActionList);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		} else {
			EditorGUILayout.PropertyField (shakeUsingDistance);
			EditorGUILayout.PropertyField (minDistanceToShake);
			EditorGUILayout.PropertyField (layer);
			EditorGUILayout.PropertyField (useShakeEvent);
			if (useShakeEvent.boolValue) {
				EditorGUILayout.PropertyField (eventAtStart);
				EditorGUILayout.PropertyField (eventAtEnd);
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window");
			EditorGUILayout.PropertyField (showGizmo);
			if (showGizmo.boolValue) {
				EditorGUILayout.PropertyField (gizmoLabelColor);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update External Shake List")) {
			shakeManager.getExternalShakeList ();
		}
			
		if (!useShakeListTriggeredByActions.boolValue) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Debug Options", "window");
			if (GUILayout.Button ("Test Shake")) {
				if (Application.isPlaying) {
					shakeManager.setCameraShake ();
				}
			}
		
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showShakeTriggeredByActionList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Action")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
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
						showShakeTriggeredByActionListElement (list.GetArrayElementAtIndex (i), i);
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
	}

	void showShakeTriggeredByActionListElement (SerializedProperty list, int shakeIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionName"));
		if (shakeManager.nameList.Length > 0) {
			list.FindPropertyRelative ("nameIndex").intValue = EditorGUILayout.Popup ("External Shake Type", list.FindPropertyRelative ("nameIndex").intValue, shakeManager.nameList);
			list.FindPropertyRelative ("shakeName").stringValue = shakeManager.nameList [list.FindPropertyRelative ("nameIndex").intValue];

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useShakeEvent"));
			if (list.FindPropertyRelative ("useShakeEvent").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventAtStart"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventAtEnd"));
			}

			if (GUILayout.Button ("Test Shake")) {
				if (Application.isPlaying) {
					shakeManager.setCameraShakeByAction (list.FindPropertyRelative ("actionName").stringValue);
				}
			}
		} 
		GUILayout.EndVertical ();
	}
}
#endif