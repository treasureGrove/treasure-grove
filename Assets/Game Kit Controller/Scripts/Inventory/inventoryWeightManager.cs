using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UnityEngine.Events;

public class inventoryWeightManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useInventoryWeightEnabled = true;

	public bool useInventoryWeightLimit;
	public float inventoryWeightLimit;

	public string weightUnit = "Kg";

	public float currentTotalWeight;

	public bool useMaxObjectWeightToPick;
	public float maxObjectWeightToPick;

	public string inventoryWeightLimitStatName = "Inventory Weight Limit";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool weightLimitReached;

	public bool weightLimitReachedPreviously;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnRegularWeight;
	public UnityEvent evenOnWeightLimitReached;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject inventoryWeightPanel;
	public Text currentObjectWeightText;

	public Text totalInventoryWeightText;

	public Slider totalInventoryWeightSlider;

	public inventoryManager mainInventoryManager;

	public GameObject currentInventoryObjectWeightPanel;
	public playerStatsSystem playerStatsManager;

	bool hasPlayerStatsManager;

	Coroutine updateFullInventoryWeightCoroutine;

	void Start ()
	{
		if (!useInventoryWeightEnabled) {
			if (inventoryWeightPanel.activeSelf) {
				inventoryWeightPanel.SetActive (false);
			}
		}
	}

	public void updateCurrentInventoryObjectWeight ()
	{
		if (!useInventoryWeightEnabled) {
			return;
		}

		if (mainInventoryManager.isCurrentObjectNotNull ()) {
			if (!currentInventoryObjectWeightPanel.activeSelf) {
				currentInventoryObjectWeightPanel.SetActive (true);
			}

			inventoryInfo currentInventoryObjectInfo = mainInventoryManager.getCurrentInventoryObjectInfo ();

			float currentObjectWeight = currentInventoryObjectInfo.weight;

			if (currentInventoryObjectInfo.storeTotalAmountPerUnit) {
				int currentObjectAmountPerUnit = mainInventoryManager.getInventoryObjectAmountPerUnitByName (currentInventoryObjectInfo.Name);

				if (currentObjectAmountPerUnit > 0) {
					currentObjectWeight /= currentObjectAmountPerUnit;
				}
			}

			float currentObjectTotalWeight = currentObjectWeight * currentInventoryObjectInfo.amount;

			currentObjectWeightText.text = (currentObjectTotalWeight).ToString ("F1") + " " + weightUnit;

			if (currentInventoryObjectInfo.amount > 1) {
				currentObjectWeightText.text += " (" + currentObjectWeight.ToString ("F2") + " x " + currentInventoryObjectInfo.amount + ")";
			}
		} else {
			if (currentInventoryObjectWeightPanel.activeSelf) {
				currentInventoryObjectWeightPanel.SetActive (false);
			}
		}
	}

	public void disableCurrentInventoryObjectWeight ()
	{
		if (currentInventoryObjectWeightPanel.activeSelf) {
			currentInventoryObjectWeightPanel.SetActive (false);
		}
	}

	public void updateInventoryWeight ()
	{
		if (!useInventoryWeightEnabled) {
			return;
		}
			
		if (updateFullInventoryWeightCoroutine != null) {
			StopCoroutine (updateFullInventoryWeightCoroutine);
		}

		updateFullInventoryWeightCoroutine = StartCoroutine (updateInventoryWeightCoroutine ());
	}

	IEnumerator updateInventoryWeightCoroutine ()
	{
		yield return new WaitForSeconds (0.01f);

		currentTotalWeight = 0;

		for (int i = 0; i < mainInventoryManager.inventoryList.Count; i++) {
			float currentObjectWeight = mainInventoryManager.inventoryList [i].weight;

			if (mainInventoryManager.inventoryList [i].storeTotalAmountPerUnit) {
				int currentObjectAmountPerUnit = mainInventoryManager.getInventoryObjectAmountPerUnitByName (mainInventoryManager.inventoryList [i].Name);

				if (currentObjectAmountPerUnit > 0) {
					currentObjectWeight /= currentObjectAmountPerUnit;
				}
			}

			currentTotalWeight += mainInventoryManager.inventoryList [i].amount * currentObjectWeight;
		}

		if (weightLimitReached) {
			weightLimitReachedPreviously = true;
		}

		if (useInventoryWeightLimit) {
			if (currentTotalWeight > inventoryWeightLimit) {
				weightLimitReached = true;
			} else {
				weightLimitReached = false;
			}
		}

		if (weightLimitReached) {
			evenOnWeightLimitReached.Invoke ();
		} else {
			if (weightLimitReachedPreviously) {
				eventOnRegularWeight.Invoke ();
			}
		}

		string totalInventoryWeightString = currentTotalWeight.ToString ("F1");

		if (useInventoryWeightLimit) {
			totalInventoryWeightString += " / " + inventoryWeightLimit + " " + weightUnit;
		} 

		totalInventoryWeightText.text = totalInventoryWeightString;

		if (useInventoryWeightLimit) {
			totalInventoryWeightSlider.maxValue = inventoryWeightLimit;

			totalInventoryWeightSlider.value = currentTotalWeight;
		}
	}

	public int checkIfCanCarryObjectWeight (inventoryInfo inventoryInfoToCheck)
	{
		float currentObjectWeight = inventoryInfoToCheck.weight;

		if (inventoryInfoToCheck.storeTotalAmountPerUnit) {
			int currentObjectAmountPerUnit = mainInventoryManager.getInventoryObjectAmountPerUnitByName (inventoryInfoToCheck.Name);

			if (currentObjectAmountPerUnit > 0) {
				currentObjectWeight /= currentObjectAmountPerUnit;
			}
		}

		float newExtraWeight = inventoryInfoToCheck.amount * currentObjectWeight;

		if (newExtraWeight + currentTotalWeight < inventoryWeightLimit) {
			return inventoryInfoToCheck.amount;
		} else {
			float weightSpaceFree = inventoryWeightLimit - currentTotalWeight;

			int amountWhichCanBeTaken = (int)(weightSpaceFree / currentObjectWeight);

			return amountWhichCanBeTaken;
		}
	}

	public bool checkIfCanCarrySingleObjectWeight (inventoryInfo inventoryInfoToCheck)
	{
		if (!useMaxObjectWeightToPick) {
			return true;
		}

		float currentObjectWeight = inventoryInfoToCheck.weight;

		if (inventoryInfoToCheck.storeTotalAmountPerUnit) {
			int currentObjectAmountPerUnit = mainInventoryManager.getInventoryObjectAmountPerUnitByName (inventoryInfoToCheck.Name);

			if (currentObjectAmountPerUnit > 0) {
				currentObjectWeight /= currentObjectAmountPerUnit;
			}
		}

		print ("object weight " + currentObjectWeight);

		if (currentObjectWeight > maxObjectWeightToPick) {
			return false;
		}

		return true;
	}

	public void initializeInventoryWeightLimitAmount (float newValue)
	{
		inventoryWeightLimit = newValue;
	}

	public void increaseInventoryWeightLimitAmount (float newValue)
	{
		inventoryWeightLimit += newValue;

		updateInventoryWeight ();
	}

	public void increaseInventoryBagWeight (float newValue)
	{
		increaseInventoryWeightLimitAmount (newValue);

		if (!hasPlayerStatsManager) {
			if (playerStatsManager != null) {
				hasPlayerStatsManager = true;
			}
		}

		if (hasPlayerStatsManager) {
			if (hasPlayerStatsManager) {
				playerStatsManager.updateStatValue (inventoryWeightLimitStatName, inventoryWeightLimit);
			}
		}
	}

	public void setUseInventoryWeightEnabledState (bool state)
	{
		useInventoryWeightEnabled = state;
	}
}
