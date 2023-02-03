using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Crafting Blueprint Template List", menuName = "GKC/Create Crafting Blueprint Template List", order = 51)]
public class craftingBlueprintInfoTemplateData : ScriptableObject
{
	public string Name;

	public int ID = 0;

	[Space]

	[TextArea (3, 5)] public string description;

	[Space]
	[Space]

	public List<craftingBlueprintInfoTemplate> craftingBlueprintInfoTemplateList = new List<craftingBlueprintInfoTemplate> ();
}