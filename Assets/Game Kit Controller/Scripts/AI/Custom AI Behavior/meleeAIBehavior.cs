using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeAIBehavior : AIBehaviorInfo
{
	[Header ("Custom Settings")]
	[Space]

	public AIMeleeCombatSystemBrain mainAIMeleeCombatSystemBrain;

	public override void updateAI ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIMeleeCombatSystemBrain.updateAI ();
	}

	public override void updateAIBehaviorState ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIMeleeCombatSystemBrain.updateMainMeleeBehavior ();
	}

	public override void updateAIAttackState (bool canUseAttack)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIMeleeCombatSystemBrain.updateMainMeleeAttack (canUseAttack);
	}

	public override void updateInsideRangeDistance (bool state)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIMeleeCombatSystemBrain.updateInsideMinDistance (state);
	}

	public override void resetBehaviorStates ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIMeleeCombatSystemBrain.resetBehaviorStates ();
	}

	public override void dropWeapon ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIMeleeCombatSystemBrain.dropWeapon ();
	}

	public override void setBehaviorStatesPausedState (bool state)
	{
		mainAIMeleeCombatSystemBrain.setBehaviorStatesPausedState (state);
	}

	public override void setSystemActiveState (bool state)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIMeleeCombatSystemBrain.setCombatSystemActiveState (state);
	}

	public override bool carryingWeapon ()
	{
		if (!behaviorEnabled) {
			return false;
		}

		return mainAIMeleeCombatSystemBrain.isWeaponEquiped ();
	}

	public override void setWaitToActivateAttackActiveState (bool state)
	{
		mainAIMeleeCombatSystemBrain.setWaitToActivateAttackActiveState (state);
	}

	public override void setUseRandomWalkEnabledState (bool state)
	{
		mainAIMeleeCombatSystemBrain.setUseRandomWalkEnabledState (state);
	}

	public override void setOriginalUseRandomWalkEnabledState ()
	{
		mainAIMeleeCombatSystemBrain.setOriginalUseRandomWalkEnabledState ();
	}

	public override bool isAIBehaviorAttackInProcess ()
	{
		return mainAIMeleeCombatSystemBrain.isAIBehaviorAttackInProcess ();
	}
}
