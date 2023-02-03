using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class examineObjectSystemPlayerManagement : MonoBehaviour
{
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool examiningObject;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStateChange;
	public UnityEvent evenOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	examineObjectSystem currentExanimeObject;

	float lastTimeExaminingObject = 0;

	public void setExaminingObjectState (bool state)
	{
		examiningObject = state;

		if (showDebugPrint) {
			print ("Setting examine object state " + state);
		}

		if (examiningObject) {
			lastTimeExaminingObject = Time.time;
		} else {
			lastTimeExaminingObject = 0;
		}

		checkEventsOnStateChange (examiningObject);
	}

	public void setcurrentExanimeObject (examineObjectSystem newExamineObject)
	{
		currentExanimeObject = newExamineObject;
	}

	//CALL INPUT FUNCTIONS TO EXAMINE OBJECTS
	public void examineObjectInputSetZoomValue (bool value)
	{
		if (!examiningObject) {
			return;
		}

		if (currentExanimeObject != null) {
			currentExanimeObject.inputSetZoomValue (value);
		}
	}

	public void examineObjectInputResetRotation ()
	{
		if (!examiningObject) {
			return;
		}

		if (currentExanimeObject != null) {
			currentExanimeObject.inputResetRotation ();
		}
	}

	public void examineObjectInputResetRotationAndPosition ()
	{
		if (!examiningObject) {
			return;
		}

		if (currentExanimeObject != null) {
			currentExanimeObject.inputResetRotationAndPosition ();
		}
	}

	public void examineObjectInputCancelExamine ()
	{
		if (!examiningObject) {
			return;
		}

		if (lastTimeExaminingObject > 0) {
			if (Time.time < lastTimeExaminingObject + 0.4f) {
				return;
			}
		}

		if (currentExanimeObject != null) {
			currentExanimeObject.inputCancelExamine ();
		}
	}

	public void examineObjectInputCheckIfMessage ()
	{
		if (!examiningObject) {
			return;
		}

		if (currentExanimeObject != null) {
			currentExanimeObject.inputCheckIfMessage ();
		}
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				evenOnStateEnabled.Invoke ();
			} else {
				eventOnStateDisabled.Invoke ();
			}
		}
	}
}
