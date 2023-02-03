using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class inventoryListManager : MonoBehaviour
{
	public List<inventoryInfo> inventoryList = new List<inventoryInfo> ();

	public List<inventoryCategoryInfo> inventoryCategoryInfoList = new List<inventoryCategoryInfo> ();

	public GameObject emptyInventoryPrefab;

	public string relativePathCaptures = "Assets/Game Kit Controller/Prefabs/Inventory/Captures";

	public Camera inventoryCamera;
	public Transform lookObjectsPosition;

	public GameObject inventoryCameraParentGameObject;

	public string relativeInventoryPath = "Assets/Game Kit Controller/Prefabs/Inventory/Usable";

	public inventoryBankManager mainInventoryBankManager;

	[TextArea (3, 10)]public string inventoryListMessage = "IMPORTANT: THIS IS A DEBUG LIST OF INVENTORY OBJECTS, USE THE INVENTORY CATEGORY INFO LIST ABOVE TO ADD NEW ONES.";

	[TextArea (3, 10)]public string inventoryListUpdateMessage = "Press the Update Inventory List Button when you change the settings of any inventory object in the Inventory Category Info List";

	public inventoryListManagerData mainInventoryListManagerData;

	public bool inventoryCaptureToolOpen;

	public bool removeInventoryPrefabWhenDeletingInventoryObject;

	public GameObject newInventoryObjectToAddThroughEditor;

	public bool useNewCategoryToAddObject;
	public string newCategoryToAddObject;

	public bool updateInventoryListScriptableObjectOnCreateEditRemove = true;

	public fullCraftingBlueprintInfoTemplateData mainFullCraftingBlueprintInfoTemplateData;


	public List<GameObject> getDisassemblePiecesOfCraftingObjectByName (string objectName, ref List<int> amountList)
	{
		if (mainFullCraftingBlueprintInfoTemplateData == null) {
			return null;
		}

		int dataListCount = mainFullCraftingBlueprintInfoTemplateData.craftingBlueprintInfoTemplateDataList.Count;

		for (int i = 0; i < dataListCount; i++) {
			List<craftingBlueprintInfoTemplateData> craftingBlueprintInfoTemplateDataList = mainFullCraftingBlueprintInfoTemplateData.craftingBlueprintInfoTemplateDataList;

			int craftingBlueprintInfoTemplateDataListCount = craftingBlueprintInfoTemplateDataList.Count;

			for (int j = 0; j < craftingBlueprintInfoTemplateDataListCount; j++) {

				int currentIndex = craftingBlueprintInfoTemplateDataList [j].craftingBlueprintInfoTemplateList.FindIndex (s => s.Name.Equals (objectName));

				if (currentIndex > -1) {
					List<craftingIngredientObjectInfo> currentIngredientObjectInfoList = 
						craftingBlueprintInfoTemplateDataList [j].craftingBlueprintInfoTemplateList [currentIndex].disassembleIngredientObjectInfoList;

					List<GameObject> objectList = new List<GameObject> ();

					for (int k = 0; k < currentIngredientObjectInfoList.Count; k++) {
						GameObject currentObject = getInventoryPrefabByName (currentIngredientObjectInfoList [k].Name);

						if (currentObject != null) {

							objectList.Add (currentObject);

							amountList.Add (currentIngredientObjectInfoList [k].amountRequired);
						}
					}

					return objectList;
				}
			}
		}

		return null;
	}


	//INGAME FUNCTIONS
	public bool existInventoryInfoFromName (string objectName)
	{
		objectName = objectName.ToLower ();

		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			if (inventoryList [i].Name.ToLower ().Equals (objectName)) {
				return true;
			}
		}

		return false;
	}

	public inventoryInfo getInventoryInfoFromName (string objectName)
	{
		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.Name.Equals (objectName)) {
				return currentInventoryInfo;
			}
		}

		return null;
	}

	public inventoryInfo getInventoryInfoFromInventoryGameObject (GameObject objectToFind)
	{
		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.inventoryGameObject == objectToFind) {
				return currentInventoryInfo;
			}
		}

		return null;
	}

	public inventoryInfo getInventoryInfoFromCategoryListByName (string inventoryObjectName)
	{
		inventoryCategoryInfo currentInventoryCategoryInfo = null;
		inventoryInfo currentInventoryInfo = null;

		int inventoryCategoryInfoListCount = inventoryCategoryInfoList.Count;

		for (int i = 0; i < inventoryCategoryInfoListCount; i++) {

			currentInventoryCategoryInfo = inventoryCategoryInfoList [i];

			int inventoryListCount = currentInventoryCategoryInfo.inventoryList.Count;

			for (int j = 0; j < inventoryListCount; j++) {

				currentInventoryInfo = currentInventoryCategoryInfo.inventoryList [j];

				if (currentInventoryInfo.Name.Equals (inventoryObjectName)) {
					return currentInventoryInfo;
				}
			}
		}

		return null;
	}

	public int getInventoryAmountPerUnitFromInventoryGameObject (GameObject objectToFind)
	{
		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.inventoryGameObject == objectToFind) {
				return currentInventoryInfo.amountPerUnit;
			}
		}

		return -1;
	}

	public int getInventoryCategoryIndexByName (string categoryName)
	{
		int inventoryCategoryInfoListCount = inventoryCategoryInfoList.Count;

		for (int i = 0; i < inventoryCategoryInfoListCount; i++) {
			if (inventoryCategoryInfoList [i].Name.Equals (categoryName)) {
				return i;
			}
		}

		return -1;
	}

	public int getInventoryInfoIndexByName (string inventoryObjectName)
	{
		inventoryCategoryInfo currentInventoryCategoryInfo = null;
		inventoryInfo currentInventoryInfo = null;

		int inventoryCategoryInfoListCount = inventoryCategoryInfoList.Count;

		for (int i = 0; i < inventoryCategoryInfoListCount; i++) {

			currentInventoryCategoryInfo = inventoryCategoryInfoList [i];

			int inventoryListCount = currentInventoryCategoryInfo.inventoryList.Count;

			for (int j = 0; j < inventoryListCount; j++) {

				currentInventoryInfo = currentInventoryCategoryInfo.inventoryList [j];

				if (currentInventoryInfo.Name.Equals (inventoryObjectName)) {
					return j;
				}
			}
		}

		return -1;
	}

	public GameObject getInventoryPrefab (GameObject inventoryGameObject)
	{
		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.inventoryGameObject == inventoryGameObject) {
				return currentInventoryInfo.inventoryObjectPrefab;
			}
		}

		return null;
	}

	public GameObject getInventoryPrefabByName (string objectName)
	{
		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.Name.Equals (objectName)) {
				return currentInventoryInfo.inventoryObjectPrefab;
			}
		}

		return null;
	}

	public GameObject getInventoryMeshByName (string objectName)
	{
		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.Name.Equals (objectName)) {
				return currentInventoryInfo.inventoryGameObject;
			}
		}

		return null;
	}


	public inventoryObject getInventoryObjectComponentByInventoryGameObject (GameObject inventoryGameObjectToSearch)
	{
		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.inventoryGameObject == inventoryGameObjectToSearch) {
				return currentInventoryInfo.inventoryObjectPrefab.GetComponentInChildren<inventoryObject> ();
			}
		}

		return null;
	}

	public int getInventoryObjectAmountPerUnitByName (string inventoryObjectName)
	{
		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.Name.Equals (inventoryObjectName)) {
				return currentInventoryInfo.amountPerUnit;
			}
		}

		return 0;
	}

	public Texture getInventoryObjectIconByName (string inventoryObjectName)
	{
		int inventoryListCount = inventoryList.Count;

		for (int i = 0; i < inventoryListCount; i++) {
			inventoryInfo currentInventoryInfo = inventoryList [i];

			if (currentInventoryInfo.Name.Equals (inventoryObjectName)) {
				return currentInventoryInfo.icon;
			}
		}

		return null;
	}

	//EDITOR FUNCTIONS
	public void setInventoryCaptureIcon (inventoryInfo info, Texture2D texture)
	{
		inventoryCategoryInfo currentInventoryCategoryInfo = null;
		inventoryInfo currentInventoryInfo = null;

		int inventoryCategoryInfoListCount = inventoryCategoryInfoList.Count;

		for (int i = 0; i < inventoryCategoryInfoListCount; i++) {

			currentInventoryCategoryInfo = inventoryCategoryInfoList [i];

			for (int j = 0; j < currentInventoryCategoryInfo.inventoryList.Count; j++) {

				currentInventoryInfo = currentInventoryCategoryInfo.inventoryList [j];

				if (currentInventoryInfo == info) {
					currentInventoryInfo.icon = texture;
				}
			}
		}

		updateComponent ();
	}

	public string getDataPath ()
	{
		string dataPath = "";

		if (!Directory.Exists (relativePathCaptures)) {
			Directory.CreateDirectory (relativePathCaptures);
		}

		dataPath = relativePathCaptures + "/";

		return dataPath;
	}

	public void addNewInventoryObject (int categoryIndex, inventoryInfo newInventoryInfo)
	{
		inventoryCategoryInfo currentInventoryCategoryInfo = inventoryCategoryInfoList [categoryIndex];

		newInventoryInfo.categoryName = currentInventoryCategoryInfo.Name;

		newInventoryInfo.categoryIndex = categoryIndex;

		newInventoryInfo.elementIndex = currentInventoryCategoryInfo.inventoryList.Count + 1;

		currentInventoryCategoryInfo.inventoryList.Add (newInventoryInfo);

		inventoryList.Add (newInventoryInfo);

		inventoryCaptureToolOpen = false;

		updateComponent ();
	}

	public void addNewInventoryObject (int categoryIndex)
	{
		inventoryCategoryInfo currentInventoryCategoryInfo = inventoryCategoryInfoList [categoryIndex];

		inventoryInfo newObject = new inventoryInfo ();

		newObject.categoryName = currentInventoryCategoryInfo.Name;

		newObject.categoryIndex = categoryIndex;

		newObject.elementIndex = currentInventoryCategoryInfo.inventoryList.Count + 1;

		currentInventoryCategoryInfo.inventoryList.Add (newObject);

		inventoryList.Add (newObject);

		inventoryCaptureToolOpen = false;

		updateComponent ();
	}

	public void removeInventoryObject (int categoryIndex, int objectIndex)
	{
		inventoryCategoryInfo currentInventoryCategoryInfo = inventoryCategoryInfoList [categoryIndex];

		string objectName = currentInventoryCategoryInfo.inventoryList [objectIndex].Name;

		currentInventoryCategoryInfo.inventoryList.RemoveAt (objectIndex);

		bool objectFound = false;

		for (int i = 0; i < inventoryList.Count; i++) {
			if (!objectFound && inventoryList [i].Name.Equals (objectName)) {
				inventoryList.RemoveAt (i);
				objectFound = true;
			}
		}

		updateSingleInventoryList ();

		print ("Object " + objectName + " removed from inventory list");

		updateComponent ();

		checkIfUpdateInventoryListScriptableObjectOnCreateEditRemove ();
	}

	public void createInventoryPrafab (int categoryIndex, int index)
	{
		#if UNITY_EDITOR
		inventoryCategoryInfo currentInventoryCategoryInfo = inventoryCategoryInfoList [categoryIndex];

		inventoryInfo currentInventoryInfo = currentInventoryCategoryInfo.inventoryList [index];

		if (currentInventoryInfo.inventoryGameObject == null) {
			print ("Please, the Inventory Object Mesh to create the prefab");
			return;
		}

		GameObject prefabToInstantiate = currentInventoryInfo.emptyInventoryPrefab;

		if (prefabToInstantiate == null) {
			prefabToInstantiate = currentInventoryCategoryInfo.emptyInventoryPrefab;
		}

		if (prefabToInstantiate == null) {
			prefabToInstantiate = emptyInventoryPrefab;
		}

		if (prefabToInstantiate == null) {
			print ("WARNING: no empty inventory prefab has been found on the category " + currentInventoryInfo.categoryName + " " +
			"Make sure to configure one of the empty inventory object prefabs on this category or this specific new object");
		}

		GameObject newEmptyInventoryPrefab = Instantiate (prefabToInstantiate);

		inventoryObject currentInventoryObject = newEmptyInventoryPrefab.GetComponentInChildren<inventoryObject> ();

		currentInventoryObject.inventoryObjectInfo = new inventoryInfo (currentInventoryInfo);

		GameObject newInventoryObjectMesh = Instantiate (currentInventoryInfo.inventoryGameObject);
		newInventoryObjectMesh.transform.SetParent (newEmptyInventoryPrefab.transform);
		newInventoryObjectMesh.transform.localPosition = Vector3.zero;
		newInventoryObjectMesh.transform.localRotation = Quaternion.identity;

		newInventoryObjectMesh.name = newInventoryObjectMesh.name.Replace ("(Clone)", "");


		string newLayer = "Default";

		int newLayerIndex = LayerMask.NameToLayer (newLayer);

		Component[] components = newInventoryObjectMesh.GetComponentsInChildren (typeof(Transform));
		foreach (Component c in components) {
			c.gameObject.layer = newLayerIndex;
		}



		Component[] colliders = newInventoryObjectMesh.GetComponentsInChildren (typeof(Collider));
		foreach (Component c in colliders) {
			Type type = c.GetComponent<Collider> ().GetType ();

			Component copy = newEmptyInventoryPrefab.AddComponent (type);

			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

			PropertyInfo[] pinfos = type.GetProperties (flags);

			for (int i = 0; i < pinfos.Length; i++) {
				pinfos [i].SetValue (copy, pinfos [i].GetValue (c.GetComponent<Collider> (), null), null);
			}
		}

		foreach (Component c in colliders) {
			DestroyImmediate (c.GetComponent<Collider> ());
		}

		deviceStringAction currentDeviceStringAction = newEmptyInventoryPrefab.GetComponentInChildren<deviceStringAction> ();

		if (currentDeviceStringAction != null) {
			currentDeviceStringAction.setNewDeviceName (currentInventoryInfo.Name);
		}

		string relativePath = relativeInventoryPath + "/" + currentInventoryCategoryInfo.Name;

		if (!Directory.Exists (relativePath)) {
			print ("Inventory prefab folder " + currentInventoryCategoryInfo.Name + " doesn't exist, created a new one with that name");

			Directory.CreateDirectory (relativePath);
		}

		pickUpObject currentPickupObject = newEmptyInventoryPrefab.GetComponent<pickUpObject> ();

		if (currentPickupObject != null) {
			currentPickupObject.amount = 1;
		}

		inventoryPrefabCreationSystem currentInventoryPrefabCreationSystem = newEmptyInventoryPrefab.GetComponent<inventoryPrefabCreationSystem> ();

		if (currentInventoryPrefabCreationSystem != null) {
			currentInventoryPrefabCreationSystem.createInventoryPrefabObject ();

			DestroyImmediate (currentInventoryPrefabCreationSystem);
		}

		string prefabFilePath = relativePath + "/" + currentInventoryInfo.Name + " (inventory)" + ".prefab";

		bool prefabExists = false;
		if ((GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject)) != null) {
			prefabExists = true;
		}

		if (prefabExists) {
			UnityEngine.Object prefab = (GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject));
			PrefabUtility.ReplacePrefab (newEmptyInventoryPrefab, prefab, ReplacePrefabOptions.ReplaceNameBased);

			print ("Replacing prefab in path " + prefabFilePath);
		} else {
			UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab (prefabFilePath);
			PrefabUtility.ReplacePrefab (newEmptyInventoryPrefab, prefab, ReplacePrefabOptions.ConnectToPrefab);

			print ("Creating new prefab in path " + prefabFilePath);
		}

		currentInventoryInfo.inventoryObjectPrefab = (GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject));

		DestroyImmediate (newEmptyInventoryPrefab);

		updateComponent ();

		updateInventoryList ();

		checkIfUpdateInventoryListScriptableObjectOnCreateEditRemove ();

		#endif
	}

	public void updateInventoryPrefab (int categoryIndex, int index)
	{
		inventoryInfo currentInventoryInfo = inventoryCategoryInfoList [categoryIndex].inventoryList [index];

		if (currentInventoryInfo.inventoryObjectPrefab != null) {
			inventoryObject currentInventoryObject = currentInventoryInfo.inventoryObjectPrefab.GetComponentInChildren<inventoryObject> ();

			if (currentInventoryObject != null) {
				currentInventoryObject.inventoryObjectInfo = new inventoryInfo (currentInventoryInfo);

				deviceStringAction currentDeviceStringAction = currentInventoryInfo.inventoryObjectPrefab.GetComponentInChildren<deviceStringAction> ();

				if (currentDeviceStringAction != null) {
					currentDeviceStringAction.setNewDeviceName (currentInventoryInfo.Name);
				}

				GKC_Utils.updateComponent (currentInventoryObject);
	
				updateSingleInventoryList ();

				updateComponent ();

				checkIfUpdateInventoryListScriptableObjectOnCreateEditRemove ();

				print ("Inventory object " + currentInventoryInfo.Name + " updated in the list and its prefab");
			} else {
				print ("WARNING: the inventory object called " + currentInventoryInfo.Name + " hasn't an inventory object component attached, make sure it has it assigned");
			}
		} else {
			print ("WARNING: the inventory object called " + currentInventoryInfo.Name + " hasn't an inventory prefab created, make sure it is created");
		}
	}

	public void removeInventoryPrafab (int categoryIndex, int index)
	{
		#if UNITY_EDITOR
		inventoryCategoryInfo currentInventoryCategoryInfo = inventoryCategoryInfoList [categoryIndex];

		inventoryInfo currentInventoryInfo = currentInventoryCategoryInfo.inventoryList [index];

		if (currentInventoryInfo.inventoryObjectPrefab == null) {
			print ("The Inventory Object hasn't a prefab assigned");

			return;
		}

		string relativePath = relativeInventoryPath + "/" + currentInventoryCategoryInfo.Name;

		if (Directory.Exists (relativePath)) {
			
			string prefabFilePath = relativePath + "/" + currentInventoryInfo.Name + " (inventory)" + ".prefab";

			bool prefabExists = false;
			if ((GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject)) != null) {
				prefabExists = true;
			}

			if (prefabExists) {
				UnityEngine.Object prefab = (GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject));

//				prefab.name = "Inventory Object Removed (Delete)";

				print ("Removing prefab in path " + prefabFilePath);

				currentInventoryCategoryInfo.inventoryList.RemoveAt (index);

				updateComponent ();

				updateInventoryList ();

				if (removeInventoryPrefabWhenDeletingInventoryObject) {
					DestroyImmediate (prefab, true);
				}

				print ("Check the prefab called " + prefab.name + " to be removed in the Project window");
			} else {

				print ("Prefab doesn't exists in path " + prefabFilePath);
			}
		} else {
			print ("Inventory prefab folder " + currentInventoryCategoryInfo.Name + " doesn't exist, created a new one with that name");
		}

		#endif
	}

	public inventoryBankManager getMainInventoryBankManager ()
	{
		return mainInventoryBankManager;
	}

	public void addNewCategory ()
	{
		inventoryCategoryInfo newInventoryCategoryInfo = new inventoryCategoryInfo ();

		inventoryCategoryInfoList.Add (newInventoryCategoryInfo);

		updateComponent ();
	}

	public void removeCategory (int categoryIndex)
	{
		inventoryCategoryInfo currentInventoryCategoryInfo = inventoryCategoryInfoList [categoryIndex];

		for (int i = 0; i < currentInventoryCategoryInfo.inventoryList.Count; i++) {

			for (int j = 0; j < inventoryList.Count; j++) {
				if (inventoryList [j].Name.Equals (currentInventoryCategoryInfo.inventoryList [i].Name)) {
					inventoryList.RemoveAt (j);

					j = 0;
				}
			}
		}

		inventoryCategoryInfoList.RemoveAt (categoryIndex);

		updateSingleInventoryList ();

		updateComponent ();

		checkIfUpdateInventoryListScriptableObjectOnCreateEditRemove ();
	}

	public void removeAllCategories ()
	{
		inventoryCategoryInfoList.Clear ();

		inventoryList.Clear ();

		updateComponent ();

		checkIfUpdateInventoryListScriptableObjectOnCreateEditRemove ();
	}

	public void updateSingleInventoryList ()
	{
		inventoryList.Clear ();

		inventoryCategoryInfo currentInventoryCategoryInfo = null;
		inventoryInfo currentInventoryInfo = null;

		int inventoryCategoryInfoListCount = inventoryCategoryInfoList.Count;

		for (int i = 0; i < inventoryCategoryInfoListCount; i++) {

			currentInventoryCategoryInfo = inventoryCategoryInfoList [i];

			for (int j = 0; j < currentInventoryCategoryInfo.inventoryList.Count; j++) {

				currentInventoryInfo = currentInventoryCategoryInfo.inventoryList [j];

				currentInventoryInfo.categoryIndex = i;

				currentInventoryInfo.elementIndex = j;

				inventoryList.Add (currentInventoryInfo);
			}
		}

		GKC_Utils.refreshAssetDatabase ();
	}

	public void updateInventoryList ()
	{
		updateSingleInventoryList ();

		print ("\n\n");

		print ("UPDATING INVENTORY LIST");

		print ("\n\n");

		for (int i = 0; i < inventoryList.Count; i++) {
			if (inventoryList [i].inventoryObjectPrefab != null) {
				inventoryObject currentInventoryObject = inventoryList [i].inventoryObjectPrefab.GetComponentInChildren<inventoryObject> ();

				if (currentInventoryObject != null) {
					currentInventoryObject.inventoryObjectInfo = new inventoryInfo (inventoryList [i]);

					GKC_Utils.updateComponent (currentInventoryObject);

					print ("Inventory object " + inventoryList [i].Name + " updated in the list and its prefab");

					pickUpObject currentpickUpObject = inventoryList [i].inventoryObjectPrefab.GetComponentInChildren<pickUpObject> ();

					if (currentpickUpObject != null) {
						currentpickUpObject.assignPickupElementsOnEditor ();
					}

					deviceStringAction currentDeviceStringAction = inventoryList [i].inventoryObjectPrefab.GetComponentInChildren<deviceStringAction> ();

					if (currentDeviceStringAction != null) {
						currentDeviceStringAction.setNewDeviceName (currentInventoryObject.inventoryObjectInfo.Name);
					}
				} else {
					print ("WARNING: the inventory object called " + inventoryList [i].Name + " hasn't an inventory object component attached, make sure it has it assigned");
				}
			} else {
				print ("WARNING: the inventory object called " + inventoryList [i].Name + " hasn't an inventory prefab created, make sure it is created");
			}
		}

		print ("\n\n");

		print ("Inventory list and prefabs updated");

		print ("\n\n");
	
		updateComponent ();
	}

	public void updateCategory ()
	{
		inventoryCategoryInfo currentInventoryCategoryInfo = null;

		inventoryInfo currentInventoryInfo = null;

		int inventoryCategoryInfoListCount = inventoryCategoryInfoList.Count;

		for (int i = 0; i < inventoryCategoryInfoListCount; i++) {

			currentInventoryCategoryInfo = inventoryCategoryInfoList [i];

			for (int j = 0; j < currentInventoryCategoryInfo.inventoryList.Count; j++) {

				currentInventoryInfo = currentInventoryCategoryInfo.inventoryList [j];

				currentInventoryInfo.categoryName = currentInventoryCategoryInfo.Name;

				currentInventoryInfo.categoryIndex = i;

				currentInventoryInfo.elementIndex = j;
			}
		}

		updateComponent ();
	}

	public void addCopyOfInventoryObject (int categoryIndex, int index)
	{
		inventoryCategoryInfo currentInventoryCategoryInfo = inventoryCategoryInfoList [categoryIndex];
		inventoryInfo currentInventoryInfo = currentInventoryCategoryInfo.inventoryList [index];

		inventoryInfo newInventoryInfo = new inventoryInfo (currentInventoryInfo);

		currentInventoryCategoryInfo.inventoryList.Insert (index + 1, newInventoryInfo);

		updateComponent ();
	}

	public static GameObject spawnInventoryObject (GameObject currentInventoryObjectPrefab, Transform positionToInstantiate,
	                                               int amount, inventoryInfo newInventoryInfo)
	{
//		print (currentInventoryObjectPrefab.name);

		GameObject inventoryObjectClone = (GameObject)Instantiate (currentInventoryObjectPrefab, positionToInstantiate.position, positionToInstantiate.rotation);

		inventoryObject inventoryObjectManager = inventoryObjectClone.GetComponentInChildren<inventoryObject> ();

		if (inventoryObjectManager != null) {
			inventoryObjectManager.inventoryObjectInfo = new inventoryInfo (newInventoryInfo);

			if (inventoryObjectManager.inventoryObjectInfo.storeTotalAmountPerUnit) {
				inventoryObjectManager.inventoryObjectInfo.amountPerUnit = 1;
			}

			pickUpObject currentPickupObject = inventoryObjectClone.GetComponent<pickUpObject> ();

			currentPickupObject.amountPerUnit = newInventoryInfo.amountPerUnit;

			if (currentPickupObject.amountPerUnit > 0 && !inventoryObjectManager.inventoryObjectInfo.storeTotalAmountPerUnit) {
				currentPickupObject.useAmountPerUnit = true;
			}

			currentPickupObject.setPickUpAmount (amount);

			inventoryObjectClone.name = inventoryObjectManager.inventoryObjectInfo.Name + " (inventory)";

			inventoryObjectClone.GetComponentInChildren<deviceStringAction> ().setNewDeviceName (inventoryObjectManager.inventoryObjectInfo.Name);

			return inventoryObjectClone;
		}

		return null;
	}

	public GameObject spawnInventoryObjectByName (string objectName, int amount, Vector3 newPosition, Quaternion newRotation)
	{
		int objectIndex = inventoryList.FindIndex (s => s.Name.Equals (objectName));

		if (objectIndex != -1) {

			GameObject inventoryObjectClone = Instantiate (inventoryList [objectIndex].inventoryObjectPrefab, newPosition, newRotation);

			inventoryObject inventoryObjectManager = inventoryObjectClone.GetComponentInChildren<inventoryObject> ();

			if (inventoryObjectManager != null) {
				pickUpObject currentPickupObject = inventoryObjectClone.GetComponent<pickUpObject> ();

//				currentPickupObject.amountPerUnit = inventoryObjectManager.inventoryObjectInfo.amountPerUnit;
//
//				if (currentPickupObject.amountPerUnit > 0 && !inventoryObjectManager.inventoryObjectInfo.storeTotalAmountPerUnit) {
//					currentPickupObject.useAmountPerUnit = true;
//				}

				currentPickupObject.setPickUpAmount (amount);

				inventoryObjectClone.name = objectName + " (inventory)";

				inventoryObjectClone.GetComponentInChildren<deviceStringAction> ().setNewDeviceName (objectName);

				return inventoryObjectClone;
			}
		}

		return null;
	}

	public void saveInventoryListToFile ()
	{
		if (mainInventoryListManagerData != null) {
			mainInventoryListManagerData.inventoryList = new List<inventoryCategoryInfo> (inventoryCategoryInfoList);

			print ("Inventory list saved to file");

			updateComponent ();
		}
	}

	public void loadInventoryListFromFile ()
	{
		if (mainInventoryListManagerData != null) {
			inventoryCategoryInfoList = new List<inventoryCategoryInfo> (mainInventoryListManagerData.inventoryList);

			updateSingleInventoryList ();

			updateComponent ();

			print ("Inventory list loaded from file");
		}
	}

	public void checkIfUpdateInventoryListScriptableObjectOnCreateEditRemove ()
	{
		if (updateInventoryListScriptableObjectOnCreateEditRemove) {
			saveInventoryListToFile ();
		}
	}

	public void setInventoryCaptureToolOpenState (bool state)
	{
		inventoryCaptureToolOpen = state;

		Debug.Log ("Inventory Object Capture Icon window opened: " + inventoryCaptureToolOpen);

		updateComponent ();
	}

	public void addObjectInfoIntoInventoryList ()
	{
		if (newInventoryObjectToAddThroughEditor == null) {
			print ("WARNING: No Inventory Object Prefab was found, make sure to assign it properly");

			return;
		}

		inventoryObject currentInventoryObject = newInventoryObjectToAddThroughEditor.GetComponentInChildren<inventoryObject> ();

		if (currentInventoryObject != null) {
			inventoryInfo newInventoryObjectInfo = new inventoryInfo (currentInventoryObject.inventoryObjectInfo);

			int categoryIndex = -1;

			string categoryName = newInventoryObjectInfo.categoryName;

			if (useNewCategoryToAddObject && newCategoryToAddObject != "") {
				categoryName = newCategoryToAddObject;
			}

			categoryIndex = inventoryCategoryInfoList.FindIndex (s => s.Name.Equals (categoryName));

			if (categoryIndex > -1) {
				inventoryCategoryInfo currentInventoryCategoryInfo = inventoryCategoryInfoList [categoryIndex];

				int objectIndex = currentInventoryCategoryInfo.inventoryList.FindIndex (s => s.Name.Equals (newInventoryObjectInfo.Name));

				if (objectIndex == -1) {
					newInventoryObjectInfo.categoryName = categoryName;

					newInventoryObjectInfo.elementIndex = currentInventoryCategoryInfo.inventoryList.Count + 1;

					newInventoryObjectInfo.inventoryObjectPrefab = newInventoryObjectToAddThroughEditor;

					currentInventoryCategoryInfo.inventoryList.Add (newInventoryObjectInfo);

					newInventoryObjectToAddThroughEditor = null;

					GKC_Utils.updateComponent (currentInventoryObject);

					updateComponent ();

					updateInventoryList ();

					checkIfUpdateInventoryListScriptableObjectOnCreateEditRemove ();

					print ("Adding inventory info " + newInventoryObjectInfo.Name + " from inventory object prefab to the main inventory list manager, " +
					"on the category " + categoryName);

				} else {
					print ("Object " + newInventoryObjectInfo.Name + " already exists on the main inventory list manager, on the category " + categoryName);
				}
			} else {
				print ("Category " + categoryName + " wasn't found, make sure to configure a new category with that name or use an already existing" +
				" category for it");
			}
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Updating main inventory list manager ", gameObject);
	}
}
