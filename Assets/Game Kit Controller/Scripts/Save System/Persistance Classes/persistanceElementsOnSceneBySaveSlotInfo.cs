using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceElementsOnSceneBySaveSlotInfo
{
	public int saveNumber;
	public List<persistancePlayerElementOnSceneInfo> playerElementsOnSceneList = new List<persistancePlayerElementOnSceneInfo> ();
}

[System.Serializable]
public class persistancePlayerElementOnSceneInfo
{
	public int playerID;
	public List<persistanceElementOnSceneInfo> elementOnSceneList = new List<persistanceElementOnSceneInfo> ();
}

[System.Serializable]
public class persistanceElementOnSceneInfo
{
	public int elementScene;
	public int elementID;

	public bool elementActiveState;

	public bool savePositionValues;

	public float positionX;
	public float positionY;
	public float positionZ;

	public bool saveRotationValues;

	public float rotationX;
	public float rotationY;
	public float rotationZ;

	public int elementPrefabID;

	public bool useElementPrefabID;

	public bool useStats;

	public List<float> floatValueStatList = new List<float> ();
	public List<bool> boolValueStatList = new List<bool> ();

	public persistanceElementOnSceneInfo ()
	{

	}
}