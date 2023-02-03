using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class applyDamage : MonoBehaviour
{
	//check if the collided object has a health component, and apply damage to it
	//any object with health component will be damaged, so the friendly fire is allowed
	//also, check if the object is a vehicle to apply damage too
	public static void checkHealth (GameObject projectile, GameObject objectToDamage, float damageAmount, Vector3 direction, Vector3 position, GameObject projectileOwner, 
	                                bool damageConstant, bool searchClosestWeakSpot, bool ignoreShield, bool ignoreDamageInScreen, bool canActivateReactionSystemTemporally,
	                                int damageReactionID, int damageTypeID)
	{
		healthManagement currentHealthManagement = objectToDamage.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.setDamageWithHealthManagement (damageAmount, direction, position, projectileOwner, projectile, damageConstant, 
				searchClosestWeakSpot, ignoreShield, ignoreDamageInScreen, true, canActivateReactionSystemTemporally, damageReactionID, damageTypeID);

			return;
		}
	}

	public static bool checkCanBeDamaged (GameObject projectile, GameObject objectToDamage, float damageAmount, Vector3 direction, Vector3 position, GameObject projectileOwner,
	                                      bool damageConstant, bool searchClosestWeakSpot, bool ignoreShield, bool ignoreDamageInScreen, bool damageCanBeBlocked, bool canActivateReactionSystemTemporally,
	                                      int damageReactionID, int damageTypeID)
	{
		healthManagement currentHealthManagement = objectToDamage.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.setDamageWithHealthManagement (damageAmount, direction, position, projectileOwner, projectile, 
				damageConstant, searchClosestWeakSpot, ignoreShield, ignoreDamageInScreen, damageCanBeBlocked, canActivateReactionSystemTemporally,
				damageReactionID, damageTypeID);

			return true;
		}

		return false;
	}

	public static void checkToDamageGKCCharacterExternally (float damageAmount, GameObject characterToDamage, GameObject attacker)
	{
		healthManagement currentHealthManagement = characterToDamage.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.setDamageWithHealthManagement (damageAmount, Vector3.forward, characterToDamage.transform.position, attacker, 
				null, false, true, false, false, true, false, -1, -1);
		}
	}

	public static bool checkIfDead (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.checkIfDeadWithHealthManagement ();
		}

		return false;
	}

	public static bool checkIfDeadOnObjectChilds (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponentInChildren<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.checkIfDeadWithHealthManagement ();
		}

		return false;
	}

	public static bool checkIfMaxHealth (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.checkIfMaxHealthWithHealthManagement ();
		}

		return false;
	}

	public static void setDamageTargetOverTimeState (GameObject objectToDamage, float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, 
	                                                 float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{
		healthManagement currentHealthManagement = objectToDamage.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.setDamageTargetOverTimeStateWithHealthManagement (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath, damageTypeID);
		}
	}

	public static void removeDamagetTargetOverTimeState (GameObject objectToDamage)
	{
		healthManagement currentHealthManagement = objectToDamage.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.removeDamagetTargetOverTimeStateWithHealthManagement ();
		}
	}

	public static void sedateCharacter (GameObject objectToDamage, Vector3 position, float sedateDelay, bool useWeakSpotToReduceDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{
		healthManagement currentHealthManagement = objectToDamage.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.sedateCharacterithHealthManagement (position, sedateDelay, useWeakSpotToReduceDelay, sedateUntilReceiveDamage, sedateDuration);
		}
	}

	//manage health, energy and fuel
	public static void setHeal (float healAmount, GameObject objectToHeal)
	{
		healthManagement currentHealthManagement = objectToHeal.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.setHealWithHealthManagement (healAmount);
		}
	}

	public static void setEnergy (float energyAmount, GameObject objectToRecharge)
	{
		otherPowers otherPowersToCheck = objectToRecharge.GetComponent<otherPowers> ();

		if (otherPowersToCheck != null) {
			otherPowersToCheck.getEnergy (energyAmount);

			return;
		}

		healthManagement currentHealthManagement = objectToRecharge.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.setEnergyWithHealthManagement (energyAmount);

			return;
		}
	}

	public static void removeEnergy (float energyAmount, GameObject objectToRecharge)
	{
		otherPowers otherPowersToCheck = objectToRecharge.GetComponent<otherPowers> ();

		if (otherPowersToCheck != null) {
			otherPowersToCheck.removeEnergy (energyAmount);

			return;
		}

		healthManagement currentHealthManagement = objectToRecharge.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.removeEnergyWithHealthManagement (energyAmount);

			return;
		}
	}

	public static void setFuel (float fuelAmount, GameObject vehicle)
	{
		healthManagement currentHealthManagement = vehicle.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.setFuelWithHealthManagement (fuelAmount);
		}
	}

	public static void removeFuel (float fuelAmount, GameObject vehicle)
	{
		healthManagement currentHealthManagement = vehicle.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.removeFuelWithHealthManagement (fuelAmount);
		}
	}

	public static void setShield (float shieldAmount, GameObject objectToAddAmount)
	{
		healthManagement currentHealthManagement = objectToAddAmount.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.setShieldWithHealthManagement (shieldAmount);
		}
	}

	public static void setStamina (float staminaAmount, GameObject objectToAddAmount)
	{
		playerComponentsManager mainPlayerComponentsManager = objectToAddAmount.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			staminaSystem mainStaminaSystem = mainPlayerComponentsManager.getStaminaSystem ();

			if (mainStaminaSystem != null) {
				mainStaminaSystem.getStamina (staminaAmount);
				return;
			}
		}
	}

	public static void setOxygen (float oxygenAmount, GameObject objectToAddAmount)
	{
		playerComponentsManager mainPlayerComponentsManager = objectToAddAmount.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			oxygenSystem mainOxygenSystem = mainPlayerComponentsManager.getOxygenSystem ();

			if (mainOxygenSystem != null) {
				mainOxygenSystem.getOxygen (oxygenAmount);
				return;
			}
		}
	}

	public static void setMoney (float moneyAmount, GameObject objectToAddAmount, bool useMoneyRandomRange, Vector2 moneyRandomRange)
	{
		playerComponentsManager mainPlayerComponentsManager = objectToAddAmount.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			currencySystem mainCurrencySystem = mainPlayerComponentsManager.getCurrencySystem ();

			float newAmount = moneyAmount;

			if (useMoneyRandomRange) {
				newAmount = Random.Range (moneyRandomRange.x, moneyRandomRange.y);

				newAmount = Mathf.RoundToInt (newAmount);
			}

			mainCurrencySystem.increaseTotalMoneyAmount (newAmount);
		}
	}

	public static void setExperience (float experienceAmount, GameObject objectToAddAmount, Transform objectTransform, bool useExperienceRandomRange, Vector2 experienceRandomRange)
	{
		playerComponentsManager mainPlayerComponentsManager = objectToAddAmount.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			playerExperienceSystem mainPlayerExperienceSystem = mainPlayerComponentsManager.getPlayerExperienceSystem ();

			float newAmount = experienceAmount;

			if (useExperienceRandomRange) {
				newAmount = Random.Range (experienceRandomRange.x, experienceRandomRange.y);

				newAmount = Mathf.RoundToInt (newAmount);
			}

			mainPlayerExperienceSystem.getExperienceAmount ((int)newAmount, objectTransform, true, "");
		}
	}

	public static void setExperienceMultiplier (float experienceMultiplierAmount, GameObject objectToAddAmount, float experienceMultiplierDuration)
	{
		playerComponentsManager mainPlayerComponentsManager = objectToAddAmount.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			playerExperienceSystem mainPlayerExperienceSystem = mainPlayerComponentsManager.getPlayerExperienceSystem ();

			mainPlayerExperienceSystem.setExperienceMultiplier (experienceMultiplierAmount, experienceMultiplierDuration);
		}
	}

	public static void setSkillPoints (float skillPointsAmount, GameObject objectToAddAmount)
	{
		playerComponentsManager mainPlayerComponentsManager = objectToAddAmount.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			playerExperienceSystem mainPlayerExperienceSystem = mainPlayerComponentsManager.getPlayerExperienceSystem ();

			mainPlayerExperienceSystem.getSkillPoints ((int)skillPointsAmount);
		}
	}

	public static void increaseInventoryBagWeight (float weightToIncrease, GameObject objectToAddAmount)
	{
		playerComponentsManager mainPlayerComponentsManager = objectToAddAmount.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			inventoryWeightManager mainPlayerExperienceSystem = mainPlayerComponentsManager.getInventoryWeightManager ();

			if (mainPlayerExperienceSystem != null) {
				mainPlayerExperienceSystem.increaseInventoryBagWeight (weightToIncrease);
			}
		}
	}

	public static void increaseStrengthAmountAndUpdateStat (float strengthToIncrease, GameObject objectToAddAmount)
	{
		playerComponentsManager mainPlayerComponentsManager = objectToAddAmount.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			grabObjects mainGrabObjects = mainPlayerComponentsManager.getGrabObjects ();

			if (mainGrabObjects != null) {
				mainGrabObjects.increaseStrengthAmountAndUpdateStat (strengthToIncrease);
			}
		}
	}

	public static bool canApplyForce (GameObject objectToCheck)
	{
		bool canReceiveForce = false;
		Rigidbody RigidbodyToCheck = objectToCheck.GetComponent<Rigidbody> ();

		if (RigidbodyToCheck != null) {
			if (!RigidbodyToCheck.isKinematic) {
				canReceiveForce = true;

				characterDamageReceiver damageReceiver = objectToCheck.GetComponent<characterDamageReceiver> ();

				if (damageReceiver != null) {
					if (damageReceiver.character.CompareTag ("Player") ||
					    damageReceiver.character.CompareTag ("enemy") ||
					    damageReceiver.character.CompareTag ("friend")) {
						canReceiveForce = false;
					}
				}
			}
		}

		return canReceiveForce;
	}

	public static bool isRagdollActive (GameObject objectToCheck)
	{
		characterDamageReceiver damageReceiver = objectToCheck.GetComponent<characterDamageReceiver> ();

		if (damageReceiver != null) {
			if (damageReceiver.characterIsOnRagdollState ()) {
				return true;
			}
		}

		return false;
	}

	public static Rigidbody applyForce (GameObject objectToCheck)
	{
		Rigidbody RigidbodyToCheck = objectToCheck.GetComponent<Rigidbody> ();

		if (RigidbodyToCheck != null) {
			if (!RigidbodyToCheck.isKinematic) {
				characterDamageReceiver damageReceiver = objectToCheck.GetComponent<characterDamageReceiver> ();

				if (damageReceiver != null) {
					if (damageReceiver.isCharacterDead ()) {
						return RigidbodyToCheck;
					} else if (!damageReceiver.character.CompareTag ("Player")) {

						if (!damageReceiver.character.CompareTag ("enemy") && !damageReceiver.character.CompareTag ("friend")) {
							return damageReceiver.character.GetComponent<Rigidbody> ();
						} else {
							if (damageReceiver.characterIsOnRagdollState ()) {
								return RigidbodyToCheck;
							}
						}
					}
				} else {
					vehicleDamageReceiver vehicleDamage = objectToCheck.GetComponent<vehicleDamageReceiver> ();

					if (vehicleDamage != null) {
						Rigidbody vehicleRigidbody = vehicleDamage.vehicle.GetComponent<Rigidbody> ();

						if (vehicleRigidbody != null) {
							if (!vehicleRigidbody.isKinematic) {
								return vehicleRigidbody;
							}
						}
					} else {
						vehicleHUDManager vehicleHUDManagerToCheck = objectToCheck.GetComponent<vehicleHUDManager> ();

						if (vehicleHUDManagerToCheck != null) {
							Rigidbody vehicleRigidbody = objectToCheck.GetComponent<Rigidbody> ();

							if (vehicleRigidbody != null) {
								if (!vehicleRigidbody.isKinematic) {
									return vehicleRigidbody;
								}
							}
						} else {
							return RigidbodyToCheck;
						}
					}
				}
			}
		} else {
			vehicleDamageReceiver vehicleDamage = objectToCheck.GetComponent<vehicleDamageReceiver> ();

			if (vehicleDamage != null) {
				Rigidbody vehicleRigidbody = vehicleDamage.vehicle.GetComponent<Rigidbody> ();

				if (vehicleRigidbody != null) {
					if (!vehicleRigidbody.isKinematic) {
						return vehicleRigidbody;
					}
				}
			} else {
				vehicleHUDManager vehicleHUDManagerToCheck = objectToCheck.GetComponent<vehicleHUDManager> ();

				if (vehicleHUDManagerToCheck != null) {
					Rigidbody vehicleRigidbody = vehicleHUDManagerToCheck.gameObject.GetComponent<Rigidbody> ();

					if (vehicleRigidbody != null) {
						if (!vehicleRigidbody.isKinematic) {
							return vehicleRigidbody;
						}
					}
				}
			}
		}

		return null;
	}

	//healt management
	public static float getCurrentHealthAmount (GameObject character)
	{
		healthManagement currentHealthManagement = character.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getCurrentHealthAmountWithHealthManagement ();
		}

		return 0;
	}

	public static float getMaxHealthAmount (GameObject character)
	{
		healthManagement currentHealthManagement = character.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getMaxHealthAmountWithHealthManagement ();
		}

		return 0;
	}

	public static float getAuxHealthAmount (GameObject character)
	{
		healthManagement currentHealthManagement = character.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getAuxHealthAmountWithHealthManagement ();
		}
	
		return 0;
	}

	public static void addAuxHealthAmount (GameObject character, float amount)
	{
		healthManagement currentHealthManagement = character.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.addAuxHealthAmountWithHealthManagement (amount);
		}
	}

	public static float getHealthAmountToPick (GameObject character, float amount)
	{
		healthManagement currentHealthManagement = character.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getHealthAmountToPickWithHealthManagement (amount);
		}

		return 0;
	}

	public static float getShieldAmountToPick (GameObject character, float amount)
	{
		characterDamageReceiver characterDamageReceiverToCheck = character.GetComponent<characterDamageReceiver> ();

		if (characterDamageReceiverToCheck != null) {
			float totalAmmoAmountToAdd = 0;

			float amountToRefill = characterDamageReceiverToCheck.getShieldAmountToLimit ();

			if (amountToRefill > 0) {
				print ("amount to refill " + amountToRefill);

				totalAmmoAmountToAdd = amount;

				if (amountToRefill < amount) {
					totalAmmoAmountToAdd = amountToRefill;
				}

				print (totalAmmoAmountToAdd);

				characterDamageReceiverToCheck.addAuxShieldAmount (totalAmmoAmountToAdd);
			}

			return totalAmmoAmountToAdd;
		}

		return 0;
	}

	public static float getStaminaAmountToPick (GameObject character, float amount)
	{
		playerComponentsManager mainPlayerComponentsManager = character.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			staminaSystem mainStaminaSystem = mainPlayerComponentsManager.getStaminaSystem ();

			if (mainStaminaSystem != null) {
				float totalAmmoAmountToAdd = 0;

				float amountToRefill = mainStaminaSystem.getStaminaAmountToLimit ();

				if (amountToRefill > 0) {
					
					print ("amount to refill " + amountToRefill);

					totalAmmoAmountToAdd = amount;

					if (amountToRefill < amount) {
						totalAmmoAmountToAdd = amountToRefill;
					}

					print (totalAmmoAmountToAdd);

					mainStaminaSystem.addAuxStaminaAmount (totalAmmoAmountToAdd);
				}

				return totalAmmoAmountToAdd;
			}
		}

		return 0;
	}

	public static float getOxygenAmountToPick (GameObject character, float amount)
	{
		playerComponentsManager mainPlayerComponentsManager = character.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			oxygenSystem mainOxygenSystem = mainPlayerComponentsManager.getOxygenSystem ();

			if (mainOxygenSystem != null) {
				float totalAmmoAmountToAdd = 0;

				float amountToRefill = mainOxygenSystem.getOxygenAmountToLimit ();

				if (amountToRefill > 0) {
//					print ("amount to refill " + amountToRefill);

					totalAmmoAmountToAdd = amount;

					if (amountToRefill < amount) {
						totalAmmoAmountToAdd = amountToRefill;
					}

//					print (totalAmmoAmountToAdd);

					mainOxygenSystem.addAuxOxygenAmount (totalAmmoAmountToAdd);
				}

				return totalAmmoAmountToAdd;
			}
		}

		return 0;
	}

	public static void refillFullOxygen (GameObject character)
	{
		playerComponentsManager mainPlayerComponentsManager = character.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			oxygenSystem mainOxygenSystem = mainPlayerComponentsManager.getOxygenSystem ();

			if (mainOxygenSystem != null) {
				mainOxygenSystem.refillOxygen (0);
			}
		}
	}

	public static void killCharacter (GameObject projectile, GameObject objectToDamage, Vector3 direction, Vector3 position, GameObject attacker, bool damageConstant)
	{
		healthManagement currentHealthManagement = objectToDamage.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.killCharacterWithHealthManagement (projectile, direction, position, attacker, damageConstant);
		}
	}

	public static void killCharacter (GameObject objectToDamage)
	{
		healthManagement currentHealthManagement = objectToDamage.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			currentHealthManagement.killCharacterWithHealthManagement ();
		}
	}

	public static healthManagement getHealthManagement (GameObject objectToDamage)
	{
		return objectToDamage.GetComponent<healthManagement> ();
	}

	public static Transform getPlaceToShoot (GameObject objectToDamage)
	{
		healthManagement currentHealthManagement = objectToDamage.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getPlaceToShootWithHealthManagement ();
		}

		return null;
	}

	public static bool checkIfWeakSpotListContainsTransform (GameObject objectToCheck, Transform transformToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.checkIfWeakSpotListContainsTransformWithHealthManagement (transformToCheck);
		}

		return false;
	}

	public static GameObject getPlaceToShootGameObject (GameObject objectToDamage)
	{
		healthManagement currentHealthManagement = objectToDamage.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getPlaceToShootGameObjectWithHealthManagement ();
		}

		return null;
	}

	public static bool isCharacter (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.isCharacterWithHealthManagement ();
		}

		return false;
	}

	public static bool isVehicle (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.isVehicleWithHealthManagement ();
		} else {
			GKCSimpleRiderSystem mainGKCSimpleRiderSystem = objectToCheck.GetComponentInChildren<GKCSimpleRiderSystem> ();

			if (mainGKCSimpleRiderSystem == null) {
				GKCRiderSocketSystem currentGKCRiderSocketSystem = objectToCheck.GetComponentInChildren<GKCRiderSocketSystem> ();

				if (currentGKCRiderSocketSystem != null) {
					mainRiderSystem currentmainRiderSystem = currentGKCRiderSocketSystem.getMainRiderSystem ();

					mainGKCSimpleRiderSystem = currentmainRiderSystem.GetComponent<GKCSimpleRiderSystem> ();
				}
			}

			if (mainGKCSimpleRiderSystem != null) {
				if (mainGKCSimpleRiderSystem.isBeingDrivenActive ()) {
					return true;
				}
			}
		}

		return false;
	}

	public static bool objectCanBeDamaged (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return true;
		}

		return false;
	}

	public static float getCharacterHeight (GameObject objectToCheck)
	{
		playerController currentPlayerController = objectToCheck.GetComponent<playerController> ();

		if (currentPlayerController != null) {
			return currentPlayerController.getCharacterHeight ();
		}

		return -1;
	}

	public static playerController getPlayerControllerComponent (GameObject objectToCheck)
	{
		playerController currentPlayerController = objectToCheck.GetComponent<playerController> ();

		if (currentPlayerController != null) {
			return currentPlayerController;
		}

		return null;
	}

	public static List<health.weakSpot> getCharacterWeakSpotList (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getCharacterWeakSpotListWithHealthManagement ();
		}

		return null;
	}

	//energy management
	public static float getCurrentEnergyAmount (GameObject character)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();

		if (otherPowersToCheck != null) {
			return otherPowersToCheck.getCurrentEnergyAmount ();
		}

		healthManagement currentHealthManagement = character.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getCurrentEnergyAmountWithHealthManagement ();
		}

		return 0;
	}

	public static float getMaxEnergyAmount (GameObject character)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();

		if (otherPowersToCheck != null) {
			return otherPowersToCheck.getMaxEnergyAmount ();
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();

		if (vehicleHUDManagerToCheck != null) {
			return vehicleHUDManagerToCheck.getMaxEnergyAmount ();
		}

		return 0;
	}

	public static float getAuxEnergyAmount (GameObject character)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();

		if (otherPowersToCheck != null) {
			return otherPowersToCheck.getAuxEnergyAmount ();
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();

		if (vehicleHUDManagerToCheck != null) {
			return vehicleHUDManagerToCheck.getAuxEnergyAmount ();
		}

		return 0;
	}

	public static void addAuxEnergyAmount (GameObject character, float amount)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();

		if (otherPowersToCheck != null) {
			otherPowersToCheck.addAuxEnergyAmount (amount);

			return;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();

		if (vehicleHUDManagerToCheck != null) {
			
			vehicleHUDManagerToCheck.addAuxEnergyAmount (amount);
			return;
		}
	}

	public static float getEnergyAmountToPick (GameObject character, float amount)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();

		if (otherPowersToCheck != null) {
			float totalAmmoAmountToAdd = 0;

			float amountToRefill = otherPowersToCheck.getEnergyAmountToLimit ();

			if (amountToRefill > 0) {
				print ("amount to refill " + amountToRefill);

				totalAmmoAmountToAdd = amount;
				if (amountToRefill < amount) {
					totalAmmoAmountToAdd = amountToRefill;
				}

				//print (totalAmmoAmountToAdd);

				otherPowersToCheck.addAuxEnergyAmount (totalAmmoAmountToAdd);
			}

			return totalAmmoAmountToAdd;
		}

		vehicleHUDManager vehicleHUDManagerToCheck = character.GetComponent<vehicleHUDManager> ();

		if (vehicleHUDManagerToCheck != null) {
			float totalAmmoAmountToAdd = 0;

			float amountToRefill = vehicleHUDManagerToCheck.getEnergyAmountToLimit ();

			if (amountToRefill > 0) {
				//print ("amount to refill " + amountToRefill);
				totalAmmoAmountToAdd = amount;
				if (amountToRefill < amount) {
					totalAmmoAmountToAdd = amountToRefill;
				}

				//print (totalAmmoAmountToAdd);

				vehicleHUDManagerToCheck.addAuxEnergyAmount (totalAmmoAmountToAdd);
			}

			return totalAmmoAmountToAdd;
		}

		return 0;
	}

	public static bool checkIfMaxEnergy (GameObject character)
	{
		otherPowers otherPowersToCheck = character.GetComponent<otherPowers> ();

		if (otherPowersToCheck != null) {
			if (otherPowersToCheck.getCurrentEnergyAmount () >= otherPowersToCheck.getMaxEnergyAmount ()) {
				return true;
			}
		}

		healthManagement currentHealthManagement = character.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.checkIfMaxEnergyWithHealthManagement ();
		}

		return false;
	}

	//vehicle fuel management
	public static float getCurrentFuelAmount (GameObject vehicle)
	{
		healthManagement currentHealthManagement = vehicle.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getCurrentFuelAmountWithHealthManagement ();
		}

		return 0;
	}

	public static float getMaxFuelAmount (GameObject vehicle)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();

		if (vehicleHUDManagerToCheck != null) {
			return vehicleHUDManagerToCheck.getMaxFuelAmount ();
		}

		return 0;
	}

	public static float getAuxFuelAmount (GameObject vehicle)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();

		if (vehicleHUDManagerToCheck != null) {
			return vehicleHUDManagerToCheck.getAuxFuelAmount ();
		}

		return 0;
	}

	public static void addAuxFuelAmount (GameObject vehicle, float amount)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();

		if (vehicleHUDManagerToCheck != null) {
			vehicleHUDManagerToCheck.addAuxFuelAmount (amount);
		}
	}

	public static float getFuelAmountToPick (GameObject vehicle, float amount)
	{
		vehicleHUDManager vehicleHUDManagerToCheck = vehicle.GetComponent<vehicleHUDManager> ();

		if (vehicleHUDManagerToCheck != null) {
			float totalAmmoAmountToAdd = 0;

			float amountToRefill = vehicleHUDManagerToCheck.getFuelAmountToLimit ();

			if (amountToRefill > 0) {
				
				print ("amount to refill " + amountToRefill);

				totalAmmoAmountToAdd = amount;

				if (amountToRefill < amount) {
					totalAmmoAmountToAdd = amountToRefill;
				}

				print (totalAmmoAmountToAdd);

				vehicleHUDManagerToCheck.addAuxFuelAmount (totalAmmoAmountToAdd);
			}

			return totalAmmoAmountToAdd;
		}

		return 0;
	}

	public static bool checkIfMaxFuel (GameObject vehicle)
	{
		healthManagement currentHealthManagement = vehicle.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.checkIfMaxFuelWithHealthManagement ();
		}

		return false;
	}

	//character ragdoll push management
	public static void pushCharacter (GameObject character, Vector3 direction)
	{
		health mainHealth = character.GetComponent<health> ();

		if (mainHealth != null) {
			if (mainHealth.advancedSettings.haveRagdoll) {
				ragdollActivator currentRagdollActivator = mainHealth.getMainRagdollActivator ();

				if (currentRagdollActivator != null) {
					currentRagdollActivator.pushCharacter (direction);
				}
			}
		}
	}

	public static void pushCharacterWithoutForce (GameObject character)
	{
		health mainHealth = character.GetComponent<health> ();

		if (mainHealth != null) {
			if (mainHealth.advancedSettings.haveRagdoll) {
				ragdollActivator currentRagdollActivator = mainHealth.getMainRagdollActivator ();

				if (currentRagdollActivator != null) {
					currentRagdollActivator.pushCharacterWithoutForce ();
				}
			}
		}
	}

	public static void pushCharacterWithoutForceAndPauseGetUp (GameObject character)
	{
		health mainHealth = character.GetComponent<health> ();

		if (mainHealth != null) {
			if (mainHealth.advancedSettings.haveRagdoll) {
				ragdollActivator currentRagdollActivator = mainHealth.getMainRagdollActivator ();

				if (currentRagdollActivator != null) {
					currentRagdollActivator.pushCharacterWithoutForce ();
					currentRagdollActivator.setCheckGetUpPausedState (true);
				}
			}
		}
	}

	public static bool checkIfCharacterCanBePushedOnExplosions (GameObject character)
	{
		bool value = false;
		health currentCharacter = character.GetComponent<health> ();

		if (currentCharacter != null) {
			if (currentCharacter.advancedSettings.haveRagdoll && currentCharacter.advancedSettings.allowPushCharacterOnExplosions) {
				value = true;
			}
		}

		return value;
	}

	public static List<int> getBodyColliderLayerList (GameObject character)
	{
		ragdollActivator currentRagdollActivator = character.GetComponent<ragdollActivator> ();

		if (currentRagdollActivator != null) {
			List<Collider> colliders = currentRagdollActivator.getBodyColliderList ();
			List<int> layerList = new List<int> ();

			for (int i = 0; i < colliders.Count; i++) {
				layerList.Add (colliders [i].gameObject.layer);
			}

			return layerList;
		}

		return null;
	}

	public static void setBodyColliderLayerList (GameObject character, List<int> newLayerList)
	{
		ragdollActivator currentRagdollActivator = character.GetComponent<ragdollActivator> ();

		if (currentRagdollActivator != null) {
			List<Collider> colliders = currentRagdollActivator.getBodyColliderList ();

			for (int i = 0; i < colliders.Count; i++) {
				colliders [i].gameObject.layer = newLayerList [i];
			}
		}
	}

	public static void setBodyColliderLayerList (GameObject character, int newLayer)
	{
		ragdollActivator currentRagdollActivator = character.GetComponent<ragdollActivator> ();

		if (currentRagdollActivator != null) {
			List<Collider> colliders = currentRagdollActivator.getBodyColliderList ();

			for (int i = 0; i < colliders.Count; i++) {
				colliders [i].gameObject.layer = newLayer;
			}
		}
	}

	public static void pushRagdoll (GameObject character, Vector3 direction)
	{
		ragdollActivator currentRagdollActivator = character.GetComponent<ragdollActivator> ();

		if (currentRagdollActivator != null) {
			currentRagdollActivator.pushCharacter (direction);
		}
	}

	public static bool isCharacterInRagdollState (GameObject character)
	{
		healthManagement currentHealthManagement = character.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.isCharacterInRagdollState ();
		}

		return false;
	}

	public static Transform getCharacterRootMotionTransform (GameObject character)
	{
		healthManagement currentHealthManagement = character.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getCharacterRootMotionTransform ();
		}

		return null;
	}

	public static void pushAnyCharacter (GameObject character, float pushCharacterForce, float pushCharacterRagdollForce, Vector3 direction)
	{
		Rigidbody RigidbodyToCheck = character.GetComponent<Rigidbody> ();

		if (RigidbodyToCheck != null) {
			if (!RigidbodyToCheck.isKinematic) {
				characterDamageReceiver damageReceiver = character.GetComponent<characterDamageReceiver> ();

				if (damageReceiver != null) {
					if (damageReceiver.isCharacterDead ()) {
						RigidbodyToCheck.AddForce (direction * (pushCharacterForce * RigidbodyToCheck.mass), ForceMode.Impulse);
					} else {
						health healthToCheck = damageReceiver.healthManager;

						if (healthToCheck != null) {
							if (healthToCheck.advancedSettings.haveRagdoll) {
								ragdollActivator currentRagdollActivator = healthToCheck.getMainRagdollActivator ();

								if (currentRagdollActivator != null) {
//									print (currentRagdollActivator.name);
									currentRagdollActivator.pushCharacter (direction * pushCharacterRagdollForce);
								}
							}
						}
					}
				} else {
					vehicleDamageReceiver vehicleDamage = character.GetComponent<vehicleDamageReceiver> ();

					if (vehicleDamage != null) {
						Rigidbody vehicleRigidbody = vehicleDamage.vehicle.GetComponent<Rigidbody> ();

						if (vehicleRigidbody != null) {
							if (!vehicleRigidbody.isKinematic) {
								vehicleRigidbody.AddForce (direction * (pushCharacterForce * vehicleRigidbody.mass), ForceMode.Impulse);
							}
						}
					} else {
						RigidbodyToCheck.AddForce (direction * (pushCharacterForce * RigidbodyToCheck.mass), ForceMode.Impulse);
					}
				}
			}
		} else {
			vehicleDamageReceiver vehicleDamage = character.GetComponent<vehicleDamageReceiver> ();

			if (vehicleDamage != null) {
				Rigidbody vehicleRigidbody = vehicleDamage.vehicle.GetComponent<Rigidbody> ();

				if (vehicleRigidbody != null) {
					if (!vehicleRigidbody.isKinematic) {
						vehicleRigidbody.AddForce (direction * (pushCharacterForce * vehicleRigidbody.mass), ForceMode.Impulse);
					}
				}
			}
		}
	}

	public static bool attachObjectToSurfaceFound (Transform surfaceFound, Transform objectToAttach, Vector3 positionToCheck, bool checkRagdoll)
	{
		bool objectAttached = false;

		bool isAttachedToCharacter = false;

		if (surfaceFound.GetComponent<Rigidbody> ()) {
			characterDamageReceiver currentCharacterDamageReceiver = surfaceFound.GetComponent<characterDamageReceiver> ();

			if (currentCharacterDamageReceiver != null) {
				Transform newParent = null;

				if (checkRagdoll) {
					ragdollActivator currentRagdoll = surfaceFound.GetComponent<ragdollActivator> ();
					
					if (currentRagdoll != null) {
						List<ragdollActivator.bodyPart> bones = currentRagdoll.getBodyPartsList ();
						float distance = Mathf.Infinity;
					
						int index = -1;
					
						for (int i = 0; i < bones.Count; i++) {
							float currentDistance = GKC_Utils.distance (bones [i].transform.position, objectToAttach.position);
							if (currentDistance < distance) {
								distance = currentDistance;
								index = i;
							}
						}
					
						if (index != -1) {
							newParent = bones [index].transform;
						}
					} 
				} else {
					newParent = currentCharacterDamageReceiver.getTransformToAttachWeaponsByClosestPosition (positionToCheck);
				}

				if (newParent != null) {
					objectToAttach.SetParent (newParent);

					isAttachedToCharacter = true;
				} else {
					objectToAttach.SetParent (currentCharacterDamageReceiver.character.transform);
				}

				objectAttached = true;
			} else {
				objectToAttach.SetParent (surfaceFound);

				objectAttached = true;
			}
		} 

		if (!objectAttached) {
			characterDamageReceiver currentCharacterDamageReceiver = surfaceFound.GetComponent<characterDamageReceiver> ();

			if (currentCharacterDamageReceiver != null) {
				checkParentToAssign (objectToAttach, surfaceFound);

				objectAttached = true;
			} 

			if (!objectAttached) {
				vehicleDamageReceiver currentVehicleDamageReceiver = surfaceFound.GetComponent<vehicleDamageReceiver> ();

				if (currentVehicleDamageReceiver != null) {
					objectToAttach.SetParent (currentVehicleDamageReceiver.vehicle.transform);
				}
			}
		} 

		return isAttachedToCharacter;
	}

	public static void checkParentToAssign (Transform objectToCheck, Transform newParent)
	{
		parentAssignedSystem currentParentAssignedSystem = newParent.GetComponent<parentAssignedSystem> ();

		if (currentParentAssignedSystem != null) {
			newParent = currentParentAssignedSystem.getAssignedParentTransform ();
		}

		objectToCheck.SetParent (newParent);
	}

	public static bool setObjectParentOnDetectedSurface (Transform objectToSet, bool attachToLimbs, Transform surfaceDetected, bool ignoreCollisionsOnObjects)
	{
		Rigidbody currentRigidbody = surfaceDetected.GetComponent<Rigidbody> ();

		if (currentRigidbody != null) {
			ragdollActivator currentRagdollActivator = surfaceDetected.GetComponent<ragdollActivator> ();

			if (currentRagdollActivator != null) {
				if (attachToLimbs) {
					List<ragdollActivator.bodyPart> bones = currentRagdollActivator.getBodyPartsList ();

					float distance = Mathf.Infinity;
					int index = -1;

					for (int i = 0; i < bones.Count; i++) {
						float currentDistance = GKC_Utils.distance (bones [i].transform.position, objectToSet.position);

						if (currentDistance < distance) {
							distance = currentDistance;
							index = i;
						}
					}

					if (index != -1) {
						objectToSet.SetParent (bones [index].transform);

						if (ignoreCollisionsOnObjects) {
							Collider objectCollider = objectToSet.GetComponent<Collider> ();

							if (objectCollider != null) {
								for (int i = 0; i < bones.Count; i++) {
									
									Collider surfaceCollider = bones [i].transform.GetComponent<Collider> ();

									if (surfaceCollider != null) {
										Physics.IgnoreCollision (objectCollider, surfaceCollider, true);
									}
								}
							}
						}
					}

					return true;
				}
			} 

			characterDamageReceiver currentCharacterDamageReceiver = surfaceDetected.GetComponent<characterDamageReceiver> ();

			if (currentCharacterDamageReceiver != null) {
				objectToSet.SetParent (currentCharacterDamageReceiver.character.transform);

				if (ignoreCollisionsOnObjects) {
					Collider objectCollider = objectToSet.GetComponent<Collider> ();

					if (objectCollider != null) {
						Collider surfaceCollider = currentCharacterDamageReceiver.character.GetComponent<Collider> ();

						if (surfaceCollider != null) {
							Physics.IgnoreCollision (objectCollider, surfaceCollider, true);
						}
					}
				}

				return true;
			} 

			objectToSet.SetParent (surfaceDetected.transform);

			return true;
		} 

		vehicleDamageReceiver currentVehicleDamageReceiver = surfaceDetected.GetComponent<vehicleDamageReceiver> ();

		if (currentVehicleDamageReceiver != null) {
			objectToSet.SetParent (currentVehicleDamageReceiver.getPlaceToShootWithHealthManagement ());

			if (ignoreCollisionsOnObjects) {
				Collider objectCollider = objectToSet.GetComponent<Collider> ();

				if (objectCollider != null) {
					currentVehicleDamageReceiver.ignoreCollisionWithSolidVehicleColliderList (objectCollider, true);
				}
			}

			return true;
		} 

		parentAssignedSystem currentParentAssignedSystem = surfaceDetected.GetComponent<parentAssignedSystem> ();

		if (currentParentAssignedSystem != null) {
			objectToSet.SetParent (currentParentAssignedSystem.getAssignedParentTransform ());

			if (ignoreCollisionsOnObjects) {
				Collider objectCollider = objectToSet.GetComponent<Collider> ();

				if (objectCollider != null) {
					Collider surfaceCollider = objectToSet.transform.parent.GetComponent<Collider> ();

					if (surfaceCollider != null) {
						Physics.IgnoreCollision (objectCollider, surfaceCollider, true);
					}
				}
			}

			return true;
		}

		return false;
	}

	public static void setExplosion (Vector3 explosionPosition, float explosionRadius, bool userLayerMask, LayerMask layerToSearch, GameObject explosionOwner, 
	                                 bool canDamageProjectileOwner, GameObject projectileGameObject, bool killObjectsInRadius, bool isExplosive, bool isImplosive, 
	                                 float explosionDamage, bool pushCharacters, bool applyExplosionForceToVehicles, float explosionForceToVehiclesMultiplier, 
	                                 float explosionForce, ForceMode forceMode, bool checkObjectsInsideExplosionOwner, Transform explosionOwnerTransform, 
	                                 bool ignoreShield, bool checkForRemoteEventsOnObject, string remoteEventNameToActivate, int damageTypeID)
	{
		List<Collider> colliders = new List<Collider> ();

		List<Rigidbody> vehiclesRigidbodyFoundList = new List<Rigidbody> ();

		if (userLayerMask) {
			colliders.AddRange (Physics.OverlapSphere (explosionPosition, explosionRadius, layerToSearch));
		} else {
			colliders.AddRange (Physics.OverlapSphere (explosionPosition, explosionRadius));
		}

		Collider ownerCollider = null;

		if (explosionOwner != null) {
			ownerCollider = explosionOwner.GetComponent<Collider> ();
		}

		foreach (Collider currentCollider in colliders) {
			if (currentCollider != null &&
			    (canDamageProjectileOwner || ownerCollider != currentCollider) &&
			    (!checkObjectsInsideExplosionOwner || !currentCollider.gameObject.transform.IsChildOf (explosionOwnerTransform))) {

				GameObject objectDetected = currentCollider.gameObject;

				Vector3 objectPosition = objectDetected.transform.position;

				Vector3 explosionDirection = objectPosition - explosionPosition;
				if (isImplosive) {
					explosionDirection = explosionPosition - objectPosition;
				}
				explosionDirection = explosionDirection / explosionDirection.magnitude;

				if (killObjectsInRadius) {
					applyDamage.killCharacter (projectileGameObject, objectDetected, -explosionDirection, objectPosition, explosionOwner, false);
				} else {
					applyDamage.checkHealth (projectileGameObject, objectDetected, explosionDamage, explosionDirection, objectPosition, 
						explosionOwner, false, false, ignoreShield, false, false, -1, damageTypeID);

					if (pushCharacters) {
						
						if (applyDamage.checkIfCharacterCanBePushedOnExplosions (objectDetected)) {
							applyDamage.pushRagdoll (objectDetected, explosionDirection);
						}
					}
				}

				if (checkForRemoteEventsOnObject) {
					remoteEventSystem currentRemoteEventSystem = objectDetected.GetComponent<remoteEventSystem> ();

					if (currentRemoteEventSystem != null) {
						currentRemoteEventSystem.callRemoteEvent (remoteEventNameToActivate);
					}
				}

				if (applyExplosionForceToVehicles) {
					Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectDetected);

					if (objectToDamageMainRigidbody != null) {
						if (!vehiclesRigidbodyFoundList.Contains (objectToDamageMainRigidbody)) {
							bool isVehicle = applyDamage.isVehicle (objectDetected);

							float finalExplosionForce = explosionForce;
							if (isVehicle) {
								finalExplosionForce *= explosionForceToVehiclesMultiplier;
							}

							if (isExplosive) {
								objectToDamageMainRigidbody.AddExplosionForce (finalExplosionForce * objectToDamageMainRigidbody.mass, explosionPosition, explosionRadius, 3, forceMode);
							}

							if (isImplosive) {
								Vector3 Dir = explosionPosition - objectPosition;
								Vector3 Dirscale = Vector3.Scale (Dir.normalized, projectileGameObject.transform.localScale);
								objectToDamageMainRigidbody.AddForce (Dirscale * (finalExplosionForce * objectToDamageMainRigidbody.mass), forceMode);
							}

							if (isVehicle) {
								vehiclesRigidbodyFoundList.Add (objectToDamageMainRigidbody);
							}
						}
					}
				} else {
					if (applyDamage.canApplyForce (objectDetected)) {
						Rigidbody currentHitRigidbody = currentCollider.GetComponent<Rigidbody> ();

						//explosion type
						if (isExplosive) {
							currentHitRigidbody.AddExplosionForce (explosionForce * currentHitRigidbody.mass, explosionPosition, explosionRadius, 3, forceMode);
						}

						//implosion type
						if (isImplosive) {
							Vector3 Dir = explosionPosition - objectPosition;
							Vector3 Dirscale = Vector3.Scale (Dir.normalized, projectileGameObject.transform.localScale);

							currentHitRigidbody.AddForce (Dirscale * (explosionForce * currentHitRigidbody.mass), forceMode);
						}
					}
				}
			}
		}
	}

	//Check if an object is a vehicle or a character
	public static GameObject getCharacterOrVehicle (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getCharacterOrVehicleWithHealthManagement ();
		}

		return null;
	}

	public static GameObject getCharacter (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getCharacterWithHealthManagement ();
		}

		return null;
	}

	public static GameObject getVehicle (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getVehicleWithHealthManagement ();
		}

		return null;
	}

	public static vehicleHUDManager getVehicleHUDManager (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getVehicleHUDManagerWithHealthManagement ();
		}

		return null;
	}

	public static void setVehicleInteractionTriggerState (GameObject objectToCheck, bool state)
	{
		vehicleHUDManager currentVehicleHUDManager = getVehicleHUDManager (objectToCheck);

		if (currentVehicleHUDManager != null) {
			currentVehicleHUDManager.setVehicleInteractionTriggerState (state);
		}
	}

	public static void ejectAllPassengersFromVehicle (GameObject objectToCheck)
	{
		vehicleHUDManager currentVehicleHUDManager = getVehicleHUDManager (objectToCheck);

		if (currentVehicleHUDManager != null) {
			currentVehicleHUDManager.ejectFromVehicle ();
		}
	}

	public static void activateSelfDestructionOnVehicleExternally (GameObject objectToCheck, float newSelfDestructDelayTime)
	{
		vehicleHUDManager currentVehicleHUDManager = getVehicleHUDManager (objectToCheck);

		if (currentVehicleHUDManager != null) {
			currentVehicleHUDManager.activateSelfDestructionOnVehicleExternally (newSelfDestructDelayTime);
		}
	}

	public static void setNewVehicleGravityForce (GameObject objectToCheck, float newSelfDestructDelayTime)
	{
		vehicleHUDManager currentVehicleHUDManager = getVehicleHUDManager (objectToCheck);

		if (currentVehicleHUDManager != null) {
			currentVehicleHUDManager.setNewVehicleGravityForce (newSelfDestructDelayTime);
		}
	}

	public static void setOriginalGravityForce (GameObject objectToCheck)
	{
		vehicleHUDManager currentVehicleHUDManager = getVehicleHUDManager (objectToCheck);

		if (currentVehicleHUDManager != null) {
			currentVehicleHUDManager.setOriginalGravityForce ();
		}
	}

	public static void setReducedVehicleSpeed (GameObject objectToCheck, float reducedVehicleSpeedMultiplier)
	{
		vehicleHUDManager currentVehicleHUDManager = getVehicleHUDManager (objectToCheck);

		if (currentVehicleHUDManager != null) {
			currentVehicleHUDManager.setReducedVehicleSpeed (reducedVehicleSpeedMultiplier);
		}
	}

	public static GameObject getVehicleDriver (GameObject objectToCheck)
	{
		healthManagement currentHealthManagement = objectToCheck.GetComponent<healthManagement> ();

		if (currentHealthManagement != null) {
			return currentHealthManagement.getVehicleDriverWithHealthManagement ();
		} else {
			GKCSimpleRiderSystem mainGKCSimpleRiderSystem = objectToCheck.GetComponentInChildren<GKCSimpleRiderSystem> ();

			if (mainGKCSimpleRiderSystem == null) {
				GKCRiderSocketSystem currentGKCRiderSocketSystem = objectToCheck.GetComponentInChildren<GKCRiderSocketSystem> ();

				if (currentGKCRiderSocketSystem != null) {
					mainRiderSystem currentmainRiderSystem = currentGKCRiderSocketSystem.getMainRiderSystem ();

					mainGKCSimpleRiderSystem = currentmainRiderSystem.GetComponent<GKCSimpleRiderSystem> ();
				}
			}

			if (mainGKCSimpleRiderSystem != null) {
				if (mainGKCSimpleRiderSystem.isBeingDrivenActive ()) {
					return mainGKCSimpleRiderSystem.getCurrentDriver ();
				}
			}
		}

		return null;
	}

	//Audio Source Management
	public static AudioSource getAudioSource (GameObject objectToCheck, string audioSourceName)
	{
		if (objectToCheck == null) {
			return null;
		}

		playerStatesManager playerStatesManagerToCheck = objectToCheck.GetComponent<playerStatesManager> ();

		if (playerStatesManagerToCheck != null) {
			return playerStatesManagerToCheck.getAudioSourceElement (audioSourceName);
		}

		vehicleHUDManager vehicleHUDManagerToCheck = objectToCheck.GetComponent<vehicleHUDManager> ();

		if (vehicleHUDManagerToCheck != null) {
			return vehicleHUDManagerToCheck.getAudioSourceElement (audioSourceName);
		}

		return null;
	}

	//Weapons management
	public static int getPlayerWeaponAmmoAmountToPick (playerWeaponsManager weaponsManager, string weaponName, int ammoAmuntToAdd)
	{
		int totalAmmoAmountToAdd = 0;
		int weaponIndex = -1;

		for (int i = 0; i < weaponsManager.weaponsList.Count; i++) {
			if (weaponsManager.weaponsList [i].getWeaponSystemName () == weaponName && weaponsManager.weaponsList [i].isWeaponEnabled ()) {
				weaponIndex = i;
			}
		}

		if (weaponIndex > -1) {
			playerWeaponSystem currentWeapon = weaponsManager.weaponsList [weaponIndex].getWeaponSystemManager ();

			if (currentWeapon.canIncreaseRemainAmmo ()) {
				
				int amountToRefill = currentWeapon.ammoAmountToMaximumLimit ();
				print ("amount to refill " + amountToRefill);

				totalAmmoAmountToAdd = ammoAmuntToAdd;
				if (amountToRefill < totalAmmoAmountToAdd) {
					totalAmmoAmountToAdd = amountToRefill;
				}
				print (totalAmmoAmountToAdd);

				if (totalAmmoAmountToAdd > 0) {
					currentWeapon.addAuxRemainAmmo (totalAmmoAmountToAdd);
				}
			}
		}

		return totalAmmoAmountToAdd;
	}

	public static int getVehicleWeaponAmmoAmountToPick (vehicleWeaponSystem weaponsManager, string weaponName, int ammoAmuntToAdd)
	{
		int totalAmmoAmountToAdd = 0;
		int weaponIndex = -1;

		for (int i = 0; i < weaponsManager.weapons.Count; i++) {
			if (weaponsManager.weapons [i].enabled && weaponsManager.weapons [i].Name.Equals (weaponName)) {
				weaponIndex = i;
			}
		}

		if (weaponIndex > -1) {
			vehicleWeaponSystem.vehicleWeapons currentWeapon = weaponsManager.weapons [weaponIndex];

			if (weaponsManager.canIncreaseRemainAmmo (currentWeapon)) {

				int amountToRefill = weaponsManager.ammoAmountToMaximumLimit (currentWeapon);
				print ("amount to refill " + amountToRefill);

				totalAmmoAmountToAdd = ammoAmuntToAdd;
				if (amountToRefill < totalAmmoAmountToAdd) {
					totalAmmoAmountToAdd = amountToRefill;
				}
				print (totalAmmoAmountToAdd);

				if (totalAmmoAmountToAdd > 0) {
					weaponsManager.addAuxRemainAmmo (currentWeapon, totalAmmoAmountToAdd);
				}
			}
		}

		return totalAmmoAmountToAdd;
	}

	//Noises Management
	public static void sendNoiseSignal (float noiseRadius, Vector3 noisePosition, LayerMask noiseLyaer, float noiseDecibels, 
	                                    bool forceNoiseDetection, bool showNoiseDetectionGizmo, int noiseID)
	{
		Collider[] colliders = Physics.OverlapSphere (noisePosition, noiseRadius, noiseLyaer);

		if (showNoiseDetectionGizmo) {
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.right * noiseRadius + Vector3.up, Color.yellow, 2);
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.left * noiseRadius + Vector3.up, Color.yellow, 2);
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.forward * noiseRadius + Vector3.up, Color.yellow, 2);
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.back * noiseRadius + Vector3.up, Color.yellow, 2);
		}

		if (colliders.Length > 0) {
			for (int i = 0; i < colliders.Length; i++) {
				findObjectivesSystem currentFindObjectivesSystem = colliders [i].GetComponent<findObjectivesSystem> ();

				if (currentFindObjectivesSystem != null) {
					currentFindObjectivesSystem.checkNoisePosition (noisePosition, noiseDecibels, forceNoiseDetection, noiseID);
				}
			}
		}
	}

	public static GameObject sendNoiseSignalToClosestAI (float noiseRadius, Vector3 noisePosition, LayerMask noiseLyaer, float noiseDecibels, 
	                                                     bool forceNoiseDetection, bool showNoiseDetectionGizmo, int noiseID)
	{
		Collider[] colliders = Physics.OverlapSphere (noisePosition, noiseRadius, noiseLyaer);

		if (showNoiseDetectionGizmo) {
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.right * noiseRadius + Vector3.up, Color.yellow, 2);
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.left * noiseRadius + Vector3.up, Color.yellow, 2);
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.forward * noiseRadius + Vector3.up, Color.yellow, 2);
			Debug.DrawLine (noisePosition + Vector3.up, noisePosition + Vector3.back * noiseRadius + Vector3.up, Color.yellow, 2);
		}

		if (colliders.Length > 0) {

			float minDistance = Mathf.Infinity;

			int AIDetecteList = colliders.Length;

			float currentDistance = 0;

			GameObject closestTarget = null;

			for (int i = 0; i < AIDetecteList; i++) {
				currentDistance = GKC_Utils.distance (colliders [i].transform.position, noisePosition);

				if (currentDistance < minDistance) {
					minDistance = currentDistance;
					closestTarget = colliders [i].gameObject;
				}
			}


			if (closestTarget) {
				findObjectivesSystem currentFindObjectivesSystem = closestTarget.GetComponent<findObjectivesSystem> ();

				if (currentFindObjectivesSystem != null) {
					currentFindObjectivesSystem.checkNoisePosition (noisePosition, noiseDecibels, forceNoiseDetection, noiseID);

					return closestTarget;
				}
			}
		}

		return null;
	}

	public static bool giveInventoryObjectToCharacter (GameObject character, string inventoryObjectName,
	                                                   int inventoryObjectAmount, Transform positionToInstantiateInventoryObjectPickup, 
	                                                   float forceAmount, float maxRadiusToInstantiate, 
	                                                   ForceMode inventoryObjectForceMode, float forceRadius, 
	                                                   bool spawnAllInventoryObjects)
	{
		if (character == null) {
			print ("WARNING: not character has being sent to the function to get inventory objects, make sure a player has been assigned");
			return true;
		}

		playerComponentsManager currentPlayerComponentsManager = character.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			inventoryManager mainInventoryManager = currentPlayerComponentsManager.getInventoryManager ();

			if (mainInventoryManager.existInventoryInfoFromName (inventoryObjectName)) {

				inventoryInfo currentInventoryInfo = new inventoryInfo (mainInventoryManager.getInventoryInfoByName (inventoryObjectName));

				if (currentInventoryInfo.inventoryGameObject != null) {
					bool isInventoryFull = mainInventoryManager.isInventoryFull ();

					bool enoughFreeSpace = mainInventoryManager.checkIfObjectCanBeStored (currentInventoryInfo.inventoryGameObject, inventoryObjectAmount);

					currentInventoryInfo.amount = inventoryObjectAmount;

					if (spawnAllInventoryObjects) {
						enoughFreeSpace = false;

						isInventoryFull = false;
					}

					if (enoughFreeSpace) {
						int inventoryAmountPicked = mainInventoryManager.tryToPickUpObject (currentInventoryInfo);

//						print ("amount taken " + inventoryAmountPicked);

						pickUpsScreenInfo pickUpsScreenInfoManager = currentPlayerComponentsManager.getPickUpsScreenInfo ();
					
						if (pickUpsScreenInfoManager != null) {
						
							string info = currentInventoryInfo.Name + " Stored";

							if (inventoryAmountPicked > 1) {
								info = currentInventoryInfo.Name + " x " + inventoryAmountPicked;
							}

							pickUpsScreenInfoManager.recieveInfo (info);
						}

						return true;

					} else if (!enoughFreeSpace && !isInventoryFull) {
						GameObject inventoryObjectPrefab = mainInventoryManager.getInventoryPrefab (currentInventoryInfo.inventoryGameObject);

						GameObject newInventoryObject = inventoryListManager.spawnInventoryObject (inventoryObjectPrefab, 
							                                positionToInstantiateInventoryObjectPickup, inventoryObjectAmount, currentInventoryInfo);

						newInventoryObject.transform.position += Random.insideUnitSphere * maxRadiusToInstantiate;

						if (newInventoryObject != null) {
							Rigidbody currentRigidbody = newInventoryObject.GetComponent<Rigidbody> ();

							if (currentRigidbody != null) {
								currentRigidbody.AddExplosionForce (forceAmount, positionToInstantiateInventoryObjectPickup.position, 
									forceRadius, 1, inventoryObjectForceMode);
							}
						}
					}
				} else {
					print ("WARNING: Inventory object called " + inventoryObjectName + " not found or is not configured properly in the inventory list manager");
				}
			} else {
				print ("WARNING: Inventory object called " + inventoryObjectName + " not found or is not configured properly in the inventory list manager");
			}
		}

		return false;
	}

	public static bool addInventoryExtraSpace (GameObject character, int extraInventorySlotsAmount)
	{
		if (character == null) {
			print ("WARNING: not character has being sent to the function to get inventory objects, make sure a player has been assigned");
			return true;
		}

		playerComponentsManager currentPlayerComponentsManager = character.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			inventoryManager mainInventoryManager = currentPlayerComponentsManager.getInventoryManager ();

			if (mainInventoryManager != null) {
				mainInventoryManager.addInventoryExtraSpace (extraInventorySlotsAmount);
			}
		}

		return false;
	}

	public static void checkGravityRoomForGrabbedObject (GameObject grabbedObject, GameObject playerControllerGameObject)
	{
		gravitySystem mainGravitrySystem = playerControllerGameObject.GetComponent<gravitySystem> ();

		if (mainGravitrySystem != null) {
			if (mainGravitrySystem.isPlayerInsiderGravityRoom ()) {
				grabbedObjectState currentGrabbedObjectState = grabbedObject.AddComponent<grabbedObjectState> ();

				currentGrabbedObjectState.setCurrentHolder (playerControllerGameObject);

				currentGrabbedObjectState.setInsideZeroGravityRoomState (true);

				currentGrabbedObjectState.setCurrentZeroGravityRoom (mainGravitrySystem.getCurrentZeroGravityRoom ());

				currentGrabbedObjectState.checkGravityRoomState ();

				currentGrabbedObjectState.setGrabbedState (false);

				Rigidbody grabbedObjectRigidbody = grabbedObject.GetComponent<Rigidbody> ();

				if (grabbedObjectRigidbody != null) {
					grabbedObjectRigidbody.useGravity = false;
				}
			}
		}
	}

	public static void checkIfPlayerIsLookingAtDeadTarget (Transform deadTarget, Transform placeToShoot)
	{
		if (deadTarget == null) {
			return;
		}

		playerCharactersManager mainPlayerCharactersManager = FindObjectOfType<playerCharactersManager> ();

		if (mainPlayerCharactersManager != null) {
			mainPlayerCharactersManager.checkIfPlayerIsLookingAtDeadTarget (deadTarget, placeToShoot);
		}
	}

	public static void sendCharacterAroundToAdd (GameObject currentPlayer, Transform targetToAdd, string sendCharacterAroundToAddName)
	{
		if (currentPlayer == null) {
			return;
		}

		remoteEventSystem currentRemoteEventSystem = currentPlayer.GetComponent<remoteEventSystem> ();

		if (currentRemoteEventSystem != null) {
			currentRemoteEventSystem.callRemoteEventWithTransform (sendCharacterAroundToAddName, targetToAdd);
		}
	}

	public static void sendCharacterAroundToRemove (GameObject currentPlayer, Transform targetToRemove, string sendCharacterAroundToRemoveName)
	{
		if (currentPlayer == null) {
			return;
		}

		remoteEventSystem currentRemoteEventSystem = currentPlayer.GetComponent<remoteEventSystem> ();

		if (currentRemoteEventSystem != null) {
			currentRemoteEventSystem.callRemoteEventWithTransform (sendCharacterAroundToRemoveName, targetToRemove);
		}
	}



	//WEAPONS FUNCTIONS
	public static GameObject setSeekerProjectileInfo (Vector3 shootPosition, List<string> tagToLocate, bool usingScreenSpaceCamera, 
	                                                  bool targetOnScreenForSeeker, Camera mainCamera, LayerMask targetToDamageLayer,
	                                                  Vector3 attackerPosition, bool checkIfAttackerLocatedOnRaycast, Collider attackerCollider)
	{
		//get all the enemies in the scene
		List<GameObject> enemiesInFront = new List<GameObject> ();
		List<GameObject> fullEnemyList = new List<GameObject> ();

		int tagToLocateCount = tagToLocate.Count;

		for (int i = 0; i < tagToLocateCount; i++) {
			GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (tagToLocate [i]);

			fullEnemyList.AddRange (enemiesList);
		}

		float screenWidth = 0;
		float screenHeight = 0;

		Vector3 screenPoint = Vector3.zero;

		bool targetOnScreen = false;

		if (!usingScreenSpaceCamera) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;
		}

		int fullEnemyListCount = fullEnemyList.Count;

		for (int i = 0; i < fullEnemyListCount; i++) {
			//get those enemies which are not dead and in front of the camera
			if (!applyDamage.checkIfDead (fullEnemyList [i])) {

				if (targetOnScreenForSeeker) {
					if (usingScreenSpaceCamera) {
						screenPoint = mainCamera.WorldToViewportPoint (fullEnemyList [i].transform.position);
						targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
					} else {
						screenPoint = mainCamera.WorldToScreenPoint (fullEnemyList [i].transform.position);
						targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
					}

					//the target is visible in the screen
					if (targetOnScreen) {
						enemiesInFront.Add (fullEnemyList [i]);
					}
				} else {
					enemiesInFront.Add (fullEnemyList [i]);
				}
			}
		}

		RaycastHit hit;

		GameObject currentEnemyInFront = null;

		for (int i = enemiesInFront.Count - 1; i >= 0; i--) {
			currentEnemyInFront = enemiesInFront [i];

			Transform placeToShoot = applyDamage.getPlaceToShoot (currentEnemyInFront);

			if (placeToShoot != null) {
				Vector3 targetPosition = placeToShoot.position;

				//for every enemy in front of the camera, use a raycast, if it finds an obstacle between the enemy and the camera, the enemy is removed from the list
				Vector3 direction = targetPosition - shootPosition;

				direction = direction / direction.magnitude;

				float distance = GKC_Utils.distance (targetPosition, shootPosition);

				bool removeCurrentTarget = false;

				if (checkIfAttackerLocatedOnRaycast) {
					if (Physics.Raycast (shootPosition, direction, out hit, distance, targetToDamageLayer)) {
						if (hit.collider != attackerCollider) {
							removeCurrentTarget = true;
						} else {
							if (Physics.Raycast (hit.point + direction * 0.2f, direction, out hit, Mathf.Infinity, targetToDamageLayer)) {
								removeCurrentTarget = true;
							}
						}
					}
				} else {
					if (Physics.Raycast (shootPosition, direction, out hit, distance, targetToDamageLayer)) {
						removeCurrentTarget = true;
					}
				}

				if (removeCurrentTarget) {
					if (!hit.transform.IsChildOf (currentEnemyInFront.transform) && hit.collider.gameObject != currentEnemyInFront) {
						enemiesInFront.RemoveAt (i);
					}
				}
			}
		}

		GameObject closestEnemy = null;

		//finally, get the enemy closest to the player
		float minDistance = Mathf.Infinity;

		int enemiesInFrontCount = enemiesInFront.Count;

		float currentDistance = 0;

		for (int i = 0; i < enemiesInFrontCount; i++) {
			currentDistance = GKC_Utils.distance (enemiesInFront [i].transform.position, attackerPosition);

			if (currentDistance < minDistance) {
				minDistance = currentDistance;
				closestEnemy = enemiesInFront [i];
			}
		}

		if (closestEnemy != null) {
			bool targetIsRagdollState = applyDamage.isCharacterInRagdollState (closestEnemy);

			if (targetIsRagdollState) {
				Transform rootMotionTransform = applyDamage.getCharacterRootMotionTransform (closestEnemy);

				if (rootMotionTransform != null) {
					closestEnemy = rootMotionTransform.gameObject;

					return closestEnemy;
				}
			}
				
			Transform placeToShoot = applyDamage.getPlaceToShoot (closestEnemy);

			if (placeToShoot != null) {
				closestEnemy = placeToShoot.gameObject;
			} 
		}

		return closestEnemy;
	}
}