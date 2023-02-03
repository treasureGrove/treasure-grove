using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armorClothInventoryPrefabCreationSystem : inventoryPrefabCreationSystem
{
	public override void createInventoryPrefabObject ()
	{
		armorClothPickup currentArmorClothPickup = GetComponent<armorClothPickup> ();

		inventoryObject currentInventoryObject = GetComponentInChildren<inventoryObject> ();

		string newName = currentInventoryObject.inventoryObjectInfo.Name;

		currentArmorClothPickup.setObjectName (newName);
	}
}
