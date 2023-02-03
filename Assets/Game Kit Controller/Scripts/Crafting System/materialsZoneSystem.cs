using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class materialsZoneSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool materialsZoneEnabled = true;

	public bool materialsZoneCanChangePosition;

	public int materialZoneID;

	[Space]
	[Header ("Extraction Settings")]
	[Space]

	public float materialExtractionRate;
	public int materialExtractionAmount;

	[Space]
	[Header ("Spawn Objects Settings")]
	[Space]

	public float maxRadiusToInstantiate = 1;
	public Transform positionToSpawnExtractedObjects;

	public bool addForceToSpawnedObjects;

	public ForceMode forceModeOnSpawnedObjects;
	public float forceAmountOnSpawnedObjects;

	public float forceRadiusOnSpawnedObjects;

	[Space]
	[Header ("Material Info List Settings")]
	[Space]

	public List<materialZoneInfo> materialZoneInfoList = new List<materialZoneInfo> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool refillMaterialsZoneAfterDelayOnEmpty;
	public float delayToRefillMaterialsOnEmpty;

	public bool canBeExtractedByExternalElementsEnabled = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public int remainMaterialsAmount;

	public bool materialsZoneEmpty;

	public bool materialsZoneFull = true;

	public bool materialsZoneExtractionInProcess;

	public int materialExtractionStationsActive;

	public bool refillInProcess;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnZoneEmpty;
	public UnityEvent eventOnZoneEmpty;

	public bool useEventOnZoneRefilled;
	public UnityEvent eventOnZoneRefilled;

	Coroutine refillCoroutine;

	inventoryListManager mainInvetoryListManager;

	bool mainInventoryManagerLocated;

	public void setMaterialsZoneExtractionInProcessState (bool state)
	{
		if (state) {
			materialExtractionStationsActive++;
		} else {
			materialExtractionStationsActive--;
		}

		if (materialExtractionStationsActive < 0) {
			materialExtractionStationsActive = 0;
		}
			
		materialsZoneExtractionInProcess = (materialExtractionStationsActive > 0);

		if (materialsZoneExtractionInProcess) {
			if (materialsZoneFull) {
				getRemainMaterialsAmount ();

				materialsZoneFull = false;
			}
		}
	}

	public int getRemainMaterialsAmount ()
	{
		remainMaterialsAmount = 0;

		for (int i = 0; i < materialZoneInfoList.Count; i++) {
			remainMaterialsAmount += materialZoneInfoList [i].materialAmount;
		}

		return remainMaterialsAmount;
	}

	public void setMaterialsZoneFullState (bool state)
	{
		materialsZoneFull = state;

		if (materialsZoneFull) {
			refillMaterialsZone ();
		}
	}

	void refillMaterialsZone ()
	{
		materialsZoneEnabled = true;

		materialsZoneEmpty = false;

		for (int i = 0; i < materialZoneInfoList.Count; i++) {
			materialZoneInfoList [i].materialAmount = materialZoneInfoList [i].maxMaterialAmount;
		}

		if (useEventOnZoneRefilled) {
			eventOnZoneRefilled.Invoke ();
		}
	}

	public bool isMaterialsZoneEnabled ()
	{
		return materialsZoneEnabled;
	}

	public void setMaterialsZoneEmptyState (bool state)
	{
		materialsZoneEmpty = state;

		for (int i = 0; i < materialZoneInfoList.Count; i++) {
			if (materialZoneInfoList [i].materialAmount > 0) {
				materialsZoneEmpty = false;

				return;
			}
		}

		if (materialsZoneEmpty) {
			if (useEventOnZoneEmpty) {
				eventOnZoneEmpty.Invoke ();
			}

			materialsZoneEnabled = false;

			if (refillMaterialsZoneAfterDelayOnEmpty) {
				stopRefillMaterialsZoneCoroutine ();

				refillCoroutine = StartCoroutine (refillMaterialsZoneCoroutine ());
			}
		}
	}

	public bool isMaterialsZoneEmpty ()
	{
		return materialsZoneEmpty;
	}

	public bool isCanBeExtractedByExternalElementsEnabled ()
	{
		return canBeExtractedByExternalElementsEnabled;
	}

	public int getMaterialZoneID ()
	{
		return materialZoneID;
	}

	public void stopRefillMaterialsZoneCoroutine ()
	{
		if (refillCoroutine != null) {
			StopCoroutine (refillCoroutine);
		}

		refillInProcess = false;
	}

	IEnumerator refillMaterialsZoneCoroutine ()
	{
		refillInProcess = true;

		yield return new WaitForSecondsRealtime (delayToRefillMaterialsOnEmpty);

		refillInProcess = false;

		setMaterialsZoneFullState (true);
	}

	public void checkMaterialZoneToExtractExternally ()
	{
		bool checkMaterialZoneResult = true;

		if (isMaterialsZoneEmpty ()) {
			checkMaterialZoneResult = false;
		}

		Vector3 newPosition = Vector3.zero;
		Quaternion newRotation = Quaternion.identity;

		if (positionToSpawnExtractedObjects != null) {
			newPosition = positionToSpawnExtractedObjects.position;
			newRotation = positionToSpawnExtractedObjects.rotation;
		} else {
			newPosition = transform.position;
			newRotation = transform.rotation;
		}

		if (checkMaterialZoneResult) {

			bool allMaterialsOnZoneEmpty = true;

			int currentExtractionAmount = 0;

			for (int j = 0; j < materialZoneInfoList.Count; j++) {
				materialZoneInfo currentMaterialZoneInfo = materialZoneInfoList [j];

				bool canExtractMaterialResult = true;

				if (currentMaterialZoneInfo.materialAmount <= 0) {
					canExtractMaterialResult = false;
				}

				if (canExtractMaterialResult) {
					currentExtractionAmount = materialExtractionAmount;

					if (currentMaterialZoneInfo.useCustomMaterialExtractionAmount) {
						currentExtractionAmount = currentMaterialZoneInfo.customMaterialExtractionAmount;
					}

					int amountToObtain = currentExtractionAmount;

					currentMaterialZoneInfo.materialAmount -= amountToObtain;

					if (currentMaterialZoneInfo.useInfiniteMaterialAmount) {
						currentMaterialZoneInfo.materialAmount = 1;
					}
						
					if (!mainInventoryManagerLocated) {
						checkGetMainInventoryManager ();
					}

					if (mainInventoryManagerLocated) {
	
						GameObject newMaterialObject = 
							mainInvetoryListManager.spawnInventoryObjectByName (currentMaterialZoneInfo.materialName,  
								amountToObtain, newPosition, newRotation); 

						if (newMaterialObject != null) {
							newMaterialObject.transform.position += Random.insideUnitSphere * maxRadiusToInstantiate;

							if (addForceToSpawnedObjects) {
								if (newMaterialObject != null) {
									Rigidbody objectRigidbody = newMaterialObject.GetComponent<Rigidbody> ();

									if (objectRigidbody != null) {
										objectRigidbody.AddExplosionForce (forceAmountOnSpawnedObjects,
											newPosition, forceRadiusOnSpawnedObjects, 1, forceModeOnSpawnedObjects);
									}
								}
							}
						} else {
							print ("not found " + currentMaterialZoneInfo.materialName);
						}
					}

					allMaterialsOnZoneEmpty = false;
				}
			}

			if (allMaterialsOnZoneEmpty) {
				if (getRemainMaterialsAmount () <= 0) {
					setMaterialsZoneEmptyState (true);
				}
			}
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

	[System.Serializable]
	public class materialZoneInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string materialName;

		public int maxMaterialAmount;

		public int materialAmount;

		public bool useInfiniteMaterialAmount;

		[Space]
		[Header ("Other Settings")]
		[Space]

		public bool useCustomMaterialExtractionAmount;

		public int customMaterialExtractionAmount;

		//		public bool useCustomExtractionRate;
		//		public float extractionRate;

		[Space]

		public int materialID;
	}
}
