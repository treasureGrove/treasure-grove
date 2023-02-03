using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(deviceStringAction))]
public class deviceStringActionEditor : Editor
{
	SerializedProperty deviceName;
	SerializedProperty deviceAction;

	SerializedProperty useMultipleInteraction;
	SerializedProperty multipleInteractionNameList;

	SerializedProperty secondaryDeviceAction;
	SerializedProperty reloadDeviceActionOnPress;
	SerializedProperty setUsingDeviceState;
	SerializedProperty useRaycastToDetectDeviceParts;

	SerializedProperty ignoreUseOnlyDeviceIfVisibleOnCamera;

	SerializedProperty useCustomDeviceTransformPosition;
	SerializedProperty customDeviceTransformPosition;

	SerializedProperty useFixedDeviceIconPosition;
	SerializedProperty hideIconOnPress;
	SerializedProperty disableIconOnPress;
	SerializedProperty hideIconOnUseDevice;
	SerializedProperty showIconOnStopUseDevice;
	SerializedProperty showIcon;
	SerializedProperty showTouchIconButton;
	SerializedProperty setTextFontSizeActive;
	SerializedProperty textFontSize;
	SerializedProperty useCustomMinDistanceToUseDevice;
	SerializedProperty customMinDistanceToUseDevice;
	SerializedProperty useCustomMinAngleToUse;
	SerializedProperty customMinAngleToUseDevice;
	SerializedProperty useRelativeDirectionBetweenPlayerAndObject;
	SerializedProperty useCustomIconButtonInfo;
	SerializedProperty customIconButtonInfoName;
	SerializedProperty useTransformForStringAction;
	SerializedProperty useSeparatedTransformForEveryView;
	SerializedProperty transformForThirdPerson;
	SerializedProperty transformForFirstPerson;
	SerializedProperty transformForStringAction;
	SerializedProperty actionOffset;
	SerializedProperty useLocalOffset;
	SerializedProperty iconEnabled;

	SerializedProperty checkIfObstacleBetweenDeviceAndPlayer;

	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoColor;
	SerializedProperty gizmoRadius;

	SerializedProperty showIconTexture;
	SerializedProperty iconTexture;

	SerializedProperty useObjectDescription;
	SerializedProperty objectDescription;
	SerializedProperty descriptionFontSize;

	SerializedProperty showObjectAmount;
	SerializedProperty objectAmount;

	SerializedProperty useHoldInteractionButton;
	SerializedProperty holdInteractionButtonDuration;

	deviceStringAction device;

	GUIStyle style = new GUIStyle ();
	Vector3 labelPosition;
	Transform currentTransform;
	Quaternion currentRotation;
	Vector3 oldPoint;

	void OnEnable ()
	{
		deviceName = serializedObject.FindProperty ("deviceName");
		deviceAction = serializedObject.FindProperty ("deviceAction");

		useMultipleInteraction = serializedObject.FindProperty ("useMultipleInteraction");
		multipleInteractionNameList = serializedObject.FindProperty ("multipleInteractionNameList");

		secondaryDeviceAction = serializedObject.FindProperty ("secondaryDeviceAction");
		reloadDeviceActionOnPress = serializedObject.FindProperty ("reloadDeviceActionOnPress");
		setUsingDeviceState = serializedObject.FindProperty ("setUsingDeviceState");
		useRaycastToDetectDeviceParts = serializedObject.FindProperty ("useRaycastToDetectDeviceParts");

		ignoreUseOnlyDeviceIfVisibleOnCamera = serializedObject.FindProperty ("ignoreUseOnlyDeviceIfVisibleOnCamera");

		useCustomDeviceTransformPosition = serializedObject.FindProperty ("useCustomDeviceTransformPosition");
		customDeviceTransformPosition = serializedObject.FindProperty ("customDeviceTransformPosition");

		useFixedDeviceIconPosition = serializedObject.FindProperty ("useFixedDeviceIconPosition");
		hideIconOnPress = serializedObject.FindProperty ("hideIconOnPress");
		disableIconOnPress = serializedObject.FindProperty ("disableIconOnPress");
		hideIconOnUseDevice = serializedObject.FindProperty ("hideIconOnUseDevice");
		showIconOnStopUseDevice = serializedObject.FindProperty ("showIconOnStopUseDevice");
		showIcon = serializedObject.FindProperty ("showIcon");
		showTouchIconButton = serializedObject.FindProperty ("showTouchIconButton");
		setTextFontSizeActive = serializedObject.FindProperty ("setTextFontSizeActive");
		textFontSize = serializedObject.FindProperty ("textFontSize");
		useCustomMinDistanceToUseDevice = serializedObject.FindProperty ("useCustomMinDistanceToUseDevice");
		customMinDistanceToUseDevice = serializedObject.FindProperty ("customMinDistanceToUseDevice");
		useCustomMinAngleToUse = serializedObject.FindProperty ("useCustomMinAngleToUse");
		customMinAngleToUseDevice = serializedObject.FindProperty ("customMinAngleToUseDevice");
		useRelativeDirectionBetweenPlayerAndObject = serializedObject.FindProperty ("useRelativeDirectionBetweenPlayerAndObject");
		useCustomIconButtonInfo = serializedObject.FindProperty ("useCustomIconButtonInfo");
		customIconButtonInfoName = serializedObject.FindProperty ("customIconButtonInfoName");
		useTransformForStringAction = serializedObject.FindProperty ("useTransformForStringAction");
		useSeparatedTransformForEveryView = serializedObject.FindProperty ("useSeparatedTransformForEveryView");
		transformForThirdPerson = serializedObject.FindProperty ("transformForThirdPerson");
		transformForFirstPerson = serializedObject.FindProperty ("transformForFirstPerson");
		transformForStringAction = serializedObject.FindProperty ("transformForStringAction");
		actionOffset = serializedObject.FindProperty ("actionOffset");
		useLocalOffset = serializedObject.FindProperty ("useLocalOffset");
		iconEnabled = serializedObject.FindProperty ("iconEnabled");

		checkIfObstacleBetweenDeviceAndPlayer = serializedObject.FindProperty ("checkIfObstacleBetweenDeviceAndPlayer");

		showIconTexture = serializedObject.FindProperty ("showIconTexture");
		iconTexture = serializedObject.FindProperty ("iconTexture");

		useObjectDescription = serializedObject.FindProperty ("useObjectDescription");
		objectDescription = serializedObject.FindProperty ("objectDescription");
		descriptionFontSize = serializedObject.FindProperty ("descriptionFontSize");

		showObjectAmount = serializedObject.FindProperty ("showObjectAmount");
		objectAmount = serializedObject.FindProperty ("objectAmount");

		useHoldInteractionButton = serializedObject.FindProperty ("useHoldInteractionButton");
		holdInteractionButtonDuration = serializedObject.FindProperty ("holdInteractionButtonDuration");

		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoColor = serializedObject.FindProperty ("gizmoColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");

		device = (deviceStringAction)target;
	}

	void OnSceneGUI ()
	{   
		if (device.showGizmo) {
			style.normal.textColor = device.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;

			labelPosition = device.gameObject.transform.position + device.gameObject.transform.up * device.actionOffset;

			if (device.useTransformForStringAction) {
				if (device.useSeparatedTransformForEveryView) {
					currentTransform = device.transformForThirdPerson;

					if (currentTransform != null) {
						currentRotation = Tools.pivotRotation == PivotRotation.Local ? currentTransform.rotation : Quaternion.identity;

						EditorGUI.BeginChangeCheck ();

						oldPoint = currentTransform.position;
						oldPoint = Handles.DoPositionHandle (oldPoint, currentRotation);

						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentTransform, "move Third Person position Handle");
							currentTransform.position = oldPoint;
						}
					
						labelPosition = currentTransform.position;
						Handles.Label (labelPosition, "Third Person \n position", style);
					}

					currentTransform = device.transformForFirstPerson;

					if (currentTransform != null) {
						currentRotation = Tools.pivotRotation == PivotRotation.Local ? currentTransform.rotation : Quaternion.identity;

						EditorGUI.BeginChangeCheck ();

						oldPoint = currentTransform.position;
						oldPoint = Handles.DoPositionHandle (oldPoint, currentRotation);

						if (EditorGUI.EndChangeCheck ()) {
							Undo.RecordObject (currentTransform, "Move First Person position Handle");
							currentTransform.position = oldPoint;
						}

						labelPosition = currentTransform.position;
						Handles.Label (labelPosition, "First Person \n position", style);
					}
				} else {
					if (device.transformForStringAction != null) {
						
						labelPosition = device.transformForStringAction.position;
						Handles.Label (labelPosition, "Device String \n position", style);
					}
				}
			} else {
				Handles.Label (labelPosition, "Device String \n position", style);
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (deviceName);
		EditorGUILayout.PropertyField (deviceAction);
		EditorGUILayout.PropertyField (secondaryDeviceAction);
		EditorGUILayout.PropertyField (reloadDeviceActionOnPress);

		EditorGUILayout.PropertyField (setUsingDeviceState);

		EditorGUILayout.PropertyField (useRaycastToDetectDeviceParts);
		EditorGUILayout.PropertyField (ignoreUseOnlyDeviceIfVisibleOnCamera);
		EditorGUILayout.PropertyField (useFixedDeviceIconPosition);

		EditorGUILayout.PropertyField (useCustomDeviceTransformPosition);
		if (useCustomDeviceTransformPosition.boolValue) {
			EditorGUILayout.PropertyField (customDeviceTransformPosition);
		}
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (checkIfObstacleBetweenDeviceAndPlayer);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Show/Hide Interaction Icon Settings", "window");

		EditorGUILayout.PropertyField (hideIconOnPress);
		EditorGUILayout.PropertyField (disableIconOnPress);

		EditorGUILayout.PropertyField (hideIconOnUseDevice);
		EditorGUILayout.PropertyField (showIconOnStopUseDevice); 

		EditorGUILayout.PropertyField (showIcon);
		EditorGUILayout.PropertyField (showTouchIconButton);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Multiple Interaction Settings", "window");

		EditorGUILayout.PropertyField (useMultipleInteraction);
		if (useMultipleInteraction.boolValue) {
			EditorGUILayout.Space ();

			showSimpleList (multipleInteractionNameList, "Mutliple Interaction Name List");
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Text Font Settings", "window");
		EditorGUILayout.PropertyField (setTextFontSizeActive);
		if (setTextFontSizeActive.boolValue) {
			EditorGUILayout.PropertyField (textFontSize);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Min Distance And Angle Settings", "window");
		EditorGUILayout.PropertyField (useCustomMinDistanceToUseDevice);
		if (useCustomMinDistanceToUseDevice.boolValue) {
			EditorGUILayout.PropertyField (customMinDistanceToUseDevice);
		}

		EditorGUILayout.PropertyField (useCustomMinAngleToUse);
		if (useCustomMinAngleToUse.boolValue) {
			EditorGUILayout.PropertyField (customMinAngleToUseDevice);
			EditorGUILayout.PropertyField (useRelativeDirectionBetweenPlayerAndObject);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use Custom Icon Button Info Settings", "window");
		EditorGUILayout.PropertyField (useCustomIconButtonInfo);
		if (useCustomIconButtonInfo.boolValue) {
			EditorGUILayout.PropertyField (customIconButtonInfoName);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Use Transform Settings", "window");
		EditorGUILayout.PropertyField (useTransformForStringAction);
		if (useTransformForStringAction.boolValue) {
			EditorGUILayout.PropertyField (useSeparatedTransformForEveryView);
			if (useSeparatedTransformForEveryView.boolValue) {
				EditorGUILayout.PropertyField (transformForThirdPerson);
				EditorGUILayout.PropertyField (transformForFirstPerson);
			} else {
				EditorGUILayout.PropertyField (transformForStringAction);
			}
		}
		GUILayout.EndVertical ();

		if (!useTransformForStringAction.boolValue) {
			
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (actionOffset);
			EditorGUILayout.PropertyField (useLocalOffset);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Object Info Settings", "window");
		EditorGUILayout.PropertyField (showIconTexture);
		if (showIconTexture.boolValue) {
			EditorGUILayout.PropertyField (iconTexture);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useObjectDescription);
		if (useObjectDescription.boolValue) {
			EditorGUILayout.PropertyField (objectDescription);
			EditorGUILayout.PropertyField (descriptionFontSize);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (showObjectAmount);
		if (showObjectAmount.boolValue) {
			EditorGUILayout.PropertyField (objectAmount);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useHoldInteractionButton);
		if (useHoldInteractionButton.boolValue) {
			EditorGUILayout.PropertyField (holdInteractionButtonDuration);
		}
	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		GUILayout.Label ("Is Icon Enabled\t" + iconEnabled.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (gizmoColor);
			EditorGUILayout.PropertyField (gizmoRadius);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + listName)) {
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

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}       
	}
}
#endif