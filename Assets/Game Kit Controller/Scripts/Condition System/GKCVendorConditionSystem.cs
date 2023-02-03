using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKCVendorConditionSystem : GKCConditionInfo
{
	[Header ("Custom Settings")]
	[Space]

	public int vendorObjectAmount;

	public bool useInfiniteVendorAmount;

	public float vendorObjectPrice;

	public float customMoneyChangeSpeed = 20;


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

				if (mainCurrencySystem.getCurrentMoneyAmount () < vendorObjectPrice) {
					currentConditionState = false;
				}

				if (currentConditionState) {
					if (useInfiniteVendorAmount || vendorObjectAmount > 0) {
						mainCurrencySystem.increaseTotalMoneyAmount (-vendorObjectPrice, customMoneyChangeSpeed);

						if (!useInfiniteVendorAmount) {
							vendorObjectAmount--;
						}
					} else {
						currentConditionState = false;
					}

				}

				conditionResult = currentConditionState;
			}
		}

		setConditionResult (conditionResult);
	}
}
