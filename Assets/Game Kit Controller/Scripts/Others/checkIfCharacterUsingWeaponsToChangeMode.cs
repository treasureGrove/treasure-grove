using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class checkIfCharacterUsingWeaponsToChangeMode : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkStateEnabled = true;

	public bool checkStateAtStartEnabled;

	public float minTimeToCallEventsOnNoCarryingWeapons;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool carryingWeapons;

	public bool notCarryingWeaponsEventCalled;

	public bool coroutineActive;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnNotCarryingWeapons;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	Coroutine mainUpdateCoroutine;

	float lastTimeNotCarryingWeapons;
	bool firstTimeCheck;


	bool carryingWeaponsResult;

	void Start ()
	{
		if (checkStateAtStartEnabled) {
			activateCheck ();
		}
	}

	public void activateCheck ()
	{
		if (!checkStateEnabled) {
			return;
		}

		stopUpdateCheckCoroutine ();

		mainUpdateCoroutine = StartCoroutine (updateCheckCoroutine ());

		coroutineActive = true;

		if (showDebugPrint) {
			print ("activate check weapons state coroutine");
		}
	}

	public void stopCheck ()
	{
		if (!checkStateEnabled) {
			return;
		}

		stopUpdateCheckCoroutine ();

		if (showDebugPrint) {
			print ("deactivate check weapons state coroutine");
		}
	}

	public void stopUpdateCheckCoroutine ()
	{
		if (coroutineActive) {
			if (mainUpdateCoroutine != null) {
				StopCoroutine (mainUpdateCoroutine);
			}

			carryingWeapons = false;

			carryingWeaponsResult = false;

			notCarryingWeaponsEventCalled = false;

			coroutineActive = false;
		}
	}

	IEnumerator updateCheckCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			updateCheck ();
		}
	}

	void updateCheck ()
	{
		carryingWeapons = false;

		if (mainPlayerController.isPlayerUsingWeapons ()) {
			carryingWeapons = true;
		}

		if (mainPlayerController.isPlayerUsingMeleeWeapons () || mainPlayerController.isPlayerMeleeWeaponThrown ()) {
			carryingWeapons = true;
		}

		if (mainPlayerController.isActionActive ()) {
			carryingWeapons = true;
		}

		if (carryingWeaponsResult != carryingWeapons || !firstTimeCheck) {
			carryingWeaponsResult = carryingWeapons;

			if (carryingWeaponsResult) {
				notCarryingWeaponsEventCalled = false;
			} else {
				lastTimeNotCarryingWeapons = Time.time;
			}

			firstTimeCheck = true;
		}

		if (!carryingWeaponsResult) {
			if (!notCarryingWeaponsEventCalled) {
				if (Time.time > lastTimeNotCarryingWeapons + minTimeToCallEventsOnNoCarryingWeapons) {
					eventOnNotCarryingWeapons.Invoke ();

					notCarryingWeaponsEventCalled = true;

					if (showDebugPrint) {
						print ("not carrying weapon enough time, calling event");
					}
				}
			}
		}
	}
}
