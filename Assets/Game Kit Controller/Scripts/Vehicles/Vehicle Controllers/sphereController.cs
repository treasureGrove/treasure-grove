using UnityEngine;
using System.Collections;

public class sphereController : vehicleController
{
	[Header ("Custom Settings")]
	[Space]

	public otherVehicleParts vehicleParts;
	public vehicleSettings settings;

	[Space]
	[Header ("Physics Settings")]
	[Space]

	[SerializeField, Range (0f, 100f)]
	float maxSpeed = 10f;

	[SerializeField, Range (0f, 100f)]
	float maxClimbSpeed = 4f;

	[SerializeField, Range (0f, 100f)]
	float maxBoostClimbSpeed = 4f;

	[SerializeField, Range (0f, 100f)]
	float maxSwimSpeed = 5f;

	[SerializeField, Range (0f, 100f)]
	float maxAcceleration = 10f;

	[SerializeField, Range (0f, 100f)]
	float maxAirAcceleration = 1f;

	[SerializeField, Range (0f, 100f)]
	float maxClimbAcceleration = 40f;

	[SerializeField, Range (0f, 100f)]
	float maxSwimAcceleration = 5f;

	[SerializeField, Range (0, 5)]
	int maxAirJumps = 0;

	[SerializeField, Range (0, 90)]
	float maxGroundAngle = 25f, maxStairsAngle = 50f;

	[SerializeField, Range (90, 170)]
	float maxClimbAngle = 140f;

	[SerializeField, Range (0f, 100f)]
	float maxSnapSpeed = 100f;

	[SerializeField]
	float probeDistance = 1f;

	[SerializeField]
	float submergenceOffset = 0.5f;

	[SerializeField]
	float submergenceRange = 1f;

	[SerializeField]
	float buoyancy = 1f;

	[SerializeField, Range (0f, 10f)]
	float waterDrag = 1f;

	[SerializeField, Range (0.01f, 1f)]
	float swimThreshold = 0.5f;

	[SerializeField]
	LayerMask probeMask = -1, stairsMask = -1, climbMask = -1, waterMask = 0;

	[SerializeField]
	float sphereRadius = 0.5f;

	[SerializeField]
	float sphereAlignSpeed = 45;

	[SerializeField]
	float sphereAirRotation = 0.5f;

	[SerializeField]
	float sphereSwimRotation = 2f;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool anyOnGround;

	public int stepsSinceLastJump;

	public int climbContactCount;

	public bool desiresClimbing;

	public Vector3 velocity;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform sphereTransform;

	float originalJumpPower;
	Vector3 moveInput;

	Transform vehicleCameraTransform;

	Rigidbody connectedBody;
	Rigidbody previousConnectedBody;

	Vector3 connectionVelocity;

	Vector3 connectionWorldPosition;
	Vector3 connectionLocalPosition;

	Vector3 upAxis, rightAxis, forwardAxis;

	bool desiredJump;

	Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;

	Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;

	int groundContactCount, steepContactCount;

	float submergence;

	int jumpPhase;

	float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;

	int stepsSinceLastGrounded;

	void Awake ()
	{
		OnValidate ();
	}

	public override void Start ()
	{
		base.Start ();

		originalJumpPower = vehicleControllerSettings.jumpPower;

		vehicleCameraTransform = mainVehicleCameraController.transform;
	}

	public override void vehicleUpdate ()
	{
		base.vehicleUpdate ();

		moveInput.x = horizontalAxis;
		moveInput.z = verticalAxis;
		moveInput.y = 0;
		moveInput = Vector3.ClampMagnitude (moveInput, 1f);

	
		rightAxis = projectDirectionOnPlane (settings.vehicleCamera.transform.right, upAxis);

		forwardAxis = projectDirectionOnPlane (settings.vehicleCamera.transform.forward, upAxis);

		if (isSwimmingActive ()) {
			desiresClimbing = false;
		}

		updateSphereState ();
	}

	void FixedUpdate ()
	{
		anyOnGround = isOnGround ();

		upAxis = mainVehicleGravityControl.getCurrentNormal ();

		Vector3 gravity = -upAxis * mainVehicleGravityControl.getGravityForce ();

		fixedUpdateSphereState ();

		if (insideWater ()) {
			velocity *= 1f - waterDrag * submergence * Time.deltaTime;
		}

		adjustVelocity ();

		if (desiredJump) {
			desiredJump = false;

			activeJump (gravity);
		}

		if (isClimbingActive ()) {
			velocity -=	contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
		} else if (insideWater ()) {
			velocity += gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
		} else if (isOnGround () && velocity.sqrMagnitude < 0.01f) {
			velocity += contactNormal * (Vector3.Dot (gravity, contactNormal) * Time.deltaTime);
		} else if (desiresClimbing && isOnGround ()) {
			velocity += (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;
		} else {
			velocity += gravity * Time.deltaTime;
		}

		mainRigidbody.velocity = velocity;

		if (anyOnGround) {
			if (braking) {
				float verticalVelocity = vehicleCameraTransform.InverseTransformDirection (mainRigidbody.velocity).y;
				Vector3 downVelocity = vehicleCameraTransform.up * verticalVelocity;
			
				mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, Vector3.zero + downVelocity, Time.deltaTime * settings.brakeForce);
			}
		}

		clearState ();

		currentSpeed = mainRigidbody.velocity.magnitude;
	}

	void updateSphereState ()
	{
		Vector3 rotationPlaneNormal = lastContactNormal;

		float rotationFactor = 1f;

		if (isClimbingActive ()) {

		} else if (isSwimmingActive ()) {

			rotationFactor = sphereSwimRotation;
		} else if (!isOnGround ()) {
			if (isOnSteep ()) {
				rotationPlaneNormal = lastSteepNormal;
			} else {
				rotationFactor = sphereAirRotation;
			}
		}

		Vector3 movement = (mainRigidbody.velocity - lastConnectionVelocity) * Time.deltaTime;

		movement -= rotationPlaneNormal * Vector3.Dot (movement, rotationPlaneNormal);

		float distance = movement.magnitude;

		Quaternion rotation = sphereTransform.localRotation;

		if (connectedBody != null && connectedBody == previousConnectedBody) {
			rotation = Quaternion.Euler (connectedBody.angularVelocity * (Mathf.Rad2Deg * Time.deltaTime)) * rotation;

			if (distance < 0.001f) {
				sphereTransform.localRotation = rotation;

				return;
			}
		} else if (distance < 0.001f) {
			return;
		}

		float angle = distance * rotationFactor * (180f / Mathf.PI) / sphereRadius;

		Vector3 rotationAxis = Vector3.Cross (rotationPlaneNormal, movement).normalized;

		rotation = Quaternion.Euler (rotationAxis * angle) * rotation;

		if (sphereAlignSpeed > 0f) {
			rotation = alignSphereRotation (rotationAxis, rotation, distance);
		}

		sphereTransform.localRotation = rotation;
	}

	Quaternion alignSphereRotation (Vector3 rotationAxis, Quaternion rotation, float traveledDistance)
	{
		Vector3 sphereAxis = sphereTransform.up;

		float dot = Mathf.Clamp (Vector3.Dot (sphereAxis, rotationAxis), -1f, 1f);

		float angle = Mathf.Acos (dot) * Mathf.Rad2Deg;

		float maxAngle = sphereAlignSpeed * traveledDistance;

		Quaternion newAlignment = Quaternion.FromToRotation (sphereAxis, rotationAxis) * rotation;

		if (angle <= maxAngle) {
			return newAlignment;
		} else {
			return Quaternion.SlerpUnclamped (rotation, newAlignment, maxAngle / angle);
		}
	}

	void clearState ()
	{
		lastContactNormal = contactNormal;

		lastSteepNormal = steepNormal;

		lastConnectionVelocity = connectionVelocity;

		groundContactCount = steepContactCount = climbContactCount = 0;

		contactNormal = steepNormal = climbNormal = Vector3.zero;

		connectionVelocity = Vector3.zero;

		previousConnectedBody = connectedBody;

		connectedBody = null;

		submergence = 0f;
	}

	void fixedUpdateSphereState ()
	{
		stepsSinceLastGrounded += 1;

		stepsSinceLastJump += 1;

		velocity = mainRigidbody.velocity;

		if (checkClimbing () ||
		    checkSwimming () ||
		    isOnGround () ||
		    snapToGround () ||
		    checkSteepContacts ()) {

			stepsSinceLastGrounded = 0;

			if (stepsSinceLastJump > 1) {
				jumpPhase = 0;
			}

			if (groundContactCount > 1) {
				contactNormal.Normalize ();
			}
		} else {
			contactNormal = upAxis;
		}

		if (connectedBody != null) {
			if (connectedBody.isKinematic || connectedBody.mass >= mainRigidbody.mass) {
				updateConnectionState ();
			}
		}
	}

	void updateConnectionState ()
	{
		if (connectedBody == previousConnectedBody) {
			Vector3 connectionMovement = connectedBody.transform.TransformPoint (connectionLocalPosition) - connectionWorldPosition;

			connectionVelocity = connectionMovement / Time.deltaTime;
		}

		connectionWorldPosition = mainRigidbody.position;

		connectionLocalPosition = connectedBody.transform.InverseTransformPoint (connectionWorldPosition);
	}

	bool checkClimbing ()
	{
		if (isClimbingActive ()) {
			if (climbContactCount > 1) {
				climbNormal.Normalize ();

				float upDot = Vector3.Dot (upAxis, climbNormal);

				if (upDot >= minGroundDotProduct) {
					climbNormal = lastClimbNormal;
				}
			}

			groundContactCount = 1;

			contactNormal = climbNormal;

			return true;
		}

		return false;
	}

	bool checkSwimming ()
	{
		if (isSwimmingActive ()) {
			groundContactCount = 0;

			contactNormal = upAxis;

			return true;
		}

		return false;
	}

	public override void updateMovingState ()
	{
		moving = verticalAxis != 0 || horizontalAxis != 0;
	}

	bool snapToGround ()
	{
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2 || insideWater ()) {
			return false;
		}

		float speed = velocity.magnitude;

		if (speed > maxSnapSpeed) {
			return false;
		}

		RaycastHit hit;

		if (!Physics.Raycast (mainRigidbody.position, -upAxis, out  hit, probeDistance, probeMask, QueryTriggerInteraction.Ignore)) {
			return false;
		}

		Vector3 hitNormal = hit.normal;

		float upDot = Vector3.Dot (upAxis, hitNormal);

		if (upDot < getMinDot (hit.collider.gameObject.layer)) {
			return false;
		}

		groundContactCount = 1;

		contactNormal = hitNormal;

		float dot = Vector3.Dot (velocity, hitNormal);

		if (dot > 0f) {
			velocity = (velocity - hitNormal * dot).normalized * speed;
		}

		connectedBody = hit.rigidbody;

		return true;
	}

	bool checkSteepContacts ()
	{
		if (steepContactCount > 1) {
			steepNormal.Normalize ();

			float upDot = Vector3.Dot (upAxis, steepNormal);

			if (upDot >= minGroundDotProduct) {
				steepContactCount = 0;

				groundContactCount = 1;

				contactNormal = steepNormal;

				return true;
			}
		}

		return false;
	}

	void adjustVelocity ()
	{
		float acceleration, speed;

		Vector3 xAxis, zAxis;

		float currentMaxSpeeed = maxSpeed * boostInput;

		if (isClimbingActive ()) {
			acceleration = maxClimbAcceleration;

			speed = maxClimbSpeed;

			if (usingBoost) {
				speed = maxBoostClimbSpeed;
			}

			xAxis = Vector3.Cross (contactNormal, upAxis);

			zAxis = upAxis;
		} else if (insideWater ()) {
			float swimFactor = Mathf.Min (1f, submergence / swimThreshold);

			acceleration = Mathf.LerpUnclamped (isOnGround () ? maxAcceleration : maxAirAcceleration, maxSwimAcceleration, swimFactor);

			speed = Mathf.LerpUnclamped (currentMaxSpeeed, maxSwimSpeed, swimFactor);

			xAxis = rightAxis;
			zAxis = forwardAxis;
		} else {
			acceleration = isOnGround () ? maxAcceleration : maxAirAcceleration;

			float currentClimbSpeed = maxClimbSpeed;

			if (usingBoost) {
				currentClimbSpeed = maxBoostClimbSpeed;
			}

			speed = isOnGround () && desiresClimbing ? currentClimbSpeed : currentMaxSpeeed;

			xAxis = rightAxis;
			zAxis = forwardAxis;
		}

		xAxis = projectDirectionOnPlane (xAxis, contactNormal);
		zAxis = projectDirectionOnPlane (zAxis, contactNormal);

		Vector3 relativeVelocity = velocity - connectionVelocity;

		Vector3 adjustment;

		adjustment.x = moveInput.x * speed - Vector3.Dot (relativeVelocity, xAxis);
		adjustment.z = moveInput.z * speed - Vector3.Dot (relativeVelocity, zAxis);
		adjustment.y = isSwimmingActive () ? moveInput.y * speed - Vector3.Dot (relativeVelocity, upAxis) : 0f;

		adjustment = Vector3.ClampMagnitude (adjustment, acceleration * Time.deltaTime);

		velocity += (xAxis * adjustment.x + zAxis * adjustment.z) * boostInput;

		if (isSwimmingActive ()) {
			velocity += upAxis * adjustment.y;
		}
	}

	bool isOnGround ()
	{
		return groundContactCount > 0;
	}

	bool isOnSteep ()
	{
		return steepContactCount > 0;
	}

	bool isClimbingActive ()
	{
		return climbContactCount > 0 && stepsSinceLastJump > 2;
	}

	bool insideWater ()
	{
		return submergence > 0f;
	}

	bool isSwimmingActive ()
	{
		return submergence >= swimThreshold;
	}

	public void preventSnapToGround ()
	{
		stepsSinceLastJump = -1;
	}

	void OnValidate ()
	{
		minGroundDotProduct = Mathf.Cos (maxGroundAngle * Mathf.Deg2Rad);

		minStairsDotProduct = Mathf.Cos (maxStairsAngle * Mathf.Deg2Rad);

		minClimbDotProduct = Mathf.Cos (maxClimbAngle * Mathf.Deg2Rad);
	}

	void activeJump (Vector3 gravity)
	{
		Vector3 jumpDirection;

		if (isOnGround ()) {
			jumpDirection = contactNormal;
		} else if (isOnSteep ()) {
			jumpDirection = steepNormal;

			jumpPhase = 0;
		} else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps) {
			if (jumpPhase == 0) {
				jumpPhase = 1;
			}
			jumpDirection = contactNormal;
		} else {
			return;
		}

		stepsSinceLastJump = 0;

		jumpPhase += 1;

		float jumpSpeed = Mathf.Sqrt (2f * gravity.magnitude * vehicleControllerSettings.jumpPower);

		if (insideWater ()) {
			jumpSpeed *= Mathf.Max (0f, 1f - submergence / swimThreshold);
		}

		jumpDirection = (jumpDirection + upAxis).normalized;

		float alignedSpeed = Vector3.Dot (velocity, jumpDirection);

		if (alignedSpeed > 0f) {
			jumpSpeed = Mathf.Max (jumpSpeed - alignedSpeed, 0f);
		}

		velocity += jumpDirection * jumpSpeed;
	}

	void OnCollisionEnter (Collision collision)
	{
		EvaluateCollision (collision);
	}

	void OnCollisionStay (Collision collision)
	{
		EvaluateCollision (collision);
	}

	void EvaluateCollision (Collision collision)
	{
		if (isSwimmingActive ()) {
			return;
		}

		int layer = collision.gameObject.layer;

		float minDot = getMinDot (layer);

		for (int i = 0; i < collision.contacts.Length; i++) {
			Vector3 normal = collision.contacts [i].normal;

			float upDot = Vector3.Dot (upAxis, normal);

			if (upDot >= minDot) {
				groundContactCount += 1;

				contactNormal += normal;

				connectedBody = collision.rigidbody;
			} else {
				if (upDot > -0.01f) {
					steepContactCount += 1;

					steepNormal += normal;

					if (groundContactCount == 0) {
						connectedBody = collision.rigidbody;
					}
				}

				if (desiresClimbing && upDot >= minClimbDotProduct && (climbMask & (1 << layer)) != 0) {

					climbContactCount += 1;

					climbNormal += normal;

					lastClimbNormal = normal;

					connectedBody = collision.rigidbody;
				}
			}
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if ((waterMask & (1 << other.gameObject.layer)) != 0) {
			EvaluateSubmergence (other);
		}
	}

	void OnTriggerStay (Collider other)
	{
		if ((waterMask & (1 << other.gameObject.layer)) != 0) {
			EvaluateSubmergence (other);
		}
	}

	void EvaluateSubmergence (Collider collider)
	{
		RaycastHit hit;

		if (Physics.Raycast (mainRigidbody.position + upAxis * submergenceOffset, -upAxis, out hit, submergenceRange + 1f, waterMask)) {
			//, QueryTriggerInteraction.Collide)) {
			submergence = 1f - hit.distance / submergenceRange;
		} else {
			submergence = 1f;
		}

		if (isSwimmingActive ()) {
			connectedBody = collider.attachedRigidbody;
		}
	}

	Vector3 projectDirectionOnPlane (Vector3 direction, Vector3 normal)
	{
		return (direction - normal * Vector3.Dot (direction, normal)).normalized;
	}

	float getMinDot (int layer)
	{
		if ((stairsMask & (1 << layer)) == 0) {
			return minGroundDotProduct;
		} else {
			return minStairsDotProduct;
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

		desiresClimbing = false;
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

	public void setNewJumpPower (float newJumpPower)
	{
		vehicleControllerSettings.jumpPower = newJumpPower * 100;
	}

	public void setOriginalJumpPower ()
	{
		vehicleControllerSettings.jumpPower = originalJumpPower;
	}

	//CALL INPUT FUNCTIONS
	public override void inputJump ()
	{
		if (driving && !usingGravityControl && isTurnedOn && vehicleControllerSettings.canJump && anyOnGround) {
			if (isSwimmingActive ()) {
				return;
			}

			desiresClimbing = false;

			desiredJump = true;
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
				
				usingBoosting ();

				boostInput = 1;
			}
		}
	}

	public override void inputHoldOrReleaseBrake (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn && anyOnGround) {
			braking = holdingButton;
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

	public void inputSetClimbState (bool state)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			if (isSwimmingActive ()) {
				return;
			}

			desiresClimbing = state; 

			if (state) {
				if (usingBoost) {
					mainVehicleCameraController.usingBoost (false, vehicleControllerSettings.boostCameraShakeStateName, 
						vehicleControllerSettings.useBoostCameraShake, vehicleControllerSettings.moveCameraAwayOnBoost);
				}
			}
		}
	}

	public void inputToggleClimbState ()
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			inputSetClimbState (!desiresClimbing);
		}
	}

	[System.Serializable]
	public class otherVehicleParts
	{
		public Transform COM;
		public GameObject chassis;
	}

	[System.Serializable]
	public class vehicleSettings
	{
		public LayerMask layer;
		public GameObject vehicleCamera;
		public float brakeForce = 5;
	}
}