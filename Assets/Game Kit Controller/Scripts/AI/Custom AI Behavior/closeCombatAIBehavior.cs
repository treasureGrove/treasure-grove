using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeCombatAIBehavior : AIBehaviorInfo
{
	[Header ("Custom Settings")]
	[Space]

	public AICloseCombatSystemBrain mainAICloseCombatSystemBrain;

	public override void updateAI ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAICloseCombatSystemBrain.updateAI ();
	}

	public override void updateAIBehaviorState ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAICloseCombatSystemBrain.updateMainCloseCombatBehavior ();
	}

	public override void updateAIAttackState (bool canUseAttack)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAICloseCombatSystemBrain.updateMainCloseCombatAttack (canUseAttack);
	}

	public override void updateInsideRangeDistance (bool state)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAICloseCombatSystemBrain.updateInsideMinDistance (state);
	}

	public override void resetBehaviorStates ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAICloseCombatSystemBrain.resetBehaviorStates ();
	}

	public override void setBehaviorStatesPausedState (bool state)
	{
		mainAICloseCombatSystemBrain.setBehaviorStatesPausedState (state);
	}

	public override void setSystemActiveState (bool state)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAICloseCombatSystemBrain.setCloseCombatSystemActiveState (state);
	}

	public override void setWaitToActivateAttackActiveState (bool state)
	{
		mainAICloseCombatSystemBrain.setWaitToActivateAttackActiveState (state);
	}

	public override void setUseRandomWalkEnabledState (bool state)
	{
		mainAICloseCombatSystemBrain.setUseRandomWalkEnabledState (state);
	}

	public override void setOriginalUseRandomWalkEnabledState ()
	{
		mainAICloseCombatSystemBrain.setOriginalUseRandomWalkEnabledState ();
	}

	public override bool isAIBehaviorAttackInProcess ()
	{
		return mainAICloseCombatSystemBrain.isAIBehaviorAttackInProcess ();
	}
}
