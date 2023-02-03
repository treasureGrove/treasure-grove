using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeWeaponConsumableOnInventory : objectOnInventory
{
	public bool showDebugPrint;

	public meleeWeaponConsumablePickup mainMeleeWeaponConsumablePickup;

	public override void activateUseObjectActionOnInventory (GameObject currentPlayer, int amountToUse)
	{
		playerComponentsManager currentPlayerComponetsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponetsManager != null) {

			grabbedObjectMeleeAttackSystem currentGrabbedObjectMeleeAttackSystem = currentPlayerComponetsManager.getGrabbedObjectMeleeAttackSystem ();

			if (currentGrabbedObjectMeleeAttackSystem != null) {
				if (currentGrabbedObjectMeleeAttackSystem.setDamageTypeAndReactionInfo (mainMeleeWeaponConsumablePickup.weaponConsumableName)) {
					checkExternalElementsOnUseInventoryObject (currentPlayer);
				} else {
					amountToUse = 0;
				}
			}
		}

		inventoryManager currentInventoryManager = currentPlayer.GetComponent<inventoryManager> ();

		if (currentInventoryManager != null) {
			currentInventoryManager.setUseObjectWithNewBehaviorResult (amountToUse);

			if (closeInventoryOnObjectUsed && amountToUse > 0) {
				if (currentInventoryManager.isInventoryMenuOpened ()) {
					currentInventoryManager.openOrCloseInventory (false);
				}
			}
		}
	}
}
