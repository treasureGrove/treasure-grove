using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plasmaCutterProjectile : projectileSystem
{
	[Header ("Main Settings")]
	[Space]

	public Material defaultSliceMaterial;

	public float forceToApplyToCutPart;
	public ForceMode forceMode;
	public float forceRadius;

	public bool cutMultipleTimesActive;

	public bool useCutLimit;
	public int cutLimit;

	[Space]
	[Header ("Shatter Object Settings")]
	[Space]

	public bool shatterObjectActive;
	public int shatterAmount;

	public bool applyForceOnShatter;

	public bool addSliceComponentToPieces = true;

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
	[Header ("Other Settings")]
	[Space]

	public float minDelayToSliceSameObject = 0.01f;

	public Transform cutPositionTransform;

	public Vector3 cutOverlapBoxSize = new Vector3 (5, 0.1f, 5);

	public bool addRigidbodyToBothSlicedObjects;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<GameObject> objectsDetected = new List<GameObject> ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.red;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform cutDirectionTransform;
	public Transform planeDefiner1;
	public Transform planeDefiner2;
	public Transform planeDefiner3;

	surfaceToSlice currentSurfaceToSlice;

	int currentNumberOfCuts;

	List<GameObject> objectsToShatter;
	bool objectShattered = false;

	public void checkObjectDetected (Collider col)
	{
		if (canActivateEffect (col)) {

			objectShattered = false;

			if (currentProjectileInfo.impactSoundEffect != null) {
				AudioSource currentAudioSource = GetComponent<AudioSource> ();
					
				if (currentAudioSource != null) {
					currentAudioSource.PlayOneShot (currentProjectileInfo.impactSoundEffect);
				}
			}

			Collider objectCollider = col.GetComponent<Collider> ();

			objectToDamage = objectCollider.gameObject;

			processObject (objectToDamage, objectCollider, cutPositionTransform.position);

			if (!cutMultipleTimesActive || currentNumberOfCuts >= cutLimit) {
				projectileUsed = true;

				mainRigidbody.isKinematic = true;

				projectilePaused = true;

				destroyProjectile ();
			}
		}
	}

	public void processObject (GameObject objectToCheck, Collider objectCollider, Vector3 slicePosition)
	{
		if (cutMultipleTimesActive) {
			if (objectsDetected.Contains (objectToCheck)) {
				return;
			}
		}

		currentSurfaceToSlice = objectToCheck.GetComponent<surfaceToSlice> ();

		if (currentSurfaceToSlice == null) {
			currentSurfaceToSlice = sliceSystemUtils.getSurfaceToSlice (objectToCheck);
		}

		bool objectIsSliceSurfaceDisabled = false;

		bool objectCanBeSliced = false;

		if (currentSurfaceToSlice != null) {
			bool isCutSurfaceEnabled = currentSurfaceToSlice.isCutSurfaceEnabled ();

			if (isCutSurfaceEnabled && currentSurfaceToSlice.sliceCanBeActivated (minDelayToSliceSameObject)) {
				Material crossSectionMaterial = defaultSliceMaterial;

				if (currentSurfaceToSlice.crossSectionMaterial != null) {
					crossSectionMaterial = currentSurfaceToSlice.crossSectionMaterial;
				}

				sliceCurrentObject (objectToCheck, objectCollider, crossSectionMaterial, slicePosition);

				objectCanBeSliced = true;
			}

			if (!isCutSurfaceEnabled) {
				objectIsSliceSurfaceDisabled = true;
			}
		} 

		if (!objectCanBeSliced) {
			if (activateDamageOnSlice) {
				Vector3 damagePosition = cutPositionTransform.position;

				if (applyDamage.checkIfDead (objectToCheck)) {
					damagePosition = objectToCheck.transform.position;
				}
					
				applyDamage.checkCanBeDamaged (gameObject, objectToCheck, damageAmountToApplyOnSlice, -cutPositionTransform.forward, damagePosition, 
					currentProjectileInfo.owner, true, true, ignoreShieldOnDamage, false, true, canActivateReactionSystemTemporally,
					damageReactionID, damageTypeID);
			}

			if (applyForcesOnObjectsDetected && !objectIsSliceSurfaceDisabled) {
				if (!checkObjectLayerAndTagToApplyForces ||
				    ((1 << objectToCheck.layer & targetToApplyForceLayer.value) == 1 << objectToCheck.layer && tagetToApplyForceTagList.Contains (objectToCheck.tag))) { 
					checkForceToApplyOnObject (objectToCheck);
				}
			}
		}
	}

	public void sliceCurrentObject (GameObject objectToCheck, Collider objectCollider, Material crossSectionMaterial, Vector3 slicePosition)
	{
		if (useCutLimit) {
			currentNumberOfCuts++;
		}

		// slice the provided object using the transforms of this object
		if (currentSurfaceToSlice.isObjectCharacter ()) {
			//			print ("character found " + objectToCheck.name + " is dead " + applyDamage.checkIfDead (objectToCheck));

			if (applyDamage.getCharacterOrVehicle (objectToCheck) != null && !applyDamage.checkIfDead (objectToCheck)) {
				processCharacter (objectToCheck);

				return;
			}

			currentSurfaceToSlice.getMainSimpleSliceSystem ().activateSlice (objectCollider, positionInWorldSpace, normalInWorldSpace, slicePosition);

			currentSurfaceToSlice.checkEventOnCut ();

			currentSurfaceToSlice.checkTimeBulletOnCut ();
		} else {
			if (shatterObjectActive) {
				shatterObject (objectToCheck, crossSectionMaterial);

				return;
			}
			
			// slice the provided object using the transforms of this object
			bool objectSliced = false;

			GameObject object1 = null;
			GameObject object2 = null;

			// slice the provided objectToCheck	ect using the transforms of this object
			sliceSystemUtils.sliceObject (transform.position, objectToCheck, cutDirectionTransform.up, crossSectionMaterial, ref objectSliced, ref object1, ref object2);

			Vector3 objectPosition = objectToCheck.transform.position;
			Quaternion objectRotation = objectToCheck.transform.rotation;

			Transform objectParent = objectToCheck.transform.parent;

			if (objectSliced) {
				currentSurfaceToSlice.checkEventOnCut ();

				currentSurfaceToSlice.checkTimeBulletOnCut ();

				Rigidbody mainObject = objectToCheck.GetComponent<Rigidbody> ();
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

				float currentForceToApply = forceToApplyToCutPart;

				if (mainObjectHasRigidbody) {
				
					if (currentSurfaceToSlice.useCustomForceAmount) {
						currentForceToApply = currentSurfaceToSlice.customForceAmount;
					}

					Rigidbody object1Rigidbody = object1.AddComponent<Rigidbody> ();

					Rigidbody object2Rigidbody = object2.AddComponent<Rigidbody> ();

					object2Rigidbody.AddExplosionForce (currentForceToApply, transform.position, 10, 1, forceMode);

					object1Rigidbody.AddExplosionForce (currentForceToApply, transform.position, 10, 1, forceMode);
				} else {
					if (currentSurfaceToSlice.useCustomForceAmount) {
						currentForceToApply = currentSurfaceToSlice.customForceAmount;
					}

					bool addRigidbodyToObject1 = false;
					bool addRigidbodyToObject2 = false;

					if (addRigidbodyToBothSlicedObjects) {
						addRigidbodyToObject1 = true;
						addRigidbodyToObject2 = true;
					} else {
						float distance1 = GKC_Utils.distance (objectToCheck.transform.position, object1.transform.position);
						float distance2 = GKC_Utils.distance (objectToCheck.transform.position, object2.transform.position);

						if (distance1 < distance2) {
							addRigidbodyToObject1 = true;
						} else {
							addRigidbodyToObject2 = true;
						}
					}

					if (addRigidbodyToObject1) {
						Rigidbody object2Rigidbody = object2.AddComponent<Rigidbody> ();

						object2Rigidbody.AddExplosionForce (currentForceToApply, transform.position, 10, 1, forceMode);
					} 

					if (addRigidbodyToObject2) {
						Rigidbody object1Rigidbody = object1.AddComponent<Rigidbody> ();

						object1Rigidbody.AddExplosionForce (currentForceToApply, transform.position, 10, 1, forceMode);
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
					object1.tag = currentSurfaceToSlice.newTagOnCut;
					object1.tag = currentSurfaceToSlice.newTagOnCut;
				}

				if (cutMultipleTimesActive) {
					if (!objectsDetected.Contains (object1)) {
						objectsDetected.Add (object1);
					}

					if (!objectsDetected.Contains (object2)) {
						objectsDetected.Add (object2);
					}
				}

				objectToCheck.SetActive (false);
			}
		}
	}

	void processCharacter (GameObject currentCharacter)
	{
		StartCoroutine (processCharacterCoroutine (currentCharacter));
	}

	IEnumerator processCharacterCoroutine (GameObject currentCharacter)
	{
		applyDamage.pushCharacterWithoutForce (currentCharacter);

		List<Collider> temporalHitsList = new List<Collider> ();

		Collider[] temporalHits = Physics.OverlapBox (cutPositionTransform.position, cutOverlapBoxSize, cutDirectionTransform.rotation, currentProjectileInfo.targetToDamageLayer);

		bool bodyPartFound = false;

		if (temporalHits.Length > 0) {
			for (int i = 0; i < temporalHits.Length; i++) {
				if (!bodyPartFound) {
					Collider currentCollider = temporalHits [i];

					if (!temporalHitsList.Contains (currentCollider)) {
						if (applyDamage.isCharacter (currentCollider.gameObject)) {
							bodyPartFound = true;
						}

						if (currentCharacter != null) {
							applyDamage.killCharacter (currentCharacter);
						}

						processObject (currentCollider.gameObject, currentCollider, cutPositionTransform.position);
					}
				}
			}
		}

		yield return null;
	}

	private Vector3 positionInWorldSpace {
		get {
			return (planeDefiner1.position + planeDefiner2.position + planeDefiner3.position) / 3f;
		}
	}

	private Vector3 normalInWorldSpace {
		get {
			Vector3 t0 = planeDefiner1.position;
			Vector3 t1 = planeDefiner2.position;
			Vector3 t2 = planeDefiner3.position;

			Vector3 vectorValue;

			vectorValue.x = t0.y * (t1.z - t2.z) + t1.y * (t2.z - t0.z) + t2.y * (t0.z - t1.z);
			vectorValue.y = t0.z * (t1.x - t2.x) + t1.z * (t2.x - t0.x) + t2.z * (t0.x - t1.x);
			vectorValue.z = t0.x * (t1.y - t2.y) + t1.x * (t2.y - t0.y) + t2.x * (t0.y - t1.y);

			return vectorValue;
		}
	}

	public void checkForceToApplyOnObject (GameObject objectToDamage)
	{
		Rigidbody objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

		Vector3 forceDirection = cutDirectionTransform.forward;

		float forceAmount = addForceMultiplier;

		float forceToVehiclesMultiplier = impactForceToVehiclesMultiplier;

		if (applyImpactForceToVehicles) {
			Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);

			if (objectToDamageMainRigidbody != null) {
				Vector3 force = forceDirection * forceAmount;

				bool isVehicle = applyDamage.isVehicle (objectToDamage);

				if (isVehicle) {
					force *= forceToVehiclesMultiplier;
				}

				objectToDamageMainRigidbody.AddForce (force * objectToDamageMainRigidbody.mass, forceMode);
			}
		} else {
			if (applyDamage.canApplyForce (objectToDamage)) {
				//print (objectToDamage.name);
				Vector3 force = forceDirection * forceAmount;

				if (objectToDamageRigidbody == null) {
					objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();
				}

				objectToDamageRigidbody.AddForce (force * objectToDamageRigidbody.mass, forceMode);
			}
		}
	}

	public override void setProjectileSpecialActionActiveState (bool state)
	{
		setShatterObjectActiveState (state);
	}

	//SHATTER FUNCTIONS
	public void setShatterObjectActiveState (bool state)
	{
		shatterObjectActive = state;
	}

	public void shatterObject (GameObject objectToCheck, Material crossSectionMaterial)
	{
		if (objectShattered) {
			return;
		}

		objectsToShatter = new List<GameObject> ();

		if (objectToCheck.transform.parent != null) {
			objectToCheck.transform.SetParent (null);
		}

		objectsToShatter.Add (objectToCheck);

		for (int i = 0; i < shatterAmount; i++) {
			randomShatter (crossSectionMaterial);
		}

		objectShattered = true;
	}

	public void randomShatter (Material crossSectionMaterial)
	{
		List<GameObject> adders = new List<GameObject> ();
		List<GameObject> removals = new List<GameObject> ();

		foreach (GameObject objectToShatter in objectsToShatter) {
			GameObject[] shatters = randomShatterSingle (objectToShatter, crossSectionMaterial);

			if (shatters != null) {
				foreach (GameObject add in shatters) {
					adders.Add (add);
				}

				removals.Add (objectToShatter);
			}
		}

		foreach (GameObject rem in removals) {
			objectsToShatter.Remove (rem);
		}

		foreach (GameObject add in adders) {
			objectsToShatter.Add (add);
		}
	}

	public GameObject[] randomShatterSingle (GameObject objectToShatter, Material crossSectionMaterial)
	{
		Vector3 shatterPosition = objectToShatter.transform.position;

		GameObject[] shatters = sliceSystemUtils.shatterObject (objectToShatter, shatterPosition, crossSectionMaterial);

		if (shatters != null && shatters.Length > 0) {
			objectToShatter.SetActive (false);

			Vector3 cutPosition = objectToShatter.transform.position;

			// add rigidbodies and colliders
			foreach (GameObject shatteredObject in shatters) {
				shatteredObject.AddComponent<MeshCollider> ().convex = true;

				if (applyForceOnShatter) {
					Rigidbody currentObjectRigidbody = shatteredObject.AddComponent<Rigidbody> ();

					currentObjectRigidbody.AddExplosionForce (forceToApplyToCutPart, cutPosition, 10, 1, forceMode);
				} else {
					shatteredObject.AddComponent<Rigidbody> ();
				}

				if (addSliceComponentToPieces) {
					surfaceToSlice newSurfaceToSlice = shatteredObject.AddComponent<surfaceToSlice> ();

					newSurfaceToSlice.crossSectionMaterial = crossSectionMaterial;
				}
			}
		}

		return shatters;
	}

	public override void resetProjectile ()
	{
		base.resetProjectile ();

		currentNumberOfCuts = 0;

		objectsDetected.Clear ();
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
}