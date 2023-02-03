using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainRiderSystem : MonoBehaviour
{
	[Header ("Main Rider Settings")]
	[Space]

	public bool isExternalVehicleController;

	public virtual GameObject getVehicleCameraGameObject ()
	{
		return null;
	}

	public virtual vehicleGravityControl getVehicleGravityControl ()
	{
		return null;
	}

	public virtual GameObject getVehicleGameObject ()
	{
		return null;
	}

	public virtual void setTriggerToDetect (Collider newCollider)
	{
		
	}

	public virtual void setPlayerVisibleInVehicleState (bool state)
	{
		
	}

	public virtual void setResetCameraRotationWhenGetOnState (bool state)
	{
		
	}

	public virtual void setEjectPlayerWhenDestroyedState (bool state)
	{
		
	}

	public virtual Transform getCustomVehicleTransform ()
	{
		return null;
	}
}
