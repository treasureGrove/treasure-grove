using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectToPlaceSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool objectToPlacedEnabled = true;

	public bool objectCanCallPlacedEvent = true;

	public bool objectCanCallRemovedEvent = true;

	public string objectName;

	[Space]
	[Header ("Debug")]
	[Space]

	public putObjectSystem currentPutObjectSystem;

	public bool objectInGrabbedState;

	public bool objectPlaced;


	public string getObjectName ()
	{
		return objectName;
	}

	public void assignPutObjectSystem (putObjectSystem putObjectSystemToAssign)
	{
		currentPutObjectSystem = putObjectSystemToAssign;
	}

	public void setObjectPlaceState (bool state)
	{
		objectPlaced = state;
	}

	public void setObjectInGrabbedState (bool state)
	{
		if (!objectToPlacedEnabled) {
			return;
		}

		objectInGrabbedState = state;

		if (objectInGrabbedState && objectPlaced) {
			objectPlaced = false;

			currentPutObjectSystem.removePlacedObject ();

			currentPutObjectSystem = null;
		}
	}

	public bool isObjectInGrabbedState ()
	{
		return objectInGrabbedState;
	}

	public void setObjectToPlacedEnabledState (bool state)
	{
		objectToPlacedEnabled = state;
	}

	public bool canObjectCanCallPlacedEvent ()
	{
		return objectCanCallPlacedEvent;
	}

	public bool canObjectCanCallRemovedEvent ()
	{
		return objectCanCallRemovedEvent;
	}
}
