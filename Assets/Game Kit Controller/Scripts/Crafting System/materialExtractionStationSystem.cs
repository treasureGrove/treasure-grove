using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class materialExtractionStationSystem : craftingStationSystem
{
	[Header ("Main Settings")]
	[Space]

	public bool stationEnabled = true;

	public bool activateStationAtStart;

	public float maxDetectionDistance = 10;

	public Transform extractionZoneTransform;

	public bool useMaxMaterialZonesToExtractAtSameTime;
	public int maxMaterialZonesToExtractAtSameTime;

	[Space]
	[Header ("Extraction Settings")]
	[Space]

	public bool useGeneralMaterialExtractionRate;

	public float generalMaterialExtractionRate;

	[Space]

	public bool useGeneralMaterialExtractionAmount;

	public int generalMaterialExtractionAmount;

	[Space]
	[Space]

	public bool stationCanOnlyExtractCustomMaterialList;
	public List<string> customMaterialListToExtract = new List<string> ();

	[Space]
	[Header ("Custom Materials Zone To Use Settings")]
	[Space]

	public bool ignoreMaterialZonesFound;

	public bool extractMaterialsEvenIfNotZoneFound;

	public materialsZoneSystem customMaterialZoneSystem;

	[Space]
	[Header ("Extraction Conditions Settings")]
	[Space]

	public bool ignoreToExtractMaterialsFromIDList;

	public  List<int> materialsIDListToIgnore = new List<int> ();

	[Space]
	[Header ("Spawn Extracted Objects Settings")]
	[Space]

	public bool spawnExtractedObjects;

	public Transform positionToSpawnExtractedObjects;

	public bool addForceToSpawnedObjects;

	public ForceMode forceModeOnSpawnedObjects;
	public float forceAmountOnSpawnedObjects;

	[Space]

	public bool packExtractedObjectsBeforeSpawn;
	public int minAmountToPackToSpawn;

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

	List<float> lastTimeMaterialExtractedList = new List<float> ();

	public List<materialsZoneSystem> materialsZoneSystemLocatedList = new List<materialsZoneSystem> ();

	public bool energySourceLocated;

	public int numberOfZonesLocated;

	public List<int> materialZonesSystemsIDToIgnoreList = new List<int> ();

	public List<extractedMaterialInfo> extractedMaterialInfoList = new List<extractedMaterialInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnActivateStation;
	public UnityEvent eventonDeactivateStation;

	[Space]

	public UnityEvent eventOnEnergyEmpty;
	public UnityEvent eventOnEnergyRecharge;
	public UnityEvent eventOnEnergyRefilled;

	[Space]
	[Header ("Components")]
	[Space]

	public inventoryBankSystem mainInventoryBankSystem;

	public inventoryBankSystem inventoryBankEnergySource;


	Coroutine updateCoroutine;

	float lastTimeMaterialExtracted;

	float lastTimeEnergyUsed;

	materialsZoneSystem currentMaterialsZoneSystem;

	inventoryListManager mainInvetoryListManager;

	bool mainInventoryManagerLocated;

	void Start ()
	{
		if (activateStationAtStart) {
			setExtractionActiveState (true);
		}
	}

	void checkGetMainInventoryManager ()
	{
		if (!mainInventoryManagerLocated) {
			if (mainInvetoryListManager == null) {
				mainInvetoryListManager = FindObjectOfType<inventoryListManager> ();

				mainInventoryManagerLocated = true;
			}
		}
	}

	public void toggleExtraction ()
	{
		setExtractionActiveState (!stationActive);
	}

	public void setExtractionActiveState (bool state)
	{
		if (!stationEnabled) {
			return;
		}

		if (stationActive == state) {
			return;
		}

		if (state) {
			if (useEnergyToExtract) {
				if (currentEnergyAmount <= 0) {
					return;
				}
			}
		}

		stationActive = state;
	
		if (stationActive) {
			eventOnActivateStation.Invoke ();

			checkMaterialZonesAround ();
		} else {
			eventonDeactivateStation.Invoke ();

			stopUpdateCoroutine ();

			for (int i = 0; i < materialsZoneSystemLocatedList.Count; i++) {
				materialsZoneSystemLocatedList [i].setMaterialsZoneExtractionInProcessState (false);
			}
		}
	}

	void checkMaterialZonesAround ()
	{
		materialsZoneSystemLocatedList.Clear ();

		lastTimeMaterialExtractedList.Clear ();

		extractedMaterialInfoList.Clear ();

		if (ignoreMaterialZonesFound) {
			if (extractMaterialsEvenIfNotZoneFound) {
				materialsZoneSystemLocatedList.Add (customMaterialZoneSystem);

				lastTimeMaterialExtractedList.Add (0);
			}
		} else {
			materialsZoneSystem[] temporalMaterialsZoneSystem = FindObjectsOfType<materialsZoneSystem> ();

			foreach (materialsZoneSystem currentMaterialsZoneSystem in temporalMaterialsZoneSystem) {
				if (currentMaterialsZoneSystem.isMaterialsZoneEnabled ()) {
					bool addZoneResult = false;

					float currentDistance = GKC_Utils.distance (extractionZoneTransform.position, currentMaterialsZoneSystem.transform.position);

					if (currentDistance < maxDetectionDistance) {
						addZoneResult = true;
					}

					if (useMaxMaterialZonesToExtractAtSameTime) {
						if (materialsZoneSystemLocatedList.Count >= maxMaterialZonesToExtractAtSameTime) {
							addZoneResult = false;
						}
					}

					if (addZoneResult) {
						materialsZoneSystemLocatedList.Add (currentMaterialsZoneSystem);

						lastTimeMaterialExtractedList.Add (0);
					}
				}
			}
		}

		energySourceLocated = (!useEnergyFromInventoryBank || inventoryBankEnergySource != null);

		lastTimeMaterialExtracted = 0;

		lastTimeEnergyUsed = 0;

		numberOfZonesLocated = 0;

		stopUpdateCoroutine ();

		if (materialsZoneSystemLocatedList.Count > 0) {
			numberOfZonesLocated = materialsZoneSystemLocatedList.Count;

			for (int i = 0; i < materialsZoneSystemLocatedList.Count; i++) {
				materialsZoneSystemLocatedList [i].setMaterialsZoneExtractionInProcessState (true);
			}

			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
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
		if (useGeneralMaterialExtractionRate) {
			if (Time.time > generalMaterialExtractionRate + lastTimeMaterialExtracted) {
				bool allMaterialZonesEmpty = true;

				for (int i = 0; i < materialsZoneSystemLocatedList.Count; i++) {
					checkMaterialZoneToExtract (i, ref allMaterialZonesEmpty);
				}

				lastTimeMaterialExtracted = Time.time;

				if (allMaterialZonesEmpty) {
					setExtractionActiveState (false);
				}
			}
		} else {
			bool allMaterialZonesEmpty = true;

			for (int i = 0; i < materialsZoneSystemLocatedList.Count; i++) {
				if (Time.time > materialsZoneSystemLocatedList [i].materialExtractionRate + lastTimeMaterialExtractedList [i]) {
					checkMaterialZoneToExtract (i, ref allMaterialZonesEmpty);

					lastTimeMaterialExtractedList [i] = Time.time;
				}
			}

			if (allMaterialZonesEmpty) {
				setExtractionActiveState (false);
			}
		}

		updateEnergyUsage ();
	}

	void checkMaterialZoneToExtract (int zoneIndex, ref bool allMaterialZonesEmpty)
	{
		currentMaterialsZoneSystem = materialsZoneSystemLocatedList [zoneIndex];

		bool checkMaterialZoneResult = true;

		if (currentMaterialsZoneSystem.isMaterialsZoneEmpty ()) {
			checkMaterialZoneResult = false;
		}

		if (checkMaterialZoneResult) {
			if (materialZonesSystemsIDToIgnoreList.Contains (currentMaterialsZoneSystem.getMaterialZoneID ())) {
				checkMaterialZoneResult = false;
			}
		}

		if (checkMaterialZoneResult) {
			allMaterialZonesEmpty = false;

			bool allMaterialsOnZoneEmpty = true;

			int currentExtractionAmount = 0;

			for (int j = 0; j < currentMaterialsZoneSystem.materialZoneInfoList.Count; j++) {
				materialsZoneSystem.materialZoneInfo currentMaterialZoneInfo = currentMaterialsZoneSystem.materialZoneInfoList [j];

				bool canExtractMaterialResult = true;
			
				if (currentMaterialZoneInfo.materialAmount <= 0) {
					canExtractMaterialResult = false;
				}

				if (canExtractMaterialResult) {
					if (stationCanOnlyExtractCustomMaterialList && !canExtractMaterial (currentMaterialZoneInfo.materialName)) {
						canExtractMaterialResult = false;
					}
				}

				if (canExtractMaterialResult) {
					if (ignoreToExtractMaterialsFromIDList && materialsIDListToIgnore.Contains (currentMaterialZoneInfo.materialID)) {
						canExtractMaterialResult = false;
					}
				}

				if (canExtractMaterialResult) {
					if (useGeneralMaterialExtractionAmount) {
						currentExtractionAmount = generalMaterialExtractionAmount;
					} else {
						currentExtractionAmount = currentMaterialsZoneSystem.materialExtractionAmount;
					}

					if (currentMaterialZoneInfo.useCustomMaterialExtractionAmount) {
						currentExtractionAmount = currentMaterialZoneInfo.customMaterialExtractionAmount;
					}

					int amountToObtain = currentExtractionAmount;

					currentMaterialZoneInfo.materialAmount -= amountToObtain;

					if (currentMaterialZoneInfo.useInfiniteMaterialAmount) {
						currentMaterialZoneInfo.materialAmount = 1;
					}

					if (spawnExtractedObjects) {
						if (packExtractedObjectsBeforeSpawn) {
							int currentIndex = extractedMaterialInfoList.FindIndex (s => s.materialName.Equals (currentMaterialZoneInfo.materialName));

							if (currentIndex > -1) {
								extractedMaterialInfoList [currentIndex].materialAmount += amountToObtain;
							} else {
								extractedMaterialInfo newExtractedMaterialInfo = new extractedMaterialInfo ();

								newExtractedMaterialInfo.materialName = currentMaterialZoneInfo.materialName;
								newExtractedMaterialInfo.materialAmount = amountToObtain;

								extractedMaterialInfoList.Add (newExtractedMaterialInfo);

								currentIndex = extractedMaterialInfoList.Count - 1;
							}

							if (extractedMaterialInfoList [currentIndex].materialAmount >= minAmountToPackToSpawn) {
								spawnExtractedMaterial (extractedMaterialInfoList [currentIndex].materialName, minAmountToPackToSpawn);

								extractedMaterialInfoList [currentIndex].materialAmount -= minAmountToPackToSpawn;

								if (extractedMaterialInfoList [currentIndex].materialAmount <= 0) {
									extractedMaterialInfoList.RemoveAt (currentIndex);
								}
							}
						} else {
							spawnExtractedMaterial (currentMaterialZoneInfo.materialName, amountToObtain);
						}
					} else {
						mainInventoryBankSystem.addInventoryObjectByName (currentMaterialZoneInfo.materialName, amountToObtain);
					}

					allMaterialsOnZoneEmpty = false;
				}
			}

			if (allMaterialsOnZoneEmpty) {
				if (currentMaterialsZoneSystem.getRemainMaterialsAmount () > 0) {
					materialZonesSystemsIDToIgnoreList.Add (currentMaterialsZoneSystem.getMaterialZoneID ());
				} else {
					currentMaterialsZoneSystem.setMaterialsZoneEmptyState (true);
				}
			}
		}
	}

	void spawnExtractedMaterial (string materialName, int amountToObtain)
	{
		if (!mainInventoryManagerLocated) {
			checkGetMainInventoryManager ();
		}

		if (mainInventoryManagerLocated) {
			GameObject newMaterialObject = 
				mainInvetoryListManager.spawnInventoryObjectByName (materialName,  
					amountToObtain, positionToSpawnExtractedObjects.position, positionToSpawnExtractedObjects.rotation); 

			if (addForceToSpawnedObjects) {
				if (newMaterialObject != null) {
					Rigidbody objectRigidbody = newMaterialObject.GetComponent<Rigidbody> ();

					if (objectRigidbody != null) {
						objectRigidbody.AddForce (positionToSpawnExtractedObjects.forward * forceAmountOnSpawnedObjects, forceModeOnSpawnedObjects);
					}
				}
			}
		}
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

				setExtractionActiveState (false);

				eventOnEnergyEmpty.Invoke ();
			}
		}
	}

	public void addEnergy (float newValue)
	{
		if (useEnergyToExtract) {
			currentEnergyAmount += newValue;

			if (currentEnergyAmount >= maxEnergyAmount) {
				currentEnergyAmount = maxEnergyAmount;

				eventOnEnergyRefilled.Invoke ();
			} else {
				eventOnEnergyRecharge.Invoke ();
			}
		}
	}

	public override void sendEnergyValue (float newAmount)
	{
		addEnergy (newAmount);
	}

	public void refullEnergy ()
	{
		addEnergy (maxEnergyAmount);
	}

	public void removeEnergySource ()
	{
		inventoryBankEnergySource = null;

		energySourceLocated = false;

		if (useInfiniteEnergy) {
			currentEnergyAmount = 1;
		} else {
			currentEnergyAmount = 0;
		}
	}

	public float getCurrentEnergyAmount ()
	{
		return currentEnergyAmount;
	}

	public float getMaxEnergyAmount ()
	{
		return maxEnergyAmount;
	}

	public void setEnergySource (inventoryBankSystem newBank)
	{
		inventoryBankEnergySource = newBank;

		energySourceLocated = inventoryBankEnergySource != null;
	}

	public override void checkEnergyStationOnStateChange ()
	{
		if (removeRemainEnergyOnRemoveEnergyStation) {
			if (currentEnergyStationSystem == null) {
				currentEnergyAmount = 0;

				if (stationActive) {
					setExtractionActiveState (false);
				}
			}
		}
	}

	public bool canExtractMaterial (string materialName)
	{
		if (stationCanOnlyExtractCustomMaterialList) {
			if (customMaterialListToExtract.Contains (materialName)) {
				return true;
			}

			return false;
		} else {
			return true;
		}
	}

	[System.Serializable]
	public class extractedMaterialInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string materialName;

		public int materialAmount;
	}
}
