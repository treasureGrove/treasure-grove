using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healerWeapon : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask layer;

	public bool canHeal;
	public float healRate;
	public float healtAmount;

	public bool canDamage;
	public float damageRate;
	public float damageAmount;

	public bool canRefillEnergy;
	public float energyRate;
	public float energyAmount;

	public bool canRefillFuel;
	public float fuelRate;
	public float fuelAmount;

	public bool canGatherHealth;
	public bool canGatherEnergy;
	public bool canGatherFuel;

	public bool useWithVehicles;
	public bool useWithCharacters;

	public bool searchinTargets;

	public bool healModeActive;

	public bool ignoreShield;

	public int damageTypeID = -1;

	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	[Space]
	[Header ("Debug")]
	[Space]

	public GameObject currentObjectToCheck;
	public GameObject currentHealthElement;
	public bool targetFound;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform mainCameraTransform;
	public playerWeaponSystem weaponManager;

	float lastTime;
	RaycastHit hit;

	void Update ()
	{
		if (searchinTargets) {
			if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.TransformDirection (Vector3.forward), out hit, Mathf.Infinity, layer)) {
				currentObjectToCheck = hit.collider.gameObject;
				if (!targetFound) {
					checkTarget (currentObjectToCheck);
				} else {
					if (currentHealthElement != currentObjectToCheck) {
						changeTriggerState (false, null, 0);
					}
				}
			} else {
				targetFound = false;
			}
		}

		if (targetFound && currentHealthElement) {
			if (healModeActive) {
				if (canHeal) {
					if (Time.time > lastTime + healRate) {
						//while the object is not fully healed, then 
						if (!applyDamage.checkIfMaxHealth (currentHealthElement) && weaponManager.remainAmmoInClip ()) {
							//heal it
							applyDamage.setHeal (healtAmount, currentHealthElement);

							lastTime = Time.time;

							if (canGatherHealth) {
								int amountOfAmmo = (int)Mathf.Round (healtAmount);

								weaponManager.useAmmo (amountOfAmmo);

								weaponManager.checkToUpdateInventoryWeaponAmmoTextFromWeaponSystem ();
							}
						} else {
							//else, stop healing it
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}

				if (canRefillFuel) {
					if (Time.time > lastTime + fuelRate) {
						//while the vehicle has not the max fuel amount, then
						if (!applyDamage.checkIfMaxFuel (currentHealthElement) && weaponManager.remainAmmoInClip ()) {
							//fill it
							applyDamage.setFuel (fuelAmount, currentHealthElement);

							lastTime = Time.time;

							if (canGatherFuel) {
								int amountOfAmmo = (int)Mathf.Round (fuelAmount);

								weaponManager.useAmmo (amountOfAmmo);

								weaponManager.checkToUpdateInventoryWeaponAmmoTextFromWeaponSystem ();
							}
						} else {
							//else, stop refill it
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}

				if (canRefillEnergy) {
					if (Time.time > lastTime + energyRate) {
						//while the vehicle has not the max fuel amount, then
						if (!applyDamage.checkIfMaxEnergy (currentHealthElement) && weaponManager.remainAmmoInClip ()) {
							//fill it
							applyDamage.setEnergy (energyAmount, currentHealthElement);

							lastTime = Time.time;

							if (canGatherEnergy) {
								int amountOfAmmo = (int)Mathf.Round (energyAmount);

								weaponManager.useAmmo (amountOfAmmo);

								weaponManager.checkToUpdateInventoryWeaponAmmoTextFromWeaponSystem ();
							}
						} else {
							//else, stop refill it
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}
			} else {
				//apply damage or heal it accordint to the time rate
				if (Time.time > lastTime + damageRate) {
					if (canDamage) {
						//if the trigger damages
						//apply damage
						applyDamage.checkHealth (gameObject, currentHealthElement, damageAmount, Vector3.zero, 
							currentHealthElement.transform.position + currentHealthElement.transform.up, gameObject, 
							true, true, ignoreShield, false, canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
						
						lastTime = Time.time;

						if (canGatherHealth) {
							weaponManager.getAndUpdateAmmo ((int)Mathf.Round (damageAmount));
						}

						//if the object inside the trigger is dead, stop applying damage
						if (applyDamage.checkIfDead (currentHealthElement)) {
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}

				if (canRefillFuel) {
					if (Time.time > lastTime + fuelRate) {
						//while the vehicle has not the max fuel amount, then
						if (applyDamage.getCurrentFuelAmount (currentHealthElement) > 0) {
							//fill it
							applyDamage.removeFuel (fuelAmount, currentHealthElement);

							lastTime = Time.time;

							if (canGatherFuel) {
								weaponManager.getAndUpdateAmmo ((int)Mathf.Round (fuelAmount));
							}
						} else {
							//else, stop refill it
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}

				if (canRefillEnergy) {
					if (Time.time > lastTime + energyRate) {
						//while the vehicle has not the max fuel amount, then
						if (applyDamage.getCurrentEnergyAmount (currentHealthElement) > 0) {
							//fill it
							applyDamage.removeEnergy (energyAmount, currentHealthElement);

							lastTime = Time.time;

							if (canGatherEnergy) {
								weaponManager.getAndUpdateAmmo ((int)Mathf.Round (energyAmount));
							}
						} else {
							//else, stop refill it
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}
			}
		}
	}

	public void changeMode ()
	{
		initializeComponents ();

		healModeActive = !healModeActive;
	}

	public void checkTarget (GameObject target)
	{
		//else, if a vehicle is inside the trigger and it can be used with vehicles, them
		if (applyDamage.isVehicle (target) && useWithVehicles) {
			changeTriggerState (true, target, Time.time);

			return;
		} 

		if (applyDamage.isCharacter (target) && useWithCharacters) {
			changeTriggerState (true, target, Time.time);

			return;
		}
	}

	//stop or start the heal or damage action
	void changeTriggerState (bool inside, GameObject obj, float time)
	{
		targetFound = inside;

		currentHealthElement = obj;

		lastTime = time;
	}

	public void downButtonAction ()
	{
		initializeComponents ();

		searchinTargets = true;
	}

	public void upButtonAction ()
	{
		initializeComponents ();

		searchinTargets = false;

		changeTriggerState (false, null, 0);
	}

	bool componentsInitialized;

	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}

		if (weaponManager != null) {
			mainCameraTransform = weaponManager.getMainCameraTransform ();
		}

		componentsInitialized = true;
	}
}
