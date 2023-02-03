using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class applyDamageOnArea : applyEffectOnArea
{
	[Space]
	[Header ("Custom Settings")]
	[Space]

	public bool ignoreShield;

	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	public int damageTypeID = -1;

	public bool damageObjectOverTimeOnExit;
	public float damageOverTimeOnExitAmount;
	public float damageOverTimeOnExitDuration;
	public float damageOverTimeOnExitRate;
	public bool damageOverTimeOnExitUntilDeath;

	public GameObject attackerGameObject;

	bool attackerAssigned;

	public override void applyEffect (GameObject objectToAffect)
	{
		if (applyValueAtOnce) {
			valueToAdd = applyDamage.getCurrentHealthAmount (objectToAffect);
		}

		if (valueToAdd > 0) {
			if (!attackerAssigned) {
				if (attackerGameObject == null) {
					attackerGameObject = gameObject;
				}

				attackerAssigned = true;
			}

			applyDamage.checkHealth (gameObject, objectToAffect, valueToAdd, Vector3.zero, objectToAffect.transform.position + objectToAffect.transform.up, attackerGameObject, 
				true, true, ignoreShield, false, canActivateReactionSystemTemporally, damageReactionID, damageTypeID);

			if (applyDamage.checkIfDead (objectToAffect)) {
				removeDetectedObject (objectToAffect);
			}
		}
	}

	public override void checkApplyEffectOnExit (GameObject objectToAffect)
	{
		if (applyEffectOnExit && damageObjectOverTimeOnExit) {
			applyDamage.setDamageTargetOverTimeState (objectToAffect, 0, damageOverTimeOnExitDuration, damageOverTimeOnExitAmount, damageOverTimeOnExitRate, damageOverTimeOnExitUntilDeath, damageTypeID);
		}
	}
}