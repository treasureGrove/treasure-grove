using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class skillSlotPanelInfo
{
	public GameObject skillSlot;
	public GameObject slotActive;
	public GameObject slotLocked;
	public GameObject slotUnlocked;
	public Text slotSkillAmountText;
	public GameObject slotPressedIcon;

	public Transform confirmUseSkillPointsPanelPosition;
}
