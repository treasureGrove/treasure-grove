using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class externalDamageProperties : MonoBehaviour
{
	public eventParameters.eventToCallWithGameObject eventToSetDamageOwner;

	public GameObject currentDamageOwner;

	public void setDamageOwner (GameObject newOwner)
	{
		currentDamageOwner = newOwner;

		eventToSetDamageOwner.Invoke (currentDamageOwner);
	}
}
