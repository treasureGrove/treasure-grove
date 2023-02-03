using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class experienceMultiplierPickup : pickupType
{
	[Header ("Custom Pickup Settings")]
	[Space]

	public float experienceMultiplierAmount;
	public float experienceMultiplierDuration;

	public override bool checkIfCanBePicked ()
	{
		GameObject character = gameObject;

		if (finderIsPlayer) {
			character = player;

			canPickCurrentObject = true;
		} 

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (finderIsPlayer) {
			applyDamage.setExperienceMultiplier (experienceMultiplierAmount, player, experienceMultiplierDuration);
		} 

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			showPickupTakenMessage ("Exp Multiplier x " + experienceMultiplierAmount + " during " + experienceMultiplierDuration + " seconds");
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}
}