using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class characterDamageReceiver : healthManagement
{
	[Header ("Main Setting")]
	[Space]

	[Range (1, 20)] public float damageMultiplier = 1;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject character;
	public health healthManager;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool characterAssigned;

	//this script is added to every collider in a vehicle, so when a projectile hits the vehicle, its health component receives the damge
	//like this the damage detection is really accurated.
	//the function sends the amount of damage, the direction of the projectile, the position where hits, the object that fired the projectile,
	//and if the damaged is done just once, like a bullet, or the damaged is constant like a laser

	//health and damage management
	public void setDamage (float amount, Vector3 fromDirection, Vector3 damagePos, GameObject bulletOwner, GameObject projectile, 
	                       bool damageConstant, bool searchClosestWeakSpot, bool ignoreDamageInScreen, bool damageCanBeBlocked,
	                       bool canActivateReactionSystemTemporally, int damageReactionID, int damageTypeID)
	{
		healthManager.setDamage ((amount * damageMultiplier), fromDirection, damagePos, bulletOwner, projectile, damageConstant, 
			searchClosestWeakSpot, false, ignoreDamageInScreen, damageCanBeBlocked, canActivateReactionSystemTemporally, 
			damageReactionID, damageTypeID);
	}

	public void setHeal (float amount)
	{
		healthManager.getHealth (amount);
	}

	public override float getCurrentHealthAmount ()
	{
		return healthManager.getCurrentHealthAmount ();
	}

	public float getMaxHealthAmount ()
	{
		return healthManager.getMaxHealthAmount ();
	}

	public float getAuxHealthAmount ()
	{
		return healthManager.getAuxHealthAmount ();
	}

	public void addAuxHealthAmount (float amount)
	{
		healthManager.addAuxHealthAmount (amount);
	}

	public float getHealthAmountToLimit ()
	{
		return healthManager.getHealthAmountToLimit ();
	}

	//kill character
	public void killCharacter (GameObject projectile, Vector3 direction, Vector3 position, GameObject attacker, bool damageConstant)
	{
		healthManager.killCharacter (direction, position, attacker, projectile, damageConstant);
	}

	//impact decal management
	public int getDecalImpactIndex ()
	{
		return healthManager.getDecalImpactIndex ();
	}

	public bool isCharacterDead ()
	{
		return healthManager.dead;
	}

	//set character component
	public void setCharacter (GameObject characterGameObject, health currentHealth)
	{
		character = characterGameObject;

		healthManager = currentHealth;

		characterAssigned = true;

		if (!GKC_Utils.isApplicationPlaying ()) {
			updateComponent ();
		}
	}

	public void setDamageTargetOverTimeState (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{
		healthManager.setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath, damageTypeID);
	}

	public void removeDamagetTargetOverTimeState ()
	{
		healthManager.stopDamageOverTime ();
	}

	public void sedateCharacter (Vector3 position, float sedateDelay, bool useWeakSpotToReduceDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{
		healthManager.sedateCharacter (position, sedateDelay, useWeakSpotToReduceDelay, sedateUntilReceiveDamage, sedateDuration);
	}

	public health getHealthManager ()
	{
		return healthManager;
	}

	void OnCollisionEnter (Collision col)
	{
		if (characterAssigned && !healthManager.isDead ()) {
			if (healthManager.canRagdollReceiveDamageOnImpact ()) {
				healthManager.setRagdollImpactReceivedInfo (col.relativeVelocity, col.collider);
			}

			if (healthManager.isReceiveDamageFromCollisionsEnabled ()) {
				healthManager.setImpactReceivedInfo (col.relativeVelocity);
			}
		}
	}

	public float getShieldAmountToLimit ()
	{
		if (healthManager.useShield) {
			return healthManager.getShieldSystem ().getShieldAmountToLimit ();
		} else {
			return -1;
		}
	}

	public void addAuxShieldAmount (float amount)
	{
		if (healthManager.useShield) {
			healthManager.getShieldSystem ().addAuxShieldAmount (amount);
		}
	}

	public void setShield (float amount)
	{
		healthManager.setShield (amount);
	}

	public bool characterIsOnRagdollState ()
	{
		return healthManager.characterIsOnRagdollState ();
	}

	public Transform getTransformToAttachWeaponsByClosestPosition (Vector3 positionToCheck)
	{
		return healthManager.getTransformToAttachWeaponsByClosestPosition (positionToCheck);
	}

	//Override functions from Health Management
	public override void setDamageWithHealthManagement (float damageAmount, Vector3 fromDirection, Vector3 damagePos, GameObject attacker, 
	                                                    GameObject projectile, bool damageConstant, bool searchClosestWeakSpot, bool ignoreShield, 
	                                                    bool ignoreDamageInScreen, bool damageCanBeBlocked, bool canActivateReactionSystemTemporally, 
	                                                    int damageReactionID, int damageTypeID)
	{
		healthManager.setDamage ((damageAmount * damageMultiplier), fromDirection, damagePos, attacker, projectile,
			damageConstant, searchClosestWeakSpot, false, ignoreDamageInScreen, damageCanBeBlocked, 
			canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
	}

	public override bool checkIfDeadWithHealthManagement ()
	{
		return healthManager.isDead ();
	}

	public override bool checkIfMaxHealthWithHealthManagement ()
	{
		return healthManager.checkIfMaxHealth ();
	}

	public override void setDamageTargetOverTimeStateWithHealthManagement (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, 
	                                                                       float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{
		healthManager.setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath, damageTypeID);
	}

	public override void removeDamagetTargetOverTimeStateWithHealthManagement ()
	{
		healthManager.stopDamageOverTime ();
	}

	public override void sedateCharacterithHealthManagement (Vector3 position, float sedateDelay, bool useWeakSpotToReduceDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{
		healthManager.sedateCharacter (position, sedateDelay, useWeakSpotToReduceDelay, sedateUntilReceiveDamage, sedateDuration);
	}

	public override void setHealWithHealthManagement (float healAmount)
	{
		healthManager.getHealth (healAmount);
	}

	public override void setShieldWithHealthManagement (float shieldAmount)
	{
		healthManager.setShield (shieldAmount);
	}

	public override float getCurrentHealthAmountWithHealthManagement ()
	{
		return healthManager.getCurrentHealthAmount ();
	}

	public override float getMaxHealthAmountWithHealthManagement ()
	{
		return healthManager.getMaxHealthAmount ();
	}

	public override float getAuxHealthAmountWithHealthManagement ()
	{
		return healthManager.getAuxHealthAmount ();
	}

	public override void addAuxHealthAmountWithHealthManagement (float amount)
	{
		healthManager.addAuxHealthAmount (amount);
	}

	public override float getHealthAmountToPickWithHealthManagement (float amount)
	{
		return healthManager.getHealthAmountToPick (amount);
	}

	public override void killCharacterWithHealthManagement (GameObject projectile, Vector3 direction, Vector3 position, GameObject attacker, bool damageConstant)
	{
		healthManager.killCharacter (direction, position, attacker, projectile, damageConstant);
	}

	public override void killCharacterWithHealthManagement ()
	{
		healthManager.killCharacter ();
	}

	public override Transform getPlaceToShootWithHealthManagement ()
	{
		return healthManager.getPlaceToShoot ();
	}

	public override GameObject getPlaceToShootGameObjectWithHealthManagement ()
	{
		return healthManager.getPlaceToShoot ().gameObject;
	}

	public override bool isCharacterWithHealthManagement ()
	{
		return true;
	}

	public override List<health.weakSpot> getCharacterWeakSpotListWithHealthManagement ()
	{
		return healthManager.advancedSettings.weakSpots;
	}

	public override GameObject getCharacterOrVehicleWithHealthManagement ()
	{
		return character;
	}

	public override GameObject getCharacterWithHealthManagement ()
	{
		return character;
	}

	public override bool checkIfWeakSpotListContainsTransformWithHealthManagement (Transform transformToCheck)
	{
		return healthManager.checkIfWeakSpotListContainsTransform (transformToCheck);
	}

	public override int getDecalImpactIndexWithHealthManagement ()
	{
		return healthManager.getDecalImpactIndex ();
	}

	public override bool isCharacterInRagdollState ()
	{
		return characterIsOnRagdollState ();
	}

	public override Transform getCharacterRootMotionTransform ()
	{
		return healthManager.getCharacterRootMotionTransform ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Damage Receiver", gameObject);
	}
}