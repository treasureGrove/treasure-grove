using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customActionSystemTrigger : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool activateActionOnEnter;
	public bool activateActionOnExit;

	public string actionToActivateName;

	public bool stopActionActive;

	[Space]
	[Header ("Condition Settings")]
	[Space]

	public bool useMinAngleToActivateAction;
	public float minAngleToActivateAction;
	public bool checkOppositeAngle;
	public Transform actionSystemDirectionTransform;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool setCustomActionSystemTransform;
	public Transform customActionSystemTransform;

	public void setPlayerOnEnter (GameObject newPlayer)
	{
		if (activateActionOnEnter) {
			activateCustomAction (newPlayer);
		}
	}

	public void setPlayerOnExit (GameObject newPlayer)
	{
		if (activateActionOnExit) {
			activateCustomAction (newPlayer);
		}
	}

	public void activateCustomAction (GameObject newPlayer)
	{
		if (newPlayer == null) {
			return;
		}

		if (!checkActionCondition (newPlayer.transform)) {
			return;
		}

		playerComponentsManager currentPlayerComponentsManager = newPlayer.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			playerActionSystem currentPlayerActionSystem = currentPlayerComponentsManager.getPlayerActionSystem ();

			if (currentPlayerActionSystem != null) {
				if (stopActionActive) {
					currentPlayerActionSystem.stopCustomAction (actionToActivateName);
				} else {
					if (setCustomActionSystemTransform) {
						currentPlayerActionSystem.setCustomActionTransform (actionToActivateName, customActionSystemTransform);
					}

					currentPlayerActionSystem.activateCustomAction (actionToActivateName);
				}
			}
		}
	}

	bool checkActionCondition (Transform playerTransform)
	{
		if (useMinAngleToActivateAction) {
			if (actionSystemDirectionTransform == null) {
				actionSystemDirectionTransform = transform;
			}

			float currentAngleWithTarget = Vector3.SignedAngle (playerTransform.forward, actionSystemDirectionTransform.forward, playerTransform.up);

			if (Mathf.Abs (currentAngleWithTarget) > minAngleToActivateAction) {
				if (checkOppositeAngle) {
					currentAngleWithTarget = Mathf.Abs (currentAngleWithTarget) - 180;

					if (Mathf.Abs (currentAngleWithTarget) > minAngleToActivateAction) {
						return false;
					}
				} else {
					return false;
				}
			}
		}

		return true;
	}
}
