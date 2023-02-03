using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(moveCameraToDevice))]
[CanEditMultipleObjects]
public class moveCameraToDeviceEditor : Editor
{
	SerializedProperty cameraMovementActive;
	SerializedProperty cameraPosition;
	SerializedProperty smoothCameraMovement;
	SerializedProperty useFixedLerpMovement;
	SerializedProperty fixedLerpMovementSpeed;
	SerializedProperty cameraMovementSpeedThirdPerson;
	SerializedProperty cameraMovementSpeedFirstPerson;
	SerializedProperty secondMoveCameraToDevice;

	SerializedProperty unlockCursor;

	SerializedProperty ignoreHideCursorOnClick;

	SerializedProperty setNewMouseCursorControllerSpeed;
	SerializedProperty newMouseCursroControllerSpeed;

	SerializedProperty disablePlayerMeshGameObject;
	SerializedProperty enablePlayerMeshGameObjectIfFirstPersonActive;

	SerializedProperty customAlignPlayerTransform;

	SerializedProperty alignPlayerWithCameraPositionOnStopUseDevice;
	SerializedProperty alignPlayerWithCameraPositionOnStartUseDevice;

	SerializedProperty alignPlayerWithCameraRotationOnStartUseDevice;
	SerializedProperty alignPlayerWithCameraRotationOnStopUseDevice;

	SerializedProperty resetPlayerCameraDirection;

	SerializedProperty disableAllPlayerHUD;
	SerializedProperty disableSecondaryPlayerHUD;
	SerializedProperty disableTouchControls;
	SerializedProperty keepWeaponsIfCarrying;
	SerializedProperty drawWeaponsIfPreviouslyCarrying;
	SerializedProperty keepOnlyIfPlayerIsOnFirstPerson;
	SerializedProperty disableWeaponsDirectlyOnStart;
	SerializedProperty disableWeaponsCamera;
	SerializedProperty carryWeaponOnLowerPositionActive;
	SerializedProperty setPlayerCameraRotationOnExit;
	SerializedProperty playerCameraTransformThirdPerson;
	SerializedProperty playerPivotTransformThirdPerson;
	SerializedProperty playerCameraTransformFirstPerson;
	SerializedProperty playerPivotTransformFirstPerson;

	SerializedProperty disableInteractionTouchButtonOnUsingDevice;

	SerializedProperty ignoreMoveCameraFunctionEnabled;

	SerializedProperty showGizmo;
	SerializedProperty gizmoRadius;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoArrowLength;
	SerializedProperty gizmoArrowLineLength;
	SerializedProperty gizmoArrowAngle;
	SerializedProperty gizmoArrowColor;

	moveCameraToDevice manager;
	GUIStyle style = new GUIStyle ();

	void OnEnable ()
	{
		cameraMovementActive = serializedObject.FindProperty ("cameraMovementActive");
		cameraPosition = serializedObject.FindProperty ("cameraPosition");
		smoothCameraMovement = serializedObject.FindProperty ("smoothCameraMovement");
		useFixedLerpMovement = serializedObject.FindProperty ("useFixedLerpMovement");
		fixedLerpMovementSpeed = serializedObject.FindProperty ("fixedLerpMovementSpeed");
		cameraMovementSpeedThirdPerson = serializedObject.FindProperty ("cameraMovementSpeedThirdPerson");
		cameraMovementSpeedFirstPerson = serializedObject.FindProperty ("cameraMovementSpeedFirstPerson");
		secondMoveCameraToDevice = serializedObject.FindProperty ("secondMoveCameraToDevice");

		unlockCursor = serializedObject.FindProperty ("unlockCursor");

		ignoreHideCursorOnClick = serializedObject.FindProperty ("ignoreHideCursorOnClick");

		setNewMouseCursorControllerSpeed = serializedObject.FindProperty ("setNewMouseCursorControllerSpeed");
		newMouseCursroControllerSpeed = serializedObject.FindProperty ("newMouseCursroControllerSpeed"); 

		disablePlayerMeshGameObject = serializedObject.FindProperty ("disablePlayerMeshGameObject");
		enablePlayerMeshGameObjectIfFirstPersonActive = serializedObject.FindProperty ("enablePlayerMeshGameObjectIfFirstPersonActive");

		customAlignPlayerTransform = serializedObject.FindProperty ("customAlignPlayerTransform");

		alignPlayerWithCameraPositionOnStopUseDevice = serializedObject.FindProperty ("alignPlayerWithCameraPositionOnStopUseDevice");

		alignPlayerWithCameraPositionOnStartUseDevice = serializedObject.FindProperty ("alignPlayerWithCameraPositionOnStartUseDevice");

		alignPlayerWithCameraRotationOnStartUseDevice = serializedObject.FindProperty ("alignPlayerWithCameraRotationOnStartUseDevice");
		alignPlayerWithCameraRotationOnStopUseDevice = serializedObject.FindProperty ("alignPlayerWithCameraRotationOnStopUseDevice");

		resetPlayerCameraDirection = serializedObject.FindProperty ("resetPlayerCameraDirection");

		disableAllPlayerHUD = serializedObject.FindProperty ("disableAllPlayerHUD");
		disableSecondaryPlayerHUD = serializedObject.FindProperty ("disableSecondaryPlayerHUD");
		disableTouchControls = serializedObject.FindProperty ("disableTouchControls");
		keepWeaponsIfCarrying = serializedObject.FindProperty ("keepWeaponsIfCarrying");
		drawWeaponsIfPreviouslyCarrying = serializedObject.FindProperty ("drawWeaponsIfPreviouslyCarrying");
		keepOnlyIfPlayerIsOnFirstPerson = serializedObject.FindProperty ("keepOnlyIfPlayerIsOnFirstPerson");
		disableWeaponsDirectlyOnStart = serializedObject.FindProperty ("disableWeaponsDirectlyOnStart");
		disableWeaponsCamera = serializedObject.FindProperty ("disableWeaponsCamera");
		carryWeaponOnLowerPositionActive = serializedObject.FindProperty ("carryWeaponOnLowerPositionActive");
		setPlayerCameraRotationOnExit = serializedObject.FindProperty ("setPlayerCameraRotationOnExit");
		playerCameraTransformThirdPerson = serializedObject.FindProperty ("playerCameraTransformThirdPerson");
		playerPivotTransformThirdPerson = serializedObject.FindProperty ("playerPivotTransformThirdPerson");
		playerCameraTransformFirstPerson = serializedObject.FindProperty ("playerCameraTransformFirstPerson");
		playerPivotTransformFirstPerson = serializedObject.FindProperty ("playerPivotTransformFirstPerson");

		disableInteractionTouchButtonOnUsingDevice = serializedObject.FindProperty ("disableInteractionTouchButtonOnUsingDevice"); 

		ignoreMoveCameraFunctionEnabled = serializedObject.FindProperty ("ignoreMoveCameraFunctionEnabled"); 

		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoArrowLength = serializedObject.FindProperty ("gizmoArrowLength");
		gizmoArrowLineLength = serializedObject.FindProperty ("gizmoArrowLineLength");
		gizmoArrowAngle = serializedObject.FindProperty ("gizmoArrowAngle");
		gizmoArrowColor = serializedObject.FindProperty ("gizmoArrowColor");

		manager = (moveCameraToDevice)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				style.normal.textColor = manager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				if (manager.cameraPosition != null) {
					Handles.Label (manager.cameraPosition.transform.position, "Camera \n position", style);
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (cameraMovementActive);
		EditorGUILayout.PropertyField (cameraPosition);
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

		EditorGUILayout.PropertyField (ignoreMoveCameraFunctionEnabled);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window");
		EditorGUILayout.PropertyField (secondMoveCameraToDevice);

		EditorGUILayout.PropertyField (disablePlayerMeshGameObject);
		EditorGUILayout.PropertyField (enablePlayerMeshGameObjectIfFirstPersonActive);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (unlockCursor);
		if (unlockCursor.boolValue) {
			EditorGUILayout.PropertyField (ignoreHideCursorOnClick);
		}
		EditorGUILayout.PropertyField (setNewMouseCursorControllerSpeed);
		if (setNewMouseCursorControllerSpeed.boolValue) {
			EditorGUILayout.PropertyField (newMouseCursroControllerSpeed);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (alignPlayerWithCameraPositionOnStartUseDevice);
		EditorGUILayout.PropertyField (alignPlayerWithCameraPositionOnStopUseDevice);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (alignPlayerWithCameraRotationOnStartUseDevice);
		EditorGUILayout.PropertyField (alignPlayerWithCameraRotationOnStopUseDevice);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (customAlignPlayerTransform);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (resetPlayerCameraDirection);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("HUD Settings", "window");
		EditorGUILayout.PropertyField (disableAllPlayerHUD);
		EditorGUILayout.PropertyField (disableSecondaryPlayerHUD);
		EditorGUILayout.PropertyField (disableTouchControls);
		EditorGUILayout.PropertyField (disableInteractionTouchButtonOnUsingDevice);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapons Settings", "window");
		EditorGUILayout.PropertyField (keepWeaponsIfCarrying);
		if (keepWeaponsIfCarrying.boolValue) {
			EditorGUILayout.PropertyField (drawWeaponsIfPreviouslyCarrying);
			EditorGUILayout.PropertyField (keepOnlyIfPlayerIsOnFirstPerson);
			EditorGUILayout.PropertyField (disableWeaponsDirectlyOnStart);
		}
		EditorGUILayout.PropertyField (disableWeaponsCamera);
		EditorGUILayout.PropertyField (carryWeaponOnLowerPositionActive);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		GUILayout.BeginVertical ("Camera Rotation On Exit Settings", "window");
		EditorGUILayout.PropertyField (setPlayerCameraRotationOnExit);
		if (setPlayerCameraRotationOnExit.boolValue) {
			EditorGUILayout.PropertyField (playerCameraTransformThirdPerson);
			EditorGUILayout.PropertyField (playerPivotTransformThirdPerson);
			EditorGUILayout.PropertyField (playerCameraTransformFirstPerson);
			EditorGUILayout.PropertyField (playerPivotTransformFirstPerson);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoRadius);
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (gizmoArrowLength);
			EditorGUILayout.PropertyField (gizmoArrowLineLength);
			EditorGUILayout.PropertyField (gizmoArrowAngle);
			EditorGUILayout.PropertyField (gizmoArrowColor);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
#endif