using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallSlideJumpZoneSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool slideZoneActive = true;

	public float slideDownSpeedMultiplier = 1;

	public bool forceSlowDownOnSurfaceActive;

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
		if (!slideZoneActive) {
			return;
		}

		if (!col.gameObject.CompareTag (tagToCheck)) {
			return;
		}

		if (isEnter) {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior wallSlideJumpExteralControllerBehavior = currentPlayerComponentsManager.getWallSlideJumpExteralControllerBehavior ();

				if (wallSlideJumpExteralControllerBehavior != null) {
					wallSlideJumpSystem currentWallSlideJumpSystem = wallSlideJumpExteralControllerBehavior.GetComponent<wallSlideJumpSystem> ();
				
					currentWallSlideJumpSystem.setSlideDownSpeedMultiplier (slideDownSpeedMultiplier);

					currentWallSlideJumpSystem.setForceSlowDownOnSurfaceActiveState (forceSlowDownOnSurfaceActive);

					currentWallSlideJumpSystem.setCheckIfDetectSlideActiveState (true);
				}
			}
		} else {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior wallSlideJumpExteralControllerBehavior = currentPlayerComponentsManager.getWallSlideJumpExteralControllerBehavior ();

				if (wallSlideJumpExteralControllerBehavior != null) {
					wallSlideJumpSystem currentWallSlideJumpSystem = wallSlideJumpExteralControllerBehavior.GetComponent<wallSlideJumpSystem> ();

					currentWallSlideJumpSystem.setSlideDownSpeedMultiplier (1);

					currentWallSlideJumpSystem.setForceSlowDownOnSurfaceActiveState (false);

					currentWallSlideJumpSystem.setCheckIfDetectSlideActiveState (false);
				}
			}
		}
	}
}
