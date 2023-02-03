using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(closetSystem))]
[CanEditMultipleObjects]
public class closetSystemeEditor : Editor
{
	SerializedProperty openType;
	SerializedProperty useSounds;
	SerializedProperty closetDoorList;
	SerializedProperty showGizmo;
	SerializedProperty gizmoRadius;
	SerializedProperty gizmoArrowLength;
	SerializedProperty gizmoArrowLineLength;
	SerializedProperty gizmoArrowAngle;
	SerializedProperty gizmoArrowColor;
	SerializedProperty gizmoLabelColor;

	closetSystem manager;
	GUIStyle style = new GUIStyle ();
	Vector3 center;
	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		openType = serializedObject.FindProperty ("openType");
		useSounds = serializedObject.FindProperty ("useSounds");
		closetDoorList = serializedObject.FindProperty ("closetDoorList");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		gizmoArrowLength = serializedObject.FindProperty ("gizmoArrowLength");
		gizmoArrowLineLength = serializedObject.FindProperty ("gizmoArrowLineLength");
		gizmoArrowAngle = serializedObject.FindProperty ("gizmoArrowAngle");
		gizmoArrowColor = serializedObject.FindProperty ("gizmoArrowColor");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");

		manager = (closetSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				if (manager.closetDoorList.Count > 0) {
					style.normal.textColor = manager.gizmoLabelColor;
					style.alignment = TextAnchor.MiddleCenter;

					for (int i = 0; i < manager.closetDoorList.Count; i++) {
						if (manager.closetDoorList [i].doorTransform) {
							Handles.Label (manager.closetDoorList [i].doorTransform.position, manager.closetDoorList [i].Name, style);
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
		EditorGUILayout.PropertyField (openType);
		EditorGUILayout.PropertyField (useSounds);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Door List", "window");
		showClosetDoorList (closetDoorList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Options", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoRadius);
			EditorGUILayout.PropertyField (gizmoArrowLength);
			EditorGUILayout.PropertyField (gizmoArrowLineLength);
			EditorGUILayout.PropertyField (gizmoArrowAngle);
			EditorGUILayout.PropertyField (gizmoArrowColor);
			EditorGUILayout.PropertyField (gizmoLabelColor);	
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showDoorInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("openSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("doorTransform"));
		if (manager.openType == closetSystem.doorOpenType.translate) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("openedPosition"));
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotatedPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCloseRotationTransform"));
			if (list.FindPropertyRelative ("useCloseRotationTransform").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("closeRotationTransform"));
			}
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeOpened"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeClosed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("opened"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("closed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects On Door List", "window");
		showSimpleList (list.FindPropertyRelative ("objectsInDoor"), "Number Of Objects: \t");

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("spawnObjectsInsideAtStart"));
		if (list.FindPropertyRelative ("spawnObjectsInsideAtStart").boolValue) {
			showObjectToSpawnInfoList (list.FindPropertyRelative ("objectToSpawInfoList"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Doors List", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("onlyOneDoor"));
		if (!list.FindPropertyRelative ("onlyOneDoor").boolValue) {
			showSimpleList (list.FindPropertyRelative ("othersDoors"), "Number Of Other Doors: \t");
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sound Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("openSound"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("closeSound"));
		GUILayout.EndVertical ();
	}

	void showClosetDoorList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Doors: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Door")) {
				manager.addNewDoor ();
			}
			if (GUILayout.Button ("Clear List")) {
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
						showDoorInfo (list.GetArrayElementAtIndex (i));
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

	void showSimpleList (SerializedProperty list, string infoText)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
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

	void showObjectToSpawnInfoList(SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space();

			GUILayout.Label("Number Of Objects: \t" + list.arraySize);

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Object")) {
				list.arraySize++;
			}

			if (GUILayout.Button("Clear List")) {
				list.arraySize = 0;
			}

			GUILayout.EndHorizontal();

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex(i).isExpanded = true;
				}
			}

			if (GUILayout.Button("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex(i).isExpanded = false;
				}
			}

			GUILayout.EndHorizontal();

			EditorGUILayout.Space();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal();
				GUILayout.BeginHorizontal("box");

				EditorGUILayout.Space();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical();
					EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), false);
					if (list.GetArrayElementAtIndex(i).isExpanded) {
						expanded = true;
						showObjectToSpawnInfoListElement(list.GetArrayElementAtIndex(i));
					}

					EditorGUILayout.Space();

					GUILayout.EndVertical();
				}

				GUILayout.EndHorizontal();
				if (expanded) {
					GUILayout.BeginVertical();
				} else {
					GUILayout.BeginHorizontal();
				}

				if (GUILayout.Button("x")) {
					list.DeleteArrayElementAtIndex(i);
				}

				if (GUILayout.Button("v")) {
					if (i >= 0) {
						list.MoveArrayElement(i, i + 1);
					}
				}

				if (GUILayout.Button("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement(i, i - 1);
					}
				}

				if (expanded) {
					GUILayout.EndVertical();
				} else {
					GUILayout.EndHorizontal();
				}

				GUILayout.EndHorizontal();
			}
		}
	}

	void showObjectToSpawnInfoListElement(SerializedProperty list)
	{
		GUILayout.BeginVertical("box");
		EditorGUILayout.PropertyField(list.FindPropertyRelative("objectToSpawn"));
		EditorGUILayout.PropertyField(list.FindPropertyRelative("positionToSpawn"));
		GUILayout.EndVertical();
	}
}
#endif