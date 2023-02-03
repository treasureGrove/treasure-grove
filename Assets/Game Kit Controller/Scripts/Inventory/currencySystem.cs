using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class currencySystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int currentMoneyAmount;

	public string statName = "Money";
	public string extraStringContent = "$";

	public bool showTotalMoneyAmountOnChange;

	public float timeToShowTotalMoneyAmount;

	public bool increaseMoneyTextSmoothly;
	public float increaseMoneyTextRate = 0.01f;
	public float delayToStartIncreasMoneyText = 0.5f;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnReceiveMoney;
	public eventParameters.eventToCallWithString eventOnReceiveMoneyWithString;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject totalMoneyAmountPanel;
	public Text totalMoneyAmountText;
	public playerStatsSystem playerStatsManager;

	int moneyToAdd;
	int previousMoneyAmount;

	bool firstAmountAssigned;

	bool previousMoneyToAddChecked = true;

	Coroutine showTotalMoneyCoroutine;

	bool lastAmountAddedIsPositive = true;

	float customMoneyChangeSpeed;


	public void increaseTotalMoneyAmount (float extraValue, float customMoneyChangeSpeedValue)
	{
		customMoneyChangeSpeed = customMoneyChangeSpeedValue;

		increaseTotalMoneyAmount (extraValue);
	}

	public void increaseTotalMoneyAmount (float extraValue)
	{
		if (increaseMoneyTextSmoothly) {
			if (moneyToAdd == 0) {
				previousMoneyAmount = currentMoneyAmount;

				previousMoneyToAddChecked = false;
			}
		}

		if (extraValue > 0) {
			lastAmountAddedIsPositive = true;
		} else {
			lastAmountAddedIsPositive = false;
		}

		currentMoneyAmount += (int)extraValue;

		eventOnReceiveMoney.Invoke ();

		string newString = "";

		if (extraValue > 0) {
			newString = "+";
		} else {
			newString = "-";
		}

		newString += extraValue + extraStringContent;

		eventOnReceiveMoneyWithString.Invoke (newString);

		playerStatsManager.updateStatValue (statName, currentMoneyAmount);

		if (showTotalMoneyAmountOnChange) {
			if (increaseMoneyTextSmoothly) {
				moneyToAdd += (int)extraValue;
			}

			showTotalMoneyAmountPanel ();
		}
	}

	public void initializeMoneyAmount (float newValue)
	{
		currentMoneyAmount = (int)newValue;
	}

	public bool useMoney (float amountToUse)
	{
		if (currentMoneyAmount >= (int)amountToUse) {
			currentMoneyAmount -= (int)amountToUse;

			playerStatsManager.updateStatValue (statName, currentMoneyAmount);

			return true;
		} else {
			return false;
		}
	}

	public float getCurrentMoneyAmount ()
	{
//		print (currentMoneyAmount);

		return (float)currentMoneyAmount;
	}

	public bool canSpendMoneyAmount (float amountToSpend)
	{
		return currentMoneyAmount >= (int)amountToSpend;
	}

	public void showTotalMoneyAmountPanel ()
	{
		stopShowTotalMoneyAmountPanelCoroutine ();

		showTotalMoneyCoroutine = StartCoroutine (showTotalMoneyAmountPanelCoroutine ());
	}

	public void stopShowTotalMoneyAmountPanelCoroutine ()
	{
		if (showTotalMoneyCoroutine != null) {
			StopCoroutine (showTotalMoneyCoroutine);
		}
	}

	IEnumerator showTotalMoneyAmountPanelCoroutine ()
	{
		totalMoneyAmountPanel.SetActive (true);

		if (!firstAmountAssigned) {
			totalMoneyAmountText.text = previousMoneyAmount + " " + extraStringContent;

			firstAmountAssigned = true;
		}

		if (increaseMoneyTextSmoothly) {

			if (!previousMoneyToAddChecked) {
				yield return new WaitForSeconds (delayToStartIncreasMoneyText);

				previousMoneyToAddChecked = true;
			}

			int moneyIncreaseAmount = 1;

			if (!lastAmountAddedIsPositive) {
				moneyToAdd = Mathf.Abs (moneyToAdd);
			}

			if (moneyToAdd > 900) {
				int extraIncreaseAmount = moneyToAdd / 900;

				if (customMoneyChangeSpeed != 0) {
					moneyIncreaseAmount += extraIncreaseAmount + (int)customMoneyChangeSpeed;
				} else {
					moneyIncreaseAmount += extraIncreaseAmount + 12;
				}
			}

			while (moneyToAdd > 0) {
				if (lastAmountAddedIsPositive) {
					previousMoneyAmount += moneyIncreaseAmount;
				} else {
					previousMoneyAmount -= moneyIncreaseAmount;
				}

				totalMoneyAmountText.text = previousMoneyAmount + " " + extraStringContent;

				moneyToAdd -= moneyIncreaseAmount;

				yield return new WaitForSeconds (increaseMoneyTextRate);
			}

			totalMoneyAmountText.text = currentMoneyAmount + " " + extraStringContent;
		} else {
			totalMoneyAmountText.text = currentMoneyAmount + " " + extraStringContent;
		}

		lastAmountAddedIsPositive = true;

		yield return new WaitForSeconds (timeToShowTotalMoneyAmount);

		totalMoneyAmountPanel.SetActive (false);

		customMoneyChangeSpeed = 0;
	}
}
