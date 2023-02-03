using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class persuasionSystem : statType
{
	[Header ("Custom Settings")]
	[Space]

	public int currentPersuasionAmount;


	public override void eventToInitializeStat (float newValue)
	{
		currentPersuasionAmount = (int)newValue;
	}

	public override void eventToIncreaseStat (float newValue)
	{
		currentPersuasionAmount += (int)newValue;
	}

	public override void eventToUseStat (float newValue)
	{
		currentPersuasionAmount -= (int)newValue;
	}

	public override void eventToAddAmount (float newValue)
	{
		currentPersuasionAmount += (int)newValue;
	}

	public override void eventToInitializeBoolStat (bool newValue)
	{

	}

	public override void eventToActivateBoolStat (bool newValue)
	{

	}

	public override void eventToInitializeStatOnComponent ()
	{

	}

	public override void eventToInitializeBoolStatOnComponent ()
	{

	}
}
