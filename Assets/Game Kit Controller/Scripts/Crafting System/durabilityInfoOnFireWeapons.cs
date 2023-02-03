using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class durabilityInfoOnFireWeapons : durabilityInfo
{
	public override void updateDurabilityAmountState ()
	{
		base.updateDurabilityAmountState ();


	}

	public override void setFullDurabilityState ()
	{
		base.setFullDurabilityState ();


	}

	public override void getMainCharacterGameObject ()
	{
		IKWeaponSystem currentIKWeaponSystem = GetComponent<IKWeaponSystem> ();

		if (currentIKWeaponSystem != null) {
			mainCharacterGameObject = currentIKWeaponSystem.getPlayerGameObject ();
		}
	}
}