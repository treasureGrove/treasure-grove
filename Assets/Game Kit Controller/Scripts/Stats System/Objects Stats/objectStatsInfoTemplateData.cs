using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Object Stats Template List", menuName = "GKC/Create Object Stats Template List", order = 51)]
public class objectStatsInfoTemplateData : ScriptableObject
{
	public string Name;

	public int ID = 0;

	[Space]

	[TextArea (3, 5)] public string description;

	[Space]
	[Space]

	public List<objectStatsInfoTemplate> objectStatsInfoTemplateList = new List<objectStatsInfoTemplate> ();
}