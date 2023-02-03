using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class coinPocketSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int currentCointAmount;

	public bool pickCoinsEnabled = true;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnDropAllCoins;

	public eventParameters.eventToCallWithInteger eventToSetAmountOfCoins;

	public bool useEventsOnPickedCoin;
	public UnityEvent eventOnPickedCoin;

	public int getCoinAmount ()
	{
		return currentCointAmount;
	}

	public void addCoinAmount (int newAmount)
	{
		currentCointAmount += newAmount;

		if (useEventsOnPickedCoin) {
			eventOnPickedCoin.Invoke ();
		}
	}

	public void dropAllCoins ()
	{
		if (currentCointAmount > 0) {
			eventToSetAmountOfCoins.Invoke (currentCointAmount);

			eventOnDropAllCoins.Invoke ();

			currentCointAmount = 0;
		}
	}

	public bool canPickCoins ()
	{
		return pickCoinsEnabled;
	}

	public void setPickCoinsEnabledState (bool state)
	{
		pickCoinsEnabled = state;
	}
}
