using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class healthElementInfo
{
	public bool used;
	public GameObject healthElementGameObject;
	public Text healthSpot;
	public Transform target;
	public RectTransform healthElementRectTransform;
}
