using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armorClothPickup : pickupType
{
	[Header ("Custom Settings")]
	[Space]

	public string objectName;
	public string categoryName;

	bool storePickedObjectOnInventory;

	public override bool checkIfCanBePicked ()
	{
		if (finderIsPlayer) {
			if (mainPickupObject.inventoryObjectManager != null) {
				bool storeObjectOnInventory = false;

				playerComponentsManager mainPlayerComponentsManager = player.GetComponent<playerComponentsManager> ();

				if (mainPlayerComponentsManager != null) {
					inventoryCharacterCustomizationSystem mainInventoryCharacterCustomizationSystem = mainPlayerComponentsManager.getInventoryCharacterCustomizationSystem ();

					if (mainInventoryCharacterCustomizationSystem != null) {
						storeObjectOnInventory = true;
					}
				}

				if (storeObjectOnInventory) {
					canPickCurrentObject = mainPickupObject.tryToPickUpObject ();
				} else {
					canPickCurrentObject = true;
				}

				storePickedObjectOnInventory = true;

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
		bool objectPickedCorrectly = false;

		if (finderIsPlayer) {
			if (storePickedObjectOnInventory) {
				objectPickedCorrectly = true;
			} else {
				
			}
		} 

		if (finderIsCharacter) {

		} 

		if (!objectPickedCorrectly) {
			return;
		}

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			showPickupTakenMessage (objectName + " Picked");
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}

	public void setObjectName (string newName)
	{
		objectName = newName;
	}
}