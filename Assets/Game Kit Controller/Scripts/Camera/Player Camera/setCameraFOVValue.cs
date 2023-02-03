using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setCameraFOVValue : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool cameraFOVChangeEnabled = true;

	public Camera mainCamera;

	public float changeFOVAmount = 0.4f;

	public Vector2 FOVClampValue;

	public void enableOrDisableCameraFOVChange (bool state)
	{
		cameraFOVChangeEnabled = state;
	}

	public void increaseFov ()
	{
		if (!cameraFOVChangeEnabled) {
			return;
		}

		changeFOV (1);
	}

	public void decreaseFOV ()
	{
		if (!cameraFOVChangeEnabled) {
			return;
		}

		changeFOV (-1);
	}

	void changeFOV (float changeDirection)
	{
		mainCamera.fieldOfView += (changeFOVAmount * changeDirection);

		clampFOVValue ();
	}

	public void setCameraFOV (float newValue)
	{
		mainCamera.fieldOfView = newValue;

		clampFOVValue ();
	}

	void clampFOVValue ()
	{
		mainCamera.fieldOfView = Mathf.Clamp (mainCamera.fieldOfView, FOVClampValue.x, FOVClampValue.y);
	}
}
