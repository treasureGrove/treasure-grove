using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputPanelUISystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public playerInputPanelSystem mainPlayerInputPanelSystem;

	public GameObject inputPanelGameObject;

	public GameObject screenActionParent;

	[Header ("Screen Action Panel Settings")]
	[Space]

	public List<screenActionInfo> screenActionInfoList = new List<screenActionInfo> ();


	public void setInputPanelGameObjectActiveState (bool state)
	{
		inputPanelGameObject.SetActive (state);
	}

	public void setMainPlayerInputPanelSystem (playerInputPanelSystem newplayerInputPanelSystem)
	{
		mainPlayerInputPanelSystem = newplayerInputPanelSystem;

		updateComponent ();
	}

	public void searchPlayerInputPanelSystem (playerInputPanelSystem newPlayerInputPanelSystem)
	{
		mainPlayerInputPanelSystem = newPlayerInputPanelSystem;

		if (mainPlayerInputPanelSystem == null) {
			mainPlayerInputPanelSystem = FindObjectOfType<playerInputPanelSystem> ();
		}

		if (mainPlayerInputPanelSystem != null) {
			mainPlayerInputPanelSystem.setInputPanelUISystem (this);
		}

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}
