using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistancePlayerSkillsListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistancePlayerCategorySkillInfo> playerSkillsList = new List<persistancePlayerCategorySkillInfo> ();
}

[System.Serializable]
public class persistancePlayerCategorySkillInfo
{
	public int playerID;
	public List<persistanceCategorySkillInfo> categorySkillsList = new List<persistanceCategorySkillInfo> ();
}

[System.Serializable]
public class persistanceCategorySkillInfo
{
	public List<persistanceSkillInfo> skillsList = new List<persistanceSkillInfo> ();
}

[System.Serializable]
public class persistanceSkillInfo
{
	public bool skillUnlocked;

	public bool skillActive;

	public bool skillComplete;

	public float currentValue;
	public bool currentBoolState;

	public int currentSkillLevel;

	public persistanceSkillInfo (persistanceSkillInfo obj)
	{
		skillUnlocked = obj.skillUnlocked;

		skillActive = obj.skillActive;

		skillComplete = obj.skillComplete;

		currentValue = obj.currentValue;
		currentBoolState = obj.currentBoolState;

		currentSkillLevel = obj.currentSkillLevel;
	}

	public persistanceSkillInfo ()
	{

	}
}