using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class grapplingHookSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool grapplingHookEnabled = true;

	public float maxRaycastDistance = 100;
	public LayerMask layerToCheckSurfaces;

	public float minDistanceToAttract = 0.5f;

	public bool applySpeedOnHookStop;
	public float extraSpeedOnHookStopMultiplier = 1;

	public bool rotatePlayerTowardTargetDirection;
	public float rotatePlayerSpeed;
	public float minAngleDifferenceToRotatePlayer;

	public float extraForceOnHookStopMultiplier = 4;

	public float maxForceToApplyOnHookStop = 40;

	[Space]
	[Header ("Swing Settings")]
	[Space]

	public bool useSwingForcesActive;

	public float AirAccelCoeff = 1f;

	public float AirDecelCoeff = 1.5f;

	public float gravityForceDown = 9.8f;

	public float MaxSpeedAlongOneDimension = 8;

	public float HookPullForce = 1f;

	public float DampingCoeff = 0.01f;

	public float SideAccelHold = 0.1f;

	public float SideAccelPull = 0.8f;

	public float AirControlPrecision = 16;
	public float AirControlAdditionForward = 8;

	public float durationAutoPullOnThrowHook = 1;

	public float minDistanceToHookPointOnSwing = 5;

	public float radiusToCheckCollisionsOnSwing = 2.5f;

	public bool useMaxSwingLength;
	public float maxSwingLength;

	public float raycastDistanceToCheckBelowPlayer = 5;

	public bool jumpOnSwingEnabled = true;
	public Vector3 jumpOnSwingForceAmount;

	public bool checkIfTooCloseToGroundToStopSwing;
	public float minDistanceToStopSwingOnGroundDetected;
	public bool onlyCheckIfCloseToGroundWhenAutomaticPullNotActive;

	[Space]
	[Header ("Attract Objects Settings")]
	[Space]

	public bool attractObjectsEnabled = true;
	public float regularAttractionForce;
	public float increasedAttractionForce;
	public float minDistanceToStopAttractObject;

	public bool addUpForceForAttraction;
	public float upForceForAttraction;
	public float addUpForceForAttractionDuration;

	[Space]
	[Header ("Movement Settings")]
	[Space]

	public float regularMovementSpeed = 6;
	public float increasedMovementSpeed;

	public float inputMovementMultiplier = 3;

	public float airControlAmount = 20;

	public bool useVerticalMovementOnHook;
	public bool ignoreBackWardsMovementOnHook;

	public bool addVerticalFallingSpeed;
	public float verticalFallingSpeed;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool checkIfObjectStuck = true;
	public float timeToStopHookIfStuck = 2;
	public float minDistanceToCheckStuck = 1;

	[Space]
	[Header ("External Controller Settings")]
	[Space]

	public bool checkExternalControllerStatesToIgnore;

	public List<otherExternalControllerInfo> otherExternalControllerInfoList = new List<otherExternalControllerInfo> ();

	[Space]
	[Header ("Action System Settings")]
	[Space]

	public int customActionCategoryID = -1;
	public int regularActionCategoryID = -1;

	[Space]
	[Header ("Camera Settings")]
	[Space]

	public bool changeCameraStateOnThirdPerson;
	public string cameraStateNameOnGrapplingHookActivate;
	public string cameraStateNameOnGrapplingHookDeactivate;
	public bool keepCameraStateActiveWhileOnAir;

	public bool changeFovOnHookActive;
	public float changeFovSpeed;
	public float regulaFov;
	public float increaseSpeedFov;

	public bool useCameraShake;
	public string regularCameraShakeName;
	public string increaseCamaraShakeName;

	[Space]
	[Header ("Animator Settings")]
	[Space]

	public bool setAnimatorState;
	public string hookStartActionName;
	public string hookEndActionName;

	public int hookStartActionID;
	public int hookEndActionID;

	public string actionActiveAnimatorName = "Action Active";
	public string actionIDAnimatorName = "Action ID";

	public float minDistancePercentageToUseHookEndAction = 0.1f;

	[Space]
	[Header ("Throw Hook Animation Settings")]
	[Space]

	public bool useAnimationToThrowHook;
	public string throwHookAnimationName;
	public float throwHookAnimationDuration;

	public bool throwAnimationInProcess;

	public bool checkIfSurfaceDetectedBeforeAnimation;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool grapplingHookActive;
	public bool grapplingHookUpdateActive;

	public Vector3 currentForceToApply;
	public Vector3 movementDirection;

	public float currentDistance;

	public float currentMovementSpeed;

	public Transform currentGrapplingHookTarget;

	public float angleToTargetDirection;

	public bool attractingObjectActive;

	public bool checkingToRemoveHookActive;

	public bool manualPullActive;

	public bool automaticPullActive;

	public bool pullFromRaycastDetectedActive;

	public float currentMinDistanceToStopAttractObject;

	public bool attractPlayerActive;

	public bool ignoreApplySpeedOnHookStopOnPlayer;

	public bool resetPlayerSpeedOnHookStop;

	[Space]
	[Header ("Debug Swing")]
	[Space]

	public Vector3 swingGravityForces = Vector3.zero;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnGrapplingHook;
	public UnityEvent eventOnGrapplingHookActivate;
	public UnityEvent eventOnGrapplingHookDeactivate;
	public eventParameters.eventToCallWithVector3 eventWithDirectionOnHookActive;

	public bool useEventsOnChangeCameraView;
	public UnityEvent eventOnSetFirstPersonView;
	public UnityEvent eventOnSetThirdPersonView;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform grapplingHookTipTransform;

	public Transform playerControllerTransform;
	public playerController mainPlayerController;
	public gravitySystem mainGravitySystem;
	public Transform mainCameraTransform;
	public Rigidbody mainRigidbody;
	public playerCamera mainPlayerCamera;

	public remoteEventSystem mainRemoteEventSystem;

	public Animator mainAnimator;


	RaycastHit hit;

	bool increaseSpeedActive;

	bool checkinGrapplingHookCameraStateAfterDeactivate;

	Vector3 pullForceToApply;

	Vector3 pullForceToApplyNormalize;

	int actionActiveAnimatorID;
	int actionIDAnimatorID;

	bool closeToReachTargetChecked;

	float initialDistanceToTarget;

	bool firstPersonActive;
	bool previoslyFirstPersonActive;

	objectToAttractWithGrapplingHook currentobjectToAttractWithGrapplingHook;

	GameObject currentObjectToAttract;
	Rigidbody currentRigidbodyToAttract;

	float lastTimeHookActive;

	float customMinDistanceToStopAttractObject;

	bool useCustomForceAttractionValues;
	bool customAddUpForceForAttraction;
	float customUpForceForAttraction;
	float customAddUpForceForAttractionDuration;

	bool attractionHookRemovedByDistance;

	Vector3 currentRaycastPosition;
	Vector3 currentRaycastDirection;

	Coroutine grapplingHookCoroutine;

	float lastTimeObjectMoving;
	float lastDistanceToObject;

	Coroutine animationCoroutine;

	Vector3 lastVelocityAdded;

	float lastTimeAutomaticPullActive;

	float lastTimePullFromRaycastDetectedActive;

	bool swingRaycastDetected;

	RaycastHit swingRacyastCheck;

	Collider mainCollider;

	bool mainColliderAssigned;

	readonly Collider[] _overlappingColliders = new Collider[10];

	bool useHookTargetPostionOffset;
	Vector3 hookTargetPositionOffset = Vector3.zero;

	bool ignoreApplySpeedOnHookStopTemporally;


	void Start ()
	{
		actionActiveAnimatorID = Animator.StringToHash (actionActiveAnimatorName);
		actionIDAnimatorID = Animator.StringToHash (actionIDAnimatorName);
	}

	public void stopGrapplingHookCoroutine ()
	{
		if (grapplingHookCoroutine != null) {
			StopCoroutine (grapplingHookCoroutine);
		}

		grapplingHookUpdateActive = false;
	}

	IEnumerator activateGrapplingHookCorouine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			updateGrapplingHookState ();
		}
	}

	void updateGrapplingHookState ()
	{
		if (attractingObjectActive) {
			applyAttractionForces ();
		} else {
			if (grapplingHookActive) {
				applyHookForces ();

				if (useEventsOnChangeCameraView) {
					firstPersonActive = mainPlayerCamera.isFirstPersonActive ();

					if (firstPersonActive != previoslyFirstPersonActive) {
						previoslyFirstPersonActive = firstPersonActive;

						if (firstPersonActive) {
							eventOnSetFirstPersonView.Invoke ();
						} else {
							eventOnSetThirdPersonView.Invoke ();
						}
					}
				}
			}

			if (checkinGrapplingHookCameraStateAfterDeactivate) {
				if (mainPlayerController.isPlayerOnGround ()) {
					if (!mainPlayerCamera.isFirstPersonActive () && mainPlayerCamera.isCameraTypeFree ()) {
						mainPlayerCamera.setCameraState (cameraStateNameOnGrapplingHookDeactivate);
					}

					checkinGrapplingHookCameraStateAfterDeactivate = false;

					stopGrapplingHookCoroutine ();
				}
			}
		}
	}

	public Vector3 TransformDirectionHorizontal (Transform t, Vector3 v)
	{
		return ToHorizontal (t.TransformDirection (v)).normalized;
	}

	public Vector3 ToHorizontal (Vector3 v)
	{
		return Vector3.ProjectOnPlane (v, Vector3.up);
	}

	public Vector3 InverseTransformDirectionHorizontal (Transform t, Vector3 v)
	{
		return ToHorizontal (t.InverseTransformDirection (v)).normalized;
	}

	public Vector3 WithX (Vector3 v, float x)
	{
		return new Vector3 (x, v.y, v.z);
	}

	public Vector3 WithY (Vector3 v, float y)
	{
		return new Vector3 (v.x, y, v.z);
	}

	public Vector3 WithZ (Vector3 v, float z)
	{
		return new Vector3 (v.x, v.y, z);
	}



	void updateSwingForces (Vector3 playerPosition, Vector3 targetPosition)
	{
		Vector2 rawAxisValues = mainPlayerController.getRawAxisValues ();

		movementDirection = new Vector3 (rawAxisValues.x, 0, rawAxisValues.y).normalized;

		if (increaseSpeedActive) {
			currentMovementSpeed = increasedMovementSpeed;
		} else {
			currentMovementSpeed = regularMovementSpeed;
		}

		pullForceToApplyNormalize = pullForceToApply.normalized;

		currentForceToApply += movementDirection * currentMovementSpeed * inputMovementMultiplier;

		//---------------------------------------------------------------------
		if (lastTimeAutomaticPullActive > 0) {
			if (Time.time > lastTimeAutomaticPullActive + 0.5f) {
				lastTimeAutomaticPullActive = 0;
			}
		} else {
			automaticPullActive = false;
		}

		if (Physics.Raycast (playerPosition, -playerControllerTransform.up, out hit, raycastDistanceToCheckBelowPlayer, layerToCheckSurfaces)) {
			automaticPullActive = true;

			lastTimeAutomaticPullActive = Time.time;
		} 

		if (durationAutoPullOnThrowHook > 0) {
			if (Time.time < durationAutoPullOnThrowHook + lastTimeHookActive) {
				automaticPullActive = true;
			}
		}

		if (useMaxSwingLength) {
			if (currentDistance > maxSwingLength) {
				automaticPullActive = true;
			}
		}

		float deltaTime = Time.fixedDeltaTime;

		Vector3 wishDir = TransformDirectionHorizontal (mainPlayerCamera.transform, movementDirection); // We want to go in this direction

		// If the input doesn't have the same facing with the current velocity then slow down instead of speeding up
		float coeff = Vector3.Dot (swingGravityForces, wishDir) > 0 ? AirAccelCoeff : AirDecelCoeff;

		//ACCELERATION
		// How much speed we already have in the direction we want to speed up
		var projSpeed = Vector3.Dot (swingGravityForces, wishDir);
		// How much speed we need to add (in that direction) to reach max speed
		var addSpeed = MaxSpeedAlongOneDimension - projSpeed;

		if (addSpeed > 0) {
			// How much we are gonna increase our speed  maxSpeed * dt => the real deal. a = v / t accelCoeff => ad hoc approach to make it feel better
			var accelAmount = coeff * MaxSpeedAlongOneDimension * deltaTime;

			// If we are accelerating more than in a way that we exceed maxSpeedInOneDimension, crop it to max
			if (accelAmount > addSpeed) {
				accelAmount = addSpeed;
			}

			swingGravityForces += wishDir * accelAmount; // Magic happens here
		}

		if (Mathf.Abs (movementDirection.z) > 0.0001) { // Pure side velocity doesn't allow air control
			// This only happens in the horizontal plane
			var playerDirHorz = WithY (swingGravityForces, 0).normalized;
			var playerSpeedHorz = WithY (swingGravityForces, 0).magnitude;

			var dot = Vector3.Dot (playerDirHorz, wishDir);
			if (dot > 0) {
				var k = AirControlPrecision * dot * dot * deltaTime;

				// CPMA thingy: If we want pure forward movement, we have much more air Of course this only happens when we're not hooked
				//					var accelDirLocal = InverseTransformDirectionHorizontal (mainCameraTransform, wishDir);
				//					var isPureForward = Mathf.Abs (accelDirLocal.x) < 0.0001 && Mathf.Abs (accelDirLocal.z) > 0;

				// A little bit closer to accelDir
				playerDirHorz = playerDirHorz * playerSpeedHorz + wishDir * k;
				playerDirHorz.Normalize ();

				// Assign new direction, without touching the vertical speed
				swingGravityForces = WithY ((playerDirHorz * playerSpeedHorz), swingGravityForces.y);
			}
		}

		swingGravityForces += -mainPlayerController.getCurrentNormal () * gravityForceDown;

		// Side acceleration On A-D input, we accelerate in that direction Way stronger when pulling
		coeff = (manualPullActive || automaticPullActive || pullFromRaycastDetectedActive) ? SideAccelPull : SideAccelHold;

		var sideAccel = movementDirection.x * coeff;
		swingGravityForces += sideAccel * playerControllerTransform.right;

		if (manualPullActive || automaticPullActive || pullFromRaycastDetectedActive) {
			var springDir = (targetPosition - playerControllerTransform.position).normalized;
			var damping = swingGravityForces * DampingCoeff;

			swingGravityForces += HookPullForce * springDir * deltaTime;
			swingGravityForces -= damping;
		}

		var displacement = swingGravityForces * deltaTime;

		if (maxSwingSpeedClamp != 0) {
			displacement = Vector3.ClampMagnitude (displacement, maxSwingSpeedClamp);
		}

		mainRigidbody.MovePosition (mainRigidbody.position + displacement);


		var collisionDisplacement = ResolveCollisions (ref swingGravityForces);

		//			if (!manualPullActive) {

		Vector3 currentPlayerPosition = playerControllerTransform.position - Vector3.up * 0.4f;

		var distance = Vector3.Distance (currentPlayerPosition, targetPosition);

		if (distance > initialDistanceToTarget) {
			// The player will have no velocity component in the hook's direction
			var playerToEndDir = (targetPosition - currentPlayerPosition).normalized;
			swingGravityForces -= Vector3.Project (swingGravityForces, playerToEndDir);

			collisionDisplacement = playerToEndDir * (distance - initialDistanceToTarget);
		}
		//			}

		mainRigidbody.MovePosition (mainRigidbody.position + collisionDisplacement);

		lastVelocityAdded = swingGravityForces;

		if (automaticPullActive || manualPullActive) {
			initialDistanceToTarget = GKC_Utils.distance (playerPosition, targetPosition);
		}

		if (currentDistance < minDistanceToHookPointOnSwing) {
			removeHook ();
		}


		if (checkIfTooCloseToGroundToStopSwing && (!automaticPullActive || !onlyCheckIfCloseToGroundWhenAutomaticPullNotActive)) {
			Vector3 raycastPosition = playerControllerTransform.position + playerControllerTransform.up;

			if (Physics.Raycast (raycastPosition, -playerControllerTransform.up, out hit, minDistanceToStopSwingOnGroundDetected, layerToCheckSurfaces)) {
				removeHook ();
			}
		}

		if (lastTimePullFromRaycastDetectedActive > 0) {
			if (Time.time > lastTimePullFromRaycastDetectedActive + 0.5f) {
				lastTimePullFromRaycastDetectedActive = 0;
			}
		} else {
			pullFromRaycastDetectedActive = false;
		}
	}

	public float maxSwingSpeedClamp = 40;

	public void applyHookForces ()
	{
		currentForceToApply = Vector3.zero;

		Vector3 playerPosition = playerControllerTransform.position + playerControllerTransform.up;

		Vector3 targetPosition = grapplingHookTipTransform.position;

		if (useHookTargetPostionOffset) {
			targetPosition += hookTargetPositionOffset;
		}

		currentDistance = GKC_Utils.distance (playerPosition, targetPosition);

		if (useSwingForcesActive) {
			updateSwingForces (playerPosition, targetPosition);
		} else {
			if (attractPlayerActive) {
				if (useCustomForceAttractionValues) {
					currentMinDistanceToStopAttractObject = customMinDistanceToStopAttractObject;
				} else {
					currentMinDistanceToStopAttractObject = minDistanceToAttract;
				}
			} else {
				currentMinDistanceToStopAttractObject = minDistanceToAttract;
			}

			if (currentDistance > currentMinDistanceToStopAttractObject) {
				pullForceToApply = (targetPosition - playerPosition).normalized;
			} else {
				removeHook ();
			}

			if (increaseSpeedActive) {
				currentMovementSpeed = increasedMovementSpeed;
			} else {
				currentMovementSpeed = regularMovementSpeed;
			}

			currentForceToApply += pullForceToApply * currentMovementSpeed;

			pullForceToApplyNormalize = pullForceToApply.normalized;

			movementDirection = 
				(mainPlayerController.getHorizontalInput () * Vector3.Cross (playerControllerTransform.up, pullForceToApplyNormalize));

			if (useVerticalMovementOnHook) {
				movementDirection += (mainPlayerController.getVerticalInput () * playerControllerTransform.up);
			} else {
				float verticalInputValue = mainPlayerController.getVerticalInput ();

				if (ignoreBackWardsMovementOnHook) {
					verticalInputValue = Mathf.Clamp (verticalInputValue, 0, 1);
				}

				movementDirection += (verticalInputValue * pullForceToApplyNormalize);
			}

			movementDirection *= inputMovementMultiplier;

			currentForceToApply += movementDirection;

			if (addVerticalFallingSpeed) {
				currentForceToApply -= playerControllerTransform.up * verticalFallingSpeed;
			}

			mainPlayerController.setExternalForceOnAir (currentForceToApply, airControlAmount);

			lastVelocityAdded = currentForceToApply;
		}

		if (rotatePlayerTowardTargetDirection) {
			pullForceToApplyNormalize -= playerControllerTransform.up * playerControllerTransform.InverseTransformDirection (pullForceToApplyNormalize).y;

			angleToTargetDirection = Vector3.SignedAngle (playerControllerTransform.forward, pullForceToApplyNormalize, playerControllerTransform.up);

			if (Mathf.Abs (angleToTargetDirection) > minAngleDifferenceToRotatePlayer) {
				playerControllerTransform.Rotate (0, (angleToTargetDirection / 2) * rotatePlayerSpeed * Time.deltaTime, 0);
			}
		}

		if (!closeToReachTargetChecked) {
			if (currentDistance < initialDistanceToTarget * minDistancePercentageToUseHookEndAction) {

				if (setAnimatorState) {
					mainAnimator.SetInteger (actionIDAnimatorID, hookEndActionID);
					mainAnimator.CrossFadeInFixedTime (hookEndActionName, 0.1f);
				}

				closeToReachTargetChecked = true;
			}
		}

		if (!useSwingForcesActive) {
			checkIfObjectIsMoving ();
		}
	}

	public void applyAttractionForces ()
	{
		currentForceToApply = Vector3.zero;

		Vector3 playerPosition = playerControllerTransform.position + playerControllerTransform.up;

		currentDistance = GKC_Utils.distance (playerPosition, grapplingHookTipTransform.position);

		if (useCustomForceAttractionValues) {
			currentMinDistanceToStopAttractObject = customMinDistanceToStopAttractObject;
		} else {
			currentMinDistanceToStopAttractObject = minDistanceToStopAttractObject;
		}

		if (currentDistance > currentMinDistanceToStopAttractObject) {
			pullForceToApply = (playerPosition - grapplingHookTipTransform.position).normalized;
		} else {
			attractionHookRemovedByDistance = true;

			removeHook ();

			return;
		}

		if (increaseSpeedActive) {
			currentMovementSpeed = increasedAttractionForce;
		} else {
			currentMovementSpeed = regularAttractionForce;
		}

		currentForceToApply += pullForceToApply * currentMovementSpeed;

		movementDirection *= inputMovementMultiplier;

		currentForceToApply += movementDirection;

		if (useCustomForceAttractionValues) {
			if (customAddUpForceForAttraction) {
				if (Time.time < lastTimeHookActive + customAddUpForceForAttractionDuration) {
					currentForceToApply += playerControllerTransform.up * customUpForceForAttraction;
				}
			}
		} else {
			if (addUpForceForAttraction) {
				if (Time.time < lastTimeHookActive + addUpForceForAttractionDuration) {
					currentForceToApply += playerControllerTransform.up * upForceForAttraction;
				}
			}
		}
	
		if (currentRigidbodyToAttract != null) {
			currentRigidbodyToAttract.velocity = currentForceToApply;

			lastVelocityAdded = currentForceToApply;
		}

		checkIfObjectIsMoving ();
	}

	public void checkIfObjectIsMoving ()
	{
		if (checkIfObjectStuck) {
			if (lastTimeObjectMoving == 0) {
				Vector3 targetPosition = grapplingHookTipTransform.position;

				if (useHookTargetPostionOffset) {
					targetPosition += hookTargetPositionOffset;
				}

				currentDistance = GKC_Utils.distance (playerControllerTransform.position, targetPosition);

				lastDistanceToObject = currentDistance;

				lastTimeObjectMoving = Time.time;
			}

			if (Time.time > lastTimeObjectMoving + timeToStopHookIfStuck) {
				lastTimeObjectMoving = Time.time;

				if ((currentDistance + minDistanceToCheckStuck) >= lastDistanceToObject) {
					print ("position hasn't changed in " + timeToStopHookIfStuck + " time, stop hook");

					removeHook ();
				}

				lastDistanceToObject = currentDistance;
			}
		}
	}

	public void removeHook ()
	{
		if (grapplingHookActive) {
		
			grapplingHookActive = false;

			setCurrentPlayerActionSystemCustomActionCategoryID (false);
		
			if (attractingObjectActive) {
				checkEventsOnGrapplingHook (false);

				stopGrapplingHookCoroutine ();
			} else {
				pauseOrResumePlayerState (false);
			}

			increaseSpeedActive = false;

			grapplingHookTipTransform.SetParent (transform);

			if (attractObjectsEnabled) {
				if (currentobjectToAttractWithGrapplingHook != null) {
					if (currentobjectToAttractWithGrapplingHook.useRemoteEventsOnStateChange) {
						if (mainRemoteEventSystem != null) {
							for (int i = 0; i < currentobjectToAttractWithGrapplingHook.remoteEventNameListOnEnd.Count; i++) {
								mainRemoteEventSystem.callRemoteEvent (currentobjectToAttractWithGrapplingHook.remoteEventNameListOnEnd [i]);
							}
						}
					}

					if (attractPlayerActive) {

					} else {
						currentobjectToAttractWithGrapplingHook.setAttractionHookRemovedByDistanceState (attractionHookRemovedByDistance);

						currentobjectToAttractWithGrapplingHook.setAttractObjectState (false);


						bool canUseExtraInteraction = currentobjectToAttractWithGrapplingHook.autoGrabObjectOnCloseDistance ||
						                              currentobjectToAttractWithGrapplingHook.activateInteractionActionWithObject;

						if (canUseExtraInteraction) {
							if (currentRigidbodyToAttract != null) {
								Vector3 playerPosition = playerControllerTransform.position + playerControllerTransform.up;

								currentDistance = GKC_Utils.distance (playerPosition, currentRigidbodyToAttract.transform.position);

								if (currentobjectToAttractWithGrapplingHook.autoGrabObjectOnCloseDistance) {
									if (currentDistance < currentobjectToAttractWithGrapplingHook.minDistanceToAutoGrab) {
										GKC_Utils.grabPhysicalObjectExternally (playerControllerTransform.gameObject, currentRigidbodyToAttract.gameObject);
									}
								}
							
								if (currentobjectToAttractWithGrapplingHook.activateInteractionActionWithObject) {
									if (currentDistance < currentobjectToAttractWithGrapplingHook.minDistanceToActivateInteractionActionWithObject) {
										GKC_Utils.useObjectExternally (playerControllerTransform.gameObject, currentobjectToAttractWithGrapplingHook.objectToActivate);
									}
								}
							}
						}

						attractingObjectActive = false;

						currentobjectToAttractWithGrapplingHook = null;

						currentObjectToAttract = null;

						currentRigidbodyToAttract = null;

						attractionHookRemovedByDistance = false;


					}
				}
			}
		}
	}

	public void setGrapplingHookTarget (Transform newTarget)
	{
		currentGrapplingHookTarget = newTarget;
	}

	public void setGrapplingHookEnabledState (bool state)
	{
		grapplingHookEnabled = state;

		if (!grapplingHookEnabled) {
			removeGrapplingHook ();
		}
	}

	void calculateRaycastValues ()
	{
		currentRaycastPosition = mainCameraTransform.position;
		currentRaycastDirection = mainCameraTransform.forward;

		if (!mainPlayerCamera.isCameraTypeFree () && !mainPlayerCamera.isPlayerAiming ()) {
			currentRaycastPosition = playerControllerTransform.position + playerControllerTransform.up * 1.3f;
			currentRaycastDirection = playerControllerTransform.forward;
		}

		if (currentGrapplingHookTarget != null) {
			currentRaycastDirection = currentGrapplingHookTarget.position - currentRaycastPosition;
			currentRaycastDirection.Normalize ();
		}
	}

	public void checkThrowGrapplingHook ()
	{
		if (grapplingHookEnabled) {
			if (!grapplingHookActive) {

				bool surfaceDetected = false;

				if (useSwingForcesActive) {
					if (swingRaycastDetected) {
						hit = swingRacyastCheck;

						surfaceDetected = true;
					}
				} else {
					calculateRaycastValues ();

					if (Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, maxRaycastDistance, layerToCheckSurfaces)) {
						surfaceDetected = true;
					}
				}

				if (surfaceDetected) {
					grapplingHookTipTransform.position = hit.point;

					grapplingHookTipTransform.SetParent (hit.collider.gameObject.transform);

					grapplingHookActive = true;

					stopGrapplingHookCoroutine ();

					grapplingHookCoroutine = StartCoroutine (activateGrapplingHookCorouine ());

					grapplingHookUpdateActive = true;

					lastTimeHookActive = Time.time;

					lastTimeObjectMoving = 0;

					ignoreApplySpeedOnHookStopOnPlayer = false;

					resetPlayerSpeedOnHookStop = false;

					if (attractObjectsEnabled) {
						currentObjectToAttract = hit.collider.gameObject;

						GameObject currentVehicle = applyDamage.getVehicle (currentObjectToAttract);

						if (currentVehicle != null) {
							currentObjectToAttract = currentVehicle;
						}

						currentobjectToAttractWithGrapplingHook = currentObjectToAttract.GetComponent<objectToAttractWithGrapplingHook> ();

						if (currentobjectToAttractWithGrapplingHook == null) {
							playerComponentsManager currentPlayerComponentsManager = currentObjectToAttract.GetComponent<playerComponentsManager> ();

							if (currentPlayerComponentsManager != null) {
								currentobjectToAttractWithGrapplingHook = currentPlayerComponentsManager.getObjectToAttractWithGrapplingHook ();
							}
						}

						attractPlayerActive = false;


						customMinDistanceToStopAttractObject = 0;

						useCustomForceAttractionValues = false;

						customAddUpForceForAttraction = false;

						customUpForceForAttraction = 0;

						customAddUpForceForAttractionDuration = 0;

						useHookTargetPostionOffset = false;

						hookTargetPositionOffset = Vector3.zero;

						if (currentobjectToAttractWithGrapplingHook != null) {
							attractPlayerActive = currentobjectToAttractWithGrapplingHook.attractPlayerEnabled;

							ignoreApplySpeedOnHookStopOnPlayer = currentobjectToAttractWithGrapplingHook.ignoreApplySpeedOnHookStopOnPlayer;

							resetPlayerSpeedOnHookStop = currentobjectToAttractWithGrapplingHook.resetPlayerSpeedOnHookStop;

							useHookTargetPostionOffset = currentobjectToAttractWithGrapplingHook.useHookTargetPostionOffset;
							hookTargetPositionOffset = currentobjectToAttractWithGrapplingHook.hookTargetPositionOffset;

							if (attractPlayerActive) {

							} else {
								attractingObjectActive = currentobjectToAttractWithGrapplingHook.setAttractObjectState (true);

								if (attractingObjectActive) {
									currentRigidbodyToAttract = currentobjectToAttractWithGrapplingHook.getRigidbodyToAttract ();

									if (currentRigidbodyToAttract == null) {
										print ("WARNING: No rigidbody has been configured in the object " + currentobjectToAttractWithGrapplingHook.name);
								
										removeGrapplingHook ();

										return;
									}

									grapplingHookTipTransform.SetParent (currentRigidbodyToAttract.transform);

									grapplingHookTipTransform.position = currentRigidbodyToAttract.position;
								}
							}

							customMinDistanceToStopAttractObject = currentobjectToAttractWithGrapplingHook.customMinDistanceToStopAttractObject;

							useCustomForceAttractionValues = currentobjectToAttractWithGrapplingHook.useCustomForceAttractionValues;

							customAddUpForceForAttraction = currentobjectToAttractWithGrapplingHook.customAddUpForceForAttraction;

							customUpForceForAttraction = currentobjectToAttractWithGrapplingHook.customUpForceForAttraction;

							customAddUpForceForAttractionDuration = currentobjectToAttractWithGrapplingHook.customAddUpForceForAttractionDuration;

							if (currentobjectToAttractWithGrapplingHook.useRemoteEventsOnStateChange) {
								if (mainRemoteEventSystem != null) {
									for (int i = 0; i < currentobjectToAttractWithGrapplingHook.remoteEventNameListOnStart.Count; i++) {
										mainRemoteEventSystem.callRemoteEvent (currentobjectToAttractWithGrapplingHook.remoteEventNameListOnStart [i]);
									}
								}
							}
						}
					}

					if (attractingObjectActive) {
						checkEventsOnGrapplingHook (true);
					} else {
						pauseOrResumePlayerState (true);
					}

					checkingToRemoveHookActive = false;

					eventWithDirectionOnHookActive.Invoke (currentRaycastDirection);

					closeToReachTargetChecked = false;

					Vector3 playerPosition = playerControllerTransform.position + playerControllerTransform.up;

					Vector3 targetPosition = grapplingHookTipTransform.position;

					if (useHookTargetPostionOffset) {
						targetPosition += hookTargetPositionOffset;
					}
						
					initialDistanceToTarget = GKC_Utils.distance (playerPosition, targetPosition);

//					print (initialDistanceToTarget);

					swingGravityForces = Vector3.zero;

					setCurrentPlayerActionSystemCustomActionCategoryID (true);
				}
			}
		}
	}

	public void removeGrapplingHook ()
	{
		if (grapplingHookActive) {
			if (checkingToRemoveHookActive) {
				removeHook ();
			}

			checkingToRemoveHookActive = true;
		}
	}

	public void checkRemoveGrapplingHook ()
	{
		if (grapplingHookEnabled) {
			removeGrapplingHook ();
		}
	}

	public void checkEventsOnGrapplingHook (bool state)
	{
		if (useEventsOnGrapplingHook) {
			if (state) {
				eventOnGrapplingHookActivate.Invoke ();
			} else {
				eventOnGrapplingHookDeactivate.Invoke ();
			}
		}
	}

	public void checkExternalControllerBehaviorToPause (string behaviorName)
	{
		if (grapplingHookActive) {
			if (checkExternalControllerStatesToIgnore) {
				externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (currentExternalControllerBehavior != null && behaviorName.Equals (currentExternalControllerBehavior.behaviorName)) {
					int currentIndex = otherExternalControllerInfoList.FindIndex (s => s.Name.Equals (behaviorName));

					if (currentIndex > -1) {
						currentExternalControllerBehavior.checkPauseStateDuringExternalForceOrBehavior ();

						if (otherExternalControllerInfoList [currentIndex].useEventsOnExternalController) {
							otherExternalControllerInfoList [currentIndex].eventToPauseExternalController.Invoke ();
						}
					}
				}
			}
		}
	}

	public void pauseOrResumePlayerState (bool state)
	{
		if (state) {
			if (mainPlayerController.isExternalControllBehaviorActive ()) {
				externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (currentExternalControllerBehavior != null) {
					bool canDisableExternalControllerState = true;
						
					if (checkExternalControllerStatesToIgnore) {
						
						int currentIndex = otherExternalControllerInfoList.FindIndex (s => s.Name.Equals (currentExternalControllerBehavior.behaviorName));

						if (currentIndex > -1) {
							currentExternalControllerBehavior.checkPauseStateDuringExternalForceOrBehavior ();

							if (otherExternalControllerInfoList [currentIndex].useEventsOnExternalController) {
								otherExternalControllerInfoList [currentIndex].eventToPauseExternalController.Invoke ();
							}

							canDisableExternalControllerState = false;
						}
					}

					if (canDisableExternalControllerState) {
						currentExternalControllerBehavior.disableExternalControllerState ();
					}
				}
			}

			mainPlayerController.setGravityForcePuase (true);

			mainPlayerController.setCheckOnGroungPausedState (true);

			mainPlayerController.setPlayerOnGroundState (false);

			mainGravitySystem.setCameraShakeCanBeUsedState (false);

			mainPlayerController.setJumpLegExternallyActiveState (true);

			mainPlayerController.setIgnoreExternalActionsActiveState (true);

			mainPlayerController.setIKBodyPausedState (true);
		} else {
			mainPlayerController.setGravityForcePuase (false);

			mainPlayerController.setCheckOnGroungPausedState (false);

			mainGravitySystem.setCameraShakeCanBeUsedState (true);

			mainPlayerController.setJumpLegExternallyActiveState (false);

			mainPlayerController.setLastTimeFalling ();

			mainPlayerController.setIgnoreExternalActionsActiveState (false);

			mainPlayerController.setIKBodyPausedState (false);
		}

		checkEventsOnGrapplingHook (state);

		bool stopCoroutine = false;

		if (changeCameraStateOnThirdPerson) {
			if (!mainPlayerCamera.isFirstPersonActive ()) {
				if (state) {
					if (mainPlayerCamera.isCameraTypeFree ()) {
						mainPlayerCamera.setCameraState (cameraStateNameOnGrapplingHookActivate);
					}
				} else {
					if (!keepCameraStateActiveWhileOnAir) {
						if (mainPlayerCamera.isCameraTypeFree ()) {
							mainPlayerCamera.setCameraState (cameraStateNameOnGrapplingHookDeactivate);
						}

						stopCoroutine = true;
					} else {
						checkinGrapplingHookCameraStateAfterDeactivate = true;
					}
				}
			} else {
				stopCoroutine = true;
			}
		} else {
			stopCoroutine = true;
		}

		if (changeFovOnHookActive) {
			if (state) {
				mainPlayerCamera.setMainCameraFov (regulaFov, changeFovSpeed);
			} else {
				mainPlayerCamera.setOriginalCameraFov ();
			}
		}

		if (useCameraShake) {
			if (state) {
				mainPlayerCamera.setShakeCameraState (true, regularCameraShakeName);
			} else {
				mainPlayerCamera.setShakeCameraState (false, "");

				mainPlayerCamera.stopShakeCamera ();
			}
		}

		if (applySpeedOnHookStop) {
			if (!state) {
				if (!ignoreApplySpeedOnHookStopOnPlayer && !ignoreApplySpeedOnHookStopTemporally) {
					lastVelocityAdded *= extraForceOnHookStopMultiplier;

					Vector3 forceOnHooKEnd = Vector3.ClampMagnitude (lastVelocityAdded, maxForceToApplyOnHookStop);

					mainPlayerController.addExternalForce (forceOnHooKEnd * extraSpeedOnHookStopMultiplier);
				}

				if (resetPlayerSpeedOnHookStop) {
					mainPlayerController.setPlayerVelocityToZero ();
				}
			}
		}

		ignoreApplySpeedOnHookStopTemporally = false;

		if (setAnimatorState) {
			if (state) {
				mainAnimator.SetInteger (actionIDAnimatorID, hookStartActionID);
				mainAnimator.CrossFadeInFixedTime (hookStartActionName, 0.1f);
			}

			mainAnimator.SetBool (actionActiveAnimatorID, state);
		}

		if (rotatePlayerTowardTargetDirection) {
			mainPlayerController.setAddExtraRotationPausedState (state);
		}

		if (state) {
			firstPersonActive = mainPlayerCamera.isFirstPersonActive ();
			previoslyFirstPersonActive = !firstPersonActive;
		}

		if (!state) {
			if (stopCoroutine) {
				stopGrapplingHookCoroutine ();
			}
		}

		manualPullActive = false;

		lastTimeAutomaticPullActive = 0;

		automaticPullActive = false;

		lastTimePullFromRaycastDetectedActive = 0;

		pullFromRaycastDetectedActive = false;

		if (!state) {
			if (checkExternalControllerStatesToIgnore) {
				if (mainPlayerController.isExternalControllBehaviorActive ()) {
					externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

					if (currentExternalControllerBehavior != null) {
						int currentIndex = otherExternalControllerInfoList.FindIndex (s => s.Name == currentExternalControllerBehavior.behaviorName);

						if (currentIndex > -1) {
							currentExternalControllerBehavior.checkResumeStateAfterExternalForceOrBehavior ();

							if (otherExternalControllerInfoList [currentIndex].useEventsOnExternalController) {
								otherExternalControllerInfoList [currentIndex].eventToResumeExternalController.Invoke ();
							}
						}
					}
				}
			}
		}
	}

	public void setIgnoreApplySpeedOnHookStopTemporallyState (bool state)
	{
		if (grapplingHookActive) {
			ignoreApplySpeedOnHookStopTemporally = state;
		}
	}

	void stopThrowHookAnimationCoroutine ()
	{
		if (animationCoroutine != null) {
			StopCoroutine (animationCoroutine);
		}

		throwAnimationInProcess = false;
	}

	IEnumerator throwHookAnimationCoroutine ()
	{
		throwAnimationInProcess = true;

		mainPlayerController.setIKBodyPausedState (true);

		mainAnimator.CrossFade (throwHookAnimationName, 0.1f);

		mainAnimator.SetBool (actionActiveAnimatorID, true);

		yield return new WaitForSeconds (throwHookAnimationDuration);

		mainAnimator.SetBool (actionActiveAnimatorID, false);

		checkThrowGrapplingHook ();

		if (grapplingHookActive) {
			checkingToRemoveHookActive = true;
		}
	
		throwAnimationInProcess = false;
	}

	public void inputThrowGrapplingHook ()
	{
		if (!grapplingHookEnabled) {
			return;
		}

		if (throwAnimationInProcess) {
			return;
		}
			
		if (useAnimationToThrowHook && !mainPlayerCamera.isFirstPersonActive ()) {
			if (!grapplingHookActive) {
				if (checkIfSurfaceDetectedBeforeAnimation) {

					calculateRaycastValues ();

					swingRaycastDetected = false;

					if (!Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, maxRaycastDistance, layerToCheckSurfaces)) {
						return;
					} else {
						if (useSwingForcesActive) {
							swingRaycastDetected = true;

							swingRacyastCheck = hit;
						}
					}
				}

				stopThrowHookAnimationCoroutine ();

				animationCoroutine = StartCoroutine (throwHookAnimationCoroutine ());
			}
		} else {
			if (useSwingForcesActive) {
				swingRaycastDetected = false;

				calculateRaycastValues ();

				if (Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, maxRaycastDistance, layerToCheckSurfaces)) {
					
					swingRaycastDetected = true;

					swingRacyastCheck = hit;
				}
			}
				
			checkThrowGrapplingHook ();
		}
	}

	public void inputRemoveGrapplingHook ()
	{
		checkRemoveGrapplingHook ();
	}

	public void inputIncreaseOrDecreaseMovementSpeed (bool state)
	{
		if (grapplingHookEnabled) {
			if (grapplingHookActive) {
				increaseSpeedActive = state;

				if (changeFovOnHookActive) {
					if (increaseSpeedActive) {
						mainPlayerCamera.setMainCameraFov (increaseSpeedFov, changeFovSpeed);
					} else {
						mainPlayerCamera.setMainCameraFov (regulaFov, changeFovSpeed);
					}
				}

				if (useCameraShake) {
					if (increaseSpeedActive) {
						mainPlayerCamera.setShakeCameraState (true, increaseCamaraShakeName);
					} else {
						mainPlayerCamera.setShakeCameraState (true, regularCameraShakeName);
					}
				}
			}
		}
	}

	public void inputThrowGrapplingHookIfTargetLocated ()
	{
		if (grapplingHookEnabled) {
			if (currentGrapplingHookTarget != null) {
				if (grapplingHookActive) {
					checkRemoveGrapplingHook ();
				} else {
					inputThrowGrapplingHook ();
				}
			}
		}
	}

	public void inputSetManualPullActiveState (bool state)
	{
		if (grapplingHookEnabled) {
			if (grapplingHookActive && useSwingForcesActive) {
				manualPullActive = state;
			}
		}
	}

	public void inputToggleManualPullActiveState ()
	{
		inputSetManualPullActiveState (!manualPullActive);
	}

	public void inputUseJumpOnSwing ()
	{
		if (grapplingHookEnabled) {
			if (grapplingHookActive && useSwingForcesActive) {
				if (jumpOnSwingEnabled) {
					checkRemoveGrapplingHook ();

					Vector3 totalForce = jumpOnSwingForceAmount.y * playerControllerTransform.up + jumpOnSwingForceAmount.z * playerControllerTransform.forward;

					mainPlayerController.useJumpPlatform (totalForce, ForceMode.Impulse);
				}
			}
		}
	}

	public void setAttractObjectsEnabled (bool state)
	{
		attractObjectsEnabled = state;
	}

	public void setUseSwingForcesActiveState (bool state)
	{
		useSwingForcesActive = state;
	}

	Vector3 ResolveCollisions (ref Vector3 playerVelocity)
	{
		Vector3 playerPosition = playerControllerTransform.position + playerControllerTransform.up;

		// Get nearby colliders
		Physics.OverlapSphereNonAlloc (playerPosition, radiusToCheckCollisionsOnSwing + 0.1f,
			_overlappingColliders, layerToCheckSurfaces);

		var totalDisplacement = Vector3.zero;
		var checkedColliderIndices = new HashSet<int> ();

		int _overlappingCollidersLength = _overlappingColliders.Length;

		if (!mainColliderAssigned) {
			mainCollider = mainPlayerController.getMainCollider ();

			mainColliderAssigned = true;
		}

		// If the player is intersecting with that environment collider, separate them
		for (var i = 0; i < _overlappingCollidersLength; i++) {
			// Two player colliders shouldn't resolve collision with the same environment collider
			if (checkedColliderIndices.Contains (i)) {
				continue;
			}

			var envColl = _overlappingColliders [i];

			// Skip empty slots
			if (envColl == null) {
				continue;
			}

			Vector3 collisionNormal;
			float collisionDistance;

			if (Physics.ComputePenetration (
				    mainCollider, mainCollider.transform.position, mainCollider.transform.rotation,
				    envColl, envColl.transform.position, envColl.transform.rotation,
				    out collisionNormal, out collisionDistance)) {
				// Ignore very small penetrations Required for standing still on slopes .. still far from perfect though
//				if (collisionDistance < 0.015) {
//					continue;
//				}

				checkedColliderIndices.Add (i);

				// Get outta that collider!
				totalDisplacement += collisionNormal * collisionDistance;

				// Crop down the velocity component which is in the direction of penetration
				playerVelocity -= Vector3.Project (playerVelocity, collisionNormal);
			}
		}

		// It's better to be in a clean state in the next resolve call
		for (var i = 0; i < _overlappingColliders.Length; i++) {
			_overlappingColliders [i] = null;
		}
			
		bool surfacesLocated = false;

		Vector3 raycastDirection = -playerControllerTransform.up;
		Vector3 raycastPosition = playerPosition;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, 3, layerToCheckSurfaces)) {
			totalDisplacement += hit.normal * hit.distance;

			playerVelocity -= Vector3.Project (playerVelocity, hit.normal);

			surfacesLocated = true;
		} 

		raycastDirection = playerControllerTransform.forward;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, 2, layerToCheckSurfaces)) {
			totalDisplacement += hit.normal * hit.distance;

			playerVelocity -= Vector3.Project (playerVelocity, hit.normal);
		} 

		raycastDirection = -playerControllerTransform.forward;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, 2, layerToCheckSurfaces)) {
			totalDisplacement += hit.normal * hit.distance;

			playerVelocity -= Vector3.Project (playerVelocity, hit.normal);
		} 

		raycastDirection = playerControllerTransform.right;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, 2, layerToCheckSurfaces)) {
			totalDisplacement += hit.normal * hit.distance;

			playerVelocity -= Vector3.Project (playerVelocity, hit.normal);
		} 

		raycastDirection = -playerControllerTransform.right;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, 2, layerToCheckSurfaces)) {
			totalDisplacement += hit.normal * hit.distance;

			playerVelocity -= Vector3.Project (playerVelocity, hit.normal);
		} 

		if (surfacesLocated) {
			pullFromRaycastDetectedActive = true;

			lastTimePullFromRaycastDetectedActive = Time.time;
		}

		return totalDisplacement;
	}

	public void setCurrentPlayerActionSystemCustomActionCategoryID (bool state)
	{
		if (state) {
			if (customActionCategoryID > -1) {
				mainPlayerController.setCurrentCustomActionCategoryID (customActionCategoryID);
			}
		} else {
			if (regularActionCategoryID > -1) {
				mainPlayerController.setCurrentCustomActionCategoryID (regularActionCategoryID);
			}
		}
	}

	[System.Serializable]
	public class otherExternalControllerInfo
	{
		public string Name;

		[Space]
		[Header ("Events Settings")]
		[Space]

		public bool useEventsOnExternalController;
		public UnityEvent eventToPauseExternalController;
		public UnityEvent eventToResumeExternalController;
	}
}
