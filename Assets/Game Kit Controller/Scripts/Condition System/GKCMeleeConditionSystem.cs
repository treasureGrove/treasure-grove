using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKCMeleeConditionSystem : GKCConditionInfo
{
	[Header ("Custom Settings")]
	[Space]

	public bool checkIfCarryinWeapon;
	public string weaponCarriedName;

	public bool checkIfWeaponOnHand;

	public bool checkIfWeaponSecondaryAbilityActive;

	public bool checkBothConditionsOnCoroutineUpdate;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool currentConditionCompleteChecked;
	public bool currentConditionNotCompleteChecked;

	grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;

	bool mainGrabbedObjectMeleeAttackSystemLocated;


	public override void checkIfConditionComplete ()
	{
		if (!checkIfPlayerAssigned ()) {
			return;
		}
			
		bool conditionResult = checkConditionResult ();

		setConditionResult (conditionResult);
	}

	public override void checkFunctionOnTriggerExit ()
	{
		base.checkFunctionOnTriggerExit ();


		mainGrabbedObjectMeleeAttackSystem = null;

		mainGrabbedObjectMeleeAttackSystemLocated = false;

		currentConditionNotCompleteChecked = false;

		currentConditionCompleteChecked = false;
	}

	public override void checkConditionOnUpdateState ()
	{
		bool conditionResult = checkConditionResult ();

		if (checkBothConditionsOnCoroutineUpdate) {
			if (conditionResult) {
				if (!currentConditionCompleteChecked) {
					setConditionResult (true);

					currentConditionCompleteChecked = true;

					currentConditionNotCompleteChecked = false;
				}
			} else {
				if (!currentConditionNotCompleteChecked) {
					setConditionResult (false);

					currentConditionNotCompleteChecked = true;

					currentConditionCompleteChecked = false;
				}
			}
		} else {
			if (conditionResult) {
				setConditionResult (true);
			}
		}
	}

	bool checkConditionResult ()
	{
		bool conditionResult = false;

		if (!mainGrabbedObjectMeleeAttackSystemLocated) {
			playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				mainGrabbedObjectMeleeAttackSystem = mainPlayerComponentsManager.getGrabbedObjectMeleeAttackSystem ();

				mainGrabbedObjectMeleeAttackSystemLocated = mainGrabbedObjectMeleeAttackSystem != null;
			}
		}

		if (mainGrabbedObjectMeleeAttackSystemLocated) {
			bool currentConditionState = true;

			if (checkIfCarryinWeapon) {
				if (!mainGrabbedObjectMeleeAttackSystem.getCurrentMeleeWeaponName ().Equals (weaponCarriedName)) {
					currentConditionState = false;
				}
			}

			if (checkIfWeaponOnHand) {
				if (!mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
					currentConditionState = false;
				}
			}

			if (checkIfWeaponSecondaryAbilityActive) {
				if (!mainGrabbedObjectMeleeAttackSystem.isSecondaryAbilityActiveOnCurrentWeapon ()) {
					currentConditionState = false;
				}
			}

			conditionResult = currentConditionState;
		}

		return conditionResult;
	}
}