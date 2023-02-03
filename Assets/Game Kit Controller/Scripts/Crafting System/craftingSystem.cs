using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class craftingSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool craftingSystemEnabled;

	bool mainCraftingUISystemAssigned;

	public string craftingSystemMenuName = "Crafting System Menu";

	[Space]
	[Header ("Blueprints/recipes unlocked")]
	[Space]

	public bool useOnlyBlueprintsUnlocked;
	public List<string> blueprintsUnlockedList = new List<string> ();

	[Space]
	[Header ("Categories To Craft Available")]
	[Space]

	public bool allowAllObjectCategoriesToCraftAtAnyMomentEnabled = true;

	public List<string> objectCategoriesToCraftAvailableAtAnyMoment = new List<string> ();

	[Space]
	[Header ("Animation Settings")]
	[Space]

	public bool useAnimationOnCraftObjectAnywhere;

	public string animationNameOnCraftAnywhere = "Craft Simple Object";

	public bool useAnimationOnCraftObjectOnWorkbench;

	public string animationNameOnCraftOnWorkbench = "Craft On Workbench";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool menuOpened;

	public bool menuOpenedFromWorkbench;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnOpenMenu;
	public UnityEvent eventOnCloseMenu;

	[Space]

	public UnityEvent eventToStopUsingWorkbenchOnDamageReceived;

	[Space]

	public UnityEvent eventOnOpenWorkbench;
	public UnityEvent eventOnCloseWorkbench;

	[Space]
	[Header ("Components")]
	[Space]

	public craftingUISystem mainCraftingUISystem;

	public inventoryManager mainInventorymanager;

	public menuPause pauseManager;

	public GameObject playerGameObject;

	public Transform positionToSpawnObjectsIfNotSpaceOnInventory;

	public playerController mainPlayerController;

	public craftingPlacementSystem mainCraftingPlacementSystem;

	public objectsStatsSystem mainObjectsStatsSystem;

	public playerStatsSystem mainPlayerStatsSystem;


	void Start ()
	{
		if (!craftingSystemEnabled) {
			return;
		}

		if (!mainCraftingUISystemAssigned) {
			checkAssignCraftingUISystem ();
		}
	}

	void checkAssignCraftingUISystem ()
	{
		if (!mainCraftingUISystemAssigned) {
			ingameMenuPanel currentIngameMenuPanel = pauseManager.getIngameMenuPanelByName (craftingSystemMenuName);

			if (currentIngameMenuPanel == null) {
				pauseManager.checkcreateIngameMenuPanel (craftingSystemMenuName);

				currentIngameMenuPanel = pauseManager.getIngameMenuPanelByName (craftingSystemMenuName);
			}

			if (currentIngameMenuPanel != null) {
				mainCraftingUISystem = currentIngameMenuPanel.GetComponent<craftingUISystem> ();

				mainCraftingUISystemAssigned = true;
			}

			if (mainCraftingUISystemAssigned) {
				setUseOnlyBlueprintsUnlockedState (useOnlyBlueprintsUnlocked);

				if (useOnlyBlueprintsUnlocked) {
					setBlueprintsUnlockedListValue (blueprintsUnlockedList);
				}
			}
		}
	}

	public void openOrCloseCraftingMenu (bool state)
	{
		if (menuOpened == state) {
			return;
		}

		menuOpened = state;

		if (menuOpened) {
			if (!mainCraftingUISystemAssigned) {
				checkAssignCraftingUISystem ();
			}
		} else {

		}

		checkEventOnStateChange (menuOpened);
	}

	void checkEventOnStateChange (bool state)
	{
		if (state) {
			eventOnOpenMenu.Invoke ();
		} else {
			eventOnCloseMenu.Invoke ();
		}

		if (menuOpenedFromWorkbench) {
			if (state) {
				eventOnOpenWorkbench.Invoke ();
			} else {
				eventOnCloseWorkbench.Invoke ();
			}
		}
	}

	public void checkStateOnCraftObject ()
	{
		if (useAnimationOnCraftObjectAnywhere || useAnimationOnCraftObjectOnWorkbench) {
			if (menuOpenedFromWorkbench) {
				if (useAnimationOnCraftObjectOnWorkbench) {
					mainPlayerController.playerCrossFadeInFixedTime (animationNameOnCraftOnWorkbench);
				}
			} else {
				if (useAnimationOnCraftObjectAnywhere) {
					mainPlayerController.playerCrossFadeInFixedTime (animationNameOnCraftAnywhere);
				}
			}
		}
	}

	//Get/set inventory info functions
	public int getInventoryObjectAmountByName (string inventoryObjectName)
	{
		return mainInventorymanager.getInventoryObjectAmountByName (inventoryObjectName);
	}

	public Texture getInventoryObjectIconByName (string inventoryObjectName)
	{
		return mainInventorymanager.getInventoryObjectIconByName (inventoryObjectName);
	}

	public void removeObjectAmountFromInventoryByName (string objectName, int amountToMove)
	{
		mainInventorymanager.removeObjectAmountFromInventoryByName (objectName, amountToMove);
	}

	public void removeObjectAmountFromInventoryByIndex (int objectIndex, int amountToMove)
	{
		mainInventorymanager.removeObjectAmountFromInventory (objectIndex, amountToMove);
	}

	public void giveInventoryObjectToCharacter (string objectName, int objectAmount)
	{
		applyDamage.giveInventoryObjectToCharacter (playerGameObject, objectName, objectAmount, 
			positionToSpawnObjectsIfNotSpaceOnInventory, 0, 2, ForceMode.Force, 0, false);
	}

	public List<inventoryInfo> getInventoryList ()
	{
		return mainInventorymanager.getInventoryList ();
	}

	public inventoryInfo getInventoryInfoByName (string objectName)
	{
		return mainInventorymanager.getInventoryInfoByName (objectName);
	}

	public inventoryInfo getInventoryInfoByIndex (int objectIndex)
	{
		return mainInventorymanager.getInventoryInfoByIndex (objectIndex);
	}

	public bool repairDurabilityObjectByIndex (int objectIndex)
	{
		return mainInventorymanager.repairDurabilityObjectByIndex (objectIndex);
	}

	public bool isObjectBroken (int objectIndex)
	{
		return mainInventorymanager.isObjectBroken (objectIndex);
	}

	public bool isObjectDurabilityComplete (int objectIndex)
	{
		return mainInventorymanager.isObjectDurabilityComplete (objectIndex);
	}

	public GameObject getInventoryMeshByName (string objectName)
	{
		return mainInventorymanager.getInventoryMeshByName (objectName);
	}

	public GameObject getCurrentObjectToPlace ()
	{
		return mainCraftingUISystem.getCurrentObjectToPlace ();
	}

	public GameObject getCurrentObjectToPlaceByName (string objectName)
	{
		return mainCraftingUISystem.getCurrentObjectToPlaceByName (objectName);
	}

	public LayerMask getCurrentObjectLayerMaskToAttachObjectByName (string objectName)
	{
		return mainCraftingUISystem.getCurrentObjectLayerMaskToAttachObjectByName (objectName);
	}

	public Vector3 getCurrentObjectToPlacePositionOffsetByName (string objectName)
	{
		return mainCraftingUISystem.getCurrentObjectToPlacePositionOffsetByName (objectName);
	}

	public bool checkIfCurrentObjectToPlaceUseCustomLayerMaskByName (string objectName)
	{
		return mainCraftingUISystem.checkIfCurrentObjectToPlaceUseCustomLayerMaskByName (objectName);
	}

	public LayerMask getCurrentObjectCustomLayerMaskToPlaceObjectByName (string objectName)
	{
		return mainCraftingUISystem.getCurrentObjectCustomLayerMaskToPlaceObjectByName (objectName);
	}

	public void getCurrentObjectCanBeRotatedValuesByName (string objectName, ref bool objectCanBeRotatedOnYAxis, ref bool objectCanBeRotatedOnXAxis)
	{
		mainCraftingUISystem.getCurrentObjectCanBeRotatedValuesByName (objectName, ref objectCanBeRotatedOnYAxis, ref objectCanBeRotatedOnXAxis);
	}


	public string getCurrentObjectSelectedName ()
	{
		return mainCraftingUISystem.getCurrentObjectSelectedName ();
	}

	public void setPlacementActiveState (bool state)
	{
		if (mainCraftingUISystem.currentObjectCanBePlaced) {
			if (state) {
				GameObject currentObjectMesh = getInventoryMeshByName (getCurrentObjectSelectedName ());

				if (currentObjectMesh != null) {
					mainCraftingPlacementSystem.setCurrentObjectToPlaceMesh (currentObjectMesh);
				} else {
					return;
				}
			}
		}

		if (state) {
			mainCraftingUISystem.openOrCloseMenuFromTouch ();
		}

		mainCraftingPlacementSystem.setPlacementActiveState (state);
	}

	public bool checkIfStatValueAvailable (string statName, int statAmount)
	{
		return mainPlayerStatsSystem.checkIfStatValueAvailable (statName, statAmount);
	}

	public void addOrRemovePlayerStatAmount (string statName, int statAmount)
	{
		mainPlayerStatsSystem.addOrRemovePlayerStatAmount (statName, statAmount);
	}

	//blueprints functions
	public void setUseOnlyBlueprintsUnlockedState (bool state)
	{
		useOnlyBlueprintsUnlocked = state;

		if (mainCraftingUISystem != null) {
			mainCraftingUISystem.setUseOnlyBlueprintsUnlockedState (state);
		}
	}

	public bool isUseOnlyBlueprintsUnlockedActive ()
	{
		return useOnlyBlueprintsUnlocked;
	}

	public void setBlueprintsUnlockedListValue (List<string> newBlueprintsUnlockedList)
	{
		blueprintsUnlockedList = new List<string> (newBlueprintsUnlockedList);

		if (mainCraftingUISystem != null) {
			mainCraftingUISystem.setBlueprintsUnlockedListValue (newBlueprintsUnlockedList);
		}
	}

	public List<string> getBlueprintsUnlockedListValue ()
	{
		return blueprintsUnlockedList;
	}

	public void addNewBlueprintsUnlockedElement (string newBlueprintsUnlockedElement)
	{
		if (!blueprintsUnlockedList.Contains (newBlueprintsUnlockedElement)) {
			blueprintsUnlockedList.Add (newBlueprintsUnlockedElement);
		}

		if (mainCraftingUISystem != null) {
			mainCraftingUISystem.addNewBlueprintsUnlockedElement (newBlueprintsUnlockedElement);
		}
	}

	public void setObjectCategoriesToCraftAvailableAtAnyMomentValue (List<string> newList)
	{
		objectCategoriesToCraftAvailableAtAnyMoment = newList;
	}

	public List<string> getObjectCategoriesToCraftAvailableAtAnyMomentValue ()
	{
		return objectCategoriesToCraftAvailableAtAnyMoment;
	}

	public void addObjectCategoriesToCraftAvailableAtAnyMomentElement (string newElement)
	{
		if (!objectCategoriesToCraftAvailableAtAnyMoment.Contains (newElement)) {
			objectCategoriesToCraftAvailableAtAnyMoment.Add (newElement);
		}
	}

	public List<craftObjectInTimeSimpleInfo> getCraftObjectInTimeInfoList ()
	{
		return mainCraftingUISystem.getCraftObjectInTimeInfoList ();
	}

	public bool anyObjectToCraftInTimeActive ()
	{
		return mainCraftingUISystem.anyObjectToCraftInTimeActive ();
	}

	public void setCraftObjectInTimeInfoList (List<craftObjectInTimeSimpleInfo> newCraftObjectInTimeSimpleInfoList)
	{
		mainCraftingUISystem.setCraftObjectInTimeInfoList (newCraftObjectInTimeSimpleInfoList);
	}

	public void setOpenFromWorkbenchState (bool state, List<string> newObjectCategoriesToCraftAvailableOnCurrentBench)
	{
		if (mainCraftingUISystem != null) {
			mainCraftingUISystem.setOpenFromWorkbenchState (state, newObjectCategoriesToCraftAvailableOnCurrentBench);

			menuOpenedFromWorkbench = state;
		}
	}

	public void setCurrentcurrentCraftingWorkbenchSystem (craftingWorkbenchSystem newCraftingWorkbenchSystem)
	{
		if (mainCraftingUISystem != null) {
			mainCraftingUISystem.setCurrentcurrentCraftingWorkbenchSystem (newCraftingWorkbenchSystem);
		}
	}

	public void stopUsingWorkbenchOnDamageReceived ()
	{
		if (menuOpened) {
			if (menuOpenedFromWorkbench) {
				eventToStopUsingWorkbenchOnDamageReceived.Invoke ();

				mainCraftingUISystem.checkEventToStopUsingWorkbenchOnDamageReceived ();
			}
		}
	}

	public List<objectStatInfo> getStatsFromObjectByName (string objectName)
	{
		return mainObjectsStatsSystem.getStatsFromObjectByName (objectName);
	}

	public bool objectCanBeUpgraded (string objectName)
	{
		return mainObjectsStatsSystem.objectCanBeUpgraded (objectName);
	}

	//Editor Functions
	public void setCraftingSystemEnabledStateFromEditor (bool state)
	{
		craftingSystemEnabled = state;

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Crafting System State", gameObject);
	}
}