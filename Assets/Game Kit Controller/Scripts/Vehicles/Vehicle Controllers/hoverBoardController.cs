using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class hoverBoardController : vehicleController
{
	[Header ("Custom Settings")]
	[Space]

	public List<hoverEngineSettings> hoverEngineList = new List<hoverEngineSettings> ();
	public OtherCarParts otherCarParts;
	public hoverCraftSettings settings;

	public float stabilityForce = 1;
	public float stabilitySpeed = 2;

	public float minSteerInputIdle = 0.4f;
	public float minSteerInputMoving = 0.4f;

	float currentMinSteerInput;

	public hoverBoardAnimationSystem mainHoverBoardAnimationSystem;

	[HideInInspector] public bool firstPersonActive;

	float audioPower = 0;
	float maxEnginePower;

	float resetTimer;

	float originalJumpPower;

	int i;

	int collisionForceLimit = 5;


	bool anyOnGround;
	bool rotating;

	bool usingHoverBoardWaypoint;

	float lastTimeReleasedFromWaypoint;

	hoverBoardWayPoints wayPointsManager;

	Vector3 gravityForce;

	hoverEngineSettings currentEngine;

	int hoverEngineListCount;

	ParticleSystem currentParticleSystem;

	Vector3 transformForward;
	Vector3 transformUp;

	public override void Start ()
	{
		base.Start ();

		//get the boost particles inside the vehicle
		hoverEngineListCount = hoverEngineList.Count;

		for (i = 0; i < otherCarParts.boostingParticles.Count; i++) {
			otherCarParts.boostingParticles [i].gameObject.SetActive (false);
		}

		for (i = 0; i < hoverEngineList.Count; i++) {
			currentEngine = hoverEngineList [i];

			currentEngine.hasTurbine = currentEngine.turbine != null;

			currentEngine.hasParticles = currentEngine.ParticleSystem != null;
		}

		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, false, false);

		otherCarParts.gravityCenterCollider.enabled = false;
		originalJumpPower = vehicleControllerSettings.jumpPower;
	}

	public override void vehicleUpdate ()
	{
		base.vehicleUpdate ();


		mainRigidbody.centerOfMass = settings.centerOfMassOffset;

		maxEnginePower = 0;

		for (i = 0; i < hoverEngineListCount; i++) {
			currentEngine = hoverEngineList [i];

			if (currentEngine.maxEnginePower > maxEnginePower) {
				maxEnginePower = currentEngine.maxEnginePower;
			}

			//configure every particle system according to the engine state
			float rpm = Mathf.Lerp (currentEngine.minRPM, currentEngine.maxRPM, currentEngine.maxEnginePower);

			if (currentEngine.hasTurbine) {
				currentEngine.turbine.Rotate (0, rpm * Time.deltaTime * 6, 0);
			}

			if (currentEngine.hasParticles) {
				var hoverEngineParticleEmission = currentEngine.ParticleSystem.emission;
				hoverEngineParticleEmission.rateOverTime = currentEngine.maxEmission * currentEngine.maxEnginePower;
			
				currentEngine.ParticleSystem.transform.position = currentEngine.hit.point + currentEngine.hit.normal * currentEngine.dustHeight;
				currentEngine.ParticleSystem.transform.LookAt (currentEngine.hit.point + currentEngine.hit.normal * 10);
			}
		}

		audioPower = Mathf.Lerp (maxEnginePower, motorInput, settings.audioEngineSpeed);

		otherCarParts.engineAudio.volume = Mathf.Lerp (settings.engineMinVolume, settings.engineMaxVolume, audioPower);
		otherCarParts.engineAudio.pitch = Mathf.Lerp (settings.minAudioPitch, settings.maxAudioPitch, audioPower);

		//reset the vehicle rotation if it is upside down 
		if (currentSpeed < 5) {
			//check the current rotation of the vehicle with respect to the normal of the gravity normal component, which always point the up direction
			float angle = Vector3.Angle (currentNormal, transform.up);

			if (angle > 60 && !rotating) {
				resetTimer += Time.deltaTime;

				if (resetTimer > settings.timeToFlip) {
					resetTimer = 0;

					StartCoroutine (rotateVehicle ());
				}
			} else {
				resetTimer = 0;
			}
		}
	}

	void FixedUpdate ()
	{
		currentSpeed = mainRigidbody.velocity.magnitude;

		//apply turn
		if (usingHoverBoardWaypoint) {
			return;
		}

		if (Mathf.Approximately (horizontalAxis, 0)) {
			float localR = Vector3.Dot (mainRigidbody.angularVelocity, transform.up);

			mainRigidbody.AddRelativeTorque (0, -localR * settings.brakingTorque, 0);
		} else {
			float targetRoll = -settings.rollOnTurns * horizontalAxis;
			float roll = Mathf.Asin (transform.right.y) * Mathf.Rad2Deg;

			// only apply additional roll if we're not "overrolled"
			if (Mathf.Abs (roll) > Mathf.Abs (targetRoll)) {
				roll = 0;
			} else {
				roll = Mathf.DeltaAngle (roll, targetRoll);
			}

			mainRigidbody.AddRelativeTorque (0, horizontalAxis * settings.steeringTorque, roll * settings.rollOnTurnsTorque);
		}

		if (!usingGravityControl && !jumpInputPressed) {
			Vector3 localVelocity = transform.InverseTransformDirection (mainRigidbody.velocity);
			Vector3 extraForce = Vector3.Scale (settings.extraRigidbodyForce, localVelocity);

			mainRigidbody.AddRelativeForce (-extraForce * mainRigidbody.mass);

			//use every engine to keep the vehicle in the air
			for (i = 0; i < hoverEngineListCount; i++) {
				currentEngine = hoverEngineList [i];

				if (!currentEngine.mainEngine) {
					//find force direction by rotating local up vector towards world up
					Vector3 engineUp = currentEngine.engineTransform.up;

					gravityForce = (currentNormal * 9.8f).normalized;

					engineUp = Vector3.RotateTowards (engineUp, gravityForce, currentEngine.maxEngineAngle * Mathf.Deg2Rad, 1);

					//check if the vehicle is on ground
					currentEngine.maxEnginePower = 0;

					if (Physics.Raycast (currentEngine.engineTransform.position, -engineUp, out currentEngine.hit, currentEngine.maxHeight, settings.layer)) {
						//calculate down force
						currentEngine.maxEnginePower = Mathf.Pow ((currentEngine.maxHeight - currentEngine.hit.distance) / currentEngine.maxHeight, currentEngine.Exponent);

						float force = currentEngine.maxEnginePower * currentEngine.engineForce;
						float velocityUp = Vector3.Dot (mainRigidbody.GetPointVelocity (currentEngine.engineTransform.position), engineUp);
						float drag = -velocityUp * Mathf.Abs (velocityUp) * currentEngine.damping;

						mainRigidbody.AddForceAtPosition (engineUp * (force + drag), currentEngine.engineTransform.position);
					}
				}
			}
				
			Vector3 torqueVector = Vector3.Cross (transform.up, mainVehicleCameraController.transform.up);

			mainRigidbody.AddTorque (torqueVector * (stabilityForce * stabilitySpeed));
	
			//if the handbrake is pressed, set the brake torque value in every wheel
			if (braking) {
				for (i = 0; i < hoverEngineListCount; i++) {
					currentEngine = hoverEngineList [i];

					if (currentEngine.mainEngine) {
						mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, Vector3.zero, Time.deltaTime);
					}
				}
			} else {
				transformForward = transform.forward;
				transformUp = transform.up;

				for (i = 0; i < hoverEngineListCount; i++) {
					currentEngine = hoverEngineList [i];

					if (currentEngine.mainEngine) {
						float movementMultiplier = settings.inAirMovementMultiplier;

						if (Physics.Raycast (currentEngine.engineTransform.position, -transformUp, out currentEngine.hit, currentEngine.maxHeight, settings.layer)) {
							movementMultiplier = 1;
						} 

						gravityForce = (currentNormal * 9.8f).normalized;

						//current speed along forward axis
						float speed = Vector3.Dot (mainRigidbody.velocity, transformForward);

						//if the vehicle doesn't move by input, apply automatic brake 
						bool isAutoBraking = Mathf.Approximately (motorInput, 0) && settings.autoBrakingDeceleration > 0;

						float thrust = motorInput;

						if (isAutoBraking) {
							thrust = -Mathf.Sign (speed) * settings.autoBrakingDeceleration / settings.maxBrakingDeceleration;
						}

						//check if it is braking, for example speed and thrust have opposing signs
						bool isBraking = motorInput * speed < 0;

						//don't apply force if speed is max already
						if (Mathf.Abs (speed) < settings.maxSpeed || isBraking) {
							//position on speed curve
							float normSpeed = Mathf.Sign (motorInput) * speed / settings.maxSpeed;

							//apply acceleration curve and select proper maximum value
							float acc = settings.accelerationCurve.Evaluate (normSpeed) * (isBraking ? settings.maxBrakingDeceleration : thrust > 0 ? settings.maxForwardAcceleration : settings.maxReverseAcceleration);

							//drag should be added to the acceleration
							float sdd = speed * settings.extraRigidbodyForce.z;
							float dragForce = sdd + mainRigidbody.drag * speed;
							float force = acc * thrust + dragForce;

							//reduce acceleration if the vehicle is close to vertical orientation and is trrying to go higher
							float y = Vector3.Dot (transformForward, gravityForce);

							if (settings.maxSurfaceAngle < 90 && y * thrust > 0) {
								if (!isAutoBraking) {
									float pitch2 = Mathf.Asin (Mathf.Abs (y)) * Mathf.Rad2Deg;

									if (pitch2 > settings.maxSurfaceAngle) {
										float forceDecrease = (pitch2 - settings.maxSurfaceAngle) / (90 - settings.maxSurfaceAngle) * settings.maxSurfaceVerticalReduction;

										force /= 1 + forceDecrease;
									}
								}
							}

							mainRigidbody.AddForce (transformForward * (force * boostInput * movementMultiplier), ForceMode.Acceleration);
						}
					}
				}
			}
		}

		anyOnGround = true;

		int totalWheelsOnAir = 0;

		for (i = 0; i < hoverEngineListCount; i++) {
			currentEngine = hoverEngineList [i];

			if (!Physics.Raycast (currentEngine.engineTransform.position, -currentEngine.engineTransform.up, out currentEngine.hit, currentEngine.maxHeight, settings.layer)) {
				totalWheelsOnAir++;
			}
		}

		//if the total amount of wheels in the air is equal to the number of wheel sin the vehicle, anyOnGround is false
		if (totalWheelsOnAir == hoverEngineListCount && anyOnGround) {
			anyOnGround = false;
		}
	}

	IEnumerator jumpCoroutine ()
	{
		jumpInputPressed = true;

		yield return new WaitForSeconds (0.5f);

		jumpInputPressed = false;
	}

	public void enterOrExitFromWayPoint (bool state)
	{
		usingHoverBoardWaypoint = state;

		mainVehicleGravityControl.enabled = !state;

		mainRigidbody.isKinematic = state;

		if (usingHoverBoardWaypoint) {
			lastTimeReleasedFromWaypoint = 0;
		} else {
			lastTimeReleasedFromWaypoint = Time.time;
		}
	}

	public float getLastTimeReleasedFromWaypoint ()
	{
		return lastTimeReleasedFromWaypoint;
	}

	public bool isUsingHoverBoardWaypoint ()
	{
		return usingHoverBoardWaypoint;
	}


	public void receiveWayPoints (hoverBoardWayPoints wayPoints)
	{
		wayPointsManager = wayPoints;
	}

	public override void updateCameraSteerState ()
	{
		if (localLook.z < 0f) {
			localLook.x = Mathf.Sign (localLook.x);
		}

		steering = localLook.x;

		steering = Mathf.Clamp (steering, -1f, 1f);

		if (axisValues.y != 0) {
			currentMinSteerInput = minSteerInputMoving;
		} else {
			currentMinSteerInput = minSteerInputIdle;
		}

		if (Mathf.Abs (steering) > currentMinSteerInput) {
			horizontalAxis = steering;
		} else {
			horizontalAxis = 0;
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

		otherCarParts.gravityCenterCollider.enabled = driving;

		mainHoverBoardAnimationSystem.changeVehicleState (driving);
	}

	public override void setTurnOnState ()
	{
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, true, false);
	}

	public override void setTurnOffState (bool previouslyTurnedOn)
	{
		base.setTurnOffState (previouslyTurnedOn);

		if (previouslyTurnedOn) {
			setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, true);
		}
	}

	public override void turnOnOrOff (bool state, bool previouslyTurnedOn)
	{
		base.turnOnOrOff (state, previouslyTurnedOn);


	}

	public override void setEngineOnOrOffState ()
	{
		base.setEngineOnOrOffState ();


	}

	//the vehicle has been destroyed, so disabled every component in it
	public override void disableVehicle ()
	{
		//stop the audio sources
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, false);

		setTurnOffState (false);

		otherCarParts.gravityCenterCollider.enabled = false;

		//disable the controller
		this.enabled = false;

		mainHoverBoardAnimationSystem.changeVehicleState (false);
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
			currentParticleSystem = otherCarParts.boostingParticles [i];

			if (usingBoost) {
				if (!currentParticleSystem.isPlaying) {
					if (!currentParticleSystem.gameObject.activeSelf) {
						currentParticleSystem.gameObject.SetActive (true);
					}

					currentParticleSystem.Play ();

					var boostingParticlesMain = currentParticleSystem.main;
					boostingParticlesMain.loop = true;
				}
			} else {
				if (currentParticleSystem.isPlaying) {
					var boostingParticlesMain = currentParticleSystem.main;
					boostingParticlesMain.loop = false;
				}
			}
		}
	}

	//use a jump platform
	public void useVehicleJumpPlatform (Vector3 direction)
	{
		StartCoroutine (jumpCoroutine ());

		mainRigidbody.AddForce (mainRigidbody.mass * direction, ForceMode.Impulse);
	}

	public void useJumpPlatformParable (Vector3 direction)
	{
		Vector3 jumpForce = direction;

		mainRigidbody.AddForce (jumpForce, ForceMode.VelocityChange);
	}

	public void setNewJumpPower (float newJumpPower)
	{
		vehicleControllerSettings.jumpPower = newJumpPower;
	}

	public void setOriginalJumpPower ()
	{
		vehicleControllerSettings.jumpPower = originalJumpPower;
	}

	public void setCanJumpState (bool state)
	{
		vehicleControllerSettings.canJump = state;
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
		braking = true;
	}

	public override void endBrakeVehicleToStopCompletely ()
	{
		braking = false;
	}

	public override float getCurrentSpeed ()
	{
		return currentSpeed;
	}


	//CALL INPUT FUNCTIONS
	public override void inputJump ()
	{
		if (driving && !usingGravityControl && isTurnedOn && vehicleControllerSettings.canJump) {
			if (anyOnGround) {
				if (!jumpInputPressed && anyOnGround) {
					StartCoroutine (jumpCoroutine ());

					mainRigidbody.AddForce (currentNormal * mainRigidbody.mass * vehicleControllerSettings.jumpPower, ForceMode.Impulse);
				}
			}

			if (usingHoverBoardWaypoint) {
				StartCoroutine (jumpCoroutine ());

				wayPointsManager.pickOrReleaseVehicle (false, false);

				mainRigidbody.AddForce ((currentNormal + transform.forward) * mainRigidbody.mass * vehicleControllerSettings.jumpPower, ForceMode.Impulse);
			}
		}
	}

	public override void inputHoldOrReleaseTurbo (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn && !usingHoverBoardWaypoint) {
			//boost input
			if (holdingButton) {
				if (vehicleControllerSettings.canUseBoost) {
					usingBoost = true;

					//set the camera move away action
					mainVehicleCameraController.usingBoost (true, vehicleControllerSettings.boostCameraShakeStateName, 
						vehicleControllerSettings.useBoostCameraShake, vehicleControllerSettings.moveCameraAwayOnBoost);
				}
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

	public override void inputHoldOrReleaseBrake (bool holdingButton)
	{
		if (driving && !usingGravityControl) {
			braking = holdingButton;
		}
	}

	[System.Serializable]
	public class hoverEngineSettings
	{
		public string Name;
		public Transform engineTransform;
		public ParticleSystem ParticleSystem;
		public float maxEmission = 100;
		public float dustHeight = 0.1f;
		public float maxHeight = 2;
		public float engineForce = 300;
		public float damping = 10;
		public float Exponent = 2;
		public float maxEngineAngle = 15;
		public bool mainEngine;
		public float minRPM = 100;
		public float maxRPM = 200;
		public Transform turbine;
		[HideInInspector] public RaycastHit hit;
		[HideInInspector] public float maxEnginePower;

		[HideInInspector] public bool hasTurbine;
		[HideInInspector] public bool hasParticles;
	}

	[System.Serializable]
	public class OtherCarParts
	{
		public Transform COM;
		public GameObject chassis;
		public AudioClip engineClip;
		public AudioClip[] crashClips;
		public AudioSource engineAudio;
		public AudioSource crashAudio;
		public List<ParticleSystem> boostingParticles = new List<ParticleSystem> ();
		public Collider gravityCenterCollider;
	}

	[System.Serializable]
	public class hoverCraftSettings
	{
		public LayerMask layer;
		public float steeringTorque = 120;
		public float brakingTorque = 200;
		public float maxSpeed = 30;
		public float maxForwardAcceleration = 20;
		public float maxReverseAcceleration = 15;
		public float maxBrakingDeceleration = 30;
		public float autoBrakingDeceleration = 20;
		public float rollOnTurns = 10;
		public float rollOnTurnsTorque = 10;
		public float timeToFlip = 2;
		public float audioEngineSpeed = 0.5f;
		public float engineMinVolume = 0.5f;
		public float engineMaxVolume = 1;
		public float minAudioPitch = 0.4f;
		public float maxAudioPitch = 1;
		public AnimationCurve accelerationCurve;
		public float maxSurfaceVerticalReduction = 10;
		public float maxSurfaceAngle = 110;
		public Vector3 extraRigidbodyForce = new Vector3 (2, 0.1f, 0.2f);
		public Vector3 centerOfMassOffset;
		[Range (0, 1)] public float inAirMovementMultiplier;
	}
}