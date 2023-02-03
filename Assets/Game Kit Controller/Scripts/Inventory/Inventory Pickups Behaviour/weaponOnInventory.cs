using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponOnInventory : objectOnInventory
{
	[Header ("Custom Settings")]
	[Space]

	public weaponPickup mainWeaponPickup;

	public int remainingAmmo = -1;

	string weaponName;

	public override void eventOnPickObject (GameObject currentPlayer)
	{
		if (remainingAmmo > -1) {
			weaponName = mainWeaponPickup.weaponName;

			playerWeaponsManager weaponsManager = currentPlayer.GetComponent<playerWeaponsManager> ();

			playerWeaponSystem currrentPlayerWeaponSystem = weaponsManager.getWeaponSystemByName (weaponName);

			currrentPlayerWeaponSystem.setCurrentProjectilesInMagazine (remainingAmmo);
		}
	}

	public override void eventOnDropObject (GameObject currentPlayer)
	{
		weaponName = mainWeaponPickup.weaponName;

		playerWeaponsManager weaponsManager = currentPlayer.GetComponent<playerWeaponsManager> ();

		playerWeaponSystem currrentPlayerWeaponSystem = weaponsManager.getWeaponSystemByName (weaponName);

		remainingAmmo = currrentPlayerWeaponSystem.getProjectilesInMagazine ();
	}

	public override bool setObjectEquippedStateOnInventory (GameObject currentPlayer, bool state)
	{
		
		return false;
	}
}