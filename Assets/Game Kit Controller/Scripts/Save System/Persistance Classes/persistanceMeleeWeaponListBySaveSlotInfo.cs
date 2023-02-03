using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceMeleeWeaponListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistanceMeleeWeaponInfo> meleeWeaponList = new List<persistanceMeleeWeaponInfo> ();
}

[System.Serializable]
public class persistanceMeleeWeaponInfo
{
	public int playerID;
	public List<persistanceMeleeInfo> meleeList = new List<persistanceMeleeInfo> ();
}

[System.Serializable]
public class persistanceMeleeInfo
{
	public int weaponActiveIndex;
	public bool isCurrentWeapon;
}