using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[System.Serializable]
public class featuresManager : MonoBehaviour
{
	//this script allows to enable and disable all the features in this asset, so you can configure which of them you need and which you don't
	[Header ("Player Controller Features")]
	[Tooltip ("Enable the player to perform a regular jump. ")]
	public bool enabledRegularJump;
	[Tooltip ("Enable the player to jump again while already in the air. If set to true players will be able to jump higher by pressing the jump button while already jumping.")]
	public bool doubleJump;
	[Tooltip ("Should the player receive damage if they have been in the air too long (see maxTimeInAirDamage in the PlayerController), are falling, and their velocity is higher than 15 (they are falling fast)?")]
	public bool fallDamage;
	[Tooltip ("If enabled player can hold down jump to slow down their fall. This reduces the effect of gravity on the player as they fall. If fallDamage is enabled this can have the additional effect of reducing the damage caused while falling.")]
	public bool holdJumpSlowDownFallEnabled;
	public bool lookAlwaysInCameraDirection;
	public bool lookInCameraDirectionIfLookingAtTarget;
	public bool lookOnlyIfMoving;
	public bool checkForStairAdherenceSystem;
	public bool canMoveWhileAimFirstPerson;
	public bool canMoveWhileAimThirdPerson;
	public bool canMoveWhileAimLockedCamera;
	public bool useLandMark;
	public bool canGetOnVehicles;
	public bool canDrive;
	public bool airDashEnabled;
	public bool sprintEnabled;

	[Space]
	[Header ("Player Camera Features")]
	[Space]

	public bool zoomCamera;
	public bool moveAwayCamera;
	public bool shakeCamera;
	public bool moveAwayCameraInAir;
	public bool useAccelerometer;
	public bool resetCameraRotationAfterTime;
	public bool lookAtTargetEnabled;
	public bool canActivateLookAtTargetEnabled;
	public bool canActiveLookAtTargetOnLockedCamera;
	public bool changeCameraViewEnabled;
	public bool changeCameraSideActive;

	[Space]
	[Header ("Gravity Control Features")]
	[Space]

	public bool gravityPower;
	public bool liftToSearchEnabled;
	public bool randomRotationOnAirEnabled;
	public bool preserveVelocityWhenDisableGravityPower;
	public bool startWithZeroGravityMode;
	public bool canResetRotationOnZeroGravityMode;
	public bool canAdjustToForwardSurface;
	public bool canActivateFreeFloatingMode;
	public bool changeModelColor;


	[Space]
	[Header ("Powers Features")]
	[Space]

	public bool runOnCrouchEnabled;
	public bool aimModeEnabled;
	public bool shootEnabled;
	public bool changePowersEnabled;
	public bool canFirePowersWithoutAiming;
	public bool useAimCameraOnFreeFireMode;
	public bool headLookWhenAiming;
	public bool useAimAssistInThirdPerson;
	public bool infinitePower;


	[Space]
	[Header ("Grab Object Features")]
	[Space]

	public bool grabObjectEnabled;
	public bool useCursor;
	public bool grabInFixedPosition;
	public bool changeGravityObjectsEnabled;
	public bool grabObjectsPhysicallyEnabled;
	public bool useObjectToGrabFoundShader;
	public bool enableTransparency;
	public bool canUseZoomWhileGrabbed;


	[Space]
	[Header ("Devices System Features")]
	[Space]

	public bool canUseDevices;
	public bool usePickUpAmountIfEqualToOne;
	public bool showUseDeviceIconEnabled;
	public bool useDeviceButtonEnabled;
	public bool useFixedDeviceIconPosition;
	public bool deviceOnScreenIfUseFixedIconPosition;
	public bool useDeviceFoundShader;
	public bool holdButtonToTakePickupsAround;


	[Space]
	[Header ("Close Combat System Features")]
	[Space]

	public bool combatSystemEnabled;


	[Space]
	[Header ("Foot Step System Features")]
	[Space]

	public bool soundsEnabled;
	public bool useFootPrints;
	public bool useFootParticles;


	[Space]
	[Header ("Scanner System Features")]
	[Space]

	public bool scannerSystemEnabled;


	[Space]
	[Header ("Pick Ups Info Features")]
	[Space]

	public bool pickUpScreenInfoEnabled;


	[Space]
	[Header ("Player Weapons Features")]
	[Space]

	public bool setWeaponWhenPicked;
	public bool canGrabObjectsCarryingWeapons;
	public bool changeToNextWeaponIfAmmoEmpty;
	public bool drawKeepWeaponWhenModeChanged;
	public bool canFireWeaponsWithoutAiming;
	public bool drawWeaponIfFireButtonPressed;
	public bool keepWeaponAfterDelayThirdPerson;
	public bool keepWeaponAfterDelayFirstPerson;
	public bool useQuickDrawWeapon;
	public bool useAimCameraOnFreeFireModeWeapons;
	public bool storePickedWeaponsOnInventoryWeaponSystem;
	public bool drawWeaponWhenPicked;
	public bool changeToNextWeaponWhenUnequipped;
	public bool changeToNextWeaponWhenEquipped;
	public bool notActivateWeaponsAtStart;
	public bool openWeaponAttachmentsMenuEnabled;
	public bool setFirstPersonForAttachmentEditor;
	public bool useUniversalAttachments;
	public bool canDropWeapons;
	public bool changeToNextWeaponWhenDrop;
	public bool dropCurrentWeaponWhenDie;
	public bool dropAllWeaponsWhenDie;
	public bool dropWeaponsOnlyIfUsing;
	public bool drawWeaponWhenResurrect;
	public bool canMarkTargets;
	public bool useAimAssistInThirdPersonWeapons;
	public bool useAimAssistInFirstPerson;
	public bool useAimAssistInLockedCamera;


	[Space]
	[Header ("Inventory Manager Features")]
	[Space]

	public bool inventoryEnabled;
	public bool combineElementsAtDrop;
	public bool useOnlyWhenNeededAmountToUseObject;
	public bool activeNumberOfObjectsToUseMenu;
	public bool setTotalAmountWhenDropObject;
	public bool examineObjectBeforeStoreEnabled;
	public bool storePickedWeaponsOnInventory;
	public bool useDragDropQuickAccessSlots;
	public bool equipWeaponsWhenPicked;


	[Space]
	[Header ("Jetpack System Features")]
	[Space]

	public bool jetpackEnabled;


	[Space]
	[Header ("Fly System Features")]
	[Space]

	public bool flyModeEnabled;


	[Space]
	[Header ("Damage Screen System Features")]
	[Space]

	public bool damageScreenEnabled;
	public bool showDamageDirection;
	public bool showDamagePositionWhenEnemyVisible;
	public bool showAllDamageDirections;


	[Space]
	[Header ("Damage In Screen System Features")]
	[Space]

	public bool showScreenInfoEnabled;


	[Space]
	[Header ("Friend List Manager Features")]
	[Space]

	public bool friendManagerEnabled;


	[Space]
	[Header ("Player States Manager Features")]
	[Space]

	public bool openPlayerModeMenuEnabled;
	public bool changeModeEnabled;
	public bool closeMenuWhenModeSelected;


	[Space]
	[Header ("Head Track Features")]
	[Space]

	public bool headTrackEnabled;
	public bool lookInCameraDirection;
	public bool lookInOppositeDirectionOutOfRange;
	public bool lookBehindIfMoving;


	[Space]
	[Header ("Hand On Surface IK System Features")]
	[Space]

	public bool adjustHandsToSurfacesEnabled;


	[Space]
	[Header ("IK Foot System Features")]
	[Space]

	public bool IKFootSystemEnabled;


	[Space]
	[Header ("Climb Ledge System Features")]
	[Space]

	public bool climbLedgeActive;
	public bool useHangFromLedgeIcon;
	public bool useFixedDeviceIconPositionClimbSystem;
	public bool keepWeaponsOnLedgeDetected;
	public bool drawWeaponsAfterClimbLedgeIfPreviouslyCarried;
	public bool onlyGrabLedgeIfMovingForward;
	public bool canJumpWhenHoldLedge;


	[Space]
	[Header ("Weapons List Manager Features")]
	[Space]

	public bool weaponListManagerEnabled;


	[Space]
	[Header ("Powers Manager Features")]
	[Space]

	public bool powersActive;


	[Space]
	[Header ("Map Features")]
	[Space]

	public bool mapActive;


	[Space]
	[Header ("TimeBullet Features")]
	[Space]

	public bool timeBullet;

	[Space]
	[Header ("Powers Features")]
	[Space]

	public bool abilitiesSystemEnabled;

	[Space]
	[Space]

	//this script uses parameters inside the player, the camera, the map and the character (the parent of the player)
	public GameObject pController;
	public GameObject pCamera;

	[Space]
	[Space]

	[TextArea (10, 10)] public string explanation;

	playerController playerControllerManager;
	playerCamera playerCameraManager;
	otherPowers powersManager;
	gravitySystem gravityManager;
	grabObjects grabObjectsManager;
	usingDevicesSystem usingDevicesManager;
	closeCombatSystem combatManager;
	scannerSystem scannerManager;
	pickUpsScreenInfo pickUpsScreenInfoManager;
	mapSystem mapManager;
	timeBullet timeBulletManager;
	powersListManager powerListManager;
	footStepManager footStepSystem;
	playerWeaponsManager weaponsManager;
	inventoryManager inventorySystem;
	jetpackSystem jetpackManager;
	damageInScreen damageInScreenManager;
	damageScreenSystem damageScreenManager;
	flySystem flyManager;
	friendListManager friendListSystem;
	playerStatesManager mainPlayerStatesManager;
	headTrack headTrackManager;
	handsOnSurfaceIKSystem handOnSurfaceIKManager;
	IKFootSystem IKFootManager;
	climbLedgeSystem climbLedgeManager;
	weaponListManager weaponListManager;
	playerAbilitiesSystem mainPlayerAbilitiesSystem;

	public void updateValues (bool settingConfiguration)
	{
		//search the component that has the values to enable or disable
		searchComponent ();

		//Player Controller
		if (playerControllerManager != null) {
			setBoolValue (ref playerControllerManager.enabledRegularJump, ref enabledRegularJump, !settingConfiguration);
			setBoolValue (ref playerControllerManager.enabledDoubleJump, ref doubleJump, !settingConfiguration);
			setBoolValue (ref playerControllerManager.fallDamageEnabled, ref fallDamage, !settingConfiguration);
			setBoolValue (ref playerControllerManager.holdJumpSlowDownFallEnabled, ref holdJumpSlowDownFallEnabled, !settingConfiguration);
			setBoolValue (ref playerControllerManager.lookAlwaysInCameraDirection, ref lookAlwaysInCameraDirection, !settingConfiguration);
			setBoolValue (ref playerControllerManager.lookInCameraDirectionIfLookingAtTarget, ref lookInCameraDirectionIfLookingAtTarget, !settingConfiguration);
			setBoolValue (ref playerControllerManager.lookOnlyIfMoving, ref lookOnlyIfMoving, !settingConfiguration);
			setBoolValue (ref playerControllerManager.checkForStairAdherenceSystem, ref checkForStairAdherenceSystem, !settingConfiguration);
			setBoolValue (ref playerControllerManager.canMoveWhileAimFirstPerson, ref canMoveWhileAimFirstPerson, !settingConfiguration);
			setBoolValue (ref playerControllerManager.canMoveWhileAimThirdPerson, ref canMoveWhileAimThirdPerson, !settingConfiguration);
			setBoolValue (ref playerControllerManager.canMoveWhileAimLockedCamera, ref canMoveWhileAimLockedCamera, !settingConfiguration);
			setBoolValue (ref playerControllerManager.useLandMark, ref useLandMark, !settingConfiguration);
			setBoolValue (ref playerControllerManager.canGetOnVehicles, ref canGetOnVehicles, !settingConfiguration);
			setBoolValue (ref playerControllerManager.canDrive, ref canDrive, !settingConfiguration);
			setBoolValue (ref playerControllerManager.airDashEnabled, ref airDashEnabled, !settingConfiguration);
			setBoolValue (ref playerControllerManager.sprintEnabled, ref sprintEnabled, !settingConfiguration);
			setBoolValue (ref playerControllerManager.runOnCrouchEnabled, ref runOnCrouchEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Player Controller script hasn't been found");
		}

		//Player Camera
		if (playerCameraManager != null) {
			setBoolValue (ref playerCameraManager.settings.zoomEnabled, ref zoomCamera, !settingConfiguration);
			setBoolValue (ref playerCameraManager.settings.moveAwayCameraEnabled, ref moveAwayCamera, !settingConfiguration);
			setBoolValue (ref playerCameraManager.settings.enableShakeCamera, ref shakeCamera, !settingConfiguration);
			setBoolValue (ref playerCameraManager.settings.enableMoveAwayInAir, ref moveAwayCameraInAir, !settingConfiguration);
			setBoolValue (ref playerCameraManager.settings.useAcelerometer, ref useAccelerometer, !settingConfiguration);
			setBoolValue (ref playerCameraManager.resetCameraRotationAfterTime, ref resetCameraRotationAfterTime, !settingConfiguration);
			setBoolValue (ref playerCameraManager.lookAtTargetEnabled, ref lookAtTargetEnabled, !settingConfiguration);
			setBoolValue (ref playerCameraManager.lookAtTargetEnabled, ref canActivateLookAtTargetEnabled, !settingConfiguration);
			setBoolValue (ref playerCameraManager.canActiveLookAtTargetOnLockedCamera, ref canActiveLookAtTargetOnLockedCamera, !settingConfiguration);
			setBoolValue (ref playerCameraManager.changeCameraViewEnabled, ref changeCameraViewEnabled, !settingConfiguration);
			setBoolValue (ref playerCameraManager.changeCameraSideActive, ref changeCameraSideActive, !settingConfiguration);
		} else {
			print ("WARNING: Player Camera script hasn't been found");
		}

		//Gravity System
		if (gravityManager != null) {
			setBoolValue (ref gravityManager.gravityPowerEnabled, ref gravityPower, !settingConfiguration);
			setBoolValue (ref gravityManager.liftToSearchEnabled, ref liftToSearchEnabled, !settingConfiguration);
			setBoolValue (ref gravityManager.randomRotationOnAirEnabled, ref randomRotationOnAirEnabled, !settingConfiguration);
			setBoolValue (ref gravityManager.preserveVelocityWhenDisableGravityPower, ref preserveVelocityWhenDisableGravityPower, !settingConfiguration);
			setBoolValue (ref gravityManager.startWithZeroGravityMode, ref startWithZeroGravityMode, !settingConfiguration);
			setBoolValue (ref gravityManager.canResetRotationOnZeroGravityMode, ref canResetRotationOnZeroGravityMode, !settingConfiguration);
			setBoolValue (ref gravityManager.canAdjustToForwardSurface, ref canAdjustToForwardSurface, !settingConfiguration);
			setBoolValue (ref gravityManager.canActivateFreeFloatingMode, ref canActivateFreeFloatingMode, !settingConfiguration);
			setBoolValue (ref gravityManager.changeModelColor, ref changeModelColor, !settingConfiguration);
		} else {
			print ("WARNING: Gravity System script hasn't been found");
		}

		//Powers
		if (powersManager != null) {
			setBoolValue (ref powersManager.settings.aimModeEnabled, ref aimModeEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.settings.shootEnabled, ref shootEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.settings.changePowersEnabled, ref changePowersEnabled, !settingConfiguration);
			setBoolValue (ref powersManager.canFirePowersWithoutAiming, ref canFirePowersWithoutAiming, !settingConfiguration);
			setBoolValue (ref powersManager.useAimCameraOnFreeFireMode, ref useAimCameraOnFreeFireMode, !settingConfiguration);

			setBoolValue (ref powersManager.headLookWhenAiming, ref headLookWhenAiming, !settingConfiguration);
			setBoolValue (ref powersManager.useAimAssistInThirdPerson, ref useAimAssistInThirdPerson, !settingConfiguration);
			setBoolValue (ref powersManager.infinitePower, ref infinitePower, !settingConfiguration);
		} else {
			print ("WARNING: Other Powers script hasn't been found");
		}

		//Grab Objects
		if (grabObjectsManager != null) {
			setBoolValue (ref grabObjectsManager.grabObjectsEnabled, ref grabObjectEnabled, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.useCursor, ref useCursor, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.grabInFixedPosition, ref grabInFixedPosition, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.changeGravityObjectsEnabled, ref changeGravityObjectsEnabled, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.grabObjectsPhysicallyEnabled, ref grabObjectsPhysicallyEnabled, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.useObjectToGrabFoundShader, ref useObjectToGrabFoundShader, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.enableTransparency, ref enableTransparency, !settingConfiguration);
			setBoolValue (ref grabObjectsManager.canUseZoomWhileGrabbed, ref canUseZoomWhileGrabbed, !settingConfiguration);
		} else {
			print ("WARNING: Grab Objects script hasn't been found");
		}

		//Using Devices System
		if (usingDevicesManager != null) {
			setBoolValue (ref usingDevicesManager.canUseDevices, ref canUseDevices, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.usePickUpAmountIfEqualToOne, ref usePickUpAmountIfEqualToOne, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.showUseDeviceIconEnabled, ref showUseDeviceIconEnabled, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.useDeviceButtonEnabled, ref useDeviceButtonEnabled, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.useFixedDeviceIconPosition, ref useFixedDeviceIconPosition, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.deviceOnScreenIfUseFixedIconPosition, ref deviceOnScreenIfUseFixedIconPosition, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.useDeviceFoundShader, ref useDeviceFoundShader, !settingConfiguration);
			setBoolValue (ref usingDevicesManager.holdButtonToTakePickupsAround, ref holdButtonToTakePickupsAround, !settingConfiguration);
		} else {
			print ("WARNING: Using Devices System script hasn't been found");
		}

		//Close Combat System
		if (combatManager != null) {
			setBoolValue (ref combatManager.combatSystemEnabled, ref combatSystemEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Close Combat System script hasn't been found");
		}

		//Foot step System
		if (footStepSystem != null) {
			setBoolValue (ref footStepSystem.soundsEnabled, ref soundsEnabled, !settingConfiguration);
			setBoolValue (ref footStepSystem.useFootPrints, ref useFootPrints, !settingConfiguration);
			setBoolValue (ref footStepSystem.useFootParticles, ref useFootParticles, !settingConfiguration);
		} else {
			print ("WARNING: Foot Step Manager script hasn't been found");
		}

		//Scanner System
		if (scannerManager != null) {
			setBoolValue (ref scannerManager.scannerSystemEnabled, ref scannerSystemEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Scanner System script hasn't been found");
		}

		//Pick Ups Screen Info
		if (pickUpsScreenInfoManager != null) {
			setBoolValue (ref pickUpsScreenInfoManager.pickUpScreenInfoEnabled, ref pickUpScreenInfoEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Pickup Screen Info System script hasn't been found");
		}

		//Player Weapons System
		if (weaponsManager != null) {
			setBoolValue (ref weaponsManager.setWeaponWhenPicked, ref setWeaponWhenPicked, !settingConfiguration);
			setBoolValue (ref weaponsManager.canGrabObjectsCarryingWeapons, ref canGrabObjectsCarryingWeapons, !settingConfiguration);
			setBoolValue (ref weaponsManager.changeToNextWeaponIfAmmoEmpty, ref changeToNextWeaponIfAmmoEmpty, !settingConfiguration);
			setBoolValue (ref weaponsManager.drawKeepWeaponWhenModeChanged, ref drawKeepWeaponWhenModeChanged, !settingConfiguration);
			setBoolValue (ref weaponsManager.canFireWeaponsWithoutAiming, ref canFireWeaponsWithoutAiming, !settingConfiguration);
			setBoolValue (ref weaponsManager.drawWeaponIfFireButtonPressed, ref drawWeaponIfFireButtonPressed, !settingConfiguration);
			setBoolValue (ref weaponsManager.keepWeaponAfterDelayThirdPerson, ref keepWeaponAfterDelayThirdPerson, !settingConfiguration);
			setBoolValue (ref weaponsManager.keepWeaponAfterDelayFirstPerson, ref keepWeaponAfterDelayFirstPerson, !settingConfiguration);
			setBoolValue (ref weaponsManager.useQuickDrawWeapon, ref useQuickDrawWeapon, !settingConfiguration);
			setBoolValue (ref weaponsManager.useAimCameraOnFreeFireMode, ref useAimCameraOnFreeFireModeWeapons, !settingConfiguration);
			setBoolValue (ref weaponsManager.storePickedWeaponsOnInventory, ref storePickedWeaponsOnInventoryWeaponSystem, !settingConfiguration);
			setBoolValue (ref weaponsManager.drawWeaponWhenPicked, ref drawWeaponWhenPicked, !settingConfiguration);
			setBoolValue (ref weaponsManager.changeToNextWeaponWhenUnequipped, ref changeToNextWeaponWhenUnequipped, !settingConfiguration);
			setBoolValue (ref weaponsManager.changeToNextWeaponWhenEquipped, ref changeToNextWeaponWhenEquipped, !settingConfiguration);
			setBoolValue (ref weaponsManager.notActivateWeaponsAtStart, ref notActivateWeaponsAtStart, !settingConfiguration);
			setBoolValue (ref weaponsManager.openWeaponAttachmentsMenuEnabled, ref openWeaponAttachmentsMenuEnabled, !settingConfiguration);
			setBoolValue (ref weaponsManager.setFirstPersonForAttachmentEditor, ref setFirstPersonForAttachmentEditor, !settingConfiguration);
			setBoolValue (ref weaponsManager.useUniversalAttachments, ref useUniversalAttachments, !settingConfiguration);
			setBoolValue (ref weaponsManager.canDropWeapons, ref canDropWeapons, !settingConfiguration);
			setBoolValue (ref weaponsManager.changeToNextWeaponWhenDrop, ref changeToNextWeaponWhenDrop, !settingConfiguration);
			setBoolValue (ref weaponsManager.dropCurrentWeaponWhenDie, ref dropCurrentWeaponWhenDie, !settingConfiguration);
			setBoolValue (ref weaponsManager.dropAllWeaponsWhenDie, ref dropAllWeaponsWhenDie, !settingConfiguration);
			setBoolValue (ref weaponsManager.dropWeaponsOnlyIfUsing, ref dropWeaponsOnlyIfUsing, !settingConfiguration);
			setBoolValue (ref weaponsManager.drawWeaponWhenResurrect, ref drawWeaponWhenResurrect, !settingConfiguration);
			setBoolValue (ref weaponsManager.canMarkTargets, ref canMarkTargets, !settingConfiguration);
			setBoolValue (ref weaponsManager.useAimAssistInThirdPerson, ref useAimAssistInThirdPersonWeapons, !settingConfiguration);
			setBoolValue (ref weaponsManager.useAimAssistInFirstPerson, ref useAimAssistInFirstPerson, !settingConfiguration);
			setBoolValue (ref weaponsManager.useAimAssistInLockedCamera, ref useAimAssistInLockedCamera, !settingConfiguration);
		} else {
			print ("WARNING: Player Weapons Manager script hasn't been found");
		}

		//Player Inventory settings
		if (inventorySystem != null) {
			setBoolValue (ref inventorySystem.inventoryEnabled, ref inventoryEnabled, !settingConfiguration);
			setBoolValue (ref inventorySystem.combineElementsAtDrop, ref combineElementsAtDrop, !settingConfiguration);
			setBoolValue (ref inventorySystem.useOnlyWhenNeededAmountToUseObject, ref useOnlyWhenNeededAmountToUseObject, !settingConfiguration);
			setBoolValue (ref inventorySystem.activeNumberOfObjectsToUseMenu, ref activeNumberOfObjectsToUseMenu, !settingConfiguration);
			setBoolValue (ref inventorySystem.setTotalAmountWhenDropObject, ref setTotalAmountWhenDropObject, !settingConfiguration);
			setBoolValue (ref inventorySystem.examineObjectBeforeStoreEnabled, ref examineObjectBeforeStoreEnabled, !settingConfiguration);
			setBoolValue (ref inventorySystem.storePickedWeaponsOnInventory, ref storePickedWeaponsOnInventory, !settingConfiguration);
			setBoolValue (ref inventorySystem.mainInventoryQuickAccessSlotsSystem.useDragDropInventorySlots, ref useDragDropQuickAccessSlots, !settingConfiguration);
			setBoolValue (ref inventorySystem.equipWeaponsWhenPicked, ref equipWeaponsWhenPicked, !settingConfiguration);
		} else {
			print ("WARNING: Inventory Manager script hasn't been found");
		}

		//Jetpack System settings
		if (jetpackManager != null) {
			setBoolValue (ref jetpackManager.jetpackEnabled, ref jetpackEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Jetpack System script hasn't been found");
		}

		//Fly System settings
		if (flyManager != null) {
			setBoolValue (ref flyManager.flyModeEnabled, ref flyModeEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Fly System script hasn't been found");
		}

		//Damage Screen System settings
		if (damageScreenManager != null) {
			setBoolValue (ref damageScreenManager.damageScreenEnabled, ref damageScreenEnabled, !settingConfiguration);
			setBoolValue (ref damageScreenManager.showDamageDirection, ref showDamageDirection, !settingConfiguration);
			setBoolValue (ref damageScreenManager.showDamagePositionWhenEnemyVisible, ref showDamagePositionWhenEnemyVisible, !settingConfiguration);
			setBoolValue (ref damageScreenManager.showAllDamageDirections, ref showAllDamageDirections, !settingConfiguration);
		} else {
			print ("WARNING: Damage Screen System script hasn't been found");
		}
	
		//Damage In Screen Info settings
		if (damageInScreenManager != null) {
			setBoolValue (ref damageInScreenManager.showScreenInfoEnabled, ref showScreenInfoEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Damage In Screen script hasn't been found");
		}

		//Damage In Screen Info settings
		if (friendListSystem != null) {
			setBoolValue (ref friendListSystem.friendManagerEnabled, ref friendManagerEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Friend List Manager script hasn't been found");
		}

		//Player States Manage settings
		if (mainPlayerStatesManager != null) {
			setBoolValue (ref mainPlayerStatesManager.openPlayerModeMenuEnabled, ref openPlayerModeMenuEnabled, !settingConfiguration);
			setBoolValue (ref mainPlayerStatesManager.changeModeEnabled, ref changeModeEnabled, !settingConfiguration);
			setBoolValue (ref mainPlayerStatesManager.closeMenuWhenModeSelected, ref closeMenuWhenModeSelected, !settingConfiguration);
		} else {
			print ("WARNING: Player States Manager script hasn't been found");
		}

		//Head Track Manage settings
		if (headTrackManager != null) {
			setBoolValue (ref headTrackManager.headTrackEnabled, ref headTrackEnabled, !settingConfiguration);
			setBoolValue (ref headTrackManager.lookInCameraDirection, ref lookInCameraDirection, !settingConfiguration);
			setBoolValue (ref headTrackManager.lookInOppositeDirectionOutOfRange, ref lookInOppositeDirectionOutOfRange, !settingConfiguration);
			setBoolValue (ref headTrackManager.lookBehindIfMoving, ref lookBehindIfMoving, !settingConfiguration);
		} else {
			print ("WARNING: Head Track script hasn't been found");
		}

		//Hand On Surface IK System settings
		if (handOnSurfaceIKManager != null) {
			setBoolValue (ref handOnSurfaceIKManager.adjustHandsToSurfacesEnabled, ref adjustHandsToSurfacesEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Hands On Surface IK System script hasn't been found");
		}

		//IK Foot System settings
		if (IKFootManager != null) {
			setBoolValue (ref IKFootManager.IKFootSystemEnabled, ref IKFootSystemEnabled, !settingConfiguration);
		} else {
			print ("WARNING: IK Foot System script hasn't been found");
		}

		//Climb Ledge System settings
		if (climbLedgeManager != null) {
			setBoolValue (ref climbLedgeManager.climbLedgeActive, ref climbLedgeActive, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.useHangFromLedgeIcon, ref useHangFromLedgeIcon, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.useFixedDeviceIconPosition, ref useFixedDeviceIconPositionClimbSystem, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.keepWeaponsOnLedgeDetected, ref keepWeaponsOnLedgeDetected, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.drawWeaponsAfterClimbLedgeIfPreviouslyCarried, ref drawWeaponsAfterClimbLedgeIfPreviouslyCarried, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.onlyGrabLedgeIfMovingForward, ref onlyGrabLedgeIfMovingForward, !settingConfiguration);
			setBoolValue (ref climbLedgeManager.canJumpWhenHoldLedge, ref canJumpWhenHoldLedge, !settingConfiguration);
		} else {
			print ("WARNING: Climb Ledge System script hasn't been found");
		}

		//Wepons List Manager settings
		if (weaponListManager != null) {
			setBoolValue (ref weaponListManager.weaponListManagerEnabled, ref weaponListManagerEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Weapons List Manager script hasn't been found");
		}

		//Map settings
		if (mapManager != null) {
			setBoolValue (ref mapManager.mapEnabled, ref mapActive, !settingConfiguration);
		} else {
			print ("WARNING: Map Manager script hasn't been found");
		}

		//Time Bullet settings
		if (timeBulletManager != null) {
			setBoolValue (ref timeBulletManager.timeBulletEnabled, ref timeBullet, !settingConfiguration);
		} else {
			print ("WARNING: Time Bullet script hasn't been found");
		}

		//Power List Manager settings
		if (powerListManager != null) {
			setBoolValue (ref powerListManager.powerListManagerEnabled, ref powersActive, !settingConfiguration);
		} else {
			print ("WARNING: Powers List Manager script hasn't been found");
		}

		if (mainPlayerAbilitiesSystem != null) {
			setBoolValue (ref mainPlayerAbilitiesSystem.abilitiesSystemEnabled, ref abilitiesSystemEnabled, !settingConfiguration);
		} else {
			print ("WARNING: Player Abilities System script hasn't been found");
		}

		//upload every change object in the editor
		updateComponents ();
	}

	public void setBoolValue (ref bool rightValue, ref bool leftValue, bool assignRightValueToLeftValue)
	{
		if (assignRightValueToLeftValue) {
			leftValue = rightValue;
		} else {
			rightValue = leftValue;
		}
	}

	//set the options that the user has configured in the inspector
	public void setConfiguration ()
	{
		updateValues (true);
	}

	//get the current values of the features, to check the if the booleans fields are correct or not
	public void getConfiguration ()
	{
		updateValues (false);
	}

	public void updateComponents ()
	{
		searchComponent ();

		GKC_Utils.updateComponent (playerControllerManager);
		GKC_Utils.updateComponent (playerCameraManager);
		GKC_Utils.updateComponent (powersManager);
		GKC_Utils.updateComponent (gravityManager);
		GKC_Utils.updateComponent (grabObjectsManager);
		GKC_Utils.updateComponent (usingDevicesManager);
		GKC_Utils.updateComponent (combatManager);
		GKC_Utils.updateComponent (scannerManager);
		GKC_Utils.updateComponent (pickUpsScreenInfoManager);
		GKC_Utils.updateComponent (mapManager);
		GKC_Utils.updateComponent (timeBulletManager);
		GKC_Utils.updateComponent (powerListManager);
		GKC_Utils.updateComponent (footStepSystem);
		GKC_Utils.updateComponent (weaponsManager);
		GKC_Utils.updateComponent (inventorySystem);
		GKC_Utils.updateComponent (jetpackManager);
		GKC_Utils.updateComponent (damageInScreenManager);
		GKC_Utils.updateComponent (damageScreenManager);
		GKC_Utils.updateComponent (flyManager);
		GKC_Utils.updateComponent (friendListSystem);
		GKC_Utils.updateComponent (mainPlayerStatesManager);
		GKC_Utils.updateComponent (headTrackManager);
		GKC_Utils.updateComponent (handOnSurfaceIKManager);
		GKC_Utils.updateComponent (IKFootManager);
		GKC_Utils.updateComponent (climbLedgeManager);
		GKC_Utils.updateComponent (weaponListManager);
		GKC_Utils.updateComponent (mainPlayerAbilitiesSystem);

		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Features Manager Values", gameObject);
	}

	void searchComponent ()
	{
		playerComponentsManager mainPlayerComponentsManager = pController.GetComponent<playerComponentsManager> ();

		playerControllerManager = mainPlayerComponentsManager.getPlayerController ();

		playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

		gravityManager = mainPlayerComponentsManager.getGravitySystem ();

		powersManager = mainPlayerComponentsManager.getOtherPowers ();

		grabObjectsManager = mainPlayerComponentsManager.getGrabObjects ();

		usingDevicesManager = mainPlayerComponentsManager.getUsingDevicesSystem ();

		combatManager = GetComponentInChildren<closeCombatSystem> ();

		scannerManager = GetComponentInChildren<scannerSystem> ();

		pickUpsScreenInfoManager = mainPlayerComponentsManager.getPickUpsScreenInfo ();

		timeBulletManager = GetComponent<timeBullet> ();

		mapManager = mainPlayerComponentsManager.getMapSystem ();

		powerListManager = GetComponent<powersListManager> ();

		footStepSystem = mainPlayerComponentsManager.getFootStepManager ();

		weaponsManager = mainPlayerComponentsManager.getPlayerWeaponsManager ();

		inventorySystem = mainPlayerComponentsManager.getInventoryManager ();

		jetpackManager = mainPlayerComponentsManager.getJetpackSystem ();

		damageInScreenManager = mainPlayerComponentsManager.getDamageInScreen ();

		damageScreenManager = mainPlayerComponentsManager.getDamageScreenSystem ();

		flyManager = mainPlayerComponentsManager.getFlySystem ();

		friendListSystem = mainPlayerComponentsManager.getFriendListManager ();

		mainPlayerStatesManager = mainPlayerComponentsManager.getPlayerStatesManager ();

		headTrackManager = mainPlayerComponentsManager.getHeadTrack ();

		handOnSurfaceIKManager = GetComponentInChildren<handsOnSurfaceIKSystem> ();

		IKFootManager = GetComponentInChildren<IKFootSystem> ();

		climbLedgeManager = mainPlayerComponentsManager.getClimbLedgeSystem ();

		weaponListManager = GetComponent<weaponListManager> ();

		mainPlayerAbilitiesSystem = mainPlayerComponentsManager.getPlayerAbilitiesSystem ();
	}
}
#endif