using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class customCharacterControllerBase : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool characterControllerEnabled = true;

	public float animatorForwardInputLerpSpeed = 0.1f;
	public float animatorTurnInputLerpSpeed = 0.1f;

	public float forwardAmountMultiplier = 1;

	public bool updateForwardAmountInputValueFromPlayerController;

	public bool updateTurnAmountInputValueFromPlayerController;

	public bool updateUsingInputValueFromPlayerController;

	public float placeToShootOffset;

	[Space]
	[Header ("Collider Settings")]
	[Space]

	public bool setCapsuleColliderValues;

	public Vector3 capsuleColliderCenter;
	public float capsuleColliderRadius;
	public float capsuleColliderHeight;
	public int capsuleColliderDirection = 0;

	[Space]
	[Header ("Animator Settings")]
	[Space]

	public Animator mainAnimator;
	public RuntimeAnimatorController originalAnimatorController;

	public Avatar originalAvatar;

	public Vector3 charactetPositionOffset;

	public Vector3 characterRotationOffset;

	[Space]
	[Header ("Root Motion Settings")]
	[Space]

	public bool useRootMotion = true;

	public float noRootWalkMovementSpeed;
	public float noRootRunMovementSpeed;
	public float noRootSprintMovementSpeed;
	public float noRootCrouchMovementSpeed;
	public float noRootWalkStrafeMovementSpeed;
	public float noRootRunStrafeMovementSpeed;

	[Space]
	[Header ("Camera Settings")]
	[Space]

	public bool setNewCameraStates;

	public string newCameraStateThirdPerson;
	public string newCameraStateFirstPerson;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public int customActionCategoryID;
	public float characterRadius;
	public int combatTypeID;
	public Transform parentForCombatDamageTriggers;
	public string customRagdollInfoName;
	public string playerModeName = "Combat";

	public float healthBarOffset;

	[Space]
	[Header ("AI Visibility")]
	[Space]

	public bool hiddenFromAIAttacks;

	[Space]
	[Header ("AI Settings")]
	[Space]

	public bool setAIValues;
	public float maxRandomTimeBetweenAttacks;
	public float minRandomTimeBetweenAttacks;
	public float newMinDistanceToEnemyUsingCloseCombat;
	public float newMinDistanceToCloseCombat;

	public float raycastPositionOffset = 1;

	[Space]
	[Header ("Other Components")]
	[Space]

	public List<GameObject> characterMeshesList = new List<GameObject> ();
	public GameObject characterGameObject;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool characterControllerActive;

	public float horizontalInput;
	public float verticalInput;

	public bool playerUsingInput;

	public Vector3 movementInput;

	public float forwardAmount;
	public float turnAmount;

	public bool onGround;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStateChange;

	[Space]

	public UnityEvent eventOnCharacterControllerActivated;
	public UnityEvent eventOnCharacterControllerDeactivated;

	public bool useEventToSendCharacterObjectOnStart;
	public eventParameters.eventToCallWithGameObject eventToSendCharacterObjectOnStart;
	public eventParameters.eventToCallWithGameObject eventToSendCharacterObjectOnEnd;

	public UnityEvent eventOnInstantiateCustomCharacterObject;

	public virtual void updateCharacterControllerState ()
	{

	}

	public virtual void updateCharacterControllerAnimator ()
	{

	}

	public virtual void updateMovementInputValues (Vector3 newValues)
	{
		movementInput = newValues;
	}

	public virtual void updateHorizontalVerticalInputValues (Vector2 newValues)
	{
		horizontalInput = newValues.x;
		verticalInput = newValues.y;
	}

	public virtual void activateJumpAnimatorState ()
	{

	}

	public virtual void updateForwardAmountInputValue (float newValue)
	{
		forwardAmount = newValue * forwardAmountMultiplier;
	}

	public virtual void updateTurnAmountInputValue (float newValue)
	{
		turnAmount = newValue;
	}

	public virtual void updateOnGroundValue (bool state)
	{
		onGround = state;
	}

	public virtual void updatePlayerUsingInputValue (bool state)
	{
		playerUsingInput = state;
	}

	public virtual void setForwardAmountMultiplierValue (float newValue)
	{
		forwardAmountMultiplier = newValue;
	}

	public virtual void resetAnimatorState ()
	{

	}

	public virtual void setCharacterControllerActiveState (bool state)
	{
		if (!characterControllerEnabled) {
			return;
		}

		characterControllerActive = state;

		checkEventsOnStateChange (characterControllerActive);

		for (int i = 0; i < characterMeshesList.Count; i++) {
			if (characterMeshesList [i] != null) {
				if (characterMeshesList [i].activeSelf != state) {
					characterMeshesList [i].SetActive (state);
				}
			}
		}
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				eventOnCharacterControllerActivated.Invoke ();
			} else {
				eventOnCharacterControllerDeactivated.Invoke ();
			}
		}

		if (useEventToSendCharacterObjectOnStart) {
			if (state) {
				eventToSendCharacterObjectOnStart.Invoke (characterGameObject);
			} else {
				eventToSendCharacterObjectOnEnd.Invoke (characterGameObject);
			}
		}
	}

	public void updateAnimatorFloatValue (int animatorIDValue, float floatValue)
	{
		mainAnimator.SetFloat (animatorIDValue, floatValue);
	}

	public void updateAnimatorIntegerValue (int animatorIDValue, int intValue)
	{
		mainAnimator.SetInteger (animatorIDValue, intValue);
	}

	public void updateAnimatorBoolValue (int animatorIDValue, bool boolValue)
	{
		mainAnimator.SetBool (animatorIDValue, boolValue);
	}

	public void updateAnimatorFloatValueLerping (int animatorIDValue, float floatValue, float  animatorLerpSpeed, float currentFixedUpdateDeltaTime)
	{
		mainAnimator.SetFloat (animatorIDValue, floatValue, animatorLerpSpeed, currentFixedUpdateDeltaTime);
	}

	//Collider Settings
	public void setCapsuleColliderDirectionValue (float newValue)
	{
		capsuleColliderDirection = (int)newValue;
	}

	public void setCapsuleColliderCenterValue (Vector3 newValue)
	{
		capsuleColliderCenter = newValue;
	}

	public void setCapsuleColliderRadiusValue (float newValue)
	{
		capsuleColliderRadius = newValue;
	}

	public void setCapsuleColliderHeightValue (float newValue)
	{
		capsuleColliderHeight = newValue;
	}

	public void setPlaceToShootOffsetValue (float newValue)
	{
		placeToShootOffset = newValue;
	}

	//Animator Settings
	public void setCharactetPositionOffsetValue (Vector3 newValue)
	{
		charactetPositionOffset = newValue;
	}

	public void setCharacterRotationOffsetValue (Vector3 newValue)
	{
		characterRotationOffset = newValue;
	}
		
	//Root Motion Settings
	public void setUseRootMotionValue (bool state)
	{
		useRootMotion = state;
	}

	public void setNoRootWalkMovementSpeedValue (float newValue)
	{
		noRootWalkMovementSpeed = newValue;
	}

	public void setNoRootRunMovementSpeedValue (float newValue)
	{
		noRootRunMovementSpeed = newValue;
	}

	public void setNoRootSprintMovementSpeedValue (float newValue)
	{
		noRootSprintMovementSpeed = newValue;
	}

	public void setNoRootCrouchMovementSpeedValue (float newValue)
	{
		noRootCrouchMovementSpeed = newValue;
	}

	public void setNoRootWalkStrafeMovementSpeedValue (float newValue)
	{
		noRootWalkStrafeMovementSpeed = newValue;
	}

	public void setNoRootRunStrafeMovementSpeedValue (float newValue)
	{
		noRootRunStrafeMovementSpeed = newValue;
	}

	//Camera Settings
	public void setSetNewCameraStatesValue (bool state)
	{
		setNewCameraStates = state;
	}

	public void setNewCameraStateThirdPersonValue (string newValue)
	{
		newCameraStateThirdPerson = newValue;
	}

	public void setNewCameraStateFirstPersonValue (string newValue)
	{
		newCameraStateFirstPerson = newValue;
	}



	//Other Settings
	public void setCustomActionCategoryIDValue (float newValue)
	{
		customActionCategoryID = (int)newValue;
	}

	public void setCharacterRadiusValue (float newValue)
	{
		characterRadius = newValue;
	}

	public void setCombatTypeIDValue (float newValue)
	{
		combatTypeID = (int)newValue;
	}

	public void setCustomRagdollInfoNameValue (string newValue)
	{
		customRagdollInfoName = newValue;
	}

	public void setPlayerModeNameValue (string newValue)
	{
		playerModeName = newValue;
	}

	public void setHealthBarOffsetValue (float newValue)
	{
		healthBarOffset = newValue;
	}

	//AI Visibility
	public void setHiddenFromAIAttacksValue (bool state)
	{
		hiddenFromAIAttacks = state;
	}


	//AI Settings
	public void setSetAIValuesValue (bool state)
	{
		setAIValues = state;
	}

	public void setMaxRandomTimeBetweenAttacksValue (float newValue)
	{
		maxRandomTimeBetweenAttacks = newValue;
	}

	public void setMinRandomTimeBetweenAttacksValue (float newValue)
	{
		minRandomTimeBetweenAttacks = newValue;
	}

	public void setNewMinDistanceToEnemyUsingCloseCombatValue (float newValue)
	{
		newMinDistanceToEnemyUsingCloseCombat = newValue;
	}

	public void setNewMinDistanceToCloseCombatValue (float newValue)
	{
		newMinDistanceToCloseCombat = newValue;
	}

	public void setRaycastPositionOffsetValue (float newValue)
	{
		raycastPositionOffset = newValue;
	}
}
