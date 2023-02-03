using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GKCSimpleRiderSystem : mainRiderSystem
{
	[Header ("Main Settings")]
	[Space]

	public GameObject vehicleGameObject;

	public string vehicleName;

	public Transform passengerParent;

	public GameObject customVehicleGameObject;

	public bool playerVisibleInVehicle = true;
	public bool hidePlayerWeaponsWhileDriving;

	public bool drawFireWeaponIfCarryingPreviously = true;
	public bool drawMeleeWeaponIfCarryingPreviously = true;

	public bool resetCameraRotationWhenGetOff;

	public LayerMask obstacleLayer;

	public bool checkIfDriverIsOnGroundOnGetOff;

	public int resetAnimatorDrivingStateID = 666777;


	[Space]
	[Header ("Ride Object On Start Settings")]
	[Space]

	public bool startGameInThisVehicle;

	public bool setCurrentPlayerManually;

	public bool searchPlayerOnSceneIfNotAssigned = true;

	public GameObject playerForVehicle;

	[Space]
	[Header ("Rider Passenger Settings")]
	[Space]

	public IKDrivingSystem.IKDrivingInformation IKRiderInfo;

	[Space]
	[Header ("Override System Settings")]
	[Space]

	public bool objectUseOverrideSystem;

	public objectToOverrideSystem mainObjectToOverrideSystem;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool riderActive;

	public GameObject currentRiderGameObject;

	public bool isBeingDriven;

	public bool passengerIsGettingOn;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnDriverGetOn;
	public eventParameters.eventToCallWithGameObject eventOnDriverGetOn;

	public bool useEventOnDriverGetOff;
	public eventParameters.eventToCallWithGameObject eventOnDriverGetOff;

	public bool useEventOnStartGame;
	public float delayToActivateEventsOnStart;
	public UnityEvent eventOnStartGame;
	
	[Space]
	[Header ("Remote Event Settings")]
	[Space]

	public bool useRemoteEventsOnPassengers;
	public List<string> remoteEventNameListGetOn = new List<string> ();
	public List<string> remoteEventNameListGetOff = new List<string> ();

	[Space]
	[Header ("Components")]
	[Space]

	public inputActionManager mainInputActionManager;

    
	IKDrivingSystem.passengerComponents currentPassengerComponents;

	bool checkIfPlayerStartInVehicle;

	bool playerAssignedProperly;

	Vector3 customPassengerPosition = Vector3.zero;


	void Start ()
	{
		if (useEventOnStartGame) {
			if (delayToActivateEventsOnStart > 0) {
				StartCoroutine (activateEventOnStartCoroutine ());
			} else {
				eventOnStartGame.Invoke ();
			}
		}
		
		StartCoroutine (checkStartGameInVehicleCoroutine ());
	}

	IEnumerator activateEventOnStartCoroutine ()
	{
		yield return new WaitForSeconds (delayToActivateEventsOnStart);

		eventOnStartGame.Invoke ();
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
				if (setCurrentPlayerManually || playerForVehicle == null) {
					if (currentRiderGameObject == null) {
						if (!playerAssignedProperly) {
							findPlayerOnScene ();
						}
					}

					playerForVehicle = currentRiderGameObject;
				}

				if (playerForVehicle != null) {
					setDriverExternally (playerForVehicle);
				} else {
					print ("Warning: assign the player to drive this car in the field Player For Vehicle in IK Driving System inspector");
				}
			}

			checkIfPlayerStartInVehicle = true;
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
		currentRiderGameObject = currentDriver;

		if (currentRiderGameObject == null) {
			return;
		}

		usingDevicesSystem usingDevicesManager = currentDriver.GetComponent<usingDevicesSystem> ();

		if (usingDevicesManager != null) {
			usingDevicesManager.clearDeviceList ();

			usingDevicesManager.addDeviceToList (vehicleGameObject);

			usingDevicesManager.setCurrentVehicle (vehicleGameObject);

			usingDevicesManager.useCurrentDevice (vehicleGameObject);

		}
	}

	public void stopDriveVehicleExternally ()
	{
		if (isBeingDriven) {
			if (currentPassengerComponents != null) {
				if (currentPassengerComponents.usingDevicesManager != null) {
					currentPassengerComponents.usingDevicesManager.clearDeviceList ();

					currentPassengerComponents.usingDevicesManager.addDeviceToList (vehicleGameObject);

					currentPassengerComponents.usingDevicesManager.setCurrentVehicle (vehicleGameObject);

					currentPassengerComponents.usingDevicesManager.useCurrentDevice (vehicleGameObject);
				}
			}
		}
	}

	public void setRiderState (bool state)
	{
		if (riderActive == state) {
			return;
		}

		if (currentRiderGameObject == null) {
			if (!playerAssignedProperly) {
				findPlayerOnScene ();

				if (currentRiderGameObject == null) {
					print ("WARNING: no player controller has been assigned to the mission." +
					" Make sure to use a trigger to activate the mission or assign the player manually");

					return;
				}
			}

			return;
		}

		riderActive = state;

		if (objectUseOverrideSystem) {
			startOrStopObjectToOverride (riderActive);
		} else {
			startOrStopVehicle (riderActive);
		}
	}


	public void startOrStopObjectToOverride (bool state)
	{
		setPassengerInfo (IKRiderInfo, currentRiderGameObject);

		passengerIsGettingOn = false;

		passengerIsGettingOn = IKRiderInfo.vehicleSeatInfo.seatIsFree;

		IKRiderInfo.vehicleSeatInfo.seatIsFree = !passengerIsGettingOn;

		IKRiderInfo.adjustPassengerPositionActive = true;
		IKRiderInfo.currentDriveIkWeightValue = 1;
		IKRiderInfo.disableIKOnPassengerSmoothly = false;

		currentPassengerComponents.playerControllerManager.setActionToGetOnVehicleActiveState (false);
		currentPassengerComponents.playerControllerManager.setActionToGetOffFromVehicleActiveState (false);

		if (showDebugPrint) {
			print ("passenger is getting on " + passengerIsGettingOn);
		}

		bool passengerOnDriverSeat = false;
		if (IKRiderInfo.vehicleSeatInfo.isDriverSeat) {
			passengerOnDriverSeat = true;
		}

		if (showDebugPrint) {
			print ("is driver seat " + passengerOnDriverSeat);
		}

		currentPassengerComponents.passengerOnDriverSeat = passengerOnDriverSeat;

		isBeingDriven = false;

		if (passengerOnDriverSeat) {
			isBeingDriven = passengerIsGettingOn;
		} else {
			isBeingDriven = false;
		}

		if (showDebugPrint) {
			print (passengerIsGettingOn + " " + passengerOnDriverSeat + " " + isBeingDriven);
		}

		bool isPlayer = false;

		if (!currentPassengerComponents.playerControllerManager.usedByAI) {
			isPlayer = true;
		}

		//set the state driving as the current state of the player
		currentPassengerComponents.playerControllerManager.setDrivingState (passengerIsGettingOn, vehicleGameObject, vehicleName);

		currentPassengerComponents.passengerPhysicallyOnVehicle = true;

		currentPassengerComponents.playerControllerManager.setUsingDeviceState (passengerIsGettingOn);

		if (isPlayer) {
			currentPassengerComponents.pauseManager.usingDeviceState (passengerIsGettingOn);

			currentPassengerComponents.mainInventoryManager.enableOrDisableWeaponSlotsParentOutOfInventory (!passengerIsGettingOn);
		}

		if (isPlayer) {
			//enable and disable the player's HUD and the vehicle's HUD
			currentPassengerComponents.mainPlayerHUDManager.enableOrDisableHUD (!passengerIsGettingOn);

			if (passengerOnDriverSeat) {
				if (passengerIsGettingOn) {
					if (useEventOnDriverGetOn) {
						eventOnDriverGetOn.Invoke (currentPassengerComponents.currentPassenger);
					}
				} else {
					if (useEventOnDriverGetOff) {
						eventOnDriverGetOff.Invoke (currentPassengerComponents.currentPassenger);
					}
				}
			}
		}

		currentPassengerComponents.mainFootStepManager.enableOrDisableFootSteps (!passengerIsGettingOn);

		currentPassengerComponents.healthManager.setSliderVisibleState (!passengerIsGettingOn);

		currentPassengerComponents.usingDevicesManager.setUseDeviceButtonEnabledState (true);

		//if the player is driving it
		if (passengerIsGettingOn) {
			//check the current state of the player, to check if he is carrying an object, aiming, etc... to disable that state
			currentPassengerComponents.statesManager.checkPlayerStates ();

			//check the current state of the player, to check if he is carrying an object, aiming, etc... to disable that state
			currentPassengerComponents.usingDevicesManager.setDrivingState (passengerIsGettingOn);

			currentPassengerComponents.usingDevicesManager.clearDeviceListButOne (vehicleGameObject);

			currentPassengerComponents.usingDevicesManager.checkIfSetOriginalShaderToPreviousDeviceFound (vehicleGameObject);

			if (IKRiderInfo.useAnimationActionIDOnDriveIdle) {
				currentPassengerComponents.mainPlayerActionSystem.setNewActionIDExternally (IKRiderInfo.animationActionID);
			}
		}

		//the player gets off from the vehicle
		else {
			currentPassengerComponents.playerControllerManager.setTemporalVehicle (null);

			if (isPlayer && passengerOnDriverSeat) {
				currentPassengerComponents.mainPlayerHUDManager.setCurrentVehicleHUD (null);
			}

			if (IKRiderInfo.useAnimationActionIDOnDriveIdle) {
				currentPassengerComponents.mainPlayerActionSystem.setNewActionIDExternally (resetAnimatorDrivingStateID);
			} else {
				currentPassengerComponents.mainPlayerActionSystem.setNewActionIDExternally (0);
			}

			currentPassengerComponents.usingDevicesManager.setDrivingState (passengerIsGettingOn);
		}

		if (hidePlayerWeaponsWhileDriving) {
			currentPassengerComponents.playerWeaponManager.enableOrDisableEnabledWeaponsMesh (!passengerIsGettingOn);

			if (currentPassengerComponents.mainMeleeWeaponsGrabbedManager != null) {
				currentPassengerComponents.mainMeleeWeaponsGrabbedManager.enableOrDisableAllMeleeWeaponMeshesOnCharacterBody (!passengerIsGettingOn);

				currentPassengerComponents.mainMeleeWeaponsGrabbedManager.enableOrDisableAllMeleeWeaponShieldMeshesOnCharacterBody (!passengerIsGettingOn);
			}
		}

		if (!passengerIsGettingOn) {
			removePassengerInfo (IKRiderInfo);
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

		if (mainInputActionManager != null && isPlayer && passengerOnDriverSeat) {
			mainInputActionManager.enableOrDisableInput (isBeingDriven, currentPassengerComponents.currentPassenger);
		}

		mainObjectToOverrideSystem.setCurrentPlayer (currentPassengerComponents.currentPassenger);

		mainObjectToOverrideSystem.setOverrideControlExternally (isBeingDriven);
	}














	//the player is getting in or off from the vehicle
	public void startOrStopVehicle (bool state)
	{
		Vector3 nextPlayerPos = getPassengerGetOffPosition ();

		if (nextPlayerPos == -Vector3.one || nextPlayerPos == Vector3.zero) {
			if (IKRiderInfo.vehicleSeatInfo.getOffPlace == IKDrivingSystem.seatInfo.getOffSide.right) {
				nextPlayerPos = IKRiderInfo.vehicleSeatInfo.rightGetOffPosition.position;
			} else {
				nextPlayerPos = IKRiderInfo.vehicleSeatInfo.leftGetOffPosition.position;
			}
		}

		if (customPassengerPosition != Vector3.zero) {
			nextPlayerPos = customPassengerPosition;

			customPassengerPosition = Vector3.zero;
		}

		passengerIsGettingOn = false;

		passengerIsGettingOn = IKRiderInfo.vehicleSeatInfo.seatIsFree;

		IKRiderInfo.vehicleSeatInfo.seatIsFree = !passengerIsGettingOn;

		IKRiderInfo.adjustPassengerPositionActive = true;
		IKRiderInfo.currentDriveIkWeightValue = 1;
		IKRiderInfo.disableIKOnPassengerSmoothly = false;

		currentPassengerComponents.playerControllerManager.setActionToGetOnVehicleActiveState (false);
		currentPassengerComponents.playerControllerManager.setActionToGetOffFromVehicleActiveState (false);

		if (showDebugPrint) {
			print ("passenger is getting on " + passengerIsGettingOn);
		}

		bool passengerOnDriverSeat = false;
		if (IKRiderInfo.vehicleSeatInfo.isDriverSeat) {
			passengerOnDriverSeat = true;
		}

		if (showDebugPrint) {
			print ("is driver seat " + passengerOnDriverSeat);
		}

		currentPassengerComponents.passengerOnDriverSeat = passengerOnDriverSeat;

		isBeingDriven = false;

		if (passengerOnDriverSeat) {
			isBeingDriven = passengerIsGettingOn;
		} else {
			isBeingDriven = false;
		}

		if (showDebugPrint) {
			print (passengerIsGettingOn + " " + passengerOnDriverSeat + " " + isBeingDriven);
		}

		bool isPlayer = false;

		if (!currentPassengerComponents.playerControllerManager.usedByAI) {
			isPlayer = true;
		}

		//set the state driving as the current state of the player
		currentPassengerComponents.playerControllerManager.setDrivingState (passengerIsGettingOn, vehicleGameObject, vehicleName);

		currentPassengerComponents.playerControllerManager.enabled = !passengerIsGettingOn;
		currentPassengerComponents.passengerPhysicallyOnVehicle = true;

		currentPassengerComponents.playerControllerManager.changeScriptState (!passengerIsGettingOn);

		currentPassengerComponents.playerControllerManager.setPlayerSetAsChildOfParentState (passengerIsGettingOn);

		currentPassengerComponents.playerControllerManager.setUsingDeviceState (passengerIsGettingOn);

		if (isPlayer) {
			currentPassengerComponents.pauseManager.usingDeviceState (passengerIsGettingOn);

			currentPassengerComponents.mainInventoryManager.enableOrDisableWeaponSlotsParentOutOfInventory (!passengerIsGettingOn);
		}

		//enable or disable the collider and the rigidbody of the player

		currentPassengerComponents.playerControllerManager.setMainColliderState (!passengerIsGettingOn);

		currentPassengerComponents.mainRigidbody.isKinematic = passengerIsGettingOn;

		//get the IK positions of the car to use them in the player
		currentPassengerComponents.IKManager.setDrivingState (passengerIsGettingOn, IKRiderInfo);

		//check if the camera in the player is in first or third view, to set the current view in the vehicle
		bool firstCameraEnabled = currentPassengerComponents.playerCameraManager.isFirstPersonActive ();

		if (isPlayer) {
			//enable and disable the player's HUD and the vehicle's HUD
			currentPassengerComponents.mainPlayerHUDManager.enableOrDisableHUD (!passengerIsGettingOn);

			if (passengerOnDriverSeat) {
				if (passengerIsGettingOn) {
					if (useEventOnDriverGetOn) {
						eventOnDriverGetOn.Invoke (currentPassengerComponents.currentPassenger);
					}
				} else {
					if (useEventOnDriverGetOff) {
						eventOnDriverGetOff.Invoke (currentPassengerComponents.currentPassenger);
					}
				}
			}
		}

		currentPassengerComponents.mainFootStepManager.enableOrDisableFootSteps (!passengerIsGettingOn);

		currentPassengerComponents.healthManager.setSliderVisibleState (!passengerIsGettingOn);

		if (passengerIsGettingOn) {
			currentPassengerComponents.playerCameraManager.playOrPauseHeadBob (!passengerIsGettingOn);
			currentPassengerComponents.playerCameraManager.stopAllHeadbobMovements ();
		} else {
			currentPassengerComponents.playerCameraManager.pauseHeadBodWithDelay (0.5f);
			currentPassengerComponents.playerCameraManager.playOrPauseHeadBob (!passengerIsGettingOn);
		}

		if (passengerIsGettingOn) {
			if (IKRiderInfo.setNewScaleOnPassenger) {
				currentPassengerComponents.currentPassenger.transform.localScale = IKRiderInfo.newScaleOnPassenger;
			}
		} else {
			if (IKRiderInfo.setNewScaleOnPassenger) {
				currentPassengerComponents.currentPassenger.transform.localScale = Vector3.one;
			}
		}

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

		currentPassengerComponents.usingDevicesManager.setUseDeviceButtonEnabledState (true);

		//if the player is driving it
		if (passengerIsGettingOn) {
			//check the current state of the player, to check if he is carrying an object, aiming, etc... to disable that state
			currentPassengerComponents.statesManager.checkPlayerStates ();

			//set the player's position and parent inside the car

			currentRiderGameObject.transform.SetParent (passengerParent.transform);
			currentRiderGameObject.transform.localPosition = Vector3.zero;
			currentRiderGameObject.transform.localRotation = Quaternion.identity;
			currentRiderGameObject.transform.position = IKRiderInfo.vehicleSeatInfo.seatTransform.position;
			currentRiderGameObject.transform.rotation = IKRiderInfo.vehicleSeatInfo.seatTransform.rotation;

			//get the vehicle camera rotation

			if (passengerOnDriverSeat) {
				//reset the player's camera rotation input values
				currentPassengerComponents.playerCameraManager.setLookAngleValue (Vector2.zero);

				//set the player's camera rotation as the same in the vehicle
				currentPassengerComponents.playerCameraGameObject.transform.rotation = transform.rotation;
			}

			if (isPlayer) {
				if (passengerOnDriverSeat) {
					currentPassengerComponents.playerCameraManager.getPivotCameraTransform ().localRotation = Quaternion.identity;

					currentPassengerComponents.playerCameraManager.resetMainCameraTransformLocalPosition ();

					currentPassengerComponents.playerCameraManager.resetPivotCameraTransformLocalPosition ();

					currentPassengerComponents.playerCameraManager.resetCurrentCameraStateAtOnce ();
				}
			}

			currentPassengerComponents.usingDevicesManager.setDrivingState (passengerIsGettingOn);

			currentPassengerComponents.usingDevicesManager.clearDeviceListButOne (vehicleGameObject);

			currentPassengerComponents.usingDevicesManager.checkIfSetOriginalShaderToPreviousDeviceFound (vehicleGameObject);

			if (IKRiderInfo.useAnimationActionIDOnDriveIdle) {
				currentPassengerComponents.mainPlayerActionSystem.setNewActionIDExternally (IKRiderInfo.animationActionID);
			}
		}

        //the player gets off from the vehicle
        else {
			currentPassengerComponents.playerControllerManager.setTemporalVehicle (null);

			if (isPlayer && passengerOnDriverSeat) {
				currentPassengerComponents.mainPlayerHUDManager.setCurrentVehicleHUD (null);
			}

			//set the parent of the player as null
			currentRiderGameObject.transform.SetParent (null);

			//set the player's position at the correct side of the car
			currentRiderGameObject.transform.position = nextPlayerPos;

			if (IKRiderInfo.useAnimationActionIDOnDriveIdle) {
				currentPassengerComponents.mainPlayerActionSystem.setNewActionIDExternally (resetAnimatorDrivingStateID);
			} else {
				currentPassengerComponents.mainPlayerActionSystem.setNewActionIDExternally (0);
			}

			//Set passenger rotation

			Vector3 targetRotation = vehicleGameObject.transform.eulerAngles;

			currentRiderGameObject.transform.eulerAngles = targetRotation;

			Vector3 currentNormal = currentPassengerComponents.playerGravityManager.getCurrentNormal ();

			Quaternion currentPlayerRotation = currentRiderGameObject.transform.rotation;
			Vector3 currentPlayerForward = Vector3.Cross (currentRiderGameObject.transform.right, currentNormal);
			Quaternion playerTargetRotation = Quaternion.LookRotation (currentPlayerForward, currentNormal);

			currentRiderGameObject.transform.rotation = playerTargetRotation;


			//set the player's camera position in the correct place

			Transform playerCameraTransform = currentPassengerComponents.playerCameraGameObject.transform;

			playerCameraTransform.position = nextPlayerPos;

			playerCameraTransform.rotation = currentRiderGameObject.transform.rotation;

			if (resetCameraRotationWhenGetOff) {
				float angleY = Vector3.Angle (vehicleGameObject.transform.forward, playerCameraTransform.forward);

				angleY *= Mathf.Sign (playerCameraTransform.InverseTransformDirection (
					Vector3.Cross (vehicleGameObject.transform.forward, playerCameraTransform.forward)).y);

				playerCameraTransform.Rotate (0, -angleY, 0);
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

			currentPassengerComponents.usingDevicesManager.setDrivingState (passengerIsGettingOn);
		}

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

		if (!passengerIsGettingOn) {
			removePassengerInfo (IKRiderInfo);
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
			
		if (mainInputActionManager != null && isPlayer && passengerOnDriverSeat) {
			mainInputActionManager.enableOrDisableInput (isBeingDriven, currentPassengerComponents.currentPassenger);
		}
	}

	public void enableOrDisablePlayerVisibleInVehicle (bool state, IKDrivingSystem.passengerComponents newPassengerComponents)
	{
		if (!newPassengerComponents.playerCameraManager.isFirstPersonActive ()) {
			newPassengerComponents.playerControllerManager.setCharacterMeshesListToDisableOnEventState (!state);

			newPassengerComponents.playerWeaponManager.enableOrDisableWeaponsMesh (!state);
		} else {
			newPassengerComponents.playerControllerManager.setCharacterMeshGameObjectState (false);
		}
	}

	public Vector3 getGetOffPosition (IKDrivingSystem.IKDrivingInformation passengerInfo)
	{
		if (passengerInfo.vehicleSeatInfo.getOffPlace == IKDrivingSystem.seatInfo.getOffSide.right) {
			return passengerInfo.vehicleSeatInfo.rightGetOffPosition.position;
		} else {
			return passengerInfo.vehicleSeatInfo.leftGetOffPosition.position;
		}
	}

	public void removePassengerInfo (IKDrivingSystem.IKDrivingInformation passengerInfo)
	{
		playerController currentPlayerController = currentPassengerComponents.playerControllerManager;

		currentPlayerController.setPlayerStatusIDValue (currentPassengerComponents.previousPlayerStatusID);

		currentPlayerController.setCurrentIdleIDValue (currentPassengerComponents.previousIdleID);

		currentPlayerController.setCurrentStrafeIDValue (currentPassengerComponents.previousStrafeID);

		currentPlayerController.updatePlayerStatusIDOnAnimator ();

		currentPlayerController.updateIdleIDOnAnimator ();

		currentPlayerController.updateStrafeIDOnAnimator ();

		currentPlayerController.setLastTimeFalling ();

		if (checkIfDriverIsOnGroundOnGetOff) {
			bool isPlayerOnGrond = currentPlayerController.checkIfPlayerOnGroundWithRaycast ();

			currentPlayerController.setCheckOnGroungPausedState (false);

			if (!isPlayerOnGrond) {
				currentPlayerController.setPlayerOnGroundState (false);
			}

			currentPlayerController.setPlayerOnGroundAnimatorStateOnOverrideOnGroundWithTime (true);

			currentPlayerController.disableOverrideOnGroundAnimatorValue ();

			currentPlayerController.setPauseResetAnimatorStateFOrGroundAnimatorState (true);

			if (isPlayerOnGrond) {
				currentPlayerController.setPlayerOnGroundState (true);

				currentPlayerController.setOnGroundAnimatorIDValue (true);
			} else {
				currentPlayerController.setOnGroundAnimatorIDValue (false);
			}
		}


		passengerInfo.vehicleSeatInfo.currentPassenger = null;
	}

	public Vector3 getPassengerGetOffPosition ()
	{
		bool isGettingOn = setPassengerInfo (IKRiderInfo, currentRiderGameObject);

		if (isGettingOn) {
			return Vector3.zero;
		}

		Transform seatTransform = IKRiderInfo.vehicleSeatInfo.seatTransform;
		Vector3 getOffPosition = Vector3.zero;
		Vector3 rayDirection = Vector3.zero;
		Vector3 passengerPosition = Vector3.zero;
		Vector3 rayPosition = Vector3.zero;
		float rayDistance = 0;

		Ray ray = new Ray ();

		bool canGetOff = false;

		RaycastHit[] hits;

		if (IKRiderInfo.vehicleSeatInfo.getOffPlace == IKDrivingSystem.seatInfo.getOffSide.right) {
			getOffPosition = IKRiderInfo.vehicleSeatInfo.rightGetOffPosition.position;
			rayDirection = seatTransform.right;
		} else {
			getOffPosition = IKRiderInfo.vehicleSeatInfo.leftGetOffPosition.position;
			rayDirection = -seatTransform.right;
		}

		rayDistance = GKC_Utils.distance (seatTransform.position, getOffPosition);

		rayPosition = getOffPosition - rayDirection * rayDistance;

		//set the ray origin at the vehicle position with a little offset set in the inspector
		ray.origin = rayPosition;
		//set the ray direction to the left

		ray.direction = rayDirection;
		//get all the colliders in that direction where the yellow sphere is placed

		hits = Physics.SphereCastAll (ray, 0.1f, rayDistance, obstacleLayer);
		//get the position where the player will be placed

		passengerPosition = getOffPosition;

		if (hits.Length == 0) {
			//any obstacle detected, so the player can get off
			canGetOff = true;
		} else {
			if (showDebugPrint) {
				if (hits.Length > 0) {
					print ("Obstacle detected at the " + IKRiderInfo.vehicleSeatInfo.getOffPlace + " side");
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

			if (IKRiderInfo.vehicleSeatInfo.getOffPlace == IKDrivingSystem.seatInfo.getOffSide.right) {
				getOffPosition = IKRiderInfo.vehicleSeatInfo.leftGetOffPosition.position;
				rayDirection = -seatTransform.right;
			} else {
				getOffPosition = IKRiderInfo.vehicleSeatInfo.rightGetOffPosition.position;
				rayDirection = seatTransform.right;
			}

			rayDistance = GKC_Utils.distance (seatTransform.position, getOffPosition);

			rayPosition = getOffPosition - rayDirection * rayDistance;

			ray.origin = rayPosition;

			ray.direction = rayDirection;

			hits = Physics.SphereCastAll (ray, 0.1f, rayDistance, obstacleLayer);

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

		if (Physics.Raycast (getOffPosition, -vehicleGameObject.transform.up, out hit, IKRiderInfo.vehicleSeatInfo.getOffDistance, obstacleLayer)) {
			Debug.DrawRay (getOffPosition, -vehicleGameObject.transform.up * hit.distance, Color.yellow);
			passengerPosition = hit.point;
		}

		return passengerPosition;
	}

	public bool setPassengerInfo (IKDrivingSystem.IKDrivingInformation passengerInfo, GameObject passenger)
	{
		bool isGettingOn = !isBeingDriven;

		passengerInfo.vehicleSeatInfo.currentPassenger = passenger;

		setPassengerComponents (passenger);

		return isGettingOn;
	}

	public bool isBeingDrivenActive ()
	{
		return isBeingDriven;
	}

	public GameObject getCurrentDriver ()
	{
		if (currentPassengerComponents != null) {
			return currentPassengerComponents.currentPassenger;
		}

		return null;
	}

	public void getRiderGameObject (GameObject newRiderGameObject)
	{
		currentRiderGameObject = newRiderGameObject;
	}

	public void setPassengerComponents (GameObject currentPassenger)
	{
		currentPassengerComponents = new IKDrivingSystem.passengerComponents ();

		currentPassengerComponents.currentPassenger = currentPassenger;

		playerComponentsManager mainPlayerComponentsManager = currentPassenger.GetComponent<playerComponentsManager> ();

		currentPassengerComponents.mainPlayerComponentsManager = mainPlayerComponentsManager;

		currentPassengerComponents.playerControllerManager = mainPlayerComponentsManager.getPlayerController ();

		currentPassengerComponents.usingDevicesManager = mainPlayerComponentsManager.getUsingDevicesSystem ();

		currentPassengerComponents.playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

		currentPassengerComponents.playerCameraGameObject = currentPassengerComponents.playerCameraManager.gameObject;

		currentPassengerComponents.playerGravityManager = mainPlayerComponentsManager.getGravitySystem ();

		currentPassengerComponents.playerWeaponManager = mainPlayerComponentsManager.getPlayerWeaponsManager ();

		currentPassengerComponents.mainMeleeWeaponsGrabbedManager = mainPlayerComponentsManager.getMeleeWeaponsGrabbedManager ();

		currentPassengerComponents.mainCamera = currentPassengerComponents.playerCameraManager.getMainCamera ();
		currentPassengerComponents.mainCameraTransform = currentPassengerComponents.mainCamera.transform;

		currentPassengerComponents.mainRigidbody = currentPassengerComponents.playerControllerManager.getRigidbody ();

		currentPassengerComponents.IKManager = mainPlayerComponentsManager.getIKSystem ();

		currentPassengerComponents.mainFootStepManager = mainPlayerComponentsManager.getFootStepManager ();

		currentPassengerComponents.healthManager = mainPlayerComponentsManager.getHealth ();

		currentPassengerComponents.statesManager = mainPlayerComponentsManager.getPlayerStatesManager ();

		currentPassengerComponents.pauseManager = mainPlayerComponentsManager.getPauseManager ();

		currentPassengerComponents.mainInventoryManager = mainPlayerComponentsManager.getInventoryManager ();

		currentPassengerComponents.mainPlayerHUDManager = mainPlayerComponentsManager.getPlayerHUDManager ();

		currentPassengerComponents.mapManager = mainPlayerComponentsManager.getMapSystem ();

		currentPassengerComponents.mainRagdollActivator = mainPlayerComponentsManager.getRagdollActivator ();

		currentPassengerComponents.carryingFireWeaponsPrevioulsy = currentPassengerComponents.playerWeaponManager.isUsingWeapons ();

		currentPassengerComponents.carryingMeleeWeaponsPreviously = currentPassengerComponents.playerControllerManager.isPlayerUsingMeleeWeapons ();

		currentPassengerComponents.mainRemoteEventSystem = mainPlayerComponentsManager.getRemoteEventSystem ();

		currentPassengerComponents.mainPlayerActionSystem = mainPlayerComponentsManager.getPlayerActionSystem ();

		currentPassengerComponents.previousPlayerStatusID = currentPassengerComponents.playerControllerManager.playerStatusID;

		currentPassengerComponents.previousIdleID = currentPassengerComponents.playerControllerManager.currentIdleID;

		currentPassengerComponents.previousStrafeID = currentPassengerComponents.playerControllerManager.currentStrafeID;

		currentPassengerComponents.playerControllerManager.setPlayerStatusIDValue (0);

		currentPassengerComponents.playerControllerManager.setCurrentIdleIDValue (0);

		currentPassengerComponents.playerControllerManager.setCurrentStrafeIDValue (0);

		currentPassengerComponents.playerControllerManager.setOriginalAirIDValue ();

		currentPassengerComponents.playerControllerManager.updatePlayerStatusIDOnAnimator ();

		currentPassengerComponents.playerControllerManager.updateIdleIDOnAnimator ();

		currentPassengerComponents.playerControllerManager.updateStrafeIDOnAnimator ();

		currentPassengerComponents.playerControllerManager.updateAirIDAnimatorID ();

		currentPassengerComponents.playerControllerManager.setOnGroundAnimatorIDValue (true);

	}

	public void findPlayerOnScene ()
	{
		if (searchPlayerOnSceneIfNotAssigned) {
			getRiderGameObject (GKC_Utils.findMainPlayerOnScene ());

			if (currentRiderGameObject != null) {
				playerAssignedProperly = true;
			}
		}
	}

	public override void setPlayerVisibleInVehicleState (bool state)
	{
		playerVisibleInVehicle = state;
	}

	public override GameObject getVehicleGameObject ()
	{
		return vehicleGameObject;
	}

	public override Transform getCustomVehicleTransform ()
	{
		if (customVehicleGameObject == null) {
			customVehicleGameObject = vehicleGameObject;
		}

		return customVehicleGameObject.transform;
	}

	public void killCharacterOnVehicle ()
	{
		if (isBeingDriven) {
			if (currentPassengerComponents != null && currentPassengerComponents.usingDevicesManager != null) {
				customPassengerPosition = passengerParent.position + passengerParent.up * 3;

				health currentHealthManager = currentPassengerComponents.healthManager;

				currentPassengerComponents.usingDevicesManager.useCurrentDevice (vehicleGameObject);
			
				currentHealthManager.killCharacter ();
			}
		}
	}

	public void ejectCharacterFromVehicle ()
	{
		if (isBeingDriven) {
			if (currentPassengerComponents != null && currentPassengerComponents.usingDevicesManager != null) {
				customPassengerPosition = passengerParent.position + passengerParent.up * 3;

				currentPassengerComponents.usingDevicesManager.useCurrentDevice (vehicleGameObject);
			}
		}
	}

	public void getOffFromVehicle ()
	{
		if (isBeingDriven) {
			if (currentPassengerComponents != null) {
				if (currentPassengerComponents.usingDevicesManager != null) {
					currentPassengerComponents.usingDevicesManager.useCurrentDevice (vehicleGameObject);

					if (showDebugPrint) {
						print ("activating get off input action");
					}
				}
			}
		}
	}

	public void inputGetOffFromVehicle ()
	{
		if (isBeingDriven) {
			getOffFromVehicle ();
		}
	}
}
