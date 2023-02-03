using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class durabilityInfoOnMeleeWeapons : durabilityInfo
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
		grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

		if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
			mainCharacterGameObject = currentGrabPhysicalObjectMeleeAttackSystem.getCurrentCharacter ();
		}
	}
}
