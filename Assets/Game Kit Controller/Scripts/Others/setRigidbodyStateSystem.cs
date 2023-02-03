using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setRigidbodyStateSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public ForceMode forceMode;
	[Tooltip ("The force direction in external transform use the UP direction of that transform")] public float forceAmount;

	public float explosionRadius;
	public float explosioUpwardAmount;

	[Space]
	[Header ("Rigidbody List Settings")]
	[Space]

	public List<GameObject> rigidbodyList = new List<GameObject> ();


	public void setKinematicState (bool state)
	{
		for (int i = 0; i < rigidbodyList.Count; i++) {
			if (rigidbodyList [i] != null) {
				Rigidbody currentRigidbody = rigidbodyList [i].GetComponent<Rigidbody> ();

				if (currentRigidbody != null) {
					if (currentRigidbody.isKinematic != false) {
						currentRigidbody.isKinematic = false;
					}
				}
			}
		}
	}

	public void addForce (Transform forceDirection)
	{
		for (int i = 0; i < rigidbodyList.Count; i++) {
			if (rigidbodyList [i] != null) {
				Rigidbody currentRigidbody = rigidbodyList [i].GetComponent<Rigidbody> ();

				if (currentRigidbody != null) {
					if (currentRigidbody.isKinematic != false) {
						currentRigidbody.isKinematic = false;
					}

					currentRigidbody.AddForce (forceDirection.up * forceAmount, forceMode);
				}
			}
		}
	}

	public void addExplosiveForce (Transform explosionCenter)
	{
		for (int i = 0; i < rigidbodyList.Count; i++) {
			if (rigidbodyList [i] != null) {
				Rigidbody currentRigidbody = rigidbodyList [i].GetComponent<Rigidbody> ();

				if (currentRigidbody != null) {
					if (currentRigidbody.isKinematic != false) {
						currentRigidbody.isKinematic = false;
					}

					currentRigidbody.AddExplosionForce (forceAmount, explosionCenter.position, explosionRadius, explosioUpwardAmount, forceMode);
				}
			}
		}
	}

	public void addForceToThis (Transform forceDirection)
	{
		Rigidbody currentRigidbody = GetComponent<Rigidbody> ();
	
		if (currentRigidbody != null) {
			if (currentRigidbody.isKinematic != false) {
				currentRigidbody.isKinematic = false;
			}

			currentRigidbody.AddForce (forceDirection.up * forceAmount, forceMode);
		}
	}
}
