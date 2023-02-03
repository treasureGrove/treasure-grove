using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class grabbedObjectMeleeAttackSystem : MonoBehaviour
{
	//	[Header ("Main Settings")]
	//	[Space]

	public bool grabbedObjectMeleeAttackEnabled = true;

	public bool canGrabMeleeObjectsEnabled = true;

	public bool useAttackTypes;

	public float generalAttackDamageMultiplier = 1;

	public List<string> meleeAttackTypes = new List<string> ();

	public bool playerOnGroundToActivateAttack = true;

	public bool disableGrabObjectsInputWhenCarryingMeleeWeapon;

	public bool useThrowReturnMeleeWeaponSystemEnabled;

	//	[Space]
	//	[Header ("Damage Detection Settings")]
	//	[Space]

	public bool useCustomLayerToDetectSurfaces;
	public LayerMask customLayerToDetectSurfaces;

	public bool useCustomIgnoreTags;
	public List<string> customTagsToIgnoreList = new List<string> ();

	public bool checkSurfacesWithCapsuleRaycastEnabled = true;

	public bool ignoreGetHealthFromDamagingObjects = true;

	public bool ignoreGetHealthFromDamagingObjectsOnWeaponInfo = true;

	public bool ignoreCutModeOnMeleeWeaponAttack = true;

	//	[Space]
	//	[Header ("Weapon Info List Settings")]
	//	[Space]

	public List<grabbedWeaponInfo> grabbedWeaponInfoList = new List<grabbedWeaponInfo> ();

	//	[Space]
	//	[Header ("Surface Info List Settings")]
	//	[Space]

	public bool checkSurfaceInfoEnabled = true;

	public string surfaceInfoOnMeleeAttackNameForNotFound = "Regular";
	public string surfaceInfoOnMeleeAttackNameForSwingOnAir = "Swing On Air";
	public List<surfaceInfoOnMeleeAttack> surfaceInfoOnMeleeAttackList = new List<surfaceInfoOnMeleeAttack> ();

	//	[Space]
	//	[Header ("Stamina Settings")]
	//	[Space]

	public bool useStaminaOnAttackEnabled = true;
	public string attackStaminaStateName = "Melee Attack With Grabbed Object";

	public float generalStaminaUseMultiplier = 1;

	//	[Space]
	//	[Header ("Cutting Mode Settings")]
	//	[Space]

	public bool cuttingModeEnabled = true;

	public UnityEvent eventOnCuttingModeStart;
	public UnityEvent eventOnCuttingModeEnd;

	//	[Space]
	//	[Header ("Block Mode Settings")]
	//	[Space]

	public bool blockModeEnabled = true;
	public float generalBlockProtectionMultiplier = 1;

	public string cancelBlockReactionStateName = "Disable Has Exit Time State";

	//	[Space]
	//	[Header ("Shield Settings")]
	//	[Space]

	public bool shieldCanBeUsedWithoutMeleeWeapon;

	public Transform rightHandMountPoint;
	public Transform leftHandMountPoint;
	public Transform shieldRightHandMountPoint;
	public Transform shieldLeftHandMountPoint;

	public Transform shieldBackMountPoint;

	//	[Space]

	public UnityEvent eventToActivateMeleeModeWhenUsingShieldWithoutMeleeWeapon;

	//	[Space]
	//	[Header ("Match Target Settings")]
	//	[Space]

	public bool useMatchTargetSystemOnAttack;
	public bool ignoreAttackSettingToMatchTarget;
	public matchPlayerToTargetSystem mainMatchPlayerToTargetSystem;

	//	[Space]
	//	[Header ("Weapon Durability Settings")]
	//	[Space]

	public bool checkDurabilityOnAttackEnabled;
	public bool checkDurabilityOnBlockEnabled;

	public float generalAttackDurabilityMultiplier = 1;
	public float generalBlockDurabilityMultiplier = 1;
	//	[Space]

	public bool useEventOnDurabilityEmptyOnMeleeWeapon;
	public UnityEvent eventOnDurabilityEmptyOnMeleeWeapon;

	//	[Space]

	public bool useEventOnDurabilityEmptyOnBlock;
	public UnityEvent eventOnDurabilityEmptyOnBlock;

	//	[Space]
	//	[Header ("Other Settings")]
	//	[Space]

	public bool weaponsCanBeStolenFromCharacter = true;

	public string mainMeleeCombatAxesInputName = "Grab Objects";
	public string mainMeleeCombatBlockInputName = "Block Attack";

	public bool ignoreUseDrawKeepWeaponAnimation;

	//	[Space]
	//	[Header ("Weapon Extra Action Settings")]
	//	[Space]

	public bool showExtraActionIconTexture;
	public GameObject extraActionIconGameObject;
	public RawImage extraActionRawImage;

	//	[Space]
	//	[Header ("Debug")]
	//	[Space]

	public bool showDebugPrint;

	public bool grabbedObjectMeleeAttackActive;

	public bool carryingObject;

	public bool attackInProcess;

	public List<int> meleeAttackTypesAmount = new List<int> ();

	public hitCombat currentHitCombat;

	public hitCombat dualWieldHitCombat;

	public int currentAttackIndex;

	public bool blockActive;

	public bool blockActivePreviously;

	public bool objectThrown;


	public bool cuttingModeActive;

	public bool attackInputPausedForStamina;

	public bool damageTriggerInProcess;

	public bool meleeAttackInputPaused;

	public bool reducedBlockDamageProtectionActive;

	public bool canCancelBlockToStartAttackActive;

	public bool blockInputPaused;

	public bool isDualWieldWeapon;
	public dualWieldMeleeWeaponObjectSystem mainDualWieldMeleeWeaponObjectSystem;

	public bool throwWeaponActionPaused;

	public bool showDebugDraw;

	//	[Space]
	//	[Header ("Shield Debug")]
	//	[Space]

	public bool shieldActive;

	public bool carryingShield;

	public string currentShieldName;

	public GameObject currentShieldGameObject;

	public Transform currentShieldHandMountPointTransformReference;
	public Transform currentShieldBackMountPointTransformReference;


	//	[Space]
	//	[Header ("Event Settings")]
	//	[Space]

	public bool useEventsOnAttack;
	public UnityEvent eventOnAttackStart;
	public UnityEvent eventOnAttackEnd;

	public bool useEventsOnBlockDamage;
	public UnityEvent eventOnBlockActivate;
	public UnityEvent eventOnBlockDeactivate;

	public bool useEventsOnGrabDropObject;
	public UnityEvent eventOnGrabObject;
	public UnityEvent eventOnDropObject;

	public bool useEventsOnAttackCantBeBlocked;
	public UnityEvent eventOnAttackCantBeBlocked;

	public bool hideInventoryQuickAccessSlotsWhenCarryingMeleeWeapon;
	public UnityEvent eventOnHideInventoryQuickAccessSlots;
	public UnityEvent eventOnShowInventoryQuickAccessSlots;

	//	[Space]
	//	[Header ("Components")]
	//	[Space]

	public Transform currentGrabbedObjectTransform;
	public remoteEventSystem mainRemoteEventSystem;

	public GameObject playerControllerGameObject;
	public playerController mainPlayerController;

	public grabObjects mainGrabObjects;
	public health mainHealth;
	public Transform mainCameraTransform;

	public staminaSystem mainStaminaSystem;

	public Transform handPositionReference;

	public AudioSource mainAudioSource;

	public sliceSystem mainSliceSystem;

	public Animator mainAnimator;

	public findObjectivesSystem mainFindObjectivesSystem;

	public meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager;

	public playerInputManager playerInput;

	public Transform parentToPlaceTriggerInFrontOfCharacter;

	public meleeCombatThrowReturnWeaponSystem mainMeleeCombatThrowReturnWeaponSystem;


	Transform objectRotationPoint;

	grabPhysicalObjectMeleeAttackSystem currentMeleeWeapon;

	grabPhysicalObjectMeleeAttackSystem lastMeleeWeaponSystemUsed;

	string lastKeepWeaponActionName;

	Transform lastWeaponParent;

	Vector3 lastWeaponLocalPosition;
	Vector3 lastWeaponLocalEuler;

	int attackListCount;

	Coroutine attackCoroutine;

	float lastTimeAttackActive;

	float lastTimeAttackComplete;

	float lastTimeDrawMeleeWeapon = -1;

	grabPhysicalObjectMeleeAttackSystem.attackInfo currentAttackInfo;

	Coroutine damageTriggerCoroutine;

	Coroutine dualWieldDamageTriggerCoroutine;

	grabPhysicalObjectSystem currentGrabPhysicalObjectSystem;

	Rigidbody currentObjectRigidbody;

	grabbedWeaponInfo currentGrabbedWeaponInfo;

	float capsuleCastRadius;
	float capsuleCastDistance;

	Vector3 currentRayOriginPosition;
	Vector3 currentRayTargetPosition;

	float distanceToTarget;
	Vector3 rayDirection;

	Vector3 point1;
	Vector3 point2;

	RaycastHit[] hits;

	Transform raycastCheckTransfrom;


	Vector3 originalHitCombatColliderSize;

	Vector3 originalHitCombatColliderPosition;

	BoxCollider currentHitCombatBoxCollider;

	Vector3 originalHitCombatColliderCenter;

	bool hitCombatParentChanged;

	bool isAttachedToSurface;

	bool surfaceDetectedIsDead;

	bool previousStrafeMode;

	bool currentAttackCanBeBlocked;

	Coroutine pauseBlockInputCoroutine;
	Coroutine disableHasExitTimeCoroutine;

	float lastTimeDamageTriggerActivated;

	bool attackActivatedOnAir;

	bool deflectProjectilesEnabled;

	bool deflectProjectilesOnBlockEnabled;

	float currentWaitTimeToNextAttack = 0;

	bool cutActivatedOnAttackChecked;

	LayerMask currentLayerMaskToCheck;

	float lastTimeCutActiveOnAttack = 0;

	float lastTimeSurfaceAudioPlayed;
	int lastSurfaceDetecetedIndex = -1;

	bool getHealthFromPerfectBlocksActive;
	float healthAmountFromPerfectBlocksValue = 1;

	bool checkDrawKeepWeaponAnimationPauseState;


	public bool isGrabbedObjectMeleeAttackEnabled ()
	{
		return grabbedObjectMeleeAttackEnabled;
	}

	public bool isCanGrabMeleeObjectsEnabled ()
	{
		return canGrabMeleeObjectsEnabled;
	}

	public Transform getRightHandMountPoint ()
	{
		return rightHandMountPoint;
	}

	public Transform getLeftHandMountPoint ()
	{
		return leftHandMountPoint;
	}

	public void checkGrabbedMeleeWeaponLocalPositionRotationValues ()
	{
		if (currentGrabbedWeaponInfo != null && currentGrabbedWeaponInfo.useCustomGrabbedWeaponReferencePosition) {
			currentGrabbedObjectTransform.localRotation = currentGrabbedWeaponInfo.customGrabbedWeaponReferencePosition.localRotation;
			currentGrabbedObjectTransform.localPosition = currentGrabbedWeaponInfo.customGrabbedWeaponReferencePosition.localPosition;
		}
	}

	public void checkKeepMeleeWeaponLocalPositionRotationValues ()
	{
		if (currentGrabbedWeaponInfo != null && currentGrabbedWeaponInfo.useCustomReferencePositionToKeepObject) {
			currentGrabbedObjectTransform.localRotation = currentGrabbedWeaponInfo.customReferencePositionToKeepObject.localRotation;
			currentGrabbedObjectTransform.localPosition = currentGrabbedWeaponInfo.customReferencePositionToKeepObject.localPosition;
		}
	}

	public Transform getCustomGrabbedWeaponReferencePosition ()
	{
		if (currentGrabbedWeaponInfo != null && currentGrabbedWeaponInfo.useCustomGrabbedWeaponReferencePosition) {
			return currentGrabbedWeaponInfo.customGrabbedWeaponReferencePosition;
		}

		return null;
	}

	public Transform getCustomReferencePositionToKeepObject ()
	{
		if (currentGrabbedWeaponInfo != null && currentGrabbedWeaponInfo.useCustomReferencePositionToKeepObject) {
			return currentGrabbedWeaponInfo.customReferencePositionToKeepObject;
		}

		return null;
	}

	public void setNewGrabPhysicalObjectSystem (grabPhysicalObjectSystem newGrabPhysicalObjectSystem)
	{
		currentGrabPhysicalObjectSystem = newGrabPhysicalObjectSystem;

		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setNewGrabPhysicalObjectSystem (currentGrabPhysicalObjectSystem);
		}
	}

	public void setCheckDrawKeepWeaponAnimationPauseState (bool state)
	{
		checkDrawKeepWeaponAnimationPauseState = state;
	}

	public void activateStartDrawAnimation ()
	{
		if (checkDrawKeepWeaponAnimationPauseState) {
			checkDrawKeepWeaponAnimationPauseState = false;

			return;
		}

		if (currentGrabbedWeaponInfo != null && currentMeleeWeapon.useDrawKeepWeaponAnimation && !ignoreUseDrawKeepWeaponAnimation) {
			mainPlayerController.activateCustomAction (currentMeleeWeapon.drawWeaponActionName);

			currentMeleeWeapon.enableOrDisableWeaponMeshActiveState (false);

			mainMeleeWeaponsGrabbedManager.enableOrDisableMeleeWeaponMeshOnCharacterBodyByName (true, currentMeleeWeapon.getWeaponName ());
		}
	}

	public void activateEndDrawAnimation ()
	{
		if (checkDrawKeepWeaponAnimationPauseState) {
			checkDrawKeepWeaponAnimationPauseState = false;

			return;
		}

		if (currentGrabbedWeaponInfo != null && currentMeleeWeapon.useDrawKeepWeaponAnimation && !ignoreUseDrawKeepWeaponAnimation) {
			currentMeleeWeapon.enableOrDisableWeaponMeshActiveState (true);

			mainMeleeWeaponsGrabbedManager.enableOrDisableMeleeWeaponMeshOnCharacterBodyByName (false, currentMeleeWeapon.getWeaponName ());
		
			lastWeaponParent = currentMeleeWeapon.transform.parent;

			lastMeleeWeaponSystemUsed = currentMeleeWeapon;

			lastKeepWeaponActionName = lastMeleeWeaponSystemUsed.keepWeaponActionName;

			lastWeaponLocalPosition = currentMeleeWeapon.transform.localPosition;
			lastWeaponLocalEuler = currentMeleeWeapon.transform.localEulerAngles;
		}
	}

	public void enableOrDisableWeaponMeshActiveState (bool state)
	{
		if (currentGrabbedWeaponInfo != null) {
			currentMeleeWeapon.enableOrDisableWeaponMeshActiveState (state);
		}
	}

	public void activateStartKeepAnimation ()
	{
		if (lastMeleeWeaponSystemUsed != null) {
			if (checkDrawKeepWeaponAnimationPauseState) {
				checkDrawKeepWeaponAnimationPauseState = false;

				removeDrawKeepWeaponAnimationInfo ();

				return;
			}

			if (lastMeleeWeaponSystemUsed.useDrawKeepWeaponAnimation &&
			    !ignoreUseDrawKeepWeaponAnimation &&
			    mainMeleeWeaponsGrabbedManager.isWeaponInGrabbedInfoList (lastMeleeWeaponSystemUsed.getWeaponName ())) {

				mainPlayerController.activateCustomAction (lastKeepWeaponActionName);
			} else {
				removeDrawKeepWeaponAnimationInfo ();
			}
		}
	}

	public void removeDrawKeepWeaponAnimationInfo ()
	{
		lastMeleeWeaponSystemUsed = null;

		lastWeaponParent = null;
	}

	public void placeLastWeaponOnCharacterHand ()
	{
		lastMeleeWeaponSystemUsed.transform.SetParent (lastWeaponParent);

		lastMeleeWeaponSystemUsed.transform.localPosition = lastWeaponLocalPosition;
		lastMeleeWeaponSystemUsed.transform.localEulerAngles = lastWeaponLocalEuler;

		lastMeleeWeaponSystemUsed.setMainObjectColliderEnabledState (false);

		lastMeleeWeaponSystemUsed.mainGrabPhysicalObjectSystem.setGrabObjectTriggerState (false);

		lastMeleeWeaponSystemUsed.gameObject.SetActive (true);

		mainMeleeWeaponsGrabbedManager.enableOrDisableMeleeWeaponMeshOnCharacterBodyByName (false, lastMeleeWeaponSystemUsed.getWeaponName ());

		currentObjectRigidbody.isKinematic = true;
	}

	public void activateEndKeepAnimation ()
	{
		if (lastMeleeWeaponSystemUsed != null) {
			lastMeleeWeaponSystemUsed.transform.SetParent (null);

			lastMeleeWeaponSystemUsed.gameObject.SetActive (false);

			lastMeleeWeaponSystemUsed.setMainObjectColliderEnabledState (true);

			lastMeleeWeaponSystemUsed.mainGrabPhysicalObjectSystem.setGrabObjectTriggerState (true);

			mainMeleeWeaponsGrabbedManager.enableOrDisableMeleeWeaponMeshOnCharacterBodyByName (true, lastMeleeWeaponSystemUsed.getWeaponName ());
		
			removeDrawKeepWeaponAnimationInfo ();
		}
	}

	public void setNewGrabPhysicalObjectMeleeAttackSystem (grabPhysicalObjectMeleeAttackSystem newGrabPhysicalObjectMeleeAttackSystem)
	{
		if (!grabbedObjectMeleeAttackEnabled) {
			return;
		}

//		print (newGrabPhysicalObjectMeleeAttackSystem.getWeaponName ());

		currentMeleeWeapon = newGrabPhysicalObjectMeleeAttackSystem;

		if (currentMeleeWeapon != null) {
			mainPlayerController.setPlayerUsingMeleeWeaponsState (true);

			currentGrabbedObjectTransform = currentMeleeWeapon.transform;

			currentObjectRigidbody = currentGrabbedObjectTransform.GetComponent<Rigidbody> ();

			objectRotationPoint = currentMeleeWeapon.objectRotationPoint;

			grabbedObjectMeleeAttackActive = true;

			isDualWieldWeapon = currentMeleeWeapon.isDualWieldWeapon;

			if (isDualWieldWeapon) {
				mainDualWieldMeleeWeaponObjectSystem = currentMeleeWeapon.mainDualWieldMeleeWeaponObjectSystem;

				dualWieldHitCombat = mainDualWieldMeleeWeaponObjectSystem.mainHitCombat;
			}

			currentHitCombat = currentMeleeWeapon.getMainHitCombat ();

			if (useCustomLayerToDetectSurfaces) {
				currentHitCombat.setCustomLayerMask (customLayerToDetectSurfaces);
			}

			if (useCustomIgnoreTags) {
				currentHitCombat.setCustomTagsToIgnore (customTagsToIgnoreList);
			} else {
				currentHitCombat.setCustomTagsToIgnore (null);
			}

			currentHitCombat.setCustomDamageCanBeBlockedState (true);

			currentHitCombatBoxCollider = currentHitCombat.getMainCollider ().GetComponent<BoxCollider> ();

			originalHitCombatColliderCenter = currentHitCombatBoxCollider.center;

			originalHitCombatColliderSize = currentHitCombatBoxCollider.size;

			originalHitCombatColliderPosition = currentHitCombat.transform.localPosition;

			currentHitCombat.getOwner (playerControllerGameObject);

			currentHitCombat.setMainColliderEnabledState (true);
		
			attackListCount = currentMeleeWeapon.getAttackListCount ();

			capsuleCastRadius = currentMeleeWeapon.capsuleCastRadius;

			capsuleCastDistance = currentMeleeWeapon.capsuleCastDistance;

			carryingObject = true;

			if (currentMeleeWeapon.disableMeleeObjectCollider) {
				setGrabbedObjectClonnedColliderEnabledState (false);
			}

			checkGrabbedWeaponInfoStateAtStart (currentMeleeWeapon.weaponInfoName, true);

			if (showDebugPrint) {
				print (currentGrabbedWeaponInfo.useEventsOnDamageDetected);
			}

			currentHitCombat.setSendMessageOnDamageDetectedState (currentGrabbedWeaponInfo.useEventsOnDamageDetected);

			if (currentGrabbedWeaponInfo.useEventsOnDamageDetected) {
				currentHitCombat.setCustomObjectToSendMessage (gameObject);
			}

			currentMeleeWeapon.useCustomReferencePositionToKeepObjectMesh = currentGrabbedWeaponInfo.useCustomReferencePositionToKeepObjectMesh;

			if (currentMeleeWeapon.useCustomReferencePositionToKeepObjectMesh) {
				currentMeleeWeapon.customReferencePositionToKeepObjectMesh = currentGrabbedWeaponInfo.customReferencePositionToKeepObjectMesh;
			}
				
			checkEventOnGrabDropObject (true);

			raycastCheckTransfrom = currentMeleeWeapon.raycastCheckTransfrom;

			mainMeleeWeaponsGrabbedManager.checkWeaponToStore (currentMeleeWeapon.getWeaponName (), currentGrabbedObjectTransform.gameObject);
		

			if (currentGrabbedWeaponInfo.isEmptyWeaponToUseOnlyShield) {
				mainGrabObjects.setGrabObjectsInputPausedState (true);
			} else {
				mainGrabObjects.setGrabObjectsInputPausedState (false);
			}


			if (useThrowReturnMeleeWeaponSystemEnabled && currentMeleeWeapon.isObjectThrown ()) {
				if (showDebugPrint) {
					print ("return weapon thrown");
				}

				mainMeleeCombatThrowReturnWeaponSystem.setObjectThrownState (true);

				mainPlayerController.setPlayerMeleeWeaponThrownState (true);

				mainMeleeCombatThrowReturnWeaponSystem.setSurfacecNotFoundState (true);

				currentGrabbedObjectTransform.position = currentGrabPhysicalObjectSystem.getLastPositionBeforeGrabbed ();
				currentGrabbedObjectTransform.rotation = currentGrabPhysicalObjectSystem.getLastRotationBeforeGrabbed ();
			
				mainMeleeCombatThrowReturnWeaponSystem.inputThrowOrReturnObject ();
			} else {
				checkGrabbedMeleeWeaponLocalPositionRotationValues ();
			}

			currentGrabPhysicalObjectSystem.setLastParentAssigned (null);

			meleeAttackTypesAmount.Clear ();

			int meleeAttackTypesCount = meleeAttackTypes.Count;

			int attackInfoListCount = currentMeleeWeapon.attackInfoList.Count;

			for (int i = 0; i < meleeAttackTypesCount; i++) {

				int meleeAttackAmount = 0;

				for (int j = 0; j < attackInfoListCount; j++) {
					if (currentMeleeWeapon.attackInfoList [j].attackType.Equals (meleeAttackTypes [i])) {
						meleeAttackAmount++;
					}
				}

				meleeAttackTypesAmount.Add (meleeAttackAmount);
			}

			checkIfCarryingShieldActive ();

			if (currentMeleeWeapon.useAbilitiesListToEnableOnWeapon) {
				GKC_Utils.enableOrDisableAbilityGroupByName (playerControllerGameObject.transform, true, currentMeleeWeapon.abilitiesListToEnableOnWeapon);
			}

			if (disableGrabObjectsInputWhenCarryingMeleeWeapon) {
				mainGrabObjects.setGrabObjectsInputDisabledState (true);
			}

			if (isDualWieldWeapon) {
				mainDualWieldMeleeWeaponObjectSystem.enableDualWieldMeleeweapobObject (this, currentGrabbedWeaponInfo.useEventsOnDamageDetected);
			}

			deflectProjectilesEnabled = currentMeleeWeapon.deflectProjectilesEnabled;

			deflectProjectilesOnBlockEnabled = currentMeleeWeapon.deflectProjectilesOnBlockEnabled;

			if (deflectProjectilesEnabled || deflectProjectilesOnBlockEnabled) {
				currentMeleeWeapon.mainArmoreSurfaceSystem.gameObject.SetActive (true);
			}

			lastTimeDrawMeleeWeapon = Time.time;

			if (currentMeleeWeapon.useExtraActions) {
				if (showExtraActionIconTexture) {
					setCurrentExtraActionTexture (currentMeleeWeapon.getActionIconTexture ());
				}
			}

			currentAttackInfoTemplateIndex = 0;

			if (!objectThrown) {
				if (currentGrabbedWeaponInfo.useMeleeWeaponAttackInfoList && currentGrabbedWeaponInfo.setInitialWeaponAttackInfoList) {
					int newIndex = currentGrabbedWeaponInfo.initialWeaponAttackInfoListIndex;

					if (newIndex >= currentGrabbedWeaponInfo.meleeWeaponAttackInfoList.Count) {
						newIndex = 0;
					}

					setAttackInfoTemplateByIndex (newIndex);
				}
			}

			currentMeleeWeapon.setCurrentCharacter (playerControllerGameObject);

			currentMeleeWeapon.addObjectStatsToMainManager ();
		} else {
			removeGrabPhysicalObjectMeleeAttackSystem ();
		}

		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setNewGrabPhysicalObjectMeleeAttackSystem (currentMeleeWeapon);
		}
	}

	public float getLastTimeDrawMeleeWeapon ()
	{
		return lastTimeDrawMeleeWeapon;
	}

	public List<int> getMeleeAttackTypesAmount ()
	{
		return meleeAttackTypesAmount;
	}

	bool removeWeaponsFromManager = true;

	public void setRemoveWeaponsFromManagerState (bool state)
	{
		removeWeaponsFromManager = state;
	}

	public void removeGrabPhysicalObjectMeleeAttackSystem ()
	{
		if (!carryingObject) {
			return;
		}

		mainPlayerController.setPlayerUsingMeleeWeaponsState (false);

		if (currentMeleeWeapon != null) {
			if (removeWeaponsFromManager) {
				mainMeleeWeaponsGrabbedManager.checkToDropWeaponFromList (currentMeleeWeapon.getWeaponName ());
			}

			if (objectThrown) {
				currentMeleeWeapon.checkDisableTrail ();

				if (removeWeaponsFromManager) {
					currentMeleeWeapon.setObjectThrownState (false);
				}
			}
		}

		stopActivateGrabbedObjectMeleeAttackCoroutine ();

		stopActivateDamageTriggerCoroutine ();

		resumeState ();

		if (currentHitCombat != null) {
			currentHitCombat.setMainColliderEnabledState (false);
		}

		drawOrSheatheShield (false);

		shieldActive = false;

		if (blockActive) {
			blockActivePreviously = false;

			disableBlockState ();
		}

		if (cuttingModeActive) {
			enableOrDisableCuttingMode (false);
		}

		if (meleeAttackInputPaused) {
			stopDisableMeleeAttackInputPausedStateWithDurationCoroutine ();

			mainGrabObjects.setGrabObjectsInputPausedState (false);

			meleeAttackInputPaused = false;
		}

		previousStrafeMode = false;

		if (currentMeleeWeapon != null) {
			previousStrafeMode = currentMeleeWeapon.wasStrafeModeActivePreviously ();

			currentMeleeWeapon.setPreviousStrafeModeState (false);

			if (currentMeleeWeapon.useAbilitiesListToDisableOnWeapon) {
				GKC_Utils.enableOrDisableAbilityGroupByName (playerControllerGameObject.transform, false, currentMeleeWeapon.abilitiesListToDisableOnWeapon);
			}

			if (deflectProjectilesEnabled || deflectProjectilesOnBlockEnabled) {
				currentMeleeWeapon.mainArmoreSurfaceSystem.gameObject.SetActive (false);

				checkSetArmorActiveState (false);

				deflectProjectilesEnabled = false;

				deflectProjectilesOnBlockEnabled = false;
			}

			if (currentMeleeWeapon.useExtraActions) {
				currentMeleeWeapon.setNewExtraActionByIndex (0, mainRemoteEventSystem);

				disableExtraActionIconGameObject ();
			}

			currentMeleeWeapon.setCurrentCharacter (null);

			currentMeleeWeapon.setLastTimeWeaponDropped ();
		}

		currentMeleeWeapon = null;

		currentGrabPhysicalObjectSystem = null;

		grabbedObjectMeleeAttackActive = false;

		currentHitCombat = null;

		objectRotationPoint = null;

		carryingObject = false;

		blockActive = false;

		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.stopThrowStateOnRemoveMeleeWeapon ();
		}

		cuttingModeActive = false;

		checkGrabbedWeaponInfoStateAtEnd ();

		checkEventOnGrabDropObject (false);

		updateShieldStateOnAnimator ();

		if (disableGrabObjectsInputWhenCarryingMeleeWeapon) {
			mainGrabObjects.setGrabObjectsInputDisabledState (false);
		}

		currentAttackInfoTemplateIndex = 0;

		if (mainDualWieldMeleeWeaponObjectSystem != null) {
			mainDualWieldMeleeWeaponObjectSystem.disableDualWieldMeleeweapobObject ();

			dualWieldHitCombat = null;

			stopActivateDamageDualWieldTriggerCoroutine ();
		}

		mainDualWieldMeleeWeaponObjectSystem = null;

		lastTimeDrawMeleeWeapon = -1;

		setThrowWeaponActionPausedState (false);

		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.removeGrabPhysicalObjectMeleeAttackSystem ();
		}
	}

	public void drawOrKeepMeleeWeapon ()
	{
		if (mainMeleeWeaponsGrabbedManager != null) {
			if (mainMeleeWeaponsGrabbedManager.meleeWeaponsGrabbedManagerActive) {
				mainMeleeWeaponsGrabbedManager.inputDrawOrKeepMeleeWeapon ();
			}
		}
	}

	public void drawOrKeepMeleeWeaponWithoutCheckingInputActive ()
	{
		if (mainMeleeWeaponsGrabbedManager != null) {
			if (mainMeleeWeaponsGrabbedManager.meleeWeaponsGrabbedManagerActive) {
				mainMeleeWeaponsGrabbedManager.drawOrKeepMeleeWeaponWithoutCheckingInputActive ();
			}
		}
	}

	public void drawMeleeWeaponGrabbedCheckingAnimationDelay ()
	{
		if (mainMeleeWeaponsGrabbedManager != null) {
			if (mainMeleeWeaponsGrabbedManager.meleeWeaponsGrabbedManagerActive) {
				mainMeleeWeaponsGrabbedManager.drawMeleeWeaponGrabbedCheckingAnimationDelay ();
			}
		}
	}

	public void checkToKeepWeapon ()
	{
		if (mainMeleeWeaponsGrabbedManager != null) {
			mainMeleeWeaponsGrabbedManager.checkToKeepWeapon ();
		}
	}

	public void checkToKeepWeaponWithoutCheckingInputActive ()
	{
		if (mainMeleeWeaponsGrabbedManager != null) {
			mainMeleeWeaponsGrabbedManager.checkToKeepWeaponWithoutCheckingInputActive ();
		}
	}

	public void setAttackInputPausedForStaminaState (bool state)
	{
		attackInputPausedForStamina = state;
	}

	public void setThrowObjectInputPausedForStaminaState (bool state)
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setThrowObjectInputPausedForStaminaState (state);
		}
	}

	public bool canActivateAttack ()
	{
		if (currentWaitTimeToNextAttack == 0) {
			return true;
		} else {
			if (Time.time > lastTimeAttackActive + currentWaitTimeToNextAttack) {
				return true;
			} else {
				return false;
			}
		}
	}

	public void activateGrabbedObjectMeleeAttack (string attackType)
	{
		if (showDebugPrint) {
			print ("input activated");
		}

		if (!grabbedObjectMeleeAttackActive) {
			return;
		}

		if (meleeAttackInputPaused) {
			return;
		}

		if (!canUseWeaponsInput ()) {
			return;
		}

		if (showDebugPrint) {
			print ("1");
		}

		if (objectThrown) {
			return;
		}

		if (cuttingModeActive) {
			return;
		}

		attackActivatedOnAir = false;

		bool canActivateAttackOnAir = false;

		if (!mainPlayerController.isPlayerOnGround ()) {
			if (playerOnGroundToActivateAttack) {
				return;
			} else {
				canActivateAttackOnAir = true;

				attackActivatedOnAir = true;
			}
		}

		if (showDebugPrint) {
			print ("2");
		}

		if (attackInputPausedForStamina && generalStaminaUseMultiplier > 0) {
			if (showDebugPrint) {
				print ("Not enough stamina, cancelling attack");
			}

			return;
		}

		if (!currentMeleeWeapon.attacksEnabled) {
			if (showDebugPrint) {
				print ("Attacks not enabled, cancelling attack");
			}

			return;
		}

		if (currentMeleeWeapon.keepGrabbedObjectState) {
			if (showDebugPrint) {
				print ("Melee weapon is in keep state, cancelling attack");
			}

			return;
		}

		if (currentMeleeWeapon.onlyAttackIfNoPreviousAttackInProcess) {
			if (attackInProcess) {
				if (showDebugPrint) {
					print ("attack already in process, cancelling attack");
				}

				return;
			}
		} 

		if (Time.time < lastTimeAttackActive + currentMeleeWeapon.minDelayBetweenAttacks) {
			if (showDebugPrint) {
				print ("Min delay between attacks in process, cancelling attack");
			}

			return;
		}

		if (!currentMeleeWeapon.onlyAttackIfNoPreviousAttackInProcess) {
			//currentAttackIndex > 0 && 

			if (currentAttackInfo != null) {
//				print (currentAttackInfo.minDelayBeforeNextAttack);

				if (Time.time < lastTimeAttackActive + currentAttackInfo.minDelayBeforeNextAttack) {
					if (showDebugPrint) {
						print ("Min wait before next attack in process, cancelling attack");
					}

					currentWaitTimeToNextAttack = 0;

					return;
				} else {
					currentWaitTimeToNextAttack = currentAttackInfo.minDelayBeforeNextAttack;
				}
			}
		}

		if (currentMeleeWeapon.resetIndexIfNotAttackAfterDelay && !attackInProcess) {
			if (Time.time > lastTimeAttackComplete + currentMeleeWeapon.delayToResetIndexAfterAttack) {
//				print ("reset attack index");

				currentAttackIndex = 0;

				if (showDebugPrint) {
					print ("Reseting attack index as min time to reset has been reached");
				}

				currentWaitTimeToNextAttack = 0;
			}
		}

		if (showDebugPrint) {
			print ("3");
		}

		if (useAttackTypes) {
			if (showDebugPrint) {
				print ("attack all conditions checked");
			}

			int numberOfAttacksSameType = 0;

			int numberOfAttacksAvailable = currentMeleeWeapon.getAttackListCount ();

			for (int i = 0; i < numberOfAttacksAvailable; i++) {
				if (currentMeleeWeapon.attackInfoList [i].attackType.Equals (attackType)) {
					numberOfAttacksSameType++;
				}
			}

			if (numberOfAttacksSameType == 1) {
				bool cancelAttack = false;

				if (attackInProcess) {
					cancelAttack = true;
				}

				if (Time.time < lastTimeAttackComplete + 0.4f) {
					cancelAttack = true;
				}

				if (cancelAttack) {
					if (showDebugPrint) {
						print ("just one attack type available and it is in process, avoiding to play it again");
					}

					return;
				}
			}
				
			bool attackFound = false;

			while (!attackFound) {
				currentAttackInfo = currentMeleeWeapon.attackInfoList [currentAttackIndex];

//				print (currentAttackInfo.attackType + " " + attackType);

				if (currentAttackInfo.attackType.Equals (attackType)) {
					attackFound = true;

					setNextAttackIndex ();
				} else {
					setNextAttackIndex ();

					numberOfAttacksAvailable--;

					if (numberOfAttacksAvailable < 0) {
						return;
					}
				}
			}
		} else {
			currentAttackInfo = currentMeleeWeapon.attackInfoList [currentAttackIndex];
		}

		if (canActivateAttackOnAir && currentAttackInfo.playerOnGroundToActivateAttack) {
			currentAttackIndex--;

			if (currentAttackIndex < 0) {
				currentAttackIndex = 0;
			}

			if (showDebugPrint) {
				print ("cancel attack on air");
			}

			return;
		}

		if (useStaminaOnAttackEnabled) {
			if (currentMeleeWeapon.objectUsesStaminaOnAttacks) {
				mainStaminaSystem.activeStaminaStateWithCustomAmount (attackStaminaStateName, currentAttackInfo.staminaUsedOnAttack * generalStaminaUseMultiplier, currentAttackInfo.customRefillStaminaDelayAfterUse);	
			}
		}

		currentHitCombat.setNewHitDamage (currentAttackInfo.attackDamage * generalAttackDamageMultiplier);

		if (isDualWieldWeapon) {
			dualWieldHitCombat.setNewHitDamage (currentAttackInfo.attackDamage * generalAttackDamageMultiplier);
		}

		int attackID = currentAttackIndex - 1;

		if (attackID < 0) {
			attackID = currentMeleeWeapon.getAttackListCount () - 1;
		}

		currentHitCombat.setTriggerId (attackID);

		if (isDualWieldWeapon) {
			dualWieldHitCombat.setTriggerId (attackID);
		}

		currentAttackCanBeBlocked = true;

		if (currentGrabbedWeaponInfo.attacksCantBeBlocked) {
			bool attackCanBeBlocked = true;

			if (currentGrabbedWeaponInfo.attackIDCantBeBlockedList.Count > 0) {
				attackCanBeBlocked = !currentGrabbedWeaponInfo.attackIDCantBeBlockedList.Contains (attackID);
			}

			currentHitCombat.setCustomDamageCanBeBlockedState (attackCanBeBlocked);

			if (isDualWieldWeapon) {
				dualWieldHitCombat.setCustomDamageCanBeBlockedState (attackCanBeBlocked);
			}

			currentAttackCanBeBlocked = attackCanBeBlocked;
		}

		if (!currentAttackCanBeBlocked) {
			if (useEventsOnAttackCantBeBlocked) {
				eventOnAttackCantBeBlocked.Invoke ();
			}
		}

//		print ("attack can be blocked " + currentAttackCanBeBlocked + "  " + currentAttackInfo.customActionName + " " + attackID);

		stopActivateGrabbedObjectMeleeAttackCoroutine ();

		attackCoroutine = StartCoroutine (activateGrabbedObjectMeleeAttackCoroutine ());

		stopActivateDamageTriggerCoroutine ();

		damageTriggerCoroutine = StartCoroutine (activateDamageTriggerCoroutine (true));

		if (isDualWieldWeapon) {
			stopActivateDamageDualWieldTriggerCoroutine ();

			dualWieldDamageTriggerCoroutine = StartCoroutine (activateDamageTriggerCoroutine (false));
		}

		if (!useAttackTypes) {
			setNextAttackIndex ();
		}
	}

	public void setNextAttackIndex ()
	{
		if (currentMeleeWeapon.useRandomAttackIndex) {
			currentAttackIndex = Random.Range (0, attackListCount - 1);
		} else {
			currentAttackIndex++;

			if (currentAttackIndex >= attackListCount) {
				currentAttackIndex = 0;
			}
		}
	}

	public void stopActivateGrabbedObjectMeleeAttackCoroutine ()
	{
		if (attackCoroutine != null) {
			StopCoroutine (attackCoroutine);
		}
	}

	IEnumerator activateGrabbedObjectMeleeAttackCoroutine ()
	{
		if (blockActive) {
			blockActivePreviously = true;

			disableBlockState ();
		}

		if (canCancelBlockToStartAttackActive) {
			if (showDebugPrint) {
				print ("cancel block");
			}

			mainAnimator.SetBool (cancelBlockReactionStateName, true);

			yield return new WaitForSeconds (0.3f);

			mainAnimator.SetBool (cancelBlockReactionStateName, false);

//			checkDisableHasExitTimeAnimator ();

			canCancelBlockToStartAttackActive = false;
		}

		if (useMatchTargetSystemOnAttack && !attackActivatedOnAir) {

			bool useMatchTargetAttack = false;

			float matchPositionOffset = 0;

			if (currentAttackInfo.useMatchPositionSystem || ignoreAttackSettingToMatchTarget) {
				useMatchTargetAttack = true;
				matchPositionOffset = currentAttackInfo.matchPositionOffset;
			}

			if (currentMeleeWeapon.useMatchPositionSystemOnAllAttacks || ignoreAttackSettingToMatchTarget) {
				useMatchTargetAttack = true;

				if (currentAttackInfo.useMatchPositionSystem) {
					matchPositionOffset = currentAttackInfo.matchPositionOffset;
				} else {
					matchPositionOffset = currentMeleeWeapon.matchPositionOffsetOnAllAttacks;
				}
			}

			if (useMatchTargetAttack) {
				mainMatchPlayerToTargetSystem.activateMatchPosition (matchPositionOffset);
			}
		}

		lastTimeSurfaceAudioPlayed = 0;

		lastSurfaceDetecetedIndex = -1;

		checkEventsOnAttack (true);

		lastTimeAttackActive = Time.time;

		attackInProcess = true;

		mainGrabObjects.setGrabObjectsInputPausedState (true);

		if (currentAttackInfo.useRemoteEvent) {
			for (int i = 0; i < currentAttackInfo.remoteEventNameList.Count; i++) {
				mainRemoteEventSystem.callRemoteEvent (currentAttackInfo.remoteEventNameList [i]);
			}
		}

		if (currentAttackInfo.useCustomAction) {
			mainPlayerController.activateCustomAction (currentAttackInfo.customActionName);

			if (showDebugPrint) {
				print ("attack activated :" + mainPlayerController.getCurrentActionName ());
			}
		}

		float attackWaitDuration = 0;
	
		if (attackActivatedOnAir) {
			attackWaitDuration = currentAttackInfo.attackDurationOnAir / currentAttackInfo.attackAnimationSpeedOnAir;
		} else {
			attackWaitDuration = currentAttackInfo.attackDuration / currentAttackInfo.attackAnimationSpeed;
		}

		yield return new WaitForSeconds (attackWaitDuration);

		if (damageTriggerInProcess) {
			if (showDebugPrint) {
				print ("damage in process, waiting to finish");
			}

			while (damageTriggerInProcess) {

				yield return null;
			}
		}

		yield return null;

		lastTimeAttackComplete = Time.time;

		resumeState ();

		if (blockActivePreviously) {
			if (!blockInputPaused) {
				blockActivePreviously = false;

				yield return new WaitForSeconds (0.3f);

				setBlockActiveState (true);
			}
		}

		if (showDebugPrint) {
			print ("end of attack");
		}
	}

	public void stopActivateDamageTriggerCoroutine ()
	{
		if (damageTriggerCoroutine != null) {
			StopCoroutine (damageTriggerCoroutine);
		}
	}

	public void stopActivateDamageDualWieldTriggerCoroutine ()
	{
		if (dualWieldDamageTriggerCoroutine != null) {
			StopCoroutine (dualWieldDamageTriggerCoroutine);
		}
	}

	public void enableOrDisableDualWieldMeleeWeaponObject (bool state)
	{
		if (isDualWieldWeapon) {
			if (mainDualWieldMeleeWeaponObjectSystem != null) {
				mainDualWieldMeleeWeaponObjectSystem.enableOrDisableDualWieldMeleeWeaponObject (state);
			}
		}
	}

	public void resetActivateDamageTriggerCoroutine ()
	{
		if (showDebugPrint) {
			print ("Resetinng Activate Damage Trigger Coroutine on Melee Weapons");
		}

		if (damageTriggerInProcess) {
//			print ("checking");
			if (Time.time > lastTimeDamageTriggerActivated + 0.05f) {
				stopActivateDamageTriggerCoroutine ();

				damageTriggerCoroutine = StartCoroutine (activateDamageTriggerCoroutine (true));

				if (isDualWieldWeapon) {
					stopActivateDamageDualWieldTriggerCoroutine ();

					dualWieldDamageTriggerCoroutine = StartCoroutine (activateDamageTriggerCoroutine (false));
				}

//				print ("reset attack for delay of " + (Mathf.Abs (Time.time - (lastTimeDamageTriggerActivated + 0.05f))));
			}
		}
	}

	IEnumerator activateDamageTriggerCoroutine (bool enableMainHitCombat)
	{
		if (showDebugPrint) {
			print ("activate attack");

			if (attackActivatedOnAir) {
				print ("Attack on air ");
			} else {
				print ("Attack on ground");
			}
		}

		damageTriggerInProcess = true;

		lastTimeDamageTriggerActivated = Time.time;

		hitCombat attackHitCombat = currentHitCombat;

		if (!enableMainHitCombat) {
			attackHitCombat = dualWieldHitCombat;
		}

		attackHitCombat.setCurrentState (false);

		checkSetArmorActiveState (false);

		attackHitCombat.setIgnoreDetectedObjectsOnListState (false);

		if (currentMeleeWeapon.useGeneralDamageTypeID) {
			attackHitCombat.setNewDamageTypeID (currentMeleeWeapon.generalDamageTypeID);

		} else {
			attackHitCombat.setNewDamageTypeID (currentAttackInfo.damageTypeID);
		}

		int numberOfEventsTriggered = 0;

		float timer = Time.time;

		bool allEventsTriggered = false;

		bool useAnimationPercentageDuration = currentMeleeWeapon.useAnimationPercentageDuration;
		bool useAnimationPercentageOver100 = currentMeleeWeapon.useAnimationPercentageOver100;

		int damageTriggerActiveInfoListCount = 0;

		if (enableMainHitCombat) {
			if (attackActivatedOnAir) {
				damageTriggerActiveInfoListCount = currentAttackInfo.attackOnAirDamageTriggerActiveInfoList.Count;
			} else {
				damageTriggerActiveInfoListCount = currentAttackInfo.damageTriggerActiveInfoList.Count;
			}
		} else {
			damageTriggerActiveInfoListCount = currentAttackInfo.dualWieldDamageTriggerActiveInfoList.Count;
		}

		bool canActivateCurrentEvent = false;

		float currentAttackDuration = 0;

		if (attackActivatedOnAir) {
			currentAttackDuration = currentAttackInfo.attackDurationOnAir / currentAttackInfo.attackAnimationSpeedOnAir;
		} else {
			currentAttackDuration = currentAttackInfo.attackDuration / currentAttackInfo.attackAnimationSpeed;
		}

		if (currentAttackInfo.useSingleSlashDamageInfo) {
			if (hitCombatParentChanged) {
				attackHitCombat.transform.SetParent (objectRotationPoint);
				attackHitCombat.transform.localPosition = originalHitCombatColliderPosition;
				attackHitCombat.transform.localRotation = Quaternion.identity;

				Transform raycastCheckTransformParent = currentMeleeWeapon.raycastCheckTransformParent;
				raycastCheckTransformParent.SetParent (objectRotationPoint);
				raycastCheckTransformParent.localPosition = Vector3.zero;
				raycastCheckTransformParent.localRotation = Quaternion.identity;

				hitCombatParentChanged = false;
			}

			if (enableMainHitCombat) {
				setHitCombatScale (originalHitCombatColliderSize);

				setHitCombatOffset (originalHitCombatColliderCenter);
			} else {
				mainDualWieldMeleeWeaponObjectSystem.setOriginalHitCombatScale ();

				mainDualWieldMeleeWeaponObjectSystem.setOriginalHitCombatOffset ();
			}

			float delayToActivateSingleSlashDamageTrigger = currentAttackInfo.delayToActivateSingleSlashDamageTrigger;
			float delayToDeactivateSingleSlashDamageTrigger = currentAttackInfo.delayToDeactivateSingleSlashDamageTrigger;

			if (useAnimationPercentageDuration) {

				if (useAnimationPercentageOver100) {
					delayToActivateSingleSlashDamageTrigger /= 100;
					delayToDeactivateSingleSlashDamageTrigger /= 100;
				}

				delayToActivateSingleSlashDamageTrigger = currentAttackDuration * delayToActivateSingleSlashDamageTrigger;
				delayToDeactivateSingleSlashDamageTrigger = currentAttackDuration * delayToDeactivateSingleSlashDamageTrigger;
			}

			bool activateDamageTriggered = false;
			bool deactivateDamageTriggered = false;

			bool currentActivateDamageTriggerValue = false;

			float currentDelay = 0;

			while (!allEventsTriggered) {
				bool animatorIsPaused = mainAnimator.speed <= 0;

				if (animatorIsPaused) {
					timer += Time.deltaTime;
				}

				if (!activateDamageTriggered) {
					currentActivateDamageTriggerValue = true;
				} else {
					currentActivateDamageTriggerValue = false;
				}

				if (!activateDamageTriggered || !deactivateDamageTriggered) {

					canActivateCurrentEvent = false;

					if (useAnimationPercentageDuration) {
						if (!activateDamageTriggered) {
							currentDelay = delayToActivateSingleSlashDamageTrigger;
						} else {
							currentDelay = delayToDeactivateSingleSlashDamageTrigger;
						}
					} else {
						if (!activateDamageTriggered) {
							currentDelay = currentAttackInfo.delayToActivateSingleSlashDamageTrigger;
						} else {
							currentDelay = currentAttackInfo.delayToDeactivateSingleSlashDamageTrigger;
						}
					}

					if (Time.time > timer + currentDelay) {
						canActivateCurrentEvent = true;
					}
						
					if (canActivateCurrentEvent) {

						bool activateDamageTrigger = currentActivateDamageTriggerValue;

						attackHitCombat.setCurrentState (activateDamageTrigger);

						checkSetArmorActiveState (activateDamageTrigger);

						if (activateDamageTrigger && currentAttackInfo.ignoreDetectedObjectsOnList) {
							attackHitCombat.setIgnoreDetectedObjectsOnListState (true);
						}

						attackHitCombat.setNewDamageReactionID (currentAttackInfo.damageReactionID);

						numberOfEventsTriggered++;

						if (!activateDamageTriggered) {
							activateDamageTriggered = true;

//							print ("activate first delay");
						} else {
							deactivateDamageTriggered = true;

//							print ("activate second delay");
						}

						if ((activateDamageTriggered && deactivateDamageTriggered) || numberOfEventsTriggered == 2) {
							allEventsTriggered = true;
						}
								
						if (activateDamageTrigger) {
							checkingNoDamageDetectionOnDualWieldTriggerActive = !enableMainHitCombat;

							checkSurfaceFoundOnAttack (false);
						}
					}
				}

				yield return null;
			}

//			print ("end of attack");
		} else {

			grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo currentdamageTriggerActiveInfo = null;

			for (int i = 0; i < damageTriggerActiveInfoListCount; i++) {
				if (enableMainHitCombat) {
					if (attackActivatedOnAir) {
						currentdamageTriggerActiveInfo = currentAttackInfo.attackOnAirDamageTriggerActiveInfoList [i];
					} else {
						currentdamageTriggerActiveInfo = currentAttackInfo.damageTriggerActiveInfoList [i];
					}
				} else {
					currentdamageTriggerActiveInfo = currentAttackInfo.dualWieldDamageTriggerActiveInfoList [i];
				}

				currentdamageTriggerActiveInfo.delayTriggered = false;

				if (useAnimationPercentageDuration) {
					float currentDelay = currentdamageTriggerActiveInfo.delayToActiveTrigger;

					if (currentDelay > 1) {
						print ("ERRRORORORROOROROR: DELAY IS HIGHER THAN 1 FIXXXXXXXXXXX----------------------------------------" +
						".............................................");
					}

					if (useAnimationPercentageOver100) {
						currentDelay /= 100;
					}

					currentdamageTriggerActiveInfo.calculatedPercentageAttackDuration = currentAttackDuration * currentDelay;
				}
			}

			detectedObjectsOnReturn.Clear ();

			while (!allEventsTriggered) {
				if (currentAttackInfo == null) {
					allEventsTriggered = true;

				} else {
					bool animatorIsPaused = mainAnimator.speed <= 0;

					if (animatorIsPaused) {
						timer += Time.deltaTime;
					}

					for (int i = 0; i < damageTriggerActiveInfoListCount; i++) {
						if (enableMainHitCombat) {
							if (attackActivatedOnAir) {
								currentdamageTriggerActiveInfo = currentAttackInfo.attackOnAirDamageTriggerActiveInfoList [i];
							} else {
								currentdamageTriggerActiveInfo = currentAttackInfo.damageTriggerActiveInfoList [i];
							}
						} else {
							currentdamageTriggerActiveInfo = currentAttackInfo.dualWieldDamageTriggerActiveInfoList [i];
						}

						if (!currentdamageTriggerActiveInfo.delayTriggered) {

							canActivateCurrentEvent = false;

							if (useAnimationPercentageDuration) {
								if (Time.time > timer + currentdamageTriggerActiveInfo.calculatedPercentageAttackDuration) {
									canActivateCurrentEvent = true;
								}
							} else {
								if (Time.time > timer + currentdamageTriggerActiveInfo.delayToActiveTrigger) {
									canActivateCurrentEvent = true;
								}
							}

							//print (currentdamageTriggerActiveInfo.delayToActiveTrigger + " " + i);

							if (canActivateCurrentEvent) {

								bool activateDamageTrigger = currentdamageTriggerActiveInfo.activateDamageTrigger;

								attackHitCombat.setCurrentState (activateDamageTrigger);

								checkSetArmorActiveState (activateDamageTrigger);

								if (activateDamageTrigger && currentAttackInfo.ignoreDetectedObjectsOnList) {
									attackHitCombat.setIgnoreDetectedObjectsOnListState (true);
								}

								attackHitCombat.setNewDamageReactionID (currentAttackInfo.damageReactionID);

								numberOfEventsTriggered++;

								currentdamageTriggerActiveInfo.delayTriggered = true;

								if (damageTriggerActiveInfoListCount == numberOfEventsTriggered) {
									allEventsTriggered = true;
								}

								if (currentdamageTriggerActiveInfo.setNewTriggerScale) {
									if (currentdamageTriggerActiveInfo.setOriginalScale) {
										if (enableMainHitCombat) {
											setHitCombatScale (originalHitCombatColliderSize);

											setHitCombatOffset (originalHitCombatColliderCenter);
										} else {
											mainDualWieldMeleeWeaponObjectSystem.setOriginalHitCombatScale ();

											mainDualWieldMeleeWeaponObjectSystem.setOriginalHitCombatOffset ();
										}
									} else {
										if (enableMainHitCombat) {
											setHitCombatScale (currentdamageTriggerActiveInfo.newTriggerScale);
										} else {
											mainDualWieldMeleeWeaponObjectSystem.setHitCombatScale (currentdamageTriggerActiveInfo.newTriggerScale);
										}

										if (currentdamageTriggerActiveInfo.newTriggerOffset != Vector3.zero) {
											if (enableMainHitCombat) {
												setHitCombatOffset (currentdamageTriggerActiveInfo.newTriggerOffset);
											} else {
												mainDualWieldMeleeWeaponObjectSystem.setHitCombatOffset (currentdamageTriggerActiveInfo.newTriggerOffset);
											}
										}
									}
								}

								if (currentdamageTriggerActiveInfo.changeDamageTriggerToLimb) {
									Transform newParent = null;

									if (currentdamageTriggerActiveInfo.placeTriggerInFrontOfCharacter) {
										newParent = parentToPlaceTriggerInFrontOfCharacter;

										if (newParent == null) {
											newParent = playerControllerGameObject.transform;
										}
									} else {
										newParent = getCharacterHumanBone (currentdamageTriggerActiveInfo.limbToPlaceTrigger);
									}

									if (newParent != null) {
										attackHitCombat.transform.SetParent (newParent);
										attackHitCombat.transform.localPosition = Vector3.zero;
										attackHitCombat.transform.localRotation = Quaternion.identity;

										Transform raycastCheckTransformParent = currentMeleeWeapon.raycastCheckTransformParent;
										raycastCheckTransformParent.SetParent (attackHitCombat.transform);
										raycastCheckTransformParent.localPosition = Vector3.zero;
										raycastCheckTransformParent.localRotation = Quaternion.identity;

										hitCombatParentChanged = true;
									}
								} else {
									if (hitCombatParentChanged) {
										attackHitCombat.transform.SetParent (objectRotationPoint);
										attackHitCombat.transform.localPosition = originalHitCombatColliderPosition;
										attackHitCombat.transform.localRotation = Quaternion.identity;

										Transform raycastCheckTransformParent = currentMeleeWeapon.raycastCheckTransformParent;
										raycastCheckTransformParent.SetParent (objectRotationPoint);
										raycastCheckTransformParent.localPosition = Vector3.zero;
										raycastCheckTransformParent.localRotation = Quaternion.identity;

										hitCombatParentChanged = false;
									}
								}

								if (currentdamageTriggerActiveInfo.useEventOnAttack) {
									currentdamageTriggerActiveInfo.eventOnAtatck.Invoke ();
								}

								if (currentdamageTriggerActiveInfo.useRangeAttackID && currentGrabbedWeaponInfo.useRangeAttackID) {

									int rangeAttackID = currentdamageTriggerActiveInfo.rangeAttackID;

									for (int k = 0; k < currentGrabbedWeaponInfo.rangeAttackInfoList.Count; k++) {
										if (currentGrabbedWeaponInfo.rangeAttackInfoList [k].rangeAttackID == rangeAttackID) {
											currentGrabbedWeaponInfo.rangeAttackInfoList [k].eventOnRangeAttack.Invoke ();
										}
									}
								}

								if (currentdamageTriggerActiveInfo.disableRangeAttackID && currentGrabbedWeaponInfo.useRangeAttackID) {

									int rangeAttackID = currentdamageTriggerActiveInfo.rangeAttackIDToDisable;

									for (int k = 0; k < currentGrabbedWeaponInfo.rangeAttackInfoList.Count; k++) {
										if (currentGrabbedWeaponInfo.rangeAttackInfoList [k].rangeAttackID == rangeAttackID) {
											currentGrabbedWeaponInfo.rangeAttackInfoList [k].eventOnDisableRangeAttack.Invoke ();
										}
									}
								}

								if (activateDamageTrigger) {
									checkingNoDamageDetectionOnDualWieldTriggerActive = !enableMainHitCombat;

									checkSurfaceFoundOnAttack (false);
								}

								detectedObjectsOnReturn.Clear ();
							} else {
								if (checkSurfacesWithCapsuleRaycastEnabled) {
									if (currentdamageTriggerActiveInfo.checkSurfacesWithCapsuleRaycast &&
									    currentdamageTriggerActiveInfo.activateDamageTrigger) {
										checkSurfacesDetectedRaycast (currentdamageTriggerActiveInfo.checkSurfaceCapsuleRaycastRadius, enableMainHitCombat);

										int hitsLength = hits.Length;

										if (hitsLength > 0) {
											for (int j = 0; j < hitsLength; j++) {
												GameObject currentObject = hits [j].collider.gameObject;

												if (!detectedObjectsOnReturn.Contains (currentObject)) {
													attackHitCombat.checkTriggerInfo (hits [j].collider, true);
										
													detectedObjectsOnReturn.Add (currentObject);
												}
											}
										}
									}
								}
							}
						}
					}
				}

				yield return null;
			}

			yield return null;
		}

		damageTriggerInProcess = false;
	}

	public Transform getCharacterHumanBone (HumanBodyBones boneToFind)
	{
		return mainPlayerController.getCharacterHumanBone (boneToFind);
	}

	public Transform getCurrentGrabbedObjectTransform ()
	{
		return currentGrabbedObjectTransform;
	}

	public string getCurrentMeleeWeaponTypeName ()
	{
		if (currentGrabbedWeaponInfo != null) {
			return currentGrabbedWeaponInfo.Name;
		}

		return "";
	}

	public string getCurrentMeleeWeaponName ()
	{
		if (currentGrabbedWeaponInfo != null && currentMeleeWeapon != null) {
			return currentMeleeWeapon.getWeaponName ();
		}

		return "";
	}

	public bool canWeaponsBeStolenFromCharacter ()
	{
		return weaponsCanBeStolenFromCharacter;
	}

	public void setCanWeaponsBeStolenFromCharacter (bool state)
	{
		weaponsCanBeStolenFromCharacter = state;
	}

	public bool isSecondaryAbilityActiveOnCurrentWeapon ()
	{
		if (currentGrabbedWeaponInfo != null && currentMeleeWeapon != null) {
			return currentMeleeWeapon.isSecondaryAbilityActive ();
		}

		return false;
	}

	public void disableCurrentAttackInProcess ()
	{
		if (attackInProcess) {
			stopActivateGrabbedObjectMeleeAttackCoroutine ();

			stopActivateDamageTriggerCoroutine ();

			currentHitCombat.setCurrentState (false);

			currentHitCombat.setIgnoreDetectedObjectsOnListState (false);


			if (isDualWieldWeapon) {
				stopActivateDamageDualWieldTriggerCoroutine ();

				dualWieldHitCombat.setCurrentState (false);

				dualWieldHitCombat.setIgnoreDetectedObjectsOnListState (false);
			}

			checkSetArmorActiveState (false);

			lastTimeAttackComplete = Time.time;

			resumeState ();
		
			blockActivePreviously = false;
		}

		if (useThrowReturnMeleeWeaponSystemEnabled) {
			if (objectThrown) {
				if (!mainMeleeCombatThrowReturnWeaponSystem.isContinueObjecThrowActivated () &&
				    !mainMeleeCombatThrowReturnWeaponSystem.isObjectThrownTravellingToTarget ()) {
					if (useThrowReturnMeleeWeaponSystemEnabled) {
						mainMeleeCombatThrowReturnWeaponSystem.cancelThrowObject ();
					}
				}
			}
		}
	}

	void checkSetArmorActiveState (bool state)
	{
		if (deflectProjectilesEnabled) {
			currentMeleeWeapon.mainArmoreSurfaceSystem.setArmorActiveState (state);

			if (state) {
				currentMeleeWeapon.setRegularDeflectProjectilesTriggerScaleValues ();
			}
		}
	}

	void checkSetArmorActiveOnBlockState (bool state)
	{
		if (deflectProjectilesOnBlockEnabled) {
			if (currentMeleeWeapon.useDurationToDeflectProjectilesOnBlock) {
				currentMeleeWeapon.mainArmoreSurfaceSystem.setEnableArmorSurfaceStateWithDuration (
					currentMeleeWeapon.durationToDeflecltProjectilesOnBlock, state, false);
			} else {
				currentMeleeWeapon.mainArmoreSurfaceSystem.setArmorActiveState (state);
			}

			if (state) {
				currentMeleeWeapon.setBlockDeflectProjectilesTriggerScaleValues ();
			}
		}
	}

	public void checkDurabilityOnAttackOnCurrentWeapon (bool ignoreDurability, float extraDurabilityMultiplier)
	{
		if (!checkDurabilityOnAttackEnabled) {
			return;
		}

		if (!ignoreDurability) {
			if (extraDurabilityMultiplier == 0) {
				extraDurabilityMultiplier = generalAttackDurabilityMultiplier;
			} else {
				extraDurabilityMultiplier *= generalAttackDurabilityMultiplier;
			}

			if (currentMeleeWeapon.checkDurabilityOnAttack (extraDurabilityMultiplier)) {
				if (useEventOnDurabilityEmptyOnMeleeWeapon) {
					eventOnDurabilityEmptyOnMeleeWeapon.Invoke ();
				}

				if (currentGrabbedWeaponInfo != null && currentGrabbedWeaponInfo.useEventOnEmptyDurability) {
					currentGrabbedWeaponInfo.eventOnEmptyDurability.Invoke ();
				}
			}
		}
	}

	public void repairObjectFully (string weaponName)
	{
		mainMeleeWeaponsGrabbedManager.repairObjectFully (weaponName);
	}

	//START OF BLOCK FUNCTIONS
	public void checkDurabilityOnBlock ()
	{
		if (carryingObject) {
			if (checkDurabilityOnBlockEnabled) {
				if (currentMeleeWeapon.checkDurabilityOnBlock (generalBlockDurabilityMultiplier)) {
					if (useEventOnDurabilityEmptyOnBlock) {
						eventOnDurabilityEmptyOnBlock.Invoke ();
					}
				}
			}
		}
	}

	public void checkOnAttackBlocked ()
	{
		if (carryingObject) {
			if (currentGrabbedWeaponInfo.getHealthFromBlocks) {
				float totalHealthAmount = currentGrabbedWeaponInfo.healthAmountFromBlocks;

				if (totalHealthAmount > 0 && !mainHealth.checkIfMaxHealth ()) {

					mainHealth.getHealth (totalHealthAmount);
				}
			}
		}
	}

	public void checkOnAttackBlockedPerfectly ()
	{
		if (carryingObject) {
			bool getHealthFromPerfectBlocksResult = currentGrabbedWeaponInfo.getHealthFromPerfectBlocks || getHealthFromPerfectBlocksActive;

			if (getHealthFromPerfectBlocksResult) {
				float totalHealthAmount = 0;

				if (currentGrabbedWeaponInfo.getHealthFromPerfectBlocks) {
					totalHealthAmount = currentGrabbedWeaponInfo.healthAmountFromPerfectBlocks;
				}

				if (getHealthFromPerfectBlocksActive) {
					totalHealthAmount = healthAmountFromPerfectBlocksValue;
				}

				if (totalHealthAmount > 0 && !mainHealth.checkIfMaxHealthWithHealthManagement ()) {

					mainHealth.setHealWithHealthManagement (totalHealthAmount);
				}
			}
		}
	}

	public void setGetHealthFromPerfectBlocksActiveState (bool state)
	{
		getHealthFromPerfectBlocksActive = state;
	}

	public void setHealthAmountFromPerfectBlocksValue (float newValue)
	{
		healthAmountFromPerfectBlocksValue = newValue;
	}

	void checkDisableHasExitTimeAnimator ()
	{
		if (disableHasExitTimeCoroutine != null) {
			StopCoroutine (disableHasExitTimeCoroutine);
		}

		disableHasExitTimeCoroutine = StartCoroutine (checkDisablehasExitTimeAnimatorCoroutine ());
	}

	IEnumerator checkDisablehasExitTimeAnimatorCoroutine ()
	{
		mainAnimator.SetBool (cancelBlockReactionStateName, true);

		yield return new WaitForSeconds (0.2f);

		mainAnimator.SetBool (cancelBlockReactionStateName, false);
	}

	//CALLED ON DODGE/ROLL ACTION SYSTEM
	public void checkIfBlockInputIsCurrentlyInUse ()
	{
//		print ("check if block input is in use");
		if (playerInput.isKeyboardButtonPressed (mainMeleeCombatAxesInputName, mainMeleeCombatBlockInputName)) {
			if (showDebugPrint) {
				print ("block is being pressed");
			}
		} else {
			if (showDebugPrint) {
				print ("block is not being pressed, disabling block");
			}

			disableBlockStateInProcess ();
		}
	}

	public void disableBlockStateInProcess ()
	{
		if (blockActive) {
			blockActivePreviously = false;

			setBlockActiveState (false);
		}
	}

	public void disableBlockActiveState ()
	{
		inputDeactivateBlock ();
	}

	public void checkEventsOnBlockDamage (bool state)
	{
		if (useEventsOnBlockDamage) {
			if (state) {
				eventOnBlockActivate.Invoke ();
			} else {
				eventOnBlockDeactivate.Invoke ();	
			}
		}
	}

	IEnumerator disableMeleeAttackInputPausedStateWithDurationCoroutine (float pauseDuration)
	{
		meleeAttackInputPaused = true;

		mainGrabObjects.setGrabObjectsInputPausedState (true);

		yield return new WaitForSeconds (pauseDuration);

		mainGrabObjects.setGrabObjectsInputPausedState (false);

		meleeAttackInputPaused = false;
	}

	public void checkIfBlockActive ()
	{
		if (blockActive) {
			setBlockActiveState (true);
		}
	}

	public void setBlockActiveState (bool state)
	{
		if (state) {
			if (currentMeleeWeapon.canUseBlock) {
				if (shieldActive && currentGrabbedWeaponInfo.weaponCanUseShield) {
					if (mainPlayerController.isStrafeModeActive ()) {
						if (currentGrabbedWeaponInfo.strafeMovementBlockShieldID > 0) {
							mainPlayerController.setCurrentStrafeIDValue (currentGrabbedWeaponInfo.strafeMovementBlockShieldID);
						}
					} else {
						if (currentGrabbedWeaponInfo.regularMovementBlockShieldActionName != "") {
							mainPlayerController.activateCustomAction (currentGrabbedWeaponInfo.regularMovementBlockShieldActionName);
						}
					}
				} else {
					mainPlayerController.activateCustomAction (currentMeleeWeapon.blockActionName);

					mainPlayerController.setCurrentStrafeIDValue (currentGrabbedWeaponInfo.strafeIDUsed);
				}

				mainHealth.setBlockDamageActiveState (true);

				if (reducedBlockDamageProtectionActive) {
					mainHealth.setBlockDamageProtectionAmount (currentMeleeWeapon.reducedBlockDamageProtectionAmount * generalBlockProtectionMultiplier);
				} else {
					mainHealth.setBlockDamageProtectionAmount (currentMeleeWeapon.blockDamageProtectionAmount * generalBlockProtectionMultiplier);
				}

				mainHealth.setBlockDamageRangleAngleState (currentMeleeWeapon.useMaxBlockRangeAngle,
					currentMeleeWeapon.maxBlockRangeAngle);

				if (shieldActive) {
					if (mainPlayerController.isStrafeModeActive ()) {
						mainHealth.setHitReactionBlockIDValue (currentGrabbedWeaponInfo.shieldIDStrafeMovement);
					} else {
						mainHealth.setHitReactionBlockIDValue (currentGrabbedWeaponInfo.shieldIDFreeMovement);
					}
				} else {
					mainHealth.setHitReactionBlockIDValue (currentMeleeWeapon.blockID);
				}

				blockActive = true;

				checkEventsOnBlockDamage (true);

				checkSetArmorActiveOnBlockState (true);
			}
		} else {
			if (currentMeleeWeapon.canUseBlock) {
				disableBlockState ();
			}
		}

		updateShieldStateOnAnimator ();
	}

	public void updateRegularBlockDamageProtectionValue ()
	{
		setBlockDamageProtectionValue (false);
	}

	public void updateReducedBlockDamageProtectionValue ()
	{
		setBlockDamageProtectionValue (true);
	}

	public void setBlockDamageProtectionValue (bool state)
	{
		reducedBlockDamageProtectionActive = state;

		if (blockActive) {
			if (!carryingObject) {
				return;
			}

			if (currentMeleeWeapon.canUseBlock) {
				if (reducedBlockDamageProtectionActive) {
					mainHealth.setBlockDamageProtectionAmount (currentMeleeWeapon.reducedBlockDamageProtectionAmount * generalBlockProtectionMultiplier);
				} else {
					mainHealth.setBlockDamageProtectionAmount (currentMeleeWeapon.blockDamageProtectionAmount * generalBlockProtectionMultiplier);
				}
			}
		}
	}

	void disableBlockState ()
	{
		if (shieldActive && currentGrabbedWeaponInfo.weaponCanUseShield) {
			if (mainPlayerController.isStrafeModeActive ()) {
				if (currentGrabbedWeaponInfo.strafeMovementBlockShieldID > 0) {
					mainPlayerController.setCurrentStrafeIDValue (currentGrabbedWeaponInfo.strafeIDUsed);
				}
			} else {
				if (currentGrabbedWeaponInfo.regularMovementBlockShieldActionName != "") {
					mainPlayerController.stopCustomAction (currentGrabbedWeaponInfo.regularMovementBlockShieldActionName);
				}
			}
		} else {
			mainPlayerController.stopCustomAction (currentMeleeWeapon.blockActionName);
		}

		mainHealth.setBlockDamageActiveState (false);

		blockActive = false;

		checkEventsOnBlockDamage (false);

		mainHealth.setHitReactionBlockIDValue (-1);

		checkSetArmorActiveOnBlockState (false);
	}

	void updateBlockShieldState ()
	{
//		print ("idhsidhs " + shieldActive + "  " + blockActive + "  " + currentGrabbedWeaponInfo.weaponCanUseShield);
		if (shieldActive && blockActive && currentGrabbedWeaponInfo.weaponCanUseShield) {
			mainPlayerController.updateStrafeModeActiveState ();

//			print (mainPlayerController.isLookAlwaysInCameraDirectionActive ());

			if (mainPlayerController.isLookAlwaysInCameraDirectionActive ()) {
				if (currentGrabbedWeaponInfo.regularMovementBlockShieldActionName != "") {
					mainPlayerController.stopCustomAction (currentGrabbedWeaponInfo.regularMovementBlockShieldActionName);
				}

				if (currentGrabbedWeaponInfo.strafeMovementBlockShieldID > 0) {
					mainPlayerController.setCurrentStrafeIDValue (currentGrabbedWeaponInfo.strafeMovementBlockShieldID);
				}

				mainHealth.setHitReactionBlockIDValue (currentGrabbedWeaponInfo.shieldIDStrafeMovement);
			} else {
				if (currentGrabbedWeaponInfo.regularMovementBlockShieldActionName != "") {
					mainPlayerController.activateCustomAction (currentGrabbedWeaponInfo.regularMovementBlockShieldActionName);
				}

				if (currentGrabbedWeaponInfo.strafeMovementBlockShieldID > 0) {
					mainPlayerController.setCurrentStrafeIDValue (currentGrabbedWeaponInfo.strafeIDUsed);
				}

				mainHealth.setHitReactionBlockIDValue (currentGrabbedWeaponInfo.shieldIDFreeMovement);
			}
		} 
	}

	public void disableBlockInputPausedStateWithDuration (float pauseDuration)
	{
		stopDisableBlockInputPausedStateWithDurationCoroutine ();

		pauseBlockInputCoroutine = StartCoroutine (disableBlockInputPausedStateWithDurationCoroutine (pauseDuration));
	}

	void stopDisableBlockInputPausedStateWithDurationCoroutine ()
	{
		if (pauseBlockInputCoroutine != null) {
			StopCoroutine (pauseBlockInputCoroutine);
		}
	}

	IEnumerator disableBlockInputPausedStateWithDurationCoroutine (float pauseDuration)
	{
		blockInputPaused = true;

		mainGrabObjects.setGrabObjectsInputPausedState (true);

		yield return new WaitForSeconds (pauseDuration);

		mainGrabObjects.setGrabObjectsInputPausedState (false);

		blockInputPaused = false;

		if (blockActivePreviously) {
			blockActivePreviously = false;

			yield return new WaitForSeconds (0.3f);

			setBlockActiveState (true);
		}
	}

	public void setBlockInputPausedState (bool state)
	{
		blockInputPaused = state;
	}

	public void setCanCancelBlockToStartAttackActiveState (bool state)
	{
		canCancelBlockToStartAttackActive = state;
	}
	//END OF BLOCK FUNCTIONS


	//START SHIELD FUNCTIONS
	public void setShieldActiveState (bool state)
	{
		shieldActive = state;

//		print ("shield state " + shieldActive + " " + carryingObject);

		if (!carryingObject) {
			if (mainMeleeWeaponsGrabbedManager.currentMeleeWeaponSheathedOrCarried) {
				shieldActive = false;
			} else {
				if (shieldCanBeUsedWithoutMeleeWeapon) {
					if (showDebugPrint) {
						print ("not carrying melee weapon, using shield without weapon");
					}

					bool emptyMeleeWeaponForShieldFound = false;

					for (int i = 0; i < grabbedWeaponInfoList.Count; i++) {
						if (grabbedWeaponInfoList [i].isEmptyWeaponToUseOnlyShield) {
							if (!grabbedObjectMeleeAttackActive) {
								eventToActivateMeleeModeWhenUsingShieldWithoutMeleeWeapon.Invoke ();
							}

							mainMeleeWeaponsGrabbedManager.checkMeleeWeaponToUse (grabbedWeaponInfoList [i].Name, false);

							emptyMeleeWeaponForShieldFound = true;
						}
					}

					updateShieldStateOnAnimator ();

					if (emptyMeleeWeaponForShieldFound) {
						return;
					}
				} else {
					shieldActive = false;
				}
			}
		}

		if (currentGrabbedWeaponInfo != null && !currentGrabbedWeaponInfo.weaponCanUseShield) {
			shieldActive = false;
		}

		if (currentShieldGameObject != null) {
			if (currentShieldGameObject.activeSelf != shieldActive) {
				currentShieldGameObject.SetActive (shieldActive);
			}

			if (shieldActive) {
				setShieldParentState (true);
			}
		}

		updateShieldStateOnAnimator ();
	}

	public string getEmptyWeaponToUseOnlyShield ()
	{
		for (int i = 0; i < grabbedWeaponInfoList.Count; i++) {
			if (grabbedWeaponInfoList [i].isEmptyWeaponToUseOnlyShield) {
				return grabbedWeaponInfoList [i].Name;
			}
		}

		return "";
	}

	public void setShieldActiveFieldValueDirectly (bool state)
	{
		shieldActive = state;

		updateShieldStateOnAnimator ();
	}

	void updateShieldStateOnAnimator ()
	{
		if (blockActive) {
			if (shieldActive) {
				mainPlayerController.setCurrentShieldActiveValue (2);
			} else {
				mainPlayerController.setCurrentShieldActiveValue (0);
			}
		} else {
			if (shieldActive) {
				mainPlayerController.setCurrentShieldActiveValue (1);
			} else {
				mainPlayerController.setCurrentShieldActiveValue (0);
			}
		}
	}

	public void drawOrSheatheShield (bool state)
	{
		if (carryingShield) {
			if (currentShieldGameObject != null) {

				if (!currentShieldGameObject.activeSelf) {
					currentShieldGameObject.SetActive (true);
				}

				if (shieldActive) {
					setShieldParentState (state);
				}
					
				if (blockActive) {
					disableShieldBlockActive ();
				}
			}
		}
	}

	public void setShieldParentState (bool state)
	{
		if (state) {
			currentShieldGameObject.transform.SetParent (shieldLeftHandMountPoint);

			currentShieldGameObject.transform.localPosition = currentShieldHandMountPointTransformReference.localPosition;
			currentShieldGameObject.transform.localRotation = currentShieldHandMountPointTransformReference.localRotation;
		} else {
			currentShieldGameObject.transform.SetParent (shieldBackMountPoint);

			currentShieldGameObject.transform.localPosition = currentShieldBackMountPointTransformReference.localPosition;
			currentShieldGameObject.transform.localRotation = currentShieldBackMountPointTransformReference.localRotation;
		}
	}

	public void checkIfCarryingShieldActive ()
	{
		setShieldActiveState (carryingShield);
	}

	public void setShieldInfo (string newShieldName, GameObject newShieldGameObject, Transform newShieldHandMountPointTransformReference, 
	                           Transform newShieldBackMountPointTransformReference, bool state)
	{
		currentShieldName = newShieldName;

		currentShieldGameObject = newShieldGameObject;

		currentShieldHandMountPointTransformReference = newShieldHandMountPointTransformReference;

		currentShieldBackMountPointTransformReference = newShieldBackMountPointTransformReference;

		carryingShield = state;

		if (blockActive) {
			disableShieldBlockActive ();
		}

		if (!carryingShield) {
			if (carryingObject) {
				if (shieldCanBeUsedWithoutMeleeWeapon) {
					if (currentGrabbedWeaponInfo.isEmptyWeaponToUseOnlyShield) {
						mainMeleeWeaponsGrabbedManager.disableCurrentMeleeWeapon (currentGrabbedWeaponInfo.Name);

						print ("removing empty weapon used only when carrying just the shield");
					}
				}
			}
		}
	}

	void disableShieldBlockActive ()
	{
		setBlockActiveState (false);

		if (showDebugPrint) {
			print ("check disable shield block state");
		}
	}
	//END SHIELD FUNCTIONS

	public bool checkIfCanUseMeleeWeaponPrefabByName (string weaponName)
	{
		return mainMeleeWeaponsGrabbedManager.checkIfCanUseMeleeWeaponPrefabByName (weaponName);
	}

	public int getCurrentNumberOfWeaponsAvailable ()
	{
		return mainMeleeWeaponsGrabbedManager.getCurrentNumberOfWeaponsAvailable ();
	}

	public void dropMeleeWeaponsExternallyWithoutResult ()
	{
		if (!grabbedObjectMeleeAttackActive) {
			return;
		}

		if (!carryingObject) {
			return;
		}

		mainGrabObjects.dropObject ();
	}

	public void dropMeleeWeaponsExternallyWithoutResultAndDestroyIt ()
	{
		if (currentMeleeWeapon != null) {
			if (attackInProcess) {
				disableCurrentAttackInProcess ();

				mainPlayerController.stopAllActionsOnActionSystem ();
			}

			GameObject lastWeaponObject = currentMeleeWeapon.gameObject;

			dropMeleeWeaponsExternallyWithoutResult ();

			if (lastWeaponObject != null) {
				Destroy (lastWeaponObject);
			}
		}
	}

	public bool dropMeleeWeaponsExternally ()
	{
		if (!grabbedObjectMeleeAttackActive) {
			return false;
		}

		if (!carryingObject) {
			return false;
		}

		mainGrabObjects.dropObject ();
	
		return true;
	}

	public void drawNextWeaponAvailable ()
	{
		mainMeleeWeaponsGrabbedManager.drawNextWeaponAvailable ();
	}

	public void drawRandomWeaponFromPrefabList (string previousWeaponName)
	{
		mainMeleeWeaponsGrabbedManager.drawRandomWeaponFromPrefabList (previousWeaponName);
	}

	void setOriginalHitCombatScale ()
	{
		setHitCombatScale (originalHitCombatColliderSize);
	}

	void setOriginalHitCombatOffset ()
	{
		setHitCombatOffset (originalHitCombatColliderCenter);
	}

	void setHitCombatScale (Vector3 newScale)
	{
		if (currentHitCombatBoxCollider != null) {
			currentHitCombatBoxCollider.size = newScale;
		}
	}

	void setHitCombatOffset (Vector3 newValue)
	{
		if (currentHitCombatBoxCollider != null) {
			currentHitCombatBoxCollider.center = newValue;
		}
	}

	public void resumeState ()
	{
		attackInProcess = false;

		cutActivatedOnAttackChecked = false;

		lastTimeCutActiveOnAttack = 0;

		damageTriggerInProcess = false;

		if (currentHitCombat != null) {
			currentHitCombat.setCurrentState (false);

			setHitCombatScale (originalHitCombatColliderSize);

			setHitCombatOffset (originalHitCombatColliderCenter);

			if (hitCombatParentChanged) {
				currentHitCombat.transform.SetParent (objectRotationPoint);
				currentHitCombat.transform.localPosition = originalHitCombatColliderPosition;
				currentHitCombat.transform.localRotation = Quaternion.identity;

				Transform raycastCheckTransformParent = currentMeleeWeapon.raycastCheckTransformParent;
				raycastCheckTransformParent.SetParent (objectRotationPoint);
				raycastCheckTransformParent.localPosition = Vector3.zero;
				raycastCheckTransformParent.localRotation = Quaternion.identity;

				hitCombatParentChanged = false;
			}


			if (isDualWieldWeapon) {
				dualWieldHitCombat.setCurrentState (false);

				mainDualWieldMeleeWeaponObjectSystem.setOriginalHitCombatScale ();

				mainDualWieldMeleeWeaponObjectSystem.setOriginalHitCombatOffset ();
			}

			checkSetArmorActiveState (false);
		}

		checkEventsOnAttack (false);

		mainGrabObjects.setGrabObjectsInputPausedState (false);

		currentWaitTimeToNextAttack = 0;
	}

	public void checkEventsOnAttack (bool state)
	{
		if (useEventsOnAttack) {
			if (state) {
				eventOnAttackStart.Invoke ();
			} else {
				eventOnAttackEnd.Invoke ();
			}
		}
	}

	public void checkEventWhenKeepingOrDrawingMeleeWeapon (bool state)
	{
		if ((carryingObject || state) && currentMeleeWeapon != null) {
			currentMeleeWeapon.checkEventWhenKeepingOrDrawingMeleeWeapon (state);

			if (currentGrabbedWeaponInfo != null) {

				if (currentGrabbedWeaponInfo.useEventsOnKeepOrDrawMeleeWeapon) {
					if (state) {
						currentGrabbedWeaponInfo.eventsOnKeepMeleeWeapon.Invoke ();
					} else {
						currentGrabbedWeaponInfo.eventsOnDrawMeleeWeapon.Invoke ();
					}
				}
			}
		}
	}

	public void setRemoteEventOnCurrentMeleeWeapon (string remoteEventName)
	{
		if (carryingObject && currentMeleeWeapon != null) {
			remoteEventSystem currentRemoteEventSystem = currentMeleeWeapon.GetComponent<remoteEventSystem> ();

			if (currentRemoteEventSystem != null) {

				if (showDebugPrint) {
					print ("remote event on weapon activated");
				}
					
				currentRemoteEventSystem.callRemoteEvent (remoteEventName);
			}
		}
	}

	public void setRemoteEventsOnCurrentMeleeWeapon (List<string> remoteEventNameList, GameObject weaponObject)
	{
		if (weaponObject != null) {
			remoteEventSystem currentRemoteEventSystem = weaponObject.GetComponent<remoteEventSystem> ();

			if (currentRemoteEventSystem != null) {

				if (showDebugPrint) {
					print ("remote event on weapon activated");
				}

				for (int i = 0; i < remoteEventNameList.Count; i++) {
					currentRemoteEventSystem.callRemoteEvent (remoteEventNameList [i]);
				}
			}
		}
	}

	public bool setDamageTypeAndReactionInfo (string newBuffObjectName)
	{
		if (carryingObject && currentMeleeWeapon != null) {
			return currentMeleeWeapon.setDamageTypeAndReactionInfo (newBuffObjectName);
		}

		return false;
	}

	public void activateReturnProjectilesOnContact ()
	{
		if (carryingObject && currentMeleeWeapon != null) {
			currentMeleeWeapon.sendReturnProjectilesOnContactInfo (playerControllerGameObject.transform, mainCameraTransform);
		
			if (currentGrabbedWeaponInfo.useEventsOnDeflectProjectile) {
				currentGrabbedWeaponInfo.eventsOnDeflectProjectile.Invoke ();
			}
		}
	}

	public void setNewMeleeWeaponAttackInfoTemplate (string templateName)
	{
		if (carryingObject && currentMeleeWeapon != null && currentGrabbedWeaponInfo.useMeleeWeaponAttackInfoList && !attackInProcess) {
			int templateIndex = currentGrabbedWeaponInfo.meleeWeaponAttackInfoList.FindIndex (a => a.Name == templateName);

			if (templateIndex > -1) {
				setAttackInfoTemplateByIndex (templateIndex);
			}
		}
	}

	int currentAttackInfoTemplateIndex = 0;

	public void setNextMeleeWeaponAttackInfoTemplate ()
	{
		if (carryingObject && currentMeleeWeapon != null && currentGrabbedWeaponInfo.useMeleeWeaponAttackInfoList && !attackInProcess) {
			currentAttackInfoTemplateIndex++;

			if (currentAttackInfoTemplateIndex >= currentGrabbedWeaponInfo.meleeWeaponAttackInfoList.Count) {
				currentAttackInfoTemplateIndex = 0;
			}

			setAttackInfoTemplateByIndex (currentAttackInfoTemplateIndex);
		}
	}

	void setAttackInfoTemplateByIndex (int newIndex)
	{
		currentAttackIndex = 0;

		currentAttackInfoTemplateIndex = newIndex;

		meleeWeaponAttackInfo currentMeleeWeaponAttackInfo = currentGrabbedWeaponInfo.meleeWeaponAttackInfoList [newIndex];

		currentMeleeWeapon.setNewMeleeWeaponAttackInfoTemplate (currentMeleeWeaponAttackInfo, false);

		currentMeleeWeapon.copyTemplateToWeaponAttackInfo (false);

		attackListCount = currentMeleeWeapon.getAttackListCount ();

		meleeWeaponInfo mainMeleeWeaponInfo = currentMeleeWeaponAttackInfo.mainMeleeWeaponInfo;

		if (mainMeleeWeaponInfo.setNewMovementValues) {
			if (mainMeleeWeaponInfo.setNewIdleID) {
				mainPlayerController.setCurrentIdleIDValue (mainMeleeWeaponInfo.idleIDUsed);
			} else {
				mainPlayerController.setCurrentIdleIDValue (0);
			}

			if (mainMeleeWeaponInfo.setNewCrouchID) {
				mainPlayerController.setCurrentCrouchIDValue (mainMeleeWeaponInfo.crouchIDUsed);
			} else {
				mainPlayerController.setCurrentCrouchIDValue (0);
			}

			if (mainMeleeWeaponInfo.setNewMovementID) {
				mainPlayerController.setPlayerStatusIDValue (mainMeleeWeaponInfo.movementIDUsed);
			} else {
				mainPlayerController.resetPlayerStatusID ();
			}

			mainPlayerController.setCurrentStrafeIDValue (mainMeleeWeaponInfo.strafeIDUsed);

			if (mainMeleeWeaponInfo.useStrafeMode) {
				mainPlayerController.activateOrDeactivateStrafeMode (true);

				setStrafeModeOnAISystemState (true);
			} else {
				currentMeleeWeapon.setPreviousStrafeModeState (false);

				mainPlayerController.activateOrDeactivateStrafeMode (false);
			}

			currentGrabbedWeaponInfo.activateStrafeModeOnLockOnTargetActive = mainMeleeWeaponInfo.activateStrafeModeOnLockOnTargetActive;

			if (mainMeleeWeaponInfo.toggleStrafeModeIfRunningActive) {
				mainPlayerController.setDisableStrafeModeExternallyIfIncreaseWalkSpeedActiveState (true);
			} else {
				mainPlayerController.setDisableStrafeModeExternallyIfIncreaseWalkSpeedActiveState (false);
			}
				
			if (mainMeleeWeaponInfo.setSprintEnabledStateWithWeapon) {
				if (!mainMeleeWeaponInfo.sprintEnabledStateWithWeapon && mainPlayerController.isPlayerRunning ()) {
					mainPlayerController.stopSprint ();
				}

				mainPlayerController.setSprintEnabledState (mainMeleeWeaponInfo.sprintEnabledStateWithWeapon);
			} else {
				mainPlayerController.setOriginalSprintEnabledValue ();
			}
		}
	}

	public void inputSetNextMeleeWeaponAttackInfoTemplate ()
	{
		setNextMeleeWeaponAttackInfoTemplate ();
	}

	public void checkGrabbedWeaponInfoStateAtStart (string weaponInfoName, bool checkIfWeaponThrown)
	{
		currentGrabbedWeaponInfo = null;

		for (int i = 0; i < grabbedWeaponInfoList.Count; i++) {
			if (grabbedWeaponInfoList [i].Name.Equals (weaponInfoName)) {
				currentGrabbedWeaponInfo = grabbedWeaponInfoList [i];

				if (useThrowReturnMeleeWeaponSystemEnabled) {
					mainMeleeCombatThrowReturnWeaponSystem.setCurrentGrabbedWeaponInfo (currentGrabbedWeaponInfo);
				}

				bool setWeaponState = true;

				if (checkIfWeaponThrown && currentMeleeWeapon.isObjectThrown ()) {
					setWeaponState = false;
				}

				if (setWeaponState) {
					if (removeWeaponsFromManager) {
						currentMeleeWeapon.setPreviousStrafeModeState (mainPlayerController.isStrafeModeActive ());

						currentGrabbedWeaponInfo.previousStrafeMode = mainPlayerController.isStrafeModeActive ();
					} else {
						currentMeleeWeapon.setPreviousStrafeModeState (false);

						currentGrabbedWeaponInfo.previousStrafeMode = false;

						mainPlayerController.activateOrDeactivateStrafeMode (false);

						setStrafeModeOnAISystemState (false);
					}

					if (currentGrabbedWeaponInfo.useStrafeMode) {
						mainPlayerController.activateOrDeactivateStrafeMode (true);

						setStrafeModeOnAISystemState (true);
					} else {
						if (!checkIfWeaponThrown) {
							if (showDebugPrint) {
								print ("checking strafe mode when returning thrown weapon");
							}

							currentMeleeWeapon.setPreviousStrafeModeState (false);

							currentGrabbedWeaponInfo.previousStrafeMode = false;

							mainPlayerController.activateOrDeactivateStrafeMode (false);
						}
					}

					if (currentGrabbedWeaponInfo.toggleStrafeModeIfRunningActive) {
						mainPlayerController.setDisableStrafeModeExternallyIfIncreaseWalkSpeedActiveState (true);
					}

					mainPlayerController.setCurrentStrafeIDValue (currentGrabbedWeaponInfo.strafeIDUsed);

					if (currentGrabbedWeaponInfo.setNewIdleID) {
						mainPlayerController.setCurrentIdleIDValue (currentGrabbedWeaponInfo.idleIDUsed);
					}
				
					if (currentGrabbedWeaponInfo.setNewCrouchID) {
						mainPlayerController.setCurrentCrouchIDValue (currentGrabbedWeaponInfo.crouchIDUsed);
					}

					if (currentGrabbedWeaponInfo.setNewMovementID) {
						mainPlayerController.setPlayerStatusIDValue (currentGrabbedWeaponInfo.movementIDUsed);
					}

					if (currentGrabbedWeaponInfo.setSprintEnabledStateWithWeapon) {
						if (!currentGrabbedWeaponInfo.sprintEnabledStateWithWeapon && mainPlayerController.isPlayerRunning ()) {
							mainPlayerController.stopSprint ();
						}

						mainPlayerController.setSprintEnabledState (currentGrabbedWeaponInfo.sprintEnabledStateWithWeapon);
					}

					if (currentGrabbedWeaponInfo.useEventsOnGrabDropObject) {
						currentGrabbedWeaponInfo.eventOnGrabObject.Invoke ();
					}

					if (currentGrabbedWeaponInfo.useRemoteEventsOnGrabObject) {
						setRemoteEventsOnCurrentMeleeWeapon (currentGrabbedWeaponInfo.remoteEventOnGrabObject, currentMeleeWeapon.gameObject);
					}
				}
			}
		}
	}

	public void checkGrabbedWeaponInfoStateAtEnd ()
	{
		if (currentGrabbedWeaponInfo != null) {
			if (currentGrabbedWeaponInfo.setPreviousStrafeModeOnDropObject) {
				mainPlayerController.activateOrDeactivateStrafeMode (previousStrafeMode);

				setStrafeModeOnAISystemState (previousStrafeMode);
			}

			mainPlayerController.setCurrentStrafeIDValue (0);

			if (currentGrabbedWeaponInfo.setNewIdleID) {
				mainPlayerController.setCurrentIdleIDValue (0);
			}

			if (currentGrabbedWeaponInfo.setNewCrouchID) {
				mainPlayerController.setCurrentCrouchIDValue (0);
			}

			if (currentGrabbedWeaponInfo.setNewMovementID) {
				mainPlayerController.resetPlayerStatusID ();
			}

			if (currentGrabbedWeaponInfo.setSprintEnabledStateWithWeapon) {
				mainPlayerController.setOriginalSprintEnabledValue ();
			}

			if (currentGrabbedWeaponInfo.useEventsOnGrabDropObject) {
				currentGrabbedWeaponInfo.eventOnDropObject.Invoke ();
			}

			if (currentGrabbedWeaponInfo.toggleStrafeModeIfRunningActive) {
				mainPlayerController.setDisableStrafeModeExternallyIfIncreaseWalkSpeedActiveState (false);
			}

			if (currentGrabbedWeaponInfo.useRemoteEventsOnDropObject) {
				if (currentGrabbedObjectTransform != null) {
					setRemoteEventsOnCurrentMeleeWeapon (currentGrabbedWeaponInfo.remoteEventOnDropObject, currentGrabbedObjectTransform.gameObject);
				}
			}
		}

		currentGrabbedWeaponInfo.previousStrafeMode = false;

		currentGrabbedWeaponInfo = null;
	}

	public void checkLookAtTargetActiveState ()
	{
		if (carryingObject && currentGrabbedWeaponInfo != null) {
			bool lookingAtTarget = mainPlayerController.isPlayerLookingAtTarget ();

			bool lockedCameraActive = mainPlayerController.isLockedCameraStateActive ();

			bool canSetStrafeMode = false;

			if (lockedCameraActive) {
				if (mainPlayerController.istargetToLookLocated ()) {
					canSetStrafeMode = true;
				} else {
					if (lookingAtTarget) {
						canSetStrafeMode = true;

						lookingAtTarget = false;
					}
				}
			} else {
				canSetStrafeMode = true;
			}

			if (lookingAtTarget) {
				if (canSetStrafeMode) {
					if (currentGrabbedWeaponInfo.activateStrafeModeOnLockOnTargetActive) {
						mainPlayerController.activateOrDeactivateStrafeMode (true);

						setStrafeModeOnAISystemState (true);
					}
				}
			} else {
				if (canSetStrafeMode) {
					if (currentGrabbedWeaponInfo.activateStrafeModeOnLockOnTargetActive) {
						mainPlayerController.activateOrDeactivateStrafeMode (false);

						setStrafeModeOnAISystemState (false);
					}
				}
			}

			updateBlockShieldState ();
		}
	}

	public void checkLookAtTargetDeactivateState ()
	{
		if (carryingObject && currentGrabbedWeaponInfo != null) {
			if (currentGrabbedWeaponInfo.activateStrafeModeOnLockOnTargetActive) {
				mainPlayerController.activateOrDeactivateStrafeMode (false);

				setStrafeModeOnAISystemState (false);
			}

			updateBlockShieldState ();
		}
	}

	public void setStrafeModeOnAISystemState (bool state)
	{
		if (mainFindObjectivesSystem != null) {
			mainFindObjectivesSystem.setStrafeModeActive (state);
		}
	}

	public void setStrafeModeState (bool state, int strafeID)
	{
		mainPlayerController.activateOrDeactivateStrafeMode (state);

		mainPlayerController.setCurrentStrafeIDValue (strafeID);
	}

	public void setCurrentCrouchIDValue (int crouchID)
	{
		mainPlayerController.setCurrentCrouchIDValue (crouchID);
	}

	public bool isStrafeModeActive ()
	{
		return mainPlayerController.isStrafeModeActive ();
	}

	public int getCurrentStrafeID ()
	{
		return mainPlayerController.getCurrentStrafeIDValue ();
	}

	public int getCurrentCrouchID ()
	{
		return mainPlayerController.getCurrentCrouchIDValue ();
	}

	public bool isActionActive ()
	{
		return mainPlayerController.isActionActive ();
	}

	public void setObjectThrownState (bool state)
	{
		objectThrown = state;
	}



	void checkSurfacesDetectedRaycast (float capsuleRadius, bool enableMainHitCombat)
	{
		if (enableMainHitCombat) {
			currentRayOriginPosition = raycastCheckTransfrom.position;
			currentRayTargetPosition = currentRayOriginPosition + raycastCheckTransfrom.forward * capsuleCastDistance;
		} else {
			currentRayOriginPosition = mainDualWieldMeleeWeaponObjectSystem.raycastCheckTransfrom.position;
			currentRayTargetPosition = currentRayOriginPosition + raycastCheckTransfrom.forward * capsuleCastDistance;
		}

		distanceToTarget = GKC_Utils.distance (currentRayOriginPosition, currentRayTargetPosition);
		rayDirection = currentRayOriginPosition - currentRayTargetPosition;
		rayDirection = rayDirection / rayDirection.magnitude;

		if (showDebugDraw) {
			Debug.DrawLine (currentRayTargetPosition, (rayDirection * distanceToTarget) + currentRayTargetPosition, Color.red, 2);
		}

		point1 = currentRayOriginPosition - rayDirection * capsuleCastRadius;
		point2 = currentRayTargetPosition + rayDirection * capsuleCastRadius;

		hits = Physics.CapsuleCastAll (point1, point2, capsuleRadius, rayDirection, 0, currentHitCombat.layerMask);
	}

	public void setGrabbedObjectClonnedColliderEnabledState (bool state)
	{
		mainGrabObjects.setGrabbedObjectClonnedColliderEnabledState (state);
	}

	List<GameObject> detectedObjectsOnReturn = new List<GameObject> ();

	bool checkingDamageDetectionOnDualWieldTriggerActive;

	public void setDamageDetectedOnDualWieldTriggerById (int attackID)
	{
		checkingDamageDetectionOnDualWieldTriggerActive = true;

		checkingNoDamageDetectionOnDualWieldTriggerActive = true;

		setDamageDetectedOnTriggerById (attackID);

		checkingDamageDetectionOnDualWieldTriggerActive = false;
	}

	public void setDamageDetectedOnTriggerById (int attackID)
	{
		if (!carryingObject) {
			return;
		}

		if (currentGrabbedWeaponInfo.useEventsOnDamageDetected) {
			for (int i = 0; i < currentGrabbedWeaponInfo.eventOnDamageInfoList.Count; i++) {
				eventOnDamageInfo currentEventOnDamageInfo = currentGrabbedWeaponInfo.eventOnDamageInfoList [i];

				if (currentEventOnDamageInfo.damageInfoID == attackID) {
					currentEventOnDamageInfo.eventOnDamageDetected.Invoke ();

					if (currentEventOnDamageInfo.useRemoteEvent) {
						bool useRemoteEvents = false;

						GameObject objectDetected =	null;

						if (checkingDamageDetectionOnDualWieldTriggerActive) {
							objectDetected = dualWieldHitCombat.getLastSurfaceDetected ();
						} else {
							objectDetected = currentHitCombat.getLastSurfaceDetected ();
						}

						if (objectDetected != null) {

							if (showDebugPrint) {
								print ("object detected to set remote event");
							}

							if (currentGrabbedWeaponInfo.checkObjectsToUseRemoteEventsOnDamage) {
								if ((1 << objectDetected.layer & currentGrabbedWeaponInfo.layerToUseRemoteEventsOnDamage.value) == 1 << objectDetected.layer) {
									useRemoteEvents = true;
								}
							} else {
								useRemoteEvents = true;
							}

							if (useRemoteEvents) {
								remoteEventSystem currentRemoteEventSystem = objectDetected.GetComponent<remoteEventSystem> ();

								if (currentRemoteEventSystem != null) {
									if (showDebugPrint) {
										print (currentRemoteEventSystem.name);
									}

									for (int j = 0; j < currentEventOnDamageInfo.remoteEventNameList.Count; j++) {
										currentRemoteEventSystem.callRemoteEvent (currentEventOnDamageInfo.remoteEventNameList [j]);
									}
								}
							}
						}
					}

					checkSurfaceFoundOnAttack (true);

					checkingDamageDetectionOnDualWieldTriggerActive = false;

					return;
				}
			}
		}

		checkSurfaceFoundOnAttack (true);

		checkingDamageDetectionOnDualWieldTriggerActive = false;
	}

	bool checkingNoDamageDetectionOnDualWieldTriggerActive;

	public void setNoDamageDetectedOnDualWieldTriggerById (GameObject objectDeteted)
	{
		checkingNoDamageDetectionOnDualWieldTriggerActive = true;

		checkSurfaceFoundOnAttack (true);
	}

	public void setNoDamageDetectedOnTriggerById (GameObject objectDetected)
	{
		checkSurfaceFoundOnAttack (true);
	}

	public void setPressDownState ()
	{
		if (currentGrabbedWeaponInfo != null && currentMeleeWeapon != null) {
			if (currentGrabbedWeaponInfo.useEventsOnPressDownState) {
				currentGrabbedWeaponInfo.eventOnPressDownState.Invoke ();
			}
		}
	}

	public void setPressUpState ()
	{
		if (currentGrabbedWeaponInfo != null && currentMeleeWeapon != null) {
			if (currentGrabbedWeaponInfo.useEventsOnPressUpState) {
				currentGrabbedWeaponInfo.eventOnPressUpState.Invoke ();
			}
		}
	}


	//Input functions
	public void inputSetPressDownState ()
	{
		if (showDebugPrint) {
			print ("input activated");
		}

		if (!grabbedObjectMeleeAttackActive) {
			return;
		}

		if (meleeAttackInputPaused) {
			return;
		}

		if (!canUseWeaponsInput ()) {
			return;
		}

		if (showDebugPrint) {
			print ("1");
		}

		if (objectThrown) {
			return;
		}

		if (cuttingModeActive) {
			return;
		}

		setPressDownState ();
	}

	public void inputSetPressUpState ()
	{
		if (showDebugPrint) {
			print ("input activated");
		}

		if (!grabbedObjectMeleeAttackActive) {
			return;
		}

		if (meleeAttackInputPaused) {
			return;
		}

		if (!canUseWeaponsInput ()) {
			return;
		}

		if (showDebugPrint) {
			print ("1");
		}

		if (objectThrown) {
			return;
		}

		if (cuttingModeActive) {
			return;
		}

		setPressUpState ();
	}


	public void inputActivateBlock ()
	{
		if (!blockModeEnabled) {
			return;
		}

		if (blockInputPaused) {
			return;
		}

		if (!canUseWeaponsInput ()) {
			return;
		}

		if (!grabbedObjectMeleeAttackActive) {
			blockActivePreviously = false;

			return;
		}

		if (objectThrown) {
			return;
		}

		if (cuttingModeActive) {
			return;
		}

		if (currentMeleeWeapon.keepGrabbedObjectState) {
			return;
		}

		if (attackInProcess) {
			blockActivePreviously = true;

			return;
		}

		if (blockActive) {
			return;
		}

		if (!mainPlayerController.isPlayerOnGround ()) {
			return;
		}

		setBlockActiveState (true);
	}

	public void inputDeactivateBlock ()
	{
		if (!grabbedObjectMeleeAttackActive) {
			return;
		}

		if (!canUseWeaponsInput ()) {
			return;
		}
			
		if (attackInProcess) {

			blockActivePreviously = false;

			return;
		}

		if (objectThrown) {
			return;
		}

		if (!blockActive) {
			return;
		}

		if (cuttingModeActive) {
			return;
		}

		setBlockActiveState (false);
	}

	public void inputSetNextExtraActionOnMeleeWeapon ()
	{
		if (!grabbedObjectMeleeAttackActive) {
			return;
		}

		if (objectThrown) {
			return;
		}

		if (meleeAttackInputPaused) {
			return;
		}

		if (!canUseWeaponsInput ()) {
			return;
		}

		if (cuttingModeActive) {
			return;
		}

		if (!currentMeleeWeapon.useExtraActions) {
			return;
		}

		currentMeleeWeapon.setNextExtraActionOnMeleeWeapon (mainRemoteEventSystem);

		if (showExtraActionIconTexture) {
			setCurrentExtraActionTexture (currentMeleeWeapon.getActionIconTexture ());
		}
	}

	void setCurrentExtraActionTexture (Texture currentExtraActionTexture)
	{
		if (showExtraActionIconTexture) {
			if (extraActionIconGameObject != null) {
				if (currentExtraActionTexture != null) {
				
					if (!extraActionIconGameObject.activeSelf) {
						extraActionIconGameObject.SetActive (true);
					}

					extraActionRawImage.texture = currentExtraActionTexture;
				} else {
					if (extraActionIconGameObject.activeSelf) {
						extraActionIconGameObject.SetActive (false);
					}
				}
			}
		}
	}

	void disableExtraActionIconGameObject ()
	{
		if (showExtraActionIconTexture) {
			if (extraActionIconGameObject != null) {
				if (extraActionIconGameObject.activeSelf) {
					extraActionIconGameObject.SetActive (false);
				}
			}
		}
	}

	public void enableExtraActionByName (string actionName)
	{
		enableOrDisableExtraActionByName (actionName, true);
	}

	public void disableExtraActionByName (string actionName)
	{
		enableOrDisableExtraActionByName (actionName, false);
	}

	public void enableOrDisableExtraActionByName (string actionName, bool state)
	{
		if (!grabbedObjectMeleeAttackActive) {
			return;
		}

		if (!carryingObject) {
			return;
		}

		if (!currentMeleeWeapon.useExtraActions) {
			return;
		}

		currentMeleeWeapon.enableOrDisableExtraActionByName (actionName, state);
	}

	public void inputActivateCurrentExtraActionOnMeleeWeapon ()
	{
		if (!throwWeaponActionPaused) {
			return;
		}

		if (!grabbedObjectMeleeAttackActive) {
			return;
		}

		if (meleeAttackInputPaused) {
			return;
		}

		if (!canUseWeaponsInput ()) {
			return;
		}
			
		if (objectThrown) {
			return;
		}

		if (cuttingModeActive) {
			return;
		}

		if (!currentMeleeWeapon.useExtraActions) {
			return;
		}

		if (currentMeleeWeapon.checkIfCarryingMeleeWeaponToActivateAction ()) {
			if (!carryingObject) {
				return;
			}
		}

		currentMeleeWeapon.activateCurrentExtraActionOnMeleeWeapon (mainRemoteEventSystem);
	}

	public void setThrowWeaponActionPausedState (bool state)
	{
		throwWeaponActionPaused = state;

		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setThrowWeaponActionPausedState (state);
		}
	}

	public void inputThrowOrReturnObject ()
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.inputThrowOrReturnObject ();
		}
	}

	public void inputPressDownThrowObject ()
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.inputPressDownThrowObject ();
		}
	}

	public void inputPressUpThrowObject ()
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.inputPressUpThrowObject ();
		}
	}

	public void inputEnableOrDisableCuttingMode ()
	{
		if (!grabbedObjectMeleeAttackActive) {
			return;
		}

		if (!canUseWeaponsInput ()) {
			return;
		}

		if (objectThrown) {
			return;
		}

		if (blockActive) {
			return;
		}

		if (attackInProcess) {
			return;
		}

		if (currentMeleeWeapon.canActivateCuttingMode) {
			enableOrDisableCuttingMode (!cuttingModeActive);
		}
	}

	public void inputActivateTeleport ()
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.inputActivateTeleport ();
		}
	}

	public void enableOrDisableCuttingMode (bool state)
	{
		if (!cuttingModeEnabled) {
			return;
		}

		cuttingModeActive = state;

		if (cuttingModeActive) {
			eventOnCuttingModeStart.Invoke ();
		} else {
			eventOnCuttingModeEnd.Invoke ();
		}
			
		if (shieldActive) {
			setShieldParentState (!cuttingModeActive);
		}
	}

	public void checkObjectsDetectedOnCuttingMode ()
	{
		if (cuttingModeActive) {
			List<GameObject> hitsGameObjectList = new List<GameObject> ();

			hitsGameObjectList = mainSliceSystem.getLastCollidersListDetected ();

			if (hitsGameObjectList.Count > 0) {
				for (int i = 0; i < hitsGameObjectList.Count; i++) {
					string surfaceName = surfaceInfoOnMeleeAttackNameForSwingOnAir;

					RaycastHit hit = new RaycastHit ();

					Vector3 raycastPosition = playerControllerGameObject.transform.position + playerControllerGameObject.transform.up;

					Vector3 raycastDirection = hitsGameObjectList [i].transform.position - raycastPosition;

					raycastDirection = raycastDirection / raycastDirection.magnitude;

					float currentRaycastDistance = GKC_Utils.distance (raycastPosition, hitsGameObjectList [i].transform.position);

					currentRaycastDistance += 1;

					Physics.Raycast (raycastPosition, raycastDirection, out hit, currentRaycastDistance, currentHitCombat.layerMask);

					bool ignoreDurability = true;
					float extraDurabilityMultiplier = 0;

					meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = hitsGameObjectList [i].GetComponent<meleeAttackSurfaceInfo> ();

					bool sendSurfaceLocatedToCheck = true;

					if (currentMeleeAttackSurfaceInfo != null) {
						if (!currentMeleeAttackSurfaceInfo.isSurfaceEnabled ()) {
							sendSurfaceLocatedToCheck = false;
						} else {
							surfaceName = currentMeleeAttackSurfaceInfo.getSurfaceName ();

							currentMeleeAttackSurfaceInfo.checkEventOnSurfaceDetected ();
						}

						if (checkDurabilityOnAttackEnabled) {
							if (!currentMeleeAttackSurfaceInfo.ignoreDurability) {
								ignoreDurability = false;

								extraDurabilityMultiplier = currentMeleeAttackSurfaceInfo.extraDurabilityMultiplier;
							}
						}
					} else {
						sendSurfaceLocatedToCheck = false;
					}
		
					if (sendSurfaceLocatedToCheck) {
						checkSurfaceFoundOnAttackToProcess (surfaceName, true, hit.point, hit.normal, false, false);
					}

					checkDurabilityOnAttackOnCurrentWeapon (ignoreDurability, extraDurabilityMultiplier);
				}
			} else {
				checkSurfaceFoundOnAttack (false);
			}
		}
	}

	public void checkEventOnGrabDropObject (bool state)
	{
		checKEventsHideShowInventoryQuickAccessSlots (state);

		if (useEventsOnGrabDropObject) {
			if (state) {
				eventOnGrabObject.Invoke ();
			} else {
				eventOnDropObject.Invoke ();
			}
		}
	}

	public void checkEventsIfObjectGrabbed ()
	{
		if (carryingObject) {
			checkEventOnGrabDropObject (true);
		}
	}

	public void checKEventsHideShowInventoryQuickAccessSlots (bool state)
	{
		if (hideInventoryQuickAccessSlotsWhenCarryingMeleeWeapon) {
			if (state) {
				eventOnHideInventoryQuickAccessSlots.Invoke ();
			} else {
				eventOnShowInventoryQuickAccessSlots.Invoke ();
			}
		}
	}

	public void setThrowObjectEnabledState (bool state)
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setThrowObjectEnabledState (state);
		}
	}

	public void setReturnObjectEnabledState (bool state)
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setReturnObjectEnabledState (state);
		}
	}

	public void setApplyDamageOnSurfaceDetectedOnReturnEnabledState (bool state)
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setApplyDamageOnSurfaceDetectedOnReturnEnabledState (state);
		}
	}

	public void setApplyDamageOnObjectReturnPathEnabledState (bool state)
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setApplyDamageOnObjectReturnPathEnabledState (state);
		}
	}

	public void setGeneralDamageMultiplierOnObjectReturnValue (float newValue)
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setGeneralDamageMultiplierOnObjectReturnValue (newValue);
		}
	}

	public void setGeneralDamageMultiplierOnReturnPathValue (float newValue)
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setGeneralDamageMultiplierOnReturnPathValue (newValue);
		}
	}

	public void setGeneralDamageOnSurfaceDetectedOnThrowValue (float newValue)
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			mainMeleeCombatThrowReturnWeaponSystem.setGeneralDamageOnSurfaceDetectedOnThrowValue (newValue);
		}
	}

	public void setCuttingModeEnabledState (bool state)
	{
		cuttingModeEnabled = state;
	}

	public void setBlockModeEnabledState (bool state)
	{
		blockModeEnabled = state;
	}

	public void setGeneralBlockProtectionMultiplierValue (float newValue)
	{
		generalBlockProtectionMultiplier = newValue;
	}

	public void setUseStaminaOnAttackEnabledState (bool state)
	{
		useStaminaOnAttackEnabled = state;
	}

	public void setGeneralStaminaUseMultiplierValue (float newValue)
	{
		generalStaminaUseMultiplier = newValue;
	}

	public float getGeneralStaminaUseMultiplier ()
	{
		return generalStaminaUseMultiplier;
	}

	public void setGeneralAttackDamageMultiplierValue (float newValue)
	{
		generalAttackDamageMultiplier = newValue;
	}

	public bool isObjectThrownTravellingToTarget ()
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			return mainMeleeCombatThrowReturnWeaponSystem.isObjectThrownTravellingToTarget ();
		}

		return false;
	}

	public bool isAttackInProcess ()
	{
		return attackInProcess;
	}

	public bool isBlockActive ()
	{
		return blockActive;
	}

	public bool canUseWeaponsInput ()
	{
		if (playerIsBusy ()) {
			return false;
		}

		if (mainPlayerController.iscloseCombatAttackInProcess ()) {
			return false;
		}

		return true;
	}

	public bool playerIsBusy ()
	{
		if (mainPlayerController.isUsingDevice ()) {
			return true;
		}

		if (mainPlayerController.isUsingSubMenu ()) {
			return true;
		}

		if (mainPlayerController.isPlayerMenuActive ()) {
			return true;
		}

		return false;
	}

	public float getLastTimeObjectReturn ()
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			return mainMeleeCombatThrowReturnWeaponSystem.getLastTimeObjectReturn ();
		}

		return 0;
	}

	public float getLastTimeObjectThrown ()
	{
		if (useThrowReturnMeleeWeaponSystemEnabled) {
			return mainMeleeCombatThrowReturnWeaponSystem.getLastTimeObjectThrown ();
		}

		return 0;
	}

	public bool isCurrentWeaponThrown ()
	{
		return objectThrown;
	}

	public void setMeleeAttackInputPausedState (bool state)
	{
		meleeAttackInputPaused = state;
	}

	Coroutine pauseMeleeAttackInputCoroutine;

	public void disableMeleeAttackInputPausedStateWithDuration (float pauseDuration)
	{
		stopDisableMeleeAttackInputPausedStateWithDurationCoroutine ();

		pauseMeleeAttackInputCoroutine = StartCoroutine (disableMeleeAttackInputPausedStateWithDurationCoroutine (pauseDuration));
	}

	void stopDisableMeleeAttackInputPausedStateWithDurationCoroutine ()
	{
		if (pauseMeleeAttackInputCoroutine != null) {
			StopCoroutine (pauseMeleeAttackInputCoroutine);
		}
	}

	public bool isCarryingObject ()
	{
		return carryingObject;
	}

	public grabPhysicalObjectMeleeAttackSystem getCurrentGrabPhysicalObjectMeleeAttackSystem ()
	{
		return currentMeleeWeapon;
	}

	public void checkSurfaceFoundOnAttack (bool surfaceLocated)
	{
		if (!checkSurfaceInfoEnabled) {
			return;
		}

		if (objectThrown) {
			return;
		}

		cutActivatedOnAttackChecked = false;

		lastTimeCutActiveOnAttack = 0;
			
		string surfaceName = surfaceInfoOnMeleeAttackNameForSwingOnAir;

		Vector3 attackPosition = Vector3.zero;
		Vector3 attackNormal = Vector3.zero;

		RaycastHit hit = new RaycastHit ();

		bool useMainHitCombat = true;

		if (checkingNoDamageDetectionOnDualWieldTriggerActive) {
			useMainHitCombat = false;
		}

		checkingNoDamageDetectionOnDualWieldTriggerActive = false;

		if (showDebugPrint) {
			print ("Surface located " + surfaceLocated + " using main hit combat " + useMainHitCombat);
		}

		bool ignoreDurability = true;
		float extraDurabilityMultiplier = 0;

		if (surfaceLocated) {
			GameObject lastSurfaceDetected = null;

			if (useMainHitCombat) {
				lastSurfaceDetected = currentHitCombat.getLastSurfaceDetected ();

				currentHitCombat.setLastSurfaceDetected (null);

				currentLayerMaskToCheck = currentHitCombat.layerMask;
			} else {
				lastSurfaceDetected = dualWieldHitCombat.getLastSurfaceDetected ();

				dualWieldHitCombat.setLastSurfaceDetected (null);

				currentLayerMaskToCheck = dualWieldHitCombat.layerMask;
			}

			GameObject surfaceFound = null;
		
			if (lastSurfaceDetected != null) {
				Collider lastSurfaceDetectedCollider = lastSurfaceDetected.GetComponent<Collider> ();

				Vector3 mainDamagePositionTransformPosition = currentMeleeWeapon.mainDamagePositionTransform.position;

				if (!useMainHitCombat) {
					mainDamagePositionTransformPosition = mainDualWieldMeleeWeaponObjectSystem.mainDamagePositionTransform.position;
				}

				Vector3 raycastPositionTarget = lastSurfaceDetectedCollider.ClosestPointOnBounds (mainDamagePositionTransformPosition);
		
				List<Transform> raycastCheckTransfromList = currentMeleeWeapon.raycastCheckTransfromList;

				if (!useMainHitCombat) {
					raycastCheckTransfromList = mainDualWieldMeleeWeaponObjectSystem.raycastCheckTransfromList;
				}

				for (int i = 0; i < raycastCheckTransfromList.Count; i++) {
					if (!surfaceFound) {			
						Vector3 raycastPosition = raycastCheckTransfromList [i].position;
			
						Vector3 raycastDirection = raycastPositionTarget - raycastPosition;
			
						raycastDirection = raycastDirection / raycastDirection.magnitude;
			
						float currentRaycastDistance = GKC_Utils.distance (raycastPosition, raycastPositionTarget);

						currentRaycastDistance += 0.5f;

						if (showDebugDraw) {
							Debug.DrawLine (raycastPosition, raycastPositionTarget, Color.black, 6);

							Debug.DrawLine (raycastPosition, raycastPosition + raycastDirection, Color.red, 4);

							Debug.DrawLine (raycastPosition + raycastDirection, 
								raycastPosition + (raycastDirection * 0.5f), Color.white, 4);

							Debug.DrawLine (raycastPositionTarget, raycastPositionTarget - (raycastDirection * 0.5f), Color.yellow, 4);
						}

						if (Physics.Raycast (raycastPosition, raycastDirection, out hit, currentRaycastDistance, currentLayerMaskToCheck)) {
							if (showDebugPrint) {
								print ("detected " + lastSurfaceDetected.name + " and raycast " + hit.collider.gameObject.name);
							}

							if (hit.collider.gameObject != playerControllerGameObject) {
								surfaceFound = hit.collider.gameObject;
							}
						} else {
							if (showDebugPrint) {
								print ("detected " + lastSurfaceDetected.name + " no raycast found");
							}

							if (raycastPositionTarget != Vector3.zero) {
								attackPosition = raycastPositionTarget;

								attackNormal = lastSurfaceDetectedCollider.transform.position - attackPosition;

								attackNormal = attackNormal / attackNormal.magnitude;
							}
						}
					}
				}

				bool checkCutModeOnAttack = false;

				if (!ignoreCutModeOnMeleeWeaponAttack && currentAttackInfo.useCutModeOnAttack) {
					if (isDualWieldWeapon) {
						if (Time.time > lastTimeCutActiveOnAttack + 0.3f || lastTimeCutActiveOnAttack == 0) {
							checkCutModeOnAttack = true;
						}
					} else {
						if (!cutActivatedOnAttackChecked) {
							checkCutModeOnAttack = true;
						}
					}
				}

				if (checkCutModeOnAttack) {
					Vector3 cutOverlapBoxSize = currentMeleeWeapon.cutOverlapBoxSize;
					Transform cutPositionTransform = currentMeleeWeapon.cutPositionTransform;
					Transform cutDirectionTransform = currentMeleeWeapon.cutDirectionTransform;

					Transform planeDefiner1 = currentMeleeWeapon.planeDefiner1;
					Transform planeDefiner2 = currentMeleeWeapon.planeDefiner2;
					Transform planeDefiner3 = currentMeleeWeapon.planeDefiner3;

					if (!useMainHitCombat) {
						cutPositionTransform = mainDualWieldMeleeWeaponObjectSystem.cutPositionTransform;
						cutDirectionTransform = mainDualWieldMeleeWeaponObjectSystem.cutDirectionTransform;

						planeDefiner1 = mainDualWieldMeleeWeaponObjectSystem.planeDefiner1;
						planeDefiner2 = mainDualWieldMeleeWeaponObjectSystem.planeDefiner2;
						planeDefiner3 = mainDualWieldMeleeWeaponObjectSystem.planeDefiner3;
					}

					mainSliceSystem.setCustomCutTransformValues (cutOverlapBoxSize, cutPositionTransform, cutDirectionTransform,
						planeDefiner1, planeDefiner2, planeDefiner3);

//					print ("CHECK CUT");

					mainSliceSystem.activateCutExternally ();

					cutActivatedOnAttackChecked = true;

					lastTimeCutActiveOnAttack = Time.time;
				}

				if (currentAttackInfo.pushCharactersOnDamage && currentAttackInfo.messageNameToSend != "") {

					bool activatePushCharacter = true;

					if (currentAttackInfo.useProbabilityToPushCharacters) {
						float pushProbability = Random.Range (1, 100);

						pushProbability /= 100;

//						print (lastSurfaceDetected.name + " " + currentAttackInfo.pushCharactersOnDamage + " " + pushProbability);

						if (currentAttackInfo.probability < pushProbability) {
							activatePushCharacter = false;
						}
					}

					if (currentAttackInfo.useIgnoreTagsToPush) {
						if (currentAttackInfo.tagsToIgnorePush.Contains (lastSurfaceDetected.tag)) {
							activatePushCharacter = false;
						}
					}

					if (activatePushCharacter) {
						Vector3 pushCharacterDirection = Vector3.zero;

						if (currentAttackInfo.ignoreMeleeWeaponAttackDirection) {
							pushCharacterDirection = playerControllerGameObject.transform.forward * currentAttackInfo.pushForceAmount;
						} else {
							pushCharacterDirection = currentMeleeWeapon.mainDamagePositionTransform.forward * currentAttackInfo.pushForceAmount;
						}

						if (currentAttackInfo.useExtraPushDirection) {
							Transform playerTransform = playerControllerGameObject.transform;

							pushCharacterDirection += playerTransform.right * currentAttackInfo.extraPushDirection.x;
							pushCharacterDirection += playerTransform.up * currentAttackInfo.extraPushDirection.y;
							pushCharacterDirection += playerTransform.forward * currentAttackInfo.extraPushDirection.z;

							pushCharacterDirection *= currentAttackInfo.extraPushForce;
						}

						lastSurfaceDetected.SendMessage (currentAttackInfo.messageNameToSend, pushCharacterDirection, SendMessageOptions.DontRequireReceiver);
					}
				}

				if (!ignoreGetHealthFromDamagingObjectsOnWeaponInfo) {
					if (currentAttackInfo.getHealthFromDamagingObjects) {
						float totalHealthAmount = currentHitCombat.getLastDamageApplied () * currentAttackInfo.healthFromDamagingObjectsMultiplier;

						if (totalHealthAmount > 0 && !mainHealth.checkIfMaxHealth ()) {

							mainHealth.getHealth (totalHealthAmount);
						}
					}
				}

				if (!ignoreGetHealthFromDamagingObjects) {
					if (currentGrabbedWeaponInfo.getHealthFromDamagingObjects) {
						float totalHealthAmount = currentHitCombat.getLastDamageApplied () * currentGrabbedWeaponInfo.healthFromDamagingObjectsMultiplier;

						if (totalHealthAmount > 0 && !mainHealth.checkIfMaxHealth ()) {

							mainHealth.getHealth (totalHealthAmount);
						}
					}
				}
			}

			if (surfaceFound != null) {
				if (showDebugPrint) {
					print ("SURFACE FOUND " + surfaceFound.name);
				}
					
				attackPosition = hit.point;
				attackNormal = hit.normal;
			} else {
				if (showDebugPrint) {
					print ("SURFACE NOT FOUND BY RAYCAST!!!!!!!!!!");
				}
			}

			if (lastSurfaceDetected != null) {
				meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = lastSurfaceDetected.GetComponent<meleeAttackSurfaceInfo> ();

				if (currentMeleeAttackSurfaceInfo != null) {
					if (!currentMeleeAttackSurfaceInfo.isSurfaceEnabled ()) {
						surfaceLocated = false;
					} else {

						surfaceName = currentMeleeAttackSurfaceInfo.getSurfaceName ();

						if (currentMeleeAttackSurfaceInfo.useRemoteEventOnWeapon) {
							remoteEventSystem currentRemoteEventSystem = currentMeleeWeapon.GetComponent<remoteEventSystem> ();

							if (currentRemoteEventSystem != null) {

								if (showDebugPrint) {
									print ("remote event on weapon detected");
								}

								for (int j = 0; j < currentMeleeAttackSurfaceInfo.remoteEventOnWeaponNameList.Count; j++) {
									currentRemoteEventSystem.callRemoteEvent (currentMeleeAttackSurfaceInfo.remoteEventOnWeaponNameList [j]);
								}
							}
						}

						if (currentMeleeWeapon.useRemoteEventOnSurfacesDetected) {
							if (currentMeleeAttackSurfaceInfo.useRemoteEvent) {
								remoteEventSystem currentRemoteEventSystem = lastSurfaceDetected.GetComponent<remoteEventSystem> ();

								if (currentRemoteEventSystem != null) {

									if (showDebugPrint) {
										print ("remote event on object detected");
									}

									for (int j = 0; j < currentMeleeAttackSurfaceInfo.remoteEventNameList.Count; j++) {
										string currentRemoteEventName = currentMeleeAttackSurfaceInfo.remoteEventNameList [j];

										if (currentMeleeWeapon.isRemoteEventIncluded (currentRemoteEventName)) {
											currentRemoteEventSystem.callRemoteEvent (currentRemoteEventName);
										}
									}
								}
							}
						}

						currentMeleeAttackSurfaceInfo.checkEventOnSurfaceDetected ();
					}

					if (checkDurabilityOnAttackEnabled) {
						if (!currentMeleeAttackSurfaceInfo.ignoreDurability) {
							ignoreDurability = false;

							extraDurabilityMultiplier = currentMeleeAttackSurfaceInfo.extraDurabilityMultiplier;
						}
					}
				} else {
					GameObject currentCharacter = applyDamage.getCharacterOrVehicle (lastSurfaceDetected);

					if (currentCharacter != null) {
						currentMeleeAttackSurfaceInfo = currentCharacter.GetComponent<meleeAttackSurfaceInfo> ();

						if (currentMeleeAttackSurfaceInfo != null) {
							if (!currentMeleeAttackSurfaceInfo.isSurfaceEnabled ()) {
								surfaceLocated = false;
							} else {
								surfaceName = currentMeleeAttackSurfaceInfo.getSurfaceName ();

								currentMeleeAttackSurfaceInfo.checkEventOnSurfaceDetected ();
							}

							if (checkDurabilityOnAttackEnabled) {
								if (!currentMeleeAttackSurfaceInfo.ignoreDurability) {
									ignoreDurability = false;

									extraDurabilityMultiplier = currentMeleeAttackSurfaceInfo.extraDurabilityMultiplier;
								}
							}
						}
					} else {
						surfaceLocated = false;
					}

					if (!surfaceLocated) {
						return;
					}
				}
			} else {
				if (showDebugPrint) {
					print ("SURFACE NOT FOUND BY TRIGGER!!!!!!!!!!");
				}
			}
		}

		bool ignoreBounceEvent = false;
		if (!currentAttackCanBeBlocked) {
			ignoreBounceEvent = true;
		}

		bool ignoreSoundOnSurface = false;

		if (attackActivatedOnAir && currentAttackInfo.pauseSoundIfAttackOnAir) {
			ignoreSoundOnSurface = true;
		}

		checkSurfaceFoundOnAttackToProcess (surfaceName, surfaceLocated, attackPosition, attackNormal, ignoreBounceEvent, ignoreSoundOnSurface);
	
		checkDurabilityOnAttackOnCurrentWeapon (ignoreDurability, extraDurabilityMultiplier);
	}

	public void checkSurfaceFoundOnAttackToProcess (string surfaceName, bool surfaceLocated, Vector3 attackPosition, Vector3 attackNormal, 
	                                                bool ignoreBounceEvent, bool ignoreSoundOnSurface)
	{
//		print ("surface name " + surfaceName);

		for (int i = 0; i < surfaceInfoOnMeleeAttackList.Count; i++) {
			surfaceInfoOnMeleeAttack currentSurfaceInfo = surfaceInfoOnMeleeAttackList [i];

			if (surfaceName.Equals (currentSurfaceInfo.surfaceName)) {

				int soundIndex = 0;

				if (currentSurfaceInfo.useSoundsListOnOrder) {
					currentSurfaceInfo.currentSoundIndex++;

					if (currentSurfaceInfo.currentSoundIndex >= currentSurfaceInfo.soundsList.Count) {
						currentSurfaceInfo.currentSoundIndex = 0;
					}

					soundIndex = currentSurfaceInfo.currentSoundIndex;
				} else {
					soundIndex = Random.Range (0, currentSurfaceInfo.soundsList.Count);
				}

				bool soundCanBePlayed = false;

				if (Time.time > lastTimeSurfaceAudioPlayed + 0.5f) {
					soundCanBePlayed = true;
				}

				if (lastSurfaceDetecetedIndex == -1 || lastSurfaceDetecetedIndex != i) {
					soundCanBePlayed = true;
				}

				if (soundCanBePlayed && !meleeAttackSoundPaused) {
					if (!ignoreSoundOnSurface) {
						mainAudioSource.PlayOneShot (currentSurfaceInfo.soundsList [soundIndex]);
					}

					lastTimeSurfaceAudioPlayed = Time.time;

					lastSurfaceDetecetedIndex = i;
				}

				if (surfaceLocated) {
					if (!ignoreBounceEvent) {
						if (currentSurfaceInfo.surfaceActivatesBounceOnCharacter) {
							currentSurfaceInfo.eventOnBounceCharacter.Invoke ();
						}
					}

					if (currentSurfaceInfo.useParticlesOnSurface && attackPosition != Vector3.zero) {
						GameObject newParticles = (GameObject)Instantiate (currentSurfaceInfo.particlesOnSurface, Vector3.zero, Quaternion.identity);
						newParticles.transform.position = attackPosition;
						newParticles.transform.LookAt (attackPosition + attackNormal * 3);
					}
				}

//				print ("surface type detected " + surfaceName);

				return;
			}
		}
	}

	bool meleeAttackSoundPaused;

	public void setMeleeAttackSoundPausedState (bool state)
	{
		meleeAttackSoundPaused = state;
	}

	public void addEventOnDamageInfoList (string weaponTypeName)
	{
		int currentIndex = grabbedWeaponInfoList.FindIndex (a => a.Name.Equals (weaponTypeName));

		if (currentIndex > -1) {
			eventOnDamageInfo newEventOnDamageInfo = new eventOnDamageInfo ();

			newEventOnDamageInfo.damageInfoID = grabbedWeaponInfoList [currentIndex].eventOnDamageInfoList.Count;

			grabbedWeaponInfoList [currentIndex].eventOnDamageInfoList.Add (newEventOnDamageInfo);
		}
	}

	public void setCheckDurabilityOnAttackEnabledStateFromEditor (bool state)
	{
		checkDurabilityOnAttackEnabled = state;

		updateComponent ();
	}

	public void setCheckDurabilityOnBlockEnabledStateFromEditor (bool state)
	{
		checkDurabilityOnBlockEnabled = state;

		updateComponent ();
	}

	public void setCustomIgnoreTagsForCharacterFromEditor ()
	{
		if (playerControllerGameObject != null) {
			useCustomIgnoreTags = true;

			customTagsToIgnoreList.Clear ();

			customTagsToIgnoreList.Add (playerControllerGameObject.tag);
	
			updateComponent ();
		}
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Melee System", gameObject);
	}

	[System.Serializable]
	public class grabbedWeaponInfo
	{
		public string Name;

		[Space]
		[Header ("Movements Settings")]
		[Space]

		public bool setNewIdleID;
		public int idleIDUsed;

		public bool useStrafeMode;
		public int strafeIDUsed;
		public bool setPreviousStrafeModeOnDropObject;
		public bool previousStrafeMode;

		public bool activateStrafeModeOnLockOnTargetActive;
		public bool deactivateStrafeModeOnLockOnTargetDeactivate;

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
		[Header ("Shield Settings")]
		[Space]

		public bool weaponCanUseShield;

		public int shieldID;

		public string regularMovementBlockShieldActionName;

		public int strafeMovementBlockShieldID;

		public int shieldIDFreeMovement;
		public int shieldIDStrafeMovement;

		public bool isEmptyWeaponToUseOnlyShield;

		[Space]
		[Header ("Events Settings")]
		[Space]

		public bool useEventsOnGrabDropObject;
		public UnityEvent eventOnGrabObject;
		public UnityEvent eventOnDropObject;

		[Space]
		[Header ("Keep/Draw Weapon Events Settings")]
		[Space]

		public bool useEventsOnKeepOrDrawMeleeWeapon;

		public UnityEvent eventsOnKeepMeleeWeapon;
		public UnityEvent eventsOnDrawMeleeWeapon;

		[Space]
		[Header ("Remote Events Settings")]
		[Space]

		public bool useRemoteEventsOnGrabObject;
		public List<string> remoteEventOnGrabObject = new List<string> ();

		[Space]

		public bool useRemoteEventsOnDropObject;
		public List<string> remoteEventOnDropObject = new List<string> ();

		[Space]
		[Header ("Attacks Unable To Block Settings")]
		[Space]

		public bool attacksCantBeBlocked;

		public List<int> attackIDCantBeBlockedList = new List<int> ();

		[Space]
		[Header ("Damage Detected Settings")]
		[Space]

		public bool useEventsOnDamageDetected;

		public bool checkObjectsToUseRemoteEventsOnDamage;
		public LayerMask layerToUseRemoteEventsOnDamage;

		public List<eventOnDamageInfo> eventOnDamageInfoList = new List<eventOnDamageInfo> ();

		[Space]
		[Header ("Obtain Health From Damage Applied Settings")]
		[Space]

		public bool getHealthFromDamagingObjects;
		public float healthFromDamagingObjectsMultiplier = 1;

		[Space]
		[Header ("Obtain Health From Blocks Settings")]
		[Space]

		public bool getHealthFromBlocks;
		public float healthAmountFromBlocks = 1;

		public bool getHealthFromPerfectBlocks;
		public float healthAmountFromPerfectBlocks = 1;

		[Space]
		[Header ("Range Attack Settings")]
		[Space]

		public bool useRangeAttackID;
		public List<rangeAttackInfo> rangeAttackInfoList = new List<rangeAttackInfo> ();

		[Space]
		[Header ("Press Up/Down Events Settings")]
		[Space]

		public bool useEventsOnPressDownState;
		public UnityEvent eventOnPressDownState;

		public bool useEventsOnPressUpState;
		public UnityEvent eventOnPressUpState;

		[Space]
		[Header ("Deflect Projectile Events Settings")]
		[Space]

		public bool useEventsOnDeflectProjectile;
		public UnityEvent eventsOnDeflectProjectile;

		[Space]
		[Header ("Custom Position Reference Settings")]
		[Space]

		public bool useCustomGrabbedWeaponReferencePosition;
		public Transform customGrabbedWeaponReferencePosition;

		[Space]

		public bool useCustomReferencePositionToKeepObjectMesh;
		public Transform customReferencePositionToKeepObjectMesh;

		[Space]

		public bool useCustomReferencePositionToKeepObject;
		public Transform customReferencePositionToKeepObject;

		[Space]
		[Header ("Attack List Info Template Settings")]
		[Space]

		public bool useMeleeWeaponAttackInfoList;

		public bool setInitialWeaponAttackInfoList;

		public int initialWeaponAttackInfoListIndex;

		public List<meleeWeaponAttackInfo> meleeWeaponAttackInfoList = new List<meleeWeaponAttackInfo> ();

		[Space]
		[Header ("Weapon Durability Settings")]
		[Space]

		public bool useEventOnEmptyDurability;
		public UnityEvent eventOnEmptyDurability;
	}

	[System.Serializable]
	public class eventOnDamageInfo
	{
		public int damageInfoID;
		public UnityEvent eventOnDamageDetected;

		public bool useRemoteEvent;
		public List<string> remoteEventNameList;
	}

	[System.Serializable]
	public class surfaceInfoOnMeleeAttack
	{
		public string surfaceName;

		[Space]
		[Header ("Sounds And Particles Settings")]
		[Space]

		public bool useSoundsListOnOrder;
		public int currentSoundIndex;
		public List<AudioClip> soundsList;

		public bool useParticlesOnSurface;
		public GameObject particlesOnSurface;

		[Space]
		[Header ("Events Settings")]
		[Space]

		public bool surfaceActivatesBounceOnCharacter;
		public UnityEvent eventOnBounceCharacter;
		public bool stopAttackOnBounce;
	}

	[System.Serializable]
	public class rangeAttackInfo
	{
		public int rangeAttackID;
		public UnityEvent eventOnRangeAttack;
		public UnityEvent eventOnDisableRangeAttack;
	}
}
