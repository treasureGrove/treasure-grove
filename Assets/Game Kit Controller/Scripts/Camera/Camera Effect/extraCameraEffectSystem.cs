using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class extraCameraEffectSystem : MonoBehaviour
{
	public cameraEffectSystem maincameraEffectSystem;

	public Camera mainCamera;

	private void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (!maincameraEffectSystem.renderEffectActive) {
			return;
		}

		if (mainCamera == null) {
			mainCamera = Camera.main;
		}

		if (!mainCamera.enabled) {
			return;
		}

		maincameraEffectSystem.extraRenderImage (source, destination, mainCamera);
	}
}
