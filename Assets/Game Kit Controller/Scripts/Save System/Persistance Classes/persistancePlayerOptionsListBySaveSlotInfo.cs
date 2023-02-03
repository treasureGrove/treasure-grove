using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistancePlayerOptionsListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistancePlayerOptionsInfo> playerOptionsList = new List<persistancePlayerOptionsInfo> ();
}

[System.Serializable]
public class persistancePlayerOptionsInfo
{
	public int playerID;
	public List<persistanceOptionsInfo> optionsList = new List<persistanceOptionsInfo> ();
}

[System.Serializable]
public class persistanceOptionsInfo
{
	public string optionName;

	public bool currentScrollBarValue;
	public float currentSliderValue;
	public bool currentToggleValue;
	public int currentDropDownValue;

	public persistanceOptionsInfo ()
	{

	}
}