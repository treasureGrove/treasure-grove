using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class inventoryCategoryInfo
{
	public string Name;
	public Texture cateogryTexture;

	public GameObject emptyInventoryPrefab;

	public List<inventoryInfo> inventoryList = new List<inventoryInfo> ();

	public inventoryCategoryInfo ()
	{
		Name = "New Category";
	}
}
