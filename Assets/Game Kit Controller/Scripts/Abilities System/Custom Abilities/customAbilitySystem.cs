using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class customAbilitySystem : abilityInfo
{
	[Space]
	[Header ("Custom Settings")]
	[Space]

	public bool useEventOnPressDown;
	public UnityEvent eventOnPressDown;

	public bool useEventOnPressHold;
	public UnityEvent eventOnPressHold;

	public bool useEventOnPressUp;
	public UnityEvent eventOnPressUp;

	public bool showDebugPrint;

	[Space]
	[Header ("Press Delay Settings")]
	[Space]

	public bool useDelayTimeToUseEventOnPressUp;
	public float delayTimeToUseEventOnPressUp;

	public bool useEventOnPressUpBeforeAndAfter;
	public UnityEvent eventOnPressUpBefore;
	public UnityEvent eventOnPressUpAfter;

	public float delayTimeToUseEventOnPressUpBefore;
	public float delayTimeToUseEventOnPressUpAfter;

	public bool useDelayTimeToUseEventOnPressHold;
	public float delayTimeToUseEventOnPressHold;
	public bool useEventOnPressHoldJustOnce;

	[Space]
	[Header ("Other Events Settings")]
	[Space]

	public bool useEventOnUpdateAbilityState;
	public UnityEvent eventOnUpdateAbilityState;

	public bool useEventOnEnableAbility;
	public UnityEvent eventOnEnableAbility;

	public bool useEventOnDisableAbility;
	public UnityEvent eventOnDisableAbility;

	public bool useEventOnDeactivateAbility;
	public UnityEvent eventOnDeactivateAbility;

	float lastTimePressDownUsed;

	bool eventTriggeredOnPressHold;

	public override void updateAbilityState ()
	{
		if (useEventOnUpdateAbilityState) {
			eventOnUpdateAbilityState.Invoke ();
		}
	}

	public override void enableAbility ()
	{
		if (useEventOnEnableAbility) {
			eventOnEnableAbility.Invoke ();
		}
	}

	public override void disableAbility ()
	{
		if (useEventOnDisableAbility) {
			eventOnDisableAbility.Invoke ();
		}
	}

	public override void deactivateAbility ()
	{
		if (useEventOnDeactivateAbility) {
			eventOnDeactivateAbility.Invoke ();
		}
	}

	public override void activateSecondaryActionOnAbility ()
	{

	}

	public override void useAbilityPressDown ()
	{
		if (useEventOnPressDown) {
			eventOnPressDown.Invoke ();
		}

		lastTimePressDownUsed = Time.time;

		eventTriggeredOnPressHold = false;

		checkUseEventOnUseAbility ();

		if (showDebugPrint) {
			print ("down");
		}
	}

	public override void useAbilityPressHold ()
	{
		if (useEventOnPressHold) {
			if (useDelayTimeToUseEventOnPressHold) {
				if (!useEventOnPressHoldJustOnce || !eventTriggeredOnPressHold) {
					if (Time.time > delayTimeToUseEventOnPressHold + lastTimePressDownUsed) {
						eventOnPressHold.Invoke ();

						if (useEventOnPressHoldJustOnce) {
							eventTriggeredOnPressHold = true;
						}
					}
				}
			} else {
				eventOnPressHold.Invoke ();
			}
		}
	}

	public override void useAbilityPressUp ()
	{
		if (useEventOnPressUp) {
			if (!useEventOnPressHoldJustOnce || !eventTriggeredOnPressHold) {
				if (useEventOnPressUpBeforeAndAfter) {
					if (Time.time < delayTimeToUseEventOnPressUpBefore + lastTimePressDownUsed) {
						eventOnPressUpBefore.Invoke ();
					} else if (Time.time > delayTimeToUseEventOnPressUpAfter + lastTimePressDownUsed) {
						eventOnPressUpAfter.Invoke ();
					}
				} else {
					if (useDelayTimeToUseEventOnPressUp) {
						if (Time.time > delayTimeToUseEventOnPressUp + lastTimePressDownUsed) {
							eventOnPressUp.Invoke ();
						}
					} else {
						eventOnPressUp.Invoke ();
					}
				}

				if (showDebugPrint) {
					print ("up");
				}
			}
		}
	}
}
