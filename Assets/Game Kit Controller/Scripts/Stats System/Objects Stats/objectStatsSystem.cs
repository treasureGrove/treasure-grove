using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class objectStatsSystem : MonoBehaviour
{
	[Header ("Stats Settings")]
	[Space]

	public string objectName;

	public bool useStats;

	public float loadStatsDelay;

	public List<statInfo> statInfoList = new List<statInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool characterLocated;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject mainCharacterGameObject;

	public objectStatsInfoTemplate mainObjectStatsInfoTemplate;


	string currentStatNameToCheck;

	public void addObjectStatsToMainManager ()
	{
		checkMainCharacterGameObject ();

		if (characterLocated) {
			playerComponentsManager currentPlayerComponentsManager = mainCharacterGameObject.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				objectsStatsSystem currentobjectsStatsSystem = currentPlayerComponentsManager.getObjectsStatsSystem ();
					
				if (currentobjectsStatsSystem != null) {
					currentobjectsStatsSystem.addObjectStatsSystem (this);
				}
			}
		}
	}

	public void setMainCharacterGameObject (GameObject newObject)
	{
		mainCharacterGameObject = newObject;
	}

	public void checkMainCharacterGameObject ()
	{
		if (characterLocated) {
			return;
		}

		if (mainCharacterGameObject == null) {
			getMainCharacterGameObject ();
		}

		characterLocated = mainCharacterGameObject != null;
	}

	public virtual void getMainCharacterGameObject ()
	{

	}

	public void checkStatsStateOnLoad ()
	{
		if (useStats) {
			if (loadStatsDelay > 0) {
				setStatsOnLoadWithDelay ();
			} else {
				setStatsOnLoad ();
			}
		}
	}

	public void setStatsOnLoadWithDelay ()
	{
		StartCoroutine (setStatsOnLoadCoroutine ());
	}

	IEnumerator setStatsOnLoadCoroutine ()
	{
		yield return new WaitForSeconds (loadStatsDelay);

		setStatsOnLoad ();
	}

	public void setStatsOnLoad ()
	{
		if (useStats) {
			if (showDebugPrint) {
				print ("\n");
				print ("SETTING STATS INFO ON " + gameObject.name);

				print ("\n");
			}

			for (int i = 0; i < statInfoList.Count; i++) {
				statInfo currentStatInfo = statInfoList [i];

				if (currentStatInfo.statIsAmount) {
					currentStatInfo.eventToInitializeFloatStat.Invoke (currentStatInfo.currentFloatValue);
				} else {
					currentStatInfo.eventToInitializeBoolStat.Invoke (currentStatInfo.currentBoolState);
				}
			}
		}
	}

	public void setCurrentStatNameToSave (string newValue)
	{
		currentStatNameToCheck = newValue;
	}

	public void setCurrentFloatValueToSave (float newValue)
	{
		if (useStats) {
			for (int i = 0; i < statInfoList.Count; i++) {
				statInfo currentStatInfo = statInfoList [i];

				if (currentStatInfo.Name.Equals (currentStatNameToCheck)) {
					currentStatInfo.currentFloatValue = newValue;

					return;
				}
			}
		}
	}

	public void setCurrentBoolValueToSave (bool newValue)
	{
		if (useStats) {
			for (int i = 0; i < statInfoList.Count; i++) {
				statInfo currentStatInfo = statInfoList [i];

				if (currentStatInfo.Name.Equals (currentStatNameToCheck)) {
					currentStatInfo.currentBoolState = newValue;

					return;
				}
			}
		}
	}

	public void checkEventOnStatsSave ()
	{
		if (useStats) {
			for (int i = 0; i < statInfoList.Count; i++) {
				statInfo currentStatInfo = statInfoList [i];

				currentStatNameToCheck = currentStatInfo.Name;

				if (currentStatInfo.statIsAmount) {
					currentStatInfo.eventToGetFloatStat.Invoke ();
				} else {
					currentStatInfo.eventToGetBoolStat.Invoke ();
				}
			}
		}
	}

	[System.Serializable]
	public class statInfo
	{
		public string Name;

		public bool statIsAmount = true;

		[Space]

		public float currentFloatValue;

		public eventParameters.eventToCallWithAmount eventToInitializeFloatStat;

		public UnityEvent eventToGetFloatStat;

		[Space]
		[Space]

		public bool currentBoolState;

		public eventParameters.eventToCallWithBool eventToInitializeBoolStat;

		public UnityEvent eventToGetBoolStat;
	}
}
