using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(hideCharacterFixedPlaceSystem))]
public class hideCharacterFixedPlaceSystemEditor : Editor
{
	SerializedProperty cameraTransformProp;
	SerializedProperty pivotTransform;
	SerializedProperty cameraPositionTransform;
	SerializedProperty canResetCameraRotation;
	SerializedProperty canResetCameraPosition;
	SerializedProperty useCharacterStateIcon;
	SerializedProperty visibleCharacterStateName;
	SerializedProperty notVisibleCharacterStateName;
	SerializedProperty checkIfDetectedWhileHidden;
	SerializedProperty hidingCharacter;
	SerializedProperty cameraCanRotate;
	SerializedProperty rotationSpeed;
	SerializedProperty smoothCameraRotationSpeed;
	SerializedProperty useSpringRotation;
	SerializedProperty springRotationDelay;
	SerializedProperty cameraCanMove;
	SerializedProperty moveCameraSpeed;
	SerializedProperty smoothMoveCameraSpeed;
	SerializedProperty useSpringMovement;
	SerializedProperty springMovementDelay;
	SerializedProperty hiddenEvent;
	SerializedProperty hideEventDelay;
	SerializedProperty visbleEvent;
	SerializedProperty visibleEventDelay;
	SerializedProperty setHiddenFov;
	SerializedProperty hiddenFov;
	SerializedProperty zoomEnabled;
	SerializedProperty zoomSpeed;
	SerializedProperty maxZoom;
	SerializedProperty minZoom;
	SerializedProperty useMessageWhenUnableToHide;
	SerializedProperty unableToHideMessage;
	SerializedProperty showMessageTime;
	SerializedProperty activateActionScreen;
	SerializedProperty actionScreenName;
	SerializedProperty showGizmo;
	SerializedProperty gizmoColor;
	SerializedProperty arcGizmoRadius;
	SerializedProperty gizmoArrowLength;
	SerializedProperty gizmoArrowAngle;
	SerializedProperty gizmoArrowColor;

	hideCharacterFixedPlaceSystem manager;

	Vector2 rangeAngleX;
	Vector2 rangeAngleY;
	Vector3 position;
	Transform cameraTransform;

	void OnEnable ()
	{
		cameraTransformProp = serializedObject.FindProperty ("cameraTransform");
		pivotTransform = serializedObject.FindProperty ("pivotTransform");
		cameraPositionTransform = serializedObject.FindProperty ("cameraPositionTransform");
		canResetCameraRotation = serializedObject.FindProperty ("canResetCameraRotation");
		canResetCameraPosition = serializedObject.FindProperty ("canResetCameraPosition");
		useCharacterStateIcon = serializedObject.FindProperty ("useCharacterStateIcon");
		visibleCharacterStateName = serializedObject.FindProperty ("visibleCharacterStateName");
		notVisibleCharacterStateName = serializedObject.FindProperty ("notVisibleCharacterStateName");
		checkIfDetectedWhileHidden = serializedObject.FindProperty ("checkIfDetectedWhileHidden");
		hidingCharacter = serializedObject.FindProperty ("hidingCharacter");
		cameraCanRotate = serializedObject.FindProperty ("cameraCanRotate");
		rotationSpeed = serializedObject.FindProperty("rotationSpeed");
		smoothCameraRotationSpeed = serializedObject.FindProperty ("smoothCameraRotationSpeed");
		useSpringRotation = serializedObject.FindProperty ("useSpringRotation");
		springRotationDelay = serializedObject.FindProperty ("springRotationDelay");
		cameraCanMove = serializedObject.FindProperty ("cameraCanMove");
		moveCameraSpeed = serializedObject.FindProperty ("moveCameraSpeed");
		smoothMoveCameraSpeed = serializedObject.FindProperty ("smoothMoveCameraSpeed");
		useSpringMovement = serializedObject.FindProperty ("useSpringMovement");
		springMovementDelay = serializedObject.FindProperty ("springMovementDelay");
		hiddenEvent = serializedObject.FindProperty ("hidenEvent");
		hideEventDelay = serializedObject.FindProperty ("hideEventDelay");
		visbleEvent = serializedObject.FindProperty ("visbleEvent");
		visibleEventDelay = serializedObject.FindProperty ("visibleEventDelay");
		setHiddenFov = serializedObject.FindProperty ("setHiddenFov");
		hiddenFov = serializedObject.FindProperty ("hiddenFov");
		zoomEnabled = serializedObject.FindProperty ("zoomEnabled");
		zoomSpeed = serializedObject.FindProperty ("zoomSpeed");
		maxZoom = serializedObject.FindProperty ("maxZoom");
		minZoom = serializedObject.FindProperty ("minZoom");
		useMessageWhenUnableToHide = serializedObject.FindProperty ("useMessageWhenUnableToHide");
		unableToHideMessage = serializedObject.FindProperty ("unableToHideMessage");
		showMessageTime = serializedObject.FindProperty ("showMessageTime");
		activateActionScreen = serializedObject.FindProperty ("activateActionScreen");
		actionScreenName = serializedObject.FindProperty ("actionScreenName");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoColor = serializedObject.FindProperty ("gizmoColor");
		arcGizmoRadius = serializedObject.FindProperty ("arcGizmoRadius");
		gizmoArrowLength = serializedObject.FindProperty ("gizmoArrowLength");
		gizmoArrowAngle = serializedObject.FindProperty ("gizmoArrowAngle");
		gizmoArrowColor = serializedObject.FindProperty ("gizmoArrowColor");

		manager = (hideCharacterFixedPlaceSystem)target;
	}

	void OnSceneGUI ()
	{
		if (manager.showGizmo) {
			Handles.color = Color.white;

			rangeAngleX = manager.rangeAngleX;
			rangeAngleY = manager.rangeAngleY;
			cameraTransform = manager.cameraPositionTransform;
			position = cameraTransform.position;

			Handles.DrawWireArc (position, -cameraTransform.up, cameraTransform.forward, -rangeAngleY.x, manager.arcGizmoRadius);
			Handles.DrawWireArc (position, cameraTransform.up, cameraTransform.forward, rangeAngleY.y, manager.arcGizmoRadius);

			Handles.color = Color.red;
			Handles.DrawWireArc (position, cameraTransform.up, -cameraTransform.forward, (180 - Mathf.Abs (rangeAngleY.x)), manager.arcGizmoRadius);
			Handles.DrawWireArc (position, -cameraTransform.up, -cameraTransform.forward, (180 - Mathf.Abs (rangeAngleY.y)), manager.arcGizmoRadius);

			Handles.color = Color.white;
			Handles.DrawWireArc (position, -cameraTransform.right, cameraTransform.forward, -rangeAngleX.x, manager.arcGizmoRadius);
			Handles.DrawWireArc (position, cameraTransform.right, cameraTransform.forward, rangeAngleX.y, manager.arcGizmoRadius);

			Handles.color = Color.red;
			Handles.DrawWireArc (position, cameraTransform.right, -cameraTransform.forward, (180 - Mathf.Abs (rangeAngleX.x)), manager.arcGizmoRadius);
			Handles.DrawWireArc (position, -cameraTransform.right, -cameraTransform.forward, (180 - Mathf.Abs (rangeAngleX.y)), manager.arcGizmoRadius);

			string text = "Camera Range\n" + "Y: " + (Mathf.Abs (rangeAngleX.x) + rangeAngleX.y) + "\n" + "X: " + (Mathf.Abs (rangeAngleY.x) + rangeAngleY.y);

			Handles.color = Color.red;
			Handles.Label (position + cameraTransform.up, text);	
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (cameraTransformProp);
		EditorGUILayout.PropertyField (pivotTransform);
		EditorGUILayout.PropertyField (cameraPositionTransform);
		EditorGUILayout.PropertyField (canResetCameraRotation);
		EditorGUILayout.PropertyField (canResetCameraPosition);
		EditorGUILayout.PropertyField (useCharacterStateIcon);
		EditorGUILayout.PropertyField (visibleCharacterStateName);
		EditorGUILayout.PropertyField (notVisibleCharacterStateName);
		EditorGUILayout.PropertyField (checkIfDetectedWhileHidden);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hide System State", "window");
		GUILayout.Label ("Hiding Character\t " + hidingCharacter.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Rotation Settings", "window");
		EditorGUILayout.PropertyField (cameraCanRotate);
		EditorGUILayout.PropertyField (rotationSpeed);
		EditorGUILayout.PropertyField (smoothCameraRotationSpeed);
		EditorGUILayout.PropertyField (useSpringRotation);
		if (useSpringRotation.boolValue) {
			EditorGUILayout.PropertyField (springRotationDelay);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Movement Settings", "window");
		EditorGUILayout.PropertyField (cameraCanMove);
		EditorGUILayout.PropertyField (moveCameraSpeed);
		EditorGUILayout.PropertyField (smoothMoveCameraSpeed);
		EditorGUILayout.PropertyField (useSpringMovement);
		if (useSpringMovement.boolValue) {
			EditorGUILayout.PropertyField (springMovementDelay);
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hide/Visible Event Settings", "window");
		EditorGUILayout.PropertyField (hiddenEvent);
		EditorGUILayout.PropertyField (hideEventDelay);
		EditorGUILayout.PropertyField (visbleEvent);
		EditorGUILayout.PropertyField (visibleEventDelay);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Rotation Range Settings", "window");

		GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
		GUILayout.BeginHorizontal ();
		manager.rangeAngleY.x = EditorGUILayout.FloatField (manager.rangeAngleY.x, GUILayout.MaxWidth (50));
		EditorGUILayout.MinMaxSlider (ref manager.rangeAngleY.x, ref manager.rangeAngleY.y, -180, 180);
		manager.rangeAngleY.y = EditorGUILayout.FloatField (manager.rangeAngleY.y, GUILayout.MaxWidth (50));
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
		GUILayout.BeginHorizontal ();
		manager.rangeAngleX.x = EditorGUILayout.FloatField (manager.rangeAngleX.x, GUILayout.MaxWidth (50));
		EditorGUILayout.MinMaxSlider (ref manager.rangeAngleX.x, ref manager.rangeAngleX.y, -180, 180);
		manager.rangeAngleX.y = EditorGUILayout.FloatField (manager.rangeAngleX.y, GUILayout.MaxWidth (50));
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Movement Range Settings", "window");

		GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
		GUILayout.BeginHorizontal ();
		manager.moveCameraLimitsY.x = EditorGUILayout.FloatField (manager.moveCameraLimitsY.x, GUILayout.MaxWidth (50));
		EditorGUILayout.MinMaxSlider (ref manager.moveCameraLimitsY.x, ref manager.moveCameraLimitsY.y, -3, 3);
		manager.moveCameraLimitsY.y = EditorGUILayout.FloatField (manager.moveCameraLimitsY.y, GUILayout.MaxWidth (50));
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
		GUILayout.BeginHorizontal ();
		manager.moveCameraLimitsX.x = EditorGUILayout.FloatField (manager.moveCameraLimitsX.x, GUILayout.MaxWidth (50));
		EditorGUILayout.MinMaxSlider (ref manager.moveCameraLimitsX.x, ref manager.moveCameraLimitsX.y, -3, 3);
		manager.moveCameraLimitsX.y = EditorGUILayout.FloatField (manager.moveCameraLimitsX.y, GUILayout.MaxWidth (50));
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera FOV Settings", "window");
		EditorGUILayout.PropertyField (setHiddenFov);
		if (setHiddenFov.boolValue) {
			EditorGUILayout.PropertyField (hiddenFov);
		}
		EditorGUILayout.PropertyField (zoomEnabled);
		if (zoomEnabled.boolValue) {
			EditorGUILayout.PropertyField (zoomSpeed);
			EditorGUILayout.PropertyField (maxZoom);
			EditorGUILayout.PropertyField (minZoom);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Unable To Hide Message Settings", "window");
		EditorGUILayout.PropertyField (useMessageWhenUnableToHide);
		if (useMessageWhenUnableToHide.boolValue) {
			EditorGUILayout.PropertyField (unableToHideMessage);
			EditorGUILayout.PropertyField (showMessageTime);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Action Screen Settings", "window");
		EditorGUILayout.PropertyField (activateActionScreen);
		if (activateActionScreen.boolValue) {
			EditorGUILayout.PropertyField (actionScreenName);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoColor);
			EditorGUILayout.PropertyField (arcGizmoRadius);
			EditorGUILayout.PropertyField (gizmoArrowLength);
			EditorGUILayout.PropertyField (gizmoArrowAngle);
			EditorGUILayout.PropertyField (gizmoArrowColor);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}
}
#endif
