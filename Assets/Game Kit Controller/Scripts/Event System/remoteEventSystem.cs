using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class remoteEventSystem : MonoBehaviour
{
	public bool remoteEventsEnabled = true;

	public bool activateRemoteEventsOnStart;

	public List<string> remoteEventsOnStartList = new List<string> ();

	public List<eventInfo> eventInfoList = new List<eventInfo> ();

	public bool showDebugPrint;

	eventInfo currentEventInfo;

	void Start ()
	{
		if (activateRemoteEventsOnStart) {

			StartCoroutine (checkRemoteEventsOnStartListCoroutine ());
		}
	}

	IEnumerator checkRemoteEventsOnStartListCoroutine ()
	{
		yield return new WaitForEndOfFrame ();


		checkRemoteEventsOnStartList ();
	}

	void checkRemoteEventsOnStartList ()
	{
		for (int i = 0; i < remoteEventsOnStartList.Count; i++) {
			callRemoteEvent (remoteEventsOnStartList [i]);
		}
	}

	public void callRemoveEvent (string eventName)
	{
		callRemoteEvent (eventName);
	}

	public void callRemoteEvent (string eventName)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		for (int i = 0; i < eventInfoList.Count; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (showDebugPrint) {
						print (eventName);
					}

					if (currentEventInfo.useRegularEvent) {
						currentEventInfo.eventToActive.Invoke ();
					}

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}

					return;
				}
			}
		}
	}

	public void callRemoteEventWithAmount (string eventName, float amount)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		for (int i = 0; i < eventInfoList.Count; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (currentEventInfo.useAmountOnEvent) {
						currentEventInfo.eventToActiveAmount.Invoke (amount);
					}

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}

					if (showDebugPrint) {
						print (eventName);
					}

					return;
				}
			}
		}
	}

	public void callRemoteEventWithBool (string eventName, bool state)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		for (int i = 0; i < eventInfoList.Count; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (currentEventInfo.useBoolOnEvent) {
						currentEventInfo.eventToActiveBool.Invoke (state);
					}

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}

					if (showDebugPrint) {
						print (eventName);
					}

					return;
				}
			}
		}
	}

	public void callRemoteEventWithGameObject (string eventName, GameObject objectToSend)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		for (int i = 0; i < eventInfoList.Count; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (currentEventInfo.useGameObjectOnEvent) {
						currentEventInfo.eventToActiveGameObject.Invoke (objectToSend);
					}

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}

					if (showDebugPrint) {
						print (eventName);
					}

					return;
				}
			}
		}
	}

	public void callRemoteEventWithTransform (string eventName, Transform transformToSend)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		for (int i = 0; i < eventInfoList.Count; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (currentEventInfo.useTransformOnEvent) {
						currentEventInfo.eventToActiveTransform.Invoke (transformToSend);
					}

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}

					if (showDebugPrint) {
						print (eventName);
					}

					return;
				}
			}
		}
	}

	public void setEnabledEventState (string eventName)
	{
		setEnabledOrDisabledEventState (true, eventName);
	}

	public void setDisabledEventState (string eventName)
	{
		setEnabledOrDisabledEventState (false, eventName);
	}

	public void setEnabledOrDisabledEventState (bool state, string eventName)
	{
		for (int i = 0; i < eventInfoList.Count; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.Name.Equals (eventName)) {
				currentEventInfo.eventEnabled = state;

				return;
			}
		}
	}

	[System.Serializable]
	public class eventInfo
	{
		public string Name;

		public bool eventEnabled = true;

		public bool disableEventAfterActivation;

		public bool useRegularEvent = true;
		public UnityEvent eventToActive;

		public bool useAmountOnEvent;
		public eventParameters.eventToCallWithAmount eventToActiveAmount;

		public bool useBoolOnEvent;
		public eventParameters.eventToCallWithBool eventToActiveBool;

		public bool useGameObjectOnEvent;
		public eventParameters.eventToCallWithGameObject eventToActiveGameObject;

		public bool useTransformOnEvent;
		public eventParameters.eventToCallWithTransform eventToActiveTransform;
	}
}
