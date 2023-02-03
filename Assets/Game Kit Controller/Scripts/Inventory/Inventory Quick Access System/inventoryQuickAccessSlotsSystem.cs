using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class inventoryQuickAccessSlotsSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int numberQuickAccessSlots = 10;

	public bool useInventoryQuickAccessSlots = true;

	public float timeToDrag = 0.5f;

	public bool useDragDropInventorySlots;

	[Space]
	[Header ("Show Slots Settings")]
	[Space]

	public bool showQuickAccessSlotsAlways = true;

	public float showQuickAccessSlotsParentDuration = 1;
	public float quickAccessSlotsParentScale = 0.7f;

	public bool setQuickAccessSlotsAlphaValueOutOfInventory;
	public float quickAccessSlotsAlphaValueOutOfInventory;

	public bool showQuickAccessSlotsWhenChangingSlot = true;

	public bool showQuickAccessSlotSelectedIcon = true;

	public bool swapInventoryObjectSlotsOnGridEnabled = true;

	public float minTimeToSelectQuickAccessSlot = 0.4f;

	public bool showSlotAmountEnabled = true;

	public bool showSlotAmmoAmountEnabled = true;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool disableQuickAccessSlotsWhenChangingFromFireWeaponsMode = false;
	public bool disableQuickAccessSlotsWhenChangingFromMeleeWeaponsMode = false;

	public bool holsterFireWeaponIfSlotAlreadySelected = true;
	public bool sheatheMeleeWeaponsIfSlotAlreadySelected = true;

	[Space]
	[Header ("Input Settings")]
	[Space]

	public bool quickAccessInputNumberKeysEnabled = true;
	public bool quickAccessInputMouseWheelActive;
	public bool changeWeaponsWithKeysActive;

	[Space]
	[Header ("Character Customization Slots Settings")]
	[Space]

	public List<GameObject> characterCustomizationSlotGameObjectList = new List<GameObject> ();


	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool currentObjectCanBeEquipped;
	public bool currentObjectCanBeUsed;

	public int currentSlotIndex;

	public bool pressedObjecWithoutEquipOrUseProperty;

	public bool draggedFromInventoryList;
	public bool draggedFromQuickAccessSlots;

	public bool inventoryOpened;

	public bool customizingCharacterActive;

	public bool checkObjectCategoriesToUseActive;
	public List<string> currentCategoryListToUseActive = new List<string> ();

	public bool checkSlotsCategoryNameToDrop;

	public bool quickAccessInputNumberKeysPaused;

	public bool changeOfWeaponsInProccess;

	[Space]
	[Header ("Quick Access Slots Debug")]
	[Space]

	public List<inventoryQuickAccessSlotElement.quickAccessSlotInfo> characterCustomizationSlotList = new List<inventoryQuickAccessSlotElement.quickAccessSlotInfo> ();

	public List<inventoryQuickAccessSlotElement.quickAccessSlotInfo> quickAccessSlotInfoList = new List<inventoryQuickAccessSlotElement.quickAccessSlotInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventToSetFireWeaponsMode;
	public UnityEvent eventToSetMeleeWeaponsMode;

	[Space]
	[Header ("UI Elements")]
	[Space]

	public GameObject slotPrefab;

	public GameObject inventoryQuickAccessSlots;

	public GameObject quickAccessSlotToMove;

	public GameObject quickAccessSlotSelectedIcon;

	public Transform quickAccessSlotsParentOnInventory;
	public Transform quickAccessSlotsParentOutOfInventory;

	[Space]
	[Header ("Components")]
	[Space]

	public inventoryManager mainInventoryManager;

	public playerWeaponsManager mainPlayerWeaponsManager;

	public meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager;

	public playerInputManager playerInput;


	bool touchPlatform;

	Touch currentTouch;

	bool touching;

	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();

	float lastTimeTouched;
	bool slotToMoveFound;

	GameObject slotFoundOnDrop;
	inventoryInfo currentSlotToMoveInventoryObject;
	inventoryQuickAccessSlotElement.quickAccessSlotInfo quickSlotFoundOnPress;

	float currentTimeTime;

	bool inventorySlotReadyToDrag;

	bool activatingDualWeaponSlot;

	string currentRighWeaponName;
	string currentLeftWeaponName;

	bool showQuickAccessSlotsPaused;

	Coroutine slotsParentCouroutine;

	List<inventoryInfo> inventoryList = new List<inventoryInfo> ();

	float lastTimeQuickAccessSlotSelected;

	int quickAccessSlotInfoListCount;

	List<inventoryQuickAccessSlotElement.quickAccessSlotInfo> currentQuickAccessList = new List<inventoryQuickAccessSlotElement.quickAccessSlotInfo> ();

	bool objectDetectedIsBroken;

	public void initializeQuickAccessSlots ()
	{
		for (int i = 0; i < numberQuickAccessSlots; i++) {
			GameObject newQuickAccessSlot = (GameObject)Instantiate (slotPrefab, Vector3.zero, Quaternion.identity, slotPrefab.transform.parent);
			newQuickAccessSlot.name = "Quick Access Slot " + (i + 1);

			newQuickAccessSlot.transform.localScale = Vector3.one;
			newQuickAccessSlot.transform.localPosition = Vector3.zero;

			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentQuickAccessSlotInfo = newQuickAccessSlot.GetComponent<inventoryQuickAccessSlotElement> ().mainQuickAccessSlotInfo;

			currentQuickAccessSlotInfo.Name = "";
			currentQuickAccessSlotInfo.slotActive = false;

			if (currentQuickAccessSlotInfo.slotMainSingleContent.activeSelf) {
				currentQuickAccessSlotInfo.slotMainSingleContent.SetActive (false);
			}

			int index = i;

			if (index == 9) {
				index = -1;
			}

			currentQuickAccessSlotInfo.iconNumberKeyText.text = "[" + (index + 1) + "]";

			quickAccessSlotInfoList.Add (currentQuickAccessSlotInfo);
		}
			
		quickAccessSlotInfoListCount = quickAccessSlotInfoList.Count;

		currentQuickAccessList = quickAccessSlotInfoList;


		for (int i = 0; i < characterCustomizationSlotGameObjectList.Count; i++) {
			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentQuickAccessSlotInfo = characterCustomizationSlotGameObjectList [i].GetComponent<inventoryQuickAccessSlotElement> ().mainQuickAccessSlotInfo;
		
			characterCustomizationSlotList.Add (currentQuickAccessSlotInfo);
		}


		touchPlatform = touchJoystick.checkTouchPlatform ();

		slotPrefab.SetActive (false);

		if (useInventoryQuickAccessSlots) {
			mainPlayerWeaponsManager.setChangeWeaponsWithNumberKeysActiveState (false);
			mainPlayerWeaponsManager.setChangeWeaponsWithKeysActive (false);
		} else {
			if (inventoryQuickAccessSlots.activeSelf) {
				inventoryQuickAccessSlots.SetActive (false);
			}
		}
	}

	public void updateInventoryOpenedState ()
	{
		if (useDragDropInventorySlots && !mainInventoryManager.examiningObject) {
			int touchCount = Input.touchCount;
			if (!touchPlatform) {
				touchCount++;
			}

			currentTimeTime = mainInventoryManager.getTimeTime ();

			for (int i = 0; i < touchCount; i++) {
				if (!touchPlatform) {
					currentTouch = touchJoystick.convertMouseIntoFinger ();
				} else {
					currentTouch = Input.GetTouch (i);
				}

				if (currentTouch.phase == TouchPhase.Began && !touching) {
					touching = true;

					lastTimeTouched = currentTimeTime;

					captureRaycastResults.Clear ();

					PointerEventData p = new PointerEventData (EventSystem.current);
					p.position = currentTouch.position;
					p.clickCount = i;
					p.dragging = false;

					EventSystem.current.RaycastAll (p, captureRaycastResults);

					foreach (RaycastResult r in captureRaycastResults) {
						if (!slotToMoveFound) {
							currentObjectCanBeEquipped = false;
							currentObjectCanBeUsed = false;

							pressedObjecWithoutEquipOrUseProperty = false;

							objectDetectedIsBroken = false;

							inventoryMenuIconElement currentInventoryMenuIconElement = r.gameObject.GetComponent<inventoryMenuIconElement> ();

							inventoryQuickAccessSlotElement currentQuickAccessSlotInfo = r.gameObject.GetComponent<inventoryQuickAccessSlotElement> ();

							if (currentInventoryMenuIconElement != null) {
								bool checkDurabilityOnObjectEnabled = mainInventoryManager.checkDurabilityOnObjectEnabled;

								inventoryList = mainInventoryManager.inventoryList;

								int inventoryListCount = inventoryList.Count;

								for (int j = 0; j < inventoryListCount; j++) {
									inventoryInfo currentInventoryInfo = inventoryList [j];

									if (currentInventoryInfo.amount > 0 && currentInventoryInfo.button == currentInventoryMenuIconElement.button) {
										currentObjectCanBeEquipped = currentInventoryInfo.canBeEquiped;
										currentObjectCanBeUsed = currentInventoryInfo.canBeUsed;

										bool canBePlaceOnQuickAccessSlot = currentInventoryInfo.canBePlaceOnQuickAccessSlot;

										if (showDebugPrint) {
											print ("inventory object pressed " + currentInventoryInfo.Name + " " + currentObjectCanBeEquipped + " " + currentObjectCanBeUsed);
										}

										bool slotCanBeSelectedd = false;

										if (currentObjectCanBeEquipped) {
											slotCanBeSelectedd = true;
										}

										if (currentObjectCanBeUsed) {
											slotCanBeSelectedd = true;
										}

										if (canBePlaceOnQuickAccessSlot) {
											slotCanBeSelectedd = true;
										} else {
											slotCanBeSelectedd = false;
										}

										if (checkObjectCategoriesToUseActive) {
											if (!currentCategoryListToUseActive.Contains (currentInventoryInfo.categoryName)) {
												slotCanBeSelectedd = false;
											}
										}

										if (checkDurabilityOnObjectEnabled) {
											if (slotCanBeSelectedd) {
												if (currentInventoryInfo.objectIsBroken) {
													if (mainInventoryManager.brokenObjectsCantBeEquipped) {
														slotCanBeSelectedd = false;

														objectDetectedIsBroken = true;
													}
												}
											}
										}

										if (slotCanBeSelectedd) {
											currentSlotToMoveInventoryObject = currentInventoryInfo;

											slotToMoveFound = true;

											draggedFromInventoryList = true;
										} else {
											if (swapInventoryObjectSlotsOnGridEnabled && !customizingCharacterActive) {
												pressedObjecWithoutEquipOrUseProperty = true;

												currentSlotToMoveInventoryObject = currentInventoryInfo;

												slotToMoveFound = true;

												draggedFromInventoryList = true;
											}
										}
									}
								}
							} else {
								if (currentQuickAccessSlotInfo != null) {
									for (int j = 0; j < quickAccessSlotInfoListCount; j++) {
										inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [j];

										if (currentSlotInfo.slot == currentQuickAccessSlotInfo.mainQuickAccessSlotInfo.slot) {
											if (currentSlotInfo.slotActive) {
												quickSlotFoundOnPress = currentSlotInfo;

												slotToMoveFound = true;

												draggedFromQuickAccessSlots = true;
											}
										}
									}
								}
							}

							if (slotToMoveFound) {
								RawImage slotToMoveRawImage = quickAccessSlotToMove.GetComponentInChildren<RawImage> ();

								if (draggedFromInventoryList) {
									slotToMoveRawImage.texture = currentInventoryMenuIconElement.icon.texture;
								}

								if (draggedFromQuickAccessSlots) {
									if (currentQuickAccessSlotInfo.mainQuickAccessSlotInfo.secondarySlotActive) {
										slotToMoveRawImage.texture = currentQuickAccessSlotInfo.mainQuickAccessSlotInfo.leftSecondarySlotIcon.texture;
									} else {
										slotToMoveRawImage.texture = currentQuickAccessSlotInfo.mainQuickAccessSlotInfo.slotIcon.texture;
									}
								}
							}
						}
					}
				}

				if ((currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved) && touching) {
					if (slotToMoveFound) {
						if (touching && currentTimeTime > lastTimeTouched + timeToDrag) {
							if (!quickAccessSlotToMove.activeSelf) {
								quickAccessSlotToMove.SetActive (true);
							}

							inventorySlotReadyToDrag = true;

							quickAccessSlotToMove.GetComponent<RectTransform> ().position = new Vector2 (currentTouch.position.x, currentTouch.position.y);
						}
					}
				}

				//if the mouse/finger press is released, then
				if (currentTouch.phase == TouchPhase.Ended && touching) {
					touching = false;

					if (slotToMoveFound && inventorySlotReadyToDrag) {
						//get the elements in the position where the player released the power element
						captureRaycastResults.Clear ();

						PointerEventData p = new PointerEventData (EventSystem.current);
						p.position = currentTouch.position;
						p.clickCount = i;
						p.dragging = false;
						EventSystem.current.RaycastAll (p, captureRaycastResults);

						bool checkingToSwapObjectsPositionOnGrid = false;

						foreach (RaycastResult r in captureRaycastResults) {
							if (r.gameObject != quickAccessSlotToMove) {
								if (r.gameObject.GetComponent<inventoryQuickAccessSlotElement> ()) {
									slotFoundOnDrop = r.gameObject;
								} else if (r.gameObject.GetComponent<inventoryMenuIconElement> ()) {
									slotFoundOnDrop = r.gameObject;

									if (swapInventoryObjectSlotsOnGridEnabled) {
										if (draggedFromInventoryList) {
											checkingToSwapObjectsPositionOnGrid = true;
										}
									}
								}
							}
						}

						if (checkingToSwapObjectsPositionOnGrid) {
							swapInventoryObjectsPositionOnGrid ();
						} else {
							checkDroppedSlot ();
						}
					} else {
						resetDragAndDropSlotState ();
					}

					inventorySlotReadyToDrag = false;

					slotToMoveFound = false;
				}
			}
		}
	}



	void swapInventoryObjectsPositionOnGrid ()
	{
		if (draggedFromInventoryList) {
			if (slotFoundOnDrop != null) {
				if (showDebugPrint) {
					print ("checking to swap objects slots on the grid");
				}

				inventoryMenuIconElement currentInventoryMenuIconElement = slotFoundOnDrop.GetComponent<inventoryMenuIconElement> ();

				if (currentInventoryMenuIconElement != null) {

					inventoryList = mainInventoryManager.inventoryList;

					bool objectsSwaped = false;

					int inventoryListCount = inventoryList.Count;

					for (int j = 0; j < inventoryListCount; j++) {
						if (!objectsSwaped) {
							inventoryInfo currentInventoryInfo = inventoryList [j]; 

							if (currentInventoryInfo.button == currentInventoryMenuIconElement.button) {
								if (showDebugPrint) {
									print ("slot dragged from inventory grid dropped into " + currentInventoryInfo.Name);
								}

								if (currentSlotToMoveInventoryObject != currentInventoryInfo) {
									int slotToMoveIndex1 = mainInventoryManager.getInventoryObjectIndexByName (currentSlotToMoveInventoryObject.Name);

									int slotToMoveIndex2 = j;
								

									inventoryList [slotToMoveIndex1] = currentInventoryInfo;

									inventoryList [slotToMoveIndex2] = currentSlotToMoveInventoryObject;
										

									mainInventoryManager.updateFullInventorySlots ();

									objectsSwaped = true;
								}
							}
						}
					}
				}
			}
		}

		resetDragAndDropSlotState ();
	}

	public void checkDroppedSlot ()
	{
		//SITUATIONS THAT CAN HAPPEN WHEN DROPPING A SLOT

		//---PRESSING IN THE INVENTORY GRID

		//DROPPING OUT OF THE INVENTORY-No action
		//DROPPING IN THE INVENTORY GRID-No action
		//DROPPING IN THE QUICK ACCESS SLOTS:
		//DROPPING FIRE WEAPON-
		//DROPPING MELEE WEAPON-
		//DROPPING REGULAR OBJECT-

		//THE QUICK ACCESS SLOT DETECTED IN DROP IS:
		//_EMPTY-Assign inventory object there
		//_NOT EMPTY-If regular object, replace-If melee weapon, replace-If fire weapon, check to combine
		//If the slot was already on the quick access slots, move of place
		//If the object can be equipped, equip it
		//If there was a previous object equipped on it, unequip it 

		//---PRESSING IN THE QUICK ACCESS SLOTS

		//DROPPPING OUT OF THE INVENTORY-Clean quick access slot
		//DROPPING IN THE QUICK ACCESS SLOTS:
		//DROPPING FIRE WEAPON-
		//DROPPING MELEE WEAPON-
		//DROPPING REGULAR OBJECT-

		//THE QUICK ACCESS SLOT DETECTED IN DROP IS:
		//_EMPTY-Move inventory object from previous quick access slot to new one
		//_NOT EMPTY-If regular object, replace-If melee weapon, replace-If fire weapon, replace
		//If the object can be equipped, equip it
		//If there was a previous object equipped on it, unequip it 


		//A quick access slots has been found when dropping the pressed inventory/quick access slot
		//and the pressed slot is not a quick access or hasn't a secondary object assigned

		if (pressedObjecWithoutEquipOrUseProperty || objectDetectedIsBroken) {
			if (showDebugPrint) {
				print ("current object selected can't be equipped or used, so only swap position with another slots in the inventory grid is allowed");
			}

			resetDragAndDropSlotState ();

			return;
		}

		if (slotFoundOnDrop != null && (quickSlotFoundOnPress == null || !quickSlotFoundOnPress.secondarySlotActive)) {
			bool slotDroppedFoundOnQuickAccessList = false;

			int slotFoundOnDropIndex = -1;

			activatingDualWeaponSlot = false;

			bool activateDualWeaponsEnabled = mainPlayerWeaponsManager.isActivateDualWeaponsEnabled ();

			mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

			if (draggedFromInventoryList || draggedFromQuickAccessSlots) {
				if (showDebugPrint) {
					if (draggedFromInventoryList) {
						print ("dragged from Inventory List to quick access slots");
					}

					if (draggedFromQuickAccessSlots) {
						print ("dragged from quick access slots to unequip or change object");
					}
				}

				inventoryQuickAccessSlotElement slotElementFoundOnDrop = slotFoundOnDrop.GetComponent<inventoryQuickAccessSlotElement> ();

				//the slot pressed has been dropped in the quick access slots
				if (slotElementFoundOnDrop != null) {
					//Make initial checks on the slots pressed and dropped
					if (draggedFromInventoryList) {
						if (slotElementFoundOnDrop.mainQuickAccessSlotInfo.secondarySlotActive) {
							if (showDebugPrint) {
								print ("The weapon slot where you are trying to combine the weapon " + currentSlotToMoveInventoryObject.mainWeaponObjectInfo.getWeaponName () +
								" is already active as dual weapon slot");
							}

							resetDragAndDropSlotState ();

							return;
						} else {
							if (currentSlotToMoveInventoryObject.isWeapon) {
								if (showDebugPrint) {
									print ("using a weapon slot");
								}

								bool isMeleeWeapon = currentSlotToMoveInventoryObject.isMeleeWeapon;

								if (!isMeleeWeapon) {
									if (showDebugPrint) {
										print ("using a fire weapon slot");
									}

									string weaponNameToEquip = currentSlotToMoveInventoryObject.mainWeaponObjectInfo.getWeaponName ();

									for (int j = 0; j < quickAccessSlotInfoListCount; j++) {
										inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [j];

										if (currentSlotInfo.secondarySlotActive) {
											if (currentSlotInfo.firstElementName.Equals (weaponNameToEquip) || currentSlotInfo.secondElementName.Equals (weaponNameToEquip)) {
												if (showDebugPrint) {
													print ("The weapon slot where you are trying to combine the weapon " +
													currentSlotToMoveInventoryObject.mainWeaponObjectInfo.getWeaponName () +
													" is already active as dual weapon slot in another slot");
												}

												resetDragAndDropSlotState ();

												return;
											}
										}
									}
								}

								if (showDebugPrint) {
									if (isMeleeWeapon) {
										print ("using a melee weapon slot");
									}
								}
							} else {
								if (currentSlotToMoveInventoryObject.isArmorClothAccessory) {
									if (checkSlotsCategoryNameToDrop) {
										GameObject currentInventoryGameObject = mainInventoryManager.getInventoryPrefab (currentSlotToMoveInventoryObject.inventoryGameObject);

										if (currentInventoryGameObject != null) {

											armorClothPickup currentPickupObject = currentInventoryGameObject.GetComponent<armorClothPickup> ();

											if (currentPickupObject != null) {

												string currentSlotCategoryName = currentPickupObject.categoryName;

												string currentSlotFoundOnDropName = slotElementFoundOnDrop.mainQuickAccessSlotInfo.slotCategoryName;

												if (currentSlotFoundOnDropName != "") {
													if (showDebugPrint) {
														print ("Current category to check " + currentSlotCategoryName +
														" and current category slot found " + currentSlotFoundOnDropName);
													}

													if (!currentSlotFoundOnDropName.Equals (currentSlotCategoryName)) {

														if (showDebugPrint) {
															print ("slot dropped is not the same category as the slot found for " +
															currentSlotFoundOnDropName + " cancelling drop object into slot");
														}

														resetDragAndDropSlotState ();

														return;
													}
												}
											}
										}
									}
								} 

								if (showDebugPrint) {
									print ("slot moved is object to use, like quest, or consumable, etc....");
								}
							}
						}
					}

					//The slot dropped was already in the quick access slots
					for (int j = 0; j < quickAccessSlotInfoListCount; j++) {
						if (currentQuickAccessList [j].slot == slotElementFoundOnDrop.mainQuickAccessSlotInfo.slot) {
							slotDroppedFoundOnQuickAccessList = true;
							slotFoundOnDropIndex = j;
						}
					}

					//check that the current weapon slot added is not already present in the list, in that case, reset that slot to update with the new one
					if (draggedFromInventoryList) {
						//Check if the use of dual weapons is disabled, to avoid to combine fire weapons
//						if (!activateDualWeaponsEnabled && slotDroppedFoundOnQuickAccessList) {
//							if (slotElementFoundOnDrop.mainQuickAccessSlotInfo.slotActive) {
//								if (slotElementFoundOnDrop.mainQuickAccessSlotInfo.inventoryInfoAssigned.isWeapon &&
//								    !slotElementFoundOnDrop.mainQuickAccessSlotInfo.inventoryInfoAssigned.isMeleeWeapon) {
//
//									if (showDebugPrint) {
//										print ("Use of dual weapons is disabled, cancelling action");
//									}
//
//									resetDragAndDropSlotState ();
//
//									return;
//								}
//							}
//						}

						//Reseting the info of the quick access slot that is assigned to the inventory slot pressed
						for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
							inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [i];

							if (currentSlotInfo.slotActive && currentSlotInfo.Name.Equals (currentSlotToMoveInventoryObject.Name)) {
								updateQuickAccessSlotInfo (-1, null, currentSlotInfo, null);
							}
						}
					}

					//Check if the object was pressed from the quick access slots
					if (draggedFromQuickAccessSlots) {
						//The slot is droppend is already on the quick access slots, so it is being moved from one place to another
						if (slotDroppedFoundOnQuickAccessList) {
							if (quickSlotFoundOnPress == slotElementFoundOnDrop.mainQuickAccessSlotInfo) {
								if (showDebugPrint) {
									print ("moving object slot to the same place, nothing to do");
								}


							} else {
								string currentObjectName = quickSlotFoundOnPress.inventoryInfoAssigned.Name;

								if (showDebugPrint) {
									print ("Object slot " + currentObjectName + " changed to " +
									"replace " + slotElementFoundOnDrop.mainQuickAccessSlotInfo.Name);
								}

								if (quickSlotFoundOnPress.inventoryInfoAssigned.isWeapon) {
									bool isMeleeWeapon = quickSlotFoundOnPress.inventoryInfoAssigned.isMeleeWeapon;

									if (!isMeleeWeapon) {
										
										mainPlayerWeaponsManager.unequipWeapon (slotElementFoundOnDrop.mainQuickAccessSlotInfo.Name, false);

										mainPlayerWeaponsManager.selectWeaponByName (currentObjectName, true);

										updateQuickAccessSlotInfo (slotFoundOnDropIndex, quickSlotFoundOnPress.inventoryInfoAssigned, null, null);
									}

									if (isMeleeWeapon) {
										mainMeleeWeaponsGrabbedManager.unEquipMeleeWeapon (slotElementFoundOnDrop.mainQuickAccessSlotInfo.Name, false);

										mainMeleeWeaponsGrabbedManager.equipMeleeWeapon (currentObjectName, false);

										updateQuickAccessSlotInfo (slotFoundOnDropIndex, quickSlotFoundOnPress.inventoryInfoAssigned, null, null);
									}
								} else {
									if (showDebugPrint) {
										print ("slot moved is object to use, like quest, or consumable, etc....");

										print ("assign regular inventory object to quick access slot");
									}

									updateQuickAccessSlotInfo (slotFoundOnDropIndex, currentSlotToMoveInventoryObject, null, null);
								}
							}
						}

						//Reseting the info of the quick access slot that is assigned to the quick access slot pressed
						for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
							inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [i];

							if (slotFoundOnDropIndex != i && currentSlotInfo.slotActive && currentSlotInfo.Name.Equals (quickSlotFoundOnPress.Name)) {
								updateQuickAccessSlotInfo (-1, null, currentSlotInfo, null);
							}
						}
					}

					if (draggedFromInventoryList) {
						//The slot found on drop is already active, so replace it or combine dual wepaons
						if (slotElementFoundOnDrop.mainQuickAccessSlotInfo.slotActive) {
							if (currentSlotToMoveInventoryObject.isWeapon) {
								bool isMeleeWeapon = currentSlotToMoveInventoryObject.isMeleeWeapon;

								bool currentSlotFoundOnDropIsFireWeapon = !slotElementFoundOnDrop.mainQuickAccessSlotInfo.inventoryInfoAssigned.isMeleeWeapon;

								if (!isMeleeWeapon) {
									bool replaceForMeleeWeaponResult = false;

									if (activateDualWeaponsEnabled) {
										if (currentSlotFoundOnDropIsFireWeapon) {
											currentRighWeaponName = currentSlotToMoveInventoryObject.mainWeaponObjectInfo.getWeaponName ();
											currentLeftWeaponName = slotElementFoundOnDrop.mainQuickAccessSlotInfo.inventoryInfoAssigned.mainWeaponObjectInfo.getWeaponName ();

											mainInventoryManager.setCurrentRighWeaponNameValue (currentRighWeaponName);
											mainInventoryManager.setCurrentLeftWeaponNameValue (currentLeftWeaponName);

											if (mainPlayerWeaponsManager.checkIfWeaponIsOnSamePocket (currentRighWeaponName, currentLeftWeaponName)) {
												if (showDebugPrint) {
													print ("trying to equip dual weapons from the same weapon pocket, combine weapons cancelled");
												}

												resetDragAndDropSlotState ();

												return;
											}

											if (showDebugPrint) {
												print ("equipping dual weapon");
											}

											activatingDualWeaponSlot = true;

											mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

											playerWeaponSystem currentPlayerWeaponSystem = mainPlayerWeaponsManager.getWeaponSystemByName (slotElementFoundOnDrop.mainQuickAccessSlotInfo.inventoryInfoAssigned.mainWeaponObjectInfo.getWeaponName ());

											updateQuickAccessSlotInfo (slotFoundOnDropIndex, currentSlotToMoveInventoryObject, null, currentPlayerWeaponSystem);

											mainInventoryManager.setCurrentInventoryObject (currentSlotToMoveInventoryObject);

											mainInventoryManager.equipCurrentObject ();
										} else {
											replaceForMeleeWeaponResult = true;
										}
									} else {
										if (currentSlotFoundOnDropIsFireWeapon) {
											mainPlayerWeaponsManager.unequipWeapon (slotElementFoundOnDrop.mainQuickAccessSlotInfo.Name, false);

											mainPlayerWeaponsManager.selectWeaponByName (currentSlotToMoveInventoryObject.mainWeaponObjectInfo.getWeaponName (), true);

											updateQuickAccessSlotInfo (slotFoundOnDropIndex, currentSlotToMoveInventoryObject, null, null);
										} else {
											replaceForMeleeWeaponResult = true;
										}
									}

									if (replaceForMeleeWeaponResult) {
										mainMeleeWeaponsGrabbedManager.unEquipMeleeWeapon (slotElementFoundOnDrop.mainQuickAccessSlotInfo.Name, false);

										mainInventoryManager.unEquipObjectByName (slotElementFoundOnDrop.mainQuickAccessSlotInfo.Name);
									}
								}

								if (isMeleeWeapon) {
									mainMeleeWeaponsGrabbedManager.unEquipMeleeWeapon (slotElementFoundOnDrop.mainQuickAccessSlotInfo.Name, false);

									mainMeleeWeaponsGrabbedManager.equipMeleeWeapon (currentSlotToMoveInventoryObject.Name, false);

									updateQuickAccessSlotInfo (slotFoundOnDropIndex, currentSlotToMoveInventoryObject, null, null);
								}
							} else {
								if (showDebugPrint) {
									print ("slot moved is object to use, like quest, or consumable, etc....");

									print ("assign regular inventory object to quick access slot");
								}

								if (slotElementFoundOnDrop.mainQuickAccessSlotInfo.inventoryInfoAssigned.canBeEquiped) {
									mainInventoryManager.setCurrentInventoryObject (slotElementFoundOnDrop.mainQuickAccessSlotInfo.inventoryInfoAssigned);

									mainInventoryManager.unEquipCurrentObject ();
								}

								updateQuickAccessSlotInfo (slotFoundOnDropIndex, currentSlotToMoveInventoryObject, null, null);
							}
						}
					}
				}
			}


			//The slot droppped was already on the quick access slots
			if (slotDroppedFoundOnQuickAccessList) {
				//the slot was press on the inventory grid
				if (draggedFromInventoryList && !activatingDualWeaponSlot) {
					//checking to equip the current object 
					if (currentSlotToMoveInventoryObject.canBeEquiped) {
//						if (!currentSlotToMoveInventoryObject.isMeleeWeapon) {
						updateQuickAccessSlotInfo (slotFoundOnDropIndex, currentSlotToMoveInventoryObject, null, null);

						mainInventoryManager.setCurrentInventoryObject (currentSlotToMoveInventoryObject);

						mainInventoryManager.equipCurrentObject ();
//						}
//
//						if (!currentSlotToMoveInventoryObject.isMeleeWeapon) {
//
//						}
					} else {
						if (showDebugPrint) {
							print ("slot moved is object to use, like quest, or consumable, etc....");
						}

						updateQuickAccessSlotInfo (slotFoundOnDropIndex, currentSlotToMoveInventoryObject, null, null);

					}
				}

				if (showDebugPrint) {
					print ("dropped correctly");
				}
			} else {
				if (showDebugPrint) {
					print ("dropped outside the list");
				}

				//the pressed slot has been dropped outside, so remove its info and unequip if possible
				if (quickSlotFoundOnPress != null) {
					if (quickSlotFoundOnPress.secondarySlotActive) {
						if (showDebugPrint) {
							print ("Dual weapon " + quickSlotFoundOnPress.inventoryInfoAssigned.mainWeaponObjectInfo.getWeaponName () + " removed");
						}

						activatingDualWeaponSlot = true;

						mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

						mainPlayerWeaponsManager.unequipWeapon (quickSlotFoundOnPress.secondElementName, activatingDualWeaponSlot);
					} else {
						if (quickSlotFoundOnPress.inventoryInfoAssigned.isWeapon) {
							bool isMeleeWeapon = quickSlotFoundOnPress.inventoryInfoAssigned.isMeleeWeapon;

							if (!isMeleeWeapon) {
								if (showDebugPrint) {
									print ("dragged fire weapon to unequip");
								}

								mainPlayerWeaponsManager.unequipWeapon (quickSlotFoundOnPress.Name, false);
							}

							if (isMeleeWeapon) {
								if (showDebugPrint) {
									print ("dragged melee Weapon to unequip");
								}

								mainMeleeWeaponsGrabbedManager.unEquipMeleeWeapon (quickSlotFoundOnPress.Name, false);

								mainInventoryManager.unEquipObjectByName (quickSlotFoundOnPress.Name);
							}
						} else {
							if (showDebugPrint) {
								print ("slot moved is object to use, like quest, or consumable, etc....");
							}

							if (quickSlotFoundOnPress.inventoryInfoAssigned.canBeEquiped) {
								if (showDebugPrint) {
									print ("dragged object to unequip");
								}

								mainInventoryManager.setCurrentInventoryObject (quickSlotFoundOnPress.inventoryInfoAssigned);

								mainInventoryManager.unEquipCurrentObject ();
							}
						}
					}

					updateQuickAccessSlotInfo (-1, null, quickSlotFoundOnPress, null);
				}
			}
		} else {
			checkSlotInfoToRemove ();
		}

		resetDragAndDropSlotState ();
	}

	void checkSlotInfoToRemove ()
	{
		//The slot has been pressed on the quick access slots and dropped out
		if (draggedFromQuickAccessSlots) {
			//dragging a slot with two weapons assigned
			if (quickSlotFoundOnPress.secondarySlotActive) {
				string currentObjectName = quickSlotFoundOnPress.inventoryInfoAssigned.mainWeaponObjectInfo.getWeaponName ();

				if (showDebugPrint) {
					print ("Dual weapon " + currentObjectName + " removed");
				}

				activatingDualWeaponSlot = true;

				mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

				currentRighWeaponName = currentObjectName;
				currentLeftWeaponName = quickSlotFoundOnPress.secondElementName;

				mainInventoryManager.setCurrentRighWeaponNameValue (currentRighWeaponName);
				mainInventoryManager.setCurrentLeftWeaponNameValue (currentLeftWeaponName);


				string weaponNameToUnequip = currentLeftWeaponName;

				bool firstWeaponSharesPocket = checkIfWeaponSharesPocketInWeaponSlots (currentLeftWeaponName);
				bool secondWeaponSharesPocket = false;

				if (!firstWeaponSharesPocket) {

					secondWeaponSharesPocket = checkIfWeaponSharesPocketInWeaponSlots (currentRighWeaponName);

					if (secondWeaponSharesPocket) {
						weaponNameToUnequip = currentRighWeaponName;
					}
				}

				print (firstWeaponSharesPocket + " " + secondWeaponSharesPocket);

				mainPlayerWeaponsManager.unequipWeapon (weaponNameToUnequip, activatingDualWeaponSlot);

				if (secondWeaponSharesPocket) {

					updateSingleWeaponSlotInfoWithoutAddingAnotherSlot (currentLeftWeaponName);

					resetDragAndDropSlotState ();

					return;
				}
			} else {
				//dragging a slot with a single object assigned
				if (quickSlotFoundOnPress.inventoryInfoAssigned.isWeapon) {
					bool isMeleeWeapon = quickSlotFoundOnPress.inventoryInfoAssigned.isMeleeWeapon;

					if (!isMeleeWeapon) {
						if (showDebugPrint) {
							print ("dragged fire weapon to unequip");
						}

						mainPlayerWeaponsManager.unequipWeapon (quickSlotFoundOnPress.Name, false);
					} 

					if (isMeleeWeapon) {
						mainMeleeWeaponsGrabbedManager.unEquipMeleeWeapon (quickSlotFoundOnPress.Name, false);

						mainInventoryManager.unEquipObjectByName (quickSlotFoundOnPress.Name);

						if (showDebugPrint) {
							print ("dragged melee Weapon to unequip");
						}
					}
				} else {
					if (showDebugPrint) {
						print ("slot moved is object to use, like quest, or consumable, etc....");
					}

					if (quickSlotFoundOnPress.inventoryInfoAssigned.canBeEquiped) {
						if (showDebugPrint) {
							print ("dragged object to unequip " + quickSlotFoundOnPress.inventoryInfoAssigned.Name);
						}

						mainInventoryManager.setCurrentInventoryObject (quickSlotFoundOnPress.inventoryInfoAssigned);

						mainInventoryManager.unEquipCurrentObject ();
					}
				}
			}

			updateQuickAccessSlotInfo (-1, null, quickSlotFoundOnPress, null);
		}
	}

	bool selectingSlotByInputActive;

	public void updateQuickAccessInputKeysState ()
	{
		if (quickAccessInputNumberKeysEnabled) {
			if (quickAccessInputNumberKeysPaused) {
				return;
			}

			if (changeOfWeaponsInProccess) {
				return;
			}

			int currentNumberInput = playerInput.checkNumberInput (numberQuickAccessSlots + 1);

			if (currentNumberInput > -1) {
				selectingSlotByInputActive = true;

				inputSetSlotByIndex (currentNumberInput);
			
				selectingSlotByInputActive = false;
			}
		}
	}

	public void inputSetSlotByIndex (int currentNumberInput)
	{
		if (!quickAccessInputNumberKeysEnabled) {
			return;
		}

		if (quickAccessInputNumberKeysPaused) {
			return;
		}

		if (changeOfWeaponsInProccess) {
			return;
		}

		if (currentNumberInput == 0) {
			currentNumberInput = 9;
		} else {
			currentNumberInput--;
		}

		if (quickAccessSlotInfoListCount == 0) {
			return;
		}

		if (currentNumberInput >= quickAccessSlotInfoListCount) {
			return;
		}

		if (!canUseInput ()) {
			return;
		}

		if (!canSelectSlot ()) {
			return;
		}

		checkQuickAccessSlotToSelect (currentNumberInput);
	}

	public void setQuickAccessInputNumberKeysPausedState (bool state)
	{
		quickAccessInputNumberKeysPaused = state;
	}

	public void checkQuickAccessSlotToSelectByName (string objectOnSlotName)
	{
		for (int j = 0; j < quickAccessSlotInfoListCount; j++) {
			if (currentQuickAccessList [j].slotActive) {
				if (currentQuickAccessList [j].inventoryInfoAssigned.Name.Equals (objectOnSlotName)) {
					checkQuickAccessSlotToSelect (j);

					return;
				}
			}
		}
	}

	bool canSelectSlot ()
	{
		if (mainPlayerWeaponsManager.isActionActiveInPlayer ()) {
			if (showDebugPrint) {
				print ("action active, cancelling quick access slot action");
			}

			return false;
		}

		if (mainPlayerWeaponsManager.currentWeaponIsMoving ()) {
			if (showDebugPrint) {
				print ("weapons are moving, cancelling quick access slot action");
			}

			return false;
		}

		if (mainPlayerWeaponsManager.weaponsAreMoving ()) {
			if (showDebugPrint) {
				print ("weapons are still moving, cancelling quick access slot action");
			}

			return false;
		}

		return true;
	}

	void checkQuickAccessSlotToSelect (int quickAccessSlotIndex)
	{
		if (Time.time < minTimeToSelectQuickAccessSlot + lastTimeQuickAccessSlotSelected) {
			if (showDebugPrint) {
				print ("not enough time in between press to change to another quick access slot");
			}

			return;
		}

		inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [quickAccessSlotIndex];

		if (showDebugPrint) {
			print ("Quick access number key pressed " + quickAccessSlotIndex + " with the object assigned " + currentSlotInfo.Name);
		}

		if (currentSlotInfo != null) {
			if (currentSlotInfo.slotActive) {
				inventoryInfo inventoryObjectInfo = currentSlotInfo.inventoryInfoAssigned;

				if (inventoryObjectInfo.isWeapon) {
					if (mainInventoryManager.isUsingGenericModelActive ()) {
						return;
					}

					if (!inventoryObjectInfo.isMeleeWeapon) {
						if (mainPlayerWeaponsManager.isWeaponsModeActive ()) {
							bool selectWeaponResult = true;

							if (selectingSlotByInputActive) {
								if (holsterFireWeaponIfSlotAlreadySelected) {
									if (canSelectSlot ()) {
										if (mainPlayerWeaponsManager.isUsingWeapons ()) {
											string weaponName = mainPlayerWeaponsManager.getCurrentWeaponName ();

											if (inventoryObjectInfo.Name.Equals (weaponName)) {
												mainPlayerWeaponsManager.drawOrKeepWeaponInput ();

												selectWeaponResult = false;
											}
										}
									}
								}
							}

							if (selectWeaponResult) {
								mainPlayerWeaponsManager.checkWeaponToSelectOnQuickAccessSlots (inventoryObjectInfo.Name, false);
							}

							lastTimeQuickAccessSlotSelected = Time.time;
						} else {
							changeFromMeleeToFireWeapons (inventoryObjectInfo.Name, quickAccessSlotIndex);
						}
					}

					if (inventoryObjectInfo.isMeleeWeapon) {
						if (mainMeleeWeaponsGrabbedManager.isMeleeWeaponsGrabbedManagerActive ()) {
							bool selectWeaponResult = true;

							if (selectingSlotByInputActive) {
								if (sheatheMeleeWeaponsIfSlotAlreadySelected) {
									if (canSelectSlot ()) {
										if (mainMeleeWeaponsGrabbedManager.characterIsCarryingWeapon ()) {
											string weaponName = mainMeleeWeaponsGrabbedManager.getCurrentWeaponName ();

											print (weaponName);

											if (inventoryObjectInfo.Name.Equals (weaponName) &&
											    mainMeleeWeaponsGrabbedManager.getLastTimeDrawMeleeWeapon () > 0.7f) {
												mainMeleeWeaponsGrabbedManager.checkToKeepWeapon ();

												selectWeaponResult = false;
											}
										}
									}
								}
							}

							if (selectWeaponResult) {
								mainMeleeWeaponsGrabbedManager.checkWeaponToSelectOnQuickAccessSlots (inventoryObjectInfo.Name);
							}

							showQuickAccessSlotsParentWhenSlotSelected (quickAccessSlotIndex + 1);

							lastTimeQuickAccessSlotSelected = Time.time;
						} else {
							changeFromFireToMeleeWeapons (inventoryObjectInfo.Name, quickAccessSlotIndex);
						}
					}
				} else {
					lastTimeQuickAccessSlotSelected = Time.time;

					if (inventoryObjectInfo.canBeEquiped) {
						if (showDebugPrint) {
							print ("object selected can be equipped");
						}

						mainInventoryManager.useEquippedObjectAction (inventoryObjectInfo.Name);
					} else {
						if (showDebugPrint) {
							print ("object selected is regular inventory object");
						}

						mainInventoryManager.setCurrentInventoryObject (inventoryObjectInfo);

						mainInventoryManager.setUsingCurrentObjectFromQuickAccessSlotsInventoryState (true);

						mainInventoryManager.useCurrentObject ();

						mainInventoryManager.setUsingCurrentObjectFromQuickAccessSlotsInventoryState (false);

						if (showDebugPrint) {
							print (inventoryObjectInfo.amount);
						}

						if (inventoryObjectInfo.amount <= 0) {
							if (showDebugPrint) {
								print ("object used " + inventoryObjectInfo.Name + " without amount remaining, removing from quick access slots");
							}

							updateQuickAccessSlotInfo (-1, null, currentSlotInfo, null);
						} else {
							updateQuickAccesSlotAmount (quickAccessSlotIndex);
						}
					}
				}
			}
		}
	}

	Coroutine changeBetweenWeaponsCoroutine;

	public void changeFromMeleeToFireWeapons (string weaponName, int quickAccessSlotIndex)
	{
		stopChangeOfWeaponsCoroutine ();

		changeBetweenWeaponsCoroutine = StartCoroutine (changeOfWeaponsCoroutine (true, weaponName, quickAccessSlotIndex));
	}

	public void changeFromFireToMeleeWeapons (string weaponName, int quickAccessSlotIndex)
	{
		stopChangeOfWeaponsCoroutine ();

		changeBetweenWeaponsCoroutine = StartCoroutine (changeOfWeaponsCoroutine (false, weaponName, quickAccessSlotIndex));
	}

	public void stopChangeOfWeaponsCoroutine ()
	{
		if (changeBetweenWeaponsCoroutine != null) {
			StopCoroutine (changeBetweenWeaponsCoroutine);
		}

		changeOfWeaponsInProccess = false;
	}

	IEnumerator changeOfWeaponsCoroutine (bool changingFromMeleeToFireWeapons, string weaponName, int quickAccessSlotIndex)
	{
		changeOfWeaponsInProccess = true;

		bool weaponHolstered = false;

		bool checkWeaponChange = true;

		if (changingFromMeleeToFireWeapons) {
			if (showDebugPrint) {
				print ("changing from melee to fire weapons");
			}

			if (mainMeleeWeaponsGrabbedManager.characterIsCarryingWeapon ()) {
				mainMeleeWeaponsGrabbedManager.drawOrKeepMeleeWeaponWithoutCheckingInputActive ();
			}
		} else {
			if (showDebugPrint) {
				print ("changing from fire to melee weapons");
			}

			if (mainPlayerWeaponsManager.isPlayerCarringWeapon ()) {
				mainPlayerWeaponsManager.checkIfKeepSingleOrDualWeapon ();
			}
		}

		if (checkWeaponChange) {
			yield return new WaitForSeconds (0.1f);

			float lastTimeChangeActive = Time.time;

			while (!weaponHolstered) {
				if (changingFromMeleeToFireWeapons) {
					if (!mainMeleeWeaponsGrabbedManager.characterIsCarryingWeapon ()) {
						weaponHolstered = true;
					}
				} else {
					if (mainPlayerWeaponsManager.checkPlayerIsNotCarringWeapons ()) {
						weaponHolstered = true;
					}
				}

				if (mainPlayerWeaponsManager.isActionActiveInPlayer ()) {
					weaponHolstered = false;
				}

				if (Time.time > lastTimeChangeActive + 4) {
					if (showDebugPrint) {
						print ("Check time too long for quick access slots, cancelling");
					}

					weaponHolstered = true;
				}

				yield return null;
			}

			if (changingFromMeleeToFireWeapons) {
				eventToSetFireWeaponsMode.Invoke ();

				if (showDebugPrint) {
					print ("object selected is fire weapon");
				}

				mainPlayerWeaponsManager.checkWeaponToSelectOnQuickAccessSlots (weaponName, true);
			} else {
				eventToSetMeleeWeaponsMode.Invoke ();

				if (showDebugPrint) {
					print ("object selected is melee weapon");
				}

				mainMeleeWeaponsGrabbedManager.checkWeaponToSelectOnQuickAccessSlots (weaponName);

				showQuickAccessSlotsParentWhenSlotSelected (quickAccessSlotIndex + 1);
			}
		}

		yield return null;

		lastTimeQuickAccessSlotSelected = Time.time;

		changeOfWeaponsInProccess = false;
	}

	public void changeToMeleeWeapons (string meleeWeaponName)
	{
		stopChangeOfWeaponsCoroutine ();

		int quickAccessSlotIndex = currentQuickAccessList.FindIndex (s => s.Name == meleeWeaponName);

		if (quickAccessSlotIndex > -1) {
			changeBetweenWeaponsCoroutine = StartCoroutine (changeToMeleeWeaponsCoroutine (meleeWeaponName, quickAccessSlotIndex));
		}
	}

	IEnumerator changeToMeleeWeaponsCoroutine (string weaponName, int quickAccessSlotIndex)
	{
		changeOfWeaponsInProccess = true;

		bool weaponHolstered = false;

		bool checkWeaponChange = true;

		if (showDebugPrint) {
			print ("changing from fire to melee weapons");
		}

		if (mainPlayerWeaponsManager.isPlayerCarringWeapon ()) {
			mainPlayerWeaponsManager.checkIfKeepSingleOrDualWeapon ();
		}

		if (checkWeaponChange) {
			yield return new WaitForSeconds (0.1f);

			float lastTimeChangeActive = Time.time;

			while (!weaponHolstered) {
				if (mainPlayerWeaponsManager.checkPlayerIsNotCarringWeapons ()) {
					weaponHolstered = true;
				}

				if (mainPlayerWeaponsManager.isActionActiveInPlayer ()) {
					weaponHolstered = false;
				}

				if (Time.time > lastTimeChangeActive + 4) {
					if (showDebugPrint) {
						print ("Check time too long for quick access slots, cancelling");
					}

					weaponHolstered = true;
				}

				yield return null;
			}

			eventToSetMeleeWeaponsMode.Invoke ();

			if (showDebugPrint) {
				print ("object selected is melee weapon");
			}

			mainMeleeWeaponsGrabbedManager.checkWeaponToSelectOnQuickAccessSlots (weaponName);

			showQuickAccessSlotsParentWhenSlotSelected (quickAccessSlotIndex + 1);
		}

		yield return null;

		lastTimeQuickAccessSlotSelected = Time.time;

		changeOfWeaponsInProccess = false;
	}


	//START INPUT FUNCTIONS
	public void inputSelectNextOrPreviousQuickAccessSlot (bool state)
	{
		if (!canUseInput ()) {
			return;
		}

		if (!canSelectSlot ()) {
			return;
		}

		if (!useInventoryQuickAccessSlots) {
			return;
		}

		if (!changeWeaponsWithKeysActive) {
			return;
		}

		int nextSlotIndex = currentSlotIndex;

		if (state) {
			nextSlotIndex++;

			if (nextSlotIndex > numberQuickAccessSlots - 1) {
				nextSlotIndex = 0;
			}
		} else {
			nextSlotIndex--;

			if (nextSlotIndex < 0) {
				nextSlotIndex = numberQuickAccessSlots - 1;
			}
		}

		bool exit = false;

		int loop = 0;

		while (!exit) {
			for (int k = 0; k < quickAccessSlotInfoListCount; k++) {
				if (currentQuickAccessList [k].slotActive && k == nextSlotIndex) {
					exit = true;
				}
			}

			loop++;

			if (loop > 100) {
				exit = true;
			}

			if (!exit) {
				if (state) {
					//get the current index
					nextSlotIndex++;

					if (nextSlotIndex > numberQuickAccessSlots - 1) {
						nextSlotIndex = 0;
					}
				} else {
					nextSlotIndex--;

					if (nextSlotIndex < 0) {
						nextSlotIndex = numberQuickAccessSlots - 1;
					}
				}
			}
		}

		checkQuickAccessSlotToSelect (nextSlotIndex);
	}

	public void inputNextOrPreviousQuickAccessSlotByMouseWheel (bool state)
	{
		if (!quickAccessInputMouseWheelActive) {
			return;
		}

		inputSelectNextOrPreviousQuickAccessSlot (state);
	}

	public bool checkIfWeaponSharesPocketInWeaponSlots (string weaponName)
	{
		for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [i];

			if (currentSlotInfo.slotActive &&
			    !currentSlotInfo.secondarySlotActive &&
			    currentSlotInfo.inventoryInfoAssigned.isWeapon &&
			    !currentSlotInfo.inventoryInfoAssigned.isMeleeWeapon) {

				string currentObjectName = currentSlotInfo.inventoryInfoAssigned.mainWeaponObjectInfo.getWeaponName ();

				if (showDebugPrint) {
					print (weaponName + " " + currentObjectName);
				}

				if (mainPlayerWeaponsManager.checkIfWeaponIsOnSamePocket (weaponName, currentObjectName)) {
					return true;
				}
			}
		}

		return false;
	}

	public void updateSingleWeaponSlotInfo (string currentRighWeaponName, string currentLeftWeaponName)
	{
		playerWeaponSystem currentRightWeaponSystem = mainPlayerWeaponsManager.getWeaponSystemByName (currentRighWeaponName);
		playerWeaponSystem currentLeftWeaponSystem = mainPlayerWeaponsManager.getWeaponSystemByName (currentLeftWeaponName);

		int currentSlotToDualWeaponsIndex = currentRightWeaponSystem.getWeaponNumberKey () - 1;

		inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotToDualWeapons = currentQuickAccessList [currentSlotToDualWeaponsIndex];

		activatingDualWeaponSlot = false;

		mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

		updateQuickAccessSlotInfo (currentSlotToDualWeaponsIndex, currentSlotToDualWeapons.inventoryInfoAssigned, null, null);

		//get the amount of free slots
		int firstFreeWeaponSlotIndex = -1;

		int numberOfFreeSlots = 0;

		for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
			if (!currentQuickAccessList [i].slotActive) {
				numberOfFreeSlots++;
			}
		}

		if (numberOfFreeSlots > 0) {
			firstFreeWeaponSlotIndex = -1;

			for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
				if (!currentQuickAccessList [i].slotActive) {
					if (firstFreeWeaponSlotIndex == -1) {
						firstFreeWeaponSlotIndex = i;
					}
				}
			}

			inventoryList = mainInventoryManager.inventoryList;

			int inventoryListCount = inventoryList.Count;

			for (int i = 0; i < inventoryListCount; i++) {
				inventoryInfo currentInventoryInfo = inventoryList [i];

				if (currentInventoryInfo.isWeapon && !currentInventoryInfo.isMeleeWeapon) {
					if (currentInventoryInfo.Name.Equals (currentLeftWeaponSystem.getWeaponSystemName ())) {
						currentSlotToMoveInventoryObject = currentInventoryInfo;
					}
				}
			}

			updateQuickAccessSlotInfo (firstFreeWeaponSlotIndex, currentSlotToMoveInventoryObject, null, null);
		} else {
			mainPlayerWeaponsManager.unequipWeapon (currentLeftWeaponSystem.getWeaponSystemName (), false);
		}
	}

	public void updateSingleWeaponSlotInfoWithoutAddingAnotherSlot (string currentRighWeaponName)
	{
		int rightWeaponSlotIndex = -1;

		playerWeaponSystem currentRightWeaponSystem = mainPlayerWeaponsManager.getWeaponSystemByName (currentRighWeaponName);

		int currentSlotToDualWeaponsIndex = currentRightWeaponSystem.getWeaponNumberKey () - 1;

		inventoryList = mainInventoryManager.inventoryList;

		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.isWeapon && !currentInventoryInfo.isMeleeWeapon) {
				if (currentInventoryInfo.Name.Equals (currentRighWeaponName)) {
					currentSlotToMoveInventoryObject = currentInventoryInfo;
				}
			}
		}

		activatingDualWeaponSlot = false;

		mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

		if (showDebugPrint) {
			print ("set single slot for weapon " + currentRighWeaponName + " on slot " + rightWeaponSlotIndex);
		}

		updateQuickAccessSlotInfo (currentSlotToDualWeaponsIndex, currentSlotToMoveInventoryObject, null, null);
	}

	public void updateDualWeaponSlotInfo (string currentRighWeaponName, string currentLeftWeaponName)
	{
		int rightWeaponSlotIndex = -1;
		int leftWeaponSlotIndex = -1;

		playerWeaponSystem currentRightWeaponSystem = mainPlayerWeaponsManager.getWeaponSystemByName (currentRighWeaponName);
		playerWeaponSystem currentLeftWeaponSystem = mainPlayerWeaponsManager.getWeaponSystemByName (currentLeftWeaponName);

		int currentSlotToDualWeaponsIndex = currentRightWeaponSystem.getWeaponNumberKey () - 1;

		inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotToDualWeapons = currentQuickAccessList [currentSlotToDualWeaponsIndex];

		playerWeaponSystem rightWeaponSystemToSetToSingle = mainPlayerWeaponsManager.getWeaponSystemByName (currentSlotToDualWeapons.firstElementName);
		playerWeaponSystem leftWeaponSystemToSetToSingle = mainPlayerWeaponsManager.getWeaponSystemByName (currentSlotToDualWeapons.secondElementName);

		bool rightWeaponIsDualOnOtherSlot = false;
		bool leftWeaponIsDualOnOtherSlot = false;

		bool rightWeaponIsMainWeaponOnCurrentSlotToDualWeapon = false;
		bool leftWeaponIsMainWepaonOnCurrentSLotToDualWeapon = false;

		for (int i = 0; i < quickAccessSlotInfoListCount; i++) {

			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [i];

			if (currentSlotToDualWeaponsIndex != i) {
				//search the right weapon slot of the new right weapon
				if (currentSlotInfo.Name.Equals (currentRighWeaponName)) {
					rightWeaponSlotIndex = i;
				}

				if (currentSlotInfo.firstElementName.Equals (currentRighWeaponName)) {
					rightWeaponSlotIndex = i;

					rightWeaponIsDualOnOtherSlot = true;
					rightWeaponIsMainWeaponOnCurrentSlotToDualWeapon = true;
				}

				if (currentSlotInfo.secondElementName.Equals (currentRighWeaponName)) {
					rightWeaponSlotIndex = i;

					rightWeaponIsDualOnOtherSlot = true;
				}

				//search the left weapon slot of the new left weapon
				if (currentSlotInfo.Name.Equals (currentLeftWeaponName)) {
					leftWeaponSlotIndex = i;
				}

				if (currentSlotInfo.firstElementName.Equals (currentLeftWeaponName)) {
					leftWeaponSlotIndex = i;

					leftWeaponIsDualOnOtherSlot = true;
					leftWeaponIsMainWepaonOnCurrentSLotToDualWeapon = true;
				}

				if (currentSlotInfo.secondElementName.Equals (currentLeftWeaponName)) {
					leftWeaponSlotIndex = i;

					leftWeaponIsDualOnOtherSlot = true;
				}
			}
		}

		int inventoryListCount = inventoryList.Count;

		//remove or add a new slot for the right weapon according to if it was configured as a single weapon in its own slot or dual weapon in a dual slot
		if (rightWeaponSlotIndex != -1) {
			if (rightWeaponIsDualOnOtherSlot) {
				if (showDebugPrint) {
					print ("right weapon is dual on other slot");
				}

				if (rightWeaponIsMainWeaponOnCurrentSlotToDualWeapon) {
					if (showDebugPrint) {
						print ("right weapon is main weapon on other slot");
					}

					//in this case, leave the other weapon from this slot as the main weapon for this slot and remove the current right one from it
					string secondaryWeaponNameOnSlot = currentQuickAccessList [rightWeaponSlotIndex].secondElementName;

					inventoryList = mainInventoryManager.inventoryList;

					inventoryListCount = inventoryList.Count;

					for (int i = 0; i < inventoryListCount; i++) {
						inventoryInfo currentInventoryInfo = inventoryList [i];

						if (currentInventoryInfo.isWeapon && !currentInventoryInfo.isMeleeWeapon) {
							if (currentInventoryInfo.Name.Equals (secondaryWeaponNameOnSlot)) {
								currentSlotToMoveInventoryObject = currentInventoryInfo;
							}
						}
					}

					activatingDualWeaponSlot = false;

					mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

					updateQuickAccessSlotInfo (rightWeaponSlotIndex, currentSlotToMoveInventoryObject, null, null);
				} else {
					if (showDebugPrint) {
						print ("right weapon is secondary weapon on other slot");
					}

					activatingDualWeaponSlot = false;

					mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

					updateQuickAccessSlotInfo (rightWeaponSlotIndex, currentQuickAccessList [rightWeaponSlotIndex].inventoryInfoAssigned, null, null);
				}
			} else {
				updateQuickAccessSlotInfo (-1, null, currentQuickAccessList [rightWeaponSlotIndex], null);
			}
		}

		inventoryListCount = inventoryList.Count;

		//remove or add a new slot for the left weapon according to if it was configured as a single weapon in its own slot or dual weapon in a dual slot
		if (leftWeaponSlotIndex != -1) {
			if (leftWeaponIsDualOnOtherSlot) {
				if (showDebugPrint) {
					print ("left weapon is dual on other slot");
				}

				if (leftWeaponIsMainWepaonOnCurrentSLotToDualWeapon) {
					if (showDebugPrint) {
						print ("left weapon is main weapon on other slot");
					}

					//in this case, leave the other weapon from this slot as the main weapon for this slot and remove the current left one from it
					string secondaryWeaponNameOnSlot = currentQuickAccessList [leftWeaponSlotIndex].secondElementName;

					inventoryList = mainInventoryManager.inventoryList;

					for (int i = 0; i < inventoryListCount; i++) {
						inventoryInfo currentInventoryInfo = inventoryList [i];

						if (currentInventoryInfo.isWeapon && !currentInventoryInfo.isMeleeWeapon) {
							if (currentInventoryInfo.Name.Equals (secondaryWeaponNameOnSlot)) {
								currentSlotToMoveInventoryObject = currentInventoryInfo;
							}
						}
					}

					activatingDualWeaponSlot = false;

					mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

					updateQuickAccessSlotInfo (leftWeaponSlotIndex, currentSlotToMoveInventoryObject, null, null);
				} else {
					if (showDebugPrint) {
						//in this case, the left weapon is configured as a secondary weapon in another slot, so set that slot as single again, removing the current left weapon from that slot
						print ("left weapon is secondary weapon on other slot");
					}

					activatingDualWeaponSlot = false;

					mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

					updateQuickAccessSlotInfo (leftWeaponSlotIndex, currentQuickAccessList [leftWeaponSlotIndex].inventoryInfoAssigned, null, null);
				}
			} else {
				updateQuickAccessSlotInfo (-1, null, currentQuickAccessList [leftWeaponSlotIndex], null);
			}
		}

		inventoryList = mainInventoryManager.inventoryList;

		inventoryListCount = inventoryList.Count;

		// configure the two selected weapons in the same slot
		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.isWeapon && !currentInventoryInfo.isMeleeWeapon) {
				if (currentInventoryInfo.Name.Equals (currentRighWeaponName)) {
					currentSlotToMoveInventoryObject = currentInventoryInfo;
				}
			}
		}

		activatingDualWeaponSlot = true;

		mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

		if (showDebugPrint) {
			print (currentRightWeaponSystem.getWeaponSystemName () + " configured as right weapon and " + currentLeftWeaponSystem.getWeaponSystemName () + " configured as left weapon");
		}

		updateQuickAccessSlotInfo (currentSlotToDualWeaponsIndex, currentSlotToMoveInventoryObject, null, currentLeftWeaponSystem);

		activatingDualWeaponSlot = false;

		mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);


		//get the amount of free slots
		int firstFreeWeaponSlotIndex = -1;

		int numberOfFreeSlots = 0;

		for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
			if (!currentQuickAccessList [i].slotActive) {
				numberOfFreeSlots++;
			}
		}

		inventoryListCount = inventoryList.Count;

		//get the original weapons assigned in the current slot which is used for other couple of weapons
		//if the number of slots available is higher than 1, both weapons can be placed as new slots if there is right and left weapon to assign
		if (numberOfFreeSlots > 1 ||
		    (rightWeaponSystemToSetToSingle != null && leftWeaponSystemToSetToSingle == null) || (rightWeaponSystemToSetToSingle == null && leftWeaponSystemToSetToSingle != null)) {

			//assign the first free slot to the right weapon
			if (numberOfFreeSlots > 1 || (rightWeaponSystemToSetToSingle != null && leftWeaponSystemToSetToSingle == null)) {
				if (rightWeaponSystemToSetToSingle != null) {

					firstFreeWeaponSlotIndex = -1;

					for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
						if (!currentQuickAccessList [i].slotActive) {
							if (firstFreeWeaponSlotIndex == -1) {
								firstFreeWeaponSlotIndex = i;
							}
						}
					}

					string temporalRightWeaponName = rightWeaponSystemToSetToSingle.getWeaponSystemName ();

					if (!temporalRightWeaponName.Equals (currentRighWeaponName) && !temporalRightWeaponName.Equals (currentLeftWeaponName)) {

						inventoryList = mainInventoryManager.inventoryList;

						inventoryListCount = inventoryList.Count;

						for (int i = 0; i < inventoryListCount; i++) {
							inventoryInfo currentInventoryInfo = inventoryList [i];

							if (currentInventoryInfo.isWeapon && !currentInventoryInfo.isMeleeWeapon) {
								if (currentInventoryInfo.Name.Equals (temporalRightWeaponName)) {
									currentSlotToMoveInventoryObject = currentInventoryInfo;
								}
							}
						}

						updateQuickAccessSlotInfo (firstFreeWeaponSlotIndex, currentSlotToMoveInventoryObject, null, null);
					}
				}
			}

			//assign the first free slot to the left weapon
			if (numberOfFreeSlots > 1 || (rightWeaponSystemToSetToSingle == null && leftWeaponSystemToSetToSingle != null)) {
				if (leftWeaponSystemToSetToSingle != null) {

					firstFreeWeaponSlotIndex = -1;

					for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
						if (!currentQuickAccessList [i].slotActive) {
							if (firstFreeWeaponSlotIndex == -1) {
								firstFreeWeaponSlotIndex = i;
							}
						}
					}

					string temporalLeftWeaponName = leftWeaponSystemToSetToSingle.getWeaponSystemName ();

					if (!temporalLeftWeaponName.Equals (currentRighWeaponName) && !temporalLeftWeaponName.Equals (currentLeftWeaponName)) {

						inventoryList = mainInventoryManager.inventoryList;

						inventoryListCount = inventoryList.Count;

						for (int i = 0; i < inventoryListCount; i++) {
							inventoryInfo currentInventoryInfo = inventoryList [i];

							if (currentInventoryInfo.isWeapon && !currentInventoryInfo.isMeleeWeapon) {
								if (currentInventoryInfo.Name.Equals (temporalLeftWeaponName)) {
									currentSlotToMoveInventoryObject = currentInventoryInfo;
								}
							}
						}

						updateQuickAccessSlotInfo (firstFreeWeaponSlotIndex, currentSlotToMoveInventoryObject, null, null);
					}
				}
			}
		} else {
			//else, the number of free slot is only 1, so both weapons need to be combined
			for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
				if (!currentQuickAccessList [i].slotActive) {
					if (firstFreeWeaponSlotIndex == -1) {
						firstFreeWeaponSlotIndex = i;
					}
				}
			}

			if (firstFreeWeaponSlotIndex != -1) {
				activatingDualWeaponSlot = true;

				mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

				inventoryList = mainInventoryManager.inventoryList;

				inventoryListCount = inventoryList.Count;

				for (int i = 0; i < inventoryList.Count; i++) {
					inventoryInfo currentInventoryInfo = inventoryList [i];

					if (currentInventoryInfo.isWeapon && !currentInventoryInfo.isMeleeWeapon) {
						if (currentInventoryInfo.Name.Equals (rightWeaponSystemToSetToSingle.getWeaponSystemName ())) {
							currentSlotToMoveInventoryObject = currentInventoryInfo;
						}
					}
				}

				updateQuickAccessSlotInfo (firstFreeWeaponSlotIndex, currentSlotToMoveInventoryObject, null, leftWeaponSystemToSetToSingle);

				activatingDualWeaponSlot = false;

				mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);
			}
		}
	}

	public void updateQuickAccessSlotInfo (int slotIndex, inventoryInfo currentSlotToMove, inventoryQuickAccessSlotElement.quickAccessSlotInfo slotToUnEquip, playerWeaponSystem secondaryWeaponToEquip)
	{
		bool slotFound = false;

		if (slotIndex > -1 && slotIndex < quickAccessSlotInfoListCount && currentSlotToMove != null) {
			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [slotIndex];
		
			if (currentSlotInfo != null) {
				currentSlotInfo.Name = currentSlotToMove.Name;
				currentSlotInfo.slotActive = true;

				playerWeaponSystem currentPlayerWeaponSystem = null;

				bool isMeleeWeapon = currentSlotToMove.isMeleeWeapon;
				bool isWeapon = currentSlotToMove.isWeapon;

				if (currentSlotToMove.isWeapon && !isMeleeWeapon) {
					currentPlayerWeaponSystem = mainPlayerWeaponsManager.getWeaponSystemByName (currentSlotToMove.mainWeaponObjectInfo.getWeaponName ());
				}

				currentSlotInfo.secondarySlotActive = activatingDualWeaponSlot;

				if (activatingDualWeaponSlot) {
					currentSlotInfo.amountText.text = "";

					currentSlotInfo.rightSecondarySlotIcon.texture = currentPlayerWeaponSystem.getWeaponInventorySlotIcon ();
					currentSlotInfo.leftSecondarySlotIcon.texture = secondaryWeaponToEquip.getWeaponInventorySlotIcon ();

					currentSlotInfo.firstElementName = currentPlayerWeaponSystem.getWeaponSystemName ();
					currentSlotInfo.secondElementName = secondaryWeaponToEquip.getWeaponSystemName ();
				} else {
					if (isWeapon) {
						if (!isMeleeWeapon) {
							currentSlotInfo.amountText.text = currentPlayerWeaponSystem.getCurrentAmmoText ();

							currentSlotInfo.slotIcon.texture = currentPlayerWeaponSystem.getWeaponInventorySlotIcon ();
						}

						if (isMeleeWeapon) {
							currentSlotInfo.amountText.text = "";

							currentSlotInfo.slotIcon.texture = currentSlotToMove.icon;
						}
					} else {
						if (showDebugPrint) {
							print ("updating inventory object slot, like quest, or consumable, etc....");
						}

						if (currentSlotToMove.infiniteAmount) {
							currentSlotInfo.amountText.text = "Inf";
						} else {
							currentSlotInfo.amountText.text = currentSlotToMove.amount.ToString ();
						}

						currentSlotInfo.slotIcon.texture = currentSlotToMove.icon;
					}

					currentSlotInfo.firstElementName = "";
					currentSlotInfo.secondElementName = "";
				}

				bool amountTextContentActive = false;

				if (isWeapon) {
					if (!isMeleeWeapon) {
						amountTextContentActive = currentPlayerWeaponSystem.weaponSettings.weaponUsesAmmo;
					}

					if (isMeleeWeapon) {
						amountTextContentActive = false;
					}
				} else {
					if (currentSlotToMove.isArmorClothAccessory) {
						amountTextContentActive = false;
					} else {
						amountTextContentActive = true;
					}
				}

				if (showSlotAmountEnabled) {
					if (!showSlotAmmoAmountEnabled) {
						if (currentSlotToMove.isWeapon && !currentSlotToMove.isMeleeWeapon) {
							amountTextContentActive = false;
						}
					}
				} else {
					amountTextContentActive = false;
				}
					
				if (currentSlotInfo.amountTextContent.activeSelf != amountTextContentActive) {
					currentSlotInfo.amountTextContent.SetActive (amountTextContentActive);
				}

				if (currentSlotInfo.slotMainSingleContent.activeSelf != !activatingDualWeaponSlot) {
					currentSlotInfo.slotMainSingleContent.SetActive (!activatingDualWeaponSlot);
				}

				if (currentSlotInfo.slotMainDualContent.activeSelf != activatingDualWeaponSlot) {
					currentSlotInfo.slotMainDualContent.SetActive (activatingDualWeaponSlot);
				}

				currentSlotInfo.inventoryInfoAssigned = currentSlotToMove;

				int newNumberKey = slotIndex + 1;

				if (newNumberKey > 9) {
					newNumberKey = 0;
				}

				if (isWeapon) {
					if (!isMeleeWeapon) {
						currentPlayerWeaponSystem.setNumberKey (newNumberKey);
					}
				}

				if (activatingDualWeaponSlot) {
					secondaryWeaponToEquip.setNumberKey (newNumberKey);
				}

				currentSlotToMove.quickAccessSlotIndex = slotIndex;

				slotFound = true;
			}
		}

		if (!slotFound && slotToUnEquip != null) {

			if (activatingDualWeaponSlot) {
				playerWeaponSystem currentPlayerWeaponSystem = mainPlayerWeaponsManager.getWeaponSystemByName (slotToUnEquip.inventoryInfoAssigned.mainWeaponObjectInfo.getWeaponName ());

				slotToUnEquip.amountText.text = currentPlayerWeaponSystem.getCurrentAmmoText ();

				slotToUnEquip.slotIcon.texture = currentPlayerWeaponSystem.getWeaponInventorySlotIcon ();

				if (!slotToUnEquip.slotMainSingleContent.activeSelf) {
					slotToUnEquip.slotMainSingleContent.SetActive (true);
				}

				if (slotToUnEquip.slotMainDualContent.activeSelf) {
					slotToUnEquip.slotMainDualContent.SetActive (false);
				}

				slotToUnEquip.secondarySlotActive = false;

				slotToUnEquip.firstElementName = "";
				slotToUnEquip.secondElementName = "";
			} else {
				slotToUnEquip.Name = "";
				slotToUnEquip.slotActive = false;
				slotToUnEquip.amountText.text = "";
				slotToUnEquip.slotIcon.texture = null;
				slotToUnEquip.inventoryInfoAssigned = null;

				if (slotToUnEquip.currentlySelectedIcon.activeSelf) {
					slotToUnEquip.currentlySelectedIcon.SetActive (false);
				}

				if (slotToUnEquip.slotMainSingleContent.activeSelf) {
					slotToUnEquip.slotMainSingleContent.SetActive (false);
				}

				if (slotToUnEquip.slotMainDualContent.activeSelf) {
					slotToUnEquip.slotMainDualContent.SetActive (false);
				}
			}
		}
	}

	public void updateQuickAccessSlotAmountByName (string objectName)
	{
		int quickAccessSlotIndex = currentQuickAccessList.FindIndex (s => s.Name == objectName);

		if (quickAccessSlotIndex > -1) {
			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [quickAccessSlotIndex];

			if (currentSlotInfo.inventoryInfoAssigned != null) {
				if (currentSlotInfo.inventoryInfoAssigned.amount <= 0) {
					updateQuickAccessSlotInfo (-1, null, currentSlotInfo, null);
				} else {
					updateQuickAccesSlotAmount (quickAccessSlotIndex);
				}
			}
		}
	}

	public void updateQuickAccesSlotAmount (int slotIndex)
	{
		if (quickAccessSlotInfoListCount > slotIndex) {
			if (slotIndex == -1) {
				slotIndex = quickAccessSlotInfoListCount - 1;
			}

			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [slotIndex];

			if (currentSlotInfo.slotActive) {
				if (currentSlotInfo.inventoryInfoAssigned.isWeapon) {
					if (!currentSlotInfo.inventoryInfoAssigned.isMeleeWeapon) {

						currentSlotInfo.amountText.text = currentSlotInfo.inventoryInfoAssigned.mainWeaponObjectInfo.getAmmoText ();
					}
				} else {
					if (currentSlotInfo.inventoryInfoAssigned.infiniteAmount) {
						currentSlotInfo.amountText.text = "Inf";
					} else {
						currentSlotInfo.amountText.text = currentSlotInfo.inventoryInfoAssigned.amount.ToString ();
					}
				}
			}
		}
	}

	public void updateAllQuickAccessSlotsAmount ()
	{
		for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
			updateQuickAccesSlotAmount (i);
		}
	}

	public string getFirstSingleWeaponSlot (string weaponNameToAvoid)
	{
		for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [i];

			if (currentSlotInfo.slotActive &&
			    !currentSlotInfo.inventoryInfoAssigned.isMeleeWeapon &&
			    !currentSlotInfo.secondarySlotActive &&
			    (weaponNameToAvoid.Equals ("") || !weaponNameToAvoid.Equals (currentSlotInfo.Name))) {

				return currentSlotInfo.Name;
			}
		}

		return "";
	}


	public void showQuickAccessSlotsParentWhenSlotSelectedByName (string objectName)
	{
		if (showQuickAccessSlotsWhenChangingSlot) {
			stopShowQuickAccessSlotsParentWhenSlotSelectedCoroutuine ();

			if (showDebugPrint) {
				print ("show " + objectName + " " + mainInventoryManager.inventoryOpened);
			}

			for (int i = 0; i < quickAccessSlotInfoListCount; i++) {
				inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [i];

				if (currentSlotInfo.slotActive && currentSlotInfo.inventoryInfoAssigned.Name.Equals (objectName)) {
					slotsParentCouroutine = StartCoroutine (showQuickAccessSlotsParentWhenSlotSelectedCoroutine (i + 1));
				}
			}
		} 
	}


	public void showQuickAccessSlotsParentWhenSlotSelected (int slotIndex)
	{
		if (showQuickAccessSlotsWhenChangingSlot) {
			stopShowQuickAccessSlotsParentWhenSlotSelectedCoroutuine ();

			if (!mainInventoryManager.inventoryOpened) {
				slotsParentCouroutine = StartCoroutine (showQuickAccessSlotsParentWhenSlotSelectedCoroutine (slotIndex));
			}
		} 
	}

	public void stopShowQuickAccessSlotsParentWhenSlotSelectedCoroutuine ()
	{
		if (slotsParentCouroutine != null) {
			StopCoroutine (slotsParentCouroutine);
		}
	}

	IEnumerator showQuickAccessSlotsParentWhenSlotSelectedCoroutine (int slotIndex)
	{
		if (slotIndex >= 0 && quickAccessSlotInfoListCount >= slotIndex) {
			yield return new WaitForSeconds (0.001f);

			if (!mainInventoryManager.inventoryOpened) {
				moveQuickAccessSlotsOutOfInventory ();
			
				if (showQuickAccessSlotSelectedIcon) {
					if (!quickAccessSlotSelectedIcon.activeSelf) {
						quickAccessSlotSelectedIcon.SetActive (true);
					}

					quickAccessSlotSelectedIcon.transform.localScale = Vector3.one * quickAccessSlotsParentScale;
				}
			}

			if (slotIndex == 0) {
				slotIndex = quickAccessSlotInfoListCount;
			}

			updateCurrentlySelectedSlotIcon (slotIndex, true);

			updateAllQuickAccessSlotsAmount ();

			yield return new WaitForSeconds (0.001f);

			quickAccessSlotSelectedIcon.transform.position = currentQuickAccessList [slotIndex - 1].slotSelectedIconPosition.position;

			yield return new WaitForSeconds (showQuickAccessSlotsParentDuration);

			if (!showQuickAccessSlotsAlways) {
				moveQuickAccessSlotsToInventory ();
			}

			if (quickAccessSlotSelectedIcon.activeSelf) {
				quickAccessSlotSelectedIcon.SetActive (false);
			}
		} else {
			if (showDebugPrint) {
				print ("WARNING: weapon slot index not found when trying to show the top icon over the weapon selected in the weapon slots");
			}
		}
	}

	public bool isShowQuickAccessSlotsAlwaysActive ()
	{
		return showQuickAccessSlotsAlways;
	}

	public void updateCurrentlySelectedSlotIcon (int slotIndex, bool activeCurrentQuickAccessSlot)
	{
		bool weaponsAvailable = mainPlayerWeaponsManager.checkIfWeaponsAvailable ();

		bool anySlotActive = false;

		for (int j = 0; j < quickAccessSlotInfoListCount; j++) {
			if (!anySlotActive && currentQuickAccessList [j].slotActive) {
				anySlotActive = true;
			}
		}

		for (int j = 0; j < quickAccessSlotInfoListCount; j++) {
			
			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [j];

			bool currentIconActive = false;

			if (activeCurrentQuickAccessSlot && (weaponsAvailable || anySlotActive)) {
				if (j == slotIndex - 1) {
					currentIconActive = true;

					currentSlotIndex = j;
				} else {
					currentIconActive = false;
				}
			} else {
				currentIconActive = false;
			}

			if (currentSlotInfo.currentlySelectedIcon.activeSelf != currentIconActive) {
				currentSlotInfo.currentlySelectedIcon.SetActive (currentIconActive);
			}
		}
	}

	public void checkModeToUpdateWeaponCurrentlySelectedIcon ()
	{
		if (mainPlayerWeaponsManager.isWeaponsModeActive ()) {
			updateCurrentlySelectedSlotIcon (mainPlayerWeaponsManager.chooseDualWeaponIndex, true);
		} else if (mainMeleeWeaponsGrabbedManager.isMeleeWeaponsGrabbedManagerActive ()) {
			string weaponName = mainMeleeWeaponsGrabbedManager.getCurrentWeaponName ();

			for (int j = 0; j < quickAccessSlotInfoListCount; j++) {
				inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [j];

				if (currentSlotInfo.slotActive && currentSlotInfo.Name.Equals (weaponName)) {
				
					updateCurrentlySelectedSlotIcon (j + 1, true);

					return;
				}
			}
		} else {
			if (currentSlotIndex != -1) {
				updateCurrentlySelectedSlotIcon (currentSlotIndex + 1, true);
			} 
		}
	}

	public void disableCurrentlySelectedIcon ()
	{
		for (int j = 0; j < quickAccessSlotInfoListCount; j++) {
			if (currentQuickAccessList [j].currentlySelectedIcon.activeSelf) {
				currentQuickAccessList [j].currentlySelectedIcon.SetActive (false);
			}
		}
	}

	bool canUseInput ()
	{
		if (mainInventoryManager.playerIsBusy ()) {
			return false;
		}

		if (mainInventoryManager.isGamePaused ()) {
			return false;
		}

		if (mainPlayerWeaponsManager.isActionActiveInPlayer ()) {
			return false;
		}

		if (!mainInventoryManager.canUseInput ()) {
			return false;
		}

		return true;
	}

	public void selectQuickAccessSlotByPressingSlot (GameObject buttonToCheck)
	{
		if (!canUseInput ()) {
			return;
		}

		if (!canSelectSlot ()) {
			return;
		}

		if (mainInventoryManager.inventoryOpened) {
			return;
		}

		for (int j = 0; j < quickAccessSlotInfoListCount; j++) {
			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [j];

			if (currentSlotInfo.slot == buttonToCheck) {
				checkQuickAccessSlotToSelect (j);
			}
		}
	}

	public void moveQuickAccessSlotsOutOfInventory ()
	{
		inventoryQuickAccessSlots.transform.SetParent (quickAccessSlotsParentOutOfInventory);
		inventoryQuickAccessSlots.transform.localScale = Vector3.one * quickAccessSlotsParentScale;
		inventoryQuickAccessSlots.transform.localPosition = Vector3.zero;

		setQuickAccessSlotsBackgroundColorAlphaValue (quickAccessSlotsAlphaValueOutOfInventory);
	}

	public void moveQuickAccessSlotsToInventory ()
	{
		inventoryQuickAccessSlots.transform.SetParent (quickAccessSlotsParentOnInventory);

		inventoryQuickAccessSlots.transform.localScale = Vector3.one;
		inventoryQuickAccessSlots.transform.localPosition = Vector3.zero;

		if (quickAccessSlotSelectedIcon.activeSelf) {
			quickAccessSlotSelectedIcon.SetActive (false);
		}

		setQuickAccessSlotsBackgroundColorAlphaValue (1);
	}

	public void setQuickAccessSlotsBackgroundColorAlphaValue (float newAlphaValue)
	{
		if (!setQuickAccessSlotsAlphaValueOutOfInventory) {
			return;
		}

		for (int j = 0; j < quickAccessSlotInfoListCount; j++) {
			inventoryQuickAccessSlotElement.quickAccessSlotInfo currentSlotInfo = currentQuickAccessList [j];

			Color currentColor = currentSlotInfo.backgroundImage.color;

			currentColor.a = newAlphaValue;

			currentSlotInfo.backgroundImage.color = currentColor;
		}
	}

	public void enableOrDisableHUD (bool state)
	{
		enableOrDisableQuickAccessSlotsParentOutOfInventory (state);
	}

	public void enableOrDisableQuickAccessSlotsParentOutOfInventory (bool state)
	{
		if (showQuickAccessSlotsPaused) {
			return;
		}

		setQuickAccessSlotsParentOutOfInventoryActiveState (state);
	}

	public void checkToEnableOrDisableQuickAccessSlotsParentOutOfInventory (bool state)
	{
		if (showQuickAccessSlotsPaused) {
			return;
		}

		if (showQuickAccessSlotsAlways) {
			setQuickAccessSlotsParentOutOfInventoryActiveState (state);
		}
	}

	public void checkToEnableOrDisableQuickAccessSlotsParentOutOfInventoryFromFireWeaponsMode (bool state)
	{
		if (showQuickAccessSlotsPaused) {
			return;
		}

		if (!disableQuickAccessSlotsWhenChangingFromFireWeaponsMode && !state) {
			return;
		}

		if (showQuickAccessSlotsAlways) {
			setQuickAccessSlotsParentOutOfInventoryActiveState (state);
		}
	}

	public void checkToEnableOrDisableQuickAccessSlotsParentOutOfInventoryFromMeleeWeaponsMode (bool state)
	{
		if (showQuickAccessSlotsPaused) {
			return;
		}

		if (!disableQuickAccessSlotsWhenChangingFromMeleeWeaponsMode && !state) {
			return;
		}

		if (showQuickAccessSlotsAlways) {
			setQuickAccessSlotsParentOutOfInventoryActiveState (state);
		}
	}

	void setQuickAccessSlotsParentOutOfInventoryActiveState (bool state)
	{
		if (quickAccessSlotsParentOutOfInventory.gameObject.activeSelf != state) {
			quickAccessSlotsParentOutOfInventory.gameObject.SetActive (state);
		}
	}

	public void setShowQuickAccessSlotsPausedState (bool state)
	{
		showQuickAccessSlotsPaused = state;
	}

	public void resetDragAndDropSlotState ()
	{
		activatingDualWeaponSlot = false;

		mainInventoryManager.setActivatingDualWeaponSlotState (activatingDualWeaponSlot);

		if (quickAccessSlotToMove.activeSelf) {
			quickAccessSlotToMove.SetActive (false);
		}

		slotFoundOnDrop = null;

		draggedFromInventoryList = false;

		draggedFromQuickAccessSlots = false;

		slotToMoveFound = false;
	}

	public void setOpenOrCloseInventoryMenuState (bool state)
	{
		inventoryOpened = state;

		resetDragAndDropSlotState ();

		touching = false;
	}

	public void setCustomizingCharacterActiveState (bool state)
	{
		customizingCharacterActive = state;

		if (customizingCharacterActive) {
			currentQuickAccessList = characterCustomizationSlotList;
		} else {
			currentQuickAccessList = quickAccessSlotInfoList;
		}

		quickAccessSlotInfoListCount = currentQuickAccessList.Count;
	}

	public bool isCustomizingCharacterActive ()
	{
		return customizingCharacterActive;
	}

	public List<inventoryQuickAccessSlotElement.quickAccessSlotInfo> getCurrentQuickAccessList ()
	{
		return currentQuickAccessList;
	}

	public void setCheckObjectCategoriesToUseActiveState (bool state)
	{
		checkObjectCategoriesToUseActive = state;
	}

	public void setCurrentCategoryListToUseActive (List<string> newList)
	{
		currentCategoryListToUseActive = newList;
	}

	public void setCheckSlotsCategoryNameToDropActive (bool state)
	{
		checkSlotsCategoryNameToDrop = state;
	}
}
