using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideFromEnemiesSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask layerToCharacters;

	public float timeDelayToHidenAgainIfDiscovered;

	public float checkIfCharacterCanBeHiddenAgainRate = 0.5f;

	public bool loseTrackOfPlayerByAIOnTriggerEnterEnabled;
	public bool reactivatePlayerColliderOnExitTrigger;

	[Space]
	[Header ("Hide Conditions Settings")]
	[Space]

	public bool characterNeedToCrouch;

	public float maxMoveAmount;

	public bool characterCantMove;
	public bool hiddenForATime;
	public float hiddenForATimeAmount;

	[Space]
	[Header ("Character State Icon Settings")]
	[Space]

	public bool useCharacterStateIcon;
	public string visibleCharacterStateName;
	public string notVisibleCharacterStateName;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<hiddenCharacterInfo> hiddenCharacterInfoList = new List<hiddenCharacterInfo> ();

	public bool charactersOnHiddenList;

	hiddenCharacterInfo currentCharacter;

	void Update ()
	{
		if (!charactersOnHiddenList) {
			return;
		}

		for (int i = 0; i < hiddenCharacterInfoList.Count; i++) {
			currentCharacter = hiddenCharacterInfoList [i];

			//if an external action changes the visibility of the character to AI, change its state here too
			if (currentCharacter.playerControllerManager.isCharacterVisibleToAI ()) {
				if (currentCharacter.hidden) {
					currentCharacter.hidden = false;

					if (currentCharacter.factionManager.isCharacterDetectedAsEnemyByOtherFaction (currentCharacter.characterGameObject)) {
						currentCharacter.lastTimeDiscovered = Time.time;	
					}
				} 
			}

			if (!currentCharacter.hidden) {
				if (Time.time > currentCharacter.lastTimeCheckIfCharacterCanBeHiddenAgain + checkIfCharacterCanBeHiddenAgainRate) {
					if (currentCharacter.factionManager.isCharacterDetectedAsEnemyByOtherFaction (currentCharacter.characterGameObject)) {
						if (currentCharacter.canBeHidden) {
							setCharacterState (visibleCharacterStateName, i);

							currentCharacter.playerControllerManager.setVisibleToAIState (true);

							currentCharacter.canBeHidden = false;
						}
					} else if (Time.time > currentCharacter.lastTimeDiscovered + timeDelayToHidenAgainIfDiscovered && currentCharacter.canBeHidden) {
						currentCharacter.playerControllerManager.setVisibleToAIState (false);

						currentCharacter.hidden = true;

						currentCharacter.lastTimeHidden = Time.time;

						setCharacterState (notVisibleCharacterStateName, i);

						if (loseTrackOfPlayerByAIOnTriggerEnterEnabled) {
							GKC_Utils.removeTargetFromAIEnemyList (currentCharacter.characterGameObject);
						}
					}

					currentCharacter.lastTimeCheckIfCharacterCanBeHiddenAgain = Time.time;
				}
			}

			if (characterNeedToCrouch) {
				if (currentCharacter.playerControllerManager.isCrouching ()) {
					if (!currentCharacter.hidden) {
						if (!currentCharacter.factionManager.isCharacterDetectedAsEnemyByOtherFaction (currentCharacter.characterGameObject)) {
							currentCharacter.playerControllerManager.setVisibleToAIState (false);

							currentCharacter.hidden = true;

							currentCharacter.canBeHidden = true;

							currentCharacter.lastTimeHidden = Time.time;

							setCharacterState (notVisibleCharacterStateName, i);

							if (loseTrackOfPlayerByAIOnTriggerEnterEnabled) {
								GKC_Utils.removeTargetFromAIEnemyList (currentCharacter.characterGameObject);
							}
						}
					}
				} else {
					if (currentCharacter.hidden) {
						currentCharacter.playerControllerManager.setVisibleToAIState (true);
						currentCharacter.hidden = false;

						currentCharacter.lastTimeDiscovered = Time.time;

						setCharacterState (visibleCharacterStateName, i);

						currentCharacter.canBeHidden = false;
					}
				}
			} else {
				currentCharacter.playerControllerManager.setVisibleToAIState (false);

				if (!currentCharacter.hidden) {
					currentCharacter.hidden = true;

					currentCharacter.canBeHidden = true;

					currentCharacter.lastTimeHidden = Time.time;

					setCharacterState (notVisibleCharacterStateName, i);

					if (loseTrackOfPlayerByAIOnTriggerEnterEnabled) {
						GKC_Utils.removeTargetFromAIEnemyList (currentCharacter.characterGameObject);
					}
				}
			}

//			if (hiddenForATime) {
//				if (Time.time > currentCharacter.lastTimeHidden + hiddenForATimeAmount) {
//					currentCharacter.playerControllerManager.setVisibleToAIState (true);
//					setCharacterState (visibleCharacterStateName, i);
//					currentCharacter.hidden = false;
//					currentCharacter.lastTimeDiscovered = Time.time;
//				}
//			}
//
//			if (characterCantMove) {
//				if (currentCharacter.playerControllerManager.isPlayerMoving (maxMoveAmount)) {
//					currentCharacter.playerControllerManager.setVisibleToAIState (true);
//					setCharacterState (visibleCharacterStateName, i);
//					currentCharacter.hidden = false;
//					currentCharacter.lastTimeDiscovered = Time.time;
//				}
//			}
		}
	}

	public void setCharacterState (string stateName, int characterIndex)
	{
		if (useCharacterStateIcon) {
			hiddenCharacterInfoList [characterIndex].playerControllerManager.setCharacterStateIcon (stateName);
		}
	}

	public void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	public void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if ((1 << col.gameObject.layer & layerToCharacters.value) == 1 << col.gameObject.layer) {
			GameObject characterFound = col.gameObject;

			bool alreadyIncluded = false;
			int characterIndex = -1;

			for (int i = 0; i < hiddenCharacterInfoList.Count; i++) {
				if (hiddenCharacterInfoList [i].characterGameObject == characterFound && !alreadyIncluded) {
					alreadyIncluded = true;
					characterIndex = i;
				}
			}

			if (isEnter) {
				if (!alreadyIncluded) {
					hiddenCharacterInfo newHiddenCharacterInfo = new hiddenCharacterInfo ();

					newHiddenCharacterInfo.name = characterFound.name;
					newHiddenCharacterInfo.characterGameObject = characterFound;

					playerComponentsManager mainPlayerComponentsManager = characterFound.GetComponent<playerComponentsManager> ();

					newHiddenCharacterInfo.playerControllerManager = mainPlayerComponentsManager.getPlayerController ();
					newHiddenCharacterInfo.playerControllerManager.setVisibleToAIState (false);

					newHiddenCharacterInfo.factionManager = mainPlayerComponentsManager.getCharacterFactionManager ();

					newHiddenCharacterInfo.hidden = true;
					newHiddenCharacterInfo.lastTimeHidden = Time.time;
					newHiddenCharacterInfo.canBeHidden = true;

					hiddenCharacterInfoList.Add (newHiddenCharacterInfo);

					setCharacterState (notVisibleCharacterStateName, hiddenCharacterInfoList.Count - 1);

					if (loseTrackOfPlayerByAIOnTriggerEnterEnabled) {
						GKC_Utils.removeTargetFromAIEnemyList (characterFound);
					}
				}
			} else {
				if (alreadyIncluded) {
					hiddenCharacterInfo characterToRemove = hiddenCharacterInfoList [characterIndex];

					characterToRemove.playerControllerManager.setVisibleToAIState (true);

					setCharacterState (visibleCharacterStateName, characterIndex);

					hiddenCharacterInfoList.RemoveAt (characterIndex);

					if (reactivatePlayerColliderOnExitTrigger) {
						characterToRemove.playerControllerManager.reactivateColliderIfPossible ();
					}
				}
			}

			if (hiddenCharacterInfoList.Count > 0) {
				charactersOnHiddenList = true;
			} else {
				charactersOnHiddenList = false;
			}
		}
	}

	[System.Serializable]
	public class hiddenCharacterInfo
	{
		public string name;
		public GameObject characterGameObject;
		public playerController playerControllerManager;
		public characterFactionManager factionManager;
		public bool hidden;
		public float hiddenTime;
		public float lastTimeHidden;
		public float lastTimeDiscovered;
		public bool canBeHidden;
		public float lastTimeCheckIfCharacterCanBeHiddenAgain;
	}
}
