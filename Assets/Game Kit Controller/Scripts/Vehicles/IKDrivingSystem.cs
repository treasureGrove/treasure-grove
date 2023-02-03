using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class IKDrivingSystem : mainRiderSystem
{
	public GameObject vehicle;
	public GameObject vehicelCameraGameObject;

	public bool useCustomVehicleGameObject;
	public GameObject customVehicleGameObject;

	public IKDrivingInformation IKDrivingInfo;

	public List<IKDrivingInformation> IKVehiclePassengersList = new List<IKDrivingInformation> ();

	public bool hidePlayerFromNPCs;
	public bool playerVisibleInVehicle = true;
	public bool ejectPlayerWhenDestroyed;
	public float ejectingPlayerForce;
	public bool useExplosionForceWhenDestroyed;
	public float explosionRadius;
	public float explosionForce;
	public float explosionDamage;
	public bool ignoreShield;

	public int damageTypeID = -1;

	public bool useRemoteEventOnObjectsFound;
	public string removeEventName;

	public bool showGizmo;
	public Color gizmoLabelColor;
	public float gizmoRadius = 0.1f;
	public bool useHandleForVertex;
	public float handleRadius;
	public Color handleGizmoColor;
	public bool usePositionHandle = true;

	public bool controlsMenuOpened;
	public bool hidePlayerWeaponsWhileDriving;
	public bool showSettings;
	public bool resetCameraRotationWhenGetOn = true;
	public bool resetCameraRotationWhenGetOff;

	public bool startGameInThisVehicle;
	public GameObject playerForVehicle;

	public bool playerIsAlwaysDriver = true;

	public bool pushCharactersOnExplosion = true;

	public bool applyExplosionForceToVehicles = true;
	public float explosionForceToVehiclesMultiplier = 0.2f;
	public bool killObjectsInRadius;
	public ForceMode forceMode;

	public bool useLayerMask;
	public LayerMask layer;

	public bool canBeDrivenRemotely;
	bool originalCanBeDrivenRemotelyValue;

	public bool isBeingDrivenRemotely;

	public bool activateFreeFloatingModeOnEject;
	public float activateFreeFloatingModeOnEjectDelay = 0.5f;

	bool activateFreeFloatingModeOnEjectEnabled;

	bool checkIfPlayerStartInVehicle;

	public bool drawFireWeaponIfCarryingPreviously = true;
	public bool drawMeleeWeaponIfCarryingPreviously = true;

	public bool addCollisionForceDirectionToPassengers;
	public Vector3 extraCollisionForceAmount = Vector3.one;

	public bool addAngularDirectionToPassengers;
	public float vehicleStabilitySpeed = 1;
	public Vector3 extraAngularDirectioAmount;

	public bool useMinCollisionForce;
	public float minCollisionForce;

	public Vector3 debugCollisionForce;

	public bool activateActionScreen;
	public string actionScreenName;

	public bool useEventOnDriverGetOn;
	public eventParameters.eventToCallWithGameObject eventOnDriverGetOn;

	public bool useEventOnDriverGetOff;
	public eventParameters.eventToCallWithGameObject eventOnDriverGetOff;

	public bool sendPlayersEnterExitTriggerToEvent;
	public eventParameters.eventToCallWithGameObject eventToSendPlayersEnterTriggerToEvent;
	public eventParameters.eventToCallWithGameObject eventToSendPlayersExitTriggerToEvent;


	public bool setPlayerCameraStateOnGetOff;
	public bool setPlayerCameraStateOnFirstPersonOnGetOff;
	public string playerCameraStateOnGetOff;

	public bool setVehicleCameraStateOnGetOn;
	public bool setVehicleCameraStateOnFirstPersonOnGetOn;
	public string vehicleCameraStateOnGetOn;

	public int resetAnimatorDrivingStateID = 666777;


	public inputActionManager actionManager;
	public vehicleCameraController vehicleCameraManager;
	public vehicleHUDManager HUDManager;
	public vehicleWeaponSystem currentVehicleWeaponSystem;
	public vehicleGravityControl vehicleGravityManager;
	public Collider mainCollider;

	public bool useRemoteEventsOnPassengers;
	public List<string> remoteEventNameListGetOn = new List<string> ();
	public List<string> remoteEventNameListGetOff = new List<string> ();


	public bool showDebugPrint;

	public List<GameObject> passengerGameObjectList = new List<GameObject> ();

	List<passengerComponents> passengerComponentsList = new List<passengerComponents> ();
	passengerComponents currentPassengerDriverComponents;

	bool isBeingDriven;
	bool passengersOnVehicle;

	Quaternion mainCameraTargetRotation = Quaternion.identity;
	Vector3 mainCameraTargetPosition = Vector3.zero;

	bool vehicleDestroyed;

	bool getDriverPosition;

	bool cursorUnlocked;

	List<usingDevicesSystem> usingDevicesManagerDetectedList = new List<usingDevicesSystem> ();

	Vector3 predictedUp;
	Vector3 newAngularDirection;

	List<GameObject> passengersInsideTrigger = new List<GameObject> ();


	void Start ()
	{
		//send the input manager component to the vehicle and its camera
		actionManager.setInputManager ();

		originalCanBeDrivenRemotelyValue = canBeDrivenRemotely;

		StartCoroutine (checkStartGameInVehicleCoroutine ());
	}

	IEnumerator checkStartGameInVehicleCoroutine ()
	{
		yield return new WaitForSeconds (0.01f);

		checkStartGameInVehicle ();
	}

	void checkStartGameInVehicle ()
	{
		if (!checkIfPlayerStartInVehicle) {
			if (startGameInThisVehicle) {
				if (playerForVehicle != null) {
					setDriverExternally (playerForVehicle);
				} else {
					print ("Warning: assign the player to drive this car in the field Player For Vehicle in IK Driving System inspector");
				}
			}

			checkIfPlayerStartInVehicle = true;
		}
	}

	public void applySeatsForces ()
	{
		if (addAngularDirectionToPassengers && HUDManager.isVehicleBeingDriven ()) {
			for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
				if (!IKVehiclePassengersList [i].vehicleSeatInfo.seatIsFree) {
					if (!IKVehiclePassengersList [i].vehicleSeatInfo.currentlyOnFirstPerson) {
						newAngularDirection = Vector3.Lerp (newAngularDirection, Vector3.zero, Time.deltaTime * vehicleStabilitySpeed);

						IKVehiclePassengersList [i].currentAngularDirection = new Vector3 (newAngularDirection.z, newAngularDirection.y, newAngularDirection.x);
					} else {
						IKVehiclePassengersList [i].currentAngularDirection = Vector3.zero;
					}
				}
			}
		}
	}

	public void setNewAngularDirection (Vector3 newValue)
	{
		newAngularDirection = Vector3.Scale (newValue, extraAngularDirectioAmount);
	}

	public void setCollisionForceDirectionToPassengers (Vector3 forceDirection)
	{
		if (addCollisionForceDirectionToPassengers && HUDManager.isVehicleBeingDriven ()) {
			if (!useMinCollisionForce || minCollisionForce < forceDirection.magnitude) {
				for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
					if (!IKVehiclePassengersList [i].vehicleSeatInfo.currentlyOnFirstPerson && !IKVehiclePassengersList [i].vehicleSeatInfo.seatIsFree) {

						Vector3 newForceDirection = forceDirection;

						newForceDirection = Vector3.Scale (newForceDirection, extraCollisionForceAmount);
						IKVehiclePassengersList [i].currentSeatShake = new Vector3 (newForceDirection.z, newForceDirection.y, newForceDirection.x);
					}
				}
			}
		}
	}

	public void setPassengerFirstPersonState (bool state, GameObject passengerToCheck)
	{
		if (addCollisionForceDirectionToPassengers || addAngularDirectionToPassengers) {
			for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
				if (IKVehiclePassengersList [i].vehicleSeatInfo.currentPassenger == passengerToCheck) {
					IKVehiclePassengersList [i].vehicleSeatInfo.currentlyOnFirstPerson = state;
				}
			}
		}
	}

	public void setPlayerToStartGameOnThisVehicle (GameObject newPlayer)
	{
		startGameInThisVehicle = true;

		playerForVehicle = newPlayer;

		checkIfPlayerStartInVehicle = false;
	}

	public void setDriverExternally (GameObject currentDriver)
	{
		usingDevicesSystem usingDevicesManager = currentDriver.GetComponent<usingDevicesSystem> ();

		if (usingDevicesManager != null) {
			usingDevicesManager.clearDeviceList ();

			usingDevicesManager.addDeviceToList (vehicle);

			usingDevicesManager.updateClosestDeviceList ();

			usingDevicesManager.setCurrentVehicle (vehicle);

			getDriverPosition = true;

			usingDevicesManager.useCurrentDevice (vehicle);

			getDriverPosition = false;
		}
	}

	//if the vehicle is destroyed, remove it from the scene
	public void destroyVehicle ()
	{
		Destroy (vehicelCameraGameObject);
		Destroy (vehicle);
		Destroy (gameObject);
	}

	//if the vehicle is destroyed
	public void disableVehicle ()
	{
		vehicleDestroyed = true;
		//disable its components
		mainCollider.enabled = false;

		//if the player was driving it
		if (passengersOnVehicle || isBeingDriven) {
			//stop the vehicle

			ejectVehiclePassengers (ejectPlayerWhenDestroyed, ejectingPlayerForce);

			//disable the weapon system if the vehicle has it
			if (currentVehicleWeaponSystem != null) {
				if (currentVehicleWeaponSystem.enabled) {
					currentVehicleWeaponSystem.changeWeaponState (false);
				}
			}
		} else {
			checkIgnorPassengersCollidersState (null);
		}

		//disable the camera and the gravity control component
		vehicleCameraManager.enabled = false;

		HUDManager.mainVehicleController.disableVehicle ();

		if (vehicleGravityManager != null) {
			vehicleGravityManager.enabled = false;
		}

		Vector3 vehiclePosition = vehicle.transform.position;

		if (useExplosionForceWhenDestroyed) {
			applyDamage.setExplosion (vehiclePosition, explosionRadius, useLayerMask, layer, vehicle, true, gameObject, killObjectsInRadius, true, false, 
				explosionDamage, pushCharactersOnExplosion, applyExplosionForceToVehicles, explosionForceToVehiclesMultiplier, explosionForce, forceMode, 
				false, vehicle.transform, ignoreShield, useRemoteEventOnObjectsFound, removeEventName, damageTypeID);
		}

		//remove this vehicle from any character using device system
		for (int i = 0; i < usingDevicesManagerDetectedList.Count; i++) {
			if (usingDevicesManagerDetectedList [i] != null) {
				if (usingDevicesManagerDetectedList [i].existInDeviceList (vehicle)) {
					usingDevicesManagerDetectedList [i].removeVehicleFromList ();
					usingDevicesManagerDetectedList [i].removeCurrentVehicle (vehicle);
					usingDevicesManagerDetectedList [i].setIconButtonCanBeShownState (false);
				}
			}
		}
	}

	public void ejectVehiclePassengersOnSelfDestruct (float ejectPassengerFoce)
	{
		ejectVehiclePassengers (true, ejectPassengerFoce);
	}

	public void ejectVehiclePassengers (bool ejectPassenger, float ejectForce)
	{
//		print (ejectPassenger);

		List<GameObject> auxPassengerGameObjectList = new List<GameObject> ();
		List<passengerComponents> auxPassengerComponentsList = new List<passengerComponents> ();

		for (int i = 0; i < passengerGameObjectList.Count; i++) {
			auxPassengerGameObjectList.Add (passengerGameObjectList [i]);
			passengerComponents auxCurrentPassengerDriverComponents = new passengerComponents (passengerComponentsList [i]);
			auxPassengerComponentsList.Add (auxCurrentPassengerDriverComponents);
		}
			
		for (int i = 0; i < auxPassengerGameObjectList.Count; i++) {
			//disable the option to get off from the vehicle if the player press that button
			passengerComponents auxCurrentPassengerDriverComponents = auxPassengerComponentsList [i];

			if (auxCurrentPassengerDriverComponents.usingDevicesManager != null) {
				auxCurrentPassengerDriverComponents.usingDevicesManager.removeVehicleFromList ();

				auxCurrentPassengerDriverComponents.usingDevicesManager.disableIcon ();
			}

			Vector3 nextPlayerPosition = Vector3.zero;

			if (auxPassengerGameObjectList [i] != null) {
				nextPlayerPosition = auxPassengerGameObjectList [i].transform.position;
			}

			startOrStopVehicle (auxPassengerGameObjectList [i], null, vehicelCameraGameObject.transform.up, nextPlayerPosition);

			Vector3 vehiclePosition = vehicle.transform.position;

			if (auxPassengerGameObjectList [i] && auxCurrentPassengerDriverComponents.passengerPhysicallyOnVehicle) {
				if (ejectPassenger) {
					//eject the player from the car
					if ((activateFreeFloatingModeOnEject || activateFreeFloatingModeOnEjectEnabled) && !auxCurrentPassengerDriverComponents.playerControllerManager.isCharacterUsedByAI ()) {
						auxCurrentPassengerDriverComponents.playerGravityManager.setfreeFloatingModeOnStateWithDelay (activateFreeFloatingModeOnEjectDelay, true);
						activateFreeFloatingModeOnEjectEnabled = false;
					} 

					auxCurrentPassengerDriverComponents.playerControllerManager.ejectPlayerFromVehicle (ejectForce);
				} else {
					//kill him
					Vector3 vehicleDirection = auxPassengerGameObjectList [i].transform.position - vehiclePosition;
					vehicleDirection = vehicleDirection / vehicleDirection.magnitude;
					applyDamage.killCharacter (vehicle, auxPassengerGameObjectList [i], vehicleDirection, vehiclePosition, vehicle, false);
				}

				if (!playerVisibleInVehicle) {
					auxCurrentPassengerDriverComponents.playerControllerManager.setCharacterMeshGameObjectState (true);
					enableOrDisablePlayerVisibleInVehicle (true, auxCurrentPassengerDriverComponents);
				}
			}
		}

		if (vehicleDestroyed) {
			checkIgnorPassengersCollidersState (auxPassengerComponentsList);
		}
	}

	Coroutine CollidersStateCoroutine;

	public void checkIgnorPassengersCollidersState (List<passengerComponents> auxPassengerComponentsList)
	{
		if (CollidersStateCoroutine != null) {
			StopCoroutine (CollidersStateCoroutine);
		}

		CollidersStateCoroutine = StartCoroutine (changeIgnorePassengerCollidersStateCoroutine (auxPassengerComponentsList));
	}

	IEnumerator changeIgnorePassengerCollidersStateCoroutine (List<passengerComponents> auxPassengerComponentsList)
	{
		if (auxPassengerComponentsList != null) {
			for (int i = 0; i < auxPassengerComponentsList.Count; i++) {
				if (auxPassengerComponentsList [i].mainRagdollActivator != null) {
					HUDManager.ignoreCollisionWithVehicleColliderList (auxPassengerComponentsList [i].mainRagdollActivator.getBodyColliderList (), true);
				}
			}

			yield return new WaitForSeconds (1);

			for (int i = 0; i < auxPassengerComponentsList.Count; i++) {
				if (auxPassengerComponentsList [i].mainRagdollActivator != null) {
					HUDManager.ignoreCollisionWithVehicleColliderList (auxPassengerComponentsList [i].mainRagdollActivator.getBodyColliderList (), false);
				}

				HUDManager.ignoreCollisionWithVehicleColliderList (auxPassengerComponentsList [i].playerControllerManager.getMainCollider (), false);
			}
		}

		yield return new WaitForSeconds (0.5f);

		HUDManager.setVehiceColliderWithMeshListLayer ("Default");

		yield return null;
	}


	public void setActivateFreeFloatingModeOnEjectEnabledState (bool state)
	{
		activateFreeFloatingModeOnEjectEnabled = state;
	}

	//the player is getting in or off from the vehicle
	public bool startOrStopVehicle (GameObject currentPassenger, Transform passengerParent, Vector3 normal, Vector3 nextPlayerPos)
	{
		if (passengerGameObjectList.Count == 0) {
			
			vehicleCameraManager.setCameraPosition (false);
			vehicleCameraManager.changeCameraDrivingState (false, false);
		
			return false;
		}

		IKDrivingInformation currentIKVehiclePassengerInfo = getIKVehiclePassengerInfo (currentPassenger);

		int currentPassengerIndex = passengerGameObjectList.IndexOf (currentPassenger);
		passengerComponents currentPassengerComponents = passengerComponentsList [currentPassengerIndex];

		//print (currentPassengerComponents.playerControllerManager.getPlayerID ());

		bool passengerIsGettingOn = false;

		passengerIsGettingOn = currentIKVehiclePassengerInfo.vehicleSeatInfo.seatIsFree;

		currentIKVehiclePassengerInfo.vehicleSeatInfo.seatIsFree = !passengerIsGettingOn;

		currentIKVehiclePassengerInfo.adjustPassengerPositionActive = true;
		currentIKVehiclePassengerInfo.currentDriveIkWeightValue = 1;
		currentIKVehiclePassengerInfo.disableIKOnPassengerSmoothly = false;

		currentPassengerComponents.playerControllerManager.setActionToGetOnVehicleActiveState (false);
		currentPassengerComponents.playerControllerManager.setActionToGetOffFromVehicleActiveState (false);

		if (passengerIsGettingOn) {
			passengerIsGettingOn = true;
		}

		if (showDebugPrint) {
			print ("passenger is getting on " + passengerIsGettingOn);
		}

		bool passengerOnDriverSeat = false;
		if (currentIKVehiclePassengerInfo.vehicleSeatInfo.isDriverSeat) {
			passengerOnDriverSeat = true;
			currentPassengerDriverComponents = currentPassengerComponents;
		}

		if (showDebugPrint) {
			print ("is driver seat " + passengerOnDriverSeat);
		}

		currentPassengerComponents.passengerOnDriverSeat = passengerOnDriverSeat;

		if (passengerOnDriverSeat) {
			isBeingDriven = passengerIsGettingOn;
		} else {
			isBeingDriven = false;
		}

		//print (passengerIsGettingOn + " " + passengerOnDriverSeat);

		bool isPlayer = false;
		if (!currentPassengerComponents.playerControllerManager.usedByAI) {
			isPlayer = true;
		}

		if (isPlayer && !isBeingDriven) {
			checkDisableShowControlsMenu ();
		}


		//CHECK IN THE ACTION SYSTEM IS USED TO PLAY AN ANIMATION TO ENTER-EXIT THE VEHICLE

		bool useActionSystemToEnterExitSeat = currentIKVehiclePassengerInfo.vehicleSeatInfo.useActionSystemToEnterExitSeat;

		bool firstCameraEnabled = currentPassengerComponents.playerCameraManager.isFirstPersonActive ();

		bool canUseActionSystem = false;

		if (useActionSystemToEnterExitSeat) {
			canUseActionSystem = true;
		}

		if (vehicleDestroyed) {
			canUseActionSystem = false;
		}

		if (passengerIsGettingOn) {
			if (firstCameraEnabled) {
				canUseActionSystem = false;
			}
		} else {
			if (vehicleCameraManager.isFirstPersonActive ()) {
				canUseActionSystem = false;
			}
		}

		if (canUseActionSystem) {
			stopEnterExitActionSystemOnPassengerCoroutine (currentPassengerComponents.actionSystemCoroutine);

			if (passengerIsGettingOn) {
				HUDManager.setIgnoreSettingPassengerStateActiveState (true);
			}

			bool enterExitActionInProcess = currentIKVehiclePassengerInfo.vehicleSeatInfo.enterExitActionInProcess;

			currentPassengerComponents.actionSystemCoroutine = 
				StartCoroutine (enterExitActionSystemOnPassengerCoroutine (currentPassenger, passengerParent, normal, nextPlayerPos, 
				currentPassengerComponents, currentIKVehiclePassengerInfo, passengerIsGettingOn, isPlayer, passengerOnDriverSeat));

			if (passengerIsGettingOn) {
				return false;
			} else {
				if (enterExitActionInProcess) {
					return false;
				}

				return passengerOnDriverSeat;
			}
		} else {
			stopEnterExitActionSystemOnPassengerCoroutine (currentPassengerComponents.actionSystemCoroutine);

			return setPassengerOnVehicleState (currentPassenger, passengerParent, normal, nextPlayerPos, currentPassengerComponents,
				currentIKVehiclePassengerInfo, passengerIsGettingOn, isPlayer, passengerOnDriverSeat, false);
		}

//		return passengerOnDriverSeat;
	}

	public void stopEnterExitActionSystemOnPassengerCoroutine (Coroutine passengerCoroutine)
	{
		if (passengerCoroutine != null) {
			StopCoroutine (passengerCoroutine);
		}
	}

	IEnumerator enterExitActionSystemOnPassengerCoroutine (GameObject currentPassenger, Transform passengerParent, Vector3 normal, Vector3 nextPlayerPos, 
	                                                       passengerComponents currentPassengerComponents, IKDrivingInformation currentIKVehiclePassengerInfo,
	                                                       bool passengerIsGettingOn, bool isPlayer, bool passengerOnDriverSeat)
	{
		bool cancelingEntering = false;

		bool jumpOffFromVehicleActive = false;

		if (currentIKVehiclePassengerInfo.vehicleSeatInfo.enterExitActionInProcess) {
			if (passengerIsGettingOn) {
				print ("character entering from vehicle already in process");
			} else {
				print ("character was entering on vehicle, but that is going to be cancelled");

				cancelingEntering = true;
			}
		}

		currentIKVehiclePassengerInfo.vehicleSeatInfo.enterExitActionInProcess = true;

		bool actionActive = true;

		if (passengerIsGettingOn) {
			currentPassengerComponents.playerControllerManager.setTemporalVehicle (vehicle);
			currentIKVehiclePassengerInfo.vehicleSeatInfo.eventOnActionToEnter.Invoke ();

			currentIKVehiclePassengerInfo.vehicleSeatInfo.actionSystemToEnterVehicle.setPlayerActionActive (currentPassengerComponents.currentPassenger);
		
			currentPassengerComponents.playerControllerManager.setActionToGetOnVehicleActiveState (true);
		} else {
			if (!cancelingEntering) {
				if (!passengerIsGettingOn) {

					float currentVehicleSpeed = Mathf.Abs (HUDManager.getCurrentSpeed ());

					if (currentVehicleSpeed > currentIKVehiclePassengerInfo.vehicleSeatInfo.minSpeedToJumpOffFromVehicle) {
						jumpOffFromVehicleActive = true;
					} else {
						
						bool vehicleStopped = false;

						HUDManager.startBrakeVehicleToStopCompletely ();

						float timer = 0;

//						print ("start");

						while (!vehicleStopped) {

							timer += Time.deltaTime;

							if (timer > 20) {
								vehicleStopped = true;
							}

							currentVehicleSpeed = Mathf.Abs (HUDManager.getCurrentSpeed ());

							if (currentVehicleSpeed < 0.1f) {
								vehicleStopped = true;
							}

							yield return null;
						}
					}

//					print ("stop");

					HUDManager.endBrakeVehicleToStopCompletely ();
				}
			}


			currentPassengerComponents.playerControllerManager.setTemporalVehicle (null);

			if (cancelingEntering) {
				currentPassengerComponents.mainPlayerActionSystem.stopAllActions ();
			} else {
				currentIKVehiclePassengerInfo.vehicleSeatInfo.eventOnActionToExit.Invoke ();

				if (jumpOffFromVehicleActive) {
					currentIKVehiclePassengerInfo.vehicleSeatInfo.actionSystemToJumpOffFromVehicle.setPlayerActionActive (currentPassengerComponents.currentPassenger);
				} else {
					currentIKVehiclePassengerInfo.vehicleSeatInfo.actionSystemToExitVehicle.setPlayerActionActive (currentPassengerComponents.currentPassenger);
				}
			}

			currentIKVehiclePassengerInfo.adjustPassengerPositionActive = false;
			currentIKVehiclePassengerInfo.disableIKOnPassengerSmoothly = true;

			currentPassengerComponents.playerControllerManager.enabled = true;

			currentPassengerComponents.playerControllerManager.setUsingDeviceState (false);

			currentPassengerComponents.playerControllerManager.setAnimatorState (true);

			currentPassengerComponents.mainRigidbody.isKinematic = false;

			currentPassengerComponents.playerControllerManager.setActionToGetOffFromVehicleActiveState (true);
		}

		bool cancelAction = false;

		if (!cancelingEntering) {
			if (!passengerIsGettingOn) {
				currentPassengerComponents.usingDevicesManager.setUseDeviceButtonEnabledState (false);
			}

			currentPassengerComponents.mainPlayerActionSystem.playCurrentAnimation ();

			Rigidbody mainVehicleRigidbody = vehicle.GetComponent<Rigidbody> ();

			bool passengerParentAdjusted = false;

			float currentDistanceToVehicle = 0;

			bool actionCanBeCancelled = false;

			bool cancelActionOutsideVehicle = false;

			if (jumpOffFromVehicleActive) {
				bool isCurrentPassengerCameraLocked = !currentPassengerComponents.playerCameraManager.isCameraTypeFree ();

				if (isPlayer && passengerOnDriverSeat) {
					if (!isCurrentPassengerCameraLocked) {

						float delayToStartJumpOff = currentIKVehiclePassengerInfo.vehicleSeatInfo.delayToStartJumpOff;

						float timer = 0;

						bool delayComplete = false;

						while (!delayComplete) {

							currentPassengerComponents.playerControllerManager.setExtraCharacterVelocity (HUDManager.currentVehicleVelocity ());

							timer += Time.deltaTime;

							if (timer >= delayToStartJumpOff) {
								delayComplete = true;
							}

							yield return null;
						}

						currentPassengerComponents.playerControllerManager.setExtraCharacterVelocity (Vector3.zero);

						currentPassenger.transform.SetParent (null);

						//change the main camera parent to player's camera
					
						Transform playerCameraTransform = currentPassengerComponents.playerCameraGameObject.transform;

						if (resetCameraRotationWhenGetOff) {
							float angleY = Vector3.Angle (vehicle.transform.forward, playerCameraTransform.forward);

							angleY *= Mathf.Sign (playerCameraTransform.InverseTransformDirection (
								Vector3.Cross (vehicle.transform.forward, playerCameraTransform.forward)).y);

							playerCameraTransform.Rotate (0, -angleY, 0);
						} else {
							if (passengerOnDriverSeat) {
								setPlayerCameraDirectionWithVehicleCameraDirection (currentPassengerComponents);
							}
						}

						currentPassengerComponents.mainCameraTransform.SetParent (currentPassengerComponents.originalCameraParentTransform);

						checkCameraTranslation (false, currentPassengerComponents);
					}
				}
			}

			while (actionActive) {
//			print ("checking for action " + currentPassengerComponents.playerControllerManager.isActionActive () + " " + currentPassengerComponents.mainPlayerActionSystem.isPlayerWalkingToDirectionActive ());

				if (!currentPassengerComponents.playerControllerManager.isActionActive () && !currentPassengerComponents.mainPlayerActionSystem.isPlayerWalkingToDirectionActive ()) {
					actionActive = false;
				} else {
					if (currentPassenger != null && passengerParent != null) {
						currentDistanceToVehicle = GKC_Utils.distance (currentPassenger.transform.position, passengerParent.position);
					}

					if (passengerIsGettingOn) {
						if (!passengerParentAdjusted) {
							if (!currentPassengerComponents.mainPlayerActionSystem.isPlayerWalkingToDirectionActive () &&
							    !currentPassengerComponents.mainPlayerActionSystem.isRotatingTowardFacingDirection ()) {
								currentPassenger.transform.SetParent (vehicle.transform);

								passengerParentAdjusted = true;

								currentPassengerComponents.usingDevicesManager.setUseDeviceButtonEnabledState (false);
							} else {
								if (currentDistanceToVehicle > 20) {
									cancelActionOutsideVehicle = true;
									actionCanBeCancelled = true;
								}
							}
						}
					}

					if (passengerParentAdjusted) {
						currentPassengerComponents.playerControllerManager.setExtraCharacterVelocity (HUDManager.currentVehicleVelocity ());
					}

					if (currentIKVehiclePassengerInfo.vehicleSeatInfo.cancelActionEnterExitVehicleIfSpeedTooHigh) {
					
						float currentVehicleVelocity = mainVehicleRigidbody.velocity.magnitude;
					
						if (Mathf.Abs (currentVehicleVelocity) > currentIKVehiclePassengerInfo.vehicleSeatInfo.minSpeedToCancelActionEnterExitVehicle) {
							actionCanBeCancelled = true;

							print ("Vehicle too fast, stopping the action");
						}

						if (passengerParentAdjusted) {
							if (currentDistanceToVehicle > 5) {
								print ("Too far from vehicle, cancelling");
								actionCanBeCancelled = true;
							}

							cancelActionOutsideVehicle = false;
						} else {
							cancelActionOutsideVehicle = true;
						}
						
						if (actionCanBeCancelled) {
							if (passengerParentAdjusted) {
								currentPassenger.transform.SetParent (null);
							}
							print ("CANCELLING");

							cancelAction = true;

							actionActive = false;

							currentPassengerComponents.passengerOnDriverSeat = false;

							currentIKVehiclePassengerInfo.vehicleSeatInfo.seatIsFree = true;

							currentIKVehiclePassengerInfo.vehicleSeatInfo.enterExitActionInProcess = false;

							currentPassengerComponents.playerControllerManager.setActionToGetOnVehicleActiveState (false);
							currentPassengerComponents.playerControllerManager.setActionToGetOffFromVehicleActiveState (false);

							currentIKVehiclePassengerInfo.vehicleSeatInfo.actionSystemToEnterVehicle.setPlayerActionDeactivate (currentPassengerComponents.currentPassenger);

							currentPassengerComponents.mainPlayerActionSystem.stopAllActions ();
						 
							print ("Cancelling action outside of vehicle " + cancelActionOutsideVehicle);

							if (cancelActionOutsideVehicle) {
								currentPassengerComponents.mainRemoteEventSystem.callRemoteEvent (currentIKVehiclePassengerInfo.vehicleSeatInfo.remoteEventToCancelActionEnterExitOutsideVehicle);
							} else {
								currentPassengerComponents.mainRemoteEventSystem.callRemoteEvent (currentIKVehiclePassengerInfo.vehicleSeatInfo.remoteEventToCancelActionEnterExitInsideVehicle);
							}

							currentIKVehiclePassengerInfo.vehicleSeatInfo.eventToCancelActionEnterAndExit.Invoke ();

							removePassengerInfo (currentIKVehiclePassengerInfo);

							stopEnterExitActionSystemOnPassengerCoroutine (currentPassengerComponents.actionSystemCoroutine);
						}
					}
				}

				yield return null;
			}
		}

		if (!cancelAction) {
			if (passengerIsGettingOn) {
				currentIKVehiclePassengerInfo.vehicleSeatInfo.actionSystemToEnterVehicle.setPlayerActionDeactivate (currentPassengerComponents.currentPassenger);
			} else {
				if (jumpOffFromVehicleActive) {
					currentIKVehiclePassengerInfo.vehicleSeatInfo.actionSystemToJumpOffFromVehicle.setPlayerActionDeactivate (currentPassengerComponents.currentPassenger);
				} else {
					currentIKVehiclePassengerInfo.vehicleSeatInfo.actionSystemToExitVehicle.setPlayerActionDeactivate (currentPassengerComponents.currentPassenger);
				}
			}

			currentPassengerComponents.playerControllerManager.setActionToGetOnVehicleActiveState (false);
			currentPassengerComponents.playerControllerManager.setActionToGetOffFromVehicleActiveState (false);

			currentIKVehiclePassengerInfo.vehicleSeatInfo.enterExitActionInProcess = false;

			if (cancelingEntering) {
//				print ("Cancelling passenger entering on vehicle properly");

				currentPassenger.transform.SetParent (null);

				currentPassengerComponents.passengerOnDriverSeat = false;

				currentIKVehiclePassengerInfo.vehicleSeatInfo.seatIsFree = true;

				currentIKVehiclePassengerInfo.vehicleSeatInfo.eventToCancelActionEnterAndExit.Invoke ();
			
				removePassengerInfo (currentIKVehiclePassengerInfo);
			} else {
//				print ("confirm to enter ");

				setPassengerOnVehicleState (currentPassenger, passengerParent, normal, nextPlayerPos, currentPassengerComponents,
					currentIKVehiclePassengerInfo, passengerIsGettingOn, isPlayer, passengerOnDriverSeat, true);
			
				if (passengerIsGettingOn) {
					HUDManager.setPassengerState (passengerOnDriverSeat);
				} else {
					HUDManager.updatePassengersInVehicleCheckState ();
				}
			}
		}
	}

	bool setPassengerOnVehicleState (GameObject currentPassenger, Transform passengerParent, Vector3 normal, Vector3 nextPlayerPos, 
	                                 passengerComponents currentPassengerComponents, IKDrivingInformation currentIKVehiclePassengerInfo,
	                                 bool passengerIsGettingOn, bool isPlayer, bool passengerOnDriverSeat, bool useActionSystemToEnterExitSeat)
	{

		//set the state driving as the current state of the player
		currentPassengerComponents.playerControllerManager.setDrivingState (passengerIsGettingOn, vehicle, HUDManager.getVehicleName ());

		isBeingDrivenRemotely = !checkIfNotDrivenRemotely (isPlayer);

		if (!isBeingDrivenRemotely) {
			currentPassengerComponents.playerControllerManager.enabled = !passengerIsGettingOn;
			currentPassengerComponents.passengerPhysicallyOnVehicle = true;

			currentPassengerComponents.playerControllerManager.changeScriptState (!passengerIsGettingOn);

			currentPassengerComponents.playerControllerManager.setPlayerSetAsChildOfParentState (passengerIsGettingOn);
		} else {
			currentPassengerComponents.playerControllerManager.changeScriptState (!passengerIsGettingOn);

			if (currentPassengerComponents.mapManager != null) {
				if (passengerIsGettingOn) {
					currentPassengerComponents.mapManager.setNewObjectToFollow (vehicle);
				} else {
					currentPassengerComponents.mapManager.setNewObjectToFollow (currentPassengerComponents.currentPassenger);
				}
			}
		}

		currentPassengerComponents.playerControllerManager.setUsingDeviceState (passengerIsGettingOn);

		if (isPlayer) {
			currentPassengerComponents.pauseManager.usingDeviceState (passengerIsGettingOn);

			currentPassengerComponents.mainInventoryManager.enableOrDisableWeaponSlotsParentOutOfInventory (!passengerIsGettingOn);
		}

		//enable or disable the collider and the rigidbody of the player
		if (!isBeingDrivenRemotely) {
			currentPassengerComponents.playerControllerManager.setMainColliderState (!passengerIsGettingOn);
	
			currentPassengerComponents.mainRigidbody.isKinematic = passengerIsGettingOn;

			//get the IK positions of the car to use them in the player
			currentPassengerComponents.IKManager.setDrivingState (passengerIsGettingOn, currentIKVehiclePassengerInfo);
		}
		
		if (isPlayer && passengerOnDriverSeat) {
			vehicleCameraManager.getPlayer (currentPassenger);
		}

		//check if the camera in the player is in first or third view, to set the current view in the vehicle
		bool firstCameraEnabled = currentPassengerComponents.playerCameraManager.isFirstPersonActive ();
		if (isPlayer && passengerIsGettingOn && passengerOnDriverSeat) {
			vehicleCameraManager.setCameraPosition (firstCameraEnabled);
		}

		stopCheckCameraTranslation (currentPassengerComponents.playerCameraManager.getVehicleCameraMovementCoroutine ());

		if (isPlayer) {
			//enable and disable the player's HUD and the vehicle's HUD
			currentPassengerComponents.mainPlayerHUDManager.enableOrDisableHUD (!passengerIsGettingOn);

			if (passengerOnDriverSeat) {
				if (HUDManager.showVehicleHUDActive ()) {
					currentPassengerComponents.mainPlayerHUDManager.enableOrDisableVehicleHUD (isBeingDriven);
				}

				//get the vehicle's HUD elements to show the current values of the vehicle, like health, energy, ammo....
				HUDManager.getHUDBars (currentPassengerComponents.mainPlayerHUDManager, currentPassengerComponents.mainPlayerHUDManager.getHudElements (), isBeingDriven);

				checkActivateActionScreen (passengerIsGettingOn, currentPassengerComponents.playerControllerManager);

				if (passengerIsGettingOn) {
					if (useEventOnDriverGetOn) {
						eventOnDriverGetOn.Invoke (currentPassengerComponents.currentPassenger);
					}
				} else {
					if (useEventOnDriverGetOff) {
						eventOnDriverGetOff.Invoke (currentPassengerDriverComponents.currentPassenger);
					}
				}
			}
		}

		currentPassengerComponents.mainFootStepManager.enableOrDisableFootSteps (!passengerIsGettingOn);

		currentPassengerComponents.healthManager.setSliderVisibleState (!passengerIsGettingOn);

		if (actionManager != null && isPlayer && passengerOnDriverSeat) {
			actionManager.enableOrDisableInput (isBeingDriven, currentPassengerComponents.currentPassenger);
		}

		if (passengerIsGettingOn) {
			currentPassengerComponents.playerCameraManager.playOrPauseHeadBob (!passengerIsGettingOn);
			currentPassengerComponents.playerCameraManager.stopAllHeadbobMovements ();
		} else {
			currentPassengerComponents.playerCameraManager.pauseHeadBodWithDelay (0.5f);
			currentPassengerComponents.playerCameraManager.playOrPauseHeadBob (!passengerIsGettingOn);
		}


		if (passengerIsGettingOn) {
			if (currentIKVehiclePassengerInfo.setNewScaleOnPassenger) {
				currentPassengerComponents.currentPassenger.transform.localScale = currentIKVehiclePassengerInfo.newScaleOnPassenger;
			}
		} else {
			if (currentIKVehiclePassengerInfo.setNewScaleOnPassenger) {
				currentPassengerComponents.currentPassenger.transform.localScale = Vector3.one;
			}
		}


		bool isCurrentPassengerCameraLocked = !currentPassengerComponents.playerCameraManager.isCameraTypeFree ();
	
		//if the first camera was enabled, set the current main camera position in the first camera position of the vehicle
		if (firstCameraEnabled) {
			//enable the player's body to see it
			if (passengerIsGettingOn) {
				currentPassengerComponents.playerControllerManager.setCharacterMeshGameObjectState (true);
				//if the first person was actived, disable the player's body
			} else {
				currentPassengerComponents.playerControllerManager.setCharacterMeshGameObjectState (false);
			}

			currentPassengerComponents.playerControllerManager.setAnimatorState (passengerIsGettingOn);
		}

		if (isBeingDrivenRemotely) {
			currentPassengerComponents.playerControllerManager.setDrivingRemotelyState (passengerIsGettingOn);
		}

		currentPassengerComponents.usingDevicesManager.setUseDeviceButtonEnabledState (true);

		//if the player is driving it
		if (passengerIsGettingOn) {
			//check the current state of the player, to check if he is carrying an object, aiming, etc... to disable that state
			currentPassengerComponents.statesManager.checkPlayerStates ();

			if (isPlayer && passengerOnDriverSeat) {
				currentPassengerComponents.mainPlayerHUDManager.setControlList (actionManager);
				currentPassengerComponents.mainPlayerHUDManager.setCurrentVehicleHUD (vehicle);
				//disable or enable the vehicle camera
				vehicleCameraManager.changeCameraDrivingState (passengerIsGettingOn, resetCameraRotationWhenGetOn);
			}

			//disable the camera rotation of the player 
			if (!isCurrentPassengerCameraLocked) {
				currentPassengerComponents.playerCameraManager.pauseOrPlayCamera (!passengerIsGettingOn);
			}

			if (isPlayer) {
				//change the main camera from the player camera component to the vehicle's camera component
				currentPassengerComponents.originalCameraParentTransform = currentPassengerComponents.playerCameraManager.getCameraTransform ();
			
				if (!isCurrentPassengerCameraLocked) {

					vehicleCameraManager.adjustCameraToCurrentCollisionDistance ();

					//else the main camera position in the third camera position of the vehicle
					currentPassengerComponents.mainCameraTransform.SetParent (vehicleCameraManager.currentState.cameraTransform);
				}
			}

			//set the player's position and parent inside the car
			if (!isBeingDrivenRemotely) {
				currentPassenger.transform.SetParent (passengerParent.transform);
				currentPassenger.transform.localPosition = Vector3.zero;
				currentPassenger.transform.localRotation = Quaternion.identity;
				currentPassenger.transform.position = currentIKVehiclePassengerInfo.vehicleSeatInfo.seatTransform.position;
				currentPassenger.transform.rotation = currentIKVehiclePassengerInfo.vehicleSeatInfo.seatTransform.rotation;
			}
				
			//get the vehicle camera rotation
			mainCameraTargetRotation = Quaternion.identity;
			//vehicleCameraManager.currentState.cameraTransform.localRotation;

			if (passengerOnDriverSeat) {
				//reset the player's camera rotation input values
				currentPassengerComponents.playerCameraManager.setLookAngleValue (Vector2.zero);

				//set the player's camera rotation as the same in the vehicle
				currentPassengerComponents.playerCameraGameObject.transform.rotation = vehicelCameraGameObject.transform.rotation;
			}

			currentPassengerComponents.usingDevicesManager.setDrivingState (passengerIsGettingOn);

			currentPassengerComponents.usingDevicesManager.clearDeviceListButOne (vehicle);

			currentPassengerComponents.usingDevicesManager.checkIfSetOriginalShaderToPreviousDeviceFound (vehicle);

			//set the same rotation in the camera pivot
			if (isPlayer) {
				if (passengerOnDriverSeat) {
					currentPassengerComponents.playerCameraManager.getPivotCameraTransform ().localRotation = vehicleCameraManager.currentState.pivotTransform.localRotation;

					currentPassengerComponents.playerCameraManager.resetMainCameraTransformLocalPosition ();

					currentPassengerComponents.playerCameraManager.resetPivotCameraTransformLocalPosition ();

					currentPassengerComponents.playerCameraManager.resetCurrentCameraStateAtOnce ();
				} else {
					if (resetCameraRotationWhenGetOn) {
						Transform playerCameraTransform = currentPassengerComponents.playerCameraGameObject.transform;

						float angleY = Vector3.Angle (vehicle.transform.forward, playerCameraTransform.forward);

						angleY *= Mathf.Sign (playerCameraTransform.InverseTransformDirection (
							Vector3.Cross (vehicle.transform.forward, playerCameraTransform.forward)).y);
						
						playerCameraTransform.Rotate (0, -angleY, 0);
					}
				}
			}

			if (isBeingDrivenRemotely) {
				mainCollider.enabled = false;
			} else {
				if (!useActionSystemToEnterExitSeat) {
					if (currentIKVehiclePassengerInfo.useAnimationActionIDOnDriveIdle) {
						currentPassengerComponents.mainPlayerActionSystem.setNewActionIDExternally (currentIKVehiclePassengerInfo.animationActionID);
					}
				}
			}
		} 

		//the player gets off from the vehicle
		else {
			currentPassengerComponents.playerControllerManager.setTemporalVehicle (null);

			if (isPlayer && passengerOnDriverSeat) {
				currentPassengerComponents.mainPlayerHUDManager.setCurrentVehicleHUD (null);
			}
				
			if (!isBeingDrivenRemotely) {
				//set the parent of the player as null
				currentPassenger.transform.SetParent (null);

				if (isPlayer && !passengerOnDriverSeat) {
					if (!isCurrentPassengerCameraLocked) {
						//change the main camera parent to player's camera
						currentPassengerComponents.mainCameraTransform.SetParent (null);
					}
				}

				if (!useActionSystemToEnterExitSeat) {
					//set the player's position at the correct side of the car
					currentPassenger.transform.position = nextPlayerPos;

					currentPassengerComponents.mainPlayerActionSystem.setNewActionIDExternally (resetAnimatorDrivingStateID);
				}

				bool setPlayerGravityOwnGravityDirection = false;
				//set the current gravity of the player's as the same in the vehicle
				if (vehicleGravityManager != null && vehicleGravityManager.changeDriverGravityWhenGetsOffActive ()) {
					//if the option to change player's gravity is on, change his gravity direction
					currentPassengerComponents.playerGravityManager.setNormal (normal);
				} else {
					//else, check if the player was using the gravity power before get on the vehicle
					if (!currentPassengerComponents.playerGravityManager.isUsingRegularGravity ()) {
						setPlayerGravityOwnGravityDirection = true;
					} else {
						//else, his rotation is (0,0,0)
						currentPassenger.transform.rotation = Quaternion.identity;
					}
				}

				Transform playerCameraTransform = currentPassengerComponents.playerCameraGameObject.transform;

				//set the player's camera position in the correct place
				if (!useActionSystemToEnterExitSeat) {
					playerCameraTransform.position = nextPlayerPos;
				}

				if (resetCameraRotationWhenGetOff) {
					float angleY = Vector3.Angle (vehicle.transform.forward, playerCameraTransform.forward);

					angleY *= Mathf.Sign (playerCameraTransform.InverseTransformDirection (
						Vector3.Cross (vehicle.transform.forward, playerCameraTransform.forward)).y);

					playerCameraTransform.Rotate (0, -angleY, 0);
				} else {
					if (passengerOnDriverSeat) {
						setPlayerCameraDirectionWithVehicleCameraDirection (currentPassengerComponents);
					}
				}
				
				if (vehicleGravityManager != null && !vehicleGravityManager.changeDriverGravityWhenGetsOffActive ()) {
					if (!vehicleGravityManager.isUsingRegularGravity ()) {
						if (!currentPassengerComponents.playerGravityManager.isUsingRegularGravity ()) {
							setPlayerGravityOwnGravityDirection = true;
						} else {
							playerCameraTransform.rotation = Quaternion.identity;
						}
					}
				}

				if (setPlayerGravityOwnGravityDirection) {
					currentPassenger.transform.position = getGetOffPosition (currentIKVehiclePassengerInfo);

					playerCameraTransform.position = currentPassenger.transform.position;

					currentPassengerComponents.playerGravityManager.setNormal (currentPassengerComponents.playerGravityManager.getCurrentNormal ());
				}

				if (isPlayer) {
					mainCameraTargetRotation = Quaternion.identity;
				}
					
				if (drawFireWeaponIfCarryingPreviously) {
					if (currentPassengerComponents.carryingFireWeaponsPrevioulsy) {
						currentPassengerComponents.playerWeaponManager.checkIfDrawSingleOrDualWeapon ();
					}
				}

				if (drawMeleeWeaponIfCarryingPreviously) {
					if (currentPassengerComponents.carryingMeleeWeaponsPreviously) {
						GKC_Utils.drawMeleeWeaponGrabbed (currentPassengerComponents.currentPassenger);
					}
				}
			}

			if (vehicleDestroyed && firstCameraEnabled) {
				if (!isCurrentPassengerCameraLocked) {
					currentPassengerComponents.playerCameraManager.pauseOrPlayCamera (!passengerIsGettingOn);
				}

				if (passengerOnDriverSeat) {
					vehicleCameraManager.changeCameraDrivingState (passengerIsGettingOn, resetCameraRotationWhenGetOn);
				}

				return passengerOnDriverSeat;
			}

			if (isBeingDrivenRemotely) {
				setPlayerCameraDirectionWithVehicleCameraDirection (currentPassengerComponents);
			}

			//add originalCameraParentTransform to passengerComponents
			if (isPlayer && passengerOnDriverSeat) {
				if (!isCurrentPassengerCameraLocked) {
					//change the main camera parent to player's camera

					if (setPlayerCameraStateOnGetOff && !vehicleDestroyed) {
						if (setPlayerCameraStateOnFirstPersonOnGetOff) {
							if (!firstCameraEnabled) {
								currentPassengerComponents.playerCameraManager.changeCameraToThirdOrFirstView (true);
							}
						} else {
							if (firstCameraEnabled) {
								currentPassengerComponents.playerCameraManager.changeCameraToThirdOrFirstView (false);
							}

							currentPassengerComponents.playerCameraManager.setCameraState (playerCameraStateOnGetOff);
						}

						currentPassengerComponents.playerCameraManager.resetCurrentCameraStateAtOnce ();

						currentPassengerComponents.playerCameraManager.configureCameraAndPivotPositionAtOnce ();
					}

					currentPassengerComponents.mainCameraTransform.SetParent (currentPassengerComponents.originalCameraParentTransform);
				}
			}

			if (isPlayer && passengerOnDriverSeat) {
				//disable or enable the vehicle camera
				vehicleCameraManager.changeCameraDrivingState (passengerIsGettingOn, resetCameraRotationWhenGetOn);
			}

			currentPassengerComponents.usingDevicesManager.setDrivingState (passengerIsGettingOn);

			if (isBeingDrivenRemotely) {
				currentPassengerComponents.usingDevicesManager.removeVehicleFromList ();
				currentPassengerComponents.usingDevicesManager.removeCurrentVehicle (vehicle);
				currentPassengerComponents.usingDevicesManager.setIconButtonCanBeShownState (false);

				mainCollider.enabled = true;
			}
		}

		if (!isBeingDrivenRemotely) {
			if (!playerVisibleInVehicle) {
				currentPassengerComponents.playerControllerManager.setCharacterMeshGameObjectState (!passengerIsGettingOn);
				enableOrDisablePlayerVisibleInVehicle (passengerIsGettingOn, currentPassengerComponents);
			}

			if (hidePlayerWeaponsWhileDriving) {
				currentPassengerComponents.playerWeaponManager.enableOrDisableEnabledWeaponsMesh (!passengerIsGettingOn);

				if (currentPassengerComponents.mainMeleeWeaponsGrabbedManager != null) {
					currentPassengerComponents.mainMeleeWeaponsGrabbedManager.enableOrDisableAllMeleeWeaponMeshesOnCharacterBody (!passengerIsGettingOn);
				
					currentPassengerComponents.mainMeleeWeaponsGrabbedManager.enableOrDisableAllMeleeWeaponShieldMeshesOnCharacterBody (!passengerIsGettingOn);
				}
			}
		}

		if (passengerOnDriverSeat) {
			vehicleCameraManager.setCheckCameraShakeActiveState (passengerIsGettingOn);
		}

		if (isPlayer) {
			//stop the current transition of the main camera from the player to the vehicle and viceversa if the camera is moving from one position to another
			if (!isCurrentPassengerCameraLocked) {
				checkCameraTranslation (passengerIsGettingOn, currentPassengerComponents);
			}
		} else {
			currentPassengerComponents.playerCameraManager.pauseOrPlayCamera (!passengerIsGettingOn);
		}

		if (!passengerIsGettingOn) {
			removePassengerInfo (currentIKVehiclePassengerInfo);

			setCanBeDrivenRemotelyState (originalCanBeDrivenRemotelyValue);
		}

		if (useRemoteEventsOnPassengers) {
			if (currentPassengerComponents.mainRemoteEventSystem != null) {
				if (passengerIsGettingOn) {
					for (int i = 0; i < remoteEventNameListGetOn.Count; i++) {
						currentPassengerComponents.mainRemoteEventSystem.callRemoteEvent (remoteEventNameListGetOn [i]);
					}
				} else {
					for (int i = 0; i < remoteEventNameListGetOff.Count; i++) {
						currentPassengerComponents.mainRemoteEventSystem.callRemoteEvent (remoteEventNameListGetOff [i]);
					}
				}
			}
		}
			
		return passengerOnDriverSeat;
	}

	public void setPlayerCameraDirectionWithVehicleCameraDirection (passengerComponents currentPassengerComponents)
	{
		Quaternion vehiclePivotRotation = Quaternion.identity;

		if (vehicleCameraManager.currentState.firstPersonCamera) {
			Vector3 cameraRotation = vehicleCameraManager.currentState.pivotTransform.localEulerAngles;

			cameraRotation = vehicelCameraGameObject.transform.eulerAngles + vehicelCameraGameObject.transform.up * cameraRotation.y;

			if (vehicleGravityManager != null) {
				if (vehicleGravityManager.isUsingRegularGravity ()) {
					currentPassengerComponents.playerCameraGameObject.transform.rotation = Quaternion.Euler (cameraRotation);
				}
			} else {
				currentPassengerComponents.playerCameraGameObject.transform.rotation = Quaternion.Euler (cameraRotation);
			}

			vehiclePivotRotation = vehicleCameraManager.getCurrentCameraTransform ().localRotation;
		} else {
			if (vehicleGravityManager != null) {
				if (vehicleGravityManager.isUsingRegularGravity ()) {
					currentPassengerComponents.playerCameraGameObject.transform.rotation = vehicelCameraGameObject.transform.rotation;
				}
			} else {
				currentPassengerComponents.playerCameraGameObject.transform.rotation = vehicelCameraGameObject.transform.rotation;
			}

			vehiclePivotRotation = vehicleCameraManager.getCurrentCameraPivot ().localRotation;
		}

		currentPassengerComponents.playerCameraManager.getPivotCameraTransform ().localRotation = vehiclePivotRotation;

		float newLookAngleValue = vehiclePivotRotation.eulerAngles.x;
		if (newLookAngleValue > 180) {
			newLookAngleValue -= 360;
		}

		currentPassengerComponents.playerCameraManager.setLookAngleValue (new Vector2 (0, newLookAngleValue));
	}

	public bool checkIfNotDrivenRemotely (bool currentPassengerIsPlayer)
	{
		if (!canBeDrivenRemotely || !currentPassengerIsPlayer || (currentPassengerIsPlayer && !canBeDrivenRemotely)) {
			return true;
		}
		return false;
	}

	public void setCanBeDrivenRemotelyState (bool state)
	{
		canBeDrivenRemotely = state;
	}

	public bool getCanBeDrivenRemotelyValue ()
	{
		return canBeDrivenRemotely;
	}

	public override void setPlayerVisibleInVehicleState (bool state)
	{
		playerVisibleInVehicle = state;
	}

	public override void setResetCameraRotationWhenGetOnState (bool state)
	{
		resetCameraRotationWhenGetOn = state;
	}

	public override void setEjectPlayerWhenDestroyedState (bool state)
	{
		ejectPlayerWhenDestroyed = state;
	}

	public void enableOrDisablePlayerVisibleInVehicle (bool state, passengerComponents currentPassengerComponents)
	{
		if (!currentPassengerComponents.playerCameraManager.isFirstPersonActive ()) {
			
			currentPassengerComponents.playerControllerManager.setCharacterMeshesListToDisableOnEventState (!state);

			currentPassengerComponents.playerWeaponManager.enableOrDisableWeaponsMesh (!state);
		} else {
			currentPassengerComponents.playerControllerManager.setCharacterMeshGameObjectState (false);
		}
	}

	public void checkCameraTranslationByPlayerCamera (bool state, playerCamera playerCameraManager)
	{
		stopCheckCameraTranslation (playerCameraManager.getVehicleCameraMovementCoroutine ());

		Coroutine moveCameraCoroutine = StartCoroutine (adjustCamera (state, playerCameraManager.getMainCamera ().transform, playerCameraManager, true));

		playerCameraManager.setVehicleCameraMovementCoroutine (moveCameraCoroutine);
	}

	public void stopCameraTranslationByPlayerCamera (playerCamera playerCameraManager)
	{
		stopCheckCameraTranslation (playerCameraManager.getVehicleCameraMovementCoroutine ());
	}

	//stop the current coroutine and start it again
	public void checkCameraTranslation (bool state, passengerComponents currentPassengerComponents)
	{
		stopCheckCameraTranslation (currentPassengerComponents.playerCameraManager.getVehicleCameraMovementCoroutine ());

		currentPassengerComponents.moveCameraCoroutine = 
			StartCoroutine (adjustCamera (state, currentPassengerComponents.mainCameraTransform, currentPassengerComponents.playerCameraManager, currentPassengerComponents.passengerOnDriverSeat));

		currentPassengerComponents.playerCameraManager.setVehicleCameraMovementCoroutine (currentPassengerComponents.moveCameraCoroutine);
	}

	public void stopCheckCameraTranslation (Coroutine moveCameraCoroutine)
	{
		if (moveCameraCoroutine != null) {
			StopCoroutine (moveCameraCoroutine);
		}
	}

	//move the camera position and rotation from the player's camera to vehicle's camera and vice-versa
	IEnumerator adjustCamera (bool state, Transform mainCameraTransform, playerCamera playerCameraManager, bool passengerOnDriverSeat)
	{
		if (!passengerOnDriverSeat) {
			if (state) {
				playerCameraManager.setCameraState (playerCameraManager.getDefaultVehiclePassengerStateName ());

				playerCameraManager.configureCurrentLerpState (vehicleCameraManager.currentState.pivotTransform.localPosition, vehicleCameraManager.currentState.cameraTransform.localPosition);
				playerCameraManager.setTargetToFollow (vehicleCameraManager.transform);
				playerCameraManager.resetCurrentCameraStateAtOnce ();
				playerCameraManager.configureCameraAndPivotPositionAtOnce ();
				playerCameraManager.getMainCamera ().transform.SetParent (playerCameraManager.getCameraTransform ());
			} else {
				print ("This way");
				playerCameraManager.pauseOrPlayCamera (false);
				playerCameraManager.getMainCamera ().transform.SetParent (null);
		
				playerCameraManager.setCameraState (playerCameraManager.getDefaultThirdPersonStateName ());

				playerCameraManager.resetCurrentCameraStateAtOnce ();
				playerCameraManager.configureCameraAndPivotPositionAtOnce ();
				playerCameraManager.setOriginalTargetToFollow ();

				playerCameraManager.getMainCamera ().transform.SetParent (playerCameraManager.getCameraTransform ());
			}
		}

		float i = 0;
		//store the current rotation of the camera
		Quaternion currentRotation = mainCameraTransform.localRotation;

		//store the current position of the camera
		Vector3 currentPosition = mainCameraTransform.localPosition;

		//if the game starts with the player inside the vehicle, set his camera in the vehicle camera target transform directly
		if (!checkIfPlayerStartInVehicle && startGameInThisVehicle) {
			mainCameraTransform.localRotation = mainCameraTargetRotation;
			mainCameraTransform.localPosition = mainCameraTargetPosition;
		} else {
			//translate position and rotation camera
			while (i < 1) {
				i += Time.deltaTime * 2;
				mainCameraTransform.localRotation = Quaternion.Lerp (currentRotation, mainCameraTargetRotation, i);
				mainCameraTransform.localPosition = Vector3.Lerp (currentPosition, mainCameraTargetPosition, i);
			
				yield return null;
			}
		}

		if (!passengerOnDriverSeat) {
			if (state) {
				playerCameraManager.pauseOrPlayCamera (true);
			}
		}

		//enable the camera rotation of the player if the vehicle is not being droven
		if (!state) {
			playerCameraManager.pauseOrPlayCamera (!state);
		}
	}

	//set the camera when the player is driving on locked camera
	public void setPlayerCameraParentAndPosition (Transform mainCameraTransform, playerCamera playerCameraManager)
	{
		vehicleCameraManager.changeCameraDrivingState (true, true);
		
		mainCameraTransform.SetParent (vehicleCameraManager.getCurrentCameraTransform ());

		for (int i = 0; i < passengerComponentsList.Count; i++) {
			if (passengerComponentsList [i].playerCameraManager == playerCameraManager) {

				checkCameraTranslation (true, passengerComponentsList [i]);

				return;
			}
		}
	}

	public void checkDisableShowControlsMenu ()
	{
		if (controlsMenuOpened) {
			openOrCloseControlsMenu (false);
		}
	}

	public void inputOpenOrCloseControlsMenu (bool state)
	{
		if (currentPassengerDriverComponents.pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
			return;
		}

		openOrCloseControlsMenu (state);
	}

	public void openOrCloseControlsMenu (bool state)
	{
		if ((!currentPassengerDriverComponents.playerControllerManager.isPlayerMenuActive () || controlsMenuOpened) && currentPassengerDriverComponents.playerControllerManager.isUsingDevice ()) {
			controlsMenuOpened = state;

			currentPassengerDriverComponents.pauseManager.openOrClosePlayerMenu (controlsMenuOpened, 
				currentPassengerDriverComponents.mainPlayerHUDManager.vehicleControlsMenu.transform, currentPassengerDriverComponents.mainPlayerHUDManager.useBlurUIPanel);
			
			currentPassengerDriverComponents.pauseManager.showOrHideCursor (controlsMenuOpened);

			//disable the touch controls
			currentPassengerDriverComponents.pauseManager.checkTouchControls (!controlsMenuOpened);

			//disable the camera rotation
			pauseOrPlayVehicleCamera (controlsMenuOpened);

			currentPassengerDriverComponents.pauseManager.usingSubMenuState (controlsMenuOpened);

			currentPassengerDriverComponents.pauseManager.enableOrDisableDynamicElementsOnScreen (!controlsMenuOpened);

			currentPassengerDriverComponents.mainPlayerHUDManager.openOrCloseControlsMenu (controlsMenuOpened);

			if (currentVehicleWeaponSystem != null) {
				currentVehicleWeaponSystem.setWeaponsPausedState (controlsMenuOpened);
			}

			currentPassengerDriverComponents.usingDevicesManager.setUseDeviceButtonEnabledState (!controlsMenuOpened);

			currentPassengerDriverComponents.pauseManager.checkEnableOrDisableTouchZoneList (!controlsMenuOpened);
		}
	}

	public bool setUnlockCursorState (bool state)
	{
		if (currentPassengerDriverComponents == null) {
			return false;
		}

		if (cursorUnlocked == state) {
			return false;
		}

		if (currentPassengerDriverComponents.pauseManager != null &&
		    (!currentPassengerDriverComponents.playerControllerManager.isPlayerMenuActive () || cursorUnlocked) &&
		    currentPassengerDriverComponents.playerControllerManager.isUsingDevice ()) {

			cursorUnlocked = state;
			currentPassengerDriverComponents.pauseManager.openOrClosePlayerMenu (cursorUnlocked, null, false);
			currentPassengerDriverComponents.pauseManager.showOrHideCursor (cursorUnlocked);

			//disable the camera rotation
			pauseOrPlayVehicleCamera (cursorUnlocked);

			currentPassengerDriverComponents.pauseManager.usingSubMenuState (cursorUnlocked);

			if (currentVehicleWeaponSystem != null) {
				currentVehicleWeaponSystem.setWeaponsPausedState (cursorUnlocked);
			}

			currentPassengerDriverComponents.pauseManager.checkEnableOrDisableTouchZoneList (!cursorUnlocked);

			return true;
		}

		return false;
	}

	public void setCameraAndWeaponsPauseState (bool state)
	{
		pauseOrPlayVehicleCamera (state);

		if (currentVehicleWeaponSystem != null) {
			currentVehicleWeaponSystem.setWeaponsPausedState (state);
		}
	}

	public void pauseOrPlayVehicleCamera (bool state)
	{
		vehicleCameraManager.pauseOrPlayVehicleCamera (state);
	}

	public GameObject getcurrentDriver ()
	{
		if (hidePlayerFromNPCs) {
			return null;
		}

		for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
			if (IKVehiclePassengersList [i].vehicleSeatInfo.isDriverSeat && !IKVehiclePassengersList [i].vehicleSeatInfo.seatIsFree) {
				return IKVehiclePassengersList [i].vehicleSeatInfo.currentPassenger;
			}
		}

		for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
			if (!IKVehiclePassengersList [i].vehicleSeatInfo.seatIsFree) {
				return IKVehiclePassengersList [i].vehicleSeatInfo.currentPassenger;
			}
		}

		return null;
	}

	public vehicleHUDManager getHUDManager ()
	{
		return HUDManager;
	}

	public override GameObject getVehicleGameObject ()
	{
		return vehicle;
	}

	public override Transform getCustomVehicleTransform ()
	{
		if (useCustomVehicleGameObject && customVehicleGameObject != null) {
			return customVehicleGameObject.transform;
		} else {
			return vehicle.transform;
		}
	}

	public override GameObject getVehicleCameraGameObject ()
	{
		return vehicleCameraManager.gameObject;
	}

	public override vehicleGravityControl getVehicleGravityControl ()
	{
		return vehicleGravityManager;
	}

	public override void setTriggerToDetect (Collider newCollider)
	{
		HUDManager.OnTriggerEnter (newCollider);
	}

	public Vector3 getPassengerGetOffPosition (GameObject currentPassenger)
	{
		//search if the current passenger is in the list, to avoid get the closest seat
		int currentPassengerGameObjectIndex = -1;

		if (passengerGameObjectList.Contains (currentPassenger)) {
			for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
				if (IKVehiclePassengersList [i].vehicleSeatInfo.currentPassenger == currentPassenger) {
					currentPassengerGameObjectIndex = i;
				}
			}	
		}

		IKDrivingInformation currentIKVehiclePassengerInfo;

		if (currentPassengerGameObjectIndex == -1) {

			int currentIKVehiclePassengerIndex = -1;

			//print (currentPassenger.name);

			bool setDriverPosition = false;

			playerController currentPlayerController = currentPassenger.GetComponent<playerController> ();

			if (currentPlayerController != null) {
				if (!currentPlayerController.usedByAI) {
					if (playerIsAlwaysDriver) {
						if (isBeingDriven) {
							if (showDebugPrint) {
								print ("vehicle is already being driven, setting new character as passenger");
							}
						} else {
							if (showDebugPrint) {
								print ("vehicle is not being driven, setting new character as driver");
							}

							if (!currentPlayerController.canCharacterGetOnVehicles ()) {
								return -Vector3.one;	
							}

							setDriverPosition = true;
						}
					}
				}
			} else {
				print ("WARNING: player not found");
			}

			if (getDriverPosition || setDriverPosition) {
				currentIKVehiclePassengerIndex = getDriverSeatPassengerIndex ();
			} else {
				currentIKVehiclePassengerIndex = getClosestSeatToPassengerIndex (currentPassenger);
			}
				
			if (currentIKVehiclePassengerIndex > -1) {
				currentIKVehiclePassengerInfo = IKVehiclePassengersList [currentIKVehiclePassengerIndex];
			} else {
				return -Vector3.one;	
			}

			bool isGettingOn = setPassengerInfo (currentIKVehiclePassengerInfo, currentPassenger);

			if (isGettingOn) {
				return Vector3.zero;
			}
		} else {
			currentIKVehiclePassengerInfo = IKVehiclePassengersList [currentPassengerGameObjectIndex];
		}

		Transform seatTransform = currentIKVehiclePassengerInfo.vehicleSeatInfo.seatTransform;
		Vector3 getOffPosition = Vector3.zero;
		Vector3 rayDirection = Vector3.zero;
		Vector3 passengerPosition = Vector3.zero;
		Vector3 rayPosition = Vector3.zero;
		float rayDistance = 0;

		Ray ray = new Ray ();

		bool canGetOff = false;
		RaycastHit[] hits;

		if (currentIKVehiclePassengerInfo.vehicleSeatInfo.getOffPlace == seatInfo.getOffSide.right) {
			getOffPosition = currentIKVehiclePassengerInfo.vehicleSeatInfo.rightGetOffPosition.position;
			rayDirection = seatTransform.right;
		} else {
			getOffPosition = currentIKVehiclePassengerInfo.vehicleSeatInfo.leftGetOffPosition.position;
			rayDirection = -seatTransform.right;
		}

		rayDistance = GKC_Utils.distance (seatTransform.position, getOffPosition);

		rayPosition = getOffPosition - rayDirection * rayDistance;

		//set the ray origin at the vehicle position with a little offset set in the inspector
		ray.origin = rayPosition;
		//set the ray direction to the left

		ray.direction = rayDirection;
		//get all the colliders in that direction where the yellow sphere is placed

		hits = Physics.SphereCastAll (ray, 0.1f, rayDistance, HUDManager.layer);
		//get the position where the player will be placed

		passengerPosition = getOffPosition;

		if (hits.Length == 0) {
			//any obstacle detected, so the player can get off
			canGetOff = true;
		} else {
			if (showDebugPrint) {
				if (hits.Length > 0) {
					print ("Obstacle detected at the " + currentIKVehiclePassengerInfo.vehicleSeatInfo.getOffPlace + " side");
				}
			}
		}

		//some obstacles found
		for (int i = 0; i < hits.Length; i++) {
			//check the distance to that obstacles, if they are lower that the rayDistance, the player can get off
			if (hits [i].distance > rayDistance) {
				canGetOff = true;
			} else {
				canGetOff = false;
			}

			if (showDebugPrint) {
				print ("Obstacle detected is " + hits [i].collider.name + " at a distance of " + hits [i].distance);
			}
		}

		//if the left side is blocked, then check the right side in the same way that previously
		if (!canGetOff) {
			if (showDebugPrint) {
				print ("Checking the other side to get off");
			}

			if (currentIKVehiclePassengerInfo.vehicleSeatInfo.getOffPlace == seatInfo.getOffSide.right) {
				getOffPosition = currentIKVehiclePassengerInfo.vehicleSeatInfo.leftGetOffPosition.position;
				rayDirection = -seatTransform.right;
			} else {
				getOffPosition = currentIKVehiclePassengerInfo.vehicleSeatInfo.rightGetOffPosition.position;
				rayDirection = seatTransform.right;
			}

			rayDistance = GKC_Utils.distance (seatTransform.position, getOffPosition);

			rayPosition = getOffPosition - rayDirection * rayDistance;

			ray.origin = rayPosition;

			ray.direction = rayDirection;

			hits = Physics.SphereCastAll (ray, 0.1f, rayDistance, HUDManager.layer);

			passengerPosition = getOffPosition;

			if (hits.Length == 0) {
				canGetOff = true;
			} else {
				if (showDebugPrint) {
					if (hits.Length > 0) {
						print ("Obstacle detected at the other side");
					}
				}
			}

			for (int i = 0; i < hits.Length; i++) {
				if (hits [i].distance > rayDistance) {
					canGetOff = true;
				} else {
					canGetOff = false;
				}

				if (showDebugPrint) {
					print ("Obstacle detected is " + hits [i].collider.name + " at a distance of " + hits [i].distance);
				}
			}
		}
	
		//if both sides are blocked, exit the function and the player can't get off
		if (!canGetOff) {
			return -Vector3.one;	
		}

		//if any side is avaliable then check a ray in down direction, to place the player above the ground
		RaycastHit hit;

		if (Physics.Raycast (getOffPosition, -vehicle.transform.up, out hit, currentIKVehiclePassengerInfo.vehicleSeatInfo.getOffDistance, HUDManager.layer)) {
			Debug.DrawRay (getOffPosition, -vehicle.transform.up * hit.distance, Color.yellow);
			passengerPosition = hit.point;
		}

		return passengerPosition;
	}

	public Vector3 getGetOffPosition (IKDrivingInformation passengerInfo)
	{
		if (passengerInfo.vehicleSeatInfo.getOffPlace == seatInfo.getOffSide.right) {
			return passengerInfo.vehicleSeatInfo.rightGetOffPosition.position;
		} else {
			return passengerInfo.vehicleSeatInfo.leftGetOffPosition.position;
		}
	}

	public int getClosestSeatToPassengerIndex (GameObject currentPassenger)
	{
		bool characterCanDrive = false;

		playerController currentPlayerController = currentPassenger.GetComponent<playerController> ();

		if (currentPlayerController != null) {
			if (currentPlayerController.canCharacterDrive ()) {
				characterCanDrive = true;
			}
		}

		int currentIKVehiclePassengerIndex = -1;

		float maxDistance = Mathf.Infinity;

		for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
			if (IKVehiclePassengersList [i].vehicleSeatInfo.seatIsFree) {
				if (characterCanDrive || (!IKVehiclePassengersList [i].vehicleSeatInfo.isDriverSeat && !characterCanDrive)) {
					
					float currentDistance = GKC_Utils.distance (currentPassenger.transform.position, IKVehiclePassengersList [i].vehicleSeatInfo.seatTransform.position);

					if (currentDistance < maxDistance) {
						maxDistance = currentDistance;
						currentIKVehiclePassengerIndex = i;
					}
				}
			}
		}

		return currentIKVehiclePassengerIndex;
	}

	public int getDriverSeatPassengerIndex ()
	{
		for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
			if (IKVehiclePassengersList [i].vehicleSeatInfo.isDriverSeat) {
				return i;
			}
		}

		return -1;
	}

	public IKDrivingInformation getIKVehiclePassengerInfo (GameObject passenger)
	{
		for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
			if (IKVehiclePassengersList [i].vehicleSeatInfo.currentPassenger == passenger) {
				return IKVehiclePassengersList [i];
			}
		}

		return null;
	}

	public bool setPassengerInfo (IKDrivingInformation passengerInfo, GameObject passenger)
	{
		bool isGettingOn = false;

		if (!passengerGameObjectList.Contains (passenger)) {
			passengerGameObjectList.Add (passenger);

			passengerInfo.vehicleSeatInfo.currentPassenger = passenger;

			isGettingOn = true;

			setPassengerComponents (passenger);

			passengersOnVehicle = true;
		}

		return isGettingOn;
	}


	public void setPassengerComponents (GameObject currentPassenger)
	{
		passengerComponents newPassengerComponentsList = new passengerComponents ();
		newPassengerComponentsList.currentPassenger = currentPassenger;

		newPassengerComponentsList.mainPlayerComponentsManager = currentPassenger.GetComponent<playerComponentsManager> ();

		newPassengerComponentsList.playerControllerManager = newPassengerComponentsList.mainPlayerComponentsManager.getPlayerController ();

		newPassengerComponentsList.usingDevicesManager = newPassengerComponentsList.mainPlayerComponentsManager.getUsingDevicesSystem ();

		newPassengerComponentsList.playerCameraManager = newPassengerComponentsList.mainPlayerComponentsManager.getPlayerCamera ();

		newPassengerComponentsList.playerCameraGameObject = newPassengerComponentsList.playerCameraManager.gameObject;

		newPassengerComponentsList.playerGravityManager = newPassengerComponentsList.mainPlayerComponentsManager.getGravitySystem ();

		newPassengerComponentsList.playerWeaponManager = newPassengerComponentsList.mainPlayerComponentsManager.getPlayerWeaponsManager ();

		newPassengerComponentsList.mainMeleeWeaponsGrabbedManager = newPassengerComponentsList.mainPlayerComponentsManager.getMeleeWeaponsGrabbedManager ();

		newPassengerComponentsList.mainCamera = newPassengerComponentsList.playerCameraManager.getMainCamera ();
		newPassengerComponentsList.mainCameraTransform = newPassengerComponentsList.mainCamera.transform;

		newPassengerComponentsList.mainRigidbody = newPassengerComponentsList.playerControllerManager.getRigidbody ();

		newPassengerComponentsList.IKManager = newPassengerComponentsList.mainPlayerComponentsManager.getIKSystem ();

		newPassengerComponentsList.mainFootStepManager = newPassengerComponentsList.mainPlayerComponentsManager.getFootStepManager ();

		newPassengerComponentsList.healthManager = newPassengerComponentsList.mainPlayerComponentsManager.getHealth ();

		newPassengerComponentsList.statesManager = newPassengerComponentsList.mainPlayerComponentsManager.getPlayerStatesManager ();

		newPassengerComponentsList.pauseManager = newPassengerComponentsList.mainPlayerComponentsManager.getPauseManager ();

		newPassengerComponentsList.mainInventoryManager = newPassengerComponentsList.mainPlayerComponentsManager.getInventoryManager ();

		newPassengerComponentsList.mainPlayerHUDManager = newPassengerComponentsList.mainPlayerComponentsManager.getPlayerHUDManager ();

		newPassengerComponentsList.mapManager = newPassengerComponentsList.mainPlayerComponentsManager.getMapSystem ();

		newPassengerComponentsList.mainRagdollActivator = newPassengerComponentsList.mainPlayerComponentsManager.getRagdollActivator ();

		newPassengerComponentsList.carryingFireWeaponsPrevioulsy = newPassengerComponentsList.playerWeaponManager.isUsingWeapons ();

		newPassengerComponentsList.carryingMeleeWeaponsPreviously = newPassengerComponentsList.playerControllerManager.isPlayerUsingMeleeWeapons ();

		newPassengerComponentsList.mainRemoteEventSystem = newPassengerComponentsList.mainPlayerComponentsManager.getRemoteEventSystem ();

		newPassengerComponentsList.mainPlayerActionSystem = newPassengerComponentsList.mainPlayerComponentsManager.getPlayerActionSystem ();



		newPassengerComponentsList.previousPlayerStatusID = newPassengerComponentsList.playerControllerManager.playerStatusID;

		newPassengerComponentsList.previousIdleID = newPassengerComponentsList.playerControllerManager.currentIdleID;

		newPassengerComponentsList.previousStrafeID = newPassengerComponentsList.playerControllerManager.currentStrafeID;


		newPassengerComponentsList.playerControllerManager.setPlayerStatusIDValue (0);

		newPassengerComponentsList.playerControllerManager.setCurrentIdleIDValue (0);

		newPassengerComponentsList.playerControllerManager.setCurrentStrafeIDValue (0);

		newPassengerComponentsList.playerControllerManager.updatePlayerStatusIDOnAnimator ();

		newPassengerComponentsList.playerControllerManager.updateIdleIDOnAnimator ();

		newPassengerComponentsList.playerControllerManager.updateStrafeIDOnAnimator ();


		passengerComponentsList.Add (newPassengerComponentsList);
	}

	public void removePassengerInfo (IKDrivingInformation passengerInfo)
	{
		if (passengerGameObjectList.Contains (passengerInfo.vehicleSeatInfo.currentPassenger)) {
			int passengerGameObjecToRemoveIndex = passengerGameObjectList.IndexOf (passengerInfo.vehicleSeatInfo.currentPassenger);

			passengerComponents currentPassengerComponents = passengerComponentsList [passengerGameObjecToRemoveIndex];

			currentPassengerComponents.playerControllerManager.setPlayerStatusIDValue (currentPassengerComponents.previousPlayerStatusID);

			currentPassengerComponents.playerControllerManager.setCurrentIdleIDValue (currentPassengerComponents.previousIdleID);

			currentPassengerComponents.playerControllerManager.setCurrentStrafeIDValue (currentPassengerComponents.previousStrafeID);

			currentPassengerComponents.playerControllerManager.updatePlayerStatusIDOnAnimator ();

			currentPassengerComponents.playerControllerManager.updateIdleIDOnAnimator ();

			currentPassengerComponents.playerControllerManager.updateStrafeIDOnAnimator ();

			if (HUDManager.isInteractionDisabledOnVehicle ()) {
				currentPassengerComponents.usingDevicesManager.removeVehicleFromList ();

				currentPassengerComponents.usingDevicesManager.removeCurrentVehicle (vehicle);

				currentPassengerComponents.usingDevicesManager.setIconButtonCanBeShownState (false);
			}

			passengerComponentsList.RemoveAt (passengerGameObjecToRemoveIndex);

			passengerGameObjectList.Remove (passengerInfo.vehicleSeatInfo.currentPassenger);

			if (passengerGameObjectList.Count == 0) {
				passengersOnVehicle = false;
			}

			passengerInfo.vehicleSeatInfo.currentPassenger = null;
		}
	}

	public void addPassenger ()
	{
		IKDrivingInformation newIKDrivingInformation = new IKDrivingInformation ();
		newIKDrivingInformation.Name = "New Seat";

		IKVehiclePassengersList.Add (newIKDrivingInformation);

		updateComponent ();
	}

	public List<GameObject> getPassengerGameObjectList ()
	{
		return passengerGameObjectList;
	}
		
	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		playerController playerControllerToCheck = col.GetComponent<playerController> ();

		if (playerControllerToCheck != null) {
			if (isEnter) {
				if (!playerControllerToCheck.canCharacterGetOnVehicles ()) {
					return;
				}

				usingDevicesSystem currentUsingDevicesSystem = col.gameObject.GetComponent<usingDevicesSystem> ();
				currentUsingDevicesSystem.setCurrentVehicle (vehicle);

				if (!usingDevicesManagerDetectedList.Contains (currentUsingDevicesSystem)) {
					usingDevicesManagerDetectedList.Add (currentUsingDevicesSystem);
				}

				if (!passengersInsideTrigger.Contains (col.gameObject)) {
					passengersInsideTrigger.Add (col.gameObject);

					if (sendPlayersEnterExitTriggerToEvent) {
						eventToSendPlayersEnterTriggerToEvent.Invoke (col.gameObject);
					}
				}
			} else {
				usingDevicesSystem currentUsingDevicesSystem = col.gameObject.GetComponent<usingDevicesSystem> ();
				currentUsingDevicesSystem.removeCurrentVehicle (vehicle);	

				if (usingDevicesManagerDetectedList.Contains (currentUsingDevicesSystem)) {
					usingDevicesManagerDetectedList.Remove (currentUsingDevicesSystem);
				}

				if (passengersInsideTrigger.Contains (col.gameObject)) {
					passengersInsideTrigger.Remove (col.gameObject);

					if (sendPlayersEnterExitTriggerToEvent) {
						eventToSendPlayersExitTriggerToEvent.Invoke (col.gameObject);
					}
				}
			}
		}
	}

	public bool isVehicleFull ()
	{
		if (passengerGameObjectList.Count == IKVehiclePassengersList.Count) {
			return true;
		}

		return false;
	}

	public Collider getMainCollider ()
	{
		return mainCollider;
	}

	public void checkActivateActionScreen (bool state, playerController playerControllerManager)
	{
		if (activateActionScreen) {
			if (playerControllerManager != null) {
				playerControllerManager.getPlayerInput ().enableOrDisableActionScreen (actionScreenName, state);
			}
		}
	}

	public void getOffFromVehicle ()
	{
		if (isBeingDriven) {
			if (currentPassengerDriverComponents != null) {
				if (currentPassengerDriverComponents.usingDevicesManager != null) {
					currentPassengerDriverComponents.usingDevicesManager.useDevice ();
				}
			}
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

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

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			for (int i = 0; i < IKVehiclePassengersList.Count; i++) {
				showIKDrivingInformationGizmo (IKVehiclePassengersList [i]);
			}

			if (useExplosionForceWhenDestroyed) {
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere (vehicle.transform.position, explosionRadius);
			}
		}
	}

	void showIKDrivingInformationGizmo (IKDrivingInformation currentIKDrivingInfo)
	{
		if (currentIKDrivingInfo.showIKPositionsGizmo) {
			for (int i = 0; i < currentIKDrivingInfo.IKDrivingPos.Count; i++) {
				if (currentIKDrivingInfo.IKDrivingPos [i].position) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (currentIKDrivingInfo.IKDrivingPos [i].position.position, gizmoRadius);
				}
			}

			for (int i = 0; i < currentIKDrivingInfo.IKDrivingKneePos.Count; i++) {
				if (currentIKDrivingInfo.IKDrivingKneePos [i].position) {
					Gizmos.color = Color.blue;
					Gizmos.DrawSphere (currentIKDrivingInfo.IKDrivingKneePos [i].position.position, gizmoRadius);
				}
			}

			if (currentIKDrivingInfo.steerDirecion) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (currentIKDrivingInfo.steerDirecion.position, gizmoRadius);
			}

			if (currentIKDrivingInfo.headLookDirection) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (currentIKDrivingInfo.headLookDirection.position, gizmoRadius);
			}

			if (currentIKDrivingInfo.headLookPosition) {
				Gizmos.color = Color.gray;
				Gizmos.DrawSphere (currentIKDrivingInfo.headLookPosition.position, gizmoRadius);
			}

			if (vehicle) {
				if (currentIKDrivingInfo.vehicleSeatInfo.leftGetOffPosition) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (currentIKDrivingInfo.vehicleSeatInfo.leftGetOffPosition.position, 0.1f);
					Gizmos.DrawLine (currentIKDrivingInfo.vehicleSeatInfo.leftGetOffPosition.position, 
						currentIKDrivingInfo.vehicleSeatInfo.leftGetOffPosition.position - vehicle.transform.up * currentIKDrivingInfo.vehicleSeatInfo.getOffDistance);
				}

				if (currentIKDrivingInfo.vehicleSeatInfo.rightGetOffPosition) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (currentIKDrivingInfo.vehicleSeatInfo.rightGetOffPosition.position, 0.1f);
					Gizmos.DrawLine (currentIKDrivingInfo.vehicleSeatInfo.rightGetOffPosition.position, 
						currentIKDrivingInfo.vehicleSeatInfo.rightGetOffPosition.position - vehicle.transform.up * currentIKDrivingInfo.vehicleSeatInfo.getOffDistance);
				}
			}
		}

		if (currentIKDrivingInfo.vehicleSeatInfo.seatTransform) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere (currentIKDrivingInfo.vehicleSeatInfo.seatTransform.position, 0.1f);
		}
	}

	[System.Serializable]
	public class IKDrivingInformation
	{
		public string Name;

		public bool setNewScaleOnPassenger;
		public Vector3 newScaleOnPassenger;

		[Space]

		public bool showIKPositionsGizmo = true;
		public bool useIKOnVehicle = true;
		public List<IKDrivingPositions> IKDrivingPos = new List<IKDrivingPositions> ();
		public List<IKDrivingKneePositions> IKDrivingKneePos = new List<IKDrivingKneePositions> ();

		[Space]

		public bool useSteerDirection;
		public Transform steerDirecion;
		public bool useHeadLookDirection;
		public Transform headLookDirection;
		public bool useHeadLookPosition;
		public Transform headLookPosition;

		[Space]

		public seatInfo vehicleSeatInfo;

		[Space]

		public bool adjustPassengerPositionActive = true;

		public bool disableIKOnPassengerSmoothly;
	
		public float currentDriveIkWeightValue = 1;

		public bool shakePlayerBodyOnCollision;
		public Transform playerBodyParent;

		[Space]

		public Vector3 currentSeatUpRotation;
		public Vector3 currentSeatShake;

		public Vector3 currentAngularDirection;

		public float stabilitySpeed;
		public float shakeSpeed;
		public float shakeFadeSpeed;

		public Vector3 shakeForceDirectionMinClamp = new Vector3 (-6, -6, -6);
		public Vector3 shakeForceDirectionMaxClamp = new Vector3 (6, 6, 6);
		public Vector3 forceDirection = new Vector3 (1, 1, 1);

		public bool useAnimationActionIDOnDriveIdle;
		public int animationActionID = 666778;

		public bool showGizmo = true;
	}

	[System.Serializable]
	public class IKDrivingPositions
	{
		public string Name;
		public AvatarIKGoal limb;
		public Transform position;
	}

	[System.Serializable]
	public class IKDrivingKneePositions
	{
		public string Name;
		public AvatarIKHint knee;
		public Transform position;
	}

	[System.Serializable]
	public class seatInfo
	{
		public GameObject currentPassenger;
		public bool seatIsFree = true;
		public Transform seatTransform;
		public Transform rightGetOffPosition;
		public Transform leftGetOffPosition;
		public float getOffDistance;
		public getOffSide getOffPlace;
		public bool isDriverSeat;

		public bool useGrabbingHandID;
		public int rightGrabbingHandID = 1;
		public int leftGrabbingHandID = 1;

		public enum getOffSide
		{
			left,
			right
		}

		public bool currentlyOnFirstPerson;

		public bool useActionSystemToEnterExitSeat;
		public UnityEvent eventOnActionToEnter;
		public UnityEvent eventOnActionToExit;

		public UnityEvent eventToCancelActionEnterAndExit;

		public actionSystem actionSystemToEnterVehicle;
		public actionSystem actionSystemToExitVehicle;
		public actionSystem actionSystemToJumpOffFromVehicle;

		public string remoteEventToCancelActionEnterExitInsideVehicle = "Cancel Action Enter Exit Inside Vehicle";
		public string remoteEventToCancelActionEnterExitOutsideVehicle = "Cancel Action Enter Exit Outside Vehicle";

		public bool cancelActionEnterExitVehicleIfSpeedTooHigh = true;
		public float minSpeedToCancelActionEnterExitVehicle = 40;

		public float minSpeedToJumpOffFromVehicle = 30;

		public float delayToStartJumpOff = 0.5f;

		public bool enterExitActionInProcess;
	}

	[System.Serializable]
	public class passengerComponents
	{
		public GameObject currentPassenger;
		public playerController playerControllerManager;
		public usingDevicesSystem usingDevicesManager;
		public GameObject playerCameraGameObject;
		public gravitySystem playerGravityManager;
		public playerCamera playerCameraManager;
		public playerWeaponsManager playerWeaponManager;

		public meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager;

		public Camera mainCamera;
		public Transform mainCameraTransform;
		public Rigidbody mainRigidbody;
		public IKSystem IKManager;
		public footStepManager mainFootStepManager;
		public health healthManager;
		public playerStatesManager statesManager;
		public menuPause pauseManager;
		public bool passengerPhysicallyOnVehicle;
		public bool carryingFireWeaponsPrevioulsy;

		public bool carryingMeleeWeaponsPreviously;

		public Transform originalCameraParentTransform;
		public bool passengerOnDriverSeat;
		public playerHUDManager mainPlayerHUDManager;
		public mapSystem mapManager;
		public ragdollActivator mainRagdollActivator;

		public inventoryManager mainInventoryManager;

		public playerComponentsManager mainPlayerComponentsManager;

		public remoteEventSystem mainRemoteEventSystem;

		public playerActionSystem mainPlayerActionSystem;


		public int previousPlayerStatusID;

		public int previousIdleID;

		public int previousStrafeID;


		public Coroutine actionSystemCoroutine;

		public Coroutine moveCameraCoroutine;

		public passengerComponents (passengerComponents newComponent)
		{
			playerControllerManager = newComponent.playerControllerManager;
			usingDevicesManager = newComponent.usingDevicesManager;
			playerCameraGameObject = newComponent.playerCameraGameObject;
			playerGravityManager = newComponent.playerGravityManager;
			playerCameraManager = newComponent.playerCameraManager;
			playerWeaponManager = newComponent.playerWeaponManager;

			mainMeleeWeaponsGrabbedManager = newComponent.mainMeleeWeaponsGrabbedManager;

			mainCamera = newComponent.mainCamera;
			mainCameraTransform = newComponent.mainCameraTransform;
			mainRigidbody = newComponent.mainRigidbody;
			IKManager = newComponent.IKManager;
			mainFootStepManager = newComponent.mainFootStepManager;
			healthManager = newComponent.healthManager;
			statesManager = newComponent.statesManager;
			pauseManager = newComponent.pauseManager;
			passengerPhysicallyOnVehicle = newComponent.passengerPhysicallyOnVehicle;
			originalCameraParentTransform = newComponent.originalCameraParentTransform;
			passengerOnDriverSeat = newComponent.passengerOnDriverSeat;
			mainPlayerHUDManager = newComponent.mainPlayerHUDManager;
			mapManager = newComponent.mapManager;

			mainRagdollActivator = newComponent.mainRagdollActivator;

			mainPlayerComponentsManager = newComponent.mainPlayerComponentsManager;

			mainInventoryManager = newComponent.mainInventoryManager;

			moveCameraCoroutine = newComponent.moveCameraCoroutine;

			mainRemoteEventSystem = mainPlayerComponentsManager.getRemoteEventSystem ();

			mainPlayerActionSystem = mainPlayerComponentsManager.getPlayerActionSystem ();

			actionSystemCoroutine = newComponent.actionSystemCoroutine;



			previousPlayerStatusID = newComponent.previousPlayerStatusID;

			previousIdleID = newComponent.previousIdleID;

			previousStrafeID = newComponent.previousStrafeID;
		}

		public passengerComponents ()
		{

		}
	}
}