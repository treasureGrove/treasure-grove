using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryPickup : pickupType
{
	public override bool checkIfCanBePicked ()
	{
		if (finderIsPlayer) {
			canPickCurrentObject = mainPickupObject.tryToPickUpObject ();

			amountTaken = mainPickupObject.getLastinventoryAmountPicked ();
		}

		if (finderIsCharacter) {

		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (finderIsPlayer) {
			if (useCustomPickupMessage) {
				showPickupTakenMessage (amountTaken);
			} else {
				if (storePickupOnInventory) {
					string messageToUse = mainPickupObject.inventoryObjectManager.inventoryObjectInfo.Name;

					if (amountTaken > 1) {
						messageToUse += " x " + amountTaken;
					} else {
						messageToUse += " Stored";
					}

					showPickupTakenMessage (messageToUse);
				}
			}

			mainPickupObject.playPickupSound ();

			if (storePickupOnInventory && useInventoryObjectWhenPicked) {
				if (mainPickupObject.playerInventoryManager != null) {
					mainPickupObject.playerInventoryManager.useInventoryObjectByName (mainPickupObject.inventoryObjectManager.inventoryObjectInfo.Name, 1);
				}
			}

			if (mainPickupObject.amount > 0 && !takeWithTrigger) {
				mainPickupObject.checkEventOnRemainingAmount ();

				return;
			}
		}

		mainPickupObject.removePickupFromLevel ();
	}
}