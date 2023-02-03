using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleCoinPickup : MonoBehaviour
{
	public int coinAmount = 1;

	public GameObject coinPickupGameObject;

	public void checkToPickCoin (GameObject characterDetected)
	{
		coinPocketSystem currentCoinPocketSystem = characterDetected.GetComponentInChildren<coinPocketSystem> ();

		if (currentCoinPocketSystem != null) {
			if (currentCoinPocketSystem.canPickCoins ()) {
				currentCoinPocketSystem.addCoinAmount (coinAmount);

				Destroy (coinPickupGameObject);
			}
		}
	}
}
