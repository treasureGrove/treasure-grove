using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class perfectDodgeSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool perfectDogdeEnabled = true;

	public bool perfectDodgeActive;

	[Space]
	[Header ("Slow Down Character Settings")]
	[Space]

	public string characterStateAffectedName = "Character Slow Down Velocity";

	public float slowDownSpeedOnCharacters = 0.2f;
	public float slowDownSpeedDurationOnCharacters = 4;

	public bool activateSlowDownOnMainPlayer;
	public float slowDownSpeedOnPlayer = 0.8f;
	public float slowDownSpeedDurationOnPlayer = 2;

	public bool useCoolDown;
	public float coolDownAmount;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStateChange;
	public UnityEvent eventOnPerfectDodgeEnabled;
	public UnityEvent eventOnPerfectDodgeDisabled;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	Coroutine perfectDodgeCoroutine;

	List<playerComponentsManager> AIPlayerControllerList = new List<playerComponentsManager> ();

	float lastTimePerfectDodgeActive;

	public void activatePerfectDodge ()
	{
		if (!perfectDogdeEnabled) {
			return;
		}

		if (perfectDodgeActive && Time.time < lastTimePerfectDodgeActive + 0.5f) {
			return;
		}

		if (useCoolDown) {
			if (Time.time < lastTimePerfectDodgeActive + coolDownAmount) {
				return;
			}
		}

		lastTimePerfectDodgeActive = Time.time;


		playerComponentsManager[] playerComponentsManagerList = FindObjectsOfType<playerComponentsManager> ();

		foreach (playerComponentsManager currentPlayerComponentsManager in playerComponentsManagerList) {
			if (!AIPlayerControllerList.Contains (currentPlayerComponentsManager)) {
				AIPlayerControllerList.Add (currentPlayerComponentsManager);
			}
		}

		for (int i = AIPlayerControllerList.Count - 1; i >= 0; i--) {
			if (AIPlayerControllerList [i] != null) {
		
				characterPropertiesSystem currentCharacterPropertiesSystem = AIPlayerControllerList [i].getCharacterPropertiesSystem ();

				if (currentCharacterPropertiesSystem != null) {
					currentCharacterPropertiesSystem.activateStateAffected (characterStateAffectedName, slowDownSpeedDurationOnCharacters, slowDownSpeedOnCharacters);
				}
			} else {
				AIPlayerControllerList.RemoveAt (i);
			}
		}

		if (activateSlowDownOnMainPlayer) {
			mainPlayerController.setNewAnimSpeedMultiplierDuringXTime (slowDownSpeedDurationOnPlayer);
			mainPlayerController.setReducedVelocity (slowDownSpeedOnPlayer);
		}

		stopActivatePerfectDodgeCoroutine ();

		perfectDodgeCoroutine = StartCoroutine (activatePerfectDodgeCoroutine ());
	}

	IEnumerator activatePerfectDodgeCoroutine ()
	{
		perfectDodgeActive = true;

		checkEventsOnStateChange (true);

		yield return new WaitForSeconds (slowDownSpeedDurationOnCharacters);

		checkEventsOnStateChange (false);

		perfectDodgeActive = false;
	}

	void stopActivatePerfectDodgeCoroutine ()
	{
		if (perfectDodgeCoroutine != null) {
			StopCoroutine (perfectDodgeCoroutine);
		}

		if (perfectDodgeActive) {
			checkEventsOnStateChange (false);

			perfectDodgeActive = false;
		}
	}

	void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				eventOnPerfectDodgeEnabled.Invoke ();
			} else {
				eventOnPerfectDodgeDisabled.Invoke ();
			}
		}
	}
}