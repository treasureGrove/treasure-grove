using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignoreCollisionSystem : MonoBehaviour
{
	public Collider mainCollider;

	public bool useColliderList;
	public List<Collider> colliderList = new List<Collider> ();

	public void activateIgnoreCollision (Collider objectToIgnore)
	{
		if (useColliderList) {
			for (int i = 0; i < colliderList.Count; i++) {
				Physics.IgnoreCollision (objectToIgnore, colliderList [i], true);
			}
		} else {
			Physics.IgnoreCollision (objectToIgnore, mainCollider, true);
		}
	}
}
