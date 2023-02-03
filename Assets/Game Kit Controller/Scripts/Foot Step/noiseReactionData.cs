using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Noise Reaction Data", menuName = "GKC/Create Noise Reaction Data", order = 51)]
public class noiseReactionData : ScriptableObject
{
	public List<noiseReactionInfo> noiseReactionInfoList = new List<noiseReactionInfo> ();
}


[System.Serializable]
public class noiseReactionInfo
{
	public string Name;
	public int ID;

	public bool noiseReactionEnabled = true;

	public bool checkNoiseOrigin;
	public bool runAwayFromNoiseOrigin;

	public bool useRemoteEventOnNoise;
	public string remoteEventOnNoise;

	public bool useMaxNoiseDistance;

	public float maxNoiseDistance;
}
