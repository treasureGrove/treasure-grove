using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customCharacterControllerBaseBuilder : MonoBehaviour
{
	public List<buildPlayer.settingsInfoCategory> settingsInfoCategoryList = new List<buildPlayer.settingsInfoCategory> ();

	public customCharacterControllerBase mainCustomCharacterControllerBase;

	buildPlayer.settingsInfo currentSettingsInfo;
	buildPlayer.settingsInfoCategory currentSettingsInfoCategory;

	public void adjustSettingsFromEditor ()
	{
		adjustSettings ();
	}

	public void adjustSettings ()
	{
		for (int i = 0; i < settingsInfoCategoryList.Count; i++) { 

			currentSettingsInfoCategory = settingsInfoCategoryList [i];

			adjustSettingsFromList (currentSettingsInfoCategory.settingsInfoList);
		}

		GKC_Utils.updateComponent (mainCustomCharacterControllerBase);
	}

	public void adjustSettingsFromList (List<buildPlayer.settingsInfo> settingsList)
	{
		print ("\n\n");

		print ("Setting list applied to character: \n\n");

		for (int j = 0; j < settingsList.Count; j++) { 

			currentSettingsInfo = settingsList [j];

			if (currentSettingsInfo.settingEnabled) {

				if (currentSettingsInfo.useBoolState) {
					currentSettingsInfo.eventToSetBoolState.Invoke (currentSettingsInfo.boolState);

					if (currentSettingsInfo.eventToSetBoolState.GetPersistentEventCount () > 0) {
						GKC_Utils.updateComponent (currentSettingsInfo.eventToSetBoolState.GetPersistentTarget (0));
					}

					print ("Setting " + currentSettingsInfo.Name + " set as " + currentSettingsInfo.useBoolState);
				}

				if (currentSettingsInfo.useFloatValue) {
					currentSettingsInfo.eventToSetFloatValue.Invoke (currentSettingsInfo.floatValue);

					if (currentSettingsInfo.eventToSetFloatValue.GetPersistentEventCount () > 0) {
						GKC_Utils.updateComponent (currentSettingsInfo.eventToSetFloatValue.GetPersistentTarget (0));
					}

					print ("Setting " + currentSettingsInfo.Name + " set as " + currentSettingsInfo.floatValue);
				}

				if (currentSettingsInfo.useStringValue) {
					currentSettingsInfo.eventToSetStringValue.Invoke (currentSettingsInfo.stringValue);

					if (currentSettingsInfo.eventToSetStringValue.GetPersistentEventCount () > 0) {
						GKC_Utils.updateComponent (currentSettingsInfo.eventToSetStringValue.GetPersistentTarget (0));
					}

					print ("Setting " + currentSettingsInfo.Name + " set as " + currentSettingsInfo.stringValue);
				}

				if (currentSettingsInfo.useVector3Value) {
					currentSettingsInfo.eventToSetVector3Value.Invoke (currentSettingsInfo.vector3Value);

					if (currentSettingsInfo.eventToSetVector3Value.GetPersistentEventCount () > 0) {
						GKC_Utils.updateComponent (currentSettingsInfo.eventToSetVector3Value.GetPersistentTarget (0));
					}

					print ("Setting " + currentSettingsInfo.Name + " set as " + currentSettingsInfo.vector3Value);
				}

				if (currentSettingsInfo.useRegularValue) {
					if (currentSettingsInfo.regularValue) {
						currentSettingsInfo.eventToEnableActiveValue.Invoke ();

						if (currentSettingsInfo.eventToEnableActiveValue.GetPersistentEventCount () > 0) {
							GKC_Utils.updateComponent (currentSettingsInfo.eventToEnableActiveValue.GetPersistentTarget (0));
						}
					} else {
						currentSettingsInfo.eventToDisableActiveValue.Invoke ();

						if (currentSettingsInfo.eventToDisableActiveValue.GetPersistentEventCount () > 0) {
							GKC_Utils.updateComponent (currentSettingsInfo.eventToDisableActiveValue.GetPersistentTarget (0));
						}
					}

					print ("Setting " + currentSettingsInfo.Name + " set as " + currentSettingsInfo.regularValue);
				}
			}
		}

		print ("Character Settings Applied\n\n\n");

		GKC_Utils.updateDirtyScene ("Updated Settings", gameObject);
	}
}
