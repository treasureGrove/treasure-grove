using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Melee Weapon Transform Data", menuName = "GKC/Create Melee Weapon Transform Data", order = 51)]
public class meleeWeaponTransformData : ScriptableObject
{
	public string meleeWeaponName;
	public objectTransformInfo mainObjectTransformInfo;
}
