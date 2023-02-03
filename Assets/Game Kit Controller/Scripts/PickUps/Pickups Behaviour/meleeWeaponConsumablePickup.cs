using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeWeaponConsumablePickup : pickupType
{
	[Header ("Custom Settings")]
	[Space]

	public string weaponConsumableName;

	//	meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager;

	bool storePickedWeaponsOnInventory;

	public override bool checkIfCanBePicked ()
	{
		if (finderIsPlayer) {
			if (mainPickupObject.inventoryObjectManager != null) {
				canPickCurrentObject = mainPickupObject.tryToPickUpObject ();
				storePickedWeaponsOnInventory = true;

				amountTaken = mainPickupObject.amount;
			}
		}

		if (finderIsCharacter) {
			findObjectivesSystem currentfindObjectivesSystem = npc.GetComponent<findObjectivesSystem> ();  

			if (currentfindObjectivesSystem != null) {
				if (currentfindObjectivesSystem.isSearchingWeapon ()) {

				}
			}
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		bool weaponPickedCorrectly = false;

		if (finderIsPlayer) {
			if (storePickedWeaponsOnInventory) {
				weaponPickedCorrectly = true;
			} else {
				//				weaponPickedCorrectly = weaponsManager.pickWeapon (weaponName);
			}
		} 

		//		if (finderIsCharacter) {
		//			weaponPickedCorrectly = weaponsManager.pickWeapon (weaponName);
		//		} 

		if (!weaponPickedCorrectly) {
			return;
		}

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			showPickupTakenMessage (weaponConsumableName + " Picked");
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}

	public void setWeaponConsumableName (string newWeaponConsumableName)
	{
		weaponConsumableName = newWeaponConsumableName;
	}
}