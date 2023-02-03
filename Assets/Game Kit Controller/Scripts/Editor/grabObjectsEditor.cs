using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(grabObjects))]
public class grabObjectsEditor : Editor
{
	SerializedProperty grabObjectsEnabled;
	SerializedProperty holdDistance;
	SerializedProperty maxDistanceHeld;
	SerializedProperty maxDistanceGrab;
	SerializedProperty holdSpeed;
	SerializedProperty alphaTransparency;
	SerializedProperty closestHoldDistanceInFixedPosition;
	SerializedProperty useCursor;
	SerializedProperty grabInFixedPosition;
	SerializedProperty grabbedObjectTag;
	SerializedProperty grabbedObjectLayer;
	SerializedProperty currentGrabMode;
	SerializedProperty changeGravityObjectsEnabled;
	SerializedProperty useForceWhenObjectDropped;
	SerializedProperty useForceWhenObjectDroppedOnThirdPerson;
	SerializedProperty forceWhenObjectDroppedOnThirdPerson;
	SerializedProperty useForceWhenObjectDroppedOnFirstPerson;
	SerializedProperty forceWhenObjectDroppedOnFirstPerson;
	SerializedProperty pauseCameraMouseWheelWhileObjectGrabbed;
	SerializedProperty grabObjectActionName;
	SerializedProperty extraTextStartActionKey;
	SerializedProperty extraTextEndActionKey;
	SerializedProperty touchButtonIcon;

	SerializedProperty ableToGrabTags;
	SerializedProperty canGrabVehicles;

	SerializedProperty ignoreDropMeleeWeaponIfCarried;

	SerializedProperty useInfiniteStrength;
	SerializedProperty strengthAmount;
	SerializedProperty showCurrentObjectWeight;
	SerializedProperty weightPanel;
	SerializedProperty currentObjectWeightText;
	SerializedProperty regularWeightTextColor;
	SerializedProperty tooHeavyWeightTextColor;
	SerializedProperty playerStatsManager;
	SerializedProperty strengthAmountStatName;
	SerializedProperty grabObjectsPhysicallyEnabled;
	SerializedProperty handInfoList;
	SerializedProperty placeToCarryPhysicalObjectsThirdPerson;
	SerializedProperty placeToCarryPhysicalObjectsFirstPerson;
	SerializedProperty positionToKeepObject;
	SerializedProperty translatePhysicalObjectSpeed;
	SerializedProperty showGrabObjectIconEnabled;
	SerializedProperty keyText;
	SerializedProperty grabObjectIcon;
	SerializedProperty iconRectTransform;
	SerializedProperty getClosestDeviceToCameraCenter;
	SerializedProperty useMaxDistanceToCameraCenter;
	SerializedProperty maxDistanceToCameraCenter;
	SerializedProperty useEventOnCheckIfDropObject;
	SerializedProperty eventOnCheckIfDropObject;
	SerializedProperty useRemoteEventOnObjectsFound;
	SerializedProperty remoteEventNameListOnGrabObject;
	SerializedProperty remoteEventNameListOnDropObject;
	SerializedProperty useObjectToGrabFoundShader;
	SerializedProperty objectToGrabFoundShader;
	SerializedProperty shaderOutlineWidth;
	SerializedProperty shaderOutlineColor;
	SerializedProperty launchedObjectsCanMakeNoise;
	SerializedProperty minObjectSpeedToActivateNoise;
	SerializedProperty objectCanBeRotated;
	SerializedProperty rotationSpeed;
	SerializedProperty rotateSpeed;
	SerializedProperty rotateToCameraInFixedPosition;
	SerializedProperty rotateToCameraInFreePosition;
	SerializedProperty minTimeToIncreaseThrowForce;
	SerializedProperty increaseThrowForceSpeed;
	SerializedProperty extraThorwForce;
	SerializedProperty maxThrowForce;

	SerializedProperty takeObjectMassIntoAccountOnThrowEnabled;
	SerializedProperty objectMassDividerOnThrow;

	SerializedProperty canUseZoomWhileGrabbed;
	SerializedProperty zoomSpeed;
	SerializedProperty maxZoomDistance;
	SerializedProperty minZoomDistance;
	SerializedProperty layer;
	SerializedProperty gravityObjectsLayer;
	SerializedProperty layerForCustomGravityObject;
	SerializedProperty enableTransparency;
	SerializedProperty powerForceMode;
	SerializedProperty useThrowObjectsLayer;
	SerializedProperty throwObjectsLayerToCheck;
	SerializedProperty useGrabbedParticles;
	SerializedProperty useLoadThrowParticles;
	SerializedProperty throwPower;
	SerializedProperty realisticForceMode;
	SerializedProperty objectHeld;
	SerializedProperty aiming;
	SerializedProperty grabbed;
	SerializedProperty gear;
	SerializedProperty rail;
	SerializedProperty regularObject;
	SerializedProperty carryingPhysicalObject;
	SerializedProperty objectIsVehicle;
	SerializedProperty currentPhysicalObjectToGrabFound;
	SerializedProperty objectFocus;
	SerializedProperty currentObjectToGrabFound;
	SerializedProperty physicalObjectToGrabFoundList;
	SerializedProperty usedByAI;

	SerializedProperty grabZoneTransform;
	SerializedProperty cursor;
	SerializedProperty cursorRectTransform;
	SerializedProperty foundObjectToGrabCursor;
	SerializedProperty grabbedObjectCursor;
	SerializedProperty pickableShader;
	SerializedProperty powerSlider;
	SerializedProperty grabbedObjectClonnedColliderTransform;
	SerializedProperty grabbedObjectClonnedCollider;
	SerializedProperty particles;
	SerializedProperty playerCameraTransform;
	SerializedProperty playerControllerManager;
	SerializedProperty powersManager;
	SerializedProperty playerInput;
	SerializedProperty playerCameraManager;
	SerializedProperty usingDevicesManager;
	SerializedProperty weaponsManager;
	SerializedProperty gravityManager;
	SerializedProperty IKManager;
	SerializedProperty mainCollider;
	SerializedProperty mainCameraTransform;
	SerializedProperty mainCamera;
	SerializedProperty highFrictionMaterial;
	SerializedProperty mainGrabbedObjectMeleeAttackSystem;

	SerializedProperty grabObjectsInputPaused;

	SerializedProperty showStrengthSettings;
	SerializedProperty showGrabPhysicalObjectsSettings;
	SerializedProperty showOutlineShaderSettings;
	SerializedProperty showEventsSettings;
	SerializedProperty showOtherSettings;
	SerializedProperty showDebugSettings;
	SerializedProperty showAllSettings;
	SerializedProperty showComponents;
	SerializedProperty showUISettings;

	bool expanded;
	string currentPhysicalObjectToGrabFoundText;

	Color defBackgroundColor;
	string buttonText;

	GUIStyle style = new GUIStyle ();

	Color buttonColor;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		grabObjectsEnabled = serializedObject.FindProperty ("grabObjectsEnabled");
		holdDistance = serializedObject.FindProperty ("holdDistance");
		maxDistanceHeld = serializedObject.FindProperty ("maxDistanceHeld");
		maxDistanceGrab = serializedObject.FindProperty ("maxDistanceGrab");
		holdSpeed = serializedObject.FindProperty ("holdSpeed");
		alphaTransparency = serializedObject.FindProperty ("alphaTransparency");
		closestHoldDistanceInFixedPosition = serializedObject.FindProperty ("closestHoldDistanceInFixedPosition");
		useCursor = serializedObject.FindProperty ("useCursor");
		grabInFixedPosition = serializedObject.FindProperty ("grabInFixedPosition");
		grabbedObjectTag = serializedObject.FindProperty ("grabbedObjectTag");
		grabbedObjectLayer = serializedObject.FindProperty ("grabbedObjectLayer");
		currentGrabMode = serializedObject.FindProperty ("currentGrabMode");
		changeGravityObjectsEnabled = serializedObject.FindProperty ("changeGravityObjectsEnabled");
		useForceWhenObjectDropped = serializedObject.FindProperty ("useForceWhenObjectDropped");
		useForceWhenObjectDroppedOnThirdPerson = serializedObject.FindProperty ("useForceWhenObjectDroppedOnThirdPerson");
		forceWhenObjectDroppedOnThirdPerson = serializedObject.FindProperty ("forceWhenObjectDroppedOnThirdPerson");
		useForceWhenObjectDroppedOnFirstPerson = serializedObject.FindProperty ("useForceWhenObjectDroppedOnFirstPerson");
		forceWhenObjectDroppedOnFirstPerson = serializedObject.FindProperty ("forceWhenObjectDroppedOnFirstPerson");
		pauseCameraMouseWheelWhileObjectGrabbed = serializedObject.FindProperty ("pauseCameraMouseWheelWhileObjectGrabbed");
		grabObjectActionName = serializedObject.FindProperty ("grabObjectActionName");
		extraTextStartActionKey = serializedObject.FindProperty ("extraTextStartActionKey");
		extraTextEndActionKey = serializedObject.FindProperty ("extraTextEndActionKey");
		touchButtonIcon = serializedObject.FindProperty ("touchButtonIcon");

		ableToGrabTags = serializedObject.FindProperty ("ableToGrabTags");
		canGrabVehicles = serializedObject.FindProperty ("canGrabVehicles");

		ignoreDropMeleeWeaponIfCarried = serializedObject.FindProperty ("ignoreDropMeleeWeaponIfCarried");

		useInfiniteStrength = serializedObject.FindProperty ("useInfiniteStrength");
		strengthAmount = serializedObject.FindProperty ("strengthAmount");
		showCurrentObjectWeight = serializedObject.FindProperty ("showCurrentObjectWeight");
		weightPanel = serializedObject.FindProperty ("weightPanel");
		currentObjectWeightText = serializedObject.FindProperty ("currentObjectWeightText");
		regularWeightTextColor = serializedObject.FindProperty ("regularWeightTextColor");
		tooHeavyWeightTextColor = serializedObject.FindProperty ("tooHeavyWeightTextColor");
		playerStatsManager = serializedObject.FindProperty ("playerStatsManager");
		strengthAmountStatName = serializedObject.FindProperty ("strengthAmountStatName");
		grabObjectsPhysicallyEnabled = serializedObject.FindProperty ("grabObjectsPhysicallyEnabled");
		handInfoList = serializedObject.FindProperty ("handInfoList");
		placeToCarryPhysicalObjectsThirdPerson = serializedObject.FindProperty ("placeToCarryPhysicalObjectsThirdPerson");
		placeToCarryPhysicalObjectsFirstPerson = serializedObject.FindProperty ("placeToCarryPhysicalObjectsFirstPerson");
		positionToKeepObject = serializedObject.FindProperty ("positionToKeepObject");
		translatePhysicalObjectSpeed = serializedObject.FindProperty ("translatePhysicalObjectSpeed");
		showGrabObjectIconEnabled = serializedObject.FindProperty ("showGrabObjectIconEnabled");
		keyText = serializedObject.FindProperty ("keyText");
		grabObjectIcon = serializedObject.FindProperty ("grabObjectIcon");
		iconRectTransform = serializedObject.FindProperty ("iconRectTransform");
		getClosestDeviceToCameraCenter = serializedObject.FindProperty ("getClosestDeviceToCameraCenter");
		useMaxDistanceToCameraCenter = serializedObject.FindProperty ("useMaxDistanceToCameraCenter");
		maxDistanceToCameraCenter = serializedObject.FindProperty ("maxDistanceToCameraCenter");
		useEventOnCheckIfDropObject = serializedObject.FindProperty ("useEventOnCheckIfDropObject");
		eventOnCheckIfDropObject = serializedObject.FindProperty ("eventOnCheckIfDropObject");
		useRemoteEventOnObjectsFound = serializedObject.FindProperty ("useRemoteEventOnObjectsFound");
		remoteEventNameListOnGrabObject = serializedObject.FindProperty ("remoteEventNameListOnGrabObject");
		remoteEventNameListOnDropObject = serializedObject.FindProperty ("remoteEventNameListOnDropObject");
		useObjectToGrabFoundShader = serializedObject.FindProperty ("useObjectToGrabFoundShader");
		objectToGrabFoundShader = serializedObject.FindProperty ("objectToGrabFoundShader");
		shaderOutlineWidth = serializedObject.FindProperty ("shaderOutlineWidth");
		shaderOutlineColor = serializedObject.FindProperty ("shaderOutlineColor");
		launchedObjectsCanMakeNoise = serializedObject.FindProperty ("launchedObjectsCanMakeNoise");
		minObjectSpeedToActivateNoise = serializedObject.FindProperty ("minObjectSpeedToActivateNoise");
		objectCanBeRotated = serializedObject.FindProperty ("objectCanBeRotated");
		rotationSpeed = serializedObject.FindProperty ("rotationSpeed");
		rotateSpeed = serializedObject.FindProperty ("rotateSpeed");
		rotateToCameraInFixedPosition = serializedObject.FindProperty ("rotateToCameraInFixedPosition");
		rotateToCameraInFreePosition = serializedObject.FindProperty ("rotateToCameraInFreePosition");
		minTimeToIncreaseThrowForce = serializedObject.FindProperty ("minTimeToIncreaseThrowForce");
		increaseThrowForceSpeed = serializedObject.FindProperty ("increaseThrowForceSpeed");
		extraThorwForce = serializedObject.FindProperty ("extraThorwForce");
		maxThrowForce = serializedObject.FindProperty ("maxThrowForce");

		takeObjectMassIntoAccountOnThrowEnabled = serializedObject.FindProperty ("takeObjectMassIntoAccountOnThrowEnabled");
		objectMassDividerOnThrow = serializedObject.FindProperty ("objectMassDividerOnThrow");

		canUseZoomWhileGrabbed = serializedObject.FindProperty ("canUseZoomWhileGrabbed");
		zoomSpeed = serializedObject.FindProperty ("zoomSpeed");
		maxZoomDistance = serializedObject.FindProperty ("maxZoomDistance");
		minZoomDistance = serializedObject.FindProperty ("minZoomDistance");
		layer = serializedObject.FindProperty ("layer");
		gravityObjectsLayer = serializedObject.FindProperty ("gravityObjectsLayer");
		layerForCustomGravityObject = serializedObject.FindProperty ("layerForCustomGravityObject");
		enableTransparency = serializedObject.FindProperty ("enableTransparency");
		powerForceMode = serializedObject.FindProperty ("powerForceMode");
		useThrowObjectsLayer = serializedObject.FindProperty ("useThrowObjectsLayer");
		throwObjectsLayerToCheck = serializedObject.FindProperty ("throwObjectsLayerToCheck");
		useGrabbedParticles = serializedObject.FindProperty ("useGrabbedParticles");
		useLoadThrowParticles = serializedObject.FindProperty ("useLoadThrowParticles");
		throwPower = serializedObject.FindProperty ("throwPower");
		realisticForceMode = serializedObject.FindProperty ("realisticForceMode");
		objectHeld = serializedObject.FindProperty ("objectHeld");
		aiming = serializedObject.FindProperty ("aiming");
		grabbed = serializedObject.FindProperty ("grabbed");
		gear = serializedObject.FindProperty ("gear");
		rail = serializedObject.FindProperty ("rail");
		regularObject = serializedObject.FindProperty ("regularObject");
		carryingPhysicalObject = serializedObject.FindProperty ("carryingPhysicalObject");
		objectIsVehicle = serializedObject.FindProperty ("objectIsVehicle");
		currentPhysicalObjectToGrabFound = serializedObject.FindProperty ("currentPhysicalObjectToGrabFound");
		objectFocus = serializedObject.FindProperty ("objectFocus");
		currentObjectToGrabFound = serializedObject.FindProperty ("currentObjectToGrabFound");
		physicalObjectToGrabFoundList = serializedObject.FindProperty ("physicalObjectToGrabFoundList");
		usedByAI = serializedObject.FindProperty ("usedByAI");

		grabZoneTransform = serializedObject.FindProperty ("grabZoneTransform");
		cursor = serializedObject.FindProperty ("cursor");
		cursorRectTransform = serializedObject.FindProperty ("cursorRectTransform");
		foundObjectToGrabCursor = serializedObject.FindProperty ("foundObjectToGrabCursor");
		grabbedObjectCursor = serializedObject.FindProperty ("grabbedObjectCursor");
		pickableShader = serializedObject.FindProperty ("pickableShader");
		powerSlider = serializedObject.FindProperty ("powerSlider");
		grabbedObjectClonnedColliderTransform = serializedObject.FindProperty ("grabbedObjectClonnedColliderTransform");
		grabbedObjectClonnedCollider = serializedObject.FindProperty ("grabbedObjectClonnedCollider");
		particles = serializedObject.FindProperty ("particles");
		playerCameraTransform = serializedObject.FindProperty ("playerCameraTransform");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		powersManager = serializedObject.FindProperty ("powersManager");
		playerInput = serializedObject.FindProperty ("playerInput");
		playerCameraManager = serializedObject.FindProperty ("playerCameraManager");
		usingDevicesManager = serializedObject.FindProperty ("usingDevicesManager");
		weaponsManager = serializedObject.FindProperty ("weaponsManager");
		gravityManager = serializedObject.FindProperty ("gravityManager");
		IKManager = serializedObject.FindProperty ("IKManager");
		mainCollider = serializedObject.FindProperty ("mainCollider");
		mainCameraTransform = serializedObject.FindProperty ("mainCameraTransform");
		mainCamera = serializedObject.FindProperty ("mainCamera");
		highFrictionMaterial = serializedObject.FindProperty ("highFrictionMaterial");
		mainGrabbedObjectMeleeAttackSystem = serializedObject.FindProperty ("mainGrabbedObjectMeleeAttackSystem");

		grabObjectsInputPaused = serializedObject.FindProperty ("grabObjectsInputPaused");

		showStrengthSettings = serializedObject.FindProperty ("showStrengthSettings");
		showGrabPhysicalObjectsSettings = serializedObject.FindProperty ("showGrabPhysicalObjectsSettings");
		showOutlineShaderSettings = serializedObject.FindProperty ("showOutlineShaderSettings");
		showEventsSettings = serializedObject.FindProperty ("showEventsSettings");
		showOtherSettings = serializedObject.FindProperty ("showOtherSettings");
		showDebugSettings = serializedObject.FindProperty ("showDebugSettings");
		showAllSettings = serializedObject.FindProperty ("showAllSettings");
		showComponents = serializedObject.FindProperty ("showComponents");
		showUISettings = serializedObject.FindProperty ("showUISettings");
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Grab Settings", "window");
		EditorGUILayout.PropertyField (grabObjectsEnabled);
		EditorGUILayout.PropertyField (holdDistance);
		EditorGUILayout.PropertyField (maxDistanceHeld);
		EditorGUILayout.PropertyField (maxDistanceGrab);
		EditorGUILayout.PropertyField (holdSpeed);

		EditorGUILayout.PropertyField (closestHoldDistanceInFixedPosition);

		EditorGUILayout.PropertyField (grabInFixedPosition);	
		EditorGUILayout.PropertyField (grabbedObjectTag);	
		EditorGUILayout.PropertyField (grabbedObjectLayer);	

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (currentGrabMode);
		if (currentGrabMode.enumValueIndex == 0) {
			EditorGUILayout.PropertyField (changeGravityObjectsEnabled);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useForceWhenObjectDropped);
		if (useForceWhenObjectDropped.boolValue) {
			EditorGUILayout.PropertyField (useForceWhenObjectDroppedOnThirdPerson);
			EditorGUILayout.PropertyField (forceWhenObjectDroppedOnThirdPerson);
			EditorGUILayout.PropertyField (useForceWhenObjectDroppedOnFirstPerson);
			EditorGUILayout.PropertyField (forceWhenObjectDroppedOnFirstPerson);
		}

		EditorGUILayout.PropertyField (pauseCameraMouseWheelWhileObjectGrabbed);

		EditorGUILayout.PropertyField (canGrabVehicles);

		EditorGUILayout.PropertyField (ignoreDropMeleeWeaponIfCarried);
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Able To Grab Tags List", "window");
		showSimpleList (ableToGrabTags, "Tags", true);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();



		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.BeginHorizontal ();

		if (showStrengthSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Strength")) {
			showStrengthSettings.boolValue = !showStrengthSettings.boolValue;
		}

		if (showGrabPhysicalObjectsSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Physical Objects")) {
			showGrabPhysicalObjectsSettings.boolValue = !showGrabPhysicalObjectsSettings.boolValue;
		}

		if (showOutlineShaderSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Outline")) {
			showOutlineShaderSettings.boolValue = !showOutlineShaderSettings.boolValue;
		}

		if (showEventsSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Events")) {
			showEventsSettings.boolValue = !showEventsSettings.boolValue;
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();

		if (showOtherSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Others")) {
			showOtherSettings.boolValue = !showOtherSettings.boolValue;
		}

		if (showUISettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("UI")) {
			showUISettings.boolValue = !showUISettings.boolValue;
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

			showStrengthSettings.boolValue = showAllSettings.boolValue;
			showGrabPhysicalObjectsSettings.boolValue = showAllSettings.boolValue;
			showOutlineShaderSettings.boolValue = showAllSettings.boolValue;
			showEventsSettings.boolValue = showAllSettings.boolValue;
			showOtherSettings.boolValue = showAllSettings.boolValue;

			showUISettings.boolValue = showAllSettings.boolValue;
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

		if (showAllSettings.boolValue || showStrengthSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("STRENGTH SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Strength Settings", "window");
			EditorGUILayout.PropertyField (useInfiniteStrength);
			if (!useInfiniteStrength.boolValue) {
				EditorGUILayout.PropertyField (strengthAmount);

				EditorGUILayout.PropertyField (showCurrentObjectWeight);
				EditorGUILayout.PropertyField (weightPanel);
				EditorGUILayout.PropertyField (currentObjectWeightText);
				EditorGUILayout.PropertyField (regularWeightTextColor);
				EditorGUILayout.PropertyField (tooHeavyWeightTextColor);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Strength Stat Settings", "window");
			EditorGUILayout.PropertyField (playerStatsManager);
			EditorGUILayout.PropertyField (strengthAmountStatName);
			GUILayout.EndVertical (); 

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showGrabPhysicalObjectsSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("PHYSICAL OBJECTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Grab Physical Objects Settings", "window");
			EditorGUILayout.PropertyField (grabObjectsPhysicallyEnabled);
			if (grabObjectsPhysicallyEnabled.boolValue) {
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Hands List", "window");
				showHandsList (handInfoList);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (placeToCarryPhysicalObjectsThirdPerson);
				EditorGUILayout.PropertyField (placeToCarryPhysicalObjectsFirstPerson);
				EditorGUILayout.PropertyField (positionToKeepObject);
				EditorGUILayout.PropertyField (translatePhysicalObjectSpeed);

				EditorGUILayout.PropertyField (showGrabObjectIconEnabled);
				if (showGrabObjectIconEnabled.boolValue) {
					EditorGUILayout.PropertyField (keyText);
					EditorGUILayout.PropertyField (grabObjectIcon);
					EditorGUILayout.PropertyField (iconRectTransform);
				}
				EditorGUILayout.PropertyField (getClosestDeviceToCameraCenter);
				if (getClosestDeviceToCameraCenter.boolValue) {
					EditorGUILayout.PropertyField (useMaxDistanceToCameraCenter);
					EditorGUILayout.PropertyField (maxDistanceToCameraCenter);	
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showOutlineShaderSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("OUTLINE/TRANSPARENCY SHADER SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Outline Shader Device Found Settings", "window");
			EditorGUILayout.PropertyField (useObjectToGrabFoundShader);
			if (useObjectToGrabFoundShader.boolValue) {
				EditorGUILayout.PropertyField (objectToGrabFoundShader);
				EditorGUILayout.PropertyField (shaderOutlineWidth);
				EditorGUILayout.PropertyField (shaderOutlineColor);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Grabbed Object Transparency Settings", "window");
			EditorGUILayout.PropertyField (enableTransparency);
			EditorGUILayout.PropertyField (alphaTransparency);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showEventsSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("EVENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Drop Object Events Settings", "window");
			EditorGUILayout.PropertyField (useEventOnCheckIfDropObject);
			if (useEventOnCheckIfDropObject.boolValue) {
			
				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventOnCheckIfDropObject);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Remove Event Settings", "window");
			EditorGUILayout.PropertyField (useRemoteEventOnObjectsFound);	
			if (useRemoteEventOnObjectsFound.boolValue) {
				showSimpleList (remoteEventNameListOnGrabObject, "Events", true);
				showSimpleList (remoteEventNameListOnDropObject, "Events", true);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showOtherSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("PHYSICAL OBJECTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		
			GUILayout.BeginVertical ("Collision Noises Settings", "window");
			EditorGUILayout.PropertyField (launchedObjectsCanMakeNoise);
			if (launchedObjectsCanMakeNoise.boolValue) {
				EditorGUILayout.PropertyField (minObjectSpeedToActivateNoise);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Rotation Settings", "window");
			EditorGUILayout.PropertyField (objectCanBeRotated);
			EditorGUILayout.PropertyField (rotationSpeed);
			EditorGUILayout.PropertyField (rotateSpeed);
			if (grabInFixedPosition.boolValue) {
				EditorGUILayout.PropertyField (rotateToCameraInFixedPosition);
			} else {
				EditorGUILayout.PropertyField (rotateToCameraInFreePosition);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Throw Settings", "window");
			EditorGUILayout.PropertyField (minTimeToIncreaseThrowForce);
			EditorGUILayout.PropertyField (increaseThrowForceSpeed);
			EditorGUILayout.PropertyField (extraThorwForce);	
			EditorGUILayout.PropertyField (maxThrowForce);		

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (takeObjectMassIntoAccountOnThrowEnabled);	
			if (takeObjectMassIntoAccountOnThrowEnabled.boolValue) {
				EditorGUILayout.PropertyField (objectMassDividerOnThrow);	
			}
		 
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Zoom Settings", "window");
			EditorGUILayout.PropertyField (canUseZoomWhileGrabbed);
			EditorGUILayout.PropertyField (zoomSpeed);
			EditorGUILayout.PropertyField (maxZoomDistance);
			EditorGUILayout.PropertyField (minZoomDistance);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Power Settings", "window");
			EditorGUILayout.PropertyField (layer);
			EditorGUILayout.PropertyField (gravityObjectsLayer);
			EditorGUILayout.PropertyField (layerForCustomGravityObject);
			EditorGUILayout.PropertyField (powerForceMode);
			EditorGUILayout.PropertyField (useThrowObjectsLayer);	
			EditorGUILayout.PropertyField (throwObjectsLayerToCheck);	

			EditorGUILayout.PropertyField (useGrabbedParticles);
			EditorGUILayout.PropertyField (useLoadThrowParticles);

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Realistic Settings", "window");
			EditorGUILayout.PropertyField (throwPower);	
			EditorGUILayout.PropertyField (realisticForceMode);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("AI Settings", "window");
			EditorGUILayout.PropertyField (usedByAI);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showUISettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("UI SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("UI Settings", "window");
			EditorGUILayout.PropertyField (useCursor);
			EditorGUILayout.PropertyField (grabObjectActionName);
			EditorGUILayout.PropertyField (extraTextStartActionKey);
			EditorGUILayout.PropertyField (extraTextEndActionKey);

			EditorGUILayout.PropertyField (touchButtonIcon);

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showDebugSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("DEBUG SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player State", "window");
			if (objectHeld.objectReferenceValue) {
				EditorGUILayout.PropertyField (objectHeld, new GUIContent ("Current Object Held"), false);
			} else {
				GUILayout.Label ("Current Object Held\t\t" + "None");
			}

			GUILayout.Label ("Aiming Active\t\t" + aiming.boolValue);
			GUILayout.Label ("Object Grabbed\t\t" + grabbed.boolValue);
			GUILayout.Label ("Is Gear\t\t\t" + gear.boolValue);
			GUILayout.Label ("Is Rail\t\t\t" + rail.boolValue);
			GUILayout.Label ("Is Regular Object\t\t" + regularObject.boolValue);
			GUILayout.Label ("Is Physical Object\t\t" + carryingPhysicalObject.boolValue);
			GUILayout.Label ("Is Vehicle\t\t\t" + objectIsVehicle.boolValue);
			GUILayout.Label ("Input Paused\t\t" + grabObjectsInputPaused.boolValue);

			EditorGUILayout.Space ();

			if (currentPhysicalObjectToGrabFound.objectReferenceValue != null) {
				currentPhysicalObjectToGrabFoundText = "YES";
			} else {
				currentPhysicalObjectToGrabFoundText = "NO";
			}

			EditorGUILayout.PropertyField (currentPhysicalObjectToGrabFound);

			GUILayout.Label ("Physic Object Found \t\t" + currentPhysicalObjectToGrabFoundText);
			GUILayout.Label ("Object To Grab Found\t\t" + objectFocus.boolValue);
			EditorGUILayout.PropertyField (currentObjectToGrabFound);

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Physical Objects Detected List", "window");
			showSimpleList (physicalObjectToGrabFoundList, "Physical Objects Found List", false);
			GUILayout.EndVertical ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
	
		if (showComponents.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("COMPONENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Grab Elements", "window");
			EditorGUILayout.PropertyField (grabZoneTransform);	
			EditorGUILayout.PropertyField (cursor);	
			EditorGUILayout.PropertyField (cursorRectTransform);

			EditorGUILayout.PropertyField (foundObjectToGrabCursor);

			EditorGUILayout.PropertyField (grabbedObjectCursor);	
			EditorGUILayout.PropertyField (pickableShader);	
			EditorGUILayout.PropertyField (powerSlider);

			EditorGUILayout.PropertyField (grabbedObjectClonnedColliderTransform);	
			EditorGUILayout.PropertyField (grabbedObjectClonnedCollider);

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Particles List", "window");
			showParticlesList (particles);
			GUILayout.EndVertical ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Elements", "window");
			EditorGUILayout.PropertyField (playerCameraTransform);
			EditorGUILayout.PropertyField (playerControllerManager);
			EditorGUILayout.PropertyField (powersManager);
			EditorGUILayout.PropertyField (playerInput);
			EditorGUILayout.PropertyField (playerCameraManager);
			EditorGUILayout.PropertyField (usingDevicesManager);
			EditorGUILayout.PropertyField (weaponsManager);
			EditorGUILayout.PropertyField (gravityManager);
			EditorGUILayout.PropertyField (IKManager);
			EditorGUILayout.PropertyField (mainCollider);
			EditorGUILayout.PropertyField (mainCameraTransform);
			EditorGUILayout.PropertyField (mainCamera);
			EditorGUILayout.PropertyField (highFrictionMaterial);
			EditorGUILayout.PropertyField (mainGrabbedObjectMeleeAttackSystem);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showParticlesList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space ();
			GUILayout.Label ("Number of Particles: " + list.arraySize);
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
				}
			
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list, string listName, bool showEditButtons)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of " + listName + ": " + list.arraySize);

			if (showEditButtons) {
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

				if (showEditButtons) {
					if (GUILayout.Button ("x")) {
						list.DeleteArrayElementAtIndex (i);
					}
				}

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showHandsListElement (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("IKHint"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("IKGoal"));
	}

	void showHandsList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Hands: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Hand")) {
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
						showHandsListElement (list.GetArrayElementAtIndex (i));
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
}
#endif