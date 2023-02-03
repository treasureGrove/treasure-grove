using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponAttachmentOnInventory : objectOnInventory
{
	public weaponAttachmentPickup mainWeaponAttachmentPickup;

	string attachmentName;

	public override void activateCombineObjectActionOnInventory (GameObject currentPlayer, inventoryInfo inventoryInfoToUse)
	{
		playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			playerWeaponsManager weaponsManager = mainPlayerComponentsManager.getPlayerWeaponsManager ();

			inventoryManager mainInventoryManager = mainPlayerComponentsManager.getInventoryManager ();

			if (mainInventoryManager != null) {

				attachmentName = mainWeaponAttachmentPickup.attachmentName;

				string weaponName = "";

				bool canCombineAttachment = false;

				inventoryInfo firstObjectToCombine = mainInventoryManager.getCurrentFirstObjectToCombine ();

				if (firstObjectToCombine.isWeapon) {
					weaponName = firstObjectToCombine.Name;
				} else {
					inventoryInfo secondObjectToCombine = mainInventoryManager.getCurrentSecondObjectToCombine ();

					if (secondObjectToCombine.isWeapon) {
						weaponName = secondObjectToCombine.Name;
					}
				}

				if (weaponName != "") {
					
					if (weaponsManager.pickupAttachment (weaponName, attachmentName)) {
						canCombineAttachment = true;
					}

					if (!canCombineAttachment) {
						weaponsManager.showCantPickAttacmentMessage (attachmentName);
					}
				}
				
				mainInventoryManager.setCombineObjectsWithNewBehaviorResult (1, canCombineAttachment);
			}
		}
	}
}
