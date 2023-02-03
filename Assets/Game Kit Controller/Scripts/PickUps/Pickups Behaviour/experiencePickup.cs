using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class experiencePickup : pickupType
{
	[Header ("Custom Pickup Settings")]
	[Space]

	public bool useExperienceRandomRange;
	public Vector2 experienceRandomRange;

	public override bool checkIfCanBePicked ()
	{
		GameObject character = gameObject;

		if (finderIsPlayer) {
			character = player;

			amountTaken = mainPickupObject.amount;

			canPickCurrentObject = true;
		} 

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (finderIsPlayer) {
			applyDamage.setExperience (amountTaken, player, transform, useExperienceRandomRange, experienceRandomRange);
		} 

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			showPickupTakenMessage ("Experience increased in " + amountTaken);
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}
}