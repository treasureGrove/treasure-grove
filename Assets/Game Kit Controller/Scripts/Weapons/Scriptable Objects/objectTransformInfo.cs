using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class objectTransformInfo
{
	public Vector3 objectPosition;
	public Quaternion objectRotation;
}

[System.Serializable]
public class objectsTransformInfo
{
	public string Name;
	public List<objectTransformInfo> objectTransformInfoList = new List<objectTransformInfo> ();
}
