using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class completeDialogInfo
{
	public string Name;

	public int ID;

	public bool playDialogWithoutPausingPlayerActions;

	public bool playDialogsAutomatically = true;

	public bool pausePlayerActionsInput;
	public bool pausePlayerMovementInput;

	public bool canUseInputToSetNextDialog = true;

	public bool showFullDialogLineOnInputIfTextPartByPart = true;

	public bool showDialogLineWordByWord;

	public float dialogLineWordSpeed = 0.5f;

	public bool showDialogLineLetterByLetter;

	public float dialogLineLetterSpeed = 0.03f;

	public bool useCustomTextAnchorAndAligment;
	public TextAnchor textAnchor = TextAnchor.MiddleCenter;

	public bool stopDialogIfPlayerDistanceTooFar;
	public float maxDistanceToStopDialog;
	public bool rewindLastDialogIfStopped;

	public UnityEvent eventOnDialogStopped;

	public bool playDialogOnTriggerEnter;
	public UnityEvent eventToPlayDialogOnTriggerEnter;

	public List<dialogInfo> dialogInfoList = new List<dialogInfo> ();
}

[System.Serializable]
public class dialogInfo
{
	public string Name;

	public int ID;

	public string dialogOwnerName;

	[TextArea (3, 10)] public string dialogContent;

	public bool showPreviousDialogLineOnOptions;

	public List<dialogLineInfo> dialogLineInfoList = new List<dialogLineInfo> ();

	public UnityEvent eventOnDialog;

	public bool useEventToSendPlayer;
	public eventParameters.eventToCallWithGameObject eventToSendPlayer;

	public bool activateWhenDialogClosed;
	public bool activateRemoteTriggerSystem;
	public string remoteTriggerName;

	public bool useNexLineButton = true;

	public bool isEndOfDialog;

	public bool changeToDialogInfoID;
	public int dialogInfoIDToActivate;

	public bool useRandomDialogInfoID;
	public bool useRandomDialogRange;
	public Vector2 randomDialogRange;
	public List<int> randomDialogIDList = new List<int> ();

	public bool checkConditionForNextLine;

	public int dialogInfoIDToActivateOnConditionTrue;
	public int dialogInfoIDToActivateOnConditionFalse;

	public UnityEvent eventToCheckConditionForNextLine;

	public bool useEventToSendPlayerToCondition;
	public eventParameters.eventToCallWithGameObject eventToSendPlayerToCondition;

	public bool disableDialogAfterSelect;
	public int dialogInfoIDToJump;
	public bool dialogInfoDisabled;

	public bool setNextCompleteDialogID;

	public bool setNewCompleteDialogID;
	public int newCompleteDialogID;

	public float delayToShowNextDialogLine = 5;
	public float delayToShowThisDialogLine;

	public bool useDialogLineSound;
	public AudioClip dialogLineSound;

	public bool useAnimations;
	public string animationName;
	public float delayToPlayAnimation;

	public bool useDelayToDisableAnimation;
	public float delayToDisableAnimation;

	public bool animationUsedOnPlayer;
}

[System.Serializable]
public class dialogLineInfo
{
	public string Name;

	public int ID;
	[TextArea (3, 10)] public string dialogLineContent;

	public int dialogInfoIDToActivate;

	public bool useRandomDialogInfoID;
	public bool useRandomDialogRange;
	public Vector2 randomDialogRange;
	public List<int> randomDialogIDList = new List<int> ();

	public bool activateRemoteTriggerSystem;
	public string remoteTriggerName;

	public Button dialogLineButton;

	public bool disableLineAfterSelect;

	public bool lineDisabled;

	public bool useStatToShowLine;
	public string statName;
	public bool statIsAmount;
	public float minStateValue;
	public bool boolStateValue;

	public bool answerNotAvailable;
	public string extraDialogLineContent;

	public bool checkConditionForNextLine;

	public int dialogInfoIDToActivateOnConditionTrue;
	public int dialogInfoIDToActivateOnConditionFalse;

	public UnityEvent eventToCheckConditionForNextLine;

	public bool useEventToSendPlayerToCondition;
	public eventParameters.eventToCallWithGameObject eventToSendPlayerToCondition;
}



//Dialog Info use for the localization system

[System.Serializable]
public class completeDialogInfoTemplate
{
	public string language;

	[Space]

	public List<simpleCompleteDialogInfo> completeDialogInfoList = new List<simpleCompleteDialogInfo> ();
}

[System.Serializable]
public class simpleCompleteDialogInfo
{
	public string Name;

	public int ID;

	[Space]

	public List<simpleDialogInfo> dialogInfoList = new List<simpleDialogInfo> ();
}

[System.Serializable]
public class simpleDialogInfo
{
	public string Name;

	public int ID;

	public string dialogOwnerName;

	[TextArea (3, 10)] public string dialogContent;

	public AudioClip dialogLineSound;

	[Space]

	public List<simpleDialogLineInfo> dialogLineInfoList = new List<simpleDialogLineInfo> ();
}

[System.Serializable]
public class simpleDialogLineInfo
{
	public string Name;

	public int ID;

	[TextArea (3, 10)] public string dialogLineContent;
}