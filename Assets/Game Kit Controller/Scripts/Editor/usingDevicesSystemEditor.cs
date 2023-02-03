using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(usingDevicesSystem))]
[CanEditMultipleObjects]
public class usingDevicesSystemEditor : Editor
{
	SerializedProperty canUseDevices;

	SerializedProperty touchButtonRawImage;
	SerializedProperty originalTouchButtonRawImage;
	SerializedProperty touchButton;

	SerializedProperty touchButtonIcon;
	SerializedProperty iconButton;
	SerializedProperty iconButtonRectTransform;
	SerializedProperty actionText;
	SerializedProperty keyText;
	SerializedProperty objectNameText;

	SerializedProperty objectImage;
	SerializedProperty objectDescriptionText;

	SerializedProperty useDeviceFunctionName;
	SerializedProperty setCurrentUserOnDeviceFunctionName;
	SerializedProperty useDevicesActionName;
	SerializedProperty usePickUpAmountIfEqualToOne;
	SerializedProperty useOnlyDeviceIfVisibleOnCamera;
	SerializedProperty showUseDeviceIconEnabled;
	SerializedProperty useFixedDeviceIconPosition;
	SerializedProperty deviceOnScreenIfUseFixedIconPosition;

	SerializedProperty showInteractionPanelIfObjectNotOnScreen;

	SerializedProperty useDeviceButtonEnabled;
	SerializedProperty getClosestDeviceToCameraCenter;
	SerializedProperty useMaxDistanceToCameraCenter;
	SerializedProperty maxDistanceToCameraCenter;
	SerializedProperty defaultDeviceNameFontSize;
	SerializedProperty extraTextStartActionKey;
	SerializedProperty extraTextEndActionKey;
	SerializedProperty showCurrentDeviceAmount;
	SerializedProperty currentDeviceAmountTextPanel;
	SerializedProperty currentDeviceAmountText;
	SerializedProperty showDetectedDevicesIconOnScreen;
	SerializedProperty detectedDevicesIconPrefab;
	SerializedProperty detectedDevicesIconParent;
	SerializedProperty useMinDistanceToUseDevices;
	SerializedProperty minDistanceToUseDevices;

	SerializedProperty disableInteractionTouchButtonIfNotDevicesDetected;
	SerializedProperty keepInteractionTouchButtonAlwaysActive;

	SerializedProperty layer;
	SerializedProperty raycastDistance;

	SerializedProperty mainCollider;
	SerializedProperty ignoreCheckIfObstacleBetweenDeviceAndPlayer;

	SerializedProperty tagsForDevices;
	SerializedProperty interactionMessageGameObject;
	SerializedProperty interactionMessageText;
	SerializedProperty useDeviceFoundShader;
	SerializedProperty deviceFoundShader;
	SerializedProperty shaderOutlineWidth;
	SerializedProperty shaderOutlineColor;
	SerializedProperty holdButtonToTakePickupsAround;
	SerializedProperty holdButtonTime;
	SerializedProperty useInteractionActions;
	SerializedProperty interactionActionInfoList;

	SerializedProperty multipleInteractionInfoList;

	SerializedProperty useIconButtonInfoList;
	SerializedProperty defaultIconButtonName;
	SerializedProperty iconButtonInfoList;
	SerializedProperty iconButtonCanBeShown;
	SerializedProperty currentVehicle;
	SerializedProperty driving;
	SerializedProperty objectToUse;
	SerializedProperty currenDeviceActionName;
	SerializedProperty currentDeviceIsPickup;
	SerializedProperty examiningObject;
	SerializedProperty deviceGameObjectList;

	SerializedProperty playerControllerManager;
	SerializedProperty grabObjectsManager;
	SerializedProperty playerInput;
	SerializedProperty mainCamera;
	SerializedProperty playerCameraManager;
	SerializedProperty mainCameraTransform;
	SerializedProperty examineObjectRenderTexturePanel;
	SerializedProperty examineObjectBlurPanelParent;
	SerializedProperty examineObjectRenderCamera;
	SerializedProperty examinateDevicesCamera;
	SerializedProperty usedByAI;

	SerializedProperty useGameObjectListToIgnore;
	SerializedProperty gameObjectListToIgnore;

	SerializedProperty holdInteractionButtonEnabled;

	string currentVehicleText;
	string usingDevices;
	string currentDeviceFound;

	Color buttonColor;

	string buttonText;

	bool expanded;

	SerializedProperty showIntereactionSettings;
	SerializedProperty showOutlineShaderSettings;
	SerializedProperty showEventsSettings;
	SerializedProperty showOtherSettings;
	SerializedProperty showDebugSettings;
	SerializedProperty showAllSettings;
	SerializedProperty showComponents;
	SerializedProperty showUISettings;

	GUIStyle style = new GUIStyle ();

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		canUseDevices = serializedObject.FindProperty ("canUseDevices");

		touchButtonRawImage = serializedObject.FindProperty ("touchButtonRawImage");
		originalTouchButtonRawImage = serializedObject.FindProperty ("originalTouchButtonRawImage");
		touchButton = serializedObject.FindProperty ("touchButton");

		touchButtonIcon = serializedObject.FindProperty ("touchButtonIcon");
		iconButton = serializedObject.FindProperty ("iconButton");
		iconButtonRectTransform = serializedObject.FindProperty ("iconButtonRectTransform");
		actionText = serializedObject.FindProperty ("actionText");
		keyText = serializedObject.FindProperty ("keyText");
		objectNameText = serializedObject.FindProperty ("objectNameText");

		objectImage = serializedObject.FindProperty ("objectImage");
		objectDescriptionText = serializedObject.FindProperty ("objectDescriptionText");

		useDeviceFunctionName = serializedObject.FindProperty ("useDeviceFunctionName");
		setCurrentUserOnDeviceFunctionName = serializedObject.FindProperty ("setCurrentUserOnDeviceFunctionName");
		useDevicesActionName = serializedObject.FindProperty ("useDevicesActionName");
		usePickUpAmountIfEqualToOne = serializedObject.FindProperty ("usePickUpAmountIfEqualToOne");
		useOnlyDeviceIfVisibleOnCamera = serializedObject.FindProperty ("useOnlyDeviceIfVisibleOnCamera");
		showUseDeviceIconEnabled = serializedObject.FindProperty ("showUseDeviceIconEnabled");
		useFixedDeviceIconPosition = serializedObject.FindProperty ("useFixedDeviceIconPosition");
		deviceOnScreenIfUseFixedIconPosition = serializedObject.FindProperty ("deviceOnScreenIfUseFixedIconPosition");

		showInteractionPanelIfObjectNotOnScreen = serializedObject.FindProperty ("showInteractionPanelIfObjectNotOnScreen");

		useDeviceButtonEnabled = serializedObject.FindProperty ("useDeviceButtonEnabled");
		getClosestDeviceToCameraCenter = serializedObject.FindProperty ("getClosestDeviceToCameraCenter");
		useMaxDistanceToCameraCenter = serializedObject.FindProperty ("useMaxDistanceToCameraCenter");
		maxDistanceToCameraCenter = serializedObject.FindProperty ("maxDistanceToCameraCenter");
		defaultDeviceNameFontSize = serializedObject.FindProperty ("defaultDeviceNameFontSize");
		extraTextStartActionKey = serializedObject.FindProperty ("extraTextStartActionKey");
		extraTextEndActionKey = serializedObject.FindProperty ("extraTextEndActionKey");
		showCurrentDeviceAmount = serializedObject.FindProperty ("showCurrentDeviceAmount");
		currentDeviceAmountTextPanel = serializedObject.FindProperty ("currentDeviceAmountTextPanel");
		currentDeviceAmountText = serializedObject.FindProperty ("currentDeviceAmountText");
		showDetectedDevicesIconOnScreen = serializedObject.FindProperty ("showDetectedDevicesIconOnScreen");
		detectedDevicesIconPrefab = serializedObject.FindProperty ("detectedDevicesIconPrefab");
		detectedDevicesIconParent = serializedObject.FindProperty ("detectedDevicesIconParent");
		useMinDistanceToUseDevices = serializedObject.FindProperty ("useMinDistanceToUseDevices");
		minDistanceToUseDevices = serializedObject.FindProperty ("minDistanceToUseDevices");

		disableInteractionTouchButtonIfNotDevicesDetected = serializedObject.FindProperty ("disableInteractionTouchButtonIfNotDevicesDetected");
		keepInteractionTouchButtonAlwaysActive = serializedObject.FindProperty ("keepInteractionTouchButtonAlwaysActive");

		layer = serializedObject.FindProperty ("layer");
		raycastDistance = serializedObject.FindProperty ("raycastDistance");

		mainCollider = serializedObject.FindProperty ("mainCollider");
		ignoreCheckIfObstacleBetweenDeviceAndPlayer = serializedObject.FindProperty ("ignoreCheckIfObstacleBetweenDeviceAndPlayer");

		tagsForDevices = serializedObject.FindProperty ("tagsForDevices");
		interactionMessageGameObject = serializedObject.FindProperty ("interactionMessageGameObject");
		interactionMessageText = serializedObject.FindProperty ("interactionMessageText");
		useDeviceFoundShader = serializedObject.FindProperty ("useDeviceFoundShader");
		deviceFoundShader = serializedObject.FindProperty ("deviceFoundShader");
		shaderOutlineWidth = serializedObject.FindProperty ("shaderOutlineWidth");
		shaderOutlineColor = serializedObject.FindProperty ("shaderOutlineColor");
		holdButtonToTakePickupsAround = serializedObject.FindProperty ("holdButtonToTakePickupsAround");
		holdButtonTime = serializedObject.FindProperty ("holdButtonTime");
		useInteractionActions = serializedObject.FindProperty ("useInteractionActions");
		interactionActionInfoList = serializedObject.FindProperty ("interactionActionInfoList");

		multipleInteractionInfoList = serializedObject.FindProperty ("multipleInteractionInfoList");

		useIconButtonInfoList = serializedObject.FindProperty ("useIconButtonInfoList");
		defaultIconButtonName = serializedObject.FindProperty ("defaultIconButtonName");
		iconButtonInfoList = serializedObject.FindProperty ("iconButtonInfoList");
		iconButtonCanBeShown = serializedObject.FindProperty ("iconButtonCanBeShown");
		currentVehicle = serializedObject.FindProperty ("currentVehicle");
		driving = serializedObject.FindProperty ("driving");
		objectToUse = serializedObject.FindProperty ("objectToUse");
		currenDeviceActionName = serializedObject.FindProperty ("currenDeviceActionName");
		currentDeviceIsPickup = serializedObject.FindProperty ("currentDeviceIsPickup");
		examiningObject = serializedObject.FindProperty ("examiningObject");
		deviceGameObjectList = serializedObject.FindProperty ("deviceGameObjectList");

		playerControllerManager = serializedObject.FindProperty ("playerControllerManager");
		grabObjectsManager = serializedObject.FindProperty ("grabObjectsManager");
		playerInput = serializedObject.FindProperty ("playerInput");
		mainCamera = serializedObject.FindProperty ("mainCamera");
		playerCameraManager = serializedObject.FindProperty ("playerCameraManager");
		mainCameraTransform = serializedObject.FindProperty ("mainCameraTransform");
		examineObjectRenderTexturePanel = serializedObject.FindProperty ("examineObjectRenderTexturePanel");
		examineObjectBlurPanelParent = serializedObject.FindProperty ("examineObjectBlurPanelParent");
		examineObjectRenderCamera = serializedObject.FindProperty ("examineObjectRenderCamera");
		examinateDevicesCamera = serializedObject.FindProperty ("examinateDevicesCamera");

		usedByAI = serializedObject.FindProperty ("usedByAI");

		useGameObjectListToIgnore = serializedObject.FindProperty ("useGameObjectListToIgnore");
		gameObjectListToIgnore = serializedObject.FindProperty ("gameObjectListToIgnore");

		holdInteractionButtonEnabled = serializedObject.FindProperty ("holdInteractionButtonEnabled");

		showIntereactionSettings = serializedObject.FindProperty ("showIntereactionSettings");
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

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (canUseDevices);

		EditorGUILayout.PropertyField (useDeviceFunctionName);
		EditorGUILayout.PropertyField (setCurrentUserOnDeviceFunctionName);
		EditorGUILayout.PropertyField (useDevicesActionName);
		EditorGUILayout.PropertyField (usePickUpAmountIfEqualToOne);
		EditorGUILayout.PropertyField (useOnlyDeviceIfVisibleOnCamera);

		EditorGUILayout.PropertyField (useDeviceButtonEnabled);

		EditorGUILayout.PropertyField (holdInteractionButtonEnabled);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Raycast Settings", "window");
		EditorGUILayout.PropertyField (layer);
		EditorGUILayout.PropertyField (raycastDistance);
		EditorGUILayout.PropertyField (ignoreCheckIfObstacleBetweenDeviceAndPlayer);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ignore GameObject List Settings", "window");
		EditorGUILayout.PropertyField (useGameObjectListToIgnore);
		if (useGameObjectListToIgnore.boolValue) {
			showSimpleList (gameObjectListToIgnore);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.BeginHorizontal ();

		if (showIntereactionSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Interaction")) {
			showIntereactionSettings.boolValue = !showIntereactionSettings.boolValue;
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

			showIntereactionSettings.boolValue = showAllSettings.boolValue;
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

		if (showAllSettings.boolValue || showIntereactionSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("INTERACTION SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Tag For Devices List", "window");
			showTagsForDevicesList (tagsForDevices);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Hold Button To Pick Around Settings", "window");
			EditorGUILayout.PropertyField (holdButtonToTakePickupsAround);
			if (holdButtonToTakePickupsAround.boolValue) {
				EditorGUILayout.PropertyField (holdButtonTime);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showUISettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("UI SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Device Icon Settings", "window");
			EditorGUILayout.PropertyField (showUseDeviceIconEnabled);
			if (showUseDeviceIconEnabled.boolValue) {
				EditorGUILayout.PropertyField (useFixedDeviceIconPosition);
				if (useFixedDeviceIconPosition.boolValue) {
					EditorGUILayout.PropertyField (deviceOnScreenIfUseFixedIconPosition);
				}
			}


			EditorGUILayout.PropertyField (getClosestDeviceToCameraCenter);
			if (getClosestDeviceToCameraCenter.boolValue) {
				EditorGUILayout.PropertyField (useMaxDistanceToCameraCenter);
				if (useMaxDistanceToCameraCenter.boolValue) {
					EditorGUILayout.PropertyField (maxDistanceToCameraCenter);
				}
			}

			EditorGUILayout.PropertyField (defaultDeviceNameFontSize);
			EditorGUILayout.PropertyField (extraTextStartActionKey);
			EditorGUILayout.PropertyField (extraTextEndActionKey);
			EditorGUILayout.PropertyField (showCurrentDeviceAmount);
			EditorGUILayout.PropertyField (currentDeviceAmountTextPanel);
			EditorGUILayout.PropertyField (currentDeviceAmountText);

			EditorGUILayout.PropertyField (showDetectedDevicesIconOnScreen);
			EditorGUILayout.PropertyField (detectedDevicesIconPrefab);
			EditorGUILayout.PropertyField (detectedDevicesIconParent);

			EditorGUILayout.PropertyField (useMinDistanceToUseDevices);
			if (useMinDistanceToUseDevices.boolValue) {
				EditorGUILayout.PropertyField (minDistanceToUseDevices);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (disableInteractionTouchButtonIfNotDevicesDetected);
			EditorGUILayout.PropertyField (keepInteractionTouchButtonAlwaysActive);

			EditorGUILayout.PropertyField (showInteractionPanelIfObjectNotOnScreen);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Icon Button List Settings", "window");
			EditorGUILayout.PropertyField (useIconButtonInfoList);
			if (useIconButtonInfoList.boolValue) {
				EditorGUILayout.PropertyField (defaultIconButtonName);

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Icon Button Info List", "window");
				showIconButtonInfoList (iconButtonInfoList);
				GUILayout.EndVertical ();
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showEventsSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("EVENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Interaction Message Settings", "window");
			EditorGUILayout.PropertyField (interactionMessageGameObject);
			EditorGUILayout.PropertyField (interactionMessageText);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Interaction Actions Settings", "window");
			EditorGUILayout.PropertyField (useInteractionActions);
			if (useInteractionActions.boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Interaction Action Info List", "window");
				showInteractionActionInfoList (interactionActionInfoList);
				GUILayout.EndVertical ();
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Multiple Interaction Info Settings", "window");
			showMultipleInteractionInfoList (multipleInteractionInfoList);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showAllSettings.boolValue || showOutlineShaderSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("OUTLINE SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Outline Shader Device Found Settings", "window");
			EditorGUILayout.PropertyField (useDeviceFoundShader);
			if (useDeviceFoundShader.boolValue) {
				EditorGUILayout.PropertyField (deviceFoundShader);
				EditorGUILayout.PropertyField (shaderOutlineWidth);
				EditorGUILayout.PropertyField (shaderOutlineColor);
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

			GUILayout.BeginVertical ("Devices State", "window");
			GUILayout.Label ("Icon Can Be Shown\t" + iconButtonCanBeShown.boolValue);

			currentVehicleText = "NONE";

			if (currentVehicle.objectReferenceValue) {
				currentVehicleText = currentVehicle.objectReferenceValue.name;
			}
			GUILayout.Label ("Current Vehicle\t" + currentVehicleText);

			GUILayout.Label ("Driving\t\t" + driving.boolValue);

			usingDevices = "NO";

			if (objectToUse.objectReferenceValue) {
				usingDevices = "YES";
			}
			GUILayout.Label ("Device Detected\t" + usingDevices);

			currentDeviceFound = "None";

			if (objectToUse.objectReferenceValue) {
				currentDeviceFound = currenDeviceActionName.stringValue;
			}

			GUILayout.Label ("Device Name\t" + currentDeviceFound);

			GUILayout.Label ("Device Is Pickup \t" + currentDeviceIsPickup.boolValue);

			GUILayout.Label ("Examining Object \t" + examiningObject.boolValue);

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Devices List", "window");
			showDeviceList (deviceGameObjectList);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}


		if (showAllSettings.boolValue || showOtherSettings.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("OTHERS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("AI Settings", "window");
			EditorGUILayout.PropertyField (usedByAI);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showComponents.boolValue) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("COMPONENTS SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Elements", "window");
			EditorGUILayout.PropertyField (playerControllerManager);
			EditorGUILayout.PropertyField (grabObjectsManager);
			EditorGUILayout.PropertyField (playerInput);
			EditorGUILayout.PropertyField (mainCamera);
			EditorGUILayout.PropertyField (playerCameraManager);
			EditorGUILayout.PropertyField (mainCameraTransform);
			EditorGUILayout.PropertyField (mainCollider);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Examine Object Elements", "window");
			EditorGUILayout.PropertyField (examineObjectRenderTexturePanel);
			EditorGUILayout.PropertyField (examineObjectBlurPanelParent);
			EditorGUILayout.PropertyField (examineObjectRenderCamera);
			EditorGUILayout.PropertyField (examinateDevicesCamera);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Interaction Elements", "window");
			EditorGUILayout.PropertyField (touchButton);
			EditorGUILayout.PropertyField (touchButtonRawImage);
			EditorGUILayout.PropertyField (originalTouchButtonRawImage);
			EditorGUILayout.PropertyField (touchButtonIcon);
			EditorGUILayout.PropertyField (iconButton);
			EditorGUILayout.PropertyField (iconButtonRectTransform);
			EditorGUILayout.PropertyField (actionText);
			EditorGUILayout.PropertyField (keyText);
			EditorGUILayout.PropertyField (objectNameText);
			EditorGUILayout.PropertyField (objectImage);
			EditorGUILayout.PropertyField (objectDescriptionText);

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showTagsForDevicesList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Number Of Tags: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Tag")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();
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
	}

	void showDeviceList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Devices: \t" + list.arraySize);
		
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
	
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showInteractionActionInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Actions: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Action")) {
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
						showInteractionActionInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showInteractionActionInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsedOnGamePaused"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnInteraction"));
		GUILayout.EndVertical ();
	}


	void showMultipleInteractionInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Actions: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Action")) {
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
						showMultipleInteractionInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showMultipleInteractionInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("inputName"));

		EditorGUILayout.Space ();

		showSimpleList (list.FindPropertyRelative ("multipleInteractionNameList"));
		GUILayout.EndVertical ();
	}

	void showIconButtonInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Icons: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Icon")) {
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
						showIconButtonInfoListElement (list.GetArrayElementAtIndex (i));
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

	void showIconButtonInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("iconButtonPanel"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("keyText"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionText"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectNameText"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectImage"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectDescriptionText"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("holdInteractionSlider"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showHoldInteractionTimer"));
		if (list.FindPropertyRelative ("showHoldInteractionTimer").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("holdInteractionTimerText"));
		}
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("touchButtonIcon"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraTextStartActionKey"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraTextEndActionKey"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFixedPosition"));
		if (list.FindPropertyRelative ("useFixedPosition").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("fixedPositionTransform"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNewTouchButtonRawImageIcon"));
		if (list.FindPropertyRelative ("setNewTouchButtonRawImageIcon").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newTouchButtonRawImageIcon"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMultipleInteractionInfo"));
		if (list.FindPropertyRelative ("useMultipleInteractionInfo").boolValue) {
			showSimpleList (list.FindPropertyRelative ("keyTextList"));

			EditorGUILayout.Space ();

			showSimpleList (list.FindPropertyRelative ("actionTextList"));
		}

		EditorGUILayout.Space ();

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
				GUILayout.BeginHorizontal ("box");
	
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();

				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
			
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif