using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeCombatAttackStartState : MonoBehaviour
{
	public bool attackResetEnabled = true;

	public grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;

	public void resetActivateDamageTriggerCoroutine ()
	{
		if (attackResetEnabled) {
			mainGrabbedObjectMeleeAttackSystem.resetActivateDamageTriggerCoroutine ();
		}
	}
}
