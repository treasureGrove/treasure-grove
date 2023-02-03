using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class teleportationPlatform : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool teleportEnabled = true;
	public Transform platformToMove;
	public LayerMask layermask;

	public bool useButtonToActivate;

	[Space]
	[Header ("Rotation Settings")]
	[Space]

	public bool setObjectRotation;
	public bool setFullObjectRotation;

	public bool adjustPlayerCameraRotationIfPlayerTeleported;

	public Transform objectRotationTransform;

	[Space]
	[Header ("Gravity Settings")]
	[Space]

	public bool setGravityDirection;

	public setGravity setGravityManager;

	[Space]
	[Header ("Debug")]
	[Space]

	public GameObject objectInside;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool callEventOnTeleport;
	public UnityEvent eventOnTeleport;
	public bool callEventOnEveryTeleport;

	[Space]
	[Header ("Components")]
	[Space]

	public teleportationPlatform platformToMoveManager;

	bool eventCalled;

	RaycastHit hit;
	grabbedObjectState currentGrabbedObject;

	void Start ()
	{
		if (platformToMove != null) {
			platformToMoveManager = platformToMove.GetComponent<teleportationPlatform> ();
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.GetComponent<Rigidbody> () && objectInside == null) {
			objectInside = col.gameObject;

			if (!teleportEnabled) {
				return;
			}

			//if the object is being carried by the player, make him drop it
			currentGrabbedObject = objectInside.GetComponent<grabbedObjectState> ();

			if (currentGrabbedObject != null) {
				if (currentGrabbedObject.isCarryingObjectPhysically ()) {
					objectInside = null;

					return;
				} else {
					GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), objectInside);
				}
			}

			if (!useButtonToActivate) {
				activateTeleport ();
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (objectInside != null && col.gameObject == objectInside) {
			objectInside = null;
			currentGrabbedObject = null;
		}
	}

	void activateDevice ()
	{
		if (!teleportEnabled) {
			return;
		}

		if (useButtonToActivate && objectInside != null) {
			activateTeleport ();
		}
	}

	public void activateTeleport ()
	{
		platformToMoveManager.sendObject (objectInside);

		if (callEventOnTeleport) {
			if (!eventCalled || callEventOnEveryTeleport) {
				eventOnTeleport.Invoke ();
				eventCalled = true;
			}
		}
	}

	public void sendObject (GameObject objectToMove)
	{
		if (Physics.Raycast (transform.position + transform.up * 2, -transform.up, out hit, Mathf.Infinity, layermask)) {
			objectToMove.transform.position = hit.point;

			objectInside = objectToMove;
			
			if (setObjectRotation) {
				if (setFullObjectRotation) {
					objectToMove.transform.rotation = objectRotationTransform.rotation;
				} else {
					float rotationAngle = Vector3.SignedAngle (objectToMove.transform.forward, objectRotationTransform.forward, objectToMove.transform.up);
				
					objectToMove.transform.Rotate (objectToMove.transform.up * rotationAngle);
				}

				if (adjustPlayerCameraRotationIfPlayerTeleported) {
					playerController currentPlayerController = objectToMove.GetComponent<playerController> ();

					if (currentPlayerController != null) {
						currentPlayerController.getPlayerCameraGameObject ().transform.rotation = objectToMove.transform.rotation;
					}
				}
			}
		}

		if (setGravityDirection && setGravityManager != null) {
			Collider currentCollider = objectInside.GetComponent<Collider> ();

			if (currentCollider != null) {
				setGravityManager.checkTriggerType (currentCollider, true);
			}
		}
	}

	public void setTeleportEnabledState (bool state)
	{
		teleportEnabled = state;
	}
}