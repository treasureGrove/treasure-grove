using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Inventory List Data", menuName = "GKC/Create Inventory List Data", order = 51)]
public class inventoryListManagerData : ScriptableObject
{
	public List<inventoryCategoryInfo> inventoryList = new List<inventoryCategoryInfo> ();
}
