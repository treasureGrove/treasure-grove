using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKCCharacterController : playerController
{
	[Space]
	[Header ("Custom Settings")]
	[Space]

	public customCharacterControllerBase mainCustomCharacterControllerBase;

	bool characterInitialized;

	Vector2 customAxisValues;
	Vector2 customRawAxisValues;

	void OnEnable ()
	{
		if (characterInitialized) {
			return;
		}
			
		StartCoroutine (startGameWithCustomCharacterControllerCoroutine ());

		characterInitialized = true;
	}

	IEnumerator startGameWithCustomCharacterControllerCoroutine ()
	{
		yield return new WaitForSeconds (0.3f);

		mainCustomCharacterControllerBase.setCharacterControllerActiveState (true);

		mainCustomCharacterControllerBase.updateOnGroundValue (isPlayerOnGround ());

		setCustomCharacterControllerActiveState (true, mainCustomCharacterControllerBase);

		animator.runtimeAnimatorController = mainCustomCharacterControllerBase.originalAnimatorController;
		animator.avatar = mainCustomCharacterControllerBase.originalAvatar;

		if (mainCustomCharacterControllerBase.setCapsuleColliderValues) {
			setPlayerColliderCapsuleScale (mainCustomCharacterControllerBase.capsuleColliderHeight);

			setPlayerCapsuleColliderDirection (mainCustomCharacterControllerBase.capsuleColliderDirection);

			setPlayerColliderCapsuleCenter (mainCustomCharacterControllerBase.capsuleColliderCenter);

			setPlayerCapsuleColliderRadius (mainCustomCharacterControllerBase.capsuleColliderRadius);
		}

		setCharacterMeshGameObjectState (false);

		setCharacterMeshesListToDisableOnEventState (false);
	}

	public override void setCustomAxisValues (Vector2 newValue)
	{
		customAxisValues = newValue;

		if (customAxisValues.x > 0) {
			customRawAxisValues.x = 1;
		} else if (customAxisValues.x < 0) {
			customRawAxisValues.x = -1;
		} else {
			customRawAxisValues.x = 0;
		}

		if (customAxisValues.y > 0) {
			customRawAxisValues.y = 1;
		} else if (customAxisValues.y < 0) {
			customRawAxisValues.y = -1;
		} else {
			customRawAxisValues.y = 0;
		}
	}

	public override void setMainAxisValues ()
	{
		axisValues = customAxisValues;
	}

	public override void setMainRawAxisValues ()
	{
		rawAxisValues = customRawAxisValues;
	}

	public override void setAIMainAxisValues ()
	{
		axisValues = customAxisValues;
	}

	public override void setMainAIRawAxisValues ()
	{
		rawAxisValues = customRawAxisValues;
	}

	public override void updateOverrideInputValues (Vector2 inputValues, bool state)
	{
//		playerInput.overrideInputValues (inputValues, state);
	}
}
