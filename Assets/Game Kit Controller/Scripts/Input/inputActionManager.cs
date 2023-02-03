using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class inputActionManager : MonoBehaviour
{
	public List<multiAxes> multiAxesList = new List<multiAxes> ();

	public bool inputActivated;
	public inputManager input;

	public bool showDebugActions;

	public bool inputCurrentlyActive = true;

	public bool inputPaused;

	public bool manualControlActive;
	[Range (-1, 1)] public float manualHorizontalInput;
	[Range (-1, 1)] public float manualVerticalInput;
	[Range (-1, 1)] public float manualMouseHorizontalInput;
	[Range (-1, 1)] public float manualMouseVerticalInput;

	public bool setAutomaticValuesOnHorizontalInput = true;

	public bool setAutomaticValuesOnVerticalInput = true;

	public bool setAutomaticValuesOnMouseHorizontalInput = true;

	public bool setAutomaticValuesOnVerticalVerticalInput = true;

	[Tooltip ("Use the touch joystick for control in preference to keys.")]
	public bool usingTouchMovementJoystick = true;

	public bool enableTouchJoysticks = true;

	public bool overrideInputValuesActive;
	Vector2 overrideInputAxisValue;

	Vector2 manualControlAxisValues;

	GameObject currentDriver;
	playerInputManager playerInput;

	multiAxes currentMultiAxes;
	Axes curentAxes;

	float footBrake;
	float handBrake;

	public Vector2 movementAxis;
	public Vector2 mouseAxis;
	public Vector2 rawMovementAxis;

	public bool useAxisAsHorizontalMovementInput = true;
	public bool useAxisAsVerticalMovementInput = true;

	public float horizontalMovementInputLerpSpeed = 10;
	public float verticalMovementInputLerpSpeed = 5;

	public bool useExternalVehicleTouchControlsEnabled;

	public bool disableTouchJoysticksOnExternalVehicleEnabled;

	public UnityEvent eventToActivateTouchControlsOnExternalVehicle;
	public UnityEvent eventToDeactivateTouchControlsOnExternalVehicle;


	float currentVerticalMovementAxisValue;
	float currentHorizontalMovementAxisValue;

	bool isUsingTouchControls;

	Vector2 temporalMovementAxis;

	bool positiveVerticalPressed;
	bool negativeVerticalPressed;

	bool positiveHorizontalPressed;
	bool negativeHorizontalPressed;

	int currentMultiAxesCount;
	int currentAxesCount;

	int MAIndex;
	int AIndex;

	Coroutine updateCoroutine;

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForSecondsRealtime (0.0001f);

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (inputActivated) {

			inputPaused = playerInput.isInputPausedForExternalComponents ();
				
			if (inputPaused) {
				return;
			}

			inputCurrentlyActive = !playerInput.isUsingPlayerMenu ();

			for (MAIndex = 0; MAIndex < currentMultiAxesCount; MAIndex++) {
				
				currentMultiAxes = multiAxesList [MAIndex];

				if (currentMultiAxes.currentlyActive) {
					currentAxesCount = currentMultiAxes.axes.Count;

					for (AIndex = 0; AIndex < currentAxesCount; AIndex++) {

						curentAxes = currentMultiAxes.axes [AIndex];

						if (curentAxes.actionEnabled) {
							if (inputCurrentlyActive || curentAxes.canBeUsedOnPausedGame) {
								if (playerInput.checkPlayerInputButtonFromMultiAxesList (currentMultiAxes.multiAxesStringIndex, curentAxes.axesStringIndex, 
									    curentAxes.buttonPressType, curentAxes.canBeUsedOnPausedGame)) {

									if (showDebugActions) {
										print (curentAxes.Name);
									}

									curentAxes.buttonEvent.Invoke ();
								}
							}
						}
					}
				}
			}

			isUsingTouchControls = playerInput.isUsingTouchControls ();

			temporalMovementAxis = playerInput.getPlayerMovementAxis ();
			rawMovementAxis = playerInput.getPlayerRawMovementAxis ();
				
			if (!useAxisAsHorizontalMovementInput && !isUsingTouchControls) {
				movementAxis.x = Mathf.MoveTowards (movementAxis.x, currentHorizontalMovementAxisValue, Time.deltaTime * horizontalMovementInputLerpSpeed);

				if (movementAxis.x > 0.01f) {
					rawMovementAxis.x = 1;
				} else if (movementAxis.x < -0.01f) {
					rawMovementAxis.x = -1;
				} else {
					rawMovementAxis.x = 0;
				}
			} else {
				currentHorizontalMovementAxisValue = 0;

				movementAxis.x = temporalMovementAxis.x;
			}
				
			if (!useAxisAsVerticalMovementInput && !isUsingTouchControls) {
				movementAxis.y = Mathf.MoveTowards (movementAxis.y, currentVerticalMovementAxisValue, Time.deltaTime * verticalMovementInputLerpSpeed);

				if (movementAxis.y > 0.01f) {
					rawMovementAxis.y = 1;
				} else if (movementAxis.y < -0.01f) {
					rawMovementAxis.y = -1;
				} else {
					rawMovementAxis.y = 0;
				}
			} else {
				currentVerticalMovementAxisValue = 0;

				movementAxis.y = temporalMovementAxis.y;
			}

			mouseAxis = playerInput.getPlayerMouseAxis ();
		}
	}

	public void setPositiveVerticalMovementAxisValue (int newValue)
	{
		positiveVerticalPressed = (newValue != 0);

		if (currentVerticalMovementAxisValue == -1 && newValue == 0) {
			return;
		}

		currentVerticalMovementAxisValue = newValue;

		if (newValue == 0) {
			if (negativeVerticalPressed) {
				currentVerticalMovementAxisValue = -1;
			}
		}
	}

	public void setNegativeVerticalMovementAxisValue (int newValue)
	{
		negativeVerticalPressed = (newValue != 0);

		if (currentVerticalMovementAxisValue == 1 && newValue == 0) {
			return;
		}

		currentVerticalMovementAxisValue = newValue;

		if (newValue == 0) {
			if (positiveVerticalPressed) {
				currentVerticalMovementAxisValue = 1;
			}
		}
	}

	public void setPositiveHorizontalMovementAxisValue (int newValue)
	{
		positiveHorizontalPressed = (newValue != 0);

		if (currentHorizontalMovementAxisValue == -1 && newValue == 0) {
			return;
		}

		currentHorizontalMovementAxisValue = newValue;

		if (newValue == 0) {
			if (negativeHorizontalPressed) {
				currentHorizontalMovementAxisValue = -1;
			}
		}
	}

	public void setNegativeHorizontalMovementAxisValue (int newValue)
	{
		negativeHorizontalPressed = (newValue != 0);

		if (currentHorizontalMovementAxisValue == 1 && newValue == 0) {
			return;
		}

		currentHorizontalMovementAxisValue = newValue;

		if (newValue == 0) {
			if (positiveHorizontalPressed) {
				currentHorizontalMovementAxisValue = 1;
			}
		}
	}

	public Vector2 getPlayerMovementAxis ()
	{
		if (inputPaused) {
			return Vector2.zero;
		}

		if (!inputActivated && !manualControlActive && !overrideInputValuesActive) {
			return Vector2.zero;
		}

		if (!inputCurrentlyActive) {
			return Vector2.zero;
		}

		if (manualControlActive) {
			if (setAutomaticValuesOnHorizontalInput && setAutomaticValuesOnVerticalInput) {
				return new Vector2 (manualHorizontalInput, manualVerticalInput);
			} else if (setAutomaticValuesOnHorizontalInput && !setAutomaticValuesOnVerticalInput) {
				return new Vector2 (manualHorizontalInput, movementAxis.y);
			} else if (!setAutomaticValuesOnHorizontalInput && setAutomaticValuesOnVerticalInput) {
				return new Vector2 (movementAxis.x, manualVerticalInput);
			} else {
				return movementAxis;
			}
		} else if (overrideInputValuesActive) {
			return overrideInputAxisValue;
		} else {
			return movementAxis;
		}
	}

	public Vector2 getPlayerMouseAxis ()
	{
		if (inputPaused) {
			return Vector2.zero;
		}

		if (!inputActivated && !manualControlActive && !overrideInputValuesActive) {
			return Vector2.zero;
		}

		if (!inputCurrentlyActive) {
			return Vector2.zero;
		}

		if (manualControlActive) {
			if (setAutomaticValuesOnMouseHorizontalInput && setAutomaticValuesOnVerticalVerticalInput) {
				return new Vector2 (manualMouseHorizontalInput, manualMouseVerticalInput);
			} else if (setAutomaticValuesOnMouseHorizontalInput && !setAutomaticValuesOnVerticalVerticalInput) {
				return new Vector2 (manualMouseHorizontalInput, mouseAxis.y);
			} else if (!setAutomaticValuesOnMouseHorizontalInput && setAutomaticValuesOnVerticalVerticalInput) {
				return new Vector2 (mouseAxis.x, manualMouseVerticalInput);
			} else {
				return mouseAxis;
			}

		} else {
			return mouseAxis;
		}
	}

	public Vector2 getPlayerRawMovementAxis ()
	{
		if (inputPaused) {
			return Vector2.zero;
		}

		if (!inputActivated && !manualControlActive && !overrideInputValuesActive) {
			return Vector2.zero;
		}

		if (!inputCurrentlyActive) {
			return Vector2.zero;
		}

		if (manualControlActive) {
			if (setAutomaticValuesOnHorizontalInput) {
				if (manualHorizontalInput > 0) {
					manualControlAxisValues.x = 1;
				} else if (manualHorizontalInput < 0) {
					manualControlAxisValues.x = -1;
				} else {
					manualControlAxisValues.x = 0;
				}
			} else {
				manualControlAxisValues.x = rawMovementAxis.x;
			}

			if (setAutomaticValuesOnVerticalInput) {
				if (manualVerticalInput > 0) {
					manualControlAxisValues.y = 1;
				} else if (manualVerticalInput < 0) {
					manualControlAxisValues.y = -1;
				} else {
					manualControlAxisValues.y = 0;
				}
			} else {
				manualControlAxisValues.y = rawMovementAxis.y;
			}

			return manualControlAxisValues;
		} else if (overrideInputValuesActive) {
			
			if (overrideInputAxisValue.x > 0) {
				manualControlAxisValues.x = 1;
			} else if (overrideInputAxisValue.x < 0) {
				manualControlAxisValues.x = -1;
			} else {
				manualControlAxisValues.x = 0;
			}

			if (overrideInputAxisValue.y > 0) {
				manualControlAxisValues.y = 1;
			} else if (overrideInputAxisValue.y < 0) {
				manualControlAxisValues.y = -1;
			} else {
				manualControlAxisValues.y = 0;
			}
			return overrideInputAxisValue;

		} else {
			return rawMovementAxis;
		}
	}

	public void enableOrDisableInput (bool state, GameObject driver)
	{
		inputActivated = state;

		stopUpdateCoroutine ();

		currentDriver = driver;

		setInputManager ();

		playerInput = currentDriver.GetComponent<playerInputManager> ();

		if (playerInput.isUsingTouchControls ()) {
			if (state) {
				if (enableTouchJoysticks) {
					playerInput.setUsingTouchMovementJoystickState (usingTouchMovementJoystick);

					playerInput.enableOrDisableTouchMovementJoystickForButtons (true);
				} else {
					playerInput.enableOrDisableTouchMovementJoystickForButtons (false);
				}
			} else {
				playerInput.setOriginalTouchMovementInputState ();
			}

			if (useExternalVehicleTouchControlsEnabled) {
				if (state) {
					eventToActivateTouchControlsOnExternalVehicle.Invoke ();
				} else {
					eventToDeactivateTouchControlsOnExternalVehicle.Invoke ();
				}

				if (disableTouchJoysticksOnExternalVehicleEnabled) {
					playerInput.enableOrDisableTouchMovementControl (!state);
					playerInput.enableOrDisableTouchCameraControl (!state);
				}
			}
		}

		if (inputActivated) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}
	}

	public void pauseOrResumeInput (bool state)
	{
		inputPaused = state;
	}

	public void setInputCurrentlyActiveState (bool state)
	{
		inputCurrentlyActive = state;
	}

	public void overrideInputValues (Vector2 newInputValues, float newFootBrakeValue, float newHandBrakeValue, bool overrideState)
	{
		overrideInputAxisValue = newInputValues;
		overrideInputValuesActive = overrideState;

		footBrake = newFootBrakeValue;
		handBrake = newHandBrakeValue;
	}

	public float getFootBrakeValue ()
	{
		return footBrake;
	}

	public float getHandBrakeValue ()
	{
		return handBrake;
	}

	public void setInputManager ()
	{
		if (input == null) {
			input = FindObjectOfType<inputManager> ();
		}

		currentMultiAxesCount = multiAxesList.Count;
	}


	//EDITOR FUNCTIONS
	public void setInputManagerOnEditor ()
	{
		if (input == null) {
			input = FindObjectOfType<inputManager> ();

			updateComponent ();
		}
	}

	public void addNewAxes ()
	{
		setInputManager ();

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
		setInputManager ();

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

			updateComponent ();
		}
	}

	public void updateMultiAxesList ()
	{
		setInputManager ();

		if (input != null) {
			for (int i = 0; i < multiAxesList.Count; i++) {

				multiAxes currentMultiAxes = multiAxesList [i];

				List<inputManager.multiAxes> currentTemporalMultiAxes = input.multiAxesList;

				currentMultiAxes.multiAxesStringList = new string[currentTemporalMultiAxes.Count];

				int currentMultiAxesIndex = -1;

				string currentMultiAxesName = currentMultiAxes.axesName;

				for (int j = 0; j < currentTemporalMultiAxes.Count; j++) {
					string axesName = currentTemporalMultiAxes [j].axesName;
					currentMultiAxes.multiAxesStringList [j] = axesName;

					if (currentMultiAxesName.Equals (axesName)) {
						currentMultiAxesIndex = j;
					}
				}

				if (currentMultiAxesIndex > -1) {
					if (currentMultiAxesIndex != currentMultiAxes.multiAxesStringIndex) {
						currentMultiAxes.multiAxesStringIndex = currentMultiAxesIndex;

						print (currentMultiAxesName.ToUpper () + " updated with index " + currentMultiAxesIndex);
					} else {
						print (currentMultiAxesName.ToUpper () + " keeps the same index " + currentMultiAxesIndex);
					}
				} else {
					print ("WARNING: Multi axes called " + currentMultiAxesName.ToUpper () + " hasn't been found, make sure to configure a multi axes " +
					"with that name in the main input manager");

					currentMultiAxes.multiAxesStringIndex = -1;
				}
			}

			updateComponent ();
		}
	}

	public void updateAxesList (int multiAxesListIndex)
	{
		setInputManager ();

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

					currentAxes.keyButton = currentTemporalAxes [currentAxes.axesStringIndex].key.ToString ();
				} else {
					print ("WARNING: Action called " + currentAxesName.ToUpper () + " hasn't been found, make sure to configure an action " +
					"with that name in the main input manager");

					currentAxes.axesStringIndex = -1;
				}
			}
		
			updateComponent ();
		}
	}

	public void setAllAxesList (int multiAxesListIndex)
	{
		setInputManager ();

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
			}

			updateComponent ();
		}
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

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Input Action Manager Values", gameObject);
	}

	[System.Serializable]
	public class multiAxes
	{
		public string axesName;
		public List<Axes> axes = new List<Axes> ();
		public GameObject screenActionsGameObject;
		public bool currentlyActive = true;
		public int multiAxesStringIndex;
		public string[] multiAxesStringList;
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
		public bool showInControlsMenu;

		public string keyButton;
	}
}