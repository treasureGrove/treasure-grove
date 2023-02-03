using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

//a simple editor to add some buttons in the vehicle weapon script inspector
[CustomEditor (typeof(hoverBoardWayPoints))]
public class hoverBoardWayPointsEditor : Editor
{
	SerializedProperty wayPointElement;
	SerializedProperty movementSpeed;
	SerializedProperty moveInOneDirection;
	SerializedProperty triggerRadius;
	SerializedProperty extraRotation;
	SerializedProperty forceAtEnd;
	SerializedProperty railsOffset;
	SerializedProperty extraScale;
	SerializedProperty vehicleTag;
	SerializedProperty modifyMovementSpeedEnabled;
	SerializedProperty maxMovementSpeed;
	SerializedProperty minMovementSpeed;
	SerializedProperty modifyMovementSpeed;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty useHandleForVertex;
	SerializedProperty handleRadius;
	SerializedProperty handleGizmoColor;
	SerializedProperty showVertexHandles;
	SerializedProperty wayPoints;

	hoverBoardWayPoints points;

	GUIStyle style = new GUIStyle ();
	Quaternion currentWaypointRotation;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform currentWaypoint;
	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		wayPointElement = serializedObject.FindProperty ("wayPointElement");
		movementSpeed = serializedObject.FindProperty ("movementSpeed");
		moveInOneDirection = serializedObject.FindProperty ("moveInOneDirection");
		triggerRadius = serializedObject.FindProperty ("triggerRadius");
		extraRotation = serializedObject.FindProperty ("extraRotation");
		forceAtEnd = serializedObject.FindProperty ("forceAtEnd");
		railsOffset = serializedObject.FindProperty ("railsOffset");
		extraScale = serializedObject.FindProperty ("extraScale");
		vehicleTag = serializedObject.FindProperty ("vehicleTag");
		modifyMovementSpeedEnabled = serializedObject.FindProperty ("modifyMovementSpeedEnabled");
		maxMovementSpeed = serializedObject.FindProperty ("maxMovementSpeed");
		minMovementSpeed = serializedObject.FindProperty ("minMovementSpeed");
		modifyMovementSpeed = serializedObject.FindProperty ("modifyMovementSpeed");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		useHandleForVertex = serializedObject.FindProperty ("useHandleForVertex");
		handleRadius = serializedObject.FindProperty ("handleRadius");
		handleGizmoColor = serializedObject.FindProperty ("handleGizmoColor");
		showVertexHandles = serializedObject.FindProperty ("showVertexHandles");
		wayPoints = serializedObject.FindProperty ("wayPoints");

		points = (hoverBoardWayPoints)target;
	}

	void OnSceneGUI ()
	{   
		if (points.showGizmo) {
			style.normal.textColor = points.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;

			for (int i = 0; i < points.wayPoints.Count; i++) {
				if (points.wayPoints [i].wayPoint != null) {
					currentWaypoint = points.wayPoints [i].wayPoint;

					Handles.Label (currentWaypoint.position + Vector3.up, (i + 1).ToString (), style);	

					if (points.useHandleForVertex) {
						Handles.color = points.handleGizmoColor;
						EditorGUI.BeginChangeCheck ();

						oldPoint = currentWaypoint.position;
						newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, points.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);

						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentWaypoint, "move Hover Board Way Point Free Handle" + i);
							currentWaypoint.position = newPoint;
						}   
					}

					if (points.showVertexHandles) {
						currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? currentWaypoint.rotation : Quaternion.identity;

						EditorGUI.BeginChangeCheck ();

						oldPoint = currentWaypoint.position;
						oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);

						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentWaypoint, "move Hover Board Way Point Position Handle" + i);
							currentWaypoint.position = oldPoint;
						}
					}
				}
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
		EditorGUILayout.PropertyField (wayPointElement);
		EditorGUILayout.PropertyField (movementSpeed);
		EditorGUILayout.PropertyField (moveInOneDirection);
		EditorGUILayout.PropertyField (triggerRadius);
		EditorGUILayout.PropertyField (extraRotation);
		EditorGUILayout.PropertyField (forceAtEnd);
		EditorGUILayout.PropertyField (railsOffset);
		EditorGUILayout.PropertyField (extraScale);
		EditorGUILayout.PropertyField (vehicleTag);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Modify Speed Settings", "window");
		EditorGUILayout.PropertyField (modifyMovementSpeedEnabled);
		if (modifyMovementSpeedEnabled.boolValue) {
			EditorGUILayout.PropertyField (maxMovementSpeed);
			EditorGUILayout.PropertyField (minMovementSpeed);
			EditorGUILayout.PropertyField (modifyMovementSpeed);
		}
		GUILayout.EndVertical ();	

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		EditorGUILayout.PropertyField (gizmoLabelColor);
		EditorGUILayout.PropertyField (gizmoRadius);
		EditorGUILayout.PropertyField (useHandleForVertex);
		EditorGUILayout.PropertyField (handleRadius);
		EditorGUILayout.PropertyField (handleGizmoColor);
		EditorGUILayout.PropertyField (showVertexHandles);	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("WayPoints List", "window");
		showUpperList (wayPoints);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("wayPoint"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("direction"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("trigger"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("railMesh"));
		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				points.addNewWayPoint ();
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

			if (GUILayout.Button ("Rename Waypoints")) {
				points.renameAllWaypoints ();
			}

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
						showListElementInfo (list.GetArrayElementAtIndex (i));
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
					points.removeWaypoint (i);
				}
				if (GUILayout.Button ("+")) {
					points.addNewWayPointAtIndex (i);
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