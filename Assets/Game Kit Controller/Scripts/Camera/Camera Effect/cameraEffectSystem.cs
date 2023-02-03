using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class cameraEffectSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool cameraEffectEnabled = true;

	public bool renderEffectActive = true;

	public bool startWithCameraEffectActive;

	public bool cameraEffectChangeDisabled;

	public int currentCameraEffectIndex;

	[Space]
	[Header ("Extra Cameras Effect")]
	[Space]

	public bool renderExtraCameras;

	public List<extraCameraEffectSystem> extraCameraEffectSystemList = new List<extraCameraEffectSystem> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool cameraEffectActive;
	public cameraEffect currentCameraEffect;

	[Space]
	[Header ("Camera Effect Info List")]
	[Space]

	public List<cameraEffectInfo> cameraEffectInfoList = new List<cameraEffectInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public Camera mainCamera;

	bool cameraEffectAtStartConfigured;


	void Start ()
	{
		if (startWithCameraEffectActive) {
			setCameraEffectActiveState (true);
		} else {
			this.enabled = false;
		}
	}

	private void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (!renderEffectActive) {
			return;
		}

		if (cameraEffectActive && currentCameraEffect != null && currentCameraEffect.useRenderEffect) {
			if (mainCamera == null) {
				mainCamera = Camera.main;
			}

			currentCameraEffect.renderEffect (source, destination, mainCamera);
		}
	}

	public void extraRenderImage (RenderTexture source, RenderTexture destination, Camera currentCamera)
	{
		if (!renderEffectActive) {
			return;
		}

		if (cameraEffectActive && currentCameraEffect != null && currentCameraEffect.useRenderEffect) {
			currentCameraEffect.renderEffect (source, destination, currentCamera);
		}
	}

	public void setCameraEffectByName (string cameraStateName)
	{
		int cameraEffectIndex = cameraEffectInfoList.FindIndex (a => a.Name == cameraStateName);

		if (cameraEffectIndex > -1) {
			currentCameraEffectIndex = cameraEffectIndex;

			setCameraEffectActiveState (true);
		}
	}

	public void setMainCameraEffectByName (string cameraStateName)
	{
		int cameraEffectIndex = cameraEffectInfoList.FindIndex (a => a.Name == cameraStateName);

		if (cameraEffectIndex > -1) {
			currentCameraEffectIndex = cameraEffectIndex;

			for (int i = 0; i < cameraEffectInfoList.Count; i++) {
				if (currentCameraEffectIndex == i) {
					cameraEffectInfoList [i].activeAsMainCameraEffect = true;
				} else {
					cameraEffectInfoList [i].activeAsMainCameraEffect = false;
				}
			}

			setCameraEffectActiveState (true);
		}
	}

	public void disableMainCameraEffectActiveState (string cameraStateName)
	{
		int cameraEffectIndex = cameraEffectInfoList.FindIndex (a => a.Name == cameraStateName);

		if (cameraEffectIndex > -1) {
			currentCameraEffectIndex = cameraEffectIndex;

			cameraEffectInfoList [currentCameraEffectIndex].activeAsMainCameraEffect = false;

			setCameraEffectActiveState (false);
		}
	}

	public void setIfCurrentCameraEffectRemainActive (bool state)
	{
		if (state) {
			checkIfMainCameraEffectsAreActive ();
		} else {
			if (cameraEffectActive) {
				setCameraEffectActiveState (false);
			}
		}
	}

	public void checkIfMainCameraEffectsAreActive ()
	{
		for (int i = 0; i < cameraEffectInfoList.Count; i++) {
			if (cameraEffectInfoList [i].activeAsMainCameraEffect) {
				print ("main camera effect active " + cameraEffectInfoList [i].Name);

				cameraEffectActive = true;
				currentCameraEffectIndex = i;

				setCameraEffectActiveState (true);

				return;
			}
		}
	}

	public bool canUseCameraEffect ()
	{
		if (!cameraEffectEnabled) {
			return false;
		}

		if (cameraEffectChangeDisabled && (!startWithCameraEffectActive || cameraEffectAtStartConfigured)) {
			return false;
		}

		return true;
	}

	public void enableOrDisableCameraEffectActive ()
	{
		setCameraEffectActiveState (!cameraEffectActive);
	}

	public void setCameraEffectActiveState (bool state)
	{
		if (!canUseCameraEffect ()) {
			return;
		}

		if (startWithCameraEffectActive) {
			cameraEffectAtStartConfigured = true;
		}

		if (!state) {
			for (int i = 0; i < cameraEffectInfoList.Count; i++) {
				if (cameraEffectInfoList [i].activeAsMainCameraEffect) {
					print ("main camera effect active " + cameraEffectInfoList [i].Name);

					cameraEffectActive = true;
					currentCameraEffectIndex = i;
				}
			}
		}

		cameraEffectActive = state;

		this.enabled = cameraEffectActive;

		if (cameraEffectActive) {
			currentCameraEffect = cameraEffectInfoList [currentCameraEffectIndex].mainCameraEffect;

			cameraEffectInfoList [currentCameraEffectIndex].eventToEnableEffect.Invoke ();

		} else {
			cameraEffectInfoList [currentCameraEffectIndex].eventToDisableEffect.Invoke ();
		}

		if (renderExtraCameras) {
			for (int i = 0; i < extraCameraEffectSystemList.Count; i++) {
				extraCameraEffectSystemList [i].enabled = cameraEffectActive;
			}
		}
	}

	public void setCurrentCameraEffect (cameraEffect newCameraEffect)
	{
		currentCameraEffect = newCameraEffect;
	}

	public void setNextCameraEffect ()
	{
		if (!canUseCameraEffect ()) {
			return;
		}

		cameraEffectInfoList [currentCameraEffectIndex].eventToDisableEffect.Invoke ();

		currentCameraEffectIndex++;

		if (currentCameraEffectIndex >= cameraEffectInfoList.Count) {
			currentCameraEffectIndex = 0;
		}

		currentCameraEffect = cameraEffectInfoList [currentCameraEffectIndex].mainCameraEffect;

		cameraEffectInfoList [currentCameraEffectIndex].eventToEnableEffect.Invoke ();

		if (!cameraEffectActive) {
			setCameraEffectActiveState (true);
		}
	}

	public void setPreviousCameraEffect ()
	{
		if (!canUseCameraEffect ()) {
			return;
		}

		cameraEffectInfoList [currentCameraEffectIndex].eventToDisableEffect.Invoke ();

		currentCameraEffectIndex--;

		if (currentCameraEffectIndex < 0) {
			currentCameraEffectIndex = cameraEffectInfoList.Count - 1;
		}

		currentCameraEffect = cameraEffectInfoList [currentCameraEffectIndex].mainCameraEffect;

		cameraEffectInfoList [currentCameraEffectIndex].eventToEnableEffect.Invoke ();

		if (!cameraEffectActive) {
			setCameraEffectActiveState (true);
		}
	}

	public void inputSetNextCameraEffect ()
	{
		setNextCameraEffect ();
	}

	public void inputSetPreviousCameraEffect ()
	{
		setPreviousCameraEffect ();
	}

	public void inputEnableOrDisableCameraEffectActive ()
	{
		enableOrDisableCameraEffectActive ();
	}

	[System.Serializable]
	public class cameraEffectInfo
	{
		public string Name;
		public bool activeAsMainCameraEffect;
		public cameraEffect mainCameraEffect;

		[Space]

		public UnityEvent eventToEnableEffect;
		public UnityEvent eventToDisableEffect;
	}
}
