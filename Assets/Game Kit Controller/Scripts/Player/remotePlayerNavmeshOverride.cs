using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remotePlayerNavmeshOverride : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public playerNavMeshSystem mainPlayerNavMeshSystem;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool playerNavMeshActive;

	public void setPlayerNavMeshTransformTargetPosition (Transform newTransformPosition)
	{
		mainPlayerNavMeshSystem.checkRaycastPositionWithVector3 (newTransformPosition.position);
	}

	public void setPlayerNavMeshTargetPosition ()
	{
		mainPlayerNavMeshSystem.checkRaycastPositionWithVector3 (transform.position);
	}

	public void enablePlayerNavMeshState ()
	{
		setPlayerNavMeshEnabledState (true);
	}

	public void disablePlayerNavMeshState ()
	{
		setPlayerNavMeshEnabledState (false);
	}

	public void setPlayerNavMeshEnabledState (bool state)
	{
		mainPlayerNavMeshSystem.setUsingPlayerNavmeshExternallyState (state);

		mainPlayerNavMeshSystem.setPlayerNavMeshEnabledState (state);

		playerNavMeshActive = state;
	}
}
