using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animalAINavmesh : AINavMesh
{
	[Space]
	[Header ("AI Navmesh Custom Settings")]
	[Space]

	public inputActionManager mainInputActionManager;

	Vector2 moveInput;

	public override void updateAIControllerInputValues ()
	{
		moveInput = new Vector2 (AIMoveInput.moveInput.x, AIMoveInput.moveInput.z);

		mainInputActionManager.overrideInputValues (moveInput, -1, 1, true);

		playerControllerManager.Move (AIMoveInput);
//
//		playerCameraManager.Rotate (rayCastPosition.forward);

		//remove once find objectives is configured on the vehicle AI
		setOnGroundState (playerControllerManager.isPlayerOnGround ());
	}

	public override void updateAICameraInputValues ()
	{

	}

	public void disableOverrideInputValues ()
	{
		mainInputActionManager.overrideInputValues (Vector2.zero, -1, 1, false);
	}
}
