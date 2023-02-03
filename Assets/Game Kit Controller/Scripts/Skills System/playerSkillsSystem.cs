using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerSkillsSystem : MonoBehaviour
{
	public bool playerSkillsActive = true;

	public bool initializeSkillsValuesAtStartActive = true;

	public bool initializeSkillsOnlyWhenLoadingGame;

	public bool saveCurrentPlayerSkillsToSaveFile;

	public List<skillCategoryInfo> skillCategoryInfoList = new List<skillCategoryInfo> ();

	public skillInfo currentSkillInfo;

	public skillLevelInfo currentSkillLevelInfo;

	public bool isLoadingGame;

	public bool initializeValuesWhenNotLoadingFromTemplate;

	public skillSettingsTemplate mainSkillSettingsTemplate;

	public bool isLoadingGameActive ()
	{
		return isLoadingGame;
	}


	public void initializeSkillsValues ()
	{
		if (!playerSkillsActive) {
			return;
		}

		bool initializingValuesFromTemplate = false;

		if (initializeSkillsValuesAtStartActive) {
			if (initializeValuesWhenNotLoadingFromTemplate && !isLoadingGame) {
				loadSettingsFromTemplate (false);

				initializingValuesFromTemplate = true;
			}
		}

		if (initializeSkillsValuesAtStartActive && (!initializeSkillsOnlyWhenLoadingGame || isLoadingGame || initializingValuesFromTemplate)) {
			int skillCategoryInfoListCount = skillCategoryInfoList.Count;

			for (int i = 0; i < skillCategoryInfoListCount; i++) {

				skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];
					
				int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

				for (int k = 0; k < skillInfoListCount; k++) {

					currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

					if (currentSkillInfo.skillEnabled) {

						if (initializingValuesFromTemplate) {
							if (currentSkillInfo.skillCompleteFromTemplate) {
								currentSkillInfo.currentValue = currentSkillInfo.valueToConfigure;

								currentSkillInfo.currentBoolState = currentSkillInfo.boolStateToConfigure;

								currentSkillInfo.skillUnlocked = true;

								currentSkillInfo.skillActive = true;

								currentSkillInfo.currentSkillLevel = currentSkillInfo.skillLevelInfoList.Count - 1;
							}
						}

						if (currentSkillInfo.useFloatValue) {
							currentSkillInfo.eventToInitializeSkill.Invoke (currentSkillInfo.currentValue);
						}

						if (currentSkillInfo.useBoolValue) {
							if (currentSkillInfo.useTwoEventsForActiveAndNotActive) {
								if (currentSkillInfo.currentBoolState) {
									currentSkillInfo.eventToInitializeSkillActive.Invoke ();
								} else {
									currentSkillInfo.eventToInitializeSkillNotActive.Invoke ();
								}

							} else {
								currentSkillInfo.eventToInitializeBoolSkill.Invoke (currentSkillInfo.currentBoolState);
							}
						}

						if (currentSkillInfo.useSkillLevel) {
							if (currentSkillInfo.skillActive) {

								int currentSkillLevel = currentSkillInfo.currentSkillLevel;

								if (currentSkillInfo.skillLevelInfoList.Count > currentSkillLevel) {
									currentSkillLevelInfo = currentSkillInfo.skillLevelInfoList [currentSkillLevel];

									if (currentSkillLevelInfo.useFloatValue) {
										currentSkillLevelInfo.eventToInitializeSkill.Invoke (currentSkillLevelInfo.currentValue);
									} 

									if (currentSkillLevelInfo.useBoolValue) {
										currentSkillLevelInfo.eventToInitializeBoolSkill.Invoke (currentSkillLevelInfo.currentBoolState);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public void increasePlayerSkill (string skillName, float skillExtraValue)
	{
		if (!playerSkillsActive) {
			return;
		}

		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

				if (currentSkillInfo.Name.Equals (skillName)) {
					currentSkillInfo.currentValue += skillExtraValue;

					currentSkillInfo.eventToIncreaseSkill.Invoke (skillExtraValue);
				}
			}
		}
	}

	public float getSkillValue (string skillName)
	{
		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

				if (currentSkillInfo.Name.Equals (skillName)) {
					return currentSkillInfo.currentValue;
				}
			}
		}

		return -1;
	}

	public void updateSkillValue (string skillName, float newValue)
	{
		if (!playerSkillsActive) {
			return;
		}

		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

				if (currentSkillInfo.Name.Equals (skillName)) {
					currentSkillInfo.currentValue = newValue;
				}
			}
		}
	}

	public void enableOrDisableBoolPlayerSkill (string skillName, bool boolSkillValue)
	{
		if (!playerSkillsActive) {
			return;
		}

		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

				if (currentSkillInfo.Name.Equals (skillName)) {
					currentSkillInfo.currentBoolState = boolSkillValue;

					currentSkillInfo.eventToActivateBoolSkill.Invoke (boolSkillValue);
				}
			}
		}
	}

	public bool getBoolSkillValue (string skillName)
	{
		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

				if (currentSkillInfo.Name.Equals (skillName)) {
					return currentSkillInfo.currentBoolState;
				}
			}
		}

		return false;
	}

	public void updateBoolSkillValue (string skillName, bool boolSkillValue)
	{
		if (!playerSkillsActive) {
			return;
		}

		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

				if (currentSkillInfo.Name.Equals (skillName)) {
					currentSkillInfo.currentBoolState = boolSkillValue;
				}
			}
		}
	}

	public skillInfo getSkillInfoByIndex (int categoryIndex, int skillIndex)
	{
		return skillCategoryInfoList [categoryIndex].skillInfoList [skillIndex];
	}

	public int useSkillPoints (int categoryIndex, int skillIndex, int totalSkillPoints, bool ignoreSkillPoints)
	{
		skillInfo skillInfoToUse = skillCategoryInfoList [categoryIndex].skillInfoList [skillIndex];

		int neededSkillPoints = 0;

		int currentSkillLevel = skillInfoToUse.currentSkillLevel;

		if (skillInfoToUse.useSkillLevel && skillInfoToUse.skillActive) {
			if (skillInfoToUse.skillLevelInfoList.Count > currentSkillLevel) {
				neededSkillPoints = skillInfoToUse.skillLevelInfoList [currentSkillLevel].neededSkillPoints;

				skillInfoToUse.currentSkillLevel++;

				if (skillInfoToUse.currentSkillLevel > skillInfoToUse.skillLevelInfoList.Count - 1) {
					skillInfoToUse.currentSkillLevel = skillInfoToUse.skillLevelInfoList.Count - 1;
				}

				if (currentSkillLevel > skillInfoToUse.skillLevelInfoList.Count - 1) {
					currentSkillLevel = skillInfoToUse.skillLevelInfoList.Count - 1;
				}
			}			
		} else {
			neededSkillPoints = skillInfoToUse.neededSkillPoints;
		}

		if (neededSkillPoints <= totalSkillPoints || ignoreSkillPoints) {

			bool useRegularSkillEvent = false;
			bool useLevelSkillEvent = false;

			if (skillInfoToUse.useSkillLevel) {
				if (skillInfoToUse.skillActive) {
					if (currentSkillLevel == skillInfoToUse.skillLevelInfoList.Count - 1) {
						skillInfoToUse.skillComplete = true;
					}

					useLevelSkillEvent = true;
					useRegularSkillEvent = true;
				} else {
					useRegularSkillEvent = true;
				}
			} else {
				skillInfoToUse.skillComplete = true;

				useRegularSkillEvent = true;
			}

			if (useRegularSkillEvent) {
				if (skillInfoToUse.useFloatValue) {
					skillInfoToUse.currentValue = skillInfoToUse.valueToConfigure;
					skillInfoToUse.eventToIncreaseSkill.Invoke (skillInfoToUse.currentValue);
				}

				if (skillInfoToUse.useBoolValue) {
					skillInfoToUse.currentBoolState = skillInfoToUse.boolStateToConfigure;
					skillInfoToUse.eventToActivateBoolSkill.Invoke (skillInfoToUse.currentBoolState);
				}
			}

			if (useLevelSkillEvent) {

//				print ("level " + currentSkillLevel);

				currentSkillLevelInfo = skillInfoToUse.skillLevelInfoList [currentSkillLevel];

				if (currentSkillLevelInfo.useFloatValue) {
					currentSkillLevelInfo.eventToIncreaseSkill.Invoke (currentSkillLevelInfo.currentValue);
				} 

				if (currentSkillLevelInfo.useBoolValue) {
					currentSkillLevelInfo.eventToActivateBoolSkill.Invoke (currentSkillLevelInfo.currentBoolState);
				}
			}

			skillInfoToUse.skillActive = true;

//			print (skillInfoToUse.Name + " " + skillInfoToUse.skillActive);

			return neededSkillPoints;
		}
			
		return -1;
	}

	public void getSkillByName (string skillName)
	{
		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

				if (currentSkillInfo.Name.Equals (skillName)) {
					useSkillPoints (i, k, 0, true);
				}
			}
		}
	}

	public void unlockSkillSlotByName (string skillName)
	{
		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

				if (currentSkillInfo.Name.Equals (skillName)) {
					currentSkillInfo.skillUnlocked = true;
				}
			}
		}
	}

	public int getCategoryIndex (string categoryName)
	{
		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			if (skillCategoryInfoList [i].Name.Equals (categoryName)) {
				return i;
			}
		}

		return -1;
	}

	public int getSkillIndex (string skillName)
	{
		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

				if (currentSkillInfo.Name.Equals (skillName)) {
					return k;
				}
			}
		}

		return -1;
	}

	public int getSkillIndex (string skillCategoryName, string skillName)
	{
		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			if (currentSkillCategoryInfo.Name.Equals (skillCategoryName)) {

				int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

				for (int k = 0; k < skillInfoListCount; k++) {

					currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

					if (currentSkillInfo.Name.Equals (skillName)) {
						return k;
					}
				}
			}
		}

		return -1;
	}

	public void setPlayerSkillsActiveState (bool state)
	{
		playerSkillsActive = state;
	}

	public void saveSettingsToTemplate ()
	{
		if (mainSkillSettingsTemplate == null) {
			return;
		}

		mainSkillSettingsTemplate.skillTemplateCategoryInfoList.Clear ();

		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillTemplateCategoryInfo newSkillTemplateCategoryInfo = new skillTemplateCategoryInfo ();

			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			newSkillTemplateCategoryInfo.Name = currentSkillCategoryInfo.Name;

			int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

			for (int k = 0; k < skillInfoListCount; k++) {

				currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

				skillTemplateInfo newSkillTemplateInfo = new skillTemplateInfo ();

				newSkillTemplateInfo.Name = currentSkillInfo.Name;

				newSkillTemplateInfo.skillEnabled = currentSkillInfo.skillEnabled;

				newSkillTemplateInfo.skillComplete = currentSkillInfo.skillComplete;

				newSkillTemplateCategoryInfo.skillTemplateInfoList.Add (newSkillTemplateInfo);
			}

			mainSkillSettingsTemplate.skillTemplateCategoryInfoList.Add (newSkillTemplateCategoryInfo);
		}

		updateComponent ();

		GKC_Utils.updateDirtyScene ("Save Skills Settings To Template", gameObject);

		print ("Skills values saved to template");
	}

	public void loadSettingsFromTemplate (bool loadingFromEditor)
	{
		if (mainSkillSettingsTemplate == null) {
			return;
		}

		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			skillCategoryInfo currentSkillCategoryInfo = skillCategoryInfoList [i];

			int categoryIndex = mainSkillSettingsTemplate.skillTemplateCategoryInfoList.FindIndex (a => a.Name == currentSkillCategoryInfo.Name);

			if (categoryIndex > -1) {
				
				skillTemplateCategoryInfo newSkillTemplateCategoryInfo = mainSkillSettingsTemplate.skillTemplateCategoryInfoList [categoryIndex];

				int skillInfoListCount = currentSkillCategoryInfo.skillInfoList.Count;

				for (int k = 0; k < skillInfoListCount; k++) {
					currentSkillInfo = currentSkillCategoryInfo.skillInfoList [k];

					int skillIndex = newSkillTemplateCategoryInfo.skillTemplateInfoList.FindIndex (a => a.Name == currentSkillInfo.Name);

					if (skillIndex > -1) {
						skillTemplateInfo newSkillTemplateInfo = newSkillTemplateCategoryInfo.skillTemplateInfoList [skillIndex];

						currentSkillInfo.skillEnabled =	newSkillTemplateInfo.skillEnabled;

						currentSkillInfo.skillCompleteFromTemplate = newSkillTemplateInfo.skillComplete;

						currentSkillInfo.skillComplete = newSkillTemplateInfo.skillComplete;
					}
				}
			}
		}

		if (loadingFromEditor) {
			updateComponent ();

			GKC_Utils.updateDirtyScene ("Load Skills Settings From Template", gameObject);

			print ("Skills values loaded from template");
		}
	}

	public void setAllSkillsCompleteStateOnTemplate (bool state)
	{
		if (mainSkillSettingsTemplate == null) {
			return;
		}

		for (int i = 0; i < mainSkillSettingsTemplate.skillTemplateCategoryInfoList.Count; i++) {
			for (int k = 0; k < mainSkillSettingsTemplate.skillTemplateCategoryInfoList [i].skillTemplateInfoList.Count; k++) {
				mainSkillSettingsTemplate.skillTemplateCategoryInfoList [i].skillTemplateInfoList [k].skillComplete = state;
			}
		}

		updateComponent ();

		GKC_Utils.updateDirtyScene ("Update Skills Settings To Template", gameObject);

		print ("All skills complete state configured as " + state);
	}

	public void enableAllSkillsOnEditor ()
	{
		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			enableOrDisableSkillsOnEditor (true, i);
		}
	}

	public void disableAllSkillsOnEditor ()
	{
		int skillCategoryInfoListCount = skillCategoryInfoList.Count;

		for (int i = 0; i < skillCategoryInfoListCount; i++) {
			enableOrDisableSkillsOnEditor (false, i);
		}
	}

	public void enableSkillsOnEditor (int categoryIndex)
	{
		enableOrDisableSkillsOnEditor (true, categoryIndex);
	}

	public void disableSkillsOnEditor (int categoryIndex)
	{
		enableOrDisableSkillsOnEditor (false, categoryIndex);
	}

	public void enableOrDisableSkillsOnEditor (bool state, int categoryIndex)
	{
		skillCategoryInfo currentCategory = skillCategoryInfoList [categoryIndex];

		int skillInfoListCount = currentCategory.skillInfoList.Count;

		for (int k = 0; k < skillInfoListCount; k++) {
			currentCategory.skillInfoList [k].skillEnabled = state;
		}
			
		updateComponent ();
	}

	public void activateSkillsOnEditor (int categoryIndex)
	{
		activateOrDeactivateSkillsOnEditor (true, categoryIndex);
	}

	public void deactivateSkillsOnEditor (int categoryIndex)
	{
		activateOrDeactivateSkillsOnEditor (false, categoryIndex);
	}

	public void activateOrDeactivateSkillsOnEditor (bool state, int categoryIndex)
	{
		skillCategoryInfo currentCategory = skillCategoryInfoList [categoryIndex];

		int skillInfoListCount = currentCategory.skillInfoList.Count;

		for (int k = 0; k < skillInfoListCount; k++) {

			currentSkillInfo = currentCategory.skillInfoList [k];

			if (currentSkillInfo.skillEnabled) {

				if (currentSkillInfo.useBoolValue) {
					if (state) {
						currentSkillInfo.currentBoolState = currentSkillInfo.boolStateToConfigure; 
					} else {
						currentSkillInfo.currentBoolState = !currentSkillInfo.boolStateToConfigure;
					}
				} else {
					if (state) {
						currentSkillInfo.currentValue = currentSkillInfo.valueToConfigure; 
					} else {
						currentSkillInfo.currentValue = 0;
					}
				} 
			}
		}

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Skills Values", gameObject);
	}

	[System.Serializable]
	public class skillCategoryInfo
	{
		public string Name;
		public List<skillInfo> skillInfoList = new List<skillInfo> ();
	}

	[System.Serializable]
	public class skillInfo
	{
		public string Name;

		public bool skillEnabled = true;

		[TextArea (1, 10)] public string skillDescription;

		public int neededSkillPoints = 1;

		public bool skillUnlocked;

		public bool skillActive;

		public bool skillComplete;

		public bool useFloatValue;

		public float currentValue;

		public float valueToConfigure;

		public eventParameters.eventToCallWithAmount eventToInitializeSkill;

		public eventParameters.eventToCallWithAmount eventToIncreaseSkill;

		public bool useBoolValue;

		public bool currentBoolState;

		public bool boolStateToConfigure;

		public eventParameters.eventToCallWithBool eventToInitializeBoolSkill;

		public eventParameters.eventToCallWithBool eventToActivateBoolSkill;

		public bool useTwoEventsForActiveAndNotActive;

		public UnityEvent eventToInitializeSkillActive;

		public UnityEvent eventToInitializeSkillNotActive;

		public bool useSkillLevel;

		public int currentSkillLevel;

		public List<skillLevelInfo> skillLevelInfoList = new List<skillLevelInfo> ();

		public bool skillCompleteFromTemplate;
	}

	[System.Serializable]
	public class skillLevelInfo
	{
		[TextArea (1, 10)] public string skillLevelDescription;

		public int neededSkillPoints = 1;

		public bool useFloatValue;
		public float currentValue;

		public eventParameters.eventToCallWithAmount eventToInitializeSkill;

		public eventParameters.eventToCallWithAmount eventToIncreaseSkill;

		public bool useBoolValue;
		public bool currentBoolState;
		public eventParameters.eventToCallWithBool eventToInitializeBoolSkill;

		public eventParameters.eventToCallWithBool eventToActivateBoolSkill;
	}




	[System.Serializable]
	public class skillTemplateCategoryInfo
	{
		public string Name;
		public List<skillTemplateInfo> skillTemplateInfoList = new List<skillTemplateInfo> ();
	}

	[System.Serializable]
	public class skillTemplateInfo
	{
		public string Name;

		public bool skillEnabled = true;

		public bool skillComplete;
	}
}
