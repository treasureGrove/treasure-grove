using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slideZoneSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool slideZoneActive = true;

	public float slideSpeedMultiplier = 1;

	public bool setEnterStateActive = true;

	public bool setExitStateActive = true;

	public bool justUpdateSlideTransform;

	[Space]
	[Header ("Jump Settings")]
	[Space]

	public bool justActivateJumpStateOnSlide;

	public Vector3 impulseOnJump;

	[Space]
	[Header ("Gravity Settings")]
	[Space]

	public bool justSetAdhereToSurfacesActiveState;

	public bool adhereToSurfacesActiveState;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform slideTransform;

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
			if (!setEnterStateActive) {
				return;
			}

			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior slideExternalControllerBehavior = currentPlayerComponentsManager.getSlideExternalControllerBehavior ();

				if (slideExternalControllerBehavior != null) {
					slideSystem currentSlideSystem = slideExternalControllerBehavior.GetComponent<slideSystem> ();

					if (justActivateJumpStateOnSlide) {
						currentSlideSystem.setJumpActive (impulseOnJump);

						return;
					}

					if (justSetAdhereToSurfacesActiveState) {
						currentSlideSystem.setAdhereToSurfacesActiveState (adhereToSurfacesActiveState);

						return;
					}

					currentSlideSystem.setCurrentSlideTransform (slideTransform);

					if (justUpdateSlideTransform) {
						return;
					}

					currentSlideSystem.setSlideSpeedMultiplier (slideSpeedMultiplier);

					currentSlideSystem.setCheckIfDetectSlideActiveState (true, true);
				}
			}
		} else {
			if (!setExitStateActive) {
				return;
			}

			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior slideExternalControllerBehavior = currentPlayerComponentsManager.getSlideExternalControllerBehavior ();

				if (slideExternalControllerBehavior != null) {
					slideSystem currentSlideSystem = slideExternalControllerBehavior.GetComponent<slideSystem> ();

					currentSlideSystem.setCheckIfDetectSlideActiveState (false, false);

					currentSlideSystem.setCurrentSlideTransform (null);
				}
			}
		}
	}
}
