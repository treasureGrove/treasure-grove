using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class healthSliderInfo
{
	public string Name;
	public Transform sliderOwner;
	public GameObject sliderGameObject;
	public RectTransform sliderRectTransform;

	public AIHealtSliderInfo sliderInfo;

	public bool sliderOwnerLocated;

	public bool sliderCanBeShown = true;

	public bool useSliderOffset;

	public Vector3 sliderOffset;

	public Slider healthSlider;

	public Slider shieldSlider;

	public bool iconCurrentlyEnabled;

	public int ID;

	public bool useHealthSlideInfoOnScreen;
}