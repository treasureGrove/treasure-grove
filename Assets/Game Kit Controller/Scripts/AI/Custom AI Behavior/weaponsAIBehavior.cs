using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponsAIBehavior : AIBehaviorInfo
{
	[Header ("Custom Settings")]
	[Space]

	public AIFireWeaponsSystemBrain mainAIFireWeaponsSystemBrain;

	public override void updateAI ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.updateAI ();
	}

	public override void updateAIBehaviorState ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.updateMainFireWeaponsBehavior ();
	}

	public override void updateAIAttackState (bool canUseAttack)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.updateMainFireWeaponsAttack (canUseAttack);
	}

	public override void updateInsideRangeDistance (bool state)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.updateInsideMinDistance (state);
	}

	public override void resetBehaviorStates ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.resetBehaviorStates ();
	}

	public override void dropWeapon ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.dropWeapon ();
	}

	public override void resetAttackState ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.resetAttackState ();
	}

	public override void stopAim ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.stopAim ();
	}

	public override void checkIfDrawWeaponsWhenResumingAI ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.checkIfDrawWeaponsWhenResumingAI ();
	}

	public override void disableOnSpottedState ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.disableOnSpottedState ();
	}

	public override void updateWeaponsAvailableState ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.updateWeaponsAvailableState ();
	}

	public override void setBehaviorStatesPausedState (bool state)
	{
		mainAIFireWeaponsSystemBrain.setBehaviorStatesPausedState (state);
	}

	public override void setSystemActiveState (bool state)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIFireWeaponsSystemBrain.setWeaponsSystemActiveState (state);
	}

	public override bool carryingWeapon ()
	{
		if (!behaviorEnabled) {
			return false;
		}

		return mainAIFireWeaponsSystemBrain.isWeaponEquiped ();
	}

	public override void setWaitToActivateAttackActiveState (bool state)
	{
		mainAIFireWeaponsSystemBrain.setWaitToActivateAttackActiveState (state);
	}

	public override void setUseRandomWalkEnabledState (bool state)
	{
		mainAIFireWeaponsSystemBrain.setUseRandomWalkEnabledState (state);
	}

	public override void setOriginalUseRandomWalkEnabledState ()
	{
		mainAIFireWeaponsSystemBrain.setOriginalUseRandomWalkEnabledState ();
	}
}
