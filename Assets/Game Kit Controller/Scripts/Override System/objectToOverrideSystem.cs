using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectToOverrideSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool canBeOverriden = true;

	public GameObject objectToOverride;

	[Space]
	[Header ("Remote Events On Object Settings")]
	[Space]

	public bool useRemoteEventsOnObjectOnStart;
	public List<string> remoteEventNameListOnObjectOnStart = new List<string> ();

	[Space]
	[Space]

	public bool useRemoteEventsOnObjectOnEnd;
	public List<string> remoteEventNameListOnObjectOnEnd = new List<string> ();

	[Space]
	[Header ("Remote Events On Player Settings")]
	[Space]

	public bool useRemoteEventsOnPlayerOnStart;
	public List<string> remoteEventNameListOnPlayerOnStart = new List<string> ();

	[Space]
	[Space]

	public bool useRemoteEventsOnPlayerOnEnd;
	public List<string> remoteEventNameListOnPlayerOnEnd = new List<string> ();

	GameObject currentPlayer;

	public bool canBeOverridenActive ()
	{
		return canBeOverriden;
	}

	public GameObject getObjectToOverride ()
	{
		if (objectToOverride == null) {
			objectToOverride = gameObject;
		}

		return objectToOverride;
	}

	public void setCurrentPlayer (GameObject newPlayer)
	{
		currentPlayer = newPlayer;
	}

	public void setOverrideControlExternally (bool state)
	{
		if (currentPlayer != null) {
			playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				
				overrideElementControlSystem mainOverrideElementControlSystem = mainPlayerComponentsManager.getOverrideElementControlSystem ();

				if (mainOverrideElementControlSystem != null) {
					if (state) {
						if (!mainOverrideElementControlSystem.usingOverride) {
							mainOverrideElementControlSystem.overrideElementControl (objectToOverride);
						}
					} else {
						if (mainOverrideElementControlSystem.usingOverride) {
							mainOverrideElementControlSystem.stopCurrentOverrideControl ();
						}
					}

					mainOverrideElementControlSystem.setInputStopOverrideControlPausedState (state);
				}
			}
		}
	}
}
