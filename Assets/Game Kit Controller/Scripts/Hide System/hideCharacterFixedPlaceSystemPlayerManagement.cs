using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class hideCharacterFixedPlaceSystemPlayerManagement : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool playerHiding;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	hideCharacterFixedPlaceSystem currentFixedHideSystem;

	public void setPlayerHidingState (bool state)
	{
		playerHiding = state;

		checkEventsOnStateChange (playerHiding);
	}

	public void setcurrentFixedHideSystem (hideCharacterFixedPlaceSystem newFixedHideSystem)
	{
		currentFixedHideSystem = newFixedHideSystem;
	}
		
	//CALL INPUT FUNCTIONS TO CURRENT HIDE SYSTEM
	public void hideInputResetCameraTransform ()
	{
		if (!playerHiding) {
			return;
		}

		if (currentFixedHideSystem != null) {
			currentFixedHideSystem.inputResetCameraTransform ();
		}
	}

	public void hideInputSetIncreaseZoomStateByButton (bool state)
	{
		if (!playerHiding) {
			return;
		}

		if (currentFixedHideSystem != null) {
			currentFixedHideSystem.inputSetIncreaseZoomStateByButton (state);
		}
	}

	public void hideInputSetDecreaseZoomStateByButton (bool state)
	{
		if (!playerHiding) {
			return;
		}

		if (currentFixedHideSystem != null) {
			currentFixedHideSystem.inputSetDecreaseZoomStateByButton (state);
		}
	}

	public void hideInputSetZoomValueByMouseWheel (bool state)
	{
		if (!playerHiding) {
			return;
		}

		if (currentFixedHideSystem != null) {
			currentFixedHideSystem.inputSetZoomValueByMouseWheel (state);
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
