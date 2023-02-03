using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oxygenOnInventory : objectOnInventory
{
	public override void activateUseObjectActionOnInventory (GameObject currentPlayer, int amountToUse)
	{
		float totalAmountToUse = mainInventoryObject.inventoryObjectInfo.amountPerUnit * amountToUse;

		float totalAmountToPick = applyDamage.getOxygenAmountToPick (currentPlayer, totalAmountToUse);

		applyDamage.setOxygen (totalAmountToPick, currentPlayer);

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
