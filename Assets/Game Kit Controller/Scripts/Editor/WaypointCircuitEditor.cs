using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(WaypointCircuit))]
public class WaypointCircuitEditor : Editor
{
	SerializedProperty smoothRoute;
	SerializedProperty editorVisualisationSubsteps;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty useHandleForVertex;
	SerializedProperty handleRadius;
	SerializedProperty handleGizmoColor;
	SerializedProperty waypointList;

	WaypointCircuit manager;
	GUIStyle style = new GUIStyle();

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable()
	{
		smoothRoute = serializedObject.FindProperty ("smoothRoute");
		editorVisualisationSubsteps = serializedObject.FindProperty ("editorVisualisationSubsteps");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		useHandleForVertex = serializedObject.FindProperty ("useHandleForVertex");
		handleRadius = serializedObject.FindProperty ("handleRadius");
		handleGizmoColor = serializedObject.FindProperty ("handleGizmoColor");
		waypointList = serializedObject.FindProperty ("waypointList");

		manager = (WaypointCircuit)target;
	}

	void OnSceneGUI()
	{   
		if (manager.showGizmo) {
			style.normal.textColor = manager.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;
			for (int i = 0; i < manager.waypointList.Count; i++) {
				if (manager.waypointList [i]) {
					Handles.Label (manager.waypointList [i].position, manager.waypointList [i].name, style);
					if (manager.useHandleForVertex) {
						Handles.color = manager.handleGizmoColor;
						EditorGUI.BeginChangeCheck ();

						Vector3 oldPoint = manager.waypointList [i].position;
						Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (manager.waypointList [i], "move waypoint circuit Handle");
							manager.waypointList [i].position = newPoint;
						}   
					}
				}
			}
		}
	}

	public override void OnInspectorGUI()
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window",GUILayout.Height(30));
		EditorGUILayout.PropertyField (smoothRoute);
		EditorGUILayout.PropertyField (editorVisualisationSubsteps);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window",GUILayout.Height(30));
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
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Waypoints List", "window",GUILayout.Height(30));
		showUpperList (waypointList);
		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showUpperList(SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Multi Axes List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded){

			EditorGUILayout.Space();

			GUILayout.Label ("Number Of Waypoints: \t" + list.arraySize);

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Point")){
				manager.addNewWayPoint ();
			}
			if (GUILayout.Button("Clear")){
				list.arraySize=0;
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			for (int i = 0; i < list.arraySize; i++){
				GUILayout.BeginHorizontal();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button("x")){
					if (list.GetArrayElementAtIndex (i).objectReferenceValue) {
						Transform point = list.GetArrayElementAtIndex (i).objectReferenceValue as Transform;
						DestroyImmediate (point.gameObject);
					}
					list.DeleteArrayElementAtIndex(i);
					list.DeleteArrayElementAtIndex(i);
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
				if (GUILayout.Button ("+")) {
					manager.addNewWayPointAtIndex (i);
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal();
			}
			if (GUILayout.Button("Rename Waypoints")){
				manager.renameWaypoints ();
			}
		}       
	}
}
#endif