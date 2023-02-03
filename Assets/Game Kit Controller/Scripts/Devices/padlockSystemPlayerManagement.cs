using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class padlockSystemPlayerManagement : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool usingPadlock;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	padlockSystem currentPadlockSystem;

	public void setCurrentPadlockSystem (padlockSystem newPadlockSystem)
	{
		currentPadlockSystem = newPadlockSystem;
	}

	public void setUsingPadlockState (bool state)
	{
		usingPadlock = state;

		checkEventsOnStateChange (usingPadlock);
	}

	//CALL INPUT FUNCTIONS TO CURRENT PUZZLE SYSTEM
	public void inputRotateWheel (bool directionUp)
	{
		if (!usingPadlock) {
			return;
		}

		if (currentPadlockSystem != null) {
			currentPadlockSystem.inputRotateWheel (directionUp);
		}
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				evenOnStateEnabled.Invoke ();
			} else {
				eventOnStateDisabled.Invoke ();
			}
		}
	}
}
