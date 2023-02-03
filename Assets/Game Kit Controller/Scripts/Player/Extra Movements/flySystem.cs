using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class flySystem : externalControllerBehavior
{
	[Header ("Main Settings")]
	[Space]

	public bool flyModeEnabled = true;

	public float flyForce;

	public float airFriction = 5;

	public float maxFlyVelocity = 50;

	public float flyHorizontalSpeedMultiplier = 1;
	public float flyVerticalSpeedMultiplier = 2;

	public bool useForceMode = true;

	public ForceMode flyForceMode;

	[Space]
	[Header ("Vertical Movement Settings")]
	[Space]

	public bool moveUpAndDownEnabled = true;

	public float flyMoveUpSpeed = 2;
	public float flyMoveDownSpeed = 2;

	[Space]
	[Header ("Dash Settings")]
	[Space]

	public bool setNewDashID;
	public int newDashID;

	[Space]
	[Header ("Turbo Settings")]
	[Space]

	public bool turboEnabled = true;
	public float flyTurboSpeed;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public float flyRotationSpeedTowardCameraDirection = 10;

	public float flySpeedOnAimMultiplier = 0.5f;

	public string shakeCameraStateName = "Use Fly Turbo";

	[Space]
	[Header ("Animation Settings")]
	[Space]

	public int regularAirID = -1;
	public int flyingID = 4;

	public string flyingModeName = "Flying Mode";

	public bool useIKFlying;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool flyModeActive;
	public bool turboActive;

	public bool playerIsMoving;

	public bool movingUp;
	public bool movingDown;

	public float velocityMagnitude;

	public bool flyForcesPaused;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;
	public playerCamera mainPlayerCamera;
	public IKSystem IKManager;
	public Rigidbody mainRigidbody;
	public Transform mainCameraTransform;
	public Transform playerCameraTransform;
	public Transform playerTransform;
	public dashSystem mainDashSystem;

	public Transform COM;

	Coroutine resetCOMCoroutine;

	Vector3 totalForce;

	float currentAimSpeedMultipler;

	bool originalFlyModeEnabled;

	int previousDashID = -1;


	void Start ()
	{
		originalFlyModeEnabled = flyModeEnabled;
	}

	public override void updateControllerBehavior ()
	{
		//if the player is flying
		if (flyModeActive) {
			if (flyForcesPaused) {
				return;
			}

			Vector3 targetDirection = mainCameraTransform.forward * (mainPlayerController.getVerticalInput () * flyVerticalSpeedMultiplier) +
			                          mainCameraTransform.right * (mainPlayerController.getHorizontalInput () * flyHorizontalSpeedMultiplier);

			playerIsMoving = mainPlayerController.isPlayerMoving (0.1f);

			if (!mainPlayerController.isPlayerOnFirstPerson () && !mainPlayerController.isPlayerRotatingToSurface ()) {
				if (mainPlayerController.isPlayerMovingOn3dWorld ()) {
					Quaternion COMTargetRotation = Quaternion.identity;

					if (playerIsMoving && targetDirection != Vector3.zero) {
						float currentLookAngle = Vector3.SignedAngle (playerCameraTransform.forward, mainCameraTransform.forward, playerCameraTransform.right);

						if (Mathf.Abs (currentLookAngle) > 10 && !mainPlayerController.isPlayerAiming ()) {
							if (turboActive) {
								currentLookAngle = Mathf.Clamp (currentLookAngle, -50, 50);
							} else {
								currentLookAngle = Mathf.Clamp (currentLookAngle, -30, 30);
							}
						} else {
							currentLookAngle = 0;
						}

						COMTargetRotation = Quaternion.Euler (Vector3.right * currentLookAngle);

						Quaternion targetRotation = playerCameraTransform.rotation;

						playerTransform.rotation = Quaternion.Lerp (playerTransform.rotation, targetRotation, 
							flyRotationSpeedTowardCameraDirection * Time.fixedDeltaTime);
					}

					COM.localRotation = Quaternion.Lerp (COM.localRotation, COMTargetRotation,
						flyRotationSpeedTowardCameraDirection * Time.fixedDeltaTime);
				}
			}

			currentAimSpeedMultipler = 1;

			if (!mainPlayerController.isPlayerOnFirstPerson () && mainPlayerController.isPlayerAiming ()) {
				currentAimSpeedMultipler = flySpeedOnAimMultiplier;
			}

			totalForce = targetDirection * (flyForce * currentAimSpeedMultipler);

			if (movingUp) {
				totalForce += playerTransform.up * flyMoveUpSpeed;

				playerIsMoving = true;
			}

			if (movingDown) {
				totalForce -= playerTransform.up * flyMoveDownSpeed;

				playerIsMoving = true;
			}

			if (playerIsMoving) {
				if (turboActive) {
					totalForce *= flyTurboSpeed;
				}

				velocityMagnitude = totalForce.magnitude;

				if (velocityMagnitude > maxFlyVelocity) {
					totalForce = Vector3.ClampMagnitude (totalForce, maxFlyVelocity);
				}

				velocityMagnitude = totalForce.magnitude;

				if (useForceMode) {
					mainRigidbody.AddForce (totalForce, flyForceMode);
				} else {
					mainRigidbody.AddForce (totalForce);
				}
			} else {
				velocityMagnitude = mainRigidbody.velocity.magnitude;

				if (velocityMagnitude > 0) {
					totalForce = mainRigidbody.velocity * (-1 * airFriction);
				}

				mainRigidbody.AddForce (totalForce, flyForceMode);
			}
		}
	}

	public void enableOrDisableFlyingMode (bool state)
	{
		if (!flyModeEnabled) {
			return;
		}

		if (mainPlayerController.isUseExternalControllerBehaviorPaused ()) {
			return;
		}

		if (flyModeActive == state) {
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

		bool flyModeActivePrevioulsy = flyModeActive;

		flyModeActive = state;

		mainPlayerController.enableOrDisableFlyingMode (state);

		setBehaviorCurrentlyActiveState (state);

		setCurrentPlayerActionSystemCustomActionCategoryID ();

		if (flyModeActive) {
			mainPlayerController.setExternalControllerBehavior (this);
		} else {
			if (flyModeActivePrevioulsy) {
				externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (currentExternalControllerBehavior == null || currentExternalControllerBehavior == this) {
					mainPlayerController.setExternalControllerBehavior (null);
				}
			}
		}

		if (useIKFlying) {
			IKManager.setIKBodyState (state, flyingModeName);
		}

		if (!state) {
			mainPlayerController.setLastTimeFalling ();
		}

		if (state) {
			mainPlayerController.setCurrentAirIDValue (flyingID);
		} else {
			mainPlayerController.setCurrentAirIDValue (regularAirID);

			mainPlayerController.setCurrentAirSpeedValue (1);
		}

		if (setNewDashID) {
			if (mainDashSystem != null) {
				if (state) {
					if (previousDashID == -1) {
						previousDashID = mainDashSystem.getCurrentDashID ();
					}

					mainDashSystem.setCheckGroundPausedState (true);

					mainDashSystem.setOverrideStrafeModeActiveStateResult (true);

					mainDashSystem.setCurrentDashID (newDashID);
				} else {
					if (previousDashID != -1) {

						mainDashSystem.setCurrentDashID (previousDashID);

						previousDashID = -1;

						mainDashSystem.setCheckGroundPausedState (false);

						mainDashSystem.setOverrideStrafeModeActiveStateResult (false);
					}
				}
			}
		}

		if (flyModeActive) {
			eventOnStateEnabled.Invoke ();
		} else {
			eventOnStateDisabled.Invoke ();

			resetCOMRotation ();

			if (turboActive) {
				enableOrDisableTurbo (false);
			}

			enableOrDisableVerticalMovementUp (false);

			enableOrDisableVerticalMovementDown (false);
		}

		mainPlayerCamera.stopShakeCamera ();

		if (flyModeActive) {
			mainPlayerController.setPlayerOnGroundState (false);

			mainPlayerController.setOnGroundAnimatorIDValue (false);

			mainPlayerController.setPreviousValueOnGroundAnimatorStateValue (false);
		}

		movingUp = false;

		movingDown = false;
	}

	public void enableOrDisableTurbo (bool state)
	{
		turboActive = state;

		mainPlayerController.enableOrDisableFlyModeTurbo (turboActive);

		mainPlayerCamera.changeCameraFov (turboActive);

		//when the player accelerates his movement in the air, the camera shakes
		if (turboActive) {
			mainPlayerCamera.setShakeCameraState (true, shakeCameraStateName);
		} else {
			mainPlayerCamera.setShakeCameraState (false, "");

			mainPlayerCamera.stopShakeCamera ();
		}

		if (turboActive) {
			mainPlayerController.setCurrentAirSpeedValue (2);
		} else {
			mainPlayerController.setCurrentAirSpeedValue (1);
		}
	}

	public void resetCOMRotation ()
	{
		if (resetCOMCoroutine != null) {
			StopCoroutine (resetCOMCoroutine);
		}

		resetCOMCoroutine = StartCoroutine (resetCOMRotationCoroutine ());
	}

	public IEnumerator resetCOMRotationCoroutine ()
	{
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 4;

			COM.localRotation = Quaternion.Slerp (COM.localRotation, Quaternion.identity, t);

			yield return null;
		}
	}

	public void inputChangeTurboState (bool state)
	{
		if (flyModeActive && turboEnabled) {
			enableOrDisableTurbo (state);
		}
	}

	public void inputMoveUp (bool state)
	{
		if (flyModeActive && moveUpAndDownEnabled) {
			enableOrDisableVerticalMovementUp (state);
		}
	}

	public void inputMoveDown (bool state)
	{
		if (flyModeActive && moveUpAndDownEnabled) {
			enableOrDisableVerticalMovementDown (state);
		}
	}

	void enableOrDisableVerticalMovementUp (bool state)
	{
		movingUp = state;

		if (movingUp) {
			movingDown = false;
		}
	}

	void enableOrDisableVerticalMovementDown (bool state)
	{
		movingDown = state;

		if (movingDown) {
			movingUp = false;
		}
	}

	public void setFlyModeEnabledState (bool state)
	{
		if (flyModeActive) {
			enableOrDisableFlyingMode (false);
		}

		flyModeEnabled = state;
	}

	public void setOriginalFlyModeEnabledState ()
	{
		setFlyModeEnabledState (originalFlyModeEnabled);
	}

	public override void disableExternalControllerState ()
	{
		enableOrDisableFlyingMode (false);
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

	public override void checkPauseStateDuringExternalForceOrBehavior ()
	{
		flyForcesPaused = true;
	}

	public override void checkResumeStateAfterExternalForceOrBehavior ()
	{
		if (flyModeActive) {
			setCurrentPlayerActionSystemCustomActionCategoryID ();

			flyForcesPaused = false;
		}
	}

	public void setEnableExternalForceOnFlyModeState (bool state)
	{
		if (flyModeActive) {
			mainPlayerController.setEnableExternalForceOnFlyModeState (state);
		}
	}
}