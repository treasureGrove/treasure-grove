using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class removeGravityFromCharacterSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float removeGravityDuration = 5;

	public float extraGravityForce = 5;

	public float extraTorqueForce = 8;
	public ForceMode torqueForceMode;

	public bool pauseExtraForceAfterDelay;

	public float pauseExtraForceDelay;

	public float pauseExtraForceSpeed = 3;

	[Space]
	[Header ("Components")]
	[Space]

	public ragdollActivator mainRagdollActivator;
	public Transform characterTransform;


	Coroutine removeGravityCoroutine;

	public void activateRemoveGravity ()
	{
		if (removeGravityCoroutine != null) {
			StopCoroutine (removeGravityCoroutine);
		}

		removeGravityCoroutine = StartCoroutine (activateRemoveGravityCoroutine ());
	}

	IEnumerator activateRemoveGravityCoroutine ()
	{
		mainRagdollActivator.pushCharacterWithoutForceXAmountOfTime (removeGravityDuration);

		mainRagdollActivator.enableOrDisableRagdollGravityState (false);

		Vector3 initialForce = characterTransform.up * extraGravityForce;

		mainRagdollActivator.setVelocityToRagdollInDirection (initialForce);

		Rigidbody hipsRigidbody = mainRagdollActivator.getHipsRigidbody ();

		if (hipsRigidbody != null) {
			hipsRigidbody.AddRelativeTorque (characterTransform.forward * extraTorqueForce, torqueForceMode);
			hipsRigidbody.AddRelativeTorque (characterTransform.right * extraTorqueForce, torqueForceMode);
//			hipsRigidbody.AddTorque (hipsRigidbody.transform.up * extraTorqueForce);
		}

//		yield return new WaitForSeconds (removeGravityDuration);

		bool targetReached = false;

		float timer = 0;

		bool pauseExtraForceActivated = false;

		while (!targetReached) {
			timer += Time.deltaTime;

			if (pauseExtraForceAfterDelay) {
				if (pauseExtraForceActivated) {
					if (GKC_Utils.distance (initialForce, Vector3.zero) > 0.05f) {
						initialForce = Vector3.MoveTowards (initialForce, Vector3.zero, timer * pauseExtraForceSpeed);

						mainRagdollActivator.setVelocityToRagdollInDirection (initialForce);
					}
				} else {
					if (timer >= pauseExtraForceDelay) {
						pauseExtraForceActivated = true;
					}
				}
			}

			if (timer >= removeGravityDuration) {
				targetReached = true;
			}

			yield return null;
		}

		mainRagdollActivator.enableOrDisableRagdollGravityState (true);
	}
}