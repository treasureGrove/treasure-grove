using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerStealthSystem : abilityInfo
{
	[Header ("Custom Settings")]
	[Space]

	public bool stealthModeEnabled = true;
	public bool stealthModeActive;

	public UnityEvent eventToActivateStealthMode;
	public UnityEvent eventToDeactivateStealthMode;

	public override void updateAbilityState ()
	{
		
	}

	public void setStealthModeState (bool state)
	{
		if (!stealthModeEnabled) {
			return;
		}

		stealthModeActive = state;

		if (stealthModeActive) {
			eventToActivateStealthMode.Invoke ();
		} else {
			eventToDeactivateStealthMode.Invoke ();
		}
	}

	Coroutine timeLimitCoroutine;

	public void stopSetStealthModeTimeLimit ()
	{
		if (timeLimitCoroutine != null) {
			StopCoroutine (timeLimitCoroutine);
		}
	}

	public void setStealthModeTimeLimit ()
	{
		stopSetStealthModeTimeLimit ();

		timeLimitCoroutine = StartCoroutine (setStealthModeTimeLimitCoroutine ());
	}

	IEnumerator setStealthModeTimeLimitCoroutine ()
	{
		yield return new WaitForSeconds (timeLimit);

		setStealthModeState (false);
	}

	public override void enableAbility ()
	{
		stealthModeEnabled = true;
	}

	public override void disableAbility ()
	{
		if (stealthModeActive) {
			setStealthModeState (false);
		}

		stealthModeEnabled = false;
	}

	public override void deactivateAbility ()
	{
		if (stealthModeActive) {
			setStealthModeState (false);
		}
	}

	public override void activateSecondaryActionOnAbility ()
	{

	}

	public override void useAbilityPressDown ()
	{
		if (!mainPlayerAbilitiesSystem.playerCurrentlyBusy) {

			setStealthModeState (!stealthModeActive);

			checkUseEventOnUseAbility ();
		}
	}

	public override void useAbilityPressHold ()
	{

	}

	public override void useAbilityPressUp ()
	{

	}
}
