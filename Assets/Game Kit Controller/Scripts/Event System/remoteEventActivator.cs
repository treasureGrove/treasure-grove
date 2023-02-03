using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remoteEventActivator : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string remoteEventToCall;

	[Space]
	[Header ("Remote Event Info Settings")]
	[Space]

	public bool useAmount;
	public float amountValue;

	[Space]

	public bool useBool;
	public bool boolValue;

	[Space]

	public bool useGameObject;
	public GameObject gameObjectToUse;

	[Space]

	public bool useTransform;
	public Transform transformToUse;

	[Space]
	[Header ("Remote Event List Settings")]
	[Space]

	public bool useRemoveEventInfoList;

	public bool useSameRemoteEvenToCall = true;

	[Space]

	public List<removeEventInfo> removeEventInfoList = new List<removeEventInfo> ();

	[Space]
	[Header ("Set Object Manually Settings")]
	[Space]

	public bool assignObjectManually;
	public GameObject objectToAssign;

	public bool searchPlayerOnSceneIfNotAssigned;

	GameObject currentObjectToCall;

	removeEventInfo currentEventInfo;

	public void callRemoteEvent (GameObject objectToCall)
	{
		currentObjectToCall = objectToCall;

		callEvent ();
	}

	public void setObjectToCall (GameObject objectToCall)
	{
		currentObjectToCall = objectToCall;
	}

	public void callEvent ()
	{
		if (assignObjectManually) {
			if (objectToAssign == null) {
				if (searchPlayerOnSceneIfNotAssigned) {
					findPlayerOnScene ();
				}
			}

			if (objectToAssign == null) {
				print ("WARNING: no object has been assigned manually on remote event activator");

				return;
			}

			currentObjectToCall = objectToAssign;
		}

		remoteEventSystem currentRemoteEventSystem = currentObjectToCall.GetComponent<remoteEventSystem> ();

		if (currentRemoteEventSystem != null) {

			if (useRemoveEventInfoList) {
				int removeEventInfoListCount = removeEventInfoList.Count;

				string currentRemoteEventToCall = remoteEventToCall;

				for (int i = 0; i < removeEventInfoListCount; i++) {
					currentEventInfo = removeEventInfoList [i];

					if (!useSameRemoteEvenToCall) {
						currentRemoteEventToCall = currentEventInfo.remoteEventToCall;
					} 

					if (currentEventInfo.useAmount) {
						currentRemoteEventSystem.callRemoteEventWithAmount (currentRemoteEventToCall, currentEventInfo.amountValue);
					} else if (currentEventInfo.useBool) {
						currentRemoteEventSystem.callRemoteEventWithBool (currentRemoteEventToCall, currentEventInfo.boolValue);
					} else if (currentEventInfo.useGameObject) {
						currentRemoteEventSystem.callRemoteEventWithGameObject (currentRemoteEventToCall, currentEventInfo.gameObjectToUse);
					} else if (currentEventInfo.useTransform) {
						currentRemoteEventSystem.callRemoteEventWithTransform (currentRemoteEventToCall, currentEventInfo.transformToUse);
					} else {
						currentRemoteEventSystem.callRemoteEvent (currentRemoteEventToCall);
					}
				}
			} else {
				if (useAmount) {
					currentRemoteEventSystem.callRemoteEventWithAmount (remoteEventToCall, amountValue);
				} else if (useBool) {
					currentRemoteEventSystem.callRemoteEventWithBool (remoteEventToCall, boolValue);
				} else if (useGameObject) {
					currentRemoteEventSystem.callRemoteEventWithGameObject (remoteEventToCall, gameObjectToUse);
				} else if (useTransform) {
					currentRemoteEventSystem.callRemoteEventWithTransform (remoteEventToCall, transformToUse);
				} else {
					currentRemoteEventSystem.callRemoteEvent (remoteEventToCall);
				}
			}
		}
	}

	public void findPlayerOnScene ()
	{
		if (searchPlayerOnSceneIfNotAssigned) {
			objectToAssign = GKC_Utils.findMainPlayerOnScene ();
		}
	}

	[System.Serializable]
	public class removeEventInfo
	{
		public string Name;

		[Space]
		[Space]

		public string remoteEventToCall;

		[Space]

		public bool useAmount;
		public float amountValue;

		[Space]

		public bool useBool;
		public bool boolValue;

		[Space]

		public bool useGameObject;
		public GameObject gameObjectToUse;

		[Space]

		public bool useTransform;
		public Transform transformToUse;
	}
}
