using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(inventoryBankSystem), true)]
public class inventoryBankSystemEditor : Editor
{
	SerializedProperty usingInventoryBank;
	SerializedProperty openBankDelay;
	SerializedProperty animationName;
	SerializedProperty openSound;
	SerializedProperty closeSound;
	SerializedProperty attachToTransformActive;
	SerializedProperty transformToAttach;
	SerializedProperty localOffset;
	SerializedProperty useAsVendorSystem;
	SerializedProperty useGeneralBuyPriceMultiplier;
	SerializedProperty generalBuyPriceMultiplayerPercentage;
	SerializedProperty useGeneralSellPriceMultiplier;
	SerializedProperty generalSellPriceMultiplayerPercentage;
	SerializedProperty infiniteVendorAmountAvailable;
	SerializedProperty positionToSpawnObjects;
	SerializedProperty useInventoryFromThisBank;
	SerializedProperty inventoryListManagerList;
	SerializedProperty bankInventoryList;
	SerializedProperty useEventOnInventoryEmpty;
	SerializedProperty eventOnInventoryEmpty;
	SerializedProperty repeatEventOnInventoryEmpty;
	SerializedProperty mainAnimation;
	SerializedProperty mainAudioSource;
	SerializedProperty mainInventoryManager;

	inventoryBankSystem manager;

	int categoryIndex;
	int elementIndex;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		usingInventoryBank = serializedObject.FindProperty ("usingInventoryBank");
		openBankDelay = serializedObject.FindProperty ("openBankDelay");
		animationName = serializedObject.FindProperty ("animationName");
		openSound = serializedObject.FindProperty ("openSound");
		closeSound = serializedObject.FindProperty ("closeSound");
		attachToTransformActive = serializedObject.FindProperty ("attachToTransformActive");
		transformToAttach = serializedObject.FindProperty ("transformToAttach");
		localOffset = serializedObject.FindProperty ("localOffset");
		useAsVendorSystem = serializedObject.FindProperty ("useAsVendorSystem");
		useGeneralBuyPriceMultiplier = serializedObject.FindProperty ("useGeneralBuyPriceMultiplier");
		generalBuyPriceMultiplayerPercentage = serializedObject.FindProperty ("generalBuyPriceMultiplayerPercentage");
		useGeneralSellPriceMultiplier = serializedObject.FindProperty ("useGeneralSellPriceMultiplier");
		generalSellPriceMultiplayerPercentage = serializedObject.FindProperty ("generalSellPriceMultiplayerPercentage");
		infiniteVendorAmountAvailable = serializedObject.FindProperty ("infiniteVendorAmountAvailable");
		positionToSpawnObjects = serializedObject.FindProperty ("positionToSpawnObjects");
		useInventoryFromThisBank = serializedObject.FindProperty ("useInventoryFromThisBank");
		inventoryListManagerList = serializedObject.FindProperty ("inventoryListManagerList");
		bankInventoryList = serializedObject.FindProperty ("bankInventoryList");
		useEventOnInventoryEmpty = serializedObject.FindProperty ("useEventOnInventoryEmpty");
		eventOnInventoryEmpty = serializedObject.FindProperty ("eventOnInventoryEmpty");
		repeatEventOnInventoryEmpty = serializedObject.FindProperty ("repeatEventOnInventoryEmpty");
		mainAnimation = serializedObject.FindProperty ("mainAnimation");
		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");

		mainInventoryManager = serializedObject.FindProperty ("mainInventoryManager");

		manager = (inventoryBankSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (usingInventoryBank);
		EditorGUILayout.PropertyField (openBankDelay);
		EditorGUILayout.PropertyField (animationName);
		EditorGUILayout.PropertyField (openSound);
		EditorGUILayout.PropertyField (closeSound);
		GUILayout.EndVertical ();

		EditorGUILayout.Space (); 

		GUILayout.BeginVertical ("Attach To Transform Settings", "window");
		EditorGUILayout.PropertyField (attachToTransformActive);
		if (attachToTransformActive.boolValue) {
			EditorGUILayout.PropertyField (transformToAttach);
			EditorGUILayout.PropertyField (localOffset);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space (); 

		GUILayout.BeginVertical ("Vendor Settings", "window");
		EditorGUILayout.PropertyField (useAsVendorSystem);
		if (useAsVendorSystem.boolValue) {
			EditorGUILayout.PropertyField (useGeneralBuyPriceMultiplier);
			if (useGeneralBuyPriceMultiplier.boolValue) {
				EditorGUILayout.PropertyField (generalBuyPriceMultiplayerPercentage);
			}
			EditorGUILayout.PropertyField (useGeneralSellPriceMultiplier);
			if (useGeneralSellPriceMultiplier.boolValue) {
				EditorGUILayout.PropertyField (generalSellPriceMultiplayerPercentage);
			}
			EditorGUILayout.PropertyField (infiniteVendorAmountAvailable);

			EditorGUILayout.PropertyField (positionToSpawnObjects);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Custom Inventory Bank Settings", "window");
		EditorGUILayout.PropertyField (useInventoryFromThisBank);

		EditorGUILayout.Space ();

		if (useInventoryFromThisBank.boolValue) {
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

			GUILayout.BeginVertical ("Bank Inventory List", "window");
			showInventoryList (bankInventoryList);
			GUILayout.EndVertical ();	

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Empty Inventory Event Settings", "window");
		EditorGUILayout.PropertyField (useEventOnInventoryEmpty);
		if (useEventOnInventoryEmpty.boolValue) {
			EditorGUILayout.PropertyField (eventOnInventoryEmpty);
			EditorGUILayout.PropertyField (repeatEventOnInventoryEmpty);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (mainAnimation);
		EditorGUILayout.PropertyField (mainAudioSource);
		EditorGUILayout.PropertyField (mainInventoryManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

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

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("categoryName"));

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

		if (manager.inventoryManagerListString.Length > 0) {
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
		}

		if (useAsVendorSystem.boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMinLevelToBuy"));
			if (list.FindPropertyRelative ("useMinLevelToBuy").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("minLevelToBuy"));
			}

			if (!useGeneralBuyPriceMultiplier.boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("vendorPrice"));
			}

			if (!useGeneralSellPriceMultiplier.boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("sellPrice"));
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
			EditorGUILayout.HelpBox ("Configure every inventory object of this Bank", MessageType.None);
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