using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class eventObjectFoundOnRaycastSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkObjectsEnabled = true;

	public Transform raycastTransform;
	public float rayDistanceToCheckObjectFound;
	public LayerMask layerToCheckObjectFound;

	public bool checkObjectsOnUpdate;

	[Space]
	[Header ("Type of Event Settings")]
	[Space]

	public bool checkEventsOnFoundCaptureSystem = true;

	public bool checkEventsOnRemoteEventSystem;

	public string remoteEventToCall;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventToCallObjecObjectDetected;
	public UnityEvent eventToCallOnObjectDetected;

	RaycastHit hit;

	GameObject currentObjectDetected;

	GameObject previousObjectDetected;

	bool raycastTransformLocated;


	void Start ()
	{
		if (checkObjectsOnUpdate) {
			activateRaycastDetectionOnUpdate ();
		}
	}

	public void activateRaycastDetectionOnUpdate ()
	{
		StartCoroutine (checkObjectWithRaycastCoroutine ());
	}

	IEnumerator checkObjectWithRaycastCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			checkObjectWithRaycast ();
		}
	}

	public void checkObjectWithRaycast ()
	{
		if (checkObjectsEnabled) {
			if (!raycastTransformLocated) {

				raycastTransformLocated = raycastTransform != null;
			}

			if (raycastTransformLocated) {
				if (Physics.Raycast (raycastTransform.position, raycastTransform.forward, out hit, rayDistanceToCheckObjectFound, layerToCheckObjectFound)) {

					currentObjectDetected = hit.collider.gameObject;

					if (currentObjectDetected != previousObjectDetected) {

						previousObjectDetected = currentObjectDetected;

						if (checkEventsOnFoundCaptureSystem) {
							eventObjectFoundOnCaptureSystem currentEventObjectFoundOnCaptureSystem = hit.collider.gameObject.GetComponent<eventObjectFoundOnCaptureSystem> ();

							if (currentEventObjectFoundOnCaptureSystem != null) {
								currentEventObjectFoundOnCaptureSystem.callEventOnCapture ();
							}
						}

						if (checkEventsOnRemoteEventSystem) {
							remoteEventSystem currentRemoteEventSystem = hit.collider.gameObject.GetComponent<remoteEventSystem> ();

							if (currentRemoteEventSystem != null) {
								currentRemoteEventSystem.callRemoteEvent (remoteEventToCall);
							}
						}


						if (useEventToCallObjecObjectDetected) {
							eventToCallOnObjectDetected.Invoke ();
						}
					}
				} else {
					if (currentObjectDetected != null) {
						currentObjectDetected = null;

						previousObjectDetected = null;
					}
				}
			}
		}
	}

	public void setRaycastTransform (Transform newObject)
	{
		raycastTransform = newObject;
	}

	public void setCheckObjectsEnabledState (bool state)
	{
		checkObjectsEnabled = state;
	}
}
