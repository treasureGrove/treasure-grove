using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activateActionOnCharactersOnScene : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool activateActionEnabled = true;
	public int currentCharacterActionInfoIndex;

	public Transform playerTransform;

	[Space]
	[Header ("Character Action Info List Settings")]
	[Space]

	public List<characterActionInfo> characterActionInfoList = new List<characterActionInfo> ();

	public void activateActionOnCharacters (string actionName)
	{
		if (!activateActionEnabled) {
			return;
		}

		for (int i = 0; i < characterActionInfoList.Count; i++) {
			if (characterActionInfoList [i].Name.Equals (actionName)) {
				currentCharacterActionInfoIndex = i;
			}
		}

		characterActionInfo currentCharacterActionInfo = characterActionInfoList [currentCharacterActionInfoIndex];

		playerActionSystem[] playerActionSystemList = FindObjectsOfType<playerActionSystem> ();

		foreach (playerActionSystem currentPlayerActionSystem in playerActionSystemList) {
			bool canActivateAction = true;

			Transform characterTransform = currentPlayerActionSystem.playerTransform;

			if (currentCharacterActionInfo.ignoreCharacters) {
				if (currentCharacterActionInfo.characterObjectToIgnoreList.Count > 0) {
					if (currentCharacterActionInfo.characterObjectToIgnoreList.Contains (characterTransform.gameObject)) {
						canActivateAction = false;
					}
				}

				if (currentCharacterActionInfo.tagsToIgnore.Contains (characterTransform.tag)) {
					canActivateAction = false;
				}
			}

			if (currentCharacterActionInfo.useMaxDistance) {
				float currentDistance = GKC_Utils.distance (playerTransform.position, characterTransform.position);

				if (currentDistance > currentCharacterActionInfo.maxDistance) {
					canActivateAction = false;
				}
			}

			if (canActivateAction) {
				if (currentCharacterActionInfo.activateAction) {
					currentPlayerActionSystem.activateCustomAction (currentCharacterActionInfo.Name);
				}

				if (currentCharacterActionInfo.useRemoteEvents) {
					remoteEventSystem currentRemoteEventSystem = currentPlayerActionSystem.mainRemoteEventSystem;

					if (currentRemoteEventSystem != null) {
						for (int i = 0; i < currentCharacterActionInfo.remoteEventNameList.Count; i++) {
							currentRemoteEventSystem.callRemoteEvent (currentCharacterActionInfo.remoteEventNameList [i]);
						}

					}

				}
			}
		}
	}

	[System.Serializable]
	public class characterActionInfo
	{
		public string Name;

		public bool activateAction;

		[Space]
		[Space]

		public bool ignoreCharacters;

		public List<string> tagsToIgnore = new List<string> ();

		public List<GameObject> characterObjectToIgnoreList = new List<GameObject> ();

		[Space]
		[Space]

		public bool useMaxDistance;
		public float maxDistance;

		[Space]
		[Space]

		public bool useRemoteEvents;
		public List<string> remoteEventNameList = new List<string> ();
	}
}
