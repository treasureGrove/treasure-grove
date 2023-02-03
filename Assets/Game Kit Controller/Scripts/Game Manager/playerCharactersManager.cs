using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using UnityEngine.Events;

public class playerCharactersManager : MonoBehaviour
{
	public bool searchPlayersAtStart;

	public int currentNumberOfPlayers = 1;

	public GameObject mainCharacter;

	public List<playerInfo> playerList = new List<playerInfo> ();

	public string cameraStateNameToUse = "1 Player";

	public List<cameraStates> cameraStatesList = new List<cameraStates> ();

	public List<GameObject> extraCharacterList = new List<GameObject> ();

	public Vector3 newPlayerPositionOffset;

	public Vector2 regularReferenceResolution = new Vector2 (1280, 720);
	public Vector2 splitReferenceResolution = new Vector2 (2560, 1440);

	Vector3 currentCharacterPosition;

	public List<GameObject> auxPlayerList = new List<GameObject> ();

	public playerInfo currentPlayeraInfoActive;

	public bool setCurrentCharacterToControlAndAIAtStart;

	public string currentCharacterToControlName = "Player 1";

	public float delayTimeToChangeBetweenCharacters = 1;

	int currentCharacterToControlIndex;

	public string[] cameraStatesListString;
	public int currentCameraStateIndex;

	bool characterToControlAtStartConfigured;

	float lastTimeCharacterChange;

	playerInfo previousCharacterInfoActive;

	public bool configureLocalMultiplayerCharactersOnAwake;
	public int numberOfPlayersOnAwake;
	public string cameraStateNameOnAwake;

	public dynamicSplitScreenSystem mainDynamicSplitScreenSystem;

	public GameObject extraPlayerPrefab;

	public Transform initialMultiplayerSpawnPosition;


	static List<playerInfo> playerListValue = new List<playerInfo> ();
	static int playerListCount;


	void Awake ()
	{
		if (configureLocalMultiplayerCharactersOnAwake) {
			currentNumberOfPlayers = numberOfPlayersOnAwake;

			cameraStateNameToUse = cameraStateNameOnAwake;
		}

		checkPlayerElementsOnScene ();

		if (searchPlayersAtStart) {
			searchPlayersOnTheLevel (false, false);
		}

		if (playerList.Count >= 1) {
			currentPlayeraInfoActive = playerList [0];
		}

		if (configureLocalMultiplayerCharactersOnAwake) {
			if (mainCharacter != null) {
				setNumberOfPlayers (false);
			} else {
				addExtraPlayerOnSceneForMultiplayer (currentNumberOfPlayers);
			}

			setCameraConfiguration (false);

			updateHUD (false);

			setPlayerID (false);

			assignMapSystem (false);
		}

		playerListValue = playerList;

		playerListCount = playerList.Count;
	}

	void Start ()
	{
		if (setCurrentCharacterToControlAndAIAtStart) {
			StartCoroutine (checkInitialStateOnCharacters ());
		}
	}

	IEnumerator checkInitialStateOnCharacters ()
	{
		yield return new WaitForSeconds (0.1f);

		if (setCurrentCharacterToControlAndAIAtStart && !characterToControlAtStartConfigured) {
			if (playerList.Count > 1) {
				setAsCurrentCharacterToControlByName (currentCharacterToControlName);

				characterToControlAtStartConfigured = true;
			}
		}
	}

	public void checkPlayerElementsOnScene ()
	{
		bool checkingEmptyPlayerElements = checkNotNullPlayerElements ();

		if (checkingEmptyPlayerElements) {
			searchPlayersOnTheLevel (false, false);
		}
	}

	public void addExtraPlayerOnSceneForMultiplayer (int numberOfExtraPlayers)
	{
		if (extraPlayerPrefab != null) {
			int newNumberOfPlayers = playerList.Count;

			for (int i = 0; i < numberOfExtraPlayers; i++) {
				GameObject newCharacter = (GameObject)Instantiate (extraPlayerPrefab, Vector3.zero, Quaternion.identity);

				if (mainCharacter == null) {
					mainCharacter = newCharacter;
				}

				int numberOfPlayers = playerList.Count;

				newNumberOfPlayers++;

				newCharacter.name = "Player " + newNumberOfPlayers;
				newCharacter.transform.SetParent (transform);
			}

			updatePlayersInfoOnTheLevel ();

			Vector3 newPosition = Vector3.zero;
			Quaternion newRotation = Quaternion.identity;

			if (initialMultiplayerSpawnPosition != null) {
				newPosition = initialMultiplayerSpawnPosition.position;
				newRotation = initialMultiplayerSpawnPosition.rotation;
			}

			for (int i = 0; i < playerList.Count; i++) {
				playerInfo newPlayerInfo = playerList [i];

				newPlayerInfo.playerControllerGameObject.transform.position = newPosition;
				newPlayerInfo.playerCameraGameObject.transform.position = newPosition;

				newPlayerInfo.playerControllerGameObject.transform.rotation = newRotation;
				newPlayerInfo.playerCameraGameObject.transform.rotation = newRotation;

				newPosition += newPlayerPositionOffset;
			}
		}
	}

	public void setNewPositionToPlayersOnScene (List<Transform> positionList)
	{
		int numberOfPlayers = playerList.Count;

		for (int i = 0; i < numberOfPlayers; i++) {
			if (playerList [i].playerControllerGameObject != null) {
				Transform currentCharacter = playerList [i].playerControllerGameObject.transform;

				if (currentCharacter != null && positionList.Count <= i) {
					currentCharacter.position = positionList [i].position;

					currentCharacter.rotation = positionList [i].rotation;
				}
			}
		}
	}

	public void updatePlayersInfoOnTheLevel ()
	{
		searchPlayersOnTheLevel (false, false);
	}

	public bool checkNotNullPlayerElements ()
	{
		if (playerList.Count == 0) {
			return true;
		}

		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].playerControllerGameObject == null) {
				return true;
			}
		}

		return false;
	}

	public void searchPlayersOnTheLevel (bool creatingNewPlayers, bool creatingCharactersOnEditor)
	{
		playerController mainPlayerController = null;

		if (creatingNewPlayers && creatingCharactersOnEditor) {
			if (playerList.Count > 0) {
				if (playerList [0].playerControllerManager != null) {
					mainPlayerController = playerList [0].playerControllerManager;
				}
			}
		}

		playerList.Clear ();

		auxPlayerList.Clear ();

		currentCharacterPosition = Vector3.zero;

		playerController[] childrens = FindObjectsOfType<playerController> (); 

		foreach (playerController c in childrens) {
			if (!c.isCharacterUsedByAI ()) {
				auxPlayerList.Add (c.gameObject);
			}
		}

		if (creatingNewPlayers && creatingCharactersOnEditor) {
			if (mainPlayerController != null) {
				auxPlayerList.Remove (mainPlayerController.gameObject);

				auxPlayerList.Insert (0, mainPlayerController.gameObject);

				print ("original player asssigned at start");

				for (int i = 0; i < auxPlayerList.Count; i++) {
					print (auxPlayerList [i].name);
				}
			}
		}

		for (int i = 0; i < auxPlayerList.Count; i++) {
			playerInfo newPlayerInfo = new playerInfo ();

			newPlayerInfo.Name = "Player " + (i + 1);
			newPlayerInfo.playerControllerGameObject = auxPlayerList [i];

			newPlayerInfo.mainPlayerComponentsManager = newPlayerInfo.playerControllerGameObject.GetComponent<playerComponentsManager> ();

			newPlayerInfo.playerControllerManager = newPlayerInfo.mainPlayerComponentsManager.getPlayerController ();

			newPlayerInfo.playerControllerManager.setPlayerID (i + 1);

			newPlayerInfo.playerCameraGameObject = newPlayerInfo.playerControllerManager.getPlayerCameraGameObject ();

			newPlayerInfo.playerCameraManager = newPlayerInfo.mainPlayerComponentsManager.getPlayerCamera ();

			newPlayerInfo.playerInput = newPlayerInfo.mainPlayerComponentsManager.getPlayerInputManager ();

			newPlayerInfo.pauseManager = newPlayerInfo.mainPlayerComponentsManager.getPauseManager ();

			newPlayerInfo.mainFriendListManager = newPlayerInfo.mainPlayerComponentsManager.getFriendListManager ();

			newPlayerInfo.mainSaveGameSystem = newPlayerInfo.mainPlayerComponentsManager.getSaveGameSystem ();

			newPlayerInfo.playerParentGameObject = newPlayerInfo.pauseManager.transform.parent.gameObject;
			newPlayerInfo.playerParentGameObject.name = newPlayerInfo.Name;

			if (i == 0) {
				currentCharacterPosition = newPlayerInfo.playerControllerGameObject.transform.position;
			}

			if (creatingNewPlayers) {
				if (newPlayerInfo.playerParentGameObject != mainCharacter) {
					currentCharacterPosition += newPlayerPositionOffset;
					newPlayerInfo.playerControllerGameObject.transform.position = currentCharacterPosition;
					newPlayerInfo.playerCameraGameObject.transform.position = currentCharacterPosition;
				}
			}

			playerList.Add (newPlayerInfo);
		}

		if (creatingCharactersOnEditor) {
			updateComponent ();
		}
	}

	public void setExtraCharacterList ()
	{
		extraCharacterList.Clear ();

		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].playerParentGameObject != mainCharacter) {

				extraCharacterList.Add (playerList [i].playerParentGameObject);
			}
		}

		updateComponent ();
	}

	public void setPlayerID (bool creatingCharactersOnEditor)
	{
		int numberOfPlayerID = 0;

		for (int i = 0; i < playerList.Count; i++) {
			playerInfo currentPlayerInfo = playerList [i];

			if (currentPlayerInfo.playerInput.useOnlyKeyboard) {
				numberOfPlayerID--;
			}
				
			currentPlayerInfo.playerControllerManager.setPlayerID (i + 1 + numberOfPlayerID);

			if (creatingCharactersOnEditor) {
				GKC_Utils.updateComponent (currentPlayerInfo.playerControllerManager);
			}
		}

		print ("Player IDs configured");

		if (creatingCharactersOnEditor) {
			updateComponent ();
		}
	}

	public void setNumberOfPlayers (bool creatingCharactersOnEditor)
	{
		int newNumberOfPlayers = currentNumberOfPlayers - 1 - extraCharacterList.Count;

		if (newNumberOfPlayers > 0) {
			for (int i = 0; i < newNumberOfPlayers; i++) {
				GameObject newCharacter = (GameObject)Instantiate (mainCharacter, mainCharacter.transform.position, mainCharacter.transform.rotation);
				newCharacter.name = "Player " + (i + 2);
				newCharacter.transform.SetParent (transform);

				extraCharacterList.Add (newCharacter);
			}
		} else {
			newNumberOfPlayers = Mathf.Abs (newNumberOfPlayers);

			print (newNumberOfPlayers);

			newNumberOfPlayers = extraCharacterList.Count - newNumberOfPlayers;

			for (int i = extraCharacterList.Count - 1; i >= newNumberOfPlayers; i--) {
				DestroyImmediate (extraCharacterList [i]);
			}

			for (int i = 0; i < extraCharacterList.Count; i++) {
				if (extraCharacterList [i] == null) {
					extraCharacterList.RemoveAt (i);
					i = 0;
				}
			}

			if (currentNumberOfPlayers == 1) {
				extraCharacterList.Clear ();
			}
		}

		searchPlayersOnTheLevel (true, creatingCharactersOnEditor);

		print ("Players configured: " + playerList.Count);

		if (creatingCharactersOnEditor) {
			updateComponent ();
		}
	}

	public void setCameraConfiguration (bool creatingCharactersOnEditor)
	{
		for (int i = 0; i < playerList.Count; i++) {

			List<Camera> currentCameraList = playerList [i].playerCameraManager.getCameraList ();

			cameraStates currentCameraStates = getCameraStateByName (cameraStateNameToUse);
				
			if (currentCameraStates != null && currentCameraStates.cameraInfoList.Count >= currentNumberOfPlayers) {
				for (int j = 0; j < currentCameraList.Count; j++) {
			
					currentCameraList [j].rect = new Rect (currentCameraStates.cameraInfoList [i].newX, currentCameraStates.cameraInfoList [i].newY, 
						currentCameraStates.cameraInfoList [i].newW, currentCameraStates.cameraInfoList [i].newH);

					if (creatingCharactersOnEditor) {
						GKC_Utils.updateComponent (currentCameraList [j]);
					}
				}

				menuPause currentPauseManager = playerList [i].pauseManager;


				Camera canvasCamera = currentPauseManager.getMainCanvasCamera ();

				bool activatingMultiplayer = currentNumberOfPlayers > 1;

				canvasCamera.gameObject.SetActive (activatingMultiplayer);

				List<menuPause.canvasInfo> currentCanvasInfoList = currentPauseManager.getCanvasInfoList (); 

				if (currentCanvasInfoList.Count == 0) {
					currentPauseManager.addNewCanvasToList (currentPauseManager.getMainCanvas ());

					currentCanvasInfoList = currentPauseManager.getCanvasInfoList (); 
				}

				for (int k = 0; k < currentCanvasInfoList.Count; k++) {
					if (currentCanvasInfoList [k].mainCanvas != null) {
						currentCanvasInfoList [k].mainCanvas.worldCamera = canvasCamera;

						if (activatingMultiplayer) {
							currentCanvasInfoList [k].mainCanvasScaler.referenceResolution = splitReferenceResolution;
							currentCanvasInfoList [k].mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
						} else {
							currentCanvasInfoList [k].mainCanvasScaler.referenceResolution = regularReferenceResolution;
							currentCanvasInfoList [k].mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
						}

						if (creatingCharactersOnEditor) {
							GKC_Utils.updateComponent (currentCanvasInfoList [k].mainCanvasScaler);

							GKC_Utils.updateComponent (currentCanvasInfoList [k].mainCanvas);
						}
					}
				}
			} else {
				print ("WARNING: Camera state called " + cameraStateNameToUse + " is not assigned");
			}
				
			if (i > 0) {
				playerList [i].playerCameraManager.enableOrDisableCameraAudioListener (false);
			}
		}

		print ("Player cameras configured");
	}

	public void updateHUD (bool creatingCharactersOnEditor)
	{
		for (int i = 0; i < playerList.Count; i++) {

			playerList [i].pauseManager.setCanvasInfo (creatingCharactersOnEditor);
		}

		print ("Player HUD Updated");
	}

	public void assignMapSystem (bool creatingCharactersOnEditor)
	{
		for (int i = 0; i < playerList.Count; i++) {

			mapCreator mapCreatorManager = FindObjectOfType<mapCreator> ();

			if (mapCreatorManager != null) {
				mapCreatorManager.addNewPlayer (playerList [i].pauseManager.gameObject.GetComponent<mapSystem> (), 
					playerList [i].playerControllerGameObject, creatingCharactersOnEditor);
			} else {
				print ("WARNING: There is no map creator in the scene, make sure to drag and drop the prefab and configure as you need if you want to use the map system");
			}
		}

		print ("Map systems configured");
	}

	public void enableOrDisableSingleCameraState (bool state, bool creatingCharactersOnEditor)
	{
		List<Transform> multipleTargetsToFollowList = new List<Transform> ();

		for (int i = 0; i < playerList.Count; i++) {
			multipleTargetsToFollowList.Add (playerList [i].playerControllerGameObject.transform);
		}

		for (int i = 0; i < playerList.Count; i++) {
			playerList [i].playerCameraManager.setFollowingMultipleTargetsState (state, creatingCharactersOnEditor);

			if (state) {
				playerList [i].playerCameraManager.setMultipleTargetsToFollowList (multipleTargetsToFollowList, creatingCharactersOnEditor);
			} else {
				playerList [i].playerCameraManager.setMultipleTargetsToFollowList (null, creatingCharactersOnEditor);
			}
		}

		print ("Single Camera State configured as " + state);

		updateComponent ();

		GKC_Utils.updateDirtyScene ("Set Single Camera State", gameObject);
	}

	public void enableOrDisableDynamicSplitCameraState (bool state, bool creatingCharactersOnEditor)
	{
		if (playerList.Count < 2) {
			mainDynamicSplitScreenSystem.setDynamicSplitScreenActiveState (false);

			playerList [0].pauseManager.enableOrDisableDynamicElementsOnScreen (!state);
		} else {

			mainDynamicSplitScreenSystem.setDynamicSplitScreenActiveState (state, playerList [0].playerControllerGameObject, 
				playerList [1].playerControllerGameObject, creatingCharactersOnEditor);

			playerList [0].pauseManager.enableOrDisableDynamicElementsOnScreen (!state);
	
			playerList [1].pauseManager.enableOrDisableDynamicElementsOnScreen (!state);
		}

		if (creatingCharactersOnEditor) {
			GKC_Utils.updateDirtyScene ("Enabling or disabling dynamic split screen " + state, gameObject);
		}
	}

	public cameraStates getCameraStateByName (string cameraStateName)
	{
		for (int j = 0; j < cameraStatesList.Count; j++) {
			if (cameraStatesList [j].Name.Equals (cameraStateName)) {
				return cameraStatesList [j];
			}
		}

		return null;
	}

	public void setAsCurrentPlayerActive (string playerName)
	{
		currentPlayeraInfoActive.playerCameraGameObject.transform.SetParent (currentPlayeraInfoActive.pauseManager.transform);
		currentPlayeraInfoActive.playerControllerGameObject.transform.SetParent (currentPlayeraInfoActive.pauseManager.transform);

		currentPlayeraInfoActive.playerParentGameObject.SetActive (false);

		Vector3 playerPosition = currentPlayeraInfoActive.playerControllerGameObject.transform.position;
		Quaternion playerRotation = currentPlayeraInfoActive.playerControllerGameObject.transform.rotation;

		Quaternion cameraRotation = currentPlayeraInfoActive.playerCameraGameObject.transform.rotation;

		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].Name.Equals (playerName)) {
				currentPlayeraInfoActive = playerList [i];
			}
		}
			
		currentPlayeraInfoActive.playerControllerGameObject.transform.position = playerPosition;
		currentPlayeraInfoActive.playerControllerGameObject.transform.rotation = playerRotation;

		currentPlayeraInfoActive.playerCameraGameObject.transform.rotation = cameraRotation;

		currentPlayeraInfoActive.playerParentGameObject.SetActive (true);
	}

	public void setAsCurrentCharacterToControlByName (string playerName)
	{
		previousCharacterInfoActive = null;

		if (currentPlayeraInfoActive != null) {
			previousCharacterInfoActive = currentPlayeraInfoActive;
		}

		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].Name.Equals (playerName)) {

				Vector3 cameraPosition = previousCharacterInfoActive.playerCameraManager.getMainCamera ().transform.position;
				Quaternion cameraRotation = previousCharacterInfoActive.playerCameraManager.getMainCamera ().transform.rotation;

				playerList [i].playerCameraManager.setPreviousCharacterControl (previousCharacterInfoActive.playerControllerGameObject, cameraPosition, 
					cameraRotation, previousCharacterInfoActive.playerCameraManager.isFirstPersonActive ());
			}
		}

		for (int i = 0; i < playerList.Count; i++) {
			playerList [i].mainFriendListManager.removeAllFriends ();

			playerList [i].playerInput.switchPlayerID (1);

			if (playerList [i].Name.Equals (playerName)) {
				currentPlayeraInfoActive = playerList [i];

				currentPlayeraInfoActive.mainFriendListManager.eventToSetCharacterAsPlayer.Invoke ();

				playerList [i].eventToSetCharacterAsPlayer.Invoke ();

				currentPlayeraInfoActive.useByAI = false;

				currentCharacterToControlIndex = i;

			} else {
				playerList [i].mainFriendListManager.eventToSetCharacterAsAI.Invoke ();

				playerList [i].eventToSetCharacterAsAI.Invoke ();

				playerList [i].useByAI = true;
			}
		}

		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].playerControllerGameObject != currentPlayeraInfoActive.playerControllerGameObject) {	
				findObjectivesSystem currentfindObjectivesSystem = playerList [i].playerControllerGameObject.GetComponentInChildren<findObjectivesSystem> ();

				if (currentfindObjectivesSystem != null) {

					if (currentfindObjectivesSystem.isPartnerFound ()) {
						currentfindObjectivesSystem.removePartner ();
					}
						
					if (playerList [i].followMainPlayerOnSwitchCompanion) {
						currentfindObjectivesSystem.addPlayerAsPartner (currentPlayeraInfoActive.playerControllerGameObject);
					} else {
						currentfindObjectivesSystem.removeCurrentPartner ();
					}
				}
			}
		}
	}

	public void toogleUseOnlyKeyboardGamepad (string playerName)
	{
		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].Name.Equals (playerName)) {

				playerList [i].useOnlyKeyboard = !playerList [i].useOnlyKeyboard;

				playerList [i].playerInput.useOnlyKeyboard = playerList [i].useOnlyKeyboard;

				GKC_Utils.updateComponent (playerList [i].playerInput);
			}
		}

		setPlayerID (true);

		GKC_Utils.updateDirtyScene ("Toggle use of keyboard or gamepad on player " + playerName, gameObject);
	}

	public void setNextCharacterToControl ()
	{
		if (playerList.Count == 1) {
			return;
		}

		currentCharacterToControlIndex++;

		if (currentCharacterToControlIndex >= playerList.Count) {
			currentCharacterToControlIndex = 0;
		}

		setAsCurrentCharacterToControlByName (playerList [currentCharacterToControlIndex].Name);
	}

	public void setAsCurrentCharacterToControlByPlayerGameObject (GameObject playerGameObjectToSearch)
	{
		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].playerControllerGameObject == playerGameObjectToSearch) {
				setAsCurrentCharacterToControlByName (playerList [i].Name);

				return;
			}
		}
	}

	public void setAsCurrentCharacterToControlByPlayerTransform (Transform playerTransformToSearch)
	{
		if (playerTransformToSearch != null) {
			setAsCurrentCharacterToControlByPlayerGameObject (playerTransformToSearch.gameObject);
		}
	}

	public menuPause getPauseManagerFromPlayerByIndex (int index)
	{
		if (index < playerList.Count) {
			return playerList [index].pauseManager;
		}

		return null;
	}

	public void setCharactersManagerPauseState (bool state)
	{
		for (int i = 0; i < playerList.Count; i++) {

			if (!playerList [i].useByAI) {

				if (playerList [i].pauseManager != null && playerList [i].pauseManager.isMenuPaused () != state) {
					playerList [i].pauseManager.setMenuPausedState (state);
				}
			}
		}
	}

	public bool isCharacterInList (GameObject characterToSearch)
	{
		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].playerControllerGameObject == characterToSearch) {
				return true;
			}
		}

		return false;
	}

	public bool anyCharacterDead ()
	{
		for (int i = 0; i < playerList.Count; i++) {
			if (playerList [i].playerControllerManager.isPlayerDead ()) {
				return true;
			}
		}

		return false;
	}



	public static void checkPanelsActiveOnGamepadOrKeyboard (bool checkKeyboard, int playerID)
	{
		for (int i = 0; i < playerListCount; i++) {
			if (playerListValue [i].playerControllerManager.getPlayerID () == playerID) {

				playerListValue [i].playerInput.checkPanelsActiveOnGamepadOrKeyboard (checkKeyboard);

				return;
			}
		}
	}

	public playerController getMainPlayerController ()
	{
		if (playerList.Count >= 1) {
			return playerList [0].playerControllerManager;
		}

		return null;
	}

	public GameObject getMainPlayerGameObject ()
	{
		if (playerList.Count >= 1) {
			return playerList [0].playerControllerGameObject;
		}

		return null;
	}

	public playerCamera getMainPlayerCamera ()
	{
		if (playerList.Count >= 1) {
			return playerList [0].playerCameraManager;
		}

		return null;
	}

	public Camera getMainPlayerCameraComponent ()
	{
		if (playerList.Count >= 1) {
			return playerList [0].playerCameraManager.getMainCamera ();
		}

		return null;
	}

	public Transform getMainPlayerCameraTransform ()
	{
		if (playerList.Count >= 1) {
			return playerList [0].playerCameraManager.getMainCamera ().transform;
		}

		return null;
	}

	public Transform getMainPlayerTransform ()
	{
		if (playerList.Count >= 1) {
			return playerList [0].playerControllerGameObject.transform;
		}

		return null;
	}

	public void getCameraStateListString ()
	{
		cameraStatesListString = new string[cameraStatesList.Count];

		for (int i = 0; i < cameraStatesList.Count; i++) {
			string newName = cameraStatesList [i].Name;

			cameraStatesListString [i] = newName;
		}

		updateComponent ();
	}

	public void inputSetNextCharacterToControl ()
	{
		if (Time.time > lastTimeCharacterChange + delayTimeToChangeBetweenCharacters) {
			if (currentPlayeraInfoActive != null) {
				if (currentPlayeraInfoActive.playerControllerManager.playerIsBusy () ||
				    currentPlayeraInfoActive.playerControllerManager.isPlayerDriving ()) {
					return;
				}
			}

			setNextCharacterToControl ();

			lastTimeCharacterChange = Time.time;
		}
	}

	public int getPlayerListCount ()
	{
		return playerList.Count;
	}

	public void checkGameInfoToLoad (int lastSaveNumber, string currentSaveDataPath, string extraFileString)
	{
		if (getPlayerListCount () > 1) {
			for (int i = 0; i < playerList.Count; i++) {
				playerList [i].mainSaveGameSystem.checkGameInfoToLoad (lastSaveNumber, currentSaveDataPath, extraFileString);
			}
		} else {
			if (getPlayerListCount () == 1) {
				playerList [0].mainSaveGameSystem.checkGameInfoToLoad (lastSaveNumber, currentSaveDataPath, extraFileString);
			}
		}
	}

	public void checkComponentsToInitialize ()
	{
		checkPlayerElementsOnScene ();

		if (getPlayerListCount () > 1) {
			for (int i = 0; i < playerList.Count; i++) {
				playerList [i].mainSaveGameSystem.checkComponentsToInitialize ();
			}
		} else {
			if (getPlayerListCount () == 1) {
				playerList [0].mainSaveGameSystem.checkComponentsToInitialize ();
			}
		}
	}

	public void saveGameInfoFromEditor (int lastSaveNumber, string infoTypeName, string extraFileString)
	{
		if (getPlayerListCount () > 1) {
			for (int i = 0; i < playerList.Count; i++) {
				playerList [i].mainSaveGameSystem.saveGameInfoFromEditor (lastSaveNumber, infoTypeName, extraFileString);
			}
		} else {
			if (getPlayerListCount () == 1) {
				playerList [0].mainSaveGameSystem.saveGameInfoFromEditor (lastSaveNumber, infoTypeName, extraFileString);
			}
		}
	}

	public void saveGameWhenReturningHomeMenu ()
	{
		if (getPlayerListCount () > 1) {
			for (int i = 0; i < playerList.Count; i++) {
				playerList [i].mainSaveGameSystem.saveGameWhenReturningHomeMenu ();
			}
		} else {
			if (getPlayerListCount () == 1) {
				playerList [0].mainSaveGameSystem.saveGameWhenReturningHomeMenu ();
			}
		}
	}

	public void checkIfPlayerIsLookingAtDeadTarget (Transform deadTarget, Transform placeToShoot)
	{
		for (int i = 0; i < playerList.Count; i++) {
				
			playerCamera currentPlayerCamera = playerList [i].playerCameraManager;

			if (currentPlayerCamera.isPlayerLookingAtTarget ()) {
				Transform currentTargetToLook = currentPlayerCamera.getCurrentTargetToLook ();

				bool isSameObject = false;
				if (currentTargetToLook == deadTarget) {
					isSameObject = true;
				} 

				if (!isSameObject) {
					if (currentTargetToLook == placeToShoot) {
						isSameObject = true;
					}
				}

				if (!isSameObject) {
					if (applyDamage.checkIfWeakSpotListContainsTransform (deadTarget.gameObject, currentTargetToLook)) {
						isSameObject = true;
					}
				}

				if (isSameObject) {
					print ("target found, disabling look at target");
					currentPlayerCamera.checkIfPlayerIsLookingAtDeadTarget (false);

					currentPlayerCamera.checkEventOnLockOnEnd ();
				} else {
					print ("TARGET NOT FOUND");
				}
			}
		}
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class playerInfo
	{
		public string Name;
		public GameObject playerParentGameObject;
		public GameObject playerControllerGameObject;
		public GameObject playerCameraGameObject;
		public playerController playerControllerManager;
		public playerCamera playerCameraManager;
	
		public playerInputManager playerInput;
		public menuPause pauseManager;

		public playerComponentsManager mainPlayerComponentsManager;
		public friendListManager mainFriendListManager;

		public saveGameSystem mainSaveGameSystem;

		public bool useByAI;

		public bool showEventSettings;
		public UnityEvent eventToSetCharacterAsAI;
		public UnityEvent eventToSetCharacterAsPlayer;

		public bool useOnlyKeyboard;

		public bool followMainPlayerOnSwitchCompanion = true;
	}

	[System.Serializable]
	public class cameraStates
	{
		public string Name;
		public int numberfOfPlayers;

		public List<cameraInfo> cameraInfoList = new List<cameraInfo> ();
	}

	[System.Serializable]
	public class cameraInfo
	{
		public float newX = 0;
		public float newY = 0;
		public float newW = 0;
		public float newH = 0;
	}
}
