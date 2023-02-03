using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(inventoryListManager), true)]
public class inventoryListManagerEditor : Editor
{
	SerializedProperty relativePathCaptures;
	SerializedProperty inventoryCamera;
	SerializedProperty lookObjectsPosition;
	SerializedProperty inventoryCameraParentGameObject;
	SerializedProperty emptyInventoryPrefab;
	SerializedProperty relativeInventoryPath;
	SerializedProperty mainInventoryBankManager;
	SerializedProperty inventoryCategoryInfoList;
	SerializedProperty inventoryListMessage;
	SerializedProperty inventoryList;
	SerializedProperty inventoryListUpdateMessage;
	SerializedProperty mainInventoryListManagerData;

	SerializedProperty removeInventoryPrefabWhenDeletingInventoryObject;

	SerializedProperty newInventoryObjectToAddThroughEditor;

	SerializedProperty useNewCategoryToAddObject;
	SerializedProperty newCategoryToAddObject;

	SerializedProperty updateInventoryListScriptableObjectOnCreateEditRemove;

	SerializedProperty mainFullCraftingBlueprintInfoTemplateData;

	inventoryListManager inventory;
	inventoryCaptureManager inventoryWindow;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		relativePathCaptures = serializedObject.FindProperty ("relativePathCaptures");
		inventoryCamera = serializedObject.FindProperty ("inventoryCamera");
		lookObjectsPosition = serializedObject.FindProperty ("lookObjectsPosition");
		inventoryCameraParentGameObject = serializedObject.FindProperty ("inventoryCameraParentGameObject");
		emptyInventoryPrefab = serializedObject.FindProperty ("emptyInventoryPrefab");
		relativeInventoryPath = serializedObject.FindProperty ("relativeInventoryPath");
		mainInventoryBankManager = serializedObject.FindProperty ("mainInventoryBankManager");
		inventoryCategoryInfoList = serializedObject.FindProperty ("inventoryCategoryInfoList");
		inventoryListMessage = serializedObject.FindProperty ("inventoryListMessage");
		inventoryList = serializedObject.FindProperty ("inventoryList");
		inventoryListUpdateMessage = serializedObject.FindProperty ("inventoryListUpdateMessage");
		mainInventoryListManagerData = serializedObject.FindProperty ("mainInventoryListManagerData");

		removeInventoryPrefabWhenDeletingInventoryObject = serializedObject.FindProperty ("removeInventoryPrefabWhenDeletingInventoryObject");

		newInventoryObjectToAddThroughEditor = serializedObject.FindProperty ("newInventoryObjectToAddThroughEditor");
		useNewCategoryToAddObject = serializedObject.FindProperty ("useNewCategoryToAddObject");
		newCategoryToAddObject = serializedObject.FindProperty ("newCategoryToAddObject");

		updateInventoryListScriptableObjectOnCreateEditRemove = serializedObject.FindProperty ("updateInventoryListScriptableObjectOnCreateEditRemove");

		mainFullCraftingBlueprintInfoTemplateData = serializedObject.FindProperty ("mainFullCraftingBlueprintInfoTemplateData");

		inventory = (inventoryListManager)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Capture Tool Settings", "window");
		EditorGUILayout.PropertyField (relativePathCaptures);
		EditorGUILayout.PropertyField (inventoryCamera);
		EditorGUILayout.PropertyField (lookObjectsPosition);
		EditorGUILayout.PropertyField (inventoryCameraParentGameObject);
		GUILayout.EndVertical ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Prefabs Settings", "window");	
		EditorGUILayout.PropertyField (emptyInventoryPrefab);
		EditorGUILayout.PropertyField (relativeInventoryPath);
		EditorGUILayout.PropertyField (removeInventoryPrefabWhenDeletingInventoryObject);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Bank Settings", "window");
		EditorGUILayout.PropertyField (mainInventoryBankManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Category Inventory List", "window");
		showInventoryCategoryInfoList (inventoryCategoryInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory List (DEBUG)", "window");

		EditorGUILayout.PropertyField (inventoryListMessage);	

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		showInventoryList (inventoryList, 0, false);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Update Inventory List Info", "window");

		EditorGUILayout.PropertyField (inventoryListUpdateMessage);	

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Inventory List")) {
			inventory.updateInventoryList ();

			inventory.checkIfUpdateInventoryListScriptableObjectOnCreateEditRemove ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Save/Load Inventory List To File", "window");
		EditorGUILayout.PropertyField (mainInventoryListManagerData);	

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (updateInventoryListScriptableObjectOnCreateEditRemove);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Save Inventory List To File")) {
			inventory.saveInventoryListToFile ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Load Inventory List From File")) {
			inventory.loadInventoryListFromFile ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Add Inventory Object Info To Main List", "window");
		EditorGUILayout.PropertyField (newInventoryObjectToAddThroughEditor);
		EditorGUILayout.PropertyField (useNewCategoryToAddObject);
		if (useNewCategoryToAddObject.boolValue) {
			EditorGUILayout.PropertyField (newCategoryToAddObject);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Object Info Into Inventory List")) {
			inventory.addObjectInfoIntoInventoryList ();
		}
	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Full Crafting Blueprint Template Data List", "window");

		EditorGUILayout.PropertyField (mainFullCraftingBlueprintInfoTemplateData);	

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showInventoryList (SerializedProperty list, int categoryIndex, bool canBeEdited)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Inventory List List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Objects: " + list.arraySize);

			EditorGUILayout.Space ();

			if (canBeEdited) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Add Object")) {
					inventory.addNewInventoryObject (categoryIndex);
				}
				if (GUILayout.Button ("Clear")) {
					inventory.removeCategory (categoryIndex);
				}
				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				if (canBeEdited) {
					GUILayout.BeginHorizontal ();
				}

				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						expanded = true;
						showInventoryListElement (currentArrayElement, expanded, categoryIndex, i, canBeEdited);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}

				if (canBeEdited) {
					GUILayout.EndHorizontal ();

					if (expanded) {
						GUILayout.BeginVertical ();
					} else {
						GUILayout.BeginHorizontal ();
					}
					if (GUILayout.Button ("x")) {
						inventory.removeInventoryObject (categoryIndex, i);
					}
					if (GUILayout.Button ("v")) {
						if (i >= 0) {
							list.MoveArrayElement (i, i + 1);
						}
					}
					if (GUILayout.Button ("^")) {
						if (i < list.arraySize) {
							list.MoveArrayElement (i, i - 1);
						}
					}
					if (expanded) {
						GUILayout.EndVertical ();
					} else {
						GUILayout.EndHorizontal ();
					}
				}

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showInventoryListElement (SerializedProperty list, bool expanded, int categoryIndex, int index, bool canBeEdited)
	{
		if (canBeEdited) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Main Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectInfo"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("inventoryGameObject"), new GUIContent ("Inventory Object Mesh"), false);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Icon Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("icon"));

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Object Icon Preview \t");
			GUILayout.BeginVertical ("box", GUILayout.Height (50), GUILayout.Width (50));
			if (list.FindPropertyRelative ("icon").objectReferenceValue && expanded) {
				Object texture = list.FindPropertyRelative ("icon").objectReferenceValue as Texture2D;
				Texture2D myTexture = AssetPreview.GetAssetPreview (texture);
				GUILayout.Label (myTexture, GUILayout.Width (50), GUILayout.Height (50));
			}
			GUILayout.EndVertical ();
			GUILayout.Label ("");
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Open Inventory Capture Tool")) {
				if (inventory.inventoryCaptureToolOpen) {
					Debug.Log ("Inventory Object Capture Icon is already opened");
				} else {
					inventoryWindow = (inventoryCaptureManager)EditorWindow.GetWindow (typeof(inventoryCaptureManager));
					inventoryWindow.init ();
					inventoryWindow.setCurrentInventoryObjectInfo (inventory.inventoryCategoryInfoList [categoryIndex].inventoryList [index], inventory.getDataPath ());
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Amount Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountPerUnit"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmount"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("storeTotalAmountPerUnit"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("showAmountPerUnitInAmountText"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Use Object Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsed"));
			if (list.FindPropertyRelative ("canBeUsed").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewBehaviorOnUse"));
				if (list.FindPropertyRelative ("useNewBehaviorOnUse").boolValue) {

					GUILayout.Label ("Tip: Use -AMOUNT- in the position of the text \n to replace the amount used if you need it");
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("newBehaviorOnUseMessage"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("newBehaviorOnUnableToUseMessage"));
				}

				EditorGUILayout.Space ();

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useSoundOnUseObject"));
				if (list.FindPropertyRelative ("useSoundOnUseObject").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("soundOnUseObject"));
				}
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Equip Object Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeEquiped"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Drop Object Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeDropped"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeDiscarded"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Combine Object Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeCombined"));
			if (list.FindPropertyRelative ("canBeCombined").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewBehaviorOnCombine"));
				if (list.FindPropertyRelative ("useNewBehaviorOnCombine").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("useOneUnitOnNewBehaviourCombine"));

					EditorGUILayout.Space ();

					GUILayout.Label ("Tip: Use -OBJECT- in the position of the text \n to replace the second combined object name used if you need it");
					GUILayout.Label ("Also, use -AMOUNT- in the position of the text \n to replace the amount used if you need it");
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("newBehaviorOnCombineMessage"));
				}

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToCombine"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("combinedObject"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("combinedObjectMessage"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Vendor Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeSold"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("vendorPrice"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sellPrice"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMinLevelToBuy"));
			if (list.FindPropertyRelative ("useMinLevelToBuy").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("minLevelToBuy"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Durability Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDurability"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("durabilityAmount"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDurabilityAmount"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("sellMultiplierIfObjectIsBroken"));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Max Amount Per Slot Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("setMaximumAmountPerSlot"));
			if (list.FindPropertyRelative ("setMaximumAmountPerSlot").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("maximumAmountPerSlot"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Other Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isWeapon"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isMeleeWeapon"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isArmorClothAccessory"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("categoryName"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("weight"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("cantBeStoredOnInventory"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeHeld"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBePlaceOnQuickAccessSlot"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeExamined"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("hideSlotOnMenu"));

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Object To Drop Settings", "window");

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomObjectToDrop"));
			if (list.FindPropertyRelative ("useCustomObjectToDrop").boolValue) {

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("customObjectToDrop"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("dropObjectOffset"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Prefab Settings", "window");

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("emptyInventoryPrefab"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("inventoryObjectPrefab"));

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Create Inventory Prefab", buttonStyle)) {
				inventory.createInventoryPrafab (categoryIndex, index);
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Update Inventory Prefab", buttonStyle)) {
				inventory.updateInventoryPrefab (categoryIndex, index);
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Add Copy of this Inventory Object", buttonStyle)) {
				inventory.addCopyOfInventoryObject (categoryIndex, index);
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Remove Inventory Object And Prefab", buttonStyle)) {
				inventory.removeInventoryPrafab (categoryIndex, index);
			}

			EditorGUILayout.Space ();
		} else {
			GUILayout.BeginVertical ("Object Info", "window");
			GUILayout.Label ("Name \t\t\t" + list.FindPropertyRelative ("Name").stringValue);
		
			EditorGUILayout.Space ();
	
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Object Icon Preview \t");
			GUILayout.BeginHorizontal ("box", GUILayout.Height (50), GUILayout.Width (50));
			if (list.FindPropertyRelative ("icon").objectReferenceValue && expanded) {
				Object texture = list.FindPropertyRelative ("icon").objectReferenceValue as Texture2D;
				Texture2D myTexture = AssetPreview.GetAssetPreview (texture);
				GUILayout.Label (myTexture, GUILayout.Width (50), GUILayout.Height (50));
			}
			GUILayout.EndHorizontal ();
			GUILayout.Label ("");
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}
	}

	void showInventoryCategoryInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Inventory Category Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Categories: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Category")) {
				inventory.addNewCategory ();
			}
			if (GUILayout.Button ("Clear")) {
				inventory.removeAllCategories ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						expanded = true;
						showInventoryCategoryInfoListElement (currentArrayElement, i);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					inventory.removeCategory (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showInventoryCategoryInfoListElement (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("cateogryTexture"));

		if (list.FindPropertyRelative ("cateogryTexture").objectReferenceValue) {
			EditorGUILayout.Space ();

			GUILayout.Label ("Category Icon Preview \t");
			GUILayout.BeginHorizontal ("box", GUILayout.Height (50), GUILayout.Width (50));
			if (list.FindPropertyRelative ("cateogryTexture").objectReferenceValue) {
				Object texture = list.FindPropertyRelative ("cateogryTexture").objectReferenceValue as Texture2D;
				Texture2D myTexture = AssetPreview.GetAssetPreview (texture);
				GUILayout.Label (myTexture, GUILayout.Width (50), GUILayout.Height (50));
			}
			GUILayout.EndHorizontal ();
			GUILayout.Label ("");
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("emptyInventoryPrefab"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory List", "window");
		showInventoryList (list.FindPropertyRelative ("inventoryList"), index, true);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}
}
#endif