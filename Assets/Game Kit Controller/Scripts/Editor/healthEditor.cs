using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(health))]
[CanEditMultipleObjects]
public class healthEditor : Editor
{
	SerializedProperty invincible;
	SerializedProperty healthAmount;
	SerializedProperty maxHealthAmount;
	SerializedProperty generalDamageMultiplerEnabled;
	SerializedProperty generalDamageMultiplerActive;
	SerializedProperty generalDamageMultiplier;
	SerializedProperty regenerateHealth;
	SerializedProperty constantRegenerate;
	SerializedProperty regenerateTime;
	SerializedProperty regenerateSpeed;
	SerializedProperty regenerateAmount;
	SerializedProperty currentID;
	SerializedProperty dead;
	SerializedProperty sedateActive;
	SerializedProperty currentSedateDuration;
	SerializedProperty useShield;
	SerializedProperty mainShieldSystem;
	SerializedProperty playerStatsManager;
	SerializedProperty healthStatName;
	SerializedProperty maxHealthStatName;
	SerializedProperty placeToShootActive;
	SerializedProperty placeToShoot;

	SerializedProperty checkDamageReceiverOnChildrenTransform;
	SerializedProperty damagePrefab;
	SerializedProperty mainDamageReceiver;
	SerializedProperty playerControllerManager;
	SerializedProperty ragdollManager;
	SerializedProperty damageInScreenManager;
	SerializedProperty mainInventoryCharacterCustomizationSystem;

	SerializedProperty showDamageDeadSettings;

	SerializedProperty useEventOnDamageEnabled;
	SerializedProperty eventOnDamage;

	SerializedProperty useEventOnDamageWithAttacker;
	SerializedProperty eventOnDamageWithAttacker;
	SerializedProperty useEventOnDamageWithAmount;
	SerializedProperty eventOnDamageWithAmount;
	SerializedProperty useExtraDamageFunctions;
	SerializedProperty extraDamageFunctionList;
	SerializedProperty useEventOnDamageShield;
	SerializedProperty eventOnDamageShield;
	SerializedProperty useEventOnDamageShieldWithAttacker;
	SerializedProperty eventOnDamageShieldWithAttacker;
	SerializedProperty deadFuncionCall;
	SerializedProperty useExtraDeadFunctions;
	SerializedProperty delayInExtraDeadFunctions;
	SerializedProperty extraDeadFunctionCall;
	SerializedProperty useEventWithAttackerOnDeath;
	SerializedProperty eventWithAttackerOnDeath;
	SerializedProperty resurrectFunctionCall;
	SerializedProperty resurrectAfterDelayEnabled;
	SerializedProperty resurrectDelay;
	SerializedProperty eventToResurrectAfterDelay;
	SerializedProperty useImpactSurface;

	SerializedProperty eventToSendCurrentHealthAmount;

	SerializedProperty impactDecalList;
	SerializedProperty impactDecalIndex;
	SerializedProperty impactDecalName;

	SerializedProperty mainDecalManagerName;

	SerializedProperty showSettings;
	SerializedProperty showAdvancedSettings;

	SerializedProperty healthSlider;
	SerializedProperty healthSliderText;

	SerializedProperty hideHealthSliderWhenNotDamageReceived;
	SerializedProperty timeToHideHealthSliderAfterDamage;
	SerializedProperty mainHealthSliderParent;
	SerializedProperty hiddenHealthSliderParent;
	SerializedProperty mainHealthSliderTransform;

	SerializedProperty useHealthSlider;
	SerializedProperty enemyHealthSlider;

	SerializedProperty useHealthSlideInfoOnScreen;

	SerializedProperty sliderOffset;
	SerializedProperty allyName;
	SerializedProperty allySliderColor;
	SerializedProperty enemyName;
	SerializedProperty enemySliderColor;
	SerializedProperty nameTextColor;
	SerializedProperty enemyTag;
	SerializedProperty friendTag;
	SerializedProperty removeHealthBarSliderOnDeath;
	SerializedProperty healthBarSliderActiveOnStart;
	SerializedProperty setHealthBarAsNotVisibleAtStart;
	SerializedProperty notHuman;
	SerializedProperty useWeakSpots;
	SerializedProperty showWeakSpotsInScannerMode;
	SerializedProperty useHealthAmountOnSpotEnabled;
	SerializedProperty scorchMarkPrefab;
	SerializedProperty layer;
	SerializedProperty weakSpotMesh;
	SerializedProperty weakSpotMeshAlphaValue;
	SerializedProperty haveRagdoll;
	SerializedProperty activateRagdollOnDamageReceived;
	SerializedProperty minDamageToEnableRagdoll;
	SerializedProperty ragdollEvent;
	SerializedProperty allowPushCharacterOnExplosions;
	SerializedProperty ragdollCanReceiveDamageOnImpact;
	SerializedProperty receiveDamageEvenDead;
	SerializedProperty canBeSedated;
	SerializedProperty awakeOnDamageIfSedated;
	SerializedProperty useEventOnSedate;
	SerializedProperty sedateStartEvent;
	SerializedProperty sedateEndEvent;

	SerializedProperty receiveDamageFromCollisionsEnabled;

	SerializedProperty minTimeToReceiveDamageOnImpact;

	SerializedProperty minVelocityToReceiveDamageOnImpact;

	SerializedProperty receiveDamageOnImpactMultiplier;


	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty alphaColor;

	SerializedProperty labelTextSize;

	SerializedProperty weakSpots;
	SerializedProperty addDamageReceiversToCustomTransformList;
	SerializedProperty customTransformListDamageReceiver;
	SerializedProperty useEventOnHealthValueList;
	SerializedProperty eventOnHealthValueList;


	SerializedProperty useDamageTypeCheck;
	SerializedProperty damageTypeInfoList;

	SerializedProperty showDamageReceivedDebugInfo;
	SerializedProperty healthAmountToTakeOnEditor;
	SerializedProperty healthAmountToGiveOnEditor;

	SerializedProperty blockDamageActive;
	SerializedProperty blockDamageProtectionAmount;
	SerializedProperty useEventsOnDamageBlocked;
	SerializedProperty eventOnDamageBlocked;

	SerializedProperty useDamageHitReaction;
	SerializedProperty mainDamageHitReactionSystem;

	SerializedProperty debugDamageSourceTransform;

	SerializedProperty objectIsCharacter;

	SerializedProperty useEventsOnInvincibleStateChange;
	SerializedProperty eventOnInvicibleOn;
	SerializedProperty eventOnInvicibleOff;

	SerializedProperty useEventOnDamageReceivedWithTemporalInvincibility;
	SerializedProperty eventOnDamageReceivedWithTemporalInvincibility;

	SerializedProperty maxDelayBetweenDamageReceivedAndInvincibilityStateActive;

	SerializedProperty sendInfoToCharacterCustomizationOnDamageEnabled;

	SerializedProperty mainManagerName;

	health healthManager;
	GUIStyle style = new GUIStyle ();

	GUIStyle gizmoStyle = new GUIStyle ();

	GUIStyle buttonStyle = new GUIStyle ();

	GUIStyle listButtonStyle = new GUIStyle ();

	Color defBackgroundColor;
	string label;
	string isDead;
	string sedateActiveText;

	bool expanded;

	void OnEnable ()
	{
		invincible = serializedObject.FindProperty ("invincible");
		healthAmount = serializedObject.FindProperty ("healthAmount");
		maxHealthAmount = serializedObject.FindProperty ("maxHealthAmount");
		generalDamageMultiplerEnabled = serializedObject.FindProperty ("generalDamageMultiplerEnabled");
		generalDamageMultiplerActive = serializedObject.FindProperty ("generalDamageMultiplerActive");
		generalDamageMultiplier = serializedObject.FindProperty ("generalDamageMultiplier");
		regenerateHealth = serializedObject.FindProperty ("regenerateHealth");
		constantRegenerate = serializedObject.FindProperty ("constantRegenerate");
		regenerateTime = serializedObject.FindProperty ("regenerateTime");
		regenerateSpeed = serializedObject.FindProperty ("regenerateSpeed");
		regenerateAmount = serializedObject.FindProperty ("regenerateAmount");
		currentID = serializedObject.FindProperty ("currentID");
		dead = serializedObject.FindProperty ("dead");
		sedateActive = serializedObject.FindProperty ("sedateActive");
		currentSedateDuration = serializedObject.FindProperty ("currentSedateDuration");
		useShield = serializedObject.FindProperty ("useShield");
		mainShieldSystem = serializedObject.FindProperty ("mainShieldSystem");
		playerStatsManager = serializedObject.FindProperty ("playerStatsManager");
		healthStatName = serializedObject.FindProperty ("healthStatName");
		maxHealthStatName = serializedObject.FindProperty ("maxHealthStatName");

		placeToShootActive = serializedObject.FindProperty ("placeToShootActive");
		placeToShoot = serializedObject.FindProperty ("placeToShoot");

		checkDamageReceiverOnChildrenTransform = serializedObject.FindProperty ("checkDamageReceiverOnChildrenTransform");
		damagePrefab = serializedObject.FindProperty ("damagePrefab");
		mainDamageReceiver = serializedObject.FindProperty ("mainDamageReceiver");

		mainInventoryCharacterCustomizationSystem = serializedObject.FindProperty ("mainInventoryCharacterCustomizationSystem");

		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		ragdollManager = serializedObject.FindProperty ("ragdollManager");
		damageInScreenManager = serializedObject.FindProperty ("damageInScreenManager");
		showDamageDeadSettings = serializedObject.FindProperty ("showDamageDeadSettings");


		useEventOnDamageEnabled = serializedObject.FindProperty ("useEventOnDamageEnabled");
		eventOnDamage = serializedObject.FindProperty ("eventOnDamage");

		useEventOnDamageWithAttacker = serializedObject.FindProperty ("useEventOnDamageWithAttacker");
		eventOnDamageWithAttacker = serializedObject.FindProperty ("eventOnDamageWithAttacker");
		useEventOnDamageWithAmount = serializedObject.FindProperty ("useEventOnDamageWithAmount");
		eventOnDamageWithAmount = serializedObject.FindProperty ("eventOnDamageWithAmount");
		useExtraDamageFunctions = serializedObject.FindProperty ("useExtraDamageFunctions");
		extraDamageFunctionList = serializedObject.FindProperty ("extraDamageFunctionList");
		useEventOnDamageShield = serializedObject.FindProperty ("useEventOnDamageShield");
		eventOnDamageShield = serializedObject.FindProperty ("eventOnDamageShield");
		useEventOnDamageShieldWithAttacker = serializedObject.FindProperty ("useEventOnDamageShieldWithAttacker");
		eventOnDamageShieldWithAttacker = serializedObject.FindProperty ("eventOnDamageShieldWithAttacker");
		deadFuncionCall = serializedObject.FindProperty ("deadFuncionCall");
		useExtraDeadFunctions = serializedObject.FindProperty ("useExtraDeadFunctions");
		delayInExtraDeadFunctions = serializedObject.FindProperty ("delayInExtraDeadFunctions");
		extraDeadFunctionCall = serializedObject.FindProperty ("extraDeadFunctionCall");
		useEventWithAttackerOnDeath = serializedObject.FindProperty ("useEventWithAttackerOnDeath");
		eventWithAttackerOnDeath = serializedObject.FindProperty ("eventWithAttackerOnDeath");
		resurrectFunctionCall = serializedObject.FindProperty ("resurrectFunctionCall");
		resurrectAfterDelayEnabled = serializedObject.FindProperty ("resurrectAfterDelayEnabled");
		resurrectDelay = serializedObject.FindProperty ("resurrectDelay");
		eventToResurrectAfterDelay = serializedObject.FindProperty ("eventToResurrectAfterDelay");
		useImpactSurface = serializedObject.FindProperty ("useImpactSurface");

		eventToSendCurrentHealthAmount = serializedObject.FindProperty ("eventToSendCurrentHealthAmount");

		impactDecalList = serializedObject.FindProperty ("impactDecalList");
		impactDecalIndex = serializedObject.FindProperty ("impactDecalIndex");
		impactDecalName = serializedObject.FindProperty ("impactDecalName");

		mainDecalManagerName = serializedObject.FindProperty ("mainDecalManagerName");

		showSettings = serializedObject.FindProperty ("showSettings");
		showAdvancedSettings = serializedObject.FindProperty ("showAdvancedSettings");

		healthSlider = serializedObject.FindProperty ("healthSlider");
		healthSliderText = serializedObject.FindProperty ("healthSliderText");

		hideHealthSliderWhenNotDamageReceived = serializedObject.FindProperty ("hideHealthSliderWhenNotDamageReceived");
		timeToHideHealthSliderAfterDamage = serializedObject.FindProperty ("timeToHideHealthSliderAfterDamage");
		mainHealthSliderParent = serializedObject.FindProperty ("mainHealthSliderParent");
		hiddenHealthSliderParent = serializedObject.FindProperty ("hiddenHealthSliderParent");
		mainHealthSliderTransform = serializedObject.FindProperty ("mainHealthSliderTransform");

		useHealthSlider = serializedObject.FindProperty ("settings.useHealthSlider");
		enemyHealthSlider = serializedObject.FindProperty ("settings.enemyHealthSlider");

		useHealthSlideInfoOnScreen = serializedObject.FindProperty ("useHealthSlideInfoOnScreen");


		sliderOffset = serializedObject.FindProperty ("settings.sliderOffset");
		allyName = serializedObject.FindProperty ("settings.allyName");
		allySliderColor = serializedObject.FindProperty ("settings.allySliderColor");
		enemyName = serializedObject.FindProperty ("settings.enemyName");
		enemySliderColor = serializedObject.FindProperty ("settings.enemySliderColor");
		nameTextColor = serializedObject.FindProperty ("settings.nameTextColor");
		enemyTag = serializedObject.FindProperty ("enemyTag");
		friendTag = serializedObject.FindProperty ("friendTag");
		removeHealthBarSliderOnDeath = serializedObject.FindProperty ("settings.removeHealthBarSliderOnDeath");
		healthBarSliderActiveOnStart = serializedObject.FindProperty ("settings.healthBarSliderActiveOnStart");
		setHealthBarAsNotVisibleAtStart = serializedObject.FindProperty ("setHealthBarAsNotVisibleAtStart");
		notHuman = serializedObject.FindProperty ("advancedSettings.notHuman");
		useWeakSpots = serializedObject.FindProperty ("advancedSettings.useWeakSpots");
		showWeakSpotsInScannerMode = serializedObject.FindProperty ("showWeakSpotsInScannerMode");
		useHealthAmountOnSpotEnabled = serializedObject.FindProperty ("advancedSettings.useHealthAmountOnSpotEnabled");
		scorchMarkPrefab = serializedObject.FindProperty ("scorchMarkPrefab");
		layer = serializedObject.FindProperty ("settings.layer");
		weakSpotMesh = serializedObject.FindProperty ("weakSpotMesh");
		weakSpotMeshAlphaValue = serializedObject.FindProperty ("weakSpotMeshAlphaValue");
		haveRagdoll = serializedObject.FindProperty ("advancedSettings.haveRagdoll");
		activateRagdollOnDamageReceived = serializedObject.FindProperty ("advancedSettings.activateRagdollOnDamageReceived");
		minDamageToEnableRagdoll = serializedObject.FindProperty ("advancedSettings.minDamageToEnableRagdoll");
		ragdollEvent = serializedObject.FindProperty ("advancedSettings.ragdollEvent");
		allowPushCharacterOnExplosions = serializedObject.FindProperty ("advancedSettings.allowPushCharacterOnExplosions");
		ragdollCanReceiveDamageOnImpact = serializedObject.FindProperty ("advancedSettings.ragdollCanReceiveDamageOnImpact");
		receiveDamageEvenDead = serializedObject.FindProperty ("receiveDamageEvenDead");
		canBeSedated = serializedObject.FindProperty ("canBeSedated");
		awakeOnDamageIfSedated = serializedObject.FindProperty ("awakeOnDamageIfSedated");
		useEventOnSedate = serializedObject.FindProperty ("useEventOnSedate");
		sedateStartEvent = serializedObject.FindProperty ("sedateStartEvent");
		sedateEndEvent = serializedObject.FindProperty ("sedateEndEvent");

		receiveDamageFromCollisionsEnabled = serializedObject.FindProperty ("receiveDamageFromCollisionsEnabled");

		minTimeToReceiveDamageOnImpact = serializedObject.FindProperty ("minTimeToReceiveDamageOnImpact");

		minVelocityToReceiveDamageOnImpact = serializedObject.FindProperty ("minVelocityToReceiveDamageOnImpact");
	
		receiveDamageOnImpactMultiplier = serializedObject.FindProperty ("receiveDamageOnImpactMultiplier");

		showGizmo = serializedObject.FindProperty ("advancedSettings.showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("advancedSettings.gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("advancedSettings.gizmoRadius");
		alphaColor = serializedObject.FindProperty ("advancedSettings.alphaColor");

		labelTextSize = serializedObject.FindProperty ("advancedSettings.labelTextSize");

		weakSpots = serializedObject.FindProperty ("advancedSettings.weakSpots");
		addDamageReceiversToCustomTransformList = serializedObject.FindProperty ("addDamageReceiversToCustomTransformList");
		customTransformListDamageReceiver = serializedObject.FindProperty ("customTransformListDamageReceiver");

		useEventOnHealthValueList = serializedObject.FindProperty ("useEventOnHealthValueList");
		eventOnHealthValueList = serializedObject.FindProperty ("eventOnHealthValueList");

		useDamageTypeCheck = serializedObject.FindProperty ("useDamageTypeCheck");
		damageTypeInfoList = serializedObject.FindProperty ("damageTypeInfoList");

		showDamageReceivedDebugInfo = serializedObject.FindProperty ("showDamageReceivedDebugInfo");
		healthAmountToTakeOnEditor = serializedObject.FindProperty ("healthAmountToTakeOnEditor");
		healthAmountToGiveOnEditor = serializedObject.FindProperty ("healthAmountToGiveOnEditor");

		blockDamageActive = serializedObject.FindProperty ("blockDamageActive");
		blockDamageProtectionAmount = serializedObject.FindProperty ("blockDamageProtectionAmount");
		useEventsOnDamageBlocked = serializedObject.FindProperty ("useEventsOnDamageBlocked");
		eventOnDamageBlocked = serializedObject.FindProperty ("eventOnDamageBlocked");

		useDamageHitReaction = serializedObject.FindProperty ("useDamageHitReaction");
		mainDamageHitReactionSystem = serializedObject.FindProperty ("mainDamageHitReactionSystem");

		debugDamageSourceTransform = serializedObject.FindProperty ("debugDamageSourceTransform");

		objectIsCharacter = serializedObject.FindProperty ("objectIsCharacter");

		useEventsOnInvincibleStateChange = serializedObject.FindProperty ("useEventsOnInvincibleStateChange");
		eventOnInvicibleOn = serializedObject.FindProperty ("eventOnInvicibleOn");
		eventOnInvicibleOff = serializedObject.FindProperty ("eventOnInvicibleOff");

		eventOnDamageReceivedWithTemporalInvincibility = serializedObject.FindProperty ("eventOnDamageReceivedWithTemporalInvincibility");

		maxDelayBetweenDamageReceivedAndInvincibilityStateActive = serializedObject.FindProperty ("maxDelayBetweenDamageReceivedAndInvincibilityStateActive");
	
		useEventOnDamageReceivedWithTemporalInvincibility = serializedObject.FindProperty ("useEventOnDamageReceivedWithTemporalInvincibility");


		sendInfoToCharacterCustomizationOnDamageEnabled = serializedObject.FindProperty ("sendInfoToCharacterCustomizationOnDamageEnabled");

		mainManagerName = serializedObject.FindProperty ("mainManagerName");

		healthManager = (health)target;
	}

	void OnSceneGUI ()
	{   
		if (healthManager.advancedSettings.showGizmo) {
			gizmoStyle.normal.textColor = healthManager.advancedSettings.gizmoLabelColor;
			gizmoStyle.alignment = TextAnchor.MiddleCenter;

			gizmoStyle.fontSize = healthManager.advancedSettings.labelTextSize;

			for (int i = 0; i < healthManager.advancedSettings.weakSpots.Count; i++) {
				if (healthManager.advancedSettings.weakSpots [i].spotTransform != null) {
					label = healthManager.advancedSettings.weakSpots [i].name;

					if (healthManager.advancedSettings.weakSpots [i].killedWithOneShoot) {
						if (healthManager.advancedSettings.weakSpots [i].needMinValueToBeKilled) {
							label += "\nOne Shoot\n >=" + healthManager.advancedSettings.weakSpots [i].minValueToBeKilled;
						} else {
							label += "\nOne Shoot";	
						}
					} else {
						label += "\nx" + healthManager.advancedSettings.weakSpots [i].damageMultiplier;
					}

					if (healthManager.advancedSettings.weakSpots [i].useHealthAmountOnSpot) {
						label += "\n" + healthManager.advancedSettings.weakSpots [i].healhtAmountOnSpot;
					}

					Handles.Label (healthManager.advancedSettings.weakSpots [i].spotTransform.position, label, gizmoStyle);	
				}
			}

			if (healthManager.settings.enemyHealthSlider != null) {
				gizmoStyle.normal.textColor = healthManager.advancedSettings.gizmoLabelColor;
				gizmoStyle.alignment = TextAnchor.MiddleCenter;
				Handles.Label (healthManager.transform.position + healthManager.settings.sliderOffset, "Health Slider", gizmoStyle);	
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		listButtonStyle = new GUIStyle (GUI.skin.button);

		listButtonStyle.fontStyle = FontStyle.Bold;
		listButtonStyle.fontSize = 12;

		style.fontStyle = FontStyle.Bold;

		GUILayout.BeginVertical ("Main Health Settings", "window");
		EditorGUILayout.PropertyField (invincible);

		if (!invincible.boolValue) {
			EditorGUILayout.PropertyField (healthAmount);
			EditorGUILayout.PropertyField (maxHealthAmount);

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Damage Multiplier Settings", "window");
			EditorGUILayout.PropertyField (generalDamageMultiplerEnabled);
			if (generalDamageMultiplerEnabled.boolValue) {
				EditorGUILayout.PropertyField (generalDamageMultiplerActive);
				EditorGUILayout.PropertyField (generalDamageMultiplier);
			}
			GUILayout.EndVertical ();
	
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Regenerate Health Settings", "window");
			EditorGUILayout.PropertyField (regenerateHealth);
			if (regenerateHealth.boolValue) {
				EditorGUILayout.PropertyField (constantRegenerate);
				EditorGUILayout.PropertyField (regenerateTime);
				if (constantRegenerate.boolValue) {
					EditorGUILayout.PropertyField (regenerateSpeed);
				} else {
					EditorGUILayout.PropertyField (regenerateAmount);
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Health State", "window");
			GUILayout.Label ("Max Health\t\t " + maxHealthAmount.floatValue);
			GUILayout.Label ("Health ID\t\t " + currentID.intValue);
			isDead = "NO";
			if (Application.isPlaying) {
				if (dead.boolValue) {
					isDead = "YES";
				} 
			} 
			GUILayout.Label ("Is Dead\t\t " + isDead);

			sedateActiveText = "NO";
			if (Application.isPlaying) {
				if (sedateActive.boolValue) {
					sedateActiveText = "YES";
				} 
			} 
			GUILayout.Label ("Is Sedated\t\t " + sedateActiveText);
			GUILayout.Label ("Sedate Duration\t " + currentSedateDuration.floatValue);

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Shield Settings", "window");
		EditorGUILayout.PropertyField (useShield);

		if (useShield.boolValue) {
			EditorGUILayout.PropertyField (mainShieldSystem);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Health Stat Settings", "window");
		EditorGUILayout.PropertyField (playerStatsManager);
		EditorGUILayout.PropertyField (healthStatName);
		EditorGUILayout.PropertyField (maxHealthStatName);
		GUILayout.EndVertical (); 

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Damage Hit Reaction Settings", "window");
		EditorGUILayout.PropertyField (useDamageHitReaction);
		EditorGUILayout.PropertyField (mainDamageHitReactionSystem);
		GUILayout.EndVertical (); 

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Block Damage Settings", "window");
		GUILayout.Label ("Block Damage Active\t\t " + blockDamageActive.boolValue);
		GUILayout.Label ("Block Damage Amount\t " + blockDamageProtectionAmount.floatValue);

		EditorGUILayout.PropertyField (useEventsOnDamageBlocked);
		if (useEventsOnDamageBlocked.boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnDamageBlocked);
		}
		GUILayout.EndVertical (); 

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Others Settings", "window");
		EditorGUILayout.PropertyField (objectIsCharacter);
		EditorGUILayout.PropertyField (placeToShootActive);
		if (placeToShootActive.boolValue) {
			EditorGUILayout.PropertyField (placeToShoot);
		}
		EditorGUILayout.PropertyField (checkDamageReceiverOnChildrenTransform);
		EditorGUILayout.PropertyField (damagePrefab);
		EditorGUILayout.PropertyField (mainManagerName);
		EditorGUILayout.PropertyField (sendInfoToCharacterCustomizationOnDamageEnabled);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Impact Surface Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (useImpactSurface);
		EditorGUILayout.PropertyField (mainDecalManagerName);

		EditorGUILayout.Space ();

		if (useImpactSurface.boolValue) {
			if (impactDecalList.arraySize > 0) {
				impactDecalIndex.intValue = EditorGUILayout.Popup ("Decal Impact Type", 
					impactDecalIndex.intValue, healthManager.impactDecalList);
				impactDecalName.stringValue = healthManager.impactDecalList [impactDecalIndex.intValue];
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Decal Impact List")) {
				healthManager.getImpactListInfo ();					
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Damage Receiver Settings", "window");
		if (GUILayout.Button ("Update Character Damage Receives")) {
			healthManager.updateCharacterDamageReceiverOnObject ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Damage Receiver To Object")) {
			healthManager.addDamageReceiverToObject ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showDamageDeadSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
			
		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 16;

		if (GUILayout.Button ("Show Damage/Death/Events Settings", buttonStyle)) {
			showDamageDeadSettings.boolValue = !showDamageDeadSettings.boolValue;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		if (showDamageDeadSettings.boolValue) {

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 30;
			style.alignment = TextAnchor.MiddleCenter;

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("EVENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Damage Event Settings", "window");

			EditorGUILayout.PropertyField (useEventOnDamageEnabled);

			EditorGUILayout.Space ();

			if (useEventOnDamageEnabled.boolValue) {
				EditorGUILayout.PropertyField (eventOnDamage);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useEventOnDamageWithAttacker);
			if (useEventOnDamageWithAttacker.boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventOnDamageWithAttacker);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useEventOnDamageWithAmount);
			if (useEventOnDamageWithAmount.boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventOnDamageWithAmount);
			}
					
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Damage Extra Event Settings", "window");
			EditorGUILayout.PropertyField (useExtraDamageFunctions);
			if (useExtraDamageFunctions.boolValue) {

				EditorGUILayout.Space ();

				getExtraDamageFunctionList (extraDamageFunctionList);
			}
			GUILayout.EndVertical ();
			GUILayout.EndVertical ();

			if (useShield.boolValue) {
				EditorGUILayout.Space ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Shield Damage Event Settings", "window");
				EditorGUILayout.PropertyField (useEventOnDamageShield);
				if (useEventOnDamageShield.boolValue) {

					EditorGUILayout.Space ();

					EditorGUILayout.PropertyField (eventOnDamageShield);

					EditorGUILayout.Space ();

					EditorGUILayout.PropertyField (useEventOnDamageShieldWithAttacker);

					if (useEventOnDamageShieldWithAttacker.boolValue) {

						EditorGUILayout.Space ();
						EditorGUILayout.PropertyField (eventOnDamageShieldWithAttacker);
					}
				}
				GUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Death Event Settings", "window");
			EditorGUILayout.PropertyField (deadFuncionCall);
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Death Extra Event Settings", "window");
			EditorGUILayout.PropertyField (useExtraDeadFunctions);

			EditorGUILayout.Space ();

			if (useExtraDeadFunctions.boolValue) {
				EditorGUILayout.PropertyField (delayInExtraDeadFunctions);

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (extraDeadFunctionCall);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Event With Attacker On Death Settings", "window");
			EditorGUILayout.PropertyField (useEventWithAttackerOnDeath);
			if (useEventWithAttackerOnDeath.boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventWithAttackerOnDeath);
			}
			GUILayout.EndVertical ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Resurrect Event Settings", "window");
			EditorGUILayout.PropertyField (resurrectFunctionCall);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Resurrect After Delay Event Settings", "window");
			EditorGUILayout.PropertyField (resurrectAfterDelayEnabled);
			if (resurrectAfterDelayEnabled.boolValue) {
				EditorGUILayout.PropertyField (resurrectDelay);

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventToResurrectAfterDelay);
			}
			GUILayout.EndVertical (); 

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Invincible State Change Settings", "window");
			EditorGUILayout.PropertyField (useEventsOnInvincibleStateChange);
			if (useEventsOnInvincibleStateChange.boolValue) {
				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventOnInvicibleOn);

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventOnInvicibleOff);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useEventOnDamageReceivedWithTemporalInvincibility);
			if (useEventOnDamageReceivedWithTemporalInvincibility.boolValue) {
				EditorGUILayout.PropertyField (maxDelayBetweenDamageReceivedAndInvincibilityStateActive);

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventOnDamageReceivedWithTemporalInvincibility);
			}
			GUILayout.EndVertical (); 

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Events To Send Stats On Save", "window");
			EditorGUILayout.PropertyField (eventToSendCurrentHealthAmount);
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 16;

		EditorGUILayout.BeginHorizontal ();
		if (showSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Settings", buttonStyle)) {
			showSettings.boolValue = !showSettings.boolValue;
		}
		if (showAdvancedSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Advanced Settings", buttonStyle)) {
			showAdvancedSettings.boolValue = !showAdvancedSettings.boolValue;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		if (showSettings.boolValue) {

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 30;
			style.alignment = TextAnchor.MiddleCenter;

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Player Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("HUD Health Slider Settings", "window");
			EditorGUILayout.PropertyField (healthSlider);

			if (healthSlider.objectReferenceValue) {
				EditorGUILayout.PropertyField (healthSliderText);

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (hideHealthSliderWhenNotDamageReceived);
				if (hideHealthSliderWhenNotDamageReceived.boolValue) {
					EditorGUILayout.PropertyField (timeToHideHealthSliderAfterDamage);
					EditorGUILayout.PropertyField (mainHealthSliderParent);
					EditorGUILayout.PropertyField (hiddenHealthSliderParent);
					EditorGUILayout.PropertyField (mainHealthSliderTransform);
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Health Slider Icon on HUD Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Health Slider Settings", "window");
			EditorGUILayout.PropertyField (useHealthSlider);
			if (useHealthSlider.boolValue) {
				EditorGUILayout.PropertyField (enemyHealthSlider, new GUIContent ("Health Slider Prefab"));
				EditorGUILayout.PropertyField (sliderOffset);
				EditorGUILayout.PropertyField (allyName);
				EditorGUILayout.PropertyField (allySliderColor);

				EditorGUILayout.PropertyField (enemyName);
				EditorGUILayout.PropertyField (enemySliderColor);

				EditorGUILayout.PropertyField (nameTextColor);
				
				EditorGUILayout.PropertyField (enemyTag);
				EditorGUILayout.PropertyField (friendTag);

				EditorGUILayout.PropertyField (removeHealthBarSliderOnDeath);
				EditorGUILayout.PropertyField (healthBarSliderActiveOnStart);
				EditorGUILayout.PropertyField (setHealthBarAsNotVisibleAtStart);

				EditorGUILayout.PropertyField (useHealthSlideInfoOnScreen);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Components", "window");
			EditorGUILayout.PropertyField (mainDamageReceiver);
			EditorGUILayout.PropertyField (playerControllerManager);
			EditorGUILayout.PropertyField (ragdollManager);
			EditorGUILayout.PropertyField (damageInScreenManager);
			EditorGUILayout.PropertyField (debugDamageSourceTransform);
			EditorGUILayout.PropertyField (mainInventoryCharacterCustomizationSystem);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showAdvancedSettings.boolValue) {

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 30;
			style.alignment = TextAnchor.MiddleCenter;

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("ADVANCED SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Ragdoll and weak spots Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Advanced Settings", "window");
			EditorGUILayout.PropertyField (notHuman);

			EditorGUILayout.PropertyField (showWeakSpotsInScannerMode);

			EditorGUILayout.PropertyField (useHealthAmountOnSpotEnabled);

			EditorGUILayout.PropertyField (scorchMarkPrefab);
			if (scorchMarkPrefab.objectReferenceValue) {
				EditorGUILayout.PropertyField (layer);
			}

			if (showWeakSpotsInScannerMode.boolValue) {
				EditorGUILayout.PropertyField (weakSpotMesh);
				EditorGUILayout.PropertyField (weakSpotMeshAlphaValue);
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Damage On Collision Settings", "window");

			EditorGUILayout.PropertyField (receiveDamageFromCollisionsEnabled);

			if (receiveDamageFromCollisionsEnabled.boolValue) {
				EditorGUILayout.PropertyField (minTimeToReceiveDamageOnImpact);
				EditorGUILayout.PropertyField (minVelocityToReceiveDamageOnImpact);
				EditorGUILayout.PropertyField (receiveDamageOnImpactMultiplier);
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Ragdoll Settings", "window");
			EditorGUILayout.PropertyField (haveRagdoll);
			if (haveRagdoll.boolValue) {
				EditorGUILayout.PropertyField (activateRagdollOnDamageReceived);
				if (activateRagdollOnDamageReceived.boolValue) {
					EditorGUILayout.PropertyField (minDamageToEnableRagdoll);
					EditorGUILayout.PropertyField (ragdollEvent);

					EditorGUILayout.Space ();
				}

				EditorGUILayout.PropertyField (allowPushCharacterOnExplosions);

				EditorGUILayout.PropertyField (ragdollCanReceiveDamageOnImpact);

				EditorGUILayout.PropertyField (receiveDamageEvenDead);

				EditorGUILayout.PropertyField (canBeSedated);
				if (canBeSedated.boolValue) {
					EditorGUILayout.PropertyField (awakeOnDamageIfSedated);

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Ragdoll Settings", "window");
					EditorGUILayout.PropertyField (useEventOnSedate);
					if (useEventOnSedate.boolValue) {
						EditorGUILayout.PropertyField (sedateStartEvent);
						EditorGUILayout.PropertyField (sedateEndEvent);
					}
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();
				}

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Damage Receivers Settings", "window");
				if (GUILayout.Button ("Add Damage Receivers To Ragdoll")) {
					healthManager.addDamageReceiversToRagdoll ();
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Remove Damage Receivers From Ragdoll")) {
					healthManager.removeDamageReceiversFromRagdoll ();
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window");
			EditorGUILayout.PropertyField (showGizmo);
			if (showGizmo.boolValue) {
				EditorGUILayout.PropertyField (gizmoLabelColor);
				EditorGUILayout.PropertyField (gizmoRadius);
				EditorGUILayout.PropertyField (alphaColor);
				EditorGUILayout.PropertyField (labelTextSize);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 20;
			style.alignment = TextAnchor.MiddleCenter;

			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("Weak Spots Settings", style);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set Humanoid Weak Spots")) {
				healthManager.setHumanoidWeaKSpots ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Reset Weak Spots Multipliers")) {
				healthManager.resetWeakSpotDamageMultipliers ();
			}
				
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weak Spots List", "window");
			EditorGUILayout.PropertyField (useWeakSpots);

			if (useWeakSpots.boolValue) {
				EditorGUILayout.Space ();

				showWeakSpotsList (weakSpots);
			}
			
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (addDamageReceiversToCustomTransformList);
			if (addDamageReceiversToCustomTransformList.boolValue) {
				
				EditorGUILayout.Space ();

				showSimpleList (customTransformListDamageReceiver);

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Add Custom Transform List Damage Receivers")) {
					healthManager.setCustomTransformListDamageReceiver ();
				}
			}			

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Damage Receivers")) {
				healthManager.updateDamageReceivers ();
			}

			EditorGUILayout.Space ();

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 20;
			style.alignment = TextAnchor.MiddleCenter;

			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("Health Value Events", style);

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Events On Health Value Settings", "window");
			EditorGUILayout.PropertyField (useEventOnHealthValueList);
			if (useEventOnHealthValueList.boolValue) {
				
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Events On Health List", "window");
				showEventOnHealthValueList (eventOnHealthValueList);
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 20;
			style.alignment = TextAnchor.MiddleCenter;

			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("Damage Type Settings", style);

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Damage Type ID Settings", "window");
			EditorGUILayout.PropertyField (useDamageTypeCheck);
			if (useDamageTypeCheck.boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Damage Type Info List", "window");
				showDamageTypeInfoList (damageTypeInfoList);
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 20;
		style.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.Space ();

		EditorGUILayout.LabelField ("DEBUG INFO", style);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Options", "window");
		EditorGUILayout.PropertyField (showDamageReceivedDebugInfo);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Kill Character (In-Game Only)")) {
			if (Application.isPlaying) {
				healthManager.killByButton ();
			}
		}

		if (haveRagdoll.boolValue) {

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Push Character (In-Game Only)")) {
				if (Application.isPlaying) {
					healthManager.pushFullCharacter ();
				}
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Activate Ragdoll (In-Game Only)")) {
				if (Application.isPlaying) {
					healthManager.pushCharacterWithoutForce ();
				}
			}
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (healthAmountToTakeOnEditor);
		if (GUILayout.Button ("Apply X Damage (In-Game Only)")) {
			if (Application.isPlaying) {
				healthManager.takeHealth (healthAmountToTakeOnEditor.floatValue);
			}
		}

		EditorGUILayout.Space ();
		if (GUILayout.Button ("Apply X Damage With Direction (In-Game Only)")) {
			if (Application.isPlaying) {
				healthManager.takeDamageFromDebugDamageSourceTransform (healthAmountToTakeOnEditor.floatValue);
			}
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (healthAmountToGiveOnEditor);
		if (GUILayout.Button ("Apply X Health (In-Game Only)")) {
			if (Application.isPlaying) {
				healthManager.getHealth (healthAmountToGiveOnEditor.floatValue);
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUI.backgroundColor = defBackgroundColor;

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showListElementInfo (SerializedProperty list, int elementIndex)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("spotTransform"));
		if (!list.FindPropertyRelative ("killedWithOneShoot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageMultiplier"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("killedWithOneShoot"));
		if (list.FindPropertyRelative ("killedWithOneShoot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("needMinValueToBeKilled"));
			if (list.FindPropertyRelative ("needMinValueToBeKilled").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("minValueToBeKilled"));
			}
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("transformToAttachWeapons"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBoneTransformToAttachWeapons"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Armor Cloth Damage Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendValueToArmorClothSystemOnDamage"));
		if (list.FindPropertyRelative ("sendValueToArmorClothSystemOnDamage").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("armorClothCategoryName"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageMultiplierOnArmorClothPiece"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event On Damage Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendFunctionWhenDamage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendFunctionWhenDie"));
		if (list.FindPropertyRelative ("sendFunctionWhenDamage").boolValue || list.FindPropertyRelative ("sendFunctionWhenDie").boolValue) {
			
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageFunction"));
		}
		GUILayout.EndVertical ();

		if (useHealthAmountOnSpotEnabled.boolValue) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Health Amount On Spot Settings", "window", GUILayout.Height (30));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHealthAmountOnSpot"));
			if (list.FindPropertyRelative ("useHealthAmountOnSpot").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("healhtAmountOnSpot"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnEmtpyHealthAmountOnSpot"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("killCharacterOnEmtpyHealthAmountOnSpot"));

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Set Full Damage Spot")) {
					if (Application.isPlaying) {
						healthManager.damageSpot (elementIndex, list.FindPropertyRelative ("healhtAmountOnSpot").floatValue);
					}
				}
			}
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Critical Damage Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCriticalDamageSpot"));
		if (list.FindPropertyRelative ("useCriticalDamageSpot").boolValue) {

			EditorGUILayout.Space ();

			GUILayout.Label (new GUIContent ("Critical Damage Probability"), EditorStyles.boldLabel);
			GUILayout.BeginHorizontal ();

			Vector2 criticalDamageProbability = list.FindPropertyRelative ("criticalDamageProbability").vector2Value;
			criticalDamageProbability.x = EditorGUILayout.FloatField (criticalDamageProbability.x, GUILayout.MaxWidth (50));
			EditorGUILayout.MinMaxSlider (ref criticalDamageProbability.x, ref criticalDamageProbability.y, 0, 100);
			criticalDamageProbability.y = EditorGUILayout.FloatField (criticalDamageProbability.y, GUILayout.MaxWidth (50));
			list.FindPropertyRelative ("criticalDamageProbability").vector2Value = criticalDamageProbability;

			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageMultiplierOnCritical"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("killTargetOnCritical"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("removeAllHealthAmountOnSpotOnCritical"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showWeakSpotsList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
	
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Weak Spots List", listButtonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ();
			GUILayout.Label ("Number Of Spots: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Spot")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();

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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showListElementInfo (list.GetArrayElementAtIndex (i), i);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void getExtraDamageFunctionList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Extra Damage Function List", listButtonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Functions: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Function")) {
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
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showFunctionInfo (list.GetArrayElementAtIndex (i));
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
	}

	void showFunctionInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageRecived"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageFunctionCall"));
		GUILayout.EndVertical ();
	}

	void showEventOnHealthValueList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Event On Health Value List", listButtonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Events: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Function")) {
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
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showEventOnHealthValueListElement (list.GetArrayElementAtIndex (i));
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
	}

	void showEventOnHealthValueListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDamagePercentage"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("minDamageToReceive"));


		EditorGUILayout.PropertyField (list.FindPropertyRelative ("callEventOnce"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnDamageReceived"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCall"));
		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Custom Transform List Damage Receiver", listButtonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Amount: \t" + list.arraySize);

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
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}

	void showDamageTypeInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Damage Type Info List", listButtonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Damage Type: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Damage Type")) {
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
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showDamageTypeInfoListElement (list.GetArrayElementAtIndex (i));
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
	}

	void showDamageTypeInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageTypeEnabled"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageTypeID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageTypeResistance"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("avoidDamageTypeIfBlockDamageActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableDamageReactionOnDamageType"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("obtainHealthOnDamageType"));
		if (list.FindPropertyRelative ("obtainHealthOnDamageType").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("healthMultiplierOnDamageType"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopDamageCheckIfHealthObtained"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnObtainHealthOnDamageType"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnObtainHealthOnDamageType"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnDamageType"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnDamageType"));
		GUILayout.EndVertical ();
	}
}
#endif