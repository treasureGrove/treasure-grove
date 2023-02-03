using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeShieldInventoryPrefabCreationSystem : inventoryPrefabCreationSystem
{
	public override void createInventoryPrefabObject ()
	{
		meleeShieldPickup currentMeleeShieldPickup = GetComponent<meleeShieldPickup> ();

		inventoryObject currentInventoryObject = GetComponentInChildren<inventoryObject> ();

		string newName = currentInventoryObject.inventoryObjectInfo.Name;

		currentMeleeShieldPickup.setMeleeShieldName (newName);
	}
}
