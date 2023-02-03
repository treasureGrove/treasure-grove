using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class damageCharacterOnCollision : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool damageEnabled = true;

	public bool addForceToRigidbodies = true;
	public float forceAmountToRigidbodies = 1000;

	public bool applyDamageWhenPushCharacter;
	public float extraForceOnCollision;

	[Space]
	[Header ("Collisions Settings")]
	[Space]

	public bool useMinimumCollisionSpeed;
	public float minimumCollisionSpeed;

	public bool checkMinimumSpeedOnMainRigidbody;
	public float minimumSpeedOnMainRigidbody;

	[Space]
	[Header ("Damage Settings")]
	[Space]

	public bool killCharacterOnCollision = true;
	public bool pushCharacterOnCollision;

	public bool ignoreShield;

	public int damageTypeID = -1;

	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	public float damageMultiplier = 1;

	public bool applyFixedDamageValue;
	public float fixedDamageValue;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameList = new List<string> ();

	[Space]
	[Header ("Components")]
	[Space]

	public Rigidbody mainRigidbody;

	ContactPoint currentContact;
	GameObject collisionObject;


	void OnCollisionEnter (Collision collision)
	{
		if (!damageEnabled) {
			return;
		}

		bool canActivateCollisionReaction = true;

		if (useMinimumCollisionSpeed) {
			float collisionMagnitude = collision.relativeVelocity.magnitude;

			if (Mathf.Abs (collisionMagnitude) < minimumCollisionSpeed) {
				canActivateCollisionReaction = false;
			} 
				
			if (checkMinimumSpeedOnMainRigidbody) {
				if (mainRigidbody == null) {
					mainRigidbody = GetComponent<Rigidbody> ();
				}

				if (mainRigidbody != null) {
					float speedOnMainRigidbody = mainRigidbody.velocity.magnitude;

					if (Mathf.Abs (speedOnMainRigidbody) < minimumSpeedOnMainRigidbody) {
						canActivateCollisionReaction = false;
					}
				}
			}
		}

		if (canActivateCollisionReaction) {
			currentContact = collision.contacts [0];

			collisionObject = collision.gameObject;

			if (useRemoteEventOnObjectsFound) {
				remoteEventSystem currentRemoteEventSystem = collisionObject.GetComponent<remoteEventSystem> ();
			
				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameList.Count; i++) {
						currentRemoteEventSystem.callRemoteEvent (remoteEventNameList [i]);
					}
				}
			}

			if (addForceToRigidbodies) {
				if (applyDamage.canApplyForce (collisionObject)) {
					collision.rigidbody.AddExplosionForce (forceAmountToRigidbodies, collision.transform.position, 100);
				}
			}

			if (killCharacterOnCollision) {
				//applyDamage.killCharacter (collisionObject);
				float damage = applyDamage.getCurrentHealthAmount (collisionObject);

				applyDamage.checkHealth (gameObject, collisionObject, damage, transform.forward, currentContact.point, gameObject, 
					false, true, ignoreShield, false, canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
			} else {
				if (pushCharacterOnCollision) {
					Vector3 pushDirection = (currentContact.point - transform.position).normalized;

					if (extraForceOnCollision > 0) {
						pushDirection *= extraForceOnCollision;
					}

					applyDamage.pushCharacter (collisionObject, pushDirection);
				}

				if (applyDamageWhenPushCharacter) {
					float damage = 0;

					if (applyFixedDamageValue) {
						damage = fixedDamageValue;
					} else {
						damage = collision.relativeVelocity.magnitude * damageMultiplier;
					}

					applyDamage.checkHealth (gameObject, collisionObject, damage, transform.forward, currentContact.point, gameObject, 
						false, true, ignoreShield, false, canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
				}
			}
		}
	}

	public void setDamageEnabledState (bool state)
	{
		damageEnabled = state;
	}

	public void disableDamage ()
	{
		setDamageEnabledState (false);
	}

	public void enableDamage ()
	{
		setDamageEnabledState (true);
	}
}
