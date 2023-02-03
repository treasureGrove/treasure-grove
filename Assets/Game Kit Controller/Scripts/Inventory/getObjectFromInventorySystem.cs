using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class getObjectFromInventorySystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string inventoryObjectName;

	public bool useInfiniteObjects;

	public bool checkInventoryManager = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public int extraInventoryObjectAmount;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnObjectFoundOnInventory;
	public UnityEvent eventOnObjectNotFoundOnInventory;

	public UnityEvent eventOnAmountAvailable;
	public UnityEvent eventOnAmountNotAvailable;

	public inventoryManager mainInventoryManager;

	public void addExtraInventoryObjectAmount (int extraAmount)
	{
		extraInventoryObjectAmount += extraAmount;

		if (extraInventoryObjectAmount < 0) {
			extraInventoryObjectAmount = 0;
		}
	}

	public void checkIfObjectFoundOnInventory ()
	{
		if (mainInventoryManager == null && checkInventoryManager) {
			return;
		}

		int remainAmount = 0;

		if (checkInventoryManager) {
			remainAmount = mainInventoryManager.getInventoryObjectAmountByName (inventoryObjectName);
		}

		if (remainAmount < 0) {
			remainAmount = 0;
		}

		if (useInfiniteObjects) {
			remainAmount = 1;
		}

		remainAmount += extraInventoryObjectAmount;

		if (remainAmount > 0) {
			eventOnObjectFoundOnInventory.Invoke ();

			if (showDebugPrint) {
				print ("using event on object found");
			}
		} else {
			eventOnObjectNotFoundOnInventory.Invoke ();

			if (showDebugPrint) {
				print ("using event on object not found");
			}
		}
	}

	public void useInventoryObject (int amountToUse)
	{
		if (mainInventoryManager == null && checkInventoryManager) {
			return;
		}

		int remainAmount = 0;

		if (checkInventoryManager) {
			remainAmount = mainInventoryManager.getInventoryObjectAmountByName (inventoryObjectName);
		}

		if (remainAmount < 0) {
			remainAmount = 0;
		}

		if (useInfiniteObjects) {
			remainAmount = 1;
		}

		remainAmount += extraInventoryObjectAmount;

		if (remainAmount >= amountToUse) {
			if (checkInventoryManager) {
				if (extraInventoryObjectAmount == 0) {
					mainInventoryManager.removeObjectAmountFromInventoryByName (inventoryObjectName, amountToUse);
				}
			}

			eventOnAmountAvailable.Invoke ();

			if (showDebugPrint) {
				print ("using event on amount available");
			}
		} else {
			eventOnAmountNotAvailable.Invoke ();

			if (showDebugPrint) {
				print ("using event on amount not available");
			}
		}
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		if (newPlayer != null) {
			playerComponentsManager currentPlayerComponentsManager = newPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				mainInventoryManager = currentPlayerComponentsManager.getInventoryManager ();
			}
		}
	}

	public void setNewInventoryObjectName (string newName)
	{
		inventoryObjectName = newName;
	}
}
