using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setCharacterAffectedState : MonoBehaviour
{
	public bool affectStateEnabled = true;

	public string characterStateAffectedName;

	public float stateDuration;

	public float stateAmount;

	public void checkObjectDetected (GameObject objectToDamage)
	{
		if (!affectStateEnabled || objectToDamage == null) {
			return;
		}

		playerComponentsManager mainPlayerComponentsManager = objectToDamage.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			characterPropertiesSystem currentCharacterPropertiesSystem = mainPlayerComponentsManager.getCharacterPropertiesSystem ();

			if (currentCharacterPropertiesSystem != null) {
				currentCharacterPropertiesSystem.activateStateAffected (characterStateAffectedName, stateDuration, stateAmount);
			}
		}
	}
}