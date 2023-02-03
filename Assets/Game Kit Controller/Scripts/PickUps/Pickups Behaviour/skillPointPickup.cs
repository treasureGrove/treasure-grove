using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skillPointPickup : pickupType
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
			applyDamage.setSkillPoints (amountTaken, player);
		} 

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			showPickupTakenMessage ("Skill point x " + amountTaken);
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}
}