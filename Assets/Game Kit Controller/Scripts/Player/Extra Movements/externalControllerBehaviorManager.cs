using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class externalControllerBehaviorManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public List<externalControllerInfo> externalControllerInfoList = new List<externalControllerInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Components")]
	[Space]

	public playerInputManager mainPlayerInputManager;
	public playerWeaponsManager mainPlayerWeaponsManager;

	public Transform playerTransform;

	bool carryingWeaponsPreviously;
	bool aimingWeaponsPrevously;


	public void setExternalControllerActive (string nameToCheck)
	{
		setExternalControllerState (nameToCheck, true);	
	}

	public void setExternalControllerDeactive (string nameToCheck)
	{
		setExternalControllerState (nameToCheck, false);	
	}

	public void setExternalControllerState (string nameToCheck, bool state)
	{
		if (nameToCheck == null || nameToCheck == "") {
			return;
		}

		if (showDebugPrint) {
			print ("External Controller state  " + nameToCheck + " " + state);
		}

		for (int i = 0; i < externalControllerInfoList.Count; i++) {
			externalControllerInfo currentInfo = externalControllerInfoList [i];
				
			if (currentInfo.Name.Equals (nameToCheck)) {
				if (showDebugPrint) {
					print (nameToCheck + " state found");
				}

				checkInputListToPauseDuringAction (currentInfo.customInputToPauseOnActionInfoList, state);

				if (currentInfo.useEventsOnStateChange) {
					if (state) {
						currentInfo.eventOnStateActive.Invoke ();
					} else {
						currentInfo.eventOnStatDeactive.Invoke ();
					}
				}

				if (state) {
					carryingWeaponsPreviously = mainPlayerWeaponsManager.isPlayerCarringWeapon ();

					aimingWeaponsPrevously = mainPlayerWeaponsManager.isAimingWeapons ();

					if (carryingWeaponsPreviously) {
						if (mainPlayerWeaponsManager.isCharacterShooting ()) {
							mainPlayerWeaponsManager.resetWeaponFiringAndAimingIfPlayerDisabled ();

							mainPlayerWeaponsManager.setHoldShootWeaponState (false);
						}

						if (aimingWeaponsPrevously) {
							mainPlayerWeaponsManager.setAimWeaponState (false);
						}

						if (currentInfo.keepWeaponsDuringAction) {

							mainPlayerWeaponsManager.checkIfDisableCurrentWeapon ();

							mainPlayerWeaponsManager.resetWeaponHandIKWeight ();
						} else if (currentInfo.disableIKWeaponsDuringAction) {
							mainPlayerWeaponsManager.enableOrDisableIKOnWeaponsDuringAction (false);
						}
					}

					if (currentInfo.keepMeleeWeaponGrabbed) {
						GKC_Utils.keepMeleeWeaponGrabbed (playerTransform.gameObject);
					}
				} else {
					if (carryingWeaponsPreviously) {
						if (currentInfo.drawWeaponsAfterAction) {
							mainPlayerWeaponsManager.checkIfDrawSingleOrDualWeapon ();
						} else if (currentInfo.disableIKWeaponsDuringAction) {
							mainPlayerWeaponsManager.enableOrDisableIKOnWeaponsDuringAction (true);
						}

						carryingWeaponsPreviously = false;
					}

					if (currentInfo.drawMeleeWeaponGrabbedOnActionEnd) {
						GKC_Utils.drawMeleeWeaponGrabbed (playerTransform.gameObject);
					}
				}

				return;
			}
		}
	}

	public void checkInputListToPauseDuringAction (List<playerActionSystem.inputToPauseOnActionIfo> inputList, bool state)
	{
		for (int i = 0; i < inputList.Count; i++) {
			if (state) {
				inputList [i].previousActiveState = mainPlayerInputManager.setPlayerInputMultiAxesStateAndGetPreviousState (false, inputList [i].inputName);
			} else {
				if (inputList [i].previousActiveState) {
					mainPlayerInputManager.setPlayerInputMultiAxesState (inputList [i].previousActiveState, inputList [i].inputName);
				}
			}
		}
	}

	[System.Serializable]
	public class externalControllerInfo
	{
		public string Name;

		public externalControllerBehavior mainExternalControllerBehavior;

		[Space]
		[Space]

		public bool keepWeaponsDuringAction = true;

		public bool disableIKWeaponsDuringAction = true;

		public bool drawWeaponsAfterAction;

		public bool keepMeleeWeaponGrabbed = true;

		public bool drawMeleeWeaponGrabbedOnActionEnd = true;

		[Space]
		[Space]

		public List<playerActionSystem.inputToPauseOnActionIfo> customInputToPauseOnActionInfoList = new List<playerActionSystem.inputToPauseOnActionIfo> ();
	
		[Space]
		[Space]

		public bool useEventsOnStateChange;
		public UnityEvent eventOnStateActive;
		public UnityEvent eventOnStatDeactive;
	}
}
