using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class inventoryListElement
{
	public string Name;
	public int amount;
	public bool infiniteAmount;

	public string inventoryCategoryName;
	public string inventoryObjectName;

	public int categoryIndex;
	public int elementIndex;

	public float vendorPrice;
	public float sellPrice;

	public bool useMinLevelToBuy;
	public float minLevelToBuy;

	public bool addObjectToList = true;

	public bool spawnObject;

	public bool infiniteVendorAmountAvailable;

	public bool isEquipped;

	public int quickAccessSlotIndex = -1;

	public bool addInventoryObject = true;

	public bool useDurability;
	public float durabilityAmount;

	public float maxDurabilityAmount;

	public bool objectIsBroken;

	public bool isWeapon;
	public bool isMeleeWeapon;
	public int projectilesInMagazine = -1;
}
