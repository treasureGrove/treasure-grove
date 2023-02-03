using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class overrideElementControlSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask layer;
	public float raycastDistance;
	public float raycastRadius = 0.3f;

	public bool useTagsToCheck;
	public List<string> allowedTagsToControl = new List<string> ();

	public bool hidePlayerWhileControlling;
	public bool reapearCloseToControllerObject;

	public bool addForceOnReapear;
	public float forceOnReapear;

	public bool disableMapOnOverride;
	public bool useSmoothTransition;
	public float launchCameraSpeed;
	public float backCameraSpeed;
	public float cameraRotationSpeed = 2;

	public float reapearCloseCameraSpeed;

	public string stopOverrideTouchButtonName = "Activate Devices";

	[Space]
	[Header ("Start Game Controlling Character Settings")]
	[Space]

	public bool startControllingCharacter;
	public bool useSmoothTransitionAtStart;
	public GameObject characterToControlAtStart;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool usingOverride;
	public bool launchingCamera;

	public bool controllingVehicle;
	public bool isDefaultController;
	public bool controllerFoundOnMesh;

	public bool playerComponentsPaused;

	public List<GameObject> temporalObjectList = new List<GameObject> ();

	public bool inputStopOverrideControlPaused;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform raycastPosition;

	public GameObject defaultOverrideController;
	public Transform playerCameraTransform;
	public Transform mainCamera;
	public menuPause pauseManager;
	public playerController playerControllerManager;
	public usingDevicesSystem devicesManager;
	public playerCamera playerCameraManager;
	public headBob headBoManager;
	public playerStatesManager statesManager;
	public mapSystem mapSystemManager;
	public remoteEventSystem mainRemoteEventSystem;

	public inputManager input;

	public playerHUDManager mainPlayerHUDManager;

	public playerWeaponsManager weaponsManager;

	public footStepManager stepManager;

	public playerAbilitiesSystem mainPlayerAbilitiesSystem;

	public Transform mainPlayerTransform;


	Coroutine launchCameraCoroutine;

	RaycastHit hit;

	GameObject currentCharacter;

	bool startFunctionCalled;

	Transform targetTransformPosition;

	GameObject currentOverrideControllerGameObject;
	overrideCameraController currentOverrideCameraController;
	overrideInputManager currentOverrideInputManager;
	overrideController currentOverrideController;
	vehicleHUDManager currentVehicleHUDManager;

	Vector3 currentOverrideControllerVelocity;

	bool objectToControlIsRigidbody;
	GameObject controllerMesh;

	bool followingTargetPosition;

	bool currentOverrideActivatedByAbility;
	string abilityNameUsedToActivateOverride;

	objectToOverrideSystem currentObjectToOverrideSystem;

	void Start ()
	{
		if (mainPlayerTransform == null) {
			mainPlayerTransform = playerControllerManager.transform;
		}
	}

	void Update ()
	{
		//if the player starts the game controlling a character, the function is called
		if (!startFunctionCalled) {
			if (startControllingCharacter && characterToControlAtStart) {
				overrideElementControl (characterToControlAtStart);
			}

			startFunctionCalled = true;
		}

		if (followingTargetPosition && !controllingVehicle) {
			mainPlayerTransform.position = currentCharacter.transform.position;
			playerCameraTransform.position = mainPlayerTransform.position;
		}

		if (controllingVehicle) {
			if (currentVehicleHUDManager.vehicleIsDestroyed ()) {
				stopCurrentOverrideControl ();
			}
		}
	}

	public void stopCurrentOverrideControl ()
	{
		if (!usingOverride) {
			return;
		}

		if (currentOverrideActivatedByAbility) {
			currentOverrideActivatedByAbility = false;

			if (abilityNameUsedToActivateOverride != "") {
				if (mainPlayerAbilitiesSystem != null) {
					mainPlayerAbilitiesSystem.inputSelectAndPressDownNewAbilityTemporally (abilityNameUsedToActivateOverride, false);

					mainPlayerAbilitiesSystem.inputPressUpUseCurrentAbility ();
				}
			}
				
			abilityNameUsedToActivateOverride = "";

			return;
		}

		if (controllingVehicle) {

			//if the player was driving and the player will repear close to it, stop the coroutine movement of the camera, set the new position and resume the coroutine
			if (reapearCloseToControllerObject) {
//				Coroutine playerCameraMovementCoroutine = playerCameraManager.getVehicleCameraMovementCoroutine ();
//
//				if (playerCameraMovementCoroutine != null) {
//					StopCoroutine (playerCameraMovementCoroutine);
//				}

				currentVehicleHUDManager.getIKDrivingSystem ().stopCameraTranslationByPlayerCamera (playerCameraManager);

				Vector3 mainCameraPosition = mainCamera.position;

				reapearCharacter ();

				Transform currentMainCameraParent = mainCamera.parent;

				mainCamera.SetParent (null);

				playerCameraTransform.rotation = currentVehicleHUDManager.getVehicleCameraController ().transform.rotation;

				mainCamera.SetParent (currentMainCameraParent);

				mainCamera.position = mainCameraPosition;

				currentVehicleHUDManager.getIKDrivingSystem ().checkCameraTranslationByPlayerCamera (false, playerCameraManager);
			}

			checkCharacterMesh (false);

			resetValuesOnStop ();

			playerComponentsPaused = false;

			checkEventsOnStateChange (usingOverride);

		} else {
			
			stopOverride ();

			if (currentOverrideInputManager.destroyObjectOnStopOverride) {
				currentOverrideInputManager.eventToDestroy.Invoke ();
			}
		}

		followingTargetPosition = false;
	}

	public void checkElementToControl ()
	{
		checkElementToControl (raycastPosition);
	}

	public void checkElementToControl (Transform newRaycastPosition)
	{
		//check with a raycast if there is a character which can be controlled
		if (Physics.Raycast (newRaycastPosition.position, newRaycastPosition.forward, out hit, raycastDistance, layer)) {
			List<Collider> colliders = new List<Collider> ();

			colliders.AddRange (Physics.OverlapSphere (hit.point, raycastRadius, layer));

			bool characterToControlFound = false;

			foreach (Collider currentCollider in colliders) {
				characterToControlFound = overrideElementControl (currentCollider.gameObject);

				if (characterToControlFound) {
					return;
				}
			}
		}
	}

	public bool overrideElementControl (GameObject objectToControl)
	{
		bool characterToControlFound = false;

		//check the possible character to control
		currentCharacter = applyDamage.getCharacterOrVehicle (objectToControl);

		Rigidbody objectToControlRigidbody = objectToControl.GetComponent<Rigidbody> ();

		if (currentCharacter == null) {
			if (objectToControlRigidbody != null) {
				objectToControlIsRigidbody = true;
				currentCharacter = objectToControl;
			} else if (objectToControl.GetComponent<overrideInputManager> ()) {
				objectToControlIsRigidbody = false;
				currentCharacter = objectToControl;
			}
		} else {
			if (objectToControlRigidbody != null && !applyDamage.isVehicle (currentCharacter)) {
				objectToControlIsRigidbody = true;
				currentCharacter = objectToControl;
			}
		}

		resetValues ();

		//if there are a character which can be controlled, then assign the fields and variables to start the control
		if (currentCharacter != null) {

			bool canControlObject = false;

			if (allowedTagsToControl.Contains (currentCharacter.tag)) {
				canControlObject = true;
			}

			currentObjectToOverrideSystem = currentCharacter.GetComponent<objectToOverrideSystem> ();

			if (!useTagsToCheck || !canControlObject) {
				if (currentObjectToOverrideSystem != null) {
					if (currentObjectToOverrideSystem.canBeOverridenActive ()) {
						currentCharacter = currentObjectToOverrideSystem.getObjectToOverride ();
					} else {
						return false;
					}
				} else {
					return false;
				}
			}

			//get the character gameobjec itself and where the main camera will be translated first
			Transform placeToTransport = applyDamage.getPlaceToShoot (currentCharacter);

			if (placeToTransport != null) {
				//the character is a vehicle, an npc, a turret or other element with its own controller
				targetTransformPosition = placeToTransport;

				isDefaultController = false;
			} else {
				//else, the character to control is a rigidbody or an element with a similar controller
				targetTransformPosition = currentCharacter.transform;

				if (objectToControlIsRigidbody) {

					controllerMesh = currentCharacter;

					//check if the rigidbody has already an override controller which has been configured on it to set a configuration different than on usual rigidbodies
					//mostly due to size and weight
					overrideController overrideControllerOnMesh = controllerMesh.GetComponentInChildren<overrideController> ();

					if (overrideControllerOnMesh != null) {
						//the rigidbody has a preconfigured override controller
						controllerFoundOnMesh = true;

						currentCharacter = overrideControllerOnMesh.gameObject;

						overrideControllerOnMesh.transform.SetParent (null);
					} else {
						//else, create and assign a prefab for an override controller configured in this component
						if (currentOverrideControllerGameObject == null) {
							currentOverrideControllerGameObject = (GameObject)Instantiate (defaultOverrideController, currentCharacter.transform.position, currentCharacter.transform.rotation);
						} else {
							currentOverrideControllerGameObject.SetActive (true);
							currentOverrideControllerGameObject.transform.rotation = Quaternion.identity;
							currentOverrideControllerGameObject.transform.position = currentCharacter.transform.position;
						}

						controllerFoundOnMesh = false;

						currentCharacter = currentOverrideControllerGameObject;
					}

					currentOverrideController = currentCharacter.GetComponent<overrideController> ();

					//reset the controller rotation
					if (controllerFoundOnMesh) {
						currentCharacter.transform.rotation = controllerMesh.transform.rotation;
					} else {
						currentOverrideControllerGameObject.transform.rotation = controllerMesh.transform.rotation;
					}

					//assign the rigidbody mesh to the controller
					currentOverrideController.setControllerMesh (controllerMesh);

					//get the velocity of the rigidbody before the control starts
					setControllerRigidbodyVelocity (true);

					//remove the rigidbody component of the original object
					Destroy (controllerMesh.GetComponent<Rigidbody> ());

					//assign the previous velocity of the rigidbody to the override controller rigidbody
					Rigidbody controllerRigidbody = currentOverrideController.GetComponent<Rigidbody> ();

					if (controllerRigidbody != null) {
						controllerRigidbody.isKinematic = false;
						controllerRigidbody.velocity = currentOverrideControllerVelocity;
					}

					isDefaultController = true;
				}
			}

			bool objectToControlFound = false;

			//get the rest of elements in the obejct to change its states
			currentOverrideInputManager = currentCharacter.GetComponent<overrideInputManager> ();

			if (currentOverrideInputManager == null) {
				playerComponentsManager currentPlayerComponentsManager = currentCharacter.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					currentOverrideInputManager = currentPlayerComponentsManager.getOverrideInputManager ();
				}
			}

			if (currentOverrideInputManager != null) {
				currentOverrideCameraController = currentOverrideInputManager.getOverrideCameraControllerManager ();

				objectToControlFound = true;
			}

			currentVehicleHUDManager = currentCharacter.GetComponent<vehicleHUDManager> ();

			//check if the character to control is a vehicle
			if (currentVehicleHUDManager != null) {
				controllingVehicle = true;

				objectToControlFound = true;
			}

			if (!objectToControlFound) {
				//print ("Object found can't be controlled");
				return false;
			}

			//pause the actions on the player
			pauseOrPlayPlayerComponents (true);

			//launch the camera to the target to control
			launchCameraToTarget (true);

			//check if the player was in first person to enable his body mesh, so the player is visible on the scene in case he is not hidden when he control other character
			checkCharacterMesh (true);

			characterToControlFound = true;

			checkRemoteEventsOnStateChange (true);
		}

		return characterToControlFound;
	}

	public void resetValues ()
	{
		isDefaultController = false;

		controllerFoundOnMesh = false;
	}

	public void launchCameraToTarget (bool launchingToCharacter)
	{
		//stop and restart the coroutine to start or finish the control
		if (launchCameraCoroutine != null) {
			StopCoroutine (launchCameraCoroutine);
		}

		launchCameraCoroutine = StartCoroutine (launchCameraToTargetCoroutine (launchingToCharacter));
	}

	IEnumerator launchCameraToTargetCoroutine (bool launchingToCharacter)
	{
		//if not, the movement and placement of the main camera is made here
		if (!controllingVehicle) {
			//change the parent of the camera according to the control state
			//also, start to activate the override camera and set its rotation
			if (launchingToCharacter) {
				currentOverrideCameraController.setParentState (true);

				if (currentOverrideCameraController.isCameraControllerEnabled ()) {
					currentOverrideCameraController.resetRotation (playerCameraTransform.rotation);
				}

				mainCamera.SetParent (currentOverrideCameraController.getCameraTransform ());

				playerCameraManager.getPivotCameraTransform ().localRotation = Quaternion.identity;

				playerCameraManager.setLookAngleValue (new Vector2 (0, 0));

				playerCameraManager.resetMainCameraTransformLocalPosition ();

				playerCameraManager.resetPivotCameraTransformLocalPosition ();

				playerCameraManager.resetCurrentCameraStateAtOnce ();

			} else {
				playerCameraTransform.rotation = currentOverrideCameraController.transform.rotation;

				mainCamera.SetParent (playerCameraManager.getCameraTransform ());

				currentOverrideCameraController.setCameraActiveState (false);

				currentOverrideCameraController.setParentState (false);
			}
				
			//call the functions that are called before the control starts or ends
			if (currentOverrideInputManager != null) {
				currentOverrideInputManager.setPreOverrideControlState (launchingToCharacter);
			}
				
			//if the camera movement towards the target or back to the player is smooth, move the camera
			if ((!startFunctionCalled && useSmoothTransitionAtStart) || (startFunctionCalled && useSmoothTransition)) {

				launchingCamera = true;

				if (!launchingToCharacter) {
					targetTransformPosition = playerCameraManager.getCameraTransform ();
				}
					
				bool reapearingClose = false;

				if (hidePlayerWhileControlling && reapearCloseToControllerObject && !launchingToCharacter) {
					reapearingClose = true;
				}

				if (reapearingClose) {
					checkCharacterMesh (false);

					float dist = GKC_Utils.distance (mainCamera.position, playerCameraManager.getCameraTransform ().position);
					float duration = dist / reapearCloseCameraSpeed;
					float t = 0;

					Quaternion targetRotation = Quaternion.identity;
					Vector3 targetPosition = Vector3.zero;

					bool targetReached = false;

					float angleDifference = 0;

					float movementTimer = 0;

					float positionDifference = 0;

					while (!targetReached) {
						t += Time.deltaTime / duration;

						mainCamera.localPosition = Vector3.Lerp (mainCamera.localPosition, targetPosition, t);
						mainCamera.localRotation = Quaternion.Lerp (mainCamera.localRotation, Quaternion.identity, t);

						angleDifference = Quaternion.Angle (mainCamera.localRotation, Quaternion.identity);

						movementTimer += Time.deltaTime;

						positionDifference = GKC_Utils.distance (mainCamera.localPosition, targetPosition);

						if ((positionDifference < 0.01f && angleDifference < 0.2f) || movementTimer > (duration + 1)) {
							targetReached = true;
						}

						yield return null;
					}
				} else {
					float dist = GKC_Utils.distance (mainCamera.position, targetTransformPosition.position);
					float duration = dist / launchCameraSpeed;
					float t = 0;

					bool targetReached = false;

					Quaternion targetRotation = Quaternion.identity;
					Vector3 targetDirection = Vector3.zero;

					while (!targetReached && t < 1 && mainCamera.position != targetTransformPosition.position) {
						t += Time.deltaTime / duration;

						mainCamera.position = Vector3.Lerp (mainCamera.position, targetTransformPosition.position, t);
						//change the distance value for a field in the override controller, to reach a proper position close to the object, avoiding to be too much close

						if (launchingToCharacter) {
							targetDirection = targetTransformPosition.position - mainCamera.position;

							if (targetDirection != Vector3.zero) {
								targetRotation = Quaternion.LookRotation (targetDirection);
								mainCamera.rotation = Quaternion.Lerp (mainCamera.rotation, targetRotation, Time.deltaTime * cameraRotationSpeed);
							}
						}

						if (GKC_Utils.distance (mainCamera.position, targetTransformPosition.position) < currentOverrideCameraController.getControllerRadius ()) {
							targetReached = true;
						}

						yield return null;
					}

					//if the camera is going toward the player, when it is close enough, check if he was on first person before the control was started, so 
					//his mesh is disabled before the camera is in the first person position
					if (!launchingToCharacter) {
						checkCharacterMesh (false);
					}

					Vector3 targetPosition = Vector3.zero;
			
					targetTransformPosition = currentOverrideCameraController.getCameraTransform ();

					dist = GKC_Utils.distance (mainCamera.position, targetTransformPosition.position);

					duration = dist / backCameraSpeed;

					t = 0;

					while (t < 1 && mainCamera.localPosition != targetPosition) {
						t += Time.deltaTime / duration;

						mainCamera.localPosition = Vector3.Lerp (mainCamera.localPosition, targetPosition, t);
						mainCamera.localRotation = Quaternion.Lerp (mainCamera.localRotation, Quaternion.identity, t);

						yield return null;
					}
					
					t = 0;

					while (t < 1 && mainCamera.localRotation != Quaternion.identity) {
						t += Time.deltaTime * backCameraSpeed;

						mainCamera.localRotation = Quaternion.Lerp (mainCamera.localRotation, Quaternion.identity, t);

						yield return null;
					}
				}

				launchingCamera = false;

				if (launchingToCharacter && hidePlayerWhileControlling && reapearCloseToControllerObject) {
					followingTargetPosition = true;
				}
			} else {
				mainCamera.localPosition = Vector3.zero;
				mainCamera.localRotation = Quaternion.identity;

				//else, check the player mesh to enable or disable it, to make it visible or invisible in the scene
				if (!launchingToCharacter) {
					checkCharacterMesh (false);
				}
			}

			//enable the override camera controller
			if (launchingToCharacter) {
				currentOverrideCameraController.setCameraActiveState (true);
			}
		}

		//if the camera has been launched to the character to control, enable the override functions
		if (launchingToCharacter) {
			startOverride ();

			launchingCamera = false;

			if (isDefaultController) {
				//if the character to control is a rigidbody with a preconfigured controller, set its rigidbody to kinematic
				if (controllerFoundOnMesh) {
					currentCharacter.GetComponent<Rigidbody> ().isKinematic = false;
				}
			}

			enableOrDisableStopOverridButton (true);
		} else {
			//the control is over, so resume the actions ont he player
			pauseOrPlayPlayerComponents (false);

			//get the proper velocity to the controlled character in case this was a rigidbody
			setControllerRigidbodyVelocity (false);

			//if the controlled character was a rigidbody, then
			if (isDefaultController) {
				//apply the velocity obtained before
				currentOverrideController.getControllerMesh ().AddComponent<Rigidbody> ().velocity = currentOverrideControllerVelocity;
			}

			//disable the override controller
			currentOverrideInputManager.overrideControlState (false, null);

			//if the controlled character was a rigidbody, then
			if (isDefaultController) {
				//remove the mesh from the override controller
				currentOverrideController.removeControllerMesh ();

				if (controllerFoundOnMesh) {
					//if the character controlled has a preconfigured controller, set the controller as a child of the rigidbody and resume its own rigidbody
					currentCharacter.transform.SetParent (currentOverrideController.getControllerMesh ().transform);
					currentCharacter.GetComponent<Rigidbody> ().isKinematic = true;
				} else {
					//else, disable the prefab override controller used for regular rigidbodies without override controller preconfigured
					currentOverrideControllerGameObject.SetActive (false);
				}
			}

			currentOverrideCameraController.resetLocalRotationPosition ();
		}
	}

	//get the current velocity of a controlled rigidbody before and after the control starts
	public void setControllerRigidbodyVelocity (bool state)
	{
		if (state) {
			currentOverrideControllerVelocity = controllerMesh.GetComponent<Rigidbody> ().velocity;
		} else {
			if (controllerFoundOnMesh) {
				currentOverrideControllerVelocity = currentCharacter.GetComponent<Rigidbody> ().velocity;
			} else {
				if (currentOverrideControllerGameObject != null) {
					currentOverrideControllerVelocity = currentOverrideControllerGameObject.GetComponent<Rigidbody> ().velocity;
				}
			}
		}
	}

	public void startOverride ()
	{
		usingOverride = true;

		if (showDebugPrint) {
			print ("Override Active");
		}

		checkEventsOnStateChange (usingOverride);

		//if the character to control is a vehicle, then call its funtion to get on
		if (currentVehicleHUDManager != null) {
			currentVehicleHUDManager.getIKDrivingSystem ().setCanBeDrivenRemotelyState (true);

			currentVehicleHUDManager.getIKDrivingSystem ().setDriverExternally (playerControllerManager.gameObject);

			return;
		} 
			
		//else, call the functions configured in the override input manager to enable its control
		if (currentOverrideInputManager != null) {
			devicesManager.clearDeviceList ();

			devicesManager.disableIcon ();

			currentOverrideInputManager.overrideControlState (true, playerControllerManager.gameObject);

			return;
		}
	}

	public void pauseOrPlayPlayerComponents (bool state)
	{
		//pause or resume all the actions in the player when the control starts or ends
		playerComponentsPaused = state;

		playerControllerManager.changeScriptState (!state);

		playerControllerManager.setHeadTrackCanBeUsedState (!state);

		playerControllerManager.setUsingDeviceState (state);

		pauseManager.usingDeviceState (state);

		pauseManager.enableOrDisablePlayerHUD (!state);

		headBoManager.stopAllHeadbobMovements ();

		headBoManager.playOrPauseHeadBob (!state);

		if (controllingVehicle) {
			playerControllerManager.setDrivingRemotelyState (state);
		} else {
			playerControllerManager.setOverridingElementState (state);
		}
			
		statesManager.checkPlayerStates ();

		playerCameraManager.pauseOrPlayCamera (!state);

		if (disableMapOnOverride && !controllingVehicle) {
			mapSystemManager.enableOrDisableMiniMap (!state);
		}

		health healthManager = currentCharacter.GetComponent<health> ();

		if (healthManager != null) {
			healthManager.setSliderVisibleState (!state);
		}

		weaponsManager.updateCanMoveValue ();
	}

	public void checkCharacterMesh (bool state)
	{
		//hide or show the player's mesh according to if he is hidden while the control is enabled or if his mesh is enable to make it visible in the level
		bool firstCameraEnabled = playerCameraManager.isFirstPersonActive ();

		if (firstCameraEnabled) {
			playerControllerManager.setCharacterMeshGameObjectState (state);

			playerControllerManager.setAnimatorState (state);
		}

		if (hidePlayerWhileControlling) {
			if (firstCameraEnabled) {
				playerControllerManager.setCharacterMeshGameObjectState (false);
			} else {
				playerControllerManager.setCharacterMeshGameObjectState (!state);

				playerControllerManager.setCharacterMeshesListToDisableOnEventState (!state);

				weaponsManager.enableOrDisableWeaponsMesh (!state);

				stepManager.enableOrDisableFootStepsComponents (!state);
			}

			playerControllerManager.setMainColliderState (!state);
		}
	}

	public void stopOverride ()
	{
		//stop the control
		if (!controllingVehicle) {

			reapearCharacter ();

			//call the function to return the camera, stop the actions on the override controller and resume the player's actions
			launchCameraToTarget (false);
		}

		resetValuesOnStop ();

		checkEventsOnStateChange (usingOverride);

		enableOrDisableStopOverridButton (false);
	}

	void resetValuesOnStop ()
	{
		usingOverride = false;

		if (showDebugPrint) {
			print ("Override Deactivate");
		}

		controllingVehicle = false;

		if (currentObjectToOverrideSystem != null) {
			checkRemoteEventsOnStateChange (false);
		}

		currentObjectToOverrideSystem = null;
	}

	void checkRemoteEventsOnStateChange (bool state)
	{
		if (currentObjectToOverrideSystem != null && currentCharacter != null) {

			remoteEventSystem currentCharacterRemoteEventSystem = currentCharacter.GetComponent<remoteEventSystem> ();

			if (state) {
				if (currentObjectToOverrideSystem.useRemoteEventsOnObjectOnStart) {
					if (currentCharacterRemoteEventSystem != null) {
						for (int i = 0; i < currentObjectToOverrideSystem.remoteEventNameListOnObjectOnStart.Count; i++) {
							currentCharacterRemoteEventSystem.callRemoteEvent (currentObjectToOverrideSystem.remoteEventNameListOnObjectOnStart [i]);
						}
					}
				}

				if (currentObjectToOverrideSystem.useRemoteEventsOnPlayerOnStart) {
					for (int i = 0; i < currentObjectToOverrideSystem.remoteEventNameListOnPlayerOnStart.Count; i++) {
						mainRemoteEventSystem.callRemoteEvent (currentObjectToOverrideSystem.remoteEventNameListOnPlayerOnStart [i]);
					}
				}
			} else {
				if (currentObjectToOverrideSystem.useRemoteEventsOnObjectOnEnd) {
					if (currentCharacterRemoteEventSystem != null) {
						for (int i = 0; i < currentObjectToOverrideSystem.remoteEventNameListOnObjectOnEnd.Count; i++) {
							currentCharacterRemoteEventSystem.callRemoteEvent (currentObjectToOverrideSystem.remoteEventNameListOnObjectOnEnd [i]);
						}
					}
				}

				if (currentObjectToOverrideSystem.useRemoteEventsOnPlayerOnEnd) {
					for (int i = 0; i < currentObjectToOverrideSystem.remoteEventNameListOnPlayerOnEnd.Count; i++) {
						mainRemoteEventSystem.callRemoteEvent (currentObjectToOverrideSystem.remoteEventNameListOnPlayerOnEnd [i]);
					}
				}
			}
		}
	}

	public void reapearCharacter ()
	{
		//if the player reapears close to the controlled character, set his position

		if (reapearCloseToControllerObject) {
			Vector3 positionToReapear = Vector3.zero;
			Quaternion rotationToReapear = Quaternion.identity;

			if (controllingVehicle) {

				positionToReapear = currentVehicleHUDManager.transform.position + currentVehicleHUDManager.transform.up * currentVehicleHUDManager.getVehicleRadius ();
				rotationToReapear = currentVehicleHUDManager.getVehicleCameraController ().transform.rotation;

			} else {
				if (currentOverrideCameraController.reapearInCertainPositionActive ()) {
					positionToReapear = currentOverrideCameraController.getPositionToReapear ();
				} else {
					float heightOffset = currentOverrideCameraController.getControllerRadius ();
	
					positionToReapear = currentCharacter.transform.position + currentOverrideCameraController.transform.up * heightOffset;
				}
				rotationToReapear = currentOverrideCameraController.transform.rotation;
			}

			mainPlayerTransform.rotation = rotationToReapear;
			mainPlayerTransform.position = positionToReapear;

			playerCameraTransform.position = mainPlayerTransform.position;

			if (addForceOnReapear) {
				bool isPlayerOnGrond = playerControllerManager.checkIfPlayerOnGroundWithRaycast ();

				playerControllerManager.setCheckOnGroungPausedState (false);

				if (!isPlayerOnGrond) {
					playerControllerManager.setPlayerOnGroundState (false);
				}

				playerControllerManager.setPlayerOnGroundAnimatorStateOnOverrideOnGroundWithTime (true);

				playerControllerManager.disableOverrideOnGroundAnimatorValue ();

				playerControllerManager.setPauseResetAnimatorStateFOrGroundAnimatorState (true);

				if (isPlayerOnGrond) {
					playerControllerManager.setPlayerOnGroundState (true);

					playerControllerManager.setOnGroundAnimatorIDValue (true);
				} else {
					playerControllerManager.setOnGroundAnimatorIDValue (false);
				}

				playerControllerManager.useJumpPlatform ((mainPlayerTransform.up + mainPlayerTransform.forward) * forceOnReapear, ForceMode.Force);
			}
		}
	}

	public void setDeadState ()
	{

	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				evenOnStateEnabled.Invoke ();
			} else {
				eventOnStateDisabled.Invoke ();
			}
		}
	}

	public void enableOrDisableStopOverridButton (bool state)
	{
		if (pauseManager.isUsingTouchControls ()) {
			if (input == null) {
				input = FindObjectOfType<inputManager> ();
			}

			if (input != null) {
				if (state) {
					input.enableTouchButtonByName (stopOverrideTouchButtonName);
				} else {
					input.disableTouchButtonByName (stopOverrideTouchButtonName);
				}
			}
		}
	}

	public void setCurrentOverrideActivatedByAbilityState (bool state)
	{
		currentOverrideActivatedByAbility = state;
	}

	public void setCurrentAbilityNameUsedToActivateOverride (string abilityName)
	{
		abilityNameUsedToActivateOverride = abilityName;
	}

	public void addNewTemporalObject (GameObject newObject)
	{
		if (!temporalObjectList.Contains (newObject)) {
			temporalObjectList.Add (newObject);
		}
	}

	public void removeNewTemporalObject (GameObject newObject)
	{
		if (temporalObjectList.Contains (newObject)) {
			temporalObjectList.Remove (newObject);
		}
	}

	public bool checkIfTemporalObjectOnList (GameObject newObject)
	{
		return temporalObjectList.Contains (newObject);
	}

	public void setInputStopOverrideControlPausedState (bool state)
	{
		inputStopOverrideControlPaused = state;
	}

	//INPUT FUNCTIONS
	public void inputStopOverrideControl ()
	{
		//if he is controlling a character, this input stops that control if the character is not a vehicle, since the input to stop that control is in the using devices sytem
		//being the same input used to get off from vehicles
		if (inputStopOverrideControlPaused) {
			return;
		}

		if (usingOverride) {
			stopCurrentOverrideControl ();
		}
	}
}
