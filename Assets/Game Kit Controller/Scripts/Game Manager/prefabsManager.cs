using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class prefabsManager : MonoBehaviour
{
	public string prefabsUrl = "Assets/Game Kit Controller/Prefabs/";
	public List<prefabTypeInfo> prefabTypeInfoList = new List<prefabTypeInfo> ();
	public LayerMask layerToPlaceObjects;

	public float placeObjectsOffset = 0.5f;

	public List<prefabTypeInfo> prefabTypeInfoListToPlaceOnScene = new List<prefabTypeInfo> ();

	public List<prefabTypeInfo> prefabSearchResultList = new List<prefabTypeInfo> ();

	public string prefabNameToSearch;
	public string prefabCategoryNameToSearch;

	public bool searchComplete;

	public bool seePrefabTypesExplanation;

	//add function to get automatically the prefabs in an specific folder

	GameObject lastPrefabCreated;


	public void placePrefabInScene (int typeIndex, int prefabIndex)
	{
		Vector3 positionToInstantiate = Vector3.zero;

		Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

		if (currentCameraEditor != null) {
			Vector3 editorCameraPosition = currentCameraEditor.transform.position;
			Vector3 editorCameraForward = currentCameraEditor.transform.forward;

			RaycastHit hit;

			if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceObjects)) {
				positionToInstantiate = hit.point + Vector3.up * placeObjectsOffset;
			}
		}

		if (typeIndex < prefabTypeInfoList.Count && prefabIndex < prefabTypeInfoList [typeIndex].prefabInfoList.Count) {
			prefabInfo newPrefabInfo = prefabTypeInfoList [typeIndex].prefabInfoList [prefabIndex];

			GameObject prefabToInstantiate = newPrefabInfo.prefabGameObject;

			string prefabName = newPrefabInfo.Name;

			if (prefabToInstantiate != null) {
				positionToInstantiate += Vector3.up * newPrefabInfo.positionToSpawnOffset;

				GameObject newObjectCreated = (GameObject)Instantiate (prefabToInstantiate, positionToInstantiate, Quaternion.identity);
				newObjectCreated.name = prefabToInstantiate.name;

				print (prefabName + " prefab added to the scene");

				lastPrefabCreated = newObjectCreated;
			} else {
				print ("WARNING: prefab gameObject is empty, make sure it is assigned correctly");
			}
		} else {
			print ("WARNING: prefab gameObject is empty or not assigned properly, make sure it is assigned correctly");
		}
	}

	public void addPrefab (int prefabTypeIndex)
	{
		prefabInfo newPrefabInfo = new prefabInfo ();

		newPrefabInfo.Name = "New Prefab";

		prefabTypeInfoList [prefabTypeIndex].prefabInfoList.Add (newPrefabInfo);

		updateComponent ();
	}

	public void addPrefabType ()
	{
		prefabTypeInfo newPrefabTypeInfo = new prefabTypeInfo ();

		newPrefabTypeInfo.Name = "New Type";

		prefabTypeInfoList.Add (newPrefabTypeInfo);

		updateComponent ();
	}

	public void addPrefabType (int prefabTypeIndex)
	{
		prefabTypeInfo newPrefabTypeInfo = new prefabTypeInfo ();

		newPrefabTypeInfo.Name = "New Type";

		prefabTypeInfoList.Insert (prefabTypeIndex + 1, newPrefabTypeInfo);

		updateComponent ();
	}

	public string getPrefabPath (string prefabType, string prefabName)
	{
		string path = "";

		int prefabTypeInfoListCount = prefabTypeInfoList.Count;

		for (int i = 0; i < prefabTypeInfoListCount; i++) {
			if (prefabTypeInfoList [i].Name.Equals (prefabType)) {

				int prefabInfoListCount = prefabTypeInfoList [i].prefabInfoList.Count;

				for (int j = 0; j < prefabInfoListCount; j++) {
					if (prefabTypeInfoList [i].prefabInfoList [j].Name.Equals (prefabName)) {
						path = prefabsUrl + prefabTypeInfoList [i].url + "/" + prefabTypeInfoList [i].prefabInfoList [j].urlName;
					}
				}
			}
		}

		return path;
	}

	public void updatePrefabsListToPlaceOnScene ()
	{
		prefabTypeInfoListToPlaceOnScene.Clear ();

		int prefabTypeInfoListCount = prefabTypeInfoList.Count;

		for (int i = 0; i < prefabTypeInfoListCount; i++) {

			prefabTypeInfo newPrefabTypeInfo = new prefabTypeInfo ();

			newPrefabTypeInfo.Name = prefabTypeInfoList [i].Name;

			newPrefabTypeInfo.prefabExplanation = prefabTypeInfoList [i].prefabExplanation;

			int prefabInfoListCount = prefabTypeInfoList [i].prefabInfoList.Count;

			for (int j = 0; j < prefabInfoListCount; j++) {

				prefabInfo newPrefabInfo = new prefabInfo ();

				newPrefabInfo.Name = prefabTypeInfoList [i].prefabInfoList [j].Name;

				newPrefabInfo.prefabGameObject = prefabTypeInfoList [i].prefabInfoList [j].prefabGameObject;

				newPrefabTypeInfo.prefabInfoList.Add (newPrefabInfo);
			}

			prefabTypeInfoListToPlaceOnScene.Add (newPrefabTypeInfo);
		}


		updateComponent ();
	}

	public void makePrefabsSearchByName ()
	{
		prefabSearchResultList.Clear ();

		if (prefabNameToSearch.Equals ("") || prefabNameToSearch.Equals (" ")) {

			searchComplete = true;

			return;
		}

		int prefabTypeInfoListCount = prefabTypeInfoList.Count;

		for (int i = 0; i < prefabTypeInfoListCount; i++) {

			prefabTypeInfo newPrefabTypeInfo = new prefabTypeInfo ();

			newPrefabTypeInfo.Name = prefabTypeInfoList [i].Name;

			newPrefabTypeInfo.prefabExplanation = prefabTypeInfoList [i].prefabExplanation;

			int prefabInfoListCount = prefabTypeInfoList [i].prefabInfoList.Count;

			for (int j = 0; j < prefabInfoListCount; j++) {

				bool prefabNameFound = false;

				prefabNameToSearch = prefabNameToSearch.ToLower ();

				string currentPrefabName = prefabTypeInfoList [i].prefabInfoList [j].Name.ToLower ();

				if (prefabNameToSearch.Equals (currentPrefabName)) {
					prefabNameFound = true;
				}

				if (!prefabNameFound) {
					if (currentPrefabName.Contains (prefabNameToSearch)) {
						prefabNameFound = true;
					}
				}

				if (prefabNameFound) {
					prefabInfo newPrefabInfo = new prefabInfo ();

					newPrefabInfo.Name = prefabTypeInfoList [i].prefabInfoList [j].Name;

					newPrefabInfo.prefabGameObject = prefabTypeInfoList [i].prefabInfoList [j].prefabGameObject;

					newPrefabTypeInfo.prefabInfoList.Add (newPrefabInfo);
				}
			}

			prefabSearchResultList.Add (newPrefabTypeInfo);
		}

		for (int i = prefabSearchResultList.Count - 1; i >= 0; i--) {
			int prefabInfoListCount = prefabTypeInfoList [i].prefabInfoList.Count;

			if (prefabInfoListCount == 0) {

				prefabSearchResultList.RemoveAt (i);
			}
		}

		searchComplete = true;
	}

	public void makePrefabsSearchByCategory ()
	{
		prefabSearchResultList.Clear ();

		if (prefabCategoryNameToSearch.Equals ("") || prefabCategoryNameToSearch.Equals (" ")) {

			searchComplete = true;

			return;
		}

		int prefabTypeInfoListCount = prefabTypeInfoList.Count;

		for (int i = 0; i < prefabTypeInfoListCount; i++) {

			prefabTypeInfo newPrefabTypeInfo = new prefabTypeInfo ();

			newPrefabTypeInfo.Name = prefabTypeInfoList [i].Name;

			newPrefabTypeInfo.prefabExplanation = prefabTypeInfoList [i].prefabExplanation;

			bool prefabNameFound = false;

			prefabCategoryNameToSearch = prefabCategoryNameToSearch.ToLower ();

			string currentPrefabName = prefabTypeInfoList [i].Name.ToLower ();

			if (prefabCategoryNameToSearch.Equals (currentPrefabName)) {
				prefabNameFound = true;
			}

			if (!prefabNameFound) {
				if (currentPrefabName.Contains (prefabCategoryNameToSearch)) {
					prefabNameFound = true;
				}
			}

			if (prefabNameFound) {
				int prefabInfoListCount = prefabTypeInfoList [i].prefabInfoList.Count;

				for (int j = 0; j < prefabInfoListCount; j++) {

					prefabInfo newPrefabInfo = new prefabInfo ();

					newPrefabInfo.Name = prefabTypeInfoList [i].prefabInfoList [j].Name;

					newPrefabInfo.prefabGameObject = prefabTypeInfoList [i].prefabInfoList [j].prefabGameObject;

					newPrefabTypeInfo.prefabInfoList.Add (newPrefabInfo);
				}
			}

			prefabSearchResultList.Add (newPrefabTypeInfo);
		}

		for (int i = prefabSearchResultList.Count - 1; i >= 0; i--) {
			if (prefabSearchResultList [i].prefabInfoList.Count == 0) {

				prefabSearchResultList.RemoveAt (i);
			}
		}

		searchComplete = true;
	}

	public void placePrefabInSceneByGameObject (GameObject objectToSearch)
	{
		int prefabTypeInfoListCount = prefabTypeInfoList.Count;

		for (int i = 0; i < prefabTypeInfoListCount; i++) {

			int prefabInfoListCount = prefabTypeInfoList [i].prefabInfoList.Count;

			for (int j = 0; j < prefabInfoListCount; j++) {
				if (prefabTypeInfoList [i].prefabInfoList [j].prefabGameObject == objectToSearch) {
					placePrefabInScene (i, j);

					return;
				}
			}
		}
	}

	public void cleanSearchResults ()
	{
		prefabSearchResultList.Clear ();

		prefabCategoryNameToSearch = "";

		prefabNameToSearch = "";

		searchComplete = false;
	}

	public GameObject getPrefabGameObject (string prefabName)
	{
		prefabName = prefabName.ToLower ();

		int prefabTypeInfoListCount = prefabTypeInfoList.Count;

		for (int i = 0; i < prefabTypeInfoListCount; i++) {
			int prefabInfoListCount = prefabTypeInfoList [i].prefabInfoList.Count;

			for (int j = 0; j < prefabInfoListCount; j++) {
				if (prefabTypeInfoList [i].prefabInfoList [j].Name.ToLower ().Equals (prefabName)) {
					return prefabTypeInfoList [i].prefabInfoList [j].prefabGameObject;
				
				}
			}
		}

		return null;
	}

	public void spawnPrefabByName (string prefabName)
	{
		prefabName = prefabName.ToLower ();

		int prefabTypeInfoListCount = prefabTypeInfoList.Count;

		for (int i = 0; i < prefabTypeInfoListCount; i++) {
			int prefabInfoListCount = prefabTypeInfoList [i].prefabInfoList.Count;

			for (int j = 0; j < prefabInfoListCount; j++) {
				if (prefabTypeInfoList [i].prefabInfoList [j].Name.ToLower ().Equals (prefabName)) {
					placePrefabInScene (i, j);

					return;
				}
			}
		}

		print ("WARNING: No prefab found with the name " + prefabName + ". Make sure to configure one with that name to use it");
	}


	public float getPrefabSpawnOffset (string prefabName)
	{
		prefabName = prefabName.ToLower ();

		int prefabTypeInfoListCount = prefabTypeInfoList.Count;

		for (int i = 0; i < prefabTypeInfoListCount; i++) {
			int prefabInfoListCount = prefabTypeInfoList [i].prefabInfoList.Count;

			for (int j = 0; j < prefabInfoListCount; j++) {
				if (prefabTypeInfoList [i].prefabInfoList [j].Name.ToLower ().Equals (prefabName)) {
					return prefabTypeInfoList [i].prefabInfoList [j].positionToSpawnOffset;

				}
			}
		}

		return 0;
	}

	public void setURLWithNames (int prefabTypeIndex)
	{
		int prefabInfoListCount = prefabTypeInfoList [prefabTypeIndex].prefabInfoList.Count;

		for (int j = 0; j < prefabInfoListCount; j++) {
			if (prefabTypeInfoList [prefabTypeIndex].prefabInfoList [j].prefabGameObject != null) {
				prefabTypeInfoList [prefabTypeIndex].prefabInfoList [j].Name = prefabTypeInfoList [prefabTypeIndex].prefabInfoList [j].prefabGameObject.name;
				prefabTypeInfoList [prefabTypeIndex].prefabInfoList [j].urlName = prefabTypeInfoList [prefabTypeIndex].prefabInfoList [j].Name;
			} else {
				print ("WARNING: there is a prefab which hasn't a gameObject assigned, make sure to assign it on the category " + prefabTypeInfoList [prefabTypeIndex].Name);
			}
		}

		updateComponent ();
	}

	public void getAndSetAllPrefabsInFolder (int prefabTypeIndex, bool searchOnSubFolders)
	{
		#if UNITY_EDITOR

		prefabTypeInfoList [prefabTypeIndex].prefabInfoList.Clear ();

		string path = prefabsUrl + prefabTypeInfoList [prefabTypeIndex].url;

		if (!Directory.Exists (path)) {
			print ("WARNING: " + path + " path doesn't exist, make sure the path is from an existing folder in the project");

			return;
		}

		string[] search_results = null;

		if (searchOnSubFolders) {
			search_results = System.IO.Directory.GetFiles (path, "*.prefab", System.IO.SearchOption.AllDirectories);
		} else {
			search_results = System.IO.Directory.GetFiles (path, "*.prefab");
		}

		if (search_results.Length > 0) {

			foreach (string file in search_results) {
				//must convert file path to relative-to-unity path (and watch for '\' character between Win/Mac)
				GameObject currentPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath (file, typeof(GameObject)) as GameObject;

				if (currentPrefab != null) {
					prefabInfo newPrefabInfo = new prefabInfo ();
					newPrefabInfo.prefabGameObject = currentPrefab;

					prefabTypeInfoList [prefabTypeIndex].prefabInfoList.Add (newPrefabInfo);
				} else {
					print ("WARNING: something went wrong when trying to get the prefab in the path " + file);
				}
			}

			print (prefabTypeInfoList [prefabTypeIndex].prefabInfoList.Count + " prefabs have been found and configured in the path " + path);

			setURLWithNames (prefabTypeIndex);

			updateComponent ();
		} else {
			print ("WARNING: no prefabs were found in the path " + path + "\n. Make sure the URL exist in the project folder");
		}

		#endif
	}

	public void selectLastPrefabSpawned ()
	{
		if (lastPrefabCreated != null) {
			GKC_Utils.setActiveGameObjectInEditor (lastPrefabCreated);

			lastPrefabCreated = null;
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Player Prefabs Manager", gameObject);
	}

	[System.Serializable]
	public class prefabTypeInfo
	{
		public string Name;
		public string url;
		public List<prefabInfo> prefabInfoList = new List<prefabInfo> ();

		[TextArea (3, 10)] public string prefabExplanation;
	}

	[System.Serializable]
	public class prefabInfo
	{
		public string Name;
		public string urlName;
		public GameObject prefabGameObject;

		public float positionToSpawnOffset;
	}
}
