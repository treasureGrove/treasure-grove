using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using GKC.Localization;

public class vendorUISystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool showObjectsWithHigherLevelThanPlayer = true;
	public bool addSoldObjectsToShop = true;

	public string currentObjectSelectedPriceExtraText;
	public string priceExtraText = "Available: ";
	public string outOfStockText = "OUT OF STOCK";

	public float zoomSpeed;
	public float maxZoomValue;
	public float minZoomValue;
	public float rotationSpeed;

	public bool useBlurUIPanel = true;

	public float configureNumberObjectsToUseRate = 0.4f;
	public float fasterNumberObjectsToUseRate = 0.1f;
	public float waitTimeToUseFasterNumberObjectsToUseRate = 1;

	public string mainInventoryManagerName = "Main Inventory Manager";

	public bool showBuyObjectAmountSign;
	public bool showSellObjectAmountSign = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool menuOpened;
	public bool addingObjectToUse;
	public bool removinvObjectToUse;
	public int numberOfObjectsToUse;

	public bool buyingObjects = true;
	public bool sellingObjects;

	[Space]
	[Header ("Inventory Debug")]
	[Space]

	public bool showDebugPrint;

	public List<inventoryInfo> vendorInventoryList = new List<inventoryInfo> ();

	public List<vendorObjectSlotPanelInfo> vendorObjectSlotPanelInfoList = new List<vendorObjectSlotPanelInfo> ();

	public List<vendorCategorySlotPanelInfo> vendorCategorySlotPanelInfoList = new List<vendorCategorySlotPanelInfo> ();

	public List<inventoryInfo> soldObjectList = new List<inventoryInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnSetBuyMode;
	public UnityEvent eventOnSetSellMode;

	public UnityEvent eventOnPurchase;
	public UnityEvent eventOnSale;

	public UnityEvent eventOnObjectOutOfStock;
	public UnityEvent eventOnNotEnoughMoney;

	public UnityEvent eventOnNotEnoughLevel;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject vendorMenu;

	public ScrollRect categoryScrollRect;
	public ScrollRect objectScrollRect;

	public GameObject categorySlotsParent;
	public GameObject categorySlotPrefab;
	public GameObject objectSlotsParent;
	public GameObject objectSlotPrefab;

	public Scrollbar vendorObjectListScrollbar;

	public Text numberOfObjectsToUseText;

	public Text currentObjectSelectedPriceText;

	public Text totalPriceText;

	public Text totalMoneyAmountAvailable;

	public Text currentCategoryNameText;
	public Text currentObjectNameText;
	public Text currentObjectInformationText;

	public Camera inventoryCamera;

	public RawImage inventoryMenuRenderTexture;

	public Transform lookObjectsPosition;

	public inventoryManager playerInventoryManager;
	public usingDevicesSystem usingDevicesManager;

	public inventoryListManager mainInventoryManager;
	public menuPause pauseManager;
	public playerInputManager playerInput;
	public currencySystem mainCurrencySystem;
	public playerExperienceSystem mainPlayerExperienceSystem;

	inventoryInfo currentInventoryObject;

	vendorObjectSlotPanelInfo currentVendorObjectSlotPanelInfo;
	vendorCategorySlotPanelInfo currentVendorCategorySlotPanelInfo;

	float originalFov;
	GameObject objectInCamera;

	bool zoomingIn;
	bool zoomingOut;

	bool enableRotation;
	inventoryBankSystem currentInventoryBankSystem;

	bool useInventoryBankAsVendorSystem;

	float lastTimeAddObjectToUse;
	float lastTimeRemoveObjectToUse;
	bool useFasterNumberObjectsToUseRateActive;
	float lastTimeConfigureNumberOfObjects;

	inventoryObject currentInventoryObjectManager;
	float currentMaxZoomValue;
	float currentMinZoomValue;

	Vector2 axisValues;

	GameObject currentInventoryBankGameObject;

	Transform positionToSpawnObjects;

	vendorSystem mainVendorSystem;

	bool mainInventoryManagerLocated;

	Coroutine updateCoroutine;


	void Start ()
	{
		originalFov = inventoryCamera.fieldOfView;

		if (vendorMenu.activeSelf) {
			vendorMenu.SetActive (false);
		}

		if (categorySlotPrefab.activeSelf) {
			categorySlotPrefab.SetActive (false);
		}

		if (objectSlotPrefab.activeSelf) {
			objectSlotPrefab.SetActive (false);
		}
	}

	void checkGetMainInventoryManager ()
	{
		if (!mainInventoryManagerLocated) {
			if (mainInventoryManager == null) {
				GKC_Utils.instantiateMainManagerOnSceneWithType (mainInventoryManagerName, typeof(inventoryListManager));

				mainInventoryManager = FindObjectOfType<inventoryListManager> ();

				mainInventoryManagerLocated = true;
			}
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
		var waitTime = new WaitForSecondsRealtime (0.00001f);

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (menuOpened) {

			if (enableRotation) {
				axisValues = playerInput.getPlayerMouseAxis ();
				objectInCamera.transform.Rotate (inventoryCamera.transform.up, -Mathf.Deg2Rad * rotationSpeed * axisValues.x, Space.World);
				objectInCamera.transform.Rotate (inventoryCamera.transform.right, Mathf.Deg2Rad * rotationSpeed * axisValues.y, Space.World);
			}

			if (currentInventoryObjectManager != null && currentInventoryObjectManager.useZoomRange) {
				currentMaxZoomValue = currentInventoryObjectManager.maxZoom;
				currentMinZoomValue = currentInventoryObjectManager.minZoom;	
			} else {
				currentMaxZoomValue = maxZoomValue;
				currentMinZoomValue = minZoomValue;
			}

			if (zoomingIn) {
				if (inventoryCamera.fieldOfView > currentMaxZoomValue) {
					inventoryCamera.fieldOfView -= Time.deltaTime * zoomSpeed;
				} else {
					inventoryCamera.fieldOfView = currentMaxZoomValue;
				}
			}

			if (zoomingOut) {
				if (inventoryCamera.fieldOfView < currentMinZoomValue) {
					inventoryCamera.fieldOfView += Time.deltaTime * zoomSpeed;
				} else {
					inventoryCamera.fieldOfView = currentMinZoomValue;
				}
			}

			if (addingObjectToUse) {
				if (!useFasterNumberObjectsToUseRateActive) {
					if (Time.time > configureNumberObjectsToUseRate + lastTimeAddObjectToUse) {

						lastTimeAddObjectToUse = Time.time;

						addObjectToUse ();
					}

					if (Time.time > lastTimeConfigureNumberOfObjects + waitTimeToUseFasterNumberObjectsToUseRate) {
						useFasterNumberObjectsToUseRateActive = true;
					}
				} else {
					if (Time.time > fasterNumberObjectsToUseRate + lastTimeAddObjectToUse) {
						lastTimeAddObjectToUse = Time.time;

						addObjectToUse ();
					}
				}
			}

			if (removinvObjectToUse) {
				if (!useFasterNumberObjectsToUseRateActive) {
					if (Time.time > configureNumberObjectsToUseRate + lastTimeRemoveObjectToUse) {

						lastTimeRemoveObjectToUse = Time.time;

						removeObjectToUse ();
					}

					if (Time.time > lastTimeConfigureNumberOfObjects + waitTimeToUseFasterNumberObjectsToUseRate) {
						useFasterNumberObjectsToUseRateActive = true;
					}
				} else {
					if (Time.time > fasterNumberObjectsToUseRate + lastTimeRemoveObjectToUse) {
						lastTimeRemoveObjectToUse = Time.time;

						removeObjectToUse ();
					}
				}
			}
		}
	}

	public void setCurrentInventoryBankSystem (GameObject inventoryBankGameObject)
	{
		currentInventoryBankGameObject = inventoryBankGameObject;

		currentInventoryBankSystem = inventoryBankGameObject.GetComponent<inventoryBankSystem> ();

		positionToSpawnObjects = currentInventoryBankSystem.getPositionToSpawnObjects ();

		useInventoryBankAsVendorSystem = currentInventoryBankSystem.useAsVendorSystem && currentInventoryBankSystem.useInventoryFromThisBank;

		if (useInventoryBankAsVendorSystem) {
			vendorInventoryList = new List<inventoryInfo> (currentInventoryBankSystem.getBankInventoryList ());
		} else {
			if (mainVendorSystem == null) {
				mainVendorSystem = FindObjectOfType<vendorSystem> ();
			}

			if (mainVendorSystem != null) {
				vendorInventoryList = new List<inventoryInfo> (mainVendorSystem.getVendorInventoryList ());
			} else {
				print ("WARNING: No Vendor System found in the scene, make sure to add the Main Inventory Manager prefab to the scene");
			}
		}
	}

	public void updateInventoryListOnVendorSystem ()
	{
		if (currentInventoryBankSystem != null) {
			if (!currentInventoryBankSystem.useInventoryFromThisBank) {
				if (mainVendorSystem == null) {
					mainVendorSystem = FindObjectOfType<vendorSystem> ();
				}

				if (mainVendorSystem != null) {
					mainVendorSystem.setVendorInventoryList (vendorInventoryList);
				}
			}
		}
	}

	public void openOrCloseVendorMenuByButton ()
	{
		if (usingDevicesManager != null) {
			usingDevicesManager.useDevice ();
		}

		if (currentInventoryBankSystem != null) {
			currentInventoryBankSystem.setUsingInventoryBankState (menuOpened);
		}
	}

	public void openOrCloseVendorMenu (bool state)
	{
		menuOpened = state;

		stopUpdateCoroutine ();

		pauseManager.openOrClosePlayerMenu (menuOpened, vendorMenu.transform, useBlurUIPanel);

		vendorMenu.SetActive (menuOpened);

		pauseManager.setIngameMenuOpenedState ("Vendor System", menuOpened, true);

		resetVendorUIState ();

		pauseManager.enableOrDisablePlayerMenu (menuOpened, true, false);

		inventoryCamera.enabled = menuOpened;

		buyingObjects = true;

		sellingObjects = false;

		callEventOnSetBuyMode ();

		if (!menuOpened) {
			playerInventoryManager.updateAllWeaponSlotAmmo ();
		}

		if (menuOpened) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}
	}

	public void updateAllInventoryVendorUI ()
	{
		for (int i = 0; i < vendorCategorySlotPanelInfoList.Count; i++) {
			string categoryNameText = vendorCategorySlotPanelInfoList [i].Name;
				
			if (gameLanguageSelector.isCheckLanguageActive ()) {
				categoryNameText = inventoryLocalizationManager.GetLocalizedValue (categoryNameText);
			} 

			vendorCategorySlotPanelInfoList [i].categoryNameText.text = categoryNameText;
		}

		for (int i = 0; i < vendorObjectSlotPanelInfoList.Count; i++) {
			string objectNameText = vendorObjectSlotPanelInfoList [i].Name;

			if (gameLanguageSelector.isCheckLanguageActive ()) {
				objectNameText = inventoryLocalizationManager.GetLocalizedValue (objectNameText);
			} 

			vendorObjectSlotPanelInfoList [i].objectNameText.text = objectNameText;
		}

		if (currentInventoryObject != null) {
			string objectNameText = currentInventoryObject.Name;

			if (gameLanguageSelector.isCheckLanguageActive ()) {
				objectNameText = inventoryLocalizationManager.GetLocalizedValue (objectNameText);
			} 

			currentObjectNameText.text = objectNameText;


			string objectInfoText = currentInventoryObject.objectInfo;

			if (gameLanguageSelector.isCheckLanguageActive ()) {
				objectInfoText = inventoryLocalizationManager.GetLocalizedValue (objectInfoText);
			} 

			currentObjectInformationText.text = objectInfoText;
		}
	}

	public void resetVendorUIState ()
	{
		setVendorCategorySlotPanelInfoPressedState (false);

		setVendorObjectSlotPanelInfoPressedState (false);

		currentVendorObjectSlotPanelInfo = null;
		currentVendorCategorySlotPanelInfo = null;

		currentInventoryObjectManager = null;

		resetAndDisableNumberOfObjectsToUseMenu ();

		inventoryCamera.fieldOfView = originalFov;
		currentInventoryObject = null;

		destroyObjectInCamera ();

		if (menuOpened) {

			if (buyingObjects) {
				if (addSoldObjectsToShop) {
					for (int i = 0; i < soldObjectList.Count; i++) {
						vendorInventoryList.Add (new inventoryInfo (soldObjectList [i]));
					}

					soldObjectList.Clear ();
				}

				joinSameInventoryObjectsAmount ();

				updateInventoryListOnVendorSystem ();
			}

			if (vendorCategorySlotPanelInfoList.Count == 0) {
				createCategorySlots ();
			}

			if (vendorObjectSlotPanelInfoList.Count == 0) {
				createInventoryIcons ();
			}

			resetScrollRectAndScrollBarPositions ();

			getCategoryPressedButton (vendorCategorySlotPanelInfoList [0].button);

			totalMoneyAmountAvailable.text = mainCurrencySystem.getCurrentMoneyAmount () + currentObjectSelectedPriceExtraText;
		} else {
			if (addSoldObjectsToShop && soldObjectList.Count > 0) {
				if (mainVendorSystem != null) {
					vendorInventoryList = new List<inventoryInfo> (mainVendorSystem.getVendorInventoryList ());

					for (int i = 0; i < soldObjectList.Count; i++) {
						vendorInventoryList.Add (new inventoryInfo (soldObjectList [i]));
					}

					soldObjectList.Clear ();

					joinSameInventoryObjectsAmount ();

					updateInventoryListOnVendorSystem ();
				}
			}

			destroyAndClearAllList ();
		}
	}

	public void createCategorySlots ()
	{
		checkGetMainInventoryManager ();

		for (int i = 0; i < mainInventoryManager.inventoryCategoryInfoList.Count; i++) {

			int objectsAvailableForCurrentCategory = 0;

			for (int j = 0; j < vendorInventoryList.Count; j++) {
				if (vendorInventoryList [j].categoryName.Equals (mainInventoryManager.inventoryCategoryInfoList [i].Name)) {
					objectsAvailableForCurrentCategory++;
				}
			}

			if (objectsAvailableForCurrentCategory > 0) {

				GameObject newCategorySlot = (GameObject)Instantiate (categorySlotPrefab, Vector3.zero, Quaternion.identity, categorySlotsParent.transform);

				if (!newCategorySlot.activeSelf) {
					newCategorySlot.SetActive (true);
				}

				newCategorySlot.transform.localScale = Vector3.one;
				newCategorySlot.transform.localPosition = Vector3.zero;

				vendorCategorySlotPanelInfo currentVendorCategorySlotPanelInfo = newCategorySlot.GetComponent<vendorCategorySlotPanel> ().mainVendorCategorySlotPanelInfo;

				currentVendorCategorySlotPanelInfo.Name = mainInventoryManager.inventoryCategoryInfoList [i].Name;

				string categoryNameText = currentVendorCategorySlotPanelInfo.Name;

				if (gameLanguageSelector.isCheckLanguageActive ()) {
					categoryNameText = inventoryLocalizationManager.GetLocalizedValue (categoryNameText);
				} 

				currentVendorCategorySlotPanelInfo.categoryNameText.text = categoryNameText;

				currentVendorCategorySlotPanelInfo.categoryIcon.texture = mainInventoryManager.inventoryCategoryInfoList [i].cateogryTexture;

				newCategorySlot.name = "Vendor Category-" + (i + 1);

				vendorCategorySlotPanelInfoList.Add (currentVendorCategorySlotPanelInfo);
			}
		}
	}

	public void createInventoryIcons ()
	{
		for (int i = 0; i < vendorInventoryList.Count; i++) {
//			print (vendorInventoryList [i].Name + " " + vendorInventoryList [i].amount);

			createVendorObjectIcon (vendorInventoryList [i], i);
		}
	}

	public void resetScrollRectAndScrollBarPositions ()
	{
		categoryScrollRect.normalizedPosition = new Vector2 (0, 1);

		LayoutRebuilder.ForceRebuildLayoutImmediate (categorySlotsParent.GetComponent<RectTransform> ());

		resetObjectScrollRectAndScrollBarPositions ();
	}

	public void resetObjectScrollRectAndScrollBarPositions ()
	{
		StartCoroutine (AutoScroll ());
	}

	private IEnumerator AutoScroll ()
	{
//		LayoutRebuilder.ForceRebuildLayoutImmediate (objectSlotsParent.GetComponent<RectTransform> ());
//		yield return new WaitForEndOfFrame ();
//		yield return new WaitForEndOfFrame ();
//		objectScrollRect.verticalNormalizedPosition = 1;
//
//		vendorObjectListScrollbar.value = 1;
//		yield return new WaitForEndOfFrame ();
//		yield return new WaitForEndOfFrame ();
//		LayoutRebuilder.ForceRebuildLayoutImmediate (vendorObjectListScrollbar.GetComponent<RectTransform> ());

		vendorObjectListScrollbar.value = 0;

		yield return null;
		vendorObjectListScrollbar.value = 1;
	}

	public void createVendorObjectIcon (inventoryInfo currentInventoryInfo, int index)
	{
		GameObject newIconButton = (GameObject)Instantiate (objectSlotPrefab, Vector3.zero, Quaternion.identity, objectSlotsParent.transform);

		if (!newIconButton.activeSelf) {
			newIconButton.SetActive (true);
		}

		newIconButton.transform.localScale = Vector3.one;
		newIconButton.transform.localPosition = Vector3.zero;

		vendorObjectSlotPanelInfo currentVendorObjectSlotPanelInfo = newIconButton.GetComponent<vendorObjectSlotPanel> ().mainVendorObjectSlotPanelInfo;

		currentVendorObjectSlotPanelInfo.Name = currentInventoryInfo.Name;
		currentVendorObjectSlotPanelInfo.categoryName = currentInventoryInfo.categoryName;

		string objectNameText = currentInventoryInfo.Name;

		if (gameLanguageSelector.isCheckLanguageActive ()) {
			objectNameText = inventoryLocalizationManager.GetLocalizedValue (objectNameText);
		} 

		currentVendorObjectSlotPanelInfo.objectNameText.text = objectNameText;

		if (currentInventoryInfo.useMinLevelToBuy) {
			if (!currentVendorObjectSlotPanelInfo.objectLevelText.gameObject.activeSelf) {
				currentVendorObjectSlotPanelInfo.objectLevelText.gameObject.SetActive (true);
			}

			currentVendorObjectSlotPanelInfo.objectLevelText.text = "Level: " + currentInventoryInfo.minLevelToBuy;
		}

		if (currentInventoryInfo.infiniteVendorAmountAvailable) {
			currentVendorObjectSlotPanelInfo.objectAmountAvailableText.text = priceExtraText + "Inf";
		} else {
			if (currentInventoryInfo.amount > 0) {
				currentVendorObjectSlotPanelInfo.objectAmountAvailableText.text = priceExtraText + currentInventoryInfo.amount;
			} else {
				currentVendorObjectSlotPanelInfo.objectAmountAvailableText.text = outOfStockText;
			}
		}

		float totalPriceAmount = 0;

		if (buyingObjects) {
			totalPriceAmount = currentInventoryInfo.vendorPrice;
		} else {
			totalPriceAmount = currentInventoryInfo.sellPrice;
		}

		if (currentInventoryInfo.objectIsBroken && currentInventoryInfo.sellMultiplierIfObjectIsBroken > 0) {
			totalPriceAmount *= currentInventoryInfo.sellMultiplierIfObjectIsBroken;
		}

		currentVendorObjectSlotPanelInfo.objectPriceText.text = totalPriceAmount + currentObjectSelectedPriceExtraText;

		newIconButton.name = "Vendor Object-" + (index + 1);

		Button button = currentVendorObjectSlotPanelInfo.button;
		currentInventoryInfo.button = button;
		currentInventoryInfo.currentVendorObjectSlotPanelInfo = currentVendorObjectSlotPanelInfo;

		vendorObjectSlotPanelInfoList.Add (currentVendorObjectSlotPanelInfo);
	}

	public void destroyVendorObjectSlotPanelInfoList ()
	{
		for (int i = 0; i < vendorInventoryList.Count; i++) {
			if (vendorInventoryList [i].button != null) {
				Destroy (vendorInventoryList [i].button.gameObject);
			}
		}
	}

	public void destroyVendorCategorySlotPanelInfoList ()
	{
		for (int i = 0; i < vendorCategorySlotPanelInfoList.Count; i++) {
			if (vendorCategorySlotPanelInfoList [i].button != null) {
				Destroy (vendorCategorySlotPanelInfoList [i].button.gameObject);
			}
		}
	}

	public void getCategoryPressedButton (Button buttonObj)
	{
		setVendorCategorySlotPanelInfoPressedState (false);

		int objectIndexToSelect = -1;

		string categoryName = "";

		for (int i = 0; i < vendorCategorySlotPanelInfoList.Count; i++) {
			if (vendorCategorySlotPanelInfoList [i].button == buttonObj) {
				categoryName = vendorCategorySlotPanelInfoList [i].Name;

				currentVendorCategorySlotPanelInfo = vendorCategorySlotPanelInfoList [i];
			}
		}

		int currentPlayerLevel = mainPlayerExperienceSystem.getCurrentLevel ();

		bool firstObjectSlotSelected = false;

		for (int i = 0; i < vendorObjectSlotPanelInfoList.Count; i++) {
			if (vendorObjectSlotPanelInfoList [i].categoryName.Equals (categoryName)) {

				if (vendorInventoryList [i].useMinLevelToBuy && !showObjectsWithHigherLevelThanPlayer) {
					if (showDebugPrint) {
						print (vendorObjectSlotPanelInfoList [i].Name);
					}

					if (vendorInventoryList [i].minLevelToBuy <= currentPlayerLevel) {
						if (!vendorObjectSlotPanelInfoList [i].button.gameObject.activeSelf) {
							vendorObjectSlotPanelInfoList [i].button.gameObject.SetActive (true);
						}

						if (!firstObjectSlotSelected) {
							objectIndexToSelect = i;

							firstObjectSlotSelected = true;
						}
					} else {
						if (showDebugPrint) {
							print (" deactivate" + vendorObjectSlotPanelInfoList [i].Name);
						}

						if (vendorObjectSlotPanelInfoList [i].button.gameObject.activeSelf) {
							vendorObjectSlotPanelInfoList [i].button.gameObject.SetActive (false);
						}
					}
				} else {
					if (!vendorObjectSlotPanelInfoList [i].button.gameObject.activeSelf) {
						vendorObjectSlotPanelInfoList [i].button.gameObject.SetActive (true);
					}

					if (!firstObjectSlotSelected) {
						objectIndexToSelect = i;

						firstObjectSlotSelected = true;
					}
				}

				if (sellingObjects && vendorInventoryList [i].amount == 0) {
					if (vendorObjectSlotPanelInfoList [i].button.gameObject.activeSelf) {
						vendorObjectSlotPanelInfoList [i].button.gameObject.SetActive (false);
					}
				}
			} else {
				if (vendorObjectSlotPanelInfoList [i].button.gameObject.activeSelf) {
					vendorObjectSlotPanelInfoList [i].button.gameObject.SetActive (false);
				}
			}
		}
			
		if (objectIndexToSelect > -1) {
			getObjectPressedButton (vendorObjectSlotPanelInfoList [objectIndexToSelect].button);
		} else {
			getObjectPressedButton (null);
		}

		setVendorCategorySlotPanelInfoPressedState (true);

		string categoryNameText = categoryName;

		if (gameLanguageSelector.isCheckLanguageActive ()) {
			categoryNameText = inventoryLocalizationManager.GetLocalizedValue (categoryNameText);
		} 

		currentCategoryNameText.text = categoryNameText;

		resetObjectScrollRectAndScrollBarPositions ();
	}

	public void getObjectPressedButton (Button buttonObj)
	{
		int inventoryObjectIndex = -1;

		if (buttonObj != null) {
			for (int i = 0; i < vendorObjectSlotPanelInfoList.Count; i++) {
				if (vendorObjectSlotPanelInfoList [i].button == buttonObj) {
					inventoryObjectIndex = i;
				}
			}
		}

		setObjectInfo (inventoryObjectIndex);
	}

	public void setObjectInfo (int index)
	{
		if (index > -1) {
			setVendorObjectSlotPanelInfoPressedState (false);

			currentInventoryObject = vendorInventoryList [index];

			if (showDebugPrint) {
				print ("object selected " + currentInventoryObject.Name);
			}

			currentVendorObjectSlotPanelInfo = vendorObjectSlotPanelInfoList [index];

			setVendorObjectSlotPanelInfoPressedState (true);

			GameObject currentInventoryObjectPrefab = currentInventoryObject.inventoryGameObject;

			checkGetMainInventoryManager ();

			for (int i = 0; i < mainInventoryManager.inventoryList.Count; i++) {
				if (mainInventoryManager.inventoryList [i].inventoryGameObject == currentInventoryObject.inventoryGameObject) {
					currentInventoryObjectPrefab = mainInventoryManager.inventoryList [i].inventoryObjectPrefab;
				}
			}

			currentInventoryObjectManager = currentInventoryObjectPrefab.GetComponentInChildren<inventoryObject> ();

			inventoryMenuRenderTexture.enabled = true;

			destroyObjectInCamera ();

			if (currentInventoryObject.inventoryGameObject != null) {

				objectInCamera = (GameObject)Instantiate (currentInventoryObject.inventoryGameObject, lookObjectsPosition.position, Quaternion.identity, lookObjectsPosition);

				float currentCameraFov = originalFov;

				if (currentInventoryObjectManager != null) {
					if (currentInventoryObjectManager.useZoomRange) {
						currentCameraFov = currentInventoryObjectManager.initialZoom;
					}
				}

				if (inventoryCamera.fieldOfView != currentCameraFov) {
					checkResetCameraFov (currentCameraFov);
				}
			}

			string objectNameText = currentInventoryObject.Name;

			if (gameLanguageSelector.isCheckLanguageActive ()) {
				objectNameText = inventoryLocalizationManager.GetLocalizedValue (objectNameText);
			} 

			currentObjectNameText.text = objectNameText;


			string objectInfoText = currentInventoryObject.objectInfo;

			if (gameLanguageSelector.isCheckLanguageActive ()) {
				objectInfoText = inventoryLocalizationManager.GetLocalizedValue (objectInfoText);
			} 

			currentObjectInformationText.text = objectInfoText;


			resetAndDisableNumberOfObjectsToUseMenu ();
		} else {
			setVendorObjectSlotPanelInfoPressedState (false);

			currentInventoryObject = null;

			if (showDebugPrint) {
				print ("no object selected");
			}

			currentVendorObjectSlotPanelInfo = null;

			currentInventoryObjectManager = null;

			inventoryMenuRenderTexture.enabled = false;

			destroyObjectInCamera ();

			currentObjectNameText.text = "";

			currentObjectInformationText.text = "";

			currentObjectSelectedPriceText.text = "0";

			totalPriceText.text = "0";

			numberOfObjectsToUseText.text = "0";

			resetAndDisableNumberOfObjectsToUseMenu ();
		}
	}

	public void setVendorObjectSlotPanelInfoPressedState (bool state)
	{
		if (currentVendorObjectSlotPanelInfo != null && currentVendorObjectSlotPanelInfo.pressedIcon != null) {
			if (currentVendorObjectSlotPanelInfo.pressedIcon.activeSelf != state) {
				currentVendorObjectSlotPanelInfo.pressedIcon.SetActive (state);
			}
		}
	}

	public void setVendorCategorySlotPanelInfoPressedState (bool state)
	{
		if (currentVendorCategorySlotPanelInfo != null && currentVendorCategorySlotPanelInfo.pressedIcon != null) {
			if (currentVendorCategorySlotPanelInfo.pressedIcon.activeSelf != state) {
				currentVendorCategorySlotPanelInfo.pressedIcon.SetActive (state);
			}
		}
	}

	public void enableObjectRotation ()
	{
		if (objectInCamera) {
			enableRotation = true;
		}
	}

	public void disableObjectRotation ()
	{
		enableRotation = false;
	}

	public void destroyObjectInCamera ()
	{
		if (objectInCamera != null) {
			Destroy (objectInCamera);
		}
	}

	public void zoomInEnabled ()
	{
		zoomingIn = true;
	}

	public void zoomInDisabled ()
	{
		zoomingIn = false;
	}

	public void zoomOutEnabled ()
	{
		zoomingOut = true;
	}

	public void zoomOutDisabled ()
	{
		zoomingOut = false;
	}

	public void checkResetCameraFov (float targetValue)
	{
		inventoryCamera.fieldOfView = targetValue;
	}

	public void addObjectToUse ()
	{
		if (currentInventoryObject == null) {
			return;
		}

		if (currentInventoryObject.infiniteVendorAmountAvailable) {
			numberOfObjectsToUse++;

			if (numberOfObjectsToUse > 99) {
				numberOfObjectsToUse = 99;
			}
		} else {
			if (currentInventoryObject.amount > 0) {
				numberOfObjectsToUse++;

				if (numberOfObjectsToUse > currentInventoryObject.amount) {
					numberOfObjectsToUse = currentInventoryObject.amount;
				}
			} else {
				numberOfObjectsToUse = 0;
			}
		}

		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void removeObjectToUse ()
	{
		if (currentInventoryObject == null) {
			return;
		}

		if (currentInventoryObject.infiniteVendorAmountAvailable) {
			numberOfObjectsToUse--;

			if (numberOfObjectsToUse < 1) {
				numberOfObjectsToUse = 1;
			}
		} else {
			if (currentInventoryObject.amount > 0) {
				numberOfObjectsToUse--;

				if (numberOfObjectsToUse < 1) {
					numberOfObjectsToUse = 1;
				}
			} else {
				numberOfObjectsToUse = 0;
			}
		}

		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void startToAddObjectToUse ()
	{
		addingObjectToUse = true;

		lastTimeConfigureNumberOfObjects = Time.time;
	}

	public void stopToAddObjectToUse ()
	{
		addingObjectToUse = false;

		lastTimeAddObjectToUse = 0;

		useFasterNumberObjectsToUseRateActive = false;
	}

	public void startToRemoveObjectToUse ()
	{
		removinvObjectToUse = true;

		lastTimeConfigureNumberOfObjects = Time.time;
	}

	public void stopToRemoveObjectToUse ()
	{
		removinvObjectToUse = false;

		lastTimeRemoveObjectToUse = 0;

		useFasterNumberObjectsToUseRateActive = false;
	}

	public void resetAndDisableNumberOfObjectsToUseMenu ()
	{
		resetNumberOfObjectsToUse ();
	}

	public void resetNumberOfObjectsToUse ()
	{
		if (currentInventoryObject != null) {
			if (currentInventoryObject.infiniteVendorAmountAvailable) {
				numberOfObjectsToUse = 1;
			} else {
				if (currentInventoryObject.amount > 0) {
					numberOfObjectsToUse = 1;
				} else {
					numberOfObjectsToUse = 0;
				}
			}
		} else {
			numberOfObjectsToUse = 1;
		}

		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void setNumberOfObjectsToUseText (int amount)
	{
		numberOfObjectsToUseText.text = amount.ToString ();

		if (currentInventoryObject != null) {
			float totalPriceAmount = 0;

			if (buyingObjects) {
				totalPriceAmount = currentInventoryObject.vendorPrice;
			} else {
				totalPriceAmount = currentInventoryObject.sellPrice;
			}

			if (currentInventoryObject.objectIsBroken && currentInventoryObject.sellMultiplierIfObjectIsBroken > 0) {
				totalPriceAmount *= currentInventoryObject.sellMultiplierIfObjectIsBroken;
			}

			currentObjectSelectedPriceText.text = totalPriceAmount + currentObjectSelectedPriceExtraText;

			totalPriceText.text = "";

			if (buyingObjects) {
				if (showBuyObjectAmountSign) {
					totalPriceText.text += "-";
				}
			} else {
				if (showSellObjectAmountSign) {
					totalPriceText.text += "+";
				}
			}
				
			totalPriceText.text += (amount * totalPriceAmount) + currentObjectSelectedPriceExtraText;
		}
	}

	public void setBuyMode ()
	{
		buyingObjects = true;

		sellingObjects = false;

		destroyAndClearAllList ();

		setCurrentInventoryBankSystem (currentInventoryBankGameObject);

		resetVendorUIState ();

		callEventOnSetBuyMode ();

		resetScrollRectAndScrollBarPositions ();
	}

	public void setSellMode ()
	{
		if (playerInventoryManager.isInventoryEmpty ()) {
			return;
		}

		buyingObjects = false;

		sellingObjects = true;

		destroyAndClearAllList ();

		for (int i = 0; i < playerInventoryManager.inventoryList.Count; i++) {
			vendorInventoryList.Add (new inventoryInfo (playerInventoryManager.inventoryList [i]));
		}
			
		for (int i = 0; i < vendorInventoryList.Count; i++) {
			if (vendorInventoryList [i].amount == 0 || !vendorInventoryList [i].canBeSold) {
				vendorInventoryList.RemoveAt (i);
				i = 0;
			}
		}

		joinSameInventoryObjectsAmount ();

		checkGetMainInventoryManager ();

		for (int i = 0; i < vendorInventoryList.Count; i++) {
			if (vendorInventoryList [i].storeTotalAmountPerUnit) {

				int currentAmountPerUnit = mainInventoryManager.getInventoryAmountPerUnitFromInventoryGameObject (vendorInventoryList [i].inventoryGameObject);

				vendorInventoryList [i].amount = (int)(vendorInventoryList [i].amount / currentAmountPerUnit);

				vendorInventoryList [i].amountPerUnit = currentAmountPerUnit;
			} 
		}

		resetVendorUIState ();

		callEventOnSetSellMode ();

		resetScrollRectAndScrollBarPositions ();
	}

	public void joinSameInventoryObjectsAmount ()
	{
		for (int i = 0; i < vendorInventoryList.Count; i++) {
			for (int j = 0; j < vendorInventoryList.Count; j++) {
				if (vendorInventoryList [i].Name.Equals (vendorInventoryList [j].Name) && vendorInventoryList [i].amount != vendorInventoryList [j].amount) {

					if (showDebugPrint) {
						print (vendorInventoryList [i].Name + " " + vendorInventoryList [j].Name);
					}

					vendorInventoryList [i].amount += vendorInventoryList [j].amount;

					vendorInventoryList.RemoveAt (j);

					i = 0;
					j = 0;
				}
			}
		}
	}

	public void destroyAndClearAllList ()
	{
		destroyVendorObjectSlotPanelInfoList ();

		destroyVendorCategorySlotPanelInfoList ();

		vendorInventoryList.Clear ();

		vendorCategorySlotPanelInfoList.Clear ();

		vendorObjectSlotPanelInfoList.Clear ();
	}

	public void callEventOnSetBuyMode ()
	{
		eventOnSetBuyMode.Invoke ();
	}

	public void callEventOnSetSellMode ()
	{
		eventOnSetSellMode.Invoke ();
	}

	public void confirmBuyObject ()
	{
		if (currentInventoryObject == null) {
			return;
		}

		if (currentInventoryObject.amount <= 0 && !currentInventoryObject.infiniteVendorAmountAvailable) {
			eventOnObjectOutOfStock.Invoke ();

			return;
		}

		if (currentInventoryObject.useMinLevelToBuy) {
			if (currentInventoryObject.minLevelToBuy > mainPlayerExperienceSystem.getCurrentLevel ()) {
				eventOnNotEnoughLevel.Invoke ();

				return;
			}
		}

		float totalMoneyAmountToSpend = numberOfObjectsToUse * currentInventoryObject.vendorPrice;

		if (currentInventoryObject.objectIsBroken && currentInventoryObject.sellMultiplierIfObjectIsBroken > 0) {
			totalMoneyAmountToSpend *= currentInventoryObject.sellMultiplierIfObjectIsBroken;
		}

		bool enoughMoneyAvailable = mainCurrencySystem.canSpendMoneyAmount (totalMoneyAmountToSpend);

		if (enoughMoneyAvailable) {

			if (currentInventoryObject.infiniteVendorAmountAvailable) {
				currentInventoryObject.amount = numberOfObjectsToUse + 1;
			}

			int currentSlotToMoveAmount = currentInventoryObject.amount;

			int currentAmountPerUnit = currentInventoryObject.amountPerUnit;

			if (currentInventoryObject.storeTotalAmountPerUnit) {
				currentInventoryObject.amount = numberOfObjectsToUse * currentAmountPerUnit;
			} else {
				currentInventoryObject.amount = numberOfObjectsToUse;
			}

			if (showDebugPrint) {
				print ("amount to take " + currentInventoryObject.amount);
			}

			bool purchasedProperly = false;

			int inventoryAmountPicked = 0;

			if (currentInventoryObject.spawnObject || currentInventoryObject.cantBeStoredOnInventory) {
				checkGetMainInventoryManager ();

				GameObject inventoryObjectPrefab = null;

				if (currentInventoryObject.useCustomObjectToDrop) {
					inventoryObjectPrefab = currentInventoryObject.customObjectToDrop;
				} else {
					inventoryObjectPrefab = mainInventoryManager.getInventoryPrefab (currentInventoryObject.inventoryGameObject);
				}

				inventoryListManager.spawnInventoryObject (inventoryObjectPrefab, positionToSpawnObjects, currentInventoryObject.amount, 
					currentInventoryObject);

				inventoryAmountPicked = numberOfObjectsToUse;

				purchasedProperly = true;

			} else {
				if (!playerInventoryManager.isInventoryFull ()) {
					inventoryAmountPicked = playerInventoryManager.tryToPickUpObject (currentInventoryObject);

					if (showDebugPrint) {
						print ("amount taken " + inventoryAmountPicked);
					}

					if (inventoryAmountPicked > 0) {

						if (currentInventoryObject.storeTotalAmountPerUnit && currentAmountPerUnit > 0) {
							inventoryAmountPicked = (int)(inventoryAmountPicked / currentAmountPerUnit);
						}

						purchasedProperly = true;
					} else {
						playerInventoryManager.showInventoryFullMessage ();
					}
				} else {
					playerInventoryManager.showInventoryFullMessage ();
				}
			}

			if (purchasedProperly) {
				currentInventoryObject.amount = currentSlotToMoveAmount - inventoryAmountPicked;

				mainCurrencySystem.useMoney (totalMoneyAmountToSpend);

				updateCurrentInventoryObjectSlot ();

				eventOnPurchase.Invoke ();

				totalMoneyAmountAvailable.text = mainCurrencySystem.getCurrentMoneyAmount () + currentObjectSelectedPriceExtraText;

				resetNumberOfObjectsToUse ();
			}

		} else {
			eventOnNotEnoughMoney.Invoke ();
		}
	}

	public void confirmSellObject ()
	{
		if (currentInventoryObject == null) {
			return;
		}

		float totalMoneyAmountToSpend = numberOfObjectsToUse * currentInventoryObject.sellPrice;

		if (currentInventoryObject.objectIsBroken && currentInventoryObject.sellMultiplierIfObjectIsBroken > 0) {
			totalMoneyAmountToSpend *= currentInventoryObject.sellMultiplierIfObjectIsBroken;
		}

		int playerInventoryObjectIndex = playerInventoryManager.getInventoryObjectIndexByName (currentInventoryObject.Name);

		if (playerInventoryObjectIndex == -1) {
			if (showDebugPrint) {
				print ("WARNING: Not inventory object found on player's inventory with name " + currentInventoryObject.Name + ", make sure that object exist and is configured properly");
			}

			return;
		}

		int amountToRemoveFromInventory = numberOfObjectsToUse;

		if (currentInventoryObject.storeTotalAmountPerUnit) {
			checkGetMainInventoryManager ();

			int currentAmountPerUnit = mainInventoryManager.getInventoryAmountPerUnitFromInventoryGameObject (currentInventoryObject.inventoryGameObject);

			amountToRemoveFromInventory = amountToRemoveFromInventory * currentAmountPerUnit;
		} 

		playerInventoryManager.sellObjectOnVendorSystem (playerInventoryObjectIndex, amountToRemoveFromInventory);
	
		currentInventoryObject.amount -= numberOfObjectsToUse;

		if (addSoldObjectsToShop) {

			bool objectFoundOnSoldList = false;

			for (int i = 0; i < soldObjectList.Count; i++) {
				if (!objectFoundOnSoldList && soldObjectList [i].inventoryGameObject == currentInventoryObject.inventoryGameObject) {
					soldObjectList [i].amount += numberOfObjectsToUse;

					objectFoundOnSoldList = true;
				}
			}

			if (!objectFoundOnSoldList) {
				inventoryInfo newInventoryInfoToSoldList = new inventoryInfo (currentInventoryObject);

				newInventoryInfoToSoldList.amount = numberOfObjectsToUse;

				soldObjectList.Add (newInventoryInfoToSoldList);
			}
		}

		if (currentInventoryObject.amount <= 0) {
			removeButton ();

			resetScrollRectAndScrollBarPositions ();
		} else {
			updateCurrentInventoryObjectSlot ();
		}

		mainCurrencySystem.increaseTotalMoneyAmount (totalMoneyAmountToSpend);

		eventOnSale.Invoke ();

		LayoutRebuilder.ForceRebuildLayoutImmediate (categorySlotsParent.GetComponent<RectTransform> ());
		LayoutRebuilder.ForceRebuildLayoutImmediate (objectSlotsParent.GetComponent<RectTransform> ());

		totalMoneyAmountAvailable.text = mainCurrencySystem.getCurrentMoneyAmount () + currentObjectSelectedPriceExtraText;

		resetNumberOfObjectsToUse ();
	}

	public void removeButton ()
	{
		if (currentVendorObjectSlotPanelInfo != null && currentVendorObjectSlotPanelInfo.button != null) {
			Destroy (currentVendorObjectSlotPanelInfo.button.gameObject);
		}

		vendorObjectSlotPanelInfoList.Remove (currentVendorObjectSlotPanelInfo);

		vendorInventoryList.Remove (currentInventoryObject);

		string currentCategoryObjectName = currentVendorCategorySlotPanelInfo.Name;

		int numberOfObjectsOnCurrentCategory = 0;

		int firstCategoryObjectIndex = -1;

		for (int i = 0; i < vendorObjectSlotPanelInfoList.Count; i++) {
			if (vendorObjectSlotPanelInfoList [i].categoryName.Equals (currentCategoryObjectName)) {
				numberOfObjectsOnCurrentCategory++;

				if (firstCategoryObjectIndex == -1) {
					firstCategoryObjectIndex = i;
				}
			}
		}

		if (showDebugPrint) {
			print (numberOfObjectsOnCurrentCategory);
		}

		if (numberOfObjectsOnCurrentCategory > 0) {
			if (showDebugPrint) {
				print ("more objects on this category, moving to " + vendorObjectSlotPanelInfoList [firstCategoryObjectIndex].Name);
			}

			getObjectPressedButton (vendorObjectSlotPanelInfoList [firstCategoryObjectIndex].button);
		} else {
			if (showDebugPrint) {
				print ("no more objects on this category, setting the first one");
			}

			Destroy (currentVendorCategorySlotPanelInfo.button.gameObject);

			vendorCategorySlotPanelInfoList.Remove (currentVendorCategorySlotPanelInfo);

			if (vendorCategorySlotPanelInfoList.Count > 0) {
				getCategoryPressedButton (vendorCategorySlotPanelInfoList [0].button);
			} else {
				setBuyMode ();
			}
		}
	}

	public void updateCurrentInventoryObjectSlot ()
	{
		if (currentInventoryObject.infiniteVendorAmountAvailable) {
			currentVendorObjectSlotPanelInfo.objectAmountAvailableText.text = priceExtraText + "Inf";
		} else {
			if (currentInventoryObject.amount > 0) {
				currentVendorObjectSlotPanelInfo.objectAmountAvailableText.text = priceExtraText + currentInventoryObject.amount;
			} else {
				currentVendorObjectSlotPanelInfo.objectAmountAvailableText.text = outOfStockText;
			}
		}
	}
}