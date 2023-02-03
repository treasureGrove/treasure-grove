using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(vehicleHUDManager))]
public class vehicleHUDManagerEditor : Editor
{
	SerializedProperty isBeingDriven;
	SerializedProperty passengersOnVehicle;
	SerializedProperty destroyed;
	SerializedProperty showMainSettings;
	SerializedProperty showVehicleStats;
	SerializedProperty showAdvancedSettings;
	SerializedProperty showPhysicsSettings;
	SerializedProperty showEventSettings;
	SerializedProperty showOtherSettings;
	SerializedProperty showDebugSettings;
	SerializedProperty showAllSettings;
	SerializedProperty showVehicleElements;
	SerializedProperty vehicleName;

	SerializedProperty destroyedSound;
	SerializedProperty destroyedSource;

	SerializedProperty useDamageParticles;
	SerializedProperty healthPercentageDamageParticles;

	SerializedProperty damageParticles;

	SerializedProperty useDestroyedParticles;
	SerializedProperty destroyedParticles;

	SerializedProperty placeToShoot;
	SerializedProperty canSetTurnOnState;
	SerializedProperty autoTurnOnWhenGetOn;
	SerializedProperty vehicleRadius;
	SerializedProperty layer;
	SerializedProperty layerForPassengers;
	SerializedProperty passengersParent;
	SerializedProperty canUnlockCursor;
	SerializedProperty showVehicleHUD;
	SerializedProperty showVehicleSpeed;

	SerializedProperty invincible;
	SerializedProperty healthAmount;
	SerializedProperty maxHealthAmount;

	SerializedProperty regenerateHealth;
	SerializedProperty constantHealthRegenerate;
	SerializedProperty regenerateHealthTime;
	SerializedProperty regenerateHealthSpeed;
	SerializedProperty regenerateHealthAmount;

	SerializedProperty vehicleUseBoost;
	SerializedProperty infiniteBoost;
	SerializedProperty boostAmount;
	SerializedProperty maxBoostAmount;
	SerializedProperty boostUseRate;
	SerializedProperty regenerateBoost;
	SerializedProperty constantBoostRegenerate;
	SerializedProperty regenerateBoostTime;
	SerializedProperty regenerateBoostSpeed;
	SerializedProperty regenerateBoostAmount;

	SerializedProperty vehicleUseFuel;
	SerializedProperty infiniteFuel;
	SerializedProperty fuelAmount;
	SerializedProperty maxFuelAmount;
	SerializedProperty fuelUseRate;
	SerializedProperty regenerateFuel;
	SerializedProperty constantFuelRegenerate;
	SerializedProperty regenerateFuelTime;
	SerializedProperty regenerateFuelSpeed;
	SerializedProperty regenerateFuelAmount;

	SerializedProperty startWithGasTankEmpty;

	SerializedProperty gasTankGameObject;
	SerializedProperty useWeakSpots;
	SerializedProperty damageReceiverList;
	SerializedProperty useCustomVehiclePartsToDestroy;
	SerializedProperty customVehiclePartsExplosionForce;
	SerializedProperty customVehiclePartsExplosionRadius;
	SerializedProperty customVehiclePartsExplosionForceMode;
	SerializedProperty customVehiclePartsListToDestroy;
	SerializedProperty useEventOnCustomVehicleParts;
	SerializedProperty eventOnCustomVehicleParts;
	SerializedProperty vehiclePartsToIgnore;

	SerializedProperty removePiecesWhenDestroyed;
	SerializedProperty fadeVehiclePiecesOnDestroyed;
	SerializedProperty timeToFadePieces;
	SerializedProperty destroyedMeshShader;

	SerializedProperty setCollidersOnAllVehicleMeshParts;


	SerializedProperty canUseSelfDestruct;
	SerializedProperty canStopSelfDestruction;
	SerializedProperty selfDestructDelay;
	SerializedProperty ejectPassengerOnSelfDestruct;
	SerializedProperty ejectPassengerFoce;
	SerializedProperty getOffPassengersOnSelfDestruct;

	SerializedProperty damageObjectsOnCollision;
	SerializedProperty damageMultiplierOnCollision;
	SerializedProperty minVehicleVelocityToDamage;
	SerializedProperty minCollisionVelocityToDamage;
	SerializedProperty ignoreShieldOnCollision;
	SerializedProperty launchDriverOnCollision;
	SerializedProperty minCollisionVelocityToLaunch;
	SerializedProperty launchDirectionOffset;
	SerializedProperty extraCollisionForce;
	SerializedProperty ignoreRagdollCollider;
	SerializedProperty applyDamageToDriver;
	SerializedProperty useCollisionVelocityAsDamage;
	SerializedProperty collisionDamageAmount;
	SerializedProperty ignoreShieldOnLaunch;
	SerializedProperty vehicleExplosionForce;
	SerializedProperty vehicleExplosionRadius;
	SerializedProperty vehicleExplosionForceMode;
	SerializedProperty receiveDamageFromCollision;
	SerializedProperty minVelocityToDamageCollision;
	SerializedProperty useCurrentVelocityAsDamage;
	SerializedProperty defaultDamageCollision;
	SerializedProperty collisionDamageMultiplier;
	SerializedProperty useImpactSurface;

	SerializedProperty useRemoteEventOnObjectsFound;
	SerializedProperty remoteEventNameList;

	SerializedProperty impactDecalList;
	SerializedProperty impactDecalIndex;
	SerializedProperty impactDecalName;

	SerializedProperty mainDecalManagerName;

	SerializedProperty useEventsOnStateChanged;
	SerializedProperty eventOnGetOn;
	SerializedProperty eventOnGetOff;
	SerializedProperty eventOnDestroyed;
	SerializedProperty useJumpPlatformEvents;
	SerializedProperty jumpPlatformEvent;
	SerializedProperty jumpPlatformParableEvent;
	SerializedProperty setNewJumpPowerEvent;
	SerializedProperty setOriginalJumpPowerEvent;
	SerializedProperty passengerGettingOnOffEvent;
	SerializedProperty changeVehicleStateEvent;
	SerializedProperty useHornToCallFriends;
	SerializedProperty callOnlyFoundFriends;
	SerializedProperty radiusToCallFriends;
	SerializedProperty useHornEvent;
	SerializedProperty hornEvent;

	SerializedProperty useEventsToCheckIfVehicleUsedByAIOnPassengerEnterExit;
	SerializedProperty eventToCheckIfVehicleUsedByAIOnPassengerEnter;
	SerializedProperty eventToCheckIfVehicleUsedByAIOnPassengerExit;

	SerializedProperty useEventToSendPassenger;
	SerializedProperty eventToSendPassengerOnGetOn;
	SerializedProperty eventToSentPassengerOnGetOff;

	SerializedProperty eventToSendCurrentHealthAmount;
	SerializedProperty eventToSendCurrentFuelAmount;
	SerializedProperty eventToSendCurrentBoostAmount;

	SerializedProperty canEjectFromVehicle;
	SerializedProperty audioSourceList;
	SerializedProperty usedByAI;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty alphaColor;
	SerializedProperty showDebugLogCollisions;
	SerializedProperty healthAmountToTakeOnEditor;
	SerializedProperty debugLaunchCharacterSpeed;
	SerializedProperty debugLaunchCharacterPosition;
	SerializedProperty IKDrivingManager;
	SerializedProperty weaponsManager;
	SerializedProperty mainRigidbody;
	SerializedProperty damageInScreenManager;
	SerializedProperty mapInformationManager;
	SerializedProperty vehicleCameraManager;
	SerializedProperty gasTankManager;
	SerializedProperty vehicleGravitymanager;
	SerializedProperty mainVehicleController;

	SerializedProperty healthStatName;
	SerializedProperty energyStatName;
	SerializedProperty fuelStatName;

	vehicleHUDManager vehicleHUD;
	GUIStyle style = new GUIStyle ();

	Color buttonColor;
	bool expanded;

	string buttonMessage;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		isBeingDriven = serializedObject.FindProperty ("isBeingDriven");
		passengersOnVehicle = serializedObject.FindProperty ("passengersOnVehicle");
		destroyed = serializedObject.FindProperty ("destroyed");
		showMainSettings = serializedObject.FindProperty ("showMainSettings");
		showVehicleStats = serializedObject.FindProperty ("showVehicleStats");
		showAdvancedSettings = serializedObject.FindProperty ("showAdvancedSettings");
		showPhysicsSettings = serializedObject.FindProperty ("showPhysicsSettings");
		showEventSettings = serializedObject.FindProperty ("showEventSettings");
		showOtherSettings = serializedObject.FindProperty ("showOtherSettings");
		showDebugSettings = serializedObject.FindProperty ("showDebugSettings");
		showAllSettings = serializedObject.FindProperty ("showAllSettings");
		showVehicleElements = serializedObject.FindProperty ("showVehicleElements");
		vehicleName = serializedObject.FindProperty ("vehicleName");

		destroyedSound = serializedObject.FindProperty ("destroyedSound");
		destroyedSource = serializedObject.FindProperty ("destroyedSource");
		healthPercentageDamageParticles = serializedObject.FindProperty ("healthPercentageDamageParticles");
		useDamageParticles = serializedObject.FindProperty ("useDamageParticles");

		damageParticles = serializedObject.FindProperty ("damageParticles");
		destroyedParticles = serializedObject.FindProperty ("destroyedParticles");
		useDestroyedParticles = serializedObject.FindProperty ("useDestroyedParticles");

		placeToShoot = serializedObject.FindProperty ("placeToShoot");
		canSetTurnOnState = serializedObject.FindProperty ("canSetTurnOnState");
		autoTurnOnWhenGetOn = serializedObject.FindProperty ("autoTurnOnWhenGetOn");
		vehicleRadius = serializedObject.FindProperty ("vehicleRadius");
		layer = serializedObject.FindProperty ("layer");
		layerForPassengers = serializedObject.FindProperty ("layerForPassengers");
		passengersParent = serializedObject.FindProperty ("passengersParent");
		canUnlockCursor = serializedObject.FindProperty ("canUnlockCursor");
		showVehicleHUD = serializedObject.FindProperty ("showVehicleHUD");
		showVehicleSpeed = serializedObject.FindProperty ("showVehicleSpeed");

		invincible = serializedObject.FindProperty ("invincible");
		healthAmount = serializedObject.FindProperty ("healthAmount");
		maxHealthAmount = serializedObject.FindProperty ("maxHealthAmount");
		regenerateHealth = serializedObject.FindProperty ("regenerateHealth");
		constantHealthRegenerate = serializedObject.FindProperty ("constantHealthRegenerate");
		regenerateHealthTime = serializedObject.FindProperty ("regenerateHealthTime");
		regenerateHealthSpeed = serializedObject.FindProperty ("regenerateHealthSpeed");
		regenerateHealthAmount = serializedObject.FindProperty ("regenerateHealthAmount");

		vehicleUseBoost = serializedObject.FindProperty ("vehicleUseBoost");
		infiniteBoost = serializedObject.FindProperty ("infiniteBoost");
		boostAmount = serializedObject.FindProperty ("boostAmount");
		maxBoostAmount = serializedObject.FindProperty ("maxBoostAmount");
		boostUseRate = serializedObject.FindProperty ("boostUseRate");
		regenerateBoost = serializedObject.FindProperty ("regenerateBoost");
		constantBoostRegenerate = serializedObject.FindProperty ("constantBoostRegenerate");
		regenerateBoostTime = serializedObject.FindProperty ("regenerateBoostTime");
		regenerateBoostSpeed = serializedObject.FindProperty ("regenerateBoostSpeed");
		regenerateBoostAmount = serializedObject.FindProperty ("regenerateBoostAmount");

		vehicleUseFuel = serializedObject.FindProperty ("vehicleUseFuel");
		infiniteFuel = serializedObject.FindProperty ("infiniteFuel");
		fuelAmount = serializedObject.FindProperty ("fuelAmount");
		maxFuelAmount = serializedObject.FindProperty ("maxFuelAmount");
		fuelUseRate = serializedObject.FindProperty ("fuelUseRate");
		regenerateFuel = serializedObject.FindProperty ("regenerateFuel");
		constantFuelRegenerate = serializedObject.FindProperty ("constantFuelRegenerate");
		regenerateFuelTime = serializedObject.FindProperty ("regenerateFuelTime");
		regenerateFuelSpeed = serializedObject.FindProperty ("regenerateFuelSpeed");
		regenerateFuelAmount = serializedObject.FindProperty ("regenerateFuelAmount");

		startWithGasTankEmpty = serializedObject.FindProperty ("startWithGasTankEmpty");

		gasTankGameObject = serializedObject.FindProperty ("gasTankGameObject");
		useWeakSpots = serializedObject.FindProperty ("useWeakSpots");
		damageReceiverList = serializedObject.FindProperty ("advancedSettings.damageReceiverList");
		useCustomVehiclePartsToDestroy = serializedObject.FindProperty ("useCustomVehiclePartsToDestroy");
		customVehiclePartsExplosionForce = serializedObject.FindProperty ("customVehiclePartsExplosionForce");
		customVehiclePartsExplosionRadius = serializedObject.FindProperty ("customVehiclePartsExplosionRadius");
		customVehiclePartsExplosionForceMode = serializedObject.FindProperty ("customVehiclePartsExplosionForceMode");
		customVehiclePartsListToDestroy = serializedObject.FindProperty ("customVehiclePartsListToDestroy");
		useEventOnCustomVehicleParts = serializedObject.FindProperty ("useEventOnCustomVehicleParts");
		eventOnCustomVehicleParts = serializedObject.FindProperty ("eventOnCustomVehicleParts");
		vehiclePartsToIgnore = serializedObject.FindProperty ("vehiclePartsToIgnore");

		removePiecesWhenDestroyed = serializedObject.FindProperty ("removePiecesWhenDestroyed");
		fadeVehiclePiecesOnDestroyed = serializedObject.FindProperty ("fadeVehiclePiecesOnDestroyed");
		timeToFadePieces = serializedObject.FindProperty ("timeToFadePieces");
		destroyedMeshShader = serializedObject.FindProperty ("destroyedMeshShader");

		setCollidersOnAllVehicleMeshParts = serializedObject.FindProperty ("setCollidersOnAllVehicleMeshParts");

		canUseSelfDestruct = serializedObject.FindProperty ("canUseSelfDestruct");
		canStopSelfDestruction = serializedObject.FindProperty ("canStopSelfDestruction");
		selfDestructDelay = serializedObject.FindProperty ("selfDestructDelay");
		ejectPassengerOnSelfDestruct = serializedObject.FindProperty ("ejectPassengerOnSelfDestruct");
		ejectPassengerFoce = serializedObject.FindProperty ("ejectPassengerFoce");
		getOffPassengersOnSelfDestruct = serializedObject.FindProperty ("getOffPassengersOnSelfDestruct");

		damageObjectsOnCollision = serializedObject.FindProperty ("damageObjectsOnCollision");
		damageMultiplierOnCollision = serializedObject.FindProperty ("damageMultiplierOnCollision");
		minVehicleVelocityToDamage = serializedObject.FindProperty ("minVehicleVelocityToDamage");
		minCollisionVelocityToDamage = serializedObject.FindProperty ("minCollisionVelocityToDamage");
		ignoreShieldOnCollision = serializedObject.FindProperty ("ignoreShieldOnCollision");
		launchDriverOnCollision = serializedObject.FindProperty ("launchDriverOnCollision");
		minCollisionVelocityToLaunch = serializedObject.FindProperty ("minCollisionVelocityToLaunch");
		launchDirectionOffset = serializedObject.FindProperty ("launchDirectionOffset");
		extraCollisionForce = serializedObject.FindProperty ("extraCollisionForce");
		ignoreRagdollCollider = serializedObject.FindProperty ("ignoreRagdollCollider");
		applyDamageToDriver = serializedObject.FindProperty ("applyDamageToDriver");
		useCollisionVelocityAsDamage = serializedObject.FindProperty ("useCollisionVelocityAsDamage");
		collisionDamageAmount = serializedObject.FindProperty ("collisionDamageAmount");
		ignoreShieldOnLaunch = serializedObject.FindProperty ("ignoreShieldOnLaunch");
		vehicleExplosionForce = serializedObject.FindProperty ("vehicleExplosionForce");
		vehicleExplosionRadius = serializedObject.FindProperty ("vehicleExplosionRadius");
		vehicleExplosionForceMode = serializedObject.FindProperty ("vehicleExplosionForceMode");
		receiveDamageFromCollision = serializedObject.FindProperty ("receiveDamageFromCollision");
		minVelocityToDamageCollision = serializedObject.FindProperty ("minVelocityToDamageCollision");
		useCurrentVelocityAsDamage = serializedObject.FindProperty ("useCurrentVelocityAsDamage");
		defaultDamageCollision = serializedObject.FindProperty ("defaultDamageCollision");
		collisionDamageMultiplier = serializedObject.FindProperty ("collisionDamageMultiplier");
		useImpactSurface = serializedObject.FindProperty ("useImpactSurface");

		useRemoteEventOnObjectsFound = serializedObject.FindProperty ("useRemoteEventOnObjectsFound");
		remoteEventNameList = serializedObject.FindProperty ("remoteEventNameList");

		impactDecalList = serializedObject.FindProperty ("impactDecalList");
		impactDecalIndex = serializedObject.FindProperty ("impactDecalIndex");
		impactDecalName = serializedObject.FindProperty ("impactDecalName");

		mainDecalManagerName = serializedObject.FindProperty ("mainDecalManagerName");

		useEventsOnStateChanged = serializedObject.FindProperty ("useEventsOnStateChanged");
		eventOnGetOn = serializedObject.FindProperty ("eventOnGetOn");
		eventOnGetOff = serializedObject.FindProperty ("eventOnGetOff");
		eventOnDestroyed = serializedObject.FindProperty ("eventOnDestroyed");
		useJumpPlatformEvents = serializedObject.FindProperty ("useJumpPlatformEvents");
		jumpPlatformEvent = serializedObject.FindProperty ("jumpPlatformEvent");
		jumpPlatformParableEvent = serializedObject.FindProperty ("jumpPlatformParableEvent");
		setNewJumpPowerEvent = serializedObject.FindProperty ("setNewJumpPowerEvent");
		setOriginalJumpPowerEvent = serializedObject.FindProperty ("setOriginalJumpPowerEvent");
		passengerGettingOnOffEvent = serializedObject.FindProperty ("passengerGettingOnOffEvent");
		changeVehicleStateEvent = serializedObject.FindProperty ("changeVehicleStateEvent");
		useHornToCallFriends = serializedObject.FindProperty ("useHornToCallFriends");
		callOnlyFoundFriends = serializedObject.FindProperty ("callOnlyFoundFriends");
		radiusToCallFriends = serializedObject.FindProperty ("radiusToCallFriends");
		useHornEvent = serializedObject.FindProperty ("useHornEvent");
		hornEvent = serializedObject.FindProperty ("hornEvent");

		useEventsToCheckIfVehicleUsedByAIOnPassengerEnterExit = serializedObject.FindProperty ("useEventsToCheckIfVehicleUsedByAIOnPassengerEnterExit");
		eventToCheckIfVehicleUsedByAIOnPassengerEnter = serializedObject.FindProperty ("eventToCheckIfVehicleUsedByAIOnPassengerEnter");
		eventToCheckIfVehicleUsedByAIOnPassengerExit = serializedObject.FindProperty ("eventToCheckIfVehicleUsedByAIOnPassengerExit");

		useEventToSendPassenger = serializedObject.FindProperty ("useEventToSendPassenger");
		eventToSendPassengerOnGetOn = serializedObject.FindProperty ("eventToSendPassengerOnGetOn");
		eventToSentPassengerOnGetOff = serializedObject.FindProperty ("eventToSentPassengerOnGetOff");

		eventToSendCurrentHealthAmount = serializedObject.FindProperty ("eventToSendCurrentHealthAmount");
		eventToSendCurrentFuelAmount = serializedObject.FindProperty ("eventToSendCurrentFuelAmount");
		eventToSendCurrentBoostAmount = serializedObject.FindProperty ("eventToSendCurrentBoostAmount");

		canEjectFromVehicle = serializedObject.FindProperty ("canEjectFromVehicle");
		audioSourceList = serializedObject.FindProperty ("audioSourceList");
		usedByAI = serializedObject.FindProperty ("usedByAI");
		showGizmo = serializedObject.FindProperty ("advancedSettings.showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("advancedSettings.gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("advancedSettings.gizmoRadius");
		alphaColor = serializedObject.FindProperty ("advancedSettings.alphaColor");
		showDebugLogCollisions = serializedObject.FindProperty ("showDebugLogCollisions");
		healthAmountToTakeOnEditor = serializedObject.FindProperty ("healthAmountToTakeOnEditor");
		debugLaunchCharacterSpeed = serializedObject.FindProperty ("debugLaunchCharacterSpeed");
		debugLaunchCharacterPosition = serializedObject.FindProperty ("debugLaunchCharacterPosition");
		IKDrivingManager = serializedObject.FindProperty ("IKDrivingManager");
		weaponsManager = serializedObject.FindProperty ("weaponsManager");
		mainRigidbody = serializedObject.FindProperty ("mainRigidbody");
		damageInScreenManager = serializedObject.FindProperty ("damageInScreenManager");
		mapInformationManager = serializedObject.FindProperty ("mapInformationManager");
		vehicleCameraManager = serializedObject.FindProperty ("vehicleCameraManager");
		gasTankManager = serializedObject.FindProperty ("gasTankManager");
		vehicleGravitymanager = serializedObject.FindProperty ("vehicleGravitymanager");
		mainVehicleController = serializedObject.FindProperty ("mainVehicleController");

		healthStatName = serializedObject.FindProperty ("healthStatName");
		energyStatName = serializedObject.FindProperty ("energyStatName");
		fuelStatName = serializedObject.FindProperty ("fuelStatName");

		vehicleHUD = (vehicleHUDManager)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (vehicleHUD.advancedSettings.showGizmo) {
				style.normal.textColor = vehicleHUD.advancedSettings.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				for (int i = 0; i < vehicleHUD.advancedSettings.damageReceiverList.Count; i++) {
					if (vehicleHUD.advancedSettings.damageReceiverList [i].spotTransform != null) {
						string label = vehicleHUD.advancedSettings.damageReceiverList [i].name;
						if (vehicleHUD.advancedSettings.damageReceiverList [i].killedWithOneShoot) {
							if (vehicleHUD.advancedSettings.damageReceiverList [i].needMinValueToBeKilled) {
								label += "\nOne Shoot\n >=" + vehicleHUD.advancedSettings.damageReceiverList [i].minValueToBeKilled;
							} else {
								label += "\nOne Shoot";	
							}
						} else {
							label += "\nx" + vehicleHUD.advancedSettings.damageReceiverList [i].damageMultiplier;
						}

						Handles.Label (vehicleHUD.advancedSettings.damageReceiverList [i].spotTransform.position, label, style);	
					}
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Vehicle State", "window");
		GUILayout.Label ("Is Being Driven\t\t" + isBeingDriven.boolValue);
		GUILayout.Label ("Passengers Inside\t\t" + passengersOnVehicle.boolValue);
		GUILayout.Label ("Destroyed\t\t\t" + destroyed.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("UPDATE VEHICLE PARTS\n (PRESS WHEN YOU ADD OR REMOVE ELEMENTS TO THE VEHICLE)")) {
			if (!Application.isPlaying) {
				vehicleHUD.setVehicleParts ();
			}
		}

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.BeginHorizontal ();

		if (showMainSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Main")) {
			showMainSettings.boolValue = !showMainSettings.boolValue;
		}

		if (showVehicleStats.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Stats")) {
			showVehicleStats.boolValue = !showVehicleStats.boolValue;
		}

		if (showAdvancedSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Advanced")) {
			showAdvancedSettings.boolValue = !showAdvancedSettings.boolValue;
		}

		if (showPhysicsSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Physics")) {
			showPhysicsSettings.boolValue = !showPhysicsSettings.boolValue;
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();

		if (showEventSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Events")) {
			showEventSettings.boolValue = !showEventSettings.boolValue;
		}

		if (showOtherSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Others")) {
			showOtherSettings.boolValue = !showOtherSettings.boolValue;
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

			showMainSettings.boolValue = showAllSettings.boolValue;
			showVehicleStats.boolValue = showAllSettings.boolValue;
			showAdvancedSettings.boolValue = showAllSettings.boolValue;
			showEventSettings.boolValue = showAllSettings.boolValue;
			showOtherSettings.boolValue = showAllSettings.boolValue;
			showPhysicsSettings.boolValue = showAllSettings.boolValue;
			showDebugSettings.boolValue = showAllSettings.boolValue;

			showVehicleElements.boolValue = false;
		}

		if (showVehicleElements.boolValue) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Vehicle Elements";
		} else {
			GUI.backgroundColor = buttonColor;
			buttonMessage = "Show Vehicle Elements";
		}
		if (GUILayout.Button (buttonMessage)) {
			showVehicleElements.boolValue = !showVehicleElements.boolValue;
		}
		GUI.backgroundColor = buttonColor;

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 30;
		style.alignment = TextAnchor.MiddleCenter;

		if (showAllSettings.boolValue || showMainSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("MAIN SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window");
			EditorGUILayout.PropertyField (vehicleName);
			EditorGUILayout.PropertyField (destroyedSound);
			EditorGUILayout.PropertyField (destroyedSource);
			EditorGUILayout.PropertyField (healthPercentageDamageParticles);
			EditorGUILayout.PropertyField (placeToShoot);
			EditorGUILayout.PropertyField (canSetTurnOnState);
			EditorGUILayout.PropertyField (autoTurnOnWhenGetOn);
			EditorGUILayout.PropertyField (vehicleRadius);
			EditorGUILayout.PropertyField (layer);
			EditorGUILayout.PropertyField (layerForPassengers);
			EditorGUILayout.PropertyField (passengersParent);
			EditorGUILayout.PropertyField (canUnlockCursor);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("HUD Settings", "window");
			EditorGUILayout.PropertyField (showVehicleHUD);	
			if (showVehicleHUD.boolValue) {
				EditorGUILayout.PropertyField (showVehicleSpeed);		
			}
			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showVehicleStats.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("STATS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Health Settings", "window");
			EditorGUILayout.PropertyField (invincible);

			if (!invincible.boolValue) {
				EditorGUILayout.PropertyField (healthAmount);
				EditorGUILayout.PropertyField (maxHealthAmount);

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Regenerate Health Settings", "window");
				EditorGUILayout.PropertyField (regenerateHealth);
				if (regenerateHealth.boolValue) {
					EditorGUILayout.PropertyField (constantHealthRegenerate);
					EditorGUILayout.PropertyField (regenerateHealthTime);
					if (constantHealthRegenerate.boolValue) {
						EditorGUILayout.PropertyField (regenerateHealthSpeed);
					} else {
						EditorGUILayout.PropertyField (regenerateHealthAmount);
					}
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (destroyed);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Energy Settings", "window");
			EditorGUILayout.PropertyField (vehicleUseBoost);

			if (vehicleUseBoost.boolValue) {
				EditorGUILayout.PropertyField (infiniteBoost);
				if (!infiniteBoost.boolValue) {

					EditorGUILayout.PropertyField (boostAmount);
					EditorGUILayout.PropertyField (maxBoostAmount);

					EditorGUILayout.Space ();

					EditorGUILayout.PropertyField (boostUseRate);

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Regenerate Boost Settings", "window");
					EditorGUILayout.PropertyField (regenerateBoost);
					if (regenerateBoost.boolValue) {
						EditorGUILayout.PropertyField (constantBoostRegenerate);
						EditorGUILayout.PropertyField (regenerateBoostTime);
						if (constantBoostRegenerate.boolValue) {
							EditorGUILayout.PropertyField (regenerateBoostSpeed);
						} else {
							EditorGUILayout.PropertyField (regenerateBoostAmount);
						}
					}
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Fuel Settings", "window");

			EditorGUILayout.PropertyField (vehicleUseFuel);

			if (vehicleUseFuel.boolValue) {
				EditorGUILayout.PropertyField (infiniteFuel);

				if (!infiniteFuel.boolValue) {
					EditorGUILayout.PropertyField (fuelAmount);
					EditorGUILayout.PropertyField (maxFuelAmount);

					EditorGUILayout.Space ();

					EditorGUILayout.PropertyField (fuelUseRate);

					EditorGUILayout.Space ();

					EditorGUILayout.PropertyField (startWithGasTankEmpty);

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Regenerate Fuel Settings", "window");
					EditorGUILayout.PropertyField (regenerateFuel);
					if (regenerateFuel.boolValue) {
						EditorGUILayout.PropertyField (constantFuelRegenerate);
						EditorGUILayout.PropertyField (regenerateFuelTime);
						if (constantFuelRegenerate.boolValue) {
							EditorGUILayout.PropertyField (regenerateFuelSpeed);
						} else {
							EditorGUILayout.PropertyField (regenerateFuelAmount);
						}
					}
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();

					EditorGUILayout.PropertyField (gasTankGameObject);
				}
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Stats Settings", "window");

			EditorGUILayout.PropertyField (healthStatName);
			EditorGUILayout.PropertyField (energyStatName);
			EditorGUILayout.PropertyField (fuelStatName);

			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showAdvancedSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("ADVANCED SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Check all the damage receivers in this vehicle", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Damage Receiver List", "window");
			EditorGUILayout.PropertyField (useWeakSpots);

			EditorGUILayout.Space ();

			showDamageReceivers (damageReceiverList);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Use Custom Vehicle Parts To Destroy Settings", "window");
			EditorGUILayout.PropertyField (useCustomVehiclePartsToDestroy);

			if (useCustomVehiclePartsToDestroy.boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (customVehiclePartsExplosionForce);
				EditorGUILayout.PropertyField (customVehiclePartsExplosionRadius);
				EditorGUILayout.PropertyField (customVehiclePartsExplosionForceMode);

				EditorGUILayout.Space ();

				showCustomVehiclePartsListToDestroy (customVehiclePartsListToDestroy);

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (useEventOnCustomVehicleParts);

				EditorGUILayout.Space ();

				if (useEventOnCustomVehicleParts.boolValue) {
					EditorGUILayout.PropertyField (eventOnCustomVehicleParts);
				}				
			} 

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vehicle Parts To Ignore Settings", "window");
			showSimpleList (vehiclePartsToIgnore);
			GUILayout.EndVertical ();
		}
			
		if (showAllSettings.boolValue || showPhysicsSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("PHYSICS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Damage and Fade Vehicle Pieces Settings", "window");
			EditorGUILayout.PropertyField (useDamageParticles);
			if (useDamageParticles.boolValue) {
				EditorGUILayout.PropertyField (damageParticles);
			}

			EditorGUILayout.PropertyField (useDestroyedParticles);
			if (useDestroyedParticles.boolValue) {
				EditorGUILayout.PropertyField (destroyedParticles);
			}

			EditorGUILayout.PropertyField (removePiecesWhenDestroyed);
			EditorGUILayout.PropertyField (fadeVehiclePiecesOnDestroyed);
			EditorGUILayout.PropertyField (timeToFadePieces);
			EditorGUILayout.PropertyField (destroyedMeshShader);

			EditorGUILayout.PropertyField (setCollidersOnAllVehicleMeshParts);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Self Destruction Settings", "window");
			EditorGUILayout.PropertyField (canUseSelfDestruct);
			if (canUseSelfDestruct.boolValue) {
				EditorGUILayout.PropertyField (canStopSelfDestruction);
				EditorGUILayout.PropertyField (selfDestructDelay);
				EditorGUILayout.PropertyField (ejectPassengerOnSelfDestruct);	
				if (ejectPassengerOnSelfDestruct.boolValue) {
					EditorGUILayout.PropertyField (ejectPassengerFoce);	
				} else {
					EditorGUILayout.PropertyField (getOffPassengersOnSelfDestruct);	
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Collision Damage Settings", "window");
			EditorGUILayout.PropertyField (damageObjectsOnCollision);
			if (damageObjectsOnCollision.boolValue) {
				EditorGUILayout.PropertyField (damageMultiplierOnCollision);
				EditorGUILayout.PropertyField (minVehicleVelocityToDamage);
				EditorGUILayout.PropertyField (minCollisionVelocityToDamage);
				EditorGUILayout.PropertyField (ignoreShieldOnCollision);


				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Remote Event Settings", "window");
				EditorGUILayout.PropertyField (useRemoteEventOnObjectsFound);
				if (useRemoteEventOnObjectsFound.boolValue) {
					EditorGUILayout.Space ();

					showSimpleList (remoteEventNameList);
				}
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Launch Passengers On Collision Settings", "window");
			EditorGUILayout.PropertyField (launchDriverOnCollision);
			if (launchDriverOnCollision.boolValue) {
				EditorGUILayout.PropertyField (minCollisionVelocityToLaunch);
				EditorGUILayout.PropertyField (launchDirectionOffset);
				EditorGUILayout.PropertyField (extraCollisionForce);
				EditorGUILayout.PropertyField (ignoreRagdollCollider);

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Collision Damage Settings", "window");
				EditorGUILayout.PropertyField (applyDamageToDriver);
				if (applyDamageToDriver.boolValue) {
					EditorGUILayout.PropertyField (useCollisionVelocityAsDamage);
					if (!useCollisionVelocityAsDamage.boolValue) {
						EditorGUILayout.PropertyField (collisionDamageAmount);
					}
					EditorGUILayout.PropertyField (ignoreShieldOnLaunch);
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vehicle Explosion Settings", "window");
			EditorGUILayout.PropertyField (vehicleExplosionForce);
			EditorGUILayout.PropertyField (vehicleExplosionRadius);
			EditorGUILayout.PropertyField (vehicleExplosionForceMode);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Damage On Collision Settings", "window");
			EditorGUILayout.PropertyField (receiveDamageFromCollision);
			if (receiveDamageFromCollision.boolValue) {
				EditorGUILayout.PropertyField (minVelocityToDamageCollision);
				EditorGUILayout.PropertyField (useCurrentVelocityAsDamage);
				if (!useCurrentVelocityAsDamage.boolValue) {
					EditorGUILayout.PropertyField (defaultDamageCollision);
				}
				EditorGUILayout.PropertyField (collisionDamageMultiplier);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Impact Surface Settings", "window");
			EditorGUILayout.PropertyField (useImpactSurface);
			EditorGUILayout.PropertyField (mainDecalManagerName);

			EditorGUILayout.Space ();

			if (useImpactSurface.boolValue) {
				if (impactDecalList.arraySize > 0) {
					impactDecalIndex.intValue = EditorGUILayout.Popup ("Decal Impact Type",
						impactDecalIndex.intValue, vehicleHUD.impactDecalList);
					impactDecalName.stringValue = vehicleHUD.impactDecalList [impactDecalIndex.intValue];
				}

				EditorGUILayout.Space ();
					
				if (GUILayout.Button ("Update Decal Impact List")) {
					vehicleHUD.getImpactListInfo ();					
				}

			}
			GUILayout.EndVertical ();
		}

		if (showAllSettings.boolValue || showEventSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("EVENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Events On Vehicle State Changed", "window");
			EditorGUILayout.PropertyField (useEventsOnStateChanged);
			if (useEventsOnStateChanged.boolValue) {
				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventOnGetOn);
				EditorGUILayout.PropertyField (eventOnGetOff);
				EditorGUILayout.PropertyField (eventOnDestroyed);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Jump Platforms Events", "window");
			EditorGUILayout.PropertyField (useJumpPlatformEvents);
			if (useJumpPlatformEvents.boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (jumpPlatformEvent);
				EditorGUILayout.PropertyField (jumpPlatformParableEvent);
				EditorGUILayout.PropertyField (setNewJumpPowerEvent);
				EditorGUILayout.PropertyField (setOriginalJumpPowerEvent);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vehicle Driving State Events", "window");
			EditorGUILayout.PropertyField (passengerGettingOnOffEvent);
			EditorGUILayout.PropertyField (changeVehicleStateEvent);
			GUILayout.EndVertical ();


			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vehicle AI Events", "window");
			EditorGUILayout.PropertyField (useEventsToCheckIfVehicleUsedByAIOnPassengerEnterExit);
			if (useEventsToCheckIfVehicleUsedByAIOnPassengerEnterExit.boolValue) {
				EditorGUILayout.PropertyField (eventToCheckIfVehicleUsedByAIOnPassengerEnter);
				EditorGUILayout.PropertyField (eventToCheckIfVehicleUsedByAIOnPassengerExit);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Events To Send Passengers Settings", "window");
			EditorGUILayout.PropertyField (useEventToSendPassenger);
			if (useEventToSendPassenger.boolValue) {
				EditorGUILayout.PropertyField (eventToSendPassengerOnGetOn);
				EditorGUILayout.PropertyField (eventToSentPassengerOnGetOff);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Events To Send Stats On Save", "window");
		
			EditorGUILayout.PropertyField (eventToSendCurrentHealthAmount);
			EditorGUILayout.PropertyField (eventToSendCurrentFuelAmount);
			EditorGUILayout.PropertyField (eventToSendCurrentBoostAmount);

			GUILayout.EndVertical ();
		}
			
		if (showAllSettings.boolValue || showOtherSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("OTHERS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Horn/Friends Settings", "window");
			EditorGUILayout.PropertyField (useHornToCallFriends);
			if (useHornToCallFriends.boolValue) {
				EditorGUILayout.PropertyField (callOnlyFoundFriends);
				if (!callOnlyFoundFriends.boolValue) {
					EditorGUILayout.PropertyField (radiusToCallFriends);
				}
			}
			EditorGUILayout.PropertyField (useHornEvent);
			if (useHornEvent.boolValue) {
				EditorGUILayout.PropertyField (hornEvent);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Eject Settings", "window");
			EditorGUILayout.PropertyField (canEjectFromVehicle);
			if (canEjectFromVehicle.boolValue) {
				EditorGUILayout.PropertyField (ejectPassengerFoce);	
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Audio Source List", "window");
			showAudioSourceList (audioSourceList);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("AI Settings", "window");
			EditorGUILayout.PropertyField (usedByAI);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window");
			EditorGUILayout.PropertyField (showGizmo);
			if (showGizmo.boolValue) {
				EditorGUILayout.PropertyField (gizmoLabelColor);
				EditorGUILayout.PropertyField (gizmoRadius);
				EditorGUILayout.PropertyField (alphaColor);
			}
			EditorGUILayout.PropertyField (showDebugLogCollisions);
			GUILayout.EndVertical ();
		}
	
		if (showAllSettings.boolValue || showDebugSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("DEBUG SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Debug Settings", "window");
			if (GUILayout.Button ("Destroy Vehicle (In-Game Only)")) {
				if (Application.isPlaying) {
					vehicleHUD.killByButton ();
				}
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (healthAmountToTakeOnEditor);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Apply X Damage (In-Game Only)")) {
				if (Application.isPlaying) {
					vehicleHUD.takeHealth (healthAmountToTakeOnEditor.floatValue);
				}
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Debug Launch Driver", "window");
			EditorGUILayout.PropertyField (debugLaunchCharacterSpeed);
			EditorGUILayout.PropertyField (debugLaunchCharacterPosition);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Launch Player")) {
				if (Application.isPlaying) {
					vehicleHUD.debugLaunchCharacterOnVehicleCollision ();
				}
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
			
			GUILayout.EndVertical ();
		}

		if (showVehicleElements.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("VEHICLE ELEMENTS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vehicle Elements", "window");
			EditorGUILayout.PropertyField (IKDrivingManager);
			EditorGUILayout.PropertyField (weaponsManager);
			EditorGUILayout.PropertyField (mainRigidbody);
			EditorGUILayout.PropertyField (damageInScreenManager);
			EditorGUILayout.PropertyField (mapInformationManager);
			EditorGUILayout.PropertyField (vehicleCameraManager);
			EditorGUILayout.PropertyField (gasTankManager);
			EditorGUILayout.PropertyField (vehicleGravitymanager);
			EditorGUILayout.PropertyField (mainVehicleController);
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("spotTransform"));
		if (!useWeakSpots.boolValue || !list.FindPropertyRelative ("killedWithOneShoot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageMultiplier"));
		}
		if (useWeakSpots.boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("killedWithOneShoot"));
			if (list.FindPropertyRelative ("killedWithOneShoot").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("needMinValueToBeKilled"));
				if (list.FindPropertyRelative ("needMinValueToBeKilled").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("minValueToBeKilled"));
				}
			}
		} 

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Function When Damage Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendFunctionWhenDamage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendFunctionWhenDie"));
		if (list.FindPropertyRelative ("sendFunctionWhenDamage").boolValue || list.FindPropertyRelative ("sendFunctionWhenDie").boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageFunction"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Health Amount On Spot Settings", "window");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHealthAmountOnSpot"));
		if (list.FindPropertyRelative ("useHealthAmountOnSpot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("healhtAmountOnSpot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnEmtpyHealthAmountOnSpot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("killCharacterOnEmtpyHealthAmountOnSpot"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showDamageReceivers (SerializedProperty list)
	{
		GUILayout.BeginVertical ();
	
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Weak Spots: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Get List")) {
				vehicleHUD.getAllDamageReceivers ();
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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showListElementInfo (list.GetArrayElementAtIndex (i), true);
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

	void showAudioSourceElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSourceName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSource"));
		GUILayout.EndVertical ();
	}

	void showAudioSourceList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Audios: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Audio")) {
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
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showAudioSourceElement (list.GetArrayElementAtIndex (i));
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
			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}

	void showCustomVehiclePartsListToDestroy (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Parts: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Part")) {
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
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showCustomVehiclePartsListToDestroyElement (list.GetArrayElementAtIndex (i));
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
			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}

	void showCustomVehiclePartsListToDestroyElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("vehiclePartGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("addCollider"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("mainRigidbody"));
		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Elements: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Element")) {
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
}
#endif