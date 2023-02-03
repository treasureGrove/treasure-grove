using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class vehicleGravityControl : gravityObjectManager
{
	[Header ("Main Settings")]
	[Space]

	public bool useGravity = true;

	public float gravityForce = 9.8f;

	public bool searchNewSurfaceOnHighFallSpeed = true;
	public bool shakeCameraOnHighFallSpeed = true;
	public float minSpeedToShakeCamera = 10;

	public bool checkSurfaceBelowOnRegularState;
	public float timeToSetNullParentOnAir = 0.5f;
	float lastTimeParentFound;

	public float circumnavigateRotationSpeed = 10;
	public bool circumnavigateCurrentSurfaceActive;
	public bool useLerpRotation;

	public bool checkSurfaceBelowLedge;
	public float surfaceBelowLedgeRaycastDistance = 3;
	public float belowLedgeRotationSpeed = 10;

	public float minAngleDifferenceToAdhereToSurface = 4;

	public bool changeDriverGravityWhenGetsOff = true;
	public Vector3 regularGravity = new Vector3 (0, 1, 0);
	public Vector3 currentNormal = new Vector3 (0, 1, 0);

	[Space]
	[Header ("Gravity At Start Settings")]
	[Space]

	public bool startWithNewGravity;
	public bool useVehicleRotation;
	public bool adjustRotationToSurfaceFound;
	public Vector3 newGravityToStart;

	[Space]
	[Header ("Others Settings")]
	[Space]

	public otherSettings settings;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool gravityControlEnabled;
	public bool OnGround;
	public bool gravityPowerActive;

	public bool powerActivated;
	public bool recalculatingSurface;
	public bool searchingSurface;
	public bool searchingNewSurfaceBelow;
	public bool searchAround;
	public bool rotating;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform gravityAdherenceRaycastParent;
	public Transform surfaceBelowRaycastTransform;

	public vehicleController mainVehicleController;
	public Collider gravityCenterCollider;
	public Rigidbody mainRigidbody;
	public vehicleCameraController vehicleCameraManager;
	public vehicleHUDManager HUDManager;
	public inputActionManager actionManager;

	Vector3 previousNormalDetected;

	Transform vehicleCamera;

	bool circumnavigableSurfaceFound;
	bool conservateSpeed;
	bool accelerating;
	Vector3 forwardAxisCamera;
	Vector3 rightAxisCamera;
	Vector3 forwardAxisMovement;
	Vector3 surfaceNormal;
	Vector3 previousVelocity;
	float normalGravityMultiplier;
	float horizontalAxis;
	float verticalAxis;
	Transform pivot;

	GameObject fatherDetected;

	RaycastHit hit;
	Coroutine rotateCharacterState;

	Vector2 axisValues;

	Coroutine rotateVehicleCoroutine;

	bool gravityForcePaused;

	bool onGroundChecked;
	float currentCircumnagivateRotationSpeed;

	bool surfaceFound;
	RaycastHit currentSurfaceFound;
	Vector3 currentPosition;
	Vector3 rayPosition;
	Vector3 rayDirection;
	float gravityAdherenceRaycastParentAngleRotation;

	bool checkDownSpeedActive = true;

	GameObject currentObjectBelowVehicle;
	GameObject previousSurfaceBelowVehicle;

	Vector3 pos;
	Vector3 dir;

	Vector3 currentUp;

	bool mainVehicleControllerLocated;

	float originalGravityForce;

	void Start ()
	{
		//set the current normal in the vehicle controller
		if (mainVehicleController == null) {
			mainVehicleController = GetComponent<vehicleController> ();
		}

		mainVehicleControllerLocated = mainVehicleController != null;

		if (mainVehicleControllerLocated) {
			mainVehicleController.setNormal (currentNormal);
		}

		if (startWithNewGravity) {
			if (useVehicleRotation) {
				currentUp = transform.up;

				Vector3 normalDirection = -currentUp;

				if (adjustRotationToSurfaceFound) {
					if (Physics.Raycast (transform.position, -currentUp, out hit, Mathf.Infinity, settings.layer)) {
						normalDirection = hit.normal;
					}
				}

				if (currentUp != currentNormal) {
					setCustomNormal (normalDirection);
				}
			} else {
				setCustomNormal (newGravityToStart);
			}
		}

		vehicleCamera = vehicleCameraManager.transform;

		originalGravityForce = gravityForce;
	}

	void Update ()
	{
		currentPosition = transform.position;

		//if the vehicle is searching a new surface
		if (searchingSurface) {
			//set the vehicle movement direction in the air (local X and Y axis)
			forwardAxisCamera = pivot.TransformDirection (Vector3.up);
			rightAxisCamera = pivot.TransformDirection (Vector3.right);

			//new position and direction of a raycast to detect a surface
			pos = Vector3.zero;
			dir = Vector3.zero;

			//if the player has activate the gravity control in the vehicle, then apply force in the camera direction and set the raycast position in the center of mass
			if (!searchingNewSurfaceBelow) {
				//this function apply force to the vehicle in the new direction and allows to move it in the air in the left, right, forward and backward direction using the direction
				//of the movement as local Y axis
				moveInTheAir (forwardAxisMovement * (gravityForce * (mainRigidbody.mass / settings.massDivider) * settings.speed), settings.speed, mainRigidbody.mass / settings.massDivider, forwardAxisMovement);

				pos = settings.centerOfMass.position;

				dir = forwardAxisMovement;
			} 
			//else, the player is falling in the air while the gravity control was enabled, so the ray direction is the negavite local Y axis or -currentNormal and the position is the 
			//vehicle itself
			else {    
				pos = currentPosition;
				dir = -currentNormal;
			}

			if (showGizmo) {
				Debug.DrawRay (pos, dir * settings.vehicleRadius, Color.yellow);
			}

			//if the raycast found a new surface, then
			if (Physics.Raycast (pos, dir, out hit, settings.vehicleRadius, settings.layer)) {
				//check is not a trigger or a rigidbody
				if (!hit.collider.isTrigger && hit.rigidbody == null) {
					//a new valid surface has been detected, so stop the search of a new surface
				
					searchingNewSurfaceBelow = false;
					searchingSurface = false;
					searchAround = false;
					powerActivated = false;

					//set to 0 the vehicle velocity
					mainRigidbody.velocity = Vector3.zero;

					//disable the collider in the center of mass
					gravityCenterCollider.enabled = false;

					//if the surface can be circumnavigate
					if (hit.collider.gameObject.CompareTag (settings.tagForCircumnavigate)) {
						circumnavigableSurfaceFound = true;
					}

					//if the surface is moving
					if (hit.collider.gameObject.CompareTag (settings.tagForMovingObjects)) {
						addParent (hit.collider.gameObject);
					}	

					//if the new normal is different from the current normal, which means that is other surface inclination, then 
					if (hit.normal != currentNormal) {
						//rotate the vehicle according to that normal
						if (rotateVehicleCoroutine != null) {
							StopCoroutine (rotateVehicleCoroutine);
						}

						rotateVehicleCoroutine = StartCoroutine (rotateToSurface (hit.normal, 2)); 
					}

					//disable the gravity control in the vehicle controller
					if (mainVehicleControllerLocated) {
						mainVehicleController.changeGravityControlUse (false);
					}

					vehicleCameraManager.usingBoost (false, "StopShake", true, true);
				}
			}
		}

		//if the gravity control is enabled, and the vehicle falls in its negative loxal Y axis, 
		//then check if there is any surface below the vehicle which will become the new ground surface
		//in case the vehicle reachs a certain velocity
		if (!OnGround && !powerActivated) {
			if (checkDownSpeedActive && shakeCameraOnHighFallSpeed && transform.InverseTransformDirection (mainRigidbody.velocity).y < -minSpeedToShakeCamera) {
				vehicleCameraManager.usingBoost (true, "Regular Gravity Control", true, true);
			}

			if (searchNewSurfaceOnHighFallSpeed && transform.InverseTransformDirection (mainRigidbody.velocity).y < -settings.velocityToSearch && !searchingNewSurfaceBelow && !powerActivated) {
				//check that the current normal is not the regular one
				if (currentNormal != regularGravity) {
					//enable the collider in the center of mass
					gravityCenterCollider.enabled = true;

					//searching a new surface
					searchingNewSurfaceBelow = true;
					searchingSurface = true;

					//stop to recalculate the normal of the vehicle
					recalculatingSurface = false;
					circumnavigableSurfaceFound = false;

					//enable the gravity control in the vehicle controller
					if (mainVehicleControllerLocated) {
						mainVehicleController.changeGravityControlUse (true);
					}
				}
			}
		}

		//if the vehicle is above a circumnavigable object, then recalculate the current normal of the vehicle according to the surface normal under the vehicle
		if (!searchingSurface && (circumnavigableSurfaceFound || fatherDetected != null) && recalculatingSurface) {
			//set the distance of the ray, if the vehicle is not on the ground, the distance is higher
			float distance = settings.rayDistance + 0.05f;
			if (!OnGround) {
				distance = 10;
			}

			surfaceFound = false;

			//if the vehicle founds a surface below it, get its normal
			if (Physics.Raycast (settings.centerOfMass.position, -currentNormal, out hit, distance, settings.layer)) {
				currentSurfaceFound = hit;
				surfaceFound = true;
				currentCircumnagivateRotationSpeed = circumnavigateRotationSpeed;
			}

			currentUp = transform.up;

			//Get the correct raycast orientation according to input, for example, it is no the same ray position and direction if the vehicle is moving forward or backward
			if (checkSurfaceBelowLedge) {
				rayPosition = currentPosition + currentUp * 0.1f;

				gravityAdherenceRaycastParentAngleRotation = Vector3.SignedAngle (transform.forward, mainRigidbody.velocity, currentUp);

				gravityAdherenceRaycastParent.localRotation = Quaternion.Euler (new Vector3 (0, gravityAdherenceRaycastParentAngleRotation, 0));
			}

			if (checkSurfaceBelowLedge) {
				rayDirection = -currentUp;

				if (showGizmo) {
					Debug.DrawRay (rayPosition, rayDirection, Color.white);
				}

				if (!Physics.Raycast (rayPosition, rayDirection, out hit, 1, settings.layer)) {
					if (showGizmo) {
						Debug.DrawRay (rayPosition, rayDirection, Color.green);
					}

					rayPosition = surfaceBelowRaycastTransform.position;
					rayDirection = surfaceBelowRaycastTransform.forward;

					if (Physics.Raycast (rayPosition, rayDirection, out hit, surfaceBelowLedgeRaycastDistance, settings.layer)) {
						if (showGizmo) {
							Debug.DrawRay (rayPosition, rayDirection * hit.distance, Color.yellow);
						}

						currentSurfaceFound = hit;
						surfaceFound = true;
						currentCircumnagivateRotationSpeed = belowLedgeRotationSpeed;
					} else {
						if (showGizmo) {
							Debug.DrawRay (rayPosition, rayDirection * surfaceBelowLedgeRaycastDistance, Color.red);
						}
					}
				} else {
					if (showGizmo) {
						Debug.DrawRay (rayPosition, rayDirection * hit.distance, Color.red);
					}
				}
			}

			if (surfaceFound) {
				if (!currentSurfaceFound.collider.isTrigger && currentSurfaceFound.rigidbody == null) {
					//the object detected can be circumnavigate, so get the normal direction
					if (currentSurfaceFound.collider.gameObject.CompareTag (settings.tagForCircumnavigate)) {
						surfaceNormal = currentSurfaceFound.normal;
					}

					//the object is moving, so get the normal direction and set the player as a children of the moving obejct
					else if (currentSurfaceFound.collider.gameObject.CompareTag (settings.tagForMovingObjects)) {
						surfaceNormal = currentSurfaceFound.normal;

						if (fatherDetected == null) {
							addParent (currentSurfaceFound.collider.gameObject);
						}
					} 
				}
			} else {
				if (fatherDetected != null) {
					removeParent ();
				}
			}


			if (useLerpRotation) {
				//set the current normal according to the hit.normal
				currentNormal = Vector3.Lerp (currentNormal, surfaceNormal, currentCircumnagivateRotationSpeed * Time.deltaTime);

				if (Vector3.Angle (previousNormalDetected, surfaceNormal) > minAngleDifferenceToAdhereToSurface) {
					previousNormalDetected = surfaceNormal;

					Vector3 myForward = Vector3.Cross (transform.right, currentNormal);
					Quaternion dstRot = Quaternion.LookRotation (myForward, currentNormal); 
					transform.rotation = Quaternion.Lerp (transform.rotation, dstRot, currentCircumnagivateRotationSpeed * Time.deltaTime);
					Vector3 myForwardCamera = Vector3.Cross (vehicleCamera.right, currentNormal);
					Quaternion dstRotCamera = Quaternion.LookRotation (myForwardCamera, currentNormal);
					vehicleCamera.rotation = Quaternion.Lerp (vehicleCamera.rotation, dstRotCamera, currentCircumnagivateRotationSpeed * Time.deltaTime);
				}
			} else {
				//set the current normal according to the hit.normal
				currentNormal = Vector3.Slerp (currentNormal, surfaceNormal, currentCircumnagivateRotationSpeed * Time.deltaTime);

				if (Vector3.Angle (previousNormalDetected, currentNormal) > minAngleDifferenceToAdhereToSurface) {
					previousNormalDetected = surfaceNormal;

					Vector3 myForward = Vector3.Cross (transform.right, currentNormal);
					Quaternion dstRot = Quaternion.LookRotation (myForward, currentNormal); 
					transform.rotation = Quaternion.Slerp (transform.rotation, dstRot, currentCircumnagivateRotationSpeed * Time.deltaTime);
				}
					
				Vector3 myForwardCamera = Vector3.Cross (vehicleCamera.right, currentNormal);
				Quaternion dstRotCamera = Quaternion.LookRotation (myForwardCamera, currentNormal);

				vehicleCamera.rotation = Quaternion.Slerp (vehicleCamera.rotation, dstRotCamera, currentCircumnagivateRotationSpeed * Time.deltaTime);
			}

			if (mainVehicleControllerLocated) {
				mainVehicleController.setNormal (currentNormal);
			}
		}

		//if the vehicle is being rotated to a new surface, set its velocity to 0
		if (rotating) {
			mainRigidbody.velocity = Vector3.zero;
		}

		if ((gravityPowerActive || circumnavigateCurrentSurfaceActive) && !powerActivated) {
			if (currentObjectBelowVehicle != null) {
				//check if the object detected can be circumnavigate
				if (currentObjectBelowVehicle.CompareTag (settings.tagForCircumnavigate)) {
					circumnavigableSurfaceFound = true;
				} else {
					circumnavigableSurfaceFound = false;
				}

				//check if the object is moving to parent the player inside it
				if (currentObjectBelowVehicle.CompareTag (settings.tagForMovingObjects)) {
					if (previousSurfaceBelowVehicle != currentObjectBelowVehicle) {
						previousSurfaceBelowVehicle = currentObjectBelowVehicle;

						addParent (currentObjectBelowVehicle);
					}
				} else if (fatherDetected != null && !currentObjectBelowVehicle.transform.IsChildOf (fatherDetected.transform)) {
					removeParent ();
				}

				//if the surface where the player lands can be circumnavigated or an moving/rotating object, then keep recalculating the player throught the normal surface
				if (circumnavigableSurfaceFound || fatherDetected != null) {
					recalculatingSurface = true;
				} 
				//else disable this state
				else {
					recalculatingSurface = false;
				}
			} else {
				if (previousSurfaceBelowVehicle != null) {
					previousSurfaceBelowVehicle = null;
				}

				if (rotating) {
					circumnavigableSurfaceFound = false;
				}
			}
		} else {
			if ((circumnavigableSurfaceFound || fatherDetected != null) && rotating) {
				circumnavigableSurfaceFound = false;
			}

			if (checkSurfaceBelowOnRegularState && !gravityPowerActive && !HUDManager.isVehicletAsChildOfParent ()) {
				if (currentObjectBelowVehicle != null) {
					//check if the object is moving to parent the player inside it
					if (currentObjectBelowVehicle.CompareTag (settings.tagForMovingObjects)) {
						if (previousSurfaceBelowVehicle != currentObjectBelowVehicle) {
							previousSurfaceBelowVehicle = currentObjectBelowVehicle;

							addParent (currentObjectBelowVehicle);

							lastTimeParentFound = Time.time;
						}
					} else if (fatherDetected != null && !currentObjectBelowVehicle.transform.IsChildOf (fatherDetected.transform)) {
						removeParent ();
					}

				} else {
					if (previousSurfaceBelowVehicle != null) {
						previousSurfaceBelowVehicle = null;
					}

					if (Time.time > timeToSetNullParentOnAir + lastTimeParentFound) {
						removeParent ();
					}
				}
			}
		}
	}

	void FixedUpdate ()
	{
		// If the gravity control is not being used
		if (!powerActivated && !gravityForcePaused) {
			// Apply force to the vehicle in the negative local Y axis, so the vehicle has a regular gravity
			if (useGravity) {
				mainRigidbody.AddForce (currentNormal * (-gravityForce * mainRigidbody.mass * settings.gravityMultiplier));
			}

			// If the vehicle is search a new surface, and previously the gravity control was enabled, then 
			if (searchingNewSurfaceBelow) {
				// Apply force in the negative local Y axis of the vehicle, and allow to move it left, right, backward and forward
				moveInTheAir (mainRigidbody.velocity, 1, 1, -currentNormal);
			} 

			// Check if the vehicle is on the ground
			OnGround = false;

			// Use a raycast to it, with the center of the mass as position and the negative local Y axis as direction, using the ray distance configured in the inspector
			if (Physics.Raycast (settings.centerOfMass.position, -currentNormal, out hit, settings.rayDistance, settings.layer)) {
				if (showGizmo) {
					Debug.DrawLine (settings.centerOfMass.position, hit.point, Color.cyan);
				}

				// If the hit is not a trigger
				if (!hit.collider.isTrigger) {
					// The vehicle is on the ground
					OnGround = true;

					currentObjectBelowVehicle = hit.collider.gameObject;
				}
			} else {
				// Else the vehicle is in the air
				OnGround = false;

				currentObjectBelowVehicle = null;
			}
		} else {
			// The vehicle is searching a new surface, so it is in the air
			OnGround = false;
		}

		if (OnGround) {
			if (!onGroundChecked) {

				vehicleCameraManager.usingBoost (false, "stopShake", true, true);

				onGroundChecked = true;

				// If the gravity control is enabled, and the surface below the vehicle can be circumnavigate or is a moving object, allow to recalculate the normal
				if ((circumnavigableSurfaceFound || fatherDetected != null) && gravityPowerActive) {
					recalculatingSurface = true;
				} else {
					recalculatingSurface = false;
				}
			}
		} else {
			if (onGroundChecked) {
				onGroundChecked = false;
			}
		}
	}

	void moveInTheAir (Vector3 newVel, float speedMultiplier, float massMultiplier, Vector3 movementDirection)
	{
		//get the new velocity to apply to the vehicle
		Vector3 newVelocity = newVel;

		//get the current values of the horizontal and vertical axis, from the input manager
		if (actionManager != null) {
			axisValues = actionManager.getPlayerMovementAxis ();
		} else {
			axisValues = Vector3.zero;
		}

		horizontalAxis = axisValues.x;
		verticalAxis = axisValues.y;

		//allow to move the vehicle in its local X and Y axis while he falls or move in the air using the gravity control
		Vector3 newmoveInput = verticalAxis * forwardAxisCamera + horizontalAxis * rightAxisCamera;

		if (newmoveInput.magnitude > 1) {
			newmoveInput.Normalize ();
		}

		//if the input axis are being used, set that movement to the vehicle
		if (newmoveInput.magnitude > 0) {
			newVelocity += newmoveInput * (settings.speed * (mainRigidbody.mass / settings.massDivider) * speedMultiplier);
		}

		//if the player is accelerating in the air, add more velocity to the vehicle
		if (accelerating) {
			newVelocity += forwardAxisMovement * (settings.accelerateSpeed * massMultiplier);
		}

		//if the current local Y velocity is lower than the limit, clamp the velocity
		if (Mathf.Abs (transform.InverseTransformDirection (newVelocity).y) > 40) {
			newVelocity -= movementDirection * Mathf.Abs (transform.InverseTransformDirection (newVelocity).y);
			newVelocity += movementDirection * 40;
		}

		//set the new velocity to the vehicle
		mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, newVelocity, Time.deltaTime * 2);
	}

	//when the colliders of the vehicle hits another surface
	public void setCollisionDetected (Collision currentCollision)
	{
		//if the vehicle was searching a new surface using the gravity control, then
		if (searchingSurface && searchAround) {
			//check the type of collider
			if (!currentCollision.gameObject.CompareTag ("Player") && currentCollision.rigidbody == null && !currentCollision.collider.isTrigger) {
				//using the collision contact, get the direction of that new surface, so the vehicle change its movement to the position of that surface
				//like this the searching of a new surface is more accurate, since a raycast and collisions are used to detect the new surface
				Vector3 hitDirection = currentCollision.contacts [0].point - settings.centerOfMass.position;

				//set the current direction of the vehicle
				hitDirection = hitDirection / hitDirection.magnitude;
				forwardAxisMovement = hitDirection;

				//stop to search collisions around the vehicle
				searchAround = false;
			}
		}
	}

	//set and remove the parent of the vehicle according to the type of surface
	void addParent (GameObject obj)
	{
		fatherDetected = obj;

		parentAssignedSystem currentParentAssignedSystem = obj.GetComponent<parentAssignedSystem> ();

		if (currentParentAssignedSystem != null) {
			fatherDetected = currentParentAssignedSystem.getAssignedParent ();
		}

		setFatherDetectedAsParent ();
	}

	void removeParent ()
	{
		setNullParent ();

		fatherDetected = null;
	}

	public void setFatherDetectedAsParent ()
	{
		transform.SetParent (fatherDetected.transform);

		vehicleCamera.SetParent (fatherDetected.transform);
	}

	public void setNullParent ()
	{
		transform.SetParent (null);
		vehicleCamera.SetParent (null);
	}

	//activate the collider in the center of mass of the vehicle to dectect collisions with a new surface when the gravity control is enabled
	IEnumerator activateCollider ()
	{
		//wait to avoid enable the collider when the vehicle stills in the ground
		yield return new WaitForSeconds (0.2f);

		//if the vehicle is searching surface, then active the collider
		if (searchingSurface || searchingNewSurfaceBelow || searchAround) {
			gravityCenterCollider.enabled = true;
		}
	}

	//enable the gravity control in the direction of the camera
	public void activateGravityPower (Vector3 dir, Vector3 right)
	{
		StartCoroutine (activateCollider ());

		searchingNewSurfaceBelow = false;

		removeParent ();

		circumnavigableSurfaceFound = false;	

		searchingSurface = true;

		searchAround = true;

		gravityPowerActive = true;

		forwardAxisMovement = dir;

		rightAxisCamera = right;

		powerActivated = true;

		//enable the gravity control in the vehicle controller
		vehicleCameraManager.usingBoost (true, "Regular Gravity Control", true, true);

		if (mainVehicleControllerLocated) {
			mainVehicleController.changeGravityControlUse (true);
		}
	}

	//disaable the gravity control, rotatin the vehicle to the regular gravity
	public void deactivateGravityPower ()
	{
		//check that the vehicle was searching a new surface, or that the gravity is different than vector3.up
		if (searchingSurface || searchingNewSurfaceBelow || searchAround || currentNormal != regularGravity) {
			conservateSpeed = true;

			gravityCenterCollider.enabled = false;

			accelerating = false;

			circumnavigableSurfaceFound = false;	

			removeParent ();

			searchingNewSurfaceBelow = false;

			searchingSurface = false;

			searchAround = false;

			recalculatingSurface = false;

			powerActivated = false;

			//if the current normal before reset the gravity to the regular one is different from vectore.up, then rotate the vehicle back to the regular state
			if (currentNormal != regularGravity) {
				if (rotateVehicleCoroutine != null) {
					StopCoroutine (rotateVehicleCoroutine);
				}

				rotateVehicleCoroutine = StartCoroutine (rotateToSurface (regularGravity, 2));
			}

			gravityPowerActive = false;

			//set the normal in the vehicle controller
			if (mainVehicleControllerLocated) {
				mainVehicleController.setNormal (regularGravity);
			}

			//disable the gravity control in the vehicle controller
			vehicleCameraManager.usingBoost (false, "stopShake", true, true);

			if (mainVehicleControllerLocated) {
				mainVehicleController.changeGravityControlUse (false);
			}
		}
	}

	//the player is getting on or off from the vehicle, so enable or disable the graivty control component
	public void changeGravityControlState (bool state)
	{
		gravityControlEnabled = state;

		//if the vehicle is not being driving, and it wasn't in the ground, deactivate the gravity control
		if (!gravityControlEnabled && !OnGround) {
			deactivateGravityPower ();

			accelerating = false;
		}
	}

	//rotate the vehicle and its camera to the new found surface, using the normal of that surface
	public IEnumerator rotateToSurface (Vector3 normal, int rotSpeed)
	{
		previousVelocity = mainRigidbody.velocity;

		//the vehicle is being rotate, so set its velocity to 0
		rotating = true;

		currentNormal = normal; 

		//set the new normal in the vehicle controller
		if (mainVehicleControllerLocated) {
			mainVehicleController.setNormal (currentNormal);
		}

		//get the current rotation of the vehicle and the camera
		Quaternion currentVehicleRotation = transform.rotation;
		Quaternion currentVehicleCameraRotation = vehicleCamera.rotation;

		Vector3 currentVehicleForward = Vector3.Cross (transform.right, normal);
		Quaternion targetVehicleRotation = Quaternion.LookRotation (currentVehicleForward, normal);
		Vector3 currentVehicleCameraForward = Vector3.Cross (vehicleCamera.right, normal);
		Quaternion targetVehicleCameraRotation = Quaternion.LookRotation (currentVehicleCameraForward, normal);

		//rotate from their rotation to thew new surface normal direction
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * rotSpeed;

			vehicleCamera.rotation = Quaternion.Slerp (currentVehicleCameraRotation, targetVehicleCameraRotation, t);
			transform.rotation = Quaternion.Slerp (currentVehicleRotation, targetVehicleRotation, t);

			yield return null;
		}

		rotating = false;

		//store the current vehicle velocity to set it again once the rotation is finished, is only applied when the gravity control is disabled
		if (conservateSpeed) {
			mainRigidbody.velocity = previousVelocity;
		}

		conservateSpeed = false;
	}

	public void rotateVehicleToLandSurface (Vector3 hitNormal)
	{
		if (rotateVehicleCoroutine != null) {
			StopCoroutine (rotateVehicleCoroutine);
		}

		rotateVehicleCoroutine = StartCoroutine (rotateVehicleToLandSurfaceCoroutine (hitNormal));
	}

	public IEnumerator rotateVehicleToLandSurfaceCoroutine (Vector3 hitNormal)
	{
		rotating = true;

		currentNormal = hitNormal; 

		//set the new normal in the vehicle controller
		if (mainVehicleControllerLocated) {
			mainVehicleController.setNormal (currentNormal);
		}

		//get the current rotation of the vehicle and the camera
		Quaternion currentVehicleRotation = transform.rotation;
		Quaternion currentVehicleCameraRotation = vehicleCamera.rotation;
		Vector3 currentVehicleForward = Vector3.Cross (transform.right, hitNormal);
		Quaternion targetVehicleRotation = Quaternion.LookRotation (currentVehicleForward, hitNormal);
		Vector3 currentVehicleCameraForward = Vector3.Cross (vehicleCamera.right, hitNormal);
		Quaternion targetVehicleCameraRotation = Quaternion.LookRotation (currentVehicleCameraForward, hitNormal);

		//rotate from their rotation to thew new surface normal direction
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * 2;

			vehicleCamera.rotation = Quaternion.Slerp (currentVehicleCameraRotation, targetVehicleCameraRotation, t);
			transform.rotation = Quaternion.Slerp (currentVehicleRotation, targetVehicleRotation, t);

			yield return null;
		}

		rotating = false;

		pauseDownForce (false);
	}

	//get the current camera pivot, according to the vehicle view, in fisrt or third person
	public void getCurrentCameraPivot (Transform newPivot)
	{
		pivot = newPivot;
	}

	public void pauseDownForce (bool state)
	{
		useGravity = !state;
	}

	public void setCustomNormal (Vector3 normal)
	{
		Quaternion currentVehicleRotation = transform.rotation;
		Quaternion currentVehicleCameraRotation = vehicleCamera.rotation;
		Vector3 currentVehicleForward = Vector3.Cross (transform.right, normal);
		Quaternion targetVehicleRotation = Quaternion.LookRotation (currentVehicleForward, normal);
		Vector3 currentVehicleCameraForward = Vector3.Cross (vehicleCamera.right, normal);
		Quaternion targetVehicleCameraRotation = Quaternion.LookRotation (currentVehicleCameraForward, normal);

		transform.rotation = targetVehicleRotation;
		vehicleCamera.rotation = targetVehicleCameraRotation;
		currentNormal = normal;
	}

	public override Vector3 getCurrentNormal ()
	{
		return currentNormal;
	}

	public float getGravityForce ()
	{
		return gravityForce;
	}

	public void setNewGravityForce (float newValue)
	{
		gravityForce = newValue;
	}

	public void setOriginalGravityForce ()
	{
		setNewGravityForce (originalGravityForce);
	}

	public bool isUsingRegularGravity ()
	{
		return currentNormal == regularGravity;
	}

	public bool changeDriverGravityWhenGetsOffActive ()
	{
		return changeDriverGravityWhenGetsOff;
	}

	public void setGravityForcePausedState (bool state)
	{
		gravityForcePaused = state;
	}

	public bool isGravityPowerActive ()
	{
		return gravityPowerActive;
	}

	//CALL INPUT FUNCTIONS
	public void inputEnableOrDisableGravityControl (bool enableGravityControl)
	{
		//if the vehicle is being driving
		if (gravityControlEnabled && settings.canUseGravityControl) {
			//activate the gravity control to search a new surface in the camera direction
			if (enableGravityControl) {
				activateGravityPower (pivot.TransformDirection (Vector3.forward), pivot.TransformDirection (Vector3.right));
			} else {
				//deactivate the gravity control
				deactivateGravityPower ();
			}
		}
	}

	public void inputIncreasOrDecreaseGravitySpeed (bool increaseGravitySpeed)
	{
		if (gravityControlEnabled && settings.canUseGravityControl) {
			if (searchingSurface || searchingNewSurfaceBelow || gravityPowerActive) {
				//if the vehicle is searching a new surface, increase its velocity, only when the gravity power is active
				if (increaseGravitySpeed) {
					accelerating = true;

					vehicleCameraManager.usingBoost (true, "Quick Gravity Control", true, true);
				} else {
					//stop to increase the velocity of the vehiclee in the air
					accelerating = false;

					vehicleCameraManager.usingBoost (true, "Regular Gravity Control", true, true);
				}
			}
		}
	}

	public void setCheckDownSpeedActiveState (bool state)
	{
		checkDownSpeedActive = state;
	}

	//draw the ray distance radius in the vehicle
	void OnDrawGizmosSelected ()
	{
		if (showGizmo) {
			if (settings.centerOfMass != null) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere (settings.centerOfMass.position, settings.vehicleRadius);

				Gizmos.color = Color.cyan;
				Gizmos.DrawLine (settings.centerOfMass.position, settings.centerOfMass.position - settings.centerOfMass.up * settings.rayDistance);
			}
		}
	}

	[System.Serializable]
	public class otherSettings
	{
		public LayerMask layer;
		public float speed = 10;
		public float accelerateSpeed = 20;
		public float velocityToSearch = 10;
		public float gravityMultiplier = 1;
		public float rayDistance;
		public float vehicleRadius;
		public Transform centerOfMass;
		public bool canUseGravityControl;
		public float massDivider = 1000;
		public string tagForCircumnavigate = "sphere";
		public string tagForMovingObjects = "moving";
	}
}