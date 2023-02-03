using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class grabObjectEventSystem : MonoBehaviour
{
	public bool useEventOnGrab = true;
	public UnityEvent eventOnGrab;

	public bool useEventOnDrop;
	public UnityEvent eventOnDrop;


	public void callEventOnGrab ()
	{
		if (useEventOnGrab) {
			eventOnGrab.Invoke ();
		}
	}

	public void callEventOnDrop ()
	{
		if (useEventOnDrop) {
			eventOnDrop.Invoke ();
		}
	}
}
