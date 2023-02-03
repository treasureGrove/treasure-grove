using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class grabObjectsPowerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool grabObjectsEnabled;

	public float grabRadius = 10;
	public List< string> ableToGrabTags = new List< string> ();

	public LayerMask layerToDamage;
	public LayerMask layerToDetectObjects;

	public float carryObjectsSpeed = 5;

	public float minForceToLaunchObjects = 300;
	public float maxForceToLaunchObjects = 3500;

	public float addForceToLaunchObjectsSpeed = 1200;

	public float waitTimeToDropObjects = 1;

	public bool useCarryObjectsPositionOffset;
	public Vector3 carryObjectsPositionOffset;

	[Space]
	[Header ("Grab Single Object Settings")]
	[Space]

	public bool grabSingleObjectOnCameraView;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameListOnGrabObject = new List<string> ();
	public List<string> remoteEventNameListOnDropObject = new List<string> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool carryingObjects;

	public bool firstPersonActive;
	public bool firstPersonPreviouslyActive;

	public List<grabbedObject> grabbedObjectList = new List<grabbedObject> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnThrowObjects;
	public UnityEvent eventOnThrowObjects;
	public UnityEvent eventOnDropObjects;

	[Space]
	[Header ("Components")]
	[Space]

	public grabObjects grabObjectsManager;

	public Transform mainCameraTransform;

	public otherPowers mainOtherPowers;

	public Transform carryObjectsTransform;
	public List<Transform> carryObjectsTransformFirstPersonList = new List<Transform> ();
	public List<Transform> carryObjectsTransformThirdPersonList = new List<Transform> ();

	public bool useCarryObjectsAnimation = true;
	public Animation carryObjectsTransformAnimation;
	public string carryObjectsAnimationName = "grabObjects";

	public Slider slider;

	public Collider mainPlayerCollider;

	Transform closestCarryObjecsTransformFirstPerson;
	Transform closestCarryObjecsTransformThirdPerson;

	float currentForceToLaunchObjects;

	bool playerCurrentlyBusy;

	RaycastHit hit;

	bool canMove;

	Vector3 currentObjectPosition;
	Vector3 nextObjectPosition;

	float currentObjectDistance;

	grabbedObject currentGrabbedObject;

	float lastTimeObjectsGrabbed;

	void FixedUpdate ()
	{
		playerCurrentlyBusy = mainOtherPowers.playerIsBusy ();

		canMove = mainOtherPowers.canMove;

		if (carryingObjects) {
			if (grabbedObjectList.Count > 0) {

				firstPersonActive = mainOtherPowers.firstPersonActive;

				if (firstPersonActive != firstPersonPreviouslyActive) {
					firstPersonPreviouslyActive = firstPersonActive;

					for (int i = 0; i < grabbedObjectList.Count; i++) {
						currentGrabbedObject = grabbedObjectList [i];

						if (currentGrabbedObject.objectToMove != null) {
							if (firstPersonActive) {
								currentGrabbedObject.objectToMove.transform.SetParent (currentGrabbedObject.objectToFollowFirstPerson);
							} else {
								currentGrabbedObject.objectToMove.transform.SetParent (currentGrabbedObject.objectToFollowThirdPerson);
							}
						}
					}
				}

				//when all the objects are stored, then set their position close to the player
				for (int k = 0; k < grabbedObjectList.Count; k++) {
					currentGrabbedObject = grabbedObjectList [k];

					if (currentGrabbedObject.objectToMove != null) {

						currentObjectDistance = GKC_Utils.distance (currentGrabbedObject.objectToMove.transform.localPosition, Vector3.zero);

						if (currentObjectDistance > 0.8f) {

							if (useCarryObjectsPositionOffset) {
								if (firstPersonActive) {
									nextObjectPosition = currentGrabbedObject.objectToFollowFirstPerson.position;

									nextObjectPosition += currentGrabbedObject.objectToFollowFirstPerson.TransformDirection (carryObjectsPositionOffset);
								} else {
									nextObjectPosition = currentGrabbedObject.objectToFollowThirdPerson.position;

									nextObjectPosition += currentGrabbedObject.objectToFollowThirdPerson.TransformVector (carryObjectsPositionOffset);
								}
							} else {
								nextObjectPosition = currentGrabbedObject.objectToFollowThirdPerson.position;

								if (firstPersonActive) {
									nextObjectPosition = currentGrabbedObject.objectToFollowFirstPerson.position;
								}
							}

							currentObjectPosition = currentGrabbedObject.objectToMove.transform.position;

							currentGrabbedObject.mainRigidbody.velocity = (nextObjectPosition - currentObjectPosition) * carryObjectsSpeed;
						} else {
							currentGrabbedObject.mainRigidbody.velocity = Vector3.zero;
						}
					} else {
						grabbedObjectList.RemoveAt (k);
					}
				}
			} else {
				carryingObjects = false;
			}
		}
	}

	public void grabCloseObjects ()
	{
		//if the player has not grabbedObjects, store them
		if (grabbedObjectList.Count == 0) {
			int ignoreRaycastLayerIndex = LayerMask.NameToLayer ("Ignore Raycast");

			//check in a radius, the close objects which can be grabbed
			Collider[] objects = Physics.OverlapSphere (carryObjectsTransform.position + transform.up, grabRadius, layerToDetectObjects);

			foreach (Collider currentCollider in objects) {
				checkObjectToGrab (currentCollider, ignoreRaycastLayerIndex);
			}

			//if there are not any object close to the player, cancel 
			if (grabbedObjectList.Count > 0) {
				carryingObjects = true;

				lastTimeObjectsGrabbed = Time.time;
			}
		} 
	}

	public void grabSingleObject ()
	{
		if (grabbedObjectList.Count == 0) {
			bool surfaceFound = false;

			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
				if (hit.collider != mainPlayerCollider) {
					surfaceFound = true;
				} else {
					if (Physics.Raycast (hit.point + mainCameraTransform.forward, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
						surfaceFound = true;
					}
				}
			}

			if (surfaceFound) {
				int ignoreRaycastLayerIndex = LayerMask.NameToLayer ("Ignore Raycast");

				checkObjectToGrab (hit.collider, ignoreRaycastLayerIndex);

				//if there are not any object close to the player, cancel 
				if (grabbedObjectList.Count > 0) {
					carryingObjects = true;

					lastTimeObjectsGrabbed = Time.time;
				}
			}
		} 
	}

	void checkObjectToGrab (Collider currentCollider, int ignoreRaycastLayerIndex)
	{
		Rigidbody currentRigidbody = currentCollider.GetComponent<Rigidbody> ();

		if (ableToGrabTags.Contains (currentCollider.tag) && currentRigidbody != null) {
			if (currentRigidbody.isKinematic) {
				currentRigidbody.isKinematic = false;
			}

			grabbedObject newGrabbedObject = new grabbedObject ();

			//removed tag and layer after store them, so the camera can still use raycast properly
			GameObject currentObject = currentCollider.gameObject;

			newGrabbedObject.objectToMove = currentObject;
			newGrabbedObject.objectTag = currentObject.tag;
			newGrabbedObject.objectLayer = currentObject.layer;
			newGrabbedObject.mainRigidbody = currentRigidbody;
			newGrabbedObject.objectCollider = currentCollider;

			if (useRemoteEventOnObjectsFound) {
				remoteEventSystem currentRemoteEventSystem = currentObject.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameListOnGrabObject.Count; i++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnGrabObject [i]);
					}
				}
			}

			currentObject.tag = "Untagged";

			currentObject.layer = ignoreRaycastLayerIndex;

			currentRigidbody.useGravity = false;

			//get the distance from every object to left and right side of the player, to set every side as parent of every object
			//disable collisions between the player and the objects, to avoid issues
			Physics.IgnoreCollision (currentCollider, mainPlayerCollider, true);

			float distance = Mathf.Infinity;

			for (int k = 0; k < carryObjectsTransformFirstPersonList.Count; k++) {
				float currentDistance = GKC_Utils.distance (currentObject.transform.position, carryObjectsTransformFirstPersonList [k].position);

				if (currentDistance < distance) {
					distance = currentDistance;
					closestCarryObjecsTransformFirstPerson = carryObjectsTransformFirstPersonList [k];
				}
			}

			if (closestCarryObjecsTransformFirstPerson != null) {
				currentObject.transform.SetParent (closestCarryObjecsTransformFirstPerson);
				newGrabbedObject.objectToFollowFirstPerson = closestCarryObjecsTransformFirstPerson;
				closestCarryObjecsTransformFirstPerson = null;
			}

			distance = Mathf.Infinity;

			for (int k = 0; k < carryObjectsTransformThirdPersonList.Count; k++) {
				float currentDistance = GKC_Utils.distance (currentObject.transform.position, carryObjectsTransformThirdPersonList [k].position);

				if (currentDistance < distance) {
					distance = currentDistance;
					closestCarryObjecsTransformThirdPerson = carryObjectsTransformThirdPersonList [k];
				}
			}

			if (closestCarryObjecsTransformThirdPerson != null) {
				currentObject.transform.SetParent (closestCarryObjecsTransformThirdPerson);

				newGrabbedObject.objectToFollowThirdPerson = closestCarryObjecsTransformThirdPerson;

				closestCarryObjecsTransformThirdPerson = null;
			}

			//if any object grabbed has its own gravity, paused the script to move the object properly
			artificialObjectGravity currentArtificialObjectGravity = currentObject.GetComponent<artificialObjectGravity> ();

			if (currentArtificialObjectGravity != null) {
				currentArtificialObjectGravity.active = false;
			}

			grabObjectProperties currentGrabObjectProperties = currentObject.GetComponent<grabObjectProperties> ();

			if (currentGrabObjectProperties != null) {
				currentGrabObjectProperties.checkEventsOnGrabObject ();
			}

			grabbedObjectState currentGrabbedObjectState = currentObject.GetComponent<grabbedObjectState> ();

			if (currentGrabbedObjectState == null) {
				currentGrabbedObjectState = currentObject.AddComponent<grabbedObjectState> ();
			}

			objectToPlaceSystem currentObjectToPlaceSystem = currentObject.GetComponent<objectToPlaceSystem> ();

			if (currentObjectToPlaceSystem != null) {
				currentObjectToPlaceSystem.setObjectInGrabbedState (true);
			}

			//if any object is pickable and is inside an opened chest, activate its trigger or if it has been grabbed by the player, remove of the list
			pickUpObject currentPickUpObject = currentObject.GetComponent<pickUpObject> ();

			if (currentPickUpObject != null) {
				currentPickUpObject.activateObjectTrigger ();
			}

			deviceStringAction currentDeviceStringAction = currentObject.GetComponentInChildren<deviceStringAction> ();

			if (currentDeviceStringAction != null) {
				currentDeviceStringAction.setIconEnabledState (false);
			}

			if (currentGrabbedObjectState != null) {
				currentGrabbedObjectState.setCurrentHolder (gameObject);
				currentGrabbedObjectState.setGrabbedState (true);
			}

			grabbedObjectList.Add (newGrabbedObject);
		}
	}

	//drop or throw the current grabbed objects
	public void dropObjects ()
	{
		//get the point at which the camera is looking, to throw the objects in that direction
		Vector3 hitDirection = Vector3.zero;

		bool surfaceFound = false;

		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
			if (hit.collider != mainPlayerCollider) {
				surfaceFound = true;
			} else {
				if (Physics.Raycast (hit.point + mainCameraTransform.forward, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
					surfaceFound = true;
				}
			}
		}

		if (surfaceFound) {
			hitDirection = hit.point;
		}

		for (int j = 0; j < grabbedObjectList.Count; j++) {
			dropObject (grabbedObjectList [j], hitDirection);
		}

		carryingObjects = false;

		grabbedObjectList.Clear ();

		mainOtherPowers.enableOrDisablePowerCursor (false);

		if (slider) {
			slider.gameObject.SetActive (false);
			slider.value = 0;
		}
	}

	public void addForceToLaunchObjects ()
	{
		if (Time.time > waitTimeToDropObjects + lastTimeObjectsGrabbed) {
			if (currentForceToLaunchObjects < maxForceToLaunchObjects) {
				//enable the power slider in the center of the screen
				currentForceToLaunchObjects += Time.deltaTime * addForceToLaunchObjectsSpeed;

				if (currentForceToLaunchObjects > minForceToLaunchObjects) {

					if (slider) {
						slider.value = currentForceToLaunchObjects;
						if (!slider.gameObject.activeSelf) {
							slider.gameObject.SetActive (true);
						}
					}

					mainOtherPowers.enableOrDisablePowerCursor (true);
				}
			}
		}
	}

	public void dropObject (grabbedObject currentGrabbedObject, Vector3 launchDirection)
	{
		GameObject currentObject = currentGrabbedObject.objectToMove;

		Rigidbody currentRigidbody = currentGrabbedObject.mainRigidbody;

		if (useRemoteEventOnObjectsFound) {
			remoteEventSystem currentRemoteEventSystem = currentObject.GetComponent<remoteEventSystem> ();

			if (currentRemoteEventSystem != null) {
				for (int i = 0; i < remoteEventNameListOnDropObject.Count; i++) {

					currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnDropObject [i]);
				}
			}
		}

		currentObject.transform.SetParent (null);

		currentObject.tag = currentGrabbedObject.objectTag;
		currentObject.layer = currentGrabbedObject.objectLayer;

		//drop the objects, because the grab objects button has been pressed quickly
		if (currentForceToLaunchObjects < minForceToLaunchObjects) {
			Physics.IgnoreCollision (currentGrabbedObject.objectCollider, mainPlayerCollider, false);
		}

		//launch the objects according to the amount of time that the player has held the buttton
		if (currentForceToLaunchObjects > minForceToLaunchObjects) {
			//if the objects are launched, add the script launchedObject, to damage any enemy that the object would touch
			currentObject.AddComponent<launchedObjects> ().setCurrentPlayerAndCollider (gameObject, mainPlayerCollider);

			//if there are any collider in from of the camera, use the hit point, else, use the camera direciton
			if (launchDirection != Vector3.zero) {
				Vector3 throwDirection = launchDirection - currentObject.transform.position;
				throwDirection = throwDirection / throwDirection.magnitude;
				currentRigidbody.AddForce (throwDirection * currentForceToLaunchObjects * currentRigidbody.mass);
			} else {
				currentRigidbody.AddForce (mainCameraTransform.TransformDirection (Vector3.forward) * currentForceToLaunchObjects * currentRigidbody.mass);
			}
		}

		//set again the custom gravity of the object
		artificialObjectGravity currentArtificialObjectGravity = currentObject.GetComponent<artificialObjectGravity> ();

		if (currentArtificialObjectGravity != null) {
			currentArtificialObjectGravity.active = true;
		}

		grabObjectProperties currentGrabObjectProperties = currentObject.GetComponent<grabObjectProperties> ();

		if (currentGrabObjectProperties != null) {
			currentGrabObjectProperties.checkEventsOnDropObject ();
		}

		deviceStringAction currentDeviceStringAction = currentObject.GetComponentInChildren<deviceStringAction> ();

		if (currentDeviceStringAction != null) {
			currentDeviceStringAction.setIconEnabledState (true);
		}

		objectToPlaceSystem currentObjectToPlaceSystem = currentObject.GetComponent<objectToPlaceSystem> ();

		if (currentObjectToPlaceSystem != null) {
			currentObjectToPlaceSystem.setObjectInGrabbedState (false);
		}

		grabbedObjectState currentGrabbedObjectState = currentObject.GetComponent<grabbedObjectState> ();

		if (currentGrabbedObjectState != null) {
			bool currentObjectWasInsideGravityRoom = currentGrabbedObjectState.isInsideZeroGravityRoom ();

			currentGrabbedObjectState.checkGravityRoomState ();

			currentGrabbedObjectState.setGrabbedState (false);

			if (!currentObjectWasInsideGravityRoom) {
				currentGrabbedObjectState.removeGrabbedObjectComponent ();
				currentRigidbody.useGravity = true;
			}
		}
	}

	public void checkIfDropObject (GameObject objectToCheck)
	{
		for (int j = 0; j < grabbedObjectList.Count; j++) {
			if (grabbedObjectList [j].objectToMove == objectToCheck) {
				dropObject (grabbedObjectList [j], Vector3.zero);

				grabbedObjectList [j].objectToMove = null;

				return;
			}
		}
	}

	public void setGrabObjectsEnabledState (bool state)
	{
		grabObjectsEnabled = state;
	}

	public void inputGrabObjects ()
	{
		if (!playerCurrentlyBusy) {
			if (!carryingObjects && canMove && grabObjectsEnabled && !grabObjectsManager.isCarryingPhysicalObject ()) {

				if (useCarryObjectsAnimation) {
					if (carryObjectsTransformAnimation != null) {
						carryObjectsTransformAnimation.Play (carryObjectsAnimationName);
					}
				}

				lastTimeObjectsGrabbed = 0;

				if (grabSingleObjectOnCameraView) {
					grabSingleObject ();
				} else {
					grabCloseObjects ();
				}

				currentForceToLaunchObjects = 0;
			}
		}
	}

	public void inputHoldToLaunchObjects ()
	{
		//the objects can be dropped or launched, according to the duration of the key press, in the camera direction
		if (!playerCurrentlyBusy && carryingObjects && canMove) {
			addForceToLaunchObjects ();
		}
	}

	public void inputReleaseToLaunchObjects ()
	{
		//drop or thrown the objects
		if (!playerCurrentlyBusy && carryingObjects && canMove) {
			if (Time.time > waitTimeToDropObjects + lastTimeObjectsGrabbed) {
				dropObjects ();

				if (useEventsOnThrowObjects) {
					if (currentForceToLaunchObjects > minForceToLaunchObjects) {
						eventOnThrowObjects.Invoke ();
					} else {
						eventOnDropObjects.Invoke ();
					}
				}
			}
		}
	}

	[System.Serializable]
	public class grabbedObject
	{
		public GameObject objectToMove;
		public Transform objectToFollowFirstPerson;
		public Transform objectToFollowThirdPerson;
		public string objectTag;
		public int objectLayer;
		public Rigidbody mainRigidbody;
		public Collider objectCollider;
	}
}