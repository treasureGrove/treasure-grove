using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brainwashAIState : MonoBehaviour
{

	[Header ("Main Settings")]
	[Space]

	public string factionToConfigure = "Friend Soldiers";

	public bool setNewName;
	public string newName;
	public bool AIIsFriend;

	public string newTag = "friend";

	public bool followPartnerOnTriggerEnabled = true;

	public bool setPlayerAsPartner = true;

	public bool useRemoteEvents;

	public List<string> remoteEventNameList = new List<string> ();

	public GameObject characterGameObject;

	public GameObject newPartnerGameObject;


	public void activateState ()
	{
		if (characterGameObject == null) {
			return;
		}

		GKC_Utils.activateBrainWashOnCharacter (characterGameObject, factionToConfigure, newTag, setNewName, newName, 
			AIIsFriend, followPartnerOnTriggerEnabled, setPlayerAsPartner,
			newPartnerGameObject, useRemoteEvents, remoteEventNameList);
	}

	public void setnewPartnerGameObject (GameObject newObject)
	{
		newPartnerGameObject = newObject;
	}
}
