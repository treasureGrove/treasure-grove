using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class setGravity : MonoBehaviour
{
	public bool useWithPlayer;
	public bool useWithNPC;
	public bool useWithVehicles;
	public bool useWithAnyRigidbody;

	public string playerTag = "Player";
	public string friendTag = "friend";
	public string enemyTag = "enemy";

	public bool checkOnlyForArtificialGravitySystem;

	public bool setCustomGravityForce;
	public float customGravityForce;

	public bool setCustomGravityForceOnCharactersEnabled;

	public triggerType typeOfTrigger;

	public enum triggerType
	{
		enter,
		exit,
		both
	}

	public bool setGravityMode = true;
	public bool setRegularGravity;

	public bool setZeroGravity;

	public bool disableZeroGravity;

	public bool movePlayerToGravityPosition;
	public Transform raycastPositionToGetGravityPosition;
	public LayerMask layermaskToGetGravityPosition;
	public float teleportSpeed;
	public float rotationSpeed;

	public bool useCustomGravityDirection;
	public Vector3 customGravityDirection;

	public bool useCenterPoint;
	public Transform centerPoint;
	public bool useCenterPointForRigidbodies;
	public bool useInverseDirectionToCenterPoint;

	public bool useCenterPointList;
	public bool useCenterIfPointListTooClose = true;
	public List<Transform> centerPointList = new List<Transform> ();
	public bool useCenterPointListForRigidbodies;

	public bool changeGravityDirectionActive;

	public bool rotateToSurfaceSmoothly = true;

	public bool setCircumnavigateSurfaceState;
	public bool circumnavigateSurfaceState;

	public bool setCheckSurfaceInFrontState;
	public bool checkSurfaceInFrontState;

	public bool setCheckSurfaceBelowLedgeState;
	public bool checkSurfaceBelowLedgeState;

	public bool preservePlayerVelocity = true;

	public bool storeSetGravityManager = true;

	public bool setTargetParent;
	public GameObject targetParent;
	public bool setRigidbodiesParent;

	public bool useAnimation = true;
	public string animationName = "arrowAnim";
	public Animation mainAnimation;

	public bool dropObjectIfGabbed = true;
	public bool dropObjectOnlyIfNotGrabbedPhysically;

	public List<artificialObjectGravity> artificialObjectGravityList = new List<artificialObjectGravity> ();


	public bool useInitialObjectsOnGravityList;

	public List<GameObject> initialObjectsOnGravityList = new List<GameObject> ();


	bool inside;
	GameObject objectToChangeGravity;
	GameObject character;
	grabObjects grabObjectsManager;
	grabbedObjectState currentGrabbedObject;

	bool initialObjectLisChecked;

	void Start ()
	{
		if (useAnimation && mainAnimation == null) {
			mainAnimation = GetComponent<Animation> ();

			if (mainAnimation == null) {
				useAnimation = false;
			}
		}

		if (centerPoint == null) {
			centerPoint = transform;
		}
	}

	//set a custom gravity for the player and the vehicles, in the direction of the arrow
	void Update ()
	{
		if (useAnimation) {
			mainAnimation.Play (animationName);
		}

		if (!initialObjectLisChecked) {
			if (useInitialObjectsOnGravityList) {
				for (int i = 0; i < initialObjectsOnGravityList.Count; i++) {
					if (initialObjectsOnGravityList [i] != null) {
						checkObjectOnGravitySystem (initialObjectsOnGravityList [i], true);
					}
				}
			}

			initialObjectLisChecked = true;
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (typeOfTrigger == triggerType.enter || typeOfTrigger == triggerType.both) {
			checkTriggerType (col, true);
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (typeOfTrigger == triggerType.exit || typeOfTrigger == triggerType.both) {
			checkTriggerType (col, false);
		}
	}

	public void checkObjectGravityOnEnter (GameObject objectToCheck)
	{
		checkTriggerType (objectToCheck.GetComponent<Collider> (), true);
	}

	public void checkObjectGravityOnExit (GameObject objectToCheck)
	{
		checkTriggerType (objectToCheck.GetComponent<Collider> (), false);
	}

	public void checkTriggerType (Collider col, bool isEnter)
	{
		checkObjectOnGravitySystem (col.gameObject, isEnter);
	}

	public void checkObjectOnGravitySystem (GameObject objectToCheck, bool isEnter)
	{
		objectToChangeGravity = objectToCheck;

		//if the player is not driving, stop the gravity power

		bool checkResult = (useWithPlayer && objectToChangeGravity.CompareTag (playerTag)) ||
		                   (useWithNPC && objectToChangeGravity.CompareTag (friendTag)) ||
		                   (useWithNPC && objectToChangeGravity.CompareTag (enemyTag));

		if (checkResult) {

			playerController currentPlayerController = objectToChangeGravity.GetComponent<playerController> ();

			if (!currentPlayerController.isPlayerDriving ()) {

				gravitySystem currentGravitySystem = objectToChangeGravity.GetComponent<gravitySystem> ();

				if (currentGravitySystem != null) {
					if (!storeSetGravityManager || currentGravitySystem.getCurrentSetGravityManager () != this) {

						if (storeSetGravityManager) {
							if (isEnter) {
								currentGravitySystem.setCurrentSetGravityManager (this);
							} else {
								currentGravitySystem.setCurrentSetGravityManager (null);
							}
						}

						if (setCheckSurfaceInFrontState) {
							currentGravitySystem.setCheckSurfaceInFrontState (checkSurfaceInFrontState);
						} 

						if (setCheckSurfaceBelowLedgeState) {
							currentGravitySystem.setCheckSurfaceBelowLedgeState (checkSurfaceBelowLedgeState);
						} 

						if (setCircumnavigateSurfaceState) {
							currentGravitySystem.setCircumnavigateSurfaceState (circumnavigateSurfaceState);
						} 

						if (setGravityMode) {
							if (setRegularGravity) {
								if (currentGravitySystem.isZeroGravityModeOn ()) {
									currentGravitySystem.setZeroGravityModeOnState (false);
								} else {
									currentGravitySystem.deactivateGravityPower ();
								}
					
							} else if (setZeroGravity) {
								currentGravitySystem.setZeroGravityModeOnState (true);
							} else if (movePlayerToGravityPosition) {
								Vector3 rayPosition = raycastPositionToGetGravityPosition.position;
								Vector3 rayDirection = raycastPositionToGetGravityPosition.forward;

								RaycastHit hit = new RaycastHit ();

								if (Physics.Raycast (rayPosition, rayDirection, out hit, Mathf.Infinity, layermaskToGetGravityPosition)) {
									currentGravitySystem.teleportPlayer (hit.point, teleportSpeed, hit.normal, rotationSpeed);
								}
							} else {
								if (disableZeroGravity) {
									currentGravitySystem.setZeroGravityModeOnStateWithOutRotation (false);
								}

								if (changeGravityDirectionActive) {
									Vector3 newGravityDirection = -getGravityDirection (objectToChangeGravity.transform.position);

									if (currentGravitySystem.getCurrentNormal () != newGravityDirection) {
										currentGravitySystem.changeGravityDirectionDirectly (newGravityDirection, preservePlayerVelocity);
									}

									if (setTargetParent) {
										currentGravitySystem.addParent (targetParent);
									}
								} else {
									objectToChangeGravity.GetComponent<playerStatesManager> ().checkPlayerStates (true, true, true, true, true, false, true, true);

									if (rotateToSurfaceSmoothly) {
										currentGravitySystem.changeOnTrigger (getGravityDirection (objectToChangeGravity.transform.position), transform.right);
									} else {
										currentGravitySystem.setNormal (getGravityDirection (objectToChangeGravity.transform.position));
									}
								}
							}
						}

						if (setCustomGravityForceOnCharactersEnabled) {
							if (setCustomGravityForce) {
								if (isEnter) {
									currentPlayerController.setGravityForceValue (false, customGravityForce);
								} else {
									currentPlayerController.setOriginalGravityForceValue ();
								}
							}
						}
					}
				}
			}
		} else if (objectToChangeGravity.GetComponent<Rigidbody> ()) {

			//if the object is being carried by the player, make him drop it
			currentGrabbedObject = objectToChangeGravity.GetComponent<grabbedObjectState> ();

			if (currentGrabbedObject != null) {
				if (dropObjectIfGabbed) {
					if (!dropObjectOnlyIfNotGrabbedPhysically || !currentGrabbedObject.isCarryingObjectPhysically ()) {
						GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), objectToChangeGravity);
					}
				}
			}

			//if the player is driving, disable the gravity control in the vehicle
			vehicleGravityControl currentVehicleGravityControl = objectToChangeGravity.GetComponent<vehicleGravityControl> ();

			if (useWithVehicles && currentVehicleGravityControl != null) {
				if (currentVehicleGravityControl.gravityControlEnabled) {
					if (setRegularGravity) {
						currentVehicleGravityControl.deactivateGravityPower ();
					} else {
						currentVehicleGravityControl.activateGravityPower (getGravityDirection (currentVehicleGravityControl.transform.position), transform.right);
					}
				}
			} else {
				if (useWithAnyRigidbody || checkOnlyForArtificialGravitySystem) {

					artificialObjectGravity currentArtificialObjectGravity = objectToChangeGravity.GetComponent<artificialObjectGravity> ();

					if (checkOnlyForArtificialGravitySystem) {
						objectToUseArtificialGravitySystem currentObjectToUseArtificialGravitySystem = objectToChangeGravity.GetComponent<objectToUseArtificialGravitySystem> ();

						if (currentObjectToUseArtificialGravitySystem == null && !currentArtificialObjectGravity) {
							return;
						}
					}

					if (currentArtificialObjectGravity == null) {
						currentArtificialObjectGravity = objectToChangeGravity.AddComponent<artificialObjectGravity> ();
					}
						

					if (setRegularGravity) {

						if (artificialObjectGravityList.Contains (currentArtificialObjectGravity)) {
							artificialObjectGravityList.Remove (currentArtificialObjectGravity);
						}

						currentArtificialObjectGravity.removeGravity ();

						if (setRigidbodiesParent) {
							objectToChangeGravity.transform.SetParent (null);
						}
					} else {
						if (!artificialObjectGravityList.Contains (currentArtificialObjectGravity)) {
							artificialObjectGravityList.Add (currentArtificialObjectGravity);
						}

						if (useCenterPointForRigidbodies && centerPoint != null) {
							currentArtificialObjectGravity.setUseCenterPointActiveState (useCenterPointForRigidbodies, centerPoint);
						}

						if (useCenterPointListForRigidbodies) { 
							currentArtificialObjectGravity.setUseCenterPointListForRigidbodiesState (useCenterPointListForRigidbodies, centerPointList);
						}
							
						currentArtificialObjectGravity.setUseInverseDirectionToCenterPointState (useInverseDirectionToCenterPoint);
					
						currentArtificialObjectGravity.setCurrentGravity (getGravityDirection (objectToChangeGravity.transform.position));

						if (setCustomGravityForce) {
							currentArtificialObjectGravity.setGravityForceValue (false, customGravityForce);
						}

						if (setRigidbodiesParent) {
							objectToChangeGravity.transform.SetParent (targetParent.transform);
						} else {
							objectToChangeGravity.transform.SetParent (null);
						}
					}
				}
			}
		}
	}

	public Vector3 getGravityDirection (Vector3 objectPosition)
	{
		if (useCustomGravityDirection) {
			return customGravityDirection;
		} else {
			if (useCenterPoint) {
				Transform centerPointToUse = transform;

				if (useCenterPointList) {
					float minDistance = Mathf.Infinity;

					for (int i = 0; i < centerPointList.Count; i++) {
						float currentDistance = GKC_Utils.distance (objectPosition, centerPointList [i].position);

						if (currentDistance < minDistance) {
							minDistance = currentDistance;
							centerPointToUse = centerPointList [i];
						}
					}

					if (useCenterIfPointListTooClose) {
						float distanceToCenter = GKC_Utils.distance (objectPosition, centerPoint.position);

						if (minDistance < distanceToCenter) {
							centerPointToUse = centerPoint;
						}
					}
				} else {
					centerPointToUse = centerPoint;
				}

				Vector3 heading = centerPointToUse.position - objectToChangeGravity.transform.position;

				float distance = heading.magnitude;

				Vector3 direction = heading / distance;

				return direction;
			} else {
				return transform.up;
			}
		}
	}

	public void reverseGravityDirection ()
	{
		useInverseDirectionToCenterPoint = !useInverseDirectionToCenterPoint;

		for (int i = 0; i < artificialObjectGravityList.Count; i++) { 
			artificialObjectGravityList [i].setUseInverseDirectionToCenterPointState (useInverseDirectionToCenterPoint);
		}
	}
}
