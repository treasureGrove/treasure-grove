using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class launchedObjects : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkCollisionsEnabled = true;

	public bool removeComponentOnCollisionEnabled = true;

	[Space]
	[Header ("Damage Settings")]
	[Space]

	public bool useCustomDamageAmount;
	public float customDamageAmount;

	public bool searchPlayerAsDamageAttacker;

	public int damageTypeID = -1;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameList = new List<string> ();

	bool ignoreShield;
	bool canActivateReactionSystemTemporally;
	int damageReactionID = -1;

	float timer = 0;
	GameObject currentPlayer;
	Collider currentPlayerCollider;
	Collider mainCollider;

	//this script is for the objects launched by the player, to check the object which collides with them

	void Update ()
	{
		if (!checkCollisionsEnabled) {
			return;
		}

		//if the launched objects does not collide with other object, remove the script
		if (timer > 0) {
			timer -= Time.deltaTime;

			if (timer < 0) {
				activateCollision ();
			}
		}
	}

	void OnCollisionEnter (Collision collision)
	{
		if (!checkCollisionsEnabled) {
			return;
		}

		if (!collision.collider.isTrigger) {
			//if the object has a health script, it reduces the amount of life according to the launched object velocity
			float damageAmountToUse = 0;

			if (useCustomDamageAmount) {
				damageAmountToUse = customDamageAmount;
			} else {
				damageAmountToUse = GetComponent<Rigidbody> ().velocity.magnitude;

				if (damageAmountToUse == 0) {
					damageAmountToUse = collision.relativeVelocity.magnitude;
				}
			}

			GameObject currentObjectFound = collision.collider.gameObject;

			bool objectToDamageFound = applyDamage.objectCanBeDamaged (currentObjectFound);

			if (objectToDamageFound) {

				if (searchPlayerAsDamageAttacker && currentPlayer == null) {
					currentPlayer = GKC_Utils.findMainPlayerOnScene ();
				}

				if (damageAmountToUse > 0) {
					applyDamage.checkHealth (gameObject, currentObjectFound, damageAmountToUse, -transform.forward, transform.position, currentPlayer, 
						false, true, ignoreShield, false, canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
				}

				bool isVehicle = applyDamage.isVehicle (currentObjectFound);

				if (isVehicle) {
					Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (currentObjectFound);

					if (objectToDamageMainRigidbody != null) {
						Vector3 collisionDirection = (collision.contacts [0].point - transform.position).normalized;

						objectToDamageMainRigidbody.AddForce (collisionDirection * damageAmountToUse * objectToDamageMainRigidbody.mass, ForceMode.Impulse);
					}				
				}

				activateCollision ();

				if (currentObjectFound != null) {
					if (useRemoteEventOnObjectsFound) {
						remoteEventSystem currentRemoteEventSystem = currentObjectFound.GetComponent<remoteEventSystem> ();

						if (currentRemoteEventSystem != null) {
							for (int i = 0; i < remoteEventNameList.Count; i++) {
								currentRemoteEventSystem.callRemoteEvent (remoteEventNameList [i]);
							}
						}
					}
				}
			}
			//else, set the timer to disable the script
			else {
				timer = 1;
			}
		}
	}

	void activateCollision ()
	{
		if (currentPlayerCollider != null) {
			
			mainCollider = GetComponent<Collider> ();

			if (mainCollider != null) {
				Physics.IgnoreCollision (mainCollider, currentPlayerCollider, false);
			}
		}

		if (removeComponentOnCollisionEnabled) {
			Destroy (this);
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
	}

	public void setCurrentPlayerAndCollider (GameObject player, Collider playerCollider)
	{
		currentPlayer = player;

		currentPlayerCollider = playerCollider;
	}

	public void setIgnoreShieldState (bool state)
	{
		ignoreShield = state;
	}

	public void setCanActivateReactionSystemTemporally (bool state)
	{
		canActivateReactionSystemTemporally = state;
	}

	public void setCamageReactionID (int newValue)
	{
		damageReactionID = newValue;
	}

	public void setCheckCollisionsEnabledState (bool state)
	{
		checkCollisionsEnabled = state;
	}
}