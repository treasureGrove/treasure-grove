using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimatorIKComponent : MonoBehaviour
{
	public bool updateIKEnabled = true;

	public virtual void updateOnAnimatorIKState ()
	{
		if (!updateIKEnabled) {
			return;
		}
	}

	public void setUpdateIKEnabledState (bool state)
	{
		updateIKEnabled = state;
	}

	public virtual void setActiveState (bool state)
	{

	}
}
