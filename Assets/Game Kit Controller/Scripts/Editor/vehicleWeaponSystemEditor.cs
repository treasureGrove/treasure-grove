using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(vehicleWeaponSystem))]
public class vehicleWeaponSystemEditor : Editor
{
	SerializedProperty weaponsEnabled;
	SerializedProperty vehicle;
	SerializedProperty weaponsEffectsSource;
	SerializedProperty outOfAmmo;

	SerializedProperty layer;
	SerializedProperty targetForScorchLayer;

	SerializedProperty useCustomIgnoreTags;
	SerializedProperty customTagsToIgnoreList;

	SerializedProperty weaponLookDirection;
	SerializedProperty weaponsSlotsAmount;
	SerializedProperty weaponIndexToStart;
	SerializedProperty hasBaseRotation;
	SerializedProperty minimumX;
	SerializedProperty maximumX;
	SerializedProperty minimumY;
	SerializedProperty maximumY;

	SerializedProperty mainVehicleWeaponsGameObject;

	SerializedProperty baseX;
	SerializedProperty baseY;
	SerializedProperty baseXRotationSpeed;
	SerializedProperty baseYRotationSpeed;
	SerializedProperty noInputWeaponDirection;
	SerializedProperty canRotateWeaponsWithNoCameraInput;
	SerializedProperty noCameraInputWeaponRotationSpeed;
	SerializedProperty weaponCursorMovementSpeed;
	SerializedProperty weaponCursorRaycastLayer;
	SerializedProperty useWeaponCursorScreenLimit;
	SerializedProperty weaponCursorHorizontalLimit;
	SerializedProperty weaponCursorVerticalLimit;
	SerializedProperty vehicleCameraManager;
	SerializedProperty mainRigidbody;
	SerializedProperty actionManager;
	SerializedProperty hudManager;
	SerializedProperty parable;
	SerializedProperty gravityControlManager;
	SerializedProperty minSwipeDist;
	SerializedProperty weaponsActivate;
	SerializedProperty reloading;
	SerializedProperty aimingCorrectly;
	SerializedProperty choosedWeapon;
	SerializedProperty currentWeaponName;
	SerializedProperty weapons;
	SerializedProperty impactDecalList;

	SerializedProperty mainDecalManagerName;

	SerializedProperty projectilesPoolEnabled;

	SerializedProperty maxAmountOfPoolElementsOnWeapon;

	Color buttonColor;
	vehicleWeaponSystem manager;
	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		weaponsEnabled = serializedObject.FindProperty ("weaponsEnabled");
		vehicle = serializedObject.FindProperty ("vehicle");
		weaponsEffectsSource = serializedObject.FindProperty ("weaponsEffectsSource");
		outOfAmmo = serializedObject.FindProperty ("outOfAmmo");

		layer = serializedObject.FindProperty ("layer");
		targetForScorchLayer = serializedObject.FindProperty ("targetForScorchLayer");

		useCustomIgnoreTags = serializedObject.FindProperty ("useCustomIgnoreTags");
		customTagsToIgnoreList = serializedObject.FindProperty ("customTagsToIgnoreList");

		weaponLookDirection = serializedObject.FindProperty ("weaponLookDirection");
		weaponsSlotsAmount = serializedObject.FindProperty ("weaponsSlotsAmount");
		weaponIndexToStart = serializedObject.FindProperty ("weaponIndexToStart");
		hasBaseRotation = serializedObject.FindProperty ("hasBaseRotation");
		minimumX = serializedObject.FindProperty ("minimumX");
		maximumX = serializedObject.FindProperty ("maximumX");
		minimumY = serializedObject.FindProperty ("minimumY");
		maximumY = serializedObject.FindProperty ("maximumY");

		mainVehicleWeaponsGameObject = serializedObject.FindProperty ("mainVehicleWeaponsGameObject");
		baseX = serializedObject.FindProperty ("baseX");
		baseY = serializedObject.FindProperty ("baseY");
		baseXRotationSpeed = serializedObject.FindProperty ("baseXRotationSpeed");
		baseYRotationSpeed = serializedObject.FindProperty ("baseYRotationSpeed");
		noInputWeaponDirection = serializedObject.FindProperty ("noInputWeaponDirection");
		canRotateWeaponsWithNoCameraInput = serializedObject.FindProperty ("canRotateWeaponsWithNoCameraInput");
		noCameraInputWeaponRotationSpeed = serializedObject.FindProperty ("noCameraInputWeaponRotationSpeed");
		weaponCursorMovementSpeed = serializedObject.FindProperty ("weaponCursorMovementSpeed");
		weaponCursorRaycastLayer = serializedObject.FindProperty ("weaponCursorRaycastLayer");
		useWeaponCursorScreenLimit = serializedObject.FindProperty ("useWeaponCursorScreenLimit");
		weaponCursorHorizontalLimit = serializedObject.FindProperty ("weaponCursorHorizontalLimit");
		weaponCursorVerticalLimit = serializedObject.FindProperty ("weaponCursorVerticalLimit");
		vehicleCameraManager = serializedObject.FindProperty ("vehicleCameraManager");
		mainRigidbody = serializedObject.FindProperty ("mainRigidbody");
		actionManager = serializedObject.FindProperty ("actionManager");
		hudManager = serializedObject.FindProperty ("hudManager");
		parable = serializedObject.FindProperty ("parable");
		gravityControlManager = serializedObject.FindProperty ("gravityControlManager");
		minSwipeDist = serializedObject.FindProperty ("minSwipeDist");
		weaponsActivate = serializedObject.FindProperty ("weaponsActivate");
		reloading = serializedObject.FindProperty ("reloading");
		aimingCorrectly = serializedObject.FindProperty ("aimingCorrectly");
		choosedWeapon = serializedObject.FindProperty ("choosedWeapon");
		currentWeaponName = serializedObject.FindProperty ("currentWeaponName");
		weapons = serializedObject.FindProperty ("weapons");
		impactDecalList = serializedObject.FindProperty ("impactDecalList");

		mainDecalManagerName = serializedObject.FindProperty ("mainDecalManagerName");

		projectilesPoolEnabled = serializedObject.FindProperty ("projectilesPoolEnabled");

		maxAmountOfPoolElementsOnWeapon = serializedObject.FindProperty ("maxAmountOfPoolElementsOnWeapon");

		manager = (vehicleWeaponSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Basic Settings", "window");
		EditorGUILayout.PropertyField (weaponsEnabled);
		EditorGUILayout.PropertyField (vehicle);
		EditorGUILayout.PropertyField (weaponsEffectsSource);
		EditorGUILayout.PropertyField (outOfAmmo);
		EditorGUILayout.PropertyField (weaponLookDirection);
		EditorGUILayout.PropertyField (weaponsSlotsAmount);
		EditorGUILayout.PropertyField (weaponIndexToStart);	
		EditorGUILayout.PropertyField (projectilesPoolEnabled);
		EditorGUILayout.PropertyField (maxAmountOfPoolElementsOnWeapon);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Target Detection Settings", "window");
		EditorGUILayout.PropertyField (layer);
		EditorGUILayout.PropertyField (targetForScorchLayer);

		EditorGUILayout.PropertyField (useCustomIgnoreTags);
		if (useCustomIgnoreTags.boolValue) {
			showLowerList (customTagsToIgnoreList);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Base Rotation Settings", "window");
		EditorGUILayout.PropertyField (hasBaseRotation);
		if (hasBaseRotation.boolValue) {
			EditorGUILayout.PropertyField (minimumX);
			EditorGUILayout.PropertyField (maximumX);
			EditorGUILayout.PropertyField (minimumY);
			EditorGUILayout.PropertyField (maximumY);

			EditorGUILayout.PropertyField (mainVehicleWeaponsGameObject);
			EditorGUILayout.PropertyField (baseX);
			EditorGUILayout.PropertyField (baseY);
			EditorGUILayout.PropertyField (baseXRotationSpeed);
			EditorGUILayout.PropertyField (baseYRotationSpeed);

			EditorGUILayout.PropertyField (noInputWeaponDirection);
			EditorGUILayout.PropertyField (canRotateWeaponsWithNoCameraInput);
			if (canRotateWeaponsWithNoCameraInput.boolValue) {
				EditorGUILayout.PropertyField (noCameraInputWeaponRotationSpeed);
				EditorGUILayout.PropertyField (weaponCursorMovementSpeed);

				EditorGUILayout.PropertyField (weaponCursorRaycastLayer);

				EditorGUILayout.PropertyField (useWeaponCursorScreenLimit);
				if (useWeaponCursorScreenLimit.boolValue) {
					EditorGUILayout.PropertyField (weaponCursorHorizontalLimit);
					EditorGUILayout.PropertyField (weaponCursorVerticalLimit);
				}
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vehicle Elements", "window");
		EditorGUILayout.PropertyField (vehicleCameraManager);
		EditorGUILayout.PropertyField (mainRigidbody);
		EditorGUILayout.PropertyField (actionManager);
		EditorGUILayout.PropertyField (hudManager);
		EditorGUILayout.PropertyField (parable);
		EditorGUILayout.PropertyField (gravityControlManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Touch Options", "window");
		EditorGUILayout.PropertyField (minSwipeDist);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("State Info", "window");
		GUILayout.Label ("Weapons Activate\t\t" + weaponsActivate.boolValue);
		GUILayout.Label ("Reloading\t\t\t" + reloading.boolValue);
		GUILayout.Label ("Aiming Correctly\t\t" + aimingCorrectly.boolValue);
		GUILayout.Label ("Current Weapon Index\t" + choosedWeapon.intValue);
		GUILayout.Label ("Current Weapon Name\t" + currentWeaponName.stringValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapons List", "window");
		showWeaponList (weapons);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (mainDecalManagerName);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showWeaponListElement (SerializedProperty list, int weaponIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Info", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("numberKey"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enabled"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Custom Reticle Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReticle"));
		if (list.FindPropertyRelative ("useCustomReticle").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("customReticle"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReticleColor"));
			if (list.FindPropertyRelative ("useCustomReticleColor").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("customReticleColor"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomReticleSize"));
			if (list.FindPropertyRelative ("useCustomReticleSize").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("customReticleSize"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Cursor Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableWeaponCursorWhileNotShooting"));
		if (list.FindPropertyRelative ("disableWeaponCursorWhileNotShooting").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToDisableWeaponCursor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Fire Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("automatic"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("fireRate"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadTime"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootAProjectile"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("launchProjectile"));
		if (list.FindPropertyRelative ("shootAProjectile").boolValue || list.FindPropertyRelative ("launchProjectile").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileToShoot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectilesPerShoot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileDamage"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("killInOneShot"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileWithAbility"));

			if (list.FindPropertyRelative ("launchProjectile").boolValue) {

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("Launch Projectile Settings", "window");
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateLaunchParable"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useParableSpeed"));
				if (!list.FindPropertyRelative ("useParableSpeed").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileSpeed"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("parableDirectionTransform"));
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("parableTrayectoryPosition"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMaxDistanceWhenNoSurfaceFound"));
					if (list.FindPropertyRelative ("useMaxDistanceWhenNoSurfaceFound").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDistanceWhenNoSurfaceFound"));
					}
				}
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileModel"));
				GUILayout.EndVertical ();

				EditorGUILayout.Space ();

			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("fireWeaponForward"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRayCastShoot"));
				if (!list.FindPropertyRelative ("useRayCastShoot").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileSpeed"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastCheckingOnRigidbody"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("customRaycastCheckingRate"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("customRaycastCheckingDistance"));
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastAllToCheckSurfaceFound"));
					if (list.FindPropertyRelative ("useRaycastAllToCheckSurfaceFound").boolValue) {
						EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDistanceToRaycastAll"));
					}

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
				}
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Search Target Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isHommingProjectile"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isSeeker"));
			if (list.FindPropertyRelative ("isHommingProjectile").boolValue || list.FindPropertyRelative ("isSeeker").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("waitTimeToSearchTarget"));
				showLowerList (list.FindPropertyRelative ("tagToLocate"));
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

			GUILayout.BeginVertical ("Particle Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("muzzleParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("projectileParticles"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactParticles"));

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sound Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootSoundEffect"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactSoundEffect"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("outOfAmmo"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadSoundEffect"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Ammo Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("clipSize"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmmo"));
		if (!list.FindPropertyRelative ("infiniteAmmo").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remainAmmo"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAmmoLimit"));
		if (list.FindPropertyRelative ("useAmmoLimit").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("ammoLimit"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Laser Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isLaser"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Spread Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useProjectileSpread"));
		if (list.FindPropertyRelative ("useProjectileSpread").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("spreadAmount"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Position Settings", "window");
		showLowerList (list.FindPropertyRelative ("projectilePosition"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Shell Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ejectShellOnShot"));
		if (list.FindPropertyRelative ("ejectShellOnShot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shell"));
			if (list.FindPropertyRelative ("shell").objectReferenceValue) {
				showLowerList (list.FindPropertyRelative ("shellPosition"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("shellEjectionForce"));
				showLowerList (list.FindPropertyRelative ("shellDropSoundList"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Components", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("weapon"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("animation"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scorch Settings", "window");

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Scorch from Decal Manager", "window");
		if (impactDecalList.arraySize > 0) {
			list.FindPropertyRelative ("impactDecalIndex").intValue = EditorGUILayout.Popup ("Default Decal Type", 
				list.FindPropertyRelative ("impactDecalIndex").intValue, manager.impactDecalList);
			list.FindPropertyRelative ("impactDecalName").stringValue = manager.impactDecalList [list.FindPropertyRelative ("impactDecalIndex").intValue];
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Decal Impact List")) {
			manager.getImpactListInfo ();					
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
			showLowerList (list.FindPropertyRelative ("autoShootTagList"));
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Force Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("applyForceAtShoot"));
		if (list.FindPropertyRelative ("applyForceAtShoot").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceDirection"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceAmount"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomTransformToForceShoot"));
			if (list.FindPropertyRelative ("useCustomTransformToForceShoot").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("customTransformToForceShoot"));
			}
		}
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

		GUILayout.BeginVertical ("Muzzle Flash Light Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMuzzleFlash"));
		if (list.FindPropertyRelative ("useMuzzleFlash").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("muzzleFlahsLight"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("muzzleFlahsDuration"));
		}
		GUILayout.EndVertical ();

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

		GUILayout.BeginVertical ("Remote Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEventOnObjectsFound"));
		if (list.FindPropertyRelative ("useRemoteEventOnObjectsFound").boolValue) {

			EditorGUILayout.Space ();

			showLowerList (list.FindPropertyRelative ("remoteEventNameList"));

		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEventOnObjectsFoundOnExplosion"));
		if (list.FindPropertyRelative ("useRemoteEventOnObjectsFoundOnExplosion").boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remoteEventNameOnExplosion"));
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

		EditorGUILayout.Space ();

		bool shakeSettings = list.FindPropertyRelative ("showShakeSettings").boolValue;
		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		string buttonText = "";
		if (shakeSettings) {
			GUI.backgroundColor = Color.gray;
			buttonText = "Hide Shake Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			buttonText = "Show Shake Settings";
		}
		if (GUILayout.Button (buttonText)) {
			shakeSettings = !shakeSettings;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndVertical ();
		list.FindPropertyRelative ("showShakeSettings").boolValue = shakeSettings;

		if (shakeSettings) {
			
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Shake Settings when this weapon fires", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Shake Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootShakeInfo.useDamageShake"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootShakeInfo.sameValueBothViews"));
			if (list.FindPropertyRelative ("shootShakeInfo.useDamageShake").boolValue) {

				EditorGUILayout.Space ();

				if (list.FindPropertyRelative ("shootShakeInfo.sameValueBothViews").boolValue) {
					showShakeInfo (list.FindPropertyRelative ("shootShakeInfo.thirdPersonDamageShake"), "Shake In Third Person");
				} else {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootShakeInfo.useDamageShakeInThirdPerson"));
					if (list.FindPropertyRelative ("shootShakeInfo.useDamageShakeInThirdPerson").boolValue) {
						showShakeInfo (list.FindPropertyRelative ("shootShakeInfo.thirdPersonDamageShake"), "Shake In Third Person");

						EditorGUILayout.Space ();

					}
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("shootShakeInfo.useDamageShakeInFirstPerson"));
					if (list.FindPropertyRelative ("shootShakeInfo.useDamageShakeInFirstPerson").boolValue) {
						showShakeInfo (list.FindPropertyRelative ("shootShakeInfo.firstPersonDamageShake"), "Shake In First Person");

						EditorGUILayout.Space ();

					}
				}

				EditorGUILayout.Space ();

				if (GUILayout.Button ("Test Shake")) {
					if (Application.isPlaying) {
						manager.testWeaponShake (weaponIndex);
					}
				}
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
	}

	void showWeaponList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of weapons: " + list.arraySize);

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
						showWeaponListElement (list.GetArrayElementAtIndex (i), i);
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

	void showLowerList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
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
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
					return;
				}
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showShakeInfo (SerializedProperty list, string shakeName)
	{
		GUILayout.BeginVertical (shakeName, "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotation"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSpeed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeRotationSmooth"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("shakeDuration"));
		GUILayout.EndVertical ();
	}
}
#endif