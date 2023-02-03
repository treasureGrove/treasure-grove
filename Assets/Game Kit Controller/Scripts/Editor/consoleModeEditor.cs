using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(consoleMode))]
public class consoleModeEditor : Editor
{
	SerializedProperty consoleModeEnabled;
	SerializedProperty incorrectCommandMessage;
	SerializedProperty lineSpacingAmount;
	SerializedProperty consoleOpened;
	SerializedProperty maxRadiusToInstantiate;
	SerializedProperty deletingTextRate;
	SerializedProperty startDeletingTimeAmount;
	SerializedProperty consoleWindow;
	SerializedProperty commandTextParent;
	SerializedProperty commandTextParentRectTransform;
	SerializedProperty currentConsoleCommandText;
	SerializedProperty input;
	SerializedProperty playerInput;
	SerializedProperty pauseManager;

	SerializedProperty playerControllerManager;
	SerializedProperty mainGameManager;
	SerializedProperty spawnPosition;
	SerializedProperty commandListScrollRect;
	SerializedProperty allowedKeysList;
	SerializedProperty commandInfoList;

	SerializedProperty showDebugPrint;

	SerializedProperty tabKeyName;
	SerializedProperty deleteKeyName;
	SerializedProperty spaceKeyName;
	SerializedProperty backSpaceKeyName;
	SerializedProperty returnKeyName;
	SerializedProperty enterKeyName;
	SerializedProperty leftShiftKeyName;
	SerializedProperty downArrowKeyName;
	SerializedProperty upArrowKeyName;
	SerializedProperty alphaKeyName;
	SerializedProperty capslockKeyName;

	consoleMode manager;
	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		consoleModeEnabled = serializedObject.FindProperty ("consoleModeEnabled");
		incorrectCommandMessage = serializedObject.FindProperty ("incorrectCommandMessage");
		lineSpacingAmount = serializedObject.FindProperty ("lineSpacingAmount");
		consoleOpened = serializedObject.FindProperty ("consoleOpened");
		maxRadiusToInstantiate = serializedObject.FindProperty ("maxRadiusToInstantiate");
		deletingTextRate = serializedObject.FindProperty ("deletingTextRate");
		startDeletingTimeAmount = serializedObject.FindProperty ("startDeletingTimeAmount");
		consoleWindow = serializedObject.FindProperty ("consoleWindow");
		commandTextParent = serializedObject.FindProperty ("commandTextParent");
		commandTextParentRectTransform = serializedObject.FindProperty ("commandTextParentRectTransform");
		currentConsoleCommandText = serializedObject.FindProperty ("currentConsoleCommandText");
		input = serializedObject.FindProperty ("input");
		playerInput = serializedObject.FindProperty ("playerInput");
		pauseManager = serializedObject.FindProperty ("pauseManager");

		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		mainGameManager = serializedObject.FindProperty ("mainGameManager");
		spawnPosition = serializedObject.FindProperty ("spawnPosition");
		commandListScrollRect = serializedObject.FindProperty ("commandListScrollRect");
		allowedKeysList = serializedObject.FindProperty ("allowedKeysList");
		commandInfoList = serializedObject.FindProperty ("commandInfoList");

		showDebugPrint = serializedObject.FindProperty ("showDebugPrint");

		tabKeyName = serializedObject.FindProperty ("tabKeyName");
		deleteKeyName = serializedObject.FindProperty ("deleteKeyName");
		spaceKeyName = serializedObject.FindProperty ("spaceKeyName");
		backSpaceKeyName = serializedObject.FindProperty ("backSpaceKeyName");
		returnKeyName = serializedObject.FindProperty ("returnKeyName");
		enterKeyName = serializedObject.FindProperty ("enterKeyName");
		leftShiftKeyName = serializedObject.FindProperty ("leftShiftKeyName");
		downArrowKeyName = serializedObject.FindProperty ("downArrowKeyName");
		upArrowKeyName = serializedObject.FindProperty ("upArrowKeyName");
		alphaKeyName = serializedObject.FindProperty ("alphaKeyName");
		capslockKeyName = serializedObject.FindProperty ("capslockKeyName");

		manager = (consoleMode)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (consoleModeEnabled);
		EditorGUILayout.PropertyField (incorrectCommandMessage);
		EditorGUILayout.PropertyField (lineSpacingAmount);
		EditorGUILayout.PropertyField (consoleOpened);
		EditorGUILayout.PropertyField (maxRadiusToInstantiate);
		EditorGUILayout.PropertyField (deletingTextRate);
		EditorGUILayout.PropertyField (startDeletingTimeAmount);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Element Settings", "window");
		EditorGUILayout.PropertyField (consoleWindow);
		EditorGUILayout.PropertyField (commandTextParent);
		EditorGUILayout.PropertyField (commandTextParentRectTransform);
		EditorGUILayout.PropertyField (currentConsoleCommandText);
		EditorGUILayout.PropertyField (input);
		EditorGUILayout.PropertyField (playerInput);
		EditorGUILayout.PropertyField (pauseManager);

		EditorGUILayout.PropertyField (playerControllerManager);
		EditorGUILayout.PropertyField (mainGameManager);
		EditorGUILayout.PropertyField (spawnPosition);
		EditorGUILayout.PropertyField (commandListScrollRect);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Key Names Settings", "window");
		EditorGUILayout.PropertyField (tabKeyName);
		EditorGUILayout.PropertyField (deleteKeyName);

		EditorGUILayout.PropertyField (spaceKeyName);
		EditorGUILayout.PropertyField (backSpaceKeyName);
		EditorGUILayout.PropertyField (returnKeyName);
		EditorGUILayout.PropertyField (enterKeyName);
		EditorGUILayout.PropertyField (leftShiftKeyName);
		EditorGUILayout.PropertyField (downArrowKeyName);
		EditorGUILayout.PropertyField (upArrowKeyName);
		EditorGUILayout.PropertyField (alphaKeyName);
		EditorGUILayout.PropertyField (capslockKeyName);
		GUILayout.EndVertical ();	 

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Allowed Key List", "window");
		showAllowedKeyList (allowedKeysList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Command List", "window");
		showCommandInfoList (commandInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (showDebugPrint);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showPrefabTypeListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("description"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("commandExecutedText"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("incorrectParametersText"));

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventSendValues"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsAmount"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsBool"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("containsName"));

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		if (!list.FindPropertyRelative ("eventSendValues").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCall"));
		} else {
			if (list.FindPropertyRelative ("containsAmount").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCallAmount"));
			}

			if (list.FindPropertyRelative ("containsBool").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCallBool"));
			}

			if (list.FindPropertyRelative ("containsName").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCallName"));
			}
		}
	
		GUILayout.EndVertical ();
	}

	void showCommandInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Commands: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Command")) {
				manager.addCommand ();
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
						showPrefabTypeListElement (list.GetArrayElementAtIndex (i));
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

	void showAllowedKeyList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Keys: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Key")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
				}
				GUILayout.EndHorizontal ();

				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
			
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}
}
#endif