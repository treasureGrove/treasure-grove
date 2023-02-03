using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class changePlayerStateOnTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool changePlayerStatesEnabled = true;

	public bool disableTriggerAfterEnter;

	[Space]
	[Header ("Player Mode Settings")]
	[Space]

	public bool changePlayerMode;

	public string playerModeName;

	[Space]
	[Header ("Jump Settings")]
	[Space]

	public bool setJumpEnabledState;
	public bool jumpEnabledState;

	public bool setDoubleJumpState;
	public bool doubleJumpState;

	[Space]
	[Header ("Camera Settings")]
	[Space]

	public bool setCameraViewState;
	public bool setCameraToFirstPerson;

	public bool setNewDefaultThirdPersonCameraState;
	public string newDefaultThirdPersonCameraStateName;

	public bool setActionToChangeBetweenViewsState;
	public bool actionToChangeBetweenViewsState;

	public bool setCameraStateName;
	public string cameraStateName;

	public bool resetCameraRotationState;

	[Space]
	[Header ("Input Settings")]
	[Space]

	public bool setPlayerInputActionState;
	public bool playerInputActionState;
	public string multiAxesInputName;
	public string axesInputName;

	[Space]
	[Header ("Weapon Settings")]
	[Space]

	public bool drawWeapon;
	public bool drawCurrentWeapon;
	public bool drawCertainWeapon;
	public string weaponNameToDraw;
	public bool keepWeapon;
	public bool setCarryWeaponOnLowerPositionState;
	public bool carryWeaponOnLowerPositionState;
	public bool setCanFireWeaponsState;
	public bool canFireWeaponsState;

	[Space]
	[Header ("Gravity Settings")]
	[Space]

	public bool setGravityPowerState;
	public bool gravityPowerState;

	public bool setZeroGravityModeState;
	public bool zeroGravityModeState;

	public bool setGravityDirection;
	public setGravity setGravityManager;

	[Space]
	[Header ("Free Floating Mode Settings")]
	[Space]

	public bool setFreeFloatingModeState;
	public bool freeFloatingModeState;

	[Space]
	[Header ("Look At Target Settings")]
	[Space]

	public bool setLookAtPointState;
	public bool lookAtPointState;
	public Transform pointToLook;
	public bool useDurationToLookAtPoint;
	public float durationToLookAtPoint;
	public bool enableLookAtPointStateAfterDuration;
	public bool setLookAtPointSpeed;
	public float lookAtPointSpeed;

	public bool setMaxDistanceToFindTarget;
	public float maxDistanceToFindTarget;

	public bool setUseLookTargetIconState;
	public bool useLookTargetIconState;

	public bool setOriginalUseLookTargetIconState;

	[Space]
	[Header ("Camera Zoom Settings")]
	[Space]

	public bool setCameraZoomState;
	public bool cameraZoomState;
	public bool useCameraZoomDuration;
	public float cameraZoomDuration;
	public bool enableCameraZoonInputAfterDuration;

	[Space]
	[Header ("Transparent Surface Detection Settings")]
	[Space]

	public bool setTransparentSurfaceDetectionState;
	public bool transparentSurfaceDetectionState;

	[Space]
	[Header ("Movement Settings")]
	[Space]

	public bool setWalkSpeedValue;
	public float walkSpeedValue;
	public bool setOriginalWalkSpeedValue;

	public bool setIncreaseWalkSpeedValue;
	public float increaseWalkSpeedValue;

	public bool setIncreaseWalkSpeedEnabled;
	public bool increaseWalkSpeedEnabled;
	public bool setOriginalIncreaseWalkSpeedEnabledValue;

	public bool setPlayerStatusID;
	public int playerStatusID;
	public bool setOriginalPlayerStatusID;

	[Space]
	[Header ("Sprint Settings")]
	[Space]

	public bool setSprintEnabledState;
	public bool sprintEnabledState;
	public bool setOriginalSprintEnabledState;

	[Space]
	[Header ("Stairs Settings")]
	[Space]

	public bool setStairsAdherenceValue;
	public float stairsMaxValue = 0.25f;
	public float stairsMinValue = 0.2f;
	public float stairsGroundAdherence = 8;

	[Space]
	[Header ("Root Motion Settings")]
	[Space]

	public bool changeRootMotionActive;
	public bool useRootMotionActive = true;

	[Space]
	[Header ("Ragdoll Settings")]
	[Space]

	public bool activateRagdollState;
	public Transform ragdollPushDirection;
	public float ragdollPushForceAmount;

	[Space]
	[Header ("Ability Settings")]
	[Space]

	public bool setAbility;
	public string abilityNameToSet;

	[Space]

	public bool useAbilitiesListToEnable;

	public List<string> abilitiesListToEnable = new List<string> ();

	[Space]

	public bool useAbilitiesListToDisable;

	public List<string> abilitiesListToDisable = new List<string> ();

	[Space]
	[Header ("Powers Settings")]
	[Space]

	public bool setCurrentPower;
	public string currentPowerName;

	[Space]
	[Header ("Map Settings")]
	[Space]

	public bool setCompassEnableValue;
	public bool compassEnabledValue;

	[Space]
	[Space]

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> removeEventNameList = new List<string> ();

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnTrigger;
	public UnityEvent eventOnTrigger;
	public bool sendPlayerOnEvent;
	public eventParameters.eventToCallWithGameObject playerSendEvent;

	[Space]
	[Header ("Player Manual Settings")]
	[Space]

	public bool setPlayerManually;
	public GameObject playerToConfigure;

	public GameObject currentPlayer;

	public bool searchPlayerOnSceneIfNotAssigned = true;

	playerComponentsManager mainPlayerComponentsManager;
	playerController playerControllerManager;
	playerCamera playerCameraManager;
	playerWeaponsManager weaponsManager;
	gravitySystem gravityManager;
	playerInputManager playerInput;
	Collider currentCollider;
	grabObjects grabObjectsManager;
	playerAbilitiesSystem mainPlayerAbilitySystem;
	remoteEventSystem currentRemoteEventSystem;
	otherPowers mainOtherPowers;
	playerStatesManager mainPlayerStatesManager;
	ragdollActivator mainRagdollActivator;
	mapSystem mainMapSystem;


	void Start ()
	{
		if (setPlayerManually) {
			setCurrentPlayer (playerToConfigure);
		}
	}

	public void findPlayerOnScene ()
	{
		if (searchPlayerOnSceneIfNotAssigned) {
			setCurrentPlayer (GKC_Utils.findMainPlayerOnScene ());
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
	}

	public void changePlayerState ()
	{
		if (currentPlayer == null) {

			findPlayerOnScene ();

			if (currentPlayer == null) {
				print ("WARNING: no player controller has been assigned to the change player state on trigger." +
				" Make sure to use a trigger to activate the element or assign the player manually");

				return;
			}
		}

		if (!changePlayerStatesEnabled) {
			return;
		}

		if (currentPlayer == null) {
			return;
		}

		mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();
			
		if (mainPlayerComponentsManager == null) {
			return;
		}

		//get all the components needed
		playerControllerManager = mainPlayerComponentsManager.getPlayerController ();

		if (playerControllerManager == null) {
			return;
		}

		playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

		weaponsManager = mainPlayerComponentsManager.getPlayerWeaponsManager ();

		gravityManager = mainPlayerComponentsManager.getGravitySystem ();

		playerInput = mainPlayerComponentsManager.getPlayerInputManager ();

		currentCollider = playerControllerManager.getMainCollider ();

		grabObjectsManager = mainPlayerComponentsManager.getGrabObjects ();

		mainPlayerAbilitySystem = mainPlayerComponentsManager.getPlayerAbilitiesSystem ();

		currentRemoteEventSystem = mainPlayerComponentsManager.getRemoteEventSystem ();

		mainOtherPowers = mainPlayerComponentsManager.getOtherPowers ();

		mainPlayerStatesManager = mainPlayerComponentsManager.getPlayerStatesManager ();

		mainRagdollActivator = mainPlayerComponentsManager.getRagdollActivator ();
		 
		mainMapSystem = mainPlayerComponentsManager.getMapSystem ();


		//Player Mode Settings
		if (changePlayerMode) {
			mainPlayerStatesManager.setPlayerModeByName (playerModeName);
		}

		//Jump settings
		if (setJumpEnabledState) {
			playerControllerManager.enableOrDisableJump (jumpEnabledState);
		}

		if (setDoubleJumpState) {
			playerControllerManager.enableOrDisableDoubleJump (doubleJumpState);
		}


		//Camera settings
		if (setCameraViewState) {
			playerCameraManager.changeCameraToThirdOrFirstView (setCameraToFirstPerson);
		}

		if (setActionToChangeBetweenViewsState) {
			playerCameraManager.enableOrDisableChangeCameraView (actionToChangeBetweenViewsState);
		}

		if (setCameraStateName) {
			if (!playerCameraManager.isFirstPersonActive ()) {
				playerCameraManager.setCameraState (cameraStateName);
			}
		}

		if (setNewDefaultThirdPersonCameraState) {
			playerCameraManager.setDefaultThirdPersonStateName (newDefaultThirdPersonCameraStateName);
		}

		if (resetCameraRotationState) {
			playerCameraManager.resetCameraRotation ();
		}

		//Weapon settings
		if (drawWeapon) {
			if (drawCurrentWeapon) {
				weaponsManager.checkIfDrawSingleOrDualWeapon ();
			}

			if (drawCertainWeapon) {
				weaponsManager.selectWeaponByName (weaponNameToDraw, true);
			}
		}

		if (keepWeapon) {
			weaponsManager.checkIfKeepSingleOrDualWeapon ();
		}

		if (setCarryWeaponOnLowerPositionState) {
			if (carryWeaponOnLowerPositionState) {
				weaponsManager.setCarryWeaponInLowerPositionActiveState (true);

				grabObjectsManager.enableOrDisableGeneralCursorFromExternalComponent (false);
			} else {
				weaponsManager.setCarryWeaponInLowerPositionActiveState (false);

				grabObjectsManager.enableOrDisableGeneralCursorFromExternalComponent (true);
			}
		}

		if (setCanFireWeaponsState) {
			weaponsManager.setFireWeaponsInputActiveState (canFireWeaponsState);
		}

		//Gravity settings
		if (setGravityPowerState) {
			gravityManager.setGravityPowerEnabledState (gravityPowerState);
		}

		if (setZeroGravityModeState) {
			gravityManager.setZeroGravityModeOnState (zeroGravityModeState);
		}

		if (setFreeFloatingModeState) {
			gravityManager.setfreeFloatingModeOnState (freeFloatingModeState);
		}

		if (setGravityDirection) {
			setGravityManager.checkTriggerType (currentCollider, true);
		}

		if (setPlayerInputActionState) {
			playerInput.setPlayerInputActionState (playerInputActionState, multiAxesInputName, axesInputName);
		}
			
		if (setUseLookTargetIconState) {
			playerCameraManager.setUseLookTargetIconState (useLookTargetIconState);
		}

		//Player camera look at point settings
		if (setLookAtPointState) {
			if (lookAtPointState) {
				playerCameraManager.setLookAtTargetEnabledState (true);
			}

			if (setMaxDistanceToFindTarget) {
				playerCameraManager.setMaxDistanceToFindTargetValue (maxDistanceToFindTarget);
			}

			playerCameraManager.setLookAtTargetStateManual (lookAtPointState, pointToLook);
		
			if (lookAtPointState) {
				playerCameraManager.setLookAtTargetEnabledState (false);
			}
		
			if (setLookAtPointSpeed) {
				playerCameraManager.setLookAtTargetSpeedValue (lookAtPointSpeed);
			}

			if (useDurationToLookAtPoint) {
				playerCameraManager.setLookAtTargetEnabledStateDuration (false, durationToLookAtPoint, enableLookAtPointStateAfterDuration);
			}

			if (setMaxDistanceToFindTarget) {
				playerCameraManager.setOriginalmaxDistanceToFindTargetValue ();
			}
		}

		if (setOriginalUseLookTargetIconState) {
			playerCameraManager.setOriginalUseLookTargetIconValue ();
		}

		//Camera zoom settings
		if (setCameraZoomState) {
			playerCameraManager.setZoom (cameraZoomState);

			if (useCameraZoomDuration) {
				playerCameraManager.setZoomStateDuration (false, cameraZoomDuration, enableCameraZoonInputAfterDuration);
			}
		}


		//Transparent Surface Detection Settings
		if (setTransparentSurfaceDetectionState) {
			setTransparentSurfaces currentSetTransparentSurfaces = mainPlayerComponentsManager.getSetTransparentSurfaces ();

			if (currentSetTransparentSurfaces != null) {
				currentSetTransparentSurfaces.setCheckSurfaceActiveState (transparentSurfaceDetectionState);
			}
		}


		//Sprint settings
		if (setSprintEnabledState) {
			if (setOriginalSprintEnabledState) {
				playerControllerManager.setOriginalSprintEnabledValue ();
			} else {
				if (!sprintEnabledState) {
					playerControllerManager.stopSprint ();
				}

				playerControllerManager.setSprintEnabledState (sprintEnabledState);
			}
		}


		//Movements settings
		if (setWalkSpeedValue) {
			if (setOriginalWalkSpeedValue) {
				playerControllerManager.setOriginalWalkSpeed ();
			} else {
				playerControllerManager.setWalkSpeedValue (walkSpeedValue);
			}
		}

		if (setIncreaseWalkSpeedEnabled) {
			if (setOriginalIncreaseWalkSpeedEnabledValue) {
				playerControllerManager.setOriginalIncreaseWalkSpeedEnabledValue ();
			} else {
				playerControllerManager.setIncreaseWalkSpeedEnabled (increaseWalkSpeedEnabled);
			}
		}

		if (setIncreaseWalkSpeedValue) {
			playerControllerManager.setIncreaseWalkSpeedValue (increaseWalkSpeedValue);
		}

		if (setPlayerStatusID) {
			if (setOriginalPlayerStatusID) {
				playerControllerManager.resetPlayerStatusID ();
			} else {
				playerControllerManager.setPlayerStatusIDValue (playerStatusID);
			}
		}

		//Stairs settings
		if (setStairsAdherenceValue) {
			playerControllerManager.setStairsValues (stairsMaxValue, stairsMinValue, stairsGroundAdherence);
		}


		//Root motion settings
		if (changeRootMotionActive) {
			playerControllerManager.setUseRootMotionActiveState (useRootMotionActive);
		}


		//Ragdoll settings
		if (activateRagdollState) {
			Vector3 pushDirection = transform.forward;

			if (ragdollPushDirection != null) {
				pushDirection = ragdollPushDirection.forward;
			}

			mainRagdollActivator.pushCharacter (pushDirection * ragdollPushForceAmount);
		}


		//Ability settings
		if (setAbility) {
			mainPlayerAbilitySystem.setCurrentAbilityByName (abilityNameToSet);
		}
			
		if (useAbilitiesListToEnable) {
			mainPlayerAbilitySystem.enableOrDisableAbilityGroupByName (abilitiesListToEnable, true);
		}

		if (useAbilitiesListToDisable) {
			mainPlayerAbilitySystem.enableOrDisableAbilityGroupByName (abilitiesListToDisable, false);
		}

		//Powers settings
		if (setCurrentPower) {
			mainOtherPowers.setCurrentPowerByName (currentPowerName);
		}

		//Map Settings
		if (setCompassEnableValue) {
			if (mainMapSystem != null) {
				mainMapSystem.setCompassEnabledStateAndUpdateState (compassEnabledValue);
			}
		}
			
		//Remote Event Settings
		if (useRemoteEventOnObjectsFound) {
			if (currentRemoteEventSystem != null) {
				for (int i = 0; i < removeEventNameList.Count; i++) {
					currentRemoteEventSystem.callRemoteEvent (removeEventNameList [i]);
				}
			}
		}


		//Events to be called after any state is triggred
		if (sendPlayerOnEvent) {
			playerSendEvent.Invoke (currentPlayer);
		}

		if (useEventOnTrigger) {
			eventOnTrigger.Invoke ();
		}

		if (disableTriggerAfterEnter) {
			gameObject.SetActive (false);
		}
	}
}
