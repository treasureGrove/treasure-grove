using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(grabbedObjectMeleeAttackSystem))]
public class grabbedObjectMeleeAttackSystemEditor : Editor
{
	SerializedProperty grabbedObjectMeleeAttackEnabled;

	SerializedProperty canGrabMeleeObjectsEnabled;

	SerializedProperty useAttackTypes;

	SerializedProperty generalAttackDamageMultiplier;

	SerializedProperty meleeAttackTypes;

	SerializedProperty playerOnGroundToActivateAttack;

	SerializedProperty disableGrabObjectsInputWhenCarryingMeleeWeapon;

	SerializedProperty useThrowReturnMeleeWeaponSystemEnabled;

	SerializedProperty useCustomLayerToDetectSurfaces;
	SerializedProperty customLayerToDetectSurfaces;

	SerializedProperty useCustomIgnoreTags;
	SerializedProperty customTagsToIgnoreList;

	SerializedProperty checkSurfacesWithCapsuleRaycastEnabled;

	SerializedProperty ignoreGetHealthFromDamagingObjects;

	SerializedProperty ignoreGetHealthFromDamagingObjectsOnWeaponInfo;

	SerializedProperty ignoreCutModeOnMeleeWeaponAttack;

	SerializedProperty grabbedWeaponInfoList;

	SerializedProperty checkSurfaceInfoEnabled;

	SerializedProperty surfaceInfoOnMeleeAttackNameForNotFound;
	SerializedProperty surfaceInfoOnMeleeAttackNameForSwingOnAir;
	SerializedProperty surfaceInfoOnMeleeAttackList;

	SerializedProperty useStaminaOnAttackEnabled;
	SerializedProperty attackStaminaStateName;

	SerializedProperty generalStaminaUseMultiplier;

	SerializedProperty cuttingModeEnabled;

	SerializedProperty eventOnCuttingModeStart;
	SerializedProperty eventOnCuttingModeEnd;

	SerializedProperty blockModeEnabled;
	SerializedProperty generalBlockProtectionMultiplier;

	SerializedProperty cancelBlockReactionStateName;

	SerializedProperty shieldCanBeUsedWithoutMeleeWeapon;

	SerializedProperty rightHandMountPoint;
	SerializedProperty leftHandMountPoint;
	SerializedProperty shieldRightHandMountPoint;
	SerializedProperty shieldLeftHandMountPoint;

	SerializedProperty shieldBackMountPoint;

	SerializedProperty eventToActivateMeleeModeWhenUsingShieldWithoutMeleeWeapon;

	SerializedProperty useMatchTargetSystemOnAttack;
	SerializedProperty ignoreAttackSettingToMatchTarget;
	SerializedProperty mainMatchPlayerToTargetSystem;

	SerializedProperty checkDurabilityOnAttackEnabled;
	SerializedProperty checkDurabilityOnBlockEnabled;

	SerializedProperty generalAttackDurabilityMultiplier;
	SerializedProperty generalBlockDurabilityMultiplier;

	SerializedProperty useEventOnDurabilityEmptyOnMeleeWeapon;
	SerializedProperty eventOnDurabilityEmptyOnMeleeWeapon;

	SerializedProperty useEventOnDurabilityEmptyOnBlock;
	SerializedProperty eventOnDurabilityEmptyOnBlock;

	SerializedProperty mainMeleeCombatAxesInputName;
	SerializedProperty mainMeleeCombatBlockInputName;

	SerializedProperty ignoreUseDrawKeepWeaponAnimation;

	SerializedProperty showExtraActionIconTexture;
	SerializedProperty extraActionIconGameObject;
	SerializedProperty extraActionRawImage;

	SerializedProperty useEventsOnAttack;
	SerializedProperty eventOnAttackStart;
	SerializedProperty eventOnAttackEnd;

	SerializedProperty useEventsOnBlockDamage;
	SerializedProperty eventOnBlockActivate;
	SerializedProperty eventOnBlockDeactivate;

	SerializedProperty useEventsOnGrabDropObject;
	SerializedProperty eventOnGrabObject;
	SerializedProperty eventOnDropObject;

	SerializedProperty useEventsOnAttackCantBeBlocked;
	SerializedProperty eventOnAttackCantBeBlocked;

	SerializedProperty hideInventoryQuickAccessSlotsWhenCarryingMeleeWeapon;
	SerializedProperty eventOnHideInventoryQuickAccessSlots;
	SerializedProperty eventOnShowInventoryQuickAccessSlots;

	SerializedProperty currentGrabbedObjectTransform;
	SerializedProperty mainRemoteEventSystem;

	SerializedProperty playerControllerGameObject;
	SerializedProperty mainPlayerController;

	SerializedProperty mainGrabObjects;
	SerializedProperty mainHealth;
	SerializedProperty mainCameraTransform;

	SerializedProperty mainStaminaSystem;

	SerializedProperty handPositionReference;

	SerializedProperty mainAudioSource;

	SerializedProperty mainSliceSystem;

	SerializedProperty mainAnimator;

	SerializedProperty mainFindObjectivesSystem;

	SerializedProperty mainMeleeWeaponsGrabbedManager;

	SerializedProperty playerInput;

	SerializedProperty parentToPlaceTriggerInFrontOfCharacter;

	SerializedProperty mainMeleeCombatThrowReturnWeaponSystem;


	SerializedProperty showDebugPrint;

	SerializedProperty showDebugDraw;
	
	SerializedProperty grabbedObjectMeleeAttackActive;
	
	SerializedProperty carryingObject;
	
	SerializedProperty attackInProcess;
	
	SerializedProperty meleeAttackTypesAmount;
	
	SerializedProperty currentHitCombat;
	
	SerializedProperty dualWieldHitCombat;
	
	SerializedProperty currentAttackIndex;
	
	SerializedProperty blockActive;
	
	SerializedProperty blockActivePreviously;
	
	SerializedProperty objectThrown;

	SerializedProperty cuttingModeActive;
	
	SerializedProperty attackInputPausedForStamina;
	
	SerializedProperty damageTriggerInProcess;
	
	SerializedProperty meleeAttackInputPaused;
	
	SerializedProperty reducedBlockDamageProtectionActive;
	
	SerializedProperty canCancelBlockToStartAttackActive;
	
	SerializedProperty blockInputPaused;
	
	SerializedProperty isDualWieldWeapon;
	SerializedProperty mainDualWieldMeleeWeaponObjectSystem;
	
	SerializedProperty throwWeaponActionPaused;

	SerializedProperty shieldActive;
	
	SerializedProperty carryingShield;
	
	SerializedProperty currentShieldName;
	
	SerializedProperty currentShieldGameObject;
	
	SerializedProperty currentShieldHandMountPointTransformReference;
	SerializedProperty currentShieldBackMountPointTransformReference;

	SerializedProperty weaponsCanBeStolenFromCharacter;

	GUIStyle sectionStyle = new GUIStyle ();

	string currentLabel;

	Color buttonColor;

	string buttonText;

	GUIStyle buttonStyle = new GUIStyle ();

	string[] mainTabsOptions = {
		"Damage Detection Settings",
		"Weapon Info List Settings",
		"Surface Info List Settings",
		"Shield Block Mode Settings",
		"Weapon Durability Settings",
		"Weapon Extra Action Settings",
		"Event Settings",
		"Other Settings",
		"Debug State",
		"Components",
		"Show All",
		"Hide All"
	};

	string currentButtonString;

	int mainTabIndex = -1;


	void OnEnable ()
	{
		grabbedObjectMeleeAttackEnabled = serializedObject.FindProperty ("grabbedObjectMeleeAttackEnabled");

		canGrabMeleeObjectsEnabled = serializedObject.FindProperty ("canGrabMeleeObjectsEnabled");

		useAttackTypes = serializedObject.FindProperty ("useAttackTypes");

		generalAttackDamageMultiplier = serializedObject.FindProperty ("generalAttackDamageMultiplier");

		meleeAttackTypes = serializedObject.FindProperty ("meleeAttackTypes");

		playerOnGroundToActivateAttack = serializedObject.FindProperty ("playerOnGroundToActivateAttack");

		disableGrabObjectsInputWhenCarryingMeleeWeapon = serializedObject.FindProperty ("disableGrabObjectsInputWhenCarryingMeleeWeapon");

		useThrowReturnMeleeWeaponSystemEnabled = serializedObject.FindProperty ("useThrowReturnMeleeWeaponSystemEnabled");

		useCustomLayerToDetectSurfaces = serializedObject.FindProperty ("useCustomLayerToDetectSurfaces");
		customLayerToDetectSurfaces = serializedObject.FindProperty ("customLayerToDetectSurfaces");

		useCustomIgnoreTags = serializedObject.FindProperty ("useCustomIgnoreTags");
		customTagsToIgnoreList = serializedObject.FindProperty ("customTagsToIgnoreList");

		checkSurfacesWithCapsuleRaycastEnabled = serializedObject.FindProperty ("checkSurfacesWithCapsuleRaycastEnabled");

		ignoreGetHealthFromDamagingObjects = serializedObject.FindProperty ("ignoreGetHealthFromDamagingObjects");

		ignoreGetHealthFromDamagingObjectsOnWeaponInfo = serializedObject.FindProperty ("ignoreGetHealthFromDamagingObjectsOnWeaponInfo");

		ignoreCutModeOnMeleeWeaponAttack = serializedObject.FindProperty ("ignoreCutModeOnMeleeWeaponAttack");

		grabbedWeaponInfoList = serializedObject.FindProperty ("grabbedWeaponInfoList");

		checkSurfaceInfoEnabled = serializedObject.FindProperty ("checkSurfaceInfoEnabled");

		surfaceInfoOnMeleeAttackNameForNotFound = serializedObject.FindProperty ("surfaceInfoOnMeleeAttackNameForNotFound");
		surfaceInfoOnMeleeAttackNameForSwingOnAir = serializedObject.FindProperty ("surfaceInfoOnMeleeAttackNameForSwingOnAir");
		surfaceInfoOnMeleeAttackList = serializedObject.FindProperty ("surfaceInfoOnMeleeAttackList");

		useStaminaOnAttackEnabled = serializedObject.FindProperty ("useStaminaOnAttackEnabled");
		attackStaminaStateName = serializedObject.FindProperty ("attackStaminaStateName");

		generalStaminaUseMultiplier = serializedObject.FindProperty ("generalStaminaUseMultiplier");

		cuttingModeEnabled = serializedObject.FindProperty ("cuttingModeEnabled");

		eventOnCuttingModeStart = serializedObject.FindProperty ("eventOnCuttingModeStart");
		eventOnCuttingModeEnd = serializedObject.FindProperty ("eventOnCuttingModeEnd");

		blockModeEnabled = serializedObject.FindProperty ("blockModeEnabled");
		generalBlockProtectionMultiplier = serializedObject.FindProperty ("generalBlockProtectionMultiplier");

		cancelBlockReactionStateName = serializedObject.FindProperty ("cancelBlockReactionStateName");

		shieldCanBeUsedWithoutMeleeWeapon = serializedObject.FindProperty ("shieldCanBeUsedWithoutMeleeWeapon");

		rightHandMountPoint = serializedObject.FindProperty ("rightHandMountPoint");
		leftHandMountPoint = serializedObject.FindProperty ("leftHandMountPoint");
		shieldRightHandMountPoint = serializedObject.FindProperty ("shieldRightHandMountPoint");
		shieldLeftHandMountPoint = serializedObject.FindProperty ("shieldLeftHandMountPoint");

		shieldBackMountPoint = serializedObject.FindProperty ("shieldBackMountPoint");

		eventToActivateMeleeModeWhenUsingShieldWithoutMeleeWeapon = serializedObject.FindProperty ("eventToActivateMeleeModeWhenUsingShieldWithoutMeleeWeapon");
	
		useMatchTargetSystemOnAttack = serializedObject.FindProperty ("useMatchTargetSystemOnAttack");
		ignoreAttackSettingToMatchTarget = serializedObject.FindProperty ("ignoreAttackSettingToMatchTarget");
		mainMatchPlayerToTargetSystem = serializedObject.FindProperty ("mainMatchPlayerToTargetSystem");

		checkDurabilityOnAttackEnabled = serializedObject.FindProperty ("checkDurabilityOnAttackEnabled");
		checkDurabilityOnBlockEnabled = serializedObject.FindProperty ("checkDurabilityOnBlockEnabled");

		generalAttackDurabilityMultiplier = serializedObject.FindProperty ("generalAttackDurabilityMultiplier");
		generalBlockDurabilityMultiplier = serializedObject.FindProperty ("generalBlockDurabilityMultiplier");

		useEventOnDurabilityEmptyOnMeleeWeapon = serializedObject.FindProperty ("useEventOnDurabilityEmptyOnMeleeWeapon");
		eventOnDurabilityEmptyOnMeleeWeapon = serializedObject.FindProperty ("eventOnDurabilityEmptyOnMeleeWeapon");

		useEventOnDurabilityEmptyOnBlock = serializedObject.FindProperty ("useEventOnDurabilityEmptyOnBlock");
		eventOnDurabilityEmptyOnBlock = serializedObject.FindProperty ("eventOnDurabilityEmptyOnBlock");

		mainMeleeCombatAxesInputName = serializedObject.FindProperty ("mainMeleeCombatAxesInputName");
		mainMeleeCombatBlockInputName = serializedObject.FindProperty ("mainMeleeCombatBlockInputName");

		ignoreUseDrawKeepWeaponAnimation = serializedObject.FindProperty ("ignoreUseDrawKeepWeaponAnimation");

		showExtraActionIconTexture = serializedObject.FindProperty ("showExtraActionIconTexture");
		extraActionIconGameObject = serializedObject.FindProperty ("extraActionIconGameObject");
		extraActionRawImage = serializedObject.FindProperty ("extraActionRawImage");

		useEventsOnAttack = serializedObject.FindProperty ("useEventsOnAttack");
		eventOnAttackStart = serializedObject.FindProperty ("eventOnAttackStart");
		eventOnAttackEnd = serializedObject.FindProperty ("eventOnAttackEnd");

		useEventsOnBlockDamage = serializedObject.FindProperty ("useEventsOnBlockDamage");
		eventOnBlockActivate = serializedObject.FindProperty ("eventOnBlockActivate");
		eventOnBlockDeactivate = serializedObject.FindProperty ("eventOnBlockDeactivate");

		useEventsOnGrabDropObject = serializedObject.FindProperty ("useEventsOnGrabDropObject");
		eventOnGrabObject = serializedObject.FindProperty ("eventOnGrabObject");
		eventOnDropObject = serializedObject.FindProperty ("eventOnDropObject");

		useEventsOnAttackCantBeBlocked = serializedObject.FindProperty ("useEventsOnAttackCantBeBlocked");
		eventOnAttackCantBeBlocked = serializedObject.FindProperty ("eventOnAttackCantBeBlocked");

		hideInventoryQuickAccessSlotsWhenCarryingMeleeWeapon = serializedObject.FindProperty ("hideInventoryQuickAccessSlotsWhenCarryingMeleeWeapon");
		eventOnHideInventoryQuickAccessSlots = serializedObject.FindProperty ("eventOnHideInventoryQuickAccessSlots");
		eventOnShowInventoryQuickAccessSlots = serializedObject.FindProperty ("eventOnShowInventoryQuickAccessSlots");

		currentGrabbedObjectTransform = serializedObject.FindProperty ("currentGrabbedObjectTransform");
		mainRemoteEventSystem = serializedObject.FindProperty ("mainRemoteEventSystem");

		playerControllerGameObject = serializedObject.FindProperty ("playerControllerGameObject");
		mainPlayerController = serializedObject.FindProperty ("mainPlayerController");

		mainGrabObjects = serializedObject.FindProperty ("mainGrabObjects");
		mainHealth = serializedObject.FindProperty ("mainHealth");
		mainCameraTransform = serializedObject.FindProperty ("mainCameraTransform");

		mainStaminaSystem = serializedObject.FindProperty ("mainStaminaSystem");

		handPositionReference = serializedObject.FindProperty ("handPositionReference");

		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");

		mainSliceSystem = serializedObject.FindProperty ("mainSliceSystem");

		mainAnimator = serializedObject.FindProperty ("mainAnimator");

		mainFindObjectivesSystem = serializedObject.FindProperty ("mainFindObjectivesSystem");

		mainMeleeWeaponsGrabbedManager = serializedObject.FindProperty ("mainMeleeWeaponsGrabbedManager");

		playerInput = serializedObject.FindProperty ("playerInput");

		parentToPlaceTriggerInFrontOfCharacter = serializedObject.FindProperty ("parentToPlaceTriggerInFrontOfCharacter");

		mainMeleeCombatThrowReturnWeaponSystem = serializedObject.FindProperty ("mainMeleeCombatThrowReturnWeaponSystem");


		showDebugPrint = serializedObject.FindProperty ("showDebugPrint");

		showDebugDraw = serializedObject.FindProperty ("showDebugDraw");

		grabbedObjectMeleeAttackActive = serializedObject.FindProperty ("grabbedObjectMeleeAttackActive");

		carryingObject = serializedObject.FindProperty ("carryingObject");

		attackInProcess = serializedObject.FindProperty ("attackInProcess");

		meleeAttackTypesAmount = serializedObject.FindProperty ("meleeAttackTypesAmount");

		currentHitCombat = serializedObject.FindProperty ("currentHitCombat");

		dualWieldHitCombat = serializedObject.FindProperty ("dualWieldHitCombat");

		currentAttackIndex = serializedObject.FindProperty ("currentAttackIndex");

		blockActive = serializedObject.FindProperty ("blockActive");

		blockActivePreviously = serializedObject.FindProperty ("blockActivePreviously");

		objectThrown = serializedObject.FindProperty ("objectThrown");

		cuttingModeActive = serializedObject.FindProperty ("cuttingModeActive");

		attackInputPausedForStamina = serializedObject.FindProperty ("attackInputPausedForStamina");

		damageTriggerInProcess = serializedObject.FindProperty ("damageTriggerInProcess");

		meleeAttackInputPaused = serializedObject.FindProperty ("meleeAttackInputPaused");

		reducedBlockDamageProtectionActive = serializedObject.FindProperty ("reducedBlockDamageProtectionActive");

		canCancelBlockToStartAttackActive = serializedObject.FindProperty ("canCancelBlockToStartAttackActive");

		blockInputPaused = serializedObject.FindProperty ("blockInputPaused");

		isDualWieldWeapon = serializedObject.FindProperty ("isDualWieldWeapon");
		mainDualWieldMeleeWeaponObjectSystem = serializedObject.FindProperty ("mainDualWieldMeleeWeaponObjectSystem");

		throwWeaponActionPaused = serializedObject.FindProperty ("throwWeaponActionPaused");

		shieldActive = serializedObject.FindProperty ("shieldActive");

		carryingShield = serializedObject.FindProperty ("carryingShield");

		currentShieldName = serializedObject.FindProperty ("currentShieldName");

		currentShieldGameObject = serializedObject.FindProperty ("currentShieldGameObject");

		currentShieldHandMountPointTransformReference = serializedObject.FindProperty ("currentShieldHandMountPointTransformReference");
		currentShieldBackMountPointTransformReference = serializedObject.FindProperty ("currentShieldBackMountPointTransformReference");
	
		weaponsCanBeStolenFromCharacter= serializedObject.FindProperty ("weaponsCanBeStolenFromCharacter");
	}

	public override void OnInspectorGUI ()
	{	
		EditorGUI.BeginChangeCheck ();

		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (grabbedObjectMeleeAttackEnabled);
		EditorGUILayout.PropertyField (canGrabMeleeObjectsEnabled);

		EditorGUILayout.PropertyField (generalAttackDamageMultiplier);
		EditorGUILayout.PropertyField (playerOnGroundToActivateAttack);
		EditorGUILayout.PropertyField (disableGrabObjectsInputWhenCarryingMeleeWeapon);
		EditorGUILayout.PropertyField (useThrowReturnMeleeWeaponSystemEnabled);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useAttackTypes);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Melee Attack Types Settings", "window");
		showSimpleList (meleeAttackTypes, "Melee Attack Types", true);
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		mainTabIndex = GUILayout.SelectionGrid (mainTabIndex, mainTabsOptions, 3);

		if (mainTabIndex >= 0 && mainTabIndex < mainTabsOptions.Length) {
			switch (mainTabsOptions [mainTabIndex]) {
			case "Damage Detection Settings":
				showDamageDetectionSettings ();

				break;

			case "Weapon Info List Settings":
				showWeaponInfoListSettings ();

				break;

			case "Surface Info List Settings":
				showSurfaceInfoListSettings ();

				break;
			
			case "Shield Block Mode Settings":
				showShieldAndBlockSettings ();

				break;
		
			case "Weapon Durability Settings":
				showDurabilitySettings ();

				break;

			case "Weapon Extra Action Settings":
				showWeaponExtraActionSettings ();

				break;
			
			case "Other Settings":
				showOtherSettings ();

				break;

			case "Event Settings":
				showEventSettings ();

				break;

			case "Components":
				showComponents ();

				break;
	
			case "Debug State":
				showDebugSettings ();

				break;

			case "Show All":
				showDamageDetectionSettings ();

				showWeaponInfoListSettings ();

				showSurfaceInfoListSettings ();

				showShieldAndBlockSettings ();

				showDurabilitySettings ();

				showWeaponExtraActionSettings ();

				showOtherSettings ();

				showEventSettings ();

				showComponents ();

				showDebugSettings ();

				break;

			case "Hide All":
				mainTabIndex = -1;

				break;

			default: 

				break;
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showSectionTitle (string sectionTitle)
	{
		sectionStyle.fontStyle = FontStyle.Bold;
		sectionStyle.fontSize = 30;
		sectionStyle.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.LabelField (sectionTitle, sectionStyle);

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
	}

	void showWeaponInfoListSettings ()
	{
		showSectionTitle ("WEAPON INFO LIST SETTINGS");

		GUILayout.BeginVertical ("Weapon Info List Settings", "window");
		showGrabbedWeaponInfoList (grabbedWeaponInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showSurfaceInfoListSettings ()
	{
		showSectionTitle ("SURFACE INFO LIST SETTINGS");

		GUILayout.BeginVertical ("Surface Info List Settings", "window");
		EditorGUILayout.PropertyField (checkSurfaceInfoEnabled);
		EditorGUILayout.PropertyField (surfaceInfoOnMeleeAttackNameForNotFound);
		EditorGUILayout.PropertyField (surfaceInfoOnMeleeAttackNameForSwingOnAir);
		showSurfaceInfoOnMeleeAttackList (surfaceInfoOnMeleeAttackList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showDamageDetectionSettings ()
	{
		showSectionTitle ("DAMAGE DETECTION SETTINGS");

		GUILayout.BeginVertical ("Damage Detection Settings", "window");
		EditorGUILayout.PropertyField (useCustomLayerToDetectSurfaces);
		EditorGUILayout.PropertyField (customLayerToDetectSurfaces);
		EditorGUILayout.PropertyField (useCustomIgnoreTags);
		showSimpleList (customTagsToIgnoreList, "Custom Tags To Ignore List", true);
		EditorGUILayout.PropertyField (checkSurfacesWithCapsuleRaycastEnabled);
		EditorGUILayout.PropertyField (ignoreGetHealthFromDamagingObjects);
		EditorGUILayout.PropertyField (ignoreGetHealthFromDamagingObjectsOnWeaponInfo);

		EditorGUILayout.PropertyField (ignoreCutModeOnMeleeWeaponAttack);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showShieldAndBlockSettings ()
	{
		showSectionTitle ("SHIED/BLOCK SETTINGS");

		GUILayout.BeginVertical ("Block Mode Settings", "window");
		EditorGUILayout.PropertyField (blockModeEnabled);
		EditorGUILayout.PropertyField (generalBlockProtectionMultiplier);
		EditorGUILayout.PropertyField (cancelBlockReactionStateName);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Shield Settings", "window");
		EditorGUILayout.PropertyField (shieldCanBeUsedWithoutMeleeWeapon);
		EditorGUILayout.PropertyField (rightHandMountPoint);
		EditorGUILayout.PropertyField (leftHandMountPoint);
		EditorGUILayout.PropertyField (shieldRightHandMountPoint);
		EditorGUILayout.PropertyField (shieldLeftHandMountPoint);
		EditorGUILayout.PropertyField (shieldBackMountPoint);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (eventToActivateMeleeModeWhenUsingShieldWithoutMeleeWeapon);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

	}

	void showOtherSettings ()
	{
		showSectionTitle ("OTHER SETTINGS");

		GUILayout.BeginVertical ("Stamina Settings", "window");
		EditorGUILayout.PropertyField (useStaminaOnAttackEnabled);
		EditorGUILayout.PropertyField (attackStaminaStateName);
		EditorGUILayout.PropertyField (generalStaminaUseMultiplier);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Cutting Mode Settings", "window");
		EditorGUILayout.PropertyField (cuttingModeEnabled);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (eventOnCuttingModeStart);
		EditorGUILayout.PropertyField (eventOnCuttingModeEnd);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Match Target Settings", "window");
		EditorGUILayout.PropertyField (useMatchTargetSystemOnAttack);
		EditorGUILayout.PropertyField (ignoreAttackSettingToMatchTarget);
		EditorGUILayout.PropertyField (mainMatchPlayerToTargetSystem);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window");
		EditorGUILayout.PropertyField (mainMeleeCombatAxesInputName);
		EditorGUILayout.PropertyField (mainMeleeCombatBlockInputName);
		EditorGUILayout.PropertyField (ignoreUseDrawKeepWeaponAnimation);
		EditorGUILayout.PropertyField (weaponsCanBeStolenFromCharacter);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showDurabilitySettings ()
	{
		showSectionTitle ("DURABILITY SETTINGS");

		GUILayout.BeginVertical ("Weapon Durability Settings", "window");
		EditorGUILayout.PropertyField (checkDurabilityOnAttackEnabled);
		EditorGUILayout.PropertyField (checkDurabilityOnBlockEnabled);

		EditorGUILayout.PropertyField (generalAttackDurabilityMultiplier);
		EditorGUILayout.PropertyField (generalBlockDurabilityMultiplier);
	
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventOnDurabilityEmptyOnMeleeWeapon);

		if (useEventOnDurabilityEmptyOnMeleeWeapon.boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnDurabilityEmptyOnMeleeWeapon);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventOnDurabilityEmptyOnBlock);

		if (useEventOnDurabilityEmptyOnBlock.boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnDurabilityEmptyOnBlock);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showWeaponExtraActionSettings ()
	{
		showSectionTitle ("WEAPON EXTRA ACTION SETTINGS");

		GUILayout.BeginVertical ("Weapon Extra Action Settings", "window");
		EditorGUILayout.PropertyField (showExtraActionIconTexture);
		EditorGUILayout.PropertyField (extraActionIconGameObject);
		EditorGUILayout.PropertyField (extraActionRawImage);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showDebugSettings ()
	{
		showSectionTitle ("DEBUG SETTINGS");

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (showDebugPrint);
		EditorGUILayout.PropertyField (showDebugDraw);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (grabbedObjectMeleeAttackActive);
		EditorGUILayout.PropertyField (carryingObject);
		EditorGUILayout.PropertyField (attackInProcess);

		EditorGUILayout.PropertyField (currentHitCombat);
		EditorGUILayout.PropertyField (dualWieldHitCombat);
		EditorGUILayout.PropertyField (currentAttackIndex);
		EditorGUILayout.PropertyField (blockActive);
		EditorGUILayout.PropertyField (blockActivePreviously);
		EditorGUILayout.PropertyField (objectThrown);
		EditorGUILayout.PropertyField (cuttingModeActive);
		EditorGUILayout.PropertyField (attackInputPausedForStamina);
		EditorGUILayout.PropertyField (damageTriggerInProcess);
		EditorGUILayout.PropertyField (meleeAttackInputPaused);
		EditorGUILayout.PropertyField (reducedBlockDamageProtectionActive);
		EditorGUILayout.PropertyField (canCancelBlockToStartAttackActive);
		EditorGUILayout.PropertyField (blockInputPaused);
		EditorGUILayout.PropertyField (isDualWieldWeapon);
		EditorGUILayout.PropertyField (mainDualWieldMeleeWeaponObjectSystem);
		EditorGUILayout.PropertyField (throwWeaponActionPaused);
		EditorGUILayout.PropertyField (shieldActive);
		EditorGUILayout.PropertyField (carryingShield);
		EditorGUILayout.PropertyField (currentShieldName);
		EditorGUILayout.PropertyField (currentShieldGameObject);
		EditorGUILayout.PropertyField (currentShieldHandMountPointTransformReference);
		EditorGUILayout.PropertyField (currentShieldBackMountPointTransformReference);

		EditorGUILayout.Space ();

		showSimpleList (meleeAttackTypesAmount, "Melee Attack Types Amount", false);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showEventSettings ()
	{
		showSectionTitle ("EVENT SETTINGS");

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (useEventsOnAttack);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (eventOnAttackStart);
		EditorGUILayout.PropertyField (eventOnAttackEnd);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventsOnBlockDamage);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (eventOnBlockActivate);
		EditorGUILayout.PropertyField (eventOnBlockDeactivate);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventsOnGrabDropObject);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (eventOnGrabObject);
		EditorGUILayout.PropertyField (eventOnDropObject);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventsOnAttackCantBeBlocked);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (eventOnAttackCantBeBlocked);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (hideInventoryQuickAccessSlotsWhenCarryingMeleeWeapon);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (eventOnHideInventoryQuickAccessSlots);
		EditorGUILayout.PropertyField (eventOnShowInventoryQuickAccessSlots);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showComponents ()
	{
		showSectionTitle ("COMPONENTS");

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (currentGrabbedObjectTransform);
		EditorGUILayout.PropertyField (mainRemoteEventSystem);
		EditorGUILayout.PropertyField (playerControllerGameObject);
		EditorGUILayout.PropertyField (mainPlayerController);
		EditorGUILayout.PropertyField (mainGrabObjects);
		EditorGUILayout.PropertyField (mainHealth);
		EditorGUILayout.PropertyField (mainCameraTransform);
		EditorGUILayout.PropertyField (mainStaminaSystem);
		EditorGUILayout.PropertyField (handPositionReference);
		EditorGUILayout.PropertyField (mainAudioSource);
		EditorGUILayout.PropertyField (mainSliceSystem);
		EditorGUILayout.PropertyField (mainAnimator);
		EditorGUILayout.PropertyField (mainFindObjectivesSystem);
		EditorGUILayout.PropertyField (mainMeleeWeaponsGrabbedManager);
		EditorGUILayout.PropertyField (playerInput);
		EditorGUILayout.PropertyField (parentToPlaceTriggerInFrontOfCharacter);
		EditorGUILayout.PropertyField (mainMeleeCombatThrowReturnWeaponSystem);

		GUILayout.EndVertical ();
	}

	void showGrabbedWeaponInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
	
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Movement Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNewIdleID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("idleIDUsed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStrafeMode"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("strafeIDUsed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setPreviousStrafeModeOnDropObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("previousStrafeMode"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateStrafeModeOnLockOnTargetActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivateStrafeModeOnLockOnTargetDeactivate"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("toggleStrafeModeIfRunningActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setSprintEnabledStateWithWeapon"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sprintEnabledStateWithWeapon"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNewCrouchID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchIDUsed"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNewMovementID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("movementIDUsed"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Shield Settings", "window");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponCanUseShield"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shieldID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("regularMovementBlockShieldActionName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("strafeMovementBlockShieldID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shieldIDFreeMovement"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shieldIDStrafeMovement"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isEmptyWeaponToUseOnlyShield"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventsOnGrabDropObject"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnGrabObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnDropObject"));
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Keep/Draw Weapon Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventsOnKeepOrDrawMeleeWeapon"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventsOnKeepMeleeWeapon"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventsOnDrawMeleeWeapon"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Remote Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEventsOnGrabObject"));

		EditorGUILayout.Space ();

		showSimpleList (list.FindPropertyRelative ("remoteEventOnGrabObject"), "Remote Event On Grab Object", true);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEventsOnDropObject"));

		EditorGUILayout.Space ();

		showSimpleList (list.FindPropertyRelative ("remoteEventOnDropObject"), "Remote Event On Drop Object", true);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attacks Unable To Block Settingss", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attacksCantBeBlocked"));
		showSimpleList (list.FindPropertyRelative ("attackIDCantBeBlockedList"), "Attack ID Cant Be Blocked List", true);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Damage Detected Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventsOnDamageDetected"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkObjectsToUseRemoteEventsOnDamage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("layerToUseRemoteEventsOnDamage"));

		showEventOnDamageInfoList (list.FindPropertyRelative ("eventOnDamageInfoList"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Obtain Health From Damage Applied Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("getHealthFromDamagingObjects"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("healthFromDamagingObjectsMultiplier"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Obtain Health From Blocks Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("getHealthFromBlocks"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("healthAmountFromBlocks"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("getHealthFromPerfectBlocks"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("healthAmountFromPerfectBlocks"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Range Attack Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRangeAttackID"));

		if (list.FindPropertyRelative ("useRangeAttackID").boolValue) {
			showRangeAttackInfoList (list.FindPropertyRelative ("rangeAttackInfoList"));		
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();


		GUILayout.BeginVertical ("Press Up/Down Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventsOnPressDownState"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnPressDownState"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventsOnPressUpState"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnPressUpState"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();


		GUILayout.BeginVertical ("Deflect Projectile Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventsOnDeflectProjectile"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventsOnDeflectProjectile"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();


		GUILayout.BeginVertical ("Custom Position Reference Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomGrabbedWeaponReferencePosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("customGrabbedWeaponReferencePosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReferencePositionToKeepObjectMesh"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("customReferencePositionToKeepObjectMesh"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReferencePositionToKeepObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("customReferencePositionToKeepObject"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attack List Info Template Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMeleeWeaponAttackInfoList"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setInitialWeaponAttackInfoList"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("initialWeaponAttackInfoListIndex"));

		showMeleeWeaponAttackInfoList (list.FindPropertyRelative ("meleeWeaponAttackInfoList"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Durability Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnEmptyDurability"));
		if (list.FindPropertyRelative ("useEventOnEmptyDurability").boolValue) {

			EditorGUILayout.Space (); 

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnEmptyDurability"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space (); 

		GUILayout.EndVertical ();
	}

	void showGrabbedWeaponInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();
	
		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}
	
		EditorGUILayout.Space ();
	
		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");
	
			EditorGUILayout.Space ();
	
			GUILayout.Label ("Number Of Weapons: \t" + list.arraySize);
	
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
	
					SerializedProperty currentArryElement = list.GetArrayElementAtIndex (i);
	
					EditorGUILayout.PropertyField (currentArryElement, false);
					if (currentArryElement.isExpanded) {
						expanded = true;
						showGrabbedWeaponInfoListElement (currentArryElement);
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
	
			GUILayout.EndVertical ();
		}
	}

	void showSurfaceInfoOnMeleeAttackList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Surfaces: \t" + list.arraySize);

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

					SerializedProperty currentArryElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArryElement, false);
					if (currentArryElement.isExpanded) {
						expanded = true;
						showSurfaceInfoOnMeleeAttackListElement (currentArryElement);
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

			GUILayout.EndVertical ();
		}
	}

	void showSurfaceInfoOnMeleeAttackListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceName"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sounds And Particles Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSoundsListOnOrder"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentSoundIndex"));
		showSimpleList (list.FindPropertyRelative ("soundsList"), "Sounds List", true);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useParticlesOnSurface"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("particlesOnSurface"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceActivatesBounceOnCharacter"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnBounceCharacter"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopAttackOnBounce"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showEventOnDamageInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Elements: \t" + list.arraySize);

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

					SerializedProperty currentArryElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArryElement, false);
					if (currentArryElement.isExpanded) {
						expanded = true;
						showEventOnDamageInfoListElement (currentArryElement);
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

			GUILayout.EndVertical ();
		}
	}

	void showEventOnDamageInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageInfoID"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnDamageDetected"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEvent"));
		if (list.FindPropertyRelative ("useRemoteEvent").boolValue) {
			showSimpleList (list.FindPropertyRelative ("remoteEventNameList"), "Remote Event Name List", true);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showRangeAttackInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Elements: \t" + list.arraySize);

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

					SerializedProperty currentArryElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArryElement, false);
					if (currentArryElement.isExpanded) {
						expanded = true;
						showRangeAttackInfoListElement (currentArryElement);
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

			GUILayout.EndVertical ();
		}
	}

	void showRangeAttackInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("rangeAttackID"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnRangeAttack"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnDisableRangeAttack"));
	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showMeleeWeaponAttackInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Elements: \t" + list.arraySize);

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

					SerializedProperty currentArryElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArryElement, false);
					if (currentArryElement.isExpanded) {
						expanded = true;
						showMeleeWeaponAttackInfoListElement (currentArryElement);
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

			GUILayout.EndVertical ();
		}
	}

	void showMeleeWeaponAttackInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ID"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("mainMeleeWeaponInfo"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list, string listName, bool showAddClearButtons)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Elements: \t" + list.arraySize);

			if (showAddClearButtons) {
				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Add")) {
					list.arraySize++;
				}
				if (GUILayout.Button ("Clear")) {
					list.arraySize = 0;
				}
				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
					return;
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
}
#endif