using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class useDevicesAtDistancePower : MonoBehaviour
{
	public bool powerEnabled;

	public LayerMask layer;
	public usingDevicesSystem usingDevicesManager;
	public Transform mainCameraTransform;

	public playerInputManager playerInput;
	public Collider playerCollider;

	public List<string> tagToCheck = new List<string> ();
	public GameObject player;

	RaycastHit hit;

	GameObject currentDeviceToUse;

	public void activatePower ()
	{
		if (!powerEnabled) {
			return;
		}

		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, layer)) {
			if (hit.collider.isTrigger && tagToCheck.Contains (hit.collider.tag)) {
				currentDeviceToUse = hit.collider.gameObject;

				electronicDevice currentElectronicDevice = currentDeviceToUse.GetComponent<electronicDevice> ();
				if (currentElectronicDevice) {
					currentElectronicDevice.checkTriggerInfo (playerCollider, true);
					usingDevicesManager.useCurrentDevice (currentDeviceToUse);
					usingDevicesManager.setObjectToRemoveAferStopUse (currentDeviceToUse);
				}

				simpleSwitch currentSimpleSwitch = currentDeviceToUse.GetComponent<simpleSwitch> ();
				if (currentSimpleSwitch) {
					usingDevicesManager.useCurrentDevice (currentDeviceToUse);
					usingDevicesManager.setObjectToRemoveAferStopUse (currentDeviceToUse);
				}

				inventoryObject currentInventoryObject = currentDeviceToUse.GetComponent<inventoryObject> ();
				if (currentInventoryObject) {
					pickUpObject currentPickupObject = currentDeviceToUse.GetComponentInParent<pickUpObject> ();
					if (currentPickupObject) {
						currentPickupObject.checkTriggerInfo (playerCollider);
						usingDevicesManager.useCurrentDevice (currentDeviceToUse);
						usingDevicesManager.setObjectToRemoveAferStopUse (currentDeviceToUse);
					}
				}
			}
		}
	}
}
