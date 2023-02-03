using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using UnityEngine.Events;

public class dialogSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool dialogEnabled = true;

	[Space]
	[Header ("Answers Settings")]
	[Space]

	public Color regularAnswerColor = Color.white;
	public Color notAvailableAnswerColor = Color.red;
	public Color availableAnswerColor = Color.green;

	[Space]
	[Header ("Animator Settings")]
	[Space]

	public string dialogueActiveAnimatorName = "Dialogue Active";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public string currentLanguage;

	public List<dialogPanelInfo> dialogPanelInfoList = new List<dialogPanelInfo> ();

	public dialogContentSystem currentDialogContentSystem;

	public dialogContentSystem previousDialogContentSystem;

	public int currentDialogContentIndex;

	public bool dialogActive;

	public bool dialogInProcess;

	public bool playDialogWithoutPausingPlayerActions;

	public bool showDialogLineWordByWord;

	public bool showDialogLineLetterByLetter;

	public bool playDialogsAutomatically;

	public bool canUseInputToSetNextDialog;

	public bool showFullDialogLineOnInputIfTextPartByPart;

	public bool useCustomTextAnchorAndAligment;

	public bool stopDialogIfPlayerDistanceTooFar;
	public float maxDistanceToStopDialog;
	public bool rewindLastDialogIfStopped;

	public bool textingShowingPartByPart;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnPausePlayerActionsInput;
	public UnityEvent eventOnResumePlayerActionsInput;


	[Space]
	[Header ("Dialog Elements")]
	[Space]

	public GameObject dialogPanel;

	public Text currentDialogOwnerNameText;

	public GameObject dialogContentWithoutOptions;
	public GameObject dialogContentWithOptions;

	public Text mainLineText;

	public Transform mainLineTextWithoutOptions;
	public Transform mainLineTextWithOptions;

	public GameObject dialogOptionsPanel;
	public RectTransform dialogOptionsPanelRectTransform;
	public Transform dialogOptionsParent;

	public GameObject nextLineButton;
	public GameObject closeDialogButton;

	public ScrollRect dialogOptionsScrollRect;
	public RectTransform dialogOptionsScrollRectRectTransform;

	public GameObject dialogOptionPrefab;

	public RectTransform dialogOptionsWithDialogLinePosition;
	public RectTransform dialogOptionsWithoutDialogLinePosition;

	public RectTransform dialogOptionsListSizeWithDialog;
	public RectTransform dialogOptionsListSizeWithoutDialog;

	public GameObject simpleDialogContent;
	public Text simpleDialogText;

	public AudioSource dialogAudioSource;

	[Space]
	[Header ("Component Elements")]
	[Space]

	public GameObject playerGameObject;
	public usingDevicesSystem usingDevicesManager;
	public playerStatsSystem mainPlayerStatsSystem;
	public menuPause pauseManager;
	public playerController mainPlayerController;

	public Animator mainAnimator;


	remoteEventSystem currentRemoteEventSystem;

	completeDialogInfo currentCompleteDialogInfo;

	List<dialogInfo> currentDialogInfoList = new List<dialogInfo> ();

	dialogLineInfo currentDialogLineInfo;

	dialogInfo currentDialogInfo;

	string currentDialogLine;

	string previousDialogLine;

	Coroutine playDialogCoroutine;

	GameObject currentDialogContentSystemGameObject;

	Coroutine playNextDialogWithoutPausingPlayerShowingDialogLine;

	float lastTimeDialogStarted;

	Animator currentCharacterAnimator;

	bool useAnimations;

	bool playingCharacterAnimation;

	bool playingPlayerAnimation;

	Coroutine characterAnimationCoroutine;

	Coroutine disableDialogCharacterAnimatorCoroutine;


	bool useDialogContentTemplate;

	completeDialogInfoTemplate currentCompleteDialogInfoTemplate;

	simpleCompleteDialogInfo currentSimpleCompleteDialogInfo;

	List<simpleDialogInfo> dialogInfoList = new List<simpleDialogInfo> ();




	void Start ()
	{
		if (dialogPanel.activeSelf) {
			dialogPanel.SetActive (false);
		}

		checkCurrentGameLanguage ();
	}

	void Update ()
	{
		if (dialogActive && stopDialogIfPlayerDistanceTooFar) {
			if (GKC_Utils.distance (currentDialogContentSystemGameObject.transform.position, playerGameObject.transform.position) > maxDistanceToStopDialog) {
				stopPlayDialogWithoutPausingPlayerCoroutine ();

				dialogInProcess = false;

				previousDialogContentSystem.setDialogInProcessState (false);

				if (simpleDialogContent.activeSelf) {
					simpleDialogContent.SetActive (false);
				}

				previousDialogContentSystem.activateEventOnDialogStopped ();
			
				closeDialog ();

				if (!rewindLastDialogIfStopped) {
					currentDialogContentSystem.setNextCompleteDialogIndex ();
				}
			}
		}
	}

	void checkCurrentGameLanguage ()
	{
		currentLanguage = GKC_Utils.getCurrentLanguage ();
	}

	public void setNewDialogContent (dialogContentSystem newDialogContent)
	{
		checkCurrentGameLanguage ();

		dialogActive = true;

		if (pauseManager != null) {
			pauseManager.setSelectedUIGameObject (null);
		}

		currentDialogContentSystem = newDialogContent;

		currentDialogContentSystemGameObject = newDialogContent.gameObject;

		if (previousDialogContentSystem != currentDialogContentSystem) {
			if (dialogInProcess) {
				stopPlayDialogWithoutPausingPlayerCoroutine ();

				dialogInProcess = false;

				previousDialogContentSystem.setDialogInProcessState (false);

				if (simpleDialogContent.activeSelf) {
					simpleDialogContent.SetActive (false);
				}

				previousDialogContentSystem.activateEventOnDialogStopped ();
			}

			previousDialogContentSystem = currentDialogContentSystem;
		} else {
			if (dialogInProcess) {
				if (!currentDialogContentSystem.playingExternalDialog) {
					return;
				}
			}
		}

		currentDialogContentIndex = 0;

		currentCompleteDialogInfo = currentDialogContentSystem.completeDialogInfoList [currentDialogContentSystem.currentDialogIndex];

		currentDialogInfoList = currentCompleteDialogInfo.dialogInfoList;

		currentDialogInfo = currentDialogInfoList [currentDialogContentIndex];

		useDialogContentTemplate = currentDialogContentSystem.useDialogContentTemplate;



		if (useDialogContentTemplate) {
			currentCompleteDialogInfoTemplate = currentDialogContentSystem.getCompleteDialogInfoTemplateByLanguageName (currentLanguage);

			currentSimpleCompleteDialogInfo = currentCompleteDialogInfoTemplate.completeDialogInfoList [currentDialogContentSystem.currentDialogIndex];
		
			dialogInfoList = currentSimpleCompleteDialogInfo.dialogInfoList;
		}



		if (currentDialogContentSystem.dialogOwner != null) {
			currentRemoteEventSystem = currentDialogContentSystem.dialogOwner.GetComponent<remoteEventSystem> ();
		}

		playDialogWithoutPausingPlayerActions = currentCompleteDialogInfo.playDialogWithoutPausingPlayerActions;

		showDialogLineWordByWord = currentCompleteDialogInfo.showDialogLineWordByWord;

		showDialogLineLetterByLetter = currentCompleteDialogInfo.showDialogLineLetterByLetter;

		playDialogsAutomatically = currentCompleteDialogInfo.playDialogsAutomatically;

		canUseInputToSetNextDialog = currentCompleteDialogInfo.canUseInputToSetNextDialog;

		showFullDialogLineOnInputIfTextPartByPart = currentCompleteDialogInfo.showFullDialogLineOnInputIfTextPartByPart;

		useCustomTextAnchorAndAligment = currentCompleteDialogInfo.useCustomTextAnchorAndAligment;

		stopDialogIfPlayerDistanceTooFar = currentCompleteDialogInfo.stopDialogIfPlayerDistanceTooFar;
		maxDistanceToStopDialog = currentCompleteDialogInfo.maxDistanceToStopDialog;
		rewindLastDialogIfStopped = currentCompleteDialogInfo.rewindLastDialogIfStopped;
	

		useAnimations = currentDialogContentSystem.useAnimations;

		if (useAnimations) {
			currentCharacterAnimator = currentDialogContentSystem.mainAnimator;
		}

		if (playDialogWithoutPausingPlayerActions) {
			playDialogWithoutPausingPlayer ();

			if (currentCompleteDialogInfo.pausePlayerMovementInput) {
				
				mainPlayerController.setCanMoveState (false);
			
				mainPlayerController.resetPlayerControllerInput ();
						
				mainPlayerController.resetOtherInputFields ();
			}

			if (currentCompleteDialogInfo.pausePlayerActionsInput) {
				eventOnPausePlayerActionsInput.Invoke ();
			}
		} else {
			usingDevicesManager.setUseDeviceButtonEnabledState (false);

			if (!dialogPanel.activeSelf) {
				dialogPanel.SetActive (true);
			}

			updateDialogContent ();
		}

		lastTimeDialogStarted = Time.time;

		currentDialogContentSystem.checkEventsOnDialog (true);

		currentDialogContentSystem.checkIfPauseOrResumeAI (true);
	}

	public void playDialogWithoutPausingPlayer ()
	{
		if (playDialogsAutomatically) {
			stopPlayDialogWithoutPausingPlayerCoroutine ();

			playDialogCoroutine = StartCoroutine (playDialogWithoutPausingPlayerCoroutine ());
		} else {
			currentDialogContentIndex = -1;

			playDialogWithoutPausingPlayerOneByOne ();
		}
	}

	public void inputPlayNextDialogWioutPausingPlayer ()
	{
		if (!canUseInputToSetNextDialog) {
			return;
		}

		if (!dialogInProcess && playDialogWithoutPausingPlayerActions) {
			return;
		}

		if (showDebugPrint) {
			print ("next dialog");
		}

		if (Time.time > lastTimeDialogStarted + 0.5f) {
			if (playDialogWithoutPausingPlayerActions) {
				if (showDialogLineWordByWord || showDialogLineLetterByLetter) {
					stopPlayDialogWithoutPausingPlayerCoroutine ();

					if (textingShowingPartByPart && showFullDialogLineOnInputIfTextPartByPart) {
					
						stopPlayNextDialogWithoutPausingPlayerShowingDialogLinePartByPartCoroutine ();

						string dialogOwnerName = "";

						string dialogContent = "";

						if (useDialogContentTemplate) {
							dialogOwnerName = dialogInfoList [currentDialogContentIndex].dialogOwnerName;
							dialogContent = dialogInfoList [currentDialogContentIndex].dialogContent;
						} else {
							dialogOwnerName = currentDialogInfoList [currentDialogContentIndex].dialogOwnerName;
							dialogContent = currentDialogInfoList [currentDialogContentIndex].dialogContent;
						}

						string textContent = dialogOwnerName + ": " + dialogContent;

						simpleDialogText.text = textContent;
					} else {
						playNextDialogWithoutPausingPlayerShowingDialogLinePartByPart ();
					}
				} else {
					playNextDialogWithoutPausingPlayer ();
				}
			} else {
				if (dialogActive) {
					if (currentDialogInfoList [currentDialogContentIndex].isEndOfDialog) {
						closeDialog ();
					} else {
						if (currentDialogInfoList [currentDialogContentIndex].useNexLineButton) {
							setNextDialog ();
						}
					}
				}
			}
		}
	}

	public void playDialogWithoutPausingPlayerOneByOne ()
	{
		if (dialogInProcess) {
			stopPlayDialogWithoutPausingPlayerCoroutine ();
		}

		currentDialogContentSystem.setDialogInProcessState (true);

		dialogInProcess = true;

		if (showDialogLineWordByWord || showDialogLineLetterByLetter) {
			playNextDialogWithoutPausingPlayerShowingDialogLinePartByPart ();
		} else {
			playNextDialogWithoutPausingPlayer ();
		}
	}

	public void playNextDialogWithoutPausingPlayer ()
	{
		if (dialogInProcess) {

			currentDialogContentIndex++;

			if (currentDialogContentIndex >= currentDialogInfoList.Count) {
				closeDialog ();

				dialogInProcess = false;

				currentDialogContentSystem.setDialogInProcessState (false);
			
				return;
			}

			updateDialogContent ();
		}
	}

	public void playNextDialogWithoutPausingPlayerShowingDialogLinePartByPart ()
	{
		if (dialogInProcess) {
			currentDialogContentIndex++;

			if (currentDialogContentIndex >= currentDialogInfoList.Count) {
				closeDialog ();

				dialogInProcess = false;

				currentDialogContentSystem.setDialogInProcessState (false);

				return;
			}

			stopPlayNextDialogWithoutPausingPlayerShowingDialogLinePartByPartCoroutine ();

			playNextDialogWithoutPausingPlayerShowingDialogLine = StartCoroutine (playNextDialogWithoutPausingPlayerShowingDialogLinePartByPartCoroutine ());
		}
	}

	public void stopPlayNextDialogWithoutPausingPlayerShowingDialogLinePartByPartCoroutine ()
	{
		textingShowingPartByPart = false;

		if (playNextDialogWithoutPausingPlayerShowingDialogLine != null) {
			StopCoroutine (playNextDialogWithoutPausingPlayerShowingDialogLine);
		}
	}

	IEnumerator playNextDialogWithoutPausingPlayerShowingDialogLinePartByPartCoroutine ()
	{
		textingShowingPartByPart = true;

		yield return new WaitForSeconds (currentDialogInfoList [currentDialogContentIndex].delayToShowThisDialogLine);

		if (showDialogLineWordByWord || showDialogLineLetterByLetter) {
			if (useDialogContentTemplate) {
				currentDialogLine = dialogInfoList [currentDialogContentIndex].dialogContent; 
			} else {
				currentDialogLine = currentDialogInfoList [currentDialogContentIndex].dialogContent;
			}
		}

		updateDialogContent ();

		float currentWaitTime = 0;

		if (showDialogLineWordByWord || showDialogLineLetterByLetter) {
			if (showDialogLineWordByWord) {
				string dialogOwnerName = "";

				if (useDialogContentTemplate) {
					dialogOwnerName = dialogInfoList [currentDialogContentIndex].dialogOwnerName;
				} else {
					dialogOwnerName = currentDialogInfoList [currentDialogContentIndex].dialogOwnerName;
				}

				string textContent = dialogOwnerName + ": ";
				
				simpleDialogText.text = textContent;

				string[] words = Regex.Split (currentDialogLine, @"(?<=[.,;])");

				foreach (string word in words) {
					simpleDialogText.text += " " + word;

					currentWaitTime += currentCompleteDialogInfo.dialogLineWordSpeed;

					yield return new WaitForSeconds (currentCompleteDialogInfo.dialogLineWordSpeed);
				}
			}

			if (showDialogLineLetterByLetter) {
				for (int j = 0; j < currentDialogLine.Length; j++) {
					string dialogOwnerName = "";

					if (useDialogContentTemplate) {
						dialogOwnerName = dialogInfoList [currentDialogContentIndex].dialogOwnerName;
					} else {
						dialogOwnerName = currentDialogInfoList [currentDialogContentIndex].dialogOwnerName;
					}

					string textContent = dialogOwnerName + ": " + currentDialogLine.Substring (0, j);

					simpleDialogText.text = textContent;

					currentWaitTime += currentCompleteDialogInfo.dialogLineLetterSpeed;

					yield return new WaitForSeconds (currentCompleteDialogInfo.dialogLineLetterSpeed);
				}
			}

			string lastCharacterToChecK = "(?<=[.,;])";

			string lastCharacter = currentDialogLine.Substring (currentDialogLine.Length - 1);

			if (lastCharacterToChecK.Contains (lastCharacter)) {
				string textContent = lastCharacter;
					
				simpleDialogText.text += textContent;

				currentWaitTime += currentCompleteDialogInfo.dialogLineWordSpeed;

				yield return new WaitForSeconds (0.1f);
			}
		}

		if (currentWaitTime > 0) {
			currentWaitTime = Mathf.Abs (currentDialogInfoList [currentDialogContentIndex].delayToShowNextDialogLine - currentWaitTime);
		} else {
			currentWaitTime = currentDialogInfoList [currentDialogContentIndex].delayToShowNextDialogLine;
		}

		yield return new WaitForSeconds (currentWaitTime);

		textingShowingPartByPart = false;
	}

	public void stopPlayDialogWithoutPausingPlayerCoroutine ()
	{
		if (playDialogCoroutine != null) {
			StopCoroutine (playDialogCoroutine);
		}
	}

	IEnumerator playDialogWithoutPausingPlayerCoroutine ()
	{
		currentDialogContentSystem.setDialogInProcessState (true);

		dialogInProcess = true;

		textingShowingPartByPart = true;

		for (int i = 0; i < currentDialogInfoList.Count; i++) {
			yield return new WaitForSeconds (currentDialogInfoList [i].delayToShowThisDialogLine);

			if (showDialogLineWordByWord || showDialogLineLetterByLetter) {
				if (useDialogContentTemplate) {
					currentDialogLine = dialogInfoList [currentDialogContentIndex].dialogContent;
				} else {
					currentDialogLine = currentDialogInfoList [currentDialogContentIndex].dialogContent;
				}
			}

			updateDialogContent ();

			float currentWaitTime = 0;

			if (showDialogLineWordByWord || showDialogLineLetterByLetter) {
				if (showDialogLineWordByWord) {
					string dialogOwnerName = "";

					if (useDialogContentTemplate) {
						dialogOwnerName = dialogInfoList [currentDialogContentIndex].dialogOwnerName;
					} else {
						dialogOwnerName = currentDialogInfoList [currentDialogContentIndex].dialogOwnerName;
					}

					string textContent = dialogOwnerName + ": ";

					simpleDialogText.text = textContent;

					string[] words = Regex.Split (currentDialogLine, @"(?<=[.,;])");

					foreach (string word in words) {
						simpleDialogText.text += " " + word;

						currentWaitTime += currentCompleteDialogInfo.dialogLineWordSpeed;

						yield return new WaitForSeconds (currentCompleteDialogInfo.dialogLineWordSpeed);
					}
				}

				if (showDialogLineLetterByLetter) {
					for (int j = 0; j < currentDialogLine.Length; j++) {
						string dialogOwnerName = "";

						if (useDialogContentTemplate) {
							dialogOwnerName = dialogInfoList [currentDialogContentIndex].dialogOwnerName;
						} else {
							dialogOwnerName = currentDialogInfoList [currentDialogContentIndex].dialogOwnerName;
						}

						string textContent = dialogOwnerName + ": " + currentDialogLine.Substring (0, j);

						simpleDialogText.text = textContent;

						currentWaitTime += currentCompleteDialogInfo.dialogLineLetterSpeed;

						yield return new WaitForSeconds (currentCompleteDialogInfo.dialogLineLetterSpeed);
					}
				}

				string lastCharacterToChecK = "(?<=[.,;])";

				string lastCharacter = currentDialogLine.Substring (currentDialogLine.Length - 1);

				if (lastCharacterToChecK.Contains (lastCharacter)) {
					simpleDialogText.text += lastCharacter;

					currentWaitTime += currentCompleteDialogInfo.dialogLineWordSpeed;

					yield return new WaitForSeconds (0.1f);
				}
			}

			if (currentWaitTime > 0) {
				currentWaitTime = Mathf.Abs (currentDialogInfoList [i].delayToShowNextDialogLine - currentWaitTime);
			} else {
				currentWaitTime = currentDialogInfoList [i].delayToShowNextDialogLine;
			}

			yield return new WaitForSeconds (currentWaitTime);

			currentDialogContentIndex++;

			if (currentDialogContentIndex >= currentDialogInfoList.Count) {
				i = currentDialogInfoList.Count;
			}
		}

		playDialogWithoutPausingPlayerActions = currentCompleteDialogInfo.playDialogWithoutPausingPlayerActions;

		closeDialog ();

		dialogInProcess = false;

		textingShowingPartByPart = false;

		currentDialogContentSystem.setDialogInProcessState (false);

		yield return null;
	}

	public void setNextDialog ()
	{
		if (currentDialogInfo.disableDialogAfterSelect) {
			currentDialogInfoList [currentDialogContentIndex].dialogInfoDisabled = true;
		}

		currentDialogInfo = currentDialogInfoList [currentDialogContentIndex];

		if (currentDialogInfo.changeToDialogInfoID) {
			
			if (currentDialogInfo.useRandomDialogInfoID) {
				int dialogContentIndex = 0;

				if (currentDialogInfo.useRandomDialogRange) {
					dialogContentIndex = (int)Random.Range ((float)currentDialogInfo.randomDialogRange.x, (float)currentDialogInfo.randomDialogRange.y);
				} else {
					bool valueFound = false;

					while (!valueFound) {
						dialogContentIndex = 
							(int)Random.Range ((float)currentDialogInfo.randomDialogIDList [0], (float)currentDialogInfo.randomDialogIDList [currentDialogInfo.randomDialogIDList.Count - 1] + 1);

						if (currentDialogInfo.randomDialogIDList.Contains (dialogContentIndex)) {
							valueFound = true;

							if (dialogContentIndex > currentDialogInfo.randomDialogIDList [currentDialogInfo.randomDialogIDList.Count - 1]) {
								dialogContentIndex--;
							}
						}
					}
				}

				currentDialogContentIndex = dialogContentIndex;

			} else {
				currentDialogContentIndex = currentDialogInfoList [currentDialogContentIndex].dialogInfoIDToActivate;
			}
				
			if (currentDialogInfoList [currentDialogContentIndex].dialogInfoDisabled) {
				currentDialogContentIndex = currentDialogInfoList [currentDialogContentIndex].dialogInfoIDToJump;
			}

			updateDialogContent ();

			return;
		} else {
			if (currentDialogInfo.checkConditionForNextLine) {

				print ("checking dialog condition");

				if (currentDialogInfo.useEventToSendPlayerToCondition) {
					currentDialogInfo.eventToSendPlayerToCondition.Invoke (playerGameObject);
				}

				currentDialogInfo.eventToCheckConditionForNextLine.Invoke ();

				return;
			} 
		}

		currentDialogContentIndex++;

		if (currentDialogContentIndex < currentDialogInfoList.Count) {
			updateDialogContent ();
		} else {
			closeDialog ();
		}
	}

	public void setNextDialogWithConditionCheckResult (bool state)
	{
		print ("dialog condition result is " + state);

		currentDialogInfo = currentDialogInfoList [currentDialogContentIndex];

		if (state) {
			currentDialogContentIndex = currentDialogInfo.dialogInfoIDToActivateOnConditionTrue;
		} else {
			currentDialogContentIndex = currentDialogInfo.dialogInfoIDToActivateOnConditionFalse;
		}

		if (currentDialogContentIndex < currentDialogInfoList.Count) {
			updateDialogContent ();
		} else {
			closeDialog ();
		}
	}

	public void closeDialog ()
	{
		if (playDialogWithoutPausingPlayerActions) {

			if (simpleDialogContent.activeSelf) {
				simpleDialogContent.SetActive (false);
			}

			stopPlayDialogWithoutPausingPlayerCoroutine ();

			if (currentCompleteDialogInfo != null && currentCompleteDialogInfo.pausePlayerMovementInput) {

				mainPlayerController.setCanMoveState (true);
			}

			if (currentCompleteDialogInfo.pausePlayerActionsInput) {
				eventOnResumePlayerActionsInput.Invoke ();
			}
		} else {
			usingDevicesManager.setUseDeviceButtonEnabledState (true);

			if (dialogPanel.activeSelf) {
				dialogPanel.SetActive (false);
			}

			usingDevicesManager.useCurrentDevice (currentDialogContentSystemGameObject);
		}
			
		dialogActive = false;

		if (currentDialogContentIndex == currentDialogInfoList.Count) {
			currentDialogContentIndex--;
		}

		if (currentDialogContentIndex < currentDialogInfoList.Count) {
			if (currentDialogInfoList [currentDialogContentIndex].activateWhenDialogClosed) {
				checkDialogEvents ();
			}

			if (currentDialogInfoList [currentDialogContentIndex].setNextCompleteDialogID) {
				currentDialogContentSystem.setNextCompleteDialogIndex ();
			} else {
				if (currentDialogInfoList [currentDialogContentIndex].setNewCompleteDialogID) {
					currentDialogContentSystem.setCompleteDialogIndex (currentDialogInfoList [currentDialogContentIndex].newCompleteDialogID);
				}
			}
		}

		if (dialogAudioSource.isPlaying) {
			dialogAudioSource.Stop ();
		}

		if (currentDialogContentSystem.playingExternalDialog) {
			currentDialogContentSystem.setPlayingExternalDialogState (false);
		}

		playDialogWithoutPausingPlayerActions = false;

		disableDialogComponents ();

		currentDialogContentSystem.checkEventsOnDialog (false);

		currentDialogContentSystem.checkIfPauseOrResumeAI (false);
	}

	public void setDialogContentAnswer (Button buttonToCheck)
	{
		dialogInfo temporalDialogInfo = currentDialogInfoList [currentDialogContentIndex];
			
		for (int i = 0; i < temporalDialogInfo.dialogLineInfoList.Count; i++) {
			currentDialogLineInfo = temporalDialogInfo.dialogLineInfoList [i];

			if (currentDialogLineInfo.dialogLineButton == buttonToCheck) {

				if (currentDialogLineInfo.answerNotAvailable) {
					if (showDebugPrint) {
						print ("answer not available");
					}

					return;
				}

				if (temporalDialogInfo.disableDialogAfterSelect) {
					temporalDialogInfo.dialogInfoDisabled = true;
				}

				if (currentDialogLineInfo.useRandomDialogInfoID) {
					int dialogContentIndex = -1;

					if (currentDialogLineInfo.useRandomDialogRange) {
						dialogContentIndex = (int)Random.Range ((float)currentDialogLineInfo.randomDialogRange.x, (float)currentDialogLineInfo.randomDialogRange.y);
					} else {
						bool valueFound = false;

						while (!valueFound) {
							dialogContentIndex = 
							(int)Random.Range ((float)currentDialogLineInfo.randomDialogIDList [0], 
								(float)currentDialogLineInfo.randomDialogIDList [currentDialogLineInfo.randomDialogIDList.Count - 1] + 1);

							if (currentDialogLineInfo.randomDialogIDList.Contains (dialogContentIndex)) {
								valueFound = true;

								if (dialogContentIndex > currentDialogLineInfo.randomDialogIDList [currentDialogLineInfo.randomDialogIDList.Count - 1]) {
									dialogContentIndex--;
								}
							}
						}
					}
					currentDialogContentIndex = dialogContentIndex;
				
				} else {
					if (currentDialogLineInfo.checkConditionForNextLine) {
						if (currentDialogLineInfo.disableLineAfterSelect) {
							currentDialogLineInfo.lineDisabled = true;
						}

						currentDialogAnswerIndex = i;

//						print ("checking dialog condition");

						if (currentDialogLineInfo.useEventToSendPlayerToCondition) {
							currentDialogLineInfo.eventToSendPlayerToCondition.Invoke (playerGameObject);
						}

						currentDialogLineInfo.eventToCheckConditionForNextLine.Invoke ();

						return;
					} else {
						currentDialogContentIndex = currentDialogLineInfo.dialogInfoIDToActivate;
					}
				}

				if (currentDialogLineInfo.disableLineAfterSelect) {
					currentDialogLineInfo.lineDisabled = true;
				}

				updateDialogContent ();
			}
		}
	}

	int currentDialogAnswerIndex = -1;


	public void setDialogAnswerConditionCheckResult (bool state)
	{
		if (currentDialogAnswerIndex == -1) {
			return;
		}

//		print ("dialog condition result is " + state);

		dialogInfo temporalDialogInfo = currentDialogInfoList [currentDialogContentIndex];

		currentDialogLineInfo = temporalDialogInfo.dialogLineInfoList [currentDialogAnswerIndex];

		if (state) {
			currentDialogContentIndex = currentDialogLineInfo.dialogInfoIDToActivateOnConditionTrue;
		} else {
			currentDialogContentIndex = currentDialogLineInfo.dialogInfoIDToActivateOnConditionFalse;
		}

		currentDialogAnswerIndex = -1;

		updateDialogContent ();
	}

	public void checkDialogEvents ()
	{
		dialogInfo temporalDialogInfo = currentDialogInfoList [currentDialogContentIndex];

		if (temporalDialogInfo.useEventToSendPlayer) {
			temporalDialogInfo.eventToSendPlayer.Invoke (playerGameObject);
		}

		temporalDialogInfo.eventOnDialog.Invoke ();

		if (temporalDialogInfo.activateRemoteTriggerSystem) {
			if (currentRemoteEventSystem != null) {
				currentRemoteEventSystem.callRemoteEventWithGameObject (temporalDialogInfo.remoteTriggerName, playerGameObject);
			}
		}
	}

	public void updateDialogContent ()
	{
		GameObject newSelectedUIGameObject = null;

		if (currentDialogContentIndex >= currentDialogInfoList.Count) {
			print ("WARNING: Dialog index not configured properly for an ID configured as " + currentDialogContentIndex + " \n" +
			"Make sure to adjust this dialog content with the proper values");

			currentDialogContentIndex = 0;

			return;
		}
			
		dialogInfo temporalDialogInfo = currentDialogInfoList [currentDialogContentIndex];

		int currentOptionsAmount = temporalDialogInfo.dialogLineInfoList.Count;
	
		bool currentLineHasOptions = currentOptionsAmount > 0;

		string textContent = "";

		if (useDialogContentTemplate) {
			textContent = dialogInfoList [currentDialogContentIndex].dialogContent;
		} else {
			textContent = temporalDialogInfo.dialogContent;
		}

		mainLineText.text = textContent;

		if (currentLineHasOptions) {
			mainLineText.transform.position = mainLineTextWithOptions.position;
		} else {
			mainLineText.transform.position = mainLineTextWithoutOptions.position;
		}

		if (dialogContentWithoutOptions.activeSelf != (!currentLineHasOptions)) {
			dialogContentWithoutOptions.SetActive (!currentLineHasOptions);
		}

		if (dialogContentWithOptions.activeSelf != currentLineHasOptions) {
			dialogContentWithOptions.SetActive (currentLineHasOptions);
		}

		if (simpleDialogContent.activeSelf != playDialogWithoutPausingPlayerActions) {
			simpleDialogContent.SetActive (playDialogWithoutPausingPlayerActions);
		}

		if (playDialogWithoutPausingPlayerActions) {
			string dialogOwnerName = "";

			string dialogContent = "";

			if (useDialogContentTemplate) {
				dialogOwnerName = dialogInfoList [currentDialogContentIndex].dialogOwnerName;
				dialogContent = dialogInfoList [currentDialogContentIndex].dialogContent; 
			} else {
				dialogOwnerName = temporalDialogInfo.dialogOwnerName;
				dialogContent = temporalDialogInfo.dialogContent;
			}

			textContent = dialogOwnerName + ": " + dialogContent;

			simpleDialogText.text = textContent;
		}

		if (useCustomTextAnchorAndAligment) {
			if (playDialogWithoutPausingPlayerActions) {
				simpleDialogText.alignment = currentCompleteDialogInfo.textAnchor;
			} else {
				mainLineText.alignment = currentCompleteDialogInfo.textAnchor;
			}
		}
		
		if (currentLineHasOptions) {
			if (temporalDialogInfo.showPreviousDialogLineOnOptions || temporalDialogInfo.dialogContent != "") {
				if (temporalDialogInfo.showPreviousDialogLineOnOptions) {
					textContent = previousDialogLine;
						
					mainLineText.text = textContent;
				}

				dialogOptionsPanelRectTransform.anchoredPosition = dialogOptionsWithDialogLinePosition.anchoredPosition;
				dialogOptionsPanelRectTransform.sizeDelta = dialogOptionsWithDialogLinePosition.sizeDelta;

				dialogOptionsScrollRectRectTransform.anchoredPosition = dialogOptionsListSizeWithDialog.anchoredPosition;
				dialogOptionsScrollRectRectTransform.sizeDelta = dialogOptionsListSizeWithDialog.sizeDelta;
			} else {
				dialogOptionsPanelRectTransform.anchoredPosition = dialogOptionsWithoutDialogLinePosition.anchoredPosition;
				dialogOptionsPanelRectTransform.sizeDelta = dialogOptionsWithoutDialogLinePosition.sizeDelta;

				dialogOptionsScrollRectRectTransform.anchoredPosition = dialogOptionsListSizeWithoutDialog.anchoredPosition;
				dialogOptionsScrollRectRectTransform.sizeDelta = dialogOptionsListSizeWithoutDialog.sizeDelta;
			}
		}

		if (currentDialogContentSystem.showDialogOnwerName) {
			string dialogOwnerName = "";

			if (useDialogContentTemplate) {
				dialogOwnerName = dialogInfoList [currentDialogContentIndex].dialogOwnerName;
			} else {
				dialogOwnerName = temporalDialogInfo.dialogOwnerName;
			}

			textContent = dialogOwnerName;

			currentDialogOwnerNameText.text = textContent; 
		}

		if (temporalDialogInfo.isEndOfDialog) {
			if (!closeDialogButton.activeSelf) {
				closeDialogButton.SetActive (true);
			}

			if (nextLineButton.activeSelf) {
				nextLineButton.SetActive (false);
			}

			newSelectedUIGameObject = closeDialogButton;
		} else {
			if (temporalDialogInfo.useNexLineButton) {
				if (!nextLineButton.activeSelf) {
					nextLineButton.SetActive (true);
				}

				newSelectedUIGameObject = nextLineButton;
			} else {
				if (nextLineButton.activeSelf) {
					nextLineButton.SetActive (false);
				}
			}

			if (closeDialogButton.activeSelf) {
				closeDialogButton.SetActive (false);
			}
		}

		if (currentLineHasOptions) {
			if (!dialogOptionsPanel.activeSelf) {
				dialogOptionsPanel.SetActive (true);
			}

			if (dialogPanelInfoList.Count < currentOptionsAmount) {
				int newAmountOfOptions = currentOptionsAmount - dialogPanelInfoList.Count;

				for (int i = 0; i < newAmountOfOptions; i++) {
					GameObject newDialogOption = (GameObject)Instantiate (dialogOptionPrefab, Vector3.zero, Quaternion.identity, dialogOptionsParent);
					newDialogOption.SetActive (true);

					newDialogOption.transform.localScale = Vector3.one;
					newDialogOption.transform.localPosition = Vector3.zero;

					dialogPanelInfo newDialogPanelInfo = new dialogPanelInfo ();

					newDialogPanelInfo.dialogOptionGameObject = newDialogOption;
					newDialogPanelInfo.dialogOptionButton = newDialogOption.GetComponent<Button> ();
					newDialogPanelInfo.dialogOptionText = newDialogOption.GetComponentInChildren<Text> ();

					dialogPanelInfoList.Add (newDialogPanelInfo);
				}

				for (int i = 0; i < dialogPanelInfoList.Count; i++) {
					if (!dialogPanelInfoList [i].dialogOptionGameObject.activeSelf) {
						dialogPanelInfoList [i].dialogOptionGameObject.SetActive (true);
					}
				}
			} else if (dialogPanelInfoList.Count > currentOptionsAmount) {
				for (int i = 0; i < dialogPanelInfoList.Count; i++) {
					if (i > currentOptionsAmount) {
						if (dialogPanelInfoList [i].dialogOptionGameObject.activeSelf) {
							dialogPanelInfoList [i].dialogOptionGameObject.SetActive (false);
						}
					}
				}
			}

			//Check if the dialog lines/answer can be used or not through the stats system
			for (int i = 0; i < temporalDialogInfo.dialogLineInfoList.Count; i++) {
				if (temporalDialogInfo.dialogLineInfoList [i].useStatToShowLine) {
					bool answerNotAvailable = false;

					string extraDialogLineContent = "";

					string statName = temporalDialogInfo.dialogLineInfoList [i].statName;

					if (temporalDialogInfo.dialogLineInfoList [i].statIsAmount) {
						float currentStateValue = mainPlayerStatsSystem.getStatValue (statName);

						answerNotAvailable = (currentStateValue >= temporalDialogInfo.dialogLineInfoList [i].minStateValue);

						extraDialogLineContent += " (" + statName + " -> " + currentStateValue + " )";
					} else {
						bool currentStateValue = mainPlayerStatsSystem.getBoolStatValue (statName);

						answerNotAvailable = (currentStateValue == temporalDialogInfo.dialogLineInfoList [i].boolStateValue);

						extraDialogLineContent += " (" + statName + " -> " + currentStateValue + " )";
					}

					temporalDialogInfo.dialogLineInfoList [i].extraDialogLineContent = extraDialogLineContent;
					temporalDialogInfo.dialogLineInfoList [i].answerNotAvailable = !answerNotAvailable;
				}
			}

			for (int i = 0; i < currentOptionsAmount; i++) {
				temporalDialogInfo.dialogLineInfoList [i].dialogLineButton = dialogPanelInfoList [i].dialogOptionButton;

				if (useDialogContentTemplate) {
					textContent = dialogInfoList [currentDialogContentIndex].dialogLineInfoList [i].dialogLineContent;
				} else {
					textContent = temporalDialogInfo.dialogLineInfoList [i].dialogLineContent;
				}

				dialogPanelInfoList [i].dialogOptionText.text = textContent;

				if (!dialogPanelInfoList [i].dialogOptionGameObject.activeSelf) {
					dialogPanelInfoList [i].dialogOptionGameObject.SetActive (true);
				}

				if (temporalDialogInfo.dialogLineInfoList [i].useStatToShowLine) {
					Color currentAnswerTextColor = availableAnswerColor;

					if (temporalDialogInfo.dialogLineInfoList [i].answerNotAvailable) {
						currentAnswerTextColor = notAvailableAnswerColor;
					}

					dialogPanelInfoList [i].dialogOptionText.color = currentAnswerTextColor;

					textContent = temporalDialogInfo.dialogLineInfoList [i].extraDialogLineContent;

					dialogPanelInfoList [i].dialogOptionText.text += textContent;
				} else {
					dialogPanelInfoList [i].dialogOptionText.color = regularAnswerColor;
				}
			}

			for (int i = currentOptionsAmount; i < dialogPanelInfoList.Count; i++) {
				if (dialogPanelInfoList [i].dialogOptionGameObject.activeSelf) {
					dialogPanelInfoList [i].dialogOptionGameObject.SetActive (false);
				}
			}
				
		} else {
			if (dialogOptionsPanel.activeSelf) {
				dialogOptionsPanel.SetActive (false);
			}
		}

		for (int i = 0; i < temporalDialogInfo.dialogLineInfoList.Count; i++) {
			if (temporalDialogInfo.dialogLineInfoList [i].lineDisabled) {
				if (temporalDialogInfo.dialogLineInfoList [i].dialogLineButton != null) {
					if (temporalDialogInfo.dialogLineInfoList [i].dialogLineButton.gameObject.activeSelf) {
						temporalDialogInfo.dialogLineInfoList [i].dialogLineButton.gameObject.SetActive (false);
					}
				}
			} else {
				if (newSelectedUIGameObject == null) {
					if (temporalDialogInfo.dialogLineInfoList [i].dialogLineButton != null) {
						newSelectedUIGameObject = temporalDialogInfo.dialogLineInfoList [i].dialogLineButton.gameObject;
					}
				}
			}
		}

		dialogOptionsScrollRect.verticalNormalizedPosition = 0f;
		dialogOptionsScrollRect.horizontalNormalizedPosition = 0.5f;
		dialogOptionsScrollRect.horizontalNormalizedPosition = 0.5f;

		if (!temporalDialogInfo.activateWhenDialogClosed) {
			checkDialogEvents ();
		}

		if (useDialogContentTemplate) {
			previousDialogLine = dialogInfoList [currentDialogContentIndex].dialogContent;
		} else {
			previousDialogLine = temporalDialogInfo.dialogContent;
		}

		if (dialogAudioSource.isPlaying) {
			dialogAudioSource.Stop ();
		}

		if (temporalDialogInfo.useDialogLineSound) {
			AudioClip dialogLineSound = temporalDialogInfo.dialogLineSound;

			if (useDialogContentTemplate) {
				dialogLineSound = dialogInfoList [currentDialogContentIndex].dialogLineSound;
			}

			if (dialogLineSound != null) {
				dialogAudioSource.PlayOneShot (dialogLineSound);
			}
		}

		if (useAnimations) {
			if (temporalDialogInfo.animationName != "") {
				if (characterAnimationCoroutine != null) {
					StopCoroutine (characterAnimationCoroutine);
				}

				characterAnimationCoroutine = StartCoroutine (
					playDialogCharacterAnimationCoroutine (temporalDialogInfo.animationName, temporalDialogInfo.delayToPlayAnimation, temporalDialogInfo.animationUsedOnPlayer));
			}
		}

		if (pauseManager != null) {
			pauseManager.setSelectedUIGameObject (newSelectedUIGameObject);
		}
	}

	IEnumerator playDialogCharacterAnimationCoroutine (string animationName, float delayToPlayAnimation, bool animationUsedOnPlayer)
	{
		if (!playingCharacterAnimation) {
			currentDialogContentSystem.stopDisableDialogCharacterAnimatorStateCoroutine ();

			currentDialogContentSystem.setDialogAnimatorState (true);

			playingCharacterAnimation = true;
		}

		if (animationUsedOnPlayer && currentDialogContentSystem.playerAnimationsOnDialogEnabled) {
			if (!playingPlayerAnimation) {
				stopDisableDialogCharacterAnimatorStateCoroutine ();

				if (animationUsedOnPlayer) {
					setDialogAnimatorState (true);
				}

				playingPlayerAnimation = true;
			}
		}
			
		yield return new WaitForSeconds (delayToPlayAnimation);

		if (animationUsedOnPlayer) {
			if (currentDialogContentSystem.playerAnimationsOnDialogEnabled) {
				mainAnimator.CrossFade (animationName, 0.1f);
			}
		} else {
			if (currentCharacterAnimator != null) {
				currentCharacterAnimator.CrossFade (animationName, 0.1f);
			}
		}
	}

	void disableDialogComponents ()
	{
		if (playingCharacterAnimation) {
			if (currentDialogInfo.useDelayToDisableAnimation) {
				currentDialogContentSystem.disableDialogCharacterAnimatorState (currentDialogInfo.delayToDisableAnimation);
			} else {
				currentDialogContentSystem.setDialogAnimatorState (false);
			}

			playingCharacterAnimation = false;
		}

		if (playingPlayerAnimation) {
			if (currentDialogInfo.useDelayToDisableAnimation) {
				disableDialogCharacterAnimatorState (currentDialogInfo.delayToDisableAnimation);
			} else {
				setDialogAnimatorState (false);
			}

			playingPlayerAnimation = false;
		}
	}


	public void disableDialogCharacterAnimatorState (float delayAmount)
	{
		if (!useAnimations) {
			return;
		}

		stopDisableDialogCharacterAnimatorStateCoroutine ();

		disableDialogCharacterAnimatorCoroutine = StartCoroutine (disableDialogCharacterAnimatorStateCoroutine (delayAmount));
	}

	public void stopDisableDialogCharacterAnimatorStateCoroutine ()
	{
		if (disableDialogCharacterAnimatorCoroutine != null) {
			StopCoroutine (disableDialogCharacterAnimatorCoroutine);
		}
	}

	IEnumerator disableDialogCharacterAnimatorStateCoroutine (float delayAmount)
	{
		yield return new WaitForSeconds (delayAmount);

		setDialogAnimatorState (false);
	}

	public void setDialogAnimatorState (bool state)
	{
		mainAnimator.SetBool (dialogueActiveAnimatorName, state);

		mainPlayerController.setApplyRootMotionAlwaysActiveState (state);
	}


	[System.Serializable]
	public class dialogPanelInfo
	{
		public string Name;

		public GameObject dialogOptionGameObject;
		public Button dialogOptionButton;
		public Text dialogOptionText;

	}
}