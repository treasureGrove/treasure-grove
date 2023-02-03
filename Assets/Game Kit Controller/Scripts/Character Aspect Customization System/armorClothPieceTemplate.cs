using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Character Armor-Cloth piece Template", menuName = "GKC/Create Armor-Cloth Piece Template", order = 51)]
public class armorClothPieceTemplate : ScriptableObject
{
	public string Name;

	public string fullSetName;

	public int ID = 0;

	[Space]
	[Space]

	public List<armorClothStatsInfo> armorClothStatsInfoList = new List<armorClothStatsInfo> ();
}