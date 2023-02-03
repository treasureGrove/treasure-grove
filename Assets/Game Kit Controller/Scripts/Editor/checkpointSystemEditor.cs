using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(checkpointSystem))]
public class checkpointSystemEditor : Editor
{
	SerializedProperty checkpointSceneID;
	SerializedProperty checkPointPrefab;
	SerializedProperty layerToPlaceNewCheckpoints;
	SerializedProperty checkpointsPositionOffset;
	SerializedProperty triggerScale;
	SerializedProperty deathLoackCheckpointType;
	SerializedProperty layerToRespawn;
	SerializedProperty currentCheckpointElement;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty useHandleForVertex;
	SerializedProperty handleRadius;
	SerializedProperty handleGizmoColor;
	SerializedProperty checkPointList;

	checkpointSystem manager;
	GUIStyle style = new GUIStyle ();

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		checkpointSceneID = serializedObject.FindProperty ("checkpointSceneID");
		checkPointPrefab = serializedObject.FindProperty ("checkPointPrefab");
		layerToPlaceNewCheckpoints = serializedObject.FindProperty ("layerToPlaceNewCheckpoints");
		checkpointsPositionOffset = serializedObject.FindProperty ("checkpointsPositionOffset");
		triggerScale = serializedObject.FindProperty ("triggerScale");
		deathLoackCheckpointType = serializedObject.FindProperty ("deathLoackCheckpointType");
		layerToRespawn = serializedObject.FindProperty ("layerToRespawn");
		currentCheckpointElement = serializedObject.FindProperty ("currentCheckpointElement");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		useHandleForVertex = serializedObject.FindProperty ("useHandleForVertex");
		handleRadius = serializedObject.FindProperty ("handleRadius");
		handleGizmoColor = serializedObject.FindProperty ("handleGizmoColor");
		checkPointList = serializedObject.FindProperty ("checkPointList");

		manager = (checkpointSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (manager.showGizmo) {
			style.normal.textColor = manager.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;

			for (int i = 0; i < manager.checkPointList.Count; i++) {
				if (manager.checkPointList [i] != null) {
					Handles.Label (manager.checkPointList [i].position, (i + 1).ToString (), style);	

					if (manager.useHandleForVertex) {
						Handles.color = manager.handleGizmoColor;
						EditorGUI.BeginChangeCheck ();

						Vector3 oldPoint = manager.checkPointList [i].position;
						Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);

						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (manager.checkPointList [i], "move Checkpoint Handle " + i);
							manager.checkPointList [i].position = newPoint;
						}   
					}
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (checkpointSceneID);
		EditorGUILayout.PropertyField (checkPointPrefab);
		EditorGUILayout.PropertyField (layerToPlaceNewCheckpoints);
		EditorGUILayout.PropertyField (checkpointsPositionOffset);
		EditorGUILayout.PropertyField (triggerScale);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Respawn Settings", "window");
		EditorGUILayout.PropertyField (deathLoackCheckpointType);
		EditorGUILayout.PropertyField (layerToRespawn);
		EditorGUILayout.PropertyField (currentCheckpointElement);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (gizmoRadius);
			EditorGUILayout.PropertyField (useHandleForVertex);
			if (useHandleForVertex.boolValue) {
				EditorGUILayout.PropertyField (handleRadius);
				EditorGUILayout.PropertyField (handleGizmoColor);
			}
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Checkpoints List", "window");
		showCheckpointsList (checkPointList);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showCheckpointsList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Checkpoint")) {
				manager.addNewCheckpoint ();
			}
			if (GUILayout.Button ("Clear")) {
				manager.removeAllCheckpoints ();
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
					manager.removeCheckpoint (i);
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
			}
		}       
	}
}
#endif
