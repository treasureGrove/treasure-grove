using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerStatsSystem : MonoBehaviour
{
	public bool playerStatsActive = true;

	public bool initializeStatsValuesAtStartActive = true;

	public bool initializeStatsOnlyWhenLoadingGame;

	public bool saveCurrentPlayerStatsToSaveFile;

	public List<statInfo> statInfoList = new List<statInfo> ();

	float currentMaxAmount;

	public bool isLoadingGame;

	public bool initializeValuesWhenNotLoadingFromTemplate;

	public statsSettingsTemplate mainStatsSettingsTemplate;


	public void initializeStatsValues ()
	{
		if (!playerStatsActive) {
			return;
		}

		bool initializingValuesFromTemplate = false;

		if (initializeStatsValuesAtStartActive) {
			if (initializeValuesWhenNotLoadingFromTemplate && !isLoadingGame) {
				loadSettingsFromTemplate (false);

				initializingValuesFromTemplate = true;
			}
		}
			
		statInfo currentStatInfo = null;

		if (initializeStatsValuesAtStartActive && (!initializeStatsOnlyWhenLoadingGame || isLoadingGame || initializingValuesFromTemplate)) {
			int statInfoListCount = statInfoList.Count;

			for (int k = 0; k < statInfoListCount; k++) {
				currentStatInfo = statInfoList [k];

				if (currentStatInfo.initializeStatWithThisValue) {
					if (currentStatInfo.statIsAmount) {
						if (currentStatInfo.useCustomStatTypeForEvents) {
							currentStatInfo.customStatType.eventToInitializeStat (currentStatInfo.currentValue);
						} else {
							currentStatInfo.eventToInitializeStat.Invoke (currentStatInfo.currentValue);
						}
					} else {
						if (currentStatInfo.useCustomStatTypeForEvents) {
							currentStatInfo.customStatType.eventToInitializeBoolStat (currentStatInfo.currentBoolState);
						} else {
							currentStatInfo.eventToInitializeBoolStat.Invoke (currentStatInfo.currentBoolState);
						}
					}
				} else {
					if (currentStatInfo.statIsAmount) {
						if (currentStatInfo.useCustomStatTypeForEvents) {
							currentStatInfo.customStatType.eventToInitializeStatOnComponent ();
						} else {
							currentStatInfo.eventToInitializeStatOnComponent.Invoke ();
						}
					} else {
						if (currentStatInfo.useCustomStatTypeForEvents) {
							currentStatInfo.customStatType.eventToInitializeBoolStatOnComponent ();
						} else {
							currentStatInfo.eventToInitializeBoolStatOnComponent.Invoke ();
						}
					}
				}
			}
		}
	}

	public float getStatMaxAmountByIndex (int statIndex)
	{
		statInfo currentStatInfo = statInfoList [statIndex];

		currentMaxAmount = currentStatInfo.maxAmount;

		if (currentStatInfo.useOtherStatAsMaxAmount) {
			currentMaxAmount = getStatValue (currentStatInfo.otherStatAsMaxAmountName);

			if (currentMaxAmount < 0) {
				currentMaxAmount = currentStatInfo.currentValue;
			}
		}

		return currentMaxAmount;
	}

	public float getStatMaxAmountByName (string statName)
	{
		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				if (currentStatInfo.useMaxAmount) {
					currentMaxAmount = currentStatInfo.maxAmount;

					if (currentStatInfo.useOtherStatAsMaxAmount) {
						currentMaxAmount = getStatValue (currentStatInfo.otherStatAsMaxAmountName);

						if (currentMaxAmount < 0) {
							currentMaxAmount = currentStatInfo.currentValue;
						}
					}

					return currentMaxAmount;
				} else {
					return -1;
				}
			}
		}

		return -1;
	}

	public bool isStatOnMaxAmount (string statName)
	{
		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				if (currentStatInfo.useMaxAmount) {
					currentMaxAmount = currentStatInfo.maxAmount;

					if (currentStatInfo.useOtherStatAsMaxAmount) {
						currentMaxAmount = getStatValue (currentStatInfo.otherStatAsMaxAmountName);

						if (currentMaxAmount < 0) {
							currentMaxAmount = currentStatInfo.currentValue;
						}
					}

					if (currentStatInfo.currentValue >= currentMaxAmount) {
						return true;
					} else {
						return false;
					}
				} else {
					return false;
				}
			}
		}

		return false;
	}

	string temporalStatName;

	public void setTemporalStatName (string statName)
	{
		temporalStatName = statName;
	}

	public void increasePlayerStateOfTemporalStat (float statExtraValue)
	{
		if (temporalStatName != "") {
			increasePlayerStat (temporalStatName, statExtraValue);

			temporalStatName = "";
		}
	}

	public void increasePlayerStat (string statName, float statExtraValue)
	{
		if (!playerStatsActive) {
			return;
		}

		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				currentStatInfo.currentValue += statExtraValue;

				if (currentStatInfo.useMaxAmount) {
					currentStatInfo.currentValue = Mathf.Clamp (currentStatInfo.currentValue, 0, getStatMaxAmountByIndex (k));
				}

				if (currentStatInfo.useCustomStatTypeForEvents) {
					currentStatInfo.customStatType.eventToIncreaseStat (statExtraValue);
				} else {
					currentStatInfo.eventToIncreaseStat.Invoke (statExtraValue);
				}

				return;
			}
		}
	}

	public void usePlayerStat (string statName, float statValueToUse)
	{
		if (!playerStatsActive) {
			return;
		}

		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				currentStatInfo.currentValue -= statValueToUse;

				if (currentStatInfo.currentValue < 0) {
					currentStatInfo.currentValue = 0;
				}

				if (currentStatInfo.useCustomStatTypeForEvents) {
					currentStatInfo.customStatType.eventToUseStat (statValueToUse);
				} else {
					currentStatInfo.eventToUseStat.Invoke (statValueToUse);
				}

				return;
			}
		}
	}

	public void usePlayerStatByIndex (int statIndex, float statValueToUse)
	{
		if (!playerStatsActive) {
			return;
		}

		if (statIndex < 0) {
			return;
		}

		statInfo currentStatInfo = statInfoList [statIndex];

		currentStatInfo.currentValue -= statValueToUse;

		if (currentStatInfo.currentValue < 0) {
			currentStatInfo.currentValue = 0;
		}

		if (currentStatInfo.useCustomStatTypeForEvents) {
			currentStatInfo.customStatType.eventToUseStat (statValueToUse);
		} else {
			currentStatInfo.eventToUseStat.Invoke (statValueToUse);
		}
	}

	public void addOrRemovePlayerStatAmount (string statName, float amountToUse)
	{
		if (!playerStatsActive) {
			return;
		}

		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				currentStatInfo.currentValue += amountToUse;

				if (currentStatInfo.currentValue < 0) {
					currentStatInfo.currentValue = 0;
				} else {
					if (currentStatInfo.useMaxAmount) {
						currentStatInfo.currentValue = Mathf.Clamp (currentStatInfo.currentValue, 0, getStatMaxAmountByIndex (k));
					}
				}

				if (amountToUse > 0) {
					if (currentStatInfo.useCustomStatTypeForEvents) {
						currentStatInfo.customStatType.eventToAddAmount (amountToUse);
					} else {
						currentStatInfo.eventToAddAmount.Invoke (amountToUse);
					}
				} else {
					if (currentStatInfo.useCustomStatTypeForEvents) {
						currentStatInfo.customStatType.eventToUseStat (-amountToUse);
					} else {
						currentStatInfo.eventToUseStat.Invoke (-amountToUse);
					}
				}

				return;
			}
		}
	}

	public float getStatValue (string statName)
	{
		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				return currentStatInfo.currentValue;
			}
		}

		return -1;
	}

	public bool checkIfStatValueAvailable (string statName, int statAmount)
	{
		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				return currentStatInfo.currentValue >= statAmount;
			}
		}

		return false;
	}

	public float getStatValueByIndex (int statIndex)
	{
		if (statIndex < 0) {
			return -1;
		}

		return statInfoList [statIndex].currentValue;
	}

	public int getStatValueIndex (string statName)
	{
		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				return k;
			}
		}

		return -1;
	}

	public void updateStatValue (string statName, float newValue)
	{
		if (!playerStatsActive) {
			return;
		}

		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				currentStatInfo.currentValue = newValue;

				if (currentStatInfo.useMaxAmount) {
					currentStatInfo.currentValue = Mathf.Clamp (currentStatInfo.currentValue, 0, getStatMaxAmountByIndex (k));
				}

				if (currentStatInfo.useEventToSendValueOnUpdateState) {
					currentStatInfo.eventToSendValueOnUpdateState.Invoke (currentStatInfo.currentValue);
				}

				return;
			}
		}
	}

	public void enableOrDisableBoolPlayerStat (string statName, bool boolStateValue)
	{
		if (!playerStatsActive) {
			return;
		}

		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				currentStatInfo.currentBoolState = boolStateValue;

				if (currentStatInfo.useCustomStatTypeForEvents) {
					currentStatInfo.customStatType.eventToActivateBoolStat (boolStateValue);
				} else {
					currentStatInfo.eventToActivateBoolStat.Invoke (boolStateValue);
				}

				return;
			}
		}
	}

	public bool getBoolStatValue (string statName)
	{
		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				return currentStatInfo.currentBoolState;
			}
		}

		return false;
	}

	public void updateBoolStatValue (string statName, bool boolStateValue)
	{
		if (!playerStatsActive) {
			return;
		}

		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int k = 0; k < statInfoListCount; k++) {
			currentStatInfo = statInfoList [k];

			if (currentStatInfo.Name.Equals (statName)) {
				currentStatInfo.currentBoolState = boolStateValue;
			}
		}
	}

	public void setPlayerStatsActiveState (bool state)
	{
		playerStatsActive = state;
	}

	public void saveSettingsToTemplate ()
	{
		if (mainStatsSettingsTemplate == null) {
			return;
		}

		mainStatsSettingsTemplate.statTemplateInfoList.Clear ();

		int statInfoListCount = statInfoList.Count;

		for (int i = 0; i < statInfoListCount; i++) {
			statTemplateInfo newStatTemplateInfo = new statTemplateInfo ();

			newStatTemplateInfo.Name = statInfoList [i].Name;

			newStatTemplateInfo.currentValue = statInfoList [i].currentValue;

			newStatTemplateInfo.currentBoolState = statInfoList [i].currentBoolState;

			mainStatsSettingsTemplate.statTemplateInfoList.Add (newStatTemplateInfo);
		}

		updateComponent ();

		GKC_Utils.updateDirtyScene ("Save Stats Settings To Template", gameObject);

		print ("States values saved to template");
	}

	public void loadSettingsFromTemplate (bool loadingFromEditor)
	{
		if (mainStatsSettingsTemplate == null) {
			return;
		}

		statInfo currentStatInfo = null;

		int statInfoListCount = statInfoList.Count;

		for (int i = 0; i < statInfoListCount; i++) {
			int statIndex = mainStatsSettingsTemplate.statTemplateInfoList.FindIndex (a => a.Name == statInfoList [i].Name);

			if (statIndex > -1) {

				statTemplateInfo newStatTemplateInfo = mainStatsSettingsTemplate.statTemplateInfoList [statIndex];

				currentStatInfo = statInfoList [i];
			
				currentStatInfo.currentBoolState = newStatTemplateInfo.currentBoolState;
				currentStatInfo.currentValue = newStatTemplateInfo.currentValue;
			}
		}

		if (loadingFromEditor) {
			updateComponent ();

			GKC_Utils.updateDirtyScene ("Load Stats Settings From Template", gameObject);

			print ("Stats values loaded from template");
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}


	[System.Serializable]
	public class statInfo
	{
		public string Name;

		public bool statIsAmount = true;

		public float currentValue;

		public eventParameters.eventToCallWithAmount eventToInitializeStat;

		public eventParameters.eventToCallWithAmount eventToIncreaseStat;

		public eventParameters.eventToCallWithAmount eventToUseStat;

		public eventParameters.eventToCallWithAmount eventToAddAmount;

		public bool useEventToSendValueOnUpdateState;

		public eventParameters.eventToCallWithAmount eventToSendValueOnUpdateState;

		public bool currentBoolState;

		public eventParameters.eventToCallWithBool eventToInitializeBoolStat;

		public eventParameters.eventToCallWithBool eventToActivateBoolStat;

		public bool initializeStatWithThisValue = true;
		public UnityEvent eventToInitializeStatOnComponent;
		public UnityEvent eventToInitializeBoolStatOnComponent;

		public bool useCustomStatTypeForEvents;
		public statType customStatType;

		public bool useMaxAmount;
		public float maxAmount;
		public bool useOtherStatAsMaxAmount;
		public string otherStatAsMaxAmountName;
	}

	[System.Serializable]
	public class statTemplateInfo
	{
		public string Name;

		public float currentValue;

		public bool currentBoolState;
	}
}