using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(IKDrivingSystem))]
public class IKDrivingSystemEditor : Editor
{
	SerializedProperty vehicle;
	SerializedProperty vehicelCameraGameObject;

	SerializedProperty useCustomVehicleGameObject;
	SerializedProperty customVehicleGameObject;

	SerializedProperty playerIsAlwaysDriver;
	SerializedProperty hidePlayerFromNPCs;
	SerializedProperty playerVisibleInVehicle;
	SerializedProperty hidePlayerWeaponsWhileDriving;
	SerializedProperty canBeDrivenRemotely;


	SerializedProperty drawFireWeaponIfCarryingPreviously;
	SerializedProperty drawMeleeWeaponIfCarryingPreviously;

	SerializedProperty resetCameraRotationWhenGetOn;
	SerializedProperty resetCameraRotationWhenGetOff;
	SerializedProperty ejectPlayerWhenDestroyed;
	SerializedProperty ejectingPlayerForce;
	SerializedProperty activateFreeFloatingModeOnEject;
	SerializedProperty activateFreeFloatingModeOnEjectDelay;
	SerializedProperty useExplosionForceWhenDestroyed;
	SerializedProperty explosionRadius;
	SerializedProperty explosionForce;
	SerializedProperty explosionDamage;
	SerializedProperty ignoreShield;

	SerializedProperty damageTypeID;

	SerializedProperty useRemoteEventOnObjectsFound;
	SerializedProperty removeEventName;

	SerializedProperty pushCharactersOnExplosion;
	SerializedProperty applyExplosionForceToVehicles;
	SerializedProperty explosionForceToVehiclesMultiplier;
	SerializedProperty killObjectsInRadius;
	SerializedProperty forceMode;
	SerializedProperty useLayerMask;
	SerializedProperty layer;
	SerializedProperty addCollisionForceDirectionToPassengers;
	SerializedProperty extraCollisionForceAmount;
	SerializedProperty useMinCollisionForce;
	SerializedProperty minCollisionForce;
	SerializedProperty debugCollisionForce;
	SerializedProperty addAngularDirectionToPassengers;
	SerializedProperty vehicleStabilitySpeed;
	SerializedProperty extraAngularDirectioAmount;
	SerializedProperty startGameInThisVehicle;
	SerializedProperty playerForVehicle;
	SerializedProperty isBeingDrivenRemotely;
	SerializedProperty activateActionScreen;
	SerializedProperty actionScreenName;

	SerializedProperty useEventOnDriverGetOn;
	SerializedProperty eventOnDriverGetOn;
	SerializedProperty useEventOnDriverGetOff;
	SerializedProperty eventOnDriverGetOff;

	SerializedProperty sendPlayersEnterExitTriggerToEvent;
	SerializedProperty eventToSendPlayersEnterTriggerToEvent;
	SerializedProperty eventToSendPlayersExitTriggerToEvent;

	SerializedProperty useRemoteEventsOnPassengers;
	SerializedProperty remoteEventNameListGetOn;
	SerializedProperty remoteEventNameListGetOff;
	SerializedProperty actionManager;
	SerializedProperty vehicleCameraManager;
	SerializedProperty HUDManager;
	SerializedProperty currentVehicleWeaponSystem;
	SerializedProperty vehicleGravityManager;
	SerializedProperty mainCollider;
	SerializedProperty showSettings;
	SerializedProperty IKVehiclePassengersList;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;
	SerializedProperty useHandleForVertex;
	SerializedProperty handleRadius;
	SerializedProperty handleGizmoColor;
	SerializedProperty usePositionHandle;

	SerializedProperty passengerGameObjectList;

	SerializedProperty showDebugPrint;

	SerializedProperty setPlayerCameraStateOnGetOff;
	SerializedProperty setPlayerCameraStateOnFirstPersonOnGetOff;
	SerializedProperty playerCameraStateOnGetOff;

	SerializedProperty setVehicleCameraStateOnGetOn;
	SerializedProperty setVehicleCameraStateOnFirstPersonOnGetOn;
	SerializedProperty vehicleCameraStateOnGetOn;

	SerializedProperty resetAnimatorDrivingStateID;

	IKDrivingSystem manager;
	Color defBackgroundColor;
	bool expanded;

	Vector3 curretPositionHandle;
	Quaternion currentRotationHandle;

	Vector3 currentFreeHandlePosition;
	Vector3 newFreeHandlePosition;

	Vector3 snapValue = new Vector3 (.25f, .25f, .25f);

	bool showVehicleElements;

	string buttonMessage;

	GUIStyle style = new GUIStyle ();

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		vehicle = serializedObject.FindProperty ("vehicle");
		vehicelCameraGameObject = serializedObject.FindProperty ("vehicelCameraGameObject");

		useCustomVehicleGameObject = serializedObject.FindProperty ("useCustomVehicleGameObject");
		customVehicleGameObject = serializedObject.FindProperty ("customVehicleGameObject");


		playerIsAlwaysDriver = serializedObject.FindProperty ("playerIsAlwaysDriver");
		hidePlayerFromNPCs = serializedObject.FindProperty ("hidePlayerFromNPCs");
		playerVisibleInVehicle = serializedObject.FindProperty ("playerVisibleInVehicle");
		hidePlayerWeaponsWhileDriving = serializedObject.FindProperty ("hidePlayerWeaponsWhileDriving");
		canBeDrivenRemotely = serializedObject.FindProperty ("canBeDrivenRemotely");

		drawFireWeaponIfCarryingPreviously = serializedObject.FindProperty ("drawFireWeaponIfCarryingPreviously");
		drawMeleeWeaponIfCarryingPreviously = serializedObject.FindProperty ("drawMeleeWeaponIfCarryingPreviously");

		resetCameraRotationWhenGetOn = serializedObject.FindProperty ("resetCameraRotationWhenGetOn");
		resetCameraRotationWhenGetOff = serializedObject.FindProperty ("resetCameraRotationWhenGetOff");
		ejectPlayerWhenDestroyed = serializedObject.FindProperty ("ejectPlayerWhenDestroyed");
		ejectingPlayerForce = serializedObject.FindProperty ("ejectingPlayerForce");
		activateFreeFloatingModeOnEject = serializedObject.FindProperty ("activateFreeFloatingModeOnEject");
		activateFreeFloatingModeOnEjectDelay = serializedObject.FindProperty ("activateFreeFloatingModeOnEjectDelay");
		useExplosionForceWhenDestroyed = serializedObject.FindProperty ("useExplosionForceWhenDestroyed");
		explosionRadius = serializedObject.FindProperty ("explosionRadius");
		explosionForce = serializedObject.FindProperty ("explosionForce");
		explosionDamage = serializedObject.FindProperty ("explosionDamage");
		ignoreShield = serializedObject.FindProperty ("ignoreShield");

		damageTypeID = serializedObject.FindProperty ("damageTypeID");

		useRemoteEventOnObjectsFound = serializedObject.FindProperty ("useRemoteEventOnObjectsFound");
		removeEventName = serializedObject.FindProperty ("removeEventName");


		pushCharactersOnExplosion = serializedObject.FindProperty ("pushCharactersOnExplosion");
		applyExplosionForceToVehicles = serializedObject.FindProperty ("applyExplosionForceToVehicles");
		explosionForceToVehiclesMultiplier = serializedObject.FindProperty ("explosionForceToVehiclesMultiplier");
		killObjectsInRadius = serializedObject.FindProperty ("killObjectsInRadius");
		forceMode = serializedObject.FindProperty ("forceMode");
		useLayerMask = serializedObject.FindProperty ("useLayerMask");
		layer = serializedObject.FindProperty ("layer");
		addCollisionForceDirectionToPassengers = serializedObject.FindProperty ("addCollisionForceDirectionToPassengers");
		extraCollisionForceAmount = serializedObject.FindProperty ("extraCollisionForceAmount");
		useMinCollisionForce = serializedObject.FindProperty ("useMinCollisionForce");
		minCollisionForce = serializedObject.FindProperty ("minCollisionForce");
		debugCollisionForce = serializedObject.FindProperty ("debugCollisionForce");
		addAngularDirectionToPassengers = serializedObject.FindProperty ("addAngularDirectionToPassengers");
		vehicleStabilitySpeed = serializedObject.FindProperty ("vehicleStabilitySpeed");
		extraAngularDirectioAmount = serializedObject.FindProperty ("extraAngularDirectioAmount");
		startGameInThisVehicle = serializedObject.FindProperty ("startGameInThisVehicle");
		playerForVehicle = serializedObject.FindProperty ("playerForVehicle");
		isBeingDrivenRemotely = serializedObject.FindProperty ("isBeingDrivenRemotely");
		activateActionScreen = serializedObject.FindProperty ("activateActionScreen");
		actionScreenName = serializedObject.FindProperty ("actionScreenName");

		useEventOnDriverGetOn = serializedObject.FindProperty ("useEventOnDriverGetOn");
		eventOnDriverGetOn = serializedObject.FindProperty ("eventOnDriverGetOn");
		useEventOnDriverGetOff = serializedObject.FindProperty ("useEventOnDriverGetOff");
		eventOnDriverGetOff = serializedObject.FindProperty ("eventOnDriverGetOff");

		sendPlayersEnterExitTriggerToEvent = serializedObject.FindProperty ("sendPlayersEnterExitTriggerToEvent");
		eventToSendPlayersEnterTriggerToEvent = serializedObject.FindProperty ("eventToSendPlayersEnterTriggerToEvent");
		eventToSendPlayersExitTriggerToEvent = serializedObject.FindProperty ("eventToSendPlayersExitTriggerToEvent");

		useRemoteEventsOnPassengers = serializedObject.FindProperty ("useRemoteEventsOnPassengers");
		remoteEventNameListGetOn = serializedObject.FindProperty ("remoteEventNameListGetOn");
		remoteEventNameListGetOff = serializedObject.FindProperty ("remoteEventNameListGetOff");
		actionManager = serializedObject.FindProperty ("actionManager");
		vehicleCameraManager = serializedObject.FindProperty ("vehicleCameraManager");
		HUDManager = serializedObject.FindProperty ("HUDManager");
		currentVehicleWeaponSystem = serializedObject.FindProperty ("currentVehicleWeaponSystem");
		vehicleGravityManager = serializedObject.FindProperty ("vehicleGravityManager");
		mainCollider = serializedObject.FindProperty ("mainCollider");
		showSettings = serializedObject.FindProperty ("showSettings");
		IKVehiclePassengersList = serializedObject.FindProperty ("IKVehiclePassengersList");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");
		useHandleForVertex = serializedObject.FindProperty ("useHandleForVertex");
		handleRadius = serializedObject.FindProperty ("handleRadius");
		handleGizmoColor = serializedObject.FindProperty ("handleGizmoColor");
		usePositionHandle = serializedObject.FindProperty ("usePositionHandle");

		passengerGameObjectList = serializedObject.FindProperty ("passengerGameObjectList");

		showDebugPrint = serializedObject.FindProperty ("showDebugPrint");

		setPlayerCameraStateOnGetOff = serializedObject.FindProperty ("setPlayerCameraStateOnGetOff");
		setPlayerCameraStateOnFirstPersonOnGetOff = serializedObject.FindProperty ("setPlayerCameraStateOnFirstPersonOnGetOff");
		playerCameraStateOnGetOff = serializedObject.FindProperty ("playerCameraStateOnGetOff");

		setVehicleCameraStateOnGetOn = serializedObject.FindProperty ("setVehicleCameraStateOnGetOn");
		setVehicleCameraStateOnFirstPersonOnGetOn = serializedObject.FindProperty ("setVehicleCameraStateOnFirstPersonOnGetOn");
		vehicleCameraStateOnGetOn = serializedObject.FindProperty ("vehicleCameraStateOnGetOn");

		resetAnimatorDrivingStateID = serializedObject.FindProperty ("resetAnimatorDrivingStateID");

		manager = (IKDrivingSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying) {
			if (manager.showGizmo) {
				
				style.normal.textColor = manager.gizmoLabelColor;
				style.alignment = TextAnchor.MiddleCenter;

				if (manager.useExplosionForceWhenDestroyed) {
					Handles.Label (manager.gameObject.transform.position + manager.gameObject.transform.up * manager.explosionRadius, 
						"Explosion Radius " + manager.explosionRadius + "\n" + "Explosion Force " + manager.explosionForce, style);
				}

				for (int i = 0; i < manager.IKVehiclePassengersList.Count; i++) {
					if (manager.IKVehiclePassengersList [i].showIKPositionsGizmo) {
						for (int j = 0; j < manager.IKVehiclePassengersList [i].IKDrivingPos.Count; j++) {
							if (manager.IKVehiclePassengersList [i].IKDrivingPos [j].position != null) {
								Handles.Label (manager.IKVehiclePassengersList [i].IKDrivingPos [j].position.position, manager.IKVehiclePassengersList [i].IKDrivingPos [j].Name, style);	
							}
						}

						for (int j = 0; j < manager.IKVehiclePassengersList [i].IKDrivingKneePos.Count; j++) {
							if (manager.IKVehiclePassengersList [i].IKDrivingKneePos [j].position != null) {
								Handles.Label (manager.IKVehiclePassengersList [i].IKDrivingKneePos [j].position.position, manager.IKVehiclePassengersList [i].IKDrivingKneePos [j].Name, style);	
							}
						}
					}

					if (manager.IKVehiclePassengersList [i].steerDirecion != null) {
						Handles.Label (manager.IKVehiclePassengersList [i].steerDirecion.position, "Steer Position " + i, style);
					}

					if (manager.IKVehiclePassengersList [i].headLookDirection != null) {
						Handles.Label (manager.IKVehiclePassengersList [i].headLookDirection.position, "Head Look\n Direction " + i, style);
					}

					if (manager.IKVehiclePassengersList [i].headLookPosition != null) {
						Handles.Label (manager.IKVehiclePassengersList [i].headLookPosition.position, "Head Look\n Position " + i, style);
					}

					Handles.color = manager.handleGizmoColor;

					if (manager.IKVehiclePassengersList [i].showGizmo) {
						if (manager.IKVehiclePassengersList [i].vehicleSeatInfo.rightGetOffPosition) {

							Handles.Label (manager.IKVehiclePassengersList [i].vehicleSeatInfo.rightGetOffPosition.position, "Right Get \n Off Ray " + i, style);

							if (manager.useHandleForVertex) {
								showFreeMoveHandle (manager.IKVehiclePassengersList [i].vehicleSeatInfo.rightGetOffPosition, "move Right Get Off Position Handle " + i, manager.handleRadius);
							}

							if (manager.usePositionHandle) {
								showPositionHandle (manager.IKVehiclePassengersList [i].vehicleSeatInfo.rightGetOffPosition, "move Right Get Off Position Handle " + i);
							}
						}

						if (manager.IKVehiclePassengersList [i].vehicleSeatInfo.leftGetOffPosition != null) {
							Handles.Label (manager.IKVehiclePassengersList [i].vehicleSeatInfo.leftGetOffPosition.position, "Left Get \n Off Ray " + i, style);

							if (manager.useHandleForVertex) {
								showFreeMoveHandle (manager.IKVehiclePassengersList [i].vehicleSeatInfo.leftGetOffPosition, "move Left Get Off Position Handle " + i, manager.handleRadius);
							}

							if (manager.usePositionHandle) {
								showPositionHandle (manager.IKVehiclePassengersList [i].vehicleSeatInfo.leftGetOffPosition, "move Left Get Off Position Handle " + i);
							}
						}

						if (manager.IKVehiclePassengersList [i].vehicleSeatInfo.seatTransform != null) {
							Handles.Label (manager.IKVehiclePassengersList [i].vehicleSeatInfo.seatTransform.position, manager.IKVehiclePassengersList [i].Name, style);

							if (manager.useHandleForVertex) {
								showFreeMoveHandle (manager.IKVehiclePassengersList [i].vehicleSeatInfo.seatTransform, "move Seat Transform Position Handle " + i, manager.handleRadius);
							}

							if (manager.usePositionHandle) {
								showPositionHandle (manager.IKVehiclePassengersList [i].vehicleSeatInfo.seatTransform, "move Seat Transform Position Handle " + i);
							}
						}
					}
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Driver Settings", "window");
		EditorGUILayout.PropertyField (vehicle);
		EditorGUILayout.PropertyField (vehicelCameraGameObject);

		EditorGUILayout.PropertyField (useCustomVehicleGameObject);
		if (useCustomVehicleGameObject.boolValue) {
			EditorGUILayout.PropertyField (customVehicleGameObject);
		}

		EditorGUILayout.PropertyField (playerIsAlwaysDriver);
		EditorGUILayout.PropertyField (hidePlayerFromNPCs, new GUIContent ("Hide Player From NPCs"), false);
		EditorGUILayout.PropertyField (playerVisibleInVehicle);

		EditorGUILayout.PropertyField (hidePlayerWeaponsWhileDriving);

		EditorGUILayout.PropertyField (canBeDrivenRemotely);

		EditorGUILayout.PropertyField (resetAnimatorDrivingStateID);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Passengers Weapons Settings", "window");
		EditorGUILayout.PropertyField (drawFireWeaponIfCarryingPreviously);
		EditorGUILayout.PropertyField (drawMeleeWeaponIfCarryingPreviously);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Settings", "window");
		EditorGUILayout.PropertyField (resetCameraRotationWhenGetOn);
		EditorGUILayout.PropertyField (resetCameraRotationWhenGetOff);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (setPlayerCameraStateOnGetOff);
		if (setPlayerCameraStateOnGetOff.boolValue) {
			EditorGUILayout.PropertyField (setPlayerCameraStateOnFirstPersonOnGetOff);
			if (!setPlayerCameraStateOnFirstPersonOnGetOff.boolValue) {
				EditorGUILayout.PropertyField (playerCameraStateOnGetOff);
			}
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (setVehicleCameraStateOnGetOn);
		if (setVehicleCameraStateOnGetOn.boolValue) {
			EditorGUILayout.PropertyField (setVehicleCameraStateOnFirstPersonOnGetOn);
			EditorGUILayout.PropertyField (vehicleCameraStateOnGetOn);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Eject Settings", "window");
		EditorGUILayout.PropertyField (ejectPlayerWhenDestroyed);
		if (ejectPlayerWhenDestroyed.boolValue) {
			EditorGUILayout.PropertyField (ejectingPlayerForce);
		}
		EditorGUILayout.PropertyField (activateFreeFloatingModeOnEject);	
		EditorGUILayout.PropertyField (activateFreeFloatingModeOnEjectDelay);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Explosion Settings", "window");
		EditorGUILayout.PropertyField (useExplosionForceWhenDestroyed);
		if (useExplosionForceWhenDestroyed.boolValue) {
			EditorGUILayout.PropertyField (explosionRadius);
			EditorGUILayout.PropertyField (explosionForce);
			EditorGUILayout.PropertyField (explosionDamage);
			EditorGUILayout.PropertyField (ignoreShield);
			EditorGUILayout.PropertyField (damageTypeID);

			EditorGUILayout.PropertyField (useRemoteEventOnObjectsFound);
			EditorGUILayout.PropertyField (removeEventName);
	
			EditorGUILayout.PropertyField (pushCharactersOnExplosion);

			EditorGUILayout.PropertyField (applyExplosionForceToVehicles);
			if (applyExplosionForceToVehicles.boolValue) {
				EditorGUILayout.PropertyField (explosionForceToVehiclesMultiplier);
			}
			EditorGUILayout.PropertyField (killObjectsInRadius);
			EditorGUILayout.PropertyField (forceMode);
			EditorGUILayout.PropertyField (useLayerMask);
			if (useLayerMask.boolValue) {
				EditorGUILayout.PropertyField (layer);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Shake Passengers Body Settings", "window");
		EditorGUILayout.PropertyField (addCollisionForceDirectionToPassengers);
		if (addCollisionForceDirectionToPassengers.boolValue) {
			EditorGUILayout.PropertyField (extraCollisionForceAmount);
			EditorGUILayout.PropertyField (useMinCollisionForce);
			if (useMinCollisionForce.boolValue) {
				EditorGUILayout.PropertyField (minCollisionForce);
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Debug Settings", "window");
			EditorGUILayout.PropertyField (debugCollisionForce);
			if (GUILayout.Button ("Simulate Collision")) {
				manager.setCollisionForceDirectionToPassengers (debugCollisionForce.vector3Value);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		EditorGUILayout.PropertyField (addAngularDirectionToPassengers);
		if (addAngularDirectionToPassengers.boolValue) {
			EditorGUILayout.PropertyField (vehicleStabilitySpeed);
			EditorGUILayout.PropertyField (extraAngularDirectioAmount);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Start Game In Vehicle Settings", "window");
		EditorGUILayout.PropertyField (startGameInThisVehicle);
		if (startGameInThisVehicle.boolValue) {
			EditorGUILayout.PropertyField (playerForVehicle);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle State", "window");
		GUILayout.Label ("Driving remotely\t" + isBeingDrivenRemotely.boolValue);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Passengers List", "window");
		showSimpleList (passengerGameObjectList);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Action Screen Settings", "window");
		EditorGUILayout.PropertyField (activateActionScreen);
		if (activateActionScreen.boolValue) {
			EditorGUILayout.PropertyField (actionScreenName);	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (useEventOnDriverGetOn);
		if (useEventOnDriverGetOn.boolValue) {
			EditorGUILayout.PropertyField (eventOnDriverGetOn);	
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventOnDriverGetOff);
		if (useEventOnDriverGetOff.boolValue) {
			EditorGUILayout.PropertyField (eventOnDriverGetOff);	
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (sendPlayersEnterExitTriggerToEvent);
		if (sendPlayersEnterExitTriggerToEvent.boolValue) {
			EditorGUILayout.PropertyField (eventToSendPlayersEnterTriggerToEvent);	
			EditorGUILayout.PropertyField (eventToSendPlayersExitTriggerToEvent);	
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Remote Event Settings", "window");
		EditorGUILayout.PropertyField (useRemoteEventsOnPassengers);
		if (useRemoteEventsOnPassengers.boolValue) {

			EditorGUILayout.Space ();

			showSimpleList (remoteEventNameListGetOn);

			EditorGUILayout.Space ();

			showSimpleList (remoteEventNameListGetOff);
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showVehicleElements) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Vehicle Elements";
		} else {
			GUI.backgroundColor = defBackgroundColor;
			buttonMessage = "Show Vehicle Elements";
		}
		if (GUILayout.Button (buttonMessage)) {
			showVehicleElements = !showVehicleElements;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		if (showVehicleElements) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vehicle Elements", "window");
			EditorGUILayout.PropertyField (actionManager);
			EditorGUILayout.PropertyField (vehicleCameraManager);
			EditorGUILayout.PropertyField (HUDManager);
			EditorGUILayout.PropertyField (currentVehicleWeaponSystem);
			EditorGUILayout.PropertyField (vehicleGravityManager);
			EditorGUILayout.PropertyField (mainCollider);
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Passenger Seats Settings";
		} else {
			GUI.backgroundColor = defBackgroundColor;
			buttonMessage = "Show Passenger Seats Settings";
		}
		if (GUILayout.Button (buttonMessage)) {
			showSettings.boolValue = !showSettings.boolValue;
		}
		GUI.backgroundColor = defBackgroundColor;

		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (showSettings.boolValue) {
			EditorGUILayout.Space ();
			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("IK positions in vehicle", MessageType.None);
			GUI.color = Color.white;
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("IK Passengers List", "window");
			showIKVehiclePassengersList (IKVehiclePassengersList);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
		GUI.backgroundColor = defBackgroundColor;

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoLabelColor);
			EditorGUILayout.PropertyField (gizmoRadius);
			EditorGUILayout.PropertyField (useHandleForVertex);
			if (useHandleForVertex.boolValue) {
				EditorGUILayout.PropertyField (handleRadius);
				EditorGUILayout.PropertyField (handleGizmoColor);
			}
			EditorGUILayout.PropertyField (usePositionHandle);
		}
		EditorGUILayout.PropertyField (showDebugPrint);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showIKDrivingInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useIKOnVehicle"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Passenger Scale Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNewScaleOnPassenger"));
		if (list.FindPropertyRelative ("setNewScaleOnPassenger").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newScaleOnPassenger"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("IK Hint List", "window");
		showIkHintList (list.FindPropertyRelative ("IKDrivingPos"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("IK Goals List", "window");
		showIKGoalList (list.FindPropertyRelative ("IKDrivingKneePos"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showIKPositionsGizmo"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showGizmo"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Seat Info", "window");
		showSeatInfo (list.FindPropertyRelative ("vehicleSeatInfo"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePlayerBodyOnCollision"));
		if (list.FindPropertyRelative ("shakePlayerBodyOnCollision").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerBodyParent"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("stabilitySpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeFadeSpeed"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeForceDirectionMinClamp"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeForceDirectionMaxClamp"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceDirection"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (list.FindPropertyRelative ("vehicleSeatInfo.isDriverSeat").boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Body Look Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSteerDirection"));
			if (list.FindPropertyRelative ("useSteerDirection").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("steerDirecion"), new GUIContent ("Steer Direction"), false);
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHeadLookDirection"));
			if (list.FindPropertyRelative ("useHeadLookDirection").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("headLookDirection"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHeadLookPosition"));
			if (list.FindPropertyRelative ("useHeadLookPosition").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("headLookPosition"));
			}
			GUILayout.EndVertical ();
		}
	}

	void showSeatInfo (SerializedProperty list)
	{
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("seatTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("rightGetOffPosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftGetOffPosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("getOffDistance"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("getOffPlace"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isDriverSeat"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Grabbing Hands Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useGrabbingHandID"));
		if (list.FindPropertyRelative ("useGrabbingHandID").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("rightGrabbingHandID"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftGrabbingHandID"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Action System Enter/Exit Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useActionSystemToEnterExitSeat"));
		if (list.FindPropertyRelative ("useActionSystemToEnterExitSeat").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionSystemToEnterVehicle"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionSystemToExitVehicle"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionSystemToJumpOffFromVehicle"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnActionToEnter"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnActionToExit"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCancelActionEnterAndExit"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("cancelActionEnterExitVehicleIfSpeedTooHigh"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("minSpeedToCancelActionEnterExitVehicle"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remoteEventToCancelActionEnterExitInsideVehicle"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remoteEventToCancelActionEnterExitOutsideVehicle"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("minSpeedToJumpOffFromVehicle"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToStartJumpOff"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Seat State", "window");
		GUILayout.Label ("Seat Is Free\t" + list.FindPropertyRelative ("seatIsFree").boolValue);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("currentPassenger"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enterExitActionInProcess"));
		//EditorGUILayout.PropertyField (list.FindPropertyRelative ("seatIsFree"));
		GUILayout.EndVertical ();
	}

	void showUpperListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("limb"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("position"), new GUIContent ("Position Transform"), false);
		GUILayout.EndVertical ();
	}

	void showIkHintList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide IK Hint List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of IK Positions: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add IK Pos")) {
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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showUpperListElementInfo (list.GetArrayElementAtIndex (i), true);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showLowerListElementInfo (SerializedProperty list, bool showListNames)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"), new GUIContent ("Name"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("knee"), new GUIContent ("Limb"), false);
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("position"), new GUIContent ("Position Transform"), false);
		GUILayout.EndVertical ();
	}

	void showIKGoalList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide IK Goal List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of IK Goals: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add IK Pos")) {
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
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showLowerListElementInfo (list.GetArrayElementAtIndex (i), true);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showIKVehiclePassengersList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Passengers: \t" + list.arraySize);

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Passenger")) {
				manager.addPassenger ();
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
						showIKDrivingInfo (list.GetArrayElementAtIndex (i));
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

	public void showPositionHandle (Transform currentTransform, string handleName)
	{
		currentRotationHandle = Tools.pivotRotation == PivotRotation.Local ? currentTransform.rotation : Quaternion.identity;

		EditorGUI.BeginChangeCheck ();

		curretPositionHandle = currentTransform.position;

		if (Tools.current == Tool.Move) {
			curretPositionHandle = Handles.DoPositionHandle (curretPositionHandle, currentRotationHandle);
		}

		currentRotationHandle = currentTransform.rotation;

		if (Tools.current == Tool.Rotate) {
			currentRotationHandle = Handles.DoRotationHandle (currentRotationHandle, curretPositionHandle);
		}

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (currentTransform, handleName);
			currentTransform.position = curretPositionHandle;
			currentTransform.rotation = currentRotationHandle;
		}
	}

	public void showFreeMoveHandle (Transform currentTransform, string handleName, float handleRadius)
	{
		EditorGUI.BeginChangeCheck ();

		currentFreeHandlePosition = currentTransform.position;
		newFreeHandlePosition = Handles.FreeMoveHandle (currentFreeHandlePosition, Quaternion.identity, handleRadius, snapValue, Handles.CircleHandleCap);

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (currentTransform, handleName);
			currentTransform.position = newFreeHandlePosition;
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

			GUILayout.Label ("Number: \t" + list.arraySize);

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
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

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
		GUILayout.EndVertical ();
	}
}
#endif