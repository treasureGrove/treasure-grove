using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(vehicleCameraController))]
[CanEditMultipleObjects]
public class vehicleCameraControllerEditor : Editor
{
	SerializedProperty thirdPersonVerticalRotationSpeed;
	SerializedProperty thirdPersonHorizontalRotationSpeed;
	SerializedProperty firstPersonVerticalRotationSpeed;
	SerializedProperty firstPersonHorizontalRotationSpeed;
	SerializedProperty clipCastRadius;
	SerializedProperty backClipSpeed;
	SerializedProperty maximumBoostDistance;
	SerializedProperty cameraBoostSpeed;
	SerializedProperty rotationDamping;
	SerializedProperty cameraChangeEnabled;
	SerializedProperty smoothBetweenState;
	SerializedProperty layer;
	SerializedProperty smoothTransitionsInNewCameraFov;
	SerializedProperty useSmoothCameraRotation;
	SerializedProperty useSmoothCameraRotationThirdPerson;
	SerializedProperty smoothCameraRotationSpeedVerticalThirdPerson;
	SerializedProperty smoothCameraRotationSpeedHorizontalThirdPerson;
	SerializedProperty useSmoothCameraRotationFirstPerson;
	SerializedProperty smoothCameraRotationSpeedVerticalFirstPerson;
	SerializedProperty smoothCameraRotationSpeedHorizontalFirstPerson;
	SerializedProperty zoomEnabled;

	SerializedProperty defaultStateName;
	SerializedProperty currentStateName;
	SerializedProperty cameraStateIndex;

	SerializedProperty vehicleCameraStates;
	SerializedProperty cameraPaused;
	SerializedProperty isFirstPerson;
	SerializedProperty usingZoomOn;
	SerializedProperty drivingVehicle;
	SerializedProperty lookAngle;
	SerializedProperty showShakeSettings;
	SerializedProperty useDamageShake;
	SerializedProperty useDamageShakeInThirdPerson;
	SerializedProperty thirdPersonDamageShake;
	SerializedProperty useDamageShakeInFirstPerson;
	SerializedProperty firstPersonDamageShake;
	SerializedProperty vehicle;

	SerializedProperty vehicleTransformToFollow;

	SerializedProperty IKDrivingManager;
	SerializedProperty weaponManager;
	SerializedProperty hudManager;
	SerializedProperty actionManager;
	SerializedProperty gravityControl;
	SerializedProperty mainRigidbody;
	SerializedProperty shakingManager;

	SerializedProperty sendCurrentCameraTransformOnChangeState;
	SerializedProperty eventToSendCurrentCameraTransformOnChangeState;

	SerializedProperty showGizmo;
	SerializedProperty gizmoRadius;
	SerializedProperty labelGizmoColor;
	SerializedProperty showCameraDirectionGizmo;
	SerializedProperty gizmoArrowLength;
	SerializedProperty gizmoArrowLineLength;
	SerializedProperty gizmoArrowAngle;

	SerializedProperty useSmoothCameraFollow;
	SerializedProperty smoothCameraFollowSpeed;
	SerializedProperty smoothCameraFollowMaxDistance;
	SerializedProperty smoothCameraFollowMaxDistanceSpeed;

	SerializedProperty resetCameraRotationOnGetOff;


	vehicleCameraController manager;

	Color defBackgroundColor;

	bool expanded;

	bool showVehicleElements;

	string buttonMessage;

	GUIStyle style = new GUIStyle ();

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		thirdPersonVerticalRotationSpeed = serializedObject.FindProperty ("thirdPersonVerticalRotationSpeed");
		thirdPersonHorizontalRotationSpeed = serializedObject.FindProperty ("thirdPersonHorizontalRotationSpeed");
		firstPersonVerticalRotationSpeed = serializedObject.FindProperty ("firstPersonVerticalRotationSpeed");
		firstPersonHorizontalRotationSpeed = serializedObject.FindProperty ("firstPersonHorizontalRotationSpeed");
		clipCastRadius = serializedObject.FindProperty ("clipCastRadius");
		backClipSpeed = serializedObject.FindProperty ("backClipSpeed");
		maximumBoostDistance = serializedObject.FindProperty ("maximumBoostDistance");
		cameraBoostSpeed = serializedObject.FindProperty ("cameraBoostSpeed");
		rotationDamping = serializedObject.FindProperty ("rotationDamping");
		cameraChangeEnabled = serializedObject.FindProperty ("cameraChangeEnabled");
		smoothBetweenState = serializedObject.FindProperty ("smoothBetweenState");
		layer = serializedObject.FindProperty ("layer");
		smoothTransitionsInNewCameraFov = serializedObject.FindProperty ("smoothTransitionsInNewCameraFov");
		useSmoothCameraRotation = serializedObject.FindProperty ("useSmoothCameraRotation");
		useSmoothCameraRotationThirdPerson = serializedObject.FindProperty ("useSmoothCameraRotationThirdPerson");
		smoothCameraRotationSpeedVerticalThirdPerson = serializedObject.FindProperty ("smoothCameraRotationSpeedVerticalThirdPerson");
		smoothCameraRotationSpeedHorizontalThirdPerson = serializedObject.FindProperty ("smoothCameraRotationSpeedHorizontalThirdPerson");
		useSmoothCameraRotationFirstPerson = serializedObject.FindProperty ("useSmoothCameraRotationFirstPerson");
		smoothCameraRotationSpeedVerticalFirstPerson = serializedObject.FindProperty ("smoothCameraRotationSpeedVerticalFirstPerson");
		smoothCameraRotationSpeedHorizontalFirstPerson = serializedObject.FindProperty ("smoothCameraRotationSpeedHorizontalFirstPerson");
		zoomEnabled = serializedObject.FindProperty ("zoomEnabled");

		defaultStateName = serializedObject.FindProperty ("defaultStateName");
		currentStateName = serializedObject.FindProperty ("currentStateName");

		cameraStateIndex = serializedObject.FindProperty ("cameraStateIndex");

		vehicleCameraStates = serializedObject.FindProperty ("vehicleCameraStates");
		cameraPaused = serializedObject.FindProperty ("cameraPaused");
		isFirstPerson = serializedObject.FindProperty ("isFirstPerson");
		usingZoomOn = serializedObject.FindProperty ("usingZoomOn");
		drivingVehicle = serializedObject.FindProperty ("drivingVehicle");
		lookAngle = serializedObject.FindProperty ("lookAngle");
		showShakeSettings = serializedObject.FindProperty ("showShakeSettings");
		useDamageShake = serializedObject.FindProperty ("shakeSettings.useDamageShake");
		useDamageShakeInThirdPerson = serializedObject.FindProperty ("shakeSettings.useDamageShakeInThirdPerson");
		thirdPersonDamageShake = serializedObject.FindProperty ("shakeSettings.thirdPersonDamageShake");
		useDamageShakeInFirstPerson = serializedObject.FindProperty ("shakeSettings.useDamageShakeInFirstPerson");
		firstPersonDamageShake = serializedObject.FindProperty ("shakeSettings.firstPersonDamageShake");
		vehicle = serializedObject.FindProperty ("vehicle");

		vehicleTransformToFollow = serializedObject.FindProperty ("vehicleTransformToFollow");

		IKDrivingManager = serializedObject.FindProperty ("IKDrivingManager");
		weaponManager = serializedObject.FindProperty ("weaponManager");
		hudManager = serializedObject.FindProperty ("hudManager");
		actionManager = serializedObject.FindProperty ("actionManager");
		gravityControl = serializedObject.FindProperty ("gravityControl");
		mainRigidbody = serializedObject.FindProperty ("mainRigidbody");
		shakingManager = serializedObject.FindProperty ("shakingManager");

		sendCurrentCameraTransformOnChangeState = serializedObject.FindProperty ("sendCurrentCameraTransformOnChangeState");
		eventToSendCurrentCameraTransformOnChangeState = serializedObject.FindProperty ("eventToSendCurrentCameraTransformOnChangeState");

		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		labelGizmoColor = serializedObject.FindProperty ("labelGizmoColor");
		showCameraDirectionGizmo = serializedObject.FindProperty ("showCameraDirectionGizmo");
		gizmoArrowLength = serializedObject.FindProperty ("gizmoArrowLength");
		gizmoArrowLineLength = serializedObject.FindProperty ("gizmoArrowLineLength");
		gizmoArrowAngle = serializedObject.FindProperty ("gizmoArrowAngle");

		useSmoothCameraFollow = serializedObject.FindProperty ("useSmoothCameraFollow");
		smoothCameraFollowSpeed = serializedObject.FindProperty ("smoothCameraFollowSpeed");
		smoothCameraFollowMaxDistance = serializedObject.FindProperty ("smoothCameraFollowMaxDistance");
		smoothCameraFollowMaxDistanceSpeed = serializedObject.FindProperty ("smoothCameraFollowMaxDistanceSpeed");

		resetCameraRotationOnGetOff = serializedObject.FindProperty ("resetCameraRotationOnGetOff");

		manager = (vehicleCameraController)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				if (manager.gameObject == Selection.activeGameObject) {
					
					style.normal.textColor = labelGizmoColor.colorValue;
					style.alignment = TextAnchor.MiddleCenter;

					for (int i = 0; i < manager.vehicleCameraStates.Count; i++) {
						if (manager.vehicleCameraStates [i].showGizmo) {
							Handles.color = manager.vehicleCameraStates [i].gizmoColor;

							Handles.Label (manager.vehicleCameraStates [i].cameraTransform.position +
							(manager.transform.up * manager.vehicleCameraStates [i].labelGizmoOffset), manager.vehicleCameraStates [i].name, style);						
						}
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

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (thirdPersonVerticalRotationSpeed);
		EditorGUILayout.PropertyField (thirdPersonHorizontalRotationSpeed);
		EditorGUILayout.PropertyField (firstPersonVerticalRotationSpeed);
		EditorGUILayout.PropertyField (firstPersonHorizontalRotationSpeed);

		EditorGUILayout.PropertyField (clipCastRadius);
		EditorGUILayout.PropertyField (backClipSpeed);
		EditorGUILayout.PropertyField (maximumBoostDistance);
		EditorGUILayout.PropertyField (cameraBoostSpeed);
		EditorGUILayout.PropertyField (rotationDamping);
		EditorGUILayout.PropertyField (cameraChangeEnabled);
		EditorGUILayout.PropertyField (smoothBetweenState);
		EditorGUILayout.PropertyField (layer, new GUIContent ("Collision Layer"));
		EditorGUILayout.PropertyField (smoothTransitionsInNewCameraFov);

		EditorGUILayout.PropertyField (resetCameraRotationOnGetOff);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Smooth Camera Rotation Settings", "window");
		EditorGUILayout.PropertyField (useSmoothCameraRotation);
		if (useSmoothCameraRotation.boolValue) {
			EditorGUILayout.PropertyField (useSmoothCameraRotationThirdPerson);
			if (useSmoothCameraRotationThirdPerson.boolValue) {
				EditorGUILayout.PropertyField (smoothCameraRotationSpeedVerticalThirdPerson);
				EditorGUILayout.PropertyField (smoothCameraRotationSpeedHorizontalThirdPerson);
			}
			EditorGUILayout.PropertyField (useSmoothCameraRotationFirstPerson);
			if (useSmoothCameraRotationFirstPerson.boolValue) {
				EditorGUILayout.PropertyField (smoothCameraRotationSpeedVerticalFirstPerson);
				EditorGUILayout.PropertyField (smoothCameraRotationSpeedHorizontalFirstPerson);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Follow Position Settings", "window");

		EditorGUILayout.PropertyField (useSmoothCameraFollow);
		if (useSmoothCameraFollow.boolValue) {
			EditorGUILayout.PropertyField (smoothCameraFollowSpeed);
			EditorGUILayout.PropertyField (smoothCameraFollowMaxDistance);
			EditorGUILayout.PropertyField (smoothCameraFollowMaxDistanceSpeed);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zoom Settings", "window");
		EditorGUILayout.PropertyField (zoomEnabled);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		GUILayout.BeginVertical ("Vehicle Camera States", "window");
		EditorGUILayout.PropertyField (defaultStateName);
		EditorGUILayout.PropertyField (currentStateName);

		EditorGUILayout.Space ();

		showUpperList (vehicleCameraStates);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (sendCurrentCameraTransformOnChangeState);
		if (sendCurrentCameraTransformOnChangeState.boolValue) {
			EditorGUILayout.PropertyField (eventToSendCurrentCameraTransformOnChangeState);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Camera Current State ", "window");

		GUILayout.Label ("Camera Paused\t" + cameraPaused.boolValue);
		GUILayout.Label ("First Person View\t" + isFirstPerson.boolValue);
		GUILayout.Label ("Using Zoom\t" + usingZoomOn.boolValue);
		GUILayout.Label ("Driving Vehicle\t" + drivingVehicle.boolValue);
		GUILayout.Label ("Look Angle\t\t" + lookAngle.vector2Value);
		GUILayout.Label ("Camera State Index\t" + cameraStateIndex.intValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		if (showShakeSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Shake Settings";
		} else {
			GUI.backgroundColor = defBackgroundColor;
			buttonMessage = "Show Shake Settings";
		}
		if (GUILayout.Button (buttonMessage)) {
			showShakeSettings.boolValue = !showShakeSettings.boolValue;
		}

		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndVertical ();

		if (showShakeSettings.boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Camera Shake Settings", "window");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Shake Settings when the vehicle receives Damage", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");
			EditorGUILayout.PropertyField (useDamageShake);
			if (useDamageShake.boolValue) {
			
				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (useDamageShakeInThirdPerson);
				if (useDamageShakeInThirdPerson.boolValue) {
					showShakeInfo (thirdPersonDamageShake);
				
					EditorGUILayout.Space ();
				}
				EditorGUILayout.PropertyField (useDamageShakeInFirstPerson);
				if (useDamageShakeInFirstPerson.boolValue) {
					showShakeInfo (firstPersonDamageShake);

					EditorGUILayout.Space ();

				}

				if (GUILayout.Button ("Test Shake")) {
					if (Application.isPlaying) {
						manager.setDamageCameraShake ();
					}
				}
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndVertical ();
		}
			
		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showVehicleElements) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Vehicle Elements";
		} else {
			GUI.backgroundColor = defBackgroundColor;
			buttonMessage = "Show vehicle Elements";
		}
		if (GUILayout.Button (buttonMessage)) {
			showVehicleElements = !showVehicleElements;
		}
		GUI.backgroundColor = defBackgroundColor;

		EditorGUILayout.EndHorizontal ();
		if (showVehicleElements) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vehicle Elements", "window");
			EditorGUILayout.PropertyField (vehicle);
			EditorGUILayout.PropertyField (vehicleTransformToFollow);
			EditorGUILayout.PropertyField (IKDrivingManager);
			EditorGUILayout.PropertyField (weaponManager);
			EditorGUILayout.PropertyField (hudManager);
			EditorGUILayout.PropertyField (actionManager);
			EditorGUILayout.PropertyField (gravityControl);
			EditorGUILayout.PropertyField (mainRigidbody);
			EditorGUILayout.PropertyField (shakingManager);
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoRadius);
			EditorGUILayout.PropertyField (labelGizmoColor);

			EditorGUILayout.PropertyField (showCameraDirectionGizmo);
			if (showCameraDirectionGizmo.boolValue) {
				EditorGUILayout.PropertyField (gizmoArrowLength);
				EditorGUILayout.PropertyField (gizmoArrowLineLength);
				EditorGUILayout.PropertyField (gizmoArrowAngle);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}

		EditorGUILayout.Space ();
	}

	void showCameraStateElementInfo (SerializedProperty list)
	{
		Color listButtonBackgroundColor;
		bool listGizmoSettings = list.FindPropertyRelative ("gizmoSettings").boolValue;

		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pivotTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraTransform"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enabled"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRotationInput"));
		if (list.FindPropertyRelative ("useRotationInput").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("xLimits"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("yLimits"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotateMainCameraOnFirstPerson"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewCameraFov"));
		if (list.FindPropertyRelative ("useNewCameraFov").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newCameraFov"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("firstPersonCamera"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cameraFixed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("smoothTransition"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useIdentityRotation"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canUnlockCursor"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCameraSteer"));
		if (list.FindPropertyRelative ("useCameraSteer").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDeadZoneForCameraSteer"));
			if (list.FindPropertyRelative ("useDeadZoneForCameraSteer").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("deadZoneAngle"));
			}
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Zoom Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canUseZoom"));
		if (list.FindPropertyRelative ("canUseZoom").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("zoomSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("zoomFovValue"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("verticalRotationSpeedZoomIn"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("horizontalRotationSpeedZoomIn"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		listButtonBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (listGizmoSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = listButtonBackgroundColor;
		}
		if (GUILayout.Button ("Gizmo Settings")) {
			listGizmoSettings = !listGizmoSettings;
		}
		GUI.backgroundColor = listButtonBackgroundColor;
		EditorGUILayout.EndHorizontal ();
		list.FindPropertyRelative ("gizmoSettings").boolValue = listGizmoSettings;
		if (listGizmoSettings) {
			
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Camera State Gizmo Settings", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("showGizmo"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("gizmoColor"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("labelGizmoOffset"));

			EditorGUILayout.Space ();

		}
		GUILayout.EndVertical ();
	}

	void showUpperList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of States: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add State")) {
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
						showCameraStateElementInfo (list.GetArrayElementAtIndex (i));
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

	void showShakeInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotation"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeDuration"));
		GUILayout.EndVertical ();
	}
}
#endif