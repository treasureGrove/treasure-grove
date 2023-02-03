using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryBankManager : MonoBehaviour
{
	public List<inventoryListElement> inventoryListManagerList = new List<inventoryListElement> ();
	public List<inventoryInfo> bankInventoryList = new List<inventoryInfo> ();

	public string[] inventoryManagerListString;
	public List<inventoryManagerStringInfo> inventoryManagerStringInfoList = new List<inventoryManagerStringInfo> ();

	public bool loadCurrentBankInventoryFromSaveFile;
	public bool saveCurrentBankInventoryToSaveFile;

	public inventoryListManager mainInventoryManager;

	public gameManager gameSystemManager;

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
		int inventoryListManagerListCount = inventoryListManagerList.Count;

		List<inventoryCategoryInfo> inventoryCategoryInfoList = mainInventoryManager.inventoryCategoryInfoList;

		for (int i = 0; i < inventoryListManagerListCount; i++) {
			inventoryListElement currentElement = inventoryListManagerList [i];

			inventoryInfo currentInventoryInfo = inventoryCategoryInfoList [currentElement.categoryIndex].inventoryList [currentElement.elementIndex];

			if (currentInventoryInfo != null) {
				inventoryInfo newInventoryInfo = new inventoryInfo (currentInventoryInfo);
				newInventoryInfo.Name = currentInventoryInfo.Name;
				newInventoryInfo.amount = currentElement.amount;

				bankInventoryList.Add (newInventoryInfo);
			}
		}
	}

	public List<inventoryInfo> getBankInventoryList ()
	{
		return bankInventoryList;
	}

	public void addInventoryObjectByName (string objectName, int amountToAdd)
	{
		int inventoryListCount = bankInventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = bankInventoryList [i];

			if (currentInventoryInfo.Name.Equals (objectName)) {

				currentInventoryInfo.amount += amountToAdd;

				return;
			}
		}

		inventoryInfo inventoryInfoToCheck = mainInventoryManager.getInventoryInfoFromName (objectName);

		if (inventoryInfoToCheck != null) {
			inventoryInfo newObjectToAdd = new inventoryInfo (inventoryInfoToCheck);
			newObjectToAdd.amount = amountToAdd;

			bankInventoryList.Add (newObjectToAdd);
		}
	}

	public int getInventoryObjectAmountByName (string inventoryObjectName)
	{
		int totalAmount = 0;

		int inventoryListCount = bankInventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = bankInventoryList [i];

			if (currentInventoryInfo.Name.Equals (inventoryObjectName)) {
				totalAmount += currentInventoryInfo.amount;
			}
		}

		if (totalAmount > 0) {
			return totalAmount;
		}

		return -1;
	}

	public void removeObjectAmountFromInventory (string objectName, int amountToRemove)
	{
		int inventoryListCount = bankInventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			if (bankInventoryList [i].Name.Equals (objectName)) {
				bankInventoryList [i].amount -= amountToRemove;

				if (bankInventoryList [i].amount <= 0) {
					bankInventoryList.RemoveAt (i);
				}

				return;
			}
		}
	}

	//EDITOR FUNCTIONS
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

	public void saveCurrentInventoryListToFile ()
	{
		if (gameSystemManager == null) {
			gameSystemManager = FindObjectOfType<gameManager> ();
		}

		if (gameSystemManager != null) {
			gameSystemManager.saveGameInfoFromEditor ("Inventory Bank");

			print ("Inventory Bank List saved");

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