using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(cameraWaypointSystem))]
public class cameraWaypointSystemEditor : Editor
{
	SerializedProperty currentCameraTransform;
	SerializedProperty waitTimeBetweenPoints;
	SerializedProperty movementSpeed;
	SerializedProperty rotationSpeed;
	SerializedProperty useEventOnEnd;
	SerializedProperty eventOnEnd;
	SerializedProperty useBezierCurve;
	SerializedProperty spline;
	SerializedProperty bezierDuration;
	SerializedProperty snapCameraToFirstSplinePoint;
	SerializedProperty useExternalProgress;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty useHandleForWaypoints;
	SerializedProperty handleRadius;
	SerializedProperty handleGizmoColor;
	SerializedProperty showWaypointHandles;
	SerializedProperty waypointList;

	SerializedProperty searchPlayerOnSceneIfNotAssigned;

	cameraWaypointSystem manager;
	GUIStyle style = new GUIStyle ();

	Quaternion currentWaypointRotation;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform currentWaypoint;
	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		currentCameraTransform = serializedObject.FindProperty ("currentCameraTransform");
		waitTimeBetweenPoints = serializedObject.FindProperty ("waitTimeBetweenPoints");
		movementSpeed = serializedObject.FindProperty ("movementSpeed");
		rotationSpeed = serializedObject.FindProperty ("rotationSpeed");
		useEventOnEnd = serializedObject.FindProperty ("useEventOnEnd");
		eventOnEnd = serializedObject.FindProperty ("eventOnEnd");
		useBezierCurve = serializedObject.FindProperty ("useBezierCurve");
		spline = serializedObject.FindProperty ("spline");
		bezierDuration = serializedObject.FindProperty ("bezierDuration");
		snapCameraToFirstSplinePoint = serializedObject.FindProperty ("snapCameraToFirstSplinePoint");
		useExternalProgress = serializedObject.FindProperty ("useExternalProgress");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		useHandleForWaypoints = serializedObject.FindProperty ("useHandleForWaypoints");
		handleRadius = serializedObject.FindProperty ("handleRadius");
		handleGizmoColor = serializedObject.FindProperty ("handleGizmoColor");
		showWaypointHandles = serializedObject.FindProperty ("showWaypointHandles");
		waypointList = serializedObject.FindProperty ("waypointList");

		searchPlayerOnSceneIfNotAssigned = serializedObject.FindProperty ("searchPlayerOnSceneIfNotAssigned");

		manager = (cameraWaypointSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (manager.showGizmo) {
			style.normal.textColor = manager.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;

			for (int i = 0; i < manager.waypointList.Count; i++) {
				if (manager.waypointList [i].waypointTransform != null) {

					currentWaypoint = manager.waypointList [i].waypointTransform;

					Handles.Label (currentWaypoint.position, (i + 1).ToString (), style);	

					if (manager.useHandleForWaypoints) {
						Handles.color = manager.handleGizmoColor;
						EditorGUI.BeginChangeCheck ();

						oldPoint = currentWaypoint.position;
						newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, manager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);

						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentWaypoint, "move Camera Way Point Handle " + i);
							currentWaypoint.position = newPoint;
						}   
					}

					if (manager.showWaypointHandles) {
						currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? currentWaypoint.rotation : Quaternion.identity;

						EditorGUI.BeginChangeCheck ();

						oldPoint = currentWaypoint.position;
						oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);

						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentWaypoint, "move Camera Way point Free Handle" + i);
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

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (currentCameraTransform);
		EditorGUILayout.PropertyField (waitTimeBetweenPoints);
		EditorGUILayout.PropertyField (movementSpeed);
		EditorGUILayout.PropertyField (rotationSpeed);

		EditorGUILayout.PropertyField (searchPlayerOnSceneIfNotAssigned);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (useEventOnEnd);
		if (useEventOnEnd.boolValue) {
			EditorGUILayout.PropertyField (eventOnEnd);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Bezier Path Settings", "window");
		EditorGUILayout.PropertyField (useBezierCurve);
		if (useBezierCurve.boolValue) {
			EditorGUILayout.PropertyField (spline);
			EditorGUILayout.PropertyField (bezierDuration);
			EditorGUILayout.PropertyField (snapCameraToFirstSplinePoint);

			EditorGUILayout.PropertyField (useExternalProgress);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (gizmoRadius);
			EditorGUILayout.PropertyField (useHandleForWaypoints);
			if (useHandleForWaypoints.boolValue) {
				EditorGUILayout.PropertyField (handleRadius);
				EditorGUILayout.PropertyField (handleGizmoColor);
			}
			EditorGUILayout.PropertyField (showWaypointHandles);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Waypoints List", "window");
		showWaypointList (waypointList);
		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();

	}

	void showWaypointList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Points: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				manager.addNewWayPoint ();
			}
			if (GUILayout.Button ("Clear")) {
				manager.removeAllWaypoints ();
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
						showWaypointListElementInfo (list.GetArrayElementAtIndex (i));
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
					manager.removeWaypoint (i);
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
					manager.addNewWayPoint (i);
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Rename Waypoints")) {
				manager.renameAllWaypoints ();
			}
		}       
	}

	void showWaypointListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("waypointTransform"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Look Direction Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotateCameraToNextWaypoint"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePointToLook"));
		if (list.FindPropertyRelative ("usePointToLook").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("pointToLook"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Transition Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("smoothTransitionToNextPoint"));
		if (list.FindPropertyRelative ("smoothTransitionToNextPoint").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomMovementSpeed"));
			if (list.FindPropertyRelative ("useCustomMovementSpeed").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("movementSpeed"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomRotationSpeed"));
			if (list.FindPropertyRelative ("useCustomRotationSpeed").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationSpeed"));
			}
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("timeOnFixedPosition"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomWaitTimeBetweenPoint"));
		if (list.FindPropertyRelative ("useCustomWaitTimeBetweenPoint").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("waitTimeBetweenPoints"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnPointReached"));
		if (list.FindPropertyRelative ("useEventOnPointReached").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnPointReached"));
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}
}
#endif