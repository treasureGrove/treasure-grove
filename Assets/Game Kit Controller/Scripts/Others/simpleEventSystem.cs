using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class simpleEventSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useEventsOnActivateAndDisabled = true;

	public bool useDelayToEvent;
	public float delayToEvent;

	public bool activated;
	public bool callOnStart;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventToCallOnActivate = new UnityEvent ();
	public UnityEvent eventToCallOnDisable = new UnityEvent ();

	[Space]
	[Space]

	public UnityEvent regularEventToCall;

	Coroutine eventCoroutine;

	void Start ()
	{
		if (callOnStart) {
			activateDevice ();
		}
	}

	public void activateDevice ()
	{
		if (useDelayToEvent) {
			if (eventCoroutine != null) {
				StopCoroutine (eventCoroutine);
			}

			eventCoroutine = StartCoroutine (activateDeviceCoroutine ());
		} else {
			callEvent ();
		}
	}

	IEnumerator activateDeviceCoroutine ()
	{
		yield return new WaitForSeconds (delayToEvent);

		callEvent ();
	}

	public void callEvent ()
	{
		if (useEventsOnActivateAndDisabled) {
			activated = !activated;

			if (activated) {
				eventToCallOnActivate.Invoke ();
			} else {
				eventToCallOnDisable.Invoke ();
			}
		} else {
			regularEventToCall.Invoke ();
		}
	}
}
