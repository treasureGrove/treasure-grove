using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remotePlayerNavmeshOverrideSystem : MonoBehaviour
{
	public bool activateNavmesh;

	public remotePlayerNavmeshOverride mainRemotePlayerNavmeshOverride;

	public Transform navmeshTargetTransform;

	public void setRemoteNavmeshState ()
	{
		setRemoteNavmeshStateExternally (activateNavmesh);
	}

	public void setRemoteNavmeshStateExternally (bool state)
	{
		if (state) {
			activateRemoteNavmesh ();
		} else {
			disableRemoteNavmesh ();
		}
	}

	public void activateRemoteNavmesh ()
	{
		if (mainRemotePlayerNavmeshOverride == null) {
			mainRemotePlayerNavmeshOverride = FindObjectOfType<remotePlayerNavmeshOverride> ();
		}

		if (mainRemotePlayerNavmeshOverride != null) {
			mainRemotePlayerNavmeshOverride.setPlayerNavMeshEnabledState (true);

			mainRemotePlayerNavmeshOverride.setPlayerNavMeshTransformTargetPosition (navmeshTargetTransform);
		}
	}

	public void disableRemoteNavmesh ()
	{
		if (mainRemotePlayerNavmeshOverride == null) {
			mainRemotePlayerNavmeshOverride = FindObjectOfType<remotePlayerNavmeshOverride> ();
		}

		if (mainRemotePlayerNavmeshOverride != null) {
			mainRemotePlayerNavmeshOverride.setPlayerNavMeshEnabledState (false);
		}
	}
}
