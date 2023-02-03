using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AINavMeshMoveInfo
{
	public Vector3 moveInput;
	public bool crouchInput;
	public bool jumpInput;
	public bool lookAtTarget;

	public bool strafeModeActive;
	public bool AIEnableInputMovementOnStrafe;
}
