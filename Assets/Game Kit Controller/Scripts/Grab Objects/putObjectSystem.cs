using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class putObjectSystem : MonoBehaviour
{
	public bool putObjectSystemEnabled = true;

	public bool useCertainObjectToPlace;
	public GameObject certainObjectToPlace;
	public string objectNameToPlace;

	public Transform placeToPutObject;

	public GameObject currentObjectPlaced;

	public float placeObjectPositionSpeed;
	public float placeObjectRotationSpeed;

	public bool useRotationLimit;
	public float maxUpRotationAngle = 30;
	public float maxForwardRotationAngle = 30;

	public bool usePositionLimit;
	public float maxPositionDistance;

	public bool needsOtherObjectPlacedBefore;
	public int numberOfObjectsToPlaceBefore;

	public bool disableObjectOnceIsPlaced;

	public bool objectInsideTrigger;
	public bool objectPlaced;

	public bool waitToObjectPlacedToCallEvent;

	public objectToPlaceSystem currentObjectToPlaceSystem;

	public bool useLimitToCheckIfObjectRemoved;
	public float minDistanceToRemoveObject;

	public bool checkingIfObjectIsRemoved;

	public float minWaitToPutSameObjectAgain = 0.7f;
	public bool updateTriggerStateAfterWait;

	public UnityEvent objectPlacedEvent;
	public UnityEvent objectRemovedEvent;

	public bool useSoundEffectOnObjectPlaced;
	public AudioClip soundEffectOnObjectPlaced;
	public bool useSoundEffectOnObjectRemoved;
	public AudioClip soundEffectOnObjectRemoved;
	public AudioSource mainAudioSource;

	public Collider mainTrigger;

	Coroutine placeObjectCoroutine;
	int currentNumberObjectsPlaced;

	bool objectInCorrectPosition;
	bool objectInCorrectRotation;
	bool movingObject;

	float lastTimeObjectRemoved;
	GameObject lastObjectPlaced;

	Coroutine pauseCoroutine;

	bool cancelPlaceObjectActive;

	void Update ()
	{
		if (objectInsideTrigger) {
			if (!objectPlaced) {
				if (useRotationLimit) {
					float forwardAngle = Vector3.SignedAngle (transform.forward, currentObjectPlaced.transform.forward, transform.up);
					float upAngle = Vector3.SignedAngle (transform.up, currentObjectPlaced.transform.up, transform.forward);

					if (Mathf.Abs (forwardAngle) > maxForwardRotationAngle || Mathf.Abs (upAngle) > maxUpRotationAngle) {  
						objectInCorrectRotation = false;
					} else {
						objectInCorrectRotation = true;
					}
				}

				if (usePositionLimit) {
					float currentDistance = GKC_Utils.distance (currentObjectPlaced.transform.position, transform.position);

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

				checkIfCanBePlaced (false);
			} else {
				if (checkingIfObjectIsRemoved) {
					float currentDistance = GKC_Utils.distance (currentObjectPlaced.transform.position, placeToPutObject.position);

					if (currentDistance > minDistanceToRemoveObject) {
						removeObject ();
					} else {
						if (!currentObjectToPlaceSystem.isObjectInGrabbedState ()) {
							checkIfCanBePlaced (true);
						}
					}
				}
			}
		}
	}

	public void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (true, col.gameObject);	
	}

	public void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (false, col.gameObject);	
	}

	public void checkTriggerInfo (bool isEnter, GameObject objectToCheck)
	{
		if (!putObjectSystemEnabled) {
			return;
		}

		if (isEnter) {
			if (!objectPlaced) {
				objectToCheck = canBeDragged (objectToCheck);

				if (objectToCheck != null) {
					if (objectToCheck == lastObjectPlaced && Time.time < lastTimeObjectRemoved + minWaitToPutSameObjectAgain) {
						if (updateTriggerStateAfterWait) {
							pausePutObject ();
						}

						print ("same object to soon, pausing");

						return;
					}

					currentObjectPlaced = objectToCheck;

					if (!useRotationLimit && !usePositionLimit) {
						checkIfCanBePlaced (false);
					}

					objectInsideTrigger = true;

					lastObjectPlaced = currentObjectPlaced;

					if (cancelPlaceObjectActive) {
						resetPlacedObjectValues ();
					}

					cancelPlaceObjectActive = false;
				}
			}
		} else {
			if (!objectPlaced) {
				objectToCheck = canBeDragged (objectToCheck);

				if (objectToCheck != null) {
					currentObjectPlaced = null;
					objectInsideTrigger = false;

					lastTimeObjectRemoved = Time.time;

					print ("updating last time");
				}
			}
		}
	}

	public void checkIfCanBePlaced (bool objectPlacedPreviously)
	{
		bool objectCanBePlaced = true;

		if (needsOtherObjectPlacedBefore) {
			if (numberOfObjectsToPlaceBefore != currentNumberObjectsPlaced) {
				objectCanBePlaced = false;
			}
		}

		if (objectCanBePlaced) {
			Rigidbody mainRigidbody = currentObjectPlaced.GetComponent<Rigidbody> ();

			if (mainRigidbody != null) {
				mainRigidbody.isKinematic = true;
			}

			moveObjectToPlace (objectPlacedPreviously);

			objectPlaced = true;

			checkingIfObjectIsRemoved = false;
		}
	}

	public GameObject canBeDragged (GameObject objectToCheck)
	{
		if (useCertainObjectToPlace) {
			if (objectToCheck == certainObjectToPlace || objectToCheck.transform.IsChildOf (certainObjectToPlace.transform)) {
				return objectToCheck;
			}
		} else {
			currentObjectToPlaceSystem = objectToCheck.GetComponent<objectToPlaceSystem> ();

			if (currentObjectToPlaceSystem != null) {
				if (objectNameToPlace == currentObjectToPlaceSystem.getObjectName ()) {
					return objectToCheck;
				}
			}
		}

		return null;
	}

	public void moveObjectToPlace (bool objectPlacedPreviously)
	{
		stopMoveObjectToPlaceCoroutine ();

		stopPausePutObjectCoroutine ();

		placeObjectCoroutine = StartCoroutine (placeObjectIntoPosition (objectPlacedPreviously));
	}

	public void stopMoveObjectToPlaceCoroutine ()
	{
		if (placeObjectCoroutine != null) {
			StopCoroutine (placeObjectCoroutine);
		}
	}

	IEnumerator placeObjectIntoPosition (bool objectPlacedPreviously)
	{
		grabbedObjectState currentGrabbedObject = currentObjectPlaced.GetComponent<grabbedObjectState> ();

		bool objectCanBePlaced = true;

		if (currentGrabbedObject != null) {
			objectCanBePlaced = GKC_Utils.checkIfObjectCanBePlaced (currentGrabbedObject.getCurrentHolder (), currentObjectPlaced);

			if (!objectCanBePlaced) {
				cancelPlaceObjectActive = true;
			}
		}

		if (objectCanBePlaced) {
			bool objectCanCallPlacedEvent = currentObjectToPlaceSystem.canObjectCanCallPlacedEvent ();

			if (!objectPlacedPreviously) {
				if (useSoundEffectOnObjectPlaced && objectCanCallPlacedEvent) {
					playSound (soundEffectOnObjectPlaced);
				}
			}
				
			if (currentGrabbedObject != null) {
				GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), currentObjectPlaced);
			}

			Rigidbody mainRigidbody = currentObjectPlaced.GetComponent<Rigidbody> ();

			if (mainRigidbody != null) {
				mainRigidbody.isKinematic = true;
			}

			currentObjectToPlaceSystem.assignPutObjectSystem (this);

			currentObjectToPlaceSystem.setObjectPlaceState (true);

			if (disableObjectOnceIsPlaced) {
				currentObjectToPlaceSystem.setObjectToPlacedEnabledState (false);

				grabPhysicalObjectSystem currentGrabPhysicalObjectSystem = currentObjectPlaced.GetComponent<grabPhysicalObjectSystem> ();

				if (currentGrabPhysicalObjectSystem != null) {
					currentGrabPhysicalObjectSystem.disableGrabPhysicalObject ();
				}
			}

			if (!objectPlacedPreviously) {
				if (!waitToObjectPlacedToCallEvent && objectCanCallPlacedEvent) {
					objectPlacedEvent.Invoke ();
				}
			}

			Transform objectTransform = currentObjectPlaced.transform;

			objectTransform.SetParent (placeToPutObject);

			Vector3 targetPosition = Vector3.zero;
			Quaternion targetRotation = Quaternion.identity;

			float dist = GKC_Utils.distance (objectTransform.position, placeToPutObject.position);
			float duration = dist / placeObjectPositionSpeed;
			float t = 0;

			float timePassed = 0;

			while ((t < 1 || objectTransform.localPosition != targetPosition || objectTransform.localRotation != targetRotation) && timePassed < 3) {
				t += Time.deltaTime / duration;

				objectTransform.localPosition = Vector3.MoveTowards (objectTransform.localPosition, targetPosition, t);
				objectTransform.localRotation = Quaternion.Lerp (objectTransform.localRotation, targetRotation, t);

				timePassed += Time.deltaTime;

				yield return null;
			}

			if (!objectPlacedPreviously) {
				if (waitToObjectPlacedToCallEvent && objectCanCallPlacedEvent) {
					objectPlacedEvent.Invoke ();
				}
			}
		} else {
			stopMoveObjectToPlaceCoroutine ();

			resetPlacedObjectValues ();
		}
	}

	void resetPlacedObjectValues ()
	{
		objectPlaced = false;

		currentObjectToPlaceSystem = null;

		currentObjectPlaced = null;

		objectInsideTrigger = false;

		checkingIfObjectIsRemoved = false;
	}

	public void removePlacedObject ()
	{
		if (useLimitToCheckIfObjectRemoved) {
			checkingIfObjectIsRemoved = true;
		} else {
			removeObject ();
		}
	}

	public void dropCurrentObjectPlaced (bool reactivateObjectToUseOnPlaceSystem)
	{
		if (!putObjectSystemEnabled) {
			return;
		}

		if (currentObjectToPlaceSystem == null) {
			return;
		}

		Rigidbody currentRigidbody = currentObjectPlaced.GetComponent<Rigidbody> ();


		if (reactivateObjectToUseOnPlaceSystem) {
			currentObjectToPlaceSystem.setObjectToPlacedEnabledState (true);

			currentObjectToPlaceSystem.setObjectPlaceState (false);
		}


		grabPhysicalObjectSystem currentGrabPhysicalObjectSystem = currentObjectPlaced.GetComponent<grabPhysicalObjectSystem> ();

		if (currentGrabPhysicalObjectSystem != null) {
			currentGrabPhysicalObjectSystem.setGrabObjectPhysicallyEnabledState (true);
		}


		removeObject ();

		if (currentRigidbody != null) {
			currentRigidbody.isKinematic = false;

			currentRigidbody.AddForce (placeToPutObject.forward * 10, ForceMode.Impulse);
		}
	}

	public void removeObject ()
	{
		if (!putObjectSystemEnabled) {
			return;
		}

		if (currentObjectToPlaceSystem == null) {
			return;
		}

		bool objectCanCallRemovedEvent = currentObjectToPlaceSystem.canObjectCanCallRemovedEvent ();

		if (objectCanCallRemovedEvent && useSoundEffectOnObjectRemoved) {
			playSound (soundEffectOnObjectRemoved);
		}

		stopMoveObjectToPlaceCoroutine ();

		objectPlaced = false;

		if (objectCanCallRemovedEvent) {
			objectRemovedEvent.Invoke ();
		}

		currentObjectToPlaceSystem = null;

		currentObjectPlaced.transform.SetParent (null);

		currentObjectPlaced = null;

		objectInsideTrigger = false;

		checkingIfObjectIsRemoved = false;
	}

	public void disableObjectPlacedFromGrab ()
	{
		if (currentObjectPlaced != null) {
			currentObjectPlaced.tag = "Untagged";
		}
	}

	public void increaseNumberObjectsPlaced ()
	{
		currentNumberObjectsPlaced++;
	}

	public void decreaseNumberObjectsPlaced ()
	{
		currentNumberObjectsPlaced--;

		if (currentNumberObjectsPlaced < 0) {
			currentNumberObjectsPlaced = 0;
		}
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

	public void playSound (AudioClip soundToPlay)
	{
		if (mainAudioSource != null) {
			mainAudioSource.PlayOneShot (soundToPlay);
		}
	}

	public void pausePutObject ()
	{
		stopPausePutObjectCoroutine ();

		pauseCoroutine = StartCoroutine (pausePutObjectCoroutine ());
	}

	public void stopPausePutObjectCoroutine ()
	{
		if (pauseCoroutine != null) {
			StopCoroutine (pauseCoroutine);
		}
	}

	IEnumerator pausePutObjectCoroutine ()
	{
		yield return new WaitForSeconds (minWaitToPutSameObjectAgain);

		if (mainTrigger == null) {
			mainTrigger = GetComponent<Collider> ();
		}

		if (mainTrigger != null) {
			mainTrigger.enabled = false;
			mainTrigger.enabled = true;
		}
	}
}