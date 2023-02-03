using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vehicleFuelPickup : pickupType
{
	public override bool checkIfCanBePicked ()
	{
		if (finderIsPlayer) {
			canPickCurrentObject = mainPickupObject.tryToPickUpObject ();

			amountTaken = mainPickupObject.getLastinventoryAmountPicked ();
		}

		if (finderIsVehicle) {
			//check the same if the player is driving and works in the same way for any type of pickup
			amountTaken = (int)applyDamage.getFuelAmountToPick (vehicle, mainPickupObject.getAmountPicked ());
			if (amountTaken > 0) {
				canPickCurrentObject = true;
			}
				
			if (mainPickupObject.useAmountPerUnit) {
				int amountUsed = Mathf.RoundToInt (amountTaken / mainPickupObject.amountPerUnit);
			
				mainPickupObject.amount -= amountUsed;

				if (showDebugPrint) {
					print ("TOTAL " + amountUsed);
				}
			} else {

				mainPickupObject.amount -= amountTaken;
			}
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (showDebugPrint) {
			print ("total amount taken " + amountTaken);
		}

		if (finderIsPlayer) {
			if (useCustomPickupMessage) {
				showPickupTakenMessage (amountTaken);
			} else {
				string messageToUse = mainPickupObject.inventoryObjectManager.inventoryObjectInfo.Name;

				if (amountTaken > 1) {
					messageToUse += " x " + amountTaken;
				} else {
					messageToUse += " Stored";
				}

				showPickupTakenMessage (messageToUse);
			}

			mainPickupObject.playPickupSound ();

			if (mainPickupObject.amount > 0 && !takeWithTrigger) {
				mainPickupObject.checkEventOnRemainingAmount ();

				return;
			}
		}

		if (finderIsVehicle) {
			if (showDebugPrint) {
				print (amountTaken);
			}

			applyDamage.setFuel (amountTaken, vehicle);

			if (useCustomPickupMessage) {
				showPickupTakenMessage (amountTaken);
			} else {
				showPickupTakenMessage ("Fuel x " + amountTaken);
			}

			mainPickupObject.playPickupSound ();
		}

		mainPickupObject.removePickupFromLevel ();
	}
}