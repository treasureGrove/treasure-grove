using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class actionSystem : MonoBehaviour
{
	public bool useMinDistanceToActivateAction;
	public float minDistanceToActivateAction;

	public bool useMinAngleToActivateAction;
	public float minAngleToActivateAction;
	public bool checkOppositeAngle;

	public bool canStopPreviousAction;

	public bool canForceToPlayCustomAction;

	public int currentActionInfoIndex;

	public List<actionInfo> actionInfoList = new List<actionInfo> ();

	public List<GameObject> playerList = new List<GameObject> ();

	public bool useEventsOnPlayerInsideOutside;

	public UnityEvent eventOnPlayerInside;

	public UnityEvent eventOnPlayerOutside;

	public bool useEventsOnStartEndAction;

	public UnityEvent eventOnStartAction;
	public UnityEvent eventOnEndAction;

	public bool useEventAfterResumePlayer;
	public UnityEvent eventAfterResumePlayer;

	public Transform actionTransform;

	public bool sendCurrentPlayerOnEvent;
	public eventParameters.eventToCallWithGameObject eventToSendCurrentPlayer;

	public Collider mainTrigger;

	public bool activateCustomActionAfterActionComplete;
	public string customActionToActiveAfterActionComplete;

	public bool addActionToListStoredToPlay;
	public bool playActionAutomaticallyIfStoredAtEnd;

	public bool useEventsToEnableDisableActionObject;
	public UnityEvent eventToEnableActionObject;
	public UnityEvent eventToDisableActionObject;

	public string categoryName;

	public bool canInterruptOtherActionActive;
	public List<string> actionListToInterrupt = new List<string> ();
	public bool useCategoryToCheckInterrupt;
	public List<string> actionCategoryListToInterrupt;
	public UnityEvent eventOnInterrupOtherActionActive;

	public bool useEventOnInterruptedAction;
	public UnityEvent eventOnInterruptedAction;

	public bool useProbabilityToActivateAction;
	public float probablityToActivateAction;

	public bool animationUsedOnUpperBody;
	public bool disableRegularActionActiveState;
	public bool disableRegularActionActiveStateOnEnd;

	public bool changeCameraViewToThirdPersonOnAction;

	public bool actionsCanBeUsedOnFirstPerson;
	public bool ignoreChangeToThirdPerson;



	public bool showDebugPrint;

	public bool showGizmo;
	public bool showAllGizmo;

	bool ignorePlayerList;


	public Animator mainAnimator;
	public AnimationClip newAnimationClip;

	public string actionNameToReplace;

	public string animationLayerName = "Base Layer";

	public float newAnimationSpeed = 1;

	public bool activateAnimationReplace;

	public bool newAnimationIsMirror;

	int numberOfLoops;

	public bool setPlayerParentDuringActionActive;
	public Transform playerParentDuringAction;

	public void setPlayerParentDuringActionActiveValues (bool state, Transform newParent)
	{
		setPlayerParentDuringActionActive = state;

		playerParentDuringAction = newParent;
	}

	public void setUseMovingPlayerToPositionTargetValues (bool state, float newSpeed, float newDelay)
	{
		if (actionInfoList.Count > 0) {
			actionInfo currentactionInfo = actionInfoList [0];

			currentactionInfo.useMovingPlayerToPositionTarget = state;

			if (state) {
				currentactionInfo.movingPlayerToPositionTargetSpeed = newSpeed;
				currentactionInfo.movingPlayerToPositionTargetDelay = newDelay;
			}
		}
	}

	public void activateCustomAction (GameObject playerDetected)
	{
		ignorePlayerList = true;

		setPlayerActionActive (playerDetected);
	}

	public void setPlayerActionActive (GameObject playerDetected)
	{
		if (ignorePlayerList || !playerList.Contains (playerDetected)) {
			if (!ignorePlayerList) {
				if (useEventsOnPlayerInsideOutside) {
					if (playerList.Count == 0) {
						eventOnPlayerInside.Invoke ();
					}
				}

				playerList.Add (playerDetected);
			}

			playerComponentsManager currentPlayerComponentsManager = playerDetected.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {

				playerActionSystem currentPlayerActionSystem = currentPlayerComponentsManager.getPlayerActionSystem ();

				if (actionTransform == null) {
					actionTransform = transform;
				}

				if (currentPlayerActionSystem != null) {
					if (showDebugPrint) {
						print ("Sending action to player action system " + actionInfoList [0].Name);
					}

					currentPlayerActionSystem.setPlayerActionActive (this);
				}
			}
		}
	}

	public void setPlayerActionDeactivate (GameObject playerDetected)
	{
		if (ignorePlayerList || playerList.Contains (playerDetected)) {
			if (!ignorePlayerList) {
				playerList.Remove (playerDetected);
			}

			playerComponentsManager currentPlayerComponentsManager = playerDetected.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {

				playerActionSystem currentPlayerActionSystem = currentPlayerComponentsManager.getPlayerActionSystem ();

				if (currentPlayerActionSystem != null) {
					currentPlayerActionSystem.setPlayerActionDeactivate (this);
				}
			}

			if (!ignorePlayerList) {
				if (useEventsOnPlayerInsideOutside) {
					if (playerList.Count == 0) {
						eventOnPlayerOutside.Invoke ();
					}
				}
			}
		}
	}

	public void checkSendCurrentPlayerOnEvent (GameObject currentPlayer)
	{
		if (sendCurrentPlayerOnEvent) {
			eventToSendCurrentPlayer.Invoke (currentPlayer);
		}
	}

	public void setCustomActionTransform (Transform newTransform)
	{
		actionTransform = newTransform;
	}

	public actionInfo getCurrentactionInfo ()
	{
		return actionInfoList [currentActionInfoIndex];
	}

	public void increaseCurrentActionInfoIndex ()
	{
		currentActionInfoIndex++;

		if (currentActionInfoIndex >= actionInfoList.Count - 1) {
			currentActionInfoIndex = actionInfoList.Count - 1;
		}
	}

	public void decreaseCurrentActionInfoIndex ()
	{
		currentActionInfoIndex--;

		if (currentActionInfoIndex < 0) {
			currentActionInfoIndex = 0;
		}
	}

	public void setCurrentActionInfoIndex (int newIndex)
	{
		currentActionInfoIndex = newIndex;

		if (currentActionInfoIndex >= actionInfoList.Count - 1) {
			currentActionInfoIndex = actionInfoList.Count - 1;
		}

		if (currentActionInfoIndex < 0) {
			currentActionInfoIndex = 0;
		}
	}

	public void resetCurrentActionInfoIndex ()
	{
		currentActionInfoIndex = 0;
	}

	public int getCurrentActionInfoIndex ()
	{
		return currentActionInfoIndex;
	}

	public void destroyAction ()
	{
		for (int i = 0; i < actionInfoList.Count; i++) {
			if (actionInfoList [i].setObjectParent) {
				Destroy (actionInfoList [i].objectToSetParent.gameObject);
			}
		}

		Destroy (gameObject);
	}

	public void checkEventOnStartAction ()
	{
		if (useEventsOnStartEndAction) {
			eventOnStartAction.Invoke ();
		}
	}

	public void checkEventOnEndAction ()
	{
		if (useEventsOnStartEndAction) {
			eventOnEndAction.Invoke ();
		}
	}

	public void checkEventAfterResumePlayer ()
	{
		if (useEventAfterResumePlayer) {
			eventAfterResumePlayer.Invoke ();
		}
	}

	public void checkEventOnEnableActionObject ()
	{
		if (useEventsToEnableDisableActionObject) {
			eventToEnableActionObject.Invoke ();
		}
	}

	public void checkEventOnDisableActionObject ()
	{
		if (useEventsToEnableDisableActionObject) {
			eventToDisableActionObject.Invoke ();
		}
	}

	public void clearPlayerList ()
	{
		playerList.Clear ();
	}

	public void removePlayerFromList (GameObject playerToRemove)
	{
		if (playerList.Contains (playerToRemove)) {
			playerList.Remove (playerToRemove);
		}
	}

	public void disableAction ()
	{
		for (int i = 0; i < playerList.Count; i++) {
			usingDevicesSystem currentUsingDevicesSystem = playerList [i].GetComponent<usingDevicesSystem> ();

			if (currentUsingDevicesSystem != null) {
				currentUsingDevicesSystem.removeDeviceFromList (gameObject);
			}

			setPlayerActionDeactivate (playerList [i]);
		}

		if (mainTrigger == null) {
			mainTrigger = GetComponent<Collider> ();
		}

		if (mainTrigger != null) {
			mainTrigger.enabled = false;
		}
	}

	public void inputPlayCurrentAnimation ()
	{
		inputPlayCurrentAnimation (true);
	}

	public void inputPlayCurrentAnimationWithoutCheckingIfExistsOnaDeviceList ()
	{
		inputPlayCurrentAnimation (false);
	}

	void inputPlayCurrentAnimation (bool checkIfObjectOnDeviceList)
	{
		for (int i = 0; i < playerList.Count; i++) {
			playerComponentsManager currentPlayerComponentsManager = playerList [i].GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {

				playerActionSystem currentPlayerActionSystem = currentPlayerComponentsManager.getPlayerActionSystem ();

				if (currentPlayerActionSystem != null) {
					usingDevicesSystem currentUsingDevicesSystem = currentPlayerComponentsManager.getUsingDevicesSystem ();

					currentUsingDevicesSystem.updateClosestDeviceList ();

					if (currentPlayerActionSystem.getCurrentActionSystem () == this) {
						if (checkIfObjectOnDeviceList) {
							currentPlayerActionSystem.inputPlayCurrentAnimation ();
						} else {
							currentPlayerActionSystem.inputPlayCurrentAnimationWithoutCheckingIfExistsOnaDeviceList ();
						}
					}
				}
			}
		}
	}

	public void addNewAction ()
	{
		actionInfo newActionInfo = new actionInfo ();

		newActionInfo.Name = "New Action";

		actionInfoList.Add (newActionInfo);

		updateComponent ();
	}

	public void addNewActionFromEditor (string actionSystemName, float actionSystemDuration, float actionSystemSpeed, 
	                                    bool useActionSystemID, int actionSystemID, string actionSystemAnimationName)
	{
		actionInfo currentactionInfo = actionInfoList [0];

		currentactionInfo.Name = actionSystemName;
		currentactionInfo.animationDuration = actionSystemDuration;
		currentactionInfo.animationSpeed = actionSystemSpeed;
		currentactionInfo.useActionID = useActionSystemID;
		currentactionInfo.actionID = actionSystemID;
		currentactionInfo.useActionName = !useActionSystemID;
		currentactionInfo.actionName = actionSystemAnimationName;

		updateComponent ();
	}

	public void addNewEvent (int actionInfoIndex)
	{
		actionInfo currentActionInfo = actionInfoList [actionInfoIndex];

		eventInfo newEventInfo = new eventInfo ();

		currentActionInfo.eventInfoList.Add (newEventInfo);

		updateComponent ();
	}

	public void addNewEventFirstPerson (int actionInfoIndex)
	{
		actionInfo currentActionInfo = actionInfoList [actionInfoIndex];

		eventInfo newEventInfo = new eventInfo ();

		currentActionInfo.firstPersonEventInfoList.Add (newEventInfo);

		updateComponent ();
	}

	public void duplicateThirdPersonEventsOnFirstPerson (int actionInfoIndex)
	{
		actionInfo currentActionInfo = actionInfoList [actionInfoIndex];

		currentActionInfo.firstPersonEventInfoList.Clear ();

		for (int i = 0; i < currentActionInfo.eventInfoList.Count; i++) {
			eventInfo newEventInfo = new eventInfo (currentActionInfo.eventInfoList [i]);

			currentActionInfo.firstPersonEventInfoList.Add (newEventInfo);
		}

		updateComponent ();
	}

	public void replaceAnimationAction ()
	{
		#if UNITY_EDITOR
		if (actionInfoList.Count > 0) {
			if (newAnimationClip == null) {
				print ("No new animation clip has been assigned to replace the previous animation, make sure to configure the settings properly");

				return;
			}

			if (mainAnimator == null) {
				GameObject mainPlayer = GameObject.FindGameObjectWithTag ("Player");

				if (mainPlayer == null) {
					print ("No animator or main player found in the scene, make sure to drop it on the hierarchy or assign an animator into " +
					" the proper field of the Action System Component");
				
					return;
				} else {
					mainAnimator = mainPlayer.GetComponent<Animator> ();

					if (mainAnimator == null) {
						print ("No animator found on scene");

						return;
					}
				}
			}

			actionInfo currentactionInfo = actionInfoList [0];

			if (actionNameToReplace != "") {
				bool actionFound = false;

				for (int i = 0; i < actionInfoList.Count; i++) {
					if (!actionFound && actionInfoList [i].actionName.Equals (actionNameToReplace)) {
						actionFound = true;

						currentactionInfo = actionInfoList [i];

						if (actionInfoList [i].useActionID) {
							actionNameToReplace = currentactionInfo.Name;
						} else {
							actionNameToReplace = currentactionInfo.actionName;
						}
					}
				}
			}

			bool animationStateFound = false;

			UnityEditor.Animations.AnimatorController ac = mainAnimator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

			foreach (var layer in ac.layers) {
				if (layer.name == animationLayerName) {
					print ("Animator Layer to check: " + layer.name);

					print ("\n\n\n");

					getChildAnimatorState (actionNameToReplace, layer.stateMachine);

					print ("\n\n\n");

					if (animatorStateToReplace.state != null) {
						print ("\n\n\n");

						print ("STATE FOUND ############################################################################################");

						print ("Last animator state found " + animatorStateToReplace.state.name + " " + numberOfLoops);

						if (animatorStateToReplace.state.name == actionNameToReplace) {
							animationStateFound = true;

							animatorStateToReplace.state.speed = newAnimationSpeed;
							animatorStateToReplace.state.motion = newAnimationClip;
							animatorStateToReplace.state.mirror = newAnimationIsMirror;
						}
					}
				}
			}

			if (animationStateFound) {
				currentactionInfo.animationSpeed = newAnimationSpeed;
				currentactionInfo.animationDuration = newAnimationClip.length;

				updateComponent ();

				print ("Animation State found and replaced properly");
			} else {
				print ("Animation State not found");
			}

			newAnimationIsMirror = false;

			activateAnimationReplace = false;
		}
		#endif
	}
		
	#if UNITY_EDITOR

	UnityEditor.Animations.ChildAnimatorState animatorStateToReplace;

	public void getChildAnimatorState (string stateName, UnityEditor.Animations.AnimatorStateMachine stateMachine)
	{
		print ("State Machine to check: " + stateMachine.name + " " + stateMachine.stateMachines.Length);

		if (stateMachine.stateMachines.Length > 0) {
			foreach (var currentStateMachine in stateMachine.stateMachines) {
				numberOfLoops++;

				if (numberOfLoops > 3000) {
					print ("number of loops too big");
					return;
				}

				getChildAnimatorState (stateName, currentStateMachine.stateMachine);
			}
		} else {
			foreach (var currentState in stateMachine.states) {
				if (currentState.state.name == stateName) {
					animatorStateToReplace = currentState;
				}
			}
		}
	}
	#endif

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Action System " + gameObject.name, gameObject);
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

	void DrawGizmos ()
	{
		if (showGizmo) {
			for (int i = 0; i < actionInfoList.Count; i++) {
				if (showAllGizmo || actionInfoList [i].showGizmo) {
					if (actionInfoList [i].usePlayerWalkTarget && actionInfoList [i].playerWalkTarget != null) {
						Gizmos.color = Color.red;
						Gizmos.DrawSphere (actionInfoList [i].playerWalkTarget.position, 0.1f);
					}

					if (actionInfoList [i].setPlayerFacingDirection && actionInfoList [i].facingDirectionPositionTransform != null) {
						Gizmos.color = Color.green;
						Gizmos.DrawSphere (actionInfoList [i].facingDirectionPositionTransform.position, 0.1f);
					}

					if (actionInfoList [i].usePositionToAdjustPlayer && actionInfoList [i].positionToAdjustPlayer != null) {
						Gizmos.color = Color.yellow;
						Gizmos.DrawSphere (actionInfoList [i].positionToAdjustPlayer.position, 0.1f);

						if (actionInfoList [i].matchTargetTransform) {
							Gizmos.color = Color.grey;
							Gizmos.DrawSphere (actionInfoList [i].matchTargetTransform.position, 0.1f);
						}
					}
				}
			}
		}
	}

	[System.Serializable]
	public class actionInfo
	{
		public string Name;

		public bool useActionName;

		public string actionName;

		public bool useActionID;
		public int actionID;

		public bool removeActionIDValueImmediately;

		public bool useCrossFadeAnimation = true;

		public bool pausePlayerActionsInput;
		public bool pausePlayerMovementInput = true;
		public bool resetPlayerMovementInput;
		public bool resetPlayerMovementInputSmoothly;
		public bool removePlayerMovementInputValues;

		public bool enablePlayerCanMoveAfterAction;

		public bool allowDownVelocityDuringAction;

		public bool pauseInteractionButton;
		public bool pauseInputListDuringActionActive;
		public bool ignorePauseInputListDuringAction;

		public bool pausePlayerCameraRotationInput;
		public bool pausePlayerCameraActionsInput;
		public bool pausePlayerCameraViewChange = true;
		public bool pausePlayerCameraMouseWheel = true;
		public bool disableHeadBob = true;
		public bool pauseHeadBob = true;
		public bool ignoreCameraDirectionOnMovement;
		public bool ignoreCameraDirectionOnStrafeMovement;

		public bool pauseStrafeState;

		public bool useNewCameraStateOnActionStart;
		public string newCameraStateNameOnActionStart;
		public bool setPreviousCameraStateOnActionEnd;
		public bool useNewCameraStateOnActionEnd;
		public string newCameraStateNameOnActionEnd;

		public bool disablePlayerGravity;
		public bool disablePlayerOnGroundState;
		public bool disablePlayerCollider;
		public bool disablePlayerColliderComponent;
		public bool enablePlayerColliderComponentOnActionEnd;
		public bool ignoreSetLastTimeFallingOnActionEnd;

		public bool reloadMainColliderOnCharacterOnActionEnd;

		public bool changePlayerColliderScale;
		public bool disableIKOnHands = true;
		public bool disableIKOnFeet = true;

		public bool pauseHeadTrack = true;

		public bool setNoFrictionOnCollider = true;
		public bool forceRootMotionDuringAction = true;
		public bool actionCanHappenOnAir;

		public bool jumpCanResumePlayer;

		public bool useMovementInput;

		public bool useInteractionButtonToActivateAnimation = true;

		public bool animationTriggeredByExternalEvent;

		public bool resumePlayerAfterAction;

		public bool increaseActionIndexAfterComplete;
		public bool waitForNextPlayerInteractionButtonPress;

		public bool stayInState;

		public bool resetActionIndexAfterComplete;


		public float animationDuration;

		public float animationSpeed = 1;

		public float actionDurationOnFirstPerson = 1;


		public bool actionUsesMovement;

		public bool adjustRotationDuringMovement;

		public float delayToPlayAnimation;

		public bool usePositionToAdjustPlayer;
		public Transform positionToAdjustPlayer;
		public float adjustPlayerPositionSpeed;


		public bool movePlayerOnDirection;
		public float movePlayerOnDirectionRaycastDistance;
		public float movePlayerOnDirectionSpeed;
		public Vector3 movePlayerDirection;
		public LayerMask movePlayerOnDirectionLayermask;

		public bool usePhysicsForceOnMovePlayer;
		public float physicsForceOnMovePlayer;
		public float physicsForceOnMovePlayerDuration;
		public bool checkIfPositionReachedOnPhysicsForceOnMovePlayer;

		public AvatarTarget mainAvatarTarget;

		public Transform matchTargetTransform;

		public Vector3 matchTargetPositionWeightMask;
		public float matchTargetRotationWeightMask;

		public bool matchPlayerRotation;
		public float matchPlayerRotationSpeed;

		public Transform playerTargetTransform;

		public bool adjustPlayerPositionRotationDuring;

		public float matchStartValue;
		public float matchEndValue;

		public bool usePlayerWalkTarget;
		public bool useWalkAtTheEndOfAction;
		public Transform playerWalkTarget;

		public float maxWalkSpeed = 1;

		public bool activateDynamicObstacleDetection;

		public bool setPlayerFacingDirection;
		public Transform playerFacingDirectionTransform;
		public float maxRotationAmount = 1;
		public float minRotationAmount = 0.3f;
		public float minRotationAngle = 5;
		public bool adjustFacingDirectionBasedOnPlayerPosition;
		public Transform facingDirectionPositionTransform;
		public bool adjustRotationAtOnce;

		public bool useMovingPlayerToPositionTarget;
		public float movingPlayerToPositionTargetSpeed = 5;
		public float movingPlayerToPositionTargetDelay;


		public bool destroyActionOnEnd;
		public bool removeActionOnEnd;

		public bool setActionState;
		public string actionStateName;
		public bool actionStateToConfigure;

		public bool setObjectParent;
		public HumanBodyBones boneParent;
		public Transform objectToSetParent;
		public Transform bonePositionReference;
		public float waitTimeToParentObject;
		public float setObjectParentSpeed;

		public bool useMountPoint;
		public string mountPointName;

		public bool useEventInfoList;

		public bool useAccumulativeDelay;
		public Coroutine eventInfoListCoroutine;
		public List<eventInfo> eventInfoList = new List<eventInfo> ();

		public List<eventInfo> firstPersonEventInfoList = new List<eventInfo> ();

		public bool disableHUDOnAction;

		public bool showGizmo;

		public bool keepWeaponsDuringAction;
		public bool drawWeaponsAfterAction;
		public bool disableIKWeaponsDuringAction;

		public bool stopAimOnFireWeapons;
		public bool stopShootOnFireWeapons;

		public bool hideMeleWeaponMeshOnAction;

		public bool setInvincibilityStateActive;
		public float invincibilityStateDuration;
		public bool checkEventsOnTemporalInvincibilityActive;

		public bool disableDamageReactionDuringAction;

		public bool checkIfPlayerOnGround = true;
		public bool checkConditionsToStartActionOnUpdate;
		public bool playerMovingToStartAction;
		public bool checkPlayerToNotCrouch;

		public bool stopActionIfPlayerIsOnAir;
		public float delayToStopActionIfPlayerIsOnAir;

		public bool getUpIfPlayerCrouching = true;
		public bool crouchOnActionEnd;
		public bool setPreviousCrouchStateOnActionEnd;
		public bool checkIfPlayerCanGetUpFromCrouch;

		public bool dropGrabbedObjectsOnAction = true;
		public bool dropOnlyIfNotGrabbedPhysically = true;
		public bool dropIfGrabbedPhysicallyWithIK = true;
		public bool keepGrabbedObjectOnActionIfNotDropped;

		public bool keepMeleeWeaponGrabbed;
		public bool drawMeleeWeaponGrabbedOnActionEnd;

		public bool useEventIfActionStopped;
		public UnityEvent eventIfActionStopped;

		public bool pauseCustomInputListDuringActionActive;
		public List<playerActionSystem.inputToPauseOnActionIfo> customInputToPauseOnActionInfoList = new List<playerActionSystem.inputToPauseOnActionIfo> ();
	
		public bool useRaycastToAdjustMatchTransform;
		public bool useRaycastToAdjustTargetTransform;
		public bool useRaycastToAdjustPositionToAdjustPlayer;
		public LayerMask layerForRaycast;

		public bool pauseAIOnActionStart;
		public bool resumeAIOnActionEnd;
		public bool assignPartnerOnActionEnd;
		public bool checkUseMinAngleToActivateAction;

		public bool ignoreAnimationTransitionCheck;

		public bool pauseActivationOfOtherCustomActions;

		public bool disableAnyStateConfiguredWithExitTime;
	}

	[System.Serializable]
	public class eventInfo
	{
		public float delayToActivate;

		public UnityEvent eventToUse;

		public bool useRemoteEvent;

		public string remoteEventName;

		public bool eventTriggered;

		public bool sendCurrentPlayerOnEvent;
		public eventParameters.eventToCallWithGameObject eventToSendCurrentPlayer;

		public bool callThisEventIfActionStopped;

		public eventInfo ()
		{

		}

		public eventInfo (eventInfo newState)
		{
			delayToActivate = newState.delayToActivate;

			eventToUse = newState.eventToUse;

			useRemoteEvent = newState.useRemoteEvent;

			remoteEventName = newState.remoteEventName;

			eventTriggered = newState.eventTriggered;

			sendCurrentPlayerOnEvent = newState.sendCurrentPlayerOnEvent;

			eventToSendCurrentPlayer = newState.eventToSendCurrentPlayer;

			callThisEventIfActionStopped = newState.callThisEventIfActionStopped;
		}
	}
}
