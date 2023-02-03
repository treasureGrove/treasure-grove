using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class createInventoryObjectEditor : EditorWindow
{
	[MenuItem ("Game Kit Controller/Create New Inventory Object", false, 20)]
	static void createNewInventoryObject ()
	{
		openCreateNewInventoryObjectEditor ();
	}

	public static void openCreateNewInventoryObjectEditor ()
	{
		string mainInventoryManagerName = "Main Inventory Manager";

		inventoryListManager mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

		string prefabsPath = pathInfoValues.getMainInventoryManagerPath ();

		if (mainInventoryListManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainInventoryManagerName, typeof(inventoryListManager));

			mainInventoryListManager = FindObjectOfType<inventoryListManager> ();
		}

		if (mainInventoryListManager == null) {
			GameObject mainInventoryManagerPrefab = (GameObject)AssetDatabase.LoadAssetAtPath (prefabsPath, typeof(GameObject));

			if (mainInventoryManagerPrefab != null) {
				
				GameObject newMainInventoryManager = (GameObject)Instantiate (mainInventoryManagerPrefab, Vector3.zero, Quaternion.identity);
				newMainInventoryManager.name = mainInventoryManagerPrefab.name;

				mainInventoryListManager = newMainInventoryManager.GetComponent<inventoryListManager> ();
			}
		}

		if (mainInventoryListManager != null) {
			GKC_Utils.setActiveGameObjectInEditor (mainInventoryListManager.gameObject);

			CollapseInspectorEditor.CollapseAllComponentsButOne (mainInventoryListManager.gameObject, typeof(inventoryListManager));
		} else {
			Debug.Log ("WARNING: Main Inventory Manager wasn't located at the path " + prefabsPath);
		}
	}
}