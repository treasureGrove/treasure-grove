using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freezeCharacterState : characterStateAffectedInfo
{
	[Header ("Custom Settings")]
	[Space]

	public playerController mainPlayerController;

	public int pauseCharacterPriority = 2;

	public override void activateStateAffected (bool state)
	{
		GKC_Utils.pauseOrResumeCharacter (state, mainPlayerController, pauseCharacterPriority);
	}


}
