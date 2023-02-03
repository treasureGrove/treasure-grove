using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(inventoryManager), true)]
public class inventoryManagerEditor : Editor
{
	SerializedProperty inventoryEnabled;
	SerializedProperty openInventoryMenuEnabled;

	SerializedProperty infiniteSlots;
	SerializedProperty setInfiniteSlotValuesOnSaveLoad;

	SerializedProperty inventorySlotAmount;
	SerializedProperty infiniteAmountPerSlot;
	SerializedProperty amountPerSlot;
	SerializedProperty combineElementsAtDrop;
	SerializedProperty buttonUsable;
	SerializedProperty buttonNotUsable;
	SerializedProperty useOnlyWhenNeededAmountToUseObject;
	SerializedProperty activeNumberOfObjectsToUseMenu;
	SerializedProperty setTotalAmountWhenDropObject;
	SerializedProperty configureNumberObjectsToUseRate;
	SerializedProperty fasterNumberObjectsToUseRate;
	SerializedProperty waitTimeToUseFasterNumberObjectsToUseRate;
	SerializedProperty useBlurUIPanel;
	SerializedProperty showInventoryObjectsName;
	SerializedProperty showObjectAmountIfEqualOne;
	SerializedProperty selectFirstInventoryObjectWhenOpeningMenu;
	SerializedProperty dropSingleObjectOnInfiniteAmount;

	SerializedProperty showAllSettings;
	SerializedProperty showWeightSettings;
	SerializedProperty showWeaponsSettings;
	SerializedProperty showExamineSettings;
	SerializedProperty showMessagesSettings;
	SerializedProperty showSoundsSettings;
	SerializedProperty showOthersSettings;
	SerializedProperty showEventSettings;
	SerializedProperty showSaveLoadSettings;
	SerializedProperty showDebugSettings;
	SerializedProperty showElementSettings;

	SerializedProperty checkWeightLimitToPickObjects;
	SerializedProperty mainInventoryWeightManager;
	SerializedProperty storePickedWeaponsOnInventory;

	SerializedProperty equipWeaponsWhenPicked;
	SerializedProperty equipPickedWeaponOnlyItNotPreviousWeaponEquipped;
	SerializedProperty changeToFireWeaponsModeWhenPickingFireWeapon;
	SerializedProperty changeToMeleeWeaponsModeWhenPickingMeleeWeapon;

	SerializedProperty examineObjectBeforeStoreEnabled;
	SerializedProperty distanceToPlaceObjectInCamera;
	SerializedProperty placeObjectInCameraSpeed;
	SerializedProperty numberOfRotationsObjectInCamera;
	SerializedProperty placeObjectInCameraRotationSpeed;
	SerializedProperty extraCameraFovOnExamineObjects;
	SerializedProperty rotationSpeed;
	SerializedProperty zoomSpeed;
	SerializedProperty maxZoomValue;
	SerializedProperty minZoomValue;

	SerializedProperty showMessageWhenObjectUsed;
	SerializedProperty usedObjectMessage;
	SerializedProperty fullInventoryMessage;
	SerializedProperty fullInventoryMessageTime;
	SerializedProperty combinedObjectMessage;
	SerializedProperty combineObjectMessageTime;
	SerializedProperty usedObjectMessageTime;
	SerializedProperty unableToUseObjectMessage;
	SerializedProperty nonNeededAmountAvaliable;
	SerializedProperty objectNotFoundMessage;
	SerializedProperty cantUseThisObjectHereMessage;
	SerializedProperty unableToCombineMessage;
	SerializedProperty canBeCombinedButObjectIsFullMessage;
	SerializedProperty notEnoughSpaceToCombineMessage;
	SerializedProperty weightLimitReachedMessage;
	SerializedProperty objectTooMuchHeavyToCarryMessage;
	SerializedProperty useAudioSounds;
	SerializedProperty mainAudioSource;
	SerializedProperty useInventoryOptionsOnSlot;
	SerializedProperty inventoryOptionsOnSlotPanel;
	SerializedProperty mainInventorySlotOptionsButtons;
	SerializedProperty usedByAI;

	SerializedProperty eventOnInventoryInitialized;
	SerializedProperty eventOnClickInventoryChange;
	SerializedProperty eventOnInventorySlotSelected;
	SerializedProperty eventOnInventorySlotUnSelected;
	SerializedProperty eventOnInventoryClosed;
	SerializedProperty eventOnInventoryOpened;
	SerializedProperty useEventIfSystemDisabled;
	SerializedProperty eventIfSystemDisabled;

	SerializedProperty eventOnInventoryListChange;

	SerializedProperty loadCurrentPlayerInventoryFromSaveFile;
	SerializedProperty saveCurrentPlayerInventoryToSaveFile;
	SerializedProperty inventoryOpened;
	SerializedProperty inventoryList;
	SerializedProperty inventoryPanel;
	SerializedProperty inventoryListContent;
	SerializedProperty inventoryListScrollRect;
	SerializedProperty objectIcon;
	SerializedProperty equipButton;
	SerializedProperty unequipButton;
	SerializedProperty useButtonImage;
	SerializedProperty equipButtonImage;
	SerializedProperty unequipButtonImage;
	SerializedProperty dropButtonImage;
	SerializedProperty combineButtonImage;
	SerializedProperty examineButtonImage;
	SerializedProperty discardButtonImage;
	SerializedProperty dropAllUnitsObjectButtonImage;
	SerializedProperty examineObjectPanel;
	SerializedProperty examineObjectName;
	SerializedProperty examineObjectDescription;
	SerializedProperty takeObjectInExaminePanelButton;
	SerializedProperty currentObjectName;
	SerializedProperty currentObjectInfo;
	SerializedProperty objectImage;

	SerializedProperty usableInventoryObjectInfoList;

	SerializedProperty inventoryCamera;
	SerializedProperty inventoryLight;

	SerializedProperty lookObjectsPosition;
	SerializedProperty emptyInventoryPrefab;
	SerializedProperty numberOfObjectsToUseMenu;
	SerializedProperty numberOfObjectsToUseMenuRectTransform;
	SerializedProperty numberOfObjectsToUseText;
	SerializedProperty numberOfObjectsToUseMenuPosition;
	SerializedProperty numberOfObjectsToDropMenuPosition;
	SerializedProperty inventorySlotsScrollbar;
	SerializedProperty inventoryObjectInforScrollbar;

	SerializedProperty mainMeleeWeaponsGrabbedManager;
	SerializedProperty weaponsManager;
	SerializedProperty playerControllerManager;
	SerializedProperty pauseManager;
	SerializedProperty playerInput;
	SerializedProperty mainInventoryListManager;
	SerializedProperty usingDevicesManager;
	SerializedProperty mainInventoryMenuPanelsSystem;
	SerializedProperty mainInventoryBankUISystem;
	SerializedProperty mainVendorUISystem;
	SerializedProperty inventoryListManagerList;

	SerializedProperty mainInventoryCharacterCustomizationSystem;

	SerializedProperty mainInventoryQuickAccessSlotsSystem;

	SerializedProperty mainInventoryManagerPrefab;

	SerializedProperty showDebugPrint;

	SerializedProperty destroyObjectsOnEmptyDurability;
	SerializedProperty dropObjectsOnEmptyDurability;
	SerializedProperty setObjectsAsBrokenOnEmptyDurability;

	SerializedProperty unequipObjectOnEmptyDurability;

	SerializedProperty brokenObjectsCantBeUsed;
	SerializedProperty brokenObjectsCantBeEquipped;

	SerializedProperty checkDurabilityOnObjectEnabled;

	inventoryManager manager;

	Color buttonColor;
	inventoryCaptureManager inventoryWindow;

	bool addInventoryObject;
	string isAdded;
	string menuOpened;
	string amountValue;
	bool isEquipped;
	string objectIsEquipped;
	int categoryIndex;
	int elementIndex;

	string buttonMessage;

	GUIStyle style = new GUIStyle ();

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		inventoryEnabled = serializedObject.FindProperty ("inventoryEnabled");
		openInventoryMenuEnabled = serializedObject.FindProperty ("openInventoryMenuEnabled");

		infiniteSlots = serializedObject.FindProperty ("infiniteSlots");
		setInfiniteSlotValuesOnSaveLoad = serializedObject.FindProperty ("setInfiniteSlotValuesOnSaveLoad");

		inventorySlotAmount = serializedObject.FindProperty ("inventorySlotAmount");
		infiniteAmountPerSlot = serializedObject.FindProperty ("infiniteAmountPerSlot");
		amountPerSlot = serializedObject.FindProperty ("amountPerSlot");
		combineElementsAtDrop = serializedObject.FindProperty ("combineElementsAtDrop");
		buttonUsable = serializedObject.FindProperty ("buttonUsable");
		buttonNotUsable = serializedObject.FindProperty ("buttonNotUsable");
		useOnlyWhenNeededAmountToUseObject = serializedObject.FindProperty ("useOnlyWhenNeededAmountToUseObject");
		activeNumberOfObjectsToUseMenu = serializedObject.FindProperty ("activeNumberOfObjectsToUseMenu");
		setTotalAmountWhenDropObject = serializedObject.FindProperty ("setTotalAmountWhenDropObject");
		configureNumberObjectsToUseRate = serializedObject.FindProperty ("configureNumberObjectsToUseRate");
		fasterNumberObjectsToUseRate = serializedObject.FindProperty ("fasterNumberObjectsToUseRate");
		waitTimeToUseFasterNumberObjectsToUseRate = serializedObject.FindProperty ("waitTimeToUseFasterNumberObjectsToUseRate");
		useBlurUIPanel = serializedObject.FindProperty ("useBlurUIPanel");
		showInventoryObjectsName = serializedObject.FindProperty ("showInventoryObjectsName");
		showObjectAmountIfEqualOne = serializedObject.FindProperty ("showObjectAmountIfEqualOne");
		selectFirstInventoryObjectWhenOpeningMenu = serializedObject.FindProperty ("selectFirstInventoryObjectWhenOpeningMenu");
		dropSingleObjectOnInfiniteAmount = serializedObject.FindProperty ("dropSingleObjectOnInfiniteAmount");
		showAllSettings = serializedObject.FindProperty ("showAllSettings");
		showWeightSettings = serializedObject.FindProperty ("showWeightSettings");
		showWeaponsSettings = serializedObject.FindProperty ("showWeaponsSettings");
		showExamineSettings = serializedObject.FindProperty ("showExamineSettings");
		showMessagesSettings = serializedObject.FindProperty ("showMessagesSettings");
		showSoundsSettings = serializedObject.FindProperty ("showSoundsSettings");
		showOthersSettings = serializedObject.FindProperty ("showOthersSettings");
		showEventSettings = serializedObject.FindProperty ("showEventSettings");
		showSaveLoadSettings = serializedObject.FindProperty ("showSaveLoadSettings");
		showDebugSettings = serializedObject.FindProperty ("showDebugSettings");
		showElementSettings = serializedObject.FindProperty ("showElementSettings");
		checkWeightLimitToPickObjects = serializedObject.FindProperty ("checkWeightLimitToPickObjects");
		mainInventoryWeightManager = serializedObject.FindProperty ("mainInventoryWeightManager");
		storePickedWeaponsOnInventory = serializedObject.FindProperty ("storePickedWeaponsOnInventory");

		equipWeaponsWhenPicked = serializedObject.FindProperty ("equipWeaponsWhenPicked");
		equipPickedWeaponOnlyItNotPreviousWeaponEquipped = serializedObject.FindProperty ("equipPickedWeaponOnlyItNotPreviousWeaponEquipped");
		changeToFireWeaponsModeWhenPickingFireWeapon = serializedObject.FindProperty ("changeToFireWeaponsModeWhenPickingFireWeapon");
		changeToMeleeWeaponsModeWhenPickingMeleeWeapon = serializedObject.FindProperty ("changeToMeleeWeaponsModeWhenPickingMeleeWeapon");

		examineObjectBeforeStoreEnabled = serializedObject.FindProperty ("examineObjectBeforeStoreEnabled");
		distanceToPlaceObjectInCamera = serializedObject.FindProperty ("distanceToPlaceObjectInCamera");
		placeObjectInCameraSpeed = serializedObject.FindProperty ("placeObjectInCameraSpeed");
		numberOfRotationsObjectInCamera = serializedObject.FindProperty ("numberOfRotationsObjectInCamera");
		placeObjectInCameraRotationSpeed = serializedObject.FindProperty ("placeObjectInCameraRotationSpeed");
		extraCameraFovOnExamineObjects = serializedObject.FindProperty ("extraCameraFovOnExamineObjects");
		rotationSpeed = serializedObject.FindProperty ("rotationSpeed");
		zoomSpeed = serializedObject.FindProperty ("zoomSpeed");
		maxZoomValue = serializedObject.FindProperty ("maxZoomValue");
		minZoomValue = serializedObject.FindProperty ("minZoomValue");

		showMessageWhenObjectUsed = serializedObject.FindProperty ("showMessageWhenObjectUsed");
		usedObjectMessage = serializedObject.FindProperty ("usedObjectMessage");
		fullInventoryMessage = serializedObject.FindProperty ("fullInventoryMessage");
		fullInventoryMessageTime = serializedObject.FindProperty ("fullInventoryMessageTime");
		combinedObjectMessage = serializedObject.FindProperty ("combinedObjectMessage");
		combineObjectMessageTime = serializedObject.FindProperty ("combineObjectMessageTime");
		usedObjectMessageTime = serializedObject.FindProperty ("usedObjectMessageTime");
		unableToUseObjectMessage = serializedObject.FindProperty ("unableToUseObjectMessage");
		nonNeededAmountAvaliable = serializedObject.FindProperty ("nonNeededAmountAvaliable");
		objectNotFoundMessage = serializedObject.FindProperty ("objectNotFoundMessage");
		cantUseThisObjectHereMessage = serializedObject.FindProperty ("cantUseThisObjectHereMessage");
		unableToCombineMessage = serializedObject.FindProperty ("unableToCombineMessage");
		canBeCombinedButObjectIsFullMessage = serializedObject.FindProperty ("canBeCombinedButObjectIsFullMessage");

		notEnoughSpaceToCombineMessage = serializedObject.FindProperty ("notEnoughSpaceToCombineMessage");
		weightLimitReachedMessage = serializedObject.FindProperty ("weightLimitReachedMessage");
		objectTooMuchHeavyToCarryMessage = serializedObject.FindProperty ("objectTooMuchHeavyToCarryMessage");
		useAudioSounds = serializedObject.FindProperty ("useAudioSounds");
		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");
		useInventoryOptionsOnSlot = serializedObject.FindProperty ("useInventoryOptionsOnSlot");
		inventoryOptionsOnSlotPanel = serializedObject.FindProperty ("inventoryOptionsOnSlotPanel");
		mainInventorySlotOptionsButtons = serializedObject.FindProperty ("mainInventorySlotOptionsButtons");
		usedByAI = serializedObject.FindProperty ("usedByAI");

		eventOnInventoryInitialized = serializedObject.FindProperty ("eventOnInventoryInitialized");
		eventOnClickInventoryChange = serializedObject.FindProperty ("eventOnClickInventoryChange");
		eventOnInventorySlotSelected = serializedObject.FindProperty ("eventOnInventorySlotSelected");
		eventOnInventorySlotUnSelected = serializedObject.FindProperty ("eventOnInventorySlotUnSelected");
		eventOnInventoryClosed = serializedObject.FindProperty ("eventOnInventoryClosed");
		eventOnInventoryOpened = serializedObject.FindProperty ("eventOnInventoryOpened");
		useEventIfSystemDisabled = serializedObject.FindProperty ("useEventIfSystemDisabled");
		eventIfSystemDisabled = serializedObject.FindProperty ("eventIfSystemDisabled");

		eventOnInventoryListChange = serializedObject.FindProperty ("eventOnInventoryListChange");

		loadCurrentPlayerInventoryFromSaveFile = serializedObject.FindProperty ("loadCurrentPlayerInventoryFromSaveFile");
		saveCurrentPlayerInventoryToSaveFile = serializedObject.FindProperty ("saveCurrentPlayerInventoryToSaveFile");
		inventoryOpened = serializedObject.FindProperty ("inventoryOpened");
		inventoryList = serializedObject.FindProperty ("inventoryList");
		inventoryPanel = serializedObject.FindProperty ("inventoryPanel");
		inventoryListContent = serializedObject.FindProperty ("inventoryListContent");
		inventoryListScrollRect = serializedObject.FindProperty ("inventoryListScrollRect");
		objectIcon = serializedObject.FindProperty ("objectIcon");
		equipButton = serializedObject.FindProperty ("equipButton");
		unequipButton = serializedObject.FindProperty ("unequipButton");
		useButtonImage = serializedObject.FindProperty ("useButtonImage");
		equipButtonImage = serializedObject.FindProperty ("equipButtonImage");
		unequipButtonImage = serializedObject.FindProperty ("unequipButtonImage");
		dropButtonImage = serializedObject.FindProperty ("dropButtonImage");
		combineButtonImage = serializedObject.FindProperty ("combineButtonImage");
		examineButtonImage = serializedObject.FindProperty ("examineButtonImage");
		discardButtonImage = serializedObject.FindProperty ("discardButtonImage");
		dropAllUnitsObjectButtonImage = serializedObject.FindProperty ("dropAllUnitsObjectButtonImage");
		examineObjectPanel = serializedObject.FindProperty ("examineObjectPanel");
		examineObjectName = serializedObject.FindProperty ("examineObjectName");
		examineObjectDescription = serializedObject.FindProperty ("examineObjectDescription");
		takeObjectInExaminePanelButton = serializedObject.FindProperty ("takeObjectInExaminePanelButton");
		currentObjectName = serializedObject.FindProperty ("currentObjectName");
		currentObjectInfo = serializedObject.FindProperty ("currentObjectInfo");
		objectImage = serializedObject.FindProperty ("objectImage");

		usableInventoryObjectInfoList = serializedObject.FindProperty ("usableInventoryObjectInfoList");

		inventoryCamera = serializedObject.FindProperty ("inventoryCamera");
		inventoryLight = serializedObject.FindProperty ("inventoryLight");

		lookObjectsPosition = serializedObject.FindProperty ("lookObjectsPosition");
		emptyInventoryPrefab = serializedObject.FindProperty ("emptyInventoryPrefab");
		numberOfObjectsToUseMenu = serializedObject.FindProperty ("numberOfObjectsToUseMenu");
		numberOfObjectsToUseMenuRectTransform = serializedObject.FindProperty ("numberOfObjectsToUseMenuRectTransform");
		numberOfObjectsToUseText = serializedObject.FindProperty ("numberOfObjectsToUseText");
		numberOfObjectsToUseMenuPosition = serializedObject.FindProperty ("numberOfObjectsToUseMenuPosition");
		numberOfObjectsToDropMenuPosition = serializedObject.FindProperty ("numberOfObjectsToDropMenuPosition");
		inventorySlotsScrollbar = serializedObject.FindProperty ("inventorySlotsScrollbar");
		inventoryObjectInforScrollbar = serializedObject.FindProperty ("inventoryObjectInforScrollbar");

		mainMeleeWeaponsGrabbedManager = serializedObject.FindProperty ("mainMeleeWeaponsGrabbedManager");
		weaponsManager = serializedObject.FindProperty ("weaponsManager");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		pauseManager = serializedObject.FindProperty ("pauseManager");

		playerInput = serializedObject.FindProperty ("playerInput");
		mainInventoryListManager = serializedObject.FindProperty ("mainInventoryListManager");
		usingDevicesManager = serializedObject.FindProperty ("usingDevicesManager");
		mainInventoryMenuPanelsSystem = serializedObject.FindProperty ("mainInventoryMenuPanelsSystem");
		mainInventoryBankUISystem = serializedObject.FindProperty ("mainInventoryBankUISystem");
		mainVendorUISystem = serializedObject.FindProperty ("mainVendorUISystem");
		inventoryListManagerList = serializedObject.FindProperty ("inventoryListManagerList");

		mainInventoryCharacterCustomizationSystem = serializedObject.FindProperty ("mainInventoryCharacterCustomizationSystem");

		mainInventoryQuickAccessSlotsSystem = serializedObject.FindProperty ("mainInventoryQuickAccessSlotsSystem");

		mainInventoryManagerPrefab = serializedObject.FindProperty ("mainInventoryManagerPrefab");

		showDebugPrint = serializedObject.FindProperty ("showDebugPrint");

		destroyObjectsOnEmptyDurability = serializedObject.FindProperty ("destroyObjectsOnEmptyDurability");
		dropObjectsOnEmptyDurability = serializedObject.FindProperty ("dropObjectsOnEmptyDurability");
		setObjectsAsBrokenOnEmptyDurability = serializedObject.FindProperty ("setObjectsAsBrokenOnEmptyDurability");

		unequipObjectOnEmptyDurability = serializedObject.FindProperty ("unequipObjectOnEmptyDurability");

		brokenObjectsCantBeUsed = serializedObject.FindProperty ("brokenObjectsCantBeUsed");
		brokenObjectsCantBeEquipped = serializedObject.FindProperty ("brokenObjectsCantBeEquipped");

		checkDurabilityOnObjectEnabled = serializedObject.FindProperty ("checkDurabilityOnObjectEnabled");

		manager = (inventoryManager)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (inventoryEnabled);
		EditorGUILayout.PropertyField (openInventoryMenuEnabled);
		GUILayout.EndVertical ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Slots Settings", "window");
		EditorGUILayout.PropertyField (infiniteSlots);
		if (!infiniteSlots.boolValue) {
			EditorGUILayout.PropertyField (inventorySlotAmount);
		}
		EditorGUILayout.PropertyField (infiniteAmountPerSlot);
		if (!infiniteAmountPerSlot.boolValue) {
			EditorGUILayout.PropertyField (amountPerSlot);
			EditorGUILayout.PropertyField (combineElementsAtDrop);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Inventory Settings", "window");
		EditorGUILayout.PropertyField (buttonUsable, new GUIContent ("Button Usable Color"), false);
		EditorGUILayout.PropertyField (buttonNotUsable, new GUIContent ("Button Not Usable Color"), false);
		EditorGUILayout.PropertyField (useOnlyWhenNeededAmountToUseObject);
		EditorGUILayout.PropertyField (activeNumberOfObjectsToUseMenu);
		EditorGUILayout.PropertyField (setTotalAmountWhenDropObject);
		EditorGUILayout.PropertyField (configureNumberObjectsToUseRate);
		EditorGUILayout.PropertyField (fasterNumberObjectsToUseRate);
		EditorGUILayout.PropertyField (waitTimeToUseFasterNumberObjectsToUseRate);
	
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useBlurUIPanel);

		EditorGUILayout.PropertyField (showInventoryObjectsName);
		EditorGUILayout.PropertyField (showObjectAmountIfEqualOne);
		EditorGUILayout.PropertyField (selectFirstInventoryObjectWhenOpeningMenu);
		EditorGUILayout.PropertyField (dropSingleObjectOnInfiniteAmount);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects Durability Settings", "window");
		EditorGUILayout.PropertyField (checkDurabilityOnObjectEnabled);
		EditorGUILayout.PropertyField (destroyObjectsOnEmptyDurability);
		EditorGUILayout.PropertyField (dropObjectsOnEmptyDurability);
		EditorGUILayout.PropertyField (setObjectsAsBrokenOnEmptyDurability);

		EditorGUILayout.PropertyField (unequipObjectOnEmptyDurability);
		EditorGUILayout.PropertyField (brokenObjectsCantBeUsed);
		EditorGUILayout.PropertyField (brokenObjectsCantBeEquipped);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.BeginHorizontal ();

		if (showWeightSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Weight")) {
			showWeightSettings.boolValue = !showWeightSettings.boolValue;
		}

		if (showWeaponsSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Weapons")) {
			showWeaponsSettings.boolValue = !showWeaponsSettings.boolValue;
		}

		if (showExamineSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Examine")) {
			showExamineSettings.boolValue = !showExamineSettings.boolValue;
		}
			
		if (showMessagesSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Messages")) {
			showMessagesSettings.boolValue = !showMessagesSettings.boolValue;
		}

		if (showSoundsSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Sounds")) {
			showSoundsSettings.boolValue = !showSoundsSettings.boolValue;
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();

		if (showOthersSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Others")) {
			showOthersSettings.boolValue = !showOthersSettings.boolValue;
		}

		if (showEventSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Events")) {
			showEventSettings.boolValue = !showEventSettings.boolValue;
		}

		if (showSaveLoadSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Save/Load")) {
			showSaveLoadSettings.boolValue = !showSaveLoadSettings.boolValue;
		}

		if (showDebugSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Debug")) {
			showDebugSettings.boolValue = !showDebugSettings.boolValue;
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();

		if (showAllSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide All Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			buttonMessage = "Show All Settings";
		}
		if (GUILayout.Button (buttonMessage)) {
			showAllSettings.boolValue = !showAllSettings.boolValue;
		
			showWeightSettings.boolValue = showAllSettings.boolValue;
			showWeaponsSettings.boolValue = showAllSettings.boolValue;
			showExamineSettings.boolValue = showAllSettings.boolValue;
			showMessagesSettings.boolValue = showAllSettings.boolValue;
			showSoundsSettings.boolValue = showAllSettings.boolValue;
			showOthersSettings.boolValue = showAllSettings.boolValue;
			showEventSettings.boolValue = showAllSettings.boolValue;
			showSaveLoadSettings.boolValue = showAllSettings.boolValue;
			showDebugSettings.boolValue = showAllSettings.boolValue;

			showElementSettings.boolValue = false;
		}

		if (showElementSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Player Components";
		} else {
			GUI.backgroundColor = buttonColor;
			buttonMessage = "Show Player Components";
		}
		if (GUILayout.Button (buttonMessage)) {
			showElementSettings.boolValue = !showElementSettings.boolValue;
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Main Inventory Manager To Scene")) {
			manager.instantiateMainInventoryManagerPrefab ();
		}

		if (GUILayout.Button ("Select Main Inventory Manager On Scene")) {
			manager.selectMainInventoryManagerOnScene ();
		}

		EditorGUILayout.Space ();

		GUI.backgroundColor = buttonColor;

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 30;
		style.alignment = TextAnchor.MiddleCenter;

		if (showAllSettings.boolValue || showWeightSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("WEIGHT SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weight Settings", "window");
			EditorGUILayout.PropertyField (checkWeightLimitToPickObjects);
			EditorGUILayout.PropertyField (mainInventoryWeightManager);
			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showWeaponsSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("WEAPON SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Inventory Weapons Settings", "window");
			EditorGUILayout.PropertyField (storePickedWeaponsOnInventory);
			if (storePickedWeaponsOnInventory.boolValue) {
				EditorGUILayout.PropertyField (equipWeaponsWhenPicked);
				EditorGUILayout.PropertyField (equipPickedWeaponOnlyItNotPreviousWeaponEquipped);
				EditorGUILayout.PropertyField (changeToFireWeaponsModeWhenPickingFireWeapon);
				EditorGUILayout.PropertyField (changeToMeleeWeaponsModeWhenPickingMeleeWeapon);
			}
			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showExamineSettings.boolValue) {
			
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("EXAMINE SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Examine Object Settings", "window");
			EditorGUILayout.PropertyField (examineObjectBeforeStoreEnabled);
			EditorGUILayout.PropertyField (distanceToPlaceObjectInCamera);
			EditorGUILayout.PropertyField (placeObjectInCameraSpeed);
			EditorGUILayout.PropertyField (numberOfRotationsObjectInCamera);
			EditorGUILayout.PropertyField (placeObjectInCameraRotationSpeed);
			EditorGUILayout.PropertyField (extraCameraFovOnExamineObjects);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Inventory Show Object Settings", "window");
			EditorGUILayout.PropertyField (rotationSpeed);
			EditorGUILayout.PropertyField (zoomSpeed);
			EditorGUILayout.PropertyField (maxZoomValue);
			EditorGUILayout.PropertyField (minZoomValue);
			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showMessagesSettings.boolValue) {
			
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("MESSAGES SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Inventory Messages Settings", "window");
			EditorGUILayout.PropertyField (showMessageWhenObjectUsed);
			EditorGUILayout.PropertyField (usedObjectMessage);
			EditorGUILayout.PropertyField (fullInventoryMessage);
			EditorGUILayout.PropertyField (fullInventoryMessageTime);
			EditorGUILayout.PropertyField (combinedObjectMessage);
			EditorGUILayout.PropertyField (combineObjectMessageTime);

			EditorGUILayout.PropertyField (usedObjectMessageTime);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (unableToUseObjectMessage);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (nonNeededAmountAvaliable);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectNotFoundMessage);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (cantUseThisObjectHereMessage);

			EditorGUILayout.Space ();
		
			EditorGUILayout.PropertyField (unableToCombineMessage);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (canBeCombinedButObjectIsFullMessage);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (notEnoughSpaceToCombineMessage);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (weightLimitReachedMessage);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (objectTooMuchHeavyToCarryMessage);

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showSoundsSettings.boolValue) {
			
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("SOUNDS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sound Settings", "window");
			EditorGUILayout.PropertyField (useAudioSounds);
			EditorGUILayout.PropertyField (mainAudioSource);
			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showOthersSettings.boolValue) {
			
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("OTHERS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Inventory Options On Slot Settings", "window");
			EditorGUILayout.PropertyField (useInventoryOptionsOnSlot);
			if (useInventoryOptionsOnSlot.boolValue) {
				EditorGUILayout.PropertyField (inventoryOptionsOnSlotPanel);
				EditorGUILayout.PropertyField (mainInventorySlotOptionsButtons);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("AI Settings", "window");
			EditorGUILayout.PropertyField (usedByAI);
			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showEventSettings.boolValue) {
			
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("EVENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Events Settings", "window");
			EditorGUILayout.PropertyField (eventOnInventoryInitialized);
			EditorGUILayout.PropertyField (eventOnClickInventoryChange);
			EditorGUILayout.PropertyField (eventOnInventorySlotSelected);
			EditorGUILayout.PropertyField (eventOnInventorySlotUnSelected);
			EditorGUILayout.PropertyField (eventOnInventoryClosed);
			EditorGUILayout.PropertyField (eventOnInventoryOpened);

			EditorGUILayout.PropertyField (eventOnInventoryListChange);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useEventIfSystemDisabled);
			if (useEventIfSystemDisabled.boolValue) {
				EditorGUILayout.PropertyField (eventIfSystemDisabled);
			}
			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showSaveLoadSettings.boolValue) {
			
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("SAVE/LOAD SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Save/Load Inventory Settings", "window");
			EditorGUILayout.PropertyField (loadCurrentPlayerInventoryFromSaveFile);
			EditorGUILayout.PropertyField (saveCurrentPlayerInventoryToSaveFile);
			EditorGUILayout.PropertyField (setInfiniteSlotValuesOnSaveLoad);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Save Inventory List")) {
				manager.saveCurrentInventoryListToFileFromEditor ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showDebugSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("DEBUG SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Inventory Menu State", "window");
			EditorGUILayout.PropertyField (showDebugPrint);

			menuOpened = "NO";
			if (Application.isPlaying) {
				if (inventoryOpened.boolValue) {
					menuOpened = "YES";
				} 
			} 
			GUILayout.Label ("Inventory Menu Opened \t " + menuOpened);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("(DEBUG ONLY)", "window");
			GUILayout.BeginVertical ("Inventory List", "window");
			showInventoryList (inventoryList);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Drop All Inventory")) {
				manager.dropAllInventory ();
			}
			GUILayout.EndVertical ();
		}

		if (showElementSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("ELEMENT SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Set here every element used in the inventory", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("UI ELEMENTS", EditorStyles.boldLabel);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (inventoryPanel);
			EditorGUILayout.PropertyField (inventoryListContent);
			EditorGUILayout.PropertyField (inventoryListScrollRect);
			EditorGUILayout.PropertyField (objectIcon);

			EditorGUILayout.PropertyField (equipButton);
			EditorGUILayout.PropertyField (unequipButton);

			EditorGUILayout.PropertyField (useButtonImage);
			EditorGUILayout.PropertyField (equipButtonImage);
			EditorGUILayout.PropertyField (unequipButtonImage);
			EditorGUILayout.PropertyField (dropButtonImage);
			EditorGUILayout.PropertyField (combineButtonImage);
			EditorGUILayout.PropertyField (examineButtonImage);	
			EditorGUILayout.PropertyField (discardButtonImage);
			EditorGUILayout.PropertyField (dropAllUnitsObjectButtonImage);

			EditorGUILayout.PropertyField (examineObjectPanel);
			EditorGUILayout.PropertyField (examineObjectName);
			EditorGUILayout.PropertyField (examineObjectDescription);
			EditorGUILayout.PropertyField (takeObjectInExaminePanelButton);

			EditorGUILayout.PropertyField (currentObjectName);
			EditorGUILayout.PropertyField (currentObjectInfo);
			EditorGUILayout.PropertyField (objectImage);
			EditorGUILayout.PropertyField (inventoryCamera);
			EditorGUILayout.PropertyField (inventoryLight);
			EditorGUILayout.PropertyField (lookObjectsPosition);
			EditorGUILayout.PropertyField (emptyInventoryPrefab);
			EditorGUILayout.PropertyField (numberOfObjectsToUseMenu);
			EditorGUILayout.PropertyField (numberOfObjectsToUseMenuRectTransform);
			EditorGUILayout.PropertyField (numberOfObjectsToUseText);
			EditorGUILayout.PropertyField (numberOfObjectsToUseMenuPosition);
			EditorGUILayout.PropertyField (numberOfObjectsToDropMenuPosition);
			EditorGUILayout.PropertyField (inventorySlotsScrollbar);
			EditorGUILayout.PropertyField (inventoryObjectInforScrollbar);	

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("COMPONENTS", EditorStyles.boldLabel);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (weaponsManager);
			EditorGUILayout.PropertyField (mainMeleeWeaponsGrabbedManager);
			EditorGUILayout.PropertyField (playerControllerManager);
			EditorGUILayout.PropertyField (pauseManager);
		
			EditorGUILayout.PropertyField (playerInput);
			EditorGUILayout.PropertyField (mainInventoryListManager);
			EditorGUILayout.PropertyField (usingDevicesManager);
			EditorGUILayout.PropertyField (mainInventoryMenuPanelsSystem);
			EditorGUILayout.PropertyField (mainInventoryBankUISystem);
			EditorGUILayout.PropertyField (mainVendorUISystem);
			EditorGUILayout.PropertyField (mainInventoryQuickAccessSlotsSystem);

			EditorGUILayout.PropertyField (mainInventoryManagerPrefab);	

			EditorGUILayout.PropertyField (mainInventoryCharacterCustomizationSystem);	

			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Initial Inventory List", "window");
		showInventoryListManagerList (inventoryListManagerList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Usable Inventory Objects Externally List", "window");
		showUsableInventoryObjectInfoList (usableInventoryObjectInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showInventoryListElementInfo (SerializedProperty list, bool expanded, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectInfo"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("inventoryGameObject"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isEquiped"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Icon Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("icon"));

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Object Icon Preview \t");
		GUILayout.BeginHorizontal ("box", GUILayout.Height (50), GUILayout.Width (50));
		if (list.FindPropertyRelative ("icon").objectReferenceValue && expanded) {
			Object texture = list.FindPropertyRelative ("icon").objectReferenceValue as Texture2D;
			Texture2D myTexture = AssetPreview.GetAssetPreview (texture);
			GUILayout.Label (myTexture, GUILayout.Width (50), GUILayout.Height (50));
		}
		GUILayout.EndHorizontal ();
		GUILayout.Label ("");
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Amount Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amount"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountPerUnit"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmount"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("storeTotalAmountPerUnit"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showAmountPerUnitInAmountText"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use Object Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsed"));
		if (list.FindPropertyRelative ("canBeUsed").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewBehaviorOnUse"));
			if (list.FindPropertyRelative ("useNewBehaviorOnUse").boolValue) {

				GUILayout.Label ("Tip: Use -AMOUNT- in the position of the text \n to replace the amount used if you need it");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("newBehaviorOnUseMessage"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Equip Object Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeEquiped"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Drop Object Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeDropped"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeDiscarded"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Combine Object Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeCombined"));
		if (list.FindPropertyRelative ("canBeCombined").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewBehaviorOnCombine"));
			if (list.FindPropertyRelative ("useNewBehaviorOnCombine").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useOneUnitOnNewBehaviourCombine"));

				EditorGUILayout.Space ();

				GUILayout.Label ("Tip: Use -OBJECT- in the position of the text \n to replace the second combined object name used if you need it");
				GUILayout.Label ("Also, use -AMOUNT- in the position of the text \n to replace the amount used if you need it");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("newBehaviorOnCombineMessage"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToCombine"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("combinedObject"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("combinedObjectMessage"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vendor Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeSold"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("vendorPrice"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sellPrice"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMinLevelToBuy"));
		if (list.FindPropertyRelative ("useMinLevelToBuy").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("minLevelToBuy"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Durability Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDurability"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("durabilityAmount"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDurabilityAmount"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sellMultiplierIfObjectIsBroken"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Max Amount Per Slot Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setMaximumAmountPerSlot"));
		if (list.FindPropertyRelative ("setMaximumAmountPerSlot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("maximumAmountPerSlot"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isWeapon"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isMeleeWeapon"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isArmorClothAccessory"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("categoryName"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weight"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cantBeStoredOnInventory"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeHeld"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBePlaceOnQuickAccessSlot"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeExamined"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hideSlotOnMenu"));

		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("button"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("menuIconElement"));
		GUILayout.EndVertical ();
	}

	void showInventoryList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Current Inventory List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize);

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

					amountValue = " - " + list.GetArrayElementAtIndex (i).FindPropertyRelative ("amount").intValue;
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + amountValue), false);

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showInventoryListElementInfo (list.GetArrayElementAtIndex (i), expanded, i);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showInventoryListManagerListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amount"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isEquipped"));

		if (manager.inventoryManagerListString.Length > 0) {
			list.FindPropertyRelative ("categoryIndex").intValue = EditorGUILayout.Popup ("Category", list.FindPropertyRelative ("categoryIndex").intValue, manager.inventoryManagerListString);

			categoryIndex = list.FindPropertyRelative ("categoryIndex").intValue;
			if (categoryIndex < manager.inventoryManagerListString.Length) {
				list.FindPropertyRelative ("inventoryCategoryName").stringValue = manager.inventoryManagerListString [categoryIndex];
			}

			if (manager.inventoryManagerStringInfoList.Count > 0) {
				list.FindPropertyRelative ("elementIndex").intValue = EditorGUILayout.Popup ("Inventory Object", list.FindPropertyRelative ("elementIndex").intValue, 
					manager.inventoryManagerStringInfoList [categoryIndex].inventoryManagerListString);

				elementIndex = list.FindPropertyRelative ("elementIndex").intValue;
				if (elementIndex < manager.inventoryManagerStringInfoList [categoryIndex].inventoryManagerListString.Length) {
					list.FindPropertyRelative ("inventoryObjectName").stringValue = manager.inventoryManagerStringInfoList [categoryIndex].inventoryManagerListString [elementIndex];
				}
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("addInventoryObject"));
		}
		GUILayout.EndVertical ();
	}

	void showInventoryListManagerList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
	
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Initial Inventory List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure the initial list of objects the player's inventory will contain", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize);

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				manager.addNewInventoryObjectToInventoryListManagerList ();
			}
			if (GUILayout.Button ("Clear List")) {
				manager.clearInitialInventoryListOnEditor ();
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

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable Add All")) {
				for (int i = 0; i < list.arraySize; i++) {
					manager.setAddAllObjectEnabledState (true);
				}
			}
			if (GUILayout.Button ("Disable Add All")) {
				for (int i = 0; i < list.arraySize; i++) {
					manager.setAddAllObjectEnabledState (false);
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

					addInventoryObject = list.GetArrayElementAtIndex (i).FindPropertyRelative ("addInventoryObject").boolValue;
					isAdded = " + ";
					if (!addInventoryObject) {
						isAdded = " - ";
					}

					isEquipped = list.GetArrayElementAtIndex (i).FindPropertyRelative ("isEquipped").boolValue;
					objectIsEquipped = "";
					if (isEquipped) {
						objectIsEquipped = " E ";
					}

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + " (" + isAdded + objectIsEquipped + ")"), false);

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showInventoryListManagerListElement (list.GetArrayElementAtIndex (i));
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
				if (GUILayout.Button ("O")) {
					manager.setAddObjectEnabledState (i);
				}
				if (GUILayout.Button ("E")) {
					manager.setEquippedObjectState (i);
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Add Inventory Object")) {
				manager.addNewInventoryObjectToInventoryListManagerList ();
			}
				
			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Inventory Object List Names")) {
				manager.setInventoryObjectListNames ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Inventory List Available")) {
				manager.getInventoryListManagerList ();
			}

			EditorGUILayout.Space ();

		}
		GUILayout.EndVertical ();
	}


	void showUsableInventoryObjectInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Usable Inventory Objects Externally List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure a list of objects which can be used by events, sending its string name", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize);

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.ClearArray ();
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
						showUsableInventoryObjectInfoListElement (list.GetArrayElementAtIndex (i));
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
			}

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();
	}

	void showUsableInventoryObjectInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountToUse"));
		GUILayout.EndVertical ();
	}
}
#endif