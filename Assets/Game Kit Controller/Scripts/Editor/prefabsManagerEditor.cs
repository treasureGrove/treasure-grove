using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(prefabsManager))]
public class prefabsManagerEditor : Editor
{
	SerializedProperty prefabsUrl;
	SerializedProperty layerToPlaceObjects;
	SerializedProperty placeObjectsOffset;
	SerializedProperty seePrefabTypesExplanation;
	SerializedProperty prefabTypeInfoList;
	SerializedProperty prefabTypeInfoListToPlaceOnScene;
	SerializedProperty searchComplete;
	SerializedProperty prefabNameToSearch;
	SerializedProperty prefabCategoryNameToSearch;
	SerializedProperty prefabSearchResultList;

	SerializedProperty currentArrayElement;

	prefabsManager manager;

	Color defBackgroundColor;

	bool showPrefabsSearcher;

	string buttonMessage;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		prefabsUrl = serializedObject.FindProperty ("prefabsUrl");
		layerToPlaceObjects = serializedObject.FindProperty ("layerToPlaceObjects");
		placeObjectsOffset = serializedObject.FindProperty ("placeObjectsOffset");
		seePrefabTypesExplanation = serializedObject.FindProperty ("seePrefabTypesExplanation");
		prefabTypeInfoList = serializedObject.FindProperty ("prefabTypeInfoList");
		prefabTypeInfoListToPlaceOnScene = serializedObject.FindProperty ("prefabTypeInfoListToPlaceOnScene");
		searchComplete = serializedObject.FindProperty ("searchComplete");
		prefabNameToSearch = serializedObject.FindProperty ("prefabNameToSearch");
		prefabCategoryNameToSearch = serializedObject.FindProperty ("prefabCategoryNameToSearch");
		prefabSearchResultList = serializedObject.FindProperty ("prefabSearchResultList");

		manager = (prefabsManager)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (prefabsUrl);
		EditorGUILayout.PropertyField (layerToPlaceObjects);
		EditorGUILayout.PropertyField (placeObjectsOffset);
		EditorGUILayout.PropertyField (seePrefabTypesExplanation);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Prefab Type List", "window");
		showPrefabTypeList (prefabTypeInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Prefab List To Place On Scene")) {
			manager.updatePrefabsListToPlaceOnScene ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Prefabs To Place On Scene List", "window");
		showPrefabTypeListToPlaceOnScene (prefabTypeInfoListToPlaceOnScene);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Prefabs Searcher Options", "window");

		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (showPrefabsSearcher) {
			GUI.backgroundColor = Color.gray;
			buttonMessage = "Hide Prefabs Searcher";
		} else {
			GUI.backgroundColor = defBackgroundColor;
			buttonMessage = "Show Prefabs Searcher";
		}
		if (GUILayout.Button (buttonMessage)) {
			showPrefabsSearcher = !showPrefabsSearcher;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();

		if (showPrefabsSearcher) {
			
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (prefabNameToSearch, new GUIContent ("Prefab Name To Search"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (prefabCategoryNameToSearch, new GUIContent ("Prefab Category To Search"));

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Search Prefab By Name")) {
				manager.makePrefabsSearchByName ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Search Prefab By Category")) {
				manager.makePrefabsSearchByCategory ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Clean Search Results")) {
				manager.cleanSearchResults ();
			}

			EditorGUILayout.Space ();

			if (searchComplete.boolValue) {
				
				EditorGUILayout.Space ();

				if (prefabSearchResultList.arraySize > 0) {
					GUILayout.BeginVertical ("Prefab List", "window");
					showPrefabListSearchResultToPlaceOnScene (prefabSearchResultList);
					GUILayout.EndVertical ();
				} else {

					EditorGUILayout.Space ();

					GUILayout.Label ("No Results Found");
				}
			}
		}
			
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (EditorGUI.EndChangeCheck ()) {
			serializedObject.ApplyModifiedProperties ();

			Repaint ();
		}
	}

	void showPrefabTypeListElement (SerializedProperty list, int prefabTypeIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("url"));

		if (seePrefabTypesExplanation.boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("prefabExplanation"));
		}

		EditorGUILayout.Space ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Prefab List", "window");
		showPrefabList (list.FindPropertyRelative ("prefabInfoList"), prefabTypeIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showPrefabTypeList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Prefab Type List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			int listArraySize = list.arraySize;

			GUILayout.Label ("Number of Types: " + listArraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Type")) {
				manager.addPrefabType ();

				listArraySize = list.arraySize;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();

				listArraySize = list.arraySize;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < listArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < listArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < listArraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < listArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, new GUIContent ((i + 1) + "-" + currentArrayElement.displayName), false);

					if (currentArrayElement.isExpanded) {
						showPrefabTypeListElement (currentArrayElement, i);
						expanded = true;
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

					listArraySize = list.arraySize;
				}
				if (GUILayout.Button ("+")) {
					manager.addPrefabType (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < listArraySize) {
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

	void showPrefabList (SerializedProperty list, int prefabTypeIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Prefab Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			int listArraySize = list.arraySize;

			GUILayout.Label ("Number of Prefabs: " + listArraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Prefab")) {
				manager.addPrefab (prefabTypeIndex);

				listArraySize = list.arraySize;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();

				listArraySize = list.arraySize;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < listArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < listArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Set URL With Names")) {
				manager.setURLWithNames (prefabTypeIndex);
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Get And Set All Prefabs In Folder (Check Subfolders)")) {
				manager.getAndSetAllPrefabsInFolder (prefabTypeIndex, true);
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Get And Set All Prefabs In Folder")) {
				manager.getAndSetAllPrefabsInFolder (prefabTypeIndex, false);
			}

			EditorGUILayout.Space ();
		
			for (int i = 0; i < listArraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < listArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						showPrefabListElement (currentArrayElement, prefabTypeIndex, i);
						expanded = true;
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

					listArraySize = list.arraySize;
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < listArraySize) {
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

	void showPrefabListElement (SerializedProperty list, int typeIndex, int prefabIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("urlName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("prefabGameObject"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionToSpawnOffset"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Place Prefab In Scene")) {
			manager.placePrefabInScene (typeIndex, prefabIndex);
		}

		GUILayout.EndVertical ();
	}

	void showPrefabTypeListToPlaceOnScene (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Prefab Type Info List To Place On Scene", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			int listArraySize = list.arraySize;

			GUILayout.Label ("Number of Prefabs: " + listArraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < listArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < listArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < listArraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < listArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						showPrefabTypeListToPlaceOnSceneElement (currentArrayElement, i);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}

				EditorGUILayout.Space ();
				GUILayout.EndHorizontal ();
			
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showPrefabTypeListToPlaceOnSceneElement (SerializedProperty list, int prefabTypeIndex)
	{
		GUILayout.BeginVertical ("Prefab List", "window");
		if (seePrefabTypesExplanation.boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("prefabExplanation"));
		}

		EditorGUILayout.Space ();

		showPrefabListToPlaceOnScene (list.FindPropertyRelative ("prefabInfoList"), prefabTypeIndex);
		GUILayout.EndVertical ();
	}

	void showPrefabListToPlaceOnScene (SerializedProperty list, int prefabTypeIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		int listArraySize = list.arraySize;

		GUILayout.Label ("Number of Prefabs: " + listArraySize);

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		for (int i = 0; i < listArraySize; i++) {

			if (GUILayout.Button ("Add: - " + list.GetArrayElementAtIndex (i).displayName)) {
				manager.placePrefabInScene (prefabTypeIndex, i);
			}
		}
		GUILayout.EndVertical ();
	}

	void showPrefabListSearchResultToPlaceOnScene (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		int listArraySize = list.arraySize;

		GUILayout.Label ("Number of Categories: " + listArraySize);

		EditorGUILayout.Space ();
	
		for (int i = 0; i < listArraySize; i++) {
			GUILayout.BeginHorizontal ();
			GUILayout.BeginHorizontal ("box");

			EditorGUILayout.BeginVertical ();

			EditorGUILayout.Space ();

			currentArrayElement = list.GetArrayElementAtIndex (i);

			GUILayout.BeginHorizontal ("box");
			GUILayout.Label (currentArrayElement.displayName, EditorStyles.boldLabel);
			GUILayout.EndHorizontal ();

			showPrefabListSearchResultToPlaceOnSceneElement (currentArrayElement.FindPropertyRelative ("prefabInfoList"));

			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();

			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
		}
		GUILayout.EndVertical ();
	}


	void showPrefabListSearchResultToPlaceOnSceneElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		int listArraySize = list.arraySize;

		GUILayout.Label ("Number of Prefabs: " + listArraySize);

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		for (int i = 0; i < listArraySize; i++) {

			currentArrayElement = list.GetArrayElementAtIndex (i);

			if (GUILayout.Button ("Add: - " + currentArrayElement.displayName)) {
				manager.placePrefabInSceneByGameObject (currentArrayElement.FindPropertyRelative ("prefabGameObject").objectReferenceValue as GameObject);
			
				listArraySize = list.arraySize;
			}
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

}
#endif