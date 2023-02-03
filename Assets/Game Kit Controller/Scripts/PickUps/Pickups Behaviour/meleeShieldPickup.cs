using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeShieldPickup : pickupType
{
	[Header ("Custom Settings")]
	[Space]

	public string shieldName;

	bool storePickedShieldsOnInventory;

	public override bool checkIfCanBePicked ()
	{
		if (finderIsPlayer) {
			if (mainPickupObject.inventoryObjectManager != null) {
				bool storeShieldOnInventory = false;

				playerComponentsManager mainPlayerComponentsManager = player.GetComponent<playerComponentsManager> ();

				if (mainPlayerComponentsManager != null) {
					meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager = mainPlayerComponentsManager.getMeleeWeaponsGrabbedManager ();

					if (mainMeleeWeaponsGrabbedManager != null) {
						if (mainMeleeWeaponsGrabbedManager.storePickedWeaponsOnInventory) {
							storeShieldOnInventory = true;
						} else {
							mainMeleeWeaponsGrabbedManager.equipShield (shieldName);
						}
					}
				}

				if (storeShieldOnInventory) {
					canPickCurrentObject = mainPickupObject.tryToPickUpObject ();
				} else {
					canPickCurrentObject = true;
				}

				storePickedShieldsOnInventory = true;

				amountTaken = mainPickupObject.amount;
			}
		}

		if (finderIsCharacter) {
			findObjectivesSystem currentfindObjectivesSystem = npc.GetComponent<findObjectivesSystem> ();  
		
			if (currentfindObjectivesSystem != null) {
						
			}
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		bool shieldPickedCorrectly = false;

		if (finderIsPlayer) {
			if (storePickedShieldsOnInventory) {
				shieldPickedCorrectly = true;
			} else {
				//				weaponPickedCorrectly = weaponsManager.pickWeapon (weaponName);
			}
		} 

		//		if (finderIsCharacter) {
		//			weaponPickedCorrectly = weaponsManager.pickWeapon (weaponName);
		//		} 

		if (!shieldPickedCorrectly) {
			return;
		}

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			showPickupTakenMessage (shieldName + " Picked");
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}

	public void setMeleeShieldName (string newShieldName)
	{
		shieldName = newShieldName;
	}
}