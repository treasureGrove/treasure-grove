using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(otherPowers))]
public class otherPowersEditor : Editor
{
	SerializedProperty choosedPower;
	SerializedProperty initialPowerIndex;
	SerializedProperty currentPowerName;
	SerializedProperty amountPowersEnabled;

	SerializedProperty usingFreeFireMode;
	SerializedProperty actionActive;
	SerializedProperty aimingInThirdPerson;
	SerializedProperty aimingInFirstPerson;
	SerializedProperty usingPowers;
	SerializedProperty shooting;
	SerializedProperty powersModeActive;
	SerializedProperty showSettings;
	SerializedProperty showAimSettings;
	SerializedProperty showShootSettings;

	SerializedProperty aimModeEnabled;
	SerializedProperty shootEnabled;
	SerializedProperty changePowersEnabled;

	SerializedProperty layerToDamage;

	SerializedProperty useCustomIgnoreTags;
	SerializedProperty customTagsToIgnoreList;

	SerializedProperty changePowersWithNumberKeysActive;
	SerializedProperty changePowersWithMouseWheelActive;
	SerializedProperty changePowersWithKeysActive;
	SerializedProperty playerStatsManager;
	SerializedProperty energyStatName;
	SerializedProperty canFirePowersWithoutAiming;
	SerializedProperty useAimCameraOnFreeFireMode;
	SerializedProperty timeToStopAimAfterStopFiring;
	SerializedProperty useLowerRotationSpeedAimedThirdPerson;
	SerializedProperty verticalRotationSpeedAimedInThirdPerson;
	SerializedProperty horizontalRotationSpeedAimedInThirdPerson;
	SerializedProperty runWhenAimingPowerInThirdPerson;
	SerializedProperty stopRunIfPreviouslyNotRunning;
	SerializedProperty cursor;
	SerializedProperty regularReticle;
	SerializedProperty customReticle;
	SerializedProperty customReticleImage;

	SerializedProperty powerBar;
	SerializedProperty powerBarText;
	SerializedProperty powersInfoPanel;
	SerializedProperty usePowerRotationPoint;
	SerializedProperty powerRotationPoint;
	SerializedProperty rotationUpPointAmountMultiplier;
	SerializedProperty rotationDownPointAmountMultiplier;
	SerializedProperty rotationPointSpeed;
	SerializedProperty useRotationUpClamp;
	SerializedProperty rotationUpClampAmount;
	SerializedProperty useRotationDownClamp;
	SerializedProperty rotationDownClamp;

	SerializedProperty useEventsOnStateChange;
	SerializedProperty evenOnStateEnabled;
	SerializedProperty eventOnStateDisabled;
	SerializedProperty showElements;
	SerializedProperty shootZoneAudioSource;
	SerializedProperty mainCameraTransform;
	SerializedProperty mainCamera;
	SerializedProperty upperBodyRotationManager;
	SerializedProperty IKSystemManager;
	SerializedProperty playerCameraManager;
	SerializedProperty headBobManager;
	SerializedProperty weaponsManager;

	SerializedProperty playerControllerManager;
	SerializedProperty grabObjectsManager;

	SerializedProperty headTrackManager;
	SerializedProperty impactDecalManager;
	SerializedProperty playerInput;
	SerializedProperty powersManager;
	SerializedProperty playerCollider;
	SerializedProperty cursorRectTransform;
	SerializedProperty parable;
	SerializedProperty leftHand;
	SerializedProperty rightHand;
	SerializedProperty powersList;
	SerializedProperty headLookWhenAiming;
	SerializedProperty headLookSpeed;
	SerializedProperty headLookTarget;
	SerializedProperty autoShootOnTag;
	SerializedProperty layerToAutoShoot;
	SerializedProperty maxDistanceToRaycast;
	SerializedProperty shootAtLayerToo;
	SerializedProperty autoShootTagList;
	SerializedProperty useAimAssistInThirdPerson;
	SerializedProperty useMaxDistanceToCameraCenterAimAssist;
	SerializedProperty maxDistanceToCameraCenterAimAssist;
	SerializedProperty useAimAssistInLockedCamera;
	SerializedProperty infinitePower;

	SerializedProperty powerAmount;
	SerializedProperty maxPowerAmount;

	SerializedProperty regeneratePower;
	SerializedProperty constantRegenerate;
	SerializedProperty regenerateTime;
	SerializedProperty regenerateSpeed;
	SerializedProperty regenerateAmount;
	SerializedProperty targetToDamageLayer;
	SerializedProperty targetForScorchLayer;
	SerializedProperty powersSlotsAmount;
	SerializedProperty selectedPowerIcon;
	SerializedProperty selectedPowerHud;
	SerializedProperty shootZone;
	SerializedProperty firstPersonShootPosition;
	SerializedProperty minSwipeDist;
	SerializedProperty usedByAI;
	SerializedProperty impactDecalList;

	SerializedProperty mainDecalManagerName;

	SerializedProperty projectilesPoolEnabled;

	SerializedProperty maxAmountOfPoolElementsOnWeapon;

	SerializedProperty currentArrayElement;

	SerializedProperty useFreeAimMode;
	SerializedProperty freeAimModeeActive;
	SerializedProperty armsPivotRotationTransform;
	SerializedProperty armsPivotRotationSpeed;
	SerializedProperty armsPivotClampRotation;
	SerializedProperty armsPivotRotationMultiplier;

	SerializedProperty freeAimModLookInOppositeDirectionExtraRange;
	SerializedProperty freeAimModePlayerStatusID;

	bool applicationIsPlaying;

	int listarraySize;

	otherPowers _powersManager;
	Color buttonColor;
	bool powerEnabled;
	string powerEnabledString;
	string powerName;
	bool expanded;

	string currentButtonString;

	string aimingInThirdPersonText;
	string aimingInFirstPersonText;
	string usingPowersText;
	string shootingWeapon;
	string powersModeActiveText;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		choosedPower = serializedObject.FindProperty ("choosedPower");
		initialPowerIndex = serializedObject.FindProperty ("initialPowerIndex");

		currentPowerName = serializedObject.FindProperty ("currentPowerName");
		amountPowersEnabled = serializedObject.FindProperty ("amountPowersEnabled");

		usingFreeFireMode = serializedObject.FindProperty ("usingFreeFireMode");
		actionActive = serializedObject.FindProperty ("actionActive");
		aimingInThirdPerson = serializedObject.FindProperty ("aimingInThirdPerson");
		aimingInFirstPerson = serializedObject.FindProperty ("aimingInFirstPerson");
		usingPowers = serializedObject.FindProperty ("usingPowers");
		shooting = serializedObject.FindProperty ("shooting");
		powersModeActive = serializedObject.FindProperty ("powersModeActive");
		showSettings = serializedObject.FindProperty ("showSettings");
		showAimSettings = serializedObject.FindProperty ("showAimSettings");
		showShootSettings = serializedObject.FindProperty ("showShootSettings");
	
		aimModeEnabled = serializedObject.FindProperty ("settings.aimModeEnabled");
		shootEnabled = serializedObject.FindProperty ("settings.shootEnabled");
		changePowersEnabled = serializedObject.FindProperty ("settings.changePowersEnabled");

		layerToDamage = serializedObject.FindProperty ("layerToDamage");

		useCustomIgnoreTags = serializedObject.FindProperty ("shootsettings.useCustomIgnoreTags");
		customTagsToIgnoreList = serializedObject.FindProperty ("shootsettings.customTagsToIgnoreList");

		changePowersWithNumberKeysActive = serializedObject.FindProperty ("changePowersWithNumberKeysActive");
		changePowersWithMouseWheelActive = serializedObject.FindProperty ("changePowersWithMouseWheelActive");
		changePowersWithKeysActive = serializedObject.FindProperty ("changePowersWithKeysActive");
		playerStatsManager = serializedObject.FindProperty ("playerStatsManager");
		energyStatName = serializedObject.FindProperty ("energyStatName");
		canFirePowersWithoutAiming = serializedObject.FindProperty ("canFirePowersWithoutAiming");
		useAimCameraOnFreeFireMode = serializedObject.FindProperty ("useAimCameraOnFreeFireMode");
		timeToStopAimAfterStopFiring = serializedObject.FindProperty ("timeToStopAimAfterStopFiring");
		useLowerRotationSpeedAimedThirdPerson = serializedObject.FindProperty ("useLowerRotationSpeedAimedThirdPerson");
		verticalRotationSpeedAimedInThirdPerson = serializedObject.FindProperty ("verticalRotationSpeedAimedInThirdPerson");
		horizontalRotationSpeedAimedInThirdPerson = serializedObject.FindProperty ("horizontalRotationSpeedAimedInThirdPerson");
		runWhenAimingPowerInThirdPerson = serializedObject.FindProperty ("runWhenAimingPowerInThirdPerson");
		stopRunIfPreviouslyNotRunning = serializedObject.FindProperty ("stopRunIfPreviouslyNotRunning");
		cursor = serializedObject.FindProperty ("settings.cursor");
		regularReticle = serializedObject.FindProperty ("settings.regularReticle");
		customReticle = serializedObject.FindProperty ("settings.customReticle");
		customReticleImage = serializedObject.FindProperty ("settings.customReticleImage");

		powerBar = serializedObject.FindProperty ("settings.powerBar");
		powerBarText = serializedObject.FindProperty ("settings.powerBarText");
		powersInfoPanel = serializedObject.FindProperty ("powersInfoPanel");
		usePowerRotationPoint = serializedObject.FindProperty ("usePowerRotationPoint");
		powerRotationPoint = serializedObject.FindProperty ("powerRotationPoint");
		rotationUpPointAmountMultiplier = serializedObject.FindProperty ("rotationPointInfo.rotationUpPointAmountMultiplier");
		rotationDownPointAmountMultiplier = serializedObject.FindProperty ("rotationPointInfo.rotationDownPointAmountMultiplier");
		rotationPointSpeed = serializedObject.FindProperty ("rotationPointInfo.rotationPointSpeed");
		useRotationUpClamp = serializedObject.FindProperty ("rotationPointInfo.useRotationUpClamp");
		rotationUpClampAmount = serializedObject.FindProperty ("rotationPointInfo.rotationUpClampAmount");
		useRotationDownClamp = serializedObject.FindProperty ("rotationPointInfo.useRotationDownClamp");
		rotationDownClamp = serializedObject.FindProperty ("rotationPointInfo.rotationDownClamp");

		useEventsOnStateChange = serializedObject.FindProperty ("useEventsOnStateChange");
		evenOnStateEnabled = serializedObject.FindProperty ("evenOnStateEnabled");
		eventOnStateDisabled = serializedObject.FindProperty ("eventOnStateDisabled");
		showElements = serializedObject.FindProperty ("showElements");
		shootZoneAudioSource = serializedObject.FindProperty ("shootZoneAudioSource");
		mainCameraTransform = serializedObject.FindProperty ("mainCameraTransform");
		mainCamera = serializedObject.FindProperty ("mainCamera");
		upperBodyRotationManager = serializedObject.FindProperty ("upperBodyRotationManager");
		IKSystemManager = serializedObject.FindProperty ("IKSystemManager");
		playerCameraManager = serializedObject.FindProperty ("playerCameraManager");

		headBobManager = serializedObject.FindProperty ("headBobManager");
		weaponsManager = serializedObject.FindProperty ("weaponsManager");

		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		grabObjectsManager = serializedObject.FindProperty ("grabObjectsManager");

		headTrackManager = serializedObject.FindProperty ("headTrackManager");
		impactDecalManager = serializedObject.FindProperty ("impactDecalManager");
		mainDecalManagerName = serializedObject.FindProperty ("mainDecalManagerName");

		playerInput = serializedObject.FindProperty ("playerInput");
		powersManager = serializedObject.FindProperty ("powersManager");
		playerCollider = serializedObject.FindProperty ("playerCollider");
		cursorRectTransform = serializedObject.FindProperty ("cursorRectTransform");
		parable = serializedObject.FindProperty ("parable");

		leftHand = serializedObject.FindProperty ("aimsettings.leftHand");
		rightHand = serializedObject.FindProperty ("aimsettings.rightHand");
		powersList = serializedObject.FindProperty ("shootsettings.powersList");
		headLookWhenAiming = serializedObject.FindProperty ("headLookWhenAiming");
		headLookSpeed = serializedObject.FindProperty ("headLookSpeed");
		headLookTarget = serializedObject.FindProperty ("headLookTarget");
		autoShootOnTag = serializedObject.FindProperty ("shootsettings.autoShootOnTag");
		layerToAutoShoot = serializedObject.FindProperty ("shootsettings.layerToAutoShoot");
		maxDistanceToRaycast = serializedObject.FindProperty ("shootsettings.maxDistanceToRaycast");
		shootAtLayerToo = serializedObject.FindProperty ("shootsettings.shootAtLayerToo");
		autoShootTagList = serializedObject.FindProperty ("shootsettings.autoShootTagList");
		useAimAssistInThirdPerson = serializedObject.FindProperty ("useAimAssistInThirdPerson");
		useMaxDistanceToCameraCenterAimAssist = serializedObject.FindProperty ("useMaxDistanceToCameraCenterAimAssist");
		maxDistanceToCameraCenterAimAssist = serializedObject.FindProperty ("maxDistanceToCameraCenterAimAssist");
		useAimAssistInLockedCamera = serializedObject.FindProperty ("useAimAssistInLockedCamera");
		infinitePower = serializedObject.FindProperty ("infinitePower");

		powerAmount = serializedObject.FindProperty ("shootsettings.powerAmount");
		maxPowerAmount = serializedObject.FindProperty ("shootsettings.maxPowerAmount");

		regeneratePower = serializedObject.FindProperty ("regeneratePower");
		constantRegenerate = serializedObject.FindProperty ("constantRegenerate");
		regenerateTime = serializedObject.FindProperty ("regenerateTime");
		regenerateSpeed = serializedObject.FindProperty ("regenerateSpeed");
		regenerateAmount = serializedObject.FindProperty ("regenerateAmount");
		targetToDamageLayer = serializedObject.FindProperty ("shootsettings.targetToDamageLayer");
		targetForScorchLayer = serializedObject.FindProperty ("targetForScorchLayer");
		powersSlotsAmount = serializedObject.FindProperty ("shootsettings.powersSlotsAmount");
		selectedPowerIcon = serializedObject.FindProperty ("shootsettings.selectedPowerIcon");
		selectedPowerHud = serializedObject.FindProperty ("shootsettings.selectedPowerHud");
		shootZone = serializedObject.FindProperty ("shootsettings.shootZone");
		firstPersonShootPosition = serializedObject.FindProperty ("shootsettings.firstPersonShootPosition");
		minSwipeDist = serializedObject.FindProperty ("shootsettings.minSwipeDist");
		usedByAI = serializedObject.FindProperty ("usedByAI");
		impactDecalList = serializedObject.FindProperty ("impactDecalList");

		projectilesPoolEnabled = serializedObject.FindProperty ("projectilesPoolEnabled");

		maxAmountOfPoolElementsOnWeapon = serializedObject.FindProperty ("maxAmountOfPoolElementsOnWeapon");

		useFreeAimMode = serializedObject.FindProperty ("useFreeAimMode");
		freeAimModeeActive = serializedObject.FindProperty ("freeAimModeeActive");
		armsPivotRotationTransform = serializedObject.FindProperty ("armsPivotRotationTransform");
		armsPivotRotationSpeed = serializedObject.FindProperty ("armsPivotRotationSpeed");
		armsPivotClampRotation = serializedObject.FindProperty ("armsPivotClampRotation");
		armsPivotRotationMultiplier = serializedObject.FindProperty ("armsPivotRotationMultiplier");
		freeAimModLookInOppositeDirectionExtraRange = serializedObject.FindProperty ("freeAimModLookInOppositeDirectionExtraRange");
		freeAimModePlayerStatusID = serializedObject.FindProperty ("freeAimModePlayerStatusID");

		_powersManager = (otherPowers)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;
		applicationIsPlaying = Application.isPlaying;
			
		GUILayout.BeginVertical ("Current Powers Action", "window");
		if (applicationIsPlaying) {
			if (_powersManager.shootsettings.powersList.Count > 0) {
				powerName = _powersManager.shootsettings.powersList [_powersManager.choosedPower].Name;
			}
		}
		GUILayout.Label ("Current Power Index\t\t" + choosedPower.intValue + "-" + powerName);
		GUILayout.Label ("Current Power\t\t" + currentPowerName.stringValue);
		GUILayout.Label ("Amount Powers\t\t" + amountPowersEnabled.intValue);

		GUILayout.Label ("Using Free Fire Mode\t\t" + usingFreeFireMode.boolValue);
		GUILayout.Label ("Action Active\t\t" + actionActive.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Powers State", "window");

		aimingInThirdPersonText = "NO";
		if (applicationIsPlaying) {
			if (aimingInThirdPerson.boolValue) {
				aimingInThirdPersonText = "YES";
			}
		}
		GUILayout.Label ("Aiming In Third Person\t" + aimingInThirdPersonText);

		aimingInFirstPersonText = "NO";
		if (applicationIsPlaying) {
			if (aimingInFirstPerson.boolValue) {
				aimingInFirstPersonText = "YES";
			}
		}
		GUILayout.Label ("Aiming In First Person\t\t" + aimingInFirstPersonText);

		usingPowersText = "NO";
		if (applicationIsPlaying) {
			if (usingPowers.boolValue) {
				usingPowersText = "YES";
			}
		}
		GUILayout.Label ("Using Powers\t\t" + usingPowersText);

		shootingWeapon = "NO";
		if (shooting.boolValue) {
			shootingWeapon = "YES";
		}
		GUILayout.Label ("Shooting Power\t\t" + shootingWeapon);

		powersModeActiveText = "NO";
		if (powersModeActive.boolValue) {
			powersModeActiveText = "YES";
		}
		GUILayout.Label ("Powers Mode Active\t\t" + powersModeActiveText);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (showSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Settings")) {
			showSettings.boolValue = !showSettings.boolValue;
		}
		if (showAimSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Aim Settings")) {
			showAimSettings.boolValue = !showAimSettings.boolValue;
		}
		if (showShootSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Shoot Settings")) {
			showShootSettings.boolValue = !showShootSettings.boolValue;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();		

		if (showSettings.boolValue) {
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Basic Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			GUILayout.BeginVertical ("Enabled abilities", "window");

			EditorGUILayout.PropertyField (aimModeEnabled);

			EditorGUILayout.PropertyField (shootEnabled);
			EditorGUILayout.PropertyField (changePowersEnabled);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers Settings", "window");
			EditorGUILayout.PropertyField (changePowersWithNumberKeysActive);
			EditorGUILayout.PropertyField (changePowersWithMouseWheelActive);
			EditorGUILayout.PropertyField (changePowersWithKeysActive);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Target Detection Settings", "window");
			EditorGUILayout.PropertyField (layerToDamage);
			EditorGUILayout.PropertyField (useCustomIgnoreTags);
			if (useCustomIgnoreTags.boolValue) {
				showLowerList (customTagsToIgnoreList);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Health Stat Settings", "window");
			EditorGUILayout.PropertyField (playerStatsManager);
			EditorGUILayout.PropertyField (energyStatName);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers Movements Settings", "window");
			EditorGUILayout.PropertyField (canFirePowersWithoutAiming);
			EditorGUILayout.PropertyField (useAimCameraOnFreeFireMode);
			EditorGUILayout.PropertyField (timeToStopAimAfterStopFiring);

			EditorGUILayout.PropertyField (useLowerRotationSpeedAimedThirdPerson);
			if (useLowerRotationSpeedAimedThirdPerson.boolValue) {
				EditorGUILayout.PropertyField (verticalRotationSpeedAimedInThirdPerson);
				EditorGUILayout.PropertyField (horizontalRotationSpeedAimedInThirdPerson);
			}
			EditorGUILayout.PropertyField (runWhenAimingPowerInThirdPerson);
			if (runWhenAimingPowerInThirdPerson.boolValue) {
				EditorGUILayout.PropertyField (stopRunIfPreviouslyNotRunning);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Reticle/Cursor Elements", "window");
			EditorGUILayout.PropertyField (cursor);
			EditorGUILayout.PropertyField (regularReticle);
			EditorGUILayout.PropertyField (customReticle);
			EditorGUILayout.PropertyField (customReticleImage);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Power Elements", "window");
			EditorGUILayout.PropertyField (powerBar);
			EditorGUILayout.PropertyField (powerBarText);

			EditorGUILayout.PropertyField (powersInfoPanel);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Power Rotation Point Settings", "window");
			EditorGUILayout.PropertyField (usePowerRotationPoint);
			if (usePowerRotationPoint.boolValue) {
				EditorGUILayout.PropertyField (powerRotationPoint);

				EditorGUILayout.PropertyField (rotationUpPointAmountMultiplier);
				EditorGUILayout.PropertyField (rotationDownPointAmountMultiplier);
				EditorGUILayout.PropertyField (rotationPointSpeed);
				EditorGUILayout.PropertyField (useRotationUpClamp);
				if (useRotationUpClamp.boolValue) {
					EditorGUILayout.PropertyField (rotationUpClampAmount);
				}
				EditorGUILayout.PropertyField (useRotationDownClamp);
				if (useRotationDownClamp.boolValue) {
					EditorGUILayout.PropertyField (rotationDownClamp);
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers Mode Active Event Settings", "window");
			EditorGUILayout.PropertyField (useEventsOnStateChange);
			if (useEventsOnStateChange.boolValue) {
				EditorGUILayout.PropertyField (evenOnStateEnabled);
				EditorGUILayout.PropertyField (eventOnStateDisabled);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			buttonColor = GUI.backgroundColor;
			EditorGUILayout.BeginVertical ();
			if (showElements.boolValue) {
				GUI.backgroundColor = Color.gray;
				currentButtonString = "Hide Player Components";
			} else {
				GUI.backgroundColor = buttonColor;
				currentButtonString = "Show Player Components";
			}

			if (GUILayout.Button (currentButtonString)) {
				showElements.boolValue = !showElements.boolValue;
			}

			GUI.backgroundColor = buttonColor;
			EditorGUILayout.EndVertical ();

			if (showElements.boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Player Elements", "window");
				EditorGUILayout.PropertyField (shootZoneAudioSource);
				EditorGUILayout.PropertyField (mainCameraTransform);
				EditorGUILayout.PropertyField (mainCamera);
				EditorGUILayout.PropertyField (upperBodyRotationManager);
				EditorGUILayout.PropertyField (IKSystemManager);
				EditorGUILayout.PropertyField (playerCameraManager);
	
				EditorGUILayout.PropertyField (headBobManager);
				EditorGUILayout.PropertyField (weaponsManager);

				EditorGUILayout.PropertyField (playerControllerManager);
				EditorGUILayout.PropertyField (grabObjectsManager);

				EditorGUILayout.PropertyField (headTrackManager);
				EditorGUILayout.PropertyField (impactDecalManager);

				EditorGUILayout.PropertyField (playerInput);
				EditorGUILayout.PropertyField (powersManager);
				EditorGUILayout.PropertyField (playerCollider);
				EditorGUILayout.PropertyField (cursorRectTransform);

				EditorGUILayout.PropertyField (parable);
				GUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAimSettings.boolValue) {
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Aim Settings for bones using powers and weapons", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Hands Settings", "window");
			EditorGUILayout.PropertyField (leftHand);
			EditorGUILayout.PropertyField (rightHand);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Free Fire Mode Settings", "window");
			EditorGUILayout.PropertyField (useFreeAimMode);
			EditorGUILayout.PropertyField (freeAimModeeActive);
			EditorGUILayout.PropertyField (armsPivotRotationTransform);
			EditorGUILayout.PropertyField (armsPivotRotationSpeed);
			EditorGUILayout.PropertyField (armsPivotClampRotation);
			EditorGUILayout.PropertyField (armsPivotRotationMultiplier);
			EditorGUILayout.PropertyField (freeAimModLookInOppositeDirectionExtraRange);
			EditorGUILayout.PropertyField (freeAimModePlayerStatusID);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
	
			EditorGUILayout.Space ();
		}

		if (showShootSettings.boolValue) {

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Shoot Settings for every power", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers List", "window");

			EditorGUILayout.PropertyField (initialPowerIndex);

			EditorGUILayout.Space ();

			showPowerList (powersList);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Head Look Settings", "window");
			EditorGUILayout.PropertyField (headLookWhenAiming);
			if (headLookWhenAiming.boolValue) {
				EditorGUILayout.PropertyField (headLookSpeed);
				EditorGUILayout.PropertyField (headLookTarget);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Auto Shoot Settings", "window");
			EditorGUILayout.PropertyField (autoShootOnTag);
			if (autoShootOnTag.boolValue) {
				EditorGUILayout.PropertyField (layerToAutoShoot);
				EditorGUILayout.PropertyField (maxDistanceToRaycast);
				EditorGUILayout.PropertyField (shootAtLayerToo);

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Auto Shoot Tag List", "window");
				showLowerList (autoShootTagList);
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Aim Assist Settings", "window");
			EditorGUILayout.PropertyField (useAimAssistInThirdPerson);
			EditorGUILayout.PropertyField (useMaxDistanceToCameraCenterAimAssist);
			if (useMaxDistanceToCameraCenterAimAssist.boolValue) {
				EditorGUILayout.PropertyField (maxDistanceToCameraCenterAimAssist);
			}
			EditorGUILayout.PropertyField (useAimAssistInLockedCamera);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Power Amount Settings", "window");
			EditorGUILayout.PropertyField (projectilesPoolEnabled);
			EditorGUILayout.PropertyField (maxAmountOfPoolElementsOnWeapon);
	
			EditorGUILayout.PropertyField (infinitePower);

			if (!infinitePower.boolValue) {
				EditorGUILayout.PropertyField (powerAmount);
				EditorGUILayout.PropertyField (maxPowerAmount);

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Regenerate Power Settings", "window");
				EditorGUILayout.PropertyField (regeneratePower);
				if (regeneratePower.boolValue) {
					EditorGUILayout.PropertyField (constantRegenerate);
					EditorGUILayout.PropertyField (regenerateTime);
					if (constantRegenerate.boolValue) {
						EditorGUILayout.PropertyField (regenerateSpeed);
					}
					if (!constantRegenerate.boolValue) {
						EditorGUILayout.PropertyField (regenerateAmount);
					}
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers Settings", "window");
			EditorGUILayout.PropertyField (targetToDamageLayer);
			EditorGUILayout.PropertyField (targetForScorchLayer);
			EditorGUILayout.PropertyField (powersSlotsAmount);

			EditorGUILayout.PropertyField (mainDecalManagerName);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Powers Elements", "window");
			EditorGUILayout.PropertyField (selectedPowerIcon);
			EditorGUILayout.PropertyField (selectedPowerHud);
			EditorGUILayout.PropertyField (shootZone);
			EditorGUILayout.PropertyField (firstPersonShootPosition);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Touch Change Powers Settings", "window");
			EditorGUILayout.PropertyField (minSwipeDist);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (usedByAI);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUI.backgroundColor = buttonColor;

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Power Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberKey"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("texture"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReticle"));
		if (list.FindPropertyRelative ("useCustomReticle").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("customReticle"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("powerEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("powerAssigned"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountPowerNeeded"));
		GUILayout.EndVertical ();		

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hand Recoil Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRecoil"));
		if (list.FindPropertyRelative ("useRecoil").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("recoilSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("recoilAmount"));
		}
		GUILayout.EndVertical ();	

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Fire Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("automatic"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFireRate"));
		if (list.FindPropertyRelative ("useFireRate").boolValue || list.FindPropertyRelative ("automatic").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("fireRate"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBurst"));
			if (list.FindPropertyRelative ("useBurst").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("burstAmount"));
			}
		}
		GUILayout.EndVertical ();		

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootAProjectile"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("launchProjectile"));
		if (list.FindPropertyRelative ("shootAProjectile").boolValue || list.FindPropertyRelative ("launchProjectile").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectile"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileDamage"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("killInOneShot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileWithAbility"));

			if (list.FindPropertyRelative ("launchProjectile").boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Launch Projectile Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateLaunchParableThirdPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateLaunchParableFirstPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useParableSpeed"));
				if (!list.FindPropertyRelative ("useParableSpeed").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileSpeed"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("parableDirectionTransform"));
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMaxDistanceWhenNoSurfaceFound"));
					if (list.FindPropertyRelative ("useMaxDistanceWhenNoSurfaceFound").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDistanceWhenNoSurfaceFound"));
					}
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRayCastShoot"));
				if (!list.FindPropertyRelative ("useRayCastShoot").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileSpeed"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastCheckingOnRigidbody"));	
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("customRaycastCheckingRate"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("customRaycastCheckingDistance"));
				}
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Search Target Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isHommingProjectile"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isSeeker"));
			if (list.FindPropertyRelative ("isHommingProjectile").boolValue || list.FindPropertyRelative ("isSeeker").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("waitTimeToSearchTarget"));
				showLowerList (list.FindPropertyRelative ("tagToLocate"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("homingProjectilesMaxAmount"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("targetOnScreenForSeeker"));
			}
			if (list.FindPropertyRelative ("isHommingProjectile").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("locatedEnemyIconName"));
			}
			GUILayout.EndVertical ();		

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Force Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactForceApplied"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceMode"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyImpactForceToVehicles"));
			if (list.FindPropertyRelative ("applyImpactForceToVehicles").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactForceToVehiclesMultiplier"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceMassMultiplier"));
			GUILayout.EndVertical ();		

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Explosion Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isExplosive"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isImplosive"));
			if (list.FindPropertyRelative ("isExplosive").boolValue || list.FindPropertyRelative ("isImplosive").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionForce"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionRadius"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useExplosionDelay"));
				if (list.FindPropertyRelative ("useExplosionDelay").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionDelay"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionDamage"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacters"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("canDamageProjectileOwner"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyExplosionForceToVehicles"));
				if (list.FindPropertyRelative ("applyExplosionForceToVehicles").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionForceToVehiclesMultiplier"));
				}
			}
			GUILayout.EndVertical ();		

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Disable Projectile Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDisableTimer"));
			if (list.FindPropertyRelative ("useDisableTimer").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("noImpactDisableTimer"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactDisableTimer"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sound Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootSoundEffect"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactSoundEffect"));
			GUILayout.EndVertical ();
	
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Particles Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactParticles"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		} 
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventToCall"));
		if (list.FindPropertyRelative ("useEventToCall").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCall"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ability Power Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("powerWithAbility"));
		if (list.FindPropertyRelative ("powerWithAbility").boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Button Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDownButton"));
			if (list.FindPropertyRelative ("useDownButton").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("downButtonAction"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHoldButton"));
			if (list.FindPropertyRelative ("useHoldButton").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("holdButtonAction"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useUpButton"));
			if (list.FindPropertyRelative ("useUpButton").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("upButtonAction"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Secondary Action Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryAction"));
		if (list.FindPropertyRelative ("useSecondaryAction").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondaryAction"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Spread Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useProjectileSpread"));
		if (list.FindPropertyRelative ("useProjectileSpread").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("spreadAmount"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scorch Settings", "window");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scorch from Decal Manager", "window");
		if (impactDecalList.arraySize > 0) {
			list.FindPropertyRelative ("impactDecalIndex").intValue = EditorGUILayout.Popup ("Default Decal Type", 
				list.FindPropertyRelative ("impactDecalIndex").intValue, _powersManager.impactDecalList);
			list.FindPropertyRelative ("impactDecalName").stringValue = _powersManager.impactDecalList [list.FindPropertyRelative ("impactDecalIndex").intValue];
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Decal Impact List")) {
			_powersManager.getImpactListInfo ();					
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Regular Scorch", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("scorch"));
		if (list.FindPropertyRelative ("scorch").objectReferenceValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("scorchRayCastDistance"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Auto Shoot Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("autoShootOnTag"));
		if (list.FindPropertyRelative ("autoShootOnTag").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("layerToAutoShoot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDistanceToRaycast"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootAtLayerToo"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Auto Shoot Tag List", "window");
			showLowerList (list.FindPropertyRelative ("autoShootTagList"));
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Force Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyForceAtShoot"));
		if (list.FindPropertyRelative ("applyForceAtShoot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceDirection"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceAmount"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Adherence Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useGravityOnLaunch"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useGraivtyOnImpact"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("adhereToSurface"));
		if (list.FindPropertyRelative ("adhereToSurface").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("adhereToLimbs"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreSetProjectilePositionOnImpact"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Pierce Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("breakThroughObjects"));
		if (list.FindPropertyRelative ("breakThroughObjects").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteNumberOfImpacts"));
			if (!list.FindPropertyRelative ("infiniteNumberOfImpacts").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberOfImpacts"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canDamageSameObjectMultipleTimes"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Muzzle Flash Light Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMuzzleFlash"));
		if (list.FindPropertyRelative ("useMuzzleFlash").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("muzzleFlahsLight"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("muzzleFlahsDuration"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Damage Target Over Time Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageTargetOverTime"));
		if (list.FindPropertyRelative ("damageTargetOverTime").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeDelay"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeDuration"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeAmount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeRate"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeToDeath"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("removeDamageOverTimeState"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sedate Characters Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateCharacters"));
		if (list.FindPropertyRelative ("sedateCharacters").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateDelay"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useWeakSpotToReduceDelay"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateUntilReceiveDamage"));
			if (!list.FindPropertyRelative ("sedateUntilReceiveDamage").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateDuration"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Push Characters Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacter"));
		if (list.FindPropertyRelative ("pushCharacter").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacterForce"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacterRagdollForce"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Remote Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEventOnObjectsFound"));
		if (list.FindPropertyRelative ("useRemoteEventOnObjectsFound").boolValue) {

			EditorGUILayout.Space ();

			showLowerList (list.FindPropertyRelative ("remoteEventNameList"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEventOnObjectsFoundOnExplosion"));
		if (list.FindPropertyRelative ("useRemoteEventOnObjectsFoundOnExplosion").boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remoteEventNameOnExplosion"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Shield And Reaction Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreShield"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canActivateReactionSystemTemporally"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageReactionID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageTypeID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileCanBeDeflected"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		bool shakeSettings = list.FindPropertyRelative ("showShakeSettings").boolValue;
		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (shakeSettings) {
			GUI.backgroundColor = Color.gray;
			currentButtonString = "Hide Shake Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			currentButtonString = "Show Shake Settings";
		}
		if (GUILayout.Button (currentButtonString)) {
			shakeSettings = !shakeSettings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();
		list.FindPropertyRelative ("showShakeSettings").boolValue = shakeSettings;

		if (shakeSettings) {

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Shake Settings when this power is used", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Auto Shoot Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useShake"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sameValueBothViews"));
			if (list.FindPropertyRelative ("useShake").boolValue) {

				EditorGUILayout.Space ();

				if (list.FindPropertyRelative ("sameValueBothViews").boolValue) {
					showShakeInfo (list.FindPropertyRelative ("thirdPersonShakeInfo"), false, "Shake In Third Person");
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useShakeInThirdPerson"));
					if (list.FindPropertyRelative ("useShakeInThirdPerson").boolValue) {
						showShakeInfo (list.FindPropertyRelative ("thirdPersonShakeInfo"), false, "Shake In Third Person");

						EditorGUILayout.Space ();

					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useShakeInFirstPerson"));
					if (list.FindPropertyRelative ("useShakeInFirstPerson").boolValue) {
						showShakeInfo (list.FindPropertyRelative ("firstPersonShakeInfo"), true, "Shake In First Person");

						EditorGUILayout.Space ();
					}
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showShakeInfo (SerializedProperty list, bool isFirstPerson, string shakeName)
	{
		GUILayout.BeginVertical (shakeName, "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shotForce"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeDuration"));
		if (isFirstPerson) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePosition"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotation"));
		GUILayout.EndVertical ();
	}

	void showPowerList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			listarraySize = list.arraySize;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of powers: " + listarraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Power")) {
				list.arraySize++;

				listarraySize = list.arraySize;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;

				listarraySize = list.arraySize;
			}

			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < listarraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < listarraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Powers")) {
				for (int i = 0; i < listarraySize; i++) {
					_powersManager.enableOrDisableAllPowers (true);
				}
			}
			if (GUILayout.Button ("Disable All Powers")) {
				for (int i = 0; i < listarraySize; i++) {
					_powersManager.enableOrDisableAllPowers (false);
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < listarraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");
			
				EditorGUILayout.Space ();

				if (i < listarraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					currentArrayElement = list.GetArrayElementAtIndex (i);

					powerEnabled = currentArrayElement.FindPropertyRelative ("powerEnabled").boolValue;
					powerEnabledString = " +";

					if (!powerEnabled) {
						powerEnabledString = " -";
					}

					EditorGUILayout.PropertyField (currentArrayElement, new GUIContent (i.ToString () + " - " + currentArrayElement.displayName + powerEnabledString), false);

					if (currentArrayElement.isExpanded) {
						showListElementInfo (currentArrayElement, true);
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

					listarraySize = list.arraySize;
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < listarraySize) {
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

	void showLowerList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("New")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showTrailsList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Trails: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Trail")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}

			GUILayout.EndHorizontal ();
	
			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + powerEnabledString), false);
				

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();

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
		GUILayout.EndVertical ();
	}
}
#endif
