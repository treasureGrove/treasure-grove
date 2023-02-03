using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text.RegularExpressions;

[System.Serializable]
public class inputManager : MonoBehaviour
{
	public List<Axes> axes = new List<Axes> ();

	public List<multiAxes> multiAxesList = new List<multiAxes> ();

	public GameObject editInputPanelPrefab;
	public GameObject editInputMenu;
	public Text currentInputPanelListText;

	public loadType loadOption;
	public bool useRelativePath;

	public bool checkInputKeyDuplicatedOnRebindAction;
	public bool checkInputKeyDuplicatedOnlyOnSameInputCategory;

	public List<GameObject> buttonsDisabledAtStart = new List<GameObject> ();
	public List<gamepadInfo> gamepadList = new List<gamepadInfo> ();

	public List<playerInputManager> playerInputManagerList = new List<playerInputManager> ();

	public bool showActionKeysOnMenu = true;

	public bool touchPlatform;
	public bool gameCurrentlyPaused;

	public bool useTouchControls;

	public playerCharactersManager charactersManager;
	public menuPause pauseManager;
	public gameManager mainGameManager;

	public bool ignoreGamepad;
	public bool allowKeyboardAndGamepad;
	public bool allowGamepadInTouchDevice;
	public float checkConnectedGamepadRate;
	float lastGamepadUpdateCheckTime;

	public List<multiAxesEditButtonInput> multiAxesEditButtonInputList = new List<multiAxesEditButtonInput> ();

	public List<touchButtonListener> touchButtonList = new List<touchButtonListener> ();

	public List<touchButtonsInfo> touchButtonsInfoList = new List<touchButtonsInfo> ();
	public List<touchButtonsStringInfo> touchButtonsStringInfoList = new List<touchButtonsStringInfo> ();

	public List<touchButtonListener> buttonsToIgnoreSetVisibleStateList = new List<touchButtonListener> ();

	bool touchButtonsCurrentlyVisible = true;

	Scrollbar scroller;
	Touch currentTouch;
	int currentFingerID;

	public bool usingGamepad;
	public bool onlyOnePlayer;
	public bool usingKeyBoard;
	public int numberOfGamepads = -1;

	[Tooltip ("If true any keypress will be logged to the Debug console.")]
	public bool showKeyboardPressed;
	public bool showKeyboardPressedAction;
	public bool showGamepadPressed;
	public bool showGamepadPressedAction;
	public bool showPressedKeyWhenEditingInput;

	public bool showDebugKeyPressed;

	public bool editingInput;
	bool newKeyAssigned;

	editButtonInput currentEditButtonInputToRebind;
	string currentEditButtonInputPreviouseValue;

	KeyCode[] validKeyCodes;

	string[] currentGamepadConnectedNameArray;
	List<string> currentGamepadConnectedNameList = new List<string> ();

	public int currentInputPanelListIndex = 0;

	bool previouslyUsingGamepad;
	bool editInputMenuOpened;

	bool usingTouchControls;
	bool gameManagerPaused;

	public enum buttonType
	{
		//type of press of a key
		getKey,
		getKeyDown,
		getKeyUp,
		negMouseWheel,
		posMouseWheel,
	}

	//load the key input in the game from the current configuration in the inspector or load from a file
	public enum loadType
	{
		loadFile,
		loadEditorConfig,
	}

	float currentAxisValue;

	KeyCode currentKeyCodeToCheck;

	public Dictionary<string, Dictionary<JoystickData.ButtonTypes, int>> register;
	bool registerInitialized;
	public string[] joystickNames;
	public string joystickName;

	Axes currentKeyButtonToCheck;

	Dictionary<JoystickData.ButtonTypes, int> buttonConfig;
	int keyIndexPressed;
	string retString;
	bool joystickNamesInitialized;

	bool currentJoystickButtonIsAxis;

	Axes currentJoystickButtonToCheck;

	Axes currentTouchButtonToCheck;

	int currentTouchButtonIndexToCheck;
	touchButtonListener currentTouchButtonListener;

	int numberOfMultiAxes;
	int[] multiAxesIndexList;

	public string[] touchButtonListString;
	string currentJoystickButtonKeyString;

	string saveFileFolderName = "Input";
	string saveFileName = "Input File";

	string defaultSaveFileName = "Default Input File";

	int numberOfTouchControls;

	public List<string> currentInputActionList = new List<string> ();

	public KeyCode actionKeyToFilter;
	public joystickButtons joystickButtonToFilter;

	public bool createRebindInputMenuWithCurrentAxes;
	public int rebindInputMenuPanelsAmount;
	public int rebindInputMenuActionsPerPanelAmount;

	bool joystickFilterActive;
	bool keyFilterActive;

	public bool lastButtonPressedOnGamepad;
	public bool lastKeyPressedOnKeyboard;

	public bool showMissingInputWarningMessagesEnabled = true;

	void Start ()
	{
		//get if the current platform is a touch device
		touchPlatform = touchJoystick.checkTouchPlatform ();

		checkPlayerInputManagerList ();

		getGameManagerSettings ();

		usingTouchControls = mainGameManager.isUsingTouchControls ();

		pauseManager = charactersManager.getPauseManagerFromPlayerByIndex (0);

		//if the current platform is a mobile, enable the touch controls in case they are not active
		if (touchPlatform && !usingTouchControls) {
			pauseManager.setUseTouchControlsState (true);

			mainGameManager.setUseTouchControlsState (true);

			pauseManager.reloadStart ();

			usingKeyBoard = true;
			
			usingTouchControls = true;
		}

		useRelativePath = mainGameManager.useRelativePath;

		//load the key input
		loadButtonsInput ();

		getValidKeyCodes ();

		if (usingTouchControls) {
			ignoreGamepad = true;
		}

		if (ignoreGamepad) {
			currentGamepadConnectedNameArray = new String[0];

			setGamePadList ();
		} else {

			currentGamepadConnectedNameArray = Input.GetJoystickNames ();

			setGamePadList ();
		}
	}

	void Update ()
	{
		usingTouchControls = mainGameManager.isUsingTouchControls ();

		gameManagerPaused = mainGameManager.isGamePaused ();

		if (gameManagerPaused && currentNumberOfTouchButtonsPressed > 0) {
			currentNumberOfTouchButtonsPressed = 0;
		}

		if (!ignoreGamepad) {
			if (usingGamepad) {
				if (Time.time > checkConnectedGamepadRate + lastGamepadUpdateCheckTime) {
					joystickNames = Input.GetJoystickNames ();
				}
			}

			if ((Time.time > checkConnectedGamepadRate + lastGamepadUpdateCheckTime || gameManagerPaused)) {
				if (usingGamepad) {
					currentGamepadConnectedNameArray = joystickNames;
				} else {
					currentGamepadConnectedNameArray = Input.GetJoystickNames ();
				}

				lastGamepadUpdateCheckTime = Time.time;

				currentGamepadConnectedNameList.Clear ();

				for (int i = 0; i < currentGamepadConnectedNameArray.Length; ++i) {
					if (!currentGamepadConnectedNameArray [i].Equals ("")) {
						currentGamepadConnectedNameList.Add (currentGamepadConnectedNameArray [i]);
					}
				}

				currentGamepadConnectedNameArray = new string[currentGamepadConnectedNameList.Count];

				for (int i = 0; i < currentGamepadConnectedNameList.Count; i++) {
					currentGamepadConnectedNameArray [i] = currentGamepadConnectedNameList [i];
				}
				
				if (numberOfGamepads != currentGamepadConnectedNameArray.Length) {
					setGamePadList ();
				}

				if (allowKeyboardAndGamepad) {
					usingKeyBoard = true;
				}

				if (usingGamepad) {
					if (!previouslyUsingGamepad) {
//						print ("Gamepad added");

						previouslyUsingGamepad = true;

						if (editInputMenuOpened) {
							reloadInputMenu ();
						}
					}
				} else {
					if (previouslyUsingGamepad) {
//						print ("Gamepad removed");

						previouslyUsingGamepad = false;

						if (editInputMenuOpened) {
							reloadInputMenu ();
						}
					}
				}
			}
		}

		if (usingGamepad && ignoreGamepad) {
			currentGamepadConnectedNameArray = new String[0];

			setGamePadList ();
		}

		if (usingTouchControls && usingGamepad) {
			ignoreGamepad = true;
			usingKeyBoard = true;
		}

		//if the player is changin this input field, search for any keyboard press
		if (editingInput) {
			if (usingGamepad) {
				bool checkPressedKey = false;
				string currentPressedKey = "";

				foreach (KeyCode vKey in validKeyCodes) {
					//set the value of the key pressed in the input field
					if (Input.GetKeyDown (vKey)) {
						currentPressedKey = vKey.ToString ();
						if (!currentPressedKey.Contains ("Alpha")) {
							checkPressedKey = true;
						}
					}
				}

				currentAxisValue = Input.GetAxis ("Left Trigger " + "1");
				if (currentAxisValue != 0) {
					currentPressedKey = "14";
					checkPressedKey = true;
				}

				currentAxisValue = Input.GetAxis ("Right Trigger " + "1");
				if (currentAxisValue != 0) {
					currentPressedKey = "15";
					checkPressedKey = true;
				}
					
				currentAxisValue = Input.GetAxis ("DPad X" + "1");
				if (currentAxisValue != 0) {
					if (currentAxisValue < 0) {
						currentPressedKey = "10";
					} else {
						currentPressedKey = "11";
					}

					checkPressedKey = true;
				}

				currentAxisValue = Input.GetAxis ("DPad Y" + "1");
				if (currentAxisValue != 0) {
					if (currentAxisValue < 0) {
						currentPressedKey = "12";
					} else {
						currentPressedKey = "13";
					}

					checkPressedKey = true;
				}

				if (checkPressedKey) {
					string keyName = "";
					int keyNumber = -1;

					string[] numbers = Regex.Split (currentPressedKey, @"\D+");
					foreach (string value in numbers) {
						int number = 0;
						if (int.TryParse (value, out number)) {
							print (value + " " + value.Length);

							keyNumber = number;
						}
					}

					if (keyNumber > -1) {
						keyName = Enum.GetName (typeof(joystickButtons), keyNumber);
						currentEditButtonInputToRebind.actionKeyText.text = keyName;

						if (showPressedKeyWhenEditingInput) {
							print (keyName);
						}

						newKeyAssigned = true;
					
						//stop the checking of the keyboard
						editingInput = false;
					}

					return;
				}

			} else {
				foreach (KeyCode vKey in validKeyCodes) {
					//set the value of the key pressed in the input field
					if (Input.GetKeyDown (vKey)) {
						if (showPressedKeyWhenEditingInput) {
							print (vKey.ToString ());
						}

						bool canAssignNewInput = true;

						if (checkInputKeyDuplicatedOnRebindAction) {
							
							string currentActionKey = vKey.ToString ();

							string currentActionName = multiAxesList [currentEditButtonInputToRebind.multiAxesIndex].axes [currentEditButtonInputToRebind.axesIndex].Name;
									
							for (int i = 0; i < multiAxesList.Count; i++) {
								if (canAssignNewInput && (!checkInputKeyDuplicatedOnlyOnSameInputCategory || currentEditButtonInputToRebind.multiAxesIndex == i)) {
									List<Axes> currentAxesList = multiAxesList [i].axes;

									for (int j = 0; j < currentAxesList.Count; j++) {

										Axes currentAxes = currentAxesList [j];

										if (canAssignNewInput) {
											//print (currentAxes.keyButton + " " + currentActionKey);

											if (currentAxes.keyButton.Equals (currentActionKey)) {
											  
												if (currentAxes.Name != currentActionName) {
													canAssignNewInput = false;

													print ("Key " + currentActionKey + " already assigned on the action " + currentAxes.Name);
												}
											}
										}
									}
								}
							}

							if (canAssignNewInput && checkInputKeyDuplicatedOnlyOnSameInputCategory) {
								print ("No key " + currentActionKey + " assigned to other action on the category " + multiAxesList [currentEditButtonInputToRebind.multiAxesIndex].axesName + ".\n" +
								"New key assigned to action " + currentActionName);
							}
						}

						if (canAssignNewInput) {
							currentEditButtonInputToRebind.actionKeyText.text = vKey.ToString ();

							//stop the checking of the keyboard
							editingInput = false;

							newKeyAssigned = true;

							return;
						}
					}
				}
			}
		}

		if (showDebugKeyPressed) {
			foreach (KeyCode vKey in validKeyCodes) {
				if (Input.GetKeyDown (vKey)) {
					Debug.Log ("KeyCode Pressed: " + vKey);
				}
			}
		}
	}

	void checkPlayerInputManagerList ()
	{
		bool checkIfEmptyInputElements = checkNullElementsOnPlayerInputManagerList ();

		if (checkIfEmptyInputElements) {
			charactersManager.checkPlayerElementsOnScene ();

			playerInputManagerList.Clear ();

			for (int i = 0; i < charactersManager.playerList.Count; i++) {
				playerInputManagerList.Add (charactersManager.playerList [i].playerInput);
			}

			pauseManager = charactersManager.getPauseManagerFromPlayerByIndex (0);

			if (playerInputManagerList.Count >= 1) {
				playerInputManager currentPlayerInputManager = playerInputManagerList [0];

				if (currentPlayerInputManager != null) {
					GameObject currentInputPanelListTextGameObject = currentPlayerInputManager.getCurrentInputPanelListText ();

					if (currentInputPanelListTextGameObject != null) {
						currentInputPanelListText = currentInputPanelListTextGameObject.GetComponent<Text> ();
					}
					
					editInputPanelPrefab = currentPlayerInputManager.getEditInputPanelPrefab ();
			
					editInputMenu = currentPlayerInputManager.getEditInputMenu ();
				}
			}
		}
	}

	public bool isUsingGamepad ()
	{
		return usingGamepad;
	}

	public bool isUsingKeyBoard ()
	{
		return usingKeyBoard;
	}

	public bool isUsingTouchControls ()
	{
		return usingTouchControls;
	}

	public bool isUsingTouchControlsOnGameManager ()
	{
		return mainGameManager.isUsingTouchControls ();
	}

	public bool isGameManagerPaused ()
	{
		return gameManagerPaused;
	}

	public bool isAllowKeyboardAndGamepad ()
	{
		return allowKeyboardAndGamepad;
	}

	public bool isAllowGamepadInTouchDevice ()
	{
		return allowGamepadInTouchDevice;
	}

	public void setEditInputMenuOpenedState (bool state)
	{
		editInputMenuOpened = state;
	}

	//Update current input panel to edit
	public void openInputMenu ()
	{
		if (!showActionKeysOnMenu) {
			return;
		}

		currentInputPanelListIndex = -1;

		setNextInputPanel ();

		updateCurrentInputPanel (currentInputPanelListIndex);

		setKeyNamesInActionsList ();
	}

	public void reloadInputMenu ()
	{
		updateCurrentInputPanel (currentInputPanelListIndex);

		setKeyNamesInActionsList ();
	}

	public void setKeyNamesInActionsList ()
	{
		for (int i = 0; i < multiAxesEditButtonInputList.Count; i++) {

			multiAxesEditButtonInput currentEditButton = multiAxesEditButtonInputList [i];
				
			if (currentEditButton.multiAxesEditPanelActive) {
				for (int j = 0; j < currentEditButton.buttonsList.Count; j++) {

					editButtonInput currentEditButtonInput = currentEditButton.buttonsList [j];

					if (currentEditButtonInput.editButtonInputActive) {
						if (usingGamepad) {
							currentEditButtonInput.actionKeyText.text = currentEditButtonInput.gamepadActionKey;
						} else {
							currentEditButtonInput.actionKeyText.text = currentEditButtonInput.keyboardActionKey;
						}
					}
				}
			}
		}
	}

	public void updateCurrentInputPanel (int index)
	{
		for (int i = 0; i < multiAxesEditButtonInputList.Count; i++) {
			if (i == index) {
				if (!multiAxesEditButtonInputList [i].multiAxesEditPanel.activeSelf) {
					multiAxesEditButtonInputList [i].multiAxesEditPanel.SetActive (true);
				}
			} else {
				if (multiAxesEditButtonInputList [i].multiAxesEditPanel.activeSelf) {
					multiAxesEditButtonInputList [i].multiAxesEditPanel.SetActive (false);
				}
			}
		}

		if (currentInputPanelListText != null) {
			currentInputPanelListText.text = multiAxesEditButtonInputList [index].multiAxesEditPanel.name;
		}

		//get the scroller in the edit input menu
		if (editInputMenu != null) {
			scroller = editInputMenu.GetComponentInChildren<Scrollbar> ();

			//set the scroller in the top position
			scroller.value = 1;
		}
	}

	public void setNextInputPanel ()
	{
		currentInputPanelListIndex++;
		if (currentInputPanelListIndex > multiAxesList.Count - 1) {
			currentInputPanelListIndex = 0;
		}

		int max = 0;
		bool exit = false;
		while (!exit) {
			if (multiAxesEditButtonInputList [currentInputPanelListIndex].multiAxesEditPanelActive) {
				exit = true;
			} else {

				max++;
				if (max > 100) {
					exit = true;
				}

				currentInputPanelListIndex++;
				if (currentInputPanelListIndex > multiAxesList.Count - 1) {
					currentInputPanelListIndex = 0;
				}
			}
		}

		updateCurrentInputPanel (currentInputPanelListIndex);
	}

	public void setPreviousInputPanel ()
	{
		currentInputPanelListIndex--;
		if (currentInputPanelListIndex < 0) {
			currentInputPanelListIndex = multiAxesList.Count - 1;
		}

		int max = 0;
		bool exit = false;
		while (!exit) {
			if (multiAxesEditButtonInputList [currentInputPanelListIndex].multiAxesEditPanelActive) {
				exit = true;
			} else {

				max++;
				if (max > 100) {
					exit = true;
				}

				currentInputPanelListIndex--;
				if (currentInputPanelListIndex < 0) {
					currentInputPanelListIndex = multiAxesList.Count - 1;
				}
			}
		}

		updateCurrentInputPanel (currentInputPanelListIndex);
	}

	public void destroyRebindMenusAndActionsPanels ()
	{
		for (int i = 0; i < multiAxesEditButtonInputList.Count; i++) {
			if (multiAxesEditButtonInputList [i].multiAxesEditPanel != null) {
				DestroyImmediate (multiAxesEditButtonInputList [i].multiAxesEditPanel);
			}
		}

		multiAxesEditButtonInputList.Clear ();

		updateComponent ();
	}

	public void instantiateRebindMenusAndActionsPanels (bool updateComponentActive)
	{
		if (multiAxesEditButtonInputList.Count > 0) {
			destroyRebindMenusAndActionsPanels ();
		}

		int amountOfMenuPanels = multiAxesList.Count;
		int amountOfMenuActionsPerPanel = 0;

		if (!createRebindInputMenuWithCurrentAxes) {
			amountOfMenuPanels = rebindInputMenuPanelsAmount;
			amountOfMenuActionsPerPanel = rebindInputMenuActionsPerPanelAmount;
		}

		if (amountOfMenuPanels == 0) {
			print ("WARNING: Make sure to configure an amount of panels or use the current multi axes list amount");

			return;
		}

		if (editInputPanelPrefab == null) {
			return;
		}
			
		for (int i = 0; i < amountOfMenuPanels; i++) {
			GameObject newEditInputPanel = (GameObject)Instantiate (editInputPanelPrefab, editInputPanelPrefab.transform.position, Quaternion.identity, editInputPanelPrefab.transform.parent);

			newEditInputPanel.transform.localScale = Vector3.one;

			RectTransform newRectTransform = newEditInputPanel.GetComponent<RectTransform> ();
			RectTransform rectTransformPrefab = editInputPanelPrefab.GetComponent<RectTransform> ();

			newRectTransform.anchorMin = rectTransformPrefab.anchorMin;
			newRectTransform.anchorMax = rectTransformPrefab.anchorMax;
			newRectTransform.anchoredPosition = rectTransformPrefab.anchoredPosition;
			newRectTransform.sizeDelta = rectTransformPrefab.sizeDelta;

			if (!newEditInputPanel.activeSelf) {
				newEditInputPanel.SetActive (true);
			}

			newEditInputPanel.name = "Input Panel " + (i + 1);

			editInputPanelInfo currentEditInputPanelInfo = newEditInputPanel.GetComponent<editInputPanelInfo> ();

			List<editButtonInput> buttonsList = new List<editButtonInput> ();

			GameObject bottom = currentEditInputPanelInfo.bottomGameObject;

			if (createRebindInputMenuWithCurrentAxes) {
				amountOfMenuActionsPerPanel = multiAxesList [i].axes.Count;
			}

			//get all the keys field inside the edit input menu
			for (int j = 0; j < amountOfMenuActionsPerPanel; j++) {
				//every key field in the edit input button has a editButtonInput component, so create every of them

				GameObject newInputButton = (GameObject)Instantiate (currentEditInputPanelInfo.editButtonInputGameObject, 
					                            currentEditInputPanelInfo.editButtonInputGameObject.transform.position, 
					                            Quaternion.identity, currentEditInputPanelInfo.buttonsParent);

				if (newInputButton.activeSelf) {
					newInputButton.SetActive (false);
				}

				newInputButton.name = "Input Button " + (j + 1);

				editButtonInput currentEditButtonInput = newInputButton.GetComponent<editButtonInput> ();

				currentEditButtonInput.editButtonGameObject = newInputButton;

				newInputButton.transform.localScale = Vector3.one;

				buttonsList.Add (currentEditButtonInput);

				if (currentEditInputPanelInfo.editButtonInputGameObject.activeSelf) {
					currentEditInputPanelInfo.editButtonInputGameObject.SetActive (false);
				}
			}

			multiAxesEditButtonInput newMultiAxesEditButtonInput = new multiAxesEditButtonInput ();

			newMultiAxesEditButtonInput.multiAxesEditPanel = newEditInputPanel;

			newMultiAxesEditButtonInput.buttonsList = buttonsList;

			multiAxesEditButtonInputList.Add (newMultiAxesEditButtonInput);

			if (newEditInputPanel.activeSelf) {
				newEditInputPanel.SetActive (false);
			}

			//set the empty element of the list in the bottom of the list
			bottom.transform.SetAsLastSibling ();

			bottom.SetActive (false);
		}

		if (updateComponentActive) {
			print ("Rebind Input Panels and actions buttons created in editor");

			updateComponent ();
		}
	}

	void getCurrentAxesListFromInspector ()
	{
		if (multiAxesEditButtonInputList.Count == 0) {
			createRebindInputMenuWithCurrentAxes = true;

			instantiateRebindMenusAndActionsPanels (false);
		}

		if (editInputMenu != null && !editInputMenu.activeSelf) {
			editInputMenu.SetActive (true);
		}

		if (editInputPanelPrefab != null) {
			editInputPanelPrefab.SetActive (false);
		}

		if (multiAxesEditButtonInputList.Count > 0) {
			for (int i = 0; i < multiAxesList.Count; i++) {
				if (multiAxesList [i].currentlyActive) {
					multiAxesEditButtonInput currentMultiAxesEditButtonInput = multiAxesEditButtonInputList [i];

					currentMultiAxesEditButtonInput.multiAxesEditPanelActive = multiAxesList [i].multiAxesEditPanelActive;

					currentMultiAxesEditButtonInput.multiAxesEditPanel.name = multiAxesList [i].axesName;

					if (currentMultiAxesEditButtonInput.multiAxesEditPanel.activeSelf != currentMultiAxesEditButtonInput.multiAxesEditPanelActive) {
						currentMultiAxesEditButtonInput.multiAxesEditPanel.SetActive (currentMultiAxesEditButtonInput.multiAxesEditPanelActive);
					}

					List<Axes> currentAxesList = multiAxesList [i].axes;

					//get all the keys field inside the edit input menu
					for (int j = 0; j < currentAxesList.Count; j++) {
						Axes currentAxes = currentAxesList [j];
						
						currentAxes.keyButton = currentAxes.key.ToString ();

						if (currentAxes.actionEnabled) {
							//every key field in the edit input button has a editButtonInput component, so create every of them
		
							currentMultiAxesEditButtonInput.buttonsList [j].name = currentAxes.Name;

							editButtonInput currentEditButtonInput = currentMultiAxesEditButtonInput.buttonsList [j];

							currentEditButtonInput.editButtonInputActive = true;

							currentEditButtonInput.actionNameText.text = currentAxes.Name;
							currentEditButtonInput.actionKeyText.text = currentAxes.keyButton;

							currentEditButtonInput.keyboardActionKey = currentAxes.keyButton;

							if (currentAxes.joystickButton != joystickButtons.None) {
								currentEditButtonInput.gamepadActionKey = currentAxes.joystickButton.ToString ();
							}

							currentEditButtonInput.multiAxesIndex = i;
							currentEditButtonInput.axesIndex = j;

							if (currentAxes.showActionInRebindPanel) {
								if (!currentEditButtonInput.editButtonGameObject.activeSelf) {
									currentEditButtonInput.editButtonGameObject.SetActive (true);
								}
							}
						}
					}
				}
			}
		}

		if (editInputMenu != null && editInputMenu.activeSelf) {
			editInputMenu.SetActive (false);
		}
	}

	public void saveButtonsInput ()
	{
		//for every key field in the edit input menu, save its value and change them in the inputManager inspector aswell
		if (Application.isPlaying) {
			for (int i = 0; i < multiAxesEditButtonInputList.Count; i++) {

				multiAxesEditButtonInput currentEditButton = multiAxesEditButtonInputList [i];

				if (currentEditButton.multiAxesEditPanelActive) {
					for (int j = 0; j < currentEditButton.buttonsList.Count; j++) {
						
						editButtonInput currentEditButtonInput = currentEditButton.buttonsList [j];

						if (currentEditButtonInput.editButtonInputActive) {
							string currentKeyAction = currentEditButtonInput.actionKeyText.text;

							if (usingGamepad) {
								if (!currentKeyAction.Equals ("")) {
									currentEditButtonInput.gamepadActionKey = currentKeyAction;

									changeGamepadKeyValue (currentKeyAction, currentEditButtonInput.multiAxesIndex, currentEditButtonInput.axesIndex);
								}
							} else {
								currentEditButtonInput.keyboardActionKey = currentKeyAction;

								changeKeyValue (currentKeyAction, currentEditButtonInput.multiAxesIndex, currentEditButtonInput.axesIndex);
							}
						}
					}
				}
			}
		}

		//create a list of axes to store it
		List<multiAxes> temporalMultiAxesList = new List<multiAxes> ();

		for (int i = 0; i < multiAxesList.Count; i++) {
			List<Axes> temporalAxesList = new List<Axes> ();
			List<Axes> currentAxesList = multiAxesList [i].axes;

			for (int j = 0; j < currentAxesList.Count; j++) {
				Axes currentAxes = currentAxesList [j];

				Axes axe = new Axes ();
				axe.Name = currentAxes.Name;
				axe.actionEnabled = currentAxes.actionEnabled;

				axe.key = currentAxes.key;
				axe.keyButton = currentAxes.key.ToString ();

				axe.joystickButton = currentAxes.joystickButton;

				axe.touchButtonIndex = currentAxes.touchButtonIndex;
				axe.touchButtonPanelIndex = currentAxes.touchButtonPanelIndex;
			
				temporalAxesList.Add (axe);
			}

			multiAxes newMultiAxes = new multiAxes ();
			newMultiAxes.axesName = multiAxesList [i].axesName;
			newMultiAxes.axes = temporalAxesList;
			temporalMultiAxesList.Add (newMultiAxes);
		}

		//save the input list
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (getDataPath (saveFileName)); 
		bf.Serialize (file, temporalMultiAxesList);
		file.Close ();

		print ("Input Saved in " + saveFileName);
	}

	public string getDataPath (string fileNameToUse)
	{
		string dataPath = "";

		if (useRelativePath) {
			dataPath = saveFileFolderName;
		} else {
			dataPath = Application.persistentDataPath + "/" + saveFileFolderName;
		}

		if (!Directory.Exists (dataPath)) {
			Directory.CreateDirectory (dataPath);
		}

		dataPath += "/" + fileNameToUse;

		return dataPath;
	}

	public void loadButtonsInput ()
	{
		List<multiAxes> temporalMultiAxesList = new List<multiAxes> ();

		//if the configuration is loaded from a file, get a new axes list with the stored values
		if (loadOption == loadType.loadFile) {
			//if the file of buttons exists, get that list
			if (!touchPlatform && File.Exists (getDataPath (saveFileName))) {
				BinaryFormatter bf = new BinaryFormatter ();

				FileStream file = File.Open (getDataPath (saveFileName), FileMode.Open);

				temporalMultiAxesList = (List<multiAxes>)bf.Deserialize (file);

				file.Close ();

				multiAxesList.Clear ();

				for (int i = 0; i < temporalMultiAxesList.Count; i++) {
					List<Axes> temporalAxesList = new List<Axes> ();
					for (int j = 0; j < temporalMultiAxesList [i].axes.Count; j++) {
						temporalAxesList.Add (temporalMultiAxesList [i].axes [j]);
					}

					multiAxes newMultiAxes = new multiAxes ();
					newMultiAxes.axesName = temporalMultiAxesList [i].axesName;
					newMultiAxes.axes = temporalAxesList;

					multiAxesList.Add (newMultiAxes);
				}
			}

			//else, get the list created in the inspector
			else {
				for (int i = 0; i < multiAxesList.Count; i++) {
					List<Axes> temporalAxesList = new List<Axes> ();

					List<Axes> currentAxesList = multiAxesList [i].axes;

					for (int j = 0; j < currentAxesList.Count; j++) {
						temporalAxesList.Add (currentAxesList [j]);
					}

					multiAxes newMultiAxes = new multiAxes ();
					newMultiAxes.axesName = multiAxesList [i].axesName;
					newMultiAxes.axes = temporalAxesList;

					temporalMultiAxesList.Add (newMultiAxes);
				}
				
				saveButtonsInputFromInspector (saveFileName);
			}
		} 

		//else the new axes list is the axes in the input manager inspector
		else {
			for (int i = 0; i < multiAxesList.Count; i++) {
				List<Axes> currentAxesList = multiAxesList [i].axes;

				multiAxes newMultiAxes = new multiAxes ();
				newMultiAxes.axesName = multiAxesList [i].axesName;
				newMultiAxes.axes = currentAxesList;

				temporalMultiAxesList.Add (newMultiAxes);
			}
		}

		for (int i = 0; i < multiAxesList.Count; i++) {
			List<Axes> currentAxesList = multiAxesList [i].axes;

			for (int j = 0; j < currentAxesList.Count; j++) {

				Axes currentAxes = currentAxesList [j];

				int currentPanelIndex = currentAxes.touchButtonPanelIndex;

				int currentButtonIndex = currentAxes.touchButtonIndex;

				if (currentPanelIndex > 0) {

					if (touchButtonsInfoList.Count > currentPanelIndex - 1) {
						if (touchButtonsInfoList [currentPanelIndex - 1].touchButtonList.Count > currentButtonIndex) {
							touchButtonListener currentTouchButtonListener = touchButtonsInfoList [currentPanelIndex - 1].touchButtonList [currentButtonIndex];

							currentAxes.currentTouchButtonIndex = touchButtonList.IndexOf (currentTouchButtonListener);

							touchButtonListener touchButtonListenerToCheck = touchButtonList [currentAxes.currentTouchButtonIndex];

							if (touchButtonListenerToCheck != null) {

								GameObject currentTouchButtonToCheck = touchButtonListenerToCheck.gameObject;

								if (currentTouchButtonToCheck != null && isInTouchButtonToDisableList (currentTouchButtonToCheck)) {
									if (currentTouchButtonToCheck.activeSelf) {
										currentTouchButtonToCheck.SetActive (false);
									}
								}
							} else {
								currentAxes.touchButtonPanelIndex = 0;
							}
						} else {
							print ("WARNING: The axes list " + multiAxesList [i].axesName + " is trying to use a touch button which is not correct, " +
							"in the touch panel " + touchButtonsInfoList [currentPanelIndex - 1].Name
							+ ", make sure to check the touch button input option in the action" + currentAxes.Name);
						}
					} else {
						print ("WARNING: The axes list " + multiAxesList [i].axesName + " is trying to use a touch panel which is not correct," +
						" make sure to check the touch button input option in the action" + currentAxes.Name);
					}
				}
			}
		}

		if (showActionKeysOnMenu) {
			//get the current list of axes defined in the inspector
			getCurrentAxesListFromInspector ();
		}

		//set in every key field in the edit input menu with the stored key input for every field
		for (int i = 0; i < multiAxesEditButtonInputList.Count; i++) {
			multiAxesEditButtonInput currentEditButton = multiAxesEditButtonInputList [i];

			if (currentEditButton.multiAxesEditPanelActive) {
				for (int j = 0; j < multiAxesEditButtonInputList [i].buttonsList.Count; j++) {
					
					editButtonInput currentEditButtonInput = currentEditButton.buttonsList [j];

					if (currentEditButtonInput.editButtonInputActive) {
						if (i <= temporalMultiAxesList.Count - 1 && j < temporalMultiAxesList [i].axes.Count - 1) {
							currentEditButtonInput.actionKeyText.text = temporalMultiAxesList [i].axes [j].keyButton;
						}
					}
				}
			}
		}

		numberOfMultiAxes = multiAxesList.Count;

		multiAxesIndexList = new int[numberOfMultiAxes];

		for (int i = 0; i < numberOfMultiAxes; i++) {
			multiAxesIndexList [i] = multiAxesList [i].axes.Count;
		}

		numberOfTouchControls = touchButtonList.Count;
	}

	//save the input list in the inspector to a file
	public void saveButtonsInputFromInspector (string fileNameToUse)
	{
		//create a list of axes to store it
		List<multiAxes> temporalMultiAxesList = new List<multiAxes> ();

		for (int i = 0; i < multiAxesList.Count; i++) {
			List<Axes> temporalAxesList = new List<Axes> ();
			List<Axes> currentAxesList = multiAxesList [i].axes;

			for (int j = 0; j < currentAxesList.Count; j++) {
				Axes currentAxes = currentAxesList [j];

				Axes axe = new Axes ();
				axe.Name = currentAxes.Name;
				axe.actionEnabled = currentAxes.actionEnabled;

				axe.key = currentAxes.key;
				axe.keyButton = currentAxes.key.ToString ();

				axe.joystickButton = currentAxes.joystickButton;

				axe.touchButtonIndex = currentAxes.touchButtonIndex;
				axe.touchButtonPanelIndex = currentAxes.touchButtonPanelIndex;

				temporalAxesList.Add (axe);
			}

			multiAxes newMultiAxes = new multiAxes ();
			newMultiAxes.axesName = multiAxesList [i].axesName;
			newMultiAxes.axes = temporalAxesList;

			temporalMultiAxesList.Add (newMultiAxes);
		}

		//save the input list
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (getDataPath (fileNameToUse)); 
		bf.Serialize (file, temporalMultiAxesList);
		file.Close ();

		print ("Current Input Saved in " + fileNameToUse);
	}

	//load the input list from the file to the inspector
	public void loadButtonsInputFromInspector (string fileNameToUse)
	{
		List<multiAxes> temporalMultiAxesList = new List<multiAxes> ();

		if (File.Exists (getDataPath (fileNameToUse))) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (getDataPath (fileNameToUse), FileMode.Open);

			temporalMultiAxesList = (List<multiAxes>)bf.Deserialize (file);

			file.Close ();
		} else {
			print ("File located in " + fileNameToUse + " doesn't exist, make sure the file is created previously to load it in the editor");

			return;
		}

		//print (axesList.Count);
		multiAxesList.Clear ();

		//set the touch button for every axes, if it had it
		for (int i = 0; i < temporalMultiAxesList.Count; i++) {
			
			List<Axes> temporalAxesList = new List<Axes> ();

			for (int j = 0; j < temporalMultiAxesList [i].axes.Count; j++) {
				temporalAxesList.Add (temporalMultiAxesList [i].axes [j]);
			}

			multiAxes newMultiAxes = new multiAxes ();
			newMultiAxes.axesName = temporalMultiAxesList [i].axesName;
			newMultiAxes.axes = temporalAxesList;

			multiAxesList.Add (newMultiAxes);
		}

		print ("Input File Loaded from " + fileNameToUse);
	}

	public void getGameManagerSettings ()
	{
		saveFileFolderName = mainGameManager.saveInputFileFolderName;

		saveFileName = mainGameManager.saveInputFileName + " " + mainGameManager.versionNumber + mainGameManager.fileExtension;

		defaultSaveFileName = mainGameManager.defaultInputSaveFileName + " " + mainGameManager.versionNumber + mainGameManager.fileExtension;

		useRelativePath = mainGameManager.useRelativePath;

		mainGameManager.setCurrentPersistentDataPath ();

		updateComponent ();
	}

	public void setCurrentInputToDefault ()
	{
		getGameManagerSettings ();

		loadButtonsInputFromInspector (defaultSaveFileName);

		updateComponent ();
	}

	public void loadButtonsInspectorFromSaveFile ()
	{
		getGameManagerSettings ();

		loadButtonsInputFromInspector (saveFileName);

		updateComponent ();
	}

	public void saveCurrentInputAsDefault ()
	{
		getGameManagerSettings ();

		saveButtonsInputFromInspector (defaultSaveFileName);

		updateComponent ();
	}

	public void saveButtonsInputToSaveFile ()
	{
		getGameManagerSettings ();

		saveButtonsInputFromInspector (saveFileName);

		updateComponent ();
	}

	public void changeKeyValue (string keyButton, int multiAxesIndex, int axesIndex)
	{
		Axes currentAxes = multiAxesList [multiAxesIndex].axes [axesIndex];

		print (currentAxes.Name + " " + keyButton);

		KeyCode newKeyCode = (KeyCode)System.Enum.Parse (typeof(KeyCode), keyButton);

		currentAxes.key = newKeyCode;
		currentAxes.keyButton = currentAxes.key.ToString ();
	}

	public void changeGamepadKeyValue (string keyButton, int multiAxesIndex, int axesIndex)
	{
		Axes currentAxes = multiAxesList [multiAxesIndex].axes [axesIndex];

		print (currentAxes.Name + " " + keyButton);

		currentAxes.joystickButton = (joystickButtons)Enum.Parse (typeof(joystickButtons), keyButton);
	}
		
	//get the key button value for an input field, using the action of the button
	public string getButtonKey (string actionName)
	{
//		print (actionName);

		for (int i = 0; i < multiAxesList.Count; i++) {
			List<Axes> currentAxesList = multiAxesList [i].axes;

			for (int j = 0; j < currentAxesList.Count; j++) {

				Axes currentAxes = currentAxesList [j];

				if (currentAxes.Name.Equals (actionName)) {
					if (((usingGamepad && !usingKeyBoard) || lastButtonPressedOnGamepad) && currentAxes.joystickButton != joystickButtons.None) {
						return currentAxes.joystickButton.ToString ();
					} else {
						return currentAxes.keyButton;
					}
				}
			}
		}

		print ("WARNING: no input key found for the action called " + actionName);

		return "";
	}

	//if the input field has been pressed, call a coroutine, to avoid the input field get the mouse press as new value
	public void startEditingInput (GameObject button)
	{
		if (!editingInput) {
			StartCoroutine (startEditingInputCoroutine (button));
		}
	}

	//set the text of the input field to ... and start to check the keyboard press
	IEnumerator startEditingInputCoroutine (GameObject button)
	{
		yield return null;

		currentEditButtonInputToRebind = button.GetComponent<editButtonInput> ();

		currentEditButtonInputPreviouseValue = currentEditButtonInputToRebind.actionKeyText.text;
		currentEditButtonInputToRebind.actionKeyText.text = "...";

		editingInput = true;

		newKeyAssigned = false;
	}

	public bool isEditingInput ()
	{
		return editingInput;
	}

	//any change done in the input field is undone
	public void cancelEditingInput ()
	{
		editingInput = false;

		newKeyAssigned = false;

		if (currentEditButtonInputToRebind) {
//			print (currentEditButtonInputPreviouseValue);

			currentEditButtonInputToRebind.actionKeyText.text = currentEditButtonInputPreviouseValue;
			currentEditButtonInputToRebind = null;
		}
	}

	public void checkIfSaveInputAfterRebind ()
	{
		if (newKeyAssigned) {
			saveButtonsInput ();

			newKeyAssigned = false;
		}
	}

	public void getValidKeyCodes ()
	{
		validKeyCodes = (KeyCode[])System.Enum.GetValues (typeof(KeyCode));
	}

	public string getKeyPressed (buttonType type, bool canBeUsedOnGamePaused)
	{
		if ((!gameManagerPaused || canBeUsedOnGamePaused) && (!usingTouchControls || allowGamepadInTouchDevice)) {
			foreach (KeyCode vKey in validKeyCodes) {
				switch (type) {
				//this key is for holding
				case buttonType.getKey:
					if (Input.GetKey (vKey)) {
						return vKey.ToString ();
					}
					break;

				case buttonType.getKeyDown:
					//this key is for press once
					if (Input.GetKeyDown (vKey)) {
						return vKey.ToString ();
					}
					break;

				case buttonType.getKeyUp:
					//this key is for release 
					if (Input.GetKeyUp (vKey)) {
						return vKey.ToString ();
					}
					break;
				}
			}
		}

		return "";
	}

	public bool checkJoystickButton (string joystickString, buttonType type)
	{
		if (!usingTouchControls || allowGamepadInTouchDevice) {
			if ((!usingKeyBoard || allowKeyboardAndGamepad)) {
				if (usingGamepad) {
					switch (type) {
					//this key is for holding
					case buttonType.getKeyDown:
						if (Input.GetKeyDown (joystickString)) {
							return true;
						}
						break;

					case buttonType.getKeyUp:
						if (Input.GetKeyUp (joystickString)) {
							return true;
						}
						break;
					}
				}
			}
		}

		return false;
	}

	public bool checkPlayerInputButtonFromMultiAxesList (int multiAxesIndex, int axesIndex, buttonType type, int controllerNumber, bool canBeUsedOnPausedGame, bool useOnlyKeyboard)
	{
		if (usingTouchControls) {
			return getTouchButtonFromMultiAxesList (multiAxesIndex, axesIndex, type);
		}

		if ((!usingKeyBoard || allowKeyboardAndGamepad) && !useOnlyKeyboard) {
			if (usingGamepad) {
				if (getJoystickButtonFromMultiAxesList (multiAxesIndex, axesIndex, type, controllerNumber, canBeUsedOnPausedGame)) {

					if (!lastButtonPressedOnGamepad) {
//						print ("pressing gamepad");

						GKC_Utils.eventOnPressingGamepadInput (controllerNumber);

						lastButtonPressedOnGamepad = true;
					}

					lastKeyPressedOnKeyboard = false;

					return true;
				}
			}
		}

		if (usingKeyBoard || allowKeyboardAndGamepad || useOnlyKeyboard) {
			if (getKeyboardButtonFromMultiAxesList (multiAxesIndex, axesIndex, type, canBeUsedOnPausedGame)) {

				if (!lastKeyPressedOnKeyboard) {
//					print ("pressing keyboard");

					GKC_Utils.eventOnPressingKeyboardInput (controllerNumber);

					lastKeyPressedOnKeyboard = true;
				}

				lastButtonPressedOnGamepad = false;

				return true;
			}
		}

		return false;
	}

	//function called in the script where pressing that button will make an action in the game, for example jump, crouch, shoot, etc...
	//every button sends its action and the type of pressing
	public bool getKeyboardButtonFromMultiAxesList (int multiAxesIndex, int axesIndex, buttonType type, bool canBeUsedOnPausedGame)
	{
		//if the game is not paused, and the current control is the keyboard
		if ((canBeUsedOnPausedGame || !gameManagerPaused) && (!usingTouchControls || allowGamepadInTouchDevice)) {

			if (multiAxesIndex >= numberOfMultiAxes) {
				if (showMissingInputWarningMessagesEnabled) {
					print ("WARNING: The input manager is trying to access to an action which has changed or has been removed.  " +
					"\n Please, check the Player Input Manager to configure properly this group of actions according to the " +
					"settings in the main custom Input Manager of GKC." +
					"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
				}

				return false;
			}

			if (multiAxesIndex < 0 || axesIndex < 0) {
				if (showMissingInputWarningMessagesEnabled) {
					if (multiAxesIndex >= 0) {
						print ("WARNING: The input manager is trying to access to an action which has changed or has been removed in the Group of actions" +
						" called " + multiAxesList [multiAxesIndex].axesName + " \n Please, check the Player Input Manager to configure properly this group of actions according to the " +
						"settings in the main custom Input Manager of GKC" +
						"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
					} else {
						print ("WARNING: The input manager is trying to access to a multi axes which has changed or has been removed.  " +
						"\n Please, check the Player Input Manager to configure properly this group of actions according to the " +
						"settings in the main custom Input Manager of GKC" +
						"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
					}
				}

				return false;
			}

			if (axesIndex >= multiAxesIndexList [multiAxesIndex]) {
				if (showMissingInputWarningMessagesEnabled) {
					print ("WARNING: The input manager is trying to access to an action which has changed or has been removed in the Group of actions" +
					" called " + multiAxesList [multiAxesIndex].axesName + " \n Please, check the Player Input Manager to configure properly this group of actions according to the " +
					"settings in the main custom Input Manager of GKC" +
					"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
				}

				return false;
			}

			if (!multiAxesList [multiAxesIndex].currentlyActive) {
				return false;
			}

			currentKeyButtonToCheck = multiAxesList [multiAxesIndex].axes [axesIndex];

			//check that the key pressed has being defined as an action
			if (!currentKeyButtonToCheck.actionEnabled) {
				return false;
			}

			currentKeyCodeToCheck = currentKeyButtonToCheck.key;

			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (Input.GetKey (currentKeyCodeToCheck)) {

					if (showKeyboardPressed) {
						print ("Get Key: " + currentKeyButtonToCheck.keyButton);
					}

					if (showKeyboardPressedAction) {
						print ("Action Name On Key: " + currentKeyButtonToCheck.Name);
					}

					return true;
				}
				break;

			//this key is for press once
			case buttonType.getKeyDown:
				if (Input.GetKeyDown (currentKeyCodeToCheck)) {

					if (showKeyboardPressed) {
						print ("Get Key Down: " + currentKeyButtonToCheck.keyButton);
					}

					if (showKeyboardPressedAction) {
						print ("Action Name On Key Down: " + currentKeyButtonToCheck.Name);
					}
						
					return true;
				}
				break;

			//this key is for release
			case buttonType.getKeyUp:
				if (Input.GetKeyUp (currentKeyCodeToCheck)) {

					if (showKeyboardPressed) {
						print ("Get Key Up: " + currentKeyButtonToCheck.keyButton);
					}

					if (showKeyboardPressedAction) {
						print ("Action Name On Key Up: " + currentKeyButtonToCheck.Name);
					}
						
					return true;
				}
				break;

			//mouse wheel
			case buttonType.negMouseWheel:
				//check if the wheel of the mouse has been used, and in what direction
				if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
					return true;
				}
				break;

			case buttonType.posMouseWheel:
				//check if the wheel of the mouse has been used, and in what direction
				if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
					return true;
				}
				break;
			}
		}

		return false;
	}

	public bool isKeyboardButtonPressed (int controllerNumber, string multiAxesName, string axesName)
	{
		int multiAxesIndex = multiAxesList.FindIndex (s => s.axesName.ToLower ().Equals (multiAxesName.ToLower ()));

		if (multiAxesIndex > -1) {

			int axesIndex = multiAxesList [multiAxesIndex].axes.FindIndex (s => s.Name.ToLower ().Equals (axesName.ToLower ()));

			if (axesIndex > -1) {
				currentKeyButtonToCheck = multiAxesList [multiAxesIndex].axes [axesIndex];

				if (usingTouchControls) {
					if (currentKeyButtonToCheck.touchButtonPanelIndex > 0) {
						currentTouchButtonIndexToCheck = currentKeyButtonToCheck.currentTouchButtonIndex;

						if (numberOfTouchControls > currentTouchButtonIndexToCheck) {
							currentTouchButtonListener = touchButtonList [currentTouchButtonIndexToCheck];

							if (currentTouchButtonListener.pressed) {
								return true;
							}
						}
					}
				} else {
					
					currentKeyCodeToCheck = currentKeyButtonToCheck.key;

					if (Input.GetKey (currentKeyCodeToCheck)) {

						return true;
					}

					if ((!usingKeyBoard || allowKeyboardAndGamepad) && usingGamepad) {
						keyIndexPressed = (int)currentKeyButtonToCheck.joystickButton;

						if (keyIndexPressed == -1) {
							return false;
						}

						currentJoystickButtonIsAxis = (keyIndexPressed >= 10 && keyIndexPressed <= 15);

						if (currentJoystickButtonIsAxis) {
							return getAxisValue (controllerNumber, keyIndexPressed, multiAxesIndex, axesIndex, buttonType.getKey);
						}

						currentJoystickButtonKeyString = gamepadList [controllerNumber - 1].joystickButtonStringList [keyIndexPressed];
	
						if (Input.GetKey (currentJoystickButtonKeyString)) {
							return true;
						}
					}
				}
			}
		}

		return false;
	}

	int currentTouchCount;
	int currentTouchIndex;

	public bool getTouchButtonFromMultiAxesList (int multiAxesIndex, int axesIndex, buttonType type)
	{
		//if the game is not paused, and the current control is a touch device
		if (!gameManagerPaused && usingTouchControls) {

			if (multiAxesIndex >= numberOfMultiAxes) {
				if (showMissingInputWarningMessagesEnabled) {
					print ("WARNING: The input manager is trying to access to an action which has changed or has been removed.  " +
					"\n Please, check the Player Input Manager to configure properly this group of actions according to the " +
					"settings in the main custom Input Manager of GKC." +
					"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
				}

				return false;
			}

			if (multiAxesIndex < 0 || axesIndex < 0) {
				if (showMissingInputWarningMessagesEnabled) {
					if (multiAxesIndex >= 0) {
						print ("WARNING: The input manager is trying to access to an action which has changed or has been removed in the Group of actions" +
						" called " + multiAxesList [multiAxesIndex].axesName + " \n Please, check the Player Input Manager to configure properly this group of actions according to the " +
						"settings in the main custom Input Manager of GKC" +
						"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
					} else {
						print ("WARNING: The input manager is trying to access to a multi axes which has changed or has been removed.  " +
						"\n Please, check the Player Input Manager to configure properly this group of actions according to the " +
						"settings in the main custom Input Manager of GKC" +
						"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
					}
				}

				return false;
			}

			if (axesIndex >= multiAxesIndexList [multiAxesIndex]) {
				if (showMissingInputWarningMessagesEnabled) {
					print ("WARNING: The input manager is trying to access to an action which has changed or has been removed in the Group of actions" +
					" called " + multiAxesList [multiAxesIndex].axesName + " \n Please, check the Player Input Manager to configure properly this group of actions according to the " +
					"settings in the main custom Input Manager of GKC" +
					"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
				}

				return false;
			}

			if (!multiAxesList [multiAxesIndex].currentlyActive) {
				return false;
			}

			currentTouchButtonToCheck = multiAxesList [multiAxesIndex].axes [axesIndex];

			if (!currentTouchButtonToCheck.actionEnabled) {
				return false;
			}

			if (currentTouchButtonToCheck.touchButtonPanelIndex > 0) {
				currentTouchButtonIndexToCheck = currentTouchButtonToCheck.currentTouchButtonIndex;

				if (numberOfTouchControls > currentTouchButtonIndexToCheck) {
					currentTouchButtonListener = touchButtonList [currentTouchButtonIndexToCheck];
				
					//check for a began touch
					if (type == buttonType.getKeyDown) {
						if (currentTouchButtonListener.pressedDown) {

							currentTouchCount = Input.touchCount;
							if (!touchPlatform) {
								currentTouchCount++;
							}

							for (currentTouchIndex = 0; currentTouchIndex < currentTouchCount; currentTouchIndex++) {
								if (!touchPlatform) {
									currentTouch = touchJoystick.convertMouseIntoFinger ();
								} else {
									currentTouch = Input.GetTouch (currentTouchIndex);
								}

								if (currentTouch.phase == TouchPhase.Began) {
									//if the button is pressed (OnPointerDown), return true
									//print ("getKeyDown");
									return true;
								}
							}
						}
					}

					//check for a hold touch
					if (type == buttonType.getKey) {
						if (currentTouchButtonListener.pressed) {

							currentTouchCount = Input.touchCount;
							if (!touchPlatform) {
								currentTouchCount++;
							}

							for (currentTouchIndex = 0; currentTouchIndex < currentTouchCount; currentTouchIndex++) {
								if (!touchPlatform) {
									currentTouch = touchJoystick.convertMouseIntoFinger ();
								} else {
									currentTouch = Input.GetTouch (currentTouchIndex);
								}
							
								if (currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved) {
									//if the button is pressed OnPointerDown, and is not released yet (OnPointerUp), return true
									//print ("getKey");
									return true;					
								}	
							}
						}
					}

					//check for a release touch
					if (type == buttonType.getKeyUp) {
						if (currentTouchButtonListener.pressedUp) {

							currentTouchCount = Input.touchCount;
							if (!touchPlatform) {
								currentTouchCount++;
							}

							for (currentTouchIndex = 0; currentTouchIndex < currentTouchCount; currentTouchIndex++) {
								if (!touchPlatform) {
									currentTouch = touchJoystick.convertMouseIntoFinger ();
								} else {
									currentTouch = Input.GetTouch (currentTouchIndex);
								}

								if (currentTouch.phase == TouchPhase.Ended) {
									//if the button is released (OnPointerUp), return true
									//print ("getKeyUp");
									return true;
								}
							}
						}
					}
				}
			}
		}

		return false;
	}

	public bool useMultipleGamepadDictionary;

	public bool getJoystickButtonFromMultiAxesList (int multiAxesIndex, int axesIndex, buttonType type, int controllerNumber, bool canBeUsedOnPausedGame)
	{
		//if the game is not paused, and the current control is the keyboard
		if ((canBeUsedOnPausedGame || !gameManagerPaused) && (!usingTouchControls || allowGamepadInTouchDevice)) {

			if (controllerNumber == 0 || controllerNumber > numberOfGamepads) {
				return false;
			}

			if (multiAxesIndex >= numberOfMultiAxes) {
				if (showMissingInputWarningMessagesEnabled) {
					print ("WARNING: The input manager is trying to access to an action which has changed or has been removed.  " +
					"\n Please, check the Player Input Manager to configure properly this group of actions according to the " +
					"settings in the main custom Input Manager of GKC." +
					"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
				}

				return false;
			}

			if (multiAxesIndex < 0 || axesIndex < 0) {
				if (showMissingInputWarningMessagesEnabled) {
					if (multiAxesIndex >= 0) {
						print ("WARNING: The input manager is trying to access to an action which has changed or has been removed in the Group of actions" +
						" called " + multiAxesList [multiAxesIndex].axesName + " \n Please, check the Player Input Manager to configure properly this group of actions according to the " +
						"settings in the main custom Input Manager of GKC" +
						"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
					} else {
						print ("WARNING: The input manager is trying to access to a multi axes which has changed or has been removed.  " +
						"\n Please, check the Player Input Manager to configure properly this group of actions according to the " +
						"settings in the main custom Input Manager of GKC" +
						"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
					}
				}

				return false;
			}

			if (axesIndex >= multiAxesIndexList [multiAxesIndex]) {
				if (showMissingInputWarningMessagesEnabled) {
					print ("WARNING: The input manager is trying to access to an action which has changed or has been removed in the Group of actions" +
					" called " + multiAxesList [multiAxesIndex].axesName + " \n Please, check the Player Input Manager to configure properly this group of actions according to the " +
					"settings in the main custom Input Manager of GKC" +
					"\n Use the Update Input List button on Player Input Manager and make sure to remove any removed action from it where the Axe field appears as empty.\n\n");
				}

				return false;
			}

			if (!multiAxesList [multiAxesIndex].currentlyActive) {
				return false;
			}

			currentJoystickButtonToCheck = multiAxesList [multiAxesIndex].axes [axesIndex];

			//check that the key pressed has being defined as an action
			if (!currentJoystickButtonToCheck.actionEnabled) {
				return false;
			}

			keyIndexPressed = (int)currentJoystickButtonToCheck.joystickButton;

			if (keyIndexPressed == -1) {
				return false;
			}

			currentJoystickButtonIsAxis = (keyIndexPressed >= 10 && keyIndexPressed <= 15);

			if (currentJoystickButtonIsAxis) {
				return getAxisValue (controllerNumber, keyIndexPressed, multiAxesIndex, axesIndex, type);
			}

			if (useMultipleGamepadDictionary) {
				currentJoystickButtonKeyString = getKeyString (controllerNumber, keyIndexPressed);
			} else {
				currentJoystickButtonKeyString = gamepadList [controllerNumber - 1].joystickButtonStringList [keyIndexPressed];
			}
				
			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (Input.GetKey (currentJoystickButtonKeyString)) {
					if (showGamepadPressed) {
						print (controllerNumber + " Get Key: " + currentJoystickButtonKeyString);
					}

					if (showGamepadPressedAction) {
						print (controllerNumber + " Action On Key: " + currentJoystickButtonToCheck.Name);
					}

					return true;
				}
				break;

			case buttonType.getKeyDown:
				//this key is for press once
				if (Input.GetKeyDown (currentJoystickButtonKeyString)) {
					if (showGamepadPressed) {
						print (controllerNumber + " Get Key Down: " + currentJoystickButtonKeyString);
					}

					if (showGamepadPressedAction) {
						print (controllerNumber + " Action On Key Down: " + currentJoystickButtonToCheck.Name);
					}
					return true;
				}
				break;

			case buttonType.getKeyUp:
				//this key is for release 
				if (Input.GetKeyUp (currentJoystickButtonKeyString)) {
					if (showGamepadPressed) {
						print (controllerNumber + " Get Key Up: " + currentJoystickButtonKeyString);
					}

					if (showGamepadPressedAction) {
						print (controllerNumber + " Action On Key Up: " + currentJoystickButtonToCheck.Name);
					}
					return true;
				}
				break;
			}
		}

		return false;
	}

	string currentAxisString;

	public bool getAxisValue (int numberPlayer, int keyPressed, int multiAxesIndex, int axesIndex, buttonType type)
	{
		//TRIGGERS
		if (keyPressed == 14) {
			currentAxisString = gamepadList [numberPlayer - 1].axisButtonStringList [0];

			currentAxisValue = Input.GetAxis (currentAxisString);

			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (currentAxisValue > 0) {
					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}

					return true;
				}
				break;

			case buttonType.getKeyDown:
				if (currentAxisValue > 0) {
					if (!getGamePadLeftTriggersInfoDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadLeftTriggersInfo (numberPlayer, true, multiAxesIndex, axesIndex);

						setGamepadLeftTriggersInfoDown (numberPlayer, true, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Down: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				} else {
					if (getGamePadLeftTriggersInfoDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadLeftTriggersInfoDown (numberPlayer, false, multiAxesIndex, axesIndex);
					}
				}
				break;

			case buttonType.getKeyUp:
				if (currentAxisValue == 0) {
					if (getGamePadLeftTriggersInfo (numberPlayer, multiAxesIndex, axesIndex)) {
						
						setGamepadLeftTriggersInfo (numberPlayer, false, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Up: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				}
				break;
			}
		}

		if (keyPressed == 15) {
			currentAxisString = gamepadList [numberPlayer - 1].axisButtonStringList [1];

			currentAxisValue = Input.GetAxis (currentAxisString);

			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (currentAxisValue > 0) {
					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}

					return true;
				}
				break;

			case buttonType.getKeyDown:
				if (currentAxisValue > 0) {
					if (!getGamePadRightTriggersInfoDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadRightTriggersInfo (numberPlayer, true, multiAxesIndex, axesIndex);

						setGamepadRightTriggersInfoDown (numberPlayer, true, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Down: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				} else {
					if (getGamePadRightTriggersInfoDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadRightTriggersInfoDown (numberPlayer, false, multiAxesIndex, axesIndex);
					}
				}
				break;

			case buttonType.getKeyUp:
				if (currentAxisValue == 0) {
					if (getGamePadRightTriggersInfo (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadRightTriggersInfo (numberPlayer, false, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				}
				break;
			}
		}


		//DPAD X
		if (keyPressed == 10) {
			currentAxisString = gamepadList [numberPlayer - 1].axisButtonStringList [2];

			currentAxisValue = Input.GetAxis (currentAxisString);

			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (currentAxisValue < 0) {
					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}

					return true;
				}
				break;

			case buttonType.getKeyDown:
				if (currentAxisValue < 0) {
					if (!getGamePadDPadXInfoNegativeDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadDPadXInfoNegative (numberPlayer, true, multiAxesIndex, axesIndex);

						setGamepadDPadXInfoNegativeDown (numberPlayer, true, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Down: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				} else {
					if (getGamePadDPadXInfoNegativeDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadDPadXInfoNegativeDown (numberPlayer, false, multiAxesIndex, axesIndex);
					}
				}
				break;

			case buttonType.getKeyUp:
				if (currentAxisValue == 0) {
					if (getGamePadDPadXInfoNegative (numberPlayer, multiAxesIndex, axesIndex)) {

						setGamepadDPadXInfoNegative (numberPlayer, false, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Up: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				}
				break;
			}
		}

		if (keyPressed == 11) {
			currentAxisString = gamepadList [numberPlayer - 1].axisButtonStringList [2];

			currentAxisValue = Input.GetAxis (currentAxisString);

			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (currentAxisValue > 0) {
					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}

					return true;
				}
				break;

			case buttonType.getKeyDown:
				if (currentAxisValue > 0) {
					if (!getGamePadDPadXInfoPositiveDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadDPadXInfoPositive (numberPlayer, true, multiAxesIndex, axesIndex);

						setGamepadDPadXInfoPositiveDown (numberPlayer, true, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Down: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				} else {
					if (getGamePadDPadXInfoPositiveDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadDPadXInfoPositiveDown (numberPlayer, false, multiAxesIndex, axesIndex);
					}
				}
				break;

			case buttonType.getKeyUp:
				if (currentAxisValue == 0) {
					if (getGamePadDPadXInfoPositive (numberPlayer, multiAxesIndex, axesIndex)) {

						setGamepadDPadXInfoPositive (numberPlayer, false, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Up: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				}
				break;
			}
		}


		//DPAD Y
		if (keyPressed == 12) {
			currentAxisString = gamepadList [numberPlayer - 1].axisButtonStringList [3];

			currentAxisValue = Input.GetAxis (currentAxisString);

			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (currentAxisValue < 0) {
					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}

					return true;
				}
				break;

			case buttonType.getKeyDown:
				if (currentAxisValue < 0) {
					if (!getGamePadDPadYInfoNegativeDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadDPadYInfoNegative (numberPlayer, true, multiAxesIndex, axesIndex);

						setGamepadDPadYInfoNegativeDown (numberPlayer, true, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Down: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				} else {
					if (getGamePadDPadYInfoNegativeDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadDPadYInfoNegativeDown (numberPlayer, false, multiAxesIndex, axesIndex);
					}
				}
				break;

			case buttonType.getKeyUp:
				if (currentAxisValue == 0) {
					if (getGamePadDPadYInfoNegative (numberPlayer, multiAxesIndex, axesIndex)) {

						setGamepadDPadYInfoNegative (numberPlayer, false, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Up: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				}
				break;
			}
		}

		if (keyPressed == 13) {
			currentAxisString = gamepadList [numberPlayer - 1].axisButtonStringList [3];

			currentAxisValue = Input.GetAxis (currentAxisString);

			switch (type) {
			//this key is for holding
			case buttonType.getKey:
				if (currentAxisValue > 0) {
					if (showGamepadPressed) {
						print (keyPressed + "-" + numberPlayer);
					}

					if (showGamepadPressedAction) {
						print (numberPlayer + " Action On Key: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
					}

					return true;
				}
				break;

			case buttonType.getKeyDown:
				if (currentAxisValue > 0) {
					if (!getGamePadDPadYInfoPositiveDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadDPadYInfoPositive (numberPlayer, true, multiAxesIndex, axesIndex);

						setGamepadDPadYInfoPositiveDown (numberPlayer, true, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Down: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				} else {
					if (getGamePadDPadYInfoPositiveDown (numberPlayer, multiAxesIndex, axesIndex)) {
						setGamepadDPadYInfoPositiveDown (numberPlayer, false, multiAxesIndex, axesIndex);
					}
				}
				break;

			case buttonType.getKeyUp:
				if (currentAxisValue == 0) {
					if (getGamePadDPadYInfoPositive (numberPlayer, multiAxesIndex, axesIndex)) {

						setGamepadDPadYInfoPositive (numberPlayer, false, multiAxesIndex, axesIndex);

						if (showGamepadPressed) {
							print (keyPressed + "-" + numberPlayer);
						}

						if (showGamepadPressedAction) {
							print (numberPlayer + " Action On Key Up: " + multiAxesList [multiAxesIndex].axes [axesIndex].Name);
						}

						return true;
					}
				}
				break;
			}
		}

		return false;
	}


	//Set and get triggerS state
	public void setGamepadLeftTriggersInfo (int numberPlayer, bool left, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingLeftTrigger = left;
	}

	public void setGamepadRightTriggersInfo (int numberPlayer, bool right, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingRightTrigger = right;
	}

	public void setGamepadLeftTriggersInfoDown (int numberPlayer, bool left, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].leftTriggerDown = left;
	}

	public void setGamepadRightTriggersInfoDown (int numberPlayer, bool right, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].rightTriggerDown = right;
	}

	public bool getGamePadLeftTriggersInfo (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingLeftTrigger;
	}

	public bool getGamePadRightTriggersInfo (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingRightTrigger;
	}

	public bool getGamePadLeftTriggersInfoDown (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].leftTriggerDown;
	}

	public bool getGamePadRightTriggersInfoDown (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].rightTriggerDown;
	}


	//Set and get dpad X state
	public bool getGamePadDPadXInfoPositive (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadXPositive;
	}

	public void setGamepadDPadXInfoPositive (int numberPlayer, bool DPad, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadXPositive = DPad;
	}

	public bool getGamePadDPadXInfoPositiveDown (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].DPadXPositiveDown;
	}

	public void setGamepadDPadXInfoPositiveDown (int numberPlayer, bool DPad, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].DPadXPositiveDown = DPad;
	}

	public bool getGamePadDPadXInfoNegative (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadXNegative;
	}

	public void setGamepadDPadXInfoNegative (int numberPlayer, bool DPad, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadXNegative = DPad;
	}

	public bool getGamePadDPadXInfoNegativeDown (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].DPadXNegativeDown;
	}

	public void setGamepadDPadXInfoNegativeDown (int numberPlayer, bool DPad, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].DPadXNegativeDown = DPad;
	}


	//Set and get dpad Y state
	public bool getGamePadDPadYInfoPositive (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadYPositive;
	}

	public void setGamepadDPadYInfoPositive (int numberPlayer, bool DPad, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadYPositive = DPad;
	}

	public bool getGamePadDPadYInfoPositiveDown (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].DPadYPositiveDown;
	}

	public void setGamepadDPadYInfoPositiveDown (int numberPlayer, bool DPad, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].DPadYPositiveDown = DPad;
	}

	public bool getGamePadDPadYInfoNegative (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadYNegative;
	}

	public void setGamepadDPadYInfoNegative (int numberPlayer, bool DPad, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].usingDPadYNegative = DPad;
	}

	public bool getGamePadDPadYInfoNegativeDown (int numberPlayer, int multiAxesIndex, int axesIndex)
	{
		return gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].DPadYNegativeDown;
	}

	public void setGamepadDPadYInfoNegativeDown (int numberPlayer, bool DPad, int multiAxesIndex, int axesIndex)
	{
		gamepadList [numberPlayer - 1].multiGamepadAxisInfoList [multiAxesIndex].gamepadAxisInfoList [axesIndex].DPadYNegativeDown = DPad;
	}


	public void setRegister ()
	{
		// First use the platform to determine which register to search
		switch (Application.platform) {
		case RuntimePlatform.WindowsPlayer | RuntimePlatform.WindowsEditor:
			register = JoystickData.register_windows;
			break;

		case RuntimePlatform.OSXPlayer | RuntimePlatform.OSXEditor:
			register = JoystickData.register_osx;
			break;

		case RuntimePlatform.LinuxPlayer:
			register = JoystickData.register_linux;
			break;
		}

		register = JoystickData.register_default;
	}

	//	string keyStringFormat = "{0}{1}{2}{3}";

	string getKeyString (int joystickNumber, int keyIndex)
	{
		if (!registerInitialized) {
			setRegister ();
			registerInitialized = true;
		}

		joystickName = "default";

		if (!joystickNamesInitialized) {
			joystickNames = Input.GetJoystickNames ();
			joystickNamesInitialized = true;
		}

		if (joystickNumber > joystickNames.Length) {
			return "";
		}

		joystickName = joystickNames [joystickNumber - 1];
	
		if (!register.ContainsKey (joystickName)) {
			joystickName = "default";
		
			// While we are here, make sure there the requested button is in the default joystick config of the default register
			if (!register [joystickName].ContainsKey ((JoystickData.ButtonTypes)keyIndex)) {
				// The requested button doesn't exist on the default joystick configuration! This is bad!
				return "";
			}
		}
			
		buttonConfig = register [joystickName];

		retString = "joystick " + joystickNumber + " button " + buttonConfig [(JoystickData.ButtonTypes)keyIndex];

//		retString = string.Format (keyStringFormat, "joystick ", joystickNumber, " button ", buttonConfig [(JoystickData.ButtonTypes)keyIndex]);

		return retString;
	}

	//change the current controls to keyboard or mobile
	public void setKeyboardControls (bool state)
	{
		mainGameManager.setUseTouchControlsState (!state);

		pauseManager.setUseTouchControlsState (!state);

		useTouchControls = !state;

		for (int i = 0; i < playerInputManagerList.Count; i++) {
			playerInputManagerList [i].changeControlsType (!state);
		}
	}

	public void setKeyboardControlsFromEditor (bool state)
	{
		mainGameManager.setUseTouchControlsStateFromEditor (!state);

		pauseManager.setUseTouchControlsStateFromEditor (!state);

		useTouchControls = !state;

		for (int i = 0; i < playerInputManagerList.Count; i++) {
			playerInputManagerList [i].changeControlsType (!state);
		}

		updateComponent ();
	}

	public void setUseTouchControlsState (bool state)
	{
		useTouchControls = state;
	}

	public void updateInputInspector ()
	{
		print ("Input Manager Inspector updated");

		updateComponent ();

		GKC_Utils.updateDirtyScene ("Update Input Manager Inspector", gameObject);
	}

	public bool isInTouchButtonToDisableList (GameObject touchButtonGameObject)
	{
		if (buttonsDisabledAtStart.Contains (touchButtonGameObject)) {
			return true;
		}

		return false;
	}

	public void setGamePadList ()
	{
		gamepadList.Clear ();

		int currentGamepadConnectedNameArrayLength = currentGamepadConnectedNameArray.Length;

		int multiAxesListCount = multiAxesList.Count;

		for (int i = 0; i < currentGamepadConnectedNameArrayLength; i++) {
			gamepadInfo newGamepad = new gamepadInfo ();
			newGamepad.gamepadNumber = (i + 1);
			newGamepad.gamepadName = currentGamepadConnectedNameArray [i];

			for (int j = 0; j < multiAxesListCount; j++) {

				multiGamepadAxisInfo newMultiGamepadAxisInfo = new multiGamepadAxisInfo ();

				multiAxes temporaMultiAxes = multiAxesList [j];

				newMultiGamepadAxisInfo.Name = temporaMultiAxes.axesName;

				int axesCount = temporaMultiAxes.axes.Count;

				for (int k = 0; k < axesCount; k++) {
					gamepadAxisInfo newGamepadAxisInfo = new gamepadAxisInfo ();

					newGamepadAxisInfo.Name = temporaMultiAxes.axes [k].Name;
					newMultiGamepadAxisInfo.gamepadAxisInfoList.Add (newGamepadAxisInfo);
				}

				newGamepad.multiGamepadAxisInfoList.Add (newMultiGamepadAxisInfo);
			}
				
			for (int j = 0; j < 20; j++) {
				newGamepad.joystickButtonStringList.Add ("joystick " + (i + 1) + " button " + j);
			}

			newGamepad.axisButtonStringList.Add ("Left Trigger " + (i + 1));
			newGamepad.axisButtonStringList.Add ("Right Trigger " + (i + 1));

			newGamepad.axisButtonStringList.Add ("DPad X" + (i + 1));
			newGamepad.axisButtonStringList.Add ("DPad Y" + (i + 1));

			gamepadList.Add (newGamepad);

//			Debug.Log (currentGamepadConnectedNameArray [i]);
		}

		numberOfGamepads = gamepadList.Count;

		if (numberOfGamepads == 0) {
			usingKeyBoard = true;
			usingGamepad = false;
			//print ("keyboard");
		}

		if (numberOfGamepads >= 1) {
			usingKeyBoard = false;
			usingGamepad = true;
			//print ("gamepad");
		}

		if (numberOfGamepads <= 1) {
			onlyOnePlayer = true;
			//print ("one player");
		}	
	}

	//set the current pause state of the game
	public void setPauseState (bool state)
	{
		gameCurrentlyPaused = state;
	}

	public void enableTouchButtonByName (string touchButtonName)
	{
		enableOrDisableTouchButtonByName (true, touchButtonName);
	}

	public void disableTouchButtonByName (string touchButtonName)
	{
		enableOrDisableTouchButtonByName (false, touchButtonName);
	}

	public void enableOrDisableTouchButtonByName (bool state, string touchButtonName)
	{
		for (int i = 0; i < touchButtonList.Count; i++) {
			if (touchButtonList [i].gameObject.name.Equals (touchButtonName)) {
				if (touchButtonList [i].gameObject.activeSelf != state) {
					touchButtonList [i].gameObject.SetActive (state);
				}

				return;
			}
		}
	}

	public bool isTouchPlatform ()
	{
		return touchPlatform;
	}

	public int currentNumberOfTouchButtonsPressed;

	public void increaseCurrentNumberOfTouchButtonsPressed ()
	{
		currentNumberOfTouchButtonsPressed++;

		print ("increase " + currentNumberOfTouchButtonsPressed);
	}

	public void decreaseCurrentNumberOfTouchButtonsPressed ()
	{
		currentNumberOfTouchButtonsPressed--;

		if (currentNumberOfTouchButtonsPressed < 0) {
			currentNumberOfTouchButtonsPressed = 0;
		}

		print ("decrease " + currentNumberOfTouchButtonsPressed);
	}

	public int getCurrentNumberOfTouchButtonsPressed ()
	{
		return currentNumberOfTouchButtonsPressed;
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

	public void enableMultiAxesInputByName (string multiAxesName)
	{
		enableOrDisableMultiAxesInputByName (true, multiAxesName);
	}

	public void disableMultiAxesInputByName (string multiAxesName)
	{
		enableOrDisableMultiAxesInputByName (false, multiAxesName);
	}

	public void enableOrDisableMultiAxesInputByName (bool state, string multiAxesName)
	{
		int multiAxesIndex = multiAxesList.FindIndex (s => s.axesName.ToLower ().Equals (multiAxesName.ToLower ()));

		if (multiAxesIndex > -1) {
			multiAxesList [multiAxesIndex].currentlyActive = state;
		}
	}

	//EDITOR FUNCTIONS

	//add a new axe to the list
	public void addNewAxe (int axeListIndex)
	{
		Axes newAxe = new Axes ();
		newAxe.Name = "New Action";
		newAxe.joystickButton = joystickButtons.None;

		multiAxesList [axeListIndex].axes.Add (newAxe);

		updateComponent ();
	}

	public void addNewAxesList ()
	{
		multiAxes newMultiAxes = new multiAxes ();
		newMultiAxes.axesName = "New Axes List";

		multiAxesList.Add (newMultiAxes);

		updateComponent ();
	}

	public void removeAxesList (int multiAxesIndex)
	{
		if (multiAxesList.Count > multiAxesIndex) {
			string removeMultiAxesName = multiAxesList [multiAxesIndex].axesName;

			multiAxesList.RemoveAt (multiAxesIndex);

			print ("IMPORTANT: Multi Axes List " + removeMultiAxesName.ToUpper () + " has been removed." +
			"\n Make sure to update the axes list in the Player Input Manager inspector of the main player");

			updateComponent ();
		}
	}

	public int getAxesListIndexByName (string axesName)
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			if (multiAxesList [i].axesName.Equals (axesName)) {
				return i;
			}
		}

		return -1;
	}

	public void removeAxesElement (int multiAxesIndex, int axesIndex)
	{
		if (multiAxesList.Count > multiAxesIndex && multiAxesList [multiAxesIndex].axes.Count > axesIndex) {
			multiAxesList [multiAxesIndex].axes.RemoveAt (axesIndex);

			print ("IMPORTANT: Axes elements in the group of actions called " + multiAxesList [multiAxesIndex].axesName + " has changed." +
			"\n Make sure to update the axes list in the Player Input Manager inspector of the main player");

			updateComponent ();
		}
	}

	public void removeTouchPanel (int touchPanelIndex)
	{
		string touchPanelName = touchButtonsInfoList [touchPanelIndex].Name;

		touchButtonsInfoList.RemoveAt (touchPanelIndex);

		print ("Touch panel  " + touchPanelName + " removed. Updating all touch panel list\n\n");

		touchPanelIndex++;

		for (int i = 0; i < multiAxesList.Count; i++) {

			List<Axes> currentAxesList = multiAxesList [i].axes;

			for (int j = 0; j < currentAxesList.Count; j++) {
				Axes currentAxes = currentAxesList [j];

				if (currentAxes.touchButtonPanelIndex > 0) {
					if (currentAxes.touchButtonPanelIndex == touchPanelIndex) {
						currentAxes.touchButtonPanelIndex = 0;
						currentAxes.touchButtonIndex = 0;

						print ("The touch panel and button assigned to the action " + currentAxes.Name + " has been removed, " +
						"make sure to set a new touch button for that action");
					} else {
						int newTouchPanelIndex = getTouchPanelIndexByName (currentAxes.touchButtonPanel);

//						print ("panel " + currentAxes.touchButtonPanel + " indice obtenido  " + newTouchPanelIndex);

						if (newTouchPanelIndex > -1) {
							newTouchPanelIndex++;

							if (currentAxes.touchButtonPanelIndex != newTouchPanelIndex) {
								currentAxes.touchButtonPanelIndex = newTouchPanelIndex;

								print (currentAxes.Name.ToUpper () + " action updated with touch panel index " + newTouchPanelIndex);
							}
						} else {
							print ("WARNING: Touch Panel called " + currentAxes.touchButtonPanel.ToUpper () + " hasn't been found, make sure to configure an action " +
							"with that name in the main input manager");

							currentAxes.touchButtonPanelIndex = 0;
							currentAxes.touchButtonIndex = 0;
						}
					}
				}
			}
		}

		updateTouchButtonListString ();

		updateComponent ();
	}

	public void removeTouchButton (int touchPanelIndex, int touchButtonIndex)
	{
		touchButtonsInfo currentTouchButtonsInfo = touchButtonsInfoList [touchPanelIndex];

		string touchButtonName = currentTouchButtonsInfo.touchButtonList [touchButtonIndex].name;

		currentTouchButtonsInfo.touchButtonList.RemoveAt (touchButtonIndex);

		print ("Touch button  " + touchButtonName + " removed. Updating " + currentTouchButtonsInfo.Name + " touch panel list\n\n");

		touchPanelIndex++;

		for (int i = 0; i < multiAxesList.Count; i++) {

			List<Axes> currentAxesList = multiAxesList [i].axes;

			for (int j = 0; j < currentAxesList.Count; j++) {
				Axes currentAxes = currentAxesList [j];

//				print (currentAxes.Name + " " + currentAxes.touchButtonPanelIndex + " " + currentAxes.touchButtonIndex);

				if (currentAxes.touchButtonPanelIndex == touchPanelIndex) {

					if (currentAxes.touchButtonIndex == touchButtonIndex) {
						currentAxes.touchButtonPanelIndex = 0;
						currentAxes.touchButtonIndex = 0;

						print ("The touch button assigned to the action " + currentAxes.Name + " has been removed, make sure to set a new touch" +
						" button for that action");
					} else {
						int newTouchButtonIndex = getTouchButtonIndexByName (currentAxes.touchButtonPanel, currentAxes.touchButtonName);

						if (newTouchButtonIndex > -1) {
							if (currentAxes.touchButtonIndex != newTouchButtonIndex) {
								currentAxes.touchButtonIndex = newTouchButtonIndex;

								print (currentAxes.Name.ToUpper () + " action updated with touch button index " + newTouchButtonIndex);
							}
						} else {
							print ("WARNING: Touch Button called " + currentAxes.touchButtonName.ToUpper () + " hasn't been found, make sure to configure an action " +
							"with that name in the main input manager");

							currentAxes.touchButtonPanelIndex = 0;
							currentAxes.touchButtonIndex = 0;
						}
					}
				}
			}
		}

		updateTouchButtonListString ();

		updateComponent ();
	}

	public void getTouchButtonList ()
	{
		checkPlayerInputManagerList ();

		touchButtonList.Clear ();

		playerInputManager currentPlayerInputManager = playerInputManagerList [0];

		currentPlayerInputManager.setTouchPanelActiveState (true);

		touchButtonsInfoList.Clear ();

		currentPlayerInputManager.setTouchPanelActiveState (true);

		List<touchPanelsInfo> touchPanelsInfoList = currentPlayerInputManager.getTouchPanelsList ();

		for (int i = 0; i < touchPanelsInfoList.Count; i++) {

			touchButtonsInfo newTouchButtonsInfo = new touchButtonsInfo ();

			newTouchButtonsInfo.Name = touchPanelsInfoList [i].Name;

			Component[] components = touchPanelsInfoList [i].touchPanel.GetComponentsInChildren (typeof(touchButtonListener));

			foreach (touchButtonListener child in components) {
				newTouchButtonsInfo.touchButtonList.Add (child);

				child.setButtonIconComponent ();

				touchButtonList.Add (child);
			}

			touchButtonsInfoList.Add (newTouchButtonsInfo);
		}

		if (!usingTouchControls) {
			currentPlayerInputManager.setTouchPanelActiveState (false);
		}

		pauseManager.updateTouchButtonsComponents ();

		updateComponent ();
	}

	public void setTouchButtonVisibleState (bool state)
	{
		for (int i = 0; i < touchButtonList.Count; i++) {
			if (touchButtonList [i] != null) {
				if (!buttonsToIgnoreSetVisibleStateList.Contains (touchButtonList [i])) {
					touchButtonList [i].setButtonColorVisibleState (state);
				}
			}
		}

		for (int i = 0; i < playerInputManagerList.Count; i++) {
			playerInputManagerList [i].setJoystickColorVisibleState (state);
		}
	}

	public bool checkNullElementsOnPlayerInputManagerList ()
	{
		if (playerInputManagerList.Count == 0) {
			return true;
		}

		for (int i = 0; i < playerInputManagerList.Count; i++) {
			if (playerInputManagerList [i] == null) {
				return true;
			}
		}

		return false;
	}

	public void toggleTouchButtonsVisibleState ()
	{
		touchButtonsCurrentlyVisible = !touchButtonsCurrentlyVisible;

		setTouchButtonVisibleState (touchButtonsCurrentlyVisible);
	}

	public void getTouchButtonListString ()
	{
		getTouchButtonList ();

		updateTouchButtonListString ();

		updateTouchButtonList ();

		print ("Touch panels and buttons stored properly");
	}

	public void selectMainTouchButtonPanelOnEditor ()
	{
		if (playerInputManagerList.Count > 0 && playerInputManagerList [0] != null) {
			GKC_Utils.setActiveGameObjectInEditor (playerInputManagerList [0].getTouchPanel ());
		}
	}

	public void updateTouchButtonListString ()
	{
		touchButtonListString = new string[touchButtonsInfoList.Count + 1];
		touchButtonListString [0] = "None";

		for (int i = 0; i < touchButtonsInfoList.Count; i++) {
			string newName = touchButtonsInfoList [i].Name;
			touchButtonListString [i + 1] = newName;
		}

		touchButtonsStringInfoList.Clear ();

		touchButtonsStringInfoList.Add (new touchButtonsStringInfo ());

		for (int i = 0; i < touchButtonsInfoList.Count; i++) {

			touchButtonsStringInfo newTouchButtonsStringInfo = new touchButtonsStringInfo ();
			newTouchButtonsStringInfo.Name = touchButtonsInfoList [i].Name;
				
			newTouchButtonsStringInfo.touchButtonListString = new string[touchButtonsInfoList [i].touchButtonList.Count];

			for (int j = 0; j < touchButtonsInfoList [i].touchButtonList.Count; j++) {
				string newName = touchButtonsInfoList [i].touchButtonList [j].gameObject.name;
				newTouchButtonsStringInfo.touchButtonListString [j] = newName;
			}

			touchButtonsStringInfoList.Add (newTouchButtonsStringInfo);
		}
			
		updateComponent ();
	}

	public void updateTouchButtonList ()
	{
		for (int i = 0; i < multiAxesList.Count; i++) {
			
			List<Axes> currentAxesList = multiAxesList [i].axes;

			for (int j = 0; j < currentAxesList.Count; j++) {
				Axes currentAxes = currentAxesList [j];

				if (currentAxes.touchButtonPanelIndex > 0) {
					int newTouchPanelIndex = getTouchPanelIndexByName (currentAxes.touchButtonPanel);

					if (newTouchPanelIndex > -1) {
						newTouchPanelIndex++;

						if (currentAxes.touchButtonPanelIndex != newTouchPanelIndex) {
							currentAxes.touchButtonPanelIndex = newTouchPanelIndex;
						}

						int newTouchButtonIndex = getTouchButtonIndexByName (currentAxes.touchButtonPanel, currentAxes.touchButtonName);

						if (newTouchButtonIndex > -1) {
							if (currentAxes.touchButtonIndex != newTouchButtonIndex) {
								currentAxes.touchButtonIndex = newTouchButtonIndex;

								print ("Multi axes " + multiAxesList [i].axesName + " " + currentAxes.Name.ToUpper () + " updated with index " + newTouchButtonIndex + " for " + currentAxes.touchButtonName);
							}
						} else {
							print ("WARNING: Touch Button called " + currentAxes.touchButtonName.ToUpper () + " hasn't been found, make sure to configure an action " +
							"with that name in the main input manager");

							currentAxes.touchButtonPanelIndex = 0;
							currentAxes.touchButtonIndex = 0;
						}
					} else {
						print ("WARNING: Touch Panel called " + currentAxes.touchButtonPanel.ToUpper () + " hasn't been found, make sure to configure an action " +
						"with that name in the main input manager");

						currentAxes.touchButtonPanelIndex = 0;
						currentAxes.touchButtonIndex = 0;
					}
				}
			}
		}
	
		updateComponent ();
	}

	public int getTouchPanelIndexByName (string touchPanelName)
	{
		for (int i = 0; i < touchButtonsInfoList.Count; i++) {
			if (touchButtonsInfoList [i].Name.Equals (touchPanelName)) {
				return i;
			}
		}

		return -1;
	}


	public int getTouchButtonIndexByName (string touchPanelName, string touchButtonName)
	{
		for (int i = 0; i < touchButtonsInfoList.Count; i++) {

			touchButtonsInfo currentTouchButtonsInfo = touchButtonsInfoList [i];

			if (currentTouchButtonsInfo.Name.Equals (touchPanelName)) {
				for (int j = 0; j < currentTouchButtonsInfo.touchButtonList.Count; j++) {
					if (currentTouchButtonsInfo.touchButtonList [j].name.Equals (touchButtonName)) {
						return j;
					}
				}
			}
		}

		return -1;
	}

	public void showCurrentInputActionList (bool showOnlyKeys, bool showOnlyJoystick)
	{
		currentInputActionList.Clear ();

		currentInputActionList.Add ("TOTAL INPUT LIST AMOUNT: " + multiAxesList.Count + "\n\n\n");

		int numberOfActions = 0;

		int numberOfActionsFound = 0;

		for (int i = 0; i < multiAxesList.Count; i++) {
			string newStringToAdd = "";

			newStringToAdd += multiAxesList [i].axesName.ToUpper () + "\n";
				
			List<Axes> currentAxesList = multiAxesList [i].axes;

			int numberOfActionsFoundInFilter = 0;

			for (int j = 0; j < currentAxesList.Count; j++) {

				Axes currentAxes = currentAxesList [j];

				if (keyFilterActive) {
					if (currentAxes.key == actionKeyToFilter) {
						newStringToAdd += "     -" + currentAxes.Name + "\n";

						numberOfActionsFoundInFilter++;

						numberOfActionsFound++;
					}
				} else if (joystickFilterActive) {
					if (currentAxes.joystickButton == joystickButtonToFilter) {
						newStringToAdd += "     -" + currentAxes.Name + "\n";

						numberOfActionsFoundInFilter++;

						numberOfActionsFound++;
					}
				} else {
					if (showOnlyKeys) {
						newStringToAdd += "     -" + currentAxes.key.ToString () + "\n";
					} else if (showOnlyJoystick) {
						newStringToAdd += "     -" + currentAxes.joystickButton.ToString () + "\n";
					} else {
						newStringToAdd += "     -" + currentAxes.Name + "  ------>  ";
						newStringToAdd += currentAxes.key.ToString ().ToUpper () + "  ------>  ";
						newStringToAdd += currentAxes.joystickButton.ToString ().ToUpper () + " ";
						newStringToAdd += "\n";
					}
				}

				numberOfActions++;
			}

			if ((keyFilterActive || joystickFilterActive) && numberOfActionsFoundInFilter == 0) {
				newStringToAdd = newStringToAdd.Replace ((multiAxesList [i].axesName.ToUpper () + "\n"), "");
			} else {
				newStringToAdd += "\n\n";
			}

			if (newStringToAdd != "") {
				currentInputActionList.Add (newStringToAdd);
			}
		}

		if (keyFilterActive || joystickFilterActive) {
			if (numberOfActionsFound == 0) {
				currentInputActionList.Add ("\n\n NO ACTIONS FOUND\n\n");
			} else {
				currentInputActionList.Add ("\n\n TOTAL ACTIONS FOUND: " + numberOfActionsFound + "\n\n");
			}
		}

		currentInputActionList.Add ("\n\nTOTAL ACTIONS AMOUNT: " + numberOfActions + "\n\n");

		updateComponent ();
	}

	public void showCurrentInputActionListByKeyFilter ()
	{
		keyFilterActive = true;

		joystickFilterActive = false;

		showCurrentInputActionList (false, false);

		keyFilterActive = false;

		updateComponent ();
	}

	public void showCurrentInputActionListByJoystickFilter ()
	{
		joystickFilterActive = true;

		keyFilterActive = false;

		showCurrentInputActionList (false, false);

		joystickFilterActive = false;

		updateComponent ();
	}

	public void clearInputActionListText ()
	{
		currentInputActionList.Clear ();

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class Axes
	{
		public string Name;

		public bool actionEnabled = true;

		public bool showActionInRebindPanel = true;

		public string keyButton;

		public joystickButtons joystickButton;

		public int touchButtonPanelIndex;
		public int touchButtonIndex = 0;
		public string touchButtonName;
		public string touchButtonPanel;

		public int currentTouchButtonIndex;

		public KeyCode key = KeyCode.A;

		//		public string joystickButtonKeyString;
		//		public bool joystickButtonKeyAssigned;

		//some constructors for a key input, incluing name, key button and touch button
		public Axes ()
		{
			Name = "";
			keyButton = "";
			actionEnabled = true;
		}

		public Axes (string n, string key)
		{
			Name = n;
			keyButton = key;
			actionEnabled = true;
		}
	}

	public enum joystickButtons
	{
		A = 0,
		B = 1,
		X = 2,
		Y = 3,
		LeftBumper = 4,
		RightBumper = 5,
		Back = 6,
		Start = 7,
		LeftStickClick = 8,
		RightStickClick = 9,
		LeftDPadX = 10,
		RightDPadX = 11,
		TopDPadY = 12,
		BottomDPadY = 13,
		LeftTrigger = 14,
		RightTrigger = 15,
		None = -1
	}

	[System.Serializable]
	public class gamepadInfo
	{
		public int gamepadNumber;
		public string gamepadName;

		public List<multiGamepadAxisInfo> multiGamepadAxisInfoList = new List<multiGamepadAxisInfo> ();

		public List<string> joystickButtonStringList = new List<string> ();

		public List<string> axisButtonStringList = new List<string> ();
	}

	[System.Serializable]
	public class multiGamepadAxisInfo
	{
		public string Name;
		public List<gamepadAxisInfo> gamepadAxisInfoList = new List<gamepadAxisInfo> ();
	}

	[System.Serializable]
	public class gamepadAxisInfo
	{
		public string Name;

		public bool usingRightTrigger;
		public bool usingLeftTrigger;
		public bool rightTriggerDown;
		public bool leftTriggerDown;

		public bool usingDPadXPositive;
		public bool DPadXPositiveDown;

		public bool usingDPadXNegative;
		public bool DPadXNegativeDown;

		public bool usingDPadYPositive;
		public bool DPadYPositiveDown;

		public bool usingDPadYNegative;
		public bool DPadYNegativeDown;
	}

	[System.Serializable]
	public class multiAxes
	{
		public string axesName;
		public bool currentlyActive = true;
		public List<Axes> axes = new List<Axes> ();

		public bool multiAxesEditPanelActive = true;
	}

	[System.Serializable]
	public class multiAxesEditButtonInput
	{
		public List<editButtonInput> buttonsList = new List<editButtonInput> ();

		public GameObject multiAxesEditPanel;

		public bool multiAxesEditPanelActive;
	}

	[System.Serializable]
	public class touchButtonsInfo
	{
		public string Name;
		public List<touchButtonListener> touchButtonList = new List<touchButtonListener> ();
	}

	[System.Serializable]
	public class touchButtonsStringInfo
	{
		public string Name;
		public string[] touchButtonListString;
	}
}
