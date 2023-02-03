using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class elementsOnSceneInfo
{
	public string elementOnSceneCategoryName;
	public List<elementOnSceneInfo> elementOnSceneInfoList = new List<elementOnSceneInfo> ();
}

[System.Serializable]
public class elementOnSceneInfo
{
	public int elementPrefabID;

	public GameObject elementPrefab;
}
