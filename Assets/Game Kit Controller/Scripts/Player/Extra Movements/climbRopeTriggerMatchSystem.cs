using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class climbRopeTriggerMatchSystem : MonoBehaviour
{
	public bool movementZoneActive = true;

	public bool checkOnTriggerEnter = true;
	public bool checkOnTriggerExit = true;

	public string tagToCheck;

	public climbRopeTriggerSystem mainClimbRopeTriggerSystem;

	GameObject currentPlayer;


	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (!movementZoneActive) {
			return;
		}

		if (isEnter) {
			if (!checkOnTriggerEnter) {
				return;
			}
		} else {
			if (!checkOnTriggerExit) {
				return;
			}
		}

		if (!col.gameObject.CompareTag (tagToCheck)) {
			return;
		}

		if (isEnter) {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior movementExternalControllerBehavior = currentPlayerComponentsManager.getClimbRopeExternaControllerBehavior ();

				if (movementExternalControllerBehavior != null) {
					climbRopeSystem currentClimbRopeSystem = movementExternalControllerBehavior.GetComponent<climbRopeSystem> ();

					currentClimbRopeSystem.addClimbRopeTriggerMatchSystem (this);
				}
			}
		} else {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior movementExternalControllerBehavior = currentPlayerComponentsManager.getClimbRopeExternaControllerBehavior ();

				if (movementExternalControllerBehavior != null) {
					climbRopeSystem currentClimbRopeSystem = movementExternalControllerBehavior.GetComponent<climbRopeSystem> ();

					currentClimbRopeSystem.removeClimbRopeTriggerMatchSystem (this);
				}
			}
		}
	}
}
