using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Object Stats Template", menuName = "GKC/Create Object Stats Template", order = 51)]
public class objectStatsInfoTemplate : ScriptableObject
{
	public string Name;

	public int ID = 0;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool usedOnObject;

	public List<string> usableObjectList = new List<string> ();

	[Space]
	[Header ("Object Stats Settings")]
	[Space]

	public List<objectStatInfo> objectStatInfoList = new List<objectStatInfo> ();
}