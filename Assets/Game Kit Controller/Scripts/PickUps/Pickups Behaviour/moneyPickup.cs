using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moneyPickup : pickupType
{
	[Header ("Custom Pickup Settings")]
	[Space]

	public bool useMoneyRandomRange;
	public Vector2 moneyRandomRange;

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
			applyDamage.setMoney (amountTaken, player, useMoneyRandomRange, moneyRandomRange);
		} 

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}
}