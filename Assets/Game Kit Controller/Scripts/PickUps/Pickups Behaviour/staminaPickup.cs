using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class staminaPickup : pickupType
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

			if (finderIsCharacter) {
				character = npc;
			}

			amountTaken = (int)applyDamage.getStaminaAmountToPick (character, mainPickupObject.amount);
			if (amountTaken > 0) {
				canPickCurrentObject = true;
			}

			mainPickupObject.amount -= amountTaken;
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (!storePickupOnInventory) {
			if (finderIsPlayer) {
				applyDamage.setStamina (amountTaken, player);
			} 

			if (finderIsCharacter) {
				applyDamage.setStamina (amountTaken, npc);
			}
		}

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			if (storePickupOnInventory) {
				showPickupTakenMessage ("Stamina x " + amountTaken + " Stored");
			} else {
				showPickupTakenMessage ("Stamina x " + amountTaken);
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