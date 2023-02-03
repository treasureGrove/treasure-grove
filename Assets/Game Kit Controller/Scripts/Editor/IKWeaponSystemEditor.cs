using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(IKWeaponSystem))]
public class IKWeaponSystemEditor : Editor
{
	SerializedProperty weaponEnabled;
	SerializedProperty currentWeapon;
	SerializedProperty carrying;
	SerializedProperty aiming;
	SerializedProperty moving;

	SerializedProperty actionActive;

	SerializedProperty IKPausedOnHandsActive;

	SerializedProperty surfaceDetected;
	SerializedProperty weaponInRunPosition;
	SerializedProperty jumpingOnProcess;
	SerializedProperty playerOnJumpStart;
	SerializedProperty playerOnJumpEnd;
	SerializedProperty crouchingActive;
	SerializedProperty reloadingWeapon;
	SerializedProperty weaponUpdateActive;
	SerializedProperty usingDualWeapon;
	SerializedProperty disablingDualWeapon;
	SerializedProperty usingRightHandDualWeapon;
	SerializedProperty weaponConfiguredAsDualWeapon;
	SerializedProperty linkedDualWeaponName;
	SerializedProperty showSettings;
	SerializedProperty showElementSettings;
	SerializedProperty showPrefabSettings;
	SerializedProperty weaponGameObject;
	SerializedProperty extraRotation;
	SerializedProperty aimFovValue;
	SerializedProperty aimFovSpeed;
	SerializedProperty hideWeaponIfKeptInThirdPerson;
	SerializedProperty canBeDropped;
	SerializedProperty canUnlockCursor;
	SerializedProperty canUnlockCursorOnThirdPerson;
	SerializedProperty disableHUDWhenCursorUnlocked;
	SerializedProperty weaponSystemManager;
	SerializedProperty weaponsManager;
	SerializedProperty weaponTransform;
	SerializedProperty layer;
	SerializedProperty weaponUsesAttachment;
	SerializedProperty mainWeaponAttachmentSystem;
	SerializedProperty showThirdPersonSettings;
	SerializedProperty thirdPersonWeaponInfo;
	SerializedProperty thirdPersonUseSwayInfo;
	SerializedProperty showSwaySettings;
	SerializedProperty thirdPersonUseRunPosition;
	SerializedProperty thirdPersonUseRunSwayInfo;
	SerializedProperty showWalkSwaySettings;
	SerializedProperty thirdPersonSwayInfo;
	SerializedProperty showRunSwaySettings;
	SerializedProperty runThirdPersonSwayInfo;
	SerializedProperty showFirstPersonSettings;
	SerializedProperty firstPersonWeaponInfo;
	SerializedProperty firstPersonUseSwayInfo;
	SerializedProperty firstPersonUseRunPosition;
	SerializedProperty firstPersonUseRunSwayInfo;
	SerializedProperty firstPersonSwayInfo;
	SerializedProperty runFirstPersonSwayInfo;
	SerializedProperty showIdleSettings;
	SerializedProperty useWeaponIdle;
	SerializedProperty timeToActiveWeaponIdle;
	SerializedProperty idlePositionAmount;
	SerializedProperty idleRotationAmount;
	SerializedProperty idleSpeed;
	SerializedProperty playerMoving;
	SerializedProperty idleActive;
	SerializedProperty showShotShakeSettings;
	SerializedProperty sameValueBothViews;
	SerializedProperty thirdPersonshotShakeInfo;
	SerializedProperty useShotShakeInThirdPerson;
	SerializedProperty useShotShakeInFirstPerson;
	SerializedProperty firstPersonshotShakeInfo;
	SerializedProperty useShotCameraNoise;
	SerializedProperty showThirdPersonGizmo;
	SerializedProperty showFirstPersonGizmo;
	SerializedProperty gizmoLabelColor;
	SerializedProperty showHandsWaypointGizmo;
	SerializedProperty showWeaponWaypointGizmo;
	SerializedProperty showPositionGizmo;
	SerializedProperty thirdPersonUseHandle;
	SerializedProperty thirdPersonHandleRadius;
	SerializedProperty thirdPersonHandleGizmoColor;
	SerializedProperty thirdPersonUsePositionHandle;
	SerializedProperty firstPersonUseHandle;
	SerializedProperty firstPersonHandleRadius;
	SerializedProperty firstPersonHandleGizmoColor;
	SerializedProperty firstPersonUsePositionHandle;
	SerializedProperty relativePathWeaponsPickups;
	SerializedProperty emtpyWeaponPrefab;
	SerializedProperty weaponPrefabModel;
	SerializedProperty inventoryWeaponPrefabObject;
	SerializedProperty relativePathWeaponsMesh;
	SerializedProperty relativePathWeaponsAmmoMesh;
	SerializedProperty inventoryWeaponCategoryName;
	SerializedProperty weaponName;
	SerializedProperty inventoryAmmoCategoryName;
	SerializedProperty ammoAmountPerPickup;
	SerializedProperty weaponAmmoMesh;
	SerializedProperty weaponAmmoIconTexture;
	SerializedProperty useLowerRotationSpeedAimedThirdPerson;
	SerializedProperty verticalRotationSpeedAimedInThirdPerson;
	SerializedProperty horizontalRotationSpeedAimedInThirdPerson;
	SerializedProperty canAimInFirstPerson;
	SerializedProperty useLowerRotationSpeedAimed;
	SerializedProperty verticalRotationSpeedAimedInFirstPerson;
	SerializedProperty horizontalRotationSpeedAimedInFirstPerson;

	SerializedProperty firstPersonArms;

	SerializedProperty disableOnlyFirstPersonArmsMesh;

	SerializedProperty firstPersonArmsMesh;

	SerializedProperty extraFirstPersonArmsMeshes;

	SerializedProperty useWeaponAnimatorFirstPerson;
	SerializedProperty mainWeaponAnimatorSystem;


	SerializedProperty thirdPersonDeactivateIKDrawPath;
	SerializedProperty thirdPersonMeleeAttackShakeInfo;
	SerializedProperty firstPersonMeleeAttackShakeInfo;
	SerializedProperty thirdPersonKeepPath;
	SerializedProperty thirdPersonHandsInfo;
	SerializedProperty headLookWhenAiming;
	SerializedProperty headLookSpeed;
	SerializedProperty headLookTarget;

	SerializedProperty rightHandMountPoint;
	SerializedProperty leftHandMountPoint;

	IKWeaponSystem IKWeaponManager;

	GUIStyle style = new GUIStyle ();

	GUIStyle gizmoStyle = new GUIStyle ();

	bool showElbowInfo;

	bool showSingleWeaponSettings;
	bool showDualWeaponSettings;

	Color buttonColor;
	Vector3 oldPoint;
	Vector3 newPoint;

	Color listButtonBackgroundColor;

	string inputListOpenedText;

	bool expanded;

	Vector3 curretPositionHandle;
	Quaternion currentRotationHandle;

	Vector3 currentFreeHandlePosition;
	Vector3 newFreeHandlePosition;

	Vector3 snapValue = new Vector3 (.25f, .25f, .25f);

	float thirdPersonHandleRadiusValue;
	float firstPersonHandleRadiusValue;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		weaponEnabled = serializedObject.FindProperty ("weaponEnabled");
		currentWeapon = serializedObject.FindProperty ("currentWeapon");
		carrying = serializedObject.FindProperty ("carrying");
		aiming = serializedObject.FindProperty ("aiming");

		actionActive= serializedObject.FindProperty ("actionActive");

		moving = serializedObject.FindProperty ("moving");

		IKPausedOnHandsActive = serializedObject.FindProperty ("IKPausedOnHandsActive");

		surfaceDetected = serializedObject.FindProperty ("surfaceDetected");
		weaponInRunPosition = serializedObject.FindProperty ("weaponInRunPosition");
		jumpingOnProcess = serializedObject.FindProperty ("jumpingOnProcess");
		playerOnJumpStart = serializedObject.FindProperty ("playerOnJumpStart");
		playerOnJumpEnd = serializedObject.FindProperty ("playerOnJumpEnd");
		crouchingActive = serializedObject.FindProperty ("crouchingActive");
		reloadingWeapon = serializedObject.FindProperty ("reloadingWeapon");
		weaponUpdateActive = serializedObject.FindProperty ("weaponUpdateActive");
		usingDualWeapon = serializedObject.FindProperty ("usingDualWeapon");
		disablingDualWeapon = serializedObject.FindProperty ("disablingDualWeapon");
		usingRightHandDualWeapon = serializedObject.FindProperty ("usingRightHandDualWeapon");
		weaponConfiguredAsDualWeapon = serializedObject.FindProperty ("weaponConfiguredAsDualWeapon");
		linkedDualWeaponName = serializedObject.FindProperty ("linkedDualWeaponName");
		showSettings = serializedObject.FindProperty ("showSettings");
		showElementSettings = serializedObject.FindProperty ("showElementSettings");
		showPrefabSettings = serializedObject.FindProperty ("showPrefabSettings");
		weaponGameObject = serializedObject.FindProperty ("weaponGameObject");
		extraRotation = serializedObject.FindProperty ("extraRotation");
		aimFovValue = serializedObject.FindProperty ("aimFovValue");
		aimFovSpeed = serializedObject.FindProperty ("aimFovSpeed");
		hideWeaponIfKeptInThirdPerson = serializedObject.FindProperty ("hideWeaponIfKeptInThirdPerson");
		canBeDropped = serializedObject.FindProperty ("canBeDropped");
		canUnlockCursor = serializedObject.FindProperty ("canUnlockCursor");
		canUnlockCursorOnThirdPerson = serializedObject.FindProperty ("canUnlockCursorOnThirdPerson");
		disableHUDWhenCursorUnlocked = serializedObject.FindProperty ("disableHUDWhenCursorUnlocked");
		weaponSystemManager = serializedObject.FindProperty ("weaponSystemManager");
		weaponsManager = serializedObject.FindProperty ("weaponsManager");
		weaponTransform = serializedObject.FindProperty ("weaponTransform");
		layer = serializedObject.FindProperty ("layer");
		weaponUsesAttachment = serializedObject.FindProperty ("weaponUsesAttachment");
		mainWeaponAttachmentSystem = serializedObject.FindProperty ("mainWeaponAttachmentSystem");
		showThirdPersonSettings = serializedObject.FindProperty ("showThirdPersonSettings");
		thirdPersonWeaponInfo = serializedObject.FindProperty ("thirdPersonWeaponInfo");
		thirdPersonUseSwayInfo = serializedObject.FindProperty ("thirdPersonWeaponInfo.useSwayInfo");
		showSwaySettings = serializedObject.FindProperty ("showSwaySettings");
		thirdPersonUseRunPosition = serializedObject.FindProperty ("thirdPersonWeaponInfo.useRunPosition");
		thirdPersonUseRunSwayInfo = serializedObject.FindProperty ("thirdPersonWeaponInfo.useRunSwayInfo");
		showWalkSwaySettings = serializedObject.FindProperty ("showWalkSwaySettings");
		thirdPersonSwayInfo = serializedObject.FindProperty ("thirdPersonSwayInfo");
		showRunSwaySettings = serializedObject.FindProperty ("showRunSwaySettings");
		runThirdPersonSwayInfo = serializedObject.FindProperty ("runThirdPersonSwayInfo");
		showFirstPersonSettings = serializedObject.FindProperty ("showFirstPersonSettings");
		firstPersonWeaponInfo = serializedObject.FindProperty ("firstPersonWeaponInfo");
		firstPersonUseSwayInfo = serializedObject.FindProperty ("firstPersonWeaponInfo.useSwayInfo");
		firstPersonUseRunPosition = serializedObject.FindProperty ("firstPersonWeaponInfo.useRunPosition");
		firstPersonUseRunSwayInfo = serializedObject.FindProperty ("firstPersonWeaponInfo.useRunSwayInfo");
		firstPersonSwayInfo = serializedObject.FindProperty ("firstPersonSwayInfo");
		runFirstPersonSwayInfo = serializedObject.FindProperty ("runFirstPersonSwayInfo");
		showIdleSettings = serializedObject.FindProperty ("showIdleSettings");
		useWeaponIdle = serializedObject.FindProperty ("useWeaponIdle");
		timeToActiveWeaponIdle = serializedObject.FindProperty ("timeToActiveWeaponIdle");
		idlePositionAmount = serializedObject.FindProperty ("idlePositionAmount");
		idleRotationAmount = serializedObject.FindProperty ("idleRotationAmount");
		idleSpeed = serializedObject.FindProperty ("idleSpeed");
		playerMoving = serializedObject.FindProperty ("playerMoving");
		idleActive = serializedObject.FindProperty ("idleActive");
		showShotShakeSettings = serializedObject.FindProperty ("showShotShakeSettings");
		sameValueBothViews = serializedObject.FindProperty ("sameValueBothViews");
		thirdPersonshotShakeInfo = serializedObject.FindProperty ("thirdPersonshotShakeInfo");
		useShotShakeInThirdPerson = serializedObject.FindProperty ("useShotShakeInThirdPerson");
		useShotShakeInFirstPerson = serializedObject.FindProperty ("useShotShakeInFirstPerson");
		firstPersonshotShakeInfo = serializedObject.FindProperty ("firstPersonshotShakeInfo");
		useShotCameraNoise = serializedObject.FindProperty ("useShotCameraNoise");
		showThirdPersonGizmo = serializedObject.FindProperty ("showThirdPersonGizmo");
		showFirstPersonGizmo = serializedObject.FindProperty ("showFirstPersonGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		showHandsWaypointGizmo = serializedObject.FindProperty ("showHandsWaypointGizmo");
		showWeaponWaypointGizmo = serializedObject.FindProperty ("showWeaponWaypointGizmo");
		showPositionGizmo = serializedObject.FindProperty ("showPositionGizmo");
		thirdPersonUseHandle = serializedObject.FindProperty ("thirdPersonWeaponInfo.useHandle");
		thirdPersonHandleRadius = serializedObject.FindProperty ("thirdPersonWeaponInfo.handleRadius");
		thirdPersonHandleGizmoColor = serializedObject.FindProperty ("thirdPersonWeaponInfo.handleGizmoColor");
		thirdPersonUsePositionHandle = serializedObject.FindProperty ("thirdPersonWeaponInfo.usePositionHandle");
		firstPersonUseHandle = serializedObject.FindProperty ("firstPersonWeaponInfo.useHandle");
		firstPersonHandleRadius = serializedObject.FindProperty ("firstPersonWeaponInfo.handleRadius");
		firstPersonHandleGizmoColor = serializedObject.FindProperty ("firstPersonWeaponInfo.handleGizmoColor");
		firstPersonUsePositionHandle = serializedObject.FindProperty ("firstPersonWeaponInfo.usePositionHandle");
		relativePathWeaponsPickups = serializedObject.FindProperty ("relativePathWeaponsPickups");
		emtpyWeaponPrefab = serializedObject.FindProperty ("emtpyWeaponPrefab");
		weaponPrefabModel = serializedObject.FindProperty ("weaponPrefabModel");
		inventoryWeaponPrefabObject = serializedObject.FindProperty ("inventoryWeaponPrefabObject");
		relativePathWeaponsMesh = serializedObject.FindProperty ("relativePathWeaponsMesh");
		relativePathWeaponsAmmoMesh = serializedObject.FindProperty ("relativePathWeaponsAmmoMesh");
		inventoryWeaponCategoryName = serializedObject.FindProperty ("inventoryWeaponCategoryName");
		weaponName = serializedObject.FindProperty ("weaponName");
		inventoryAmmoCategoryName = serializedObject.FindProperty ("inventoryAmmoCategoryName");
		ammoAmountPerPickup = serializedObject.FindProperty ("ammoAmountPerPickup");
		weaponAmmoMesh = serializedObject.FindProperty ("weaponAmmoMesh");
		weaponAmmoIconTexture = serializedObject.FindProperty ("weaponAmmoIconTexture");
		useLowerRotationSpeedAimedThirdPerson = serializedObject.FindProperty ("useLowerRotationSpeedAimedThirdPerson");
		verticalRotationSpeedAimedInThirdPerson = serializedObject.FindProperty ("verticalRotationSpeedAimedInThirdPerson");
		horizontalRotationSpeedAimedInThirdPerson = serializedObject.FindProperty ("horizontalRotationSpeedAimedInThirdPerson");
		canAimInFirstPerson = serializedObject.FindProperty ("canAimInFirstPerson");
		useLowerRotationSpeedAimed = serializedObject.FindProperty ("useLowerRotationSpeedAimed");
		verticalRotationSpeedAimedInFirstPerson = serializedObject.FindProperty ("verticalRotationSpeedAimedInFirstPerson");
		horizontalRotationSpeedAimedInFirstPerson = serializedObject.FindProperty ("horizontalRotationSpeedAimedInFirstPerson");

		firstPersonArms = serializedObject.FindProperty ("firstPersonArms");

		disableOnlyFirstPersonArmsMesh = serializedObject.FindProperty ("disableOnlyFirstPersonArmsMesh");

		firstPersonArmsMesh = serializedObject.FindProperty ("firstPersonArmsMesh");

		extraFirstPersonArmsMeshes = serializedObject.FindProperty ("extraFirstPersonArmsMeshes");

		thirdPersonDeactivateIKDrawPath = serializedObject.FindProperty ("thirdPersonWeaponInfo.deactivateIKDrawPath");
		thirdPersonMeleeAttackShakeInfo = serializedObject.FindProperty ("thirdPersonMeleeAttackShakeInfo");
		firstPersonMeleeAttackShakeInfo = serializedObject.FindProperty ("firstPersonMeleeAttackShakeInfo");
		thirdPersonKeepPath = serializedObject.FindProperty ("thirdPersonWeaponInfo.keepPath");
		thirdPersonHandsInfo = serializedObject.FindProperty ("thirdPersonWeaponInfo.handsInfo");
		headLookWhenAiming = serializedObject.FindProperty ("headLookWhenAiming");
		headLookSpeed = serializedObject.FindProperty ("headLookSpeed");
		headLookTarget = serializedObject.FindProperty ("headLookTarget");

		useWeaponAnimatorFirstPerson = serializedObject.FindProperty ("useWeaponAnimatorFirstPerson");
		mainWeaponAnimatorSystem = serializedObject.FindProperty ("mainWeaponAnimatorSystem");

		rightHandMountPoint = serializedObject.FindProperty ("rightHandMountPoint");
		leftHandMountPoint = serializedObject.FindProperty ("leftHandMountPoint");

		IKWeaponManager = (IKWeaponSystem)target;
	}

	void OnSceneGUI ()
	{
		if (!Application.isPlaying) {
			if (IKWeaponManager.showThirdPersonGizmo) {
				gizmoStyle.normal.textColor = IKWeaponManager.gizmoLabelColor;
				gizmoStyle.alignment = TextAnchor.MiddleCenter;

				gizmoStyle.fontStyle = FontStyle.Normal;
				gizmoStyle.fontSize = 10;

				if (IKWeaponManager.showPositionGizmo) {
					Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.aimPosition.position, "Aim \n Position", gizmoStyle);
					Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.walkPosition.position, "Walk \n Position", gizmoStyle);
					Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.keepPosition.position, "Keep \n Position", gizmoStyle);
					Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.aimRecoilPosition.position, "Aim \n Recoil \n Position", gizmoStyle);

					if (IKWeaponManager.thirdPersonWeaponInfo.checkSurfaceCollision) {
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionPosition.position, "Surface \n Collision \n Position", gizmoStyle);
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionRayPosition.position, "Surface \n Collision \n Ray \n Position", gizmoStyle);
					}

					if (IKWeaponManager.thirdPersonWeaponInfo.useRunPosition && IKWeaponManager.thirdPersonWeaponInfo.runPosition) {
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.runPosition.position, "Run \n Position", gizmoStyle);
					}

					if (IKWeaponManager.thirdPersonWeaponInfo.hasAttachments) {
						if (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition) {
							Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition.position, "Edit \n Attachment \n Position", gizmoStyle);
						}
					}

					if (IKWeaponManager.thirdPersonWeaponInfo.checkSurfaceCollision) {
						gizmoStyle.normal.textColor = IKWeaponManager.gizmoLabelColor;
						gizmoStyle.alignment = TextAnchor.MiddleCenter;

						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionRayPosition.position +
						IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionRayPosition.forward * IKWeaponManager.thirdPersonWeaponInfo.collisionRayDistance,
							"Collision \n Ray \n Distance", gizmoStyle);
					}
				}

				thirdPersonHandleRadiusValue = IKWeaponManager.thirdPersonWeaponInfo.handleRadius;

				if (IKWeaponManager.showHandsWaypointGizmo) {
					for (int i = 0; i < IKWeaponManager.thirdPersonWeaponInfo.handsInfo.Count; i++) {
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].position.position, IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].Name, gizmoStyle);

						if (IKWeaponManager.thirdPersonWeaponInfo.useHandle) {
							for (int j = 0; j < IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].wayPoints.Count; j++) {

								Handles.color = IKWeaponManager.thirdPersonWeaponInfo.handleGizmoColor;

								showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].wayPoints [j],
									"move Third IKWeapon Waypoint Info Handle" + i, thirdPersonHandleRadiusValue);
							}
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.usePositionHandle) {
							for (int j = 0; j < IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].wayPoints.Count; j++) {

								showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.handsInfo [i].wayPoints [j], "move handle waypoint" + i);
							}
						}
					}
				}

				if (IKWeaponManager.showWeaponWaypointGizmo) {
					for (int i = 0; i < IKWeaponManager.thirdPersonWeaponInfo.keepPath.Count; i++) {
						Handles.Label (IKWeaponManager.thirdPersonWeaponInfo.keepPath [i].position, (1 + i).ToString (), gizmoStyle);

						if (IKWeaponManager.thirdPersonWeaponInfo.useHandle) {
							for (int j = 0; j < IKWeaponManager.thirdPersonWeaponInfo.keepPath.Count; j++) {

								Handles.color = IKWeaponManager.thirdPersonWeaponInfo.handleGizmoColor;

								showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.keepPath [i], "move Third IKWeapon Weapon Waypoint Info Handle" + i, thirdPersonHandleRadiusValue);
							}
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.usePositionHandle) {
							showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.keepPath [i], "move weapon handle waypoint" + i);
						}
					}
				}

				if (IKWeaponManager.showPositionGizmo) {
					if (IKWeaponManager.thirdPersonWeaponInfo.useHandle) {
						showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.aimPosition, "move Third Aim Position Info Handle", thirdPersonHandleRadiusValue);

						showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.walkPosition, "move Third Walk Position Info Handle", thirdPersonHandleRadiusValue);
						showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.keepPosition, "move Third Keep Position Info Handle", thirdPersonHandleRadiusValue);
						showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.aimRecoilPosition, "move Third Aim Recoil Position Info Handle", thirdPersonHandleRadiusValue);


						if (IKWeaponManager.thirdPersonWeaponInfo.checkSurfaceCollision) {
							showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionPosition, "move Third Aim Surface Collision Position Info Handle", thirdPersonHandleRadiusValue);

							showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionRayPosition, "move Third Aim Surface Collision Ray Position Info Handle", thirdPersonHandleRadiusValue);
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.hasAttachments) {
							if (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition) {
								showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition, "move Third Edit Attachment Position Info Handle", thirdPersonHandleRadiusValue);
							}
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.useRunPosition && IKWeaponManager.thirdPersonWeaponInfo.runPosition) {
							showFreeMoveHandle (IKWeaponManager.thirdPersonWeaponInfo.runPosition, "move Third Run Position Info Handle", thirdPersonHandleRadiusValue);
						}
					}

					if (IKWeaponManager.thirdPersonWeaponInfo.usePositionHandle) {
						showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.aimPosition, "move Third Aim Position Info Handle");

						showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.walkPosition, "move Third Walk Position Info Handle");
						showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.keepPosition, "move Third Keep Position Info Handle");
						showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.aimRecoilPosition, "move Third Aim Recoil Position Info Handle");


						if (IKWeaponManager.thirdPersonWeaponInfo.checkSurfaceCollision) {
							showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionPosition, "move Third Aim Surface Collision Position Info Handle");

							showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.surfaceCollisionRayPosition, "move Third Aim Surface Collision Ray Position Info Handle");
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.hasAttachments) {
							if (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition) {
								showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.editAttachmentPosition, "move Third Edit Attachment Position Info Handle");
							}
						}

						if (IKWeaponManager.thirdPersonWeaponInfo.useRunPosition && IKWeaponManager.thirdPersonWeaponInfo.runPosition) {
							showPositionHandle (IKWeaponManager.thirdPersonWeaponInfo.runPosition, "move Third Run Position Info Handle");
						}
					}
				}
			}

			if (IKWeaponManager.showFirstPersonGizmo) {
				gizmoStyle.normal.textColor = IKWeaponManager.gizmoLabelColor;
				gizmoStyle.alignment = TextAnchor.MiddleCenter;

				if (IKWeaponManager.showPositionGizmo) {
					Handles.Label (IKWeaponManager.firstPersonWeaponInfo.aimPosition.position, "Aim \n Position", gizmoStyle);
					Handles.Label (IKWeaponManager.firstPersonWeaponInfo.walkPosition.position, "Walk \n Position", gizmoStyle);
					Handles.Label (IKWeaponManager.firstPersonWeaponInfo.keepPosition.position, "Keep \n Position", gizmoStyle);
					Handles.Label (IKWeaponManager.firstPersonWeaponInfo.aimRecoilPosition.position, "Aim \n Recoil \n Position", gizmoStyle);
					Handles.Label (IKWeaponManager.firstPersonWeaponInfo.walkRecoilPosition.position, "Walk \n Recoil \n Position", gizmoStyle);

					if (IKWeaponManager.firstPersonWeaponInfo.checkSurfaceCollision) {
						Handles.Label (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionPosition.position, "Surface \n Collision \n Position", gizmoStyle);
						Handles.Label (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionRayPosition.position, "Surface \n Collision \n Ray \n Position", gizmoStyle);
					}

					if (IKWeaponManager.firstPersonWeaponInfo.useRunPosition) {
						Handles.Label (IKWeaponManager.firstPersonWeaponInfo.runPosition.position, "Run \n Position", gizmoStyle);
					}

					if (IKWeaponManager.firstPersonWeaponInfo.hasAttachments) {
						if (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition) {
							Handles.Label (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition.position, "Edit \n Attachment \n Position", gizmoStyle);
						}

						if (IKWeaponManager.firstPersonWeaponInfo.useSightPosition) {
							if (IKWeaponManager.firstPersonWeaponInfo.sightPosition) {
								Handles.Label (IKWeaponManager.firstPersonWeaponInfo.sightPosition.position, "Sight \n Position", gizmoStyle);
							}

							if (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition) {
								Handles.Label (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition.position, "Sight \n Recoil \n Position", gizmoStyle);
							}
						}
					}

					if (IKWeaponManager.firstPersonWeaponInfo.checkSurfaceCollision) {
						gizmoStyle.normal.textColor = IKWeaponManager.gizmoLabelColor;
						gizmoStyle.alignment = TextAnchor.MiddleCenter;

						Handles.Label (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionRayPosition.position +
						IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionRayPosition.forward * IKWeaponManager.firstPersonWeaponInfo.collisionRayDistance,
							"Collision \n Ray \n Distance", gizmoStyle);
					}
				}

				firstPersonHandleRadiusValue = IKWeaponManager.firstPersonWeaponInfo.handleRadius;

				if (IKWeaponManager.showPositionGizmo) {
					if (IKWeaponManager.firstPersonWeaponInfo.useHandle) {
						Handles.color = IKWeaponManager.firstPersonWeaponInfo.handleGizmoColor;

						showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.aimPosition, "move First Aim Position Info Handle", firstPersonHandleRadiusValue);
						showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.walkPosition, "move First Walk Position Info Handle", firstPersonHandleRadiusValue);
						showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.keepPosition, "move First Keep Position Info Handle", firstPersonHandleRadiusValue);
						showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.aimRecoilPosition, "move First Aim Recoil Position Info Handle", firstPersonHandleRadiusValue);

						showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.walkRecoilPosition, "move First Walk Recoil Position Info Handle", firstPersonHandleRadiusValue);


						if (IKWeaponManager.firstPersonWeaponInfo.checkSurfaceCollision) {
							showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionPosition, "move First Aim Surface Collision Position Info Handle", firstPersonHandleRadiusValue);
							showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionRayPosition, "move First Aim Surface Collision Ray Position Info Handle", firstPersonHandleRadiusValue);
						}

						if (IKWeaponManager.firstPersonWeaponInfo.hasAttachments) {
							if (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition) {
								showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition, "move First Edit Attachment Position Info Handle", firstPersonHandleRadiusValue);
							}

							if (IKWeaponManager.firstPersonWeaponInfo.useSightPosition) {
								if (IKWeaponManager.firstPersonWeaponInfo.sightPosition) {
									showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.sightPosition, "move First Sight Position Info Handle", firstPersonHandleRadiusValue);
								}

								if (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition) {
									showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition, "move First Sight Recoil Position Info Handle", firstPersonHandleRadiusValue);
								}
							}

							if (IKWeaponManager.firstPersonWeaponInfo.useRunPosition && IKWeaponManager.firstPersonWeaponInfo.runPosition) {
								showFreeMoveHandle (IKWeaponManager.firstPersonWeaponInfo.runPosition, "move First Run Position Info Handle", firstPersonHandleRadiusValue);
							}
						}
					}

					if (IKWeaponManager.firstPersonWeaponInfo.usePositionHandle) {
						Handles.color = IKWeaponManager.firstPersonWeaponInfo.handleGizmoColor;

						showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.aimPosition, "move First Aim Position Info Handle");
						showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.walkPosition, "move First Walk Position Info Handle");
						showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.keepPosition, "move First Keep Position Info Handle");
						showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.aimRecoilPosition, "move First Aim Recoil Position Info Handle");

						showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.walkRecoilPosition, "move First Walk Recoil Position Info Handle");


						if (IKWeaponManager.firstPersonWeaponInfo.checkSurfaceCollision) {
							showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionPosition, "move First Aim Surface Collision Position Info Handle");
							showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.surfaceCollisionRayPosition, "move First Aim Surface Collision Ray Position Info Handle");
						}

						if (IKWeaponManager.firstPersonWeaponInfo.hasAttachments) {
							if (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition) {
								showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.editAttachmentPosition, "move First Edit Attachment Position Info Handle");
							}

							if (IKWeaponManager.firstPersonWeaponInfo.useSightPosition) {
								if (IKWeaponManager.firstPersonWeaponInfo.sightPosition) {
									showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.sightPosition, "move First Sight Position Info Handle");
								}

								if (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition) {
									showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.sightRecoilPosition, "move First Sight Recoil Position Info Handle");
								}
							}

							if (IKWeaponManager.firstPersonWeaponInfo.useRunPosition && IKWeaponManager.firstPersonWeaponInfo.runPosition) {
								showPositionHandle (IKWeaponManager.firstPersonWeaponInfo.runPosition, "move First Run Position Info Handle");
							}
						}
					}
				}
			}
		}
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

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Weapon State", "window", GUILayout.Height (30));

		GUILayout.Label ("Weapon Enabled\t\t" + weaponEnabled.boolValue);

		GUILayout.Label ("Is Current Weapon\t\t" + currentWeapon.boolValue);

		GUILayout.Label ("Carrying Weapon\t\t" + carrying.boolValue);

		GUILayout.Label ("Aiming Weapon\t\t" + aiming.boolValue);

		GUILayout.Label ("Moving Weapon\t\t" + moving.boolValue);
		GUILayout.Label ("IK Paused\t\t\t" + IKPausedOnHandsActive.boolValue);

		GUILayout.Label ("Surface Detected\t\t" + surfaceDetected.boolValue);

		GUILayout.Label ("In Run Position\t\t" + weaponInRunPosition.boolValue);

		GUILayout.Label ("Jump In Process\t\t" + jumpingOnProcess.boolValue);
		GUILayout.Label ("Player Jump Start\t\t" + playerOnJumpStart.boolValue);
		GUILayout.Label ("Player Jump End\t\t" + playerOnJumpEnd.boolValue);
		GUILayout.Label ("Crouch Active\t\t" + crouchingActive.boolValue);
		GUILayout.Label ("Reloading\t\t\t" + reloadingWeapon.boolValue);
		GUILayout.Label ("Action Active\t\t" + actionActive.boolValue);
		GUILayout.Label ("Update Active\t\t" + weaponUpdateActive.boolValue);

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dual Weapon State", "window");
		GUILayout.Label ("Using Dual Weapons\t\t" + usingDualWeapon.boolValue);
		GUILayout.Label ("Disabling Dual State\t\t" + disablingDualWeapon.boolValue);
		GUILayout.Label ("Right Dual Weapon\t\t" + usingRightHandDualWeapon.boolValue);
		GUILayout.Label ("Configured as Dual\t\t" + weaponConfiguredAsDualWeapon.boolValue);
		GUILayout.Label ("Linked Weapon\t\t" + linkedDualWeaponName.stringValue);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;

		EditorGUILayout.BeginVertical ();
		if (showSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Main Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Main Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showSettings.boolValue = !showSettings.boolValue;
		}

		if (showElementSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Element Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Element Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showElementSettings.boolValue = !showElementSettings.boolValue;
		}

		if (showPrefabSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Prefab Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Prefab Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			showPrefabSettings.boolValue = !showPrefabSettings.boolValue;
		}

		if (GUILayout.Button ("Hide All Settings")) {
			showSettings.boolValue = false;
			showElementSettings.boolValue = false;
			showPrefabSettings.boolValue = false;
			showThirdPersonSettings.boolValue = false;

			showSwaySettings.boolValue = false;
			showWalkSwaySettings.boolValue = false;
			showRunSwaySettings.boolValue = false;
			showFirstPersonSettings.boolValue = false;
			showSwaySettings.boolValue = false;
			showWalkSwaySettings.boolValue = false;
			showRunSwaySettings.boolValue = false;

			showIdleSettings.boolValue = false;
			showShotShakeSettings.boolValue = false;
			showSingleWeaponSettings = false;
			showDualWeaponSettings = false;
			showElbowInfo = false;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();

		if (showSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("MAIN SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window");
			EditorGUILayout.PropertyField (weaponGameObject);
			EditorGUILayout.PropertyField (extraRotation);
			EditorGUILayout.PropertyField (aimFovValue);
			EditorGUILayout.PropertyField (aimFovSpeed);
			EditorGUILayout.PropertyField (weaponEnabled);
			EditorGUILayout.PropertyField (hideWeaponIfKeptInThirdPerson);
			EditorGUILayout.PropertyField (canBeDropped);

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Unlock Cursor Settings", "window");
			EditorGUILayout.PropertyField (canUnlockCursor, new GUIContent ("Can Unlock Cursor On First Person"));
			EditorGUILayout.PropertyField (canUnlockCursorOnThirdPerson);
			EditorGUILayout.PropertyField (disableHUDWhenCursorUnlocked);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Components", "window");
			EditorGUILayout.PropertyField (weaponSystemManager);
			EditorGUILayout.PropertyField (weaponsManager);
			EditorGUILayout.PropertyField (weaponTransform);

			EditorGUILayout.PropertyField (rightHandMountPoint);
			EditorGUILayout.PropertyField (leftHandMountPoint);
		
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("First Person Animator Settings", "window");
			EditorGUILayout.PropertyField (useWeaponAnimatorFirstPerson);
			if (useWeaponAnimatorFirstPerson.boolValue) {
				EditorGUILayout.PropertyField (mainWeaponAnimatorSystem);

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Select Weapon Animator On Editor")) {
					IKWeaponManager.selectWeaponAnimator ();
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Surface Collision Settings", "window");
			EditorGUILayout.PropertyField (layer);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Attachment Components", "window");
			GUILayout.Label ("Attachment Assigned\t " + weaponUsesAttachment.boolValue);
			EditorGUILayout.PropertyField (mainWeaponAttachmentSystem);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Check Attachments Assigned")) {
				IKWeaponManager.checkIfWeaponAttachmentAssigned ();
			}

			EditorGUILayout.Space (); 

			if (GUILayout.Button ("Select Attachment System On Editor")) {
				IKWeaponManager.selectAttachmentSystemOnEditor ();
			}

			GUILayout.EndVertical ();
		}

		if (showSettings.boolValue || showElementSettings.boolValue || showPrefabSettings.boolValue) {
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 30;
			style.alignment = TextAnchor.MiddleCenter;
			style.normal.textColor = Color.black;
		}

		if (showElementSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("WEAPON SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure the settings for third and first person", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			buttonColor = GUI.backgroundColor;
			EditorGUILayout.BeginVertical ();
			if (showThirdPersonSettings.boolValue) {
				GUI.backgroundColor = Color.gray;
				inputListOpenedText = "\nHide Third Person Settings\n";
			} else {
				GUI.backgroundColor = buttonColor;
				inputListOpenedText = "\nShow Third Person Settings\n";
			}
			if (GUILayout.Button (inputListOpenedText)) {
				showThirdPersonSettings.boolValue = !showThirdPersonSettings.boolValue;
			}
			GUI.backgroundColor = buttonColor;
			EditorGUILayout.EndVertical ();

			if (showThirdPersonSettings.boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Third Person Weapon Settings", "window");
				showWeaponSettings (thirdPersonWeaponInfo, true);

				EditorGUILayout.Space ();

				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (thirdPersonUseSwayInfo);

				if (thirdPersonUseSwayInfo.boolValue) {
					EditorGUILayout.BeginVertical ();
					if (showSwaySettings.boolValue) {
						GUI.backgroundColor = Color.gray;
						inputListOpenedText = "Hide Third Person Sway Settings";
					} else {
						GUI.backgroundColor = buttonColor;
						inputListOpenedText = "Show Third Person Sway Settings";
					}
					if (GUILayout.Button (inputListOpenedText)) {
						showSwaySettings.boolValue = !showSwaySettings.boolValue;
					}
					GUI.backgroundColor = buttonColor;
					EditorGUILayout.EndVertical ();

					if (showSwaySettings.boolValue) {
						if (thirdPersonUseRunPosition.boolValue && thirdPersonUseRunSwayInfo.boolValue) {
							GUILayout.BeginVertical ("box");

							EditorGUILayout.BeginVertical ();
							if (showWalkSwaySettings.boolValue) {
								GUI.backgroundColor = Color.gray;
								inputListOpenedText = "Hide Walk Sway Settings";
							} else {
								GUI.backgroundColor = buttonColor;
								inputListOpenedText = "Show Walk Sway Settings";
							}
							if (GUILayout.Button (inputListOpenedText)) {
								showWalkSwaySettings.boolValue = !showWalkSwaySettings.boolValue;
							}
							GUI.backgroundColor = buttonColor;
							EditorGUILayout.EndVertical ();

							if (showWalkSwaySettings.boolValue) {
								showSelectedSwaySettings (thirdPersonSwayInfo);
							}

							EditorGUILayout.Space ();

							EditorGUILayout.BeginVertical ();
							if (showRunSwaySettings.boolValue) {
								GUI.backgroundColor = Color.gray;
								inputListOpenedText = "Hide Run Sway Settings";
							} else {
								GUI.backgroundColor = buttonColor;
								inputListOpenedText = "Show Run Sway Settings";
							}
							if (GUILayout.Button (inputListOpenedText)) {
								showRunSwaySettings.boolValue = !showRunSwaySettings.boolValue;
							}
							GUI.backgroundColor = buttonColor;
							EditorGUILayout.EndVertical ();

							if (showRunSwaySettings.boolValue) {
								showSelectedSwaySettings (runThirdPersonSwayInfo);
							}
							EditorGUILayout.EndVertical ();
						} else {
							showSelectedSwaySettings (thirdPersonSwayInfo);
						}
					}

					EditorGUILayout.Space ();
				}
			}

			EditorGUILayout.Space ();

			buttonColor = GUI.backgroundColor;
			EditorGUILayout.BeginVertical ();
			if (showFirstPersonSettings.boolValue) {
				GUI.backgroundColor = Color.gray;
				inputListOpenedText = "\nHide First Person Settings\n";
			} else {
				GUI.backgroundColor = buttonColor;
				inputListOpenedText = "\nShow First Person Settings\n";
			}
			if (GUILayout.Button (inputListOpenedText)) {
				showFirstPersonSettings.boolValue = !showFirstPersonSettings.boolValue;
			}
			GUI.backgroundColor = buttonColor;
			EditorGUILayout.EndVertical ();

			if (showFirstPersonSettings.boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("First Person Weapon Settings", "window");
				showWeaponSettings (firstPersonWeaponInfo, false);

				EditorGUILayout.Space ();

				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (firstPersonUseSwayInfo);

				if (firstPersonUseSwayInfo.boolValue) {
					EditorGUILayout.BeginVertical ();
					if (showSwaySettings.boolValue) {
						GUI.backgroundColor = Color.gray;
						inputListOpenedText = "Hide First Person Sway Settings";
					} else {
						GUI.backgroundColor = buttonColor;
						inputListOpenedText = "Show First Person Sway Settings";
					}
					if (GUILayout.Button (inputListOpenedText)) {
						showSwaySettings.boolValue = !showSwaySettings.boolValue;
					}
					GUI.backgroundColor = buttonColor;
					EditorGUILayout.EndVertical ();

					if (showSwaySettings.boolValue) {

						if (firstPersonUseRunPosition.boolValue && firstPersonUseRunSwayInfo.boolValue) {
							GUILayout.BeginVertical ("box");

							EditorGUILayout.BeginVertical ();
							if (showWalkSwaySettings.boolValue) {
								GUI.backgroundColor = Color.gray;
								inputListOpenedText = "Hide Walk Sway Settings";
							} else {
								GUI.backgroundColor = buttonColor;
								inputListOpenedText = "Show Walk Sway Settings";
							}
							if (GUILayout.Button (inputListOpenedText)) {
								showWalkSwaySettings.boolValue = !showWalkSwaySettings.boolValue;
							}
							GUI.backgroundColor = buttonColor;
							EditorGUILayout.EndVertical ();

							if (showWalkSwaySettings.boolValue) {
								showSelectedSwaySettings (firstPersonSwayInfo);
							}

							EditorGUILayout.Space ();

							EditorGUILayout.BeginVertical ();
							if (showRunSwaySettings.boolValue) {
								GUI.backgroundColor = Color.gray;
								inputListOpenedText = "Hide Run Sway Settings";
							} else {
								GUI.backgroundColor = buttonColor;
								inputListOpenedText = "Show Run Sway Settings";
							}
							if (GUILayout.Button (inputListOpenedText)) {
								showRunSwaySettings.boolValue = !showRunSwaySettings.boolValue;
							}
							GUI.backgroundColor = buttonColor;
							EditorGUILayout.EndVertical ();

							if (showRunSwaySettings.boolValue) {
								showSelectedSwaySettings (runFirstPersonSwayInfo);
							}
							EditorGUILayout.EndVertical ();
						} else {
							showSelectedSwaySettings (firstPersonSwayInfo);
						}
					}

					EditorGUILayout.Space ();
				}

				EditorGUILayout.BeginVertical ();
				if (showIdleSettings.boolValue) {
					GUI.backgroundColor = Color.gray;
					inputListOpenedText = "Hide Idle Settings";
				} else {
					GUI.backgroundColor = buttonColor;
					inputListOpenedText = "Show Idle Settings";
				}
				if (GUILayout.Button (inputListOpenedText)) {
					showIdleSettings.boolValue = !showIdleSettings.boolValue;
				}
				GUI.backgroundColor = buttonColor;
				EditorGUILayout.EndVertical ();

				if (showIdleSettings.boolValue) {

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("First Person Weapon Idle Settings", "window");
					EditorGUILayout.PropertyField (useWeaponIdle);
					if (useWeaponIdle.boolValue) {
						EditorGUILayout.PropertyField (timeToActiveWeaponIdle);
						EditorGUILayout.PropertyField (idlePositionAmount);
						EditorGUILayout.PropertyField (idleRotationAmount);
						EditorGUILayout.PropertyField (idleSpeed);
						GUILayout.BeginVertical ("Weapon State", "window");
						EditorGUILayout.PropertyField (playerMoving);
						EditorGUILayout.PropertyField (idleActive);
						GUILayout.EndVertical ();
					}
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();
				}
			}

			EditorGUILayout.Space ();

			EditorGUILayout.BeginVertical ();
			if (showShotShakeSettings.boolValue) {
				GUI.backgroundColor = Color.gray;
				inputListOpenedText = "Hide Shot Shake Settings";
			} else {
				GUI.backgroundColor = buttonColor;
				inputListOpenedText = "Show Shot Shake Settings";
			}
			if (GUILayout.Button (inputListOpenedText)) {
				showShotShakeSettings.boolValue = !showShotShakeSettings.boolValue;
			}
			GUI.backgroundColor = buttonColor;
			EditorGUILayout.EndVertical ();

			if (showShotShakeSettings.boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Shot Shake Settings", "window");
				EditorGUILayout.PropertyField (sameValueBothViews);

				EditorGUILayout.Space ();

				if (sameValueBothViews.boolValue) {
					showShakeInfo (thirdPersonshotShakeInfo, false, true);
				} else {
					EditorGUILayout.PropertyField (useShotShakeInThirdPerson);
					if (useShotShakeInThirdPerson.boolValue) {
						showShakeInfo (thirdPersonshotShakeInfo, false, true);
					}
					EditorGUILayout.PropertyField (useShotShakeInFirstPerson);
					if (useShotShakeInFirstPerson.boolValue) {
						showShakeInfo (firstPersonshotShakeInfo, true, true);
					}
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Shot Camera Noise Settings", "window");
			EditorGUILayout.PropertyField (useShotCameraNoise);
			if (useShotCameraNoise.boolValue) {
				//				EditorGUILayout.PropertyField (objectToUse.FindProperty ("verticalShotCameraNoiseAmount"));
				//				EditorGUILayout.PropertyField (objectToUse.FindProperty ("horizontalShotCameraNoiseAmount"));

				GUILayout.Label (new GUIContent ("Vertical Range"), EditorStyles.boldLabel);
				GUILayout.BeginHorizontal ();
				IKWeaponManager.verticalShotCameraNoiseAmount.x = EditorGUILayout.FloatField (IKWeaponManager.verticalShotCameraNoiseAmount.x, GUILayout.MaxWidth (50));
				EditorGUILayout.MinMaxSlider (ref IKWeaponManager.verticalShotCameraNoiseAmount.x, ref IKWeaponManager.verticalShotCameraNoiseAmount.y, -2, 2);
				IKWeaponManager.verticalShotCameraNoiseAmount.y = EditorGUILayout.FloatField (IKWeaponManager.verticalShotCameraNoiseAmount.y, GUILayout.MaxWidth (50));
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.Label (new GUIContent ("Horizontal Range"), EditorStyles.boldLabel);
				GUILayout.BeginHorizontal ();
				IKWeaponManager.horizontalShotCameraNoiseAmount.x = EditorGUILayout.FloatField (IKWeaponManager.horizontalShotCameraNoiseAmount.x, GUILayout.MaxWidth (50));
				EditorGUILayout.MinMaxSlider (ref IKWeaponManager.horizontalShotCameraNoiseAmount.x, ref IKWeaponManager.horizontalShotCameraNoiseAmount.y, -2, 2);
				IKWeaponManager.horizontalShotCameraNoiseAmount.y = EditorGUILayout.FloatField (IKWeaponManager.horizontalShotCameraNoiseAmount.y, GUILayout.MaxWidth (50));
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Options", "window");
			EditorGUILayout.PropertyField (showThirdPersonGizmo);
			EditorGUILayout.PropertyField (showFirstPersonGizmo);

			if (showThirdPersonGizmo.boolValue || showFirstPersonGizmo.boolValue) {

				EditorGUILayout.PropertyField (gizmoLabelColor);
				EditorGUILayout.PropertyField (showHandsWaypointGizmo);
				EditorGUILayout.PropertyField (showWeaponWaypointGizmo);
				EditorGUILayout.PropertyField (showPositionGizmo);

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Handle Options", "window");
				if (showThirdPersonGizmo.boolValue) {
					EditorGUILayout.PropertyField (thirdPersonUseHandle);
					if (thirdPersonUseHandle.boolValue) {
						EditorGUILayout.PropertyField (thirdPersonHandleRadius);
						EditorGUILayout.PropertyField (thirdPersonHandleGizmoColor);
					}
					EditorGUILayout.PropertyField (thirdPersonUsePositionHandle);
				}

				if (showFirstPersonGizmo.boolValue) {
					EditorGUILayout.PropertyField (firstPersonUseHandle);
					if (firstPersonUseHandle.boolValue) {
						EditorGUILayout.PropertyField (firstPersonHandleRadius);
						EditorGUILayout.PropertyField (firstPersonHandleGizmoColor);
					}
					EditorGUILayout.PropertyField (firstPersonUsePositionHandle);
				}
				GUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		if (showPrefabSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("WEAPON PREFAB SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Pickup Prefab (No Inventory) Settings", "window");
			EditorGUILayout.PropertyField (relativePathWeaponsPickups);
			EditorGUILayout.PropertyField (emtpyWeaponPrefab);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (weaponPrefabModel, new GUIContent ("Weapon Pickup (No Inventory)"));

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Create Weapon Pickup Prefab (No Inventory)")) {
				IKWeaponManager.createWeaponPrefab ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Pickup Prefab (Inventory) Settings", "window");

			EditorGUILayout.PropertyField (inventoryWeaponPrefabObject, new GUIContent ("Weapon Pickup (Inventory)"));

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Assign Weapon Pickup Prefab (Inventory)")) {
				IKWeaponManager.assignWeaponPrefab ();
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Create Inventory Weapon And Ammo Prefabs Settings", "window");
			EditorGUILayout.PropertyField (relativePathWeaponsMesh);
			EditorGUILayout.PropertyField (relativePathWeaponsAmmoMesh);
			EditorGUILayout.PropertyField (inventoryWeaponCategoryName);
			EditorGUILayout.PropertyField (weaponName);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Create Inventory Weapon Object")) {
				IKWeaponManager.createInventoryWeapon ();
			}

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (inventoryAmmoCategoryName);
			EditorGUILayout.PropertyField (ammoAmountPerPickup);
			EditorGUILayout.PropertyField (weaponAmmoMesh);
			EditorGUILayout.PropertyField (weaponAmmoIconTexture);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Create Inventory Weapon Ammo Object")) {
				IKWeaponManager.createInventoryWeaponAmmo ();
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
			EditorUtility.SetDirty (target);
		}
	}

	void showSelectedSwaySettings (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sway Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSway"));

		EditorGUILayout.Space ();

		if (list.FindPropertyRelative ("useSway").boolValue) {
			GUILayout.BeginVertical ("Sway Position Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePositionSway"));
			if (list.FindPropertyRelative ("usePositionSway").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionVertical"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionHorizontal"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionMaxAmount"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionSmooth"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sway Rotation Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRotationSway"));
			if (list.FindPropertyRelative ("useRotationSway").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationVertical"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationHorizontal"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationSmooth"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sway Bob Position Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBobPosition"));
			if (list.FindPropertyRelative ("useBobPosition").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobPositionSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobPositionAmount"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useInputMultiplierForBobPosition"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sway Bob Rotation Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBobRotation"));
			if (list.FindPropertyRelative ("useBobRotation").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobRotationVertical"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobRotationHorizontal"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useInputMultiplierForBobRotation"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Position Clamp Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSwayPositionClamp"));
			if (list.FindPropertyRelative ("useSwayPositionClamp").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionHorizontalClamp"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionVerticalClamp"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Rotation Clamp Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSwayRotationClamp"));
			if (list.FindPropertyRelative ("useSwayRotationClamp").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationClampX"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationClampZ"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Reset Position Rotation Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetSwayPositionSmooth"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetSwayRotationSmooth"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Extra Sway Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("movingExtraPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionRunningMultiplier"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationRunningMultiplier"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobPositionRunningMultiplier"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobRotationRunningMultiplier"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayPositionPercentageAiming"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("swayRotationPercentageAiming"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobPositionPercentageAiming"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bobRotationPercentageAiming"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("minMouseAmountForSway"));
			GUILayout.EndVertical ();

		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showWeaponSettings (SerializedProperty list, bool isThirdPerson)
	{
		EditorGUILayout.Space ();

		showSingleWeaponSettings = list.FindPropertyRelative ("showSingleWeaponSettings").boolValue;

		EditorGUILayout.Space ();

		listButtonBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showSingleWeaponSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Show Single Weapon Settings")) {
			showSingleWeaponSettings = !showSingleWeaponSettings;
		}
		GUI.backgroundColor = listButtonBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		list.FindPropertyRelative ("showSingleWeaponSettings").boolValue = showSingleWeaponSettings;

		if (showSingleWeaponSettings) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("SINGLE WEAPON SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window");

			if (isThirdPerson) {
				EditorGUILayout.PropertyField (useLowerRotationSpeedAimedThirdPerson);
				if (useLowerRotationSpeedAimedThirdPerson.boolValue) {
					EditorGUILayout.PropertyField (verticalRotationSpeedAimedInThirdPerson);
					EditorGUILayout.PropertyField (horizontalRotationSpeedAimedInThirdPerson);
				}

			} else {
				EditorGUILayout.PropertyField (canAimInFirstPerson);
				EditorGUILayout.PropertyField (useLowerRotationSpeedAimed);
				if (useLowerRotationSpeedAimed.boolValue) {
					EditorGUILayout.PropertyField (verticalRotationSpeedAimedInFirstPerson);
					EditorGUILayout.PropertyField (horizontalRotationSpeedAimedInFirstPerson);
				}
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("drawWeaponMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimMovementSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchMovementSpeed"));

			if (isThirdPerson) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeOneOrTwoHandWieldSpeed"));
			}
			GUILayout.EndVertical ();

			if (!isThirdPerson) {
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("First Person Arms Settings", "window");
				EditorGUILayout.PropertyField (firstPersonArms);

				EditorGUILayout.PropertyField (disableOnlyFirstPersonArmsMesh);

				EditorGUILayout.PropertyField (firstPersonArmsMesh);

				EditorGUILayout.Space ();

				showSimpleList (extraFirstPersonArmsMeshes);

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Enable Arms On Editor")) {
					IKWeaponManager.enableOrDisableFirstPersonArmsFromEditor (true);
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Disable Arms On Editor")) {
					IKWeaponManager.enableOrDisableFirstPersonArmsFromEditor (false);
				}

				GUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Positions", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("walkPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("keepPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimRecoilPosition"));

			if (!isThirdPerson) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("walkRecoilPosition"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCrouchPosition"));
				if (list.FindPropertyRelative ("useCrouchPosition").boolValue) {

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchPosition"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchRecoilPosition"));
				}
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lowerWeaponPosition"));

			EditorGUILayout.Space ();

			if (isThirdPerson) {
				GUILayout.BeginVertical ("Deactivate IK If Not Aiming Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivateIKIfNotAiming"));
				if (list.FindPropertyRelative ("deactivateIKIfNotAiming").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("placeWeaponOnWalkPositionBeforeDeactivateIK"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponPositionInHandForDeactivateIK"));

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Copy Weapon Position In Hand")) {
						IKWeaponManager.copyTransformValuesToBuffer (IKWeaponManager.weaponTransform);
					}

					if (GUILayout.Button ("Paste Weapon Position In Hand")) {
						IKWeaponManager.pasteTransformValuesToBuffer (list.FindPropertyRelative ("weaponPositionInHandForDeactivateIK").objectReferenceValue as Transform);
					}

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Draw Deactivate IK Weapon Path", "window");
					showSimpleList (thirdPersonDeactivateIKDrawPath);
					GUILayout.EndVertical ();
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Weapon Rotation Point Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useWeaponRotationPoint"));
				if (list.FindPropertyRelative ("useWeaponRotationPoint").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponRotationPoint"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponRotationPointHolder"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponRotationPointHeadLookTarget"));

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.rotationUpPointAmountMultiplier"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.rotationDownPointAmountMultiplier"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.rotationPointSpeed"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.useRotationUpClamp"));
					if (list.FindPropertyRelative ("rotationPointInfo.useRotationUpClamp").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.rotationUpClampAmount"));
					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.useRotationDownClamp"));
					if (list.FindPropertyRelative ("rotationPointInfo.useRotationDownClamp").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("rotationPointInfo.rotationDownClamp"));
					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponPivotPoint"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			} else {
				GUILayout.BeginVertical ("Reload Path Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadMovement"));
				if (list.FindPropertyRelative ("useReloadMovement").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadSpline"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadDuration"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadLookDirectionSpeed"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}

			GUILayout.BeginVertical ("Surface Position Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkSurfaceCollision"));
			if (list.FindPropertyRelative ("checkSurfaceCollision").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("collisionRayDistance"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("collisionMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceCollisionPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceCollisionRayPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("hideCursorOnCollision"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Run Position Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRunPosition"));
			if (list.FindPropertyRelative ("useRunPosition").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("runMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("runPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("hideCursorOnRun"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRunSwayInfo"));
			}
			GUILayout.EndVertical ();


			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Jump Position Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useJumpPositions"));
			if (list.FindPropertyRelative ("useJumpPositions").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpStartPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpEndPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpStartMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpEndtMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetJumpMovementSped"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayAtJumpEnd"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Run Fov Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewFovOnRun"));
			if (list.FindPropertyRelative ("useNewFovOnRun").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeFovSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("newFovOnRun"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Melee Attack Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMeleeAttack"));
			if (list.FindPropertyRelative ("useMeleeAttack").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomMeleeAttackLayer"));
				if (list.FindPropertyRelative ("useCustomMeleeAttackLayer").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("customMeleeAttackLayer"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackPosition"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCapsuleRaycast"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackStartMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackEndMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackEndDelay"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackRaycastPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackRaycastDistance"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackRaycastRadius"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackDamageAmount"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackIgnoreShield"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMeleeAttackShakeInfo"));
				if (list.FindPropertyRelative ("useMeleeAttackShakeInfo").boolValue) {

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Melee Attack Shake Settings", "window");
					if (isThirdPerson) {
						showShakeInfo (thirdPersonMeleeAttackShakeInfo, false, false);
					} else {
						showShakeInfo (firstPersonMeleeAttackShakeInfo, true, false);
					}
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyMeleeAtackForce"));
				if (list.FindPropertyRelative ("applyMeleeAtackForce").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackForceAmount"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackForceMode"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackApplyForceToVehicles"));
					if (list.FindPropertyRelative ("meleeAttackApplyForceToVehicles").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleAttackForceToVehicles"));
					}
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMeleeAttackSound"));
				if (list.FindPropertyRelative ("useMeleeAttackSound").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackSurfaceSound"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackAirSound"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMeleeAttackParticles"));
				if (list.FindPropertyRelative ("useMeleeAttackParticles").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackParticles"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("canActivateReactionSystemTemporally"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageReactionID"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageTypeID"));
			
				if (isThirdPerson) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useActionSystemForMeleeAttack"));
					if (list.FindPropertyRelative ("useActionSystemForMeleeAttack").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackActionName"));
					}			
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Attachment Positions", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("hasAttachments"));
			if (list.FindPropertyRelative ("hasAttachments").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("editAttachmentPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftHandMesh"));

				if (!isThirdPerson) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraLeftHandMesh"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraLeftHandMeshParent"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftHandEditPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("editAttachmentMovementSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("editAttachmentHandSpeed"));

				if (!isThirdPerson) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSightPosition"));
					if (list.FindPropertyRelative ("useSightPosition").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("sightPosition"));
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("sightRecoilPosition"));
					}
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentCameraPosition"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("leftHandParent"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("usingSecondPositionForHand"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondPositionForHand"));
			}
			GUILayout.EndVertical ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Movement Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canRunWhileCarrying"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canRunWhileAiming"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewCarrySpeed"));
			if (list.FindPropertyRelative ("useNewCarrySpeed").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("newCarrySpeed"));
			}

			if (!isThirdPerson) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewAimSpeed"));
				if (list.FindPropertyRelative ("useNewAimSpeed").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("newAimSpeed"));
				}
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Recoil Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponHasRecoil"));
			if (list.FindPropertyRelative ("weaponHasRecoil").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("recoilSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("endRecoilSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useExtraRandomRecoil"));
				if (list.FindPropertyRelative ("useExtraRandomRecoil").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useExtraRandomRecoilPosition"));
					if (list.FindPropertyRelative ("useExtraRandomRecoilPosition").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraRandomRecoilPosition"));
					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useExtraRandomRecoilRotation"));
					if (list.FindPropertyRelative ("useExtraRandomRecoilRotation").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("extraRandomRecoilRotation"));
					}
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			if (isThirdPerson) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Quick Draw Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useQuickDrawKeepWeapon"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Hands State", "window");

				GUILayout.Label ("Hands In Position To Aim\t" + list.FindPropertyRelative ("handsInPosition").boolValue);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Draw/Keep Weapon Bezier Path", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBezierCurve"));
				if (list.FindPropertyRelative ("useBezierCurve").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("spline"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("bezierDuration"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookDirectionSpeed"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Draw/Keep Weapon Path", "window");
				showSimpleList (thirdPersonKeepPath);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Hands Info", "window");
				showHandList (thirdPersonHandsInfo);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Head Look Settings", "window");
				EditorGUILayout.PropertyField (headLookWhenAiming);
				if (headLookWhenAiming.boolValue) {
					EditorGUILayout.PropertyField (headLookSpeed);
					EditorGUILayout.PropertyField (headLookTarget);
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Custom Camera State Third Person Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomThirdPersonAimActive"));
				if (list.FindPropertyRelative ("useCustomThirdPersonAimActive").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("customDefaultThirdPersonAimRightStateName"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("customDefaultThirdPersonAimLeftStateName"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Idle ID Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewIdleID"));
				if (list.FindPropertyRelative ("useNewIdleID").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("newIdleID"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Draw/Keep Weapon Settings Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDrawKeepWeaponAnimation"));
				if (list.FindPropertyRelative ("useDrawKeepWeaponAnimation").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("drawWeaponActionName"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("keepWeaponActionName"));

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToPlaceWeaponOnHandOnDrawAnimation"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToPlaceWeaponOnKeepAnimation"));
				}
				GUILayout.EndVertical ();
			}

			EditorGUILayout.Space ();
		}

		showDualWeaponSettings = list.FindPropertyRelative ("showDualWeaponSettings").boolValue;

		EditorGUILayout.Space ();

		listButtonBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showDualWeaponSettings) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Show Dual Weapon Settings")) {
			showDualWeaponSettings = !showDualWeaponSettings;
		}
		GUI.backgroundColor = listButtonBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		list.FindPropertyRelative ("showDualWeaponSettings").boolValue = showDualWeaponSettings;

		if (showDualWeaponSettings) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("DUAL WEAPON SETTINGS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Dual Weapon Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsedAsDualWeapon"));
			if (list.FindPropertyRelative ("canBeUsedAsDualWeapon").boolValue) {

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				EditorGUILayout.LabelField ("RIGHT HAND WEAPON SETTINGS", style);

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Right Hand Weapon Settings", "window");
				showDualWeaponInfo (list.FindPropertyRelative ("rightHandDualWeaopnInfo"), isThirdPerson);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				EditorGUILayout.LabelField ("LEFT HAND WEAPON SETTINGS", style);

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Left Hand Weapon Settings", "window");
				showDualWeaponInfo (list.FindPropertyRelative ("leftHandDualWeaponInfo"), isThirdPerson);
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();
		}
	}

	void showDualWeaponInfo (SerializedProperty list, bool isThirdPerson)
	{
		GUILayout.BeginVertical ("Weapon Positions", "window");
		if (isThirdPerson) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimRecoilPosition"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("walkPosition"));

		if (!isThirdPerson) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("walkRecoilPosition"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchRecoilPosition"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("keepPosition"));

		GUILayout.EndVertical ();

		if (!isThirdPerson) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Lower Position Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lowerWeaponPosition"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Surface Position Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("collisionRayDistance"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceCollisionPosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceCollisionRayPosition"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Run Position Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("runPosition"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (!isThirdPerson) {
			GUILayout.BeginVertical ("Jump Position Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpStartPosition"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpEndPosition"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Melee Attack Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackPosition"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("meleeAttackRaycastPosition"));
		GUILayout.EndVertical ();    

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Attachment Positions", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("editAttachmentPosition"));
		if (isThirdPerson) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("attachmentCameraPosition"));
		}
		GUILayout.EndVertical ();    

		if (!isThirdPerson) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("First Person Arms Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("firstPersonHandMesh"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Reload Path Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadMovement"));
			if (list.FindPropertyRelative ("useReloadMovement").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadSpline"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadDuration"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadLookDirectionSpeed"));
			}
			GUILayout.EndVertical ();
		}

		if (isThirdPerson) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Hands State", "window");

			GUILayout.Label ("Hands In Position To Aim\t" + list.FindPropertyRelative ("handsInPosition").boolValue);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Draw/Keep Weapon Bezier Path", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBezierCurve"));
			if (list.FindPropertyRelative ("useBezierCurve").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("spline"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("bezierDuration"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookDirectionSpeed"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Draw/Keep Weapon Path", "window");
			showSimpleList (list.FindPropertyRelative ("keepPath"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Hands Info", "window");
			showHandList (list.FindPropertyRelative ("handsInfo"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Draw/Keep Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useQuickDrawKeepWeapon"));

			if (isThirdPerson) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("placeWeaponOnKeepPositionSideBeforeDraw"));
			}
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();


		if (isThirdPerson) {
			GUILayout.BeginVertical ("Deactivate IK If Not Aiming Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("deactivateIKIfNotAiming"));
			if (list.FindPropertyRelative ("deactivateIKIfNotAiming").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("placeWeaponOnWalkPositionBeforeDeactivateIK"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponPositionInHandForDeactivateIK"));

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Copy Weapon Position In Hand")) {
					IKWeaponManager.copyTransformValuesToBuffer (IKWeaponManager.weaponTransform);
				}

				if (GUILayout.Button ("Paste Weapon Position In Hand")) {
					IKWeaponManager.pasteTransformValuesToBuffer (list.FindPropertyRelative ("weaponPositionInHandForDeactivateIK").objectReferenceValue as Transform);
				}

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Draw Deactivate IK Weapon Path", "window");
				showSimpleList (list.FindPropertyRelative ("deactivateIKDrawPath"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}

			GUILayout.EndVertical ();
		}
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

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

	void showHandElementInfo (SerializedProperty list)
	{
		Color listButtonBackgroundColor;
		showElbowInfo = list.FindPropertyRelative ("showElbowInfo").boolValue;

		GUILayout.BeginVertical ("box");

		GUILayout.BeginVertical ("Hand Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("handTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("limb"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("position"), new GUIContent ("IK Position"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("waypointFollower"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("handMovementSpeed"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Grabbing Hand ID Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useGrabbingHandID"));
		if (list.FindPropertyRelative ("useGrabbingHandID").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("grabbingHandID"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hand Way Points", "window");
		//		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBezierCurve"));
		//		if (list.FindPropertyRelative ("useBezierCurve").boolValue) {
		//			EditorGUILayout.PropertyField (list.FindPropertyRelative ("spline"));
		//			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bezierDuration"));
		//			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lookDirectionSpeed"));
		//		} else {
		//			EditorGUILayout.Space ();

		showSimpleList (list.FindPropertyRelative ("wayPoints"));
		//		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Hand Settings and State", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("handUsedInWeapon"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usedToDrawWeapon"));

		GUILayout.Label ("Hand In Position To Aim\t" + list.FindPropertyRelative ("handInPositionToAim").boolValue);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("HandIKWeight"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("targetValue"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("handMovementSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("waypointFollower"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		listButtonBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showElbowInfo) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Elbow Settings")) {
			showElbowInfo = !showElbowInfo;
		}
		GUI.backgroundColor = listButtonBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		list.FindPropertyRelative ("showElbowInfo").boolValue = showElbowInfo;

		if (showElbowInfo) {
			GUILayout.BeginVertical ("box");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("elbowInfo.Name"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("elbowInfo.elbow"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("elbowInfo.position"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("elbowInfo.elbowOriginalPosition"));
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
	}

	void showHandList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			if (list.arraySize < 2) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Add Hand")) {
					list.arraySize++;
				}
				if (GUILayout.Button ("Clear")) {
					list.arraySize = 0;
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
			}
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
						showHandElementInfo (list.GetArrayElementAtIndex (i));
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

	void showShakeInfo (SerializedProperty list, bool isFirstPerson, bool isShootShake)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shotForce"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeDuration"));
		if (isFirstPerson) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakePosition"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotation"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Test Shake")) {
			if (Application.isPlaying) {
				if (isShootShake) {
					IKWeaponManager.checkWeaponCameraShake ();
				} else {
					IKWeaponManager.checkMeleeAttackShakeInfo ();
				}
			}
		}
		GUILayout.EndVertical ();
	}
}
#endif