using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class inventoryInfo
{
	//Main settings
	public string Name;
	public GameObject inventoryGameObject;
	[TextArea (3, 10)] public string objectInfo;
	public Texture icon;
	public int amount;
	public int amountPerUnit;
	public bool infiniteAmount;

	public bool storeTotalAmountPerUnit;

	public bool showAmountPerUnitInAmountText;


	//Special settings to use, equi, drop, combine, etc...

	public bool canBeUsed;
	public bool canBeEquiped;
	public bool canBeDropped;
	public bool canBeCombined;

	public bool canBeExamined = true;

	public bool canBeDiscarded;

	public bool hideSlotOnMenu;


	//Behavior elements

	public bool useNewBehaviorOnUse;
	[TextArea (3, 10)] public string newBehaviorOnUseMessage;
	[TextArea (3, 10)] public string newBehaviorOnUnableToUseMessage;

	public bool useNewBehaviorOnCombine;
	[TextArea (3, 10)] public string newBehaviorOnCombineMessage;

	public bool useOneUnitOnNewBehaviourCombine = true;

	public bool useSoundOnUseObject;
	public AudioClip soundOnUseObject;


	//States

	public bool isEquiped;

	public int quickAccessSlotIndex = -1;


	public bool useDurability;
	public float durabilityAmount = 100;

	public float maxDurabilityAmount = 100;

	public bool objectIsBroken;

	public float sellMultiplierIfObjectIsBroken = 1;


	//Weapon settings

	public bool isWeapon;
	public bool isMeleeWeapon;

	public bool isArmorClothAccessory;

	public int projectilesInMagazine = -1;

	public weaponObjectInfo mainWeaponObjectInfo;


	//Combine object settings

	public GameObject objectToCombine;
	public GameObject combinedObject;
	[TextArea (3, 10)] public string combinedObjectMessage;


	//Vendor settings

	public bool canBeSold = true;

	public float vendorPrice;
	public bool infiniteVendorAmountAvailable;

	public string categoryName;

	public int categoryIndex;
	public int elementIndex;

	public float sellPrice;

	public bool useMinLevelToBuy;
	public float minLevelToBuy;


	//Other settings

	public float weight;

	public bool spawnObject;

	public bool useCustomObjectToDrop;
	public GameObject customObjectToDrop;
	public Vector3 dropObjectOffset;

	public bool cantBeStoredOnInventory;

	public bool canBeHeld;

	public bool canBePlaceOnQuickAccessSlot;

	public bool setMaximumAmountPerSlot;
	public int maximumAmountPerSlot;


	//Editor and prefabs settings
	public Button button;
	public inventoryMenuIconElement menuIconElement;
	public GameObject inventoryObjectPrefab;

	public vendorObjectSlotPanelInfo currentVendorObjectSlotPanelInfo;

	public GameObject emptyInventoryPrefab;

	public inventoryInfo (inventoryInfo obj)
	{
		Name = obj.Name;
		inventoryGameObject = obj.inventoryGameObject;
		objectInfo = obj.objectInfo;
		icon = obj.icon;
		amount = obj.amount;
		amountPerUnit = obj.amountPerUnit;

		infiniteAmount = obj.infiniteAmount;

		storeTotalAmountPerUnit = obj.storeTotalAmountPerUnit;

		showAmountPerUnitInAmountText = obj.showAmountPerUnitInAmountText;

		canBeUsed = obj.canBeUsed;
		canBeEquiped = obj.canBeEquiped;
		canBeDropped = obj.canBeDropped;
		canBeCombined = obj.canBeCombined;

		canBeExamined = obj.canBeExamined;

		hideSlotOnMenu = obj.hideSlotOnMenu;

		useNewBehaviorOnUse = obj.useNewBehaviorOnUse;
		newBehaviorOnUseMessage = obj.newBehaviorOnUseMessage;
		newBehaviorOnUnableToUseMessage = obj.newBehaviorOnUnableToUseMessage;

		useOneUnitOnNewBehaviourCombine = obj.useOneUnitOnNewBehaviourCombine;

		useNewBehaviorOnCombine = obj.useNewBehaviorOnCombine;
		newBehaviorOnCombineMessage = obj.newBehaviorOnCombineMessage;

		useSoundOnUseObject = obj.useSoundOnUseObject;
		soundOnUseObject = obj.soundOnUseObject;

		canBeDiscarded = obj.canBeDiscarded;

		canBeSold = obj.canBeSold;

		vendorPrice = obj.vendorPrice;
		infiniteVendorAmountAvailable = obj.infiniteVendorAmountAvailable;

		categoryName = obj.categoryName;

		categoryIndex = obj.categoryIndex;
		elementIndex = obj.elementIndex; 

		sellPrice = obj.sellPrice;

		useMinLevelToBuy = obj.useMinLevelToBuy;
		minLevelToBuy = obj.minLevelToBuy;

		isEquiped = obj.isEquiped;

		quickAccessSlotIndex = obj.quickAccessSlotIndex;

		useDurability = obj.useDurability;
		durabilityAmount = obj.durabilityAmount;

		maxDurabilityAmount = obj.maxDurabilityAmount;

		objectIsBroken = obj.objectIsBroken;

		sellMultiplierIfObjectIsBroken = obj.sellMultiplierIfObjectIsBroken;


		isWeapon = obj.isWeapon;
		isMeleeWeapon = obj.isMeleeWeapon;

		isArmorClothAccessory = obj.isArmorClothAccessory;

		projectilesInMagazine = obj.projectilesInMagazine;

		mainWeaponObjectInfo = obj.mainWeaponObjectInfo;

		weight = obj.weight;

		objectToCombine = obj.objectToCombine;
		combinedObject = obj.combinedObject;
		combinedObjectMessage = obj.combinedObjectMessage;
		button = obj.button;

		spawnObject = obj.spawnObject;

		useCustomObjectToDrop = obj.useCustomObjectToDrop;
		customObjectToDrop = obj.customObjectToDrop;
		dropObjectOffset = obj.dropObjectOffset;

		cantBeStoredOnInventory = obj.cantBeStoredOnInventory;

		canBeHeld = obj.canBeHeld;

		canBePlaceOnQuickAccessSlot = obj.canBePlaceOnQuickAccessSlot;

		setMaximumAmountPerSlot = obj.setMaximumAmountPerSlot;
		maximumAmountPerSlot = obj.maximumAmountPerSlot;
	}

	public inventoryInfo ()
	{
		Name = "New Object";
		objectInfo = "New Description";
	}

	public void resetInventoryInfo ()
	{
		Name = "Empty Slot";
		inventoryGameObject = null;
		objectInfo = "It is an empty slot";
		icon = null;
		amount = 0;
		amountPerUnit = 0;

		infiniteAmount = false;

		storeTotalAmountPerUnit = false;

		showAmountPerUnitInAmountText = false;

		canBeUsed = false;
		canBeDropped = false;
		canBeEquiped = false;
		canBeCombined = false;

		canBeExamined = false;

		hideSlotOnMenu = false;

		useNewBehaviorOnUse = false;
		newBehaviorOnUseMessage = "";
		newBehaviorOnUnableToUseMessage = "";

		useOneUnitOnNewBehaviourCombine = true;

		useNewBehaviorOnCombine = false;
		newBehaviorOnCombineMessage = "";

		useSoundOnUseObject = false;
		soundOnUseObject = null;

		canBeDiscarded = false;

		canBeSold = true;
		vendorPrice = 0;

		sellPrice = 0;

		useMinLevelToBuy = false;
		minLevelToBuy = 0;

		weight = 0;

		infiniteVendorAmountAvailable = false;

		categoryName = "";

		categoryIndex = 0;
		elementIndex = 0; 

		isEquiped = false;

		quickAccessSlotIndex = -1;

		useDurability = false;
		durabilityAmount = 100;

		maxDurabilityAmount = 100;

		objectIsBroken = false;

		sellMultiplierIfObjectIsBroken = 1;

		isWeapon = false;
		isMeleeWeapon = false;

		isArmorClothAccessory = false;

		projectilesInMagazine = -1;

		mainWeaponObjectInfo = null;

		objectToCombine = null;
		combinedObject = null;
		combinedObjectMessage = "";

		if (menuIconElement != null) {
			menuIconElement.icon.texture = null;
			menuIconElement.iconName.text = "Empty Slot";
			menuIconElement.amount.text = "0";
		}

		spawnObject = false;

		useCustomObjectToDrop = false;
		customObjectToDrop = null;
		dropObjectOffset = Vector3.zero;

		cantBeStoredOnInventory = false;

		canBeHeld = false;

		canBePlaceOnQuickAccessSlot = false;

		setMaximumAmountPerSlot = false;
		maximumAmountPerSlot = 0;
	}

	public inventoryInfo copyInventoryObject (inventoryInfo objectToStore, inventoryInfo objectToCopy)
	{
		objectToStore.Name = objectToCopy.Name;
		objectToStore.inventoryGameObject = objectToCopy.inventoryGameObject;
		objectToStore.objectInfo = objectToCopy.objectInfo;
		objectToStore.icon = objectToCopy.icon;

		objectToStore.infiniteAmount = objectToCopy.infiniteAmount;

		objectToStore.storeTotalAmountPerUnit = objectToCopy.storeTotalAmountPerUnit;

		objectToStore.showAmountPerUnitInAmountText = objectToCopy.showAmountPerUnitInAmountText;

		objectToStore.canBeUsed = objectToCopy.canBeUsed;
		objectToStore.canBeEquiped = objectToCopy.canBeEquiped;
		objectToStore.canBeDropped = objectToCopy.canBeDropped;
		objectToStore.canBeCombined = objectToCopy.canBeCombined;

		objectToStore.canBeExamined = objectToCopy.canBeExamined;

		objectToStore.hideSlotOnMenu = objectToCopy.hideSlotOnMenu;

		objectToStore.useNewBehaviorOnUse = objectToCopy.useNewBehaviorOnUse;
		objectToStore.newBehaviorOnUseMessage = objectToCopy.newBehaviorOnUseMessage;
		objectToStore.newBehaviorOnUnableToUseMessage = objectToCopy.newBehaviorOnUnableToUseMessage;

		objectToStore.useOneUnitOnNewBehaviourCombine = objectToCopy.useOneUnitOnNewBehaviourCombine;	

		objectToStore.useNewBehaviorOnCombine = objectToCopy.useNewBehaviorOnCombine;
		objectToStore.newBehaviorOnCombineMessage = objectToCopy.newBehaviorOnCombineMessage;

		useSoundOnUseObject = objectToCopy.useSoundOnUseObject;
		soundOnUseObject = objectToCopy.soundOnUseObject;

		objectToStore.isEquiped = objectToCopy.isEquiped;

		objectToStore.quickAccessSlotIndex = objectToCopy.quickAccessSlotIndex;

		useDurability = objectToCopy.useDurability;
		durabilityAmount = objectToCopy.durabilityAmount;

		maxDurabilityAmount = objectToCopy.maxDurabilityAmount;

		objectIsBroken = objectToCopy.objectIsBroken;

		sellMultiplierIfObjectIsBroken = objectToCopy.sellMultiplierIfObjectIsBroken;

		objectToStore.isWeapon = objectToCopy.isWeapon;
		objectToStore.isMeleeWeapon = objectToCopy.isMeleeWeapon;

		objectToStore.isArmorClothAccessory = objectToCopy.isArmorClothAccessory;

		objectToStore.objectToCombine = objectToCopy.objectToCombine;
		objectToStore.combinedObject = objectToCopy.combinedObject;
		objectToStore.combinedObjectMessage = objectToCopy.combinedObjectMessage;

		objectToStore.menuIconElement.icon.texture = objectToCopy.icon;

		objectToStore.canBeDiscarded = objectToCopy.canBeDiscarded;

		objectToStore.canBeSold = objectToCopy.canBeSold;
		objectToStore.vendorPrice = objectToCopy.vendorPrice;
		objectToStore.infiniteVendorAmountAvailable = objectToCopy.infiniteVendorAmountAvailable;
		objectToStore.categoryName = objectToCopy.categoryName;

		objectToStore.categoryIndex = objectToCopy.categoryIndex;
		objectToStore.elementIndex = objectToCopy.elementIndex;

		objectToStore.sellPrice = objectToCopy.sellPrice;

		objectToStore.useMinLevelToBuy = objectToCopy.useMinLevelToBuy;
		objectToStore.minLevelToBuy = objectToCopy.minLevelToBuy;

		objectToStore.weight = objectToCopy.weight;

		objectToStore.spawnObject = objectToCopy.spawnObject;

		useCustomObjectToDrop = objectToCopy.useCustomObjectToDrop;
		customObjectToDrop = objectToCopy.customObjectToDrop;
		dropObjectOffset = objectToCopy.dropObjectOffset;

		objectToStore.cantBeStoredOnInventory = objectToCopy.cantBeStoredOnInventory;

		objectToStore.canBeHeld = objectToCopy.canBeHeld;

		objectToStore.canBePlaceOnQuickAccessSlot = objectToCopy.canBePlaceOnQuickAccessSlot;

		setMaximumAmountPerSlot = objectToCopy.setMaximumAmountPerSlot;
		maximumAmountPerSlot = objectToCopy.maximumAmountPerSlot;

		return objectToStore;
	}
}
