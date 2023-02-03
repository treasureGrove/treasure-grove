using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class brainwashAIProjectile : projectileSystem
{
	[Header ("Custom Settings")]
	[Space]

	public string factionToConfigure = "Friend Soldiers";

	public bool setNewName;
	public string newName;
	public bool AIIsFriend;

	public string newTag = "friend";

	public bool followPartnerOnTriggerEnabled = true;

	public bool setPlayerAsPartner = true;

	public bool useRemoteEvents;


	//when the bullet touchs a surface, then
	public void checkObjectDetected (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactSoundEffect != null) {
				GetComponent<AudioSource> ().PlayOneShot (currentProjectileInfo.impactSoundEffect);
			}

			projectileUsed = true;

			objectToDamage = col.GetComponent<Collider> ().gameObject;

			mainRigidbody.isKinematic = true;


			Rigidbody objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

			Transform currentCharacter = null;

			GameObject currentCharacterGameObject = applyDamage.getCharacterOrVehicle (objectToDamage);

			if (currentCharacterGameObject != null) {
				currentCharacter = currentCharacterGameObject.transform;
			}

			if (objectToDamageRigidbody != null) {
				if (currentCharacter != null) {
					GKC_Utils.activateBrainWashOnCharacter (currentCharacter.gameObject, factionToConfigure, newTag, setNewName, newName, 
						followPartnerOnTriggerEnabled, setPlayerAsPartner, AIIsFriend,
						currentProjectileInfo.owner, useRemoteEvents, currentProjectileInfo.remoteEventNameList);
				}
			}

			disableBullet (currentProjectileInfo.impactDisableTimer);
		}
	}

	public override void resetProjectile ()
	{
		base.resetProjectile ();


	}
}