using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class vehicleController : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool updateVehicleStateEnabled;
	public bool callVehicleUpdateEachFrame = true;
	public bool callVehicleFixedUpdateEachFrame = true;

	public bool updatePassengersAngularDirection;

	public bool useHorizontalInputLerp = true;

	[Space]
	[Header ("Vehicle Settings")]
	[Space]

	public vehicleControllerSettingsInfo vehicleControllerSettings;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool driving;
	public bool isTurnedOn;
	public bool vehicleDestroyed;

	public bool usingGravityControl;

	public bool braking;

	public bool usingImpulse;

	public bool jumpInputPressed;

	public Vector3 currentNormal;

	public float lastTimeJump;

	public float currentSpeed;

	public bool autobrakeActive;

	[Space]
	[Header ("Debug Input")]
	[Space]


	public float horizontalAxis = 0;

	public float verticalAxis = 0;

	public Vector2 axisValues;

	public Vector2 rawAxisValues;

	public float motorInput;

	public bool moving;

	public bool usingBoost;

	public float boostInput = 1;

	public bool touchPlatform;

	[Space]
	[Header ("Components")]
	[Space]

	public vehicleHUDManager mainVehicleHUDManager;
	public vehicleCameraController mainVehicleCameraController;
	public vehicleGravityControl mainVehicleGravityControl;
	public Rigidbody mainRigidbody;
	public IKDrivingSystem mainIKDrivingSystem;
	public inputActionManager mainInputActionManager;


	[HideInInspector]public bool changingGear;
	[HideInInspector] public float steering;
	[HideInInspector]public Vector3 localLook;

	float lastTimeImpulseUsed;


	public virtual void Start ()
	{
		touchPlatform = touchJoystick.checkTouchPlatform ();
	}

	public virtual void vehicleUpdate ()
	{
		//if the player is driving this vehicle and the gravity control is not being used, then
		if (driving && !usingGravityControl) {
			axisValues = mainInputActionManager.getPlayerMovementAxis ();

			horizontalAxis = axisValues.x;

			rawAxisValues = mainInputActionManager.getPlayerRawMovementAxis ();

			if (!useHorizontalInputLerp && !touchPlatform) {

				horizontalAxis = rawAxisValues.x;
			}

			if (mainVehicleCameraController.currentState.useCameraSteer && horizontalAxis == 0) {
				localLook = transform.InverseTransformDirection (mainVehicleCameraController.getLookDirection ());

				updateCameraSteerState ();
			}

			if (isTurnedOn) {

				//get the current values from the input manager, keyboard and touch controls
				verticalAxis = axisValues.y;

				if (vehicleControllerSettings.canImpulseHoldingJump) {
					if (usingImpulse) {
						if (vehicleControllerSettings.impulseUseEnergy) {
							if (Time.time > lastTimeImpulseUsed + vehicleControllerSettings.impulseUseEnergyRate) {
								mainVehicleHUDManager.removeEnergy (vehicleControllerSettings.impulseUseEnergyAmount);
								lastTimeImpulseUsed = Time.time;
							}
						}

						mainRigidbody.AddForce (transform.up * (mainRigidbody.mass * vehicleControllerSettings.impulseForce));
					}
				}
			}

			//if the boost input is enabled, check if there is energy enough to use it
			if (usingBoost) {
				//if there is enough energy, enable the boost
				if (mainVehicleHUDManager.useBoost (moving)) {
					boostInput = vehicleControllerSettings.maxBoostMultiplier;

					usingBoosting ();
				} else {

					//else, disable the boost
					usingBoost = false;

					//if the vehicle is not using the gravity control system, disable the camera move away action
					if (!mainVehicleGravityControl.isGravityPowerActive ()) {
						mainVehicleCameraController.usingBoost (false, vehicleControllerSettings.boostCameraShakeStateName, 
							vehicleControllerSettings.useBoostCameraShake, vehicleControllerSettings.moveCameraAwayOnBoost);
					}

					usingBoosting ();

					boostInput = 1;
				}
			}

			//set the current speed in the HUD of the vehicle
			mainVehicleHUDManager.getSpeed (currentSpeed, vehicleControllerSettings.maxForwardSpeed);

			if (updatePassengersAngularDirection) {
				mainIKDrivingSystem.setNewAngularDirection (Vector3.forward * verticalAxis + Vector3.right * horizontalAxis);
			}
		} else {
			//else, set the input values to 0
			horizontalAxis = 0;
			verticalAxis = 0;

			rawAxisValues = Vector2.zero;
		}

		if (!changingGear) {
			motorInput = verticalAxis;
		} else {
			motorInput = Mathf.Clamp (verticalAxis, -1, 0);
		}

		updateMovingState ();

		//if the vehicle has fuel, allow to move it
		if (moving) {
			if (!mainVehicleHUDManager.useFuel ()) {
				motorInput = 0;

				if (isTurnedOn) {
					turnOnOrOff (false, isTurnedOn);
				}
			}
		}

		checkAutoBrakeOnGetOffState ();
	}

	public virtual void updateCameraSteerState ()
	{
		if (localLook.z < 0f) {
			localLook.x = Mathf.Sign (localLook.x);
		}

		steering = localLook.x;
		steering = Mathf.Clamp (steering, -1f, 1f);

		if (axisValues.y < 0) {
			steering *= (-1);
		}

		horizontalAxis = steering;
	}

	public virtual void updateMovingState ()
	{
		moving = verticalAxis != 0;
	}

	public virtual void vehicleFixedUpdate ()
	{
		
	}

	public virtual void setCollisionDetected (Collision currentCollision)
	{

	}

	public virtual void startBrakeVehicleToStopCompletely ()
	{

	}

	public virtual void endBrakeVehicleToStopCompletely ()
	{

	}

	public virtual void changeVehicleState ()
	{
		driving = !driving;

		//set the audio values if the player is getting on or off from the vehicle
		if (driving) {
			if (mainVehicleHUDManager.autoTurnOnWhenGetOn) {
				turnOnOrOff (true, isTurnedOn);
			}
		} else {
			turnOnOrOff (false, isTurnedOn);
		}
		
		//set the same state in the gravity control components
	
		if (mainVehicleGravityControl != null) {
			mainVehicleGravityControl.changeGravityControlState (driving);
		}

		if (driving) {
			autobrakeActive = false;
		}
	}

	public virtual void passengerGettingOnOff ()
	{

	}

	public virtual void disableVehicle ()
	{
		vehicleDestroyed = true;
		
		setTurnOffState (false);

		//disable the controller
		this.enabled = false;
	}

	public virtual float getCurrentSpeed ()
	{
		return 100;
	}


	//INPUT FUNCTIONS
	public virtual void inputJump ()
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			//jump input
			if (vehicleControllerSettings.canJump) {
				jumpInputPressed = true;
			}
		}
	}

	public virtual void inputHoldOrReleaseJump (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			if (vehicleControllerSettings.canImpulseHoldingJump) {
				if (holdingButton) {
					if (Time.time > lastTimeJump + 0.2f) {
						usingImpulse = true;
					}
				} else {
					usingImpulse = false;
				}
			}
		}
	}

	public virtual void inputHoldOrReleaseTurbo (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			if (holdingButton) {
				//boost input
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

	public virtual void inputSetTurnOnState ()
	{
		if (driving && !usingGravityControl) {
			if (mainVehicleHUDManager.canSetTurnOnState) {
				setEngineOnOrOffState ();
			}
		}
	}

	public virtual void inputHorn ()
	{
		if (driving && !usingGravityControl) {
			pressHorn ();
		}
	}

	public virtual void inputHoldOrReleaseBrake (bool holdingButton)
	{
		if (driving && !usingGravityControl) {
			braking = holdingButton;
		}
	}


	public virtual void pressHorn ()
	{
		mainVehicleHUDManager.activateHorn ();
	}

	//play or stop every audio component in the vehicle, like engine, skid, etc.., configuring also volume and loop according to the movement of the vehicle
	public void setAudioState (AudioSource source, float distance, float volume, AudioClip audioClip, bool loop, bool play, bool stop)
	{
		source.minDistance = distance;
		source.volume = volume;
		source.clip = audioClip;
		source.loop = loop;
		source.spatialBlend = 1;

		if (play) {
			source.Play ();
		}

		if (stop) {
			source.Stop ();
		}
	}

	public virtual void setEngineOnOrOffState ()
	{
		if (mainVehicleHUDManager.hasFuel ()) {
			turnOnOrOff (!isTurnedOn, isTurnedOn);
		}
	}

	public virtual void turnOnOrOff (bool state, bool previouslyTurnedOn)
	{
		isTurnedOn = state;

		if (isTurnedOn) {
			setTurnOnState ();
		} else {
			setTurnOffState (previouslyTurnedOn);
		}
	}

	public virtual void setTurnOnState ()
	{
		
	}

	public virtual void setTurnOffState (bool previouslyTurnedOn)
	{
		motorInput = 0;

		boostInput = 1;

		horizontalAxis = 0;

		verticalAxis = 0;
	
		//stop the boost
		if (usingBoost) {
			usingBoost = false;

			mainVehicleCameraController.usingBoost (false, vehicleControllerSettings.boostCameraShakeStateName, 
				vehicleControllerSettings.useBoostCameraShake, vehicleControllerSettings.moveCameraAwayOnBoost);

			usingBoosting ();
		}
	
		usingImpulse = false;
	}

	//if the vehicle is using the gravity control, set the state in this component
	public virtual void changeGravityControlUse (bool state)
	{
		usingGravityControl = state;
	
		usingImpulse = false;
	}

	//get the current normal in the gravity control component
	public virtual void setNormal (Vector3 normalValue)
	{
		currentNormal = normalValue;
	}

	public virtual Vector3 getNormal ()
	{
		return currentNormal;
	}

	//if the vehicle is using the boost, set the boost particles
	public virtual void usingBoosting ()
	{
		
	}

	public virtual bool isUseOfGravityActive ()
	{
		return mainVehicleGravityControl.useGravity;
	}

	public bool isPlayerUsingInput ()
	{
		if (rawAxisValues.x != 0 || rawAxisValues.y != 0) {
			return true;
		}

		return false;
	}

	public virtual Vector3 getCurrentNormal ()
	{
		return mainVehicleGravityControl.getCurrentNormal ();
	}

	public virtual void setNewMainCameraTransform (Transform newTransform)
	{

	}

	public virtual void setNewPlayerCameraTransform (Transform newTransform)
	{

	}

	public virtual void setUseForwardDirectionForCameraDirectionState (bool state)
	{

	}

	public virtual void setUseRightDirectionForCameraDirectionState (bool state)
	{

	}

	public virtual void setAddExtraRotationPausedState (bool state)
	{

	}

	public virtual void activateAutoBrakeOnGetOff ()
	{
		braking = true;

		autobrakeActive = true;
	}

	public virtual void checkAutoBrakeOnGetOffState ()
	{
		if (autobrakeActive) {
			if (Mathf.Abs (mainRigidbody.velocity.magnitude) < 0.5f) {
				autobrakeActive = false;

				braking = false;
			}
		}
	}

	public float getHorizontalAxis ()
	{
		return horizontalAxis;
	}

	public float getVerticalAxis ()
	{
		return verticalAxis;
	}

	public Vector2 getPlayerMouseAxis ()
	{
		return mainInputActionManager.getPlayerMouseAxis ();
	}

	public Vector2 getPlayerMovementAxis ()
	{
		return mainInputActionManager.getPlayerMovementAxis ();
	}

	public void getVehicleComponents (GameObject mainVehicleObject)
	{
		mainVehicleHUDManager = mainVehicleObject.GetComponentInChildren<vehicleHUDManager> ();

		mainVehicleCameraController = mainVehicleObject.GetComponentInChildren<vehicleCameraController> ();

		mainVehicleGravityControl = mainVehicleObject.GetComponentInChildren<vehicleGravityControl> ();

		mainRigidbody = mainVehicleHUDManager.gameObject.GetComponent<Rigidbody> ();

		mainIKDrivingSystem = mainVehicleObject.GetComponentInChildren<IKDrivingSystem> ();

		mainInputActionManager = mainVehicleObject.GetComponentInChildren<inputActionManager> ();

		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class vehicleControllerSettingsInfo
	{
		public float maxForwardSpeed;

		[Space]

		public bool canUseBoost;

		public float maxBoostMultiplier;

		public bool useBoostCameraShake = true;
		public bool moveCameraAwayOnBoost = true;
		public string boostCameraShakeStateName = "Boost";

		[Space]

		public bool canJump;

		public float jumpPower;
	
		public bool canImpulseHoldingJump;
		public float impulseForce;
		public bool impulseUseEnergy;
		public float impulseUseEnergyRate;
		public float impulseUseEnergyAmount;

		[Space]

		public bool autoBrakeOnGetOff;
	}
}