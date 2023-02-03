using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class templateStatTypeComponent : statType
{
	public float currentStatAmount;

	public bool currentStateBoolValue;

	public override void eventToInitializeStat (float newValue)
	{
		currentStatAmount = newValue;
	}

	public override void eventToIncreaseStat (float newValue)
	{
		currentStatAmount += newValue;
	}

	public override void eventToUseStat (float newValue)
	{
		currentStatAmount -= newValue;
	}

	public override void eventToAddAmount (float newValue)
	{
		currentStatAmount += newValue;
	}

	public override void eventToInitializeBoolStat (bool newValue)
	{
		currentStateBoolValue = newValue;
	}

	public override void eventToActivateBoolStat (bool newValue)
	{
		currentStateBoolValue = newValue;
	}

	public override void eventToInitializeStatOnComponent ()
	{

	}

	public override void eventToInitializeBoolStatOnComponent ()
	{

	}
}
