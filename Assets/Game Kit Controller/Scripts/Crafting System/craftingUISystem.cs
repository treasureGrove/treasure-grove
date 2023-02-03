using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class craftingUISystem : ingameMenuPanel
{
	[Header ("Main Settings")]
	[Space]

	public bool spawnObjectButtonsEnabled = true;
	public int amountOfObjectButtonsToSpawn = 50;
	public int amountOfIngredientsButtonsToSpawn = 50;
	public int amountOfInventoryButtonsToSpawn = 100;
	public int amountOnRepareIngredientsButtonsToSpawn = 30;
	public int amountOfObjectStatsButtonToSpawn = 0;
	public int amountOfObjectToCraftInTimeButtonToSpawn = 20;

	[Space]
	[Header ("Workbench Settings")]
	[Space]

	public bool craftingMenuOnlyUsableOnWorkbenchesEnabled;

	public bool repairObjectsOnlyOnWorkbenchEnabled;

	public bool disassembleObjectsOnlyOnWorkbenchEnabled;

	public bool upgradeObjectsOnlyOnWorkbencheEnabled;

	[Space]
	[Header ("Crafting Template Settings")]
	[Space]

	public List<craftingBlueprintInfoTemplateData> craftingBlueprintInfoTemplateDataList = new List<craftingBlueprintInfoTemplateData> ();

	[Space]
	[Header ("UI Panel Settings")]
	[Space]

	public List<panelCategoryInfo> panelCategoryInfoList = new List<panelCategoryInfo> ();

	[Space]
	[Header ("Message Settings")]
	[Space]

	[TextArea (3, 5)]public string messageOnObjectCreated;
	[TextArea (3, 5)]public string messageOnUnableToCreateObject;
	[TextArea (3, 5)]public string messageOnObjectRepaired;
	[TextArea (3, 5)]public string messageOnUnableToRepairObject;
	[TextArea (3, 5)]public string messageOnObjectDisassembled;

	[Space]

	[TextArea (3, 5)]public string messageOnUnableToCreateObjectFromStats;

	[TextArea (3, 5)]public string messageToShowObjectStats;

	[Space]

	public string objectNameField = "-OBJECT-";

	public string objectAmountField = "-AMOUNT-";

	public string objectStatsField = "-STATS-";

	[Space]
	[Header ("Blueprints/recipes unlocked")]
	[Space]

	public bool useOnlyBlueprintsUnlocked;
	public List<string> blueprintsUnlockedList = new List<string> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool repairObjectsOnlyIfFullyBroken = true;

	public bool useUpgradePanelEnabled = true;

	public bool ignoreStatsOnCraftObjectsEnabled;

	public bool showObjectStatsOnSelectObject;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool showingInventoryList;

	public bool showAllCategoriesActive = true;

	public bool mainCraftingSystemAssigned;

	public bool componentsAssigned;

	public bool menuOpened;

	public int currentAmountToCreate = 1;

	public string currentObjectSelectedName;

	public string currentObjectCategorySelectedName;

	public int currentObjectSelectedIndex;

	public bool objectFilterActive;

	public bool currentObjectIsBroken;

	public bool currentObjectCanbeDisassembled;

	public bool currentObjectCanBePlaced;

	public bool upgradePanelActive;

	[Space]
	[Header ("Current Workbench Debug")]
	[Space]

	public bool menuOpenedFromWorkbench;

	public List<string> objectCategoriesToCraftAvailableOnCurrentBench = new List<string> ();

	public craftingWorkbenchSystem currentCraftingWorkbenchSystem;

	[Space]
	[Header ("Object List Debug")]
	[Space]

	public List<craftingObjectButtonInfo> craftingObjectButtonInfoList = new List<craftingObjectButtonInfo> ();

	public List<craftingObjectButtonInfo> currentObjectIngredientsInfoList = new List<craftingObjectButtonInfo> ();

	public List<craftingObjectButtonInfo> currentInventoryInfoList = new List<craftingObjectButtonInfo> ();

	public List<craftingObjectButtonInfo> currentRepairIngredientsInfoList = new List<craftingObjectButtonInfo> ();

	public List<craftingObjectButtonInfo> currentCraftObjectInTimeInfoList = new List<craftingObjectButtonInfo> ();

	public List<objectStatButtonInfo> objectStatButtonInfoList = new List<objectStatButtonInfo> ();

	[Space]

	public List<craftObjectInTimeInfo> craftObjectInTimeInfoList = new List<craftObjectInTimeInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnShowInventoryList;
	public UnityEvent eventOnShowBlueprintList;

	[Space]

	public UnityEvent eventOnOpenUpgradeObjectPanel;
	public UnityEvent eventOnCloseUpgradeObjectPanel;

	[Space]

	public eventParameters.eventToCallWithString eventOnObjectCreated;
	public eventParameters.eventToCallWithString eventOnObjectNotCreated;
	public eventParameters.eventToCallWithString eventOnObjectRepaired;
	public eventParameters.eventToCallWithString eventOnObjectDisassembled;

	public eventParameters.eventToCallWithString eventOnShowObjectStats;

	[Space]
	[Header ("Components")]
	[Space]

	public craftingSystem mainCraftingSystem;

	[Space]
	[Header ("UI Components")]
	[Space]

	public GameObject craftingButtonInfoPrefab;

	public Text currentObjectNameText;
	public Text currentObjectDescriptionText;

	public Text currentAmountOfObjectsSelectedText;

	public InputField searcherInputField;

	public Text currentObjectCategoryNameText;

	public RectTransform craftingButtonInfoPanelList;

	public RawImage currentObjectSelectedImage;

	public GameObject disassembleButton;
	public GameObject repairButton;

	public GameObject repairIngredientsPanel;

	[Space]
	[Header ("UI Repair Components")]
	[Space]

	public RectTransform repairPanelList;
	public GameObject repairPanelListElementPrefab;

	[Space]
	[Header ("UI Place Object Components")]
	[Space]

	public GameObject placeObjectButton;

	[Space]
	[Header ("UI Hover Components")]
	[Space]

	public RectTransform hoverBlueprintPanel;

	public RectTransform hoverBlueprintPanelList;

	public GameObject hoverBlueprintPanelListElementPrefab;

	[Space]
	[Header ("UI Upgrade Components")]
	[Space]

	public GameObject upgradeButton;
	public GameObject upgradePanel;

	public RectTransform objectStatsPanelList;

	public GameObject objectStatPanelListElementPrefab;

	[Space]
	[Header ("UI Craft Objects In Time Components")]
	[Space]

	public RectTransform craftObjectsInTimePanelList;
	public GameObject craftObjectsInTimePanelListElementPrefab;

	[Space]
	[Header ("Inventory Components")]
	[Space]

	public RectTransform inventoryPanelList;

	public GameObject inventoryPanelListElementPrefab;


	GameObject currentButtonObjectPressed;

	bool UIElementSpawned;

	craftingObjectButtonInfo currentButtonInfo;

	GameObject currentObjectMesh;

	bool unableToCreateObjectsFromStatsActive;


	void Start ()
	{
		if (!mainCraftingSystemAssigned) {
			if (mainCraftingSystem != null) {
				mainCraftingSystemAssigned = true;
			}
		}
	}

	public void showInventoryList ()
	{
		showingInventoryList = true;

		eventOnShowInventoryList.Invoke ();

		updateCategoryObjectPanelInfo ();

		selectFirstCategoryPanel ();

		disassembleButton.SetActive (true);

		repairButton.SetActive (true);
	}

	public void showBlueprintList ()
	{
		showingInventoryList = false;

		showAllCategoriesActive = true;

		repairIngredientsPanel.SetActive (false);

		eventOnShowBlueprintList.Invoke ();

		updateCategoryObjectPanelInfo ();

		selectFirstCategoryPanel ();


		placeObjectButton.SetActive (false);

		disassembleButton.SetActive (false);

		repairButton.SetActive (false);
	}

	//Upgrade Functions
	public void showUpgradePanel ()
	{
		if (!useUpgradePanelEnabled) {
			return;
		}

		upgradePanelActive = true;

		eventOnOpenUpgradeObjectPanel.Invoke ();

		upgradePanel.SetActive (true);
	}

	public void closeUpgradePanel ()
	{
		if (!useUpgradePanelEnabled) {
			return;
		}

		upgradePanelActive = false;

		eventOnCloseUpgradeObjectPanel.Invoke ();

		upgradePanel.SetActive (false);
	}

	void updateObjectStatsPanelListInfo ()
	{
		for (int i = 0; i < objectStatButtonInfoList.Count; i++) {
			objectStatButtonInfo currentObjectStatButtonInfo = objectStatButtonInfoList [i];

			if (currentObjectStatButtonInfo.buttonGameObject.activeSelf) {
				currentObjectStatButtonInfo.buttonGameObject.SetActive (false);

				currentObjectStatButtonInfo.objectAssigned = false;
			}
		}

		List<objectStatInfo> objectStatInfoList = mainCraftingSystem.getStatsFromObjectByName (currentObjectSelectedName);

		if (objectStatInfoList != null && objectStatInfoList.Count > 0) {
			if (showDebugPrint) {
				print ("list of stats found " + objectStatInfoList.Count);
			}

			for (int i = 0; i < objectStatInfoList.Count; i++) {
				int currentAmount = mainCraftingSystem.getInventoryObjectAmountByName (objectStatInfoList [i].Name);

				if (currentAmount < 0) {
					currentAmount = 0;
				}

				objectStatButtonInfo currentObjectStatButtonInfo = objectStatButtonInfoList [i];

				currentObjectStatButtonInfo.buttonGameObject.SetActive (true);

				currentObjectStatButtonInfo.objectAssigned = true;

				currentObjectStatButtonInfo.statNameText.text = objectStatInfoList [i].Name;

				currentObjectStatButtonInfo.statImage.texture = objectStatInfoList [i].statIcon;

				if (currentObjectStatButtonInfo.statSlider.gameObject.activeSelf != objectStatInfoList [i].statIsAmount) {
					currentObjectStatButtonInfo.statSlider.gameObject.SetActive (objectStatInfoList [i].statIsAmount);

					currentObjectStatButtonInfo.statValueText.gameObject.SetActive (objectStatInfoList [i].statIsAmount);
				}

				if (currentObjectStatButtonInfo.statToggle.gameObject.activeSelf == objectStatInfoList [i].statIsAmount) {
					currentObjectStatButtonInfo.statToggle.gameObject.SetActive (objectStatInfoList [i].statIsAmount);
				}

				if (objectStatInfoList [i].statIsAmount) {
					currentObjectStatButtonInfo.statValueText.text = objectStatInfoList [i].currentFloatValue.ToString ();

					currentObjectStatButtonInfo.statSlider.value = objectStatInfoList [i].currentFloatValue;

					currentObjectStatButtonInfo.statSlider.maxValue = objectStatInfoList [i].maxFloatValue;
				} else {
					currentObjectStatButtonInfo.statToggle.isOn = objectStatInfoList [i].currentBoolState;

				}
			}
		}
	}


	public void setShowAllObjectsOrOnlyAvailableToCreate (bool state)
	{
		if (state) {

		} else {

		}
	}

	public void repairObject ()
	{
		if (currentObjectIsBroken) {
			if (currentButtonInfo != null) {
				if (canObjectBeRepaired (currentButtonInfo.objectCategoryName, currentButtonInfo.objectName)) {

					int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (currentObjectCategorySelectedName));

					if (currentCategoryIndex > -1) {

						int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (currentObjectSelectedName));

						if (currentIndex > -1) {
							List<craftingIngredientObjectInfo> repairIngredientObjectInfoList = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].repairIngredientObjectInfoList;

							for (int i = 0; i < repairIngredientObjectInfoList.Count; i++) {
								mainCraftingSystem.removeObjectAmountFromInventoryByName (repairIngredientObjectInfoList [i].Name, 
									repairIngredientObjectInfoList [i].amountRequired);
							}
						}
					}

					if (mainCraftingSystem.repairDurabilityObjectByIndex (currentObjectSelectedIndex)) {
						string newMessage = messageOnObjectRepaired;

						if (newMessage.Contains (objectNameField)) {
							newMessage = newMessage.Replace (objectNameField, currentObjectSelectedName);
						}

						eventOnObjectRepaired.Invoke (newMessage);

						selectFirstCategoryPanel ();

						mainCraftingSystem.checkStateOnCraftObject ();
					}
				} else {
					string newMessage = messageOnUnableToRepairObject;

					if (newMessage.Contains (objectNameField)) {
						newMessage = newMessage.Replace (objectNameField, currentObjectSelectedName);
					}

					eventOnObjectRepaired.Invoke (newMessage);
				}
			}
		}
	}

	public void disassembleObject ()
	{
		if (currentObjectCanbeDisassembled) {
			mainCraftingSystem.removeObjectAmountFromInventoryByIndex (currentObjectSelectedIndex, 1);

			int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (currentObjectCategorySelectedName));

			if (currentCategoryIndex > -1) {

				int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (currentObjectSelectedName));

				if (currentIndex > -1) {
					List<craftingIngredientObjectInfo> currentIngredientObjectInfoList = null;

					if (currentObjectIsBroken) {
						currentIngredientObjectInfoList =
							craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].brokenIngredientObjectInfoList;
					} else {
						currentIngredientObjectInfoList =
							craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].disassembleIngredientObjectInfoList;
					}

						
					for (int i = 0; i < currentIngredientObjectInfoList.Count; i++) {
						mainCraftingSystem.giveInventoryObjectToCharacter (currentIngredientObjectInfoList [i].Name, 
							currentIngredientObjectInfoList [i].amountRequired);
					}
				}
			}

			string newMessage = messageOnObjectDisassembled;

			if (newMessage.Contains (objectNameField)) {
				newMessage = newMessage.Replace (objectNameField, currentObjectSelectedName);
			}

			eventOnObjectDisassembled.Invoke (newMessage);

			if (showingInventoryList) {
				showInventoryList ();
			} else {
				showBlueprintList ();
			}

			mainCraftingSystem.checkStateOnCraftObject ();
		}
	}

	public void checkShowAllObjectsByFilter ()
	{
		string currentTextToSearch = searcherInputField.text;

		if (currentTextToSearch != "") {
			currentTextToSearch = currentTextToSearch.ToLower ();

			print (currentTextToSearch);

			if (showingInventoryList) {
				for (int i = 0; i < currentInventoryInfoList.Count; i++) {
					craftingObjectButtonInfo currentInventoryInfo = currentInventoryInfoList [i];

					if (currentInventoryInfo.buttonGameObject.activeSelf) {
						currentInventoryInfo.buttonGameObject.SetActive (false);
					}
				}

				for (int i = 0; i < currentInventoryInfoList.Count; i++) {
					craftingObjectButtonInfo currentInventoryInfo = currentInventoryInfoList [i];

					if (currentInventoryInfo.objectName != "") { 
						string objectName = currentInventoryInfo.objectName.ToLower ();

						if (objectName.Contains (currentTextToSearch) ||
						    objectName.Equals (currentTextToSearch)) {

							currentInventoryInfo.buttonGameObject.SetActive (true);
						}
					}
				}
			} else {
				for (int i = 0; i < craftingObjectButtonInfoList.Count; i++) {
					craftingObjectButtonInfo currentCraftingInfo = craftingObjectButtonInfoList [i];

					if (currentCraftingInfo.buttonGameObject.activeSelf) {
						currentCraftingInfo.buttonGameObject.SetActive (false);
					}
				}

				for (int i = 0; i < craftingObjectButtonInfoList.Count; i++) {
					craftingObjectButtonInfo currentCraftingInfo = craftingObjectButtonInfoList [i];

					if (currentCraftingInfo.objectName != "") { 
						bool canShowObjectResult = true;

						if (useOnlyBlueprintsUnlocked) {
							if (!blueprintsUnlockedList.Contains (currentCraftingInfo.objectName)) {
								canShowObjectResult = false;
							}
						}

						if (canShowObjectResult) {
							string objectName = currentCraftingInfo.objectName.ToLower ();

							if (objectName.Contains (currentTextToSearch) ||
							    objectName.Equals (currentTextToSearch)) {

								if (isCurrentObjectCategoryAvailable (currentCraftingInfo.objectCategoryName)) {
									currentCraftingInfo.buttonGameObject.SetActive (true);
								}
							}
						}
					}
				}
			}

			showAllCategoriesActive = false;

			objectFilterActive = true;
		} else {
			removeObjectsFilter ();
		}
	}

	public void removeObjectsFilter ()
	{
		if (objectFilterActive) {
			searcherInputField.text = "";

			showAllCategoriesActive = true;

			objectFilterActive = false;

			updateCategoryObjectPanelInfoActive ();
		}
	}

	bool checkIfStatValueAvailable (string statName, int statAmount)
	{
		return mainCraftingSystem.checkIfStatValueAvailable (statName, statAmount);
	}

	void addOrRemovePlayerStatAmount (string statName, int statAmount)
	{
		mainCraftingSystem.addOrRemovePlayerStatAmount (statName, statAmount);
	}

	public void confirmToCreate ()
	{
		if (!menuOpened) {
			return;
		}

		float timeToCraftObject = 0;
			
		bool craftObjectInTimeResult = isObjectCreatedInTime (currentObjectCategorySelectedName, currentObjectSelectedName, ref timeToCraftObject);

		if (craftObjectInTimeResult) {
			craftObjectInTime (timeToCraftObject);
		} else {
			bool allObjectsCreated = createObjects (currentAmountToCreate, currentObjectCategorySelectedName, currentObjectSelectedName, menuOpenedFromWorkbench, -1);

			if (allObjectsCreated) {
				string newMessage = messageOnObjectCreated;

				if (newMessage.Contains (objectNameField)) {
					newMessage = newMessage.Replace (objectNameField, currentObjectSelectedName);
				}

				if (currentAmountToCreate > 1) {
					if (newMessage.Contains (objectAmountField)) {
						newMessage = newMessage.Replace (objectAmountField, (" x " + currentAmountToCreate.ToString ()));
					}
				} else {
					if (newMessage.Contains (objectAmountField)) {
						newMessage = newMessage.Replace (objectAmountField, "");
					}
				}

				eventOnObjectCreated.Invoke (newMessage);

				checkButtonPressed (currentButtonObjectPressed);

				mainCraftingSystem.checkStateOnCraftObject ();
			} else {
				string newMessage = messageOnUnableToCreateObject;

				if (unableToCreateObjectsFromStatsActive) {
					newMessage = messageOnUnableToCreateObjectFromStats;
				}

				if (newMessage.Contains (objectNameField)) {
					newMessage = newMessage.Replace (objectNameField, currentObjectSelectedName);
				}

				if (unableToCreateObjectsFromStatsActive) {
					showObjectStatsInfo ();
				}

				eventOnObjectNotCreated.Invoke (newMessage);
			}
		}
	}

	void showObjectStatsInfo ()
	{
		string statsInfoString = getCrafStatsString ();

		if (statsInfoString != "") {
			if (messageToShowObjectStats.Contains (objectStatsField)) {
				messageToShowObjectStats = messageToShowObjectStats.Replace (objectStatsField, statsInfoString);
			}
		}

		if (messageToShowObjectStats != "") {
			eventOnShowObjectStats.Invoke (messageToShowObjectStats);
		}
	}

	bool createObjects (int amountToCreate, string categoryName, string objectName, bool menuOpenedFromWorkbenchValue, int workbenchID)
	{
		bool allObjectsCreated = true;

		bool canBeCreatedResult = true;

		bool canCreateNextObject = true;

		for (int j = 0; j < amountToCreate; j++) {
			if (canCreateNextObject) {
				canBeCreatedResult = canObjectBeCreated (categoryName, objectName, 1, true);

				if (canBeCreatedResult) {

					int amountToObtain = 1;

					int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (categoryName));

					if (currentCategoryIndex > -1) {

						int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (objectName));

						if (currentIndex > -1) {
							craftingBlueprintInfoTemplate currentcraftingBlueprintInfoTemplate = 
								craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex];

							amountToObtain = currentcraftingBlueprintInfoTemplate.amountObtained;

							List<craftingIngredientObjectInfo> craftingIngredientObjectInfoList = currentcraftingBlueprintInfoTemplate.craftingIngredientObjectInfoList;

							for (int i = 0; i < craftingIngredientObjectInfoList.Count; i++) {
								mainCraftingSystem.removeObjectAmountFromInventoryByName (craftingIngredientObjectInfoList [i].Name, 
									craftingIngredientObjectInfoList [i].amountRequired);
							}
								
							if (!ignoreStatsOnCraftObjectsEnabled) {
								if (currentcraftingBlueprintInfoTemplate.checkStatsInfoToCraft) {
									for (int i = 0; i < currentcraftingBlueprintInfoTemplate.craftingStatInfoToCraftList.Count; i++) {
										craftingStatInfo currentcraftingStatInfo = currentcraftingBlueprintInfoTemplate.craftingStatInfoToCraftList [i];

										if (currentcraftingStatInfo.useStatValue) {
											addOrRemovePlayerStatAmount (currentcraftingStatInfo.statName, currentcraftingStatInfo.statValueToUse);
										}
									}
								}
							}
						}
					}

					//comprobar si hay espacio para obtener la pieza antes

					bool storeObjectOnPlayerInventoryResult = true;

					if (menuOpenedFromWorkbenchValue) {
						if (workbenchID != -1) {
							craftingWorkbenchSystem[] craftingWorkbenchSystemList = FindObjectsOfType<craftingWorkbenchSystem> ();

							foreach (craftingWorkbenchSystem currentWorkbench in craftingWorkbenchSystemList) {
								if (currentWorkbench.getWorkbenchID () == workbenchID) {
									currentWorkbench.addInventoryObjectByName (objectName, amountToObtain);

									storeObjectOnPlayerInventoryResult = false;
								}
							}
						} else {
							if (currentCraftingWorkbenchSystem.storeCraftedObjectsOnInventoryBank) {
								currentCraftingWorkbenchSystem.addInventoryObjectByName (objectName, amountToObtain);

								storeObjectOnPlayerInventoryResult = false;
							}
						}
					}

					if (storeObjectOnPlayerInventoryResult) {
						mainCraftingSystem.giveInventoryObjectToCharacter (objectName, amountToObtain);
					}
				} else {
					canCreateNextObject = false;

					allObjectsCreated = false;
				}
			}
		}

		return allObjectsCreated;
	}

	string getCrafStatsString ()
	{
		string statsString = "";

		int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (currentObjectCategorySelectedName));

		if (currentCategoryIndex > -1) {

			int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (currentObjectSelectedName));

			if (currentIndex > -1) {
				craftingBlueprintInfoTemplate currentcraftingBlueprintInfoTemplate = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex];

				statsString += "\n";

//				statsString += "Stats Required: \n";

				for (int j = 0; j < currentcraftingBlueprintInfoTemplate.craftingStatInfoToCraftList.Count; j++) {
					craftingStatInfo currentcraftingStatInfo = currentcraftingBlueprintInfoTemplate.craftingStatInfoToCraftList [j];

					statsString += " -" + currentcraftingStatInfo.statName;
						
					if (currentcraftingStatInfo.useStatValue) {							
						statsString += ": " + currentcraftingStatInfo.statValueToUse;				
					} else {
						statsString += ": " + currentcraftingStatInfo.valueRequired;
					}

					statsString += "\n";
				}						
			}
		}

		return statsString;
	}

	void craftObjectInTime (float timeToCraftObjectValue)
	{
		craftObjectInTimeInfo newCraftObjectInTimeInfo = new craftObjectInTimeInfo ();

		newCraftObjectInTimeInfo.objectName = currentObjectSelectedName;

		newCraftObjectInTimeInfo.objectCategoryName = currentObjectCategorySelectedName;

		newCraftObjectInTimeInfo.amount = currentAmountToCreate;

		newCraftObjectInTimeInfo.timeToCraftObject = timeToCraftObjectValue;

		if (menuOpenedFromWorkbench) {
			newCraftObjectInTimeInfo.objectCreatedOnWorkbench = true;

			newCraftObjectInTimeInfo.workbenchID = currentCraftingWorkbenchSystem.getWorkbenchID ();
		}

		craftObjectInTimeInfoList.Add (newCraftObjectInTimeInfo);

		newCraftObjectInTimeInfo.craftObjectCoroutine = StartCoroutine (craftObjectInTimeCoroutine (newCraftObjectInTimeInfo));
	}

	IEnumerator craftObjectInTimeCoroutine (craftObjectInTimeInfo newCraftObjectInTimeInfo)
	{
		float timeToCraftObject = newCraftObjectInTimeInfo.timeToCraftObject * newCraftObjectInTimeInfo.amount;

		bool buttonLocated = false;

		for (int i = 0; i < currentCraftObjectInTimeInfoList.Count; i++) {
			if (!buttonLocated) {
				if (!currentCraftObjectInTimeInfoList [i].objectAssigned) {
					craftingObjectButtonInfo currentCraftingObjectButtonInfo = currentCraftObjectInTimeInfoList [i];

					currentCraftingObjectButtonInfo.buttonGameObject.SetActive (true);

					currentCraftingObjectButtonInfo.objectAssigned = true;

					currentCraftingObjectButtonInfo.objectNameText.text = newCraftObjectInTimeInfo.objectName;

					currentCraftingObjectButtonInfo.objectImage.texture = 
						mainCraftingSystem.getInventoryObjectIconByName (newCraftObjectInTimeInfo.objectName);

					currentCraftingObjectButtonInfo.objectSlider.maxValue = timeToCraftObject;

					newCraftObjectInTimeInfo.currentCraftingObjectButtonInfo = currentCraftingObjectButtonInfo;

					buttonLocated = true;
				}
			}
		}

		bool targetReached = false;

		float timer = timeToCraftObject;

		while (!targetReached) {
			timer -= Time.deltaTime;

			if (timer <= 0) {
				targetReached = true;
			} else {
				newCraftObjectInTimeInfo.currentCraftingObjectButtonInfo.amountAvailableToCreateText.text = timer.ToString ("0.#");

				newCraftObjectInTimeInfo.currentCraftingObjectButtonInfo.objectSlider.value = timer;
			}

			yield return null;
		}

		createObjects (newCraftObjectInTimeInfo.amount,
			newCraftObjectInTimeInfo.objectCategoryName,
			newCraftObjectInTimeInfo.objectName, 
			newCraftObjectInTimeInfo.objectCreatedOnWorkbench, newCraftObjectInTimeInfo.workbenchID);

		if (menuOpened) {
			string newMessage = messageOnObjectCreated;

			if (newMessage.Contains (objectNameField)) {
				newMessage = newMessage.Replace (objectNameField, newCraftObjectInTimeInfo.objectName);
			}

			if (currentAmountToCreate > 1) {
				if (newMessage.Contains (objectAmountField)) {
					newMessage = newMessage.Replace (objectAmountField, (" x " + newCraftObjectInTimeInfo.amount.ToString ()));
				}
			} else {
				if (newMessage.Contains (objectAmountField)) {
					newMessage = newMessage.Replace (objectAmountField, "");
				}
			}

			eventOnObjectCreated.Invoke (newMessage);

			checkButtonPressed (currentButtonObjectPressed);
		}

		craftObjectInTimeInfoList.Remove (newCraftObjectInTimeInfo);

		newCraftObjectInTimeInfo.currentCraftingObjectButtonInfo.buttonGameObject.SetActive (false);

		newCraftObjectInTimeInfo.currentCraftingObjectButtonInfo.objectAssigned = false;

		StopCoroutine (newCraftObjectInTimeInfo.craftObjectCoroutine);
	}


	public void increaseOrDecreaseAmountToCreate (bool state)
	{
		if (state) {
			currentAmountToCreate++;

			bool canCreateNextObject = true;

			if (!canObjectBeCreated (currentObjectCategorySelectedName, currentObjectSelectedName, currentAmountToCreate, true)) {
				canCreateNextObject = false;
			}

			if (showDebugPrint) {
				print ("can create all current amount result " + canCreateNextObject);
			}

			if (!canCreateNextObject) {
				currentAmountToCreate--;
			}
		} else {
			currentAmountToCreate--;
		}

		if (currentAmountToCreate < 1) {
			currentAmountToCreate = 1;
		}

		updatecurrentAmountOfObjectsSelectedText ();
	}

	void updatecurrentAmountOfObjectsSelectedText ()
	{
		currentAmountOfObjectsSelectedText.text = currentAmountToCreate.ToString ();
	}

	public void resetAmountToCreate ()
	{
		currentAmountToCreate = 1;

		updatecurrentAmountOfObjectsSelectedText ();
	}

	public void selectFirstObjectAvailableButton ()
	{
		if (showAllCategoriesActive) {
			if (showingInventoryList) {
				checkButtonPressed (currentInventoryInfoList [0].buttonGameObject);
			} else {
				if (useOnlyBlueprintsUnlocked) {
					for (int i = 0; i < craftingObjectButtonInfoList.Count; i++) {
						craftingObjectButtonInfo currentCraftingInfo = craftingObjectButtonInfoList [i];

						if (blueprintsUnlockedList.Contains (currentCraftingInfo.objectName)) {
							if (isCurrentObjectCategoryAvailable (currentCraftingInfo.objectCategoryName)) {
								checkButtonPressed (currentCraftingInfo.buttonGameObject);
							}
							return;
						}
					}
				} else {
					for (int i = 0; i < craftingObjectButtonInfoList.Count; i++) {
						craftingObjectButtonInfo currentCraftingInfo = craftingObjectButtonInfoList [i];

						if (isCurrentObjectCategoryAvailable (currentCraftingInfo.objectCategoryName)) {
							checkButtonPressed (currentCraftingInfo.buttonGameObject);

							return;
						}
					}
				}

				removeObjectPanelInfoContent ();
			}
		} else {
			int firstObjectIndexActive = -1;

			if (showingInventoryList) {
				for (int i = 0; i < currentInventoryInfoList.Count; i++) {
					craftingObjectButtonInfo currentInventoryInfo = currentInventoryInfoList [i];

					if (firstObjectIndexActive == -1 && currentInventoryInfo.buttonGameObject.activeSelf) {
						if (canObjectBeCreated (currentInventoryInfo.objectCategoryName, currentInventoryInfo.objectName, 1, false)) {
							checkButtonPressed (currentInventoryInfo.buttonGameObject);

							return;
						} else {
							firstObjectIndexActive = i;
						}
					}
				}

				if (firstObjectIndexActive != -1) {
					checkButtonPressed (currentInventoryInfoList [firstObjectIndexActive].buttonGameObject);
				} else {
					removeObjectPanelInfoContent ();
				}
			} else {
				for (int i = 0; i < craftingObjectButtonInfoList.Count; i++) {
					craftingObjectButtonInfo currentCraftingInfo = craftingObjectButtonInfoList [i];

					if (firstObjectIndexActive == -1 && currentCraftingInfo.buttonGameObject.activeSelf) {
						if (isCurrentObjectCategoryAvailable (currentCraftingInfo.objectCategoryName) &&
						    canObjectBeCreated (currentCraftingInfo.objectCategoryName, currentCraftingInfo.objectName, 1, false)) {
							checkButtonPressed (currentCraftingInfo.buttonGameObject);

							return;
						} else {
							firstObjectIndexActive = i;
						}
					}
				}

				if (firstObjectIndexActive != -1) {
					checkButtonPressed (craftingObjectButtonInfoList [firstObjectIndexActive].buttonGameObject);
				} else {
					removeObjectPanelInfoContent ();
				}
			}
		}
	}

	bool canObjectBeCreated (string objectCategoryName, string objectName, int amountMultiplier, bool checkForStats)
	{
		unableToCreateObjectsFromStatsActive = false;

		int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (objectCategoryName));

		if (currentCategoryIndex > -1) {

			int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (objectName));

			if (currentIndex > -1) {
				craftingBlueprintInfoTemplate currentcraftingBlueprintInfoTemplate = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex];
					
				List<craftingIngredientObjectInfo> craftingIngredientObjectInfoList = currentcraftingBlueprintInfoTemplate.craftingIngredientObjectInfoList;

				if (!ignoreStatsOnCraftObjectsEnabled) {
					if (checkForStats) {
						if (currentcraftingBlueprintInfoTemplate.checkStatsInfoToCraft) {
							for (int j = 0; j < currentcraftingBlueprintInfoTemplate.craftingStatInfoToCraftList.Count; j++) {
								craftingStatInfo currentcraftingStatInfo = currentcraftingBlueprintInfoTemplate.craftingStatInfoToCraftList [j];

								if (currentcraftingStatInfo.useStatValue) {
									if (!checkIfStatValueAvailable (currentcraftingStatInfo.statName, currentcraftingStatInfo.statValueToUse)) {
										unableToCreateObjectsFromStatsActive = true;

										return false;
									}					
								} else {
									if (!checkIfStatValueAvailable (currentcraftingStatInfo.statName, currentcraftingStatInfo.valueRequired)) {
										unableToCreateObjectsFromStatsActive = true;

										return false;
									}	
								}
							}
						}
					}
				}

				for (int i = 0; i < craftingIngredientObjectInfoList.Count; i++) {
					int currentAmount = mainCraftingSystem.getInventoryObjectAmountByName (craftingIngredientObjectInfoList [i].Name);

					if (currentAmount < (craftingIngredientObjectInfoList [i].amountRequired * amountMultiplier)) {
						return false;
					}
				}

				return true;
			}
		}

		return false;
	}

	bool canObjectBeRepaired (string objectCategoryName, string objectName)
	{
		int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (objectCategoryName));

		if (currentCategoryIndex > -1) {

			int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (objectName));

			if (currentIndex > -1) {
				List<craftingIngredientObjectInfo> repairIngredientObjectInfoList = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].repairIngredientObjectInfoList;

				for (int i = 0; i < repairIngredientObjectInfoList.Count; i++) {
					int currentAmount = mainCraftingSystem.getInventoryObjectAmountByName (repairIngredientObjectInfoList [i].Name);

					if (currentAmount < repairIngredientObjectInfoList [i].amountRequired) {
						return false;
					}
				}

				return true;
			}
		}

		return false;
	}

	bool isObjectCreatedInTime (string objectCategoryName, string objectName, ref float timeToCraftObject)
	{
		int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (objectCategoryName));

		if (currentCategoryIndex > -1) {

			int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (objectName));

			if (currentIndex > -1) {
				timeToCraftObject = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].timeToCraftObject;

				return craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].craftObjectInTime;
			}
		}

		return false;
	}

	public void checkButtonPressedWithoutCheckIfPreviouslyPressed (GameObject buttonObject)
	{
		checkButton (buttonObject);
	}

	public void checkButtonPressed (GameObject buttonObject)
	{
		if (showDebugPrint) {
			print (buttonObject.name);
		}

//		if (currentButtonObjectPressed != null && buttonObject == currentButtonObjectPressed) {
//			return;
//		}

		checkButton (buttonObject);
	}

	void checkButton (GameObject buttonObject)
	{
		currentButtonObjectPressed = buttonObject;

		craftingObjectButtonInfo currentCraftingObjectButtonInfo = currentButtonObjectPressed.GetComponent<craftingObjectButtonInfo> ();

		if (currentCraftingObjectButtonInfo != null) {
//			print (currentCraftingObjectButtonInfo.objectName);

			currentButtonInfo = currentCraftingObjectButtonInfo;

			currentObjectSelectedName = currentCraftingObjectButtonInfo.objectName;

			if (craftingObjectButtonInfoList.Contains (currentCraftingObjectButtonInfo)) {
				currentObjectSelectedIndex = craftingObjectButtonInfoList.IndexOf (currentCraftingObjectButtonInfo);
			} else {
				if (currentInventoryInfoList.Contains (currentCraftingObjectButtonInfo)) {
					currentObjectSelectedIndex = currentInventoryInfoList.IndexOf (currentCraftingObjectButtonInfo);
				} 
			}

			updateObjectPanelInfo ();				
		}
	}

	void updateObjectPanelInfo ()
	{
		for (int i = 0; i < currentObjectIngredientsInfoList.Count; i++) {
			craftingObjectButtonInfo currentIngredientInfo = currentObjectIngredientsInfoList [i];

			if (currentIngredientInfo.buttonGameObject.activeSelf) {
				currentIngredientInfo.buttonGameObject.SetActive (false);

				currentIngredientInfo.objectAssigned = false;
			}
		}

		inventoryInfo curretInventoryInfo = null;

		if (showingInventoryList) {
			curretInventoryInfo = mainCraftingSystem.getInventoryInfoByIndex (currentObjectSelectedIndex);
		} else {
			curretInventoryInfo = mainCraftingSystem.getInventoryInfoByName (currentObjectSelectedName);
		}

		if (showAllCategoriesActive) {
			if (curretInventoryInfo != null) {
				currentObjectCategorySelectedName = curretInventoryInfo.categoryName;
			}
		}

		currentObjectIsBroken = false;

		currentObjectCanBePlaced = false;

		bool checkStatsToCreateObjectResult = false;

		int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (currentObjectCategorySelectedName));

		if (currentCategoryIndex > -1) {
			if (showDebugPrint) {
				print ("category of ingredients found");
			}

			int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (currentObjectSelectedName));

			if (currentIndex > -1) {
				currentObjectCanBePlaced = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].useObjectToPlace; 

				List<craftingIngredientObjectInfo> craftingIngredientObjectInfoList = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].craftingIngredientObjectInfoList;

				checkStatsToCreateObjectResult = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].checkStatsInfoToCraft; 

				if (showDebugPrint) {
					print ("list of ingredients found " + craftingIngredientObjectInfoList.Count);
				}

				for (int i = 0; i < craftingIngredientObjectInfoList.Count; i++) {
					int currentAmount = mainCraftingSystem.getInventoryObjectAmountByName (craftingIngredientObjectInfoList [i].Name);

					if (currentAmount < 0) {
						currentAmount = 0;
					}

					craftingObjectButtonInfo currentCraftingObjectButtonInfo = currentObjectIngredientsInfoList [i];

					currentCraftingObjectButtonInfo.buttonGameObject.SetActive (true);

					currentCraftingObjectButtonInfo.objectAssigned = true;

					currentCraftingObjectButtonInfo.objectNameText.text = craftingIngredientObjectInfoList [i].Name;

					currentCraftingObjectButtonInfo.objectImage.texture = mainCraftingSystem.getInventoryObjectIconByName (craftingIngredientObjectInfoList [i].Name);

					currentCraftingObjectButtonInfo.amountAvailableToCreateText.text = 
						(craftingIngredientObjectInfoList [i].amountRequired.ToString () + "/" + currentAmount.ToString ());

				}
			}
		}

		currentObjectNameText.text = currentObjectSelectedName;

		if (curretInventoryInfo != null) {
			string descriptionText = curretInventoryInfo.objectInfo;

			if (!ignoreStatsOnCraftObjectsEnabled) {
				if (checkStatsToCreateObjectResult) {
					string statsInfoString = getCrafStatsString ();

					if (statsInfoString != "") {
						descriptionText += "\nStats Required:" + statsInfoString;
					}

					if (showObjectStatsOnSelectObject) {
						showObjectStatsInfo ();
					}
				}
			}

			currentObjectDescriptionText.text = descriptionText;

			currentObjectSelectedImage.texture = curretInventoryInfo.icon;

			if (showingInventoryList) {
				if (repairObjectsOnlyIfFullyBroken) {
					currentObjectIsBroken = mainCraftingSystem.isObjectBroken (currentObjectSelectedIndex);
				} else {
					currentObjectIsBroken = !mainCraftingSystem.isObjectDurabilityComplete (currentObjectSelectedIndex);
				}

				currentObjectCanbeDisassembled = canCurrentObjectBeDisassembled ();

				//check disassembele UI elements
				bool canSetDisassembleUIElementsResult = currentObjectCanbeDisassembled;

				if (disassembleObjectsOnlyOnWorkbenchEnabled && currentObjectCanbeDisassembled) {
					if (menuOpenedFromWorkbench) {
						if (!currentCraftingWorkbenchSystem.disassembleObjectsOnlyOnWorkbenchEnabled) {
							canSetDisassembleUIElementsResult = false;
						}
					} else {
						canSetDisassembleUIElementsResult = false;
					}
				}

				disassembleButton.SetActive (canSetDisassembleUIElementsResult);


				//check repair UI elements
				bool canSetRepairUIElementsResult = currentObjectIsBroken;

				if (repairObjectsOnlyOnWorkbenchEnabled && currentObjectIsBroken) {
					if (menuOpenedFromWorkbench) {
						if (!currentCraftingWorkbenchSystem.repairObjectsOnlyOnWorkbenchEnabled) {
							canSetRepairUIElementsResult = false;
						}
					} else {
						canSetRepairUIElementsResult = false;
					}
				}

				repairButton.SetActive (canSetRepairUIElementsResult);

				repairIngredientsPanel.SetActive (canSetRepairUIElementsResult);


				//check place object UI elements
				bool canSetObjectToPlaceUIElementsResult = currentObjectCanBePlaced;

				if (menuOpenedFromWorkbench) {
					canSetObjectToPlaceUIElementsResult = false;
				}

				placeObjectButton.SetActive (canSetObjectToPlaceUIElementsResult);

				if (canSetRepairUIElementsResult) {
					updateRepairIngredientsPanelListInfo ();
				}


				if (useUpgradePanelEnabled) {
					bool objectCanBeUpgradedResult = mainCraftingSystem.objectCanBeUpgraded (currentObjectSelectedName);

					if (upgradeObjectsOnlyOnWorkbencheEnabled && objectCanBeUpgradedResult) {
						if (menuOpenedFromWorkbench) {
							if (!currentCraftingWorkbenchSystem.upgradeObjectsOnlyOnWorkbencheEnabled) {
								objectCanBeUpgradedResult = false;
							}
						} else {
							objectCanBeUpgradedResult = false;
						}
					}

					upgradeButton.SetActive (objectCanBeUpgradedResult);
				}
			}

			checkIfShowCurrentObjectMesh (true);
		}
			
		if (showDebugPrint) {
			print ("updating " + currentObjectSelectedName + " info");
		}

		if (upgradePanelActive) {
			updateObjectStatsPanelListInfo ();
		}
	}

	void checkIfShowCurrentObjectMesh (bool state)
	{
		if (menuOpenedFromWorkbench) {
			if (currentCraftingWorkbenchSystem.showCurrentObjectMesh) {
				if (currentObjectMesh != null) {
					Destroy (currentObjectMesh);
				}

				if (state) {
					GameObject objectMesh = mainCraftingSystem.getInventoryMeshByName (currentObjectSelectedName);

					if (objectMesh != null) {
						currentObjectMesh = Instantiate (objectMesh, currentCraftingWorkbenchSystem.currentObjectMeshPlaceTransform);

						currentObjectMesh.transform.localPosition = Vector3.zero;
						currentObjectMesh.transform.localRotation = Quaternion.identity;
					}
				}
			}
		}
	}

	void removeObjectPanelInfoContent ()
	{
		for (int i = 0; i < currentObjectIngredientsInfoList.Count; i++) {
			craftingObjectButtonInfo currentIngredientInfo = currentObjectIngredientsInfoList [i];

			if (currentIngredientInfo.buttonGameObject.activeSelf) {
				currentIngredientInfo.buttonGameObject.SetActive (false);

				currentIngredientInfo.objectAssigned = false;
			}
		}
	
		currentObjectNameText.text = "";

		currentObjectDescriptionText.text = "";

		currentObjectSelectedImage.texture = null;
	}

	void updateRepairIngredientsPanelListInfo ()
	{
		for (int i = 0; i < currentRepairIngredientsInfoList.Count; i++) {
			craftingObjectButtonInfo currentRepairIngredientInfo = currentRepairIngredientsInfoList [i];

			if (currentRepairIngredientInfo.buttonGameObject.activeSelf) {
				currentRepairIngredientInfo.buttonGameObject.SetActive (false);

				currentRepairIngredientInfo.objectAssigned = false;
			}
		}

		int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (currentObjectCategorySelectedName));

		if (currentCategoryIndex > -1) {
			if (showDebugPrint) {
				print ("category of ingredients found");
			}

			int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (currentObjectSelectedName));

			if (currentIndex > -1) {
				List<craftingIngredientObjectInfo> repairIngredientsInfoList = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].repairIngredientObjectInfoList;

				if (showDebugPrint) {
					print ("list of ingredients found " + repairIngredientsInfoList.Count);
				}

				for (int i = 0; i < repairIngredientsInfoList.Count; i++) {
					int currentAmount = mainCraftingSystem.getInventoryObjectAmountByName (repairIngredientsInfoList [i].Name);

					if (currentAmount < 0) {
						currentAmount = 0;
					}

					craftingObjectButtonInfo currentCraftingObjectButtonInfo = currentRepairIngredientsInfoList [i];

					currentCraftingObjectButtonInfo.buttonGameObject.SetActive (true);

					currentCraftingObjectButtonInfo.objectAssigned = true;

					currentCraftingObjectButtonInfo.objectNameText.text = repairIngredientsInfoList [i].Name;

					currentCraftingObjectButtonInfo.objectImage.texture = mainCraftingSystem.getInventoryObjectIconByName (repairIngredientsInfoList [i].Name);

					currentCraftingObjectButtonInfo.amountAvailableToCreateText.text = 
						(repairIngredientsInfoList [i].amountRequired.ToString () + "/" + currentAmount.ToString ());

				}
			}
		}
	}

	bool canCurrentObjectBeDisassembled ()
	{
		int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (currentObjectCategorySelectedName));

		if (currentCategoryIndex > -1) {

			int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (currentObjectSelectedName));

			if (currentIndex > -1) {
				if (craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].disassembleIngredientObjectInfoList.Count > 0) {
					return true;
				}
			}
		}

		return false;
	}

	public void openPanelInfo (GameObject buttonObject)
	{
		int panelCategoryIndex = -1;
		int panelIndex = -1;

		for (int i = 0; i < panelCategoryInfoList.Count; i++) {
			for (int j = 0; j < panelCategoryInfoList [i].panelInfoList.Count; j++) {
				panelInfo currentpanelInfo = panelCategoryInfoList [i].panelInfoList [j];

				if (currentpanelInfo.panelButton == buttonObject) {
					panelIndex = j;

					panelCategoryIndex = i;
	
					currentObjectCategorySelectedName = currentpanelInfo.Name;

					currentObjectCategoryNameText.text = currentObjectCategorySelectedName;

					showAllCategoriesActive = currentpanelInfo.showAllCategories;

					updateCategoryObjectPanelInfoActive ();

					resetAmountToCreate ();

					selectFirstObjectAvailableButton ();
				}
			}
		}

		if (panelIndex == -1 || panelCategoryIndex == -1) {
			return;
		}

		panelInfo currentPanelInfo = panelCategoryInfoList [panelCategoryIndex].panelInfoList [panelIndex];

		if (currentPanelInfo.useEventsOnSelectPanel) {
			currentPanelInfo.eventOnSelectPanel.Invoke ();
		}

		return;
	}

	void updateCategoryObjectPanelInfo ()
	{
		if (showingInventoryList) {
			for (int i = 0; i < currentInventoryInfoList.Count; i++) {
				craftingObjectButtonInfo currentInventoryInfo = currentInventoryInfoList [i];

				if (currentInventoryInfo.buttonGameObject.activeSelf) {
					currentInventoryInfo.buttonGameObject.SetActive (false);

					currentInventoryInfo.objectAssigned = false;
				}
			}

			List<inventoryInfo> inventoryList = mainCraftingSystem.getInventoryList ();

			for (int i = 0; i < inventoryList.Count; i++) {
				craftingObjectButtonInfo currentInventoryInfo = currentInventoryInfoList [i];

				currentInventoryInfo.objectName = inventoryList [i].Name;

				currentInventoryInfo.objectCategoryName = inventoryList [i].categoryName;

				currentInventoryInfo.buttonGameObject.SetActive (true);

				currentInventoryInfo.objectAssigned = true;

				currentInventoryInfo.objectNameText.text = inventoryList [i].Name;

				currentInventoryInfo.objectImage.texture = inventoryList [i].icon;

				currentInventoryInfo.amountAvailableToCreateText.text = inventoryList [i].amount.ToString ();
			}
		} else {
			for (int i = 0; i < currentObjectIngredientsInfoList.Count; i++) {
				craftingObjectButtonInfo currentIngredientInfo = currentObjectIngredientsInfoList [i];

				if (currentIngredientInfo.buttonGameObject.activeSelf) {
					currentIngredientInfo.buttonGameObject.SetActive (false);

					currentIngredientInfo.objectAssigned = false;
				}
			}

			int currentCraftingButtonIndex = 0;

			for (int i = 0; i < craftingBlueprintInfoTemplateDataList.Count; i++) {
				for (int j = 0; j < craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList.Count; j++) {
					if (currentCraftingButtonIndex < craftingObjectButtonInfoList.Count) {
						craftingObjectButtonInfo currentCraftingObjectButtonInfo = craftingObjectButtonInfoList [currentCraftingButtonIndex];

						string objectName = craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList [j].Name;

						currentCraftingObjectButtonInfo.objectName = objectName;

						currentCraftingObjectButtonInfo.objectCategoryName = craftingBlueprintInfoTemplateDataList [i].Name;

						bool canShowObjectResult = true;

						if (useOnlyBlueprintsUnlocked) {
							if (!blueprintsUnlockedList.Contains (objectName)) {
								canShowObjectResult = false;
							}
						}

						if (craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList [j].isRawMaterial) {
							canShowObjectResult = false;
						}

						if (canShowObjectResult) {
							if (isCurrentObjectCategoryAvailable (currentCraftingObjectButtonInfo.objectCategoryName)) {
								currentCraftingObjectButtonInfo.buttonGameObject.SetActive (true);
							}
						}

						currentCraftingObjectButtonInfo.objectAssigned = true;

						currentCraftingObjectButtonInfo.objectNameText.text = objectName;

						currentCraftingObjectButtonInfo.objectImage.texture =
						mainCraftingSystem.getInventoryObjectIconByName (objectName);

						currentCraftingButtonIndex++;
					} else {
						print ("WARNING: There are not enough buttons, increase the initial amount to spawn on start");
					}
				}
			}
		}
	}


	void updateCategoryObjectPanelInfoActive ()
	{
		if (showingInventoryList) {
			setListElementsActiveState (currentInventoryInfoList);
		} else {
			setListElementsActiveState (craftingObjectButtonInfoList);
		}
	}

	void setListElementsActiveState (List<craftingObjectButtonInfo> currentList)
	{
		for (int i = 0; i < currentList.Count; i++) {
			bool setActiveResult = true;

			if (currentList [i].objectCategoryName != null) {
				if (!showAllCategoriesActive) {
					if (!currentList [i].objectCategoryName.Equals (currentObjectCategorySelectedName)) {
						setActiveResult = false;
					}
				} else {
					if (currentList [i].objectCategoryName == "") {
						setActiveResult = false;
					}
				}
			} else {
				setActiveResult = false;
			}

			if (setActiveResult) {
				if (!showingInventoryList) {
					if (useOnlyBlueprintsUnlocked) {
						if (!blueprintsUnlockedList.Contains (currentList [i].objectName)) {
							setActiveResult = false;
						}
					}

					if (!isCurrentObjectCategoryAvailable (currentList [i].objectCategoryName)) {
						setActiveResult = false;
					}
				}
			}

			if (currentList [i].buttonGameObject.activeSelf != setActiveResult) {
				currentList [i].buttonGameObject.SetActive (setActiveResult);
			}
		}
	}

	public override void initializeMenuPanel ()
	{
		if (mainCraftingSystem == null) {
			checkMenuComponents ();
		}
	}

	void checkMenuComponents ()
	{
		if (!componentsAssigned) {
			if (pauseManager != null) {
				playerComponentsManager currentPlayerComponentsManager = pauseManager.getPlayerControllerGameObject ().GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {

					mainCraftingSystem = currentPlayerComponentsManager.getCraftingSystem ();

					mainCraftingSystemAssigned = mainCraftingSystem != null;
				}
			}

			componentsAssigned = true;
		}
	}

	public void setOpenFromWorkbenchState (bool state, List<string> newObjectCategoriesToCraftAvailableOnCurrentBench)
	{
		if (menuOpenedFromWorkbench == state) {
			return;
		}

		menuOpenedFromWorkbench = state;

		if (menuOpenedFromWorkbench) {
			objectCategoriesToCraftAvailableOnCurrentBench = newObjectCategoriesToCraftAvailableOnCurrentBench;
		} else {
			objectCategoriesToCraftAvailableOnCurrentBench = null;
		}
	}

	public void setCurrentcurrentCraftingWorkbenchSystem (craftingWorkbenchSystem newCraftingWorkbenchSystem)
	{
		currentCraftingWorkbenchSystem = newCraftingWorkbenchSystem;
	}

	public void checkEventToStopUsingWorkbenchOnDamageReceived ()
	{
		if (menuOpenedFromWorkbench) {
			if (currentCraftingWorkbenchSystem != null) {
				currentCraftingWorkbenchSystem.checkEventToStopUsingWorkbenchOnDamageReceived ();
			}
		}
	}

	public GameObject getCurrentObjectToPlace ()
	{
		int currentCategoryIndex = craftingBlueprintInfoTemplateDataList.FindIndex (s => s.Name.Equals (currentObjectCategorySelectedName));

		if (currentCategoryIndex > -1) {

			int currentIndex = craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (currentObjectSelectedName));

			if (currentIndex > -1) {
				return craftingBlueprintInfoTemplateDataList [currentCategoryIndex].craftingBlueprintInfoTemplateList [currentIndex].objectToPlace;
			}
		}

		return null;
	}

	public GameObject getCurrentObjectToPlaceByName (string objectName)
	{
		for (int i = 0; i < craftingBlueprintInfoTemplateDataList.Count; i++) {
			for (int j = 0; j < craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList.Count; j++) {
				craftingBlueprintInfoTemplate currentCraftingBlueprintInfoTemplate = craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList [j];

				if (currentCraftingBlueprintInfoTemplate.Name.Equals (objectName)) {
					return currentCraftingBlueprintInfoTemplate.objectToPlace;
				}
			}
		}


		return null;
	}

	public LayerMask getCurrentObjectLayerMaskToAttachObjectByName (string objectName)
	{
		for (int i = 0; i < craftingBlueprintInfoTemplateDataList.Count; i++) {
			for (int j = 0; j < craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList.Count; j++) {
				craftingBlueprintInfoTemplate currentCraftingBlueprintInfoTemplate = craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList [j];

				if (currentCraftingBlueprintInfoTemplate.Name.Equals (objectName)) {
					return currentCraftingBlueprintInfoTemplate.layerMaskToAttachObject;
				}
			}
		}


		return new LayerMask ();
	}

	public Vector3 getCurrentObjectToPlacePositionOffsetByName (string objectName)
	{
		for (int i = 0; i < craftingBlueprintInfoTemplateDataList.Count; i++) {
			for (int j = 0; j < craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList.Count; j++) {
				craftingBlueprintInfoTemplate currentCraftingBlueprintInfoTemplate = craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList [j];

				if (currentCraftingBlueprintInfoTemplate.Name.Equals (objectName)) {
					return currentCraftingBlueprintInfoTemplate.objectToPlacePositionOffset;
				}
			}
		}


		return Vector3.zero;
	}

	public bool checkIfCurrentObjectToPlaceUseCustomLayerMaskByName (string objectName)
	{
		for (int i = 0; i < craftingBlueprintInfoTemplateDataList.Count; i++) {
			for (int j = 0; j < craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList.Count; j++) {
				craftingBlueprintInfoTemplate currentCraftingBlueprintInfoTemplate = craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList [j];

				if (currentCraftingBlueprintInfoTemplate.Name.Equals (objectName)) {
					return currentCraftingBlueprintInfoTemplate.useCustomLayerMaskToPlaceObject;
				}
			}
		}


		return false;
	}

	public LayerMask getCurrentObjectCustomLayerMaskToPlaceObjectByName (string objectName)
	{
		for (int i = 0; i < craftingBlueprintInfoTemplateDataList.Count; i++) {
			for (int j = 0; j < craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList.Count; j++) {
				craftingBlueprintInfoTemplate currentCraftingBlueprintInfoTemplate = craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList [j];

				if (currentCraftingBlueprintInfoTemplate.Name.Equals (objectName)) {
					return currentCraftingBlueprintInfoTemplate.customLayerMaskToPlaceObject;
				}
			}
		}


		return new LayerMask ();
	}

	public void getCurrentObjectCanBeRotatedValuesByName (string objectName, ref bool objectCanBeRotatedOnYAxis, ref bool objectCanBeRotatedOnXAxis)
	{
		objectCanBeRotatedOnYAxis = false;
		objectCanBeRotatedOnXAxis = false;

		for (int i = 0; i < craftingBlueprintInfoTemplateDataList.Count; i++) {
			for (int j = 0; j < craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList.Count; j++) {
				craftingBlueprintInfoTemplate currentCraftingBlueprintInfoTemplate = craftingBlueprintInfoTemplateDataList [i].craftingBlueprintInfoTemplateList [j];

				if (currentCraftingBlueprintInfoTemplate.Name.Equals (objectName)) {
					objectCanBeRotatedOnYAxis = currentCraftingBlueprintInfoTemplate.objectCanBeRotatedOnYAxis;
					objectCanBeRotatedOnXAxis = currentCraftingBlueprintInfoTemplate.objectCanBeRotatedOnXAxis;
				
					return;
				}
			}
		}
	}

	public List<craftingBlueprintInfoTemplateData> getCraftingBlueprintInfoTemplateDataList ()
	{
		return craftingBlueprintInfoTemplateDataList;
	}

	public string getCurrentObjectSelectedName ()
	{
		return currentObjectSelectedName;
	}

	public void setPlacementActiveState (bool state)
	{
		if (menuOpenedFromWorkbench) {
			return;
		}

		mainCraftingSystem.setPlacementActiveState (state);
	}

	public override void openOrCloseMenuPanel (bool state)
	{
		base.openOrCloseMenuPanel (state);

		menuOpened = state;

		checkMenuComponents ();

		objectFilterActive = false;

		searcherInputField.text = "";

		if (!state) {
			if (currentObjectMesh != null) {
				Destroy (currentObjectMesh);
			}
		}

		if (state) {
			checkSpawnUIElements ();

			for (int i = 0; i < panelCategoryInfoList.Count; i++) {
				for (int j = 0; j < panelCategoryInfoList [i].panelInfoList.Count; j++) {
					panelInfo currentpanelInfo = panelCategoryInfoList [i].panelInfoList [j];

					bool panelButtonEnabled = currentpanelInfo.panelButtonEnabled;

					if (showDebugPrint) {
						print (panelButtonEnabled + "  " + currentpanelInfo.Name);
					}

					if (currentpanelInfo.panelButton.activeSelf != panelButtonEnabled) {
						currentpanelInfo.panelButton.SetActive (panelButtonEnabled);
					}
				}
			}

		} else {
			currentButtonObjectPressed = null;	
		}
			
		if (state) {
			updateCategoryObjectPanelInfo ();

			selectFirstCategoryPanel ();
		}

		if (craftingMenuOnlyUsableOnWorkbenchesEnabled) {
			if (state && !menuOpenedFromWorkbench) {
				openOrCloseMenuFromTouch ();
			}
		}

		if (mainCraftingSystemAssigned) {
			mainCraftingSystem.openOrCloseCraftingMenu (menuOpened);
		}

		if (state) {
			if (showingInventoryList) {
				showBlueprintList ();
			}

			showingInventoryList = false;
		}
	}

	void selectFirstCategoryPanel ()
	{
		for (int i = 0; i < panelCategoryInfoList.Count; i++) {
			for (int j = 0; j < panelCategoryInfoList [i].panelInfoList.Count; j++) {
				panelInfo currentpanelInfo = panelCategoryInfoList [i].panelInfoList [j];

				if (currentpanelInfo.panelButtonEnabled) {
					openPanelInfo (currentpanelInfo.panelButton);

					return;
				}
			}
		}
	}

	void checkSpawnUIElements ()
	{
		if (spawnObjectButtonsEnabled) {
			if (UIElementSpawned) {
				return;
			}

			for (int i = 0; i < amountOfObjectButtonsToSpawn; i++) {
				GameObject newButton = (GameObject)Instantiate (craftingButtonInfoPrefab, Vector3.zero, Quaternion.identity, craftingButtonInfoPanelList);

				newButton.transform.localScale = Vector3.one;
				newButton.transform.localPosition = Vector3.zero;

				craftingObjectButtonInfo currentCraftingObjectButtonInfo = newButton.GetComponent<craftingObjectButtonInfo> ();

				craftingObjectButtonInfoList.Add (currentCraftingObjectButtonInfo);
			}

			for (int i = 0; i < amountOfIngredientsButtonsToSpawn; i++) {
				GameObject newButton = (GameObject)Instantiate (hoverBlueprintPanelListElementPrefab, Vector3.zero, Quaternion.identity, hoverBlueprintPanelList);

				newButton.transform.localScale = Vector3.one;
				newButton.transform.localPosition = Vector3.zero;

				craftingObjectButtonInfo currentCraftingObjectButtonInfo = newButton.GetComponent<craftingObjectButtonInfo> ();

				currentObjectIngredientsInfoList.Add (currentCraftingObjectButtonInfo);
			}

			for (int i = 0; i < amountOfInventoryButtonsToSpawn; i++) {
				GameObject newButton = (GameObject)Instantiate (inventoryPanelListElementPrefab, Vector3.zero, Quaternion.identity, inventoryPanelList);

				newButton.transform.localScale = Vector3.one;
				newButton.transform.localPosition = Vector3.zero;

				craftingObjectButtonInfo currentCraftingObjectButtonInfo = newButton.GetComponent<craftingObjectButtonInfo> ();

				currentInventoryInfoList.Add (currentCraftingObjectButtonInfo);
			}

			for (int i = 0; i < amountOnRepareIngredientsButtonsToSpawn; i++) {
				GameObject newButton = (GameObject)Instantiate (repairPanelListElementPrefab, Vector3.zero, Quaternion.identity, repairPanelList);

				newButton.transform.localScale = Vector3.one;
				newButton.transform.localPosition = Vector3.zero;

				craftingObjectButtonInfo currentCraftingObjectButtonInfo = newButton.GetComponent<craftingObjectButtonInfo> ();

				currentRepairIngredientsInfoList.Add (currentCraftingObjectButtonInfo);
			}

			for (int i = 0; i < amountOfObjectStatsButtonToSpawn; i++) {
				GameObject newButton = (GameObject)Instantiate (objectStatPanelListElementPrefab, Vector3.zero, Quaternion.identity, objectStatsPanelList);

				newButton.transform.localScale = Vector3.one;
				newButton.transform.localPosition = Vector3.zero;

				objectStatButtonInfo currentObjectStatButtonInfo = newButton.GetComponent<objectStatButtonInfo> ();

				objectStatButtonInfoList.Add (currentObjectStatButtonInfo);
			}

			for (int i = 0; i < amountOfObjectToCraftInTimeButtonToSpawn; i++) {
				GameObject newButton = (GameObject)Instantiate (craftObjectsInTimePanelListElementPrefab, Vector3.zero, Quaternion.identity, craftObjectsInTimePanelList);

				newButton.transform.localScale = Vector3.one;
				newButton.transform.localPosition = Vector3.zero;

				craftingObjectButtonInfo currentCraftingObjectButtonInfo = newButton.GetComponent<craftingObjectButtonInfo> ();

				currentCraftObjectInTimeInfoList.Add (currentCraftingObjectButtonInfo);
			}

			UIElementSpawned = true;
		}
	}

	public void setUseOnlyBlueprintsUnlockedState (bool state)
	{
		useOnlyBlueprintsUnlocked = state;
	}

	public bool isUseOnlyBlueprintsUnlockedActive ()
	{
		return useOnlyBlueprintsUnlocked;
	}

	public void setBlueprintsUnlockedListValue (List<string> newBlueprintsUnlockedList)
	{
		blueprintsUnlockedList = new List<string> (newBlueprintsUnlockedList);
	}

	public List<string> getBlueprintsUnlockedListValue ()
	{
		return blueprintsUnlockedList;
	}

	public void addNewBlueprintsUnlockedElement (string newBlueprintsUnlockedElement)
	{
		if (!blueprintsUnlockedList.Contains (newBlueprintsUnlockedElement)) {
			blueprintsUnlockedList.Add (newBlueprintsUnlockedElement);

			if (menuOpened) {
				showBlueprintList ();
			}
		}
	}

	public bool isCurrentObjectCategoryAvailable (string categoryName)
	{
		if (mainCraftingSystem.allowAllObjectCategoriesToCraftAtAnyMomentEnabled) {
			return true;
		} else {
			if (menuOpenedFromWorkbench) {
				if (objectCategoriesToCraftAvailableOnCurrentBench.Count > 0) {
					if (objectCategoriesToCraftAvailableOnCurrentBench.Contains (categoryName)) {
						return true;
					}
				}

				if (mainCraftingSystem.objectCategoriesToCraftAvailableAtAnyMoment.Contains (categoryName)) {
					return true;
				}
			} else {
				if (mainCraftingSystem.objectCategoriesToCraftAvailableAtAnyMoment.Contains (categoryName)) {
					return true;
				}
			}
		}

		return false;
	}

	public bool anyObjectToCraftInTimeActive ()
	{
		return craftObjectInTimeInfoList.Count > 0;
	}

	public List<craftObjectInTimeSimpleInfo> getCraftObjectInTimeInfoList ()
	{
		List<craftObjectInTimeSimpleInfo> newCraftObjectInTimeSimpleInfoList = new List<craftObjectInTimeSimpleInfo> ();

		for (int i = 0; i < craftObjectInTimeInfoList.Count; i++) {
			craftObjectInTimeSimpleInfo newCraftObjectInTimeSimpleInfo = new craftObjectInTimeSimpleInfo ();

			newCraftObjectInTimeSimpleInfo.objectName = craftObjectInTimeInfoList [i].objectName;

			newCraftObjectInTimeSimpleInfo.objectCategoryName = craftObjectInTimeInfoList [i].objectCategoryName;

			newCraftObjectInTimeSimpleInfo.amount = craftObjectInTimeInfoList [i].amount;

			newCraftObjectInTimeSimpleInfo.timeToCraftObject = craftObjectInTimeInfoList [i].timeToCraftObject;

			newCraftObjectInTimeSimpleInfo.objectCreatedOnWorkbench = craftObjectInTimeInfoList [i].objectCreatedOnWorkbench;

			newCraftObjectInTimeSimpleInfo.workbenchID = craftObjectInTimeInfoList [i].workbenchID;

			newCraftObjectInTimeSimpleInfoList.Add (newCraftObjectInTimeSimpleInfo);
		}

		return newCraftObjectInTimeSimpleInfoList;
	}

	public void setCraftObjectInTimeInfoList (List<craftObjectInTimeSimpleInfo> newCraftObjectInTimeSimpleInfoList)
	{
		for (int i = 0; i < newCraftObjectInTimeSimpleInfoList.Count; i++) {
			craftObjectInTimeInfo newCraftObjectInTimeInfo = new craftObjectInTimeInfo ();

			newCraftObjectInTimeInfo.objectName = newCraftObjectInTimeSimpleInfoList [i].objectName;

			newCraftObjectInTimeInfo.objectCategoryName = newCraftObjectInTimeSimpleInfoList [i].objectCategoryName;

			newCraftObjectInTimeInfo.amount = newCraftObjectInTimeSimpleInfoList [i].amount;

			newCraftObjectInTimeInfo.timeToCraftObject = newCraftObjectInTimeSimpleInfoList [i].timeToCraftObject;
		
			newCraftObjectInTimeInfo.objectCreatedOnWorkbench = newCraftObjectInTimeSimpleInfoList [i].objectCreatedOnWorkbench;

			newCraftObjectInTimeInfo.workbenchID = newCraftObjectInTimeSimpleInfoList [i].workbenchID;

			craftObjectInTimeInfoList.Add (newCraftObjectInTimeInfo);
		}

		StartCoroutine (setCraftObjectInTimeInfoListCoroutine ());
	}

	IEnumerator setCraftObjectInTimeInfoListCoroutine ()
	{
		yield return new WaitForSeconds (0.5f);

		for (int i = 0; i < craftObjectInTimeInfoList.Count; i++) {
			craftObjectInTimeInfoList [i].craftObjectCoroutine = StartCoroutine (craftObjectInTimeCoroutine (craftObjectInTimeInfoList [i]));
		}
	}

	[System.Serializable]
	public class panelCategoryInfo
	{
		public string Name;

		[Space]

		public List<panelInfo> panelInfoList = new List<panelInfo> ();
	}

	[System.Serializable]
	public class panelInfo
	{
		public string Name;

		public bool panelButtonEnabled = true;

		public bool showAllCategories;

		[Space]
		[Space]

		public GameObject panelButton;

		[Space]
		[Space]

		public bool useEventsOnSelectPanel;
		public UnityEvent eventOnSelectPanel;
	}

	[System.Serializable]
	public class craftObjectInTimeInfo
	{
		public string objectName;

		public string objectCategoryName;

		public int amount;

		public float timeToCraftObject;
		public Coroutine craftObjectCoroutine;

		[Space]

		public bool objectCreatedOnWorkbench;
		public int workbenchID;

		[Space]

		public craftingObjectButtonInfo currentCraftingObjectButtonInfo;
	}
}