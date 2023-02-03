using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class weaponAmmoUsingPlayerStat : MonoBehaviour
{
	public playerStatsSystem mainPlayerStatsSystem;
	public string statToUseName;
	public float amountToUsePerShoot;

	public void useWeaponAmmo ()
	{
		mainPlayerStatsSystem.usePlayerStat (statToUseName, amountToUsePerShoot);
	}
}
