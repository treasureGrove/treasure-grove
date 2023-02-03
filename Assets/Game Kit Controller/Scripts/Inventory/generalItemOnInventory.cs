using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generalItemOnInventory : objectOnInventory
{
	[Header ("Custom Settings")]
	[Space]

	public List<statInfo> statInfoList = new List<statInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public override void activateUseObjectActionOnInventory (GameObject currentPlayer, int amountToUse)
	{
		playerComponentsManager currentPlayerComponetsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponetsManager != null) {

			playerStatsSystem currentPlayerStatsSystem = currentPlayerComponetsManager.getPlayerStatsSystem ();

			remoteEventSystem currentRemoteEventSystem = currentPlayerComponetsManager.getRemoteEventSystem ();

			inventoryManager currentInventoryManager = currentPlayerComponetsManager.getInventoryManager ();

			bool checkConditionsResult = checkConditions (currentPlayer);

			if (showDebugPrint) {
				print (checkConditionsResult);
			}

			if (checkConditionsResult) {
				if (currentPlayerStatsSystem != null) {
					bool canUseStats = true;

					if (useOnlyAmountNeeded) {
						for (int k = 0; k < statInfoList.Count; k++) {
							if (canUseStats && currentPlayerStatsSystem.isStatOnMaxAmount (statInfoList [k].Name)) {
								canUseStats = false;

								amountToUse = 0;
							}
						}
					}

					if (canUseStats) {
						for (int k = 0; k < statInfoList.Count; k++) {
							currentPlayerStatsSystem.addOrRemovePlayerStatAmount (statInfoList [k].Name, statInfoList [k].amountToAdd);

							if (statInfoList [k].useRemoteEvent) {
								if (currentRemoteEventSystem != null) {
									for (int i = 0; i < statInfoList [k].remoteEventList.Count; i++) {

										currentRemoteEventSystem.callRemoteEvent (statInfoList [k].remoteEventList [i]);
									}
								}
							}
						}
					}
				}

				if (currentInventoryManager != null) {
					if (amountToUse > 0) {
						if (activateAbilityOnUseInventoryObject && checkIfAbilityIsNotActiveOrOnCoolDown) {
							if (checkIfAbilitiesOnUseOrCooldown (currentPlayer)) {

								amountToUse = 0;
							}
						}
					}

					currentInventoryManager.setUseObjectWithNewBehaviorResult (amountToUse);

					if (amountToUse > 0) {
						if (closeInventoryOnObjectUsed) {
							if (currentInventoryManager.isInventoryMenuOpened ()) {
								currentInventoryManager.openOrCloseInventory (false);
							}
						}

						checkExternalElementsOnUseInventoryObject (currentPlayer);

						if (showDebugPrint) {
							print ("Using Inventory Object " + mainInventoryObject.inventoryObjectInfo.Name);
						}
					}
				}
			} else {
				if (currentInventoryManager != null) {
					if (useCustomMessageOnConditionFailed) {
						currentInventoryManager.setCustomMessageOnConditionFailedOnUseObjectWithNewBehavior (customMessageOnConditionFailed, true);
					}

					currentInventoryManager.setUseObjectWithNewBehaviorResult (0);
				}
			}
		}
	}

	public override bool setObjectEquippedStateOnInventory (GameObject currentPlayer, bool state)
	{
		checkRemoteEvents (currentPlayer);

		checkRemoteEventsOnSetObjectEquipState (currentPlayer, state);

		return true;
	}

	[System.Serializable]
	public class statInfo
	{
		public string Name;
		public float amountToAdd;

		[Space]

		public bool useRemoteEvent;

		public List<string> remoteEventList = new List<string> ();
	}
}
