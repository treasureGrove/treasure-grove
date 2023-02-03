using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class climbRopeSystem : externalControllerBehavior
{
	[Header ("Main Settings")]
	[Space]

	public bool systemEnabled = true;

	public float lookDirectionSpeed;

	public float resetPlayerRotationSpeed = 5;

	public float adjustPlayerToMovementSpeed = 5;

	public float movementUpSpeed = 5;
	public float movementUpTurboSpeed = 10;
	public float movementDownSpeed = 10;
	public float movementDownTurboSpeed = 15;

	public float movementAmountMultiplierUp = 1;
	public float movementAmountMultiplierDown = 1;

	public float jumpRotationSpeedThirdPerson = 1;
	public float jumpRotationSpeedFirstPerson = 0.5f;

	public bool stopClimbOnTopReached;
	public bool stopCLimbOnBottomReached;

	public bool checkClimbRopeTriggerMatchSystemListOnJump;
	public float minAngleWithClimbRopeTriggerMatchSystem;

	public float minDistanceToReachTopOrBottom = 0.18f;

	[Space]
	[Header ("Physics Settings")]
	[Space]

	public bool useForceMode;

	public ForceMode railForceMode;

	public bool turboEnabled;

	public float speedOnAimMultiplier;

	public bool setMainColliderAsTriggerOnStateActive;

	[Space]
	[Header ("Jump Settings")]
	[Space]

	public bool canJumpEnabled;

	public Vector3 impulseOnJump;
	public Vector3 endOfSurfaceTopImpulse;
	public Vector3 endOfSurfaceBottomImpulse;

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
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool movementPausedDebug;

	public bool movementSystemActive;

	public bool turboActive;

	public Vector3 targetDirection;

	public bool adjustingPlayerToMovement;

	public bool movingOnForwardDirection;
	public bool movingOnBackwardDirection;

	public float verticalInput;
	public float horizontalInput;

	public float movementDirectionMultiplier = 1;

	public bool movingUp;
	public bool movingDown;
	public bool rotatingHorizontally;

	public bool setPlayerAsChildActive;

	public Transform playerParentTransform;

	public Transform bottomTransform;
	public Transform topTransform;

	public List<climbRopeTriggerMatchSystem> climbRopeTriggerMatchSystemList = new List<climbRopeTriggerMatchSystem> ();

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

	float targetRotation;

	Vector3 currentTargetDirection;

	bool jumpActivated;

	bool topReached;

	Transform previousParent;

	climbRopeTriggerSystem currentClimbRopeTriggerSystem;

	Vector3 currentDirectionToRotatePlayer;

	int originalActionID;


	void Start ()
	{
		externalControlleBehaviorActiveAnimatorID = Animator.StringToHash (externalControlleBehaviorActiveAnimatorName);

		actionIDAnimatorID = Animator.StringToHash (actionIDAnimatorName);

		horizontalAnimatorID = Animator.StringToHash (horizontalAnimatorName);

		originalActionID = actionID;
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

				horizontalInput = mainPlayerController.getRawAxisValues ().x;

				verticalInput *= movementDirectionMultiplier;

				movingUp = false;

				movingDown = false;

				rotatingHorizontally = false;

				topReached = false;
			

				if (verticalInput != 0) {
					if (verticalInput > 0) {
						slidingAnimatorValue = 1;

						movingUp = true;
					} else {
						slidingAnimatorValue = -1;

						movingDown = true;
					}
				}


				if (horizontalInput != 0) {
					rotatingHorizontally = true;
				}

				if (movingUp) {
					float distanceToTop = GKC_Utils.distance (playerTransform.position, topTransform.position);

					if (distanceToTop < minDistanceToReachTopOrBottom) {
						movingUp = false;

						slidingAnimatorValue = 0;

						if (stopClimbOnTopReached) {
							targetReached = true;

							topReached = true;
						}
					}
				} 

				if (movingDown) {
					float distanceToBottom = GKC_Utils.distance (playerTransform.position, bottomTransform.position);

					if (distanceToBottom < minDistanceToReachTopOrBottom) {
						movingDown = false;

						slidingAnimatorValue = 0;

						if (stopCLimbOnBottomReached) {
							targetReached = true;
						}
					}
				}
	
				float currentSpeed = movementUpSpeed;

				if (turboActive) {
					if (movingUp) {
						currentSpeed = movementUpTurboSpeed;
					}

					if (movingDown) {
						currentSpeed = movementDownTurboSpeed;
					}

					slidingAnimatorValue *= 2;
				} else {
					if (movingDown) {
						currentSpeed = movementDownSpeed;
					}
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

				Vector3 targetPosition = playerTransform.position;

				if (movingUp) {
					targetPosition += playerTransform.up * movementAmountMultiplierUp;
				} else if (movingDown) {
					targetPosition -= playerTransform.up * movementAmountMultiplierDown;
				}

				if (movingUp || movingDown) {
					playerTransform.position = Vector3.MoveTowards (playerTransform.position, targetPosition, Time.deltaTime * currentSpeed);
				}

				float angle = 0;

				if (rotatingHorizontally) {
					angle = horizontalInput;
				}

				targetRotation = Mathf.Lerp (targetRotation, angle, lookDirectionSpeed * Time.fixedDeltaTime);

				if (Mathf.Abs (targetRotation) > 0.001f) {
					playerTransform.Rotate (0, targetRotation * 4, 0);
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
			jumpActivated = true;

			setMovementSystemActivestate (false);
		
			currentDirectionToRotatePlayer = -playerTransform.forward;

			Vector3 jumpDirection = playerTransform.forward;
	
			if (isFirstPersonActive) {
				float angleWithCamera = Vector3.SignedAngle (playerTransform.forward, mainPlayerCamera.transform.forward, playerTransform.up);

				if (Mathf.Abs (angleWithCamera) > 45) {
					currentDirectionToRotatePlayer = mainPlayerCamera.transform.forward;
				}
			} 

			if (checkClimbRopeTriggerMatchSystemListOnJump) {
				int closestMatchSystemIndex = -1;

				float minAngle = Mathf.Infinity;

				Vector3 playerPosition = playerTransform.position;

				for (int i = 0; i < climbRopeTriggerMatchSystemList.Count; i++) {
					if (climbRopeTriggerMatchSystemList [i].mainClimbRopeTriggerSystem != currentClimbRopeTriggerSystem) {

						Vector3 lookDirection = climbRopeTriggerMatchSystemList [i].transform.position - playerPosition;

						lookDirection = new Vector3 (lookDirection.x, 0, lookDirection.z);

						lookDirection = lookDirection / lookDirection.magnitude;

						float angle = Vector3.SignedAngle (lookDirection, -playerTransform.forward, playerTransform.up);

						float ABSAngle = Mathf.Abs (angle);

//						print (ABSAngle + " " + i);

						if (ABSAngle < minAngle && ABSAngle < minAngleWithClimbRopeTriggerMatchSystem) {
							minAngle = ABSAngle;

							closestMatchSystemIndex = i;
						}
					}
				}

				if (closestMatchSystemIndex > -1) {
					if (showDebugPrint) {
						print ("jumping to trigger match with index " + closestMatchSystemIndex);
					}

					Vector3 lookDirection = playerPosition - climbRopeTriggerMatchSystemList [closestMatchSystemIndex].transform.position;

					lookDirection = new Vector3 (lookDirection.x, 0, lookDirection.z);

					lookDirection = lookDirection / lookDirection.magnitude;

					currentDirectionToRotatePlayer = -lookDirection;

					Debug.DrawRay (playerTransform.position, -lookDirection * 20, Color.blue, 5);

					jumpDirection = lookDirection;
				}
			}

			
			Vector3 totalForce = newImpulseOnJumpAmount.y * playerTransform.up + newImpulseOnJumpAmount.z * jumpDirection;
			
			mainPlayerController.useJumpPlatform (totalForce, ForceMode.Impulse);


			rotateCharacterOnJump ();
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

		} else {
			mainPlayerController.setCheckOnGroungPausedState (false);

			mainPlayerController.setPlayerOnGroundState (false);

			mainPlayerController.setPlayerOnGroundAnimatorStateOnOverrideOnGroundWithTime (true);

			mainPlayerController.disableOverrideOnGroundAnimatorValue ();

			mainPlayerController.setPauseResetAnimatorStateFOrGroundAnimatorState (true);

			mainPlayerController.setOnGroundAnimatorIDValue (false);
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

				if (!jumpActivated) {
					if (playerTransform.up != mainPlayerController.getCurrentNormal ()) {
						resetPlayerRotation ();
					}
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

			Vector3 targetPosition = getClosestPointToLine (playerPosition, topTransform.position, bottomTransform.position);

			targetPosition = getClosestPointToLine (targetPosition, topTransform.position, bottomTransform.position);

			Vector3 movementDirection = topTransform.position - bottomTransform.position;
		
			movementDirection = movementDirection / movementDirection.magnitude;

			adjustPlayerToMovementOnStart (targetPosition, movementDirection);
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

		targetRotation = 0;

		jumpActivated = false;

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

		if (setMainColliderAsTriggerOnStateActive) {
			mainPlayerController.setMainColliderTriggerState (state);
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

	public void setTransformElements (Transform newBottomTransform, Transform newTopTransform)
	{
		bottomTransform = newBottomTransform;

		topTransform = newTopTransform;
	}

	public void setSetPlayerAsChildStateState (bool state, Transform newPlayerParentTransform)
	{
		setPlayerAsChildActive = state;

		playerParentTransform = newPlayerParentTransform;
	}

	public void setCurrentClimbRopeTriggerSystem (climbRopeTriggerSystem newClimbRopeTriggerSystem)
	{
		currentClimbRopeTriggerSystem = newClimbRopeTriggerSystem;
	}

	void stopMovement ()
	{
		Vector3 totalForce = Vector3.zero;

		if (topReached) {
			totalForce = playerTransform.forward * endOfSurfaceTopImpulse.z + playerTransform.up * endOfSurfaceTopImpulse.y;
		} else {
			totalForce = playerTransform.forward * endOfSurfaceBottomImpulse.z + playerTransform.up * endOfSurfaceBottomImpulse.y;
		}

		setMovementSystemActivestate (false);

		setImpulseForce (totalForce, false);
	}

	public void adjustPlayerToMovementOnStart (Vector3 initialPosition, Vector3 movementDirection)
	{
		stopResetPlayerRotationCoroutine ();

		adjustPlayerToMovementCoroutine = StartCoroutine (adjustPlayerToMovementOnStartCoroutine (initialPosition, movementDirection));
	}

	void stopAdjustPlayerToMovementOnStartCoroutine ()
	{
		if (adjustPlayerToMovementCoroutine != null) {
			StopCoroutine (adjustPlayerToMovementCoroutine);
		}

		adjustingPlayerToMovement = false;
	}

	IEnumerator adjustPlayerToMovementOnStartCoroutine (Vector3 initialPosition, Vector3 movementDirection)
	{
		adjustingPlayerToMovement = true;

		float t = 0;

		Vector3 pos = initialPosition;

		if (setPlayerAsChildActive) {
			pos = playerParentTransform.InverseTransformPoint (pos);

			pos = new Vector3 (0, pos.y, 0);
		}

		float dist = 0;

		if (setPlayerAsChildActive) {
			dist = GKC_Utils.distance (playerTransform.localPosition, pos);
		} else {
			dist = GKC_Utils.distance (playerTransform.position, pos);
		}

		float duration = dist / adjustPlayerToMovementSpeed;

		float movementTimer = 0;

		bool targetReached = false;

		float angleDifference = 0;

		float positionDifference = 0;

		Quaternion currentPlayerRotation = playerTransform.rotation;
		Vector3 currentPlayerForward = Vector3.Cross (playerTransform.right, movementDirection);
		Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, movementDirection);

		while (!targetReached) {
			t += Time.deltaTime / duration;

			if (setPlayerAsChildActive) {
				playerTransform.localPosition = Vector3.Lerp (playerTransform.localPosition, pos, t);
			} else {
				playerTransform.position = Vector3.Lerp (playerTransform.position, pos, t);
			}

			playerTransform.rotation = Quaternion.Slerp (playerTransform.rotation, playerTargetRotation, t);

			angleDifference = Quaternion.Angle (playerTransform.rotation, playerTargetRotation);

			movementTimer += Time.deltaTime;

			if (setPlayerAsChildActive) {
				positionDifference = GKC_Utils.distance (playerTransform.localPosition, pos);
			} else {
				positionDifference = GKC_Utils.distance (playerTransform.position, pos);
			}

			if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
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

	Vector3 getClosestPointToLine (Vector3 point, Vector3 line_start, Vector3 line_end)
	{
		Vector3 line_direction = line_end - line_start;
		float line_length = line_direction.magnitude;
		line_direction.Normalize ();
		float project_length = Mathf.Clamp (Vector3.Dot (point - line_start, line_direction), 0f, line_length);

		return line_start + line_direction * project_length;
	}

	Coroutine jumpCoroutine;

	public void rotateCharacterOnJump ()
	{
		stopRotateCharacterOnJumpCoroutine ();

		jumpCoroutine = StartCoroutine (rotateCharacterOnJumpCoroutine ());
	}

	void stopRotateCharacterOnJumpCoroutine ()
	{
		if (jumpCoroutine != null) {
			StopCoroutine (jumpCoroutine);
		}
	}

	public IEnumerator rotateCharacterOnJumpCoroutine ()
	{
		bool targetReached = false;

		float movementTimer = 0;

		float t = 0;

		float duration = 0;

		if (isFirstPersonActive) {
			duration = 0.5f / jumpRotationSpeedFirstPerson;
		} else {
			duration = 0.5f / jumpRotationSpeedThirdPerson;
		}

		float angleDifference = 0;

		Transform objectToRotate = playerTransform;

		if (isFirstPersonActive) {
			objectToRotate = mainPlayerCamera.transform;
		}

		Quaternion targetRotation = Quaternion.LookRotation (currentDirectionToRotatePlayer, objectToRotate.up);

		while (!targetReached) {
			t += Time.deltaTime / duration; 

			objectToRotate.rotation = Quaternion.Slerp (objectToRotate.rotation, targetRotation, t);

			angleDifference = Quaternion.Angle (objectToRotate.rotation, targetRotation);

			movementTimer += Time.deltaTime;

			if (angleDifference < 0.2f || movementTimer > (duration + 1)) {
				targetReached = true;
			}
			yield return null;
		}

		if (playerTransform.up != mainPlayerController.getCurrentNormal ()) {
			resetPlayerRotation ();
		}
	}

	public void addClimbRopeTriggerMatchSystem (climbRopeTriggerMatchSystem newClimbRopeTriggerMatchSystem)
	{
		if (!climbRopeTriggerMatchSystemList.Contains (newClimbRopeTriggerMatchSystem)) {
			climbRopeTriggerMatchSystemList.Add (newClimbRopeTriggerMatchSystem);
		}
	}

	public void removeClimbRopeTriggerMatchSystem (climbRopeTriggerMatchSystem newClimbRopeTriggerMatchSystem)
	{
		if (climbRopeTriggerMatchSystemList.Contains (newClimbRopeTriggerMatchSystem)) {
			climbRopeTriggerMatchSystemList.Remove (newClimbRopeTriggerMatchSystem);
		}
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

	public void setNewClimbActionID (int newValue)
	{
		actionID = newValue;
	}

	public void setOriginalClimbActionID ()
	{
		setNewClimbActionID (originalActionID);
	}
}