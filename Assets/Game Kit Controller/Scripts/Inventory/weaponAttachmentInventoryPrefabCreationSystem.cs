using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponAttachmentInventoryPrefabCreationSystem : inventoryPrefabCreationSystem
{
	public override void createInventoryPrefabObject ()
	{
		weaponAttachmentPickup currentWeaponAttachmentPickup = GetComponent<weaponAttachmentPickup> ();

		inventoryObject currentInventoryObject = GetComponentInChildren<inventoryObject> ();

		string newName = currentInventoryObject.inventoryObjectInfo.Name;

		currentWeaponAttachmentPickup.setAttachmentName (newName);
	}
}
