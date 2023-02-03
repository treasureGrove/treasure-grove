using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialActivatorSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool tutorialEnabled = true;

	public string tutorialName;

	public string extraString = "Controls";

	GameObject currentPlayer;


	public void activateTutorial (GameObject newPlayer)
	{
		if (!tutorialEnabled) {
			return;
		}
			
		playerTutorialSystem currentPlayerTutorialSystem = FindObjectOfType<playerTutorialSystem> ();

		if (currentPlayerTutorialSystem != null) {
			string tutorialNameToActivate = tutorialName;

			if (extraString != "") {
				tutorialNameToActivate += " " + extraString;
			}
						
			currentPlayerTutorialSystem.activateTutorialByName (tutorialNameToActivate);
		} else {
			print ("WARNING: no tutorial system has been found in the scene, make sure to drop the tutorial system prefab into it");
		}
	}

	public void setTutorialEnabledState (bool state)
	{
		tutorialEnabled = state;
	}
}
