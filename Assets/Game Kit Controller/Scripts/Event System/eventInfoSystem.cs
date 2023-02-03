using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class eventInfoSystem : MonoBehaviour
{
	public bool eventInfoEnabled = true;

	public bool useDelayEnabled = true;

	public List<eventInfo> eventInfoList = new List<eventInfo> ();

	public remoteEventSystem mainRemoteEventSystem;

	public bool useAccumulativeDelay;

	public bool eventInProcess;

	Coroutine eventInfoListCoroutine;

	public void stopCheckActionEventInfoList ()
	{
		if (eventInfoListCoroutine != null) {
			StopCoroutine (eventInfoListCoroutine);
		}

		eventInProcess = false;
	}

	public void activateActionEventInfoList ()
	{
		if (!eventInfoEnabled) {
			return;
		}

		bool mainRemoteEventSystemLocated = mainRemoteEventSystem != null;

		if (useDelayEnabled) {

			stopCheckActionEventInfoList ();

			eventInfoListCoroutine = StartCoroutine (checkActionEventInfoListCoroutine ());
		} else {
			for (int i = 0; i < eventInfoList.Count; i++) {
				eventInfoList [i].eventToUse.Invoke ();

				if (mainRemoteEventSystemLocated) {
					if (eventInfoList [i].useRemoteEvent) {
						mainRemoteEventSystem.callRemoteEvent (eventInfoList [i].remoteEventName);
					}
				}
			}
		}
	}

	IEnumerator checkActionEventInfoListCoroutine ()
	{
		eventInProcess = true;

		for (int i = 0; i < eventInfoList.Count; i++) {
			eventInfoList [i].eventTriggered = false;
		}

		bool mainRemoteEventSystemLocated = mainRemoteEventSystem != null;

		if (useAccumulativeDelay) {

			for (int i = 0; i < eventInfoList.Count; i++) {

				yield return new WaitForSeconds (eventInfoList [i].delayToActivate);

				eventInfoList [i].eventToUse.Invoke ();

				if (mainRemoteEventSystemLocated) {
					if (eventInfoList [i].useRemoteEvent) {
						mainRemoteEventSystem.callRemoteEvent (eventInfoList [i].remoteEventName);
					}
				}
			}
		} else {
			int numberOfEvents = eventInfoList.Count;

			int numberOfEventsTriggered = 0;

			float timer = Time.time;

			bool allEventsTriggered = false;

			while (!allEventsTriggered) {
				for (int i = 0; i < eventInfoList.Count; i++) {

					if (!eventInfoList [i].eventTriggered) {
						if (Time.time > timer + eventInfoList [i].delayToActivate) {
							eventInfoList [i].eventToUse.Invoke ();

							if (mainRemoteEventSystemLocated) {
								if (eventInfoList [i].useRemoteEvent) {
									mainRemoteEventSystem.callRemoteEvent (eventInfoList [i].remoteEventName);
								}
							}

							eventInfoList [i].eventTriggered = true;

							numberOfEventsTriggered++;

							if (numberOfEvents == numberOfEventsTriggered) {
								allEventsTriggered = true;
							}
						}
					}
				}

				yield return null;
			}
		}

		eventInProcess = false;
	}

	public void addNewEvent ()
	{
		eventInfo newEventInfo = new eventInfo ();

		eventInfoList.Add (newEventInfo);

		updateComponent ();
	}

	public void setEventInfoEnabledState (bool state)
	{
		eventInfoEnabled = state;
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class eventInfo
	{
		public float delayToActivate;

		public UnityEvent eventToUse;

		public bool useRemoteEvent;

		public string remoteEventName;

		public bool eventTriggered;
	}
}
