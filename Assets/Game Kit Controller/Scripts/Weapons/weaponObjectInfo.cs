using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponObjectInfo : MonoBehaviour
{
	//REGULAR WEAPON ELEMENTS
	public virtual string getWeaponName ()
	{

		return "";
	}


	//MELEE WEAPON ELEMENTS
	public virtual bool isMeleeWeapon ()
	{

		return false;
	}


	//FIRE WEAPON ELEMENTS
	public virtual bool isFireWeapon ()
	{

		return false;
	}

	public virtual string getAmmoText ()
	{
		
		return "";
	}

	public virtual int getProjectilesInWeaponMagazine ()
	{
		
		return -1;
	}

	public virtual string getWeaponAmmoName ()
	{
		
		return "";
	}

	public virtual void setWeaponRemainAmmoAmount (int newRemainAmmoAmount)
	{
		
	}

	public virtual bool isWeaponUseRemainAmmoFromInventoryActive ()
	{
		
		return false;
	}
}
