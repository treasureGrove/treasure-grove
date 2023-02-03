using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(AIWayPointPatrol))]
public class AIWayPointPatrolEditor : Editor
{
	SerializedProperty waitTimeBetweenPoints;
	SerializedProperty movingForward;
	SerializedProperty layerMask;
	SerializedProperty newWaypointOffset;
	SerializedProperty surfaceAdjusmentOffset;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty useHandleForVertex;
	SerializedProperty handleRadius;
	SerializedProperty handleGizmoColor;
	SerializedProperty patrolList;

	SerializedProperty useFreeHandle;

	AIWayPointPatrol patrol;
	bool expanded;

	GUIStyle style = new GUIStyle ();

	Quaternion currentRotationHandle;

	Vector3 curretPositionHandle;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		waitTimeBetweenPoints = serializedObject.FindProperty ("waitTimeBetweenPoints");
		movingForward = serializedObject.FindProperty ("movingForward");
		layerMask = serializedObject.FindProperty ("layerMask");
		newWaypointOffset = serializedObject.FindProperty ("newWaypointOffset");
		surfaceAdjusmentOffset = serializedObject.FindProperty ("surfaceAdjusmentOffset");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		useHandleForVertex = serializedObject.FindProperty ("useHandleForVertex");
		handleRadius = serializedObject.FindProperty ("handleRadius");
		handleGizmoColor = serializedObject.FindProperty ("handleGizmoColor");
		patrolList = serializedObject.FindProperty ("patrolList");

		useFreeHandle = serializedObject.FindProperty ("useFreeHandle");

		patrol = (AIWayPointPatrol)target;
	}

	void OnSceneGUI ()
	{   
		if (patrol.showGizmo) {
			
			style.normal.textColor = patrol.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;

			for (int i = 0; i < patrol.patrolList.Count; i++) {
				
				if (patrol.patrolList [i].patrolTransform != null) {
					Handles.Label (patrol.patrolList [i].patrolTransform.position, ("Patrol " + (i + 1)), style);
				}

				for (int j = 0; j < patrol.patrolList [i].wayPoints.Count; j++) {
					if (patrol.patrolList [i].wayPoints [j] != null) {
						Handles.Label (patrol.patrolList [i].wayPoints [j].position, (j + 1).ToString (), style);	

						if (patrol.useHandleForVertex) {
							Handles.color = patrol.handleGizmoColor;
							EditorGUI.BeginChangeCheck ();

							Vector3 oldPoint = patrol.patrolList [i].wayPoints [j].position;
							Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, patrol.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
							if (EditorGUI.EndChangeCheck ()) {
								Undo.RecordObject (patrol.patrolList [i].wayPoints [j], "move Patrol Point Handle");
								patrol.patrolList [i].wayPoints [j].position = newPoint;
							}   
						}

						if (patrol.useFreeHandle) {

							Handles.color = patrol.handleGizmoColor;

							showPositionHandle (patrol.patrolList [i].wayPoints [j], "move AI Waypoint handle" + i);
						}
					}
				}

				if (patrol.useHandleForVertex) {
					Handles.color = patrol.handleGizmoColor;
					EditorGUI.BeginChangeCheck ();

					Vector3 oldPoint = patrol.patrolList [i].patrolTransform.position;
					Vector3 newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, patrol.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);

					if (EditorGUI.EndChangeCheck ()) {
						Undo.RecordObject (patrol.patrolList [i].patrolTransform, "move Patrol Parent Handle");
						patrol.patrolList [i].patrolTransform.position = newPoint;
					}   
				}

				if (patrol.useFreeHandle) {

					Handles.color = patrol.handleGizmoColor;

					showPositionHandle (patrol.patrolList [i].patrolTransform, "move Patrol Parent Handle");
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
		EditorGUILayout.PropertyField (waitTimeBetweenPoints);
		EditorGUILayout.PropertyField (movingForward);
		EditorGUILayout.PropertyField (layerMask);
		EditorGUILayout.PropertyField (newWaypointOffset);
		EditorGUILayout.PropertyField (surfaceAdjusmentOffset);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (gizmoRadius);
			EditorGUILayout.PropertyField (useHandleForVertex);

			EditorGUILayout.PropertyField (useFreeHandle);

			if (useHandleForVertex.boolValue || useFreeHandle.boolValue) {
				EditorGUILayout.PropertyField (handleRadius);
				EditorGUILayout.PropertyField (handleGizmoColor);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Patrol List", "window");
		showPatrolList (patrolList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Adjust To Surface")) {
			patrol.adjustWayPoints ();
		}

//		if (GUILayout.Button ("Invert Path")) {
//			patrol.invertPath ();
//		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showPatrolInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("patrolTransform"));
		GUILayout.BeginVertical ("WayPoint List", "window");

		showWayPointList (list.FindPropertyRelative ("wayPoints"), index);

		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}

	void showWayPointList (SerializedProperty list, int index)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of WayPoints: " + list.arraySize);
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				patrol.addNewWayPoint (index);
			}
			if (GUILayout.Button ("Clear List")) {
				patrol.clearWayPoint (index);
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				if (GUILayout.Button ("x")) {
					patrol.removeWaypoint (index, i);
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
		}       
	}

	void showPatrolList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Patrols: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Patrol")) {
				patrol.addNewPatrol ();
			}
			if (GUILayout.Button ("Clear List")) {
				patrol.clearPatrolList ();
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
						showPatrolInfo (list.GetArrayElementAtIndex (i), i);
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
					patrol.clearWayPoint (i);
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

	public void showPositionHandle (Transform currentTransform, string handleName)
	{
		currentRotationHandle = Tools.pivotRotation == PivotRotation.Local ? currentTransform.rotation : Quaternion.identity;

		EditorGUI.BeginChangeCheck ();

		curretPositionHandle = currentTransform.position;

		if (Tools.current == Tool.Move) {
			curretPositionHandle = Handles.DoPositionHandle (curretPositionHandle, currentRotationHandle);
		}

		currentRotationHandle = currentTransform.rotation;

		if (Tools.current == Tool.Rotate) {
			currentRotationHandle = Handles.DoRotationHandle (currentRotationHandle, curretPositionHandle);
		}

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (currentTransform, handleName);
			currentTransform.position = curretPositionHandle;
			currentTransform.rotation = currentRotationHandle;
		}
	}
}
#endif