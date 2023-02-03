using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setCustomCharacterStateTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool triggerEnabled = true;

	[Space]
	[Header ("Generic Character Settings")]
	[Space]

	public bool setRegularCharacter;

	public string customCharacterNameToConfigure;


	public void checkCustomCharacterToActivate (GameObject newCharacterGameObject)
	{
		if (!triggerEnabled) {
			return;
		}

		if (newCharacterGameObject == null) {
			return;
		}

		playerComponentsManager currentPlayerComponentsManager = newCharacterGameObject.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			customCharacterControllerManager currentCustomCharacterControllerManager = currentPlayerComponentsManager.getCustomCharacterControllerManager ();

			if (currentCustomCharacterControllerManager != null) {
				if (setRegularCharacter) {
					if (currentCustomCharacterControllerManager.isCustomCharacterControllerActive ()) {
						currentCustomCharacterControllerManager.disableCustomCharacterControllerState ();
					}
				} else {
					currentCustomCharacterControllerManager.setCustomCharacterControllerState (customCharacterNameToConfigure);
				}
			}
		}
	}
}
