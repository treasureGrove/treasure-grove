using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System;

[System.Serializable]
//System to use touch buttons to check if the button is being pressing, holding or released
public class touchButtonListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[Header ("Main Settings")]
	[Space]

	public bool changeColorOnPress;
	public Color releaseColor;
	public Color pressColor;
	public float colorChangeSpeed;

	public bool useCurrentColorForRelease;

	//	public inputManager input;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool pressedDown = false;
	public bool pressedUp = false;
	public bool pressed;

	public bool buttonColorVisible = true;

	public int currentFingerId;

	[Space]
	[Header ("Components")]
	[Space]

	public RawImage buttonIcon;

	Color currentColorForRelease;

	Coroutine colorTransition;

	Touch currentTouch;
	int currentTouchCount;
	bool touchPlatform;

	void Start ()
	{
		if (changeColorOnPress) {
			if (useCurrentColorForRelease && buttonIcon != null) {
				currentColorForRelease = buttonIcon.color;
			}
		}

		touchPlatform = touchJoystick.checkTouchPlatform ();
	}

	//if you press the button
	public void OnPointerDown (PointerEventData eventData)
	{
		pressedDown = true;
		pressed = true;

		if (changeColorOnPress) {
			changeColor (true);
		}

		StartCoroutine (disableDown ());

//		input.increaseCurrentNumberOfTouchButtonsPressed ();

//		print (eventData.pointerId);

		//checkTouchID ();
	}

	public void checkTouchID ()
	{
		currentTouchCount = Input.touchCount;
		if (!touchPlatform) {
			currentTouchCount++;
		}

		for (int i = 0; i < currentTouchCount; i++) {
			if (!touchPlatform) {
				currentTouch = touchJoystick.convertMouseIntoFinger ();
			} else {
				currentTouch = Input.GetTouch (i);
			}

			if (currentTouch.phase == TouchPhase.Began) {
				currentFingerId = currentTouch.fingerId;
			}
		}
	}

	public int getCurrentFingerId ()
	{
		return currentFingerId;
	}

	//if you release the button
	public void OnPointerUp (PointerEventData eventData)
	{
		pressedUp = true;
		pressed = false;

		if (changeColorOnPress) {
			changeColor (false);
		}

		StartCoroutine (disableUp ());

//		input.decreaseCurrentNumberOfTouchButtonsPressed ();
	}

	//disable the booleans parameters after press them
	IEnumerator disableDown ()
	{
		yield return new WaitForSeconds (0.001f);

		pressedDown = false;
	}

	IEnumerator disableUp ()
	{
		yield return new WaitForSeconds (0.001f);

		pressedUp = false;
	}

	//if the button is disabled, reset the button
	void OnDisable ()
	{
		pressedDown = false;
		pressedUp = false;
		pressed = false;
	}

	void changeColor (bool state)
	{
		if (!buttonColorVisible) {
			return;
		}

		if (colorTransition != null) {
			StopCoroutine (colorTransition);
		}

		colorTransition = StartCoroutine (changeColorCoroutine (state));
	}

	IEnumerator changeColorCoroutine (bool state)
	{
		if (buttonIcon != null) {
			Color targetColor = Color.white;

			if (state) {
				targetColor = pressColor;
			} else {
				if (useCurrentColorForRelease) {
					targetColor = currentColorForRelease;
				} else {
					targetColor = releaseColor;
				}
			}

			while (buttonIcon.color != targetColor) {
				buttonIcon.color = Color.Lerp (buttonIcon.color, targetColor, Time.deltaTime * colorChangeSpeed);
				yield return null;
			}
		}

		yield return null;
	}

	public void setButtonColorVisibleState (bool state)
	{
		if (buttonIcon != null) {
			buttonColorVisible = state;

			Color targetColor = buttonIcon.color;

			if (buttonColorVisible) {
				targetColor.a = releaseColor.a;
			} else {
				targetColor.a = 0;
			}
	
			buttonIcon.color = targetColor;
		}
	}

	public void setButtonIconComponent ()
	{
		buttonIcon = GetComponent<RawImage> ();

//		input = FindObjectOfType <inputManager> ();

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}