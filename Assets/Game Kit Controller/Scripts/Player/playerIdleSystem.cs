using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerIdleSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool idleEnabledAtStart = true;

	public bool idleActive;

	public int currentIdleInfoIndex;

	public bool playRandomIdle;

	[Space]
	[Header ("Idle Info List Settings")]
	[Space]

	public List<idleInfo> idleInfoList = new List<idleInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnStartIdle;
	public UnityEvent eventOnStopIdle;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	idleInfo currentIdleInfo;

	Coroutine idleCoroutine;

	bool idleStoppedAutomatically;

	void Start ()
	{
		if (idleEnabledAtStart) {
			activateIdle ();
		}

		StartCoroutine (LateFixedUpdate ());
	}

	IEnumerator LateFixedUpdate ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			if (idleActive) {
				if (mainPlayerController.isPlayerUsingInput ()) {
					if (!idleStoppedAutomatically) {
						stopIdle ();

						idleActive = true;

						idleStoppedAutomatically = true;
					}
				} else {
					if (idleStoppedAutomatically) {
						currentIdleInfoIndex = 0;

						playIdle ();

						idleStoppedAutomatically = false;
					}
				}
			}
		}
	}

	public void activateIdle ()
	{
		playIdle ();
	}

	public void deactivateIdle ()
	{
		stopIdle ();
	}

	void stopIdle ()
	{
		if (idleCoroutine != null) {
			StopCoroutine (idleCoroutine);
		}

		idleActive = false;

		if (currentIdleInfo != null) {
			currentIdleInfo.currentState = false;
		}

		eventOnStopIdle.Invoke ();
	}

	void playIdle ()
	{
		stopIdle ();

		eventOnStartIdle.Invoke ();

		if (currentIdleInfoIndex < idleInfoList.Count) {

			currentIdleInfo = idleInfoList [currentIdleInfoIndex];

			idleActive = true;

			idleCoroutine = StartCoroutine (playIdleCoroutine ());
		}
	}

	IEnumerator playIdleCoroutine ()
	{
		currentIdleInfo.currentState = true;

		currentIdleInfo.eventToSetIdle.Invoke ();

		yield return new WaitForSeconds (currentIdleInfo.duration);

		if (playRandomIdle) {
			int currentIndex = currentIdleInfoIndex;

			while (currentIdleInfoIndex == currentIndex) {
				currentIdleInfoIndex = Random.Range (0, idleInfoList.Count);
			}
		} else {
			currentIdleInfoIndex++;
		}

		if (currentIdleInfoIndex > idleInfoList.Count - 1) {
			currentIdleInfoIndex = 0;
		}

		currentIdleInfo.currentState = false;

		currentIdleInfo = idleInfoList [currentIdleInfoIndex];

		currentIdleInfo.currentState = true;

		playIdle ();
	}

	public void setIdleIndex (int newValue)
	{
		currentIdleInfoIndex = newValue;

		if (currentIdleInfoIndex > idleInfoList.Count - 1) {
			currentIdleInfoIndex = 0;
		}
	}

	[System.Serializable]
	public class idleInfo
	{
		public string Name;

		public bool currentState;

		public float duration;

		public UnityEvent eventToSetIdle;
	}
}
