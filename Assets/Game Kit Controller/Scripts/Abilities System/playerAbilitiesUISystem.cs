using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class playerAbilitiesUISystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool abilityListManagerEnabled;

	public int numberOfAbilitySlots = 8;

	public bool selectAbilityOnMenuClose;

	public bool useDpadPanel = true;

	public float timeToShowAbilitySlotsWheel = 1;

	public bool useBlurUIPanel = true;

	public bool slowDownTimeWhenMenuActive;
	public float timeScaleWhenMenuActive;

	public Vector2 abilityWheelMenuRotationrange = new Vector2 (5f, 3f);
	public float rotationHUDSpeed = 20;

	[Space]
	[Header ("Ability Slot Settings")]
	[Space]

	public bool changeCurrentSlotSize;
	public float changeCurrentSlotSpeed;
	public float changeCurrentSlotMultiplier;
	public float distanceFromCenterToSelectAbility = 10;

	[Space]
	[Header ("Slot Position And Size Settings")]
	[Space]

	public bool useSlotsChangeScale;
	public int numberOfSlotsToChangeScale;
	public float slotScale;
	public float minSlotScale = 0.3f;
	public float slotHeightOffset;

	public float wheelRadius = 880;

	[Space]
	[Header ("Wheel And Scroll Rect Settings")]
	[Space]

	public bool useWheelMenu = true;

	[Space]
	[Header ("Sound Settings")]
	[Space]

	public bool useSoundOnSlot;
	public AudioClip soundEffect;
	public AudioSource mainAudioSource;

	[Space]
	[Header ("Dpad Icon Info List Settings")]
	[Space]

	public List<dpadIconInfo> dpadIconInfoList = new List<dpadIconInfo> ();

	public bool useCurrentAbilityIcon;

	public dpadIconInfo currentAbilityIcon;

	public Text currentAbilityInputText;

	public string abilityInputName;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool selectingAbility;
	public int currentAbilityPressedIndex = -1;

	public bool pauseCheckUsingDeviceOnAbilityInput;

	public bool inputPaused;

	public List<abilitySlotInfo> abilitySlotInfoList = new List<abilitySlotInfo> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool usedByAI;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnAbilitiesAssignedOnDpad;
	public UnityEvent eventOnAbilitiesNotAssignedOnDpad;

	[Space]

	public UnityEvent eventOnOpenAbilitiesWheel;
	public UnityEvent eventOnCloseAbilitiesWheel;

	[Space]
	[Header ("UI Elements")]
	[Space]

	public GameObject abilitySlotsMenu;
	public GameObject wheelAbilityListElement;


	public GameObject dpadPanel;
	public Slider mainEnerySlider;
	public Slider abilitiesEnergySlider;

	public Transform abilitySlotsWheel;

	public Text currentAbilityNameText;
	public GameObject completeAbilityWheel;
	public RectTransform completeAbilityWheelRectTransform;
	public Transform slotArrow;
	public Transform slotArrowIcon;
	public RectTransform slotArrowRectTransform;

	public GameObject wheelBackground;

	[Space]

	public Transform slotsGridTransformParent;
	public GameObject slotsGridPanelGameObject;
	public GameObject gridAbilityListElement;

	public GameObject slotsGridBackground;

	[Space]
	[Header ("Components")]
	[Space]

	public menuPause pauseManager;
	public playerAbilitiesSystem mainPlayerAbilitiesSystem;
	public playerController playerControllerManager;
	public timeBullet timeBulletManager;

	public playerInputManager playerInput;

	abilitySlotInfo closestSlot;
	abilitySlotInfo previousClosestSlot;

	float screenWidth;
	float screenHeight;
	bool touchPlatform;
	bool touching;
	Vector2 mRot = Vector2.zero;
	Quaternion mStart;

	int numberOfCurrentAbilities;

	Touch currentTouch;

	float anglePerSlot;
	float currentAngle;
	float lastTimeTouching;
	bool isFirstAbilitySelected;

	Vector3 initialTouchPosition;

	float currentArrowAngle;

	Vector2 currentTouchPosition;

	float currentDistance;

	bool dpadDirectionsAssigned;

	bool touchPressHoldActive;

	bool fingerPressingTouchPanel;

	bool currentAbilityIconChecked;

	public enum dpadDirection
	{
		up,
		down,
		left,
		right
	}

	bool dpadInitialized;

	bool dpadEventsChecked;

	bool dpadEventsInitialized;

	void Start ()
	{
		if (!mainPlayerAbilitiesSystem.abilitiesSystemEnabled) {
			abilityListManagerEnabled = false;
		}

		if (!abilityListManagerEnabled) {
			enableOrDisableDpadPanel (false);

			return;
		}

		if (usedByAI) {
			return;
		}

		Transform slotsParent = abilitySlotsWheel;

		GameObject currentSlotPrefab = wheelAbilityListElement;

		if (useWheelMenu) {
			if (slotsGridPanelGameObject.activeSelf) {
				slotsGridPanelGameObject.SetActive (false);
			}

			if (!slotArrow.gameObject.activeSelf) {
				slotArrow.gameObject.SetActive (true);
			}

			if (!abilitySlotsWheel.gameObject.activeSelf) {
				abilitySlotsWheel.gameObject.SetActive (true);
			}

			if (!wheelBackground.gameObject.activeSelf) {
				wheelBackground.gameObject.SetActive (true);
			}

			if (slotsGridBackground.gameObject.activeSelf) {
				slotsGridBackground.gameObject.SetActive (false);
			}
		} else {
			slotsParent = slotsGridTransformParent;

			if (!slotsGridPanelGameObject.activeSelf) {
				slotsGridPanelGameObject.SetActive (true);
			}

			if (slotArrow.gameObject.activeSelf) {
				slotArrow.gameObject.SetActive (false);
			}

			if (abilitySlotsWheel.gameObject.activeSelf) {
				abilitySlotsWheel.gameObject.SetActive (false);
			}

			if (wheelBackground.gameObject.activeSelf) {
				wheelBackground.gameObject.SetActive (false);
			}

			if (!slotsGridBackground.gameObject.activeSelf) {
				slotsGridBackground.gameObject.SetActive (true);
			}

			currentSlotPrefab = gridAbilityListElement;
		}

		int abilityInfoListCount = mainPlayerAbilitiesSystem.abilityInfoList.Count;

		//for every ability created in the player abilities manager inspector, add to the list of the ability manager
		for (int i = 0; i < abilityInfoListCount; i++) {
			GameObject newAbilityListElement = (GameObject)Instantiate (currentSlotPrefab, Vector3.zero, Quaternion.identity, slotsParent);

			newAbilityListElement.name = "Ability Slot " + (i + 1);

			newAbilityListElement.transform.localScale = Vector3.one;
			newAbilityListElement.transform.position = currentSlotPrefab.transform.position;

			abilitySlotInfo newAbilitySlotInfo = newAbilityListElement.GetComponent<abilitySlotElement> ().slotInfo;

			abilityInfo currentAbilityInfo = mainPlayerAbilitiesSystem.abilityInfoList [i];

			newAbilitySlotInfo.Name = currentAbilityInfo.Name;

			newAbilitySlotInfo.abilityIcon.transform.rotation = Quaternion.identity;

			newAbilitySlotInfo.abilityIcon.texture = currentAbilityInfo.abilityTexture;

			newAbilitySlotInfo.abilityIndex = i;

			abilitySlotInfoList.Add (newAbilitySlotInfo);
		}

		currentSlotPrefab.SetActive (false);

		//get the rotation of the ability wheel
		mStart = completeAbilityWheel.transform.localRotation;

		//check if the platform is a touch device or not
		touchPlatform = touchJoystick.checkTouchPlatform ();

		if (!useDpadPanel) {
			enableOrDisableDpadPanel (false);
		}
	}

	public void enableOrDisableDpadPanel (bool state)
	{
		if (!useDpadPanel) {
			state = false;
		}

		if (dpadPanel != null) {
			if (dpadPanel.activeSelf != state) {
				dpadPanel.SetActive (state);
			}
		}
	}

	public void enableOrDisableExtraElementsOnAbilitiesUI (bool state)
	{
		if (useWheelMenu) {
			if (slotArrow.gameObject.activeSelf != state) {
				slotArrow.gameObject.SetActive (state);
			}
		} else {
			if (slotsGridBackground.activeSelf != state) {
				slotsGridBackground.SetActive (state);
			}
		}

		if (currentAbilityNameText.gameObject.activeSelf != state) {
			currentAbilityNameText.gameObject.SetActive (state);
		}
	}

	public bool isUseWheelMenuActive ()
	{
		return useWheelMenu;
	}

	void Update ()
	{
		if (usedByAI) {
			return;
		}

		if (!dpadInitialized) {
			setDpadIconsInfo ();

			dpadInitialized = true;
		}

		if (useDpadPanel) {
			if (!currentAbilityIconChecked) {
				if (useCurrentAbilityIcon && currentAbilityIcon != null) {

					int currentAbilityIndexToCheck = mainPlayerAbilitiesSystem.getCurrentAbilityIndex ();

					bool anyAbilityAssignedOnDpad = false;

					if (currentAbilityIndexToCheck > -1) {

						abilityInfo currentAbilityInfo = mainPlayerAbilitiesSystem.abilityInfoList [currentAbilityIndexToCheck];

						if (currentAbilityInfo.abilityVisibleOnWheelSelection && currentAbilityInfo.abilityCanBeShownOnWheelSelection) {
							setDpadIconInfo (currentAbilityIcon, currentAbilityIndexToCheck);

							updateCurrentAbilityInputText ();

							anyAbilityAssignedOnDpad = true;
						}
					}

					for (int i = 0; i < dpadIconInfoList.Count; i++) {

						dpadIconInfo currentDpadIconInfo = dpadIconInfoList [i];

						if (currentDpadIconInfo.dpadAbilityIndex > -1) {
							anyAbilityAssignedOnDpad = true;
						}
					}

					checkEventsOnAbilitiesAssignedOnDpad (anyAbilityAssignedOnDpad);

					if (!anyAbilityAssignedOnDpad) {
						mainPlayerAbilitiesSystem.removeCurrentAbilityInfo ();
					}
				}

				currentAbilityIconChecked = true;
			}
		}

		//if the player is selecting or the touch controls are enabled, then
		if ((selectingAbility || pauseManager.isUsingTouchControls ()) && !pauseManager.isGamePaused ()) {
			//check the mouse position in the screen if we are in the editor, or the finger position in a touch device
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

				currentTouchPosition = currentTouch.position;

				if (fingerPressingTouchPanel && !touching) {
					touching = true;

					initialTouchPosition = currentTouchPosition;

					lastTimeTouching = Time.time;
				}

				if (touching && (currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved)) {
					if (!selectingAbility) {
						if (Time.time > lastTimeTouching + timeToShowAbilitySlotsWheel) {
							selectAbilitySlots (true);
						}
					}
				}

				if (currentTouch.phase == TouchPhase.Ended) {
					if (touching && selectingAbility) {
						selectAbilitySlots (false);
					}

					touching = false;

					fingerPressingTouchPanel = false;

					return;
				}

				if (selectingAbility) {
					if (useWheelMenu) {
						//get the current screen size
						screenWidth = Screen.width;
						screenHeight = Screen.height;

						//get the arrow rotates toward the mouse, selecting the closest ability to it
						Vector2 slotDirection = new Vector2 (currentTouchPosition.x, currentTouchPosition.y) - slotArrowRectTransform.anchoredPosition;
						Vector2 screenCenter = new Vector2 (screenWidth, screenHeight) / 2;

						slotDirection -= screenCenter;

						currentArrowAngle = Mathf.Atan2 (slotDirection.y, slotDirection.x);
						currentArrowAngle -= 90 * Mathf.Deg2Rad;

						slotArrow.localRotation = Quaternion.Euler (0, 0, currentArrowAngle * Mathf.Rad2Deg);

						if (GKC_Utils.distance (initialTouchPosition, currentTouchPosition) > distanceFromCenterToSelectAbility) {
							if (!slotArrow.gameObject.activeSelf) {
								slotArrow.gameObject.SetActive (true);
							}

							//make the slots wheel looks toward the mouse
							float halfWidth = screenWidth * 0.5f;
							float halfHeight = screenHeight * 0.5f;

							float x = Mathf.Clamp ((currentTouchPosition.x - halfWidth) / halfWidth, -1f, 1f);
							float y = Mathf.Clamp ((currentTouchPosition.y - halfHeight) / halfHeight, -1f, 1f);

							mRot = Vector2.Lerp (mRot, new Vector2 (x, y), Time.deltaTime * rotationHUDSpeed);

							completeAbilityWheel.transform.localRotation = mStart * Quaternion.Euler (mRot.y * abilityWheelMenuRotationrange.y, -mRot.x * abilityWheelMenuRotationrange.x, 0f);

							//get the ability inside the wheel closest to the mouse
							float distance = Mathf.Infinity;

							for (int k = 0; k < abilitySlotInfoList.Count; k++) {
								if (abilitySlotInfoList [k].slotActive) {
									currentDistance = GKC_Utils.distance (slotArrowIcon.position, abilitySlotInfoList [k].abilityIcon.transform.position);

									if (currentDistance < distance) {
										distance = currentDistance;
										closestSlot = abilitySlotInfoList [k];
									}
								}
							}

							if (previousClosestSlot != closestSlot) {
								if (changeCurrentSlotSize) {
									if (previousClosestSlot != null) {
										changeSlotSize (previousClosestSlot, true, changeCurrentSlotMultiplier, true);
									}
								}

								previousClosestSlot = closestSlot;

								if (changeCurrentSlotSize) {
									changeSlotSize (closestSlot, false, changeCurrentSlotMultiplier, true);
								}

								if (closestSlot.Name != "") {
									currentAbilityNameText.text = closestSlot.Name;
								}

								if (useSoundOnSlot) {
									playSound (soundEffect);
								}
							}

							//set the name of the closest ability in the center of the ability wheel
							if (mainPlayerAbilitiesSystem.getCurrentAbilityIndex () != closestSlot.abilityIndex || !isFirstAbilitySelected) {
								isFirstAbilitySelected = true;

								if (!selectAbilityOnMenuClose) {
									selectAbility (closestSlot);
								}
							}
						} else {
							if (slotArrow.gameObject.activeSelf) {
								slotArrow.gameObject.SetActive (false);
							}
						}

						touchPressHoldActive = false;
					} else {

						if (GKC_Utils.distance (initialTouchPosition, currentTouchPosition) > distanceFromCenterToSelectAbility) {

							float distance = Mathf.Infinity;

							for (int k = 0; k < abilitySlotInfoList.Count; k++) {
								if (abilitySlotInfoList [k].slotActive) {
									Vector2 slotPosition = abilitySlotInfoList [k].slotRectTransform.position;

									currentDistance = GKC_Utils.distance (currentTouchPosition, slotPosition);

									if (currentDistance < distance) {
										distance = currentDistance;
										closestSlot = abilitySlotInfoList [k];
									}
								}
							}

							if (previousClosestSlot != closestSlot) {
								if (changeCurrentSlotSize) {
									if (previousClosestSlot != null) {
										changeSlotSize (previousClosestSlot, true, changeCurrentSlotMultiplier, true);
									}
								}

								previousClosestSlot = closestSlot;

								if (changeCurrentSlotSize) {
									changeSlotSize (closestSlot, false, changeCurrentSlotMultiplier, true);
								}

								if (closestSlot.Name != "") {
									currentAbilityNameText.text = closestSlot.Name;
								}

								if (useSoundOnSlot) {
									playSound (soundEffect);
								}
							}

							//set the name of the closest ability in the center of the ability wheel
							if (mainPlayerAbilitiesSystem.getCurrentAbilityIndex () != closestSlot.abilityIndex || !isFirstAbilitySelected) {
								isFirstAbilitySelected = true;

								if (!selectAbilityOnMenuClose) {
									selectAbility (closestSlot);
								}
							}
						} 

						touchPressHoldActive = false;
					}
				} else {
					if (touchPressHoldActive) {
						inputUseDpadDirectionPressHold ();
					}
				}
			}
		}
	}

	public void updateCurrentAbilityInputText ()
	{
		if (useCurrentAbilityIcon) {
			currentAbilityInputText.text = playerInput.getButtonKey (abilityInputName);
		}
	}

	public void setTouchingMenuPanelState (bool state)
	{
		fingerPressingTouchPanel = state;

		pauseManager.setIgnoreDisableTouchZoneListState (fingerPressingTouchPanel);
	}

	public void selectAbilityByName (string abilityName)
	{
		for (int k = 0; k < abilitySlotInfoList.Count; k++) {
			if (abilitySlotInfoList [k].Name.Equals (abilityName)) {
				selectAbility (abilitySlotInfoList [k]);

				return;
			}
		}
	}

	public void selectAbility (abilitySlotInfo slotInfo)
	{
		mainPlayerAbilitiesSystem.setCurrentAbilityByName (slotInfo.Name);

		for (int k = 0; k < abilitySlotInfoList.Count; k++) {
			if (slotInfo.abilityIndex == abilitySlotInfoList [k].abilityIndex) {
				if (!abilitySlotInfoList [k].currentAbilitySelectedIcon.activeSelf) {
					abilitySlotInfoList [k].currentAbilitySelectedIcon.SetActive (true);
				}
			} else {
				if (abilitySlotInfoList [k].currentAbilitySelectedIcon.activeSelf) {
					abilitySlotInfoList [k].currentAbilitySelectedIcon.SetActive (false);
				}
			}
		}

		if (useCurrentAbilityIcon) {
			setDpadIconInfo (currentAbilityIcon, slotInfo.abilityIndex);

			updateCurrentAbilityInputText ();

			checkEventsOnAbilitiesAssignedOnDpad (true);
		}
	}

	public void changeSlotSize (abilitySlotInfo slotInfo, bool setRegularSize, float sizeMultiplier, bool smoothSizeChange)
	{
		if (slotInfo.sizeCoroutine != null) {
			StopCoroutine (slotInfo.sizeCoroutine);
		}

		slotInfo.sizeCoroutine = StartCoroutine (changeSlotSizeCoroutine (slotInfo, setRegularSize, sizeMultiplier, smoothSizeChange));
	}

	IEnumerator changeSlotSizeCoroutine (abilitySlotInfo slotInfo, bool setRegularSize, float sizeMultiplier, bool smoothSizeChange)
	{
		Vector3 targetValue = Vector3.one;

		if (!setRegularSize) {
			targetValue *= sizeMultiplier;
		}

		if (smoothSizeChange) {
			while (slotInfo.innerSlot.localScale != targetValue) {
				slotInfo.innerSlot.localScale = Vector3.MoveTowards (slotInfo.innerSlot.localScale, targetValue, Time.deltaTime * changeCurrentSlotMultiplier);

				yield return null;
			}
		} else {
			slotInfo.innerSlot.localScale = targetValue;
		}
	}

	//enable the ability wheel to select the current ability to use
	public void selectAbilitySlots (bool state)
	{
		if (currentAbilityPressedIndex != -1) {
			return;
		}

		//check that the game is not paused, that the player is not editing the ability, using a device and that the ability manager can be enabled
		if ((canBeOpened () || selectingAbility)) {

			if (!mainPlayerAbilitiesSystem.checkIfAbilitiesAvailable ()) {
				return;
			}

			if (mainPlayerAbilitiesSystem.isAbilityInputInUse ()) {
				return;
			}

			selectingAbility = state;

			mainPlayerAbilitiesSystem.setSelectingAbilityActiveState (selectingAbility);

			if (!selectingAbility) {
				if (changeCurrentSlotSize && closestSlot != null) {
					changeSlotSize (closestSlot, true, changeCurrentSlotMultiplier, false);
				}
			}

			pauseManager.openOrClosePlayerMenu (selectingAbility, abilitySlotsMenu.transform, useBlurUIPanel);

			//enable the ability wheel
			completeAbilityWheel.SetActive (selectingAbility);

			pauseManager.setIngameMenuOpenedState ("Ability List Manager", selectingAbility, true);

			pauseManager.enableOrDisablePlayerMenu (selectingAbility, false, false);

			//reset the arrow and the wheel rotation
			completeAbilityWheel.transform.localRotation = Quaternion.identity;

			slotArrow.localRotation = Quaternion.identity;

			if (selectingAbility) {

				updateSlotsInfo ();

				if (!touchPlatform) {
					initialTouchPosition = touchJoystick.convertMouseIntoFinger ().position;
				} else {
					initialTouchPosition = Input.GetTouch (1).position;
				}

				if (slowDownTimeWhenMenuActive) {
					timeBulletManager.setBulletTimeState (true, timeScaleWhenMenuActive);
				}

				completeAbilityWheelRectTransform.anchoredPosition = Vector3.zero;
			} else {
				isFirstAbilitySelected = false;

				if (slowDownTimeWhenMenuActive) {
					timeBulletManager.setBulletTimeState (false, 1);
				}

				if (selectAbilityOnMenuClose && closestSlot != null) {
					selectAbility (closestSlot);
				}
			}

			if (state) {
				eventOnOpenAbilitiesWheel.Invoke ();
			} else {
				eventOnCloseAbilitiesWheel.Invoke ();
			}

			closestSlot = null;
			previousClosestSlot = null;
		}
	}

	public void setPauseCheckUsingDeviceOnAbilityInputState (bool state)
	{
		pauseCheckUsingDeviceOnAbilityInput = state;
	}

	public void togglePauseCheckUsingDeviceOnAbilityInputState ()
	{
		setPauseCheckUsingDeviceOnAbilityInputState (!pauseCheckUsingDeviceOnAbilityInput);
	}

	public bool canBeOpened ()
	{
		if (pauseManager.isGamePaused ()) {
			return false;
		}

		if (!pauseCheckUsingDeviceOnAbilityInput && playerControllerManager.isUsingDevice ()) {
			return false;
		}

		if (!abilityListManagerEnabled) {
			return false;
		}

		if (!mainPlayerAbilitiesSystem.isAbilitesModeActive ()) {
			return false;
		}

		if (playerControllerManager.isPlayerMenuActive ()) {
			return false;
		}

		if (mainPlayerAbilitiesSystem.disableAbilitySystemOnFirstPerson && playerControllerManager.isPlayerOnFirstPerson ()) {
			return false;
		}

		return true;
	}

	public void udpateSlotsInfo ()
	{
		updateSlotsInfo ();
	}

	public void updateSlotsInfo ()
	{
		numberOfCurrentAbilities = mainPlayerAbilitiesSystem.getNumberOfAbilitiesCanBeShownOnWheelSelection ();

		anglePerSlot = 360 / (float)numberOfCurrentAbilities;
		currentAngle = 0;

		for (int j = 0; j < abilitySlotInfoList.Count; j++) {
			abilitySlotInfoList [j].slotActive = false;

			if (abilitySlotInfoList [j].slot.activeSelf) {
				abilitySlotInfoList [j].slot.SetActive (false);
			}
		}

		bool anySlotAviable = false;

		int numberSlotsActive = 0;

		int abilityInfoListCount = mainPlayerAbilitiesSystem.abilityInfoList.Count;

		for (int i = 0; i < abilityInfoListCount; i++) {

			abilityInfo currentAbilityInfo = mainPlayerAbilitiesSystem.abilityInfoList [i];

			if (currentAbilityInfo.abilityEnabled && currentAbilityInfo.addAbilityToUIWheelActive && i <= numberOfAbilitySlots) {

				abilitySlotInfo newAbilitySlotInfo = abilitySlotInfoList [i];

				if (!newAbilitySlotInfo.slotActive &&
				    currentAbilityInfo.abilityVisibleOnWheelSelection &&
				    currentAbilityInfo.abilityCanBeShownOnWheelSelection) {

					if (useWheelMenu) {
						newAbilitySlotInfo.slotRectTransform.rotation = Quaternion.Euler (new Vector3 (0, 0, currentAngle));

						currentAngle += anglePerSlot;	
					}

					newAbilitySlotInfo.abilityIcon.transform.rotation = Quaternion.identity;

					if (mainPlayerAbilitiesSystem.getCurrentAbilityIndex () == i) {
						if (!newAbilitySlotInfo.currentAbilitySelectedIcon.activeSelf) {
							newAbilitySlotInfo.currentAbilitySelectedIcon.SetActive (true);
						}

						currentAbilityNameText.text = newAbilitySlotInfo.Name;
					} else {
						if (newAbilitySlotInfo.currentAbilitySelectedIcon.activeSelf) {
							newAbilitySlotInfo.currentAbilitySelectedIcon.SetActive (false);
						}
					}

					newAbilitySlotInfo.slotActive = true;

					if (!newAbilitySlotInfo.slot.activeSelf) {
						newAbilitySlotInfo.slot.SetActive (true);
					}

					anySlotAviable = true;

					numberSlotsActive++;
				}
			}
		}

		if (useWheelMenu) {
			if (useSlotsChangeScale) {
				Vector3 newScale = Vector3.one;

				Vector2 newAnchoredPosition = Vector2.zero;

				if (numberSlotsActive >= numberOfSlotsToChangeScale) {
					newScale = Vector3.one - (Vector3.one * (numberSlotsActive / slotScale));

					if (newScale.x < minSlotScale) {
						newScale = minSlotScale * Vector3.one;
					}

					float slotHeight = (wheelRadius / slotHeightOffset) * numberSlotsActive;

					slotHeight = Mathf.Clamp (slotHeight, 0, wheelRadius);

					newAnchoredPosition = new Vector2 (0, slotHeight); 
				}

				for (int j = 0; j < abilitySlotInfoList.Count; j++) {
					abilitySlotInfoList [j].slotCenter.transform.localScale = newScale;
					abilitySlotInfoList [j].slotCenter.anchoredPosition = newAnchoredPosition;
				}
			}
		} else {

		}

		if (!anySlotAviable) {
			selectAbilitySlots (false);
		}

		updateDpadIconsOnWheelMenu ();
	}

	public void checkIfAssignActivatedAbilitiesToFreeDpadSlots (string currentAbilityName, int abilityIndexToCheck, bool addingAbilityToWheel)
	{
		bool abilityAssignedOnDpad = false;

		for (int i = 0; i < dpadIconInfoList.Count; i++) {

			dpadIconInfo currentDpadIconInfo = dpadIconInfoList [i];

			if (currentDpadIconInfo.dpadAbilityIndex == abilityIndexToCheck) {
				abilityAssignedOnDpad = true;
			}
		}

		if (abilityAssignedOnDpad) {
			if (addingAbilityToWheel) {
				if (showDebugPrint) {
					print ("ability already on dpad, not doing anything");
				}
			} else {
				if (showDebugPrint) {
					print ("ability already on dpad, removing from it");
				}

				int dpadIndex = getDpadIconInfoListIndexByAbilityIndex (abilityIndexToCheck);

				if (dpadIndex > -1) {
					setDpadIconInfo (dpadIconInfoList [dpadIndex], -1);
				}
			}
		} else {
			if (addingAbilityToWheel) {
				if (showDebugPrint) {
					print ("ability not on dpad, adding to it");
				}

				bool dpadFound = false;

				for (int i = 0; i < dpadIconInfoList.Count; i++) {
					if (!dpadFound) {

						dpadIconInfo currentDpadIconInfo = dpadIconInfoList [i];

						if (currentDpadIconInfo.dpadAbilityIndex == -1) {
							setDpadIconInfo (currentDpadIconInfo, abilityIndexToCheck);

							dpadFound = true;
						}
					}
				}
			} else {
				if (showDebugPrint) {
					print ("ability not on dpad, not doing anything");
				}
			}
		}

		bool anyAbilityAssignedOnDpad = false;

		for (int i = 0; i < dpadIconInfoList.Count; i++) {

			dpadIconInfo currentDpadIconInfo = dpadIconInfoList [i];

			if (currentDpadIconInfo.dpadAbilityIndex > -1) {
				anyAbilityAssignedOnDpad = true;
			}
		}

		checkEventsOnAbilitiesAssignedOnDpad (anyAbilityAssignedOnDpad);

		if (showDebugPrint) {
			print (anyAbilityAssignedOnDpad + " " + abilityAssignedOnDpad + " " + addingAbilityToWheel + " " + abilityIndexToCheck + " " + currentAbilityName);
		}

		if (anyAbilityAssignedOnDpad) {

			if (!abilityAssignedOnDpad && addingAbilityToWheel) {
				if (currentAbilityIcon.dpadAbilityIndex == -1) {
					if (showDebugPrint) {
						print ("assigning ability on main dpad icon");
					}

					setDpadIconInfo (currentAbilityIcon, abilityIndexToCheck);

					if (mainPlayerAbilitiesSystem.currentAbilityInfo == null) {
						mainPlayerAbilitiesSystem.setCurrentAbilityByName (currentAbilityName);
					}
				}
			}

			if (abilityAssignedOnDpad && !addingAbilityToWheel) {
				if (showDebugPrint) {
					print ("here " + currentAbilityIcon.abilityName + " " + currentAbilityName);
				}

				if (currentAbilityIcon.abilityName == currentAbilityName) {
					if (showDebugPrint) {
						print ("current ability icon contained the ability removed, assigning new one");
					}

					bool newAbilityToAssignOnCurrentDpadIconInfoConfigured = false;

					for (int i = 0; i < dpadIconInfoList.Count; i++) {
						if (!newAbilityToAssignOnCurrentDpadIconInfoConfigured) {
							dpadIconInfo currentDpadIconInfo = dpadIconInfoList [i];

							if (currentDpadIconInfo.dpadAbilityIndex > -1) {
								if (showDebugPrint) {
									print ("new ability configured on the current ability icon");
								}

								newAbilityToAssignOnCurrentDpadIconInfoConfigured = true;

								setDpadIconInfo (currentAbilityIcon, currentDpadIconInfo.dpadAbilityIndex);

								if (mainPlayerAbilitiesSystem.currentAbilityInfo != null && mainPlayerAbilitiesSystem.currentAbilityInfo.Name == currentAbilityName) {
									mainPlayerAbilitiesSystem.setCurrentAbilityByName (currentDpadIconInfo.abilityName);
								}
							}
						}
					}
				}
			}
		} else {
			if (showDebugPrint) {
				print ("check to rmeove the ability assigned on current dpad icon");
			}

			setDpadIconInfo (currentAbilityIcon, -1);

			mainPlayerAbilitiesSystem.removeCurrentAbilityInfo ();
		}
	}

	public void updateDpadIconsOnWheelMenu ()
	{
		for (int j = 0; j < abilitySlotInfoList.Count; j++) {

			abilitySlotInfo newAbilitySlotInfo = abilitySlotInfoList [j];

			if (newAbilitySlotInfo.slotActive) {

				int abilityIndex = getDpadIconInfoListIndexByAbilityIndex (newAbilitySlotInfo.abilityIndex);

				if (abilityIndex > -1) {
					dpadIconInfo currentDpadIconInfo = dpadIconInfoList [abilityIndex];

					currentDpadIconInfo.abilitySlotIndex = j;

					newAbilitySlotInfo.currentDpadDirection = currentDpadIconInfo.currentDpadDirection;

					if (!newAbilitySlotInfo.dpadIcon.activeSelf) {
						newAbilitySlotInfo.dpadIcon.SetActive (true);
					}

					newAbilitySlotInfo.dpadIcon.transform.rotation = Quaternion.identity;

					if (newAbilitySlotInfo.upDpadIcon.activeSelf) {
						newAbilitySlotInfo.upDpadIcon.SetActive (false);
					}

					if (newAbilitySlotInfo.downDpadIcon.activeSelf) {
						newAbilitySlotInfo.downDpadIcon.SetActive (false);
					}

					if (newAbilitySlotInfo.leftDpadIcon.activeSelf) {
						newAbilitySlotInfo.leftDpadIcon.SetActive (false);
					}

					if (newAbilitySlotInfo.rightDpadIcon.activeSelf) {
						newAbilitySlotInfo.rightDpadIcon.SetActive (false);
					}

					switch (newAbilitySlotInfo.currentDpadDirection) {
					case dpadDirection.up:
						if (!newAbilitySlotInfo.upDpadIcon.activeSelf) {
							newAbilitySlotInfo.upDpadIcon.SetActive (true);
						}

						break;
					case dpadDirection.down:
						if (!newAbilitySlotInfo.downDpadIcon.activeSelf) {
							newAbilitySlotInfo.downDpadIcon.SetActive (true);
						}

						break;
					case dpadDirection.left:
						if (!newAbilitySlotInfo.leftDpadIcon.activeSelf) {
							newAbilitySlotInfo.leftDpadIcon.SetActive (true);
						}

						break;
					case dpadDirection.right:
						if (!newAbilitySlotInfo.rightDpadIcon.activeSelf) {
							newAbilitySlotInfo.rightDpadIcon.SetActive (true);
						}

						break;
					}
				} else {
					if (newAbilitySlotInfo.dpadIcon.activeSelf) {
						newAbilitySlotInfo.dpadIcon.SetActive (false);
					}
				}
			}
		}
	}

	public int getDpadIconInfoListIndexByAbilityIndex (int abilityIndex)
	{
		for (int j = 0; j < dpadIconInfoList.Count; j++) {
			if (dpadIconInfoList [j].dpadAbilityIndex == abilityIndex) {
				return j;
			}
		}

		return -1;
	}

	public int getDpadIconInfoListIndexByAbilityName (string abilityName)
	{
		for (int j = 0; j < dpadIconInfoList.Count; j++) {
			if (dpadIconInfoList [j].abilityName == abilityName) {
				return j;
			}
		}

		return -1;
	}

	public void setDpadIconsInfo ()
	{
		if (!dpadDirectionsAssigned) {

			bool anyAbilityAssignedOnDpad = false;

			int currentAbilityIndex = 0;

			int abilityInfoListCount = mainPlayerAbilitiesSystem.abilityInfoList.Count;

			for (int i = 0; i < dpadIconInfoList.Count; i++) {

				bool abilityFound = false;

				while (!abilityFound) {
					if (currentAbilityIndex < abilityInfoListCount) {
						if (mainPlayerAbilitiesSystem.abilityInfoList [currentAbilityIndex].abilityEnabled &&
						    mainPlayerAbilitiesSystem.abilityInfoList [currentAbilityIndex].abilityVisibleOnWheelSelection &&
						    mainPlayerAbilitiesSystem.abilityInfoList [currentAbilityIndex].abilityCanBeShownOnWheelSelection) {

							abilityFound = true;

							anyAbilityAssignedOnDpad = true;
						} else {
							currentAbilityIndex++;
						}
					} else {
						abilityFound = true;
					}
				}

				if (currentAbilityIndex < abilitySlotInfoList.Count) {
					setDpadIconInfo (dpadIconInfoList [i], currentAbilityIndex);

					currentAbilityIndex++;
				}
			}

			dpadDirectionsAssigned = true;

			checkEventsOnAbilitiesAssignedOnDpad (anyAbilityAssignedOnDpad);

			if (anyAbilityAssignedOnDpad) {
				if (showDebugPrint) {
					print ("abilities assigned");
				}
			} else {
				if (showDebugPrint) {
					print ("abilities not assigned");
				}

				for (int j = 0; j < dpadIconInfoList.Count; j++) {

					dpadIconInfo newDpadIconInfo = dpadIconInfoList [j];

					if (!newDpadIconInfo.dpadIconAssigned && newDpadIconInfo.dpadIconGameObject.activeSelf) {
						newDpadIconInfo.dpadIconGameObject.SetActive (false);

						newDpadIconInfo.dpadAbilityIndex = -1;
					}
				}
					
				mainPlayerAbilitiesSystem.removeCurrentAbilityInfo ();
			}
		}
	}

	public void checkEventsOnAbilitiesAssignedOnDpad (bool state)
	{
		if (dpadEventsChecked != state || !dpadEventsInitialized) {

			dpadEventsChecked = state;

			if (state) {
				eventOnAbilitiesAssignedOnDpad.Invoke ();
			} else {
				eventOnAbilitiesNotAssignedOnDpad.Invoke ();
			}
		}

		dpadEventsInitialized = true;
	}

	public List<abilitySlotInfo> getAbilitySlotInfoList ()
	{
		return abilitySlotInfoList;
	}

	public int getAbilityIndexByName (string abilityName)
	{
		for (int i = 0; i < mainPlayerAbilitiesSystem.abilityInfoList.Count; i++) {

			abilityInfo currentAbilityInfo = mainPlayerAbilitiesSystem.abilityInfoList [i];

			if (currentAbilityInfo.Name == abilityName) {
				return i;
			}
		}

		return -1;
	}

	public void setDpadIconInfo (dpadIconInfo newDpadIconInfo, int currentAbilityIndex)
	{
		if (currentAbilityIndex > -1) {
			abilityInfo newAbilitySlotInfo = mainPlayerAbilitiesSystem.abilityInfoList [currentAbilityIndex];

			newDpadIconInfo.abilityName = newAbilitySlotInfo.Name;
			newDpadIconInfo.dpadAbilityIndex = currentAbilityIndex;
			newDpadIconInfo.iconActive = true;
			newDpadIconInfo.backgroundIcon.texture = newAbilitySlotInfo.abilityTexture;
			newDpadIconInfo.icon.sprite = newAbilitySlotInfo.abilitySprite;

			if (!newDpadIconInfo.iconActiveGameObject.activeSelf) {
				newDpadIconInfo.iconActiveGameObject.SetActive (true);
			}

			newDpadIconInfo.dpadIconAssigned = true;

			if (!newDpadIconInfo.dpadIconGameObject.activeSelf) {
				newDpadIconInfo.dpadIconGameObject.SetActive (true);
			}
		} else {
			newDpadIconInfo.abilityName = "";
			newDpadIconInfo.dpadAbilityIndex = -1;
			newDpadIconInfo.iconActive = false;

			if (!newDpadIconInfo.iconActiveGameObject.activeSelf) {
				newDpadIconInfo.iconActiveGameObject.SetActive (false);
			}

			newDpadIconInfo.dpadIconAssigned = false;
		}

		for (int j = 0; j < dpadIconInfoList.Count; j++) {
			if (!dpadIconInfoList [j].dpadIconAssigned && dpadIconInfoList [j].dpadIconGameObject.activeSelf) {
				dpadIconInfoList [j].dpadIconGameObject.SetActive (false);
			}
		}
	}

	public void playSound (AudioClip sound)
	{
		if (mainAudioSource != null) {
			mainAudioSource.PlayOneShot (sound);
		}
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	public void updateEnergySlider ()
	{
		if (useDpadPanel && Application.isPlaying) {
			abilitiesEnergySlider.maxValue = mainEnerySlider.maxValue;
			abilitiesEnergySlider.value = mainEnerySlider.value;			
		}
	}

	public void activateAbilityCoolDown (string abilityName)
	{
		int dpadAbilityIndex = getDpadIconInfoListIndexByAbilityName (abilityName);

		if (dpadAbilityIndex > -1) {

			dpadIconInfo currentDpadIconInfo = dpadIconInfoList [dpadAbilityIndex];

			if (currentDpadIconInfo.limitTextGameObject.activeSelf) {
				currentDpadIconInfo.limitTextGameObject.SetActive (false);
			}

			if (currentDpadIconInfo.coolDownCoroutine != null) {
				StopCoroutine (currentDpadIconInfo.coolDownCoroutine);
			}

			currentDpadIconInfo.coolDownCoroutine = StartCoroutine (activateAbilityCoolDownCoroutine (currentDpadIconInfo));
		}

		if (useCurrentAbilityIcon) {
			if (currentAbilityIcon.abilityName.Equals (abilityName)) {
				if (currentAbilityIcon.limitTextGameObject.activeSelf) {
					currentAbilityIcon.limitTextGameObject.SetActive (false);
				}

				if (currentAbilityIcon.coolDownCoroutine != null) {
					StopCoroutine (currentAbilityIcon.coolDownCoroutine);
				}

				currentAbilityIcon.coolDownCoroutine = StartCoroutine (activateAbilityCoolDownCoroutine (currentAbilityIcon));
			}
		}
	}

	IEnumerator activateAbilityCoolDownCoroutine (dpadIconInfo currentDpadIconInfo)
	{
		if (showDebugPrint) {
			print (currentDpadIconInfo.abilityName);
		}

		float t = mainPlayerAbilitiesSystem.abilityInfoList [currentDpadIconInfo.dpadAbilityIndex].coolDownDuration;

		currentDpadIconInfo.icon.fillAmount = 0;

		float timer = 0;

		float fillamount = 0;

		while (timer < t) {
			timer += Time.deltaTime;

			fillamount = timer / t;

			currentDpadIconInfo.icon.fillAmount = Mathf.MoveTowards (currentDpadIconInfo.icon.fillAmount, fillamount, fillamount);
			yield return null;
		}
	}

	public void activateAbilityLimit (string abilityName)
	{
		int dpadAbilityIndex = getDpadIconInfoListIndexByAbilityName (abilityName);

		if (dpadAbilityIndex > -1) {
			dpadIconInfo currentDpadIconInfo = dpadIconInfoList [dpadAbilityIndex];

			stopActivateAbilityLimit (currentDpadIconInfo);

			currentDpadIconInfo.limitCoroutine = StartCoroutine (activateAbilityLimitCoroutine (currentDpadIconInfo));
		}

		if (useCurrentAbilityIcon) {
			if (currentAbilityIcon.abilityName.Equals (abilityName)) {
				stopActivateAbilityLimit (currentAbilityIcon);

				currentAbilityIcon.limitCoroutine = StartCoroutine (activateAbilityLimitCoroutine (currentAbilityIcon));
			}
		}
	}

	public void stopActivateAbilityLimit (dpadIconInfo currentDpadIconInfo)
	{
		if (currentDpadIconInfo != null) {
			if (currentDpadIconInfo.limitCoroutine != null) {
				StopCoroutine (currentDpadIconInfo.limitCoroutine);
			}

			if (currentDpadIconInfo.limitTextGameObject.activeSelf) {
				currentDpadIconInfo.limitTextGameObject.SetActive (false);
			}
		}
	}

	IEnumerator activateAbilityLimitCoroutine (dpadIconInfo currentDpadIconInfo)
	{
		if (showDebugPrint) {
			print (currentDpadIconInfo.abilityName);
		}

		float t = mainPlayerAbilitiesSystem.abilityInfoList [currentDpadIconInfo.dpadAbilityIndex].timeLimit;

		float timer = 0;

		if (!currentDpadIconInfo.limitTextGameObject.activeSelf) {
			currentDpadIconInfo.limitTextGameObject.SetActive (true);
		}

		while (timer < t) {
			timer += Time.deltaTime;

			currentDpadIconInfo.limitText.text = (t - timer).ToString ("##.0");

			yield return null;
		}

		currentDpadIconInfo.limitTextGameObject.SetActive (false);
	}

	public void setInputPausedState (bool state)
	{
		inputPaused = state;
	}

	bool canUseInput ()
	{
		if (playerControllerManager.iscloseCombatAttackInProcess ()) {
			return false;
		}

		return true;
	}

	//INPUT FUNCTIONS
	public void inputOpenOrCloseAbilityWheel (bool openAbilityWheel)
	{
		if (!abilityListManagerEnabled) {
			return;
		}

		if (inputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
			return;
		}

		//if the select Ability button is holding, enable the Ability wheel to select Ability
		if (openAbilityWheel) {
			selectAbilitySlots (true);
		} else {
			selectAbilitySlots (false);
		}
	}

	public void inputStartToPressAbilityWheelPanel ()
	{
		if (!abilityListManagerEnabled) {
			return;
		}

		if (inputPaused) {
			return;
		}

		if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (abilityListManagerEnabled) {
			selectAbilitySlots (true);
		}
	}

	public void inputStopToPressAbilityWheelPanel ()
	{
		if (!abilityListManagerEnabled) {
			return;
		}

		if (inputPaused) {
			return;
		}

		if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (selectingAbility) {
			selectAbilitySlots (false);
		}
	}

	public void inputUseDpadDirectionPressDown (int dpadIndex)
	{
		if (!abilityListManagerEnabled) {
			return;
		}

		if (inputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (mainPlayerAbilitiesSystem.isAbilityInputInUse ()) {
			return;
		}

		if (selectingAbility) {

			if (closestSlot != null) {

				bool abilityFound = false;
				for (int i = 0; i < dpadIconInfoList.Count; i++) {
					if (!abilityFound && dpadIconInfoList [i].dpadAbilityIndex == closestSlot.abilityIndex) {
						setDpadIconInfo (dpadIconInfoList [i], -1);

						abilityFound = true;
					}
				}

				setDpadIconInfo (dpadIconInfoList [dpadIndex], closestSlot.abilityIndex);

				updateDpadIconsOnWheelMenu ();

				checkEventsOnAbilitiesAssignedOnDpad (true);
			}

			return;
		}

		if (currentAbilityPressedIndex == -1) {
			if (dpadIconInfoList [dpadIndex].iconActive) {

				currentAbilityPressedIndex = dpadIndex;

				print (dpadIconInfoList [dpadIndex].abilityName + " " + dpadIndex);

				mainPlayerAbilitiesSystem.setCurrentAbilityByName (dpadIconInfoList [dpadIndex].abilityName);

				mainPlayerAbilitiesSystem.pressDownUseCurrentAbility ();

				if (useCurrentAbilityIcon) {
					setDpadIconInfo (currentAbilityIcon, dpadIconInfoList [dpadIndex].dpadAbilityIndex);

					updateCurrentAbilityInputText ();

					checkEventsOnAbilitiesAssignedOnDpad (true);
				}
			}
		}
	}

	public void inputUseDpadDirectionPressHold ()
	{
		if (!abilityListManagerEnabled) {
			return;
		}

		if (inputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (mainPlayerAbilitiesSystem.isAbilityInputInUse ()) {
			return;
		}

		if (selectingAbility) {
			return;
		}

		if (currentAbilityPressedIndex != -1) {
			mainPlayerAbilitiesSystem.pressHoldUseCurrentAbility ();
		}
	}

	public void inputUseDpadDirectionPressUp ()
	{
		print ("POAIHDhs");

		if (!abilityListManagerEnabled) {
			return;
		}

		if (inputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (mainPlayerAbilitiesSystem.isAbilityInputInUse ()) {
			return;
		}

		if (selectingAbility) {
			return;
		}

		if (currentAbilityPressedIndex != -1) {
			mainPlayerAbilitiesSystem.pressUpUseCurrentAbility ();

			currentAbilityPressedIndex = -1;

			print (currentAbilityPressedIndex);
		}
	}

	public void resetCurrentAbilityPressedIndexByName (string abilityName)
	{
		if (!abilityListManagerEnabled) {
			return;
		}

		if (currentAbilityPressedIndex != -1) {
			if (currentAbilityPressedIndex < dpadIconInfoList.Count &&
			    dpadIconInfoList [currentAbilityPressedIndex].abilityName.Equals (abilityName)) {

				currentAbilityPressedIndex = -1;
			}
		}
	}

	public void inputTouchPressDown (int dpadIndex)
	{
		if (!abilityListManagerEnabled) {
			return;
		}

		if (inputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (mainPlayerAbilitiesSystem.isAbilityInputInUse ()) {
			return;
		}

		if (touchPlatform || pauseManager.isUsingTouchControls ()) {
			inputUseDpadDirectionPressDown (dpadIndex);

			touchPressHoldActive = true;
		}
	}

	public void inputTouchPressUp ()
	{
		if (!abilityListManagerEnabled) {
			return;
		}

		if (inputPaused) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (mainPlayerAbilitiesSystem.isAbilityInputInUse ()) {
			return;
		}

		if (touchPlatform || pauseManager.isUsingTouchControls ()) {
			touchPressHoldActive = false;

			inputUseDpadDirectionPressUp ();
		}
	}

	public bool isSelectingAbilityActive ()
	{
		return selectingAbility;
	}

	[System.Serializable]
	public class abilitySlotInfo
	{
		public string Name;
		public bool slotActive;
		public GameObject slot;
		public RectTransform slotRectTransform;

		public int abilityIndex;

		public bool slotVisible = true;

		public RawImage abilityIcon;
		public Texture abilityTexture;
		public GameObject currentAbilitySelectedIcon;

		public RectTransform slotPivot;
		public RectTransform slotCenter;

		public RectTransform innerSlot;

		public Coroutine sizeCoroutine;

		public dpadDirection currentDpadDirection;

		public GameObject dpadIcon;
		public GameObject upDpadIcon;
		public GameObject downDpadIcon;
		public GameObject leftDpadIcon;
		public GameObject rightDpadIcon;
	}

	[System.Serializable]
	public class dpadIconInfo
	{
		public string Name;
		public string abilityName;
		public Image icon;
		public RawImage backgroundIcon;
		public GameObject iconActiveGameObject;
		public bool iconActive;
		public int dpadAbilityIndex;

		public GameObject limitTextGameObject;
		public Text limitText;

		public GameObject dpadIconGameObject;

		public dpadDirection currentDpadDirection;

		public int abilitySlotIndex;

		public bool dpadIconAssigned;

		public Coroutine coolDownCoroutine;
		public Coroutine limitCoroutine;
	}
}