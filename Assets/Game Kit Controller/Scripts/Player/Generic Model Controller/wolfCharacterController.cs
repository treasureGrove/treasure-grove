using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wolfCharacterController : vehicleController
{
	[Header ("Custom Settings")]
	[Space]

	public customCharacterControllerBase mainCustomCharacterControllerBase;

	public playerController mainPlayerController;

	public Transform vehicleTransform;

	public Transform vehicleCameraTransform;

	[Space]
	[Header ("Debug")]
	[Space]

	public float turnAmount;

	public float forwardAmount;


	bool playerUsingInput;

	Vector3 currentMoveInput;

	Vector3 currentForwardDirection;

	Vector3 currentRightDirection;

	public Transform currentCameraPivotTransform;
	public Transform currentMainCameraTranform;

	public bool useForwardDirectionForCameraDirection;

	public bool useRightDirectionForCameraDirection;

	public bool addExtraRotationPaused;

	public override void Start ()
	{
		base.Start ();

		mainPlayerController.setCustomCharacterControllerActiveState (true, mainCustomCharacterControllerBase);

		currentCameraPivotTransform = vehicleCameraTransform;
		currentMainCameraTranform = vehicleCameraTransform;
	}

	void Update ()
	{
		base.vehicleUpdate ();
	}

	void FixedUpdate ()
	{
		mainPlayerController.setCustomAxisValues (new Vector2 (horizontalAxis, verticalAxis));

		currentForwardDirection = currentCameraPivotTransform.forward;
		currentRightDirection = currentMainCameraTranform.right;

		currentMoveInput = verticalAxis * currentForwardDirection + horizontalAxis * currentRightDirection;

		if (currentMoveInput.magnitude > 1) {
			currentMoveInput.Normalize ();
		}

		Vector3 localMove = vehicleTransform.InverseTransformDirection (currentMoveInput);

		//get the amount of rotation added to the character mecanim
		if (currentMoveInput.magnitude > 0) {
			turnAmount = Mathf.Atan2 (localMove.x, localMove.z);
		} else {
			turnAmount = Mathf.Atan2 (0, 0);
		}

		forwardAmount = localMove.z;

		forwardAmount *= boostInput;

		forwardAmount = Mathf.Clamp (forwardAmount, -boostInput, boostInput);
	

		mainCustomCharacterControllerBase.updateForwardAmountInputValue (forwardAmount);

		if (addExtraRotationPaused && forwardAmount < 0.0001f && verticalAxis < 0) {
			turnAmount = 0;
		}

		mainCustomCharacterControllerBase.updateTurnAmountInputValue (turnAmount);

		playerUsingInput = isPlayerUsingInput ();
	
		mainCustomCharacterControllerBase.updatePlayerUsingInputValue (playerUsingInput);

		mainCustomCharacterControllerBase.updateCharacterControllerAnimator ();

		mainCustomCharacterControllerBase.updateCharacterControllerState ();
	}

	public override void updateMovingState ()
	{
		moving = verticalAxis != 0 || horizontalAxis != 0;
	}

	//if the vehicle is using the gravity control, set the state in this component
	public override void changeGravityControlUse (bool state)
	{
		base.changeGravityControlUse (state);


	}

	//the player is getting on or off from the vehicle, so
	public override void changeVehicleState ()
	{
		base.changeVehicleState ();


	}

	public override void setTurnOnState ()
	{

	}

	public override void setTurnOffState (bool previouslyTurnedOn)
	{
		base.setTurnOffState (previouslyTurnedOn);

		if (previouslyTurnedOn) {

		}
	}

	public override void setEngineOnOrOffState ()
	{
		base.setEngineOnOrOffState ();


	}

	public override void turnOnOrOff (bool state, bool previouslyTurnedOn)
	{
		base.turnOnOrOff (state, previouslyTurnedOn);


	}

	//the vehicle has been destroyed, so disabled every component in it
	public override void disableVehicle ()
	{
		base.disableVehicle ();


	}

	//if the vehicle is using the boost, set the boost particles
	public override void usingBoosting ()
	{
		base.usingBoosting ();


	}

	public override void setNewMainCameraTransform (Transform newTransform)
	{
		mainPlayerController.setNewMainCameraTransform (newTransform);

		currentMainCameraTranform = newTransform;
	}

	public override void setNewPlayerCameraTransform (Transform newTransform)
	{
		mainPlayerController.setNewPlayerCameraTransform (newTransform);

		currentCameraPivotTransform = newTransform;
	}

	public override void setUseForwardDirectionForCameraDirectionState (bool state)
	{
//		mainPlayerController.setUseForwardDirectionForCameraDirectionState (state);

		useForwardDirectionForCameraDirection = state;

		if (useForwardDirectionForCameraDirection) {
			currentCameraPivotTransform = vehicleTransform;
		} else {
			currentCameraPivotTransform = vehicleCameraTransform;
		}
	}

	public override void setUseRightDirectionForCameraDirectionState (bool state)
	{
//		mainPlayerController.setUseRightDirectionForCameraDirectionState (state);

		useRightDirectionForCameraDirection = state;

		if (useRightDirectionForCameraDirection) {
			currentMainCameraTranform = vehicleTransform;
		} else {
			currentMainCameraTranform = vehicleCameraTransform;
		}
	}

	public override void setAddExtraRotationPausedState (bool state)
	{
		mainPlayerController.setAddExtraRotationPausedState (state);

		addExtraRotationPaused = state;
	}
		
	//CALL INPUT FUNCTIONS
	public override void inputJump ()
	{
		if (driving && !usingGravityControl && isTurnedOn && vehicleControllerSettings.canJump && mainPlayerController.isPlayerOnGround ()) {
			
			mainPlayerController.inputJump ();
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

					mainPlayerController.inputStartToRun ();
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

				mainPlayerController.inputStopToRun ();
			}
		}
	}

	public override void inputHoldOrReleaseBrake (bool holdingButton)
	{

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
