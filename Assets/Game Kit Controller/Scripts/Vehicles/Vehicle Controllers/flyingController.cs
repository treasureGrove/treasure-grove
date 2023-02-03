using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class flyingController : vehicleController
{
	[Header ("Custom Settings")]
	[Space]

	public float timeToFlip = 2;
	public float audioEngineSpeed;
	public float engineMinVolume = 0.5f;
	public float engineMaxVolume = 1;
	public float minAudioPitch = 0.5f;
	public float maxAudioPitch = 1.2f;

	public float maxSpeed = 30;

	public float velocityChangeSpeed = 10;

	public float stabilityForce = 2;
	public float stabilitySpeed = 2;

	public float forwardSpeed = 15;
	public float rightSpeed = 15;
	public float upSpeed = 15;

	public float rollRotationSpeed = 5;

	public float engineArmRotationAmountForward;
	public float engineArmRotationAmountRight;

	public float engineArmExtraRotationAmount;

	public float engineArmRotationSpeed;

	public float engineRotationSpeed = 10;

	public float groundRaycastDistance;
	public float hoverForce = 5;
	public float hoverFoeceOnAir = 2;
	public float rotateForce = 5;
	public float extraHoverSpeed = 2;

	public bool increaseHeightIfSurfaceBelowFound;
	public float raycastDistanceToCheckSurfaceBelow = 5;
	public float minDistanceToIncreaseHeight = 4;
	public Transform surfaceBelowRaycastPosition;

	[Space]
	[Header ("Vehicle Settings")]
	[Space]

	public otherVehicleParts otherCarParts;
	public vehicletSettings settings;

	[Space]
	[Header ("Debug")]
	[Space]

	public float heightInput;
	public float rollDirection;
	public bool groundFound;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform leftEngineTransform;
	public Transform rightEngineTransform;

	public Transform leftEngineArmTransform;
	public Transform rightEngineArmTransform;

	public Transform groundRaycastPosition;

	float audioPower = 0;
	float maxEnginePower;

	float resetTimer;

	float steerInput;
	float forwardInput;

	int i;
	int collisionForceLimit = 5;

	bool rotating;
	Transform vehicleCameraTransform;

	float leftInput = 0;
	float rightInput = 0;


	bool checkVehicleTurnedOn;

	Vector3 currenLeftEngineArmRotation;
	Vector3 currenRightEngineArmRotation;

	Vector3 currenLeftEngineRotation;
	Vector3 currenRightEngineRotation;

	Vector3 appliedHoverForce;
	RaycastHit hit;

	bool usingRollInput;

	float currentHitDistance;

	bool surfaceDetected;

	bool increasingHeightFromSurfaceDetected;

	public override void Start ()
	{
		base.Start ();

		//get the boost particles inside the vehicle
		for (i = 0; i < otherCarParts.boostingParticles.Count; i++) {
			otherCarParts.boostingParticles [i].gameObject.SetActive (false);
		}

		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);

		vehicleCameraTransform = mainVehicleCameraController.transform;
	}

	public override void vehicleUpdate ()
	{
		//if the player is driving this vehicle and the gravity control is not being used, then
		if (driving && !usingGravityControl) {
			axisValues = mainInputActionManager.getPlayerMovementAxis ();
			horizontalAxis = axisValues.x;

			if (!useHorizontalInputLerp && !touchPlatform) {
				rawAxisValues = mainInputActionManager.getPlayerRawMovementAxis ();
			
				horizontalAxis = rawAxisValues.x;
			}
				
			if (mainVehicleHUDManager.usedByAI) {
				rollDirection = horizontalAxis;

				horizontalAxis = 0;
			}
				
			if (mainVehicleCameraController.currentState.useCameraSteer && !usingRollInput) {
				localLook = transform.InverseTransformDirection (mainVehicleCameraController.getLookDirection ());

				if (localLook.z < 0f) {
					localLook.x = Mathf.Sign (localLook.x);
				}

				steering = localLook.x;
				steering = Mathf.Clamp (steering, -1f, 1f);

				steering = -steering;

				rollDirection = steering;
			}

			if (isTurnedOn) {
				//get the current values from the input manager, keyboard and touch controls
				verticalAxis = axisValues.y;
			}

			//if the boost input is enabled, check if there is energy enough to use it
			if (usingBoost) {
				//if there is enough energy, enable the boost
				if (mainVehicleHUDManager.useBoost (moving)) {
					boostInput = vehicleControllerSettings.maxBoostMultiplier;

					usingBoosting ();
				} 
				//else, disable the boost
				else {
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
			mainVehicleHUDManager.getSpeed (currentSpeed, maxSpeed);

			mainIKDrivingSystem.setNewAngularDirection (Vector3.forward * verticalAxis + Vector3.right * horizontalAxis + Vector3.forward * heightInput + Vector3.up * rollDirection);
		}

		moving = verticalAxis != 0 || horizontalAxis != 0 || heightInput != 0 || rollDirection != 0;

		motorInput = verticalAxis;

		//if the vehicle has fuel, allow to move it
		if (moving) {
			if (!mainVehicleHUDManager.useFuel ()) {
				motorInput = 0;

				if (isTurnedOn) {
					turnOnOrOff (false, isTurnedOn);
				}
			}
		}

		maxEnginePower = currentSpeed;

		audioPower = Mathf.Lerp (maxEnginePower, motorInput, audioEngineSpeed);
		otherCarParts.engineAudio.volume = Mathf.Lerp (engineMinVolume, engineMaxVolume, audioPower);
		otherCarParts.engineAudio.pitch = Mathf.Lerp (minAudioPitch, maxAudioPitch, audioPower);

		if (Mathf.Abs (horizontalAxis) > 0.05f) {
			steerInput = Mathf.Lerp (steerInput, horizontalAxis, Time.deltaTime * 10);
		} else {
			steerInput = Mathf.Lerp (steerInput, horizontalAxis, Time.deltaTime * 10);
		}
		if (Mathf.Abs (motorInput) > 0.05f) {
			forwardInput = Mathf.Lerp (forwardInput, motorInput, Time.deltaTime * 10);
		} else {
			forwardInput = Mathf.Lerp (forwardInput, motorInput, Time.deltaTime * 10);
		}

		float left = 0;
		float right = 0;

		if ((forwardInput > 0.05f || forwardInput < -0.05f) && (steerInput < 0.05f && steerInput > -0.05f)) {
			left = right = forwardInput;
			//print("moving forward or backward");
		} else if (forwardInput > 0.05f && steerInput > 0) {
			left = forwardInput;
			right = -steerInput;
			//print("moving forward and to the right");
		} else if (forwardInput > 0.05f && steerInput < 0) {
			left = steerInput;
			right = forwardInput;
			//print("moving forward and to the left");
		} else if ((forwardInput < 0.05f && forwardInput > -0.05f) && steerInput > 0) {
			left = 0;
			right = steerInput;
			//print("moving to the right");
		} else if ((forwardInput < 0.05f && forwardInput > -0.05f) && steerInput < 0) {
			left = -steerInput;
			right = 0;
			//print("moving to the left");
		} else if (forwardInput < -0.05f && steerInput > 0) {
			left = 0;
			right = -steerInput;
			//print("moving backward and to the right");
		} else if (forwardInput < -0.05f && steerInput < 0) {
			left = steerInput;
			right = 0;
			//print("moving backward and to the left");
		}

		leftInput = Mathf.Lerp (leftInput, left, Time.deltaTime * 10);
		rightInput = Mathf.Lerp (rightInput, right, Time.deltaTime * 10);

		Vector3 rightHandLebarEuler = otherCarParts.rightHandLebar.transform.localEulerAngles;
		Vector3 lefttHandLebarEuler = otherCarParts.leftHandLebar.transform.localEulerAngles;

		otherCarParts.rightHandLebar.transform.localRotation = Quaternion.Euler (settings.steerAngleLimit * rightInput * 2, rightHandLebarEuler.y, rightHandLebarEuler.z);
		otherCarParts.leftHandLebar.transform.localRotation = Quaternion.Euler (settings.steerAngleLimit * leftInput * 2, lefttHandLebarEuler.y, lefttHandLebarEuler.z);

		//reset the vehicle rotation if it is upside down 
		if (currentSpeed < 5) {
			//check the current rotation of the vehicle with respect to the normal of the gravity normal component, which always point the up direction
			float angle = Vector3.Angle (currentNormal, transform.up);

			if (angle > 60 && !rotating) {

				resetTimer += Time.deltaTime;

				if (resetTimer > timeToFlip) {
					resetTimer = 0;
					StartCoroutine (rotateVehicle ());
				}
			}
		}
	}

	void FixedUpdate ()
	{
		if (!usingGravityControl && isTurnedOn) {
			surfaceDetected = false;

			if (increaseHeightIfSurfaceBelowFound) {

				if (Physics.Raycast (surfaceBelowRaycastPosition.position, -transform.up, out hit, raycastDistanceToCheckSurfaceBelow, settings.layer)) {
					if (hit.distance < minDistanceToIncreaseHeight) {
						increasingHeightFromSurfaceDetected = true;

						inputIncreaseHeightState (true);

					} else {
						if (increasingHeightFromSurfaceDetected) {
							inputIncreaseHeightState (false);

							increasingHeightFromSurfaceDetected = false;
						}
					}
				} else {
					if (increasingHeightFromSurfaceDetected) {
						inputIncreaseHeightState (false);

						increasingHeightFromSurfaceDetected = false;
					}
				}
			}

			if (Physics.Raycast (groundRaycastPosition.position, -transform.up, out hit, groundRaycastDistance, settings.layer)) {

				currentHitDistance = hit.distance;

				surfaceDetected = true;
			}

			if (surfaceDetected) {
				float proportionalHeight = (groundRaycastDistance - currentHitDistance) / groundRaycastDistance;
				appliedHoverForce = vehicleCameraTransform.up * (proportionalHeight * hoverForce) + ((Mathf.Cos (Time.time * extraHoverSpeed)) / 2) * transform.up;
				mainVehicleGravityControl.setGravityForcePausedState (false);

				groundFound = true;
			} else {

				if (isTurnedOn) {
					mainVehicleGravityControl.setGravityForcePausedState (true);
				} else {
					mainVehicleGravityControl.setGravityForcePausedState (false);
				}

				groundFound = false;
			}

			if (!groundFound) {
				if (!moving) {
					appliedHoverForce = ((Mathf.Cos (Time.time * extraHoverSpeed)) / 2) * hoverFoeceOnAir * transform.up;
				} else {
					appliedHoverForce = Vector3.zero;
				}
			}

			if (groundFound && heightInput < 0) {
				heightInput = 0;
			}

			Vector3 newVelocity = transform.forward * (motorInput * forwardSpeed) + transform.right * (horizontalAxis * rightSpeed) + transform.up * (heightInput * upSpeed);

			newVelocity += appliedHoverForce;

			newVelocity *= boostInput;

			mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, newVelocity, Time.deltaTime * velocityChangeSpeed);

			if (rollDirection != 0) {
				transform.Rotate (0, -rollDirection * rollRotationSpeed, 0);
			}

			Vector3 predictedUp = Quaternion.AngleAxis (mainRigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * stabilityForce / stabilitySpeed, mainRigidbody.angularVelocity) * transform.up;
			Vector3 torqueVector = Vector3.Cross (predictedUp, vehicleCameraTransform.up);
			mainRigidbody.AddTorque (torqueVector * (stabilitySpeed * stabilitySpeed));

			currentSpeed = mainRigidbody.velocity.magnitude;

			currenLeftEngineArmRotation = Vector3.forward * (heightInput * engineArmRotationAmountForward) +
			Vector3.right * (motorInput * engineArmRotationAmountRight) -
			Vector3.forward * (horizontalAxis * engineArmRotationAmountForward) -
			Vector3.right * (rollDirection * engineArmRotationAmountRight);

			currenLeftEngineArmRotation.x = 
				Mathf.Clamp (currenLeftEngineArmRotation.x, -engineArmRotationAmountRight - engineArmExtraRotationAmount, engineArmRotationAmountRight + engineArmExtraRotationAmount);
			currenLeftEngineArmRotation.z = 
				Mathf.Clamp (currenLeftEngineArmRotation.z, -engineArmRotationAmountForward - engineArmExtraRotationAmount, engineArmRotationAmountForward + engineArmExtraRotationAmount);

			leftEngineArmTransform.localRotation = Quaternion.Lerp (leftEngineArmTransform.localRotation, Quaternion.Euler (currenLeftEngineArmRotation), Time.deltaTime * engineArmRotationSpeed);


			currenRightEngineRotation = Vector3.forward * (-heightInput * engineArmRotationAmountForward) +
			Vector3.right * (motorInput * engineArmRotationAmountRight) -
			Vector3.forward * (horizontalAxis * engineArmRotationAmountForward) +
			Vector3.right * (rollDirection * engineArmRotationAmountRight);

			currenRightEngineRotation.x = 
				Mathf.Clamp (currenRightEngineRotation.x, -engineArmRotationAmountRight - engineArmExtraRotationAmount, engineArmRotationAmountRight + engineArmExtraRotationAmount);
			currenRightEngineRotation.z = 
				Mathf.Clamp (currenRightEngineRotation.z, -engineArmRotationAmountForward - engineArmExtraRotationAmount, engineArmRotationAmountForward + engineArmExtraRotationAmount);

			rightEngineArmTransform.localRotation = Quaternion.Lerp (rightEngineArmTransform.localRotation, Quaternion.Euler (currenRightEngineRotation), Time.deltaTime * engineArmRotationSpeed);

			rightEngineTransform.Rotate (0, engineRotationSpeed * Time.deltaTime, 0);

			leftEngineTransform.Rotate (0, engineRotationSpeed * Time.deltaTime, 0);
		}

		if (isTurnedOn) {
			checkVehicleTurnedOn = true;

			//set the exhaust particles state
			for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {
				if (isTurnedOn && currentSpeed < 20) {
					if (!otherCarParts.normalExhaust [i].isPlaying) {
						otherCarParts.normalExhaust [i].Play ();
					}
				} else {
					otherCarParts.normalExhaust [i].Stop ();
				}
			}

			for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {
				if (isTurnedOn && currentSpeed > 10 && motorInput > 0.1f) {
					if (!otherCarParts.heavyExhaust [i].isPlaying) {
						otherCarParts.heavyExhaust [i].Play ();
					}
				} else {
					otherCarParts.heavyExhaust [i].Stop ();
				}
			}
		} else {
			if (checkVehicleTurnedOn) {
				stopVehicleParticles ();
				checkVehicleTurnedOn = false;
			}
		}
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

		mainVehicleGravityControl.setGravityForcePausedState (false);
	}

	public override void setTurnOnState ()
	{
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, true, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, true, false);
	}

	public override void setTurnOffState (bool previouslyTurnedOn)
	{
		base.setTurnOffState (previouslyTurnedOn);

		if (previouslyTurnedOn) {
			setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, true);
			setAudioState (otherCarParts.engineAudio, 5, 1, otherCarParts.engineEndClip, false, true, false);
		}

		steerInput = 0;
	
		mainVehicleGravityControl.setGravityForcePausedState (false);
	}

	public override void setEngineOnOrOffState ()
	{
		base.setEngineOnOrOffState ();


	}

	public override void turnOnOrOff (bool state, bool previouslyTurnedOn)
	{
		base.turnOnOrOff (state, previouslyTurnedOn);

		mainVehicleGravityControl.setCheckDownSpeedActiveState (!isTurnedOn);
	}

	//the vehicle has been destroyed, so disabled every component in it
	public override void disableVehicle ()
	{
		//stop the audio sources
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);

		setTurnOffState (false);

		//disable the exhausts particles
		stopVehicleParticles ();

		//disable the controller
		this.enabled = false;
	}

	public void stopVehicleParticles ()
	{
		for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {
			otherCarParts.normalExhaust [i].Stop ();
		}

		for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {
			otherCarParts.heavyExhaust [i].Stop ();
		}

		for (int i = 0; i < otherCarParts.boostingParticles.Count; i++) {
			var boostingParticlesMain = otherCarParts.boostingParticles [i].main;
			boostingParticlesMain.loop = false;
		}
	}
		
	//reset the vehicle rotation if it is upside down
	IEnumerator rotateVehicle ()
	{
		rotating = true;

		Quaternion currentRotation = transform.rotation;

		//rotate in the forward direction of the vehicle
		Quaternion dstRotPlayer = Quaternion.LookRotation (transform.forward, currentNormal);
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;

			transform.rotation = Quaternion.Slerp (currentRotation, dstRotPlayer, t);
			mainRigidbody.velocity = Vector3.zero;
			yield return null;
		
		}
		rotating = false;
	}

	//if the vehicle is using the boost, set the boost particles
	public override void usingBoosting ()
	{
		base.usingBoosting ();

		for (int i = 0; i < otherCarParts.boostingParticles.Count; i++) {
			if (usingBoost) {
				if (!otherCarParts.boostingParticles [i].isPlaying) {
					otherCarParts.boostingParticles [i].gameObject.SetActive (true);
					otherCarParts.boostingParticles [i].Play ();

					var boostingParticlesMain = otherCarParts.boostingParticles [i].main;
					boostingParticlesMain.loop = true;
				}
			} else {
				if (otherCarParts.boostingParticles [i].isPlaying) {
					var boostingParticlesMain = otherCarParts.boostingParticles [i].main;
					boostingParticlesMain.loop = false;
				}
			}
		}
	}

	//OVERRIDE FUNCTIONS FOR VEHICLE CONTROLLER

	//if any collider in the vehicle collides, then
	public override void setCollisionDetected (Collision currentCollision)
	{
		//check that the collision is not with the player
		if (!currentCollision.gameObject.CompareTag ("Player")) {
			//if the velocity of the collision is higher that the limit
			if (currentCollision.relativeVelocity.magnitude > collisionForceLimit) {
				//set the collision audio with a random collision clip
				if (otherCarParts.crashClips.Length > 0) {
					setAudioState (otherCarParts.crashAudio, 5, 1, otherCarParts.crashClips [UnityEngine.Random.Range (0, otherCarParts.crashClips.Length)], false, true, false);
				}
			}
		}
	}

	public override void startBrakeVehicleToStopCompletely ()
	{

	}

	public override void endBrakeVehicleToStopCompletely ()
	{
		
	}

	public override float getCurrentSpeed ()
	{
		return currentSpeed;
	}


	//CALL INPUT FUNCTIONS
	public override void inputHoldOrReleaseTurbo (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			//boost input
			if (holdingButton) {
				usingBoost = true;

				//set the camera move away action
				mainVehicleCameraController.usingBoost (true, vehicleControllerSettings.boostCameraShakeStateName, 
					vehicleControllerSettings.useBoostCameraShake, vehicleControllerSettings.moveCameraAwayOnBoost);
			} else {
				//stop boost
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

	public override void inputHorn ()
	{
		if (driving) {
			setAudioState (otherCarParts.hornAudio, 5, 1, otherCarParts.hornClip, false, true, false);
		}
	}

	public void inputIncreaseHeightState (bool state)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			if (state) {
				heightInput = 1;
			} else {
				heightInput = 0;
			}
		}
	}

	public void inputDecreaseHeightState (bool state)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			if (state) {
				heightInput = -1;
			} else {
				heightInput = 0;
			}
		}
	}

	public void inputSetRotateToLeftState (bool state)
	{
		if (driving && isTurnedOn) {
			if (state) {
				rollDirection = 1;
			} else {
				rollDirection = 0;
			}

			usingRollInput = state;
		}
	}

	public void inputSetRotateToRightState (bool state)
	{
		if (driving && isTurnedOn) {
			if (state) {
				rollDirection = -1;
			} else {
				rollDirection = 0;
			}

			usingRollInput = state;
		}
	}

	[System.Serializable]
	public class otherVehicleParts
	{
		public Transform rightHandLebar;
		public Transform leftHandLebar;
		public Transform COM;
		public GameObject chassis;
		public AudioClip engineStartClip;
		public AudioClip engineClip;
		public AudioClip engineEndClip;
		public AudioClip[] crashClips;
		public AudioClip hornClip;
		public List<ParticleSystem> normalExhaust = new List<ParticleSystem> ();
		public List<ParticleSystem> heavyExhaust = new List<ParticleSystem> ();
		public AudioSource engineStartAudio;
		public AudioSource engineAudio;
		public AudioSource crashAudio;
		public AudioSource hornAudio;
		public List<ParticleSystem> boostingParticles = new List<ParticleSystem> ();
	}

	[System.Serializable]
	public class vehicletSettings
	{
		public LayerMask layer;
		public float steerAngleLimit;
		public float increaseHeightSpeed = 2;
		public float decreaseHeightSpeed = 2;
	}
}