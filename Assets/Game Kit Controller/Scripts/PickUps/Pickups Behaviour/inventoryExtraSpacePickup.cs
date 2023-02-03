using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryExtraSpacePickup : pickupType
{
	public override bool checkIfCanBePicked ()
	{
		if (finderIsPlayer) {
			canPickCurrentObject = true;
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (finderIsPlayer) {
			int extraSpaceAmount = mainPickupObject.inventoryObjectManager.inventoryObjectInfo.amount;

			player.GetComponent<inventoryManager> ().addInventoryExtraSpace (extraSpaceAmount);

			if (useCustomPickupMessage) {
				showPickupTakenMessage (extraSpaceAmount);
			} else {
				showPickupTakenMessage ("+" + extraSpaceAmount + " slots added to inventory");
			}

			mainPickupObject.playPickupSound ();
		}

		mainPickupObject.removePickupFromLevel ();
	}
}