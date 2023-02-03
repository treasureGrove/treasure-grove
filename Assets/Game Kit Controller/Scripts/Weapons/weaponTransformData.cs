using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Objects Transform Data", menuName = "GKC/Create Objects Transform Data", order = 51)]
public class weaponTransformData : ScriptableObject
{
	public int weaponID = -1;
	public List<objectsTransformInfo> objectsTransformInfoList = new List<objectsTransformInfo> ();
}
