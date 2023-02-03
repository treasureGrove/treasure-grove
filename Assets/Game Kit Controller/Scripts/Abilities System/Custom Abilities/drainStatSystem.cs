using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class drainStatSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool drainEnabled;

	public LayerMask layerToCheck;

	public bool useSphereCastAll;
	public float sphereCastAllRadius;

	[Space]
	[Header ("Drain Settings")]
	[Space]

	public bool drainStatsEnabled = true;
	public bool activateDrainCoroutineDirectly;

	public float drainStatRate = 0.3f;
	public float maxDrainDistance = 20;

	public bool moveObjectCloserToPlayer;
	public float moveObjectSpeed = 5;
	public float lookObjectSpeed = 5;
	public float minDistanceToMove = 3;

	public float startDrainDelay;

	public bool stopDrainDirectly = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool drainActive;
	public bool drainActionPaused;
	public bool moveObjectActive;

	public List<GameObject> detectedGameObjectList = new List<GameObject> ();

	[Space]
	[Header ("Stats Settings")]
	[Space]

	public List<statsInfo> statsInfoList = new List<statsInfo> ();

	[Space]
	[Header ("Remote Event Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameListOnStart = new List<string> ();
	public List<string> remoteEventNameListOnEnd = new List<string> ();

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnStartDrain;
	public UnityEvent envetOnEndDrain;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform mainCameraTransform;
	public Transform playerTransform;

	public playerStatsSystem mainPlayerStatsSystem;


	List<playerStatsSystem> playerStatsSystemList = new List<playerStatsSystem> ();

	RaycastHit hit;

	Coroutine drainCoroutine;

	remoteEventSystem currentRemoteEventSystem;

	RaycastHit[] hitsList;

	Coroutine moveCoroutine;


	public void checkTargetToDrain ()
	{
		if (!drainEnabled) {
			return;
		}

		if (drainActionPaused) {
			return;
		}

		if (drainActive) {
			if (stopDrainDirectly) {
				stopDrain ();
			} else {
				envetOnEndDrain.Invoke ();
			}

			return;
		}

		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, maxDrainDistance, layerToCheck)) {
			bool startDrain = false;

			detectedGameObjectList.Clear ();

			playerStatsSystemList.Clear ();

			if (useSphereCastAll) {
				Ray newRay = new Ray (hit.point, mainCameraTransform.forward);

				hitsList = Physics.SphereCastAll (newRay, sphereCastAllRadius, maxDrainDistance, layerToCheck);

				List<GameObject> temporalGameObjectList = new List<GameObject> ();
			
				for (int i = 0; i < hitsList.Length; i++) {
					temporalGameObjectList.Add (hitsList [i].collider.gameObject);
				}

				for (int i = 0; i < temporalGameObjectList.Count; i++) {
					playerComponentsManager currentPlayerComponentsManager = temporalGameObjectList [i].GetComponent<playerComponentsManager> ();

					if (currentPlayerComponentsManager != null) {
						playerStatsSystem statsSystemDetected = currentPlayerComponentsManager.getPlayerStatsSystem ();

						if (statsSystemDetected != null) {
							playerStatsSystemList.Add (statsSystemDetected);

							detectedGameObjectList.Add (temporalGameObjectList [i]);

							startDrain = true;
						}
					}
				}
			} else {
				playerComponentsManager currentPlayerComponentsManager = hit.collider.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					playerStatsSystem statsSystemDetected = currentPlayerComponentsManager.getPlayerStatsSystem ();

					if (statsSystemDetected != null) {
						playerStatsSystemList.Add (statsSystemDetected);

						detectedGameObjectList.Add (hit.collider.gameObject);

						startDrain = true;
					}
				}
			}

			if (startDrain) {
				if (showDebugPrint) {
					print ("start drain");
				}

				eventOnStartDrain.Invoke ();

				drainActive = true;

				if (activateDrainCoroutineDirectly) {
					startDrainCoroutine ();
				}
			}
		}
	}

	public void startDrainCoroutine ()
	{
		stopActivateDrainStatCoroutine ();

		drainCoroutine = StartCoroutine (activateDrainStatCoroutine ());
	}

	public void stopActivateDrainStatCoroutine ()
	{
		if (drainCoroutine != null) {
			StopCoroutine (drainCoroutine);
		}
	}

	IEnumerator activateDrainStatCoroutine ()
	{
		yield return new WaitForSeconds (startDrainDelay);

		if (drainStatsEnabled) {
			bool statTotallyDrained = false;

			int statsDrainedAmount = 0;

			while (!statTotallyDrained) {
				yield return new WaitForSeconds (drainStatRate);

				for (int i = 0; i < statsInfoList.Count; i++) {
				
					mainPlayerStatsSystem.addOrRemovePlayerStatAmount (statsInfoList [i].statToIncrease, statsInfoList [i].increaseStatAmount);

					statsDrainedAmount = 0;

					for (int j = 0; j < statsInfoList [i].statToDrainList.Count; j++) {
						for (int k = 0; k < playerStatsSystemList.Count; k++) {
							playerStatsSystemList [k].addOrRemovePlayerStatAmount (statsInfoList [i].statToDrainList [j], statsInfoList [i].drainStatAmount);

							if (playerStatsSystemList [k].getStatValue (statsInfoList [i].statToDrainList [j]) <= 0) {
								statsDrainedAmount++;
							}
						}
					}

					if (useSphereCastAll) {
						if (statsDrainedAmount >= statsInfoList [i].statToDrainList.Count * playerStatsSystemList.Count) {
							statTotallyDrained = true;
						}
					} else {
						if (statsDrainedAmount >= statsInfoList [i].statToDrainList.Count) {
							statTotallyDrained = true;
						}
					}
				}
				
				yield return null;
			}
		}

		yield return null;

		stopDrain ();

		if (!stopDrainDirectly) {
			envetOnEndDrain.Invoke ();
		}
	}

	public void stopDrain ()
	{
		if (!drainEnabled) {
			return;
		}

		if (drainActive) {
			if (showDebugPrint) {
				print ("stop drain");
			}

			stopActivateDrainStatCoroutine ();

			if (stopDrainDirectly) {
				envetOnEndDrain.Invoke ();
			}

			drainActive = false;

			checkRemoteEventOnEnd ();

			moveObjectActive = false;

			stopMoveObjectCoroutine ();

			drainActionPaused = false;
		}
	}

	public void checkRemoteEventOnStart ()
	{
		if (useRemoteEventOnObjectsFound) {
			for (int i = 0; i < detectedGameObjectList.Count; i++) {
				currentRemoteEventSystem = detectedGameObjectList [i].GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int j = 0; j < remoteEventNameListOnStart.Count; j++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnStart [j]);
					}
				}
			}
		}
	}

	public void checkRemoteEventOnEnd ()
	{
		if (useRemoteEventOnObjectsFound) {
			for (int i = 0; i < detectedGameObjectList.Count; i++) {
				currentRemoteEventSystem = detectedGameObjectList [i].GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int j = 0; j < remoteEventNameListOnEnd.Count; j++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventNameListOnEnd [j]);
					}
				}
			}
		}
	}

	public void startMoveObjectCoroutine ()
	{
		stopMoveObjectCoroutine ();

		moveCoroutine = StartCoroutine (moveObjectCoroutine ());
	}

	public void stopMoveObjectCoroutine ()
	{
		if (moveCoroutine != null) {
			StopCoroutine (moveCoroutine);
		}
	}

	IEnumerator moveObjectCoroutine ()
	{
		while (drainActive) {
			if (moveObjectCloserToPlayer && moveObjectActive) {
				for (int i = 0; i < detectedGameObjectList.Count; i++) {
					moveObject (detectedGameObjectList [i].transform);
				}
			}

			yield return null;
		}
	}

	void moveObject (Transform objectToMove)
	{
		float currentDistance = GKC_Utils.distance (playerTransform.position, objectToMove.position);

		if (currentDistance > minDistanceToMove) {
			objectToMove.position = Vector3.Lerp (objectToMove.position, playerTransform.position, Time.deltaTime * moveObjectSpeed);
		}

		Vector3 lookDirection = playerTransform.position - objectToMove.position;
		lookDirection = lookDirection / lookDirection.magnitude;

		Quaternion targetRotation = Quaternion.LookRotation (lookDirection);

		objectToMove.rotation = Quaternion.Lerp (objectToMove.rotation, targetRotation, Time.deltaTime * lookObjectSpeed);
	}

	public void setDrainActionPausedState (bool state)
	{
		drainActionPaused = state;
	}

	public void setMoveObjectActiveState (bool state)
	{
		moveObjectActive = state;

		if (moveObjectActive) {
			startMoveObjectCoroutine ();
		}
	}

	[System.Serializable]
	public class statsInfo
	{
		public string statToIncrease;

		public float increaseStatAmount = 5;
		public float drainStatAmount = 5;

		public List<string> statToDrainList = new List<string> ();
	}
}
