using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remoteEventOnCameraCaptureSystem : MonoBehaviour
{
	public bool checkObjectFoundOnCaptureEnabled = true;
	public Transform cameraTransform;
	public LayerMask layermaskToUse;
	public float maxRaycastDistance = 20;

	RaycastHit hit;

	public void checkCapture ()
	{
		if (checkObjectFoundOnCaptureEnabled) {
			if (Physics.Raycast (cameraTransform.position, cameraTransform.forward, out hit, maxRaycastDistance, layermaskToUse)) {

				GameObject objectDetected = hit.collider.gameObject;

				GameObject character = applyDamage.getCharacterOrVehicle (objectDetected);

				if (character != null) {
					objectDetected = character;
				}

				if (objectDetected != null) {

					eventObjectFoundOnCaptureSystem currentEventObjectFoundOnCaptureSystem = objectDetected.GetComponent<eventObjectFoundOnCaptureSystem> ();
			
					if (currentEventObjectFoundOnCaptureSystem) {
						currentEventObjectFoundOnCaptureSystem.callEventOnCapture ();
					}
				}
			}
		}
	}

	public void setNewCameraTransform (Transform newCamera)
	{
		cameraTransform = newCamera;
	}
}
