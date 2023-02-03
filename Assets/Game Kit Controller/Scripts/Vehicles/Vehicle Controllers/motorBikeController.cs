using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class motorBikeController : vehicleController
{
	[Header ("Custom Settings")]
	[Space]

	public List<Wheels> wheelsList = new List<Wheels> ();
	public List<Gears> gearsList = new List<Gears> ();
	public OtherCarParts otherCarParts;
	public motorBikeSettings settings;

	public float stabilitySpeed = 220;
	public float stabilityForce = 36;

	public int currentGear;
	public float currentRPM = 0;

	[Space]
	[Header ("IK and Animation Settings")]
	[Space]

	public bool placeDriverFeetOnGroundOnLowSpeed;
	public float lowSpeedChassisRotationAmount = 20;
	public float minimumLowSpeed = 5;
	public float chassisRotationSpeed = 1;
	public Transform rightRaycastPositionToFeet;
	public Transform leftRaycastPositionToFeet;

	public Vector3 footOnGroundPositionOffset;

	public float adjustFeetToGroundSpeed = 4;
	public Transform rightFootTransform;
	public Transform lefFootTransform;
	public Transform originalRightFootPosition;
	public Transform originalLeftFootPosition;

	public Transform footPositionOnGroundParent;
	public Transform footPositionOnGround;

	public bool useStandAnimation;
	public string standAnimationName;
	public Animation mainAnimation;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool anyOnGround;

	[Space]
	[Header ("Components")]
	[Space]

	public skidsManager skidMarksManager;

	bool useRightFootForGround;
	Vector3 raycastPositionFoundForFeet;

	bool lowSpeedDetected;
	bool previoslyLowSpeedDetected;

	bool adjustChassisLean = true;

	List<ParticleSystem> boostingParticles = new List<ParticleSystem> ();

	bool reversing;

	int i;
	int collisionForceLimit = 10;
	float horizontalLean = 0;
	float verticalLean = 0;
	float steerInput = 0;

	float defSteerAngle = 0;


	float originalJumpPower;
	Wheels frontWheel;
	Wheels rearWheel;
	RaycastHit hit;

	Vector3 ColliderCenterPoint;

	WheelCollider currentWheelCollider;
	Transform currentWheelTransform;
	Transform currentWheelMeshTransform;

	WheelHit wheelGroundHit;
	Quaternion newRotation;
	float normalizedLeanAngle;
	Vector3 currentAngularVelocity;

	WheelHit wheelGroundHitFront;
	WheelHit wheelGroundHitRear;

	int totalWheelsOnAir;

	bool checkVehicleTurnedOn;
	float speedMultiplier;

	Vector3 predictedUp;
	Vector3 torqueVector;

	float currentSkidsVolume;

	Wheels currentWheel;

	int wheelsListCount;

	int gearListCount;

	ParticleSystem currentParticleSystem;

	public override void Start ()
	{
		base.Start ();

		//set every wheel slip smoke particles and get the front and the rear wheel
		wheelsListCount = wheelsList.Count;

		gearListCount = gearsList.Count;

		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			if (currentWheel.wheelSide == wheelType.front) {
				frontWheel = currentWheel;
			}

			if (currentWheel.wheelSide == wheelType.rear) {
				rearWheel = currentWheel;
			}

			currentWheel.hasMudGuard = currentWheel.mudGuard != null;

			currentWheel.hasSuspension = currentWheel.suspension != null;

			currentWheel.hasParticles = currentWheel.wheelParticleSystem != null;
		}

		//set the sound components
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, false, false);
		setAudioState (otherCarParts.skidAudio, 5, 0, otherCarParts.skidClip, true, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);

		mainRigidbody.centerOfMass = new Vector3 (otherCarParts.COM.localPosition.x * transform.localScale.x, 
			otherCarParts.COM.localPosition.y * transform.localScale.y, 
			otherCarParts.COM.localPosition.z * transform.localScale.z);
		mainRigidbody.maxAngularVelocity = 2;

		//store the max steer angle
		defSteerAngle = settings.steerAngleLimit;

		//get the boost particles inside the vehicle
		if (otherCarParts.boostParticles) {
			Component[] boostParticlesComponents = otherCarParts.boostParticles.GetComponentsInChildren (typeof(ParticleSystem));
			foreach (ParticleSystem child in boostParticlesComponents) {
				boostingParticles.Add (child);

				child.gameObject.SetActive (false);
			}
		}
			
		originalJumpPower = vehicleControllerSettings.jumpPower;
	}

	public override void vehicleUpdate ()
	{
		base.vehicleUpdate ();

		//change gear
		if (!changingGear && !usingGravityControl) {
			if (currentGear + 1 < gearListCount) {
				if (currentSpeed >= gearsList [currentGear].gearSpeed && rearWheel.wheelCollider.rpm >= 0) {
					StartCoroutine (changeGear (currentGear + 1));
				}
			}

			if (currentGear - 1 >= 0) {
				if (currentSpeed < gearsList [currentGear - 1].gearSpeed) {
					StartCoroutine (changeGear (currentGear - 1));
				}
			}

			//set the current gear to 0 if the velocity is too low
			if (currentSpeed < 5 && currentGear > 1) {
				StartCoroutine (changeGear (0));
			}
		}

		//check every wheel collider of the vehicle, to move it and apply rotation to it correctly using raycast
		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheelCollider = currentWheel.wheelCollider;
			currentWheelTransform = currentWheelCollider.transform;
			currentWheelMeshTransform = currentWheel.wheelMesh.transform;

			//get the center position of the wheel
			ColliderCenterPoint = currentWheelTransform.TransformPoint (currentWheelCollider.center);
			//use a raycast in the ground direction
			currentWheelCollider.GetGroundHit (out wheelGroundHit);

			//if the wheel is close enough to the ground, then
			if (Physics.Raycast (ColliderCenterPoint, -currentWheelTransform.up, out hit, 
				    (currentWheelCollider.suspensionDistance + currentWheelCollider.radius) * transform.localScale.y, settings.layer)) {
				//set the wheel mesh position according to the values of the wheel collider
				currentWheelMeshTransform.position = hit.point + (currentWheelCollider.radius * transform.localScale.y) * currentWheelTransform.up;
			} 

			//the wheel is in the air
			else {
				//set the wheel mesh position according to the values of the wheel collider
				currentWheelMeshTransform.position = ColliderCenterPoint - (currentWheelCollider.suspensionDistance * transform.localScale.y) * currentWheelTransform.up;
			}

			//if the current wheel is the front one,rotate the steering handlebar according to the wheel collider steerAngle
			if (currentWheel.wheelSide == wheelType.front) {
				otherCarParts.steeringHandlebar.transform.rotation = currentWheelTransform.rotation * Quaternion.Euler (0, currentWheelCollider.steerAngle, currentWheelTransform.rotation.z);
			}

			//if the wheel has a mudguard
			if (currentWheel.hasMudGuard) {
				//rotate the mudguard according to that rotation
				currentWheel.mudGuard.transform.position = currentWheelMeshTransform.position;
			}

			//if the wheel has suspension, set its rotation according to the wheel position
			if (currentWheel.hasSuspension) {
				newRotation = Quaternion.LookRotation (currentWheel.suspension.transform.position - currentWheelMeshTransform.position, currentWheel.suspension.transform.up);
				currentWheel.suspension.transform.rotation = newRotation;
			}

			//set the rotation value in the wheel collider
			currentWheel.rotationValue += currentWheelCollider.rpm * (6) * Time.deltaTime;

			//rotate the wheel mesh only according to the current speed 
			currentWheelMeshTransform.rotation = currentWheelTransform.rotation * Quaternion.Euler (currentWheel.rotationValue, currentWheelCollider.steerAngle, currentWheelTransform.rotation.z);
		}

		currentAngularVelocity = mainRigidbody.angularVelocity;

		//rotate the vehicle chassis when the gear is being changed
		//get the vertical lean value
		verticalLean = Mathf.Clamp (Mathf.Lerp (verticalLean, transform.InverseTransformDirection (currentAngularVelocity).x * settings.chassisLean.y, 
			Time.deltaTime * 5), -settings.chassisLeanLimit.y, settings.chassisLeanLimit.y);
		
		frontWheel.wheelCollider.GetGroundHit (out wheelGroundHit);
		normalizedLeanAngle = Mathf.Clamp (wheelGroundHit.sidewaysSlip, -1, 1);	

		if (transform.InverseTransformDirection (mainRigidbody.velocity).z > 0) {
			normalizedLeanAngle = -1;
		} else {
			normalizedLeanAngle = 1;
		}

		if (placeDriverFeetOnGroundOnLowSpeed) {
			if (currentSpeed < minimumLowSpeed && anyOnGround) {
				adjustChassisLean = false;

				lowSpeedDetected = true;
			} else {
				lowSpeedDetected = false;
			}

			checkDriverFootPosition ();
		} else {
			adjustChassisLean = true;
		}
			
		//get the horizontal lean value
		horizontalLean = Mathf.Clamp (Mathf.Lerp (horizontalLean, (transform.InverseTransformDirection (currentAngularVelocity).y * normalizedLeanAngle) *
		settings.chassisLean.x, Time.deltaTime * 3), -settings.chassisLeanLimit.x, settings.chassisLeanLimit.x);

		Quaternion target = Quaternion.Euler (verticalLean, otherCarParts.chassis.transform.localRotation.y + (currentAngularVelocity.z), horizontalLean);

		if (adjustChassisLean) {
			//set the lean rotation value in the chassis transform
			otherCarParts.chassis.transform.localRotation = target;
		}

		//set the vehicle mass center
		mainRigidbody.centerOfMass = new Vector3 ((otherCarParts.COM.localPosition.x) * transform.lossyScale.x, 
			(otherCarParts.COM.localPosition.y) * transform.lossyScale.y, 
			(otherCarParts.COM.localPosition.z) * transform.localScale.z);
	}

	void FixedUpdate ()
	{
		//get the current speed value
		currentSpeed = mainRigidbody.velocity.magnitude * 3.6f;
	
		//allows vehicle to remain roughly pointing in the direction of travel
		if (!anyOnGround && settings.preserveDirectionWhileInAir && currentSpeed > 5) {
			float velocityDirection = Vector3.Dot (mainRigidbody.velocity, currentNormal);

			if (velocityDirection > -20) {
				float angleX = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (currentNormal.normalized, transform.up)).x) * Mathf.Rad2Deg;

				transform.eulerAngles -= (angleX * Time.deltaTime) * transform.InverseTransformDirection (transform.right);
			}
		}

		steerInput = Mathf.Lerp (steerInput, horizontalAxis, Time.deltaTime * settings.steerInputSpeed);

		//set the steer limit
		settings.steerAngleLimit = Mathf.Lerp (defSteerAngle, settings.highSpeedSteerAngle, (currentSpeed / settings.highSpeedSteerAngleAtSpeed));

		//set the steer angle in the fron wheel
		frontWheel.wheelCollider.steerAngle = settings.steerAngleLimit * steerInput;

		//set the current RPM
		currentRPM = Mathf.Clamp ((((Mathf.Abs ((frontWheel.wheelCollider.rpm + rearWheel.wheelCollider.rpm)) * settings.gearShiftRate) + settings.minRPM)) /
		(currentGear + 1), settings.minRPM, settings.maxRPM);

		//check if the vehicle is moving forwards or backwards
		if (motorInput < 0) { 
			reversing = true;
		} else {
			reversing = false;
		}

		//set the engine audio volume and pitch according to input and current RPM
		if (!vehicleDestroyed) {
			otherCarParts.engineAudio.volume = Mathf.Lerp (otherCarParts.engineAudio.volume, Mathf.Clamp (motorInput, 0.35f, 0.85f), Time.deltaTime * 5);

			otherCarParts.engineAudio.pitch = 
				Mathf.Lerp (otherCarParts.engineAudio.pitch, Mathf.Lerp (1, 2, (currentRPM - (settings.minRPM / 1.5f)) / (settings.maxRPM + settings.minRPM)), Time.deltaTime * 5);
		}

		if (otherCarParts.engineStartAudio != null) {
			if (otherCarParts.engineStartAudio.volume > 0) {
				otherCarParts.engineStartAudio.volume -= Time.deltaTime / 5;
			}
		}

		//if the current speed is higher that the max speed, stop apply motor torque to the powered wheel
		if (currentSpeed > vehicleControllerSettings.maxForwardSpeed || usingGravityControl) {
			rearWheel.wheelCollider.motorTorque = 0;
		}

		//else if the vehicle is moving in fowards direction, apply motor torque to the powered wheel using the gear animation curve
		else if (!reversing && !changingGear) {
			speedMultiplier = 1;

			if (settings.useCurves) {
				speedMultiplier = gearsList [currentGear].engineTorqueCurve.Evaluate (currentSpeed);
			}

			rearWheel.wheelCollider.motorTorque = settings.engineTorque * Mathf.Clamp (motorInput, 0, 1) * boostInput * speedMultiplier;
		}

		//if the vehicle is moving backwards, apply motor torque to every powered wheel
		if (reversing) {
			//if the current speed is lower than the maxBackWardSpeed, apply motor torque
			if (currentSpeed < settings.maxBackwardSpeed && Mathf.Abs (rearWheel.wheelCollider.rpm / 2) < 3000) {
				rearWheel.wheelCollider.motorTorque = settings.engineTorque * motorInput;
			} 
			//else, stop adding motor torque
			else {
				rearWheel.wheelCollider.motorTorque = 0;
			}
		}

		//if the handbrake is pressed, set the brake torque value in every wheel
		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			if (braking) {
				if (currentWheel.wheelSide == wheelType.front) {
					currentWheel.wheelCollider.brakeTorque = settings.brake * 5;
				}

				if (currentWheel.wheelSide == wheelType.rear) {
					currentWheel.wheelCollider.brakeTorque = settings.brake * 25;
				}
			} else {
				//else, check if the vehicle input is in forward or in backward direction
				//the vehicle is decelerating
				if (Mathf.Abs (motorInput) <= 0.05f && !changingGear) {
					currentWheel.wheelCollider.brakeTorque = settings.brake / 25;
				} 
				//the vehicle is braking
				else if (motorInput < 0 && !reversing) {
					if (currentWheel.wheelSide == wheelType.front) {
						currentWheel.wheelCollider.brakeTorque = settings.brake * (Mathf.Abs (motorInput) / 5);
					}

					if (currentWheel.wheelSide == wheelType.rear) {
						currentWheel.wheelCollider.brakeTorque = settings.brake * (Mathf.Abs (motorInput));
					}
				} else {
					currentWheel.wheelCollider.brakeTorque = 0;
				}
			}
		}

		//check the right front and right rear wheel to play the skid audio according to their state
		frontWheel.wheelCollider.GetGroundHit (out wheelGroundHitFront);
		rearWheel.wheelCollider.GetGroundHit (out wheelGroundHitRear);

		currentSkidsVolume = otherCarParts.skidAudio.volume;

		if (anyOnGround) {
			//if the values in the wheel hit are higher that
			if (Mathf.Abs (wheelGroundHitFront.sidewaysSlip) > 0.25f || Mathf.Abs (wheelGroundHitRear.forwardSlip) > 0.5f || Mathf.Abs (wheelGroundHitFront.forwardSlip) > 0.5f) {
				//and the vehicle is moving, then 
				if (mainRigidbody.velocity.magnitude > 1) {
					//set the skid volume value according to the vehicle skid
					currentSkidsVolume = Mathf.Abs (wheelGroundHitFront.sidewaysSlip) + ((Mathf.Abs (wheelGroundHitFront.forwardSlip) + Mathf.Abs (wheelGroundHitRear.forwardSlip)) / 4);
				} else {
					//set the skid volume value to 0
					currentSkidsVolume -= Time.deltaTime;
				}
			} else {
				//set the skid volume value to 0
				currentSkidsVolume -= Time.deltaTime;
			}
		} else {
			currentSkidsVolume -= Time.deltaTime * 10;
		}

		otherCarParts.skidAudio.volume = currentSkidsVolume;

		anyOnGround = true;

		totalWheelsOnAir = 0;

		//set the smoke skid particles in every wheel
		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheel.wheelCollider.GetGroundHit (out wheelGroundHit);

			//set the skid marks under every wheel
			currentWheel.wheelSlipAmountSideways = Mathf.Abs (wheelGroundHit.sidewaysSlip);
			currentWheel.wheelSlipAmountForward = Mathf.Abs (wheelGroundHit.forwardSlip);

			if (currentWheel.wheelSlipAmountSideways > 0.25f || wheelsList [i].wheelSlipAmountForward > 0.5f) {
				Vector3 skidPoint = wheelGroundHit.point + (2 * Time.deltaTime) * (mainRigidbody.velocity);

				if (mainRigidbody.velocity.magnitude > 1) {
					currentWheel.lastSkidmark = skidMarksManager.AddSkidMark (skidPoint, wheelGroundHit.normal, 
						(currentWheel.wheelSlipAmountSideways / 2) + (currentWheel.wheelSlipAmountForward / 2.5f), currentWheel.lastSkidmark);
				} else {
					currentWheel.lastSkidmark = -1;
				}
			} else {
				currentWheel.lastSkidmark = -1;
			}
				
			if (currentWheel.hasParticles) {
				currentParticleSystem = currentWheel.wheelParticleSystem;

				if (Mathf.Abs (wheelGroundHit.sidewaysSlip) > 0.25f || Mathf.Abs (wheelGroundHit.forwardSlip) > 0.5f) {
					if (!currentParticleSystem.isPlaying) {
						currentParticleSystem.Play ();
					}
				} else { 
					if (currentParticleSystem.isPlaying) {
						currentParticleSystem.Stop ();
					}
				}
			}

			//check if the car is in the ground or not
			if (!currentWheel.wheelCollider.isGrounded) {
				//if the current wheel is in the air, increase the number of wheels in the air
				totalWheelsOnAir++;
			}
		}

		if (isTurnedOn) {
			checkVehicleTurnedOn = true;
			//set the exhaust particles state
			for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {

				currentParticleSystem = otherCarParts.normalExhaust [i];

				if (isTurnedOn && currentSpeed < 20) {
					if (!currentParticleSystem.isPlaying) {
						currentParticleSystem.Play ();
					}
				} else {
					currentParticleSystem.Stop ();
				}
			}

			for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {
				currentParticleSystem = otherCarParts.heavyExhaust [i];

				if (isTurnedOn && currentSpeed > 20 && motorInput > 0.5f) {
					if (!currentParticleSystem.isPlaying) {
						currentParticleSystem.Play ();
					}
				} else {
					currentParticleSystem.Stop ();
				}
			}
		} else {
			if (checkVehicleTurnedOn) {
				stopVehicleParticles ();

				checkVehicleTurnedOn = false;
			}
		}

		//if the total amount of wheels in the air is equal to the number of wheel sin the vehicle, anyOnGround is false
		if (totalWheelsOnAir == wheelsListCount && anyOnGround) {
			anyOnGround = false;
		}

		//if any wheel is in the ground rear, then 
		if (anyOnGround) {
			//check if the jump input has been presses
			if (jumpInputPressed) {
				//apply force in the up direction
				mainRigidbody.AddForce ((mainRigidbody.mass * vehicleControllerSettings.jumpPower) * transform.up);

				jumpInputPressed = false;
			}
		}

		//stabilize the vehicle in its forward direction
		predictedUp = Quaternion.AngleAxis (mainRigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * stabilityForce / stabilitySpeed, mainRigidbody.angularVelocity) * transform.up;

		torqueVector = Vector3.Cross (predictedUp, mainVehicleCameraController.transform.up);
		torqueVector = transform.forward * transform.InverseTransformDirection (torqueVector).z;

		mainRigidbody.AddTorque ((stabilitySpeed * stabilitySpeed) * torqueVector);
	}

	public void checkDriverFootPosition ()
	{
		if (lowSpeedDetected != previoslyLowSpeedDetected) {
			previoslyLowSpeedDetected = lowSpeedDetected;

			if (lowSpeedDetected) {

				Vector3 currentNormal = getCurrentNormal ();

				float chassisAngle = Vector3.SignedAngle (transform.up, currentNormal, transform.forward);

				useRightFootForGround = true;

				if (chassisAngle > 0) {
					useRightFootForGround = false;
				}

				Transform raycastPositionTransform = rightRaycastPositionToFeet;
				if (!useRightFootForGround) {
					raycastPositionTransform = leftRaycastPositionToFeet;
				}

				if (Physics.Raycast (raycastPositionTransform.position, -currentNormal, out hit, 20, settings.layer)) {
					raycastPositionFoundForFeet = hit.point;

					raycastPositionFoundForFeet += transform.right * footOnGroundPositionOffset.x + hit.normal * footOnGroundPositionOffset.y + transform.forward * footOnGroundPositionOffset.z;

					footPositionOnGround.position = raycastPositionFoundForFeet;

					footPositionOnGround.SetParent (footPositionOnGroundParent);
					footPositionOnGround.localRotation = Quaternion.identity;

					if (useRightFootForGround) {
						setChassisRotation (new Vector3 (0, 0, -lowSpeedChassisRotationAmount));
					} else {
						setChassisRotation (new Vector3 (0, 0, lowSpeedChassisRotationAmount));
					}

					adjustFeetToGround (true);
				}
			} else {
				setChassisRotation (new Vector3 (0, 0, 0));
				adjustFeetToGround (false);
			}
		}
	}

	public void playStandAnimation (bool playForward)
	{
		if (useStandAnimation) {
			if (playForward) {
				mainAnimation [standAnimationName].speed = 1;
			} else {
				mainAnimation [standAnimationName].speed = -1; 
				mainAnimation [standAnimationName].time = mainAnimation [standAnimationName].length;
			}

			mainAnimation.Play (standAnimationName);
		}
	}

	public override void setEngineOnOrOffState ()
	{
		base.setEngineOnOrOffState ();


	}

	public override void pressHorn ()
	{
		base.pressHorn ();

		setAudioState (otherCarParts.hornAudio, 5, 1, otherCarParts.hornClip, false, true, false);
	}

	//if the vehicle is using the gravity control, set the state in this component
	public override void changeGravityControlUse (bool state)
	{
		base.changeGravityControlUse (state);

		if (usingGravityControl) {
			StartCoroutine (changeGear (0));
		}
	}

	//the player is getting on or off from the vehicle, so
	public override void changeVehicleState ()
	{
		base.changeVehicleState ();

		playStandAnimation (driving);
	}

	public override void setTurnOnState ()
	{
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, true, false);
		setAudioState (otherCarParts.skidAudio, 5, 0, otherCarParts.skidClip, true, true, false);
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
	}

	public override void turnOnOrOff (bool state, bool previouslyTurnedOn)
	{
		base.turnOnOrOff (state, previouslyTurnedOn);


	}

	//the vehicle has been destroyed, so disabled every component in it
	public override void disableVehicle ()
	{
		//stop the audio sources
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, false);
		setAudioState (otherCarParts.skidAudio, 5, 0, otherCarParts.skidClip, false, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);

		skidMarksManager.setSkidsEnabledState (false);

		vehicleDestroyed = true;

		setTurnOffState (false);

		//disable the skid particles
		stopVehicleParticles ();

		//disable the controller
		this.enabled = false;
	}

	public void stopVehicleParticles ()
	{
		for (i = 0; i < wheelsListCount; i++) {
			wheelsList [i].wheelParticleSystem.Stop ();
		}

		//disable the exhausts particles
		for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {
			otherCarParts.normalExhaust [i].Stop ();
		}

		for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {
			otherCarParts.heavyExhaust [i].Stop ();
		}
	}

	//change the gear in the vehicle
	IEnumerator changeGear (int gear)
	{
		changingGear = true;

		setAudioState (otherCarParts.gearShiftingSound, 5, 0.3f, gearsList [gear].gearShiftingClip, false, true, false);	

		yield return new WaitForSeconds (0.5f);

		changingGear = false;

		currentGear = gear;
	}
		
	//if the vehicle is using the boost, set the boost particles
	public override void usingBoosting ()
	{
		base.usingBoosting ();

		if (otherCarParts.boostParticles) {
			for (int i = 0; i < boostingParticles.Count; i++) {
				if (usingBoost) {
					if (!boostingParticles [i].isPlaying) {
						boostingParticles [i].gameObject.SetActive (true);
						boostingParticles [i].Play ();

						var boostingParticlesMain = boostingParticles [i].main;
						boostingParticlesMain.loop = true;
					}
				} else {
					if (boostingParticles [i].isPlaying) {
						var boostingParticlesMain = boostingParticles [i].main;
						boostingParticlesMain.loop = false;
					}
				}
			}
		}
	}

	//use a jump platform
	public void useVehicleJumpPlatform (Vector3 direction)
	{
		mainRigidbody.AddForce (mainRigidbody.mass * direction, ForceMode.Impulse);
	}

	public void useJumpPlatformParable (Vector3 direction)
	{
		mainRigidbody.AddForce (direction, ForceMode.VelocityChange);
	}

	public void setNewJumpPower (float newJumpPower)
	{
		vehicleControllerSettings.jumpPower = newJumpPower * 100;
	}

	public void setOriginalJumpPower ()
	{
		vehicleControllerSettings.jumpPower = originalJumpPower;
	}

	Coroutine chassisRotationCoroutine;

	public void setChassisRotation (Vector3 rotationAmount)
	{
		stopSetChassisRotationCoroutine ();
		chassisRotationCoroutine = StartCoroutine (setChassisRotationCoroutine (rotationAmount));
	}

	public void stopSetChassisRotationCoroutine ()
	{
		if (chassisRotationCoroutine != null) {
			StopCoroutine (chassisRotationCoroutine);
		}
	}

	IEnumerator setChassisRotationCoroutine (Vector3 rotationAmount)
	{

		float t = 0;

		float normalAngle = 0;

		bool targetReached = false;

		float coroutineTimer = 0;

		while (!targetReached) {
			t += Time.deltaTime * chassisRotationSpeed;

			normalAngle = Quaternion.Angle (otherCarParts.chassis.transform.localRotation, Quaternion.Euler (rotationAmount));

			otherCarParts.chassis.transform.localRotation = Quaternion.Slerp (otherCarParts.chassis.transform.localRotation, Quaternion.Euler (rotationAmount), t);

			coroutineTimer += Time.deltaTime;

			if (normalAngle < 0.1f || coroutineTimer > 2) {
				targetReached = true;

				if (rotationAmount == Vector3.zero) {
					adjustChassisLean = true;
				}
			}
			yield return null;	
		}
	}

	Coroutine adjustFeetCoroutine;

	public void adjustFeetToGround (bool adjustToGround)
	{
		stopAdjustHoldOnLedgeCoroutine ();

		adjustFeetCoroutine = StartCoroutine (adjustFeetToGroundCoroutine (adjustToGround));
	}

	public void stopAdjustHoldOnLedgeCoroutine ()
	{
		if (adjustFeetCoroutine != null) {
			StopCoroutine (adjustFeetCoroutine);
		}
	}

	IEnumerator adjustFeetToGroundCoroutine (bool adjustToGround)
	{
		Transform currentFootTransform = rightFootTransform;

		Vector3 targetPosition = raycastPositionFoundForFeet;

		Quaternion targetRotation = footPositionOnGround.localRotation;

		Transform currentTargetRotationTransform = footPositionOnGround;

		footPositionOnGround.SetParent (mainVehicleCameraController.transform);
		footPositionOnGround.localEulerAngles = new Vector3 (0, footPositionOnGround.localEulerAngles.y, 0);
		footPositionOnGround.SetParent (null);

		if (!adjustToGround) {
			targetPosition = originalRightFootPosition.localPosition;
			targetRotation = originalRightFootPosition.localRotation;
			currentTargetRotationTransform = originalRightFootPosition;
		}

		if (!useRightFootForGround) {
			currentFootTransform = lefFootTransform;

			if (!adjustToGround) {
				targetPosition = originalLeftFootPosition.localPosition;
				targetRotation = originalLeftFootPosition.localRotation;
				currentTargetRotationTransform = originalLeftFootPosition;
			}
		}

		if (adjustToGround) {
			targetPosition = footPositionOnGround.position;
			targetRotation = footPositionOnGround.rotation;
		}

		float dist = 0;

		if (adjustToGround) {
			dist = GKC_Utils.distance (currentFootTransform.position, targetPosition);
		} else {
			dist = GKC_Utils.distance (currentFootTransform.localPosition, targetPosition);
		}

		float duration = dist / adjustFeetToGroundSpeed;
		float translateTimer = 0;
		float rotateTimer = 0;

		float adjustmentTimer = 0;

		float normalAngle = 0;

		bool targetReached = false;
		while (!targetReached) {
			translateTimer += Time.deltaTime / duration;
			if (adjustToGround) {
				currentFootTransform.position = Vector3.Lerp (currentFootTransform.position, targetPosition, translateTimer);
			} else {
				currentFootTransform.localPosition = Vector3.Lerp (currentFootTransform.localPosition, targetPosition, translateTimer);
			}

			rotateTimer += Time.deltaTime * adjustFeetToGroundSpeed;

			if (adjustToGround) {
				currentFootTransform.rotation = Quaternion.Lerp (currentFootTransform.rotation, targetRotation, rotateTimer);
			} else {
				currentFootTransform.localRotation = Quaternion.Lerp (currentFootTransform.localRotation, targetRotation, rotateTimer);
			}

			adjustmentTimer += Time.deltaTime;

			normalAngle = Vector3.Angle (currentFootTransform.up, currentTargetRotationTransform.up);   

			if (adjustToGround) {
				if ((GKC_Utils.distance (currentFootTransform.position, targetPosition) < 0.01f && normalAngle < 0.1f) || adjustmentTimer > (duration + 2f)) {
					targetReached = true;
				}
			} else {
				if ((GKC_Utils.distance (currentFootTransform.localPosition, targetPosition) < 0.01f && normalAngle < 0.1f) || adjustmentTimer > (duration + 2f)) {
					targetReached = true;
				}
			}
			yield return null;
		}
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
			jumpInputPressed = true;
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
			pressHorn ();
		}
	}

	public override void inputHoldOrReleaseBrake (bool holdingButton)
	{
		if (driving && !usingGravityControl) {
			braking = holdingButton;
		}
	}

	[System.Serializable]
	public class Wheels
	{
		public string Name;
		public WheelCollider wheelCollider;
		public GameObject wheelMesh;
		public GameObject mudGuard;
		public GameObject suspension;
		public wheelType wheelSide;
		public ParticleSystem wheelParticleSystem;

		[HideInInspector] public bool hasMudGuard;
		[HideInInspector] public bool hasSuspension;
		[HideInInspector] public bool hasParticles;

		[HideInInspector] public float suspensionSpringPos;
		[HideInInspector] public float rotationValue;
		[HideInInspector] public float wheelSlipAmountSideways;
		[HideInInspector] public float wheelSlipAmountForward;
		[HideInInspector] public int lastSkidmark = -1;
	}

	public enum wheelType
	{
		front,
		rear,
	}

	[System.Serializable]
	public class OtherCarParts
	{
		public Transform steeringHandlebar;
		public Transform COM;
		public GameObject wheelSlipPrefab;
		public GameObject chassis;
		public AudioClip engineStartClip;
		public AudioClip engineClip;
		public AudioClip engineEndClip;
		public AudioClip skidClip;
		public AudioClip[] crashClips;
		public AudioClip hornClip;
		public List<ParticleSystem> normalExhaust = new List<ParticleSystem> ();
		public List<ParticleSystem> heavyExhaust = new List<ParticleSystem> ();
		public AudioSource engineStartAudio;
		public AudioSource engineAudio;
		public AudioSource skidAudio;
		public AudioSource crashAudio;
		public AudioSource gearShiftingSound;
		public AudioSource hornAudio;
		public GameObject boostParticles;
	}

	[System.Serializable]
	public class Gears
	{
		public string Name;
		public AnimationCurve engineTorqueCurve;
		public float gearSpeed;
		public AudioClip gearShiftingClip;
	}

	[System.Serializable]
	public class motorBikeSettings
	{
		public LayerMask layer;
		public float engineTorque = 1500;
		public float maxRPM = 6000;
		public float minRPM = 1000;
		public float steerInputSpeed = 10;
		public float steerAngleLimit;
		public float highSpeedSteerAngle = 5;
		public float highSpeedSteerAngleAtSpeed = 80;
		public float brake;
		public float maxBackwardSpeed;
		public float gearShiftRate = 10;
		public Vector2 chassisLean;
		public Vector2 chassisLeanLimit;
		public bool preserveDirectionWhileInAir;
		public bool useCurves;
	}
}