using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Character Aspect Template List", menuName = "GKC/Create Character Aspect Template List", order = 51)]
public class characterAspectCustomizationTemplateData : ScriptableObject
{
	public string Name;

	public int ID = 0;

	public List<characterAspectCustomizationTemplate> characterAspectCustomizationTemplateList = new List<characterAspectCustomizationTemplate> ();
}