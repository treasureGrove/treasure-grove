using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class useInventoryObject : MonoBehaviour
{
	public bool canBeReUsed;
	public useInventoryObjectType useInventoryType;

	public bool useObjectsOneByOneUsingButton = true;

	public string inventoryObjectAction;

	[TextArea (3, 10)]
	public string objectUsedMessage;

	public bool useCustomObjectNotFoundMessage;
	[TextArea (3, 10)]
	public string objectNotFoundMessage;

	public bool enableObjectWhenActivate;
	public GameObject objectToEnable;
	public bool useAnimation;
	public GameObject objectWithAnimation;
	public string animationName;

	public List<inventoryObjectNeededInfo> inventoryObjectNeededList = new List<inventoryObjectNeededInfo> ();

	public UnityEvent unlockFunctionCall = new UnityEvent ();

	public bool useUnlockEventFromLoadingGame;
	public UnityEvent unlockEventFromLoadingGame;

	public bool disableObjectActionAfterUse;

	public bool objectUsed;

	public int numberOfObjectsUsed;
	public int numberOfObjectsNeeded;

	public int currentNumberOfObjectsNeeded;

	public enum useInventoryObjectType
	{
		menu,
		button,
		automatic,
		inventoryOnSide
	}

	public bool checkPlayerInventoryWhileDriving;

	public string tagToConfigure = "device";

	public string playerTag = "Player";
	public string vehicleTag = "vehicle";

	public bool canUseAndUseInventoryPickupsOnTriggerEnabled = true;

	public bool removeInventoryPickupObjectOnTrigger = true;

	public bool openPlayerInventoryMenu;
	public bool setNewInventoryPanel;
	public string defaultInventoryPanelName = "Default";
	public string newInventoryPanelName = "Inventory On Side";

	public bool playerInventoryMenuOpened;

	public bool useEventOnUnlockWithDelay;
	public UnityEvent eventOnUnlockWithDelay;
	public float delayToActivateEvent;

	public deviceStringAction deviceStringActionManager;
	public AudioSource mainAudioSource;
	public Collider mainCollider;


	public bool showDebugInfo;

	GameObject currentPlayer;
	inventoryManager playerInventoryManager;
	usingDevicesSystem usingDevicesManager;

	string previousAction;
	string currentObjectUsedMessage;


	GameObject currentObjectToUse;
	string currentObjectToUseName;

	bool canBeUsed = true;

	int currentAmountUsed;

	Coroutine eventWithDelayCoroutine;



	void Start ()
	{
		if (deviceStringActionManager == null) {
			deviceStringActionManager = GetComponent<deviceStringAction> ();
		}

		if (deviceStringActionManager != null) {
			previousAction = deviceStringActionManager.deviceAction;
		}

		for (int i = 0; i < inventoryObjectNeededList.Count; i++) {
			numberOfObjectsNeeded += inventoryObjectNeededList [i].amountNeeded;
		}	

		if (mainAudioSource == null) {
			mainAudioSource = GetComponent<AudioSource> ();
		}

		if (mainCollider == null) {
			mainCollider = GetComponent<Collider> ();
		}

		if (useInventoryType == useInventoryObjectType.button) {
			gameObject.tag = tagToConfigure;
		}
	}

	public void setCanBeUsedState (bool state)
	{
		canBeUsed = state;
	}

	public bool getInventoryObjectCanBeUsed ()
	{
		return canBeUsed;
	}

	public void activateDevice ()
	{
		if (useInventoryType == useInventoryObjectType.button) {
			if (!objectUsed) {
				if (showDebugInfo) {
					print ("activating use of inventory object by using devices interaction");
				}

				playerInventoryManager.useCurrentObject ();
			}
		}
	}

	//When player uses an inventory object, this can happen:
	//-The option is menu
	//--Only one object can be used at the same time
	//-The option is button
	//--The player has all the objects needed
	//--The player has some objects needed
	//-The option is automatic
	//--The player has all the objects needed
	//---All inventory objects are used
	//--The player has some objects needed
	//---Those available objects are used

	public void useObject (int amountUsed, string inventoryObjectName, GameObject inventoryInfoGameObject)
	{
		if (!objectUsed) {
			inventoryObjectNeededInfo currentObjectNeededInfo = null;
			inventoryElementNeededInfo currentElementNeededInfo = null;

			for (int i = 0; i < inventoryObjectNeededList.Count; i++) {

				currentObjectNeededInfo = inventoryObjectNeededList [i];

				if (!currentObjectNeededInfo.objectUsed) {
					bool objectFound = false;

					GameObject currentObjectNeeded = currentObjectNeededInfo.objectNeeded;
					string currentObjectNeededName = currentObjectNeededInfo.objectNeededName;

					if (currentObjectNeededInfo.useObjectNeededName) {
						if (inventoryObjectName.Equals (currentObjectNeededName)) {
							objectFound = true;
						}
					} else {
						if (inventoryInfoGameObject.Equals (currentObjectNeeded)) {
							objectFound = true;
						}
					}

					if (objectFound) {
	
						currentObjectNeededInfo.amountOfObjectsUsed += amountUsed;

						if (currentObjectNeededInfo.amountOfObjectsUsed == currentObjectNeededInfo.amountNeeded) {
							currentObjectNeededInfo.objectUsed = true;

							if (currentObjectNeededInfo.useEventOnObjectsPlaced) {
								currentObjectNeededInfo.eventOnObjectsPlaced.Invoke ();
							}
						}

						currentAmountUsed = amountUsed;

						numberOfObjectsUsed += amountUsed;

						for (int j = 0; j < currentObjectNeededInfo.inventoryObjectNeededList.Count; j++) {
							if (j < currentObjectNeededInfo.amountOfObjectsUsed) {

								currentElementNeededInfo = currentObjectNeededInfo.inventoryObjectNeededList [j];

								if (!currentElementNeededInfo.objectActivated) {
									currentElementNeededInfo.objectActivated = true;

									if (currentElementNeededInfo.instantiateObject) {
										if (currentObjectNeeded != null) {
											Instantiate (currentObjectNeeded, 
												currentElementNeededInfo.placeForObject.position, 
												currentElementNeededInfo.placeForObject.rotation);
										}
									} else if (currentElementNeededInfo.enableObject) {
										currentElementNeededInfo.objectToEnable.SetActive (true);
									}

									if (currentElementNeededInfo.useEventOnObjectPlaced) {
										currentElementNeededInfo.eventOnObjectPlaced.Invoke ();
									}

									if (currentElementNeededInfo.useAnimation) {
										Animation currentAnimation = currentElementNeededInfo.objectWithAnimation.GetComponent<Animation> ();

										if (currentAnimation != null) {
											currentAnimation.Play (currentElementNeededInfo.animationName);
										}
									}
								}
							}
						}

						currentNumberOfObjectsNeeded = currentObjectNeededInfo.amountNeeded - currentObjectNeededInfo.amountOfObjectsUsed;

						currentObjectUsedMessage = currentObjectNeededInfo.objectUsedMessage;

						if (currentObjectNeededInfo.useObjectSound) {
							playObjectUsedSound (i);
						}
					}
				}
			}
				
			if (numberOfObjectsUsed >= numberOfObjectsNeeded) {
				solveThisInventoryObject ();
			} 
		}
	}

	void solveObject (bool solveFromLoadingGame)
	{
		currentObjectUsedMessage = currentObjectUsedMessage + "\n" + objectUsedMessage;

		bool useRegularUnlockFunctionCall = true;

		if (solveFromLoadingGame) {
			unlockEventFromLoadingGame.Invoke ();

			useRegularUnlockFunctionCall = false;
		} 

		if (useRegularUnlockFunctionCall) {
			if (unlockFunctionCall.GetPersistentEventCount () > 0) {
				unlockFunctionCall.Invoke ();
			}
		}

		if (deviceStringActionManager != null) {
			if (disableObjectActionAfterUse) {
				deviceStringActionManager.showIcon = false;
				removeDeviceFromList ();

				mainCollider.enabled = false;
			} else {
				deviceStringActionManager.setDeviceAction (previousAction);
			}
		}

		if (useAnimation) {
			Animation currentAnimation = objectWithAnimation.GetComponent<Animation> ();

			if (currentAnimation != null) {
				currentAnimation.Play (animationName);
			}
		}

		if (!canBeReUsed) {
			objectUsed = true;
		}

		if (enableObjectWhenActivate) {
			objectToEnable.SetActive (true);
		}

		if (useEventOnUnlockWithDelay) {
			activateEventOnUnlockWithDelay ();
		}

		if (canBeReUsed) {
			for (int i = 0; i < inventoryObjectNeededList.Count; i++) {

				inventoryObjectNeededInfo currentObjectNeededInfo = inventoryObjectNeededList [i];

				currentObjectNeededInfo.objectUsed = false;
				currentObjectNeededInfo.amountOfObjectsUsed = 0;
			}

			numberOfObjectsUsed = 0;
		}
	}

	public void solveThisInventoryObject ()
	{
		solveObject (false);
	}

	public void solveThisInventoryObjectFromLoadingGame ()
	{
		solveObject (useUnlockEventFromLoadingGame);
	}

	public void activateEventOnUnlockWithDelay ()
	{
		if (eventWithDelayCoroutine != null) {
			StopCoroutine (eventWithDelayCoroutine);
		}

		eventWithDelayCoroutine = StartCoroutine (activateEventOnUnlockWithDelayCoroutine ());
	}

	IEnumerator activateEventOnUnlockWithDelayCoroutine ()
	{
		yield return new WaitForSecondsRealtime (delayToActivateEvent);
	
		eventOnUnlockWithDelay.Invoke ();
	}

	public void updateUseInventoryObjectState ()
	{
		if (canBeReUsed) {
			if (canBeUsed) {
				selectObjectOnInventory ();
			} else {
				removePlayerInventoryInfo ();
			}
		} else {
			if (!objectUsed) {
				if (useInventoryType == useInventoryObjectType.button) {
					
					selectObjectOnInventory ();

					if (!useObjectsOneByOneUsingButton) {
						if (currentObjectToUse != null || currentObjectToUseName != "") {
							playerInventoryManager.useCurrentObject ();
						}
					}
				} else if (useInventoryType == useInventoryObjectType.menu) {
					setInfoCurrentInventoryObjectToUse ();
				} else if (useInventoryType == useInventoryObjectType.automatic) {
					selectObjectOnInventory ();

//					if (currentObjectToUse != null || currentObjectToUseName != "") {
//						playerInventoryManager.useCurrentObject ();
//					}
				} else if (useInventoryType == useInventoryObjectType.inventoryOnSide) {
					setInfoCurrentInventoryObjectToUse ();
				}
			}
		}
	}

	public bool setToNullCurrentInventoryObject ()
	{
		return useInventoryType != useInventoryObjectType.inventoryOnSide;
	}

	//set the object needed to be used from the current player inside the trigger of this use inventory object
	public void selectObjectOnInventory ()
	{
		counter++;

		if (counter > 100) {
			print ("ERROR");

			return;
		}

		if (deviceStringActionManager != null) {
			if (inventoryObjectAction != "") {
				deviceStringActionManager.setDeviceAction (inventoryObjectAction);
			}
		}
			
		if (inventoryPickupObjectDetected) {
			useObjectAutomatically ();

			inventoryPickupObjectDetected = false;

			return;
		} else {
			setInfoCurrentInventoryObjectToUse ();
		}

		//check the options button and automatic to use correctly the inventory objects
		if (useInventoryType == useInventoryObjectType.button) {
			useObjectByButton ();
		} else if (useInventoryType == useInventoryObjectType.menu) {
			useObjectByMenu ();
		} else if (useInventoryType == useInventoryObjectType.automatic) {
			useObjectAutomatically ();
		} else if (useInventoryType == useInventoryObjectType.inventoryOnSide) {
			useObjectByMenu ();
		}
	}

	public void useObjectByButton ()
	{
		playerInventoryManager.setCurrenObjectByPrefab (currentObjectToUse, currentObjectToUseName);

		playerInventoryManager.setCurrentUseInventoryGameObject (gameObject);

		if (showDebugInfo) {
			print ("using object by button");
		}
	}

	public void useObjectByMenu ()
	{
		playerInventoryManager.setCurrentUseInventoryGameObject (gameObject);

		if (showDebugInfo) {
			print ("using object by menu");
		}
	}

	public void useObjectAutomatically ()
	{
		if (inventoryPickupObjectDetected) {
			playerInventoryManager.setCurrenInventoryInfoByPickup (currentInventoryInfoPickupDetected);
		} else {
//			print ("USE OBJECT " + currentObjectToUseName);

			playerInventoryManager.setCurrenObjectByPrefab (currentObjectToUse, currentObjectToUseName);
		}

		playerInventoryManager.searchForObjectNeed (gameObject);

		if (showDebugInfo) {
			print ("using object automatically");
		}
	}

	public int counter = 0;

	public void setInfoCurrentInventoryObjectToUse ()
	{
		inventoryObjectNeededInfo currentObjectNeededInfo = null;

		for (int i = 0; i < inventoryObjectNeededList.Count; i++) {
			currentObjectNeededInfo = inventoryObjectNeededList [i];

			if (!currentObjectNeededInfo.objectUsed) {
				bool objectFound = false;
			
				if (currentObjectNeededInfo.useObjectNeededName) {
					if (playerInventoryManager.existInPlayerInventoryFromName (currentObjectNeededInfo.objectNeededName)) {
						currentObjectToUseName = currentObjectNeededInfo.objectNeededName;

						objectFound = true;

						if (showDebugInfo) {
							print ("first object detected to use " + currentObjectToUseName);
						}
					}
				} else {
					if (playerInventoryManager.inventoryContainsObject (currentObjectNeededInfo.objectNeeded)) {
						currentObjectToUse = currentObjectNeededInfo.objectNeeded;

						currentObjectToUseName = "";

						objectFound = true;

						if (showDebugInfo) {
							print ("first object detected to use " + currentObjectToUse);
						}
					}
				}

				if (objectFound) {
					currentNumberOfObjectsNeeded = currentObjectNeededInfo.amountNeeded - currentObjectNeededInfo.amountOfObjectsUsed;

					if (currentObjectNeededInfo.useObjectAction) {
						if (deviceStringActionManager != null) {
							
							deviceStringActionManager.setDeviceAction (currentObjectNeededInfo.objectAction + " x " + currentNumberOfObjectsNeeded);

							if (usingDevicesManager != null) {
								usingDevicesManager.checkDeviceName ();
							}
						}
					}

					if (showDebugInfo) {
						if (currentObjectToUse != null) {
							print ("Object to use found " + currentObjectToUse.name);
						} else {
							if (currentObjectToUseName != "") {
								print ("Object to use found " + currentObjectToUseName);
							}
						}
					}

					return;
				} else {
					currentObjectToUse = null;

					currentObjectToUseName = "";
				}
			}
		}

		if (showDebugInfo) {
			print ("Object to use not found");
		}
	}

	public string getObjectUsedMessage ()
	{
		return currentObjectUsedMessage;
	}

	public string getCustomObjectNotFoundMessage ()
	{
		return objectNotFoundMessage;
	}

	public bool isCustomObjectNotFoundMessageActive ()
	{
		return useCustomObjectNotFoundMessage;
	}

	public bool isInventoryPlaceUsed ()
	{
		return objectUsed;
	}

	public bool inventoryObjectNeededListContainsObject (GameObject objectToCheck, string inventoryObjectName)
	{
		inventoryObjectNeededInfo currentObjectNeededInfo = null;

		for (int i = 0; i < inventoryObjectNeededList.Count; i++) {

			currentObjectNeededInfo = inventoryObjectNeededList [i];

			if (!currentObjectNeededInfo.objectUsed) {
				if (currentObjectNeededInfo.useObjectNeededName) {
					if (currentObjectNeededInfo.objectNeededName.Equals (inventoryObjectName)) {
						currentObjectToUseName = currentObjectNeededInfo.objectNeededName;

						return true;
					}
				} else {
					if (currentObjectNeededInfo.objectNeeded == objectToCheck) {
						currentObjectToUse = currentObjectNeededInfo.objectNeeded;

						currentObjectToUseName = "";

						return true;
					}
				}
			}
		}
		
		return false;
	}

	public int getInventoryObjectNeededAmount (GameObject objectToCheck, string inventoryObjectName)
	{
		if (canBeReUsed) {
			return 1;
		}

		inventoryObjectNeededInfo currentObjectNeededInfo = null;

		for (int i = 0; i < inventoryObjectNeededList.Count; i++) {

			currentObjectNeededInfo = inventoryObjectNeededList [i];

			if (!currentObjectNeededInfo.objectUsed) {
				if (currentObjectNeededInfo.useObjectNeededName) {
					if (currentObjectNeededInfo.objectNeededName.Equals (inventoryObjectName)) {
						currentNumberOfObjectsNeeded = currentObjectNeededInfo.amountNeeded - currentObjectNeededInfo.amountOfObjectsUsed;

						return currentNumberOfObjectsNeeded;
					}
				} else {
					if (currentObjectNeededInfo.objectNeeded == objectToCheck) {
						currentNumberOfObjectsNeeded = currentObjectNeededInfo.amountNeeded - currentObjectNeededInfo.amountOfObjectsUsed;

						return currentNumberOfObjectsNeeded;
					}
				}
			}
		}

		return -1;
	}

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	bool inventoryPickupObjectDetected;

	inventoryInfo currentInventoryInfoPickupDetected;

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (isEnter) {
			if (!objectUsed) {
				bool playerDetected = false;

				if (col.CompareTag (playerTag)) {
					setCurrentPlayer (col.gameObject);

					selectObjectOnInventory ();

					playerDetected = true;
				}

				if (checkPlayerInventoryWhileDriving && col.CompareTag (vehicleTag)) {

					GameObject currentDriver = applyDamage.getVehicleDriver (col.gameObject);

					if (currentDriver != null) {
						setCurrentPlayer (currentDriver);

						selectObjectOnInventory ();

						playerDetected = true;
					}
				}

				if (canUseAndUseInventoryPickupsOnTriggerEnabled && !playerDetected) {
					pickUpObject currentPickupObject = col.gameObject.GetComponent<pickUpObject> ();

					if (currentPickupObject != null) {
						if (currentPickupObject.inventoryObjectManager != null) {

							if (checkIfInventoryPickupIsUsed (currentPickupObject.inventoryObjectManager)) {
							
								inventoryPickupObjectDetected = true;

								currentInventoryInfoPickupDetected = currentPickupObject.inventoryObjectManager.inventoryObjectInfo;

								if (currentPlayer == null) {
									GameObject playerOnScene = GKC_Utils.findMainPlayerOnScene ();

									setCurrentPlayer (playerOnScene);
								}

								selectObjectOnInventory ();

								if (removeInventoryPickupObjectOnTrigger) {
									Destroy (col.gameObject);
								}
							}
						}
					}
				}
			}
		} else {
			if (!objectUsed) {
				if (col.CompareTag (playerTag)) {
					removePlayerInventoryInfo ();

					removeCurrentPlayer ();
				}

				if (checkPlayerInventoryWhileDriving && col.CompareTag (vehicleTag)) {

					GameObject currentDriver = applyDamage.getVehicleDriver (col.gameObject);

					if (currentDriver != null) {
						removePlayerInventoryInfo ();

						removeCurrentPlayer ();
					}
				}
			}
		}
	}



	public bool checkIfInventoryPickupIsUsed (inventoryObject inventoryObjectToCheck)
	{
		bool objectFound = false;

		for (int i = 0; i < inventoryObjectNeededList.Count; i++) {
			if (!objectFound) {
				inventoryObjectNeededInfo currentObjectNeededInfo = inventoryObjectNeededList [i];

				if (!currentObjectNeededInfo.objectUsed) {
					if (currentObjectNeededInfo.useObjectNeededName) {
						if (inventoryObjectToCheck.inventoryObjectInfo.Name.Equals (currentObjectNeededInfo.objectNeededName)) {
							currentObjectToUseName = currentObjectNeededInfo.objectNeededName;

							objectFound = true;

							if (showDebugInfo) {
								print ("first object detected to use " + currentObjectToUseName);
							}
						}
					} else {
						if (inventoryObjectToCheck.inventoryObjectInfo.inventoryGameObject == currentObjectNeededInfo.objectNeeded) {
							currentObjectToUse = currentObjectNeededInfo.objectNeeded;

							currentObjectToUseName = "";

							objectFound = true;

							if (showDebugInfo) {
								print ("first object detected to use " + currentObjectToUse);
							}
						}
					}

					if (objectFound) {
						currentNumberOfObjectsNeeded = currentObjectNeededInfo.amountNeeded - currentObjectNeededInfo.amountOfObjectsUsed;

						if (currentObjectNeededInfo.useObjectAction) {
							if (deviceStringActionManager != null) {

								deviceStringActionManager.setDeviceAction (currentObjectNeededInfo.objectAction + " x " + currentNumberOfObjectsNeeded);

								if (usingDevicesManager != null) {
									usingDevicesManager.checkDeviceName ();
								}
							}
						}

						if (showDebugInfo) {
							if (currentObjectToUse != null) {
								print ("Object to use found " + currentObjectToUse.name);
							} else {
								if (currentObjectToUseName != "") {
									print ("Object to use found " + currentObjectToUseName);
								}
							}
						}
					} else {
						currentObjectToUse = null;

						currentObjectToUseName = "";
					}
				}
			}
		}

		if (!objectFound) {
			if (showDebugInfo) {
				print ("Object to use not found");
			}
		} 

		return objectFound;
	}

	public void removePlayerInventoryInfo ()
	{
		if (playerInventoryManager != null) {
			playerInventoryManager.setCurrentUseInventoryGameObject (null);

			playerInventoryManager.removeCurrentInventoryObject ();
		}
	}

	public void playObjectUsedSound (int index)
	{
		if (mainAudioSource != null) {
			AudioClip currentAudioClip = inventoryObjectNeededList [index].usedObjectSound;

			if (currentAudioClip != null) {
				mainAudioSource.PlayOneShot (currentAudioClip);
			}
		}
	}

	public void setCurrentPlayer (GameObject player)
	{
		currentPlayer = player;

		if (currentPlayer != null) {
			playerInventoryManager = currentPlayer.GetComponent<inventoryManager> ();
			usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
		}
	}

	public void removeCurrentPlayer ()
	{
		currentPlayer = null;

		playerInventoryManager = null;
	}

	public void removeDeviceFromList ()
	{
		if (usingDevicesManager != null) {
			usingDevicesManager.removeDeviceFromListExternalCall (gameObject);
		}
	}

	public int getCurrentAmountUsed ()
	{
		return currentAmountUsed;
	}

	public void enableOrDisableTrigger (bool state)
	{
		if (mainCollider == null) {
			mainCollider = GetComponent<Collider> ();
		}

		if (mainCollider != null) {
			if (mainCollider.enabled != state) {
				canBeUsed = state;

				mainCollider.enabled = state;

				if (!state) {
					removeDeviceFromList ();
				}
			}
		}
	}

	public void openOrClosePlayerInventoryMenu ()
	{
		if (openPlayerInventoryMenu) {
			if (playerInventoryManager != null) {
				playerInventoryMenuOpened = !playerInventoryMenuOpened;

				if (setNewInventoryPanel) {
					if (playerInventoryMenuOpened) {
						playerInventoryManager.setInventoryPanelByName (newInventoryPanelName);
					} else {
						playerInventoryManager.setInventoryPanelByName (defaultInventoryPanelName);
					}
				}
			
				playerInventoryManager.openOrClosePlayerMenuFromUseInventoryObject (playerInventoryMenuOpened);
			}
		}
	}

	public void disableInventoryMenu ()
	{
		if (playerInventoryManager != null) {
			playerInventoryManager.enableOrDisableInventoryMenu (false);
		}
	}

	//EDITOR FUNCTIONS
	public void addInventoryObjectNeededInfo ()
	{
		inventoryObjectNeededInfo newInventoryObjectNeededInfo = new inventoryObjectNeededInfo ();

		newInventoryObjectNeededInfo.Name = "New Object";

		inventoryObjectNeededList.Add (newInventoryObjectNeededInfo);

		updateComponent ();
	}

	public void addSubInventoryObjectNeededList (int index)
	{
		inventoryElementNeededInfo newInventoryElementNeededInfo = new inventoryElementNeededInfo ();

		inventoryObjectNeededList [index].inventoryObjectNeededList.Add (newInventoryElementNeededInfo);

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Use Inventory Object Values " + gameObject.name, gameObject);
	}

	[System.Serializable]
	public class inventoryObjectNeededInfo
	{
		public string Name;
		public GameObject objectNeeded;

		public bool useObjectNeededName;
		public string objectNeededName;

		public bool useObjectAction;
		public string objectAction;
		public int amountNeeded;
		public bool objectUsed;
		public int amountOfObjectsUsed;
		[TextArea (3, 10)]
		public string objectUsedMessage;

		public bool useEventOnObjectsPlaced;
		public UnityEvent eventOnObjectsPlaced;

		public bool useObjectSound;
		public AudioClip usedObjectSound;
		public List<inventoryElementNeededInfo> inventoryObjectNeededList = new List<inventoryElementNeededInfo> ();
	}

	[System.Serializable]
	public class inventoryElementNeededInfo
	{
		public bool instantiateObject;
		public Transform placeForObject;
		public bool enableObject;
		public GameObject objectToEnable;
		public bool objectActivated;

		public bool useEventOnObjectPlaced;
		public UnityEvent eventOnObjectPlaced;

		public bool useAnimation;
		public GameObject objectWithAnimation;
		public string animationName;
	}
}