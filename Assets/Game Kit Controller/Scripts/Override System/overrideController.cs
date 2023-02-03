using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class overrideController : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask layer;
	public float moveSpeedMultiplier;
	public float airControlAmount;
	public float jumpPower;
	public bool canJump;
	public float brakeForce = 5;

	public float timeToJumpOnCollidingExit = 0.4f;

	public LayerMask layerMask;

	public float raycastDistance;

	[Space]
	[Header ("Angular Velocity Settings")]
	[Space]

	public bool useAngularVelocity;
	public float angularVelocityRotationSpeed = 100;
	public float angularVelocityRotationLerpSpeed = 5;
	public bool applyForceOnAngularVelocity;
	public float forceMultiplierOnAngularVelocity;

	[Space]
	[Header ("Run Settings")]
	[Space]

	public bool canRun;
	public float runSpeed;

	[Space]
	[Header ("Impulse Settings")]
	[Space]

	public bool canImpulse;
	public float impulseForce;
	public float impulseCoolDown = 0.5f;

	[Space]
	[Header ("Damage Settings")]
	[Space]

	public bool damageObjectsEnabled;
	public float damageObjectsMultiplier;
	public bool ignoreShield;
	public float collisionForceLimit;
	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	public int damageTypeID = -1;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool controllerEnabled;
	public bool onGround;
	public float currentSpeed;
	public bool runActive;

	[Space]
	[Header ("Components")]
	[Space]

	public overrideInputManager overrideInput;
	public Transform overrideCameraTransform;
	public Transform overrideCameraParentTransform;

	public Transform controllerMeshParent;
	public GameObject controllerMesh;
	public Transform raycastPosition;

	public Rigidbody mainRigidbody;

	float lastTimeImpulse;

	float horizontalAxis;
	float verticalAxis;
	Vector3 moveInput;

	Vector2 axisValues;
	bool braking;

	float lastTimeOnGround;

	Vector3 forceToApply;

	bool externalForceActivated;
	Vector3 externalForceValue;

	ContactPoint currentContact;

	Coroutine updateCoroutine;

	void Start ()
	{
		if (mainRigidbody == null) {
			mainRigidbody = GetComponent<Rigidbody> ();	
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForSecondsRealtime (0.0001f);

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (controllerEnabled) {
			if (Physics.Raycast (raycastPosition.position, -raycastPosition.up, raycastDistance, layerMask)) {
				onGround = true;
				lastTimeOnGround = Time.time;
			} else {
				onGround = false;
			}

			axisValues = overrideInput.getCustomMovementAxis ();

			horizontalAxis = axisValues.x;
			verticalAxis = axisValues.y;

			moveInput = verticalAxis * overrideCameraTransform.forward + horizontalAxis * overrideCameraTransform.right;	
	
			forceToApply = Vector3.zero;

			if (onGround) {
				forceToApply = moveInput * moveSpeedMultiplier;
			} else {
				forceToApply = moveInput * airControlAmount;
			}
	
			if (externalForceActivated) {
				mainRigidbody.AddForce (externalForceValue);
				onGround = false;
				externalForceActivated = false;
			}

			if (onGround) {
				currentSpeed = mainRigidbody.velocity.magnitude;

				if (runActive) {
					forceToApply *= runSpeed;
				}

				if (useAngularVelocity) {
					if (forceToApply != Vector3.zero) {
						Quaternion targeRotation = Quaternion.LookRotation (Vector3.Cross (forceToApply, overrideCameraTransform.up), transform.up);

						mainRigidbody.rotation = Quaternion.Lerp (mainRigidbody.rotation, targeRotation, Time.deltaTime * angularVelocityRotationLerpSpeed);
						mainRigidbody.angularVelocity = mainRigidbody.rotation * new Vector3 (0f, 0f, -angularVelocityRotationSpeed);

						if (applyForceOnAngularVelocity) {
							mainRigidbody.AddForce (forceToApply * forceMultiplierOnAngularVelocity, ForceMode.VelocityChange);
						}
					}
				} else {
					mainRigidbody.AddForce (forceToApply, ForceMode.VelocityChange);
				}
			} else {
				mainRigidbody.AddForce (forceToApply);
			}
		
			if (braking) {
				float verticalVelocity = overrideCameraTransform.InverseTransformDirection (mainRigidbody.velocity).y;
				Vector3 downVelocity = overrideCameraTransform.up * verticalVelocity;

				mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, Vector3.zero + downVelocity, Time.deltaTime * brakeForce);
			}
		}
	}

	public void changeControllerState (bool state)
	{
		controllerEnabled = state;

		stopUpdateCoroutine ();

		braking = false;
		runActive = false;

		axisValues = Vector2.zero;
		moveInput = Vector3.zero;
		horizontalAxis = 0;
		verticalAxis = 0;

		if (mainRigidbody != null) {
			mainRigidbody.velocity = Vector3.zero;
			mainRigidbody.isKinematic = !state;
		}

		if (controllerEnabled) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}
	}

	void OnCollisionEnter (Collision collision)
	{
		if (damageObjectsEnabled) {

			currentContact = collision.contacts [0];

			float collisionMagnitude = collision.relativeVelocity.magnitude;

			//check that the collision is not with the player
			//if the velocity of the collision is higher that the limit
			if (collisionMagnitude > collisionForceLimit) {
				applyDamage.checkHealth (gameObject, collision.collider.gameObject, collisionMagnitude * damageObjectsMultiplier, 
					currentContact.normal, currentContact.point, gameObject, false, true, ignoreShield, false, 
					canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
			}
		}
	}

	public void setControllerMesh (GameObject newControllerMesh)
	{
		controllerMesh = newControllerMesh;

		controllerMesh.transform.SetParent (controllerMeshParent);

		controllerMesh.AddComponent<parentAssignedSystem> ().assignParent (gameObject);
	}

	public GameObject getControllerMesh ()
	{
		return controllerMesh;
	}

	public void removeControllerMesh ()
	{
		controllerMesh.transform.SetParent (null);

		parentAssignedSystem currentParentAssignedSystem = controllerMesh.GetComponent<parentAssignedSystem> ();

		if (currentParentAssignedSystem != null) {
			Destroy (currentParentAssignedSystem);
		}
	}

	public void inputJump ()
	{
		if (canJump && onGround && Time.time < lastTimeOnGround + timeToJumpOnCollidingExit) {
			externalForceValue = overrideCameraTransform.up * mainRigidbody.mass * jumpPower;

			externalForceActivated = true;
		}
	}

	public void inputSetBrakeState (bool state)
	{
		if (state) {
			if (onGround) {
				braking = true;
			}
		} else {
			braking = false;
		}
	}

	public void inputImpulse ()
	{
		if (canImpulse) {
			if (Time.time > impulseCoolDown + lastTimeImpulse) {
				
				lastTimeImpulse = Time.time;

				Vector3 dashDirection = moveInput;

				dashDirection.Normalize ();

				if (dashDirection == Vector3.zero || dashDirection.magnitude < 0.1f) {
					dashDirection = overrideCameraParentTransform.forward;
				}

				externalForceValue = dashDirection * impulseForce * mainRigidbody.mass;

				externalForceActivated = true;
			}
		}
	}

	public void inputRunState (bool state)
	{
		if (canRun) {
			runActive = state;
		}
	}
}