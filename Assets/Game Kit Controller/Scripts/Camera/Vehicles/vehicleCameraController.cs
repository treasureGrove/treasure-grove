using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class vehicleCameraController : cameraControllerManager
{
	public float firstPersonVerticalRotationSpeed = 5;
	public float firstPersonHorizontalRotationSpeed = 5;
	public float thirdPersonVerticalRotationSpeed = 5;
	public float thirdPersonHorizontalRotationSpeed = 5;

	public float currentVerticalRotationSpeed;
	public float currentHorizontalRotationSpeed;

	float originalFirstPersonVerticalRotationSpeed;
	float originalFirstPersonHorizontalRotationSpeed;
	float originalThirdPersonVerticalRotationSpeed;
	float originalThirdPersonHorizontalRotationSpeed;

	public bool useSmoothCameraRotation;
	public bool useSmoothCameraRotationThirdPerson;
	public float smoothCameraRotationSpeedVerticalThirdPerson = 10;
	public float smoothCameraRotationSpeedHorizontalThirdPerson = 10;
	public bool useSmoothCameraRotationFirstPerson;
	public float smoothCameraRotationSpeedVerticalFirstPerson = 10;
	public float smoothCameraRotationSpeedHorizontalFirstPerson = 10;
	float currentCameraUpRotation;

	float currentSmoothCameraRotationSpeedVertical;
	float currentSmoothCameraRotationSpeedHorizontal;
	Quaternion currentPivotRotation;

	public float clipCastRadius = 0.16f;
	public float backClipSpeed;
	public float maximumBoostDistance;
	public float cameraBoostSpeed;
	public float smoothBetweenState;
	public string defaultStateName = "Third Person";
	public string currentStateName;

	public int cameraStateIndex;

	public List<vehicleCameraStateInfo> vehicleCameraStates = new List<vehicleCameraStateInfo> ();
	public shakeSettingsInfo shakeSettings;

	public float gizmoRadius;
	public bool showGizmo;
	public Color labelGizmoColor;

	public GameObject vehicle;

	public Transform vehicleTransformToFollow;

	public LayerMask layer;
	public bool cameraChangeEnabled;
	public float rotationDamping = 3;
	public bool cameraPaused;

	public bool zoomEnabled;

	public bool isFirstPerson;
	public bool usingZoomOn;
	public vehicleCameraStateInfo currentState;
	public bool smoothTransitionsInNewCameraFov;

	public bool drivingVehicle;

	public bool sendCurrentCameraTransformOnChangeState;
	public eventParameters.eventToCallWithTransform eventToSendCurrentCameraTransformOnChangeState;

	public bool resetCameraRotationOnGetOff;

	public bool showCameraDirectionGizmo;
	public float gizmoArrowLength = 1;
	public float gizmoArrowLineLength = 2.5f;
	public float gizmoArrowAngle = 20;


	public bool useSmoothCameraFollow;
	public float smoothCameraFollowSpeed;
	public float smoothCameraFollowMaxDistance;
	public float smoothCameraFollowMaxDistanceSpeed;
	Vector3 cameraVelocity;

	public IKDrivingSystem IKDrivingManager;
	public vehicleWeaponSystem weaponManager;
	public vehicleHUDManager hudManager;
	public inputActionManager actionManager;
	public vehicleGravityControl gravityControl;
	public Rigidbody mainRigidbody;
	public vehicleCameraShake shakingManager;

	float cameraSpeed;

	float currentCameraDistance;
	float originalCameraDistance;
	float currentOriginalDistValue;

	float originalCameraFov;

	public Vector2 lookAngle;

	Vector2 mouseAxis;

	Vector3 currentPivotPosition;
	Vector3 nextPivotPositon;
	bool boosting;
	bool releaseCamera;
	bool pickCamera;
	bool followVehiclePosition = true;
	bool firstCameraEnabled;
	Ray ray;
	RaycastHit[] hits;

	Coroutine moveCamera;

	GameObject player;
	playerController playerManager;
	playerCamera playerCameraManager;
	Camera mainCamera;
	Transform mainCameraTransform;

	Vector2 axisValues;

	float fixedRotationAmount;
	float vehicleSpeed;
	float forwadDirection;
	float upRotationAmount;

	public bool showShakeSettings;

	bool playerCameraIsLocked;

	bool isGamePaused;
	bool checkCollision;

	bool cameraShakeLocated;

	void Start ()
	{
		if (vehicleTransformToFollow == null) {
			vehicleTransformToFollow = vehicle.transform;
		}

		cameraShakeLocated = shakingManager != null;

		for (int i = 0; i < vehicleCameraStates.Count; i++) {
			vehicleCameraStates [i].originalDist = vehicleCameraStates [i].cameraTransform.localPosition.magnitude;
			vehicleCameraStates [i].originalPivotPosition = vehicleCameraStates [i].pivotTransform.localPosition;
		}

		//get the main components of the camera, like the pivot and the transform which contains the main camera when the player is driving this vehicle
		setCameraState (currentStateName);

		//get the current local position of the camera
		originalCameraDistance = currentState.cameraTransform.localPosition.magnitude;

		//if the vehicle has a weapon system, store it
		if (weaponManager != null) {
			//set the current camera used in the vehicle in the weapon component
			weaponManager.getCameraInfo (currentState.cameraTransform, currentState.useRotationInput);
		}

		//get the original local position of the pivot
		if (gravityControl != null) {
			gravityControl.getCurrentCameraPivot (currentState.pivotTransform);
		}
			
		originalThirdPersonVerticalRotationSpeed = thirdPersonVerticalRotationSpeed;
		originalThirdPersonHorizontalRotationSpeed = thirdPersonHorizontalRotationSpeed;
		originalFirstPersonVerticalRotationSpeed = firstPersonVerticalRotationSpeed;
		originalFirstPersonHorizontalRotationSpeed = firstPersonHorizontalRotationSpeed;
	}

	void Update ()
	{
		//set the camera position in the vehicle position to follow it
		if (followVehiclePosition) {

			if (useSmoothCameraFollow) {
				Vector3 positionToFollow = vehicleTransformToFollow.position;

				float speed = smoothCameraFollowSpeed;

				float distance = GKC_Utils.distance (transform.position, positionToFollow);

				if (distance > smoothCameraFollowMaxDistance) {
					speed *= smoothCameraFollowMaxDistanceSpeed;
				}

				transform.position = Vector3.SmoothDamp (transform.position, positionToFollow, ref cameraVelocity, speed);
			} else {
				transform.position = vehicleTransformToFollow.position;
			}
		}
			
		if (drivingVehicle) {

			isGamePaused = actionManager.input.gameCurrentlyPaused;

			if (!isGamePaused && !cameraPaused) {
				checkCollision = false;

				if (currentState.cameraFixed) {
					calculateCameraFixedRotation ();

					checkCollision = true;
				} else {
					//if the first camera view is enabled
					if (!currentState.firstPersonCamera) {
						checkCollision = true;
					}
				}

				if (checkCollision) {
					//get the current camera position for the camera collision detection
					currentCameraDistance = checkCameraCollision ();

					//set the local camera position
					currentCameraDistance = Mathf.Clamp (currentCameraDistance, 0, originalCameraDistance);

					if (currentCameraDistance != float.NaN) {
						currentState.cameraTransform.localPosition = -Vector3.forward * currentCameraDistance;
					}
				}
			}
		}

		//if the boost is being used, move the camera in the backward direction
		if (boosting) {
			//the camera is moving in backward direction
			if (releaseCamera) {
				originalCameraDistance += Time.deltaTime * cameraBoostSpeed;

				if (originalCameraDistance >= maximumBoostDistance + currentState.originalDist) {
					originalCameraDistance = currentState.originalDist + maximumBoostDistance;
					releaseCamera = false;
				}
			}

			//the camera is moving to its regular position
			if (pickCamera) {
				originalCameraDistance -= Time.deltaTime * cameraBoostSpeed;

				if (originalCameraDistance <= currentState.originalDist) {
					originalCameraDistance = currentState.originalDist;
					pickCamera = false;
					boosting = false;
				}
			}
		}
	}

	void FixedUpdate ()
	{
		//if the vehicle is being driving and the pause menu is not active, allow the camera to rotate
		if (drivingVehicle) {

			if (!isGamePaused && !cameraPaused) {
				
				if (currentState.cameraFixed) {
					transform.Rotate (0, fixedRotationAmount, 0);
				} else {
					//get the current input axis values from the input manager
					axisValues = actionManager.getPlayerMouseAxis ();
					mouseAxis.x = axisValues.x;
					mouseAxis.y = axisValues.y;

					playerCameraIsLocked = !playerCameraManager.isCameraTypeFree ();

					checkCurrentRotationSpeed ();

					//if the first camera view is enabled
					if (currentState.firstPersonCamera) {
					
						isFirstPerson = true;

						if (currentState.useRotationInput) {

							//get the look angle value
							if (currentState.rotateMainCameraOnFirstPerson) {
								lookAngle.x = mouseAxis.x * currentHorizontalRotationSpeed;
							} else {
								lookAngle.x += mouseAxis.x * currentHorizontalRotationSpeed;
							}

							lookAngle.y -= mouseAxis.y * currentVerticalRotationSpeed;
						
							//clamp these values to limit the camera rotation
							lookAngle.y = Mathf.Clamp (lookAngle.y, -currentState.xLimits.x, currentState.xLimits.y);

							if (!currentState.rotateMainCameraOnFirstPerson) {
								lookAngle.x = Mathf.Clamp (lookAngle.x, -currentState.yLimits.x, currentState.yLimits.y);
							}

							//set every angle in the camera and the pivot
							if (useSmoothCameraRotation && useSmoothCameraRotationFirstPerson) {
								currentSmoothCameraRotationSpeedVertical = smoothCameraRotationSpeedVerticalFirstPerson;
								currentSmoothCameraRotationSpeedHorizontal = smoothCameraRotationSpeedHorizontalFirstPerson;

								if (currentState.rotateMainCameraOnFirstPerson) {

									currentPivotRotation = Quaternion.Euler (lookAngle.y, 0, 0);

									currentState.pivotTransform.localRotation = 
										Quaternion.Slerp (currentState.pivotTransform.localRotation, currentPivotRotation, currentSmoothCameraRotationSpeedVertical * Time.deltaTime);

									currentCameraUpRotation = Mathf.Lerp (currentCameraUpRotation, lookAngle.x, currentSmoothCameraRotationSpeedHorizontal * Time.deltaTime);

									transform.Rotate (0, currentCameraUpRotation, 0);		
								} else {

									currentPivotRotation = Quaternion.Euler (0, lookAngle.x, 0);

									currentState.pivotTransform.localRotation = 
								Quaternion.Slerp (currentState.pivotTransform.localRotation, currentPivotRotation, currentSmoothCameraRotationSpeedVertical * Time.deltaTime);
							
									currentCameraUpRotation = Mathf.Lerp (currentCameraUpRotation, lookAngle.y, currentSmoothCameraRotationSpeedVertical * Time.deltaTime);

									currentState.cameraTransform.localRotation = Quaternion.Euler (currentCameraUpRotation, 0, 0);	
								}
										
							} else {
								if (currentState.rotateMainCameraOnFirstPerson) {
									transform.Rotate (0, lookAngle.x, 0);
									currentState.pivotTransform.localRotation = Quaternion.Euler (lookAngle.y, 0, 0);
								} else {
									currentState.cameraTransform.localRotation = Quaternion.Euler (lookAngle.y, 0, 0);
									currentState.pivotTransform.localRotation = Quaternion.Euler (0, lookAngle.x, 0);
								}
							}
						}
					} else {
						//else, the camera is in third person view
						isFirstPerson = false;

						if (currentState.useRotationInput) {

							//get the look angle value
							lookAngle.x = mouseAxis.x * currentHorizontalRotationSpeed;
							lookAngle.y -= mouseAxis.y * currentVerticalRotationSpeed;

							//clamp these values to limit the camera rotation
							lookAngle.y = Mathf.Clamp (lookAngle.y, -currentState.xLimits.x, currentState.xLimits.y);

							//set every angle in the camera and the pivot
							if (useSmoothCameraRotation && useSmoothCameraRotationThirdPerson) {
								currentSmoothCameraRotationSpeedVertical = smoothCameraRotationSpeedVerticalThirdPerson;
								currentSmoothCameraRotationSpeedHorizontal = smoothCameraRotationSpeedHorizontalThirdPerson;

								currentPivotRotation = Quaternion.Euler (lookAngle.y, 0, 0);

								currentState.pivotTransform.localRotation = 
							Quaternion.Slerp (currentState.pivotTransform.localRotation, currentPivotRotation, currentSmoothCameraRotationSpeedVertical * Time.deltaTime);

								currentCameraUpRotation = Mathf.Lerp (currentCameraUpRotation, lookAngle.x, currentSmoothCameraRotationSpeedHorizontal * Time.deltaTime);

								transform.Rotate (0, currentCameraUpRotation, 0);					
							} else {
								transform.Rotate (0, lookAngle.x, 0);
								currentState.pivotTransform.localRotation = Quaternion.Euler (lookAngle.y, 0, 0);
							}
						}
					}

					if (currentState.useIdentityRotation || playerCameraIsLocked) {
						transform.rotation = Quaternion.identity;
					}
				}
			}
		}
	}

	void calculateCameraFixedRotation ()
	{
		upRotationAmount = Mathf.Asin (transform.InverseTransformDirection (Vector3.Cross (transform.right, vehicle.transform.right)).y) * Mathf.Rad2Deg;
	
		vehicleSpeed = (mainRigidbody.transform.InverseTransformDirection (mainRigidbody.velocity).z) * 3f;

		forwadDirection = 1;

		if (vehicleSpeed < -2) {
			forwadDirection = -1;
		}

		fixedRotationAmount = upRotationAmount * Time.fixedDeltaTime * rotationDamping * forwadDirection;

		if (float.IsNaN (fixedRotationAmount)) {
			fixedRotationAmount = 0;
		}

//		targetFixedRotationAmount = Mathf.Lerp (targetFixedRotationAmount, fixedRotationAmount, Time.deltaTime * rotationDamping);
	}

	public Vector3 getLookDirection ()
	{
		Vector3 newDirection = Vector3.zero;

		if (currentState.firstPersonCamera) {
			newDirection = currentState.pivotTransform.forward;
		} else {
			newDirection = transform.forward;
		}

		if (currentState.useDeadZoneForCameraSteer) {
			float currentAngle = Vector3.Angle (newDirection, Vector3.forward);

			if (currentAngle < currentState.deadZoneAngle) {
				newDirection = Vector3.forward;
			}
		}

		return newDirection;
	}

	public void setZoom (bool state)
	{
		if (zoomEnabled && currentState.canUseZoom) {
			//to the fieldofview of the camera, it is added of substracted the zoomvalue
			usingZoomOn = state;

			float targetFov = currentState.zoomFovValue;

			float verticalRotationSpeedTarget = currentState.verticalRotationSpeedZoomIn;
			float horizontalRotationSpeedTarget = currentState.horizontalRotationSpeedZoomIn;

			if (!usingZoomOn) {
				if (currentState.firstPersonCamera) {
					verticalRotationSpeedTarget = originalFirstPersonVerticalRotationSpeed;
					horizontalRotationSpeedTarget = originalFirstPersonHorizontalRotationSpeed;
				} else {
					verticalRotationSpeedTarget = originalThirdPersonVerticalRotationSpeed;
					horizontalRotationSpeedTarget = originalThirdPersonHorizontalRotationSpeed;
				}

				targetFov = originalCameraFov;
			}

			playerCameraManager.setMainCameraFov (targetFov, currentState.zoomSpeed);

			//also, change the sensibility of the camera when the zoom is on or off, to control the camera properly

			changeRotationSpeedValue (verticalRotationSpeedTarget, horizontalRotationSpeedTarget);
		}
	}

	public void disableZoom ()
	{
		if (usingZoomOn) {
			usingZoomOn = false;

			setOriginalRotationSpeed ();
		}
	}

	public void changeRotationSpeedValue (float newVerticalRotationValue, float newHorizontalRotationValue)
	{
		if (currentState.firstPersonCamera) {
			firstPersonVerticalRotationSpeed = newVerticalRotationValue;
			firstPersonHorizontalRotationSpeed = newHorizontalRotationValue;
		} else {
			thirdPersonVerticalRotationSpeed = newVerticalRotationValue;
			thirdPersonHorizontalRotationSpeed = newHorizontalRotationValue;
		}
	}

	public void setOriginalRotationSpeed ()
	{
		firstPersonVerticalRotationSpeed = originalFirstPersonVerticalRotationSpeed;
		firstPersonHorizontalRotationSpeed = originalFirstPersonHorizontalRotationSpeed;
		thirdPersonVerticalRotationSpeed = originalThirdPersonVerticalRotationSpeed;
		thirdPersonHorizontalRotationSpeed = originalThirdPersonHorizontalRotationSpeed;
	}

	public void checkCurrentRotationSpeed ()
	{
		if (currentState.firstPersonCamera) {
			currentVerticalRotationSpeed = firstPersonVerticalRotationSpeed;
			currentHorizontalRotationSpeed = firstPersonHorizontalRotationSpeed;
		} else {
			currentVerticalRotationSpeed = thirdPersonVerticalRotationSpeed;
			currentHorizontalRotationSpeed = thirdPersonHorizontalRotationSpeed;
		}
	}

	public void setCameraState (string stateName)
	{
		bool cameraStateFound = false;

		for (int i = 0; i < vehicleCameraStates.Count; i++) {
			if (!cameraStateFound && vehicleCameraStates [i].name.Equals (stateName)) {
				currentState = new vehicleCameraStateInfo (vehicleCameraStates [i]);
				currentStateName = stateName;

				cameraStateIndex = i;

				cameraStateFound = true;
			}
		}

		hudManager.disableUnlockedCursor ();
	
		disableZoom ();

		IKDrivingManager.setPassengerFirstPersonState (currentState.firstPersonCamera, player);

		if (sendCurrentCameraTransformOnChangeState) {
			eventToSendCurrentCameraTransformOnChangeState.Invoke (currentState.cameraTransform);
		}

		hudManager.setUseRightDirectionForCameraDirectionState (currentState.firstPersonCamera);

		hudManager.setUseForwardDirectionForCameraDirectionState (currentState.firstPersonCamera);

		hudManager.setAddExtraRotationPausedState (currentState.firstPersonCamera);
	}

	bool checkCameraShakeActive;

	public void setCheckCameraShakeActiveState (bool state)
	{
		checkCameraShakeActive = state;

		if (cameraShakeLocated) {
			shakingManager.setVehicleActiveState (state);
		}

		if (!checkCameraShakeActive) {
			releaseCamera = false;
			pickCamera = true;

			if (cameraShakeLocated) {
				shakingManager.stopShake ();
			}
		}
	}

	//function called when the player uses the boost in the vehicle
	public void usingBoost (bool state, string shakeName, bool useBoostCameraShake, bool moveCameraAwayOnBoost)
	{
		if (!checkCameraShakeActive) {
			return;
		}
			
		boosting = true;

		if (state) {
			if (moveCameraAwayOnBoost) {
				releaseCamera = true;

				pickCamera = false;
			}

			if (cameraShakeLocated) {
				if (useBoostCameraShake) {
					shakingManager.startShake (shakeName);
				}
			}
		} else {
			if (moveCameraAwayOnBoost) {
				releaseCamera = false;

				pickCamera = true;
			}

			if (cameraShakeLocated) {
				if (useBoostCameraShake) {
					shakingManager.stopShake ();
				}
			}
		}
	}

	//the player has changed the current camera view for the other option, firts or third
	public void changeCameraPosition ()
	{
		if (!playerCameraManager.isCameraTypeFree ()) {
			print ("camera is locked");

			return;
		}

		//if the camera can be changed
		if (cameraChangeEnabled) {
			cameraStateIndex++;

			if (cameraStateIndex > vehicleCameraStates.Count - 1) {
				cameraStateIndex = 0;
			}

			bool exit = false;
			int max = 0;

			while (!exit) {
				for (int k = 0; k < vehicleCameraStates.Count; k++) {
					if (vehicleCameraStates [k].enabled && k == cameraStateIndex) {
						cameraStateIndex = k;
						exit = true;
					}
				}

				if (!exit) {
					max++;

					if (max > 100) {
						print ("error");
						return;
					}

					//set the current power
					cameraStateIndex++;

					if (cameraStateIndex > vehicleCameraStates.Count - 1) {
						cameraStateIndex = 0;
					}
				}
			}

			updateCameraPositionOnCameraStateChange (false);
		}
	}

	void updateCameraPositionOnCameraStateChange (bool useSmoothTransition)
	{
		nextPivotPositon = currentState.pivotTransform.position;

		setCameraState (vehicleCameraStates [cameraStateIndex].name);

		//reset the look angle
		lookAngle = Vector2.zero;

		if (!useSmoothTransition) {
			//reset the pivot rotation
			if (currentState.xLimits != Vector2.zero || currentState.xLimits != Vector2.zero || currentState.cameraFixed) {
				currentState.pivotTransform.localRotation = Quaternion.identity;
				currentState.cameraTransform.localRotation = Quaternion.identity;
			}

			//set the new parent of the camera as the first person position
			mainCameraTransform.SetParent (currentState.cameraTransform);

			//reset camera rotation and position
			mainCameraTransform.localPosition = Vector3.zero;
			mainCameraTransform.localRotation = Quaternion.identity;

			currentOriginalDistValue = currentState.originalDist;

			if (currentState.smoothTransition) {
				checkCameraTranslation ();
			} else {
				originalCameraDistance = currentState.originalDist;
			}
		}

		//change the current camera in the gravity controller component
		if (gravityControl != null) {
			gravityControl.getCurrentCameraPivot (currentState.pivotTransform);
		}

		if (cameraShakeLocated) {
			shakingManager.getCurrentCameraTransform (mainCameraTransform);
		}

		//do the same in the weapons system if the vehicle has it
		if (weaponManager != null) {
			weaponManager.getCameraInfo (currentState.cameraTransform, currentState.useRotationInput);
		}

		if (currentState.firstPersonCamera) {
			playerManager.changeHeadScale (true);
		} else {
			playerManager.changeHeadScale (false);
		}

		if (currentState.useNewCameraFov) {
			originalCameraFov = currentState.newCameraFov;

			if (smoothTransitionsInNewCameraFov) {
				playerCameraManager.setMainCameraFov (originalCameraFov, currentState.zoomSpeed);
			} else {
				playerCameraManager.quickChangeFovValue (originalCameraFov);
			}
		}
	}

	public void resetCameraRotation ()
	{
		currentState.pivotTransform.localRotation = Quaternion.identity;
		currentState.cameraTransform.localRotation = Quaternion.identity;
	}

	public void resetAxisCameraRotationToVehicleForwardDirection ()
	{
		float angleY = Vector3.Angle (vehicle.transform.forward, transform.forward);
		angleY *= Mathf.Sign (transform.InverseTransformDirection (Vector3.Cross (vehicle.transform.forward, transform.forward)).y);
		transform.Rotate (0, -angleY, 0);
	}

	public override bool isFirstPersonActive ()
	{
		return isFirstPerson;
	}

	public void setDamageCameraShake ()
	{
		if (isFirstPerson) {
			if (shakeSettings.useDamageShakeInFirstPerson) {
				shakingManager.setExternalShakeState (shakeSettings.firstPersonDamageShake);
			}
		} else {
			if (shakeSettings.useDamageShakeInThirdPerson) {
				shakingManager.setExternalShakeState (shakeSettings.thirdPersonDamageShake);
			}
		}
	}

	public void setCameraExternalShake (externalShakeInfo externalShake)
	{
		if (cameraShakeLocated) {
			shakingManager.setExternalShakeState (externalShake);
		}
	}

	//move away or turn back the camera
	public void moveAwayCamera ()
	{
		
	}

	//stop the current coroutine and start it again
	void checkCameraTranslation ()
	{
		if (moveCamera != null) {
			StopCoroutine (moveCamera);
		}

		moveCamera = StartCoroutine (changeCamerCollisionDistanceCoroutine ());
	}

	IEnumerator changeCamerCollisionDistanceCoroutine ()
	{
		currentState.pivotTransform.position = nextPivotPositon;
		//move the pivot and the camera dist for the camera collision 
		float t = 0;

		//translate position of the pivot
		while (t < 1) {
			t += Time.deltaTime * smoothBetweenState;

			originalCameraDistance = Mathf.Lerp (originalCameraDistance, currentOriginalDistValue, t);
			currentState.pivotTransform.localPosition = Vector3.Lerp (currentState.pivotTransform.localPosition, currentState.originalPivotPosition, t);

			yield return null;
		}
	}

	public void getPlayer (GameObject playerElement)
	{
		player = playerElement;

		playerManager = player.GetComponent<playerController> ();

		playerCameraManager = playerManager.getPlayerCameraManager ();

		mainCamera = playerCameraManager.getMainCamera ();

		mainCameraTransform = mainCamera.transform;

		if (cameraShakeLocated) {
			shakingManager.getCurrentCameraTransform (mainCameraTransform);
		}
	}

	public Transform getVehicleMainCameraTransform ()
	{
		return currentState.cameraTransform;
	}

	//the vehicle is being driving or not
	public void changeCameraDrivingState (bool state, bool resetCameraRotationWhenGetOn)
	{
		drivingVehicle = state;

		//if the vehicle is not being driving, stop all its states
		if (!drivingVehicle) {
			releaseCamera = false;
			pickCamera = false;
			boosting = false;

			if (playerManager != null) {
				playerManager.changeHeadScale (false);
			}

			if (usingZoomOn) {
				setZoom (false);
			}

			if (currentState.useNewCameraFov) {
				if (playerCameraManager != null) {
					playerCameraManager.setMainCameraFov (playerCameraManager.getOriginalCameraFov (), currentState.zoomSpeed);
				}
			}

			cameraPaused = false;
		} 
		//else, reset the vehicle camera rotation
		else {
			if (firstCameraEnabled) {
				if (playerManager != null) {
					playerManager.changeHeadScale (true);
				}
			}

			if (resetCameraRotationWhenGetOn) {
				//reset the camera position in the vehicle, so always that the player gets on, the camera is set just behind the vehicle
				originalCameraDistance = currentState.originalDist;

				//reset the local angle x of the pivot camera
				currentState.pivotTransform.localRotation = Quaternion.identity;
				currentState.cameraTransform.localPosition = -Vector3.forward * originalCameraDistance;
				lookAngle = Vector2.zero;

				//reset the local angle y of the vehicle camera
				resetAxisCameraRotationToVehicleForwardDirection ();
			} else {
				//reset the camera position in the vehicle, so always that the player gets on, the camera is set just behind the vehicle
				resetAxisCameraRotationToVehicleForwardDirection ();

				resetCameraRotation ();

				drivingVehicle = false;

				originalCameraDistance = currentState.originalDist;
			
				lookAngle = Vector2.zero;

				//reset the local angle x of the pivot camera
				Quaternion playerPivotCameraRotation = playerCameraManager.getPivotCameraTransform ().localRotation;
				currentState.pivotTransform.localRotation = playerPivotCameraRotation;

				float newLookAngleValue = playerPivotCameraRotation.eulerAngles.x;

				if (newLookAngleValue > 180) {
					newLookAngleValue -= 360;
				}

				lookAngle.y = newLookAngleValue;

				currentState.cameraTransform.localPosition = -Vector3.forward * originalCameraDistance;

				if (currentState.firstPersonCamera) {
					if (currentState.rotateMainCameraOnFirstPerson) {
						currentCameraUpRotation = lookAngle.x;
					} else {
						currentState.pivotTransform.localEulerAngles = Vector3.zero;

						lookAngle.x = 0;

						currentState.cameraTransform.localEulerAngles = new Vector3 (lookAngle.y, 0, 0);

						currentCameraUpRotation = lookAngle.y;
					}
				}

				float currentVehicleCameraAngle = transform.InverseTransformVector (transform.eulerAngles).y;
				float vehicleCameraAngleTarget = 0;

				if (currentVehicleCameraAngle > 0) {
					vehicleCameraAngleTarget = 360 - currentVehicleCameraAngle;
				} else {
					vehicleCameraAngleTarget = Mathf.Abs (currentVehicleCameraAngle);
				}

				transform.Rotate (0, vehicleCameraAngleTarget, 0);

				//reset the local angle y of the vehicle camera
				float angleY = playerCameraManager.transform.InverseTransformVector (playerCameraManager.transform.eulerAngles).y;
				transform.Rotate (0, angleY, 0);

				drivingVehicle = true;
			}

			//get values for the new fov if it is configured, or the original fov from the camera player
			if (currentState.useNewCameraFov) {
				originalCameraFov = currentState.newCameraFov;
			} else {
				if (playerCameraManager != null) {
					originalCameraFov = playerCameraManager.getOriginalCameraFov ();
				}
			}

			if (playerCameraManager != null) {
				playerCameraManager.setMainCameraFov (originalCameraFov, currentState.zoomSpeed);

				if (playerCameraManager.isUsingZoom ()) {
					playerCameraManager.setUsingZoomOnValue (false);
				}
			}
		}

		if (resetCameraRotationOnGetOff) {
			if (!drivingVehicle) {
				resetCameraRotation ();
			}
		}
	}

	public void resetCameraTransformRotation ()
	{
		transform.rotation = Quaternion.identity;

		resetCameraRotation ();
	}

	//when the player gets on to the vehicle, it is checked if the first person was enabled or not, to set that camera view in the vehicle too
	public void setCameraPosition (bool state)
	{
		//get the current view of the camera, so when it is changed, it is done correctly
		bool adjustRegularCamera = true;

		if (IKDrivingManager.setVehicleCameraStateOnGetOn) {
			firstCameraEnabled = IKDrivingManager.setVehicleCameraStateOnFirstPersonOnGetOn;

			int temporalIndex = vehicleCameraStates.FindIndex (s => s.name.ToLower () == IKDrivingManager.vehicleCameraStateOnGetOn.ToLower ());

			if (temporalIndex > -1) {
				cameraStateIndex = temporalIndex;

				updateCameraPositionOnCameraStateChange (true);

				adjustRegularCamera = false;
			}
		} 

		if (adjustRegularCamera) {

			firstCameraEnabled = state;

			if (firstCameraEnabled) {
				setFirstOrThirdPerson (true);
			} else {
				setFirstOrThirdPerson (false);
			}
		}

		//set the current camera view in the weapons system
		if (weaponManager != null) {
			weaponManager.getCameraInfo (currentState.cameraTransform, currentState.useRotationInput);
		}
	}

	public void setFirstOrThirdPerson (bool state)
	{
		bool assigned = false;

		for (int k = 0; k < vehicleCameraStates.Count; k++) {
			if (!assigned) {
				if (state) {
					if (vehicleCameraStates [k].firstPersonCamera) {
						setCameraState (vehicleCameraStates [k].name);

						cameraStateIndex = k;
						assigned = true;
					}
				} else {
					if (!vehicleCameraStates [k].firstPersonCamera) {
						setCameraState (vehicleCameraStates [k].name);

						cameraStateIndex = k;
						assigned = true;
					}
				}
			}
		}
	}

	public void adjustCameraToCurrentCollisionDistance ()
	{
		if (!currentState.firstPersonCamera) {
			//get the current camera position for the camera collision detection
			currentCameraDistance = checkCameraCollision ();

			//set the local camera position
			currentCameraDistance = Mathf.Clamp (currentCameraDistance, 0, originalCameraDistance);
			currentState.cameraTransform.localPosition = -Vector3.forward * currentCameraDistance;
		}
	}

	//adjust the camera position to avoid cross any collider
	public float checkCameraCollision ()
	{
		//launch a ray from the pivot position to the camera direction
		ray = new Ray (currentState.pivotTransform.position, -currentState.pivotTransform.forward);

		//store the hits received
		hits = Physics.SphereCastAll (ray, clipCastRadius, originalCameraDistance + clipCastRadius, layer);
		float closest = Mathf.Infinity;
		float hitDist = originalCameraDistance;

		//find the closest
		for (int i = 0; i < hits.Length; i++) {
			if (hits [i].distance < closest && !hits [i].collider.isTrigger) {
				//the camera will be moved that hitDist in its forward direction
				closest = hits [i].distance;
				hitDist = -currentState.pivotTransform.InverseTransformPoint (hits [i].point).z;
			}
		}

		//clamp the hidDist value
		if (hitDist < 0) {
			hitDist = 0;
		}

		if (hitDist > originalCameraDistance) {
			hitDist = originalCameraDistance;
		}

		//return the value of the collision in the camera
		return Mathf.SmoothDamp (currentCameraDistance, hitDist, ref cameraSpeed, currentCameraDistance > hitDist ? 0 : backClipSpeed);
	}

	public void startOrStopFollowVehiclePosition (bool state)
	{
		followVehiclePosition = state;
	}

	public void pauseOrPlayVehicleCamera (bool state)
	{
		cameraPaused = state;
	}

	public bool isVehicleCameraPaused ()
	{
		return cameraPaused;
	}

	public Transform getCurrentCameraPivot ()
	{
		return currentState.pivotTransform;
	}

	public Transform getCurrentCameraTransform ()
	{
		return currentState.cameraTransform;
	}

	public Vector2 getVehicleLookAngle ()
	{
		return lookAngle;
	}

	public bool gameIsPaused ()
	{
		return actionManager.input.gameCurrentlyPaused;
	}

	public void setVehicleAndCameraParent (Transform newParent)
	{
		vehicle.transform.SetParent (newParent);

		transform.SetParent (newParent);
	}

	//CALL INPUT FUNCTIONS
	public void inputChangeCamera ()
	{
		if (drivingVehicle && !isGamePaused && !cameraPaused) {
			//check if the change camera input is used
			changeCameraPosition ();
		}
	}

	public void inputZoom ()
	{
		if (drivingVehicle && !isGamePaused && !cameraPaused) {
			setZoom (!usingZoomOn);
		}
	}

	public void inputTakeCapture ()
	{
		if (drivingVehicle && !isGamePaused && !cameraPaused) {
			if (playerCameraManager != null) {
				playerCameraManager.takeCapture ();
			}
		}
	}

	//draw the move away position of the pivot and the camera in the inspector
	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		//&& !Application.isPlaying
		if (showGizmo) {
			for (int i = 0; i < vehicleCameraStates.Count; i++) {
				if (vehicleCameraStates [i].showGizmo) {
					Gizmos.color = Color.white;
					Gizmos.DrawLine (vehicleCameraStates [i].pivotTransform.position, vehicleCameraStates [i].cameraTransform.position);
					Gizmos.DrawLine (vehicleCameraStates [i].pivotTransform.position, transform.position);
					Gizmos.color = vehicleCameraStates [i].gizmoColor;
					Gizmos.DrawSphere (vehicleCameraStates [i].pivotTransform.position, gizmoRadius);
					Gizmos.DrawSphere (vehicleCameraStates [i].cameraTransform.position, gizmoRadius);

					if (showCameraDirectionGizmo) {
						GKC_Utils.drawGizmoArrow (vehicleCameraStates [i].cameraTransform.position, vehicleCameraStates [i].cameraTransform.forward * gizmoArrowLineLength, 
							vehicleCameraStates [i].gizmoColor, gizmoArrowLength, gizmoArrowAngle);
					}
				}
			}
		}
	}

	[System.Serializable]
	public class vehicleCameraStateInfo
	{
		public string name;
		public Transform pivotTransform;
		public Transform cameraTransform;

		public bool useRotationInput;
		public Vector2 xLimits;
		public Vector2 yLimits;

		public bool rotateMainCameraOnFirstPerson;

		public bool useNewCameraFov;
		public float newCameraFov;
		public bool enabled;
		public bool firstPersonCamera;
		public bool cameraFixed;
		public bool smoothTransition;

		public bool useIdentityRotation;

		public bool canUnlockCursor;

		public bool useCameraSteer;

		public bool useDeadZoneForCameraSteer;
		public float deadZoneAngle;

		public bool canUseZoom = true;
		public float zoomSpeed = 120;
		public float zoomFovValue = 17;
		public float verticalRotationSpeedZoomIn = 2.5f;
		public float horizontalRotationSpeedZoomIn = 2.5f;

		public bool showGizmo;
		public Color gizmoColor;
		public float labelGizmoOffset;
		public bool gizmoSettings;

		public float originalDist;
		public Vector3 originalPivotPosition;

		public vehicleCameraStateInfo (vehicleCameraStateInfo newState)
		{
			name = newState.name;
			pivotTransform = newState.pivotTransform;
			cameraTransform = newState.cameraTransform;
			useRotationInput = newState.useRotationInput;
			xLimits = newState.xLimits;
			yLimits = newState.yLimits;

			rotateMainCameraOnFirstPerson = newState.rotateMainCameraOnFirstPerson;

			useNewCameraFov = newState.useNewCameraFov;
			newCameraFov = newState.newCameraFov;
			enabled = newState.enabled;
			firstPersonCamera = newState.firstPersonCamera;
			cameraFixed = newState.cameraFixed;
			smoothTransition = newState.smoothTransition;
			useIdentityRotation = newState.useIdentityRotation;
			originalDist = newState.originalDist;
			originalPivotPosition = newState.originalPivotPosition;
			canUnlockCursor = newState.canUnlockCursor;

			canUseZoom = newState.canUseZoom;
			zoomSpeed = newState.zoomSpeed;
			zoomFovValue = newState.zoomFovValue;

			verticalRotationSpeedZoomIn = newState.verticalRotationSpeedZoomIn;
			horizontalRotationSpeedZoomIn = newState.horizontalRotationSpeedZoomIn; 

			useCameraSteer = newState.useCameraSteer;

			useDeadZoneForCameraSteer = newState.useDeadZoneForCameraSteer;
			deadZoneAngle = newState.deadZoneAngle; 
		}
	}

	[System.Serializable]
	public class shakeSettingsInfo
	{
		public bool useDamageShake;
		public bool sameValueBothViews;
		public bool useDamageShakeInThirdPerson;
		public externalShakeInfo thirdPersonDamageShake;
		public bool useDamageShakeInFirstPerson;
		public externalShakeInfo firstPersonDamageShake;
	}
}