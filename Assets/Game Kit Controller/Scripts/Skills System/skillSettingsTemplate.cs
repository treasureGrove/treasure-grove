using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Skills Settings Template", menuName = "GKC/Create Skills Template", order = 51)]
public class skillSettingsTemplate : ScriptableObject
{
	public int characterTemplateID = 0;
	public List<playerSkillsSystem.skillTemplateCategoryInfo> skillTemplateCategoryInfoList = new List<playerSkillsSystem.skillTemplateCategoryInfo> ();
}
