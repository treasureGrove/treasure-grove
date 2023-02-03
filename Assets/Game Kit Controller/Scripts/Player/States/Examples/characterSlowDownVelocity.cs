using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterSlowDownVelocity : characterStateAffectedInfo
{
	[Header ("Custom Settings")]
	[Space]

	public bool animationSpeedCanBeChanged = true;

	public playerController mainPlayerController;

	public override void activateStateAffected (float stateDuration, float stateAmount)
	{
		if (animationSpeedCanBeChanged) {
			if (mainPlayerController.usedByAI) {
				mainPlayerController.setNewAnimSpeedMultiplierDuringXTime (stateDuration);
				mainPlayerController.setReducedVelocity (stateAmount);

				if (stateAmount < 0.09f) {
					mainPlayerController.setCanMoveAIState (false);
				}
			}
		}
	}
}
