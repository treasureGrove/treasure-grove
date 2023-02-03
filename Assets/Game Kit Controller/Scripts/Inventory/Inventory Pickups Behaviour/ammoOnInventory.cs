using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoOnInventory : objectOnInventory
{
	public ammoPickup mainAmmoPickup;

	string ammoName;

	public override void activateCombineObjectActionOnInventory (GameObject currentPlayer, inventoryInfo inventoryInfoToUse)
	{
		int amountTaken = 0;

		bool canCombineAmmo = false;

		ammoName = mainAmmoPickup.ammoName;

		print ("ammo selected for the weapon " + ammoName);

		playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		playerWeaponsManager weaponsManager = currentPlayerComponentsManager.getPlayerWeaponsManager ();

		playerWeaponSystem currrentPlayerWeaponSystem = weaponsManager.getWeaponSystemByName (ammoName);

		inventoryManager mainInventoryManager = currentPlayerComponentsManager.getInventoryManager ();

		print ("ammo type selected is " + currrentPlayerWeaponSystem.getWeaponSystemName () + " to combine with the ammo for the weapon " + ammoName);

		bool ammoToUseIsForWeaponSelected = false;
		inventoryInfo firstObjectToCombine = mainInventoryManager.getCurrentFirstObjectToCombine ();
		inventoryInfo secondObjectToCombine = mainInventoryManager.getCurrentSecondObjectToCombine ();

		print ("first inventory object selected is " + firstObjectToCombine.Name);
		print ("second inventory object selected is " + secondObjectToCombine.Name);

		if (firstObjectToCombine.isWeapon && firstObjectToCombine.mainWeaponObjectInfo.getWeaponAmmoName () == secondObjectToCombine.Name) {
			ammoToUseIsForWeaponSelected = true;
		} else {
			if (secondObjectToCombine.isWeapon && secondObjectToCombine.mainWeaponObjectInfo.getWeaponAmmoName () == firstObjectToCombine.Name) {
				ammoToUseIsForWeaponSelected = true;
			}
		}

		if (ammoToUseIsForWeaponSelected) {
			int amountAvailable = inventoryInfoToUse.amountPerUnit;

			if (inventoryInfoToUse.storeTotalAmountPerUnit) {
				amountAvailable = inventoryInfoToUse.amount;
			}

			print ("amount available " + amountAvailable);

			bool weaponAvailable = weaponsManager.checkIfWeaponIsAvailable (ammoName);
			bool weaponHasAmmoLimit = weaponsManager.hasAmmoLimit (ammoName);

			if (!weaponAvailable) {
				if (mainInventoryManager.existInPlayerInventoryFromName (ammoName)) {
					weaponAvailable = true;

					weaponHasAmmoLimit = false;
				}
			}

			if (weaponAvailable) {
				if (weaponHasAmmoLimit) {
					bool weaponHasMaximumAmmoAmount = weaponsManager.hasMaximumAmmoAmount (ammoName);

					if (weaponHasMaximumAmmoAmount) {
						print ("maximum amount on " + ammoName);
					} else {
						amountTaken = applyDamage.getPlayerWeaponAmmoAmountToPick (weaponsManager, ammoName, amountAvailable);
					}

					print ("use weapon ammo limit");
				} else {
					if (currrentPlayerWeaponSystem.isUseRemainAmmoFromInventoryActive ()) {
						int magazineSizeToRefill = currrentPlayerWeaponSystem.getAmmoAmountToRefillMagazine ();

						print ("magazine free space " + magazineSizeToRefill);
						if (magazineSizeToRefill > 0) {
							if (amountAvailable >= magazineSizeToRefill) {
								amountTaken = magazineSizeToRefill;
							} else {
								amountTaken = amountAvailable;
							}
						} else {
							amountTaken = 0;

							canCombineAmmo = true;
						}

						print ("use remain ammo from inventory active");
					} else {
						amountTaken = amountAvailable;
					}
				}
			} 

			print (amountTaken);

			if (amountTaken > 0) {
				weaponsManager.setWeaponRemainAmmoFromInventory (currrentPlayerWeaponSystem);

				weaponsManager.AddAmmo ((int)Mathf.Round (amountTaken), ammoName);

				canCombineAmmo = true;
			}
		} else {
			print ("Weapon selected doesn't use the ammo of the " + ammoName);
		}

		mainInventoryManager.setCombineObjectsWithNewBehaviorResult ((int)amountTaken, canCombineAmmo);
	}
}