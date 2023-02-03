using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setGameObjectActiveState : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool setInitialState;
	public bool initialState;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool objectActive;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject objectToActive;


	void Start ()
	{
		if (objectToActive == null) {
			objectToActive = gameObject;
		}
			
		if (objectToActive.activeSelf) {
			objectActive = true;
		}

		if (setInitialState) {
			setActiveState (initialState);
		}
	}

	public void changeActiveState ()
	{
		objectActive = !objectActive;

		setActiveState (objectActive);
	}

	public void setActiveState (bool state)
	{
		objectActive = state;

		if (objectToActive.activeSelf != state) {
			objectToActive.SetActive (state);
		}
	}
}
