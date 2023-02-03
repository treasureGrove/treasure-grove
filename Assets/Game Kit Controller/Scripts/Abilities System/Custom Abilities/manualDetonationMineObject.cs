using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class manualDetonationMineObject : MonoBehaviour
{
	public UnityEvent eventToActivateMine;

	manualDetonationMineSystem mainManualDetonationMineSystem;

	public void activateMine ()
	{
		eventToActivateMine.Invoke ();
	}

	public void removeMine ()
	{
		if (mainManualDetonationMineSystem != null) {
			mainManualDetonationMineSystem.removeCurrentMine ();
		}
	}

	public void setManualDetonationMineSystem (manualDetonationMineSystem newManualDetonationMineSystem)
	{
		mainManualDetonationMineSystem = newManualDetonationMineSystem;
	}
}
