using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class meleeWeaponInfo
{
	[Space]
	[Header ("Movements Settings")]
	[Space]

	public bool setNewMovementValues;

	[Space]
	[Space]

	public bool setNewIdleID;
	public int idleIDUsed;

	public bool useStrafeMode;
	public int strafeIDUsed;

	public bool activateStrafeModeOnLockOnTargetActive;

	[Space]

	public bool toggleStrafeModeIfRunningActive;

	public bool setSprintEnabledStateWithWeapon;
	public bool sprintEnabledStateWithWeapon;

	[Space]

	public bool setNewCrouchID;
	public int crouchIDUsed;

	[Space]

	public bool setNewMovementID;
	public int movementIDUsed;

	[Space]
	[Header ("Attack Settings")]
	[Space]

	public List<grabPhysicalObjectMeleeAttackSystem.attackInfo> mainAttackInfoList = new List<grabPhysicalObjectMeleeAttackSystem.attackInfo> ();
}
