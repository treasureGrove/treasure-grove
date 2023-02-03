using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class gravityGun : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float holdDistance = 3;
	public float maxDistanceHeld = 4;
	public float maxDistanceGrab = 10;
	public float holdSpeed = 10;

	public float rotationSpeed;

	public grabMode currentGrabMode;

	public string grabbedObjectTag;
	public string grabbedObjectLayer;

	public LayerMask layer;

	public LayerMask gravityObjectsLayer;
	public string layerForCustomGravityObject;

	public bool changeGravityObjectsEnabled = true;

	public List<string> ableToGrabTags = new List<string> ();

	public float rotorRotationSpeed = 20;
	public float rotorRotationSpeedOnThrowCharge = 40;

	[Space]
	[Header ("Throw Object Settings")]
	[Space]

	public float minTimeToIncreaseThrowForce = 300;
	public float increaseThrowForceSpeed = 1500;
	public float extraThorwForce = 10;
	public float maxThrowForce = 3500;

	public ForceMode powerForceMode;

	public bool useThrowObjectsLayer = true;
	public LayerMask throwObjectsLayerToCheck;

	public float throwPower;
	public ForceMode realisticForceMode;

	[Space]
	[Header ("Grab In Fixed Position Settings")]
	[Space]

	public bool grabInFixedPosition;
	public bool rotateToCameraInFixedPosition;
	public bool rotateToCameraInFreePosition;

	public float closestHoldDistanceInFixedPosition;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameListOnGrabObject = new List<string> ();
	public List<string> remoteEventNameListOnDropObject = new List<string> ();

	[Space]
	[Header ("Drop Settings")]
	[Space]

	public bool useForceWhenObjectDropped;
	public bool useForceWhenObjectDroppedOnFirstPerson;
	public bool useForceWhenObjectDroppedOnThirdPerson;
	public float forceWhenObjectDroppedOnFirstPerson;
	public float forceWhenObjectDroppedOnThirdPerson;

	[Space]
	[Header ("Transparency Settings")]
	[Space]

	public float alphaTransparency = 0.5f;
	public bool enableTransparency = true;
	public Shader pickableShader;

	[Space]
	[Header ("Noise Settings")]
	[Space]

	public bool launchedObjectsCanMakeNoise;
	public float minObjectSpeedToActivateNoise;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool canUseWeapon = true;

	public bool objectIsVehicle;

	public bool objectFocus;

	public bool grabbed;

	public float currentMaxDistanceHeld;
	public float currentDistanceToGrabbedObject;

	public bool chargingThrowObject;

	public GameObject objectHeld;
	public GameObject currentObjectToGrabFound;

	public bool weaponActive;

	[Space]
	[Header ("Main Events Settings")]
	[Space]

	public bool useEventOnObjectFound;
	public UnityEvent eventOnObjectFound;
	public UnityEvent eventOnObjectLost;

	public bool useEventOnObjectGrabbedDropped;
	public UnityEvent eventOnObjectGrabbed;
	public UnityEvent eventOnObjectDropped;

	public bool useEventOnLaunchObjects;
	public UnityEvent eventOnLaunchObjects;

	public bool useEventOnChangeGravityGunActiveState;
	public UnityEvent eventOnGravityGunActive;
	public UnityEvent eventOnGravityGunDeactivate;

	[Space]
	[Header ("Secondary Events Settings")]
	[Space]

	public UnityEvent secondaryFunctionEventPressDown;
	public UnityEvent secondaryFunctionEventPress;
	public UnityEvent secondaryFunctionEventPressUp;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController playerControllerManager;
	public playerCamera playerCameraManager;

	public playerWeaponSystem mainPlayerWeaponSystem;

	public Transform mainCameraTransform;

	public simpleAnimationSystem animationSystem;

	public Slider powerSlider;

	public bool useRotor = true;
	public Transform rotor;
	public Transform grabZoneTransform;

	public PhysicMaterial highFrictionMaterial;

	public enum grabMode
	{
		powers,
		realistic
	}

	Rigidbody objectHeldRigidbody;
	float holdTimer = 0;
	float timer = 0;
	RaycastHit hit;

	bool grabbedObjectTagLayerStored;
	string originalGrabbedObjectTag;
	int originalGrabbedObjectLayer;

	float orignalHoldDistance;

	Transform fixedGrabedTransform;
	RigidbodyConstraints objectHeldRigidbodyConstraints = RigidbodyConstraints.None;
	Transform objectHeldFollowTransform;

	Vector3 nextObjectHeldPosition;
	Vector3 currentObjectHeldPosition;

	GameObject currentObjectToThrow;
	Transform currentHoldTransform;
	artificialObjectGravity currentArtificialObjectGravity;
	vehicleGravityControl currentVehicleGravityControl;

	Vector2 axisValues;

	bool currentObjectWasInsideGravityRoom;

	grabbedObjectState currentGrabbedObjectState;

	float currentGrabExtraDistance;

	Vector3 throwObjectDirection;

	GameObject currentCharacterGrabbed;

	List<int> currentObjectGrabbedLayerList = new List<int> ();
	bool grabbedObjectIsRagdoll;

	void Start ()
	{
		orignalHoldDistance = holdDistance;

		powerSlider.maxValue = maxThrowForce;
		powerSlider.value = maxThrowForce;
	}

	void Update ()
	{
		// if an object is grabbed, then move it from its original position, to the other in front of the camera
		if (objectHeld != null) {

			if (useRotor) {
				if (chargingThrowObject) {
					rotor.Rotate (0, 0, Time.deltaTime * rotorRotationSpeedOnThrowCharge);	
				} else {
					rotor.Rotate (0, 0, Time.deltaTime * rotorRotationSpeed);
				}
			}

			//get the transform for the grabbed object to follow
			currentHoldTransform = mainCameraTransform;

			if (playerCameraManager.is2_5ViewActive ()) {
				currentHoldTransform = playerCameraManager.getCurrentLookDirection2_5d ();
				holdDistance = 0;
			}

			if (playerCameraManager.useTopDownView) {
				currentHoldTransform = playerCameraManager.getCurrentLookDirectionTopDown ();
				holdDistance = 0;
			}

			currentDistanceToGrabbedObject = GKC_Utils.distance (objectHeld.transform.position, currentHoldTransform.position);

			if (!grabbed) {
				timer += Time.deltaTime;

				if ((currentDistanceToGrabbedObject <= currentMaxDistanceHeld || grabInFixedPosition) && timer > 0.5f) {
					grabbed = true;
					timer = 0;
				}
			}

			//if the object is not capable to move in front of the camera, because for example is being blocked for a wall, drop it
			if (currentDistanceToGrabbedObject > (currentMaxDistanceHeld + 2) && grabbed) {
				dropObject ();
			} else {
				//if the object is a cube, a turret, or anything that can move freely, set its position in front of the camera
				if (grabInFixedPosition) {
					nextObjectHeldPosition = fixedGrabedTransform.position + fixedGrabedTransform.forward * holdDistance;
					currentObjectHeldPosition = objectHeld.transform.position;
				} else {
					if (playerCameraManager.is2_5ViewActive ()) {
						nextObjectHeldPosition = currentHoldTransform.position + mainCameraTransform.forward * holdDistance;
					} else {
						nextObjectHeldPosition = currentHoldTransform.position + mainCameraTransform.forward * (holdDistance + objectHeld.transform.localScale.x);
					}
					currentObjectHeldPosition = objectHeld.transform.position;
				}

				objectHeldRigidbody.velocity = (nextObjectHeldPosition - currentObjectHeldPosition) * holdSpeed;

				if ((rotateToCameraInFixedPosition && grabInFixedPosition) || (!grabInFixedPosition && rotateToCameraInFreePosition)) {
					objectHeld.transform.rotation = Quaternion.Slerp (objectHeld.transform.rotation, mainCameraTransform.rotation, Time.deltaTime * rotationSpeed);
				}
			}
		}
			
		if (weaponActive && objectHeld == null && canUseWeapon) {
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, maxDistanceGrab, layer)) {

				if (currentObjectToGrabFound != hit.collider.gameObject) {
					currentObjectToGrabFound = hit.collider.gameObject;

					if (checkTypeObject (currentObjectToGrabFound)) {
						GameObject mainObjectFound = applyDamage.getCharacterOrVehicle (currentObjectToGrabFound);

						if (mainObjectFound == null) {
							grabObjectParent currentGrabObjectParent = currentObjectToGrabFound.GetComponent<grabObjectParent> ();
							if (currentGrabObjectParent) {
								mainObjectFound = currentGrabObjectParent.getObjectToGrab ();
							} else {
								mainObjectFound = currentObjectToGrabFound;
							}
						}

						if (!objectFocus) {

							animationSystem.playForwardAnimation ();

							objectFocus = true;

							checkObjectFoundOrLostState ();
						}
					} else {
						if (objectFocus) {

							animationSystem.playBackwardAnimation ();

							objectFocus = false;

							checkObjectFoundOrLostState ();
						}
					}
				}
			} else {
				if (objectFocus) {

					animationSystem.playBackwardAnimation ();
					
					objectFocus = false;

					checkObjectFoundOrLostState ();

					currentObjectToGrabFound = null;
				}
			}
		}

		if (grabbed && objectHeld == null) {
			dropObject ();
		}
	}

	public void checkObjectFoundOrLostState ()
	{
		if (useEventOnObjectFound) {
			if (objectFocus) {
				eventOnObjectFound.Invoke ();
			} else {
				eventOnObjectLost.Invoke ();
			}
		}
	}

	public void grabObject ()
	{
		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, maxDistanceGrab, layer) && objectFocus) {
			grabCurrenObject (hit.collider.gameObject);
		} 
	}

	public void grabCurrenObject (GameObject objectToGrab)
	{
		if (checkTypeObject (objectToGrab)) {
			
			//reset the hold distance
			holdDistance = orignalHoldDistance;

			currentGrabExtraDistance = 0;

			grabObjectProperties currentGrabObjectProperties = null;

			objectIsVehicle = applyDamage.isVehicle (objectToGrab);

			bool objectIsCharacter = applyDamage.isCharacter (objectToGrab);

			objectHeld = applyDamage.getCharacterOrVehicle (objectToGrab);

			if (objectHeld != null) {
				currentGrabObjectProperties = objectHeld.GetComponent<grabObjectProperties> ();
			}

			currentCharacterGrabbed = objectHeld;

			grabbedObjectIsRagdoll = false;

			if (objectIsVehicle || objectIsCharacter) {
				if (objectIsVehicle) {
					Rigidbody objectToGrabRigidbody = applyDamage.applyForce (objectHeld);

					if (objectToGrabRigidbody.isKinematic) {
						objectHeld = null;
						return;
					}
				} else {
					objectHeld = objectToGrab;

					if (applyDamage.isCharacter (objectHeld)) {
						Animator currentObjectAnimator = objectHeld.GetComponent<Animator> ();

						if (currentObjectAnimator != null) {
							objectHeld = currentObjectAnimator.GetBoneTransform (HumanBodyBones.Hips).gameObject;
							grabbedObjectIsRagdoll = true;
						}
					}
				}
			} else {
				grabObjectParent currentGrabObjectParent = objectToGrab.GetComponent<grabObjectParent> ();

				if (currentGrabObjectParent != null) {
					objectHeld = currentGrabObjectParent.getObjectToGrab ();
				} else {
					objectHeld = objectToGrab;
				}

				currentCharacterGrabbed = objectHeld;

				currentGrabObjectProperties = objectHeld.GetComponent<grabObjectProperties> ();
			}

			objectHeldRigidbody = objectHeld.GetComponent<Rigidbody> ();

			if (objectHeldRigidbody == null) {
				grabbed = false;
				objectHeld = null;
				currentCharacterGrabbed = null;

				objectFocus = false;

				return;
			}

//			print (currentCharacterGrabbed.name);

			if (useRemoteEventOnObjectsFound) {
				remoteEventSystem currentRemoteEventSystem = currentCharacterGrabbed.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameListOnGrabObject.Count; i++) {
						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnGrabObject [i]);
					}
				}
			}

			if (currentGrabObjectProperties != null) {
				if (currentGrabObjectProperties.useExtraGrabDistance) {
					currentGrabExtraDistance = currentGrabObjectProperties.getExtraGrabDistance ();

					holdDistance += currentGrabExtraDistance;
				}

				currentGrabObjectProperties.checkEventsOnGrabObject ();
			}

			//get its tag, to set it again to the object, when it is dropped
			if (!objectIsVehicle) {
				grabbedObjectTagLayerStored = true;
				originalGrabbedObjectTag = objectHeld.tag;

				if (grabbedObjectIsRagdoll) {
					currentObjectGrabbedLayerList = applyDamage.getBodyColliderLayerList (currentCharacterGrabbed);
					applyDamage.setBodyColliderLayerList (currentCharacterGrabbed, LayerMask.NameToLayer (grabbedObjectLayer));
				} else {
					originalGrabbedObjectLayer = objectHeld.layer;
				}

				objectHeld.tag = grabbedObjectTag;
				objectHeld.layer = LayerMask.NameToLayer (grabbedObjectLayer);
			}

			if (objectHeldRigidbody != null) {
				objectHeldRigidbody.isKinematic = false;
				objectHeldRigidbody.useGravity = false;
				objectHeldRigidbody.velocity = Vector3.zero;
			}

			//if the object has its gravity modified, pause that script
			currentArtificialObjectGravity = objectHeld.GetComponent<artificialObjectGravity> ();

			if (currentArtificialObjectGravity != null) {
				currentArtificialObjectGravity.active = false;
			}

			if (currentGrabObjectProperties != null) {
				currentGrabObjectProperties.checkEventToSetPlayer (playerControllerManager.gameObject);
			}

			pickUpObject currentPickUpObject = objectHeld.GetComponent<pickUpObject> ();

			if (currentPickUpObject != null) {
				currentPickUpObject.activateObjectTrigger ();
			}

			grabObjectEventSystem currentGrabObjectEventSystem = objectHeld.GetComponent<grabObjectEventSystem> ();

			if (currentGrabObjectEventSystem != null) {
				currentGrabObjectEventSystem.callEventOnGrab ();
			}

			objectToPlaceSystem currentObjectToPlaceSystem = objectHeld.GetComponent<objectToPlaceSystem> ();

			if (currentObjectToPlaceSystem != null) {
				currentObjectToPlaceSystem.setObjectInGrabbedState (true);
			}

			currentGrabbedObjectState = null;

			if (currentGrabMode == grabMode.powers) {
				if (objectHeldRigidbody != null) {
					objectHeldRigidbodyConstraints = objectHeldRigidbody.constraints;
					objectHeldRigidbody.freezeRotation = true;
				}

				currentGrabbedObjectState = objectHeld.GetComponent<grabbedObjectState> ();

				if (currentGrabbedObjectState == null) {
					currentGrabbedObjectState = objectHeld.AddComponent<grabbedObjectState> ();
				}

				if (currentGrabbedObjectState != null) {
					currentGrabbedObjectState.setCurrentHolder (playerControllerManager.gameObject);

					currentGrabbedObjectState.setGrabbedState (true);
				}
			} else {
				if (objectHeldRigidbody != null) {
					if (!objectIsVehicle) {
						objectHeldRigidbodyConstraints = objectHeldRigidbody.constraints;
						objectHeldRigidbody.freezeRotation = true;
					}
				}
			}
				
			if (objectHeld.GetComponent<pickUpObject> ()) {
				objectHeld.transform.SetParent (null);
			}
				
			//if the transparency is enabled, change all the color of all the materials of the object
			if (enableTransparency) {
				outlineObjectSystem currentOutlineObjectSystem = objectHeld.GetComponentInChildren<outlineObjectSystem> ();

				if (currentOutlineObjectSystem != null) {
					currentOutlineObjectSystem.setTransparencyState (true, pickableShader, alphaTransparency);
				}
			}
				
			powerSlider.value = 0;
			powerSlider.maxValue = maxThrowForce;

			holdTimer = 0;
				
			if (fixedGrabedTransform == null) {
				GameObject fixedPositionObject = new GameObject ();
				fixedGrabedTransform = fixedPositionObject.transform;
				fixedGrabedTransform.name = "Fixed Grabbed Transform";
			}

			if (grabInFixedPosition) {
				fixedGrabedTransform.SetParent (mainCameraTransform);
				fixedGrabedTransform.transform.position = objectHeld.transform.position;
				fixedGrabedTransform.transform.rotation = mainCameraTransform.rotation;

				currentMaxDistanceHeld = GKC_Utils.distance (objectHeld.transform.position, mainCameraTransform.position) + holdDistance;
				holdDistance = 0;
			} else {
				currentMaxDistanceHeld = maxDistanceHeld + (holdDistance + orignalHoldDistance) + 0.5f;
			}

			currentObjectWasInsideGravityRoom = false;
		}
	}

	//check if the object detected by the raycast is in the able to grab list or is a vehicle
	public bool checkTypeObject (GameObject objectToCheck)
	{
		if (ableToGrabTags.Contains (objectToCheck.tag)) {
			return true;
		}

		if (applyDamage.isVehicle (objectToCheck)) {
			return true;
		}

		characterDamageReceiver currentCharacterDamageReceiver = objectToCheck.GetComponent<characterDamageReceiver> ();

		if (currentCharacterDamageReceiver != null) {
			if (ableToGrabTags.Contains (currentCharacterDamageReceiver.character.tag)) {
				return true;
			}
		}

		grabObjectParent currentGrabObjectParent = objectToCheck.GetComponent<grabObjectParent> ();

		if (currentGrabObjectParent != null) {
			return true;
		}

		return false;
	}

	//drop the object
	public void dropObject ()
	{
		if (useThrowObjectsLayer) {
			throwObjectDirection = Vector3.zero;

			if (objectHeld != null) {
				if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, throwObjectsLayerToCheck)) {
					Vector3 heading = hit.point - objectHeld.transform.position;
					float distance = heading.magnitude;
					throwObjectDirection = heading / distance;
					throwObjectDirection.Normalize ();
				}
			}
		}

		if (objectHeld != null) {
			//set the tag of the object that had before grab it, and if the object has its own gravity, enable again
			if (grabbedObjectTagLayerStored) {
				objectHeld.tag = originalGrabbedObjectTag;

				if (grabbedObjectIsRagdoll && currentObjectGrabbedLayerList.Count > 0) {
					applyDamage.setBodyColliderLayerList (currentCharacterGrabbed, currentObjectGrabbedLayerList);
				} else {
					objectHeld.layer = originalGrabbedObjectLayer;
				}

				grabbedObjectIsRagdoll = false;
				currentObjectGrabbedLayerList.Clear ();
			
				grabbedObjectTagLayerStored = false;
			}

			currentArtificialObjectGravity = objectHeld.GetComponent<artificialObjectGravity> ();

			if (currentArtificialObjectGravity != null) {
				currentArtificialObjectGravity.active = true;
			}
				
			if (objectHeldRigidbody != null) {
				if (!objectIsVehicle) {
					objectHeldRigidbody.useGravity = true;
				}
					
				objectHeldRigidbody.freezeRotation = false;

				if (objectHeldRigidbodyConstraints != RigidbodyConstraints.None) {
					objectHeldRigidbody.constraints = objectHeldRigidbodyConstraints;
					objectHeldRigidbodyConstraints = RigidbodyConstraints.None;
				}
			}

			grabObjectProperties currentGrabObjectProperties = objectHeld.GetComponent<grabObjectProperties> ();

			if (currentGrabObjectProperties != null) {
				currentGrabObjectProperties.checkEventsOnDropObject ();
			}

			if (enableTransparency) {
				//set the normal shader of the object 
				outlineObjectSystem currentOutlineObjectSystem = objectHeld.GetComponentInChildren<outlineObjectSystem> ();

				if (currentOutlineObjectSystem != null) {
					currentOutlineObjectSystem.setTransparencyState (false, null, 0);
				}
			}

			if (useRemoteEventOnObjectsFound) {
				remoteEventSystem currentRemoteEventSystem = currentCharacterGrabbed.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameListOnDropObject.Count; i++) {
						print (remoteEventNameListOnDropObject [i]);

						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnDropObject [i]);
					}
				}
			}

			grabbedObjectState currentGrabbedObjectState = objectHeld.GetComponent<grabbedObjectState> ();

			if (currentGrabbedObjectState != null) {
				currentObjectWasInsideGravityRoom = currentGrabbedObjectState.isInsideZeroGravityRoom ();

				currentGrabbedObjectState.checkGravityRoomState ();

				currentGrabbedObjectState.setGrabbedState (false);

				if (!currentObjectWasInsideGravityRoom) {
					currentGrabbedObjectState.removeGrabbedObjectComponent ();
				}
			}

			objectToPlaceSystem currentObjectToPlaceSystem = objectHeld.GetComponent<objectToPlaceSystem> ();

			if (currentObjectToPlaceSystem != null) {
				currentObjectToPlaceSystem.setObjectInGrabbedState (false);
			}

			grabObjectEventSystem currentGrabObjectEventSystem = objectHeld.GetComponent<grabObjectEventSystem> ();

			if (currentGrabObjectEventSystem != null) {
				currentGrabObjectEventSystem.callEventOnDrop ();
			}
		}

		grabbed = false;
		objectHeld = null;
		objectHeldRigidbody = null;
		currentCharacterGrabbed = null;

		animationSystem.playBackwardAnimation ();

		objectFocus = false;
	}

	public void checkJointsInObject (GameObject objectToThrow, float force)
	{
		CharacterJoint currentCharacterJoint = objectToThrow.GetComponent<CharacterJoint> ();

		if (currentCharacterJoint != null) {
			checkJointsInObject (currentCharacterJoint.connectedBody.gameObject, force);
		} else {
			addForceToThrownRigidbody (objectToThrow, force);
		}
	}

	public void addForceToThrownRigidbody (GameObject objectToThrow, float force)
	{
		Component[] components = objectToThrow.GetComponentsInChildren (typeof(Rigidbody));
		foreach (Rigidbody child in components) {
			if (!child.isKinematic && child.GetComponent<Collider> ()) {
				Vector3 forceDirection = mainCameraTransform.forward;

				if (useThrowObjectsLayer) {
					//print (throwObjectDirection + " " + forceDirection);
					if (throwObjectDirection != Vector3.zero) {
						forceDirection = throwObjectDirection;
					}
				}

				forceDirection *= force * child.mass;

				child.AddForce (forceDirection, powerForceMode);

				checkIfEnableNoiseOnCollision (objectToThrow, forceDirection.magnitude);
			}
		}
	}

	public void throwRealisticGrabbedObject ()
	{
		Rigidbody currentRigidbody = objectHeld.GetComponent<Rigidbody> ();

		dropObject ();

		if (currentRigidbody != null) {
			Vector3 forceDirection = mainCameraTransform.forward * throwPower * currentRigidbody.mass;
			currentRigidbody.AddForce (forceDirection, realisticForceMode);

			checkIfEnableNoiseOnCollision (objectHeld, forceDirection.magnitude);
		}
	}

	Coroutine setFixedGrabbedTransformCoroutine;

	public void resetFixedGrabedTransformPosition ()
	{
		stopResetFixedGrabedTransformPositionCoroutine ();

		setFixedGrabbedTransformCoroutine = StartCoroutine (resetFixedGrabedTransformPositionCoroutine ());
	}

	public void stopResetFixedGrabedTransformPositionCoroutine ()
	{
		if (setFixedGrabbedTransformCoroutine != null) {
			StopCoroutine (setFixedGrabbedTransformCoroutine);
		}
	}

	IEnumerator resetFixedGrabedTransformPositionCoroutine ()
	{
		Vector3 targetPosition = Vector3.forward * (orignalHoldDistance + currentGrabExtraDistance);

		float dist = GKC_Utils.distance (fixedGrabedTransform.position, mainCameraTransform.position + targetPosition);
		float duration = dist / 10;
		float t = 0;
		bool targetReached = false;

		float timer = 0;

		while (!targetReached && t < 1 && fixedGrabedTransform.localPosition != targetPosition) {
			t += Time.deltaTime / duration;
			fixedGrabedTransform.localPosition = Vector3.Lerp (fixedGrabedTransform.localPosition, targetPosition, t);

			timer += Time.deltaTime;

			if (GKC_Utils.distance (fixedGrabedTransform.localPosition, targetPosition) < 0.01f || timer > duration) {
				targetReached = true;
			}

			yield return null;
		}
	}

	public void checkIfDropObject (GameObject objectToCheck)
	{
		if (objectHeld == objectToCheck) {
			dropObject ();
		}
	}

	public void setWeaponActiveState (bool state)
	{
		weaponActive = state;

		initializeComponents ();

		dropObject ();

		if (useEventOnChangeGravityGunActiveState) {
			if (weaponActive) {
				eventOnGravityGunActive.Invoke ();
			} else {
				eventOnGravityGunDeactivate.Invoke ();
			}
		}
	}

	public GameObject getGrabbedObject ()
	{
		return objectHeld;
	}

	public bool isGrabbedObject ()
	{
		return objectHeld != null;
	}

	public void checkIfEnableNoiseOnCollision (GameObject objectToCheck, float launchSpeed)
	{
		if (launchedObjectsCanMakeNoise && launchSpeed >= minObjectSpeedToActivateNoise) {
			noiseSystem currentNoiseSystem = objectToCheck.GetComponent<noiseSystem> ();

			if (currentNoiseSystem != null) {
				currentNoiseSystem.setUseNoiseState (true);
			}
		}
	}

	public void addForceToLaunchObject ()
	{
		if (currentGrabMode == grabMode.powers) {
			if (holdTimer < maxThrowForce) {
				holdTimer += Time.deltaTime * increaseThrowForceSpeed;

				powerSlider.value += Time.deltaTime * increaseThrowForceSpeed;

				chargingThrowObject = true;
			}
		}
	}

	public void launchObject ()
	{
		powerSlider.value = 0;

		currentObjectToThrow = objectHeld;

		Rigidbody currentRigidbody = currentObjectToThrow.GetComponent<Rigidbody> ();

		dropObject ();

		bool canAddForceToObjectDropped = false;

		if (currentRigidbody != null) {
			//if the button has been pressed and released quickly, drop the object, else addforce to its rigidbody
			if (!objectIsVehicle && !currentObjectWasInsideGravityRoom) {
				currentRigidbody.useGravity = true;
			}

			if (currentGrabMode == grabMode.powers) {
				if (holdTimer > minTimeToIncreaseThrowForce) {
					currentObjectToThrow.AddComponent<launchedObjects> ().setCurrentPlayer (gameObject);

					if (currentObjectToThrow.GetComponent<CharacterJoint> ()) {
						checkJointsInObject (currentObjectToThrow, holdTimer);
					} else {
						holdTimer += extraThorwForce;
						addForceToThrownRigidbody (currentObjectToThrow, holdTimer);
					}

					if (useEventOnLaunchObjects) {
						eventOnLaunchObjects.Invoke ();
					}
				} else {
					canAddForceToObjectDropped = true;
				}
			} else {
				canAddForceToObjectDropped = true;
			}
		}

		if (canAddForceToObjectDropped) {
			if (useForceWhenObjectDropped) {
				if (useForceWhenObjectDroppedOnFirstPerson && playerCameraManager.isFirstPersonActive ()) {
					addForceToThrownRigidbody (currentObjectToThrow, forceWhenObjectDroppedOnFirstPerson);
				} else if (useForceWhenObjectDroppedOnThirdPerson && !playerCameraManager.isFirstPersonActive ()) {
					addForceToThrownRigidbody (currentObjectToThrow, forceWhenObjectDroppedOnThirdPerson);
				}
			}
		}

		chargingThrowObject = false;
	}

	public bool isCurrentObjectToGrabFound (GameObject objectToCheck)
	{
		if (currentObjectToGrabFound == objectToCheck) {
			return true;
		}

		return false;
	}

	//CALL INPUT FUNCTIONS
	public void inputGrabObject ()
	{
		//if the player is in aim mode, grab an object
		if (!playerControllerManager.isUsingDevice () && weaponActive && !objectHeld && canUseWeapon) {
			grabObject ();
		}
	}

	public void inputHoldToLaunchObject ()
	{
		//if the drop button is being holding, add force to the final velocity of the drooped object
		if (grabbed && weaponActive) {
			addForceToLaunchObject ();
		}
	}

	public void inputReleaseToLaunchObject ()
	{
		//when the button is released, check the amount of strength accumulated
		if (weaponActive) {
			if (grabbed) {
				launchObject ();

				if (useEventOnObjectGrabbedDropped) {
					eventOnObjectDropped.Invoke ();
				}
			} else {
				if (objectHeld) {
					if (useEventOnObjectGrabbedDropped) {
						eventOnObjectGrabbed.Invoke ();
					}
				}
			}
		}
	}

	public void inputResetFixedGrabedTransformPosition ()
	{
		if (objectHeld != null && weaponActive) {
			resetFixedGrabedTransformPosition ();
		}
	}

	public void activateSecondaryFunctionPressDown ()
	{
		if (weaponActive) {
			secondaryFunctionEventPressDown.Invoke ();
		}
	}

	public void activateSecondaryFunctionPress ()
	{
		if (weaponActive) {
			secondaryFunctionEventPress.Invoke ();
		}
	}

	public void activateSecondaryFunctionPressUp ()
	{
		if (weaponActive) {
			secondaryFunctionEventPressUp.Invoke ();
		}
	}

	public void setNewGravityToGrabbedObject ()
	{
		if (changeGravityObjectsEnabled) {
			GameObject grabbedObject = getGrabbedObject ();

			if (grabbedObject.GetComponent<Rigidbody> ()) {
				dropObject ();

				//if the current object grabbed is a vehicle, enable its own gravity control component
				currentVehicleGravityControl = grabbedObject.GetComponent<vehicleGravityControl> ();

				if (currentVehicleGravityControl != null) {
					currentVehicleGravityControl.activateGravityPower (mainCameraTransform.TransformDirection (Vector3.forward), 
						mainCameraTransform.TransformDirection (Vector3.right));
				} else {
					//else, it is a regular object
					//change the layer, because the object will use a raycast to check the new normal when a collision happens
					grabbedObject.layer = LayerMask.NameToLayer (layerForCustomGravityObject);

					//if the object has a regular gravity, attach the scrip and set its values
					currentArtificialObjectGravity = grabbedObject.GetComponent<artificialObjectGravity> ();

					if (currentArtificialObjectGravity == null) {
						currentArtificialObjectGravity = grabbedObject.AddComponent<artificialObjectGravity> ();
					} 

					currentArtificialObjectGravity.enableGravity (gravityObjectsLayer, highFrictionMaterial, mainCameraTransform.forward);
				}
			}
		}
	}

	public bool isCarryingObject ()
	{
		return grabbed;
	}

	public void setCanUseWeaponState (bool state)
	{
		canUseWeapon = state;

		initializeComponents ();

		if (!canUseWeapon) {
			if (objectHeld != null) {
				dropObject ();
			}

			if (objectFocus) {
				animationSystem.playBackwardAnimation ();

				objectFocus = false;

				checkObjectFoundOrLostState ();
			}
		}
	}

	public void rotateRotor (float newSpeed)
	{
		if (useRotor) {
			rotor.Rotate (0, 0, Time.deltaTime * newSpeed);
		}
	}


	bool componentsInitialized;

	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}

		if (mainPlayerWeaponSystem != null) {
			GameObject playerControllerGameObject = mainPlayerWeaponSystem.getPlayerWeaponsManger ().gameObject;

			playerComponentsManager mainPlayerComponentsManager = playerControllerGameObject.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				playerControllerManager = mainPlayerComponentsManager.getPlayerController ();

				playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

				mainCameraTransform = playerCameraManager.getCameraTransform ();
			}
		}

		componentsInitialized = true;
	}
}
