using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponInventoryPrefabCreationSystem : inventoryPrefabCreationSystem
{
	public override void createInventoryPrefabObject ()
	{
		weaponPickup currentWeaponPickup = GetComponent<weaponPickup> ();

		inventoryObject currentInventoryObject = GetComponentInChildren<inventoryObject> ();

		string newName = currentInventoryObject.inventoryObjectInfo.Name;

		currentWeaponPickup.setWeaponName (newName);
	}
}
