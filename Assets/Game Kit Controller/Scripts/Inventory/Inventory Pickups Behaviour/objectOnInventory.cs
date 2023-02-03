using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectOnInventory : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public inventoryObject mainInventoryObject;

	public bool useOnlyAmountNeeded;

	public bool closeInventoryOnObjectUsed;

	[Space]
	[Header ("Condition Settings")]
	[Space]

	public bool checkConditionsToUseObjectEnabled;

	public bool playerOnGroundToUseObject;

	public bool actionSystemNotPlayingAnimations;

	public bool useCustomMessageOnConditionFailed;
	[TextArea (3, 10)] public string customMessageOnConditionFailed;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEvent;

	[Space]

	public List<string> remoteEventList = new List<string> ();

	[Space]
	[Header ("Remote Events On Use Object Settings")]
	[Space]

	public bool useRemoteEventOnUseObject;

	[Space]

	public List<string> remoteEventListOnUseObject = new List<string> ();

	[Space]
	[Header ("Remote Events On Equip Object Settings")]
	[Space]

	public bool useRemoteEventOnEquipObject;

	[Space]

	public List<string> remoteEventListOnEquipObject = new List<string> ();

	[Space]
	[Space]

	public bool useRemoteEventOnUnequipObject;

	[Space]

	public List<string> remoteEventListOnUnquipObject = new List<string> ();

	[Space]
	[Header ("Enable Abilities on Use Inventory Object Settings")]
	[Space]

	public bool useAbilitiesListToEnableOnUseInventoryObject;

	[Space]

	public List<string> abilitiesListToEnableOnUseInventoryObject = new List<string> ();

	[Space]
	[Header ("Activate Abilities on Use Inventory Object Settings")]
	[Space]

	public bool activateAbilityOnUseInventoryObject;

	[Space]

	public string abilityNameToActiveOnUseInventoryObject;
	public bool abilityIsTemporallyActivated;

	public bool checkIfAbilityIsNotActiveOrOnCoolDown;

	[Space]
	[Header ("Stats To Increase Settings")]
	[Space]

	public bool increaseStatsValues;
	public List<objectExperienceSystem.statInfo> statsToIncreaseInfoList = new List<objectExperienceSystem.statInfo> ();

	[Space]
	[Header ("Crafting Settings")]
	[Space]

	public bool getCraftingRecipes;
	public List<string> craftingRecipesList = new List<string> ();


	public virtual void activateUseObjectActionOnInventory (GameObject currentPlayer, int amountToUse)
	{
		
	}

	public virtual void activateCombineObjectActionOnInventory (GameObject currentPlayer, inventoryInfo inventoryInfoToUse)
	{

	}

	public virtual void carryPhysicalObjectFromInventory (GameObject currentPlayer)
	{

	}

	public virtual void eventOnPickObject (GameObject currentPlayer)
	{

	}

	public virtual void eventOnDropObject (GameObject currentPlayer)
	{

	}

	public virtual void checkRemoteEvents (GameObject currentPlayer)
	{
		if (useRemoteEvent) {
			if (remoteEventList.Count == 0) {
				return;
			}

			playerComponentsManager currentPlayerComponetsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponetsManager != null) {
				remoteEventSystem currentRemoteEventSystem = currentPlayerComponetsManager.getRemoteEventSystem ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventList.Count; i++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventList [i]);
					}
				}
			}
		}
	}

	public virtual void checkRemoteEventsOnUseObject (GameObject currentPlayer)
	{
		if (useRemoteEventOnUseObject) {
			if (remoteEventListOnUseObject.Count == 0) {
				return;
			}

			playerComponentsManager currentPlayerComponetsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponetsManager != null) {
				remoteEventSystem currentRemoteEventSystem = currentPlayerComponetsManager.getRemoteEventSystem ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventListOnUseObject.Count; i++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventListOnUseObject [i]);
					}
				}
			}
		}
	}

	public virtual void checkRemoteEventsOnSetObjectEquipState (GameObject currentPlayer, bool state)
	{
		bool checkEvents = false;

		if (useRemoteEventOnEquipObject && state) {
			if (remoteEventListOnEquipObject.Count > 0) {
				checkEvents = true;
			}

		}

		if (useRemoteEventOnUnequipObject && !state) {
			if (remoteEventListOnUnquipObject.Count > 0) {
				checkEvents = true;
			}
		}

		if (checkEvents) {
			playerComponentsManager currentPlayerComponetsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponetsManager != null) {
				remoteEventSystem currentRemoteEventSystem = currentPlayerComponetsManager.getRemoteEventSystem ();

				if (currentRemoteEventSystem != null) {
					if (state) {
						if (useRemoteEventOnEquipObject) {
							for (int i = 0; i < remoteEventListOnEquipObject.Count; i++) {
								currentRemoteEventSystem.callRemoteEvent (remoteEventListOnEquipObject [i]);
							}
						} 
					} else {
						if (useRemoteEventOnUnequipObject) {
							for (int i = 0; i < remoteEventListOnUnquipObject.Count; i++) {
								currentRemoteEventSystem.callRemoteEvent (remoteEventListOnUnquipObject [i]);
							}
						}
					}
				}
			}
		}
	}


	public virtual bool setObjectEquippedStateOnInventory (GameObject currentPlayer, bool state)
	{

		return false;
	}

	//	public virtual bool isObjectEquipped ()
	//	{
	//
	//		return false;
	//	}
	//
	//	public virtual void updateObjectState ()
	//	{
	//
	//	}

	public virtual void checkIfEnableAbilitiesOnUseInventoryObject (GameObject currentPlayer)
	{
		if (useAbilitiesListToEnableOnUseInventoryObject && currentPlayer != null) {
			GKC_Utils.enableOrDisableAbilityGroupByName (currentPlayer.transform, true, abilitiesListToEnableOnUseInventoryObject);
		}
	}

	public virtual void checkIfActivateAbilitiesOnUseInventoryObject (GameObject currentPlayer)
	{
		if (activateAbilityOnUseInventoryObject && currentPlayer != null) {
			GKC_Utils.activateAbilityByName (currentPlayer.transform, abilityNameToActiveOnUseInventoryObject, abilityIsTemporallyActivated);
		}
	}

	public virtual bool checkIfAbilitiesOnUseOrCooldown (GameObject currentPlayer)
	{
		if (activateAbilityOnUseInventoryObject && currentPlayer != null) {
			return GKC_Utils.checkIfAbilitiesOnUseOrCooldown (currentPlayer.transform, abilityNameToActiveOnUseInventoryObject);
		}

		return false;
	}

	public virtual void checkIfIncreaseStatsOnUseInventoryObject (GameObject currentPlayer)
	{
		if (useAbilitiesListToEnableOnUseInventoryObject && currentPlayer != null) {
			GKC_Utils.increaseStatsByList (currentPlayer.transform, true, statsToIncreaseInfoList);
		}
	}

	public virtual void checkIfaddNewBlueprintsUnlockedList (GameObject currentPlayer)
	{
		if (getCraftingRecipes && currentPlayer != null) {
			GKC_Utils.addNewBlueprintsUnlockedList (currentPlayer, craftingRecipesList);
		}
	}

	public bool checkConditions (GameObject currentPlayer)
	{
		if (checkConditionsToUseObjectEnabled) {
			playerController currentPlayerController = currentPlayer.GetComponent<playerController> ();

			if (currentPlayerController != null) {
				if (playerOnGroundToUseObject) {
					if (!currentPlayerController.isPlayerOnGround ()) {
						return false;
					}
				}

				if (actionSystemNotPlayingAnimations) {
					if (currentPlayerController.isActionActive ()) {
						return false;
					}
				}
			}
		}

		return true;
	}

	public void checkExternalElementsOnUseInventoryObject (GameObject currentPlayer)
	{
		checkIfEnableAbilitiesOnUseInventoryObject (currentPlayer);

		checkIfActivateAbilitiesOnUseInventoryObject (currentPlayer);

		checkIfIncreaseStatsOnUseInventoryObject (currentPlayer);

		checkIfaddNewBlueprintsUnlockedList (currentPlayer);

		checkRemoteEvents (currentPlayer);

		checkRemoteEventsOnUseObject (currentPlayer);
	}
}