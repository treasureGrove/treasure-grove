using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vendorSystem : MonoBehaviour
{
	public List<inventoryListElement> inventoryListManagerList = new List<inventoryListElement> ();
	public List<inventoryInfo> vendorInventoryList = new List<inventoryInfo> ();

	public string[] inventoryManagerListString;
	public List<inventoryManagerStringInfo> inventoryManagerStringInfoList = new List<inventoryManagerStringInfo> ();

	public bool useGeneralBuyPriceMultiplier;
	public float generalBuyPriceMultiplayerPercentage;

	public bool useGeneralSellPriceMultiplier;
	public float generalSellPriceMultiplayerPercentage;

	public bool infiniteVendorAmountAvailable;

	public inventoryListManager mainInventoryManager;
	public gameManager gameSystemManager;

	public bool loadCurrentVendorInventoryFromSaveFile;
	public bool saveCurrentVendorInventoryToSaveFile;

	public void setNewInventoryListManagerList (List<inventoryListElement> newList)
	{
		inventoryListManagerList = newList;
	}

	void Start ()
	{
		setInventoryFromInventoryListManager ();
	}

	public void setInventoryFromInventoryListManager ()
	{
		List<inventoryCategoryInfo> inventoryCategoryInfoList = mainInventoryManager.inventoryCategoryInfoList;

		int inventoryListManagerListCount = inventoryListManagerList.Count;

		for (int i = 0; i < inventoryListManagerListCount; i++) {

			inventoryListElement currentElement = inventoryListManagerList [i];

			if (currentElement.addObjectToList) {
				inventoryInfo currentInventoryInfo = 
					inventoryCategoryInfoList [currentElement.categoryIndex].inventoryList [currentElement.elementIndex];

				if (currentInventoryInfo != null) {
					inventoryInfo newInventoryInfo = new inventoryInfo (currentInventoryInfo);
					newInventoryInfo.Name = currentInventoryInfo.Name;
					newInventoryInfo.amount = currentElement.amount;

					float buyPrice = currentElement.vendorPrice;

					if (useGeneralBuyPriceMultiplier) {
						buyPrice = currentInventoryInfo.vendorPrice * generalBuyPriceMultiplayerPercentage;
					}
					newInventoryInfo.vendorPrice = buyPrice;

					float sellPrice = currentElement.sellPrice;

					if (useGeneralBuyPriceMultiplier) {
						sellPrice = currentInventoryInfo.sellPrice * generalSellPriceMultiplayerPercentage;
					}
					newInventoryInfo.sellPrice = sellPrice;

					newInventoryInfo.infiniteVendorAmountAvailable = infiniteVendorAmountAvailable || currentElement.infiniteAmount;

					if (currentElement.useMinLevelToBuy) {
						newInventoryInfo.useMinLevelToBuy = true;
						newInventoryInfo.minLevelToBuy = currentElement.minLevelToBuy;
					}

					newInventoryInfo.spawnObject = currentElement.spawnObject;

					vendorInventoryList.Add (newInventoryInfo);
				}
			}
		}
	}

	public List<inventoryInfo> getVendorInventoryList ()
	{
		return vendorInventoryList;
	}

	public void setVendorInventoryList (List<inventoryInfo> newVendorInventoryList)
	{
		vendorInventoryList = new List<inventoryInfo> (newVendorInventoryList);
	}

	public void getInventoryListManagerList ()
	{
		inventoryManagerListString = new string[mainInventoryManager.inventoryCategoryInfoList.Count];

		for (int i = 0; i < inventoryManagerListString.Length; i++) {
			inventoryManagerListString [i] = mainInventoryManager.inventoryCategoryInfoList [i].Name;
		}

		inventoryManagerStringInfoList.Clear ();

		for (int i = 0; i < mainInventoryManager.inventoryCategoryInfoList.Count; i++) {

			inventoryManagerStringInfo newInventoryManagerStringInfoo = new inventoryManagerStringInfo ();
			newInventoryManagerStringInfoo.Name = mainInventoryManager.inventoryCategoryInfoList [i].Name;

			newInventoryManagerStringInfoo.inventoryManagerListString = new string[mainInventoryManager.inventoryCategoryInfoList [i].inventoryList.Count];

			for (int j = 0; j < mainInventoryManager.inventoryCategoryInfoList [i].inventoryList.Count; j++) {
				string newName = mainInventoryManager.inventoryCategoryInfoList [i].inventoryList [j].Name;
				newInventoryManagerStringInfoo.inventoryManagerListString [j] = newName;
			}

			inventoryManagerStringInfoList.Add (newInventoryManagerStringInfoo);
		}

		updateComponent ();
	}

	public void setInventoryObjectListNames ()
	{
		for (int i = 0; i < inventoryListManagerList.Count; i++) {
			inventoryListManagerList [i].Name = inventoryListManagerList [i].inventoryObjectName;
		}

		updateComponent ();
	}

	public void addNewInventoryObjectToInventoryListManagerList ()
	{
		inventoryListElement newInventoryListElement = new inventoryListElement ();

		newInventoryListElement.Name = "New Object";

		inventoryListManagerList.Add (newInventoryListElement);

		updateComponent ();
	}

	public void saveCurrentVendorListToFile ()
	{
		if (gameSystemManager == null) {
			gameSystemManager = FindObjectOfType<gameManager> ();
		}

		if (gameSystemManager != null) {
			gameSystemManager.saveGameInfoFromEditor ("Vendor");
	
			print ("Inventory Vendor List Saved");

			updateComponent ();
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class inventoryManagerStringInfo
	{
		public string Name;
		public string[] inventoryManagerListString;
	}
}