using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freeClimbZoneSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool climbZoneActive = true;

	public bool activateClimbCheckOnTriggerEnter = true;

	public bool stopClimbOnTriggerExit;

	public bool activateClimbStateAutomaticallyOnEnter;

	public bool climbAutomaticallyOnlyIfPlayerOnAir;

	[Space]
	[Header ("Action Settings")]
	[Space]

	public bool ignoreSurfaceToClimbEnabled;

	public bool allowClimbSurfaceOnInputEnabled;

	public bool activateAutoSlideDownOnSurface;

	public bool movementInputDisabledOnSurface;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool setPlayerAsChild;

	public Transform playerParentTransform;

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
		if (!climbZoneActive) {
			return;
		}

		if (!col.gameObject.CompareTag (tagToCheck)) {
			return;
		}

		if (isEnter) {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior freeClimbExteralControllerBehavior = currentPlayerComponentsManager.getFreeClimbExternalControllerBehavior ();

				if (freeClimbExteralControllerBehavior != null) {
					freeClimbSystem currentFreeClimbSystem = freeClimbExteralControllerBehavior.GetComponent<freeClimbSystem> ();

					if (activateClimbCheckOnTriggerEnter) {
						currentFreeClimbSystem.setCheckIfDetectClimbActiveState (true);

						if (activateClimbStateAutomaticallyOnEnter) {
							bool canActivateClimbStateResult = true;

							if (climbAutomaticallyOnlyIfPlayerOnAir) {
								if (currentPlayerComponentsManager.getPlayerController ().isPlayerOnGround ()) {
									canActivateClimbStateResult = false;
								}
							} 

							if (canActivateClimbStateResult) {
								currentFreeClimbSystem.activateGrabSurface ();
							}
						}

//						if (movementInputDisabledOnSurface) {
//							currentFreeClimbSystem.setMovementInputDisabledOnSurfaceState (true);
//						}
					}
				}
			}
		} else {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior freeClimbExteralControllerBehavior = currentPlayerComponentsManager.getFreeClimbExternalControllerBehavior ();

				if (freeClimbExteralControllerBehavior != null) {
					freeClimbSystem currentFreeClimbSystem = freeClimbExteralControllerBehavior.GetComponent<freeClimbSystem> ();

					if (stopClimbOnTriggerExit) {
						currentFreeClimbSystem.setCheckIfDetectClimbActiveState (false);
					}

//					if (movementInputDisabledOnSurface) {
//						currentFreeClimbSystem.setMovementInputDisabledOnSurfaceState (false);
//					}
				}
			}
		}
	}

	public Transform checkPlayerParentState ()
	{
		if (setPlayerAsChild) {
			return playerParentTransform;
		} 

		return null;
	}

	public bool isIgnoreSurfaceToClimbEnabled ()
	{
		return ignoreSurfaceToClimbEnabled;
	}

	public void setIgnoreSurfaceToClimbEnabledState (bool state)
	{
		ignoreSurfaceToClimbEnabled = state;
	}

	public bool isAllowClimbSurfaceOnInputEnabled ()
	{
		return allowClimbSurfaceOnInputEnabled;
	}
}
