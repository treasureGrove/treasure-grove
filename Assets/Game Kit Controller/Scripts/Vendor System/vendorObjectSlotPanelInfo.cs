using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class vendorObjectSlotPanelInfo
{
	public string Name;

	public string categoryName;

	public Button button;

	public Text objectNameText;
	public Text objectAmountAvailableText;
	public Text objectPriceText;

	public GameObject pressedIcon;

	public Text objectLevelText;
}
