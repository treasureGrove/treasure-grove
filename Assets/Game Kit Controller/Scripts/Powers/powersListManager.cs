using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class powersListManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool powerListManagerEnabled;

	public Vector2 range = new Vector2 (5f, 3f);
	public float rotationHUDSpeed = 20;
	public float doubleTapTime = 0.5f;
	public float holdTapTime = 0.7f;

	public int numberOfPowerSlots = 8;

	public int numberOfEnabledPowers;

	public bool adjustSlotNumberToAmountOfPowers;

	public bool useBlurUIPanel = true;

	public bool slowDownTimeWhenMenuActive;
	public float timeScaleWhenMenuActive;

	public bool selectPowerOnMenuClose;

	public bool useSoundOnSlot;
	public AudioClip soundEffect;

	public bool changeCurrentSlotSize;
	public float changeCurrentSlotSpeed;
	public float changeCurrentSlotMultiplier;
	public float distanceFromCenterToSelectPower = 10;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool editingPowers;
	public bool selectingPower;

	public bool touching;

	public List<powerSlotInfo> powerSlotInfoList = new List<powerSlotInfo> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool usedByAI;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject powersSlotsMenu;
	public GameObject powersListContent;
	public GameObject powersListElement;
	public Transform powersSlotsWheel;
	public GameObject completePowersWheel;
	public GameObject completePowersList;
	public Text currentPowerNameText;
	public GameObject slotArrow;
	public Transform slotArrowIcon;
	public Scrollbar powerListScrollBar;

	public GameObject powerListWheelElement;

	public AudioSource mainAudioSource;

	public menuPause pauseManager;
	public playerController playerControllerManager;
	public otherPowers powersManager;
	public timeBullet timeBulletManager;

	powerSlotInfo closestSlot;
	powerSlotInfo previousClosestSlot;

	powerSlotInfo currentPowerSlotSelected;
	powerSlotInfo currentPowerSlotFound;

	List<GameObject> powersListElements = new List<GameObject> ();

	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();

	GameObject buttonToMove;
	RectTransform buttonToMoveRectTransform;
	GameObject slotSelected;
	GameObject slotFound;
	otherPowers.Powers previousPower;
	float lastButtonTime;
	float screenWidth;
	float screenHeight;
	bool touchPlatform;

	bool touchingScreenCenter;
	Vector2 mRot = Vector2.zero;
	Quaternion mStart;
	int tapCount = 0;

	int numberOfCurrentPowers;

	Touch currentTouch;

	float anglePerSlot;
	float currentAngle;
	float lastTimeTouching;
	bool isFirstPowerSelected;

	Vector3 initialTouchPosition;

	float currentArrowAngle;

	Vector2 currentTouchPosition;

	bool slotDraggedFromWheel;
	bool slotDroppedOnWheel;

	float currentDistance;

	bool fingerPressingTouchPanel;

	bool initializingPowerSlots = true;

	void Start ()
	{
		if (usedByAI) {
			return;
		}

		if (!powerListManagerEnabled) {
			return;
		}
		//for every power created in the otherPowers inspector, add to the list of the power manager

		for (int i = 0; i < powersManager.shootsettings.powersList.Count; i++) {
			//create the list of power in the right side of the HUD
			otherPowers.Powers currentPower = powersManager.shootsettings.powersList [i];

			GameObject powersListElementClone = (GameObject)Instantiate (powersListElement, Vector3.zero, Quaternion.identity, powersListContent.transform);
			powersListElementClone.name = currentPower.Name;

			powersListElementClone.transform.localScale = Vector3.one;

			powerSlotInfo newPowerSlotInfo = powersListElementClone.GetComponent<powerSlotElement> ().slotInfo;

			newPowerSlotInfo.Name = currentPower.Name;
			newPowerSlotInfo.powerInfo = currentPower;

			Texture powerIconTexture = currentPower.texture;
			if (powerIconTexture != null) {
				newPowerSlotInfo.powerIcon.texture = powerIconTexture;
			}

			newPowerSlotInfo.slotActive = true;

			//add this element to the list
			powersListElements.Add (powersListElementClone);
			//if the number of powers enabled is higher thah the powers created, disable the other powers of the player

			if (!currentPower.powerEnabled) {
				powersListElementClone.SetActive (false);
			} else {
				numberOfEnabledPowers++;
			}
		}

		powersListElement.SetActive (false);

		//for every power created in the other powers inspector, add to the list of the power manager
		for (int i = 0; i < numberOfPowerSlots; i++) {
			GameObject newPowerListWheelElement = (GameObject)Instantiate (powerListWheelElement, Vector3.zero, Quaternion.identity, powersSlotsWheel);
			newPowerListWheelElement.name = "Power Slot " + (i + 1);

			newPowerListWheelElement.transform.localScale = Vector3.one;
			newPowerListWheelElement.transform.position = powerListWheelElement.transform.position;

			powerSlotInfo newPowerSlotInfo = newPowerListWheelElement.GetComponent<powerSlotElement> ().slotInfo;

			//add this element to the list
			powerSlotInfoList.Add (newPowerSlotInfo);
		}

		powerListWheelElement.SetActive (false);

		//get the scroller in the powers manager
		powerListScrollBar.value = 1;

		//get the rotation of the powers wheel
		mStart = completePowersWheel.transform.localRotation;

		//check if the platform is a touch device or not
		touchPlatform = touchJoystick.checkTouchPlatform ();

		updateSlotsInfo (false);

		initializingPowerSlots = false;
	}

	void Update ()
	{
		if (usedByAI) {
			return;
		}
	
		//if the player is selecting, editing the powers, or the touch controls are enabled, then
		if (editingPowers || selectingPower || pauseManager.isUsingTouchControls ()) {
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

				if (currentTouch.phase == TouchPhase.Began) {
					touching = true;

					if (fingerPressingTouchPanel) {
						touchingScreenCenter = true;
						initialTouchPosition = currentTouchPosition;
						lastTimeTouching = Time.time;
					}
		
					//if the edit powers manager is open, then
					if (editingPowers) {
						//check where the mouse or the finger press, to get a power list element, to edit the powers
						captureRaycastResults.Clear ();
						PointerEventData p = new PointerEventData (EventSystem.current);
						p.position = currentTouchPosition;
						p.clickCount = i;
						p.dragging = false;
						EventSystem.current.RaycastAll (p, captureRaycastResults);

						slotDraggedFromWheel = false;

						foreach (RaycastResult r in captureRaycastResults) {
							//if the object pressed is a powerListElement, and it is enabled
							for (int k = 0; k < powerSlotInfoList.Count; k++) {
								if (powerSlotInfoList [k].slotActive && powerSlotInfoList [k].slotWithPowerAssigned) {
									if (powerSlotInfoList [k].powerIconSlot == r.gameObject) {
										//the power element pressed is in the wheel
										//if the texture is enabled, grab the element, to remove from the wheel or to change its position
										buttonToMove = (GameObject)Instantiate (r.gameObject, r.gameObject.transform.position, Quaternion.identity);
										buttonToMove.transform.SetParent (powersSlotsMenu.transform.parent);
										powerSlotInfoList [k].powerIcon.texture = null;
										slotSelected = r.gameObject;

										currentPowerSlotSelected = powerSlotInfoList [k];
										buttonToMoveRectTransform = buttonToMove.GetComponent<RectTransform> ();
										slotDraggedFromWheel = true;

										return;
									}
								}
							}

							for (int k = 0; k < powersListElements.Count; k++) {
								powerSlotInfo newPowerSlotInfo = powersListElements [k].GetComponent<powerSlotElement> ().slotInfo;

								//the power element pressed is in the list in the right
								if (newPowerSlotInfo.powerIconSlot == r.gameObject) {
									//grab the list element to drop it in the wheel
									buttonToMove = (GameObject)Instantiate (r.gameObject, r.gameObject.transform.position, Quaternion.identity);
									buttonToMove.transform.SetParent (powersSlotsMenu.transform.parent);
									buttonToMoveRectTransform = buttonToMove.GetComponent<RectTransform> ();

									currentPowerSlotSelected = newPowerSlotInfo;
								}
							}
						}
					}
				}

				//if the power list element is grabbed, follow the mouse/finger position in screen
				if ((currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved)) {
					if (editingPowers) {
						if (buttonToMove != null) {
							buttonToMoveRectTransform.position = currentTouchPosition;
						}
					}
				}

				//if the mouse/finger press is released, then
				if (currentTouch.phase == TouchPhase.Ended) {
					touching = false;
					touchingScreenCenter = false;

					//if the player was editing the powers
					if (editingPowers) {
						if (buttonToMove != null) {
							//get the elements in the position where the player released the power element
							captureRaycastResults.Clear ();
							PointerEventData p = new PointerEventData (EventSystem.current);
							p.position = currentTouchPosition;
							p.clickCount = i;
							p.dragging = false;
							EventSystem.current.RaycastAll (p, captureRaycastResults);

							slotDroppedOnWheel = false;

							foreach (RaycastResult r in captureRaycastResults) {
								if (r.gameObject != buttonToMove) {
									//if the power element was released above other power element from the wheel, store the power element from the wheel
									for (int k = 0; k < powerSlotInfoList.Count; k++) {
										if (slotFound == null && powerSlotInfoList [k].slotActive) {
											if (powerSlotInfoList [k].powerIconSlot == r.gameObject) {
												slotFound = r.gameObject;
												currentPowerSlotFound = powerSlotInfoList [k];
												slotDroppedOnWheel = true;
											}
										}
									}
								}
							}

							//if the power element was released above other power element from the wheel, then
							if (slotFound != null) {
								//check that the power dragged is not already in the wheel and that the power is released above the wheel, and not the list in the right
								if (slotDroppedOnWheel && (!checkDuplicatedSlot (currentPowerSlotSelected.powerInfo.Name) || slotDraggedFromWheel)) {
									bool empty = true;
									//if the stored power element has a texture, then a power is going to be replaced, so store it to remove it after
									if (currentPowerSlotFound.slotWithPowerAssigned) {
										empty = false;
										previousPower = currentPowerSlotFound.powerInfo;
									}

									//set the data of the dragged power in the wheel
									assignPowerToSlot (currentPowerSlotSelected.powerInfo, currentPowerSlotFound, true);

									otherPowers.Powers currentPowerOnPowersManager = powersManager.getCurrentPower ();

									if (showDebugPrint) {
										print (slotSelected.name + " " + slotFound.name);
									}

									//if the element dragged and dropped was a power inside the wheel, then 
									if (slotSelected != null) {
										if (slotSelected != slotFound) {

											if (empty) {
												//the dropped power is released in a empty element of the wheel
												if (showDebugPrint) {
													print ("changed to empty");
												}
											} else {
												//the dropped power is released in anoter power element, so change the previous power for the new power
												powersManager.changePowerState (previousPower, -1, false, -1);

												if (showDebugPrint) {
													print ("changed to occupied");
												}
											}

											//set the new data in that power
											assignPowerToSlot (new otherPowers.Powers (), currentPowerSlotSelected, false);
										} else {
											//the dropped power is released in same position where it was previously
											if (showDebugPrint) {
												print ("change to the same");
											}
										}
									}
									//else, the element dragged and dropped was a power of the list in the right of the screen
									else {
										if (empty) {
											//the dropped power is released in a empty element of the wheel
											if (showDebugPrint) {
												print ("set in empty");
											}

											powersManager.changePowerState (currentPowerSlotSelected.powerInfo, currentPowerSlotFound.numberKey, true, 1);

										} else {
											//the dropped power is released in anoter power element, so change the previous power for the new power
											if (showDebugPrint) {
												print ("set in occupied");
											}

											powersManager.changePowerState (previousPower, -1, false, -1);

											currentPowerOnPowersManager = currentPowerSlotSelected.powerInfo;
										}
									}

									updateCurrentSelectedPowerSlotIcon (currentPowerOnPowersManager);

									//remove the dragged object
									Destroy (buttonToMove);
								} else {
									Destroy (buttonToMove);

									if (showDebugPrint) {
										print ("power already added");
									}
								}
							}

							//the dragged power is released in any other position
							else {
								//check if the power was grabbed from the wheel, in that case the power has been removed, so change the info in otherpowers
								if (slotDraggedFromWheel) {
									powersManager.changePowerState (currentPowerSlotSelected.powerInfo, -1, false, -1);

									//set the info in the powers wheel
									assignPowerToSlot (new otherPowers.Powers (), currentPowerSlotSelected, false);

									updateCurrentSelectedPowerSlotIcon (powersManager.getCurrentPower ());

									if (showDebugPrint) {
										print ("power removed");
									}
								}

								//remove the dragged object
								Destroy (buttonToMove);
								buttonToMoveRectTransform = null;
								currentPowerSlotSelected = null;
							}

							slotFound = null;
							slotSelected = null;
						}
					}

					//if the player is selecting a power, enable only the powers wheel 
					if (selectingPower) {
						selectPowersSlots ();
					}

					fingerPressingTouchPanel = false;
				}

				if (selectingPower) {
					//get the current screen size
					screenWidth = Screen.width;
					screenHeight = Screen.height;

					Vector2 slotDirection = new Vector2 (currentTouchPosition.x, currentTouchPosition.y) - slotArrow.GetComponent<RectTransform> ().anchoredPosition;
					Vector2 screenCenter = new Vector2 (screenWidth, screenHeight) / 2;

					slotDirection -= screenCenter;
					currentArrowAngle = Mathf.Atan2 (slotDirection.y, slotDirection.x);
					currentArrowAngle -= 90 * Mathf.Deg2Rad;
					slotArrow.transform.localRotation = Quaternion.Euler (0, 0, currentArrowAngle * Mathf.Rad2Deg);

					if (GKC_Utils.distance (initialTouchPosition, currentTouchPosition) > distanceFromCenterToSelectPower) {
						if (!slotArrow.gameObject.activeSelf) {
							slotArrow.gameObject.SetActive (true);
						}

						//make the slots wheel looks toward the mouse
						float halfWidth = screenWidth * 0.5f;
						float halfHeight = screenHeight * 0.5f;
						float x = Mathf.Clamp ((currentTouchPosition.x - halfWidth) / halfWidth, -1f, 1f);
						float y = Mathf.Clamp ((currentTouchPosition.y - halfHeight) / halfHeight, -1f, 1f);
						mRot = Vector2.Lerp (mRot, new Vector2 (x, y), Time.deltaTime * rotationHUDSpeed);

						completePowersWheel.transform.localRotation = mStart * Quaternion.Euler (mRot.y * range.y, -mRot.x * range.x, 0f);

						//get the power inside the wheel closest to the mouse
						float distance = Mathf.Infinity;
						for (int k = 0; k < powerSlotInfoList.Count; k++) {
							if (powerSlotInfoList [k].slotActive) {

								currentDistance = GKC_Utils.distance (slotArrowIcon.position, powerSlotInfoList [k].powerIcon.transform.position);

								if (currentDistance < distance) {
									distance = currentDistance;
									closestSlot = powerSlotInfoList [k];
								}
							}
						}

						//set the name of the closes power in the center of the powers wheel
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

							if (!closestSlot.Name.Equals ("")) {
								currentPowerNameText.text = closestSlot.Name;
							}

							if (useSoundOnSlot) {
								playSound (soundEffect);
							}
						}

						//set the name of the closest power in the center of the power wheel
						if (powersManager.getCurrentPower () != closestSlot.powerInfo || !isFirstPowerSelected) {
							isFirstPowerSelected = true;

							if (!selectPowerOnMenuClose) {
								selectPower (closestSlot);
							}
						}
					}
				} else {
					if (slotArrow.gameObject.activeSelf) {
						slotArrow.gameObject.SetActive (false);
					}
				}

				//in a touch device, if the finger is touching the screen
				//check if the player keeps its finger in the center of the screen for a second, to enable the powers wheel, and without releasing the pressing,
				//move the finger towards a power, when the tap is released, the closest power to the finger position is set as the current power
				if (touching && touchingScreenCenter) {
					//and the tap is being holding inside the touch rect zone in the center of the screen, then
					if (fingerPressingTouchPanel) {
						//if the player is not selecting a power, get the time of the holding
						if (!selectingPower) {
							if (Time.time > holdTapTime + lastTimeTouching) {
								selectPowersSlots ();
							}
						}
					}
				}
			}

			if (tapCount > 0 && Time.time > lastButtonTime + doubleTapTime) {
				tapCount = 0;
			}
		}
	}

	public void setTouchingMenuPanelState (bool state)
	{
		fingerPressingTouchPanel = state;

		//if the finger tap is inside the rect zone in the upper left corner of the screen
		if (fingerPressingTouchPanel) {
			//check the time between taps, if the number is 2 and they are done quickly
			if (Time.time - lastButtonTime < doubleTapTime) {
				//open the edit powers manager
				lastButtonTime = 0;

				editPowersSlots ();
			}

			lastButtonTime = Time.time;	
			tapCount++;

			//reset the tap count
			if (tapCount == 2) {
				tapCount = 0;
				lastButtonTime = 0;
			}
		}

		pauseManager.setIgnoreDisableTouchZoneListState (fingerPressingTouchPanel);
	}

	public void updateCurrentSelectedPowerSlotIcon (otherPowers.Powers currentPowerOnPowersManager)
	{
		string currentPowerName = "";

		if (showDebugPrint) {
			print ("Set current power selected " + currentPowerOnPowersManager.Name);
		}

		for (int k = 0; k < powerSlotInfoList.Count; k++) {
			if (currentPowerOnPowersManager.Name.Equals (powerSlotInfoList [k].powerInfo.Name)) {

				if (!powerSlotInfoList [k].currentPowerSelectedIcon.activeSelf) {
					powerSlotInfoList [k].currentPowerSelectedIcon.SetActive (true);
				}

				currentPowerName = currentPowerOnPowersManager.Name;
			} else {
				if (powerSlotInfoList [k].currentPowerSelectedIcon.activeSelf) {
					powerSlotInfoList [k].currentPowerSelectedIcon.SetActive (false);
				}
			}
		}

		currentPowerNameText.text = currentPowerName;

		powersManager.setCurrentPowerByName (currentPowerName);
	}

	public void enableOrDisablePowerSlot (string powerName, bool state)
	{
		for (int i = 0; i < powersListElements.Count; i++) {
			if (powersListElements [i].name.Equals (powerName)) {

				if (powersListElements [i].activeSelf != state) {
					powersListElements [i].SetActive (state);
				}

				if (state) {
					numberOfEnabledPowers++;
				} else {
					numberOfEnabledPowers--;
				}
			}
		}
	}
		
	//enable the powers wheel and the list in the right to edit the current powers to select
	public void editPowersSlots ()
	{
		//check that the game is not paused, that the player is not selecting a power, using a device and that the power manager can be enabled
		if ((canBeOpened () || editingPowers) && !selectingPower) {

			if (powersManager.getNumberOfPowersAvailable () == 0) {
				return;
			}

			editingPowers = !editingPowers;

			pauseManager.openOrClosePlayerMenu (editingPowers, powersSlotsMenu.transform, useBlurUIPanel);

			//enable the powers wheel and the list
			completePowersList.SetActive (editingPowers);

			completePowersWheel.SetActive (editingPowers);

			pauseManager.setIngameMenuOpenedState ("Powers List Manager", editingPowers, true);

			slotArrow.SetActive (!editingPowers);

			currentPowerNameText.text = "";

			pauseManager.enableOrDisablePlayerMenu (editingPowers, false, false);

			//reset the wheel rotation
			completePowersWheel.transform.localRotation = Quaternion.identity;

			if (editingPowers) {
				updateSlotsInfo (true);
			}
		}
	}

	public void selectPower (powerSlotInfo slotInfo)
	{
		powersManager.setPower (slotInfo.powerInfo);

		for (int k = 0; k < powerSlotInfoList.Count; k++) {
			if (slotInfo.powerInfo.Name.Equals (powerSlotInfoList [k].powerInfo.Name)) {
				if (!powerSlotInfoList [k].currentPowerSelectedIcon.activeSelf) {
					powerSlotInfoList [k].currentPowerSelectedIcon.SetActive (true);
				}
			} else {
				if (powerSlotInfoList [k].currentPowerSelectedIcon.activeSelf) {
					powerSlotInfoList [k].currentPowerSelectedIcon.SetActive (false);
				}
			}
		}
	}

	public void changeSlotSize (powerSlotInfo slotInfo, bool setRegularSize, float sizeMultiplier, bool smoothSizeChange)
	{
		if (slotInfo.sizeCoroutine != null) {
			StopCoroutine (slotInfo.sizeCoroutine);
		}

		slotInfo.sizeCoroutine = StartCoroutine (changeSlotSizeCoroutine (slotInfo, setRegularSize, sizeMultiplier, smoothSizeChange));
	}

	IEnumerator changeSlotSizeCoroutine (powerSlotInfo slotInfo, bool setRegularSize, float sizeMultiplier, bool smoothSizeChange)
	{
		Vector3 targetValue = Vector3.one;

		if (!setRegularSize) {
			targetValue *= sizeMultiplier;
		}

		if (smoothSizeChange) {
			while (slotInfo.slotCenter.localScale != targetValue) {
				slotInfo.slotCenter.localScale = Vector3.MoveTowards (slotInfo.slotCenter.localScale, targetValue, Time.deltaTime * changeCurrentSlotMultiplier);
			
				yield return null;
			}
		} else {
			slotInfo.slotCenter.localScale = targetValue;
		}
	}

	//enable the powers wheel to select the current powers to use
	public void selectPowersSlots ()
	{
		//check that the game is not paused, that the player is not editing the powers, using a device and that the power manager can be enabled
		if ((canBeOpened () || selectingPower) && !editingPowers) {
			selectingPower = !selectingPower;

			if (!selectingPower) {
				if (changeCurrentSlotSize && closestSlot != null) {
					changeSlotSize (closestSlot, true, changeCurrentSlotMultiplier, false);
				}
			}

			pauseManager.openOrClosePlayerMenu (selectingPower, powersSlotsMenu.transform, useBlurUIPanel);

			//enable the powers wheel
			completePowersWheel.SetActive (selectingPower);

			pauseManager.setIngameMenuOpenedState ("Powers List Manager", selectingPower, true);

			pauseManager.enableOrDisablePlayerMenu (selectingPower, false, false);

			//reset the arrow and the wheel rotation
			completePowersWheel.transform.localRotation = Quaternion.identity;
			slotArrow.transform.localRotation = Quaternion.identity;

			if (selectingPower) {
				updateSlotsInfo (false);

				if (!touchPlatform) {
					initialTouchPosition = touchJoystick.convertMouseIntoFinger ().position;
				} else {
					initialTouchPosition = Input.GetTouch (1).position;
				}

				if (slowDownTimeWhenMenuActive) {
					timeBulletManager.setBulletTimeState (true, timeScaleWhenMenuActive);
				}
			} else {
				isFirstPowerSelected = false;

				if (slowDownTimeWhenMenuActive) {
					timeBulletManager.setBulletTimeState (false, 1);
				}

				if (selectPowerOnMenuClose && closestSlot != null) {
					selectPower (closestSlot);
				}
			}

			closestSlot = null;
			previousClosestSlot = null;
		}
	}

	public void closePowerListManagerMenus ()
	{
		editingPowers = true;

		editPowersSlots ();

		selectingPower = true;

		selectPowersSlots ();
	}

	public bool canBeOpened ()
	{
		if (pauseManager.isGamePaused ()) {
			return false;
		}

		if (playerControllerManager.isUsingDevice ()) {
			return false;
		}

		if (!powerListManagerEnabled) {
			return false;
		}

		if (!powersManager.isPowersModeActive ()) {
			return false;
		}

		if (playerControllerManager.isPlayerMenuActive ()) {
			return false;
		}

		if (playerControllerManager.iscloseCombatAttackInProcess ()) {
			return false;
		}

		return true;
	}

	//check that the dropped power is not already in the wheel, using the power name
	public bool checkDuplicatedSlot (string powerName)
	{
		for (int i = 0; i < powerSlotInfoList.Count; i++) {
			if (powerSlotInfoList [i].Name == powerName) {
				return true;
			}
		}

		return false;
	}

	public int getPowerByNumberKeyIndex (int numberKeyToSearch)
	{
		for (int i = 0; i < powersManager.shootsettings.powersList.Count; i++) {
			if (powersManager.shootsettings.powersList [i].powerEnabled) {
				if (powersManager.shootsettings.powersList [i].numberKey == numberKeyToSearch) {
					return i;
				}
			}
		}

		return -1;
	}

	public void updateSlotsInfo (bool isEditingPowers)
	{
		//get the max amount of powers that the player can used currently
		if (adjustSlotNumberToAmountOfPowers) {
			numberOfCurrentPowers = powersManager.getNumberOfPowersAvailable ();

			if (numberOfCurrentPowers > numberOfPowerSlots) {
				numberOfCurrentPowers = numberOfPowerSlots;
			}

			if (numberOfEnabledPowers > numberOfPowerSlots) {
				numberOfEnabledPowers = numberOfPowerSlots;
			}

			if (isEditingPowers) {
				numberOfCurrentPowers = numberOfEnabledPowers;
			} else {
				if (numberOfCurrentPowers > numberOfEnabledPowers) {
					numberOfCurrentPowers = numberOfEnabledPowers;
				}
			}
			anglePerSlot = 360 / (float)numberOfCurrentPowers;
		} else {
			anglePerSlot = 360 / (float)numberOfPowerSlots;
		}

		currentAngle = 0;

		int currentSlotIndex = 0;

		bool anySlotAvailable = false;

		otherPowers.Powers currentPowerOnPowersManager = powersManager.getCurrentPower ();

		string currentPowerName = currentPowerOnPowersManager.Name;

		List<otherPowers.Powers> powersList = powersManager.shootsettings.powersList;

		if (initializingPowerSlots) {
			for (int i = 0; i < powersList.Count; i++) {

				bool powerAssigned = false;

				if (powersList [i].powerEnabled && powersList [i].numberKey >= 0) {
					//&& powersList [i].numberKey == (currentSlotIndex + 1)
					if (currentSlotIndex < powerSlotInfoList.Count) {

						powerSlotInfo newPowerSlotInfo = powerSlotInfoList [currentSlotIndex];
		
						otherPowers.Powers currentPower = powersList [i];

						newPowerSlotInfo.Name = currentPower.Name;
						newPowerSlotInfo.powerInfo = currentPower;
						currentPower.numberKey = currentSlotIndex + 1;

						Texture powerIconTexture = currentPower.texture;

						if (powerIconTexture != null) {
							newPowerSlotInfo.powerIcon.texture = powerIconTexture;
						}

						newPowerSlotInfo.slotWithPowerAssigned = true;

						anySlotAvailable = true;

						if (currentPowerName.Equals (newPowerSlotInfo.Name)) {
							if (!newPowerSlotInfo.currentPowerSelectedIcon.activeSelf) {
								newPowerSlotInfo.currentPowerSelectedIcon.SetActive (true);
							}

							currentPowerNameText.text = newPowerSlotInfo.Name;
						} else {
							if (newPowerSlotInfo.currentPowerSelectedIcon.activeSelf) {
								newPowerSlotInfo.currentPowerSelectedIcon.SetActive (false);
							}
						}

						currentSlotIndex++;

						powerAssigned = true;
					}

				}

				if (!powerAssigned) {
					powersList [i].powerAssigned = false;
					powersList [i].numberKey = -1;
				}
			}
		} else {
			for (int i = 0; i < powerSlotInfoList.Count; i++) {
				powerSlotInfo newPowerSlotInfo = powerSlotInfoList [i];

				if (newPowerSlotInfo.slotWithPowerAssigned) {	
					int powerIndex = powersManager.getPowerIndexByNumberKey (i + 1);

					if (powerIndex > -1) {
						otherPowers.Powers currentPower = powersList [powerIndex];

						newPowerSlotInfo.Name = currentPower.Name;
						newPowerSlotInfo.powerInfo = currentPower;

						Texture powerIconTexture = currentPower.texture;

						if (powerIconTexture != null) {
							newPowerSlotInfo.powerIcon.texture = powerIconTexture;
						}

						anySlotAvailable = true;

						if (currentPowerName.Equals (newPowerSlotInfo.Name)) {
							newPowerSlotInfo.currentPowerSelectedIcon.SetActive (true);
							currentPowerNameText.text = newPowerSlotInfo.Name;
						} else {
							newPowerSlotInfo.currentPowerSelectedIcon.SetActive (false);
						}
					}
				}
			}
		}

		if (anySlotAvailable) {
			int numberOfSlotsEnabled = 0;

			for (int j = 0; j < powerSlotInfoList.Count; j++) {
				powerSlotInfo newPowerSlotInfo = powerSlotInfoList [j];

				bool enableSlot = false;

				if (!adjustSlotNumberToAmountOfPowers) {
					enableSlot = true;

				} else {
					if (isEditingPowers) {
						if (j < numberOfCurrentPowers) {
							enableSlot = true;

						}
					} else {
						if (newPowerSlotInfo.slotWithPowerAssigned) {
							
							if (numberOfSlotsEnabled < numberOfCurrentPowers) {
								numberOfSlotsEnabled++;
								enableSlot = true;
							}
						}
					}
				}

				if (enableSlot) {
					newPowerSlotInfo.numberKey = j + 1;

					newPowerSlotInfo.powerNumberKeyText.text = newPowerSlotInfo.numberKey.ToString ();

					newPowerSlotInfo.slot.GetComponent<RectTransform> ().rotation = Quaternion.Euler (new Vector3 (0, 0, currentAngle));

					currentAngle += anglePerSlot;	

					newPowerSlotInfo.powerNumberKeyIcon.rotation = Quaternion.identity;

					newPowerSlotInfo.powerIconSlot.transform.rotation = Quaternion.identity;
					newPowerSlotInfo.slotActive = true;

					if (!newPowerSlotInfo.slot.activeSelf) {
						newPowerSlotInfo.slot.SetActive (true);
					}

					if (!newPowerSlotInfo.slotWithPowerAssigned) {
						newPowerSlotInfo.powerIcon.texture = null;
						newPowerSlotInfo.powerNumberKeyText.text = "";
					}
				} else {
					newPowerSlotInfo.slotActive = false;

					if (newPowerSlotInfo.slot.activeSelf) {
						newPowerSlotInfo.slot.SetActive (false);
					}
				}
			}
		} else {
			selectPowersSlots ();
		}
	}

	public void assignPowerToSlot (otherPowers.Powers currentPowerToAssign, powerSlotInfo powerSlotToUse, bool slotWithPowerAssigned)
	{
		if (showDebugPrint) {
			print (currentPowerToAssign.Name + " " + powerSlotToUse.numberKey);
		}

		powerSlotToUse.Name = currentPowerToAssign.Name;
		powerSlotToUse.powerInfo = currentPowerToAssign;
		currentPowerToAssign.numberKey = powerSlotToUse.numberKey;

		powerSlotToUse.powerNumberKeyText.text = powerSlotToUse.numberKey.ToString ();

		Texture powerIconTexture = currentPowerToAssign.texture;

		if (powerIconTexture != null) {
			powerSlotToUse.powerIcon.texture = powerIconTexture;
		}

		if (powersManager.getCurrentPower ().Name.Equals (currentPowerToAssign.Name)) {

			if (!powerSlotToUse.currentPowerSelectedIcon.activeSelf) {
				powerSlotToUse.currentPowerSelectedIcon.SetActive (true);
			}

			currentPowerNameText.text = powerSlotToUse.Name;
		} else {
			if (powerSlotToUse.currentPowerSelectedIcon.activeSelf) {
				powerSlotToUse.currentPowerSelectedIcon.SetActive (false);
			}
		}

		powerSlotToUse.slotActive = true;

		if (!powerSlotToUse.slot.activeSelf) {
			powerSlotToUse.slot.SetActive (true);
		}

		powerSlotToUse.slotWithPowerAssigned = slotWithPowerAssigned;

		currentPowerToAssign.powerAssigned = slotWithPowerAssigned;
	}

	public void playSound (AudioClip sound)
	{
		if (mainAudioSource != null) {
			mainAudioSource.PlayOneShot (sound);
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputEditPowerSlots ()
	{
		//if the edit power button is pressed, enable the power manager
		if (powerListManagerEnabled) {	
		
			if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
				return;
			}

			editPowersSlots ();
		}
	}

	public void inputOpenOrClosePowersWheel (bool openPowersWheel)
	{
		//if the select power button is holding, enable the powers wheel to select power
		if (powerListManagerEnabled) {
			if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
				return;
			}

			if (openPowersWheel) {
				selectPowersSlots ();
			} else {
				selectPowersSlots ();
			}
		}
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	public void setPowerListManagerEnabledState (bool state)
	{
		powerListManagerEnabled = state;
	}

	[System.Serializable]
	public class powerSlotInfo
	{
		public string Name;
		public bool slotWithPowerAssigned;
		public bool slotActive;
		public GameObject slot;
		public GameObject powerIconSlot;
		public otherPowers.Powers powerInfo;
		public Transform powerNumberKeyIcon;
		public int numberKey;
		public Text powerNumberKeyText;
		public RawImage powerIcon;
		public Texture powerTexture;
		public GameObject currentPowerSelectedIcon;
		public Transform slotCenter;

		public Coroutine sizeCoroutine;
	}
}