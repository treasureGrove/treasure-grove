using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class weaponListManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool weaponListManagerEnabled;

	public Vector2 range = new Vector2 (5f, 3f);
	public float rotationHUDSpeed = 20;

	public float timeToShowWeaponSlotsWheel = 1;

	public int numberOfWeaponSlots = 8;

	public bool useBlurUIPanel = true;

	public bool drawSelectedWeapon;

	[Space]
	[Header ("Other Settings")]
	[Space]
		
	public bool slowDownTimeWhenMenuActive;
	public float timeScaleWhenMenuActive;

	public bool selectWeaponOnMenuClose;

	public bool useSoundOnSlot;
	public AudioClip soundEffect;
	public AudioSource mainAudioSource;

	public bool changeCurrentSlotSize;
	public float changeCurrentSlotSpeed;
	public float changeCurrentSlotMultiplier;
	public float distanceFromCenterToSelectWeapon = 10;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool selectingWeapon;

	public bool usingDualWepaon;

	public bool selectingRightWeapon;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool usedByAI;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject weaponSlotsMenu;
	public GameObject weaponsListElement;
	public Transform weaponsSlotsWheel;
	public Text currentWeaponNameText;
	public GameObject completeWeaponsWheel;
	public Transform slotArrow;
	public Transform slotArrowIcon;

	public RectTransform rightHandWheelPosition;
	public RectTransform leftHandWheelPosition;

	public RectTransform dualHandIcon;

	public Camera mainCamera;

	public menuPause pauseManager;
	public playerWeaponsManager weaponsManager;
	public playerController playerControllerManager;
	public timeBullet timeBulletManager;

	List<weaponSlotInfo> weaponSlotInfoList = new List<weaponSlotInfo> ();

	weaponSlotInfo closestSlot;
	weaponSlotInfo previousClosestSlot;

	weaponSlotInfo closestRightSlot;
	weaponSlotInfo closestLeftSlot;

	float screenWidth;
	float screenHeight;
	bool touchPlatform;
	bool touching;
	Vector2 mRot = Vector2.zero;
	Quaternion mStart;

	int numberOfCurrentWeapons;

	Touch currentTouch;

	float anglePerSlot;
	float currentAngle;
	float lastTimeTouching;
	bool isFirstWeaponSelected;

	Vector3 initialTouchPosition;

	float currentArrowAngle;

	Vector2 currentTouchPosition;

	float currentDistance;

	bool fingerPressingTouchPanel;

	int weaponSlotInfoListCount;

	void Start ()
	{
		if (usedByAI) {
			return;
		}

		if (!weaponListManagerEnabled) {
			return;
		}

		//for every weapon created in the player weapons manager inspector, add to the list of the weapon manager
		for (int i = 0; i < numberOfWeaponSlots; i++) {
			GameObject newWeaponsListElement = (GameObject)Instantiate (weaponsListElement, Vector3.zero, Quaternion.identity, weaponsSlotsWheel);
			newWeaponsListElement.name = "Weapon Slot " + (i + 1);

			newWeaponsListElement.transform.localScale = Vector3.one;
			newWeaponsListElement.transform.position = weaponsListElement.transform.position;

			weaponSlotInfo newWeaponSlotInfo = newWeaponsListElement.GetComponent<weaponSlotElement> ().slotInfo;

			//add this element to the list
			weaponSlotInfoList.Add (newWeaponSlotInfo);
		}

		weaponSlotInfoListCount = weaponSlotInfoList.Count;

		weaponsListElement.SetActive (false);

		//get the rotation of the weapon wheel
		mStart = completeWeaponsWheel.transform.localRotation;

		//check if the platform is a touch device or not
		touchPlatform = touchJoystick.checkTouchPlatform ();
	}

	void Update ()
	{
		if (usedByAI) {
			return;
		}

		if (!weaponListManagerEnabled) {
			return;
		}
			
		//if the player is selecting or the touch controls are enabled, then
		if (weaponsManager.isWeaponsModeActive () && (selectingWeapon || pauseManager.isUsingTouchControls ()) && !pauseManager.isGamePaused ()) {
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
					if (!selectingWeapon) {
						if (Time.time > lastTimeTouching + timeToShowWeaponSlotsWheel) {
							selectWeaponsSlots (true);
						}
					}
				}

				if (currentTouch.phase == TouchPhase.Ended) {
					if (touching && selectingWeapon) {
						selectWeaponsSlots (false);
					}

					touching = false;

					fingerPressingTouchPanel = false;

					return;
				}

				if (selectingWeapon) {
					//get the current screen size
					screenWidth = Screen.width;
					screenHeight = Screen.height;

					//get the arrow rotates toward the mouse, selecting the closest weapon to it
					Vector2 slotDirection = new Vector2 (currentTouchPosition.x, currentTouchPosition.y) - slotArrow.GetComponent<RectTransform> ().anchoredPosition;
					Vector2 screenCenter = new Vector2 (screenWidth, screenHeight) / 2;

					slotDirection -= screenCenter;
					currentArrowAngle = Mathf.Atan2 (slotDirection.y, slotDirection.x);
					currentArrowAngle -= 90 * Mathf.Deg2Rad;
					slotArrow.localRotation = Quaternion.Euler (0, 0, currentArrowAngle * Mathf.Rad2Deg);

					if (GKC_Utils.distance (initialTouchPosition, currentTouchPosition) > distanceFromCenterToSelectWeapon) {
						if (!slotArrow.gameObject.activeSelf) {
							slotArrow.gameObject.SetActive (true);
						}

						//make the slots wheel looks toward the mouse
						float halfWidth = screenWidth * 0.5f;
						float halfHeight = screenHeight * 0.5f;
						float x = Mathf.Clamp ((currentTouchPosition.x - halfWidth) / halfWidth, -1f, 1f);
						float y = Mathf.Clamp ((currentTouchPosition.y - halfHeight) / halfHeight, -1f, 1f);

						mRot = Vector2.Lerp (mRot, new Vector2 (x, y), Time.deltaTime * rotationHUDSpeed);

						completeWeaponsWheel.transform.localRotation = mStart * Quaternion.Euler (mRot.y * range.y, -mRot.x * range.x, 0f);

						//get the weapon inside the wheel closest to the mouse
						float distance = Mathf.Infinity;

						for (int k = 0; k < weaponSlotInfoListCount; k++) {
							if (weaponSlotInfoList [k].slotActive) {
								currentDistance = GKC_Utils.distance (slotArrowIcon.position, weaponSlotInfoList [k].weaponIcon.transform.position);

								if (currentDistance < distance) {
									distance = currentDistance;
									closestSlot = weaponSlotInfoList [k];
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

							if (usingDualWepaon) {
								if (selectingRightWeapon) {
									closestRightSlot = closestSlot;
								} else {
									closestLeftSlot = closestSlot;
								}
							}

							if (changeCurrentSlotSize) {
								changeSlotSize (closestSlot, false, changeCurrentSlotMultiplier, true);
							}

							if (closestSlot.Name != "") {
								currentWeaponNameText.text = closestSlot.Name;
							}

							if (useSoundOnSlot) {
								playSound (soundEffect);
							}
						}

						//set the name of the closest weapon in the center of the weapon wheel
						if (weaponsManager.getCurrentWeaponSystem () != closestSlot.weaponSystem || !isFirstWeaponSelected || usingDualWepaon) {
							isFirstWeaponSelected = true;

							if (!selectWeaponOnMenuClose && !selectingRightWeapon) {
								selectWeapon (closestSlot);
							}
						}
					} else {
						if (slotArrow.gameObject.activeSelf) {
							slotArrow.gameObject.SetActive (false);
						}
					}
				}
			}
		}
	}

	public void setTouchingMenuPanelState (bool state)
	{
		fingerPressingTouchPanel = state;

		pauseManager.setIgnoreDisableTouchZoneListState (fingerPressingTouchPanel);
	}

	public void selectWeapon (weaponSlotInfo slotInfo)
	{
		bool dualWeaponsCanBeUsed = false;

		if (usingDualWepaon) {

			string rightWeaponName = "";
			string leftWeaponName = "";

			if (closestRightSlot != null) {
				rightWeaponName = closestRightSlot.Name;
			}

			if (closestLeftSlot != null) {
				leftWeaponName = closestLeftSlot.Name;
			}

			if (weaponsManager.checkIfWeaponsAvailable ()) {
				dualWeaponsCanBeUsed = true;

				if (rightWeaponName.Equals ("") || leftWeaponName.Equals ("")) {
					if (rightWeaponName.Equals ("")) {
						rightWeaponName = weaponsManager.getCurrentWeaponSystem ().getWeaponSystemName ();
					} else {
						leftWeaponName = weaponsManager.getCurrentWeaponSystem ().getWeaponSystemName ();
					}

					if (rightWeaponName.Equals (leftWeaponName)) {
						if (showDebugPrint) {
							print ("the player is trying to use the same weapon on both slots or hasn't selected two weapons in the weapons wheel menu");
						}

						dualWeaponsCanBeUsed = false;
					}
				}
			}

			if (dualWeaponsCanBeUsed) {

				int rightWeaponNumberKey = -1;
				int leftWeaponNumberKey = -1;

				playerWeaponSystem rightWeaponSystem = weaponsManager.getWeaponSystemByName (rightWeaponName);

				if (rightWeaponSystem != null) {
					rightWeaponNumberKey = rightWeaponSystem.getWeaponNumberKey ();
				}

				if (rightWeaponNumberKey > -1) {
					weaponsManager.removeWeaponConfiguredAsDualWeaponState (rightWeaponNumberKey);
				}

				playerWeaponSystem leftWeaponSystem = weaponsManager.getWeaponSystemByName (leftWeaponName);

				if (leftWeaponSystem != null) {
					leftWeaponNumberKey = leftWeaponSystem.getWeaponNumberKey ();
				}

				if (leftWeaponNumberKey > -1) {
					weaponsManager.removeWeaponConfiguredAsDualWeaponState (leftWeaponNumberKey);
				}

				if (rightWeaponNumberKey > -1 && leftWeaponNumberKey > -1) {
					weaponsManager.changeDualWeapons (rightWeaponName, leftWeaponName);

					weaponsManager.updateWeaponSlotInfo ();

					for (int k = 0; k < weaponSlotInfoListCount; k++) {
						if (weaponSlotInfoList [k].currentWeaponSelectedIcon.activeSelf) {
							weaponSlotInfoList [k].currentWeaponSelectedIcon.SetActive (false);
						}
					}
				}
			}
		} 

		if (!dualWeaponsCanBeUsed) {
			weaponsManager.selectWeaponByName (slotInfo.Name, drawSelectedWeapon);

			for (int k = 0; k < weaponSlotInfoListCount; k++) {
				if (slotInfo.weaponSystem == weaponSlotInfoList [k].weaponSystem) {
					if (!weaponSlotInfoList [k].currentWeaponSelectedIcon.activeSelf) {
						weaponSlotInfoList [k].currentWeaponSelectedIcon.SetActive (true);
					}
				} else {
					if (weaponSlotInfoList [k].currentWeaponSelectedIcon.activeSelf) {
						weaponSlotInfoList [k].currentWeaponSelectedIcon.SetActive (false);
					}
				}
			}
		}
	}

	public void changeSlotSize (weaponSlotInfo slotInfo, bool setRegularSize, float sizeMultiplier, bool smoothSizeChange)
	{
		if (slotInfo.sizeCoroutine != null) {
			StopCoroutine (slotInfo.sizeCoroutine);
		}

		slotInfo.sizeCoroutine = StartCoroutine (changeSlotSizeCoroutine (slotInfo, setRegularSize, sizeMultiplier, smoothSizeChange));
	}

	IEnumerator changeSlotSizeCoroutine (weaponSlotInfo slotInfo, bool setRegularSize, float sizeMultiplier, bool smoothSizeChange)
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

	//enable the weapon wheel to select the current weapon to use
	public void selectWeaponsSlots (bool state)
	{
		//check that the game is not paused, that the player is not editing the weapon, using a device and that the weapon manager can be enabled
		if ((canBeOpened () || selectingWeapon)) {

			if (!weaponsManager.checkIfWeaponsAvailable ()) {
				return;
			}

			selectingWeapon = state;

			if (!selectingWeapon) {
				if (changeCurrentSlotSize && closestSlot != null) {
					changeSlotSize (closestSlot, true, changeCurrentSlotMultiplier, false);
				}
			}

			pauseManager.openOrClosePlayerMenu (selectingWeapon, weaponSlotsMenu.transform, useBlurUIPanel);

			//enable the weapon wheel
			completeWeaponsWheel.SetActive (selectingWeapon);

			pauseManager.setIngameMenuOpenedState ("Weapon List Manager", selectingWeapon, true);
		
			pauseManager.enableOrDisablePlayerMenu (selectingWeapon, false, false);

			//reset the arrow and the wheel rotation
			completeWeaponsWheel.transform.localRotation = Quaternion.identity;

			slotArrow.localRotation = Quaternion.identity;

			if (selectingWeapon) {
				
				updateSlotsInfo ();

				if (!touchPlatform) {
					initialTouchPosition = touchJoystick.convertMouseIntoFinger ().position;
				} else {
					initialTouchPosition = Input.GetTouch (1).position;
				}

				if (slowDownTimeWhenMenuActive) {
					timeBulletManager.setBulletTimeState (true, timeScaleWhenMenuActive);
				}

				if (usingDualWepaon) {
					dualHandIcon.gameObject.SetActive (true);

					setRightOrLeftWeapon (true);
				} else {
					dualHandIcon.gameObject.SetActive (false);

					completeWeaponsWheel.GetComponent<RectTransform> ().anchoredPosition = Vector3.zero;
				}
			} else {
				isFirstWeaponSelected = false;

				if (slowDownTimeWhenMenuActive) {
					timeBulletManager.setBulletTimeState (false, 1);
				}

				if ((selectWeaponOnMenuClose || selectingRightWeapon) && closestSlot != null) {
					selectWeapon (closestSlot);
				}
			}

			closestSlot = null;
			previousClosestSlot = null;

			closestRightSlot = null;
			closestLeftSlot = null;
		}
	}

	public bool canBeOpened ()
	{
		if (pauseManager.isGamePaused ()) {
			return false;
		}

		if (playerControllerManager.isUsingDevice ()) {
			return false;
		}

		if (!weaponListManagerEnabled) {
			return false;
		}

		if (!weaponsManager.isWeaponsModeActive ()) {
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

	public void udpateSlotsInfo ()
	{
		updateSlotsInfo ();
	}

	public void updateSlotsInfo ()
	{
		numberOfCurrentWeapons = weaponsManager.getNumberOfWeaponsAvailable ();

		anglePerSlot = 360 / (float)numberOfCurrentWeapons;
		currentAngle = 0;

		for (int j = 0; j < weaponSlotInfoListCount; j++) {
			weaponSlotInfoList [j].slotActive = false;

			if (weaponSlotInfoList [j].slot.activeSelf) {
				weaponSlotInfoList [j].slot.SetActive (false);
			}
		}

		int currentSlotIndex = 0;

		bool anySlotAviable = false;

		int weaponsListCount = weaponsManager.weaponsList.Count;

		for (int i = 0; i < weaponsListCount; i++) {

			if (weaponsManager.weaponsList [i].weaponEnabled) {
				if (currentSlotIndex < weaponSlotInfoListCount) {
					weaponSlotInfo newWeaponSlotInfo = weaponSlotInfoList [currentSlotIndex];

					if (!newWeaponSlotInfo.slotActive) {
						newWeaponSlotInfo.slot.GetComponent<RectTransform> ().rotation = Quaternion.Euler (new Vector3 (0, 0, currentAngle));
				
						currentAngle += anglePerSlot;	

						playerWeaponSystem newPlayerWeaponSystem = weaponsManager.weaponsList [i].getWeaponSystemManager ();

						newWeaponSlotInfo.Name = newPlayerWeaponSystem.weaponSettings.Name;
						newWeaponSlotInfo.weaponSystem = newPlayerWeaponSystem;
						newWeaponSlotInfo.weaponAmmoText.text = newPlayerWeaponSystem.getCurrentAmmoText ();
						//newWeaponSlotInfo.weaponAmmoText.transform.rotation = Quaternion.identity;

						newWeaponSlotInfo.ammoAmountIcon.transform.rotation = Quaternion.identity;

						newWeaponSlotInfo.weaponIcon.transform.rotation = Quaternion.identity;

						Texture weaponIconTexture = newPlayerWeaponSystem.getWeaponIcon ();

						if (weaponIconTexture != null) {
							newWeaponSlotInfo.weaponIcon.texture = weaponIconTexture;
						}

						if (weaponsManager.getCurrentWeaponSystem () == newPlayerWeaponSystem) {
							if (!newWeaponSlotInfo.currentWeaponSelectedIcon.activeSelf) {
								newWeaponSlotInfo.currentWeaponSelectedIcon.SetActive (true);
							}

							currentWeaponNameText.text = newWeaponSlotInfo.Name;
						} else {
							if (newWeaponSlotInfo.currentWeaponSelectedIcon.activeSelf) {
								newWeaponSlotInfo.currentWeaponSelectedIcon.SetActive (false);
							}
						}

						newWeaponSlotInfo.slotActive = true;

						if (!newWeaponSlotInfo.slot.activeSelf) {
							newWeaponSlotInfo.slot.SetActive (true);
						}

						currentSlotIndex++;

						anySlotAviable = true;
					}
				}
			}
		}

		if (!anySlotAviable) {
			selectWeaponsSlots (false);
		}
	}

	public void playSound (AudioClip sound)
	{
		if (mainAudioSource != null) {
			mainAudioSource.PlayOneShot (sound);
		}
	}

	public void inputOpenOrCloseWeaponsWheel (bool openWeaponsWheel)
	{
		//if the select weapon button is holding, enable the weapon wheel to select weapon
		if (openWeaponsWheel) {
			selectWeaponsSlots (true);
		} else {
			selectWeaponsSlots (false);
		}
	}

	public void setSelectingWeaponState (bool state)
	{
		usingDualWepaon = state;
	}

	public void setRightOrLeftWeapon (bool state)
	{
		if (!weaponsManager.isActivateDualWeaponsEnabled ()) {
			return;
		}

		if (selectingWeapon) {
			if (weaponsManager.storePickedWeaponsOnInventory) {
				if (weaponsManager.getNumberOfWeaponsAvailable () >= 2) {
					usingDualWepaon = true;

					dualHandIcon.gameObject.SetActive (true);

					selectingRightWeapon = state;

					if (selectingRightWeapon) {
						completeWeaponsWheel.GetComponent<RectTransform> ().anchoredPosition = rightHandWheelPosition.anchoredPosition;

						dualHandIcon.localEulerAngles = new Vector3 (0, 0, 0);
					} else {
						completeWeaponsWheel.GetComponent<RectTransform> ().anchoredPosition = leftHandWheelPosition.anchoredPosition;

						dualHandIcon.localEulerAngles = new Vector3 (0, 180, 0);
					}
				}
			} else {
				print ("WARNING: the weapons need to be managed with the inventory system to use the dual weapon system, make sure to active the option Store Picked Weapons On Inventory in both" +
				"player weapons manager and inventory manager components in the player controller gameObject"); 
			}
		} 
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	public void setWeaponListManagerEnabledState (bool state)
	{
		weaponListManagerEnabled = state;
	}

	[System.Serializable]
	public class weaponSlotInfo
	{
		public string Name;
		public bool slotActive;
		public GameObject slot;
		public playerWeaponSystem weaponSystem;
		public GameObject ammoAmountIcon;
		public Text weaponAmmoText;
		public RawImage weaponIcon;
		public Texture weaponTexture;
		public GameObject currentWeaponSelectedIcon;
		public Transform slotCenter;

		public Coroutine sizeCoroutine;
	}
}