using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class carController : vehicleController
{
	[Header ("Custom Settings")]
	[Space]

	public List<Wheels> wheelsList = new List<Wheels> ();
	public List<Gears> gearsList = new List<Gears> ();
	public OtherCarParts otherCarParts;
	public carSettings settings;

	public bool rotateVehicleUpward = true;

	public bool bounceVehicleOnPassengerGettingOnOff;

	[Space]
	[Header ("Debug")]
	[Space]

	public int currentGear;

	public float currentRearRPM = 0;
	public float currentRPM = 0;
	public bool anyOnGround;

	public bool externalBrakeActive;

	public bool reversing;

	[Space]
	[Header ("Components")]
	[Space]

	public skidsManager skidMarksManager;
	public shipInterfaceInfo interfaceInfo;

	List<ParticleSystem> boostingParticles = new List<ParticleSystem> ();

	bool colliding;
	bool rotating;

	int i, j;
	int collisionForceLimit = 5;
	float horizontalLean = 0;
	float verticalLean = 0;
	float resetTimer = 0;

	float steerInput = 0;

	float acceleration;
	float lastVelocity = 0;
	float defSteerAngle;
	float driftAngle;
	float timeToStabilize = 0;

	float originalJumpPower;
	RaycastHit hit;

	bool interfaceActive;

	bool bouncingVehicle;
	Coroutine bounceCoroutine;

	Transform currentWheelColliderTransform;

	WheelCollider currentWheelCollider;

	Transform currentWheelMesh;
	Transform currentMudGuard;
	Transform currentSuspension;

	Wheels currentWheel;

	Quaternion currentWheelColliderRotation;

	float localScaleY;

	WheelHit wheelGroundHitFront = new WheelHit ();
	WheelHit wheelGroundHitRear = new WheelHit ();

	WheelHit FrontWheelHit;
	WheelHit RearWheelHit;

	float currentSteerInput;

	Vector3 newLocalEulerAngles;

	Vector3 vector3Zero = Vector3.zero;

	Vector3 currentWheelColliderLocalEulerAngles;
	float currentWheelColliderZAxisAngle;

	float currentSteeringWheelZAxisAngle;

	Vector3 wheelCenterPosition;

	bool currentHasMudGuardValue;
	bool currentHasSuspensionValue;

	Vector3 currentWheelUp;

	float currentWheelRadius;
	float currentWheelSuspensionDistance;

	RaycastHit[] raycastResults = new RaycastHit[1];

	int hitsAmount;

	float currentSkidsVolume;

	int wheelsListCount;

	int gearListCount;

	ParticleSystem currentParticleSystem;

	public override void Start ()
	{
		base.Start ();

		//get every wheel component, like the mudguard, suspension and the slip smoke particles
		wheelsListCount = wheelsList.Count;

		gearListCount = gearsList.Count;

		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheel.hasMudGuard = currentWheel.mudGuard != null;

			if (currentWheel.hasMudGuard) {
				currentMudGuard = currentWheel.mudGuard.transform;

				currentWheel.mudGuardOriginalLocalEuler = transform.localEulerAngles;
				currentWheel.mudGuardOffset = currentMudGuard.localPosition - currentWheel.wheelMesh.transform.localPosition;
			}

			currentWheel.hasSuspension = currentWheel.suspension != null;

			if (currentWheel.hasSuspension) {
				currentSuspension = currentWheel.suspension.transform;

				currentWheel.suspensionOffset = currentSuspension.localPosition - currentWheel.wheelMesh.transform.localPosition;
			}

			currentWheel.hasParticles = currentWheel.wheelParticleSystem != null;
		}

		//set the sound components
		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, false, false);
		setAudioState (otherCarParts.skidAudio, 5, 0, otherCarParts.skidClip, true, false, false);
		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);

		//get the vehicle rigidbody
		mainRigidbody.maxAngularVelocity = 5;

		//store the max sterr angle
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

		//set the current axis input in the motor input
		currentRearRPM = 0;

		float currentDeltaTime = Time.deltaTime;

		//steering input
		steerInput = Mathf.Lerp (steerInput, horizontalAxis, currentDeltaTime * settings.steerInputSpeed);

		currentSteerInput = settings.steerAngleLimit * steerInput;

		//set the steer angle in every steerable wheel and get the currentRearRPM from the wheel that power the vehicle
		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheelCollider = currentWheel.wheelCollider;

			if (currentWheel.steerable) {
				if (currentWheel.reverseSteer) {
					currentWheelCollider.steerAngle = -currentSteerInput;
				} else {
					currentWheelCollider.steerAngle = currentSteerInput;
				}
			}

			if (currentWheel.powered) {
				currentRearRPM += currentWheelCollider.rpm;
			}
		}

		//change gears
		if (!changingGear && !usingGravityControl) {
			if (currentGear + 1 < gearListCount) {
				if (currentSpeed >= gearsList [currentGear].gearSpeed && currentRearRPM > 0) {
					//print ("mas"+gearsList [currentGear+1].Name + " " + gearsList [currentGear].gearSpeed);
					StartCoroutine (changeGear (currentGear + 1));
				}
			}

			if (currentGear - 1 >= 0) {
				if (currentSpeed < gearsList [currentGear - 1].gearSpeed) {
					//print ("menos"+gearsList [currentGear-1].Name + " " + gearsList [currentGear-1].gearSpeed);
					StartCoroutine (changeGear (currentGear - 1));
				}
			}
		}

		//reset the vehicle rotation if it is upside down 
		if (isUseOfGravityActive () && currentSpeed < 5) {
			//check the current rotation of the vehicle with respect to the normal of the gravity normal component, which always point the up direction
			float angle = Vector3.Angle (currentNormal, transform.up);

			//&& !colliding
			if (angle > 60 && !rotating) {
				resetTimer += currentDeltaTime;

				if (resetTimer > 1.5f) {
					resetTimer = 0;

					if (rotateVehicleUpward) {
						rotateVehicle ();
					}
				}
			}

			//set the current gear to 0
			if (currentGear > 0) {
				StartCoroutine (changeGear (0));
			}
		}

		//if the vehicle has a steering Wheel, rotate it according to the steer input
		if (otherCarParts.SteeringWheel) {
			currentSteeringWheelZAxisAngle = -settings.steerAngleLimit * steerInput + (driftAngle / settings.steeringAssistanceDivider) * 2;

			if (Mathf.Abs (currentSteeringWheelZAxisAngle) > 0.01f) {
	
				otherCarParts.SteeringWheel.localEulerAngles = 
					new Vector3 (otherCarParts.SteeringWheel.localEulerAngles.x, otherCarParts.SteeringWheel.localEulerAngles.y, currentSteeringWheelZAxisAngle);
			}
		}
	}

	void FixedUpdate ()
	{
		float currentDeltaTime = Time.deltaTime;

		localScaleY = transform.localScale.y;

		//check every wheel collider of the vehicle, to move it and apply rotation to it correctly using raycast
		WheelHit wheelGroundHit = new WheelHit ();

		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			//get the center position of the wheel
			currentWheelCollider = currentWheel.wheelCollider;

			currentWheelColliderTransform = currentWheelCollider.transform;

			currentWheelMesh = currentWheel.wheelMesh.transform;

			currentHasMudGuardValue = currentWheel.hasMudGuard;

			if (currentHasMudGuardValue) {
				currentMudGuard = currentWheel.mudGuard.transform;
			}

			currentHasSuspensionValue = currentWheel.hasSuspension;

			if (currentHasSuspensionValue) {
				currentSuspension = currentWheel.suspension.transform;
			}

			currentWheelRadius = currentWheelCollider.radius;

			currentWheelSuspensionDistance = currentWheelCollider.suspensionDistance;

			wheelCenterPosition = currentWheelColliderTransform.TransformPoint (currentWheelCollider.center);

			//use a raycast in the ground direction
			currentWheelCollider.GetGroundHit (out wheelGroundHit);

			currentWheelUp = currentWheelColliderTransform.up;

			//if the wheel is close enough to the ground, then
			hitsAmount = Physics.RaycastNonAlloc (wheelCenterPosition, -currentWheelUp, raycastResults, 
				(currentWheelSuspensionDistance + currentWheelRadius) * localScaleY, settings.layer);

			if (hitsAmount > 0) {
				//set the wheel mesh position according to the values of the wheel collider
				currentWheelMesh.position = raycastResults [0].point + (currentWheelRadius * localScaleY) * currentWheelUp;

				//set the suspension spring position of the wheel collider
				currentWheel.suspensionSpringPos = -(raycastResults [0].distance - currentWheelRadius);
			}

			//the wheel is in the air
			else {
				//set the suspension spring position of the wheel collider
				currentWheel.suspensionSpringPos = -currentWheelSuspensionDistance;

				//set the wheel mesh position according to the values of the wheel collider
				currentWheelMesh.position = wheelCenterPosition - (currentWheelSuspensionDistance * localScaleY) * currentWheelUp;
			}

			//set the rotation value in the wheel collider
			currentWheel.rotationValue += currentWheelCollider.rpm * (6) * currentDeltaTime;

			currentWheelColliderRotation = currentWheelColliderTransform.rotation;

			//if the wheel powers the vehicle
			if (currentWheel.powered) {
				//rotate the wheel mesh only according to the current speed 
				currentWheelMesh.rotation = currentWheelColliderRotation * Quaternion.Euler (currentWheel.rotationValue, 0, currentWheelColliderRotation.z);
			}

			//if the wheel is used to change the vehicle direction
			if (currentWheel.steerable) {
				//rotate the wheel mesh according to the current speed and rotate in the local Y axis according to the rotation of the steering wheel
				currentWheelMesh.rotation = currentWheelColliderRotation * Quaternion.Euler (currentWheel.rotationValue, 
					currentWheelCollider.steerAngle + (driftAngle / settings.steeringAssistanceDivider), currentWheelColliderRotation.z);
			}

			//if the wheel has a mudguard
			if (currentHasMudGuardValue) {
				if (currentWheel.steerable) {
					newLocalEulerAngles = currentWheel.mudGuardOriginalLocalEuler + new Vector3 (0, settings.steerAngleLimit * steerInput, 0);

					//if the wheel is steerable, rotate the mudguard according to that rotation
					if (!newLocalEulerAngles.Equals (vector3Zero)) {
						currentMudGuard.localEulerAngles = newLocalEulerAngles;
					}
				}

				//set its position according to the wheel position
				currentMudGuard.localPosition = currentWheelMesh.localPosition + currentWheel.mudGuardOffset;
			}

			//if the wheel has suspension, set its poition just like the mudguard
			if (currentHasSuspensionValue) {
				currentSuspension.localPosition = currentWheel.suspensionOffset + currentWheelMesh.localPosition;	
			}

			//calculate the drift angle, using the right side of the vehicle
			if (currentWheel.powered && currentWheel.rightSide) {
				currentWheelCollider.GetGroundHit (out wheelGroundHit);
				driftAngle = Mathf.Lerp (driftAngle, (Mathf.Clamp (wheelGroundHit.sidewaysSlip, settings.driftAngleLimit.x, settings.driftAngleLimit.y)), currentDeltaTime * 2);
			}

			//rotate the wheel collider in its  forward local axis
			float handling = Mathf.Lerp (-1, 1, wheelGroundHit.force / settings.steerWheelRotationPercentage);

			int mult = 1;

			if (currentWheel.leftSide) {
				mult = -1;
			}
				
			currentWheelColliderZAxisAngle = (handling * mult);

			if (Mathf.Abs (currentWheel.lastWheelColliderRotationValue - currentWheelColliderZAxisAngle) > 0.01f) {
				currentWheel.lastWheelColliderRotationValue = currentWheelColliderZAxisAngle;
					
				currentWheelColliderLocalEulerAngles = currentWheelColliderTransform.localEulerAngles;

				currentWheelColliderTransform.localEulerAngles = 
					new Vector3 (currentWheelColliderLocalEulerAngles.x, currentWheelColliderLocalEulerAngles.y, currentWheelColliderZAxisAngle);
			}

			//check the right front and right rear wheel to play the skid audio according to their state

			//use a wheel hit to that
			if (currentWheel.powered && currentWheel.rightSide) {
				currentWheelCollider.GetGroundHit (out wheelGroundHitFront);
			}

			if (currentWheel.steerable && currentWheel.rightSide) {
				currentWheelCollider.GetGroundHit (out wheelGroundHitRear);
			}
		}

		currentSkidsVolume = otherCarParts.skidAudio.volume;

		if (anyOnGround) {
			//if the values in the wheel hit are higher that
			if (Mathf.Abs (wheelGroundHitFront.sidewaysSlip) > 0.25f || Mathf.Abs (wheelGroundHitRear.forwardSlip) > 0.5f || Mathf.Abs (wheelGroundHitFront.forwardSlip) > 0.5f) {
				//and the vehicle is moving, then 
				if (mainRigidbody.velocity.magnitude > 1) {
					//set the skid volume value according to the vehicle skid
					currentSkidsVolume = Mathf.Abs (wheelGroundHitFront.sidewaysSlip) + ((Mathf.Abs (wheelGroundHitFront.forwardSlip) +
					Mathf.Abs (wheelGroundHitRear.forwardSlip)) / 4f);
				} else {
					//set the skid volume value to 0
					currentSkidsVolume -= currentDeltaTime;
				}
			} else {
				//set the skid volume value to 0
				currentSkidsVolume -= currentDeltaTime;
			}
		} else {
			currentSkidsVolume -= currentDeltaTime * 10;
		}

		otherCarParts.skidAudio.volume = currentSkidsVolume;

		//rotate the vehicle chassis when the gear is being changed
		//get the vertical lean value
		verticalLean = Mathf.Clamp (Mathf.Lerp (verticalLean, mainRigidbody.angularVelocity.x * settings.chassisLean.y, currentDeltaTime * 3), 
			-settings.chassisLeanLimit, settings.chassisLeanLimit);

		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheelCollider = currentWheel.wheelCollider;

			if (currentWheel.powered && currentWheel.rightSide) {
				currentWheelCollider.GetGroundHit (out wheelGroundHit);
			}
		}

		float normalizedLeanAngle = Mathf.Clamp (wheelGroundHit.sidewaysSlip, -1, 1);
		if (normalizedLeanAngle > 0) {
			normalizedLeanAngle = 1;
		} else {
			normalizedLeanAngle = -1;
		}

		if (!bouncingVehicle) {
			//get the horizontal lean value
			horizontalLean = Mathf.Clamp (Mathf.Lerp (horizontalLean, 
				(Mathf.Abs (transform.InverseTransformDirection (mainRigidbody.angularVelocity).y) * -normalizedLeanAngle) * settings.chassisLean.x, currentDeltaTime * 3), 
				-settings.chassisLeanLimit, settings.chassisLeanLimit);
			
			Quaternion chassisRotation = Quaternion.Euler (verticalLean, otherCarParts.chassis.transform.localRotation.y + (mainRigidbody.angularVelocity.z), horizontalLean);

			Vector3 targetLocalEulerAngles = chassisRotation.eulerAngles;

			//set the lean rotation value in the chassis transform
			if (Mathf.Abs (targetLocalEulerAngles.magnitude) > 0.01f) {
				if (!float.IsNaN (targetLocalEulerAngles.x) && !float.IsNaN (targetLocalEulerAngles.y) && !float.IsNaN (targetLocalEulerAngles.z)) {
					otherCarParts.chassis.transform.localEulerAngles = targetLocalEulerAngles;
				}
			}

			//set the vehicle mass center 
			mainRigidbody.centerOfMass = new Vector3 ((otherCarParts.COM.localPosition.x) * transform.localScale.x, 
				(otherCarParts.COM.localPosition.y) * localScaleY, (otherCarParts.COM.localPosition.z) * transform.localScale.z);
		}

		//allows vehicle to remain roughly pointing in the direction of travel
		//if the vehicle is not on the ground, not colliding, rotating and its speed is higher that 5
		if (!anyOnGround && settings.preserveDirectionWhileInAir && !colliding && !rotating && currentSpeed > 5) {
			//check the time to stabilize
			if (timeToStabilize < 0.6f) {
				timeToStabilize += currentDeltaTime;
			} else {
				//rotate every axis of the vehicle in the rigidbody velocity direction
				mainRigidbody.freezeRotation = true;

				float angleX = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (currentNormal.normalized, transform.up)).x) * Mathf.Rad2Deg;
				float angleZ = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (currentNormal.normalized, transform.up)).z) * Mathf.Rad2Deg;
				float angleY = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (mainRigidbody.velocity.normalized, transform.forward)).y) * Mathf.Rad2Deg;

				transform.Rotate (new Vector3 (angleX, angleY, angleZ) * (currentDeltaTime * -1));
			}
		}

		//if any of the vehicle is on the groud, free the rigidbody rotation
		if (anyOnGround) {
			mainRigidbody.freezeRotation = false;
			timeToStabilize = 0;
		}

		//if the handbrake is pressed, set the brake torque value in every wheel
		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheelCollider = currentWheel.wheelCollider;

			if (braking) {
				if (currentWheel.powered) {
					currentWheelCollider.brakeTorque = settings.brake * 15;
				}

				if (currentWheel.steerable) {
					currentWheelCollider.brakeTorque = settings.brake / 10;				
				}
			} else {
				//else, check if the vehicle input is in forward or in backward direction

				//the vehicle is decelerating
				if (Mathf.Abs (motorInput) <= 0.05f && !changingGear) {
					currentWheelCollider.brakeTorque = settings.brake / 25;
				} 

				//the vehicle is braking
				else if (motorInput < 0 && !reversing) {
					if (currentWheel.powered) {
						currentWheelCollider.brakeTorque = settings.brake * (Mathf.Abs (motorInput / 2));
					}

					if (currentWheel.steerable) {
						currentWheelCollider.brakeTorque = settings.brake * (Mathf.Abs (motorInput));
					}
				} else {
					currentWheelCollider.brakeTorque = 0;
				}
			}
		}

		//adhere the vehicle to the ground
		//check the front part
		float travel = 1;
		float totalTravel = 0;
		float antiRollForceFront;

		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheelCollider = currentWheel.wheelCollider;

			if (currentWheel.steerable) {
				//if the wheel is in the ground
				bool grounded = currentWheelCollider.GetGroundHit (out FrontWheelHit);
				if (grounded) {
					//get the value to the ground according to the wheel collider configuration
					travel = (-currentWheelCollider.transform.InverseTransformPoint (FrontWheelHit.point).y - currentWheelCollider.radius) /
					currentWheelCollider.suspensionDistance;
				}

				//if the wheel is the front wheel
				if (currentWheel.leftSide) {
					//add to the total travel
					totalTravel += travel;
				}

				//else
				if (currentWheel.rightSide) {
					//substract from the total travel
					totalTravel -= travel;
				}
			}

			travel = 1;
		}

		//now, with the force multiplier which has to be applied in the front wheels
		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheelCollider = currentWheel.wheelCollider;

			if (currentWheel.steerable) {
				int mult = 1;
				if (currentWheel.leftSide) {
					mult = -1;
				}

				//get the total amount of force applied to every wheel
				antiRollForceFront = totalTravel * settings.antiRoll;

				//if the wheel is on the ground, apply the force
				bool grounded = currentWheelCollider.GetGroundHit (out FrontWheelHit);

				if (grounded) {
					mainRigidbody.AddForceAtPosition ((mult * antiRollForceFront) * currentWheelCollider.transform.up, currentWheelCollider.transform.position);  
				}
			}
		}

		//like the above code, but this time in the rear wheels
		bool groundRear = true;
		travel = 1;
		totalTravel = 0;
		float antiRollForceRear = 0;

		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheelCollider = currentWheel.wheelCollider;

			if (currentWheel.powered) {
				bool grounded = currentWheelCollider.GetGroundHit (out RearWheelHit);
				if (grounded) {
					travel = (-currentWheelCollider.transform.InverseTransformPoint (RearWheelHit.point).y - currentWheelCollider.radius) /
					currentWheelCollider.suspensionDistance;
				}

				if (currentWheel.leftSide) {
					totalTravel += travel;
				}

				if (currentWheel.rightSide) {
					totalTravel -= travel;
				}
			}

			travel = 1;
		}

		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheelCollider = currentWheel.wheelCollider;

			if (currentWheel.powered) {
				int mult = 1;

				if (currentWheel.leftSide) {
					mult = -1;
				}

				antiRollForceRear = totalTravel * settings.antiRoll;

				bool grounded = currentWheelCollider.GetGroundHit (out RearWheelHit);

				if (grounded) {
					mainRigidbody.AddForceAtPosition (currentWheelCollider.transform.up * (mult * antiRollForceRear), currentWheelCollider.transform.position);
				} 
				//if both rear wheels are not in the ground, then 
				else {
					groundRear = false;
				}
			}
		}

		//if both rear wheels are in the ground, then 
		if (groundRear) {
			//add an extra force to the main rigidbody of the vehicle
			mainRigidbody.AddRelativeTorque ((steerInput * 5000) * Vector3.up);

			//check if the jump input has been presses
			if (jumpInputPressed) {
				//apply force in the up direction
				mainRigidbody.AddForce ((mainRigidbody.mass * vehicleControllerSettings.jumpPower) * transform.up);

				jumpInputPressed = false;

				lastTimeJump = Time.time;
			}
		}

		//get the current speed value
		currentSpeed = mainRigidbody.velocity.magnitude * 3;

		//calculate the current acceleration
		acceleration = 0;
		acceleration = (transform.InverseTransformDirection (mainRigidbody.velocity).z - lastVelocity) / Time.fixedDeltaTime;
		lastVelocity = transform.InverseTransformDirection (mainRigidbody.velocity).z;

		//set the drag according to vehicle acceleration
		mainRigidbody.drag = Mathf.Clamp ((acceleration / 50), 0, 1);

		//set the steer limit
		settings.steerAngleLimit = Mathf.Lerp (defSteerAngle, settings.highSpeedSteerAngle, (currentSpeed / settings.highSpeedSteerAngleAtSpeed));

		//set the current RPM
		currentRPM = Mathf.Clamp ((((Mathf.Abs (currentRearRPM) * settings.gearShiftRate) + settings.minRPM)) / (currentGear + 1), settings.minRPM, settings.maxRPM);

		//check if the vehicle is moving forwards or backwards
		if (motorInput <= 0 && currentRearRPM < 20) {
			reversing = true;
		} else {
			reversing = false;
		}

		//set the engine audio volume and pitch according to input and current RPM
		if (otherCarParts.engineAudio && !vehicleDestroyed) {
			if (!reversing) {
				otherCarParts.engineAudio.volume = Mathf.Lerp (otherCarParts.engineAudio.volume, Mathf.Clamp (motorInput, 0.35f, 0.85f), Time.deltaTime * 5);
			} else {
				otherCarParts.engineAudio.volume = Mathf.Lerp (otherCarParts.engineAudio.volume, Mathf.Clamp (Mathf.Abs (motorInput), 0.35f, 0.85f), Time.deltaTime * 5);
			}

			otherCarParts.engineAudio.pitch = Mathf.Lerp (otherCarParts.engineAudio.pitch, 
				Mathf.Lerp (1, 2, (currentRPM - settings.minRPM / 1.5f) / (settings.maxRPM + settings.minRPM)), currentDeltaTime * 5);
		}

		//if the current speed is higher that the max speed, stop apply motor torque to the powered wheels	
		if (currentSpeed > vehicleControllerSettings.maxForwardSpeed || Mathf.Abs (currentRearRPM / 2) > 3000 || usingGravityControl) {
			for (i = 0; i < wheelsListCount; i++) {
				currentWheel = wheelsList [i];

				currentWheelCollider = currentWheel.wheelCollider;

				if (currentWheel.powered) {
					currentWheelCollider.motorTorque = 0;
				}
			}
		} else if (!reversing) {
			//else if the vehicle is moving in fowards direction, apply motor torque to every powered wheel using the gear animation curve
			float speedMultiplier = 1;
			if (settings.useCurves) {
				speedMultiplier = gearsList [currentGear].engineTorqueCurve.Evaluate (currentSpeed);
			}

			float motorTorqueValue = settings.engineTorque * Mathf.Clamp (motorInput, 0, 1) * boostInput * speedMultiplier;

			for (i = 0; i < wheelsListCount; i++) {
				currentWheel = wheelsList [i];

				currentWheelCollider = currentWheel.wheelCollider;

				if (currentWheel.powered) {
					currentWheelCollider.motorTorque = motorTorqueValue;
				}
			}
		}

		//if the vehicle is moving backwards, apply motor torque to every powered wheel
		if (reversing) {
			//if the current speed is lower than the maxBackWardSpeed, apply motor torque
			if (currentSpeed < settings.maxBackWardSpeed && Mathf.Abs (currentRearRPM / 2) < (settings.rearEngineTorque + 500)) {
				for (i = 0; i < wheelsListCount; i++) {
					currentWheel = wheelsList [i];

					currentWheelCollider = currentWheel.wheelCollider;

					if (currentWheel.powered) {
						currentWheelCollider.motorTorque = settings.rearEngineTorque * motorInput;
					}
				}
			} 
			//else, stop adding motor torque
			else {
				for (i = 0; i < wheelsListCount; i++) {
					currentWheel = wheelsList [i];

					currentWheelCollider = currentWheel.wheelCollider;

					if (currentWheel.powered) {
						currentWheelCollider.motorTorque = 0;
					}
				}
			}
		}

		//if the vehicle is not being driving
		if (!driving && !externalBrakeActive && !autobrakeActive) {
			//stop the motor torque and apply brake torque to every wheel
			for (i = 0; i < wheelsListCount; i++) {
				currentWheel = wheelsList [i];

				currentWheelCollider = currentWheel.wheelCollider;

				currentWheelCollider.motorTorque = 0;
				currentWheelCollider.brakeTorque = settings.brake / 10;
			}
		}

		//set the smoke skid particles in every wheel
		wheelGroundHit = new WheelHit ();

		for (i = 0; i < wheelsListCount; i++) {
			currentWheel = wheelsList [i];

			currentWheelCollider = currentWheel.wheelCollider;

			currentWheelCollider.GetGroundHit (out wheelGroundHit);

			//set the skid marks under every wheel
			currentWheel.wheelSlipAmountSideways = Mathf.Abs (wheelGroundHit.sidewaysSlip);
			currentWheel.wheelSlipAmountForward = Mathf.Abs (wheelGroundHit.forwardSlip);

			if (currentWheel.wheelSlipAmountSideways > 0.25f || currentWheel.wheelSlipAmountForward > 0.5f) {
				Vector3 skidPoint = wheelGroundHit.point + (2 * currentDeltaTime) * mainRigidbody.velocity;

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
				currentWheelCollider.GetGroundHit (out wheelGroundHit);

				if (Mathf.Abs (wheelGroundHit.sidewaysSlip) > 0.25f || Mathf.Abs (wheelGroundHit.forwardSlip) > 0.5f) {
					currentWheel.wheelParticleSystem.Play ();
				} else { 
					currentWheel.wheelParticleSystem.Stop ();
				}
			}
		}

		//set the exhaust particles state
		for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {
			currentParticleSystem = otherCarParts.normalExhaust [i];

			if (isTurnedOn) {
				if (currentSpeed < 10) {
					if (!currentParticleSystem.isPlaying) {
						currentParticleSystem.Play ();
					}
				} else {
					if (currentParticleSystem.isPlaying) {
						currentParticleSystem.Stop ();
					}
				}
			} else {
				if (currentParticleSystem.isPlaying) {
					currentParticleSystem.Stop ();
				}
			}
		}

		for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {

			currentParticleSystem = otherCarParts.heavyExhaust [i];

			if (isTurnedOn) {
				if (currentSpeed > 10 && motorInput > 0.1f) {
					if (!currentParticleSystem.isPlaying) {
						currentParticleSystem.Play ();
					}
				} else {
					if (currentParticleSystem.isPlaying) {
						currentParticleSystem.Stop ();
					}
				}
			} else {
				if (currentParticleSystem.isPlaying) {
					currentParticleSystem.Stop ();
				}
			}
		}

		//check if the car is in the ground or not
		anyOnGround = true;
		int totalWheelsOnAir = 0;

		for (i = 0; i < wheelsListCount; i++) {
			if (!wheelsList [i].wheelCollider.isGrounded) {
				//if the current wheel is in the air, increase the number of wheels in the air
				totalWheelsOnAir++;
			}
		}

		//if the total amount of wheels in the air is equal to the number of wheel sin the vehicle, anyOnGround is false
		if (totalWheelsOnAir == wheelsListCount && anyOnGround) {
			anyOnGround = false;
		}

		if (interfaceInfo != null) {
			if (interfaceActive != isTurnedOn) {
				interfaceActive = isTurnedOn;

				interfaceInfo.shipEnginesState (interfaceActive);
			}
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

	public override void passengerGettingOnOff ()
	{
		vehicleBounceMovement (settings.horizontalLeanPassengerAmount, settings.verticalLeanPassengerAmount);
	}

	public void vehicleBounceMovement (float horizontalLeanAmount, float verticalLeanAmount)
	{
		if (!bounceVehicleOnPassengerGettingOnOff) {
			return;
		}

		if (bounceCoroutine != null) {
			StopCoroutine (bounceCoroutine);
		}

		bounceCoroutine = StartCoroutine (vehicleBounceMovementCoroutine (horizontalLeanAmount, verticalLeanAmount));
	}

	IEnumerator vehicleBounceMovementCoroutine (float horizontalLeanAmount, float verticalLeanAmount)
	{
		bouncingVehicle = true;

		horizontalLean = Mathf.Clamp (horizontalLeanAmount, -settings.chassisLeanLimit, settings.chassisLeanLimit);
		verticalLean = Mathf.Clamp (verticalLeanAmount, -settings.chassisLeanLimit, settings.chassisLeanLimit);
	
		Quaternion chassisRotation = Quaternion.Euler (otherCarParts.chassis.transform.localRotation.x + verticalLean, 
			                             otherCarParts.chassis.transform.localRotation.y, 
			                             otherCarParts.chassis.transform.localRotation.z + horizontalLean);
		
		Vector3 targetRotation = chassisRotation.eulerAngles;
		Vector3 originalEuler = otherCarParts.chassis.transform.eulerAngles;

		float t = 0;

		while (t < 1 && otherCarParts.chassis.transform.localEulerAngles != targetRotation) {
			t += Time.deltaTime * settings.leanPassengerSpeed;

			otherCarParts.chassis.transform.localRotation = Quaternion.Slerp (otherCarParts.chassis.transform.localRotation, Quaternion.Euler (targetRotation), t);	
		}

		bouncingVehicle = false;

		yield return null;	
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

		if (interfaceInfo != null) {
			interfaceInfo.enableOrDisableInterface (driving);
		}
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
		for (i = 0; i < wheelsListCount; i++) {
			if (wheelsList [i].hasParticles) {
				wheelsList [i].wheelParticleSystem.Stop ();
			}
		}

		//disable the exhausts particles
		for (i = 0; i < otherCarParts.normalExhaust.Count; i++) {
			otherCarParts.normalExhaust [i].Stop ();
		}

		for (i = 0; i < otherCarParts.heavyExhaust.Count; i++) {
			otherCarParts.heavyExhaust [i].Stop ();
		}

		//disable the controller
		this.enabled = false;

		if (interfaceInfo != null) {
			interfaceInfo.enableOrDisableInterface (false);
		}
	}

	Coroutine rotatingVehicleCoroutine;

	public void rotateVehicle ()
	{
		if (rotatingVehicleCoroutine != null) {
			StopCoroutine (rotatingVehicleCoroutine);
		}

		rotatingVehicleCoroutine = StartCoroutine (rotateVehicleCoroutine ());
	}

	//reset the vehicle rotation if it is upside down
	IEnumerator rotateVehicleCoroutine ()
	{
		rotating = true;

		timeToStabilize = 0;
		Quaternion currentRotation = transform.rotation;

		//rotate in the forward direction of the vehicle
		Quaternion dstRotPlayer = Quaternion.LookRotation (transform.forward, currentNormal);

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 3;

			transform.rotation = Quaternion.Slerp (currentRotation, dstRotPlayer, t);

			mainRigidbody.velocity = vector3Zero;

			yield return null;
		}

		rotating = false;
	}

	//change the gear in the vehicle
	IEnumerator changeGear (int gear)
	{
		changingGear = true;

		setAudioState (otherCarParts.gearShiftingSound, 5, 0.3f, gearsList [gear].gearShiftingClip, false, true, false);

		yield return new WaitForSeconds (0.5f);

		changingGear = false;

		currentGear = gear;

		currentGear = Mathf.Clamp (currentGear, 0, gearListCount - 1);
	}

	//if the vehicle is colliding, then
	void OnCollisionStay (Collision collision)
	{
		//set the values to avoid stabilize the vehicle yet
		mainRigidbody.freezeRotation = false;
		colliding = true;
		timeToStabilize = 0;
	}

	//the vehicle is not colliding
	void OnCollisionExit (Collision collision)
	{
		colliding = false;
	}

	//if the vehicle is using the boost, set the boost particles
	public override void usingBoosting ()
	{
		base.usingBoosting ();

		if (otherCarParts.boostParticles) {
			for (int i = 0; i < boostingParticles.Count; i++) {
				currentParticleSystem = boostingParticles [i];

				if (usingBoost) {
					if (!currentParticleSystem.isPlaying) {
						currentParticleSystem.gameObject.SetActive (true);
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
	}

	//use a jump platform
	public void useVehicleJumpPlatform (Vector3 direction)
	{
		mainRigidbody.AddForce (mainRigidbody.mass * direction, ForceMode.Impulse);
	}

	public void useJumpPlatformParable (Vector3 direction)
	{
		Vector3 jumpForce = direction;

		mainRigidbody.AddForce (jumpForce, ForceMode.VelocityChange);
	}

	public void setJumpPower (float newJumpPower)
	{
		vehicleControllerSettings.jumpPower = newJumpPower;
	}

	public void setNewJumpPower (float newJumpPower)
	{
		vehicleControllerSettings.jumpPower = newJumpPower * 100;
	}

	public void setOriginalJumpPower ()
	{
		vehicleControllerSettings.jumpPower = originalJumpPower;
	}

	public void setMaxSpeed (float maxSpeedValue)
	{
		vehicleControllerSettings.maxForwardSpeed = maxSpeedValue;
	}

	public void setMaxAcceleration (float maxAccelerationValue)
	{
		settings.engineTorque = maxAccelerationValue;
	}

	public void setMaxBrakePower (float maxBrakePower)
	{
		settings.brake = maxBrakePower;
	}

	public void setMaxTurboPower (float maxTurboPower)
	{
		vehicleControllerSettings.maxBoostMultiplier = maxTurboPower;
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

		//reset the collision values
		mainRigidbody.freezeRotation = false;

		colliding = true;

		timeToStabilize = 0;
	}

	public override void startBrakeVehicleToStopCompletely ()
	{
		braking = true;

		externalBrakeActive = braking;
	}

	public override void endBrakeVehicleToStopCompletely ()
	{
		braking = false;

		externalBrakeActive = braking;
	}

	public override void activateAutoBrakeOnGetOff ()
	{
		base.activateAutoBrakeOnGetOff ();
	}

	public override float getCurrentSpeed ()
	{
		return currentSpeed;
	}

	//CALL INPUT FUNCTIONS
	public override void inputJump ()
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			//jump input
			if (vehicleControllerSettings.canJump) {
				jumpInputPressed = true;
			}
		}
	}

	public override void inputHoldOrReleaseJump (bool holdingButton)
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

	public override void inputHoldOrReleaseTurbo (bool holdingButton)
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
		if (driving && !usingGravityControl) {
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
	public class Gears
	{
		public string Name;
		public AnimationCurve engineTorqueCurve;
		public float gearSpeed;
		public AudioClip gearShiftingClip;
	}

	[System.Serializable]
	public class Wheels
	{
		public string Name;
		public WheelCollider wheelCollider;
		public GameObject wheelMesh;
		public GameObject mudGuard;
		public GameObject suspension;
		public bool steerable;
		public bool powered;
		public bool leftSide;
		public bool rightSide;
		public ParticleSystem wheelParticleSystem;

		public bool reverseSteer;

		[HideInInspector] public bool hasMudGuard;
		[HideInInspector] public bool hasSuspension;
		[HideInInspector] public bool hasParticles;

		[HideInInspector] public float lastWheelColliderRotationValue;

		[HideInInspector] public Vector3 mudGuardOriginalLocalEuler;
		[HideInInspector] public Vector3 mudGuardOffset;
		[HideInInspector] public Vector3 suspensionOffset;
		[HideInInspector] public float suspensionSpringPos;
		[HideInInspector] public float rotationValue;
		[HideInInspector] public float wheelSlipAmountSideways;
		[HideInInspector] public float wheelSlipAmountForward;
		[HideInInspector] public int lastSkidmark = -1;
	}

	[System.Serializable]
	public class OtherCarParts
	{
		public Transform SteeringWheel;
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
	public class carSettings
	{
		public float engineTorque = 2500;
		public float rearEngineTorque = 2500;
		public float maxRPM = 6000;
		public float minRPM = 1000;
		public float steerAngleLimit;
		public float highSpeedSteerAngle = 10;
		public float highSpeedSteerAngleAtSpeed = 100;
		public Vector2 driftAngleLimit = new Vector2 (-35, 35);
		public float steerWheelRotationPercentage = 8000;
		public int steeringAssistanceDivider = 5;

		public float steerInputSpeed = 10;

		public float brake = 4000;
		public float maxBackWardSpeed;

		public float antiRoll = 10000;

		[Space]
		[Space]
		[Space]

		public Vector2 chassisLean;
		public float chassisLeanLimit;

		[Space]
		[Space]
		[Space]

		public LayerMask layer;

		public float gearShiftRate = 10;
		public bool preserveDirectionWhileInAir;

		public bool useCurves;

		[Space]
		[Space]
		[Space]

		public float horizontalLeanPassengerAmount;
		public float verticalLeanPassengerAmount;
		public float leanPassengerSpeed;
	}
}