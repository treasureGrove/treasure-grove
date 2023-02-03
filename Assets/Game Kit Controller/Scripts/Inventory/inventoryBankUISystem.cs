using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using GKC.Localization;

public class inventoryBankUISystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useNumberOfElementsOnPressUp;
	public bool useNumberOfElementsWhenIconNotMoved;
	public bool dropFullObjectsAmount = true;
	public bool dragAndDropIcons = true;

	public float timeToDrag = 0.5f;

	public float configureNumberObjectsToUseRate = 0.4f;
	public float fasterNumberObjectsToUseRate = 0.1f;
	public float waitTimeToUseFasterNumberObjectsToUseRate = 1;

	public string mainInventoryManagerName = "Main Inventory Manager";

	[Space]
	[Header ("Zoom/Rotation Settings")]
	[Space]

	public float zoomSpeed;
	public float maxZoomValue;
	public float minZoomValue;
	public float rotationSpeed;

	public bool useBlurUIPanel = true;

	[Space]
	[Header ("Debug State")]
	[Space]

	public bool menuOpened;

	public bool slotToMoveFound;
	public GameObject slotFound;
	public bool draggedFromPlayerInventoryList;
	public bool draggedFromBankInventoryList;
	public bool droppedInPlayerInventoryList;
	public bool droppedInBankInventoryList;

	public bool canDropObjectsToInventoryBank = true;
	public bool touching;

	public bool resetDragDropElementsEnabled = true;

	public inventoryInfo currentSlotToMoveInventoryObject;
	public inventoryInfo currentSlotFoundInventoryObject;

	[Space]
	[Header ("Debug List")]
	[Space]

	public List<inventoryInfo> playerInventoryList = new List<inventoryInfo> ();

	public List<inventoryInfo> currentBankInventoryList = new List<inventoryInfo> ();

	public List<inventoryMenuIconElement> playerIconElementList = new List<inventoryMenuIconElement> ();
	public List<inventoryMenuIconElement> bankIconElementList = new List<inventoryMenuIconElement> ();

	public bool showDebugPrint;

	[Space]
	[Header ("UI Elements")]
	[Space]

	public RawImage inventoryMenuRenderTexture;

	public Transform lookObjectsPosition;

	public GameObject slotToMove;
	public GameObject menuBackground;
	public GameObject inventoryObjectRenderPanel;

	public Camera inventoryCamera;

	public GameObject inventoryMenu;

	public GameObject playerInventorySlots;
	public GameObject playerInventorySlotsContent;
	public GameObject playerInventorySlot;
	public Scrollbar playerInventoryScrollbar;

	public GameObject bankInventorySlots;
	public GameObject bankInventorySlotsContent;
	public GameObject bankInventorySlot;
	public Scrollbar bankInventoryScrollbar;

	public GameObject numberOfElementsWindow;
	public Text numberOfObjectsToUseText;

	[Space]
	[Header ("Components")]
	[Space]

	public inventoryManager playerInventoryManager;

	public usingDevicesSystem usingDevicesManager;

	public inventoryListManager mainInventoryManager;

	public playerInputManager playerInput;

	public menuPause pauseManager;

	inventoryInfo currentInventoryObject;
	Coroutine resetCameraFov;

	float originalFov;
	GameObject objectInCamera;
	int numberOfObjectsToUse;

	bool touchPlatform;

	Touch currentTouch;
	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();

	bool zoomingIn;
	bool zoomingOut;

	bool enableRotation;

	inventoryBankSystem currentInventoryBankSystem;

	inventoryBankManager mainInventoryBankManager;

	bool useInventoryFromThisBank;

	int objectToMoveIndex;
	int objectFoundIndex;

	int currentAmountToMove;
	int currentBankInventoryListIndex;
	bool currentObjectAlreadyInBankList;
	bool settingAmountObjectsToMove;

	float lastTimeTouched;

	bool addingObjectToUse;
	bool removinvObjectToUse;
	float lastTimeAddObjectToUse;
	float lastTimeRemoveObjectToUse;
	bool useFasterNumberObjectsToUseRateActive;
	float lastTimeConfigureNumberOfObjects;

	inventoryObject currentInventoryObjectManager;
	float currentMaxZoomValue;
	float currentMinZoomValue;

	Vector2 axisValues;

	bool mainInventoryManagerLocated;

	Coroutine updateCoroutine;


	void Start ()
	{
		originalFov = inventoryCamera.fieldOfView;

		inventoryMenu.SetActive (false);

		playerInventorySlot.SetActive (false);

		bankInventorySlot.SetActive (false);

		touchPlatform = touchJoystick.checkTouchPlatform ();
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

			if (settingAmountObjectsToMove) {
				return;
			}

			checkInventoryObjectSlotsInput ();
		}
	}

	public void setCurrentInventoryBankSystem (GameObject inventoryBankGameObject)
	{
		currentInventoryBankSystem = inventoryBankGameObject.GetComponent<inventoryBankSystem> ();

		useInventoryFromThisBank = currentInventoryBankSystem.useInventoryFromThisBank;

		if (useInventoryFromThisBank) {
			currentBankInventoryList = currentInventoryBankSystem.getBankInventoryList ();

			canDropObjectsToInventoryBank = false;
		} else {
			if (mainInventoryBankManager == null) {
				GKC_Utils.instantiateMainManagerOnSceneWithType (mainInventoryManagerName, typeof(inventoryListManager));

				mainInventoryBankManager = FindObjectOfType<inventoryBankManager> ();
			}

			if (mainInventoryBankManager != null) {
				currentBankInventoryList = mainInventoryBankManager.getBankInventoryList ();
			} else {
				print ("WARNING: No Inventory Bank Manager found in the scene, make sure to add the Main Inventory Manager prefab to the scene");
			}

			canDropObjectsToInventoryBank = true;
		}

		menuBackground.SetActive (!useInventoryFromThisBank);

		inventoryObjectRenderPanel.SetActive (!useInventoryFromThisBank);
	}

	public void openOrCloseInventoryBankMenuByButton ()
	{
		if (usingDevicesManager != null) {
			usingDevicesManager.useDevice ();
		}

		//openOrCloseInventoryBankMenu (!menuOpened);
		if (currentInventoryBankSystem != null) {
			currentInventoryBankSystem.setUsingInventoryBankState (menuOpened);
		}
	}

	public void openOrCloseInventoryBankMenu (bool state)
	{
		menuOpened = state;

		stopUpdateCoroutine ();

		setMenuIconElementPressedState (false);

		pauseManager.openOrClosePlayerMenu (menuOpened, inventoryMenu.transform, useBlurUIPanel);

		inventoryMenu.SetActive (menuOpened);

		pauseManager.setIngameMenuOpenedState ("Inventory Bank Manager", menuOpened, true);

		bankIconElementList.Clear ();

		playerIconElementList.Clear ();

		if (menuOpened) {
			getCurrentPlayerInventoryList ();

			createInventoryIcons (playerInventoryList, playerInventorySlot, playerInventorySlotsContent, playerIconElementList);

			createInventoryIcons (currentBankInventoryList, bankInventorySlot, bankInventorySlotsContent, bankIconElementList);
		
			playerInventorySlotsContent.GetComponentInParent<ScrollRect> ().verticalNormalizedPosition = 0.5f;
			bankInventorySlotsContent.GetComponentInParent<ScrollRect> ().verticalNormalizedPosition = 0.5f;

			resetScroll (bankInventoryScrollbar);
			resetScroll (playerInventoryScrollbar);
		} else {
			for (int i = 0; i < playerInventoryList.Count; i++) {
				if (playerInventoryList [i].button != null) {
					Destroy (playerInventoryList [i].button.gameObject);
				}
			}

			for (int i = 0; i < currentBankInventoryList.Count; i++) {
				if (currentBankInventoryList [i].button != null) {
					Destroy (currentBankInventoryList [i].button.gameObject);
				}
			}
		}

		pauseManager.enableOrDisablePlayerMenu (menuOpened, true, false);

		destroyObjectInCamera ();

		resetAndDisableNumberOfObjectsToUseMenu ();

		inventoryCamera.fieldOfView = originalFov;
		currentInventoryObject = null;
		inventoryCamera.enabled = menuOpened;

		currentInventoryObjectManager = null;

		if (!menuOpened) {
			resetDragDropElements ();
		}

		if (menuOpened) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}
	}

	public void createInventoryIcons (List<inventoryInfo> currentInventoryListToCreate, GameObject currentInventorySlot, GameObject currentInventorySlotsContent, 
	                                  List<inventoryMenuIconElement> currentIconElementList)
	{
		for (int i = 0; i < currentInventoryListToCreate.Count; i++) {
			if (currentInventoryListToCreate [i].button != null) {
				Destroy (currentInventoryListToCreate [i].button.gameObject);
			}
		}

		for (int i = 0; i < currentInventoryListToCreate.Count; i++) {
			createInventoryIcon (currentInventoryListToCreate [i], i, currentInventorySlot, currentInventorySlotsContent, currentIconElementList);
		}
	}

	public void createInventoryIcon (inventoryInfo currentInventoryInfo, int index, GameObject currentInventorySlot, GameObject currentInventorySlotsContent, 
	                                 List<inventoryMenuIconElement> currentIconElementList)
	{
		GameObject newIconButton = (GameObject)Instantiate (currentInventorySlot, Vector3.zero, Quaternion.identity, currentInventorySlotsContent.transform);

		if (!newIconButton.activeSelf) {
			newIconButton.SetActive (true);
		}

		newIconButton.transform.localScale = Vector3.one;
		newIconButton.transform.localPosition = Vector3.zero;

		inventoryMenuIconElement menuIconElement = newIconButton.GetComponent<inventoryMenuIconElement> ();

		string currentInventoryInfoName = currentInventoryInfo.Name;

		if (gameLanguageSelector.isCheckLanguageActive ()) {
			currentInventoryInfoName = inventoryLocalizationManager.GetLocalizedValue (currentInventoryInfoName);
		} 

		menuIconElement.iconName.text = currentInventoryInfoName;

		if (currentInventoryInfo.inventoryGameObject != null) {
			menuIconElement.icon.texture = currentInventoryInfo.icon;
		} else {
			menuIconElement.icon.texture = null;
		}

		menuIconElement.amount.text = currentInventoryInfo.amount.ToString ();

		menuIconElement.pressedIcon.SetActive (false);

		newIconButton.name = "Inventory Object-" + (index + 1);

		Button button = menuIconElement.button;
		currentInventoryInfo.button = button;
		currentInventoryInfo.menuIconElement = menuIconElement;

		bool slotIsActive = currentInventoryInfo.amount > 0;

		if (currentInventoryInfo.menuIconElement.activeSlotContent) {
			currentInventoryInfo.menuIconElement.activeSlotContent.SetActive (slotIsActive);
		}

		if (currentInventoryInfo.menuIconElement.emptySlotContent) {
			currentInventoryInfo.menuIconElement.emptySlotContent.SetActive (!slotIsActive);
		}

		currentIconElementList.Add (menuIconElement);
	}

	public void enableNumberOfElementsOnPressUp ()
	{
		if (dragAndDropIcons && (!dropFullObjectsAmount || useNumberOfElementsWhenIconNotMoved)) {
			enableNumberOfObjectsToUseMenu (currentSlotToMoveInventoryObject.button.GetComponent<RectTransform> ());

			if (draggedFromPlayerInventoryList) {
				droppedInBankInventoryList = true;
				droppedInPlayerInventoryList = false;
			}

			if (draggedFromBankInventoryList) {
				droppedInPlayerInventoryList = true;
				droppedInBankInventoryList = false;
			}

			draggedFromPlayerInventoryList = true;
			draggedFromBankInventoryList = true;
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
		if (objectInCamera) {
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
		if (resetCameraFov != null) {
			StopCoroutine (resetCameraFov);
		}

		resetCameraFov = StartCoroutine (resetCameraFovCorutine (targetValue));
	}

	IEnumerator resetCameraFovCorutine (float targetValue)
	{
		while (inventoryCamera.fieldOfView != targetValue) {
			inventoryCamera.fieldOfView = Mathf.MoveTowards (inventoryCamera.fieldOfView, targetValue, Time.deltaTime * zoomSpeed);

			yield return null;
		}
	}

	public void getCurrentPlayerInventoryList ()
	{
		playerInventoryList.Clear ();

		for (int i = 0; i < playerInventoryManager.inventoryList.Count; i++) {
			inventoryInfo newPlayerInventoryObjectInfo = new inventoryInfo (playerInventoryManager.inventoryList [i]);
			newPlayerInventoryObjectInfo.button = null;
			playerInventoryList.Add (newPlayerInventoryObjectInfo);
		}
	}

	public void addObjectToUse ()
	{
		numberOfObjectsToUse++;

		if (numberOfObjectsToUse > currentInventoryObject.amount) {
			numberOfObjectsToUse = currentInventoryObject.amount;
		}

		currentAmountToMove = numberOfObjectsToUse;
		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void removeObjectToUse ()
	{
		numberOfObjectsToUse--;

		if (numberOfObjectsToUse < 1) {
			numberOfObjectsToUse = 1;
		}

		currentAmountToMove = numberOfObjectsToUse;

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

	public void enableNumberOfObjectsToUseMenu (RectTransform menuPosition)
	{
		numberOfElementsWindow.SetActive (true);

		numberOfObjectsToUse = 1;
	
		currentAmountToMove = numberOfObjectsToUse;

		setNumberOfObjectsToUseText (numberOfObjectsToUse);

		settingAmountObjectsToMove = true;
	}

	public void resetAndDisableNumberOfObjectsToUseMenu ()
	{
		disableNumberOfObjectsToUseMenu ();

		resetNumberOfObjectsToUse ();
	}

	public void disableNumberOfObjectsToUseMenu ()
	{
		numberOfObjectsToUse = 1;

		numberOfElementsWindow.SetActive (false);
	}

	public void resetNumberOfObjectsToUse ()
	{
		numberOfObjectsToUse = 1;

		setNumberOfObjectsToUseText (numberOfObjectsToUse);
	}

	public void setNumberOfObjectsToUseText (int amount)
	{
		numberOfObjectsToUseText.text = amount.ToString ();
	}

	public void resetScroll (Scrollbar scrollBarToReset)
	{
		StartCoroutine (resetScrollCoroutine (scrollBarToReset));
	}

	IEnumerator resetScrollCoroutine (Scrollbar scrollBarToReset)
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		scrollBarToReset.value = 1;
	}

	public void resetBankInventoryRectTransform ()
	{
		StartCoroutine (resetBankInventoryRectTransformCoroutine ());
	}

	IEnumerator resetBankInventoryRectTransformCoroutine ()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate (bankInventorySlotsContent.GetComponent<RectTransform> ());

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		LayoutRebuilder.ForceRebuildLayoutImmediate (bankInventoryScrollbar.GetComponent<RectTransform> ());
	}

	public void updateFullInventorySlots (List<inventoryInfo> currentInventoryList, List<inventoryMenuIconElement> currentIconElementList, 
	                                      bool checkInventorySlotAmount)
	{
		for (int i = 0; i < currentInventoryList.Count; i++) {
			inventoryInfo currentInventoryInfo = currentInventoryList [i];
			currentInventoryInfo.menuIconElement = currentIconElementList [i];
			currentInventoryInfo.button = currentIconElementList [i].button;

			string currentInventoryInfoName = currentInventoryInfo.Name;

			if (gameLanguageSelector.isCheckLanguageActive ()) {
				currentInventoryInfoName = inventoryLocalizationManager.GetLocalizedValue (currentInventoryInfoName);
			} 

			currentInventoryInfo.menuIconElement.iconName.text = currentInventoryInfoName;

			currentInventoryInfo.menuIconElement.icon.texture = currentInventoryInfo.icon;

			currentInventoryInfo.menuIconElement.amount.text = currentInventoryInfo.amount.ToString ();

			if (checkInventorySlotAmount) {
				bool slotIsActive = currentInventoryInfo.amount > 0;

				currentInventoryInfo.menuIconElement.activeSlotContent.SetActive (slotIsActive);
				currentInventoryInfo.menuIconElement.emptySlotContent.SetActive (!slotIsActive);
			}
		}
	}

	public void setMenuIconElementPressedState (bool state)
	{
		if (currentInventoryObject != null) {
			if (currentInventoryObject.menuIconElement != null) {
				currentInventoryObject.menuIconElement.pressedIcon.SetActive (state);
			}
		}
	}

	public void removeButton (List<inventoryInfo> currentInventoryList, List<inventoryMenuIconElement> currentIconElementList)
	{
		for (int i = 0; i < currentInventoryList.Count; i++) {
			if (currentInventoryList [i].amount == 0) {
				Destroy (currentInventoryList [i].button.gameObject);

				currentIconElementList.Remove (currentInventoryList [i].menuIconElement);

//				print ("index to remove " + i);

				currentInventoryList.RemoveAt (i);

				i = 0;
			}
		}

		enableRotation = false;
		destroyObjectInCamera ();
		setMenuIconElementPressedState (false);
		currentInventoryObject = null;

		currentInventoryObjectManager = null;
	}

	public void resetDragDropElements ()
	{
		if (!resetDragDropElementsEnabled) {
			return;
		}

		slotFound = null;

		draggedFromPlayerInventoryList = false;
		draggedFromBankInventoryList = false;
		droppedInBankInventoryList = false;
		droppedInPlayerInventoryList = false;
		slotToMoveFound = false;

		currentSlotFoundInventoryObject = null;
		currentSlotToMoveInventoryObject = null;

		settingAmountObjectsToMove = false;

		resetAndDisableNumberOfObjectsToUseMenu ();
	}

	public void checkIfObjecToMoveAlreadyInBankList ()
	{
		currentObjectAlreadyInBankList = false;

		if (currentSlotToMoveInventoryObject != null) {
			for (int j = 0; j < currentBankInventoryList.Count; j++) {
				if (!currentObjectAlreadyInBankList && currentBankInventoryList [j].Name.Equals (currentSlotToMoveInventoryObject.Name)) {
					currentObjectAlreadyInBankList = true;
					currentBankInventoryListIndex = j;

					if (showDebugPrint) {
						print ("Current Object Already In Bank List, adding amount to that object");
					}
				}
			}
		}
	}

	public void confirmMoveObjects ()
	{
		if (draggedFromPlayerInventoryList) {

			if ((dragAndDropIcons && droppedInBankInventoryList) || !dragAndDropIcons) {

				checkIfObjecToMoveAlreadyInBankList ();

				moveObjectsToBankInventory ();

				return;
			}
		}

		if (draggedFromBankInventoryList) {

			if ((dragAndDropIcons && droppedInPlayerInventoryList) || !dragAndDropIcons) {
				moveObjectsToPlayerInventory ();

				return;
			}
		}
	}

	public void confirmMoveAllObjects ()
	{
		currentAmountToMove = currentInventoryObject.amount;

		confirmMoveObjects ();
	}

	public void cancelMoveObjects ()
	{
		resetDragDropElements ();

		resetAndDisableNumberOfObjectsToUseMenu ();
	}

	//Inventory bank UI management
	public void getPressedButton (Button buttonObj)
	{
		int inventoryObjectIndex = -1;

		if (draggedFromPlayerInventoryList) {
			for (int i = 0; i < playerInventoryList.Count; i++) {
				if (playerInventoryList [i].button == buttonObj) {
					inventoryObjectIndex = i;
				}
			}
		} else {
			for (int i = 0; i < currentBankInventoryList.Count; i++) {
				if (currentBankInventoryList [i].button == buttonObj) {
					inventoryObjectIndex = i;
				}
			}
		}

		if (currentInventoryObject != null) {
			if (((draggedFromPlayerInventoryList && currentInventoryObject == playerInventoryList [inventoryObjectIndex]) ||
			    (!draggedFromPlayerInventoryList && currentInventoryObject == currentBankInventoryList [inventoryObjectIndex])) &&
			    currentInventoryObject.menuIconElement.pressedIcon.activeSelf) {
				return;
			}
		}

		setObjectInfo (inventoryObjectIndex, draggedFromPlayerInventoryList);

		float currentCameraFov = originalFov;

		if (currentInventoryObjectManager) {
			if (currentInventoryObjectManager.useZoomRange) {
				currentCameraFov = currentInventoryObjectManager.initialZoom;
			}
		}

		if (inventoryCamera.fieldOfView != currentCameraFov) {
			checkResetCameraFov (currentCameraFov);
		}
	}

	public void setObjectInfo (int index, bool isPlayerInventoryList)
	{
		resetAndDisableNumberOfObjectsToUseMenu ();

		setMenuIconElementPressedState (false);

		if (isPlayerInventoryList) {
			currentInventoryObject = playerInventoryList [index];
		} else {
			currentInventoryObject = currentBankInventoryList [index];
		}
		setMenuIconElementPressedState (true);

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
		}
	}

	public void checkInventoryObjectSlotsInput ()
	{
		//check the mouse position in the screen if we are in the editor, or the finger position in a touch device
		int touchCount = Input.touchCount;

		if (!touchPlatform) {
			touchCount++;
		}

		for (int i = 0; i < touchCount; i++) {
			if (!touchPlatform) {
				currentTouch = touchJoystick.convertMouseIntoFinger ();
			} else {
				currentTouch = Input.GetTouch (i);
			}

			if (currentTouch.phase == TouchPhase.Began) {
				touching = true;
				lastTimeTouched = Time.time;

				captureRaycastResults.Clear ();

				PointerEventData p = new PointerEventData (EventSystem.current);
				p.position = currentTouch.position;
				p.clickCount = i;
				p.dragging = false;

				EventSystem.current.RaycastAll (p, captureRaycastResults);

				foreach (RaycastResult r in captureRaycastResults) {

					if (!slotToMoveFound) {
						inventoryMenuIconElement currentInventoryMenuIconElement = r.gameObject.GetComponent<inventoryMenuIconElement> ();

						if (currentInventoryMenuIconElement) {

							for (int j = 0; j < playerInventoryList.Count; j++) {
								if (playerInventoryList [j].button == currentInventoryMenuIconElement.button) {

									if (!canDropObjectsToInventoryBank) {
										return;
									}

									if (playerInventoryList [j].amount > 0) {
										currentSlotToMoveInventoryObject = playerInventoryList [j];
										draggedFromPlayerInventoryList = true;
										slotToMoveFound = true;
										objectToMoveIndex = j;

										if (showDebugPrint) {
											print ("dragged from player inventory list " + j);
										}

										getPressedButton (currentInventoryMenuIconElement.button);
									}
								}
							}

							if (!draggedFromPlayerInventoryList) {
								for (int j = 0; j < currentBankInventoryList.Count; j++) {
									if (currentBankInventoryList [j].button == currentInventoryMenuIconElement.button) {
										if (currentBankInventoryList [j].amount > 0) {
											currentSlotToMoveInventoryObject = currentBankInventoryList [j];
											draggedFromBankInventoryList = true;
											slotToMoveFound = true;
											objectToMoveIndex = j;

											if (showDebugPrint) {
												print ("dragged from bank inventory list " + j);
											}

											getPressedButton (currentInventoryMenuIconElement.button);
										}
									}
								}
							}

							if (slotToMoveFound && dragAndDropIcons) {
								slotToMove.GetComponentInChildren<RawImage> ().texture = currentInventoryMenuIconElement.icon.texture;
							}
						}
					}
				}
			}

			if (touching && !dragAndDropIcons && slotToMoveFound) {
				touching = false;

				checkDroppedObject ();

				return;
			}

			if ((currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved)) {
				if (slotToMoveFound && dragAndDropIcons) {
					if (touching && Time.time > lastTimeTouched + timeToDrag) {
						if (!slotToMove.activeSelf) {
							slotToMove.SetActive (true);
						}

						slotToMove.GetComponent<RectTransform> ().position = new Vector2 (currentTouch.position.x, currentTouch.position.y);
					}
				}
			}

			//if the mouse/finger press is released, then
			if (currentTouch.phase == TouchPhase.Ended && touching) {

				touching = false;

				if (slotToMoveFound) {
					//get the elements in the position where the player released the power element
					captureRaycastResults.Clear ();

					PointerEventData p = new PointerEventData (EventSystem.current);
					p.position = currentTouch.position;
					p.clickCount = i;
					p.dragging = false;

					EventSystem.current.RaycastAll (p, captureRaycastResults);

					foreach (RaycastResult r in captureRaycastResults) {
						if (r.gameObject != slotToMove) {
							if (r.gameObject.GetComponent<inventoryMenuIconElement> ()) {
								slotFound = r.gameObject;
							} else if (r.gameObject == playerInventorySlots) {
								droppedInPlayerInventoryList = true;
							} else if (r.gameObject == bankInventorySlots) {
								droppedInBankInventoryList = true;
							}
						}
					}

					checkDroppedObject ();
				}
			}
		}
	}

	public void checkDroppedObject ()
	{
		//an icon has been dropped on top of another icon from both inventory list
		if (slotFound) {
			bool droppedCorreclty = true;
			bool currentSlotDroppedFoundOnList = false;

			inventoryMenuIconElement slotFoundInventoryMenuIconElement = slotFound.GetComponent<inventoryMenuIconElement> ();

			for (int j = 0; j < playerInventoryList.Count; j++) {
				if (playerInventoryList [j].button == slotFoundInventoryMenuIconElement.button) {
					currentSlotFoundInventoryObject = playerInventoryList [j];
					currentSlotDroppedFoundOnList = true;
					objectFoundIndex = j;
				}
			}

			if (!currentSlotDroppedFoundOnList) {
				for (int j = 0; j < currentBankInventoryList.Count; j++) {
					if (currentBankInventoryList [j].button == slotFoundInventoryMenuIconElement.button) {
						currentSlotFoundInventoryObject = currentBankInventoryList [j];
						currentSlotDroppedFoundOnList = true;
						objectFoundIndex = j;
					}
				}
			}

			if (currentSlotDroppedFoundOnList) {
				if (((draggedFromBankInventoryList && droppedInPlayerInventoryList) || (draggedFromPlayerInventoryList && droppedInBankInventoryList))) {
					if (showDebugPrint) {
						print (currentSlotToMoveInventoryObject.Name + " dropped on top of " + currentSlotFoundInventoryObject.Name);
					}

					if (droppedInPlayerInventoryList) {
						if (showDebugPrint) {
							print ("dropped correctly to player inventory");
						}

						resetDragDropElementsEnabled = false;

						inventoryInfo auxInventoryInfo = currentSlotToMoveInventoryObject;
						int auxObjectToMoveIndex = objectToMoveIndex;

						//move the object dropped to players's inventory
						currentSlotToMoveInventoryObject = currentSlotFoundInventoryObject;
						objectToMoveIndex = objectFoundIndex;

						if (canDropObjectsToInventoryBank) {
							dragToBankInventory ();
						}

						objectToMoveIndex = auxObjectToMoveIndex;

						currentSlotToMoveInventoryObject = auxInventoryInfo;

						if (showDebugPrint) {
							print (currentSlotToMoveInventoryObject.Name + " found on player's inventory");
						}

						resetDragDropElementsEnabled = true;

						dragToPlayerInventory ();

					} else if (droppedInBankInventoryList) {
						if (showDebugPrint) {
							print ("dropped correctly to bank inventory");
						}

						resetDragDropElementsEnabled = false;
						//move the object dropped to bank inventory
						dragToBankInventory ();

						objectToMoveIndex = objectFoundIndex;

						currentSlotToMoveInventoryObject = currentBankInventoryList [objectToMoveIndex];

						if (showDebugPrint) {
							print (currentSlotToMoveInventoryObject.Name + " found on bank");
						}

						resetDragDropElementsEnabled = true;

						dragToPlayerInventory ();
					}
				} else {
					if (showDebugPrint) {
						print ("dropped into the same dragged list");
					}

					droppedCorreclty = false;

					if (useNumberOfElementsWhenIconNotMoved) {
						enableNumberOfElementsOnPressUp ();
						droppedCorreclty = true;
					}
				}
			} else {
				if (showDebugPrint) {
					print ("dropped outside the list");
				}

				droppedCorreclty = false;
			}

			slotToMove.SetActive (false);

			if (!droppedCorreclty) {
				resetDragDropElements ();
			}
		} 

		//the icon is dropped on top or bottom of any of the inventory list
		else {
			bool droppedCorreclty = true;

			//if the object to move has been dragged and dropped from one list to another correctly, then
			if (((draggedFromBankInventoryList && droppedInPlayerInventoryList) || (draggedFromPlayerInventoryList && droppedInBankInventoryList))) {
				//dropping object from bank to player's inventory
				if (droppedInPlayerInventoryList) {
					dragToPlayerInventory ();
				} else if (droppedInBankInventoryList) {
					//dropping object from player's inventory to bank
					dragToBankInventory ();
				} else {
					if (showDebugPrint) {
						print ("not dropped correctly");
					}

					droppedCorreclty = false;
				}
			} else {
				if (dragAndDropIcons) {
					if (showDebugPrint) {
						print ("dropped into the same dragged list or outside of the list");
					}

					droppedCorreclty = false;

					if (useNumberOfElementsOnPressUp) {
						enableNumberOfElementsOnPressUp ();

						droppedCorreclty = true;
					}

				} else {
					//if the drag and drop function is not being used, check if the pressed icon is in player or bank inventory to move the object
					if (draggedFromBankInventoryList) {
						dragToPlayerInventory ();
					} else if (draggedFromPlayerInventoryList) {
						dragToBankInventory ();
					}
				}
			}

			slotToMove.SetActive (false);

			if (!droppedCorreclty) {
				resetDragDropElements ();
			}
		}
	}

	public void dragToPlayerInventory ()
	{
		if (showDebugPrint) {
			print ("dropped correctly to player inventory");

			print (currentSlotToMoveInventoryObject.Name + " dropped correctly to player inventory");
		}

		currentAmountToMove = currentSlotToMoveInventoryObject.amount;

		if (dropFullObjectsAmount) {
			moveObjectsToPlayerInventory ();
		} else {
			enableNumberOfObjectsToUseMenu (currentSlotToMoveInventoryObject.button.GetComponent<RectTransform> ());
		}
	}

	public void dragToBankInventory ()
	{
		if (showDebugPrint) {
			print ("dropped correctly to bank inventory");
		}

		currentObjectAlreadyInBankList = false;
		currentBankInventoryListIndex = -1;

		for (int j = 0; j < currentBankInventoryList.Count; j++) {
			if (!currentObjectAlreadyInBankList && currentBankInventoryList [j].Name.Equals (currentSlotToMoveInventoryObject.Name)) {
				currentObjectAlreadyInBankList = true;
				currentBankInventoryListIndex = j;
			}
		}

		currentAmountToMove = currentSlotToMoveInventoryObject.amount;

		if (dropFullObjectsAmount) {
			moveObjectsToBankInventory ();
		} else {
			enableNumberOfObjectsToUseMenu (currentSlotToMoveInventoryObject.button.GetComponent<RectTransform> ());
		}
	}

	public void moveObjectsToBankInventory ()
	{
		if (currentObjectAlreadyInBankList) {
			currentBankInventoryList [currentBankInventoryListIndex].amount += currentAmountToMove;

			if (showDebugPrint) {
				print ("combine objects amount + " + currentAmountToMove);
			}
		} else {
			currentSlotToMoveInventoryObject.amount = currentAmountToMove;

			currentBankInventoryList.Add (new inventoryInfo (currentSlotToMoveInventoryObject));

			int lastAddedObjectIndex = currentBankInventoryList.Count;

			createInventoryIcon (currentBankInventoryList [lastAddedObjectIndex - 1], lastAddedObjectIndex - 1, bankInventorySlot, 
				bankInventorySlotsContent, bankIconElementList);

			if (showDebugPrint) {
				print ("new object added +" + currentAmountToMove);
			}
		}

		playerInventoryManager.moveObjectToBank (objectToMoveIndex, currentAmountToMove);

		getCurrentPlayerInventoryList ();

		updateFullInventorySlots (playerInventoryList, playerIconElementList, true);

		updateFullInventorySlots (currentBankInventoryList, bankIconElementList, false);

		resetDragDropElements ();

		resetBankInventoryRectTransform ();
	}

	public void moveObjectsToPlayerInventory ()
	{
		int currentSlotToMoveAmount = currentSlotToMoveInventoryObject.amount;
		currentSlotToMoveInventoryObject.amount = currentAmountToMove;

		if (!playerInventoryManager.isInventoryFull ()) {
			int inventoryAmountPicked = playerInventoryManager.tryToPickUpObject (currentSlotToMoveInventoryObject);

//			print ("amount taken " + inventoryAmountPicked);
			if (inventoryAmountPicked > 0) {
				currentSlotToMoveInventoryObject.amount = currentSlotToMoveAmount - inventoryAmountPicked;
			} else {
				playerInventoryManager.showInventoryFullMessage ();
			}
		} else {
			playerInventoryManager.showInventoryFullMessage ();
		}

		getCurrentPlayerInventoryList ();

		updateFullInventorySlots (playerInventoryList, playerIconElementList, true);

		updateFullInventorySlots (currentBankInventoryList, bankIconElementList, false);

		removeButton (currentBankInventoryList, bankIconElementList);

		LayoutRebuilder.ForceRebuildLayoutImmediate (bankInventorySlotsContent.GetComponent<RectTransform> ());

		resetDragDropElements ();

		resetBankInventoryRectTransform ();
	}
}