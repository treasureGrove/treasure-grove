using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class armorClothStatsInfo
{
	[Header ("Main Settings")]
	[Space]

	public string Name;

	public bool statIsAmount = true;

	[Space]
	[Header ("Stats Settings")]
	[Space]

	public float statAmount;

	public bool useStatMultiplier;

	public bool useRandomRange;
	public Vector2 randomRange;

	[Space]

	public bool newBoolState;

	[Space]
	[Header ("Abilities Settings")]
	[Space]

	public bool activateAbility;
	public string abilityToActivateName;

	[Space]
	[Header ("Skills Settings")]
	[Space]

	public bool unlockSkill;
	public string skillNameToUnlock;

	[Space]
	[Header ("Damage Type-Resistances Settings")]
	[Space]

	public bool setDamageTypeState;
	public string damageTypeName;
	public bool damageTypeState;

	public bool increaseDamageType;
	public float extraDamageType;

	public bool setObtainHealthOnDamageType;
	public bool obtainHealthOnDamageType;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEvent;

	public List<string> remoteEventNameListOnEquip = new List<string> ();

	public List<string> remoteEventNameListOnUnequip = new List<string> ();


	public armorClothStatsInfo ()
	{

	}
}