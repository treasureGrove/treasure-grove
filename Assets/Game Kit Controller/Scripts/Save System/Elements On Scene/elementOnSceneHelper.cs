using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elementOnSceneHelper : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkElementOnSceneEnabled = true;

	public elementOnScene mainElementOnScene;


	public void setElementActiveStateToMainElementOnSceneManager (bool state)
	{
		if (!checkElementOnSceneEnabled) {
			return;
		}

		if (mainElementOnScene != null) {
			mainElementOnScene.setElementActiveStateToMainElementOnSceneManager (state);
		}
	}

	public void setNewInstantiatedElementOnSceneManagerIngameWithInfo ()
	{
		if (!checkElementOnSceneEnabled) {
			return;
		}

		if (mainElementOnScene != null) {
			mainElementOnScene.setNewInstantiatedElementOnSceneManagerIngameWithInfo ();
		}
	}

	public void setElementActiveState (bool state)
	{
		if (!checkElementOnSceneEnabled) {
			return;
		}

		if (mainElementOnScene != null) {
			mainElementOnScene.setElementActiveState (state);
		}
	}

	public int getElementScene ()
	{
		if (!checkElementOnSceneEnabled) {
			return -1;
		}

		return mainElementOnScene.elementScene;
	}

	public int getElementID ()
	{
		if (!checkElementOnSceneEnabled) {
			return -1;
		}

		return mainElementOnScene.elementID;
	}

	public void setStatsSearchingByInfo (int currentElementScene, int currentElementID)
	{
		if (!checkElementOnSceneEnabled) {
			return;
		}

		mainElementOnScene.setStatsSearchingByInfo (currentElementScene, currentElementID);
	}
}
