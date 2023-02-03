using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Element On Scene Data", menuName = "GKC/Create Element On Scene Data", order = 51)]
public class elementsOnSceneData : ScriptableObject
{
	public List<elementsOnSceneInfo> elementsOnSceneInfoList = new List<elementsOnSceneInfo> ();


	public GameObject getElementScenePrefabById (int prefabID)
	{
		if (prefabID <= -1) {
			return null;
		}

		int elementsOnSceneInfoListCount = elementsOnSceneInfoList.Count;

		for (int i = 0; i < elementsOnSceneInfoListCount; i++) {

			int elementOnSceneInfoListCount = elementsOnSceneInfoList [i].elementOnSceneInfoList.Count;

			for (int j = 0; j < elementOnSceneInfoListCount; j++) {
				if (elementsOnSceneInfoList [i].elementOnSceneInfoList [j].elementPrefabID == prefabID) {
					return elementsOnSceneInfoList [i].elementOnSceneInfoList [j].elementPrefab;
				}
			}
		}

		return null;
	}

	public int getElementScenePrefabIDByName (string prefabName)
	{
		int elementsOnSceneInfoListCount = elementsOnSceneInfoList.Count;

		for (int i = 0; i < elementsOnSceneInfoListCount; i++) {
			int elementOnSceneInfoListCount = elementsOnSceneInfoList [i].elementOnSceneInfoList.Count;

			for (int j = 0; j < elementOnSceneInfoListCount; j++) {
				if (elementsOnSceneInfoList [i].elementOnSceneInfoList [j].elementPrefab != null) {
					string currentName = elementsOnSceneInfoList [i].elementOnSceneInfoList [j].elementPrefab.name;

					if (currentName.Equals (prefabName) || prefabName.Contains (currentName)) {
						return elementsOnSceneInfoList [i].elementOnSceneInfoList [j].elementPrefabID;
					}
				}
			}
		}

		return -1;
	}
}
