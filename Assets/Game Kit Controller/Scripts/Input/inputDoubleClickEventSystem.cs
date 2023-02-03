using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class inputDoubleClickEventSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkDoubleClickEnabled = true;

	public float maxTimeBetweenClicks = 0.4f;

	public bool useMaxTimeDuringAllClicks;

	public float maxTimeForAllClicks;

	public int numberOfClicksToActivateInput;

	[Space]
	[Header ("Debug")]
	[Space]

	public int currentNumberOfClicks;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnActivateInput;


	float lastTimeClickUsed;

	bool pressedInTime;


	public void checkClickInput ()
	{
		if (!checkDoubleClickEnabled) {
			return;
		}

		if (useMaxTimeDuringAllClicks) {
			if (lastTimeClickUsed == 0) {
				lastTimeClickUsed = Time.time;
			}

			currentNumberOfClicks++;

			pressedInTime = Time.time - lastTimeClickUsed < maxTimeForAllClicks;

			if (currentNumberOfClicks >= numberOfClicksToActivateInput) {
				if (pressedInTime) {
					eventOnActivateInput.Invoke ();

					print ("pressed in time");
				} else {
					print ("amount pressed, but too much wait");
				}

				lastTimeClickUsed = 0;

				currentNumberOfClicks = 0;


			} else {
				if (Time.time - lastTimeClickUsed > maxTimeForAllClicks) {
					lastTimeClickUsed = 0;

					currentNumberOfClicks = 0;

					print ("too much wait");
				}
			}
		
		} else {
			pressedInTime = Time.time < lastTimeClickUsed + maxTimeBetweenClicks;

			if (!pressedInTime) {
				currentNumberOfClicks = 0;
			}

			if (pressedInTime || currentNumberOfClicks == 0) {
				currentNumberOfClicks++;

				if (currentNumberOfClicks >= numberOfClicksToActivateInput) {
					eventOnActivateInput.Invoke ();

					currentNumberOfClicks = 0;
				}
			} else {
				currentNumberOfClicks = 0;
			}

			lastTimeClickUsed = Time.time;
		}
	}
}