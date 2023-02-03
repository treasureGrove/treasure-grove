using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldPickup : pickupType
{
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

			if (finderIsVehicle) {
				character = vehicle;
			}

			if (finderIsCharacter) {
				character = npc;
			}

			amountTaken = (int)applyDamage.getShieldAmountToPick (character, mainPickupObject.amount);
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
				applyDamage.setShield (amountTaken, player);
			} 

			if (finderIsVehicle) {
				applyDamage.setShield (amountTaken, vehicle);
			}

			if (finderIsCharacter) {
				applyDamage.setShield (amountTaken, npc);
			}
		}

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			if (storePickupOnInventory) {
				showPickupTakenMessage ("Shield x " + amountTaken + " Stored");
			} else {
				showPickupTakenMessage ("Shield x " + amountTaken);
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