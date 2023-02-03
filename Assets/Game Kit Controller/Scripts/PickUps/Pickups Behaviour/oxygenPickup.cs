using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oxygenPickup : pickupType
{
	[Header ("Custom Pickup Settings")]
	[Space]

	public bool refillOxygen;

	public override bool checkIfCanBePicked ()
	{
		if (storePickupOnInventory) {
			canPickCurrentObject = mainPickupObject.tryToPickUpObject ();

			amountTaken = mainPickupObject.getLastinventoryAmountPicked ();
		} else {
			GameObject character = gameObject;

			if (finderIsPlayer) {
				character = player;
			} 

			if (finderIsCharacter) {
				character = npc;
			}

			if (refillOxygen) {
				amountTaken = mainPickupObject.amount;
			} else {
				amountTaken = (int)applyDamage.getOxygenAmountToPick (character, mainPickupObject.amount);
			}

			if (amountTaken > 0) {
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
				if (refillOxygen) {
					applyDamage.refillFullOxygen (player);
				} else {
					applyDamage.setOxygen (amountTaken, player);
				}
			} 

			if (finderIsCharacter) {
				applyDamage.setOxygen (amountTaken, npc);
			}
		}

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			if (storePickupOnInventory) {
				showPickupTakenMessage ("Oxygen x " + amountTaken + " Stored");
			} else {
				if (refillOxygen) {
					showPickupTakenMessage ("Oxygen Refilled");
				} else {
					showPickupTakenMessage ("Oxygen x " + amountTaken);
				}
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