using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeGlobalGravityPower : MonoBehaviour
{
	public bool powerEnabled = true;
	public otherPowers powersManager;
	public Transform mainCameraTransform;
	public LayerMask layer;

	public gravitySystem gravityManager;

	public bool changeGlobalGravityOnSecondaryPowerActive;

	public bool changeGravityOnVehicles;

	public bool changeGravityOnCharacter;

	public bool pushNPCS;
	public string messageNameToSend = "pushCharacter";
	public List<string> ignoreTagList = new List<string> ();

	List<vehicleGravityControl> vehicleGravityControlList = new List<vehicleGravityControl> ();

	RaycastHit hit;

	public void activatePower ()
	{
		if (!powerEnabled) {
			return;
		}

		//change level's gravity
		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, layer)) {
			if (!hit.collider.isTrigger && !hit.collider.gameObject.GetComponent<Rigidbody> ()) {
				powersManager.createShootParticles ();
				Physics.gravity = -hit.normal * 9.8f;
			}
		}
	}

	public void activateSecondaryPower ()
	{
		if (!powerEnabled) {
			return;
		}

		//change level's gravity
		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, layer)) {
			if (!hit.collider.isTrigger && !hit.collider.gameObject.GetComponent<Rigidbody> ()) {
				powersManager.createShootParticles ();

				if (changeGlobalGravityOnSecondaryPowerActive) {
					Physics.gravity = -hit.normal * 9.8f;
				}

				if (changeGravityOnCharacter) {
					gravityManager.changeGravityDirectionDirectly (hit.normal, true);
				}

				if (changeGravityOnVehicles) {
					
					vehicleGravityControl[] vehicleGravityControlListFound = FindObjectsOfType (typeof(vehicleGravityControl)) as vehicleGravityControl[];
					for (int i = 0; i < vehicleGravityControlListFound.Length; i++) {
						vehicleGravityControlList.Add (vehicleGravityControlListFound [i]);
					}

					for (int i = 0; i < vehicleGravityControlList.Count; i++) {
						vehicleGravityControlList[i].rotateVehicleToLandSurface (hit.normal);
					}
				}

				if (pushNPCS) {
					playerController[] playerControllerListFound = FindObjectsOfType (typeof(playerController)) as playerController[];
					for (int i = 0; i < playerControllerListFound.Length; i++) {
						GameObject objectToPush = playerControllerListFound [i].gameObject;

						if (!ignoreTagList.Contains (objectToPush.tag)) {

							objectToPush.SendMessage (messageNameToSend, hit.normal, SendMessageOptions.DontRequireReceiver);
						}
					}
				}
			}
		}
	}
}
