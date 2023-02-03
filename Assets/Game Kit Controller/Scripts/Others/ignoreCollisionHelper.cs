using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignoreCollisionHelper : MonoBehaviour
{
	public bool collisionCheckEnabled = true;

	public Collider mainCollider;

	public LayerMask layerToIgnore;

	void OnCollisionEnter (Collision col)
	{
		if (!collisionCheckEnabled) {
			return;
		}

		GameObject objectDetected = col.gameObject;

		if ((1 << objectDetected.layer & layerToIgnore.value) == 1 << objectDetected.layer) {
			if (mainCollider == null) {
				mainCollider = GetComponent<Collider> ();
			}

			Physics.IgnoreCollision (col.collider, mainCollider, true);
		}
	}
}
