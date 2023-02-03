using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(projectileSystem))]
public class projectileSystemEditor : Editor
{
	SerializedProperty armorSurfaceLayer;
	SerializedProperty abilityFunctionNameAtStart;
	SerializedProperty abilityFunctionName;
	SerializedProperty useCustomValues;
	SerializedProperty fakeProjectileTrail;
	SerializedProperty projectileDamage;
	SerializedProperty projectileSpeed;
	SerializedProperty killInOneShot;
	SerializedProperty projectileWithAbility;
	SerializedProperty isHommingProjectile;

	SerializedProperty isSeeker;
	SerializedProperty targetOnScreenForSeeker;

	SerializedProperty waitTimeToSearchTarget;
	SerializedProperty impactForceApplied;
	SerializedProperty forceMode;
	SerializedProperty applyImpactForceToVehicles;
	SerializedProperty isExplosive;
	SerializedProperty isImplosive;
	SerializedProperty explosionForce;
	SerializedProperty explosionRadius;
	SerializedProperty useDisableTimer;
	SerializedProperty noImpactDisableTimer;
	SerializedProperty impactDisableTimer;
	SerializedProperty impactSoundEffect;
	SerializedProperty impactParticles;
	SerializedProperty scorch;
	SerializedProperty scorchRayCastDistance;
	SerializedProperty targetForScorchLayer;
	SerializedProperty useEventOnImpact;
	SerializedProperty eventOnImpact;
	SerializedProperty useLayerMaskImpact;
	SerializedProperty layerMaskImpact;
	SerializedProperty sendObjectDetectedOnImpactEvent;
	SerializedProperty objectDetectedOnImpactEvent;
	SerializedProperty useEventOnExplosion;
	SerializedProperty evenOnExplosion;

	SerializedProperty mainTrailRenderer;
	SerializedProperty mainCollider;
	SerializedProperty secondaryBulletMeshCollider;

	SerializedProperty mainMapObjectInformation;
	SerializedProperty audioClipBipManager;
	SerializedProperty mainRigidbody;

	SerializedProperty bulletMesh;
	SerializedProperty secondaryBulletMesh;

	SerializedProperty mainProjectileImpactSystem;

	SerializedProperty setContinuousSpeculativeIngame;

	SerializedProperty useEventOnAdhereToSurface;
	SerializedProperty eventOnAdhereToSurface;

	SerializedProperty useEventOnProjectileFiredExternally;
	SerializedProperty eventOnProjectileFiredExternally;

	projectileSystem manager;

	void OnEnable ()
	{
		armorSurfaceLayer = serializedObject.FindProperty ("armorSurfaceLayer");
		abilityFunctionNameAtStart = serializedObject.FindProperty ("abilityFunctionNameAtStart");
		abilityFunctionName = serializedObject.FindProperty ("abilityFunctionName");
		useCustomValues = serializedObject.FindProperty ("useCustomValues");
		fakeProjectileTrail = serializedObject.FindProperty ("fakeProjectileTrail");
		projectileDamage = serializedObject.FindProperty ("currentProjectileInfo.projectileDamage");
		projectileSpeed = serializedObject.FindProperty ("currentProjectileInfo.projectileSpeed");
		killInOneShot = serializedObject.FindProperty ("currentProjectileInfo.killInOneShot");
		projectileWithAbility = serializedObject.FindProperty ("currentProjectileInfo.projectileWithAbility");
		isHommingProjectile = serializedObject.FindProperty ("currentProjectileInfo.isHommingProjectile");

		isSeeker = serializedObject.FindProperty ("currentProjectileInfo.isSeeker");
		targetOnScreenForSeeker = serializedObject.FindProperty ("currentProjectileInfo.targetOnScreenForSeeker");

		waitTimeToSearchTarget = serializedObject.FindProperty ("currentProjectileInfo.waitTimeToSearchTarget");
		impactForceApplied = serializedObject.FindProperty ("currentProjectileInfo.impactForceApplied");
		forceMode = serializedObject.FindProperty ("currentProjectileInfo.forceMode");
		applyImpactForceToVehicles = serializedObject.FindProperty ("currentProjectileInfo.applyImpactForceToVehicles");
		isExplosive = serializedObject.FindProperty ("currentProjectileInfo.isExplosive");
		isImplosive = serializedObject.FindProperty ("currentProjectileInfo.isImplosive");
		explosionForce = serializedObject.FindProperty ("currentProjectileInfo.explosionForce");
		explosionRadius = serializedObject.FindProperty ("currentProjectileInfo.explosionRadius");
		useDisableTimer = serializedObject.FindProperty ("currentProjectileInfo.useDisableTimer");
		noImpactDisableTimer = serializedObject.FindProperty ("currentProjectileInfo.noImpactDisableTimer");
		impactDisableTimer = serializedObject.FindProperty ("currentProjectileInfo.impactDisableTimer");
		impactSoundEffect = serializedObject.FindProperty ("currentProjectileInfo.impactSoundEffect");
		impactParticles = serializedObject.FindProperty ("currentProjectileInfo.impactParticles");
		scorch = serializedObject.FindProperty ("currentProjectileInfo.scorch");
		scorchRayCastDistance = serializedObject.FindProperty ("currentProjectileInfo.scorchRayCastDistance");
		targetForScorchLayer = serializedObject.FindProperty ("currentProjectileInfo.targetForScorchLayer");
		useEventOnImpact = serializedObject.FindProperty ("useEventOnImpact");
		eventOnImpact = serializedObject.FindProperty ("eventOnImpact");
		useLayerMaskImpact = serializedObject.FindProperty ("useLayerMaskImpact");
		layerMaskImpact = serializedObject.FindProperty ("layerMaskImpact");
		sendObjectDetectedOnImpactEvent = serializedObject.FindProperty ("sendObjectDetectedOnImpactEvent");
		objectDetectedOnImpactEvent = serializedObject.FindProperty ("objectDetectedOnImpactEvent");
		useEventOnExplosion = serializedObject.FindProperty ("useEventOnExplosion");
		evenOnExplosion = serializedObject.FindProperty ("evenOnExplosion");

		mainTrailRenderer = serializedObject.FindProperty ("mainTrailRenderer");
		mainCollider = serializedObject.FindProperty ("mainCollider");
		secondaryBulletMeshCollider = serializedObject.FindProperty ("secondaryBulletMeshCollider");

		mainMapObjectInformation = serializedObject.FindProperty ("mainMapObjectInformation");
		audioClipBipManager = serializedObject.FindProperty ("audioClipBipManager");
		mainRigidbody = serializedObject.FindProperty ("mainRigidbody");

		bulletMesh = serializedObject.FindProperty ("bulletMesh");
		secondaryBulletMesh = serializedObject.FindProperty ("secondaryBulletMesh");

		mainProjectileImpactSystem = serializedObject.FindProperty ("mainProjectileImpactSystem");

		setContinuousSpeculativeIngame = serializedObject.FindProperty ("setContinuousSpeculativeIngame");

		useEventOnAdhereToSurface = serializedObject.FindProperty ("useEventOnAdhereToSurface");
		eventOnAdhereToSurface = serializedObject.FindProperty ("eventOnAdhereToSurface");

		useEventOnProjectileFiredExternally = serializedObject.FindProperty ("useEventOnProjectileFiredExternally");
		eventOnProjectileFiredExternally = serializedObject.FindProperty ("eventOnProjectileFiredExternally");

		manager = (projectileSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (setContinuousSpeculativeIngame);
		EditorGUILayout.PropertyField (armorSurfaceLayer);
		EditorGUILayout.PropertyField (abilityFunctionNameAtStart);
		EditorGUILayout.PropertyField (abilityFunctionName);
		EditorGUILayout.PropertyField (useCustomValues);
		EditorGUILayout.PropertyField (fakeProjectileTrail);
		GUILayout.EndVertical ();

		if (useCustomValues.boolValue) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Projectile Settings", "window");

			EditorGUILayout.PropertyField (projectileDamage);
			EditorGUILayout.PropertyField (projectileSpeed);
			EditorGUILayout.PropertyField (killInOneShot);
			EditorGUILayout.PropertyField (projectileWithAbility);

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Search Target Settings", "window");
			EditorGUILayout.PropertyField (isHommingProjectile);
			EditorGUILayout.PropertyField (isSeeker);
			if (isHommingProjectile.boolValue || isSeeker.boolValue) {
				EditorGUILayout.PropertyField (waitTimeToSearchTarget);
				EditorGUILayout.PropertyField (targetOnScreenForSeeker);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Force Settings", "window");
			EditorGUILayout.PropertyField (impactForceApplied);
			EditorGUILayout.PropertyField (forceMode);
			EditorGUILayout.PropertyField (applyImpactForceToVehicles);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Explosion Settings", "window");
			EditorGUILayout.PropertyField (isExplosive);
			EditorGUILayout.PropertyField (isImplosive);
			if (isExplosive.boolValue || isImplosive.boolValue) {
				EditorGUILayout.PropertyField (explosionForce);
				EditorGUILayout.PropertyField (explosionRadius);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Disable Projectile Settings", "window");
			EditorGUILayout.PropertyField (useDisableTimer);
			if (useDisableTimer.boolValue) {
				EditorGUILayout.PropertyField (noImpactDisableTimer);
				EditorGUILayout.PropertyField (impactDisableTimer);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Sound Settings", "window");
			EditorGUILayout.PropertyField (impactSoundEffect);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Particle Settings", "window");
			EditorGUILayout.PropertyField (impactParticles);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Scorch Settings", "window");
			//EditorGUILayout.Space ();
//		GUILayout.BeginVertical("Scorch from Decal Manager", "window",GUILayout.Height(30));
//		if (objectToUse.FindProperty ("impactDecalList").arraySize > 0) {
//			objectToUse.FindProperty ("impactDecalIndex").intValue = EditorGUILayout.Popup ("Default Decal Type", 
//				objectToUse.FindProperty ("impactDecalIndex").intValue, weapon.impactDecalList);
//			objectToUse.FindProperty ("impactDecalName").stringValue = weapon.impactDecalList [objectToUse.FindProperty ("impactDecalIndex").intValue];
//		}
//
//		EditorGUILayout.PropertyField (list.FindProperty ("getImpactListEveryFrame"));
//		if (!list.FindProperty ("getImpactListEveryFrame").boolValue) {
//
//			EditorGUILayout.Space ();
//
//			if (GUILayout.Button ("Update Decal Impact List")) {
//				weapon.getImpactListInfo ();					
//			}
//
//			EditorGUILayout.Space ();
//
//		}
			//GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Regular Scorch", "window");
			EditorGUILayout.PropertyField (scorch);
			if (scorch.objectReferenceValue) {
				EditorGUILayout.PropertyField (scorchRayCastDistance);
			}
			EditorGUILayout.PropertyField (targetForScorchLayer);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Impact Event Settings", "window");
		EditorGUILayout.PropertyField (useEventOnImpact);
		if (useEventOnImpact.boolValue) {
			EditorGUILayout.PropertyField (eventOnImpact);
			EditorGUILayout.PropertyField (useLayerMaskImpact);
			if (useLayerMaskImpact.boolValue) {
				EditorGUILayout.PropertyField (layerMaskImpact);
			}
			EditorGUILayout.PropertyField (sendObjectDetectedOnImpactEvent);
			if (sendObjectDetectedOnImpactEvent.boolValue) {
				EditorGUILayout.PropertyField (objectDetectedOnImpactEvent);
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Explosion Event Settings", "window");
		EditorGUILayout.PropertyField (useEventOnExplosion);
		if (useEventOnExplosion.boolValue) {
			EditorGUILayout.PropertyField (evenOnExplosion);
		}
		GUILayout.EndVertical ();


		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events On Adhere To Surface Settings", "window");
		EditorGUILayout.PropertyField (useEventOnAdhereToSurface);
		if (useEventOnAdhereToSurface.boolValue) {
			EditorGUILayout.PropertyField (eventOnAdhereToSurface);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events On Projectile Fired Externally Settings", "window");
		EditorGUILayout.PropertyField (useEventOnProjectileFiredExternally);
		if (useEventOnProjectileFiredExternally.boolValue) {
			EditorGUILayout.PropertyField (eventOnProjectileFiredExternally);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Projectile Components", "window");
		EditorGUILayout.PropertyField (mainTrailRenderer);
		EditorGUILayout.PropertyField (mainCollider);
		EditorGUILayout.PropertyField (secondaryBulletMeshCollider);
		EditorGUILayout.PropertyField (mainMapObjectInformation);
		EditorGUILayout.PropertyField (audioClipBipManager);
		EditorGUILayout.PropertyField (mainRigidbody);
		EditorGUILayout.PropertyField (bulletMesh);
		EditorGUILayout.PropertyField (secondaryBulletMesh);
		EditorGUILayout.PropertyField (mainProjectileImpactSystem);
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Projectile Components")) {
			manager.getProjectileComponents (true);					
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
#endif