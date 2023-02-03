using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryWeightBagPickup : pickupType
{
	public override bool checkIfCanBePicked ()
	{
		GameObject character = gameObject;

		if (finderIsPlayer) {
			character = player;

			canPickCurrentObject = true;

			amountTaken = mainPickupObject.amount;
		} 

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (finderIsPlayer) {
			applyDamage.increaseInventoryBagWeight (amountTaken, player);
		} 

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			showPickupTakenMessage ("Inventory Weight Bag + " + amountTaken);
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}
}