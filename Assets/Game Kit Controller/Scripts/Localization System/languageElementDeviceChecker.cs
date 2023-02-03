using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class languageElementDeviceChecker : languageElementChecker
{
	[Header ("Custom Settings")]
	[Space]

	public deviceStringAction mainDeviceStringAction;

	public override void updateLanguageOnElement (string currentLanguage)
	{
		for (int i = 0; i < UIElementLanguageInfoList.Count; i++) {
			if (UIElementLanguageInfoList [i].language.Equals (currentLanguage)) {
				mainDeviceStringAction.setNewDeviceName (UIElementLanguageInfoList [i].textContent);

				mainDeviceStringAction.setDeviceAction (UIElementLanguageInfoList [i].extraTextContent);

				return;
			}
		}
	}
}
