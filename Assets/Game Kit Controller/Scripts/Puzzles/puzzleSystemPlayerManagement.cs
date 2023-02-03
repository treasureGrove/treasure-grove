using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class puzzleSystemPlayerManagement : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool usingPuzzle;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	puzzleSystem currentPuzzleSystem;

	public void setcurrentPuzzleSystem (puzzleSystem newPuzzleObject)
	{
		currentPuzzleSystem = newPuzzleObject;
	}

	public void setUsingPuzzleState (bool state)
	{
		usingPuzzle = state;

		checkEventsOnStateChange (usingPuzzle);
	}

	//CALL INPUT FUNCTIONS TO CURRENT PUZZLE SYSTEM
	public void puzzleObjectInputSetRotateObjectState (bool state)
	{
		if (!usingPuzzle) {
			return;
		}

		if (currentPuzzleSystem != null) {
			currentPuzzleSystem.inputSetRotateObjectState (state);
		}
	}

	public void puzzleObjectInputIncreaseObjectHolDistanceByButton (bool state)
	{
		if (!usingPuzzle) {
			return;
		}

		if (currentPuzzleSystem != null) {
			currentPuzzleSystem.inputIncreaseObjectHolDistanceByButton (state);
		}
	}

	public void puzzleObjectInputDecreaseObjectHolDistanceByButton (bool state)
	{
		if (!usingPuzzle) {
			return;
		}

		if (currentPuzzleSystem != null) {
			currentPuzzleSystem.inputDecreaseObjectHolDistanceByButton (state);
		}
	}

	public void puzzleObjectInputSetObjectHolDistanceByMouseWheel (bool state)
	{
		if (!usingPuzzle) {
			return;
		}

		if (currentPuzzleSystem != null) {
			currentPuzzleSystem.inputSetObjectHolDistanceByMouseWheel (state);
		}
	}

	public void puzzleObjectInputResetPuzzle ()
	{
		if (!usingPuzzle) {
			return;
		}

		if (currentPuzzleSystem != null) {
			currentPuzzleSystem.inputResetPuzzle ();
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
