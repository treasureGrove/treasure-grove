using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hiddenMeleeSurface : MonoBehaviour
{
	public bool hiddenSurfaceEnabled = true;

	public meleeAttackSurfaceInfo mainMeleeAttackSurfaceInfo;

	public bool isSurfaceEnabled ()
	{
		return hiddenSurfaceEnabled;
	}

	public void setHiddenSurfaceEnabledState (bool state)
	{
		hiddenSurfaceEnabled = state;
	}
}
