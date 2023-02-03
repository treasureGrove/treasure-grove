using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class spearProjectile : projectileSystem
{
	[Header ("Main Settings")]
	[Space]

	public bool carryingRagdoll;
	public LayerMask layer;
	public bool surfaceFound;

	public float rayDistance = 1;
	public float carryinRagdollSpeedMultiplier;
	public float capsuleCastRadius = 2;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform raycastPositionTransform;

	ragdollActivator currentRagdoll;
	Vector3 previousVelocity;
	Transform currentRagdollRootMotion;
	bool firstSurfaceDetected;
	Vector3 surfacePoint;
	float extraDistanceRaycast = 5;
	Collider[] hitColliders;

	void FixedUpdate ()
	{
		if (carryingRagdoll && !surfaceFound) {
			if (!firstSurfaceDetected) {
//				hitColliders = Physics.OverlapSphere (currentRagdollRootMotion.position + transform.forward * extraDistanceRaycast, capsuleCastRadius, layer);
//				for (int i = 0; i < hitColliders.Length; i++) {
//					if (!hitColliders[i].GetComponent<Rigidbody> ()) {
//						if (Physics.Linecast (currentRagdollRootMotion.position, currentRagdollRootMotion.position + transform.forward * extraDistanceRaycast, out hit, layer)) {
//							if (!hit.collider.GetComponent<Rigidbody> ()) {
//								firstSurfaceDetected = true;
//								surfacePoint = hit.point;
//							}
//						} 
//					}
//				} 

				if (Physics.Linecast (currentRagdollRootMotion.position, currentRagdollRootMotion.position + transform.forward * extraDistanceRaycast, out hit, layer)) {
					if (!hit.collider.GetComponent<Rigidbody> ()) {
						firstSurfaceDetected = true;
						surfacePoint = hit.point;
					}
				} 
			} 

			if (firstSurfaceDetected) {
				float currentDistance = GKC_Utils.distance (currentRagdollRootMotion.position, surfacePoint);

				if (currentDistance < rayDistance) {
					surfaceFound = true;
				}

				if (currentDistance < extraDistanceRaycast) {
					carryinRagdollSpeedMultiplier = 1;
				}
			}

			if (Physics.Linecast (currentRagdollRootMotion.position, currentRagdollRootMotion.position + transform.forward * rayDistance, out hit, layer)) {
				if (!hit.collider.GetComponent<Rigidbody> ()) {
					surfaceFound = true;
				}
			}

			transform.Translate (transform.InverseTransformDirection (transform.forward) * (Mathf.Abs (previousVelocity.magnitude) * carryinRagdollSpeedMultiplier * Time.deltaTime));
		}
	}


	//when the bullet touchs a surface, then
	public void checkObjectDetected (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactSoundEffect != null) {
				GetComponent<AudioSource> ().PlayOneShot (currentProjectileInfo.impactSoundEffect);
			}

			projectileUsed = true;

			//set the bullet kinematic
			objectToDamage = col.GetComponent<Collider> ().gameObject;

			previousVelocity = mainRigidbody.velocity;

			//print (objectToDamage.name);
			mainRigidbody.isKinematic = true;

			if (carryingRagdoll) {
				return;
			}

			Rigidbody objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

			Transform currentCharacter = null;

			GameObject currentCharacterGameObject = applyDamage.getCharacterOrVehicle (objectToDamage);

			if (currentCharacterGameObject != null) {
				currentCharacter = currentCharacterGameObject.transform;
			}

			if (objectToDamageRigidbody != null) {
				
				currentRagdoll = objectToDamage.GetComponent<ragdollActivator> ();

				if (currentRagdoll != null) {
					Vector3 currentPosition = transform.position;

					List<ragdollActivator.bodyPart> bones = currentRagdoll.getBodyPartsList ();

					float distance = Mathf.Infinity;

					int index = -1;

					for (int i = 0; i < bones.Count; i++) {
						float currentDistance = GKC_Utils.distance (bones [i].transform.position, currentPosition);

						if (currentDistance < distance) {
							distance = currentDistance;
							index = i;
						}
					}

					if (index != -1) {
						//transform.SetParent (bones [index].transform);
						//print (bones [index].transform.name);
						if (applyDamage.checkIfDead (objectToDamage)) {
							mainRigidbody.isKinematic = false;

							mainRigidbody.velocity = previousVelocity;

							projectileUsed = false;

							currentRagdoll.setRagdollOnExternalParent (transform);
						}
					}
				} else if (currentCharacter != null) {
					transform.SetParent (currentCharacter);
				} else if (objectToDamage != null) {
					transform.SetParent (objectToDamage.transform);
				}
			} else if (currentCharacter != null) {
				transform.SetParent (currentCharacter);
			}

			checkProjectilesParent ();

			//add velocity if the touched object has rigidbody
			if (currentProjectileInfo.killInOneShot) {
				applyDamage.killCharacter (gameObject, objectToDamage, -transform.forward, transform.position, currentProjectileInfo.owner, false);
			
				if (currentRagdoll != null) {
					projectilePaused = true;

					transform.SetParent (null);

					currentRagdollRootMotion = currentRagdoll.getRootMotion ();

					currentRagdollRootMotion.SetParent (transform);

					currentRagdollRootMotion.GetComponent<Rigidbody> ().isKinematic = true;

					currentRagdoll.setRagdollOnExternalParent (transform);

					projectileUsed = false;
					carryingRagdoll = true;
				}
			} else {
				applyDamage.checkHealth (gameObject, objectToDamage, currentProjectileInfo.projectileDamage, -transform.forward, 
					transform.position, currentProjectileInfo.owner, false, true, currentProjectileInfo.ignoreShield, false, 
					currentProjectileInfo.canActivateReactionSystemTemporally, currentProjectileInfo.damageReactionID, currentProjectileInfo.damageTypeID);
			}

			if (currentProjectileInfo.applyImpactForceToVehicles) {
				Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);

				if (objectToDamageMainRigidbody != null) {
					Vector3 force = transform.forward * currentProjectileInfo.impactForceApplied;

					objectToDamageMainRigidbody.AddForce (force * objectToDamageMainRigidbody.mass, currentProjectileInfo.forceMode);
				}
			} else {
				if (applyDamage.canApplyForce (objectToDamage)) {
					Vector3 force = transform.forward * currentProjectileInfo.impactForceApplied;

					objectToDamageRigidbody.AddForce (force * objectToDamageRigidbody.mass, currentProjectileInfo.forceMode);
				}
			}
		}
	}

	public override void resetProjectile ()
	{
		base.resetProjectile ();


	}
}