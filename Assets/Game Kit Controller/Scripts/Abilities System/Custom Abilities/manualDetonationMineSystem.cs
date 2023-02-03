using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class manualDetonationMineSystem : MonoBehaviour
{
	public float minDelayToActivateMine;

	public UnityEvent eventToPlaceNewMine;

	public GameObject currentMine;

	manualDetonationMineObject currentManualDetonationMineObject;

	float lastTimeMinePlaced;

	public void placeNewMine ()
	{
		if (currentMine == null) {
			eventToPlaceNewMine.Invoke ();
		}
	}

	public void setCurrentMine (GameObject newMineObject)
	{
		currentMine = newMineObject;

		currentManualDetonationMineObject = currentMine.GetComponent<manualDetonationMineObject> ();

		currentManualDetonationMineObject.setManualDetonationMineSystem (this);

		lastTimeMinePlaced = Time.time;
	}

	public void activateCurrentMine ()
	{
		if (currentMine != null) {
			if (Time.time > minDelayToActivateMine + lastTimeMinePlaced) {
				currentManualDetonationMineObject.activateMine ();

				removeCurrentMine ();
			}
		}
	}

	public void removeCurrentMine ()
	{
		currentMine = null;
	}
}
