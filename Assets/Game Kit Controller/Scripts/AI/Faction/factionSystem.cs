using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class factionSystem : MonoBehaviour
{
	public List<factionInfo> factionList = new List<factionInfo> ();

	public string[] factionStringList;

	public List<characterFactionManager> characterFactionManagerList = new List<characterFactionManager> ();


	public void addCharacterToList (characterFactionManager character)
	{
		characterFactionManagerList.Add (character);
	}

	public void removeCharacterToList (characterFactionManager character)
	{
		characterFactionManagerList.Remove (character);
	}

	public bool isCharacterFriendly (int ownFactionIndex, string characterToCheckFactionName)
	{
		factionInfo currentFactionInfo = factionList [ownFactionIndex];

		if (currentFactionInfo.Name.Equals (characterToCheckFactionName)) {
			return true;
		}

		int relationWithFactionsCount = currentFactionInfo.relationWithFactions.Count;

		for (int j = 0; j < relationWithFactionsCount; j++) {

			relationInfo currentRelationInfo = currentFactionInfo.relationWithFactions [j];

			if (currentRelationInfo.factionName.Equals (characterToCheckFactionName)) {
				if (currentRelationInfo.relation == relationInfo.relationType.friend) {
					return true;
				} else {
					return false;
				}
			}
		}

		return false;
	}

	public bool isCharacterEnemy (int ownFactionIndex, string characterToCheckFacionName)
	{
		factionInfo currentFactionInfo = factionList [ownFactionIndex];

		if (currentFactionInfo.Name.Equals (characterToCheckFacionName)) {
			return false;
		}

		int relationWithFactionsCount = currentFactionInfo.relationWithFactions.Count;

		for (int j = 0; j < relationWithFactionsCount; j++) {
			relationInfo currentRelationInfo = currentFactionInfo.relationWithFactions [j];

			if (currentRelationInfo.factionName.Equals (characterToCheckFacionName)) {
				if (currentRelationInfo.relation == relationInfo.relationType.enemy) {

//					print ("checking relation of " + currentFactionInfo + " with " + currentRelationInfo.factionName);

					return true;
				} else {
					return false;
				}
			}
		}

		return true;
	}

	public bool isAttackerEnemy (int ownFactionIndex, string characterToCheckFacionName, bool checkForFriendlyFactionAttackers, bool changeFactionRelationWithFriendlyAttackers)
	{
		factionInfo currentFactionInfo = factionList [ownFactionIndex];

		if (currentFactionInfo.Name.Equals (characterToCheckFacionName)) {
			if (currentFactionInfo.friendlyFireTurnIntoEnemies) {
				return true;
			}

			return false;
		}

		int relationWithFactionsCount = currentFactionInfo.relationWithFactions.Count;

		for (int j = 0; j < relationWithFactionsCount; j++) {
			relationInfo currentRelationInfo = currentFactionInfo.relationWithFactions [j];

			if (currentRelationInfo.factionName.Equals (characterToCheckFacionName)) {
				if (currentRelationInfo.relation == relationInfo.relationType.enemy) {
					return true;
				} else {
					if (checkForFriendlyFactionAttackers && currentFactionInfo.turnToEnemyIfAttack) {
						if (changeFactionRelationWithFriendlyAttackers) {
							if (currentFactionInfo.turnFactionToEnemy) {
								changeFactionRelation (ownFactionIndex, characterToCheckFacionName, relationInfo.relationType.enemy);
							}
						}

						return true;
					} else {
						return false;
					}
				}
			}
		}

		return true;
	}

	public bool isCharacterNeutral (int ownFactionIndex, string characterToCheckFacionName)
	{
		factionInfo currentFactionInfo = factionList [ownFactionIndex];

		if (currentFactionInfo.Name.Equals (characterToCheckFacionName)) {
			return false;
		}

		int relationWithFactionsCount = currentFactionInfo.relationWithFactions.Count;

		for (int j = 0; j < relationWithFactionsCount; j++) {
			relationInfo currentRelationInfo = currentFactionInfo.relationWithFactions [j];

			if (currentRelationInfo.factionName.Equals (characterToCheckFacionName)) {
				if (currentRelationInfo.relation == relationInfo.relationType.neutral) {
					return true;
				} else {
					return false;
				}
			}
		}

		return true;
	}

	public void changeFactionRelation (int ownFactionIndex, string otherFactionName, relationInfo.relationType relationType)
	{
		factionInfo currentFactionInfo = factionList [ownFactionIndex];

		if (!currentFactionInfo.Name.Equals (otherFactionName)) {
			int relationWithFactionsCount = currentFactionInfo.relationWithFactions.Count;

			for (int j = 0; j < relationWithFactionsCount; j++) {
				relationInfo currentRelationInfo = currentFactionInfo.relationWithFactions [j];

				if (currentRelationInfo.factionName.Equals (otherFactionName)) {
					currentRelationInfo.relation = relationType;
				}
			}
		}

		int characterFactionManagerListCount = characterFactionManagerList.Count;

		for (int i = 0; i < characterFactionManagerListCount; i++) {
			characterFactionManager currentCharacterFactionManager = characterFactionManagerList [i];

			if (currentCharacterFactionManager.factionName.Equals (currentFactionInfo.Name)) {
				currentCharacterFactionManager.checkCharactersAround ();
			}
		}
	}

	public void removeEnemiesFromNewFriendFaction (int ownFactionIndex)
	{
		factionInfo currentFactionInfo = factionList [ownFactionIndex];

		int characterFactionManagerListCount = characterFactionManagerList.Count;

		for (int i = 0; i < characterFactionManagerListCount; i++) {
			characterFactionManager currentCharacterFactionManager = characterFactionManagerList [i];

			if (currentCharacterFactionManager.factionName.Equals (currentFactionInfo.Name)) {

				GKC_Utils.removeEnemiesFromNewFriendFaction (currentCharacterFactionManager.characterTransform);
			}
		}
	}

	public void removeEnemiesFromCharacter (int ownFactionIndex, Transform characterTransform)
	{
		factionInfo currentFactionInfo = factionList [ownFactionIndex];

		int characterFactionManagerListCount = characterFactionManagerList.Count;

		for (int i = 0; i < characterFactionManagerListCount; i++) {
			characterFactionManager currentCharacterFactionManager = characterFactionManagerList [i];

			if (currentCharacterFactionManager.factionName.Equals (currentFactionInfo.Name) && characterTransform == currentCharacterFactionManager.characterTransform) {

				GKC_Utils.removeEnemiesFromNewFriendFaction (currentCharacterFactionManager.characterTransform);

				return;
			}
		}
	}

	public bool checkIfCharacterBelongsToFaction (string factionName, GameObject character)
	{
		if (character == null) {
			return false;
		}

		characterFactionManager characterFactionToCheck = character.GetComponent<characterFactionManager> ();

		if (characterFactionToCheck != null) {
			if (characterFactionToCheck.factionName.Equals (factionName)) {
				return true;
			}
		}

		return false;
	}

	public bool isCharacterDetectedAsEnemyByOtherFaction (GameObject characterToCheck)
	{
		int characterFactionManagerListCount = characterFactionManagerList.Count;

		for (int i = 0; i < characterFactionManagerListCount; i++) {
			if (characterFactionManagerList [i].currentDetectedEnemyList.Contains (characterToCheck)) {
				return true;
			}
		}

		return false;
	}

	public void alertFactionOnSpotted (int ownFactionIndex, float alertCloseFactionRadius, GameObject target, Vector3 alertPosition)
	{
		string factionNameToCheck = factionList [ownFactionIndex].Name;

//		print (factionNameToCheck);

		int characterFactionManagerListCount = characterFactionManagerList.Count;

		for (int i = 0; i < characterFactionManagerListCount; i++) {
			characterFactionManager currentCharacterFactionManager = characterFactionManagerList [i];

			if (currentCharacterFactionManager.factionName.Equals (factionNameToCheck)) {
				float distance = GKC_Utils.distance (currentCharacterFactionManager.getCharacterTransform ().position, alertPosition);

//				print ("distance to target " + characterFactionManagerList [i].getCharacterTransform ().name + " " + distance);

				if (distance <= alertCloseFactionRadius) {
					currentCharacterFactionManager.alertFaction (target);
				}
			}
		}
	}

	public void removeCharacterAsTargetOnSameFaction (GameObject characterToCheck, int ownFactionIndex)
	{
		factionInfo currentFactionInfo = factionList [ownFactionIndex];

		string factionNameToCheck = currentFactionInfo.Name;

		int characterFactionManagerListCount = characterFactionManagerList.Count;

//		print ("faction to remove " + factionNameToCheck + " character to remove " + characterToCheck.name);

		for (int i = 0; i < characterFactionManagerListCount; i++) {
			characterFactionManager currentCharacterFactionManager = characterFactionManagerList [i];

			bool characterEnemyOfNewFaction = isCharacterEnemy (currentCharacterFactionManager.factionIndex, factionNameToCheck);

//			print (characterEnemyOfNewFaction);

			if (currentCharacterFactionManager.factionName.Equals (factionNameToCheck) || !characterEnemyOfNewFaction) {
				currentCharacterFactionManager.sendSignalToRemoveCharacterAsTargetOnSameFaction (characterToCheck);

//				print ("remove enemy turned into friend or neutral from " + currentCharacterFactionManager.gameObject.name);
			}
		}
	}


	//EDITOR FUNCTIONS
	public void addFaction ()
	{
		factionInfo newFactionInfo = new factionInfo ();
		newFactionInfo.Name = "New Faction";
		factionList.Add (newFactionInfo);

		updateComponent ();
	}

	public void removeFaction (int factionIndexToRemove)
	{
//		print ("Removing " + factionList [factionIndexToRemove].Name + " faction");

		factionList.RemoveAt (factionIndexToRemove);

		updateComponent ();
	}

	public void updateFactionsList ()
	{
		int previousFactionAmount = factionStringList.Length;

		getFactionStringList ();

		int newFactionAmount = factionStringList.Length;

		if (previousFactionAmount > newFactionAmount) {
			print (previousFactionAmount - newFactionAmount + " factions were removed");
		} else if (previousFactionAmount < newFactionAmount) {
			print (newFactionAmount - previousFactionAmount + " factions were added");
		} else {
			print ("No factions were added or removed");
		}

		if (factionList.Count > 1) {
			int i = 0;
			int j = 0;
			int k = 0;

			print ("Checking for factions removed");

			for (i = 0; i < factionStringList.Length; i++) {
				if (checkIfFactionExists (factionStringList [i])) {
					
				} else {
					print ("faction " + factionStringList [i] + " was removed, removing relation with " + factionList [i].Name);

					for (j = 0; j < factionList.Count; j++) {
						for (k = factionList [j].relationWithFactions.Count - 1; k >= 0; k--) {
							if (factionList [j].relationWithFactions [k].factionName.Equals (factionStringList [i])) {
								factionList [j].relationWithFactions.RemoveAt (k);
							}
						}
					}
				}
			}

			for (i = 0; i < factionList.Count; i++) {
				for (j = factionList [i].relationWithFactions.Count - 1; j >= 0; j--) {
					if (!checkIfFactionExistsInStringList (factionList [i].relationWithFactions [j].factionName)) {
						print ("faction " + factionList [i].relationWithFactions [j].factionName + " was removed, removing relation with " + factionList [i].Name);

						factionList [i].relationWithFactions.RemoveAt (j);
					}
				}
			}

			print ("Updating factions relations index");

			for (i = 0; i < factionList.Count; i++) {
				for (j = 0; j < factionList [i].relationWithFactions.Count; j++) {
					int factionIndex = getFactionIndex (factionList [i].relationWithFactions [j].factionName);

					if (factionIndex > -1) {
						if (factionList [i].relationWithFactions [j].factionIndex > factionIndex) {
							print ("Updating faction " + factionList [i].Name + " relation index on faction " +
							factionList [i].relationWithFactions [j].factionName + " with index " + factionIndex);
						
							factionList [i].relationWithFactions [j].factionIndex = factionIndex;
						}
					}
				}
			}

			print ("Updating relations with new factions added");

			for (i = 0; i < factionList.Count; i++) {
				for (j = 0; j < factionList.Count; j++) {
					if (!factionList [i].Name.Equals (factionList [j].Name)) {
						
						bool containsRelation = factionContainsRelation (i, factionStringList [j]);

						if (!containsRelation) {
							print ("New relation to configure on " + factionList [i].Name + " and " + factionStringList [j] + " has been added");

							relationInfo newRelationInfo = new relationInfo ();
							newRelationInfo.factionName = factionStringList [j];
							newRelationInfo.factionIndex = j;
							factionList [i].relationWithFactions.Add (newRelationInfo);
						}
					}
				}
			}
		}

		updateComponent ();
	}

	public int getFactionIndex (string factionName)
	{
		int factionListCount = factionList.Count;

		for (int i = 0; i < factionListCount; i++) {
			if (factionList [i].Name.Equals (factionName)) {
				return i;
			}
		}

		return -1;
	}

	public bool checkIfFactionExists (string factionName)
	{
		int factionListCount = factionList.Count;

		for (int i = 0; i < factionListCount; i++) {
			if (factionList [i].Name.Equals (factionName)) {
				return true;
			}
		}

		return false;
	}

	public bool checkIfFactionExistsInStringList (string factionName)
	{
		int factionStringListLength = factionStringList.Length;

		for (int i = 0; i < factionStringListLength; i++) {
			if (factionStringList [i].Equals (factionName)) {
				return true;
			}
		}

		return false;
	}

	public bool factionContainsRelation (int factionIndex, string factionWithRelation)
	{
		factionInfo currentFactionInfo = factionList [factionIndex];

		if (currentFactionInfo.Name.Equals (factionWithRelation)) {
			return true;
		}

		int relationWithFactionsCount = currentFactionInfo.relationWithFactions.Count;

		for (int j = 0; j < relationWithFactionsCount; j++) {
			if (currentFactionInfo.relationWithFactions [j].factionName.Equals (factionWithRelation)) {
				return true;
			}
		}

		return false;
	}

	public void getFactionStringList ()
	{
		factionStringList = new string[factionList.Count];

		for (int i = 0; i < factionList.Count; i++) {
			string newFactionName = factionList [i].Name;

			factionStringList [i] = newFactionName;
		}
	}

	public void updateFactionListNames ()
	{
		getFactionStringList ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Faction System values", gameObject);
	}

	[System.Serializable]
	public class factionInfo
	{
		public string Name;
		public bool turnToEnemyIfAttack;
		public bool turnFactionToEnemy;
		public bool friendlyFireTurnIntoEnemies;
		public List<relationInfo> relationWithFactions = new List<relationInfo> ();
	}

	[System.Serializable]
	public class relationInfo
	{
		public string factionName;
		public int factionIndex;
		public relationType relation;

		public enum relationType
		{
			friend,
			enemy,
			neutral
		}
	}
}
