using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Character Armor-Cloth piece Template List", menuName = "GKC/Create Armor-Cloth Piece Template List", order = 51)]
public class armorClothPieceTemplateData : ScriptableObject
{
	public string Name;

	public int ID = 0;

	public List<armorClothPieceTemplate> armorClothPieceTemplateList = new List<armorClothPieceTemplate> ();
}