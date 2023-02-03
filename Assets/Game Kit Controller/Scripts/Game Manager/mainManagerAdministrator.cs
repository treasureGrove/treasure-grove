using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class mainManagerAdministrator : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool placeManagersInParent;

	[Space]

	[Space]

	public List<mainManagerInfo> mainManagerInfoList = new List<mainManagerInfo> ();

	[Space]

	[TextArea (3, 15)]public string explanation = 
		"This component stores the a list of prefabs which are the main managers of the game, as these objects are now separated objects" +
		"from the player it self. \n\n" +

		"This includes elements like the main inventory list manager, faction system, mission manager, dialog manager, etc....\n\n" +

		"Just press the button Add Main Managers to Scene and they will be spawned on the scene, " +
		"so you can configure the values on these manager and use the button Update Main Managers Info to Prefabs, to update the new info " +
		"configured. ";


	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[HideInInspector] public Transform managersParent;

	[HideInInspector] public bool managersParentFound;

	void Awake ()
	{
		for (int i = 0; i < mainManagerInfoList.Count; i++) {
			if (mainManagerInfoList [i].spawnManagerOnAwakeEnabled) {
				addMainManagerToScene (mainManagerInfoList [i].Name);
			}
		}
	}

	public void setSpawnManagerOnAwakeEnabledByName (string managerName)
	{
		setSpawnManagerOnAwakeEnabledStateByName (managerName, true);
	}

	public void setSpawnManagerOnAwakeDisableddByName (string managerName)
	{
		setSpawnManagerOnAwakeEnabledStateByName (managerName, false);
	}

	public void setSpawnManagerOnAwakeEnabledStateByName (string managerName, bool state)
	{
		int currentIndex = mainManagerInfoList.FindIndex (s => s.Name == managerName);

		if (currentIndex > -1) {
			mainManagerInfo currentMainManagerInfo = mainManagerInfoList [currentIndex];

			currentMainManagerInfo.spawnManagerOnAwakeEnabled = state;

			updateComponent ();
		}
	}

	public void updateMainManagersInfoToPrefabs ()
	{
		#if UNITY_EDITOR

		for (int i = 0; i < mainManagerInfoList.Count; i++) {
			if (mainManagerInfoList [i].mainManagerOnScene != null && mainManagerInfoList [i].updatePrefabEnabled &&
			    mainManagerInfoList [i].mainManagerPrefab != null) {
				GameObject newPrefab = GameObject.Find (mainManagerInfoList [i].mainManagerOnScene.name);

				PrefabUtility.ReplacePrefab (newPrefab, mainManagerInfoList [i].mainManagerPrefab, ReplacePrefabOptions.ReplaceNameBased);
			}
		}

		updateComponent ();

		#endif
	}

	public void addAllMainManagersToScene ()
	{
		for (int i = 0; i < mainManagerInfoList.Count; i++) {
			addMainManagerToScene (mainManagerInfoList [i].Name);
		}

		if (placeManagersInParent) {
			setMainManagersOnParent ();
		}
	}

	public void setMainManagersOnParent ()
	{
		if (placeManagersInParent) {
			checkParentForManagers ();

			if (managersParentFound) {
				for (int i = 0; i < mainManagerInfoList.Count; i++) {
					if (mainManagerInfoList [i].mainManagerOnScene != null && mainManagerInfoList [i].mainManagerPrefab != null) {
						GameObject newManagerOnScene = GameObject.Find (mainManagerInfoList [i].mainManagerPrefab.name);

						if (newManagerOnScene != null) {

							newManagerOnScene.transform.SetParent (managersParent);
						}
					}
				}
			}
		}
	}

	void checkParentForManagers ()
	{
		managersParentFound = managersParent != null;

		if (managersParentFound) {
			return;
		}

		GameObject newParent = GameObject.Find ("Main Managers Prefabs");

		if (newParent != null) {
			managersParent = newParent.transform;
		} else {
			newParent = new GameObject ();

			newParent.name = "Main Managers Prefabs";
			managersParent = newParent.transform;
		}

		managersParentFound = true;
	}

	public void setMainManagersOnParentFromEditor ()
	{
		placeManagersInParent = true;

		setMainManagersOnParent ();
	}

	public void selectMainManagersParent ()
	{
		if (managersParent == null) {
			setMainManagersOnParentFromEditor ();
		}

		if (managersParent != null) {
			if (managersParent.childCount > 0) {
				GKC_Utils.setActiveGameObjectInEditor (managersParent.GetChild (0).gameObject);
			} else {
				GKC_Utils.setActiveGameObjectInEditor (managersParent.gameObject);
			}
		}
	}

	public void addMainManagerToScene (string managerName)
	{
		int currentIndex = mainManagerInfoList.FindIndex (s => s.Name == managerName);

		if (currentIndex > -1) {
			mainManagerInfo currentMainManagerInfo = mainManagerInfoList [currentIndex];

			if (currentMainManagerInfo.mainManagerOnScene == null) {
				GameObject managerPrefab = currentMainManagerInfo.mainManagerPrefab;

				if (managerPrefab != null) {
					GameObject newManagerOnScene = GameObject.Find (managerPrefab.name);

					if (newManagerOnScene == null) {
						bool instantiateManagerLinkedToPrefabEnabled = currentMainManagerInfo.instantiateManagerLinkedToPrefabEnabled;
							
						if (Application.isPlaying || !instantiateManagerLinkedToPrefabEnabled) {
							newManagerOnScene = (GameObject)Instantiate (managerPrefab, Vector3.zero, Quaternion.identity);

							newManagerOnScene.name = managerPrefab.name;

							currentMainManagerInfo.mainManagerOnScene = newManagerOnScene as UnityEngine.Object;

							instantiateManagerLinkedToPrefabEnabled = false;
						} 

						if (instantiateManagerLinkedToPrefabEnabled) {
							#if UNITY_EDITOR
							UnityEngine.Object newManagerOnSceneObject = PrefabUtility.InstantiatePrefab (managerPrefab);

							newManagerOnSceneObject.name = managerPrefab.name;

							currentMainManagerInfo.mainManagerOnScene = newManagerOnSceneObject;
							#endif
						}
					} else {
						currentMainManagerInfo.mainManagerOnScene = newManagerOnScene as UnityEngine.Object;
					}

					updateComponent ();

					if (showDebugPrint) {
						print ("Main Manager " + managerName + " added on scene");
					}
				}
			}
		}
	}

	public void addMainManagerToSceneWithType (string managerName, Type typeToSearch)
	{
		int currentIndex = mainManagerInfoList.FindIndex (s => s.Name == managerName);

		if (currentIndex > -1) {
			mainManagerInfo currentMainManagerInfo = mainManagerInfoList [currentIndex];

			if (currentMainManagerInfo.mainManagerOnScene == null) {

				UnityEngine.Object typeObject = UnityEngine.Object.FindObjectOfType (typeToSearch);

				if (typeObject != null) {
					if (showDebugPrint) {
						print (typeObject.name);
					}

					currentMainManagerInfo.mainManagerOnScene = typeObject;

					updateComponent ();

					if (showDebugPrint) {
						print ("Main Manager " + managerName + " located on scene");
					}
				} else {

					GameObject managerPrefab = currentMainManagerInfo.mainManagerPrefab;

					if (managerPrefab != null) {
						bool instantiateManagerLinkedToPrefabEnabled = currentMainManagerInfo.instantiateManagerLinkedToPrefabEnabled;

						if (Application.isPlaying || !instantiateManagerLinkedToPrefabEnabled) {
							GameObject newManagerOnScene = (GameObject)Instantiate (managerPrefab, Vector3.zero, Quaternion.identity);

							newManagerOnScene.name = managerPrefab.name;

							currentMainManagerInfo.mainManagerOnScene = newManagerOnScene as UnityEngine.Object;

							instantiateManagerLinkedToPrefabEnabled = false;
						} 

						if (instantiateManagerLinkedToPrefabEnabled) {
							#if UNITY_EDITOR
							UnityEngine.Object newManagerOnSceneObject = PrefabUtility.InstantiatePrefab (managerPrefab);

							newManagerOnSceneObject.name = managerPrefab.name;

							currentMainManagerInfo.mainManagerOnScene = newManagerOnSceneObject;
							#endif
						}

						updateComponent ();

						if (showDebugPrint) {
							print ("Main Manager " + managerName + " added on scene");
						}
					}
				}
			}
		}
	}

	void updateComponent ()
	{
		if (!Application.isPlaying) {
			GKC_Utils.updateComponent (this);

			GKC_Utils.updateDirtyScene ("Update Managers Administrator elements", gameObject);
		}
	}

	[System.Serializable]
	public class mainManagerInfo
	{
		public string Name;
		public GameObject mainManagerPrefab;

		public UnityEngine.Object mainManagerOnScene;

		public bool spawnManagerOnAwakeEnabled;

		public bool updatePrefabEnabled = true;

		public bool instantiateManagerLinkedToPrefabEnabled = true;
	}
}
