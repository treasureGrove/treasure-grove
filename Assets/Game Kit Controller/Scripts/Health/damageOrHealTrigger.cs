using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class damageOrHealTrigger : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useWithPlayer;
	public bool useWithVehicles;
	public bool useWithCharacters;

	public List<string> tagToAffectList = new List<string> ();

	[Space]
	[Header ("Damage-Health Settinga")]
	[Space]

	public bool addAmountEnabled;
	public bool removeAmountEnabled;

	public bool canHeal;
	public float healRate;
	public float healtAmount;

	public bool canRefillEnergy;
	public float energyRate;
	public float energyAmount;

	public bool canRefillFuel;
	public float fuelRate;
	public float fuelAmount;

	[Space]
	[Header ("Other Settinga")]
	[Space]

	public bool ignoreShield;
	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	public int damageTypeID = -1;

	public bool applyValueAtOnce;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject player;
	public GameObject objectWithHealth;

	bool objectInside;
	float lastTime;
	float valueToAdd;

	void Update ()
	{
		//if an object which can be damaged is inside the trigger, then
		if (objectInside && objectWithHealth) {
			if (addAmountEnabled) {
				//if the trigger heals
				if (canHeal) {
					if (Time.time > lastTime + healRate) {
						//while the object is not fully healed, then 
						if (!applyDamage.checkIfMaxHealth (objectWithHealth)) {

							valueToAdd = healtAmount;

							if (applyValueAtOnce) {
								valueToAdd = applyDamage.getMaxHealthAmount (objectWithHealth);
							}

							//heal it
							applyDamage.setHeal (valueToAdd, objectWithHealth);

							lastTime = Time.time;
						} else {
							//else, stop healing it
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}

				if (canRefillFuel) {
					//if the trigger recharges fuel
					if (Time.time > lastTime + fuelRate) {
						//while the vehicle has not the max fuel amount, then
						if (!applyDamage.checkIfMaxFuel (objectWithHealth)) {
							valueToAdd = fuelAmount;

							if (applyValueAtOnce) {
								valueToAdd = applyDamage.getMaxFuelAmount (objectWithHealth);
							}

							//fill it
							applyDamage.setFuel (valueToAdd, objectWithHealth);

							lastTime = Time.time;
						} else {
							//else, stop refill it
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}

				if (canRefillEnergy) {
					//if the trigger recharges energy
					if (Time.time > lastTime + energyRate) {
						//while the vehicle has not the max fuel amount, then
						if (!applyDamage.checkIfMaxEnergy (objectWithHealth)) {

							valueToAdd = energyAmount;

							if (applyValueAtOnce) {
								valueToAdd = applyDamage.getMaxEnergyAmount (objectWithHealth);
							}

							//fill it
							applyDamage.setEnergy (valueToAdd, objectWithHealth);

							lastTime = Time.time;
						} else {
							//else, stop refill it
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}
			} 

			if (removeAmountEnabled) {
				if (canHeal) {
					//apply damage or heal it accordint to the time rate
					if (Time.time > lastTime + healRate) {
						//if the trigger damages
						valueToAdd = healtAmount;

						if (applyValueAtOnce) {
							valueToAdd = applyDamage.getCurrentHealthAmount (objectWithHealth);
						}

						//apply damage
						applyDamage.checkHealth (gameObject, objectWithHealth, valueToAdd, Vector3.zero, objectWithHealth.transform.position + objectWithHealth.transform.up, 
							gameObject, true, true, ignoreShield, false, canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
						
						lastTime = Time.time;

						//if the object inside the trigger is dead, stop applying damage
						if (applyDamage.checkIfDead (objectWithHealth)) {
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}

				if (canRefillFuel) {
					if (Time.time > lastTime + fuelRate) {
						//while the vehicle has not the max fuel amount, then
						if (applyDamage.getCurrentFuelAmount (objectWithHealth) > 0) {
							valueToAdd = fuelAmount;

							if (applyValueAtOnce) {
								valueToAdd = applyDamage.getMaxFuelAmount (objectWithHealth);
							}

							//fill it
							applyDamage.removeFuel (valueToAdd, objectWithHealth);

							lastTime = Time.time;
						} else {
							//else, stop refill it
							changeTriggerState (false, null, 0);

							return;
						}
					}
				}

				if (Time.time > lastTime + energyRate) {
					if (canRefillEnergy) {
						//while the vehicle has not the max fuel amount, then
						if (applyDamage.getCurrentEnergyAmount (objectWithHealth) > 0) {
							valueToAdd = energyAmount;

							if (applyValueAtOnce) {
								valueToAdd = applyDamage.getMaxEnergyAmount (objectWithHealth);
							}

							//fill it
							applyDamage.removeEnergy (valueToAdd, objectWithHealth);

							lastTime = Time.time;
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

	void OnTriggerEnter (Collider col)
	{
		//if the player enters the trigger and it can used with him, then
		if (col.gameObject.CompareTag ("Player") && useWithPlayer) {
			player = col.gameObject;

			//if he is not driving, apply damage or heal
			if (!player.GetComponent<playerController> ().driving) {
				changeTriggerState (true, player, Time.time);
			}
		} 

		//else, if a vehicle is inside the trigger and it can be used with vehicles, them
		else if (isInTagToAffectList (col.gameObject) && col.gameObject.GetComponent<vehicleHUDManager> () && useWithVehicles) {
			changeTriggerState (true, col.gameObject, Time.time);
		} else if (isInTagToAffectList (col.gameObject) && col.gameObject.GetComponent<AINavMesh> () && useWithCharacters) {
			changeTriggerState (true, col.gameObject, Time.time);
		}
	}

	void OnTriggerExit (Collider col)
	{
		//if the player or a vehicle exits, stop the healing or the damaging
		if (col.gameObject.CompareTag ("Player") && useWithPlayer) {
			changeTriggerState (false, null, 0);
		} else if (isInTagToAffectList (col.gameObject) && col.gameObject.GetComponent<vehicleHUDManager> () && useWithVehicles) {
			changeTriggerState (false, null, 0);
		} else if (isInTagToAffectList (col.gameObject) && col.gameObject.GetComponent<AINavMesh> () && useWithCharacters) {
			changeTriggerState (false, null, 0);
		}
	}

	//stop or start the heal or damage action
	void changeTriggerState (bool inside, GameObject obj, float time)
	{
		objectInside = inside;
		objectWithHealth = obj;
		lastTime = time;
	}

	public bool isInTagToAffectList (GameObject objectToCheck)
	{
		if (tagToAffectList.Contains (objectToCheck.tag)) {
			return true;
		}

		return false;
	}
}