using UnityEngine;
using System.Collections;

public class inventoryObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public inventoryInfo inventoryObjectInfo;

	[Space]
	[Header ("Main Inventory Object Settings")]
	[Space]

	public objectOnInventory mainObjectOnInventory;

	[Space]
	[Header ("Inventory Settings")]
	[Space]

	public bool useZoomRange = true;
	public float maxZoom = 30;
	public float minZoom = 100;
	public float initialZoom = 46;

	public void useObjectOnNewBehavior (GameObject currentPlayer, int amountToUse)
	{
		if (mainObjectOnInventory != null) {
			mainObjectOnInventory.activateUseObjectActionOnInventory (currentPlayer, amountToUse);
		}
	}

	public void combineObjectsOnNewBehavior (GameObject currentPlayer, inventoryInfo inventoryInfoToUse)
	{
		if (mainObjectOnInventory != null) {
			mainObjectOnInventory.activateCombineObjectActionOnInventory (currentPlayer, inventoryInfoToUse);
		}
	}

	public void carryPhysicalObjectFromInventory (GameObject currentPlayer)
	{
		if (mainObjectOnInventory != null) {
			mainObjectOnInventory.carryPhysicalObjectFromInventory (currentPlayer);
		}
	}

	public void eventOnPickObjectNewBehaviour (GameObject currentPlayer)
	{
		if (mainObjectOnInventory != null) {
			mainObjectOnInventory.eventOnPickObject (currentPlayer);
		}
	}

	public void eventOnDropObjectNewBehaviour (GameObject currentPlayer)
	{
		if (mainObjectOnInventory != null) {
			mainObjectOnInventory.eventOnDropObject (currentPlayer);
		}
	}

	public bool setObjectEquippedStateOnInventoryOnNewBehavior (GameObject currentPlayer, bool state)
	{
		if (mainObjectOnInventory != null) {
			return mainObjectOnInventory.setObjectEquippedStateOnInventory (currentPlayer, state);
		}

		return false;
	}
}