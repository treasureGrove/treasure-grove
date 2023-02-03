using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class objectStatButtonInfo : MonoBehaviour
{
	public string objectCategoryName;

	public string objectName;

	[Space]

	public GameObject buttonGameObject;

	public Text statNameText;

	public RawImage statImage;

	public Text statValueText;

	public Text extraStatValueText;

	public Slider statSlider;

	public Toggle statToggle;

	[Space]

	public bool objectAssigned;
}
