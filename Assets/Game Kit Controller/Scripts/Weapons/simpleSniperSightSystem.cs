using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleSniperSightSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool sniperSightThirdPersonEnabled = true;
	public bool sniperSightFirstPersonEnabled = true;

	public string sniperCameraThirdPersonStateName = "Sniper Camera View Third Person";
	public string sniperCameraFirstPersonStateName = "Sniper Camera View First Person";

	public float newCameraFovValue = 10;

	public bool changeFovSmoothly;
	public float changeFovSpeed;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool sniperSightActive;

	public string previuousCameraState;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject sniperSightPanelGameObject;
	public playerCamera mainPlayerCamera;
	public playerController mainPlayerController;
	public Camera weaponsCamera;

	Coroutine sniperSightCoroutine;

	public void enableOrDisableSniperSight (bool state)
	{
		bool isFirstPersonActive = mainPlayerCamera.isFirstPersonActive ();

		if (isFirstPersonActive) {
			if (!sniperSightFirstPersonEnabled) {
				return;
			}
		} else {
			if (!sniperSightThirdPersonEnabled) {
				return;
			}
		}

		if (!mainPlayerCamera.isCameraTypeFree ()) {
			return;
		}
		 
		setSniperSightState (state, isFirstPersonActive);
	}

	void setSniperSightState (bool state, bool isFirstPersonActive)
	{
		sniperSightActive = state;

		if (sniperSightPanelGameObject.activeSelf != sniperSightActive) {
			sniperSightPanelGameObject.SetActive (sniperSightActive);
		}

		if (sniperSightActive) {
			previuousCameraState = mainPlayerCamera.getCurrentStateName ();

			if (sniperSightCoroutine != null) {
				StopCoroutine (sniperSightCoroutine);
			}

			sniperSightCoroutine = StartCoroutine (enableSniperSightCoroutine ());

		} else {
			if (previuousCameraState != "") {
				mainPlayerCamera.setCameraState (previuousCameraState);

				setCameraStateValues ();

				if (isFirstPersonActive) {
					weaponsCamera.enabled = true;
				}
			}

			previuousCameraState = "";
		}

		mainPlayerCamera.setPauseChangeCameraFovActiveState (sniperSightActive);
	}

	public void disableSniperSight ()
	{
		setSniperSightState (false, mainPlayerCamera.isFirstPersonActive ());
	}


	IEnumerator enableSniperSightCoroutine ()
	{
		yield return new WaitForSeconds (0.001f);

		bool isFirstPersonActive = mainPlayerCamera.isFirstPersonActive ();

		if (isFirstPersonActive) {
			mainPlayerCamera.setCameraState (sniperCameraFirstPersonStateName);

			weaponsCamera.enabled = false;
		} else {
			mainPlayerCamera.setCameraState (sniperCameraThirdPersonStateName);
		}

		setCameraStateValues ();

		mainPlayerCamera.stopFovChangeCoroutine ();

		mainPlayerCamera.stopCameraFovStartAndEndCoroutine ();

		if (changeFovSmoothly) {
			mainPlayerCamera.setMainCameraFov (newCameraFovValue, changeFovSpeed);
		} else {
			mainPlayerCamera.quickChangeFovValue (newCameraFovValue);
		}
	}

	void setCameraStateValues ()
	{
		mainPlayerCamera.resetCurrentCameraStateAtOnce ();

		mainPlayerCamera.configureCameraAndPivotPositionAtOnce ();

		mainPlayerCamera.quickChangeFovValue (mainPlayerCamera.getOriginalCameraFov ());
	}
}
