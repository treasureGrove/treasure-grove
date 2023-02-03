using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(playerWeaponSystem))]
public class playerWeaponSystemEditor : Editor
{
	SerializedProperty weaponSettingsName;
	SerializedProperty weaponSettingsNumberKey;
	SerializedProperty reloading;
	SerializedProperty carryingWeaponInThirdPerson;
	SerializedProperty carryingWeaponInFirstPerson;
	SerializedProperty aimingInThirdPerson;
	SerializedProperty aimingInFirstPerson;
	SerializedProperty weaponUpdateActive;
	SerializedProperty showAllSettings;
	SerializedProperty showHUDSettings;
	SerializedProperty showWeaponSettings;
	SerializedProperty showProjectileSettings;
	SerializedProperty showAmmoSettings;
	SerializedProperty showFireTypeSettings;
	SerializedProperty showSoundAndParticlesSettings;
	SerializedProperty showEventsSettings;
	SerializedProperty showOtherSettings;
	SerializedProperty weaponSettings;
	SerializedProperty layer;
	SerializedProperty playerControllerGameObject;
	SerializedProperty playerCameraGameObject;
	SerializedProperty useHumanBodyBonesEnum;
	SerializedProperty weaponParentBone;
	SerializedProperty weaponProjectile;
	SerializedProperty outOfAmmo;
	SerializedProperty useEventOnSetDualWeapon;
	SerializedProperty eventOnSetRightWeapon;
	SerializedProperty eventOnSetLeftWeapon;
	SerializedProperty useEventOnSetSingleWeapon;
	SerializedProperty eventOnSetSingleWeapon;
	SerializedProperty useRemoteEventOnObjectsFound;
	SerializedProperty remoteEventNameList;

	SerializedProperty useRemoteEventOnObjectsFoundOnExplosion;
	SerializedProperty remoteEventNameOnExplosion;

	SerializedProperty impactDecalList;
	SerializedProperty impactDecalIndex;
	SerializedProperty impactDecalName;

	SerializedProperty mainDecalManagerName;
	SerializedProperty mainNoiseMeshManagerName;

	SerializedProperty canMarkTargets;
	SerializedProperty tagListToMarkTargets;
	SerializedProperty markTargetName;
	SerializedProperty maxDistanceToMarkTargets;
	SerializedProperty markTargetsLayer;
	SerializedProperty canMarkTargetsOnFirstPerson;
	SerializedProperty canMarkTargetsOnThirdPerson;
	SerializedProperty aimOnFirstPersonToMarkTarget;
	SerializedProperty useMarkTargetSound;
	SerializedProperty markTargetSound;

	SerializedProperty projectilesPoolEnabled;

	SerializedProperty maxAmountOfPoolElementsOnWeapon;

	SerializedProperty useAbilitiesListToEnableOnWeapon;
	SerializedProperty abilitiesListToEnableOnWeapon;

	SerializedProperty useAbilitiesListToDisableOnWeapon;
	SerializedProperty abilitiesListToDisableOnWeapon;

	SerializedProperty useObjectDurability;

	SerializedProperty durabilityUsedOnAttack;

	SerializedProperty mainDurabilityInfo;

	playerWeaponSystem weapon;

	bool showDrawAimFunctionSettings;
	Color buttonColor;

	string buttonMessage;

	GUIStyle style = new GUIStyle ();

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		weaponSettingsName = serializedObject.FindProperty ("weaponSettings.Name");
		weaponSettingsNumberKey = serializedObject.FindProperty ("weaponSettings.numberKey");
		reloading = serializedObject.FindProperty ("reloading");
		carryingWeaponInThirdPerson = serializedObject.FindProperty ("carryingWeaponInThirdPerson");
		carryingWeaponInFirstPerson = serializedObject.FindProperty ("carryingWeaponInFirstPerson");
		aimingInThirdPerson = serializedObject.FindProperty ("aimingInThirdPerson");
		aimingInFirstPerson = serializedObject.FindProperty ("aimingInFirstPerson");
		weaponUpdateActive = serializedObject.FindProperty ("weaponUpdateActive");
		showAllSettings = serializedObject.FindProperty ("showAllSettings");
		showHUDSettings = serializedObject.FindProperty ("showHUDSettings");
		showWeaponSettings = serializedObject.FindProperty ("showWeaponSettings");
		showProjectileSettings = serializedObject.FindProperty ("showProjectileSettings");
		showAmmoSettings = serializedObject.FindProperty ("showAmmoSettings");
		showFireTypeSettings = serializedObject.FindProperty ("showFireTypeSettings");
		showSoundAndParticlesSettings = serializedObject.FindProperty ("showSoundAndParticlesSettings");
		showEventsSettings = serializedObject.FindProperty ("showEventsSettings");
		showOtherSettings = serializedObject.FindProperty ("showOtherSettings");
		weaponSettings = serializedObject.FindProperty ("weaponSettings");
		layer = serializedObject.FindProperty ("layer");
		playerControllerGameObject = serializedObject.FindProperty ("playerControllerGameObject");
		playerCameraGameObject = serializedObject.FindProperty ("playerCameraGameObject");
		useHumanBodyBonesEnum = serializedObject.FindProperty ("useHumanBodyBonesEnum");
		weaponParentBone = serializedObject.FindProperty ("weaponParentBone");
		weaponProjectile = serializedObject.FindProperty ("weaponProjectile");
		outOfAmmo = serializedObject.FindProperty ("outOfAmmo");
		useEventOnSetDualWeapon = serializedObject.FindProperty ("useEventOnSetDualWeapon");
		eventOnSetRightWeapon = serializedObject.FindProperty ("eventOnSetRightWeapon");
		eventOnSetLeftWeapon = serializedObject.FindProperty ("eventOnSetLeftWeapon");
		useEventOnSetSingleWeapon = serializedObject.FindProperty ("useEventOnSetSingleWeapon");
		eventOnSetSingleWeapon = serializedObject.FindProperty ("eventOnSetSingleWeapon");
		useRemoteEventOnObjectsFound = serializedObject.FindProperty ("useRemoteEventOnObjectsFound");
		remoteEventNameList = serializedObject.FindProperty ("remoteEventNameList");

		useRemoteEventOnObjectsFoundOnExplosion = serializedObject.FindProperty ("useRemoteEventOnObjectsFoundOnExplosion");
		remoteEventNameOnExplosion = serializedObject.FindProperty ("remoteEventNameOnExplosion");

		impactDecalList = serializedObject.FindProperty ("impactDecalList");
		impactDecalIndex = serializedObject.FindProperty ("impactDecalIndex");
		impactDecalName = serializedObject.FindProperty ("impactDecalName");

		mainDecalManagerName = serializedObject.FindProperty ("mainDecalManagerName");
		mainNoiseMeshManagerName = serializedObject.FindProperty ("mainNoiseMeshManagerName");

		canMarkTargets = serializedObject.FindProperty ("canMarkTargets");
		tagListToMarkTargets = serializedObject.FindProperty ("tagListToMarkTargets");
		markTargetName = serializedObject.FindProperty ("markTargetName");
		maxDistanceToMarkTargets = serializedObject.FindProperty ("maxDistanceToMarkTargets");
		markTargetsLayer = serializedObject.FindProperty ("markTargetsLayer");
		canMarkTargetsOnFirstPerson = serializedObject.FindProperty ("canMarkTargetsOnFirstPerson");
		canMarkTargetsOnThirdPerson = serializedObject.FindProperty ("canMarkTargetsOnThirdPerson");
		aimOnFirstPersonToMarkTarget = serializedObject.FindProperty ("aimOnFirstPersonToMarkTarget");
		useMarkTargetSound = serializedObject.FindProperty ("useMarkTargetSound");
		markTargetSound = serializedObject.FindProperty ("markTargetSound");

		projectilesPoolEnabled = serializedObject.FindProperty ("projectilesPoolEnabled");

		maxAmountOfPoolElementsOnWeapon = serializedObject.FindProperty ("maxAmountOfPoolElementsOnWeapon");


		useAbilitiesListToEnableOnWeapon = serializedObject.FindProperty ("useAbilitiesListToEnableOnWeapon");

		abilitiesListToEnableOnWeapon = serializedObject.FindProperty ("abilitiesListToEnableOnWeapon");

		useAbilitiesListToDisableOnWeapon = serializedObject.FindProperty ("useAbilitiesListToDisableOnWeapon");

		abilitiesListToDisableOnWeapon = serializedObject.FindProperty ("abilitiesListToDisableOnWeapon");

		useObjectDurability = serializedObject.FindProperty ("useObjectDurability");

		durabilityUsedOnAttack = serializedObject.FindProperty ("durabilityUsedOnAttack");

		mainDurabilityInfo = serializedObject.FindProperty ("mainDurabilityInfo");


		weapon = (playerWeaponSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Weapon Info", "window");
		EditorGUILayout.PropertyField (weaponSettingsName);
		EditorGUILayout.PropertyField (weaponSettingsNumberKey);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon State", "window");
		GUILayout.Label ("Reloading\t\t\t" + reloading.boolValue);
		GUILayout.Label ("Carrying 3rd Person\t\t" + carryingWeaponInThirdPerson.boolValue);
		GUILayout.Label ("Carrying 1st Person\t\t" + carryingWeaponInFirstPerson.boolValue);
		GUILayout.Label ("Aiming 3rd Person\t\t" + aimingInThirdPerson.boolValue);
		GUILayout.Label ("Aiming 1st Person\t\t" + aimingInFirstPerson.boolValue);
		GUILayout.Label ("Update Active\t\t" + weaponUpdateActive.boolValue);
		GUILayout.Label ("Key Number\t\t" + weaponSettingsNumberKey.intValue);
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUI.color = Color.cyan;
		EditorGUILayout.HelpBox ("Configure every setting of this weapon", MessageType.None);
		GUI.color = Color.white;

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.BeginHorizontal ();

		if (showHUDSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("HUD")) {
			showHUDSettings.boolValue = !showHUDSettings.boolValue;
		}

		if (showWeaponSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Weapon")) {
			showWeaponSettings.boolValue = !showWeaponSettings.boolValue;
		}

		if (showProjectileSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Projectile")) {
			showProjectileSettings.boolValue = !showProjectileSettings.boolValue;
		}

		if (showAmmoSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Ammo")) {
			showAmmoSettings.boolValue = !showAmmoSettings.boolValue;
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();

		if (showFireTypeSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Fire Type")) {
			showFireTypeSettings.boolValue = !showFireTypeSettings.boolValue;
		}

		if (showSoundAndParticlesSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Sound & Particles")) {
			showSoundAndParticlesSettings.boolValue = !showSoundAndParticlesSettings.boolValue;
		}

		if (showEventsSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Events")) {
			showEventsSettings.boolValue = !showEventsSettings.boolValue;
		}
			
		if (showOtherSettings.boolValue) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = buttonColor;
		}
		if (GUILayout.Button ("Others")) {
			showOtherSettings.boolValue = !showOtherSettings.boolValue;
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

			showHUDSettings.boolValue = showAllSettings.boolValue;
			showWeaponSettings.boolValue = showAllSettings.boolValue;
			showProjectileSettings.boolValue = showAllSettings.boolValue;
			showAmmoSettings.boolValue = showAllSettings.boolValue;
			showFireTypeSettings.boolValue = showAllSettings.boolValue;
			showSoundAndParticlesSettings.boolValue = showAllSettings.boolValue;
			showEventsSettings.boolValue = showAllSettings.boolValue;
			showOtherSettings.boolValue = showAllSettings.boolValue;
		}

		GUI.backgroundColor = buttonColor;
	
		EditorGUILayout.Space ();

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 30;
		style.alignment = TextAnchor.MiddleCenter;
	
		showSettings (weaponSettings);
	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showSettings (SerializedProperty list)
	{
		if (showHUDSettings.boolValue || showAllSettings.boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("HUD", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("HUD Elements Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponIcon"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponInventorySlotIcon"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponIConHUD"), new GUIContent ("Weapon Icon HUD"));	
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("showWeaponNameInHUD"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("showWeaponIconInHUD"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("showWeaponAmmoSliderInHUD"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("showWeaponAmmoTextInHUD"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sniperSightThirdPersonEnabled"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sniperSightFirstPersonEnabled"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Reticle Setting", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReticleThirdPerson"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAimReticleThirdPerson"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReticleFirstPerson"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAimReticleFirstPerson"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReticle"));
			if (list.FindPropertyRelative ("useCustomReticle").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("regularCustomReticle"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAimCustomReticle"));
				if (list.FindPropertyRelative ("useAimCustomReticle").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("aimCustomReticle"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("HUD/Hologram On Weapon Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCanvasHUD"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHUD"));
			if (list.FindPropertyRelative ("useHUD").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("clipSizeText"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("remainAmmoText"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUD"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("ammoInfoHUD"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableHUDInFirstPersonAim"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeHUDPosition"));
				if (list.FindPropertyRelative ("changeHUDPosition").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDTransformInThirdPerson"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDTransformInFirstPerson"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHUDDualWeaponThirdPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHUDDualWeaponFirstPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeHUDPositionDualWeapon"));
				if (list.FindPropertyRelative ("changeHUDPositionDualWeapon").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDRightHandTransformThirdPerson"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDLeftHandTransformThirdPerson"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDRightHandTransformFirstPerson"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("HUDLeftHandTransformFirstPerson"));
				}
			}
			GUILayout.EndVertical ();
		}

		if (showWeaponSettings.boolValue || showAllSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("WEAPON", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Fire Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("automatic"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("fireRate"));
			if (list.FindPropertyRelative ("automatic").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBurst"));
				if (list.FindPropertyRelative ("useBurst").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("burstAmount"));
				}
			}
			EditorGUILayout.PropertyField (layer, new GUIContent ("Layer To Check"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Reload Settings", "window");
			GUILayout.BeginVertical ("Third Person Reload Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadTimeThirdPerson"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadDelayThirdPerson"));
			if (list.FindPropertyRelative ("useReloadDelayThirdPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadDelayThirdPerson"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePreReloadDelayThirdPerson"));
			if (list.FindPropertyRelative ("usePreReloadDelayThirdPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("preReloadDelayThirdPerson"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("First Person Reload Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadTimeFirstPerson"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadDelayFirstPerson"));
			if (list.FindPropertyRelative ("useReloadDelayFirstPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadDelayFirstPerson"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePreReloadDelayFirstPerson"));
			if (list.FindPropertyRelative ("usePreReloadDelayFirstPerson").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("preReloadDelayFirstPerson"));
			}
			GUILayout.EndVertical ();
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Components", "window");
			EditorGUILayout.PropertyField (playerControllerGameObject);
			EditorGUILayout.PropertyField (playerCameraGameObject);
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponMesh"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Place On Player's Body Settings", "window");
			EditorGUILayout.PropertyField (useHumanBodyBonesEnum);	
			if (useHumanBodyBonesEnum.boolValue) {
				EditorGUILayout.PropertyField (weaponParentBone);
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponParent"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Weapon Animations", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFireAnimation"));
			if (list.FindPropertyRelative ("useFireAnimation").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("animation"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("animationSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("playAnimationBackward"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadAnimation"));
			if (list.FindPropertyRelative ("useReloadAnimation").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadAnimationName"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadAnimationSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadOnThirdPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadOnFirstPerson"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Shell Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shell"));
			if (list.FindPropertyRelative ("shell").objectReferenceValue) {
				showSimpleList (list.FindPropertyRelative ("shellPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("shellEjectionForce"));
				showSimpleList (list.FindPropertyRelative ("shellDropSoundList"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useShellDelay"));
				if (list.FindPropertyRelative ("useShellDelay").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("shellDelayThirdPerson"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("shellDelayFirsPerson"), new GUIContent ("Shell Delay First Person"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkToCreateShellsIfNoRemainAmmo"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("removeDroppedShellsAfterTime"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("createShellsOnReload"));
				if (list.FindPropertyRelative ("createShellsOnReload").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("createShellsOnReloadDelayThirdPerson"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("createShellsOnReloadDelayFirstPerson"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxAmountOfShellsBeforeRemoveThem"));
			}
			GUILayout.EndVertical ();
		}

		if (showProjectileSettings.boolValue || showAllSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("PROJECTILE", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Projectile Settings", "window");

			EditorGUILayout.PropertyField (projectilesPoolEnabled);
			EditorGUILayout.PropertyField (maxAmountOfPoolElementsOnWeapon);

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootAProjectile"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("launchProjectile"));

			EditorGUILayout.PropertyField (weaponProjectile);
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectilesPerShoot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileDamage"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("killInOneShot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileWithAbility"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("allowDamageForProjectileOwner"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useInfiniteRaycastDistance"));
			if (!list.FindPropertyRelative ("useInfiniteRaycastDistance").boolValue) {

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxRaycastDistance"));
			}

			if (list.FindPropertyRelative ("launchProjectile").boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Launch Projectile Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateLaunchParableThirdPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateLaunchParableFirstPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useParableSpeed"));
				if (!list.FindPropertyRelative ("useParableSpeed").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileSpeed"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("parableDirectionTransform"));
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMaxDistanceWhenNoSurfaceFound"));
					if (list.FindPropertyRelative ("useMaxDistanceWhenNoSurfaceFound").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDistanceWhenNoSurfaceFound"));
					}
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastCheckingOnRigidbody"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("customRaycastCheckingRate"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("customRaycastCheckingDistance"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("fireWeaponForward"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRayCastShoot"));

				if (list.FindPropertyRelative ("useRayCastShoot").boolValue) {

					EditorGUILayout.Space ();

					GUILayout.BeginVertical ("Shoot Delay and Fake Trail Settings", "window");
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastShootDelay"));
					if (list.FindPropertyRelative ("useRaycastShootDelay").boolValue) {

						EditorGUILayout.PropertyField (list.FindPropertyRelative ("getDelayWithDistance"));
						if (list.FindPropertyRelative ("getDelayWithDistance").boolValue) {
							EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayWithDistanceSpeed"));
							EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDelayWithDistance"));
						} else {
							EditorGUILayout.PropertyField (list.FindPropertyRelative ("raycastShootDelay"));
						}
					}

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useFakeProjectileTrails"));
					GUILayout.EndVertical ();

					EditorGUILayout.Space ();

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkIfSurfacesCloseToWeapon"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("minDistanceToCheck"));

					EditorGUILayout.Space ();
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileSpeed"));

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastCheckingOnRigidbody"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("customRaycastCheckingRate"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("customRaycastCheckingDistance"));
				}
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("setProjectileMeshRotationToFireRotation"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Projectile Circumnavigation Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkGravitySystemOnProjectile"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkCircumnavigationValuesOnProjectile"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("gravityForceForCircumnavigationOnProjectile"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Search Target Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isHommingProjectile"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isSeeker"));
			if (list.FindPropertyRelative ("isHommingProjectile").boolValue || list.FindPropertyRelative ("isSeeker").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("waitTimeToSearchTarget"));
				showTagToLocateList (list.FindPropertyRelative ("tagToLocate"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("targetOnScreenForSeeker"));
			}
			if (list.FindPropertyRelative ("isHommingProjectile").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("locatedEnemyIconName"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Force Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactForceApplied"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceMode"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyImpactForceToVehicles"));
			if (list.FindPropertyRelative ("applyImpactForceToVehicles").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactForceToVehiclesMultiplier"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceMassMultiplier"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Explosion Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isExplosive"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isImplosive"));
			if (list.FindPropertyRelative ("isExplosive").boolValue || list.FindPropertyRelative ("isImplosive").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionForce"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionRadius"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useExplosionDelay"));
				if (list.FindPropertyRelative ("useExplosionDelay").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionDelay"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionDamage"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacters"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("canDamageProjectileOwner"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyExplosionForceToVehicles"));
				if (list.FindPropertyRelative ("applyExplosionForceToVehicles").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("explosionForceToVehiclesMultiplier"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Disable Projectile Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDisableTimer"));
			if (list.FindPropertyRelative ("useDisableTimer").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("noImpactDisableTimer"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactDisableTimer"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Spread Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useProjectileSpread"));
			if (list.FindPropertyRelative ("useProjectileSpread").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("thirdPersonSpreadAmountAiming"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("thirdPersonSpreadAmountNoAiming"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("firstPersonSpreadAmountAiming"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("firstPersonSpreadAmountNoAiming"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Projectile Position Settings", "window");
			showSimpleList (list.FindPropertyRelative ("projectilePosition"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkCrossingSurfacesOnCameraDirection"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Projectile Adherence Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useGravityOnLaunch"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useGraivtyOnImpact"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("adhereToSurface"));
			if (list.FindPropertyRelative ("adhereToSurface").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("adhereToLimbs"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreSetProjectilePositionOnImpact"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Projectile Pierce Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("breakThroughObjects"));
			if (list.FindPropertyRelative ("breakThroughObjects").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteNumberOfImpacts"));
				if (!list.FindPropertyRelative ("infiniteNumberOfImpacts").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberOfImpacts"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("canDamageSameObjectMultipleTimes"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Shield And Reaction Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreShield"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canActivateReactionSystemTemporally"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageReactionID"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageTypeID"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileCanBeDeflected"));
			GUILayout.EndVertical ();
		}

		if (showAmmoSettings.boolValue || showAllSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("AMMO", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Ammo Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponUsesAmmo"));
			if (list.FindPropertyRelative ("weaponUsesAmmo").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("clipSize"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmmo"));
				if (!list.FindPropertyRelative ("infiniteAmmo").boolValue) {

					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemainAmmoFromInventory"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("remainAmmo"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("removePreviousAmmoOnClip"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("startWithEmptyClip"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("autoReloadWhenClipEmpty"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAmmoLimit"));
				if (list.FindPropertyRelative ("useAmmoLimit").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("ammoLimit"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("ammoName"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreAnyWaitTimeOnReload"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Magazine Mesh Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dropClipWhenReload"));
			if (list.FindPropertyRelative ("dropClipWhenReload").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionToDropClip"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("clipModel"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayDropClipWhenReloadThirdPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayDropClipWhenReloadFirstPerson"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("dropClipWhenReloadFirstPerson"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("dropClipWhenReloadThirdPerson"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("magazineInHandTransform"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("magazineInHandGameObject"));
			GUILayout.EndVertical ();
		}

		if (showFireTypeSettings.boolValue || showAllSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("FIRE TYPE", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Damage Target Over Time Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageTargetOverTime"));
			if (list.FindPropertyRelative ("damageTargetOverTime").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeDelay"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeDuration"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeAmount"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeRate"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("damageOverTimeToDeath"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("removeDamageOverTimeState"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sedate Characters Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateCharacters"));
			if (list.FindPropertyRelative ("sedateCharacters").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateDelay"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useWeakSpotToReduceDelay"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateUntilReceiveDamage"));
				if (!list.FindPropertyRelative ("sedateUntilReceiveDamage").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("sedateDuration"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Push Characters Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacter"));
			if (list.FindPropertyRelative ("pushCharacter").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacterForce"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("pushCharacterRagdollForce"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (showSoundAndParticlesSettings.boolValue || showAllSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("SOUNDS & PARTICLES", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sound Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootSoundEffect"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("silencerShootEffect"));	
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactSoundEffect"));
			EditorGUILayout.PropertyField (outOfAmmo);
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadSoundEffect"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("cockSound"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSoundsPool"));
			if (list.FindPropertyRelative ("useSoundsPool").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxSoundsPoolAmount"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponEffectSourcePrefab"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponEffectSourceParent"));
			}
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Draw/Keep Weapon Sound Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSoundOnDrawKeepWeapon"));
			if (list.FindPropertyRelative ("useSoundOnDrawKeepWeapon").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("drawWeaponSound"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("keepWeaponSound"));
			}
			GUILayout.EndVertical ();
			GUILayout.EndVertical ();
	
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Particle Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("setShootParticlesLayerOnFirstPerson"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Muzzle Flash Light Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMuzzleFlash"));
			if (list.FindPropertyRelative ("useMuzzleFlash").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("muzzleFlahsLight"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("muzzleFlahsDuration"));
			}
			GUILayout.EndVertical ();
		}

		if (showEventsSettings.boolValue || showAllSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("EVENTS", style);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Ability Weapon Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weaponWithAbility"));
			if (list.FindPropertyRelative ("weaponWithAbility").boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Button Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDownButton"));
				if (list.FindPropertyRelative ("useDownButton").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("downButtonAction"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useHoldButton"));
				if (list.FindPropertyRelative ("useHoldButton").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("holdButtonAction"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useUpButton"));
				if (list.FindPropertyRelative ("useUpButton").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("upButtonAction"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnFireWeapon"));
				if (list.FindPropertyRelative ("useEventOnFireWeapon").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnFireWeapon"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();
			}
			GUILayout.EndVertical ();


			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Draw/Aim Settings", "window");
			buttonColor = GUI.backgroundColor;
			showDrawAimFunctionSettings = list.FindPropertyRelative ("showDrawAimFunctionSettings").boolValue;

			EditorGUILayout.BeginVertical ();

			if (showDrawAimFunctionSettings) {
				GUI.backgroundColor = Color.gray;
				buttonMessage = "Hide Draw/Aim Function Settings";
			} else {
				GUI.backgroundColor = buttonColor;
				buttonMessage = "Show Draw/Aim Function Settings";
			}
			if (GUILayout.Button (buttonMessage)) {
				showDrawAimFunctionSettings = !showDrawAimFunctionSettings;
			}
			GUI.backgroundColor = buttonColor;
			EditorGUILayout.EndVertical ();

			list.FindPropertyRelative ("showDrawAimFunctionSettings").boolValue = showDrawAimFunctionSettings;

			if (showDrawAimFunctionSettings) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Draw Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStartDrawAction"));
				if (list.FindPropertyRelative ("useStartDrawAction").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("startDrawAction"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStopDrawAction"));
				if (list.FindPropertyRelative ("useStopDrawAction").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopDrawAction"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Third Person Draw Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStartDrawActionThirdPerson"));
				if (list.FindPropertyRelative ("useStartDrawActionThirdPerson").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("startDrawActionThirdPerson"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStopDrawActionThirdPerson"));
				if (list.FindPropertyRelative ("useStopDrawActionThirdPerson").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopDrawActionThirdPerson"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("First Person Draw Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStartDrawActionFirstPerson"));
				if (list.FindPropertyRelative ("useStartDrawActionFirstPerson").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("startDrawActionFirstPerson"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStopDrawActionFirstPerson"));
				if (list.FindPropertyRelative ("useStopDrawActionFirstPerson").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopDrawActionFirstPerson"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Third Person Aim Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStartAimActionThirdPerson"));
				if (list.FindPropertyRelative ("useStartAimActionThirdPerson").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("startAimActionThirdPerson"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStopAimActionThirdPerson"));
				if (list.FindPropertyRelative ("useStopAimActionThirdPerson").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopAimActionThirdPerson"));
				}
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("First Person Aim Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStartAimActionFirstPerson"));
				if (list.FindPropertyRelative ("useStartAimActionFirstPerson").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("startAimActionFirstPerson"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStopAimActionFirstPerson"));
				if (list.FindPropertyRelative ("useStopAimActionFirstPerson").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopAimActionFirstPerson"));
				}
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Secondary Action Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryAction"));
			if (list.FindPropertyRelative ("useSecondaryAction").boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondaryAction"));
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryActionOnDownPress"));
			if (list.FindPropertyRelative ("useSecondaryActionOnDownPress").boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondaryActionOnDownPress"));
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryActionOnHoldPress"));
			if (list.FindPropertyRelative ("useSecondaryActionOnHoldPress").boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondaryActionOnHoldPress"));
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryActionOnUpPress"));
			if (list.FindPropertyRelative ("useSecondaryActionOnUpPress").boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("secondaryActionOnUpPress"));
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSecondaryActionsOnlyAimingThirdPerson"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Forward And Backward Action Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useForwardActionEvent"));
			if (list.FindPropertyRelative ("useForwardActionEvent").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("forwardActionEvent"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useBackwardActionEvent"));
			if (list.FindPropertyRelative ("useBackwardActionEvent").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("backwardActionEvent"));
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Dual Weapon Event Settings", "window");
			EditorGUILayout.PropertyField (useEventOnSetDualWeapon);
			if (useEventOnSetDualWeapon.boolValue) {
				EditorGUILayout.PropertyField (eventOnSetRightWeapon);
				EditorGUILayout.PropertyField (eventOnSetLeftWeapon);
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Single Weapon Event Settings", "window");
			EditorGUILayout.PropertyField (useEventOnSetSingleWeapon);
			if (useEventOnSetSingleWeapon.boolValue) {
				EditorGUILayout.PropertyField (eventOnSetSingleWeapon);
			}

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Remote Event Settings", "window");
			EditorGUILayout.PropertyField (useRemoteEventOnObjectsFound);
			if (useRemoteEventOnObjectsFound.boolValue) {
				EditorGUILayout.Space ();

				showSimpleList (remoteEventNameList);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useRemoteEventOnObjectsFoundOnExplosion);
			if (useRemoteEventOnObjectsFoundOnExplosion.boolValue) {
				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (remoteEventNameOnExplosion);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Reload Event Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadEvent"));
			if (list.FindPropertyRelative ("useReloadEvent").boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadEventThirdPerson"));
				if (list.FindPropertyRelative ("useReloadEventThirdPerson").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadSingleWeaponThirdPersonEvent"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadDualWeaponThirdPersonEvent"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useReloadEventFirstPerson"));
				if (list.FindPropertyRelative ("useReloadEventFirstPerson").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadSingleWeaponFirstPersonEvent"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadDualWeaponFirstPersonEvent"));
				}				
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Event On Weapon Activated/Deactivated Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnWeaponActivated"));
			if (list.FindPropertyRelative ("useEventOnWeaponActivated").boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnWeaponActivated"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnWeaponDeactivated"));
			if (list.FindPropertyRelative ("useEventOnWeaponDeactivated").boolValue) {

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnWeaponDeactivated"));
			}
			GUILayout.EndVertical ();
		}

		if (showOtherSettings.boolValue || showAllSettings.boolValue) {
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("OTHERS", style);
		
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Durability Settings", "window");
			EditorGUILayout.PropertyField (useObjectDurability);
			EditorGUILayout.PropertyField (durabilityUsedOnAttack);
			EditorGUILayout.PropertyField (mainDurabilityInfo);
		
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Scorch Settings", "window");
	
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Scorch from Decal Manager", "window");

			EditorGUILayout.PropertyField (mainDecalManagerName);

			EditorGUILayout.Space ();

			if (impactDecalList.arraySize > 0) {
				impactDecalIndex.intValue = EditorGUILayout.Popup ("Default Decal Type", impactDecalIndex.intValue, weapon.impactDecalList);

				if (impactDecalIndex.intValue < impactDecalList.arraySize) {
					impactDecalName.stringValue = weapon.impactDecalList [impactDecalIndex.intValue];
				}
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Decal Impact List")) {
				weapon.getImpactListInfo ();					
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Regular Scorch", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("scorch"));
			if (list.FindPropertyRelative ("scorch").objectReferenceValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("scorchRayCastDistance"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Auto Shoot Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("autoShootOnTag"));
			if (list.FindPropertyRelative ("autoShootOnTag").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("layerToAutoShoot"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDistanceToRaycast"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootAtLayerToo"));

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Auto Shoot Tag List", "window");
				showSimpleList (list.FindPropertyRelative ("autoShootTagList"));
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Avoid Shoot Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("avoidShootAtTag"));
			if (list.FindPropertyRelative ("avoidShootAtTag").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("layertToAvoidShoot"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("avoidShootMaxDistanceToRaycast"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("avoidShootAtLayerToo"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useLowerPositionOnAvoidShoot"));

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Avoid Shoot Tag List", "window");
				showSimpleList (list.FindPropertyRelative ("avoidShootTagList"));
				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Player Force Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyForceAtShoot"));
			if (list.FindPropertyRelative ("applyForceAtShoot").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceDirection"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceAmount"));
			}
			GUILayout.EndVertical ();


			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Upper Body Shake Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeUpperBodyWhileShooting"));
			if (list.FindPropertyRelative ("shakeUpperBodyWhileShooting").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeAmount"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeSpeed"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Dual Weapon Upper Body Shake Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeUpperBodyShootingDualWeapons"));
			if (list.FindPropertyRelative ("shakeUpperBodyShootingDualWeapons").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("dualWeaponShakeAmount"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("dualWeaponShakeSpeed"));
			}
	
			GUILayout.EndVertical ();
		
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Shoot Noise Settings", "window");
			EditorGUILayout.PropertyField (mainNoiseMeshManagerName);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoise"));
			if (list.FindPropertyRelative ("useNoise").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseRadius"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseExpandSpeed"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoiseDetection"));
				if (list.FindPropertyRelative ("useNoiseDetection").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseDetectionLayer"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("showNoiseDetectionGizmo"));
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseDecibels"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoiseMesh"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceNoiseDetection"));

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("noiseID"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Mark Targets Settings", "window");
			EditorGUILayout.PropertyField (canMarkTargets);
			if (canMarkTargets.boolValue) {
				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Tag List To Mark Targets", "window");
				showSimpleList (tagListToMarkTargets);
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (markTargetName);
				EditorGUILayout.PropertyField (maxDistanceToMarkTargets);
				EditorGUILayout.PropertyField (markTargetsLayer);

				EditorGUILayout.PropertyField (canMarkTargetsOnFirstPerson);
				EditorGUILayout.PropertyField (canMarkTargetsOnThirdPerson);
				EditorGUILayout.PropertyField (aimOnFirstPersonToMarkTarget);
				EditorGUILayout.PropertyField (useMarkTargetSound);
				if (useMarkTargetSound.boolValue) {
					EditorGUILayout.PropertyField (markTargetSound);
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Enable/Disable Abilities on Weapon Settings", "window");
			EditorGUILayout.PropertyField (useAbilitiesListToEnableOnWeapon);
			if (useAbilitiesListToEnableOnWeapon.boolValue) {
				showSimpleList (abilitiesListToEnableOnWeapon);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (useAbilitiesListToDisableOnWeapon);
			if (useAbilitiesListToDisableOnWeapon.boolValue) {
				showSimpleList (abilitiesListToDisableOnWeapon);
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

	void showTagToLocateList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Tags: \t" + list.arraySize);

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
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
			}
			GUILayout.EndVertical ();
		}       
	}
}
#endif