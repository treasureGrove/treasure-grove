using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class actionInputDelaySystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkInputEnabled = true;

	public float actionDelay;

	public bool useLowerDelay;

	public pressType pressTypeCheck;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool pressedDown;

	public bool previousPressedDown;

	public bool eventTriggered;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent actionEvent;

	public enum pressType
	{
		hold,
		up
	}

	float lastTimePressedDown;

	public void inputSetPressedDownState ()
	{
		if (!checkInputEnabled) {
			return;
		}

		pressedDown = true;

		if (previousPressedDown != pressedDown) {
			previousPressedDown = pressedDown;

			lastTimePressedDown = Time.time;
			eventTriggered = false;
		}

		if (!eventTriggered && pressTypeCheck == pressType.hold && Time.time > lastTimePressedDown + actionDelay) {
			actionEvent.Invoke ();
			eventTriggered = true;
		}
	}

	public void inpuSetPressedUpState ()
	{
		if (!checkInputEnabled) {
			return;
		}

		pressedDown = false;

		if (previousPressedDown != pressedDown) {
			previousPressedDown = pressedDown;
			eventTriggered = false;
		}

		if (!eventTriggered && pressTypeCheck == pressType.up) {
			if (useLowerDelay) {
				if (Time.time < lastTimePressedDown + actionDelay) {
					actionEvent.Invoke ();

					eventTriggered = true;
				}
			} else {
				if (Time.time > lastTimePressedDown + actionDelay) {
					actionEvent.Invoke ();

					eventTriggered = true;
				}
			}
		}
	}
}
