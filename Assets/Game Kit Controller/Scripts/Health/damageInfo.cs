using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class damageInfo
{
	public GameObject projectile;
	public GameObject objectToDamage;
	public float damageAmount;
	public Vector3 direction;
	public Vector3 position;
	public GameObject projectileOwner;
	public bool damageConstant;
	public bool searchClosestWeakSpot;
	public bool ignoreShield;
	public bool ignoreDamageInScreen;
}