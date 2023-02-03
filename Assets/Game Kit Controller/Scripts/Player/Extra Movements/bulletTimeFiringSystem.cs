using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class bulletTimeFiringSystem : OnAnimatorIKComponent
{
	[Header ("Main Settings")]
	[Space]

	public bool movementEnabled = true;

	public float lookDirectionForwardOffset = 1000;

	public float timeForRaisingHands = 0.5f;

	public bool onlyActiveIfUsingFireWeapons;

	public float bodyWeightLerpSpeed = 1;

	[Space]
	[Header ("Get Up Settings")]
	[Space]

	public bool getUpAfterDelayOnGround;

	public float delayToGetUpOnGround;

	public float minWaitToCheckOnGroundState;

	[Space]
	[Header ("Weapons Settings")]
	[Space]

	public bool keepWeaponsDuringAction;

	public bool drawWeaponsAfterAction;

	public bool useOnlyWhenUsingFireWeapons;

	[Space]
	[Header ("Time Bullet Settings")]
	[Space]

	public bool useTimeBullet;
	public float timeBulletScale = 0.5f;

	public bool useTimeBulletDuration;
	public float timeBulletDuration;

	public bool useEventsOnTimeBullet;
	public UnityEvent eventOnTimeBulletStart;
	public UnityEvent eventOnTimeBulletEnd;

	[Space]
	[Header ("Physics Settings")]
	[Space]

	public float jumpForce;
	public float pushForce;

	public float extraSpeedOnFall;
	public float addExtraSpeedOnFallDuration;
	public float delayToAddExtraSpeedOnFall;

	[Space]
	[Header ("Third Person Settings")]
	[Space]

	public int actionID = 323565;
	public int actionIDWithRoll = 323566;

	public bool startActionWithRoll;
	public float rollOnStartDuration;

	public string externalControlleBehaviorActiveAnimatorName = "External Behavior Active";
	public string actionIDAnimatorName = "Action ID";

	public string horizontalAnimatorName = "Horizontal Action";
	public string verticalAnimatorName = "Vertical Action";

	public float delayToResumeAfterGetUp = 1;

	[Space]
	[Header ("Third Person Camera State Settings")]
	[Space]

	public bool setNewCameraStateOnThirdPerson;

	public string newCameraStateOnThirdPerson;

	[Space]
	[Header ("Action System Settings")]
	[Space]

	public int customActionCategoryID = -1;
	public int regularActionCategoryID = -1;


	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool movementActive;

	public bool armsIKActive;

	public float currentTimeForRisingHands;

	public float currentArmsWeight;

	public float armsWeight = 1;

	public bool playerIsCarryingWeapons;

	public Vector3 lookDirectionTarget;

	public bool groundDetectedAfterJump;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnStart;
	public UnityEvent eventOnEnd;

	public UnityEvent eventOnGetUpForward;
	public UnityEvent eventOnGetUpBackward;
	public UnityEvent eventOnGetUpRight;
	public UnityEvent eventOnGetUpLeft;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	public playerCamera mainPlayerCamera;

	public playerInputManager playerInput;

	public timeBullet mainTimeBullet;

	public Animator mainAnimator;

	public Camera mainCamera;

	public Transform mainCameraTransform;

	public playerWeaponsManager mainPlayerWeaponsManager;

	public headTrack mainHeadTrack;

	public Transform playerTransform;

	Vector3 lookDirection;

	Vector2 mouseAxisValues;

	Vector3 leftArmAimPosition;

	Vector3 rightArmAimPosition;

	int externalControlleBehaviorActiveAnimatorID;

	int actionIDAnimatorID;

	int horizontalAnimatorID;

	int verticalAnimatorID;

	bool activateGetUp;

	float lastTimeGetup;

	string previousCameraState;

	float bodyWeight;

	Vector3 lastJumpDirection;

	bool timeBulletActive;

	bool carryingWeaponsPreviously;

	float lastTimeMovementActive;


	void Start ()
	{
		externalControlleBehaviorActiveAnimatorID = Animator.StringToHash (externalControlleBehaviorActiveAnimatorName);

		actionIDAnimatorID = Animator.StringToHash (actionIDAnimatorName);

		horizontalAnimatorID = Animator.StringToHash (horizontalAnimatorName);

		verticalAnimatorID = Animator.StringToHash (verticalAnimatorName);
	}

	void Update ()
	{
		if (movementActive) {
			if (!activateGetUp) {
				lookDirectionTarget = mainCameraTransform.position + mainCameraTransform.forward * lookDirectionForwardOffset;
			}
		}
	}

	void FixedUpdate ()
	{
		if (movementActive) {
			if (activateGetUp) {
				if (Time.unscaledTime > delayToResumeAfterGetUp + lastTimeGetup) {
					disableMovement ();
				}
			} else {
				Vector3 aimDirection = mainCameraTransform.forward;

				aimDirection.y = 0f;

				aimDirection = aimDirection.normalized;

				aimDirection = playerTransform.InverseTransformDirection (aimDirection);

				mainAnimator.SetFloat (horizontalAnimatorID, aimDirection.x);
				mainAnimator.SetFloat (verticalAnimatorID, aimDirection.z);

				if (mainPlayerController.isPlayerOnGround ()) {
					if (Time.unscaledTime > minWaitToCheckOnGroundState + lastTimeMovementActive) { 
						groundDetectedAfterJump = true;
					}
				}

				if (!jumpCoroutineActive) {
					if (extraSpeedOnFall > 0) {
						if (Time.unscaledTime > lastTimeMovementActive + delayToAddExtraSpeedOnFall) {
							if (Time.unscaledTime < addExtraSpeedOnFallDuration + lastTimeMovementActive) {
								if (!groundDetectedAfterJump) {
									mainPlayerController.addExternalForce (lastJumpDirection * extraSpeedOnFall);
								}
							}
						}
					}

					if (getUpAfterDelayOnGround && groundDetectedAfterJump) {
						if (Time.unscaledTime > lastTimeMovementActive + delayToGetUpOnGround) {
							setMovementActiveState (false);
						}
					}
				}

				if (useEventsOnTimeBullet && timeBulletActive) {
					if (Time.unscaledTime > lastTimeMovementActive + timeBulletDuration) {
//						print (Time.unscaledTime + " " + (lastTimeMovementActive + timeBulletDuration));

						checkTimeBullet (false);
					}
				}
			}
		}
	}

	public void setMovementActiveState (bool state)
	{
		if (!movementEnabled) {
			return;
		}

		if (movementActive == state) {
			return;
		}

		if (state) {
			movementActive = state;

			lastTimeMovementActive = Time.unscaledTime;

			if (startActionWithRoll) {
				stopActivateJumpForcesCoroutine ();

				activateJumpCoroutine = StartCoroutine (activateJumpForcesCoroutine ());
			} else {
				activateJumpForces ();
			}

			setPlayerState (true);

			checkTimeBullet (true);
		} else {
			bool lookingForward = false;

			Vector3 movementDirection = playerTransform.forward;

			if (playerInput.getPlayerMovementAxis () != Vector2.zero) {
				movementDirection = mainPlayerController.getMoveInputDirection ();
			}

			float angle = Vector3.SignedAngle (movementDirection, mainCameraTransform.forward, playerTransform.up);

			float ABSAngle = Mathf.Abs (angle);

			if (ABSAngle < 160) {
				lookingForward = true;
			}

			if (showDebugPrint) {
				print ("looking in forward direction " + lookingForward + " " + ABSAngle);
			}

			activateGetUp = true;

			lastTimeGetup = Time.unscaledTime;

			mainAnimator.SetBool (externalControlleBehaviorActiveAnimatorID, false);

			mainAnimator.SetInteger (actionIDAnimatorID, 0);

			if (lookingForward) {
				if (ABSAngle < 45) {
					eventOnGetUpForward.Invoke ();
				} else if (angle < 0) {
					eventOnGetUpRight.Invoke ();

					if (showDebugPrint) {
						print ("activating event on right");
					}
				} else {
					eventOnGetUpLeft.Invoke ();

					if (showDebugPrint) {
						print ("activating event on left");
					}
				}
			} else {
				eventOnGetUpBackward.Invoke ();
			}

			checkTimeBullet (false);

			lastJumpDirection = Vector3.zero;
		}

		groundDetectedAfterJump = false;

		//desactivar el salto y demas acciones que no se puedan hacer, quizas algunas cosas de las armas
		//opciones de usar con y sin armas, quizas para algun tipo de dash o esquive
	}

	void activateJumpForces ()
	{
		setUpdateIKEnabledState (true);

		Vector2 movementDirection = playerInput.getPlayerMovementAxis ();

		Vector3 forceDirection = new Vector3 (movementDirection.x, 0, movementDirection.y);

		if (playerInput.getAuxRawMovementAxis () == Vector2.zero) {
			forceDirection = playerTransform.forward;
		}

		lastJumpDirection = forceDirection;

		Vector3 jumpDirection = pushForce * forceDirection;

		jumpDirection += playerTransform.up * jumpForce;

		mainPlayerController.useJumpPlatform (jumpDirection, ForceMode.Impulse);
	}

	bool jumpCoroutineActive;

	Coroutine activateJumpCoroutine;

	public void stopActivateJumpForcesCoroutine ()
	{
		if (activateJumpCoroutine != null) {
			StopCoroutine (activateJumpCoroutine);
		}

		jumpCoroutineActive = false;
	}

	IEnumerator activateJumpForcesCoroutine ()
	{
		jumpCoroutineActive = true;

		yield return new WaitForSeconds (rollOnStartDuration);

		jumpCoroutineActive = false;

		activateJumpForces ();
	}

	void checkTimeBullet (bool state)
	{
		if (useTimeBullet) {
			if (state) {
				if (!timeBulletActive) {
					mainTimeBullet.setNewTimeBulletTimeSpeedValue (timeBulletScale);
					mainTimeBullet.activateTime ();

					timeBulletActive = true;

					if (useEventsOnTimeBullet) {
						eventOnTimeBulletStart.Invoke ();
					}
				}
			} else {
				if (timeBulletActive) {
					mainTimeBullet.setOriginalTimeBulletTimeSpeed ();
					mainTimeBullet.activateTime ();

					timeBulletActive = false;

					if (useEventsOnTimeBullet) {
						eventOnTimeBulletEnd.Invoke ();
					}
				}
			}
		}
	}

	public void disableMovement ()
	{
		if (movementActive) {
			stopActivateJumpForcesCoroutine ();

			movementActive = false;

			activateGetUp = false;

			setPlayerState (false);
		}
	}

	void setPlayerState (bool state)
	{
		bool isFirstPersonActive = mainPlayerController.isPlayerOnFirstPerson ();

		if (state) {
			if (startActionWithRoll) {
				mainAnimator.SetInteger (actionIDAnimatorID, actionIDWithRoll);
			} else {
				mainAnimator.SetInteger (actionIDAnimatorID, actionID);
			}

			mainAnimator.SetBool (externalControlleBehaviorActiveAnimatorID, state);

			if (setNewCameraStateOnThirdPerson && !isFirstPersonActive) {
				previousCameraState = mainPlayerCamera.getCurrentStateName ();

				mainPlayerCamera.setCameraStateOnlyOnThirdPerson (newCameraStateOnThirdPerson);
			}

			carryingWeaponsPreviously = mainPlayerWeaponsManager.isPlayerCarringWeapon ();

			if (carryingWeaponsPreviously) {
				if (keepWeaponsDuringAction) {
					mainPlayerWeaponsManager.checkIfDisableCurrentWeapon ();

					mainPlayerWeaponsManager.resetWeaponHandIKWeight ();
				} else {
					mainPlayerWeaponsManager.enableOrDisableIKOnWeaponsDuringAction (false);
				}
			}
		} else {
			mainAnimator.SetBool (externalControlleBehaviorActiveAnimatorID, false);

			mainAnimator.SetInteger (actionIDAnimatorID, 0);

			if (setNewCameraStateOnThirdPerson && !isFirstPersonActive) {
				if (previousCameraState != "") {
					if (previousCameraState != newCameraStateOnThirdPerson) {
						mainPlayerCamera.setCameraStateOnlyOnThirdPerson (previousCameraState);
					}

					previousCameraState = "";
				}
			}
		
			if (carryingWeaponsPreviously) {
				if (drawWeaponsAfterAction) {
					mainPlayerWeaponsManager.checkIfDrawSingleOrDualWeapon ();
				} else {
					mainPlayerWeaponsManager.enableOrDisableIKOnWeaponsDuringAction (true);
				}

				carryingWeaponsPreviously = false;
			}

			checkTimeBullet (false);
		}

		setCurrentPlayerActionSystemCustomActionCategoryID ();

		setUpdateIKEnabledState (state);

		mainPlayerController.setIgnoreExternalActionsActiveState (state);

		mainPlayerController.setAddExtraRotationPausedState (state);

		mainHeadTrack.setHeadTrackSmoothPauseState (state);

		mainPlayerController.setUseExternalControllerBehaviorPausedState (state);

		mainPlayerWeaponsManager.setPauseUpperBodyRotationSystemActiveState (state);

		mainPlayerWeaponsManager.setPauseRecoilOnWeaponActiveState (state);

		mainPlayerWeaponsManager.setPauseWeaponReloadActiveState (state);

		mainPlayerWeaponsManager.setPauseWeaponAimMovementActiveState (state);

		mainPlayerController.setIgnoreLookInCameraDirectionOnFreeFireActiveState (state);

		checkEventOnStateChange (state);

		mainPlayerController.setFootStepManagerState (state);

		mainPlayerController.setIgnoreInputOnAirControlActiveState (state);

		mainPlayerController.setPlayerActionsInputEnabledState (!state);
	}

	public void resetActionIdOnTimeBulletJump ()
	{
		mainAnimator.SetInteger (actionIDAnimatorID, 0);
	}

	public override void updateOnAnimatorIKState ()
	{
		if (!updateIKEnabled) {
			return;
		}

		mainAnimator.SetLookAtPosition (lookDirectionTarget);

		mainAnimator.SetLookAtWeight (bodyWeight, 0.5f, 1.0f, 1.0f, 0.7f);

		if (activateGetUp || jumpCoroutineActive) {
			bodyWeight = Mathf.Lerp (0, bodyWeight, currentTimeForRisingHands / bodyWeightLerpSpeed);
		} else {
			bodyWeight = Mathf.Lerp (1, bodyWeight, currentTimeForRisingHands / bodyWeightLerpSpeed);
		}

		if (activateGetUp || jumpCoroutineActive) {
			if (!armsIKActive) {
				return;
			}

			if (currentTimeForRisingHands > 0) {
				currentTimeForRisingHands -= Time.unscaledDeltaTime;

				currentArmsWeight = Mathf.Lerp (0, armsWeight, currentTimeForRisingHands / timeForRaisingHands);
			} else {
				currentTimeForRisingHands = 0;

				currentArmsWeight = 0;

				armsIKActive = false;
			}

		} else {
			armsIKActive = true;

			if (currentTimeForRisingHands < timeForRaisingHands) {
				currentTimeForRisingHands += Time.unscaledDeltaTime;

				currentArmsWeight = Mathf.Lerp (0, armsWeight, currentTimeForRisingHands / timeForRaisingHands);
			} else {
				currentTimeForRisingHands = timeForRaisingHands;

				currentArmsWeight = armsWeight;
			}
		}
	}

	public void setCurrentPlayerActionSystemCustomActionCategoryID ()
	{
		if (movementActive) {
			if (customActionCategoryID > -1) {
				mainPlayerController.setCurrentCustomActionCategoryID (customActionCategoryID);
			}
		} else {
			if (regularActionCategoryID > -1) {
				mainPlayerController.setCurrentCustomActionCategoryID (regularActionCategoryID);
			}
		}
	}

	void checkIfSetBulletTimeFiringState ()
	{
		if (movementEnabled) {
			if (activateGetUp) {
				return;
			}

			if (mainPlayerController.iscloseCombatAttackInProcess ()) {
				return;
			}

			if (useOnlyWhenUsingFireWeapons) {
				if (!mainPlayerController.isPlayerUsingWeapons () ||
				    mainPlayerWeaponsManager.weaponsAreMoving () ||
				    mainPlayerWeaponsManager.currentWeaponIsMoving ()) {
					return;
				}
			}

			if (mainPlayerController.isExternalControllBehaviorActive ()) {
				return;
			}

			if (mainPlayerController.isActionActive ()) {
				return;
			}

			if (mainPlayerController.playerIsBusy ()) {
				return;
			}

			if (!mainPlayerController.canPlayerMove ()) {
				return;
			}

			playerIsCarryingWeapons = mainPlayerController.isPlayerUsingWeapons ();

			if (onlyActiveIfUsingFireWeapons) {
				if (!playerIsCarryingWeapons) {
					return;
				}
			}

			if (movementActive) {
				if (mainPlayerController.isPlayerOnGround ()) {
					setMovementActiveState (false);
				} else {
					disableMovement ();
				}
			} else {
				if (!mainPlayerController.isPlayerOnGround ()) {
					return;
				}

				setMovementActiveState (true);
			}
		}
	}

	public void inputSetTimeBullettFiring ()
	{
		checkIfSetBulletTimeFiringState ();
	}

	public void checkEventOnStateChange (bool state)
	{
		if (state) {
			eventOnStart.Invoke ();
		} else {
			eventOnEnd.Invoke ();
		}
	}
}
