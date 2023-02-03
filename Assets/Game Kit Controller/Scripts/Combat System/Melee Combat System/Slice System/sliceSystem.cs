using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using NobleMuffins.LimbHacker.Guts;

public class sliceSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public Vector3 cutOverlapBoxSize = new Vector3 (5, 0.1f, 5);

	public bool activateRigidbodiesOnNewObjects;

	public float minDelayToSliceSameObject = 0.3f;

	[Space]
	[Header ("Objects To Affect Settings")]
	[Space]

	public LayerMask targetToDamageLayer;

	public bool useObjectsToIgnoreList;
	public List<GameObject> objectsToIgnoreList = new List<GameObject> ();

	[Space]
	[Header ("Force Settings")]
	[Space]

	public float forceToApplyToCutPart;
	public ForceMode forceMode;
	public float forceRadius;
	public float forceUp;

	[Space]
	[Header ("Damage Settings")]
	[Space]

	public bool activateDamageOnSlice;
	public float damageAmountToApplyOnSlice;
	public bool ignoreShieldOnDamage = true;
	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	public int damageTypeID = -1;

	[Space]
	[Header ("Physics Settings")]
	[Space]

	public bool applyForcesOnObjectsDetected;
	public float addForceMultiplier;
	public bool applyImpactForceToVehicles;
	public float impactForceToVehiclesMultiplier;

	public bool checkObjectLayerAndTagToApplyForces;

	public LayerMask targetToApplyForceLayer;
	public List<string> tagetToApplyForceTagList = new List<string> ();

	[Space]
	[Header ("Slice Transform Values Settings")]
	[Space]

	public List<sliceTransformValuesInfo> sliceTransformValuesInfoList = new List<sliceTransformValuesInfo> ();

	[Space]
	[Header ("Bullet Time Settings")]
	[Space]

	public bool ignoreTimeBulletOnRegularSlice;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.red;

	public bool showDebugPrint;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject playerGameObject;

	public Material defaultSliceMaterial;

	public Transform cutPositionTransform;
	public Transform cutDirectionTransform;

	public Transform planeDefiner1;
	public Transform planeDefiner2;
	public Transform planeDefiner3;

	bool usingCustomCutValues;

	surfaceToSlice currentSurfaceToSlice;

	Collider[] hits;

	List<GameObject> hitsGameObjectList = new List<GameObject> ();

	bool collidersListSent;

	Vector3 currentCutPosition;
	Quaternion currentCutRotation;
	Vector3 currentCutUp;
	Vector3 currentCutForward;

	Vector3 currentCutDirection;

	Vector3 currentPlaneDefiner1;
	Vector3 currentPlaneDefiner2;
	Vector3 currentPlaneDefiner3;

	Vector3 currentCutOverlapBoxSize;

	bool objectsDetectedOnLastSlice;

	float lastTimeSliceActivated;


	public void setCustomSliceTransformValues (string sliceInfoName)
	{
		int sliceIndex = sliceTransformValuesInfoList.FindIndex (a => a.Name == sliceInfoName);

		if (sliceIndex > -1) {

			print (sliceInfoName);

			sliceTransformValuesInfo currentSliceTransformValuesInfo = sliceTransformValuesInfoList [sliceIndex];

			if (currentSliceTransformValuesInfo.sliceInfoEnabled) {
				setCustomCutTransformValues (currentSliceTransformValuesInfo.cutOverlapBoxSize, 
					currentSliceTransformValuesInfo.cutPositionTransform,
					currentSliceTransformValuesInfo.cutDirectionTransform,
					currentSliceTransformValuesInfo.planeDefiner1, 
					currentSliceTransformValuesInfo.planeDefiner2, 
					currentSliceTransformValuesInfo.planeDefiner3);

				activateCut ();
			}
		}
	}


	public void setCustomCutTransformValues (Vector3 newCutOverlapBoxSize,
	                                         Transform newcutPositionTransform,
	                                         Transform newCutDirectionTransform,

	                                         Transform newPlaneDefiner1,
	                                         Transform newPlaneDefiner2,
	                                         Transform newPlaneDefiner3)
	{
		usingCustomCutValues = true;

		currentCutPosition = newcutPositionTransform.position;
		currentCutRotation = newcutPositionTransform.rotation;
		currentCutUp = newcutPositionTransform.up;
		currentCutForward = newcutPositionTransform.forward;

		currentCutDirection = newCutDirectionTransform.right;

		currentPlaneDefiner1 = newPlaneDefiner1.position;
		currentPlaneDefiner2 = newPlaneDefiner2.position;
		currentPlaneDefiner3 = newPlaneDefiner3.position;

		currentCutOverlapBoxSize = newCutOverlapBoxSize;
	}

	void setCurrentCutTransformValues ()
	{
		if (usingCustomCutValues) {

			usingCustomCutValues = false;

			return;
		}

		currentCutPosition = cutPositionTransform.position;
		currentCutRotation = cutPositionTransform.rotation;
		currentCutUp = cutPositionTransform.up;
		currentCutForward = cutPositionTransform.forward;

		currentCutDirection = cutDirectionTransform.right;

		currentPlaneDefiner1 = planeDefiner1.position;
		currentPlaneDefiner2 = planeDefiner2.position;
		currentPlaneDefiner3 = planeDefiner3.position;

		currentCutOverlapBoxSize = cutOverlapBoxSize;
	}

	bool currentSliceActivatedExternally;

	public void activateCutExternally ()
	{
		currentSliceActivatedExternally = true;

		activateCut ();
	}

	public void activateCut ()
	{
		if (lastTimeSliceActivated > 0) {
			if (Time.time > lastTimeSliceActivated + 0.7f) {
				currentSliceActivatedExternally = false;

				lastTimeSliceActivated = 0;
			}
		}

		objectsDetectedOnLastSlice = false;

		setCurrentCutTransformValues ();

		hits = Physics.OverlapBox (currentCutPosition, currentCutOverlapBoxSize, currentCutRotation, targetToDamageLayer);

		if (hits.Length > 0) {
			for (int i = 0; i < hits.Length; i++) {
				Collider currentCollider = hits [i];

				processObject (currentCollider.gameObject, currentCollider, hits [i].ClosestPointOnBounds (currentCutPosition));
			}
		}

		collidersListSent = false;

		lastTimeSliceActivated = Time.time;
	}

	public List<GameObject> getLastCollidersListDetected ()
	{
		hitsGameObjectList.Clear ();

		if (!collidersListSent) {
			if (hits.Length > 0) {
				for (int i = 0; i < hits.Length; i++) {
					hitsGameObjectList.Add (hits [i].gameObject);
				}
			}
		}

		collidersListSent = true;

		return hitsGameObjectList;
	}

	public void processObject (GameObject obj, Collider objectCollider, Vector3 slicePosition)
	{
		if (useObjectsToIgnoreList) {
			if (objectsToIgnoreList.Contains (obj)) {
				return;
			}
		}
			
		currentSurfaceToSlice = obj.GetComponent<surfaceToSlice> ();

		if (currentSurfaceToSlice == null) {
			currentSurfaceToSlice = sliceSystemUtils.getSurfaceToSlice (obj);
		}

		bool objectIsSliceSurfaceDisabled = false;

		bool objectCanBeSliced = false;

		if (showDebugPrint) {
			print ("processing object " + obj.name);
		}

		if (currentSurfaceToSlice != null) {
			bool isCutSurfaceEnabled = currentSurfaceToSlice.isCutSurfaceEnabled ();

			if (showDebugPrint) {
				print ("Surface cut enabled state " + isCutSurfaceEnabled +
				" can activate slice " + currentSurfaceToSlice.sliceCanBeActivated (minDelayToSliceSameObject));
			}

			if (isCutSurfaceEnabled && currentSurfaceToSlice.sliceCanBeActivated (minDelayToSliceSameObject)) {

				Material crossSectionMaterial = defaultSliceMaterial;

				if (currentSurfaceToSlice.crossSectionMaterial != null) {
					crossSectionMaterial = currentSurfaceToSlice.crossSectionMaterial;
				}

				sliceCurrentObject (obj, objectCollider, crossSectionMaterial, slicePosition);

				objectsDetectedOnLastSlice = true;

				objectCanBeSliced = true;
			}

			if (!isCutSurfaceEnabled) {
				objectIsSliceSurfaceDisabled = true;
			}
		} else {
			if (showDebugPrint) {
				print ("Surface to slice not found on " + obj.name);
			}
		}

		if (!objectCanBeSliced) {
			if (activateDamageOnSlice) {
				Vector3 damagePosition = currentCutPosition;

				if (applyDamage.checkIfDead (obj)) {
					damagePosition = obj.transform.position;
				}
					
				applyDamage.checkCanBeDamaged (gameObject, obj, damageAmountToApplyOnSlice, -currentCutForward, damagePosition, 
					playerGameObject, true, true, ignoreShieldOnDamage, false, true, 
					canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
			}

			if (applyForcesOnObjectsDetected && !objectIsSliceSurfaceDisabled) {
				if (!checkObjectLayerAndTagToApplyForces ||
				    ((1 << obj.layer & targetToApplyForceLayer.value) == 1 << obj.layer && tagetToApplyForceTagList.Contains (obj.tag))) { 
					checkForceToApplyOnObject (obj);
				}
			}
		}
	}

	public void sliceCurrentObject (GameObject obj, Collider objectCollider, Material crossSectionMaterial, Vector3 slicePosition)
	{
		// slice the provided object using the transforms of this object
		if (currentSurfaceToSlice.isObjectCharacter ()) {

			bool isCharacterOrVehicle = applyDamage.getCharacterOrVehicle (obj) != null;

			bool objectIsDead = applyDamage.checkIfDead (obj);

			if (isCharacterOrVehicle && !objectIsDead) {
				processCharacter (obj);

				return;
			}

			if (showDebugPrint) {
				print ("SLICING " + obj.name + " is character " + isCharacterOrVehicle + " is dead" + objectIsDead);
			}

			currentSurfaceToSlice.getMainSimpleSliceSystem ().activateSlice (objectCollider, positionInWorldSpace, normalInWorldSpace, slicePosition);

			currentSurfaceToSlice.checkEventOnCut ();

			bool canCheckTimeBulletResult = true;

			if (!currentSliceActivatedExternally) {
				if (ignoreTimeBulletOnRegularSlice) {
					canCheckTimeBulletResult = false;
				}
			}

			if (canCheckTimeBulletResult) {
				currentSurfaceToSlice.checkTimeBulletOnCut ();
			}
		} else {
			bool objectSliced = false;

			GameObject object1 = null;
			GameObject object2 = null;

			sliceSystemUtils.sliceObject (currentCutPosition, obj, currentCutUp, crossSectionMaterial, ref objectSliced, ref object1, ref object2);

			Vector3 objectPosition = obj.transform.position;
			Quaternion objectRotation = obj.transform.rotation;

			Transform objectParent = obj.transform.parent;

			if (objectSliced) {
				currentSurfaceToSlice.checkEventOnCut ();

				bool canCheckTimeBulletResult = true;

				if (!currentSliceActivatedExternally) {
					if (ignoreTimeBulletOnRegularSlice) {
						canCheckTimeBulletResult = false;
					}
				}

				if (canCheckTimeBulletResult) {
					currentSurfaceToSlice.checkTimeBulletOnCut ();
				}


				Rigidbody mainObject = obj.GetComponent<Rigidbody> ();
				bool mainObjectHasRigidbody = mainObject != null;

				object1.transform.position = objectPosition;
				object1.transform.rotation = objectRotation;

				object2.transform.position = objectPosition;
				object2.transform.rotation = objectRotation;

				if (objectParent != null) {
					object1.transform.SetParent (objectParent);
					object2.transform.SetParent (objectParent);
				}

				surfaceToSlice newSurfaceToSlice1 = object1.AddComponent<surfaceToSlice> ();
				surfaceToSlice newSurfaceToSlice2 = object2.AddComponent<surfaceToSlice> ();

				currentSurfaceToSlice.copySurfaceInfo (newSurfaceToSlice1);
				currentSurfaceToSlice.copySurfaceInfo (newSurfaceToSlice2);


				float distance1 = GKC_Utils.distance (obj.transform.position, object1.transform.position);
				float distance2 = GKC_Utils.distance (obj.transform.position, object2.transform.position);

				float currentForceToApply = forceToApplyToCutPart;

				if (mainObjectHasRigidbody || activateRigidbodiesOnNewObjects) {

					if (currentSurfaceToSlice.useCustomForceAmount) {
						currentForceToApply = currentSurfaceToSlice.customForceAmount;
					}

					Rigidbody object1Rigidbody = object1.AddComponent<Rigidbody> ();

					Rigidbody object2Rigidbody = object2.AddComponent<Rigidbody> ();

					object1Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

					object2Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

					object2Rigidbody.AddExplosionForce (currentForceToApply, currentCutPosition, forceRadius, forceUp, forceMode);

					object1Rigidbody.AddExplosionForce (currentForceToApply, currentCutPosition, forceRadius, forceUp, forceMode);
				} else {
					if (currentSurfaceToSlice.useCustomForceAmount) {
						currentForceToApply = currentSurfaceToSlice.customForceAmount;
					}

					if (distance1 < distance2) {
						Rigidbody object2Rigidbody = object2.AddComponent<Rigidbody> ();

						object2Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

						object2Rigidbody.AddExplosionForce (currentForceToApply, currentCutPosition, forceRadius, forceUp, forceMode);
					} else {
						Rigidbody object1Rigidbody = object1.AddComponent<Rigidbody> ();

						object1Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

						object1Rigidbody.AddExplosionForce (currentForceToApply, currentCutPosition, forceRadius, forceUp, forceMode);
					}
				}

				if (currentSurfaceToSlice.useBoxCollider) {
					object1.AddComponent<BoxCollider> ();
					object2.AddComponent<BoxCollider> ();
				} else {
					MeshCollider object1Collider = object1.AddComponent<MeshCollider> ();
					MeshCollider object2Collider = object2.AddComponent<MeshCollider> ();

					object1Collider.convex = true;
					object2Collider.convex = true;
				}

				if (currentSurfaceToSlice.setNewLayerOnCut) {
					object1.layer = LayerMask.NameToLayer (currentSurfaceToSlice.newLayerOnCut);
					object1.layer = LayerMask.NameToLayer (currentSurfaceToSlice.newLayerOnCut);
				}

				if (currentSurfaceToSlice.setNewTagOnCut) {
					object1.tag = currentSurfaceToSlice.tag;
					object1.tag = currentSurfaceToSlice.tag;
				}

				obj.SetActive (false);
			}
		}
	}

	public void checkForceToApplyOnObject (GameObject objectToDamage)
	{
		Rigidbody objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

		Vector3 forceDirection = currentCutDirection;
	
		float forceAmount = addForceMultiplier;

		float forceToVehiclesMultiplier = impactForceToVehiclesMultiplier;

		if (applyImpactForceToVehicles) {
			Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);

			if (objectToDamageMainRigidbody) {
				Vector3 force = forceDirection * forceAmount;

				bool isVehicle = applyDamage.isVehicle (objectToDamage);
				if (isVehicle) {
					force *= forceToVehiclesMultiplier;
				}

				objectToDamageMainRigidbody.AddForce (force * objectToDamageMainRigidbody.mass, forceMode);
			}
		} else {
			if (applyDamage.canApplyForce (objectToDamage)) {
				if (showDebugPrint) {
					print (objectToDamage.name);
				}

				Vector3 force = forceDirection * forceAmount;

				if (objectToDamageRigidbody == null) {
					objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();
				}

				objectToDamageRigidbody.AddForce (force * objectToDamageRigidbody.mass, forceMode);
			}
		}
	}

	private Vector3 positionInWorldSpace {
		get {
			return (currentPlaneDefiner1 + currentPlaneDefiner2 + currentPlaneDefiner3) / 3f;

		}
	}

	private Vector3 normalInWorldSpace {
		get {
			Vector3 t0 = currentPlaneDefiner1;
			Vector3 t1 = currentPlaneDefiner2;
			Vector3 t2 = currentPlaneDefiner3;

			Vector3 vectorValue;

			vectorValue.x = t0.y * (t1.z - t2.z) + t1.y * (t2.z - t0.z) + t2.y * (t0.z - t1.z);
			vectorValue.y = t0.z * (t1.x - t2.x) + t1.z * (t2.x - t0.x) + t2.z * (t0.x - t1.x);
			vectorValue.z = t0.x * (t1.y - t2.y) + t1.x * (t2.y - t0.y) + t2.x * (t0.y - t1.y);

			return vectorValue;
		}
	}

	void processCharacter (GameObject currentCharacter)
	{
		StartCoroutine (processCharacterCoroutine (currentCharacter));
	}

	IEnumerator processCharacterCoroutine (GameObject currentCharacter)
	{
		applyDamage.pushCharacterWithoutForceAndPauseGetUp (currentCharacter);

		yield return new WaitForEndOfFrame ();

		List<Collider> temporalHitsList = new List<Collider> ();

		if (hits.Length > 0) {
			for (int i = 0; i < hits.Length; i++) {
				temporalHitsList.Add (hits [i]);
			}
		}

		Collider[] temporalHits = Physics.OverlapBox (currentCutPosition, currentCutOverlapBoxSize, currentCutRotation, targetToDamageLayer);

		bool bodyPartFound = false;

		if (showDebugPrint) {
			print ("activating ragdoll on " + currentCharacter.name);
		}

		if (showDebugPrint) {
			print ("\n\n");
		}

		if (temporalHits.Length > 0) {
			for (int i = 0; i < temporalHits.Length; i++) {
				if (!bodyPartFound) {
					Collider currentCollider = temporalHits [i];

					if (showDebugPrint) {
						print ("checking " + currentCollider.name);
					}

					if (!temporalHitsList.Contains (currentCollider)) {
						if (showDebugPrint) {
							print (currentCollider.name + " body part when killing");
						}

						if (applyDamage.isCharacter (currentCollider.gameObject)) {
							bodyPartFound = true;
						}

						if (currentCharacter != null) {
							applyDamage.killCharacter (currentCharacter);
						}

						processObject (currentCollider.gameObject, currentCollider, currentCutPosition);
					} else {
						if (showDebugPrint) {
							print (currentCollider.name + " already on temporal hist list");
						}
					}
				}
			}
		}
	}

	public bool anyObjectDetectedOnLastSlice ()
	{
		return objectsDetectedOnLastSlice;
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			GKC_Utils.drawRectangleGizmo (cutPositionTransform.position, cutPositionTransform.rotation, Vector3.zero, cutOverlapBoxSize, gizmoColor);
		}
	}


	[System.Serializable]
	public class sliceTransformValuesInfo
	{
		public string Name;
		public bool sliceInfoEnabled;

		public Vector3 cutOverlapBoxSize = new Vector3 (5, 0.1f, 5);

		public Transform cutPositionTransform;
		public Transform cutDirectionTransform;

		public Transform planeDefiner1;
		public Transform planeDefiner2;
		public Transform planeDefiner3;
	}
}
