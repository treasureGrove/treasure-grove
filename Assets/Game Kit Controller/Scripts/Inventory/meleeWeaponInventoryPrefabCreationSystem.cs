using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeWeaponInventoryPrefabCreationSystem : inventoryPrefabCreationSystem
{
	public override void createInventoryPrefabObject ()
	{
		meleeWeaponPickup currentMeleeWeaponPickup = GetComponent<meleeWeaponPickup> ();

		inventoryObject currentInventoryObject = GetComponentInChildren<inventoryObject> ();

		string newName = currentInventoryObject.inventoryObjectInfo.Name;

		currentMeleeWeaponPickup.setMeleeWeaponName (newName);
	}
}
