using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerGravitySystem : abilityInfo
{
	[Header ("Custom Settings")]
	[Space]

	public bool gravitySystemEnabled = true;

	public gravitySystem mainGravitySystem;

	public override void updateAbilityState ()
	{

	}

	public override void enableAbility ()
	{

	}

	public override void disableAbility ()
	{
		gravitySystemEnabled = false;
	}

	public override void deactivateAbility ()
	{

	}

	public override void activateSecondaryActionOnAbility ()
	{

	}

	public override void useAbilityPressDown ()
	{



		checkUseEventOnUseAbility ();
	}

	public override void useAbilityPressHold ()
	{

	}

	public override void useAbilityPressUp ()
	{

	}
}
