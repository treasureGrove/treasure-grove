using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class characterAspectCustomizationUISystem : ingameMenuPanel
{
	[Space]
	[Header ("Main Settings")]
	[Space]

	public bool setNewAnimatorIdle;

	public int animatorIdle;

	public bool setUnscaledTimeOnAnimator;

	[Space]
	[Header ("Template Settings")]
	[Space]

	public characterAspectCustomizationTemplateData mainCharacterAspectCustomizationTemplateData;

	[Space]
	[Header ("Customization Settings")]
	[Space]

	public List<characterCustomizationTypeButtonInfo> characterCustomizationTypeButtonInfoList = new List<characterCustomizationTypeButtonInfo> ();

	[Space]
	[Header ("UI Panel Settings")]
	[Space]

	public List<panelCategoryInfo> panelCategoryInfoList = new List<panelCategoryInfo> ();

	[Space]
	[Header ("Rotate Character Settings")]
	[Space]

	public bool rotateCharacterEnabled = true;

	public float characterRotationSpeed = 10;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool characterCustomizationManagerAssigned;

	public bool menuOpened;

	public bool keepAllCharacterMeshesDisabledActive;

	[Space]
	[Header ("Components")]
	[Space]

	public characterCustomizationManager currentCharacterCustomizationManager;

	public mouseCursorController mainMouseCursorController;

	public Transform mainCameraTransform;

	public Transform mainCharacterTransform;

	playerController mainPlayerController;

	headTrack mainHeadTrack;

	Transform originalCameraParent;

	bool componentsAssigned;

	int previousIdleID = -1;

	GameObject currentButtonObjectPressed;

	Coroutine menuCoroutine;

	bool rotationInputActive;

	Vector2 axisValues;

	bool mainMouseCursorControllerAssigned;

	void Start ()
	{
		if (!characterCustomizationManagerAssigned) {
			if (currentCharacterCustomizationManager != null) {
				characterCustomizationManagerAssigned = true;
			}
		}
	}


	public void stopMenuCoroutineUpdate ()
	{
		if (menuCoroutine != null) {
			StopCoroutine (menuCoroutine);
		}
	}

	public void inputSetRotationInputActive (bool state)
	{
		if (menuOpened) {
			rotationInputActive = state;
		}
	}

	public void inputToggleRotationInputActive ()
	{
		inputSetRotationInputActive (!rotationInputActive);
	}

	IEnumerator menuCoroutineUpdate ()
	{
		var waitTime = new WaitForSecondsRealtime (0.0001f);

		while (true) {
			yield return waitTime;

			if (rotateCharacterEnabled) {
				if (rotationInputActive) {
					if (mainMouseCursorControllerAssigned) {
						axisValues = mainMouseCursorController.getMouseAxis ();
					}

					mainCharacterTransform.Rotate (mainCharacterTransform.up, -Mathf.Deg2Rad * characterRotationSpeed * axisValues.x, Space.World);
				}
			}
		}
	}

	public void checkButtonPressedWithoutCheckIfPreviouslyPressed (GameObject buttonObject)
	{
		checkButton (buttonObject);
	}

	public void checkButtonPressed (GameObject buttonObject)
	{
		if (showDebugPrint) {
			print (buttonObject.name);
		}

		if (currentButtonObjectPressed != null && buttonObject == currentButtonObjectPressed) {
			return;
		}

		checkButton (buttonObject);
	}

	void checkButton (GameObject buttonObject)
	{
		currentButtonObjectPressed = buttonObject;

		for (int i = 0; i < characterCustomizationTypeButtonInfoList.Count; i++) {
			for (int j = 0; j < characterCustomizationTypeButtonInfoList [i].characterCustomizationButtonInfoList.Count; j++) {

				characterCustomizationButtonInfo currentInfo = characterCustomizationTypeButtonInfoList [i].characterCustomizationButtonInfoList [j];

				if (currentInfo.buttonObject == buttonObject) {
					
					if (currentInfo.canBePressedMultipleTimes) {
						currentButtonObjectPressed = null;
					}

					bool isToggle = currentInfo.isToggle;

					bool disableOtherObjects = currentInfo.disableOtherObjects;

					string categoryOfObjectsToDisable = currentInfo.categoryOfObjectsToDisable;

					string optionName = currentInfo.Name;

					bool boolValue = false;

					if (isToggle) {
						Toggle currentToggle = buttonObject.GetComponent<Toggle> ();

						if (currentToggle != null) {
							boolValue = currentToggle.isOn;
						}

						optionName = currentInfo.toggleName;
					}

					bool setObjectActiveState = currentInfo.setObjectActiveState;

					if (setObjectActiveState) {
						boolValue = currentInfo.setObjectActive;
					}

					float floatValue = 0;

					string sliderName = currentInfo.sliderName;

					bool isSlider = currentInfo.isSlider;

					float sliderMaxValue = 0;

					if (isSlider) {
						Slider currenSlider = buttonObject.GetComponent<Slider> ();

						floatValue = currenSlider.value;

						sliderMaxValue = currenSlider.maxValue;
					}

					bool useRandomFloatValue = currentInfo.useRandomFloatValue;

					if (showDebugPrint) {
						print (optionName + " " + floatValue + " " + boolValue + " " + sliderName + " " + isSlider + " " + useRandomFloatValue);
					}

					bool isFullSet = currentInfo.isFullSet;

					setCustomizationOnCharacterFromOption (optionName, floatValue, boolValue, setObjectActiveState, 
						sliderName, isSlider, 
						useRandomFloatValue, sliderMaxValue, isToggle, 
						disableOtherObjects, categoryOfObjectsToDisable, isFullSet);

					return;
				}
			}
		}
	}

	void setCustomizationOnCharacterFromOption (string optionName, float floatValue, bool boolValue, bool setObjectActiveState, 
	                                            string sliderName, bool isSlider,
	                                            bool useRandomFloatValue, float sliderMaxValue, bool isToggle, 
	                                            bool disableOtherObjects, string categoryOfObjectsToDisable, bool isFullSet)
	{
		if (!characterCustomizationManagerAssigned) {
			return;
		}

		if (isSlider) {
			if (showDebugPrint) {
				print ("sending blendshape value " + sliderName + " " + floatValue + " " + useRandomFloatValue + "  " + sliderMaxValue);
			}

			currentCharacterCustomizationManager.setBlendShapeValue (sliderName, floatValue, useRandomFloatValue, sliderMaxValue);
		} else if (isToggle) {
			if (showDebugPrint) {
				print ("sending toggle value " + optionName + " " + boolValue);
			}

			currentCharacterCustomizationManager.setObjectState (boolValue, optionName, disableOtherObjects, categoryOfObjectsToDisable, false);
		} else if (setObjectActiveState) {
			if (showDebugPrint) {
				print ("sending set object active state " + optionName + " " + boolValue);
			}

			currentCharacterCustomizationManager.setObjectState (boolValue, optionName, disableOtherObjects, categoryOfObjectsToDisable, false);
		} else {
			if (showDebugPrint) {
				print ("sending full aspect  " + optionName);
			}

			int mainCharacterAspectCustomizationTemplateDataCount = mainCharacterAspectCustomizationTemplateData.characterAspectCustomizationTemplateList.Count;

			for (int i = 0; i < mainCharacterAspectCustomizationTemplateDataCount; i++) {
				characterAspectCustomizationTemplate currentTemplate = mainCharacterAspectCustomizationTemplateData.characterAspectCustomizationTemplateList [i];

				if (currentTemplate.Name.Equals (optionName)) {

					if (showDebugPrint) {
						print ("aspect found  " + optionName);
					}

					currentCharacterCustomizationManager.setCustomizationFromTemplate (currentTemplate, isFullSet);

					return;
				}
			}
		}
	}

	public void openPanelInfo (GameObject buttonObject)
	{
		int panelCategoryIndex = -1;
		int panelIndex = -1;

		for (int i = 0; i < panelCategoryInfoList.Count; i++) {
			for (int j = 0; j < panelCategoryInfoList [i].panelInfoList.Count; j++) {
				if (panelCategoryInfoList [i].panelInfoList [j].panelButton == buttonObject) {

					if (panelCategoryInfoList [i].panelInfoList [j].isCurrentPanel) {
						return;
					}

					panelIndex = j;

					panelCategoryIndex = i;
				}
			}
		}

		if (panelIndex == -1 || panelCategoryIndex == -1) {
			return;
		}

		closeAllPanelInfo ();

		panelInfo currentPanelInfo = panelCategoryInfoList [panelCategoryIndex].panelInfoList [panelIndex];


		if (currentPanelInfo.panelObject.activeSelf != true) {
			currentPanelInfo.panelObject.SetActive (true);
		}

		if (currentPanelInfo.useEventsOnSelectPanel) {
			currentPanelInfo.eventOnSelectPanel.Invoke ();
		}

		if (currentPanelInfo.setCameraPositionOnPanel) {		
			if (mainCharacterTransform != null) {	
				mainCameraTransform.SetParent (mainCharacterTransform);
			}

			mainCameraTransform.localEulerAngles = currentPanelInfo.cameraEulerOffset;

			mainCameraTransform.localPosition = currentPanelInfo.cameraPositionOffset;

			if (showDebugPrint) {
				print (currentPanelInfo.cameraPositionOffset + " " + currentPanelInfo.cameraEulerOffset);
			}

			mainCameraTransform.SetParent (originalCameraParent);
		}

		setSliderValuesFromBlendShapes ();

		currentPanelInfo.isCurrentPanel = true;

		return;
	}

	void setSliderValuesFromBlendShapes ()
	{
		if (!characterCustomizationManagerAssigned) {
			return;
		}

		for (int i = 0; i < characterCustomizationTypeButtonInfoList.Count; i++) {
			for (int j = 0; j < characterCustomizationTypeButtonInfoList [i].characterCustomizationButtonInfoList.Count; j++) {
				characterCustomizationButtonInfo currentInfo = characterCustomizationTypeButtonInfoList [i].characterCustomizationButtonInfoList [j];

				if (currentInfo.isSlider && currentInfo.setInitialValueFromBlendshape) {
					Slider currenSlider = currentInfo.buttonObject.GetComponent<Slider> ();

					float newValue = currentCharacterCustomizationManager.getBlendShapeValue (currentInfo.sliderName, currenSlider.maxValue);

					if (newValue > -1) {
						currenSlider.value = newValue;
					}
				}
			}
		}
	}

	public void closeAllPanelInfo ()
	{
		for (int i = 0; i < panelCategoryInfoList.Count; i++) {
			for (int j = 0; j < panelCategoryInfoList [i].panelInfoList.Count; j++) {
				if (panelCategoryInfoList [i].panelInfoList [j].panelObject.activeSelf != false) {
					panelCategoryInfoList [i].panelInfoList [j].panelObject.SetActive (false);
				}

				panelCategoryInfoList [i].panelInfoList [j].isCurrentPanel = false;
			}
		}
	}

	public override void initializeMenuPanel ()
	{
		if (currentCharacterCustomizationManager == null) {
			currentCharacterCustomizationManager = pauseManager.getPlayerControllerGameObject ().GetComponentInChildren<characterCustomizationManager> ();
		
			if (currentCharacterCustomizationManager != null) {
				characterCustomizationManagerAssigned = true;
			}

			checkCharacterComponents ();
		}
	}

	void checkCharacterComponents ()
	{
		if (!componentsAssigned) {
			if (pauseManager != null) {
				playerComponentsManager currentPlayerComponentsManager = pauseManager.getPlayerControllerGameObject ().GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
	
					mainPlayerController = currentPlayerComponentsManager.getPlayerController ();

					mainHeadTrack = currentPlayerComponentsManager.getHeadTrack ();
				}

				if (mainCameraTransform == null) {
					mainCameraTransform = pauseManager.getMainCameraTransform ();

					originalCameraParent = mainCameraTransform.parent;
				}

				if (mainCharacterTransform == null) {
					if (characterCustomizationManagerAssigned) {
						mainCharacterTransform = currentCharacterCustomizationManager.transform;
					} else {
						mainCharacterTransform = pauseManager.getPlayerControllerGameObject ().transform;
					}
				}
			}

			componentsAssigned = true;
		}
	}

	public void enableOrDisableHeadTemporaly (bool state)
	{
		if (currentCharacterCustomizationManager != null) {
			currentCharacterCustomizationManager.enableOrDisableHeadTemporaly (state);
		}
	}

	public override void openOrCloseMenuPanel (bool state)
	{
		base.openOrCloseMenuPanel (state);

		menuOpened = state;

		checkCharacterComponents ();

		stopMenuCoroutineUpdate ();

		if (!state) {
			mainCameraTransform.localPosition = Vector3.zero;
			mainCameraTransform.localEulerAngles = Vector3.zero;
		}

		if (currentCharacterCustomizationManager != null) {
			currentCharacterCustomizationManager.setEditActiveState (state);
		}

		if (state) {
			for (int i = 0; i < panelCategoryInfoList.Count; i++) {
				for (int j = 0; j < panelCategoryInfoList [i].panelInfoList.Count; j++) {
					bool panelButtonEnabled = panelCategoryInfoList [i].panelInfoList [j].panelButtonEnabled;

					if (showDebugPrint) {
						print (panelButtonEnabled + "  " + panelCategoryInfoList [i].panelInfoList [j].Name);
					}

					if (panelCategoryInfoList [i].panelInfoList [j].panelButton.activeSelf != panelButtonEnabled) {
						panelCategoryInfoList [i].panelInfoList [j].panelButton.SetActive (panelButtonEnabled);
					}

					panelCategoryInfoList [i].panelInfoList [j].isCurrentPanel = false;
				}
			}

		} else {
			closeAllPanelInfo ();

			currentButtonObjectPressed = null;

			enableOrDisableHeadTemporaly (false);
		}
			
		if (state) {
			if (setNewAnimatorIdle) {
				if (previousIdleID == -1) {
					previousIdleID = mainPlayerController.getCurrentIdleID ();
				}
			
				mainPlayerController.setCurrentIdleIDValue (animatorIdle);

				mainPlayerController.updateIdleIDOnAnimator ();
			}

		} else {
			if (previousIdleID != -1) {
				mainPlayerController.setCurrentIdleIDValue (previousIdleID);

				mainPlayerController.updateIdleIDOnAnimator ();

				previousIdleID = -1;
			}
		}

		if (setUnscaledTimeOnAnimator) {
			mainPlayerController.setAnimatorUnscaledTimeState (state);
		}
			
		if (mainHeadTrack != null) {
			if (state) {
				mainHeadTrack.setExternalHeadTrackPauseActiveState (true);
			} else {
				mainHeadTrack.setExternalHeadTrackPauseActiveState (false);
			}
		}

		if (state) {
			menuCoroutine = StartCoroutine (menuCoroutineUpdate ());
		}

		rotationInputActive = false;

		axisValues = Vector3.zero;

		if (!mainMouseCursorControllerAssigned) {
			mainMouseCursorController = FindObjectOfType<mouseCursorController> ();

			mainMouseCursorControllerAssigned = mainMouseCursorController != null;
		}

		if (mainPlayerController != null) {
			if (mainPlayerController.isPlayerOnFirstPerson ()) {
				if (state) {
					previousKeepAllCharacterMeshesDisabledActiveValue = keepAllCharacterMeshesDisabledActive;

					if (previousKeepAllCharacterMeshesDisabledActiveValue) {
						checkCameraViewToFirstOrThirdPerson (false);

						mainPlayerController.setPausePlayerRotateToCameraDirectionOnFirstPersonActiveState (true);
					}
				} else {
					if (previousKeepAllCharacterMeshesDisabledActiveValue) {
						checkCameraViewToFirstOrThirdPerson (true);

						mainPlayerController.setPausePlayerRotateToCameraDirectionOnFirstPersonActiveState (false);

						previousKeepAllCharacterMeshesDisabledActiveValue = false;
					}
				}

				mainPlayerController.setAnimatorState (menuOpened);

				mainCharacterTransform.localRotation = Quaternion.identity;
			} else {
				previousKeepAllCharacterMeshesDisabledActiveValue = false;
			}
		}

		if (state) {
			for (int i = 0; i < panelCategoryInfoList.Count; i++) {
				for (int j = 0; j < panelCategoryInfoList [i].panelInfoList.Count; j++) {
					if (panelCategoryInfoList [i].panelInfoList [j].panelButtonEnabled) {
						openPanelInfo (panelCategoryInfoList [i].panelInfoList [j].panelButton);

						return;
					}
				}
			}
		}
	}

	bool previousKeepAllCharacterMeshesDisabledActiveValue;

	public bool equipOrUnequipObject (bool state, string objectName, string categoryName)
	{
		if (!characterCustomizationManagerAssigned) {
			initializeMenuPanel ();

			if (!characterCustomizationManagerAssigned) {
				return false;
			}
		}

		bool result = currentCharacterCustomizationManager.setObjectState (state, objectName, true, categoryName, true);

		if (!result) {
			return false;
		}
			
		currentCharacterCustomizationManager.checkEquippedStateOnObject (state, objectName, categoryName);


		return result;
	}

	public void checkCameraViewToFirstOrThirdPerson (bool state)
	{
		if (characterCustomizationManagerAssigned) {
			keepAllCharacterMeshesDisabledActive = state;

			currentCharacterCustomizationManager.checkCameraViewToFirstOrThirdPerson (state);
		}
	}

	public void checkCameraViewToFirstOrThirdPersonOnEditor (bool state)
	{
		if (currentCharacterCustomizationManager != null) {
			keepAllCharacterMeshesDisabledActive = state;

			currentCharacterCustomizationManager.checkCameraViewToFirstOrThirdPersonOnEditor (state);
		}
	}

	public bool checkIfObjectAlreadyOnCurrentPiecesList (string pieceName)
	{
		if (currentCharacterCustomizationManager != null) {
			return currentCharacterCustomizationManager.checkIfObjectAlreadyOnCurrentPiecesList (pieceName);
		}

		return false;
	}

	public string getArmorClothPieceByName (string categoryName)
	{
		if (currentCharacterCustomizationManager != null) {
			return currentCharacterCustomizationManager.getArmorClothPieceByName (categoryName);
		}

		return "";
	}

	public string getArmorClothCategoryByName (string categoryName)
	{
		if (currentCharacterCustomizationManager != null) {
			return currentCharacterCustomizationManager.getArmorClothCategoryByName (categoryName);
		}

		return "";
	}

	[System.Serializable]
	public class characterCustomizationButtonInfo
	{
		public string Name;

		public GameObject buttonObject;

		public bool canBePressedMultipleTimes;

		[Space]

		public bool setObjectActiveState;

		public bool setObjectActive;

		[Space]

		public bool isToggle;

		public string toggleName;

		[Space]

		public bool disableOtherObjects;

		public string categoryOfObjectsToDisable;

		[Space]

		public bool isSlider;

		public string sliderName;

		public bool useRandomFloatValue;

		public bool setInitialValueFromBlendshape;

		[Space]

		public bool isFullSet;
	}

	[System.Serializable]
	public class characterCustomizationTypeButtonInfo
	{
		public string Name;

		[Space]

		public List<characterCustomizationButtonInfo> characterCustomizationButtonInfoList = new List<characterCustomizationButtonInfo> ();
	}

	[System.Serializable]
	public class panelCategoryInfo
	{
		public string Name;

		[Space]

		public List<panelInfo> panelInfoList = new List<panelInfo> ();
	}

	[System.Serializable]
	public class panelInfo
	{
		public string Name;

		public bool panelButtonEnabled = true;

		[Space]
		[Space]

		public GameObject panelButton;

		public GameObject panelObject;

		[Space]
		[Space]

		public bool setCameraPositionOnPanel;

		public Vector3 cameraPositionOffset;
		public Vector3 cameraEulerOffset;

		[Space]
		[Space]

		public bool isCurrentPanel;

		[Space]
		[Space]

		public bool useEventsOnSelectPanel;
		public UnityEvent eventOnSelectPanel;
	}
}