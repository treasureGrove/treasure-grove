using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class AIViewTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkTriggerEnabled = true;

	public bool onTriggerEnter;
	public bool onTriggerExit;

	[Space]
	[Header ("Regular Event Settings")]
	[Space]

	public bool useEvents;

	public UnityEvent onTriggerEnterEvent = new UnityEvent ();
	public UnityEvent onTriggerExitEvent = new UnityEvent ();

	[Space]
	[Header ("Events With Objects Settings")]
	[Space]

	public bool useOnTriggerEnterEventWithObject;
	public eventParameters.eventToCallWithGameObject onTriggerEnterEventWithObject;

	public bool useOnTriggerExitEventWithObject;
	public eventParameters.eventToCallWithGameObject onTriggerExitEventWithObject;

	[Space]
	[Header ("Components")]
	[Space]

	public findObjectivesSystem mainFindObjectivesSystem;

	void OnTriggerEnter (Collider col)
	{
		if (!checkTriggerEnabled) {
			return;
		}

		checkTrigger (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		if (!checkTriggerEnabled) {
			return;
		}

		checkTrigger (col, false);
	}

	public void checkTrigger (Collider col, bool isEnter)
	{
		if (isEnter) {
			if (onTriggerEnter) {
				
				if (useEvents) {
					callEvent (onTriggerEnterEvent);

					if (useOnTriggerEnterEventWithObject) {
						callEventWithObject (onTriggerEnterEventWithObject, col.gameObject);
					}
				} 

				mainFindObjectivesSystem.checkSuspect (col.gameObject);
			}
		} else {
			if (onTriggerExit) {
				
				if (useEvents) {
					callEvent (onTriggerExitEvent);

					if (useOnTriggerExitEventWithObject) {
						callEventWithObject (onTriggerExitEventWithObject, col.gameObject);
					}
				} 

				mainFindObjectivesSystem.cancelCheckSuspect (col.gameObject);	
			}
		}
	}

	public void callEvent (UnityEvent eventToCall)
	{
		eventToCall.Invoke ();
	}

	public void callEventWithObject (eventParameters.eventToCallWithGameObject eventToCall, GameObject objectToSend)
	{
		eventToCall.Invoke (objectToSend);
	}

	public void setCheckTriggerEnabledState (bool state)
	{
		checkTriggerEnabled = state;
	}
}