using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeWeaponConsumableInventoryPrefabCreationSystem : inventoryPrefabCreationSystem
{
	public override void createInventoryPrefabObject ()
	{
		meleeWeaponConsumablePickup currentMeleeWeaponConsumablePickup = GetComponent<meleeWeaponConsumablePickup> ();

		inventoryObject currentInventoryObject = GetComponentInChildren<inventoryObject> ();

		string newName = currentInventoryObject.inventoryObjectInfo.Name;

		currentMeleeWeaponConsumablePickup.setWeaponConsumableName (newName);
	}
}
