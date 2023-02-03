using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(waypointPlatform))]
public class waypointPlatformEditor : Editor
{
	SerializedProperty platformActive;
	SerializedProperty playerTag;
	SerializedProperty vehicleTag;
	SerializedProperty platformTransform;
	SerializedProperty waypointsParent;
	SerializedProperty repeatWaypoints;
	SerializedProperty moveInCircles;
	SerializedProperty stopIfPlayerOutside;
	SerializedProperty waitTimeBetweenPoints;
	SerializedProperty movementSpeed;
	SerializedProperty movingForward;
	SerializedProperty useJustToMovePlatform;
	SerializedProperty tagToCheckToMove;
	SerializedProperty tagToCheckBelow;
	SerializedProperty mirrorPlatformMovement;
	SerializedProperty mirrorMovementDirection;
	SerializedProperty platformToMirror;
	SerializedProperty useEventOnWaypointReached;
	SerializedProperty eventOnWaypointReachedList;
	SerializedProperty showGizmo;
	SerializedProperty showVertexHandles;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty useHandleForVertex;
	SerializedProperty handleRadius;
	SerializedProperty handleGizmoColor;
	SerializedProperty wayPoints;

	SerializedProperty deviceStringActionManager;

	waypointPlatform platform;
	GUIStyle style = new GUIStyle ();
	Quaternion currentWaypointRotation;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform currentWaypoint;
	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		platformActive = serializedObject.FindProperty ("platformActive");
		playerTag = serializedObject.FindProperty ("playerTag");
		vehicleTag = serializedObject.FindProperty ("vehicleTag");
		platformTransform = serializedObject.FindProperty ("platformTransform");
		waypointsParent = serializedObject.FindProperty ("waypointsParent");
		repeatWaypoints = serializedObject.FindProperty ("repeatWaypoints");
		moveInCircles = serializedObject.FindProperty ("moveInCircles");
		stopIfPlayerOutside = serializedObject.FindProperty ("stopIfPlayerOutside");
		waitTimeBetweenPoints = serializedObject.FindProperty ("waitTimeBetweenPoints");
		movementSpeed = serializedObject.FindProperty ("movementSpeed");
		movingForward = serializedObject.FindProperty ("movingForward");
		useJustToMovePlatform = serializedObject.FindProperty ("useJustToMovePlatform");
		tagToCheckToMove = serializedObject.FindProperty ("tagToCheckToMove");
		tagToCheckBelow = serializedObject.FindProperty ("tagToCheckBelow");
		mirrorPlatformMovement = serializedObject.FindProperty ("mirrorPlatformMovement");
		mirrorMovementDirection = serializedObject.FindProperty ("mirrorMovementDirection");
		platformToMirror = serializedObject.FindProperty ("platformToMirror");
		useEventOnWaypointReached = serializedObject.FindProperty ("useEventOnWaypointReached");
		eventOnWaypointReachedList = serializedObject.FindProperty ("eventOnWaypointReachedList");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		showVertexHandles = serializedObject.FindProperty ("showVertexHandles");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		useHandleForVertex = serializedObject.FindProperty ("useHandleForVertex");
		handleRadius = serializedObject.FindProperty ("handleRadius");
		handleGizmoColor = serializedObject.FindProperty ("handleGizmoColor");
		wayPoints = serializedObject.FindProperty ("wayPoints");

		deviceStringActionManager = serializedObject.FindProperty ("deviceStringActionManager");

		platform = (waypointPlatform)target;
	}

	void OnSceneGUI ()
	{   
		if (platform.showGizmo) {
			style.normal.textColor = platform.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;

			for (int i = 0; i < platform.wayPoints.Count; i++) {
				if (platform.wayPoints [i] != null) {
					currentWaypoint = platform.wayPoints [i];

					Handles.Label (currentWaypoint.position + Vector3.up, (i + 1).ToString (), style);	

					if (platform.useHandleForVertex) {
						Handles.color = platform.handleGizmoColor;
						EditorGUI.BeginChangeCheck ();

						oldPoint = currentWaypoint.position;
						newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, platform.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentWaypoint, "move Platform Way Point Handle");
							currentWaypoint.position = newPoint;
						}   
					}

					if (platform.showVertexHandles) {
						currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? currentWaypoint.rotation : Quaternion.identity;

						EditorGUI.BeginChangeCheck ();

						oldPoint = currentWaypoint.position;
						oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);
						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentWaypoint, "move waypoint" + i);
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
		EditorGUILayout.PropertyField (platformActive);

		EditorGUILayout.PropertyField (playerTag);
		EditorGUILayout.PropertyField (vehicleTag);

		EditorGUILayout.PropertyField (repeatWaypoints);
		EditorGUILayout.PropertyField (moveInCircles);
		EditorGUILayout.PropertyField (stopIfPlayerOutside);
		EditorGUILayout.PropertyField (waitTimeBetweenPoints);
		EditorGUILayout.PropertyField (movementSpeed);
		EditorGUILayout.PropertyField (movingForward);
		EditorGUILayout.PropertyField (useJustToMovePlatform);

		EditorGUILayout.Space ();

		showSimpleList (tagToCheckToMove);

		EditorGUILayout.Space ();

		showSimpleList (tagToCheckBelow);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Mirror Platform Options", "window");
		EditorGUILayout.PropertyField (mirrorPlatformMovement);
		EditorGUILayout.PropertyField (mirrorMovementDirection);
		EditorGUILayout.PropertyField (platformToMirror);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (useEventOnWaypointReached);
		if (useEventOnWaypointReached.boolValue) {
			showEventOnWaypointReachedList (eventOnWaypointReachedList);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (platformTransform);
		EditorGUILayout.PropertyField (deviceStringActionManager);

		EditorGUILayout.PropertyField (waypointsParent);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (showVertexHandles);
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

		GUILayout.BeginVertical ("Waypoints List", "window");
		showUpperList (wayPoints);
		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();
	}

	void showUpperList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				platform.addNewWayPoint ();
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
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
					if (list.GetArrayElementAtIndex (i).objectReferenceValue) {
						Transform point = list.GetArrayElementAtIndex (i).objectReferenceValue as Transform;
						DestroyImmediate (point.gameObject);
					}
					list.DeleteArrayElementAtIndex (i);
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
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
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
					list.DeleteArrayElementAtIndex (i);
					return;
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

	void showEventOnWaypointReachedList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.Space ();
			GUILayout.Label ("Number Of Events: \t" + list.arraySize);

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
						showEventOnWaypointReachedListElement (list.GetArrayElementAtIndex (i));
						expanded = true;
					}
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
			EditorGUILayout.Space ();
			GUILayout.EndVertical ();
		}       
	}

	void showEventOnWaypointReachedListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("waypointToReach"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnWaypoint"));
		GUILayout.EndVertical ();
	}
}
#endif