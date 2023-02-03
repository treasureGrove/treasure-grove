using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoPickup : pickupType
{
	[Header ("Custom Pickup Settings")]
	[Space]

	public string ammoName;

	public string unableToPickAmmoMessage = "You haven't weapons with this ammo type";

	playerWeaponsManager weaponsManager;

	public override bool checkIfCanBePicked ()
	{
		if (storePickupOnInventory) {
			canPickCurrentObject = mainPickupObject.tryToPickUpObject ();

			amountTaken = mainPickupObject.getLastinventoryAmountPicked ();
		} else {
			if (finderIsPlayer || finderIsCharacter) {
				GameObject character = player;
				if (finderIsCharacter) {
					character = npc;
				}

				weaponsManager = character.GetComponent<playerWeaponsManager> ();

				bool weaponAvailable = weaponsManager.checkIfWeaponIsAvailable (ammoName);
				bool weaponHasAmmoLimit = weaponsManager.hasAmmoLimit (ammoName);

				if (!weaponAvailable) {
					inventoryManager mainInventoryManager = character.GetComponent<inventoryManager> ();

					if (mainInventoryManager != null) {

						if (mainInventoryManager.existInPlayerInventoryFromName (ammoName)) {
							weaponAvailable = true;

							if (showDebugPrint) {
								print ("weapon is available in the inventory not equipped, but the ammo can be taken and added directly to the weapon");
							}
						}
					}
				}

				if (weaponAvailable) {
					if (weaponHasAmmoLimit) {
						bool weaponHasMaximumAmmoAmount = weaponsManager.hasMaximumAmmoAmount (ammoName);

						if (weaponHasMaximumAmmoAmount) {
							if (showDebugPrint) {
								print ("maximum amount on " + ammoName);
							}
						}

						if (!weaponHasMaximumAmmoAmount) {
							amountTaken = applyDamage.getPlayerWeaponAmmoAmountToPick (weaponsManager, ammoName, mainPickupObject.amount);

							if (amountTaken > 0) {
								canPickCurrentObject = true;
							}
						}
					} else {
						amountTaken = mainPickupObject.amount;

						canPickCurrentObject = true;
					}
				} else {
					showPickupTakenMessage (unableToPickAmmoMessage);
				}
			}

			if (finderIsVehicle) {
				vehicleWeaponSystem currentVehicleWeaponSystem = vehicle.GetComponentInChildren<vehicleWeaponSystem> ();

				if (currentVehicleWeaponSystem != null) {
					bool weaponAvailable = currentVehicleWeaponSystem.checkIfWeaponIsAvailable (ammoName);
					bool weaponHasAmmoLimit = currentVehicleWeaponSystem.hasAmmoLimit (ammoName);

					if (weaponAvailable) {
						if (weaponHasAmmoLimit) {
							bool weaponHasMaximumAmmoAmount = currentVehicleWeaponSystem.hasMaximumAmmoAmount (ammoName);

							if (weaponHasMaximumAmmoAmount) {
								if (showDebugPrint) {
									print ("maximum amount on " + ammoName);
								}
							}

							if (!weaponHasMaximumAmmoAmount) {
								amountTaken = applyDamage.getVehicleWeaponAmmoAmountToPick (currentVehicleWeaponSystem, ammoName, mainPickupObject.amount);

								if (amountTaken > 0) {
									canPickCurrentObject = true;
								}
							}
						} else {
							amountTaken = mainPickupObject.amount;

							canPickCurrentObject = true;
						}
					} 
				}
			}

			mainPickupObject.amount -= amountTaken;
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (showDebugPrint) {
			print ("total amount taken " + amountTaken);
		}

		if (!storePickupOnInventory) {
			if (finderIsPlayer) {
				weaponsManager.AddAmmo ((int)Mathf.Round (amountTaken), ammoName);
			}

			if (finderIsVehicle) {
				mainPickupObject.vehicleHUD.getAmmo (ammoName, (int)Mathf.Round (amountTaken));
			}

			if (finderIsCharacter) {
				weaponsManager.AddAmmo ((int)Mathf.Round (amountTaken), ammoName);
			}
		}

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			if (storePickupOnInventory) {
				showPickupTakenMessage ("Ammo " + ammoName + " x " + Mathf.Round (amountTaken) + " Stored");
			} else {
				showPickupTakenMessage ("Ammo " + ammoName + " x " + Mathf.Round (amountTaken));
			}
		}

		mainPickupObject.playPickupSound ();

		if (mainPickupObject.amount > 0 && !takeWithTrigger) {
			mainPickupObject.checkEventOnRemainingAmount ();

			return;
		}

		mainPickupObject.removePickupFromLevel ();
	}

	public void setAmmoName (string newAmmoName)
	{
		ammoName = newAmmoName;
	}
}