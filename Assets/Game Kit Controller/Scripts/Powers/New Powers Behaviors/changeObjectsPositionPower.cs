using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeObjectsPositionPower : MonoBehaviour
{
	public bool powerEnabled = true;
	public otherPowers powersManager;
	public Transform mainCameraTransform;
	public LayerMask layer;
	public LayerMask groundLayer;
	public Transform playerTransform;
	public Transform playerCameraTransform;
	public gravitySystem gravityManager;

	RaycastHit hit;

	public void activatePower ()
	{
		if (!powerEnabled) {
			return;
		}

		//this power changes the player's position with the object located with a ray when the player fires

		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, layer)) {
			if (!hit.collider.isTrigger) {
				
				powersManager.createShootParticles ();

				GameObject objectToMove = hit.collider.gameObject;

				GameObject characterDetected = applyDamage.getCharacterOrVehicle (objectToMove);

				if (characterDetected != null) {
					objectToMove = characterDetected;
				} else if (!objectToMove.GetComponent<Rigidbody> ()) {
					return;
				}

				Vector3 newPlayerPosition = objectToMove.transform.position;
				Vector3 newObjectPosition = playerTransform.position;
				objectToMove.transform.position = Vector3.one * 100;
				playerTransform.position = Vector3.zero;

				if (Physics.Raycast (newPlayerPosition, -gravityManager.getCurrentNormal (), out hit, Mathf.Infinity, groundLayer)) {
					newPlayerPosition = hit.point;
				}

				if (Physics.Raycast (newObjectPosition + playerTransform.up, -gravityManager.getCurrentNormal (), out hit, Mathf.Infinity, groundLayer)) {
					newObjectPosition = hit.point + playerTransform.up;
				}

				objectToMove.transform.position = newObjectPosition;
				playerTransform.position = newPlayerPosition;
				playerCameraTransform.position = newPlayerPosition;
			}
		}
	}
}