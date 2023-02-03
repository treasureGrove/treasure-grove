using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class graphicQualitySettingsSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool graphicSettingsEnabled = true;

	public bool canChangeGameWindowedStateEnabled = true;

	public bool canChangeAntiAliasingEnabled = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public int currentSettingIndex;
	public int currentResolutionIndex;

	public bool showDebugPrint;

	[Space]
	[Header ("Quality Settings")]
	[Space]

	public List<qualitySettingInfo> qualitySettingInfoList = new List<qualitySettingInfo> ();

	[Space]
	[Header ("Resolution Settings")]
	[Space]

	public List<resolutionSettingInfo> resolutionSettingInfoList = new List<resolutionSettingInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnResolutionChange;

	public UnityEvent eventOnResolutionChange;

	public void setGraphicQualityValue (string newValue)
	{
		if (!graphicSettingsEnabled) {
			return;
		}

		int currentIndex = qualitySettingInfoList.FindIndex (s => s.Name.Equals (newValue));

		if (currentIndex > -1) {
			qualitySettingInfo currentQualitySettingInfo = qualitySettingInfoList [currentIndex];

			if (!currentQualitySettingInfo.isCurrentSetting) {
				for (int i = 0; i < qualitySettingInfoList.Count; i++) {
					qualitySettingInfoList [i].isCurrentSetting = false;
				}

				currentQualitySettingInfo.isCurrentSetting = true;

				currentSettingIndex = currentIndex;

				if (QualitySettings.GetQualityLevel () != currentQualitySettingInfo.qualityValue) {

					QualitySettings.SetQualityLevel (currentQualitySettingInfo.qualityValue);

					if (currentQualitySettingInfo.useEventsOnStateChange) {
						currentQualitySettingInfo.eventOnStateChange.Invoke ();
					}
				}

				if (showDebugPrint) {
					print ("Quality Settings configured as " + currentQualitySettingInfo.Name);
				}
			}
		} else {
			if (showDebugPrint) {
				print ("Quality Settings not found " + newValue);
			}
		}
	}

	public void setScreenResolution (string newValue)
	{
		if (!graphicSettingsEnabled) {
			return;
		}

		int currentIndex = resolutionSettingInfoList.FindIndex (s => s.Name.Equals (newValue));

		if (currentIndex > -1) {
			resolutionSettingInfo currentResolutionSettingInfo = resolutionSettingInfoList [currentIndex];

			if (!currentResolutionSettingInfo.isCurrentSetting) {
				for (int i = 0; i < resolutionSettingInfoList.Count; i++) {
					resolutionSettingInfoList [i].isCurrentSetting = false;
				}

				currentResolutionSettingInfo.isCurrentSetting = true;

				currentResolutionIndex = currentIndex;

				if (Screen.currentResolution.width != currentResolutionSettingInfo.resolutionValues.x ||
				    Screen.currentResolution.height != currentResolutionSettingInfo.resolutionValues.y) {
				
					Screen.SetResolution ((int)currentResolutionSettingInfo.resolutionValues.x, (int)currentResolutionSettingInfo.resolutionValues.y, true);
				}

				if (currentResolutionSettingInfo.useEventsOnStateChange) {
					currentResolutionSettingInfo.eventOnStateChange.Invoke ();
				}

				if (useEventsOnResolutionChange) {
					eventOnResolutionChange.Invoke ();
				}

				if (showDebugPrint) {
					print ("Resolution Settings configured as " + currentResolutionSettingInfo.Name);
				}
			}
		} else {
			if (showDebugPrint) {
				print ("Resolution Settings not found " + newValue);
			}
		}
	}

	public void setGameWindowedState (bool state)
	{
		if (!graphicSettingsEnabled) {
			return;
		}

		if (!canChangeGameWindowedStateEnabled) {
			return;
		}

		resolutionSettingInfo currentResolutionSettingInfo = resolutionSettingInfoList [currentResolutionIndex];

		if (Screen.fullScreen != state) {
			Screen.SetResolution ((int)currentResolutionSettingInfo.resolutionValues.x, (int)currentResolutionSettingInfo.resolutionValues.y, state);
		}

		if (showDebugPrint) {
			print ("Windowed Settings configured as " + state);
		}
	}

	public void setAntiAliasingState (string valueString)
	{
		int newValue = -1;

		if (int.TryParse (valueString, out newValue)) {
			setAntiAliasingState (newValue);
		}
	}

	public void setAntiAliasingState (int newValue)
	{
		if (!graphicSettingsEnabled) {
			return;
		}

		if (!canChangeAntiAliasingEnabled) {
			return;
		}

		if (QualitySettings.antiAliasing != newValue) {
			QualitySettings.antiAliasing = newValue;
		}

		if (showDebugPrint) {
			print ("AntiAliasing Settings configured as " + newValue);
		}
	}


	[System.Serializable]
	public class qualitySettingInfo
	{
		public string Name;

		public int qualityValue;

		public bool isCurrentSetting;

		[Space]
		[Space]

		public bool useEventsOnStateChange;

		public UnityEvent eventOnStateChange;
	}

	[System.Serializable]
	public class resolutionSettingInfo
	{
		public string Name;

		public Vector2 resolutionValues;

		public bool isCurrentSetting;

		[Space]
		[Space]

		public bool useEventsOnStateChange;

		public UnityEvent eventOnStateChange;
	}
}
