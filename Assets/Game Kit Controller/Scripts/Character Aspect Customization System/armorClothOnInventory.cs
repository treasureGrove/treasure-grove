using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armorClothOnInventory : objectOnInventory
{
	[Header ("Custom Settings")]
	[Space]

	public armorClothPickup mainArmorClothPickup;

	public override void eventOnPickObject (GameObject currentPlayer)
	{



	}

	public override void eventOnDropObject (GameObject currentPlayer)
	{



	}

	public override bool setObjectEquippedStateOnInventory (GameObject currentPlayer, bool state)
	{
		playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			inventoryCharacterCustomizationSystem mainInventoryCharacterCustomizationSystem = mainPlayerComponentsManager.getInventoryCharacterCustomizationSystem ();

			if (mainInventoryCharacterCustomizationSystem != null) {
				bool equipResult = false;

				if (state) {
					equipResult = mainInventoryCharacterCustomizationSystem.equipObject (mainArmorClothPickup.objectName, mainArmorClothPickup.categoryName);
				} else {
					equipResult = mainInventoryCharacterCustomizationSystem.unequipObject (mainArmorClothPickup.objectName, mainArmorClothPickup.categoryName);
				}

				if (equipResult) {
					checkRemoteEvents (currentPlayer);

					checkRemoteEventsOnSetObjectEquipState (currentPlayer, state);
				}

				return equipResult;
			}
		}

		return false;
	}
}