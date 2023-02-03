using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryManagerInfo : MonoBehaviour
{
	public bool saveCurrentPlayerInventoryToSaveFile = true;
	public bool loadCurrentPlayerInventoryFromSaveFile = true;

	public bool setInfiniteSlotValuesOnSaveLoad = true;

	public int inventorySlotAmount;

	public bool infiniteSlots;

	public List<inventoryInfo> inventoryList = new List<inventoryInfo> ();

	public bool isLoadingGame;


	public virtual bool isSaveCurrentPlayerInventoryToSaveFileActive ()
	{
		return saveCurrentPlayerInventoryToSaveFile;
	}

	public virtual bool isLoadCurrentPlayerInventoryFromSaveFileActive ()
	{
		return loadCurrentPlayerInventoryFromSaveFile;
	}

	public virtual void setInventorySlotAmountValue (int newValue)
	{
		inventorySlotAmount = newValue;
	}

	public virtual int getInventorySlotAmount ()
	{
		return inventorySlotAmount;
	}

	public virtual void setInfiniteSlotsState (bool state)
	{
		infiniteSlots = state;
	}

	public virtual bool isSetInfiniteSlotValuesOnSaveLoadActive ()
	{
		return setInfiniteSlotValuesOnSaveLoad;
	}

	public virtual bool isInfiniteSlotsActive ()
	{
		return infiniteSlots;
	}

	public virtual void initializeInventoryValues ()
	{
	
	}

	public virtual List<inventoryInfo> getInventoryList ()
	{
		return inventoryList;
	}

	public virtual List<inventoryListElement> getCurrentInventoryListManagerList ()
	{
		return null;
	}

	public virtual void setNewInventoryListManagerList (List<inventoryListElement> newList)
	{
		
	}
}
