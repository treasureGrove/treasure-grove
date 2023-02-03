using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class consoleMode : MonoBehaviour
{
	public bool consoleModeEnabled = true;

	public List<commandInfo> commandInfoList = new List<commandInfo> ();

	public GameObject consoleWindow;
	public Transform commandTextParent;
	public RectTransform commandTextParentRectTransform;
	public Text currentConsoleCommandText;

	public string incorrectCommandMessage;

	public float lineSpacingAmount;

	public inputManager input;
	public playerInputManager playerInput;
	public bool consoleOpened;
	public menuPause pauseManager;
	public playerController playerControllerManager;

	public ScrollRect commandListScrollRect;

	public Transform spawnPosition;
	public float maxRadiusToInstantiate;
	public float deletingTextRate = 0.3f;
	public float startDeletingTimeAmount = 0.5f;

	public List<string> allowedKeysList = new List<string> ();

	public gameManager mainGameManager;

	public bool showDebugPrint;

	public string tabKeyName = "tab";
	public string deleteKeyName = "delete";
	public string spaceKeyName = "space";
	public string backSpaceKeyName = "backspace";
	public string returnKeyName = "return";
	public string enterKeyName = "enter";
	public string leftShiftKeyName = "leftshift";
	public string downArrowKeyName = "downarrow";
	public string upArrowKeyName = "uparrow";
	public string alphaKeyName = "alpha";
	public string capslockKeyName = "capslock";

	List<GameObject> commandTextGameObjectList = new List<GameObject> ();

	string currentKeyPressed;
	string originalKeyPressed;
	string currentTextWritten;
	string previousTextWritten;
	bool capsLockActivated;
	prefabsManager mainPrefabsManager;

	int numberOfLines;
	bool arrowKeyPressed;

	List<string> previousCommandList = new List<string> ();
	int previousCommandListIndex;
	bool canStartDeletingText;
	bool deletingText;
	float lastTimeDeletePressed;
	float lastTimeDeletingText;

	bool autocompletingCommand;
	int currentAutocompleteCommandIndex;
	string currentAutocompleteCommandFound = "";

	Coroutine scrollRectCoroutine;

	Coroutine consoleCoroutine;

	bool prefabsManagerLocated;

	void Start ()
	{
		if (consoleWindow.activeSelf) {
			consoleWindow.SetActive (false);
		}
	}

	public void stopUpdateConsoleCoroutine ()
	{
		if (consoleCoroutine != null) {
			StopCoroutine (consoleCoroutine);
		}
	}

	IEnumerator updateConsoleCoroutine ()
	{
		var waitTime = new WaitForSeconds (0.00001f);
	
		while (true) {
//	void Update ()
//	{
			if (consoleOpened) {
			
				currentKeyPressed = input.getKeyPressed (inputManager.buttonType.getKeyDown, true);

				if (currentKeyPressed != "") {
					checkKeyPressed (currentKeyPressed);

					if (currentKeyPressed.ToLower ().Equals (deleteKeyName) || currentKeyPressed.ToLower ().Equals (backSpaceKeyName)) {
						deletingText = true;

						lastTimeDeletePressed = Time.time;
					}
				}

				currentKeyPressed = input.getKeyPressed (inputManager.buttonType.getKeyUp, true);

				if (currentKeyPressed != "") {

					if (currentKeyPressed.ToLower ().Equals (leftShiftKeyName)) {
						capsLockActivated = !capsLockActivated;
					}

					if (currentKeyPressed.ToLower ().Equals (deleteKeyName) || currentKeyPressed.ToLower ().Equals (backSpaceKeyName)) {
						stopDeletingText ();
					}
				}

				if (deletingText) {
					if (canStartDeletingText) {				
						if (Time.time > lastTimeDeletingText + deletingTextRate) {
							if (currentTextWritten.Length > 0) {
								currentTextWritten = currentTextWritten.Remove (currentTextWritten.Length - 1, 1);
							}

							currentConsoleCommandText.text = "> " + currentTextWritten;

							lastTimeDeletingText = Time.time;
						}
					} else {
						if (Time.time > lastTimeDeletePressed + startDeletingTimeAmount) {
							canStartDeletingText = true;
						}
					}
				}
			}

			yield return waitTime;
		}
	}

	public void checkKeyPressed (string keyPressed)
	{
		originalKeyPressed = keyPressed;

		keyPressed = keyPressed.ToLower ();

		if (!allowedKeysList.Contains (keyPressed)) {
			return;
		}

		bool checkPass = false;

		if (keyPressed.Contains (alphaKeyName)) {
			keyPressed = keyPressed.Substring (keyPressed.Length - 1);
			originalKeyPressed = keyPressed;
		}

		//check if the arrow keys have been pressed
		if (keyPressed.Equals (upArrowKeyName) || keyPressed.Equals (downArrowKeyName)) {
			if (previousCommandList.Count > 0) {
				if (arrowKeyPressed) {
					if (keyPressed.Equals (upArrowKeyName)) {
						previousCommandListIndex--;

						if (previousCommandListIndex < 0) {
							previousCommandListIndex = 0;
						}
					} else {
						previousCommandListIndex++;

						if (previousCommandListIndex > previousCommandList.Count - 1) {
							previousCommandListIndex = previousCommandList.Count - 1;
						}
					}
				}

				arrowKeyPressed = true;

				if (showDebugPrint) {
					print ("index " + previousCommandListIndex);
				}

				currentTextWritten = "";
					
				originalKeyPressed = previousCommandList [previousCommandListIndex];
			} else {
				originalKeyPressed = "";
			}
		}

		if (keyPressed.Equals (tabKeyName)) {
			if (currentTextWritten.Length == 0) {
				return;
			}

			string commandToSearch = currentTextWritten;

			if (currentAutocompleteCommandFound != "") {
				commandToSearch = currentAutocompleteCommandFound;
			}

			bool commandFound = false;

			string commandToSearchToLower = commandToSearch.ToLower ();

			int commandToSearchLength = commandToSearch.Length;

			for (int i = 0; i < commandInfoList.Count; i++) {
				if (!commandFound && commandToSearchLength <= commandInfoList [i].Name.Length &&
				    commandToSearchToLower.Equals (commandInfoList [i].Name.ToLower ().Substring (0, commandToSearchLength))) {
					if ((autocompletingCommand && i > currentAutocompleteCommandIndex) || !autocompletingCommand) {
						originalKeyPressed = commandInfoList [i].Name;

						if (commandInfoList [i].containsAmount || commandInfoList [i].containsBool || commandInfoList [i].containsName) {
							originalKeyPressed += " ";
						}

						commandFound = true;

						currentAutocompleteCommandIndex = i;
					}
				}
			}

			if (!commandFound) {
				return;
			}

			if (currentAutocompleteCommandFound.Equals ("")) {
				currentAutocompleteCommandFound = currentTextWritten;
			}

			currentTextWritten = "";

			autocompletingCommand = true;

			string currentAutocompleteCommandFoundToLower = currentAutocompleteCommandFound.ToLower ();

			int currentAutocompleteCommandFoundLength = currentAutocompleteCommandFound.Length;

			if (showDebugPrint) {
				print (currentAutocompleteCommandFound + " " + currentAutocompleteCommandIndex);
			}

			if (currentAutocompleteCommandIndex < commandInfoList.Count - 1) {
				bool commandFoundAgain = false;

				for (int i = currentAutocompleteCommandIndex + 1; i < commandInfoList.Count; i++) {
					if (currentAutocompleteCommandFoundToLower.Equals (commandInfoList [i].Name.ToLower ().Substring (0, currentAutocompleteCommandFoundLength))) {
						commandFoundAgain = true;
					}
				}

				if (showDebugPrint) {
					print (commandFoundAgain);
				}

				if (!commandFoundAgain) {
					autocompletingCommand = false;
				}
			} else {
				autocompletingCommand = false;
			}
		} else {
			resetAutocompleteParameters ();
		}

		//add the an space
		if (keyPressed.Equals (spaceKeyName)) {
			currentTextWritten += " ";
		}

		//delete the last character
		else if (keyPressed.Equals (deleteKeyName) || keyPressed.Equals (backSpaceKeyName)) {
			if (currentTextWritten.Length > 0) {
				currentTextWritten = currentTextWritten.Remove (currentTextWritten.Length - 1);
			}
		}

		//check the current word added
		else if (keyPressed.Equals (enterKeyName) || keyPressed.Equals (returnKeyName)) {
			checkPass = true;
		} 

		//check if the caps are being using
		else if (keyPressed.Equals (capslockKeyName) || keyPressed.Equals (leftShiftKeyName)) {
			capsLockActivated = !capsLockActivated;
			return;
		}

		//add the current key pressed to the password
		else {
			if (capsLockActivated) {
				originalKeyPressed = originalKeyPressed.ToUpper ();
			} else {
				originalKeyPressed = originalKeyPressed.ToLower ();
			}
			currentTextWritten += originalKeyPressed;
		}

		currentConsoleCommandText.text = "> " + currentTextWritten;

		//the enter key has been pressed, so check if the current text written is the correct password
		if (checkPass) {
			previousTextWritten = currentTextWritten.ToLower ();
			checkCurrentCommand (currentTextWritten);

			currentTextWritten = "";
		}

		setScrollRectPosition (Vector3.up * (lineSpacingAmount * numberOfLines));
	}

	public void resetAutocompleteParameters ()
	{
		autocompletingCommand = false;

		currentAutocompleteCommandIndex = 0;

		currentAutocompleteCommandFound = "";
	}

	public bool checkCurrentCommand (string currentCommand)
	{
		if (currentCommand.Equals ("")) {
			createNewCommandText ("> ");

			return false;
		}

		arrowKeyPressed = false;

		previousCommandList.Add (currentCommand);

		previousCommandListIndex = previousCommandList.Count - 1;

		resetAutocompleteParameters ();

		createNewCommandText ("> " + currentCommand);

		currentCommand = currentCommand.ToLower ();

		for (int i = 0; i < commandInfoList.Count; i++) {
			string commandName = commandInfoList [i].Name.ToLower ();

			bool commandFound = false;

			if (commandName.Equals (currentCommand)) {
				commandFound = true;
			}

			if (!commandFound && (commandInfoList [i].containsAmount || commandInfoList [i].containsBool || commandInfoList [i].containsName)) {
				if (currentCommand.Contains (commandName)) {
					commandFound = true;
				}
			}

			if (commandFound) {

				//check the parameters of the command
				string nameParameter = "";

				bool incorrectCommand = false;

				if (commandInfoList [i].containsName) {
					nameParameter = currentCommand.Replace (commandInfoList [i].Name.ToLower () + " ", "");

					if (nameParameter.Length == 0) {
						incorrectCommand = true;
					} else {
						int amount = 0;

						string[] digits = Regex.Split (currentCommand, @"\D+");

						foreach (string value in digits) {
							if (int.TryParse (value, out amount)) {
								incorrectCommand = true;
							}
						}
					}
				}

				int amountValue = 0;

				if (commandInfoList [i].containsAmount) {
					
					bool numberFound = false;

					string[] digits = Regex.Split (currentCommand, @"\D+");

					foreach (string value in digits) {
						if (int.TryParse (value, out amountValue)) {
							if (showDebugPrint) {
								Debug.Log (value);
							}

							numberFound = true;
						}
					}

					if (commandInfoList [i].containsName) {
						nameParameter = nameParameter.Replace (amountValue.ToString (), "");

						if (nameParameter.Length > 0) {
							nameParameter = nameParameter.Remove (nameParameter.Length - 1, 1);
						}
					}

					incorrectCommand = !numberFound;
				}

				bool boolValue = false;

				if (commandInfoList [i].containsBool) {
					bool boolFound = false;

					if (currentCommand.Contains ("true")) {
						boolValue = true;
						boolFound = true;
					} else if (currentCommand.Contains ("false")) {
						boolFound = true;
					}

					if (showDebugPrint) {
						print (boolValue);
					}

					incorrectCommand = !boolFound;
				}

				//if the command is not correctly written, show the incorrect message and stop
				if (incorrectCommand) {
					createNewCommandText (commandInfoList [i].incorrectParametersText);
					return false;
				}
					
				//execute the event with the proper parameter
				if (commandInfoList [i].eventSendValues) {
					if (commandInfoList [i].containsAmount) {
						commandInfoList [i].eventToCallAmount.Invoke ((float)amountValue);
					}

					if (commandInfoList [i].containsBool) {
						commandInfoList [i].eventToCallBool.Invoke (boolValue);
					}

					if (showDebugPrint) {
						print (nameParameter);
					}

					if (commandInfoList [i].containsName) {
						commandInfoList [i].eventToCallName.Invoke (nameParameter);
					}
				} else {
					if (commandInfoList [i].eventToCall.GetPersistentEventCount () > 0) {
						commandInfoList [i].eventToCall.Invoke ();
					}
				}

				createNewCommandText (commandInfoList [i].commandExecutedText);
				return true;
			}
		}
		createNewCommandText (incorrectCommandMessage);
		return false;
	}

	public void createNewCommandText (string commandContent)
	{
		if (commandContent.Equals ("")) {
			return;
		}

		GameObject newConsoleCommnadGameObject = (GameObject)Instantiate (currentConsoleCommandText.gameObject, Vector3.zero, Quaternion.identity);

		newConsoleCommnadGameObject.transform.SetParent (commandTextParent);

		newConsoleCommnadGameObject.transform.localScale = Vector3.one;

		newConsoleCommnadGameObject.GetComponent<Text> ().text = commandContent;

		commandTextGameObjectList.Add (newConsoleCommnadGameObject);

		currentConsoleCommandText.transform.SetSiblingIndex (commandTextParent.childCount - 1);

		currentConsoleCommandText.text = ">";

		numberOfLines++;

		setScrollRectPosition (Vector3.up * (lineSpacingAmount * numberOfLines));
	}

	public void setScrollRectPosition (Vector3 position)
	{
		if (scrollRectCoroutine != null) {
			StopCoroutine (scrollRectCoroutine);
		}

		scrollRectCoroutine = StartCoroutine (setScrollRectPositionCoroutine (position));
	}

	IEnumerator setScrollRectPositionCoroutine (Vector3 position)
	{
		commandListScrollRect.vertical = false;

		//yield return new WaitForSeconds (0.01f);
		commandTextParentRectTransform.localPosition = position;

		yield return new WaitForSeconds (0.01f);

		commandListScrollRect.vertical = true;
	}

	public void showCommandList ()
	{
		for (int i = 0; i < commandInfoList.Count; i++) {
			createNewCommandText (commandInfoList [i].Name + " -> " + commandInfoList [i].description);
		}
	}

	public void clearCommandList ()
	{
		for (int i = 0; i < commandTextGameObjectList.Count; i++) {
			if (commandTextGameObjectList [i] != null) {
				Destroy (commandTextGameObjectList [i]);
			}
		}

		setScrollRectPosition (Vector3.zero);

		numberOfLines = 0;

		previousCommandList.Clear ();

		previousCommandListIndex = 0;
	}

	public void openOrCloseConsoleMode (bool state)
	{
		if (consoleOpened == state) {
			return;
		}

		consoleOpened = state;

		stopUpdateConsoleCoroutine ();

		if (consoleOpened) {
			if (input == null) {
				input = FindObjectOfType<inputManager> ();
			}

			consoleCoroutine = StartCoroutine (updateConsoleCoroutine ());
		}

		consoleWindow.SetActive (consoleOpened);

		pauseManager.showOrHideCursor (consoleOpened);

		pauseManager.setHeadBobPausedState (consoleOpened);

		pauseManager.changeCursorState (!consoleOpened);

		playerControllerManager.changeScriptState (!consoleOpened);

		pauseManager.openOrClosePlayerMenu (consoleOpened, null, false);

		pauseManager.usingDeviceState (consoleOpened);

		mainGameManager.setGamePauseState (consoleOpened);

		playerInput.setPlayerInputEnabledState (!consoleOpened);

		stopDeletingText ();

		pauseManager.setIngameMenuOpenedState ("Console Mode", consoleOpened, true);
	}

	public void stopDeletingText ()
	{
		deletingText = false;

		canStartDeletingText = false;
	}

	public void checkPrefabsManager ()
	{
		if (!prefabsManagerLocated) {
			mainPrefabsManager = GKC_Utils.addPrefabsManagerToScene ();

			prefabsManagerLocated = mainPrefabsManager != null;
		}
	}

	public void spawnObject ()
	{
		bool objectToSpawnFound = false;

		checkPrefabsManager ();

		if (prefabsManagerLocated) {
			string objectToSpawnName = previousTextWritten.Replace ("spawn", "");

			int amountToSpawn = 0;

			if (objectToSpawnName.Length == 0) {
				return;
			}

			string[] digits = Regex.Split (objectToSpawnName, @"\D+");

			foreach (string value in digits) {
				if (int.TryParse (value, out amountToSpawn)) {
					if (showDebugPrint) {
						Debug.Log (value);
					}
				}
			}

			objectToSpawnName = objectToSpawnName.Replace (amountToSpawn.ToString (), "");

			if (objectToSpawnName.Length == 0 || objectToSpawnName.Length < 3) {
				return;
			}

			objectToSpawnName = objectToSpawnName.Remove (0, 1);

			objectToSpawnName = objectToSpawnName.Remove (objectToSpawnName.Length - 1, 1);

			if (showDebugPrint) {
				print (objectToSpawnName);
			}

			GameObject objectToSpawn = mainPrefabsManager.getPrefabGameObject (objectToSpawnName);
		
			if (objectToSpawn != null) {
				for (int i = 0; i < amountToSpawn; i++) {
					Vector3 positionToSpawn = spawnPosition.position;

					positionToSpawn += Random.insideUnitSphere * maxRadiusToInstantiate;

					float extraOffset = mainPrefabsManager.getPrefabSpawnOffset (objectToSpawnName);

					if (extraOffset != 0) {
						positionToSpawn += spawnPosition.forward * extraOffset;
					}

					spawnGameObject (objectToSpawn, positionToSpawn, spawnPosition.rotation);

					objectToSpawnFound = true;
				}
			}

		} else {
			print ("WARNING: No prefabs manager component has been found in the scene, make sure this elements has been adde to the level");
		}

		if (!objectToSpawnFound) {
			createNewCommandText ("That object doesn't exist");
		}
	}

	public void showSpawnObjectsList ()
	{
		checkPrefabsManager ();

		if (prefabsManagerLocated) {
			for (int i = 0; i < mainPrefabsManager.prefabTypeInfoList.Count; i++) {
				for (int j = 0; j < mainPrefabsManager.prefabTypeInfoList [i].prefabInfoList.Count; j++) {
					createNewCommandText (mainPrefabsManager.prefabTypeInfoList [i].prefabInfoList [j].Name);
				}
			}
		}
	}

	public void spawnGameObject (GameObject objectToSpawn, Vector3 position, Quaternion rotation)
	{
		GameObject newConsoleCommnadGameObject = (GameObject)Instantiate (objectToSpawn, position, rotation);

		newConsoleCommnadGameObject.transform.position = position;
	}

	public void killAllEnemies ()
	{
		killCharacters (false);
	}

	public void killAllCharacters ()
	{
		killCharacters (true);
	}

	void killCharacters (bool killAllCharacters)
	{
		health[] healthList = FindObjectsOfType (typeof(health)) as health[];

		for (int i = 0; i < healthList.Length; i++) {
			if (!healthList [i].isDead ()) {
				bool canKillCharacter = false;

				if (killAllCharacters) {
					canKillCharacter = true;
				} else {
					if (!healthList [i].gameObject.CompareTag ("Player")) {
						canKillCharacter = true;
					}

					if (!healthList [i].gameObject.CompareTag ("friend")) {
						canKillCharacter = true;
					}
				}

				if (canKillCharacter) {
					healthList [i].killByButton ();
				}
			}
		}
	}

	public void destroyAllVehicles ()
	{
		vehicleHUDManager[] vehicleHUDManagerList = FindObjectsOfType (typeof(vehicleHUDManager)) as vehicleHUDManager[];

		for (int i = 0; i < vehicleHUDManagerList.Length; i++) {
			vehicleHUDManagerList [i].destroyVehicle ();
		}
	}

	public void inputActivateConsoleMode ()
	{
		if (!consoleModeEnabled) {
			return;
		}

		if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
			return;
		}

		openOrCloseConsoleMode (!consoleOpened);
	}

	public void setConsoleModeEnabledState (bool state)
	{
		consoleModeEnabled = state;
	}

	//EDITOR FUNCTIONS
	public void addCommand ()
	{
		commandInfo newCommandInfo = new commandInfo ();

		newCommandInfo.Name = "New Command";

		commandInfoList.Add (newCommandInfo);

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Updating console mode", gameObject);
	}

	[System.Serializable]
	public class commandInfo
	{
		public string Name;
		[TextArea (1, 10)] public string description;
		[TextArea (1, 10)] public string commandExecutedText;
		[TextArea (1, 10)] public string incorrectParametersText;

		public UnityEvent eventToCall;

		public bool eventSendValues;
		public bool containsAmount;
		public bool containsBool;
		public bool containsName;

		[System.Serializable]
		public class eventToCallWithAmount : UnityEvent<float>
		{

		}

		[SerializeField] public eventToCallWithAmount eventToCallAmount;

		[System.Serializable]
		public class eventToCallWithBool : UnityEvent<bool>
		{

		}

		[SerializeField] public eventToCallWithBool eventToCallBool;

		[System.Serializable]
		public class eventToCallWithName : UnityEvent<string>
		{

		}

		[SerializeField] public eventToCallWithName eventToCallName;
	}
}
