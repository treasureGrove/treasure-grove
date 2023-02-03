using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class railTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool railZoneActive = true;

	public float bezierDuration;

	public BezierSpline railSpline;

	public bool checkOnTriggerEnter = true;
	public bool checkOnTriggerExit = true;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEvents;
	public bool useRemoteEventOnStart;
	public List<string> remoteEventNameListOnStart = new List<string> ();

	public bool useRemoteEventOnEnd;
	public List<string> remoteEventNameListOnEnd = new List<string> ();

	GameObject currentPlayer;


	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (!railZoneActive) {
			return;
		}

		if (isEnter) {
			if (!checkOnTriggerEnter) {
				return;
			}
		} else {
			if (!checkOnTriggerExit) {
				return;
			}
		}

		if (!col.gameObject.CompareTag (tagToCheck)) {
			return;
		}

		if (isEnter) {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior railExternalControllerBehavior = currentPlayerComponentsManager.getRailExternalControllerBehavior ();

				if (railExternalControllerBehavior != null) {
					railSystem currentRailSystem = railExternalControllerBehavior.GetComponent<railSystem> ();

					currentRailSystem.setCurrentSpline (railSpline);

					currentRailSystem.setCurrentBezierDuration (bezierDuration);

					currentRailSystem.setRailSystemActivestate (true);

					checkRemoteEvents (true, currentPlayer);
				}
			}
		} else {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior railExternalControllerBehavior = currentPlayerComponentsManager.getRailExternalControllerBehavior ();

				if (railExternalControllerBehavior != null) {
					railSystem currentRailSystem = railExternalControllerBehavior.GetComponent<railSystem> ();

					currentRailSystem.setRailSystemActivestate (false);

					currentRailSystem.setCurrentSpline (null);

					checkRemoteEvents (false, currentPlayer);
				}
			}
		}
	}

	void checkRemoteEvents (bool state, GameObject objectToCheck)
	{
		if (!useRemoteEvents) {
			return;
		}

		if (state) {
			if (useRemoteEventOnStart) {
				remoteEventSystem currentRemoteEventSystem = objectToCheck.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameListOnStart.Count; i++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnStart [i]);
					}
				}
			}
		} else {
			if (useRemoteEventOnEnd) {
				remoteEventSystem currentRemoteEventSystem = objectToCheck.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameListOnEnd.Count; i++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnEnd [i]);
					}
				}
			}
		}
	}
}