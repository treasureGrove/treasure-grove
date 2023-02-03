using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class meleeCombatThrowReturnWeaponSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool throwObjectEnabled = true;
	public bool returnObjectEnabled = true;

	public bool playerOnGroundToActivateThrow;
	public ForceMode throwObjectsForceMode;
	public LayerMask throwObjectsLayerToCheck;

	public bool useSplineToReturnObject;
	public BezierSpline splineToReturnObject;

	public bool disableDropObjectWhenThrownOrReturning;

	public string movingObjectsTag = "moving";

	public float generalDamageOnSurfaceDetectedOnThrow = 1;

	public bool applyDamageOnSurfaceDetectedOnReturnEnabled = true;

	public bool applyDamageOnObjectReturnPathEnabled = true;

	public float generalDamageMultiplierOnObjectReturn = 1;

	public float generalDamageMultiplierOnReturnPath = 1;

	[Space]
	[Header ("Follow Object On Throw Melee Weapon Settings")]
	[Space]

	public bool checkIfObjectToFollowOnThrowMeleeWeapon;

	public float extraWeaponOnAirTimeIfObjectToFollowDetected = 6;

	[Space]
	[Header ("Throw Object Events Settings")]
	[Space]

	public bool useEventsOnThrowReturnObject;
	public UnityEvent eventOnThrowObject;
	public UnityEvent eventOnReturnObject;

	public UnityEvent eventOnDropObjectWhenIsThrown;

	[Space]
	[Header ("Teleport Settings")]
	[Space]

	public bool teleportPlayerOnThrowEnabled = true;
	public bool grabMeleeWeaponOnTeleportPositionReached = true;
	public float teleportSpeed = 10;
	public float cameraFovOnTeleport = 70;
	public float cameraFovOnTeleportSpeed = 5;
	public float minDistanceToStopTeleport = 1;
	public bool teleportInstantlyToPosition;

	public bool useSmoothCameraFollowStateOnTeleport = true;
	public float smoothCameraFollowDuration = 3;

	[Space]

	public UnityEvent eventOnStartTeleport;
	public UnityEvent eventOnEndTeleport;

	[Space]
	[Header ("Stamina Settings")]
	[Space]

	public bool useStaminaOnThrowObjectEnabled = true;
	public string throwObjectStaminaState = "Throw Grabbed Object";
	public float staminaToUseOnThrowObject = 10;
	public float customRefillStaminaDelayAfterThrow;

	public bool useStaminaOnReturnObjectEnabled = true;
	public string returnObjectStaminaState = "Return Grabbed Object";
	public float staminaToUseOnReturnObject = 10;
	public float customRefillStaminaDelayAfterReturn;

	[Space]
	[Header ("Hold Throw Weapon Settings")]
	[Space]

	public string actionActiveAnimatorName = "Action Active";
	public string holdThrowMeleeWeaponAnimatorName = "Action ID";
	public int holdThrowMeleeWeaponAnimatorID;

	public bool useStrafeOnHoldThrowWeapon;

	public bool setNewCameraStateOnThirdPerson;

	public string newCameraStateOnThirdPerson;
	public bool returnToPreviuosViewOnEnd = true;

	public string locatedEnemyIconName = "Melee Located Enemy Icon";

	public bool useSoundOnTargetLocatedOnHoldThrowWeapon;
	public AudioClip soundOnTargetLocatedOnHoldThrowWeapon;

	[Space]
	[Space]

	public bool useEventsOnHoldThrowWeapon;
	public UnityEvent eventOnStartHoldThrowWeapon;
	public UnityEvent eventOnEndHoldThrowWeapon;

	[Space]

	public bool useEventOnTargetLocatedOnHoldThrowWeapon;
	public UnityEvent eventOnTargetLocatedOnHoldThrowWeapon;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color sphereColor = Color.green;
	public Color cubeColor = Color.blue;

	public bool showDebugDraw;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool objectThrown;

	public bool objectThrownTravellingToTarget;

	public bool continueObjecThrowActivated;

	public bool returningThrownObject;

	public bool attackInputPausedForStamina;

	public bool throwObjectInputPausedForStamina;

	public bool objectToFollowFound;
	public Transform objectToFollow;

	public Transform currentObjectToFollowFromHoldThrowWeapon;

	public bool teleportInProcess;

	public bool throwWeaponActionPaused;

	public GameObject surfaceDetecetedOnObjectThrown;

	public bool surfaceDetected;

	public bool surfaceNotFound;

	public List<Transform> locatedEnemies = new List<Transform> ();

	public List<Transform> temporalLocatedEnemies = new List<Transform> ();

	[Space]
	[Header ("Components")]
	[Space]

	public grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;

	public playerController mainPlayerController;

	public grabObjects mainGrabObjects;

	public staminaSystem mainStaminaSystem;

	public Transform mainCameraTransform;

	public sliceSystem mainSliceSystem;

	public playerTeleportSystem mainPlayerTeleportSystem;

	public GameObject playerControllerGameObject;

	public Transform handPositionReference;

	public playerCamera mainPlayerCamera;

	public AudioSource mainAudioSource;

	[Space]
	[Header ("Melee Weapon Components Debug")]
	[Space]

	public Transform currentGrabbedObjectTransform;

	public grabPhysicalObjectMeleeAttackSystem currentMeleeWeapon;

	public Rigidbody currentObjectRigidbody;

	public grabbedObjectMeleeAttackSystem.grabbedWeaponInfo currentGrabbedWeaponInfo;

	public Transform objectRotationPoint;

	public grabPhysicalObjectSystem currentGrabPhysicalObjectSystem;

	public Transform objectRotationPointParent;

	public hitCombat currentHitCombat;

	public dualWieldMeleeWeaponObjectSystem mainDualWieldMeleeWeaponObjectSystem;

	public Transform raycastCheckTransfrom;

	public BoxCollider currentHitCombatBoxCollider;




	Coroutine returnCoroutine;

	Coroutine throwCoroutine;

	float capsuleCastRadius;
	float capsuleCastDistance;

	Vector3 currentRayOriginPosition;
	Vector3 currentRayTargetPosition;

	float distanceToTarget;
	Vector3 rayDirection;

	Vector3 point1;
	Vector3 point2;

	RaycastHit[] hits;

	bool throwObjectWithRotation;

	bool returnObjectWithRotation;

	bool useSplineForReturn;

	float lastTimeObjectThrown;

	float lastTimeObjectReturn;

	Coroutine teleportCoroutine;

	float lastTimeDamageTriggerActivated;

	bool attackActivatedOnAir;

	bool throwWeaponQuicklyAndTeleportIfSurfaceFound;

	RaycastHit hitToQuicklyTeleport;

	bool surfaceToQuicklyTeleportLocated;

	float timeToReachSurfaceOnQuickTeleport;

	Vector3 teleportPosition;
	float teleportDistanceOffset;

	bool isAttachedToSurface;
	bool surfaceDetectedIsDead;

	List<GameObject> detectedObjectsOnReturn = new List<GameObject> ();

	List<GameObject> detectedObjectsOnThrow = new List<GameObject> ();



	string previousCameraState = "";

	bool strafeStateOnHoldWeaponActive;
	bool previousStrafeState;

	Coroutine locateTargetsCoroutine;

	RaycastHit locateTargetHit;

	bool locateObjectstToTrackOnHoldEnabled;

	float lastTimeThrowObjectInputPressed;

	public bool targetListToTrackActive;

	bool settingNextTargetFromListToTrack;

	GameObject currentObjectOnUpdateTargets;
	GameObject previousObjectOnUpdateTargets;



	public void setNewGrabPhysicalObjectSystem (grabPhysicalObjectSystem newGrabPhysicalObjectSystem)
	{
		currentGrabPhysicalObjectSystem = newGrabPhysicalObjectSystem;
	}

	public void setCurrentGrabbedWeaponInfo (grabbedObjectMeleeAttackSystem.grabbedWeaponInfo newCurrentGrabbedWeaponInfo)
	{
		currentGrabbedWeaponInfo = newCurrentGrabbedWeaponInfo;
	}

	public void setNewGrabPhysicalObjectMeleeAttackSystem (grabPhysicalObjectMeleeAttackSystem newGrabPhysicalObjectMeleeAttackSystem)
	{
		currentMeleeWeapon = newGrabPhysicalObjectMeleeAttackSystem;

		if (currentMeleeWeapon != null) {
			currentGrabbedObjectTransform = currentMeleeWeapon.transform;

			currentObjectRigidbody = currentGrabbedObjectTransform.GetComponent<Rigidbody> ();

			objectRotationPoint = currentMeleeWeapon.objectRotationPoint;

			objectRotationPointParent = currentMeleeWeapon.objectRotationPointParent;

			mainDualWieldMeleeWeaponObjectSystem = currentMeleeWeapon.mainDualWieldMeleeWeaponObjectSystem;

			currentHitCombat = currentMeleeWeapon.getMainHitCombat ();

			currentHitCombatBoxCollider = currentHitCombat.getMainCollider ().GetComponent<BoxCollider> ();

			raycastCheckTransfrom = currentMeleeWeapon.raycastCheckTransfrom;
		}
	}

	public void removeGrabPhysicalObjectMeleeAttackSystem ()
	{
		currentMeleeWeapon = null;

		currentGrabPhysicalObjectSystem = null;

		currentHitCombat = null;

		objectRotationPoint = null;

		objectRotationPointParent = null;

		mainDualWieldMeleeWeaponObjectSystem = null;

		cancelHoldToThrowMeleeWeapon ();
	}

	public void setThrowWeaponActionPausedState (bool state)
	{
		throwWeaponActionPaused = state;
	}

	public void inputPressDownThrowObject ()
	{
		setPressDownOrUpOnThrowObject (true);
	}

	public void inputPressUpThrowObject ()
	{
		setPressDownOrUpOnThrowObject (false);
	}

	void setPressDownOrUpOnThrowObject (bool state)
	{
		if (!mainGrabbedObjectMeleeAttackSystem.grabbedObjectMeleeAttackActive) {
			return;
		}

		if (throwWeaponActionPaused) {
			return;
		}

		if (!mainGrabbedObjectMeleeAttackSystem.canUseWeaponsInput ()) {
			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.blockActive) {
			return;
		}

		if (!currentMeleeWeapon.holdToThrowObjectEnabled) {
			if (state) {
				inputThrowOrReturnObject ();

				return;
			}
		}

		if (objectThrown) {
			inputThrowOrReturnObject ();

			removeLocatedEnemiesIcons ();

			mainGrabbedObjectMeleeAttackSystem.setMeleeAttackInputPausedState (false);

			mainGrabbedObjectMeleeAttackSystem.setBlockInputPausedState (false);

			settingNextTargetFromListToTrack = false;

			locateObjectstToTrackOnHoldEnabled = false;

			targetListToTrackActive = false;

			objectToFollow = null;

			objectToFollowFound = false;

			stopUpdateLocateTargetsCoroutine ();

			return;
		}

		if (state) {
			lastTimeThrowObjectInputPressed = Time.time;

			locateObjectstToTrackOnHoldEnabled = false;

			if (currentMeleeWeapon.locateObjectstToTrackOnHoldEnabled) {
				locateObjectstToTrackOnHoldEnabled = true;

				stopUpdateLocateTargetsCoroutine ();

				locateTargetsCoroutine = StartCoroutine (updateLocateTargetsCoroutine ());

				if (holdThrowMeleeWeaponAnimatorID != 0) {
					mainGrabbedObjectMeleeAttackSystem.mainAnimator.SetInteger (holdThrowMeleeWeaponAnimatorName, holdThrowMeleeWeaponAnimatorID);
					mainGrabbedObjectMeleeAttackSystem.mainAnimator.SetBool (actionActiveAnimatorName, true);
				}

				if (useStrafeOnHoldThrowWeapon) {
					mainPlayerController.activateOrDeactivateStrafeMode (true);

					strafeStateOnHoldWeaponActive = true;

					previousStrafeState = mainPlayerController.isStrafeModeActive ();
				}

				if (setNewCameraStateOnThirdPerson && !mainPlayerController.isPlayerOnFirstPerson () && mainPlayerCamera != null) {
					if (state) {
						previousCameraState = mainPlayerCamera.getCurrentStateName ();

						mainPlayerCamera.setCameraStateOnlyOnThirdPerson (newCameraStateOnThirdPerson);
					}
				}

				checkEventsOnHoldThrowWeapon (true);

				mainGrabbedObjectMeleeAttackSystem.setMeleeAttackInputPausedState (true);

				mainGrabbedObjectMeleeAttackSystem.setBlockInputPausedState (true);
			}
		} else {
			if (Time.time < lastTimeThrowObjectInputPressed + 0.5f) {
				inputThrowOrReturnObject ();
			} else {
				if (locateObjectstToTrackOnHoldEnabled && locatedEnemies.Count > 0) {
					targetListToTrackActive = true;

					currentObjectToFollowFromHoldThrowWeapon = locatedEnemies [0];

					Transform newTarget = applyDamage.getPlaceToShoot (currentObjectToFollowFromHoldThrowWeapon.gameObject);

					if (newTarget == null) {
						newTarget = currentObjectToFollowFromHoldThrowWeapon;
					}

					objectToFollow = newTarget;

					objectToFollowFound = true;

					locatedEnemies.RemoveAt (0);

					checkEventsOnHoldThrowWeapon (false);
				} else {
					locateObjectstToTrackOnHoldEnabled = false;
				}

				disableStatesFromHoldThrowWeapon ();

				inputThrowOrReturnObject ();
			}

			stopUpdateLocateTargetsCoroutine ();
		}
	}

	void checkEventsOnHoldThrowWeapon (bool state)
	{
		if (useEventsOnHoldThrowWeapon) {
			if (state) {
				eventOnStartHoldThrowWeapon.Invoke ();
			} else {
				eventOnEndHoldThrowWeapon.Invoke ();
			}
		}
	}

	void disableStatesFromHoldThrowWeapon ()
	{
		if (holdThrowMeleeWeaponAnimatorID != 0) {
			mainGrabbedObjectMeleeAttackSystem.mainAnimator.SetInteger (holdThrowMeleeWeaponAnimatorName, 0);
			mainGrabbedObjectMeleeAttackSystem.mainAnimator.SetBool (actionActiveAnimatorName, false);
		}

		if (useStrafeOnHoldThrowWeapon && strafeStateOnHoldWeaponActive) {
			mainPlayerController.activateOrDeactivateStrafeMode (previousStrafeState);

			strafeStateOnHoldWeaponActive = false;
		}

		if (setNewCameraStateOnThirdPerson && !mainPlayerController.isPlayerOnFirstPerson () && mainPlayerCamera != null) {
			if (previousCameraState != "") {
				if (returnToPreviuosViewOnEnd) {
					if (previousCameraState != newCameraStateOnThirdPerson) {
						mainPlayerCamera.setCameraStateOnlyOnThirdPerson (previousCameraState);
					}
				}

				previousCameraState = "";
			}
		}

		mainGrabbedObjectMeleeAttackSystem.setMeleeAttackInputPausedState (false);

		mainGrabbedObjectMeleeAttackSystem.setBlockInputPausedState (false);
	}

	void cancelHoldToThrowMeleeWeapon ()
	{
		if (locateObjectstToTrackOnHoldEnabled) {
			settingNextTargetFromListToTrack = false;

			locateObjectstToTrackOnHoldEnabled = false;

			targetListToTrackActive = false;

			objectToFollow = null;

			objectToFollowFound = false;

			removeLocatedEnemiesIcons ();


			disableStatesFromHoldThrowWeapon ();

			stopUpdateLocateTargetsCoroutine ();
		}
	}

	public void stopUpdateLocateTargetsCoroutine ()
	{
		if (locateTargetsCoroutine != null) {
			StopCoroutine (locateTargetsCoroutine);
		}
	}

	IEnumerator updateLocateTargetsCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			if (Time.time > lastTimeThrowObjectInputPressed + 0.5f) {
				//uses a ray to detect enemies, to locked them
				bool surfaceFound = false;

				if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out locateTargetHit,
					    currentMeleeWeapon.maxRaycastDistanceToLocateObjectToTrack, currentMeleeWeapon.layerToLocateObjectsToTrack)) {

					if (locateTargetHit.transform.gameObject != playerControllerGameObject) {
						surfaceFound = true;
					} else {
						if (Physics.Raycast (locateTargetHit.point + mainCameraTransform.forward * 0.2f, mainCameraTransform.forward, out locateTargetHit,
							    currentMeleeWeapon.maxRaycastDistanceToLocateObjectToTrack, currentMeleeWeapon.layerToLocateObjectsToTrack)) {
							surfaceFound = true;
						}
					}

					currentObjectOnUpdateTargets = locateTargetHit.collider.gameObject;
				} else {
					currentObjectOnUpdateTargets = null;

					previousObjectOnUpdateTargets = null;
				}

				if (surfaceFound) {
					if (previousObjectOnUpdateTargets != currentObjectOnUpdateTargets) {
						previousObjectOnUpdateTargets = currentObjectOnUpdateTargets;

						GameObject target = applyDamage.getCharacterOrVehicle (currentObjectOnUpdateTargets);

						bool objectLocated = target != null;

						if (!objectLocated) {
							meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = currentObjectOnUpdateTargets.GetComponent<meleeAttackSurfaceInfo> ();

							if (currentMeleeAttackSurfaceInfo != null) {
								target = currentMeleeAttackSurfaceInfo.gameObject;
							}
						}

						if (target != null) {
							if (showDebugPrint) {
								print ("new object detected " + target.name);
							}

							if (target != playerControllerGameObject) {
								if (!locatedEnemies.Contains (target.transform)) {
//									print (target.transform.name);

									//if an enemy is detected, add it to the list of located enemies and instantiated an icon in screen to follow the enemy
									locatedEnemies.Add (target.transform);

									temporalLocatedEnemies.Add (target.transform);

									GKC_Utils.addElementToPlayerScreenObjectivesManager (playerControllerGameObject, target, locatedEnemyIconName);
								
									if (useSoundOnTargetLocatedOnHoldThrowWeapon) {
										playSound (soundOnTargetLocatedOnHoldThrowWeapon);
									}

									if (useEventOnTargetLocatedOnHoldThrowWeapon) {
										eventOnTargetLocatedOnHoldThrowWeapon.Invoke ();
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public void playSound (AudioClip newClip)
	{
		if (mainAudioSource != null) {
			mainAudioSource.PlayOneShot (newClip);
		}
	}

	void removeLocatedEnemiesIcons ()
	{
		if (showDebugPrint) {
			print ("remove located enemies icons");
		}

		if (temporalLocatedEnemies.Count > 0) {
			GKC_Utils.removeElementListToPlayerScreenObjectivesManager (playerControllerGameObject, temporalLocatedEnemies);

			temporalLocatedEnemies.Clear ();
		}

		if (locatedEnemies.Count >= 0) {
			locatedEnemies.Clear ();
		}
	}

	public void inputThrowOrReturnObject ()
	{
		if (!mainGrabbedObjectMeleeAttackSystem.grabbedObjectMeleeAttackActive) {
			return;
		}

		if (throwWeaponActionPaused) {
			return;
		}

		if (!mainGrabbedObjectMeleeAttackSystem.canUseWeaponsInput ()) {
			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.blockActive) {
			return;
		}

		if (throwObjectInputPausedForStamina && mainGrabbedObjectMeleeAttackSystem.getGeneralStaminaUseMultiplier () > 0) {
			if (showDebugPrint) {
				print ("not enough stamina");
			}

			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ()) {
			return;
		}

		if (!objectThrown) {
			if (!mainPlayerController.isPlayerOnGround () && playerOnGroundToActivateThrow) {
				return;
			}

			throwObject ();
		} else {
			returnObject ();
		}
	}

	public void inputActivateTeleport ()
	{
		if (!mainGrabbedObjectMeleeAttackSystem.grabbedObjectMeleeAttackActive) {
			return;
		}

		if (!mainGrabbedObjectMeleeAttackSystem.canUseWeaponsInput ()) {
			return;
		}

		if (!objectThrown) {
			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.blockActive) {
			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ()) {
			return;
		}

		if (!teleportPlayerOnThrowEnabled) {
			return;
		}

		activateTeleport ();
	}

	public void throwObject ()
	{
		if (!throwObjectEnabled) {
			return;
		}

		if (objectThrown) {
			return;
		}

		if (returningThrownObject) {
			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.cuttingModeActive) {
			return;
		}

		if (currentMeleeWeapon.canThrowObject) {
			stopThrowObjectCoroutine ();

			throwCoroutine = StartCoroutine (throwObjectCoroutine ());
		}
	}

	public void setObjectThrownState (bool state)
	{
		objectThrown = state;

		mainGrabbedObjectMeleeAttackSystem.setObjectThrownState (state);
	}

	void stopThrowObjectCoroutine ()
	{
		if (throwCoroutine != null) {
			StopCoroutine (throwCoroutine);
		}
	}

	IEnumerator throwObjectCoroutine ()
	{
		setObjectThrownState (true);

		mainPlayerController.setPlayerMeleeWeaponThrownState (true);

		currentMeleeWeapon.setObjectThrownState (true);

		if (disableDropObjectWhenThrownOrReturning) {
			mainGrabObjects.setGrabObjectsInputPausedState (true);
		}

		if (!settingNextTargetFromListToTrack) {
			if (useStaminaOnThrowObjectEnabled) {
				mainStaminaSystem.activeStaminaStateWithCustomAmount (throwObjectStaminaState, 
					staminaToUseOnThrowObject * mainGrabbedObjectMeleeAttackSystem.getGeneralStaminaUseMultiplier (), customRefillStaminaDelayAfterThrow);				
			}
		}

		surfaceDetected = false;

		setSurfacecNotFoundState (false);

		mainGrabbedObjectMeleeAttackSystem.setGrabbedObjectClonnedColliderEnabledState (false);

		if (!settingNextTargetFromListToTrack) {
			if (currentMeleeWeapon.useThrowActionName) {
				mainPlayerController.activateCustomAction (currentMeleeWeapon.throwActionName);
			}
		

			if (!targetListToTrackActive) {
				RaycastHit hit;

				objectToFollowFound = false;

				objectToFollow = null;

				if (checkIfObjectToFollowOnThrowMeleeWeapon) {
					if (mainPlayerController.isPlayerLookingAtTarget ()) {
						objectToFollow = mainPlayerController.getCurrentTargetToLook ();

						if (objectToFollow != null) {
							objectToFollowFound = true;
						}
					} 

					if (!objectToFollowFound) {
						if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, throwObjectsLayerToCheck)) {

							objectToFollowOnThrowMeleeWeapon currentObjectToFollow = hit.transform.GetComponent<objectToFollowOnThrowMeleeWeapon> ();

							if (currentObjectToFollow != null) {
								objectToFollow = currentObjectToFollow.getMainObjectToFollow ();

								objectToFollowFound = true;
							}
						}
					}
				}
			}
		}

		throwWeaponQuicklyAndTeleportIfSurfaceFound = currentMeleeWeapon.throwWeaponQuicklyAndTeleportIfSurfaceFound;

		if (throwWeaponQuicklyAndTeleportIfSurfaceFound) {

			surfaceToQuicklyTeleportLocated = false;

			bool surfaceDetectedToQuickTeleport = false;

			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hitToQuicklyTeleport, Mathf.Infinity, throwObjectsLayerToCheck)) {
				if (hitToQuicklyTeleport.collider.gameObject != playerControllerGameObject) {
					surfaceDetectedToQuickTeleport = true;
				} else {
					if (Physics.Raycast (hitToQuicklyTeleport.point, mainCameraTransform.TransformDirection (Vector3.forward), out hitToQuicklyTeleport, Mathf.Infinity, throwObjectsLayerToCheck)) {
						surfaceDetectedToQuickTeleport = true;
					}
				}
			}

			if (surfaceDetectedToQuickTeleport) {
				surfaceToQuicklyTeleportLocated = true;

				float dist = GKC_Utils.distance (hitToQuicklyTeleport.point, playerControllerGameObject.transform.position);

				timeToReachSurfaceOnQuickTeleport = dist /
				(currentObjectRigidbody.mass *
				currentMeleeWeapon.throwSpeed *
				currentMeleeWeapon.extraSpeedOnThrowWeaponQuicklyAndTeleport);

				timeToReachSurfaceOnQuickTeleport *= 0.93f;

				meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = hitToQuicklyTeleport.collider.gameObject.GetComponent<meleeAttackSurfaceInfo> ();

				if (currentMeleeAttackSurfaceInfo != null) {
					if (currentMeleeAttackSurfaceInfo.disableInstaTeleportOnThisSurface) {
						throwWeaponQuicklyAndTeleportIfSurfaceFound = false;

						surfaceToQuicklyTeleportLocated = false;
					}
				}
			}
		}

		if (mainGrabbedObjectMeleeAttackSystem.shieldActive) {
			mainGrabbedObjectMeleeAttackSystem.setShieldParentState (false);
		}

		continueObjecThrowActivated = false;

		teleportPosition = Vector3.zero;

		teleportDistanceOffset = 0;

		yield return null;

		if (settingNextTargetFromListToTrack) {
			continueThrowObject ();
		} else {
			if (currentMeleeWeapon.delayToThrowObject > 0) {
				continueThrowObject ();
			}
		}
	}

	public void continueThrowObject ()
	{
		stopThrowObjectCoroutine ();

		if (!mainGrabbedObjectMeleeAttackSystem.grabbedObjectMeleeAttackActive) {
			setObjectThrownState (false);

			mainPlayerController.setPlayerMeleeWeaponThrownState (false);

			return;
		}

		throwCoroutine = StartCoroutine (continueThrowObjectCoroutine ());
	}

	IEnumerator continueThrowObjectCoroutine ()
	{
		setObjectThrownTravellingToTargetState (true);

		continueObjecThrowActivated = true;

		bool ignoreRigidbodiesOnThrowWeapon = currentMeleeWeapon.ignoreRigidbodiesOnThrowWeapon;

		if (ignoreRigidbodiesOnThrowWeapon) {
			detectedObjectsOnThrow.Clear ();
		}

		bool ignoreAttachToLocatedObjectOnThrow = currentMeleeWeapon.ignoreAttachToLocatedObjectOnThrow;

		if (mainGrabbedObjectMeleeAttackSystem.isDualWieldWeapon) {
			mainDualWieldMeleeWeaponObjectSystem.enableOrDisableDualWieldMeleeWeaponObject (false);
		}

		if (currentMeleeWeapon.delayToThrowObject > 0) {
			yield return new WaitForSeconds (currentMeleeWeapon.delayToThrowObject);
		}

		currentMeleeWeapon.checkEventOnThrow (true);

		//Disable the strafe mode
		if (currentGrabbedWeaponInfo.setPreviousStrafeModeOnDropObject) {
			mainPlayerController.activateOrDeactivateStrafeMode (currentMeleeWeapon.wasStrafeModeActivePreviously ());
		}

		if (currentGrabbedWeaponInfo.toggleStrafeModeIfRunningActive) {
			mainPlayerController.setDisableStrafeModeExternallyIfIncreaseWalkSpeedActiveState (false);
		}

		mainPlayerController.setCurrentStrafeIDValue (0);

		if (currentGrabbedWeaponInfo.useEventsOnGrabDropObject) {
			currentGrabbedWeaponInfo.eventOnDropObject.Invoke ();
		}


		checkEventOnThrowOrReturnObject (true);

		//Enable physics and throw the object
		currentGrabbedObjectTransform.SetParent (null);

		currentObjectRigidbody.useGravity = false;
		currentObjectRigidbody.isKinematic = false;

		Vector3 throwObjectDirection = mainCameraTransform.forward;

		RaycastHit hit;

		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, throwObjectsLayerToCheck)) {
			Vector3 heading = hit.point - currentObjectRigidbody.transform.position;

			float distance = heading.magnitude;
			throwObjectDirection = heading / distance;

			throwObjectDirection.Normalize ();
		}

		Vector3 forceDirection = throwObjectDirection * (currentObjectRigidbody.mass * currentMeleeWeapon.throwSpeed);

		if (surfaceToQuicklyTeleportLocated) {
			forceDirection *= currentMeleeWeapon.extraSpeedOnThrowWeaponQuicklyAndTeleport;
		}

		currentObjectRigidbody.AddForce (forceDirection, throwObjectsForceMode);

		// Check the state of the throw object
		bool targetReachedOnThrow = false;

		lastTimeObjectThrown = Time.time;

		surfaceDetecetedOnObjectThrown = null;

		capsuleCastRadius = currentMeleeWeapon.capsuleCastRadius;

		capsuleCastDistance = currentMeleeWeapon.capsuleCastDistance;

		throwObjectWithRotation = currentMeleeWeapon.throwObjectWithRotation;

		returnObjectWithRotation = currentMeleeWeapon.returnObjectWithRotation;

		useSplineForReturn = currentMeleeWeapon.useSplineForReturn;

		Quaternion targetRotation = Quaternion.LookRotation (throwObjectDirection, objectRotationPoint.up);


		float timeOnAir = currentMeleeWeapon.maxTimeOnAirIfNoSurfaceFound;

		if (checkIfObjectToFollowOnThrowMeleeWeapon && objectToFollowFound) {
			timeOnAir += extraWeaponOnAirTimeIfObjectToFollowDetected;
		}

		var waitTime = new WaitForFixedUpdate ();

		while (!targetReachedOnThrow) {
			yield return waitTime;

			if (throwObjectWithRotation) {

				bool rotateObjectActive = true;

				if (currentMeleeWeapon.rotateWeaponToSurfaceLocatedWhenCloseEnough) {
					Vector3 raycastDirection = throwObjectDirection;

					if (objectToFollowFound) {
						raycastDirection = objectToFollow.position - objectRotationPoint.position;
						raycastDirection = raycastDirection / raycastDirection.magnitude;
					}

					if (Physics.Raycast (objectRotationPoint.position, raycastDirection, out hit, 5, throwObjectsLayerToCheck)) {
						bool checkSurfaceResult = true;

						if (ignoreRigidbodiesOnThrowWeapon) {
							if (hit.collider.attachedRigidbody != null) {
								checkSurfaceResult = false;
							}
						}

						if (checkSurfaceResult) {
							Vector3 surfaceDirection = hit.point - objectRotationPoint.position;
							surfaceDirection = surfaceDirection / surfaceDirection.magnitude;

							Quaternion newTargetRotation = Quaternion.LookRotation (surfaceDirection, objectRotationPoint.up);

							objectRotationPoint.rotation = Quaternion.Lerp (objectRotationPoint.rotation, newTargetRotation, 25 * Time.deltaTime);

							rotateObjectActive = false;
						}
					} else {
						if (currentMeleeWeapon.rotateObjectToThrowDirection) {
							Quaternion newTargetRotation = Quaternion.LookRotation (throwObjectDirection, objectRotationPoint.up);

							objectRotationPoint.rotation = Quaternion.Lerp (objectRotationPoint.rotation, newTargetRotation, 10 * Time.deltaTime);
						}
					}
				} 

				if (rotateObjectActive) {
					if (!currentMeleeWeapon.rotateObjectToThrowDirection) {
						objectRotationPoint.Rotate (Vector3.right * (currentMeleeWeapon.throwObjectRotationSpeed * Time.deltaTime));
					}
				}
			} else {
				objectRotationPoint.rotation = Quaternion.Lerp (objectRotationPoint.rotation, targetRotation, 10 * Time.deltaTime);
			}

			checkSurfacesDetectedRaycast (capsuleCastRadius, true);

			if (hits.Length > 0) {
				if (showDebugPrint) {
					print (hits.Length + " " + hits [0].collider.gameObject.name);
				}

				surfaceDetecetedOnObjectThrown = hits [0].collider.gameObject;

				if (surfaceDetecetedOnObjectThrown == playerControllerGameObject) {

					if (showDebugPrint) {
						print ("player found, selecting another collider -------------------------------------------------");
					}

					if (hits.Length > 1) {
						surfaceDetecetedOnObjectThrown = hits [1].collider.gameObject;

						targetReachedOnThrow = true;
					} else {
						surfaceDetecetedOnObjectThrown = null;
					}
				} else {
					bool checkSurfaceResult = false;

					if (ignoreRigidbodiesOnThrowWeapon) {
						for (int i = 0; i < hits.Length; i++) {
							GameObject currentObject = hits [i].collider.gameObject;

							if (currentObject != playerControllerGameObject) {
								if (hits [i].collider.attachedRigidbody != null) {

									if (!detectedObjectsOnThrow.Contains (currentObject)) {
										detectedObjectsOnThrow.Add (currentObject);

										currentHitCombat.checkObjectDetected (currentObject, false);
						
										if (currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnThrowWeapon) {
											float newPushForce = currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnThrowWeaponMultiplier;

											newPushForce = Mathf.Clamp (newPushForce, 0, 6);

											Vector3 pushCharacterDirection = throwObjectDirection * newPushForce;

											currentObject.SendMessage (currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnThrowWeaponMessageNameToSend,
												pushCharacterDirection, SendMessageOptions.DontRequireReceiver);
										}
									}
								} else {
									checkSurfaceResult = true;

									surfaceDetecetedOnObjectThrown = currentObject;
								}
							}
						}
					} else {
						checkSurfaceResult = true;
					}

					if (checkSurfaceResult) {
						if (!targetListToTrackActive) {
							targetReachedOnThrow = true;
						}
					}
				}
			}

			if (!targetReachedOnThrow) {
				if (throwWeaponQuicklyAndTeleportIfSurfaceFound) {
					if (surfaceToQuicklyTeleportLocated) {
						float currentObjectDistance = GKC_Utils.distance (hitToQuicklyTeleport.point, currentGrabbedObjectTransform.position);

						if (currentObjectDistance < 2 || Time.time > timeToReachSurfaceOnQuickTeleport + lastTimeObjectThrown) {
							currentObjectRigidbody.velocity = Vector3.zero;
							currentObjectRigidbody.isKinematic = true;

							surfaceDetecetedOnObjectThrown = hitToQuicklyTeleport.collider.gameObject;

							targetReachedOnThrow = true;
						}
					}
				}

				if (Physics.Raycast (objectRotationPoint.position, throwObjectDirection, out hit, 1.5f, throwObjectsLayerToCheck)) {
					if (hit.collider.gameObject != playerControllerGameObject) {
						bool checkSurfaceResult = true;

						if (ignoreRigidbodiesOnThrowWeapon) {
							if (hit.collider.attachedRigidbody != null) {
								checkSurfaceResult = false;
							}
						}

						if (checkSurfaceResult) {
							if (showDebugPrint) {
								print ("player found, selecting another collider -------------------------------------------------");
							}

							surfaceDetecetedOnObjectThrown = hit.collider.gameObject;

							if (!targetListToTrackActive) {
								targetReachedOnThrow = true;
							}
						}
					}
				}
			}

			if (objectToFollowFound && objectToFollow != null) {
				Vector3 nextObjectPosition = objectToFollow.position;

				Vector3 currentObjectPosition = currentGrabbedObjectTransform.position;

				Vector3 currentSpeed = (nextObjectPosition - currentObjectPosition);

				currentSpeed.Normalize ();

				currentSpeed *= currentMeleeWeapon.followObjectOnThrowMeleeWeaponSpeed;

				currentObjectRigidbody.velocity = currentSpeed;

				if (locateObjectstToTrackOnHoldEnabled && !targetReachedOnThrow) {
					float distanceToTarget = GKC_Utils.distance (nextObjectPosition, currentObjectPosition);

					Vector3 heading = nextObjectPosition - currentObjectRigidbody.transform.position;

					float distance = heading.magnitude;
					Vector3 currentDirection = heading / distance;

					bool objectLocatedWithRaycastResult = false;

					if (Physics.Raycast (objectRotationPoint.position, currentDirection, out locateTargetHit, 1.5f, currentMeleeWeapon.layerToLocateObjectsToTrack)) {
						if (locateTargetHit.collider.gameObject == currentObjectToFollowFromHoldThrowWeapon) {
							objectLocatedWithRaycastResult = true;
						}
					}

					if (distanceToTarget < 1 || objectLocatedWithRaycastResult) {
						if (currentObjectToFollowFromHoldThrowWeapon != null) {
							currentHitCombat.checkObjectDetected (currentObjectToFollowFromHoldThrowWeapon.gameObject, false);

							if (currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeapon) {

								currentDirection.Normalize ();

								currentHitCombat.setCustomForceDirection (currentDirection);
								currentHitCombat.setCustomForceAmount (currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeaponMultiplier);

								float newPushForce = currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeaponMultiplier;

								newPushForce = Mathf.Clamp (newPushForce, 0, 6);

								Vector3 pushCharacterDirection = currentDirection * newPushForce;

								currentObjectToFollowFromHoldThrowWeapon.SendMessage (currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeaponMessageNameToSend,
									pushCharacterDirection, SendMessageOptions.DontRequireReceiver);
							}

							targetReachedOnThrow = true;
						}
					} else {
						GameObject currentDetected = currentHitCombat.getLastSurfaceDetected ();

						if (currentDetected != null) {
							if (currentDetected == currentObjectToFollowFromHoldThrowWeapon && currentDetected != playerControllerGameObject) {
								targetReachedOnThrow = true;
							}
						}
					}

					if (!targetReachedOnThrow) {
						if (detectedObjectsOnThrow.Count > 0 && currentObjectToFollowFromHoldThrowWeapon != null &&
						    detectedObjectsOnThrow.Contains (currentObjectToFollowFromHoldThrowWeapon.gameObject)) {

							targetReachedOnThrow = true;
						}
					}
				}
			}

			if (!targetListToTrackActive) {
				if (Time.time > timeOnAir + lastTimeObjectThrown) {
					targetReachedOnThrow = true;

					setSurfacecNotFoundState (true);
				}
			}

			if (targetReachedOnThrow && surfaceDetecetedOnObjectThrown != null) {
				if (currentMeleeWeapon.useCutOnThrowObject) {
					if (showDebugPrint) {
						print ("activating cut on throw weapon on surface reached ");
					}

					Vector3 cutOverlapBoxSize = currentMeleeWeapon.cutOverlapBoxSize;
					Transform cutPositionTransform = currentMeleeWeapon.cutPositionTransform;
					Transform cutDirectionTransform = currentMeleeWeapon.cutDirectionTransform;

					Transform planeDefiner1 = currentMeleeWeapon.planeDefiner1;
					Transform planeDefiner2 = currentMeleeWeapon.planeDefiner2;
					Transform planeDefiner3 = currentMeleeWeapon.planeDefiner3;

					mainSliceSystem.setCustomCutTransformValues (cutOverlapBoxSize, cutPositionTransform, cutDirectionTransform,
						planeDefiner1, planeDefiner2, planeDefiner3);

					mainSliceSystem.activateCutExternally ();

					if (mainSliceSystem.anyObjectDetectedOnLastSlice ()) {
						surfaceDetecetedOnObjectThrown = null;
						targetReachedOnThrow = false;

						setSurfacecNotFoundState (true);
					}
				}
			}
		}

		isAttachedToSurface = false;

		surfaceDetectedIsDead = false;

		bool isAttachedToCharacter = false;

		currentGrabPhysicalObjectSystem.setLastParentAssigned (null);

		if (targetReachedOnThrow) {
			if (surfaceDetecetedOnObjectThrown != null) {
				applyForceOnObjectDetectedOnThrowWeapon (surfaceDetecetedOnObjectThrown);

				surfaceDetected = true;

				if (!targetListToTrackActive) {

					if (!ignoreAttachToLocatedObjectOnThrow) {
						if (applyDamage.objectCanBeDamaged (surfaceDetecetedOnObjectThrown)) {
							if (!applyDamage.checkIfDead (surfaceDetecetedOnObjectThrown)) {
								isAttachedToCharacter = applyDamage.attachObjectToSurfaceFound (surfaceDetecetedOnObjectThrown.transform, currentGrabbedObjectTransform, 
									currentMeleeWeapon.mainDamagePositionTransform.position, false);

								if (showDebugPrint) {
									print ("target not dead");
								}

								isAttachedToSurface = true;
							} else {
								surfaceDetectedIsDead = true;

								if (showDebugPrint) {
									print ("target killed");
								}
							}
						} else {
							if (showDebugPrint) {
								print ("target dead");
							}

							isAttachedToSurface = true;
						}
					} 
					
					if (!surfaceDetectedIsDead) {
						meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = surfaceDetecetedOnObjectThrown.GetComponent<meleeAttackSurfaceInfo> ();

						if (currentMeleeAttackSurfaceInfo != null) {
							if (currentMeleeAttackSurfaceInfo.setAttachMeleeWeaponOnSurfaceValue) {
								if (currentMeleeAttackSurfaceInfo.attachMeleeWeaponOnSurfaceValue) {
									isAttachedToSurface = true;

									ignoreAttachToLocatedObjectOnThrow = false;
								} else {
									isAttachedToSurface = false;
								}
							}
						}
					}
				}
			}
		}

		if (showDebugPrint) {
			print ("isAttachedToSurface " + isAttachedToSurface + " surface detected is dead " + surfaceDetectedIsDead);
		}

		if (surfaceDetecetedOnObjectThrown != null) {
			currentObjectRigidbody.useGravity = !isAttachedToSurface;
			currentObjectRigidbody.isKinematic = isAttachedToSurface;

			currentObjectRigidbody.freezeRotation = isAttachedToSurface;

			if (showDebugPrint) {
				print (" kinematic " + isAttachedToSurface);
			}

			if (!ignoreAttachToLocatedObjectOnThrow) {
				Rigidbody detectedObjectRigidbody = surfaceDetecetedOnObjectThrown.GetComponent<Rigidbody> ();

				if (!surfaceDetectedIsDead && !isAttachedToCharacter && (surfaceDetecetedOnObjectThrown.CompareTag (movingObjectsTag) || detectedObjectRigidbody != null)) {
					applyDamage.checkParentToAssign (currentGrabbedObjectTransform, surfaceDetecetedOnObjectThrown.transform);
				}
			}

			checkSurfaceOnThrowWeapon (surfaceDetecetedOnObjectThrown);

			currentGrabPhysicalObjectSystem.setLastParentAssigned (currentGrabbedObjectTransform.parent);


			checkRemoteEvents (true, surfaceDetecetedOnObjectThrown);
		} else {
			currentObjectRigidbody.useGravity = true;
			currentObjectRigidbody.freezeRotation = false;
		}

		if (!targetListToTrackActive) {
			if (!isAttachedToSurface) {
				currentMeleeWeapon.setMainObjectColliderEnabledState (true);
			}
		}

		Transform originalParent = currentGrabbedObjectTransform.parent;

		Vector3 originalPosition = objectRotationPointParent.localPosition;

		objectRotationPoint.SetParent (null);

		currentGrabbedObjectTransform.SetParent (objectRotationPoint);

		currentGrabbedObjectTransform.localEulerAngles = objectRotationPointParent.localEulerAngles * -1;

		if (objectRotationPointParent.localEulerAngles == Vector3.zero) {
			currentGrabbedObjectTransform.localPosition = originalPosition * (-1);
		} else {
			originalPosition = new Vector3 (0, 0, originalPosition.y);

			currentGrabbedObjectTransform.localPosition = originalPosition * (-1);
		}

		currentGrabbedObjectTransform.SetParent (originalParent);

		objectRotationPoint.SetParent (objectRotationPointParent);

		//If the object can't return after checking surfaces due to not being configured like that, then drop the object in its current state
		if (!currentMeleeWeapon.canReturnObject) {
			if (showDebugPrint) {
				print ("object can't return, setting next state");
			}

			Transform grabbedObjectTransform = currentGrabbedObjectTransform;
			Transform currentParent = currentGrabbedObjectTransform.parent;

			Rigidbody currentGrabbedObjectRigidbody = currentObjectRigidbody;

			bool canReUseObjectIfNotReturnActive = currentMeleeWeapon.canReUseObjectIfNotReturnActive;

			Collider grabbedObjectMainTrigger = currentGrabPhysicalObjectSystem.grabObjectTrigger;

			mainGrabObjects.dropObject ();

			if (isAttachedToSurface) {
				grabbedObjectTransform.SetParent (currentParent);
				currentGrabbedObjectRigidbody.useGravity = false;
				currentGrabbedObjectRigidbody.isKinematic = true;

				if (showDebugPrint) {
					print ("is fixed on surface");
				}
			}

			if (!canReUseObjectIfNotReturnActive) {
				if (showDebugPrint) {
					print ("disabling grabbing properties");
				}

				grabbedObjectMainTrigger.enabled = false;

				mainGrabObjects.removeCurrentPhysicalObjectToGrabFound (grabbedObjectTransform.gameObject);
			}
		}

		currentMeleeWeapon.checkEventOnThrow (false);

		if (showDebugPrint) {
			print ("isAttachedToCharacter " + isAttachedToCharacter);
		}

		if (isAttachedToCharacter) {
			targetReachedOnThrow = false;

			float t = 0;

			float positionDifference = 0;
			float angleDifference = 0;

			float movementTimer = 0;

			float attachToSurfaceAdjustSpeed = currentMeleeWeapon.attachToSurfaceAdjustSpeed;

			Vector3 targetPosition = Vector3.zero;
			targetRotation = Quaternion.identity;

			if (currentMeleeWeapon.attachToCharactersReferenceTransform) {
				targetPosition = currentMeleeWeapon.attachToCharactersReferenceTransform.localPosition;
				targetRotation = currentMeleeWeapon.attachToCharactersReferenceTransform.localRotation;
			}

			while (!targetReachedOnThrow) {
				t += Time.deltaTime / attachToSurfaceAdjustSpeed; 

				currentGrabbedObjectTransform.localPosition = Vector3.Lerp (currentGrabbedObjectTransform.localPosition, targetPosition, t);

				currentGrabbedObjectTransform.localRotation = Quaternion.Lerp (currentGrabbedObjectTransform.localRotation, targetRotation, t);

				positionDifference = GKC_Utils.distance (currentGrabbedObjectTransform.localPosition, targetPosition);

				angleDifference = Quaternion.Angle (currentGrabbedObjectTransform.localRotation, targetRotation);

				movementTimer += Time.deltaTime;

				if ((positionDifference < 0.01f && angleDifference < 0.02f) || movementTimer > 3) {
					targetReachedOnThrow = true;
				}

				yield return null;
			}
		}

		setObjectThrownTravellingToTargetState (false);

		bool autoReturnActivated = false;

		if (surfaceDetected) {
			if (currentMeleeWeapon.returnWeaponIfObjectDetected) {
				inputThrowOrReturnObject ();

				autoReturnActivated = true;
			}
		}

		if (!autoReturnActivated) {
			if (throwWeaponQuicklyAndTeleportIfSurfaceFound) {
				if (surfaceToQuicklyTeleportLocated) {
					bool disableInstaTeleportOnThisSurface = false;

					if (surfaceDetecetedOnObjectThrown != null) {

					}

					if (!disableInstaTeleportOnThisSurface) {
						inputActivateTeleport ();
					}
				}
			}
		}

		settingNextTargetFromListToTrack = false;

		if (showDebugPrint) {
			print (targetListToTrackActive);
		}

		if (targetListToTrackActive) {
			for (int i = locatedEnemies.Count - 1; i >= 0; i--) {	
				if (locatedEnemies [i] == null) {
					locatedEnemies.RemoveAt (i);
				}
			}

			if (objectToFollow != null) {
				GKC_Utils.removeElementToPlayerScreenObjectivesManager (playerControllerGameObject, currentObjectToFollowFromHoldThrowWeapon.gameObject);
			}

			if (locatedEnemies.Count > 0) {
				settingNextTargetFromListToTrack = true;

				objectToFollowFound = true;

				currentObjectToFollowFromHoldThrowWeapon = locatedEnemies [0];

				locatedEnemies.RemoveAt (0);

				Transform newTarget = applyDamage.getPlaceToShoot (currentObjectToFollowFromHoldThrowWeapon.gameObject);

				if (newTarget == null) {
					newTarget = currentObjectToFollowFromHoldThrowWeapon;
				}

				objectToFollow = newTarget;

				stopThrowObjectCoroutine ();

				throwCoroutine = StartCoroutine (throwObjectCoroutine ());
			} else {
				targetListToTrackActive = false;

				removeLocatedEnemiesIcons ();

				stopReturnObjectCoroutine ();

				returnCoroutine = StartCoroutine (returnObjectCoroutine ());
			}
		}
	}

	void checkRemoteEvents (bool state, GameObject objectDetected)
	{
		if (state) {
			if (currentMeleeWeapon.useRemoteEventOnThrow) {
				remoteEventSystem currentRemoteEventSystem = objectDetected.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {

					for (int i = 0; i < currentMeleeWeapon.remoteEventOnThrowNameList.Count; i++) {
						currentRemoteEventSystem.callRemoteEvent (currentMeleeWeapon.remoteEventOnThrowNameList [i]);
					}
				}
			}
		} else {
			if (currentMeleeWeapon.useRemoteEventOnReturn) {
				remoteEventSystem currentRemoteEventSystem = objectDetected.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {

					for (int i = 0; i < currentMeleeWeapon.remoteEventOnReturnNameList.Count; i++) {
						currentRemoteEventSystem.callRemoteEvent (currentMeleeWeapon.remoteEventOnReturnNameList [i]);
					}
				}
			}
		}
	}

	public void checkSurfaceOnThrowWeapon (GameObject objectToCheck)
	{
		bool surfaceLocated = true;

		string surfaceName = mainGrabbedObjectMeleeAttackSystem.surfaceInfoOnMeleeAttackNameForSwingOnAir;

		Vector3 attackPosition = Vector3.zero;
		Vector3 attackNormal = Vector3.zero;

		RaycastHit hit = new RaycastHit ();

		GameObject surfaceFound = null;

		Transform raycastTransform = objectRotationPoint;

		Vector3 raycastPosition = raycastTransform.position;

		Vector3 raycastDirection = objectToCheck.transform.position - raycastPosition;

		raycastDirection = raycastDirection / raycastDirection.magnitude;

		float currentRaycastDistance = GKC_Utils.distance (raycastPosition, objectToCheck.transform.position);

		currentRaycastDistance += 1;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, currentRaycastDistance, currentHitCombat.layerMask)) {
			surfaceFound = hit.collider.gameObject;
		}

		if (surfaceFound != null) {
			attackPosition = hit.point;
			attackNormal = hit.normal;

			meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = surfaceFound.GetComponent<meleeAttackSurfaceInfo> ();

			if (currentMeleeAttackSurfaceInfo != null) {
				if (!currentMeleeAttackSurfaceInfo.isSurfaceEnabled ()) {
					surfaceLocated = false;
				}

				surfaceName = currentMeleeAttackSurfaceInfo.getSurfaceName ();

				currentMeleeAttackSurfaceInfo.checkEventOnSurfaceDetected ();

				currentMeleeAttackSurfaceInfo.checkEventOnThrowWeapon ();
			} else {
				GameObject currentCharacter = applyDamage.getCharacterOrVehicle (surfaceFound);

				if (currentCharacter != null) {
					currentMeleeAttackSurfaceInfo = currentCharacter.GetComponent<meleeAttackSurfaceInfo> ();

					if (currentMeleeAttackSurfaceInfo != null) {
						if (!currentMeleeAttackSurfaceInfo.isSurfaceEnabled ()) {
							surfaceLocated = false;
						}

						surfaceName = currentMeleeAttackSurfaceInfo.getSurfaceName ();

						currentMeleeAttackSurfaceInfo.checkEventOnSurfaceDetected ();

						currentMeleeAttackSurfaceInfo.checkEventOnThrowWeapon ();
					}
				} else {
					surfaceLocated = false;
				}

				if (!surfaceLocated) {
					return;
				}
			}
		} 

		mainGrabbedObjectMeleeAttackSystem.checkSurfaceFoundOnAttackToProcess (surfaceName, true, attackPosition, attackNormal, true, false);
	}

	void applyForceOnObjectDetectedOnThrowWeapon (GameObject objectToCheck)
	{
		if (currentMeleeWeapon.damageOnSurfaceDetectedOnThrow > 0) {
			currentHitCombat.setNewHitDamage (currentMeleeWeapon.damageOnSurfaceDetectedOnThrow * generalDamageOnSurfaceDetectedOnThrow);
		}

		Vector3	forceDirection = objectToCheck.transform.position - currentGrabbedObjectTransform.position;
		forceDirection = forceDirection / forceDirection.magnitude;

		currentHitCombat.setCustomForceDirection (forceDirection);
		currentHitCombat.setCustomForceAmount (currentMeleeWeapon.forceToApplyToSurfaceFound);
		currentHitCombat.setCustomForceToVehiclesMultiplier (currentMeleeWeapon.forceExtraToApplyOnVehiclesFound);

		checkSurfacesDetectedRaycast (capsuleCastRadius + 0.05f, true);

		for (int i = 0; i < hits.Length; i++) {
			GameObject currentObject = hits [i].collider.gameObject;

			if (currentObject != objectToCheck) {
				currentHitCombat.activateDamage (currentObject);
			}
		}

		currentHitCombat.setCurrentState (false);

		currentHitCombat.checkObjectDetected (objectToCheck, false);

		currentHitCombat.setCustomForceDirection (Vector3.zero);
		currentHitCombat.setCustomForceAmount (0);
		currentHitCombat.setCustomForceToVehiclesMultiplier (0);
	}

	void checkSurfacesDetectedRaycast (float capsuleRadius, bool enableMainHitCombat)
	{
		if (enableMainHitCombat) {
			currentRayOriginPosition = raycastCheckTransfrom.position;
			currentRayTargetPosition = currentRayOriginPosition + raycastCheckTransfrom.forward * capsuleCastDistance;
		} else {
			currentRayOriginPosition = mainDualWieldMeleeWeaponObjectSystem.raycastCheckTransfrom.position;
			currentRayTargetPosition = currentRayOriginPosition + raycastCheckTransfrom.forward * capsuleCastDistance;
		}

		distanceToTarget = GKC_Utils.distance (currentRayOriginPosition, currentRayTargetPosition);
		rayDirection = currentRayOriginPosition - currentRayTargetPosition;
		rayDirection = rayDirection / rayDirection.magnitude;

		if (showDebugDraw) {
			Debug.DrawLine (currentRayTargetPosition, (rayDirection * distanceToTarget) + currentRayTargetPosition, Color.red, 2);
		}

		point1 = currentRayOriginPosition - rayDirection * capsuleCastRadius;
		point2 = currentRayTargetPosition + rayDirection * capsuleCastRadius;

		hits = Physics.CapsuleCastAll (point1, point2, capsuleRadius, rayDirection, 0, currentHitCombat.layerMask);
	}

	public void returnObject ()
	{
		if (!returnObjectEnabled) {
			return;
		}

		if (!objectThrown) {
			return;
		}

		if (returningThrownObject) {
			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.cuttingModeActive) {
			return;
		}

		if (currentMeleeWeapon.canReturnObject) {
			bool canActivateReturn = false;

			if (surfaceDetected || surfaceNotFound) {
				canActivateReturn = true;
			}

			if (currentMeleeWeapon.canReturnWeaponIfNoSurfaceFound) {
				if (currentMeleeWeapon.ignoreIfSurfaceFoundToReturnWeapon || surfaceDetecetedOnObjectThrown == null) {
					if (Time.time > 0.6f + lastTimeObjectThrown) {

						stopThrowObjectCoroutine ();

						canActivateReturn = true;
					}
				}
			}

			if (canActivateReturn) {
				stopReturnObjectCoroutine ();

				returnCoroutine = StartCoroutine (returnObjectCoroutine ());
			} else {
				if (showDebugPrint) {
					print ("can't activate return " + surfaceDetected + " " + surfaceNotFound + " " + (surfaceDetecetedOnObjectThrown != null));
				}
			}
		}
	}

	void stopReturnObjectCoroutine ()
	{
		if (returnCoroutine != null) {
			StopCoroutine (returnCoroutine);
		}
	}

	IEnumerator returnObjectCoroutine ()
	{
		surfaceDetected = false;

		setSurfacecNotFoundState (false);

		if (useStaminaOnReturnObjectEnabled) {
			mainStaminaSystem.activeStaminaStateWithCustomAmount (returnObjectStaminaState, 
				staminaToUseOnReturnObject * mainGrabbedObjectMeleeAttackSystem.getGeneralStaminaUseMultiplier (), customRefillStaminaDelayAfterReturn);				
		}

		if (currentMeleeWeapon.useStartReturnActionName) {
			mainPlayerController.activateCustomAction (currentMeleeWeapon.startReturnActionName);
		}

		yield return new WaitForSeconds (currentMeleeWeapon.delayToReturnObject);

		currentMeleeWeapon.setMainObjectColliderEnabledState (false);

		currentObjectRigidbody.useGravity = false;
		currentObjectRigidbody.isKinematic = true;

		currentHitCombat.setCurrentExtraDamageValue (0);

		if (surfaceDetecetedOnObjectThrown != null) {
			meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = surfaceDetecetedOnObjectThrown.GetComponent<meleeAttackSurfaceInfo> ();

			if (currentMeleeAttackSurfaceInfo != null) {
				currentMeleeAttackSurfaceInfo.checkEventOnReturnWeapon ();
			} else {
				GameObject currentCharacter = applyDamage.getCharacterOrVehicle (surfaceDetecetedOnObjectThrown);

				if (currentCharacter != null) {
					currentMeleeAttackSurfaceInfo = currentCharacter.GetComponent<meleeAttackSurfaceInfo> ();

					if (currentMeleeAttackSurfaceInfo != null) {
						currentMeleeAttackSurfaceInfo.checkEventOnReturnWeapon ();
					}
				}
			}

			checkRemoteEvents (false, surfaceDetecetedOnObjectThrown);
		}

		if (applyDamageOnSurfaceDetectedOnReturnEnabled && isAttachedToSurface && currentMeleeWeapon.applyDamageOnSurfaceDetectedOnReturn) {
			if (surfaceDetecetedOnObjectThrown != null) {
				if (currentHitCombat.checkObjectDetection (surfaceDetecetedOnObjectThrown, false)) {
					currentHitCombat.setNewHitDamage (currentMeleeWeapon.damageOnSurfaceDetectedOnReturn * generalDamageMultiplierOnObjectReturn);

					currentHitCombat.activateDamage (surfaceDetecetedOnObjectThrown);
				}
			}
		}

		if (currentMeleeWeapon.applyDamageOnObjectReturnPath) {
			currentHitCombat.setNewHitDamage (currentMeleeWeapon.damageOnObjectReturnPath * generalDamageMultiplierOnReturnPath);

			detectedObjectsOnReturn.Clear ();
		}

		returningThrownObject = true;

		currentMeleeWeapon.checkEventOnReturn (true);

		Transform currentHandForObject = currentGrabPhysicalObjectSystem.getCurrentObjectParent ();

		Transform referencePositionThirdPerson = currentGrabPhysicalObjectSystem.getReferencePositionThirdPerson ();

		if (currentGrabbedWeaponInfo.useCustomGrabbedWeaponReferencePosition) {
			referencePositionThirdPerson = currentGrabbedWeaponInfo.customGrabbedWeaponReferencePosition;
		}

		handPositionReference.localRotation = referencePositionThirdPerson.localRotation;
		handPositionReference.localPosition = referencePositionThirdPerson.localPosition;

		float dist = GKC_Utils.distance (handPositionReference.position, currentGrabbedObjectTransform.position);

		float duration = dist / currentMeleeWeapon.returnSpeed;

		if (useSplineForReturn) {
			duration = dist / currentMeleeWeapon.returnSplineSpeed;
		}

		float t = 0;

		bool targetReached = false;

		float angleDifference = 0;

		float positionDifference = 0;

		float movementTimer = 0;

		bool objectCloseEnough = false;

		bool activateCatchObjectAction = false;

		bool resetObjectRotationPointRotation = false;

		float progress = 0;

		if (useSplineToReturnObject) {
			splineToReturnObject.transform.LookAt (currentGrabbedObjectTransform.position);

			splineToReturnObject.transform.localEulerAngles = new Vector3 (0, splineToReturnObject.transform.localEulerAngles.y, 0);

			splineToReturnObject.setInitialSplinePoint (currentGrabbedObjectTransform.position);
		}

		Vector3 targetPosition = handPositionReference.position;

		Quaternion targetRotation = handPositionReference.rotation;

		while (!targetReached) {
			t += Time.deltaTime / duration; 

			if (resetObjectRotationPointRotation) {
				objectRotationPoint.localRotation = Quaternion.Lerp (objectRotationPoint.localRotation, Quaternion.identity, t);
			} else {
				if (returnObjectWithRotation) {
					objectRotationPoint.Rotate (Vector3.right * (currentMeleeWeapon.returnObjectRotationSpeed * Time.deltaTime));
				} else {
					Vector3 rotationPointTargetDirection = objectRotationPoint.position - playerControllerGameObject.transform.position;
					rotationPointTargetDirection = rotationPointTargetDirection / rotationPointTargetDirection.magnitude;

					Quaternion rotationPointTargetRotation = Quaternion.LookRotation (rotationPointTargetDirection, objectRotationPoint.up);

					objectRotationPoint.rotation = Quaternion.Lerp (objectRotationPoint.rotation, rotationPointTargetRotation, t);
				}
			}

			if (useSplineToReturnObject && !objectCloseEnough && useSplineForReturn) {
				progress += Time.deltaTime / currentMeleeWeapon.returnSplineSpeed;

				Vector3 position = splineToReturnObject.GetPoint (progress);
				currentGrabbedObjectTransform.position = position;
			} else {
				targetPosition = handPositionReference.position;

				targetRotation = handPositionReference.rotation;

				currentGrabbedObjectTransform.position = Vector3.Lerp (currentGrabbedObjectTransform.position, targetPosition, t);
				currentGrabbedObjectTransform.rotation = Quaternion.Lerp (currentGrabbedObjectTransform.rotation, targetRotation, t);
			}

			angleDifference = Quaternion.Angle (currentGrabbedObjectTransform.rotation, targetRotation);

			positionDifference = GKC_Utils.distance (currentGrabbedObjectTransform.position, targetPosition);

			if (positionDifference < 4 || progress > 0.60f) {
				resetObjectRotationPointRotation = true;
			}

			if (positionDifference < 2.5 || progress > 0.70f) {
				if (!objectCloseEnough) {

					dist = GKC_Utils.distance (handPositionReference.position, currentGrabbedObjectTransform.position);

					duration = dist / currentMeleeWeapon.resetObjectRotationSpeed;

					objectCloseEnough = true;
				}
			}

			if (positionDifference < 0.1f || progress > 0.95f) {
				if (!activateCatchObjectAction) {

					mainGrabbedObjectMeleeAttackSystem.checkGrabbedWeaponInfoStateAtStart (currentMeleeWeapon.weaponInfoName, false);

					if (currentMeleeWeapon.useStartReturnActionName) {
						mainPlayerController.stopCustomAction (currentMeleeWeapon.startReturnActionName);
					}

					if (currentMeleeWeapon.useEndReturnActionName) {
						mainPlayerController.activateCustomAction (currentMeleeWeapon.endReturnActionName);
					}

					activateCatchObjectAction = true;
				}
			}

			movementTimer += Time.deltaTime;

			if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 2)) {
				targetReached = true;
			}

			if (applyDamageOnObjectReturnPathEnabled && currentMeleeWeapon.applyDamageOnObjectReturnPath) {
				checkSurfacesDetectedRaycast (capsuleCastRadius, true);

				if (hits.Length > 0) {
					for (int i = 0; i < hits.Length; i++) {
						GameObject currentObject = hits [i].collider.gameObject;

						if (currentObject != playerControllerGameObject && !detectedObjectsOnReturn.Contains (currentObject)) {

							detectedObjectsOnReturn.Add (currentObject);

							currentHitCombat.checkObjectDetected (currentObject, false);

							if (currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeapon) {
								Vector3 heading = targetPosition - currentObjectRigidbody.transform.position;
							
								float distance = heading.magnitude;
								Vector3 currentDirection = heading / distance;

								currentDirection.Normalize ();


								currentHitCombat.setCustomForceDirection (currentDirection);
								currentHitCombat.setCustomForceAmount (currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeaponMultiplier);

								float newPushForce = currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeaponMultiplier;

								newPushForce = Mathf.Clamp (newPushForce, 0, 6);

								Vector3 pushCharacterDirection = currentDirection * newPushForce;

								currentObject.SendMessage (currentMeleeWeapon.pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeaponMessageNameToSend,
									pushCharacterDirection, SendMessageOptions.DontRequireReceiver);
							}
						}
					}
				}
			}

			if (currentMeleeWeapon.useCutOnReturnObject) {
				Vector3 cutOverlapBoxSize = currentMeleeWeapon.cutOverlapBoxSize;
				Transform cutPositionTransform = currentMeleeWeapon.cutPositionTransform;
				Transform cutDirectionTransform = currentMeleeWeapon.cutDirectionTransform;

				Transform planeDefiner1 = currentMeleeWeapon.planeDefiner1;
				Transform planeDefiner2 = currentMeleeWeapon.planeDefiner2;
				Transform planeDefiner3 = currentMeleeWeapon.planeDefiner3;

				mainSliceSystem.setCustomCutTransformValues (cutOverlapBoxSize, cutPositionTransform, cutDirectionTransform,
					planeDefiner1, planeDefiner2, planeDefiner3);

				mainSliceSystem.activateCutExternally ();
			}

			yield return null;
		}

		currentGrabbedObjectTransform.SetParent (currentHandForObject);

		currentGrabbedObjectTransform.localRotation = referencePositionThirdPerson.localRotation;
		currentGrabbedObjectTransform.localPosition = referencePositionThirdPerson.localPosition;

		objectRotationPoint.localRotation = Quaternion.identity;

		returningThrownObject = false;

		currentMeleeWeapon.checkEventOnReturn (false);

		currentMeleeWeapon.setObjectThrownState (false);

		setObjectThrownState (false);

		mainPlayerController.setPlayerMeleeWeaponThrownState (false);

		if (!currentMeleeWeapon.disableMeleeObjectCollider) {
			mainGrabbedObjectMeleeAttackSystem.setGrabbedObjectClonnedColliderEnabledState (true);
		}

		mainGrabObjects.setGrabObjectsInputPausedState (false);

		checkEventOnThrowOrReturnObject (false);

		setObjectThrownTravellingToTargetState (false);

		lastTimeObjectReturn = Time.time;

		currentHitCombatBoxCollider = null;

		if (mainGrabbedObjectMeleeAttackSystem.shieldActive) {
			mainGrabbedObjectMeleeAttackSystem.setShieldParentState (true);
		}

		if (mainGrabbedObjectMeleeAttackSystem.isDualWieldWeapon) {
			mainDualWieldMeleeWeaponObjectSystem.enableOrDisableDualWieldMeleeWeaponObject (true);
		}

		currentHitCombat.setCustomForceDirection (Vector3.zero);
		currentHitCombat.setCustomForceAmount (0);
		currentHitCombat.setCustomForceToVehiclesMultiplier (0);
	}

	public float getLastTimeObjectReturn ()
	{
		return lastTimeObjectReturn;
	}

	public void checkEventOnThrowOrReturnObject (bool state)
	{
		if (useEventsOnThrowReturnObject) {
			if (state) {
				eventOnThrowObject.Invoke ();
			} else {
				eventOnReturnObject.Invoke ();
			}
		}
	}

	public void cancelThrowObject ()
	{
		stopThrowObjectCoroutine ();

		setObjectThrownState (false);

		mainPlayerController.setPlayerMeleeWeaponThrownState (false);

		currentMeleeWeapon.setObjectThrownState (false);

		if (mainGrabbedObjectMeleeAttackSystem.shieldActive) {
			mainGrabbedObjectMeleeAttackSystem.setShieldParentState (true);
		}

		if (showDebugPrint) {
			print ("cancel throw object");
		}

		if (locateObjectstToTrackOnHoldEnabled) {
			cancelHoldToThrowMeleeWeapon ();
		}
	}

	public void activateTeleport ()
	{
		stopActivateTeleportCoroutine ();

		if (surfaceDetecetedOnObjectThrown != null) {
			meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = surfaceDetecetedOnObjectThrown.GetComponent<meleeAttackSurfaceInfo> ();

			if (currentMeleeAttackSurfaceInfo != null) {
				if (currentMeleeAttackSurfaceInfo.useOffsetTransformOnWeaponThrow) {
					teleportPosition = currentMeleeAttackSurfaceInfo.offsetTransformOnWeaponThrow.position;
				}

				if (currentMeleeAttackSurfaceInfo.useOffsetDistanceOnWeaponThrow) {
					teleportDistanceOffset = currentMeleeAttackSurfaceInfo.offsetDistanceOnWeaponThrow;
				}
			}
		}

		if (teleportPosition == Vector3.zero) {
			teleportPosition = currentGrabbedObjectTransform.position;
		}

		mainPlayerTeleportSystem.teleportPlayer (playerControllerGameObject.transform, teleportPosition, false, true, 
			cameraFovOnTeleport, cameraFovOnTeleportSpeed, teleportSpeed, true, teleportInstantlyToPosition, true,
			useSmoothCameraFollowStateOnTeleport, smoothCameraFollowDuration, teleportDistanceOffset);

		teleportCoroutine = StartCoroutine (activateTeleportCoroutine ());

		teleportDistanceOffset = 0;
	}

	public void stopActivateTeleportCoroutine ()
	{
		if (teleportCoroutine != null) {
			StopCoroutine (teleportCoroutine);
		}

		teleportInProcess = false;
	}

	IEnumerator activateTeleportCoroutine ()
	{
		eventOnStartTeleport.Invoke ();

		teleportInProcess = true;

		bool targetReached = false;

		float positionDifference = 0;

		float lastTimeDistanceChecked = 0;

		Vector3 targetPosition = teleportPosition;

		teleportPosition = Vector3.zero;

		if (targetPosition == Vector3.zero) {
			targetPosition = currentGrabbedObjectTransform.position;
		}

		while (!targetReached) {

			if (!mainPlayerTeleportSystem.isTeleportInProcess ()) {
				targetReached = true;
			}

			positionDifference = GKC_Utils.distance (targetPosition, playerControllerGameObject.transform.position);

			if (lastTimeDistanceChecked == 0) {
				if (positionDifference < minDistanceToStopTeleport + 1) {
					lastTimeDistanceChecked = Time.time;
				}
			} else {
				if (Time.time > lastTimeDistanceChecked + 0.5f) {
					if (showDebugPrint) {
						print ("too much time without moving");
					}

					targetReached = true;
				}
			}

			if (positionDifference < minDistanceToStopTeleport) {
				targetReached = true;
			}

			if (targetReached) {
				mainPlayerTeleportSystem.resumeIfTeleportActive ();
			}

			yield return null;
		}

		teleportInProcess = false;

		if (grabMeleeWeaponOnTeleportPositionReached) {
			inputThrowOrReturnObject ();
		}

		eventOnEndTeleport.Invoke ();
	}

	public void setThrowObjectEnabledState (bool state)
	{
		if (showDebugPrint) {
			print (state);
		}

		throwObjectEnabled = state;
	}

	public void setReturnObjectEnabledState (bool state)
	{
		returnObjectEnabled = state;
	}

	public void setApplyDamageOnSurfaceDetectedOnReturnEnabledState (bool state)
	{
		applyDamageOnSurfaceDetectedOnReturnEnabled = state;
	}

	public void setApplyDamageOnObjectReturnPathEnabledState (bool state)
	{
		applyDamageOnObjectReturnPathEnabled = state;
	}

	public void setGeneralDamageMultiplierOnObjectReturnValue (float newValue)
	{
		generalDamageMultiplierOnObjectReturn = newValue;
	}

	public void setGeneralDamageMultiplierOnReturnPathValue (float newValue)
	{
		generalDamageMultiplierOnReturnPath = newValue;
	}

	public void setGeneralDamageOnSurfaceDetectedOnThrowValue (float newValue)
	{
		generalDamageOnSurfaceDetectedOnThrow = newValue;
	}

	public void setObjectThrownTravellingToTargetState (bool state)
	{
		objectThrownTravellingToTarget = state;
	}

	public bool isObjectThrownTravellingToTarget ()
	{
		return objectThrownTravellingToTarget;
	}

	public bool isContinueObjecThrowActivated ()
	{
		return continueObjecThrowActivated;
	}

	public float getLastTimeObjectThrown ()
	{
		return lastTimeObjectThrown;
	}

	public bool isCurrentWeaponThrown ()
	{
		return objectThrown;
	}

	public void setThrowObjectInputPausedForStaminaState (bool state)
	{
		throwObjectInputPausedForStamina = state;
	}

	public void stopThrowStateOnRemoveMeleeWeapon ()
	{
		if (objectThrown) {
			stopThrowObjectCoroutine ();

			stopReturnObjectCoroutine ();

			eventOnDropObjectWhenIsThrown.Invoke ();
		}

		setObjectThrownState (false);

		mainPlayerController.setPlayerMeleeWeaponThrownState (false);

		returningThrownObject = false;

		setObjectThrownTravellingToTargetState (false);
	}

	public void setSurfacecNotFoundState (bool state)
	{
		surfaceNotFound = state;
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo && Application.isPlaying && objectThrown) {
			GKC_Utils.drawCapsuleGizmo (point1, point2, capsuleCastRadius, sphereColor, cubeColor, currentRayTargetPosition, rayDirection, distanceToTarget);
		}
	}
}
