using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class overrideInputManager : MonoBehaviour
{
	public List<multiAxes> multiAxesList = new List<multiAxes> ();

	public bool inputEnabled;
	public bool isPlayerController;
	public playerInputManager playerInput;

	public UnityEvent startOverrideFunction;
	public UnityEvent stopOverrideFunction;

	public bool usePreOverrideFunctions;
	public UnityEvent preStartOverrideFunction;
	public UnityEvent preStopOverrideFunction;

	public bool activateActionScreen = true;
	public string actionScreenName;

	public bool destroyObjectOnStopOverride;
	public UnityEvent eventToDestroy;

	public bool showDebugActions;

	public overrideCameraController overrideCameraControllerManager;

	GameObject currentPlayer;
	playerController playerControllerManager;

	public inputManager input;

	overrideElementControlSystem currentOverrideElementControlSystem;

	multiAxes currentMultiAxes;
	Axes currentAxes;

	int currentMultiAxesCount;
	int currentAxesCount;

	int MAIndex;
	int AIndex;

	Coroutine updateCoroutine;

	bool playerInputAssigned;

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
		if (inputEnabled) {
			for (MAIndex = 0; MAIndex < currentMultiAxesCount; MAIndex++) {
				currentMultiAxes = multiAxesList [MAIndex];

				if (currentMultiAxes.currentlyActive) {
					currentAxesCount = currentMultiAxes.axes.Count;

					for (AIndex = 0; AIndex < currentAxesCount; AIndex++) {
						currentAxes = currentMultiAxes.axes [AIndex];

						if (currentAxes.actionEnabled) {
							if (playerInput.checkPlayerInputButtonFromMultiAxesList (currentMultiAxes.multiAxesStringIndex, currentAxes.axesStringIndex, 
								    currentAxes.buttonPressType, currentAxes.canBeUsedOnPausedGame)) {

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

	public Vector2 getCustomRawMovementAxis ()
	{
		if (!inputEnabled) {
			return Vector2.zero;
		}

		return playerInput.getPlayerRawMovementAxis ();
	}

	public Vector2 getCustomMovementAxis ()
	{
		if (!inputEnabled) {
			return Vector2.zero;
		}

		return playerInput.getPlayerMovementAxis ();
	}

	public Vector2 getCustomMouseAxis ()
	{
		if (!inputEnabled) {
			return Vector2.zero;
		}

		return playerInput.getPlayerMouseAxis ();
	}

	public void setCustomInputEnabledState (bool state)
	{
		inputEnabled = state;

		stopUpdateCoroutine ();

		if (inputEnabled) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}
	}

	public void setPlayerInfo (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer == null) {
			return;
		}

		findInputManager ();

		currentMultiAxesCount = multiAxesList.Count;

		playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		playerControllerManager = mainPlayerComponentsManager.getPlayerController ();

		currentOverrideElementControlSystem = mainPlayerComponentsManager.getOverrideElementControlSystem ();

		if (!isPlayerController) {
			playerInput = mainPlayerComponentsManager.getPlayerInputManager ();
		}

		playerInputAssigned = playerInput != null;
	}

	public void overrideControlState (bool state, GameObject currentPlayer)
	{
		if (state) {
			setPlayerInfo (currentPlayer);

			startOverrideFunction.Invoke ();
		} else {
			stopOverrideFunction.Invoke ();
		}

		if (playerInput != null) {
			if (isPlayerController) {
				playerInput.setPlayerID (playerControllerManager.getPlayerID ());
				playerInput.setPlayerInputEnabledState (state);
			} else {
				playerInput.setInputCurrentlyActiveState (!state);
			}
		}

		setCustomInputEnabledState (state);

		checkActivateActionScreen (state);
	}

	public void checkActivateActionScreen (bool state)
	{
		if (activateActionScreen) {
			if (playerControllerManager != null) {
				playerControllerManager.getPlayerInput ().enableOrDisableActionScreen (actionScreenName, state);
			}
		}
	}

	public void setPreOverrideControlState (bool state)
	{
		if (usePreOverrideFunctions) {
			if (state) {
				preStartOverrideFunction.Invoke ();
			} else {
				preStopOverrideFunction.Invoke ();
			}
		}
	}

	public void stopOverride ()
	{
		if (inputEnabled) {
			currentOverrideElementControlSystem.stopCurrentOverrideControl ();
		}
	}

	public void removeNewTemporalObjectFromOverrideControlSystem (GameObject newObject)
	{
		if (currentOverrideElementControlSystem != null) {
			currentOverrideElementControlSystem.removeNewTemporalObject (newObject);
		}
	}

	public void setPlayerInputActionState (bool playerInputActionState, string multiAxesInputName, string axesInputName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].axesName.Equals (multiAxesInputName)) {
				for (int j = 0; j < multiAxesList [i].axes.Count; j++) {
					if (multiAxesList [i].axes [j].Name.Equals (axesInputName)) {
						multiAxesList [i].axes [j].actionEnabled = playerInputActionState;
					}
				}
			}
		}
	}

	public void setPlayerInputMultiAxesState (bool playerInputMultiAxesState, string multiAxesInputName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].axesName.Equals (multiAxesInputName)) {
				multiAxesList [i].currentlyActive = playerInputMultiAxesState;
			}
		}
	}

	public void enablePlayerInputMultiAxes (string multiAxesInputName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].axesName.Equals (multiAxesInputName)) {
				multiAxesList [i].currentlyActive = true;
			}
		}
	}

	public void disablePlayerInputMultiAxes (string multiAxesInputName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].axesName.Equals (multiAxesInputName)) {
				multiAxesList [i].currentlyActive = false;
			}
		}
	}

	public bool isGamePaused ()
	{
		if (playerInputAssigned) {
			return playerInput.isGameManagerPaused ();
		} else {
			return false;
		}
	}

	//EDITOR FUNCTIONS
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

			Axes newAxes = new Axes ();

			newAxes.axesStringList = new string[input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count];

			for (int i = 0; i < input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count; i++) {
				string actionName = input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes [i].Name;
				newAxes.axesStringList [i] = actionName;
			}

			newAxes.multiAxesStringIndex = multiAxesIndex;
			currentMultiAxesList.axes.Add (newAxes);

			updateComponent ();
		}
	}

	public void updateMultiAxesList ()
	{
		findInputManager ();

		if (input != null) {
			for (int i = 0; i < multiAxesList.Count; i++) {
				multiAxesList [i].multiAxesStringList = new string[input.multiAxesList.Count];

				for (int j = 0; j < input.multiAxesList.Count; j++) {
					string axesName = input.multiAxesList [j].axesName;
					multiAxesList [i].multiAxesStringList [j] = axesName;
				}

				multiAxesList [i].axesName = input.multiAxesList [multiAxesList [i].multiAxesStringIndex].axesName;
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
				currentMultiAxesList.axes [i].axesStringList = new string[input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count];

				for (int j = 0; j < input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count; j++) {
					string actionName = input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes [j].Name;
					currentMultiAxesList.axes [i].axesStringList [j] = actionName;
				}
			}

			updateComponent ();
		}
	}

	public void setAllAxesList (int multiAxesListIndex)
	{
		findInputManager ();

		if (input != null) {
			
			multiAxes currentMultiAxesList = multiAxesList [multiAxesListIndex];

			currentMultiAxesList.axes.Clear ();

			for (int i = 0; i < input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count; i++) {
				Axes newAxes = new Axes ();

				newAxes.axesStringList = new string[input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count];

				for (int j = 0; j < input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes.Count; j++) {
					string actionName = input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes [j].Name;
					newAxes.axesStringList [j] = actionName;
				}

				newAxes.multiAxesStringIndex = multiAxesListIndex;
				newAxes.axesStringIndex = i;
				newAxes.Name = input.multiAxesList [currentMultiAxesList.multiAxesStringIndex].axes [i].Name;
				newAxes.actionName = newAxes.Name;

				currentMultiAxesList.axes.Add (newAxes);
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

	public overrideCameraController getOverrideCameraControllerManager ()
	{
		return overrideCameraControllerManager;
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
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
	}
}
