using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class friendListManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool friendManagerEnabled;

	public string attackTargetOrderName = "Attacking";
	public string followTargetOrderName = "Following";
	public string waitOrderName = "Waiting";
	public string hideOrderName = "Hiding";
	public string switchCharacterOrderName = "Switch Character";
	public List<string> tagToLocate = new List<string> ();

	public bool useBlurUIPanel = true;
	public bool usedByAI;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventToSetCharacterAsAI;
	public UnityEvent eventToSetCharacterAsPlayer;

	public bool useEventIfSystemDisabled;
	public UnityEvent eventIfSystemDisabled;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool menuOpened;

	public List<friendInfo> friendsList = new List<friendInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public menuPause pauseManager;
	public playerController playerControllerManager;
	public Collider mainCollider;

	public GameObject friendsMenuContent;
	public GameObject friendListContent;
	public GameObject friendListElement;
	public Button attackButton;
	public Button followButton;
	public Button waitButton;
	public Button hideButton;

	public playerCharactersManager mainPlayerCharactersManager;
	public AIHidePositionsManager hidePositionsManager;

	List<GameObject> closestEnemyList = new List<GameObject> ();

	int i, j;

	friendInfo currentFriendInfo;


	void Start ()
	{
		if (usedByAI) {
			return;
		}

		if (!friendManagerEnabled) {
			checkEventOnSystemDisabled ();
		}

		if (friendListElement.activeSelf) {
			friendListElement.SetActive (false);
		}

		if (friendsMenuContent.activeSelf) {
			friendsMenuContent.SetActive (false);
		}
	}

	public void checkEventOnSystemDisabled ()
	{
		if (useEventIfSystemDisabled) {
			eventIfSystemDisabled.Invoke ();
		}
	}

	public void openOrCloseFriendMenu (bool state)
	{
		if ((!playerControllerManager.isPlayerMenuActive () || menuOpened) &&
		    (!playerControllerManager.isUsingDevice () || playerControllerManager.isPlayerDriving ()) &&
		    !pauseManager.isGamePaused ()) {

			menuOpened = state;

			pauseManager.openOrClosePlayerMenu (menuOpened, friendsMenuContent.transform, useBlurUIPanel);

			friendsMenuContent.SetActive (menuOpened);

			pauseManager.setIngameMenuOpenedState ("Friend List Manager", menuOpened, true);

			pauseManager.enableOrDisablePlayerMenu (menuOpened, true, false);

			if (playerControllerManager.isPlayerDriving ()) {
				GameObject currentVehicleObject = playerControllerManager.getCurrentVehicle ();

				if (currentVehicleObject != null) {
					vehicleHUDManager currentVehicleHUDManager = currentVehicleObject.GetComponent<vehicleHUDManager> ();

					if (currentVehicleHUDManager != null) {
						currentVehicleHUDManager.IKDrivingManager.setCameraAndWeaponsPauseState (menuOpened);
					}
				}
			}
		}
	}

	public void openOrCLoseFriendMenuFromTouch ()
	{
		openOrCloseFriendMenu (!menuOpened);
	}

	public void closeFriendMenu ()
	{
		openOrCloseFriendMenu (false);
	}

	public void addFriend (GameObject friend)
	{
		if (!checkIfContains (friend.transform)) {
			GameObject newFriendListElement = (GameObject)Instantiate (friendListElement, friendListElement.transform.position, 
				                                  Quaternion.identity, friendListElement.transform.parent);
			
			newFriendListElement.name = "New Friend List Element " + (friendsList.Count + 1);

			friendInfo newFriend = newFriendListElement.GetComponent<friendListElement> ().friendListElementInfo;

			healthManagement currentHealthManagement = friend.GetComponent<healthManagement> ();

			if (currentHealthManagement != null) {
				health currentHealth = currentHealthManagement.GetComponent<health> ();

				if (currentHealth != null) {
					newFriend.Name = currentHealth.settings.allyName;
				}
			}

			newFriend.friendTransform = friend.transform;

			newFriendListElement.SetActive (true);

			newFriend.friendRemoteEventSystem = friend.GetComponent<remoteEventSystem> ();

			newFriendListElement.transform.localScale = Vector3.one;

			newFriend.friendListElement = newFriendListElement;

			if (!canAIAttack (friend)) {
				newFriend.attackButton.gameObject.SetActive (false);
			}

			if (mainPlayerCharactersManager.isCharacterInList (friend)) {
				newFriend.switchCharacterButton.gameObject.SetActive (true);
			}

			friendsList.Add (newFriend);

			setCurrentStateText (friendsList.Count - 1, "Following");

			setFriendListName ();
		}
	}

	public void setFriendListName ()
	{
		for (i = 0; i < friendsList.Count; i++) {
			friendsList [i].nameText.text = (i + 1) + ".- " + friendsList [i].Name;
		}
	}

	public bool checkIfContains (Transform friend)
	{
		bool itContains = false;

		for (i = 0; i < friendsList.Count; i++) {
			if (friendsList [i].friendTransform == friend) {
				itContains = true;
			}
		}

		return itContains;
	}

	public void setIndividualOrder (Button pressedButton)
	{
		for (i = 0; i < friendsList.Count; i++) {

			currentFriendInfo = friendsList [i];
			Transform target = transform;

			string action = "";

			if (currentFriendInfo.attackButton == pressedButton) {
				if (canAIAttack (currentFriendInfo.friendTransform.gameObject)) {
					//print ("attack");
					Transform closestEnemy = getClosestEnemy ();
					target = closestEnemy;
					action = attackTargetOrderName;
				}
			} else if (currentFriendInfo.followButton == pressedButton) {
				//print ("follow");
				target = transform;
				action = followTargetOrderName;

			} else if (currentFriendInfo.waitButton == pressedButton) {
				//print ("wait");
				target = transform;
				action = waitOrderName;

			} else if (currentFriendInfo.hideButton == pressedButton) {
				//print ("hide");
				target = getClosestHidePosition (currentFriendInfo.friendTransform);
				action = hideOrderName;

			} else if (currentFriendInfo.switchCharacterButton == pressedButton) {
				target = friendsList [i].friendTransform;
				action = switchCharacterOrderName;
			}

			if (target && action != "") {
				currentFriendInfo.friendRemoteEventSystem.callRemoteEventWithTransform (action, target);

				setCurrentStateText (i, action);
			}
		}
	}

	public void setGeneralOrder (Button pressedButton)
	{
		Transform target = transform;

		string action = "";

		if (attackButton == pressedButton) {
			//print ("attack");
			action = attackTargetOrderName;

			target = getClosestEnemy ();
		} else if (followButton == pressedButton) {
			//print ("follow");
			action = followTargetOrderName;
		} else if (waitButton == pressedButton) {
			//print ("wait");
			action = waitOrderName;
		} else if (hideButton == pressedButton) {
			//print ("hide");
			action = hideOrderName;
		}

		for (i = 0; i < friendsList.Count; i++) {
			currentFriendInfo = friendsList [i];

			bool canDoAction = true;

			if (action.Equals (attackTargetOrderName)) {
				if (!canAIAttack (currentFriendInfo.friendTransform.gameObject)) {
					canDoAction = false;
				}
			}

			if (action.Equals (hideOrderName)) {
				target = getClosestHidePosition (currentFriendInfo.friendTransform);
			}

			if (canDoAction) {
				if (target != null) {
					currentFriendInfo.friendRemoteEventSystem.callRemoteEventWithTransform (action, target);

					setCurrentStateText (i, action);
				}
			}
		}
	}

	public void callToFriends ()
	{
		setGeneralOrder (followButton);
	}

	public void findFriendsInRadius (float radius)
	{
		Collider[] colliders = Physics.OverlapSphere (transform.position, radius);

		if (colliders.Length > 0) {
			
			for (int i = 0; i < colliders.Length; i++) {
				findObjectivesSystem currentFindObjectivesSystem = colliders [i].GetComponentInChildren<findObjectivesSystem> ();

				if (currentFindObjectivesSystem != null) {
					currentFindObjectivesSystem.checkTriggerInfo (mainCollider, true);
				}
			}
		}
	}

	public bool canAIAttack (GameObject AIFriend)
	{
		bool canAttack = false;

		findObjectivesSystem currentFindObjectivesSystem = AIFriend.GetComponentInChildren<findObjectivesSystem> ();

		if (currentFindObjectivesSystem != null) {
			if (currentFindObjectivesSystem.attackType != findObjectivesSystem.AIAttackType.none) {
				canAttack = true;
			}
		}

		return canAttack;
	}

	public void setCurrentStateText (int index, string state)
	{
		if (friendsList.Count > 0 && index < friendsList.Count) {
			friendsList [index].currentState.text = "State: " + state;
		}
	}

	public Transform getClosestEnemy ()
	{
		List<GameObject> fullEnemyList = new List<GameObject> ();

		GameObject closestEnemy;
		for (int i = 0; i < tagToLocate.Count; i++) {
			GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (tagToLocate [i]);

			fullEnemyList.AddRange (enemiesList);
		}

		closestEnemyList.Clear ();

		for (j = 0; j < fullEnemyList.Count; j++) {
			if (!applyDamage.checkIfDead (fullEnemyList [j])) {
				closestEnemyList.Add (fullEnemyList [j]);
			}
		}

		if (closestEnemyList.Count > 0) {
			float distance = Mathf.Infinity;
			int index = -1;

			for (j = 0; j < closestEnemyList.Count; j++) {
				float currentDistance = GKC_Utils.distance (closestEnemyList [j].transform.position, transform.position);
				if (currentDistance < distance) {
					distance = currentDistance;
					index = j;
				}
			}

			if (index != -1) {
				closestEnemy = closestEnemyList [index];
				return closestEnemy.transform;
			}
		}
		return null;
	}

	public Transform getClosestHidePosition (Transform AIFriend)
	{
		if (hidePositionsManager == null) {
			hidePositionsManager = FindObjectOfType<AIHidePositionsManager> ();
		}

		if (hidePositionsManager != null) {
			if (hidePositionsManager.hidePositionList.Count > 0) {
				float distance = Mathf.Infinity;

				int index = -1;

				for (j = 0; j < hidePositionsManager.hidePositionList.Count; j++) {
					float currentDistance = GKC_Utils.distance (AIFriend.position, hidePositionsManager.hidePositionList [j].position);

					if (currentDistance < distance) {
						distance = currentDistance;
						index = j;
					}
				}

				return hidePositionsManager.hidePositionList [index];
			}
		}

		return null;
	}

	public void removeFriend (Transform friend)
	{
		for (i = 0; i < friendsList.Count; i++) {
			if (friendsList [i].friendTransform == friend) {
				Destroy (friendsList [i].friendListElement);

				friendsList.RemoveAt (i);

				return;
			}
		}
	}

	public void removeAllFriends ()
	{
		for (i = 0; i < friendsList.Count; i++) {
			Destroy (friendsList [i].friendListElement);
		}

		friendsList.Clear ();
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	public void inputOpenOrCloseFriendListMenu ()
	{
		if (friendManagerEnabled) {
			if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
				return;
			}

			openOrCloseFriendMenu (!menuOpened);
		}
	}

	public void inputSwitchToNextCharacter ()
	{
		if (!playerControllerManager.isPlayerMenuActive () && (!playerControllerManager.isUsingDevice () || playerControllerManager.isPlayerDriving ()) && !pauseManager.isGamePaused ()) {
			mainPlayerCharactersManager.inputSetNextCharacterToControl ();
		}
	}

	public void setFriendManagerEnabledState (bool state)
	{
		friendManagerEnabled = state;
	}

	[System.Serializable]
	public class friendInfo
	{
		public string Name;
		public Transform friendTransform;
		public Text nameText;
		public Text currentState;
		public GameObject friendListElement;
		public Button attackButton;
		public Button followButton;
		public Button waitButton;
		public Button hideButton;

		public Button switchCharacterButton;

		public remoteEventSystem friendRemoteEventSystem;
	}
}