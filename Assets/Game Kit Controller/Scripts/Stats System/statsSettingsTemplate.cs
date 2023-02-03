using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Stats Settings Template", menuName = "GKC/Create Stats Template", order = 51)]
public class statsSettingsTemplate : ScriptableObject
{
	public int characterTemplateID = 0;
	public List<playerStatsSystem.statTemplateInfo> statTemplateInfoList = new List<playerStatsSystem.statTemplateInfo> ();
}
