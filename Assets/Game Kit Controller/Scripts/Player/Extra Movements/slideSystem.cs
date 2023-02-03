using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class slideSystem : externalControllerBehavior
{
	[Header ("Main Settings")]
	[Space]

	public bool slideEnabled = true;

	public LayerMask raycastLayermask;

	public float raycastDistance = 0.6f;

	public float slideActiveRaycastDistance = 3;

	[Space]
	[Header ("Speed Settings")]
	[Space]

	public float slideSpeed;
	public float slideHorizontalSpeed;
	public float slideSprintSpeed;
	public float slideSlowDownSped;
	public float slideVerticalSpeed;

	public float slideRotationSpeed;
	public bool sliderCanBeUsed = true;

	public Vector3 impulseOnJump;
	public Vector3 endOfSurfaceImpulse;

	public float horizontalInputAmount = 1;

	public float regularSlideMovementAmount = 0.4f;

	[Space]
	[Header ("Surface Angle Speed Multiplier Settings")]
	[Space]

	public bool changeSpeedBasedOnSurfaceAngle;
	public float minAngleToIncreaseSpeed;
	public float speedOnSurfaceAngleMultiplier;

	public bool stopSlideIfSurfaceAngleTooLow;
	public float minAngleToStopSlide;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public float maxVelocityChangeSlide;

	public bool keepWeapons;
	public bool drawWeaponsIfCarriedPreviously;

	public bool setNewAimWeaponCameraState;

	public string customDefaultThirdPersonAimRightStateName = "Aim Right On Slide";
	public string customDefaultThirdPersonAimLeftStateName = "Aim Left On Slide";

	public string newCameraState = "Slide View";

	public bool stopManualSlideAfterTimeDelay;
	public float timeDelayToStopManualSlide;

	[Space]
	[Header ("Gravity Settings")]
	[Space]

	public bool adhereToSurfacesEnabled;

	public float adhereToSurfacesRotationSpeed;

	public bool disableSlideWhenDisablingAdhereState = true;

	[Space]
	[Header ("Third Person Settings")]
	[Space]

	public int actionID = 08632946;

	public int slideStandingActionID = 08632945;

	public bool useSlideStandingActionIDOnManualSlide;
	public bool useSlideStandingActionIDOnTriggerSlide;

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

	public bool checkIfDetectSlideActive;

	public bool slideActive;

	public bool isFirstPersonActive;

	public float currentHorizontalMovement;
	public float currentVerticalMovement;

	public float slideSpeedMultiplier = 1;

	public bool carryingWeaponsPreviously;

	public Transform currentSlideTransform;

	public float currentSlideSpeed;

	public Vector3 moveInput;

	public Vector3 localMove;

	public int currentSlideZonesAmountDetected;

	public bool slideActivatedBySlideZone;

	public bool adhereToSurfacesActive;
	public Vector3 originalNormal;

	public bool runningActive;

	public bool manualSlideActive;

	public float targetRotation;

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
	[Header ("Run On Slide Events Settings")]
	[Space]

	public bool useEventtsOnRunSlideStateChange;
	public UnityEvent eventOnStartRunOnSlide;
	public UnityEvent eventOnStopRunOnSlide;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;
	public Transform playerTransform;
	public Rigidbody mainRigidbody;
	public Transform playerCameraTransform;
	public playerCamera mainPlayerCamera;
	public playerWeaponsManager mainPlayerWeaponsManager;
	public gravitySystem mainGravitySystem;

	public headTrack mainHeadTrack;

	public Animator mainAnimator;

	Vector3 currentMovementDirection;

	float lastSlideRunningActive;

	RaycastHit slideHit;

	bool originalSlideEnabled;

	Vector3 playerTransformUp;
	Vector3 playerTransformForward;

	RaycastHit hit;

	Vector3 velocityChange;

	int externalControlleBehaviorActiveAnimatorID;

	int actionIDAnimatorID;

	string previousCameraState = "";

	float lastTimeSlideActive;

	bool resetAnimatorIDValue;

	int horizontalAnimatorID;

	bool jumpInputUsed;

	Transform mainSlideTransform;

	string defaultThirdPersonStateName = "";

	Vector3 currentSurfaceNormal;

	bool runStartEventActive;
	bool runStopEventActive;

	bool canUseRunPaused;

	void Start ()
	{
		originalSlideEnabled = slideEnabled;

		externalControlleBehaviorActiveAnimatorID = Animator.StringToHash (externalControlleBehaviorActiveAnimatorName);
		actionIDAnimatorID = Animator.StringToHash (actionIDAnimatorName);

		horizontalAnimatorID = Animator.StringToHash (horizontalAnimatorName);
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

			float currentFixedUpdateDeltaTime = Time.fixedDeltaTime;

			playerTransformUp = playerTransform.up;

			playerTransformForward = playerTransform.forward;

			Vector3 currentRaycastPosition = playerTransform.position + playerTransformUp * 2 + playerTransform.forward * regularSlideMovementAmount;
			Vector3 currentRaycastDirection = -playerTransformUp;

			currentVerticalMovement = mainPlayerController.getVerticalInput ();
			currentHorizontalMovement = mainPlayerController.getHorizontalInput ();

			moveInput = playerCameraTransform.forward * (currentVerticalMovement) + playerCameraTransform.right * (currentHorizontalMovement);

			if (moveInput.magnitude > 1) {
				moveInput.Normalize ();
			}

			localMove = playerTransform.InverseTransformDirection (moveInput);

			mainAnimator.SetFloat (horizontalAnimatorID, localMove.x, inputLerpSpeed, Time.fixedDeltaTime);

			if (slideActivatedBySlideZone) {
				mainSlideTransform = currentSlideTransform;
			} else {
				mainSlideTransform = playerTransform;
			}

			currentRaycastPosition += mainSlideTransform.right * localMove.x * horizontalInputAmount;

			runningActive = mainPlayerController.isPlayerRunning ();

			if (canUseRunPaused) {
				runningActive = false;
			}

			if (runningActive) {
				if (!runStartEventActive) {
					checkEventsOnRunOnSlideStateChange (true);

					runStartEventActive = true;
				}

				runStopEventActive = false;
			} else {
				if (!runStopEventActive) {
					checkEventsOnRunOnSlideStateChange (false);

					runStopEventActive = true;
				}

				runStartEventActive = false;
			}
				
			float angle = 0;

			if (slideActivatedBySlideZone) {
				angle = Vector3.SignedAngle (playerTransformForward, mainSlideTransform.forward, playerTransformUp);
			} else {
				angle = Vector3.SignedAngle (playerTransformForward, playerCameraTransform.forward, playerTransformUp);
			}

			currentSlideSpeed = slideSpeed;

			if (runningActive) {
				currentSlideSpeed = slideSprintSpeed;
			}
		
			if (localMove.z > 0) {
				currentSlideSpeed += localMove.z * slideVerticalSpeed;
			}

			currentSlideSpeed += Mathf.Abs (localMove.x) * slideHorizontalSpeed;

			if (!runningActive) {
				if (localMove.z < 0) {
					currentSlideSpeed += slideSlowDownSped * localMove.z;
				}
			}

			currentSlideSpeed *= slideSpeedMultiplier;

			if (!adhereToSurfacesActive) {
				if (changeSpeedBasedOnSurfaceAngle) {
					float surfaceAngle = Vector3.SignedAngle (playerTransformUp, currentSurfaceNormal, playerTransform.right);

					float surfaceAngleABS = Mathf.Abs (surfaceAngle);

					if (surfaceAngleABS > minAngleToIncreaseSpeed) {
						currentSlideSpeed += speedOnSurfaceAngleMultiplier * surfaceAngleABS;
					}
				}
					
				if (stopSlideIfSurfaceAngleTooLow) {
					if (Mathf.Abs (angle) < minAngleToStopSlide) {
						currentSlideSpeed = mainRigidbody.velocity.magnitude / 2;

						if (currentSlideSpeed < 0.5f) {
							setCheckIfDetectSlideActiveState (false, false);

							return;
						}
					}
				}
			}

			Debug.DrawRay (currentRaycastPosition, currentRaycastDirection * 3, Color.yellow);

			if (Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, slideActiveRaycastDistance, raycastLayermask)) {
				slideHit = hit;

				currentMovementDirection = slideHit.point;

				mainRigidbody.position = Vector3.MoveTowards (mainRigidbody.position, currentMovementDirection, currentFixedUpdateDeltaTime * currentSlideSpeed);
			
				currentSurfaceNormal = hit.normal;
			} else {
				stopSlide ();
			}

			mainPlayerController.setCurrentVelocityValue (mainRigidbody.velocity);

			targetRotation = angle;
			//Mathf.MoveTowards (targetRotation, angle, slideRotationSpeed);

			if (Mathf.Abs (targetRotation) > 0.001f) {
				playerTransform.Rotate (0, targetRotation * slideRotationSpeed * currentFixedUpdateDeltaTime, 0);
			}

			if (mainPlayerController.updateHeadbobState) {
				mainPlayerController.setCurrentHeadBobState ();
			}

			if (adhereToSurfacesActive) {
				updateGravityValues ();
			} 

			if (manualSlideActive) {
				if (stopManualSlideAfterTimeDelay) {
					if (Time.time > timeDelayToStopManualSlide + lastTimeSlideActive) {
						setCheckIfDetectSlideActiveState (false, false);

						manualSlideActive = false;
					}
				}
			}
		} else {
			if (checkIfDetectSlideActive) {
				if (slideEnabled && sliderCanBeUsed && !mainPlayerController.useFirstPersonPhysicsInThirdPersonActive) {
					if (!slideActive && !mainPlayerController.pauseAllPlayerDownForces && !mainPlayerController.ignoreExternalActionsActiveState) {

						if (Time.time > lastTimeSlideActive + 1) {
							playerTransformUp = playerTransform.up;

							Vector3 currentRaycastPosition = playerTransform.position + playerTransformUp;
							Vector3 currentRaycastDirection = -playerTransformUp;

							if (Physics.Raycast (currentRaycastPosition, currentRaycastDirection, out hit, raycastDistance, raycastLayermask)) {
								setSlideActiveState (true);
							}
						}
					}
				}
			}
		}
	}

	void updateGravityValues ()
	{
		Vector3 currentNormal = mainGravitySystem.getCurrentNormal ();

		//recalculate the rotation of the player and the camera according to the normal of the surface under the player
		currentNormal = Vector3.Lerp (currentNormal, currentSurfaceNormal, adhereToSurfacesRotationSpeed * Time.deltaTime);
		Vector3 myForward = Vector3.Cross (playerTransform.right, currentNormal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, currentNormal); 

		playerTransform.rotation = Quaternion.Lerp (playerTransform.rotation, dstRot, adhereToSurfacesRotationSpeed * Time.deltaTime);

		Vector3 myForwardCamera = Vector3.Cross (playerCameraTransform.right, currentNormal);
		Quaternion dstRotCamera = Quaternion.LookRotation (myForwardCamera, currentNormal);

		playerCameraTransform.rotation = Quaternion.Lerp (playerCameraTransform.rotation, dstRotCamera, adhereToSurfacesRotationSpeed * Time.deltaTime);

		mainGravitySystem.setCurrentNormal (currentNormal);

		mainGravitySystem.updateCurrentRotatingNormal (currentNormal);

		//set the normal in the playerController component
		mainPlayerController.setCurrentNormalCharacter (currentNormal);
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

			if (!slideActivatedBySlideZone) {
				setCheckIfDetectSlideActiveState (false, false);
			}
		}
	}

	public override void setExternalForceActiveState (bool state)
	{
		setSlideActiveState (state);
	}

	public void setCheckIfDetectSlideActiveState (bool state, bool slideActivatedBySlideZoneValue)
	{
		if (mainPlayerController.isUseExternalControllerBehaviorPaused ()) {
			return;
		}

		if (state) {
			currentSlideZonesAmountDetected++;
		} else {
			currentSlideZonesAmountDetected--;

			if (currentSlideZonesAmountDetected < 0) {
				currentSlideZonesAmountDetected = 0;
			}

			if (currentSlideZonesAmountDetected > 0) {
				if (showDebugPrint) {
					print ("more slide zones detected, cancelling the change to regular state");
				}

				return;
			}
		}

		if (checkIfDetectSlideActive == state) {
			return;
		}

		if (state) {
			externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

			if (currentExternalControllerBehavior != null && currentExternalControllerBehavior != this) {
				if (canBeActivatedIfOthersBehaviorsActive && checkIfCanEnableBehavior (currentExternalControllerBehavior.behaviorName)) {
					currentExternalControllerBehavior.disableExternalControllerState ();
				} else {
					currentSlideZonesAmountDetected = 0;

					return;
				}
			}
		}

		slideActivatedBySlideZone = slideActivatedBySlideZoneValue;

		bool checkIfDetectSlideActivePrevioulsy = checkIfDetectSlideActive;

		checkIfDetectSlideActive = state;

		if (checkIfDetectSlideActive) {
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

		slideActive = state;

		setBehaviorCurrentlyActiveState (state);

		setCurrentPlayerActionSystemCustomActionCategoryID ();

		mainPlayerController.setAddExtraRotationPausedState (state);

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

		if (showDebugPrint) {
			print ("Slide active state " + state);
		}

		isFirstPersonActive = mainPlayerController.isPlayerOnFirstPerson ();

		if (slideActive) {
			checkEventsOnStateChange (true);

			if (!isFirstPersonActive) {
				bool animationStandingResult = true;

				if (manualSlideActive) {
					animationStandingResult = useSlideStandingActionIDOnManualSlide;
				} else {
					animationStandingResult = useSlideStandingActionIDOnTriggerSlide;
				}

				if (animationStandingResult) {
					mainAnimator.SetInteger (actionIDAnimatorID, slideStandingActionID);
				} else {
					mainAnimator.SetInteger (actionIDAnimatorID, actionID);
				}

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

				if (setNewAimWeaponCameraState) {
					if (defaultThirdPersonStateName == "") {
						defaultThirdPersonStateName = mainPlayerCamera.getDefaultThirdPersonStateName ();

						mainPlayerCamera.setDefaultThirdPersonStateName (newCameraState);
					}

					mainPlayerCamera.setUseCustomThirdPersonAimActivePausedState (true);

					mainPlayerCamera.setUseCustomThirdPersonAimActiveState (true, 
						customDefaultThirdPersonAimRightStateName, customDefaultThirdPersonAimLeftStateName);
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

			if (setNewAimWeaponCameraState) {
				if (defaultThirdPersonStateName != "") {
					mainPlayerCamera.setDefaultThirdPersonStateName (defaultThirdPersonStateName);

					defaultThirdPersonStateName = "";
				}

				mainPlayerCamera.setUseCustomThirdPersonAimActivePausedState (false);

				mainPlayerCamera.setUseCustomThirdPersonAimActiveState (false, "", "");
			}

			if (carryingWeaponsPreviously) {
				if (!drawWeaponsIfCarriedPreviously) {
					carryingWeaponsPreviously = false;
				}
			}

			resetAnimatorIDValue = false;
		}

		mainPlayerCamera.setPausePlayerCameraViewChangeState (slideActive);

		mainPlayerController.setLastTimeFalling ();

		if (mainHeadTrack != null) {
			mainHeadTrack.setHeadTrackSmoothPauseState (slideActive);
		}

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

		if (state) {
			originalNormal = mainGravitySystem.getCurrentNormal ();
		} else {
			checkResetGravityState ();
		}

		mainPlayerCamera.stopShakeCamera ();

		lastTimeSlideActive = Time.time;

		targetRotation = 0;

		if (runStartEventActive) {
			checkEventsOnRunOnSlideStateChange (false);
		}

		runStartEventActive = false;
		runStopEventActive = false;

		canUseRunPaused = false;

		if (!slideActive) {
			if (manualSlideActive) {
				if (mainPlayerController.isPlayerRunning () && jumpInputUsed) {
					mainPlayerController.forceStopRun ();
				}
			}
		}

		jumpInputUsed = false;
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
			setCheckIfDetectSlideActiveState (false, false);
		}

		setSlideEnabledState (originalSlideEnabled);
	}

	public void setCurrentSlideTransform (Transform newTransform)
	{
		if (newTransform == null) {
			if (currentSlideZonesAmountDetected > 0) {
				return;
			}
		}

		currentSlideTransform = newTransform;
	}

	public void setSlideSpeedMultiplier (float newValue)
	{
		slideSpeedMultiplier = newValue;
	}

	public void enableCheckIfDetectSlideActiveStateExternally ()
	{
		if (checkIfDetectSlideActive || slideActivatedBySlideZone) {
			return;
		}

		setCheckIfDetectSlideActiveState (true, false);

		manualSlideActive = true;
	}

	public void disableCheckIfDetectSlideActiveStateExternally ()
	{
		if (!checkIfDetectSlideActive || slideActivatedBySlideZone) {
			return;
		}

		setCheckIfDetectSlideActiveState (false, false);

		manualSlideActive = false;
	}

	public override void disableExternalControllerState ()
	{
		setCheckIfDetectSlideActiveState (false, false);
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

	public void checkEventsOnRunOnSlideStateChange (bool state)
	{
		if (useEventtsOnRunSlideStateChange) {
			if (state) {
				eventOnStartRunOnSlide.Invoke ();
			} else {
				eventOnStopRunOnSlide.Invoke ();
			}
		}
	}

	public void setAdhereToSurfacesActiveState (bool state)
	{
		if (!adhereToSurfacesEnabled) {
			return;
		}

		if (!state) {
			if (disableSlideWhenDisablingAdhereState) {
				stopSlide ();
			}
		} else {
			if (!adhereToSurfacesActive) {
				originalNormal = mainGravitySystem.getCurrentNormal ();
			}
		}

		adhereToSurfacesActive = state;
	}

	void checkResetGravityState ()
	{
		if (adhereToSurfacesActive) {
			if (mainGravitySystem.getCurrentRotatingNormal () != originalNormal && originalNormal != Vector3.zero) {

				mainGravitySystem.checkRotateToSurface (originalNormal, 2);
			}

			originalNormal = Vector3.zero;
		}
	}

	void stopSlide ()
	{
		Vector3 totalForce = Vector3.zero;

		if (mainSlideTransform != null) {
			totalForce = mainSlideTransform.forward * endOfSurfaceImpulse.z + mainSlideTransform.up * endOfSurfaceImpulse.y;
		} else {
			totalForce = playerTransform.forward * endOfSurfaceImpulse.z + playerTransform.up * endOfSurfaceImpulse.y;
		}

		setSlideActiveState (false);

		setSlideImpulseForce (totalForce, false);

		currentSurfaceNormal = Vector3.zero;
	}

	public void setCanUseRunPausedState (bool state)
	{
		canUseRunPaused = state;
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
