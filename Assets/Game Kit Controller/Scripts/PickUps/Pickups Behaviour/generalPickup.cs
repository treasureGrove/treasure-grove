using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generalPickup : pickupType
{
	[Header ("Custom Settings")]
	[Space]

	public List<statInfo> statInfoList = new List<statInfo> ();

	[Space]

	public string pickupName;

	public override bool checkIfCanBePicked ()
	{
		if (storePickupOnInventory) {
			canPickCurrentObject = mainPickupObject.tryToPickUpObject ();

			amountTaken = mainPickupObject.getLastinventoryAmountPicked ();
		} else {
			canPickCurrentObject = true;
		
			mainPickupObject.amount = 0;
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (!storePickupOnInventory) {
			GameObject character = player;

			if (finderIsCharacter) {
				character = npc;
			}

			//if the player is not driving then
			playerComponentsManager currentPlayerComponetsManager = character.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponetsManager != null) {

				playerStatsSystem currentPlayerStatsSystem = currentPlayerComponetsManager.getPlayerStatsSystem ();

				remoteEventSystem currentRemoteEventSystem = currentPlayerComponetsManager.getRemoteEventSystem ();

				if (currentPlayerStatsSystem != null) {
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

			checkIfEnableAbilitiesOnTakePickup (character);

			checkIfActivateAbilitiesOnTakePickup (character);

			checkIfaddNewBlueprintsUnlockedList (character);
		}

		//set the info in the screen to show the type of object used and its amount

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			if (storePickupOnInventory) {
				showPickupTakenMessage (pickupName + " x " + amountTaken + " Stored");
			} else {
				showPickupTakenMessage (pickupName + " x " + amountTaken);
			}
		}

		mainPickupObject.playPickupSound ();

		checkIfUseInventoryObjectWhenPicked ();

		mainPickupObject.removePickupFromLevel ();
	}

	public void setConsumableName (string newConsumableName)
	{
		pickupName = newConsumableName;
	}

	[System.Serializable]
	public class statInfo
	{
		public string Name;
		public float amountToAdd;

		public bool useRemoteEvent;

		public List<string> remoteEventList = new List<string> ();
	}
}