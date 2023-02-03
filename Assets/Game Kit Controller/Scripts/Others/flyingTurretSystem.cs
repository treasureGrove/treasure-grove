using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyingTurretSystem : MonoBehaviour
{
	[Header ("Turret Settings")]
	[Space]

	public bool flyingTurretEnabled = true;

	public bool setManualFireOnTurretEnabled;

	public bool manualFireInputEnabled;

	public bool reactivateAutoShootAfterDelay;

	public float delayToReactivateAutoShoot;

	public Transform currentCameraTransformDirection;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool flyingTurretActive;
	public bool manualControlActive;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject flyingTurretObjectPrefab;

	public GameObject flyingTurretObject;

	public Transform objectToFollow;

	public AITurret currentAITurret;


	public void enableOrDisableFlyingTurret ()
	{
		setFlyingTurretActiveState (!flyingTurretActive);
	}

	public void setFlyingTurretActiveState (bool state)
	{
		flyingTurretActive = state;

		if (flyingTurretObject == null) {

			flyingTurretObject = (GameObject)Instantiate (flyingTurretObjectPrefab, objectToFollow.position, objectToFollow.rotation);

			followObjectPositionUpdateSystem currentFollowObjectPositionUpdateSystem = flyingTurretObject.GetComponentInChildren<followObjectPositionUpdateSystem> ();
		
			if (currentFollowObjectPositionUpdateSystem != null) {
				currentFollowObjectPositionUpdateSystem.setObjectToFollow (objectToFollow);
			}

			currentAITurret = flyingTurretObject.GetComponentInChildren<AITurret> ();
		}

		if (flyingTurretObject != null) {
			if (state) {
				flyingTurretObject.SetActive (state);
			}

			if (currentAITurret != null) {
				currentAITurret.setNewTurretAttacker (objectToFollow.gameObject);

				enableOrDisableManualStateOnTurret (setManualFireOnTurretEnabled);	

				if (state) {
					if (setManualFireOnTurretEnabled) {
						currentAITurret.setNewCurrentCameraTransformDirection (currentCameraTransformDirection);
					}
				}

				manualControlActive = manualFireInputEnabled;

				if (manualFireInputEnabled) {
					if (state) {
						if (!currentAITurret.weaponsActive) {
							currentAITurret.inputSetWeaponsState ();
						}
					} else {
						enableOrDisableManualFireOnTurret (false);
					}
				}
			}

			if (!state) {
				flyingTurretObject.SetActive (state);
			}
		}
	}

	public void enableOrDisableManualFireOnTurret (bool state)
	{
		if (flyingTurretActive) {
			if (manualFireInputEnabled) {
				if (currentAITurret != null) {
					if (state) {
						if (!currentAITurret.controlOverriden) {
							currentAITurret.overrideTurretControlState (true);
						}

						if (!currentAITurret.weaponsActive) {
							currentAITurret.inputSetWeaponsState ();
						}
					}

					currentAITurret.inputSetShootState (state);

					if (!state) {
						if (reactivateAutoShootAfterDelay) {
							currentAITurret.disableOverrideAfterDelay (delayToReactivateAutoShoot);
						}
					}
				}
			}
		}
	}

	public void enableOrDisableManualStateOnTurret (bool state)
	{
		if (currentAITurret != null) {
			currentAITurret.overrideTurretControlState (state);

			manualControlActive = state;
		}
	}

	public void inputToggleManualFire ()
	{
		if (flyingTurretActive) {
			enableOrDisableManualStateOnTurret (!manualControlActive);
		}
	}
}
