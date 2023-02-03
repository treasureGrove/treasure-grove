using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleFPSCounter : MonoBehaviour
{
	public bool fpsCounterEnabled = true;

	public Text fpsText;

	float deltaTime = 0.0f;

	bool fpsCounterActive;

	void Start ()
	{
		enableOrDisableFPSCounter (fpsCounterEnabled);
	}

	void Update ()
	{
		if (!fpsCounterActive) {
			return;
		}

		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;

		fpsText.text = string.Format ("{0:0.0} ms ({1:0.})", msec, fps);
	}

	void OnEnable ()
	{
		if (Application.isPlaying) {
			enableOrDisableFPSCounter (true);
		}
	}

	void enableOrDisableFPSCounter (bool state)
	{
		if (fpsCounterActive == state) {
			return;
		}

		fpsCounterActive = state;

		if (fpsText != null) {
			if (fpsText.gameObject.activeSelf != fpsCounterActive) {
				fpsText.gameObject.SetActive (fpsCounterActive);
			}
		}
	}
}