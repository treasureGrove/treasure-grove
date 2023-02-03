using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistanceDialogContentListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistancePlayerDialogContentInfo> playerDialogContentList = new List<persistancePlayerDialogContentInfo> ();
}

[System.Serializable]
public class persistancePlayerDialogContentInfo
{
	public int playerID;
	public List<persistanceDialogContentInfo> dialogContentList = new List<persistanceDialogContentInfo> ();
}

[System.Serializable]
public class persistanceDialogContentInfo
{
	public int dialogContentID;

	public int dialogContentScene;
	public int currentDialogIndex;

	public persistanceDialogContentInfo ()
	{

	}
}