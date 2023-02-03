using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterStateAffectedInfo : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool stateEnabled = true;

	public string stateAffectedName;

	public bool stateAffectedActive;

	public virtual void activateStateAffected (float stateDuration, float stateAmount)
	{

	}

	public virtual void activateStateAffected (bool state)
	{

	}

	public bool isStateAffectedActive ()
	{
		return stateAffectedActive;
	}
}
