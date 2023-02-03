using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class eventObjectFoundOnCaptureSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool eventEnabled = true;

	public bool callEventMultipleTimes;

	[Header ("Debug")]
	[Space]

	public bool eventTriggered;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventToCallOnCapture;

	public void callEventOnCapture ()
	{
		if (eventEnabled && !eventTriggered) {
			eventToCallOnCapture.Invoke ();

			if (!callEventMultipleTimes) {
				eventTriggered = true;
			}
		}
	}
}
