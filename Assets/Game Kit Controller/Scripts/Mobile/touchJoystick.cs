using UnityEngine;
using System.Reflection;
using UnityEngine.UI;

public class touchJoystick : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float dragDistance = 1;
	public bool snapsToFinger = true;
	public bool hideOnRelease = false;
	public bool touchPad;

	public float touchPadHorizontalExtraValue = 1;
	public float touchPadVerticalExtraValue = 1;
	public bool showJoystick;

	[Space]
	[Header ("Colors Settings")]
	[Space]

	public bool useChaneColor;
	public float changeColorSpeed;
	public Color regularBaseColor;
	public Color regularStickColor;
	public Color pressedBaseColor;
	public Color pressedStickColor;

	[Space]
	[Header ("Debug")]
	[Space]

	public Vector2 currentAxisValue;
	public Vector2 currentTouchPosition;
	public bool touching;
	public bool hoveringJoystick;
	public bool touchingPreviously;

	public bool adjustinColors;

	public bool buttonColorVisible = true;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject stickBase;
	public GameObject stick;
	public RectTransform baseRectTransform;
	public RectTransform stickRectTransform;
	public RawImage baseRenderer;
	public RawImage stickRenderer;

	bool touchPlatform;

	Color currentBaseColor;
	Color currentStickColor;

	Vector3 previousPosition;
	Vector2 originalStickPosition;
	Vector2 originalBasePosition;
	Vector3 globalTouchPosition;
	Vector3 differenceVector;
	Vector3 difference;
	int touchCount;

	Touch currentPressedTouch;
	Touch currentTouch;

	int currentFingerId;

	int currentTouchIndex;

	bool firstTouchActive;

	void Start ()
	{
		touchPlatform = checkTouchPlatform ();

		if (!showJoystick) {
			setSticksState (false, false);
		}

		originalStickPosition = stickRectTransform.anchoredPosition;
		originalBasePosition = baseRectTransform.anchoredPosition;
	}

	void Update ()
	{
		setJoystickColors ();

		if (currentFingerId > -1) {
			moveJoystick ();
		}
	}

	public void setHoverState (bool state)
	{
		hoveringJoystick = state;

		if (hoveringJoystick && !touching) {
			touchCount = Input.touchCount;

			if (!touchPlatform) {
				touchCount++;
			}

			for (currentTouchIndex = 0; currentTouchIndex < touchCount; currentTouchIndex++) {
				if (!touchPlatform) {
					currentTouch = convertMouseIntoFinger ();
				} else {
					currentTouch = Input.GetTouch (currentTouchIndex);
				}
					
				//if the touch action has started, check if the finger is inside the touch zone rect, visible in the editor
				if (currentTouch.phase == TouchPhase.Began) {
					currentTouchPosition = currentTouch.position;

					currentFingerId = currentTouch.fingerId;

					fingerTouching (true);

					if (snapsToFinger) {
						stickRectTransform.position = currentTouchPosition;
						baseRectTransform.position = stickRectTransform.position;
					}

					if (touchPad) {
						previousPosition = currentTouchPosition;
					}
				}
			}
		}
	}

	// set the value of touching to activate of deactivate the icons of the joystick
	void fingerTouching (bool state)
	{
		touching = state;

		if (showJoystick) {
			if (hideOnRelease) {
				setSticksState (state, state);
			} else if ((!stickBase.gameObject.activeSelf || !stick.gameObject.activeSelf)) {
				setSticksState (true, true);
			}
		} else {
			setSticksState (false, false);
		}
	}

	void setSticksState (bool stickBaseState, bool stickState)
	{
		stickBase.SetActive (stickBaseState);
		stick.SetActive (stickState);
	}

	//if the joystick is released, the icons back to their default positions
	void resetJoystickPosition ()
	{
		stickRectTransform.anchoredPosition = originalStickPosition;
		baseRectTransform.anchoredPosition = originalBasePosition;

		currentAxisValue = Vector2.zero;

		fingerTouching (false);
	}

	//check if the joystick is being used and set the icons position according to the finger or mouse movement
	void moveJoystick ()
	{
		if (touching) {
			touchCount = Input.touchCount;

			if (!touchPlatform) {
				touchCount++;
			}

			currentPressedTouch.fingerId = 666;

			for (currentTouchIndex = 0; currentTouchIndex < touchCount; currentTouchIndex++) {
				if (!touchPlatform) {
					currentTouch = convertMouseIntoFinger ();
				} else {
					currentTouch = Input.GetTouch (currentTouchIndex);
				}

				if (currentTouch.fingerId == currentFingerId) {
					currentPressedTouch = currentTouch;
				}
			}

			if (currentPressedTouch.fingerId == 666 || currentPressedTouch.phase == TouchPhase.Ended) {
				resetJoystickPosition ();

				currentFingerId = -1;

				return;
			}

			globalTouchPosition = currentPressedTouch.position;
			differenceVector = globalTouchPosition - stickBase.transform.position;

			if (differenceVector.sqrMagnitude > dragDistance * dragDistance) {
				differenceVector.Normalize ();
				differenceVector *= dragDistance;
				stickRectTransform.position = baseRectTransform.position + differenceVector;
			} else {
				stickRectTransform.position = globalTouchPosition;
			}

			if (!touchPad) {
				currentAxisValue = differenceVector / dragDistance;
			} else {
				difference = globalTouchPosition - previousPosition;
				if (differenceVector.sqrMagnitude > dragDistance * dragDistance) {
					difference.Normalize ();
				}
				currentAxisValue = (difference / dragDistance);

				currentAxisValue.x *= touchPadHorizontalExtraValue;
				currentAxisValue.y *= touchPadVerticalExtraValue;

				previousPosition = globalTouchPosition;
			}
		}
	}

	//get the vertical and horizontal axis values
	public Vector2 GetAxis ()
	{
		return currentAxisValue;
	}

	public Vector2 getRawAxis ()
	{
		Vector2 axisValues = Vector2.zero;

		if (currentAxisValue.x > 0) {
			axisValues.x = 1;
		} else if (currentAxisValue.x < 0) {
			axisValues.x = -1;
		} else {
			axisValues.x = 0;
		}

		if (currentAxisValue.y > 0) {
			axisValues.y = 1;
		} else if (currentAxisValue.y < 0) {
			axisValues.y = -1;
		} else {
			axisValues.y = 0;
		}

		return axisValues;
	}

	void setJoystickColors ()
	{
		if (useChaneColor) {
			if (buttonColorVisible) {
				if (touching != touchingPreviously || !firstTouchActive) {
					touchingPreviously = touching;

					adjustinColors = true;

					firstTouchActive = true;
				}

				if (adjustinColors) {
					if (touching) {
						currentBaseColor = pressedBaseColor;
						currentStickColor = pressedStickColor;
					} else {
						currentBaseColor = regularBaseColor;
						currentStickColor = regularStickColor;
					}

					baseRenderer.color = Color.Lerp (baseRenderer.color, currentBaseColor, Time.deltaTime * changeColorSpeed);

					stickRenderer.color = Color.Lerp (stickRenderer.color, currentStickColor, Time.deltaTime * changeColorSpeed);

					if (baseRenderer.color == currentBaseColor && stickRenderer.color == currentStickColor) {
						adjustinColors = false;
					}
				}
			}
		}
	}

	//it simulates touch control in the game with the mouse position, using left button as tap finger with press, hold and release actions
	public static Touch convertMouseIntoFinger ()
	{
		object mouseFinger = new Touch ();

		FieldInfo phase = mouseFinger.GetType ().GetField ("m_Phase", BindingFlags.NonPublic | BindingFlags.Instance);
		FieldInfo fingerId = mouseFinger.GetType ().GetField ("m_FingerId", BindingFlags.NonPublic | BindingFlags.Instance);
		FieldInfo position = mouseFinger.GetType ().GetField ("m_Position", BindingFlags.NonPublic | BindingFlags.Instance);

		if (Input.GetMouseButtonDown (0)) {
			phase.SetValue (mouseFinger, TouchPhase.Began);
		} else if (Input.GetMouseButtonUp (0)) {
			phase.SetValue (mouseFinger, TouchPhase.Ended);
		} else {
			phase.SetValue (mouseFinger, TouchPhase.Moved);
		}

		fingerId.SetValue (mouseFinger, 11);
		position.SetValue (mouseFinger, new Vector2 (Input.mousePosition.x, Input.mousePosition.y));

		return (Touch)mouseFinger;
	}

	//check the if the current platform is a touch device
	public static bool checkTouchPlatform ()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			return true;
		}

		return false;
	}

	public void setTouchPadState (bool state)
	{
		touchPad = state;
	}

	public void setShowJoystickState (bool state)
	{
		showJoystick = state;

		setSticksState (showJoystick, showJoystick);
	}

	public void setSnapsToFingerState (bool state)
	{
		snapsToFinger = state;
	}

	public void setHideOnReleaseState (bool state)
	{
		hideOnRelease = state;
	}

	public void setJoystickColorVisibleState (bool state)
	{
		buttonColorVisible = state;

		Color targetColorBase = baseRenderer.color;
		Color targetColorStick = stickRenderer.color;

		if (buttonColorVisible) {
			targetColorBase.a = regularBaseColor.a;
			targetColorStick.a = regularStickColor.a;
		} else {
			targetColorBase.a = 0;
			targetColorStick.a = 0;
		}

		baseRenderer.color = targetColorBase;
		stickRenderer.color = targetColorStick;
	}

	public bool touchJoystickIsPressed ()
	{
		return touching;
	}
}