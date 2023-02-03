using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openMenuSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool openMenuEnabled;

	public string menuToOpenName;

	public bool openPauseMenu;

	public float openMenuDelay;
	public float closeMenuDelay;

	public bool pauseEscapeMenuKey = true;

	[Space]
	[Header ("Blur Settings")]
	[Space]

	public bool ignoreBlurMenu;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool menuOpened;

	[Space]
	[Header ("Components")]
	[Space]

	public menuPause mainMenuPause;

	public GameObject currentPlayer;

	Coroutine menuCoroutine;


	public void toggleOpenOrCloseMenu ()
	{
		openOrCloseMenu (!menuOpened);
	}

	public void openMenu ()
	{
		openOrCloseMenu (true);
	}

	public void closeMenu ()
	{
		openOrCloseMenu (false);
	}

	public void closeMenuIfOpened ()
	{
		if (menuOpened) {
			closeMenu ();
		}
	}

	public void openOrCloseMenu (bool state)
	{
		if (!openMenuEnabled) {
			return;
		}

		if (mainMenuPause == null) {
			getPauseManager ();
		}

		if (mainMenuPause == null) {
			print ("WARNING: no player assigned in open menu system, make sure to assign it or send it through events to the " +
			" function set current player");

			return;
		}

		stopOpenOrCloseMenuCoroutine ();

		menuCoroutine = StartCoroutine (openOrCloseMenuCoroutine (state));
	}

	public void stopOpenOrCloseMenuCoroutine ()
	{
		if (menuCoroutine != null) {
			StopCoroutine (menuCoroutine);
		}
	}

	IEnumerator openOrCloseMenuCoroutine (bool state)
	{
		menuOpened = state;

		if (menuOpened) {
			if (pauseEscapeMenuKey) {
				mainMenuPause.setPauseGameInputPausedState (true);
			} else {
				mainMenuPause.setCurrentOpenMenuSystem (this);
			}

			mainMenuPause.setChangeBetweenIngameMenuPausedState (true);

			if (ignoreBlurMenu) {
				mainMenuPause.setIgnoreBlurPanelActiveState (true);
			}
		}

		if (menuOpened) {
			yield return new WaitForSeconds (openMenuDelay);
		} else {
			yield return new WaitForSeconds (closeMenuDelay);
		}

		if (!menuOpened) {
			mainMenuPause.setOpenOrClosePlayerOpenMenuByNamePausedState (false);
		}

		if (openPauseMenu) {
			mainMenuPause.openOrClosePauseMenuByName (menuToOpenName, menuOpened);

			mainMenuPause.setOpenOrClosePauseMenuExternallyPausedState (menuOpened);

			mainMenuPause.setPauseScreenWithoutPausingGameState (menuOpened);
		} else {
			mainMenuPause.openPlayerOpenMenuByName (menuToOpenName);
		}

		if (menuOpened) {
			mainMenuPause.setOpenOrClosePlayerOpenMenuByNamePausedState (true);
		} else {
			if (pauseEscapeMenuKey) {
				mainMenuPause.setPauseGameInputPausedState (false);
			} else {
				mainMenuPause.setCurrentOpenMenuSystem (null);
			}

			mainMenuPause.setChangeBetweenIngameMenuPausedState (false);

			if (ignoreBlurMenu) {
				mainMenuPause.setIgnoreBlurPanelActiveState (false);
			}
		}
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		if (currentPlayer != newPlayer) {
			currentPlayer = newPlayer;

			getPauseManager ();
		}
	}

	public void getPauseManager ()
	{
		if (currentPlayer != null) {
			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				mainMenuPause = currentPlayerComponentsManager.getPauseManager ();
			}
		}
	}
}
