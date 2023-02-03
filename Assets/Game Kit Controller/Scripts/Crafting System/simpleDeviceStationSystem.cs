using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class simpleDeviceStationSystem : craftingStationSystem
{
	[Header ("Main Settings")]
	[Space]

	public bool stationEnabled = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool stationActive;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnConnectInput;
	public UnityEvent eventOnDisconnectInput;


	public override void checkStateOnSetInput ()
	{
		if (stationEnabled) {
			if (stationActive) {

				return;
			}

			stationActive = true;

			eventOnConnectInput.Invoke ();
		}
	}

	public override void checkStateOnRemoveInput ()
	{
		if (stationEnabled) {
			if (!stationActive) {

				return;
			}

			stationActive = false;

			eventOnDisconnectInput.Invoke ();
		}
	}
}
