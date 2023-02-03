using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class placeObjectSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public GameObject objectToPlace;

	public bool needsOtherObjectPlacedBefore;
	public int numberOfObjectsToPlaceBefore;

	[Space]
	[Header ("Placement Settings")]
	[Space]

	public bool useRotationLimit;
	public float maxUpRotationAngle = 30;
	public float maxForwardRotationAngle = 30;

	public bool usePositionLimit;
	public float maxPositionDistance;

	public float placeObjectPositionSpeed;
	public float placeObjectRotationSpeed;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool objectPlaced;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent objectPlacedEvent;

	[Space]
	[Header ("Components")]
	[Space]

	public puzzleSystem puzzleManager;

	Coroutine placeObjectCoroutine;
	int currentNumberObjectsPlaced;
	bool objectInsideTrigger;

	bool objectInCorrectPosition;
	bool objectInCorrectRotation;
	bool movingObject;


	void Update ()
	{
		if (objectInsideTrigger && !objectPlaced) {
			if (useRotationLimit) {
				float forwardAngle = Vector3.SignedAngle (transform.forward, objectToPlace.transform.forward, transform.up);
				float upAngle = Vector3.SignedAngle (transform.up, objectToPlace.transform.up, transform.forward);
				if (Mathf.Abs (forwardAngle) > maxForwardRotationAngle || Mathf.Abs (upAngle) > maxUpRotationAngle) {  
					objectInCorrectRotation = false;
				} else {
					objectInCorrectRotation = true;
				}
			}

			if (usePositionLimit) {
				float currentDistance = GKC_Utils.distance (objectToPlace.transform.position, transform.position);
				if (currentDistance <= maxPositionDistance) {
					objectInCorrectPosition = true;
				} else {
					objectInCorrectPosition = false;
				}
			}

			if (useRotationLimit && !objectInCorrectRotation) {
				return;
			}

			if (usePositionLimit && !objectInCorrectPosition) {
				return;
			}

			checkIfCanBePlaced ();
		}
	}

	public void OnTriggerEnter (Collider col)
	{
		if (!objectPlaced) {
			GameObject objectToCheck = canBeDragged (col.gameObject);

			if (objectToCheck != null) {

				if (!useRotationLimit && !usePositionLimit) {
					checkIfCanBePlaced ();
				}

				objectInsideTrigger = true;
			}
		}
	}

	public void OnTriggerExit (Collider col)
	{
		if (!objectPlaced) {
			GameObject objectToCheck = canBeDragged (col.gameObject);

			if (objectToCheck != null) {
				objectInsideTrigger = false;
			}
		}
	}

	public void checkIfCanBePlaced ()
	{
		bool objectCanBePlaced = true;

		if (needsOtherObjectPlacedBefore) {
			if (numberOfObjectsToPlaceBefore != currentNumberObjectsPlaced) {
				objectCanBePlaced = false;
			}
		}

		if (objectCanBePlaced) {

			puzzleManager.checkIfObjectGrabbed (objectToPlace);

			Rigidbody mainRigidbody = objectToPlace.GetComponent<Rigidbody> ();

			if (mainRigidbody != null) {
				mainRigidbody.isKinematic = true;
			}

			moveObjectToPlace ();

			objectPlaced = true;
		}
	}

	public GameObject canBeDragged (GameObject objectToCheck)
	{
		if (objectToPlace == objectToCheck) {
			return objectToCheck;
		}

		if (objectToCheck.transform.IsChildOf (objectToPlace.transform)) {
			return objectToPlace;
		}

		return null;
	}

	public void moveObjectToPlace ()
	{
		if (placeObjectCoroutine != null) {
			StopCoroutine (placeObjectCoroutine);
		}

		placeObjectCoroutine = StartCoroutine (placeObjectIntoPosition ());
	}

	IEnumerator placeObjectIntoPosition ()
	{
		float dist = GKC_Utils.distance (objectToPlace.transform.position, transform.position);
		float duration = dist / placeObjectPositionSpeed;
		float t = 0;

		float timePassed = 0;

		while ((t < 1 || objectToPlace.transform.position != transform.position || objectToPlace.transform.rotation != transform.rotation) && timePassed < 3) {
			t += Time.deltaTime / duration;

			objectToPlace.transform.position = Vector3.MoveTowards (objectToPlace.transform.position, transform.position, t);
			objectToPlace.transform.rotation = Quaternion.Slerp (objectToPlace.transform.rotation, transform.rotation, t);
			timePassed += Time.deltaTime;

			yield return null;
		}

		if (objectPlacedEvent.GetPersistentEventCount () > 0) {
			objectPlacedEvent.Invoke ();
		}
	}

	public void increaseNumberObjectsPlaced ()
	{
		currentNumberObjectsPlaced++;
	}

	public void resetNumberObjectsPlaced ()
	{
		if (placeObjectCoroutine != null) {
			StopCoroutine (placeObjectCoroutine);
		}

		currentNumberObjectsPlaced = 0;

		objectPlaced = false;

		objectInsideTrigger = false;
	}
}
