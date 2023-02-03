using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(moveDeviceToCamera))]
[CanEditMultipleObjects]
public class moveDeviceToCameraEditor : Editor
{
	SerializedProperty deviceGameObject;
	SerializedProperty smoothCameraMovement;
	SerializedProperty useFixedLerpMovement;
	SerializedProperty fixedLerpMovementSpeed;

	SerializedProperty rotateObjectOnCameraDirectionEnabled;

	SerializedProperty cameraMovementSpeedThirdPerson;
	SerializedProperty cameraMovementSpeedFirstPerson;
	SerializedProperty distanceFromCamera;
	SerializedProperty layerToExaminateDevices;
	SerializedProperty activateExaminateObjectSystem;
	SerializedProperty objectHasActiveRigidbody;
	SerializedProperty disablePlayerMeshGameObject;
	SerializedProperty useBlurUIPanel;
	SerializedProperty disableAllPlayerHUD;
	SerializedProperty disableSecondaryPlayerHUD;
	SerializedProperty maxZoomDistance;
	SerializedProperty minZoomDistance;
	SerializedProperty zoomSpeed;
	SerializedProperty deviceTrigger;
	SerializedProperty colliderListToDisable;
	SerializedProperty colliderListButtons;
	SerializedProperty useListOfDisabledObjects;
	SerializedProperty disabledObjectList;
	SerializedProperty keepWeaponsIfCarrying;
	SerializedProperty drawWeaponsIfPreviouslyCarrying;
	SerializedProperty keepOnlyIfPlayerIsOnFirstPerson;
	SerializedProperty disableWeaponsDirectlyOnStart;

	SerializedProperty setNewMouseCursorControllerSpeed;
	SerializedProperty newMouseCursroControllerSpeed;

	SerializedProperty hideMouseCursorIfUsingGamepad;

	SerializedProperty examineObjectManager;
	SerializedProperty mainRigidbody;

	SerializedProperty disableInteractionTouchButtonOnUsingDevice;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		deviceGameObject = serializedObject.FindProperty ("deviceGameObject");
		smoothCameraMovement = serializedObject.FindProperty ("smoothCameraMovement");
		useFixedLerpMovement = serializedObject.FindProperty ("useFixedLerpMovement");
		fixedLerpMovementSpeed = serializedObject.FindProperty ("fixedLerpMovementSpeed");

		rotateObjectOnCameraDirectionEnabled = serializedObject.FindProperty ("rotateObjectOnCameraDirectionEnabled");

		cameraMovementSpeedThirdPerson = serializedObject.FindProperty ("cameraMovementSpeedThirdPerson");
		cameraMovementSpeedFirstPerson = serializedObject.FindProperty ("cameraMovementSpeedFirstPerson");
		distanceFromCamera = serializedObject.FindProperty ("distanceFromCamera");
		layerToExaminateDevices = serializedObject.FindProperty ("layerToExaminateDevices");
		activateExaminateObjectSystem = serializedObject.FindProperty ("activateExaminateObjectSystem");
		objectHasActiveRigidbody = serializedObject.FindProperty ("objectHasActiveRigidbody");
		disablePlayerMeshGameObject = serializedObject.FindProperty ("disablePlayerMeshGameObject");
		useBlurUIPanel = serializedObject.FindProperty ("useBlurUIPanel");
		disableAllPlayerHUD = serializedObject.FindProperty ("disableAllPlayerHUD");
		disableSecondaryPlayerHUD = serializedObject.FindProperty ("disableSecondaryPlayerHUD");
		maxZoomDistance = serializedObject.FindProperty ("maxZoomDistance");
		minZoomDistance = serializedObject.FindProperty ("minZoomDistance");
		zoomSpeed = serializedObject.FindProperty ("zoomSpeed");
		deviceTrigger = serializedObject.FindProperty ("deviceTrigger");
		colliderListToDisable = serializedObject.FindProperty ("colliderListToDisable");
		colliderListButtons = serializedObject.FindProperty ("colliderListButtons");
		useListOfDisabledObjects = serializedObject.FindProperty ("useListOfDisabledObjects");
		disabledObjectList = serializedObject.FindProperty ("disabledObjectList");
		keepWeaponsIfCarrying = serializedObject.FindProperty ("keepWeaponsIfCarrying");
		drawWeaponsIfPreviouslyCarrying = serializedObject.FindProperty ("drawWeaponsIfPreviouslyCarrying");
		keepOnlyIfPlayerIsOnFirstPerson = serializedObject.FindProperty ("keepOnlyIfPlayerIsOnFirstPerson");
		disableWeaponsDirectlyOnStart = serializedObject.FindProperty ("disableWeaponsDirectlyOnStart");

		setNewMouseCursorControllerSpeed = serializedObject.FindProperty ("setNewMouseCursorControllerSpeed");
		newMouseCursroControllerSpeed = serializedObject.FindProperty ("newMouseCursroControllerSpeed"); 

		hideMouseCursorIfUsingGamepad = serializedObject.FindProperty ("hideMouseCursorIfUsingGamepad"); 

		examineObjectManager = serializedObject.FindProperty ("examineObjectManager"); 
		mainRigidbody = serializedObject.FindProperty ("mainRigidbody"); 

		disableInteractionTouchButtonOnUsingDevice = serializedObject.FindProperty ("disableInteractionTouchButtonOnUsingDevice"); 
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (deviceGameObject);
		EditorGUILayout.PropertyField (smoothCameraMovement);
		if (smoothCameraMovement.boolValue) {
			EditorGUILayout.PropertyField (useFixedLerpMovement);
			if (useFixedLerpMovement.boolValue) {
				EditorGUILayout.PropertyField (fixedLerpMovementSpeed);
			} else {
				EditorGUILayout.PropertyField (cameraMovementSpeedThirdPerson);
				EditorGUILayout.PropertyField (cameraMovementSpeedFirstPerson);
			}
		}
		EditorGUILayout.PropertyField (distanceFromCamera);

		EditorGUILayout.PropertyField (rotateObjectOnCameraDirectionEnabled);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window");
		EditorGUILayout.PropertyField (layerToExaminateDevices);
		EditorGUILayout.PropertyField (activateExaminateObjectSystem);
		EditorGUILayout.PropertyField (objectHasActiveRigidbody);
		EditorGUILayout.PropertyField (disablePlayerMeshGameObject);
		EditorGUILayout.PropertyField (useBlurUIPanel);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (setNewMouseCursorControllerSpeed);
		if (setNewMouseCursorControllerSpeed.boolValue) {
			EditorGUILayout.PropertyField (newMouseCursroControllerSpeed);
		}

		EditorGUILayout.PropertyField (hideMouseCursorIfUsingGamepad);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("HUD Settings", "window");
		EditorGUILayout.PropertyField (disableAllPlayerHUD);
		EditorGUILayout.PropertyField (disableSecondaryPlayerHUD);
		EditorGUILayout.PropertyField (disableInteractionTouchButtonOnUsingDevice);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zoom Settings", "window");
		EditorGUILayout.PropertyField (maxZoomDistance);
		EditorGUILayout.PropertyField (minZoomDistance);
		EditorGUILayout.PropertyField (zoomSpeed);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Collider List Settings", "window");
		EditorGUILayout.PropertyField (deviceTrigger);
		showSimpleList (colliderListToDisable, "Collider");
		showSimpleList (colliderListButtons, "Collider");
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Disabled Object List Settings", "window");
		EditorGUILayout.PropertyField (useListOfDisabledObjects);
		if (useListOfDisabledObjects.boolValue) {
			showSimpleList (disabledObjectList, "Object");
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapons Settings", "window");
		EditorGUILayout.PropertyField (keepWeaponsIfCarrying);
		if (keepWeaponsIfCarrying.boolValue) {
			EditorGUILayout.PropertyField (drawWeaponsIfPreviouslyCarrying);
			EditorGUILayout.PropertyField (keepOnlyIfPlayerIsOnFirstPerson);
			EditorGUILayout.PropertyField (disableWeaponsDirectlyOnStart);
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (examineObjectManager);
		EditorGUILayout.PropertyField (mainRigidbody);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showSimpleList (SerializedProperty list, string listName)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of " + listName + "s: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add " + listName)) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();
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
	}
}
#endif