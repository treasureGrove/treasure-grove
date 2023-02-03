using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class externalControllerBehavior : MonoBehaviour
{
	[Header ("Behavior Main Settings")]
	[Space]

	public string behaviorName;

	public bool externalControllerJumpEnabled;

	[Space]
	[Header ("Disable Other Behaviors Settings")]
	[Space]

	public bool canBeActivatedIfOthersBehaviorsActive;

	public bool disableAllOthersBehaviorsOnActive;

	public List<string> listOfBehaviorToDisableOnActive = new List<string> ();

	[Space]
	[Header ("Action System Settings")]
	[Space]

	public int customActionCategoryID = -1;
	public int regularActionCategoryID = -1;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool behaviorCurrentlyActive;

	public virtual void updateControllerBehavior ()
	{

	}

	public virtual bool isCharacterOnGround ()
	{

		return false;
	}

	public virtual bool isBehaviorActive ()
	{

		return false;
	}

	public virtual void setExternalForceActiveState (bool state)
	{
		
	}

	public virtual void setExternalForceEnabledState (bool state)
	{

	}

	public virtual void updateExternalForceActiveState (Vector3 forceDirection, float forceAmount)
	{

	}

	public virtual void checkIfActivateExternalForce ()
	{

	}

	public virtual void setJumpActiveForExternalForce ()
	{

	}

	public virtual void setExtraImpulseForce (Vector3 forceAmount, bool useCameraDirection)
	{
		
	}

	public virtual void disableExternalControllerState ()
	{
		
	}

	public virtual void checkIfResumeExternalControllerState ()
	{

	}

	public virtual bool checkIfCanEnableBehavior (string behaviorName)
	{
		if (behaviorName != "") {
			if (disableAllOthersBehaviorsOnActive) {
				return true;
			} else {
				if (listOfBehaviorToDisableOnActive.Contains (behaviorName)) {
					return true;
				} else {
					return false;
				}
			}
		} else {
			return true;
		}
	}

	public virtual void setBehaviorCurrentlyActiveState (bool state)
	{
		behaviorCurrentlyActive = state;
	}

	public virtual void setCurrentPlayerActionSystemCustomActionCategoryID ()
	{
		
	}

	public virtual void checkPauseStateDuringExternalForceOrBehavior ()
	{

	}

	public virtual void checkResumeStateAfterExternalForceOrBehavior ()
	{
		
	}
}
