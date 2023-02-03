using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(elevatorSystem))]
public class elevatorSystemEditor : Editor
{
	SerializedProperty elevatorSystemEnabled;

	SerializedProperty currentFloor;
	SerializedProperty elevatorSpeed;
	SerializedProperty floorHeight;
	SerializedProperty hasInsideElevatorDoor;
	SerializedProperty insideElevatorDoor;
	SerializedProperty elevatorSwitchPrefab;
	SerializedProperty elevatorDoorPrefab;
	SerializedProperty addSwitchInNewFloors;
	SerializedProperty addDoorInNewFloors;
	SerializedProperty doorsClosed;
	SerializedProperty changeIconFloorWhenMoving;
	SerializedProperty moving;
	SerializedProperty useEventsOnMoveStartAndEnd;
	SerializedProperty eventOnMoveStart;
	SerializedProperty eventOnMoveEnd;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty floors;

	elevatorSystem elevator;
	GUIStyle style = new GUIStyle ();
	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		elevatorSystemEnabled = serializedObject.FindProperty ("elevatorSystemEnabled");

		currentFloor = serializedObject.FindProperty ("currentFloor");
		elevatorSpeed = serializedObject.FindProperty ("elevatorSpeed");
		floorHeight = serializedObject.FindProperty ("floorHeight");
		hasInsideElevatorDoor = serializedObject.FindProperty ("hasInsideElevatorDoor");
		insideElevatorDoor = serializedObject.FindProperty ("insideElevatorDoor");
		elevatorSwitchPrefab = serializedObject.FindProperty ("elevatorSwitchPrefab");
		elevatorDoorPrefab = serializedObject.FindProperty ("elevatorDoorPrefab");
		addSwitchInNewFloors = serializedObject.FindProperty ("addSwitchInNewFloors");
		addDoorInNewFloors = serializedObject.FindProperty ("addDoorInNewFloors");
		doorsClosed = serializedObject.FindProperty ("doorsClosed");
		changeIconFloorWhenMoving = serializedObject.FindProperty ("changeIconFloorWhenMoving");
		moving = serializedObject.FindProperty ("moving");
		useEventsOnMoveStartAndEnd = serializedObject.FindProperty ("useEventsOnMoveStartAndEnd");
		eventOnMoveStart = serializedObject.FindProperty ("eventOnMoveStart");
		eventOnMoveEnd = serializedObject.FindProperty ("eventOnMoveEnd");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		floors = serializedObject.FindProperty ("floors");

		elevator = (elevatorSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (elevator.showGizmo) {
				style.normal.textColor = elevator.gizmoLabelColor;

				for (int i = 0; i < elevator.floors.Count; i++) {
					if (elevator.floors [i].floorPosition != null) {
						Handles.Label (elevator.floors [i].floorPosition.position, elevator.floors [i].name + " - " + elevator.floors [i].floorNumber, style);
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
		EditorGUILayout.PropertyField (elevatorSystemEnabled);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (currentFloor);
		EditorGUILayout.PropertyField (elevatorSpeed);
		EditorGUILayout.PropertyField (floorHeight);
		EditorGUILayout.PropertyField (hasInsideElevatorDoor);
		if (hasInsideElevatorDoor.boolValue) {
			EditorGUILayout.PropertyField (insideElevatorDoor);
		}

		EditorGUILayout.PropertyField (elevatorSwitchPrefab);
		EditorGUILayout.PropertyField (elevatorDoorPrefab);
		EditorGUILayout.PropertyField (addSwitchInNewFloors);
		EditorGUILayout.PropertyField (addDoorInNewFloors);
	
		EditorGUILayout.PropertyField (doorsClosed);
		EditorGUILayout.PropertyField (changeIconFloorWhenMoving);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Elevator State", "window");		
		GUILayout.Label ("Moving\t " + moving.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (useEventsOnMoveStartAndEnd);
		if (useEventsOnMoveStartAndEnd.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnMoveStart);
			EditorGUILayout.PropertyField (eventOnMoveEnd);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoLabelColor);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Floors List", "window");
		showUpperList (floors);
		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorNumber"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorPosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hasFloorButton"));
		if (list.FindPropertyRelative ("hasFloorButton").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorButton"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hasOutSideElevatorDoor"));
		if (list.FindPropertyRelative ("hasOutSideElevatorDoor").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("outsideElevatorDoor"));
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventsOnMoveStartAndEnd"));
		if (list.FindPropertyRelative ("useEventsOnMoveStartAndEnd").boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnMoveStart"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnMoveEnd"));
		}
		GUILayout.EndVertical ();
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

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Floors: \t" + list.arraySize);

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Floor")) {
				elevator.addNewFloor ();
			}
			if (GUILayout.Button ("Clear List")) {
				elevator.removeAllFloors ();
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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				expanded = false;
				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showListElementInfo (list.GetArrayElementAtIndex (i));
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
					elevator.removeFloor (i);
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