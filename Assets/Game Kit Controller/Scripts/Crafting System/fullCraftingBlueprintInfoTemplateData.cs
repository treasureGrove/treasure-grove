using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Full Crafting Blueprint Template List", menuName = "GKC/Create Full Crafting Blueprint Template List", order = 51)]
public class fullCraftingBlueprintInfoTemplateData : ScriptableObject
{
	public string Name;

	public int ID = 0;

	[Space]
	[Header ("Crafting Blueptint Template Data List Settings")]
	[Space]

	public List<craftingBlueprintInfoTemplateData> craftingBlueprintInfoTemplateDataList = new List<craftingBlueprintInfoTemplateData> ();
}