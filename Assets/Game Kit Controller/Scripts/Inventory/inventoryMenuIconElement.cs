using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class inventoryMenuIconElement : MonoBehaviour
{
	public Button button;
	public Text iconName;
	public Text amount;
	public RawImage icon;
	public GameObject pressedIcon;
	public GameObject combineIcon;
	public GameObject equipedIcon;
	public GameObject activeSlotContent;
	public GameObject emptySlotContent;

	public GameObject amountPerUnitPanel;
	public Text amountPerUnitText;

	public GameObject infiniteAmountIcon;

	public RectTransform inventoryOptionsOnSlotPanelPosition;

	public GameObject durabilitySliderGameObject;
	public Slider durabilitySlider;

	public GameObject objectBrokenIconGameObject;
}