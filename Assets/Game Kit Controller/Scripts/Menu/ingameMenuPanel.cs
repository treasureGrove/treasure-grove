using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ingameMenuPanel : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string menuPanelName;

	public bool useBlurUIPanel;

	[Space]
	[Header ("Move Panels")]
	[Space]

	public bool movePanelsEnabled;

	public RectTransform mainPanelToMoveTransform;

	[Space]
	[Space]

	public float screenLimitOffset = 20;

	public bool useScreenLimitOffsets;
	public float leftScreenLimitOffset;
	public float rightScreenLimitOffset;
	public float upScreenLimitOffset;
	public float downScreenLimitOffset;

	[Space]
	[Header ("Enable/disable Panel Settings")]
	[Space]

	public bool enableDisablePanelsEnabled;

	public List<menuPanelInfo> menuPanelInfoList = new List<menuPanelInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool menuPanelOpened;

	public bool movingPanelActive;

	public Vector2 newPosition;

	public Vector2 screenLimit;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject menuPanelObject;

	public menuPause pauseManager;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnOpenClostMenu;
	public UnityEvent eventOnOpenMenu;
	public UnityEvent eventOnCloseMenu;


	bool pauseManagerLocated;

	Coroutine updateCoroutine;

	bool touchPlatform;

	Touch currentTouch;

	Transform previousParent;

	RectTransform currentPanelToMove;

	Vector2 currentScreenLimitOffset;

	public virtual void closeMenuPanelIfOpened ()
	{
		if (menuPanelOpened) {
			openOrCloseMenuPanel (false);
		}
	}

	public virtual void openOrCloseMenuPanel (bool state)
	{
		menuPanelOpened = state;

		pauseManagerLocated = pauseManager != null;

		menuPanelObject.SetActive (menuPanelOpened);

		if (pauseManagerLocated) {
			pauseManager.openOrClosePlayerMenu (menuPanelOpened, menuPanelObject.transform, useBlurUIPanel);

			pauseManager.setIngameMenuOpenedState (menuPanelName, menuPanelOpened, false);

			pauseManager.enableOrDisablePlayerMenu (menuPanelOpened, true, false);

			pauseManager.checkUpdateReticleActiveState (state);
		}

		if (menuPanelOpened) {
			openMenuPanelState ();
		} else {
			closeMenuPanelState ();
		}

		checkEventsOnStateChange (menuPanelOpened);

		if (!menuPanelOpened) {
			if (movingPanelActive) {
				disableMovePanel ();
			}
		}
	}

	public void openOrCloseMenuFromTouch ()
	{
		openOrCloseMenuPanel (!menuPanelOpened);

		pauseManagerLocated = pauseManager != null;

		if (pauseManagerLocated) {
			pauseManager.setIngameMenuOpenedState (menuPanelName, menuPanelOpened, true);
		}
	}

	public virtual void openMenuPanelState ()
	{

	}

	public virtual void closeMenuPanelState ()
	{

	}

	public void setPauseManager (menuPause currentMenuPause)
	{
		pauseManager = currentMenuPause;

		initializeMenuPanel ();
	}

	public virtual void initializeMenuPanel ()
	{

	}

	public void openNextPlayerOpenMenu ()
	{
		pauseManagerLocated = pauseManager != null;

		if (pauseManagerLocated) {
			pauseManager.openNextPlayerOpenMenu ();
		}
	}

	public void openPreviousPlayerOpenMenu ()
	{
		pauseManagerLocated = pauseManager != null;

		if (pauseManagerLocated) {
			pauseManager.openPreviousPlayerOpenMenu ();
		}
	}

	public void closePlayerMenuByName (string menuName)
	{
		pauseManagerLocated = pauseManager != null;

		if (pauseManagerLocated) {
			pauseManager.closePlayerMenuByName (menuName);
		}
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnOpenClostMenu) {
			if (state) {
				eventOnOpenMenu.Invoke ();
			} else {
				eventOnCloseMenu.Invoke ();
			}
		}
	}

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
		updatePanelPosition ();
	}

	void updatePanelPosition ()
	{
		int touchCount = Input.touchCount;
		if (!touchPlatform) {
			touchCount++;
		}

		for (int i = 0; i < touchCount; i++) {
			if (!touchPlatform) {
				currentTouch = touchJoystick.convertMouseIntoFinger ();
			} else {
				currentTouch = Input.GetTouch (i);
			}

			newPosition = new Vector2 (currentTouch.position.x, currentTouch.position.y);

			if (useScreenLimitOffsets) {
				newPosition.x = Mathf.Clamp (newPosition.x, (leftScreenLimitOffset + screenLimitOffset),
					screenLimit.x - (rightScreenLimitOffset + screenLimitOffset));
				
				newPosition.y = Mathf.Clamp (newPosition.y, (downScreenLimitOffset + screenLimitOffset), 
					screenLimit.y - (upScreenLimitOffset + screenLimitOffset));
			} else {
				currentScreenLimitOffset = new Vector2 (screenLimitOffset, screenLimitOffset);

				newPosition.x = Mathf.Clamp (newPosition.x, currentScreenLimitOffset.x, screenLimit.x - currentScreenLimitOffset.x);
				newPosition.y = Mathf.Clamp (newPosition.y, currentScreenLimitOffset.y, screenLimit.y - currentScreenLimitOffset.y);
			}	

			mainPanelToMoveTransform.position = newPosition;
		}
	}

	public void enableMovePanel (RectTransform panelToMove)
	{
		if (movePanelsEnabled) {
			if (!movingPanelActive) {
				screenLimit = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

				touchPlatform = touchJoystick.checkTouchPlatform ();

				currentPanelToMove = panelToMove;

				updatePanelPosition ();

				previousParent = panelToMove.parent;

				panelToMove.SetParent (mainPanelToMoveTransform);

				stopUpdateCoroutine ();

				updateCoroutine = StartCoroutine (updateSystemCoroutine ());

				movingPanelActive = true;
			}
		}
	}

	public void disableMovePanel ()
	{
		if (movePanelsEnabled) {
			if (movingPanelActive) {
				stopUpdateCoroutine ();

				if (currentPanelToMove != null) {
					currentPanelToMove.SetParent (previousParent);

					currentPanelToMove = null;
				}

				movingPanelActive = false;
			}
		}
	}

	public void toggleMenuPanelActiveState (GameObject buttonObject)
	{
		if (enableDisablePanelsEnabled) {
			if (buttonObject != null) {
				int currentIndex = menuPanelInfoList.FindIndex (s => s.mainPanelButton.Equals (buttonObject));

				if (currentIndex > -1) {
					menuPanelInfo currentMenuPanelInfo = menuPanelInfoList [currentIndex];

					bool panelState = !currentMenuPanelInfo.mainPanel.activeSelf;

					currentMenuPanelInfo.mainPanel.SetActive (panelState);

					if (currentMenuPanelInfo.disabledPanel != null) {
						currentMenuPanelInfo.disabledPanel.SetActive (!panelState);
					}
				}
			}
		}
	}

	public void openOrCloseAllMenuPanels (bool state)
	{
		if (enableDisablePanelsEnabled) {
			for (int i = 0; i < menuPanelInfoList.Count; i++) {
				setMenuPanelActiveState (state, menuPanelInfoList [i].mainPanelButton);
			}
		}
	}

	public void setMenuPanelActive (GameObject panelGameObject)
	{
		setMenuPanelActiveState (true, panelGameObject);
	}

	public void setMenuPanelDeactive (GameObject panelGameObject)
	{
		setMenuPanelActiveState (false, panelGameObject);
	}

	public void setMenuPanelActiveState (bool state, GameObject buttonObject)
	{
		if (enableDisablePanelsEnabled) {
			if (buttonObject != null) {
				int currentIndex = menuPanelInfoList.FindIndex (s => s.mainPanelButton.Equals (buttonObject));

				if (currentIndex > -1) {
					menuPanelInfo currentMenuPanelInfo = menuPanelInfoList [currentIndex];

					currentMenuPanelInfo.mainPanel.SetActive (state);

					if (currentMenuPanelInfo.disabledPanel != null) {
						currentMenuPanelInfo.disabledPanel.SetActive (!state);
					}
				}
			}
		}
	}

	[System.Serializable]
	public class menuPanelInfo
	{
		public string Name;

		public GameObject mainPanel;

		public GameObject disabledPanel;

		public GameObject mainPanelButton;
	}
}
