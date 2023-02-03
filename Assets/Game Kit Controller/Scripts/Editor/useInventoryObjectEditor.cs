using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(useInventoryObject))]
public class useInventoryObjectEditor : Editor
{
	SerializedProperty canBeReUsed;
	SerializedProperty useInventoryType;
	SerializedProperty useObjectsOneByOneUsingButton;
	SerializedProperty inventoryObjectAction;
	SerializedProperty disableObjectActionAfterUse;
	SerializedProperty tagToConfigure;
	SerializedProperty playerTag;
	SerializedProperty vehicleTag;
	SerializedProperty checkPlayerInventoryWhileDriving;

	SerializedProperty objectUsedMessage;

	SerializedProperty useCustomObjectNotFoundMessage;
	SerializedProperty objectNotFoundMessage;

	SerializedProperty enableObjectWhenActivate;
	SerializedProperty objectToEnable;
	SerializedProperty useAnimation;
	SerializedProperty objectWithAnimation;
	SerializedProperty animationName;
	SerializedProperty openPlayerInventoryMenu;
	SerializedProperty setNewInventoryPanel;
	SerializedProperty defaultInventoryPanelName;
	SerializedProperty newInventoryPanelName;
	SerializedProperty showDebugInfo;
	SerializedProperty objectUsed;
	SerializedProperty numberOfObjectsUsed;
	SerializedProperty numberOfObjectsNeeded;
	SerializedProperty currentNumberOfObjectsNeeded;
	SerializedProperty inventoryObjectNeededList;
	SerializedProperty unlockFunctionCall;
	SerializedProperty useEventOnUnlockWithDelay;
	SerializedProperty delayToActivateEvent;
	SerializedProperty eventOnUnlockWithDelay;

	SerializedProperty useUnlockEventFromLoadingGame;
	SerializedProperty unlockEventFromLoadingGame;

	SerializedProperty canUseAndUseInventoryPickupsOnTriggerEnabled;
	SerializedProperty removeInventoryPickupObjectOnTrigger;

	SerializedProperty deviceStringActionManager;
	SerializedProperty mainAudioSource;
	SerializedProperty mainCollider;

	useInventoryObject manager;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		canBeReUsed = serializedObject.FindProperty ("canBeReUsed");
		useInventoryType = serializedObject.FindProperty ("useInventoryType");
		useObjectsOneByOneUsingButton = serializedObject.FindProperty ("useObjectsOneByOneUsingButton");
		inventoryObjectAction = serializedObject.FindProperty ("inventoryObjectAction");
		disableObjectActionAfterUse = serializedObject.FindProperty ("disableObjectActionAfterUse");
		tagToConfigure = serializedObject.FindProperty ("tagToConfigure");
		playerTag = serializedObject.FindProperty ("playerTag");
		vehicleTag = serializedObject.FindProperty ("vehicleTag");
		checkPlayerInventoryWhileDriving = serializedObject.FindProperty ("checkPlayerInventoryWhileDriving");

		objectUsedMessage = serializedObject.FindProperty ("objectUsedMessage");

		useCustomObjectNotFoundMessage = serializedObject.FindProperty ("useCustomObjectNotFoundMessage"); 
		objectNotFoundMessage = serializedObject.FindProperty ("objectNotFoundMessage");

		enableObjectWhenActivate = serializedObject.FindProperty ("enableObjectWhenActivate");
		objectToEnable = serializedObject.FindProperty ("objectToEnable");
		useAnimation = serializedObject.FindProperty ("useAnimation");
		objectWithAnimation = serializedObject.FindProperty ("objectWithAnimation");
		animationName = serializedObject.FindProperty ("animationName");
		openPlayerInventoryMenu = serializedObject.FindProperty ("openPlayerInventoryMenu");
		setNewInventoryPanel = serializedObject.FindProperty ("setNewInventoryPanel");
		defaultInventoryPanelName = serializedObject.FindProperty ("defaultInventoryPanelName");
		newInventoryPanelName = serializedObject.FindProperty ("newInventoryPanelName");
		showDebugInfo = serializedObject.FindProperty ("showDebugInfo");
		objectUsed = serializedObject.FindProperty ("objectUsed");
		numberOfObjectsUsed = serializedObject.FindProperty ("numberOfObjectsUsed");
		numberOfObjectsNeeded = serializedObject.FindProperty ("numberOfObjectsNeeded");
		currentNumberOfObjectsNeeded = serializedObject.FindProperty ("currentNumberOfObjectsNeeded");
		inventoryObjectNeededList = serializedObject.FindProperty ("inventoryObjectNeededList");
		unlockFunctionCall = serializedObject.FindProperty ("unlockFunctionCall");
		useEventOnUnlockWithDelay = serializedObject.FindProperty ("useEventOnUnlockWithDelay");
		delayToActivateEvent = serializedObject.FindProperty ("delayToActivateEvent");
		eventOnUnlockWithDelay = serializedObject.FindProperty ("eventOnUnlockWithDelay");

		useUnlockEventFromLoadingGame = serializedObject.FindProperty ("useUnlockEventFromLoadingGame");
		unlockEventFromLoadingGame = serializedObject.FindProperty ("unlockEventFromLoadingGame");

		canUseAndUseInventoryPickupsOnTriggerEnabled = serializedObject.FindProperty ("canUseAndUseInventoryPickupsOnTriggerEnabled");
		removeInventoryPickupObjectOnTrigger = serializedObject.FindProperty ("removeInventoryPickupObjectOnTrigger");

		deviceStringActionManager = serializedObject.FindProperty ("deviceStringActionManager");
		mainAudioSource = serializedObject.FindProperty ("mainAudioSource");
		mainCollider = serializedObject.FindProperty ("mainCollider");

		manager = (useInventoryObject)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (canBeReUsed);
		EditorGUILayout.PropertyField (useInventoryType);
		if (useInventoryType.enumValueIndex == 1) {
			EditorGUILayout.PropertyField (useObjectsOneByOneUsingButton);
		}
		EditorGUILayout.PropertyField (inventoryObjectAction);
		EditorGUILayout.PropertyField (disableObjectActionAfterUse);
		EditorGUILayout.PropertyField (tagToConfigure);
		EditorGUILayout.PropertyField (playerTag);
		EditorGUILayout.PropertyField (vehicleTag);
		EditorGUILayout.PropertyField (checkPlayerInventoryWhileDriving);
		EditorGUILayout.PropertyField (canUseAndUseInventoryPickupsOnTriggerEnabled);
		EditorGUILayout.PropertyField (removeInventoryPickupObjectOnTrigger);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Message Settings", "window");
		EditorGUILayout.PropertyField (objectUsedMessage);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useCustomObjectNotFoundMessage);
		if (useCustomObjectNotFoundMessage.boolValue) {
			EditorGUILayout.PropertyField (objectNotFoundMessage);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object Activated Settings", "window");

		EditorGUILayout.PropertyField (enableObjectWhenActivate);
		if (enableObjectWhenActivate.boolValue) {
			EditorGUILayout.PropertyField (objectToEnable);
		}
		EditorGUILayout.PropertyField (useAnimation);
		if (useAnimation.boolValue) {
			EditorGUILayout.PropertyField (objectWithAnimation);
			EditorGUILayout.PropertyField (animationName);
		}	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Inventory Menu Settings", "window");
		EditorGUILayout.PropertyField (openPlayerInventoryMenu);
		if (openPlayerInventoryMenu.boolValue) {
			EditorGUILayout.PropertyField (setNewInventoryPanel);
			EditorGUILayout.PropertyField (defaultInventoryPanelName);
			EditorGUILayout.PropertyField (newInventoryPanelName);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug", "window");
		EditorGUILayout.PropertyField (showDebugInfo);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Object State", "window");
		GUILayout.Label ("Object Used\t\t\t " + objectUsed.boolValue);
		GUILayout.Label ("Number Of Objects Used\t\t " + numberOfObjectsUsed.intValue);
		GUILayout.Label ("Number Of Objects Needed\t\t " + numberOfObjectsNeeded.intValue);
		GUILayout.Label ("Current Number Of Objects Needed\t " + currentNumberOfObjectsNeeded.intValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Object To Use List", "window");
		showInventoryObjectNeededList (inventoryObjectNeededList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object Solved Event Settings", "window");
		EditorGUILayout.PropertyField (unlockFunctionCall);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventOnUnlockWithDelay);
		if (useEventOnUnlockWithDelay.boolValue) {
			EditorGUILayout.PropertyField (delayToActivateEvent);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnUnlockWithDelay);
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useUnlockEventFromLoadingGame);

		EditorGUILayout.Space ();
		if (useUnlockEventFromLoadingGame.boolValue) {

			EditorGUILayout.PropertyField (unlockEventFromLoadingGame);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Debug Settings", "window");

		if (GUILayout.Button ("Solve Use Inventory Object (Debug)", buttonStyle)) {
			if (Application.isPlaying) {
				manager.solveThisInventoryObject ();
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Components", "window");
		EditorGUILayout.PropertyField (deviceStringActionManager);
		EditorGUILayout.PropertyField (mainAudioSource);
		EditorGUILayout.PropertyField (mainCollider);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
		 
		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showInventoryObjectNeededList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Inventory Object Needed List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize);

			EditorGUILayout.Space ();	

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				manager.addInventoryObjectNeededInfo ();
			}
			if (GUILayout.Button ("Clear")) {
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

					SerializedProperty currentElement = list.GetArrayElementAtIndex (i);

					string amountValue = " - " + currentElement.FindPropertyRelative ("amountNeeded").intValue;
					EditorGUILayout.PropertyField (currentElement, new GUIContent (currentElement.displayName + amountValue), false);
				
					if (currentElement.isExpanded) {
						expanded = true;
						showInventoryObjectNeededListElementInfo (currentElement, i);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}

				EditorGUILayout.Space ();

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

	void showInventoryObjectNeededListElementInfo (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectNeeded"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useObjectNeededName"));
		if (list.FindPropertyRelative ("useObjectNeededName").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectNeededName"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useObjectAction"));
		if (list.FindPropertyRelative ("useObjectAction").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectAction"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountNeeded"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectUsedMessage"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Sound Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useObjectSound"));
		if (list.FindPropertyRelative ("useObjectSound").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("usedObjectSound"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Objects Placed Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnObjectsPlaced"));
		if (list.FindPropertyRelative ("useEventOnObjectsPlaced").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnObjectsPlaced"));
		}	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object State", "window");
		GUILayout.Label ("Object Used\t\t " + list.FindPropertyRelative ("objectUsed").boolValue);
		GUILayout.Label ("Amount Of Objects Used\t " + list.FindPropertyRelative ("amountOfObjectsUsed").intValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Inventory Objects Info", "window");
		showInventoryObjectList (list.FindPropertyRelative ("inventoryObjectNeededList"), index);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showInventoryObjectList (SerializedProperty list, int index)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Inventory Object List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Objects: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				manager.addSubInventoryObjectNeededList (index);
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
				GUILayout.BeginVertical ("box");
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showInventoryObjectListElement (list.GetArrayElementAtIndex (i));
					}
				}
				GUILayout.EndVertical ();

				GUILayout.BeginHorizontal ();

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

				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showInventoryObjectListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("instantiateObject"));
		if (list.FindPropertyRelative ("instantiateObject").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("placeForObject"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enableObject"));
		if (list.FindPropertyRelative ("enableObject").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToEnable"));
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Animation Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAnimation"));
		if (list.FindPropertyRelative ("useAnimation").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectWithAnimation"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("animationName"));
		}	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object Placed Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventOnObjectPlaced"));
		if (list.FindPropertyRelative ("useEventOnObjectPlaced").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnObjectPlaced"));
		}	
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Object State", "window");
		GUILayout.Label ("Object Activated\t\t " + list.FindPropertyRelative ("objectActivated").boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}
}
#endif