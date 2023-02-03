using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class pickUpObject : MonoBehaviour
{
	public int amount;
	public bool useAmountPerUnit;
	public int amountPerUnit;

	public bool useDurability;

	public float maxDurabilityAmount;

	public float durabilityAmount;

	public AudioClip pickUpSound;

	public bool staticPickUp;
	public bool moveToPlayerOnTrigger = true;
	public pickUpMode pickUpOption;

	public bool canBeExamined;

	public bool usableByAnything;
	public bool usableByPlayer = true;
	public bool usableByVehicles = true;
	public bool usableByCharacters;

	public bool showPickupInfoOnTaken = true;
	public bool usePickupIconOnTaken;
	public Texture pickupIcon;

	public enum pickUpType
	{
		health,
		energy,
		ammo,
		inventory,
		jetpackFuel,
		weapon,
		inventoryExtraSpace,
		map,
		vehicleFuel,
		attachment,
		power
	}

	public enum pickUpMode
	{
		trigger,
		button
	}

	public bool takeWithTrigger = true;

	public bool usePickupIconOnScreen = true;
	public string pickupIconGeneralName;
	public string pickupIconName;

	public bool useEventOnTaken;
	public UnityEvent eventOnTaken;
	public bool useEventOnRemainingAmount;
	public UnityEvent eventOnRemainingAmount;

	public bool sendPickupFinder;
	public eventParameters.eventToCallWithGameObject sendPickupFinderEvent;

	public GameObject player;
	public GameObject vehicle;

	public GameObject npc;
	public GameObject finder;

	public bool finderIsPlayer;
	public bool finderIsVehicle;
	public bool finderIsCharacter;

	public string playerTag = "Player";
	public string friendTag = "friend";
	public string enemyTag = "enemy";

	public int amountTaken;

	public bool ignoreExamineObjectBeforeStoreEnabled;

	public pickupType mainPickupType;

	public inventoryObject inventoryObjectManager;

	public deviceStringAction deviceStringActionManager;

	public SphereCollider mainSphereCollider;
	public Collider mainCollider;

	public Rigidbody mainRigidbody;


	public playerWeaponsManager weaponsManager;
	public vehicleHUDManager vehicleHUD;

	public inventoryManager playerInventoryManager;

	public string mainPickupManagerName = "Pickup Manager";


	bool freeSpaceInInventorySlot;
	public int inventoryAmountPicked;
	Vector3 pickUpTargetPosition;

	pickUpManager mainPickupManager;
	grabbedObjectState currentGrabbedObject;

	pickUpsScreenInfo pickUpsScreenInfoManager;

	float targetPositionOffset = 1.5f;
	float distanceToByUsed = 1;

	public bool examiningObject;

	bool touched;

	List<GameObject> playerFoundList = new List<GameObject> ();

	bool initialAmountConfigured;

	bool pickupIconRemoved;

	GameObject gameObjectDetected;

	//if the pick up object has an icon in the inspector, instantiated in the hud
	void Start ()
	{
		getComponents ();
	}

	public void activatePickupMovement ()
	{
		StartCoroutine (activatePickupMovementCoroutine ());
	}

	IEnumerator activatePickupMovementCoroutine ()
	{
		bool targetReached = false;

		while (!targetReached) {
			//if the player enters inside the object's trigger, translate the object's position to the player 
		
			if (finder != null) {
				pickUpTargetPosition = finder.transform.position + finder.transform.up * targetPositionOffset;
			}

			transform.position = Vector3.MoveTowards (transform.position, pickUpTargetPosition, Time.deltaTime * 15);

			//if the object is close enough, increase the finder's values, according to the type of object
			if (GKC_Utils.distance (transform.position, pickUpTargetPosition) < distanceToByUsed) {
				targetReached = true;
			}

			yield return null;
		}

		pickObject ();
	}

	void assignComponents ()
	{
		if (mainRigidbody == null) {
			mainRigidbody = GetComponent<Rigidbody> ();
		}

		if (mainSphereCollider == null) {
			mainSphereCollider = GetComponentInChildren<SphereCollider> ();
		}

		if (mainCollider == null) {
			mainCollider = GetComponent<Collider> ();
		}

		if (deviceStringActionManager == null) {
			deviceStringActionManager = GetComponentInChildren<deviceStringAction> ();
		}

		if (inventoryObjectManager == null) {
			inventoryObjectManager = GetComponentInChildren<inventoryObject> ();
		}
	}

	public void getComponents ()
	{
		if (amount == 0) {
			amount = 1;
		}

		assignComponents ();

		setUpIcon ();

		//if the pick up is static, set its rigibody to kinematic and reduce its radius, so the player has to come closer to get it

		if (pickUpOption != pickUpMode.trigger) {
			takeWithTrigger = false;
		}

		if (staticPickUp) {
			mainRigidbody.isKinematic = true;
			mainSphereCollider.radius = 1;
		}

		getInventoryInfo ();

		if (deviceStringActionManager != null) {
			if (takeWithTrigger) {
				deviceStringActionManager.setIconEnabledState (false);

				deviceStringActionManager.gameObject.tag = "Untagged";
			}
		}
	}

	public void playPickupSound ()
	{
		//play the pick up sound effect
		AudioSource finderAudioSource = applyDamage.getAudioSource (finder, "Pickup Object Audio Source");

		if (finderAudioSource != null) {
			finderAudioSource.PlayOneShot (pickUpSound);
		}

	}

	public void pickObject ()
	{
		//check if this object has been grabbed by the player, to drop it, before destroy it
		checkIfGrabbed ();

		mainPickupType.confirmTakePickup ();

		checkEventOnTaken ();
	}

	public void removePickupFromLevel ()
	{
		//remove the icon object
		removePickupInfo ();

		Destroy (gameObject);
	}

	void removePickupInfo ()
	{
		if (mainPickupManager != null) {
			mainPickupManager.removeTarget (gameObject);
		}

		if (canBeExamined && examiningObject) {
			if (finderIsPlayer) {
				if (player != null) {
					player.GetComponent<usingDevicesSystem> ().setExamineteDevicesCameraState (false, false);
				}
			}
		}

		pickupIconRemoved = true;
	}

	void OnDestroy ()
	{
		if (GKC_Utils.isApplicationPlaying () && Time.deltaTime > 0) {
//			print ("scene loaded " + this.gameObject.scene.isLoaded + " " + Time.timeScale + " " + Time.deltaTime);

			if (!pickupIconRemoved) {
				removePickupInfo ();
			}
		}
	}

	public void checkEventOnTaken ()
	{
		if (useEventOnTaken) {
			eventOnTaken.Invoke ();
		}

		if (sendPickupFinder && finder != null) {
			sendPickupFinderEvent.Invoke (finder);
		}
	}

	public void checkEventOnRemainingAmount ()
	{
		if (finderIsPlayer) {
			usingDevicesSystem currentUsingDevicesSystem = player.GetComponent<usingDevicesSystem> ();

			if (currentUsingDevicesSystem != null) {
				currentUsingDevicesSystem.setInteractionButtonName ();
			}
		}

		if (useEventOnRemainingAmount) {
			eventOnRemainingAmount.Invoke ();
		}
	}

	public float getAmountPicked ()
	{
		if (useAmountPerUnit) {
			return (amount * amountPerUnit);
		}

		return amount;
	}

	//instantiate the icon object to show the type of pick up in the player's HUD
	public void setUpIcon ()
	{
		if (!usePickupIconOnScreen) {
			return;
		}

		bool pickupManagerLocated = (mainPickupManager != null);

		if (!pickupManagerLocated) {
			mainPickupManager = FindObjectOfType<pickUpManager> (); 

			pickupManagerLocated = (mainPickupManager != null);
		}

		if (!pickupManagerLocated) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainPickupManagerName, typeof(pickUpManager));

			mainPickupManager = FindObjectOfType<pickUpManager> (); 

			pickupManagerLocated = (mainPickupManager != null);
		}

		if (pickupManagerLocated) {
			mainPickupManager.setPickUpIcon (gameObject, pickupIconGeneralName, pickupIconName);

			pickupIconRemoved = false;
		}
	}

	public void pausePickupIconState ()
	{
		if (mainPickupManager != null) {
			mainPickupManager.setPauseState (true, gameObject);
		}
	}

	public void resumePickupIconState ()
	{
		if (mainPickupManager != null) {
			mainPickupManager.setPauseState (false, gameObject);
		}
	}

	public void removePickupIcon ()
	{
		removePickupInfo ();
	}

	public void removePickupIconAndDisableObject ()
	{
		removePickupInfo ();

		if (gameObject.activeSelf) {
			gameObject.SetActive (false);
		}
	}

	public void pickObjectByButton ()
	{
//		print (canBeExamined + " " + examiningObject + " " + checkIfCanBePicked ());

		if (canBeExamined && !examiningObject) {
			pausePickupIconState ();

			examiningObject = true;
			return;
		}

		if (!checkIfCanBePicked ()) {
			return;
		}

		mainPickupType.takePickupByButton ();
	}

	public void confirmTakePickupByButton ()
	{
		checkIfGrabbed ();

		pickObject ();
	}

	public void cancelPickObject ()
	{
		if (canBeExamined && examiningObject) {
			resumePickupIconState ();

			examiningObject = false;

			electronicDevice mainElectronicDevice = GetComponentInChildren<electronicDevice> ();
				
			if (mainElectronicDevice != null) {
				mainElectronicDevice.activateDevice ();
			}
		}
	}

	//check if the player is inside the object trigger
	public void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col);
	}

	public void OnTriggerExit (Collider col)
	{
		if (playerFoundList.Contains (col.gameObject)) {
			playerFoundList.Remove (col.gameObject);
		}
	}

	public void checkTriggerInfoByGameObject (GameObject objectToCheck)
	{
		if (objectToCheck != null) {
			Collider newCollider = objectToCheck.GetComponent<Collider> ();

			if (newCollider != null) {
				checkTriggerInfo (newCollider);
			}
		}
	}

	public void checkTriggerInfo (Collider col)
	{
		if (touched) {
			return;
		}

		gameObjectDetected = col.gameObject;

		if (usableByAnything || usableByPlayer) {
			if (col.CompareTag (playerTag) && !col.isTrigger) {
				player = gameObjectDetected;

				if (!playerFoundList.Contains (player)) {
					playerFoundList.Add (player);
				}

				setFinderType (true, false, false);

				playerComponentsManager currentPlayerComponentsManager = player.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					pickUpsScreenInfoManager = currentPlayerComponentsManager.getPickUpsScreenInfo ();
				}

				//check if the player needs this pickup
				finder = player;

				if (takeWithTrigger) {
					if (!checkIfCanBePicked ()) {
						return;
					}

					mainPickupType.takePickupByTrigger ();
				}

				return;
			}
		}

		if (usableByAnything || usableByVehicles) {
			vehicleHUD = applyDamage.getVehicleHUDManager (gameObjectDetected);

			//else check if the player is driving
			if (vehicleHUD != null) {
				if (vehicleHUD.isVehicleBeingDriven ()) {
					//then set the vehicle as the object which use the pickup
					vehicle = gameObjectDetected;

					setFinderType (false, true, false);

					player = vehicleHUD.IKDrivingManager.getcurrentDriver ();

					if (player != null) {
						playerComponentsManager currentPlayerComponentsManager = player.GetComponent<playerComponentsManager> ();

						if (currentPlayerComponentsManager != null) {
							pickUpsScreenInfoManager = currentPlayerComponentsManager.getPickUpsScreenInfo ();
						}

						//check if the vehicle needs this pickup
						if (!checkIfCanBePicked ()) {
							return;
						}

						mainPickupType.takePickupByTrigger ();
					}
				}

				return;
			}
		}

		if (usableByAnything || usableByCharacters) {
			//else check if the finder is an ai
			bool checkResult = false;

			if ((col.CompareTag (enemyTag) || col.CompareTag (friendTag)) && !col.isTrigger) {
				checkResult = true;
			}

			if (gameObjectDetected.GetComponent<AINavMesh> ()) {
				checkResult = true;
			}

			if (checkResult) {
				//then set the character as the object which use the pickup
				npc = gameObjectDetected;

				setFinderType (false, false, true);

				if (takeWithTrigger) {
					if (!checkIfCanBePicked ()) {
						return;
					}

					mainPickupType.takePickupByTrigger ();
				}
			}
		}
	}

	public void confirmTakePickupByTrigger ()
	{
		if (finderIsPlayer) {
			Physics.IgnoreCollision (finder.GetComponent<Collider> (), mainCollider);

			checkIfGrabbed ();

			mainRigidbody.isKinematic = true;

			if (moveToPlayerOnTrigger) {
				touched = true;

				activatePickupMovement ();
			} else {
				pickObject ();
			}

			return;
		}

		//else check if the player is driving
		if (finderIsVehicle) {
			finder = vehicle;

			checkIfGrabbed ();

			if (takeWithTrigger) {
				mainCollider.isTrigger = true;

				mainRigidbody.isKinematic = true;

				if (moveToPlayerOnTrigger) {
					touched = true;

					activatePickupMovement ();

				} else {
					pickObject ();
				}
			} else {
				pickObject ();
			}

			return;
		}

		//else check if the finder is an ai
		if (finderIsCharacter) {
			finder = npc;

			checkIfGrabbed ();

			if (takeWithTrigger) {
				Physics.IgnoreCollision (finder.GetComponent<Collider> (), mainCollider);

				mainCollider.isTrigger = true;
				mainRigidbody.isKinematic = true;

				if (moveToPlayerOnTrigger) {
					touched = true;

					activatePickupMovement ();
				} else {
					pickObject ();
				}
			} else {
				pickObject ();
			}
		}
	}

	public void setFinderType (bool isPlayer, bool isVehicle, bool isCharacter)
	{
		finderIsPlayer = isPlayer;
		finderIsVehicle = isVehicle;
		finderIsCharacter = isCharacter;
	}

	//check the values of health and energy according to the type of pickup, so the pickup will be used or not according to the values of health or energy
	//When the player/vehicle grabs a pickup, this will check if the amount of health, energy or ammo is filled or not,
	//so the player/vehicle only will get the neccessary objects to restore his state. In version 2.3, the player grabbed every pickup close to him.
	//for example, if the player has 90/100, he only will grab a health pickup
	bool checkIfCanBePicked ()
	{
		if (!usableByAnything) {
			if ((usableByPlayer && finderIsPlayer) || (usableByVehicles && finderIsVehicle) || (usableByCharacters && finderIsCharacter)) {
				//print ("usable");
			} else {
				return false;
			}
		}

		return true;
	}

	public bool tryToPickUpObject ()
	{
		playerComponentsManager mainPlayerComponentsManager = player.GetComponent<playerComponentsManager> ();

		playerController mainPlayerController = mainPlayerComponentsManager.getPlayerController ();

		if (mainPlayerController.isPlayerDriving ()) {
			return false;
		}

		inventoryAmountPicked = 0;

		playerInventoryManager = mainPlayerComponentsManager.getInventoryManager ();

		if (!playerInventoryManager.isInventoryFull ()) {
			if (useDurability) {
				inventoryObjectManager.inventoryObjectInfo.durabilityAmount = durabilityAmount;

				inventoryObjectManager.inventoryObjectInfo.maxDurabilityAmount = maxDurabilityAmount;
			}

			if (!ignoreExamineObjectBeforeStoreEnabled &&
			    !playerInventoryManager.playerIsExaminingInventoryObject () &&
			    !mainPlayerComponentsManager.getUsingDevicesSystem ().isExaminingObject ()) {

				if (playerInventoryManager.examineObjectBeforeStoreEnabled) {
					playerInventoryManager.examineCurrentPickupObject (inventoryObjectManager.inventoryObjectInfo);

					playerInventoryManager.setCurrentPickupObject (this);

					return false;
				} 
			}
		
			inventoryAmountPicked = playerInventoryManager.tryToPickUpObject (inventoryObjectManager.inventoryObjectInfo);

//			print ("pickup amount " + inventoryAmountPicked);

			if (inventoryAmountPicked > 0) {
				inventoryObjectManager.inventoryObjectInfo.amount -= inventoryAmountPicked;
				amount = inventoryObjectManager.inventoryObjectInfo.amount;			

				inventoryObjectManager.eventOnPickObjectNewBehaviour (player);

				return true;
			} else {
				playerInventoryManager.showInventoryFullMessage ();
			}
		} else {
			playerInventoryManager.showInventoryFullMessage ();
		}

		return false;
	}

	public void setDurabilityInfo (bool state, float newDurabilityAmount, float newMaxDurabilityAmount)
	{
		useDurability = state;

		durabilityAmount = newDurabilityAmount;

		maxDurabilityAmount = newMaxDurabilityAmount;
	}

	public int getLastinventoryAmountPicked ()
	{
		return inventoryAmountPicked;
	}

	//just to ignore the collisions with a turret when it explodes
	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.layer == LayerMask.NameToLayer ("Ignore Raycast")) {
			if (col.collider != null && mainCollider != null) {
				Physics.IgnoreCollision (col.collider, mainCollider);
			}
		}
	}

	//drop this object just in case the object has grabbed it to use it
	void checkIfGrabbed ()
	{
		if (finderIsPlayer && player != null) {
			//if the object is being carried by the player, make him drop it
			currentGrabbedObject = GetComponent<grabbedObjectState> ();

			if (currentGrabbedObject != null) {
				GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), gameObject);
			}
		}
	}

	//enable the trigger of the pickup, so the player can use it
	public void activateObjectTrigger ()
	{
		if (mainSphereCollider != null && !mainSphereCollider.enabled) {
			mainSphereCollider.enabled = true;
		}
	}

	public void showRecieveInfo (string message)
	{
		if (!showPickupInfoOnTaken) {
			return;
		}

		if (finderIsPlayer || finderIsVehicle) {
			if (pickUpsScreenInfoManager != null) {
				if (usePickupIconOnTaken) {
					pickUpsScreenInfoManager.recieveInfo (message, pickupIcon);
				} else {
					pickUpsScreenInfoManager.recieveInfo (message);
				}
			}
		}
	}

	public void setPickUpAmount (int amountToSet)
	{
		amount = amountToSet;

		getInventoryInfo ();
	}

	public void getInventoryInfo ()
	{
		if (inventoryObjectManager == null) {
			inventoryObjectManager = GetComponentInChildren<inventoryObject> ();
		}
		
		if (inventoryObjectManager != null && !initialAmountConfigured) {
			setAmount (amount);
			
			initialAmountConfigured = true;
		}
	}

	public void setAmount (int newAmount)
	{
		if (inventoryObjectManager.inventoryObjectInfo.storeTotalAmountPerUnit) {
			amount = amount * inventoryObjectManager.inventoryObjectInfo.amountPerUnit;

			inventoryObjectManager.inventoryObjectInfo.amountPerUnit = 0;
		}

		inventoryObjectManager.inventoryObjectInfo.amount = amount;

		if (useAmountPerUnit) {
			inventoryObjectManager.inventoryObjectInfo.amountPerUnit = amountPerUnit;
		}
	}

	public void setNewAmount (int newAmount)
	{
		amount = newAmount;

		if (inventoryObjectManager == null) {
			getInventoryInfo ();
		} else {
			setAmount (amount);
		}
	}

	public SphereCollider getPickupTrigger ()
	{
		return mainSphereCollider;
	}

	public void removeDeviceFromListExternalCall (GameObject deviceGameObject)
	{
		if (playerFoundList.Count > 0) {
			for (int i = 0; i < playerFoundList.Count; i++) {

				usingDevicesSystem currentUsingDevicesSystem = playerFoundList [i].GetComponent<usingDevicesSystem> ();

				if (currentUsingDevicesSystem != null) {

					currentUsingDevicesSystem.removeDeviceFromListExternalCall (deviceGameObject);
				}
			}
		}
	}

	public void assignPickupElementsOnEditor ()
	{
		objectOnInventory currentobjectOnInventory = GetComponent<objectOnInventory> ();

		inventoryObjectManager = GetComponentInChildren<inventoryObject> ();

		if (currentobjectOnInventory != null && inventoryObjectManager != null) {
			currentobjectOnInventory.mainInventoryObject = inventoryObjectManager;

			inventoryObjectManager.mainObjectOnInventory = currentobjectOnInventory;
		}

		mainPickupType = GetComponent<pickupType> ();

		if (mainPickupType != null) {
			mainPickupType.mainPickupObject = this;
		}

		assignComponents ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}