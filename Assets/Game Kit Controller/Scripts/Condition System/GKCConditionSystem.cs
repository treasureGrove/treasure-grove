using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKCConditionSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool conditionSystemEnabled = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public GKCConditionInfo currentGKCConditionInfo;

	public void setCurrentGKCConditionInfo (GKCConditionInfo newGKCConditionInfo)
	{
		if (!conditionSystemEnabled) {
			return;
		}

		currentGKCConditionInfo = newGKCConditionInfo;
	}

	public void checkIfConditionComplete ()
	{
		if (!conditionSystemEnabled) {
			return;
		}

		if (currentGKCConditionInfo != null) {
			currentGKCConditionInfo.checkIfConditionComplete ();
		}
	}

	public void refreshConditionSystemCheckStatus ()
	{
		checkIfConditionComplete ();
	}
}
