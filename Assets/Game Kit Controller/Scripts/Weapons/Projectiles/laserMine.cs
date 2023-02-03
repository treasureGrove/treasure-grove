using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserMine : projectileSystem
{
	[Header ("Main Settings")]
	[Space]

	public GameObject laserBeam;
	public bool disableByTime;
	public float timeToDisable;
	public bool infiniteEnergy;
	public int numberOfContactsToDisable;

	int currentNumberOfContacts;

	public void increaseNumberOfContacts ()
	{
		if (!infiniteEnergy) {
			currentNumberOfContacts++;

			if (currentNumberOfContacts >= numberOfContactsToDisable) {
				disableBullet (0);

				projectilePaused = false;
			}
		}
	}

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

			if (currentProjectileInfo.adhereToSurface) {
				attachProjectileToSurface ();
			}

			if (disableByTime) {
				disableBullet (timeToDisable);
			} else {
				projectilePaused = true;
			}
		}
	}

	public override void resetProjectile ()
	{
		base.resetProjectile ();

		currentNumberOfContacts = 0;
	}
}