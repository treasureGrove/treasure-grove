using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceWeaponListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistanceWeaponListByPlayerInfo> playerWeaponList = new List<persistanceWeaponListByPlayerInfo> ();
}

[System.Serializable]
public class persistanceWeaponListByPlayerInfo
{
	public int playerID;
	public List<persistanceWeaponInfo> weaponList = new List<persistanceWeaponInfo> ();
}

[System.Serializable]
public class persistanceWeaponInfo
{
	public string Name;
	public int index;
	public bool isWeaponEnabled;
	public bool isCurrentWeapon;
	public int remainingAmmo;

	public bool weaponUsesAttachment;
	public List<persistanceWeaponAttachmentPlaceInfo> weaponAttachmentPlaceList = new List<persistanceWeaponAttachmentPlaceInfo> ();
}

[System.Serializable]
public class persistanceWeaponAttachmentPlaceInfo
{
	public bool attachmentPlaceEnabled;
	public List<persistanceAttachmentInfo> attachmentList = new List<persistanceAttachmentInfo> ();
}

[System.Serializable]
public class persistanceAttachmentInfo
{
	public bool attachmentEnabled;
	public bool attachmentActive;
}

