using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponAttachmentPickup : pickupType
{
	[Header ("Custom Pickup Settings")]
	[Space]

	public string weaponName;
	public string attachmentName;

	playerWeaponsManager weaponsManager;

	public override bool checkIfCanBePicked ()
	{
		if (storePickupOnInventory) {
			canPickCurrentObject = mainPickupObject.tryToPickUpObject ();
		} else {
			if (finderIsPlayer) {
				weaponsManager = player.GetComponent<playerWeaponsManager> ();

				if (weaponsManager.pickupAttachment (weaponName, attachmentName)) {
					canPickCurrentObject = true;
				} 

				if (!canPickCurrentObject) {
					weaponsManager.showCantPickAttacmentMessage (attachmentName);
				}
			}
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			if (storePickupOnInventory) {
				showPickupTakenMessage ("Attachment " + attachmentName + " Stored");
			} else {
				showPickupTakenMessage ("Attachment " + attachmentName + " Picked");
			}
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}

	public void setAttachmentName (string newName)
	{
		attachmentName = newName;
	}
}