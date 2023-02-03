using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class templateAbilitySystem : abilityInfo
{
	public override void updateAbilityState ()
	{
		
	}
		
	public override void enableAbility ()
	{

	}

	public override void disableAbility ()
	{
		
	}

	public override void deactivateAbility ()
	{
		
	}

	public override void activateSecondaryActionOnAbility ()
	{

	}

	public override void useAbilityPressDown ()
	{
		//base.useAbilityPressDown ();


		checkUseEventOnUseAbility ();
	}

	public override void useAbilityPressHold ()
	{

	}

	public override void useAbilityPressUp ()
	{
		
	}
}
