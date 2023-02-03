using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Player Prefs Custom Data", menuName = "GKC/Create Player Prefs Custom Data", order = 51)]
public class playerPrefsCustomData : ScriptableObject
{
	public List<playerPrefsCustomInfo> playerPrefsCustomInfoList = new List<playerPrefsCustomInfo> ();
}

