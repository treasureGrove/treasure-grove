using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryObjectToEquipInfo : MonoBehaviour
{
	public virtual bool setObjectEquippedState (bool state)
	{

		return false;
	}

	public virtual bool isObjectEquipped ()
	{
		
		return false;
	}

	public virtual void updateObjectState ()
	{

	}
}
