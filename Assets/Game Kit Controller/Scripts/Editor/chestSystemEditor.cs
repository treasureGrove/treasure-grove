using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(chestSystem))]
public class chestSystemEditor : Editor
{
	SerializedProperty chestPickUpList;

	SerializedProperty enablePickupsTriggerAtStart;
	SerializedProperty setNewPickupTriggerRadius;
	SerializedProperty newPickupTriggerRadius;

	SerializedProperty randomContent;
	SerializedProperty refillChestAfterDelay;
	SerializedProperty timeOpenedAfterEmpty;
	SerializedProperty refilledTime;
	SerializedProperty openAnimationName;
	SerializedProperty useElectronicDeviceManager;
	SerializedProperty isLocked;
	SerializedProperty openWhenUnlocked;
	SerializedProperty useEventOnOpenChest;
	SerializedProperty eventOnOpenChest;
	SerializedProperty useEventOnCloseChest;
	SerializedProperty eventOnCloseChest;
	SerializedProperty chestAnim;
	SerializedProperty mainCollider;
	SerializedProperty mapObjectInformationManager;
	SerializedProperty settings;
	SerializedProperty minAmount;
	SerializedProperty maxAmount;
	SerializedProperty numberOfObjects;
	SerializedProperty placeWhereInstantiatePickUps;
	SerializedProperty placeOffset;
	SerializedProperty space;
	SerializedProperty amount;
	SerializedProperty pickUpScale;

	SerializedProperty mainPickupManagerName;

	SerializedProperty useEventToSendSpawnedObjects;
	SerializedProperty eventToSendSpawnedObjects;

	SerializedProperty showGizmo;
	SerializedProperty gizmoColor;
	SerializedProperty gizmoLabelColor;
	SerializedProperty gizmoRadius;

	chestSystem manager;
	GUIStyle style = new GUIStyle ();
	Color buttonColor;

	int currentTypeIndex;
	int currentNameIndex;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		enablePickupsTriggerAtStart = serializedObject.FindProperty ("enablePickupsTriggerAtStart");
		setNewPickupTriggerRadius = serializedObject.FindProperty ("setNewPickupTriggerRadius");
		newPickupTriggerRadius = serializedObject.FindProperty ("newPickupTriggerRadius");

		chestPickUpList = serializedObject.FindProperty ("chestPickUpList");
		randomContent = serializedObject.FindProperty ("randomContent");
		refillChestAfterDelay = serializedObject.FindProperty ("refillChestAfterDelay");
		timeOpenedAfterEmpty = serializedObject.FindProperty ("timeOpenedAfterEmpty");
		refilledTime = serializedObject.FindProperty ("refilledTime");
		openAnimationName = serializedObject.FindProperty ("openAnimationName");
		useElectronicDeviceManager = serializedObject.FindProperty ("useElectronicDeviceManager");
		isLocked = serializedObject.FindProperty ("isLocked");
		openWhenUnlocked = serializedObject.FindProperty ("openWhenUnlocked");
		useEventOnOpenChest = serializedObject.FindProperty ("useEventOnOpenChest");
		eventOnOpenChest = serializedObject.FindProperty ("eventOnOpenChest");
		useEventOnCloseChest = serializedObject.FindProperty ("useEventOnCloseChest");
		eventOnCloseChest = serializedObject.FindProperty ("eventOnCloseChest");
		chestAnim = serializedObject.FindProperty ("chestAnim");
		mainCollider = serializedObject.FindProperty ("mainCollider");
		mapObjectInformationManager = serializedObject.FindProperty ("mapObjectInformationManager");
		settings = serializedObject.FindProperty ("settings");
		minAmount = serializedObject.FindProperty ("minAmount");
		maxAmount = serializedObject.FindProperty ("maxAmount");
		numberOfObjects = serializedObject.FindProperty ("numberOfObjects");
		placeWhereInstantiatePickUps = serializedObject.FindProperty ("placeWhereInstantiatePickUps");
		placeOffset = serializedObject.FindProperty ("placeOffset");
		space = serializedObject.FindProperty ("space");
		amount = serializedObject.FindProperty ("amount");
		pickUpScale = serializedObject.FindProperty ("pickUpScale");

		mainPickupManagerName = serializedObject.FindProperty ("mainPickupManagerName");

		useEventToSendSpawnedObjects = serializedObject.FindProperty ("useEventToSendSpawnedObjects");
		eventToSendSpawnedObjects = serializedObject.FindProperty ("eventToSendSpawnedObjects");

		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoColor = serializedObject.FindProperty ("gizmoColor");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");
		gizmoRadius = serializedObject.FindProperty ("gizmoRadius");

		manager = (chestSystem)target;
	}

	void OnSceneGUI ()
	{   
		if (!Application.isPlaying && manager.showGizmo) {
			Vector3 originalPosition = manager.placeWhereInstantiatePickUps.position
			                           + manager.placeWhereInstantiatePickUps.right * manager.placeOffset.x
			                           + manager.placeWhereInstantiatePickUps.up * manager.placeOffset.y
			                           + manager.placeWhereInstantiatePickUps.forward * manager.placeOffset.z;
			
			Vector3 currentPosition = originalPosition;

			//the original x and z values, to make rows of the objects
			int rows = 0;

			int columns = 0;

			style.normal.textColor = manager.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;

			// Set the local position of every object, so every object is actually inside the chest
			for (int i = 0; i < manager.numberOfObjects; i++) {	
				
				Handles.Label (currentPosition, (i + 1).ToString (), style);
				currentPosition += manager.placeWhereInstantiatePickUps.right * manager.space.x;

				if (i != 0 && (i + 1) % Mathf.Round (manager.amount.y) == 0) {
					currentPosition = originalPosition + manager.placeWhereInstantiatePickUps.up * (manager.space.y * columns);
					rows++;
					currentPosition -= manager.placeWhereInstantiatePickUps.forward * (manager.space.z * rows);
				}

				if (rows == Mathf.Round (manager.amount.x)) {
					columns++;
					currentPosition = originalPosition + manager.placeWhereInstantiatePickUps.up * (manager.space.y * columns);
					rows = 0;
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (randomContent);
		EditorGUILayout.PropertyField (refillChestAfterDelay);
		if (refillChestAfterDelay.boolValue) {
			EditorGUILayout.PropertyField (timeOpenedAfterEmpty);
			EditorGUILayout.PropertyField (refilledTime);
		}
		EditorGUILayout.PropertyField (openAnimationName);
		EditorGUILayout.PropertyField (useElectronicDeviceManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Pickups Trigger Settings", "window");
		EditorGUILayout.PropertyField (setNewPickupTriggerRadius);
		EditorGUILayout.PropertyField (newPickupTriggerRadius);
		EditorGUILayout.PropertyField (enablePickupsTriggerAtStart);
		EditorGUILayout.PropertyField (mainPickupManagerName);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Chest Pickups List", "window");
		showChestPickupTypeList (chestPickUpList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Locked Settings", "window");
		EditorGUILayout.PropertyField (isLocked);
		if (isLocked.boolValue) {
			EditorGUILayout.PropertyField (openWhenUnlocked);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (useEventOnOpenChest);
		if (useEventOnOpenChest.boolValue) {
			EditorGUILayout.PropertyField (eventOnOpenChest);
		}
		EditorGUILayout.PropertyField (useEventOnCloseChest);
		if (useEventOnCloseChest.boolValue) {
			EditorGUILayout.PropertyField (eventOnCloseChest);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventToSendSpawnedObjects);
		if (useEventToSendSpawnedObjects.boolValue) {
			EditorGUILayout.PropertyField (eventToSendSpawnedObjects);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (chestAnim);
		EditorGUILayout.PropertyField (mainCollider);
		EditorGUILayout.PropertyField (mapObjectInformationManager);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		buttonColor = GUI.backgroundColor;
		EditorGUILayout.BeginVertical ();
		string inputListOpenedText = "";
		if (settings.boolValue) {
			GUI.backgroundColor = Color.gray;
			inputListOpenedText = "Hide Settings";
		} else {
			GUI.backgroundColor = buttonColor;
			inputListOpenedText = "Show Settings";
		}
		if (GUILayout.Button (inputListOpenedText)) {
			settings.boolValue = !settings.boolValue;
		}
		GUI.backgroundColor = buttonColor;
		EditorGUILayout.EndHorizontal ();
		if (settings.boolValue) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Configure the position and offset for every pickup to spawn, along with number of rows and columns", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			if (randomContent.boolValue) {
				GUILayout.Label ("Number of pickups to spawn (random): \t" + "(Min) " + minAmount.intValue
				+ " + " + " (Max) " + maxAmount.intValue + " = " + numberOfObjects.intValue);
			} else {
				GUILayout.Label ("Number of pickups to spawn: \t" + numberOfObjects.intValue);
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (placeWhereInstantiatePickUps);
			EditorGUILayout.PropertyField (placeOffset);
			EditorGUILayout.PropertyField (space);
			EditorGUILayout.PropertyField (amount);
			EditorGUILayout.PropertyField (pickUpScale);

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Gizmo Settings", "window");
			EditorGUILayout.PropertyField (showGizmo);
			if (showGizmo.boolValue) {
				EditorGUILayout.PropertyField (gizmoColor);
				EditorGUILayout.PropertyField (gizmoLabelColor);
				EditorGUILayout.PropertyField (gizmoRadius);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get Pickup Manager List")) {
			manager.getManagerPickUpList ();
		}
		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showChestPickupTypeInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		if (manager.managerPickUpList.Count > 0) {
			currentTypeIndex = list.FindPropertyRelative ("typeIndex").intValue;

			currentTypeIndex = EditorGUILayout.Popup ("Pickup Type", currentTypeIndex, getTypeList ());
			if (currentTypeIndex < manager.managerPickUpList.Count) {
				list.FindPropertyRelative ("pickUpType").stringValue = manager.managerPickUpList [currentTypeIndex].pickUpType;
			}

			list.FindPropertyRelative ("typeIndex").intValue = currentTypeIndex;

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			showChestPickupList (list.FindPropertyRelative ("chestPickUpTypeList"), list.FindPropertyRelative ("pickUpType").stringValue, list.FindPropertyRelative ("typeIndex").intValue);

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
	}

	void showChestPickupTypeList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Chest Pickup List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();
		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Add the pickups for the chest here", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Pickups: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Pickup")) {
				list.arraySize++;
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
						showChestPickupTypeInfo (list.GetArrayElementAtIndex (i));
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

	void showChestPickupElementInfo (SerializedProperty list, int typeIndex)
	{
		if (manager.managerPickUpList [typeIndex].pickUpTypeList.Count > 0) {
			currentNameIndex = list.FindPropertyRelative ("nameIndex").intValue;

			currentNameIndex = EditorGUILayout.Popup ("Name", currentNameIndex, getNameList (typeIndex));

			if (currentNameIndex < manager.managerPickUpList.Count) {
				list.FindPropertyRelative ("name").stringValue = manager.managerPickUpList [typeIndex].pickUpTypeList [currentNameIndex].Name;
			}

			list.FindPropertyRelative ("nameIndex").intValue = currentNameIndex;
		}

		if (randomContent.boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountLimits"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("quantityLimits"));
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("amount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("quantity"));
		}
	}

	void showChestPickupList (SerializedProperty list, string pickUpType, int typeIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + pickUpType + " Type List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Pickups: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Pickup")) {
				list.arraySize++;
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
						showChestPickupElementInfo (list.GetArrayElementAtIndex (i), typeIndex);
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

	string[] getTypeList ()
	{
		string[] names = new string[manager.managerPickUpList.Count];

		int managerPickUpListCount = manager.managerPickUpList.Count;

		for (int i = 0; i < managerPickUpListCount; i++) {
			names [i] = manager.managerPickUpList [i].pickUpType;
		}

		return names;
	}

	string[] getNameList (int index)
	{
		string[] names = new string[manager.managerPickUpList [index].pickUpTypeList.Count];

		int pickUpTypeListCount = manager.managerPickUpList [index].pickUpTypeList.Count;

		for (int i = 0; i < pickUpTypeListCount; i++) {
			names [i] = manager.managerPickUpList [index].pickUpTypeList [i].Name;
		}

		return names;
	}
}
#endif