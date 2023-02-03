using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class grabPhysicalObjectMeleeAttackSystem : weaponObjectInfo
{
	[Header ("Main Settings")]
	[Space]

	public string weaponName;

	public bool useRandomAttackIndex;

	public bool onlyAttackIfNoPreviousAttackInProcess;
	public float minDelayBetweenAttacks = 0.5f;

	public bool resetIndexIfNotAttackAfterDelay;

	public float delayToResetIndexAfterAttack;

	public string weaponInfoName;

	public bool objectUsesStaminaOnAttacks = true;

	public bool hideWeaponMeshWhenNotUsed;

	public bool disableMeleeObjectCollider = true;

	[Space]
	[Header ("Block Settings")]
	[Space]

	public bool canUseBlock;
	public string blockActionName;
	public float blockDamageProtectionAmount = 1;
	public bool useMaxBlockRangeAngle;
	public float maxBlockRangeAngle = 360;
	public int blockID;
	public float reducedBlockDamageProtectionAmount = 0.1f;

	[Space]
	[Header ("Shield Settings")]
	[Space]

	public bool canUseShieldBlock;
	public string shieldBlockActionName;
	public int shieldBlockID;

	[Space]
	[Header ("Match Target Settings")]
	[Space]

	public bool useMatchPositionSystemOnAllAttacks;
	public float matchPositionOffsetOnAllAttacks = 1.6f;

	[Space]
	[Header ("Throw/Return Settings")]
	[Space]

	public bool holdToThrowObjectEnabled;

	public bool locateObjectstToTrackOnHoldEnabled;

	public LayerMask layerToLocateObjectsToTrack;

	public float maxRaycastDistanceToLocateObjectToTrack;

	[Space]

	public bool canThrowObject;
	public bool canReturnObject;
	public bool canReUseObjectIfNotReturnActive = true;

	public float throwSpeed;
	public float returnSpeed;

	public float returnSplineSpeed;
	public float resetObjectRotationSpeed;

	public float delayToThrowObject;
	public float delayToReturnObject;

	public float maxTimeOnAirIfNoSurfaceFound;

	public bool ignoreRigidbodiesOnThrowWeapon;

	public bool ignoreAttachToLocatedObjectOnThrow;

	public float followObjectOnThrowMeleeWeaponSpeed = 45;

	[Space]
	[Header ("Throw/Return Push Objects Settings")]
	[Space]

	public bool pushCharactersDetectedOnIgnoreRigidbodiesOnThrowWeapon;

	public float pushCharactersDetectedOnIgnoreRigidbodiesOnThrowWeaponMultiplier = 1;

	public string pushCharactersDetectedOnIgnoreRigidbodiesOnThrowWeaponMessageNameToSend = "pushCharacter";

	[Space]

	public bool pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeapon;

	public float pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeaponMultiplier = 1;

	public string pushCharactersDetectedOnIgnoreRigidbodiesOnReturnWeaponMessageNameToSend = "pushCharacter";

	[Space]
	[Header ("Other Throw/Return Weapon Settings")]
	[Space]

	public bool throwObjectWithRotation = true;

	public float throwObjectRotationSpeed = 500;

	public bool returnObjectWithRotation = true;

	public float returnObjectRotationSpeed = 500;

	public bool rotateObjectToThrowDirection;

	[Space]

	public bool useSplineForReturn = true;

	public bool canReturnWeaponIfNoSurfaceFound;

	public bool ignoreIfSurfaceFoundToReturnWeapon;

	public bool rotateWeaponToSurfaceLocatedWhenCloseEnough;

	public float attachToSurfaceAdjustSpeed = 5;

	public bool useCutOnThrowObject;

	public bool useCutOnReturnObject;

	public bool returnWeaponIfObjectDetected;

	[Space]
	[Header ("Throw/Return Action Settings")]
	[Space]

	public bool useThrowActionName;
	public string throwActionName;

	public bool useStartReturnActionName;
	public string startReturnActionName;

	public bool useEndReturnActionName;
	public string endReturnActionName;

	[Space]
	[Header ("Throw/Return Physics Settings")]
	[Space]

	public float forceToApplyToSurfaceFound;
	public float forceExtraToApplyOnVehiclesFound;

	public float capsuleCastRadius = 1;
	public float capsuleCastDistance = 2;

	[Space]
	[Header ("Throw/Return Damage Settings")]
	[Space]

	public float damageOnSurfaceDetectedOnThrow;

	public bool applyDamageOnSurfaceDetectedOnReturn;
	public float damageOnSurfaceDetectedOnReturn;

	public bool applyDamageOnObjectReturnPath = true;
	public float damageOnObjectReturnPath = 20;

	[Space]
	[Header ("Throw/Return Events Settings")]
	[Space]

	public bool useEventsOnThrowReturn;
	public UnityEvent eventOnThrowStart;
	public UnityEvent eventOnThrowEnd;
	public UnityEvent eventOnReturnStart;
	public UnityEvent eventOnReturnEnd;

	[Space]
	[Header ("Throw/Return Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnThrow;
	public List<string> remoteEventOnThrowNameList = new List<string> ();

	[Space]

	public bool useRemoteEventOnReturn;
	public List<string> remoteEventOnReturnNameList = new List<string> ();

	[Space]
	[Header ("Teleport Settings")]
	[Space]

	public bool teleportPlayerOnThrowEnabled = true;

	public bool throwWeaponQuicklyAndTeleportIfSurfaceFound;
	public float extraSpeedOnThrowWeaponQuicklyAndTeleport = 6;

	[Space]
	[Header ("Trail Renderer Settings")]
	[Space]

	public float trailSpeed = 10;
	public bool useTrailOnThrowReturn;

	[Space]
	[Header ("Keep/Draw Weapon Events Settings")]
	[Space]

	public bool useEventsOnKeepOrDrawMeleeWeapon;

	public UnityEvent eventsOnKeepMeleeWeapon;
	public UnityEvent eventsOnDrawMeleeWeapon;

	[Space]
	[Header ("Cutting Mode Settings")]
	[Space]

	public bool canActivateCuttingMode;

	public Vector3 cutOverlapBoxSize;
	public Transform cutPositionTransform;
	public Transform cutDirectionTransform;

	public Transform planeDefiner1;
	public Transform planeDefiner2;
	public Transform planeDefiner3;

	[Space]
	[Header ("Attack List Settings")]
	[Space]

	public bool attacksEnabled = true;
	public bool useAnimationPercentageDuration;
	public bool useAnimationPercentageOver100;

	public bool useGeneralDamageTypeID;

	public int generalDamageTypeID = -1;

	[Space]
	[Space]

	public List<attackInfo> attackInfoList = new List<attackInfo> ();

	[Space]
	[Space]

	[Space]
	[Header ("Dual Wield Settings")]
	[Space]

	public bool isDualWieldWeapon;
	public dualWieldMeleeWeaponObjectSystem mainDualWieldMeleeWeaponObjectSystem;

	[Space]
	[Header ("Melee Attack Info Template Settings")]
	[Space]

	public bool useMeleeWeaponAttackInfoTemplate;

	public meleeWeaponAttackInfo mainMeleeWeaponAttackInfoTemplate;

	[Space]
	[Header ("Damage Detection Settings")]
	[Space]

	public List<Transform> raycastCheckTransfromList = new List<Transform> ();

	[Space]
	[Header ("Damage Type And Reaction Settings")]
	[Space]

	public List<damageTypeAndReactionInfo> damageTypeAndReactionInfoList = new List<damageTypeAndReactionInfo> ();

	[Space]
	[Header ("Melee Weapon Mesh Settings")]
	[Space]

	public GameObject weaponMesh;

	public bool useCustomWeaponMeshToInstantiate;

	public GameObject customWeaponMeshToInstantiate;

	[Space]
	[Header ("Deflect Projectiles Settings")]
	[Space]

	public bool deflectProjectilesEnabled;
	public armorSurfaceSystem mainArmoreSurfaceSystem;
	public bool deflectProjectilesOnBlockEnabled;
	public bool useDurationToDeflectProjectilesOnBlock;
	public float durationToDeflecltProjectilesOnBlock;

	[Space]

	public bool adjustTriggerScaleValuesEnabled;

	public Vector3 deflectProjectilesTriggerCenter;
	public Vector3 deflectProjectilesTriggerSize;

	public Vector3 deflectProjectilesOnBlockTriggerCenter;
	public Vector3 deflectProjectilesOnBlockTriggerSize;

	[Space]
	[Header ("Draw/Keep Weapon Settings Settings")]
	[Space]

	public bool useDrawKeepWeaponAnimation;
	public string drawWeaponActionName;
	public string keepWeaponActionName;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnSurfacesDetected;
	public List<remoteEventOnSurfaceDetectedInfo> remoteEventNameListOnSurfaceDetected = new List<remoteEventOnSurfaceDetectedInfo> ();

	[Space]
	[Header ("Enable/Disable Abilities on Weapon Settings")]
	[Space]

	public bool useAbilitiesListToEnableOnWeapon;
	public List<string> abilitiesListToEnableOnWeapon = new List<string> ();

	[Space]

	public bool useAbilitiesListToDisableOnWeapon;
	public List<string> abilitiesListToDisableOnWeapon = new List<string> ();

	[Space]
	[Header ("Extra Actions List Settings")]
	[Space]

	public bool useExtraActions;

	public int currentExtraActionIndex;

	[Space]
	[Space]

	public List<extraActionMeleeWeaponInfo> extraActionMeleeWeaponInfoList = new List<extraActionMeleeWeaponInfo> ();

	[Space]
	[Header ("Durability Settings")]
	[Space]

	public bool useObjectDurabilityOnAttack;
	public float durabilityUsedOnAttack = 1;

	[Space]

	public bool useObjectDurabilityOnBlock;

	public float durabilityUsedOnBlock = 1;

	public durabilityInfo mainDurabilityInfo;

	[Space]
	[Header ("Object Stats Settings")]
	[Space]

	public bool useObjectStatsSystemEnabled;

	public objectStatsSystem mainObjectStatsSystem;


	[Space]
	[Header ("Debug")]
	[Space]

	public bool keepGrabbedObjectState;
	public bool objectThrown;

	public bool previousStrafeMode;

	public bool secondaryAbilityActive;

	public GameObject currentCharacter;

	[Space]
	[Header ("Components")]
	[Space]

	public hitCombat mainHitCombat;

	public Collider mainObjectCollider;

	public Transform objectRotationPoint;
	public Transform raycastCheckTransfrom;
	public Transform objectRotationPointParent;

	public Transform raycastCheckTransformParent;

	public TrailRenderer mainTraileRenderer;

	public Transform attachToCharactersReferenceTransform;

	public Transform mainDamagePositionTransform;

	public Transform referencePositionToKeepObjectMesh;

	public grabPhysicalObjectSystem mainGrabPhysicalObjectSystem;


	[HideInInspector] public bool useCustomReferencePositionToKeepObjectMesh;

	[HideInInspector] public Transform customReferencePositionToKeepObjectMesh;

	Coroutine trailCoroutine;

	Coroutine damageTypeInfoCoroutine;


	public void setRegularDeflectProjectilesTriggerScaleValues ()
	{
		if (adjustTriggerScaleValuesEnabled) {
			if (mainArmoreSurfaceSystem != null) {
				mainArmoreSurfaceSystem.setTriggerScaleValues (deflectProjectilesTriggerCenter, deflectProjectilesTriggerSize);
			}
		}
	}

	public void setBlockDeflectProjectilesTriggerScaleValues ()
	{
		if (adjustTriggerScaleValuesEnabled) {
			if (mainArmoreSurfaceSystem != null) {
				mainArmoreSurfaceSystem.setTriggerScaleValues (deflectProjectilesOnBlockTriggerCenter, deflectProjectilesOnBlockTriggerSize);
			}
		}
	}

	public void setMainObjectColliderEnabledState (bool state)
	{
		if (mainObjectCollider != null) {
			mainObjectCollider.enabled = state;
		}
	}

	public int getAttackListCount ()
	{
		return attackInfoList.Count;
	}

	public hitCombat getMainHitCombat ()
	{
		return mainHitCombat;
	}

	public void setKeepOrCarryGrabbebObjectState (bool state)
	{
		keepGrabbedObjectState = state;
	}

	public bool iskeepGrabbedObjectStateActive ()
	{
		return keepGrabbedObjectState;
	}

	public void setObjectThrownState (bool state)
	{
		objectThrown = state;
	}

	public bool isObjectThrown ()
	{
		return objectThrown;
	}

	public void checkEventOnThrow (bool state)
	{
		if (useEventsOnThrowReturn) {
			if (state) {
				eventOnThrowStart.Invoke ();
			} else {
				eventOnThrowEnd.Invoke ();
			}
		}

		if (useTrailOnThrowReturn) {
			setTrailActiveState (state);
		}
	}

	public void checkEventOnReturn (bool state)
	{
		if (useEventsOnThrowReturn) {
			if (state) {
				eventOnReturnStart.Invoke ();
			} else {
				eventOnReturnEnd.Invoke ();
			}
		}

		if (useTrailOnThrowReturn) {
			setTrailActiveState (state);
		}
	}

	public void checkEventWhenKeepingOrDrawingMeleeWeapon (bool state)
	{
		if (useEventsOnKeepOrDrawMeleeWeapon) {
			if (state) {
				eventsOnKeepMeleeWeapon.Invoke ();
			} else {
				eventsOnDrawMeleeWeapon.Invoke ();
			}
		}
	}

	public void checkDisableTrail ()
	{
		if (useTrailOnThrowReturn) {
			setTrailActiveState (false);
		}
	}

	public void setTrailActiveState (bool state)
	{
		stopDisableTrailCoroutine ();

		if (state) {
			mainTraileRenderer.enabled = true;
			mainTraileRenderer.time = 1;
		} else {
			if (enabled) {
				mainTraileRenderer.enabled = false;
			} else {
				trailCoroutine = StartCoroutine (disableTrailCoroutine ());
			}
		}
	}

	void stopDisableTrailCoroutine ()
	{
		if (trailCoroutine != null) {
			StopCoroutine (trailCoroutine);
		}
	}

	IEnumerator disableTrailCoroutine ()
	{
		bool targetReached = false;

		float t = 0;

		while (!targetReached) {
			t += Time.deltaTime / trailSpeed; 
			mainTraileRenderer.time -= t;

			if (mainTraileRenderer.time <= 0) {
				targetReached = true;
			}

			yield return null;
		}

		mainTraileRenderer.enabled = false;
	}

	public void setPreviousStrafeModeState (bool state)
	{
		previousStrafeMode = state;
	}

	public bool wasStrafeModeActivePreviously ()
	{
		return previousStrafeMode;
	}

	public grabPhysicalObjectSystem getGrabPhysicalObjectMeleeAttackSystem ()
	{
		return mainGrabPhysicalObjectSystem;
	}

	public override bool isMeleeWeapon ()
	{
		return true;
	}

	public override string getWeaponName ()
	{
		return weaponName;
	}

	public void enableOrDisableRemoteEventsOnSurfacesDetected (bool state)
	{
		useRemoteEventOnSurfacesDetected = state;
	}

	public void enableOrDisableRemoteEventsOnSurfacesDetectedElement (bool state, string remoteEventName)
	{
		for (int i = 0; i < remoteEventNameListOnSurfaceDetected.Count; i++) {
			if (remoteEventNameListOnSurfaceDetected [i].remoteEventName.Equals (remoteEventName)) {
				remoteEventNameListOnSurfaceDetected [i].remoteEventActive = state;

				return;
			}
		}
	}

	public void enableRemoteEventsOnSurfacesDetectedElement (string remoteEventName)
	{
		enableOrDisableRemoteEventsOnSurfacesDetectedElement (true, remoteEventName);
	}

	public void disableRemoteEventsOnSurfacesDetectedElement (string remoteEventName)
	{
		enableOrDisableRemoteEventsOnSurfacesDetectedElement (false, remoteEventName);
	}

	public bool isRemoteEventIncluded (string remoteEventName)
	{
		for (int i = 0; i < remoteEventNameListOnSurfaceDetected.Count; i++) {
			if (remoteEventNameListOnSurfaceDetected [i].remoteEventActive &&
			    remoteEventNameListOnSurfaceDetected [i].remoteEventName.Equals (remoteEventName)) {

				return true;
			}
		}

		return false;
	}

	public void enableOrDisableRemoteEventsOnAttackList (bool state)
	{
		for (int i = 0; i < attackInfoList.Count; i++) {
			attackInfoList [i].useRemoteEvent = state;
		}
	}

	public void setSecondaryAbilityActiveState (bool state)
	{
		secondaryAbilityActive = state;
	}

	public bool isSecondaryAbilityActive ()
	{
		return secondaryAbilityActive;
	}

	public void setNewGeneralDamageTypeID (int newValue)
	{
		generalDamageTypeID = newValue;
	}

	public bool setDamageTypeAndReactionInfo (string newBuffObjectName)
	{
		int damageIndex = damageTypeAndReactionInfoList.FindIndex (s => s.buffObjectName.Equals (newBuffObjectName));

		if (damageIndex > -1) {
			for (int i = 0; i < damageTypeAndReactionInfoList.Count; i++) {
				if (damageIndex != i && damageTypeAndReactionInfoList [i].effectCurrentlyActive) {
					damageTypeAndReactionInfoList [i].effectCurrentlyActive = false;

					if (damageTypeAndReactionInfoList [i].useEventToDeactivateDamageType) {
						damageTypeAndReactionInfoList [i].eventToDeactivateDamageType.Invoke ();
					}
				}
			}

			damageTypeAndReactionInfo currentDamageTypeAndReactionInfo = damageTypeAndReactionInfoList [damageIndex];

			currentDamageTypeAndReactionInfo.effectCurrentlyActive = true;

			setNewGeneralDamageTypeID (currentDamageTypeAndReactionInfo.damageTypeID);

			if (currentDamageTypeAndReactionInfo.useEventToActivateDamageType) {
				currentDamageTypeAndReactionInfo.eventToActivateDamageType.Invoke ();
			}

			if (currentDamageTypeAndReactionInfo.useDamageTypeDuration) {
				stopDamageTypeAndReactionInfoCoroutine ();

				damageTypeInfoCoroutine = StartCoroutine (damageTypeAndReactionInfoCoroutine (currentDamageTypeAndReactionInfo));
			}

			return true;
		}

		return false;
	}

	void stopDamageTypeAndReactionInfoCoroutine ()
	{
		if (damageTypeInfoCoroutine != null) {
			StopCoroutine (damageTypeInfoCoroutine);
		}
	}

	IEnumerator damageTypeAndReactionInfoCoroutine (damageTypeAndReactionInfo currentDamageTypeAndReactionInfo)
	{
		yield return new WaitForSeconds (currentDamageTypeAndReactionInfo.damageTypeDuration);

		currentDamageTypeAndReactionInfo.effectCurrentlyActive = false;

		if (currentDamageTypeAndReactionInfo.useEventToDeactivateDamageType) {
			currentDamageTypeAndReactionInfo.eventToDeactivateDamageType.Invoke ();
		}
	}

	public void activateReturnProjectilesOnContact ()
	{
		if (deflectProjectilesEnabled) {
			mainGrabPhysicalObjectSystem.activateReturnProjectilesOnContact ();
		}
	}

	public void sendReturnProjectilesOnContactInfo (Transform playerTransform, Transform mainCameraTransform)
	{
		if (deflectProjectilesEnabled) {
			mainArmoreSurfaceSystem.setNewArmorOwner (playerTransform.gameObject);

			mainArmoreSurfaceSystem.throwProjectilesStoredCheckingDirection (playerTransform, mainCameraTransform);
		}
	}

	public void enableOrDisableWeaponMeshActiveState (bool state)
	{
		weaponMesh.SetActive (state);
	}

	public void setNextExtraActionOnMeleeWeapon (remoteEventSystem currentRemoteEventSystem)
	{
		if (useExtraActions) {
			
			bool nextActionFound = false;

			int loopCount = 0;

			int actionsCount = extraActionMeleeWeaponInfoList.Count;

			int newIndex = currentExtraActionIndex;

			while (!nextActionFound) {

				newIndex++;

				if (newIndex >= extraActionMeleeWeaponInfoList.Count) {
					newIndex = 0;
				}

				if (extraActionMeleeWeaponInfoList [newIndex].actionEnabled) {
					nextActionFound = true;
				}

				loopCount++;

				if (loopCount > actionsCount * 3) {
					nextActionFound = true;
				}
			}

			setNewExtraActionByIndex (newIndex, currentRemoteEventSystem);
		}
	}

	public void setNewExtraActionByIndex (int newIndex, remoteEventSystem currentRemoteEventSystem)
	{
		if (useExtraActions) {
			if (newIndex == currentExtraActionIndex) {
				return;
			}

			extraActionMeleeWeaponInfo currentExtraActionMeleeWeaponInfo = extraActionMeleeWeaponInfoList [currentExtraActionIndex];

			currentExtraActionMeleeWeaponInfo.isCurrentAction = false;

			if (currentExtraActionMeleeWeaponInfo.useRemoteEventToDisableAction) {
				currentRemoteEventSystem.callRemoteEvent (currentExtraActionMeleeWeaponInfo.remoteEventToDisableAction);
			}

			currentExtraActionIndex = newIndex;

			currentExtraActionMeleeWeaponInfo = extraActionMeleeWeaponInfoList [currentExtraActionIndex];

			currentExtraActionMeleeWeaponInfo.isCurrentAction = true;

			if (currentExtraActionMeleeWeaponInfo.useRemoteEventToEnableAction) {
				currentRemoteEventSystem.callRemoteEvent (currentExtraActionMeleeWeaponInfo.remoteEventToEnableAction);
			}
		}
	}

	public Texture getActionIconTexture ()
	{
		if (useExtraActions) {
			if (extraActionMeleeWeaponInfoList [currentExtraActionIndex].useActionIcon) {
				return extraActionMeleeWeaponInfoList [currentExtraActionIndex].actionIcon;
			}
		}

		return null;
	}

	public void activateCurrentExtraActionOnMeleeWeapon (remoteEventSystem currentRemoteEventSystem)
	{
		if (useExtraActions) {
			if (extraActionMeleeWeaponInfoList [currentExtraActionIndex].remoteEventToActivateAction != "") {
				currentRemoteEventSystem.callRemoteEvent (extraActionMeleeWeaponInfoList [currentExtraActionIndex].remoteEventToActivateAction);
			}

			if (extraActionMeleeWeaponInfoList [currentExtraActionIndex].useEventOnActivateAction) {
				extraActionMeleeWeaponInfoList [currentExtraActionIndex].eventOnActivateAction.Invoke ();
			}
		}
	}

	public bool checkIfCarryingMeleeWeaponToActivateAction ()
	{
		if (useExtraActions) {
			return extraActionMeleeWeaponInfoList [currentExtraActionIndex].checkIfCarryingMeleeWeaponToActivateAction;
		}

		return false;
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
		int newIndex = extraActionMeleeWeaponInfoList.FindIndex (s => s.Name.Equals (actionName));

		if (newIndex > -1) {
			extraActionMeleeWeaponInfo currentExtraActionMeleeWeaponInfo = extraActionMeleeWeaponInfoList [newIndex];

			currentExtraActionMeleeWeaponInfo.actionEnabled = state;
		}
	}

	public bool checkDurabilityOnAttack (float extraMultiplier)
	{
		if (useObjectDurabilityOnAttack) {
			if (extraMultiplier != 0) {
				mainDurabilityInfo.addOrRemoveDurabilityAmountToObjectByName (-durabilityUsedOnAttack * extraMultiplier, false);
			} else {
				mainDurabilityInfo.addOrRemoveDurabilityAmountToObjectByName (-durabilityUsedOnAttack, false);
			}

			if (mainDurabilityInfo.durabilityAmount <= 0) {
				return true;
			}
		}

		return false;
	}

	public bool checkDurabilityOnBlock (float extraMultiplier)
	{
		if (useObjectDurabilityOnBlock) {
			mainDurabilityInfo.addOrRemoveDurabilityAmountToObjectByName (-durabilityUsedOnBlock * extraMultiplier, false);

			if (mainDurabilityInfo.durabilityAmount <= 0) {
				return true;
			}
		}

		return false;
	}

	public void updateDurabilityAmountState ()
	{
		if (useObjectDurabilityOnAttack || useObjectDurabilityOnBlock) {
			mainDurabilityInfo.updateDurabilityAmountState ();
		}
	}

	public float getDurabilityAmount ()
	{
		if (useObjectDurabilityOnAttack || useObjectDurabilityOnBlock) {
			return mainDurabilityInfo.getDurabilityAmount ();
		}

		return -1;
	}

	public void initializeDurabilityValue (float newAmount)
	{
		if (useObjectDurabilityOnAttack || useObjectDurabilityOnBlock) {
			mainDurabilityInfo.initializeDurabilityValue (newAmount);
		}
	}

	public void repairObjectFully ()
	{
		if (useObjectDurabilityOnAttack || useObjectDurabilityOnBlock) {
			mainDurabilityInfo.repairObjectFully ();
		}
	}

	public void breakFullDurabilityOnCurrentWeapon ()
	{
		if (useObjectDurabilityOnAttack || useObjectDurabilityOnBlock) {
			mainDurabilityInfo.breakFullDurability ();
		}
	}

	public void addObjectStatsToMainManager ()
	{
		if (useObjectStatsSystemEnabled) {
			mainObjectStatsSystem.addObjectStatsToMainManager ();
		}
	}

	public void setCurrentCharacter (GameObject newObject)
	{
		currentCharacter = newObject;
	}

	public GameObject getCurrentCharacter ()
	{
		return currentCharacter;
	}

	public bool isWeaponCarriedByCharacter ()
	{
		return currentCharacter != null;
	}

	float lastTimeWeaponDropped;

	public float getLastTimeWeaponDropped ()
	{
		return lastTimeWeaponDropped;
	}

	public void setLastTimeWeaponDropped ()
	{
		lastTimeWeaponDropped = Time.time;
	}

	//EDITOR FUNCTIONS
	public void copyWeaponInfoToTemplate (bool settingInfoOnEditorTime)
	{
		if (mainMeleeWeaponAttackInfoTemplate != null) {
			if (attackInfoList.Count > 0) {
				mainMeleeWeaponAttackInfoTemplate.Name = weaponName;

				mainMeleeWeaponAttackInfoTemplate.mainMeleeWeaponInfo.mainAttackInfoList = attackInfoList;

				if (settingInfoOnEditorTime) {
					updateComponent ();
				}
			}
		}
	}

	public void copyTemplateToWeaponAttackInfo (bool settingInfoOnEditorTime)
	{
		if (mainMeleeWeaponAttackInfoTemplate != null) {
			if (mainMeleeWeaponAttackInfoTemplate.mainMeleeWeaponInfo.mainAttackInfoList.Count > 0) {
				attackInfoList = mainMeleeWeaponAttackInfoTemplate.mainMeleeWeaponInfo.mainAttackInfoList;

				if (settingInfoOnEditorTime) {
					updateComponent ();
				}
			}
		}
	}

	public void setNewMeleeWeaponAttackInfoTemplate (meleeWeaponAttackInfo newMeleeWeaponAttackInfo, bool settingInfoOnEditorTime)
	{
		mainMeleeWeaponAttackInfoTemplate = newMeleeWeaponAttackInfo;

		if (settingInfoOnEditorTime) {
			updateComponent ();
		}
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("updating melee weapon state " + weaponName, gameObject);
	}

	[System.Serializable]
	public class attackInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;
		public float attackDamage;
		public string attackType;

		public float minDelayBeforeNextAttack;

		public int damageReactionID = -1;

		public int damageTypeID = -1;

		public bool playerOnGroundToActivateAttack = true;

		public bool pauseSoundIfAttackOnAir;

		[Space]
		[Header ("Damage Trigger Settings")]
		[Space]

		public List<damageTriggerActiveInfo> damageTriggerActiveInfoList = new List<damageTriggerActiveInfo> ();

		[Space]
		[Header ("Dual Wield Damage Trigger Settings")]
		[Space]

		public List<damageTriggerActiveInfo> dualWieldDamageTriggerActiveInfoList = new List<damageTriggerActiveInfo> ();

		[Space]
		[Header ("Damage On Air Trigger Settings")]
		[Space]

		public List<damageTriggerActiveInfo> attackOnAirDamageTriggerActiveInfoList = new List<damageTriggerActiveInfo> ();

		[Space]

		public bool useSingleSlashDamageInfo;

		public float delayToActivateSingleSlashDamageTrigger;
		public float delayToDeactivateSingleSlashDamageTrigger;

		public bool ignoreDetectedObjectsOnList;

		[Space]
		[Header ("Action System Settings")]
		[Space]
	
		public bool useCustomAction;
		public string customActionName;

		public float attackDuration;

		public float attackAnimationSpeed = 1;

		[Space]
		[Space]

		public float attackDurationOnAir;
		public float attackAnimationSpeedOnAir = 1;

		[Space]
		[Header ("Stamina Settings")]
		[Space]

		public float staminaUsedOnAttack;
		public float customRefillStaminaDelayAfterUse;

		[Space]
		[Header ("Remote Events Settings")]
		[Space]

		public bool useRemoteEvent;
		public List<string> remoteEventNameList = new List<string> ();

		[Space]
		[Header ("Cut Mode Settings")]
		[Space]

		public bool useCutModeOnAttack;

		[Space]
		[Header ("Push Character Settings")]
		[Space]

		public bool pushCharactersOnDamage;
		public string messageNameToSend = "pushCharacter";
		public float pushForceAmount = 4;
		public bool useProbabilityToPushCharacters;
		[Range (0, 1)]public float probability = 1;
		public bool useExtraPushDirection;
		public Vector3 extraPushDirection;
		public float extraPushForce;

		public bool ignoreMeleeWeaponAttackDirection;

		public bool useIgnoreTagsToPush;
		public List<string> tagsToIgnorePush = new List<string> ();

		[Space]
		[Header ("Match Attack Position Settings")]
		[Space]

		public bool useMatchPositionSystem = true;
		public float matchPositionOffset = 1.6f;

		[Space]
		[Header ("Obtain Health From Damage Applied Settings")]
		[Space]

		public bool getHealthFromDamagingObjects;
		public float healthFromDamagingObjectsMultiplier = 1;
	}

	[System.Serializable]
	public class damageTriggerActiveInfo
	{
		[Header ("Damage Delay Settings")]
		[Space]

		public float delayToActiveTrigger;
		public bool activateDamageTrigger = true;
		[HideInInspector] public bool delayTriggered;

		[Space]
		[Header ("Damage Trigger Settings")]
		[Space]

		public bool setNewTriggerScale;
		public Vector3 newTriggerScale;
		public Vector3 newTriggerOffset;
		public bool setOriginalScale = true;

		public bool checkSurfacesWithCapsuleRaycast;
		public float checkSurfaceCapsuleRaycastRadius;

		public bool changeDamageTriggerToLimb;
		public HumanBodyBones limbToPlaceTrigger;
		public bool placeTriggerInFrontOfCharacter;

		[Space]
		[Header ("Events Settings")]
		[Space]

		public bool useEventOnAttack;
		public UnityEvent eventOnAtatck;

		[HideInInspector] public float calculatedPercentageAttackDuration;

		[Space]
		[Header ("Range Attack Settings")]
		[Space]

		public bool useRangeAttackID;
		public int rangeAttackID;
		public bool disableRangeAttackID;
		public int rangeAttackIDToDisable;

		public damageTriggerActiveInfo ()
		{

		}

		public damageTriggerActiveInfo (damageTriggerActiveInfo newInfo)
		{
			delayToActiveTrigger = newInfo.delayToActiveTrigger;
			activateDamageTrigger = newInfo.activateDamageTrigger;
		
			setNewTriggerScale = newInfo.setNewTriggerScale;
			newTriggerScale = newInfo.newTriggerScale;
			newTriggerOffset = newInfo.newTriggerOffset;
			setOriginalScale = newInfo.setOriginalScale;

			checkSurfacesWithCapsuleRaycast = newInfo.checkSurfacesWithCapsuleRaycast;
			checkSurfaceCapsuleRaycastRadius = newInfo.checkSurfaceCapsuleRaycastRadius;

			changeDamageTriggerToLimb = newInfo.changeDamageTriggerToLimb;
			limbToPlaceTrigger = newInfo.limbToPlaceTrigger;
			placeTriggerInFrontOfCharacter = newInfo.placeTriggerInFrontOfCharacter;

			useRangeAttackID = newInfo.useRangeAttackID;
			rangeAttackID = newInfo.rangeAttackID;
			disableRangeAttackID = newInfo.disableRangeAttackID;
			rangeAttackIDToDisable = newInfo.rangeAttackIDToDisable;
		}
	}

	[System.Serializable]
	public class damageTypeAndReactionInfo
	{
		public string Name;

		public string buffObjectName;

		public bool effectCurrentlyActive;

		public int damageTypeID = -1;

		public bool useDamageTypeDuration;
		public float damageTypeDuration;

		[Space]
		[Header ("Event Settings")]
		[Space]

		public bool useEventToActivateDamageType;
		public UnityEvent eventToActivateDamageType;

		public bool useEventToDeactivateDamageType;
		public UnityEvent eventToDeactivateDamageType;
	}

	[System.Serializable]
	public class remoteEventOnSurfaceDetectedInfo
	{
		public string remoteEventName;
		public bool remoteEventActive;
	}

	[System.Serializable]
	public class extraActionMeleeWeaponInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;
		public bool actionEnabled = true;

		public bool isCurrentAction;

		[Space]
		[Header ("Icon Settings")]
		[Space]

		public bool useActionIcon;
		public Texture actionIcon;

		[Space]
		[Header ("Event Settings")]
		[Space]

		public bool useRemoteEventToEnableAction;
		public string remoteEventToEnableAction;

		[Space]

		public bool useRemoteEventToDisableAction;
		public string remoteEventToDisableAction;

		[Space]

		public bool checkIfCarryingMeleeWeaponToActivateAction;
		public string remoteEventToActivateAction;

		[Space]

		public bool useEventOnActivateAction;
		public UnityEvent eventOnActivateAction;
	}
}
