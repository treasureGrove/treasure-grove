using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class laserDotOnScreenSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool laserDotEnabled = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool laserDotActive;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject laserDotIcon;

	public RectTransform laserDotIconRectTransform;

	public Camera mainCamera;

	public playerCamera mainPlayerCamera;

	public bool targetOnScreen;

	Vector3 currentIconPosition;

	Vector3 screenPoint;

	float screenWidth;
	float screenHeight;

	Vector2 iconPosition2d;

	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	bool usingScreenSpaceCamera;

	public void updateLaserDotPosition (Vector3 hitPoint)
	{
		currentIconPosition = hitPoint;

		screenWidth = Screen.width;
		screenHeight = Screen.height;

		if (usingScreenSpaceCamera) {
			screenPoint = mainCamera.WorldToViewportPoint (currentIconPosition);
			targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
		} else {
			screenPoint = mainCamera.WorldToScreenPoint (currentIconPosition);
			targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
		}

		if (targetOnScreen) {
			if (usingScreenSpaceCamera) {
				iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, 
					(screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

				laserDotIconRectTransform.anchoredPosition = iconPosition2d;
			} else {
				laserDotIcon.transform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
			}

			if (laserDotActive) {
				if (!laserDotIcon.activeSelf) {
					laserDotIcon.SetActive (true);
				}
			}
		} else {
			if (laserDotActive) {
				if (laserDotIcon.activeSelf) {
					laserDotIcon.SetActive (false);
				}
			}
		}
	}

	public void setLasetDotIconActiveState (bool state)
	{
		if (!laserDotEnabled) {
			return;
		}

		laserDotActive = state;

		if (laserDotActive) {
			mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
			halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
			usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();
		
			if (mainCamera == null) {
				mainCamera = mainPlayerCamera.getMainCamera ();
			}
		}

		if (laserDotIcon.activeSelf != laserDotActive) {
			laserDotIcon.SetActive (laserDotActive);
		}
	}
}
