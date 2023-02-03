using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderControllerInterface : MonoBehaviour
{
	public virtual void setGroundcheck (bool b)
	{

	}

	public virtual void walk (Vector3 direction)
	{

	}

	public virtual void run (Vector3 direction)
	{

	}

	public virtual void turn (Vector3 goalForward)
	{

	}

	public virtual Vector3 getGroundNormal ()
	{
		return Vector3.zero;
	}

	public virtual Vector3 getCamTargetPosition ()
	{
		return Vector3.zero;
	}

	public virtual Quaternion getCamTargetRotation ()
	{
		return Quaternion.identity;
	}

	public virtual void setTargetPosition (Vector3 pos)
	{
        
	}

	public virtual void setTargetRotation (Quaternion rot)
	{
     
	}

	public virtual Transform getCameraTarget ()
	{
		return null;
	}
}
