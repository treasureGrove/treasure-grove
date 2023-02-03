using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class meleeWeaponRangeAttacksManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool rangeAttacksActive;

	public float aimingActiveBodyWeight = 0.5f;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public int currentRangeWeaponID;

	public int currentRangeWeaponInfo;

	public bool aimingActive;

	[Space]
	[Header ("Range Weapon Types List")]
	[Space]

	public List<rangeWeaponSystemInfo> rangeWeaponSystemInfoList = new List<rangeWeaponSystemInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnAim;
	public UnityEvent eventOnStartAiming;
	public UnityEvent eventOnStopAiming;

	public bool callEventsOnDisableRangeAttacksState;

	[Space]
	[Header ("Components")]
	[Space]

	public grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;
	public playerCamera mainPlayerCamera;
	public playerController mainPlayerController;
	public headTrack mainHeadTrack;

	meleeWeaponRangeAttacksSystem currentMeleeWeaponRangeAttacksSystem;

	rangeWeaponSystemInfo currentRangeWeaponSystemInfo;

	simpleWeaponSystem currentSimpleWeaponSystem;

	bool currentSimpleWeaponSystemAssigned;

	bool firingCurrentSimpleWeaponSystem;


	public void setMeleeWeaponRangeAttackManagerActiveState (bool state)
	{
		if (rangeAttacksActive == state) {
			return;
		}

		rangeAttacksActive = state;

		if (rangeAttacksActive) {
			Transform currentGrabbedObjectTransform = mainGrabbedObjectMeleeAttackSystem.getCurrentGrabbedObjectTransform ();

			if (currentGrabbedObjectTransform == null) {

				return;
			}

			currentMeleeWeaponRangeAttacksSystem = currentGrabbedObjectTransform.GetComponent<meleeWeaponRangeAttacksSystem> ();

			currentRangeWeaponInfo = 0;

			currentRangeWeaponID = currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList [currentRangeWeaponInfo].rangeWeaponID;

			setCurrentRangeWeaponSystemByID (currentRangeWeaponID);

			currentSimpleWeaponSystem.setCustomprojectilePosition (currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList [currentRangeWeaponInfo].projectilePosition);
		} else {
			if (aimingActive || firingCurrentSimpleWeaponSystem) {
				forceStopFireCurrentRangeWeapon ();
			}

			if (currentSimpleWeaponSystem != null) {
				currentSimpleWeaponSystem.enabled = false;
			}

			aimingActive = false;

			if (callEventsOnDisableRangeAttacksState) {
				checkEventsOnStateChange (false);
			}
		}
	}

	public void setCurrentRangeWeaponSystemByID (int rangeWeaponID)
	{
		for (int i = 0; i < rangeWeaponSystemInfoList.Count; i++) {
			if (rangeWeaponSystemInfoList [i].rangeWeaponID == rangeWeaponID) {
				currentRangeWeaponSystemInfo = rangeWeaponSystemInfoList [i];

				currentRangeWeaponSystemInfo.isCurrentWeapon = true;

				currentSimpleWeaponSystem = currentRangeWeaponSystemInfo.mainSimpleWeaponSystem;

				currentSimpleWeaponSystem.enabled = true;

				currentSimpleWeaponSystemAssigned = true;
			} else {
				if (rangeWeaponSystemInfoList [i].isCurrentWeapon) {

					rangeWeaponSystemInfoList [i].mainSimpleWeaponSystem.enabled = false;
				}

				rangeWeaponSystemInfoList [i].isCurrentWeapon = false;
			}
		}
	}

	public void fireCurrentRangeWeapon ()
	{
		if (rangeAttacksActive) {
			if (currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList [currentRangeWeaponInfo].useEventOnRangeWeapon) {
				currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList [currentRangeWeaponInfo].eventOnRangeWeapon.Invoke ();
			}

			currentSimpleWeaponSystem.inputShootWeaponOnPressDown ();

			currentSimpleWeaponSystem.inputShootWeaponOnPressUp ();
		}
	}

	public void fireCurrentRangeWeaponByAttackRangeID (int newRangeID)
	{
		if (rangeAttacksActive) {
			setCurrentRangeWeaponSystemByID (newRangeID);

			int rangeWeaponInfoListCount = currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList.Count;

			for (int i = 0; i < rangeWeaponInfoListCount; i++) {
				if (currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList [i].rangeWeaponID == newRangeID) {
					currentRangeWeaponInfo = newRangeID;

					currentSimpleWeaponSystem.setCustomprojectilePosition (currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList [i].projectilePosition);

					fireCurrentRangeWeapon ();

					return;
				}
			}
		}
	}

	public void startFireCurrentRangeWeapon ()
	{
		if (rangeAttacksActive) {
			if (currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList [currentRangeWeaponInfo].useEventOnRangeWeapon) {
				currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList [currentRangeWeaponInfo].eventOnRangeWeapon.Invoke ();
			}

			currentSimpleWeaponSystem.inputHoldOrReleaseShootWeapon (true);

			firingCurrentSimpleWeaponSystem = true;

			if (showDebugPrint) {
				print ("Star fire current range weapon");
			}

			if (currentRangeWeaponSystemInfo.useStrafeMode) {
				if (currentRangeWeaponSystemInfo.previousStrafeID == -1) {
					currentRangeWeaponSystemInfo.previousStrafeMode = mainGrabbedObjectMeleeAttackSystem.isStrafeModeActive ();

					currentRangeWeaponSystemInfo.previousStrafeID = mainGrabbedObjectMeleeAttackSystem.getCurrentStrafeID ();

					mainGrabbedObjectMeleeAttackSystem.setStrafeModeState (true, currentRangeWeaponSystemInfo.strafeIDUsed);
				}
			}

			if (currentRangeWeaponSystemInfo.setNewCrouchID) {
				if (currentRangeWeaponSystemInfo.previousCrouchID == -1) {
					currentRangeWeaponSystemInfo.previousCrouchID = mainGrabbedObjectMeleeAttackSystem.getCurrentCrouchID ();

					mainGrabbedObjectMeleeAttackSystem.setCurrentCrouchIDValue (currentRangeWeaponSystemInfo.crouchIDUsed);
				}
			}

			mainPlayerController.enableOrDisableAiminig (true);		

			mainHeadTrack.setHeadTrackActiveWhileAimingState (true);

			if (aimingActiveBodyWeight != 0) {
				mainHeadTrack.setCameraBodyWeightValue (aimingActiveBodyWeight);
			}
		}
	}

	public void stopFireCurrentRangeWeapon ()
	{
		if (rangeAttacksActive) {
//			if (currentSimpleWeaponSystemAssigned && firingCurrentSimpleWeaponSystem) {
//				currentSimpleWeaponSystem.inputHoldOrReleaseShootWeapon (false);
//
//				firingCurrentSimpleWeaponSystem = false;
//
//				if (showDebugPrint) {
//					print ("Stop fire current range weapon");
//				}
//
//				if (currentRangeWeaponSystemInfo.useStrafeMode) {
//					if (currentRangeWeaponSystemInfo.previousStrafeID != -1) {
//						mainGrabbedObjectMeleeAttackSystem.setStrafeModeState (currentRangeWeaponSystemInfo.previousStrafeMode, currentRangeWeaponSystemInfo.previousStrafeID);
//					} else {
//						mainGrabbedObjectMeleeAttackSystem.setStrafeModeState (false, 0);
//					}
//
//					currentRangeWeaponSystemInfo.previousStrafeMode = false;
//
//					currentRangeWeaponSystemInfo.previousStrafeID = -1;
//				}
//
//				if (currentRangeWeaponSystemInfo.setNewCrouchID) {
//					if (currentRangeWeaponSystemInfo.previousCrouchID != -1) {
//						mainGrabbedObjectMeleeAttackSystem.setCurrentCrouchIDValue (currentRangeWeaponSystemInfo.previousCrouchID);
//					} else {
//						mainGrabbedObjectMeleeAttackSystem.setCurrentCrouchIDValue (0);
//					}
//
//					currentRangeWeaponSystemInfo.previousCrouchID = -1;
//				}
//
//				mainPlayerController.enableOrDisableAiminig (false);		
//
//				mainHeadTrack.setHeadTrackActiveWhileAimingState (false);
//
//				if (aimingActiveBodyWeight != 0) {
//					mainHeadTrack.setOriginalCameraBodyWeightValue ();
//				}
//			}

			forceStopFireCurrentRangeWeapon ();
		}
	}

	void forceStopFireCurrentRangeWeapon ()
	{
		if (currentSimpleWeaponSystemAssigned && firingCurrentSimpleWeaponSystem) {
//			currentSimpleWeaponSystem.inputHoldOrReleaseShootWeapon (false);
//
//			firingCurrentSimpleWeaponSystem = false;
//
//			mainPlayerController.enableOrDisableAiminig (false);		
//
//			mainHeadTrack.setHeadTrackActiveWhileAimingState (false);
//
//			if (aimingActiveBodyWeight != 0) {
//				mainHeadTrack.setOriginalCameraBodyWeightValue ();
//			}

			currentSimpleWeaponSystem.inputHoldOrReleaseShootWeapon (false);

			firingCurrentSimpleWeaponSystem = false;

			if (showDebugPrint) {
				print ("Stop fire current range weapon");
			}

			if (currentRangeWeaponSystemInfo.useStrafeMode) {
				if (currentRangeWeaponSystemInfo.previousStrafeID != -1) {
					mainGrabbedObjectMeleeAttackSystem.setStrafeModeState (currentRangeWeaponSystemInfo.previousStrafeMode, currentRangeWeaponSystemInfo.previousStrafeID);
				} else {
					mainGrabbedObjectMeleeAttackSystem.setStrafeModeState (false, 0);
				}

				currentRangeWeaponSystemInfo.previousStrafeMode = false;

				currentRangeWeaponSystemInfo.previousStrafeID = -1;
			}

			if (currentRangeWeaponSystemInfo.setNewCrouchID) {
				if (currentRangeWeaponSystemInfo.previousCrouchID != -1) {
					mainGrabbedObjectMeleeAttackSystem.setCurrentCrouchIDValue (currentRangeWeaponSystemInfo.previousCrouchID);
				} else {
					mainGrabbedObjectMeleeAttackSystem.setCurrentCrouchIDValue (0);
				}

				currentRangeWeaponSystemInfo.previousCrouchID = -1;
			}

			mainPlayerController.enableOrDisableAiminig (false);		

			mainHeadTrack.setHeadTrackActiveWhileAimingState (false);

			if (aimingActiveBodyWeight != 0) {
				mainHeadTrack.setOriginalCameraBodyWeightValue ();
			}
		}
	}

	public void startFireCurrentRangeWeaponByAttackRangeID (int newRangeID)
	{
		if (rangeAttacksActive) {
			setCurrentRangeWeaponSystemByID (newRangeID);

			if (showDebugPrint) {
				print ("Start Fire Current Range Weapon By Attack Range ID " + newRangeID);
			}

			for (int i = 0; i < rangeWeaponSystemInfoList.Count; i++) {
				if (currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList [i].rangeWeaponID == newRangeID) {
					currentRangeWeaponInfo = newRangeID;

					currentSimpleWeaponSystem.setCustomprojectilePosition (currentMeleeWeaponRangeAttacksSystem.rangeWeaponInfoList [i].projectilePosition);

					if (showDebugPrint) {
						print ("Range ID detected " + newRangeID);
					}

					startFireCurrentRangeWeapon ();

					return;
				}
			}
		}
	}

	public void enableCustomReticle (string reticleName)
	{
		if (mainPlayerCamera != null) {
			mainPlayerCamera.enableOrDisableCustomReticle (true, reticleName);
		}
	}

	public void disableCustomReticle (string reticleName)
	{
		if (mainPlayerCamera != null) {
			mainPlayerCamera.enableOrDisableCustomReticle (false, reticleName);
		}
	}

	public void disableAllCustomReticle ()
	{
		if (mainPlayerCamera != null) {
			mainPlayerCamera.disableAllCustomReticle ();
		}
	}

	public void inputSetAimState (bool state)
	{
		if (!rangeAttacksActive) {
			return;
		}

		setAimState (state);
	}

	public void inputToggleAimState ()
	{
		if (!rangeAttacksActive) {
			return;
		}

		setAimState (!aimingActive);
	}

	public void setAimState (bool state)
	{
		if (aimingActive == state) {
			return;
		}

		aimingActive = state;

		checkEventsOnStateChange (aimingActive);
	}

	void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnAim) {
			if (state) {
				eventOnStartAiming.Invoke ();
			} else {
				eventOnStopAiming.Invoke ();
			}
		}
	}

	[System.Serializable]
	public class rangeWeaponSystemInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;

		public int rangeWeaponID;

		public bool isCurrentWeapon;

		[Space]
		[Header ("Movement Settings")]
		[Space]

		public bool useStrafeMode;
		public int strafeIDUsed;

		public bool previousStrafeMode;
		public int previousStrafeID = -1;

		[Space]

		public bool setNewCrouchID;
		public int crouchIDUsed;

		public int previousCrouchID = -1;

		[Space]
		[Header ("Other Settings")]
		[Space]

		public simpleWeaponSystem mainSimpleWeaponSystem;
	}
}
