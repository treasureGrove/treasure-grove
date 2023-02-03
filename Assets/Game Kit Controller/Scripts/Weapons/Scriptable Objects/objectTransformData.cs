using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Object Transform Data", menuName = "GKC/Create Object Transform Data", order = 51)]
public class objectTransformData : ScriptableObject
{
	public List<objectTransformInfo> objectTransformInfoList = new List<objectTransformInfo> ();
}
