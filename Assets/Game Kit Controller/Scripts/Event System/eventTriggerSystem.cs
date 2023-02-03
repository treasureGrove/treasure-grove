using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class eventTriggerSystem : MonoBehaviour
{
	public List<eventInfo> eventList = new List<eventInfo> ();
	public List<eventInfo> enterEventList = new List<eventInfo> ();
	public List<eventInfo> exitEventList = new List<eventInfo> ();
	public bool useSameFunctionInList;
	public List<string> sameFunctionList = new List<string> ();
	public bool useSameObjectToCall;
	public bool callThisObject;
	public GameObject sameObjectToCall;
	public bool triggeredByButton;
	public bool useObjectToTrigger;
	public GameObject objectNeededToTrigger;

	public bool useTagToTrigger;
	public string tagNeededToTrigger;
	public bool useTagList;
	public List<string> tagList = new List<string> ();


	public bool justCallOnTrigger;
	public bool callFunctionEveryTimeTriggered;
	public bool eventTriggered;

	public bool dontUseDelay;
	public bool useSameDelay;
	public float generalDelay;

	public bool useRandomDelay;
	public Vector2 randomDelayRange;

	public triggerType triggerEventType;
	public bool useLayerMask;
	public LayerMask layerMask;
	public bool coroutineActive;
	public bool setParentToNull;
	public GameObject objectDetected;

	public bool triggerEventAtStart;

	public enum triggerType
	{
		enter,
		exit,
		both
	}

	public bool isEnter;
	public bool isExit;

	Coroutine eventCoroutine;

	GameObject currentObjectToCall;

	void Start ()
	{
		if (triggerEventAtStart) {
			activateEvent ();
		}
	}

	public void activateEventIfObjectActiveSelf ()
	{
		if (gameObject.activeInHierarchy) {
			activateEvent ();
		}
	}

	public void activateEvent ()
	{
		if (!eventTriggered || callFunctionEveryTimeTriggered) {
			eventTriggered = true;

			if (setParentToNull) {
				transform.SetParent (null);
			}

			if (dontUseDelay) {
				activateEventAtOnce ();
			} else {
				stopEventCoroutine ();

				eventCoroutine = StartCoroutine (activateEventInTime ());
			}
		}
	}

	public void stopEventCoroutine ()
	{
		if (eventCoroutine != null) {
			StopCoroutine (eventCoroutine);
		}
	}

	public void stopAllEventsAndDisableTriggerSystem ()
	{
		stopEventCoroutine ();

		eventTriggered = true;

		callFunctionEveryTimeTriggered = false;
	}

	IEnumerator activateEventInTime ()
	{
		coroutineActive = true;

		List<eventInfo> currentEventList = eventList;

		if (triggerEventType == triggerType.both) {
			if (isEnter) {
				currentEventList = enterEventList;
			}

			if (isExit) {
				currentEventList = exitEventList;
			}
		} else if (triggerEventType == triggerType.exit) {
			if (isExit) {
				currentEventList = exitEventList;
			}
		}

		for (int i = 0; i < currentEventList.Count; i++) {
			if (useSameDelay) {
				float currentDelay = generalDelay;

				if (useRandomDelay) {
					currentDelay = Random.Range (randomDelayRange.x, randomDelayRange.y);
				}

				yield return new WaitForSeconds (currentDelay);
			} else {
				float currentDelay = currentEventList [i].secondsDelay;

				if (currentEventList [i].useRandomDelay) {
					currentDelay = Random.Range (currentEventList [i].randomDelayRange.x, currentEventList [i].randomDelayRange.y);
				}

				yield return new WaitForSeconds (currentDelay);
			}

			callElementFromList (currentEventList [i]);
		}

		coroutineActive = false;
	}

	public void activateEventAtOnce ()
	{
		List<eventInfo> currentEventList = eventList;

		if (triggerEventType == triggerType.both) {
			if (isEnter) {
				currentEventList = enterEventList;
			}

			if (isExit) {
				currentEventList = exitEventList;
			}
		} else if (triggerEventType == triggerType.exit) {
			if (isExit) {
				currentEventList = exitEventList;
			}
		}

		for (int i = 0; i < currentEventList.Count; i++) {
			callElementFromList (currentEventList [i]);
		}
	}

	public void callElementFromList (eventInfo currentEventInfo)
	{
		if (useSameFunctionInList) {
			for (int j = 0; j < sameFunctionList.Count; j++) {
				currentObjectToCall = getObjectToCall (currentEventInfo.objectToCall);

				if (currentObjectToCall != null) {
					if (currentEventInfo.sendGameObject) {
						currentObjectToCall.SendMessage (sameFunctionList [j], currentEventInfo.objectToSend, SendMessageOptions.DontRequireReceiver);
					} else {
						currentObjectToCall.SendMessage (sameFunctionList [j], SendMessageOptions.DontRequireReceiver);
					}
				} else {
					print ("WARNING: no object to call has been configure on the event trigger system " + gameObject.name + ", check it is correctly assigned");
				}
			}
		} else {
			if (currentEventInfo.useEventFunction) {
				if (currentEventInfo.sendObjectDetected) {
					if (objectDetected != null) {

						if (currentEventInfo.sendObjectDetectedByEvent) {
							currentEventInfo.eventToSendObjectDetected.Invoke (objectDetected);
						} else {
							currentObjectToCall = getObjectToCall (currentEventInfo.objectToCall);

//							print (objectDetected.name + " " + currentObjectToCall.name);
							if (currentObjectToCall != null) {
								currentObjectToCall.SendMessage (currentEventInfo.sendObjectDetectedFunction, objectDetected, SendMessageOptions.DontRequireReceiver);
							} else {
								print ("WARNING: no object to call has been configure on the event trigger system " + gameObject.name + ", check it is correctly assigned");
							}
						}
					}
				}

				if (currentEventInfo.useRemoteEvent) {
					checkRemoteEventsOnObjectDetected (currentEventInfo, objectDetected);
				}

				currentEventInfo.eventFunction.Invoke ();

			} else {
				if (currentEventInfo.sendObjectDetected) {
					if (objectDetected != null) {
						
						if (currentEventInfo.sendObjectDetectedByEvent) {
							currentEventInfo.eventToSendObjectDetected.Invoke (objectDetected);
						} else {
							currentObjectToCall = getObjectToCall (currentEventInfo.objectToCall);

							if (currentObjectToCall != null) {
							
								currentObjectToCall.SendMessage (currentEventInfo.sendObjectDetectedFunction, objectDetected, SendMessageOptions.DontRequireReceiver);
							
							} else {
								print ("WARNING: no object to call has been configure on the event trigger system " + gameObject.name + ", check it is correctly assigned");
							}
						}
					}
				}

				if (currentEventInfo.useRemoteEvent) {
					checkRemoteEventsOnObjectDetected (currentEventInfo, objectDetected);
				}

				if (currentEventInfo.useBroadcastMessage) {
					for (int j = 0; j < currentEventInfo.broadcastMessageStringList.Count; j++) {
//						print (currentEventInfo.broadcastMessageStringList [j]);

						currentObjectToCall = getObjectToCall (currentEventInfo.objectToCall);

						if (currentObjectToCall != null) {
							currentObjectToCall.BroadcastMessage (currentEventInfo.broadcastMessageStringList [j], SendMessageOptions.DontRequireReceiver);
						} else {
							print ("WARNING: no object to call has been configure on the event trigger system " + gameObject.name + ", check it is correctly assigned");
						}
					}
				} else {
					for (int j = 0; j < currentEventInfo.functionNameList.Count; j++) {
						currentObjectToCall = getObjectToCall (currentEventInfo.objectToCall);

						if (currentObjectToCall != null) {
							if (currentEventInfo.sendGameObject) {
								currentObjectToCall.SendMessage (currentEventInfo.functionNameList [j], currentEventInfo.objectToSend, SendMessageOptions.DontRequireReceiver);
							} else {
								currentObjectToCall.SendMessage (currentEventInfo.functionNameList [j], SendMessageOptions.DontRequireReceiver);
							}
						} else {
							print ("WARNING: no object to call has been configure on the event trigger system " + gameObject.name + ", check it is correctly assigned");
						}
					}
				}
			}
		}
	}

	void checkRemoteEventsOnObjectDetected (eventInfo currentEventInfo, GameObject newObjectDetected)
	{
		if (newObjectDetected != null) {
			remoteEventSystem currentRemoteEventSystem = newObjectDetected.GetComponent<remoteEventSystem> ();

			if (currentRemoteEventSystem == null) {
				playerComponentsManager currentPlayerComponetsManager = newObjectDetected.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponetsManager != null) {
					currentRemoteEventSystem = currentPlayerComponetsManager.getRemoteEventSystem ();
				}
			}

			if (currentRemoteEventSystem != null) {
				for (int i = 0; i < currentEventInfo.remoteEventNameList.Count; i++) {
					currentRemoteEventSystem.callRemoteEvent (currentEventInfo.remoteEventNameList [i]);
				}
			}
		}
	}

	public GameObject getObjectToCall (GameObject defaultObjectToCall)
	{
		GameObject objectToCall = defaultObjectToCall;

		if (useSameObjectToCall) {
			if (callThisObject) {
				objectToCall = gameObject;
			} else {
				objectToCall = sameObjectToCall;
			}
		}

		return objectToCall;
	}

	public void checkTriggerEventEnterType (GameObject objectToCheck)
	{
		checkTriggerEventType (objectToCheck, triggerType.enter);
	}

	public void checkTriggerEventExitType (GameObject objectToCheck)
	{
		checkTriggerEventType (objectToCheck, triggerType.exit);
	}

	public void checkTriggerEventType (GameObject objectToCheck, triggerType trigger)
	{
		if ((!eventTriggered || callFunctionEveryTimeTriggered) && !triggeredByButton) {
			if (trigger == triggerEventType || triggerEventType == triggerType.both) {

				if (triggerEventType == triggerType.both) {
					if (trigger == triggerType.enter) {
						isEnter = true;
						isExit = false;
					} else {
						isEnter = false;
						isExit = true;
					}
				} else if (triggerEventType == triggerType.exit) {
					if (isExit) {
						isEnter = false;
						isExit = true;
					}
				}

				if (useObjectToTrigger) {
					if (objectToCheck == objectNeededToTrigger) {
						objectDetected = objectToCheck;

						activateEvent ();
					}
				}

				if (checkTag (objectToCheck)) {
					objectDetected = objectToCheck;

					activateEvent ();
				}

				if (justCallOnTrigger) {
					objectDetected = objectToCheck;

					activateEvent ();
				}
			}
		}
	}

	public GameObject getLastObjectDetected ()
	{
		return objectDetected;
	}

	public bool checkTag (GameObject objectToCheck)
	{
		if (useTagToTrigger) {
			if (useTagList) {
				if (tagList.Contains (objectToCheck.tag)) {
					return true;
				}
			} else {
				if (objectToCheck.CompareTag (tagNeededToTrigger)) {
					return true;
				}
			}
		}

		return false;
	}

	public void checkTriggerEventTypeByFunction ()
	{
		if ((!eventTriggered || callFunctionEveryTimeTriggered) && !triggeredByButton) {
			activateEvent ();
		}
	}

	public void resetEventTriggered ()
	{
		eventTriggered = false;
	}

	public void setEventAsTriggered ()
	{
		eventTriggered = true;
	}

	public void setObjectOnTriggerEnterExternally (GameObject newObject)
	{
		Collider newCollider = newObject.GetComponent<Collider> ();

		if (newCollider != null) {
			OnTriggerEnter (newCollider);
		}
	}

	void OnTriggerEnter (Collider col)
	{
		//if it uses layermask and the object touched have a different layer, ignore it
		if (useLayerMask) {
			if ((1 << col.gameObject.layer & layerMask.value) != 1 << col.gameObject.layer) {
				return;
			}
		}

		checkTriggerEventType (col.gameObject, triggerType.enter);
	}

	void OnTriggerExit (Collider col)
	{
		//if it uses layermask and the object touched have a different layer, ignore it
		if (useLayerMask) {
			if ((1 << col.gameObject.layer & layerMask.value) != 1 << col.gameObject.layer) {
				return;
			}
		}

		checkTriggerEventType (col.gameObject, triggerType.exit);
	}

	//EDITOR FUNCTIONS
	public void addNewEvent ()
	{
		eventInfo newEvent = new eventInfo ();

		eventList.Add (newEvent);

		updateComponent ();
	}

	public void InsertEventAtIndex (int index)
	{
		eventInfo newEvent = new eventInfo ();

		eventList.Insert (index + 1, newEvent);

		updateComponent ();
	}

	public void setSimpleFunctionByTag (string functionName, GameObject objectTocall, string tag)
	{
		addNewEvent ();

		eventInfo newEvent = eventList [eventList.Count - 1];

		newEvent.objectToCall = objectTocall;

		newEvent.functionNameList.Add (functionName);

		useTagToTrigger = true;

		tagNeededToTrigger = tag;
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class eventInfo
	{
		public string name;
		public GameObject objectToCall;
		public List<string> functionNameList = new List<string> ();

		public float secondsDelay;
		public bool useRandomDelay;
		public Vector2 randomDelayRange;
	
		public bool sendGameObject;
		public GameObject objectToSend;
		public bool sendObjectDetected;
		public string sendObjectDetectedFunction;

		public bool sendObjectDetectedByEvent;
		public eventParameters.eventToCallWithGameObject eventToSendObjectDetected;

		public bool useEventFunction;

		public UnityEvent eventFunction = new UnityEvent ();

		public bool useBroadcastMessage;
		public List<string> broadcastMessageStringList = new List<string> ();

		public bool useRemoteEvent;
		public List<string> remoteEventNameList = new List<string> ();
	}
}
