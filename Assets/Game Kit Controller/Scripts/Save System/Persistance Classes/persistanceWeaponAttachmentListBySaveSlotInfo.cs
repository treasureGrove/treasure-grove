using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceWeaponAttachmentListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistanceAttachmentWeaponListByPlayerInfo> playerAttachmentWeaponList = new List<persistanceAttachmentWeaponListByPlayerInfo> ();
}

[System.Serializable]
public class persistanceAttachmentWeaponListByPlayerInfo
{
	public int playerID;
	public List<persistanceAttachmentWeaponPlaceInfo> attachmentWeaponPlaceList = new List<persistanceAttachmentWeaponPlaceInfo> ();
}

[System.Serializable]
public class persistanceAttachmentWeaponPlaceInfo
{
	public bool attachmentPlaceEnabled;
	public List<persistanceAttachmentWeaponInfo> attachmentWeaponList = new List<persistanceAttachmentWeaponInfo> ();
}

[System.Serializable]
public class persistanceAttachmentWeaponInfo
{
	public bool attachmentEnabled;
	public bool attachmentActive;
}
