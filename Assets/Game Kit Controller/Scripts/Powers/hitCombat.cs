using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class hitCombat : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float hitDamage = 5;

	public float damageMultiplier = 1;

	public bool configureDamageAsConstant;

	public bool ignoreShield;
	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	public int damageTypeID = -1;

	public bool damageCanBeBlocked = true;

	public LayerMask layerMask;

	public bool checkTriggerSurfaces;

	[Space]
	[Header ("Ignore Tags Settings")]
	[Space]

	public bool useIgnoreTags;
	public List<string> tagsToIgnoreList = new List<string> (new string[] { "enemy" });

	[Space]
	[Header ("Objects Detected Settings")]
	[Space]

	public bool sendMessageOnDamageDetected = true;
	public string messageOnDamageDetectedName = "setDamageDetectedOnTriggerById";
	public bool useCustomObjectToSendMessage;
	public GameObject customObjectToSendMessage;

	public bool sendMessageOnSurfaceWithNoDamageDetected;
	public string messageOnSurfaceWithNoDamageDetectedName = "setNoDamageDetectedOnTriggerById";

	public bool storeDetectedObjectOnList;

	[Space]
	[Header ("Physics Settings")]
	[Space]

	public bool checkSurfacesForDecals;

	public float raycastDistance;

	[Space]
	[Header ("Push Objects Settings")]
	[Space]

	public float addForceMultiplier;

	public ForceMode forceMode = ForceMode.Impulse;
	public bool applyImpactForceToVehicles = true;
	public float impactForceToVehiclesMultiplier = 0.2f;

	public bool usePushCharacter;
	public string pushCharacterFunctionName = "pushCharacter";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool currentlyEnabled;

	public int currentCombatTypeIndex;

	public int currentAttackIndex;

	public bool damageAppliedInLastAttack;

	public int triggerId;

	public bool detectSurfaceStateActive;

	public bool useCastAllDamageDetection;

	public List<GameObject> detectedObjectList = new List<GameObject> ();

	public bool ignoreDetectedObjectsOnList;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnDetectSurfaceStateActive;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject currentPlayer;

	public Collider mainCollider;

	public BoxCollider mainBoxCollider;

	public SphereCollider mainSphereCollider;

	Rigidbody objectToDamageRigidbody;

	bool pushCharacterOnce;

	GameObject lastSurfaceDetected;

	GameObject currentObjectToSendMessage;

	Vector3 customForceDirection;

	float customForceAmount;
	float customForceToVehiclesMultiplier;

	bool ignoreForcesToApplyOnAttackActive;

	//check the collision in the sphere colliders in the hands and feet of the player when the close combat system is active
	//else the sphere collider are disabled to avoid damage enemies just with touch it without fight
	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (currentlyEnabled) {
			if (isEnter) {
				GameObject currentObject = col.gameObject;

				if (showDebugPrint) {
					print ("possible object to process " + currentObject.name);
				}

				checkObjectDetected (currentObject, col.isTrigger);
			}
		}
	}

	public bool checkObjectDetection (GameObject objectDetected, bool isTrigger)
	{
		if (!useIgnoreTags || !tagsToIgnoreList.Contains (objectDetected.tag)) {
			if ((1 << objectDetected.layer & layerMask.value) == 1 << objectDetected.layer) {
				if (objectDetected != currentPlayer && (!isTrigger || checkTriggerSurfaces)) {
					return true;
				}
			}
		}

		return false;
	}

	public void checkObjectDetected (GameObject objectDetected, bool isTrigger)
	{
		if (checkObjectDetection (objectDetected, isTrigger)) {

			if (storeDetectedObjectOnList) {
				GameObject objectToDamage = applyDamage.getCharacterOrVehicle (objectDetected);

				if (objectToDamage != null) {
					if (detectedObjectList.Contains (objectToDamage)) {
						if (!ignoreDetectedObjectsOnList) {
							if (showDebugPrint) {
								print ("already detected " + objectToDamage.name);
							}

							return;
						}
					} else {
						detectedObjectList.Add (objectToDamage);
					}
				} else {
					detectedObjectList.Add (objectDetected);
				}
			}

			if (showDebugPrint) {
				print ("current object to process " + objectDetected.name);
			}

			lastSurfaceDetected = objectDetected;

			checkDecalsToApplyOnObject (objectDetected);

			checkDamageToApplyOnObject (objectDetected);

			checkPushCharactersOnObject (objectDetected);

			checkForceToApplyOnObject (objectDetected);

			if (detectSurfaceStateActive) {
				eventOnDetectSurfaceStateActive.Invoke ();

				detectSurfaceStateActive = false;
			}
		}
	}

	public void checkDamageToApplyOnObject (GameObject objectToDamage)
	{
		if (activateDamage (objectToDamage)) {
			
			if (!storeDetectedObjectOnList) {
				currentlyEnabled = false;

				ignoreForcesToApplyOnAttackActive = false;
			}

			if (showDebugPrint) {
				print ("Object detected " + objectToDamage.name + " " + (objectToDamage == gameObject) + " damage: " + hitDamage * damageMultiplier);
			}
		
			damageAppliedInLastAttack = true;

			if (sendMessageOnDamageDetected) {
				currentObjectToSendMessage = currentPlayer;

				if (useCustomObjectToSendMessage) {
					currentObjectToSendMessage = customObjectToSendMessage;
				}

				currentObjectToSendMessage.SendMessage (messageOnDamageDetectedName, triggerId, SendMessageOptions.DontRequireReceiver);
			}
		} else {
			if (sendMessageOnSurfaceWithNoDamageDetected) {
				currentObjectToSendMessage = currentPlayer;

				if (useCustomObjectToSendMessage) {
					currentObjectToSendMessage = customObjectToSendMessage;
				}

				currentObjectToSendMessage.SendMessage (messageOnSurfaceWithNoDamageDetectedName, objectToDamage, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void checkPushCharactersOnObject (GameObject objectToDamage)
	{
		if (usePushCharacter || pushCharacterOnce) {

			if (showDebugPrint) {
				print ("Use push character on " + objectToDamage.name);
			}

			GameObject currentCharacter = applyDamage.getCharacterOrVehicle (objectToDamage);

			if (currentCharacter != null) {
				currentCharacter.SendMessage (pushCharacterFunctionName, currentPlayer.transform.forward, SendMessageOptions.DontRequireReceiver);

				deactivatePushCHaracterOnce ();
			}
		}
	}

	public void setCustomForceDirection (Vector3 newValue)
	{
		customForceDirection = newValue;
	}

	public void setCustomForceAmount (float newValue)
	{
		customForceAmount = newValue;
	}

	public void setIgnoreForcesToApplyOnAttackActiveState (bool state)
	{
		ignoreForcesToApplyOnAttackActive = state;
	}

	public void setCustomForceToVehiclesMultiplier (float newValue)
	{
		customForceToVehiclesMultiplier = newValue;
	}

	public void setAddForceMultiplierValue (float newValue)
	{
		addForceMultiplier = newValue;
	}

	public void checkForceToApplyOnObject (GameObject objectToDamage)
	{
		if (ignoreForcesToApplyOnAttackActive) {
			return;
		}

		objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

		Vector3 forceDirection = Vector3.zero;

		if (customForceDirection != Vector3.zero) {
			forceDirection = customForceDirection;

			customForceDirection = Vector3.zero;
		} else {
			forceDirection = currentPlayer.transform.up + currentPlayer.transform.forward;
		}

		float forceAmount = addForceMultiplier;

		if (customForceAmount != 0) {
			forceAmount = customForceAmount;
		}

		if (forceAmount != 0) {
			float forceToVehiclesMultiplier = impactForceToVehiclesMultiplier;

			if (customForceToVehiclesMultiplier != 0) {
				forceToVehiclesMultiplier = customForceToVehiclesMultiplier;
			}

			if (applyImpactForceToVehicles) {
				Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);

				if (objectToDamageMainRigidbody != null) {
					Vector3 force = forceDirection * forceAmount;

					bool isVehicle = applyDamage.isVehicle (objectToDamage);

					if (isVehicle) {
						force *= forceToVehiclesMultiplier;
					} 

//				else {
//					if (storeDetectedObjectOnList) {
//						GameObject currentObject = applyDamage.getCharacter (objectToDamage);
//
//						if (detectedObjectList.Contains (currentObject)) {
//							return;
//						} else {
//							detectedObjectList.Add (currentObject);
//						}
//					}
//				}

					objectToDamageMainRigidbody.AddForce (force * objectToDamageMainRigidbody.mass, forceMode);
				}
			} else {
				if (applyDamage.canApplyForce (objectToDamage)) {
					//print (objectToDamage.name);
					Vector3 force = forceDirection * forceAmount;

					if (objectToDamageRigidbody == null) {
						objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();
					}

					objectToDamageRigidbody.AddForce (force * objectToDamageRigidbody.mass, forceMode);
				}
			}
		}
	}

	public void checkDecalsToApplyOnObject (GameObject objectToDamage)
	{
		if (checkSurfacesForDecals) {
//			decalManager.setImpactDecal (decalManager.checkIfHasDecalImpact (objectToDamage), raycastCheckTransfrom, objectToDamage, raycastDistance, layerMask, false);
		}
	}

	public GameObject getLastSurfaceDetected ()
	{
		return lastSurfaceDetected;
	}

	public void setLastSurfaceDetected (GameObject newSurface)
	{
		lastSurfaceDetected = newSurface;
	}

	public void getOwner (GameObject ownerObject)
	{
		currentPlayer = ownerObject;

		assignMainCollider ();

		//ignore collision between the owner and the sphere colliders, to avoid hurt him self
		Physics.IgnoreCollision (currentPlayer.GetComponent<Collider> (), mainCollider);
	}

	public Collider getMainCollider ()
	{
		assignMainCollider ();
	
		return mainCollider;
	}

	public void setMainColliderEnabledState (bool state)
	{
		if (mainCollider != null) {
			mainCollider.enabled = state;
		}
	}

	void assignMainCollider ()
	{
		if (mainCollider == null) {
			mainCollider = GetComponent<Collider> ();
		}
	}

	public void setCurrentState (bool value)
	{
		currentlyEnabled = value;

		if (!currentlyEnabled) {
			deactivatePushCHaracterOnce ();
		}

		damageAppliedInLastAttack = false;

		if (storeDetectedObjectOnList) {
			detectedObjectList.Clear ();
		}

		if (!currentlyEnabled) {
			ignoreForcesToApplyOnAttackActive = false;
		}
	}

	public void setIgnoreDetectedObjectsOnListState (bool state)
	{
		ignoreDetectedObjectsOnList = state;
	}

	public void setConfigureDamageAsConstant (bool state)
	{
		configureDamageAsConstant = state;
	}

	public void setUseCastAllDamageDetectionState (bool state)
	{
		useCastAllDamageDetection = state;

//		if (showDebugPrint) {
//			print ("useCastAllDamageDetection " + useCastAllDamageDetection);
//		}

		if (useCastAllDamageDetection) {
			Collider[] colliders = null;

			if (mainSphereCollider != null) {
				colliders = Physics.OverlapSphere (transform.position, mainSphereCollider.radius, layerMask);
			} else {
				colliders = Physics.OverlapBox (transform.position, mainBoxCollider.size, transform.rotation, layerMask);
			}

			for (int i = 0; i < colliders.Length; i++) {
				if (!colliders [i].isTrigger || checkTriggerSurfaces) {
					checkTriggerInfo (colliders [i], true);
				}
			}

			useCastAllDamageDetection = false;
		}
	}

	public void setNewHitDamage (float newValue)
	{
		hitDamage = newValue;
	}

	public void setCurrentExtraDamageValue (float extraDamageValue)
	{
		damageMultiplier = extraDamageValue;

		if (damageMultiplier <= 0) {
			damageMultiplier = 1;
		}
	}

	public void activePushCharacterOnce ()
	{
		pushCharacterOnce = true;
	}

	public void deactivatePushCHaracterOnce ()
	{
		pushCharacterOnce = false;
	}

	public void setUsePushCharacterState (bool state)
	{
		usePushCharacter = state;
	}

	public void setCurrentAttackInfoIndex (int combatTypeIndex, int attackIndex)
	{
		currentCombatTypeIndex = combatTypeIndex;
		currentAttackIndex = attackIndex;
	}

	public bool getDamageAppliedInLastAttackState ()
	{
		return damageAppliedInLastAttack;
	}

	public void setDetectSurfaceStateActive (bool state)
	{
		detectSurfaceStateActive = state;
	}

	public bool isDetectSurfaceStateActive ()
	{
		return detectSurfaceStateActive;
	}

	public bool activateDamage (GameObject objectToDamage)
	{
		return applyDamage.checkCanBeDamaged (gameObject, objectToDamage, hitDamage * damageMultiplier, -transform.forward, 
			transform.position, currentPlayer, configureDamageAsConstant, true, ignoreShield, false, damageCanBeBlocked,
			canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
	}

	public float getLastDamageApplied ()
	{
		return hitDamage * damageMultiplier;
	}

	public void setCustomDamageCanBeBlockedState (bool state)
	{
		if (showDebugPrint) {
			print ("setting can be blocked state " + state);
		}

		damageCanBeBlocked = state;
	}

	public void setTriggerId (int newValue)
	{
		triggerId = newValue;
	}

	public void setTriggerIdOnEditor (int newValue)
	{
		triggerId = newValue;

		updateComponent ();
	}

	public void setCustomObjectToSendMessage (GameObject newObject)
	{
		customObjectToSendMessage = newObject;
	}

	public void setCustomLayerMask (LayerMask newLayerMask)
	{
		layerMask = newLayerMask;
	}

	public void setCustomTagsToIgnore (List<string> newTagList)
	{
		if (newTagList != null) {
			useIgnoreTags = true;

			tagsToIgnoreList = newTagList;
		} else {
			useIgnoreTags = false;

			tagsToIgnoreList.Clear ();
		}
	}

	public void setSendMessageOnDamageDetectedState (bool state)
	{
		sendMessageOnDamageDetected = state;
	}

	public void setCanActivateReactionSystemTemporallyState (bool state)
	{
		canActivateReactionSystemTemporally = state;
	}

	public void setNewDamageReactionID (int newValue)
	{
		damageReactionID = newValue;
	}

	public void setNewDamageTypeID (int newValue)
	{
		damageTypeID = newValue;
	}

	public void setNewSphereColliderTriggerRadius (float newValue)
	{
		if (mainSphereCollider != null) {
			mainSphereCollider.radius = newValue;
		}
	}

	public void setNewBoxColliderTriggerSize (Vector3 newValue)
	{
		if (mainBoxCollider != null) {
			mainBoxCollider.size = newValue;
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Hit Combat", gameObject);
	}
}