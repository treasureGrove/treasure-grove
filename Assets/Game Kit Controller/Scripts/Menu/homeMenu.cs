using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class homeMenu : MonoBehaviour
{
	[Space]
	[Header ("Main Settings")]
	[Space]

	public bool useInitialUIElementSelected;
	public GameObject initialUIElementSelected;

	public string mainMenuName = "Main Menu";

	[Space]
	[Header ("Load Screen Settings")]
	[Space]

	public bool useLoadScreen;

	public int loadScreenScene = 1;

	public bool useLastSceneIndexAsLoadScreen = true;

	public string sceneToLoadAsyncPrefsName = "SceneToLoadAsync";

	public bool checkLoadingScreenSceneConfigured = true;

	public string loadingScreenSceneName = "Loading Screen Scene";
	
	[Space]
	[Header ("Debug")]
	[Space]

	public bool subMenuActive;

	[Space]
	[Header ("Menu Settings")]
	[Space]

	public List<menuPause.submenuInfo> submenuInfoList = new List<menuPause.submenuInfo> ();


	void Start ()
	{
		if (useInitialUIElementSelected) {
			GKC_Utils.setSelectedGameObjectOnUI (false, true, initialUIElementSelected, false);
		}

		AudioListener.pause = false;

		Time.timeScale = 1;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
	
			openOrClosePauseMenuByName (mainMenuName, true);
		}
	}

	public void confirmExit ()
	{
		Application.Quit ();
	}

	public void loadScene (int sceneNumber)
	{
		GKC_Utils.loadScene (sceneNumber, useLoadScreen, loadScreenScene, sceneToLoadAsyncPrefsName, 
			useLastSceneIndexAsLoadScreen, checkLoadingScreenSceneConfigured, loadingScreenSceneName);
	}

	public void disableMenuList ()
	{
		for (int i = 0; i < submenuInfoList.Count; i++) {	
			if (submenuInfoList [i].menuGameObject != null && submenuInfoList [i].menuGameObject.activeSelf != false) {
				submenuInfoList [i].menuGameObject.SetActive (false);
			}

			if (submenuInfoList [i].menuOpened) {
				submenuInfoList [i].menuOpened = false;

				if (submenuInfoList [i].useEventOnClose) {
					if (submenuInfoList [i].eventOnClose.GetPersistentEventCount () > 0) {
						submenuInfoList [i].eventOnClose.Invoke ();
					}
				}
			}
		}
	}

	public void openOrClosePauseMenuByName (string menuName, bool state)
	{
		int pauseMenuIndex = submenuInfoList.FindIndex (s => s.Name.Equals (menuName));

		if (pauseMenuIndex > -1) {
			if (state) {
				for (int i = 0; i < submenuInfoList.Count; i++) {	
					if (pauseMenuIndex != i) {
						if (submenuInfoList [i].menuGameObject != null && submenuInfoList [i].menuGameObject.activeSelf != false) {
							submenuInfoList [i].menuGameObject.SetActive (false);
						}

						if (submenuInfoList [i].menuOpened) {
							submenuInfoList [i].menuOpened = false;

							if (submenuInfoList [i].useEventOnClose) {
								if (submenuInfoList [i].eventOnClose.GetPersistentEventCount () > 0) {
									submenuInfoList [i].eventOnClose.Invoke ();
								}
							}

							if (submenuInfoList [i].isSubMenu) {	
								exitSubMenu ();
							}
						}
					}
				}
			}

			menuPause.submenuInfo currentSubmenuInfo = submenuInfoList [pauseMenuIndex];

			if (state) {
				currentSubmenuInfo.eventOnOpen.Invoke ();
			} else {
				currentSubmenuInfo.eventOnClose.Invoke ();
			}

			if (currentSubmenuInfo.menuGameObject != null && currentSubmenuInfo.menuGameObject.activeSelf != state) {
				currentSubmenuInfo.menuGameObject.SetActive (state);
			}

			currentSubmenuInfo.menuOpened = state;

			if (currentSubmenuInfo.isSubMenu) {	
				if (state) {
					enterSubMenu ();
				} else {
					exitSubMenu ();
				}
			}
		}
	}

	public void openPauseMenuByName (string menuName)
	{
		openOrClosePauseMenuByName (menuName, true);
	}

	public void closePauseMenuByName (string menuName)
	{
		openOrClosePauseMenuByName (menuName, false);
	}

	public void enterSubMenu ()
	{
		subMenuActive = true;
	}

	public void exitSubMenu ()
	{
		subMenuActive = false;
	}
}