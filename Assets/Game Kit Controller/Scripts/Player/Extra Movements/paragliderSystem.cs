using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class paragliderSystem : externalControllerBehavior
{
	[Header ("Main Settings")]
	[Space]

	public bool paragliderModeEnabled = true;

	public float gravityForce = -9.8f;
	public float gravityMultiplier;

	public float minWaitTimeToActivateParaglider = 0.6f;

	public float airSpeed = 25;
	public float airControl = 10;

	public bool addExtraUpForce;
	public float extraUpForceAmount = 5;

	[Space]
	[Header ("Animation Settings")]
	[Space]

	public int regularAirID = -1;
	public int paragliderID = 1;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool paragliderModePaused;

	public bool paragliderModeActive;

	public bool playerIsMoving;

	public bool externalForceActive;

	public Vector3 externaForceDirection;

	public float externalForceAmount;

	public bool checkingToActivateParaglider;

	public int externalForcesCounter;

	public bool useLastTimeParagliderPauseActive;

	public float previousGravityForce = -1000;

	public float previousGravityMultiplier = -1000;

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
	public Rigidbody mainRigidbody;
	public Transform playerTransform;

	float lastTimeParagliderPauseActive;
	float useLastTimeParagliderPauseActiveDuration;

	Coroutine checkingToActivateCoroutine;

	bool originalParagliderModeEnabled;

	void Start ()
	{
		originalParagliderModeEnabled = paragliderModeEnabled;
	}

	public override void updateControllerBehavior ()
	{
		if (paragliderModeActive) {
			if ((!mainPlayerController.isPlayerOnFirstPerson () && mainPlayerController.isPlayerAiming ()) ||
			    mainPlayerController.isPlayerOnGround () ||
			    mainPlayerController.isSwimModeActive () ||
			    mainPlayerController.isWallRunningActive () ||
			    mainPlayerController.isExternalControlBehaviorForAirTypeActive ()) {

				enableOrDisableParagliderMode (false);
			}
				
			mainPlayerController.setLastTimeFalling ();

			Vector3 movementDirection = mainPlayerController.getMoveInputDirection () * airSpeed;

			movementDirection += playerTransform.InverseTransformDirection (mainRigidbody.velocity).y * playerTransform.up;

			if (addExtraUpForce) {
				movementDirection += playerTransform.up * extraUpForceAmount;
			}

			if (externalForceActive) {
				movementDirection += externaForceDirection * externalForceAmount;
			}

			mainPlayerController.setExternalForceOnAir (movementDirection, airControl);
		}
	}

	public override bool isCharacterOnGround ()
	{
		if (paragliderModeActive) {
			return mainPlayerController.isPlayerOnGround ();
		}

		return true;
	}

	public override bool isBehaviorActive ()
	{
		return paragliderModeActive;
	}

	public void enableOrDisableParagliderMode (bool state)
	{
		if (showDebugPrint) {
			print ("paraglider " + state);
		}

		if (!paragliderModeEnabled) {
			return;
		}

		if (mainPlayerController.isUseExternalControllerBehaviorPaused ()) {
			return;
		}

		if (paragliderModePaused && state) {
			return;
		}

		if (paragliderModeActive == state) {
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
			
		bool paragliderModeActivePrevioulsy = paragliderModeActive;

		paragliderModeActive = state;

		bool usingDifferentExternalControllerBehavior = false;

		if (showDebugPrint) {
			print ("setting state");
		}

		setBehaviorCurrentlyActiveState (state);

		setCurrentPlayerActionSystemCustomActionCategoryID ();

		if (paragliderModeActive) {
			mainPlayerController.setExternalControllerBehavior (this);
		} else {
			if (paragliderModeActivePrevioulsy) {
				externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (currentExternalControllerBehavior == null || currentExternalControllerBehavior == this) {
					mainPlayerController.setExternalControllerBehavior (null);
				} else {
					usingDifferentExternalControllerBehavior = true;
				}
			}
		}

		if (!state) {
			if (!usingDifferentExternalControllerBehavior) {
				mainPlayerController.setLastTimeFalling ();
			}
		}

		if (state) {
			mainPlayerController.setCurrentAirIDValue (paragliderID);
		} else {
			if (!usingDifferentExternalControllerBehavior) {
				mainPlayerController.setCurrentAirIDValue (regularAirID);
			}
		}

		if (paragliderModeActive) {
			eventOnStateEnabled.Invoke ();
		} else {
			eventOnStateDisabled.Invoke ();
		}

		mainPlayerController.setSlowFallExternallyActiveState (paragliderModeActive);

		if (paragliderModeActive) {
			if (previousGravityForce == -1000) {
				previousGravityForce = mainPlayerController.getGravityForce ();

				previousGravityMultiplier = mainPlayerController.getGravityMultiplier ();

				if (previousGravityMultiplier > gravityMultiplier) {
					mainPlayerController.setGravityMultiplierValue (false, gravityMultiplier); 
				}

				if (previousGravityForce > gravityForce) {
					mainPlayerController.setGravityForceValue (false, gravityForce);
				}
			}
		} else {
			if (previousGravityForce != -1000) {
				mainPlayerController.setGravityMultiplierValue (false, previousGravityMultiplier);

				mainPlayerController.setGravityForceValue (false, previousGravityForce);

				previousGravityForce = -1000;

				previousGravityMultiplier = -1000;
			}
		}

		mainPlayerCamera.stopShakeCamera ();
	}

	public override void updateExternalForceActiveState (Vector3 forceDirection, float forceAmount)
	{
		if (!paragliderModeEnabled) {
			return;
		}

		externaForceDirection = forceDirection;

		externalForceAmount = forceAmount;
	}

	public override void setExternalForceActiveState (bool state)
	{
		if (!paragliderModeEnabled) {
			return;
		}

		externalForceActive = state;

		if (state) {
			externalForcesCounter++;
		} else {
			externalForcesCounter--;
		}

		if (externalForcesCounter < 0) {
			externalForcesCounter = 0;
		}

		if (externalForcesCounter > 0) {
			externalForceActive = true;
		} else if (externalForcesCounter == 0) {
			externalForceActive = false;
		}
	}

	public void inputToggletParagliderActiveState ()
	{
		if (paragliderModeActive) {
			enableOrDisableParagliderMode (false);
		} else {
			inputSetParagliderActiveState (!checkingToActivateParaglider);
		}
	}

	public void inputSetParagliderActiveState (bool state)
	{
		if (!paragliderModeEnabled) {
			return;
		}

		if (mainPlayerController.isPlayerOnGround () ||
		    mainPlayerController.isPlayerOnFFOrZeroGravityModeOn () ||
		    mainPlayerController.isWallRunningActive () ||
		    mainPlayerController.isSwimModeActive () ||
		    mainPlayerController.isPlayerDriving () ||
		    mainPlayerController.isExternalControlBehaviorForAirTypeActive ()) {

			if (checkingToActivateParaglider) {

				stopCheckingToActivateParagliderCoroutine ();

				checkingToActivateParaglider = false;
			}

			return;
		}

		if (useLastTimeParagliderPauseActive) {
			if (Time.time < lastTimeParagliderPauseActive + useLastTimeParagliderPauseActiveDuration) {
				return;
			} else {
				useLastTimeParagliderPauseActive = false;
			}
		}

		checkingToActivateParaglider = state;

		if (checkingToActivateParaglider) {
			stopCheckingToActivateParagliderCoroutine ();

			checkingToActivateCoroutine = StartCoroutine (checkingToActivateParagliderCoroutine ());
		} else {
			if (paragliderModeActive) {
				enableOrDisableParagliderMode (false);
			}

			stopCheckingToActivateParagliderCoroutine ();
		}
	}

	public void stopCheckingToActivateParagliderCoroutine ()
	{
		if (checkingToActivateCoroutine != null) {
			StopCoroutine (checkingToActivateCoroutine);
		}
	}

	IEnumerator checkingToActivateParagliderCoroutine ()
	{
		yield return new WaitForSeconds (minWaitTimeToActivateParaglider);

		if (checkingToActivateParaglider) {
			
			enableOrDisableParagliderMode (true);

			checkingToActivateParaglider = false;
		}
	}

	public void setParagliderModePausedState (bool state)
	{
		if (state) {
			if (paragliderModeActive) {
				enableOrDisableParagliderMode (false);
			}
		}

		paragliderModePaused = state;
	}

	public void setUseLastTimeParagliderPauseActive (float newDuration)
	{
		useLastTimeParagliderPauseActive = true;

		lastTimeParagliderPauseActive = Time.time;

		useLastTimeParagliderPauseActiveDuration = newDuration;
	}

	public void setParagliderModeEnabledState (bool state)
	{
		if (paragliderModeActive) {
			enableOrDisableParagliderMode (false);
		}

		paragliderModeEnabled = state;
	}

	public void setOriginalParagliderModeEnabledState ()
	{
		setParagliderModeEnabledState (originalParagliderModeEnabled);
	}

	public override void disableExternalControllerState ()
	{
		if (paragliderModeActive) {
			enableOrDisableParagliderMode (false);
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