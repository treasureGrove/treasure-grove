using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class attrackObjectsSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool attractionEnabled;

	public bool attractionApplyForce = true;
	public bool attractionActive;
	public float attractionRadius = 20;

	public LayerMask layerToSearch;

	public float attractionVelocity = 10;
	public float attractionForce = 1000;
	public ForceMode forceMode;

	public float maxThrowForce = 3500;
	public float increaseThrowForceSpeed = 1500;

	public List<string> tagToLocate = new List<string> ();

	public LayerMask layerToDamage;

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

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventsOnActivateAttraction;
	public UnityEvent eventOnDeactivateAttraction;

	[Space]
	[Header ("Components")]
	[Space]

	public Slider powerSlider;
	public Transform mainCameraTransform;
	public Transform attractionPosition;
	public Collider playerCollider;
	public GameObject playerGameObject;

	public playerWeaponSystem mainPlayerWeaponSystem;

	float currentForceToLaunchObjects;

	Rigidbody currentRigidbody;

	Vector3 attractDirection;

	Vector3 currentPosition;

	RaycastHit hit;

	List<grabbedObject> grabbedObjectList = new List<grabbedObject> ();

	float lastTimeActivateAttraction;

	Vector3 nextObjectHeldPosition;
	Vector3 currentObjectHeldPosition;

	Coroutine grabObjectsCoroutine;

	void Awake ()
	{
		if (mainCameraTransform == null) {
			mainCameraTransform = transform;
		}

		if (playerGameObject == null) {
			playerGameObject = gameObject;
		}
	}

	void FixedUpdate ()
	{
		if (attractionActive) {

			if (!carryingObjects) {
				return;
			}

			currentPosition = attractionPosition.position;

			for (int i = 0; i < grabbedObjectList.Count; i++) {
				if (grabbedObjectList [i].objectToMove != null) {
					currentRigidbody = grabbedObjectList [i].mainRigidbody;
						
					if (currentRigidbody) {

						if (attractionApplyForce) {
							attractDirection = currentPosition - currentRigidbody.position;
							currentRigidbody.AddForce (attractDirection.normalized * (attractionForce * currentRigidbody.mass * Time.fixedDeltaTime), forceMode);
						} else {
							nextObjectHeldPosition = currentPosition + attractionPosition.forward;
							
							currentObjectHeldPosition = currentRigidbody.position;

							currentRigidbody.velocity = (nextObjectHeldPosition - currentObjectHeldPosition) * attractionVelocity;
						}
					}
				}
			}
		}
	}

	public void setAttractionEnabledState (bool state)
	{
		attractionEnabled = state;
	}

	public void checkGrabObject ()
	{
		if (grabObjectsCoroutine != null) {
			StopCoroutine (grabObjectsCoroutine);
		}

		grabObjectsCoroutine = StartCoroutine (checkGrabObjectCoroutine ());
	}

	IEnumerator checkGrabObjectCoroutine ()
	{
		yield return new WaitForSeconds (0.05f);

		if (attractionEnabled && !carryingObjects) {
			lastTimeActivateAttraction = Time.time;

			initializeComponents ();

			attractionActive = true;

			grabCloseObjects ();

			if (carryingObjects) {
				eventsOnActivateAttraction.Invoke ();
			}
		}
	}

	public void inputGrabObjects ()
	{
		checkGrabObject ();
	}

	public void inputHoldToLaunchObject ()
	{
		if (!attractionEnabled) {
			return;
		}

		if (carryingObjects) {
			if (Time.time > lastTimeActivateAttraction + 0.5f) {
				addForceToLaunchObjects ();
			}
		}
	}

	public void inputReleaseToLaunchObject ()
	{
		if (!attractionEnabled) {
			return;
		}

		if (carryingObjects) {
			if (Time.time > lastTimeActivateAttraction + 0.5f) {
				dropObjects ();

				attractionActive = false;

				eventOnDeactivateAttraction.Invoke ();
			}
		}
	}

	public void grabCloseObjects ()
	{
		//if the player has not grabbedObjects, store them
		if (grabbedObjectList.Count == 0) {
			//check in a radius, the close objects which can be grabbed
			currentPosition = attractionPosition.position;

			int ignoreRaycastLayerIndex = LayerMask.NameToLayer ("Ignore Raycast");

			Collider[] objects = Physics.OverlapSphere (currentPosition, attractionRadius, layerToSearch);
			foreach (Collider currentCollider in objects) {
				
				Rigidbody currentRigidbody = currentCollider.GetComponent<Rigidbody> ();

				if (tagToLocate.Contains (currentCollider.tag) && currentRigidbody) {
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
					if (playerCollider != null) {
						Physics.IgnoreCollision (currentCollider, playerCollider, true);
					}

					//if any object grabbed has its own gravity, paused the script to move the object properly
					artificialObjectGravity currentArtificialObjectGravity = currentObject.GetComponent<artificialObjectGravity> ();

					if (currentArtificialObjectGravity) {
						currentArtificialObjectGravity.active = false;
					}

					grabObjectProperties currentGrabObjectProperties = currentObject.GetComponent<grabObjectProperties> ();

					if (currentGrabObjectProperties) {
						currentGrabObjectProperties.checkEventsOnGrabObject ();
					}

					grabbedObjectState currentGrabbedObjectState = currentObject.GetComponent<grabbedObjectState> ();

					if (!currentGrabbedObjectState) {
						currentGrabbedObjectState = currentObject.AddComponent<grabbedObjectState> ();
					}

					objectToPlaceSystem currentObjectToPlaceSystem = currentObject.GetComponent<objectToPlaceSystem> ();

					if (currentObjectToPlaceSystem) {
						currentObjectToPlaceSystem.setObjectInGrabbedState (true);
					}

					//if any object is pickable and is inside an opened chest, activate its trigger or if it has been grabbed by the player, remove of the list
					pickUpObject currentPickUpObject = currentObject.GetComponent<pickUpObject> ();

					if (currentPickUpObject) {
						currentPickUpObject.activateObjectTrigger ();
					}

					deviceStringAction currentDeviceStringAction = currentObject.GetComponentInChildren<deviceStringAction> ();

					if (currentDeviceStringAction) {
						currentDeviceStringAction.setIconEnabledState (false);
					}

					if (currentGrabbedObjectState) {
						currentGrabbedObjectState.setCurrentHolder (gameObject);

						currentGrabbedObjectState.setGrabbedState (true);
					}

					grabbedObjectList.Add (newGrabbedObject);
				}
			}

			//if there are not any object close to the player, cancel 
			if (grabbedObjectList.Count > 0) {
				carryingObjects = true;
			} else {
				attractionActive = false;
			}

			powerSlider.maxValue = maxThrowForce;

			currentForceToLaunchObjects = 0;
		} 
	}

	//drop or throw the current grabbed objects
	public void dropObjects ()
	{
		//get the point at which the camera is looking, to throw the objects in that direction
		Vector3 hitDirection = Vector3.zero;

		bool surfaceFound = false;

		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layerToDamage)) {
			if (hit.collider != playerCollider) {
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

		powerSlider.value = 0;
	}

	public void addForceToLaunchObjects ()
	{
		if (currentForceToLaunchObjects < maxThrowForce) {
			//enable the power slider in the center of the screen
			currentForceToLaunchObjects += Time.deltaTime * increaseThrowForceSpeed;
			if (currentForceToLaunchObjects > 300) {
				
				powerSlider.value = currentForceToLaunchObjects;
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
		if (currentForceToLaunchObjects < 300) {
			if (playerCollider != null) {
				Physics.IgnoreCollision (currentObject.GetComponent<Collider> (), playerCollider, false);
			}
		}

		//launch the objects according to the amount of time that the player has held the buttton
		if (currentForceToLaunchObjects > 300) {
			//if the objects are launched, add the script launchedObject, to damage any enemy that the object would touch

			currentObject.AddComponent<launchedObjects> ().setCurrentPlayerAndCollider (playerGameObject, playerCollider);

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

		if (currentArtificialObjectGravity) {
			currentArtificialObjectGravity.active = true;
		}

		grabObjectProperties currentGrabObjectProperties = currentObject.GetComponent<grabObjectProperties> ();

		if (currentGrabObjectProperties) {
			currentGrabObjectProperties.checkEventsOnDropObject ();
		}
	
		deviceStringAction currentDeviceStringAction = currentObject.GetComponentInChildren<deviceStringAction> ();

		if (currentDeviceStringAction) {
			currentDeviceStringAction.setIconEnabledState (true);
		}

		objectToPlaceSystem currentObjectToPlaceSystem = currentObject.GetComponent<objectToPlaceSystem> ();

		if (currentObjectToPlaceSystem) {
			currentObjectToPlaceSystem.setObjectInGrabbedState (false);
		}

		grabbedObjectState currentGrabbedObjectState = currentObject.GetComponent<grabbedObjectState> ();

		if (currentGrabbedObjectState) {
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


	bool componentsInitialized;

	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}

		if (mainPlayerWeaponSystem != null) {
			playerGameObject = mainPlayerWeaponSystem.getPlayerWeaponsManger ().gameObject;

			playerComponentsManager mainPlayerComponentsManager = playerGameObject.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				playerCollider = mainPlayerComponentsManager.getPlayerController ().getMainCollider ();

				mainCameraTransform = mainPlayerComponentsManager.getPlayerCamera ().getCameraTransform ();
			}
		}

		componentsInitialized = true;
	}

	[System.Serializable]
	public class grabbedObject
	{
		public GameObject objectToMove;
		public string objectTag;
		public int objectLayer;
		public Rigidbody mainRigidbody;
	}
}
