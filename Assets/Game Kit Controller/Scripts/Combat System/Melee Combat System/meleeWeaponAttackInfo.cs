using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Melee Weapon Attack Info Template", menuName = "GKC/Create Melee Weapon Attack Info Template", order = 51)]
public class meleeWeaponAttackInfo : ScriptableObject
{
	public string Name;
	public int ID = 0;

	public meleeWeaponInfo mainMeleeWeaponInfo;
}