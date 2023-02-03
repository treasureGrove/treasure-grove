using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class staminaOnInventory : objectOnInventory
{
	public override void activateUseObjectActionOnInventory (GameObject currentPlayer, int amountToUse)
	{
		float totalAmountToUse = mainInventoryObject.inventoryObjectInfo.amountPerUnit * amountToUse;

		float totalAmountToPick = applyDamage.getStaminaAmountToPick (currentPlayer, totalAmountToUse);

		applyDamage.setStamina (totalAmountToPick, currentPlayer);

		int totalAmountUsed = (int)totalAmountToPick / mainInventoryObject.inventoryObjectInfo.amountPerUnit;

		if (totalAmountToPick % totalAmountToUse > 0) {
			totalAmountUsed += 1;
		}

		if (!useOnlyAmountNeeded) {
			totalAmountUsed = amountToUse;
		}

		if (amountToUse > 0) {
			checkExternalElementsOnUseInventoryObject (currentPlayer);
		}

		inventoryManager currentInventoryManager = currentPlayer.GetComponent<inventoryManager> ();

		if (currentInventoryManager != null) {
			currentInventoryManager.setUseObjectWithNewBehaviorResult (totalAmountUsed);
		}
	}
}
