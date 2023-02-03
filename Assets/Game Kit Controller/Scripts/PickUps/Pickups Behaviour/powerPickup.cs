using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerPickup : pickupType
{
	[Header ("Custom Pickup Settings")]
	[Space]

	public string powerName;

	powersAndAbilitiesSystem powersAndAbilitiesManager;

	public override bool checkIfCanBePicked ()
	{
		GameObject character = gameObject;

		if (finderIsPlayer) {
			character = player;
		} 

		if (finderIsCharacter) {
			character = npc;
		}

		powersAndAbilitiesManager = character.GetComponentInChildren<powersAndAbilitiesSystem> ();

		if (powersAndAbilitiesManager != null) {
			canPickCurrentObject = true;
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (finderIsPlayer) {
			powersAndAbilitiesManager.enableGeneralPower (powerName);

			if (useCustomPickupMessage) {
				showPickupTakenMessage (amountTaken);
			} else {
				showPickupTakenMessage (powerName + " Activated");
			}
		} 

		if (finderIsCharacter) {
			powersAndAbilitiesManager.enableGeneralPower (powerName);
		} 

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}
}