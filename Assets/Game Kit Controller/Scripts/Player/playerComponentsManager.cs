using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerComponentsManager : MonoBehaviour
{
	[Space]
	[Header ("Main Components")]
	[Space]

	public playerController mainPlayerController;

	public inventoryManager mainInventoryManager;

	public playerInputManager mainPlayerInputManager;

	public playerWeaponsManager mainPlayerWeaponsManager;

	public usingDevicesSystem mainUsingDevicesSystem;

	public footStepManager mainFootStepManager;

	public headTrack mainHeadTrack;

	public otherPowers mainOtherPowers;

	public gravitySystem mainGravitySystem;

	public grabObjects mainGrabObjects;

	public IKSystem mainIKSystem;

	public playerStatesManager mainPlayerStatesManager;

	public health mainHealth;

	public characterFactionManager mainCharacterFactionManager;

	public playerInfoPanelOnScreenSystem mainPlayerInfoPanelOnScreenSystem;

	public ragdollActivator mainRagdollActivator;

	public playerActionSystem mainPlayerActionSystem;

	public remoteEventSystem mainRemoteEventSystem;

	public damageHitReactionSystem mainDamageHitReactionSystem;

	public characterPropertiesSystem mainCharacterPropertiesSystem;

	public GKCConditionSystem mainGKCConditionSystem;

	public closeCombatSystem mainCloseCombatSystem;

	[Space]
	[Header ("Camera Components")]
	[Space]

	public playerCamera mainPlayerCamera;

	public headBob mainHeadBob;



	[Space]
	[Header ("Stats Components")]
	[Space]

	public playerExperienceSystem mainPlayerExperienceSystem;

	public currencySystem mainCurrencySystem;

	public inventoryBankUISystem mainInventoryBankUISystem;

	public vendorUISystem mainVendorUISystem;

	public playerHUDManager mainPlayerHUDManager;

	public staminaSystem mainStaminaSystem;

	public oxygenSystem mainOxygenSystem;

	public playerStatsSystem mainPlayerStatsSystem;

	public playerSkillsSystem mainPlayerSkillsSystem;

	public inventoryWeightManager mainInventoryWeightManager;

	public playerAbilitiesSystem mainPlayerAbilitiesSystem;

	[Space]
	[Header ("Character Components")]
	[Space]

	public saveGameSystem mainSaveGameSystem;

	public mapSystem mainMapSystem;

	public menuPause mainPauseManager;

	public showGameInfoHud gameInfoHudManager;

	public objectiveStationUISystem mainObjectiveStationUISystem;

	public bodyMountPointsSystem mainBodyMountPointsSystem;

	public buildPlayer mainBuildPlayer;

	public inventoryCharacterCustomizationSystem mainInventoryCharacterCustomizationSystem;

	public craftingUISystem mainCraftingUISystem;

	public craftingSystem mainCraftingSystem;

	public findObjectivesSystem mainFindObjectivesSystem;


	[Space]
	[Header ("Others Components")]
	[Space]

	public objectiveLogSystem mainObjectiveLogSystem;

	public playerTutorialSystem mainPlayerTutorialSystem;

	public playerScreenObjectivesSystem mainPlayerScreenObjectivesSystem;

	public playerLadderSystem mainPlayerLadderSystem;

	public pickUpsScreenInfo mainPickUpsScreenInfo;

	public friendListManager mainFriendListManager;

	public overrideElementControlSystem mainOverrideElementControlSystem;

	public travelStationUISystem mainTravelStationUISystem;

	public playerOptionsEditorSystem mainPlayerOptionsEditorSystem;

	public grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;

	public meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager;

	public remotePlayerNavmeshOverride mainRemotePlayerNavmeshOverride;

	[Space]
	[Header ("Secondary Components")]
	[Space]

	public jetpackSystem mainJetpackSystem;

	public flySystem mainFlySystem;

	public hideCharacterFixedPlaceSystemPlayerManagement mainHideCharacterFixedPlaceSystemPlayerManagement;

	public examineObjectSystemPlayerManagement mainExamineObjectSystemPlayerManagement;

	public puzzleSystemPlayerManagement mainPuzzleSystemPlayerManagement;

	public padlockSystemPlayerManagement mainPadlockSystemPlayerManagement;

	public grapplingHookTargetsSystem mainGrapplingHookTargetsSystem;

	public setTransparentSurfaces mainSetTransparentSurfaces;

	public weaponsAttachmentUIManager mainWeaponsAttachmentUIManager;

	public climbLedgeSystem mainClimbLedgeSystem;

	public damageInScreen mainDamageInScreen;

	public damageScreenSystem mainDamageScreenSystem;

	public dialogSystem mainDialogSystem;

	public projectilesOnCharacterBodyManager mainProjectilesOnCharacterBodyManager;

	public paragliderSystem mainParagliderSystem;

	public cameraCaptureSystem mainCameraCaptureSystem;

	public customCharacterControllerManager mainCustomCharacterControllerManager;

	public playerNavMeshSystem mainPlayerNavMeshSystem;

	public objectToAttractWithGrapplingHook mainObjectToAttractWithGrapplingHook;

	public overrideInputManager mainOverrideInputManager;

	public gravityWallRunSystem mainGravityWallRunSystem;

	public objectsStatsSystem mainObjectsStatsSystem;

	[Space]
	[Header ("External Controller Behavior Components")]
	[Space]

	public externalControllerBehavior wallRunningExternalControllerBehavior;
	public externalControllerBehavior swimExternalControllerBehavior;
	public externalControllerBehavior slideExternalControllerBehavior;
	public externalControllerBehavior wallSlideJumpExteralControllerBehavior;
	public externalControllerBehavior railExternalControllerBehavior;
	public externalControllerBehavior walkOnBalanceExternaControllerBehavior;
	public externalControllerBehavior climbRopeExternaControllerBehavior;
	public externalControllerBehavior freeClimbSystemBehavior;


	public playerExperienceSystem getPlayerExperienceSystem ()
	{
		return mainPlayerExperienceSystem;
	}

	public currencySystem getCurrencySystem ()
	{
		return mainCurrencySystem;
	}

	public vendorUISystem getVendorUISystem ()
	{
		return mainVendorUISystem;
	}

	public playerHUDManager getPlayerHUDManager ()
	{
		return mainPlayerHUDManager;
	}

	public mapSystem getMapSystem ()
	{
		return mainMapSystem;
	}

	public inventoryManager getInventoryManager ()
	{
		return mainInventoryManager;
	}

	public menuPause getPauseManager ()
	{
		return mainPauseManager;
	}

	public showGameInfoHud getGameInfoHudManager ()
	{
		return gameInfoHudManager;
	}

	public playerInputManager getPlayerInputManager ()
	{
		return mainPlayerInputManager;
	}

	public objectiveLogSystem getObjectiveLogSystem ()
	{
		return mainObjectiveLogSystem;
	}

	public playerTutorialSystem getPlayerTutorialSystem ()
	{
		return mainPlayerTutorialSystem;
	}

	public playerScreenObjectivesSystem getPlayerScreenObjectivesSystem ()
	{
		return mainPlayerScreenObjectivesSystem;
	}

	public staminaSystem getStaminaSystem ()
	{
		return mainStaminaSystem;
	}

	public playerLadderSystem getPlayerLadderSystem ()
	{
		return mainPlayerLadderSystem;
	}

	public oxygenSystem getOxygenSystem ()
	{
		return mainOxygenSystem;
	}

	public objectiveStationUISystem getObjectiveStationUISystem ()
	{
		return mainObjectiveStationUISystem;
	}

	public playerStatsSystem getPlayerStatsSystem ()
	{
		return mainPlayerStatsSystem;
	}

	public playerSkillsSystem getPlayerSkillsSystem ()
	{
		return mainPlayerSkillsSystem;
	}

	public pickUpsScreenInfo getPickUpsScreenInfo ()
	{
		return mainPickUpsScreenInfo;
	}

	public saveGameSystem getSaveGameSystem ()
	{
		return mainSaveGameSystem;
	}

	public playerWeaponsManager getPlayerWeaponsManager ()
	{
		return mainPlayerWeaponsManager;
	}

	public playerController getPlayerController ()
	{
		return mainPlayerController;
	}

	public playerCamera getPlayerCamera ()
	{
		return mainPlayerCamera;
	}

	public usingDevicesSystem getUsingDevicesSystem ()
	{
		return mainUsingDevicesSystem;
	}

	public footStepManager getFootStepManager ()
	{
		return mainFootStepManager;
	}

	public headTrack getHeadTrack ()
	{
		return mainHeadTrack;
	}

	public otherPowers getOtherPowers ()
	{
		return mainOtherPowers;
	}

	public headBob getHeadBob ()
	{
		return mainHeadBob;
	}

	public gravitySystem getGravitySystem ()
	{
		return mainGravitySystem;
	}

	public grabObjects getGrabObjects ()
	{
		return mainGrabObjects;
	}

	public IKSystem getIKSystem ()
	{
		return mainIKSystem;
	}

	public playerStatesManager getPlayerStatesManager ()
	{
		return mainPlayerStatesManager;
	}

	public  friendListManager getFriendListManager ()
	{
		return mainFriendListManager;
	}

	public health getHealth ()
	{
		return mainHealth;
	}

	public jetpackSystem getJetpackSystem ()
	{
		return mainJetpackSystem;
	}

	public characterFactionManager getCharacterFactionManager ()
	{
		return mainCharacterFactionManager;
	}

	public hideCharacterFixedPlaceSystemPlayerManagement getHideCharacterFixedPlaceSystemPlayerManagement ()
	{
		return mainHideCharacterFixedPlaceSystemPlayerManagement;
	}

	public examineObjectSystemPlayerManagement getExamineObjectSystemPlayerManagement ()
	{
		return mainExamineObjectSystemPlayerManagement;
	}

	public puzzleSystemPlayerManagement getPuzzleSystemPlayerManagement ()
	{
		return mainPuzzleSystemPlayerManagement;
	}

	public padlockSystemPlayerManagement getPadlockSystemPlayerManagement ()
	{
		return mainPadlockSystemPlayerManagement;
	}

	public overrideElementControlSystem getOverrideElementControlSystem ()
	{
		return mainOverrideElementControlSystem;
	}

	public playerInfoPanelOnScreenSystem getPlayerInfoPanelOnScreenSystem ()
	{
		return mainPlayerInfoPanelOnScreenSystem;
	}

	public inventoryWeightManager getInventoryWeightManager ()
	{
		return mainInventoryWeightManager;
	}

	public travelStationUISystem getTravelStationUISystem ()
	{
		return mainTravelStationUISystem;
	}

	public inventoryBankUISystem getInventoryBankUISystem ()
	{
		return mainInventoryBankUISystem;
	}

	public ragdollActivator getRagdollActivator ()
	{
		return mainRagdollActivator;
	}

	public playerActionSystem getPlayerActionSystem ()
	{
		return mainPlayerActionSystem;
	}

	public playerAbilitiesSystem getPlayerAbilitiesSystem ()
	{
		return mainPlayerAbilitiesSystem;
	}

	public remoteEventSystem getRemoteEventSystem ()
	{
		return mainRemoteEventSystem;
	}

	public grapplingHookTargetsSystem getGrapplingHookTargetsSystem ()
	{
		return mainGrapplingHookTargetsSystem;
	}

	public setTransparentSurfaces getSetTransparentSurfaces ()
	{
		return mainSetTransparentSurfaces;
	}

	public playerOptionsEditorSystem getPlayerOptionsEditorSystem ()
	{
		return mainPlayerOptionsEditorSystem;
	}

	public weaponsAttachmentUIManager getWeaponsAttachmentUIManager ()
	{
		return mainWeaponsAttachmentUIManager;
	}

	public grabbedObjectMeleeAttackSystem getGrabbedObjectMeleeAttackSystem ()
	{
		return mainGrabbedObjectMeleeAttackSystem;
	}

	public damageHitReactionSystem getDamageHitReactionSystem ()
	{
		return mainDamageHitReactionSystem;
	}

	public flySystem getFlySystem ()
	{
		return mainFlySystem;
	}

	public characterPropertiesSystem getCharacterPropertiesSystem ()
	{
		return mainCharacterPropertiesSystem;
	}

	public meleeWeaponsGrabbedManager getMeleeWeaponsGrabbedManager ()
	{
		return mainMeleeWeaponsGrabbedManager;
	}

	public remotePlayerNavmeshOverride getRemotePlayerNavmeshOverride ()
	{
		return mainRemotePlayerNavmeshOverride;
	}

	public climbLedgeSystem getClimbLedgeSystem ()
	{
		return mainClimbLedgeSystem;
	}

	public  damageInScreen getDamageInScreen ()
	{
		return mainDamageInScreen;
	}

	public damageScreenSystem getDamageScreenSystem ()
	{
		return mainDamageScreenSystem;
	}

	public dialogSystem getMainDialogSystem ()
	{
		return mainDialogSystem;
	}

	public projectilesOnCharacterBodyManager getProjectilesOnCharacterBodyManager ()
	{
		return mainProjectilesOnCharacterBodyManager;
	}

	public paragliderSystem getParagliderSystem ()
	{
		return mainParagliderSystem;
	}

	public GKCConditionSystem getGKCConditionSystem ()
	{
		return mainGKCConditionSystem;
	}

	public cameraCaptureSystem getCameraCaptureSystem ()
	{
		return mainCameraCaptureSystem;
	}

	public externalControllerBehavior getWallRunningExternalControllerBehavior ()
	{
		return wallRunningExternalControllerBehavior;
	}

	public externalControllerBehavior getSwimExternalControllerBehavior ()
	{
		return swimExternalControllerBehavior;
	}

	public externalControllerBehavior getSlideExternalControllerBehavior ()
	{
		return slideExternalControllerBehavior;
	}

	public customCharacterControllerManager getCustomCharacterControllerManager ()
	{
		return mainCustomCharacterControllerManager;
	}

	public bodyMountPointsSystem getBodyMountPointsSystem ()
	{
		return mainBodyMountPointsSystem;
	}

	public externalControllerBehavior getWallSlideJumpExteralControllerBehavior ()
	{
		return wallSlideJumpExteralControllerBehavior;
	}

	public buildPlayer getBuildPlayer ()
	{
		return mainBuildPlayer;
	}

	public inventoryCharacterCustomizationSystem getInventoryCharacterCustomizationSystem ()
	{
		return mainInventoryCharacterCustomizationSystem;
	}

	public externalControllerBehavior getRailExternalControllerBehavior ()
	{
		return railExternalControllerBehavior;
	}

	public externalControllerBehavior getWalkOnBalanceExternaControllerBehavior ()
	{
		return walkOnBalanceExternaControllerBehavior;
	}

	public externalControllerBehavior getClimbRopeExternaControllerBehavior ()
	{
		return climbRopeExternaControllerBehavior;
	}

	public playerNavMeshSystem getPlayerNavMeshSystem ()
	{
		return mainPlayerNavMeshSystem;
	}

	public objectToAttractWithGrapplingHook getObjectToAttractWithGrapplingHook ()
	{
		return mainObjectToAttractWithGrapplingHook;
	}

	public overrideInputManager getOverrideInputManager ()
	{
		return mainOverrideInputManager;
	}

	public gravityWallRunSystem getGravityWallRunSystem ()
	{
		return mainGravityWallRunSystem;
	}

	public externalControllerBehavior getFreeClimbExternalControllerBehavior ()
	{
		return freeClimbSystemBehavior;
	}

	public closeCombatSystem getCloseCombatSystem ()
	{
		return mainCloseCombatSystem;
	}

	public craftingUISystem getCraftingUISystem ()
	{
		return mainCraftingUISystem;
	}

	public craftingSystem getCraftingSystem ()
	{
		return mainCraftingSystem;
	}

	public objectsStatsSystem getObjectsStatsSystem ()
	{
		return mainObjectsStatsSystem;
	}

	public findObjectivesSystem getFindObjectivesSystem ()
	{
		return mainFindObjectivesSystem;
	}
}