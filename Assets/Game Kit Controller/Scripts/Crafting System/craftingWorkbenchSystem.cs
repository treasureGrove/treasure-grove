using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class craftingWorkbenchSystem : MonoBehaviour
{
	[Header ("Main Setting")]
	[Space]

	public int workbenchID;

	public bool setObjectCategoriesToCraftAvailableOnCurrentBench;

	public List<string> objectCategoriesToCraftAvailableOnCurrentBench = new List<string> ();

	[Space]
	[Header ("Repair/Disassemble/Upgrade Setting")]
	[Space]

	public bool repairObjectsOnlyOnWorkbenchEnabled = true;

	public bool disassembleObjectsOnlyOnWorkbenchEnabled = true;

	public bool upgradeObjectsOnlyOnWorkbencheEnabled = true;

	[Space]
	[Header ("Other Setting")]
	[Space]

	public bool showCurrentObjectMesh;

	public Transform currentObjectMeshPlaceTransform;

	[Space]
	[Header ("Inventory Bank Setting")]
	[Space]

	public bool storeCraftedObjectsOnInventoryBank;

	public inventoryBankSystem mainInventoryBankSystem;

	[Space]
	[Header ("Event Setting")]
	[Space]

	public UnityEvent eventToStopUsingWorkbenchOnDamageReceived;


	public void activateWorkbench (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				currentCraftingSystem.setCurrentcurrentCraftingWorkbenchSystem (this);

				if (setObjectCategoriesToCraftAvailableOnCurrentBench) {
					currentCraftingSystem.setOpenFromWorkbenchState (true, objectCategoriesToCraftAvailableOnCurrentBench);
				} else {
					currentCraftingSystem.setOpenFromWorkbenchState (true, null);
				}
			}
		}
	}

	public void deactivateWorkBench (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				currentCraftingSystem.setCurrentcurrentCraftingWorkbenchSystem (null);

				currentCraftingSystem.setOpenFromWorkbenchState (false, null);
			}
		}
	}

	public void addInventoryObjectByName (string objectName, int amountToMove)
	{
		mainInventoryBankSystem.addInventoryObjectByName (objectName, amountToMove);
	}

	public void checkEventToStopUsingWorkbenchOnDamageReceived ()
	{
		eventToStopUsingWorkbenchOnDamageReceived.Invoke ();
	}

	public int getWorkbenchID ()
	{
		return workbenchID;
	}
}
