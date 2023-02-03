using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addGrenadeToThrowAwayFromCharacter : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkGrenadeOnCharacterEnabled = true;

	public string throwGrenadeAbilityName = "Throw Grenade";

	public string messagePanelName = "Return Grenade";

	public bool setThrowGrenadeAbilityAutomatically = true;

	public projectileSystem mainProjectileSystem;

	[Space]
	[Header ("Debug")]
	[Space]

	public GameObject currentPlayer;

	public bool confirmThrowGrenadeActive;


	public void addGrenadeToCharacterSystem (GameObject playerToCheck)
	{
		if (!checkGrenadeOnCharacterEnabled) {
			return;
		}

		currentPlayer = playerToCheck;

		if (currentPlayer == null) {
			return;
		}

		checkPlayer (true);
	}

	public void removeGrenadeToCharacterSystem ()
	{
		if (!checkGrenadeOnCharacterEnabled) {
			return;
		}

		if (currentPlayer == null) {
			return;
		}

		checkPlayer (false);
	}

	void checkPlayer (bool addingGrenade)
	{
		playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {

			playerAbilitiesSystem currentPlayerAbilitiesSystem = currentPlayerComponentsManager.getPlayerAbilitiesSystem ();

			if (currentPlayerAbilitiesSystem != null) {
				abilityInfo throwGrenadeAbility = currentPlayerAbilitiesSystem.getAbilityByName (throwGrenadeAbilityName);

				if (throwGrenadeAbility != null) {
					getObjectFromInventorySystem currentGetObjectFromInventorySystem = throwGrenadeAbility.GetComponentInChildren<getObjectFromInventorySystem> ();

					if (currentGetObjectFromInventorySystem != null) {
						if (addingGrenade) {
							currentGetObjectFromInventorySystem.addExtraInventoryObjectAmount (1);
						
							if (setThrowGrenadeAbilityAutomatically) {
								playerAbilitiesUISystem currentPlayerAbilitiesUISystem = currentPlayerAbilitiesSystem.GetComponent<playerAbilitiesUISystem> ();

								if (currentPlayerAbilitiesUISystem != null) {
									currentPlayerAbilitiesUISystem.selectAbilityByName (throwGrenadeAbilityName);
								}
							}

							simpleWeaponSystem currentSimpleWeaponSystem = throwGrenadeAbility.GetComponentInChildren<simpleWeaponSystem> ();

							if (currentSimpleWeaponSystem != null) {
								currentSimpleWeaponSystem.addExternalProjectileToFire (mainProjectileSystem.gameObject);
							}

							showMessageOnHUDSystem currentShowMessageOnHUDSystem = throwGrenadeAbility.GetComponentInChildren<showMessageOnHUDSystem> ();

							if (currentShowMessageOnHUDSystem != null) {
								currentShowMessageOnHUDSystem.showMessagePanel (messagePanelName);
							}
						} else {
							showMessageOnHUDSystem currentShowMessageOnHUDSystem = throwGrenadeAbility.GetComponentInChildren<showMessageOnHUDSystem> ();

							if (currentShowMessageOnHUDSystem != null) {
								currentShowMessageOnHUDSystem.hideMessagePanel (messagePanelName);
							}

							currentGetObjectFromInventorySystem.addExtraInventoryObjectAmount (-1);

							simpleWeaponSystem currentSimpleWeaponSystem = throwGrenadeAbility.GetComponentInChildren<simpleWeaponSystem> ();

							if (currentSimpleWeaponSystem != null) {
								currentSimpleWeaponSystem.removeExternalProjectileToFire (mainProjectileSystem.gameObject);
							}

							currentPlayer = null;

							if (!confirmThrowGrenadeActive) {
								currentPlayerAbilitiesSystem.cancelPressAbility ();
							}

							confirmThrowGrenadeActive = false;
						}
					}
				}
			}
		}
	}

	public void confirmThrowAwayGrenade ()
	{
		confirmThrowGrenadeActive = true;

		mainProjectileSystem.projectileInitialized = false;

		mainProjectileSystem.setProjectileUsedState (false);

		if (mainProjectileSystem.currentProjectileInfo != null) {
			GameObject previousOwner = mainProjectileSystem.currentProjectileInfo.owner;

			if (previousOwner != null) {
				Collider previousOwnerCollider = previousOwner.GetComponent<Collider> ();

				if (previousOwnerCollider != null) {
					Physics.IgnoreCollision (mainProjectileSystem.mainCollider, previousOwnerCollider, false);
				}
			}
		}
	}
}
