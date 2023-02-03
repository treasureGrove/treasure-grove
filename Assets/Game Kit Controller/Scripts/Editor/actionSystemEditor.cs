using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(actionSystem))]
public class actionSystemEditor : Editor
{
	SerializedProperty useMinDistanceToActivateAction;
	SerializedProperty minDistanceToActivateAction;
	SerializedProperty useMinAngleToActivateAction;
	SerializedProperty minAngleToActivateAction;
	SerializedProperty checkOppositeAngle;
	SerializedProperty canStopPreviousAction;
	SerializedProperty canForceToPlayCustomAction;
	SerializedProperty mainTrigger;
	SerializedProperty currentActionInfoIndex;

	SerializedProperty activateCustomActionAfterActionComplete;
	SerializedProperty customActionToActiveAfterActionComplete;

	SerializedProperty addActionToListStoredToPlay;
	SerializedProperty playActionAutomaticallyIfStoredAtEnd;

	SerializedProperty useEventsOnPlayerInsideOutside;
	SerializedProperty eventOnPlayerInside;
	SerializedProperty eventOnPlayerOutside;
	SerializedProperty useEventsOnStartEndAction;
	SerializedProperty eventOnStartAction;
	SerializedProperty eventOnEndAction;
	SerializedProperty sendCurrentPlayerOnEvent;
	SerializedProperty eventToSendCurrentPlayer;

	SerializedProperty useEventAfterResumePlayer;
	SerializedProperty eventAfterResumePlayer;

	SerializedProperty useEventsToEnableDisableActionObject;
	SerializedProperty eventToEnableActionObject;
	SerializedProperty eventToDisableActionObject;

	SerializedProperty animationUsedOnUpperBody;
	SerializedProperty disableRegularActionActiveState;
	SerializedProperty disableRegularActionActiveStateOnEnd;

	SerializedProperty changeCameraViewToThirdPersonOnAction;

	SerializedProperty actionsCanBeUsedOnFirstPerson;
	SerializedProperty ignoreChangeToThirdPerson;

	SerializedProperty actionInfoList;

	SerializedProperty showDebugPrint;
	SerializedProperty showGizmo;
	SerializedProperty showAllGizmo;

	SerializedProperty mainAnimator;
	SerializedProperty newAnimationClip;
	SerializedProperty actionNameToReplace;
	SerializedProperty animationLayerName;
	SerializedProperty newAnimationSpeed;
	SerializedProperty newAnimationIsMirror;
	SerializedProperty activateAnimationReplace;

	SerializedProperty setPlayerParentDuringActionActive;
	SerializedProperty playerParentDuringAction;

	actionSystem manager;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		useMinDistanceToActivateAction = serializedObject.FindProperty ("useMinDistanceToActivateAction");
		minDistanceToActivateAction = serializedObject.FindProperty ("minDistanceToActivateAction");
		useMinAngleToActivateAction = serializedObject.FindProperty ("useMinAngleToActivateAction");
		minAngleToActivateAction = serializedObject.FindProperty ("minAngleToActivateAction");
		checkOppositeAngle = serializedObject.FindProperty ("checkOppositeAngle");
		canStopPreviousAction = serializedObject.FindProperty ("canStopPreviousAction");
		canForceToPlayCustomAction = serializedObject.FindProperty ("canForceToPlayCustomAction");
		mainTrigger = serializedObject.FindProperty ("mainTrigger");
		currentActionInfoIndex = serializedObject.FindProperty ("currentActionInfoIndex");

		activateCustomActionAfterActionComplete = serializedObject.FindProperty ("activateCustomActionAfterActionComplete");
		customActionToActiveAfterActionComplete = serializedObject.FindProperty ("customActionToActiveAfterActionComplete");

		addActionToListStoredToPlay = serializedObject.FindProperty ("addActionToListStoredToPlay");
		playActionAutomaticallyIfStoredAtEnd = serializedObject.FindProperty ("playActionAutomaticallyIfStoredAtEnd");

		useEventsOnPlayerInsideOutside = serializedObject.FindProperty ("useEventsOnPlayerInsideOutside");
		eventOnPlayerInside = serializedObject.FindProperty ("eventOnPlayerInside");
		eventOnPlayerOutside = serializedObject.FindProperty ("eventOnPlayerOutside");
		useEventsOnStartEndAction = serializedObject.FindProperty ("useEventsOnStartEndAction");
		eventOnStartAction = serializedObject.FindProperty ("eventOnStartAction");
		eventOnEndAction = serializedObject.FindProperty ("eventOnEndAction");
		sendCurrentPlayerOnEvent = serializedObject.FindProperty ("sendCurrentPlayerOnEvent");
		eventToSendCurrentPlayer = serializedObject.FindProperty ("eventToSendCurrentPlayer");

		useEventAfterResumePlayer = serializedObject.FindProperty ("useEventAfterResumePlayer");
		eventAfterResumePlayer = serializedObject.FindProperty ("eventAfterResumePlayer");

		useEventsToEnableDisableActionObject = serializedObject.FindProperty ("useEventsToEnableDisableActionObject");
		eventToEnableActionObject = serializedObject.FindProperty ("eventToEnableActionObject");
		eventToDisableActionObject = serializedObject.FindProperty ("eventToDisableActionObject");

		animationUsedOnUpperBody = serializedObject.FindProperty ("animationUsedOnUpperBody");
		disableRegularActionActiveState = serializedObject.FindProperty ("disableRegularActionActiveState");
		disableRegularActionActiveStateOnEnd = serializedObject.FindProperty ("disableRegularActionActiveStateOnEnd");

		changeCameraViewToThirdPersonOnAction = serializedObject.FindProperty ("changeCameraViewToThirdPersonOnAction");

		actionsCanBeUsedOnFirstPerson = serializedObject.FindProperty ("actionsCanBeUsedOnFirstPerson");
		ignoreChangeToThirdPerson = serializedObject.FindProperty ("ignoreChangeToThirdPerson");

		actionInfoList = serializedObject.FindProperty ("actionInfoList");

		showDebugPrint = serializedObject.FindProperty ("showDebugPrint");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		showAllGizmo = serializedObject.FindProperty ("showAllGizmo");

		mainAnimator = serializedObject.FindProperty ("mainAnimator");
		newAnimationClip = serializedObject.FindProperty ("newAnimationClip");
		actionNameToReplace = serializedObject.FindProperty ("actionNameToReplace");
		animationLayerName = serializedObject.FindProperty ("animationLayerName");
		newAnimationSpeed = serializedObject.FindProperty ("newAnimationSpeed");
		newAnimationIsMirror = serializedObject.FindProperty ("newAnimationIsMirror");

		activateAnimationReplace = serializedObject.FindProperty ("activateAnimationReplace");

		setPlayerParentDuringActionActive = serializedObject.FindProperty ("setPlayerParentDuringActionActive");
		playerParentDuringAction = serializedObject.FindProperty ("playerParentDuringAction");

		manager = (actionSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical (GUILayout.Height (50));

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (useMinDistanceToActivateAction);
		if (useMinDistanceToActivateAction.boolValue) {
			EditorGUILayout.PropertyField (minDistanceToActivateAction);
		}
		EditorGUILayout.PropertyField (useMinAngleToActivateAction);
		if (useMinAngleToActivateAction.boolValue) {
			EditorGUILayout.PropertyField (minAngleToActivateAction);
			EditorGUILayout.PropertyField (checkOppositeAngle);
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (canStopPreviousAction);
		EditorGUILayout.PropertyField (canForceToPlayCustomAction);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Settings", "window");
		EditorGUILayout.PropertyField (mainTrigger);

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (animationUsedOnUpperBody);
		if (animationUsedOnUpperBody.boolValue) {
			EditorGUILayout.PropertyField (disableRegularActionActiveState);

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (disableRegularActionActiveStateOnEnd);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (setPlayerParentDuringActionActive);
		if (setPlayerParentDuringActionActive.boolValue) {
			EditorGUILayout.PropertyField (playerParentDuringAction);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("First Person Settings", "window");
		EditorGUILayout.PropertyField (changeCameraViewToThirdPersonOnAction);
		EditorGUILayout.PropertyField (actionsCanBeUsedOnFirstPerson);
		if (actionsCanBeUsedOnFirstPerson.boolValue) {
			EditorGUILayout.PropertyField (ignoreChangeToThirdPerson);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Action State", "window");
		EditorGUILayout.PropertyField (currentActionInfoIndex);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Activate New Action After Action Complete", "window");
		EditorGUILayout.PropertyField (activateCustomActionAfterActionComplete);
		if (activateCustomActionAfterActionComplete.boolValue) {
			EditorGUILayout.PropertyField (customActionToActiveAfterActionComplete);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Store Action On List To Play", "window");
		EditorGUILayout.PropertyField (addActionToListStoredToPlay);
		if (addActionToListStoredToPlay.boolValue) {
			EditorGUILayout.PropertyField (playActionAutomaticallyIfStoredAtEnd);
		}
		GUILayout.EndVertical ();
	
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (useEventsOnPlayerInsideOutside);
		if (useEventsOnPlayerInsideOutside.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnPlayerInside);
			EditorGUILayout.PropertyField (eventOnPlayerOutside);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventsOnStartEndAction);
		if (useEventsOnStartEndAction.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventOnStartAction);
			EditorGUILayout.PropertyField (eventOnEndAction);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventAfterResumePlayer);
		if (useEventAfterResumePlayer.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventAfterResumePlayer);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (sendCurrentPlayerOnEvent);
		if (sendCurrentPlayerOnEvent.boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (eventToSendCurrentPlayer);
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (useEventsToEnableDisableActionObject);
		if (useEventsToEnableDisableActionObject.boolValue) {
			EditorGUILayout.PropertyField (eventToEnableActionObject);
			EditorGUILayout.PropertyField (eventToDisableActionObject);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Action Info List", "window");
		showActionInfoList (actionInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo And Debug Settings", "window");
		EditorGUILayout.PropertyField (showGizmo);
		EditorGUILayout.PropertyField (showAllGizmo);
		EditorGUILayout.PropertyField (showDebugPrint);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Replace Action Animation Settings", "window");

		EditorGUILayout.PropertyField (activateAnimationReplace);

		if (activateAnimationReplace.boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (mainAnimator);
			EditorGUILayout.PropertyField (animationLayerName);
			EditorGUILayout.PropertyField (actionNameToReplace);

			EditorGUILayout.Space ();

			GUILayout.Label ("New Animation Properties");

			EditorGUILayout.PropertyField (newAnimationClip);

			EditorGUILayout.PropertyField (newAnimationSpeed);

			EditorGUILayout.PropertyField (newAnimationIsMirror);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Replace Action")) {
				manager.replaceAnimationAction ();
			}

		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showActionInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Actions: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Action")) {
				manager.addNewAction ();
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showActionInfoListElement (list.GetArrayElementAtIndex (i), i);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}

				EditorGUILayout.Space ();

				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showActionInfoListElement (SerializedProperty list, int actionInfoIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useActionID"));
		if (list.FindPropertyRelative ("useActionID").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionID"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("removeActionIDValueImmediately"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useActionName"));
		if (list.FindPropertyRelative ("useActionName").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionName"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCrossFadeAnimation"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Input Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pausePlayerActionsInput"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pausePlayerMovementInput"));
		if (list.FindPropertyRelative ("pausePlayerMovementInput").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetPlayerMovementInput"));
			if (!list.FindPropertyRelative ("resetPlayerMovementInput").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetPlayerMovementInputSmoothly"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("removePlayerMovementInputValues"));
			}
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMovementInput"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("allowDownVelocityDuringAction"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enablePlayerCanMoveAfterAction"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera Input Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pausePlayerCameraRotationInput"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pausePlayerCameraActionsInput"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pausePlayerCameraViewChange"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pausePlayerCameraMouseWheel"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseHeadBob"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableHeadBob"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreCameraDirectionOnMovement"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreCameraDirectionOnStrafeMovement"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Input Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseInteractionButton"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseInputListDuringActionActive"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignorePauseInputListDuringAction"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Physics Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disablePlayerGravity"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disablePlayerOnGroundState"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreSetLastTimeFallingOnActionEnd"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disablePlayerCollider"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disablePlayerColliderComponent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("enablePlayerColliderComponentOnActionEnd"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("reloadMainColliderOnCharacterOnActionEnd"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("changePlayerColliderScale"));
	
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableIKOnHands"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableIKOnFeet"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseHeadTrack"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNoFrictionOnCollider"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("forceRootMotionDuringAction"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionCanHappenOnAir"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("jumpCanResumePlayer"));

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Walk To Target Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePlayerWalkTarget"));
		if (list.FindPropertyRelative ("usePlayerWalkTarget").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useWalkAtTheEndOfAction"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerWalkTarget"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxWalkSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateDynamicObstacleDetection"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Player Face Direction Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setPlayerFacingDirection"));
		if (list.FindPropertyRelative ("setPlayerFacingDirection").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerFacingDirectionTransform"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxRotationAmount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("minRotationAmount"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("minRotationAngle"));

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("adjustFacingDirectionBasedOnPlayerPosition"));
			if (list.FindPropertyRelative ("adjustFacingDirectionBasedOnPlayerPosition").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("facingDirectionPositionTransform"));
			
			}

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("adjustRotationAtOnce"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseStrafeState"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Move Player On Direction Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("movePlayerOnDirection"));
		if (list.FindPropertyRelative ("movePlayerOnDirection").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("movePlayerOnDirectionRaycastDistance"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("movePlayerOnDirectionSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("movePlayerDirection"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("movePlayerOnDirectionLayermask"));

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePhysicsForceOnMovePlayer"));
			if (list.FindPropertyRelative ("usePhysicsForceOnMovePlayer").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("physicsForceOnMovePlayer"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("physicsForceOnMovePlayerDuration"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkIfPositionReachedOnPhysicsForceOnMovePlayer"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Match Animation Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("usePositionToAdjustPlayer"));
		if (list.FindPropertyRelative ("usePositionToAdjustPlayer").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("positionToAdjustPlayer"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("adjustPlayerPositionSpeed"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToPlayAnimation"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("adjustPlayerPositionRotationDuring"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("mainAvatarTarget"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("matchTargetTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("matchTargetPositionWeightMask"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("matchTargetRotationWeightMask"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("matchStartValue"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("matchEndValue"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("matchPlayerRotation"));
		if (list.FindPropertyRelative ("matchPlayerRotation").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("matchPlayerRotationSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerTargetTransform"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMovingPlayerToPositionTarget"));
		if (list.FindPropertyRelative ("useMovingPlayerToPositionTarget").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("movingPlayerToPositionTargetSpeed"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("movingPlayerToPositionTargetDelay"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Action Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useInteractionButtonToActivateAnimation"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("animationTriggeredByExternalEvent"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("resumePlayerAfterAction"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("increaseActionIndexAfterComplete"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("waitForNextPlayerInteractionButtonPress"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("resetActionIndexAfterComplete"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stayInState"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("animationDuration"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("animationSpeed"));

		if (actionsCanBeUsedOnFirstPerson.boolValue && ignoreChangeToThirdPerson.boolValue) {
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionDurationOnFirstPerson"));

			EditorGUILayout.Space ();
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionUsesMovement"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("adjustRotationDuringMovement"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ignoreAnimationTransitionCheck"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Grabbed Objects Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dropGrabbedObjectsOnAction"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dropOnlyIfNotGrabbedPhysically")); 
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dropIfGrabbedPhysicallyWithIK")); 
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("keepGrabbedObjectOnActionIfNotDropped")); 
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("keepMeleeWeaponGrabbed"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("drawMeleeWeaponGrabbedOnActionEnd"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Other Action Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("destroyActionOnEnd"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("removeActionOnEnd"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableHUDOnAction"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopActionIfPlayerIsOnAir"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToStopActionIfPlayerIsOnAir"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseActivationOfOtherCustomActions"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableAnyStateConfiguredWithExitTime"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Raycast Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastToAdjustMatchTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastToAdjustTargetTransform"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRaycastToAdjustPositionToAdjustPlayer"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("layerForRaycast"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Set Player Action State Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setActionState"));
		if (list.FindPropertyRelative ("setActionState").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionStateName"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("actionStateToConfigure"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Set Player Bone As Object Parent Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setObjectParent"));
		if (list.FindPropertyRelative ("setObjectParent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("boneParent"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("objectToSetParent"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("bonePositionReference"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("waitTimeToParentObject"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("setObjectParentSpeed"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMountPoint"));
			if (list.FindPropertyRelative ("useMountPoint").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("mountPointName"));
			} 
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Camera State Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewCameraStateOnActionStart"));
		if (list.FindPropertyRelative ("useNewCameraStateOnActionStart").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("newCameraStateNameOnActionStart"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("setPreviousCameraStateOnActionEnd")); 
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNewCameraStateOnActionEnd")); 
			if (list.FindPropertyRelative ("useNewCameraStateOnActionEnd").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("newCameraStateNameOnActionEnd"));
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Weapon Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("keepWeaponsDuringAction"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("drawWeaponsAfterAction"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableIKWeaponsDuringAction"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopAimOnFireWeapons"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopShootOnFireWeapons"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("hideMeleWeaponMeshOnAction"));
		 
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Invincibility Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setInvincibilityStateActive"));
		if (list.FindPropertyRelative ("setInvincibilityStateActive").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("invincibilityStateDuration"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkEventsOnTemporalInvincibilityActive"));
		}
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableDamageReactionDuringAction"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Crouch Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("getUpIfPlayerCrouching"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("crouchOnActionEnd"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setPreviousCrouchStateOnActionEnd"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Conditions To Start Action Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkIfPlayerOnGround"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkConditionsToStartActionOnUpdate"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playerMovingToStartAction"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkPlayerToNotCrouch"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkIfPlayerCanGetUpFromCrouch"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseAIOnActionStart"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("resumeAIOnActionEnd"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("assignPartnerOnActionEnd"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkUseMinAngleToActivateAction"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventInfoList"));
		if (list.FindPropertyRelative ("useEventInfoList").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAccumulativeDelay"));

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Third Person Event Info List", "window");
			showEventInfoList (list.FindPropertyRelative ("eventInfoList"), actionInfoIndex, false);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("First Person Event Info List", "window");
			showEventInfoList (list.FindPropertyRelative ("firstPersonEventInfoList"), actionInfoIndex, true);
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Duplicate Third Person Events On First Person", buttonStyle)) {
				manager.duplicateThirdPersonEventsOnFirstPerson (actionInfoIndex);
			}
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventIfActionStopped"));
		if (list.FindPropertyRelative ("useEventIfActionStopped").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventIfActionStopped"));
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Pause Custom Input List Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("pauseCustomInputListDuringActionActive"));
		if (list.FindPropertyRelative ("pauseCustomInputListDuringActionActive").boolValue) {
			
			EditorGUILayout.Space ();

			showSimpleList (list.FindPropertyRelative ("customInputToPauseOnActionInfoList"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("showGizmo"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
	}

	void showEventInfoList (SerializedProperty list, int actionInfoIndex, bool firstPersonEventInfoList)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Events: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Event")) {
				if (firstPersonEventInfoList) {
					manager.addNewEventFirstPerson (actionInfoIndex);
				} else {
					manager.addNewEvent (actionInfoIndex);
				}
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						showEventInfoListElement (list.GetArrayElementAtIndex (i), actionInfoIndex);
						expanded = true;
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}

	void showEventInfoListElement (SerializedProperty list, int actionInfoIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToActivate"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToUse"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRemoteEvent"));
		if (list.FindPropertyRelative ("useRemoteEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remoteEventName"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("sendCurrentPlayerOnEvent"));
		if (list.FindPropertyRelative ("sendCurrentPlayerOnEvent").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSendCurrentPlayer"));
		}
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("callThisEventIfActionStopped"));

//		EditorGUILayout.Space ();
//
//		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventTriggered"));
		GUILayout.EndVertical ();
	}

	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Custom Input To Pause On Action Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i).FindPropertyRelative ("inputName"), false);
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
					return;
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif