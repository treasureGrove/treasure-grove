using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class chestSystem : MonoBehaviour
{
	public List<chestPickUpElementInfo> chestPickUpList = new List<chestPickUpElementInfo> ();
	public List<pickUpElementInfo> managerPickUpList = new List<pickUpElementInfo> ();

	public bool enablePickupsTriggerAtStart = true;

	public bool setNewPickupTriggerRadius = true;
	public float newPickupTriggerRadius = 3;

	public bool randomContent;
	public bool refillChestAfterDelay;
	public float timeOpenedAfterEmpty = 1;
	public float refilledTime;
	public string openAnimationName;
	public int numberOfObjects;
	public int minAmount;
	public int maxAmount;

	public Transform placeWhereInstantiatePickUps;
	public Vector3 placeOffset;
	public Vector3 space;
	public Vector2 amount;
	public float pickUpScale;
	public bool showGizmo;
	public Color gizmoColor;
	public Color gizmoLabelColor;
	public float gizmoRadius;
	public bool settings;
	public bool isLocked;
	public bool openWhenUnlocked;

	public bool useElectronicDeviceManager;

	public bool useEventOnOpenChest;
	public UnityEvent eventOnOpenChest;
	public bool useEventOnCloseChest;
	public UnityEvent eventOnCloseChest;

	public Animation chestAnim;
	public Collider mainCollider;
	public mapObjectInformation mapObjectInformationManager;

	public string mainPickupManagerName = "Pickup Manager";

	public bool useEventToSendSpawnedObjects;
	public eventParameters.eventToCallWithGameObject eventToSendSpawnedObjects;

	List<GameObject> objectsList = new List<GameObject> ();
	GameObject newObject;
	GameObject objectsParent;
	GameObject currentPlayer;
	bool enter;
	bool opened;

	pickUpManager mainPickupManager;

	pickUpObject currentPickUpObjectToCreate;

	usingDevicesSystem usingDevicesManager;

	GameObject objectToInstantiate;

	Coroutine mainCoroutine;


	void Start ()
	{
		objectsParent = transform.GetChild (0).gameObject;

		if (chestAnim == null) {
			chestAnim = GetComponent<Animation> ();
		}

		if (mainCollider == null) {
			mainCollider = GetComponent<Collider> ();
		}

		if (isLocked) {
			mainCollider.enabled = false;
		}

		if (mapObjectInformationManager == null) {
			mapObjectInformationManager = GetComponent<mapObjectInformation> ();
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (mainCoroutine != null) {
			StopCoroutine (mainCoroutine);
		}
	}

	IEnumerator updateCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateState ();

			yield return waitTime;
		}
	}

	void updateState ()
	{
		//if the chest can be refilled once is has been opened, check if is empty, and then wait one second to close it again
		if (opened && refillChestAfterDelay) {
			if (objectsParent.transform.childCount == 0) {
				StartCoroutine (waitTimeOpened ());

				opened = false;

				if (useEventOnCloseChest) {
					eventOnCloseChest.Invoke ();
				}

				stopUpdateCoroutine ();
			}
		}
	}

	//instantiate the objects inside the chest, setting their configuration
	void createObjects ()
	{
		numberOfObjects = 0;

		for (int i = 0; i < chestPickUpList.Count; i++) {
			chestPickUpElementInfo currentPickupElementInfo = chestPickUpList [i];

			for (int k = 0; k < currentPickupElementInfo.chestPickUpTypeList.Count; k++) {

				chestPickUpTypeElementInfo currentPickupInfo = currentPickupElementInfo.chestPickUpTypeList [k];

				//of every object, create the amount set in the inspector, the ammo and the inventory objects will be added in future updates
				int maxAmount = currentPickupInfo.amount;
				int quantity = currentPickupInfo.quantity;

				objectToInstantiate = managerPickUpList [currentPickupElementInfo.typeIndex].pickUpTypeList [currentPickupInfo.nameIndex].pickUpObject;

				if (randomContent) {
					maxAmount = (int)Random.Range (currentPickupInfo.amountLimits.x, currentPickupInfo.amountLimits.y);
				}

				for (int j = 0; j < maxAmount; j++) {
					if (randomContent) {
						quantity = (int)Random.Range (currentPickupInfo.quantityLimits.x, currentPickupInfo.quantityLimits.y);
					}
						
					newObject = (GameObject)Instantiate (objectToInstantiate, transform.position, Quaternion.identity);

					currentPickUpObjectToCreate = newObject.GetComponent<pickUpObject> ();

					if (currentPickUpObjectToCreate != null) {
						currentPickUpObjectToCreate.amount = quantity;
					}

					newObject.transform.localScale = Vector3.one * pickUpScale;
					addNewObject (newObject);
				}

				numberOfObjects += maxAmount;
			}
		}

		//set a fix position inside the chest, according to the amount of objects instantiated
		//the position of the first object
		Vector3 originialPosition = placeWhereInstantiatePickUps.position
		                            + placeWhereInstantiatePickUps.right * placeOffset.x
		                            + placeWhereInstantiatePickUps.up * placeOffset.y
		                            + placeWhereInstantiatePickUps.forward * placeOffset.z;
		
		Vector3 currentPosition = originialPosition;
		int rows = 0;
		int columns = 0;

		//set the localposition of every object, so every object is actually inside the chest
		for (int i = 0; i < numberOfObjects; i++) {	
			objectsList [i].transform.position = currentPosition;
			currentPosition += placeWhereInstantiatePickUps.right * space.x;

			if ((i + 1) % Mathf.Round (amount.y) == 0) {
				currentPosition = originialPosition + placeWhereInstantiatePickUps.up * space.y * columns;
				rows++;
				currentPosition -= placeWhereInstantiatePickUps.forward * space.z * rows;
			}

			if (rows == Mathf.Round (amount.x)) {
				columns++;
				currentPosition = originialPosition + placeWhereInstantiatePickUps.up * space.y * columns;
				rows = 0;
			}
		}

		objectsList.Clear ();
	}

	public void addNewObject (GameObject newObject)
	{
		newObject.transform.SetParent (objectsParent.transform);

		if (enablePickupsTriggerAtStart) {
			if (setNewPickupTriggerRadius) {
				SphereCollider currentPickupTrigger = newObject.GetComponentInChildren<SphereCollider> ();

				if (currentPickupTrigger != null) {
					currentPickupTrigger.radius = newPickupTriggerRadius;
				}
			}
		} else {
			SphereCollider currentPickupTrigger = newObject.GetComponentInChildren<SphereCollider> ();

			if (currentPickupTrigger != null) {
				currentPickupTrigger.enabled = false;
			}
		}

		objectsList.Add (newObject);

		if (useEventToSendSpawnedObjects) {
			eventToSendSpawnedObjects.Invoke (newObject);
		}
	}

	//when the player press the interaction button, this function is called
	void activateDevice ()
	{
		if (useElectronicDeviceManager) {
			return;
		}

		openOrCloseChest ();
	}

	public void openOrCloseChest ()
	{
		//check that the chest is not already opening, and play the open animation
		if (!chestAnim.IsPlaying (openAnimationName)) {
			if (!opened) {
				chestAnim [openAnimationName].speed = 1; 

				chestAnim.Play (openAnimationName);

				opened = true;

				createObjects ();

				if (useEventOnOpenChest) {
					eventOnOpenChest.Invoke ();
				}

				mainCoroutine = StartCoroutine (updateCoroutine ());

				if (!refillChestAfterDelay) {
					disableChestInteraction ();
				}
			}
		}
	}

	void disableChestInteraction ()
	{
		tag = "Untagged";

		if (currentPlayer != null) {
			usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();

			if (usingDevicesManager != null) {
				usingDevicesManager.removeDeviceFromList (gameObject);
			}
		}
	}

	IEnumerator waitTimeOpened ()
	{
		yield return new WaitForSeconds (timeOpenedAfterEmpty);

		//when the second ends, play the open animation reversed, to close it, enabling the icon of open chest again
		chestAnim [openAnimationName].speed = -1; 
		chestAnim [openAnimationName].time = chestAnim [openAnimationName].length;
		chestAnim.Play (openAnimationName);

		//wait the recharge time, so the chest can be reopened again
		yield return new WaitForSeconds (chestAnim [openAnimationName].length + refilledTime);

		//once the waiting time is over, enable the interaction button of the player
		tag = "device";

		if (enter && usingDevicesManager != null) {
			//enable the open icon in the hud
			usingDevicesManager.checkTriggerInfo (mainCollider, true);
		}
	}

	//check when the player enters or exits in the trigger of the chest
	void OnTriggerEnter (Collider col)
	{
		if (!enter && col.CompareTag ("Player")) {
			enter = true;

			if (currentPlayer == null) {
				currentPlayer = col.gameObject;
				usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();
			}
		} 
	}

	void OnTriggerExit (Collider col)
	{
		if (col.CompareTag ("Player")) {
			if (col.gameObject == currentPlayer) {
				enter = false;

				if (opened) {
					disableChestInteraction ();
				}

				currentPlayer = null;
				usingDevicesManager = null;
			}
		}
	}

	public void unlockChest ()
	{
		setLockedState (false);
	}

	public void setLockedState (bool state)
	{
		isLocked = state;

		if (isLocked) {
			if (mapObjectInformationManager != null) {
				mapObjectInformationManager.addMapObject ("Locked Chest");
			}

			mainCollider.enabled = false;
		} else {
			
			if (openWhenUnlocked) {
				if (currentPlayer != null) {
					usingDevicesManager.checkTriggerInfo (mainCollider, true);
					usingDevicesManager.inputActivateDevice ();
				} else {
					activateDevice ();
				}
			} else {
				mainCollider.enabled = true;
			}

			if (mapObjectInformationManager != null) {
				mapObjectInformationManager.addMapObject ("Chest");
			}
		}
	}

	public void getManagerPickUpList ()
	{
		if (mainPickupManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainPickupManagerName, typeof(pickUpManager));

			mainPickupManager = FindObjectOfType<pickUpManager> ();
		} 

		if (mainPickupManager != null) {
			managerPickUpList.Clear ();

			for (int i = 0; i < mainPickupManager.mainPickUpList.Count; i++) {	
				managerPickUpList.Add (mainPickupManager.mainPickUpList [i]);
			}

			updateComponent ();
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Chest Values", gameObject);
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (!Application.isPlaying && showGizmo && placeWhereInstantiatePickUps != null) {
			Vector3 originialPosition = placeWhereInstantiatePickUps.position
			                            + placeWhereInstantiatePickUps.right * placeOffset.x
			                            + placeWhereInstantiatePickUps.up * placeOffset.y
			                            + placeWhereInstantiatePickUps.forward * placeOffset.z;
			
			Vector3 currentPosition = originialPosition;

			int rows = 0;
			int columns = 0;

			//set the localposition of every object, so every object is actually inside the chest
			numberOfObjects = 0;

			if (randomContent) {
				minAmount = 0;
				maxAmount = 0;

				for (int i = 0; i < chestPickUpList.Count; i++) {	
					for (int j = 0; j < chestPickUpList [i].chestPickUpTypeList.Count; j++) {	
						minAmount += (int)chestPickUpList [i].chestPickUpTypeList [j].amountLimits.x;
						maxAmount += (int)chestPickUpList [i].chestPickUpTypeList [j].amountLimits.y;
					}
				}

				numberOfObjects = minAmount + maxAmount;

				for (int i = 0; i < numberOfObjects; i++) {	
					if (i < minAmount) {
						Gizmos.color = Color.blue;
						Gizmos.DrawSphere (currentPosition, gizmoRadius);
						Gizmos.color = gizmoLabelColor;
						//Gizmos.DrawWireSphere (currentPosition, pickUpScale);

						currentPosition += placeWhereInstantiatePickUps.right * space.x;

						if (i != 0 && (i + 1) % Mathf.Round (amount.y) == 0) {
							currentPosition = originialPosition + placeWhereInstantiatePickUps.up * space.y * columns;
							rows++;
							currentPosition -= placeWhereInstantiatePickUps.forward * space.z * rows;
						}

						if (rows == Mathf.Round (amount.x)) {
							columns++;
							currentPosition = originialPosition + placeWhereInstantiatePickUps.up * space.y * columns;
							rows = 0;
						}
					}

					if (i >= minAmount) {
						Gizmos.color = Color.red;
						Gizmos.DrawSphere (currentPosition, gizmoRadius);
						Gizmos.color = gizmoLabelColor;

						//Gizmos.DrawWireSphere (currentPosition, pickUpScale);
						currentPosition += placeWhereInstantiatePickUps.right * space.x;

						if (i != 0 && (i + 1) % Mathf.Round (amount.y) == 0) {
							currentPosition = originialPosition + placeWhereInstantiatePickUps.up * space.y * columns;
							rows++;
							currentPosition -= placeWhereInstantiatePickUps.forward * space.z * rows;
						}

						if (rows == Mathf.Round (amount.x)) {
							columns++;
							currentPosition = originialPosition + placeWhereInstantiatePickUps.up * space.y * columns;
							rows = 0;
						}
					}
				}
			} else {
				for (int i = 0; i < chestPickUpList.Count; i++) {	
					for (int j = 0; j < chestPickUpList [i].chestPickUpTypeList.Count; j++) {	
						numberOfObjects += chestPickUpList [i].chestPickUpTypeList [j].amount;
					}
				}

				for (int i = 0; i < numberOfObjects; i++) {	
					Gizmos.color = gizmoColor;
					Gizmos.DrawSphere (currentPosition, gizmoRadius);
					Gizmos.color = gizmoLabelColor;
					//Gizmos.DrawWireSphere (currentPosition, pickUpScale);

					currentPosition += placeWhereInstantiatePickUps.right * space.x;

					if ((i + 1) % Mathf.Round (amount.y) == 0) {
						currentPosition = originialPosition + placeWhereInstantiatePickUps.up * space.y * columns;
						rows++;
						currentPosition -= placeWhereInstantiatePickUps.forward * space.z * rows;
					}

					if (rows == Mathf.Round (amount.x)) {
						columns++;
						currentPosition = originialPosition + placeWhereInstantiatePickUps.up * space.y * columns;
						rows = 0;
					}
				}
			}
		}
	}

	[System.Serializable]
	public class chestPickUpElementInfo
	{
		public string pickUpType;
		public int typeIndex;
		public List<chestPickUpTypeElementInfo> chestPickUpTypeList = new List<chestPickUpTypeElementInfo> ();
	}

	[System.Serializable]
	public class chestPickUpTypeElementInfo
	{
		public string name;
		public int amount;
		public int quantity;
		public Vector2 amountLimits;
		public Vector2 quantityLimits;
		public int nameIndex;
	}
}