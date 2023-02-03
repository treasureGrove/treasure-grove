using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class craftingObjectButtonInfo : MonoBehaviour
{
	public string objectCategoryName;

	public string objectName;

	[Space]

	public GameObject buttonGameObject;

	public Text objectNameText;

	public RawImage objectImage;

	public Text amountAvailableToCreateText;

	public RectTransform buttonNotAvailableAmountPanel;

	public Slider objectSlider;

	[Space]

	public bool objectAssigned;
}
