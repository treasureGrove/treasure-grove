using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class characterCustomizationInfo
{
	public string Name;
	public string typeName;

	public string categoryName;

	public int ID;

	[Space]

	public float floatValue;
	public bool useRandomFloatValue;

	[Space]

	public bool boolValue;
	public bool useRandomBoolValue;
	public bool useRandomObjectFromCategoryName;

	[Space]

	public bool multipleElements;
}

[System.Serializable]
public class characterCustomizationTypeInfo
{
	public string Name;
	public List<characterCustomizationInfo> characterCustomizationInfoList = new List<characterCustomizationInfo> ();
}