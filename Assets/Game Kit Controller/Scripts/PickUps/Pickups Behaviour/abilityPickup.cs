using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class abilityPickup : pickupType
{
	[Header ("Custom Pickup Settings")]
	[Space]

	public string abilityName;

	playerAbilitiesSystem mainPlayerAbilitiesSystem;

	public override bool checkIfCanBePicked ()
	{
		GameObject character = gameObject;

		if (finderIsPlayer) {
			character = player;
		} 

		if (finderIsCharacter) {
			character = npc;
		}

		playerComponentsManager mainPlayerComponentsManager = character.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			mainPlayerAbilitiesSystem = mainPlayerComponentsManager.getPlayerAbilitiesSystem ();

			if (mainPlayerAbilitiesSystem != null) {
				canPickCurrentObject = true;
			}
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (finderIsPlayer) {
			mainPlayerAbilitiesSystem.enableAbilityByName (abilityName);

			if (useCustomPickupMessage) {
				showPickupTakenMessage (amountTaken);
			} else {
				showPickupTakenMessage (abilityName + " Unlocked");
			}
		} 

		if (finderIsCharacter) {
			mainPlayerAbilitiesSystem.enableAbilityByName (abilityName);
		} 

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}
}