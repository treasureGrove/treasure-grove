using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerInputPanelSystem : MonoBehaviour
{
	public bool screenActionPanelsEnabled = true;

	public bool screenActionPanelsActive = true;

	public List<screenActionInfo> screenActionInfoList = new List<screenActionInfo> ();

	public List<touchPanelsInfo> touchPanelsInfoList = new List<touchPanelsInfo> ();
	public List<touchPanelsSchemesInfo> touchPanelsSchemesInfoList = new List<touchPanelsSchemesInfo> ();

	public GameObject screenActionParent;

	public playerInputManager playerInput;

	public GameObject inputPanelPrefab;

	public inputPanelUISystem mainInputPanelUISystem;

	public bool instantiateInputPanelUIIfNotFound = true;

	public menuPause pauseManager;


	touchPanelsSchemesInfo currentTouchPanelsSchemesInfo;
	touchPanelsSchemesInfo previousTouchPanelsSchemesInfo;

	screenActionInfo currentScreenActionInfo;
	screenActionInfo previousScreenActionInfo;

	bool mainInputPanelUISystemFound;

	bool panelParentActivePreviously;


	void getInputPanelUISystem ()
	{
		if (mainInputPanelUISystem != null) {
			mainInputPanelUISystemFound = true;
		} else {
			if (mainInputPanelUISystem == null) {
				mainInputPanelUISystem = FindObjectOfType<inputPanelUISystem> ();
			}

			if (mainInputPanelUISystem != null) {
				mainInputPanelUISystemFound = true;
			}
		}

		if (mainInputPanelUISystemFound) {
			screenActionInfoList = mainInputPanelUISystem.screenActionInfoList;
		}
	}

	void checkInputPanelUISystem ()
	{
		if (!mainInputPanelUISystemFound) {
			bool tutorialFound = false;

			if (instantiateInputPanelUIIfNotFound) {
				instantiateInputPanelPrefab ();

				getInputPanelUISystem ();

				if (mainInputPanelUISystemFound) {
					tutorialFound = true;
				}
			} 

			if (!tutorialFound) {
				print ("WARNING: No Input Panel UI system found, make sure to drop it on the scene");

				return;
			}
		}
	}

	public void setInputPanelGameObjectActiveState (bool state)
	{
		if (mainInputPanelUISystemFound) {
			mainInputPanelUISystem.setInputPanelGameObjectActiveState (state);
		}
	}

	//Touch panels functions
	public List<touchPanelsInfo> getTouchPanelsList ()
	{
		return touchPanelsInfoList;
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
		for (int i = 0; i < touchPanelsInfoList.Count; i++) {
			if (touchPanelsInfoList [i].Name.Equals (panelName) && touchPanelsInfoList [i].touchPanel.activeSelf != state) {
				touchPanelsInfoList [i].touchPanel.SetActive (state);
			}
		}
	}

	public void enableTouchPanelSchemeByName (string panelName)
	{
		bool isMainGameTouchPanel = false;

		for (int i = 0; i < touchPanelsSchemesInfoList.Count; i++) {
			if (touchPanelsSchemesInfoList [i].Name.Equals (panelName)) {

				if (previousTouchPanelsSchemesInfo != currentTouchPanelsSchemesInfo) {
					previousTouchPanelsSchemesInfo = currentTouchPanelsSchemesInfo;
				}

				currentTouchPanelsSchemesInfo = touchPanelsSchemesInfoList [i];

				currentTouchPanelsSchemesInfo.currentTouchPanelScheme = true;

				for (int j = 0; j < touchPanelsInfoList.Count; j++) {
					if (currentTouchPanelsSchemesInfo.enabledPanels.Contains (touchPanelsInfoList [j].touchPanel)) {
						if (!touchPanelsInfoList [j].touchPanel.activeSelf) {
							touchPanelsInfoList [j].touchPanel.SetActive (true);
						}
					} else {
						if (touchPanelsInfoList [j].touchPanel.activeSelf) {
							touchPanelsInfoList [j].touchPanel.SetActive (false);
						}
					}
				}

				if (currentTouchPanelsSchemesInfo.isMainGameTouchPanel) {
					currentTouchPanelsSchemesInfo.mainGameTouchPanelActive = true;

					isMainGameTouchPanel = true;
				}
			} else {
				touchPanelsSchemesInfoList [i].currentTouchPanelScheme = false;
			}
		}

		if (isMainGameTouchPanel) {
			for (int i = 0; i < touchPanelsSchemesInfoList.Count; i++) {
				if (touchPanelsSchemesInfoList [i].Name != panelName) {
					if (touchPanelsSchemesInfoList [i].isMainGameTouchPanel) {
						touchPanelsSchemesInfoList [i].mainGameTouchPanelActive = false;
					}
				}
			}
		}
	}

	public void enableCurrentMainTouchPanelScheme ()
	{
		for (int i = 0; i < touchPanelsSchemesInfoList.Count; i++) {
			if (touchPanelsSchemesInfoList [i].isMainGameTouchPanel && touchPanelsSchemesInfoList [i].mainGameTouchPanelActive) {
				enableTouchPanelSchemeByName (touchPanelsSchemesInfoList [i].Name);

				return;
			}
		}
	}

	public void enablePreviousTouchPanelScheme ()
	{
		if (previousTouchPanelsSchemesInfo != null) {
			enableTouchPanelSchemeByName (previousTouchPanelsSchemesInfo.Name);
		}
	}


	//Action Screen Panels Functions
	public void enableOrDisableActionScreen (string actionScreenName)
	{
		if (canUseActionPanel ()) {
			
			checkInputPanelUISystem ();

			int screenActionInfoListCount = screenActionInfoList.Count;

			if (screenActionInfoListCount == 0) {
				return;
			}

			for (int i = 0; i < screenActionInfoListCount; i++) {
				if (screenActionInfoList [i].screenActionName.Equals (actionScreenName)) {

					if (previousScreenActionInfo != currentScreenActionInfo) {
						previousScreenActionInfo = currentScreenActionInfo;
					}

					currentScreenActionInfo = screenActionInfoList [i];

					if (currentScreenActionInfo.screenActionsGameObject != null) {
						if (currentScreenActionInfo.screenActionsGameObject.activeSelf != (!currentScreenActionInfo.screenActionsGameObject.activeSelf)) {
							currentScreenActionInfo.screenActionsGameObject.SetActive (!currentScreenActionInfo.screenActionsGameObject.activeSelf);
						}
					}

					if (playerInput.isUsingGamepad ()) {
						if (currentScreenActionInfo.screenActionsGamepadGameObject != null) {
							if (currentScreenActionInfo.screenActionsGamepadGameObject.activeSelf != (!currentScreenActionInfo.screenActionsGamepadGameObject.activeSelf)) {
								currentScreenActionInfo.screenActionsGamepadGameObject.SetActive (!currentScreenActionInfo.screenActionsGamepadGameObject.activeSelf);
							}
						}

						if (currentScreenActionInfo.mainScreenActionGameObject != null && currentScreenActionInfo.mainScreenActionGameObject.activeSelf) {
							currentScreenActionInfo.mainScreenActionGameObject.SetActive (false);
						}
					} else {
						if (currentScreenActionInfo.mainScreenActionGameObject != null) {
							if (currentScreenActionInfo.mainScreenActionGameObject.activeSelf != (!currentScreenActionInfo.mainScreenActionGameObject.activeSelf)) {
								currentScreenActionInfo.mainScreenActionGameObject.SetActive (!currentScreenActionInfo.mainScreenActionGameObject.activeSelf);
							}
						}

						if (currentScreenActionInfo.screenActionsGamepadGameObject != null && currentScreenActionInfo.screenActionsGamepadGameObject.activeSelf) {
							currentScreenActionInfo.screenActionsGamepadGameObject.SetActive (false);
						}
					}

					return;
				}
			}
		}
	}

	public void enableOrDisableActionScreenTemporally (string actionScreenName)
	{
		if (canUseActionPanel ()) {

			checkInputPanelUISystem ();

			int screenActionInfoListCount = screenActionInfoList.Count;

			if (screenActionInfoListCount == 0) {
				return;
			}

			for (int i = 0; i < screenActionInfoListCount; i++) {
				if (screenActionInfoList [i].screenActionName.Equals (actionScreenName)) {

					if (screenActionInfoList [i].screenActionsGameObject != null) {
						if (screenActionInfoList [i].screenActionsGameObject.activeSelf != (!screenActionInfoList [i].screenActionsGameObject.activeSelf)) {
							screenActionInfoList [i].screenActionsGameObject.SetActive (!screenActionInfoList [i].screenActionsGameObject.activeSelf);
						}
					}
						
					if (playerInput.isUsingGamepad ()) {
						if (screenActionInfoList [i].screenActionsGamepadGameObject != null) {
							if (screenActionInfoList [i].screenActionsGamepadGameObject.activeSelf != (!screenActionInfoList [i].screenActionsGamepadGameObject.activeSelf)) {
								screenActionInfoList [i].screenActionsGamepadGameObject.SetActive (!screenActionInfoList [i].screenActionsGamepadGameObject.activeSelf);
							}
						}

						if (screenActionInfoList [i].mainScreenActionGameObject != null && screenActionInfoList [i].mainScreenActionGameObject.activeSelf) {
							screenActionInfoList [i].mainScreenActionGameObject.SetActive (false);
						}
					} else {
						if (screenActionInfoList [i].mainScreenActionGameObject != null) {
							if (screenActionInfoList [i].mainScreenActionGameObject.activeSelf != (!screenActionInfoList [i].mainScreenActionGameObject.activeSelf)) {
								screenActionInfoList [i].mainScreenActionGameObject.SetActive (!screenActionInfoList [i].mainScreenActionGameObject.activeSelf);
							}
						}

						if (screenActionInfoList [i].screenActionsGamepadGameObject != null && screenActionInfoList [i].screenActionsGamepadGameObject.activeSelf) {
							screenActionInfoList [i].screenActionsGamepadGameObject.SetActive (false);
						}
					}

					return;
				}
			}
		}
	}

	public void enableOrDisableActionScreen (string actionScreenName, bool state)
	{
		if (canUseActionPanel ()) {

			checkInputPanelUISystem ();

			int screenActionInfoListCount = screenActionInfoList.Count;

			if (screenActionInfoListCount == 0) {
				return;
			}

			for (int i = 0; i < screenActionInfoListCount; i++) {
				if (screenActionInfoList [i].screenActionName.Equals (actionScreenName)) {

					if (previousScreenActionInfo != currentScreenActionInfo) {
						previousScreenActionInfo = currentScreenActionInfo;
					}

					currentScreenActionInfo = screenActionInfoList [i];

					if (currentScreenActionInfo.screenActionsGameObject != null && currentScreenActionInfo.screenActionsGameObject.activeSelf != state) {
						currentScreenActionInfo.screenActionsGameObject.SetActive (state);
					}
						
					if (playerInput.isUsingGamepad ()) {
						if (currentScreenActionInfo.screenActionsGamepadGameObject != null && currentScreenActionInfo.screenActionsGamepadGameObject != state) {
							currentScreenActionInfo.screenActionsGamepadGameObject.SetActive (state);
						}

						if (currentScreenActionInfo.mainScreenActionGameObject != null && currentScreenActionInfo.mainScreenActionGameObject.activeSelf) {
							currentScreenActionInfo.mainScreenActionGameObject.SetActive (false);
						}
					} else {
						if (currentScreenActionInfo.mainScreenActionGameObject != null && currentScreenActionInfo.mainScreenActionGameObject.activeSelf != state) {
							currentScreenActionInfo.mainScreenActionGameObject.SetActive (state);
						}

						if (currentScreenActionInfo.screenActionsGamepadGameObject != null && currentScreenActionInfo.screenActionsGamepadGameObject.activeSelf) {
							currentScreenActionInfo.screenActionsGamepadGameObject.SetActive (false);
						}
					}

					if (state) {
						if (!screenActionParent.activeSelf) {
							if (screenActionPanelsActive) {
								screenActionParent.SetActive (true);
							}
						}
					}

					if (state) {
						checkDisableMainGameScreenActionPanel ();
					} else {
						checkEnableMainGameScreenActionPanel ();
					}

					return;
				}
			}
		}
	}

	public void enableActionScreen (string actionScreenName)
	{
		if (canUseActionPanel ()) {

			checkInputPanelUISystem ();

			int screenActionInfoListCount = screenActionInfoList.Count;

			if (screenActionInfoListCount == 0) {
				return;
			}

			for (int i = 0; i < screenActionInfoListCount; i++) {
				if (screenActionInfoList [i].screenActionName.Equals (actionScreenName)) {

					if (previousScreenActionInfo != currentScreenActionInfo) {
						previousScreenActionInfo = currentScreenActionInfo;
					}

					currentScreenActionInfo = screenActionInfoList [i];

					if (currentScreenActionInfo.screenActionsGameObject != null && !currentScreenActionInfo.screenActionsGameObject.activeSelf) {
						currentScreenActionInfo.screenActionsGameObject.SetActive (true);
					}

					if (playerInput.isUsingGamepad ()) {
						if (currentScreenActionInfo.screenActionsGamepadGameObject != null && !currentScreenActionInfo.screenActionsGamepadGameObject.activeSelf) {
							currentScreenActionInfo.screenActionsGamepadGameObject.SetActive (true);
						}

						if (currentScreenActionInfo.mainScreenActionGameObject != null && currentScreenActionInfo.mainScreenActionGameObject.activeSelf) {
							currentScreenActionInfo.mainScreenActionGameObject.SetActive (false);
						}
					} else {
						if (currentScreenActionInfo.mainScreenActionGameObject != null && !currentScreenActionInfo.mainScreenActionGameObject.activeSelf) {
							currentScreenActionInfo.mainScreenActionGameObject.SetActive (true);
						}

						if (currentScreenActionInfo.screenActionsGamepadGameObject != null && currentScreenActionInfo.screenActionsGamepadGameObject.activeSelf) {
							currentScreenActionInfo.screenActionsGamepadGameObject.SetActive (false);
						}
					}

					return;
				}
			}
		}
	}

	public void disableActionScreen (string actionScreenName)
	{
		if (canUseActionPanel ()) {

			checkInputPanelUISystem ();

			int screenActionInfoListCount = screenActionInfoList.Count;

			if (screenActionInfoListCount == 0) {
				return;
			}

			for (int i = 0; i < screenActionInfoListCount; i++) {
				if (screenActionInfoList [i].screenActionName.Equals (actionScreenName)) {

					if (previousScreenActionInfo != currentScreenActionInfo) {
						previousScreenActionInfo = currentScreenActionInfo;
					}

					currentScreenActionInfo = screenActionInfoList [i];
			
					if (currentScreenActionInfo.screenActionsGameObject != null && currentScreenActionInfo.screenActionsGameObject.activeSelf) {
						currentScreenActionInfo.screenActionsGameObject.SetActive (false);
					}

					if (currentScreenActionInfo.mainScreenActionGameObject != null && currentScreenActionInfo.mainScreenActionGameObject.activeSelf) {
						currentScreenActionInfo.mainScreenActionGameObject.SetActive (false);
					}

					if (currentScreenActionInfo.screenActionsGamepadGameObject != null && currentScreenActionInfo.screenActionsGamepadGameObject.activeSelf) {
						currentScreenActionInfo.screenActionsGamepadGameObject.SetActive (false);
					}

					if (currentScreenActionInfo.isMainGameScreenActionPanel) {
						currentScreenActionInfo.mainGameScreenActionPanelActive = false;
					}

					return;
				}
			}
		}
	}

	public void enablePreviousActionScreen ()
	{
		if (canUseActionPanel ()) {
			if (previousScreenActionInfo != null && currentScreenActionInfo != null) {
				if (currentScreenActionInfo != previousScreenActionInfo) {
					enableSingleActionScreen (previousScreenActionInfo.screenActionName);
				}

			} else {
				if (currentScreenActionInfo != null) {
					disableActionScreen (currentScreenActionInfo.screenActionName);
				}

				currentScreenActionInfo = null;
				previousScreenActionInfo = null;
			}
		}
	}

	public void enableSingleActionScreen (string actionScreenName)
	{
		if (canUseActionPanel ()) {

			checkInputPanelUISystem ();

			int screenActionInfoListCount = screenActionInfoList.Count;

			if (screenActionInfoListCount == 0) {
				return;
			}

			for (int i = 0; i < screenActionInfoListCount; i++) {
				if (screenActionInfoList [i].screenActionName.Equals (actionScreenName)) {

					if (previousScreenActionInfo != currentScreenActionInfo) {
						previousScreenActionInfo = currentScreenActionInfo;
					}

					currentScreenActionInfo = screenActionInfoList [i];

					if (currentScreenActionInfo.screenActionsGameObject != null && !currentScreenActionInfo.screenActionsGameObject.activeSelf) {
						currentScreenActionInfo.screenActionsGameObject.SetActive (true);
					}
						
					if (playerInput.isUsingGamepad ()) {
						if (currentScreenActionInfo.screenActionsGamepadGameObject != null && !currentScreenActionInfo.screenActionsGamepadGameObject.activeSelf) {
							currentScreenActionInfo.screenActionsGamepadGameObject.SetActive (true);
						}

						if (currentScreenActionInfo.mainScreenActionGameObject != null && currentScreenActionInfo.mainScreenActionGameObject.activeSelf) {
							currentScreenActionInfo.mainScreenActionGameObject.SetActive (false);
						}
					} else {
						if (currentScreenActionInfo.mainScreenActionGameObject != null && !currentScreenActionInfo.mainScreenActionGameObject.activeSelf) {
							currentScreenActionInfo.mainScreenActionGameObject.SetActive (true);
						}

						if (currentScreenActionInfo.screenActionsGamepadGameObject != null && currentScreenActionInfo.screenActionsGamepadGameObject.activeSelf) {
							currentScreenActionInfo.screenActionsGamepadGameObject.SetActive (false);
						}
					}

					if (currentScreenActionInfo.isMainGameScreenActionPanel) {
						currentScreenActionInfo.mainGameScreenActionPanelActive = true;
					}
				} else {
					if (screenActionInfoList [i].screenActionsGameObject != null && screenActionInfoList [i].screenActionsGameObject.activeSelf) {
						screenActionInfoList [i].screenActionsGameObject.SetActive (false);
					}

					if (screenActionInfoList [i].mainScreenActionGameObject != null && screenActionInfoList [i].mainScreenActionGameObject.activeSelf) {
						screenActionInfoList [i].mainScreenActionGameObject.SetActive (false);
					}
						
					if (screenActionInfoList [i].screenActionsGamepadGameObject != null && screenActionInfoList [i].screenActionsGamepadGameObject.activeSelf) {
						screenActionInfoList [i].screenActionsGamepadGameObject.SetActive (false);
					}
				}
			}
		}
	}

	public void checkEnableMainGameScreenActionPanel ()
	{
		if (!screenActionPanelsEnabled) {
			return;
		}

		checkInputPanelUISystem ();

		int screenActionInfoListCount = screenActionInfoList.Count;

		if (screenActionInfoListCount == 0) {
			return;
		}

		for (int i = 0; i < screenActionInfoListCount; i++) {
			if (screenActionInfoList [i].isMainGameScreenActionPanel && screenActionInfoList [i].mainGameScreenActionPanelActive) {
				
				enableSingleActionScreen (screenActionInfoList [i].screenActionName);

				return;
			}
		}
	}

	public void enableSecondaryActionPanel (string actionScreenName)
	{
		enableOrDisableSecondaryActionPanel (actionScreenName, true);
	}

	public void disableSecondaryActionPanel (string actionScreenName)
	{
		enableOrDisableSecondaryActionPanel (actionScreenName, false);
	}

	public void enableOrDisableSecondaryActionPanel (string actionScreenName, bool state)
	{
		checkInputPanelUISystem ();

		int screenActionInfoListCount = screenActionInfoList.Count;

		if (screenActionInfoListCount == 0) {
			return;
		}

		for (int i = 0; i < screenActionInfoListCount; i++) {
			if (screenActionInfoList [i].screenActionName.Equals (actionScreenName)) {
				if (playerInput.isUsingGamepad ()) {
					if (screenActionInfoList [i].secondaryActionPanelGamepadGameObject != null && screenActionInfoList [i].secondaryActionPanelGamepadGameObject.activeSelf != state) {
						screenActionInfoList [i].secondaryActionPanelGamepadGameObject.SetActive (state);
					}

					if (screenActionInfoList [i].secondaryActionPanelGameObject != null && screenActionInfoList [i].secondaryActionPanelGameObject.activeSelf) { 
						screenActionInfoList [i].secondaryActionPanelGameObject.SetActive (false);
					}
				} else {
					if (screenActionInfoList [i].secondaryActionPanelGameObject != null && screenActionInfoList [i].secondaryActionPanelGameObject.activeSelf != state) { 
						screenActionInfoList [i].secondaryActionPanelGameObject.SetActive (state);
					}

					if (screenActionInfoList [i].secondaryActionPanelGamepadGameObject != null && screenActionInfoList [i].secondaryActionPanelGamepadGameObject.activeSelf) {
						screenActionInfoList [i].secondaryActionPanelGamepadGameObject.SetActive (false);
					}
				}

				return;
			}
		}
	}

	public void checkDisableMainGameScreenActionPanel ()
	{
		if (!screenActionPanelsEnabled) {
			return;
		}

		checkInputPanelUISystem ();

		int screenActionInfoListCount = screenActionInfoList.Count;

		if (screenActionInfoListCount == 0) {
			return;
		}

		for (int i = 0; i < screenActionInfoListCount; i++) {
			if (screenActionInfoList [i].isMainGameScreenActionPanel && screenActionInfoList [i].mainGameScreenActionPanelActive) {

				if (screenActionInfoList [i].screenActionsGameObject != null && screenActionInfoList [i].screenActionsGameObject.activeSelf) {
					screenActionInfoList [i].screenActionsGameObject.SetActive (false);
				}

				if (screenActionInfoList [i].mainScreenActionGameObject != null && screenActionInfoList [i].mainScreenActionGameObject.activeSelf) {
					screenActionInfoList [i].mainScreenActionGameObject.SetActive (false);
				}
					
				if (screenActionInfoList [i].screenActionsGamepadGameObject != null && screenActionInfoList [i].screenActionsGamepadGameObject.activeSelf) {
					screenActionInfoList [i].screenActionsGamepadGameObject.SetActive (false);
				}

				return;
			}
		}
	}

	public void checkPanelsActiveOnGamepadOrKeyboard (bool checkKeyboard)
	{
		if (!screenActionPanelsEnabled) {
			return;
		}

		checkInputPanelUISystem ();

		int screenActionInfoListCount = screenActionInfoList.Count;

		if (screenActionInfoListCount == 0) {
			return;
		}

		for (int i = 0; i < screenActionInfoListCount; i++) {
			if (checkKeyboard) {
				if (screenActionInfoList [i].screenActionsGameObject != null) {
					if (screenActionInfoList [i].screenActionsGameObject.activeSelf) {
						
						if (screenActionInfoList [i].mainScreenActionGameObject != null &&
						    !screenActionInfoList [i].mainScreenActionGameObject.activeSelf) {
							screenActionInfoList [i].mainScreenActionGameObject.SetActive (true);
						}

						if (screenActionInfoList [i].hasSecondaryActionPanel &&
						    screenActionInfoList [i].secondaryActionPanelGameObject != null &&
						    !screenActionInfoList [i].secondaryActionPanelGameObject.activeSelf) {

							screenActionInfoList [i].secondaryActionPanelGameObject.SetActive (true);
						}

						if (screenActionInfoList [i].screenActionsGamepadGameObject != null &&
						    screenActionInfoList [i].screenActionsGamepadGameObject.activeSelf) {
							screenActionInfoList [i].screenActionsGamepadGameObject.SetActive (false);
						}

						if (screenActionInfoList [i].secondaryActionPanelGamepadGameObject != null &&
						    screenActionInfoList [i].secondaryActionPanelGamepadGameObject.activeSelf) {
							screenActionInfoList [i].secondaryActionPanelGamepadGameObject.SetActive (false);
						}
					}
				}
			} else {
				if (screenActionInfoList [i].screenActionsGameObject != null) {
					if (screenActionInfoList [i].screenActionsGameObject.activeSelf) {

						if (screenActionInfoList [i].secondaryActionPanelGameObject != null &&
						    screenActionInfoList [i].secondaryActionPanelGameObject.activeSelf) {
							screenActionInfoList [i].secondaryActionPanelGameObject.SetActive (false);
						}

						if (screenActionInfoList [i].mainScreenActionGameObject != null &&
						    screenActionInfoList [i].mainScreenActionGameObject.activeSelf) {
							screenActionInfoList [i].mainScreenActionGameObject.SetActive (false);
						}
							
						if (screenActionInfoList [i].screenActionsGamepadGameObject != null &&
						    !screenActionInfoList [i].screenActionsGamepadGameObject.activeSelf) {
							screenActionInfoList [i].screenActionsGamepadGameObject.SetActive (true);
						}

						if (screenActionInfoList [i].hasSecondaryActionPanel &&
						    screenActionInfoList [i].secondaryActionPanelGamepadGameObject != null &&
						    !screenActionInfoList [i].secondaryActionPanelGamepadGameObject.activeSelf) {

							screenActionInfoList [i].secondaryActionPanelGamepadGameObject.SetActive (true);
						}
					}
				}
			}
		}
	}

	public void enableOrDisableScreenActionParent (bool state)
	{
		if (!screenActionPanelsActive) {
			return;
		}

		if (state && playerInput.isUsingTouchControlsOnGameManager ()) {
			return;
		}

		if (screenActionParent != null && screenActionParent.activeSelf != state) {
			screenActionParent.SetActive (state);
		}
	}

	public bool canUseActionPanel ()
	{
		if (!screenActionPanelsEnabled) {
			return false;
		}

		if (!screenActionPanelsActive) {
			return false;
		}

		if (playerInput.isUsingTouchControls ()) {
			return false;
		}

		return true;
	}

	public void setScreenActionPanelsActiveState (bool state)
	{
		if (screenActionParent == null) {
			return;
		}

		if (screenActionPanelsActive) {
			panelParentActivePreviously = screenActionParent.activeSelf;
		}

		screenActionPanelsActive = state;

		if (screenActionPanelsActive) {
			if (panelParentActivePreviously) {
				if (playerInput.isUsingTouchControlsOnGameManager ()) {
					return;
				}

				if (!screenActionParent.activeSelf) {
					screenActionParent.SetActive (true);
				}
			}
		} else {
			if (screenActionParent.activeSelf) {
				screenActionParent.SetActive (false);
			}
		}
	}

	public void setScreenActionPanelsEnabledState (bool state)
	{
		screenActionPanelsEnabled = state;

		updateComponent ();
	}

	public void instantiateInputPanelPrefab ()
	{
		if (mainInputPanelUISystem == null && inputPanelPrefab != null) {
			GameObject newInputPanel = (GameObject)Instantiate (inputPanelPrefab, Vector3.zero, Quaternion.identity);
			newInputPanel.name = inputPanelPrefab.name;

			inputPanelUISystem currentInputPanelUISystem = newInputPanel.GetComponentInChildren<inputPanelUISystem> ();
		
			if (currentInputPanelUISystem != null) {
				currentInputPanelUISystem.searchPlayerInputPanelSystem (this);

				pauseManager.addNewCanvasToList (newInputPanel.GetComponent<Canvas> ());
			}
		} else {
			print ("Player Input Panel is already on the scene");
		}
	}

	public void setInputPanelUISystem (inputPanelUISystem newInputPanelUISystem)
	{
		mainInputPanelUISystem = newInputPanelUISystem;

		screenActionParent = mainInputPanelUISystem.screenActionParent;

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}
