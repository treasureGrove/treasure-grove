using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(vendorSystem), true)]
public class vendorSystemEditor : Editor
{
	SerializedProperty useGeneralBuyPriceMultiplier;
	SerializedProperty generalBuyPriceMultiplayerPercentage;
	SerializedProperty useGeneralSellPriceMultiplier;
	SerializedProperty generalSellPriceMultiplayerPercentage;
	SerializedProperty infiniteVendorAmountAvailable;
	SerializedProperty gameSystemManager;
	SerializedProperty mainInventoryManager;
	SerializedProperty inventoryListManagerList;
	SerializedProperty vendorInventoryList;
	SerializedProperty loadCurrentVendorInventoryFromSaveFile;
	SerializedProperty saveCurrentVendorInventoryToSaveFile;

	vendorSystem manager;

	int categoryIndex;
	int elementIndex;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		useGeneralBuyPriceMultiplier = serializedObject.FindProperty ("useGeneralBuyPriceMultiplier");
		generalBuyPriceMultiplayerPercentage = serializedObject.FindProperty ("generalBuyPriceMultiplayerPercentage");
		useGeneralSellPriceMultiplier = serializedObject.FindProperty ("useGeneralSellPriceMultiplier");
		generalSellPriceMultiplayerPercentage = serializedObject.FindProperty ("generalSellPriceMultiplayerPercentage");
		infiniteVendorAmountAvailable = serializedObject.FindProperty ("infiniteVendorAmountAvailable");
		gameSystemManager = serializedObject.FindProperty ("gameSystemManager");
		mainInventoryManager = serializedObject.FindProperty ("mainInventoryManager");
		inventoryListManagerList = serializedObject.FindProperty ("inventoryListManagerList");
		vendorInventoryList = serializedObject.FindProperty ("vendorInventoryList");
		loadCurrentVendorInventoryFromSaveFile = serializedObject.FindProperty ("loadCurrentVendorInventoryFromSaveFile");
		saveCurrentVendorInventoryToSaveFile = serializedObject.FindProperty ("saveCurrentVendorInventoryToSaveFile");

		manager = (vendorSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (useGeneralBuyPriceMultiplier);
		if (useGeneralBuyPriceMultiplier.boolValue) {
			EditorGUILayout.PropertyField (generalBuyPriceMultiplayerPercentage);
		}
		EditorGUILayout.PropertyField (useGeneralSellPriceMultiplier);
		if (useGeneralSellPriceMultiplier.boolValue) {
			EditorGUILayout.PropertyField (generalSellPriceMultiplayerPercentage);
		}
		EditorGUILayout.PropertyField (infiniteVendorAmountAvailable);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Inventory Elements", "window");
		EditorGUILayout.PropertyField (gameSystemManager);
		EditorGUILayout.PropertyField (mainInventoryManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory List Manager List", "window");
		showInventoryListManagerList (inventoryListManagerList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get Inventory Manager List")) {
			manager.getInventoryListManagerList ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Inventory Object List Names")) {
			manager.setInventoryObjectListNames ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Vendor Inventory List (DEBUG)", "window");
		showInventoryList (vendorInventoryList);
		GUILayout.EndVertical ();	

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Save/Load Vendor Inventory Settings", "window");
		EditorGUILayout.PropertyField (loadCurrentVendorInventoryFromSaveFile);
		EditorGUILayout.PropertyField (saveCurrentVendorInventoryToSaveFile);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Save Inventory Vendor List")) {
			manager.saveCurrentVendorListToFile ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showInventoryListElementInfo (SerializedProperty list, bool expanded, int index)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("inventoryGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectInfo"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("icon"));

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

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmount"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amount"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountPerUnit"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeUsed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeEquiped"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeDropped"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeCombined"));
		if (list.FindPropertyRelative ("canBeCombined").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToCombine"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("combinedObject"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("combinedObjectMessage"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeDiscarded"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("vendorPrice"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sellPrice"));
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isWeapon"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("isArmorClothAccessory"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("canBeExamined"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("button"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("menuIconElement"));
		GUILayout.EndVertical ();
	}

	void showInventoryList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize);

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

					string amountValue = " - " + list.GetArrayElementAtIndex (i).FindPropertyRelative ("amount").intValue;
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent (list.GetArrayElementAtIndex (i).displayName + amountValue), false);

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showInventoryListElementInfo (list.GetArrayElementAtIndex (i), expanded, i);
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
					list.DeleteArrayElementAtIndex (i);
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

	void showInventoryListManagerListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("infiniteAmount"));
		if (!list.FindPropertyRelative ("infiniteAmount").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("amount"));
		}

		if (manager != null && manager.inventoryManagerListString.Length > 0) {
			list.FindPropertyRelative ("categoryIndex").intValue = EditorGUILayout.Popup ("Category", list.FindPropertyRelative ("categoryIndex").intValue, manager.inventoryManagerListString);

			categoryIndex = list.FindPropertyRelative ("categoryIndex").intValue;
			if (categoryIndex < manager.inventoryManagerListString.Length) {
				list.FindPropertyRelative ("inventoryCategoryName").stringValue = manager.inventoryManagerListString [categoryIndex];
			}

			if (manager.inventoryManagerStringInfoList.Count > 0) {
				list.FindPropertyRelative ("elementIndex").intValue = EditorGUILayout.Popup ("Inventory Object", list.FindPropertyRelative ("elementIndex").intValue, 
					manager.inventoryManagerStringInfoList [categoryIndex].inventoryManagerListString);

				elementIndex = list.FindPropertyRelative ("elementIndex").intValue;
				if (elementIndex < manager.inventoryManagerStringInfoList [categoryIndex].inventoryManagerListString.Length) {
					list.FindPropertyRelative ("inventoryObjectName").stringValue = manager.inventoryManagerStringInfoList [categoryIndex].inventoryManagerListString [elementIndex];
				}
			}

			if (!useGeneralBuyPriceMultiplier.boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("vendorPrice"));
			}

			if (!useGeneralSellPriceMultiplier.boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("sellPrice"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMinLevelToBuy"));
			if (list.FindPropertyRelative ("useMinLevelToBuy").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("minLevelToBuy"));
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("addObjectToList"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("spawnObject"));
		}

		GUILayout.EndVertical ();
	}

	void showInventoryListManagerList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Inventory Manager List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure every inventory object of this Vendor", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize);

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				manager.addNewInventoryObjectToInventoryListManagerList ();
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
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

					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);

					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showInventoryListManagerListElement (list.GetArrayElementAtIndex (i));
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
					list.DeleteArrayElementAtIndex (i);
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
}
#endif