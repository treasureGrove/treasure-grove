using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(vehicleInterface))]
public class vehicleInterfaceEditor : Editor
{
	SerializedProperty interfaceCanBeEnabled;
	SerializedProperty interfaceEnabled;
	SerializedProperty vehicle;
	SerializedProperty interfaceCanvas;
	SerializedProperty interfaceElementList;
	SerializedProperty useInterfacePanelInfoList;
	SerializedProperty HUDManager;
	SerializedProperty interfacePanelParent;
	SerializedProperty movePanelSpeed;
	SerializedProperty rotatePanelSpeed;
	SerializedProperty interfacePanelInfoList;

	vehicleInterface manager;
	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		interfaceCanBeEnabled = serializedObject.FindProperty ("interfaceCanBeEnabled");
		interfaceEnabled = serializedObject.FindProperty ("interfaceEnabled");
		vehicle = serializedObject.FindProperty ("vehicle");
		interfaceCanvas = serializedObject.FindProperty ("interfaceCanvas");
		interfaceElementList = serializedObject.FindProperty ("interfaceElementList");
		useInterfacePanelInfoList = serializedObject.FindProperty ("useInterfacePanelInfoList");
		HUDManager = serializedObject.FindProperty ("HUDManager");
		interfacePanelParent = serializedObject.FindProperty ("interfacePanelParent");
		movePanelSpeed = serializedObject.FindProperty ("movePanelSpeed");
		rotatePanelSpeed = serializedObject.FindProperty ("rotatePanelSpeed");
		interfacePanelInfoList = serializedObject.FindProperty ("interfacePanelInfoList");

		manager = (vehicleInterface)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (interfaceCanBeEnabled);
		EditorGUILayout.PropertyField (interfaceEnabled);
		EditorGUILayout.PropertyField (vehicle);
		EditorGUILayout.PropertyField (interfaceCanvas);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Interface Elements List", "window");
		showMainInterfaceElementList (interfaceElementList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Interface Panel Info List", "window");
		EditorGUILayout.PropertyField (useInterfacePanelInfoList);
		if (useInterfacePanelInfoList.boolValue) {
			EditorGUILayout.PropertyField (HUDManager);
			EditorGUILayout.PropertyField (interfacePanelParent);
			EditorGUILayout.PropertyField (movePanelSpeed);
			EditorGUILayout.PropertyField (rotatePanelSpeed);

			EditorGUILayout.Space ();

			showInterfacePanelInfoList (interfacePanelInfoList);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showInterfacePanelInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Elements: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Element")) {
				manager.addInterfaceElement ();
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
						showInterfacePanelInfoListElement (list.GetArrayElementAtIndex (i));
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
		GUILayout.EndVertical ();
	}

	void showInterfacePanelInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("uiRectTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("panelParent"));

		GUILayout.EndVertical ();
	}

	void showMainInterfaceElementList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Elements: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Element")) {
				list.arraySize++;
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
						showMainInterfaceElementListElement (list.GetArrayElementAtIndex (i));
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
		GUILayout.EndVertical ();
	}

	void showMainInterfaceElementListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("uiElement"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableWhenVehicleOff"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventSendValues"));

		if (list.FindPropertyRelative ("eventSendValues").boolValue) {
			GUILayout.BeginVertical ("Send Amount Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsAmount"));
			if (list.FindPropertyRelative ("containsAmount").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsRange"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("range"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentAmountValue"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCallAmount"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Send Bool Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsBool"));
			if (list.FindPropertyRelative ("containsBool").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentBoolValue"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("setValueOnText"));
				if (list.FindPropertyRelative ("setValueOnText").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("valuetText"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomValueOnText"));
				if (list.FindPropertyRelative ("useCustomValueOnText").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("boolActiveCustomText"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("boolNoActiveCustomText"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCallBool"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCall"));
		}

		GUILayout.EndVertical ();
	}
}
#endif