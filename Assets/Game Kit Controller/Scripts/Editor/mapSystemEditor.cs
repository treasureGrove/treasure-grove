using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(mapSystem))]
public class mapSystemEditor : Editor
{
	SerializedProperty mapEnabled;
	SerializedProperty changeFloorWithTriggers;
	SerializedProperty showWarningMessages;
	SerializedProperty updateMapIconTypesOnStart;
	SerializedProperty currentBuildingName;
	SerializedProperty currentBuildingIndex;
	SerializedProperty currentFloorIndex;
	SerializedProperty currentFloorNumber;
	SerializedProperty currentMapPartIndex;
	SerializedProperty mapCreatorManager;
	SerializedProperty showMapComponents;
	SerializedProperty showMapSettings;
	SerializedProperty showCompassComponents;
	SerializedProperty showCompassSettings;
	SerializedProperty showMapFloorAndIcons;
	SerializedProperty showMarkSettings;
	SerializedProperty mapContent;
	SerializedProperty mapCamera;
	SerializedProperty mapSystemPivotTransform;
	SerializedProperty mapSystemCameraTransform;
	SerializedProperty player;
	SerializedProperty mapMenu;
	SerializedProperty mapWindowTargetPosition;
	SerializedProperty mapRender;
	SerializedProperty mapWindow;
	SerializedProperty playerMapIcon;
	SerializedProperty playerIconChild;
	SerializedProperty removeMarkButtonImage;
	SerializedProperty quickTravelButtonImage;
	SerializedProperty mapObjectNameField;
	SerializedProperty mapObjectInfoField;
	SerializedProperty currentFloorNumberText;
	SerializedProperty useMapIndexWindow;
	SerializedProperty mapIndexWindow;
	SerializedProperty mapIndexWindowContent;
	SerializedProperty mapIndexWindowScroller;
	SerializedProperty mapObjectTextIcon;
	SerializedProperty useBlurUIPanel;
	SerializedProperty mapWindowMask;
	SerializedProperty zoomScrollbar;
	SerializedProperty currentMapZoneText;
	SerializedProperty mapCursor;
	SerializedProperty mapCursorRectTransform;
	SerializedProperty currenMapIconPressed;
	SerializedProperty mapCircleTransform;
	SerializedProperty playerInput;
	SerializedProperty pauseManager;
	SerializedProperty screenObjectivesManager;
	SerializedProperty playerControllerManager;
	SerializedProperty playerMapObjectInformation;
	SerializedProperty mainMapCamera;
	SerializedProperty mapCreatorPrefab;
	SerializedProperty playerIconMovementSpeed;
	SerializedProperty openMapSpeed;
	SerializedProperty mouseDragMapSpeed;
	SerializedProperty keysDragMapSpeed;
	SerializedProperty getClosestFloorToPlayerByDistance;
	SerializedProperty mapCameraMovementType;
	SerializedProperty recenterCameraSpeed;
	SerializedProperty rotateMap;
	SerializedProperty smoothRotationMap;
	SerializedProperty rotationSpeed;
	SerializedProperty usingCircleMap;
	SerializedProperty circleMapRadius;
	SerializedProperty showOffScreenIcons;
	SerializedProperty iconSize;
	SerializedProperty maxIconSize;
	SerializedProperty offScreenIconSize;
	SerializedProperty openMapIconSizeMultiplier;
	SerializedProperty changeIconSizeSpeed;
	SerializedProperty showIconsByFloor;
	SerializedProperty borderOffScreen;
	SerializedProperty useTextInIcons;
	SerializedProperty textIconsOffset;
	SerializedProperty mapObjectTextIconColor;
	SerializedProperty mapObjectTextSize;
	SerializedProperty miniMapWindowEnabledInGame;
	SerializedProperty miniMapWindowSmoothOpening;
	SerializedProperty miniMapWindowWithMask;
	SerializedProperty playerUseMapObjectInformation;
	SerializedProperty playerIconOffset;
	SerializedProperty useCurrentMapIconPressed;
	SerializedProperty useMapCursor;
	SerializedProperty showInfoIconInsideCursor;
	SerializedProperty maxDistanceToMapIcon;
	SerializedProperty zoomWhenOpen;
	SerializedProperty zoomWhenClose;
	SerializedProperty openCloseZoomSpeed;
	SerializedProperty zoomSpeed;
	SerializedProperty maxZoom;
	SerializedProperty minZoom;
	SerializedProperty zoomToActivateIcons;
	SerializedProperty zoomToActivateTextIcons;
	SerializedProperty zoomWhenOpen3d;
	SerializedProperty zoomWhenClose3d;
	SerializedProperty openCloseZoomSpeed3d;
	SerializedProperty zoomSpeed3d;
	SerializedProperty maxZoom3d;
	SerializedProperty minZoom3d;
	SerializedProperty zoomToActivateIcons3d;
	SerializedProperty zoomToActivateTextIcons3d;
	SerializedProperty setColorOnCurrent3dMapPart;
	SerializedProperty colorOnCurrent3dMapPart;
	SerializedProperty disabledRemoveMarkColor;
	SerializedProperty disabledQuickTravelColor;
	SerializedProperty map3dEnabled;
	SerializedProperty map3dPositionSpeed;
	SerializedProperty map3dRotationSpeed;
	SerializedProperty rangeAngleX;
	SerializedProperty rangeAngleY;
	SerializedProperty transtionTo3dSpeed;
	SerializedProperty maxTimeBetweenTransition;
	SerializedProperty reset3dCameraSpeed;
	SerializedProperty inital3dCameraRotation;
	SerializedProperty hideOffscreenIconsOn3dView;
	SerializedProperty showIconsOn3dView;
	SerializedProperty use3dMeshForPlayer;
	SerializedProperty player3dMesh;
	SerializedProperty useEventIfMapEnabled;
	SerializedProperty eventIfMapEnabled;
	SerializedProperty useEventIfMapDisabled;
	SerializedProperty eventIfMapDisabled;
	SerializedProperty markPrefab;
	SerializedProperty setMarkOnCurrenBuilding;
	SerializedProperty setMarkOnCurrentFloor;
	SerializedProperty compassWindow;
	SerializedProperty compassElementsParent;
	SerializedProperty north;
	SerializedProperty south;
	SerializedProperty east;
	SerializedProperty west;
	SerializedProperty northEast;
	SerializedProperty southWest;
	SerializedProperty southEast;
	SerializedProperty northWest;
	SerializedProperty compassDirections;
	SerializedProperty northGameObject;
	SerializedProperty southGameObject;
	SerializedProperty eastGameObject;
	SerializedProperty westGameObject;
	SerializedProperty northEastGameObject;
	SerializedProperty southWestGameObject;
	SerializedProperty southEastGameObject;
	SerializedProperty northwestGameObject;
	SerializedProperty compassEnabled;
	SerializedProperty compassOffset;
	SerializedProperty compassScale;
	SerializedProperty showIntermediateDirections;
	SerializedProperty maximumLeftDistance;
	SerializedProperty maximumRightDistance;
	SerializedProperty usePlayerTransformOrientationOnCompassOnThirdPerson;
	SerializedProperty usePlayerTransformOrientationOnCompassOnFirstPerson;
	SerializedProperty usePlayerTransformOrientationOnCompassOnLockedCamera;
	SerializedProperty buildingList;

	SerializedProperty mainScreenObjectivesManagerName;

	SerializedProperty ingameMenuName;
	SerializedProperty beaconIconTypeName;
	SerializedProperty markIconTypeName;

	Color defBackgroundColor;
	mapSystem mapSystemManager;

	string[] buildingListString;
	int temporalBuildingIndex;

	string[] floorListString;

	int temporalFloorIndex;

	string[] mapPartListString;
	int temporalMapPartIndex;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		mapEnabled = serializedObject.FindProperty ("mapEnabled");
		changeFloorWithTriggers = serializedObject.FindProperty ("changeFloorWithTriggers");
		showWarningMessages = serializedObject.FindProperty ("showWarningMessages");
		updateMapIconTypesOnStart = serializedObject.FindProperty ("updateMapIconTypesOnStart");
		currentBuildingName = serializedObject.FindProperty ("currentBuildingName");
		currentBuildingIndex = serializedObject.FindProperty ("currentBuildingIndex");
		currentFloorIndex = serializedObject.FindProperty ("currentFloorIndex");
		currentFloorNumber = serializedObject.FindProperty ("currentFloorNumber");
		currentMapPartIndex = serializedObject.FindProperty ("currentMapPartIndex");
		mapCreatorManager = serializedObject.FindProperty ("mapCreatorManager");
		showMapComponents = serializedObject.FindProperty ("showMapComponents");
		showMapSettings = serializedObject.FindProperty ("showMapSettings");
		showCompassComponents = serializedObject.FindProperty ("showCompassComponents");
		showCompassSettings = serializedObject.FindProperty ("showCompassSettings");
		showMapFloorAndIcons = serializedObject.FindProperty ("showMapFloorAndIcons");
		showMarkSettings = serializedObject.FindProperty ("showMarkSettings");
		mapContent = serializedObject.FindProperty ("mapContent");
		mapCamera = serializedObject.FindProperty ("mapCamera");
		mapSystemPivotTransform = serializedObject.FindProperty ("mapSystemPivotTransform");
		mapSystemCameraTransform = serializedObject.FindProperty ("mapSystemCameraTransform");
		player = serializedObject.FindProperty ("player");
		mapMenu = serializedObject.FindProperty ("mapMenu");
		mapWindowTargetPosition = serializedObject.FindProperty ("mapWindowTargetPosition");
		mapRender = serializedObject.FindProperty ("mapRender");
		mapWindow = serializedObject.FindProperty ("mapWindow");
		playerMapIcon = serializedObject.FindProperty ("playerMapIcon");
		playerIconChild = serializedObject.FindProperty ("playerIconChild");
		removeMarkButtonImage = serializedObject.FindProperty ("removeMarkButtonImage");
		quickTravelButtonImage = serializedObject.FindProperty ("quickTravelButtonImage");
		mapObjectNameField = serializedObject.FindProperty ("mapObjectNameField");
		mapObjectInfoField = serializedObject.FindProperty ("mapObjectInfoField");
		currentFloorNumberText = serializedObject.FindProperty ("currentFloorNumberText");
		useMapIndexWindow = serializedObject.FindProperty ("useMapIndexWindow");
		mapIndexWindow = serializedObject.FindProperty ("mapIndexWindow");
		mapIndexWindowContent = serializedObject.FindProperty ("mapIndexWindowContent");
		mapIndexWindowScroller = serializedObject.FindProperty ("mapIndexWindowScroller");
		mapObjectTextIcon = serializedObject.FindProperty ("mapObjectTextIcon");
		useBlurUIPanel = serializedObject.FindProperty ("useBlurUIPanel");
		mapWindowMask = serializedObject.FindProperty ("mapWindowMask");
		zoomScrollbar = serializedObject.FindProperty ("zoomScrollbar");
		currentMapZoneText = serializedObject.FindProperty ("currentMapZoneText");
		mapCursor = serializedObject.FindProperty ("mapCursor");
		mapCursorRectTransform = serializedObject.FindProperty ("mapCursorRectTransform");
		currenMapIconPressed = serializedObject.FindProperty ("currenMapIconPressed");
		mapCircleTransform = serializedObject.FindProperty ("mapCircleTransform");
		playerInput = serializedObject.FindProperty ("playerInput");
		pauseManager = serializedObject.FindProperty ("pauseManager");
		screenObjectivesManager = serializedObject.FindProperty ("screenObjectivesManager");
		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		playerMapObjectInformation = serializedObject.FindProperty ("playerMapObjectInformation");
		mainMapCamera = serializedObject.FindProperty ("mainMapCamera");
		mapCreatorPrefab = serializedObject.FindProperty ("mapCreatorPrefab");
		playerIconMovementSpeed = serializedObject.FindProperty ("playerIconMovementSpeed");
		openMapSpeed = serializedObject.FindProperty ("openMapSpeed");
		mouseDragMapSpeed = serializedObject.FindProperty ("mouseDragMapSpeed");
		keysDragMapSpeed = serializedObject.FindProperty ("keysDragMapSpeed");
		getClosestFloorToPlayerByDistance = serializedObject.FindProperty ("getClosestFloorToPlayerByDistance");
		mapCameraMovementType = serializedObject.FindProperty ("mapCameraMovementType");
		recenterCameraSpeed = serializedObject.FindProperty ("recenterCameraSpeed");
		rotateMap = serializedObject.FindProperty ("rotateMap");
		smoothRotationMap = serializedObject.FindProperty ("smoothRotationMap");
		rotationSpeed = serializedObject.FindProperty ("rotationSpeed");
		usingCircleMap = serializedObject.FindProperty ("usingCircleMap");
		circleMapRadius = serializedObject.FindProperty ("circleMapRadius");
		showOffScreenIcons = serializedObject.FindProperty ("showOffScreenIcons");
		iconSize = serializedObject.FindProperty ("iconSize");
		maxIconSize = serializedObject.FindProperty ("maxIconSize");
		offScreenIconSize = serializedObject.FindProperty ("offScreenIconSize");
		openMapIconSizeMultiplier = serializedObject.FindProperty ("openMapIconSizeMultiplier");
		changeIconSizeSpeed = serializedObject.FindProperty ("changeIconSizeSpeed");
		showIconsByFloor = serializedObject.FindProperty ("showIconsByFloor");
		borderOffScreen = serializedObject.FindProperty ("borderOffScreen");
		useTextInIcons = serializedObject.FindProperty ("useTextInIcons");
		textIconsOffset = serializedObject.FindProperty ("textIconsOffset");
		mapObjectTextIconColor = serializedObject.FindProperty ("mapObjectTextIconColor");
		mapObjectTextSize = serializedObject.FindProperty ("mapObjectTextSize");
		miniMapWindowEnabledInGame = serializedObject.FindProperty ("miniMapWindowEnabledInGame");
		miniMapWindowSmoothOpening = serializedObject.FindProperty ("miniMapWindowSmoothOpening");
		miniMapWindowWithMask = serializedObject.FindProperty ("miniMapWindowWithMask");
		playerUseMapObjectInformation = serializedObject.FindProperty ("playerUseMapObjectInformation");
		playerIconOffset = serializedObject.FindProperty ("playerIconOffset");
		useCurrentMapIconPressed = serializedObject.FindProperty ("useCurrentMapIconPressed");
		useMapCursor = serializedObject.FindProperty ("useMapCursor");
		showInfoIconInsideCursor = serializedObject.FindProperty ("showInfoIconInsideCursor");
		maxDistanceToMapIcon = serializedObject.FindProperty ("maxDistanceToMapIcon");
		zoomWhenOpen = serializedObject.FindProperty ("zoomWhenOpen");
		zoomWhenClose = serializedObject.FindProperty ("zoomWhenClose");
		openCloseZoomSpeed = serializedObject.FindProperty ("openCloseZoomSpeed");
		zoomSpeed = serializedObject.FindProperty ("zoomSpeed");
		maxZoom = serializedObject.FindProperty ("maxZoom");
		minZoom = serializedObject.FindProperty ("minZoom");
		zoomToActivateIcons = serializedObject.FindProperty ("zoomToActivateIcons");
		zoomToActivateTextIcons = serializedObject.FindProperty ("zoomToActivateTextIcons");
		zoomWhenOpen3d = serializedObject.FindProperty ("zoomWhenOpen3d");
		zoomWhenClose3d = serializedObject.FindProperty ("zoomWhenClose3d");
		openCloseZoomSpeed3d = serializedObject.FindProperty ("openCloseZoomSpeed3d");
		zoomSpeed3d = serializedObject.FindProperty ("zoomSpeed3d");
		maxZoom3d = serializedObject.FindProperty ("maxZoom3d");
		minZoom3d = serializedObject.FindProperty ("minZoom3d");
		zoomToActivateIcons3d = serializedObject.FindProperty ("zoomToActivateIcons3d");
		zoomToActivateTextIcons3d = serializedObject.FindProperty ("zoomToActivateTextIcons3d");
		setColorOnCurrent3dMapPart = serializedObject.FindProperty ("setColorOnCurrent3dMapPart");
		colorOnCurrent3dMapPart = serializedObject.FindProperty ("colorOnCurrent3dMapPart");
		disabledRemoveMarkColor = serializedObject.FindProperty ("disabledRemoveMarkColor");
		disabledQuickTravelColor = serializedObject.FindProperty ("disabledQuickTravelColor");
		map3dEnabled = serializedObject.FindProperty ("map3dEnabled");
		map3dPositionSpeed = serializedObject.FindProperty ("map3dPositionSpeed");
		map3dRotationSpeed = serializedObject.FindProperty ("map3dRotationSpeed");
		rangeAngleX = serializedObject.FindProperty ("rangeAngleX");
		rangeAngleY = serializedObject.FindProperty ("rangeAngleY");
		transtionTo3dSpeed = serializedObject.FindProperty ("transtionTo3dSpeed");
		maxTimeBetweenTransition = serializedObject.FindProperty ("maxTimeBetweenTransition");
		reset3dCameraSpeed = serializedObject.FindProperty ("reset3dCameraSpeed");
		inital3dCameraRotation = serializedObject.FindProperty ("inital3dCameraRotation");
		hideOffscreenIconsOn3dView = serializedObject.FindProperty ("hideOffscreenIconsOn3dView");
		showIconsOn3dView = serializedObject.FindProperty ("showIconsOn3dView");
		use3dMeshForPlayer = serializedObject.FindProperty ("use3dMeshForPlayer");
		player3dMesh = serializedObject.FindProperty ("player3dMesh");
		useEventIfMapEnabled = serializedObject.FindProperty ("useEventIfMapEnabled");
		eventIfMapEnabled = serializedObject.FindProperty ("eventIfMapEnabled");
		useEventIfMapDisabled = serializedObject.FindProperty ("useEventIfMapDisabled");
		eventIfMapDisabled = serializedObject.FindProperty ("eventIfMapDisabled");
		markPrefab = serializedObject.FindProperty ("markPrefab");
		setMarkOnCurrenBuilding = serializedObject.FindProperty ("setMarkOnCurrenBuilding");
		setMarkOnCurrentFloor = serializedObject.FindProperty ("setMarkOnCurrentFloor");
		compassWindow = serializedObject.FindProperty ("compassWindow");
		compassElementsParent = serializedObject.FindProperty ("compassElementsParent");
		north = serializedObject.FindProperty ("north");
		south = serializedObject.FindProperty ("south");
		east = serializedObject.FindProperty ("east");
		west = serializedObject.FindProperty ("west");
		northEast = serializedObject.FindProperty ("northEast");
		southWest = serializedObject.FindProperty ("southWest");
		southEast = serializedObject.FindProperty ("southEast");
		northWest = serializedObject.FindProperty ("northWest");
		compassDirections = serializedObject.FindProperty ("compassDirections");
		northGameObject = serializedObject.FindProperty ("northGameObject");
		southGameObject = serializedObject.FindProperty ("southGameObject");
		eastGameObject = serializedObject.FindProperty ("eastGameObject");
		westGameObject = serializedObject.FindProperty ("westGameObject");
		northEastGameObject = serializedObject.FindProperty ("northEastGameObject");
		southWestGameObject = serializedObject.FindProperty ("southWestGameObject");
		southEastGameObject = serializedObject.FindProperty ("southEastGameObject");
		northwestGameObject = serializedObject.FindProperty ("northwestGameObject");
		compassEnabled = serializedObject.FindProperty ("compassEnabled");
		compassOffset = serializedObject.FindProperty ("compassOffset");
		compassScale = serializedObject.FindProperty ("compassScale");
		showIntermediateDirections = serializedObject.FindProperty ("showIntermediateDirections");
		maximumLeftDistance = serializedObject.FindProperty ("maximumLeftDistance");
		maximumRightDistance = serializedObject.FindProperty ("maximumRightDistance");
		usePlayerTransformOrientationOnCompassOnThirdPerson = serializedObject.FindProperty ("usePlayerTransformOrientationOnCompassOnThirdPerson");
		usePlayerTransformOrientationOnCompassOnFirstPerson = serializedObject.FindProperty ("usePlayerTransformOrientationOnCompassOnFirstPerson");
		usePlayerTransformOrientationOnCompassOnLockedCamera = serializedObject.FindProperty ("usePlayerTransformOrientationOnCompassOnLockedCamera");
		buildingList = serializedObject.FindProperty ("buildingList");

		mainScreenObjectivesManagerName = serializedObject.FindProperty ("mainScreenObjectivesManagerName");

		ingameMenuName = serializedObject.FindProperty ("ingameMenuName");
		beaconIconTypeName = serializedObject.FindProperty ("beaconIconTypeName");
		markIconTypeName = serializedObject.FindProperty ("markIconTypeName");

		mapSystemManager = (mapSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (mapEnabled);
		EditorGUILayout.PropertyField (changeFloorWithTriggers);
		EditorGUILayout.PropertyField (showWarningMessages);
		EditorGUILayout.PropertyField (updateMapIconTypesOnStart);

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (mainScreenObjectivesManagerName);

		EditorGUILayout.PropertyField (ingameMenuName);
		EditorGUILayout.PropertyField (beaconIconTypeName);
		EditorGUILayout.PropertyField (markIconTypeName);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Position In Map Settings", "window");
		if (Application.isPlaying) {
			GUILayout.Label ("Current Building Name\t" + currentBuildingName.stringValue);
			GUILayout.Label ("Current Building Index\t" + currentBuildingIndex.intValue);
			GUILayout.Label ("Current Floor Index\t\t" + currentFloorIndex.intValue);
			GUILayout.Label ("Current Floor Number\t\t" + currentFloorNumber.intValue);
			GUILayout.Label ("Current Map Part Index\t" + currentMapPartIndex.intValue);
		}

		buildingListString = mapSystemManager.buildingListString;

		if (buildingListString.Length > 0) {

			temporalBuildingIndex = mapSystemManager.currentBuildingIndex;

			temporalBuildingIndex = EditorGUILayout.Popup ("Building Number", temporalBuildingIndex, buildingListString);

			mapSystemManager.currentBuildingIndex = temporalBuildingIndex;

			if (temporalBuildingIndex >= 0) {
				mapSystemManager.currentBuildingName = buildingListString [temporalBuildingIndex];
			}

			floorListString = mapSystemManager.floorListString;

			if (floorListString.Length > 0) {

				temporalFloorIndex = mapSystemManager.currentFloorIndex;

				temporalFloorIndex = EditorGUILayout.Popup ("Floor Number", temporalFloorIndex, floorListString);

				mapSystemManager.currentFloorIndex = temporalFloorIndex;

				if (temporalFloorIndex >= 0 && temporalFloorIndex < floorListString.Length) {
					mapSystemManager.currentFloorName = floorListString [temporalFloorIndex];
				}

				mapPartListString = mapSystemManager.mapPartListString;

				if (mapPartListString.Length > 0) {
					temporalMapPartIndex = mapSystemManager.currentMapPartIndex;

					temporalMapPartIndex = EditorGUILayout.Popup ("Map Part", temporalMapPartIndex, mapPartListString);

					mapSystemManager.currentMapPartIndex = temporalMapPartIndex;

					if (mapPartListString.Length > temporalMapPartIndex) {
						mapSystemManager.currentMapPartName = mapPartListString [temporalMapPartIndex];
					}
				}
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Map Info")) {
			mapSystemManager.updateEditorMapInfo ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Player Map System To Map Creator")) {
			mapSystemManager.addPlayerMapSystemToMapCreator ();	
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (mapCreatorManager);

		if (mapCreatorManager.objectReferenceValue) {
			EditorGUILayout.Space ();

			if (GUILayout.Button ("Select Map Creator Object")) {
				mapSystemManager.selectMapCreatorObject ();
			}
		} else {
				
			EditorGUILayout.Space ();

			if (GUILayout.Button ("Add Map Creator")) {
				mapSystemManager.addNewMapCreator ();	
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();

		if (showMapComponents.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Map Components")) {
			showMapComponents.boolValue = !showMapComponents.boolValue;
		}

		if (showMapSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Map Settings")) {
			showMapSettings.boolValue = !showMapSettings.boolValue;
		}

		if (showMarkSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Mark Settings")) {
			showMarkSettings.boolValue = !showMarkSettings.boolValue;
		}

		if (showCompassComponents.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Compass Components")) {
			showCompassComponents.boolValue = !showCompassComponents.boolValue;
		}

		if (showCompassSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Compass Settings")) {
			showCompassSettings.boolValue = !showCompassSettings.boolValue;
		}

		if (showMapFloorAndIcons.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("World Map")) {
			showMapFloorAndIcons.boolValue = !showMapFloorAndIcons.boolValue;
		}
		GUI.backgroundColor = defBackgroundColor;

		EditorGUILayout.EndVertical ();

		if (showMapComponents.boolValue) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Set every Map Component here", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (mapContent);
			EditorGUILayout.PropertyField (mapCamera);
			EditorGUILayout.PropertyField (mapSystemPivotTransform);
			EditorGUILayout.PropertyField (mapSystemCameraTransform);
			EditorGUILayout.PropertyField (player);
			EditorGUILayout.PropertyField (mapMenu);
			EditorGUILayout.PropertyField (mapWindowTargetPosition);
			EditorGUILayout.PropertyField (mapRender);
			EditorGUILayout.PropertyField (mapWindow);
			EditorGUILayout.PropertyField (playerMapIcon);
			EditorGUILayout.PropertyField (playerIconChild);

			EditorGUILayout.PropertyField (removeMarkButtonImage);
			EditorGUILayout.PropertyField (quickTravelButtonImage);
	
			EditorGUILayout.PropertyField (mapObjectNameField);
			EditorGUILayout.PropertyField (mapObjectInfoField);

			EditorGUILayout.PropertyField (currentFloorNumberText);
			EditorGUILayout.PropertyField (useMapIndexWindow);
			EditorGUILayout.PropertyField (mapIndexWindow);
			EditorGUILayout.PropertyField (mapIndexWindowContent);
			EditorGUILayout.PropertyField (mapIndexWindowScroller);
			EditorGUILayout.PropertyField (mapObjectTextIcon);
			EditorGUILayout.PropertyField (useBlurUIPanel);
			EditorGUILayout.PropertyField (mapWindowMask);	
			EditorGUILayout.PropertyField (zoomScrollbar);
			EditorGUILayout.PropertyField (currentMapZoneText);

			EditorGUILayout.PropertyField (mapCursor);
			EditorGUILayout.PropertyField (mapCursorRectTransform);
			EditorGUILayout.PropertyField (currenMapIconPressed);

			EditorGUILayout.PropertyField (mapCircleTransform);

			EditorGUILayout.PropertyField (playerInput);
			EditorGUILayout.PropertyField (pauseManager);
			EditorGUILayout.PropertyField (screenObjectivesManager);
			EditorGUILayout.PropertyField (playerControllerManager);
			EditorGUILayout.PropertyField (mapCreatorManager);
			EditorGUILayout.PropertyField (playerMapObjectInformation);
			EditorGUILayout.PropertyField (mainMapCamera);

			EditorGUILayout.PropertyField (mapCreatorPrefab);

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showMapSettings.boolValue) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Map Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("CONTROL", "window");
			EditorGUILayout.PropertyField (playerIconMovementSpeed);
			EditorGUILayout.PropertyField (openMapSpeed);
			EditorGUILayout.PropertyField (mouseDragMapSpeed);
			EditorGUILayout.PropertyField (keysDragMapSpeed);
			EditorGUILayout.PropertyField (getClosestFloorToPlayerByDistance);
			EditorGUILayout.PropertyField (mapCameraMovementType);
			EditorGUILayout.PropertyField (recenterCameraSpeed);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("ROTATION", "window");
			EditorGUILayout.PropertyField (rotateMap);
			if (rotateMap.boolValue) {
				EditorGUILayout.PropertyField (smoothRotationMap);
				EditorGUILayout.PropertyField (rotationSpeed);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("CIRCLE MAP", "window");
			EditorGUILayout.PropertyField (usingCircleMap);
			if (usingCircleMap.boolValue) {
				EditorGUILayout.PropertyField (circleMapRadius);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("ICONS", "window");
			EditorGUILayout.PropertyField (showOffScreenIcons);
			EditorGUILayout.PropertyField (iconSize);
			EditorGUILayout.PropertyField (maxIconSize);
			EditorGUILayout.PropertyField (offScreenIconSize);
			EditorGUILayout.PropertyField (openMapIconSizeMultiplier);
			EditorGUILayout.PropertyField (changeIconSizeSpeed);
			EditorGUILayout.PropertyField (showIconsByFloor);
			EditorGUILayout.PropertyField (borderOffScreen);
			EditorGUILayout.PropertyField (useTextInIcons);
			EditorGUILayout.PropertyField (textIconsOffset);	
			EditorGUILayout.PropertyField (mapObjectTextIconColor);
			EditorGUILayout.PropertyField (mapObjectTextSize);
			EditorGUILayout.PropertyField (miniMapWindowEnabledInGame);
			EditorGUILayout.PropertyField (miniMapWindowSmoothOpening);
			EditorGUILayout.PropertyField (miniMapWindowWithMask);
			EditorGUILayout.PropertyField (playerUseMapObjectInformation);
			EditorGUILayout.PropertyField (playerIconOffset);
			EditorGUILayout.PropertyField (useCurrentMapIconPressed);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("MAP CURSOR SETTINGS", "window");
			EditorGUILayout.PropertyField (useMapCursor);
			if (useMapCursor.boolValue) {
				EditorGUILayout.PropertyField (showInfoIconInsideCursor);
				if (showInfoIconInsideCursor.boolValue) {
					EditorGUILayout.PropertyField (maxDistanceToMapIcon);
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("2D ZOOM SETTINGS", "window");
			EditorGUILayout.PropertyField (zoomWhenOpen);
			EditorGUILayout.PropertyField (zoomWhenClose);
			EditorGUILayout.PropertyField (openCloseZoomSpeed);
			EditorGUILayout.PropertyField (zoomSpeed);
			EditorGUILayout.PropertyField (maxZoom);
			EditorGUILayout.PropertyField (minZoom);
			EditorGUILayout.PropertyField (zoomToActivateIcons);	
			EditorGUILayout.PropertyField (zoomToActivateTextIcons);	
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("3D ZOOM SETTINGS", "window");
			EditorGUILayout.PropertyField (zoomWhenOpen3d);
			EditorGUILayout.PropertyField (zoomWhenClose3d);
			EditorGUILayout.PropertyField (openCloseZoomSpeed3d);
			EditorGUILayout.PropertyField (zoomSpeed3d);
			EditorGUILayout.PropertyField (maxZoom3d);
			EditorGUILayout.PropertyField (minZoom3d);
			EditorGUILayout.PropertyField (zoomToActivateIcons3d);	
			EditorGUILayout.PropertyField (zoomToActivateTextIcons3d);
			EditorGUILayout.PropertyField (setColorOnCurrent3dMapPart);
			if (setColorOnCurrent3dMapPart.boolValue) {
				EditorGUILayout.PropertyField (colorOnCurrent3dMapPart);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("MARKS", "window");
			EditorGUILayout.PropertyField (disabledRemoveMarkColor);
			EditorGUILayout.PropertyField (disabledQuickTravelColor);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("3D Map Settings", "window");
			EditorGUILayout.PropertyField (map3dEnabled);
			if (map3dEnabled.boolValue) {
				EditorGUILayout.PropertyField (map3dPositionSpeed);
				EditorGUILayout.PropertyField (map3dRotationSpeed);
				EditorGUILayout.PropertyField (rangeAngleX);
				EditorGUILayout.PropertyField (rangeAngleY);
				EditorGUILayout.PropertyField (transtionTo3dSpeed);
				EditorGUILayout.PropertyField (maxTimeBetweenTransition);
				EditorGUILayout.PropertyField (reset3dCameraSpeed);
				EditorGUILayout.PropertyField (inital3dCameraRotation);
				EditorGUILayout.PropertyField (hideOffscreenIconsOn3dView);
				EditorGUILayout.PropertyField (showIconsOn3dView);
				EditorGUILayout.PropertyField (use3dMeshForPlayer);
				EditorGUILayout.PropertyField (player3dMesh);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Map Event Settings", "window");
			EditorGUILayout.PropertyField (useEventIfMapEnabled);
			if (useEventIfMapEnabled.boolValue) {
				EditorGUILayout.PropertyField (eventIfMapEnabled);
			}
			EditorGUILayout.PropertyField (useEventIfMapDisabled);
			if (useEventIfMapDisabled.boolValue) {
				EditorGUILayout.PropertyField (eventIfMapDisabled);
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showMarkSettings.boolValue) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Mark Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (markPrefab);
			EditorGUILayout.PropertyField (setMarkOnCurrenBuilding);
			EditorGUILayout.PropertyField (setMarkOnCurrentFloor);

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showCompassComponents.boolValue) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Set every Compass Component here", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (compassWindow);
			EditorGUILayout.PropertyField (compassElementsParent);
			EditorGUILayout.PropertyField (north);
			EditorGUILayout.PropertyField (south);
			EditorGUILayout.PropertyField (east);
			EditorGUILayout.PropertyField (west);

			EditorGUILayout.PropertyField (northEast);
			EditorGUILayout.PropertyField (southWest);
			EditorGUILayout.PropertyField (southEast);
			EditorGUILayout.PropertyField (northWest);

			EditorGUILayout.PropertyField (compassDirections);

			EditorGUILayout.PropertyField (northGameObject);
			EditorGUILayout.PropertyField (southGameObject);
			EditorGUILayout.PropertyField (eastGameObject);
			EditorGUILayout.PropertyField (westGameObject);

			EditorGUILayout.PropertyField (northEastGameObject);
			EditorGUILayout.PropertyField (southWestGameObject);
			EditorGUILayout.PropertyField (southEastGameObject);
			EditorGUILayout.PropertyField (northwestGameObject);
	
			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showCompassSettings.boolValue) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Compass Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window");
			EditorGUILayout.PropertyField (compassEnabled);
			EditorGUILayout.PropertyField (compassOffset);
			EditorGUILayout.PropertyField (compassScale);
			EditorGUILayout.PropertyField (showIntermediateDirections);
			EditorGUILayout.PropertyField (maximumLeftDistance);
			EditorGUILayout.PropertyField (maximumRightDistance);

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Compass Orientation Settings", "window");
			EditorGUILayout.PropertyField (usePlayerTransformOrientationOnCompassOnThirdPerson);
			EditorGUILayout.PropertyField (usePlayerTransformOrientationOnCompassOnFirstPerson);
			EditorGUILayout.PropertyField (usePlayerTransformOrientationOnCompassOnLockedCamera);
			GUILayout.EndVertical ();
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showMapFloorAndIcons.boolValue) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure every Floor and Icon element for the map", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Building List", "window");
			showBuildingList (buildingList);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}
		GUI.backgroundColor = defBackgroundColor;
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();
	}

	void showBuildingList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Buildings: " + list.arraySize);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Search Building List")) {
				mapSystemManager.searchBuildingList ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Building")) {
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
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showBuildingElementInfo (list.GetArrayElementAtIndex (i));
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
	}

	void showBuildingElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isCurrentMap"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isInterior"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCameraPositionOnMapMenu"));
		if (list.FindPropertyRelative ("useCameraPositionOnMapMenu").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraPositionOnMapMenu"));
		}
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Floors List", "window");
		showFloorList (list.FindPropertyRelative ("floors"));
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showFloorList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Floors: " + list.arraySize);
	
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Floor")) {
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
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showFloorElementInfo (list.GetArrayElementAtIndex (i));
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
	}

	void showFloorElementInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floorNumber"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("floor"));
		GUILayout.EndVertical ();
	}
}
#endif