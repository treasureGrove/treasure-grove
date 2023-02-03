using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class railSystem : externalControllerBehavior
{
	[Header ("Main Settings")]
	[Space]

	public bool railSystemEnabled = true;

	public float lookDirectionSpeed;

	public float resetPlayerRotationSpeed = 5;

	public float adjustPlayerToRailSpeed = 5;

	[Space]
	[Header ("Physics Settings")]
	[Space]

	public bool useForceMode;

	public ForceMode railForceMode;

	public bool turboEnabled;

	public float turboSpeed;

	public float speedOnAimMultiplier;

	[Space]
	[Header ("Speed Settings")]
	[Space]

	public bool adjustSpeedBasedOnInclination;
	public float speedMultiplierOnInclinationUp;
	public float speedMultiplierOnInclinationDown;
	public float minInclinationDifferenceToAffectSpeed;

	public float speedMultiplierOnInclinationLerp;

	//	public float aimRotationSpeed = 5;

	[Space]
	[Header ("Jump Settings")]
	[Space]

	public bool canJumpEnabled;

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
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool movementOnRailPausedDebug;

	public bool railSystemActive;

	public float bezierDuration;

	public bool turboActive;

	public Vector3 targetDirection;

	public BezierSpline currentSpline;

	public bool movingForward;

	public bool adjustingPlayerToRail;

	public float progress = 0;
	public float progressTarget = 0;

	public bool playerAiming;

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

	Coroutine adjustPlayerToRailCoroutine;

	Coroutine resetPlayerCoroutine;

	Vector3 lookDirection;

	float currentInclinationSpeed;

	bool resetCameraShakeActive;


	void Start ()
	{
		externalControlleBehaviorActiveAnimatorID = Animator.StringToHash (externalControlleBehaviorActiveAnimatorName);

		actionIDAnimatorID = Animator.StringToHash (actionIDAnimatorName);

		horizontalAnimatorID = Animator.StringToHash (horizontalAnimatorName);
	}

	public override void updateControllerBehavior ()
	{
		if (railSystemActive && !movementOnRailPausedDebug) {
			if (!adjustingPlayerToRail) {
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

				float currentSpeed = bezierDuration;

				if (turboActive) {
					slidingAnimatorValue = 1;

					currentSpeed /= turboSpeed;
				}

				if (adjustSpeedBasedOnInclination) {

					lookDirection = currentSpline.GetDirection (progress);

					Vector3 currentNormal = mainPlayerController.getCurrentNormal ();

					float angle = Vector3.SignedAngle (playerTransform.up, currentNormal, playerTransform.right);

					if (Mathf.Abs (angle) > minInclinationDifferenceToAffectSpeed) {
						if (angle < 0) {
							currentInclinationSpeed = Mathf.Lerp (currentInclinationSpeed, speedMultiplierOnInclinationDown, Time.deltaTime * speedMultiplierOnInclinationLerp);
						} else {
							currentInclinationSpeed = Mathf.Lerp (currentInclinationSpeed, speedMultiplierOnInclinationUp, Time.deltaTime * speedMultiplierOnInclinationLerp);
						}
					} else {
						currentInclinationSpeed = Mathf.Lerp (currentInclinationSpeed, 1, Time.deltaTime * speedMultiplierOnInclinationLerp);
					}

					currentSpeed *= currentInclinationSpeed;
				}

				mainAnimator.SetFloat (horizontalAnimatorID, slidingAnimatorValue, inputLerpSpeed, Time.fixedDeltaTime);


				bool canRotatePlayerResult = true;

				currentAimSpeedMultipler = 1;


				if (!mainPlayerController.isPlayerOnFirstPerson () &&
				    (mainPlayerController.isPlayerAiming () || mainPlayerController.isUsingFreeFireMode ()) &&
				    !mainPlayerController.isCheckToKeepWeaponAfterAimingWeaponFromShooting ()) {

					currentAimSpeedMultipler = speedOnAimMultiplier;
			
					if (!turnAndForwardAnimatorValuesPaused) {
						mainPlayerController.setTurnAndForwardAnimatorValuesPausedState (true);
			
						mainPlayerController.setRotateInCameraDirectionOnAirOnExternalActionActiveState (true);

						turnAndForwardAnimatorValuesPaused = true;
					}

					canRotatePlayerResult = false;

					if (Time.time > mainPlayerController.getLastTimeFiring () + 0.2f) {
						if (resetCameraShakeActive) {
							mainPlayerCamera.stopAllHeadbobMovements ();
						
							resetCameraShakeActive = false;
						}
					} else {
						resetCameraShakeActive = true;
					}
	
				} else {
					if (turnAndForwardAnimatorValuesPaused) {
						mainPlayerController.setTurnAndForwardAnimatorValuesPausedState (false);

						mainPlayerController.setRotateInCameraDirectionOnAirOnExternalActionActiveState (false);
			
						turnAndForwardAnimatorValuesPaused = false;

						mainPlayerCamera.stopAllHeadbobMovements ();
					}
				}


				float currentProgress = Time.deltaTime / (currentSpeed * currentAimSpeedMultipler);
		
				if (movingForward) {
					progress += currentProgress;
				} else {
					progress -= currentProgress;
				}

				Vector3 position = currentSpline.GetPoint (progress);
				playerTransform.position = position;

				lookDirection = currentSpline.GetDirection (progress);

				if (!movingForward) {
					lookDirection = -lookDirection;
				}


				if (canRotatePlayerResult) {
					Quaternion targetRotation = Quaternion.LookRotation (lookDirection);

					playerTransform.rotation = Quaternion.Lerp (playerTransform.rotation, targetRotation, Time.fixedDeltaTime * lookDirectionSpeed);
				}

				if (movingForward) {
					if (progress > progressTarget) {
						targetReached = true;
					}
				} else {
					if (progress < progressTarget) {
						targetReached = true;
					}
				}

				if (targetReached) {
					stopRail ();
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
		if (railSystemActive) {
			setRailSystemActivestate (false);

			Vector3 totalForce = newImpulseOnJumpAmount.y * playerTransform.up + newImpulseOnJumpAmount.z * playerTransform.forward;

			mainPlayerController.useJumpPlatform (totalForce, ForceMode.Impulse);
		}
	}

	public override void setExternalForceActiveState (bool state)
	{
		setRailSystemActivestate (state);
	}

	public void setRailSystemActivestate (bool state)
	{
		if (!railSystemEnabled) {
			return;
		}
			
		if (railSystemActive == state) {
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

		bool modeActivePrevioulsy = railSystemActive;

		railSystemActive = state;

		setBehaviorCurrentlyActiveState (state);

		setCurrentPlayerActionSystemCustomActionCategoryID ();

		if (showDebugPrint) {
			print ("Setting state as " + railSystemActive);
		}

		mainPlayerController.setExternalControlBehaviorForAirTypeActiveState (state);

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

		if (railSystemActive) {
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

		if (railSystemActive) {
			eventOnStateEnabled.Invoke ();
		} else {
			eventOnStateDisabled.Invoke ();

			if (turboActive) {
				enableOrDisableTurbo (false);
			}
		}

		mainHeadTrack.setExternalHeadTrackPauseActiveState (state);

		mainPlayerController.stopShakeCamera ();

		mainPlayerCamera.setPausePlayerCameraViewChangeState (state);

		mainPlayerController.setLastTimeFalling ();

		if (state) {
			progress = 0;

			progressTarget = 1;

			Vector3 playerPosition = currentSpline.FindNearestPointTo (playerTransform.position, 100, 30);

			adjustPlayerToRailOnStart (playerPosition);

			float step = currentSpline.AccuracyToStepSize (100);

			float minDistance = Mathf.Infinity;

			for (float i = 0f; i < 1f; i += step) {
				Vector3 thisPoint = currentSpline.GetPoint (i);
				float thisDistance = (playerPosition - thisPoint).sqrMagnitude;

				if (thisDistance < minDistance) {
					minDistance = thisDistance;
					progress = i;
				}
			}

			lookDirection = currentSpline.GetDirection (progress);

			float angle = Vector3.SignedAngle (playerTransform.forward, lookDirection, playerTransform.up);

			movingForward = false;

			if (Mathf.Abs (angle) < 90) {
				movingForward = true;
			} 
			
			if (showDebugPrint) {
				print (movingForward + " " + progress + "  " + angle);
			}
				
			if (!movingForward) {
				progressTarget = 0;
			}
		} else {
			stopAdjustPlayerToRailOnStartCoroutine ();
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

			mainPlayerController.setRotateInCameraDirectionOnAirOnExternalActionActiveState (false);

			turnAndForwardAnimatorValuesPaused = false;
		}

		currentInclinationSpeed = 1;

		playerAiming = false;

		resetCameraShakeActive = false;
	}

	public override void setExternalForceEnabledState (bool state)
	{
		setRailEnabledState (state);
	}

	public void setRailEnabledState (bool state)
	{
		if (!state) {
			setRailSystemActivestate (false);
		}

		railSystemEnabled = state;
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
		setRailSystemActivestate (false);
	}

	public void inputChangeTurboState (bool state)
	{
		if (railSystemActive && turboEnabled) {
			enableOrDisableTurbo (state);
		}
	}

	public void setCurrentSpline (BezierSpline newSpline)
	{
		currentSpline = newSpline;
	}

	public void setCurrentBezierDuration (float newValue)
	{
		bezierDuration = newValue;
	}

	void stopRail ()
	{
		Vector3 totalForce = Vector3.zero;

		totalForce = playerTransform.forward * endOfSurfaceImpulse.z + playerTransform.up * endOfSurfaceImpulse.y;

		setRailSystemActivestate (false);

		setImpulseForce (totalForce, false);
	}

	public void adjustPlayerToRailOnStart (Vector3 initialPosition)
	{
		stopResetPlayerRotationCoroutine ();

		adjustPlayerToRailCoroutine = StartCoroutine (adjustPlayerToRailOnStartCoroutine (initialPosition));
	}

	void stopAdjustPlayerToRailOnStartCoroutine ()
	{
		if (adjustPlayerToRailCoroutine != null) {
			StopCoroutine (adjustPlayerToRailCoroutine);
		}

		adjustingPlayerToRail = false;
	}

	IEnumerator adjustPlayerToRailOnStartCoroutine (Vector3 initialPosition)
	{
		adjustingPlayerToRail = true;

		float dist = GKC_Utils.distance (playerTransform.position, initialPosition);

		float duration = dist / adjustPlayerToRailSpeed;

		float t = 0;

		Vector3 pos = initialPosition;

		float movementTimer = 0;

		bool targetReached = false;

		while (!targetReached) {
			t += Time.deltaTime / duration;

			playerTransform.position = Vector3.Slerp (playerTransform.position, pos, t);

			movementTimer += Time.deltaTime;

			if (GKC_Utils.distance (playerTransform.position, pos) < 0.01f || movementTimer > (duration + 1)) {
				targetReached = true;
			}

			yield return null;
		}

		adjustingPlayerToRail = false;
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
}