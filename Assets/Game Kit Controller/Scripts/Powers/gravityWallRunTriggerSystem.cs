using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityWallRunTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool wallRunningZoneActive = true;

	public bool activateRunAction;

	public bool stopWallRunningOnEnter;
	public bool stopWallRunningOnExit;

	public bool setNewAnimSpeedMultiplierOnWallRun;
	public float animSpeedMultiplierOnWallRun = 2;

	public bool setForceWalkWallActiveExternally;

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
		if (!wallRunningZoneActive) {
			return;
		}

		if (!col.gameObject.CompareTag (tagToCheck)) {
			return;
		}

		if (isEnter) {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				gravityWallRunSystem currentGravityWallRunSystem = currentPlayerComponentsManager.getGravityWallRunSystem ();

				if (currentGravityWallRunSystem != null) {
					if (stopWallRunningOnEnter) {
						currentGravityWallRunSystem.enableOrDisableWalkWallExternally (false, false);
					} else {
						if (setNewAnimSpeedMultiplierOnWallRun) {
							currentGravityWallRunSystem.setAnimSpeedMultiplierOnWallRunValue (animSpeedMultiplierOnWallRun);
						}

						if (setForceWalkWallActiveExternally) {
							currentGravityWallRunSystem.setForceWalkWallActiveExternallyState (true);
						}

						currentGravityWallRunSystem.enableOrDisableWalkWallExternally (true, activateRunAction);
					}
				}
			}
		} else {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				gravityWallRunSystem currentGravityWallRunSystem = currentPlayerComponentsManager.getGravityWallRunSystem ();

				if (currentGravityWallRunSystem != null) {
					if (stopWallRunningOnExit) {
						currentGravityWallRunSystem.enableOrDisableWalkWallExternally (false, false);
					}

					if (setNewAnimSpeedMultiplierOnWallRun) {
						currentGravityWallRunSystem.setOriginalAnimSpeedMultiplierOnWallRunValue ();
					}
						
					currentGravityWallRunSystem.setForceWalkWallActiveExternallyState (false);
				}
			}
		}
	}
}
