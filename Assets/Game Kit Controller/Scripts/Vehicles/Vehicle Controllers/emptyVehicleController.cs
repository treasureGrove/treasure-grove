//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class emptyVehicleController : MonoBehaviour
//{
//	public OtherCarParts otherCarParts;
//	public carSettings settings;
//
//	public float currentSpeed;
//
//	public bool anyOnGround;
//
//	bool driving;
//	bool jump;
//	bool colliding;
//	bool moving;
//	bool usingBoost;
//	bool vehicleDestroyed;
//	bool usingGravityControl;
//
//	float resetTimer = 0;
//	float motorInput = 0;
//	float steerInput = 0;
//	float boostInput = 1;
//
//	float horizontalAxis;
//	float verticalAxis;
//	float originalJumpPower;
//	RaycastHit hit;
//	Vector3 normal;
//	Rigidbody mainRigidbody;
//	vehicleCameraController vCamera;
//	vehicleHUDManager hudManager;
//	inputActionManager actionManager;
//	vehicleGravityControl vehicleGravityControlManager;
//	shipInterfaceInfo interfaceInfo;
//	bool isTurnedOn;
//
//	Vector2 axisValues;
//	public bool usingImpulse;
//	float lastTimeImpulseUsed;
//	float lastTimeJump;
//
//	bool bouncingVehicle;
//	Coroutine bounceCoroutine;
//	float collisionForceLimit = 5;
//
//	bool braking;
//
//	void Start ()
//	{
//		//set the sound components
//		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, false, false);
//		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);
//
//		//get the vehicle rigidbody
//		mainRigidbody = GetComponent<Rigidbody> ();
//
//		vCamera = settings.vehicleCamera.GetComponent<vehicleCameraController> ();
//		hudManager = GetComponent<vehicleHUDManager> ();
//		originalJumpPower = settings.jumpPower;
//		vehicleGravityControlManager = GetComponent<vehicleGravityControl> ();
//
//		actionManager = GetComponentInParent<inputActionManager> ();
//
//		interfaceInfo = GetComponentInChildren<shipInterfaceInfo> ();
//	}
//
//	void Update ()
//	{
//		//if the player is driving this vehicle and the gravity control is not being used, then
//		if (driving && !usingGravityControl) {
//			axisValues = actionManager.getPlayerMovementAxis ();
//			horizontalAxis = axisValues.x;
//			if (isTurnedOn) {
//
//				//get the current values from the input manager, keyboard and touch controls
//				verticalAxis = axisValues.y;
//
//				//jump input
////				if (settings.canJump && actionManager.getActionInput ("Jump")) {
////					jump = true;
////				}
//
//				//boost input
////				if (settings.canUseBoost && actionManager.getActionInput ("Enable Turbo")) {
////					usingBoost = true;
////					//set the camera move away action
////					vCamera.usingBoost (true, "Boost");
////				}
//
//				//stop boost input
////				if (actionManager.getActionInput ("Disable Turbo")) {
////					usingBoost = false;
////					//disable the camera move away action
////					vCamera.usingBoost (false, "Boost");
////					//disable the boost particles
////					boostInput = 1;
////				}
//			}
//
//			//if the boost input is enabled, check if there is energy enough to use it
//			if (usingBoost) {
//				//if there is enough energy, enable the boost
//				if (hudManager.useBoost (moving)) {
//					boostInput = settings.maxBoostMultiplier;
//				} 
//
//				//else, disable the boost
//				else {
//					usingBoost = false;
//					//if the vehicle is not using the gravity control system, disable the camera move away action
//					if (!vehicleGravityControlManager.isGravityPowerActive ()) {
//						vCamera.usingBoost (false, "Boost");
//					}
//					boostInput = 1;
//				}
//			}
////			if (hudManager.canSetTurnOnState && actionManager.getActionInput ("Set TurnOn State")) {
////				setEngineOnOrOffState ();
////			}
////
////			if (actionManager.getActionInput ("Horn")) {
////				pressHorn ();
////			}
////
////			if (actionManager.getActionInput ("Brake")) {
////				braking = true;
////			} else {
////				braking = false;
////			}
//
//			//set the current speed in the HUD of the vehicle
//			hudManager.getSpeed (currentSpeed, settings.maxForwardSpeed);
//		} else {
//			//else, set the input values to 0
//			horizontalAxis = 0;
//			verticalAxis = 0;
//		}
//
//		if (hudManager.usedByAI) {
//			horizontalAxis = vehicleAI.steering;
//			verticalAxis = vehicleAI.accel;
//			//print (verticalAxis);
//			//braking = vehicleAI.footbrake == 1 ? true : false;
//		}
//
//		//set the current axis input in the motor input
//		motorInput = verticalAxis;
//		moving = verticalAxis != 0;
//
//		//if the vehicle has fuel, allow to move it
//		if (moving) {
//			if (!hudManager.useFuel ()) {
//				motorInput = 0;
//				if (isTurnedOn) {
//					turnOnOrOff (false, isTurnedOn);
//				}
//			}
//		}
//			
//	}
//
//	public void setEngineOnOrOffState ()
//	{
//		if (hudManager.hasFuel ()) {
//			turnOnOrOff (!isTurnedOn, isTurnedOn);
//		}
//	}
//
//	public void pressHorn ()
//	{
//		setAudioState (otherCarParts.hornAudio, 5, 1, otherCarParts.hornClip, false, true, false);
//		hudManager.activateHorn ();
//	}
//
//	void FixedUpdate ()
//	{
//		//allows vehicle to remain roughly pointing in the direction of travel
//		//if the vehicle is not on the ground, not colliding, rotating and its speed is higher that 5
//		if (!anyOnGround && settings.preserveDirectionWhileInAir && !colliding && currentSpeed > 5) {
//			//check the time to stabilize
//			//rotate every axis of the vehicle in the rigidbody velocity direction
//			mainRigidbody.freezeRotation = true;
//			float angleX = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (normal.normalized, transform.up)).x) * Mathf.Rad2Deg;
//			float angleZ = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (normal.normalized, transform.up)).z) * Mathf.Rad2Deg;
//			float angleY = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (mainRigidbody.velocity.normalized, transform.forward)).y) * Mathf.Rad2Deg;
//			transform.Rotate (new Vector3 (angleX, angleY, angleZ) * Time.deltaTime * (-1));
//		}
//
//		//if any of the vehicle is on the groud, free the rigidbody rotation
//		if (anyOnGround) {
//			mainRigidbody.freezeRotation = false;
//		}
//			
//		//get the current speed value
//		currentSpeed = mainRigidbody.velocity.magnitude * 3;
//		//calculate the current acceleration
//	
//		if (interfaceInfo) {
//			interfaceInfo.shipEnginesState (isTurnedOn);
//		}
//
//	}
//
//	//if the vehicle is using the gravity control, set the state in this component
//	public void changeGravityControlUse (bool state)
//	{
//		usingGravityControl = state;
//
//		usingImpulse = false;
//	}
//
//	//the player is getting on or off from the vehicle, so
//	//public void changeVehicleState (Vector3 nextPlayerPos)
//	public void changeVehicleState ()
//	{
//		driving = !driving;
//		//set the audio values if the player is getting on or off from the vehicle
//		if (driving) {
//			if (hudManager.autoTurnOnWhenGetOn) {
//				turnOnOrOff (true, isTurnedOn);
//			}
//		} else {
//			turnOnOrOff (false, isTurnedOn);
//		}
//
//		//set the same state in the gravity control components
//		vehicleGravityControlManager.changeGravityControlState (driving);
//
//		if (interfaceInfo) {
//			interfaceInfo.enableOrDisableInterface (driving);
//		}
//	}
//
//	public void setTurnOnState ()
//	{
//		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, true, true, false);
//		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, true, false);
//	}
//
//	public void setTurnOffState (bool previouslyTurnedOn)
//	{
//		if (previouslyTurnedOn) {
//			setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, true);
//			setAudioState (otherCarParts.engineAudio, 5, 1, otherCarParts.engineEndClip, false, true, false);
//		}
//		motorInput = 0;
//		steerInput = 0;
//		boostInput = 1;
//		horizontalAxis = 0;
//		verticalAxis = 0;
//
//		//stop the boost
//		if (usingBoost) {
//			usingBoost = false;
//			vCamera.usingBoost (false, "Boost");
//		
//		}
//		usingImpulse = false;
//	}
//
//	public void turnOnOrOff (bool state, bool previouslyTurnedOn)
//	{
//		isTurnedOn = state;
//		if (isTurnedOn) {
//			setTurnOnState ();
//		} else {
//			setTurnOffState (previouslyTurnedOn);
//		}
//	}
//
//	//the vehicle has been destroyed, so disabled every component in it
//	public void disableVehicle ()
//	{
//		//stop the audio sources
//		setAudioState (otherCarParts.engineAudio, 5, 0, otherCarParts.engineClip, false, false, false);
//		setAudioState (otherCarParts.engineStartAudio, 5, 0.7f, otherCarParts.engineStartClip, false, false, false);
//		vehicleDestroyed = true;
//
//		setTurnOffState (false);
//
//		//disable the controller
//		//GetComponent<carController> ().enabled = false;
//
//		if (interfaceInfo) {
//			interfaceInfo.enableOrDisableInterface (false);
//		}
//	}
//
//	//get the current normal in the gravity control component
//	public void setNormal (Vector3 normalValue)
//	{
//		normal = normalValue;
//	}
//		
//	//play or stop every audio component in the vehicle, like engine, skid, etc.., configuring also volume and loop according to the movement of the vehicle
//	public void setAudioState (AudioSource source, float distance, float volume, AudioClip audioClip, bool loop, bool play, bool stop)
//	{
//		source.minDistance = distance;
//		source.volume = volume;
//		source.clip = audioClip;
//		source.loop = loop;
//		source.spatialBlend = 1;
//		if (play) {
//			source.Play ();
//		}
//		if (stop) {
//			source.Stop ();
//		}
//	}
//
//	//if any collider in the vehicle collides, then
//	void OnCollisionEnter (Collision collision)
//	{
//		//check that the collision is not with the player
//		if (collision.contacts.Length > 0 && collision.gameObject.tag != "Player") {	
//			//if the velocity of the collision is higher that the limit
//			if (collision.relativeVelocity.magnitude > collisionForceLimit) {
//				//set the collision audio with a random collision clip
//				if (otherCarParts.crashClips.Length > 0) {
//					setAudioState (otherCarParts.crashAudio, 5, 1, otherCarParts.crashClips [UnityEngine.Random.Range (0, otherCarParts.crashClips.Length)], false, true, false);
//				}
//			}
//		}
//		//reset the collision values
//		mainRigidbody.freezeRotation = false;
//		colliding = true;
//	}
//
//	//if the vehicle is colliding, then
//	void OnCollisionStay (Collision collision)
//	{
//		//set the values to avoid stabilize the vehicle yet
//		mainRigidbody.freezeRotation = false;
//		colliding = true;
//	}
//
//	//the vehicle is not colliding
//	void OnCollisionExit (Collision collision)
//	{
//		colliding = false;
//	}
//
//	//use a jump platform
//	public void useVehicleJumpPlatform (Vector3 direction)
//	{
//		mainRigidbody.AddForce (mainRigidbody.mass * direction, ForceMode.Impulse);
//	}
//
//	public void useJumpPlatformParable (Vector3 direction)
//	{
//		Vector3 jumpForce = direction;
//		print (jumpForce);
//		mainRigidbody.AddForce (jumpForce, ForceMode.VelocityChange);
//	}
//
//	public void setJumpPower (float newJumpPower)
//	{
//		settings.jumpPower = newJumpPower;
//	}
//
//	public void setNewJumpPower (float newJumpPower)
//	{
//		settings.jumpPower = newJumpPower * 100;
//	}
//
//	public void setOriginalJumpPower ()
//	{
//		settings.jumpPower = originalJumpPower;
//	}
//
//	public void setMaxSpeed (float maxSpeedValue)
//	{
//		settings.maxForwardSpeed = maxSpeedValue;
//	}
//
//	public void setMaxTurboPower (float maxTurboPower)
//	{
//		settings.maxBoostMultiplier = maxTurboPower;
//	}
//
//	vehicleAINavMesh.movementInfo vehicleAI;
//
//	public void Move (vehicleAINavMesh.movementInfo AI)
//	{
//		vehicleAI = AI;
//	}
//
//	[System.Serializable]
//	public class OtherCarParts
//	{
//		public Transform COM;
//		public GameObject chassis;
//		public AudioClip engineStartClip;
//		public AudioClip engineClip;
//		public AudioClip engineEndClip;
//		public AudioClip[] crashClips;
//		public AudioClip hornClip;
//		public AudioSource engineStartAudio;
//		public AudioSource engineAudio;
//		public AudioSource crashAudio;
//		public AudioSource hornAudio;
//	}
//
//	[System.Serializable]
//	public class carSettings
//	{
//		public LayerMask layer;
//
//		public float maxForwardSpeed;
//		public float maxBackWardSpeed;
//		public float maxBoostMultiplier;
//		public GameObject vehicleCamera;
//		public bool preserveDirectionWhileInAir;
//		public float jumpPower;
//		public bool canJump;
//		public bool canUseBoost;
//	}
//}