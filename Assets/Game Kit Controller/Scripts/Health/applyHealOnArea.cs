using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class applyHealOnArea : applyEffectOnArea
{
	public override void applyEffect (GameObject objectToAffect)
	{
		if (applyValueAtOnce) {
			valueToAdd = applyDamage.getMaxHealthAmount (objectToAffect);
		}

		if (!applyDamage.checkIfMaxHealth (objectToAffect)) {

			applyDamage.setHeal (valueToAdd, objectToAffect);
	
		} else {
			removeDetectedObject (objectToAffect);
		}
	}
}