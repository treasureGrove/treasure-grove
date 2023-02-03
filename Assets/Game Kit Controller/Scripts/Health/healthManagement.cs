using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthManagement : MonoBehaviour
{
	public virtual void setDamageWithHealthManagement (float damageAmount, Vector3 fromDirection, Vector3 damagePos, GameObject attacker, 
	                                                   GameObject projectile, bool damageConstant, bool searchClosestWeakSpot, 
	                                                   bool ignoreShield, bool ignoreDamageInScreen, bool damageCanBeBlocked,
	                                                   bool canActivateReactionSystemTemporally,
	                                                   int damageReactionID, int damageTypeID)
	{

	}

	public virtual float getCurrentHealthAmount ()
	{
		return -1;
	}

	public virtual bool isUseShieldActive ()
	{
		return false;
	}

	public virtual float getCurrentShieldAmount ()
	{
		return 0;
	}

	public virtual bool checkIfDeadWithHealthManagement ()
	{
		return false;
	}

	public virtual bool checkIfMaxHealthWithHealthManagement ()
	{
		return false;
	}

	public virtual void setDamageTargetOverTimeStateWithHealthManagement (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, 
	                                                                      float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{

	}

	public virtual void removeDamagetTargetOverTimeStateWithHealthManagement ()
	{

	}

	public virtual void sedateCharacterithHealthManagement (Vector3 position, float sedateDelay, bool useWeakSpotToReduceDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{

	}

	public virtual void setHealWithHealthManagement (float healAmount)
	{

	}

	public virtual void setShieldWithHealthManagement (float shieldAmount)
	{

	}

	public virtual float getCurrentHealthAmountWithHealthManagement ()
	{
		return 0;
	}

	public virtual float getMaxHealthAmountWithHealthManagement ()
	{
		return 0;
	}

	public virtual float getAuxHealthAmountWithHealthManagement ()
	{
		return 0;
	}

	public virtual void addAuxHealthAmountWithHealthManagement (float amount)
	{

	}

	public virtual float getHealthAmountToPickWithHealthManagement (float amount)
	{
		return 0;
	}

	public virtual void killCharacterWithHealthManagement (GameObject projectile, Vector3 direction, Vector3 position, GameObject attacker, bool damageConstant)
	{

	}

	public virtual void killCharacterWithHealthManagement ()
	{
		
	}

	public virtual Transform getPlaceToShootWithHealthManagement ()
	{
		return null;
	}

	public virtual GameObject getPlaceToShootGameObjectWithHealthManagement ()
	{
		return null;
	}

	public virtual bool isCharacterWithHealthManagement ()
	{
		return false;
	}

	public virtual bool isVehicleWithHealthManagement ()
	{
		return false;
	}

	public virtual List<health.weakSpot> getCharacterWeakSpotListWithHealthManagement ()
	{
		return null;
	}

	public virtual bool isCharacterInRagdollState ()
	{
		return false;
	}

	public virtual Transform getCharacterRootMotionTransform ()
	{
		return null;
	}

	public virtual GameObject getCharacterOrVehicleWithHealthManagement ()
	{
		return null;
	}

	public virtual void setFuelWithHealthManagement (float fuelAmount)
	{

	}

	public virtual void removeFuelWithHealthManagement (float fuelAmount)
	{

	}

	public virtual float getCurrentFuelAmountWithHealthManagement ()
	{
		return 0;
	}

	public virtual bool checkIfMaxFuelWithHealthManagement ()
	{
		return false;
	}

	public virtual GameObject getCharacterWithHealthManagement ()
	{
		return null;
	}

	public virtual GameObject getVehicleWithHealthManagement ()
	{
		return null;
	}

	public virtual vehicleHUDManager getVehicleHUDManagerWithHealthManagement ()
	{
		return null;
	}

	public virtual GameObject getVehicleDriverWithHealthManagement ()
	{
		return null;
	}

	public virtual bool isVehicleBeingDrivenWithHealthManagement ()
	{
		return false;
	}

	public virtual bool checkIfDetectSurfaceBelongToVehicleWithHealthManagement (Collider surfaceFound)
	{
		return false;
	}

	public virtual bool checkIfWeakSpotListContainsTransformWithHealthManagement (Transform transformToCheck)
	{
		return false;
	}

	public virtual void setEnergyWithHealthManagement (float energyAmount)
	{

	}

	public virtual void removeEnergyWithHealthManagement (float energyAmount)
	{

	}

	public virtual float getCurrentEnergyAmountWithHealthManagement ()
	{
		return 0;
	}

	public virtual bool checkIfMaxEnergyWithHealthManagement ()
	{
		return false;
	}

	public virtual int getDecalImpactIndexWithHealthManagement ()
	{
		return -1;
	}

	public virtual void changePlaceToShootPosition (bool state)
	{
		
	}

	public virtual void setPlaceToShootPositionOffset (float newValue)
	{
		
	}

	public virtual bool isDead ()
	{
		return false;
	}

	public virtual void setBlockDamageActiveState (bool state)
	{
		
	}

	public virtual void setBlockDamageProtectionAmount (float newValue)
	{

	}

	public virtual void setBlockDamageRangleAngleState (bool useMaxBlockRangeAngleValue, float maxBlockRangeAngleValue)
	{

	}

	public virtual void setHitReactionBlockIDValue (int newBlockID)
	{
		
	}

	public virtual void updateSliderOffset (float value)
	{
		
	}

	public virtual void updateOriginalSliderOffset ()
	{

	}
}