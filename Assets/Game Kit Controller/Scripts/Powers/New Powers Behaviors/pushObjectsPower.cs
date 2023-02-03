using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushObjectsPower : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool powerEnabled = true;

	public bool pushObjectsFromCenterPosition;

	public bool pushObjectsFromPlayerForwardDirection;

	public LayerMask layer;

	public List<string> ignoreTagList = new List<string> ();

	public bool useMessageToPushCharactersFound = true;
	public string messageNameToSend = "pushCharacter";

	[Space]
	[Header ("Damage Settings")]
	[Space]

	public bool applyDamageOnFoundObjects;
	public float damageToApply;
	public bool ignoreShield;
	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	public int damageTypeID = -1;

	[Space]
	[Header ("Force Settings")]
	[Space]

	public float forceToApply = 4000;
	public ForceMode forceMode;

	public bool canApplyForceToVehicles = true;
	public float applyForceToVehiclesMultiplier = 0.2f;

	public bool applyForceToFoundObjectsOnlyOnce;

	public bool checkRagdollsDetected;
	public float ragdollMultiplierForce = 1;

	[Space]
	[Header ("Others Settings")]
	[Space]

	public bool useCustomPushCenterDistance;
	public float pushCenterDistance;

	public bool searchForObjectsOnUpdate;

	public bool checkIfObjectInFrontOfPlayer;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameList = new List<string> ();
	public bool callRemoteEventsBeforeApplyingForce;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool useOfPowerPaused;

	public List<Rigidbody> vehiclesRigidbodyFoundList = new List<Rigidbody> ();

	public List<GameObject> gameObjectsFoundList = new List<GameObject> ();

	[Space]
	[Header ("Components")]
	[Space]

	public playerWeaponSystem mainPlayerWeaponSystem;
	public GameObject playerGameObject;
	public Transform centerPosition;
	public otherPowers powersManager;
	public Transform pushObjectsCenter;
	public Transform mainCameraTransform;

	Rigidbody objectToDamageMainRigidbody;

	GameObject objectToPush;
	Collider[] colliders;
	Vector3 currentForceToApply;
	float finalExplosionForce;
	bool isVehicle;

	bool componentsInitialized;

	void Start ()
	{
		if (!useCustomPushCenterDistance) {
			//get the distance from the empty object in the player to push objects, close to it
			pushCenterDistance = GKC_Utils.distance (playerGameObject.transform.position, pushObjectsCenter.position);
		}

		if (pushObjectsFromCenterPosition) {
			if (centerPosition == null) {
				centerPosition = transform;
			}
		}
	}

	void Update ()
	{
		if (searchForObjectsOnUpdate) {
			activatePower ();
		}
	}

	public void activatePower ()
	{
		if (useOfPowerPaused) {
			return;
		}

		if (!powerEnabled) {
			return;
		}

		initializeComponents ();

		vehiclesRigidbodyFoundList.Clear ();

		if (powersManager != null) {
			//the power number 2 is push objects, so any bullet is created
			powersManager.createShootParticles ();
		}

		//if the power selected is push objects, check the objects close to pushObjectsCenter and add force to them in camera forward direction
		colliders = Physics.OverlapSphere (pushObjectsCenter.position, pushCenterDistance, layer);

		for (int i = 0; i < colliders.Length; i++) {
			if (!colliders [i].isTrigger) {
				objectToPush = colliders [i].gameObject;

				checkObjectToApplyForce (objectToPush);
			}
		}
	}

	public void checkObjectToApplyForce (GameObject currentObject)
	{
		if (applyForceToFoundObjectsOnlyOnce) {
			if (!gameObjectsFoundList.Contains (currentObject)) {
				gameObjectsFoundList.Add (currentObject);
			} else {
				return;
			}
		}

		if (!ignoreTagList.Contains (currentObject.tag) && currentObject != playerGameObject) {
			if (playerGameObject == null) {
				playerGameObject = gameObject;
			}

			if (checkIfObjectInFrontOfPlayer) {
				float dot = Vector3.Dot (playerGameObject.transform.forward, (currentObject.transform.position - playerGameObject.transform.position).normalized);
				if (dot < 0) {
					return;
				}
			}

			Vector3 pushDirection = Vector3.zero;

			if (pushObjectsFromCenterPosition) {
				pushDirection = (currentObject.transform.position - centerPosition.position).normalized;
			} else {
				if (pushObjectsFromPlayerForwardDirection) {
					pushDirection = playerGameObject.transform.forward;
				} else {
					if (mainCameraTransform != null) {
						pushDirection = mainCameraTransform.forward;
					}
				}
			}

			if (useMessageToPushCharactersFound && messageNameToSend != "") {
				currentObject.SendMessage (messageNameToSend, pushDirection, SendMessageOptions.DontRequireReceiver);
			}

			if (applyDamageOnFoundObjects) {
				applyDamage.checkHealth (playerGameObject, currentObject, damageToApply, playerGameObject.transform.forward, transform.position, playerGameObject,
					false, true, ignoreShield, false, canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
			}

			objectToDamageMainRigidbody = applyDamage.applyForce (currentObject);

			bool rigidbodyFound = false;

			if (canApplyForceToVehicles) {
				if (objectToDamageMainRigidbody != null) {

					if (!vehiclesRigidbodyFoundList.Contains (objectToDamageMainRigidbody)) {

						if (callRemoteEventsBeforeApplyingForce) {
							checkRemoteEvent (objectToDamageMainRigidbody.gameObject);
						}

						isVehicle = applyDamage.isVehicle (currentObject);

						finalExplosionForce = forceToApply;

						if (isVehicle) {
							finalExplosionForce *= applyForceToVehiclesMultiplier;
						}

						if (pushObjectsFromCenterPosition) {
							currentForceToApply = (objectToDamageMainRigidbody.position - centerPosition.position).normalized * finalExplosionForce;
						} else {
							if (pushObjectsFromPlayerForwardDirection) {
								currentForceToApply = playerGameObject.transform.forward * finalExplosionForce;
							} else {
								currentForceToApply = mainCameraTransform.TransformDirection (Vector3.forward) * finalExplosionForce;
							}
						}

						if (checkRagdollsDetected) {
							if (applyDamage.isRagdollActive (currentObject)) {
//								print ("ragdoll found");
								currentForceToApply *= ragdollMultiplierForce;
							} else {
//								print ("Not found");
							}
						}

						if (currentForceToApply != Vector3.zero) {
							objectToDamageMainRigidbody.AddForce (currentForceToApply * objectToDamageMainRigidbody.mass, forceMode);
						}

						if (isVehicle) {
							vehiclesRigidbodyFoundList.Add (objectToDamageMainRigidbody);
						}

						if (!callRemoteEventsBeforeApplyingForce) {
							checkRemoteEvent (objectToDamageMainRigidbody.gameObject);
						}

						rigidbodyFound = true;
					}
				}
			} else {
				if (applyDamage.canApplyForce (currentObject)) {
					if (callRemoteEventsBeforeApplyingForce) {
						checkRemoteEvent (currentObject);
					}

					if (pushObjectsFromCenterPosition) {
						currentForceToApply = (objectToDamageMainRigidbody.position - centerPosition.position).normalized * forceToApply;
					} else {
						if (pushObjectsFromPlayerForwardDirection) {
							currentForceToApply = playerGameObject.transform.forward * forceToApply;
						} else {
							currentForceToApply = mainCameraTransform.TransformDirection (Vector3.forward) * forceToApply;
						}
					}

					if (currentForceToApply != Vector3.zero) {
						objectToDamageMainRigidbody = currentObject.GetComponent<Rigidbody> ();

						objectToDamageMainRigidbody.AddForce (currentForceToApply * objectToDamageMainRigidbody.mass, forceMode);
					}

					if (!callRemoteEventsBeforeApplyingForce) {
						checkRemoteEvent (currentObject);
					}

					rigidbodyFound = true;
				}
			}

			if (!rigidbodyFound) {
				checkRemoteEvent (currentObject);
			}
		}
	}

	public void setPowerEnabledState (bool state)
	{
		powerEnabled = state;

		initializeComponents ();

		if (powerEnabled) {
			gameObjectsFoundList.Clear ();
		}
	}

	public void cleanGameObjectFoundList ()
	{
		gameObjectsFoundList.Clear ();
	}

	public void checkRemoteEvent (GameObject objectToCheck)
	{
		if (useRemoteEventOnObjectsFound) {

			remoteEventSystem currentRemoteEventSystem = objectToCheck.GetComponent<remoteEventSystem> ();

			if (currentRemoteEventSystem != null) {
				for (int i = 0; i < remoteEventNameList.Count; i++) {

					currentRemoteEventSystem.callRemoteEvent (remoteEventNameList [i]);
				}
			}
		}
	}

	public void setUseOfPowerPausedState (bool state)
	{
		useOfPowerPaused = state;
	}

	public void setNewPushCenterDistance (float newValue)
	{
		pushCenterDistance = newValue;
	}

	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}

		if (mainPlayerWeaponSystem != null) {
			playerGameObject = mainPlayerWeaponSystem.getPlayerWeaponsManger ().gameObject;

			if (playerGameObject != null) {
				playerComponentsManager mainPlayerComponentsManager = playerGameObject.GetComponent<playerComponentsManager> ();

				if (mainPlayerComponentsManager != null) {
					powersManager = mainPlayerComponentsManager.getOtherPowers ();

					mainCameraTransform = mainPlayerComponentsManager.getPlayerCamera ().getCameraTransform ();
				}
			}
		}

		componentsInitialized = true;
	}
}
