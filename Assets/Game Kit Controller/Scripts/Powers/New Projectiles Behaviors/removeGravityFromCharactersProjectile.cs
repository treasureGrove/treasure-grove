using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class removeGravityFromCharactersProjectile : projectileSystem
{
	[Header ("Custom Settings")]
	[Space]

	public string remoteEventName = "Remove Gravity From Character";

	public applyEffectOnArea mainAreaEffectOnArea;

	//when the bullet touchs a surface, then
	public void checkObjectDetected (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactSoundEffect != null) {
				GetComponent<AudioSource> ().PlayOneShot (currentProjectileInfo.impactSoundEffect);
			}

			projectileUsed = true;

			mainAreaEffectOnArea.gameObject.SetActive (true);

			mainAreaEffectOnArea.setEffectActiveState (true);

			mainRigidbody.isKinematic = true;

			creatImpactParticles ();

			disableBullet (currentProjectileInfo.impactDisableTimer);
		}
	}

	public override void resetProjectile ()
	{
		base.resetProjectile ();

		mainAreaEffectOnArea.gameObject.SetActive (false);

		mainAreaEffectOnArea.setEffectActiveState (false);
	}

	public void applyEffect (GameObject objectToAffect)
	{
		if (objectToAffect != null) {
			playerComponentsManager currentplayerComponentsManager = objectToAffect.GetComponent<playerComponentsManager> ();

			if (currentplayerComponentsManager != null) {

				remoteEventSystem currentRemoteEventSystem = currentplayerComponentsManager.getRemoteEventSystem ();

				if (currentRemoteEventSystem != null) {
					currentRemoteEventSystem.callRemoteEvent (remoteEventName);
				}
			}
		}
	}
}