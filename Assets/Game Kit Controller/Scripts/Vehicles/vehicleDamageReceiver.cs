using UnityEngine;
using System.Collections;

public class vehicleDamageReceiver : healthManagement
{
	[Header ("Main Setting")]
	[Space]

	[Range (1, 10)] public float damageMultiplier = 1;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject vehicle;
	public vehicleHUDManager hudManager;

	//this script is added to every collider in a vehicle, so when a projectile hits the vehicle, its health component receives the damge
	//like this the damage detection is really accurated.
	//the function sends the amount of damage, the direction of the projectile, the position where hits, the object that fired the projectile,
	//and if the damaged is done just once, like a bullet, or the damaged is constant like a laser

	//health and damage management
	public void setDamage (float amount, Vector3 fromDirection, Vector3 damagePos, GameObject bulletOwner, GameObject projectile, 
	                       bool damageConstant, bool searchClosestWeakSpot, bool damageCanBeBlocked)
	{
		hudManager.setDamage (amount * damageMultiplier, fromDirection, damagePos, bulletOwner, projectile, damageConstant, 
			searchClosestWeakSpot, damageCanBeBlocked);
	}

	public void setHeal (float amount)
	{
		hudManager.getHealth (amount);
	}

	public override float getCurrentHealthAmount ()
	{
		return hudManager.getCurrentHealthAmount ();
	}

	public float getMaxHealthAmount ()
	{
		return hudManager.getMaxHealthAmount ();
	}

	public float getAuxHealthAmount ()
	{
		return hudManager.getAuxHealthAmount ();
	}

	public void addAuxHealthAmount (float amount)
	{
		hudManager.addAuxHealthAmount (amount);
	}

	//fuel management
	public float getCurrentFuelAmount ()
	{
		return hudManager.getCurrentFuelAmount ();
	}

	public float getMaxFuelAmount ()
	{
		return hudManager.getMaxFuelAmount ();
	}

	public void getFuel (float amount)
	{
		hudManager.getFuel (amount);
	}

	public void removeFuel (float amount)
	{
		hudManager.removeFuel (amount);
	}

	//destroy vehicle
	public void destroyVehicle ()
	{
		hudManager.destroyVehicle ();
	}

	//impact decal management
	public int getDecalImpactIndex ()
	{
		return hudManager.getDecalImpactIndex ();
	}

	//energy management
	public void getEnergy (float amount)
	{
		hudManager.getEnergy (amount);
	}

	public void removeEnergy (float amount)
	{
		hudManager.removeEnergy (amount);
	}

	public float getCurrentEnergyAmount ()
	{
		return hudManager.getCurrentEnergyAmount ();
	}

	public float getMaxEnergyAmount ()
	{
		return hudManager.getMaxEnergyAmount ();
	}

	public float getAuxEnergyAmount ()
	{
		return hudManager.getAuxEnergyAmount ();
	}

	public void addAuxEnergyAmount (float amount)
	{
		hudManager.addAuxEnergyAmount (amount);
	}

	public bool isVehicleDestroyed ()
	{
		return hudManager.destroyed;
	}

	public void ignoreCollisionWithSolidVehicleColliderList (Collider colliderToIgnore, bool state)
	{
		hudManager.ignoreCollisionWithSolidVehicleColliderList (colliderToIgnore, state);
	}

	//set vehicle component
	public void setVehicle (GameObject vehicleGameObject, vehicleHUDManager currentVehicleHUDManager)
	{
		vehicle = vehicleGameObject;
		hudManager = currentVehicleHUDManager;

		updateComponent ();
	}

	public void setDamageTargetOverTimeState (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{
		hudManager.setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath, damageTypeID);
	}

	public void removeDamagetTargetOverTimeState ()
	{
		hudManager.stopDamageOverTime ();
	}

	public vehicleHUDManager getHUDManager ()
	{
		return hudManager;
	}

	//Override functions from Health Management
	public override void setDamageWithHealthManagement (float damageAmount, Vector3 fromDirection, Vector3 damagePos, GameObject attacker,
	                                                    GameObject projectile, bool damageConstant, bool searchClosestWeakSpot, bool ignoreShield, 
	                                                    bool ignoreDamageInScreen, bool damageCanBeBlocked, bool canActivateReactionSystemTemporally,
	                                                    int damageReactionID, int damageTypeID)
	{
		hudManager.setDamage ((damageAmount * damageMultiplier), fromDirection, damagePos, attacker, projectile, damageConstant, 
			searchClosestWeakSpot, damageCanBeBlocked);
	}

	public override bool checkIfDeadWithHealthManagement ()
	{
		return hudManager.vehicleIsDestroyed ();
	}

	public override bool checkIfMaxHealthWithHealthManagement ()
	{
		return hudManager.checkIfMaxHealth ();
	}

	public override void setDamageTargetOverTimeStateWithHealthManagement (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, 
	                                                                       float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{
		hudManager.setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath, damageTypeID);
	}

	public override void removeDamagetTargetOverTimeStateWithHealthManagement ()
	{
		hudManager.stopDamageOverTime ();
	}

	public override void setHealWithHealthManagement (float healAmount)
	{
		hudManager.getHealth (healAmount);
	}

	public override float getCurrentHealthAmountWithHealthManagement ()
	{
		return hudManager.getCurrentHealthAmount ();
	}

	public override float getMaxHealthAmountWithHealthManagement ()
	{
		return hudManager.getMaxHealthAmount ();
	}

	public override float getAuxHealthAmountWithHealthManagement ()
	{
		return hudManager.getAuxHealthAmount ();
	}

	public override void addAuxHealthAmountWithHealthManagement (float amount)
	{
		hudManager.addAuxHealthAmount (amount);
	}

	public override float getHealthAmountToPickWithHealthManagement (float amount)
	{
		return hudManager.getHealthAmountToPick (amount);
	}

	public override void killCharacterWithHealthManagement ()
	{
		hudManager.destroyVehicle ();
	}

	public override Transform getPlaceToShootWithHealthManagement ()
	{
		return hudManager.getPlaceToShoot ();
	}

	public override GameObject getPlaceToShootGameObjectWithHealthManagement ()
	{
		return hudManager.getPlaceToShoot ().gameObject;
	}

	public override bool isVehicleWithHealthManagement ()
	{
		return hudManager.isVehicleWithHealthManagement ();
	}

	public override GameObject getCharacterOrVehicleWithHealthManagement ()
	{
		return vehicle;
	}

	public override void setFuelWithHealthManagement (float fuelAmount)
	{
		hudManager.getFuel (fuelAmount);
	}

	public override void removeFuelWithHealthManagement (float fuelAmount)
	{
		hudManager.removeFuel (fuelAmount);
	}

	public override float getCurrentFuelAmountWithHealthManagement ()
	{
		return hudManager.getCurrentFuelAmount ();
	}

	public override bool checkIfMaxFuelWithHealthManagement ()
	{
		return hudManager.checkIfMaxFuel ();
	}

	public override GameObject getVehicleWithHealthManagement ()
	{
		return vehicle;
	}

	public override vehicleHUDManager getVehicleHUDManagerWithHealthManagement ()
	{
		return hudManager;
	}

	public override GameObject getVehicleDriverWithHealthManagement ()
	{
		return hudManager.getCurrentDriver ();
	}

	public override void setEnergyWithHealthManagement (float amount)
	{
		hudManager.getEnergy (amount);
	}

	public override void removeEnergyWithHealthManagement (float amount)
	{
		hudManager.removeEnergy (amount);
	}

	public override float getCurrentEnergyAmountWithHealthManagement ()
	{
		return hudManager.getCurrentEnergyAmount ();
	}

	public override bool checkIfMaxEnergyWithHealthManagement ()
	{
		return hudManager.checkIfMaxEnergy ();
	}

	public override int getDecalImpactIndexWithHealthManagement ()
	{
		return hudManager.getDecalImpactIndex ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Damage Receiver", gameObject);
	}
}