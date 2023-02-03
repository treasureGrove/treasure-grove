using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderCharacterController : vehicleController
{
	[Space]
	[Header ("Custom Settings")]
	[Space]

	public spiderControllerInterface spider;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool jumpInputActive;

	public bool pauseSpiderInput;

	public override void Start ()
	{
		base.Start ();

		if (spider == null) {
			enabled = false;
		}
	}

	void Update ()
	{
		base.vehicleUpdate ();

		//Hold down Space to deactivate ground checking. The spider will fall while space is hold.
		if (pauseSpiderInput) {
			return;
		}

		spider.setGroundcheck (!jumpInputActive);
	}

	void FixedUpdate ()
	{
		if (pauseSpiderInput) {
			return;
		}

		//** Movement **//
		Vector3 input = getInput ();

		if (usingBoost) {
			spider.run (input);
		} else {
			spider.walk (input);
		}

		Quaternion tempCamTargetRotation = spider.getCamTargetRotation ();
		Vector3 tempCamTargetPosition = spider.getCamTargetPosition ();

		spider.turn (input);

		spider.setTargetRotation (tempCamTargetRotation);
		spider.setTargetPosition (tempCamTargetPosition);
	}

	private Vector3 getInput ()
	{
		Vector3 up = spider.transform.up;
		Vector3 right = spider.transform.right;

		axisValues = getPlayerMovementAxis ();

		Vector3 input = Vector3.ProjectOnPlane (spider.getCameraTarget ().forward, up).normalized * axisValues.y +
		                (Vector3.ProjectOnPlane (spider.getCameraTarget ().right, up).normalized * axisValues.x);

		Quaternion fromTo = Quaternion.AngleAxis (Vector3.SignedAngle (up, spider.getGroundNormal (), right), right);

		input = fromTo * input;
		float magnitude = input.magnitude;

		return (magnitude <= 1) ? input : input /= magnitude;
	}

	public void setPauseSpiderInputState (bool state)
	{
		pauseSpiderInput = state;
	}

	//INPUT FUNCTIONS
	public override void inputHoldOrReleaseJump (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			jumpInputActive = holdingButton;
		}
	}

	public override void inputHoldOrReleaseTurbo (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			//boost input
			if (holdingButton) {
				if (vehicleControllerSettings.canUseBoost) {
					usingBoost = true;
					//set the camera move away action
					mainVehicleCameraController.usingBoost (true, vehicleControllerSettings.boostCameraShakeStateName, 
						vehicleControllerSettings.useBoostCameraShake, vehicleControllerSettings.moveCameraAwayOnBoost);
				}
			} else {
				//stop boost input
				usingBoost = false;

				//disable the camera move away action
				mainVehicleCameraController.usingBoost (false, vehicleControllerSettings.boostCameraShakeStateName, 
					vehicleControllerSettings.useBoostCameraShake, vehicleControllerSettings.moveCameraAwayOnBoost);

				//disable the boost particles
				usingBoosting ();

				boostInput = 1;
			}
		}
	}

	public override void inputSetTurnOnState ()
	{
		if (driving && !usingGravityControl) {
			if (mainVehicleHUDManager.canSetTurnOnState) {
				setEngineOnOrOffState ();
			}
		}
	}
}
