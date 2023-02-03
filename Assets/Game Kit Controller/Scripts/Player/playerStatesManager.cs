using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class playerStatesManager : MonoBehaviour
{
	public Text currentPlayerModeText;
	public menuPause pauseManager;
	public List<playerMode> playersMode = new List<playerMode> ();

	public bool useDefaultPlayersMode;
	public string defaultPlayersModeName = "Weapons";
	public string currentPlayersModeName;

	public List<playerControl> playerControlList = new List<playerControl> ();
	public string defaultControlStateName = "Regular Mode";
	public string currentControlStateName;

	public GameObject playerControlModeMenu;
	public RawImage currentPlayerControlModeImage;

	public List<audioSourceInfo> audioSourceList = new List<audioSourceInfo> ();

	public bool changeModeByInputEnabled = true;

	public bool openPlayerModeMenuEnabled = true;
	[Tooltip ("If enabled players will be able to change mode, for example, they will be able to change between weapon and power mode. If disabled players will forever be in the default mode.")]
	public bool changeModeEnabled = true;
	public bool closeMenuWhenModeSelected = true;

	public bool switchBetweenPlayerControlsEnabled;

	public bool canSetRegularModeActive = true;

	public bool changePlayerControlEnabled = true;

	public bool useBlurUIPanel = true;

	public bool menuOpened;

	public List<playerStateInfo> playerStateInfoList = new List<playerStateInfo> ();

	public otherPowers powersManager;
	public grabObjects grabManager;
	public scannerSystem scannerManager;
	public playerController playerManager;
	public playerCamera playerCameraManager;
	public gravitySystem gravityManager;
	public playerWeaponsManager weaponsManager;
	public closeCombatSystem combatManager;
	public IKSystem IKSystemManager;
	public usingDevicesSystem usingDevicesManager;
	public overrideElementControlSystem overrideElementManager;
	public headBob headBobManager;
	public damageInScreen damageInScreenManager;
	public oxygenSystem mainOxygenSystem;
	public playerAbilitiesSystem mainPlayerAbilitiesSystem;

	public ragdollActivator mainRagdollActivator;
	public footStepManager mainFootStepManager;

	public upperBodyRotationSystem mainUpperBodyRotationSystem;

	public findObjectivesSystem mainFindObjectivesSystem;

	public AINavMesh mainAINavMesh;

	public UnityEvent eventToEnableComponents;
	public UnityEvent eventToDisableComponents;

	public bool useEventIfSystemDisabled;
	public UnityEvent eventIfSystemDisabled;

	int currentStateIndex = -1;

	int currentControlIndex = -1;

	public bool showElementSettings;
	public bool showEventsSettings;

	bool originalChangeModeByInputEnabledValue;

	void Start ()
	{
		if (useDefaultPlayersMode) {
			for (int i = 0; i < playersMode.Count; i++) {
				if (playersMode [i].nameMode.Equals (defaultPlayersModeName)) {
					playersMode [i].isCurrentState = true;
				} else {
					playersMode [i].isCurrentState = false;
				}
			}
		}

		for (int i = 0; i < playersMode.Count; i++) {
			if (playersMode [i].isCurrentState && playersMode [i].modeEnabled) {
				currentStateIndex = i;
			}
		}

		if (currentStateIndex == -1) {
			for (int i = 0; i < playersMode.Count; i++) {
				if (playersMode [i].modeEnabled && currentStateIndex == -1) {
					currentStateIndex = i;
				}
			}
		}

		if (playerControlModeMenu != null) {
			playerControlModeMenu.SetActive (false);
		}

		setNextPlayerMode ();

		setCurrentControlMode (defaultControlStateName);

		if (!openPlayerModeMenuEnabled) {
			checkEventOnSystemDisabled ();
		}

		originalChangeModeByInputEnabledValue = changeModeByInputEnabled;
	}

	public void checkEventOnSystemDisabled ()
	{
		if (useEventIfSystemDisabled) {
			eventIfSystemDisabled.Invoke ();
		}
	}

	public void setInitialPlayerModeByNameFromEditor (string modeName)
	{
		int modeIndex = playersMode.FindIndex (s => s.nameMode == modeName);

		if (modeIndex > -1) {
			if (playersMode [modeIndex].modeEnabled) {
				currentStateIndex = modeIndex;

				for (int i = 0; i < playersMode.Count; i++) {
					if (i == currentStateIndex) {
						playersMode [i].isCurrentState = true;

						currentPlayersModeName = playersMode [i].nameMode;
					} else {
						playersMode [i].isCurrentState = false;
					}
				}

				if (useDefaultPlayersMode) {
					defaultPlayersModeName = currentPlayersModeName;
				}

				updateComponent ();
			}
		}
	}

	public void enableOrDisablePlayerModeByNameFromEditor (string modeName, bool state)
	{
		int modeIndex = playersMode.FindIndex (s => s.nameMode == modeName);

		if (modeIndex > -1) {
			playersMode [modeIndex].modeEnabled = state;

			updateComponent ();
		}
	}

	public void enablePlayerModeByNameFromEditor (string modeName)
	{
		enableOrDisablePlayerModeByNameFromEditor (modeName, true);
	}

	public void disablePlayerModeByNameFromEditor (string modeName)
	{
		enableOrDisablePlayerModeByNameFromEditor (modeName, false);
	}

	public void openOrCloseControlMode (bool state)
	{
		if ((!playerManager.isPlayerMenuActive () || menuOpened) &&
		    ((!playerManager.driving && canSetRegularModeActive && !playerManager.isUsingDevice ()) || !canSetRegularModeActive) &&
		    playerManager.checkWeaponsState ()) {

			menuOpened = state;

			if (playerManager.driving) {
				GameObject currentVehicle = playerManager.getCurrentVehicle ();

				if (currentVehicle != null) {
					vehicleHUDManager currentVehicleHUDManager = currentVehicle.GetComponent<vehicleHUDManager> ();

					if (currentVehicleHUDManager != null) {
						currentVehicleHUDManager.getVehicleCameraController ().pauseOrPlayVehicleCamera (menuOpened);
					}
				}
			}

			playerControlModeMenu.SetActive (state);

			pauseManager.openOrClosePlayerMenu (menuOpened, playerControlModeMenu.transform, useBlurUIPanel);

			pauseManager.setIngameMenuOpenedState ("Player States Manager", menuOpened, true);

			pauseManager.enableOrDisablePlayerMenu (menuOpened, false, false);
		}
	}

	public void openOrCLoseControlModeMenuFromTouch ()
	{
		openOrCloseControlMode (!menuOpened);
	}

	public void toogleCurrentControlMode (string controlModeNameToCheck)
	{
		if (!currentControlStateName.Equals (controlModeNameToCheck)) {
			setCurrentControlMode (controlModeNameToCheck);
		} else {
			setCurrentControlMode (defaultControlStateName);
		}
	}

	public void setCurrentControlMode (string controlModeNameToCheck)
	{
		if (!currentControlStateName.Equals (controlModeNameToCheck)) {
			currentControlStateName = controlModeNameToCheck;

			for (int i = 0; i < playerControlList.Count; i++) {
				if (playerControlList [i].modeEnabled) {
					if (playerControlList [i].Name.Equals (controlModeNameToCheck)) {
						currentPlayerControlModeImage.texture = playerControlList [i].modeTexture;

						canSetRegularModeActive = !playerControlList [i].avoidToSetRegularModeWhenActive;

						playerControlList [i].activateControlModeEvent.Invoke ();

						playerControlList [i].isCurrentState = true;

						currentControlIndex = i;
					} else {
						playerControlList [i].deactivateControlModeEvent.Invoke ();
						playerControlList [i].isCurrentState = false;
					}
				}
			}

			if (closeMenuWhenModeSelected && pauseManager != null) {
				if (menuOpened) {
					openOrCloseControlMode (false);
				}
			}
		}
	}

	public void setNextPlayerControl ()
	{
		if (changePlayerControlEnabled) {

			bool indexFound = false;
			int loopsAmount = 0;

			while (!indexFound) {
				currentControlIndex++;

				if (currentControlIndex >= playerControlList.Count) {
					currentControlIndex = 0;
				}

				if (playerControlList [currentControlIndex].modeEnabled) {
					indexFound = true;
				}

				loopsAmount++;
				if (loopsAmount > playerControlList.Count * 2) {
					return;
				}
			}

			setCurrentControlMode (playerControlList [currentControlIndex].Name);
		}
	}

	public void setNextPlayerMode ()
	{
		if (!powersManager.isAimingPowerInThirdPerson () &&
		    ((!weaponsManager.isCarryingWeaponInThirdPerson () && !weaponsManager.isAimingInThirdPerson () &&
		    !weaponsManager.carryingWeaponInFirstPerson && !weaponsManager.aimingInFirstPerson) ||
		    weaponsManager.drawKeepWeaponWhenModeChanged)) {

			for (int i = 0; i < playersMode.Count; i++) {
				playerMode currentPlayerMode = playersMode [i];

				if (i == currentStateIndex) {
					if (currentPlayerModeText != null) {
						currentPlayerModeText.text = currentPlayerMode.nameMode;
					}

					currentPlayerMode.isCurrentState = true;

					currentPlayersModeName = currentPlayerMode.nameMode;
				} else {
					if (currentPlayerMode.isCurrentState) {
						
						currentPlayerMode.deactivatePlayerModeEvent.Invoke ();
					}

					currentPlayerMode.isCurrentState = false;
				}
			}

			playersMode [currentStateIndex].activatePlayerModeEvent.Invoke ();
		}
	}

	public void setModeIndex ()
	{
		if (changeModeEnabled) {

			bool indexFound = false;
			int loopsAmount = 0;

			while (!indexFound) {
				currentStateIndex++;

				if (currentStateIndex >= playersMode.Count) {
					currentStateIndex = 0;
				}

				if (playersMode [currentStateIndex].modeEnabled) {
					indexFound = true;
				}

				loopsAmount++;
				if (loopsAmount > playersMode.Count * 2) {
					return;
				}
			}

			setNextPlayerMode ();
		}
	}

	public void setPlayerModeByName (string modeName)
	{
//		print (modeName + " " + name);

		for (int i = 0; i < playersMode.Count; i++) {
			if (playersMode [i].modeEnabled) {
				if (playersMode [i].nameMode.Equals (modeName)) {
//					print (modeName + " " + playersMode [i].nameMode + " " + playersMode [i].nameMode.Equals (modeName));

					currentStateIndex = i;

					setNextPlayerMode ();

					return;
				}
			}
		}
	}

	public void updateCurrentPlayerMode ()
	{
		setNextPlayerMode ();
	}

	public void updateCurrentPlayerModeByName (string playerModeToCheck)
	{
		if (currentPlayersModeName.Equals (playerModeToCheck)) {
			setNextPlayerMode ();
		}
	}

	public string getCurrentPlayersModeName ()
	{
		return currentPlayersModeName;
	}

	//check every possible state that must not keep enabled when the player is going to make a certain action, like drive
	public void checkPlayerStates (bool disableWeaponsValue, bool disableAimModeValue, bool disableGrabModeValue, bool disableScannerModeValue,
	                               bool resetAnimatorStateValue, bool disableGravityPowerValue, bool disablePowersValue, bool disablePlayerModesValue)
	{
		//print ("disable some states");
		//disable weapons
		if (disableWeaponsValue) {
			disableWeapons ();
		}

		//disable the aim mode
		if (disableAimModeValue) {
			disableAimMode ();
		}

		//disable the grab mode of one single object
		if (disableGrabModeValue) {
			disableGrabMode ();
		}

		//disable the grab mode when the player is carrying more than one object
		if (disableScannerModeValue) {
			disableScannerMode ();
		}

		//set the iddle state in the animator
		if (resetAnimatorStateValue) {
			resetAnimatorState ();
		}

		//disable the gravity power
		if (disableGravityPowerValue) {
			disableGravityPower ();
		}

		//disable powers states
		if (disablePowersValue) {
			disablePowers ();
		}

		//set the normal mode for the player, to disable the jetpack and the sphere mode
		if (disablePlayerModesValue) {
			disablePlayerModes ();
		}
	}

	//check every possible state that must not keep enabled when the player is going to make a certain action, like drive
	public void checkPlayerStates ()
	{
		//print ("disable all states");
		//disable weapons
		disableWeapons ();

		//disable the aim mode
		disableAimMode ();

		//disable the grab mode of one single object
		disableGrabMode ();

		//disable the grab mode when the player is carrying more than one object
		disableScannerMode ();

		//set the iddle state in the animator
		resetAnimatorState ();

		//disable the gravity power
		disableGravityPower ();

		//disable powers states
		disablePowers ();

		//set the normal mode for the player, to disable the jetpack and the sphere mode
		disablePlayerModes ();
	}

	public void disableWeapons ()
	{
		weaponsManager.checkIfDisableCurrentWeapon ();

		weaponsManager.resetWeaponFiringAndAimingIfPlayerDisabled ();
	}

	public void disableAimMode ()
	{
		if (powersManager != null) {
			if (powersManager.isAimingPowerInThirdPerson ()) {
				powersManager.deactivateAimMode ();
			}
		}
	}

	public void disableGrabMode ()
	{
		if (grabManager != null) {
			if (grabManager.isGrabbedObject ()) {
				grabManager.checkToDropObjectIfNotPhysicalWeaponElseKeepWeapon ();

				grabManager.setGrabObjectsInputPausedState (false);
			}
		}
	}

	public void disableScannerMode ()
	{
		if (scannerManager != null) {
			if (scannerManager.isScannerActivated ()) {
				scannerManager.disableScanner ();
			}
		}
	}

	public void resetAnimatorState ()
	{
		playerManager.resetAnimatorState ();

		if (playerManager.isExternalControllBehaviorActive ()) {
			externalControllerBehavior currentExternalControllerBehavior = playerManager.getCurrentExternalControllerBehavior ();
		
			if (currentExternalControllerBehavior != null) {
				currentExternalControllerBehavior.disableExternalControllerState ();
			}
		}
	}

	public void disableGravityPower ()
	{
		if (gravityManager.isGravityPowerActive ()) {
			gravityManager.stopGravityPower ();
		}
	}

	public void disablePowers ()
	{
		if (playerManager.isPlayerRunning ()) {
			playerManager.stopRun ();
		}

		if (mainPlayerAbilitiesSystem != null) {
			mainPlayerAbilitiesSystem.deactivateAllAbilites ();
		}
	}

	public void disablePlayerModes ()
	{
		if (canSetRegularModeActive) {
			setCurrentControlMode (defaultControlStateName);
		}
	}

	public void setDefaultControlState ()
	{
		if (currentControlStateName != defaultControlStateName) {
			setCurrentControlMode (defaultControlStateName);
		}
	}

	public void setAllControllerComponentsState (bool state)
	{
		playerManager.getMainCollider ().enabled = state;

		playerManager.enabled = state;

		playerCameraManager.enabled = state;

		gravityManager.enabled = state;

		powersManager.enabled = state;

		playerComponentsManager mainPlayerComponentsManager = GetComponent<playerComponentsManager> ();

		mainPlayerComponentsManager.getHealth ().checkIfEnabledStateCanChange (state);

		usingDevicesManager.enabled = state;

		combatManager.enabled = state;

		if (mainRagdollActivator == null) {
			mainRagdollActivator =	mainPlayerComponentsManager.getRagdollActivator ();
		}

		if (mainRagdollActivator != null) {
			if (state) {
				mainRagdollActivator.enabled = state;
			} else {
				mainRagdollActivator.checkIfDisableRagdollActivatorComponent ();
			}
		}

		if (mainFootStepManager == null) {
			mainFootStepManager = mainPlayerComponentsManager.getFootStepManager ();
		}

		if (mainFootStepManager != null) {
			mainFootStepManager.enabled = state;
		}
			
		IKSystemManager.stopAllMovementCoroutines ();

		IKSystemManager.disableAllIKStates ();

		IKSystemManager.enabled = state;

		weaponsManager.enabled = state;

		if (mainUpperBodyRotationSystem == null) {
			mainUpperBodyRotationSystem = GetComponent<upperBodyRotationSystem> ();
		}
			
		if (mainUpperBodyRotationSystem != null) {
			mainUpperBodyRotationSystem.enabled = state;
		}

		if (mainFindObjectivesSystem == null) {
			mainFindObjectivesSystem = GetComponent<findObjectivesSystem> ();
		}

		if (mainFindObjectivesSystem != null) {
			mainFindObjectivesSystem.enabled = state;
		}

		if (mainAINavMesh == null) {
			mainAINavMesh = GetComponent<AINavMesh> ();
		}

		if (mainAINavMesh != null) {
			mainAINavMesh.enabled = state;
		}

		if (state) {
			eventToEnableComponents.Invoke ();
		} else {
			eventToDisableComponents.Invoke ();
		}
	}

	public AudioSource getAudioSourceElement (string name)
	{
		for (int i = 0; i < audioSourceList.Count; i++) {
			if (audioSourceList [i].audioSourceName.Equals (name)) {
				return audioSourceList [i].audioSource;
			}
		}

		return null;
	}

	public void disableVehicleDrivenRemotely ()
	{
		if (playerManager.isDrivingRemotely ()) {
			usingDevicesManager.useDevice ();
		}

		if (playerManager.isOverridingElement ()) {
			if (overrideElementManager != null) {
				overrideElementManager.stopOverride ();
			}
		}
	}

	public void setPlayerStateByName (string stateName)
	{
		for (int i = 0; i < playerStateInfoList.Count; i++) {
			if (playerStateInfoList [i].stateEnabled) {
				if (playerStateInfoList [i].Name.Equals (stateName)) {
					playerStateInfoList [i].stateActive = true;

					playerStateInfoList [i].eventOnState.Invoke ();
				} else {
					playerStateInfoList [i].stateActive = false;
				}
			}
		}
	}

	//CALL INPUT FUNCTIONS
	public void inputChangeMode ()
	{
		if (!changeModeByInputEnabled) {
			return;
		}

		if (canUseInput ()) {
			setModeIndex ();
		}
	}

	bool canUseInput ()
	{
		if (playerManager.driving) {
			return false;
		}

		if (playerManager.isUsingDevice ()) {
			return false;
		}

		if (playerManager.isUsingSubMenu ()) {
			return false;
		}

		if (playerManager.isPlayerMenuActive ()) {
			return false;
		}

		if (playerManager.isActionActive ()) {
			return false;
		}

		if (weaponsManager.currentWeaponIsMoving ()) {
			return false;
		}

		if (weaponsManager.weaponsAreMoving ()) {
			return false;
		}

		if (playerManager.iscloseCombatAttackInProcess ()) {
			return false;
		}

		return true;
	}

	public void inputChangePlayerControlMode ()
	{
		if (openPlayerModeMenuEnabled) {
			if (switchBetweenPlayerControlsEnabled) {
				inputSetNextPlayerControl ();
			} else {
				if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
					return;
				}

				openOrCloseControlMode (!menuOpened);
			}
		}
	}

	public void inputSetNextPlayerControl ()
	{
		if (!changePlayerControlEnabled) {
			return;
		}

		if (canUseInput ()) {

			setNextPlayerControl ();
		}
	}

	public void setOpenPlayerModeMenuEnabledState (bool state)
	{
		openPlayerModeMenuEnabled = state;
	}

	public void setChangeModeByInputEnabledState (bool state)
	{
		changeModeByInputEnabled = state;
	}

	public void setOriginalChangeModeByInputEnabledState ()
	{
		setChangeModeByInputEnabledState (originalChangeModeByInputEnabledValue);
	}

	public void setInitialPlayersModeOnStart (string newModeName)
	{
		useDefaultPlayersMode = true;
		defaultPlayersModeName = newModeName;
	}

	//set in editor mode, without game running, the camera position, starting the game in fisrt person view
	public void setFirstPersonEditor ()
	{
		//check that the player is not in this mode already and that the game is not being played
		if (!playerCameraManager.isFirstPersonActive () && !Application.isPlaying) {
			//set the parameters correctly, so there won't be issues
			gravityManager.setFirstPersonView (true);

			//disable the player's meshes
			gravityManager.setGravityArrowState (false);

			headBobManager.setFirstOrThirdMode (true);

			//put the camera in the correct position
			playerCameraManager.activateFirstPersonCameraEditor ();

			//this is the first person view, so move the camera position directly to the first person view
			playerCameraManager.resetMainCameraTransformLocalPosition ();
			playerCameraManager.resetPivotCameraTransformLocalPosition ();

			//in this mode the player hasn't to aim, so enable the grab objects function
			grabManager.setAimingState (true);

			powersManager.setUsingPowersState (true);

			if (damageInScreenManager != null) {
				damageInScreenManager.pauseOrPlayDamageInScreen (true);
			}

			weaponsManager.getPlayerWeaponsManagerComponents (true);

			weaponsManager.setWeaponsParent (true, true, false);

			headBobManager.setFirstOrThirdMode (true);

			mainOxygenSystem.setFirstPersonActiveState (true);

			updateCameraComponents ();

			print ("Player's view configured as First Person");
		}
	}

	//set in editor mode, without game running, the camera position, starting the game in third person view
	public void setThirdPersonEditor ()
	{
		//check that the player is not in this mode already and that the game is not being played
		if (playerCameraManager.isFirstPersonActive () && !Application.isPlaying) {
			//set the parameters correctly, so there won't be issues
			gravityManager.setFirstPersonView (false);

			//enable the player's meshes
			gravityManager.setGravityArrowState (true);
			headBobManager.setFirstOrThirdMode (false);

			//put the camera in the correct position
			playerCameraManager.deactivateFirstPersonCameraEditor ();
			playerCameraManager.resetMainCameraTransformLocalPosition ();
			playerCameraManager.resetPivotCameraTransformLocalPosition ();

			//set the changes in grabObjects and other powers
			grabManager.setAimingState (false);

			powersManager.keepPower ();

			powersManager.setUsingPowersState (false);

			if (damageInScreenManager != null) {
				damageInScreenManager.pauseOrPlayDamageInScreen (false);
			}

			weaponsManager.getPlayerWeaponsManagerComponents (false);

			weaponsManager.setWeaponsParent (false, true, false);

			headBobManager.setFirstOrThirdMode (false);

			mainOxygenSystem.setFirstPersonActiveState (false);

			updateCameraComponents ();

			print ("Player's view configured as Third Person");
		}
	}

	public void setInitialPlayersMode (string newModeName)
	{
		useDefaultPlayersMode = true;
		defaultPlayersModeName = newModeName;

		updateComponent ();
	}

	void updateCameraComponents ()
	{
		GKC_Utils.updateComponent (playerManager);
		GKC_Utils.updateComponent (playerCameraManager);
		GKC_Utils.updateComponent (powersManager);
		GKC_Utils.updateComponent (gravityManager);
		GKC_Utils.updateComponent (headBobManager);
		GKC_Utils.updateComponent (grabManager);
		GKC_Utils.updateComponent (damageInScreenManager);
		GKC_Utils.updateComponent (weaponsManager);
		GKC_Utils.updateComponent (mainOxygenSystem);

		GKC_Utils.updateDirtyScene ("Update Camera Values", gameObject);
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class playerMode
	{
		public string nameMode;
		public bool modeEnabled = true;
		public bool isCurrentState;
		public UnityEvent activatePlayerModeEvent;
		public UnityEvent deactivatePlayerModeEvent;
	}

	[System.Serializable]
	public class playerControl
	{
		public string Name;
		public bool modeEnabled = true;
		public bool isCurrentState;
		public UnityEvent activateControlModeEvent;
		public UnityEvent deactivateControlModeEvent;
		public Texture modeTexture;
		public bool avoidToSetRegularModeWhenActive;
	}

	[System.Serializable]
	public class playerStateInfo
	{
		public string Name;
		public bool stateEnabled = true;
		public bool stateActive;

		public UnityEvent eventOnState;
	}
}
