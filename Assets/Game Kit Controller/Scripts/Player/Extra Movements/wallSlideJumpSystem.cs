using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class wallSlideJumpSystem : externalControllerBehavior
{
	[Header ("Main Settings")]
	[Space]

	public bool slideEnabled = true;

	public LayerMask raycastLayermask;

	public float raycastDistance = 0.6f;

	public float slideActiveRaycastDistance = 3;

	public float adhereToWallSpeed = 5;

	public float jumpRotationSpeedThirdPerson = 1;
	public float jumpRotationSpeedFirstPerson = 0.5f;

	public float minAngleToAdhereToWall = 20;
	public float downRaycastDistanceToCheckToAdhereToWall = 3;

	public float minWaitTimeToAdhereAgain = 0.3f;

	[Space]
	[Header ("Slide Down Settings")]
	[Space]

	public bool slideDownOnWall;
	public float slideDownOnWallSpeed;

	[Space]

	public float waitTimeToStartToSlideDown;
	public bool stopSlideDownAfterTime;
	public float timeToStopSlideDown;

	[Space]

	public bool disableSlideOnWallAfterTime;
	public float timeToDisableSlideDown;

	[Space]

	public bool slideDownIfInputNotPressed;

	public bool disableSlideIfInputPressedDown;

	public bool useInputPressUpToAdhereToSurface;

	[Space]
	[Header ("Speed Settings")]
	[Space]

	public float slideRotationSpeedThirdPerson = 200;
	public float slideRotationSpeedFirstPerson = 100;

	public Vector3 impulseOnJump;

	public float maxVelocityChangeSlide;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool sliderCanBeUsed = true;

	public bool keepWeapons;
	public bool drawWeaponsIfCarriedPreviously;

	[Space]
	[Header ("Third Person Settings")]
	[Space]

	public int actionID = 08632946;

	public string externalControlleBehaviorActiveAnimatorName = "External Behavior Active";
	public string actionIDAnimatorName = "Action ID";

	public string horizontalAnimatorName = "Horizontal Action";

	public float inputLerpSpeed = 3;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool checkIfDetectSlideActive;

	public bool slideActive;

	public bool isFirstPersonActive;

	public float currentVerticalMovement;

	public float currentHorizontalMovement;

	public float slideDownSpeedMultiplier = 1;

	public bool forceSlowDownOnSurfaceActive;

	public bool carryingWeaponsPreviously;

	public bool slideDownInProcess;

	public bool wallJumpSlidePaused;

	[Space]
	[Header ("First Person Events Settings")]
	[Space]

	public bool useEventsOnFirstPerson;

	public UnityEvent eventOnStartFirstPerson;
	public UnityEvent eventOnEndFirstPerson;

	[Space]
	[Header ("Third Person Events Settings")]
	[Space]

	public bool useEventsOnThirdPerson;

	public UnityEvent eventOnStartThirdPerson;
	public UnityEvent eventOnEndThirdPerson;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;
	public Transform playerTransform;
	public Rigidbody mainRigidbody;
	public Transform playerCameraTransform;
	public playerCamera mainPlayerCamera;
	public playerWeaponsManager mainPlayerWeaponsManager;

	public Animator mainAnimator;

	public headTrack mainHeadTrack;

	bool originalSlideEnabled;

	Vector3 playerTransformUp;
	Vector3 playerTransformForward;

	RaycastHit hit;

	Vector3 velocityChange;

	int externalControlleBehaviorActiveAnimatorID;

	int actionIDAnimatorID;

	float targetRotation;

	float lastTimeSlideActive;

	float lastTimeInputPressed;

	bool resetAnimatorIDValue;

	int horizontalAnimatorID;

	bool jumpInputUsed;

	Vector3 adherePosition;

	Coroutine jumpCoroutine;

	Vector3 moveInput;

	Vector3 localMove;

	float currentSlideRotationSpeed;


	void Start ()
	{
		originalSlideEnabled = slideEnabled;

		externalControlleBehaviorActiveAnimatorID = Animator.StringToHash (externalControlleBehaviorActiveAnimatorName);
		actionIDAnimatorID = Animator.StringToHash (actionIDAnimatorName);

		horizontalAnimatorID = Animator.StringToHash (horizontalAnimatorName);
	}

	void updateInputValues ()
	{
		currentVerticalMovement = mainPlayerController.getVerticalInput ();
		currentHorizontalMovement = mainPlayerController.getHorizontalInput ();

		moveInput = playerCameraTransform.forward * (currentVerticalMovement) + playerCameraTransform.right * (currentHorizontalMovement);

		if (moveInput.magnitude > 1) {
			moveInput.Normalize ();
		}

		localMove = playerTransform.InverseTransformDirection (moveInput);
	}

	public override void updateControllerBehavior ()
	{
		if (slideActive) {
			if (resetAnimatorIDValue) {
				if (Time.time > lastTimeSlideActive + 0.3f) {
					mainAnimator.SetInteger (actionIDAnimatorID, 0);

					resetAnimatorIDValue = false;
				}
			}

			float currentFixedUpdateDeltaTime = mainPlayerController.getCurrentDeltaTime ();

			playerTransformUp = playerTransform.up;

			playerTransformForward = playerTransform.forward;

			Vector3 currentRaycastPosition = playerTransform.position + playerTransformUp;
			Vector3 currentRaycastDirection = playerTransformForward;

			float surfaceAngle = 0;

			if (Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, slideActiveRaycastDistance, raycastLayermask)) {
				surfaceAngle = Vector3.SignedAngle (-playerTransformForward, hit.normal, playerTransformUp);
			} else {
				setCheckIfDetectSlideActiveState (false);

				return;
			}

			updateInputValues ();

			int slidingDownAnimatorValue = 0;

			slideDownInProcess = false;

			float currentSlideDownSpeed = slideDownOnWallSpeed;

			if (slideDownOnWall || slideDownIfInputNotPressed) {
				if (Time.time > waitTimeToStartToSlideDown + lastTimeSlideActive &&
				    Time.time > waitTimeToStartToSlideDown + lastTimeInputPressed) {

					slideDownInProcess = true;

					slidingDownAnimatorValue = 1;

					if (slideDownIfInputNotPressed) {
						
						if (localMove.z > 0.001f) {
							slideDownInProcess = false;

							slidingDownAnimatorValue = 0;

							lastTimeInputPressed = Time.time;
						}
					}
				}

				if (stopSlideDownAfterTime) {
					if (Time.time > lastTimeSlideActive + timeToStopSlideDown) {
						slideDownInProcess = false;

						slidingDownAnimatorValue = 0;
					}
				}
			}

			if (forceSlowDownOnSurfaceActive) {
				slideDownInProcess = true;

				slidingDownAnimatorValue = 1;
			}

			if (disableSlideIfInputPressedDown) {

				if (localMove.z < -0.001f) {
					setSlideActiveState (false);

					return;
				}
			}

			if (disableSlideOnWallAfterTime) {
				if (Time.time > timeToDisableSlideDown + lastTimeSlideActive) {
					setCheckIfDetectSlideActiveState (false);

					return;
				}
			}

			mainAnimator.SetFloat (horizontalAnimatorID, slidingDownAnimatorValue, inputLerpSpeed, Time.fixedDeltaTime);

			if (slideDownInProcess) {
				currentRaycastPosition = playerTransform.position - playerTransformUp;
				currentRaycastDirection = playerTransformForward;

				if (Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, slideActiveRaycastDistance, raycastLayermask)) {
					adherePosition = hit.point + hit.normal * 0.1f;

					currentSlideDownSpeed *= slideDownSpeedMultiplier;
				
					mainRigidbody.position = Vector3.MoveTowards (mainRigidbody.position, adherePosition, currentFixedUpdateDeltaTime * currentSlideDownSpeed);
				}

				currentRaycastPosition = playerTransform.position;
				currentRaycastDirection = playerTransformUp;

				if (Physics.Raycast (currentRaycastPosition, -currentRaycastDirection, out hit, slideActiveRaycastDistance, raycastLayermask)) {
					setSlideActiveState (false);

					return;
				} 
			} else {
				if (GKC_Utils.distance (mainRigidbody.position, adherePosition) > 0.01f) {
					mainRigidbody.position = Vector3.MoveTowards (mainRigidbody.position, adherePosition, currentFixedUpdateDeltaTime * adhereToWallSpeed);
				}
			}
				
			mainPlayerController.setCurrentVelocityValue (mainRigidbody.velocity);

			currentSlideRotationSpeed = slideRotationSpeedThirdPerson;

			if (isFirstPersonActive) {
				currentSlideRotationSpeed = slideRotationSpeedFirstPerson;
			}

			targetRotation = Mathf.Lerp (targetRotation, surfaceAngle, currentSlideRotationSpeed);

			if (Mathf.Abs (targetRotation) > 0.001f) {
				if (isFirstPersonActive) {
					playerCameraTransform.Rotate (0, targetRotation * currentFixedUpdateDeltaTime, 0);
				} else {
					playerTransform.Rotate (0, targetRotation * currentFixedUpdateDeltaTime, 0);
				}
			}
		} else {
			if (checkIfDetectSlideActive) {
				if (slideEnabled && sliderCanBeUsed && !mainPlayerController.useFirstPersonPhysicsInThirdPersonActive) {
					if (!slideActive && !mainPlayerController.pauseAllPlayerDownForces && !mainPlayerController.ignoreExternalActionsActiveState) {
						if (!mainPlayerController.isPlayerOnGround ()) {

							updateInputValues ();

							if (disableSlideIfInputPressedDown) {
								if (localMove.z < -0.001f) {
									return;
								}
							}

							if (useInputPressUpToAdhereToSurface) {
								if (localMove.z <= 0.001f) {
									return;
								}
							}

							if (Time.time > lastTimeSlideActive + minWaitTimeToAdhereAgain) {
								playerTransformUp = playerTransform.up;

								playerTransformForward = playerTransform.forward;

								Vector3 currentRaycastPosition = playerTransform.position + playerTransformUp;
								Vector3 currentRaycastDirection = playerTransformForward;

								bool canAdhereToWall = true;

								if (Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, raycastDistance, raycastLayermask)) {
									float angle = Vector3.SignedAngle (-playerTransformForward, hit.normal, playerTransformUp);

									if (angle > minAngleToAdhereToWall) {
										canAdhereToWall = false;
									} else {
										adherePosition = hit.point + hit.normal * 0.1f + playerTransformUp;
									}
								} else {
									canAdhereToWall = false;
								}

								currentRaycastDirection = -playerTransformUp;

								if (Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, downRaycastDistanceToCheckToAdhereToWall, raycastLayermask)) {
									canAdhereToWall = false;
								}

								if (canAdhereToWall) {
									setSlideActiveState (true);
								}
							}
						}
					}
				}
			}
		}
	}

	public override void setExtraImpulseForce (Vector3 forceAmount, bool useCameraDirection)
	{
		setSlideImpulseForce (forceAmount, useCameraDirection);
	}

	public void setSlideImpulseForce (Vector3 forceAmount, bool useCameraDirection)
	{
		Vector3 impulseForce = forceAmount;

		if (maxVelocityChangeSlide > 0) {
			velocityChange = impulseForce - mainRigidbody.velocity;

			velocityChange = Vector3.ClampMagnitude (velocityChange, maxVelocityChangeSlide);

		} else {
			velocityChange = impulseForce;
		}

		mainPlayerController.setVelocityChangeValue (velocityChange);

		mainRigidbody.AddForce (velocityChange, ForceMode.VelocityChange);
	}

	public override void setJumpActiveForExternalForce ()
	{
		setJumpActive (impulseOnJump);
	}

	public void setJumpActive (Vector3 newImpulseOnJumpAmount)
	{
		if (slideActive) {
			jumpInputUsed = true;

			setSlideActiveState (false);

			Vector3 totalForce = newImpulseOnJumpAmount.y * playerTransform.up + newImpulseOnJumpAmount.z * playerTransform.forward;

			mainPlayerController.useJumpPlatform (totalForce, ForceMode.Impulse);

			setCheckIfDetectSlideActiveState (false);

			rotateCharacterOnJump ();
		}
	}

	public override void setExternalForceActiveState (bool state)
	{
		setSlideActiveState (state);
	}

	public void setCheckIfDetectSlideActiveState (bool state)
	{
		if (checkIfDetectSlideActive == state) {
			return;
		}

		if (mainPlayerController.isUseExternalControllerBehaviorPaused ()) {
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

		bool checkIfDetectSlideActivePrevioulsy = checkIfDetectSlideActive;

		checkIfDetectSlideActive = state;

		if (checkIfDetectSlideActive) {
//			externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();
//			
//			if (currentExternalControllerBehavior != null && currentExternalControllerBehavior != this) {
//				currentExternalControllerBehavior.disableExternalControllerState ();
//			}
//
			mainPlayerController.setExternalControllerBehavior (this);
		} else {
			if (checkIfDetectSlideActivePrevioulsy) {
				externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (currentExternalControllerBehavior == null || currentExternalControllerBehavior == this) {
					mainPlayerController.setExternalControllerBehavior (null);
				}
			}
		}

		mainPlayerController.setFallDamageCheckPausedState (state);

		if (!checkIfDetectSlideActive) {
			setSlideActiveState (false);
		}
	}

	public void setSlideActiveState (bool state)
	{
		if (!slideEnabled) {
			return;
		}

		if (slideActive == state) {
			return;
		}


		if (state && wallJumpSlidePaused) {
			return;
		}

		slideActive = state;

		mainPlayerController.setAddExtraRotationPausedState (state);

		mainPlayerController.setExternalControlBehaviorForAirTypeActiveState (state);

		setBehaviorCurrentlyActiveState (state);

		setCurrentPlayerActionSystemCustomActionCategoryID ();

		if (state) {
			mainPlayerController.setCheckOnGroungPausedState (true);

			mainPlayerController.setPlayerOnGroundState (false);

			mainPlayerController.setPlayerOnGroundAnimatorStateOnOverrideOnGroundWithTime (false);

			mainPlayerController.overrideOnGroundAnimatorValue (0);

			mainPlayerController.setPlayerOnGroundAnimatorStateOnOverrideOnGround (false);

			mainPlayerController.setOnGroundAnimatorIDValue (false);

			mainPlayerController.setPlayerVelocityToZero ();

		} else {
			mainPlayerController.setCheckOnGroungPausedState (false);

			mainPlayerController.setPlayerOnGroundState (false);

			mainPlayerController.setPlayerOnGroundAnimatorStateOnOverrideOnGroundWithTime (true);

			mainPlayerController.disableOverrideOnGroundAnimatorValue ();

			mainPlayerController.setPauseResetAnimatorStateFOrGroundAnimatorState (true);

			if (jumpInputUsed) {
				mainPlayerController.setOnGroundAnimatorIDValue (false);
			} else {
				if (mainPlayerController.getCurrentSurfaceBelowPlayer () != null) {

					mainPlayerController.setPlayerOnGroundState (true);

					mainPlayerController.setOnGroundAnimatorIDValue (true);
				}
			}
		}

		mainPlayerController.setFootStepManagerState (state);

		jumpInputUsed = false;

		if (showDebugPrint) {
			print ("Slide active state " + state);
		}

		isFirstPersonActive = mainPlayerController.isPlayerOnFirstPerson ();

		if (slideActive) {
			checkEventsOnStateChange (true);

			if (!isFirstPersonActive) {
				mainAnimator.SetInteger (actionIDAnimatorID, actionID);

				mainAnimator.SetBool (externalControlleBehaviorActiveAnimatorID, state);
			}

			mainPlayerController.setJumpsAmountValue (0);

			mainPlayerCamera.enableOrDisableChangeCameraView (false);

			if (!isFirstPersonActive) {
				if (keepWeapons) {
					carryingWeaponsPreviously = mainPlayerWeaponsManager.isPlayerCarringWeapon ();

					if (carryingWeaponsPreviously) {
						mainPlayerWeaponsManager.checkIfDisableCurrentWeapon ();
					}

					mainPlayerWeaponsManager.setGeneralWeaponsInputActiveState (false);
				}
			}

			resetAnimatorIDValue = true;
		} else {
			checkEventsOnStateChange (false);

			if (!isFirstPersonActive) {
				mainAnimator.SetBool (externalControlleBehaviorActiveAnimatorID, state);

				mainAnimator.SetInteger (actionIDAnimatorID, 0);
			}

			mainPlayerCamera.setOriginalchangeCameraViewEnabledValue ();


			if (keepWeapons) {
				mainPlayerWeaponsManager.setGeneralWeaponsInputActiveState (true);
			}

			if (carryingWeaponsPreviously) {
				if (!drawWeaponsIfCarriedPreviously) {
					carryingWeaponsPreviously = false;
				}
			}

			resetAnimatorIDValue = false;
		}

		if (mainHeadTrack != null) {
			mainHeadTrack.setHeadTrackSmoothPauseState (slideActive);
		}

		mainPlayerCamera.setPausePlayerCameraViewChangeState (slideActive);

		mainPlayerController.setLastTimeFalling ();

		mainPlayerCamera.stopShakeCamera ();

		lastTimeSlideActive = Time.time;

		lastTimeInputPressed = 0;

		targetRotation = 0;
	}

	public override void setExternalForceEnabledState (bool state)
	{
		setSlideEnabledState (state);
	}

	public void setSlideEnabledState (bool state)
	{
		if (!state) {
			setSlideActiveState (false);
		}

		slideEnabled = state;
	}

	public void setSlideCanBeUsedState (bool state)
	{
		sliderCanBeUsed = state;
	}

	public void setSliderEnabledState ()
	{
		setSlideEnabledState (originalSlideEnabled);
	}

	public void setOriginalSliderEnabledState ()
	{
		if (slideActive) {
			setCheckIfDetectSlideActiveState (false);
		}

		setSlideEnabledState (originalSlideEnabled);
	}

	public void setSlideDownSpeedMultiplier (float newValue)
	{
		slideDownSpeedMultiplier = newValue;
	}

	public void setForceSlowDownOnSurfaceActiveState (bool state)
	{
		forceSlowDownOnSurfaceActive = state;
	}

	public void enableCheckIfDetectSlideActiveStateExternally ()
	{
		if (checkIfDetectSlideActive) {
			return;
		}

		setCheckIfDetectSlideActiveState (true);
	}

	public void disableCheckIfDetectSlideActiveStateExternally ()
	{
		if (!checkIfDetectSlideActive) {
			return;
		}

		setCheckIfDetectSlideActiveState (false);
	}


	public override void checkIfResumeExternalControllerState ()
	{
		if (checkIfDetectSlideActive) {
			if (showDebugPrint) {
				print ("resuming wall slide jump state");
			}

			externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

			if (currentExternalControllerBehavior != null && currentExternalControllerBehavior != this) {
				currentExternalControllerBehavior.disableExternalControllerState ();
			}

			checkIfDetectSlideActive = false;

			setCheckIfDetectSlideActiveState (true);
		}
	}

	public override void disableExternalControllerState ()
	{
		setCheckIfDetectSlideActiveState (false);
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (isFirstPersonActive) {
			if (useEventsOnFirstPerson) {
				if (state) {
					eventOnStartFirstPerson.Invoke ();
				} else {
					eventOnEndFirstPerson.Invoke ();
				}
			} 
		} else {
			if (useEventsOnThirdPerson) {
				if (state) {
					eventOnStartThirdPerson.Invoke ();
				} else {
					eventOnEndThirdPerson.Invoke ();
				}
			}
		}
	}

	public void setWallJumpSlidePausedState (bool state)
	{
		if (state) {
			if (slideActive) {
				setSlideActiveState (false);
			}
		}

		wallJumpSlidePaused = state;
	}

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
			objectToRotate = playerCameraTransform;
		}

		Quaternion targetRotation = Quaternion.LookRotation (-objectToRotate.forward, objectToRotate.up);

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

//		if (drawWeaponsIfCarriedPreviously) {
//			yield return new WaitForSeconds (1);
//
//			if (carryingWeaponsPreviously) {
//				if (!mainPlayerController.isPlayerOnFirstPerson () && !wallRunningActive && mainPlayerController.canPlayerMove ()) {
//					mainPlayerWeaponsManager.checkIfDrawSingleOrDualWeapon ();
//				}
//
//				carryingWeaponsPreviously = false;
//			}
//		}
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
