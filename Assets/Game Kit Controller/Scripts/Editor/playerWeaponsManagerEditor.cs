using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(playerWeaponsManager))]
public class playerWeaponsManagerEditor : Editor
{
	SerializedProperty carryingWeaponInThirdPerson;
	SerializedProperty carryingWeaponInFirstPerson;
	SerializedProperty aimingInThirdPerson;
	SerializedProperty aimingInFirstPerson;
	SerializedProperty anyWeaponAvailable;
	SerializedProperty currentWeaponName;
	SerializedProperty changingWeapon;
	SerializedProperty keepingWeapon;
	SerializedProperty editingWeaponAttachments;
	SerializedProperty avoidShootCurrentlyActive;
	SerializedProperty reloadingWithAnimationActive;
	SerializedProperty weaponsModeActive;
	SerializedProperty shootingSingleWeapon;
	SerializedProperty choosedWeapon;
	SerializedProperty usingFreeFireMode;
	SerializedProperty currentIKWeapon;
	SerializedProperty usingDualWeapon;
	SerializedProperty currentRighWeaponName;
	SerializedProperty currentLeftWeaponName;
	SerializedProperty shootingRightWeapon;
	SerializedProperty shootingLeftWeapon;
	SerializedProperty chooseDualWeaponIndex;
	SerializedProperty currentRightIKWeapon;
	SerializedProperty currentLeftIkWeapon;
	SerializedProperty showMainSettings;
	SerializedProperty showWeaponsList;
	SerializedProperty showElementSettings;
	SerializedProperty showDebugSettings;
	SerializedProperty weaponsSlotsAmount;
	SerializedProperty weaponsLayer;

	SerializedProperty targetToDamageLayer;
	SerializedProperty targetForScorchLayer;

	SerializedProperty useCustomLayerToCheck;
	SerializedProperty customLayerToCheck;

	SerializedProperty useCustomIgnoreTags;
	SerializedProperty customTagsToIgnoreList;

	SerializedProperty setWeaponWhenPicked;
	SerializedProperty canGrabObjectsCarryingWeapons;
	SerializedProperty changeToNextWeaponIfAmmoEmpty;
	SerializedProperty drawKeepWeaponWhenModeChanged;
	SerializedProperty changeWeaponsWithNumberKeysActive;
	SerializedProperty changeWeaponsWithMouseWheelActive;
	SerializedProperty changeWeaponsWithKeysActive;
	SerializedProperty weaponsHUDEnabled;
	SerializedProperty disableWeaponsHUDAfterDelayEnabled;
	SerializedProperty delayToDisableWeaponsHUD;
	SerializedProperty weaponCursorActive;
	SerializedProperty useWeaponCursorUnableToShootThirdPerson;
	SerializedProperty useWeaponCursorUnableToShootFirstPerson;
	SerializedProperty canFireWeaponsWithoutAiming;
	SerializedProperty drawWeaponIfFireButtonPressed;
	SerializedProperty drawAndAimWeaponIfFireButtonPressed;
	SerializedProperty keepWeaponAfterDelayThirdPerson;
	SerializedProperty keepWeaponAfterDelayFirstPerson;
	SerializedProperty keepWeaponDelay;
	SerializedProperty useQuickDrawWeapon;
	SerializedProperty timeToStopAimAfterStopFiring;
	SerializedProperty useAimCameraOnFreeFireMode;
	SerializedProperty pivotPointRotationActive;
	SerializedProperty pivotPointRotationSpeed;
	SerializedProperty runWhenAimingWeaponInThirdPerson;
	SerializedProperty stopRunIfPreviouslyNotRunning;
	SerializedProperty runAlsoWhenFiringWithoutAiming;
	SerializedProperty canJumpWhileAimingThirdPerson;
	SerializedProperty canAimOnAirThirdPerson;
	SerializedProperty stopAimingOnAirThirdPerson;
	SerializedProperty ignoreParentWeaponOutsidePlayerBodyWhenNotActive;
	SerializedProperty keepLookAtTargetActiveWhenFiringWithoutAiming;
	SerializedProperty ignoreCheckSurfaceCollisionThirdPerson;
	SerializedProperty ignoreCheckSurfaceCollisionFirstPerson;
	SerializedProperty fireWeaponsInputActive;
	SerializedProperty generalWeaponsInputActive;
	SerializedProperty storePickedWeaponsOnInventory;

	SerializedProperty checkDurabilityOnObjectEnabled;

	SerializedProperty useEventOnDurabilityEmptyOnMeleeWeapon;
	SerializedProperty eventOnDurabilityEmptyOnMeleeWeapon;

	SerializedProperty drawWeaponWhenPicked;
	SerializedProperty drawPickedWeaponOnlyItNotPreviousWeaponEquipped;

	SerializedProperty changeToNextWeaponWhenEquipped;
	SerializedProperty changeToNextWeaponWhenUnequipped;

	SerializedProperty notActivateWeaponsAtStart;
	SerializedProperty canStoreAnyNumberSameWeapon;
	SerializedProperty useAmmoFromInventoryInAllWeapons;
	SerializedProperty openWeaponAttachmentsMenuEnabled;
	SerializedProperty setFirstPersonForAttachmentEditor;
	SerializedProperty useUniversalAttachments;
	SerializedProperty startGameWithCurrentWeapon;
	SerializedProperty drawInitialWeaponSelected;
	SerializedProperty startWithFirstWeaponAvailable;
	SerializedProperty startGameWithDualWeapons;
	SerializedProperty avaliableWeaponList;
	SerializedProperty rightWeaponToStartIndex;
	SerializedProperty leftWeaponToStartIndex;
	SerializedProperty rightWeaponToStartName;
	SerializedProperty leftWeaponToStartName;
	SerializedProperty weaponToStartIndex;
	SerializedProperty weaponToStartName;
	SerializedProperty canDropWeapons;
	SerializedProperty changeToNextWeaponWhenDrop;
	SerializedProperty dropWeaponForceThirdPerson;
	SerializedProperty dropWeaponForceFirstPerson;
	SerializedProperty holdDropButtonToIncreaseForce;
	SerializedProperty dropIncreaseForceSpeed;
	SerializedProperty maxDropForce;
	SerializedProperty dropWeaponInventoryObjectsPickups;
	SerializedProperty dropCurrentWeaponWhenDie;
	SerializedProperty dropAllWeaponsWhenDie;
	SerializedProperty dropWeaponsOnlyIfUsing;
	SerializedProperty drawWeaponWhenResurrect;
	SerializedProperty canMarkTargets;
	SerializedProperty useAimAssistInThirdPerson;
	SerializedProperty useAimAssistInFirstPerson;
	SerializedProperty useMaxDistanceToCameraCenterAimAssist;
	SerializedProperty maxDistanceToCameraCenterAimAssist;
	SerializedProperty useAimAssistInLockedCamera;
	SerializedProperty aimAssistLookAtTargetSpeed;
	SerializedProperty damageMultiplierStat;
	SerializedProperty extraDamageStat;
	SerializedProperty spreadMultiplierStat;
	SerializedProperty fireRateMultiplierStat;
	SerializedProperty extraReloadSpeedStat;
	SerializedProperty magazineExtraSizeStat;
	SerializedProperty cantPickWeaponMessage;
	SerializedProperty cantDropCurrentWeaponMessage;
	SerializedProperty cantPickAttachmentMessage;
	SerializedProperty weaponMessageDuration;
	SerializedProperty useEventsOnStateChange;
	SerializedProperty evenOnStateEnabled;
	SerializedProperty eventOnStateDisabled;

	SerializedProperty loadCurrentPlayerWeaponsFromSaveFile;
	SerializedProperty saveCurrentPlayerWeaponsToSaveFile;

	SerializedProperty loadWeaponAttachmentsInfoFromSaveFile;
	SerializedProperty saveWeaponAttachmentsInfoToSaveFile;

	SerializedProperty showRemainAmmoText;

	SerializedProperty minSwipeDist;
	SerializedProperty usedByAI;
	SerializedProperty weaponsList;
	SerializedProperty weaponPocketList;
	SerializedProperty weaponsHUD;
	SerializedProperty singleWeaponHUD;
	SerializedProperty currentWeaponNameText;
	SerializedProperty currentWeaponAmmoText;
	SerializedProperty ammoSlider;
	SerializedProperty currentWeaponIcon;
	SerializedProperty attachmentPanel;
	SerializedProperty attachmentAmmoText;
	SerializedProperty currentAttachmentIcon;
	SerializedProperty dualWeaponHUD;
	SerializedProperty currentRightWeaponAmmoText;
	SerializedProperty rightAttachmentPanel;
	SerializedProperty rigthAttachmentAmmoText;
	SerializedProperty currentRightAttachmentIcon;
	SerializedProperty currentLeftWeaponAmmoText;
	SerializedProperty leftAttachmentPanel;
	SerializedProperty leftAttachmentAmmoText;
	SerializedProperty currentLeftAttachmentIcon;
	SerializedProperty weaponsParent;
	SerializedProperty weaponsTransformInFirstPerson;
	SerializedProperty weaponsTransformInThirdPerson;
	SerializedProperty thirdPersonParent;
	SerializedProperty firstPersonParent;
	SerializedProperty cameraController;
	SerializedProperty weaponsCamera;
	SerializedProperty weaponCursor;
	SerializedProperty cursorRectTransform;
	SerializedProperty weaponCursorRegular;
	SerializedProperty weaponCursorAimingInFirstPerson;
	SerializedProperty weaponCursorUnableToShoot;
	SerializedProperty weaponCustomReticle;
	SerializedProperty weaponsMessageWindow;
	SerializedProperty mainObjectTransformData;
	SerializedProperty temporalParentForWeapons;
	SerializedProperty pauseManager;
	SerializedProperty playerScreenObjectivesManager;
	SerializedProperty playerCameraManager;
	SerializedProperty headBobManager;
	SerializedProperty playerInput;
	SerializedProperty IKManager;
	SerializedProperty playerManager;
	SerializedProperty upperBodyRotationManager;
	SerializedProperty headTrackManager;
	SerializedProperty grabObjectsManager;

	SerializedProperty playerInventoryManager;
	SerializedProperty mainInventoryListManager;
	SerializedProperty usingDevicesManager;
	SerializedProperty mainCollider;
	SerializedProperty ragdollManager;
	SerializedProperty mainCameraTransform;
	SerializedProperty mainCamera;
	SerializedProperty mainWeaponListManager;

	SerializedProperty rightHandTransform;
	SerializedProperty leftHandTransform;

	SerializedProperty rightHandMountPoint;
	SerializedProperty leftHandMountPoint;

	SerializedProperty showDebugLog;

	SerializedProperty mainSimpleSniperSightSystem;

	SerializedProperty activateDualWeaponsEnabled;

	SerializedProperty useCameraShakeOnShootEnabled;

	SerializedProperty useUsableWeaponPrefabInfoList;
	SerializedProperty usableWeaponPrefabInfoList;

	SerializedProperty ignoreUseDrawKeepWeaponAnimation;

	SerializedProperty removeCurrentWeaponAsNotUsableIfAmmoEmptyEnabled;

	SerializedProperty weaponsCanBeStolenFromCharacter;

	SerializedProperty useEventOnWeaponStolen;
	SerializedProperty eventOnWeaponStolen;

	playerWeaponsManager manager;

	Color buttonColor;

	string carryingWeaponMode;
	string aimingWeaponMode;
	string currentState;
	string currentWeaponText;
	string shootingWeapon;
	string weaponsModeActiveText;
	string inputListOpenedText;

	GUIStyle style = new GUIStyle ();

	IKWeaponSystem _currentIKWeapon;

	GUIStyle buttonStyle = new GUIStyle ();

	bool applicationIsPlaying;

	void OnEnable ()
	{
		carryingWeaponInThirdPerson = serializedObject.FindProperty ("carryingWeaponInThirdPerson");
		carryingWeaponInFirstPerson = serializedObject.FindProperty ("carryingWeaponInFirstPerson");
		aimingInThirdPerson = serializedObject.FindProperty ("aimingInThirdPerson");
		aimingInFirstPerson = serializedObject.FindProperty ("aimingInFirstPerson");
		anyWeaponAvailable = serializedObject.FindProperty ("anyWeaponAvailable");
		currentWeaponName = serializedObject.FindProperty ("currentWeaponName");
		changingWeapon = serializedObject.FindProperty ("changingWeapon");
		keepingWeapon = serializedObject.FindProperty ("keepingWeapon");
		editingWeaponAttachments = serializedObject.FindProperty ("editingWeaponAttachments");
		avoidShootCurrentlyActive = serializedObject.FindProperty ("avoidShootCurrentlyActive");
		reloadingWithAnimationActive = serializedObject.FindProperty ("reloadingWithAnimationActive");
		weaponsModeActive = serializedObject.FindProperty ("weaponsModeActive");
		shootingSingleWeapon = serializedObject.FindProperty ("shootingSingleWeapon");
		choosedWeapon = serializedObject.FindProperty ("choosedWeapon");
		usingFreeFireMode = serializedObject.FindProperty ("usingFreeFireMode");
		currentIKWeapon = serializedObject.FindProperty ("currentIKWeapon");
		usingDualWeapon = serializedObject.FindProperty ("usingDualWeapon");
		currentRighWeaponName = serializedObject.FindProperty ("currentRighWeaponName");
		currentLeftWeaponName = serializedObject.FindProperty ("currentLeftWeaponName");
		shootingRightWeapon = serializedObject.FindProperty ("shootingRightWeapon");
		shootingLeftWeapon = serializedObject.FindProperty ("shootingLeftWeapon");
		chooseDualWeaponIndex = serializedObject.FindProperty ("chooseDualWeaponIndex");
		currentRightIKWeapon = serializedObject.FindProperty ("currentRightIKWeapon");
		currentLeftIkWeapon = serializedObject.FindProperty ("currentLeftIkWeapon");
		showMainSettings = serializedObject.FindProperty ("showMainSettings");
		showWeaponsList = serializedObject.FindProperty ("showWeaponsList");
		showElementSettings = serializedObject.FindProperty ("showElementSettings");
		showDebugSettings = serializedObject.FindProperty ("showDebugSettings");
		weaponsSlotsAmount = serializedObject.FindProperty ("weaponsSlotsAmount");
		weaponsLayer = serializedObject.FindProperty ("weaponsLayer");

		targetToDamageLayer = serializedObject.FindProperty ("targetToDamageLayer");
		targetForScorchLayer = serializedObject.FindProperty ("targetForScorchLayer");

		useCustomLayerToCheck = serializedObject.FindProperty ("useCustomLayerToCheck");
		customLayerToCheck = serializedObject.FindProperty ("customLayerToCheck");

		useCustomIgnoreTags = serializedObject.FindProperty ("useCustomIgnoreTags");
		customTagsToIgnoreList = serializedObject.FindProperty ("customTagsToIgnoreList");

		setWeaponWhenPicked = serializedObject.FindProperty ("setWeaponWhenPicked");
		canGrabObjectsCarryingWeapons = serializedObject.FindProperty ("canGrabObjectsCarryingWeapons");
		changeToNextWeaponIfAmmoEmpty = serializedObject.FindProperty ("changeToNextWeaponIfAmmoEmpty");
		drawKeepWeaponWhenModeChanged = serializedObject.FindProperty ("drawKeepWeaponWhenModeChanged");
		changeWeaponsWithNumberKeysActive = serializedObject.FindProperty ("changeWeaponsWithNumberKeysActive");
		changeWeaponsWithMouseWheelActive = serializedObject.FindProperty ("changeWeaponsWithMouseWheelActive");
		changeWeaponsWithKeysActive = serializedObject.FindProperty ("changeWeaponsWithKeysActive");
		weaponsHUDEnabled = serializedObject.FindProperty ("weaponsHUDEnabled");
		disableWeaponsHUDAfterDelayEnabled = serializedObject.FindProperty ("disableWeaponsHUDAfterDelayEnabled");
		delayToDisableWeaponsHUD = serializedObject.FindProperty ("delayToDisableWeaponsHUD");
		weaponCursorActive = serializedObject.FindProperty ("weaponCursorActive");
		useWeaponCursorUnableToShootThirdPerson = serializedObject.FindProperty ("useWeaponCursorUnableToShootThirdPerson");
		useWeaponCursorUnableToShootFirstPerson = serializedObject.FindProperty ("useWeaponCursorUnableToShootFirstPerson");
		canFireWeaponsWithoutAiming = serializedObject.FindProperty ("canFireWeaponsWithoutAiming");
		drawWeaponIfFireButtonPressed = serializedObject.FindProperty ("drawWeaponIfFireButtonPressed");
		drawAndAimWeaponIfFireButtonPressed = serializedObject.FindProperty ("drawAndAimWeaponIfFireButtonPressed");

		keepWeaponAfterDelayThirdPerson = serializedObject.FindProperty ("keepWeaponAfterDelayThirdPerson");
		keepWeaponAfterDelayFirstPerson = serializedObject.FindProperty ("keepWeaponAfterDelayFirstPerson");
		keepWeaponDelay = serializedObject.FindProperty ("keepWeaponDelay");
		useQuickDrawWeapon = serializedObject.FindProperty ("useQuickDrawWeapon");
		timeToStopAimAfterStopFiring = serializedObject.FindProperty ("timeToStopAimAfterStopFiring");
		useAimCameraOnFreeFireMode = serializedObject.FindProperty ("useAimCameraOnFreeFireMode");
		pivotPointRotationActive = serializedObject.FindProperty ("pivotPointRotationActive");
		pivotPointRotationSpeed = serializedObject.FindProperty ("pivotPointRotationSpeed");
		runWhenAimingWeaponInThirdPerson = serializedObject.FindProperty ("runWhenAimingWeaponInThirdPerson");
		stopRunIfPreviouslyNotRunning = serializedObject.FindProperty ("stopRunIfPreviouslyNotRunning");
		runAlsoWhenFiringWithoutAiming = serializedObject.FindProperty ("runAlsoWhenFiringWithoutAiming");
		canJumpWhileAimingThirdPerson = serializedObject.FindProperty ("canJumpWhileAimingThirdPerson");
		canAimOnAirThirdPerson = serializedObject.FindProperty ("canAimOnAirThirdPerson");
		stopAimingOnAirThirdPerson = serializedObject.FindProperty ("stopAimingOnAirThirdPerson");
		ignoreParentWeaponOutsidePlayerBodyWhenNotActive = serializedObject.FindProperty ("ignoreParentWeaponOutsidePlayerBodyWhenNotActive");
		keepLookAtTargetActiveWhenFiringWithoutAiming = serializedObject.FindProperty ("keepLookAtTargetActiveWhenFiringWithoutAiming");
		ignoreCheckSurfaceCollisionThirdPerson = serializedObject.FindProperty ("ignoreCheckSurfaceCollisionThirdPerson");
		ignoreCheckSurfaceCollisionFirstPerson = serializedObject.FindProperty ("ignoreCheckSurfaceCollisionFirstPerson");

		fireWeaponsInputActive = serializedObject.FindProperty ("fireWeaponsInputActive");
		generalWeaponsInputActive = serializedObject.FindProperty ("generalWeaponsInputActive");
		storePickedWeaponsOnInventory = serializedObject.FindProperty ("storePickedWeaponsOnInventory");

		checkDurabilityOnObjectEnabled = serializedObject.FindProperty ("checkDurabilityOnObjectEnabled");

		useEventOnDurabilityEmptyOnMeleeWeapon = serializedObject.FindProperty ("useEventOnDurabilityEmptyOnMeleeWeapon");
		eventOnDurabilityEmptyOnMeleeWeapon = serializedObject.FindProperty ("eventOnDurabilityEmptyOnMeleeWeapon");

		drawWeaponWhenPicked = serializedObject.FindProperty ("drawWeaponWhenPicked");

		drawPickedWeaponOnlyItNotPreviousWeaponEquipped = serializedObject.FindProperty ("drawPickedWeaponOnlyItNotPreviousWeaponEquipped");

		changeToNextWeaponWhenEquipped = serializedObject.FindProperty ("changeToNextWeaponWhenEquipped");
		changeToNextWeaponWhenUnequipped = serializedObject.FindProperty ("changeToNextWeaponWhenUnequipped");
		notActivateWeaponsAtStart = serializedObject.FindProperty ("notActivateWeaponsAtStart");
		canStoreAnyNumberSameWeapon = serializedObject.FindProperty ("canStoreAnyNumberSameWeapon");
		useAmmoFromInventoryInAllWeapons = serializedObject.FindProperty ("useAmmoFromInventoryInAllWeapons");
		openWeaponAttachmentsMenuEnabled = serializedObject.FindProperty ("openWeaponAttachmentsMenuEnabled");
		setFirstPersonForAttachmentEditor = serializedObject.FindProperty ("setFirstPersonForAttachmentEditor");
		useUniversalAttachments = serializedObject.FindProperty ("useUniversalAttachments");
		startGameWithCurrentWeapon = serializedObject.FindProperty ("startGameWithCurrentWeapon");
		drawInitialWeaponSelected = serializedObject.FindProperty ("drawInitialWeaponSelected");
		startWithFirstWeaponAvailable = serializedObject.FindProperty ("startWithFirstWeaponAvailable");
		startGameWithDualWeapons = serializedObject.FindProperty ("startGameWithDualWeapons");
		avaliableWeaponList = serializedObject.FindProperty ("avaliableWeaponList");

		rightWeaponToStartIndex = serializedObject.FindProperty ("rightWeaponToStartIndex");
		leftWeaponToStartIndex = serializedObject.FindProperty ("leftWeaponToStartIndex");
		rightWeaponToStartName = serializedObject.FindProperty ("rightWeaponToStartName");
		leftWeaponToStartName = serializedObject.FindProperty ("leftWeaponToStartName");
		weaponToStartIndex = serializedObject.FindProperty ("weaponToStartIndex");
		weaponToStartName = serializedObject.FindProperty ("weaponToStartName");
		canDropWeapons = serializedObject.FindProperty ("canDropWeapons");
		changeToNextWeaponWhenDrop = serializedObject.FindProperty ("changeToNextWeaponWhenDrop");
		dropWeaponForceThirdPerson = serializedObject.FindProperty ("dropWeaponForceThirdPerson");
		dropWeaponForceFirstPerson = serializedObject.FindProperty ("dropWeaponForceFirstPerson");
		holdDropButtonToIncreaseForce = serializedObject.FindProperty ("holdDropButtonToIncreaseForce");
		dropIncreaseForceSpeed = serializedObject.FindProperty ("dropIncreaseForceSpeed");
		maxDropForce = serializedObject.FindProperty ("maxDropForce");
		dropWeaponInventoryObjectsPickups = serializedObject.FindProperty ("dropWeaponInventoryObjectsPickups");
		dropCurrentWeaponWhenDie = serializedObject.FindProperty ("dropCurrentWeaponWhenDie");
		dropAllWeaponsWhenDie = serializedObject.FindProperty ("dropAllWeaponsWhenDie");
		dropWeaponsOnlyIfUsing = serializedObject.FindProperty ("dropWeaponsOnlyIfUsing");
		drawWeaponWhenResurrect = serializedObject.FindProperty ("drawWeaponWhenResurrect");
		canMarkTargets = serializedObject.FindProperty ("canMarkTargets");
		useAimAssistInThirdPerson = serializedObject.FindProperty ("useAimAssistInThirdPerson");
		useAimAssistInFirstPerson = serializedObject.FindProperty ("useAimAssistInFirstPerson");
		useMaxDistanceToCameraCenterAimAssist = serializedObject.FindProperty ("useMaxDistanceToCameraCenterAimAssist");
		maxDistanceToCameraCenterAimAssist = serializedObject.FindProperty ("maxDistanceToCameraCenterAimAssist");
		useAimAssistInLockedCamera = serializedObject.FindProperty ("useAimAssistInLockedCamera");
		aimAssistLookAtTargetSpeed = serializedObject.FindProperty ("aimAssistLookAtTargetSpeed");
		damageMultiplierStat = serializedObject.FindProperty ("damageMultiplierStat");
		extraDamageStat = serializedObject.FindProperty ("extraDamageStat");
		spreadMultiplierStat = serializedObject.FindProperty ("spreadMultiplierStat");
		fireRateMultiplierStat = serializedObject.FindProperty ("fireRateMultiplierStat");
		extraReloadSpeedStat = serializedObject.FindProperty ("extraReloadSpeedStat");

		magazineExtraSizeStat = serializedObject.FindProperty ("magazineExtraSizeStat");
		cantPickWeaponMessage = serializedObject.FindProperty ("cantPickWeaponMessage");
		cantDropCurrentWeaponMessage = serializedObject.FindProperty ("cantDropCurrentWeaponMessage");
		cantPickAttachmentMessage = serializedObject.FindProperty ("cantPickAttachmentMessage");
		weaponMessageDuration = serializedObject.FindProperty ("weaponMessageDuration");
		useEventsOnStateChange = serializedObject.FindProperty ("useEventsOnStateChange");
		evenOnStateEnabled = serializedObject.FindProperty ("evenOnStateEnabled");
		eventOnStateDisabled = serializedObject.FindProperty ("eventOnStateDisabled");

		loadCurrentPlayerWeaponsFromSaveFile = serializedObject.FindProperty ("loadCurrentPlayerWeaponsFromSaveFile");
		saveCurrentPlayerWeaponsToSaveFile = serializedObject.FindProperty ("saveCurrentPlayerWeaponsToSaveFile");

		loadWeaponAttachmentsInfoFromSaveFile = serializedObject.FindProperty ("loadWeaponAttachmentsInfoFromSaveFile");
		saveWeaponAttachmentsInfoToSaveFile = serializedObject.FindProperty ("saveWeaponAttachmentsInfoToSaveFile");

		showRemainAmmoText = serializedObject.FindProperty ("showRemainAmmoText");

		minSwipeDist = serializedObject.FindProperty ("minSwipeDist");
		usedByAI = serializedObject.FindProperty ("usedByAI");
		weaponsList = serializedObject.FindProperty ("weaponsList");
		weaponPocketList = serializedObject.FindProperty ("weaponPocketList");
		weaponsHUD = serializedObject.FindProperty ("weaponsHUD");
		singleWeaponHUD = serializedObject.FindProperty ("singleWeaponHUD");
		currentWeaponNameText = serializedObject.FindProperty ("currentWeaponNameText");
		currentWeaponAmmoText = serializedObject.FindProperty ("currentWeaponAmmoText");
		ammoSlider = serializedObject.FindProperty ("ammoSlider");
		currentWeaponIcon = serializedObject.FindProperty ("currentWeaponIcon");
		attachmentPanel = serializedObject.FindProperty ("attachmentPanel");
		attachmentAmmoText = serializedObject.FindProperty ("attachmentAmmoText");
		currentAttachmentIcon = serializedObject.FindProperty ("currentAttachmentIcon");
		dualWeaponHUD = serializedObject.FindProperty ("dualWeaponHUD");
		currentRightWeaponAmmoText = serializedObject.FindProperty ("currentRightWeaponAmmoText");

		rightAttachmentPanel = serializedObject.FindProperty ("rightAttachmentPanel");
		rigthAttachmentAmmoText = serializedObject.FindProperty ("rigthAttachmentAmmoText");
		currentRightAttachmentIcon = serializedObject.FindProperty ("currentRightAttachmentIcon");
		currentLeftWeaponAmmoText = serializedObject.FindProperty ("currentLeftWeaponAmmoText");
		leftAttachmentPanel = serializedObject.FindProperty ("leftAttachmentPanel");
		leftAttachmentAmmoText = serializedObject.FindProperty ("leftAttachmentAmmoText");
		currentLeftAttachmentIcon = serializedObject.FindProperty ("currentLeftAttachmentIcon");
		weaponsParent = serializedObject.FindProperty ("weaponsParent");
		weaponsTransformInFirstPerson = serializedObject.FindProperty ("weaponsTransformInFirstPerson");
		weaponsTransformInThirdPerson = serializedObject.FindProperty ("weaponsTransformInThirdPerson");
		thirdPersonParent = serializedObject.FindProperty ("thirdPersonParent");
		firstPersonParent = serializedObject.FindProperty ("firstPersonParent");
		cameraController = serializedObject.FindProperty ("cameraController");
		weaponsCamera = serializedObject.FindProperty ("weaponsCamera");
		weaponCursor = serializedObject.FindProperty ("weaponCursor");
		cursorRectTransform = serializedObject.FindProperty ("cursorRectTransform");
		weaponCursorRegular = serializedObject.FindProperty ("weaponCursorRegular");
		weaponCursorAimingInFirstPerson = serializedObject.FindProperty ("weaponCursorAimingInFirstPerson");
		weaponCursorUnableToShoot = serializedObject.FindProperty ("weaponCursorUnableToShoot");
		weaponCustomReticle = serializedObject.FindProperty ("weaponCustomReticle");
		weaponsMessageWindow = serializedObject.FindProperty ("weaponsMessageWindow");
		mainObjectTransformData = serializedObject.FindProperty ("mainObjectTransformData");
		temporalParentForWeapons = serializedObject.FindProperty ("temporalParentForWeapons");
		pauseManager = serializedObject.FindProperty ("pauseManager");
		playerScreenObjectivesManager = serializedObject.FindProperty ("playerScreenObjectivesManager");
		playerCameraManager = serializedObject.FindProperty ("playerCameraManager");

		headBobManager = serializedObject.FindProperty ("headBobManager");
		playerInput = serializedObject.FindProperty ("playerInput");
		IKManager = serializedObject.FindProperty ("IKManager");
		playerManager = serializedObject.FindProperty ("playerManager");

		upperBodyRotationManager = serializedObject.FindProperty ("upperBodyRotationManager");
		headTrackManager = serializedObject.FindProperty ("headTrackManager");
		grabObjectsManager = serializedObject.FindProperty ("grabObjectsManager");
	
		playerInventoryManager = serializedObject.FindProperty ("playerInventoryManager");
		mainInventoryListManager = serializedObject.FindProperty ("mainInventoryListManager");
		usingDevicesManager = serializedObject.FindProperty ("usingDevicesManager");
		mainCollider = serializedObject.FindProperty ("mainCollider");
		ragdollManager = serializedObject.FindProperty ("ragdollManager");
		mainCameraTransform = serializedObject.FindProperty ("mainCameraTransform");
		mainCamera = serializedObject.FindProperty ("mainCamera");
		mainWeaponListManager = serializedObject.FindProperty ("mainWeaponListManager");

		rightHandTransform = serializedObject.FindProperty ("rightHandTransform");
		leftHandTransform = serializedObject.FindProperty ("leftHandTransform");

		rightHandMountPoint = serializedObject.FindProperty ("rightHandMountPoint");
		leftHandMountPoint = serializedObject.FindProperty ("leftHandMountPoint");

		showDebugLog = serializedObject.FindProperty ("showDebugLog");

		mainSimpleSniperSightSystem = serializedObject.FindProperty ("mainSimpleSniperSightSystem");

		activateDualWeaponsEnabled = serializedObject.FindProperty ("activateDualWeaponsEnabled");

		useCameraShakeOnShootEnabled = serializedObject.FindProperty ("useCameraShakeOnShootEnabled");

		useUsableWeaponPrefabInfoList = serializedObject.FindProperty ("useUsableWeaponPrefabInfoList");
		usableWeaponPrefabInfoList = serializedObject.FindProperty ("usableWeaponPrefabInfoList");

		ignoreUseDrawKeepWeaponAnimation = serializedObject.FindProperty ("ignoreUseDrawKeepWeaponAnimation");

		removeCurrentWeaponAsNotUsableIfAmmoEmptyEnabled = serializedObject.FindProperty ("removeCurrentWeaponAsNotUsableIfAmmoEmptyEnabled");

		weaponsCanBeStolenFromCharacter = serializedObject.FindProperty ("weaponsCanBeStolenFromCharacter");

		useEventOnWeaponStolen = serializedObject.FindProperty ("useEventOnWeaponStolen");
		eventOnWeaponStolen = serializedObject.FindProperty ("eventOnWeaponStolen");

		manager = (playerWeaponsManager)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		applicationIsPlaying = Application.isPlaying;

		GUILayout.BeginVertical ("Player Weapons State", "window");

		carryingWeaponMode = "Not Carrying";

		if (applicationIsPlaying) {
			if (carryingWeaponInThirdPerson.boolValue) {
				carryingWeaponMode = "Third Person";
			} 
			if (carryingWeaponInFirstPerson.boolValue) {
				carryingWeaponMode = "First Person";
			}
		} 
		GUILayout.Label ("Carrying Weapon\t\t" + carryingWeaponMode);

		aimingWeaponMode = "Not Aiming";
		if (applicationIsPlaying) {
			if (aimingInThirdPerson.boolValue) {
				aimingWeaponMode = "Third Person";
			} 
			if (aimingInFirstPerson.boolValue) {
				aimingWeaponMode = "First Person";
			}
		} 
		GUILayout.Label ("Aiming Weapon\t\t" + aimingWeaponMode);

		currentState = "Not Using";
		if (applicationIsPlaying) {
			if (anyWeaponAvailable.boolValue) {
				if (carryingWeaponInThirdPerson.boolValue) {
					currentState = "Carrying Weapon 3st";
				} else if (carryingWeaponInFirstPerson.boolValue) {
					currentState = "Carrying Weapon 1st";
				} else {
					currentState = "Not Carrying";
				}
			} else {
				currentState = "Not Available";
			}
		} 
		GUILayout.Label ("Current State\t\t" + currentState);

		currentWeaponText = "None";
		if (applicationIsPlaying) {
			if (anyWeaponAvailable.boolValue) {
				currentWeaponText = currentWeaponName.stringValue;
			}
		} 

		GUILayout.Label ("Weapons Available \t\t" + anyWeaponAvailable.boolValue);
		GUILayout.Label ("Changing Weapon \t\t" + changingWeapon.boolValue);
		GUILayout.Label ("Keeping Weapon \t\t" + keepingWeapon.boolValue);
		GUILayout.Label ("Editing Attachments \t\t" + editingWeaponAttachments.boolValue);
		GUILayout.Label ("Avoid Shoot Active \t\t" + avoidShootCurrentlyActive.boolValue);
		GUILayout.Label ("Reload Anim Active \t\t" + reloadingWithAnimationActive.boolValue);

		weaponsModeActiveText = "NO";
		if (weaponsModeActive.boolValue) {
			weaponsModeActiveText = "YES";
		}
		GUILayout.Label ("Weapons Mode Active \t\t" + weaponsModeActiveText);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Single Weapon State", "window");
		GUILayout.Label ("Current Weapon \t\t" + currentWeaponText);

		shootingWeapon = "NO";
		if (shootingSingleWeapon.boolValue) {
			shootingWeapon = "YES";
		}
		GUILayout.Label ("Shooting Weapon \t\t" + shootingWeapon);
		GUILayout.Label ("Weapon Index \t\t" + choosedWeapon.intValue);
		GUILayout.Label ("Using Free Fire Mode \t\t" + usingFreeFireMode.boolValue);
		EditorGUILayout.PropertyField (currentIKWeapon);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dual Weapon State", "window");
		GUILayout.Label ("Using Dual Weapons \t\t" + usingDualWeapon.boolValue);
		GUILayout.Label ("Current R Weapon \t\t" + currentRighWeaponName.stringValue);
		GUILayout.Label ("Current L Weapon \t\t" + currentLeftWeaponName.stringValue);
		GUILayout.Label ("Shooting R Weapon \t\t" + shootingRightWeapon.boolValue);
		GUILayout.Label ("Shooting L Weapon \t\t" + shootingLeftWeapon.boolValue);
		GUILayout.Label ("Dual Slot Index \t\t" + chooseDualWeaponIndex.intValue);
		EditorGUILayout.PropertyField (currentRightIKWeapon);
		EditorGUILayout.PropertyField (currentLeftIkWeapon);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 30;
		style.alignment = TextAnchor.MiddleCenter;

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();

		if (showMainSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Main Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Main Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showMainSettings.boolValue = !showMainSettings.boolValue;
		}

		if (showWeaponsList.boolValue) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Weapons List";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Weapons List";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showWeaponsList.boolValue = !showWeaponsList.boolValue;
		}

		if (showElementSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Element Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Element Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showElementSettings.boolValue = !showElementSettings.boolValue;
		}
			
		if (showDebugSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Debug Options";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Debug Options";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showDebugSettings.boolValue = !showDebugSettings.boolValue;
		}

		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		if (GUILayout.Button ("Select Weapons On Editor")) {
			manager.selectFirstWeaponGameObjectOnEditor ();
		}

		if (showMainSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("MAIN SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure the max amount of weapons and the layer used in weapons", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window");
			EditorGUILayout.PropertyField (weaponsSlotsAmount);
			EditorGUILayout.PropertyField (weaponsLayer);

			EditorGUILayout.PropertyField (setWeaponWhenPicked);
			EditorGUILayout.PropertyField (canGrabObjectsCarryingWeapons);
			EditorGUILayout.PropertyField (changeToNextWeaponIfAmmoEmpty);	
			EditorGUILayout.PropertyField (drawKeepWeaponWhenModeChanged);	
			EditorGUILayout.PropertyField (changeWeaponsWithNumberKeysActive);	
			EditorGUILayout.PropertyField (changeWeaponsWithMouseWheelActive);	
			EditorGUILayout.PropertyField (changeWeaponsWithKeysActive);	

			EditorGUILayout.PropertyField (ignoreUseDrawKeepWeaponAnimation);	
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Target Detection Settings", "window");
			EditorGUILayout.PropertyField (targetToDamageLayer);
			EditorGUILayout.PropertyField (targetForScorchLayer);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useCustomLayerToCheck);
			if (useCustomLayerToCheck.boolValue) {
				EditorGUILayout.PropertyField (customLayerToCheck);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useCustomIgnoreTags);
			if (useCustomIgnoreTags.boolValue) {
				showSimpleList (customTagsToIgnoreList, "Custom Tags To Ignore List");
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon HUD Settings", "window");
			EditorGUILayout.PropertyField (weaponsHUDEnabled);
			EditorGUILayout.PropertyField (disableWeaponsHUDAfterDelayEnabled);
			if (disableWeaponsHUDAfterDelayEnabled.boolValue) {
				EditorGUILayout.PropertyField (delayToDisableWeaponsHUD);
			}
			EditorGUILayout.PropertyField (showRemainAmmoText);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Reticle Settings", "window");
			EditorGUILayout.PropertyField (weaponCursorActive);
			EditorGUILayout.PropertyField (useWeaponCursorUnableToShootThirdPerson);
			EditorGUILayout.PropertyField (useWeaponCursorUnableToShootFirstPerson);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Management Settings", "window");
			EditorGUILayout.PropertyField (canFireWeaponsWithoutAiming);	
			EditorGUILayout.PropertyField (drawWeaponIfFireButtonPressed);	
			EditorGUILayout.PropertyField (drawAndAimWeaponIfFireButtonPressed);	

			EditorGUILayout.PropertyField (keepWeaponAfterDelayThirdPerson);	
			EditorGUILayout.PropertyField (keepWeaponAfterDelayFirstPerson);	
			if (keepWeaponAfterDelayThirdPerson.boolValue || keepWeaponAfterDelayFirstPerson.boolValue) {
				EditorGUILayout.PropertyField (keepWeaponDelay);	
			} 
			EditorGUILayout.PropertyField (useQuickDrawWeapon);	
			EditorGUILayout.PropertyField (timeToStopAimAfterStopFiring);
			EditorGUILayout.PropertyField (useAimCameraOnFreeFireMode);
			EditorGUILayout.PropertyField (pivotPointRotationActive);
			if (pivotPointRotationActive.boolValue) {
				EditorGUILayout.PropertyField (pivotPointRotationSpeed);
			}
			EditorGUILayout.PropertyField (runWhenAimingWeaponInThirdPerson);
			if (runWhenAimingWeaponInThirdPerson.boolValue) {
				EditorGUILayout.PropertyField (stopRunIfPreviouslyNotRunning);
				EditorGUILayout.PropertyField (runAlsoWhenFiringWithoutAiming);
			}
			EditorGUILayout.PropertyField (canJumpWhileAimingThirdPerson);
			EditorGUILayout.PropertyField (canAimOnAirThirdPerson);
			EditorGUILayout.PropertyField (stopAimingOnAirThirdPerson);
//			EditorGUILayout.PropertyField (originalWeaponsCameraFov);
			EditorGUILayout.PropertyField (ignoreParentWeaponOutsidePlayerBodyWhenNotActive);
			EditorGUILayout.PropertyField (keepLookAtTargetActiveWhenFiringWithoutAiming);

			EditorGUILayout.PropertyField (useCameraShakeOnShootEnabled);

			EditorGUILayout.PropertyField (removeCurrentWeaponAsNotUsableIfAmmoEmptyEnabled);	
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Dual Wield Fire Weapons Settings", "window");
			EditorGUILayout.PropertyField (activateDualWeaponsEnabled);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapons Collision Detection Settings", "window");
			EditorGUILayout.PropertyField (ignoreCheckSurfaceCollisionThirdPerson);
			EditorGUILayout.PropertyField (ignoreCheckSurfaceCollisionFirstPerson);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapons Input Settings", "window");
			EditorGUILayout.PropertyField (fireWeaponsInputActive);
			EditorGUILayout.PropertyField (generalWeaponsInputActive);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Objects Durability Settings", "window");
			EditorGUILayout.PropertyField (checkDurabilityOnObjectEnabled);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useEventOnDurabilityEmptyOnMeleeWeapon);
			if (useEventOnDurabilityEmptyOnMeleeWeapon.boolValue) {
				EditorGUILayout.PropertyField (eventOnDurabilityEmptyOnMeleeWeapon);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Inventory Settings", "window");
			EditorGUILayout.PropertyField (storePickedWeaponsOnInventory);
			EditorGUILayout.PropertyField (drawWeaponWhenPicked);
			EditorGUILayout.PropertyField (drawPickedWeaponOnlyItNotPreviousWeaponEquipped);
			EditorGUILayout.PropertyField (changeToNextWeaponWhenEquipped);
			EditorGUILayout.PropertyField (changeToNextWeaponWhenUnequipped);
			EditorGUILayout.PropertyField (notActivateWeaponsAtStart);
			EditorGUILayout.PropertyField (canStoreAnyNumberSameWeapon);
			EditorGUILayout.PropertyField (useAmmoFromInventoryInAllWeapons);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Attachment Settings", "window");
			EditorGUILayout.PropertyField (openWeaponAttachmentsMenuEnabled);
			EditorGUILayout.PropertyField (setFirstPersonForAttachmentEditor);
			EditorGUILayout.PropertyField (useUniversalAttachments);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Initial Weapon Settings", "window");
			EditorGUILayout.PropertyField (startGameWithCurrentWeapon);
			if (startGameWithCurrentWeapon.boolValue) {
				EditorGUILayout.PropertyField (drawInitialWeaponSelected);
				EditorGUILayout.PropertyField (startWithFirstWeaponAvailable);
				if (!startWithFirstWeaponAvailable.boolValue) {
					EditorGUILayout.PropertyField (startGameWithDualWeapons);

					if (avaliableWeaponList.arraySize > 0) {
						if (startGameWithDualWeapons.boolValue) {
							if (rightWeaponToStartIndex.intValue < manager.avaliableWeaponList.Length &&
							    leftWeaponToStartIndex.intValue < manager.avaliableWeaponList.Length) {

								rightWeaponToStartIndex.intValue = 
								EditorGUILayout.Popup ("Right Weapon To Start", rightWeaponToStartIndex.intValue, manager.avaliableWeaponList);

								rightWeaponToStartName.stringValue = manager.avaliableWeaponList [rightWeaponToStartIndex.intValue];

								leftWeaponToStartIndex.intValue = 
								EditorGUILayout.Popup ("Left Weapon To Start", leftWeaponToStartIndex.intValue, manager.avaliableWeaponList);

								leftWeaponToStartName.stringValue = manager.avaliableWeaponList [leftWeaponToStartIndex.intValue];
							}
						} else {
							if (weaponToStartIndex.intValue < manager.avaliableWeaponList.Length) {
								weaponToStartIndex.intValue = 
								EditorGUILayout.Popup ("Weapon To Start", weaponToStartIndex.intValue, manager.avaliableWeaponList);
								
								weaponToStartName.stringValue = manager.avaliableWeaponList [weaponToStartIndex.intValue];
							}
						}
					}

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Update Available Weapon List")) {
						if (!Application.isPlaying) {
//							if (storePickedWeaponsOnInventory.boolValue) {
//								manager.getWeaponListString ();
//							} else {
							manager.getAvailableWeaponListString ();
//							}
						}
					}
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Drop Weapon Settings", "window");
			EditorGUILayout.PropertyField (canDropWeapons);
			EditorGUILayout.PropertyField (changeToNextWeaponWhenDrop);
			EditorGUILayout.PropertyField (dropWeaponForceThirdPerson);
			EditorGUILayout.PropertyField (dropWeaponForceFirstPerson);
			EditorGUILayout.PropertyField (holdDropButtonToIncreaseForce);
			if (holdDropButtonToIncreaseForce.boolValue) {
				EditorGUILayout.PropertyField (dropIncreaseForceSpeed);
				EditorGUILayout.PropertyField (maxDropForce);
			}
			EditorGUILayout.PropertyField (dropWeaponInventoryObjectsPickups);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Death Settings", "window");
			EditorGUILayout.PropertyField (dropCurrentWeaponWhenDie);
			EditorGUILayout.PropertyField (dropAllWeaponsWhenDie);
			EditorGUILayout.PropertyField (dropWeaponsOnlyIfUsing);
			EditorGUILayout.PropertyField (drawWeaponWhenResurrect);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Mark Targets Settings", "window");
			EditorGUILayout.PropertyField (canMarkTargets);
			GUILayout.EndVertical ();
		
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Aim Assist Settings", "window");
			EditorGUILayout.PropertyField (useAimAssistInThirdPerson);
			EditorGUILayout.PropertyField (useAimAssistInFirstPerson);
			EditorGUILayout.PropertyField (useMaxDistanceToCameraCenterAimAssist);
			if (useMaxDistanceToCameraCenterAimAssist.boolValue) {
				EditorGUILayout.PropertyField (maxDistanceToCameraCenterAimAssist);
			}
			EditorGUILayout.PropertyField (useAimAssistInLockedCamera);
			EditorGUILayout.PropertyField (aimAssistLookAtTargetSpeed);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapons Stats Settings", "window");
			EditorGUILayout.PropertyField (damageMultiplierStat);
			EditorGUILayout.PropertyField (extraDamageStat);
			EditorGUILayout.PropertyField (spreadMultiplierStat);
			EditorGUILayout.PropertyField (fireRateMultiplierStat);
			EditorGUILayout.PropertyField (extraReloadSpeedStat);
			EditorGUILayout.PropertyField (magazineExtraSizeStat);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapons Message Settings", "window");
			EditorGUILayout.PropertyField (cantPickWeaponMessage);
			EditorGUILayout.PropertyField (cantDropCurrentWeaponMessage);
			EditorGUILayout.PropertyField (cantPickAttachmentMessage);
			EditorGUILayout.PropertyField (weaponMessageDuration);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapons Mode Active Event Settings", "window");
			EditorGUILayout.PropertyField (useEventsOnStateChange);
			if (useEventsOnStateChange.boolValue) {
				EditorGUILayout.PropertyField (evenOnStateEnabled);
				EditorGUILayout.PropertyField (eventOnStateDisabled);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Stolen Weapon Settings", "window");
			EditorGUILayout.PropertyField (weaponsCanBeStolenFromCharacter);	
			EditorGUILayout.PropertyField (useEventOnWeaponStolen);
			if (useEventOnWeaponStolen.boolValue) {
				EditorGUILayout.PropertyField (eventOnWeaponStolen);
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Save/Load Weapons Settings", "window");
			EditorGUILayout.PropertyField (loadCurrentPlayerWeaponsFromSaveFile);
			EditorGUILayout.PropertyField (saveCurrentPlayerWeaponsToSaveFile);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (loadWeaponAttachmentsInfoFromSaveFile);
			EditorGUILayout.PropertyField (saveWeaponAttachmentsInfoToSaveFile);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Touch Settings", "window");
			EditorGUILayout.PropertyField (minSwipeDist);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("AI Settings", "window");
			EditorGUILayout.PropertyField (usedByAI);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}
			
		if (showWeaponsList.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("WEAPON LIST", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure every weapon added to the character", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Usable Weapon Prefab List", "window");
			EditorGUILayout.PropertyField (useUsableWeaponPrefabInfoList);
			if (useUsableWeaponPrefabInfoList.boolValue) {
				showUsableWeaponPrefabInfoList (usableWeaponPrefabInfoList);

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Instantiate Weapons On Character")) {
					manager.instantiateUsableWeaponPrefabInfoListFromEditor ();
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 20;
			style.alignment = TextAnchor.MiddleLeft;

			GUILayout.BeginVertical ("Weapon List", "window");
			showWeaponList (weaponsList);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Get Weapon List")) {
				manager.setWeaponList ();
			}

			EditorGUILayout.Space ();
		
			if (GUILayout.Button ("Clear Weapon List")) {
				manager.clearWeaponList ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Save Weapon List")) {
				manager.saveCurrentWeaponListToFile ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Enable Weapon GameObject List")) {
				manager.enableOrDisableWeaponGameObjectList (true);
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Disable Weapon GameObject List")) {
				manager.enableOrDisableWeaponGameObjectList (false);
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Pocket List", "window");
			showWeaponPocketList (weaponPocketList);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showElementSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("ELEMENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure every gameObject used for the weapons", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("HUD Elements", "window");
			EditorGUILayout.PropertyField (weaponsHUD);
			EditorGUILayout.PropertyField (singleWeaponHUD);
			EditorGUILayout.PropertyField (currentWeaponNameText);
			EditorGUILayout.PropertyField (currentWeaponAmmoText);
			EditorGUILayout.PropertyField (ammoSlider);
			EditorGUILayout.PropertyField (currentWeaponIcon);
			EditorGUILayout.PropertyField (attachmentPanel);
			EditorGUILayout.PropertyField (attachmentAmmoText);
			EditorGUILayout.PropertyField (currentAttachmentIcon);


			EditorGUILayout.PropertyField (dualWeaponHUD);
			EditorGUILayout.PropertyField (currentRightWeaponAmmoText);
			EditorGUILayout.PropertyField (rightAttachmentPanel);
			EditorGUILayout.PropertyField (rigthAttachmentAmmoText);
			EditorGUILayout.PropertyField (currentRightAttachmentIcon);

			EditorGUILayout.PropertyField (currentLeftWeaponAmmoText);
			EditorGUILayout.PropertyField (leftAttachmentPanel);
			EditorGUILayout.PropertyField (leftAttachmentAmmoText);
			EditorGUILayout.PropertyField (currentLeftAttachmentIcon);

			EditorGUILayout.PropertyField (weaponsParent);
			EditorGUILayout.PropertyField (weaponsTransformInFirstPerson);
			EditorGUILayout.PropertyField (weaponsTransformInThirdPerson);
			EditorGUILayout.PropertyField (thirdPersonParent);
			EditorGUILayout.PropertyField (firstPersonParent);
			EditorGUILayout.PropertyField (cameraController);
			EditorGUILayout.PropertyField (weaponsCamera);
			EditorGUILayout.PropertyField (weaponCursor);
			EditorGUILayout.PropertyField (cursorRectTransform);
			EditorGUILayout.PropertyField (weaponCursorRegular);
			EditorGUILayout.PropertyField (weaponCursorAimingInFirstPerson);
			EditorGUILayout.PropertyField (weaponCursorUnableToShoot);
			EditorGUILayout.PropertyField (weaponCustomReticle);

			EditorGUILayout.PropertyField (weaponsMessageWindow);
			EditorGUILayout.PropertyField (mainObjectTransformData);
			EditorGUILayout.PropertyField (temporalParentForWeapons);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Elements", "window");
			EditorGUILayout.PropertyField (pauseManager);	
			EditorGUILayout.PropertyField (playerScreenObjectivesManager);	

			EditorGUILayout.PropertyField (playerCameraManager);	
			EditorGUILayout.PropertyField (headBobManager);	
			EditorGUILayout.PropertyField (playerInput);	
			EditorGUILayout.PropertyField (IKManager);	
			EditorGUILayout.PropertyField (playerManager);	
			EditorGUILayout.PropertyField (upperBodyRotationManager);	
			EditorGUILayout.PropertyField (headTrackManager);	
			EditorGUILayout.PropertyField (grabObjectsManager);	

			EditorGUILayout.PropertyField (playerInventoryManager);	
			EditorGUILayout.PropertyField (mainInventoryListManager);	
			EditorGUILayout.PropertyField (usingDevicesManager);	
			EditorGUILayout.PropertyField (mainCollider);	
			EditorGUILayout.PropertyField (ragdollManager);	
			EditorGUILayout.PropertyField (mainCameraTransform);	
			EditorGUILayout.PropertyField (mainCamera);	
			EditorGUILayout.PropertyField (mainWeaponListManager);
			EditorGUILayout.PropertyField (mainSimpleSniperSightSystem);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("IK Hands Transform", "window");
			EditorGUILayout.PropertyField (rightHandTransform);
			EditorGUILayout.PropertyField (leftHandTransform);

			EditorGUILayout.PropertyField (rightHandMountPoint);
			EditorGUILayout.PropertyField (leftHandMountPoint);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showDebugSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("DEBUG OPTIONS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Debug options for weapon in-game", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Debug Controls", "window");

			EditorGUILayout.PropertyField (showDebugLog);
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Set Next Weapon")) {
				manager.chooseNextWeapon (false, true);
			}
			if (GUILayout.Button ("Set Previous Weapon")) {
				manager.choosePreviousWeapon (false, true);
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Draw/Keep Weapon")) {
				manager.drawOrKeepWeaponInput ();
			}
			if (GUILayout.Button ("Drop Weapon")) {
				manager.dropWeaponByBebugButton ();
			}
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("Aim Weapon")) {
				manager.aimCurrentWeaponInput ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showWeaponListElementInfo (SerializedProperty list)
	{
		_currentIKWeapon = list.objectReferenceValue as IKWeaponSystem;

		if (_currentIKWeapon != null) {

			GUILayout.BeginVertical ();
			GUILayout.Label (_currentIKWeapon.weaponSystemManager.weaponSettings.Name, style);

			EditorGUILayout.PropertyField (list, new GUIContent ("", null, ""), false);

			_currentIKWeapon.weaponEnabled = EditorGUILayout.Toggle ("Enabled", _currentIKWeapon.weaponEnabled);
			_currentIKWeapon.weaponSystemManager.weaponSettings.numberKey = EditorGUILayout.IntField ("Number Key:", _currentIKWeapon.weaponSystemManager.weaponSettings.numberKey);

			GUILayout.Label ("Is Current Weapon\t" + _currentIKWeapon.currentWeapon);
			EditorUtility.SetDirty (_currentIKWeapon);
			GUILayout.EndVertical ();
		}
	}

	void showWeaponList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();


		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Expand All")) {
			list.isExpanded = true;
		}
		if (GUILayout.Button ("Collapse All")) {
			list.isExpanded = false;
		}
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Weapons: \t" + list.arraySize);

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				
				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					showWeaponListElementInfo (list.GetArrayElementAtIndex (i));
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginVertical ();
				if (GUILayout.Button ("x")) {
					manager.removeWeaponFromList (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (GUILayout.Button ("O")) {
					manager.selectWeaponOnListInEditor (i);
				}
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();

			if (weaponsList.arraySize > 0) {
				if (GUILayout.Button ("Enable All Weapons")) {
					manager.enableOrDisableWeaponsList (true);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Disable All Weapon")) {
					manager.enableOrDisableWeaponsList (false);
				}

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Expand All")) {
					list.isExpanded = true;
				}
				if (GUILayout.Button ("Collapse All")) {
					list.isExpanded = false;
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}

	void showWeaponPocketList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Pockets: \t" + list.arraySize);

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Pocket")) {
				manager.addPocket ();
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showWeaponListOnPocket (list.GetArrayElementAtIndex (i), i);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					manager.removePocket (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showWeaponListOnPocket (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pocketTransform"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapons On Pocket List", "window");
		showWeaponOnPocketList (list.FindPropertyRelative ("weaponOnPocketList"), index);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showWeaponOnPocketList (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Subpockets: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Subpocket")) {
				manager.addSubPocket (index);
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginVertical ("box");
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showWeaponOnPocketListElement (list.GetArrayElementAtIndex (i), index, i);
					}
				}
				GUILayout.EndVertical ();

				GUILayout.BeginHorizontal ();

				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					manager.removeSubPocket (index, i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}

				GUILayout.EndHorizontal ();

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showWeaponOnPocketListElement (SerializedProperty list, int pocketIndex, int subPocketIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon List", "window");
		showWeaponOnPocketListElementList (list.FindPropertyRelative ("weaponList"), pocketIndex, subPocketIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showWeaponOnPocketListElementList (SerializedProperty list, int pocketIndex, int subPocketIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Weapons: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Weapon")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				
				GUILayout.BeginHorizontal ();
				GUILayout.BeginVertical ("box");
				if (i < list.arraySize && i >= 0) {
					string weaponName = "Weapon " + i;

					if (list.GetArrayElementAtIndex (i).objectReferenceValue) {
						weaponName = list.GetArrayElementAtIndex (i).objectReferenceValue.name;
					}
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (weaponName), false);
				}
				GUILayout.EndVertical ();

				GUILayout.BeginHorizontal ();

				if (GUILayout.Button ("x")) {
					manager.removeWeaponFromSubPocket (pocketIndex, subPocketIndex, i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}

				GUILayout.EndHorizontal ();

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of " + listName + "s: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add " + listName)) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showUsableWeaponPrefabInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Weapon Prefabs: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Weapon")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginVertical ("box");
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showUsableWeaponPrefabInfoListElement (list.GetArrayElementAtIndex (i));
					}
				}
				GUILayout.EndVertical ();

				GUILayout.BeginHorizontal ();

				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}

				GUILayout.EndHorizontal ();

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showUsableWeaponPrefabInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usableWeaponPrefab"));
		GUILayout.EndVertical ();
	}
}
#endif