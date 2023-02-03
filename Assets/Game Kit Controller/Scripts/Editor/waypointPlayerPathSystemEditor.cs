using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(waypointPlayerPathSystem))]
public class waypointPlayerPathSystemEditor : Editor
{
	SerializedProperty inOrder;
	SerializedProperty showOneByOne;
	SerializedProperty triggerRadius;
	SerializedProperty showOffScreenIcon;
	SerializedProperty showMapWindowIcon;
	SerializedProperty showDistance;
	SerializedProperty pathActive;
	SerializedProperty eventOnPathComplete;
	SerializedProperty eventOnPathIncomplete;
	SerializedProperty useTimer;
	SerializedProperty timerSpeed;
	SerializedProperty minutesToComplete;
	SerializedProperty secondsToComplete;
	SerializedProperty extraTimePerPoint;
	SerializedProperty pathCompleteAudioSound;
	SerializedProperty pathUncompleteAudioSound;
	SerializedProperty secondTimerSound;
	SerializedProperty secondSoundTimerLowerThan;
	SerializedProperty pointReachedSound;
	SerializedProperty useLineRenderer;
	SerializedProperty lineRendererColor;
	SerializedProperty lineRendererWidth;
	SerializedProperty showGizmo;
	SerializedProperty showInfoLabel;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty useRegularGizmoRadius;
	SerializedProperty useHandleForVertex;
	SerializedProperty handleRadius;
	SerializedProperty handleGizmoColor;
	SerializedProperty showDoPositionHandles;
	SerializedProperty wayPoints;
	SerializedProperty pointsReached;

	SerializedProperty mainManagerName;

	waypointPlayerPathSystem pathManager;
	GUIStyle style = new GUIStyle ();
	bool advancedSettings;
	Color buttonColor;
	mapObjectInformation currentMapObjectInformation;
	Quaternion currentWaypointRotation;
	Vector3 oldPoint;
	Vector3 newPoint;
	Transform currentWaypoint;
	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		inOrder = serializedObject.FindProperty ("inOrder");
		showOneByOne = serializedObject.FindProperty ("showOneByOne");
		triggerRadius = serializedObject.FindProperty ("triggerRadius");
		showOffScreenIcon = serializedObject.FindProperty ("showOffScreenIcon");
		showMapWindowIcon = serializedObject.FindProperty ("showMapWindowIcon");
		showDistance = serializedObject.FindProperty ("showDistance");
		pathActive = serializedObject.FindProperty ("pathActive");
		eventOnPathComplete = serializedObject.FindProperty ("eventOnPathComplete");
		eventOnPathIncomplete = serializedObject.FindProperty ("eventOnPathIncomplete");
		useTimer = serializedObject.FindProperty ("useTimer");
		timerSpeed = serializedObject.FindProperty ("timerSpeed");
		minutesToComplete = serializedObject.FindProperty ("minutesToComplete");
		secondsToComplete = serializedObject.FindProperty ("secondsToComplete");
		extraTimePerPoint = serializedObject.FindProperty ("extraTimePerPoint");
		pathCompleteAudioSound = serializedObject.FindProperty ("pathCompleteAudioSound");
		pathUncompleteAudioSound = serializedObject.FindProperty ("pathUncompleteAudioSound");
		secondTimerSound = serializedObject.FindProperty ("secondTimerSound");
		secondSoundTimerLowerThan = serializedObject.FindProperty ("secondSoundTimerLowerThan");
		pointReachedSound = serializedObject.FindProperty ("pointReachedSound");
		useLineRenderer = serializedObject.FindProperty ("useLineRenderer");
		lineRendererColor = serializedObject.FindProperty ("lineRendererColor");
		lineRendererWidth = serializedObject.FindProperty ("lineRendererWidth");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		showInfoLabel = serializedObject.FindProperty ("showInfoLabel");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		useRegularGizmoRadius = serializedObject.FindProperty ("useRegularGizmoRadius");
		useHandleForVertex = serializedObject.FindProperty ("useHandleForVertex");
		handleRadius = serializedObject.FindProperty ("handleRadius");
		handleGizmoColor = serializedObject.FindProperty ("handleGizmoColor");
		showDoPositionHandles = serializedObject.FindProperty ("showDoPositionHandles");
		wayPoints = serializedObject.FindProperty ("wayPoints");
		pointsReached = serializedObject.FindProperty ("pointsReached");

		mainManagerName = serializedObject.FindProperty ("mainManagerName");

		pathManager = (waypointPlayerPathSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (pathManager.showGizmo) {
				style.normal.textColor = pathManager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				for (int i = 0; i < pathManager.wayPoints.Count; i++) {
					currentWaypoint = pathManager.wayPoints [i].point;

					if (currentWaypoint) {
						string label = "Point: " + pathManager.wayPoints [i].Name;

						currentMapObjectInformation = currentWaypoint.GetComponent<mapObjectInformation> ();

						if (pathManager.showInfoLabel) {
							label += "\n Radius: ";
							if (pathManager.useRegularGizmoRadius) {
								label += pathManager.triggerRadius;
							} else {
								label += pathManager.wayPoints [i].triggerRadius;
							}

							label += "\n Show OffScreen Icon: ";

							if (currentMapObjectInformation.showOffScreenIcon) {
								label += "On";
							} else {
								label += "Off";
							}

							label += "\n Show Map Icon: ";
							if (currentMapObjectInformation.showMapWindowIcon) {
								label += "On";
							} else {
								label += "Off";
							}

							label += "\n Show Distance: ";
							if (currentMapObjectInformation.showDistance) {
								label += "On";
							} else {
								label += "Off";
							}
						}

						Handles.Label (currentWaypoint.position + currentWaypoint.up * currentMapObjectInformation.triggerRadius, label, style);

						currentMapObjectInformation.showGizmo = pathManager.showGizmo;
						currentMapObjectInformation.showOffScreenIcon = pathManager.showOffScreenIcon;
						currentMapObjectInformation.showMapWindowIcon = pathManager.showMapWindowIcon;
						currentMapObjectInformation.showDistance = pathManager.showDistance;

						if (pathManager.useRegularGizmoRadius) {
							currentMapObjectInformation.triggerRadius = pathManager.triggerRadius;
						} else {
							currentMapObjectInformation.triggerRadius = pathManager.wayPoints [i].triggerRadius;
						}

						if (pathManager.useHandleForVertex) {
							Handles.color = pathManager.handleGizmoColor;
							EditorGUI.BeginChangeCheck ();

							oldPoint = currentWaypoint.position;
							newPoint = Handles.FreeMoveHandle (oldPoint, Quaternion.identity, pathManager.handleRadius, new Vector3 (.25f, .25f, .25f), Handles.CircleHandleCap);
							if (EditorGUI.EndChangeCheck ()) {
								Undo.RecordObject (currentWaypoint, "move Player Path Way Point Handle " + i.ToString ());
								currentWaypoint.position = newPoint;
							}   
						}

						if (pathManager.showDoPositionHandles) {
							currentWaypointRotation = Tools.pivotRotation == PivotRotation.Local ? currentWaypoint.rotation : Quaternion.identity;

							EditorGUI.BeginChangeCheck ();

							oldPoint = currentWaypoint.position;
							oldPoint = Handles.DoPositionHandle (oldPoint, currentWaypointRotation);
							if (EditorGUI.EndChangeCheck ()) {
								Undo.RecordObject (currentWaypoint, "move Player Path Way Point Do Position Handle" + i);
								currentWaypoint.position = oldPoint;
							}
						}
					}
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical ("box", GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (inOrder);
		EditorGUILayout.PropertyField (showOneByOne);
		EditorGUILayout.PropertyField (triggerRadius);
		EditorGUILayout.PropertyField (showOffScreenIcon);
		EditorGUILayout.PropertyField (showMapWindowIcon);
		EditorGUILayout.PropertyField (showDistance);
		EditorGUILayout.PropertyField (pathActive);
		EditorGUILayout.PropertyField (mainManagerName);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events To Call Settings", "window");
		EditorGUILayout.PropertyField (eventOnPathComplete);
		EditorGUILayout.PropertyField (eventOnPathIncomplete);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		string inputListOpenedText = "";
		if (advancedSettings) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Advanced Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Advanced Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			advancedSettings = !advancedSettings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndHorizontal ();

		if (advancedSettings) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure timer options and functions to call once all the path is complete", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useTimer);
			EditorGUILayout.PropertyField (timerSpeed);
			EditorGUILayout.PropertyField (minutesToComplete);
			EditorGUILayout.PropertyField (secondsToComplete);
			EditorGUILayout.PropertyField (extraTimePerPoint);
			EditorGUILayout.PropertyField (pathCompleteAudioSound);
			EditorGUILayout.PropertyField (pathUncompleteAudioSound);
			EditorGUILayout.PropertyField (secondTimerSound);
			EditorGUILayout.PropertyField (secondSoundTimerLowerThan);
			EditorGUILayout.PropertyField (pointReachedSound);
			EditorGUILayout.PropertyField (useLineRenderer);
			EditorGUILayout.PropertyField (lineRendererColor);
			EditorGUILayout.PropertyField (lineRendererWidth);

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (showInfoLabel);
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (gizmoRadius);
			EditorGUILayout.PropertyField (useRegularGizmoRadius);
			EditorGUILayout.PropertyField (useHandleForVertex);
			if (useHandleForVertex.boolValue) {
				EditorGUILayout.PropertyField (handleRadius);
				EditorGUILayout.PropertyField (handleGizmoColor);
			}
			EditorGUILayout.PropertyField (showDoPositionHandles);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Waypoints List", "window");
		showUpperList (wayPoints);
		EditorGUILayout.Space ();
		if (GUILayout.Button ("Rename WayPoints")) {
			pathManager.renamePoints ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("point"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("triggerRadius"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("reached"));
		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.Label ("Number Of Points: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.Label ("Reached points: \t" + pointsReached.intValue);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Point")) {
				pathManager.addNewWayPoint ();
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
					Transform point = list.GetArrayElementAtIndex (i).FindPropertyRelative ("point").objectReferenceValue as Transform;
					DestroyImmediate (point.gameObject);
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
}
#endif