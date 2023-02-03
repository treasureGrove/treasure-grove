using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walkOnBalanceTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool movementZoneActive = true;

	public simpleWaypointSystem mainSimpleWaypointSystem;

	public bool checkOnTriggerEnter = true;
	public bool checkOnTriggerExit = true;

	public bool setPlayerAsChild = true;
	public Transform playerParentTransform;

	[Space]
	[Header ("Jump On Target Position Reached Settings")]
	[Space]

	public bool setJumpOnEndState;
	public bool jumpOnEndState;

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
		if (!movementZoneActive) {
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
				externalControllerBehavior movementExternalControllerBehavior = currentPlayerComponentsManager.getWalkOnBalanceExternaControllerBehavior ();

				if (movementExternalControllerBehavior != null) {
					walkOnBalanceSystem currentMovementSystem = movementExternalControllerBehavior.GetComponent<walkOnBalanceSystem> ();

					if (playerParentTransform == null) {
						playerParentTransform = transform;
					}

					currentMovementSystem.setSetPlayerAsChildStateState (setPlayerAsChild, playerParentTransform);

					currentMovementSystem.setCurrentWaypoint (mainSimpleWaypointSystem);

					currentMovementSystem.setMovementSystemActivestate (true);

					if (setJumpOnEndState) {
						currentMovementSystem.setJumpOnEndState (jumpOnEndState);
					}

					checkRemoteEvents (true, currentPlayer);
				}
			}
		} else {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				externalControllerBehavior movementExternalControllerBehavior = currentPlayerComponentsManager.getWalkOnBalanceExternaControllerBehavior ();

				if (movementExternalControllerBehavior != null) {
					walkOnBalanceSystem currentMovementSystem = movementExternalControllerBehavior.GetComponent<walkOnBalanceSystem> ();

					currentMovementSystem.setSetPlayerAsChildStateState (false, null);

					currentMovementSystem.setMovementSystemActivestate (false);

					currentMovementSystem.setCurrentWaypoint (null);

					currentMovementSystem.setJumpOnEndState (true);

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