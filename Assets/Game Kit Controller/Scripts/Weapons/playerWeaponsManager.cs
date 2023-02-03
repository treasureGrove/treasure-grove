using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class playerWeaponsManager : MonoBehaviour
{
	public bool carryingWeaponInThirdPerson;
	public bool carryingWeaponInFirstPerson;
	public bool aimingInThirdPerson;
	public bool aimingInFirstPerson;
	public bool shootingSingleWeapon;

	public bool shootingRightWeapon;
	public bool shootingLeftWeapon;

	public bool reloadingWithAnimationActive;
	public float lastTimeReload;

	public int weaponsSlotsAmount;

	public bool dropCurrentWeaponWhenDie;
	public bool dropAllWeaponsWhenDie;
	public bool drawWeaponWhenResurrect;

	public bool dropWeaponsOnlyIfUsing;
	public GameObject weaponsHUD;

	public bool weaponsHUDCanBeActive = true;

	public bool weaponsHUDEnabled = true;

	public bool disableWeaponsHUDAfterDelayEnabled;
	public float delayToDisableWeaponsHUD = 3;

	public GameObject singleWeaponHUD;

	public GameObject dualWeaponHUD;

	public string currentWeaponName;

	public string currentRighWeaponName;
	public string currentLeftWeaponName;

	public Text currentWeaponNameText;
	public Text currentWeaponAmmoText;
	public Slider ammoSlider;
	public RawImage currentWeaponIcon;

	public GameObject attachmentPanel;
	public Text attachmentAmmoText;
	public RawImage currentAttachmentIcon;

	public Text currentRightWeaponAmmoText;
	public GameObject rightAttachmentPanel;
	public Text rigthAttachmentAmmoText;
	public RawImage currentRightAttachmentIcon;

	public Text currentLeftWeaponAmmoText;
	public GameObject leftAttachmentPanel;
	public Text leftAttachmentAmmoText;
	public RawImage currentLeftAttachmentIcon;

	public Transform weaponsParent;
	public Transform weaponsTransformInFirstPerson;
	public Transform weaponsTransformInThirdPerson;
	public Transform thirdPersonParent;
	public Transform firstPersonParent;
	public Transform cameraController;
	public Camera weaponsCamera;
	public string weaponsLayer;

	bool weaponsCameraLocated;

	public bool showRemainAmmoText = true;

	public float minSwipeDist;
	public bool touching;

	public bool anyWeaponAvailable;

	public List<IKWeaponSystem> weaponsList = new List<IKWeaponSystem> ();
	public IKWeaponSystem currentIKWeapon;
	public playerWeaponSystem currentWeaponSystem;

	bool currentIKWeaponLocated;

	public LayerMask targetToDamageLayer;
	public LayerMask targetForScorchLayer;

	public bool useCustomLayerToCheck;
	public LayerMask customLayerToCheck;

	public bool useCustomIgnoreTags;
	public List<string> customTagsToIgnoreList = new List<string> ();

	public bool changeToNextWeaponWhenDrop;
	public bool canDropWeapons;
	public float dropWeaponForceThirdPerson;
	public float dropWeaponForceFirstPerson;

	public bool holdDropButtonToIncreaseForce;
	public float dropIncreaseForceSpeed;
	public float maxDropForce;
	bool holdingDropButtonToIncreaseForce;
	float currentDropForce;
	float lastTimeHoldDropButton;

	public Transform rightHandTransform;
	public Transform leftHandTransform;

	public Transform rightHandMountPoint;
	public Transform leftHandMountPoint;

	public bool startGameWithCurrentWeapon;
	public bool drawInitialWeaponSelected = true;

	public bool activateDualWeaponsEnabled = true;

	public bool startGameWithDualWeapons;

	public bool usedByAI;

	public bool weaponCursorActive = true;
	public bool useWeaponCursorUnableToShootThirdPerson = true;
	public bool useWeaponCursorUnableToShootFirstPerson;

	public GameObject weaponCursor;
	public RectTransform cursorRectTransform;
	public GameObject weaponCursorRegular;
	public GameObject weaponCursorAimingInFirstPerson;
	public GameObject weaponCursorUnableToShoot;
	public GameObject weaponCustomReticle;

	public menuPause pauseManager;

	public bool startWithFirstWeaponAvailable;

	public string weaponToStartName;
	public string[] avaliableWeaponList;
	public int weaponToStartIndex;

	public string rightWeaponToStartName;
	public string leftWeaponToStartName;
	public int rightWeaponToStartIndex;
	public int leftWeaponToStartIndex;

	public float extraRotation;
	public float targetRotation;

	public bool useAimAssistInThirdPerson;
	public bool useAimAssistInFirstPerson;
	public bool useMaxDistanceToCameraCenterAimAssist;
	public float maxDistanceToCameraCenterAimAssist;

	public bool useAimAssistInLockedCamera = true;

	public float aimAssistLookAtTargetSpeed = 4;

	public bool setWeaponWhenPicked;

	public bool canGrabObjectsCarryingWeapons;

	public bool changeToNextWeaponIfAmmoEmpty;

	public List<weaponPocket> weaponPocketList = new List<weaponPocket> ();

	public bool weaponsModeActive;
	public bool drawKeepWeaponWhenModeChanged;

	public bool checkDurabilityOnObjectEnabled;

	public bool useEventOnDurabilityEmptyOnMeleeWeapon;
	public UnityEvent eventOnDurabilityEmptyOnMeleeWeapon;

	public bool storePickedWeaponsOnInventory;
	public bool drawWeaponWhenPicked;
	public bool drawPickedWeaponOnlyItNotPreviousWeaponEquipped;
	public bool changeToNextWeaponWhenUnequipped;
	public bool changeToNextWeaponWhenEquipped;
	public bool notActivateWeaponsAtStart;

	public bool canStoreAnyNumberSameWeapon = true;

	public bool useAmmoFromInventoryInAllWeapons;

	public int choosedWeapon = 0;
	public int chooseDualWeaponIndex = -1;

	public bool dropWeaponInventoryObjectsPickups;

	public bool changingWeapon;
	public bool keepingWeapon;

	public bool changingDualWeapon;
	public bool changingSingleWeapon;

	public bool canFireWeaponsWithoutAiming;
	bool aimingWeaponFromShooting;
	public bool useAimCameraOnFreeFireMode;

	public bool drawWeaponIfFireButtonPressed;

	public bool drawAndAimWeaponIfFireButtonPressed;

	public bool keepWeaponAfterDelayThirdPerson;
	public bool keepWeaponAfterDelayFirstPerson;
	public float keepWeaponDelay;
	float lastTimeWeaponUsed;

	public bool useQuickDrawWeapon;

	public bool usingFreeFireMode;
	public float timeToStopAimAfterStopFiring = 0.85f;

	public bool aimModeInputPressed;

	public bool canJumpWhileAimingThirdPerson = true;
	public bool canAimOnAirThirdPerson = true;
	public bool stopAimingOnAirThirdPerson;

	public bool ignoreParentWeaponOutsidePlayerBodyWhenNotActive;

	public GameObject weaponsMessageWindow;
	[TextArea (1, 5)] public string cantDropCurrentWeaponMessage;
	[TextArea (1, 5)] public string cantPickWeaponMessage;
	[TextArea (1, 5)] public string cantPickAttachmentMessage;
	public float weaponMessageDuration;

	public bool loadCurrentPlayerWeaponsFromSaveFile;
	public bool saveCurrentPlayerWeaponsToSaveFile;

	public bool loadWeaponAttachmentsInfoFromSaveFile;
	public bool saveWeaponAttachmentsInfoToSaveFile;

	public bool canMarkTargets;
	public playerScreenObjectivesSystem playerScreenObjectivesManager;

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	public bool fireWeaponsInputActive = true;
	public bool generalWeaponsInputActive = true;

	bool shootWeaponEnabled = true;

	public Transform temporalParentForWeapons;

	public bool useCameraShakeOnShootEnabled;

	public string mainInventoryManagerName = "Main Inventory Manager";

	public playerCamera playerCameraManager;
	public headBob headBobManager;
	public playerInputManager playerInput;
	public IKSystem IKManager;
	public playerController playerManager;
	public upperBodyRotationSystem upperBodyRotationManager;
	public headTrack headTrackManager;
	public grabObjects grabObjectsManager;
	public inventoryManager playerInventoryManager;
	public inventoryListManager mainInventoryListManager;
	public usingDevicesSystem usingDevicesManager;
	public Collider mainCollider;
	public ragdollActivator ragdollManager;
	public Transform mainCameraTransform;
	public Camera mainCamera;
	public weaponListManager mainWeaponListManager;

	public simpleSniperSightSystem mainSimpleSniperSightSystem;

	public bool pivotPointRotationActive;
	public float pivotPointRotationSpeed = 2;

	public bool runWhenAimingWeaponInThirdPerson;
	public bool stopRunIfPreviouslyNotRunning;
	public bool runAlsoWhenFiringWithoutAiming;
	bool runningPreviouslyAiming;

	public float originalWeaponsCameraFov;

	public bool removeCurrentWeaponAsNotUsableIfAmmoEmptyEnabled;

	Vector3 swipeStartPos;
	Quaternion originalWeaponsParentRotation;
	float originalFov;

	float lastTimeFired;

	bool isThirdPersonView;
	bool touchPlatform;

	Coroutine cameraFovCoroutine;
	Touch currentTouch;

	RaycastHit hit;

	public Vector3 originalWeaponsPositionThirdPerson;
	public Quaternion originalWeaponsRotationThirdPerson;
	public Vector3 originalWeaponsPositionFirstPerson;
	public Quaternion originalWeaponsRotationFirstPerson;

	bool playerIsDead;

	GameObject currentWeaponGameObject;
	Coroutine changeExtraRotation;

	bool aimWhenItIsReady;
	bool stopAimWhenItIsReady;
	bool weaponCursorRegularPreviouslyEnabled;
	bool weaponCursorAimingInFirstPersonPreviouslyEnabled;
	bool drawWhenItIsReady;
	bool keepWhenItIsReady;

	bool carryingPhysicalObject;

	//Attachments variables
	public bool openWeaponAttachmentsMenuEnabled = true;
	public bool setFirstPersonForAttachmentEditor;
	public bool useUniversalAttachments;
	public bool editingWeaponAttachments;

	weaponAttachmentSystem currentWeaponAttachmentSystem;
	weaponAttachmentSystem currentRightWeaponAttachmentSystem;
	weaponAttachmentSystem currentLeftWeaponAttachmentSystem;

	playerWeaponSystem currentWeaponSystemWithAttachment;

	public bool changeWeaponsWithNumberKeysActive = true;
	public bool changeWeaponsWithMouseWheelActive = true;
	public bool changeWeaponsWithKeysActive = true;

	bool cursorLocked = true;

	bool playerCurrentlyBusy;

	IKWeaponSystem IKWeaponToDrop;
	IKWeaponSystem currentIKWeaponBeforeCheckPockets;

	bool startInitialized;

	bool running;

	bool canMove;

	bool checkToKeepWeaponAfterAimingWeaponFromShooting;

	bool checkToKeepWeaponAfterAimingWeaponFromShooting2_5d;

	bool carryWeaponInLowerPositionActive;

	bool initialWeaponChecked;

	bool playerOnGround;

	public bool usingDualWeapon;
	public bool usingDualWeaponsPreviously;
	public IKWeaponSystem currentRightIKWeapon;
	public IKWeaponSystem currentLeftIkWeapon;

	public playerWeaponSystem currentRightWeaponSystem;
	public playerWeaponSystem currentLeftWeaponSystem;

	public float damageMultiplierStat = 1;
	public float extraDamageStat = 0;
	public float spreadMultiplierStat = 1;
	public float fireRateMultiplierStat = 1;
	public float extraReloadSpeedStat = 1;
	public int magazineExtraSizeStat = 0;

	public objectTransformData mainObjectTransformData;

	public bool ignoreCheckSurfaceCollisionThirdPerson;
	public bool ignoreCheckSurfaceCollisionFirstPerson;

	bool playerRunningWithNewFov;

	bool weaponsHUDActive;

	bool carryingSingleWeaponPreviously;
	bool carryingDualWeaponsPreviously;

	bool equippingDualWeaponsFromInventoryMenu;

	IKWeaponSystem previousSingleIKWeapon;

	IKWeaponSystem previousRightIKWeapon;
	IKWeaponSystem previousLeftIKWeapon;

	IKWeaponSystem currentIKWeaponToCheck;

	bool settingSingleWeaponFromNumberKeys;
	string singleWeaponNameToChangeFromNumberkeys;

	float movementHorizontalInput;
	float movementVerticalInput;
	float cameraHorizontalInput;
	float cameraVerticalInput;

	bool swayCanBeActive;

	bool fingerPressingTouchPanel;

	Coroutine disableWeaponHUDCoroutine;

	public bool showDebugLog;

	public bool keepLookAtTargetActiveWhenFiringWithoutAiming = true;

	bool checkAimAssistState;

	public bool useForwardDirectionOnLaserAttachments;

	public int weaponListCount;


	public bool useUsableWeaponPrefabInfoList;
	public List<usableWeaponPrefabInfo> usableWeaponPrefabInfoList = new List<usableWeaponPrefabInfo> ();

	bool autoShootOnTagActive;

	GameObject previousTargetDetectedOnAutoShootOnTag;
	GameObject currentTargetDetectedOnAutoShootOnTag;

	public bool ignoreUseDrawKeepWeaponAnimation;

	bool animatorInfoAssigned;

	Animator mainCharacterAnimator;

	Transform chest;
	Transform spine;

	Transform temporalWeaponsParent;

	bool drawWeaponsPaused;

	bool ignoreInstantiateDroppedWeaponActive;


	public bool weaponsCanBeStolenFromCharacter = true;

	public bool useEventOnWeaponStolen;
	public UnityEvent eventOnWeaponStolen;

	bool mainCameraAssigned;

	//Inspector variables
	public bool showMainSettings;
	public bool showElementSettings;
	public bool showWeaponsList;
	public bool showDebugSettings;

	void Awake ()
	{
		weaponListCount = weaponsList.Count;
	}

	public void initializePlayerWeaponsValues ()
	{
		getComponents ();

		bool isFirstPerson = isFirstPersonActive ();

		if (isFirstPerson) {
			weaponsParent.SetParent (firstPersonParent);

			weaponsParent.localRotation = originalWeaponsRotationFirstPerson;
			weaponsParent.localPosition = originalWeaponsPositionFirstPerson;
		} else {
			weaponsParent.SetParent (thirdPersonParent);

			weaponsParent.localPosition = originalWeaponsPositionThirdPerson;
			weaponsParent.localRotation = originalWeaponsRotationThirdPerson;
		}

		weaponListCount = weaponsList.Count;

		for (int k = weaponListCount - 1; k >= 0; k--) {
			if (weaponsList [k] == null) {
				weaponsList.RemoveAt (k);
			}
		}

		weaponListCount = weaponsList.Count;

		for (int k = 0; k < weaponListCount; k++) {

			Transform weaponTransform = weaponsList [k].weaponGameObject.transform;
		
			if (!isFirstPerson) {
				weaponTransform.SetParent (weaponsList [k].thirdPersonWeaponInfo.keepPosition.parent);

				weaponTransform.localPosition = weaponsList [k].thirdPersonWeaponInfo.keepPosition.localPosition;
				weaponTransform.localRotation = weaponsList [k].thirdPersonWeaponInfo.keepPosition.localRotation;
			} else {
				weaponTransform.SetParent (weaponsList [k].transform);

				weaponTransform.localPosition = weaponsList [k].firstPersonWeaponInfo.keepPosition.localPosition;
				weaponTransform.localRotation = weaponsList [k].firstPersonWeaponInfo.keepPosition.localRotation;
			}
		}

		canMove = !playerIsDead && playerManager.canPlayerMove ();

		if (storePickedWeaponsOnInventory) {
			for (int k = 0; k < weaponListCount; k++) {
				weaponsList [k].setWeaponEnabledState (false);
			}
		} else {
			if (useUsableWeaponPrefabInfoList) {
				for (int k = 0; k < usableWeaponPrefabInfoList.Count; k++) {
					instantiateWeaponInRunTime (usableWeaponPrefabInfoList [k].Name);
				}

				for (int k = 0; k < weaponListCount; k++) {
					weaponsList [k].setWeaponEnabledState (true);

					weaponsList [k].getWeaponSystemManager ().setNumberKey (k + 1);
				}
			}
		}
	}

	void Start ()
	{
		if (usedByAI) {
			initializePlayerWeaponsValues ();
		}

		bool anyWeaponEnabled = checkIfWeaponsAvailable ();

		//print (anyWeaponEnabled);

		if (anyWeaponEnabled) {
			anyWeaponAvailable = true;
			setFirstWeaponWithLowerKeyNumber ();
		} else {
			anyWeaponAvailable = checkAndSetWeaponsAvailable ();
		}

		if (anyWeaponAvailable) {
			getCurrentWeapon ();
			getCurrentWeaponRotation (currentIKWeapon);
		}

		mainCameraAssigned = mainCamera != null;

		if (!mainCameraAssigned) {
			mainCamera = playerCameraManager.getMainCamera ();

			mainCameraTransform = mainCamera.transform;

			mainCameraAssigned = true;
		}

		originalFov = mainCamera.fieldOfView;

		touchPlatform = touchJoystick.checkTouchPlatform ();

		weaponsCameraLocated = weaponsCamera != null;

		if (weaponsCameraLocated) {
			originalWeaponsCameraFov = weaponsCamera.fieldOfView;
		}

		if (weaponsSlotsAmount < 10) {
			weaponsSlotsAmount++;
		}

		if (storePickedWeaponsOnInventory) {
			if (mainInventoryListManager == null) {
				GKC_Utils.instantiateMainManagerOnSceneWithType (mainInventoryManagerName, typeof(inventoryListManager));

				mainInventoryListManager = FindObjectOfType<inventoryListManager> ();
			}

			if (!notActivateWeaponsAtStart) {
				for (int k = 0; k < weaponListCount; k++) {

					currentIKWeaponToCheck = weaponsList [k];

					if (currentIKWeaponToCheck.isWeaponEnabled ()) {
						string weaponName = currentIKWeaponToCheck.getWeaponSystemName ();

						inventoryInfo newWeaponInventoryInfo = mainInventoryListManager.getInventoryInfoFromName (weaponName);

						newWeaponInventoryInfo.amount = 1;

						playerInventoryManager.tryToPickUpObject (newWeaponInventoryInfo);
					}
				}
			}
		}

		if (!weaponCursorActive) {
			enableOrDisableGeneralWeaponCursor (false);
		}
	}

	public void enableOrDisableWeaponsCamera (bool state)
	{
		if (!weaponsCameraLocated) {
			weaponsCameraLocated = weaponsCamera != null;
		}

		if (weaponsCameraLocated) {
			if (weaponsCamera.enabled != state) {
				weaponsCamera.enabled = state;
			}
		}
	}

	void Update ()
	{
		if (!startInitialized) {
			startInitialized = true;
		}

		canMove = !playerIsDead && playerManager.canPlayerMove ();

		playerOnGround = playerManager.isPlayerOnGround ();

		if (canMove && weaponsModeActive) {

			playerCurrentlyBusy = playerIsBusy ();

			if (!playerCurrentlyBusy) {
		
				checkTypeView ();

				if (anyWeaponAvailable) {
					if ((!usingDualWeapon && currentIKWeapon.isCurrentWeapon ()) || (usingDualWeapon && (currentLeftIkWeapon.isCurrentWeapon () || currentRightIKWeapon.isCurrentWeapon ()))) {
						if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
							if ((keepWeaponAfterDelayThirdPerson && isThirdPersonView) || (keepWeaponAfterDelayFirstPerson && !isThirdPersonView)) {
								if (!isAimingWeapons () && !shootingSingleWeapon) {
									if (Time.time > keepWeaponDelay + lastTimeWeaponUsed) {
										drawOrKeepWeaponInput ();
									}
								}
							}

							if (stopAimingOnAirThirdPerson && aimingInThirdPerson && !canAimOnAirThirdPerson && !playerOnGround) {
								aimCurrentWeaponInput ();
							}

							if (usingDualWeapon) {
								if (isThirdPersonView) {
									checkForDisableFreeFireModeAfterStopFiring ();
								}
							} else {
								if (isThirdPersonView) {
									checkForDisableFreeFireModeAfterStopFiring ();
								}
							}
						}

						if (!usedByAI) {
					
							if (carryingWeaponInFirstPerson || carryingWeaponInThirdPerson) {

								//if the touch controls are enabled, activate the swipe option
								if (playerInput.isUsingTouchControls ()) {
									//select the weapon by swiping the finger in the right corner of the screen, above the weapon info
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

										//get the start position of the swipe
										if (fingerPressingTouchPanel && !touching) {
											swipeStartPos = currentTouch.position;
											touching = true;
										}

										//and the final position, and get the direction, to change to the previous or the next power
										if (currentTouch.phase == TouchPhase.Ended) {
											if (touching) {
												float swipeDistHorizontal = (new Vector3 (currentTouch.position.x, 0, 0) - new Vector3 (swipeStartPos.x, 0, 0)).magnitude;
												if (swipeDistHorizontal > minSwipeDist) {
													float swipeValue = Mathf.Sign (currentTouch.position.x - swipeStartPos.x);
													if (swipeValue > 0) {
														//right swipe
														if (usingDualWeapon) {
															keepDualWeaponAndDrawSingle (true);
														} else {
															choosePreviousWeapon (false, true);
														}
													} else if (swipeValue < 0) {
														//left swipe
														if (usingDualWeapon) {
															keepDualWeaponAndDrawSingle (false);
														} else {
															chooseNextWeapon (false, true);
														}
													}
												}
												touching = false;
											}

											fingerPressingTouchPanel = false;
										}
									}
								}
							}
						}
					}

					if (!usedByAI) {
						if (changeWeaponsWithNumberKeysActive && !reloadingWithAnimationActive && !playerInput.isUsingTouchControls ()) {
							if (!currentIKWeapon.isWeaponMoving () &&
							    !playerManager.isGamePaused () &&
							    !currentIKWeapon.isReloadingWeapon () &&
							    !weaponsAreMoving ()) {

								int currentNumberInput = playerInput.checkNumberInput (weaponsSlotsAmount);

								if (currentNumberInput > -1) {
									if (canUseInput ()) {
										if (Time.time > lastTimeReload + 0.5f) {
											for (int k = 0; k < weaponListCount; k++) {
												int keyNumberToCheck = weaponsList [k].getWeaponSystemKeyNumber ();

												if ((keyNumberToCheck == currentNumberInput) && weaponsList [k].isWeaponEnabled ()) {

													if (checkWeaponToChangeByIndex (weaponsList [k], keyNumberToCheck, currentNumberInput, k)) {
														return;
													}
												}
											}
										}
									}
								}
							}
						}
					} else {
						if (drawWhenItIsReady) {
							
							inputDrawWeapon ();

							if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
								drawWhenItIsReady = false;
							}
						}

						if (keepWhenItIsReady) {
							
							inputDrawWeapon ();

							if (!carryingWeaponInThirdPerson && !carryingWeaponInFirstPerson) {
								keepWhenItIsReady = false;
							}
						}

						if (aimWhenItIsReady) {
							inputAimWeapon ();

							if (aimingInFirstPerson || aimingInThirdPerson) {
								aimWhenItIsReady = false;
							}
						}

						if (stopAimWhenItIsReady) {
							inputAimWeapon ();

							if (!aimingInFirstPerson && !aimingInThirdPerson) {
								stopAimWhenItIsReady = false;
							}
						}
					}

					if (!initialWeaponChecked) {
						checkInitialWeapon ();
					}
				}
			} 

			initialWeaponChecked = true;
		} else if (!weaponsModeActive && anyWeaponAvailable) {
			if (!initialWeaponChecked) {
				checkInitialWeapon ();
			}

			initialWeaponChecked = true;
		}

		if (changingWeapon) {
			if (usingDualWeapon || changingDualWeapon) {
				if (!keepingWeapon) {
					keepDualWeapons ();
					keepingWeapon = true;
				}

				if (!currentRightIKWeapon.isWeaponMoving () && !currentLeftIkWeapon.isWeaponMoving () && (!carryingSingleWeaponPreviously || !currentIKWeapon.isWeaponMoving ()) &&
				    (!isThirdPersonView || !IKManager.isUsingWeapons ()) &&
				    (!carryingDualWeaponsPreviously || (!previousRightIKWeapon.isWeaponMoving () && !previousLeftIKWeapon.isWeaponMoving ()))) {

					if (currentRightIKWeapon != null) {
						currentRightIKWeapon.setDisablingDualWeaponState (false);
					}

					if (currentLeftIkWeapon != null) {
						currentLeftIkWeapon.setDisablingDualWeaponState (false);
					}

					if (currentIKWeapon != null) {
						currentIKWeapon.setDisablingDualWeaponState (false);
					}

					if (previousRightIKWeapon != null) {
						previousRightIKWeapon.setDisablingDualWeaponState (false);
					}

					weaponChanged ();

					if (showDebugLog) {
						print ("Draw dual weapons here: " + currentRightIKWeapon.getWeaponSystemName () + " " + currentLeftIkWeapon.getWeaponSystemName ());
					}

					drawDualWeapons ();

					keepingWeapon = false;

					setChangingWeaponState (false);

					changingDualWeapon = false;

					carryingSingleWeaponPreviously = false;

					carryingDualWeaponsPreviously = false;
				}
			} else {
				if (!keepingWeapon) {
					drawOrKeepWeapon (false);
					keepingWeapon = true;
				}

				if (!currentIKWeapon.isWeaponMoving () && (!carryingDualWeaponsPreviously || (!currentRightIKWeapon.isWeaponMoving () && !currentLeftIkWeapon.isWeaponMoving ())) &&
				    (!isThirdPersonView || !IKManager.isUsingWeapons ())) {

					if (currentRightIKWeapon != null) {
						currentRightIKWeapon.setDisablingDualWeaponState (false);
					}

					if (currentLeftIkWeapon != null) {
						currentLeftIkWeapon.setDisablingDualWeaponState (false);
					}

					if (currentIKWeapon != null) {
						currentIKWeapon.setDisablingDualWeaponState (false);
					}

					weaponChanged ();

					if (!playerIsDead) {
						drawOrKeepWeapon (true);
					}

					keepingWeapon = false;

					setChangingWeaponState (false);

					if (changingSingleWeapon) {
						currentRightIKWeapon = null;
						currentLeftIkWeapon = null;
						currentRightWeaponSystem = null;
						currentLeftWeaponSystem = null;

						changingSingleWeapon = false;

						carryingDualWeaponsPreviously = false;
					}
				}
			}
		}
	}

	bool changeToWeaponWithoutDoubleSelectionActive;

	public bool checkWeaponToSelectOnQuickAccessSlots (string weaponName, bool changeToWeaponWithoutDoubleSelection)
	{
		if (!reloadingWithAnimationActive &&
		    !currentIKWeapon.isWeaponMoving () &&
		    !playerManager.isGamePaused () &&
		    !currentIKWeapon.isReloadingWeapon () &&
		    !weaponsAreMoving ()) {

			if (Time.time > lastTimeReload + 0.5f) {
				for (int k = 0; k < weaponListCount; k++) {
					if (weaponsList [k].isWeaponEnabled () && weaponsList [k].getWeaponSystemName ().Equals (weaponName)) {
						int keyNumberToCheck = weaponsList [k].getWeaponSystemKeyNumber ();

						if (changeToWeaponWithoutDoubleSelection) {
							changeToWeaponWithoutDoubleSelectionActive = true;
						}

						if (checkWeaponToChangeByIndex (weaponsList [k], keyNumberToCheck, keyNumberToCheck, k)) {
							changeToWeaponWithoutDoubleSelectionActive = false;

							return true;
						}

						changeToWeaponWithoutDoubleSelectionActive = false;
					}
				}
			}
		}

		return false;
	}

	public bool checkPlayerIsNotCarringWeapons ()
	{
		if (!anyWeaponAvailable) {
			return true;
		}

		if (!reloadingWithAnimationActive &&
		    !currentIKWeapon.isWeaponMoving () &&
		    !playerManager.isGamePaused () &&
		    !currentIKWeapon.isReloadingWeapon () &&
		    !weaponsAreMoving ()) {

			return true;
		}

		return false;
	}
		
	//set the weapon sway values in first person
	void FixedUpdate ()
	{
		if (weaponsModeActive && anyWeaponAvailable) {
			if (!usedByAI && (currentIKWeapon.isCurrentWeapon () || usingDualWeapon) &&
			    isPlayerCarringWeapon () && !editingWeaponAttachments && cursorLocked &&
			    (!isThirdPersonView || !aimingInThirdPerson)) {

				setWeaponSway ();
			}

			if (usingDualWeapon) {
				if (!currentRightIKWeapon.isWeaponUpdateActive ()) {
					currentRightIKWeapon.activateWeaponUpdate ();
				}

				if (!currentLeftIkWeapon.isWeaponUpdateActive ()) {
					currentLeftIkWeapon.activateWeaponUpdate ();
				}
			} else {
				if (!currentIKWeapon.isWeaponUpdateActive ()) {
					currentIKWeapon.activateWeaponUpdate ();
				}
			}
			
			if (canMove) {
				if (!playerCurrentlyBusy) {
					if (aimingInThirdPerson || carryingWeaponInFirstPerson) {
						checkAutoShootOnTag ();

						checkAvoidShootOnTag ();

						checkIfCanMarkTargets ();

						if (autoShootOnTagActive) {
							if (usingDualWeapon) {
								shootDualWeapon (true, false, true);
							} else {
								shootWeapon (true);
							}
						}
					} else {
						if (autoShootOnTagActive) {
							resetAutoShootValues ();

							autoShootOnTagActive = false;
						}
					}
				
					if (!usingDualWeapon && currentIKWeapon.isCurrentWeapon ()) {
						if (isCameraTypeFree () && pivotPointRotationActive && carryingWeaponInThirdPerson && !checkToKeepWeaponAfterAimingWeaponFromShooting) {
							if (currentIKWeapon.thirdPersonWeaponInfo.weaponPivotPoint) {
								float currentAngle = 0;

								if ((usingFreeFireMode || checkToKeepWeaponAfterAimingWeaponFromShooting) && playerManager.isPlayerRunning () && playerManager.isPlayerMoving (0.1f)) {
									currentAngle = Vector3.SignedAngle (-currentIKWeapon.thirdPersonWeaponInfo.weaponPivotPoint.forward, transform.forward, transform.up);
								}

								currentAngle = -180 + currentAngle;

								Quaternion pivotTargetRotation = Quaternion.Euler (new Vector3 (0, currentAngle, 0));

								currentIKWeapon.thirdPersonWeaponInfo.weaponPivotPoint.localRotation =
									Quaternion.Slerp (currentIKWeapon.thirdPersonWeaponInfo.weaponPivotPoint.localRotation, 
									pivotTargetRotation, Time.deltaTime * pivotPointRotationSpeed);
							}
						}
					}
				}
			}
		}
	}

	public void checkInitialWeapon ()
	{
		updateWeaponListCount ();

		if (startGameWithCurrentWeapon) {
			if (startGameWithDualWeapons) {
				if (!startWithFirstWeaponAvailable) {
					if (avaliableWeaponList.Length > 1) {
						if (drawInitialWeaponSelected && weaponsModeActive) {
							changeDualWeapons (rightWeaponToStartName, leftWeaponToStartName);

							getCurrentWeaponRotation (currentRightIKWeapon);

							updateWeaponSlotInfo ();
						}
					}
				}
			} else {
				bool canUseWeaponSelected = false;

				if (!startWithFirstWeaponAvailable) {
					if (avaliableWeaponList.Length > 0) {
						if (storePickedWeaponsOnInventory) {
							if (checkIfWeaponEnabledByName (weaponToStartName)) {
								canUseWeaponSelected = true;
							}
						} else {
							canUseWeaponSelected = true;
						}

						if (canUseWeaponSelected) {
							setWeaponToStartGame (weaponToStartName);
						}
					}
				}

				if (canUseWeaponSelected) {
					getCurrentWeaponRotation (currentIKWeapon);

					if (drawInitialWeaponSelected && weaponsModeActive) {
						
						drawOrKeepWeapon (true);

						checkShowWeaponSlotsParentWhenWeaponSelected (currentIKWeapon.getWeaponSystemKeyNumber ());
					} else {

						updateCurrentChoosedDualWeaponIndex ();

						if (playerInventoryManager != null) {
							playerInventoryManager.updateWeaponCurrentlySelectedIcon (chooseDualWeaponIndex, true);
						}
					}
				}
			}

		} else {
			if (!initialWeaponChecked) {
				for (int k = 0; k < weaponListCount; k++) {
					if (weaponsList [k].isWeaponEnabled () && weaponsList [k].getWeaponSystemManager ().getWeaponNumberKey () == 1) {
						setWeaponByName (weaponsList [k].getWeaponSystemManager ().getWeaponSystemName ());

						getCurrentWeaponRotation (currentIKWeapon);

						updateCurrentChoosedDualWeaponIndex ();

						if (playerInventoryManager != null) {
							playerInventoryManager.updateWeaponCurrentlySelectedIcon (chooseDualWeaponIndex, true);
						}
					}
				}
			}
		}
	}

	public void setTouchingMenuPanelState (bool state)
	{
		fingerPressingTouchPanel = state;
	}

	public bool checkWeaponToChangeByIndex (IKWeaponSystem currentWeaponToCheck, int keyNumberToCheck, int weaponSlotIndex, int weaponIndex)
	{
		checkShowWeaponSlotsParentWhenWeaponSelected (weaponSlotIndex);

		if (showDebugLog) {
			print (currentWeaponToCheck.getWeaponSystemName () + " " + currentWeaponToCheck.isWeaponConfiguredAsDualWeapon ());
		}

		if (grabObjectsManager != null) {
			if (grabObjectsManager.isCarryingPhysicalObject () && !grabObjectsManager.isCarryingMeleeWeapon ()) {
				grabObjectsManager.dropObject ();
			}
		}

		if (currentWeaponToCheck.isWeaponConfiguredAsDualWeapon ()) {
			if (chooseDualWeaponIndex != keyNumberToCheck) {
				chooseDualWeaponIndex = keyNumberToCheck;

				if (isPlayerCarringWeapon ()) {

					if (showDebugLog) {
						print ("configured with dual weapon");
					}

					if (currentWeaponToCheck.usingRightHandDualWeapon) {
						changeDualWeapons (currentWeaponToCheck.getWeaponSystemName (), currentWeaponToCheck.getLinkedDualWeaponName ());
					} else {
						changeDualWeapons (currentWeaponToCheck.getLinkedDualWeaponName (), currentWeaponToCheck.getWeaponSystemName ());
					}
				}

				return true;
			} else {
				if (!isPlayerCarringWeapon ()) {

					if (showDebugLog) {
						print ("configured with dual weapon");
					}

					if (currentWeaponToCheck.usingRightHandDualWeapon) {
						changeDualWeapons (currentWeaponToCheck.getWeaponSystemName (), currentWeaponToCheck.getLinkedDualWeaponName ());
					} else {
						changeDualWeapons (currentWeaponToCheck.getLinkedDualWeaponName (), currentWeaponToCheck.getWeaponSystemName ());
					}
				}
			}
		} else {
			if (showDebugLog) {
				print ("configured as single weapon " + choosedWeapon + "  " + weaponIndex + " " + keyNumberToCheck + " " + usingDualWeapon);
			}

			if (choosedWeapon != weaponIndex || usingDualWeapon) {
				if (usingDualWeapon) {
					chooseDualWeaponIndex = keyNumberToCheck;

					if (showDebugLog) {
						print ("previously using dual weapons");
					}

					settingSingleWeaponFromNumberKeys = true;

					singleWeaponNameToChangeFromNumberkeys = currentWeaponToCheck.getWeaponSystemName ();
					changeSingleWeapon (currentWeaponToCheck.getWeaponSystemName ());

					return true;
				} else {
					if (choosedWeapon != weaponIndex) {
						chooseDualWeaponIndex = keyNumberToCheck;

						if (showDebugLog) {
							print ("previously using single weapon");
						}

						choosedWeapon = weaponIndex;

						if (currentIKWeapon != null) {
							currentIKWeapon.setCurrentWeaponState (false);
						}

						if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson || changeToWeaponWithoutDoubleSelectionActive) {
							if (useQuickDrawWeapon && carryingWeaponInThirdPerson) {
								quicChangeWeaponThirdPersonAction ();
							} else {
								setChangingWeaponState (true);
							}
						} else {
							weaponChanged ();
						}

						return true;
					}
				}
			} else {
				if (chooseDualWeaponIndex == keyNumberToCheck) {

					if (!isPlayerCarringWeapon () && !carryingWeaponInThirdPerson) {
						drawOrKeepWeaponInput ();
					}
				} else {
					chooseDualWeaponIndex = keyNumberToCheck;

					if (changeToWeaponWithoutDoubleSelectionActive) {
						if (!isPlayerCarringWeapon ()) {
							drawOrKeepWeaponInput ();
						}
					}
				}
			}
		}

		return false;
	}

	public void checkShowWeaponSlotsParentWhenWeaponSelected (int weaponSlotIndex)
	{
		if (storePickedWeaponsOnInventory && playerInventoryManager != null) {
			playerInventoryManager.showWeaponSlotsParentWhenWeaponSelected (weaponSlotIndex);
		}
	}

	public void checkForDisableFreeFireModeAfterStopFiring ()
	{
		if (carryingWeaponInThirdPerson && aimingInThirdPerson && (checkToKeepWeaponAfterAimingWeaponFromShooting || checkToKeepWeaponAfterAimingWeaponFromShooting2_5d)) {
			if (reloadingWithAnimationActive) {
				setLastTimeFired ();
			}

			if (Time.time > lastTimeFired + timeToStopAimAfterStopFiring) {

				disableFreeFireModeAfterStopFiring ();
			}
		}
	}

	public void disableFreeFireModeAfterStopFiring ()
	{
		if (headTrackManager != null) {
			headTrackManager.setOriginalCameraBodyWeightValue ();
		}

		checkToKeepWeaponAfterAimingWeaponFromShooting = false;

		checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = false;

		playerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

		setAimModeInputPressedState (false);

		aimCurrentWeaponInput ();
	}

	public void resetWeaponFiringAndAimingIfPlayerDisabled ()
	{
		if (weaponsModeActive) {
			disableFreeFireModeState ();

			if (usingDualWeapon) {
				shootDualWeapon (false, false, false);
			} else {
				shootWeapon (false);
			}
		}
	}

	public void setWeaponsJumpStartPositionState (bool state)
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon != null && currentRightIKWeapon.weaponUseJumpPositions ()) {
				currentRightIKWeapon.setPlayerOnJumpStartState (state);
			}

			if (currentLeftIkWeapon != null && currentLeftIkWeapon.weaponUseJumpPositions ()) {
				currentLeftIkWeapon.setPlayerOnJumpStartState (state);
			}
		} else {
			if (currentIKWeapon != null && currentIKWeapon.weaponUseJumpPositions ()) {
				currentIKWeapon.setPlayerOnJumpStartState (state);
			}
		}
	}

	public void setWeaponsJumpEndPositionState (bool state)
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon != null && currentRightIKWeapon.weaponUseJumpPositions ()) {
				currentRightIKWeapon.setPlayerOnJumpEndState (state);
			}

			if (currentLeftIkWeapon != null && currentLeftIkWeapon.weaponUseJumpPositions ()) {
				currentLeftIkWeapon.setPlayerOnJumpEndState (state);
			}
		} else {
			if (currentIKWeapon != null && currentIKWeapon.weaponUseJumpPositions ()) {
				currentIKWeapon.setPlayerOnJumpEndState (state);
			}
		}
	}

	public void disableWeaponJumpState ()
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.setPlayerOnJumpStartState (false);
			currentRightIKWeapon.setPlayerOnJumpEndState (false);

			currentLeftIkWeapon.setPlayerOnJumpStartState (false);
			currentLeftIkWeapon.setPlayerOnJumpEndState (false);
		} else {
			if (currentIKWeapon != null) {
				currentIKWeapon.setPlayerOnJumpStartState (false);
				currentIKWeapon.setPlayerOnJumpEndState (false);
			}
		}
	}

	public void setRunningState (bool state)
	{
		running = state;
	}

	public bool isPlayerRunning ()
	{
		return running;
	}

	public bool isPlayerCrouching ()
	{
		return playerManager.isCrouching ();
	}

	void activateCrouch ()
	{
		playerManager.crouch ();
	}

	//if the option to change camera fov is active, set the fov value according to if the player is running or not
	public void setPlayerRunningState (bool state, IKWeaponSystem IKWeaponToUse)
	{
		IKWeaponToUse.setPlayerRunningState (state);

		playerRunningWithNewFov = state;

		if (usingDualWeapon) {
			IKWeaponToUse = currentRightIKWeapon;
		}

		if (state) {
			playerCameraManager.setMainCameraFov (IKWeaponToUse.firstPersonWeaponInfo.newFovOnRun, IKWeaponToUse.firstPersonWeaponInfo.changeFovSpeed);

			if (playerCameraManager.isUsingZoom ()) {
				changeWeaponsCameraFov (true, originalWeaponsCameraFov, IKWeaponToUse.aimFovSpeed);
				playerCameraManager.disableZoom ();
			}
		} else {
			playerCameraManager.setMainCameraFov (playerCameraManager.getOriginalCameraFov (), IKWeaponToUse.firstPersonWeaponInfo.changeFovSpeed);
		}
	}

	public void disablePlayerRunningState ()
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon.isPlayerRunning ()) {
				setPlayerRunningState (false, currentRightIKWeapon);
			}
		} else {
			if (currentIKWeapon.isPlayerRunning ()) {
				setPlayerRunningState (false, currentIKWeapon);
			}
		}
	}

	public void resetPlayerRunningState ()
	{
		playerRunningWithNewFov = false;
		playerCameraManager.setMainCameraFov (playerCameraManager.getOriginalCameraFov (), playerCameraManager.zoomSpeed);
	}

	public bool isPlayerRunningWithNewFov ()
	{
		return playerRunningWithNewFov;
	}

	public void useMeleeAttack ()
	{
		if (usingDualWeapon) {
			if (!shootingRightWeapon) {
				currentRightIKWeapon.walkOrMeleeAttackWeaponPosition ();
			}

			if (!shootingLeftWeapon) {
				currentLeftIkWeapon.walkOrMeleeAttackWeaponPosition ();
			}
		} else {
			if (!shootingSingleWeapon && (carryingWeaponInFirstPerson || currentIKWeapon.isWeaponHandsOnPositionToAim ())) {
				currentIKWeapon.walkOrMeleeAttackWeaponPosition ();
			}
		}
	}

	public void checkMeleeAttackExternally ()
	{
		if (usingDualWeapon) {
			if (!shootingRightWeapon) {
				currentRightIKWeapon.checkMeleeAttackExternally ();
			}

			if (!shootingLeftWeapon) {
				currentLeftIkWeapon.checkMeleeAttackExternally ();
			}
		} else {
			if (!shootingSingleWeapon && (carryingWeaponInFirstPerson || currentIKWeapon.isWeaponHandsOnPositionToAim ())) {
				currentIKWeapon.checkMeleeAttackExternally ();
			}
		}
	}

	public bool isPlayerCarringWeapon ()
	{
		if (usingDualWeapon) {
			return currentRightIKWeapon.isPlayerCarringWeapon () && currentLeftIkWeapon.isPlayerCarringWeapon ();
		} else {
			if (currentIKWeaponLocated) {
				return currentIKWeapon.isPlayerCarringWeapon ();
			} else {
				return false;
			}
		}
	}

	public bool currentWeaponIsMoving ()
	{
		if (usingDualWeapon) {
			return currentRightIKWeapon.isWeaponMoving () || currentLeftIkWeapon.isWeaponMoving ();
		} else {
			if (currentIKWeapon != null) {
				return currentIKWeapon.isWeaponMoving ();
			} else {
				return false;
			}
		}
	}

	public bool checkIfWeaponStateIsBusy ()
	{
		if (!currentWeaponIsMoving () && isPlayerCarringWeapon ()) {
			return false;
		}

		return true;
	}

	public bool currentWeaponWithHandsInPosition ()
	{
		return currentIKWeapon.thirdPersonWeaponInfo.handsInPosition;
	}

	public bool currentDualWeaponWithHandsInPosition (bool isRightWeapon)
	{
		if (isRightWeapon) {
			return currentRightIKWeapon.thirdPersonWeaponInfo.rightHandDualWeaopnInfo.handsInPosition;
		} else {
			return currentLeftIkWeapon.thirdPersonWeaponInfo.leftHandDualWeaponInfo.handsInPosition;
		}
	}

	public bool weaponsAreMoving ()
	{
		if (changingWeapon || keepingWeapon) {
			return true;
		}

		return false;
	}

	public void checkTypeView ()
	{
		isThirdPersonView = !playerCameraManager.isFirstPersonActive ();
	}

	public bool isFirstPersonActive ()
	{
		return playerCameraManager.isFirstPersonActive ();
	}

	//shoot to any object with the tag configured in the inspector, in case the option is enabled
	public void checkAutoShootOnTag ()
	{
		if (currentWeaponSystem.weaponSettings.autoShootOnTag) {
			Vector3 raycastPosition = mainCameraTransform.position;
			Vector3 raycastDirection = mainCameraTransform.TransformDirection (Vector3.forward);

			if (Physics.Raycast (raycastPosition, raycastDirection, out hit, 
				    currentWeaponSystem.weaponSettings.maxDistanceToRaycast, currentWeaponSystem.weaponSettings.layerToAutoShoot)) {

				currentTargetDetectedOnAutoShootOnTag = hit.collider.gameObject;

				if (previousTargetDetectedOnAutoShootOnTag == null || previousTargetDetectedOnAutoShootOnTag != currentTargetDetectedOnAutoShootOnTag) {

					previousTargetDetectedOnAutoShootOnTag = currentTargetDetectedOnAutoShootOnTag;
						 
					GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);

					if (target == null) {
						target = hit.collider.gameObject;
					}

					if (currentWeaponSystem.weaponSettings.autoShootTagList.Contains (target.tag) ||
					    (currentWeaponSystem.weaponSettings.shootAtLayerToo &&
					    (1 << target.layer & currentWeaponSystem.weaponSettings.layerToAutoShoot.value) == 1 << target.layer)) {
						autoShootOnTagActive = true;

					} else {
						autoShootOnTagActive = false;
					}
				}
			} else {
				resetAutoShootValues ();
			}
		} else {
			resetAutoShootValues ();
		}
	}

	void resetAutoShootValues ()
	{
		if (autoShootOnTagActive) {
			if (usingDualWeapon) {
				shootDualWeapon (false, false, true);
			} else {
				shootWeapon (false);
			}

			previousTargetDetectedOnAutoShootOnTag = null;
			currentTargetDetectedOnAutoShootOnTag = null;

			autoShootOnTagActive = false;
		}
	}

	public bool avoidShootCurrentlyActive;
	bool avoidShootPreviouslyActive;

	public void checkAvoidShootOnTag ()
	{
		if (currentWeaponSystem.weaponSettings.avoidShootAtTag) {

			bool activateAvoidShoot = (carryingWeaponInThirdPerson && aimingInThirdPerson) || carryingWeaponInFirstPerson;

			if (activateAvoidShoot) {
				if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, 
					    currentWeaponSystem.weaponSettings.avoidShootMaxDistanceToRaycast, currentWeaponSystem.weaponSettings.layertToAvoidShoot)) {

					GameObject target = applyDamage.getCharacterOrVehicle (hit.collider.gameObject);

					if (target != null && target != gameObject) {
						if (currentWeaponSystem.weaponSettings.avoidShootTagList.Contains (target.tag)) {
							avoidShootCurrentlyActive = true;
						} else {
							avoidShootCurrentlyActive = false;
						}
					} else {
						if (currentWeaponSystem.weaponSettings.avoidShootTagList.Contains (hit.collider.gameObject.tag) ||
						    (currentWeaponSystem.weaponSettings.avoidShootAtLayerToo &&
						    (1 << hit.collider.gameObject.layer & currentWeaponSystem.weaponSettings.layerToAutoShoot.value) == 1 << hit.collider.gameObject.layer)) {

							avoidShootCurrentlyActive = true;
						} else {
							avoidShootCurrentlyActive = false;
						}
					}
				} else {
					avoidShootCurrentlyActive = false;
				}
			} else {
				avoidShootCurrentlyActive = false;
			}

			if (avoidShootCurrentlyActive != avoidShootPreviouslyActive) {
				avoidShootPreviouslyActive = avoidShootCurrentlyActive;

				shootWeaponEnabled = !avoidShootCurrentlyActive;

				if (currentWeaponSystem.weaponSettings.useLowerPositionOnAvoidShoot) {
					carryWeaponInLowerPositionActive = avoidShootCurrentlyActive;
				}
			}
		} else {
			if (avoidShootCurrentlyActive) {
				avoidShootCurrentlyActive = false;

				shootWeaponEnabled = true;
			}
		}
	}

	GameObject previousObjectToMarkTargetDetected;
	GameObject currentObjectToMarkTargetDetected;

	public void checkIfCanMarkTargets ()
	{
		if (!canMarkTargets) {
			return;
		}

		if (currentWeaponSystem.canMarkTargets) {
			if ((isThirdPersonView && currentWeaponSystem.canMarkTargetsOnThirdPerson) || (!isThirdPersonView && currentWeaponSystem.canMarkTargetsOnFirstPerson)) {
				if ((isThirdPersonView && aimingInThirdPerson) || !currentWeaponSystem.aimOnFirstPersonToMarkTarget || aimingInFirstPerson) {
					
					if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, 
						    currentWeaponSystem.maxDistanceToMarkTargets, currentWeaponSystem.markTargetsLayer)) {

						currentObjectToMarkTargetDetected = hit.collider.gameObject;

						if (previousObjectToMarkTargetDetected != currentObjectToMarkTargetDetected) {

							previousObjectToMarkTargetDetected = currentObjectToMarkTargetDetected;

							if (currentWeaponSystem.tagListToMarkTargets.Contains (hit.collider.tag)) {

								GameObject currentCharacter = applyDamage.getCharacterOrVehicle (currentObjectToMarkTargetDetected);

								if (currentCharacter != null) {
									float iconOffset = applyDamage.getCharacterHeight (currentCharacter);

									if (iconOffset < 0) {
										iconOffset = 2;
									}

									if (!playerScreenObjectivesManager.objectAlreadyOnList (currentCharacter)) {

										playerScreenObjectivesManager.addElementToPlayerList (currentCharacter, false, false, 0, true, false, 
											true, false, currentWeaponSystem.markTargetName, false, Color.white, true, -1, iconOffset, false);

										if (currentWeaponSystem.useMarkTargetSound) {
											currentWeaponSystem.playSound (currentWeaponSystem.markTargetSound);
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public void addElementToPlayerList (GameObject obj, bool addMapIcon, bool useCloseDistance, float radiusDistance, bool showOffScreen, bool showMapWindowIcon, 
	                                    bool showDistanceInfo, bool showDistanceOffScreenInfo, string objectiveIconName, bool useCustomObjectiveColor, Color objectiveColor, 
	                                    bool removeCustomObjectiveColor, int buildingIndex, float iconOffset, bool addIconOnRestOfPlayers)
	{
		playerScreenObjectivesManager.addElementToPlayerList (obj, addMapIcon, useCloseDistance, radiusDistance, showOffScreen, showMapWindowIcon, 
			showDistanceInfo, showDistanceOffScreenInfo, objectiveIconName, useCustomObjectiveColor, objectiveColor, removeCustomObjectiveColor, buildingIndex, iconOffset, addIconOnRestOfPlayers);
	}

	public void removeLocatedEnemiesIcons (GameObject objectToRemove)
	{
		playerScreenObjectivesManager.removeElementFromListByPlayer (objectToRemove);
	}

	//shoot the current weapon
	public void shootWeapon (bool state)
	{
		if (!playerCurrentlyBusy) {
			if (state) {
				if (!currentWeaponSystem.isShootingBurst ()) {

					if (currentIKWeapon.isCursorHidden ()) {
						enableOrDisableGeneralWeaponCursor (true);
						currentIKWeapon.setCursorHiddenState (false);
					}

					disablePlayerRunningState ();

					if (!currentWeaponSystem.isWeaponReloading () && currentWeaponSystem.getWeaponClipSize () > 0) {
						shootingSingleWeapon = true;
					} else {
						shootingSingleWeapon = false;
					}

					currentWeaponSystem.shootWeapon (isThirdPersonView, state);

					setLastTimeFired ();

					setLastTimeUsed ();

					checkToUpdateInventoryWeaponAmmoText ();

					currentWeaponSystem.checkWeaponAbilityHoldButton ();
				}
			} else {
				shootingSingleWeapon = false;
				
				if (currentWeaponSystem != null) {
					currentWeaponSystem.shootWeapon (isThirdPersonView, state);
				}
			}
		}
	}

	public void checkToUpdateInventoryWeaponAmmoText ()
	{
		if (currentWeaponSystem != null) {
			checkToUpdateInventoryWeaponAmmoTextByWeaponNumberKey (currentWeaponSystem.getWeaponNumberKey ());
		}
	}

	public void checkToUpdateInventoryWeaponAmmoTextByWeaponNumberKey (int currentWeaponNumberKey)
	{
		if (storePickedWeaponsOnInventory && currentWeaponSystem.weaponSettings.weaponUsesAmmo) {
			if (playerInventoryManager.isShowWeaponSlotsAlwaysActive ()) {
				playerInventoryManager.updateQuickAccesSlotAmount (currentWeaponNumberKey - 1);
			}
		}
	}

	public void shootDualWeapon (bool state, bool rightWeapon, bool shootBothWeapons)
	{
		if (!playerCurrentlyBusy) {
			if (rightWeapon || shootBothWeapons) {
				if (state) {
					if (!currentRightWeaponSystem.isShootingBurst ()) {

						if (currentRightIKWeapon.isCursorHidden ()) {
							enableOrDisableGeneralWeaponCursor (true);
							currentRightIKWeapon.setCursorHiddenState (false);
						}

						disablePlayerRunningState ();

						if (!currentRightWeaponSystem.isWeaponReloading () && currentRightWeaponSystem.getWeaponClipSize () > 0) {
							shootingRightWeapon = true;
						} else {
							shootingRightWeapon = false;
						}

						currentRightWeaponSystem.shootWeapon (isThirdPersonView, state);

						setLastTimeFired ();

						setLastTimeUsed ();

						currentRightWeaponSystem.checkWeaponAbilityHoldButton ();
					}
				} else {
					shootingRightWeapon = false;
					currentRightWeaponSystem.shootWeapon (isThirdPersonView, state);
				}
			} 

			if (!rightWeapon || shootBothWeapons) {
				if (state) {
					if (!currentLeftWeaponSystem.isShootingBurst ()) {

						if (currentLeftIkWeapon.isCursorHidden ()) {
							enableOrDisableGeneralWeaponCursor (true);
							currentLeftIkWeapon.setCursorHiddenState (false);
						}

						disablePlayerRunningState ();

						if (!currentLeftWeaponSystem.isWeaponReloading () && currentLeftWeaponSystem.getWeaponClipSize () > 0) {
							shootingLeftWeapon = true;
						} else {
							shootingLeftWeapon = false;
						}

						currentLeftWeaponSystem.shootWeapon (isThirdPersonView, state);

						setLastTimeFired ();

						setLastTimeUsed ();

						currentLeftWeaponSystem.checkWeaponAbilityHoldButton ();
					}
				} else {
					shootingLeftWeapon = false;
					currentLeftWeaponSystem.shootWeapon (isThirdPersonView, state);
				}
			}
		}
	}

	public void setShootingState (bool state)
	{
		if (usingDualWeapon) {
			shootingRightWeapon = state;
			shootingLeftWeapon = state;
		} else {
			shootingSingleWeapon = state;
		}
	}

	public bool isCharacterShooting ()
	{
		if (usingDualWeapon) {
			return shootingRightWeapon || shootingLeftWeapon;
		} else {
			return shootingSingleWeapon;
		}
	}

	// Check if the player is using a device or using a game submenu
	public bool playerIsBusy ()
	{
		if (playerManager.isUsingDevice ()) {
			return true;
		}

		if (carryingPhysicalObject) {
			return true;
		}

		if (editingWeaponAttachments) {
			return true;
		}

		if (playerManager.isUsingSubMenu ()) {
			return true;
		}

		if (playerManager.isPlayerMenuActive ()) {
			return true;
		}

		return false;
	}

	//get and set the last time when player fired a weapon
	public void setLastTimeFired ()
	{
		lastTimeFired = Time.time;

		playerManager.setLastTimeFiring ();
	}

	public void setLastTimeUsed ()
	{
		lastTimeWeaponUsed = Time.time;
	}

	public float getLastTimeFired ()
	{
		return lastTimeFired;
	}

	public void setLastTimeMoved ()
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.setLastTimeMoved ();
			currentLeftIkWeapon.setLastTimeMoved ();
		} else {
			if (currentIKWeapon != null) {
				currentIKWeapon.setLastTimeMoved ();
			}
		}
	}

	public void setGamePausedState (bool state)
	{
		setLastTimeFired ();

		setLastTimeMoved ();
	}

	public void setUsingDeviceState (bool state)
	{
		setLastTimeFired ();

		setLastTimeMoved ();
	}

	public void setReloadingWithAnimationActiveState (bool state)
	{
		reloadingWithAnimationActive = state;

		if (!state) {
			setLastTimeReload ();
		}
	}

	public bool isReloadingWithAnimationActive ()
	{
		return reloadingWithAnimationActive;
	}

	public void setLastTimeReload ()
	{
		lastTimeReload = Time.time;
	}

	public float getLastTimeReload ()
	{
		return lastTimeReload;
	}

	public bool reloadingActionNotActive ()
	{
		if (lastTimeReload > 0) {
			if (Time.time > lastTimeReload + 0.8f) {
				lastTimeReload = 0;
			} else {
				return false;
			}
		}

		return true;
	}

	public bool isActionActiveInPlayer ()
	{
		return playerManager.isActionActive ();
	}

	public void activateCustomAction (string actionName)
	{
		playerManager.activateCustomAction (actionName);
	}

	public void setHeadBobShotShakeState (IKWeaponSystem.weaponShotShakeInfo newWeaponShotShakeInfo)
	{
		headBobManager.setShotShakeState (newWeaponShotShakeInfo);
	}

	public bool isPlayerOnGroundForWeapons ()
	{
		return playerOnGround || playerManager.isLadderFound () || playerManager.isWallRunningActive ();
	}

	bool pauseCurrentWeaponSwayMovementInputActive;

	public void setPauseCurrentWeaponSwayMovementInputActiveState (bool state)
	{
		pauseCurrentWeaponSwayMovementInputActive = state;
	}

	public void setWeaponSway ()
	{
		movementHorizontalInput = playerManager.getHorizontalInput ();
		movementVerticalInput = playerManager.getVerticalInput ();
		cameraHorizontalInput = playerCameraManager.getHorizontalInput ();
		cameraVerticalInput = playerCameraManager.getVerticalInput ();

		swayCanBeActive = isPlayerOnGroundForWeapons ();

		if (pauseCurrentWeaponSwayMovementInputActive || playerManager.isObstacleToAvoidMovementFound ()) {
			movementHorizontalInput = 0;
			movementVerticalInput = 0;
		}

		if (usingDualWeapon) {
			currentRightIKWeapon.setWeaponSway (cameraHorizontalInput, cameraVerticalInput, movementVerticalInput, movementHorizontalInput, running, shootingRightWeapon, swayCanBeActive, 
				headBobManager.externalShakingActive, playerManager.isUsingDevice (), isThirdPersonView, canMove);
			
			currentLeftIkWeapon.setWeaponSway (cameraHorizontalInput, cameraVerticalInput, movementVerticalInput, movementHorizontalInput, running, shootingLeftWeapon, swayCanBeActive, 
				headBobManager.externalShakingActive, playerManager.isUsingDevice (), isThirdPersonView, canMove);
		} else {
			currentIKWeapon.setWeaponSway (cameraHorizontalInput, cameraVerticalInput, movementVerticalInput, movementHorizontalInput, running, shootingSingleWeapon, swayCanBeActive, 
				headBobManager.externalShakingActive, playerManager.isUsingDevice (), isThirdPersonView, canMove);
		}
	}

	public bool isPlayerOnGround ()
	{
		return playerOnGround;
	}

	public bool isPlayerMoving ()
	{
		return playerManager.isPlayerMoving (0);
	}

	public bool isObstacleToAvoidMovementFound ()
	{
		return playerManager.isObstacleToAvoidMovementFound ();
	}

	//in any view, draw or keep the weapon
	public void drawOrKeepWeaponInput ()
	{
		if (isThirdPersonView) {
			drawOrKeepWeapon (!carryingWeaponInThirdPerson);
		} else {
			drawOrKeepWeapon (!carryingWeaponInFirstPerson);
		}
	}

	public void drawWeaponAIInput ()
	{
		if (isThirdPersonView) {
			if (!carryingWeaponInThirdPerson) {
				drawOrKeepWeapon (true);
			}
		} else {
			if (!carryingWeaponInFirstPerson) {
				drawOrKeepWeapon (true);
			}
		}
	}

	public void keepWeaponAIInput ()
	{
		if (isThirdPersonView) {
			if (carryingWeaponInThirdPerson) {
				drawOrKeepWeapon (false);
			}
		} else {
			if (carryingWeaponInFirstPerson) {
				drawOrKeepWeapon (false);
			}
		}
	}

	public void drawOrKeepWeapon (bool state)
	{
		if (isThirdPersonView) {
			drawOrKeepWeaponThirdPerson (state);
		} else {
			drawOrKeepWeaponFirstPerson (state);

			currentWeaponSystem.setWeaponCarryState (false, state);
		}

		if (state) {
			checkIfWeaponUseAmmoFromInventory ();

			if (usingDualWeapon) {
				checkShowWeaponSlotsParentWhenWeaponSelected (currentRightWeaponSystem.getWeaponNumberKey ());
			} else {
				checkShowWeaponSlotsParentWhenWeaponSelected (currentWeaponSystem.getWeaponNumberKey ());
			}

			updateCurrentChoosedDualWeaponIndex ();

			checkToUpdateInventoryWeaponAmmoText ();
		}

		setLastTimeFired ();

		setLastTimeUsed ();

		lastTimeDrawWeapon = Time.time;
	}

	public void aimCurrentWeaponWhenItIsReady (bool state)
	{
		aimWhenItIsReady = state;

		stopAimWhenItIsReady = false;
	}

	public void stopAimCurrentWeaponWhenItIsReady (bool state)
	{
		stopAimWhenItIsReady = state;

		aimWhenItIsReady = false;
	}

	public void drawCurrentWeaponWhenItIsReady (bool state)
	{
		if (isUsingWeapons ()) {
			return;
		}

		drawWhenItIsReady = state;
	}

	public void keepCurrentWeaponWhenItIsReady (bool state)
	{
		if (!isUsingWeapons ()) {
			return;
		}

		keepWhenItIsReady = state;
	}

	//in any view, aim or draw the current weapon
	public void aimCurrentWeaponInput ()
	{
		if (isThirdPersonView) {
			aimCurrentWeapon (!aimingInThirdPerson);
		} else {
			aimCurrentWeapon (!aimingInFirstPerson);
		}
	}

	public void aimCurrentWeapon (bool state)
	{
		if (isThirdPersonView) {
			aimOrDrawWeaponThirdPerson (state);
		} else {
			aimOrDrawWeaponFirstPerson (state);
		}

		setLastTimeFired ();

		setLastTimeUsed ();
	}

	//draw or keep the weapon in third person
	public void drawOrKeepWeaponThirdPerson (bool state)
	{
		if (!canUseWeapons () && state) {
			currentWeaponSystem.setWeaponCarryState (false, false);

			return;
		}

		lockCursorAgain ();

		if (!playerManager.canCrouchWhenUsingWeaponsOnThirdPerson) {
			if (isPlayerCrouching ()) {
				activateCrouch ();
			}

			if (isPlayerCrouching ()) {
				return;
			}
		}

		if (useQuickDrawWeapon || (usingDualWeapon && currentIKWeapon.isQuickDrawKeepDualWeaponActive ())) {
			if (state) {
				quickDrawWeaponThirdPersonAction ();
			} else {
				quickKeepWeaponThirdPersonAction ();
			}

			enableOrDisableWeaponCursor (true);
		} else {
			currentWeaponSystem.setWeaponCarryState (state, false);

			carryingWeaponInThirdPerson = state;

			enableOrDisableWeaponsHUD (carryingWeaponInThirdPerson);

			if (!usingDualWeapon) {
				getCurrentWeapon ();
			}

			if (carryingWeaponInThirdPerson) {
				updateWeaponHUDInfo ();

				updateAmmo ();

				currentIKWeapon.checkWeaponSidePosition ();

				IKManager.setIKWeaponState (carryingWeaponInThirdPerson, currentIKWeapon.thirdPersonWeaponInfo, true, currentIKWeapon.getWeaponSystemName ());

				enableOrDisableWeaponCursor (true);
			} else {
				currentWeaponSystem.enableHUD (false);

				if (aimingInThirdPerson) {
					activateOrDeactivateAimMode (false);
					aimingInThirdPerson = state;

					checkPlayerCanJumpWhenAimingState ();

					enableOrDisableGrabObjectsManager (aimingInThirdPerson);
				} 

				currentWeaponSystem.setWeaponAimState (false, false);

				IKManager.setIKWeaponState (carryingWeaponInThirdPerson, currentIKWeapon.thirdPersonWeaponInfo, false, currentIKWeapon.getWeaponSystemName ());

				if (currentIKWeapon.carrying) {
					if (currentIKWeapon.thirdPersonWeaponInfo.useQuickDrawKeepWeapon && !currentIKWeapon.thirdPersonWeaponInfo.useDrawKeepWeaponAnimation) {
						weaponReadyToMoveDirectlyOnDrawHand ();
					} else {
						weaponReadyToMove ();
					}
				}

				enableOrDisableWeaponCursor (false);

				setAimAssistInThirdPersonState ();
			}
		}
	}

	public void weaponReadyToMove ()
	{
		currentIKWeapon.drawOrKeepWeaponThirdPerson (carryingWeaponInThirdPerson);
	}

	public void dualWeaponReadyToMove (bool isRightWeapon)
	{
		if (isRightWeapon) {
			if (currentRightIKWeapon != null) {
				currentRightIKWeapon.drawOrKeepWeaponThirdPerson (carryingWeaponInThirdPerson);
			}
		} else {
			if (currentLeftIkWeapon != null) {
				currentLeftIkWeapon.drawOrKeepWeaponThirdPerson (carryingWeaponInThirdPerson);
			}
		}
	}

	public void grabWeaponWithNoDrawHand ()
	{
		IKManager.setIKWeaponState (carryingWeaponInThirdPerson, currentIKWeapon.thirdPersonWeaponInfo, false, currentIKWeapon.getWeaponSystemName ());
	}

	public void weaponReadyToMoveDirectlyOnDrawHand ()
	{
		currentIKWeapon.placeWeaponDirectlyOnDrawHand (carryingWeaponInThirdPerson);
	}

	public void dualWeaponReadyToMoveDirectlyOnDrawHand (bool isRightWeapon)
	{
		if (isRightWeapon) {
			if (currentRightIKWeapon != null) {
				currentRightIKWeapon.placeWeaponDirectlyOnDrawHand (carryingWeaponInThirdPerson);
			}
		} else {
			if (currentLeftIkWeapon != null) {
				currentLeftIkWeapon.placeWeaponDirectlyOnDrawHand (carryingWeaponInThirdPerson);
			}
		}
	}

	public void enableOrDisableIKOnHands (bool state)
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.enableOrDisableIKOnHands (state);
			currentLeftIkWeapon.enableOrDisableIKOnHands (state);
		} else {
			if (currentIKWeapon != null) {
				currentIKWeapon.enableOrDisableIKOnHands (state);
			}
		}
	}

	public void checkIfStopAimIfPlayerCantGetUpFromCrouch ()
	{
		if (!playerManager.playerCanGetUpFromCrouch ()) {
			if (isAimingWeapons () && aimingInThirdPerson) {
				setAimModeInputPressedState (false);

				aimCurrentWeapon (false);
			}
		}
	}

	//aim or draw the weapon in third person
	public void aimOrDrawWeaponThirdPerson (bool state)
	{
		if (state != aimingInThirdPerson || (state && aimModeInputPressed && (checkToKeepWeaponAfterAimingWeaponFromShooting || usingFreeFireMode))) {
			if (!canUseWeapons () && state) {
				return;
			}

			if (isPlayerCrouching ()) {
				activateCrouch ();
			}

			if (isPlayerCrouching ()) {
				return;
			}

			if (aimModeInputPressed && (checkToKeepWeaponAfterAimingWeaponFromShooting || usingFreeFireMode)) {
				disableFreeFireModeState ();
			}
				
			if (usingDualWeapon) {
				if (!currentRightIKWeapon.isRightWeaponHandOnPositionToAim () || !currentLeftIkWeapon.isLeftWeaponHandOnPositionToAim ()) {
					return;
				}
			} else {
				if (!currentIKWeapon.isWeaponHandsOnPositionToAim ()) {
					return;
				}
			}
				
			aimingInThirdPerson = state;

			enableOrDisableWeaponCursor (state);

			checkPlayerCanJumpWhenAimingState ();

			enableOrDisableGrabObjectsManager (aimingInThirdPerson);

			if (usingDualWeapon) {
				currentRightIKWeapon.aimOrDrawWeaponThirdPerson (aimingInThirdPerson);
				currentRightWeaponSystem.setWeaponAimState (aimingInThirdPerson, false);
				currentRightWeaponSystem.enableHUD (aimingInThirdPerson);

				currentLeftIkWeapon.aimOrDrawWeaponThirdPerson (aimingInThirdPerson);
				currentLeftWeaponSystem.setWeaponAimState (aimingInThirdPerson, false);
				currentLeftWeaponSystem.enableHUD (aimingInThirdPerson);
			} else {
				currentIKWeapon.aimOrDrawWeaponThirdPerson (aimingInThirdPerson);
				currentWeaponSystem.setWeaponAimState (aimingInThirdPerson, false);
				currentWeaponSystem.enableHUD (aimingInThirdPerson);
			}

			if (aimingInThirdPerson) {
				activateOrDeactivateAimMode (true);
			} else {
				activateOrDeactivateAimMode (false);
			}

			setAimAssistInThirdPersonState ();

			if (!usingDualWeapon) {
				if (currentIKWeapon.useLowerRotationSpeedAimedThirdPerson) {
					if (aimingInThirdPerson) {
						playerCameraManager.changeRotationSpeedValue (currentIKWeapon.verticalRotationSpeedAimedInThirdPerson, currentIKWeapon.horizontalRotationSpeedAimedInThirdPerson);
					} else {
						playerCameraManager.setOriginalRotationSpeed ();
					}
				}
			}

			if (runWhenAimingWeaponInThirdPerson) {
				if (!aimingWeaponFromShooting || runAlsoWhenFiringWithoutAiming) {
					if (aimingInThirdPerson) {
						runningPreviouslyAiming = running;

						if (!runningPreviouslyAiming) {
							playerManager.checkIfCanRun ();
						}
					} else {
						if (stopRunIfPreviouslyNotRunning) {
							if (!runningPreviouslyAiming) {
								playerManager.stopRun ();
							}
						}
					}
				}
			}
		}
	}


	public void setAimAssistInThirdPersonState ()
	{
		if (useAimAssistInThirdPerson) {
//			print (aimingInThirdPerson + " " + aimingWeaponFromShooting + " " + aimModeInputPressed + " " + playerCameraManager.isPlayerLookingAtTarget ());

			checkAimAssistState = false;

			if (keepLookAtTargetActiveWhenFiringWithoutAiming) {
				if ((!aimingWeaponFromShooting && aimModeInputPressed) || (!aimingInThirdPerson && !playerCameraManager.isPlayerLookingAtTarget ())) {
					checkAimAssistState = true;
				}
			} else {
				checkAimAssistState = true;
			}

			if (!isCameraTypeFree ()) {
				checkAimAssistState = true;
			}

			if (checkAimAssistState) {
				if (isCameraTypeFree ()) {
					playerCameraManager.setLookAtTargetSpeedValue (aimAssistLookAtTargetSpeed);

					playerCameraManager.setLookAtTargetEnabledStateDuration (true, playerCameraManager.timeToStopAimAssist, true);
				}

				if (aimingInThirdPerson) {
					playerCameraManager.setCurrentLockedCameraCursor (cursorRectTransform);
				} else {
					playerCameraManager.setCurrentLockedCameraCursor (null);
				}

				playerCameraManager.setMaxDistanceToCameraCenter (useMaxDistanceToCameraCenterAimAssist, maxDistanceToCameraCenterAimAssist);

				if (!isCameraTypeFree ()) {
					if (useAimAssistInLockedCamera) {
						playerCameraManager.setLookAtTargetOnLockedCameraState ();
					}
				}

				playerCameraManager.setLookAtTargetState (aimingInThirdPerson, null);

				playerCameraManager.disableStrafeModeActivateFromNoTargetsFoundActive ();
			}
		}
	}

	public void setAimAssistInFirstPersonState ()
	{
		if (useAimAssistInFirstPerson) {

			checkAimAssistState = false;

			if (aimingInFirstPerson || (!aimingInFirstPerson && !playerCameraManager.isPlayerLookingAtTarget ())) {
				checkAimAssistState = true;
			}
			
			if (checkAimAssistState) {
				if (isCameraTypeFree ()) {
					playerCameraManager.setLookAtTargetSpeedValue (aimAssistLookAtTargetSpeed);

					playerCameraManager.setLookAtTargetEnabledStateDuration (true, playerCameraManager.timeToStopAimAssist, true);
				}

				playerCameraManager.setMaxDistanceToCameraCenter (useMaxDistanceToCameraCenterAimAssist, maxDistanceToCameraCenterAimAssist);
				playerCameraManager.setLookAtTargetState (aimingInFirstPerson, null);
			}
		}
	}

	//draw or keep the weapon in first person
	public void drawOrKeepWeaponFirstPerson (bool state)
	{
		if (!canUseWeapons () && state) {
			return;
		}

		enableOrDisableWeaponsCamera (false);

		enableOrDisableWeaponsCamera (true);

		lockCursorAgain ();

		carryingWeaponInFirstPerson = state;

		enableOrDisableWeaponsHUD (carryingWeaponInFirstPerson);

		if (!usingDualWeapon) {
			getCurrentWeapon ();
		}

		if (carryingWeaponInFirstPerson) {
			updateWeaponHUDInfo ();

			updateAmmo ();

			if (playerCameraManager.isUsingZoom ()) {
				changeWeaponsCameraFov (true, playerCameraManager.getMainCameraCurrentFov (), currentIKWeapon.aimFovSpeed);
			}
		} else {
			if (aimingInFirstPerson) {
				aimingInFirstPerson = state;

				currentWeaponSystem.setWeaponAimState (false, false);

				changeCameraFov (aimingInFirstPerson);
			}

			IKManager.setUsingWeaponsState (false);
		}

		currentIKWeapon.drawOrKeepWeaponFirstPerson (carryingWeaponInFirstPerson);

		enableOrDisableWeaponCursor (state);

		currentWeaponSystem.enableHUD (carryingWeaponInFirstPerson);
	}

	//aim or draw the weapon in first person
	public void aimOrDrawWeaponFirstPerson (bool state)
	{
		if (!canUseWeapons () && state) {
			return;
		}

		if (usingDualWeapon) {
			return;
		}

		if (currentIKWeapon.canAimInFirstPerson) {
			aimingInFirstPerson = state;

			//if the weapon was detecting an obstacle which has disabled the weapon cursor, enable it again when the weapon enters in aim mode
			if (aimingInFirstPerson) {
				enableOrDisableGeneralWeaponCursor (true);
			}

			enableOrDisableWeaponCursor (aimingInFirstPerson);

			currentIKWeapon.aimOrDrawWeaponFirstPerson (aimingInFirstPerson);

			if (currentWeaponSystem.weaponSettings.disableHUDInFirstPersonAim) {
				currentWeaponSystem.enableHUD (!aimingInFirstPerson);
			}

			changeCameraFov (aimingInFirstPerson);

			if (currentIKWeapon.useLowerRotationSpeedAimed) {
				if (aimingInFirstPerson) {
					playerCameraManager.changeRotationSpeedValue (currentIKWeapon.verticalRotationSpeedAimedInFirstPerson, currentIKWeapon.horizontalRotationSpeedAimedInFirstPerson);
				} else {
					playerCameraManager.setOriginalRotationSpeed ();
				}
			}

			currentWeaponSystem.setWeaponAimState (false, aimingInFirstPerson);
			playerManager.enableOrDisableAiminig (aimingInFirstPerson);

			setAimAssistInFirstPersonState ();
		}
	}

	//change the camera fov when the player aims in first person
	public void changeCameraFov (bool increaseFov)
	{
		disablePlayerRunningState ();

		playerCameraManager.disableZoom ();

		float newAimFovValue = currentIKWeapon.aimFovValue;
		float newAimFovSpeed = currentIKWeapon.aimFovSpeed;

		if (usingDualWeapon) {
			newAimFovValue = currentRightIKWeapon.aimFovValue;
			newAimFovSpeed = currentRightIKWeapon.aimFovSpeed;
		}

		if (!increaseFov) {
			newAimFovValue = originalFov;
		}

		playerCameraManager.setMainCameraFov (newAimFovValue, newAimFovSpeed);

//		changeWeaponsCameraFov (true, newAimFovValue, newAimFovSpeed);

		if (!weaponsCameraLocated) {
			weaponsCameraLocated = weaponsCamera != null;
		}

		if (weaponsCameraLocated) {
			if (weaponsCamera.fieldOfView != originalWeaponsCameraFov) {
				if (usingDualWeapon) {
					changeWeaponsCameraFov (false, originalWeaponsCameraFov, newAimFovSpeed);
				} else {
					changeWeaponsCameraFov (false, originalWeaponsCameraFov, newAimFovSpeed);
				}
			}
		}
	}

	public void changeWeaponsCameraFov (bool increaseFov, float targetFov, float speed)
	{
		stopChangeWeaponsCameraFovCoroutine ();

		cameraFovCoroutine = StartCoroutine (changeWeaponsCameraFovCoroutine (increaseFov, targetFov, speed));
	}

	void stopChangeWeaponsCameraFovCoroutine ()
	{
		if (cameraFovCoroutine != null) {
			StopCoroutine (cameraFovCoroutine);
		}
	}

	IEnumerator changeWeaponsCameraFovCoroutine (bool increaseFov, float targetFov, float speed)
	{
		float targetValue = originalWeaponsCameraFov;

		if (increaseFov) {
			targetValue = targetFov;
		}

		if (weaponsCameraLocated) {
			while (weaponsCamera.fieldOfView != targetValue) {
				weaponsCamera.fieldOfView = Mathf.MoveTowards (weaponsCamera.fieldOfView, targetValue, Time.deltaTime * speed);

				yield return null;
			}
		}
	}

	//used to change the parent of all the objects used for weapons in the place for first or third person
	public void setCurrentWeaponsParent (bool isFirstPerson)
	{
		//###################################################

		bool quickDrawWeaponThirdPerson = false;
		bool quickDrawWeaponFirstPerson = false;

		checkTypeView ();

		//if the player is activating the change to first person view, check if the player was carring a weapon in third person previously
		if (isFirstPerson) {
			//then, keep that weapon quickly, without transition
			if (carryingWeaponInThirdPerson) {
				//print ("from third to first");
				carryingWeaponInThirdPerson = false;

				if (usingDualWeapon) {
					currentRightWeaponSystem.enableHUD (false);
					currentLeftWeaponSystem.enableHUD (false);

					IKManager.setIKWeaponState (false, currentRightIKWeapon.thirdPersonWeaponInfo, false, currentRightIKWeapon.getWeaponSystemName ());
					IKManager.setIKWeaponState (false, currentLeftIkWeapon.thirdPersonWeaponInfo, false, currentLeftIkWeapon.getWeaponSystemName ());

					currentRightIKWeapon.quickKeepWeaponThirdPerson ();
					currentLeftIkWeapon.quickKeepWeaponThirdPerson ();

					currentRightWeaponSystem.setPauseDrawKeepWeaponSound ();
					currentLeftWeaponSystem.setPauseDrawKeepWeaponSound ();

					currentRightWeaponSystem.setWeaponCarryState (false, true);
					currentLeftWeaponSystem.setWeaponCarryState (false, true);
				} else {
					currentWeaponSystem.enableHUD (false);

					IKManager.setIKWeaponState (false, currentIKWeapon.thirdPersonWeaponInfo, false, currentIKWeapon.getWeaponSystemName ());
			
					currentIKWeapon.quickKeepWeaponThirdPerson ();

					currentWeaponSystem.setPauseDrawKeepWeaponSound ();

					currentWeaponSystem.setWeaponCarryState (false, true);
				}

				quickDrawWeaponFirstPerson = true;


			} else {
				if (usingDualWeapon) {
					if (currentRightIKWeapon != null && currentRightIKWeapon.isWeaponMoving ()) {
						currentRightIKWeapon.quickKeepWeaponThirdPerson ();
					}

					if (currentLeftIkWeapon != null && currentLeftIkWeapon.isWeaponMoving ()) {
						currentLeftIkWeapon.quickKeepWeaponThirdPerson ();
					}
				} else {
					//if the player was keeping his weapon while keeping his weapon, make a quick weapon keep
					if (currentIKWeapon != null && currentIKWeapon.isWeaponMoving ()) {
						currentIKWeapon.quickKeepWeaponThirdPerson ();
					}
				}
			}
		} else {

			//if the player is activating the change to third person view, check if the player was carrying a weapon in first person previously
			if (carryingWeaponInFirstPerson) {
				//print ("from first to third");
				carryingWeaponInFirstPerson = false;

				enableOrDisableWeaponsHUD (false);

				setChangingWeaponState (false);

				keepingWeapon = false;

				if (usingDualWeapon) {
					currentRightIKWeapon.quickKeepWeaponFirstPerson ();
					currentLeftIkWeapon.quickKeepWeaponFirstPerson ();

					currentRightWeaponSystem.enableHUD (false);
					currentLeftWeaponSystem.enableHUD (false);
				} else {
					currentIKWeapon.quickKeepWeaponFirstPerson ();

					currentWeaponSystem.enableHUD (false);
				}

				enableOrDisableWeaponCursor (false);

				if (usingDualWeapon) {
					currentRightWeaponSystem.setWeaponAimState (false, false);
					currentLeftWeaponSystem.setWeaponAimState (false, false);
				} else {
					currentWeaponSystem.setWeaponAimState (false, false);
				}

				if (aimingInFirstPerson) {
					aimingInFirstPerson = false;
					changeCameraFov (false);
				}

				quickDrawWeaponThirdPerson = true;

				if (usingDualWeapon) {
					currentRightWeaponSystem.setPauseDrawKeepWeaponSound ();
					currentLeftWeaponSystem.setPauseDrawKeepWeaponSound ();

					currentRightWeaponSystem.setWeaponCarryState (true, false);
					currentLeftWeaponSystem.setWeaponCarryState (true, false);
				} else {

					currentWeaponSystem.setPauseDrawKeepWeaponSound ();

					currentWeaponSystem.setWeaponCarryState (true, false);
				}
			} else {
				//if the player was keeping his weapon while keeping his weapon, make a quick weapon keep
				if (usingDualWeapon) {
					if (currentRightIKWeapon != null && currentRightIKWeapon.isWeaponMoving ()) {
						currentRightIKWeapon.quickKeepWeaponFirstPerson ();
					}

					if (currentLeftIkWeapon != null && currentLeftIkWeapon.isWeaponMoving ()) {
						currentLeftIkWeapon.quickKeepWeaponFirstPerson ();
					}
				} else {
					if (currentIKWeapon != null && currentIKWeapon.isWeaponMoving ()) {
						currentIKWeapon.quickKeepWeaponFirstPerson ();
					}
				}
			}
		}

		playerCameraManager.setOriginalRotationSpeed ();

		setWeaponsParent (isFirstPerson, false, false);

		if (quickDrawWeaponThirdPerson) {
			carryingWeaponInThirdPerson = true;

			enableOrDisableWeaponsHUD (true);

			updateWeaponHUDInfo ();

			updateAmmo ();

			if (usingDualWeapon) {
				currentRightIKWeapon.quickDrawWeaponThirdPerson ();
				currentLeftIkWeapon.quickDrawWeaponThirdPerson ();

				IKManager.setUsingDualWeaponState (true);

				IKManager.quickDrawWeaponState (currentRightIKWeapon.thirdPersonWeaponInfo);
				IKManager.quickDrawWeaponState (currentLeftIkWeapon.thirdPersonWeaponInfo);
			} else {
				currentIKWeapon.quickDrawWeaponThirdPerson ();

				IKManager.quickDrawWeaponState (currentIKWeapon.thirdPersonWeaponInfo);
			}

			enableOrDisableWeaponCursor (false);
		}

		if (usingDualWeapon) {
			if (quickDrawWeaponFirstPerson) {
				currentRightWeaponSystem.setPauseDrawKeepWeaponSound ();
				currentLeftWeaponSystem.setPauseDrawKeepWeaponSound ();

				currentRightWeaponSystem.setWeaponCarryState (false, false);
				currentLeftWeaponSystem.setWeaponCarryState (false, false);

				if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable) {
					currentRightWeaponSystem.setPauseDrawKeepWeaponSound ();
					currentLeftWeaponSystem.setPauseDrawKeepWeaponSound ();

					drawRightWeapon ();
					drawLeftWeapon ();
				}
			}
		} else {
			if (quickDrawWeaponFirstPerson) {

				currentWeaponSystem.setPauseDrawKeepWeaponSound ();

				keepingWeapon = true;

				setChangingWeaponState (true);
			}
		}

		if (usingDualWeapon) {
			if (currentRightIKWeapon != null) {
				currentRightIKWeapon.checkHandsPosition ();
			}

			if (currentLeftIkWeapon != null) {
				currentLeftIkWeapon.checkHandsPosition ();
			}
		} else {
			if (currentIKWeapon != null) {
				currentIKWeapon.checkHandsPosition ();
			}
		}

		if (quickDrawWeaponThirdPerson) {
			if (isPlayerCrouching ()) {
				if (usingDualWeapon) {
					currentRightIKWeapon.setIKPausedOnHandsActiveState (false);
					currentLeftIkWeapon.setIKPausedOnHandsActiveState (false);
				} else {
					if (currentIKWeapon != null) {
						currentIKWeapon.setIKPausedOnHandsActiveState (false);
					}
				}

				enableOrDisableIKOnHands (false);

				if (usingDualWeapon) {
					currentRightIKWeapon.adjustWeaponPositionToDeactivateIKOnHands ();
					currentLeftIkWeapon.adjustWeaponPositionToDeactivateIKOnHands ();
				} else {
					if (currentIKWeapon != null) {
						currentIKWeapon.adjustWeaponPositionToDeactivateIKOnHands ();
					}
				}
			}
		} else {
			if (usingDualWeapon) {
				currentRightIKWeapon.setIKPausedOnHandsActiveState (false);
				currentLeftIkWeapon.setIKPausedOnHandsActiveState (false);
			} else {
				if (currentIKWeapon != null) {
					currentIKWeapon.setIKPausedOnHandsActiveState (false);
				}
			}
		}
	}

	//enable or disable a weapon mesh, when he drops it or pick it
	public void enableOrDisableWeaponsMesh (bool state)
	{
		updateWeaponListCount ();

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].weaponGameObject.activeSelf != state) {
				weaponsList [k].weaponGameObject.SetActive (state);
			}
		}
	}

	public void enableOrDisableCurrentWeaponsMesh (bool state)
	{
		if (isUsingWeapons ()) {
			if (usingDualWeapon) {
				if (currentRightIKWeapon != null) {
					if (currentRightIKWeapon.weaponGameObject.activeSelf != state) {
						currentRightIKWeapon.weaponGameObject.SetActive (state);
					}
				}

				if (currentLeftIkWeapon != null) {
					if (currentLeftIkWeapon.weaponGameObject.activeSelf != state) {
						currentLeftIkWeapon.weaponGameObject.SetActive (state);
					}
				}
			} else {
				if (currentIKWeapon != null) {
					if (currentIKWeapon.weaponGameObject.activeSelf != state) {
						currentIKWeapon.weaponGameObject.SetActive (state);
					}
				}
			}
		}
	}

	public void enableOrDisableAllWeaponsMeshes (bool state)
	{
		updateWeaponListCount ();

		for (int k = 0; k < weaponListCount; k++) {
			weaponsList [k].enableOrDisableWeaponMesh (state);
		}
	}

	public void enableOrDisableEnabledWeaponsMesh (bool state)
	{
		updateWeaponListCount ();

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].isWeaponEnabled () && weaponsList [k].weaponGameObject.activeSelf != state) {
				weaponsList [k].weaponGameObject.SetActive (state);
			}
		}
	}

	public bool checkIfWeaponEnabledByName (string weaponName)
	{
		updateWeaponListCount ();

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].isWeaponEnabled () && weaponsList [k].getWeaponName ().Equals (weaponName)) {
				return true;
			}
		}

		return false;
	}

	//check if the player can draw a weapon right now
	public bool canUseWeapons ()
	{
		return weaponsModeActive;
	}

	//select next or previous weapon
	public void chooseNextWeapon (bool isDroppingWeapon, bool checkIfMoreThanOneWeaponAvailable)
	{
		if (!moreThanOneWeaponAvailable () && checkIfMoreThanOneWeaponAvailable) {
			return;
		}

		updateWeaponListCount ();

		lockCursorAgain ();

		//check the index and get the correctly weapon 
		int max = 0;

		currentIKWeapon.setCurrentWeaponState (false);

		int currentWeaponIndex = currentIKWeapon.getWeaponSystemKeyNumber ();

		currentWeaponIndex++;

		if (currentWeaponIndex > weaponsSlotsAmount) {
			currentWeaponIndex = 1;
		}

		bool exit = false;

		while (!exit) {
			for (int k = 0; k < weaponListCount; k++) {
				if (weaponsList [k].isWeaponEnabled () && weaponsList [k].getWeaponSystemKeyNumber () == currentWeaponIndex) {
					choosedWeapon = k;
					exit = true;
				}
			}

			max++;
			if (max > 100) {
				return;
			}

			//get the current weapon index
			currentWeaponIndex++;

			if (currentWeaponIndex > weaponsSlotsAmount) {
				currentWeaponIndex = 1;
			}
		}

		checkIfChangeWeapon (isDroppingWeapon);
	}

	public void choosePreviousWeapon (bool isDroppingWeapon, bool checkIfMoreThanOneWeaponAvailable)
	{
		if (!moreThanOneWeaponAvailable () && checkIfMoreThanOneWeaponAvailable) {
			return;
		}

		lockCursorAgain ();

		updateWeaponListCount ();

		int max = 0;

		currentIKWeapon.setCurrentWeaponState (false);

		int currentWeaponIndex = currentIKWeapon.getWeaponSystemKeyNumber ();

		currentWeaponIndex--;

		if (currentWeaponIndex < 1) {
			currentWeaponIndex = weaponsSlotsAmount;
		}

		bool exit = false;

		while (!exit) {
			for (int k = weaponListCount - 1; k >= 0; k--) {
				if (weaponsList [k].isWeaponEnabled () && weaponsList [k].getWeaponSystemKeyNumber () == currentWeaponIndex) {
					choosedWeapon = k;
					exit = true;
				}
			}

			max++;
			if (max > 100) {
				return;
			}

			currentWeaponIndex--;

			if (currentWeaponIndex < 1) {
				currentWeaponIndex = weaponsSlotsAmount;
			}
		}

		checkIfChangeWeapon (isDroppingWeapon);
	}

	bool usingQuickDrawWeapon;

	public void checkIfChangeWeapon (bool isDroppingWeapon)
	{
		usingQuickDrawWeapon = false;

		bool changeWeaponResult = false;

		if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson || (changeToNextWeaponWhenDrop && isDroppingWeapon)) {
			if ((changeToNextWeaponWhenDrop && isDroppingWeapon) || (!changeToNextWeaponWhenDrop && !isDroppingWeapon) || (changeToNextWeaponWhenDrop && !isDroppingWeapon)) {
				changeWeaponResult = true;
			}
		}

		if (!canMove) {
			changeWeaponResult = false;
		}

//		print (changeWeaponResult + " " + choosedWeapon + " " + weaponsList [choosedWeapon].getWeaponName ());

		//set the current weapon 
		if (changeWeaponResult) {

			if (weaponsList [choosedWeapon].isWeaponConfiguredAsDualWeapon ()) {
				checkWeaponToChangeByIndex (weaponsList [choosedWeapon], weaponsList [choosedWeapon].getWeaponSystemKeyNumber (), 
					weaponsList [choosedWeapon].getWeaponSystemKeyNumber (), choosedWeapon);
			} else {

				checkShowWeaponSlotsParentWhenWeaponSelected (weaponsList [choosedWeapon].getWeaponSystemKeyNumber ());

				if (useQuickDrawWeapon && isThirdPersonView) {
					usingQuickDrawWeapon = true;
				} else {
					setChangingWeaponState (true);
				}
			}
			//print ("changing weapon");
		} else {

			checkShowWeaponSlotsParentWhenWeaponSelected (weaponsList [choosedWeapon].getWeaponSystemKeyNumber ());

			weaponChanged ();

			//print ("weapon changed while not carrying any of them");
		}

		if (usingQuickDrawWeapon) {
			quicChangeWeaponThirdPersonAction ();
		}
	}

	//set the info of the selected weapon in the hud
	void weaponChanged ()
	{
		if (!usingDualWeapon && !changingDualWeapon) {
			weaponsList [choosedWeapon].setCurrentWeaponState (true);
		}

		getCurrentWeapon ();

		updateWeaponHUDInfo ();

		updateAmmo ();

		getCurrentWeaponRotation (weaponsList [choosedWeapon]);
	}

	public void updateWeaponHUDInfo ()
	{
		if (currentWeaponNameText != null) {
			if (usingDualWeapon || changingDualWeapon) {

				if (currentRightWeaponSystem != null) {
					currentRightWeaponAmmoText.text = currentRightWeaponSystem.getCurrentAmmoText ();
				}

				if (currentLeftWeaponSystem != null) {
					currentLeftWeaponAmmoText.text = currentLeftWeaponSystem.getCurrentAmmoText ();
				}
			} else {
				if (currentWeaponSystem != null) {
					currentWeaponNameText.text = currentWeaponSystem.getWeaponSystemName ();
					currentWeaponAmmoText.text = currentWeaponSystem.getCurrentAmmoText ();
					ammoSlider.maxValue = currentWeaponSystem.weaponSettings.ammoPerClip;
					ammoSlider.value = currentWeaponSystem.getWeaponClipSize ();
			
					updateElementsEnabledOnHUD ();
				}
			}
		}
	}

	public void updateElementsEnabledOnHUD ()
	{
		if (currentWeaponSystem.weaponSettings.showWeaponNameInHUD) {
			if (!currentWeaponNameText.gameObject.activeSelf) {
				currentWeaponNameText.gameObject.SetActive (true);
			}
		} else {
			if (currentWeaponNameText.gameObject.activeSelf) {
				currentWeaponNameText.gameObject.SetActive (false);
			}
		}

		if (currentWeaponSystem.weaponSettings.showWeaponIconInHUD) {
			if (!currentWeaponIcon.gameObject.activeSelf) {
				currentWeaponIcon.gameObject.SetActive (true);
			}

			currentWeaponIcon.texture = currentWeaponSystem.weaponSettings.weaponIConHUD;
		} else {
			if (currentWeaponIcon.gameObject.activeSelf) {
				currentWeaponIcon.gameObject.SetActive (false);
			}
		}

		if (currentWeaponSystem.weaponSettings.showWeaponAmmoSliderInHUD) {
			if (!ammoSlider.gameObject.activeSelf) {
				ammoSlider.gameObject.SetActive (true);
			}
		} else {
			if (ammoSlider.gameObject.activeSelf) {
				ammoSlider.gameObject.SetActive (false);
			}
		}

		if (currentWeaponSystem.weaponSettings.showWeaponAmmoTextInHUD) {
			if (!currentWeaponAmmoText.gameObject.activeSelf) {
				currentWeaponAmmoText.gameObject.SetActive (true);
			}
		} else {
			if (currentWeaponAmmoText.gameObject.activeSelf) {
				currentWeaponAmmoText.gameObject.SetActive (false);
			}
		}
	}

	public void updateAmmo ()
	{
		if (currentWeaponAmmoText != null) {
			if (usingDualWeapon || changingDualWeapon) {
				if (!currentRightWeaponSystem.weaponSettings.infiniteAmmo) {
					currentRightWeaponAmmoText.text = currentRightWeaponSystem.getWeaponClipSize ().ToString ();

					if (showRemainAmmoText) {
						currentRightWeaponAmmoText.text += "/" + currentRightWeaponSystem.weaponSettings.remainAmmo;
					}
				} else {
					currentRightWeaponAmmoText.text = currentRightWeaponSystem.getWeaponClipSize ().ToString ();

					if (showRemainAmmoText) {
						currentRightWeaponAmmoText.text += "/" + "Inf";
					}
				}

				if (!currentLeftWeaponSystem.weaponSettings.infiniteAmmo) {
					currentLeftWeaponAmmoText.text = currentLeftWeaponSystem.getWeaponClipSize ().ToString ();

					if (showRemainAmmoText) {
						currentLeftWeaponAmmoText.text += "/" + currentLeftWeaponSystem.weaponSettings.remainAmmo;
					}
				} else {
					currentLeftWeaponAmmoText.text = currentLeftWeaponSystem.getWeaponClipSize ().ToString ();

					if (showRemainAmmoText) {
						currentLeftWeaponAmmoText.text += "/" + "Inf";
					}
				}
			} else {
				if (currentWeaponSystem != null) {
					if (!currentWeaponSystem.weaponSettings.infiniteAmmo) {
						currentWeaponAmmoText.text = currentWeaponSystem.getWeaponClipSize ().ToString ();

						if (showRemainAmmoText) {
							currentWeaponAmmoText.text += "/" + currentWeaponSystem.weaponSettings.remainAmmo;
						}
					} else {
						currentWeaponAmmoText.text = currentWeaponSystem.getWeaponClipSize ().ToString ();

						if (showRemainAmmoText) {
							currentWeaponAmmoText.text += "/" + "Inf";
						}
					}

					ammoSlider.value = currentWeaponSystem.getWeaponClipSize ();
				}
			}
		}
	}

	public string getWeaponNameByAmmoName (string ammoName)
	{
		updateWeaponListCount ();

		string ammoNameToLower = ammoName.ToLower ();

		for (int k = 0; k < weaponListCount; k++) {

			if (showDebugLog) {
				print (weaponsList [k].getWeaponSystemAmmoName ().ToLower () + " " + ammoNameToLower);
			}

			if (weaponsList [k].getWeaponSystemAmmoName ().ToLower ().Equals (ammoNameToLower)) {
				return weaponsList [k].getWeaponSystemName ();
			}
		}

		return "";
	}

	public string getCurrentWeaponName ()
	{
		return currentWeaponName;
	}

	public bool canWeaponsBeStolenFromCharacter ()
	{
		return weaponsCanBeStolenFromCharacter;
	}

	public void setCanWeaponsBeStolenFromCharacter (bool state)
	{
		weaponsCanBeStolenFromCharacter = state;
	}

	public void checkEventOnWeaponStolen ()
	{
		if (useEventOnWeaponStolen) {
			eventOnWeaponStolen.Invoke ();
		}
	}

	public void enableOrDisableWeaponsHUD (bool state)
	{
		if (weaponsHUD != null) {
			if (currentWeaponSystem != null && currentWeaponSystem.weaponSettings.useCanvasHUD) {
				if (usingDualWeapon) {
					if (dualWeaponHUD != null) {
						if (dualWeaponHUD.activeSelf != state) {
							dualWeaponHUD.SetActive (state);
						}
					}

					if (singleWeaponHUD != null) {
						if (singleWeaponHUD.activeSelf) {
							singleWeaponHUD.SetActive (false);
						}
					}
				} else {
					if (dualWeaponHUD != null) {
						if (dualWeaponHUD.activeSelf) {
							dualWeaponHUD.SetActive (false);
						}
					}

					if (singleWeaponHUD != null) {
						if (singleWeaponHUD.activeSelf != state) {
							singleWeaponHUD.SetActive (state);
						}
					}
				}

				if (weaponsHUDCanBeActive) {
					bool newHUDState = state;

					if (!weaponsHUDEnabled) {
						newHUDState = false;
					}

					if (weaponsHUD.activeSelf != newHUDState) {
						weaponsHUD.SetActive (newHUDState);
					}

					checkDisableWeaponsHUDAfterDelay (state);
				}

				weaponsHUDActive = state;
			}
		}
	}

	public bool isWeaponsHUDActive ()
	{
		return weaponsHUDActive;
	}

	public void enableOrDisableHUD (bool state)
	{
		if (isWeaponsHUDActive ()) {
			bool newHUDState = state;

			if (!weaponsHUDEnabled) {
				newHUDState = false;
			}

			if (weaponsHUD.activeSelf != newHUDState) {
				weaponsHUD.SetActive (newHUDState);
			}

			checkDisableWeaponsHUDAfterDelay (state);
		}

		weaponsHUDCanBeActive = state;
	}

	public void checkDisableWeaponsHUDAfterDelay (bool state)
	{
		if (disableWeaponsHUDAfterDelayEnabled) {

			stopDisableWeaponsHUDAfterDelay ();
		
			if (state) {
				disableWeaponHUDCoroutine = StartCoroutine (disableWeaponsHUDAfterDelayCoroutine ());
			}
		}
	}

	public void stopDisableWeaponsHUDAfterDelay ()
	{
		if (disableWeaponHUDCoroutine != null) {
			StopCoroutine (disableWeaponHUDCoroutine);
		}
	}

	IEnumerator disableWeaponsHUDAfterDelayCoroutine ()
	{
		yield return new WaitForSeconds (delayToDisableWeaponsHUD);

		if (weaponsHUD.activeSelf) {
			weaponsHUD.SetActive (false);
		}
	}

	//get the current weapon which is being used by the player right now
	public void getCurrentWeapon ()
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {

			if (usingDualWeapon || changingDualWeapon) {
				if (weaponsList [i].getWeaponSystemName ().Equals (currentRighWeaponName)) {
//					print ("setting right weapon " + currentRighWeaponName);

					currentRightWeaponSystem = weaponsList [i].getWeaponSystemManager ();
					currentRightIKWeapon = weaponsList [i];

					if (!currentRightIKWeapon.checkAttachmentsHUD ()) {
						setRightWeaponAttachmentPanelState (false);
					}
				} 

				if (weaponsList [i].getWeaponSystemName ().Equals (currentLeftWeaponName)) {
//					print ("setting left weapon " + currentLeftWeaponName);

					currentLeftWeaponSystem = weaponsList [i].getWeaponSystemManager ();
					currentLeftIkWeapon = weaponsList [i];

					if (!currentLeftIkWeapon.checkAttachmentsHUD ()) {
						setLeftWeaponAttachmentPanelState (false);
					}
				} 
			} else {
				if (weaponsList [i].isCurrentWeapon ()) {
					currentWeaponSystem = weaponsList [i].getWeaponSystemManager ();
					currentIKWeapon = weaponsList [i];

					checkIfCurrentIkWeaponNotNull ();

					currentWeaponName = currentWeaponSystem.getWeaponSystemName ();

//					print ("setting current weapon to " + currentWeaponName);

					if (!currentIKWeapon.checkAttachmentsHUD ()) {
						setAttachmentPanelState (false);
					}
				} 
			}
		}
	}

	void checkIfCurrentIkWeaponNotNull ()
	{
		if (currentIKWeapon != null) {
			currentIKWeaponLocated = true;
		} else {
			currentIKWeaponLocated = false;
		}
	}

	public void setWeaponByElement (IKWeaponSystem weaponToConfigure)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i] == weaponToConfigure) {
				weaponsList [i].setCurrentWeaponState (true);
				choosedWeapon = i;
			} else {
				weaponsList [i].setCurrentWeaponState (false);
			}
		}

		getCurrentWeapon ();
	}

	//check if there is any weapon which can be used by the player and set as current
	public bool checkAndSetWeaponsAvailable ()
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if ((weaponsList [i].isWeaponEnabled () && !loadCurrentPlayerWeaponsFromSaveFile) ||
			    (loadCurrentPlayerWeaponsFromSaveFile && weaponsList [i].isCurrentWeapon ())) {
				weaponsList [i].setCurrentWeaponState (true);

				choosedWeapon = i;

				return true;
			}
		}

		return false;
	}

	public bool setNextWeaponAvailableToIndex (int currentIndex)
	{
		updateWeaponListCount ();

		if (currentIndex < weaponListCount) {

			int currentWeaponIndex = weaponsList [currentIndex].getWeaponSystemKeyNumber ();

			currentWeaponIndex++;
			if (currentWeaponIndex > weaponsSlotsAmount) {
				currentWeaponIndex = 1;
			}

			bool nextWeaponFound = false;

			int max = 0;
			bool exit = false;

			while (!exit) {
				for (int k = 0; k < weaponListCount; k++) {
					if (!nextWeaponFound) {
						IKWeaponSystem currentWeaponToCheck = weaponsList [k];

						if (currentWeaponToCheck.isWeaponEnabled () && currentWeaponToCheck.getWeaponSystemKeyNumber () == currentWeaponIndex) {
							choosedWeapon = k;

							nextWeaponFound = true;

							currentWeaponToCheck.setCurrentWeaponState (true);

							if (showDebugLog) {
								print ("The next weapon available is " + currentWeaponToCheck.getWeaponSystemName ());
							}
						}

						if (nextWeaponFound) {
							exit = true;
						}
					}
				}

				max++;
				if (max > 100) {
					return false;
				}

				//get the current weapon index
				currentWeaponIndex++;
				if (currentWeaponIndex > weaponsSlotsAmount) {
					currentWeaponIndex = 1;
				}
			}

			if (nextWeaponFound) {
				return true;
			}
		}
			
		return false;
	}

	public void setFirstWeaponAvailable ()
	{
		bool weaponConfigured = false;

		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].isWeaponEnabled () && !weaponConfigured) {
				weaponsList [i].setCurrentWeaponState (true);

				choosedWeapon = i;

				weaponConfigured = true;
			} else {
				weaponsList [i].setCurrentWeaponState (false);
			}
		}
	}

	public void setFirstWeaponWithLowerKeyNumber ()
	{
		bool anyWeaponFound = false;

		int lowerKeyNumber = 10000;

		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].isWeaponEnabled () && weaponsList [i].getWeaponSystemKeyNumber () < lowerKeyNumber) {
				lowerKeyNumber = weaponsList [i].getWeaponSystemKeyNumber ();

				anyWeaponFound = true;
			}
		}

		//print (lowerKeyNumber);

		if (anyWeaponFound) {
			for (int i = 0; i < weaponListCount; i++) {
				if (weaponsList [i].isWeaponEnabled () && lowerKeyNumber == weaponsList [i].getWeaponSystemKeyNumber ()) {
					weaponsList [i].setCurrentWeaponState (true);

					choosedWeapon = i;
				} else {
					weaponsList [i].setCurrentWeaponState (false);
				}
			}
		}
	}

	//check if there is any weapon which can be used by the player
	public bool checkIfWeaponsAvailable ()
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].isWeaponEnabled ()) {
				return true;
			}
		}

		return false;
	}

	public bool checkIfUsableWeaponsPrefabListActive ()
	{
		if (useUsableWeaponPrefabInfoList) {
			if (usableWeaponPrefabInfoList.Count > 0) {
				return true;
			}
		}

		return false;
	}

	//check if there is any more that one weapon which can be used, so the player doesn't change between the same weapon
	public bool moreThanOneWeaponAvailable ()
	{
		int number = 0;

		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].isWeaponEnabled ()) {
				number++;
			}
		}

		if (number > 1) {
			return true;
		} else {
			return false;
		}
	}

	//check if a weapon can be picked or is already Available to be used by the player
	public bool checkIfWeaponCanBePicked (string weaponName)
	{
		if (useUsableWeaponPrefabInfoList) {
			instantiateWeaponInRunTime (weaponName);
		}

		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			//print (weaponsList [i].getWeaponSystemName ().Equals (weaponName) && !weaponsList [i].isWeaponEnabled ());

//			print (weaponsList [i].isWeaponEnabled ());

			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName) && !weaponsList [i].isWeaponEnabled ()) {
				return true;
			}
		}

		return false;
	}

	public bool checkIfWeaponExists (string weaponName)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName)) {
				return true;
			}
		}

		return false;
	}

	public bool canStoreAnyNumberSameWeaponState ()
	{
		return canStoreAnyNumberSameWeapon;
	}

	public bool checkIfWeaponIsAvailable (string weaponName)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName) && weaponsList [i].isWeaponEnabled ()) {
				return true;
			}
		}

		return false;
	}

	public bool hasAmmoLimit (string weaponName)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName) && weaponsList [i].isWeaponEnabled () && weaponsList [i].getWeaponSystemManager ().hasAmmoLimit ()) {
				return true;
			}
		}

		return false;
	}

	public bool hasMaximumAmmoAmount (string weaponName)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName) && weaponsList [i].isWeaponEnabled () && weaponsList [i].getWeaponSystemManager ().hasMaximumAmmoAmount ()) {
				return true;
			}
		}

		return false;
	}

	public int getAmmoAmountToMaximumLimit (string weaponName)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName) && weaponsList [i].isWeaponEnabled ()) {
				return weaponsList [i].getWeaponSystemManager ().ammoAmountToMaximumLimit ();
			}
		}

		return -1;
	}

	//add ammo to a certain weapon
	public void AddAmmo (int amount, string weaponName)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName)) {
				weaponsList [i].getWeaponSystemManager ().getAmmo (amount);

				playerInventoryManager.updateQuickAccesSlotAmount (weaponsList [i].getWeaponSystemKeyNumber () - 1);

				updateAmmo ();

				return;
			}
		}
	}

	public void addAmmoToCurrentWeapon (float amount)
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.getWeaponSystemManager ().getAmmo ((int)amount);
			currentLeftIkWeapon.getWeaponSystemManager ().getAmmo ((int)amount);

			playerInventoryManager.updateQuickAccesSlotAmount (currentRightIKWeapon.getWeaponSystemKeyNumber () - 1);
			playerInventoryManager.updateQuickAccesSlotAmount (currentLeftIkWeapon.getWeaponSystemKeyNumber () - 1);

			updateAmmo ();
		} else {
			if (currentIKWeapon != null) {
				currentIKWeapon.getWeaponSystemManager ().getAmmo ((int)amount);

				playerInventoryManager.updateQuickAccesSlotAmount (currentIKWeapon.getWeaponSystemKeyNumber () - 1);

				updateAmmo ();
			}
		}
	}

	public void addAmmoForAllWeapons (float amount)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].weaponEnabled) {
				weaponsList [i].getWeaponSystemManager ().getAmmo ((int)amount);

				playerInventoryManager.updateQuickAccesSlotAmount (weaponsList [i].getWeaponSystemKeyNumber () - 1);
			}
		}

		updateAmmo ();
	}

	public void setKillInOneShootToAllWeaponsState (bool state)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			weaponsList [i].getWeaponSystemManager ().setKillOneShotState (state);
		}
	}

	public void enableOrDisableIKOnWeaponsDuringAction (bool state)
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.enableOrDisableIKOnWeaponsDuringAction (state);
			currentLeftIkWeapon.enableOrDisableIKOnWeaponsDuringAction (state);
		} else {
			if (currentIKWeapon != null) {
				currentIKWeapon.enableOrDisableIKOnWeaponsDuringAction (state);
			}
		}
	}

	public void setActionActiveState (bool state)
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.setActionActiveState (state);
			currentLeftIkWeapon.setActionActiveState (state);
		} else {
			if (currentIKWeapon != null) {
				currentIKWeapon.setActionActiveState (state);
			}
		}
	}

	public void enableAllAttachmentsInCurrentWeapon ()
	{
		if (usingDualWeapon) {
			currentRightIKWeapon.enableAllAttachments ();
			currentLeftIkWeapon.enableAllAttachments ();
		} else {
			if (currentIKWeapon != null) {
				currentIKWeapon.enableAllAttachments ();
			}
		}
	}

	public void enableWeaponByName (string weaponName)
	{
		//print (weaponName);
		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.tryToPickUpObjectByName (weaponName);
		} else {
			pickWeapon (weaponName);
		}
	}

	public Transform getRightHandTransform ()
	{
		return rightHandTransform;
	}

	public Transform getLeftHandTransform ()
	{
		return leftHandTransform;
	}

	public Transform getRightHandMountPoint ()
	{
		return rightHandMountPoint;
	}

	public Transform getLeftHandMountPoint ()
	{
		return leftHandMountPoint;
	}

	public void setVisibleToAIState (bool state)
	{
		playerManager.setVisibleToAIState (state);
	}

	public void externalForce (Vector3 direction)
	{
		playerManager.externalForce (direction);
	}

	public void checkPlayerCanJumpWhenAimingState ()
	{
		if (!canJumpWhileAimingThirdPerson) {
			playerManager.setcanJumpActiveState (!aimingInThirdPerson);
		}
	}

	void getAnimatorElements ()
	{
		if (!animatorInfoAssigned) {
			mainCharacterAnimator = transform.GetChild (0).GetComponentInChildren<Animator> ();

			if (mainCharacterAnimator == null) {
				mainCharacterAnimator = transform.GetComponent<Animator> ();
			}

			temporalWeaponsParent = thirdPersonParent;

			if (temporalWeaponsParent == null) {
				chest = mainCharacterAnimator.GetBoneTransform (HumanBodyBones.Chest);
				spine = mainCharacterAnimator.GetBoneTransform (HumanBodyBones.Spine);

				temporalWeaponsParent = chest;

				if (temporalWeaponsParent == null) {
					temporalWeaponsParent = mainCharacterAnimator.GetBoneTransform (HumanBodyBones.Head).parent.parent;

					if (temporalWeaponsParent.parent != spine) {
						temporalWeaponsParent = temporalWeaponsParent.parent;
					}
				}
			}

			animatorInfoAssigned = true;
		}
	}

	public void addWeaponToListInRunTime (GameObject weaponObject, bool showDebugInfo, bool addingOnRunTime, bool weaponEnabledState)
	{
		if (!addingOnRunTime) {
			animatorInfoAssigned = false;
		}

		getAnimatorElements ();

		if (addingOnRunTime) {
			weaponObject = (GameObject)Instantiate (weaponObject, transform.position, transform.rotation, weaponsParent);

			weaponObject.transform.localPosition = Vector3.zero;
			weaponObject.transform.localRotation = Quaternion.identity;
		}

		GameObject playerCameraGameObject = playerCameraManager.gameObject;

		IKWeaponSystem currentWeapon = weaponObject.GetComponent<IKWeaponSystem> ();

		currentWeapon.setPlayer (gameObject);

		currentWeapon.getWeaponSystemManager ().setCharacter (gameObject, playerCameraGameObject);

		currentWeapon.setHandTransform (rightHandTransform, leftHandTransform, rightHandMountPoint, leftHandMountPoint);

		currentWeapon.getWeaponSystemManager ().setWeaponParent (temporalWeaponsParent, mainCharacterAnimator);

		currentWeapon.setWeaponSystemManager ();

		currentWeapon.checkWeaponBuilder ();

		if (showDebugLog || showDebugInfo) {
			print ("Warning: Assign weapon parent into PlayerWeaponSystem inspector of the weapon: " + currentWeapon.getWeaponSystemName ());
		}

		playerWeaponSystem currentPlayerWeaponSystem = currentWeapon.getWeaponSystemManager ();

		if (currentPlayerWeaponSystem != null) {
			currentPlayerWeaponSystem.getWeaponComponents ();

			if (currentPlayerWeaponSystem.getWeaponsParent ()) {
				if (showDebugLog || showDebugInfo) {
					print (currentPlayerWeaponSystem.getWeaponsParent ().name + " Assigned by default, change it in the player weapon system inspector");
				}
			} else {
				if (showDebugLog || showDebugInfo) {
					print ("Parent not assigned or assigned by using Human Bones configuration. Check it in the player weapon system inspector if there is any issue");
				}
			}
		}

		launchTrayectory currentLaunchTrajectory = currentWeapon.weaponGameObject.GetComponentInChildren<launchTrayectory> ();

		if (currentLaunchTrajectory != null) {
			currentLaunchTrajectory.character = gameObject;
			currentLaunchTrajectory.setMainCameraTransform (playerCameraManager.getCameraTransform ());
		}

		currentWeapon.getWeaponSystemManager ().getWeaponComponents ();

		Component[] attachmentWeaponSystemList = currentWeapon.weaponGameObject.GetComponentsInChildren (typeof(weaponSystem));
		foreach (weaponSystem currentWeaponAttachment in attachmentWeaponSystemList) {
			currentWeaponAttachment.getWeaponComponents (this);

			if (showDebugLog || showDebugInfo) {
				print ("Weapon attachment " + currentWeaponAttachment.getWeaponSystemName () + " found in weapon  " + currentWeapon.getWeaponSystemName () + " updated");
			}
		}

		weaponAttachmentSystem currentWeaponAttachmentSystemToConfigure = currentWeapon.weaponGameObject.GetComponentInChildren<weaponAttachmentSystem> ();

		if (currentWeaponAttachmentSystemToConfigure != null) {
			currentWeapon.setWeaponAttachmentSystem (currentWeaponAttachmentSystemToConfigure);
		} else {
			currentWeapon.removeAttachmentSystem ();
		}

		if (showDebugLog || showDebugInfo) {
			print ("\n\n");
		}

		weaponsList.Add (currentWeapon);

		updateWeaponListCount ();

		if (addingOnRunTime) {
			bool isFirstPerson = isFirstPersonActive ();

			Transform weaponTransform = currentWeapon.weaponGameObject.transform;

			if (!isFirstPerson) {
				weaponTransform.SetParent (currentWeapon.thirdPersonWeaponInfo.keepPosition.parent);

				weaponTransform.localPosition = currentWeapon.thirdPersonWeaponInfo.keepPosition.localPosition;
				weaponTransform.localRotation = currentWeapon.thirdPersonWeaponInfo.keepPosition.localRotation;
			} else {
				weaponTransform.SetParent (currentWeapon.transform);

				weaponTransform.localPosition = currentWeapon.firstPersonWeaponInfo.keepPosition.localPosition;
				weaponTransform.localRotation = currentWeapon.firstPersonWeaponInfo.keepPosition.localRotation;
			}

			currentWeapon.setWeaponEnabledState (weaponEnabledState);

			addWeaponToPocketByPrefab (weaponObject.name, weaponObject);

			setWeaponsParent (isFirstPerson, false, addingOnRunTime);

			currentWeapon.initializeComponents ();

			currentPlayerWeaponSystem.initializeComponents ();
		} else {
			if (usedByAI) {
				for (int k = 0; k < weaponListCount; k++) {
					weaponsList [k].getWeaponSystemManager ().setNumberKey (k + 1);
				}
			}
		}
	}

	public void changeToNextWeaponWithAmmo ()
	{
		if (usingDualWeapon) {
			return;
		}

		if (removeCurrentWeaponAsNotUsableIfAmmoEmptyEnabled) {
			dropCurrentWeapon (currentIKWeapon, true, true, true, true);

			return;
		}

		updateWeaponListCount ();

		int currentWeaponIndex = currentIKWeapon.getWeaponSystemKeyNumber ();

		currentWeaponIndex++;

		if (currentWeaponIndex > weaponsSlotsAmount) {
			currentWeaponIndex = 1;
		}

		bool nextWeaponFound = false;

		int max = 0;
		bool exit = false;

		while (!exit) {
			for (int k = 0; k < weaponListCount; k++) {
				if (!nextWeaponFound) {
					IKWeaponSystem currentWeaponToCheck = weaponsList [k];

					if (currentWeaponToCheck.isWeaponEnabled () && currentWeaponToCheck.getWeaponSystemKeyNumber () == currentWeaponIndex) {
						if (currentWeaponToCheck.isWeaponConfiguredAsDualWeapon ()) {
							if (currentWeaponToCheck.weaponSystemManager.hasAnyAmmo ()) { 
								choosedWeapon = k;

								nextWeaponFound = true;

								if (showDebugLog) {
									print ("Ammo of the weapon " + currentIKWeapon.getWeaponSystemName () + " is over. Changing to dual weapon " +
									currentWeaponToCheck.getWeaponSystemName () + " and " + currentWeaponToCheck.getLinkedDualWeaponName ());
								}

							} else if (getWeaponSystemByName (currentWeaponToCheck.getLinkedDualWeaponName ()).hasAnyAmmo ()) {

								choosedWeapon = getWeaponIndexByName (currentWeaponToCheck.getLinkedDualWeaponName ());

								nextWeaponFound = true;

								if (showDebugLog) {
									print ("Ammo of the weapon " + currentIKWeapon.getWeaponSystemName () + " is over. Changing to dual weapon " +
									currentWeaponToCheck.getWeaponSystemName () + " and " + currentWeaponToCheck.getLinkedDualWeaponName ());
								}
							}
						} else {
							if (currentWeaponToCheck.weaponSystemManager.hasAnyAmmo ()) {
								choosedWeapon = k;

								nextWeaponFound = true;

								if (showDebugLog) {
									print ("Ammo of the weapon " + currentIKWeapon.getWeaponSystemName () + " is over. Changing to weapon " +
									currentWeaponToCheck.getWeaponSystemName ());
								}

							}
						}
					}

					if (nextWeaponFound) {
						exit = true;
					}
				}
			}

			max++;
			if (max > 100) {
				return;
			}

			//get the current weapon index
			currentWeaponIndex++;
			if (currentWeaponIndex > weaponsSlotsAmount) {
				currentWeaponIndex = 1;
			}
		}

		if (nextWeaponFound) {
			if (weaponsList [choosedWeapon].isWeaponConfiguredAsDualWeapon ()) {
				checkIfChangeWeapon (false);
			} else {
				currentIKWeapon.setCurrentWeaponState (false);

				if (useQuickDrawWeapon && isThirdPersonView) {
					quicChangeWeaponThirdPersonAction ();
				} else {
					setChangingWeaponState (true);
				}
			}
		}
	}

	public void addWeaponToSamePocket (string newWeaponName, string weaponToGetPocket)
	{
		print ("Adding weapon  " + newWeaponName + " to same pocket as " + weaponToGetPocket);

		bool pocketFound = false;

		int pocketGroupIndex = -1;
		int pocketIndex = -1;

		for (int j = 0; j < weaponPocketList.Count; j++) {
			if (!pocketFound) {
				for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
					for (int h = 0; h < weaponPocketList [j].weaponOnPocketList [k].weaponList.Count; h++) {
						if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h]) {
							if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h].name.Equals (weaponToGetPocket)) {
								pocketGroupIndex = j;
								pocketIndex = k;

								pocketFound = true;
							}
						}	
					}
				}
			}
		}

		if (pocketFound) {
			IKWeaponSystem newWeaponToAddIKWeaponSystem = getIKWeaponSystem (newWeaponName);

			if (newWeaponToAddIKWeaponSystem != null) {
				weaponListOnPocket currentWeaponListOnPocket = weaponPocketList [pocketGroupIndex].weaponOnPocketList [pocketIndex];

				currentWeaponListOnPocket.weaponList.Add (newWeaponToAddIKWeaponSystem.gameObject);

				print ("Weapon " + newWeaponName + " added to pocket " + currentWeaponListOnPocket.Name);
			} else {
				print ("WARNING: Weapon " + newWeaponName + " not found on weapon list");
			}
		} else {
			print ("WARNING: Pocket of weapon " + weaponToGetPocket + " hasn't been found");
		}
	}

	public void addWeaponToPocketByPrefab (string newWeaponName, GameObject newWeaponObject)
	{
		for (int j = 0; j < weaponPocketList.Count; j++) {
			for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
				for (int h = 0; h < weaponPocketList [j].weaponOnPocketList [k].weaponList.Count; h++) {
					if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h]) {
						if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h].name.Equals (newWeaponName)) {
							weaponPocketList [j].weaponOnPocketList [k].weaponList [h] = newWeaponObject;

							return;
						}
					}	
				}
			}
		}
	}

	public void removeAllWeaponsFromPocketList ()
	{
		for (int j = 0; j < weaponPocketList.Count; j++) {
			for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
				weaponPocketList [j].weaponOnPocketList [k].weaponList.Clear ();
			}
		}
	}

	public void removeAllNullWeaponsFromPocketList ()
	{
		for (int j = 0; j < weaponPocketList.Count; j++) {
			for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
				for (int h = weaponPocketList [j].weaponOnPocketList [k].weaponList.Count - 1; h >= 0; h--) {
					if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h] == null) {
						weaponPocketList [j].weaponOnPocketList [k].weaponList.RemoveAt (h);
					}
				}
			}
		}
	}

	public void selectWeaponOnListInEditor (int weaponIndex)
	{
		updateWeaponListCount ();

		if (weaponListCount > 0 && weaponListCount > weaponIndex) {
			if (weaponsList [weaponIndex] != null) {
				selectObjectInEditor (weaponsList [weaponIndex].gameObject);
			}
		}
	}

	public void selectObjectInEditor (GameObject objectToUse)
	{
		GKC_Utils.setActiveGameObjectInEditor (objectToUse);
	}

	//if the player is in aim mode, enable the upper body to rotate with the camera movement
	public void checkSetExtraRotationCoroutine (bool state)
	{
		stopSetExtraRotationCoroutine ();

		changeExtraRotation = StartCoroutine (setExtraRotation (state));
	}

	void stopSetExtraRotationCoroutine ()
	{
		if (changeExtraRotation != null) {
			StopCoroutine (changeExtraRotation);
		}
	}

	IEnumerator setExtraRotation (bool state)
	{
		if (targetRotation != 0 || (!state && extraRotation != targetRotation)) {
			for (float t = 0; t < 1;) {
				t += Time.deltaTime;
				if (state) {
					extraRotation = Mathf.Lerp (extraRotation, targetRotation, t);
				} else {
					extraRotation = Mathf.Lerp (extraRotation, 0, t);
				}
				currentWeaponGameObject.transform.localEulerAngles = new Vector3 (0, -extraRotation, 0);

				upperBodyRotationManager.setCurrentBodyRotation (extraRotation);

				yield return null;
			}
		}
	}

	public void checkDualWeaponShakeUpperBodyRotationCoroutine (float extraAngleValue, float speedValue, bool isRightWeapon)
	{
		extraAngleValue = Math.Abs (extraAngleValue);

		if (!isRightWeapon) {
			extraAngleValue = -extraAngleValue;
		}

		upperBodyRotationManager.checkShakeUpperBodyRotationCoroutine (extraAngleValue, speedValue);
	}

	public void checkShakeUpperBodyRotationCoroutine (float extraAngleValue, float speedValue)
	{
		upperBodyRotationManager.checkShakeUpperBodyRotationCoroutine (extraAngleValue, speedValue);
	}

	//get the extra rotation in the upper body of the player for the current weapon
	public void getCurrentWeaponRotation (IKWeaponSystem weapon)
	{
		currentWeaponGameObject = weapon.gameObject;

		if (usingDualWeapon || changingDualWeapon || weapon.getUsingWeaponAsOneHandWieldState ()) {
			targetRotation = 0;
		} else {
			targetRotation = weapon.extraRotation;
		}
	}

	public void resetCurrentWeaponRotation (IKWeaponSystem weapon)
	{
		weapon.gameObject.transform.localEulerAngles = new Vector3 (0, 0, 0);
	}

	public void checkUseCustomCameraStateOnAim ()
	{
		if (playerCameraManager.isUseCustomThirdPersonAimActivePaused ()) {
			return;
		}

		if (usingDualWeapon) {
			if (currentRightIKWeapon.thirdPersonWeaponInfo.useCustomThirdPersonAimActive) {
				playerCameraManager.setUseCustomThirdPersonAimActiveState (true,  
					currentRightIKWeapon.thirdPersonWeaponInfo.customDefaultThirdPersonAimRightStateName, 
					currentRightIKWeapon.thirdPersonWeaponInfo.customDefaultThirdPersonAimLeftStateName);
			}
		} else {
			if (currentIKWeapon != null && currentIKWeapon.thirdPersonWeaponInfo.useCustomThirdPersonAimActive) {
				playerCameraManager.setUseCustomThirdPersonAimActiveState (true,  
					currentIKWeapon.thirdPersonWeaponInfo.customDefaultThirdPersonAimRightStateName, 
					currentIKWeapon.thirdPersonWeaponInfo.customDefaultThirdPersonAimLeftStateName);
			}
		}
	}

	//used in third person to enable or disable the player's spine rotation
	public void activateOrDeactivateAimMode (bool state)
	{
		if (state) {
			if (!canFireWeaponsWithoutAiming || !aimingWeaponFromShooting || (canFireWeaponsWithoutAiming && useAimCameraOnFreeFireMode)) {
				checkUseCustomCameraStateOnAim ();

				playerCameraManager.activateAiming (); 	
			}

			playerManager.enableOrDisableAiminig (true);	
		} else {
			playerCameraManager.deactivateAiming ();
			playerManager.enableOrDisableAiminig (false);
		}

		if (!isFirstPersonActive ()) {
			if (usingDualWeapon) {
				if (currentRightIKWeapon.thirdPersonWeaponInfo.useWeaponRotationPoint) {
					upperBodyRotationManager.setCurrentRightWeaponRotationPoint (currentRightIKWeapon.thirdPersonWeaponInfo.weaponRotationPoint,
						currentRightIKWeapon.thirdPersonWeaponInfo.rotationPointInfo, 1);
				}

				if (currentLeftIkWeapon.thirdPersonWeaponInfo.useWeaponRotationPoint) {
					upperBodyRotationManager.setCurrentLeftWeaponRotationPoint (currentLeftIkWeapon.thirdPersonWeaponInfo.weaponRotationPoint,
						currentLeftIkWeapon.thirdPersonWeaponInfo.rotationPointInfo, 1);
				}

				upperBodyRotationManager.setUsingDualWeaponState (true);
			} else {
				if (currentIKWeapon.thirdPersonWeaponInfo.useWeaponRotationPoint) {
					upperBodyRotationManager.setCurrentWeaponRotationPoint (currentIKWeapon.thirdPersonWeaponInfo.weaponRotationPoint, 
						currentIKWeapon.thirdPersonWeaponInfo.rotationPointInfo, 1);
				}

				upperBodyRotationManager.setUsingDualWeaponState (false);
			}

			upperBodyRotationManager.setUsingWeaponRotationPointState (true);
		}
			
		if (!pauseUpperBodyRotationSystemActive) {
			checkSetExtraRotationCoroutine (state);

			upperBodyRotationManager.enableOrDisableIKUpperBody (state);

			upperBodyRotationManager.setCurrentBodyRotation (extraRotation);
		}
	}

	bool pauseUpperBodyRotationSystemActive;

	public void setPauseUpperBodyRotationSystemActiveState (bool state)
	{
		pauseUpperBodyRotationSystemActive = state;
	}

	bool pauseRecoilOnWeaponActive;

	public void setPauseRecoilOnWeaponActiveState (bool state)
	{
		pauseRecoilOnWeaponActive = state;
	}

	public bool isPauseRecoilOnWeaponActive ()
	{
		return pauseRecoilOnWeaponActive;
	}

	bool pauseWeaponReloadActive;

	public void setPauseWeaponReloadActiveState (bool state)
	{
		pauseWeaponReloadActive = state;
	}

	public bool isPauseWeaponReloadActive ()
	{
		return pauseWeaponReloadActive;
	}

	public void setPauseWeaponAimMovementActiveState (bool state)
	{
		pauseWeaponAimMovementActive = state;
	}

	bool pauseWeaponAimMovementActive;

	public bool isPauseWeaponAimMovementActive ()
	{
		return pauseWeaponAimMovementActive;
	}

	//enable or disable the UI cursor used in weapons
	public void enableOrDisableWeaponCursor (bool value)
	{
		if (!weaponCursorActive) {
			checkUpdateReticleActiveState (false);

			return;
		}

		if (weaponCursorRegular != null) {
			bool useReticleOnAimingEnabled = currentIKWeapon.isUseReticleOnAimingEnabled ();
			bool useReticleEnabled = currentIKWeapon.isUseReticleEnabled ();

			if (carryingWeaponInThirdPerson) {
				enableOrDisableRegularWeaponCursor (value);

				if (weaponCursorAimingInFirstPerson.activeSelf) {
					weaponCursorAimingInFirstPerson.SetActive (false);
				}

				if (aimingInThirdPerson) {
					if (useReticleOnAimingEnabled) {
						checkRegularCustomWeaponReticle (weaponCursorRegular, value, useReticleOnAimingEnabled);
					} else {
						checkRegularCustomWeaponReticle (weaponCursorRegular, false, useReticleOnAimingEnabled);
					}
				} else {
					if (useReticleEnabled) {
						checkRegularCustomWeaponReticle (weaponCursorRegular, true, useReticleEnabled);
					} else {
						checkRegularCustomWeaponReticle (weaponCursorRegular, false, useReticleEnabled);
					}
				}
			} else if (carryingWeaponInFirstPerson) {
				if (aimingInFirstPerson) {
					if ((!usingDualWeapon && currentWeaponSystem != null && currentWeaponSystem.isUsingSight ()) ||
					    (usingDualWeapon && (currentRightWeaponSystem.isUsingSight () || currentLeftWeaponSystem.isUsingSight ()))) {

						if (weaponCursorAimingInFirstPerson.activeSelf) {
							weaponCursorAimingInFirstPerson.SetActive (false);
						}

						enableOrDisableRegularWeaponCursor (false);
					} else {
						if (useReticleOnAimingEnabled) {
							if (weaponCursorAimingInFirstPerson.activeSelf != value) {
								weaponCursorAimingInFirstPerson.SetActive (value);
							}

							checkRegularCustomWeaponReticle (weaponCursorAimingInFirstPerson, value, useReticleOnAimingEnabled);
						} else {
							checkRegularCustomWeaponReticle (weaponCursorAimingInFirstPerson, false, useReticleOnAimingEnabled);
						}

						enableOrDisableRegularWeaponCursor (false);
					}
				} else {
					if (weaponCursorAimingInFirstPerson.activeSelf) {
						weaponCursorAimingInFirstPerson.SetActive (false);
					}

					enableOrDisableRegularWeaponCursor (true);

					if (useReticleEnabled) {
						checkRegularCustomWeaponReticle (weaponCursorRegular, true, useReticleEnabled);
					} else {
						checkRegularCustomWeaponReticle (weaponCursorRegular, false, useReticleEnabled);
					}
				}
			} else {
				enableOrDisableRegularWeaponCursor (value);

				if (weaponCursorAimingInFirstPerson.activeSelf != value) {
					weaponCursorAimingInFirstPerson.SetActive (value);
				}

				checkRegularCustomWeaponReticle (weaponCursorRegular, value, false);
			}

			if (!weaponCursorRegular.activeSelf && !weaponCursorAimingInFirstPerson.activeSelf) {
				if (weaponCursorUnableToShoot.activeSelf) {
					weaponCursorUnableToShoot.SetActive (false);
				}
			}

			checkIfAnyReticleActive ();
		}
	}

	public void checkUpdateReticleActiveState (bool state)
	{
		playerCameraManager.checkUpdateReticleActiveState (state);
	}

	public void checkIfAnyReticleActive ()
	{
		if (weaponCursor != null) {
			if ((weaponCursor.activeSelf && (weaponCursorRegular.activeSelf || weaponCursorAimingInFirstPerson.activeSelf || weaponCustomReticle.activeSelf)) || weaponCursorUnableToShoot.activeSelf) {
				checkUpdateReticleActiveState (true);
			} else {
				checkUpdateReticleActiveState (false);
			}
		}
	}

	public void enableOrDisableWeaponCursorUnableToShoot (bool state)
	{
		if (weaponCursorRegular != null) {

			if (usingDualWeapon) {
				if (state) {
					if (!currentRightIKWeapon.isWeaponSurfaceDetected () || !currentLeftIkWeapon.isWeaponSurfaceDetected ()) {
						return;
					}
				}
			}

			if (state) {
				weaponCursorRegularPreviouslyEnabled = weaponCursorRegular.activeSelf;
				weaponCursorAimingInFirstPersonPreviouslyEnabled = weaponCursorAimingInFirstPerson.activeSelf;

				enableOrDisableRegularWeaponCursor (false);

				if (weaponCursorAimingInFirstPerson.activeSelf) {
					weaponCursorAimingInFirstPerson.SetActive (false);
				}

				if (weaponCustomReticle.activeSelf) {
					weaponCustomReticle.SetActive (false);
				}
			} else {
				enableOrDisableRegularWeaponCursor (weaponCursorRegularPreviouslyEnabled);

				if (weaponCursorAimingInFirstPerson.activeSelf != weaponCursorAimingInFirstPersonPreviouslyEnabled) {
					weaponCursorAimingInFirstPerson.SetActive (weaponCursorAimingInFirstPersonPreviouslyEnabled);
				}

				enableOrDisableWeaponCursor (true);
			}

			bool weaponCursorUnableToShootState = false;               

			if ((useWeaponCursorUnableToShootThirdPerson && carryingWeaponInThirdPerson) || (useWeaponCursorUnableToShootFirstPerson && carryingWeaponInFirstPerson)) {
				weaponCursorUnableToShootState = state;
			}

			if (weaponCursorUnableToShoot.activeSelf != weaponCursorUnableToShootState) {
				weaponCursorUnableToShoot.SetActive (weaponCursorUnableToShootState);
			}
		}
	}

	public void enableOrDisableGeneralWeaponCursor (bool state)
	{
		if (weaponCursor != null) {
			if (weaponCursorActive) {
				if (weaponCursor.activeSelf != state) {
					weaponCursor.SetActive (state);
				}
			} else {
				if (weaponCursor.activeSelf) {
					weaponCursor.SetActive (false);
				}
			}
		}
	}

	public void enableOrDisableRegularWeaponCursor (bool state)
	{
		if (weaponCursorRegular != null) {
			if (weaponCursorRegular.activeSelf != state) {
				weaponCursorRegular.SetActive (state);
			}
		}
	}

	public void checkRegularCustomWeaponReticle (GameObject cursorToCheck, bool state, bool weaponReticleActive)
	{
//		print (cursorToCheck.name + " " + state + " " + weaponReticleActive);

		if (usingDualWeapon || currentWeaponSystem != null) {
			if (weaponReticleActive && (!usingDualWeapon && currentWeaponSystem.useCustomReticleEnabled ()) || (usingDualWeapon && currentRightWeaponSystem.useCustomReticleEnabled ())) {
				if (state) {
					if (!weaponCustomReticle.activeSelf) {
						weaponCustomReticle.SetActive (true);
					}

					if (cursorToCheck.activeSelf) {
						cursorToCheck.SetActive (false);
					}

					RawImage currentWeaponCustomReticle = weaponCustomReticle.GetComponent<RawImage> ();

					if (usingDualWeapon) {
						if (currentRightWeaponSystem.useAimCustomReticleEnabled ()) {
							if (isAimingWeapons ()) {
								currentWeaponCustomReticle.texture = currentRightWeaponSystem.getAimCustomReticle ();
							} else {
								currentWeaponCustomReticle.texture = currentRightWeaponSystem.getRegularCustomReticle ();
							}
						} else {
							currentWeaponCustomReticle.texture = currentRightWeaponSystem.getRegularCustomReticle ();
						}
					} else {
						if (currentWeaponSystem.useAimCustomReticleEnabled ()) {
							if (isAimingWeapons ()) {
								currentWeaponCustomReticle.texture = currentWeaponSystem.getAimCustomReticle ();
							} else {
								currentWeaponCustomReticle.texture = currentWeaponSystem.getRegularCustomReticle ();
							}
						} else {
							currentWeaponCustomReticle.texture = currentWeaponSystem.getRegularCustomReticle ();
						}
					}
				} else {
					if (weaponCustomReticle.activeSelf) {
						weaponCustomReticle.SetActive (false);
					}
				}
			} else {
				if (weaponCustomReticle.activeSelf) {
					weaponCustomReticle.SetActive (false);
				}

				if (cursorToCheck.activeSelf != state) {
					cursorToCheck.SetActive (state);
				}
			}
		}
	}

	public bool checkIfWeaponIsOnSamePocket (string firstWeaponName, string secondWeaponName)
	{
		IKWeaponSystem firstIKWeaponSystem = getIKWeaponSystem (firstWeaponName);

		IKWeaponSystem secondIKWeaponSystem = getIKWeaponSystem (secondWeaponName);

		if (firstIKWeaponSystem != null && secondIKWeaponSystem != null) {
			GameObject firstWeapon = firstIKWeaponSystem.gameObject;

			GameObject secondWeapon = secondIKWeaponSystem.gameObject;

			for (int j = 0; j < weaponPocketList.Count; j++) {
				for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
					if (weaponPocketList [j].weaponOnPocketList [k].weaponList.Contains (firstWeapon) &&
					    weaponPocketList [j].weaponOnPocketList [k].weaponList.Contains (secondWeapon)) {
						return true;
					}
				}
			}
		}
		return false;
	}
		
	//pick a weapon unable to be used before
	public bool pickWeapon (string weaponName)
	{
		string weaponNameToLower = weaponName.ToLower ();

		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().ToLower ().Equals (weaponNameToLower)) {
				if (!weaponsList [i].isWeaponEnabled ()) {
					//check if the pocket of the weapon to pick already contains another active weapon
					bool weaponInPocketFound = false;
					GameObject weaponGameObject = weaponsList [i].gameObject;

					int weaponPocketIndex = -1;
					int weaponSubpocketIndex = -1;

					for (int j = 0; j < weaponPocketList.Count; j++) {
						if (!weaponInPocketFound) {
							for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
								for (int h = 0; h < weaponPocketList [j].weaponOnPocketList [k].weaponList.Count; h++) {
									if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h] == weaponGameObject) {
										weaponPocketIndex = j;

										weaponSubpocketIndex = k;

										weaponInPocketFound = true;
									}
								}
							}
						}
					}

					bool carryingWeaponPreviously = false;
					if (isUsingWeapons ()) {
						carryingWeaponPreviously = true;
					}
	
					IKWeaponSystem currentIKWeaponSystem = currentIKWeapon;
					bool weaponToDropFound = false;
					bool canBeDropped = true;

					//when the player picks a weapon and another is going to be dropped, these are the cases that can happen:
					//-The player was using the weapon to drop, so this can happen:
					//--The option to change to weapon picked is not active, so the player just drops his weapon and the new one is enabled
					//--The option to change to weapon picked is active, so the player needs to drop his weapon and draw the new one
					//-The player was not using the weapon to drop, so it is just dropped and this can happen
					//--The weapon to drop was the current weapon
					//--The weapon to drop was not the current weapon, and this can happen:
					//---The current weapon, which doesn't changes, is being used by the player
					if (weaponPocketIndex > -1 && weaponSubpocketIndex > -1) {
						List<GameObject> currentPocketWeaponList = weaponPocketList [weaponPocketIndex].weaponOnPocketList [weaponSubpocketIndex].weaponList;
							
						for (int j = 0; j < currentPocketWeaponList.Count; j++) {
							if (!weaponToDropFound) {
								GameObject weaponGameObjectToDrop = currentPocketWeaponList [j];

								if (weaponGameObjectToDrop != null) {
									IKWeaponToDrop = weaponGameObjectToDrop.GetComponent<IKWeaponSystem> ();

									if (IKWeaponToDrop.isWeaponEnabled ()) {
										weaponToDropFound = true;

										//print (IKWeaponToDrop.name + " enabled in that pocket found will be dropped");
										//-The player was using the weapon to drop, so this can happen:

										if (IKWeaponToDrop.carrying) {
											currentIKWeaponBeforeCheckPockets = currentIKWeapon;
											//print (IKWeaponToDrop.name + " was being used by the player");

											canBeDropped = dropCurrentWeapon (IKWeaponToDrop, false, setWeaponWhenPicked, false, false);

											if (!canBeDropped) {
												//print (IKWeaponToDrop.name + " can't be dropped");

												showObjectMessage (IKWeaponToDrop.getWeaponSystemName () + " " + cantPickWeaponMessage, weaponMessageDuration, weaponsMessageWindow);

												return false;
											}

											setWeaponByElement (currentIKWeaponBeforeCheckPockets);
										} 
										//-The player was not using the weapon to drop, so it is just dropped and this can happen
										else {
											//print (IKWeaponToDrop.name + " wasn't being used by the player");
											if (IKWeaponToDrop.isCurrentWeapon ()) {
												//print (IKWeaponToDrop.name + " is current weapon, so it needs to be changed");

												canBeDropped = dropCurrentWeapon (IKWeaponToDrop, false, false, false, false);

												if (!canBeDropped) {
													//print (IKWeaponToDrop.name + " can't be dropped");
													showObjectMessage (IKWeaponToDrop.getWeaponSystemName () + " " + cantPickWeaponMessage, weaponMessageDuration, weaponsMessageWindow);

													return false;
												}

												currentIKWeapon.setCurrentWeaponState (false);
											} else {
												//-The player was not using the weapon to drop, so it is just dropped
												//---The current weapon, which doesn't changes, is being used by the player
												currentIKWeaponBeforeCheckPockets = currentIKWeapon;

												//print (IKWeaponToDrop.name + " just dropped");
												if (isUsingWeapons ()) {
													//print ("previous weapon was " + currentIKWeaponBeforeCheckPockets.name);

													canBeDropped = dropCurrentWeapon (IKWeaponToDrop, false, false, false, false);

													if (!canBeDropped) {
														//print (IKWeaponToDrop.name + " can't be dropped");
														showObjectMessage (IKWeaponToDrop.getWeaponSystemName () + " " + cantPickWeaponMessage, weaponMessageDuration, weaponsMessageWindow);
													
														return false;
													}

													//in case the current weapon has been changed due to the function dropCurrentWeapon, select again the previous current weapon before
													//the player picked the new weapon
													setWeaponByElement (currentIKWeaponBeforeCheckPockets);
												} else {
													//before drop the weapon, it is neccessary to store the weapon that the player is using
													canBeDropped = dropCurrentWeapon (IKWeaponToDrop, false, true, false, false);

													if (!canBeDropped) {
														//print (IKWeaponToDrop.name + " can't be dropped");
														showObjectMessage (IKWeaponToDrop.getWeaponSystemName () + " " + cantPickWeaponMessage, weaponMessageDuration, weaponsMessageWindow);

														return false;
													}

													setWeaponByElement (currentIKWeaponBeforeCheckPockets);
												}
											}
										}
									}
								}
							}
						}
					}

					//print ("taking " + weaponsList [i].name);
					//check if this picked weapon is the first one that the player has
					bool anyWeaponAvailablePreviosly = checkIfWeaponsAvailable ();

					//reset IK values
					weaponsList [i].setHandsIKTargetValue (0, 0);
					weaponsList [i].setIKWeight (0, 0);

					weaponsList [i].setWeaponEnabledState (true);

					//enable the picked weapon model in the player
					if (isThirdPersonView) {
						weaponsList [i].enableOrDisableWeaponMesh (true);
					}
						
					//get the upper body rotation of the current weapon picked
					getCurrentWeaponRotation (weaponsList [i]);	

					//set the state of weapons availables again
					anyWeaponAvailable = checkIfWeaponsAvailable ();
				
					//draw or not the picked weapon according to the main settings
					if (!setWeaponWhenPicked) {
						if (!anyWeaponAvailablePreviosly || !carryingWeaponPreviously) {
							//set the new picked weapon as the current one
							setWeaponByElement (weaponsList [i]);
						} else {
							if (weaponToDropFound) {
								//print (currentIKWeaponBeforeCheckPockets.name + " " + IKWeaponToDrop.name);
								if (currentIKWeaponBeforeCheckPockets == IKWeaponToDrop) {
									//print ("weapon to drop is the same that is being used by the player");
									setWeaponByElement (weaponsList [i]);
									carryingWeaponInFirstPerson = false;
									carryingWeaponInThirdPerson = false;
								}
							}
						}

						return true;
					}

					//the player was carring a weapon, set it to not the current weapon
					if (anyWeaponAvailablePreviosly) {
						//if (carryingWeaponPreviously) {
						setWeaponByElement (weaponsList [i]);
						//print ("disable " + currentIKWeaponSystem.name);
						//}
					} else {
						// else, the player hadn't any weapon previously, so se the current picked weapon as the current weapon
						//print ("first weapon picked");
						setWeaponByElement (weaponsList [i]);
					}

					//change between current weapon and the picked weapon
					if (weaponsModeActive) {
						if (carryingWeaponPreviously) {
							setWeaponByElement (currentIKWeaponSystem);

							currentIKWeaponSystem.setCurrentWeaponState (false);

							choosedWeapon = i;
							//print ("Change weapon");
							keepingWeapon = false;

							if (useQuickDrawWeapon && isThirdPersonView) {
								quicChangeWeaponThirdPersonAction ();
							} else {
								setChangingWeaponState (true);
							}
						} else {
							if (anyWeaponAvailablePreviosly) {
								weaponsList [i].setCurrentWeaponState (true);

								if (!weaponToDropFound) {
									setWeaponByElement (weaponsList [i]);
								}
							}

							//print ("draw weapon");
							drawOrKeepWeapon (true);
						}
					} else {
						setWeaponByElement (weaponsList [i]);
					}
				}

				return true;
			}
		}

		return false;
	}

	bool equippingPickedWeaponActive;

	public void setEquippingPickedWeaponActiveState (bool state)
	{
		equippingPickedWeaponActive = state;
	}

	void instantiateWeaponInRunTime (string weaponName)
	{
		if (useUsableWeaponPrefabInfoList) {

			int weaponIndex = usableWeaponPrefabInfoList.FindIndex (s => s.Name == weaponName);

			if (weaponIndex > -1) {

				updateWeaponListCount ();

				if (weaponListCount > 0) {
					for (int i = 0; i < weaponListCount; i++) {
						if (weaponsList [i].getWeaponSystemName ().Equals (weaponName)) {
							return;
						}
					}
				}
					
				addWeaponToListInRunTime (usableWeaponPrefabInfoList [weaponIndex].usableWeaponPrefab, false, true, false);
			}
		}
	}

	public IKWeaponSystem equipWeapon (string weaponName, bool initializingInventory, bool equippingDualWeapon, string rightWeaponName, string lefWeaponName)
	{
		checkTypeView ();

		if (useUsableWeaponPrefabInfoList) {
			instantiateWeaponInRunTime (weaponName);
		}

		updateWeaponListCount ();

		//###################################################
		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName)) {
				if (!weaponsList [i].isWeaponEnabled () || equippingDualWeapon) {
					//check if the pocket of the weapon to pick already contains another active weapon
					bool weaponInPocketFound = false;
					GameObject weaponGameObject = weaponsList [i].gameObject;

					if (showDebugLog) {
						if (!initializingInventory) {
							if (equippingDualWeapon) {
								print ("equip dual weapons: " + rightWeaponName + " and " + lefWeaponName);
							} else {
								print ("equip single weapon: " + weaponName);
							}
						}
					}

					int weaponPocketIndex = -1;
					int weaponSubpocketIndex = -1;

					if (!equippingDualWeapon) {
						for (int j = 0; j < weaponPocketList.Count; j++) {
							if (!weaponInPocketFound) {
								for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
									for (int h = 0; h < weaponPocketList [j].weaponOnPocketList [k].weaponList.Count; h++) {
										if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h] == weaponGameObject) {
											weaponPocketIndex = j;

											weaponSubpocketIndex = k;

											weaponInPocketFound = true;
										}
									}
								}
							}
						}
					}

					bool carryingWeaponPreviously = false;

					if (isUsingWeapons ()) {
						carryingWeaponPreviously = true;
					}

					bool carryingDualWeaponToRemove = false;

					IKWeaponSystem currentIKWeaponSystem = currentIKWeapon;
					IKWeaponSystem IKWeaponToEquip = weaponsList [i];

					bool weaponToDropFound = false;

					if (!equippingDualWeapon) {
						if (weaponPocketIndex > -1 && weaponSubpocketIndex > -1) {
							List<GameObject> currentPocketWeaponList = weaponPocketList [weaponPocketIndex].weaponOnPocketList [weaponSubpocketIndex].weaponList;

							for (int j = 0; j < currentPocketWeaponList.Count; j++) {
								if (!weaponToDropFound) {
									GameObject weaponGameObjectToDrop = currentPocketWeaponList [j];

									if (weaponGameObjectToDrop != null) {
										IKWeaponToDrop = weaponGameObjectToDrop.GetComponent<IKWeaponSystem> ();

										if (IKWeaponToDrop.isWeaponEnabled ()) {
											weaponToDropFound = true;
											//print (IKWeaponToDrop.name + " enabled in that pocket found will be dropped");

											if (usingDualWeapon) {
												if (currentRightIKWeapon == IKWeaponToDrop) {
													currentIKWeapon = currentRightIKWeapon;
													currentWeaponSystem = currentRightWeaponSystem;

													disableCurrentWeapon ();
												}

												if (currentLeftIkWeapon == IKWeaponToDrop) {
													currentIKWeapon = currentLeftIkWeapon;
													currentWeaponSystem = currentLeftWeaponSystem;

													disableCurrentWeapon ();
												}

												checkIfCurrentIkWeaponNotNull ();
											} else {
												if (currentIKWeaponSystem == IKWeaponToDrop) {
													disableCurrentWeapon ();
												}
											}

											IKWeaponToDrop.setWeaponEnabledState (false);

											IKWeaponToDrop.setCurrentWeaponState (false);

											IKWeaponToDrop.enableOrDisableWeaponMesh (false);

											playerInventoryManager.unEquipObjectByName (IKWeaponToDrop.getWeaponSystemName ());

											if (showDebugLog) {
												print (IKWeaponToDrop.name + " is on the same pocket as " + weaponName + " so it will be unequipped");
											}

											bool weaponToRemoveIsConfiguredAsDual = IKWeaponToDrop.isWeaponConfiguredAsDualWeapon ();

											if (weaponToRemoveIsConfiguredAsDual) {
												if (showDebugLog) {
													print (IKWeaponToDrop.name + " was configured as dual weapon with " + IKWeaponToDrop.getLinkedDualWeaponName ());
												}

												IKWeaponSystem secondaryWeaponToSetAsSingle = getWeaponSystemByName (IKWeaponToDrop.getLinkedDualWeaponName ()).getIKWeaponSystem ();

												IKWeaponToDrop.setWeaponConfiguredAsDualWepaonState (false, "");
												IKWeaponToDrop.setUsingDualWeaponState (false);

												secondaryWeaponToSetAsSingle.setWeaponConfiguredAsDualWepaonState (false, "");
												secondaryWeaponToSetAsSingle.setUsingDualWeaponState (false);
											
												if (storePickedWeaponsOnInventory) {
													playerInventoryManager.updateSingleWeaponSlotInfoWithoutAddingAnotherSlot (secondaryWeaponToSetAsSingle.getWeaponSystemName ());

													updateCurrentChoosedDualWeaponIndex ();
												}

												if (usingDualWeapon) {
													if (currentRighWeaponName.Equals (IKWeaponToDrop.name) || currentLeftWeaponName.Equals (IKWeaponToDrop.name)) {
														if (showDebugLog) {
															print ("carrying the weapon to remove " + IKWeaponToDrop.name + " as dual weapon");
														}

														carryingDualWeaponToRemove = true;
													}
												}
											}
										}
									}
								}
							}
						}
					}

					//print ("taking " + IKWeaponToEquip.name);
					//check if this picked weapon is the first one that the player has
					bool anyWeaponAvailablePreviosly = checkIfWeaponsAvailable ();

					//reset IK values
					IKWeaponToEquip.setHandsIKTargetValue (0, 0);
					IKWeaponToEquip.setIKWeight (0, 0);

					IKWeaponToEquip.setWeaponEnabledState (true);

					//enable the picked weapon model in the player

					if (initializingInventory) {
						checkTypeViewBeforeStart ();
					}
						
					if (isThirdPersonView && drawWeaponWhenPicked && !IKWeaponToEquip.hideWeaponIfKeptInThirdPerson) {
						IKWeaponToEquip.enableOrDisableWeaponMesh (true);
					}

					if (!isThirdPersonView && initializingInventory) {
						IKWeaponToEquip.enableOrDisableWeaponMesh (false);
					}

					bool canDrawWeapon = false;

					if (drawWeaponWhenPicked) {
						if (drawPickedWeaponOnlyItNotPreviousWeaponEquipped && equippingPickedWeaponActive) {
							if (!carryingWeaponPreviously) {
								canDrawWeapon = true;
							}
						} else {
							canDrawWeapon = true;
						}
					}

					if (canDrawWeapon) {
						//get the upper body rotation of the current weapon picked
						getCurrentWeaponRotation (IKWeaponToEquip);	
					}

					//set the state of weapons availables again
					anyWeaponAvailable = checkIfWeaponsAvailable ();

					//draw or not the picked weapon according to the main settings
					bool checkDrawWepon = false;

					if (initializingInventory) {
						checkDrawWepon = true;
					}

					if (!changeToNextWeaponWhenEquipped && !drawWeaponWhenPicked) {
						checkDrawWepon = true;
					}

					if (!checkDrawWepon) {
						if (drawPickedWeaponOnlyItNotPreviousWeaponEquipped && equippingPickedWeaponActive) {

							if (carryingWeaponPreviously && currentIKWeaponSystem != IKWeaponToEquip) {
		
								return IKWeaponToEquip;
							}
						}
					}

					if (checkDrawWepon) {
						if (!anyWeaponAvailablePreviosly || !carryingWeaponPreviously) {

							if (equippingDualWeapon) {
								if (currentRightIKWeapon != null) {
									currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
								}

								if (currentLeftIkWeapon != null) {
									currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
								}

								currentRighWeaponName = rightWeaponName;
								currentLeftWeaponName = lefWeaponName;

								setCurrentRightIkWeaponByName (currentRighWeaponName);
								setCurrentLeftIKWeaponByName (currentLeftWeaponName);

								currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (true, currentLeftIkWeapon.getWeaponSystemName ());
								currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (true, currentRightIKWeapon.getWeaponSystemName ());
							} else {
								//set the new picked weapon as the current one
								IKWeaponToEquip.getWeaponSystemName ();
								setWeaponByElement (IKWeaponToEquip);
							}
						}

						return IKWeaponToEquip;
					}

					if (!equippingDualWeapon && !carryingDualWeaponToRemove) {
						//the player was carring a weapon, set it to not the current weapon
						if (anyWeaponAvailablePreviosly) {
							setWeaponByElement (IKWeaponToEquip);
							//print ("disable " + currentIKWeaponSystem.name);
						} else {
							// else, the player hadn't any weapon previously, so se the current picked weapon as the current weapon
							//print ("first weapon picked");
							setWeaponByElement (IKWeaponToEquip);
						}
					}

					//change between current weapon and the picked weapon
					if (weaponsModeActive) {
						if (equippingDualWeapon) {
							equippingDualWeaponsFromInventoryMenu = true;

							if (currentIKWeaponSystem && currentIKWeaponSystem != currentRightIKWeapon && currentIKWeaponSystem != currentLeftIkWeapon) {
								currentIKWeaponSystem.setCurrentWeaponState (false);
							}

							changeDualWeapons (rightWeaponName, lefWeaponName);
						} else {
							if (carryingWeaponPreviously) {

								if (usingDualWeapon) {
									if (carryingDualWeaponToRemove) {
										IKWeaponToDrop.setHandsIKTargetValue (0, 0);
										IKWeaponToDrop.setIKWeight (0, 0);

										currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
										currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
									}

									if (showDebugLog) {
										print ("previously using dual weapons");
									}

									settingSingleWeaponFromNumberKeys = true;

									singleWeaponNameToChangeFromNumberkeys = IKWeaponToEquip.getWeaponSystemName ();
									changeSingleWeapon (IKWeaponToEquip.getWeaponSystemName ());
								} else {
									setWeaponByElement (currentIKWeaponSystem);
									currentIKWeaponSystem.setCurrentWeaponState (false);
									choosedWeapon = i;

									//print ("Change weapon");
									keepingWeapon = false;
						
									if (useQuickDrawWeapon && isThirdPersonView) {
										quicChangeWeaponThirdPersonAction ();
									} else {
										setChangingWeaponState (true);
									}
								}
							} else {
								IKWeaponToEquip.setCurrentWeaponState (true);

								if (anyWeaponAvailablePreviosly) {
								
									if (!weaponToDropFound) {
										setWeaponByElement (IKWeaponToEquip);
									}
								}

								//print ("draw weapon");
								drawOrKeepWeapon (true);
							}
						}
					} else {
						if (!equippingDualWeapon) {
							setWeaponByElement (IKWeaponToEquip);
						}
					}
				}

				return weaponsList [i];
		
			}
		}

		return null;
	}

	bool setRightWeaponAsCurrentSingleWeapon = true;

	public void unequipWeapon (string weaponName, bool unequippingDualWeapon)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName)) {
				if (weaponsList [i].isWeaponEnabled ()) {

					string dualWeaponNameToSetAsSingle = currentRighWeaponName;

					if (weaponName.Equals (currentRighWeaponName)) {
						dualWeaponNameToSetAsSingle = currentLeftWeaponName;

						if (showDebugLog) {
							print ("dual right weapon " + currentRighWeaponName + " is the same to unequip, setting " + currentLeftWeaponName + " as the one to keep as single");
						}

						setRightWeaponAsCurrentSingleWeapon = false;
					}

					if (showDebugLog) {
						if (unequippingDualWeapon) {
							print ("Unequipping Dual Weapon: " + weaponName + " to leave equipped " + dualWeaponNameToSetAsSingle);
						} else {
							print ("Unequipping Single Weapon: " + weaponName);
						}
					}
						
					IKWeaponSystem currentIKWeaponSystem = weaponsList [i];

					bool carryinWeaponToUnequip = false;

					if (usingDualWeapon) {
						if (currentRightIKWeapon == currentIKWeaponSystem) {
							if (showDebugLog) {
								print ("setting " + currentRightIKWeapon.getWeaponSystemName () + "  as the current weapon to disable");
							}

							currentIKWeapon = currentRightIKWeapon;
							currentWeaponSystem = currentRightWeaponSystem;

							carryinWeaponToUnequip = true;
							disableCurrentWeapon ();
						}

						if (currentLeftIkWeapon == currentIKWeaponSystem) {
							if (showDebugLog) {
								print ("setting " + currentLeftIkWeapon.getWeaponSystemName () + "  as the current weapon to disable");
							}

							currentIKWeapon = currentLeftIkWeapon;
							currentWeaponSystem = currentLeftWeaponSystem;

							carryinWeaponToUnequip = true;
							disableCurrentWeapon ();
						}

						if (!carryinWeaponToUnequip) {
							currentIKWeapon = currentIKWeaponSystem;
							currentWeaponSystem = currentIKWeaponSystem.getWeaponSystemManager ();

							disableCurrentWeapon ();
						}

						checkIfCurrentIkWeaponNotNull ();
					} else {
						if (currentIKWeapon == currentIKWeaponSystem) {
							carryinWeaponToUnequip = true;
							disableCurrentWeapon ();
						}
					}

					currentIKWeaponSystem.setWeaponEnabledState (false);
					currentIKWeaponSystem.setCurrentWeaponState (false);

					//enable the picked weapon model in the player
					currentIKWeaponSystem.enableOrDisableWeaponMesh (false);

					playerInventoryManager.unEquipObjectByName (currentIKWeaponSystem.getWeaponSystemName ());

					//reset IK values
					currentIKWeaponSystem.setHandsIKTargetValue (0, 0);
					currentIKWeaponSystem.setIKWeight (0, 0);
				
					//print ("disabling " + currentIKWeaponSystem.name);
					if (!unequippingDualWeapon) {
						bool anyWeaponAvailableCurrently = checkIfWeaponsAvailable ();

						if (carryinWeaponToUnequip && anyWeaponAvailableCurrently) {
						
							if (setNextWeaponAvailableToIndex (i)) {
								weaponChanged ();

								chooseDualWeaponIndex = currentWeaponSystem.getWeaponNumberKey ();
							}
						}

						if (!anyWeaponAvailableCurrently) {
							playerInventoryManager.disableCurrentlySelectedIcon ();
						}

						//draw or not the picked weapon according to the main settings
						if (!changeToNextWeaponWhenUnequipped || !anyWeaponAvailableCurrently) {
							return;
						}
					}
						
					//change between current weapon and the picked weapon
					if (weaponsModeActive) {
						if (unequippingDualWeapon) {
							if (carryinWeaponToUnequip) {
								if (showDebugLog) {
									print ("carrying current dual weapons to unequip " + currentIKWeaponSystem.getWeaponSystemName ());
								}

								currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
								currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");

								changeSingleWeapon (dualWeaponNameToSetAsSingle);
							} else {
								if (showDebugLog) {
									print ("not carrying current dual weapons to unequip " + currentIKWeaponSystem.getWeaponSystemName ());
								}

								IKWeaponSystem secondaryWeaponToSetAsSingle = getWeaponSystemByName (currentIKWeaponSystem.getLinkedDualWeaponName ()).getIKWeaponSystem ();

								secondaryWeaponToSetAsSingle.setWeaponConfiguredAsDualWepaonState (false, "");
								currentIKWeaponSystem.setWeaponConfiguredAsDualWepaonState (false, "");
							}
						} else {
							if (carryinWeaponToUnequip && !isPlayerCarringWeapon ()) {
								//print ("draw next weapon");
								drawOrKeepWeapon (true);
							}
						}
					}
				} else {
					playerInventoryManager.unEquipObjectByName (weaponsList [i].getWeaponSystemName ());
				}

				return;
			}
		}

		return;
	}

	public void checkTypeViewBeforeStart ()
	{
		checkTypeView ();
	}

	//the player has dead, so set his state in the weapons
	public void setDeadState (bool state)
	{
		playerIsDead = state;

		if (state) {
			lockCursorAgain ();

			dropWeaponWhenPlayerDies ();
		} else {
			if (anyWeaponAvailable) {
				getCurrentWeaponRotation (currentIKWeapon);

				extraRotation = 0;
			}
		}
	}

	public bool isDrawWeaponWhenResurrectActive ()
	{
		return drawWeaponWhenResurrect;
	}

	float lastTimeDropWeapon = 0;

	public bool dropWeaponCheckingMinDelay ()
	{
		if (lastTimeDropWeapon > 0 && Time.time < lastTimeDropWeapon + 1) {
			return false;
		}

		if (currentIKWeapon != null) {
			if (reloadingWithAnimationActive ||
			    currentIKWeapon.isWeaponMoving () ||
			    playerManager.isGamePaused () ||
			    currentIKWeapon.isReloadingWeapon () ||
			    weaponsAreMoving () ||
			    Time.time < lastTimeReload + 0.5f) {

				return false;
			}
		}

		if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable && currentIKWeapon.isCurrentWeapon ()) {
			if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
				dropWeapon ();

				lastTimeDropWeapon = Time.time;

				return true;
			}
		}

		return false;
	}

	//drop a weapon, so it is disabled and the player can't use it until that weapon is picked again
	public void dropWeapon ()
	{
		//&& (!aimingInFirstPerson && !aimingInThirdPerson) 
		if (currentWeaponSystem != null && canDropWeapons) {

			bool canBeDropped = false;

			if (usingDualWeapon) {
				canBeDropped = dropCurrentDualWeapon (currentIKWeapon, false, true);
			} else {
				canBeDropped = dropCurrentWeapon (currentIKWeapon, false, true, false, false);
			}

			if (!canBeDropped) {
				showObjectMessage (currentIKWeapon.getWeaponSystemName () + " " + cantDropCurrentWeaponMessage, weaponMessageDuration, weaponsMessageWindow);
			}
		}
	}

	public void dropWeaponByBebugButton ()
	{
		dropWeaponCheckingMinDelay ();
	}

	//drop the weapons when the player dies, according to the configuration in the inspector
	public void dropWeaponWhenPlayerDies ()
	{
		if (dropCurrentWeaponWhenDie || dropAllWeaponsWhenDie) {
			if (!weaponsModeActive || (dropWeaponsOnlyIfUsing && !carryingWeaponInThirdPerson && !carryingWeaponInFirstPerson)) {
				return;
			}

			if (dropAllWeaponsWhenDie) {
				updateWeaponListCount ();

				for (int k = 0; k < weaponListCount; k++) {
					if (weaponsList [k].isWeaponEnabled ()) {
						dropCurrentWeapon (weaponsList [k], true, true, false, false);
					}
				}
			} else {
				if (dropCurrentWeaponWhenDie && currentIKWeapon != null) {
					if (currentIKWeapon.weaponEnabled) {
						dropCurrentWeapon (currentIKWeapon, true, true, false, false);
					}
				}
			}
		}
	}

	public void dropAllWeaponsExternally ()
	{
		if (weaponsModeActive) {
			dropCurrentWeaponWhenDie = true;

			dropAllWeaponsWhenDie = true;

			dropWeaponWhenPlayerDies ();
		}
	}

	public void dropCurrentWeaponExternally ()
	{
		if (weaponsModeActive) {
			dropWeapon ();
		}
	}

	public void dropCurrentWeaponExternallyWithoutResultAndDestroyIt ()
	{
		if (weaponsModeActive) {
			ignoreInstantiateDroppedWeaponActive = true;

			dropWeapon ();

			ignoreInstantiateDroppedWeaponActive = false;
		}
	}

	//drop the current weapon that the player is carrying, in third or first person
	public bool dropCurrentWeapon (IKWeaponSystem weaponToDrop, bool ignoreRagdollCollision, bool checkChangeToNextWeapon, 
	                               bool dropOnlyWeaponMesh, bool justKeepWeaponWithoutChangeToNext)
	{
		if (weaponToDrop == null || !weaponToDrop.canBeDropped) {
			return false;
		}

		lockCursorAgain ();

		if (!ignoreInstantiateDroppedWeaponActive) {
			launchWeaponRigidbodyWhenDropWeapon (weaponToDrop, dropOnlyWeaponMesh, ignoreRagdollCollision);
		}

		if (checkChangeToNextWeapon) {
			//if player dies and he is aiming, disable that state
			if (isAimingWeapons ()) {
				disableCurrentWeapon ();
			}
				
			if (!useQuickDrawWeapon) {
				//set the states in the weapons manager to search the next weapon to use
				if (moreThanOneWeaponAvailable () && !justKeepWeaponWithoutChangeToNext) {
					chooseNextWeapon (true, true);
				} else {
					weaponToDrop.setCurrentWeaponState (false);
				}
			}
		} else {
			if (isAimingWeapons ()) {
				changeCameraFov (false);
			}
		}

		if (isThirdPersonView) {
			weaponToDrop.enableOrDisableWeaponMesh (false);

			if (checkChangeToNextWeapon) {
				carryingWeaponInThirdPerson = false;
			}

			weaponToDrop.quickKeepWeaponThirdPerson ();

			if (checkChangeToNextWeapon) {
				IKManager.quickKeepWeaponState ();

				IKManager.stopIKWeaponsActions ();
			}
		} else {
			if (checkChangeToNextWeapon) {
				carryingWeaponInFirstPerson = false;
			}

			weaponToDrop.quickKeepWeaponFirstPerson ();

			weaponToDrop.enableOrDisableFirstPersonArms (false);
		}

		playerCameraManager.setOriginalRotationSpeed ();

		weaponToDrop.setWeaponEnabledState (false);

		currentWeaponSystem.setWeaponCarryState (false, false);

		enableOrDisableWeaponsHUD (false);

		enableOrDisableWeaponCursor (false);

		currentWeaponSystem.enableHUD (false);

		aimingInFirstPerson = false;
		aimingInThirdPerson = false;

		checkPlayerCanJumpWhenAimingState ();

		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.dropEquipByName (weaponToDrop.getWeaponSystemName (), 1, false, dropOnlyWeaponMesh);
		}

		resetCurrentWeaponRotation (weaponToDrop);
	
		if (!canMove) {
			getCurrentWeapon ();
		} else {
			if (checkChangeToNextWeapon && useQuickDrawWeapon) {
				//set the states in the weapons manager to search the next weapon to use

				if (checkIfWeaponsAvailable () && !justKeepWeaponWithoutChangeToNext) {
					chooseNextWeapon (true, false);
				} else {
					weaponToDrop.setCurrentWeaponState (false);
				}
			}
		}
			
		return true;
	}

	public bool dropCurrentDualWeapon (IKWeaponSystem weaponToDrop, bool ignoreRagdollCollision, bool checkChangeToNextWeapon)
	{
		if (weaponToDrop == null || !weaponToDrop.canBeDropped) {
			return false;
		}

		lockCursorAgain ();

		if (!ignoreInstantiateDroppedWeaponActive) {
			launchWeaponRigidbodyWhenDropWeapon (weaponToDrop, false, ignoreRagdollCollision);

			if (weaponToDrop == currentRightIKWeapon) {
				launchWeaponRigidbodyWhenDropWeapon (currentLeftIkWeapon, false, ignoreRagdollCollision);
			} else {
				launchWeaponRigidbodyWhenDropWeapon (currentRightIKWeapon, false, ignoreRagdollCollision);
			}
		}
			
		//if player dies and he is aiming, disable that state
		disableCurrentDualWeapon ();

		currentRightIKWeapon.setCurrentWeaponState (false);
		currentLeftIkWeapon.setCurrentWeaponState (false);

		currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
		currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");

		setUsingDualWeaponsState (false);

		disableDualWeaponStateOnWeapons ();

		if (isAimingWeapons ()) {
			changeCameraFov (false);
		}

		if (isThirdPersonView) {
			currentRightIKWeapon.enableOrDisableWeaponMesh (false);
			currentLeftIkWeapon.enableOrDisableWeaponMesh (false);

			if (checkChangeToNextWeapon) {
				carryingWeaponInThirdPerson = false;
			}

			currentRightIKWeapon.quickKeepWeaponThirdPerson ();
			currentLeftIkWeapon.quickKeepWeaponThirdPerson ();

			if (checkChangeToNextWeapon) {
				IKManager.quickKeepWeaponState ();
			}
		} else {
			if (checkChangeToNextWeapon) {
				carryingWeaponInFirstPerson = false;
			}

			currentRightIKWeapon.quickKeepWeaponFirstPerson ();
			currentRightIKWeapon.enableOrDisableFirstPersonArms (false);

			currentLeftIkWeapon.quickKeepWeaponFirstPerson ();
			currentLeftIkWeapon.enableOrDisableFirstPersonArms (false);
		}

		playerCameraManager.setOriginalRotationSpeed ();

		currentRightIKWeapon.setWeaponEnabledState (false);
		currentRightWeaponSystem.setWeaponCarryState (false, false);

		currentLeftIkWeapon.setWeaponEnabledState (false);
		currentLeftWeaponSystem.setWeaponCarryState (false, false);

		enableOrDisableWeaponsHUD (false);

		enableOrDisableWeaponCursor (false);

		currentRightWeaponSystem.enableHUD (false);
		currentLeftWeaponSystem.enableHUD (false);

		aimingInFirstPerson = false;
		aimingInThirdPerson = false;

		checkPlayerCanJumpWhenAimingState ();

		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.dropEquipByName (currentRightIKWeapon.getWeaponSystemName (), 1, false, false);
			playerInventoryManager.dropEquipByName (currentLeftIkWeapon.getWeaponSystemName (), 1, false, false);
		}

		resetCurrentWeaponRotation (weaponToDrop);

		if (!canMove) {
			getCurrentWeapon ();
		} else {
			if (checkChangeToNextWeapon) {
				//set the states in the weapons manager to search the next weapon to use
				if (checkIfWeaponsAvailable ()) {
					chooseNextWeapon (true, false);
				}
			}
		}

		return true;
	}

	public void launchWeaponRigidbodyWhenDropWeapon (IKWeaponSystem weaponToDrop, bool dropOnlyWeaponMesh, bool ignoreRagdollCollision)
	{
		Vector3 position = weaponToDrop.weaponTransform.position;
		Quaternion rotation = weaponToDrop.weaponTransform.rotation;

		GameObject weaponPickupToInstantiate = weaponToDrop.weaponPrefabModel;

		if (storePickedWeaponsOnInventory) {
			weaponPickupToInstantiate = weaponToDrop.inventoryWeaponPrefabObject;
		} 

		if (dropWeaponInventoryObjectsPickups || weaponPickupToInstantiate == null || dropOnlyWeaponMesh) {

			if (mainInventoryListManager == null) {
				mainInventoryListManager = FindObjectOfType<inventoryListManager> ();
			}

			if (dropOnlyWeaponMesh) {
				weaponPickupToInstantiate = mainInventoryListManager.getInventoryMeshByName (weaponToDrop.getWeaponSystemName ());
			} else {
				weaponPickupToInstantiate = mainInventoryListManager.getInventoryPrefabByName (weaponToDrop.getWeaponSystemName ());
			}
		}

		if (weaponPickupToInstantiate == null) {
			print ("WARNING: the character called " + gameObject.name + " is trying to drop the weapon " + weaponToDrop.getWeaponSystemName () + " but the prefab wasn't found");

			return;
		}

		//instantiate and drag the weapon object
		GameObject weaponClone = (GameObject)Instantiate (weaponPickupToInstantiate, position, rotation);

		Collider weaponToDropCollider = weaponClone.GetComponent<Collider> ();

		Physics.IgnoreCollision (weaponToDropCollider, mainCollider, true);

		Rigidbody weaponRigidbody = weaponClone.GetComponent<Rigidbody> ();

		if (weaponRigidbody == null) {
			weaponRigidbody = weaponClone.AddComponent<Rigidbody> ();

			setWeaponPartLayerFromCameraView (weaponClone, false);
		}

		weaponRigidbody.isKinematic = true;

		weaponClone.transform.position = position;
		weaponClone.transform.rotation = rotation;

		weaponRigidbody.isKinematic = false;

		Vector3 forceDirection = transform.forward * dropWeaponForceThirdPerson;

		if (holdingDropButtonToIncreaseForce) {
			forceDirection = transform.forward * currentDropForce;
		}

		if (ignoreRagdollCollision) {
			forceDirection = weaponToDrop.weaponTransform.position - transform.position;
			float distance = forceDirection.magnitude;
			forceDirection = forceDirection / distance;
		}

		if (!isThirdPersonView) {
			forceDirection = playerCameraManager.mainCameraTransform.forward * dropWeaponForceFirstPerson;

			if (holdingDropButtonToIncreaseForce) {
				forceDirection = playerCameraManager.mainCameraTransform.forward * currentDropForce;
			}
		}

		weaponRigidbody.AddForce (forceDirection);

		if (holdingDropButtonToIncreaseForce) {
			weaponClone.AddComponent<launchedObjects> ().setCurrentPlayer (gameObject);
		}

		//if the player dies, ignore collision between the current dropped weapon and the player's ragdoll
		if (ignoreRagdollCollision) {
			List <Collider> ragdollColliders = ragdollManager.getBodyColliderList ();

			for (int k = 0; k < ragdollColliders.Count; k++) {
				Physics.IgnoreCollision (weaponToDropCollider, ragdollColliders [k]);
			}
		}

		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.checkObjectDroppedEvent (weaponClone);
		}
	}

	public void activateSecondaryAction ()
	{
		if (usingDualWeapon) {
			currentRightWeaponSystem.activateSecondaryAction ();
			currentLeftWeaponSystem.activateSecondaryAction ();
		} else {
			currentWeaponSystem.activateSecondaryAction ();
		}
	}

	public void activateSecondaryActionOnDownPress ()
	{
		if (usingDualWeapon) {
			currentRightWeaponSystem.activateSecondaryActionOnDownPress ();
			currentLeftWeaponSystem.activateSecondaryActionOnDownPress ();
		} else {
			currentWeaponSystem.activateSecondaryActionOnDownPress ();
		}
	}

	public void activateSecondaryActionOnUpPress ()
	{
		if (usingDualWeapon) {
			currentRightWeaponSystem.activateSecondaryActionOnUpPress ();
			currentLeftWeaponSystem.activateSecondaryActionOnUpPress ();
		} else {
			currentWeaponSystem.activateSecondaryActionOnUpPress ();
		}
	}

	public void activateSecondaryActionOnUpHold ()
	{
		if (usingDualWeapon) {
			currentRightWeaponSystem.activateSecondaryActionOnUpHold ();
			currentLeftWeaponSystem.activateSecondaryActionOnUpHold ();
		} else {
			currentWeaponSystem.activateSecondaryActionOnUpHold ();
		}
	}

	public void activateForwardAcion ()
	{
		if (usingDualWeapon) {
			currentRightWeaponSystem.activateForwardAcion ();
			currentLeftWeaponSystem.activateForwardAcion ();
		} else {
			if (currentWeaponSystem != null) {
				currentWeaponSystem.activateForwardAcion ();
			}
		}
	}

	public void activateBackwardAcion ()
	{
		if (usingDualWeapon) {
			currentRightWeaponSystem.activateBackwardAcion ();
			currentLeftWeaponSystem.activateBackwardAcion ();
		} else {
			if (currentWeaponSystem != null) {
				currentWeaponSystem.activateBackwardAcion ();
			}
		}
	}

	//a quick function to keep the current weapon that the player is using (if he is using one), for example, when he enters in a vehicle. It is called from playerStatesManager
	public void disableCurrentWeapon ()
	{
		if (carryingWeaponInFirstPerson || carryingWeaponInThirdPerson) {
			currentIKWeapon.stopWeaponMovement ();

			if (aimingInThirdPerson) {
				//print ("deactivate");
				activateOrDeactivateAimMode (false);
				IKManager.disableIKWeight ();
			}

			if (aimingInFirstPerson) {
				playerManager.enableOrDisableAiminig (false);
			}

			changeCameraFov (false);

			currentWeaponSystem.setPauseDrawKeepWeaponSound ();

			currentWeaponSystem.setWeaponAimState (false, false);
			currentWeaponSystem.setWeaponCarryState (false, false);

			aimingInFirstPerson = false;
			aimingInThirdPerson = false;

			checkPlayerCanJumpWhenAimingState ();

			if (carryingWeaponInThirdPerson) {
				carryingWeaponInThirdPerson = false;
				currentIKWeapon.quickKeepWeaponThirdPerson ();

				if (!usingDualWeapon) {
					IKManager.quickKeepWeaponState ();

					IKManager.setUsingWeaponsState (false);

					IKManager.setDisableWeaponsState (false);
				}
			} 

			if (carryingWeaponInFirstPerson) {
				carryingWeaponInFirstPerson = false;

				currentIKWeapon.quickKeepWeaponFirstPerson ();

				currentIKWeapon.enableOrDisableFirstPersonArms (false);
			}

			enableOrDisableWeaponsHUD (false);

			currentWeaponSystem.enableHUD (false);

			enableOrDisableWeaponCursor (false);

			playerCameraManager.setOriginalRotationSpeed ();

			if (usingDualWeapon) {
				if (currentRightIKWeapon.isCurrentWeapon () && currentLeftIkWeapon.isCurrentWeapon ()) {
					if (isFirstPersonActive ()) {
						carryingWeaponInFirstPerson = true;
					} else {
						carryingWeaponInThirdPerson = true;
					}
				}
			}
		}
	}

	public void disableCurrentDualWeapon ()
	{
		if (carryingWeaponInFirstPerson || carryingWeaponInThirdPerson) {
			currentRightIKWeapon.stopWeaponMovement ();
			currentLeftIkWeapon.stopWeaponMovement ();

			if (aimingInThirdPerson) {
				//print ("deactivate");
				activateOrDeactivateAimMode (false);

				IKManager.disableIKWeight ();
			}

			if (aimingInFirstPerson) {
				playerManager.enableOrDisableAiminig (false);
			}

			changeCameraFov (false);

			currentRightWeaponSystem.setPauseDrawKeepWeaponSound ();
			currentLeftWeaponSystem.setPauseDrawKeepWeaponSound ();

			currentRightWeaponSystem.setWeaponAimState (false, false);
			currentRightWeaponSystem.setWeaponCarryState (false, false);

			currentLeftWeaponSystem.setWeaponAimState (false, false);
			currentLeftWeaponSystem.setWeaponCarryState (false, false);

			aimingInFirstPerson = false;
			aimingInThirdPerson = false;

			checkPlayerCanJumpWhenAimingState ();

			if (carryingWeaponInThirdPerson) {
				carryingWeaponInThirdPerson = false;

				currentRightIKWeapon.quickKeepWeaponThirdPerson ();
				currentLeftIkWeapon.quickKeepWeaponThirdPerson ();

				if (!usingDualWeapon) {
					IKManager.quickKeepWeaponState ();

					IKManager.setUsingWeaponsState (false);

					IKManager.setDisableWeaponsState (false);
				}
			} 

			if (carryingWeaponInFirstPerson) {
				carryingWeaponInFirstPerson = false;

				currentRightIKWeapon.quickKeepWeaponFirstPerson ();
				currentRightIKWeapon.enableOrDisableFirstPersonArms (false);

				currentLeftIkWeapon.quickKeepWeaponFirstPerson ();
				currentLeftIkWeapon.enableOrDisableFirstPersonArms (false);
			}

			enableOrDisableWeaponsHUD (false);

			currentWeaponSystem.enableHUD (false);

			enableOrDisableWeaponCursor (false);

			playerCameraManager.setOriginalRotationSpeed ();

			setUsingDualWeaponsState (false);

			disableDualWeaponStateOnWeapons ();
		}
	}

	public void checkIfDisableCurrentWeapon ()
	{
		if (carryingWeaponInFirstPerson || carryingWeaponInThirdPerson) {
			if (usingDualWeapon) {

				usingDualWeaponsPreviously = true;
				disableCurrentDualWeapon ();
			} else {
				disableCurrentWeapon ();
			}
		}
	}

	public void resetWeaponHandIKWeight ()
	{
		IKManager.resetWeaponHandIKWeight ();
	}

	//set the weapons mode in third or first person from the player camera component, only in editor mode
	public void getPlayerWeaponsManagerComponents (bool isFirstPerson)
	{
		getComponents ();

		anyWeaponAvailable = checkAndSetWeaponsAvailable ();

		if (anyWeaponAvailable) {
			getCurrentWeapon ();
			getCurrentWeaponRotation (currentIKWeapon);
		}

		originalFov = mainCamera.fieldOfView;

		touchPlatform = touchJoystick.checkTouchPlatform ();

		if (!weaponsCameraLocated) {
			weaponsCameraLocated = weaponsCamera != null;
		}

		if (weaponsCameraLocated) {
			originalWeaponsCameraFov = weaponsCamera.fieldOfView;
		}
	}

	//get the components in player used in the script
	public void getComponents ()
	{
		originalWeaponsPositionThirdPerson = weaponsTransformInThirdPerson.localPosition;
		originalWeaponsRotationThirdPerson = weaponsTransformInThirdPerson.localRotation;

		originalWeaponsPositionFirstPerson = weaponsTransformInFirstPerson.localPosition;
		originalWeaponsRotationFirstPerson = weaponsTransformInFirstPerson.localRotation;
	}

	public Transform getTemporalParentForWeapons ()
	{
		return temporalParentForWeapons;
	}

	public Transform getPlayerManagersParentGameObject ()
	{
		return playerManager.getPlayerManagersParentGameObject ().transform;
	}

	public Transform getWeaponsParent ()
	{
		return weaponsParent;
	}

	public Transform getMainCameraTransform ()
	{
		if (!mainCameraAssigned) {
			mainCameraAssigned = mainCamera != null;

			if (!mainCameraAssigned) {
				mainCamera = playerCameraManager.getMainCamera ();

				mainCameraTransform = mainCamera.transform;

				mainCameraAssigned = true;
			}
		}

		return mainCameraTransform;
	}

	public Camera getMainCamera ()
	{
		if (!mainCameraAssigned) {
			mainCameraAssigned = mainCamera != null;

			if (!mainCameraAssigned) {
				mainCamera = playerCameraManager.getMainCamera ();

				mainCameraTransform = mainCamera.transform;

				mainCameraAssigned = true;
			}
		}

		return mainCamera;
	}

	public bool isUsingScreenSpaceCamera ()
	{
		return playerCameraManager.isUsingScreenSpaceCamera ();
	}

	public bool isCameraPlacedInFirstPerson ()
	{
		return playerCameraManager.isCameraPlacedInFirstPerson ();
	}

	public bool isCameraTypeFree ()
	{
		return playerCameraManager.isCameraTypeFree ();
	}

	public Vector3 getCurrentNormal ()
	{
		return playerManager.getCurrentNormal ();
	}

	public GameObject getPlayerGameObject ()
	{
		return playerManager.gameObject;
	}

	public Vector2 getMainCanvasSizeDelta ()
	{
		return playerCameraManager.getMainCanvasSizeDelta ();
	}

	public void changeCameraStateInPauseManager (bool state)
	{
		pauseManager.changeCameraState (state);
	}

	//change the weapons parent between third and first person
	public void setWeaponsParent (bool isFirstPerson, bool settingInEditor, bool addingOnRunTime)
	{
		//###################################################
		if (!isFirstPerson && !settingInEditor) {
			if (!weaponsCameraLocated) {
				weaponsCameraLocated = weaponsCamera != null;
			}

			if (weaponsCameraLocated) {
				weaponsCamera.enabled = isFirstPerson;
			}
		}

		string newLayer = "Default";

		if (isFirstPerson) {
			if (!settingInEditor) {
				weaponsParent.SetParent (firstPersonParent);

				weaponsParent.localRotation = originalWeaponsRotationFirstPerson;
				weaponsParent.localPosition = originalWeaponsPositionFirstPerson;
			}

			newLayer = weaponsLayer;
		} else {
			if (!settingInEditor) {
				weaponsParent.SetParent (thirdPersonParent);

				weaponsParent.localPosition = originalWeaponsPositionThirdPerson;
				weaponsParent.localRotation = originalWeaponsRotationThirdPerson;
			}
		}

		int newLayerIndex = LayerMask.NameToLayer (newLayer);

		updateWeaponListCount ();

		for (int k = 0; k < weaponListCount; k++) {
			playerWeaponSystem currentPlayerWeaponSystem = weaponsList [k].getWeaponSystemManager ();

			currentPlayerWeaponSystem.enableHUDTemporarily (true);

			Component[] components = currentPlayerWeaponSystem.weaponSettings.weaponMesh.GetComponentsInChildren (typeof(Transform));
			foreach (Transform child in components) {
				child.gameObject.layer = newLayerIndex;
			}

			currentPlayerWeaponSystem.enableHUDTemporarily (false);

			if (weaponsList [k].isWeaponEnabled ()) {
				if (isFirstPerson || !weaponsList [k].hideWeaponIfKeptInThirdPerson) {
					weaponsList [k].enableOrDisableWeaponMesh (!isFirstPerson);
				}
			}

			Transform weaponTransform = weaponsList [k].weaponTransform;
			if (weaponTransform == null) {
				weaponTransform = weaponsList [k].weaponGameObject.transform;
			}

			if (!isFirstPerson) {
				if (!settingInEditor && !addingOnRunTime) {
					weaponTransform.SetParent (weaponsList [k].thirdPersonWeaponInfo.keepPosition.parent);

					weaponTransform.localPosition = weaponsList [k].thirdPersonWeaponInfo.keepPosition.localPosition;
					weaponTransform.localRotation = weaponsList [k].thirdPersonWeaponInfo.keepPosition.localRotation;
				}

				weaponsList [k].enableOrDisableFirstPersonArms (false);

				weaponsList [k].resetWeaponMeshTransform ();
			} else {
				if (!settingInEditor && !addingOnRunTime) {
					weaponTransform.SetParent (weaponsList [k].transform);

					weaponTransform.localPosition = weaponsList [k].firstPersonWeaponInfo.keepPosition.localPosition;
					weaponTransform.localRotation = weaponsList [k].firstPersonWeaponInfo.keepPosition.localRotation;
				}
			}

			currentPlayerWeaponSystem.changeHUDPosition (!isFirstPerson);

			weaponsList [k].setCurrentSwayInfo (true);
		}

		if (!addingOnRunTime) {
			IKManager.stopIKWeaponsActions ();
		}
	}

	public void setWeaponPartLayer (GameObject weaponPart)
	{
		setWeaponPartLayerFromCameraView (weaponPart, isFirstPersonActive ());
	}

	public void setWeaponPartLayerFromCameraView (GameObject weaponPart, bool firstPersonActive)
	{
		string newLayer = "Default";

		if (firstPersonActive) {
			newLayer = weaponsLayer;
		} 

		int newLayerIndex = LayerMask.NameToLayer (newLayer);

		Component[] components = weaponPart.GetComponentsInChildren (typeof(Transform));
		foreach (Component c in components) {
			c.gameObject.layer = newLayerIndex;
		}
	}

	//check if the player is carrying weapons in third or first person
	public bool isUsingWeapons ()
	{
		return carryingWeaponInFirstPerson || carryingWeaponInThirdPerson;
	}

	public bool isCarryingWeaponInThirdPerson ()
	{
		return carryingWeaponInThirdPerson;
	}

	public bool isAimingWeapons ()
	{
		return aimingInFirstPerson || aimingInThirdPerson;
	}

	public bool isAimingInThirdPerson ()
	{
		return aimingInThirdPerson;
	}

	public void setWeaponToStartGame (string weaponName)
	{
		setWeaponByName (weaponName);
	}

	public void setWeaponByName (string weaponName)
	{
		currentIKWeapon.setCurrentWeaponState (false);

		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName)) {

				weaponsList [i].setCurrentWeaponState (true);

				anyWeaponAvailable = true;
				choosedWeapon = i;

				currentWeaponSystem = weaponsList [i].getWeaponSystemManager ();

				currentIKWeapon = weaponsList [i];

				checkIfCurrentIkWeaponNotNull ();

				currentWeaponName = currentWeaponSystem.getWeaponSystemName ();

				if (showDebugLog) {
					print ("current IK Weapon " + currentIKWeapon.getWeaponSystemName ());
				}
			} 
		}
	}

	public playerWeaponSystem getWeaponSystemByName (string weaponName)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName)) {
				return weaponsList [i].getWeaponSystemManager ();
			}
		}

		return null;
	}

	public int getWeaponIndexByName (string weaponName)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName)) {
				return i;
			}
		}

		return -1;
	}

	public void changeCurrentWeaponByName (string weaponName)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].getWeaponSystemName ().Equals (weaponName)) {
				checkWeaponToChangeByIndex (weaponsList [i], weaponsList [i].getWeaponSystemKeyNumber (), weaponsList [i].getWeaponSystemKeyNumber (), i);
			}
		}
	}

	public void changeCurrentWeaponByNameCheckingIfReady (string weaponName)
	{
		if (currentIKWeapon != null) {
			if (reloadingWithAnimationActive ||
			    currentIKWeapon.isWeaponMoving () ||
			    playerManager.isGamePaused () ||
			    currentIKWeapon.isReloadingWeapon () ||
			    weaponsAreMoving () ||
			    Time.time < lastTimeReload + 0.5f) {

				return;
			}

			updateWeaponListCount ();

			for (int i = 0; i < weaponListCount; i++) {
				if (weaponsList [i].getWeaponSystemName ().Equals (weaponName)) {
					checkWeaponToChangeByIndex (weaponsList [i], weaponsList [i].getWeaponSystemKeyNumber (), weaponsList [i].getWeaponSystemKeyNumber (), i);
				}
			}
		}
	}

	public void enableOrDisableGrabObjectsManager (bool state)
	{
		if (canGrabObjectsCarryingWeapons) {
			if (grabObjectsManager != null) {
				
				grabObjectsManager.setAimingState (state);

				if (!state) {
					grabObjectsManager.dropObject ();
				}
			}
		}
	}

	public void setCarryingPhysicalObjectState (bool state)
	{
		carryingPhysicalObject = state;
	}

	public void setShotCameraNoise (Vector2 noiseAmount)
	{
		playerCameraManager.setShotCameraNoise (noiseAmount);
	}

	public void setCameraPositionMouseWheelEnabledState (bool state)
	{
		playerCameraManager.setCameraPositionMouseWheelEnabledState (state);
	}

	public bool isCheckDurabilityOnObjectEnabled ()
	{
		return checkDurabilityOnObjectEnabled;
	}

	public void showCantPickAttacmentMessage (string attachmentName)
	{
		showObjectMessage (attachmentName + " " + cantPickAttachmentMessage, weaponMessageDuration, weaponsMessageWindow);
	}

	Coroutine weaponMessageCoroutine;

	public void showObjectMessage (string message, float messageDuration, GameObject messagePanel)
	{
		stopShowObjectMessageCoroutine ();

		weaponMessageCoroutine = StartCoroutine (showObjectMessageCoroutine (message, messageDuration, messagePanel));
	}

	void stopShowObjectMessageCoroutine ()
	{
		if (weaponMessageCoroutine != null) {
			StopCoroutine (weaponMessageCoroutine);
		}
	}

	IEnumerator showObjectMessageCoroutine (string info, float messageDuration, GameObject messagePanel)
	{
		usingDevicesManager.checkDeviceName ();

		if (!messagePanel.activeSelf) {
			messagePanel.SetActive (true);
		}

		messagePanel.GetComponentInChildren<Text> ().text = info;

		yield return new WaitForSeconds (messageDuration);

		messagePanel.SetActive (false);
	}

	public List<IKWeaponSystem> getPlayerWeaponList ()
	{
		return weaponsList;
	}

	public IKWeaponSystem getIKWeaponSystem (string weaponName)
	{
		updateWeaponListCount ();

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].getWeaponSystemName ().Equals (weaponName)) {
				return weaponsList [k];
			}
		}

		return null;
	}

	public void setDrawWeaponsPausedState (bool state)
	{
		drawWeaponsPaused = state;
	}

	public void setWeaponsModeActive (bool state)
	{
		bool weaponsModeActivePreviously = weaponsModeActive;

		weaponsModeActive = state;

		if (drawKeepWeaponWhenModeChanged) {
			if (canMove && !playerIsBusy () && checkIfWeaponsAvailable () && (startInitialized || !notActivateWeaponsAtStart)) {
				
				checkTypeView ();

				if (weaponsModeActive) {
					if (!drawWeaponsPaused) {
						if (!weaponsModeActivePreviously) {
							if (!isUsingWeapons ()) {
								checkIfDrawSingleOrDualWeapon ();
							}
						}
					}

					drawWeaponsPaused = false;
				} else {
					checkIfKeepSingleOrDualWeapon ();

					disableFreeFireModeState ();
				}
			}
		}

		if (weaponsModeActivePreviously != weaponsModeActive) {
			checkEventsOnStateChange (weaponsModeActive);

			if (storePickedWeaponsOnInventory) {
				playerInventoryManager.checkToEnableOrDisableQuickAccessSlotsParentOutOfInventoryFromFireWeaponsMode (weaponsModeActive);
			}
		}
	}

	public void checkIfDrawWeaponWithAnimationAfterAction ()
	{
		if (weaponsModeActive) {
			if (!isFirstPersonActive ()) {
				if (currentIKWeapon != null) {
					if (currentIKWeapon.isCurrentWeapon ()) {
						bool useDrawKeepWeaponAnimation = currentIKWeapon.isUseDrawKeepWeaponAnimationActive ();

						setActivateWaitOnDrawWeaponWithAnimationOnThirdPersonState (useDrawKeepWeaponAnimation);
					}
				}
			}
		}
	}

	public bool activateWaitOnDrawWeaponWithAnimationOnThirdPerson;

	public void setActivateWaitOnDrawWeaponWithAnimationOnThirdPersonState (bool state)
	{
		activateWaitOnDrawWeaponWithAnimationOnThirdPerson = state;
	}

	public void checkIfDrawSingleOrDualWeapon ()
	{
		bool drawDualWeaponsCorrectly = false;

		if (usingDualWeaponsPreviously) {
			usingDualWeaponsPreviously = false;

			if (currentRightWeaponSystem != null && currentLeftWeaponSystem != null) {

				drawDualWeapons ();

				drawDualWeaponsCorrectly = true;
			}
		} 

		if (!drawDualWeaponsCorrectly) {
			if (currentIKWeapon != null) {
				if (currentIKWeapon.isCurrentWeapon ()) {
					if (activateWaitOnDrawWeaponWithAnimationOnThirdPerson) {
						StartCoroutine (drawOrKeepWeaponThirdPersonCoroutine ());

						activateWaitOnDrawWeaponWithAnimationOnThirdPerson = false;
					} else {
						drawOrKeepWeaponInput ();
					}
				}
			}
		}
	}

	IEnumerator drawOrKeepWeaponThirdPersonCoroutine ()
	{
		activateWaitOnDrawWeaponWithAnimationOnThirdPerson = false;

		bool previousGeneralWeaponsInputActiveValue = generalWeaponsInputActive;

		setGeneralWeaponsInputActiveState (false);

		yield return new WaitForSeconds (0.6f);

		setGeneralWeaponsInputActiveState (previousGeneralWeaponsInputActiveValue);

		if (canMove && weaponsModeActive && generalWeaponsInputActive) {
			drawOrKeepWeaponInput ();
		}
	}

	public void checkIfKeepSingleOrDualWeapon ()
	{
		if (isPlayerCarringWeapon ()) {

			if (usingDualWeapon) {
				keepDualWeapons ();

				usingDualWeaponsPreviously = true;
			} else {
				drawOrKeepWeaponInput ();
			}
		} else {
			if (isUsingWeapons ()) {
				if (usingDualWeapon) {
					disableCurrentDualWeapon ();

					usingDualWeaponsPreviously = true;
				} else {
					disableCurrentWeapon ();
				}
			}
		}
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				evenOnStateEnabled.Invoke ();
			} else {
				eventOnStateDisabled.Invoke ();
			}
		}
	}

	public bool isWeaponsModeActive ()
	{
		return weaponsModeActive;
	}

	public bool isActivateDualWeaponsEnabled ()
	{
		return activateDualWeaponsEnabled;
	}

	public void selectWeaponByName (string weaponName, bool drawSelectedWeapon)
	{
		if (!currentIKWeapon.isWeaponMoving () && !weaponsAreMoving ()) {
			updateWeaponListCount ();

			for (int k = 0; k < weaponListCount; k++) {
				if (weaponsList [k].getWeaponSystemName ().Equals (weaponName) && (choosedWeapon != k || drawSelectedWeapon) && weaponsList [k].isWeaponEnabled ()) {

					if (choosedWeapon == k && (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson)) {
						return;
					}

					choosedWeapon = k;

					currentIKWeapon.setCurrentWeaponState (false);

					if (drawSelectedWeapon) {
						if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
							if (useQuickDrawWeapon && isThirdPersonView) {
								quicChangeWeaponThirdPersonAction ();
							} else {
								setChangingWeaponState (true);
							}
						} else {
							if (isWeaponsModeActive ()) {
								weaponChanged ();
								drawOrKeepWeaponInput ();
							}
						}
					} else {
						if (carryingWeaponInThirdPerson || carryingWeaponInFirstPerson) {
							if (useQuickDrawWeapon && isThirdPersonView) {
								quicChangeWeaponThirdPersonAction ();
							} else {
								setChangingWeaponState (true);
							}
						} else {
							weaponChanged ();
						}
					}
				}
			}
		}
	}

	public void setCurrentRightIkWeaponByName (string weaponName)
	{
		updateWeaponListCount ();

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].getWeaponSystemName ().Equals (weaponName) && weaponsList [k].isWeaponEnabled ()) {

				if (currentRightIKWeapon != null) {
					currentRightIKWeapon.setCurrentWeaponState (false);
				}

				currentRightIKWeapon = weaponsList [k];
				currentRightWeaponSystem = weaponsList [k].getWeaponSystemManager ();
			}
		}
	}

	public void setCurrentLeftIKWeaponByName (string weaponName)
	{
		updateWeaponListCount ();

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].getWeaponSystemName ().Equals (weaponName) && weaponsList [k].isWeaponEnabled ()) {

				if (currentLeftIkWeapon != null) {
					currentLeftIkWeapon.setCurrentWeaponState (false);
				}

				currentLeftIkWeapon = weaponsList [k];
				currentLeftWeaponSystem = weaponsList [k].getWeaponSystemManager ();
			}
		}
	}

	public void keepDualWeapons ()
	{
		currentRightWeaponSystem.setWeaponCarryStateAtOnce (true);
		currentLeftWeaponSystem.setWeaponCarryStateAtOnce (true);

		drawRightWeapon ();
		drawLeftWeapon ();
	}

	public void drawDualWeapons ()
	{
		currentRightWeaponSystem.setWeaponCarryStateAtOnce (false);
		currentLeftWeaponSystem.setWeaponCarryStateAtOnce (false);

		if (playerIsDead) {
			return;
		}

		if (equippingDualWeaponsFromInventoryMenu) {
			drawRightWeapon ();
			drawLeftWeapon ();

			equippingDualWeaponsFromInventoryMenu = false;
		} else {
			
			updateCanMoveValue ();

			if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable) {
				drawRightWeapon ();
				drawLeftWeapon ();
			}
		}
	}

	public void drawDualWeaponsIfNotCarryingWeapons ()
	{
		if (!activateDualWeaponsEnabled) {
			return;
		}

		if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable && !usingDualWeapon) {
			string rightWeaponToUseName = currentIKWeapon.getWeaponSystemName ();
			string leftWeaponToUseName = "";

			if (currentIKWeapon.isWeaponEnabled ()) {
				if (storePickedWeaponsOnInventory) {
					leftWeaponToUseName = playerInventoryManager.getFirstSingleWeaponSlot (rightWeaponToUseName);

					if (leftWeaponToUseName != "") {
						changeDualWeapons (rightWeaponToUseName, leftWeaponToUseName);

						updateWeaponSlotInfo ();
					}
				} else {

					updateWeaponListCount ();

					for (int k = 0; k < weaponListCount; k++) {
						leftWeaponToUseName = weaponsList [k].getWeaponSystemName ();

						if (!weaponsList [k].isWeaponConfiguredAsDualWeapon () && leftWeaponToUseName != rightWeaponToUseName) {
						
							changeDualWeapons (rightWeaponToUseName, leftWeaponToUseName);

							updateWeaponSlotInfo ();

							return;
						}
					}
				}
			}
		}
	}

	public void removeWeaponConfiguredAsDualWeaponState (int keyNumberToSearch)
	{
		updateWeaponListCount ();

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].isWeaponEnabled ()) {
				if (weaponsList [k].getWeaponSystemKeyNumber () == keyNumberToSearch) {
					weaponsList [k].setWeaponConfiguredAsDualWepaonState (false, "");
				}
			}
		}
	}

	void setChangingWeaponState (bool state)
	{
		changingWeapon = state;
	}

	public void changeDualWeapons (string rightWeaponName, string lefWeaponName)
	{
		carryingSingleWeaponPreviously = false;

		if (isUsingWeapons ()) {
			if (!usingDualWeapon) {
				previousSingleIKWeapon = currentIKWeapon;

				carryingSingleWeaponPreviously = true;
				drawOrKeepWeaponInput ();

				setUsingDualWeaponsState (true);

			} else {

				if (currentRightIKWeapon != null && currentRightIKWeapon.getWeaponSystemName ().Equals (rightWeaponName)) {
					currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
				}

				if (currentLeftIkWeapon != null && currentLeftIkWeapon.getWeaponSystemName ().Equals (lefWeaponName)) {
					currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
				}

				carryingDualWeaponsPreviously = true;

				previousRightIKWeapon = currentRightIKWeapon;
				previousLeftIKWeapon = currentLeftIkWeapon;

				keepDualWeapons ();
			}
		} else {
			if (!equippingDualWeaponsFromInventoryMenu) {
				previousSingleIKWeapon = currentIKWeapon;
			}
		}

		currentRighWeaponName = rightWeaponName;
		currentLeftWeaponName = lefWeaponName;

		setCurrentRightIkWeaponByName (rightWeaponName);
		setCurrentLeftIKWeaponByName (lefWeaponName);
	
		currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (true, currentLeftIkWeapon.getWeaponSystemName ());
		currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (true, currentRightIKWeapon.getWeaponSystemName ());

		changingDualWeapon = true;
		keepingWeapon = true;

		setChangingWeaponState (true);
	}

	public void changeSingleWeapon (string singleWeaponName)
	{
		if (showDebugLog) {
			print ("set single weapon " + singleWeaponName);
		}

		carryingDualWeaponsPreviously = false;

		if (isUsingWeapons ()) {
			if (usingDualWeapon) {

				carryingDualWeaponsPreviously = true;

				keepDualWeapons ();
			}
		}

		setUsingDualWeaponsState (false);

		disableDualWeaponStateOnWeapons ();

//		setWeaponByName (singleWeaponName);

		changingSingleWeapon = true;
		changingDualWeapon = false;
		keepingWeapon = true;

		setChangingWeaponState (true);
	}

	public void separateDualWeapons ()
	{
		carryingDualWeaponsPreviously = false;

		if (isUsingWeapons ()) {
			if (usingDualWeapon) {

				carryingDualWeaponsPreviously = true;

				keepDualWeapons ();
			}
		}

		setUsingDualWeaponsState (false);

		disableDualWeaponStateOnWeapons ();

		currentRightIKWeapon.setWeaponConfiguredAsDualWepaonState (false, "");
		currentLeftIkWeapon.setWeaponConfiguredAsDualWepaonState (false, "");

		changingSingleWeapon = true;
		changingDualWeapon = false;
		keepingWeapon = true;

		setChangingWeaponState (true);

		updateSingleWeaponSlotInfo ();
	}

	public void disableDualWeaponStateOnWeapons ()
	{
		mainWeaponListManager.setSelectingWeaponState (false);

		currentRightIKWeapon.setUsingDualWeaponState (false);
		currentLeftIkWeapon.setUsingDualWeaponState (false);

		currentRightIKWeapon.disableUsingDualWeaponState ();
		currentLeftIkWeapon.disableUsingDualWeaponState ();

		IKManager.setUsingDualWeaponState (false);
	}

	public void updateWeaponSlotInfo ()
	{
		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.updateDualWeaponSlotInfo (currentRighWeaponName, currentLeftWeaponName);

			updateCurrentChoosedDualWeaponIndex ();

			playerInventoryManager.updateWeaponCurrentlySelectedIcon (chooseDualWeaponIndex, true);
		}
	}

	public void updateSingleWeaponSlotInfo ()
	{
		if (storePickedWeaponsOnInventory) {
			playerInventoryManager.updateSingleWeaponSlotInfo (currentRighWeaponName, currentLeftWeaponName);

			updateCurrentChoosedDualWeaponIndex ();

			playerInventoryManager.updateWeaponCurrentlySelectedIcon (chooseDualWeaponIndex, true);
		}
	}

	public void updateCurrentChoosedDualWeaponIndex ()
	{
		if (isUsingWeapons ()) {
			if (usingDualWeapon) {
				chooseDualWeaponIndex = currentRightWeaponSystem.getWeaponNumberKey ();
			} else {
				chooseDualWeaponIndex = currentWeaponSystem.getWeaponNumberKey ();
			}
		}
	}

	public void checkIfWeaponUseAmmoFromInventory ()
	{
		if (isUsingWeapons ()) {
			if (usingDualWeapon) {
				setWeaponRemainAmmoFromInventory (currentRightWeaponSystem);
				setWeaponRemainAmmoFromInventory (currentLeftWeaponSystem);
			} else {
				setWeaponRemainAmmoFromInventory (currentWeaponSystem);
			}
		}
	}

	public void setWeaponRemainAmmoFromInventory (playerWeaponSystem weaponSystemToCheck)
	{
		if (weaponSystemToCheck.isUseRemainAmmoFromInventoryActive ()) {	
			int currentAmmoFromInventory = playerInventoryManager.getInventoryObjectAmountByName (weaponSystemToCheck.getWeaponSystemAmmoName ());

			if (currentAmmoFromInventory < 0) {
				currentAmmoFromInventory = 0;
			}

			weaponSystemToCheck.setRemainAmmoAmount (currentAmmoFromInventory);
		}
	}

	public void useAmmoFromInventory (string weaponName, int amountToUse)
	{
		playerInventoryManager.useAmmoFromInventory (weaponName, amountToUse);
	}

	public bool isUseAmmoFromInventoryInAllWeaponsActive ()
	{
		return useAmmoFromInventoryInAllWeapons;
	}

	public int getNumberOfWeaponsAvailable ()
	{
		int numberOfWeaponsAvailable = 0;

		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].isWeaponEnabled ()) {
				numberOfWeaponsAvailable++;
			}
		}

		return numberOfWeaponsAvailable;
	}

	public playerWeaponSystem getCurrentWeaponSystem ()
	{
		return currentWeaponSystem;
	}

	public bool isPlayerUsingWeapon (string weaponNameToCheck)
	{
		if (usingDualWeapon) {
			if (currentRighWeaponName.Equals (weaponNameToCheck) || currentLeftWeaponName.Equals (weaponNameToCheck)) {
				return true;
			}
		} else {
			if (currentWeaponName.Equals (weaponNameToCheck)) {
				return true;
			}
		}

		return false;
	}

	public void editWeaponAttachments ()
	{
		if ((!playerManager.isPlayerMenuActive () || editingWeaponAttachments) &&
		    !playerManager.isUsingDevice () &&
		    !playerManager.isGamePaused () &&
		    (!usingDevicesManager.hasDeviceToUse () || editingWeaponAttachments)) {

			if (usingDualWeapon || currentIKWeapon != null) {

				if (usingDualWeapon) {
					currentRightWeaponAttachmentSystem = currentRightIKWeapon.getWeaponAttachmentSystem ();
					currentLeftWeaponAttachmentSystem = currentLeftIkWeapon.getWeaponAttachmentSystem ();
					 
					if (currentRightWeaponAttachmentSystem != null || currentLeftWeaponAttachmentSystem != null) {

						bool canOpenRightWeaponAttachments = false;
						bool canOpenLeftWeaponAttachments = false;

						if (currentRightWeaponAttachmentSystem && currentRightWeaponAttachmentSystem.canBeOpened ()) {
							canOpenRightWeaponAttachments = true;
						}

						if (currentLeftWeaponAttachmentSystem && currentLeftWeaponAttachmentSystem.canBeOpened ()) {
							canOpenLeftWeaponAttachments = true;
						}

						if (canOpenRightWeaponAttachments || canOpenLeftWeaponAttachments) {
							editingWeaponAttachments = !editingWeaponAttachments;

							pauseManager.setIngameMenuOpenedState ("Player Weapons Manager", editingWeaponAttachments, true);
						}

						if (canOpenRightWeaponAttachments) {
							if (currentLeftWeaponAttachmentSystem) {
								currentRightWeaponAttachmentSystem.setSecondaryWeaponAttachmentSystem (currentLeftWeaponAttachmentSystem);
							}

							currentRightWeaponAttachmentSystem.setUsingDualWeaponState (true);

							currentRightWeaponAttachmentSystem.openOrCloseWeaponAttachmentEditor (editingWeaponAttachments);
						}

						if (canOpenLeftWeaponAttachments) {
							currentLeftWeaponAttachmentSystem.setAdjustThirdPersonCameraActiveState (false);

							currentLeftWeaponAttachmentSystem.setUsingDualWeaponState (true);

							currentLeftWeaponAttachmentSystem.openOrCloseWeaponAttachmentEditor (editingWeaponAttachments);
						}
					}
				} else {
					currentWeaponAttachmentSystem = currentIKWeapon.getWeaponAttachmentSystem ();

					if (currentWeaponAttachmentSystem != null) {
						if (currentWeaponAttachmentSystem.canBeOpened ()) {
							editingWeaponAttachments = !editingWeaponAttachments;

							pauseManager.setIngameMenuOpenedState ("Player Weapons Manager", editingWeaponAttachments, true);

							if (currentLeftWeaponAttachmentSystem) {
								currentLeftWeaponAttachmentSystem.setAdjustThirdPersonCameraActiveState (true);
							}
							
							currentWeaponAttachmentSystem.setUsingDualWeaponState (false);

							currentWeaponAttachmentSystem.openOrCloseWeaponAttachmentEditor (editingWeaponAttachments);
						}
					}
				}
			}
		}
	}

	public void openOrCloseWeaponAttachmentEditor (bool state, bool disableHUDWhenEditingAttachments)
	{
		pauseManager.showOrHideCursor (state);

		pauseManager.setHeadBobPausedState (state);

		if (disableHUDWhenEditingAttachments) {
			pauseManager.enableOrDisablePlayerHUD (!state);
		}

		grabObjectsManager.enableOrDisableGeneralCursorFromExternalComponent (!state);

		enableOrDisableGeneralWeaponCursor (!state);

		headTrackManager.setSmoothHeadTrackDisableState (state);

		playerManager.setHeadTrackCanBeUsedState (!state);

		playerManager.changeScriptState (!state);

		pauseManager.openOrClosePlayerMenu (state, null, false);

		pauseManager.enableOrDisableDynamicElementsOnScreen (!state);

		pauseManager.showOrHideMouseCursorController (state);

		pauseManager.checkEnableOrDisableTouchZoneList (!state);
	}

	public void cancelWeaponAttachmentEditor (bool disableHUDWhenEditingAttachments)
	{
		if (editingWeaponAttachments) {

			editingWeaponAttachments = false;

			pauseManager.showOrHideCursor (editingWeaponAttachments);

			pauseManager.changeCameraState (!editingWeaponAttachments);

			pauseManager.setHeadBobPausedState (editingWeaponAttachments);

			pauseManager.enableOrDisableDynamicElementsOnScreen (!editingWeaponAttachments);

			if (disableHUDWhenEditingAttachments) {
				pauseManager.enableOrDisablePlayerHUD (!editingWeaponAttachments);
			}

			pauseManager.checkEnableOrDisableTouchZoneList (!editingWeaponAttachments);
		}
	}

	public void setChangeTypeOfViewState ()
	{
		playerCameraManager.changeTypeView ();
	}

	public bool isEditinWeaponAttachments ()
	{
		return editingWeaponAttachments;
	}

	public void checkPressedAttachmentButton (Button pressedButton)
	{
		if (usingDualWeapon) {
			if (currentRightWeaponAttachmentSystem != null) {
				currentRightWeaponAttachmentSystem.checkPressedAttachmentButton (pressedButton);
			}

			if (currentLeftWeaponAttachmentSystem != null) {
				currentLeftWeaponAttachmentSystem.checkPressedAttachmentButton (pressedButton);
			}
		} else {
			if (currentIKWeapon != null) {
				currentWeaponAttachmentSystem = currentIKWeapon.getWeaponAttachmentSystem ();

				if (currentWeaponAttachmentSystem != null) {
					currentWeaponAttachmentSystem.checkPressedAttachmentButton (pressedButton);
				}
			}
		}
	}

	public void setAttachmentPanelState (bool state)
	{
		if (attachmentPanel != null && attachmentPanel.activeSelf != state) {
			attachmentPanel.SetActive (state);
		}
	}

	public void setRightWeaponAttachmentPanelState (bool state)
	{
		if (rightAttachmentPanel != null && rightAttachmentPanel.activeSelf != state) {
			rightAttachmentPanel.SetActive (state);
		}
	}

	public void setLeftWeaponAttachmentPanelState (bool state)
	{
		if (leftAttachmentPanel != null && leftAttachmentPanel.activeSelf != state) {
			leftAttachmentPanel.SetActive (state);
		}
	}

	public void setAttachmentIcon (Texture attachmentIcon)
	{
		if (currentAttachmentIcon != null) {
			currentAttachmentIcon.texture = attachmentIcon;
		}
	}

	public void setAttachmentPanelAmmoText (string ammoText)
	{
		if (attachmentAmmoText != null) {
			attachmentAmmoText.text = ammoText;
		}
	}

	//functions to change current weapon stats
	public void setCurrentWeaponSystemWithAttachment (playerWeaponSystem weaponToConfigure)
	{
		currentWeaponSystemWithAttachment = weaponToConfigure;
//		print ("current weapon with attachments: " + currentWeaponSystemWithAttachment.getWeaponSystemName () + " " + gameObject.name);
	}

	public void setNumberKey (int newNumberKey)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setNumberKey (newNumberKey);
		}
	}

	public void setMagazineSize (int magazineSize)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setMagazineSize (magazineSize);
		}
	}

	public void setOriginalMagazineSize ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalMagazineSize ();
		}
	}

	public void setSilencerState (bool state)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setSilencerState (state);
		}
	}

	public void setAutomaticFireMode (bool state)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setAutomaticFireMode (state);
		}
	}

	public void setOriginalAutomaticFireMode ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalAutomaticFireMode ();
		}
	}

	public void setFireRate (float newFireRate)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setFireRate (newFireRate);
		}
	}

	public void setOriginalFireRate ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalFireRate ();
		}
	}

	public void setProjectileDamage (float newDamage)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setProjectileDamage (newDamage);
		}
	}

	public void setOriginalProjectileDamage ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalProjectileDamage ();
		}
	}

	public void setBurstModeState (bool state)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setBurstModeState (state);
		}
	}

	public void setOriginalBurstMode ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalBurstMode ();
		}
	}

	public void setUsingSightState (bool state)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setUsingSightState (state);
		}
	}

	public void setProjectileDamageMultiplier (float multiplier)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setProjectileDamageMultiplier (multiplier);
		}
	}

	public void setSpreadState (bool state)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setSpreadState (state);
		}
	}

	public void setOriginalSpreadState ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalSpreadState ();
		}
	}

	public void setExplosiveAmmoState (bool state)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setExplosiveAmmoState (state);
		}
	}

	public void setOriginalExplosiveAmmoState ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalExplosiveAmmoState ();
		}
	}

	public void setDamageOverTimeAmmoState (bool state)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setDamageOverTimeAmmoState (state);
		}
	}

	public void setOriginalDamageOverTimeAmmoState ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalDamageOverTimeAmmoState ();
		}
	}

	public void setRemoveDamageOverTimeAmmoState (bool state)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setRemoveDamageOverTimeAmmoState (state);
		}
	}

	public void setOriginalRemoveDamageOverTimeAmmoState ()
	{
		currentWeaponSystemWithAttachment.setOriginalRemoveDamageOverTimeAmmoState ();
	}

	public void setSedateAmmoState (bool state)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setSedateAmmoState (state);
		}
	}

	public void setOriginalSedateAmmoState ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalSedateAmmoState ();
		}
	}

	public void setPushCharacterState (bool state)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setPushCharacterState (state);
		}
	}

	public void setOriginalPushCharacterState ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalPushCharacterState ();
		}
	}

	public void setNewWeaponProjectile (GameObject newProjectile)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setNewWeaponProjectile (newProjectile);
		}
	}

	public void setOriginalWeaponProjectile ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalWeaponProjectile ();
		}
	}

	public void setProjectileWithAbilityState (bool newValue)
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setProjectileWithAbilityState (newValue);
		}
	}

	public void setOriginalProjectileWithAbilityValue ()
	{
		if (currentWeaponSystemWithAttachment != null) {
			currentWeaponSystemWithAttachment.setOriginalProjectileWithAbilityValue ();
		}
	}

	public void enableOrDisableInfiniteAmmoOnAllWeapons (bool value)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i] != null) {
				weaponsList [i].weaponSystemManager.setInfiniteAmmoValue (value);
			}
		}
	}

	public bool pickupAttachment (string weaponName, string attachmentName)
	{
		int weaponIndex = -1;

		updateWeaponListCount ();

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponIndex == -1 && weaponsList [k].getWeaponSystemName ().Equals (weaponName)) {
				weaponIndex = k;
			}
		}

		if (weaponIndex > -1) {
			return weaponsList [weaponIndex].pickupAttachment (attachmentName);
		} else {
			if (useUniversalAttachments) {
				if (usingDualWeapon) {
					return currentRightIKWeapon.pickupAttachment (attachmentName) || currentLeftIkWeapon.pickupAttachment (attachmentName);
				} else {
					if (currentIKWeapon != null) {
						return currentIKWeapon.pickupAttachment (attachmentName);
					} else {
						return false;
					}
				}
			}
		}

		return false;
	}

	public bool isUseForwardDirectionOnLaserAttachmentsActive ()
	{
		return useForwardDirectionOnLaserAttachments;
	}

	public void checkSniperSightUsedOnWeapon (bool state)
	{
		if (mainSimpleSniperSightSystem != null) {
			mainSimpleSniperSightSystem.enableOrDisableSniperSight (state);
		}
	}

	public IKSystem getIKSystem ()
	{
		return IKManager;
	}

	public void startOverride ()
	{
		overrideTurretControlState (true);
	}

	public void stopOverride ()
	{
		overrideTurretControlState (false);
	}

	public void overrideTurretControlState (bool state)
	{
		usedByAI = !state;
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	public void lockOrUnlockCursor (bool state)
	{
		cursorLocked = state;

		pauseManager.showOrHideCursor (!cursorLocked);

		pauseManager.changeCameraState (cursorLocked);

		pauseManager.setHeadBobPausedState (!cursorLocked);

		pauseManager.openOrClosePlayerMenu (!cursorLocked, null, false);

		pauseManager.usingDeviceState (!cursorLocked);

		setLastTimeFired ();

		setLastTimeMoved ();

		enableOrDisableGeneralWeaponCursor (cursorLocked);

		grabObjectsManager.enableOrDisableGeneralCursorFromExternalComponent (cursorLocked);

		pauseManager.enableOrDisableDynamicElementsOnScreen (cursorLocked);

		pauseManager.showOrHideMouseCursorController (!cursorLocked);

		bool disableHUDWhenCursorUnlocked = false;

		if (!usingDualWeapon && currentIKWeapon.disableHUDWhenCursorUnlocked) {
			disableHUDWhenCursorUnlocked = true;
		} else if (usingDualWeapon && (currentRightIKWeapon.disableHUDWhenCursorUnlocked || currentLeftIkWeapon.disableHUDWhenCursorUnlocked)) {
			disableHUDWhenCursorUnlocked = true;
		}
			
		if (disableHUDWhenCursorUnlocked) {
			pauseManager.enableOrDisablePlayerHUD (cursorLocked);
		}

		pauseManager.checkEnableOrDisableTouchZoneList (cursorLocked);
	}

	public void lockCursorAgain ()
	{
		if (!cursorLocked) {
			lockOrUnlockCursor (true);
		}
	}

	public bool isCursorLocked ()
	{
		return cursorLocked;
	}

	public void setPlayerControllerCurrentIdleIDValue (int newValue)
	{
		playerManager.setCurrentIdleIDValue (newValue);
	}

	public void setPlayerControllerMovementValues (bool useNewCarrySpeed, float newCarrySpeed)
	{
		if (useNewCarrySpeed) {
			if (isFirstPersonActive ()) {
				playerManager.setNoAnimatorGeneralMovementSpeed (newCarrySpeed, false);
			} else {
				playerManager.setAnimatorGeneralMovementSpeed (newCarrySpeed, false);
			}
		}
	}

	public void setPlayerControllerCanRunValue (bool canRun)
	{
		if (isFirstPersonActive ()) {
			playerManager.setNoAnimatorCanRunState (canRun, false);
		} else {
			playerManager.setAnimatorCanRunState (canRun, false);
		}
	}

	public void setPlayerControllerMovementOriginalValues ()
	{
		if (isFirstPersonActive ()) {
			playerManager.setNoAnimatorGeneralMovementSpeed (0, true);
			playerManager.setNoAnimatorCanRunState (false, true);
		} else {
			playerManager.setAnimatorGeneralMovementSpeed (0, true);
			playerManager.setAnimatorCanRunState (false, true);
		}
	}

	bool canUseInput ()
	{
		if (playerManager.iscloseCombatAttackInProcess ()) {
			return false;
		}

		return true;
	}

	public bool canUseCarriedWeaponsInput ()
	{
		if (!canMove) {
			return false;
		}

		if (!weaponsModeActive) {
			return false;
		}

		if (playerCurrentlyBusy) {
			return false;
		}

		if (!anyWeaponAvailable) {
			return false;
		}

		if (!currentIKWeapon.isCurrentWeapon () && !usingDualWeapon) {
			return false;
		}

		if (playerManager.isUsingGenericModelActive ()) {
			return false;
		}

		if (!canUseInput ()) {
			return false;
		}

		if (!carryingWeaponInThirdPerson && !carryingWeaponInFirstPerson) {
			return false;
		}

		return true;
	}

	public bool canUseWeaponsInput ()
	{
		if (!canMove) {
			return false;
		}

		if (!weaponsModeActive) {
			return false;
		}

		if (playerCurrentlyBusy) {
			return false;
		}

		if (!anyWeaponAvailable) {
			return false;
		}

		if (!currentIKWeapon.isCurrentWeapon () && !usingDualWeapon) {
			return false;
		}

		if (playerManager.isUsingGenericModelActive ()) {
			return false;
		}

		if (!canUseInput ()) {
			return false;
		}

		return true;
	}

	public void quickDrawWeaponThirdPersonAction ()
	{
		if (!playerManager.canCrouchWhenUsingWeaponsOnThirdPerson) {
			if (isPlayerCrouching ()) {
				activateCrouch ();
			}

			if (isPlayerCrouching ()) {
				return;
			}
		}

		playerCameraManager.setOriginalRotationSpeed ();

		carryingWeaponInThirdPerson = true;

		getCurrentWeapon ();

		enableOrDisableWeaponsHUD (true);

		updateWeaponHUDInfo ();

		updateAmmo ();

		if (usingDualWeapon) {
			IKManager.setUsingDualWeaponState (true);
		}

		currentIKWeapon.quickDrawWeaponThirdPersonAction ();

		currentWeaponSystem.setPauseDrawKeepWeaponSound ();

		currentWeaponSystem.setWeaponCarryState (true, false);

		IKManager.quickDrawWeaponState (currentIKWeapon.thirdPersonWeaponInfo);

		enableOrDisableWeaponCursor (false);

		if (!usingDualWeapon) {
			if (currentIKWeapon != null) {
				currentIKWeapon.checkHandsPosition ();
			}
		}

		if (usingDualWeapon) {
			checkShowWeaponSlotsParentWhenWeaponSelected (currentRightWeaponSystem.getWeaponNumberKey ());
		} else {
			checkShowWeaponSlotsParentWhenWeaponSelected (currentWeaponSystem.getWeaponNumberKey ());
		}
	}

	public void quickKeepWeaponThirdPersonAction ()
	{
		carryingWeaponInThirdPerson = false;
		enableOrDisableWeaponsHUD (false);

		currentWeaponSystem.enableHUD (false);

		IKManager.setIKWeaponState (false, currentIKWeapon.thirdPersonWeaponInfo, false, currentIKWeapon.getWeaponSystemName ());

		currentIKWeapon.quickKeepWeaponThirdPersonAction ();

		IKManager.stopIKWeaponsActions ();

		currentWeaponSystem.setPauseDrawKeepWeaponSound ();
	
		currentWeaponSystem.setWeaponCarryState (false, false);

		enableOrDisableWeaponCursor (false);

		playerCameraManager.setOriginalRotationSpeed ();

		if (aimingInThirdPerson) {
			activateOrDeactivateAimMode (false);
			aimingInThirdPerson = false;

			checkPlayerCanJumpWhenAimingState ();

			enableOrDisableGrabObjectsManager (aimingInThirdPerson);

			currentWeaponSystem.setWeaponAimState (aimingInThirdPerson, false);

			if (currentIKWeapon.useLowerRotationSpeedAimedThirdPerson) {
				playerCameraManager.setOriginalRotationSpeed ();
			}
		}
			
		setAimAssistInThirdPersonState ();
	}

	public void quicChangeWeaponThirdPersonAction ()
	{
		drawOrKeepWeapon (false);

		weaponChanged ();

		drawOrKeepWeapon (true);
		keepingWeapon = false;

		setChangingWeaponState (false);
	}

	public bool isUsingFreeFireMode ()
	{
		return usingFreeFireMode || checkToKeepWeaponAfterAimingWeaponFromShooting;
	}

	public void enableFreeFireModeState ()
	{
		checkToKeepWeaponAfterAimingWeaponFromShooting = false;

		playerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

		playerManager.setUsingFreeFireModeState (true);

		usingFreeFireMode = true;
	}

	public void disableFreeFireModeState ()
	{
		aimingWeaponFromShooting = false;

		checkToKeepWeaponAfterAimingWeaponFromShooting = false;

		checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = false;

		playerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (false);

		playerManager.setUsingFreeFireModeState (false);

		usingFreeFireMode = false;

		if (headTrackManager != null) {
			headTrackManager.setOriginalCameraBodyWeightValue ();
		}

		if (!pauseUpperBodyRotationSystemActive) {
			checkSetExtraRotationCoroutine (false);

			upperBodyRotationManager.enableOrDisableIKUpperBody (false);

			upperBodyRotationManager.setCurrentBodyRotation (extraRotation);
		}
	}

	public void stopFreeFireOnDrawWeaponWithAnimation ()
	{
		if (usingFreeFireMode) {
			if (holdShootActive) {
				stopUpdateHoldShootWeaponCoroutine ();
			}

			resetWeaponFiringAndAimingIfPlayerDisabled ();
		}
	}

	public void enableOrDisableIKUpperBodyExternally (bool state)
	{
		if (!pauseUpperBodyRotationSystemActive) {
			checkSetExtraRotationCoroutine (state);

			upperBodyRotationManager.enableOrDisableIKUpperBody (state);

			if (state) {
				upperBodyRotationManager.setUsingWeaponRotationPointState (true);
			}
		}
	}

	public playerScreenObjectivesSystem getPlayerScreenObjectivesManager ()
	{
		return playerScreenObjectivesManager;
	}

	public void updateCanMoveValue ()
	{
		canMove = !playerIsDead && playerManager.canPlayerMove ();
	}

	public bool canPlayerMove ()
	{
		return canMove;
	}

	public bool isCheckToKeepWeaponAfterAimingWeaponFromShooting ()
	{
		return checkToKeepWeaponAfterAimingWeaponFromShooting;
	}

	public void setCarryWeaponInLowerPositionActiveState (bool state)
	{
		carryWeaponInLowerPositionActive = state;
	}

	public bool isCarryWeaponInLowerPositionActive ()
	{
		return carryWeaponInLowerPositionActive;
	}

	public bool currentWeaponUsesHeadLookWhenAiming ()
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon != null) {
				return currentRightIKWeapon.headLookWhenAiming;
			} else {
				return false;
			}
		} else {
			if (currentIKWeaponLocated) {
				return currentIKWeapon.headLookWhenAiming;
			} else {
				return false;
			}
		}
	}

	public float getCurrentWeaponHeadLookSpeed ()
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon != null) {
				return currentRightIKWeapon.headLookSpeed;
			} else {
				return 0;
			}
		} else {
			if (currentIKWeaponLocated) {
				return currentIKWeapon.headLookSpeed;
			} else {
				return 0;
			}
		}
	}

	public Vector3 getCurrentHeadLookTargetPosition ()
	{
		if (usingDualWeapon) {
			if (currentRightIKWeapon != null) {
				return currentRightIKWeapon.currentHeadLookTarget.position;
			} else {
				return Vector3.zero;
			}
		} else {
			if (currentIKWeaponLocated) {
				return currentIKWeapon.currentHeadLookTarget.position;
			} else {
				return Vector3.zero;
			}
		}
	}

	public bool playerCanAIm ()
	{
		if (currentIKWeapon != null && currentIKWeapon.isReloadingWeapon ()) {
			return false;
		}

		if (canAimOnAirThirdPerson || isFirstPersonActive () || (!canAimOnAirThirdPerson && !isFirstPersonActive () && playerManager.isPlayerOnGround ()) || aimingInThirdPerson) {
			return true;
		}

		return false;
	}

	public void disableAimModeInputPressedState ()
	{
		setAimModeInputPressedState (false);
	}

	public void setAimModeInputPressedState (bool state)
	{
		aimModeInputPressed = state;
	}

	public void setCurrentWeaponAsOneHandWield ()
	{
		currentIKWeapon.setUsingDualWeaponState (true);
		
		currentIKWeapon.setCurrentWeaponAsOneHandWield ();

		getCurrentWeaponRotation (currentIKWeapon);
	}

	public void setCurrentWeaponAsTwoHandsWield ()
	{
		currentIKWeapon.setUsingDualWeaponState (false);

		currentIKWeapon.setCurrentWeaponAsTwoHandsWield ();

		getCurrentWeaponRotation (currentIKWeapon);
	}

	public void updateWeaponListCount ()
	{
		weaponListCount = weaponsList.Count;
	}

	//Player weapon stats
	public float getDamageMultiplierStat ()
	{
		return damageMultiplierStat;
	}

	public float getExtraDamageStat ()
	{
		return extraDamageStat;
	}

	public float getSpreadMultiplierStat ()
	{
		return spreadMultiplierStat;
	}

	public float getFireRateMultiplierStat ()
	{
		return fireRateMultiplierStat;
	}

	public float getExtraReloadSpeedStat ()
	{
		return extraReloadSpeedStat;
	}

	public int getMagazineExtraSizeStat ()
	{
		return magazineExtraSizeStat;
	}

	public void weaponStatIncreaseDamageMultiplierStat (float extraValue)
	{
		damageMultiplierStat += extraValue;
	}

	public void weaponStatIncreaseExtraDamageStat (float extraValue)
	{
		extraDamageStat += extraValue;
	}

	public void weaponStatDecreaseSpreadMultiplierStat (float extraValue)
	{
		spreadMultiplierStat -= extraValue;
	}

	public void weaponStatIncreaseFireRateMultiplierStat (float extraValue)
	{
		fireRateMultiplierStat += extraValue;
	}

	public void weaponStatIncreaseExtraReloadSpeedStat (float extraValue)
	{
		extraReloadSpeedStat += extraValue;
	}

	public void weaponStatIncreaseMagazineExtraSizeStat (float extraValue)
	{
		magazineExtraSizeStat += (int)extraValue;
	}

	public void initializeDamageMultiplierStatAmount (float newValue)
	{
		damageMultiplierStat = newValue;
	}

	public void initializeExtraDamageStatAmount (float newValue)
	{
		extraDamageStat = newValue;
	}

	public void initializeSpreadMultiplierStatAmount (float newValue)
	{
		spreadMultiplierStat = newValue;
	}

	public void initializeFireRateMultiplierStatAmount (float newValue)
	{
		fireRateMultiplierStat = newValue;
	}

	public void initializeExtraReloadSpeedStatAmount (float newValue)
	{
		extraReloadSpeedStat = newValue;
	}

	public void initializeMagazineExtraSizeStatAmount (float newValue)
	{
		magazineExtraSizeStat = (int)newValue;
	}

	public void setFireWeaponsInputActiveState (bool state)
	{
		fireWeaponsInputActive = state;
	}

	public void setGeneralWeaponsInputActiveState (bool state)
	{
		generalWeaponsInputActive = state;
	}

	public void selectFirstWeaponGameObjectOnEditor ()
	{
		updateWeaponListCount ();

		if (weaponListCount > 0) {
			selectWeaponOnListInEditor (0);
		} else {
			selectObjectInEditor (weaponsParent.gameObject);
		}
	}

	public void updateDurabilityAmountStateOnAllObjects ()
	{
		if (!checkDurabilityOnObjectEnabled) {
			return;
		}

		weaponListCount = weaponsList.Count;

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].weaponEnabled) {
				weaponsList [k].updateDurabilityAmountState ();
			}
		}
	}

	public void updateDurabilityAmountStateOnObjectByName (string weaponName)
	{
		if (!checkDurabilityOnObjectEnabled) {
			return;
		}

		weaponListCount = weaponsList.Count;

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].weaponEnabled && weaponsList [k].getWeaponName ().Equals (weaponName)) {
				weaponsList [k].updateDurabilityAmountState ();

				return;
			}
		}
	}

	public float getDurabilityAmountStateOnObjectByName (string weaponName)
	{
		if (!checkDurabilityOnObjectEnabled) {
			return -1;
		}

		weaponListCount = weaponsList.Count;

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].weaponEnabled && weaponsList [k].getWeaponName ().Equals (weaponName)) {
				return weaponsList [k].getDurabilityAmount ();
			}
		}

		return -1;
	}

	public void initializeDurabilityValue (float newAmount, string weaponName)
	{
		if (!checkDurabilityOnObjectEnabled) {
			return;
		}

		weaponListCount = weaponsList.Count;

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].weaponEnabled && weaponsList [k].getWeaponName ().Equals (weaponName)) {
				weaponsList [k].initializeDurabilityValue (newAmount);

				return;
			}
		}
	}

	public void repairObjectFully (string weaponName)
	{
		if (!checkDurabilityOnObjectEnabled) {
			return;
		}

		weaponListCount = weaponsList.Count;

		for (int k = 0; k < weaponListCount; k++) {
			if (weaponsList [k].getWeaponName ().Equals (weaponName)) {
				weaponsList [k].repairObjectFully ();

				return;
			}
		}
	}

	public void breakFullDurabilityOnCurrentWeapon ()
	{
		if (!checkDurabilityOnObjectEnabled) {
			return;
		}

		if (weaponsModeActive) {
			if (isPlayerCarringWeapon ()) {
				if (currentIKWeapon != null) {
					currentIKWeapon.breakFullDurabilityOnCurrentWeapon ();
				}
			}
		}
	}

	public void checkEventOnEmptyDurability ()
	{
		if (!checkDurabilityOnObjectEnabled) {
			return;
		}

		if (useEventOnDurabilityEmptyOnMeleeWeapon) {
			eventOnDurabilityEmptyOnMeleeWeapon.Invoke ();
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputSetCurrentWeaponAsOneOrTwoHandsWield ()
	{ 
		if (!generalWeaponsInputActive) {
			return;
		}

		if (canUseCarriedWeaponsInput () && !usingDualWeapon && !isFirstPersonActive () && !currentIKWeapon.isReloadingWeapon () && !currentIKWeapon.isWeaponMoving ()) {
			if (isAimingWeapons ()) {
				return;
			}

			if (showDebugLog) {
				print ("change hand");
			}

			bool usingWeaponAsOneHandWield = currentIKWeapon.getUsingWeaponAsOneHandWieldState ();

			usingWeaponAsOneHandWield = !usingWeaponAsOneHandWield;

			if (usingWeaponAsOneHandWield) {
				setCurrentWeaponAsOneHandWield ();
			} else {
				setCurrentWeaponAsTwoHandsWield ();
			}
		}
	}

	public void inputAimWeapon ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (!playerCanAIm ()) {
			return;
		}

		if (canUseWeaponsInput ()) {
			setAimModeInputPressedState (!aimModeInputPressed);

			if (isUsingWeapons ()) {
				if (aimingInThirdPerson && (checkToKeepWeaponAfterAimingWeaponFromShooting || usingFreeFireMode)) {
					aimCurrentWeapon (true);
				} else {
					aimCurrentWeaponInput ();
				}
			} else {
				if (drawAndAimWeaponIfFireButtonPressed && isThirdPersonView) {
					quickDrawWeaponThirdPersonAction ();
					aimCurrentWeaponInput ();
				}
			}
		}
	}

	public void inputStartOrStopAimWeapon (bool aimingWeapon)
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (usingDualWeapon) {
			return;
		}

		if (!playerCanAIm ()) {
			return;
		}

		if (canUseWeaponsInput ()) {
			setAimWeaponState (aimingWeapon);
		}
	}

	public void setAimWeaponState (bool aimingWeapon)
	{
		setAimModeInputPressedState (aimingWeapon);

		if (isUsingWeapons ()) {
			if (usingFreeFireMode) {
				aimCurrentWeapon (true);
			} else {
				if (aimingWeapon) {
					aimCurrentWeapon (true);
				} else {
					aimCurrentWeapon (false);
				}
			}
		} else {
			if (drawAndAimWeaponIfFireButtonPressed && isThirdPersonView) {
				if (aimingWeapon) {
					quickDrawWeaponThirdPersonAction ();
					aimCurrentWeaponInput ();
				}
			}
		}
	}

	public void inputPressDownOrReleaseDropWeapon (bool holdingButton)
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (canUseCarriedWeaponsInput ()) {
			if (holdingButton) {
				holdingDropButtonToIncreaseForce = true;
				lastTimeHoldDropButton = Time.time;

				if (holdDropButtonToIncreaseForce) {
					currentDropForce = 0;
				} else {
					dropWeapon ();
				}
			} else {
				if (holdDropButtonToIncreaseForce) {
					if (holdingDropButtonToIncreaseForce && Time.time < lastTimeHoldDropButton + 0.2f) {
						holdingDropButtonToIncreaseForce = false;
					}

					dropWeapon ();
				}

				holdingDropButtonToIncreaseForce = false;
			}
		}
	}

	public void inputHoldDropWeapon ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (canUseCarriedWeaponsInput ()) {
			if (holdDropButtonToIncreaseForce) {
				currentDropForce += dropIncreaseForceSpeed * Time.deltaTime;

				currentDropForce = Mathf.Clamp (currentDropForce, 0, maxDropForce);		
			}
		}
	}

	public void inputActivateSecondaryAction ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (canUseCarriedWeaponsInput ()) {
			activateSecondaryAction ();
		}
	}

	public void inputActivateSecondaryActionOnPressDown ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (canUseCarriedWeaponsInput ()) {
			activateSecondaryActionOnDownPress ();
		}
	}

	public void inputActivateSecondaryActionOnPressUp ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (canUseCarriedWeaponsInput ()) {
			activateSecondaryActionOnUpPress ();
		}
	}

	public void inputActivateSecondaryActionOnPressHold ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (canUseCarriedWeaponsInput ()) {
			activateSecondaryActionOnUpHold ();
		}
	}

	public void inputWeaponMeleeAttack ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (!shootWeaponEnabled) {
			return;
		}

		if (canUseCarriedWeaponsInput ()) {
			useMeleeAttack ();
		}
	}

	public void inputLockOrUnLockCursor ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (canMove && weaponsModeActive && anyWeaponAvailable) {
			bool canUnlockCursor = false;

			if (carryingWeaponInFirstPerson) {
				canUnlockCursor = (!usingDualWeapon && currentIKWeapon.canUnlockCursor) ||
				(usingDualWeapon && (currentRightIKWeapon.canUnlockCursor || currentLeftIkWeapon.canUnlockCursor));
			} else if (carryingWeaponInThirdPerson) {
				canUnlockCursor = (!usingDualWeapon && currentIKWeapon.canUnlockCursorOnThirdPerson) ||
				(usingDualWeapon && (currentRightIKWeapon.canUnlockCursorOnThirdPerson || currentLeftIkWeapon.canUnlockCursorOnThirdPerson));
			}

			if (canUnlockCursor && (currentIKWeapon.isCurrentWeapon () || usingDualWeapon) &&
			    !playerManager.isUsingDevice () && !playerManager.isUsingSubMenu () && !editingWeaponAttachments) {

				lockOrUnlockCursor (!cursorLocked);
			}
		}
	}

	float lastTimeDrawWeapon;

	public void inputHoldOrReleaseShootWeapon (bool holdingButton)
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		//used on press down and press up button actions
		if (usingDualWeapon) {
			return;
		}

		if (!fireWeaponsInputActive) {
			return;
		}

		if (!shootWeaponEnabled) {
			return;
		}

		//if the player is not dead, the weapons mode is active, the player is not busy, there are weapons available and currentIKWeapon is assigned, then
		if (canUseWeaponsInput ()) {

			if (holdingButton) {
				setHoldShootWeaponState (true);
			}

			//if the player is carrying a weapon in third or first person, or the option to draw weapons if the fire button is pressed is active
			if (isUsingWeapons () || drawWeaponIfFireButtonPressed) {
				//if the player is aiming in third person or the option to fire weapons is active, or the player is carrying a weapon in first person, 
				//or the option to draw weapons if the fire button is pressed is active
				if ((aimingInThirdPerson || canFireWeaponsWithoutAiming) || carryingWeaponInFirstPerson || drawWeaponIfFireButtonPressed) {
					//if the cursor is not unlocked in the current weapom
					if (cursorLocked && !weaponsAreMoving ()) {

						//if the player is holding the fire button
						if (holdingButton) {
							//if the player is in third person
							if (!playerManager.isPlayerNavMeshEnabled ()) {
								if (isThirdPersonView) {
									//if the player is carrying a weapon
									if (carryingWeaponInThirdPerson) {
										//if the player is not aiming and the option to fire weapons without aim is active
										if (!aimingInThirdPerson && canFireWeaponsWithoutAiming) {
											if (playerManager.isPlayerMovingOn3dWorld ()) {
												//set the aim state activate from firing to true and activate the aim mode

												enableFreeFireModeState ();

												aimingWeaponFromShooting = true;

												setAimModeInputPressedState (false);

												if (useQuickDrawWeapon) {
													if (Time.time > lastTimeDrawWeapon + 0.2f) {
														aimCurrentWeaponInput ();
													}
												} else {
													aimCurrentWeaponInput ();
												}
											} else {
												checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = true;
												aimCurrentWeaponInput ();
											}
										}
									}
								}

								//if the player is not carrying a weapon and the option to draw weapon is active, draw the current weapon
								if (!isUsingWeapons ()) {
									if (drawWeaponIfFireButtonPressed) {
										if (!currentWeaponIsMoving ()) {
											drawOrKeepWeaponInput ();
										}

										//avoid to keep checking the function, so the weapon is not fired before draw it
										return;
									}
								}
							}
								
							if ((aimingInThirdPerson || carryingWeaponInFirstPerson) && (!isThirdPersonView || currentWeaponWithHandsInPosition ())) {
								//if the weapon is in automatic mode and with the option to fire burst, shoot the weapon
								if (currentWeaponSystem.weaponSettings.automatic) {
									if (currentWeaponSystem.weaponSettings.useBurst) {
										shootWeapon (true);
									}
								} else {
									shootWeapon (true);
								}
							}
							
						} else {
							//if the fire button is released, stop shoot the weapon
							shootWeapon (false);

							if (playerManager.isPlayerMovingOn3dWorld ()) {
								//if the player is aiming in third person and the option to fire weapons without need to aim is active and the player activated the aiming weapon from shooting
								if (aimingInThirdPerson && canFireWeaponsWithoutAiming && aimingWeaponFromShooting && isThirdPersonView) {
									//deactivate the aim mode
									checkToKeepWeaponAfterAimingWeaponFromShooting = true;
									playerManager.setCheckToKeepWeaponAfterAimingWeaponFromShootingState (true);
								} else if (canFireWeaponsWithoutAiming && aimingWeaponFromShooting && isThirdPersonView) {
									if (Time.time < lastTimeDrawWeapon + 0.2f) {
										disableFreeFireModeState ();
									}
								}
							}
								
							aimingWeaponFromShooting = false;

							playerManager.setUsingFreeFireModeState (false);

							usingFreeFireMode = false;
						}
					}
				}
			}
		}

		if (!holdingButton) {
			setHoldShootWeaponState (false);
		}
	}


	Coroutine holdShootWeaponCoroutine;

	bool holdShootActive;

	public void setHoldShootWeaponState (bool state)
	{
		stopUpdateHoldShootWeaponCoroutine ();

		if (state) {
//			print ("start shoot coroutine");

			holdShootWeaponCoroutine = StartCoroutine (updateHoldShootWeaponCoroutine ());

			holdShootActive = true;
		}
	}

	public void stopUpdateHoldShootWeaponCoroutine ()
	{
		if (holdShootWeaponCoroutine != null) {
			StopCoroutine (holdShootWeaponCoroutine);

//			print ("stop shoot coroutine");
		}

		holdShootActive = false;
	}

	IEnumerator updateHoldShootWeaponCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			inputHoldShootWeapon ();
		}
	}



	public void inputHoldShootWeapon ()
	{
		if (!generalWeaponsInputActive) {
			if (holdShootActive) {
				stopUpdateHoldShootWeaponCoroutine ();
			}

			return;
		}

		//used on press button action
		if (usingDualWeapon) {
			if (holdShootActive) {
				stopUpdateHoldShootWeaponCoroutine ();
			}

			return;
		}

		if (!fireWeaponsInputActive) {
			if (holdShootActive) {
				stopUpdateHoldShootWeaponCoroutine ();
			}

			return;
		}

		if (!shootWeaponEnabled) {
			if (holdShootActive) {
				stopUpdateHoldShootWeaponCoroutine ();
			}

			return;
		}

		if (holdShootActive) {
			if (currentIKWeapon != null && currentIKWeapon.isReloadingWeapon ()) {
				stopUpdateHoldShootWeaponCoroutine ();

				return;
			}
		}

		//if the player is not dead, the weapons mode is active, the player is not busy, there are weapons available and currentIKWeapon is assigned, then
		if (canUseWeaponsInput ()) {
			//if the player is carrying a weapon in third or first person, or the option to draw weapons if the fire button is pressed is active
			if (isUsingWeapons () || drawWeaponIfFireButtonPressed) {
				//if the player is aiming in third person or the option to fire weapons is active, or the player is carrying a weapon in first person, 
				//or the option to draw weapons if the fire button is pressed is active
				if (aimingInThirdPerson || canFireWeaponsWithoutAiming || carryingWeaponInFirstPerson || drawWeaponIfFireButtonPressed) {
					//if the cursor is not unlocked in the current weapom
					if (cursorLocked && !weaponsAreMoving ()) {

						if (!playerManager.isPlayerNavMeshEnabled ()) {
							//if the player is in third person
							if (isThirdPersonView) {
								//if the player is carrying a weapon
								if (carryingWeaponInThirdPerson) {
									//if the player is not aiming and the option to fire weapons without aim is active
									if (!aimingInThirdPerson && canFireWeaponsWithoutAiming) {
										if (playerManager.isPlayerMovingOn3dWorld ()) {
											//set the aim state activate from firing to true and activate the aim mode

											enableFreeFireModeState ();

											aimingWeaponFromShooting = true;

											setAimModeInputPressedState (false);

											if (useQuickDrawWeapon) {
												if (Time.time > lastTimeDrawWeapon + 0.2f) {
													aimCurrentWeaponInput ();
												}
											} else {
												aimCurrentWeaponInput ();
											}
										} else {
											checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = true;
											aimCurrentWeaponInput ();
										}
									}
								}
							} 
							
							//if the player is not carrying a weapon and the option to draw weapon is active, draw the current weapon
							if (!isUsingWeapons ()) {
								if (drawWeaponIfFireButtonPressed) {
									if (!currentWeaponIsMoving ()) {
										drawOrKeepWeaponInput ();
									}
					
									//avoid to keep checking the function, so the weapon is not fired before draw it
									return;
								}
							}
						}

						if ((aimingInThirdPerson || carryingWeaponInFirstPerson) && (!isThirdPersonView || currentWeaponWithHandsInPosition ())) {
							//if the weapon is in automatic mode and with the option to fire burst, shoot the weapon
							if (currentWeaponSystem.weaponSettings.automatic) {
								if (!currentWeaponSystem.weaponSettings.useBurst) {
									shootWeapon (true);
								}
							} else {
								currentWeaponSystem.checkWeaponAbilityHoldButton ();
							}
						}
					}
				}
			}
		} else {
			if (holdShootActive) {
				stopUpdateHoldShootWeaponCoroutine ();
			}
		}
	}

	public void inputDrawWeapon ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (canMove && weaponsModeActive && !playerCurrentlyBusy && anyWeaponAvailable && currentIKWeapon.isCurrentWeapon ()) {
			if (!usingDualWeapon && !currentIKWeapon.isWeaponMoving ()) {
				if (currentIKWeapon.isWeaponConfiguredAsDualWeapon ()) {
					
					chooseDualWeaponIndex = currentIKWeapon.getWeaponSystemKeyNumber ();

					if (showDebugLog) {
						print ("weapon to draw " + currentIKWeapon.getWeaponSystemName () + "configured as dual weapon with " + currentIKWeapon.getLinkedDualWeaponName ());
					}

					if (currentIKWeapon.usingRightHandDualWeapon) {
						changeDualWeapons (currentIKWeapon.getWeaponSystemName (), currentIKWeapon.getLinkedDualWeaponName ());
					} else {
						changeDualWeapons (currentIKWeapon.getLinkedDualWeaponName (), currentIKWeapon.getWeaponSystemName ());
					}
				} else {
					drawOrKeepWeaponInput ();
				}

				disableFreeFireModeState ();

				drawWhenItIsReady = false;

				keepWhenItIsReady = false;
			}
		}
	}

	public void inputReloadWeapon ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (canUseCarriedWeaponsInput () && isUsingWeapons ()) {		
			if (usingDualWeapon) {
				currentRightWeaponSystem.manualReload ();
				currentLeftWeaponSystem.manualReload ();
			} else {
				currentWeaponSystem.manualReload ();
			}
		}
	}

	public void inputNextOrPreviousWeaponByButton (bool setNextWeapon)
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (!changeWeaponsWithKeysActive) {
			return;
		}

		if (isActionActiveInPlayer ()) {
			return;
		}

		setNextOrPreviousWeapon (setNextWeapon);
	}

	public void inputNextOrPreviousWeapnByMouse (bool setNextWeapon)
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (!changeWeaponsWithMouseWheelActive) {
			return;
		}

		if (isActionActiveInPlayer ()) {
			return;
		}

		setNextOrPreviousWeapon (setNextWeapon);
	}

	void setNextOrPreviousWeapon (bool setNextWeapon)
	{
		if (canMove && weaponsModeActive && !playerCurrentlyBusy && anyWeaponAvailable && currentIKWeapon.isCurrentWeapon ()) {
			if (!reloadingWithAnimationActive) {
				if (!currentIKWeapon.isWeaponMoving () &&
				    !playerManager.isGamePaused () &&
				    !currentIKWeapon.isReloadingWeapon () &&
				    !weaponsAreMoving ()) {

					if (Time.time > lastTimeReload + 0.5f) {
						if (setNextWeapon) {
							chooseNextWeapon (false, true);
						} else {
							choosePreviousWeapon (false, true);
						}
					}
				}
			}
		}
	}

	public void inputEditWeaponAttachments ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (!openWeaponAttachmentsMenuEnabled) {
			return;
		}
			
		if ((canMove || editingWeaponAttachments) && weaponsModeActive && anyWeaponAvailable && isPlayerCarringWeapon () && !isAimingWeapons ()) {
			if (!weaponsAreMoving () && !currentIKWeapon.isWeaponMoving ()) {
				if (currentIKWeapon.isCurrentWeapon () || usingDualWeapon) {
					editWeaponAttachments ();
				}
			}
		}
	}

	public void inputDrawRightWeapon ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (!usingDualWeapon) {
			return;
		}

		if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable && !currentWeaponIsMoving () && !changingDualWeapon) {
			keepDualWeaponAndDrawSingle (true);
		}
	}

	public void inputDrawLeftWeapon ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (!usingDualWeapon) {
			return;
		}

		if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable && !currentWeaponIsMoving () && !changingDualWeapon) {
			keepDualWeaponAndDrawSingle (false);
		}
	}

	public void drawRightWeapon ()
	{
		if (currentRightIKWeapon == null || currentLeftIkWeapon == null) {
			return;
		}

		drawCurrentDualWeaponSelected (true);
	}

	public void drawLeftWeapon ()
	{
		if (currentLeftIkWeapon == null || currentRightIKWeapon == null) {
			return;
		}

		drawCurrentDualWeaponSelected (false);
	}

	public void drawCurrentDualWeaponSelected (bool isRightWeapon)
	{
		if (previousSingleIKWeapon != null && previousSingleIKWeapon.isCurrentWeapon ()) {
			if (previousSingleIKWeapon != currentRightIKWeapon && previousSingleIKWeapon != currentLeftIkWeapon) {
				previousSingleIKWeapon.setCurrentWeaponState (false);

				previousSingleIKWeapon = null;
			}
		}

		if (isRightWeapon) {
			currentIKWeapon = currentRightIKWeapon;
			currentWeaponSystem = currentRightWeaponSystem;
		} else {
			currentIKWeapon = currentLeftIkWeapon;
			currentWeaponSystem = currentLeftWeaponSystem;
		}

		checkIfCurrentIkWeaponNotNull ();

		currentIKWeapon.setCurrentWeaponState (true);

		if (showDebugLog) {
			print ("drawing right weapon " + isRightWeapon + " " + currentIKWeapon.getWeaponSystemName ());
		}

		if (currentIKWeapon.isCurrentWeapon ()) {
			if (!currentIKWeapon.isWeaponMoving ()) {

				currentIKWeapon.thirdPersonWeaponInfo.usedOnRightHand = isRightWeapon;
				currentIKWeapon.thirdPersonWeaponInfo.dualWeaponActive = true;

				currentIKWeapon.firstPersonWeaponInfo.usedOnRightHand = isRightWeapon;
				currentIKWeapon.firstPersonWeaponInfo.dualWeaponActive = true;

				if (!currentWeaponSystem.carryingWeapon ()) {
					setUsingDualWeaponsState (true);

					mainWeaponListManager.setSelectingWeaponState (true);

					getCurrentWeaponRotation (currentRightIKWeapon);
				}

				if (isRightWeapon) {
					IKManager.setIKWeaponsRightHandSettings (currentIKWeapon.thirdPersonWeaponInfo);
				} else {
					IKManager.setIKWeaponsLeftHandSettings (currentIKWeapon.thirdPersonWeaponInfo);
				}

				if (isThirdPersonView) {
					IKManager.setUsingDualWeaponState (true);
				}

				currentIKWeapon.setUsingRightHandDualWeaponState (isRightWeapon);

				currentIKWeapon.setUsingDualWeaponState (true);

				drawOrKeepWeapon (!currentWeaponSystem.carryingWeapon ());

				if (currentRightWeaponSystem.carryingWeapon () || currentLeftWeaponSystem.carryingWeapon ()) {

					if (isThirdPersonView) {
						carryingWeaponInThirdPerson = true;
					} else {
						carryingWeaponInFirstPerson = true;
					}

					enableOrDisableWeaponsHUD (true);
				} else {
					setUsingDualWeaponsState (false);

					disableDualWeaponStateOnWeapons ();

					if (settingSingleWeaponFromNumberKeys) {

						currentRightIKWeapon.setCurrentWeaponState (false);
						currentLeftIkWeapon.setCurrentWeaponState (false);

						setWeaponByName (singleWeaponNameToChangeFromNumberkeys);
						settingSingleWeaponFromNumberKeys = false;
					} else {
						if (setRightWeaponAsCurrentSingleWeapon && currentRightIKWeapon != null && currentRightIKWeapon.isWeaponEnabled ()) {
							setWeaponByName (currentRighWeaponName);
						} else if (currentLeftIkWeapon != null && currentLeftIkWeapon.isWeaponEnabled ()) {

							if (!setRightWeaponAsCurrentSingleWeapon) {
								currentIKWeapon = currentRightIKWeapon;
							}

							checkIfCurrentIkWeaponNotNull ();

							setWeaponByName (currentLeftWeaponName);
						}

						setRightWeaponAsCurrentSingleWeapon = true;
					}

					weaponChanged ();
				}

				disableFreeFireModeState ();
			}
		}
	}

	public void inputKeepDualWeapons ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		if (!usingDualWeapon) {
			return;
		}

		if (canMove && weaponsModeActive && !playerIsBusy () && anyWeaponAvailable && !currentWeaponIsMoving ()) {
			keepDualWeaponAndDrawSingle (true);
		}
	}

	public void keepDualWeaponAndDrawSingle (bool drawRightWeaponActive)
	{
		settingSingleWeaponFromNumberKeys = true;

		if (drawRightWeaponActive) {
			chooseDualWeaponIndex = currentRightIKWeapon.getWeaponSystemKeyNumber ();

			singleWeaponNameToChangeFromNumberkeys = currentRightIKWeapon.getWeaponSystemName ();
		} else {
			chooseDualWeaponIndex = currentLeftIkWeapon.getWeaponSystemKeyNumber ();

			singleWeaponNameToChangeFromNumberkeys = currentLeftIkWeapon.getWeaponSystemName ();
		}

		separateDualWeapons ();
	}

	public void setSelectingWeaponState (bool state)
	{
		setUsingDualWeaponsState (state);
	}

	public void setUsingDualWeaponsState (bool state)
	{
		usingDualWeapon = state;
	}

	public bool isUsingDualWeapons ()
	{
		return usingDualWeapon;
	}

	public void setChangeWeaponsWithNumberKeysActiveState (bool state)
	{
		changeWeaponsWithNumberKeysActive = state;
	}

	public void setChangeWeaponsWithKeysActive (bool state)
	{
		changeWeaponsWithKeysActive = state;
	}

	public void inputHoldOrReleaseShootRightWeapon (bool holdingButton)
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		//used on press down and press up button actions
		holdOrReleaseShootRightOrLeftDualWeapon (holdingButton, true);
	}

	public void inputHoldShootRightWeapon ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		//used on press button action
		holdShootRightOrLeftDualWeapon (true);
	}

	public void inputHoldOrReleaseShootLeftWeapon (bool holdingButton)
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		//used on press down and press up button actions
		holdOrReleaseShootRightOrLeftDualWeapon (holdingButton, false);
	}

	public void inputHoldShootLeftWeapon ()
	{
		if (!generalWeaponsInputActive) {
			return;
		}

		//used on press button action
		holdShootRightOrLeftDualWeapon (false);
	}

	public void holdOrReleaseShootRightOrLeftDualWeapon (bool holdingButton, bool isRightWeapon)
	{
		if (!fireWeaponsInputActive) {
			return;
		}

		if (!shootWeaponEnabled) {
			return;
		}

		//if the player is not dead, the weapons mode is active, the player is not busy, there are weapons available and currentIKWeapon is assigned, then
		if (usingDualWeapon && canUseWeaponsInput ()) {

			if (holdingButton) {
				if (isRightWeapon) {
					setHoldRightShootWeaponState (true);
				} else {
					setHoldLeftShootWeaponState (true);
				}
			}

			//if the player is carrying a weapon in third or first person, or the option to draw weapons if the fire button is pressed is active
			if (isUsingWeapons () || drawWeaponIfFireButtonPressed) {
				//if the player is aiming in third person or the option to fire weapons is active, or the player is carrying a weapon in first person, 
				//or the option to draw weapons if the fire button is pressed is active
				if ((aimingInThirdPerson || canFireWeaponsWithoutAiming) || carryingWeaponInFirstPerson || drawWeaponIfFireButtonPressed) {
					//if the cursor is not unlocked in the current weapom
					if (cursorLocked && !weaponsAreMoving ()) {

						//if the player is holding the fire button
						if (holdingButton) {
							if (!playerManager.isPlayerNavMeshEnabled ()) {
								//if the player is in third person
								if (isThirdPersonView) {
									//if the player is carrying a weapon
									if (carryingWeaponInThirdPerson) {
										//if the player is not aiming and the option to fire weapons without aim is active
										if (!aimingInThirdPerson && canFireWeaponsWithoutAiming) {
											if (playerManager.isPlayerMovingOn3dWorld ()) {
												//set the aim state activate from firing to true and activate the aim mode

												enableFreeFireModeState ();

												aimingWeaponFromShooting = true;

												setAimModeInputPressedState (false);

												if (useQuickDrawWeapon) {
													if (Time.time > lastTimeDrawWeapon + 0.2f) {
														aimCurrentWeaponInput ();
													}
												} else {
													aimCurrentWeaponInput ();
												}
											} else {
												checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = true;
												aimCurrentWeaponInput ();
											}
										}
									}
								}

								//if the player is not carrying a weapon and the option to draw weapon is active, draw the current weapon
								if (!isUsingWeapons ()) {
									if (drawWeaponIfFireButtonPressed) {
										drawOrKeepWeaponInput ();

										//avoid to keep checking the function, so the weapon is not fired before draw it
										return;
									}
								}
							}

							if ((aimingInThirdPerson || carryingWeaponInFirstPerson) && (!isThirdPersonView || currentDualWeaponWithHandsInPosition (isRightWeapon))) {
								//if the weapon is in automatic mode and with the option to fire burst, shoot the weapon
								if (isRightWeapon) {
									if (currentRightWeaponSystem.weaponSettings.automatic) {
										if (currentRightWeaponSystem.weaponSettings.useBurst) {
											shootDualWeapon (true, isRightWeapon, false);
										}
									} else {
										shootDualWeapon (true, isRightWeapon, false);
									}
								} else {
									if (currentLeftWeaponSystem.weaponSettings.automatic) {
										if (currentLeftWeaponSystem.weaponSettings.useBurst) {
											shootDualWeapon (true, isRightWeapon, false);
										}
									} else {
										shootDualWeapon (true, isRightWeapon, false);
									}
								}
							}

						} else {
							//if the fire button is released, stop shoot the weapon
							shootDualWeapon (false, isRightWeapon, false);

							if (playerManager.isPlayerMovingOn3dWorld ()) {
								//if the player is aiming in third person and the option to fire weapons without need to aim is active and the player activated the aiming weapon from shooting
								if (aimingInThirdPerson && canFireWeaponsWithoutAiming && aimingWeaponFromShooting && isThirdPersonView) {
									//deactivate the aim mode
									checkToKeepWeaponAfterAimingWeaponFromShooting = true;
								} else if (canFireWeaponsWithoutAiming && aimingWeaponFromShooting && isThirdPersonView) {
									if (Time.time < lastTimeDrawWeapon + 0.2f) {
										disableFreeFireModeState ();
									}
								}
							}

							aimingWeaponFromShooting = false;

							usingFreeFireMode = false;
						}
					}
				}
			}
		}

		if (!holdingButton) {
			if (isRightWeapon) {
				setHoldRightShootWeaponState (false);
			} else {
				setHoldLeftShootWeaponState (false);
			}
		}
	}

	public void holdShootRightOrLeftDualWeapon (bool isRightWeapon)
	{
		if (!fireWeaponsInputActive) {
			if (holdRightShootActive) {
				stopUpdateHoldRightShootWeaponCoroutine ();
			}

			if (holdLeftShootActive) {
				stopUpdateHoldLeftShootWeaponCoroutine ();
			}

			return;
		}

		if (!shootWeaponEnabled) {
			if (holdRightShootActive) {
				stopUpdateHoldRightShootWeaponCoroutine ();
			}

			if (holdLeftShootActive) {
				stopUpdateHoldLeftShootWeaponCoroutine ();
			}

			return;
		}


		if (!generalWeaponsInputActive) {
			if (holdRightShootActive) {
				stopUpdateHoldRightShootWeaponCoroutine ();
			}

			if (holdLeftShootActive) {
				stopUpdateHoldLeftShootWeaponCoroutine ();
			}

			return;
		}

		//used on press button action
		if (!usingDualWeapon) {
			if (holdRightShootActive) {
				stopUpdateHoldRightShootWeaponCoroutine ();
			}

			if (holdLeftShootActive) {
				stopUpdateHoldLeftShootWeaponCoroutine ();
			}

			return;
		}

		if (holdRightShootActive) {
			if (currentRightIKWeapon != null && currentRightIKWeapon.isReloadingWeapon ()) {
				stopUpdateHoldRightShootWeaponCoroutine ();

				return;
			}
		}

		if (holdLeftShootActive) {
			if (currentLeftIkWeapon != null && currentLeftIkWeapon.isReloadingWeapon ()) {
				stopUpdateHoldLeftShootWeaponCoroutine ();

				return;
			}
		}

		//if the player is not dead, the weapons mode is active, the player is not busy, there are weapons available and currentIKWeapon is assigned, then
		if (usingDualWeapon && canUseWeaponsInput ()) {
	
			//if the player is carrying a weapon in third or first person, or the option to draw weapons if the fire button is pressed is active
			if (isUsingWeapons () || drawWeaponIfFireButtonPressed) {
	
				//if the player is aiming in third person or the option to fire weapons is active, or the player is carrying a weapon in first person, 
				//or the option to draw weapons if the fire button is pressed is active
				if (aimingInThirdPerson || canFireWeaponsWithoutAiming || carryingWeaponInFirstPerson || drawWeaponIfFireButtonPressed) {
					//if the cursor is not unlocked in the current weapom
					if (cursorLocked && !weaponsAreMoving ()) {

						if (!playerManager.isPlayerNavMeshEnabled ()) {
							//if the player is in third person
							if (isThirdPersonView) {
								//if the player is carrying a weapon
								if (carryingWeaponInThirdPerson) {
									//if the player is not aiming and the option to fire weapons without aim is active
									if (!aimingInThirdPerson && canFireWeaponsWithoutAiming) {
										if (playerManager.isPlayerMovingOn3dWorld ()) {
											//set the aim state activate from firing to true and activate the aim mode
											aimingWeaponFromShooting = true;

											enableFreeFireModeState ();

											setAimModeInputPressedState (false);

											if (useQuickDrawWeapon) {
												if (Time.time > lastTimeDrawWeapon + 0.2f) {
													aimCurrentWeaponInput ();
												}
											} else {
												aimCurrentWeaponInput ();
											}
										} else {
											checkToKeepWeaponAfterAimingWeaponFromShooting2_5d = true;
											aimCurrentWeaponInput ();
										}
									}
								}
							} 
							
							//if the player is not carrying a weapon and the option to draw weapon is active, draw the current weapon
							if (!isUsingWeapons ()) {
								if (drawWeaponIfFireButtonPressed) {

									drawOrKeepWeaponInput ();

									//avoid to keep checking the function, so the weapon is not fired before draw it
									return;
								}
							}
						}
							
						if ((aimingInThirdPerson || carryingWeaponInFirstPerson) && (!isThirdPersonView || currentDualWeaponWithHandsInPosition (isRightWeapon))) {
							//if the weapon is in automatic mode and with the option to fire burst, shoot the weapon
							if (isRightWeapon) {
								if (currentRightWeaponSystem.weaponSettings.automatic) {
									if (!currentRightWeaponSystem.weaponSettings.useBurst) {
										shootDualWeapon (true, isRightWeapon, false);
									}
								} else {
									currentRightWeaponSystem.checkWeaponAbilityHoldButton ();
								}
							} else {
								if (currentLeftWeaponSystem.weaponSettings.automatic) {
									if (!currentLeftWeaponSystem.weaponSettings.useBurst) {
										shootDualWeapon (true, isRightWeapon, false);
									}
								} else {
									if (playerCurrentlyBusy) {
										currentLeftWeaponSystem.checkWeaponAbilityHoldButton ();
									}
								}
							}
						}
					}
				}
			}
		} else {
			if (holdRightShootActive) {
				stopUpdateHoldRightShootWeaponCoroutine ();
			}

			if (holdLeftShootActive) {
				stopUpdateHoldLeftShootWeaponCoroutine ();
			}
		}
	}


	Coroutine holdShootRightWeaponCoroutine;

	bool holdRightShootActive;

	void setHoldRightShootWeaponState (bool state)
	{
		stopUpdateHoldRightShootWeaponCoroutine ();

		if (state) {
			holdShootRightWeaponCoroutine = StartCoroutine (updateHoldShootRightWeaponCoroutine ());

			holdRightShootActive = true;
		}
	}

	public void stopUpdateHoldRightShootWeaponCoroutine ()
	{
		if (holdShootRightWeaponCoroutine != null) {
			StopCoroutine (holdShootRightWeaponCoroutine);
		}

		holdRightShootActive = false;
	}

	IEnumerator updateHoldShootRightWeaponCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			inputHoldShootRightWeapon ();
		}
	}


	Coroutine holdShootLeftWeaponCoroutine;

	bool holdLeftShootActive;

	void setHoldLeftShootWeaponState (bool state)
	{
		stopUpdateHoldLeftShootWeaponCoroutine ();

		if (state) {
			holdShootLeftWeaponCoroutine = StartCoroutine (updateHoldShootLeftWeaponCoroutine ());

			holdLeftShootActive = true;
		}
	}

	public void stopUpdateHoldLeftShootWeaponCoroutine ()
	{
		if (holdShootLeftWeaponCoroutine != null) {
			StopCoroutine (holdShootLeftWeaponCoroutine);
		}

		holdLeftShootActive = false;
	}

	public void stopAllCoroutinesOnDeath ()
	{
		stopShowObjectMessageCoroutine ();
			
		stopChangeWeaponsCameraFovCoroutine ();

		stopDisableWeaponsHUDAfterDelay ();

		stopSetExtraRotationCoroutine ();

		stopUpdateHoldShootWeaponCoroutine ();

		stopUpdateHoldRightShootWeaponCoroutine ();

		stopUpdateHoldLeftShootWeaponCoroutine ();
	}

	IEnumerator updateHoldShootLeftWeaponCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			inputHoldShootLeftWeapon ();
		}
	}

	public objectTransformData getMainObjectTransformData ()
	{
		return mainObjectTransformData;
	}


	//EDITOR FUNCTIONS
	public void instantiateUsableWeaponPrefabInfoListFromEditor ()
	{
		if (useUsableWeaponPrefabInfoList) {

			if (weaponsList.Count == 0) {
				for (int k = 0; k < usableWeaponPrefabInfoList.Count; k++) {
					GameObject weaponObject = (GameObject)Instantiate (usableWeaponPrefabInfoList [k].usableWeaponPrefab, transform.position, transform.rotation, weaponsParent);

					weaponObject.name = usableWeaponPrefabInfoList [k].usableWeaponPrefab.name;

					weaponObject.transform.localPosition = Vector3.zero;
					weaponObject.transform.localRotation = Quaternion.identity;
				}

				setWeaponList ();

				useUsableWeaponPrefabInfoList = false;

				updateComponent (true);
			}
		}
	}

	//get all the weapons configured inside the player's body
	public void setWeaponList ()
	{	
		weaponsList.Clear ();

		print ("\n\n");
		print ("SETTING WEAPONS INFO");
		print ("\n\n");

		Component[] components = GetComponentsInChildren (typeof(IKWeaponSystem));
		foreach (Component c in components) {
			addWeaponToListInRunTime (c.gameObject, true, false, false);
		}

		updateWeaponListCount ();

		getAvailableWeaponListString ();

		removeAllNullWeaponsFromPocketList ();

		updateComponent (true);
	}

	//get the list of Available weapons in the list, in case the atribute weaponEnabled is true
	public void getAvailableWeaponListString ()
	{
		int numberOfWeaponsAvailable = 0;

		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i].isWeaponEnabled ()) {
				numberOfWeaponsAvailable++;
			}
		}

		if (numberOfWeaponsAvailable > 0) {
			avaliableWeaponList = new string[numberOfWeaponsAvailable];

			int currentWeaponIndex = 0;

			for (int i = 0; i < weaponListCount; i++) {
				if (weaponsList [i].isWeaponEnabled ()) {
					string name = weaponsList [i].getWeaponSystemName ();
					avaliableWeaponList [currentWeaponIndex] = name;

					currentWeaponIndex++;
				}
			}
		} else {
			if (useUsableWeaponPrefabInfoList) {
				numberOfWeaponsAvailable = usableWeaponPrefabInfoList.Count;

				if (numberOfWeaponsAvailable > 0) {
					avaliableWeaponList = new string[numberOfWeaponsAvailable];

					int currentWeaponIndex = 0;

					for (int i = 0; i < usableWeaponPrefabInfoList.Count; i++) {
						string name = usableWeaponPrefabInfoList [i].Name;
						avaliableWeaponList [currentWeaponIndex] = name;

						currentWeaponIndex++;
					}
				} 
			} else {
				avaliableWeaponList = new string[0];
			}
		}

		print (numberOfWeaponsAvailable + " weapons available to use");

		weaponToStartIndex = 0;

		updateComponent (false);
	}

	public void getWeaponListString ()
	{
		updateWeaponListCount ();

		if (weaponListCount > 0) {
			avaliableWeaponList = new string[weaponListCount];
			int currentWeaponIndex = 0;
			for (int i = 0; i < weaponListCount; i++) {
				string name = weaponsList [i].getWeaponSystemName ();

				avaliableWeaponList [currentWeaponIndex] = name;

				currentWeaponIndex++;
			}
		} else {
			avaliableWeaponList = new string[0];
		}

		weaponToStartIndex = 0;

		rightWeaponToStartIndex = 0;

		leftWeaponToStartIndex = 0;

		updateComponent (false);
	}

	public void removeWeaponFromList (int weaponIndex)
	{
		IKWeaponSystem currentWeaponToRemove = weaponsList [weaponIndex];

		weaponsList.RemoveAt (weaponIndex);

		string weaponName = "";

		if (currentWeaponToRemove != null) {
			GameObject weaponGameObject = currentWeaponToRemove.gameObject;

			weaponName = weaponGameObject.name;

			bool weaponFound = false;

			for (int j = 0; j < weaponPocketList.Count; j++) {
				if (!weaponFound) {
					for (int k = 0; k < weaponPocketList [j].weaponOnPocketList.Count; k++) {
						for (int h = 0; h < weaponPocketList [j].weaponOnPocketList [k].weaponList.Count; h++) {
							if (weaponPocketList [j].weaponOnPocketList [k].weaponList [h] == weaponGameObject) {
								weaponPocketList [j].weaponOnPocketList [k].weaponList.RemoveAt (h);

								weaponFound = true;
							}
						}
					}
				}
			}
		} 

		print ("Weapon " + weaponName + " removed from weapon list");

		updateComponent (true);
	}

	//clear the weapon list of the player in the inspector
	public void clearWeaponList ()
	{
		removeAllWeaponsFromPocketList ();

		weaponsList.Clear ();

		getAvailableWeaponListString ();

		updateWeaponListCount ();

		updateComponent (false);
	}

	public void enableOrDisableWeaponGameObjectList (bool state)
	{
		Transform[] Children = new Transform[weaponsParent.childCount];

		for (int ID = 0; ID < weaponsParent.childCount; ID++) {
			Children [ID] = weaponsParent.GetChild (ID);
		}

		for (int i = 0; i < Children.Length; i++) {
			if (Children [i] != null) {

				if (Children [i].gameObject.activeSelf != state) {
					Children [i].gameObject.SetActive (state);
				}
			}
		}

		setWeaponList ();

		updateComponent (true);
	}

	public void removeWeaponsFromPlayerBody ()
	{
		removeAllWeaponsFromPocketList ();

		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			if (weaponsList [i] != null) {
				DestroyImmediate (weaponsList [i].gameObject);
			}
		}

		weaponsList.Clear ();

		updateWeaponListCount ();

		clearWeaponList ();

		updateComponent (true);
	}

	//set to usable or non usable all the current weapon list in the player, used in the custom editor
	public void enableOrDisableWeaponsList (bool value)
	{
		updateWeaponListCount ();

		for (int i = 0; i < weaponListCount; i++) {
			weaponsList [i].setWeaponEnabledState (value);

			GKC_Utils.updateComponent (weaponsList [i]);
		}

		getAvailableWeaponListString ();
	}

	public void addPocket ()
	{
		weaponPocket newPocket = new weaponPocket ();

		newPocket.Name = "New Pocket";

		weaponPocketList.Add (newPocket);

		updateComponent (true);
	}

	public void addSubPocket (int index)
	{
		weaponListOnPocket newSubPocket = new weaponListOnPocket ();

		newSubPocket.Name = "New Subpocket";

		weaponPocketList [index].weaponOnPocketList.Add (newSubPocket);

		updateComponent (true);
	}

	public void removePocket (int index)
	{
		weaponPocketList.RemoveAt (index);

		updateComponent (true);
	}

	public void removeSubPocket (int pocketIndex, int subPocketIndex)
	{
		weaponPocketList [pocketIndex].weaponOnPocketList.RemoveAt (subPocketIndex);

		updateComponent (true);
	}

	public void removeWeaponFromSubPocket (int pocketIndex, int subPocketIndex, int weaponIndex)
	{
		weaponPocketList [pocketIndex].weaponOnPocketList [subPocketIndex].weaponList.RemoveAt (weaponIndex);

		updateComponent (true);
	}

	public void setCheckDurabilityOnObjectEnabledValueFromEditor (bool state)
	{
		checkDurabilityOnObjectEnabled = state;

		updateComponent (true);
	}

	public void setThirdPersonParent (Transform newParent)
	{
		thirdPersonParent = newParent;

		updateComponent (true);
	}

	public void setRightHandTransform (Transform handTransform)
	{
		rightHandTransform = handTransform;

		updateComponent (true);
	}

	public void setLeftHandTransform (Transform handTransform)
	{
		leftHandTransform = handTransform;

		updateComponent (true);
	}

	public void saveCurrentWeaponListToFile ()
	{
		pauseManager.saveGameInfoFromEditor ("Player Weapons");

		updateComponent (true);
	}

	public void setCustomIgnoreTagsForCharacterFromEditor ()
	{
		if (playerManager != null) {
			useCustomIgnoreTags = true;

			customTagsToIgnoreList.Clear ();

			customTagsToIgnoreList.Add (playerManager.gameObject.tag);

			updateComponent (true);
		}
	}

	public void updateComponent (bool updateDirtyScene)
	{
		GKC_Utils.updateComponent (this);

		if (updateDirtyScene) {

			GKC_Utils.updateDirtyScene ("Update Weapons Info " + gameObject.name, gameObject);
		}
	}

	[System.Serializable]
	public class weaponPocket
	{
		public string Name;
		public Transform pocketTransform;
		public List<weaponListOnPocket> weaponOnPocketList = new List<weaponListOnPocket> ();
	}

	[System.Serializable]
	public class weaponListOnPocket
	{
		public string Name;
		public List<GameObject> weaponList = new List<GameObject> ();
	}

	[System.Serializable]
	public class usableWeaponPrefabInfo
	{
		public string Name;
		public GameObject usableWeaponPrefab;
	}
}