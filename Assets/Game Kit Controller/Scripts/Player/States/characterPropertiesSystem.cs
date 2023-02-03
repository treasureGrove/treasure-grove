using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterPropertiesSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool characeterStatesAffectedEnabled = true;

	[Space]
	[Header ("States List Settings")]
	[Space]

	public List<characterStateAffectedInfo> characterStateAffectedInfoList = new List<characterStateAffectedInfo> ();

	public void activateStateAffected (string stateName, float stateDuration, float stateAmount)
	{
		if (!characeterStatesAffectedEnabled) {
			return;
		}

		for (int i = 0; i < characterStateAffectedInfoList.Count; i++) {
			if (characterStateAffectedInfoList [i].stateAffectedName.Equals (stateName) && characterStateAffectedInfoList [i].stateEnabled) {
				characterStateAffectedInfoList [i].activateStateAffected (stateDuration, stateAmount);
			}
		}
	}

	public void activateStateAffected (string stateName)
	{
		activateOrDeactivateStateAffected (stateName, true);
	}

	public void deactivateStateAffected (string stateName)
	{
		activateOrDeactivateStateAffected (stateName, false);
	}

	public void activateOrDeactivateStateAffected (string stateName, bool state)
	{
		if (!characeterStatesAffectedEnabled) {
			return;
		}

		for (int i = 0; i < characterStateAffectedInfoList.Count; i++) {
			if (characterStateAffectedInfoList [i].stateAffectedName.Equals (stateName) && characterStateAffectedInfoList [i].stateEnabled) {
				characterStateAffectedInfoList [i].activateStateAffected (state);
			}
		}
	}

	public characterStateAffectedInfo getCharacterStateAffectedInfoByName (string stateName)
	{
		for (int i = 0; i < characterStateAffectedInfoList.Count; i++) {
			if (characterStateAffectedInfoList [i].stateAffectedName.Equals (stateName)) {
				return characterStateAffectedInfoList [i];
			}
		}

		return null;
	}

	public GameObject getCharacterStateAffectedInfoGameObjectByName (string stateName)
	{
		for (int i = 0; i < characterStateAffectedInfoList.Count; i++) {
			if (characterStateAffectedInfoList [i].stateAffectedName.Equals (stateName)) {
				return characterStateAffectedInfoList [i].gameObject;
			}
		}

		return null;
	}
}
