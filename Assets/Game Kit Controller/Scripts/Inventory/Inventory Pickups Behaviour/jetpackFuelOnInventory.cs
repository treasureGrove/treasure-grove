using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jetpackFuelOnInventory : objectOnInventory
{
	public override void activateUseObjectActionOnInventory (GameObject currentPlayer, int amountToUse)
	{
		float totalAmountToUse = mainInventoryObject.inventoryObjectInfo.amountPerUnit * amountToUse;

		float totalAmountToPick = 0;

		playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			jetpackSystem jetpackManager = mainPlayerComponentsManager.getJetpackSystem ();

			if (jetpackManager != null) {
				totalAmountToPick = jetpackManager.getJetpackFuelAmountToPick ();

				jetpackManager.getJetpackFuel (totalAmountToPick);
			}
		}

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
