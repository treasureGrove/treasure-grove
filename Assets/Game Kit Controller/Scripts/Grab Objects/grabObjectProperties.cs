using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class grabObjectProperties : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool objectUsesWeight = true;

	public float objectWeight;

	public bool useExtraGrabDistance;

	public float extraGrabDistance;

	[Space]
	[Header ("Mass Settings")]
	[Space]

	public bool useCustomMassToThrow;
	public float customMassToThrow;
	public float customObjectMassDividerOnThrow = 1;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnGrabObject;
	public UnityEvent eventOnGrabObject;

	public bool useEventsOnDropObject;
	public UnityEvent eventOnDropObject;

	public bool useEventToSetPlayer;
	public eventParameters.eventToCallWithGameObject eventToSetPlayer;

	[Space]
	[Header ("Components")]
	[Space]

	public Collider mainTrigger;

	public float getObjectWeight ()
	{
		if (objectUsesWeight) {
			return objectWeight;
		}

		return 0;
	}

	public float getExtraGrabDistance ()
	{
		if (useExtraGrabDistance) {
			return extraGrabDistance;
		}

		return 0;
	}

	public void checkEventsOnGrabObject ()
	{
		if (useEventsOnGrabObject) {
			eventOnGrabObject.Invoke ();
		}
	}

	public void checkEventsOnDropObject ()
	{
		if (useEventsOnDropObject) {
			eventOnDropObject.Invoke ();
		}
	}

	public void checkEventToSetPlayer (GameObject newPlayer)
	{
		if (useEventToSetPlayer) {
			eventToSetPlayer.Invoke (newPlayer);
		}
	}

	public Collider getMainTrigger ()
	{
		return mainTrigger;
	}
}
