using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectStatsSystemOnMeleeWeapons : objectStatsSystem
{


	public override void getMainCharacterGameObject ()
	{
		grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

		if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
			mainCharacterGameObject = currentGrabPhysicalObjectMeleeAttackSystem.getCurrentCharacter ();
		}
	}
}
