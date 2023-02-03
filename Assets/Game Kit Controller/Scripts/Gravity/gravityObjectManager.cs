using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityObjectManager : MonoBehaviour
{
	public virtual void onGroundOrOnAir (bool state)
	{

	}

	public virtual bool isSearchingSurface ()
	{
		return false;
	}

	public virtual Vector3 getCurrentNormal ()
	{
		return Vector3.zero;
	}

	public virtual Transform getGravityCenter ()
	{
		return null;
	}

	public virtual bool isPlayerSearchingGravitySurface ()
	{
		return false;
	}
}
