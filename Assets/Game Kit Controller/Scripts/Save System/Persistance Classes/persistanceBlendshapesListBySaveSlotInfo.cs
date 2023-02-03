using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceBlendshapesListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistancePlayerBlendshapesInfo> playerBlendshapesList = new List<persistancePlayerBlendshapesInfo> ();
}

[System.Serializable]
public class persistancePlayerBlendshapesInfo
{
	public int playerID;

	public List<string> accessoriesList = new List<string> ();

	public List<persistanceBlendshapesInfo> blendshapesList = new List<persistanceBlendshapesInfo> ();
}

[System.Serializable]
public class persistanceBlendshapesInfo
{
	public string Name;

	public float blendShapeValue;

	public persistanceBlendshapesInfo ()
	{

	}
}