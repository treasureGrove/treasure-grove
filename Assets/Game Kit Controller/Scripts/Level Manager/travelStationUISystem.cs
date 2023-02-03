using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class travelStationUISystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool travelStationUIActive = true;
	public bool menuOpened;
	public bool useBlurUIPanel;

	public bool saveCurrentTravelStationToSaveFile = true;

	public bool allStationsUnlocked;

	public bool showTravelStationFoundPanel;
	public float showTraveStationPanelDuration = 1;
	public string travelStationFoundExtraMessage = "Travel Station Found";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public List<foundTravelStationInfo> foundTravelStationInfoList = new List<foundTravelStationInfo> ();

	public travelStationSystem currentTravelStationSystem;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventIfSystemDisabled;
	public UnityEvent eventIfSystemDisabled;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject travelStationMenu;

	public GameObject travelStationPanelPrefab;

	public Transform travelStationPanelParent;

	public GameObject travelStationFoundPanel;
	public Text travelStationFoundPanelText;

	public menuPause pauseManager;
	public playerController playerControllerManager;
	public saveGameSystem mainSaveGameSystem;
	public usingDevicesSystem usingDevicesManager;

	List<travelStationSystem.travelStationInfo> currentTravelStationInfoList = new List<travelStationSystem.travelStationInfo> ();

	List<travelStationPanelInfo> travelStationPanelInfoList = new List<travelStationPanelInfo> ();

	travelStationPanelInfo currentTravelStationPanelInfo;

	public void initializeTravelStationValues ()
	{
		if (!travelStationUIActive) {
			checkEventOnSystemDisabled ();

			return;
		}
	}

	public void checkEventOnSystemDisabled ()
	{
		if (useEventIfSystemDisabled) {
			eventIfSystemDisabled.Invoke ();
		}
	}

	public void checkPressedButton (GameObject buttonToCheck)
	{
		if (currentTravelStationPanelInfo != null) {
			currentTravelStationPanelInfo.pressedPanelIcon.SetActive (false);
		}

		for (int i = 0; i < travelStationPanelInfoList.Count; i++) {

			if (travelStationPanelInfoList [i].panelGameObject == buttonToCheck) {

				travelStationPanelInfoList [i].pressedPanelIcon.SetActive (true);

				currentTravelStationPanelInfo = travelStationPanelInfoList [i];

				return;
			}
		}
	}

	public void confirmTravelToStationSelected ()
	{
		if (currentTravelStationInfoList.Count > 0 && currentTravelStationPanelInfo != null && currentTravelStationPanelInfo.travelStationInfoIndex < currentTravelStationInfoList.Count) {
			travelStationSystem.travelStationInfo currentTravelStationInfo = currentTravelStationInfoList [currentTravelStationPanelInfo.travelStationInfoIndex];

			if (currentTravelStationInfo.sceneNumberToLoad != currentTravelStationSystem.sceneNumberToLoad ||
			    currentTravelStationInfo.levelManagerIDToLoad != currentTravelStationSystem.levelManagerIDToLoad) {
				mainSaveGameSystem.saveGameCheckpoint (null, currentTravelStationInfo.levelManagerIDToLoad, 
					currentTravelStationInfo.sceneNumberToLoad, true, true, true);

				mainSaveGameSystem.activateAutoLoad ();
			} else {
				if (showDebugPrint) {
					print ("trying to travel to this station system");
				}
			}
		}
	}

	public void setCurrentTravelStationSystem (travelStationSystem newTravelStationSystem, bool settingPhysicalStation)
	{
		currentTravelStationSystem = newTravelStationSystem;

		currentTravelStationInfoList = currentTravelStationSystem.getTravelStationInfoList ();

		int sceneNumberToLoad = currentTravelStationSystem.sceneNumberToLoad;

		int levelManagerIDToLoad = currentTravelStationSystem.levelManagerIDToLoad;

		bool travelStationFound = false;

		if (settingPhysicalStation) {
			for (int i = 0; i < foundTravelStationInfoList.Count; i++) {
				if (foundTravelStationInfoList [i].sceneNumberToLoad == sceneNumberToLoad && foundTravelStationInfoList [i].levelManagerIDToLoad == levelManagerIDToLoad) {
					travelStationFound = true;
				}
			}

			if (!travelStationFound) {
				if (showDebugPrint) {
					print ("new station");
				}

				foundTravelStationInfo newFoundTravelStationInfo = new foundTravelStationInfo ();

				newFoundTravelStationInfo.sceneNumberToLoad = sceneNumberToLoad;
				newFoundTravelStationInfo.levelManagerIDToLoad = levelManagerIDToLoad;

				foundTravelStationInfoList.Add (newFoundTravelStationInfo);
			}
		}

		if (currentTravelStationSystem.allStationsUnlocked || allStationsUnlocked) {
			for (int i = 0; i < currentTravelStationInfoList.Count; i++) {
				currentTravelStationInfoList [i].zoneFound = true;
			}
		} else {
			for (int i = 0; i < currentTravelStationInfoList.Count; i++) {
				for (int j = 0; j < foundTravelStationInfoList.Count; j++) {
					if (currentTravelStationInfoList [i].sceneNumberToLoad == foundTravelStationInfoList [j].sceneNumberToLoad &&
					    currentTravelStationInfoList [i].levelManagerIDToLoad == foundTravelStationInfoList [j].levelManagerIDToLoad) {
						currentTravelStationInfoList [i].zoneFound = true;
					}
				}

				if (currentTravelStationInfoList [i].sceneNumberToLoad == sceneNumberToLoad) {
					if (!travelStationFound && settingPhysicalStation) {
						showTravelStationFound (currentTravelStationInfoList [i].Name);
					}
				}
			}
		}
	}

	public void openOrCloseTravelStationMenu (bool state)
	{
		menuOpened = state;

		pauseManager.openOrClosePlayerMenu (menuOpened, travelStationMenu.transform, useBlurUIPanel);

		travelStationMenu.SetActive (menuOpened);

		pauseManager.setIngameMenuOpenedState ("Travel Station System", menuOpened, false);

		pauseManager.enableOrDisablePlayerMenu (menuOpened, true, false);
	
		if (menuOpened) {

			setTravelStationInfoList ();

			if (travelStationPanelInfoList.Count > 0) {

				GameObject panelButtonToCheck = travelStationPanelInfoList [0].panelGameObject;

				if (currentTravelStationSystem) {
					for (int i = 0; i < currentTravelStationInfoList.Count; i++) {
						if (currentTravelStationInfoList [i].sceneNumberToLoad == currentTravelStationSystem.sceneNumberToLoad) {
							if (i < travelStationPanelInfoList.Count) {
								panelButtonToCheck = travelStationPanelInfoList [i].panelGameObject;
							}
						}
					}
				}

				checkPressedButton (panelButtonToCheck);
			}
		}
	}

	public void setTravelStationInfoList ()
	{
		for (int i = 0; i < travelStationPanelInfoList.Count; i++) {
			Destroy (travelStationPanelInfoList [i].panelGameObject);
		}

		travelStationPanelInfoList.Clear ();

		if (currentTravelStationSystem == null || currentTravelStationInfoList.Count == 0) {
			currentTravelStationSystem = FindObjectOfType<travelStationSystem> ();

			if (currentTravelStationSystem != null) {
				setCurrentTravelStationSystem (currentTravelStationSystem, false);
			}
		}

		for (int i = 0; i < currentTravelStationInfoList.Count; i++) {
			if (currentTravelStationInfoList [i].zoneFound) {
				GameObject newtravelStationPanelPrefab = (GameObject)Instantiate (travelStationPanelPrefab, travelStationPanelPrefab.transform.position, Quaternion.identity, travelStationPanelParent);

				if (!newtravelStationPanelPrefab.activeSelf) {
					newtravelStationPanelPrefab.SetActive (true);
				}

				newtravelStationPanelPrefab.transform.localScale = Vector3.one;
				newtravelStationPanelPrefab.transform.localPosition = Vector3.zero;

				travelStationPanel currentTravelStationPanel = newtravelStationPanelPrefab.GetComponent<travelStationPanel> ();

				travelStationPanelInfo currentTravelStationPanelInfo = currentTravelStationPanel.mainTravelStationPanelInfo;

				currentTravelStationPanelInfo.panelText.text = currentTravelStationInfoList [i].Name;
				currentTravelStationPanelInfo.travelStationInfoIndex = i;

				travelStationPanelInfoList.Add (currentTravelStationPanelInfo);
			}
		}
	}

	public void openOrCloseTravelStationMenuFromTouch ()
	{
		openOrCloseTravelStationMenu (!menuOpened);

		pauseManager.setIngameMenuOpenedState ("Travel Station System", menuOpened, true);
	}

	public void checkOpenOrCloseTravelStationMenuByButton ()
	{
		if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
			return;
		}

		oopenOrCloseTravelStationMenuByButton ();
	}

	public void oopenOrCloseTravelStationMenuByButton ()
	{
		if (playerControllerManager.isUsingDevice ()) {
			usingDevicesManager.useDevice ();

			if (currentTravelStationSystem) {
				currentTravelStationSystem.setUsingTravelStationSystemState (menuOpened);
			}
		} else {
			openOrCloseTravelStationMenuFromTouch ();
		}
	}

	Coroutine showTravelStationCoroutine;

	public void showTravelStationFound (string travelStationName)
	{
		if (!showTravelStationFoundPanel) {
			return;
		}

		if (showTravelStationCoroutine != null) {
			StopCoroutine (showTravelStationCoroutine);
		}

		showTravelStationCoroutine = StartCoroutine (showTravelStationFoundCoroutine (travelStationName));
	}

	IEnumerator showTravelStationFoundCoroutine (string travelStationName)
	{
		travelStationFoundPanel.SetActive (true);

		travelStationFoundPanelText.text = travelStationName + " " + travelStationFoundExtraMessage;

		yield return new WaitForSeconds (showTraveStationPanelDuration);

		travelStationFoundPanel.SetActive (false);
	}

	[System.Serializable]
	public class foundTravelStationInfo
	{
		public int sceneNumberToLoad;

		public int levelManagerIDToLoad;
	}
}
