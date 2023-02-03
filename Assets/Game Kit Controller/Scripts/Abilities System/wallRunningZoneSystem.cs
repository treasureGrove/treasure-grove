using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallRunningZoneSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool wallRunningZoneActive = true;

	public bool wallRunningEnabled = true;
	public bool autoUseDownMovementOnWallRunningActive;
	public bool autoUseStopWallRunnigAfterDelay;

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
				externalControllerBehavior wallRunningExternalControllerBehavior = currentPlayerComponentsManager.getWallRunningExternalControllerBehavior ();

				if (wallRunningExternalControllerBehavior != null) {
					wallRunningSystem currentwallRunningSystem = wallRunningExternalControllerBehavior.GetComponent<wallRunningSystem> ();

					currentwallRunningSystem.setWallRunningEnabledState (wallRunningEnabled);

					currentwallRunningSystem.setAutoUseDownMovementOnWallRunningActiveState (autoUseDownMovementOnWallRunningActive);

					currentwallRunningSystem.setAutoUseStopWallRunningAfterDelayState (autoUseStopWallRunnigAfterDelay);
				}
			}
		} else {
			currentPlayer = col.gameObject;
		
			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior wallRunningExternalControllerBehavior = currentPlayerComponentsManager.getWallRunningExternalControllerBehavior ();

				if (wallRunningExternalControllerBehavior != null) {
					wallRunningSystem currentwallRunningSystem = wallRunningExternalControllerBehavior.GetComponent<wallRunningSystem> ();

					currentwallRunningSystem.setOriginalWallRunningEnabledState ();

					currentwallRunningSystem.setAutoUseDownMovementOnWallRunningActiveState (false);

					currentwallRunningSystem.setAutoUseStopWallRunningAfterDelayState (false);
				}
			}
		}
	}
}
