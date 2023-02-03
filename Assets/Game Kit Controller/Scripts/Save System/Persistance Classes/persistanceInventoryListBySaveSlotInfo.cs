using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceInventoryListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistanceInventoryListByPlayerInfo> playerInventoryList = new List<persistanceInventoryListByPlayerInfo> ();
}

[System.Serializable]
public class persistanceInventoryListByPlayerInfo
{
	public int playerID;

	public int inventorySlotAmount;
	public bool infiniteSlots;

	public List<persistanceInventoryObjectInfo> inventoryObjectList = new List<persistanceInventoryObjectInfo> ();

	//Crafting info
	public bool useOnlyBlueprintsUnlocked;
	public List<string> blueprintsUnlockedList = new  List<string> ();

	public bool anyObjectToCraftInTimeActive;
	public List<craftObjectInTimeSimpleInfo> craftObjectInTimeSimpleInfoList = new  List<craftObjectInTimeSimpleInfo> ();

	public List<string> objectCategoriesToCraftAvailableAtAnyMoment = new List<string> ();
}

[System.Serializable]
public class persistanceInventoryObjectInfo
{
	public string Name;
	public int amount;
	public bool infiniteAmount;
	public string inventoryObjectName;

	public int categoryIndex;
	public int elementIndex;

	public bool isEquipped;

	public int quickAccessSlotIndex = -1;

	public float vendorPrice;
	public float sellPrice;

	public bool useMinLevelToBuy;
	public float minLevelToBuy;

	public bool spawnObject;

	public bool useDurability;
	public float durabilityAmount;

	public float maxDurabilityAmount;

	public bool objectIsBroken;

	public bool isWeapon;
	public bool isMeleeWeapon;
	public int projectilesInMagazine = -1;

	public persistanceInventoryObjectInfo (persistanceInventoryObjectInfo obj)
	{
		Name = obj.Name;
		amount = obj.amount;
		infiniteAmount = obj.infiniteAmount;
		inventoryObjectName = obj.inventoryObjectName;

		categoryIndex = obj.categoryIndex;
		elementIndex = obj.elementIndex;

		isEquipped = obj.isEquipped;

		quickAccessSlotIndex = obj.quickAccessSlotIndex;

		vendorPrice = obj.vendorPrice;
		sellPrice = obj.sellPrice;

		useMinLevelToBuy = obj.useMinLevelToBuy;
		minLevelToBuy = obj.minLevelToBuy;

		spawnObject = obj.spawnObject;

		useDurability = obj.useDurability;
		durabilityAmount = obj.durabilityAmount;

		maxDurabilityAmount = obj.maxDurabilityAmount;

		objectIsBroken = obj.objectIsBroken;

		isWeapon = obj.isWeapon;
		isMeleeWeapon = obj.isWeapon;
		projectilesInMagazine = obj.projectilesInMagazine; 
	}

	public persistanceInventoryObjectInfo ()
	{
		Name = "New Object";
	}
}

[System.Serializable]
public class craftObjectInTimeSimpleInfo
{
	public string objectName;

	public string objectCategoryName;

	public int amount;

	public float timeToCraftObject;

	public bool objectCreatedOnWorkbench;
	public int workbenchID;
}