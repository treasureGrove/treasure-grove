using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;

public class playerExperienceSystem : MonoBehaviour
{
	public bool experienceSystemEnabled = true;

	public string XPExtraString = " XP";

	public List<levelInfo> levelInfoList = new List<levelInfo> ();

	public List<statUIInfo> statUIInfoList = new List<statUIInfo> ();

	public int currentLevelExperienceAmount;

	public int currentLevelExperienceToNextLevel;

	public int totalExperienceAmount;

	public int totalExperiencePoints;

	public int currentLevel = 1;

	public int totalExperienceAmountLimit = 9999999;

	public float experienceTextMovementSpeed;
	public float experienceTextMovementDelay;

	public float timeToShowLevelUpPanel;

	[Range (0, 1)] public float experienceAmountTextOffset;

	public bool useMaxLevel;
	public int maxLevel;

	public bool hideExperienceSliderAfterTime;
	public float timeToHideExperienceSlider;

	public bool experienceMenuActive = true;
	public bool experienceMenuOpened;

	public float currentExperienceMultiplier = 1;
	public bool experienceMultiplerEnabled;
	public GameObject experienceMultiplierTextPanel;
	public Text experienceMultiplerText;
	public UnityEvent eventOnExperienceMultiplerEnabled;
	public UnityEvent eventOnExperienceMultiplerDisabled;

	public string extraTextOnLevelNumber = "LV ";
	public string extraTextOnNewLevelNumber = "LV ";

	public string totalExperiencePointsName = "Skill Points";
	public string totalExperienceAmountName = "Experience";
	public string currentLevelExperienceAmountName = "Current Level Experience Amount";
	public string currentLevelExperienceToNextLevelName = "Current Level Experience To Next Level Amount";
	public string currentLevelName = "Level";

	public bool setLevelManually;

	public eventParameters.eventToCallWithString eventOnExperienceWithoutTextPosition;

	public Transform experienceSliderPanelPositionInsideMenu;
	public Transform experienceSliderPanelPositionOutsideMenu;

	public GameObject statsMenuPanel;

	public UnityEvent eventOnExperienceMenuOpened;
	public UnityEvent eventOnExperienceMenuClosed;

	public playerController playerControllerManager;
	public playerCamera mainPlayerCamera;
	public GameObject experienceMenuGameObject;
	public GameObject experienceObtaniedTextPrefab;
	public Transform experienceObtainedTextParent;

	public Text currentLevelText;

	public GameObject levelUpPanel;

	public Text levelUpText;

	public RectTransform experienceTextTargetPosition;
	public RectTransform experienceSliderTransform;

	public Slider experienceSlider;

	public GameObject experienceSliderPanel;

	public AudioSource levelUpAudioSource;
	public AudioClip levelUpAudioClip;

	public playerSkillsSystem playerSkillManager;
	public playerStatsSystem playerStatsManager;
	public Camera mainCamera;
	public menuPause pauseManager;

	public bool useEventIfSystemDisabled;
	public UnityEvent eventIfSystemDisabled;

	public bool callEventOnLevelReachedOnLoadEnabled = true;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;
	Vector3 screenPoint;
	bool targetOnScreen;

	Coroutine showLevelUpCoroutine;

	Coroutine showSliderCoroutine;

	float angle;
	float cos;
	float sin;
	float m;
	Vector3 screenBounds;
	Vector3 screenCenter;

	levelInfo currentLevelInfo;

	bool startInitialized;

	float screenWidth;
	float screenHeight;

	bool isLoadingGame;

	void Start ()
	{
		if (!experienceSystemEnabled) {
			checkEventOnSystemDisabled ();

			if (experienceSliderPanel.activeSelf) {
				experienceSliderPanel.SetActive (false);
			}

			return;
		}

		mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();

		if (setLevelManually && currentLevel > 1) {
			for (int i = 0; i < currentLevel; i++) {

				currentLevelInfo = levelInfoList [i];

				totalExperienceAmount += currentLevelInfo.experienceRequiredToNextLevel;

				if (i + 1 <= levelInfoList.Count) {

					currentLevelInfo = levelInfoList [i + 1];

					currentLevelInfo.eventsOnLevelReached.Invoke ();

					currentLevelInfo.levelUnlocked = true;

					totalExperiencePoints += currentLevelInfo.experiencePointsAmount;

					currentLevelExperienceToNextLevel = currentLevelInfo.experienceRequiredToNextLevel;
				}
			}
		}

		if (callEventOnLevelReachedOnLoadEnabled) {
			isLoadingGame = playerSkillManager.isLoadingGameActive ();

			if (isLoadingGame) {
				if (currentLevel > 1) {
					for (int i = 0; i < currentLevel; i++) {

						currentLevelInfo = levelInfoList [i];

						if (currentLevelInfo.levelNumber <= currentLevel) {
							if (currentLevelInfo.callEventOnLevelReachedOnLoad) {
								currentLevelInfo.eventsOnLevelReached.Invoke ();
							}
						}
					}
				}
			}
		}

		playerStatsManager.updateStatValue (totalExperiencePointsName, totalExperiencePoints);

		playerStatsManager.updateStatValue (currentLevelExperienceToNextLevelName, currentLevelExperienceToNextLevel);

		playerStatsManager.updateStatValue (currentLevelExperienceAmountName, currentLevelExperienceAmount);

		playerStatsManager.updateStatValue (totalExperienceAmountName, totalExperienceAmount);

		updateExperienceInfo ();

		if (hideExperienceSliderAfterTime) {
			if (experienceSliderPanel.activeSelf) {
				experienceSliderPanel.SetActive (false);
			}
		}

		StartCoroutine (initializeComponent ());
	}

	public void checkEventOnSystemDisabled ()
	{
		if (useEventIfSystemDisabled) {
			eventIfSystemDisabled.Invoke ();
		}
	}

	public bool checkIncreasePlayerLevel ()
	{
		if (useMaxLevel) {
			if (currentLevel >= maxLevel) {
				return false;
			}
		}

		if (currentLevel <= levelInfoList.Count) {

			currentLevelInfo = levelInfoList [currentLevel - 1];

			if (currentLevelExperienceAmount >= currentLevelInfo.experienceRequiredToNextLevel) {

				if (currentLevel + 1 <= levelInfoList.Count) {
					currentLevel++;

					currentLevelInfo = levelInfoList [currentLevel - 1];

					currentLevelInfo.eventsOnLevelReached.Invoke ();

					currentLevelInfo.levelUnlocked = true;

					currentLevelExperienceAmount = currentLevelExperienceAmount - levelInfoList [currentLevel - 2].experienceRequiredToNextLevel;

					totalExperiencePoints += currentLevelInfo.experiencePointsAmount;

					updateExperienceInfo ();

					if (startInitialized) {
						showLevelUpPanel ();
					}

					currentLevelExperienceToNextLevel = currentLevelInfo.experienceRequiredToNextLevel;

					for (int k = 0; k < currentLevelInfo.statInfoList.Count; k++) {

						if (currentLevelInfo.statInfoList [k].statIsAmount) {
							float extraValue = currentLevelInfo.statInfoList [k].statExtraValue;
							if (currentLevelInfo.statInfoList [k].useRandomRange) {
								extraValue = Random.Range (currentLevelInfo.statInfoList [k].randomRange.x, currentLevelInfo.statInfoList [k].randomRange.y);

								extraValue = Mathf.RoundToInt (extraValue);
							}

							playerStatsManager.increasePlayerStat (currentLevelInfo.statInfoList [k].Name, extraValue);
						} else {
							playerStatsManager.enableOrDisableBoolPlayerStat (currentLevelInfo.statInfoList [k].Name, currentLevelInfo.statInfoList [k].newBoolState);
						}

						if (currentLevelInfo.statInfoList [k].unlockSkill) {
							playerSkillManager.unlockSkillSlotByName (currentLevelInfo.statInfoList [k].skillNameToUnlock);
						}
					}

					playerStatsManager.updateStatValue (totalExperiencePointsName, totalExperiencePoints);

					playerStatsManager.updateStatValue (currentLevelExperienceToNextLevelName, currentLevelExperienceToNextLevel);

					playerStatsManager.updateStatValue (currentLevelExperienceAmountName, currentLevelExperienceAmount);

					playerStatsManager.updateStatValue (currentLevelName, currentLevel);

					return true;
				} else {
					
					if (currentLevelExperienceAmount > levelInfoList [currentLevel - 1].experienceRequiredToNextLevel) {
						currentLevelExperienceAmount = levelInfoList [currentLevel - 1].experienceRequiredToNextLevel;
					}
					
					if (totalExperienceAmount > totalExperienceAmountLimit) {
						totalExperienceAmount = totalExperienceAmountLimit;
					}
					
					playerStatsManager.updateStatValue (totalExperienceAmountName, totalExperienceAmount);
					playerStatsManager.updateStatValue (currentLevelExperienceAmountName, currentLevelExperienceAmount);
				}
			} 
		} 

		return false;
	}

	public void getExperienceAmount (int newExperienceAmount)
	{
		if (!experienceSystemEnabled) {
			return;
		}

		getExperienceAmount (newExperienceAmount, null, false, "");
	}

	public void getExperienceAmount (int newExperienceAmount, Transform objectTransform, bool useTransformAsExpTextPosition, string extraExperienceText)
	{
		if (!experienceSystemEnabled) {
			return;
		}

		newExperienceAmount *= (int)currentExperienceMultiplier;

		totalExperienceAmount += newExperienceAmount;

		currentLevelExperienceAmount += newExperienceAmount;
	
		experienceSlider.value = currentLevelExperienceAmount;

		playerStatsManager.updateStatValue (totalExperienceAmountName, totalExperienceAmount);
		playerStatsManager.updateStatValue (currentLevelExperienceAmountName, currentLevelExperienceAmount);

		float experienceAcumulated = currentLevelExperienceAmount;

		if (checkIncreasePlayerLevel ()) {
			for (int k = 0; k < levelInfoList.Count; k++) {
				if (!levelInfoList [k].levelUnlocked) {
					
					experienceAcumulated -= levelInfoList [currentLevel - 1].experienceRequiredToNextLevel;
				
					if (experienceAcumulated > 0) {
						checkIncreasePlayerLevel ();
					}
				}
			}
		}

		if (!startInitialized) {
			return;
		}

		if (hideExperienceSliderAfterTime) {
			if (!experienceSliderPanel.activeSelf) {
				experienceSliderPanel.SetActive (true);
			}

			showExperienceSlider ();
		}

		if (mainCamera == null) {
			mainCamera = mainPlayerCamera.getMainCamera ();
		}
			
		if (useTransformAsExpTextPosition) {
			if (!usingScreenSpaceCamera) {
				screenWidth = Screen.width;
				screenHeight = Screen.height;
			}

			Vector3 objectPosition = objectTransform.position;

			if (usingScreenSpaceCamera) {
				screenPoint = mainCamera.WorldToViewportPoint (objectPosition);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
			} else {
				screenPoint = mainCamera.WorldToScreenPoint (objectPosition);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
			}

			GameObject newExperienceText = (GameObject)Instantiate (experienceObtaniedTextPrefab, Vector3.zero, Quaternion.identity, experienceObtainedTextParent);

			newExperienceText.transform.localScale = Vector3.one;

			if (!newExperienceText.activeSelf) {
				newExperienceText.SetActive (true);
			}

			experienceObtainedInfo currentexperienceObtainedInfo = newExperienceText.GetComponent<experienceObtainedInfoPanel> ().experienceObtainedInfo;

			if (usingScreenSpaceCamera) {
				if (targetOnScreen) {
					iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, 
						(screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

					currentexperienceObtainedInfo.experienceObtainedRectTransform.anchoredPosition = iconPosition2d;
				} else {
					iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x,
						(screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

					if (screenPoint.z < 0) {
						iconPosition2d *= -1;
					}

					angle = Mathf.Atan2 (iconPosition2d.y, iconPosition2d.x);
					angle -= 90 * Mathf.Deg2Rad;
					cos = Mathf.Cos (angle);
					sin = -Mathf.Sin (angle);
					m = cos / sin;
					screenBounds = halfMainCanvasSizeDelta * experienceAmountTextOffset;
					if (cos > 0) {
						iconPosition2d = new Vector2 (screenBounds.y / m, screenBounds.y);
					} else {
						iconPosition2d = new Vector2 (-screenBounds.y / m, -screenBounds.y);
					}

					if (iconPosition2d.x > screenBounds.x) {
						iconPosition2d = new Vector2 (screenBounds.x, screenBounds.x * m);
					} else if (iconPosition2d.x < -screenBounds.x) {
						iconPosition2d = new Vector2 (-screenBounds.x, -screenBounds.x * m);
					}

					currentexperienceObtainedInfo.experienceObtainedRectTransform.anchoredPosition = iconPosition2d;
				}
			} else {
				if (targetOnScreen) {
					currentexperienceObtainedInfo.experienceObtainedRectTransform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);		
				} else {
					if (screenPoint.z < 0) {
						screenPoint *= -1;
					}

					screenCenter = new Vector3 (screenWidth, screenHeight, 0) / 2;
					screenPoint -= screenCenter;
					angle = Mathf.Atan2 (screenPoint.y, screenPoint.x);
					angle -= 90 * Mathf.Deg2Rad;
					cos = Mathf.Cos (angle);
					sin = -Mathf.Sin (angle);
					m = cos / sin;
					screenBounds = screenCenter * experienceAmountTextOffset;
					if (cos > 0) {
						screenPoint = new Vector3 (screenBounds.y / m, screenBounds.y, 0);
					} else {
						screenPoint = new Vector3 (-screenBounds.y / m, -screenBounds.y, 0);
					}

					if (screenPoint.x > screenBounds.x) {
						screenPoint = new Vector3 (screenBounds.x, screenBounds.x * m, 0);
					} else if (screenPoint.x < -screenBounds.x) {
						screenPoint = new Vector3 (-screenBounds.x, -screenBounds.x * m, 0);
					}

					//set the position and rotation of the arrow
					screenPoint += screenCenter;

					currentexperienceObtainedInfo.experienceObtainedRectTransform.position = screenPoint;
				}
			}

			currentexperienceObtainedInfo.experienceAmountText.text = newExperienceAmount + XPExtraString;
			currentexperienceObtainedInfo.experienceAmount = newExperienceAmount;

			currentexperienceObtainedInfo.moveExperienceTextPanel = StartCoroutine (moveExperienceTextPanelCoroutine (currentexperienceObtainedInfo));
		} else {
			eventOnExperienceWithoutTextPosition.Invoke (extraExperienceText + newExperienceAmount + XPExtraString);
		}
	}

	IEnumerator moveExperienceTextPanelCoroutine (experienceObtainedInfo currentExperienceObtainedInfo)
	{
		yield return new WaitForSeconds (experienceTextMovementDelay);

		float dist = GKC_Utils.distance (currentExperienceObtainedInfo.experienceObtainedRectTransform.position, experienceSliderTransform.position);

		float duration = dist / experienceTextMovementSpeed;

		float t = 0;

		Vector2 targetPosition = experienceTextTargetPosition.position;

		Vector2 currentPosition = currentExperienceObtainedInfo.experienceObtainedRectTransform.position;

		float movementTimer = 0;

		bool targetReached = false;
		Vector2 experienceAmountPosition = Vector2.zero;
		Vector2 experienceSliderTransformPosition = Vector2.zero;
		float distance = 0;

		while (!targetReached) {
			t += Time.deltaTime / duration; 

			currentExperienceObtainedInfo.experienceObtainedRectTransform.position = 
				Vector2.Lerp (currentExperienceObtainedInfo.experienceObtainedRectTransform.position, targetPosition, t);
			
			experienceAmountPosition = new Vector2 (currentExperienceObtainedInfo.experienceObtainedRectTransform.position.y, 0);
			experienceSliderTransformPosition = new Vector2 (targetPosition.y, 0);

			distance = GKC_Utils.distance (experienceAmountPosition, experienceSliderTransformPosition);

			movementTimer += Time.deltaTime;

			if (distance < 0.01f || movementTimer > (duration + 1)) {
				targetReached = true;
			}

			yield return null;
		}
	
		Destroy (currentExperienceObtainedInfo.experienceObtainedRectTransform.gameObject);

		StopCoroutine (currentExperienceObtainedInfo.moveExperienceTextPanel);
	}

	public void updateExperienceInfo ()
	{
		experienceSlider.maxValue = levelInfoList [currentLevel - 1].experienceRequiredToNextLevel;

		experienceSlider.value = currentLevelExperienceAmount;

		if (extraTextOnLevelNumber != "") {
			currentLevelText.text = extraTextOnLevelNumber;
		}

		currentLevelText.text += currentLevel;
	}

	public void showLevelUpPanel ()
	{
		stopShowLevelUpPanelCoroutine ();

		showLevelUpCoroutine = StartCoroutine (showLevelUpPanelCoroutine ());
	}

	public void stopShowLevelUpPanelCoroutine ()
	{
		if (showLevelUpCoroutine != null) {
			StopCoroutine (showLevelUpCoroutine);
		}
	}

	IEnumerator showLevelUpPanelCoroutine ()
	{
		if (levelUpAudioSource && levelUpAudioClip) {
			levelUpAudioSource.PlayOneShot (levelUpAudioClip);
		}

		if (!levelUpPanel.activeSelf) {
			levelUpPanel.SetActive (true);
		}

		if (extraTextOnNewLevelNumber != "") {
			levelUpText.text = extraTextOnNewLevelNumber;
		}

		levelUpText.text += currentLevel;

		yield return new WaitForSeconds (timeToShowLevelUpPanel);

		levelUpPanel.SetActive (false);
	}

	public void showExperienceSlider ()
	{
		stopShowExperienceSliderCoroutine ();

		showSliderCoroutine = StartCoroutine (showExperienceSliderCoroutine ());
	}

	public void stopShowExperienceSliderCoroutine ()
	{
		if (showSliderCoroutine != null) {
			StopCoroutine (showSliderCoroutine);
		}
	}

	IEnumerator showExperienceSliderCoroutine ()
	{
		yield return new WaitForSeconds (timeToHideExperienceSlider);

		if (experienceSliderPanel.activeSelf) {
			experienceSliderPanel.SetActive (false);
		}
	}

	public void openOrCloseExperienceMenu (bool state)
	{
		if ((!playerControllerManager.isPlayerMenuActive () || experienceMenuOpened) && (!playerControllerManager.isUsingDevice () || playerControllerManager.isPlayerDriving ()) && !pauseManager.isGamePaused ()) {
			experienceMenuOpened = state;

			pauseManager.openOrClosePlayerMenu (experienceMenuOpened, experienceMenuGameObject.transform, true);

			experienceMenuGameObject.SetActive (experienceMenuOpened);

			pauseManager.setIngameMenuOpenedState ("Experience System", experienceMenuOpened, true);

			pauseManager.enableOrDisablePlayerMenu (experienceMenuOpened, true, false);

			if (experienceMenuOpened) {
				updateStatsInfo ();

				experienceSliderPanel.transform.SetParent (experienceSliderPanelPositionInsideMenu);

				if (hideExperienceSliderAfterTime) {
					if (!experienceSliderPanel.activeSelf) {
						experienceSliderPanel.SetActive (true);
					}
				}

				eventOnExperienceMenuOpened.Invoke ();
			} else {
				experienceSliderPanel.transform.SetParent (experienceSliderPanelPositionOutsideMenu);

				if (hideExperienceSliderAfterTime) {
					if (experienceSliderPanel.activeSelf) {
						experienceSliderPanel.SetActive (false);
					}
				}

				eventOnExperienceMenuClosed.Invoke ();
			}

			experienceSliderPanel.transform.localPosition = Vector3.zero;
		}
	}

	public void enableOrDisableHUD (bool state)
	{
		if (!hideExperienceSliderAfterTime) {
			if (experienceSliderPanel.activeSelf != state) {
				experienceSliderPanel.SetActive (state);
			}
		}
	}

	public void openOrCLoseObjectiveMenuFromTouch ()
	{
		openOrCloseExperienceMenu (!experienceMenuOpened);
	}

	public void inputOpenOrCloseObjectiveMenu ()
	{
		if (experienceMenuActive) {

			if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
				return;
			}

			openOrCloseExperienceMenu (!experienceMenuOpened);
		}
	}

	public void updateStatsInfo ()
	{
		for (int i = 0; i < statUIInfoList.Count; i++) {
			if (statUIInfoList [i].statIsAmount) {
				statUIInfoList [i].statAmountText.text = 
					statUIInfoList [i].extraTextAtStart + playerStatsManager.getStatValue (statUIInfoList [i].Name) + statUIInfoList [i].extraTextAtEnd;
			} else {
				statUIInfoList [i].statAmountText.text = 
					statUIInfoList [i].extraTextAtStart + playerStatsManager.getBoolStatValue (statUIInfoList [i].Name) + statUIInfoList [i].extraTextAtEnd;
			}
		}
	}

	public void initializeCurrentLevelExperienceAmount (float newValue)
	{
		currentLevelExperienceAmount = (int)newValue;
	}

	public void initializeTotalExperienceAmount (float newValue)
	{
		totalExperienceAmount = (int)newValue;
	}

	public void initializeTotalExperiencePointsAmount (float newValue)
	{
		totalExperiencePoints = (int)newValue;
	}

	public void initializeCurrentLevelAmount (float newValue)
	{
		currentLevel = (int)newValue;
	}

	public void increaseTotalExperiencePointsAmount (float extraValue)
	{
		totalExperiencePoints += (int)extraValue;
	}

	public void increaseTotalExperienceAmount (float extraValue)
	{
		totalExperienceAmount += ((int)extraValue * (int)currentExperienceMultiplier);
	}

	public void initializeCurrentLevelExperienceToNextLevelAmount (float newValue)
	{
		currentLevelExperienceToNextLevel = (int)newValue;
	}

	public int getCurrentLevel ()
	{
		return currentLevel;
	}

	public int getSkillPointsAmount ()
	{
		return totalExperiencePoints;
	}

	public void useSkillPoints (int amountToUse)
	{
		totalExperiencePoints -= amountToUse;

		if (totalExperiencePoints < 0) {
			totalExperiencePoints = 0;
		}

		playerStatsManager.updateStatValue (totalExperiencePointsName, totalExperiencePoints);

		updateStatsInfo ();
	}

	public void getSkillPoints (int amountToUse)
	{
		totalExperiencePoints += amountToUse;
	
		playerStatsManager.updateStatValue (totalExperiencePointsName, totalExperiencePoints);
	}

	Coroutine experienceMultiplierCoroutine;

	public void setExperienceMultiplier (float experienceMultiplierAmount, float experienceMultiplierDuration)
	{
		stopSetExperienceMultiplier ();

		experienceMultiplierCoroutine = StartCoroutine (setExperienceMultiplierCoroutine (experienceMultiplierAmount, experienceMultiplierDuration));
	}

	public void stopSetExperienceMultiplier ()
	{
		if (experienceMultiplierCoroutine != null) {
			StopCoroutine (experienceMultiplierCoroutine);
		}
	}

	IEnumerator setExperienceMultiplierCoroutine (float experienceMultiplierAmount, float experienceMultiplierDuration)
	{
		currentExperienceMultiplier = experienceMultiplierAmount;

		experienceMultiplerEnabled = true;

		eventOnExperienceMultiplerEnabled.Invoke ();

		if (!experienceMultiplierTextPanel.activeSelf) {
			experienceMultiplierTextPanel.SetActive (true);
		}

		experienceMultiplerText.text = currentExperienceMultiplier.ToString ();

		yield return new WaitForSeconds (experienceMultiplierDuration);

		currentExperienceMultiplier = 1;

		experienceMultiplerEnabled = true;

		eventOnExperienceMultiplerDisabled.Invoke ();

		experienceMultiplierTextPanel.SetActive (false);
	}

	IEnumerator initializeComponent ()
	{
		yield return new WaitForSeconds (0.3f);

		startInitialized = true;
	}

	public void enableOrDisableStatsMenuPanel (bool state)
	{
		if (statsMenuPanel.activeSelf != state) {
			statsMenuPanel.SetActive (state);
		}
	}

	public void setExperienceSystemEnabledState (bool state)
	{
		experienceSystemEnabled = state;
	}

	[System.Serializable]
	public class levelInfo
	{
		public string Name;
		public int levelNumber;
		public int experienceRequiredToNextLevel;

		public int experiencePointsAmount;

		public bool levelUnlocked;

		public List<statInfo> statInfoList = new List<statInfo> ();

		public UnityEvent eventsOnLevelReached;

		public bool callEventOnLevelReachedOnLoad;
	}

	[System.Serializable]
	public class statInfo
	{
		public string Name;

		public bool statIsAmount = true;

		public float statExtraValue;

		public bool useRandomRange;
		public Vector2 randomRange;

		public bool newBoolState;

		public bool unlockSkill;
		public string skillNameToUnlock;
	}

	[System.Serializable]
	public class experienceObtainedInfo
	{
		public RectTransform experienceObtainedRectTransform;

		public Text experienceAmountText;

		public int experienceAmount;

		public Coroutine moveExperienceTextPanel;
	}

	[System.Serializable]
	public class statUIInfo
	{
		public string Name;

		public Text statAmountText;

		public string extraTextAtStart;
		public string extraTextAtEnd;

		public bool statIsAmount = true;
	}
}
