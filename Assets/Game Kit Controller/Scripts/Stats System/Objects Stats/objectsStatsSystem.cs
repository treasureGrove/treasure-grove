using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectsStatsSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool objectsStatsSystemEnabled = true;

	public List<objectStatsSystem> objectStatsSystemList = new List<objectStatsSystem> ();

	[Space]
	[Header ("Object Stats Template Settings")]
	[Space]

	public List<objectStatsInfoTemplateData> objectStatsInfoTemplateDataList = new List<objectStatsInfoTemplateData> ();

	[Space]
	[Header ("Object Stats List Settings")]
	[Space]

	public List<objectStatsInfo> objectStatsInfoList = new List<objectStatsInfo> ();



	public List<objectStatsInfoTemplateData> getObjectStatsInfoTemplateDataList ()
	{
		return objectStatsInfoTemplateDataList;
	}

	public void addObjectStatsSystem (objectStatsSystem newObjectStatsSystem)
	{
		if (!objectsStatsSystemEnabled) {
			return;
		}

		if (!objectStatsSystemList.Contains (newObjectStatsSystem)) {
			objectStatsSystemList.Add (newObjectStatsSystem);
		}
	}

	public List<objectStatInfo> getStatsFromObjectByName (string objectName)
	{
		if (!objectsStatsSystemEnabled) {
			return null;
		}

		int currentObjectIndex = objectStatsSystemList.FindIndex (s => s.objectName.Equals (objectName));

		if (currentObjectIndex > -1) {
			return objectStatsSystemList [currentObjectIndex].mainObjectStatsInfoTemplate.objectStatInfoList;
		}

		return null;
	}

	public bool objectCanBeUpgraded (string objectName)
	{
		if (!objectsStatsSystemEnabled) {
			return false;
		}

		int currentObjectIndex = objectStatsSystemList.FindIndex (s => s.objectName.Equals (objectName));

		if (currentObjectIndex > -1) {
			return true;
		}

		return false;
	}

	public void increaseStat (string statName, float statExtraValue)
	{
		if (!objectsStatsSystemEnabled) {
			return;
		}

//		statInfo currentStatInfo = null;
//
//		int statInfoListCount = statInfoList.Count;
//
//		for (int k = 0; k < statInfoListCount; k++) {
//			currentStatInfo = statInfoList [k];
//
//			if (currentStatInfo.Name.Equals (statName)) {
//				currentStatInfo.currentValue += statExtraValue;
//
//				if (currentStatInfo.useMaxAmount) {
//					currentStatInfo.currentValue = Mathf.Clamp (currentStatInfo.currentValue, 0, getStatMaxAmountByIndex (k));
//				}
//
//				if (currentStatInfo.useCustomStatTypeForEvents) {
//					currentStatInfo.customStatType.eventToIncreaseStat (statExtraValue);
//				} else {
//					currentStatInfo.eventToIncreaseStat.Invoke (statExtraValue);
//				}
//
//				return;
//			}
//		}
	}


	public void setObjectsStatsSystemEnabledState (bool state)
	{
		objectsStatsSystemEnabled = state;
	}

	public void setObjectsStatsSystemEnabledStateFromEditor (bool state)
	{
		setObjectsStatsSystemEnabledState (state);
	}
}
