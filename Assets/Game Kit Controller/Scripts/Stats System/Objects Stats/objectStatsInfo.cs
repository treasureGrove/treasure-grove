using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class objectStatsInfo
{
	public string Name;

	[Space]

	public List<objectStatInfo> objectStatInfoList = new List<objectStatInfo> ();

	[Space]

	public bool usedOnObject;

	public List<string> usableObjectList = new List<string> ();
}

[System.Serializable]
public class objectStatInfo
{
	public string Name;

	public bool statIsAmount = true;

	public Texture statIcon;

	[Space]

	public float currentFloatValue;

	public float maxFloatValue;

	[Space]

	public bool currentBoolState;
}