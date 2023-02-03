using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterCustomizationInventoryHelper : inventoryManagerInfo
{
	[Header ("Custom Settings")]
	[Space]

	public bool setCharacterElementsEquipped = true;

	public int amountOnElements = 1;

	public characterCustomizationManager mainCharacterCustomizationManager;

	public inventoryListManager mainInventoryListManager;


	public void storeCustomizationElementsAsInventory ()
	{
		inventoryList.Clear ();

		List<string> currentPiecesList = mainCharacterCustomizationManager.getCurrentPiecesList ();

		for (int i = 0; i < currentPiecesList.Count; i++) {

			inventoryInfo newInventoryInfo = mainInventoryListManager.getInventoryInfoFromName (currentPiecesList [i]);

			if (newInventoryInfo != null) {
				newInventoryInfo.amount = amountOnElements;
				newInventoryInfo.isEquiped = setCharacterElementsEquipped;

				inventoryList.Add (newInventoryInfo);
			}
		}
	}
}
