using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class mouseCursorController : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useMouseCursorEnabled = true;

	public bool useSmoothCursorSpeedOnGame = true;
	public float cursorSpeedOnGame = 400;
	public float cursorSpeedOnPauseMenu = 7;

	public float cursorLerpSpeed = 10;

	public bool enableGamepadCursorOnPause = true;
	public float timeToResetCursorAfterDisable = 1;

	public bool useMouseLimits = true;

	public bool activateCursorOnStart;

	[Space]
	[Header ("Input Settings")]
	[Space]

	public string horizontalXString = "Horizontal X";
	public string verticalYString = "Vertical Y";

	public string mouseXString = "Mouse X";
	public string mouseYString = "Mouse Y";

	public int buttonIndex = 0;
	public int joystickNumber = 1;

	public bool useRightJoystickAxis = true;
	public bool useBothJoysticks;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public Vector2 cursorPosition;
	float newX, newY;

	Point cursorPos = new Point ();

	[Space]
	[Space]

	public bool usingActualMouse;
	public bool screenFocused;

	[Space]
	[Space]

	public bool cursorCurrentlyActive;
	public bool cursorPreviouslyActive;

	public Vector2 mouseLimits;
	public bool horizontalLimitConfigured;
	public bool verticalLimitConfigured;

	public Vector2 realMouseAxisInput;

	public Vector2 axisInput;

	public bool ignoreSetSelectedGameObjectOnUIByGamepad;

	[Space]
	[Space]

	public bool playerInputLocated;

	public bool inputLocated;

	[Space]
	[Header ("Components")]
	[Space]

	public playerInputManager playerInput;
	public inputManager input;
		
	float currentMouseSpeed;

	[DllImport ("user32.dll")]
	static extern bool SetCursorPos (int X, int Y);

	[DllImport ("user32.dll")]
	static extern bool GetCursorPos (out Point pos);

	float lastTimeCursorEnabled;
	float lastTimeCursorDisabled;

	bool mouseLimitsAssigned;

	float lastTimeRealMouseUsed;
	float inputDifference;

	string joystickString;

	bool canUseGamepad;

	string mouseXPlayerIDString;
	string mouseYPlayerIDString;
	string horizontalXPlayerIDString;
	string vertircalYPlayerIDString;

	Vector2 currentResolution;

	float originalCursorSpeedOnGame = -1;


	void Start ()
	{
		updateJoystickString ();

		if (!playerInputLocated) {
			playerInputLocated = playerInput != null;
		}

		if (!inputLocated) {
			inputLocated = input != null;
		}

		mouseXPlayerIDString = mouseXString + joystickNumber;
		mouseYPlayerIDString = mouseYString + joystickNumber;

		horizontalXPlayerIDString = horizontalXString + joystickNumber;
		vertircalYPlayerIDString = verticalYString + joystickNumber;

		if (activateCursorOnStart) {
			menuPause.setCursorVisibleState (true);

			menuPause.setCursorLockState (false);

			showOrHideMouseController (playerInput, true, joystickNumber, false); 
		}
	}

	void updateJoystickString ()
	{
		joystickString = "joystick " + joystickNumber + " button " + buttonIndex;
	}

	void Update ()
	{
		if (cursorCurrentlyActive) {

			canUseGamepad = false;

			if (inputLocated) {
				if (!input.isUsingTouchControls () && input.isUsingGamepad ()) {
					canUseGamepad = true;
				}
			} else {
				canUseGamepad = true;
			}

			if (canUseGamepad) {
				if (inputLocated) {
					if (input.checkJoystickButton (joystickString, inputManager.buttonType.getKeyDown)) {
						MouseOperations.MouseEvent (MouseOperations.MouseEventFlags.LeftDown);
					}

					if (input.checkJoystickButton (joystickString, inputManager.buttonType.getKeyUp)) {
						MouseOperations.MouseEvent (MouseOperations.MouseEventFlags.LeftUp);
					}
				} else {
					if (Input.GetKeyDown (joystickString)) {
						MouseOperations.MouseEvent (MouseOperations.MouseEventFlags.LeftDown);
					}
						
					if (Input.GetKeyUp (joystickString)) {
						MouseOperations.MouseEvent (MouseOperations.MouseEventFlags.LeftUp);
					}
				}
				
				//Check if the actual mouse is being used
				screenFocused = Application.isFocused;

				if (playerInputLocated) {
					realMouseAxisInput = playerInput.getRealMouseAxisInput ();
				} else {
					realMouseAxisInput = new Vector2 (Input.GetAxis (mouseXString), Input.GetAxis (mouseYString));
				}

				inputDifference = Mathf.Abs (realMouseAxisInput.sqrMagnitude);

				if (inputDifference > 0.01f) {
					lastTimeRealMouseUsed = Time.unscaledTime;

					axisInput = realMouseAxisInput;
				}

				if (Time.unscaledTime > lastTimeRealMouseUsed + 1 && screenFocused) {
					if (usingActualMouse) {
						cursorPosition.x = cursorPos.X;
						cursorPosition.y = cursorPos.Y;

						newX = cursorPosition.x;
						newY = cursorPosition.y;

						usingActualMouse = false;
					}
				} else {
					usingActualMouse = true;
				}
	
				//Assign the input values according to if the gamepad or the actual mouse is being used
				if (!usingActualMouse) {
					if (playerInputLocated) {
						if (playerInput.isInputPaused () || playerInput.isPlayerDead ()) {
							if (useBothJoysticks) {
								axisInput = playerInput.getRealMouseGamepadAxisInput () + playerInput.getRealMovementGamepadAxisInput ();
							} else {
								if (useRightJoystickAxis) {
									axisInput = playerInput.getRealMouseGamepadAxisInput ();
								} else {
									axisInput = playerInput.getRealMovementGamepadAxisInput ();
								}
							}
						} else {
							if (useBothJoysticks) {
								axisInput = playerInput.getPlayerMouseAxis () + playerInput.getPlayerMovementAxis ();
							} else {
								if (useRightJoystickAxis) {
									axisInput = playerInput.getPlayerMouseAxis ();
								} else {
									axisInput = playerInput.getPlayerMovementAxis ();
								}
							}
						}
					} else {
						if (useBothJoysticks) {
							axisInput = new Vector2 (Input.GetAxis (mouseXPlayerIDString), Input.GetAxis (mouseYPlayerIDString)) +
							new Vector2 (Input.GetAxis (horizontalXPlayerIDString), Input.GetAxis (vertircalYPlayerIDString));
						} else {
							if (useRightJoystickAxis) {
								axisInput = new Vector2 (Input.GetAxis (mouseXPlayerIDString), Input.GetAxis (mouseYPlayerIDString));
							} else {
								axisInput = new Vector2 (Input.GetAxis (horizontalXPlayerIDString), Input.GetAxis (vertircalYPlayerIDString));
							}
						}
					}
				}

				GetCursorPos (out cursorPos);

				if (Time.deltaTime > 0) {
					if (useSmoothCursorSpeedOnGame) {
						currentMouseSpeed = Time.unscaledTime * cursorSpeedOnGame;
					} else {
						currentMouseSpeed = cursorSpeedOnPauseMenu;
					}
				} else {
					currentMouseSpeed = cursorSpeedOnPauseMenu;

					if (!enableGamepadCursorOnPause) {
						return;
					}
				}

				if (!usingActualMouse) {
					if (axisInput.x > 0) {
						cursorPosition.x += currentMouseSpeed;
					} else if (axisInput.x < 0) {
						cursorPosition.x -= currentMouseSpeed;
					}
					if (axisInput.y > 0) {
						cursorPosition.y -= currentMouseSpeed;
					} else if (axisInput.y < 0) {
						cursorPosition.y += currentMouseSpeed;
					}
				} 

				if (useMouseLimits) {
					cursorPosition.x = Mathf.Clamp (cursorPosition.x, 0, mouseLimits.x);
					cursorPosition.y = Mathf.Clamp (cursorPosition.y, 0, mouseLimits.y);
				}

				if (Time.deltaTime > 0 && !usingActualMouse) {
					newX = Mathf.Lerp (newX, cursorPosition.x, Time.unscaledTime * cursorLerpSpeed);
					newY = Mathf.Lerp (newY, cursorPosition.y, Time.unscaledTime * cursorLerpSpeed);
				} else {
					newX = cursorPosition.x;
					newY = cursorPosition.y;
				}

				if (!usingActualMouse) {
					SetCursorPos ((int)newX, (int)newY);
				}

				getScreenClampValues ();
			}
		}
	}

	public void showOrHideMouseController (playerInputManager newInput, bool state, int newJoystickNumber, bool pausingGame)
	{
		if (newInput == null || !newInput.useOnlyKeyboard) {
			playerInput = newInput;

			joystickNumber = newJoystickNumber;

			updateJoystickString ();

			showOrHideMouseController (state, pausingGame);
		}
	}

	public void setIgnoreSetSelectedGameObjectOnUIByGamepadState (bool state)
	{
		ignoreSetSelectedGameObjectOnUIByGamepad = state;
	}

	void getScreenClampValues ()
	{
		if (useMouseLimits && (!horizontalLimitConfigured || !verticalLimitConfigured)) {
			if (Time.deltaTime == 0 || Time.time > lastTimeCursorEnabled + 0.4f) {
				if (!horizontalLimitConfigured) {
					if (cursorPos.X > 0 && newX > (cursorPos.X + 50)) {

						if (cursorPos.X > currentResolution.x / 1.5f) {
							mouseLimits.x = cursorPos.X;

							horizontalLimitConfigured = true;

//							print ("max X");
						}
					}
				}

				if (!verticalLimitConfigured) {
					if (cursorPos.Y > 0 && newY > (cursorPos.Y + 50)) {
						if (cursorPos.Y > currentResolution.y / 1.5f) {
							mouseLimits.y = cursorPos.Y;

							verticalLimitConfigured = true;

//							print ("max Y");
						}
					}
				}
			}
		}
	}

	public void showOrHideMouseController (bool state, bool pausingGame)
	{
		if (input != null && !input.isUsingGamepad ()) {
			return;
		}

		if (!useMouseCursorEnabled) {
			return;
		}

		if (pausingGame) {
			cursorPreviouslyActive = false;
		} else {
			if (!state) {
				if (cursorCurrentlyActive) {
					cursorPreviouslyActive = true;
				}
			}
		}

		if (state) {
			if (Time.time > lastTimeCursorDisabled + timeToResetCursorAfterDisable) {
				cursorPreviouslyActive = false;
			}
		}

		if (state && (pausingGame || !cursorPreviouslyActive)) {
			resetCursorPosition ();
		}

		cursorCurrentlyActive = state;

		if (showDebugPrint) {
			print ("Setting cursor active state as " + cursorCurrentlyActive);
		}

		if (!pausingGame && state) {
			if (!cursorCurrentlyActive) {
				cursorPreviouslyActive = false;
			}
		}

		if (!ignoreSetSelectedGameObjectOnUIByGamepad) {
			if (state) {
				StartCoroutine (FocusButton ());
			}
		}

		ignoreSetSelectedGameObjectOnUIByGamepad = false;

		if (state) {
			lastTimeCursorEnabled = Time.time;
		} else {
			lastTimeCursorDisabled = Time.time;
		}
	}

	IEnumerator FocusButton ()
	{
		yield return null;

		GKC_Utils.setSelectedGameObjectOnUI (true, false, null, showDebugPrint);
	}

	public void resetCursorPosition ()
	{
		GetCursorPos (out cursorPos);

		cursorPosition.x = cursorPos.X;
		cursorPosition.y = cursorPos.Y;

		if (!mouseLimitsAssigned) {
			currentResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

			mouseLimits = currentResolution;

			mouseLimitsAssigned = true;
		}

		newX = cursorPosition.x;
		newY = cursorPosition.y;

		SetCursorPos ((int)cursorPosition.x, (int)cursorPosition.y);
		GetCursorPos (out cursorPos);
	}

	public Point getCursorPos ()
	{
		return cursorPos;
	}

	public void setUseRightOrLeftJoysticsAxis (bool state)
	{
		useRightJoystickAxis = state;
	}

	public int getCurrentJoystickNumber ()
	{
		return joystickNumber;
	}

	public void setMouseCursorControllerSpeedOnGameValue (float newValue)
	{
		if (originalCursorSpeedOnGame == -1) {
			originalCursorSpeedOnGame = cursorSpeedOnGame;
		}

		cursorSpeedOnGame = newValue;
	}

	public void setOriginalMouseCursorControllerSpeedOnGameValue ()
	{
		if (originalCursorSpeedOnGame != -1) {
			setMouseCursorControllerSpeedOnGameValue (originalCursorSpeedOnGame);
		}
	}

	[System.Serializable]
	public struct Point
	{
		public int X;
		public int Y;

		public Point (int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}

	public Vector2 getMouseAxis ()
	{
		if (playerInputLocated) {
			if (cursorCurrentlyActive) {
				return playerInput.getRealMouseGamepadAxisInput ();
			} else {
				return playerInput.getRealMouseAxisInput ();
			}
		} else {
			return new Vector2 (Input.GetAxis (mouseXPlayerIDString), Input.GetAxis (mouseYPlayerIDString)) +
			new Vector2 (Input.GetAxis (mouseXString), Input.GetAxis (mouseYString));
		}
	}

	public Vector2 getMovementAxis ()
	{
		if (playerInputLocated) {
			if (cursorCurrentlyActive) {
				return playerInput.getRealMovementGamepadAxisInput ();
			} else {
				return playerInput.getRealMovementAxisInput ();
			}
		} else {
			return new Vector2 (Input.GetAxis (horizontalXPlayerIDString), Input.GetAxis (vertircalYPlayerIDString)) +
			new Vector2 (Input.GetAxis (horizontalXString), Input.GetAxis (verticalYString));
		}
	}
}
