using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(closeCombatSystem))]
public class closeCombatSystemEditor : Editor
{
	SerializedProperty combatSystemEnabled;
	SerializedProperty layerMaskToDamage;

	SerializedProperty setCanActivateReactionSystemTemporallyState;
	SerializedProperty canActivateReactionSystemTemporally;

	SerializedProperty useCustomIgnoreTags;
	SerializedProperty customTagsToIgnoreList;

	SerializedProperty addForceMultiplier;
	SerializedProperty hitCombatPrefab;
	SerializedProperty combatLimbList;
	SerializedProperty currentPlayerMode;
	SerializedProperty timerCombat;
	SerializedProperty combatPlaying;
	SerializedProperty currentComboInProcessName;
	SerializedProperty previousComboInProcessName;
	SerializedProperty combatTypeInfoList;
	SerializedProperty useEventsOnStateChange;
	SerializedProperty evenOnStateEnabled;
	SerializedProperty eventOnStateDisabled;

	SerializedProperty playerControllerManager;
	SerializedProperty animator;
	SerializedProperty mainHeadTrack;

	SerializedProperty mainHealth;

	SerializedProperty showGizmo;
	SerializedProperty gizmoColor;
	SerializedProperty gizmoRadius;
	SerializedProperty gizmoLabelColor;

	SerializedProperty useAnimationPercentageDuration;
	SerializedProperty useAnimationPercentageOver100;

	SerializedProperty setCombatIdleID;
	SerializedProperty combatIdleID;

	SerializedProperty showDebugPrint;

	SerializedProperty activateStrafeModeOnLockOnTargetActive;

	SerializedProperty toggleStrafeModeIfRunningActive;

	SerializedProperty useStrafeMode;
	SerializedProperty strafeIDUsed;
	SerializedProperty setPreviousStrafeModeOnDisableMode;



	SerializedProperty blockEnabled;
	SerializedProperty blockActive;

	SerializedProperty generalBlockProtectionMultiplier;

	SerializedProperty cancelBlockReactionStateName;

	SerializedProperty mainMeleeCombatBlockInputName;

	SerializedProperty blockActionName;

	SerializedProperty blockID;

	SerializedProperty blockActivePreviously;

	SerializedProperty useMaxBlockRangeAngle;
	SerializedProperty maxBlockRangeAngle;

	SerializedProperty blockDamageProtectionAmount;

	SerializedProperty reducedBlockDamageProtectionAmount;

	SerializedProperty reducedBlockDamageProtectionActive;

	SerializedProperty canCancelBlockToStartAttackActive;

	SerializedProperty blockInputPaused;

	SerializedProperty useEventsOnBlockDamage;
	SerializedProperty eventOnBlockActivate;
	SerializedProperty eventOnBlockDeactivate;


	SerializedProperty useEventsOnAttackCantBeBlocked;
	SerializedProperty eventOnAttackCantBeBlocked;


	SerializedProperty useMatchTargetSystemOnAttack;
	SerializedProperty ignoreAttackSettingToMatchTarget;
	SerializedProperty mainMatchPlayerToTargetSystem;

	SerializedProperty useMainMatchPositionOffset;
	SerializedProperty mainMatchPositionOffset;

	SerializedProperty checkSurfaceInfoEnabled;
	SerializedProperty surfaceInfoOnMeleeAttackNameForNotFound;
	SerializedProperty surfaceInfoOnMeleeAttackNameForSwingOnAir;
	SerializedProperty surfaceInfoOnMeleeAttackList;

	SerializedProperty mainAudioSource;

	SerializedProperty combatTypeID;

	SerializedProperty showMovementSettings;
	SerializedProperty showMatchPositionSettings;
	SerializedProperty showBlockSettings;
	SerializedProperty showSurfaceSettings;
	SerializedProperty showCombatLimbListSettings;
	SerializedProperty showAttackListSettings;

	SerializedProperty useStaminaOnAttackEnabled;
	SerializedProperty attackInputPausedForStamina;
	SerializedProperty attackStaminaStateName;
	SerializedProperty generalStaminaUseMultiplier;
	SerializedProperty mainStaminaSystem;

	SerializedProperty playerOnGroundToActivateAttack;

	SerializedProperty checkExternalControllerBehaviorActiveList;

	SerializedProperty externalControllerBehaviorNameListToUseCloseCombat;

	SerializedProperty canMoveStatePaused;

	SerializedProperty rootMotionStateChanged;

	SerializedProperty headTrackStateChanged;

	SerializedProperty showEventsSettings;

	SerializedProperty showDebugSettings;
	SerializedProperty showAllSettings;
	SerializedProperty showComponents;

	SerializedProperty activateCloseCombatSystemAtStart;

	SerializedProperty canUseAttacksWithoutCombatActiveEnabled;

	SerializedProperty hideMeleeWeaponsOnExternalCombat;
	SerializedProperty hideFireWeaponsOnExternalCombat;

	SerializedProperty ignoreGetHealthFromDamagingObjects;

	SerializedProperty getHealthFromBlocks;
	SerializedProperty healthAmountFromBlocks;

	SerializedProperty getHealthFromPerfectBlocks;
	SerializedProperty healthAmountFromPerfectBlocks;

	closeCombatSystem combatManager;

	GUIStyle style = new GUIStyle ();

	string currentLabel;

	Color buttonColor;

	string buttonText;


	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		combatSystemEnabled = serializedObject.FindProperty ("combatSystemEnabled");
		layerMaskToDamage = serializedObject.FindProperty ("layerMaskToDamage");

		setCanActivateReactionSystemTemporallyState = serializedObject.FindProperty ("setCanActivateReactionSystemTemporallyState");
		canActivateReactionSystemTemporally = serializedObject.FindProperty ("canActivateReactionSystemTemporally");

		useCustomIgnoreTags = serializedObject.FindProperty ("useCustomIgnoreTags");
		customTagsToIgnoreList = serializedObject.FindProperty ("customTagsToIgnoreList");

		addForceMultiplier = serializedObject.FindProperty ("addForceMultiplier");
		hitCombatPrefab = serializedObject.FindProperty ("hitCombatPrefab");
		combatLimbList = serializedObject.FindProperty ("combatLimbList");
		currentPlayerMode = serializedObject.FindProperty ("currentPlayerMode");
		timerCombat = serializedObject.FindProperty ("timerCombat");
		combatPlaying = serializedObject.FindProperty ("combatPlaying");
		currentComboInProcessName = serializedObject.FindProperty ("currentComboInProcessName");
		previousComboInProcessName = serializedObject.FindProperty ("previousComboInProcessName");
		combatTypeInfoList = serializedObject.FindProperty ("combatTypeInfoList");
		useEventsOnStateChange = serializedObject.FindProperty ("useEventsOnStateChange");
		evenOnStateEnabled = serializedObject.FindProperty ("evenOnStateEnabled");
		eventOnStateDisabled = serializedObject.FindProperty ("eventOnStateDisabled");

		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		animator = serializedObject.FindProperty ("animator");
		mainHeadTrack = serializedObject.FindProperty ("mainHeadTrack");

		mainHealth = serializedObject.FindProperty ("mainHealth");

		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoColor = serializedObject.FindProperty ("gizmoColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");

		useAnimationPercentageDuration = serializedObject.FindProperty ("useAnimationPercentageDuration");
		useAnimationPercentageOver100 = serializedObject.FindProperty ("useAnimationPercentageOver100");

		setCombatIdleID = serializedObject.FindProperty ("setCombatIdleID");
		combatIdleID = serializedObject.FindProperty ("combatIdleID");

		showDebugPrint = serializedObject.FindProperty ("showDebugPrint");

		activateStrafeModeOnLockOnTargetActive = serializedObject.FindProperty ("activateStrafeModeOnLockOnTargetActive");

		toggleStrafeModeIfRunningActive = serializedObject.FindProperty ("toggleStrafeModeIfRunningActive");

		useStrafeMode = serializedObject.FindProperty ("useStrafeMode");
		strafeIDUsed = serializedObject.FindProperty ("strafeIDUsed");
		setPreviousStrafeModeOnDisableMode = serializedObject.FindProperty ("setPreviousStrafeModeOnDisableMode");

		blockEnabled = serializedObject.FindProperty ("blockEnabled");
		blockActive = serializedObject.FindProperty ("blockActive");

		generalBlockProtectionMultiplier = serializedObject.FindProperty ("generalBlockProtectionMultiplier");
		cancelBlockReactionStateName = serializedObject.FindProperty ("cancelBlockReactionStateName");
		mainMeleeCombatBlockInputName = serializedObject.FindProperty ("mainMeleeCombatBlockInputName");
		blockActionName = serializedObject.FindProperty ("blockActionName");
		blockID = serializedObject.FindProperty ("blockID");
		blockActivePreviously = serializedObject.FindProperty ("blockActivePreviously");
		useMaxBlockRangeAngle = serializedObject.FindProperty ("useMaxBlockRangeAngle");
		maxBlockRangeAngle = serializedObject.FindProperty ("maxBlockRangeAngle");
		blockDamageProtectionAmount = serializedObject.FindProperty ("blockDamageProtectionAmount");
		reducedBlockDamageProtectionAmount = serializedObject.FindProperty ("reducedBlockDamageProtectionAmount");
		reducedBlockDamageProtectionActive = serializedObject.FindProperty ("reducedBlockDamageProtectionActive");
		canCancelBlockToStartAttackActive = serializedObject.FindProperty ("canCancelBlockToStartAttackActive");
		blockInputPaused = serializedObject.FindProperty ("blockInputPaused");
		useEventsOnBlockDamage = serializedObject.FindProperty ("useEventsOnBlockDamage");
		eventOnBlockActivate = serializedObject.FindProperty ("eventOnBlockActivate");
		eventOnBlockDeactivate = serializedObject.FindProperty ("eventOnBlockDeactivate");	 

		useEventsOnAttackCantBeBlocked = serializedObject.FindProperty ("useEventsOnAttackCantBeBlocked");	 
		eventOnAttackCantBeBlocked = serializedObject.FindProperty ("eventOnAttackCantBeBlocked");	


		useMatchTargetSystemOnAttack = serializedObject.FindProperty ("useMatchTargetSystemOnAttack");
		ignoreAttackSettingToMatchTarget = serializedObject.FindProperty ("ignoreAttackSettingToMatchTarget");
		mainMatchPlayerToTargetSystem = serializedObject.FindProperty ("mainMatchPlayerToTargetSystem");

		useMainMatchPositionOffset = serializedObject.FindProperty ("useMainMatchPositionOffset");
		mainMatchPositionOffset = serializedObject.FindProperty ("mainMatchPositionOffset");

		checkSurfaceInfoEnabled = serializedObject.FindProperty ("checkSurfaceInfoEnabled");
		surfaceInfoOnMeleeAttackNameForNotFound = serializedObject.FindProperty ("surfaceInfoOnMeleeAttackNameForNotFound");
		surfaceInfoOnMeleeAttackNameForSwingOnAir = serializedObject.FindProperty ("surfaceInfoOnMeleeAttackNameForSwingOnAir");
		surfaceInfoOnMeleeAttackList = serializedObject.FindProperty ("surfaceInfoOnMeleeAttackList");

		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");

		combatTypeID = serializedObject.FindProperty ("combatTypeID");

		showMovementSettings = serializedObject.FindProperty ("showMovementSettings");
		showMatchPositionSettings = serializedObject.FindProperty ("showMatchPositionSettings");
		showBlockSettings = serializedObject.FindProperty ("showBlockSettings");
		showSurfaceSettings = serializedObject.FindProperty ("showSurfaceSettings");
		showCombatLimbListSettings = serializedObject.FindProperty ("showCombatLimbListSettings");
		showAttackListSettings = serializedObject.FindProperty ("showAttackListSettings");

		useStaminaOnAttackEnabled = serializedObject.FindProperty ("useStaminaOnAttackEnabled");
		attackInputPausedForStamina = serializedObject.FindProperty ("attackInputPausedForStamina");
		attackStaminaStateName = serializedObject.FindProperty ("attackStaminaStateName");
		generalStaminaUseMultiplier = serializedObject.FindProperty ("generalStaminaUseMultiplier");
		mainStaminaSystem = serializedObject.FindProperty ("mainStaminaSystem");

		canMoveStatePaused = serializedObject.FindProperty ("canMoveStatePaused");

		rootMotionStateChanged = serializedObject.FindProperty ("rootMotionStateChanged");

		headTrackStateChanged = serializedObject.FindProperty ("headTrackStateChanged");

		playerOnGroundToActivateAttack = serializedObject.FindProperty ("playerOnGroundToActivateAttack");

		checkExternalControllerBehaviorActiveList = serializedObject.FindProperty ("checkExternalControllerBehaviorActiveList");

		externalControllerBehaviorNameListToUseCloseCombat = serializedObject.FindProperty ("externalControllerBehaviorNameListToUseCloseCombat");

		showEventsSettings = serializedObject.FindProperty ("showEventsSettings");

		showDebugSettings = serializedObject.FindProperty ("showDebugSettings");
		showAllSettings = serializedObject.FindProperty ("showAllSettings");
		showComponents = serializedObject.FindProperty ("showComponents");

		activateCloseCombatSystemAtStart = serializedObject.FindProperty ("activateCloseCombatSystemAtStart");

		canUseAttacksWithoutCombatActiveEnabled = serializedObject.FindProperty ("canUseAttacksWithoutCombatActiveEnabled");

		hideMeleeWeaponsOnExternalCombat = serializedObject.FindProperty ("hideMeleeWeaponsOnExternalCombat");
		hideFireWeaponsOnExternalCombat = serializedObject.FindProperty ("hideFireWeaponsOnExternalCombat");

		ignoreGetHealthFromDamagingObjects = serializedObject.FindProperty ("ignoreGetHealthFromDamagingObjects");

		getHealthFromBlocks = serializedObject.FindProperty ("getHealthFromBlocks");
		healthAmountFromBlocks = serializedObject.FindProperty ("healthAmountFromBlocks");

		getHealthFromPerfectBlocks = serializedObject.FindProperty ("getHealthFromPerfectBlocks");
		healthAmountFromPerfectBlocks = serializedObject.FindProperty ("healthAmountFromPerfectBlocks");

		combatManager = (closeCombatSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (combatManager.showGizmo) {
			style.normal.textColor = combatManager.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;

			for (int i = 0; i < combatManager.combatLimbList.Count; i++) {
				if (combatManager.combatLimbList [i].limb != null) {
					
					currentLabel = combatManager.combatLimbList [i].name + "\n" + combatManager.combatLimbList [i].hitDamage;

					Handles.Label (combatManager.combatLimbList [i].limb.transform.position, currentLabel, style);	
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{	
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (combatSystemEnabled);
		EditorGUILayout.PropertyField (combatTypeID);

		EditorGUILayout.PropertyField (activateCloseCombatSystemAtStart);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window");
		EditorGUILayout.PropertyField (canUseAttacksWithoutCombatActiveEnabled);
		EditorGUILayout.PropertyField (hideMeleeWeaponsOnExternalCombat);
		EditorGUILayout.PropertyField (hideFireWeaponsOnExternalCombat);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Damage Settings", "window");
		EditorGUILayout.PropertyField (layerMaskToDamage);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useCustomIgnoreTags);
		if (useCustomIgnoreTags.boolValue) {
			showSimpleList (customTagsToIgnoreList, "Custom Tags To Ignore List");
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (addForceMultiplier);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (setCanActivateReactionSystemTemporallyState);
		if (setCanActivateReactionSystemTemporallyState.boolValue) {
			EditorGUILayout.PropertyField (canActivateReactionSystemTemporally);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Get Health From Attacks And Block Settings", "window");
		EditorGUILayout.PropertyField (ignoreGetHealthFromDamagingObjects);
		EditorGUILayout.PropertyField (getHealthFromBlocks);
		EditorGUILayout.PropertyField (healthAmountFromBlocks);
		EditorGUILayout.PropertyField (getHealthFromPerfectBlocks);
		EditorGUILayout.PropertyField (healthAmountFromPerfectBlocks);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.BeginHorizontal ();
	
		if (showMovementSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Movement & Animation")) {
			showMovementSettings.boolValue = !showMovementSettings.boolValue;
		}

		if (showMatchPositionSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Match Position")) {
			showMatchPositionSettings.boolValue = !showMatchPositionSettings.boolValue;
		}

		if (showBlockSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Block")) {
			showBlockSettings.boolValue = !showBlockSettings.boolValue;
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();

		if (showSurfaceSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Surfaces")) {
			showSurfaceSettings.boolValue = !showSurfaceSettings.boolValue;
		}

		if (showCombatLimbListSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Combat Limb List")) {
			showCombatLimbListSettings.boolValue = !showCombatLimbListSettings.boolValue;
		}

		if (showAttackListSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Attack List")) {
			showAttackListSettings.boolValue = !showAttackListSettings.boolValue;
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();

		if (showEventsSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Events")) {
			showEventsSettings.boolValue = !showEventsSettings.boolValue;
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
			buttonText = "Hide All Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			buttonText = "Show All Settings";
		}
		if (GUILayout.Button (buttonText)) {
			showAllSettings.boolValue = !showAllSettings.boolValue;

			showMovementSettings.boolValue = showAllSettings.boolValue;
			showMatchPositionSettings.boolValue = showAllSettings.boolValue;
			showBlockSettings.boolValue = showAllSettings.boolValue;
			showSurfaceSettings.boolValue = showAllSettings.boolValue;
			showCombatLimbListSettings.boolValue = showAllSettings.boolValue;
			showAttackListSettings.boolValue = showAllSettings.boolValue;

			showEventsSettings.boolValue = showAllSettings.boolValue;

			showDebugSettings.boolValue = showAllSettings.boolValue;

			showComponents.boolValue = false;
		}

		if (showComponents.boolValue) {
			GUI.backgroundColor = Color.gray;
			buttonText = "Hide Player Components";
		} else {
			GUI.backgroundColor = buttonColor;
			buttonText = "Show Player Components";
		}
		if (GUILayout.Button (buttonText)) {
			showComponents.boolValue = !showComponents.boolValue;
		}

		GUI.backgroundColor = buttonColor;

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 30;
		style.alignment = TextAnchor.MiddleCenter;
	

		if (showAllSettings.boolValue || showMovementSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("MOVEMENT & ANIMATION SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Movement Settings", "window");
			EditorGUILayout.PropertyField (useStrafeMode);
			EditorGUILayout.PropertyField (strafeIDUsed);
			EditorGUILayout.PropertyField (setPreviousStrafeModeOnDisableMode);

			EditorGUILayout.PropertyField (activateStrafeModeOnLockOnTargetActive);
			EditorGUILayout.PropertyField (toggleStrafeModeIfRunningActive);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (playerOnGroundToActivateAttack);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (checkExternalControllerBehaviorActiveList);
			if (checkExternalControllerBehaviorActiveList.boolValue) {

				EditorGUILayout.Space ();

				showSimpleList (externalControllerBehaviorNameListToUseCloseCombat, "External Controller Behavior Name List To Use Combat");
			} 
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Animation Settings", "window");
			EditorGUILayout.PropertyField (setCombatIdleID);
			EditorGUILayout.PropertyField (combatIdleID);
			EditorGUILayout.PropertyField (useAnimationPercentageDuration);
			EditorGUILayout.PropertyField (useAnimationPercentageOver100);

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showMatchPositionSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("MATCH POSITION SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Match Attack Position Settings", "window");
			EditorGUILayout.PropertyField (useMatchTargetSystemOnAttack);
			if (useMatchTargetSystemOnAttack.boolValue) {
				EditorGUILayout.PropertyField (ignoreAttackSettingToMatchTarget);
				EditorGUILayout.PropertyField (mainMatchPlayerToTargetSystem);
				EditorGUILayout.PropertyField (useMainMatchPositionOffset);
				EditorGUILayout.PropertyField (mainMatchPositionOffset);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
			
		if (showAllSettings.boolValue || showBlockSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("BLOCK SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Block Settings", "window");
			EditorGUILayout.PropertyField (blockEnabled);
			if (blockEnabled.boolValue) {
				EditorGUILayout.PropertyField (generalBlockProtectionMultiplier);
				EditorGUILayout.PropertyField (cancelBlockReactionStateName);
				EditorGUILayout.PropertyField (mainMeleeCombatBlockInputName);
				EditorGUILayout.PropertyField (blockActionName);
				EditorGUILayout.PropertyField (blockID);
				EditorGUILayout.PropertyField (blockActivePreviously);
				EditorGUILayout.PropertyField (useMaxBlockRangeAngle);
				EditorGUILayout.PropertyField (maxBlockRangeAngle);
				EditorGUILayout.PropertyField (blockDamageProtectionAmount);
				EditorGUILayout.PropertyField (reducedBlockDamageProtectionAmount);
				EditorGUILayout.PropertyField (reducedBlockDamageProtectionActive);
				EditorGUILayout.PropertyField (canCancelBlockToStartAttackActive);
				EditorGUILayout.PropertyField (blockInputPaused);

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (useEventsOnBlockDamage);
				if (useEventsOnBlockDamage.boolValue) {
					EditorGUILayout.PropertyField (eventOnBlockActivate);
					EditorGUILayout.PropertyField (eventOnBlockDeactivate);
				}

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (useEventsOnAttackCantBeBlocked);
				if (useEventsOnAttackCantBeBlocked.boolValue) {
					EditorGUILayout.PropertyField (eventOnAttackCantBeBlocked);
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showSurfaceSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("SURFACE SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Surface Settings", "window");
			EditorGUILayout.PropertyField (checkSurfaceInfoEnabled);
			if (checkSurfaceInfoEnabled.boolValue) {
				EditorGUILayout.PropertyField (surfaceInfoOnMeleeAttackNameForNotFound);
				EditorGUILayout.PropertyField (surfaceInfoOnMeleeAttackNameForSwingOnAir);

				EditorGUILayout.Space ();

				showSurfaceInfoOnMeleeAttackList (surfaceInfoOnMeleeAttackList);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showCombatLimbListSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("COMBAT LIMB LIST SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Combat Limb List", "window");

			EditorGUILayout.Space ();

			showCombatLimbList (combatLimbList);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Hit Combat Triggers Info")) {
				combatManager.udpateHitCombatInfo ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showAttackListSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("ATTACK LIST SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Stamina Settings", "window");
			EditorGUILayout.PropertyField (useStaminaOnAttackEnabled);
			if (useStaminaOnAttackEnabled.boolValue) {
				EditorGUILayout.PropertyField (attackStaminaStateName);
				EditorGUILayout.PropertyField (generalStaminaUseMultiplier);
				EditorGUILayout.PropertyField (mainStaminaSystem);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Combat Types List", "window");

			EditorGUILayout.Space ();

			showCombatTypeInfoList (combatTypeInfoList);

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showEventsSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("EVENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Events Settings", "window");
			EditorGUILayout.PropertyField (useEventsOnStateChange);
			if (useEventsOnStateChange.boolValue) {
				EditorGUILayout.PropertyField (evenOnStateEnabled);
				EditorGUILayout.PropertyField (eventOnStateDisabled);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showDebugSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("DEBUG SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window");
			EditorGUILayout.PropertyField (showGizmo);
			if (showGizmo.boolValue) {
				EditorGUILayout.PropertyField (gizmoColor);
				EditorGUILayout.PropertyField (gizmoRadius);
				EditorGUILayout.PropertyField (gizmoLabelColor);
			}
			GUILayout.EndVertical ();	

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Current State", "window");
			EditorGUILayout.PropertyField (showDebugPrint);
			GUILayout.Label ("Combat Mode Active\t\t" + currentPlayerMode.boolValue);
			GUILayout.Label ("Combat Timer\t\t" + timerCombat.floatValue);
			GUILayout.Label ("Combat Playing\t\t" + combatPlaying.boolValue);
			GUILayout.Label ("Current Combo\t\t" + currentComboInProcessName.stringValue);
			GUILayout.Label ("Previous Combo\t\t" + previousComboInProcessName.stringValue);
			GUILayout.Label ("Block Active\t\t" + blockActive.boolValue);
			GUILayout.Label ("Stamina Empty\t\t" + attackInputPausedForStamina.boolValue);

			GUILayout.Label ("Can Move Paused\t" + canMoveStatePaused.boolValue);
			GUILayout.Label ("Root Motion State\t" + rootMotionStateChanged.boolValue);
			GUILayout.Label ("Head Track State\t" + headTrackStateChanged.boolValue);

			GUILayout.EndVertical ();
		
			EditorGUILayout.Space ();
		}
			
		if (showComponents.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("COMPONENTS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Components", "window");
			EditorGUILayout.PropertyField (playerControllerManager);
			EditorGUILayout.PropertyField (animator);
			EditorGUILayout.PropertyField (mainHealth);
			EditorGUILayout.PropertyField (hitCombatPrefab);
			EditorGUILayout.PropertyField (mainAudioSource);
			EditorGUILayout.PropertyField (mainHeadTrack);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
			
		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showCombatLimbListElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hitDamage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("limbType"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("originalTriggerRadius"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("limb"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hitCombatManager"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("trigger"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("originalParent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("originalLocalPosition"));
		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}

	void showCombatLimbList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Combat Limb List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Limbs: \t" + list.arraySize);

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
						showCombatLimbListElementInfo (currentArryElement);
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

			if (GUILayout.Button ("Assign Basic Combat Triggers")) {
				combatManager.assignBasicCombatTriggers ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Clear Combat Triggers")) {
				combatManager.removeLimbParts ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}

	void showCombatTypeInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Combat Type Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Attacks Type: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add Type")) {
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
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArryElement = list.GetArrayElementAtIndex (i);

					string elementName = currentArryElement.displayName + " -> " + currentArryElement.FindPropertyRelative ("combatTypeDescription").stringValue;

					EditorGUILayout.PropertyField (currentArryElement, new GUIContent (elementName), false);
				
					if (currentArryElement.isExpanded) {
						expanded = true;

						EditorGUILayout.Space ();

						showCombatTypeInfoListElement (currentArryElement);
					}
					EditorGUILayout.Space ();


					GUILayout.EndVertical ();
				}

				EditorGUILayout.Space ();

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

				EditorGUILayout.Space ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}

	void showCombatTypeInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("combatTypeDescription"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("combatTypeID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDelayToStartAttackAfterFullCombo"));
		if (list.FindPropertyRelative ("useDelayToStartAttackAfterFullCombo").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToStartAttackAfterFullCombo"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMinTimeToChangeCombo"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canChangeOfComboWhenPreviousNotComplete"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("increaseAttackIndexOnlyOnAttackPerformedCorrectly"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDelayToStartSameAttack"));

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attacksUsedOnAir"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Combo State (Debug)", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("waitingToStartAttackActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentAttackIndex"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attackActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("comboCounter"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetingCombo"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Combo List", "window");
		showCombatAttackInfoList (list.FindPropertyRelative ("combatAttackInfoList"));

		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showCombatAttackInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Combat Attack Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Attacks: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Attack")) {
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
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArryElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArryElement, false);
					if (currentArryElement.isExpanded) {
						expanded = true;
						showCombatAttackInfoListElement (currentArryElement);
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

	void showCombatAttackInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAttackID"));
		if (list.FindPropertyRelative ("useAttackID").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("attackID"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("limbForAttack"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attackDuration"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("animationSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("minTimeToPlayNextAttack"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetComboIfIncorrectMinTime"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Movement Settings", "window");
//		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerOnGroundToActivateAttack"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canMoveWhileAttackActive"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRootMotionOnAttack"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseHeadTrackOnAttack"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseHeadTrackLookInOppositeDirection"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableGravityOnAttacks"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Stamina Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStaminaOnAttack"));

		if (list.FindPropertyRelative ("useStaminaOnAttack").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("staminaUsedOnAttack"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("customRefillStaminaDelayAfterUse"));
		}
		GUILayout.EndVertical ();


		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Obtain Health From Damage Applied Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("getHealthFromDamagingObjects"));

		if (list.FindPropertyRelative ("getHealthFromDamagingObjects").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("healthFromDamagingObjectsMultiplier"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Damage Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useExtraDamageMultiplier"), new GUIContent ("Use Damage Multiplier"));
		if (list.FindPropertyRelative ("useExtraDamageMultiplier").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraDamageMultiplier"), new GUIContent ("Damage Multiplier"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreActivateReactionSystem"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageReactionID"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("attackCantBeBlocked"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreStoreDetectedObjectOnList"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreForcesToApplyOnAttack"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attack State (Debug)", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playingNextAttack"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnAttackStart"));
		if (list.FindPropertyRelative ("useEventOnAttackStart").boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDelayOnEventOnAttackStart"));
			if (list.FindPropertyRelative ("useDelayOnEventOnAttackStart").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayOnEventOnAttackStart"));
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnAttackStart"));
		}

		EditorGUILayout.Space ();
	
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnAttackEnd"));
		if (list.FindPropertyRelative ("useEventOnAttackEnd").boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDelayOnEventOnAttackEnd"));
			if (list.FindPropertyRelative ("useDelayOnEventOnAttackEnd").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayOnEventOnAttackEnd"));
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnAttackEnd"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnDamage"));
		if (list.FindPropertyRelative ("useEventOnDamage").boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnDamage"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Remote Events Settings", "window");

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkObjectsToUseRemoteEventsOnDamage"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("layerToUseRemoteEventsOnDamage"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEvent"));

			EditorGUILayout.Space ();

			showSimpleList (list.FindPropertyRelative ("remoteEventNameList"), "Remote Event Name List");
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Damage Triggers Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDamageTriggerActiveInfo"));
		if (list.FindPropertyRelative ("useDamageTriggerActiveInfo").boolValue) {
			
			EditorGUILayout.Space ();

			showDamageTriggerActiveInfoList (list.FindPropertyRelative ("damageTriggerActiveInfoList"));

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventInfoList"));
		if (list.FindPropertyRelative ("useEventInfoList").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAccumulativeDelay"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Event Info List", "window");
			showEventInfoList (list.FindPropertyRelative ("eventInfoList"));
			GUILayout.EndVertical ();
		}

		GUILayout.EndVertical ();
		if (useMatchTargetSystemOnAttack.boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Match Attack Position Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMatchPositionSystem"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("matchPositionOffset"));
			GUILayout.EndVertical ();
		}
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Elements: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
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

	void showDamageTriggerActiveInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Damage Trigger Active Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of States: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
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
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArryElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArryElement, false);
					if (currentArryElement.isExpanded) {
						expanded = true;
						showDamageTriggerActiveInfoListElement (currentArryElement);
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

	void showDamageTriggerActiveInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToActiveTrigger"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateDamageTrigger"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayTriggered"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNewTriggerRadius"));
		if (list.FindPropertyRelative ("setNewTriggerRadius").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newTriggerRadius"));
//			EditorGUILayout.PropertyField (list.FindPropertyRelative ("setOriginalRadius"));
//			if (list.FindPropertyRelative ("setOriginalRadius").boolValue) {
//				EditorGUILayout.PropertyField (list.FindPropertyRelative ("originalRadius"));
//			}
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCastAllDamageDetection"));

		GUILayout.EndVertical ();
	}

	void showEventInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Event Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Events: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Event")) {
				list.arraySize++;
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

					SerializedProperty currentArryElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArryElement, false);
					if (currentArryElement.isExpanded) {
						showEventInfoListElement (currentArryElement);
						expanded = true;
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
		}
		GUILayout.EndVertical ();
	}

	void showEventInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToActivate"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToUse"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendCurrentPlayerOnEvent"));
		if (list.FindPropertyRelative ("sendCurrentPlayerOnEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSendCurrentPlayer"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();
	
		GUILayout.Label ("Debug");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventTriggered"));

		GUILayout.EndVertical ();
	}

	void showSurfaceInfoOnMeleeAttackList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Surface Info On Melee Attack List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of States: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
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
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceName"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSoundsListOnOrder"));

		EditorGUILayout.Space ();

		showSimpleList (list.FindPropertyRelative ("soundsList"), "Sounds List");

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useParticlesOnSurface"));
		if (list.FindPropertyRelative ("useParticlesOnSurface").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("particlesOnSurface"));
		}
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceActivatesBounceOnCharacter"));
		if (list.FindPropertyRelative ("surfaceActivatesBounceOnCharacter").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnBounceCharacter"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopAttackOnBounce"));
		}
		GUILayout.EndVertical ();
	}
}
#endif