using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powersAIBehavior : AIBehaviorInfo
{
	[Header ("Custom Settings")]
	[Space]

	public AIPowersSystemBrain mainAIPowersSystemBrain;

	public override void updateAI ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIPowersSystemBrain.updateAI ();
	}

	public override void updateAIBehaviorState ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIPowersSystemBrain.updateBehavior ();
	}

	public override void updateAIAttackState (bool canUseAttack)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIPowersSystemBrain.updateAIAttackState (canUseAttack);
	}

	public override void updateInsideRangeDistance (bool state)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIPowersSystemBrain.updateInsideMinDistance (state);
	}

	public override void resetBehaviorStates ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIPowersSystemBrain.resetBehaviorStates ();
	}

	public override void resetAttackState ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIPowersSystemBrain.resetAttackState ();
	}

	public override void stopAim ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIPowersSystemBrain.stopAim ();
	}

	public override void disableOnSpottedState ()
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIPowersSystemBrain.disableOnSpottedState ();
	}

	public override void setBehaviorStatesPausedState (bool state)
	{
		mainAIPowersSystemBrain.setBehaviorStatesPausedState (state);
	}

	public override void setSystemActiveState (bool state)
	{
		if (!behaviorEnabled) {
			return;
		}

		mainAIPowersSystemBrain.setSystemActiveState (state);
	}

	public override void setWaitToActivateAttackActiveState (bool state)
	{
		mainAIPowersSystemBrain.setWaitToActivateAttackActiveState (state);
	}
}
