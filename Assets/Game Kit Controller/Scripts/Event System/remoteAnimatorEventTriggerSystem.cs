using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remoteAnimatorEventTriggerSystem : MonoBehaviour
{
	public bool remoteEventsEnabled = true;

	public bool showDebugPrint;

	public List<remoteEventSystem.eventInfo> eventInfoList = new List<remoteEventSystem.eventInfo> ();

	remoteEventSystem.eventInfo currentEventInfo;


	public void callRemoteEvent (string eventName)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		for (int i = 0; i < eventInfoList.Count; i++) {
			currentEventInfo = eventInfoList [i];

			if (currentEventInfo.eventEnabled) {
				if (currentEventInfo.Name.Equals (eventName)) {
					if (currentEventInfo.useRegularEvent) {
						currentEventInfo.eventToActive.Invoke ();
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
}
