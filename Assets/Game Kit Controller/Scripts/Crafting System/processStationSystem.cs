using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class processStationSystem : craftingStationSystem
{
	[Header ("Main Settings")]
	[Space]

	public bool stationEnabled = true;

	public List<string> materialProcessNameList = new List<string> ();

	[Space]
	[Header ("Process Settings")]
	[Space]

	public float processRate;

	[Space]
	[Header ("Energy Settings")]
	[Space]

	public bool useEnergyToExtract;

	public float maxEnergyAmount;

	public float currentEnergyAmount;

	public float useEnergyRate;

	public float energyToUseAmount;

	[Space]

	public bool useEnergyFromInventoryBank;

	public string energyObjectName;

	public bool checkEnergyObjectNameOnEnergyStation;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool stationActive;

	public bool energySourceLocated;

	public List<materialInfo> materialInfoList = new List<materialInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public inventoryBankSystem inventoryBankMaterialsProcessed;

	public inventoryBankSystem inventoryBankMaterialsSource;

	public inventoryBankSystem inventoryBankEnergySource;

	float lastTimeEnergyUsed;

	Coroutine updateCoroutine;

	List<craftingBlueprintInfoTemplateData> craftingBlueprintInfoTemplateDataList;

	float lastTimeMaterialsProcessed;


	public void toggleProcessStation ()
	{
		setStationActiveState (!stationActive);
	}

	public void setStationActiveState (bool state)
	{
		if (!stationEnabled) {
			return;
		}

		if (stationActive == state) {
			return;
		}

		stationActive = state;

		energySourceLocated = (!useEnergyFromInventoryBank || inventoryBankEnergySource != null);

		craftingUISystem currentCraftingUISystem = FindObjectOfType<craftingUISystem> ();

		if (currentCraftingUISystem != null) {
			craftingBlueprintInfoTemplateDataList = currentCraftingUISystem.getCraftingBlueprintInfoTemplateDataList ();
		}

		stopUpdateCoroutine ();

		if (stationActive) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		} else {
			
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (Time.time > processRate + lastTimeMaterialsProcessed) {
			materialInfoList.Clear ();

			List<inventoryInfo> inventoryList = inventoryBankMaterialsSource.getBankInventoryList ();

			for (int i = 0; i < inventoryList.Count; i++) {
				materialInfo newMaterialInfo = new materialInfo ();

				newMaterialInfo.materialName = inventoryList [i].Name;

				newMaterialInfo.materialAmount = inventoryList [i].amount;

				processMaterialInfo newProcessMaterialInfo = getProcessMaterialInfoByName (newMaterialInfo.materialName);

				newMaterialInfo.mainProcessMaterialInfo = newProcessMaterialInfo;

				newMaterialInfo.mainProcessMaterialInfoAssigned = newProcessMaterialInfo != null;

				materialInfoList.Add (newMaterialInfo);
			}

			if (materialInfoList.Count == 0) {
				setStationActiveState (false);

				return;
			}

			bool allObjectsProcessed = true;

			for (int i = 0; i < materialInfoList.Count; i++) {
				if (materialInfoList [i].mainProcessMaterialInfoAssigned) {
					if (materialInfoList [i].materialAmount >= materialInfoList [i].mainProcessMaterialInfo.materialAmountNeeded) {
						materialInfoList [i].materialAmount -= materialInfoList [i].mainProcessMaterialInfo.materialAmountNeeded;

						inventoryBankMaterialsProcessed.addInventoryObjectByName (materialInfoList [i].mainProcessMaterialInfo.materialProcessedName, materialInfoList [i].mainProcessMaterialInfo.materialAmountToObtain);
							
						inventoryBankMaterialsSource.removeObjectAmountFromInventory (materialInfoList [i].materialName, materialInfoList [i].mainProcessMaterialInfo.materialAmountNeeded);

						allObjectsProcessed = false;
					}
				}
			}

			for (int i = materialInfoList.Count - 1; i >= 0; i--) {	
				if (materialInfoList [i].materialAmount < 0) {
					materialInfoList.RemoveAt (i);
				}
			}

			lastTimeMaterialsProcessed = Time.time;

			if (allObjectsProcessed) {
				setStationActiveState (false);
			}
		}


		updateEnergyUsage ();
	}

	public processMaterialInfo getProcessMaterialInfoByName (string objectName)
	{
		for (int i = 0; i < craftingBlueprintInfoTemplateDataList.Count; i++) {
			for (int j = 0; j < craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList.Count; j++) {
				craftingBlueprintInfoTemplate currentCraftingBlueprintInfoTemplate = craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList [j];

				if (currentCraftingBlueprintInfoTemplate.Name.Equals (objectName)) {
					if (currentCraftingBlueprintInfoTemplate.processMaterialInfoList.Count > 0) {
						for (int k = 0; k < currentCraftingBlueprintInfoTemplate.processMaterialInfoList.Count; k++) {
							if (materialProcessNameList.Contains (currentCraftingBlueprintInfoTemplate.processMaterialInfoList [k].materialProcessedName)) {
								return currentCraftingBlueprintInfoTemplate.processMaterialInfoList [k];
							}
						}
					} else {
						return null;
					}
				}
			}
		}


		return null;
	}

	void updateEnergyUsage ()
	{
		if (useEnergyToExtract) {
			if (Time.time > lastTimeEnergyUsed + useEnergyRate) {
				if (useEnergyFromInventoryBank) {
					if (energySourceLocated) {
						currentEnergyAmount = inventoryBankEnergySource.getInventoryObjectAmountByName (energyObjectName);

						if (currentEnergyAmount > 0) {
							inventoryBankEnergySource.removeObjectAmountFromInventory (energyObjectName, (int)currentEnergyAmount);
						}
					} else {
						currentEnergyAmount = 0;
					}
				}

				if (energyStationAssigned) {
					if (checkEnergyObjectNameOnEnergyStation) {
						if (energyObjectName.Equals (currentEnergyStationSystem.getEnergyName ())) {
							currentEnergyAmount = currentEnergyStationSystem.getCurrentEnergyAmount ();
						} else {
							currentEnergyAmount = 0;
						}
					} else {
						currentEnergyAmount = currentEnergyStationSystem.getCurrentEnergyAmount ();
					}
				}

				currentEnergyAmount -= energyToUseAmount;

				if (useInfiniteEnergy) {
					currentEnergyAmount = maxEnergyAmount;
				}

				if (currentEnergyAmount < 0) {
					currentEnergyAmount = 0;
				}

				lastTimeEnergyUsed = Time.time;
			}

			if (currentEnergyAmount <= 0) {

				setStationActiveState (false);

			
			}
		}
	}

	[System.Serializable]
	public class materialInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string materialName;

		public int materialAmount;

		[Space]

		public bool mainProcessMaterialInfoAssigned;

		public processMaterialInfo mainProcessMaterialInfo;
	}
}
