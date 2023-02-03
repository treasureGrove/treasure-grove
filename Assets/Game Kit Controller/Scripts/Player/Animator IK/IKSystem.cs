using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class IKSystem : MonoBehaviour
{
	[Header ("Main Setting")]
	[Space]

	public LayerMask layer;
	[Range (1, 10)] public float IKSpeed;

	public string rightHandGrabbingIDName = "Right Hand Grabbing ID";
	public string leftHandGrabbingIDName = "Left Hand Grabbing ID";

	[Space]
	[Header ("IK Body Info State List")]
	[Space]

	public List<IKBodyInfoState> IKBodyInfoStateList = new List<IKBodyInfoState> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public aimMode currentAimMode;

	public bool usingHands;

	public bool usingWeapons;
	public bool usingDualWeapon;
	public bool disablingDualWeapons;

	public bool usingArms;
	public bool driving;
	public bool usingZipline;

	public bool disableWeapons;
	public bool IKBodyInfoActive;

	public bool disablingIKBodyInfoActive;
	public bool objectGrabbed;
	public bool IKSystemEnabledToGrab;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;

	[Space]
	[Header ("Mount Points Settings")]
	[Space]

	public Transform rightHandMountPoint;
	public Transform leftHandMountPoint;

	public Transform rightLowerArmMountPoint;
	public Transform leftLowerArmMountPoint;

	[Space]
	[Header ("On Animator IK Components")]
	[Space]

	public OnAnimatorIKComponent handsOnSurfaceIKComponent;
	public OnAnimatorIKComponent IKFootSystemComponent;
	public OnAnimatorIKComponent headTrackIKComponent;
	public OnAnimatorIKComponent bulletTimeIKComponent;


	[Space]
	[Header ("Components")]
	[Space]

	public Transform rightHandIKPos;
	public Transform leftHandIKPos;
	public Transform currentHandIKPos;

	public Transform rightElbowIKPos;
	public Transform leftElbowIKPos;
	public Transform currentElbowIKPos;
	public Transform IKBodyCOM;

	public Animator animator;
	public playerController playerControllerManager;
	public playerWeaponsManager weaponsManager;
	public otherPowers powersManager;

	List<IKBodyInfo> currentIKBodyInfoList = new List<IKBodyInfo> ();

	IKBodyInfoState currentIKBodyInfoState;

	IKBodyInfo currentIKBodyInfo;
	Vector2 playerInputAxis;

	public IKSettings settings;

	public enum aimMode
	{
		hands,
		weapons
	}

	IKDrivingSystem.IKDrivingInformation IKDrivingSettings;
	IKDrivingSystem.IKDrivingPositions currentIKDrivingPositions;
	IKDrivingSystem.IKDrivingKneePositions currentIKDrivingKneePositions;
	float currentDriveIkWeightValue;

	zipline.IKZiplineInfo IKZiplineSettings;
	zipline.IKGoalsZiplinePositions currentIKGoalsZiplinePositions;
	zipline.IKHintsZiplinePositions currentIKHintsZiplinePositions;

	IKWeaponInfo IKWeaponsSettings;

	IKWeaponInfo IKWeaponsRightHandSettings;
	IKWeaponInfo IKWeaponsLeftHandSettings;

	AvatarIKGoal currentHand;

	float IKWeight;
	float IKWeightTargetValue;
	float originalDist;
	float hitDist;
	float currentDist;
	AvatarIKHint currenElbow;

	RaycastHit hit;

	Coroutine powerHandRecoil;
	float originalCurrentDist;
	int handsDisabled;
	float currentHeadWeight;
	float headWeightTarget;
	Transform playerHead;

	bool isThirdPersonView;

	List<grabObjects.handInfo> handInfoList = new List<grabObjects.handInfo> ();
	grabObjects.handInfo currentHandInfo;

	float currentWeightLerpSpeed;

	bool resetIKCOMRotationChecked;

	Vector3 targetRotation;

	bool headLookStateEnabled;
	bool disableHeadLookState;
	float headLookSpeed;
	Transform headLookTarget;

	Vector3 drivingBodyRotation;

	IKWeaponsPosition currentIKWeaponsPosition;

	bool IKBodyInfoStateListOriginalPositionAssigned;

	AvatarIKGoal currentIKGoal;
	AvatarIKHint currentIKHint;

	bool handsOnSurfaceIKComponentLocated;
	bool IKFootSystemComponentLocated;
	bool headTrackIKComponentLocated;
	bool bulletTimeIKComponentLocated;

	bool componentEnabled = true;

	OnAnimatorIKComponent temporalOnAnimatorIKComponent;

	bool temporalOnAnimatorIKComponentAssigned;


	void Start ()
	{
		setIKBodyInfoListOriginalPosition ();

		handsOnSurfaceIKComponentLocated = handsOnSurfaceIKComponent != null;

		IKFootSystemComponentLocated = IKFootSystemComponent != null;

		headTrackIKComponentLocated = headTrackIKComponent != null;

		bulletTimeIKComponentLocated = bulletTimeIKComponent != null;
	}

	void Update ()
	{
		isThirdPersonView = !playerControllerManager.isPlayerOnFirstPerson ();

		if (usingWeapons || usingArms || driving || usingZipline || objectGrabbed) {
			usingHands = true;
		} else {
			usingHands = false;
		}

		if (!driving && usingArms) {
			//change the current weight of the ik 
			if (IKWeight != IKWeightTargetValue) {
				IKWeight = Mathf.MoveTowards (IKWeight, IKWeightTargetValue, Time.deltaTime * IKSpeed);
			}

			if (IKWeight > 0) {
				//if the raycast detects a surface, get the distance to it
				if (Physics.Raycast (currentHandIKPos.position, transform.forward, out hit, 3, layer)) {
					if (!hit.collider.isTrigger) {
						if (hit.distance < originalDist) {
							hitDist = hit.distance;
						} else {
							hitDist = originalDist;
						}
					}
				}
				//else, set the original distance
				else {
					hitDist = originalDist;
				}
				hitDist = Mathf.Clamp (hitDist, 0.1f, originalDist);
				//set the correct position of the current hand to avoid cross any collider with it
				currentDist = Mathf.Lerp (currentDist, hitDist, Time.deltaTime * IKSpeed);
				currentHandIKPos.localPosition = new Vector3 (currentHandIKPos.localPosition.x, currentDist, currentHandIKPos.localPosition.z);
			}

			if (IKWeight == 0) {
				usingArms = false;
			}
		}

		if (usingWeapons) {
			if (disableWeapons || disablingDualWeapons) {
				handsDisabled = 0;
			}

			if (usingDualWeapon) {
				if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.dualWeaponActive) {
					currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];

					if (currentIKWeaponsPosition.HandIKWeight != currentIKWeaponsPosition.targetValue) {
						currentIKWeaponsPosition.HandIKWeight = Mathf.MoveTowards (currentIKWeaponsPosition.HandIKWeight, currentIKWeaponsPosition.targetValue, Time.deltaTime * IKSpeed);
					}

					if (currentIKWeaponsPosition.elbowInfo.elbowIKWeight != currentIKWeaponsPosition.elbowInfo.targetValue) {
						currentIKWeaponsPosition.elbowInfo.elbowIKWeight = 
							Mathf.MoveTowards (currentIKWeaponsPosition.elbowInfo.elbowIKWeight, currentIKWeaponsPosition.elbowInfo.targetValue, Time.deltaTime * IKSpeed);
					}

					if (disablingDualWeapons) {
						if (currentIKWeaponsPosition.HandIKWeight == 0) {
							handsDisabled++;
						}
					}
				}

				if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.dualWeaponActive) {
					currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];

					if (currentIKWeaponsPosition.HandIKWeight != currentIKWeaponsPosition.targetValue) {
						currentIKWeaponsPosition.HandIKWeight = Mathf.MoveTowards (currentIKWeaponsPosition.HandIKWeight, currentIKWeaponsPosition.targetValue, Time.deltaTime * IKSpeed);
					}

					if (currentIKWeaponsPosition.elbowInfo.elbowIKWeight != currentIKWeaponsPosition.elbowInfo.targetValue) {
						currentIKWeaponsPosition.elbowInfo.elbowIKWeight = 
							Mathf.MoveTowards (currentIKWeaponsPosition.elbowInfo.elbowIKWeight, currentIKWeaponsPosition.elbowInfo.targetValue, Time.deltaTime * IKSpeed);
					}

					if (disablingDualWeapons) {
						if (currentIKWeaponsPosition.HandIKWeight == 0) {
							handsDisabled++;
						}
					}
				}
			} else {
				int handsInfoCount = IKWeaponsSettings.handsInfo.Count;

				for (int j = 0; j < handsInfoCount; j++) {
					currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [j];

					if (currentIKWeaponsPosition.HandIKWeight != currentIKWeaponsPosition.targetValue &&
					    (!currentIKWeaponsPosition.ignoreIKWeight || currentIKWeaponsPosition.targetValue == 0)) {

						currentIKWeaponsPosition.HandIKWeight = Mathf.MoveTowards (currentIKWeaponsPosition.HandIKWeight, currentIKWeaponsPosition.targetValue, Time.deltaTime * IKSpeed);
					}

					if (currentIKWeaponsPosition.elbowInfo.elbowIKWeight != currentIKWeaponsPosition.elbowInfo.targetValue &&
					    (!currentIKWeaponsPosition.ignoreIKWeight || currentIKWeaponsPosition.elbowInfo.targetValue == 0)) {
						currentIKWeaponsPosition.elbowInfo.elbowIKWeight = 
							Mathf.MoveTowards (currentIKWeaponsPosition.elbowInfo.elbowIKWeight, currentIKWeaponsPosition.elbowInfo.targetValue, Time.deltaTime * IKSpeed);
					}

					if (disableWeapons) {
						if (currentIKWeaponsPosition.HandIKWeight == 0) {
							handsDisabled++;
						}
					}
				}
			}

			if (disableWeapons || disablingDualWeapons) {					
				if (handsDisabled == 2) {
					setDisableWeaponsState (false);

					handsDisabled = 0;

					usingDualWeapon = false;

					setUsingWeaponsState (false);

					checkHandsInPosition (false);
				}
			}
		}

		if (usingZipline) {
			if (IKWeight != IKWeightTargetValue) {
				IKWeight = Mathf.MoveTowards (IKWeight, IKWeightTargetValue, Time.deltaTime * IKSpeed);
			}
		}

		if (IKBodyInfoActive && !isThirdPersonView && resetIKCOMRotationChecked) {
			IKBodyCOM.localRotation = Quaternion.identity;
			resetIKCOMRotationChecked = false;
		}
	}

	void LateUpdate ()
	{
		if (driving) {
			if (IKDrivingSettings.useIKOnVehicle && IKDrivingSettings.useHeadLookDirection) {
				if (IKDrivingSettings.headLookDirection != null) {
					if (playerHead == null) {
						playerHead = animator.GetBoneTransform (HumanBodyBones.Head);
					} else {
						Quaternion headRotation = Quaternion.FromToRotation (playerHead.transform.InverseTransformDirection (IKDrivingSettings.headLookDirection.forward), 
							                          IKDrivingSettings.headLookDirection.InverseTransformDirection (IKDrivingSettings.headLookDirection.forward));
						Vector3 headDirection = headRotation.eulerAngles;
//				Quaternion headRotationY = Quaternion.FromToRotation (playerHead.transform.InverseTransformDirection (IKDrivingSettings.headLookDirection.forward), 
						//IKDrivingSettings.headLookDirection.InverseTransformDirection (IKDrivingSettings.headLookDirection.forward));
//				Vector3 headDirectionY = headRotationY.eulerAngles;
						playerHead.transform.localEulerAngles = -headDirection;
					}
				}
			}
		}
	}

	void OnAnimatorIK (int layerIndex)
	{
		if (!driving && !usingWeapons && IKWeight > 0 && usingArms) {
			//set the current hand target position and rotation
			animator.SetIKPositionWeight (currentHand, IKWeight);
			animator.SetIKRotationWeight (currentHand, IKWeight);  

			animator.SetIKPosition (currentHand, currentHandIKPos.position);
			animator.SetIKRotation (currentHand, currentHandIKPos.rotation);     

			animator.SetIKHintPositionWeight (currenElbow, IKWeight);
			animator.SetIKHintPosition (currenElbow, currentElbowIKPos.position);
		}

		//if the player is driving, set all the position and rotations of every player's limb
		if (driving) {
			if (IKDrivingSettings.disableIKOnPassengerSmoothly) {
				IKDrivingSettings.currentDriveIkWeightValue = Mathf.Lerp (IKDrivingSettings.currentDriveIkWeightValue, 0, Time.deltaTime * 2);
			} else {
				if (IKDrivingSettings.currentDriveIkWeightValue != 1) {
					IKDrivingSettings.currentDriveIkWeightValue = Mathf.Lerp (IKDrivingSettings.currentDriveIkWeightValue, 1, Time.deltaTime * 2);
				}
			}

			currentDriveIkWeightValue = IKDrivingSettings.currentDriveIkWeightValue;

			if (IKDrivingSettings.useIKOnVehicle) {
				int IKDrivingPosCount = IKDrivingSettings.IKDrivingPos.Count;

				for (int i = 0; i < IKDrivingPosCount; i++) {
					//hands and foots
					currentIKDrivingPositions = IKDrivingSettings.IKDrivingPos [i];

					currentIKGoal = currentIKDrivingPositions.limb;

					animator.SetIKPositionWeight (currentIKGoal, currentDriveIkWeightValue);
					animator.SetIKRotationWeight (currentIKGoal, currentDriveIkWeightValue); 

					animator.SetIKPosition (currentIKGoal, currentIKDrivingPositions.position.position);
					animator.SetIKRotation (currentIKGoal, currentIKDrivingPositions.position.rotation);   
				}

				//knees and elbows

				int IKDrivingKneePosCount = IKDrivingSettings.IKDrivingKneePos.Count;

				for (int i = 0; i < IKDrivingKneePosCount; i++) {
					currentIKDrivingKneePositions = IKDrivingSettings.IKDrivingKneePos [i];

					animator.SetIKHintPositionWeight (currentIKDrivingKneePositions.knee, currentDriveIkWeightValue);

					animator.SetIKHintPosition (currentIKDrivingKneePositions.knee, currentIKDrivingKneePositions.position.position);
				}
			}

			if (IKDrivingSettings.adjustPassengerPositionActive) {
				//comment/discomment these two lines to edit correctly the body position of the player ingame.
				transform.position = IKDrivingSettings.vehicleSeatInfo.seatTransform.position;
				transform.rotation = IKDrivingSettings.vehicleSeatInfo.seatTransform.rotation;
			}

			//set the rotation of the upper body of the player according to the steering direction
			if (IKDrivingSettings.useIKOnVehicle && IKDrivingSettings.useSteerDirection) {
				if (IKDrivingSettings.steerDirecion != null) {
					Vector3 lookDirection = IKDrivingSettings.steerDirecion.transform.forward + IKDrivingSettings.steerDirecion.transform.position;
					animator.SetLookAtPosition (lookDirection);

					animator.SetLookAtWeight (settings.weight, settings.bodyWeight, settings.headWeight, settings.eyesWeight, settings.clampWeight);
				}
			}

			if (IKDrivingSettings.shakePlayerBodyOnCollision) {
				IKDrivingSettings.currentSeatShake = Vector3.Lerp (IKDrivingSettings.currentSeatShake, Vector3.zero, Time.deltaTime * IKDrivingSettings.shakeFadeSpeed);

				drivingBodyRotation = IKDrivingSettings.currentAngularDirection;

				drivingBodyRotation = Vector3.Scale (drivingBodyRotation, IKDrivingSettings.forceDirection);

				drivingBodyRotation.x += ((Mathf.Cos (Time.time * IKDrivingSettings.shakeSpeed)) / 2) * IKDrivingSettings.currentSeatShake.x;
				drivingBodyRotation.y -= (Mathf.Cos (Time.time * IKDrivingSettings.shakeSpeed)) * IKDrivingSettings.currentSeatShake.y;
				drivingBodyRotation.z -= ((Mathf.Sin (Time.time * IKDrivingSettings.shakeSpeed)) / 2) * IKDrivingSettings.currentSeatShake.z;

				drivingBodyRotation.x = Mathf.Clamp (drivingBodyRotation.x, IKDrivingSettings.shakeForceDirectionMinClamp.z, IKDrivingSettings.shakeForceDirectionMaxClamp.z);
				drivingBodyRotation.y = Mathf.Clamp (drivingBodyRotation.y, IKDrivingSettings.shakeForceDirectionMinClamp.y, IKDrivingSettings.shakeForceDirectionMaxClamp.y);
				drivingBodyRotation.z = Mathf.Clamp (drivingBodyRotation.z, IKDrivingSettings.shakeForceDirectionMinClamp.x, IKDrivingSettings.shakeForceDirectionMaxClamp.x);

				IKDrivingSettings.currentSeatUpRotation = drivingBodyRotation;

				IKDrivingSettings.playerBodyParent.localRotation =
					Quaternion.Lerp (IKDrivingSettings.playerBodyParent.localRotation, Quaternion.Euler (IKDrivingSettings.currentSeatUpRotation), Time.deltaTime * IKDrivingSettings.stabilitySpeed);
			}

			if (IKDrivingSettings.useIKOnVehicle && IKDrivingSettings.useHeadLookPosition) {
				if (IKDrivingSettings.headLookPosition != null) {
					animator.SetLookAtWeight (1, 0, 1);
					animator.SetLookAtPosition (IKDrivingSettings.headLookPosition.position);
				}
			}
		}

		if (!driving && usingWeapons) {
			if (usingDualWeapon) {
				if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.dualWeaponActive) {
					currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];

					currentIKGoal = currentIKWeaponsPosition.limb;

					animator.SetIKPositionWeight (currentIKGoal, currentIKWeaponsPosition.HandIKWeight);
					animator.SetIKRotationWeight (currentIKGoal, currentIKWeaponsPosition.HandIKWeight);

					if (currentIKWeaponsPosition.transformFollowByHand != null) {
						animator.SetIKPosition (currentIKGoal, currentIKWeaponsPosition.transformFollowByHand.position);
						animator.SetIKRotation (currentIKGoal, currentIKWeaponsPosition.transformFollowByHand.rotation); 
					}

					currentIKHint = currentIKWeaponsPosition.elbowInfo.elbow;

					animator.SetIKHintPositionWeight (currentIKHint, currentIKWeaponsPosition.elbowInfo.elbowIKWeight);
					animator.SetIKHintPosition (currentIKHint, currentIKWeaponsPosition.elbowInfo.position.position);
				}

				if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.dualWeaponActive) {
					currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];

					currentIKGoal = currentIKWeaponsPosition.limb;

					animator.SetIKPositionWeight (currentIKGoal, currentIKWeaponsPosition.HandIKWeight);
					animator.SetIKRotationWeight (currentIKGoal, currentIKWeaponsPosition.HandIKWeight);

					if (currentIKWeaponsPosition.transformFollowByHand != null) {
						animator.SetIKPosition (currentIKGoal, currentIKWeaponsPosition.transformFollowByHand.position);
						animator.SetIKRotation (currentIKGoal, currentIKWeaponsPosition.transformFollowByHand.rotation); 
					}

					currentIKHint = currentIKWeaponsPosition.elbowInfo.elbow;

					animator.SetIKHintPositionWeight (currentIKHint, currentIKWeaponsPosition.elbowInfo.elbowIKWeight);
					animator.SetIKHintPosition (currentIKHint, currentIKWeaponsPosition.elbowInfo.position.position);
				}
			} else {
				int handsInfoCount = IKWeaponsSettings.handsInfo.Count;

				for (int i = 0; i < handsInfoCount; i++) {
					currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

					if (currentIKWeaponsPosition.handUsedInWeapon) {
						currentIKGoal = currentIKWeaponsPosition.limb;

						animator.SetIKPositionWeight (currentIKGoal, currentIKWeaponsPosition.HandIKWeight);
						animator.SetIKRotationWeight (currentIKGoal, currentIKWeaponsPosition.HandIKWeight);  

						if (currentIKWeaponsPosition.transformFollowByHand != null) {
							animator.SetIKPosition (currentIKGoal, currentIKWeaponsPosition.transformFollowByHand.position);
							animator.SetIKRotation (currentIKGoal, currentIKWeaponsPosition.transformFollowByHand.rotation); 
						}

						currentIKHint = currentIKWeaponsPosition.elbowInfo.elbow;

						animator.SetIKHintPositionWeight (currentIKHint, currentIKWeaponsPosition.elbowInfo.elbowIKWeight);

						animator.SetIKHintPosition (currentIKHint, currentIKWeaponsPosition.elbowInfo.position.position);
					}
				}
			}

			if (weaponsManager.currentWeaponUsesHeadLookWhenAiming ()) {
				if (weaponsManager.isAimingInThirdPerson () && !weaponsManager.isReloadingWithAnimationActive ()) {
					headWeightTarget = 1;
				} else {
					headWeightTarget = 0;
				}

				if (currentHeadWeight != headWeightTarget) {
					currentHeadWeight = Mathf.MoveTowards (currentHeadWeight, headWeightTarget, Time.deltaTime * weaponsManager.getCurrentWeaponHeadLookSpeed ());
				}

				animator.SetLookAtWeight (1, 0, currentHeadWeight);
				animator.SetLookAtPosition (weaponsManager.getCurrentHeadLookTargetPosition ());
			}
		}

		if (!driving) {
			if (headLookStateEnabled) {

				if (disableHeadLookState) {
					if (headWeightTarget == 0 && currentHeadWeight == 0) {
						headLookStateEnabled = false;
					}
				} else {
					if (powersManager.aimingInThirdPerson) {
						headWeightTarget = 1;
					} else {
						headWeightTarget = 0;
					}
				}

				if (currentHeadWeight != headWeightTarget) {
					currentHeadWeight = Mathf.MoveTowards (currentHeadWeight, headWeightTarget, Time.deltaTime * headLookSpeed);
				}

				animator.SetLookAtWeight (1, 0, currentHeadWeight);
				animator.SetLookAtPosition (headLookTarget.position);
			}
		}

		if (usingZipline) {
			int IKZiplineSettingsIKGoalsCount = IKZiplineSettings.IKGoals.Count;

			for (int i = 0; i < IKZiplineSettingsIKGoalsCount; i++) {
				currentIKGoalsZiplinePositions = IKZiplineSettings.IKGoals [i];

				currentIKGoal = currentIKGoalsZiplinePositions.limb;

				animator.SetIKPositionWeight (currentIKGoal, IKWeight);
				animator.SetIKRotationWeight (currentIKGoal, IKWeight);  

				animator.SetIKPosition (currentIKGoal, currentIKGoalsZiplinePositions.position.position);
				animator.SetIKRotation (currentIKGoal, currentIKGoalsZiplinePositions.position.rotation); 
			}

			int IKZiplineSettingsIKHintsCount = IKZiplineSettings.IKHints.Count;

			for (int i = 0; i < IKZiplineSettingsIKHintsCount; i++) {
				currentIKHintsZiplinePositions = IKZiplineSettings.IKHints [i];

				currentIKHint = currentIKHintsZiplinePositions.limb;

				animator.SetIKHintPositionWeight (currentIKHint, IKWeight);
				animator.SetIKHintPosition (currentIKHint, currentIKHintsZiplinePositions.position.position);
			}

			transform.position = IKZiplineSettings.bodyPosition.position;
			transform.rotation = IKZiplineSettings.bodyPosition.rotation;
		}
			
		if (isThirdPersonView && objectGrabbed) {

			if (IKSystemEnabledToGrab) {
				int handInfoListCount = handInfoList.Count;

				for (int j = 0; j < handInfoListCount; j++) {

					currentHandInfo = handInfoList [j];

					if (currentHandInfo.useHand) {

						if (currentHandInfo.handTransform != null) {
							currentHandInfo.handPosition = currentHandInfo.handTransform.position;          
							currentHandInfo.handRotation = currentHandInfo.handTransform.rotation;
						}

						currentHandInfo.currentHandWeight = 1;

						currentIKGoal = currentHandInfo.IKGoal;

						animator.SetIKRotationWeight (currentIKGoal, currentHandInfo.currentHandWeight);
						animator.SetIKPositionWeight (currentIKGoal, currentHandInfo.currentHandWeight);

						animator.SetIKPosition (currentIKGoal, currentHandInfo.handPosition);
						animator.SetIKRotation (currentIKGoal, currentHandInfo.handRotation);

						if (currentHandInfo.useElbow) {

							if (currentHandInfo.elbowTransform != null) {
								currentHandInfo.elbowPosition = currentHandInfo.elbowTransform.position;     
							}

							currentIKHint = currentHandInfo.IKHint;

							animator.SetIKHintPositionWeight (currentIKHint, currentHandInfo.currentHandWeight);

							animator.SetIKHintPosition (currentIKHint, currentHandInfo.elbowPosition);
						}
					}
				}
			}
		} 

		if (isThirdPersonView && IKBodyInfoActive) {
			if (!playerControllerManager.isPlayerOnGround () || disablingIKBodyInfoActive) {
				resetIKCOMRotationChecked = true;

				playerInputAxis = playerControllerManager.getAxisValues ();

				int numberOfLimbsActive = 0;

				int currentIKBodyInfoListCount = currentIKBodyInfoList.Count;

				for (int j = 0; j < currentIKBodyInfoListCount; j++) {

					currentIKBodyInfo = currentIKBodyInfoList [j];

					if (currentIKBodyInfo.IKBodyPartEnabled) {
			
						if (IKBodyPaused || playerControllerManager.isActionActive ()) {
							if (!currentIKBodyInfo.bodyPartBusy) {
								currentIKBodyInfo.bodyPartBusy = true;
								currentIKBodyInfo.IKBodyWeigthTarget = 0;
							}
						} else {
							if (currentIKBodyInfo.isHand) {
								if (!usingHands) {
									if (currentIKBodyInfo.bodyPartBusy) {
										currentIKBodyInfo.bodyPartBusy = false;
										currentIKBodyInfo.IKBodyWeigthTarget = 1;
									}
								} else {
									if (!currentIKBodyInfo.bodyPartBusy) {
										currentIKBodyInfo.bodyPartBusy = true;
										currentIKBodyInfo.IKBodyWeigthTarget = 0;
									}
								}
							}
						}

						if (currentIKBodyInfo.currentIKWeight != currentIKBodyInfo.IKBodyWeigthTarget) {

							if (currentIKBodyInfo.bodyPartBusy) {
								currentWeightLerpSpeed = currentIKBodyInfoState.busyBodyPartWeightLerpSpeed;
							} else {
								currentWeightLerpSpeed = currentIKBodyInfo.weightLerpSpeed;
							}

							currentIKBodyInfo.currentIKWeight = Mathf.MoveTowards (currentIKBodyInfo.currentIKWeight,
								currentIKBodyInfo.IKBodyWeigthTarget, Time.deltaTime * currentWeightLerpSpeed);
						} else {

							if (currentIKBodyInfo.currentIKWeight == 0 && currentIKBodyInfo.IKBodyWeigthTarget == 0) {
								numberOfLimbsActive++;
							}

							if (numberOfLimbsActive == currentIKBodyInfoList.Count && disablingIKBodyInfoActive) {
								IKBodyInfoActive = false;
								disablingIKBodyInfoActive = false;

								return;
							}
						}

						bool applyIKToBodyPart = false;

						if (!currentIKBodyInfo.bodyPartBusy) {
							if (playerInputAxis == Vector2.zero) {
								currentIKBodyInfo.currentLimbVerticalMovementSpeed = currentIKBodyInfo.limbVerticalMovementSpeed;
							} else {
								currentIKBodyInfo.currentLimbVerticalMovementSpeed = currentIKBodyInfo.slowLimbVerticalMovementSpeed;
							}

							if (Time.time > lastTimeIkBodyStateActive + currentIKBodyInfoState.delayForVerticalMovement) {
								if (currentIKBodyInfo.useSin) {
									currentIKBodyInfo.posTargetY = Mathf.Sin (Time.time * currentIKBodyInfo.currentLimbVerticalMovementSpeed) * currentIKBodyInfo.limbMovementAmount;
								} else {
									currentIKBodyInfo.posTargetY = Mathf.Cos (Time.time * currentIKBodyInfo.currentLimbVerticalMovementSpeed) * currentIKBodyInfo.limbMovementAmount;
								}
							} else {
								currentIKBodyInfo.posTargetY = Mathf.Lerp (currentIKBodyInfo.posTargetY, 0, Time.deltaTime / currentIKBodyInfoState.verticalMovementTransitionSpeed);
							}

							if (playerControllerManager.isPlayerMovingOn3dWorld ()) {
								currentIKBodyInfo.newPosition = new Vector3 (-playerInputAxis.x, currentIKBodyInfo.posTargetY, -playerInputAxis.y);
							} else {
								currentIKBodyInfo.newPosition = new Vector3 (0, currentIKBodyInfo.posTargetY - (playerInputAxis.y / 8), -Mathf.Abs (playerInputAxis.x));
							}

							currentIKBodyInfo.newPosition.x = Mathf.Clamp (currentIKBodyInfo.newPosition.x, currentIKBodyInfo.minClampPosition.x, currentIKBodyInfo.maxClampPosition.x);

							currentIKBodyInfo.newPosition.z = Mathf.Clamp (currentIKBodyInfo.newPosition.z, currentIKBodyInfo.minClampPosition.y, currentIKBodyInfo.maxClampPosition.y);

							currentIKBodyInfo.newPosition += currentIKBodyInfo.originalPosition;


							currentIKBodyInfo.targetToFollow.localPosition = Vector3.Slerp (currentIKBodyInfo.targetToFollow.localPosition, 
								currentIKBodyInfo.newPosition, Time.deltaTime * currentIKBodyInfo.limbsMovementSpeed);
							

							currentIKBodyInfo.IKGoalPosition = currentIKBodyInfo.targetToFollow.position;          
							currentIKBodyInfo.IKGoalRotation = currentIKBodyInfo.targetToFollow.rotation;

							applyIKToBodyPart = true;

						} else if (currentIKBodyInfo.currentIKWeight != currentIKBodyInfo.IKBodyWeigthTarget) {
							applyIKToBodyPart = true;
						}

						if (applyIKToBodyPart) {
							if (currentIKBodyInfo.useIKHint) {

								currentIKHint = currentIKBodyInfo.IKHint;

								animator.SetIKHintPositionWeight (currentIKHint, currentIKBodyInfo.currentIKWeight);
								animator.SetIKHintPosition (currentIKHint, currentIKBodyInfo.IKGoalPosition);
							} else {
								currentIKGoal = currentIKBodyInfo.IKGoal;

								animator.SetIKRotationWeight (currentIKGoal, currentIKBodyInfo.currentIKWeight);
								animator.SetIKPositionWeight (currentIKGoal, currentIKBodyInfo.currentIKWeight);
								animator.SetIKPosition (currentIKGoal, currentIKBodyInfo.IKGoalPosition);
								animator.SetIKRotation (currentIKGoal, currentIKBodyInfo.IKGoalRotation);
							}
						}
					}
				}

				if (!disablingIKBodyInfoActive) {
					if (weaponsManager.isUsingWeapons ()) {
						targetRotation = Vector3.zero;
					} else {

						if (currentIKBodyInfoState.increaseRotationAmountWhenHigherSpeed && playerControllerManager.isMovementSpeedIncreased ()) {
							currentIKBodyInfoState.currentIKBodyCOMRotationAmountX = currentIKBodyInfoState.increasedIKBodyCOMRotationAmountX;
							currentIKBodyInfoState.currentIKBodyCOMRotationAmountY = currentIKBodyInfoState.increasedIKBodyComRotationAmountY;

							currentIKBodyInfoState.currentIKBodyCOMRotationSpeed = currentIKBodyInfoState.increasedIKBodyCOMRotationSpeed;

						} else {
							currentIKBodyInfoState.currentIKBodyCOMRotationAmountX = currentIKBodyInfoState.IKBodyCOMRotationAmountX;
							currentIKBodyInfoState.currentIKBodyCOMRotationAmountY = currentIKBodyInfoState.IKBodyComRotationAmountY;

							currentIKBodyInfoState.currentIKBodyCOMRotationSpeed = currentIKBodyInfoState.IKBodyCOMRotationSpeed;
						}

						if (playerControllerManager.isPlayerMovingOn3dWorld ()) {
							targetRotation = new Vector3 (playerInputAxis.y * currentIKBodyInfoState.currentIKBodyCOMRotationAmountY, 
								0, playerInputAxis.x * currentIKBodyInfoState.currentIKBodyCOMRotationAmountX);
						} else {
							targetRotation = new Vector3 (-(Math.Abs (playerInputAxis.x) - playerInputAxis.y) * currentIKBodyInfoState.currentIKBodyCOMRotationAmountX, 0, 0);
						}
					}
					
					IKBodyCOM.localRotation = Quaternion.Lerp (IKBodyCOM.localRotation, Quaternion.Euler (targetRotation), Time.deltaTime * currentIKBodyInfoState.currentIKBodyCOMRotationSpeed);
				}
			} else if (playerControllerManager.isPlayerOnGround ()) {
				IKBodyCOM.localRotation = Quaternion.Lerp (IKBodyCOM.localRotation, Quaternion.identity, Time.deltaTime * currentIKBodyInfoState.currentIKBodyCOMRotationSpeed);
			}
		}

		if (handsOnSurfaceIKComponentLocated) {
			handsOnSurfaceIKComponent.updateOnAnimatorIKState ();
		}

		if (IKFootSystemComponentLocated) {
			IKFootSystemComponent.updateOnAnimatorIKState ();
		}

		if (headTrackIKComponentLocated) {
			if (headTrackIKComponent.updateIKEnabled) {
				headTrackIKComponent.updateOnAnimatorIKState ();
			}
		}

		if (bulletTimeIKComponentLocated) {
			if (bulletTimeIKComponent.updateIKEnabled) {
				bulletTimeIKComponent.updateOnAnimatorIKState ();
			}
		}

		if (temporalOnAnimatorIKComponentAssigned) {
			if (temporalOnAnimatorIKComponent.updateIKEnabled) {
				temporalOnAnimatorIKComponent.updateOnAnimatorIKState ();
			}
		}
	}

	public void setHeadLookState (bool state, float newHeadLookSpeed, Transform newHeadLookTarget)
	{
		if (state) {
			headLookStateEnabled = state;
			headWeightTarget = 1;
		} else {
			headWeightTarget = 0;
		}

		disableHeadLookState = !state;

		headLookSpeed = newHeadLookSpeed;
		headLookTarget = newHeadLookTarget;
	}


	public bool getUsingHands ()
	{
		return usingHands;
	}

	public float getHeadWeight ()
	{
		return currentHeadWeight;
	}

	public bool isUsingWeapons ()
	{
		return usingWeapons;
	}

	//change the ik weight in the current arm
	public void changeArmState (float value)
	{
		if (currentAimMode == aimMode.weapons) {
			usingArms = false;
		} else {
			usingArms = true;
		}
			
		IKWeightTargetValue = value;

		if (usingZipline) {
			IKWeightTargetValue = 1;
		}
	}

	public void disableArmsState ()
	{
		usingArms = false;
		IKWeightTargetValue = 0;
		IKWeight = 0;
	}

	//change current arm to aim
	public void changeArmSide (bool value)
	{
		if (value) {
			//set the right arm as the current ik position
			currentHandIKPos.position = rightHandIKPos.position;
			currentHandIKPos.rotation = rightHandIKPos.rotation;
			currentHand = AvatarIKGoal.RightHand;

			currentElbowIKPos.position = rightElbowIKPos.position;
			currenElbow = AvatarIKHint.RightElbow;
		} else {
			//set the left arm as the current ik position
			currentHandIKPos.position = leftHandIKPos.position;
			currentHandIKPos.rotation = leftHandIKPos.rotation;
			currentHand = AvatarIKGoal.LeftHand;

			currentElbowIKPos.position = leftElbowIKPos.position;
			currenElbow = AvatarIKHint.LeftElbow;
		}

		originalDist = currentHandIKPos.localPosition.y;
		currentDist = originalDist;
		hitDist = originalDist;
	}

	//set if the player is driving or not, getting the current positions to every player's limb
	public void setDrivingState (bool state, IKDrivingSystem.IKDrivingInformation IKPositions)
	{
		driving = state;

		if (driving) {
			IKDrivingSettings = IKPositions;

			if (IKDrivingSettings != null && IKDrivingSettings.vehicleSeatInfo.useGrabbingHandID) {
				setGrabbingHandState (IKDrivingSettings.vehicleSeatInfo.rightGrabbingHandID, true);
				setGrabbingHandState (IKDrivingSettings.vehicleSeatInfo.leftGrabbingHandID, false);
			}
		} else {
			if (IKDrivingSettings != null && IKDrivingSettings.vehicleSeatInfo.useGrabbingHandID) {
				setGrabbingHandState (0, true);
				setGrabbingHandState (0, false);
			}

			IKDrivingSettings = null;
		}
	}

	public bool checkIfDeactivateIKIfNotAimingActive (IKWeaponInfo IKPositions)
	{
		if (playerControllerManager.isCrouching ()) {
			return true;
		}

		if (usingDualWeapon) {
			if (IKWeaponsSettings.usedOnRightHand) {
				if (IKPositions.rightHandDualWeaopnInfo.deactivateIKIfNotAiming) {
					return true;
				}
			} else {
				if (IKPositions.leftHandDualWeaponInfo.deactivateIKIfNotAiming) {
					return true;
				}
			}
		} else {
			if (IKPositions.deactivateIKIfNotAiming) {
				return true;
			}
		}

		return false;
	}

	public void setIKWeaponState (bool state, IKWeaponInfo IKPositions, bool useHandToDrawWeapon, string weaponName)
	{
		//move hands through the waypoints to grab the weapon
		IKWeaponsSettings = IKPositions;

//		print ("SET IK WEAPON STATE " + state + " " + useHandToDrawWeapon + " " + weaponName);

		bool useDrawKeepWeaponAnimation = IKPositions.useDrawKeepWeaponAnimation &&
		                                  !weaponsManager.ignoreUseDrawKeepWeaponAnimation &&
		                                  !weaponsManager.isUsingDualWeapons ();

		if (useDrawKeepWeaponAnimation) {
			if (state) {
				weaponsManager.weaponReadyToMove ();
			}
		} else {
			//the player is drawing a weapon
			if (state) {
				setUsingWeaponsState (state);

				bool deactivateIKIfNotAiming = checkIfDeactivateIKIfNotAimingActive (IKPositions);

				if (deactivateIKIfNotAiming) {
					setDisableSingleWeaponState (false);
				}

				if (usingDualWeapon) {
					if (IKWeaponsSettings.usedOnRightHand) {
						currentIKWeaponsPosition = IKWeaponsSettings.rightHandDualWeaopnInfo.handsInfo [0];
					} else {
						currentIKWeaponsPosition = IKWeaponsSettings.leftHandDualWeaponInfo.handsInfo [0];
					}
					
					stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);

					setIKHandTargetValue (currentIKWeaponsPosition, 1);

					currentIKWeaponsPosition.handMovementCoroutine = 
					StartCoroutine (moveThroughWaypoints (currentIKWeaponsPosition, false, deactivateIKIfNotAiming, IKWeaponsSettings.usedOnRightHand, 
						IKWeaponsSettings.useQuickDrawKeepWeapon, weaponName));

				} else {
					for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
						currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

						if ((currentIKWeaponsPosition.usedToDrawWeapon && useHandToDrawWeapon) || (!currentIKWeaponsPosition.usedToDrawWeapon && !useHandToDrawWeapon)) {
							stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);

							setIKHandTargetValue (currentIKWeaponsPosition, 1);

							currentIKWeaponsPosition.handMovementCoroutine = 
							StartCoroutine (moveThroughWaypoints (currentIKWeaponsPosition, false, IKPositions.deactivateIKIfNotAiming,
								IKWeaponsSettings.usedOnRightHand, IKWeaponsSettings.useQuickDrawKeepWeapon, weaponName));
						}
					}
				}
			} else {
				//the player is keeping a weapon
				if (!usingDualWeapon) {
					for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
						currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

						if (!currentIKWeaponsPosition.usedToDrawWeapon) {
							stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);

							currentIKWeaponsPosition.handMovementCoroutine = 
							StartCoroutine (moveThroughWaypoints (currentIKWeaponsPosition, true, IKPositions.deactivateIKIfNotAiming, 
								IKWeaponsSettings.usedOnRightHand, IKWeaponsSettings.useQuickDrawKeepWeapon, weaponName));
						} else {
							if (!currentIKWeaponsPosition.handInPositionToAim) {
								stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);

								setIKHandTargetValue (currentIKWeaponsPosition, 0);
				
								setDisableSingleWeaponState (true);
							}

							checkGrabbingHandsState (false, currentIKWeaponsPosition.useGrabbingHandID, 
								currentIKWeaponsPosition.grabbingHandID, currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand);
						}
					}

					IKWeaponsSettings.handsInPosition = false;
				}
			}
		}
	}

	IEnumerator moveThroughWaypoints (IKWeaponsPosition IKWeapon, bool keepingWeapon, bool deactivateIKIfNotAiming, 
	                                  bool isUsedOnDualRightHand, bool useQuickDrawKeepWeapon, string weaponName)
	{
		bool playerIsCrouching = playerControllerManager.isCrouching ();

		deactivateIKIfNotAiming = deactivateIKIfNotAiming || playerIsCrouching;

		if (IKWeapon.handUsedInWeapon) {
			Transform follower = IKWeapon.waypointFollower;
			List<Transform> wayPoints = new List<Transform> (IKWeapon.wayPoints);

			if (keepingWeapon) {
				wayPoints.Reverse ();
				wayPoints.RemoveAt (0);
			}

			follower.position = IKWeapon.handTransform.position;
			follower.rotation = IKWeapon.handTransform.rotation;
			IKWeapon.transformFollowByHand = follower;

			if (!useQuickDrawKeepWeapon || !IKWeapon.usedToDrawWeapon) {
				if (!deactivateIKIfNotAiming || IKWeapon.usedToDrawWeapon) {

					bool targetReached = false;

					float angleDifference = 0;

					float movementTimer = 0;

					foreach (Transform transformPath in wayPoints) {
						// find the distance to travel
						float dist = GKC_Utils.distance (follower.position, transformPath.position); 

						// calculate the movement duration
						float duration = dist / IKWeapon.handMovementSpeed; 
						float t = 0;

						targetReached = false;

						angleDifference = 0;

						movementTimer = 0;

						float handDistance = 0;

						while (!targetReached) {
							t += Time.deltaTime / duration;
							follower.position = Vector3.Lerp (follower.position, transformPath.position, t);
							follower.rotation = Quaternion.Slerp (follower.rotation, transformPath.rotation, t);

							angleDifference = Quaternion.Angle (follower.rotation, transformPath.rotation);

							movementTimer += Time.deltaTime;

							dist = GKC_Utils.distance (transformPath.position, IKWeapon.handTransform.position); 

							handDistance = GKC_Utils.distance (transformPath.position, IKWeapon.handTransform.position);

							if ((dist < 0.02f && angleDifference < 0.02f && handDistance < 0.02f) || movementTimer > (duration + 0.5f)) {
//								print ("reached");
								targetReached = true;
							} else {
//								print ("moving " + handDistance + " " + dist + " " + angleDifference + " " + movementTimer);
							}

							yield return null;
						}
					}
				}
			}

			if (keepingWeapon) {
				IKWeapon.handInPositionToAim = false;

				setIKHandTargetValue (IKWeapon, 0);

				bool usingCurrentWeapon = weaponsManager.isPlayerUsingWeapon (weaponName);

				if (usingCurrentWeapon || weaponsManager.isUsingDualWeapons ()) {
					setDisableSingleWeaponState (true);

//					print ("player is using " + weaponName + "setting keep state");
				} else {
//					print ("player is not using " + weaponName + " avoiding to disable weapons");
				}

				IKWeapon.elbowInfo.targetValue = 0;
			} else {
				IKWeapon.handInPositionToAim = true;
				IKWeapon.transformFollowByHand = IKWeapon.position;

				if (IKWeapon.usedToDrawWeapon) {
					if (usingDualWeapon) {
						if (useQuickDrawKeepWeapon) {
							weaponsManager.dualWeaponReadyToMoveDirectlyOnDrawHand (isUsedOnDualRightHand);
						} else {
							weaponsManager.dualWeaponReadyToMove (isUsedOnDualRightHand);
						}
					} else {
						if (useQuickDrawKeepWeapon) {
							weaponsManager.weaponReadyToMoveDirectlyOnDrawHand ();
						} else {
							weaponsManager.weaponReadyToMove ();
						}
					}
				} else {
					IKWeapon.elbowInfo.targetValue = 1;
				}

				if (deactivateIKIfNotAiming && !IKWeapon.usedToDrawWeapon) {

					setIKHandTargetValue (IKWeapon, 0);

					IKWeapon.elbowInfo.targetValue = 0;
				}

				if (!deactivateIKIfNotAiming && !IKWeapon.usedToDrawWeapon) {
					setDisableSingleWeaponState (false);
				}

				checkHandsInPosition (true);
			}

			checkGrabbingHandsState (!keepingWeapon, IKWeapon.useGrabbingHandID, IKWeapon.grabbingHandID, IKWeapon.limb == AvatarIKGoal.RightHand);
		} else {
			if (keepingWeapon) {
				IKWeapon.handInPositionToAim = false;
			
				setDisableSingleWeaponState (true);
			} else {
				IKWeapon.handInPositionToAim = true;

				if (IKWeapon.usedToDrawWeapon) {
					
					if (usingDualWeapon) {
						weaponsManager.dualWeaponReadyToMove (isUsedOnDualRightHand);
					} else {
						weaponsManager.weaponReadyToMove ();
					}
				} 

				checkHandsInPosition (true);
			}
		}
	}

	public void checkGrabbingHandsState (bool drawingWeapon, bool useGrabbingHandID, int grabbingHandID, bool isRightHand)
	{
		if (useGrabbingHandID) {
			int currentGrabbingHandID = 0;

			if (drawingWeapon) {
				currentGrabbingHandID = grabbingHandID;
			}

			setGrabbingHandState (currentGrabbingHandID, isRightHand);
		}
	}

	public void setGrabbingHandState (int grabbingHandID, bool isRightHand)
	{
		if (!componentEnabled) {
			return;
		}

		if (isRightHand) {
			animator.SetInteger (rightHandGrabbingIDName, grabbingHandID);
		} else {
			animator.SetInteger (leftHandGrabbingIDName, grabbingHandID);
		}
	}

	public void checkHandsInPosition (bool drawingWeapon)
	{
		if (usingDualWeapon) {
			if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo.Count > 0) {

				currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];
				if (currentIKWeaponsPosition.handInPositionToAim == drawingWeapon) {
					IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInPosition = drawingWeapon;
				}
			}

			if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo.Count > 0) {

				currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];
				if (currentIKWeaponsPosition.handInPositionToAim == drawingWeapon) {
					IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInPosition = drawingWeapon;
				}
			}
		} else {
			int numberOfHandsInPosition = 0;

			for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
				if (IKWeaponsSettings.handsInfo [i].handInPositionToAim == drawingWeapon) {
					numberOfHandsInPosition++;
				}
			}

			if (numberOfHandsInPosition == 2) {
				IKWeaponsSettings.handsInPosition = drawingWeapon;
			}
		}
	}

	public void stopIKWeaponsActions ()
	{
		if (usingDualWeapon) {
			if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}

			if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}
		} else {
			if (IKWeaponsSettings != null) {
				for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
					currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

					stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
				}
			}
		}
			
		setUsingWeaponsState (false);

		usingDualWeapon = false;

		setDisableWeaponsState (false);
	}

	public void quickDrawWeaponState (IKWeaponInfo IKPositions)
	{
		IKWeaponsSettings = IKPositions;

		if (usingDualWeapon) {
			if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}

			if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}
		} else {
			if (IKWeaponsSettings != null) {
				for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
					currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

					stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
				}
			}
		}

		setUsingWeaponsState (true);

		if (usingDualWeapon) {
			if (IKWeaponsSettings.usedOnRightHand) {
				currentIKWeaponsPosition = IKWeaponsSettings.rightHandDualWeaopnInfo.handsInfo [0];
			} else {
				currentIKWeaponsPosition = IKWeaponsSettings.leftHandDualWeaponInfo.handsInfo [0];
			}

			bool deactivateIKIfNotAiming = checkIfDeactivateIKIfNotAimingActive (IKPositions);

			float targetValue = 1;
			if (deactivateIKIfNotAiming) {
				targetValue = 0;
			}

			setIKHandTargetValue (currentIKWeaponsPosition, targetValue);

			currentIKWeaponsPosition.HandIKWeight = targetValue;
			currentIKWeaponsPosition.elbowInfo.targetValue = targetValue;
			currentIKWeaponsPosition.elbowInfo.elbowIKWeight = targetValue;

			currentIKWeaponsPosition.handInPositionToAim = true;
			currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;
		} else {
			for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {

				currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

				float targetValue = 1;
				if (IKWeaponsSettings.deactivateIKIfNotAiming || playerControllerManager.isCrouching ()) {
					targetValue = 0;
				}

				if (IKWeaponsSettings.usingWeaponAsOneHandWield && !currentIKWeaponsPosition.usedToDrawWeapon) {
					targetValue = 0;
				}

				setIKHandTargetValue (currentIKWeaponsPosition, targetValue);

				currentIKWeaponsPosition.HandIKWeight = targetValue;
				currentIKWeaponsPosition.elbowInfo.targetValue = targetValue;
				currentIKWeaponsPosition.elbowInfo.elbowIKWeight = targetValue;

				currentIKWeaponsPosition.handInPositionToAim = true;
				currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;
			}
		}

		checkHandsInPosition (true);
	}

	public void quickDrawWeaponStateDualWeapon (IKWeaponInfo IKPositions, bool isRigthWeapon)
	{
		IKWeaponsSettings = IKPositions;

		if (isRigthWeapon) {
			if (IKWeaponsRightHandSettings != null && IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsRightHandSettings.rightHandDualWeaopnInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}
		} else {

			if (IKWeaponsLeftHandSettings != null && IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo.Count > 0) {
				currentIKWeaponsPosition = IKWeaponsLeftHandSettings.leftHandDualWeaponInfo.handsInfo [0];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}
		}

		setUsingWeaponsState (true);

		if (IKWeaponsSettings.usedOnRightHand) {
			currentIKWeaponsPosition = IKWeaponsSettings.rightHandDualWeaopnInfo.handsInfo [0];
		} else {
			currentIKWeaponsPosition = IKWeaponsSettings.leftHandDualWeaponInfo.handsInfo [0];
		}

		bool deactivateIKIfNotAiming = checkIfDeactivateIKIfNotAimingActive (IKPositions);

		float targetValue = 1;
		if (deactivateIKIfNotAiming) {
			targetValue = 0;
		}

		setIKHandTargetValue (currentIKWeaponsPosition, targetValue);

		currentIKWeaponsPosition.HandIKWeight = targetValue;
		currentIKWeaponsPosition.elbowInfo.targetValue = targetValue;
		currentIKWeaponsPosition.elbowInfo.elbowIKWeight = targetValue;

		currentIKWeaponsPosition.handInPositionToAim = true;
		currentIKWeaponsPosition.transformFollowByHand = currentIKWeaponsPosition.position;

		checkHandsInPosition (true);
	}

	public void resetWeaponHandIKWeight ()
	{
		if (IKWeaponsSettings != null) {
			for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
				IKWeaponsSettings.handsInfo [i].HandIKWeight = 0;
			}
		}
	}

	public void stopIKWeaponHandMovementCoroutine (IKWeaponsPosition IKWeaponPositionToCheck)
	{
		if (IKWeaponPositionToCheck.handMovementCoroutine != null) {
			StopCoroutine (IKWeaponPositionToCheck.handMovementCoroutine);
		}
	}

	public void quickKeepWeaponState ()
	{
		setDisableWeaponsState (true);
	}

	public void setDisableWeaponsState (bool state)
	{
		setDisableSingleWeaponState (state);

		setDisableDualWeaponsState (state);
	}

	public void setDisableSingleWeaponState (bool state)
	{
		disableWeapons = state;
	}

	public void setDisableDualWeaponsState (bool state)
	{
		disablingDualWeapons = state;
	}

	public void disableIKWeight ()
	{
		headWeightTarget = 0;
		currentHeadWeight = 0;
	}

	public void setUsingWeaponsState (bool state)
	{
		usingWeapons = state;
	}

	public void setUsingDualWeaponState (bool state)
	{
		if (state) {
			usingDualWeapon = state;
		} else {
			setDisableDualWeaponsState (true);
		}
	}

	public void disableUsingDualWeaponState ()
	{
		usingDualWeapon = false;

		setDisableDualWeaponsState (false);
	}

	public void setIKWeaponsRightHandSettings (IKWeaponInfo IKPositions)
	{
		IKWeaponsRightHandSettings = IKPositions;
	}

	public void setIKWeaponsLeftHandSettings (IKWeaponInfo IKPositions)
	{
		IKWeaponsLeftHandSettings = IKPositions;
	}

	public void startRecoil (float recoilSpeed, float recoilAmount)
	{
		if (powerHandRecoil != null) {
			StopCoroutine (powerHandRecoil);
		}

		powerHandRecoil = StartCoroutine (recoilMovementBack (recoilSpeed, recoilAmount));
	}

	IEnumerator recoilMovementBack (float recoilSpeed, float recoilAmount)
	{
		originalCurrentDist = currentDist;
		float newDist = currentDist - recoilAmount;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * recoilSpeed;
			currentDist = Mathf.Lerp (currentDist, newDist, t);
			yield return null;
		}

		powerHandRecoil = StartCoroutine (recoilMovementForward (recoilSpeed));
	}

	IEnumerator recoilMovementForward (float recoilSpeed)
	{
		for (float t = 0; t < 1;) {
			t += Time.deltaTime * recoilSpeed;
			currentDist = Mathf.Lerp (currentDist, originalCurrentDist, t);
			yield return null;
		}
	}

	public void setElbowsIKTargetValue (float leftValue, float rightValue)
	{
		for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
			currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

			if (currentIKWeaponsPosition.limb == AvatarIKGoal.LeftHand) {
				currentIKWeaponsPosition.elbowInfo.targetValue = leftValue;
			}

			if (currentIKWeaponsPosition.limb == AvatarIKGoal.RightHand) {
				currentIKWeaponsPosition.elbowInfo.targetValue = rightValue;
			}
		}
	}

	public void setIKHandTargetValue (IKWeaponsPosition IKWeaponPositionToCheck, float newTargetValue)
	{
		IKWeaponPositionToCheck.targetValue = newTargetValue;

//		print ((IKWeaponPositionToCheck.limb == AvatarIKGoal.RightHand) + " " + newTargetValue);
	}

	public void ziplineState (bool state, zipline.IKZiplineInfo IKPositions)
	{
		usingZipline = state;

		if (usingZipline) {
			IKWeightTargetValue = 1;
			IKZiplineSettings = IKPositions;
		} else {
			IKWeightTargetValue = 0;
			IKZiplineSettings = null;
		}
	}

	public void setGrabedObjectState (bool state, bool IKSystemEnabledToGrabState, List<grabObjects.handInfo> handInfo)
	{
		objectGrabbed = state;

		IKSystemEnabledToGrab = IKSystemEnabledToGrabState;

		if (objectGrabbed) {
			handInfoList = handInfo;

			for (int i = 0; i < handInfoList.Count; i++) {
				if (handInfoList [i].useAnimationGrabbingHand) {
					if (handInfoList [i].IKGoal == AvatarIKGoal.RightHand) {
						setGrabbingHandState (handInfoList [i].grabbingHandID, true);
					} else {
						setGrabbingHandState (handInfoList [i].grabbingHandID, false);
					}
				}
			}
		} else {
			setGrabbingHandState (0, true);
			setGrabbingHandState (0, false);
		}
	}

	public void setIKBodyInfoListOriginalPosition ()
	{
		if (!IKBodyInfoStateListOriginalPositionAssigned) {
			IKBodyInfoStateListOriginalPositionAssigned = true;

			for (int i = 0; i < IKBodyInfoStateList.Count; i++) {
				for (int j = 0; j < IKBodyInfoStateList [i].IKBodyInfoList.Count; j++) {
					IKBodyInfoStateList [i].IKBodyInfoList [j].originalPosition = IKBodyInfoStateList [i].IKBodyInfoList [j].targetToFollow.localPosition;
				}
			}
		}
	}

	bool IKBodyPaused;

	public void setIKBodyPausedState (bool state)
	{
		IKBodyPaused = state;
	}

	public void setIKBodyState (bool state, string IKBodyStateName)
	{
		setIKBodyInfoListOriginalPosition ();

		for (int j = 0; j < IKBodyInfoStateList.Count; j++) {
			if (IKBodyInfoStateList [j].Name.Equals (IKBodyStateName)) {
				currentIKBodyInfoState = IKBodyInfoStateList [j];
				currentIKBodyInfoList = currentIKBodyInfoState.IKBodyInfoList;

				currentIKBodyInfoState.currentlyActive = state;
			}
		}

		if (state) {
			for (int j = 0; j < currentIKBodyInfoList.Count; j++) {
				currentIKBodyInfoList [j].targetToFollow.localPosition = currentIKBodyInfoList [j].originalPosition +
				(Vector3.up * currentIKBodyInfoState.extraVerticalPositionAtStartMultiplier);

				currentIKBodyInfoList [j].posTargetY = currentIKBodyInfoState.extraVerticalPositionAtStart;
			}
		}
			
		if (state) {
			IKBodyInfoActive = true;
			disablingIKBodyInfoActive = false;
		} else {
			disablingIKBodyInfoActive = true;
		}

		for (int j = 0; j < currentIKBodyInfoList.Count; j++) {
			if (state) {
				currentIKBodyInfoList [j].IKBodyWeigthTarget = 1;

				if (currentIKBodyInfoList [j].isHand) {
					if (usingHands) {
						if (!currentIKBodyInfoList [j].bodyPartBusy) {
							currentIKBodyInfoList [j].bodyPartBusy = true;
							currentIKBodyInfoList [j].IKBodyWeigthTarget = 0;
						}
					}
				}
			} else {
				currentIKBodyInfoList [j].IKBodyWeigthTarget = 0;

				if (currentIKBodyInfoList [j].isHand) {
					if (usingHands) {
						if (!currentIKBodyInfoList [j].bodyPartBusy) {
							currentIKBodyInfoList [j].bodyPartBusy = true;
							currentIKBodyInfoList [j].IKBodyWeigthTarget = 0;
						}
					}
				}
			}
		}

		if (!state) {
			resetBodyRotation ();
		}

		lastTimeIkBodyStateActive = Time.time;
	}

	float lastTimeIkBodyStateActive;

	Coroutine bodyRotationCoroutine;

	public void resetBodyRotation ()
	{
		if (bodyRotationCoroutine != null) {
			StopCoroutine (bodyRotationCoroutine);
		}

		if (currentIKBodyInfoState == null) {
			return;
		}

		bodyRotationCoroutine = StartCoroutine (resetBodyRotationCoroutine ());
	}

	IEnumerator resetBodyRotationCoroutine ()
	{
		targetRotation = Vector3.zero;

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * currentIKBodyInfoState.IKBodyCOMRotationSpeed;

			IKBodyCOM.localRotation = Quaternion.Lerp (IKBodyCOM.localRotation, Quaternion.Euler (targetRotation), t);

			yield return null;
		}
	}

	public Transform getIKBodyCOM ()
	{
		return IKBodyCOM;
	}

	public Transform getRightHandMountPoint ()
	{
		return rightHandMountPoint;
	}

	public Transform getLeftHandMountPoint ()
	{
		return leftHandMountPoint;
	}

	public Transform getRightLowerArmMountPoint ()
	{
		return rightLowerArmMountPoint;
	}

	public Transform getLeftLowerArmMountPoint ()
	{
		return leftLowerArmMountPoint;
	}

	public void setCurrentAimModeType (string newAimModeTypeName)
	{
		if (newAimModeTypeName.Equals ("Weapons")) {
			currentAimMode = aimMode.weapons;
		}

		if (newAimModeTypeName.Equals ("Powers")) {
			currentAimMode = aimMode.hands;
		}
	}

	public void stopAllMovementCoroutines ()
	{
		if (powerHandRecoil != null) {
			StopCoroutine (powerHandRecoil);
		}

		if (bodyRotationCoroutine != null) {
			StopCoroutine (bodyRotationCoroutine);
		}

		if (IKWeaponsSettings != null) {
			for (int i = 0; i < IKWeaponsSettings.handsInfo.Count; i++) {
				currentIKWeaponsPosition = IKWeaponsSettings.handsInfo [i];

				stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
			}
		}
	}

	public void enableOrDisableIKSystemManagerState (bool state)
	{
		if (!state) {
			disableAllIKStates ();
		}

		enabled = state;

		componentEnabled = state;
	}

	public void disableAllIKStates ()
	{
		if (currentIKWeaponsPosition != null) {
			stopIKWeaponHandMovementCoroutine (currentIKWeaponsPosition);
		}

		disableArmsState ();

		usingHands = false;

		stopIKWeaponsActions ();

		resetIKCOMRotationChecked = false;

		headLookStateEnabled = false;

		IKBodyInfoActive = false;

		disablingIKBodyInfoActive = false;

		headWeightTarget = 0;

		if (driving) {
			if (currentIKDrivingPositions != null) {
				setDrivingState (false, IKDrivingSettings);
			}
		}

		if (usingZipline) {
			if (IKZiplineSettings != null) {
				ziplineState (false, IKZiplineSettings);
			}
		}

		objectGrabbed = false;

		IKSystemEnabledToGrab = false;
	}

	public void setTemporalOnAnimatorIKComponentActive (OnAnimatorIKComponent newTemporalOnAnimatorIKComponent)
	{
		setTemporalOnAnimatorIKComponent (newTemporalOnAnimatorIKComponent, true);
	}

	public void setTemporalOnAnimatorIKComponentDeactivate (OnAnimatorIKComponent newTemporalOnAnimatorIKComponent)
	{
		setTemporalOnAnimatorIKComponent (newTemporalOnAnimatorIKComponent, false);
	}

	public void setTemporalOnAnimatorIKComponent (OnAnimatorIKComponent newTemporalOnAnimatorIKComponent, bool state)
	{
		temporalOnAnimatorIKComponent = newTemporalOnAnimatorIKComponent;

		if (temporalOnAnimatorIKComponent != null) {
			temporalOnAnimatorIKComponent.setActiveState (state);
		}

		temporalOnAnimatorIKComponentAssigned = temporalOnAnimatorIKComponent != null;
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo && !Application.isPlaying) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawCube (leftHandIKPos.transform.position, Vector3.one / 10);
			Gizmos.DrawCube (rightHandIKPos.transform.position, Vector3.one / 10);
		}
	}

	[System.Serializable]
	public class IKSettings
	{
		public float weight;
		public float bodyWeight;
		public float headWeight;
		public float eyesWeight;
		public float clampWeight;
	}

	[System.Serializable]
	public class IKBodyInfoState
	{
		[Header ("Main Setting")]
		[Space]

		public string Name;
		public List<IKBodyInfo> IKBodyInfoList = new List<IKBodyInfo> ();

		[Space]
		[Header ("Speed Movement Settings")]
		[Space]

		public float IKBodyCOMRotationAmountX = 20;
		public float IKBodyComRotationAmountY = 20;
		public float IKBodyCOMRotationSpeed = 2;

		public bool increaseRotationAmountWhenHigherSpeed;
		public float increasedIKBodyCOMRotationAmountX = 20;
		public float increasedIKBodyComRotationAmountY = 20;
		public float increasedIKBodyCOMRotationSpeed = 2;

		public float busyBodyPartWeightLerpSpeed = 3;

		[Space]
		[Header ("Other Settings")]
		[Space]

		public float delayForVerticalMovement = 1;

		public float verticalMovementTransitionSpeed = 5;

		public float extraVerticalPositionAtStart = 0.5f;
		public float extraVerticalPositionAtStartMultiplier = 0.5f;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool currentlyActive;

		[HideInInspector] public float currentIKBodyCOMRotationAmountX;
		[HideInInspector] public float currentIKBodyCOMRotationAmountY;

		[HideInInspector] public float currentIKBodyCOMRotationSpeed = 2;
	}

	[System.Serializable]
	public class IKBodyInfo
	{
		[Header ("Main Setting")]
		[Space]

		public string Name;
		public bool IKBodyPartEnabled = true;

		public Transform targetToFollow;

		public AvatarIKGoal IKGoal;
		public bool isHand;

		public bool useIKHint;
		public AvatarIKHint IKHint;

		public bool bodyPartBusy;

		public float currentIKWeight;
		public float IKBodyWeigthTarget;

		public float weightLerpSpeed;

		[HideInInspector] public Vector3 newPosition;

		public bool useSin = true;

		[HideInInspector] public Vector3 IKGoalPosition;
		[HideInInspector] public Quaternion IKGoalRotation;

		public float limbsMovementSpeed;
		public float limbMovementAmount;
		public float limbVerticalMovementSpeed;

		public float slowLimbVerticalMovementSpeed = 0.2f;

		[HideInInspector] public float currentLimbVerticalMovementSpeed;

		public Vector2 minClampPosition;
		public Vector2 maxClampPosition;
		public Vector3 originalPosition;

		public float posTargetY;
	}
}