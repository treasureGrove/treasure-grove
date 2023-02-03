using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(dialogContentSystem))]
public class dialogContentSystemEditor : Editor
{
	SerializedProperty dialogContentID;
	SerializedProperty dialogContentScene;
	SerializedProperty currentDialogIndex;
	SerializedProperty dialogOwner;
	SerializedProperty showDialogOnwerName;
	SerializedProperty dialogActive;
	SerializedProperty dialogInProcess;
	SerializedProperty playingExternalDialog;
	SerializedProperty completeDialogInfoList;

	SerializedProperty useAnimations;
	SerializedProperty mainAnimator;
	SerializedProperty mainPlayerController;
	SerializedProperty dialogueActiveAnimatorName;

	SerializedProperty playerAnimationsOnDialogEnabled;

	SerializedProperty useEventsOnStartEndDialog;
	SerializedProperty eventOnStartDialog;
	SerializedProperty eventOnEndDialog;

	SerializedProperty newCharacterToAddDialog;

	SerializedProperty pauseAIOnDialogStart;
	SerializedProperty resumeAIOnDialogEnd;
	SerializedProperty interruptWanderAroundStateIfActiveOnDialogStart;
	SerializedProperty disableWanderAroundStateOnDialogEnd;

	SerializedProperty useDialogContentTemplate;
	SerializedProperty mainDialogContentTemplate;


	dialogContentSystem manager;

	SerializedProperty currentArrayElement;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		dialogContentID = serializedObject.FindProperty ("dialogContentID");
		dialogContentScene = serializedObject.FindProperty ("dialogContentScene");
		currentDialogIndex = serializedObject.FindProperty ("currentDialogIndex");
		dialogOwner = serializedObject.FindProperty ("dialogOwner");
		showDialogOnwerName = serializedObject.FindProperty ("showDialogOnwerName");
		dialogActive = serializedObject.FindProperty ("dialogActive");
		dialogInProcess = serializedObject.FindProperty ("dialogInProcess");
		playingExternalDialog = serializedObject.FindProperty ("playingExternalDialog");
		completeDialogInfoList = serializedObject.FindProperty ("completeDialogInfoList");

		useAnimations = serializedObject.FindProperty ("useAnimations");
		mainAnimator = serializedObject.FindProperty ("mainAnimator");
		mainPlayerController = serializedObject.FindProperty ("mainPlayerController");

		dialogueActiveAnimatorName = serializedObject.FindProperty ("dialogueActiveAnimatorName");

		playerAnimationsOnDialogEnabled = serializedObject.FindProperty ("playerAnimationsOnDialogEnabled");

		useEventsOnStartEndDialog = serializedObject.FindProperty ("useEventsOnStartEndDialog");
		eventOnStartDialog = serializedObject.FindProperty ("eventOnStartDialog");
		eventOnEndDialog = serializedObject.FindProperty ("eventOnEndDialog");

		newCharacterToAddDialog = serializedObject.FindProperty ("newCharacterToAddDialog");

		pauseAIOnDialogStart = serializedObject.FindProperty ("pauseAIOnDialogStart");
		resumeAIOnDialogEnd = serializedObject.FindProperty ("resumeAIOnDialogEnd");
		interruptWanderAroundStateIfActiveOnDialogStart = serializedObject.FindProperty ("interruptWanderAroundStateIfActiveOnDialogStart");
		disableWanderAroundStateOnDialogEnd = serializedObject.FindProperty ("disableWanderAroundStateOnDialogEnd");

		useDialogContentTemplate = serializedObject.FindProperty ("useDialogContentTemplate");
		mainDialogContentTemplate = serializedObject.FindProperty ("mainDialogContentTemplate");
	
		manager = (dialogContentSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (50));

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (dialogContentID);
		EditorGUILayout.PropertyField (dialogContentScene);
		EditorGUILayout.PropertyField (currentDialogIndex);
		EditorGUILayout.PropertyField (dialogOwner);
		EditorGUILayout.PropertyField (showDialogOnwerName);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("AI Settings", "window");
		EditorGUILayout.PropertyField (pauseAIOnDialogStart);
		EditorGUILayout.PropertyField (resumeAIOnDialogEnd);
		EditorGUILayout.PropertyField (interruptWanderAroundStateIfActiveOnDialogStart);
		EditorGUILayout.PropertyField (disableWanderAroundStateOnDialogEnd);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Animations Settings", "window");
		EditorGUILayout.PropertyField (useAnimations);
		if (useAnimations.boolValue) {
			EditorGUILayout.PropertyField (mainAnimator);
			EditorGUILayout.PropertyField (mainPlayerController);
			EditorGUILayout.PropertyField (dialogueActiveAnimatorName);
			EditorGUILayout.PropertyField (playerAnimationsOnDialogEnabled);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dialog State", "window");
		GUILayout.Label ("Dialog Active\t" + dialogActive.boolValue);
		GUILayout.Label ("Dialog In Process\t" + dialogInProcess.boolValue);
		GUILayout.Label ("External Dialog\t" + playingExternalDialog.boolValue);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Complete Dialog List", "window");
		showCompleteDialogInfoList (completeDialogInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Events Settings", "window");
		EditorGUILayout.PropertyField (useEventsOnStartEndDialog);
		if (useEventsOnStartEndDialog.boolValue) {
			EditorGUILayout.PropertyField (eventOnStartDialog);
			EditorGUILayout.PropertyField (eventOnEndDialog);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Add Dialog To Character Manually Settings", "window");
		EditorGUILayout.PropertyField (newCharacterToAddDialog);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Dialog To Character")) {
			manager.addDialogToCharacterManually ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dialog Content Template Settings", "window");
		EditorGUILayout.PropertyField (useDialogContentTemplate);
		if (useDialogContentTemplate.boolValue) {
			EditorGUILayout.PropertyField (mainDialogContentTemplate);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Add Dialog Content To Template")) {
				manager.addDialogContentToTemplate ();
			}
		}
		GUILayout.EndVertical ();
		


		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
		EditorGUILayout.Space ();
	}

	void showCompleteDialogInfoList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Complete Dialog Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			int currentArraySize = list.arraySize;

			GUILayout.Label ("Number of Complete Dialogs: " + currentArraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Dialog")) {
				manager.addNewDialog ();

				currentArraySize = list.arraySize;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();

				currentArraySize = list.arraySize;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
		
			for (int i = 0; i < currentArraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						showCompleteDialogInfoListElement (currentArrayElement, i);
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

					currentArraySize = list.arraySize;
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < currentArraySize) {
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


	void showCompleteDialogInfoListElement (SerializedProperty list, int dialogIndex)
	{
		GUILayout.BeginVertical ("box");

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ID"));

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dialog Text Anchor And Alignment Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useCustomTextAnchorAndAligment"));
		if (list.FindPropertyRelative ("useCustomTextAnchorAndAligment").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("textAnchor"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dialogs Without Pausing Player Actions Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("playDialogWithoutPausingPlayerActions"));
		if (list.FindPropertyRelative ("playDialogWithoutPausingPlayerActions").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("playDialogsAutomatically"));	

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("canUseInputToSetNextDialog"));	

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("pausePlayerActionsInput"));	

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("pausePlayerMovementInput"));	

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Show Dialog Part By Part Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("showDialogLineWordByWord"));

			if (list.FindPropertyRelative ("showDialogLineWordByWord").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogLineWordSpeed"));
			}
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("showDialogLineLetterByLetter"));
			if (list.FindPropertyRelative ("showDialogLineLetterByLetter").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogLineLetterSpeed"));
			}

			if (list.FindPropertyRelative ("showDialogLineWordByWord").boolValue || list.FindPropertyRelative ("showDialogLineLetterByLetter").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("showFullDialogLineOnInputIfTextPartByPart"));	
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Stop Dialog By Distance Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("stopDialogIfPlayerDistanceTooFar"));
			if (list.FindPropertyRelative ("stopDialogIfPlayerDistanceTooFar").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("maxDistanceToStopDialog"));
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("rewindLastDialogIfStopped"));
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnDialogStopped"));	

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Play Dialog On Trigger Enter Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("playDialogOnTriggerEnter"));

			EditorGUILayout.Space ();

			if (list.FindPropertyRelative ("playDialogOnTriggerEnter").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToPlayDialogOnTriggerEnter"));
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dialogs List", "window");
		showDialogInfoList (list.FindPropertyRelative ("dialogInfoList"), dialogIndex, list.FindPropertyRelative ("playDialogWithoutPausingPlayerActions").boolValue);
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showDialogInfoList (SerializedProperty list, int dialogIndex, bool playDialogWithoutPausingPlayerActions)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Dialog Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			int currentArraySize = list.arraySize;

			GUILayout.Label ("Number of Dialogs: " + currentArraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Line")) {
				manager.addNewLine (dialogIndex);

				currentArraySize = list.arraySize;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();

				currentArraySize = list.arraySize;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < currentArraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						showDialogInfoListElement (currentArrayElement, dialogIndex, i, playDialogWithoutPausingPlayerActions);
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

					currentArraySize = list.arraySize;
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < currentArraySize) {
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

	void showDialogInfoListElement (SerializedProperty list, int dialogIndex, int lineIndex, bool playDialogWithoutPausingPlayerActions)
	{
		GUILayout.BeginVertical ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogOwnerName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ID"));

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogContent"));

		if (playDialogWithoutPausingPlayerActions) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToShowThisDialogLine"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToShowNextDialogLine"));
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Event System Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateRemoteTriggerSystem"));
		if (list.FindPropertyRelative ("activateRemoteTriggerSystem").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remoteTriggerName"));
		}

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateWhenDialogClosed"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventOnDialog"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventToSendPlayer"));
		if (list.FindPropertyRelative ("useEventToSendPlayer").boolValue) {

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSendPlayer"));
		}

		GUILayout.EndVertical ();

		if (!playDialogWithoutPausingPlayerActions) {

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Next UI Element Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNexLineButton"), new GUIContent ("Use Next Line Button"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("isEndOfDialog"));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Next Line Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("changeToDialogInfoID"));

		if (list.FindPropertyRelative ("changeToDialogInfoID").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRandomDialogInfoID"));
			if (list.FindPropertyRelative ("useRandomDialogInfoID").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRandomDialogRange"));
				if (list.FindPropertyRelative ("useRandomDialogRange").boolValue) {
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("randomDialogRange"));
				} else {
					GUILayout.BeginVertical ("Random Dialog Id List", "window");
					showRandomDialogIDList (list.FindPropertyRelative ("randomDialogIDList"));
					GUILayout.EndVertical ();
				}
			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogInfoIDToActivate"));
			}
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Condition On Line Info Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkConditionForNextLine"));
		if (list.FindPropertyRelative ("checkConditionForNextLine").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogInfoIDToActivateOnConditionTrue"));	

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogInfoIDToActivateOnConditionFalse"));	

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCheckConditionForNextLine"));	

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventToSendPlayerToCondition"));
			if (list.FindPropertyRelative ("useEventToSendPlayerToCondition").boolValue) {

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSendPlayerToCondition"));	
			}
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();

		if (!playDialogWithoutPausingPlayerActions) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Disable Dialog Info Settings", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableDialogAfterSelect"));
			if (list.FindPropertyRelative ("disableDialogAfterSelect").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogInfoIDToJump"));
			}
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Set Next Complete Dialog Info Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNextCompleteDialogID"));
		if (!list.FindPropertyRelative ("setNextCompleteDialogID").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("setNewCompleteDialogID"));
			if (list.FindPropertyRelative ("setNewCompleteDialogID").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("newCompleteDialogID"));
			}
		}
		GUILayout.EndVertical ();

		if (!playDialogWithoutPausingPlayerActions) {
			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("Dialog Lines List", "window");
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("showPreviousDialogLineOnOptions"));

			EditorGUILayout.Space ();

			showDialogLineInfoList (list.FindPropertyRelative ("dialogLineInfoList"), dialogIndex, lineIndex);
			GUILayout.EndVertical ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dialog Line Voices Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDialogLineSound"));
		if (list.FindPropertyRelative ("useDialogLineSound").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogLineSound"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Dialog Character Animations Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useAnimations"));
		if (list.FindPropertyRelative ("useAnimations").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("animationName"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToPlayAnimation"));

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useDelayToDisableAnimation"));
			if (list.FindPropertyRelative ("useDelayToDisableAnimation").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("delayToDisableAnimation"));
			}

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("animationUsedOnPlayer"));
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showDialogLineInfoList (SerializedProperty list, int dialogIndex, int lineIndex)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Dialog Line Info List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			int currentArraySize = list.arraySize;

			GUILayout.Label ("Number of Lines: " + currentArraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Answer")) {
				manager.addNewAnswer (dialogIndex, lineIndex);

				currentArraySize = list.arraySize;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();

				currentArraySize = list.arraySize;
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

			for (int i = 0; i < currentArraySize; i++) {
				bool expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					currentArrayElement = list.GetArrayElementAtIndex (i);

					EditorGUILayout.PropertyField (currentArrayElement, false);
					if (currentArrayElement.isExpanded) {
						showDialogLineInfoListElement (currentArrayElement);
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

					currentArraySize = list.arraySize;
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < currentArraySize) {
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

	void showDialogLineInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("ID"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogLineContent"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Next Line Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRandomDialogInfoID"));
		if (list.FindPropertyRelative ("useRandomDialogInfoID").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useRandomDialogRange"));
			if (list.FindPropertyRelative ("useRandomDialogRange").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("randomDialogRange"));
			} else {
				GUILayout.BeginVertical ("Random Dialog Id List", "window");
				showRandomDialogIDList (list.FindPropertyRelative ("randomDialogIDList"));
				GUILayout.EndVertical ();
			}
		} else {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogInfoIDToActivate"));
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Remote Event System Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("activateRemoteTriggerSystem"));
		if (list.FindPropertyRelative ("activateRemoteTriggerSystem").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("remoteTriggerName"));
		}
			
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Disable Line Info Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("disableLineAfterSelect"));
		if (list.FindPropertyRelative ("disableLineAfterSelect").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("lineDisabled"));	
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Stat To Check On Line Info Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useStatToShowLine"));
		if (list.FindPropertyRelative ("useStatToShowLine").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("statName"));	
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("statIsAmount"));	
			if (list.FindPropertyRelative ("statIsAmount").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("minStateValue"));	
			} else {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("boolStateValue"));	
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Condition On Line Info Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkConditionForNextLine"));
		if (list.FindPropertyRelative ("checkConditionForNextLine").boolValue) {

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogInfoIDToActivateOnConditionTrue"));	
		
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("dialogInfoIDToActivateOnConditionFalse"));	

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToCheckConditionForNextLine"));	

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("useEventToSendPlayerToCondition"));
			if (list.FindPropertyRelative ("useEventToSendPlayerToCondition").boolValue) {

				EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToSendPlayerToCondition"));	
			}
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}

	void showRandomDialogIDList (SerializedProperty list)
	{
		GUILayout.BeginVertical ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide Random Dialog ID List", buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			int currentArraySize = list.arraySize;

			GUILayout.Label ("Number of ID: " + currentArraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add ID")) {
				list.arraySize++;

				currentArraySize = list.arraySize;
			}
			if (GUILayout.Button ("Clear")) {
				list.ClearArray ();

				currentArraySize = list.arraySize;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			EditorGUILayout.Space ();

			for (int i = 0; i < currentArraySize; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < currentArraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}

				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);

					currentArraySize = list.arraySize;
				}

				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}
		GUILayout.EndVertical ();
	}
}
#endif