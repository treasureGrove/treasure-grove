using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabObjectsStrengthPickup : pickupType
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
			applyDamage.increaseStrengthAmountAndUpdateStat (amountTaken, player);
		} 

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			showPickupTakenMessage ("Grab Objects Strength + " + amountTaken);
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}
}