using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerTeleportSystem : abilityInfo
{
	[Header ("Custom Settings")]
	[Space]

	//Teleport ability variables
	public bool teleportingEnabled;
	public LayerMask teleportLayerMask;

	public float maxDistanceToTeleport = 100;
	public bool useTeleportIfSurfaceNotFound = true;
	public float maxDistanceToTeleportAir = 10;

	public float holdButtonTimeToActivateTeleport = 0.4f;
	public bool stopTeleportIfMoving;
	public bool addForceIfTeleportStops;
	public float forceIfTeleportStops;

	public bool canTeleportOnZeroGravity;

	public bool teleportInstantlyToPosition;

	public bool useFixedTeleportHookPointsEnabled = true;

	[Space]
	[Header ("Capsule Cast Settings")]
	[Space]

	public bool useCapsuleCastToCheckTargetReached = true;
	public float capsuleCastRadius = 0.5f;
	public float maxCapsuleCastDistance = 1.3f;

	[Space]
	[Header ("Camera Settings")]
	[Space]

	public bool useSmoothCameraFollowStateOnTeleport = true;
	public float smoothCameraFollowDuration = 3;

	[Space]
	[Header ("Movement/Rotation Settings")]
	[Space]

	public float teleportSpeed = 10;

	public bool rotateTowardTeleportPosition;

	public float teleportRotationSpeed = 5;
	public float minAngleToRotateTowardTeleportDirection = 15;

	[Space]
	[Header ("Effect Settings")]
	[Space]

	public bool useBulletTimeOnTeleport;
	public float bulletTimeScaleOnTeleport = 0.5f;

	public bool useTeleportMark;

	public bool changeCameraFovOnTeleport;
	public float cameraFovOnTeleport;
	public float cameraFovOnTeleportSpeed;

	[Space]
	[Header ("Animation Settings")]
	[Space]

	public bool useActionSystemOnTeleport = true;
	public string actionNameUsedOnTeleport = "Teleport Pose";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool teleportCanBeExecuted;
	public bool searchingForTeleport;

	public bool teleportSurfaceFound;
	public bool teleportInProcess;

	public Transform currentTeleportHookTarget;

	public bool teleportActivateByHookTarget;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStartTeleport;
	public UnityEvent eventsOnStartTeleport;
	public bool useEventsOnEndTeleport;
	public UnityEvent eventsOnEndTeleport;

	[Space]
	[Header ("Component Elements")]
	[Space]

	public GameObject teleportMark;
	public Transform playerControllerTransform;
	public Transform mainCameraTransform;
	public gravitySystem mainGravitySystem;
	public playerController mainPlayerController;
	public timeBullet mainTimeBullet;
	public playerCamera mainPlayerCamera;


	float lastTimeTeleportButtonPressed;
	Coroutine teleportCoroutine;
	Vector3 currentTeleportPosition;
	Vector3 teleportMarkDirection;
	Vector3 currentTeleportPositionNormal;

	RaycastHit hit;


	public override void updateAbilityState ()
	{
		if (teleportingEnabled && !mainGravitySystem.isSearchingSurface () && (!mainPlayerController.isPlayerOnZeroGravityMode () || canTeleportOnZeroGravity)) {
			if (searchingForTeleport) {
				bool activateTeleportResult = false;
			
				if (!teleportCanBeExecuted) {
					if (Time.time > lastTimeTeleportButtonPressed + holdButtonTimeToActivateTeleport || teleportInProcess) { 
						activateTeleportResult = true;
					}

					if (!teleportInProcess && currentTeleportHookTarget != null) {
						activateTeleportResult = true;

						teleportActivateByHookTarget = true;
					}
				}

				if (activateTeleportResult) {
					teleportCanBeExecuted = true;

					if (useTeleportMark) {
						teleportMark.SetActive (true);
					}

					bool canActivateTeleportOnAir = teleportInProcess || !mainPlayerController.isPlayerOnGround ();

					if (canActivateTeleportOnAir) {
						stopTeleporting ();
					}

					if (useBulletTimeOnTeleport) {
						mainTimeBullet.setBulletTimeState (true, bulletTimeScaleOnTeleport); 
					}

					if (canActivateTeleportOnAir) {
						setPlayerControlState (false);
					}
				}

				if (teleportCanBeExecuted && useTeleportMark) {
					Vector3 raycastDirection = Vector3.zero;
					Vector3 raycastPosition = Vector3.zero;
				
					if (currentTeleportHookTarget != null) {
						raycastPosition = playerControllerTransform.position + playerControllerTransform.up;

						raycastDirection = currentTeleportHookTarget.position - raycastPosition;

						raycastDirection.Normalize ();
					} else {
						raycastDirection = mainCameraTransform.TransformDirection (Vector3.forward);

						raycastPosition = mainCameraTransform.position;
					}

					if (Physics.Raycast (raycastPosition, raycastDirection, out hit, maxDistanceToTeleport, teleportLayerMask)) {
						currentTeleportPosition = hit.point + hit.normal * 0.4f;
						teleportMark.transform.position = hit.point + hit.normal * 0.1f;

						currentTeleportPositionNormal = hit.normal;

						teleportSurfaceFound = true;
					} else {
						teleportSurfaceFound = false;

						currentTeleportPosition = mainCameraTransform.position + mainCameraTransform.forward * maxDistanceToTeleportAir;
					
						teleportMark.transform.position = currentTeleportPosition;

						currentTeleportPositionNormal = Vector3.up;
					}
						
					if (useTeleportIfSurfaceNotFound || (!useTeleportIfSurfaceNotFound && teleportSurfaceFound)) {
						Quaternion teleportMarkTargetRotation = Quaternion.LookRotation (currentTeleportPositionNormal);

						teleportMark.transform.rotation = teleportMarkTargetRotation;

						if (!teleportMark.activeSelf) {
							teleportMark.SetActive (true);
						}
					} else {
						if (teleportMark.activeSelf) {
							teleportMark.SetActive (false);
						}
					}
				}

				if (mainPlayerAbilitiesSystem.playerCurrentlyBusy ||
				    (stopTeleportIfMoving && mainPlayerController.isPlayerUsingInput () && !teleportActivateByHookTarget)) {

					stopTeleportInProcess ();
				}
			}

			if (teleportInProcess && stopTeleportIfMoving && mainPlayerController.isPlayerUsingInput () && !teleportActivateByHookTarget) {
				stopTeleporting ();

				setTeleportEndState ();

				if (addForceIfTeleportStops) {
					Vector3 targetPositionDirection = currentTeleportPosition - playerControllerTransform.position;

					targetPositionDirection = targetPositionDirection / targetPositionDirection.magnitude;

					mainPlayerController.addExternalForce (targetPositionDirection * forceIfTeleportStops);
				}
			}
		}
	}

	public void stopTeleportInProcess ()
	{
		if (teleportInProcess || searchingForTeleport) {
			teleportCanBeExecuted = false;

			if (useBulletTimeOnTeleport) {
				mainTimeBullet.disableTimeBulletTotally (); 
			}

			teleportActivateByHookTarget = false;

			searchingForTeleport = false;

			if (useTeleportMark) {
				teleportMark.SetActive (false);
			}

			stopTeleporting ();
		}
	}

	public void teleportPlayer (Transform objectToMove, Vector3 teleportPosition, bool checkPositionAngle, bool changeFov,
	                            float newFovValue, float fovChangeSpeed, float teleportSpeed, bool rotatePlayerTowardPosition, 
	                            bool teleportInstantlyToPositionValue, bool useActionSystemOnTeleportValue, 
	                            bool useSmoothCameraFollowStateOnTeleportValue, float smoothCameraFollowDurationValue, 
	                            float teleportDistanceOffsetValue)
	{
		stopTeleporting ();

		teleportCoroutine = StartCoroutine (teleportPlayerCoroutine (objectToMove, teleportPosition, checkPositionAngle, 
			changeFov, newFovValue, fovChangeSpeed, teleportSpeed, rotatePlayerTowardPosition, teleportInstantlyToPositionValue, 
			useActionSystemOnTeleportValue, useSmoothCameraFollowStateOnTeleportValue, smoothCameraFollowDurationValue, teleportDistanceOffsetValue));
	}

	public void stopTeleporting ()
	{
		if (teleportCoroutine != null) {
			StopCoroutine (teleportCoroutine);
		}

		setPlayerControlState (true);
	}

	public void resumeIfTeleportActive ()
	{
		if (teleportInProcess) {
			stopTeleporting ();

			setTeleportEndState ();
		}
	}

	bool currentUseActionSystemOnTeleportValue;

	public void setTeleportEndState ()
	{
		if (teleportInProcess) {
			teleportInProcess = false;

			if (currentUseActionSystemOnTeleportValue && actionNameUsedOnTeleport != "") {
				mainPlayerController.stopCustomAction (actionNameUsedOnTeleport);
			}

			if (changeCameraFovOnTeleport) {
				mainPlayerCamera.setOriginalCameraFov ();
			}

			checkEventOnTeleport (false);
		}
	}

	public void setPlayerControlState (bool state)
	{
		mainPlayerController.changeScriptState (state);

		mainPlayerController.setGravityForcePuase (!state);

		mainPlayerController.setRigidbodyVelocityToZero ();

		mainPlayerController.setPhysicMaterialAssigmentPausedState (!state);

		mainPlayerController.setUsingAbilityActiveState (!state);

		mainPlayerController.setCheckOnGroungPausedState (!state);

		if (!state) {
			mainPlayerController.setZeroFrictionMaterial ();

			mainPlayerController.setPlayerOnGroundState (false);

			mainPlayerController.setLastTimeFalling ();
		}
	}

	IEnumerator teleportPlayerCoroutine (Transform objectToMove, Vector3 targetPosition, bool checkPositionAngle, 
	                                     bool changeFov, float newFovValue, float fovChangeSpeed, float currentTeleportSpeed,
	                                     bool rotatePlayerTowardPosition, bool teleportInstantlyToPositionValue, bool useActionSystemOnTeleportValue,
	                                     bool useSmoothCameraFollowStateOnTeleportValue, float smoothCameraFollowDurationValue,
	                                     float teleportDistanceOffsetValue)
	{
		teleportInProcess = true;

		checkEventOnTeleport (true);

		currentUseActionSystemOnTeleportValue = useActionSystemOnTeleportValue;

		if (currentUseActionSystemOnTeleportValue && actionNameUsedOnTeleport != "") {
			mainPlayerController.activateCustomAction (actionNameUsedOnTeleport);
		}

		if (useSmoothCameraFollowStateOnTeleportValue) {
			mainPlayerCamera.activateUseSmoothCameraFollowStateDuration (smoothCameraFollowDurationValue);
		}

		setPlayerControlState (false);

		if (changeCameraFovOnTeleport && changeFov) {
			mainPlayerCamera.setMainCameraFov (newFovValue, fovChangeSpeed);
		}

		float dist = GKC_Utils.distance (objectToMove.position, targetPosition);
		float duration = dist / currentTeleportSpeed;
		float t = 0;

		Vector3 targetPositionDirection = targetPosition - objectToMove.position;
		targetPositionDirection = targetPositionDirection / targetPositionDirection.magnitude;

		if (checkPositionAngle) {
			float targetNormalAngle = Vector3.Angle (objectToMove.up, currentTeleportPositionNormal);

			if (targetNormalAngle > 150) {
				targetPosition += currentTeleportPositionNormal * 2;
			}
		}
	
		float targetEulerRotation = 0;

		bool targetReached = false;

		float movementTimer = 0;

		float positionDifference = 0;

		float positionDifferenceAmount = 0.01f;

		if (teleportDistanceOffsetValue > 0) {
			positionDifferenceAmount = teleportDistanceOffsetValue;
		}

		float lastTimeDistanceChecked = 0;

		if (teleportInstantlyToPositionValue) {
			if (positionDifferenceAmount > 0) {
				targetPosition -= targetPositionDirection * positionDifferenceAmount;
			}

			objectToMove.position = targetPosition;

			if (rotatePlayerTowardPosition) {

				Vector3 teleportDirection = targetPositionDirection;

				teleportDirection = teleportDirection - objectToMove.up * objectToMove.InverseTransformDirection (teleportDirection).y;

				targetEulerRotation = Vector3.SignedAngle (objectToMove.forward, teleportDirection, objectToMove.up);

				objectToMove.Rotate (0, targetEulerRotation, 0);
			}
		} else {

			while (!targetReached) {
				t += Time.deltaTime / duration;

				objectToMove.position = Vector3.Lerp (objectToMove.position, targetPosition, t);

				if (rotatePlayerTowardPosition) {

					Vector3 teleportDirection = targetPosition - objectToMove.position;
					teleportDirection = teleportDirection / teleportDirection.magnitude;

					teleportDirection = teleportDirection - objectToMove.up * objectToMove.InverseTransformDirection (teleportDirection).y;

					targetEulerRotation = Vector3.SignedAngle (objectToMove.forward, teleportDirection, objectToMove.up);

					if (Mathf.Abs (targetEulerRotation) > minAngleToRotateTowardTeleportDirection) {
						objectToMove.Rotate (0, (targetEulerRotation / 2) * teleportRotationSpeed * Time.deltaTime, 0);
					}
				}

				positionDifference = GKC_Utils.distance (objectToMove.position, targetPosition);

				if (lastTimeDistanceChecked == 0) {
					if (positionDifference < 1) {
						lastTimeDistanceChecked = Time.time;
					}
				} else {
					if (Time.time > lastTimeDistanceChecked + 0.5f) {
//						print ("too much time without moving");

						targetReached = true;
					}
				}

				movementTimer += Time.deltaTime;

				if (positionDifference < positionDifferenceAmount || movementTimer > (duration + 0.5f)) {
					targetReached = true;
				}

				if (useCapsuleCastToCheckTargetReached) {
					if (movementTimer > 0.7f) {
						Vector3 currentObjectPosition = objectToMove.position;
				
						Vector3 point1 = currentObjectPosition;
						Vector3 point2 = currentObjectPosition + objectToMove.up * 2;
				
						if (Physics.CapsuleCast (point1, point2, capsuleCastRadius, targetPositionDirection, out hit, maxCapsuleCastDistance, teleportLayerMask)) {
//							print ("surface detected");

							targetReached = true;
						}
					}
				}

				yield return null;
			}
		}

		setPlayerControlState (true);

		setTeleportEndState ();
	}

	public void holdTeleport ()
	{
		if (!mainPlayerAbilitiesSystem.playerCurrentlyBusy &&
		    teleportingEnabled &&
		    (!mainPlayerController.isPlayerUsingInput () || currentTeleportHookTarget != null) &&
		    !mainGravitySystem.isSearchingSurface () &&
		    (!mainPlayerController.isPlayerOnZeroGravityMode () || canTeleportOnZeroGravity)) {

			searchingForTeleport = true;

			lastTimeTeleportButtonPressed = Time.time;
		}
	}

	public void releaseTeleport ()
	{
		if (!mainPlayerAbilitiesSystem.playerCurrentlyBusy && teleportingEnabled) {
			if (teleportCanBeExecuted) {
				if (useTeleportIfSurfaceNotFound || (!useTeleportIfSurfaceNotFound && teleportSurfaceFound)) {
					teleportPlayer (playerControllerTransform, currentTeleportPosition, true, true, cameraFovOnTeleport, 
						cameraFovOnTeleportSpeed, teleportSpeed, rotateTowardTeleportPosition, teleportInstantlyToPosition, 
						useActionSystemOnTeleport, useSmoothCameraFollowStateOnTeleport, smoothCameraFollowDuration, 0);
				
					mainPlayerController.setCheckOnGroungPausedState (true);

					mainPlayerController.setPlayerOnGroundState (false);

				} else {
					stopTeleporting ();
				}

				if (useBulletTimeOnTeleport) {
					mainTimeBullet.setBulletTimeState (false, 1); 
				}
			}

			teleportCanBeExecuted = false;

			searchingForTeleport = false;

			if (useTeleportMark) {
				teleportMark.SetActive (false);
			}
		}
	}

	public void setTeleportingEnabledState (bool state)
	{
		teleportingEnabled = state;
	}

	public bool isTeleportInProcess ()
	{
		return teleportInProcess;
	}

	public bool isSearchingForTeleport ()
	{
		return searchingForTeleport;
	}

	public bool getTeleportCanBeExecutedState ()
	{
		return teleportCanBeExecuted;
	}

	public override void enableAbility ()
	{
		
	}

	public override void disableAbility ()
	{
		stopTeleportInProcess ();

		teleportingEnabled = false;
	}

	public override void deactivateAbility ()
	{
		stopTeleportInProcess ();
	}

	public override void activateSecondaryActionOnAbility ()
	{
		
	}

	public override void useAbilityPressDown ()
	{
		holdTeleport ();

		checkUseEventOnUseAbility ();
	}

	public override void useAbilityPressHold ()
	{
		
	}

	public override void useAbilityPressUp ()
	{
		releaseTeleport ();

		disableAbilityCurrentActiveFromPressState ();
	}

	void checkEventOnTeleport (bool state)
	{
		if (state) {
			if (useEventsOnStartTeleport) {
				eventsOnStartTeleport.Invoke ();
			}
		} else {
			if (useEventsOnEndTeleport) {
				eventsOnEndTeleport.Invoke ();
			}
		}
	}

	public void setTeleportHookTarget (Transform newTarget)
	{
		if (!useFixedTeleportHookPointsEnabled) {
			return;
		}

		currentTeleportHookTarget = newTarget;
	}

}
