using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class mapIconType
{
	public string typeName;
	public RectTransform icon;
	public bool showIconPreview;
	public bool enabled;

	public bool useCompassIcon;
	public GameObject compassIconPrefab;
	public float verticalOffset;
}
