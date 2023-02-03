using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerInputManager))]
public class playerInputManagerEditor : Editor
{
	SerializedProperty playerID;
	SerializedProperty showDebugActions;
	SerializedProperty useOnlyKeyboard;
	SerializedProperty mouseSensitivity;
	SerializedProperty leftGamepadJoystickSensitivity;
	SerializedProperty rightGamepadJoystickSensitivity;
	SerializedProperty leftTouchSensitivity;
	SerializedProperty rightTouchSensitivity;
	SerializedProperty useMovementOrientation;
	SerializedProperty horizontalMovementOrientation;
	SerializedProperty verticalMovementOrientation;
	SerializedProperty useCameraOrientation;
	SerializedProperty horizontalCameraOrientation;
	SerializedProperty verticalCameraOrientation;
	SerializedProperty touchMovementControl;
	SerializedProperty touchCameraControl;
	SerializedProperty touchPanel;
	SerializedProperty usingTouchMovementJoystick;
	SerializedProperty inputLerpSpeedTouchMovementButtons;
	SerializedProperty useHorizontaTouchMovementButtons;
	SerializedProperty useVerticalTouchMovementButtons;
	SerializedProperty horizontalTouchMovementButtons;
	SerializedProperty verticalTouchMovementButtons;
	SerializedProperty inputEnabled;
	SerializedProperty inputCurrentlyActive;
	SerializedProperty overrideInputValuesActive;
	SerializedProperty inputPaused;

	SerializedProperty ignoreAllActionsInput;
	SerializedProperty ignoreAllAxisInput;

	SerializedProperty movementAxis;
	SerializedProperty mouseAxis;
	SerializedProperty rawMovementAxis;
	SerializedProperty usingGamepad;
	SerializedProperty usingKeyBoard;
	SerializedProperty usingTouchControls;
	SerializedProperty usingPlayerMenu;
	SerializedProperty manualControlActive;
	SerializedProperty manualMovementHorizontalInput;
	SerializedProperty manualMovementVerticalInput;
	SerializedProperty manualMouseHorizontalInput;
	SerializedProperty manualMouseVerticalInput;
	SerializedProperty showSingleMultiAxes;
	SerializedProperty currentMultiAxesToShow;
	SerializedProperty multiAxesList;
	SerializedProperty hideSingleMultiAxes;

	SerializedProperty input;
	SerializedProperty playerControllerManager;
	SerializedProperty mainPlayerInputPanelSystem;

	SerializedProperty editInputPanelPrefab;
	SerializedProperty editInputMenu;
	SerializedProperty currentInputPanelListText;

	playerInputManager manager;
	Color defBackgroundColor;
	string[] currentStringList;

	string isEnabled;
	bool currentlyActive;

	string currentButtonMessage;

	bool expandedMultiAxesList = false;
	bool expandedAxesList = false;

	int currentMultiAxesToShowValue;

	SerializedProperty currentSingleMultiAxes;

	int multiAxesStringIndex;
	int axesStringIndex;

	playerInputManager.Axes currentAxes;
	playerInputManager.multiAxes currentMultiAxes;

	int currentAxesArraySize;

	int currentMultiAxesArraySize;

	GUIStyle buttonStyle = new GUIStyle ();

	GUIStyle mainInputListButtonStyle = new GUIStyle ();


	void OnEnable ()
	{
		playerID = serializedObject.FindProperty ("playerID");
		showDebugActions = serializedObject.FindProperty ("showDebugActions");
		useOnlyKeyboard = serializedObject.FindProperty ("useOnlyKeyboard");
		mouseSensitivity = serializedObject.FindProperty ("mouseSensitivity");
		leftGamepadJoystickSensitivity = serializedObject.FindProperty ("leftGamepadJoystickSensitivity");
		rightGamepadJoystickSensitivity = serializedObject.FindProperty ("rightGamepadJoystickSensitivity");
		leftTouchSensitivity = serializedObject.FindProperty ("leftTouchSensitivity");
		rightTouchSensitivity = serializedObject.FindProperty ("rightTouchSensitivity");
		useMovementOrientation = serializedObject.FindProperty ("useMovementOrientation");
		horizontalMovementOrientation = serializedObject.FindProperty ("horizontalMovementOrientation");
		verticalMovementOrientation = serializedObject.FindProperty ("verticalMovementOrientation");
		useCameraOrientation = serializedObject.FindProperty ("useCameraOrientation");
		horizontalCameraOrientation = serializedObject.FindProperty ("horizontalCameraOrientation");
		verticalCameraOrientation = serializedObject.FindProperty ("verticalCameraOrientation");
		touchMovementControl = serializedObject.FindProperty ("touchMovementControl");
		touchCameraControl = serializedObject.FindProperty ("touchCameraControl");
		touchPanel = serializedObject.FindProperty ("touchPanel");
		usingTouchMovementJoystick = serializedObject.FindProperty ("usingTouchMovementJoystick");
		inputLerpSpeedTouchMovementButtons = serializedObject.FindProperty ("inputLerpSpeedTouchMovementButtons");
		useHorizontaTouchMovementButtons = serializedObject.FindProperty ("useHorizontaTouchMovementButtons");
		useVerticalTouchMovementButtons = serializedObject.FindProperty ("useVerticalTouchMovementButtons");
		horizontalTouchMovementButtons = serializedObject.FindProperty ("horizontalTouchMovementButtons");
		verticalTouchMovementButtons = serializedObject.FindProperty ("verticalTouchMovementButtons");
		inputEnabled = serializedObject.FindProperty ("inputEnabled");
		inputCurrentlyActive = serializedObject.FindProperty ("inputCurrentlyActive");
		overrideInputValuesActive = serializedObject.FindProperty ("overrideInputValuesActive");
		inputPaused = serializedObject.FindProperty ("inputPaused");

		ignoreAllActionsInput = serializedObject.FindProperty ("ignoreAllActionsInput");
		ignoreAllAxisInput = serializedObject.FindProperty ("ignoreAllAxisInput");

		movementAxis = serializedObject.FindProperty ("movementAxis");
		mouseAxis = serializedObject.FindProperty ("mouseAxis");
		rawMovementAxis = serializedObject.FindProperty ("rawMovementAxis");
		usingGamepad = serializedObject.FindProperty ("usingGamepad");
		usingKeyBoard = serializedObject.FindProperty ("usingKeyBoard");
		usingTouchControls = serializedObject.FindProperty ("usingTouchControls");
		usingPlayerMenu = serializedObject.FindProperty ("usingPlayerMenu");
		manualControlActive = serializedObject.FindProperty ("manualControlActive");
		manualMovementHorizontalInput = serializedObject.FindProperty ("manualMovementHorizontalInput");
		manualMovementVerticalInput = serializedObject.FindProperty ("manualMovementVerticalInput");
		manualMouseHorizontalInput = serializedObject.FindProperty ("manualMouseHorizontalInput");
		manualMouseVerticalInput = serializedObject.FindProperty ("manualMouseVerticalInput");
		showSingleMultiAxes = serializedObject.FindProperty ("showSingleMultiAxes");
		currentMultiAxesToShow = serializedObject.FindProperty ("currentMultiAxesToShow");
		multiAxesList = serializedObject.FindProperty ("multiAxesList");
		hideSingleMultiAxes = serializedObject.FindProperty ("hideSingleMultiAxes");

		input = serializedObject.FindProperty ("input");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		mainPlayerInputPanelSystem = serializedObject.FindProperty ("mainPlayerInputPanelSystem");

		editInputPanelPrefab = serializedObject.FindProperty ("editInputPanelPrefab");
		editInputMenu = serializedObject.FindProperty ("editInputMenu");
		currentInputPanelListText = serializedObject.FindProperty ("currentInputPanelListText");

		manager = (playerInputManager)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;
	
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (playerID);
		EditorGUILayout.PropertyField (showDebugActions);
		EditorGUILayout.PropertyField (useOnlyKeyboard);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sensitivity Settings", "window");
		EditorGUILayout.PropertyField (mouseSensitivity);
		EditorGUILayout.PropertyField (leftGamepadJoystickSensitivity);
		EditorGUILayout.PropertyField (rightGamepadJoystickSensitivity);
		EditorGUILayout.PropertyField (leftTouchSensitivity);
		EditorGUILayout.PropertyField (rightTouchSensitivity);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Control Orientation Settings", "window");
		EditorGUILayout.PropertyField (useMovementOrientation);
		if (useMovementOrientation.boolValue) {
			EditorGUILayout.PropertyField (horizontalMovementOrientation);
			EditorGUILayout.PropertyField (verticalMovementOrientation);
		}
		EditorGUILayout.PropertyField (useCameraOrientation);
		if (useCameraOrientation.boolValue) {
			EditorGUILayout.PropertyField (horizontalCameraOrientation);
			EditorGUILayout.PropertyField (verticalCameraOrientation);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Controls Settings", "window");
		EditorGUILayout.PropertyField (touchMovementControl);
		EditorGUILayout.PropertyField (touchCameraControl);
		EditorGUILayout.PropertyField (touchPanel);

		EditorGUILayout.PropertyField (usingTouchMovementJoystick);
		EditorGUILayout.PropertyField (inputLerpSpeedTouchMovementButtons);

		EditorGUILayout.PropertyField (useHorizontaTouchMovementButtons);
		EditorGUILayout.PropertyField (useVerticalTouchMovementButtons);
		EditorGUILayout.PropertyField (horizontalTouchMovementButtons);
		EditorGUILayout.PropertyField (verticalTouchMovementButtons);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Input State", "window");
		EditorGUILayout.PropertyField (inputEnabled);
		GUILayout.Label ("Input Active\t\t" + inputCurrentlyActive.boolValue);
		GUILayout.Label ("Override Input Active\t\t" + overrideInputValuesActive.boolValue);
		GUILayout.Label ("Input Paused\t\t" + inputPaused.boolValue);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (movementAxis);
		EditorGUILayout.PropertyField (mouseAxis);
		EditorGUILayout.PropertyField (rawMovementAxis);

		EditorGUILayout.Space ();

		GUILayout.Label ("Using Gamepad\t\t" + usingGamepad.boolValue);
		GUILayout.Label ("Using Keyboard\t\t" + usingKeyBoard.boolValue);
		GUILayout.Label ("Using Touch\t\t" + usingTouchControls.boolValue);
		GUILayout.Label ("Using Player Menu\t\t" + usingPlayerMenu.boolValue);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (ignoreAllActionsInput);
		EditorGUILayout.PropertyField (ignoreAllAxisInput);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Manual Control Settings", "window");
		EditorGUILayout.PropertyField (manualControlActive);
		if (manualControlActive.boolValue) {
			EditorGUILayout.PropertyField (manualMovementHorizontalInput);
			EditorGUILayout.PropertyField (manualMovementVerticalInput);
			EditorGUILayout.PropertyField (manualMouseHorizontalInput);
			EditorGUILayout.PropertyField (manualMouseVerticalInput);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Multi Axes List", "window");

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showSingleMultiAxes.boolValue) {
			GUI.backgroundColor = Color.gray;

			currentButtonMessage = "\n Hide Single Multi Axes \n";
		} else {
			GUI.backgroundColor = defBackgroundColor;

			currentButtonMessage = "\n Show Single Multi Axes \n";
		}
		if (GUILayout.Button (currentButtonMessage)) {
			showSingleMultiAxes.boolValue = !showSingleMultiAxes.boolValue;

			if (showSingleMultiAxes.boolValue) {
				currentMultiAxesToShowValue = currentMultiAxesToShow.intValue;

				currentSingleMultiAxes = multiAxesList.GetArrayElementAtIndex (currentMultiAxesToShowValue);

				hideSingleMultiAxes.boolValue = true;
			}
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		if (showSingleMultiAxes.boolValue) {			
			defBackgroundColor = GUI.backgroundColor;
			EditorGUILayout.BeginHorizontal ();
			if (hideSingleMultiAxes.boolValue) {
				GUI.backgroundColor = Color.gray;

				currentButtonMessage = "Hide Single Axes";
			} else {
				GUI.backgroundColor = defBackgroundColor;

				currentButtonMessage = "Show Single Axes";
			}
			if (GUILayout.Button (currentButtonMessage)) {
				hideSingleMultiAxes.boolValue = !hideSingleMultiAxes.boolValue;
			}
			GUI.backgroundColor = defBackgroundColor;
			EditorGUILayout.EndHorizontal ();

			if (hideSingleMultiAxes.boolValue) {
				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				currentMultiAxesToShowValue = currentMultiAxesToShow.intValue;
		
				if (currentMultiAxesToShowValue == 0) {
					if (GUILayout.Button ("Next List")) {
						currentMultiAxesToShowValue++;
						currentSingleMultiAxes = multiAxesList.GetArrayElementAtIndex (currentMultiAxesToShowValue);
					}
					if (GUILayout.Button (">>")) {
						currentMultiAxesToShowValue = multiAxesList.arraySize - 1;
						currentSingleMultiAxes = multiAxesList.GetArrayElementAtIndex (currentMultiAxesToShowValue);
					}
				} else if (currentMultiAxesToShowValue > 0 && currentMultiAxesToShowValue < manager.multiAxesList.Count - 1) {
					if (GUILayout.Button ("<<")) {
						currentMultiAxesToShowValue = 0;
						currentSingleMultiAxes = multiAxesList.GetArrayElementAtIndex (currentMultiAxesToShowValue);
					}
					if (GUILayout.Button ("Previous List")) {
						currentMultiAxesToShowValue--;
						currentSingleMultiAxes = multiAxesList.GetArrayElementAtIndex (currentMultiAxesToShowValue);
					}

					if (GUILayout.Button ("Next List")) {
						currentMultiAxesToShowValue++;
						currentSingleMultiAxes = multiAxesList.GetArrayElementAtIndex (currentMultiAxesToShowValue);
					}
					if (GUILayout.Button (">>")) {
						currentMultiAxesToShowValue = multiAxesList.arraySize - 1;
						currentSingleMultiAxes = multiAxesList.GetArrayElementAtIndex (currentMultiAxesToShowValue);
					}
				} else if (currentMultiAxesToShowValue == manager.multiAxesList.Count - 1) {
					if (GUILayout.Button ("<<")) {
						currentMultiAxesToShowValue = 0;
						currentSingleMultiAxes = multiAxesList.GetArrayElementAtIndex (currentMultiAxesToShowValue);
					}
					if (GUILayout.Button ("Previous List")) {
						currentMultiAxesToShowValue--;
						currentSingleMultiAxes = multiAxesList.GetArrayElementAtIndex (currentMultiAxesToShowValue);
					}
				}

				currentMultiAxesToShow.intValue = currentMultiAxesToShowValue;
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Update Input List")) {
					if (!Application.isPlaying) {
						manager.updateMultiAxesList ();
					}
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("\nUpdate Current Values (PRESS AFTER CHANGE ANYTHING)\n", buttonStyle)) {
					if (!Application.isPlaying) {
						manager.updateCurrentInputValues ();
					}
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				if (currentSingleMultiAxes == null) {
					currentMultiAxesToShowValue = currentMultiAxesToShow.intValue;

					currentSingleMultiAxes = multiAxesList.GetArrayElementAtIndex (currentMultiAxesToShowValue);
				}

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Expand All")) {
					for (int i = 0; i < multiAxesList.arraySize; i++) {
						multiAxesList.GetArrayElementAtIndex (i).isExpanded = true;
					}
				}
				if (GUILayout.Button ("Collapse All")) {
					for (int i = 0; i < multiAxesList.arraySize; i++) {
						multiAxesList.GetArrayElementAtIndex (i).isExpanded = false;
					}
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				EditorGUILayout.BeginVertical ();

				currentlyActive = currentSingleMultiAxes.FindPropertyRelative ("currentlyActive").boolValue;
				isEnabled = " +";
				if (!currentlyActive) {
					isEnabled = " -";
				}

				isEnabled += " (" + (currentMultiAxesToShowValue + 1) + "/" + multiAxesList.arraySize + ")";
				EditorGUILayout.PropertyField (currentSingleMultiAxes, new GUIContent (currentSingleMultiAxes.displayName + isEnabled));

				if (currentSingleMultiAxes.isExpanded) {
					showSingleMultiAxesListElementInfo (currentSingleMultiAxes, currentMultiAxesToShowValue);
				}

				EditorGUILayout.Space ();

				GUILayout.EndVertical ();

				GUILayout.EndHorizontal ();
			}

		} else {
			showMultiAxesList (multiAxesList);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Element Settings", "window");
		EditorGUILayout.PropertyField (input);
		EditorGUILayout.PropertyField (playerControllerManager);
		EditorGUILayout.PropertyField (mainPlayerInputPanelSystem);
		EditorGUILayout.PropertyField (editInputPanelPrefab);
		EditorGUILayout.PropertyField (editInputMenu);
		EditorGUILayout.PropertyField (currentInputPanelListText);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showSingleMultiAxesListElementInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentlyActive"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canUseInputOnPlayerPaused"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Axes List", "window");
		showAxesList (list.FindPropertyRelative ("axes"), index);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showMultiAxesListElementInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");

		currentMultiAxes = manager.multiAxesList [index];

		currentStringList = currentMultiAxes.multiAxesStringList;

		if (currentStringList.Length > 0) {
			multiAxesStringIndex = currentMultiAxes.multiAxesStringIndex;

			multiAxesStringIndex = EditorGUILayout.Popup ("Axe ", multiAxesStringIndex, currentStringList);

			currentMultiAxes.multiAxesStringIndex = multiAxesStringIndex;

			if (multiAxesStringIndex >= 0) {
				currentMultiAxes.axesName = currentStringList [multiAxesStringIndex];
			} else {
				currentMultiAxes.axesName = "MULTI AXES NAME NO FOUND";
			}
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentlyActive"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canUseInputOnPlayerPaused"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Axes List", "window");
		showAxesList (list.FindPropertyRelative ("axes"), index);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showMultiAxesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		mainInputListButtonStyle = new GUIStyle (GUI.skin.button);

		mainInputListButtonStyle.fontStyle = FontStyle.Bold;
		mainInputListButtonStyle.fontSize = 20;

		if (GUILayout.Button ("Show/Hide Multi Axes List", mainInputListButtonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			currentMultiAxesArraySize = list.arraySize;

			GUILayout.Label ("Number Of Axes: \t" + currentMultiAxesArraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Axes")) {
				if (!Application.isPlaying) {
					manager.addNewAxes ();

					currentMultiAxesArraySize = list.arraySize;
				}
			}
			if (GUILayout.Button ("Clear List")) {
				if (!Application.isPlaying) {
					list.arraySize = 0;

					currentMultiAxesArraySize = list.arraySize;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Axes")) {
				if (!Application.isPlaying) {
					manager.setMultiAxesEnabledState (true);
				}
			}
			if (GUILayout.Button ("Disable All Axes")) {
				if (!Application.isPlaying) {
					manager.setMultiAxesEnabledState (false);
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < currentMultiAxesArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < currentMultiAxesArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Update Input List", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.updateMultiAxesList ();
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("\nUpdate Current Values (PRESS AFTER CHANGE ANYTHING)\n", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.updateCurrentInputValues ();
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < currentMultiAxesArraySize; i++) {
				expandedMultiAxesList = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentMultiAxesArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					currentlyActive = currentArrayElement.FindPropertyRelative ("currentlyActive").boolValue;
					isEnabled = " +";
					if (!currentlyActive) {
						isEnabled = " -";
					}
					EditorGUILayout.PropertyField (currentArrayElement, new GUIContent (currentArrayElement.displayName + isEnabled), false);

					if (currentArrayElement.isExpanded) {
						showMultiAxesListElementInfo (currentArrayElement, i);
						expandedMultiAxesList = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expandedMultiAxesList) {
					GUILayout.BeginVertical ();
					if (GUILayout.Button ("x")) {
						if (!Application.isPlaying) {
							list.DeleteArrayElementAtIndex (i);

							currentMultiAxesArraySize = list.arraySize;
						}
					}
					if (GUILayout.Button ("v")) {
						if (i >= 0) {
							list.MoveArrayElement (i, i + 1);
						}
					}
					if (GUILayout.Button ("^")) {
						if (i < currentMultiAxesArraySize) {
							list.MoveArrayElement (i, i - 1);
						}
					}
					GUILayout.EndVertical ();
				} else {
					GUILayout.BeginHorizontal ();
					if (GUILayout.Button ("x")) {
						if (!Application.isPlaying) {
							list.DeleteArrayElementAtIndex (i);

							currentMultiAxesArraySize = list.arraySize;
						}
					}
					if (GUILayout.Button ("v")) {
						if (i >= 0) {
							list.MoveArrayElement (i, i + 1);
						}
					}
					if (GUILayout.Button ("^")) {
						if (i < currentMultiAxesArraySize) {
							list.MoveArrayElement (i, i - 1);
						}
					}
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();
			if (GUILayout.Button ("\nUpdate Current Values (PRESS AFTER CHANGE ANYTHING)\n", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.updateCurrentInputValues ();
				}
			}
		}
		GUILayout.EndVertical ();
	}

	void showAxesListElementInfo (SerializedProperty list, int multiAxesIndex, int axesIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionName"));

		currentMultiAxes = manager.multiAxesList [multiAxesIndex];

		currentAxes = currentMultiAxes.axes [axesIndex];

		currentStringList = currentAxes.axesStringList;

		if (currentStringList.Length > 0) {
			currentAxes.axesStringIndex = EditorGUILayout.Popup ("Axe ", currentAxes.axesStringIndex, currentStringList);

			axesStringIndex = currentAxes.axesStringIndex;

			if (axesStringIndex >= 0 && axesIndex < currentMultiAxes.axes.Count && axesStringIndex < currentStringList.Length) {
				currentAxes.Name = currentStringList [axesStringIndex];
			} else {
				currentAxes.Name = "AXES NAME NO FOUND";
			}
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buttonPressType"));

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsedOnPausedGame"));

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("buttonEvent"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Axes Action Name")) {
			if (!Application.isPlaying) {
				manager.setAxesActionName (multiAxesIndex, axesIndex);
			}
		}
		GUILayout.EndVertical ();
	}

	void showAxesList (SerializedProperty list, int multiAxesIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Axes List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			currentAxesArraySize = list.arraySize;

			GUILayout.Label ("Number Of Actions: \t" + currentAxesArraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Action")) {
				if (!Application.isPlaying) {
					manager.addNewAction (multiAxesIndex);

					currentAxesArraySize = list.arraySize;
				}
			}
			if (GUILayout.Button ("Clear List")) {
				if (!Application.isPlaying) {
					list.arraySize = 0;

					currentAxesArraySize = list.arraySize;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < currentAxesArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < currentAxesArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Actions")) {
				if (!Application.isPlaying) {
					manager.setAllActionsEnabledState (multiAxesIndex, true);
				}
			}
			if (GUILayout.Button ("Disable All Actions")) {
				if (!Application.isPlaying) {
					manager.setAllActionsEnabledState (multiAxesIndex, false);
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Update Input List")) {
				if (!Application.isPlaying) {
					manager.updateAxesList (multiAxesIndex);
				}
			}

			if (currentAxesArraySize == 0) {
				if (GUILayout.Button ("Set All Actions")) {
					if (!Application.isPlaying) {
						manager.setAllAxesList (multiAxesIndex);
					}
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < currentAxesArraySize; i++) {
				expandedAxesList = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentAxesArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						showAxesListElementInfo (currentArrayElement, multiAxesIndex, i);
						expandedAxesList = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expandedAxesList) {
					GUILayout.BeginVertical ();
					if (GUILayout.Button ("x")) {
						if (!Application.isPlaying) {
							list.DeleteArrayElementAtIndex (i);

							currentAxesArraySize = list.arraySize;
						}
					}
					if (GUILayout.Button ("v")) {
						if (i >= 0) {
							list.MoveArrayElement (i, i + 1);
						}
					}
					if (GUILayout.Button ("^")) {
						if (i < currentAxesArraySize) {
							list.MoveArrayElement (i, i - 1);
						}
					}
					GUILayout.EndVertical ();
				} else {
					GUILayout.BeginHorizontal ();
					if (GUILayout.Button ("x")) {
						if (!Application.isPlaying) {
							list.DeleteArrayElementAtIndex (i);

							currentAxesArraySize = list.arraySize;
						}
					}
					if (GUILayout.Button ("v")) {
						if (i >= 0) {
							list.MoveArrayElement (i, i + 1);
						}
					}
					if (GUILayout.Button ("^")) {
						if (i < currentAxesArraySize) {
							list.MoveArrayElement (i, i - 1);
						}
					}
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}
}
#endif