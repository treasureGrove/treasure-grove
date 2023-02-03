using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class craftedObjectPlacedHelper : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string objectName;

	public GameObject mainGameObject;

	public bool objectCanBeTakenBackToInventoryEnabled = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool objectPlaced;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventsOnPlaceOrTakeBackObject;
	public UnityEvent eventOnPlaceObject;
	public UnityEvent eventOnTakeBackObject;

	[Space]

	public bool useEventsToSendPlayerOnPlaceOrTakeBackObject;
	public eventParameters.eventToCallWithGameObject eventsToSendPlayerOnPlaceObject;
	public eventParameters.eventToCallWithGameObject eventsToSendPlayerOnTakeBackObject;

	public string getObjectName ()
	{
		return objectName;
	}

	public GameObject getObject ()
	{
		return mainGameObject;
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnPlaceOrTakeBackObject) {
			if (state) {
				eventOnPlaceObject.Invoke ();
			} else {
				eventOnTakeBackObject.Invoke ();
			}
		}
	}

	public void checkStateOnTakeObjectBack ()
	{
		if (mainGameObject != null) {
			craftingStationSystem currentCraftingStationSystem = mainGameObject.GetComponent<craftingStationSystem> ();

			if (currentCraftingStationSystem != null) {
				currentCraftingStationSystem.checkStateOnTakeObjectBack ();
			}
		}
	}

	public void checkEventsOnStateChangeWithPlayer (bool state, GameObject currentPlayer)
	{
		if (useEventsToSendPlayerOnPlaceOrTakeBackObject) {
			if (state) {
				eventsToSendPlayerOnPlaceObject.Invoke (currentPlayer);
			} else {
				eventsToSendPlayerOnTakeBackObject.Invoke (currentPlayer);
			}
		}
	}

	public void setObjectPlacedState (bool state)
	{
		objectPlaced = state;
	}

	public bool isObjectPlaced ()
	{
		return objectPlaced;
	}

	public void setObjectCanBeTakenBackToInventoryEnabledState (bool state)
	{
		objectCanBeTakenBackToInventoryEnabled = state;
	}

	public bool isObjectCanBeTakenBackToInventoryEnabled ()
	{
		return objectCanBeTakenBackToInventoryEnabled;
	}
}
