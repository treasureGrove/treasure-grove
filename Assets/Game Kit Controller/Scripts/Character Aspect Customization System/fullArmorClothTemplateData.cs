using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Character Full Armor-Cloth Template List", menuName = "GKC/Create Full Armor-Cloth Piece Template List", order = 51)]
public class fullArmorClothTemplateData : ScriptableObject
{
	public string Name;

	public int ID = 0;

	public List<fullArmorClothTemplate> fullArmorClothTemplateList = new List<fullArmorClothTemplate> ();
}