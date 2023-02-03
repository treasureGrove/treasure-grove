using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterActionSystemChecker : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool actionSystemCheckerEnabled = true;

	public List<string> actionsToPlayAutomaticallyList = new List<string> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Components")]
	[Space]

	public playerActionSystem mainPlayerActionSystem;



	public void checkToPlayActionAutomatically ()
	{
		if (!actionSystemCheckerEnabled) {
			return;
		}

		bool isActionActive = mainPlayerActionSystem.isActionActive ();

		if (showDebugPrint) {
			print ("Is action active " + isActionActive);
		}

		if (isActionActive) {
			return;
		}

		string currentActionDetected = mainPlayerActionSystem.getCurrentActionName ();

		if (showDebugPrint) {
			print ("checking action detected " + currentActionDetected);
		}

		if (currentActionDetected != "" && actionsToPlayAutomaticallyList.Contains (currentActionDetected)) {
			if (showDebugPrint) {
				print ("action detected, calling to play animation for " + currentActionDetected);
			}

			mainPlayerActionSystem.playCurrentAnimation ();
		}
	}
}
