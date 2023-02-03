using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(gravitySystem))]
public class gravitySystemEditor : Editor
{
	SerializedProperty gravityPowerEnabled;
	SerializedProperty gravityPowerInputEnabled;
	SerializedProperty liftToSearchEnabled;
	SerializedProperty randomRotationOnAirEnabled;
	SerializedProperty layer;
	SerializedProperty searchSurfaceSpeed;
	SerializedProperty airControlSpeed;
	SerializedProperty accelerateSpeed;
	SerializedProperty highGravityMultiplier;
	SerializedProperty hoverSpeed;
	SerializedProperty hoverAmount;
	SerializedProperty hoverSmooth;
	SerializedProperty rotateToSurfaceSpeed;
	SerializedProperty rotateToRegularGravitySpeed;
	SerializedProperty preserveVelocityWhenDisableGravityPower;

	SerializedProperty searchNewSurfaceOnHighFallSpeed;
	SerializedProperty minSpeedToSearchNewSurface;

	SerializedProperty pauseSearchNewSurfaceOnHighFallSpeedOnReverseGravityInput;

	SerializedProperty shakeCameraOnHighFallSpeed;
	SerializedProperty minSpeedToShakeCamera;
	SerializedProperty checkSurfaceBelowOnRegularState;
	SerializedProperty timeToSetNullParentOnAir;
	SerializedProperty stopAimModeWhenSearchingSurface;
	SerializedProperty checkGravityArrowStateActive;
	SerializedProperty currentNormal;
	SerializedProperty circumnavigateRotationSpeed;
	SerializedProperty useLerpRotation;
	SerializedProperty tagForCircumnavigate;
	SerializedProperty tagForMovingObjects;
	SerializedProperty checkCircumnavigateSurfaceOnZeroGravity;
	SerializedProperty checkSurfaceBelowLedge;
	SerializedProperty surfaceBelowLedgeRaycastDistance;
	SerializedProperty belowLedgeRotationSpeed;
	SerializedProperty surfaceBelowRaycastTransform;
	SerializedProperty checkSurfaceInFront;
	SerializedProperty surfaceInFrontRaycastDistance;
	SerializedProperty surfaceInFrontRotationSpeed;
	SerializedProperty surfaceInFrontRaycastTransform;
	SerializedProperty gravityAdherenceRaycastParent;
	SerializedProperty useEventsOnUseGravityPowerStateChange;
	SerializedProperty eventsOnUseGravityPowerStateEnabled;
	SerializedProperty eventsOnUseGravityPowerStateDisabled;
	SerializedProperty startWithZeroGravityMode;
	SerializedProperty canResetRotationOnZeroGravityMode;
	SerializedProperty canAdjustToForwardSurface;
	SerializedProperty forwardSurfaceRayPosition;
	SerializedProperty maxDistanceToAdjust;
	SerializedProperty resetRotationZeroGravitySpeed;
	SerializedProperty adjustToForwardSurfaceSpeed;
	SerializedProperty useEventsOnZeroGravityModeStateChange;
	SerializedProperty evenOnZeroGravityModeStateEnabled;
	SerializedProperty eventOnZeroGravityModeStateDisabled;
	SerializedProperty canActivateFreeFloatingMode;
	SerializedProperty useEventsOnFreeFloatingModeStateChange;
	SerializedProperty evenOnFreeFloatingModeStateEnabled;
	SerializedProperty eventOnFreeFloatingModeStateDisabled;
	SerializedProperty startWithNewGravity;
	SerializedProperty usePlayerRotation;
	SerializedProperty adjustRotationToSurfaceFound;
	SerializedProperty newGravityToStart;
	SerializedProperty gravityCenter;
	SerializedProperty cursor;
	SerializedProperty arrow;
	SerializedProperty playerRenderer;
	SerializedProperty changeModelColor;
	SerializedProperty materialToChange;
	SerializedProperty powerColor;
	SerializedProperty debugGravityDirection;
	SerializedProperty playerCameraGameObject;
	SerializedProperty pivotCameraTransform;
	SerializedProperty gravityCenterCollider;
	SerializedProperty playerControllerManager;
	SerializedProperty powers;
	SerializedProperty playerCollider;
	SerializedProperty playerInput;
	SerializedProperty weaponsManager;
	SerializedProperty mainRigidbody;
	SerializedProperty playerCameraManager;
	SerializedProperty mainCameraTransform;
	SerializedProperty grabObjectsManager;
	SerializedProperty usedByAI;
	SerializedProperty gravityPowerActive;
	SerializedProperty powerActivated;
	SerializedProperty choosingDirection;
	SerializedProperty recalculatingSurface;
	SerializedProperty searchingSurface;
	SerializedProperty searchingNewSurfaceBelow;
	SerializedProperty searchAround;
	SerializedProperty firstPersonView;
	SerializedProperty zeroGravityModeOn;
	SerializedProperty circumnavigateCurrentSurfaceActive;
	SerializedProperty freeFloatingModeOn;
	SerializedProperty hovering;
	SerializedProperty turning;


	SerializedProperty raycastDistanceToCheckBelowPlayer;

	SerializedProperty useInfiniteRaycastDistanceToCheckBelowPlayer;

	SerializedProperty showCircumnavigationhSettings;
	SerializedProperty showZeroGravitySettings;
	SerializedProperty showFreeFloatingModeSettings;
	SerializedProperty showEventsSettings;
	SerializedProperty showOtherSettings;
	SerializedProperty showDebugSettings;
	SerializedProperty showAllSettings;
	SerializedProperty showComponents;

	SerializedProperty showGizmo;

	Color buttonColor;
	gravitySystem manager;

	string buttonMessage;

	GUIStyle style = new GUIStyle ();

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		gravityPowerEnabled = serializedObject.FindProperty ("gravityPowerEnabled");
		gravityPowerInputEnabled = serializedObject.FindProperty ("gravityPowerInputEnabled");
		liftToSearchEnabled = serializedObject.FindProperty ("liftToSearchEnabled");
		randomRotationOnAirEnabled = serializedObject.FindProperty ("randomRotationOnAirEnabled");
		layer = serializedObject.FindProperty ("layer");
		searchSurfaceSpeed = serializedObject.FindProperty ("searchSurfaceSpeed");
		airControlSpeed = serializedObject.FindProperty ("airControlSpeed");
		accelerateSpeed = serializedObject.FindProperty ("accelerateSpeed");
		highGravityMultiplier = serializedObject.FindProperty ("highGravityMultiplier");
		hoverSpeed = serializedObject.FindProperty ("hoverSpeed");
		hoverAmount = serializedObject.FindProperty ("hoverAmount");
		hoverSmooth = serializedObject.FindProperty ("hoverSmooth");
		rotateToSurfaceSpeed = serializedObject.FindProperty ("rotateToSurfaceSpeed");
		rotateToRegularGravitySpeed = serializedObject.FindProperty ("rotateToRegularGravitySpeed");
		preserveVelocityWhenDisableGravityPower = serializedObject.FindProperty ("preserveVelocityWhenDisableGravityPower");

		searchNewSurfaceOnHighFallSpeed = serializedObject.FindProperty ("searchNewSurfaceOnHighFallSpeed");
		minSpeedToSearchNewSurface = serializedObject.FindProperty ("minSpeedToSearchNewSurface");

		pauseSearchNewSurfaceOnHighFallSpeedOnReverseGravityInput = serializedObject.FindProperty ("pauseSearchNewSurfaceOnHighFallSpeedOnReverseGravityInput");
	
		shakeCameraOnHighFallSpeed = serializedObject.FindProperty ("shakeCameraOnHighFallSpeed");
		minSpeedToShakeCamera = serializedObject.FindProperty ("minSpeedToShakeCamera");
		checkSurfaceBelowOnRegularState = serializedObject.FindProperty ("checkSurfaceBelowOnRegularState");
		timeToSetNullParentOnAir = serializedObject.FindProperty ("timeToSetNullParentOnAir");
		stopAimModeWhenSearchingSurface = serializedObject.FindProperty ("stopAimModeWhenSearchingSurface");
		checkGravityArrowStateActive = serializedObject.FindProperty ("checkGravityArrowStateActive");
		currentNormal = serializedObject.FindProperty ("currentNormal");
		circumnavigateRotationSpeed = serializedObject.FindProperty ("circumnavigateRotationSpeed");
		useLerpRotation = serializedObject.FindProperty ("useLerpRotation");
		tagForCircumnavigate = serializedObject.FindProperty ("tagForCircumnavigate");
		tagForMovingObjects = serializedObject.FindProperty ("tagForMovingObjects");
		checkCircumnavigateSurfaceOnZeroGravity = serializedObject.FindProperty ("checkCircumnavigateSurfaceOnZeroGravity");
		checkSurfaceBelowLedge = serializedObject.FindProperty ("checkSurfaceBelowLedge");
		surfaceBelowLedgeRaycastDistance = serializedObject.FindProperty ("surfaceBelowLedgeRaycastDistance");
		belowLedgeRotationSpeed = serializedObject.FindProperty ("belowLedgeRotationSpeed");
		surfaceBelowRaycastTransform = serializedObject.FindProperty ("surfaceBelowRaycastTransform");
		checkSurfaceInFront = serializedObject.FindProperty ("checkSurfaceInFront");
		surfaceInFrontRaycastDistance = serializedObject.FindProperty ("surfaceInFrontRaycastDistance");
		surfaceInFrontRotationSpeed = serializedObject.FindProperty ("surfaceInFrontRotationSpeed");
		surfaceInFrontRaycastTransform = serializedObject.FindProperty ("surfaceInFrontRaycastTransform");
		gravityAdherenceRaycastParent = serializedObject.FindProperty ("gravityAdherenceRaycastParent");
		useEventsOnUseGravityPowerStateChange = serializedObject.FindProperty ("useEventsOnUseGravityPowerStateChange");
		eventsOnUseGravityPowerStateEnabled = serializedObject.FindProperty ("eventsOnUseGravityPowerStateEnabled");
		eventsOnUseGravityPowerStateDisabled = serializedObject.FindProperty ("eventsOnUseGravityPowerStateDisabled");
		startWithZeroGravityMode = serializedObject.FindProperty ("startWithZeroGravityMode");
		canResetRotationOnZeroGravityMode = serializedObject.FindProperty ("canResetRotationOnZeroGravityMode");
		canAdjustToForwardSurface = serializedObject.FindProperty ("canAdjustToForwardSurface");
		forwardSurfaceRayPosition = serializedObject.FindProperty ("forwardSurfaceRayPosition");
		maxDistanceToAdjust = serializedObject.FindProperty ("maxDistanceToAdjust");
		resetRotationZeroGravitySpeed = serializedObject.FindProperty ("resetRotationZeroGravitySpeed");
		adjustToForwardSurfaceSpeed = serializedObject.FindProperty ("adjustToForwardSurfaceSpeed");
		useEventsOnZeroGravityModeStateChange = serializedObject.FindProperty ("useEventsOnZeroGravityModeStateChange");
		evenOnZeroGravityModeStateEnabled = serializedObject.FindProperty ("evenOnZeroGravityModeStateEnabled");
		eventOnZeroGravityModeStateDisabled = serializedObject.FindProperty ("eventOnZeroGravityModeStateDisabled");
		canActivateFreeFloatingMode = serializedObject.FindProperty ("canActivateFreeFloatingMode");
		useEventsOnFreeFloatingModeStateChange = serializedObject.FindProperty ("useEventsOnFreeFloatingModeStateChange");
		evenOnFreeFloatingModeStateEnabled = serializedObject.FindProperty ("evenOnFreeFloatingModeStateEnabled");
		eventOnFreeFloatingModeStateDisabled = serializedObject.FindProperty ("eventOnFreeFloatingModeStateDisabled");
		startWithNewGravity = serializedObject.FindProperty ("startWithNewGravity");
		usePlayerRotation = serializedObject.FindProperty ("usePlayerRotation");
		adjustRotationToSurfaceFound = serializedObject.FindProperty ("adjustRotationToSurfaceFound");
		newGravityToStart = serializedObject.FindProperty ("newGravityToStart");
		gravityCenter = serializedObject.FindProperty ("gravityCenter");
		cursor = serializedObject.FindProperty ("cursor");
		arrow = serializedObject.FindProperty ("arrow");
		playerRenderer = serializedObject.FindProperty ("playerRenderer");
		changeModelColor = serializedObject.FindProperty ("changeModelColor");
		materialToChange = serializedObject.FindProperty ("materialToChange");
		powerColor = serializedObject.FindProperty ("powerColor");
		debugGravityDirection = serializedObject.FindProperty ("debugGravityDirection");
		playerCameraGameObject = serializedObject.FindProperty ("playerCameraGameObject");
		pivotCameraTransform = serializedObject.FindProperty ("pivotCameraTransform");
		gravityCenterCollider = serializedObject.FindProperty ("gravityCenterCollider");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		powers = serializedObject.FindProperty ("powers");
		playerCollider = serializedObject.FindProperty ("playerCollider");
		playerInput = serializedObject.FindProperty ("playerInput");

		weaponsManager = serializedObject.FindProperty ("weaponsManager");
		mainRigidbody = serializedObject.FindProperty ("mainRigidbody");
		playerCameraManager = serializedObject.FindProperty ("playerCameraManager");
		mainCameraTransform = serializedObject.FindProperty ("mainCameraTransform");
		grabObjectsManager = serializedObject.FindProperty ("grabObjectsManager");
		usedByAI = serializedObject.FindProperty ("usedByAI");
		gravityPowerActive = serializedObject.FindProperty ("gravityPowerActive");
		powerActivated = serializedObject.FindProperty ("powerActivated");
		choosingDirection = serializedObject.FindProperty ("choosingDirection");
		recalculatingSurface = serializedObject.FindProperty ("recalculatingSurface");
		searchingSurface = serializedObject.FindProperty ("searchingSurface");
		searchingNewSurfaceBelow = serializedObject.FindProperty ("searchingNewSurfaceBelow");
		searchAround = serializedObject.FindProperty ("searchAround");
		firstPersonView = serializedObject.FindProperty ("firstPersonView");
		zeroGravityModeOn = serializedObject.FindProperty ("zeroGravityModeOn");
		circumnavigateCurrentSurfaceActive = serializedObject.FindProperty ("circumnavigateCurrentSurfaceActive");
		freeFloatingModeOn = serializedObject.FindProperty ("freeFloatingModeOn");
		hovering = serializedObject.FindProperty ("hovering");
		turning = serializedObject.FindProperty ("turning");

		raycastDistanceToCheckBelowPlayer = serializedObject.FindProperty ("raycastDistanceToCheckBelowPlayer");

		useInfiniteRaycastDistanceToCheckBelowPlayer = serializedObject.FindProperty ("useInfiniteRaycastDistanceToCheckBelowPlayer");

		showCircumnavigationhSettings = serializedObject.FindProperty ("showCircumnavigationhSettings");
		showZeroGravitySettings = serializedObject.FindProperty ("showZeroGravitySettings");
		showFreeFloatingModeSettings = serializedObject.FindProperty ("showFreeFloatingModeSettings");
		showEventsSettings = serializedObject.FindProperty ("showEventsSettings");
		showOtherSettings = serializedObject.FindProperty ("showOtherSettings");
		showDebugSettings = serializedObject.FindProperty ("showDebugSettings");
		showAllSettings = serializedObject.FindProperty ("showAllSettings");
		showComponents = serializedObject.FindProperty ("showComponents");

		showGizmo = serializedObject.FindProperty ("showGizmo");

		manager = (gravitySystem)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Gravity Settings", "window");
		EditorGUILayout.PropertyField (gravityPowerEnabled);
		EditorGUILayout.PropertyField (gravityPowerInputEnabled);
		EditorGUILayout.PropertyField (liftToSearchEnabled);
		EditorGUILayout.PropertyField (randomRotationOnAirEnabled);
		EditorGUILayout.PropertyField (layer);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (searchSurfaceSpeed);
		EditorGUILayout.PropertyField (airControlSpeed);
		EditorGUILayout.PropertyField (accelerateSpeed);
		EditorGUILayout.PropertyField (highGravityMultiplier);
		EditorGUILayout.PropertyField (rotateToSurfaceSpeed);
		EditorGUILayout.PropertyField (rotateToRegularGravitySpeed);
		EditorGUILayout.PropertyField (preserveVelocityWhenDisableGravityPower);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (raycastDistanceToCheckBelowPlayer);
		EditorGUILayout.PropertyField (useInfiniteRaycastDistanceToCheckBelowPlayer);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Gravity Settings", "window");

		EditorGUILayout.PropertyField (searchNewSurfaceOnHighFallSpeed);
		if (searchNewSurfaceOnHighFallSpeed.boolValue) {
			EditorGUILayout.PropertyField (minSpeedToSearchNewSurface);

			EditorGUILayout.PropertyField (pauseSearchNewSurfaceOnHighFallSpeedOnReverseGravityInput);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (shakeCameraOnHighFallSpeed);
		if (shakeCameraOnHighFallSpeed.boolValue) {
			EditorGUILayout.PropertyField (minSpeedToShakeCamera);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (checkSurfaceBelowOnRegularState);
		if (checkSurfaceBelowOnRegularState.boolValue) {
			EditorGUILayout.PropertyField (timeToSetNullParentOnAir);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (stopAimModeWhenSearchingSurface);
		EditorGUILayout.PropertyField (checkGravityArrowStateActive);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hover Settings", "window");

		EditorGUILayout.PropertyField (hoverSpeed);
		EditorGUILayout.PropertyField (hoverAmount);
		EditorGUILayout.PropertyField (hoverSmooth);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Current Normal Settings", "window");
		EditorGUILayout.PropertyField (currentNormal);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

	
		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.BeginHorizontal ();

		if (showCircumnavigationhSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Circumnavigation")) {
			showCircumnavigationhSettings.boolValue = !showCircumnavigationhSettings.boolValue;
		}

		if (showZeroGravitySettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Zero Gravity")) {
			showZeroGravitySettings.boolValue = !showZeroGravitySettings.boolValue;
		}

		if (showFreeFloatingModeSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Free Floating")) {
			showFreeFloatingModeSettings.boolValue = !showFreeFloatingModeSettings.boolValue;
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

			showCircumnavigationhSettings.boolValue = showAllSettings.boolValue;
			showZeroGravitySettings.boolValue = showAllSettings.boolValue;
			showFreeFloatingModeSettings.boolValue = showAllSettings.boolValue;
			showEventsSettings.boolValue = showAllSettings.boolValue;
			showOtherSettings.boolValue = showAllSettings.boolValue;


			showDebugSettings.boolValue = showAllSettings.boolValue;

			showComponents.boolValue = false;
		}

		if (showComponents.boolValue) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Player Components";
		} else {
			GUI.backgroundColor = buttonColor;
			buttonMessage = "Show Player Components";
		}
		if (GUILayout.Button (buttonMessage)) {
			showComponents.boolValue = !showComponents.boolValue;
		}

		GUI.backgroundColor = buttonColor;

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 30;
		style.alignment = TextAnchor.MiddleCenter;

		if (showAllSettings.boolValue || showCircumnavigationhSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("CIRCUMNAVIGATION SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Circumnavigation Settings", "window");
			EditorGUILayout.PropertyField (circumnavigateRotationSpeed);
			EditorGUILayout.PropertyField (useLerpRotation);
			EditorGUILayout.PropertyField (tagForCircumnavigate);
			EditorGUILayout.PropertyField (tagForMovingObjects);
			EditorGUILayout.PropertyField (checkCircumnavigateSurfaceOnZeroGravity);

			EditorGUILayout.PropertyField (checkSurfaceBelowLedge);
			if (checkSurfaceBelowLedge.boolValue) {
				EditorGUILayout.PropertyField (surfaceBelowLedgeRaycastDistance);
				EditorGUILayout.PropertyField (belowLedgeRotationSpeed);
				EditorGUILayout.PropertyField (surfaceBelowRaycastTransform);
			}
			EditorGUILayout.PropertyField (checkSurfaceInFront);
			if (checkSurfaceInFront.boolValue) {
				EditorGUILayout.PropertyField (surfaceInFrontRaycastDistance);
				EditorGUILayout.PropertyField (surfaceInFrontRotationSpeed);
				EditorGUILayout.PropertyField (surfaceInFrontRaycastTransform);
			}

			EditorGUILayout.PropertyField (gravityAdherenceRaycastParent);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showEventsSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("EVENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Events On Use Gravity Power Settings", "window");	
			EditorGUILayout.PropertyField (useEventsOnUseGravityPowerStateChange);
			if (useEventsOnUseGravityPowerStateChange.boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (eventsOnUseGravityPowerStateEnabled);
				EditorGUILayout.PropertyField (eventsOnUseGravityPowerStateDisabled);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
			
		if (showAllSettings.boolValue || showZeroGravitySettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("ZERO GRAVITY MODE SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Zero Gravity Mode Settings", "window");	
			EditorGUILayout.PropertyField (startWithZeroGravityMode);
			EditorGUILayout.PropertyField (canResetRotationOnZeroGravityMode);	
			EditorGUILayout.PropertyField (canAdjustToForwardSurface);

			if (canAdjustToForwardSurface.boolValue) {
				EditorGUILayout.PropertyField (forwardSurfaceRayPosition);	
				EditorGUILayout.PropertyField (maxDistanceToAdjust);

				EditorGUILayout.PropertyField (resetRotationZeroGravitySpeed);	
				EditorGUILayout.PropertyField (adjustToForwardSurfaceSpeed);
			}
			EditorGUILayout.PropertyField (useEventsOnZeroGravityModeStateChange);
			if (useEventsOnZeroGravityModeStateChange.boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (evenOnZeroGravityModeStateEnabled);
				EditorGUILayout.PropertyField (eventOnZeroGravityModeStateDisabled);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showFreeFloatingModeSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("FREE FLOATING MODE SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Free Floating Mode Settings", "window");	
			EditorGUILayout.PropertyField (canActivateFreeFloatingMode);	
			EditorGUILayout.PropertyField (useEventsOnFreeFloatingModeStateChange);
			if (useEventsOnFreeFloatingModeStateChange.boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (evenOnFreeFloatingModeStateEnabled);
				EditorGUILayout.PropertyField (eventOnFreeFloatingModeStateDisabled);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showOtherSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("OTHERS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("New Gravity At Start Settings", "window");
			EditorGUILayout.PropertyField (startWithNewGravity);
			if (startWithNewGravity.boolValue) {
				EditorGUILayout.PropertyField (usePlayerRotation);
				if (usePlayerRotation.boolValue) {
					EditorGUILayout.PropertyField (adjustRotationToSurfaceFound);
				} else {
					EditorGUILayout.PropertyField (newGravityToStart);
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		
			GUILayout.BeginVertical ("Gravity Color Settings", "window");
			EditorGUILayout.PropertyField (changeModelColor);
			if (changeModelColor.boolValue) {
				showSimpleList (materialToChange);
				EditorGUILayout.PropertyField (powerColor);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("AI Settings", "window");
			EditorGUILayout.PropertyField (usedByAI);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showDebugSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("DEBUG SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Debug Settings", "window");

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (showGizmo);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (debugGravityDirection);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set New Debug Gravity Direction")) {
				if (Application.isPlaying) {
					if (debugGravityDirection.vector3Value != Vector3.zero) {
						manager.changeGravityDirectionDirectly (debugGravityDirection.vector3Value, true);
					}
				}
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set Regular Gravity")) {
				if (Application.isPlaying) {
					manager.deactivateGravityPower ();
				}
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gravity State", "window");
			GUILayout.Label ("Gravity Power Active\t\t" + gravityPowerActive.boolValue);
			GUILayout.Label ("Power Activated\t\t" + powerActivated.boolValue);
			GUILayout.Label ("Choosing Direction\t\t" + choosingDirection.boolValue);
			GUILayout.Label ("Recalculating Surface\t\t" + recalculatingSurface.boolValue);
			GUILayout.Label ("Searching Surface\t\t" + searchingSurface.boolValue);
			GUILayout.Label ("Searching New Surface Below\t" + searchingNewSurfaceBelow.boolValue);
			GUILayout.Label ("Searching Around \t\t" + searchAround.boolValue);
			GUILayout.Label ("First Person \t\t" + firstPersonView.boolValue);
			GUILayout.Label ("Zero Gravity Mode On \t" + zeroGravityModeOn.boolValue);
			GUILayout.Label ("Circumnavigate Surface Active \t" + circumnavigateCurrentSurfaceActive.boolValue);
			GUILayout.Label ("Free Floating Mode On \t" + freeFloatingModeOn.boolValue);
			GUILayout.Label ("Hovering \t\t\t" + hovering.boolValue);
			GUILayout.Label ("Turning \t\t\t" + turning.boolValue);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showComponents.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("PLAYER COMPONENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Elements", "window");
			EditorGUILayout.PropertyField (playerCameraGameObject);
			EditorGUILayout.PropertyField (pivotCameraTransform);
			EditorGUILayout.PropertyField (gravityCenterCollider);
			EditorGUILayout.PropertyField (playerControllerManager);
			EditorGUILayout.PropertyField (powers);
			EditorGUILayout.PropertyField (playerCollider);
			EditorGUILayout.PropertyField (playerInput);

			EditorGUILayout.PropertyField (weaponsManager);
			EditorGUILayout.PropertyField (mainRigidbody);
			EditorGUILayout.PropertyField (playerCameraManager);
			EditorGUILayout.PropertyField (mainCameraTransform);
			EditorGUILayout.PropertyField (grabObjectsManager);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gravity Components", "window");
			EditorGUILayout.PropertyField (gravityCenter);
			EditorGUILayout.PropertyField (cursor);
			EditorGUILayout.PropertyField (arrow);
			EditorGUILayout.PropertyField (playerRenderer);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showSimpleList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space ();
			GUILayout.Label ("Number of Colors: " + list.arraySize);
			EditorGUILayout.Space ();
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Color")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
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
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
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