using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class onEnableCheckSystem : MonoBehaviour
{
	public bool checkEnabled;

	public UnityEvent eventOnEnabled;

	void OnEnable ()
	{
		if (checkEnabled) {
			eventOnEnabled.Invoke ();
		}
	}
}
