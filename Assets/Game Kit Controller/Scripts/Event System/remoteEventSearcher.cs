using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remoteEventSearcher : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool remoteEventsEnabled = true;

	public Transform mainTransformCenter;

	[Space]
	[Header ("Remote Event Searcher Info List Settings")]
	[Space]

	public List<remoteEventSearcherInfo> remoteEventSearcherInfoList = new List<remoteEventSearcherInfo> ();

	int currentRemoteEventSearcherInfoIndex;

	remoteEventActivator.removeEventInfo currentEventInfo;

	public void activateRemoteEventSearcher (string remoteEventSearcherName)
	{
		if (!remoteEventsEnabled) {
			return;
		}

		for (int i = 0; i < remoteEventSearcherInfoList.Count; i++) {
			if (remoteEventSearcherInfoList [i].Name.Equals (remoteEventSearcherName)) {
				currentRemoteEventSearcherInfoIndex = i;
			}
		}

		remoteEventSearcherInfo currentRemoteEventSearcherInfo = remoteEventSearcherInfoList [currentRemoteEventSearcherInfoIndex];

		if (!currentRemoteEventSearcherInfo.remoteEventEnabled) {
			return;
		}

		if (mainTransformCenter == null) {
			mainTransformCenter = transform;
		}

		Collider[] colliders = Physics.OverlapSphere (mainTransformCenter.position, currentRemoteEventSearcherInfo.radiusLayer, currentRemoteEventSearcherInfo.radiusLayer);

		for (int i = 0; i < colliders.Length; i++) {		
			bool canActivateEvent = true;

			GameObject currentObjectDetected = colliders [i].gameObject;

			if (currentRemoteEventSearcherInfo.ignoreCharacters) {
				if (currentRemoteEventSearcherInfo.characterObjectToIgnoreList.Count > 0) {
					if (currentRemoteEventSearcherInfo.characterObjectToIgnoreList.Contains (currentObjectDetected.gameObject)) {
						canActivateEvent = false;
					}
				}

				if (currentRemoteEventSearcherInfo.tagsToIgnore.Contains (currentObjectDetected.tag)) {
					canActivateEvent = false;
				}
			}

			if (canActivateEvent) {
				remoteEventSystem currentRemoteEventSystem = currentObjectDetected.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {

					if (currentRemoteEventSearcherInfo.useRemoveEventInfoList) {
						int removeEventInfoListCount = currentRemoteEventSearcherInfo.mainRemoveEventInfoList.Count;

						string currentRemoteEventToCall = currentRemoteEventSearcherInfo.remoteEventToCall;

						for (int j = 0; j < removeEventInfoListCount; j++) {
							currentEventInfo = currentRemoteEventSearcherInfo.mainRemoveEventInfoList [j];

							if (!currentRemoteEventSearcherInfo.useSameRemoteEventToCall) {
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
						if (currentRemoteEventSearcherInfo.useAmount) {
							currentRemoteEventSystem.callRemoteEventWithAmount (currentRemoteEventSearcherInfo.remoteEventToCall, currentRemoteEventSearcherInfo.amountValue);
						} else if (currentRemoteEventSearcherInfo.useBool) {
							currentRemoteEventSystem.callRemoteEventWithBool (currentRemoteEventSearcherInfo.remoteEventToCall, currentRemoteEventSearcherInfo.boolValue);
						} else if (currentRemoteEventSearcherInfo.useGameObject) {
							currentRemoteEventSystem.callRemoteEventWithGameObject (currentRemoteEventSearcherInfo.remoteEventToCall, currentRemoteEventSearcherInfo.gameObjectToUse);
						} else if (currentRemoteEventSearcherInfo.useTransform) {
							currentRemoteEventSystem.callRemoteEventWithTransform (currentRemoteEventSearcherInfo.remoteEventToCall, currentRemoteEventSearcherInfo.transformToUse);
						} else {
							currentRemoteEventSystem.callRemoteEvent (currentRemoteEventSearcherInfo.remoteEventToCall);
						}
					}

				}
			}
		}
	}

	[System.Serializable]
	public class remoteEventSearcherInfo
	{
		public string Name;

		public bool remoteEventEnabled = true;

		[Space]
		[Space]

		public string remoteEventToCall;

		public bool useRemoveEventInfoList;

		public bool useSameRemoteEventToCall = true;

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
		[Space]

		public bool ignoreCharacters;

		public List<string> tagsToIgnore = new List<string> ();

		public List<GameObject> characterObjectToIgnoreList = new List<GameObject> ();

		[Space]
		[Space]

		public float radiusToSearch;
		public LayerMask radiusLayer;

		[Space]
		[Space]

		public List<remoteEventActivator.removeEventInfo> mainRemoveEventInfoList = new List<remoteEventActivator.removeEventInfo> ();
	}
}
