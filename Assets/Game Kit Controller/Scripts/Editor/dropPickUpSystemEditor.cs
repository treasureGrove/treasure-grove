using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(dropPickUpSystem))]
[CanEditMultipleObjects]
public class dropPickUpSystemEditor : Editor
{
	SerializedProperty dropPickupsEnabled;
	SerializedProperty dropDelay;
	SerializedProperty destroyAfterDropping;
	SerializedProperty setPickupScale;
	SerializedProperty pickUpScale;
	SerializedProperty randomContent;
	SerializedProperty showGizmo;
	SerializedProperty maxRadiusToInstantiate;
	SerializedProperty pickUpOffset;
	SerializedProperty dropPickUpList;
	SerializedProperty extraForceToPickup;
	SerializedProperty extraForceToPickupRadius;
	SerializedProperty forceMode;

	SerializedProperty mainPickupManagerName;

	SerializedProperty useGeneralProbabilityDropObjects;
	SerializedProperty generalProbabilityDropObjects;

	SerializedProperty useEventToSendSpawnedObjects;
	SerializedProperty eventToSendSpawnedObjects;

	dropPickUpSystem manager;

	int typeIndex;
	int nameIndex;

	List<pickUpElementInfo> managerPickUpList;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		dropPickupsEnabled = serializedObject.FindProperty ("dropPickupsEnabled");
		dropDelay = serializedObject.FindProperty ("dropDelay");
		destroyAfterDropping = serializedObject.FindProperty ("destroyAfterDropping");
		setPickupScale = serializedObject.FindProperty ("setPickupScale");
		pickUpScale = serializedObject.FindProperty ("pickUpScale");
		randomContent = serializedObject.FindProperty ("randomContent");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		maxRadiusToInstantiate = serializedObject.FindProperty ("maxRadiusToInstantiate");
		pickUpOffset = serializedObject.FindProperty ("pickUpOffset");
		dropPickUpList = serializedObject.FindProperty ("dropPickUpList");
		extraForceToPickup = serializedObject.FindProperty ("extraForceToPickup");
		extraForceToPickupRadius = serializedObject.FindProperty ("extraForceToPickupRadius");
		forceMode = serializedObject.FindProperty ("forceMode");

		mainPickupManagerName = serializedObject.FindProperty ("mainPickupManagerName");

		useGeneralProbabilityDropObjects = serializedObject.FindProperty ("useGeneralProbabilityDropObjects");
		generalProbabilityDropObjects = serializedObject.FindProperty ("generalProbabilityDropObjects");

		useEventToSendSpawnedObjects = serializedObject.FindProperty ("useEventToSendSpawnedObjects");
		eventToSendSpawnedObjects = serializedObject.FindProperty ("eventToSendSpawnedObjects");

		manager = (dropPickUpSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (dropPickupsEnabled);
		EditorGUILayout.PropertyField (dropDelay);
		EditorGUILayout.PropertyField (destroyAfterDropping);
		EditorGUILayout.PropertyField (setPickupScale);
		EditorGUILayout.PropertyField (pickUpScale);
		EditorGUILayout.PropertyField (randomContent);
		EditorGUILayout.PropertyField (maxRadiusToInstantiate);
		EditorGUILayout.PropertyField (pickUpOffset);
		EditorGUILayout.PropertyField (mainPickupManagerName);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Drop Object Probability Settings", "window");
		EditorGUILayout.PropertyField (useGeneralProbabilityDropObjects);
		EditorGUILayout.PropertyField (generalProbabilityDropObjects);
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Drop PickUps List", "window");
		showDropPickUpList (dropPickUpList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Force To Pickups Settings", "window");
		EditorGUILayout.PropertyField (extraForceToPickup);
		EditorGUILayout.PropertyField (extraForceToPickupRadius);
		EditorGUILayout.PropertyField (forceMode);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (useEventToSendSpawnedObjects);
		if (useEventToSendSpawnedObjects.boolValue) {
			EditorGUILayout.PropertyField (eventToSendSpawnedObjects);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get PickUp Manager List")) {
			manager.getManagerPickUpList ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showDropPickUpTypeInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");

		managerPickUpList = manager.managerPickUpList;

		if (managerPickUpList.Count > 0) {
			typeIndex = list.FindPropertyRelative ("typeIndex").intValue;

			typeIndex = EditorGUILayout.Popup ("PickUp Type", typeIndex, getTypeList ());

			if (typeIndex < managerPickUpList.Count) {
				list.FindPropertyRelative ("pickUpType").stringValue = managerPickUpList [typeIndex].pickUpType;

				EditorGUILayout.Space ();

				GUILayout.BeginVertical ("box");

				EditorGUILayout.Space ();

				showDropPickUpList (list.FindPropertyRelative ("dropPickUpTypeList"), list.FindPropertyRelative ("pickUpType").stringValue, typeIndex);

				EditorGUILayout.Space ();
			}

			list.FindPropertyRelative ("typeIndex").intValue = typeIndex;

			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();
	}

	void showDropPickUpList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Drop PickUp List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Add the pickups to drop here", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of PickUps: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add PickUp")) {
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

					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);

					if (currentArrayElement.isExpanded) {
						expanded = true;
						showDropPickUpTypeInfo (currentArrayElement);
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

	void showDropPickUpElementInfo (SerializedProperty list, int typeIndex)
	{
		managerPickUpList = manager.managerPickUpList;



		GUILayout.BeginVertical ("Main Settings", "window");

		if (managerPickUpList.Count > typeIndex && managerPickUpList [typeIndex].pickUpTypeList.Count > 0) {
			nameIndex = list.FindPropertyRelative ("nameIndex").intValue;

			nameIndex = EditorGUILayout.Popup ("Name", nameIndex, getNameList (typeIndex));

			if (managerPickUpList [typeIndex].pickUpTypeList.Count > nameIndex) {
				list.FindPropertyRelative ("name").stringValue = managerPickUpList [typeIndex].pickUpTypeList [nameIndex].Name;
			}

			list.FindPropertyRelative ("nameIndex").intValue = nameIndex;
		}

		if (randomContent.boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountLimits"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("quantityLimits"));
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("amount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("quantity"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Crafting Blueprint Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dropCraftingBlueprintIngredients"));
		if (list.FindPropertyRelative ("dropCraftingBlueprintIngredients").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("blueprintName"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("amountMultiplier"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Drop Object Probability Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomProbabilityDropObjects"));
		if (list.FindPropertyRelative ("useCustomProbabilityDropObjects").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("customProbabilityDropObjects"));

		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreProbabilityDropObjects"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();
	}

	void showDropPickUpList (SerializedProperty list, string pickUpType, int typeIndex)
	{
		GUILayout.BeginVertical ();
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide" + pickUpType + " Type List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of PickUps: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add PickUp")) {
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
					SerializedProperty currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);

					if (currentArrayElement.isExpanded) {
						expanded = true;

						showDropPickUpElementInfo (currentArrayElement, typeIndex);
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