using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using GKC.Localization;

public class elementsOnSceneSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool saveCurrentPlayerElementsOnSceneToSaveFile;

	public string mainElementsOnSceneManagerName = "Elements On Scene Manager";

	[Space]
	[Header ("Debug")]
	[Space]

	public elementOnSceneManager mainElementOnSceneManager;

	public void initializeValues ()
	{
		//Load the missions saved previously with those missions found by the player or activated in some way, setting their state or complete or not complete
		if (!saveCurrentPlayerElementsOnSceneToSaveFile) {
			return;
		}

		//Search for an objectives manager in the level, if no one is present, add one
		if (mainElementOnSceneManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainElementsOnSceneManagerName, typeof(elementOnSceneManager));

			mainElementOnSceneManager = FindObjectOfType<elementOnSceneManager> ();
		}
	}
}
