using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class simpleScannerSystem : MonoBehaviour
{
	public bool simpleScannerCameraEnabled = true;

	public Camera mainCamera;

	public LayerMask regularLayerMask;
	public LayerMask scannerLayerMask;

	public bool cameraActive;

	public bool useEventsOnCameraStateChange;
	public UnityEvent eventOnCameraActive;
	public UnityEvent eventOnCameraDeactivate;


	public void enableOrDisableCamera (bool state)
	{
		if (!simpleScannerCameraEnabled) {
			return;
		}

		if (cameraActive == state) {
			return;
		}

		cameraActive = state;

		if (useEventsOnCameraStateChange) {
			if (state) {
				eventOnCameraActive.Invoke ();
			} else {
				eventOnCameraDeactivate.Invoke ();
			}
		}

		if (state) {
			mainCamera.cullingMask = scannerLayerMask;
		} else {
			mainCamera.cullingMask = regularLayerMask;
		}
	}

	public void toggleCameraEnabledState ()
	{
		enableOrDisableCamera (!cameraActive);
	}
}
