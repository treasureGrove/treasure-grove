using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class rollOnLandingSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool rollOnlandingEnabled = true;

	public float rollOnLandingCheckDuration;

	public float eventsOnRollLandingDelay;

	public float distanceFromGroundToActivateRollOnLanding = 1;

	public float minWaitToActivateRollOnLandingInput = 0.5f;

	public LayerMask raycastLayermask;

	public bool useMaxTimeOnAirToActivateRollOnLanding;
	public float maxTimeOnAirToActivateRollOnLanding;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool rollOnLandingCheckActive;

	public bool showDebugPrint;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventsOnRollOnLandingThirdPerson;

	public UnityEvent eventsOnRollOnLandingFirstPerson;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	public Transform playerTransform;

	Coroutine landingCoroutine;

	float lastTimeRollOnLandingActive;

	bool eventsActivated;

	float lastTimeRollOnLandingInputActive;

	bool cancelCheckRollOnLandingOnceActive;

	float lastTimeCancelCheckRoll;

	public void setCancelCheckRollOnLandingOnceActive (bool state)
	{
		cancelCheckRollOnLandingOnceActive = state;

		lastTimeCancelCheckRoll = Time.time;
	}

	public void inputActivateRollOnLanding ()
	{
//		print (cancelCheckRollOnLandingOnceActive + " " + (Time.time) + " " + (lastTimeCancelCheckRoll + 0.7f));

		if (cancelCheckRollOnLandingOnceActive) {
			if (Time.time > lastTimeCancelCheckRoll + 0.7f) {

				cancelCheckRollOnLandingOnceActive = false;
			} else {
				return;
			}
		}

		if (!rollOnlandingEnabled) {
			return;
		}

		if (mainPlayerController.isPlayerOnGround ()) {
			return;
		}

		if (Time.time < minWaitToActivateRollOnLandingInput + lastTimeRollOnLandingInputActive) {
			return;
		}

		stopActivateRollOnLandingCoroutine ();

		bool canActivateRollOnLanding = true;

		if (useMaxTimeOnAirToActivateRollOnLanding) {
			float totalTimeOnAir = Mathf.Abs (Time.time - mainPlayerController.getLastTimeFalling ());

			if (showDebugPrint) {
				print ("Current time on air " + totalTimeOnAir);
			}

			if (totalTimeOnAir > maxTimeOnAirToActivateRollOnLanding) {
				canActivateRollOnLanding = false;

				if (showDebugPrint) {
					print ("Too much time on air, can't activate roll on landing");
				}
			}
		}

		if (canActivateRollOnLanding) {
			landingCoroutine = StartCoroutine (activateRollOnLandingCoroutine ());

			lastTimeRollOnLandingInputActive = Time.time;
		}
	}

	public void inputDeactivateRollOnLanding ()
	{
		if (!rollOnlandingEnabled) {
			return;
		}

		if (mainPlayerController.isPlayerOnGround ()) {
			return;
		}

		stopActivateRollOnLandingCoroutine ();
	}

	void stopActivateRollOnLandingCoroutine ()
	{
		if (landingCoroutine != null) {
			StopCoroutine (landingCoroutine);
		}

		if (rollOnLandingCheckActive) {
			mainPlayerController.setCheckFallStatePausedState (false);

			rollOnLandingCheckActive = false;
		}
	}

	IEnumerator activateRollOnLandingCoroutine ()
	{
		rollOnLandingCheckActive = true;

		lastTimeRollOnLandingActive = Time.time;

		eventsActivated = false;

		mainPlayerController.setCheckFallStatePausedState (true);

		bool targetReached = false;

		while (!targetReached) {
			if (Time.time > lastTimeRollOnLandingActive + rollOnLandingCheckDuration) {
				targetReached = true;
			} else {
				if (!eventsActivated) {
					if (Physics.Raycast (playerTransform.position, -playerTransform.up, distanceFromGroundToActivateRollOnLanding, raycastLayermask)) {
						if (Time.time > lastTimeRollOnLandingActive + eventsOnRollLandingDelay) {
							eventsActivated = true;

							if (showDebugPrint) {
								print ("Activate roll on landing");
							}
								
							if (mainPlayerController.isPlayerOnFirstPerson ()) {
								eventsOnRollOnLandingFirstPerson.Invoke ();
							} else {
								eventsOnRollOnLandingThirdPerson.Invoke ();
							}
						}
					}
				}
			}

			yield return null;
		}

		mainPlayerController.setCheckFallStatePausedState (false);

		rollOnLandingCheckActive = false;
	}
}