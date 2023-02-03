using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupType : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool storePickupOnInventory;

	public bool useInventoryObjectWhenPicked;

	[Space]
	[Header ("Pickup Message Settings")]
	[Space]

	public bool useCustomPickupMessage = true;

	[Space]

	[TextArea (5, 15)] public string objectTakenAsPickupMessage;
	[TextArea (5, 15)] public string objectTakenAsInventoryMessage;

	[Space]

	[TextArea (2, 6)] public string explanation = "Use -AMOUNT- in the position of the text \n to replace the amount picked if you need it";

	[Space]
	[Header ("Enable Abilities on Use Take Pickup Settings")]
	[Space]

	public bool useAbilitiesListToEnableOnTakePickup;

	[Space]

	public List<string> abilitiesListToEnableOnTakePickup = new List<string> ();

	[Space]
	[Header ("Activate Abilities on Take Pickup Settings")]
	[Space]

	public bool activateAbilityOnTakePickup;

	[Space]

	public string abilityNameToActiveOnTakePickup;
	public bool abilityIsTemporallyActivated;

	[Space]
	[Header ("Crafting Settings")]
	[Space]

	public bool getCraftingRecipes;
	public List<string> craftingRecipesList = new List<string> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Components")]
	[Space]

	public pickUpObject mainPickupObject;

	protected bool canPickCurrentObject;

	protected bool finderIsPlayer;
	protected bool finderIsVehicle;
	protected bool finderIsCharacter;

	protected GameObject player;
	protected GameObject vehicle;
	protected GameObject npc;

	protected bool takeWithTrigger;

	protected int amountTaken;

	public void takePickupByButton ()
	{
		assignPickupObjectState ();

		if (checkIfCanBePicked ()) {
			mainPickupObject.confirmTakePickupByButton ();
		}
	}

	public void takePickupByTrigger ()
	{
		assignPickupObjectState ();

		if (checkIfCanBePicked ()) {
			mainPickupObject.confirmTakePickupByTrigger ();
		}
	}

	public void assignPickupObjectState ()
	{
		canPickCurrentObject = false;

		finderIsPlayer = mainPickupObject.finderIsPlayer;
		finderIsVehicle = mainPickupObject.finderIsVehicle;
		finderIsCharacter = mainPickupObject.finderIsCharacter;

		player = mainPickupObject.player;
		vehicle = mainPickupObject.vehicle;
		npc = mainPickupObject.npc;

		takeWithTrigger = mainPickupObject.takeWithTrigger;
	}

	public virtual bool checkIfCanBePicked ()
	{
		return canPickCurrentObject;
	}

	public virtual void confirmTakePickup ()
	{
		
	}

	public virtual void showPickupTakenMessage (int amountToShow)
	{
		string currentPickedMessage = objectTakenAsPickupMessage;

		if (storePickupOnInventory) {
			currentPickedMessage = objectTakenAsInventoryMessage;
		}

		if (currentPickedMessage != "") {
			string amountString = amountToShow.ToString ();

			currentPickedMessage = currentPickedMessage.Replace ("-AMOUNT-", amountString);
		
			mainPickupObject.showRecieveInfo (currentPickedMessage);
		}
	}

	public virtual void showPickupTakenMessage (string messageToUse)
	{
		if (messageToUse != "") {
			mainPickupObject.showRecieveInfo (messageToUse);
		}
	}

	public  void checkIfUseInventoryObjectWhenPicked ()
	{
		if (storePickupOnInventory && useInventoryObjectWhenPicked) {
			if (mainPickupObject.playerInventoryManager != null) {
				mainPickupObject.playerInventoryManager.useInventoryObjectByName (mainPickupObject.inventoryObjectManager.inventoryObjectInfo.Name, 1);
			}
		}
	}

	public virtual void checkIfEnableAbilitiesOnTakePickup (GameObject currentPlayer)
	{
		if (useAbilitiesListToEnableOnTakePickup && currentPlayer != null) {
			GKC_Utils.enableOrDisableAbilityGroupByName (currentPlayer.transform, true, abilitiesListToEnableOnTakePickup);
		}
	}

	public virtual void checkIfActivateAbilitiesOnTakePickup (GameObject currentPlayer)
	{
		if (activateAbilityOnTakePickup && currentPlayer != null) {
			GKC_Utils.activateAbilityByName (currentPlayer.transform, abilityNameToActiveOnTakePickup, abilityIsTemporallyActivated);
		}
	}

	public virtual void checkIfaddNewBlueprintsUnlockedList (GameObject currentPlayer)
	{
		if (getCraftingRecipes && currentPlayer != null) {
			GKC_Utils.addNewBlueprintsUnlockedList (currentPlayer, craftingRecipesList);
		}
	}
}
