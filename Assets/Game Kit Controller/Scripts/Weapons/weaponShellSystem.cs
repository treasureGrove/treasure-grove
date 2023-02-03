using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponShellSystem : MonoBehaviour
{
	public Rigidbody mainRigidbody;
	public Collider mainCollider;
	public AudioSource mainAudioSource;

	public bool addRandomRotationToShells = true;
	public Vector2 randomRotationXRange = new Vector2 (-20, 20);
	public Vector2 randomRotationYRange = new Vector2 (-20, 20);
	public Vector2 randomRotationZRange = new Vector2 (-20, 20);

	public float randomRotationMultiplier;

	public void setShellValues (Vector3 forceDirection, Collider playerCollider, AudioClip clipToUse)
	{
		mainRigidbody.AddForce (forceDirection);

		if (addRandomRotationToShells) {
			float randomRotationX = Random.Range (randomRotationXRange.x, randomRotationXRange.y);
			float randomRotationY = Random.Range (randomRotationYRange.x, randomRotationYRange.y);
			float randomRotationZ = Random.Range (randomRotationZRange.x, randomRotationZRange.y);
			Vector3 randomRotation = new Vector3 (randomRotationX, randomRotationY, randomRotationZ);

			mainRigidbody.AddTorque (randomRotation * randomRotationMultiplier);
		}

		if (playerCollider != null) {
			Physics.IgnoreCollision (playerCollider, mainCollider);
		}

		if (clipToUse != null) {
			mainAudioSource.clip = clipToUse;
		}
	}
}
