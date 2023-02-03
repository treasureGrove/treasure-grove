using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class craftingBlueprintInfo
{
	public string Name;

	public int amountObtained = 1;

	[Space]

	public List<craftingIngredientObjectInfo> craftingIngredientObjectInfoList = new List<craftingIngredientObjectInfo> ();
}

[System.Serializable]
public class craftingIngredientObjectInfo
{
	public string Name;

	public int amountRequired;
}

[System.Serializable]
public class craftingStatInfo
{
	public string statName;

	public int valueRequired;

	[Space]

	public bool useStatValue;

	public int statValueToUse;
}

[System.Serializable]
public class processMaterialInfo
{
	public string materialProcessedName;

	public string processName;

	public int materialAmountNeeded;

	public int materialAmountToObtain;
}