using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneWayPlatformSystem : MonoBehaviour
{
	public Collider platformCollider;

	public Transform platformTransform;

	public List<string> tagToCheck = new List<string> ();

	public bool playerNeedsToCrouchToIgnore;

	public bool useOnlyCrouchToIgnore;

	public bool ignoreCollisionOnTop;
	public bool ignoreCollisionOnBottom;

	public bool ignoringPlayerCollider;
	public bool playerFound;
	public Transform currentPlayerTransform;
	public Collider currentPlayerCollider;
	playerController currentPlayerControllerManager;

	void Start ()
	{
		
	}

	void Update ()
	{
		if (ignoringPlayerCollider) {
			if (platformTransform.transform.position.y < currentPlayerTransform.position.y) {
				Physics.IgnoreCollision (platformCollider, currentPlayerCollider, false);
				ignoringPlayerCollider = false;
			}
		}

		if (ignoreCollisionOnTop && playerFound) {
			if (currentPlayerControllerManager) {
				if ((currentPlayerControllerManager.getVerticalInput () < 0 && (!playerNeedsToCrouchToIgnore || currentPlayerControllerManager.isCrouching ()))
				    || (useOnlyCrouchToIgnore && currentPlayerControllerManager.isCrouching ())) {

					Physics.IgnoreCollision (platformCollider, currentPlayerCollider, true);

					currentPlayerControllerManager.setcheckOnGroundStatePausedState (true);
				}
			}
		}
	}

	public void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	public void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (tagToCheck.Contains (col.tag)) {
			currentPlayerTransform = col.transform;
			currentPlayerCollider = col;
			currentPlayerControllerManager = currentPlayerTransform.GetComponent<playerController> ();

			if (isEnter) {
				if (ignoreCollisionOnBottom) {
					if (platformTransform.transform.position.y > currentPlayerTransform.position.y) {
						Physics.IgnoreCollision (platformCollider, currentPlayerCollider, true);
						ignoringPlayerCollider = true;
					}
				}

				playerFound = true;
			} else {
				Physics.IgnoreCollision (platformCollider, col, false);

				if (ignoreCollisionOnTop) {
					currentPlayerControllerManager.setcheckOnGroundStatePausedState (false);
				}

				playerFound = false;
			}
		}
	}
}
