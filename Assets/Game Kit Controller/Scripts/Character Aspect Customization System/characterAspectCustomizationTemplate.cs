using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Character Aspect Template", menuName = "GKC/Create Character Aspect Template", order = 51)]
public class characterAspectCustomizationTemplate : ScriptableObject
{
	public string Name;
	public int characterAspectTemplateID = 0;

	[Space]

	public List<characterCustomizationTypeInfo> characterCustomizationTypeInfoList = new List<characterCustomizationTypeInfo> ();
}