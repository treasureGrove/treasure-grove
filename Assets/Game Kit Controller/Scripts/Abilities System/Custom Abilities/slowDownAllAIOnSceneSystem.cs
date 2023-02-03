using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class slowDownAllAIOnSceneSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool slowDownEnabled = true;

	public string playerFactionName = "Player";

	public bool slowDownOnlyEnemies;

	public int pauseCharacterPriority = 1;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool slowDownActive;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnStart;
	public UnityEvent eventOnEnd;

	public void toggleSlowDownState ()
	{
		setSlowDownState (!slowDownActive);
	}

	public void setSlowDownState (bool state)
	{
		if (slowDownEnabled) {
			if (slowDownActive == state) {
				return;
			}

			slowDownActive = state;

			if (slowDownActive) {
				eventOnStart.Invoke ();
			} else {
				eventOnEnd.Invoke ();
			}

			if (slowDownOnlyEnemies) {
				GKC_Utils.pauseOrResumeEnemyAIOnScene (slowDownActive, playerFactionName, pauseCharacterPriority);
			} else {
				GKC_Utils.pauseOrResumeAIOnScene (slowDownActive, pauseCharacterPriority);
			}
		}
	}
}
