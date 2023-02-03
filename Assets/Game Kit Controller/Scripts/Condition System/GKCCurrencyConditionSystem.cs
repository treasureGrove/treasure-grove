using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKCCurrencyConditionSystem : GKCConditionInfo
{
	[Header ("Custom Settings")]
	[Space]

	public int amountToCheck;

	public override void checkIfConditionComplete ()
	{
		if (!checkIfPlayerAssigned ()) {
			return;
		}

		bool conditionResult = false;

		playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			currencySystem mainCurrencySystem = mainPlayerComponentsManager.getCurrencySystem ();

			if (mainCurrencySystem != null) {
				bool currentConditionState = true;

				if (mainCurrencySystem.getCurrentMoneyAmount () < amountToCheck) {
					currentConditionState = false;
				}

				conditionResult = currentConditionState;
			}
		}

		setConditionResult (conditionResult);
	}
}
