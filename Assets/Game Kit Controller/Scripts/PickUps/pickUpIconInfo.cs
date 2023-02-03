using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class pickUpIconInfo
{
	public int ID;
	public string name;
	public GameObject iconObject;
	public GameObject texture;
	public GameObject target;

	public Transform targetTransform;

	public bool iconActive;

	public RawImage pickupIconImage;

	public bool paused;
	public RectTransform iconRectTransform;
}
