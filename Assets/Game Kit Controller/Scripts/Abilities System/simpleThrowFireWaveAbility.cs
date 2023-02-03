using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class simpleThrowFireWaveAbility : MonoBehaviour
{
	public bool currentState;

	public UnityEvent eventOnActivateState;
	public UnityEvent eventOnDeactivateState;

	public void toggleCurrentState ()
	{
		setCurrentState (!currentState);
	}

	public void setCurrentState (bool state)
	{
		if (currentState == state) {
			return;
		}

		currentState = state;

		if (currentState) {
			eventOnActivateState.Invoke ();
		} else {
			eventOnDeactivateState.Invoke ();
		}
	}
}
