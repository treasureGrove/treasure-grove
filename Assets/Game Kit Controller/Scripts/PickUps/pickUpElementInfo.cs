using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class pickUpElementInfo
{
	public string pickUpType;
	public List<pickUpTypeElementInfo> pickUpTypeList = new List<pickUpTypeElementInfo> ();
	public bool useGeneralIcon;
	public Texture generalIcon;

	public bool useCustomIconPrefab;
	public GameObject customIconPrefab;

	[System.Serializable]
	public class pickUpTypeElementInfo
	{
		public string Name;
		public GameObject pickUpObject;

		public Texture pickupIcon;

		public bool useCustomPickupIcon;

		public bool useCustomIconPrefab;
		public GameObject customIconPrefab;
	}
}