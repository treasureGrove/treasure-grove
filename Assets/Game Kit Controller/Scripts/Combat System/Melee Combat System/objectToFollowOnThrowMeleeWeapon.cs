using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectToFollowOnThrowMeleeWeapon : MonoBehaviour
{
	public Transform mainObjectToFollow;

	public bool componentedAddedTemporaly;

	public Transform getMainObjectToFollow ()
	{
		if (mainObjectToFollow == null) {
			mainObjectToFollow = transform;
		}

		return mainObjectToFollow;
	}

	public void setComponentedAddedTemporalyState (bool state)
	{
		componentedAddedTemporaly = state;
	}
}
