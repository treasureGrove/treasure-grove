using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Character Full Armor-Cloth Template", menuName = "GKC/Create Full Armor-Cloth Template", order = 51)]
public class fullArmorClothTemplate : ScriptableObject
{
	public string Name;

	public int ID = 0;

	[Space]

	public List<string> fullArmorPiecesList = new List<string> ();

	[Space]

	public List<armorClothStatsInfo> armorClothStatsInfoList = new List<armorClothStatsInfo> ();
}