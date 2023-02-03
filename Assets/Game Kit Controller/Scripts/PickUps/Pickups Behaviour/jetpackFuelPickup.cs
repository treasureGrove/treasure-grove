using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jetpackFuelPickup : pickupType
{
	public override bool checkIfCanBePicked ()
	{
		if (storePickupOnInventory) {
			canPickCurrentObject = mainPickupObject.tryToPickUpObject ();

			amountTaken = mainPickupObject.getLastinventoryAmountPicked ();
		} else {
			if (finderIsPlayer) {
				canPickCurrentObject = true;
			}

			mainPickupObject.amount -= amountTaken;
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (showDebugPrint) {
			print ("total amount taken " + amountTaken);
		}

		if (!storePickupOnInventory) {
			if (finderIsPlayer) {
				playerComponentsManager mainPlayerComponentsManager = player.GetComponent<playerComponentsManager> ();

				if (mainPlayerComponentsManager != null) {
					jetpackSystem jetpackManager = mainPlayerComponentsManager.getJetpackSystem ();

					if (jetpackManager) {
						jetpackManager.getJetpackFuel (amountTaken);
					}
				}
			} 
		}

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			if (storePickupOnInventory) {
				showPickupTakenMessage ("Jetpack Fuel x " + amountTaken + " Stored");
			} else {
				showPickupTakenMessage ("Jetpack Fuel x " + amountTaken);
			}
		}

		mainPickupObject.playPickupSound ();

		if (mainPickupObject.amount > 0 && !takeWithTrigger) {
			mainPickupObject.checkEventOnRemainingAmount ();
			return;
		}

		mainPickupObject.removePickupFromLevel ();
	}
}