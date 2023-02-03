using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class walkOnBalanceSystem : externalControllerBehavior
{
	[Header ("Main Settings")]
	[Space]

	public bool systemEnabled = true;

	public float lookDirectionSpeed;

	public float resetPlayerRotationSpeed = 5;

	public float adjustPlayerToMovementSpeed = 5;

	public float movementSpeed = 5;

	public float turboSpeed = 4;

	public float delayToResumeMovementOnReverseDirection = 0.7f;

	public bool stopMovementIfLastPointReached = true;
	public bool stopMovementIfFirstPointReached = true;

	[Space]
	[Header ("Physics Settings")]
	[Space]

	public bool useForceMode;

	public ForceMode railForceMode;

	public bool turboEnabled;

	public float speedOnAimMultiplier;

	[Space]
	[Header ("Jump Settings")]
	[Space]

	public bool canJumpEnabled;

	public bool jumpOnEndStateEnabled = true;

	public Vector3 impulseOnJump;
	public Vector3 endOfSurfaceImpulse;

	public float maxVelocityChangeOnJump;

	[Space]
	[Header ("Third Person Settings")]
	[Space]

	public int actionID = 08632946;

	public string externalControlleBehaviorActiveAnimatorName = "External Behavior Active";
	public string actionIDAnimatorName = "Action ID";

	public string horizontalAnimatorName = "Horizontal Action";

	public float inputLerpSpeed = 3;

	[Space]
	[Header ("Third Person Camera State Settings")]
	[Space]

	public bool setNewCameraStateOnThirdPerson;

	public string newCameraStateOnThirdPerson;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool fallAfterDelayIfNotMoving;
	public float delayToFallIfNotMoving;
	public bool fallAfterDelayIfMoving;
	public float delayToFallIfMoving;

	public int actionIDToFall = 4548931;


	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool movementPausedDebug;

	public bool movementSystemActive;

	public bool turboActive;

	public Vector3 targetDirection;

	public simpleWaypointSystem currentSimpleWaypointSystem;

	public List<Transform> currentWayPoints = new List<Transform> ();

	public bool adjustingPlayerToMovement;

	public bool movingOnForwardDirection;
	public bool movingOnBackwardDirection;

	public int currentWaypointIndex;

	public int nextTargetWaypointIndex;

	public float verticalInput;

	public float movementDirectionMultiplier = 1;

	public bool jumpOnEndState;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	public UnityEvent eventBeforeActivatingState;
	public UnityEvent eventBeforeDeactivatingState;

	public bool useEventUseTurbo;
	public UnityEvent eventOnStarTurbo;
	public UnityEvent eventOnEndTurbo;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	public Animator mainAnimator;

	public playerCamera mainPlayerCamera;

	public Transform playerTransform;

	public Rigidbody mainRigidbody;

	public headTrack mainHeadTrack;

	float currentAimSpeedMultipler;

	bool canUseTurboPausedState;

	int externalControlleBehaviorActiveAnimatorID;

	int actionIDAnimatorID;

	int horizontalAnimatorID;

	bool isFirstPersonActive;

	string previousCameraState = "";

	bool turnAndForwardAnimatorValuesPaused;

	Coroutine adjustPlayerToMovementCoroutine;

	Coroutine resetPlayerCoroutine;

	Vector3 lookDirection;

	float lastTimeReverseMovementDirectionInput;

	float targetRotation;

	Vector3 currentTargetDirection;

	float lastTimeMoving;

	float lastTimeNotMoving;

	bool ignoreJumpActive;

	bool setPlayerAsChildActive;
	Transform playerParentTransform;
	Transform previousParent;

	float lastTimeWalkOnBalanceActive;


	void Start ()
	{
		externalControlleBehaviorActiveAnimatorID = Animator.StringToHash (externalControlleBehaviorActiveAnimatorName);

		actionIDAnimatorID = Animator.StringToHash (actionIDAnimatorName);

		horizontalAnimatorID = Animator.StringToHash (horizontalAnimatorName);
	}

	public override void updateControllerBehavior ()
	{
		if (movementSystemActive && !movementPausedDebug) {
			if (!adjustingPlayerToMovement) {
				bool targetReached = false;

				if (mainPlayerController.isPlayerRunning ()) {
					if (!turboActive) {
						enableOrDisableTurbo (true);
					}
				} else {
					if (turboActive) {
						enableOrDisableTurbo (false);
					}
				}

				int slidingAnimatorValue = 0;

				verticalInput = mainPlayerController.getRawAxisValues ().y;

				verticalInput *= movementDirectionMultiplier;

				if (verticalInput != 0) {
					slidingAnimatorValue = 1;

					if (verticalInput > 0) {
						if (!movingOnForwardDirection) {
							movingOnForwardDirection = true;
							movingOnBackwardDirection = false;

							lastTimeReverseMovementDirectionInput = Time.time;

							if (showDebugPrint) {
								print ("moving forward");
							}

							currentTargetDirection = Vector3.zero;

							nextTargetWaypointIndex++;

							if (nextTargetWaypointIndex >= currentWayPoints.Count - 1) {
								nextTargetWaypointIndex = currentWayPoints.Count - 1;
							}
						}
					} else {
						if (!movingOnBackwardDirection) {
							movingOnForwardDirection = false;
							movingOnBackwardDirection = true;

							lastTimeReverseMovementDirectionInput = Time.time;

							if (showDebugPrint) {
								print ("moving backward");
							}

							currentTargetDirection = Vector3.zero;

							nextTargetWaypointIndex--;

							if (nextTargetWaypointIndex < 0) {
								nextTargetWaypointIndex = 0;
							}
						}
					}

					lastTimeMoving = Time.time;

					if (fallAfterDelayIfMoving && lastTimeNotMoving > 0) {
						if (Time.time > lastTimeNotMoving + delayToFallIfMoving) {
							mainAnimator.SetInteger (actionIDAnimatorID, actionID);

							ignoreJumpActive = true;

							stopMovement ();
						}
					}
						
				} else {
					if (fallAfterDelayIfNotMoving && lastTimeMoving > 0) {			
						if (Time.time > lastTimeMoving + delayToFallIfNotMoving) {
							mainAnimator.SetInteger (actionIDAnimatorID, actionID);

							ignoreJumpActive = true;

							stopMovement ();
						}
					}

					lastTimeNotMoving = Time.time;

				}

				float currentSpeed = movementSpeed;

				if (turboActive) {
					slidingAnimatorValue = 2;

					currentSpeed = turboSpeed;
				}

				currentAimSpeedMultipler = 1;

				if (!mainPlayerController.isPlayerOnFirstPerson () && mainPlayerController.isPlayerAiming ()) {
					currentAimSpeedMultipler = speedOnAimMultiplier;

					if (!turnAndForwardAnimatorValuesPaused) {
						mainPlayerController.setTurnAndForwardAnimatorValuesPausedState (true);

						turnAndForwardAnimatorValuesPaused = true;
					}
				} else {
					if (turnAndForwardAnimatorValuesPaused) {
						mainPlayerController.setTurnAndForwardAnimatorValuesPausedState (false);

						turnAndForwardAnimatorValuesPaused = false;
					}
				}

				currentSpeed *= currentAimSpeedMultipler;

				Vector3 targetPosition = currentWayPoints [nextTargetWaypointIndex].position;

				float distanceToNextWaypoint = GKC_Utils.distance (playerTransform.position, targetPosition);

				if (distanceToNextWaypoint < 0.3f) {

					if (movingOnForwardDirection) {
						if (nextTargetWaypointIndex < currentWayPoints.Count - 1) {
							nextTargetWaypointIndex++;

							if (showDebugPrint) {
								print (nextTargetWaypointIndex);

								print ("changing to next waypoint");
							}

							currentTargetDirection = Vector3.zero;

							targetPosition = currentWayPoints [nextTargetWaypointIndex].position;

							distanceToNextWaypoint = GKC_Utils.distance (playerTransform.position, targetPosition);
						}
					} else {
						if (nextTargetWaypointIndex > 0) {
							nextTargetWaypointIndex--;
						
							if (showDebugPrint) {
								print (nextTargetWaypointIndex);

								print ("changing to previous waypoint");
							}

							currentTargetDirection = Vector3.zero;

							targetPosition = currentWayPoints [nextTargetWaypointIndex].position;

							distanceToNextWaypoint = GKC_Utils.distance (playerTransform.position, targetPosition);
						}
					}
				}

				bool canMove = true;

				if (lastTimeReverseMovementDirectionInput > 0) {
					if (Time.time < lastTimeReverseMovementDirectionInput + delayToResumeMovementOnReverseDirection) {
						canMove = false;

						slidingAnimatorValue = 1;
					} else {
						lastTimeReverseMovementDirectionInput = 0;
					}
				}

				if (verticalInput != 0 && canMove) {
					playerTransform.position = Vector3.MoveTowards (playerTransform.position, targetPosition, Time.deltaTime * currentSpeed);
				}
					

				lookDirection = targetPosition - playerTransform.position;

				lookDirection = lookDirection / lookDirection.magnitude;

				if (distanceToNextWaypoint < 0.4f) {		
					if (currentTargetDirection == Vector3.zero) {
						currentTargetDirection = lookDirection;

						if (stopMovementIfLastPointReached) {
							if (movingOnForwardDirection) {
								if (nextTargetWaypointIndex == currentWayPoints.Count - 1) {
									targetReached = true;
								}
							}
						}

						if (stopMovementIfFirstPointReached) {
							if (movingOnBackwardDirection) {
								if (nextTargetWaypointIndex == 0) {
									targetReached = true;
								}
							}
						}
					}

					lookDirection = currentTargetDirection;
				}

				if (showDebugPrint) {
					Debug.DrawRay (playerTransform.position, lookDirection * 5, Color.yellow, 1);
				}

				float angle = Vector3.SignedAngle (playerTransform.forward, lookDirection, mainPlayerController.getCurrentNormal ());

				targetRotation = Mathf.Lerp (targetRotation, angle, lookDirectionSpeed * Time.fixedDeltaTime);

				if (Mathf.Abs (targetRotation) > 0.1f) {
					playerTransform.Rotate (0, targetRotation * Time.fixedDeltaTime, 0);
				}

				mainAnimator.SetFloat (horizontalAnimatorID, slidingAnimatorValue, inputLerpSpeed, Time.fixedDeltaTime);

				if (targetReached) {
					stopMovement ();
				}
			}
		}
	}

	public override void setExtraImpulseForce (Vector3 forceAmount, bool useCameraDirection)
	{
		setImpulseForce (forceAmount, useCameraDirection);
	}

	public void setImpulseForce (Vector3 forceAmount, bool useCameraDirection)
	{
		Vector3 impulseForce = forceAmount;

		Vector3 velocityChange = Vector3.zero;

		if (maxVelocityChangeOnJump > 0) {
			velocityChange = impulseForce - mainRigidbody.velocity;

			velocityChange = Vector3.ClampMagnitude (velocityChange, maxVelocityChangeOnJump);

		} else {
			velocityChange = impulseForce;
		}

		mainPlayerController.setVelocityChangeValue (impulseForce);

		mainRigidbody.AddForce (impulseForce, ForceMode.VelocityChange);
	}

	public override void setJumpActiveForExternalForce ()
	{
		setJumpActive (impulseOnJump);
	}

	public void setJumpActive (Vector3 newImpulseOnJumpAmount)
	{
		if (movementSystemActive) {
			setMovementSystemActivestate (false);

			Vector3 totalForce = newImpulseOnJumpAmount.y * playerTransform.up + newImpulseOnJumpAmount.z * playerTransform.forward;

			mainPlayerController.useJumpPlatform (totalForce, ForceMode.Impulse);
		}
	}

	public override void setExternalForceActiveState (bool state)
	{
		setMovementSystemActivestate (state);
	}

	public void setMovementSystemActivestate (bool state)
	{
		if (!systemEnabled) {
			return;
		}

		if (movementSystemActive == state) {
			return;
		}

		if (mainPlayerController.isUseExternalControllerBehaviorPaused ()) {
			return;
		}

		if (state && mainPlayerController.isPlayerDead ()) {
			return;
		}

		if (state) {
			if (lastTimeWalkOnBalanceActive > 0 && Time.time < lastTimeWalkOnBalanceActive + 1) {
				return;
			}
		}

		if (state) {
			externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

			if (currentExternalControllerBehavior != null && currentExternalControllerBehavior != this) {
				if (canBeActivatedIfOthersBehaviorsActive && checkIfCanEnableBehavior (currentExternalControllerBehavior.behaviorName)) {
					currentExternalControllerBehavior.disableExternalControllerState ();
				} else {
					return;
				}
			}
		}

		bool modeActivePrevioulsy = movementSystemActive;

		movementSystemActive = state;

		setBehaviorCurrentlyActiveState (state);

		setCurrentPlayerActionSystemCustomActionCategoryID ();

		if (showDebugPrint) {
			print ("Setting state as " + movementSystemActive);
		}

		mainPlayerController.setExternalControlBehaviorForAirTypeActiveState (state);

		mainPlayerController.setAddExtraRotationPausedState (state);

		if (state) {
			mainPlayerController.setCheckOnGroungPausedState (true);

			mainPlayerController.setPlayerOnGroundState (false);

			mainPlayerController.setPlayerOnGroundAnimatorStateOnOverrideOnGroundWithTime (false);

			mainPlayerController.overrideOnGroundAnimatorValue (0);

			mainPlayerController.setPlayerOnGroundAnimatorStateOnOverrideOnGround (false);

			mainPlayerController.setOnGroundAnimatorIDValue (false);

			lastTimeWalkOnBalanceActive = Time.time;

		} else {
			mainPlayerController.setCheckOnGroungPausedState (false);

			mainPlayerController.setPlayerOnGroundState (false);

			mainPlayerController.setPlayerOnGroundAnimatorStateOnOverrideOnGroundWithTime (true);

			mainPlayerController.disableOverrideOnGroundAnimatorValue ();

			mainPlayerController.setPauseResetAnimatorStateFOrGroundAnimatorState (true);

			mainPlayerController.setOnGroundAnimatorIDValue (false);

			lastTimeWalkOnBalanceActive = 0;
		}

		mainPlayerController.setFootStepManagerState (state);

		mainPlayerController.setIgnoreExternalActionsActiveState (state);

		mainPlayerController.setPlayerVelocityToZero ();

		if (movementSystemActive) {
			mainPlayerController.setExternalControllerBehavior (this);
		} else {
			if (modeActivePrevioulsy) {
				externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (currentExternalControllerBehavior == null || currentExternalControllerBehavior == this) {
					mainPlayerController.setExternalControllerBehavior (null);
				}

				if (playerTransform.up != mainPlayerController.getCurrentNormal ()) {
					resetPlayerRotation ();
				}
			}
		}

		if (state) {
			eventBeforeActivatingState.Invoke ();
		} else {
			eventBeforeDeactivatingState.Invoke ();
		}

		mainPlayerController.setFallDamageCheckPausedState (state);

		if (movementSystemActive) {
			eventOnStateEnabled.Invoke ();
		} else {
			eventOnStateDisabled.Invoke ();

			if (turboActive) {
				enableOrDisableTurbo (false);
			}
		}

		mainHeadTrack.setExternalHeadTrackPauseActiveState (state);

		mainPlayerController.stopShakeCamera ();

		mainPlayerController.setPauseCameraShakeFromGravityActiveState (state);

		mainPlayerCamera.setPausePlayerCameraViewChangeState (state);

		mainPlayerController.setLastTimeFalling ();

		if (state) {
			Vector3 playerPosition = playerTransform.position;

			getClosestPosition (playerPosition);

			int nextWaypointIndex = -1;

			int previousWaypointIndex = -1;

			Vector3 targetPosition = Vector3.zero;

			if (currentWaypointIndex == 0) {
				nextWaypointIndex = 1;

				targetPosition = getClosestPointToLine (playerPosition, currentWayPoints [currentWaypointIndex].position, currentWayPoints [nextWaypointIndex].position);

				Vector3 forwardDirection = currentWayPoints [currentWaypointIndex].position - currentWayPoints [nextWaypointIndex].position;

				forwardDirection = forwardDirection / forwardDirection.magnitude;

				float angleForward = Vector3.SignedAngle (playerTransform.forward, forwardDirection, playerTransform.up);

				if (showDebugPrint) {
					print (Mathf.Abs (angleForward));
				}

				if (Mathf.Abs (angleForward) > 90) {
					movingOnForwardDirection = true;
					movingOnBackwardDirection = false;
				} else {
					movingOnForwardDirection = false;
					movingOnBackwardDirection = true;
				}

				if (showDebugPrint) {
					print ("1 " + movingOnForwardDirection + " " + movingOnBackwardDirection);
				}

			} else if (currentWaypointIndex == currentWayPoints.Count - 1) {
				previousWaypointIndex = currentWaypointIndex - 1;

				targetPosition = getClosestPointToLine (playerPosition, currentWayPoints [currentWaypointIndex].position, currentWayPoints [previousWaypointIndex].position);

				Vector3 backwardDirection = currentWayPoints [previousWaypointIndex].position - currentWayPoints [currentWaypointIndex].position;

				backwardDirection = backwardDirection / backwardDirection.magnitude;

				float angleForward = Vector3.SignedAngle (playerTransform.forward, backwardDirection, playerTransform.up);

				if (showDebugPrint) {
					print (Mathf.Abs (angleForward));
				}

				if (Mathf.Abs (angleForward) > 90) {
					movingOnForwardDirection = true;
					movingOnBackwardDirection = false;
				} else {
					movingOnForwardDirection = false;
					movingOnBackwardDirection = true;
				}

				if (showDebugPrint) {
					print ("2 " + movingOnForwardDirection + " " + movingOnBackwardDirection);
				}
			} else if (currentWayPoints.Count > 2) {
				nextWaypointIndex = currentWaypointIndex + 1;

				previousWaypointIndex = currentWaypointIndex - 1;


				Vector3 forwardDirection = currentWayPoints [currentWaypointIndex].position - currentWayPoints [nextWaypointIndex].position;

				forwardDirection = forwardDirection / forwardDirection.magnitude;


				Vector3 backwardDirection = currentWayPoints [previousWaypointIndex].position - currentWayPoints [currentWaypointIndex].position;

				backwardDirection = backwardDirection / backwardDirection.magnitude;


				float angleForward = Vector3.SignedAngle (playerTransform.forward, forwardDirection, playerTransform.up);

				float angleBackward = Vector3.SignedAngle (playerTransform.forward, backwardDirection, playerTransform.up);

				if (Mathf.Abs (angleForward) < Mathf.Abs (angleBackward)) {
					movingOnForwardDirection = true;
					movingOnBackwardDirection = false;

					targetPosition = getClosestPointToLine (playerPosition, currentWayPoints [currentWaypointIndex].position, currentWayPoints [nextWaypointIndex].position);
				} else {
					movingOnForwardDirection = false;
					movingOnBackwardDirection = true;

					targetPosition = getClosestPointToLine (playerPosition, currentWayPoints [currentWaypointIndex].position, currentWayPoints [previousWaypointIndex].position);
				}

				if (showDebugPrint) {
					print ("3 " + movingOnForwardDirection + " " + movingOnBackwardDirection);
				}
			}
				
			adjustPlayerToMovementOnStart (targetPosition);

			nextTargetWaypointIndex = currentWaypointIndex;

			if (movingOnForwardDirection) {
				nextTargetWaypointIndex++;

				if (nextTargetWaypointIndex >= currentWayPoints.Count - 1) {
					nextTargetWaypointIndex = currentWayPoints.Count - 1;
				}

				movementDirectionMultiplier = 1;
			} else {
				nextTargetWaypointIndex--;

				if (nextTargetWaypointIndex < 0) {
					nextTargetWaypointIndex = 0;
				}

				movementDirectionMultiplier = -1;
			}

			if (showDebugPrint) {
				print (nextTargetWaypointIndex);
			}
		} else {
			stopAdjustPlayerToMovementOnStartCoroutine ();
		}

		if (state) {
			mainAnimator.SetInteger (actionIDAnimatorID, actionID);

			mainAnimator.SetBool (externalControlleBehaviorActiveAnimatorID, state);
		} else {
			mainAnimator.SetBool (externalControlleBehaviorActiveAnimatorID, state);

			mainAnimator.SetInteger (actionIDAnimatorID, 0);
		}

		isFirstPersonActive = mainPlayerController.isPlayerOnFirstPerson ();

		if (setNewCameraStateOnThirdPerson && !isFirstPersonActive) {
			if (state) {
				previousCameraState = mainPlayerCamera.getCurrentStateName ();

				mainPlayerCamera.setCameraStateOnlyOnThirdPerson (newCameraStateOnThirdPerson);
			} else {

				if (previousCameraState != "") {
					if (previousCameraState != newCameraStateOnThirdPerson) {
						mainPlayerCamera.setCameraStateOnlyOnThirdPerson (previousCameraState);
					}

					previousCameraState = "";
				}
			}
		}

		if (turnAndForwardAnimatorValuesPaused) {
			mainPlayerController.setTurnAndForwardAnimatorValuesPausedState (false);

			turnAndForwardAnimatorValuesPaused = false;
		}

		lastTimeReverseMovementDirectionInput = 0;

		targetRotation = 0;

		lastTimeMoving = 0;

		lastTimeNotMoving = Time.time;

		if (setPlayerAsChildActive) {
			if (state) {
				if (previousParent == null) {
					previousParent	= playerTransform.parent;
				}

				mainPlayerController.setPlayerAndCameraParent (playerParentTransform);
			} else {
				if (previousParent != null) {
					mainPlayerController.setPlayerAndCameraParent (previousParent);
				} else {
					mainPlayerController.setPlayerAndCameraParent (null);
				}

				previousParent = null;
			}
		}
	}

	public override void setExternalForceEnabledState (bool state)
	{
		setMovementEnabledState (state);
	}

	public void setMovementEnabledState (bool state)
	{
		if (!state) {
			setMovementSystemActivestate (false);
		}

		systemEnabled = state;
	}

	public void enableOrDisableTurbo (bool state)
	{
		if (turboActive == state) {
			return;
		}

		if (state && canUseTurboPausedState) {
			return;
		}

		turboActive = state;

		if (useEventUseTurbo) {
			if (state) {
				eventOnStarTurbo.Invoke ();
			} else {
				eventOnEndTurbo.Invoke ();
			}
		}

		if (!turboActive) {
			mainPlayerController.stopShakeCamera ();
		}
	}

	public void setCanUseTurboPausedState (bool state)
	{
		if (!state && turboActive) {
			enableOrDisableTurbo (false);
		}

		canUseTurboPausedState = state;
	}

	public override void disableExternalControllerState ()
	{
		setMovementSystemActivestate (false);
	}

	public void inputChangeTurboState (bool state)
	{
		if (movementSystemActive && turboEnabled) {
			enableOrDisableTurbo (state);
		}
	}

	public void setCurrentWaypoint (simpleWaypointSystem newSimpleWaypointSystem)
	{
		currentSimpleWaypointSystem = newSimpleWaypointSystem;

		if (currentSimpleWaypointSystem != null) {
			currentWayPoints = currentSimpleWaypointSystem.wayPoints;
		}
	}

	void stopMovement ()
	{
		Vector3 totalForce = Vector3.zero;

		totalForce = playerTransform.forward * endOfSurfaceImpulse.z + playerTransform.up * endOfSurfaceImpulse.y;

		setMovementSystemActivestate (false);

		if (!ignoreJumpActive && jumpOnEndState && jumpOnEndStateEnabled) {
			setImpulseForce (totalForce, false);
		}

		ignoreJumpActive = false;
	}

	public void adjustPlayerToMovementOnStart (Vector3 initialPosition)
	{
		stopResetPlayerRotationCoroutine ();

		adjustPlayerToMovementCoroutine = StartCoroutine (adjustPlayerToMovementOnStartCoroutine (initialPosition));
	}

	void stopAdjustPlayerToMovementOnStartCoroutine ()
	{
		if (adjustPlayerToMovementCoroutine != null) {
			StopCoroutine (adjustPlayerToMovementCoroutine);
		}

		adjustingPlayerToMovement = false;
	}

	IEnumerator adjustPlayerToMovementOnStartCoroutine (Vector3 initialPosition)
	{
		adjustingPlayerToMovement = true;

		float t = 0;

		Vector3 pos = initialPosition;

		if (setPlayerAsChildActive) {
			pos = playerParentTransform.InverseTransformPoint (pos);
		}

		float dist = 0;

		if (setPlayerAsChildActive) {
			dist = GKC_Utils.distance (playerTransform.localPosition, pos);
		} else {
			dist = GKC_Utils.distance (playerTransform.position, pos);
		}

		float duration = dist / adjustPlayerToMovementSpeed;

		float movementTimer = 0;

		float positionDifference = 0;

		bool targetReached = false;

		while (!targetReached) {
			t += Time.deltaTime / duration;

			if (setPlayerAsChildActive) {
				playerTransform.localPosition = Vector3.Slerp (playerTransform.localPosition, pos, t);
			} else {
				playerTransform.position = Vector3.Slerp (playerTransform.position, pos, t);
			}

			movementTimer += Time.deltaTime;

			if (setPlayerAsChildActive) {
				positionDifference = GKC_Utils.distance (playerTransform.localPosition, pos);
			} else {
				positionDifference = GKC_Utils.distance (playerTransform.position, pos);
			}

			if (positionDifference < 0.01f || movementTimer > (duration + 1)) {
				targetReached = true;
			}

			yield return null;
		}

		adjustingPlayerToMovement = false;
	}

	public void resetPlayerRotation ()
	{
		stopResetPlayerRotationCoroutine ();

		resetPlayerCoroutine = StartCoroutine (resetPlayerRotationCoroutine ());
	}

	void stopResetPlayerRotationCoroutine ()
	{
		if (resetPlayerCoroutine != null) {
			StopCoroutine (resetPlayerCoroutine);
		}
	}

	IEnumerator resetPlayerRotationCoroutine ()
	{
		float movementTimer = 0;

		float t = 0;

		float duration = 1;

		float angleDifference = 0;

		Vector3 currentNormal = mainPlayerController.getCurrentNormal ();

		Quaternion currentPlayerRotation = playerTransform.rotation;
		Vector3 currentPlayerForward = Vector3.Cross (playerTransform.right, currentNormal);
		Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, currentNormal);

		bool targetReached = false;

		while (!targetReached) {
			t += (Time.deltaTime / duration) * resetPlayerRotationSpeed;

			playerTransform.rotation = Quaternion.Slerp (playerTransform.rotation, playerTargetRotation, t);

			angleDifference = Quaternion.Angle (playerTransform.rotation, playerTargetRotation);

			movementTimer += Time.deltaTime;

			if (angleDifference < 0.01f || movementTimer > (duration + 1)) {
				targetReached = true;
			}

			yield return null;
		}
	}

	public Vector3 getClosestPosition (Vector3 position)
	{
		float minDistance = Mathf.Infinity;

		int index = -1;

		for (int i = 0; i < currentWayPoints.Count; i++) {
			float distance = GKC_Utils.distance (position, currentWayPoints [i].position);

			if (distance < minDistance) {
				minDistance = distance;

				index = i;
			}
		}

		currentWaypointIndex = index;

		if (showDebugPrint) {
			print (currentWaypointIndex);
		}

		if (index > -1) {
			return currentWayPoints [index].position;
		} else {
			return Vector3.zero;
		}
	}


	Vector3 getClosestPointToLine (Vector3 point, Vector3 line_start, Vector3 line_end)
	{
		Vector3 line_direction = line_end - line_start;
		float line_length = line_direction.magnitude;
		line_direction.Normalize ();
		float project_length = Mathf.Clamp (Vector3.Dot (point - line_start, line_direction), 0f, line_length);

		return line_start + line_direction * project_length;
	}

	public override void setCurrentPlayerActionSystemCustomActionCategoryID ()
	{
		if (behaviorCurrentlyActive) {
			if (customActionCategoryID > -1) {
				mainPlayerController.setCurrentCustomActionCategoryID (customActionCategoryID);
			}
		} else {
			if (regularActionCategoryID > -1) {
				mainPlayerController.setCurrentCustomActionCategoryID (regularActionCategoryID);
			}
		}
	}

	public void setSetPlayerAsChildStateState (bool state, Transform newPlayerParentTransform)
	{
		setPlayerAsChildActive = state;

		playerParentTransform = newPlayerParentTransform;
	}

	public void setJumpOnEndState (bool state)
	{
		jumpOnEndState = state;
	}
}