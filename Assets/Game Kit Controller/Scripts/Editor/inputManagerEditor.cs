using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(inputManager))]
public class inputManagerEditor : Editor
{
	SerializedProperty showActionKeysOnMenu;
	SerializedProperty editInputPanelPrefab;
	SerializedProperty editInputMenu;
	SerializedProperty currentInputPanelListText;
	SerializedProperty charactersManager;
	SerializedProperty pauseManager;
	SerializedProperty mainGameManager;
	SerializedProperty checkInputKeyDuplicatedOnRebindAction;
	SerializedProperty checkInputKeyDuplicatedOnlyOnSameInputCategory;

	SerializedProperty buttonsDisabledAtStart;
	SerializedProperty touchButtonList;
	SerializedProperty touchButtonsInfoList;

	SerializedProperty buttonsToIgnoreSetVisibleStateList;

	SerializedProperty playerInputManagerList;
	SerializedProperty ignoreGamepad;
	SerializedProperty allowKeyboardAndGamepad;
	SerializedProperty allowGamepadInTouchDevice;
	SerializedProperty checkConnectedGamepadRate;
	SerializedProperty showKeyboardPressed;
	SerializedProperty showKeyboardPressedAction;
	SerializedProperty showGamepadPressed;
	SerializedProperty showGamepadPressedAction;
	SerializedProperty showPressedKeyWhenEditingInput;
	SerializedProperty showDebugKeyPressed;
	SerializedProperty usingGamepad;
	SerializedProperty onlyOnePlayer;
	SerializedProperty usingKeyBoard;
	SerializedProperty numberOfGamepads;
	SerializedProperty lastButtonPressedOnGamepad;
	SerializedProperty lastKeyPressedOnKeyboard;
	SerializedProperty useTouchControls;
	SerializedProperty editingInput;
	SerializedProperty loadOption;
	SerializedProperty createRebindInputMenuWithCurrentAxes;
	SerializedProperty rebindInputMenuPanelsAmount;
	SerializedProperty rebindInputMenuActionsPerPanelAmount;
	SerializedProperty multiAxesList;
	SerializedProperty gamepadList;
	SerializedProperty currentInputActionList;
	SerializedProperty actionKeyToFilter;
	SerializedProperty joystickButtonToFilter;

	SerializedProperty showMissingInputWarningMessagesEnabled;

	inputManager manager;
	string controlScheme;
	bool checkState;

	string gamepadActive;
	bool actionEnabled;
	string isEnabled;

	string buttonMessage;

	Color defBackgroundColor;

	bool showCurentInputActionList;

	int touchButtonPanelIndex;
	int touchButtonIndex;

	string[] currentStringList;

	inputManager.Axes currentAxes;

	string currentStringValue;
	string temporalStringValue;

	GUIStyle buttonStyle = new GUIStyle ();

	GUIStyle windowTextStyle = new GUIStyle ();

	GUIStyle mainInputListButtonStyle = new GUIStyle ();

	void OnEnable ()
	{
		showActionKeysOnMenu = serializedObject.FindProperty ("showActionKeysOnMenu");
		editInputPanelPrefab = serializedObject.FindProperty ("editInputPanelPrefab");
		editInputMenu = serializedObject.FindProperty ("editInputMenu");
		currentInputPanelListText = serializedObject.FindProperty ("currentInputPanelListText");
		charactersManager = serializedObject.FindProperty ("charactersManager");
		pauseManager = serializedObject.FindProperty ("pauseManager");
		mainGameManager = serializedObject.FindProperty ("mainGameManager");
		checkInputKeyDuplicatedOnRebindAction = serializedObject.FindProperty ("checkInputKeyDuplicatedOnRebindAction");
		checkInputKeyDuplicatedOnlyOnSameInputCategory = serializedObject.FindProperty ("checkInputKeyDuplicatedOnlyOnSameInputCategory");

		buttonsDisabledAtStart = serializedObject.FindProperty ("buttonsDisabledAtStart");
		touchButtonList = serializedObject.FindProperty ("touchButtonList");
		touchButtonsInfoList = serializedObject.FindProperty ("touchButtonsInfoList");

		buttonsToIgnoreSetVisibleStateList = serializedObject.FindProperty ("buttonsToIgnoreSetVisibleStateList");

		playerInputManagerList = serializedObject.FindProperty ("playerInputManagerList");
		ignoreGamepad = serializedObject.FindProperty ("ignoreGamepad");
		allowKeyboardAndGamepad = serializedObject.FindProperty ("allowKeyboardAndGamepad");
		allowGamepadInTouchDevice = serializedObject.FindProperty ("allowGamepadInTouchDevice");
		checkConnectedGamepadRate = serializedObject.FindProperty ("checkConnectedGamepadRate");
		showKeyboardPressed = serializedObject.FindProperty ("showKeyboardPressed");
		showKeyboardPressedAction = serializedObject.FindProperty ("showKeyboardPressedAction");
		showGamepadPressed = serializedObject.FindProperty ("showGamepadPressed");
		showGamepadPressedAction = serializedObject.FindProperty ("showGamepadPressedAction");
		showPressedKeyWhenEditingInput = serializedObject.FindProperty ("showPressedKeyWhenEditingInput");
		showDebugKeyPressed = serializedObject.FindProperty ("showDebugKeyPressed");
		usingGamepad = serializedObject.FindProperty ("usingGamepad");
		onlyOnePlayer = serializedObject.FindProperty ("onlyOnePlayer");
		usingKeyBoard = serializedObject.FindProperty ("usingKeyBoard");
		numberOfGamepads = serializedObject.FindProperty ("numberOfGamepads");
		lastButtonPressedOnGamepad = serializedObject.FindProperty ("lastButtonPressedOnGamepad");
		lastKeyPressedOnKeyboard = serializedObject.FindProperty ("lastKeyPressedOnKeyboard");
		useTouchControls = serializedObject.FindProperty ("useTouchControls");
		editingInput = serializedObject.FindProperty ("editingInput");
		loadOption = serializedObject.FindProperty ("loadOption");
		createRebindInputMenuWithCurrentAxes = serializedObject.FindProperty ("createRebindInputMenuWithCurrentAxes");
		rebindInputMenuPanelsAmount = serializedObject.FindProperty ("rebindInputMenuPanelsAmount");
		rebindInputMenuActionsPerPanelAmount = serializedObject.FindProperty ("rebindInputMenuActionsPerPanelAmount");
		multiAxesList = serializedObject.FindProperty ("multiAxesList");
		gamepadList = serializedObject.FindProperty ("gamepadList");
		currentInputActionList = serializedObject.FindProperty ("currentInputActionList");
		actionKeyToFilter = serializedObject.FindProperty ("actionKeyToFilter");
		joystickButtonToFilter = serializedObject.FindProperty ("joystickButtonToFilter");

		showMissingInputWarningMessagesEnabled = serializedObject.FindProperty ("showMissingInputWarningMessagesEnabled");

		manager = (inputManager)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 16;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (showActionKeysOnMenu);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gamepad Settings", "window");
		EditorGUILayout.PropertyField (ignoreGamepad);
		EditorGUILayout.PropertyField (allowKeyboardAndGamepad);
		EditorGUILayout.PropertyField (allowGamepadInTouchDevice);
		EditorGUILayout.PropertyField (checkConnectedGamepadRate);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Settings", "window");
		EditorGUILayout.PropertyField (showMissingInputWarningMessagesEnabled);
		EditorGUILayout.PropertyField (showKeyboardPressed);
		EditorGUILayout.PropertyField (showKeyboardPressedAction);
		EditorGUILayout.PropertyField (showGamepadPressed);
		EditorGUILayout.PropertyField (showGamepadPressedAction);
		EditorGUILayout.PropertyField (showPressedKeyWhenEditingInput);

		EditorGUILayout.PropertyField (showDebugKeyPressed);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Input State (DEBUG)", "window");
		EditorGUILayout.PropertyField (usingGamepad);
		EditorGUILayout.PropertyField (onlyOnePlayer);
		EditorGUILayout.PropertyField (usingKeyBoard);
		EditorGUILayout.PropertyField (numberOfGamepads);
		EditorGUILayout.PropertyField (lastButtonPressedOnGamepad);
		EditorGUILayout.PropertyField (lastKeyPressedOnKeyboard);
		EditorGUILayout.PropertyField (useTouchControls);
		EditorGUILayout.PropertyField (editingInput);
		EditorGUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Rebing Settings", "window");
		EditorGUILayout.PropertyField (checkInputKeyDuplicatedOnRebindAction);
		EditorGUILayout.PropertyField (checkInputKeyDuplicatedOnlyOnSameInputCategory);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Save/Load Settings", "window");
		EditorGUILayout.PropertyField (loadOption);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		windowTextStyle.fontStyle = FontStyle.Bold;
		windowTextStyle.fontSize = 25;
		windowTextStyle.alignment = TextAnchor.MiddleCenter;

		GUILayout.Label ("Multi Input List", windowTextStyle);

//		GUILayout.BeginVertical (windowTextStyle);
		EditorGUILayout.Space ();

		showMultiAxesList (multiAxesList);
//		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//check the current controls enabled
		if (!checkState) {
			if (useTouchControls.boolValue) {
				controlScheme = "Mobile";
			} else {
				controlScheme = "Keyboard";
			}
			checkState = true;
		}

		GUILayout.Label ("Input Scheme Options", windowTextStyle);
		//set the axes list in the inspector to the default value
		if (GUILayout.Button ("Save Current Input As Default")) {
			manager.saveCurrentInputAsDefault ();
		}

		if (GUILayout.Button ("Load Default Input")) {
			manager.setCurrentInputToDefault ();
		}

		//save the axes list in the inspector in a file
		if (GUILayout.Button ("Save Input To File")) {
			manager.saveButtonsInputToSaveFile ();
		}

		//set the axes list in the inspector to the values stored in a file
		if (GUILayout.Button ("Load Input From File")) {
			manager.loadButtonsInspectorFromSaveFile ();
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//show the controls scheme
		GUILayout.Label ("CURRENT CONTROLS: " + controlScheme, windowTextStyle);
		//set the keyboard controls
		if (GUILayout.Button ("Set Keyboard Controls")) {
			manager.setKeyboardControlsFromEditor (true);
			controlScheme = "Keyboard";
		}

		//set the touch controls
		if (GUILayout.Button ("Set Touch Controls")) {
			manager.setKeyboardControlsFromEditor (false);
			controlScheme = "Mobile";
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Input Manager List", "window");
		showSimpleList (playerInputManagerList, "Player");
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Input Elements", "window");
		EditorGUILayout.PropertyField (editInputPanelPrefab);
		EditorGUILayout.PropertyField (editInputMenu);
		EditorGUILayout.PropertyField (currentInputPanelListText);
		EditorGUILayout.PropertyField (charactersManager);
		EditorGUILayout.PropertyField (pauseManager);
		EditorGUILayout.PropertyField (mainGameManager);	
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Controls Settings", "window");

		GUILayout.BeginVertical ("Touch Buttons Disabled At Start", "window");
		showButtonsToDisableAtStartList (buttonsDisabledAtStart);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Buttons Ignores To Set Visible State", "window");
		showButtonsToIgnoreSetVisibleStateList (buttonsToIgnoreSetVisibleStateList);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get Touch Button List")) {
			manager.getTouchButtonListString ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Select Main Touch Button Panel On Editor")) {
			manager.selectMainTouchButtonPanelOnEditor ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Button List", "window");

		EditorGUILayout.Space ();

		showTouchButtonList (touchButtonList);

		EditorGUILayout.Space ();

		showTouchButtonsInfoList (touchButtonsInfoList);

		EditorGUILayout.Space ();

		EditorGUILayout.EndVertical ();

		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Rebind Input Menu Options", "window");
		EditorGUILayout.PropertyField (createRebindInputMenuWithCurrentAxes);
		EditorGUILayout.PropertyField (rebindInputMenuPanelsAmount);
		EditorGUILayout.PropertyField (rebindInputMenuActionsPerPanelAmount);
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Instantiate Rebind Menus And Actions Panels")) {
			manager.instantiateRebindMenusAndActionsPanels (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Destroy Rebind Menus And Actions Panels")) {
			manager.destroyRebindMenusAndActionsPanels ();
		}
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		gamepadActive = "NO";
		if (usingGamepad.boolValue) {
			gamepadActive = "YES";
		}

		GUILayout.BeginVertical ("Gamepad State (Debug)", "window");
		GUILayout.Label ("Using Gamepad: " + gamepadActive);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gamepad List", "window");
		showGamepadList (gamepadList);
		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Show Current Input Action List Settings Filter", "window");

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showCurentInputActionList) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Current Input Action List Settings";
		} else {
			GUI.backgroundColor = defBackgroundColor;
			buttonMessage = "Show Current Input Action List Settings";
		}
		if (GUILayout.Button (buttonMessage, buttonStyle)) {
			showCurentInputActionList = !showCurentInputActionList;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		if (showCurentInputActionList) {

			showCurrentInputActionListButtons ();

			EditorGUILayout.Space ();

			if (currentInputActionList.arraySize > 0) {
				for (int i = 0; i < currentInputActionList.arraySize; i++) {
					

					GUILayout.Label (currentInputActionList.GetArrayElementAtIndex (i).stringValue);
				}

//				currentStringValue = currentInputActionList.stringValue;

//				if (currentStringValue.Length > 1000) {
//
//					int stringLength = currentStringValue.Length;
//
//					int stringLengthLoopAmount = (stringLength / 1000) + 1;
//
//					int currentLineNumber = 0;
//
//
//					for (int i = 0; i < stringLengthLoopAmount; i++) {
//						currentLineNumber++;
//
//						int startIndex = i * 1000;
//						int endIndex = 999;
//
//						int remainingCharacters = stringLength - (currentLineNumber * 1000);
//
//						Debug.Log (startIndex + " " + stringLength + " " + endIndex + " " + currentStringValue.Length + " " + remainingCharacters);
//							
//						if (startIndex < stringLength && remainingCharacters > 1000) {
//							temporalStringValue = currentStringValue.Substring (startIndex, endIndex);
//						} else {
//							endIndex = 1000 - Mathf.Abs (stringLength - (currentLineNumber * 1000));
//							temporalStringValue = currentStringValue.Substring (startIndex, endIndex);
//						}
//
//						GUILayout.Label (temporalStringValue);
//					}
//				} else {
//					GUILayout.Label (currentInputActionList.stringValue);
//				}

				showCurrentInputActionListButtons ();
			}
		}
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showCurrentInputActionListButtons ()
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show Full Current Input Action List")) {
			manager.showCurrentInputActionList (false, false);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show Only Keys Input Action List")) {
			manager.showCurrentInputActionList (true, false);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show Only Joysticks Input Action List")) {
			manager.showCurrentInputActionList (false, true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Clear Keys Input Action List Text")) {
			manager.clearInputActionListText ();
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (actionKeyToFilter);
		if (GUILayout.Button ("Show Input Actions Keys By Filter")) {
			manager.showCurrentInputActionListByKeyFilter ();
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (joystickButtonToFilter);
		if (GUILayout.Button ("Show Input Actions Joystick By Filter")) {
			manager.showCurrentInputActionListByJoystickFilter ();
		}
	}

	void showListElementInfo (SerializedProperty list, int multiAxesIndex, int axesIndex)
	{
		EditorGUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionEnabled"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showActionInRebindPanel"));

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Keyboard Input", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("key"));
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Joystick Input", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("joystickButton"));
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Button Input", "window");

		if (manager.touchButtonsStringInfoList.Count > 0) {

			currentStringList = manager.touchButtonListString;

			if (currentStringList.Length > 0) {

				currentAxes = manager.multiAxesList [multiAxesIndex].axes [axesIndex];

				touchButtonPanelIndex = currentAxes.touchButtonPanelIndex;

				touchButtonPanelIndex = EditorGUILayout.Popup ("Touch Panel", touchButtonPanelIndex, currentStringList);

				currentAxes.touchButtonPanelIndex = touchButtonPanelIndex;

				if (touchButtonPanelIndex >= 0) {
					if (touchButtonPanelIndex > 0) {
						if (touchButtonPanelIndex < currentStringList.Length) {
							currentAxes.touchButtonPanel = currentStringList [touchButtonPanelIndex];
			
							touchButtonIndex = currentAxes.touchButtonIndex;

							if (manager.touchButtonsStringInfoList.Count > touchButtonPanelIndex) {
								currentStringList = manager.touchButtonsStringInfoList [touchButtonPanelIndex].touchButtonListString;

								touchButtonIndex = EditorGUILayout.Popup ("Touch Button", touchButtonIndex, currentStringList);
			
								currentAxes.touchButtonIndex = touchButtonIndex;

								if (touchButtonIndex >= 0) {
									if (touchButtonIndex < currentStringList.Length) {
										currentAxes.touchButtonName = currentStringList [touchButtonIndex];
									}
								} else {
									currentAxes.touchButtonName = "TOUCH BUTTON NAME NO FOUND";
								}
							}
						}
					}
				} else {
					currentAxes.touchButtonPanel = "TOUCH PANEL NAME NO FOUND";
				}
			}
		}

		EditorGUILayout.EndVertical ();
	
		EditorGUILayout.EndVertical ();
	}

	void showMultiListElementInfo (SerializedProperty list, int axeListIndex)
	{
		GUILayout.BeginVertical ("Axes", "window");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Axes List Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("axesName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentlyActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("multiAxesEditPanelActive"));
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Input List", "window");
		showAxesList (list.FindPropertyRelative ("axes"), axeListIndex);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.EndVertical ();
	}

	void showAxesList (SerializedProperty list, int axeListIndex)
	{
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Axes List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			int currentArraySize = list.arraySize;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Actions: \t" + currentArraySize);

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add Action")) {
				manager.addNewAxe (axeListIndex);

				currentArraySize = list.arraySize;
			}

			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;

				currentArraySize = list.arraySize;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				bool expanded = false;
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					actionEnabled = currentArrayElement.FindPropertyRelative ("actionEnabled").boolValue;
					isEnabled = " +";
					if (!actionEnabled) {
						isEnabled = " -";
					}
					EditorGUILayout.PropertyField (currentArrayElement, new GUIContent (currentArrayElement.displayName + isEnabled), false);

					if (currentArrayElement.isExpanded) {
						expanded = true;
						showListElementInfo (currentArrayElement, axeListIndex, i);
					}

					EditorGUILayout.Space ();

					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndHorizontal ();
				if (expanded) {
					EditorGUILayout.BeginVertical ();
				} else {
					EditorGUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					manager.removeAxesElement (axeListIndex, i);

					currentArraySize = list.arraySize;
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < currentArraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					EditorGUILayout.EndVertical ();
				} else {
					EditorGUILayout.EndHorizontal ();
				}
				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.EndVertical ();
	}

	void showMultiAxesList (SerializedProperty list)
	{
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		mainInputListButtonStyle = new GUIStyle (GUI.skin.button);

		mainInputListButtonStyle.fontStyle = FontStyle.Bold;
		mainInputListButtonStyle.fontSize = 20;

		if (GUILayout.Button ("Show/Hide Multi Axes List", mainInputListButtonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			int currentArraySize = list.arraySize;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Axes: \t" + currentArraySize);

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add Axes List")) {
				manager.addNewAxesList ();

				currentArraySize = list.arraySize;
			}

			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;

				currentArraySize = list.arraySize;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Input Inspector")) {
				manager.updateInputInspector ();
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				bool expanded = false;
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, new GUIContent (currentArrayElement.displayName), false);

					if (currentArrayElement.isExpanded) {
						expanded = true;
						showMultiListElementInfo (currentArrayElement, i);
					}

					EditorGUILayout.Space ();

					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndHorizontal ();
				if (expanded) {
					EditorGUILayout.BeginVertical ();
				} else {
					EditorGUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					manager.removeAxesList (i);

					currentArraySize = list.arraySize;
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < currentArraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					EditorGUILayout.EndVertical ();
				} else {
					EditorGUILayout.EndHorizontal ();
				}
				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.EndVertical ();
	}

	void showButtonsToDisableAtStartList (SerializedProperty list)
	{
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Buttons To Disable At Start List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			int currentArraySize = list.arraySize;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Actions: \t" + currentArraySize);

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add Button")) {
				list.arraySize++;

				currentArraySize = list.arraySize;
			}

			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;

				currentArraySize = list.arraySize;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, new GUIContent (currentArrayElement.displayName), false);
						
					EditorGUILayout.Space ();

					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);

					currentArraySize = list.arraySize;
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Player Input Manager List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			int currentArraySize = list.arraySize;

			EditorGUILayout.Space ();

			GUILayout.Label (listName + "s: \t" + currentArraySize);

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add " + listName)) {
				list.arraySize++;

				currentArraySize = list.arraySize;
			}

			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;

				currentArraySize = list.arraySize;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, new GUIContent (currentArrayElement.displayName), false);

					EditorGUILayout.Space ();

					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);

					currentArraySize = list.arraySize;
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.EndVertical ();
	}

	void showTouchButtonsInfoListElementButton (SerializedProperty list, int touchPanelIndex)
	{
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Touch Buttons Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			int currentArraySize = list.arraySize;

			EditorGUILayout.Space ();

			GUILayout.Label ("Touch Button List \t" + currentArraySize);

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, new GUIContent (currentArrayElement.displayName), false);

					EditorGUILayout.Space ();

					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					manager.removeTouchButton (touchPanelIndex, i);

					currentArraySize = list.arraySize;

				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.EndVertical ();
	}

	void showTouchButtonList (SerializedProperty list)
	{
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Touch Button List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			int currentArraySize = list.arraySize;

			EditorGUILayout.Space ();

			GUILayout.Label ("Touch Button List: \t" + currentArraySize);

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, new GUIContent (currentArrayElement.displayName), false);

					EditorGUILayout.Space ();

					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.EndVertical ();
	}


	void showTouchButtonsInfoListElement (SerializedProperty list, int touchPanelIndex)
	{
		EditorGUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.Space ();

		showTouchButtonsInfoListElementButton (list.FindPropertyRelative ("touchButtonList"), touchPanelIndex);
		EditorGUILayout.EndVertical ();
	}

	void showTouchButtonsInfoList (SerializedProperty list)
	{
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Touch Buttons Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			int currentArraySize = list.arraySize;

			GUILayout.Label ("Number Of Panels: \t" + currentArraySize);

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Panel")) {
				list.arraySize++;

				currentArraySize = list.arraySize;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;

				currentArraySize = list.arraySize;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				bool expanded = false;
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);

					if (currentArrayElement.isExpanded) {
						showTouchButtonsInfoListElement (currentArrayElement, i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndHorizontal ();
				if (expanded) {
					EditorGUILayout.BeginVertical ();
					if (GUILayout.Button ("x")) {
						manager.removeTouchPanel (i);

						currentArraySize = list.arraySize;
					}
					if (GUILayout.Button ("v")) {
						if (i >= 0) {
							list.MoveArrayElement (i, i + 1);
						}
					}
					if (GUILayout.Button ("^")) {
						if (i < currentArraySize) {
							list.MoveArrayElement (i, i - 1);
						}
					}
					EditorGUILayout.EndVertical ();
				} else {
					EditorGUILayout.BeginHorizontal ();
					if (GUILayout.Button ("x")) {
						manager.removeTouchPanel (i);

						currentArraySize = list.arraySize;
					}
					if (GUILayout.Button ("v")) {
						if (i >= 0) {
							list.MoveArrayElement (i, i + 1);
						}
					}
					if (GUILayout.Button ("^")) {
						if (i < currentArraySize) {
							list.MoveArrayElement (i, i - 1);
						}
					}
					EditorGUILayout.EndHorizontal ();
				}
				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.EndVertical ();
	}

	void showButtonsToIgnoreSetVisibleStateList (SerializedProperty list)
	{
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Buttons To Ignore List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			int currentArraySize = list.arraySize;

			GUILayout.Label ("Number Of Buttons: \t" + currentArraySize);

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Button")) {
				list.arraySize++;

				currentArraySize = list.arraySize;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;

				currentArraySize = list.arraySize;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				EditorGUILayout.BeginHorizontal ("box");

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);

					EditorGUILayout.EndVertical ();
				}

				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);

					currentArraySize = list.arraySize;
				}

				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.EndVertical ();
	}


	void showGamepadList (SerializedProperty list)
	{
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Gamepad List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			int currentArraySize = list.arraySize;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Gamepads: \t" + currentArraySize);
		
			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
	
					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);

					if (currentArrayElement.isExpanded) {
						showGamepadListElement (currentArrayElement);
					}

					EditorGUILayout.Space ();

					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndHorizontal ();
			
				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.EndVertical ();
	}

	void showGamepadListElement (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("gamepadNumber"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("gamepadName"));
	}
}
#endif