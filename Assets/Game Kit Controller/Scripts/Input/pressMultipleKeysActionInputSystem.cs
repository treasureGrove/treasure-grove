using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class pressMultipleKeysActionInputSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkPressKeysEnabled = true;

	public int numberOfKeysToPress;

	public bool resetNumberOfKeysPressedAtEnd;

	public bool pressKeysInOrder;

	[Space]
	[Header ("Reset Keys Settings")]
	[Space]

	public bool resetNumberOfKeysPressedIfNotCorrectOrder;

	public bool resetNumberOfKeysAfterDelay;
	public float maxDelayToPressKeys;
	public bool cancelActionIfMaxDelayReached;

	[Space]
	[Header ("Debug")]
	[Space]

	public int currentNumberOfKeysPressed;

	public bool showDebugPrint;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventToUseOnKeysPressed;

	float lastTimeKeyPressed;

	public void setPressedKeyState (bool state)
	{
		if (!checkPressKeysEnabled) {
			return;
		}
			
		if (checkIfDelayComplete ()) {

			if (cancelActionIfMaxDelayReached) {
				if (showDebugPrint) {
					print ("cancelling action");
				}

				return;
			}
		}

		if (state) {
			currentNumberOfKeysPressed++;

			if (showDebugPrint) {
				print ("increasing counter");
			}
		} else {
			currentNumberOfKeysPressed--;

			if (showDebugPrint) {
				print ("decreasing counter");
			}
		}

		if (currentNumberOfKeysPressed >= numberOfKeysToPress) {
			eventToUseOnKeysPressed.Invoke ();

			if (resetNumberOfKeysPressedAtEnd) {
				currentNumberOfKeysPressed = 0;
			}

			if (showDebugPrint) {
				print ("Action activated, reseting");
			}
		}

		lastTimeKeyPressed = Time.time;
	}

	public void setPressedKeyStateInOrder (int pressedKeyID)
	{
		if (!checkPressKeysEnabled) {
			return;
		}

		if (checkIfDelayComplete ()) {
			if (cancelActionIfMaxDelayReached) {

				if (showDebugPrint) {
					print ("cancelling action");
				}

				return;
			}
		}
			
		if (currentNumberOfKeysPressed == pressedKeyID) {
			currentNumberOfKeysPressed++;

			if (showDebugPrint) {
				print ("increasing counter");
			}
		} else {
			if (resetNumberOfKeysPressedIfNotCorrectOrder) {
				currentNumberOfKeysPressed = 0;
			}
		}

		if (currentNumberOfKeysPressed >= numberOfKeysToPress) {
			eventToUseOnKeysPressed.Invoke ();

			if (resetNumberOfKeysPressedAtEnd) {
				currentNumberOfKeysPressed = 0;
			}

			if (showDebugPrint) {
				print ("Action activated, reseting");
			}
		}

		lastTimeKeyPressed = Time.time;
	}

	public void setCheckPressKeysEnabledState (bool state)
	{
		checkPressKeysEnabled = state;
	}

	bool checkIfDelayComplete ()
	{
		if (resetNumberOfKeysAfterDelay) {
			if (lastTimeKeyPressed > 0 && Time.time > lastTimeKeyPressed + maxDelayToPressKeys) {
				currentNumberOfKeysPressed = 0;

				lastTimeKeyPressed = 0;

				if (showDebugPrint) {
					print ("Pressed out of time, cancelling action");
				}

				return true;
			}
		}

		return false;
	}
}