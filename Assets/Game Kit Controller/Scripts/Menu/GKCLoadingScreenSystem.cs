using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class GKCLoadingScreenSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int currentSceneIndexToLoad;

	public bool activateLoadOnStart;

	public string sceneToLoadAsyncPrefsName = "SceneToLoadAsync";

	public float loadScreenProgressLerpSpeed = 10;

	public bool useFakeLoadProgress;
	public float fakeLoadSpeed;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool activateChangeOfSceneOnAnyKeyPressDownEnabled = true;

	public bool ignoreKeyPressedActive;

	public bool ignoreMousePressed;

	public bool useCircleImageLoad = true;

	public bool useSliderLoad = true;

	public bool useTextLoad = true;

	public bool useTextPercentageLoad = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool sceneLoaded;
	public bool showDebugPrint = true;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnLoadComplete;
	public UnityEvent eventOnLoadComplete;

	[Space]
	[Header ("Components")]
	[Space]

	public Image fill;

	public Slider mainSlider;

	public Text mainLoadingText;

	public Text percentegeText;

	bool keyPressed = false;

	float currentProgressPercentage;


	void Start ()
	{
		bool canActivateLoadResult = false;

		if (activateLoadOnStart) {
			canActivateLoadResult = true;
		} else {
			if (PlayerPrefs.HasKey (sceneToLoadAsyncPrefsName)) {
				currentSceneIndexToLoad = PlayerPrefs.GetInt (sceneToLoadAsyncPrefsName);

				PlayerPrefs.DeleteKey (sceneToLoadAsyncPrefsName);

				canActivateLoadResult = true;
			}
		}

		if (currentSceneIndexToLoad > -1) {
			if (canActivateLoadResult) {
				activateSceneLoad ();
			}
		}
	}

	void Update ()
	{
		if (sceneLoaded) {
			if (activateChangeOfSceneOnAnyKeyPressDownEnabled) {
				if (!ignoreKeyPressedActive) {
					if (Input.anyKeyDown) {
						if (ignoreMousePressed) {
							if (!Input.GetMouseButton (0) && !Input.GetMouseButton (1) && !Input.GetMouseButton (2)) {
								keyPressed = true;
							}
						} else {
							keyPressed = true;
						}
					}
				}
			} else {
				keyPressed = true;
			}
		}
	}

	void activateSceneLoad ()
	{
		StartCoroutine (LoadAsync (currentSceneIndexToLoad));
	}

	IEnumerator LoadAsync (int sceneIndex)
	{
		sceneLoaded = false;

		menuPause.setCursorVisibleState (true);

		menuPause.setCursorLockState (false);

		if (showDebugPrint) {
			print ("Scene Index To Load Async " + sceneIndex);
		}

		AsyncOperation operation = SceneManager.LoadSceneAsync (sceneIndex, LoadSceneMode.Single);

		operation.allowSceneActivation = false;

		currentProgressPercentage = 0;

		float fakeProgress = 0;

		bool targetReached = false;

		float progressValue = 0;

		float progressToShowOnScreen = 0;
	
		if (useTextLoad) {
			mainLoadingText.text = "Loading... "; 
		}

		while (!targetReached) {
			float currentProgress = operation.progress;

			if (useFakeLoadProgress || currentProgress >= 0.9f) {
				fakeProgress += Time.fixedDeltaTime * fakeLoadSpeed;

				currentProgress += fakeProgress;
			}

			progressValue = Mathf.Clamp01 (currentProgress);

			currentProgressPercentage = (progressValue * 100f);

			progressToShowOnScreen = Mathf.MoveTowards (progressToShowOnScreen, currentProgressPercentage, Time.fixedDeltaTime * loadScreenProgressLerpSpeed);

			if (useCircleImageLoad) {
				fill.fillAmount = progressToShowOnScreen / 100;
			}

			if (useTextPercentageLoad) {
				percentegeText.text = (int)progressToShowOnScreen + " %"; 
			}

			if (useSliderLoad) {
				mainSlider.value = progressToShowOnScreen; 
			}

			if (currentProgress >= 0.9f && progressValue >= 1 && progressToShowOnScreen >= 100) {
				targetReached = true;

				sceneLoaded = true;
			}

			yield return null;
		}

		if (useTextLoad) {
			mainLoadingText.text = "Click anywhere to start.";
		}

		if (useEventOnLoadComplete) {
			eventOnLoadComplete.Invoke ();
		}

		while (!keyPressed) {
			yield return null;
		}

		operation.allowSceneActivation = true;
	}

	public void setKeyPressedState (bool state)
	{
		keyPressed = state;
	}
}