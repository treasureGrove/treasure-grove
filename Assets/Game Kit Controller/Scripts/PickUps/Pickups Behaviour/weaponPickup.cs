using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : pickupType
{
	[Header ("Custom Pickup Settings")]
	[Space]

	public string weaponName;

	playerWeaponsManager weaponsManager;

	bool storePickedWeaponsOnInventory;

	public override bool checkIfCanBePicked ()
	{
		if (finderIsPlayer) {
			weaponsManager = player.GetComponent<playerWeaponsManager> ();

			if (!weaponsManager.isAimingWeapons ()) {

				bool canStoreAnyNumberSameWeapon = weaponsManager.canStoreAnyNumberSameWeaponState ();

				bool weaponCanBePicked = weaponsManager.checkIfWeaponCanBePicked (weaponName);

				if (canStoreAnyNumberSameWeapon) {
					weaponCanBePicked = weaponsManager.checkIfWeaponExists (weaponName);
				}

				bool weaponsAreMoving = weaponsManager.weaponsAreMoving ();

				if (weaponCanBePicked && !weaponsAreMoving && !weaponsManager.currentWeaponIsMoving ()) {
					//check if the weapon can be stored in the inventory too
					bool canBeStoredOnInventory = false;
					bool hasInventoryObjectComponent = false;

					if (weaponsManager.storePickedWeaponsOnInventory) {
						if (mainPickupObject.inventoryObjectManager) {
							hasInventoryObjectComponent = true;
							canBeStoredOnInventory = mainPickupObject.tryToPickUpObject ();
							storePickedWeaponsOnInventory = true;
						}
					}

					if ((weaponsManager.storePickedWeaponsOnInventory && canBeStoredOnInventory) ||
					    !weaponsManager.storePickedWeaponsOnInventory || !hasInventoryObjectComponent) {
						canPickCurrentObject = true;
					}
				}
			}
		}

		if (finderIsCharacter) {
			findObjectivesSystem currentfindObjectivesSystem = npc.GetComponent<findObjectivesSystem> ();  

			if (currentfindObjectivesSystem != null) {
				if (currentfindObjectivesSystem.isSearchingWeapon ()) {
					
					weaponsManager = npc.GetComponent<playerWeaponsManager> ();

					if (!weaponsManager.isAimingWeapons ()) {
						bool weaponCanBePicked = weaponsManager.checkIfWeaponCanBePicked (weaponName);
						bool weaponsAreMoving = weaponsManager.weaponsAreMoving ();

						if (weaponCanBePicked && !weaponsAreMoving) {
							canPickCurrentObject = true;
						}
					}
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
				weaponPickedCorrectly = weaponsManager.pickWeapon (weaponName);
			}
		} 

		if (finderIsCharacter) {
			weaponPickedCorrectly = weaponsManager.pickWeapon (weaponName);
		} 

		if (!weaponPickedCorrectly) {
			return;
		}

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			showPickupTakenMessage (weaponName + " Picked");
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}

	public void setWeaponName (string newWeaponName)
	{
		weaponName = newWeaponName;
	}
}