using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterFactionManager : MonoBehaviour
{
	public string factionName;
	public int factionIndex;

	public bool checkForFriendlyFactionAttackers = true;
	public bool changeFactionRelationWithFriendlyAttackers = true;

	public List<GameObject> currentDetectedEnemyList = new List<GameObject> ();

	public Transform characterTransform;

	public string[] factionStringList;
	public factionSystem factionManager;

	public bool factionManagerAssigned;

	public string mainManagerName = "Faction System";


	void Start ()
	{
		addCharacterFromFaction ();
	}

	public void changeCharacterToFaction (string factionToChange)
	{
		getFactionManager ();

		if (factionManager != null) {
			int factionListCount = factionManager.factionList.Count;

			for (int i = 0; i < factionListCount; i++) {
				if (factionManager.factionList [i].Name.Equals (factionToChange)) {
					factionIndex = i;

					factionName = factionToChange;
				}
			}
		}
	}

	public bool isCharacterFriendly (string characterToCheckFacionName)
	{
		if (factionManager == null) {
			return false;
		}

		return factionManager.isCharacterFriendly (factionIndex, characterToCheckFacionName);
	}

	public bool isCharacterEnemy (string characterToCheckFacionName)
	{
		if (factionManager == null) {
			return false;
		}

		return factionManager.isCharacterEnemy (factionIndex, characterToCheckFacionName);
	}

	public bool isAttackerEnemy (string characterToCheckFacionName)
	{
		if (factionManager == null) {
			return false;
		}

		return factionManager.isAttackerEnemy (factionIndex, characterToCheckFacionName, checkForFriendlyFactionAttackers, changeFactionRelationWithFriendlyAttackers);
	}

	public bool isCharacterNeutral (string characterToCheckFacionName)
	{
		if (factionManager == null) {
			return false;
		}

		return factionManager.isCharacterNeutral (factionIndex, characterToCheckFacionName);
	}

	public string getFactionName ()
	{
		return factionName;
	}

	public bool checkIfCharacterBelongsToFaction (string factionName, GameObject character)
	{
		if (factionManager == null) {
			return false;
		}

		return factionManager.checkIfCharacterBelongsToFaction (factionName, character);
	}

	public void removeCharacterAsTargetOnSameFaction (GameObject characterGameObject)
	{
		factionManager.removeCharacterAsTargetOnSameFaction (characterGameObject, factionIndex);
	}

	public void sendSignalToRemoveCharacterAsTargetOnSameFaction (GameObject targetToRemove)
	{
		characterTransform.SendMessage ("sendSignalToRemoveCharacterAsTarget", targetToRemove, SendMessageOptions.DontRequireReceiver);
	}

	public void checkCharactersAround ()
	{
		characterTransform.SendMessage ("checkCharactersAroundAI", SendMessageOptions.DontRequireReceiver);
	}

	public void alertFaction (GameObject target)
	{
		characterTransform.SendMessage ("enemyAlert", target, SendMessageOptions.DontRequireReceiver);
	}

	public string[] getFactionStringList ()
	{
		return factionStringList;
	}

	public void addDetectedEnemyFromFaction (GameObject enemy)
	{
		if (!currentDetectedEnemyList.Contains (enemy)) {
			currentDetectedEnemyList.Add (enemy);
		}
	}

	public void removeDetectedEnemyFromFaction (GameObject enemy)
	{
		if (currentDetectedEnemyList.Contains (enemy)) {
			currentDetectedEnemyList.Remove (enemy);
		}
	}

	public void clearDetectedEnemyFromFaction ()
	{
		currentDetectedEnemyList.Clear ();
	}

	public bool isCharacterDetectedAsEnemyByOtherFaction (GameObject characterToCheck)
	{
		if (factionManager == null) {
			return false;
		}

		return factionManager.isCharacterDetectedAsEnemyByOtherFaction (characterToCheck);
	}

	public void addCharacterFromFaction ()
	{
		factionManagerAssigned = false;

		getFactionManager ();

		if (factionManager != null) {
			factionManager.addCharacterToList (this);
		}

		if (characterTransform == null) {
			characterTransform = transform;
		}
	}

	public void removeCharacterDeadFromFaction ()
	{
		clearDetectedEnemyFromFaction ();

		if (factionManager != null) {
			factionManager.removeCharacterToList (this);
		}
	}

	public void alertFactionOnSpotted (float alertCloseFactionRadius, GameObject target, Vector3 alertPosition)
	{
		if (factionManager != null) {
			factionManager.alertFactionOnSpotted (factionIndex, alertCloseFactionRadius, target, alertPosition);
		}
	}

	public Transform getCharacterTransform ()
	{
		return characterTransform;
	}

	public void getFactionManager ()
	{
		if (!factionManagerAssigned) {
			if (factionManager == null) {
				GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(factionSystem));

				factionManager = FindObjectOfType<factionSystem> ();
			}

			factionManagerAssigned = true;
		}
	}

	public void setNeutralRelationWithFactionByName (string otherFactionName)
	{
		changeFactionRelation (otherFactionName, factionSystem.relationInfo.relationType.neutral);

		factionManager.removeEnemiesFromNewFriendFaction (factionIndex);
	}

	public void setEnemyRelationWithFactionByName (string otherFactionName)
	{
		changeFactionRelation (otherFactionName, factionSystem.relationInfo.relationType.enemy);

		factionManager.removeEnemiesFromNewFriendFaction (factionIndex);
	}

	public void setFriendRelationWithFactionByName (string otherFactionName)
	{
		changeFactionRelation (otherFactionName, factionSystem.relationInfo.relationType.friend);

		factionManager.removeEnemiesFromNewFriendFaction (factionIndex);
	}

	public void changeCharacterToFactionAndCleanTargetListIngame (string factionToChange)
	{
		getFactionManager ();

		if (factionManager != null) {
			int factionListCount = factionManager.factionList.Count;

			for (int i = 0; i < factionListCount; i++) {
				if (factionManager.factionList [i].Name.Equals (factionToChange)) {
					factionIndex = i;

					factionName = factionToChange;

					addCharacterFromFaction ();

					factionManager.removeEnemiesFromCharacter (factionIndex, characterTransform);

					return;
				}
			}
		}
	}

	public void changeFactionRelation (string otherFactionName, factionSystem.relationInfo.relationType relationType)
	{
		factionManager.changeFactionRelation (factionIndex, otherFactionName, relationType);
	}


	//EDITOR FUNCTIONS
	public void getFactionListFromEditor ()
	{
		if (factionManager == null) {
			factionManager = FindObjectOfType<factionSystem> ();
		}

		factionManagerAssigned = false;

		getFactionList ();

		updateComponent ();
	}

	public void changeCharacterToFactionFromEditor (string factionToChange)
	{
		getFactionListFromEditor ();

		changeCharacterToFaction (factionToChange);

		updateComponent ();
	}

	public void getFactionList ()
	{
		getFactionManager ();

		if (factionManager != null) {
			factionStringList = new string[factionManager.factionList.Count];

			for (int i = 0; i < factionManager.factionList.Count; i++) {
				string name = factionManager.factionList [i].Name;
				factionStringList [i] = name;
			}

			updateComponent ();
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}