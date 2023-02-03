using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Character Settings Template", menuName = "GKC/Create Character Template", order = 51)]
public class characterSettingsTemplate : ScriptableObject
{
	public int characterTemplateID = 0;
	public List<buildPlayer.settingsInfoCategory> settingsInfoCategoryList = new List<buildPlayer.settingsInfoCategory> ();
}
