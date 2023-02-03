using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
using UnityEngine.Video;
#endif

public class playerTutorialSystem : MonoBehaviour
{
	public bool tutorialsEnabled;

	public float minTimeToPressKey = 0.4f;

	public int currentTutorialIndex;
	public int currentTutorialPanelIndex;
	public bool tutorialOpened;

	public bool instantiateTutorialPanelIfNotFound;

	public bool mainTutorialUISystemFound;

	public GameObject tutorialPanelPrefab;

	public playerController playerControllerManager;
	public menuPause pauseManager;
	public AudioSource mainAudioSource;
	public tutorialUISystem mainTutorialUISystem;

	tutorialInfo currentTutorialInfo;
	KeyCode[] validKeyCodes;
	float lastTimeTutorialOpened;
	float previousTimeScale;

	Coroutine resumeCoroutine;

	bool activatingTutorialByNameFromEditorState;

	Coroutine openTutorialCoroutine;

	Coroutine tutorialOpenedCoroutine;

	public Transform tutorialsPanel;
	public AudioSource videoAudioSource;

	public List<tutorialInfo> tutorialInfoList = new List<tutorialInfo> ();

	bool validKeyCodesStored;

	float lastTimeKeyPressed;

	void Start ()
	{
		getTutorialUISystem ();
	}

	void getTutorialUISystem ()
	{
		if (mainTutorialUISystem != null) {
			mainTutorialUISystemFound = true;
		} else {
			if (mainTutorialUISystem == null) {
				mainTutorialUISystem = FindObjectOfType<tutorialUISystem> ();
			}

			if (mainTutorialUISystem != null) {
				mainTutorialUISystemFound = true;
			}
		}
	}

	void getValidKeyCodes ()
	{
		if (!validKeyCodesStored) {
			validKeyCodes = (KeyCode[])System.Enum.GetValues (typeof(KeyCode));

			validKeyCodesStored = true;
		}
	}

	void updateTutorialOpenedState ()
	{
		if (tutorialOpened && (currentTutorialInfo.pressAnyButtonToNextTutorial || pauseManager.isMenuPaused ())) {
			if (Time.unscaledTime > lastTimeTutorialOpened + currentTutorialInfo.timeToEnableKeys ||
			    pauseManager.isMenuPaused () ||
			    currentTutorialInfo.setCustomTimeScale) {

				if (Time.unscaledTime > lastTimeKeyPressed + minTimeToPressKey) {
					foreach (KeyCode vKey in validKeyCodes) {
						if (Input.GetKeyDown (vKey)) {

							lastTimeKeyPressed = Time.unscaledTime;

							setNextPanelOnTutorial ();

							if (!tutorialOpened) {
								stopCheckTutorialOpenedState ();
							}

							return;
						}
					}
				}
			}
		}
	}

	IEnumerator checkTutorialOpenedStateCoroutine ()
	{
		while (true) {
			yield return new WaitForSecondsRealtime (0.0001f);

			updateTutorialOpenedState ();
		}
	}

	public void stopCheckTutorialOpenedState ()
	{
		if (tutorialOpenedCoroutine != null) {
			StopCoroutine (tutorialOpenedCoroutine);
		}
	}

	public void checkTutorialOpenedState ()
	{
		stopCheckTutorialOpenedState ();

		tutorialOpenedCoroutine = StartCoroutine (checkTutorialOpenedStateCoroutine ());
	}


	public void activatingTutorialByNameFromEditor ()
	{
		activatingTutorialByNameFromEditorState = true;
	}

	public void activateTutorialByName (string tutorialName)
	{
		if (!tutorialsEnabled && !pauseManager.isMenuPaused ()) {
			return;
		}

		if (!mainTutorialUISystemFound) {
			bool tutorialFound = false;

			if (instantiateTutorialPanelIfNotFound) {
				instantiateTutorialPanelPrefab ();

				getTutorialUISystem ();

				if (mainTutorialUISystemFound) {
					tutorialFound = true;
				}
			} 

			if (!tutorialFound) {
				print ("WARNING: No tutorial UI system found, make sure to drop it on the scene");

				return;
			}
		}

		tutorialsPanel = mainTutorialUISystem.getTutorialsPanel ();
		videoAudioSource = mainTutorialUISystem.getVideoAudioSource ();

		tutorialInfoList = mainTutorialUISystem.getTutorialInfoList ();

		for (int i = 0; i < tutorialInfoList.Count; i++) {
			if (tutorialInfoList [i].Name.Equals (tutorialName)) {
				currentTutorialInfo = tutorialInfoList [i];

				if (!activatingTutorialByNameFromEditorState) {
					if (currentTutorialInfo.playTutorialOnlyOnce && !pauseManager.isMenuPaused () && currentTutorialInfo.tutorialPlayed) {
						return;
					}
				}

				currentTutorialIndex = i;
				currentTutorialPanelIndex = 0;

				checkOpenOrCloseTutorial (true);

				return;
			}
		}

		print ("WARNING: no tutorial was found with the name " + tutorialName + ", check if the name is properly configured in the event and in the player tutorial system inspector");
	}

	public void checkOpenOrCloseTutorial (bool state)
	{
		if (state) {
			if (pauseManager.isMenuPaused ()) {
				openOrCloseTutorial (true);
			} else {
				stopCheckOpenOrCloseTutorial ();

				playerControllerManager.getPlayerInput ().pauseOrResumeInput (true);

				openTutorialCoroutine = StartCoroutine (checkOpenOrCloseTutorialCoroutine ());
			}
		} else {
			stopCheckOpenOrCloseTutorial ();

			openOrCloseTutorial (false);
		}
	}

	public void stopCheckOpenOrCloseTutorial ()
	{
		if (openTutorialCoroutine != null) {
			StopCoroutine (openTutorialCoroutine);
		}
	}

	IEnumerator checkOpenOrCloseTutorialCoroutine ()
	{
		yield return new WaitForSeconds (currentTutorialInfo.openTutorialDelay);

		openOrCloseTutorial (true);
	}

	public void setNextPanelOnTutorial ()
	{
		if (currentTutorialInfo.tutorialPanelList.Count == 0) {
			closeTutorial ();

			return;
		}

		currentTutorialInfo.tutorialPanelList [currentTutorialPanelIndex].panelGameObject.SetActive (false);
		currentTutorialPanelIndex++;

		if (currentTutorialPanelIndex <= currentTutorialInfo.tutorialPanelList.Count - 1) {
			currentTutorialInfo.tutorialPanelList [currentTutorialPanelIndex].panelGameObject.SetActive (true);

			#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
			if (currentTutorialInfo.containsVideo) {
				currentTutorialInfo.currentVideoIndex++;

				playTutorialVideo (currentTutorialInfo);
			}
			#endif

		} else {
			currentTutorialPanelIndex = currentTutorialInfo.tutorialPanelList.Count - 1;
			closeTutorial ();
		}
	}

	public void setPreviousPanelOnTutorial ()
	{
		if (currentTutorialInfo.tutorialPanelList.Count == 0) {
			return;
		}

		currentTutorialInfo.tutorialPanelList [currentTutorialPanelIndex].panelGameObject.SetActive (false);
		currentTutorialPanelIndex--;

		if (currentTutorialPanelIndex < 0) {
			currentTutorialPanelIndex = 0;
		}

		currentTutorialInfo.tutorialPanelList [currentTutorialPanelIndex].panelGameObject.SetActive (false);
	}

	public void openOrCloseTutorial (bool opened)
	{
		tutorialOpened = opened;

		if (tutorialOpened) {

			lastTimeKeyPressed = 0;

			getValidKeyCodes ();

			checkTutorialOpenedState ();

			currentTutorialInfo.panelGameObject.SetActive (true);

			if (currentTutorialInfo.tutorialPanelList.Count > 0) {
				currentTutorialInfo.tutorialPanelList [0].panelGameObject.SetActive (true);
			}

			lastTimeTutorialOpened = Time.unscaledTime;

			#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
			if (currentTutorialInfo.containsVideo) {
				playTutorialVideo (currentTutorialInfo);
			}
			#endif
		}

		bool isPlayerDriving = playerControllerManager.isPlayerDriving ();

		if (!pauseManager.isMenuPaused ()) {
			if (!tutorialOpened || currentTutorialInfo.unlockCursorOnTutorialActive) {
				if (!isPlayerDriving) {
					pauseManager.showOrHideCursor (tutorialOpened);
				}
			}

			if (!isPlayerDriving) {
				pauseManager.changeCameraState (!tutorialOpened);
				pauseManager.setHeadBobPausedState (!tutorialOpened);
			}

			pauseManager.usingSubMenuState (tutorialOpened);
		}
			
		pauseManager.openOrClosePlayerMenu (tutorialOpened, tutorialsPanel, true);

		if (!isPlayerDriving) {
			pauseManager.usingDeviceState (tutorialOpened);

			playerControllerManager.setUsingDeviceState (tutorialOpened);
	
			playerControllerManager.changeScriptState (!tutorialOpened);
		} else {
			playerControllerManager.getPlayerInput ().setInputCurrentlyActiveState (!tutorialOpened);
		}

		if (tutorialOpened) {
			stopResumePlayerInputCoroutine ();

			playerControllerManager.getPlayerInput ().pauseOrResumeInput (tutorialOpened);
		} else {
			resumePlayerInput ();
		}

		if (pauseManager.isMenuPaused () && !tutorialOpened) {
			pauseManager.resetPauseMenuBlurPanel ();
		}

		if (!pauseManager.isMenuPaused ()) {

			if (!activatingTutorialByNameFromEditorState) {
				currentTutorialInfo.tutorialPlayed = true;
			}

			activatingTutorialByNameFromEditorState = false;

			if (tutorialOpened && currentTutorialInfo.useSoundOnTutorialOpen) {
				if (mainAudioSource) {
					mainAudioSource.PlayOneShot (currentTutorialInfo.soundOnTutorialOpen);
				}
			}

			if (currentTutorialInfo.setCustomTimeScale) {
				if (tutorialOpened) {
					previousTimeScale = Time.timeScale;
					Time.timeScale = currentTutorialInfo.customTimeScale;
				} else {
					Time.timeScale = previousTimeScale;
				}
			}
		}
	}

	public void resumePlayerInput ()
	{
		stopResumePlayerInputCoroutine ();

		if (Time.timeScale == 0) {
			playerControllerManager.getPlayerInput ().pauseOrResumeInput (false);
		} else {
			resumeCoroutine = StartCoroutine (resumePlayerInputCoroutine ());
		}
	}

	public void stopResumePlayerInputCoroutine ()
	{
		if (resumeCoroutine != null) {
			StopCoroutine (resumeCoroutine);
		}
	}

	IEnumerator resumePlayerInputCoroutine ()
	{
		yield return new WaitForSeconds (0.5f);

		playerControllerManager.getPlayerInput ().pauseOrResumeInput (tutorialOpened);
	}

	#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
	Coroutine currentTutorialVideoCoroutine;

	public void playTutorialVideo (tutorialInfo newTutorialInfo)
	{
		stopTutorialVideoCoroutine ();

		currentTutorialVideoCoroutine = StartCoroutine (playTutorialVideoCoroutine (newTutorialInfo));
	}

	public void stopTutorialVideoCoroutine ()
	{
		if (currentTutorialInfo != null) {
			if (currentTutorialInfo.containsVideo) {
				
				VideoPlayer mainVideoPlayer = mainTutorialUISystem.getMainVideoPlayerPanel ().GetComponent<VideoPlayer> ();
				
				if (mainVideoPlayer != null) {
					if (mainVideoPlayer.isPlaying) {
						mainVideoPlayer.Stop ();

						if (videoAudioSource != null) {
							videoAudioSource.Stop ();
						}
					}
				}
			}
		}

		if (currentTutorialVideoCoroutine != null) {
			StopCoroutine (currentTutorialVideoCoroutine);
		}
	}

	IEnumerator playTutorialVideoCoroutine (tutorialInfo newTutorialInfo)
	{
		if (currentTutorialInfo.currentVideoIndex < currentTutorialInfo.videoInfoList.Count) {

			VideoPlayer mainVideoPlayer = mainTutorialUISystem.getMainVideoPlayerPanel ().GetComponent<VideoPlayer> ();

			if (mainVideoPlayer != null) {
				videoInfo currentVideoInfo = currentTutorialInfo.videoInfoList [currentTutorialInfo.currentVideoIndex];

				mainVideoPlayer.source = VideoSource.VideoClip;

				mainVideoPlayer.clip = currentVideoInfo.videoFile;

				//Set Audio Output to AudioSource
				mainVideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

				//Assign the Audio from Video to AudioSource to be played

				mainVideoPlayer.EnableAudioTrack (0, true);
				mainVideoPlayer.SetTargetAudioSource (0, videoAudioSource);

				if (currentVideoInfo.useVideoAudio) {
					videoAudioSource.volume = currentVideoInfo.videoAudioVolume;
				} else {
					videoAudioSource.volume = 0;
				}
				
				//Set video To Play then prepare Audio to prevent Buffering
				mainVideoPlayer.Prepare ();

				//Wait until video is prepared
				while (!mainVideoPlayer.isPrepared) {
					yield return null;
				}

				//Assign the Texture from Video to RawImage to be displayed
				currentVideoInfo.videoRawImage.texture = mainVideoPlayer.texture;

				//Play Video
				mainVideoPlayer.Play ();

				//Play Sound
				if (currentVideoInfo.useVideoAudio) {
					videoAudioSource.Play ();
				}
			
				while (mainVideoPlayer.isPlaying) {
					//Debug.LogWarning ("Video Time: " + Mathf.FloorToInt ((float)mainVideoPlayer.time));
					yield return null;
				}

				if (currentVideoInfo.loopVideo) {
					playTutorialVideo (currentTutorialInfo);
				}
			} else {
				yield return null;
			}

			if (currentTutorialInfo.setNextPanelWhenVideoEnds) {
				setNextPanelOnTutorial ();
			}
		}
	}
	#endif

	public void closeTutorial ()
	{
		if (currentTutorialInfo.tutorialPanelList.Count > 0) {
			currentTutorialInfo.tutorialPanelList [currentTutorialPanelIndex].panelGameObject.SetActive (false);
		}
			
		checkOpenOrCloseTutorial (false);

		#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
		if (currentTutorialInfo.containsVideo) {
			
			stopTutorialVideoCoroutine ();

			currentTutorialInfo.currentVideoIndex = 0;
		}
		#endif

		currentTutorialInfo.panelGameObject.SetActive (false);
	}

	public void callNextTutorialPanel ()
	{
		if (tutorialOpened) {
			if (currentTutorialInfo.useActionButtonToMoveThroughTutorial) {
				setNextPanelOnTutorial ();
			}
		}
	}

	public void resetAllTutorials ()
	{
		for (int i = 0; i < tutorialInfoList.Count; i++) {
			if (tutorialInfoList [i].panelGameObject != null) {
				tutorialInfoList [i].panelGameObject.SetActive (false);
			}

			for (int j = 0; j < tutorialInfoList [i].tutorialPanelList.Count; j++) {
				if (tutorialInfoList [i].tutorialPanelList [j].panelGameObject != null) {
					if (j == 0) {
						tutorialInfoList [i].tutorialPanelList [j].panelGameObject.SetActive (true);
					} else {
						tutorialInfoList [i].tutorialPanelList [j].panelGameObject.SetActive (false);
					}
				}
			}
		}
	}

	public void setTutorialsEnabledState (bool state)
	{
		tutorialsEnabled = state;
	}

	public void setTutorialUISystem (tutorialUISystem newTutorialUISystem)
	{
		mainTutorialUISystem = newTutorialUISystem;

		updateComponent ();
	}

	public void instantiateTutorialPanelPrefab ()
	{
		if (mainTutorialUISystem == null) {
			if (tutorialPanelPrefab == null) {
				return;
			}

			GameObject newTutorialPanel = (GameObject)Instantiate (tutorialPanelPrefab, Vector3.zero, Quaternion.identity);
			newTutorialPanel.name = tutorialPanelPrefab.name;

			tutorialUISystem currentTutorialUISystem = newTutorialPanel.GetComponentInChildren<tutorialUISystem> ();

			if (currentTutorialUISystem != null) {
				currentTutorialUISystem.searchPlayerTutorialSystem ();
			}
		} else {
			print ("Tutorial Panel is already on the scene");
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}
