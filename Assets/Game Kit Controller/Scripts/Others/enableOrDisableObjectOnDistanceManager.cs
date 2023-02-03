using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class enableOrDisableObjectOnDistanceManager : MonoBehaviour
{
	[Header ("Behavior Main Settings")]
	[Space]

	public bool addObjectToMainSystem = true;
	public Transform mainTransform;

	[Space]
	[Header ("Distance Settings")]
	[Space]

	public bool useCustomDistance;

	public float maxDistanceObjectEnabledOnScreen;
	public float maxDistanceObjectEnableOutOfScreen;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool objectActive = true;

	public bool objectInfoSentToMainSystem;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnEnableObject;
	public UnityEvent eventOnDisableObject;

	enableOrDisableObjectsOnDistanceSystem mainEnableOrDisableObjectsOnDistanceSystem;

	void Start ()
	{
		if (addObjectToMainSystem) {
			addObjectToSystem ();
		}
	}

	void addObjectToSystem ()
	{
		if (objectInfoSentToMainSystem) {
			return;
		}

		if (mainEnableOrDisableObjectsOnDistanceSystem == null) {
			mainEnableOrDisableObjectsOnDistanceSystem = FindObjectOfType<enableOrDisableObjectsOnDistanceSystem> ();
		}

		if (mainEnableOrDisableObjectsOnDistanceSystem != null) {
			mainEnableOrDisableObjectsOnDistanceSystem.addObject (this);

			objectInfoSentToMainSystem = true;
		}
	}

	public void removeObjectFromSystem ()
	{
		if (mainEnableOrDisableObjectsOnDistanceSystem != null) {
			mainEnableOrDisableObjectsOnDistanceSystem.removeObject (mainTransform);
		}
	}

	public void setActiveState (bool state)
	{
		if (objectActive == state) {
			return;
		}

		objectActive = state;

		if (objectActive) {
			eventOnEnableObject.Invoke ();
		} else {
			eventOnDisableObject.Invoke ();
		}
	}
}
