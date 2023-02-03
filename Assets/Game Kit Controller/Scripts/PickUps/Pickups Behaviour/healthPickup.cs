using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPickup : pickupType
{
	public override bool checkIfCanBePicked ()
	{
		if (storePickupOnInventory) {
			canPickCurrentObject = mainPickupObject.tryToPickUpObject ();

			amountTaken = mainPickupObject.getLastinventoryAmountPicked ();
		} else {
			GameObject character = gameObject;

			if (finderIsPlayer) {
				//if the player is not driving then increase an auxiliar value to check the amount of the same pickup that the player will use at once 
				//for example, when the player is close to more than one pickup, if he has 90/100 of health and he is close to two health pickups, 
				//he only will grab one of them.
				character = player;
			} 

			if (finderIsVehicle) {
				//check the same if the player is driving and works in the same way for any type of pickup
				character = vehicle;
			}

			if (finderIsCharacter) {
				character = npc;
			}

			amountTaken = (int)applyDamage.getHealthAmountToPick (character, mainPickupObject.amount);
			if (amountTaken > 0) {
				canPickCurrentObject = true;
			}

			mainPickupObject.amount -= amountTaken;
		}

		if (showDebugPrint) {
			print ("can pick current object " + canPickCurrentObject); 
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (showDebugPrint) {
			print ("total amount taken " + amountTaken);
		}

		if (!storePickupOnInventory) {
			//if the player is not driving then
			if (finderIsPlayer) {
				//increase its health
				applyDamage.setHeal (amountTaken, player);
			} 

			//the player is driving so the pickup will recover its health
			if (finderIsVehicle) {
				applyDamage.setHeal (amountTaken, vehicle);
			}

			if (finderIsCharacter) {
				applyDamage.setHeal (amountTaken, npc);
			}
		}

		//set the info in the screen to show the type of object used and its amount
	
		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			if (storePickupOnInventory) {
				showPickupTakenMessage ("Health x " + amountTaken + " Stored");
			} else {
				showPickupTakenMessage ("Health x " + amountTaken);
			}
		}

		mainPickupObject.playPickupSound ();

		checkIfUseInventoryObjectWhenPicked ();

		if (mainPickupObject.amount > 0 && !takeWithTrigger) {
			mainPickupObject.checkEventOnRemainingAmount ();
			return;
		}

		mainPickupObject.removePickupFromLevel ();
	}
}