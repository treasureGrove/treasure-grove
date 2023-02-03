using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoInventoryPrefabCreationSystem : inventoryPrefabCreationSystem
{
	public override void createInventoryPrefabObject ()
	{
		ammoPickup currentAmmoPickup = GetComponent<ammoPickup> ();

		inventoryObject currentInventoryObject = GetComponentInChildren<inventoryObject> ();

		string newName = currentInventoryObject.inventoryObjectInfo.Name;

		newName = newName.Replace (" Ammo", "");

		currentAmmoPickup.setAmmoName (newName);
	}
}
