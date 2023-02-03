using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class nanoBlade : projectileSystem
{
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
			Vector3 previousVelocity = mainRigidbody.velocity;

			//print (objectToDamage.name);
			mainRigidbody.isKinematic = true;

			Rigidbody objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

			Transform currentCharacter = null;

			GameObject currentCharacterGameObject = applyDamage.getCharacterOrVehicle (objectToDamage);

			if (currentCharacterGameObject != null) {
				currentCharacter = currentCharacterGameObject.transform;
			}

			if (objectToDamageRigidbody != null) {
				ragdollActivator currentRagdollActivator = objectToDamage.GetComponent<ragdollActivator> ();

				if (currentRagdollActivator != null) {

					Vector3 currentPosition = transform.position;

					List<ragdollActivator.bodyPart> bones = currentRagdollActivator.getBodyPartsList ();

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
						transform.SetParent (bones [index].transform);

						//print (bones [index].transform.name);
						if (applyDamage.checkIfDead (objectToDamage)) {
							mainRigidbody.isKinematic = false;

							mainRigidbody.velocity = previousVelocity;

							projectileUsed = false;
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
			} else {
				applyDamage.checkHealth (gameObject, objectToDamage, currentProjectileInfo.projectileDamage, -transform.forward, transform.position, 
					currentProjectileInfo.owner, false, true, currentProjectileInfo.ignoreShield, false, currentProjectileInfo.canActivateReactionSystemTemporally,
					currentProjectileInfo.damageReactionID, currentProjectileInfo.damageTypeID);
			}

			if (currentProjectileInfo.applyImpactForceToVehicles) {
				Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);

				if (objectToDamageMainRigidbody) {
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