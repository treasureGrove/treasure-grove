using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerInputManager : MonoBehaviour
{
	public List<multiAxes> multiAxesList = new List<multiAxes> ();

	public int playerID;
	public bool inputEnabled = true;

	public bool showDebugActions;

	public bool inputCurrentlyActive = true;

	public inputManager input;
	public playerController playerControllerManager;

	public bool ignoreGamepad;
	public bool allowKeyboardAndGamepad;
	public bool allowGamepadInTouchDevice;

	public touchJoystick touchMovementControl;
	public touchJoystick touchCameraControl;
	[Range (0, 10)] public float leftTouchSensitivity;
	[Range (0, 10)] public float rightTouchSensitivity;
	[Range (0, 10)] public float mouseSensitivity;

	[Range (0, 10)] public float leftGamepadJoystickSensitivity = 1;
	[Range (0, 10)] public float rightGamepadJoystickSensitivity = 1;

	public GameObject touchPanel;

	public bool usingTouchMovementJoystick = true;

	public bool useHorizontaTouchMovementButtons;
	public bool useVerticalTouchMovementButtons;

	public GameObject horizontalTouchMovementButtons;
	public GameObject verticalTouchMovementButtons;
	public float inputLerpSpeedTouchMovementButtons = 5;

	public bool pressingTouchHorizontalRightInput;
	public bool pressingTouchHorizontalLeftInput;

	public bool pressingTouchVerticalUpInput;
	public bool pressingTouchVerticalDownInput;

	public bool useOnlyKeyboard;

	public bool usingGamepad;
	public bool usingKeyBoard;
	public bool usingTouchControls;
	public bool gameManagerPaused;

	public string horizontalString = "Horizontal";
	public string verticalString = "Vertical";
	public string mouseXString = "Mouse X";
	public string mouseYString = "Mouse Y";
	public string horizontalXString = "Horizontal X";
	public string verticalYString = "Vertical Y";

	multiAxes currentMultiAxes;
	Axes currentAxes;

	public Vector2 movementAxis;
	public Vector2 mouseAxis;
	public Vector2 rawMovementAxis;

	public bool useMovementOrientation;
	[Range (-1, 1)] public int horizontalMovementOrientation = 1;
	[Range (-1, 1)] public int verticalMovementOrientation = 1;
	public bool useCameraOrientation;
	[Range (-1, 1)] public int horizontalCameraOrientation = 1;
	[Range (-1, 1)] public int verticalCameraOrientation = 1;

	public bool manualControlActive;
	[Range (-1, 1)] public float manualMovementHorizontalInput;
	[Range (-1, 1)] public float manualMovementVerticalInput;
	[Range (-1, 1)] public float manualMouseHorizontalInput;
	[Range (-1, 1)] public float manualMouseVerticalInput;

	public bool inputPaused;

	public bool ignoreAllActionsInput;
	public bool ignoreAllAxisInput;

	public bool showSingleMultiAxes;
	public int currentMultiAxesToShow;
	public bool hideSingleMultiAxes;

	public playerInputPanelSystem mainPlayerInputPanelSystem;

	public List<inputCurrentlyActiveInfo> inputCurrentlyActiveInfoList = new List<inputCurrentlyActiveInfo> ();

	public GameObject editInputPanelPrefab;
	public GameObject editInputMenu;
	public GameObject currentInputPanelListText;

	string playerIDString;

	string mouseXPlayerIDString;
	string mouseYPlayerIDString;
	string horizontalXPlayerIDString;
	string vertircalYPlayerIDString;

	bool keyboardActive;

	public bool overrideInputValuesActive;
	Vector2 overrideInputAxisValue;

	Vector2 auxMovementAxis;
	Vector2 auxRawMovementAxis;

	bool playerIsDead;

	bool originalUsingTouchMovementJoystickValue;

	bool inputPausedForExternalComponents;

	bool ignoreOverrideInput;

	bool avoidInputActive;

	int MAIndex;
	int AIndex;

	int currentMultiAxesCount;
	int currentAxesCount;

	Vector2 vector2Zero = Vector2.zero;

	public bool ignorePlayerActionsInput;

	bool ignorePlayerIsDeadOnCameraAxis;

	bool lastButtonPressedOnGamepad;
	bool lastKeyPressedOnKeyboard;

	Vector2 temporalRawMovementAxis;
	
	public bool axisInputPaused;

	void Awake ()
	{
		findInputManager ();
	}

	void Start ()
	{
		if (useOnlyKeyboard) {
			playerControllerManager.setPlayerID (0);
		}

		playerID = playerControllerManager.getPlayerID ();

		switchPlayerID (playerID);
	
		originalUsingTouchMovementJoystickValue = usingTouchMovementJoystick;

		if (input != null) {
			usingTouchControls = input.isUsingTouchControls ();
		}

		currentMultiAxesCount = multiAxesList.Count;
	}

	void Update ()
	{
		playerIsDead = playerControllerManager.isPlayerDead ();

		if (inputEnabled && !ignorePlayerActionsInput) {
			gameManagerPaused = input.isGameManagerPaused ();

			if (!inputPaused && !ignoreAllActionsInput) {
				for (MAIndex = 0; MAIndex < currentMultiAxesCount; MAIndex++) {

					currentMultiAxes = multiAxesList [MAIndex];

					if (!avoidInputActive || currentMultiAxes.canUseInputOnPlayerPaused) {
						if (currentMultiAxes.currentlyActive) {
							currentAxesCount = currentMultiAxes.axesCount;

							for (AIndex = 0; AIndex < currentAxesCount; AIndex++) {

								currentAxes = currentMultiAxes.axes [AIndex];

								if (currentAxes.actionEnabled) {
									if (inputCurrentlyActive || currentAxes.canBeUsedOnPausedGame) {
										if (input.checkPlayerInputButtonFromMultiAxesList (currentMultiAxes.multiAxesStringIndex, currentAxes.axesStringIndex, 
											    currentAxes.buttonPressType, playerID, currentAxes.canBeUsedOnPausedGame, useOnlyKeyboard)) {

											if (showDebugActions) {
												print (currentAxes.Name);
											}

											currentAxes.buttonEvent.Invoke ();
										}
									}
								}
							}
						}
					}
				}
			}

			//convert the input from keyboard or a touch screen into values to move the player, given the camera direction
			//also, it checks in the player is using a device, like a vehicle
			//convert the mouse input in the tilt angle for the camera or the input from the touch screen depending of the settings
			//also, it checks in the player is using a device, like a vehicle

			usingTouchControls = input.isUsingTouchControls ();

			usingKeyBoard = input.isUsingKeyBoard ();

			allowKeyboardAndGamepad = input.isAllowKeyboardAndGamepad ();

			allowGamepadInTouchDevice = input.isAllowGamepadInTouchDevice ();

			usingGamepad = input.isUsingGamepad ();

			if (usingKeyBoard || allowKeyboardAndGamepad || useOnlyKeyboard) {
				if (!usingTouchControls || (allowGamepadInTouchDevice && usingGamepad)) {
					movementAxis.x = Input.GetAxis (horizontalString);
					movementAxis.y = Input.GetAxis (verticalString);

					mouseAxis.x = Input.GetAxis (mouseXString);
					mouseAxis.y = Input.GetAxis (mouseYString);

					mouseAxis *= mouseSensitivity;

					rawMovementAxis.x = Input.GetAxisRaw (horizontalString);
					rawMovementAxis.y = Input.GetAxisRaw (verticalString);

					keyboardActive = true;

					if (!lastKeyPressedOnKeyboard && rawMovementAxis != vector2Zero) {
//						print ("pressing keyboard");

						GKC_Utils.eventOnPressingKeyboardInput (playerID);

						lastKeyPressedOnKeyboard = true;

						lastButtonPressedOnGamepad = false;

						input.checkButtonPressedOnGamepadOrKeyboard (true);
					}
				} else {
					if (usingTouchMovementJoystick) {
						movementAxis = touchMovementControl.GetAxis () * leftTouchSensitivity;
					} else {
						float movementHorizontalValue = 0;
						float movementVerticalValue = 0;

						if (pressingTouchHorizontalRightInput) {
							movementHorizontalValue = 1;
						}

						if (pressingTouchHorizontalLeftInput) {
							movementHorizontalValue = -1;
						}

						if ((pressingTouchHorizontalRightInput && pressingTouchHorizontalLeftInput) || (!pressingTouchHorizontalRightInput && !pressingTouchHorizontalLeftInput)) {
							movementHorizontalValue = 0;
						}

						if (pressingTouchVerticalUpInput) {
							movementVerticalValue = 1;
						}

						if (pressingTouchVerticalDownInput) {
							movementVerticalValue = -1;
						}

						if ((pressingTouchVerticalUpInput && pressingTouchVerticalDownInput) || (!pressingTouchVerticalUpInput && !pressingTouchVerticalDownInput)) {
							movementVerticalValue = 0;
						}

						movementAxis = Vector2.MoveTowards (movementAxis, new Vector2 (movementHorizontalValue, movementVerticalValue) * leftTouchSensitivity, 
							Time.deltaTime * inputLerpSpeedTouchMovementButtons);
					}


					mouseAxis = touchCameraControl.GetAxis () * rightTouchSensitivity;					

					if (usingTouchMovementJoystick) {
						rawMovementAxis = touchMovementControl.getRawAxis ();
					} else {
						if (movementAxis.x > 0) {
							rawMovementAxis.x = 1;
						} else if (movementAxis.x < 0) {
							rawMovementAxis.x = -1;
						} else {
							rawMovementAxis.x = 0;
						}

						if (movementAxis.y > 0) {
							rawMovementAxis.y = 1;
						} else if (movementAxis.y < 0) {
							rawMovementAxis.y = -1;
						} else {
							rawMovementAxis.y = 0;
						}
					}

					keyboardActive = false;
				}
			} else {
				keyboardActive = false;
			}

			if ((!usingKeyBoard || allowKeyboardAndGamepad) && !useOnlyKeyboard) {
				if (usingGamepad) {
					if (!keyboardActive) {
						movementAxis = vector2Zero;
						mouseAxis = vector2Zero;
						rawMovementAxis = vector2Zero;
					}

					movementAxis.x += Input.GetAxis (horizontalXPlayerIDString);
					movementAxis.y += Input.GetAxis (vertircalYPlayerIDString);

					movementAxis *= leftGamepadJoystickSensitivity;

					mouseAxis.x += Input.GetAxis (mouseXPlayerIDString);
					mouseAxis.y += Input.GetAxis (mouseYPlayerIDString);

					mouseAxis *= rightGamepadJoystickSensitivity;

					if (allowKeyboardAndGamepad && usingKeyBoard) {
						temporalRawMovementAxis = rawMovementAxis;
					} 
						
					rawMovementAxis.x = Input.GetAxisRaw (horizontalXPlayerIDString);
					rawMovementAxis.y = Input.GetAxisRaw (vertircalYPlayerIDString);

					if (!lastButtonPressedOnGamepad && rawMovementAxis != vector2Zero) {
//						print ("pressing gamepad");

						GKC_Utils.eventOnPressingGamepadInput (playerID);

						lastButtonPressedOnGamepad = true;

						lastKeyPressedOnKeyboard = false;

						input.checkButtonPressedOnGamepadOrKeyboard (false);
					}

					if (allowKeyboardAndGamepad && usingKeyBoard && temporalRawMovementAxis != vector2Zero) {
						rawMovementAxis = temporalRawMovementAxis;
					} 
				}
			}

			if (useMovementOrientation) {
				if (!isAnyMenuActive ()) {
					if (horizontalMovementOrientation == -1) {
						movementAxis.x *= horizontalMovementOrientation;
						rawMovementAxis.x *= horizontalMovementOrientation;
					}

					if (verticalMovementOrientation == -1) {
						movementAxis.y *= verticalMovementOrientation;
						rawMovementAxis.y *= verticalMovementOrientation;
					}
				}
			}
				
			if (useCameraOrientation) {
				if (!isAnyMenuActive ()) {
					if (horizontalCameraOrientation == -1) {
						mouseAxis.x *= horizontalCameraOrientation;
					}

					if (verticalCameraOrientation == -1) {
						mouseAxis.y *= verticalCameraOrientation;
					}
				}
			}
		}

		if (ignoreOverrideInput) {
			return;
		}

		if (overrideInputValuesActive) {

			auxMovementAxis = movementAxis;
			auxRawMovementAxis = rawMovementAxis;

			movementAxis = overrideInputAxisValue;

			if (movementAxis.x > 0) {
				rawMovementAxis.x = 1;
			} else if (movementAxis.x < 0) {
				rawMovementAxis.x = -1;
			} else {
				rawMovementAxis.x = 0;
			}

			if (movementAxis.y > 0) {
				rawMovementAxis.y = 1;
			} else if (movementAxis.y < 0) {
				rawMovementAxis.y = -1;
			} else {
				rawMovementAxis.y = 0;
			}
		}

		if (manualControlActive) {
			auxMovementAxis = movementAxis;
			auxRawMovementAxis = rawMovementAxis;

			movementAxis = new Vector2 (manualMovementHorizontalInput, manualMovementVerticalInput);

			if (movementAxis.x > 0) {
				rawMovementAxis.x = 1;
			} else if (movementAxis.x < 0) {
				rawMovementAxis.x = -1;
			} else {
				rawMovementAxis.x = 0;
			}

			if (movementAxis.y > 0) {
				rawMovementAxis.y = 1;
			} else if (movementAxis.y < 0) {
				rawMovementAxis.y = -1;
			} else {
				rawMovementAxis.y = 0;
			}

			mouseAxis.x = manualMouseHorizontalInput;
			mouseAxis.y = manualMouseVerticalInput;
		}
		
		if (axisInputPaused) {
			movementAxis = vector2Zero;
			mouseAxis = vector2Zero;
			rawMovementAxis = vector2Zero;
		}
	}

	public bool isAnyMenuActive ()
	{
		if (gameManagerPaused || playerControllerManager.isPlayerMenuActive () || playerControllerManager.isUsingSubMenu ()) {
			return true;
		}

		return false;
	}

	public Vector2 getAuxMovementAxis ()
	{
		return auxMovementAxis;
	}

	public Vector2 getAuxRawMovementAxis ()
	{
		return auxRawMovementAxis;
	}

	//get if the touch controls are enabled, so any other component can check it
	public void changeControlsType (bool state)
	{
		enableOrDisableTouchControls (state);

		enableOrDisableScreenActionParent (!state);
	}

	public void enableOrDisableTouchControls (bool state)
	{
		if (touchPanel.activeSelf != state) {
			touchPanel.SetActive (state);
		}

		enableOrDisableTouchCameraControl (state);

		enableOrDisableTouchMovementJoystickForButtons (state);
	}

	public void enableOrDisableTouchMovementControl (bool state)
	{
		if (touchMovementControl.gameObject.activeSelf != state) {
			touchMovementControl.gameObject.SetActive (state);
		}
	}

	public void enableOrDisableTouchCameraControl (bool state)
	{
		if (touchCameraControl.gameObject.activeSelf != state) {
			touchCameraControl.gameObject.SetActive (state);
		}
	}

	public void enableOrDisableTouchMovementJoystickForButtons (bool state)
	{
		if (usingTouchMovementJoystick) {
			enableOrDisableTouchMovementControl (state);

			if (horizontalTouchMovementButtons.activeSelf) {
				horizontalTouchMovementButtons.SetActive (false);
			}

			if (verticalTouchMovementButtons.activeSelf) {
				verticalTouchMovementButtons.SetActive (false);
			}
		} else {

			enableOrDisableTouchMovementControl (false);

			if (useHorizontaTouchMovementButtons) {
				if (horizontalTouchMovementButtons.activeSelf != state) {
					horizontalTouchMovementButtons.SetActive (state);
				}
			} else {
				if (horizontalTouchMovementButtons.activeSelf) {
					horizontalTouchMovementButtons.SetActive (false);
				}
			}

			if (useVerticalTouchMovementButtons) {
				if (verticalTouchMovementButtons.activeSelf != state) {
					verticalTouchMovementButtons.SetActive (state);
				}
			} else {
				if (verticalTouchMovementButtons.activeSelf) {
					verticalTouchMovementButtons.SetActive (false);
				}
			}
		}
	}

	public void setJoystickColorVisibleState (bool state)
	{
		touchMovementControl.setJoystickColorVisibleState (state);
		touchCameraControl.setJoystickColorVisibleState (state);
	}

	public void setOriginalTouchMovementInputState ()
	{
		setUsingTouchMovementJoystickState (originalUsingTouchMovementJoystickValue);

		enableOrDisableTouchMovementJoystickForButtons (true);
	}

	public void setUsingTouchMovementJoystickState (bool state)
	{
		usingTouchMovementJoystick = state;
	}

	public Vector2 getPlayerRawMovementAxis ()
	{
		if (!inputEnabled) {
			return vector2Zero;
		}

		if (inputPaused) {
			return vector2Zero;
		}

		if (ignoreAllAxisInput) {
			return vector2Zero;
		}

		if (playerIsDead) {
			return vector2Zero;
		}
			
		return rawMovementAxis;
	}

	public Vector2 getPlayerRawMovementAxisWithoutCheckingEnabled ()
	{
		if (inputPaused) {
			return vector2Zero;
		}

		if (ignoreAllAxisInput) {
			return vector2Zero;
		}

		if (playerIsDead) {
			return vector2Zero;
		}

		return rawMovementAxis;
	}

	//get the current values of the axis keys or the mouse in the input manager
	public Vector2 getPlayerMovementAxis ()
	{
		if (!inputEnabled) {
			return vector2Zero;
		}

		if (inputPaused) {
			return vector2Zero;
		}

		if (ignoreAllAxisInput) {
			return vector2Zero;
		}

		if (playerIsDead) {
			return vector2Zero;
		}
			
		return movementAxis;
	}

	public Vector2 getPlayerMovementAxisWithoutCheckingEnabled ()
	{
		if (inputPaused) {
			return vector2Zero;
		}

		if (playerIsDead) {
			return vector2Zero;
		}

		return movementAxis;
	}

	public Vector2 getPlayerMouseAxis ()
	{
		if (!inputEnabled) {
			return vector2Zero;
		}

		if (inputPaused) {
			return vector2Zero;
		}

		if (ignoreAllAxisInput) {
			return vector2Zero;
		}

		if (playerIsDead && !ignorePlayerIsDeadOnCameraAxis) {
			return vector2Zero;
		}
			
		return mouseAxis;
	}

	public Vector2 getRealMovementAxisInput ()
	{
		return new Vector2 (Input.GetAxis (horizontalString), Input.GetAxis (verticalString));
	}

	public Vector2 getRealMouseAxisInput ()
	{
		return new Vector2 (Input.GetAxis (mouseXString), Input.GetAxis (mouseYString));
	}

	public Vector2 getRealMouseRawAxisInput ()
	{
		return new Vector2 (Input.GetAxisRaw (mouseXString), Input.GetAxisRaw (mouseYString));
	}

	public Vector2 getRealMouseGamepadAxisInput ()
	{
		return new Vector2 (Input.GetAxis (mouseXPlayerIDString), Input.GetAxis (mouseYPlayerIDString));
	}

	public Vector2 getRealMovementGamepadAxisInput ()
	{
		return new Vector2 (Input.GetAxis (horizontalXPlayerIDString), Input.GetAxis (vertircalYPlayerIDString));
	}

	public Vector2 getRealMovementAxisInputAnyType ()
	{
		if (usingTouchControls) {
			return touchMovementControl.GetAxis ();
		} else {
			if (usingGamepad) {
				if (allowKeyboardAndGamepad) {
					return getRealMovementGamepadAxisInput () + getRealMovementAxisInput ();
				} else {
					return getRealMovementGamepadAxisInput ();
				}
			} else {
				return getRealMovementAxisInput ();
			}
		}
	}

	public bool checkPlayerInputButtonFromMultiAxesList (int multiAxesIndex, int axesIndex, inputManager.buttonType type, bool canBeUsedOnPausedGame)
	{
		if (!inputEnabled) {
			return false;
		}

		if (inputPaused) {
			return false;
		}

		if (ignoreAllActionsInput) {
			return false;
		}

		if (playerIsDead) {
			return false;
		}
			
		return input.checkPlayerInputButtonFromMultiAxesList (multiAxesIndex, axesIndex, type, playerID, canBeUsedOnPausedGame, useOnlyKeyboard);
	}

	public bool checkPlayerInputButtonWithoutEvents (int multiAxesStringIndex, int axesStringIndex, inputManager.buttonType buttonType)
	{
		if (!inputEnabled) {
			return false;
		}

		if (inputPaused) {
			return false;
		}

		if (ignoreAllActionsInput) {
			return false;
		}

		if (playerIsDead) {
			return false;
		}

		return input.checkPlayerInputButtonFromMultiAxesList (multiAxesStringIndex, axesStringIndex, buttonType, playerID, false, useOnlyKeyboard);
	}

	string[] keyNumberListString = new string []{ "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

	public int checkNumberInput (int numberAmount)
	{
		if (numberAmount > 10) {
			numberAmount = 10;
		}

		if (Input.anyKeyDown) {
			for (int i = 0; i < numberAmount; i++) {
				if (Input.GetKeyDown (keyNumberListString [i])) {
					return i;
				}
			}
		}

		return -1;
	}

	public void setPlayerInputEnabledState (bool state)
	{
		inputEnabled = state;
	}

	public void setIgnorePlayerActionsInputState (bool state)
	{
		ignorePlayerActionsInput = state;
	}

	Coroutine enablePlayerInputCoroutine;

	public void setPlayerInputEnabledStateWithDelay (bool state)
	{
		if (state) {
			if (enablePlayerInputCoroutine != null) {
				StopCoroutine (enablePlayerInputCoroutine);
			}
			enablePlayerInputCoroutine = StartCoroutine (setPlayerInputEnabledStateWithDelayCoroutine ());

		} else {
			inputEnabled = false;
		}
	}

	IEnumerator setPlayerInputEnabledStateWithDelayCoroutine ()
	{
		yield return new WaitForSeconds (0.3f);

		inputEnabled = true;
	}

	public void setInputCurrentlyActiveState (bool state)
	{
		inputCurrentlyActive = state;
	}

	public void toggleInputCurrentlyActiveState ()
	{
		setInputCurrentlyActiveState (!inputCurrentlyActive);
	}

	public void setPlayerID (int newID)
	{
		playerID = newID;
	}

	public int getPlayerID ()
	{
		return playerID;
	}

	public void switchPlayerID (int newID)
	{
		playerID = newID;
		playerIDString = playerID.ToString ();

		horizontalXPlayerIDString = horizontalXString + playerIDString;
		vertircalYPlayerIDString = verticalYString + playerIDString;

		mouseXPlayerIDString = mouseXString + playerIDString;
		mouseYPlayerIDString = mouseYString + playerIDString;
	}

	public void setPlayerInputActionState (bool playerInputActionState, string multiAxesInputName, string axesInputName)
	{
		int multiAxesListCount = multiAxesList.Count;

		for (int i = 0; i < multiAxesListCount; i++) {
			multiAxes temporalMultiAxes = multiAxesList [i];

			if (temporalMultiAxes.axesName.Equals (multiAxesInputName)) {

				int axesCount = temporalMultiAxes.axes.Count;

				for (int j = 0; j < axesCount; j++) {
					if (temporalMultiAxes.axes [j].Name.Equals (axesInputName)) {
						temporalMultiAxes.axes [j].actionEnabled = playerInputActionState;
					}
				}
			}
		}
	}

	public bool getPlayerInputMultiAxesState (string multiAxesInputName)
	{
		int multiAxesListCount = multiAxesList.Count;

		for (int i = 0; i < multiAxesListCount; i++) {
			multiAxes temporalMultiAxes = multiAxesList [i];

			if (temporalMultiAxes.axesName.Equals (multiAxesInputName)) {
				return temporalMultiAxes.currentlyActive;
			}
		}

		return false;
	}

	public void updateCurrentActiveInputList ()
	{
		if (playerControllerManager.checkIfPlayerDeadFromHealthComponent ()) {
			return;
		}

		int multiAxesListCount = multiAxesList.Count;

		if (inputCurrentlyActiveInfoList.Count == 0) {
			for (int i = 0; i < multiAxesListCount; i++) {
				inputCurrentlyActiveInfo newInputCurrentlyActiveInfo = new inputCurrentlyActiveInfo ();

				multiAxes temporalMultiAxes = multiAxesList [i];

				newInputCurrentlyActiveInfo.inputName = temporalMultiAxes.axesName;

				newInputCurrentlyActiveInfo.currentState = temporalMultiAxes.currentlyActive;

				inputCurrentlyActiveInfoList.Add (newInputCurrentlyActiveInfo);
			}
		}

		for (int i = 0; i < multiAxesListCount; i++) {
			inputCurrentlyActiveInfoList [i].currentState = multiAxesList [i].currentlyActive;
		}

//		for (int i = 0; i < multiAxesList.Count; i++) {
//			print (inputCurrentlyActiveInfoList [i].inputName + " " + inputCurrentlyActiveInfoList [i].currentState);
//		}
//
//		print ("UPDATE INPUT---------------------------------------------------------------------");
	}

	public void setCurrentActiveInputList ()
	{
//		print ("SET ACTIVE INPUT------------------------------------------------------------");
		int multiAxesListCount = multiAxesList.Count;

		for (int i = 0; i < multiAxesListCount; i++) {
			multiAxesList [i].currentlyActive = inputCurrentlyActiveInfoList [i].currentState;

//			print (multiAxesList [i].axesName + " " + multiAxesList [i].currentlyActive);
		}
	}

	public void setPlayerInputMultiAxesState (bool playerInputMultiAxesState, string multiAxesInputName)
	{
		int multiAxesListCount = multiAxesList.Count;

		for (int i = 0; i < multiAxesListCount; i++) {
			multiAxes temporalMultiAxes = multiAxesList [i];

			if (temporalMultiAxes.axesName.Equals (multiAxesInputName)) {
				temporalMultiAxes.currentlyActive = playerInputMultiAxesState;

				return;
			}
		}
	}

	public bool setPlayerInputMultiAxesStateAndGetPreviousState (bool playerInputMultiAxesState, string multiAxesInputName)
	{
		bool previousState = false;

		int multiAxesListCount = multiAxesList.Count;

		for (int i = 0; i < multiAxesListCount; i++) {
			multiAxes temporalMultiAxes = multiAxesList [i];

			if (temporalMultiAxes.axesName.Equals (multiAxesInputName)) {
				previousState = temporalMultiAxes.currentlyActive;

				temporalMultiAxes.currentlyActive = playerInputMultiAxesState;

				return previousState;
			}
		}

		return false;
	}

	public void enablePlayerInputMultiAxes (string multiAxesInputName)
	{
		int multiAxesListCount = multiAxesList.Count;

		for (int i = 0; i < multiAxesListCount; i++) {
			multiAxes temporalMultiAxes = multiAxesList [i];

			if (temporalMultiAxes.axesName.Equals (multiAxesInputName)) {
				temporalMultiAxes.currentlyActive = true;

				return;
			}
		}
	}

	public void disablePlayerInputMultiAxes (string multiAxesInputName)
	{
		int multiAxesListCount = multiAxesList.Count;

		for (int i = 0; i < multiAxesListCount; i++) {
			multiAxes temporalMultiAxes = multiAxesList [i];

			if (temporalMultiAxes.axesName.Equals (multiAxesInputName)) {
				temporalMultiAxes.currentlyActive = false;

				return;
			}
		}
	}

	public void overrideInputValues (Vector2 newInputValues, bool overrideState)
	{
		overrideInputAxisValue = newInputValues;

		overrideInputValuesActive = overrideState;
	}

	public void setManuaInputValues (Vector2 newMovementInputValues, Vector2 newMouseInputValues)
	{
		manualControlActive = true;

		manualMovementHorizontalInput = newMovementInputValues.x;

		manualMovementVerticalInput = newMovementInputValues.y;

		manualMouseHorizontalInput = newMouseInputValues.x;

		manualMouseVerticalInput = newMouseInputValues.y;
	}

	public void disableManuaInput ()
	{
		manualControlActive = false;
	}

	public void setIgnoreOverrideInputState (bool state)
	{
		ignoreOverrideInput = state;
	}

	public inputManager getInputManager ()
	{
		return input;
	}

	public bool isUsingTouchControls ()
	{
		return input.isUsingTouchControls ();
	}

	public bool isUsingTouchControlsOnGameManager ()
	{
		return input.isUsingTouchControlsOnGameManager ();
	}

	public string getButtonKey (string buttonName)
	{
		return input.getButtonKey (buttonName);
	}

	public void setAvoidInputActiveState (bool state)
	{
		avoidInputActive = state;
	}

	public GameObject getTouchPanel ()
	{
		return touchPanel;
	}

	public void setTouchPanelActiveState (bool state)
	{
		if (touchPanel.activeSelf != state) {
			touchPanel.SetActive (state);
		}
	}

	public GameObject getEditInputPanelPrefab ()
	{
		return editInputPanelPrefab;
	}

	public GameObject getEditInputMenu ()
	{
		return editInputMenu;
	}

	public GameObject getCurrentInputPanelListText ()
	{
		return currentInputPanelListText;
	}

	//Start of functions to enable or disable the reverse direction of movement and camera input
	public void setHorizontalMovementOrientationValue (int newValue)
	{
		horizontalMovementOrientation = newValue;
	}

	public void setVerticalMovementOrientationValue (int newValue)
	{
		verticalMovementOrientation = newValue;
	}

	public void setHorizontalCameraOrientationValue (int newValue)
	{
		horizontalCameraOrientation = newValue;
	}

	public void setVerticalCameratOrientationValue (int newValue)
	{
		verticalCameraOrientation = newValue;
	}

	public void setHorizontalMovementOrientationValue (bool state)
	{
		setHorizontalMovementOrientationValue (state ? 1 : -1);
	}

	public void setVerticalMovementOrientationValue (bool state)
	{
		setVerticalMovementOrientationValue (state ? 1 : -1);
	}

	public void setHorizontalCameraOrientationValue (bool state)
	{
		setHorizontalCameraOrientationValue (state ? 1 : -1);
	}

	public void setVerticalCameratOrientationValue (bool state)
	{
		setVerticalCameratOrientationValue (state ? 1 : -1);
	}
	//End of functions to enable or disable the reverse direction of movement and camera input


	public void setPressingTouchHorizontalRightInputState (bool state)
	{
		pressingTouchHorizontalRightInput = state;
	}

	public void setPressingTouchHorizontalLeftInputState (bool state)
	{
		pressingTouchHorizontalLeftInput = state;
	}

	public void setPressingTouchVerticalUpInputState (bool state)
	{
		pressingTouchVerticalUpInput = state;
	}

	public void setPressingTouchVerticalDownInputState (bool state)
	{
		pressingTouchVerticalDownInput = state;
	}

	public void pauseOrResumeInput (bool state)
	{
		inputPaused = state;
	}

	public void setAxisInputPausedState (bool state)
	{
		axisInputPaused = state;
	}

	public void toggleAxisInputPausedState ()
	{
		setAxisInputPausedState (!axisInputPaused);
	}

	public void setIgnorePlayerIsDeadOnCameraAxisState (bool state)
	{
		ignorePlayerIsDeadOnCameraAxis = state;
	}

	public void setInputPausedForExternalComponentsState (bool state)
	{
		inputPausedForExternalComponents = state;
	}

	public bool isInputPausedForExternalComponents ()
	{
		return inputPausedForExternalComponents;
	}

	public List<touchPanelsInfo> getTouchPanelsList ()
	{
		return mainPlayerInputPanelSystem.getTouchPanelsList ();
	}

	public void enableTouchPanelByName (string panelName)
	{
		enableOrDisableTouchPanelByName (panelName, true);
	}

	public void disableTouchPanelByName (string panelName)
	{
		enableOrDisableTouchPanelByName (panelName, false);
	}

	public void enableOrDisableTouchPanelByName (string panelName, bool state)
	{
		mainPlayerInputPanelSystem.enableOrDisableTouchPanelByName (panelName, state);
	}

	public void enableTouchPanelSchemeByName (string panelName)
	{
		mainPlayerInputPanelSystem.enableTouchPanelSchemeByName (panelName);
	}

	public void enablePreviousTouchPanelScheme ()
	{
		mainPlayerInputPanelSystem.enablePreviousTouchPanelScheme ();
	}

	public void enableOrDisableActionScreen (string actionScreenName)
	{
		mainPlayerInputPanelSystem.enableOrDisableActionScreen (actionScreenName);
	}

	public void enableOrDisableActionScreen (string actionScreenName, bool state)
	{
		mainPlayerInputPanelSystem.enableOrDisableActionScreen (actionScreenName, state);
	}

	public void enableActionScreen (string actionScreenName)
	{
		mainPlayerInputPanelSystem.enableActionScreen (actionScreenName);
	}

	public void disableActionScreen (string actionScreenName)
	{
		mainPlayerInputPanelSystem.disableActionScreen (actionScreenName);
	}

	public void enablePreviousActionScreen ()
	{
		mainPlayerInputPanelSystem.enablePreviousActionScreen ();
	}

	public void enableSingleActionScreen (string actionScreenName)
	{
		mainPlayerInputPanelSystem.enableSingleActionScreen (actionScreenName);
	}

	public void enableOrDisableScreenActionParent (bool state)
	{
		mainPlayerInputPanelSystem.enableOrDisableScreenActionParent (state);
	}

	public void checkPanelsActiveOnGamepadOrKeyboard (bool checkKeyboard)
	{
		mainPlayerInputPanelSystem.checkPanelsActiveOnGamepadOrKeyboard (checkKeyboard);

		checkButtonPressedOnGamepadOrKeyboard (checkKeyboard);
	}

	public bool usingPlayerMenu;

	public void setUsingPlayerMenuState (bool state)
	{
		usingPlayerMenu = state;
	}

	public bool isUsingPlayerMenu ()
	{
		return usingPlayerMenu;
	}

	//get the values from the touch joystick sensitivity in the touch options menu when the player adjust the joysticks sensitivity
	public void setRightSensitivityValue (float sensitivityValue)
	{
		//set the values in the input manager
		rightTouchSensitivity = sensitivityValue;
	}

	public void setLeftSensitivityValue (float sensitivityValue)
	{
		//set the values in the input manager
		leftTouchSensitivity = sensitivityValue;
	}

	//get the mouse sensitivity value when the player adjusts it in the edit input menu
	public void setMouseSensitivityValue (float sensitivityValue)
	{
		//set the values in the input manager
		mouseSensitivity = sensitivityValue;
	}

	public void setLeftGamepadJoystickSensitivityValue (float sensitivityValue)
	{
		leftGamepadJoystickSensitivity = sensitivityValue;
	}

	public void setRightGamepadJoystickSensitivityValue (float sensitivityValue)
	{
		rightGamepadJoystickSensitivity = sensitivityValue;
	}

	public bool isKeyboardButtonPressed (string multiAxesName, string axesName)
	{
		return input.isKeyboardButtonPressed (playerID, multiAxesName, axesName);
	}

	public bool isUsingGamepad ()
	{
		return input.isUsingGamepad ();
	}

	public bool isPlayerDead ()
	{
		return playerIsDead;
	}

	public bool isInputPaused ()
	{
		return inputPaused;
	}

	public bool isGameManagerPaused ()
	{
		return input.isGameManagerPaused ();
	}

	public bool areTouchJoysticksPressed ()
	{
		return touchMovementControl.touchJoystickIsPressed () || touchCameraControl.touchJoystickIsPressed ();
	}

	public void checkButtonPressedOnGamepadOrKeyboard (bool checkKeyboard)
	{
		if (checkKeyboard) {
			lastButtonPressedOnGamepad = false;
			lastKeyPressedOnKeyboard = true;
		} else {
			lastButtonPressedOnGamepad = true;
			lastKeyPressedOnKeyboard = false;
		}
	}


	//Input management through the custom inspector
	public void addNewAxes ()
	{
		findInputManager ();

		if (input != null) {
			multiAxes newMultiAxes = new multiAxes ();

			newMultiAxes.multiAxesStringList = new string[input.multiAxesList.Count];

			for (int i = 0; i < input.multiAxesList.Count; i++) {
				string axesName = input.multiAxesList [i].axesName;
				newMultiAxes.multiAxesStringList [i] = axesName;
			}

			multiAxesList.Add (newMultiAxes);

			updateComponent ();
		}
	}

	public void addNewAction (int multiAxesIndex)
	{
		findInputManager ();

		if (input != null) {
			multiAxes currentMultiAxesList = multiAxesList [multiAxesIndex];

			int multiAxesStringIndex = currentMultiAxesList.multiAxesStringIndex;

			List<inputManager.Axes> currentTemporalAxes = input.multiAxesList [multiAxesStringIndex].axes;

			Axes newAxes = new Axes ();
			newAxes.axesStringList = new string[currentTemporalAxes.Count];

			for (int i = 0; i < currentTemporalAxes.Count; i++) {
				string actionName = currentTemporalAxes [i].Name;
				newAxes.axesStringList [i] = actionName;
			}

			newAxes.multiAxesStringIndex = multiAxesIndex;

			currentMultiAxesList.axes.Add (newAxes);

			currentMultiAxesList.axesCount = currentMultiAxesList.axes.Count;

			updateComponent ();
		}
	}

	public void setAxesActionName (int multiAxesIndex, int axesIndex)
	{
		if (multiAxesList.Count > multiAxesIndex && multiAxesList [multiAxesIndex].axes.Count > axesIndex) {
			Axes currentAxes = multiAxesList [multiAxesIndex].axes [axesIndex];

			int currentAxesStringIndex = currentAxes.axesStringIndex;

			print (currentAxesStringIndex);

			if (currentAxesStringIndex > -1) {
				currentAxes.actionName = currentAxes.axesStringList [currentAxesStringIndex];

				updateComponent ();
			} else {
				print ("WARNING: Action no properly configured, make sure the Multi Axes exist on the Input Manager and inside of it," +
				"there is an action configured for this current element");
			}
		}
	}

	public void updateMultiAxesList ()
	{
		findInputManager ();

		if (input != null) {
			for (int i = 0; i < multiAxesList.Count; i++) {

				multiAxes currentMultiAxesList = multiAxesList [i];

				List<inputManager.multiAxes> currentTemporalMultiAxes = input.multiAxesList;

				currentMultiAxesList.multiAxesStringList = new string[currentTemporalMultiAxes.Count];

				int currentMultiAxesIndex = -1;

				string currentMultiAxesName = currentMultiAxesList.axesName;

				for (int j = 0; j < currentTemporalMultiAxes.Count; j++) {
					string axesName = currentTemporalMultiAxes [j].axesName;
					currentMultiAxesList.multiAxesStringList [j] = axesName;

					if (currentMultiAxesName.Equals (axesName)) {
						currentMultiAxesIndex = j;
					}
				}

				if (currentMultiAxesIndex > -1) {
					if (currentMultiAxesIndex != currentMultiAxesList.multiAxesStringIndex) {
						currentMultiAxesList.multiAxesStringIndex = currentMultiAxesIndex;

						print (currentMultiAxesName.ToUpper () + " updated with index " + currentMultiAxesIndex);
					} else {
						print (currentMultiAxesName.ToUpper () + " keeps the same index " + currentMultiAxesIndex);
					}
				} else {
					print ("WARNING: Multi axes called " + currentMultiAxesName.ToUpper () + " hasn't been found, make sure to configure a multi axes " +
					"with that name in the main input manager");

					currentMultiAxesList.multiAxesStringIndex = -1;
				}

				currentMultiAxesList.axesCount = currentMultiAxesList.axes.Count;
			}

			updateComponent ();
		}
	}

	public void updateAxesList (int multiAxesListIndex)
	{
		findInputManager ();

		if (input != null) {
			multiAxes currentMultiAxesList = multiAxesList [multiAxesListIndex];

			for (int i = 0; i < currentMultiAxesList.axes.Count; i++) {

				Axes currentAxes = currentMultiAxesList.axes [i];

				int multiAxesStringIndex = currentMultiAxesList.multiAxesStringIndex;

				List<inputManager.Axes> currentTemporalAxes = input.multiAxesList [multiAxesStringIndex].axes;

				currentAxes.axesStringList = new string[currentTemporalAxes.Count];

				int currentAxesIndex = -1;

				string currentAxesName = currentAxes.Name;

				for (int j = 0; j < currentTemporalAxes.Count; j++) {
					string actionName = currentTemporalAxes [j].Name;
					currentAxes.axesStringList [j] = actionName;

					if (currentAxesName.Equals (actionName)) {
						currentAxesIndex = j;
					}
				}

				if (currentAxesIndex > -1) {
					if (currentAxesIndex != currentAxes.axesStringIndex) {
						currentAxes.axesStringIndex = currentAxesIndex;

						print (currentAxes.actionName.ToUpper () + " updated with index " + currentAxesIndex);
					} else {
						print (currentAxes.actionName.ToUpper () + " keeps the same index " + currentAxesIndex);
					}
				} else {
					print ("WARNING: Action called " + currentAxesName.ToUpper () + " hasn't been found, make sure to configure an action " +
					"with that name in the main input manager");

					currentAxes.axesStringIndex = -1;
				}

				currentMultiAxesList.axesCount = currentMultiAxesList.axes.Count;
			}

			updateComponent ();
		}
	}

	public void setAllAxesList (int multiAxesListIndex)
	{
		if (input != null) {
			multiAxes currentMultiAxesList = multiAxesList [multiAxesListIndex];

			currentMultiAxesList.axes.Clear ();

			int multiAxesStringIndex = currentMultiAxesList.multiAxesStringIndex;

			List<inputManager.Axes> currentTemporalAxes = input.multiAxesList [multiAxesStringIndex].axes;

			for (int i = 0; i < currentTemporalAxes.Count; i++) {
				Axes newAxes = new Axes ();
				newAxes.axesStringList = new string[currentTemporalAxes.Count];

				for (int j = 0; j < currentTemporalAxes.Count; j++) {
					string actionName = currentTemporalAxes [j].Name;
					newAxes.axesStringList [j] = actionName;
				}

				newAxes.multiAxesStringIndex = multiAxesListIndex;

				newAxes.axesStringIndex = i;

				newAxes.Name = currentTemporalAxes [i].Name;

				newAxes.actionName = newAxes.Name;

				currentMultiAxesList.axes.Add (newAxes);

				currentMultiAxesList.axesCount = currentMultiAxesList.axes.Count;
			}

			updateComponent ();
		}
	}

	void findInputManager ()
	{
		if (input == null) {
			input = FindObjectOfType<inputManager> ();
		}
	}

	public void updateCurrentInputValues ()
	{
		print ("Player Input Manager settings updated");

		for (int i = 0; i < multiAxesList.Count; i++) {
			multiAxesList [i].axesCount = multiAxesList [i].axes.Count;
		}

		updateComponent ();
	}

	public void setMultiAxesEnabledState (bool state)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			multiAxesList [i].currentlyActive = state;
		}

		updateComponent ();
	}

	public void setAllActionsEnabledState (int multiAxesListIndex, bool state)
	{
		for (int j = 0; j < multiAxesList [multiAxesListIndex].axes.Count; j++) {
			multiAxesList [multiAxesListIndex].axes [j].actionEnabled = state;
		}

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Player Input Manager Values", gameObject);
	}

	[System.Serializable]
	public class multiAxes
	{
		public string axesName;
		public List<Axes> axes = new List<Axes> ();

		public int axesCount;

		public bool currentlyActive = true;
		public bool canUseInputOnPlayerPaused;

		public int multiAxesStringIndex;
		public string[] multiAxesStringList;

		public multiAxes (multiAxes newState)
		{
			axesName = newState.axesName;
		}

		public multiAxes ()
		{
		}
	}

	[System.Serializable]
	public class Axes
	{
		public string actionName = "New Action";
		public string Name;
		public bool actionEnabled = true;

		public bool canBeUsedOnPausedGame;

		public inputManager.buttonType buttonPressType;

		public UnityEvent buttonEvent;

		public int axesStringIndex;
		public string[] axesStringList;

		public int multiAxesStringIndex;
	}

	[System.Serializable]
	public class inputCurrentlyActiveInfo
	{
		public string inputName;
		public bool currentState;
	}
}
