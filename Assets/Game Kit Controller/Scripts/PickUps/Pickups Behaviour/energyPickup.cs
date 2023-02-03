using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class energyPickup : pickupType
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

			amountTaken = (int)applyDamage.getEnergyAmountToPick (character, mainPickupObject.amount);
			//print (amountTaken);
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
				applyDamage.setEnergy (amountTaken, player);
			}

			if (finderIsVehicle) {
				applyDamage.setEnergy (amountTaken, vehicle);
			}

			if (finderIsCharacter) {
				applyDamage.setEnergy (amountTaken, npc);
			}
		}
			
		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			if (storePickupOnInventory) {
				showPickupTakenMessage ("Energy x " + amountTaken + " Stored");
			} else {
				showPickupTakenMessage ("Energy x " + amountTaken);
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