using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class dialogContentSystem : MonoBehaviour
{
	public int dialogContentID;

	public int dialogContentScene;

	public List<completeDialogInfo> completeDialogInfoList = new List<completeDialogInfo> ();

	public int currentDialogIndex;

	public GameObject dialogOwner;

	public bool showDialogOnwerName;

	public bool dialogActive;

	public bool dialogInProcess;

	public bool playingExternalDialog;

	public bool useEventsOnStartEndDialog;
	public UnityEvent eventOnStartDialog;
	public UnityEvent eventOnEndDialog;

	public bool pauseAIOnDialogStart;
	public bool resumeAIOnDialogEnd;
	public bool interruptWanderAroundStateIfActiveOnDialogStart;
	public bool disableWanderAroundStateOnDialogEnd;


	public bool useAnimations;
	public Animator mainAnimator;
	public playerController mainPlayerController;

	public bool playerAnimationsOnDialogEnabled = true;

	public string dialogueActiveAnimatorName = "Dialogue Active";


	public GameObject newCharacterToAddDialog;


	public bool useDialogContentTemplate;
	public dialogContentTemplate mainDialogContentTemplate;


	GameObject currentPlayer;

	Coroutine playDialogOnTriggerEnterCoroutine;


	Coroutine disableDialogCharacterAnimatorCoroutine;


	public void setCurrentPlayer (GameObject newPlayer)
	{
		currentPlayer = newPlayer;
	}

	public void activateDialog ()
	{
		if (dialogInProcess) {

			bool canActivateDialog = false;

			if (playingExternalDialog) {
				if (currentDialogIndex < completeDialogInfoList.Count && completeDialogInfoList [currentDialogIndex].playDialogWithoutPausingPlayerActions) {
					canActivateDialog = true;

					dialogActive = false;
				}
			}

			if (!canActivateDialog) {
				return;
			}
		}

		if (!currentPlayer) {
			print ("WARNING: current player hasn't been assigned, make sure to assign the function to set current player into the electronic device component");

			return;
		}

		dialogActive = !dialogActive;

		if (dialogActive) {
			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				dialogSystem currentDialogSystem = currentPlayerComponentsManager.getMainDialogSystem ();
			
				if (currentDialogSystem != null) {
					currentDialogSystem.setNewDialogContent (this);
				}
			}
		}
	}

	public void setDialogInProcessState (bool state)
	{
		dialogInProcess = state;

		if (!dialogInProcess) {
			dialogActive = false;
		}
	}

	public void activateEventOnDialogStopped ()
	{
		if (currentDialogIndex < completeDialogInfoList.Count) {
			if (completeDialogInfoList [currentDialogIndex].playDialogWithoutPausingPlayerActions) {
				completeDialogInfoList [currentDialogIndex].eventOnDialogStopped.Invoke ();
			}
		}
	}

	public void setNextCompleteDialogIndex ()
	{
		currentDialogIndex++;

		if (currentDialogIndex >= completeDialogInfoList.Count) {
			currentDialogIndex = completeDialogInfoList.Count - 1;
		}
	}

	public void setPrevioustCompleteDialogIndex ()
	{
		currentDialogIndex--;

		if (currentDialogIndex < 0) {
			currentDialogIndex = 0;
		}
	}

	public void setCompleteDialogIndex (int newIndex)
	{
		currentDialogIndex = newIndex;

		if (currentDialogIndex >= completeDialogInfoList.Count) {
			currentDialogIndex = completeDialogInfoList.Count - 1;
		}
	}

	public void setPlayingExternalDialogState (bool state)
	{
		playingExternalDialog = state;
	}

	public void checkIfPlayDialogOnTriggerEnter ()
	{
		if (playDialogOnTriggerEnterCoroutine != null) {
			StopCoroutine (playDialogOnTriggerEnterCoroutine);
		}

		playDialogOnTriggerEnterCoroutine = StartCoroutine (checkIfPlayDialogOnTriggerEnterCoroutine ());
	}

	IEnumerator checkIfPlayDialogOnTriggerEnterCoroutine ()
	{
		yield return new WaitForEndOfFrame ();

		if (completeDialogInfoList [currentDialogIndex].playDialogOnTriggerEnter) {
			completeDialogInfoList [currentDialogIndex].eventToPlayDialogOnTriggerEnter.Invoke ();
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
		if (mainAnimator != null) {
			mainAnimator.SetBool (dialogueActiveAnimatorName, state);
		}

		if (mainPlayerController != null) {
			mainPlayerController.setApplyRootMotionAlwaysActiveState (state);
		}
	}

	public void checkEventsOnDialog (bool state)
	{
		if (useEventsOnStartEndDialog) {
			if (state) {
				eventOnStartDialog.Invoke ();
			} else {
				eventOnEndDialog.Invoke ();
			}
		}
	}

	public void checkIfPauseOrResumeAI (bool state)
	{
		if (dialogOwner == null) {
			return;
		}

		playerComponentsManager mainPlayerComponentsManager = dialogOwner.GetComponent<playerComponentsManager> ();

		if (mainPlayerComponentsManager != null) {
			AINavMesh mainAINavmesh = dialogOwner.GetComponent<AINavMesh> ();
			findObjectivesSystem mainFindObjectivesSystem = dialogOwner.GetComponent<findObjectivesSystem> ();

			if (mainAINavmesh == null) {
				return;
			}

			if (mainFindObjectivesSystem == null) {
				return;
			}


			if (state) {
				if (interruptWanderAroundStateIfActiveOnDialogStart) {
					mainFindObjectivesSystem.checkIfInterruptWanderState ();
				} 
					
				if (pauseAIOnDialogStart) {
					mainAINavmesh.pauseAI (true);
				}
			} else {
				if (resumeAIOnDialogEnd) {
					mainAINavmesh.pauseAI (false);
				}

				if (disableWanderAroundStateOnDialogEnd) {
					mainFindObjectivesSystem.disableWanderState ();
				}
			}
		}
	}

	public void setDialogAnswerConditionCheckResult (bool state)
	{
		if (dialogActive) {
			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				dialogSystem currentDialogSystem = currentPlayerComponentsManager.getMainDialogSystem ();

				if (currentDialogSystem != null) {
					currentDialogSystem.setDialogAnswerConditionCheckResult (state);
				}
			}
		}
	}

	public void setNextDialogWithConditionCheckResult (bool state)
	{
		if (dialogActive) {
			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				dialogSystem currentDialogSystem = currentPlayerComponentsManager.getMainDialogSystem ();

				if (currentDialogSystem != null) {
					currentDialogSystem.setNextDialogWithConditionCheckResult (state);
				}
			}
		}
	}

	public completeDialogInfoTemplate getCompleteDialogInfoTemplateByLanguageName (string languageName)
	{
		for (int i = 0; i < mainDialogContentTemplate.completeDialogInfoTemplateList.Count; i++) {

			if (mainDialogContentTemplate.completeDialogInfoTemplateList [i].language.Equals (languageName)) {
				return mainDialogContentTemplate.completeDialogInfoTemplateList [i];
			}
		}

		return null;
	}

	//Editor functions
	public void addNewDialog ()
	{
		completeDialogInfo newCompleteDialogInfo = new completeDialogInfo ();

		newCompleteDialogInfo.ID = completeDialogInfoList.Count;

		completeDialogInfoList.Add (newCompleteDialogInfo);

		updateComponent ();
	}

	public void addNewLine (int dialogIndex)
	{
		dialogInfo newDialogInfo = new dialogInfo ();

		newDialogInfo.ID = completeDialogInfoList [dialogIndex].dialogInfoList.Count;

		completeDialogInfoList [dialogIndex].dialogInfoList.Add (newDialogInfo);

		updateComponent ();
	}

	public void addNewAnswer (int dialogIndex, int lineIndex)
	{
		dialogLineInfo newDialogLineInfo = new dialogLineInfo ();

		newDialogLineInfo.ID = completeDialogInfoList [dialogIndex].dialogInfoList [lineIndex].dialogLineInfoList.Count;

		completeDialogInfoList [dialogIndex].dialogInfoList [lineIndex].dialogLineInfoList.Add (newDialogLineInfo);

		updateComponent ();
	}

	public void assignIDToDialog (int newID)
	{
		dialogContentID = newID;

		updateComponent ();
	}

	public void assignDialogContentScene (int newScene)
	{
		dialogContentScene = newScene;

		updateComponent ();
	}

	public void setDialogOwnerGameObject (GameObject newOwner)
	{
		dialogOwner = newOwner;

		if (newOwner != null) {
			mainPlayerController = newOwner.GetComponentInChildren<playerController> ();

			if (mainPlayerController != null) {
				dialogOwner = mainPlayerController.gameObject;

				mainAnimator = mainPlayerController.getCharacterAnimator ();

				Transform previousParent = transform.parent;

				transform.SetParent (mainPlayerController.transform.parent);

				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;

				transform.SetParent (previousParent);

				followObjectPositionSystem newFollowObjectPositionSystem = GetComponent<followObjectPositionSystem> ();

				if (newFollowObjectPositionSystem != null) {
					newFollowObjectPositionSystem.setObjectToFollowFromEditor (dialogOwner.transform);
				}
			}
		}

		updateComponent ();
	}

	public void addDialogToCharacterManually ()
	{
		if (newCharacterToAddDialog != null) {

			setDialogOwnerGameObject (newCharacterToAddDialog);

			newCharacterToAddDialog = null;

			updateComponent ();
		}
	}

	public void addDialogContentToTemplate ()
	{
		if (mainDialogContentTemplate == null) {


		}

		if (mainDialogContentTemplate != null) {
			completeDialogInfoTemplate newCompleteDialogInfoTemplate = new completeDialogInfoTemplate ();

			newCompleteDialogInfoTemplate.language = "English";

			for (int i = 0; i < completeDialogInfoList.Count; i++) {
				completeDialogInfo currentCompleteDialogInfo = completeDialogInfoList [i];

				simpleCompleteDialogInfo newSimpleCompleteDialogInfo = new simpleCompleteDialogInfo ();

				newSimpleCompleteDialogInfo.Name = currentCompleteDialogInfo.Name;
				newSimpleCompleteDialogInfo.ID = currentCompleteDialogInfo.ID;

				for (int j = 0; j < currentCompleteDialogInfo.dialogInfoList.Count; j++) {
					dialogInfo currentDialogInfo = currentCompleteDialogInfo.dialogInfoList [j];

					simpleDialogInfo newSimpleDialogInfo = new simpleDialogInfo ();

					newSimpleDialogInfo.Name = currentDialogInfo.Name;
					newSimpleDialogInfo.ID = currentDialogInfo.ID;
					newSimpleDialogInfo.dialogOwnerName = currentDialogInfo.dialogOwnerName;
					newSimpleDialogInfo.dialogContent = currentDialogInfo.dialogContent;
					newSimpleDialogInfo.dialogLineSound = currentDialogInfo.dialogLineSound;

					for (int k = 0; k < currentDialogInfo.dialogLineInfoList.Count; k++) {
						dialogLineInfo currentDialogLineInfo = currentDialogInfo.dialogLineInfoList [k];

						simpleDialogLineInfo newSimpleDialogLineInfo = new simpleDialogLineInfo ();

						newSimpleDialogLineInfo.Name = currentDialogLineInfo.Name;
						newSimpleDialogLineInfo.ID = currentDialogLineInfo.ID;
						newSimpleDialogLineInfo.dialogLineContent = currentDialogLineInfo.dialogLineContent;

						newSimpleDialogInfo.dialogLineInfoList.Add (newSimpleDialogLineInfo);
					}

					newSimpleCompleteDialogInfo.dialogInfoList.Add (newSimpleDialogInfo);
				}

				newCompleteDialogInfoTemplate.completeDialogInfoList.Add (newSimpleCompleteDialogInfo);
			}

			mainDialogContentTemplate.completeDialogInfoTemplateList.Add (newCompleteDialogInfoTemplate);

			updateComponent ();
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Dialog Content Object", gameObject);
	}
}
