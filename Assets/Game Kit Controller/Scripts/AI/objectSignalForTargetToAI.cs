using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectSignalForTargetToAI : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float rangeToDetectAI = 20;
	public LayerMask layerDetection;

	public bool activateDetectionAtStart;
	public float sendDetectionSignalRate;

	public bool checkToSendToSingleAI;

	public bool disableSignalOnceAIReached;

	public int noiseID = -1;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool singleAIDetected;

	public GameObject currentAIDetected;

	public findObjectivesSystem currentFindObjectivesSystem;
	public AINavMesh currentAINavMesh;

	Coroutine signalCoroutine;

	void Start ()
	{
		if (activateDetectionAtStart) {
			activateSignalWithRate ();
		}
	}

	public void activateSignalWithRate ()
	{
		stopCheckSignalCoroutine ();

		signalCoroutine = StartCoroutine (checkSignalCoroutine ());
	}

	void stopCheckSignalCoroutine ()
	{
		if (signalCoroutine != null) {
			StopCoroutine (signalCoroutine);
		}
	}

	IEnumerator checkSignalCoroutine ()
	{
		float lastTimeSignalSent = 0;

		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			if (singleAIDetected) {
				if (currentAINavMesh.isFollowingTarget ()) {
					if (currentAINavMesh.getCurrentTarget () != transform) {
						checkToDetectAI ();
					} else {
						if (disableSignalOnceAIReached) {
							if (GKC_Utils.distance (currentAIDetected.transform.position, transform.position) < 0.5f) {
								stopCheckSignalCoroutine ();
							}
						}
					}
				} else {
					checkToDetectAI ();
				}
			} else {
				if (Time.time > lastTimeSignalSent + sendDetectionSignalRate) {
					checkToDetectAI ();

					sendDetectionSignalRate = Time.time;
				}
			}
		}
	}

	public void checkToDetectAI ()
	{
		if (checkToSendToSingleAI) {
			currentAIDetected = applyDamage.sendNoiseSignalToClosestAI (rangeToDetectAI, transform.position + Vector3.up,
				layerDetection, 0, true, false, noiseID);
		
			singleAIDetected = currentAIDetected != null;

			if (singleAIDetected) {
				currentFindObjectivesSystem = currentAIDetected.GetComponent<findObjectivesSystem> ();

				currentAINavMesh = currentAIDetected.GetComponent<AINavMesh> ();
			}
		} else {
			applyDamage.sendNoiseSignal (rangeToDetectAI, transform.position + Vector3.up, layerDetection, 0, true, false, noiseID);
		}
	}
}
