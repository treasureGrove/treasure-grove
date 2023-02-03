using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using GKC.Localization;

public class playerSkillsUISystem : MonoBehaviour
{
	public List<skillUICategoryInfo> skillUICategoryInfoList = new List<skillUICategoryInfo> ();

	public bool skillsMenuOpened;

	public bool showLockedSkillName = true;
	public string lockedSkillNameToShow = "???";
	public bool showLockedSkillDescription = true;
	public string lockedSkillDescriptionToShow = "???";

	public bool useDoublePressToShowIncreaseSkillButtonEnabled = true;

	public GameObject skillsMenuPanel;

	public Text currentSkillCategoryNameText;
	public Text currentSkillNameText;
	public Text currentSkillDescriptionText;
	public Text currentSkillLevelDescriptionText;
	public Text currentSkillPointsText;

	public Text requiredSkillPointsText;

	public RectTransform confirmUseSkillPointsPanel;

	public Scrollbar categoryScrollBar;
	public RectTransform categoryListContent;
	public ScrollRect categoryScrollRect;

	public UnityEvent eventOnSkillMenuOpened;
	public UnityEvent eventOnSkillMenuClosed;

	public UnityEvent eventOnSkillPointsUsed;
	public UnityEvent eventOnNotEnoughSkillPoints;

	public playerExperienceSystem playerExperienceManager;
	public playerSkillsSystem playerSkillsManager;

	public skillUICategoryInfo currentSkillUICategoryInfo;

	public skillUIInfo currentSkillUIInfo;
	public skillUIInfo previousSkillUIInfo;

	public skillSlotPanelInfo currentSkillSlotPanelInfo;

	public string skillSlotsName = "Skill Slot ";

	public bool showDebugPrint;

	playerSkillsSystem.skillInfo currentSkillInfo;


	public void updateSkillSlots ()
	{
		for (int i = 0; i < skillUICategoryInfoList.Count; i++) {

			currentSkillUICategoryInfo = skillUICategoryInfoList [i];

			for (int k = 0; k < currentSkillUICategoryInfo.skillUIInfoList.Count; k++) {

				currentSkillUIInfo = currentSkillUICategoryInfo.skillUIInfoList [k];

				currentSkillInfo = playerSkillsManager.getSkillInfoByIndex (currentSkillUIInfo.categorySkillIndex, currentSkillUIInfo.skillIndex);

				if (currentSkillInfo.skillUnlocked) {
					if (currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotLocked.activeSelf) {
						currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotLocked.SetActive (false);
					}

					if (currentSkillInfo.skillActive) {
						if (!currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotActive.activeSelf) {
							currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotActive.SetActive (true);
						}
					} else {
						if (!currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotUnlocked.activeSelf) {
							currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotUnlocked.SetActive (true);
						}
					}

					if (currentSkillInfo.useSkillLevel) {

						if (!currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotSkillAmountText.gameObject.activeSelf) {
							currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotSkillAmountText.gameObject.SetActive (true);
						}

						if (currentSkillInfo.skillComplete) {
							currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotSkillAmountText.text = (currentSkillInfo.currentSkillLevel + 1) + "/" +
							currentSkillInfo.skillLevelInfoList.Count;
						} else {
							currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotSkillAmountText.text = currentSkillInfo.currentSkillLevel + "/" +
							currentSkillInfo.skillLevelInfoList.Count;
						}
					}
				}
			}
		}
	}

	public void openOrCloseSkillsMenu (bool state)
	{
		skillsMenuOpened = state;

		if (skillsMenuPanel.activeSelf != skillsMenuOpened) {
			skillsMenuPanel.SetActive (skillsMenuOpened);
		}

		previousSkillUIInfo = null;

		if (skillsMenuOpened) {

			updateSkillSlots ();

			checkSkillCategoryPressed (skillUICategoryInfoList [0].categorySlot);

			eventOnSkillMenuOpened.Invoke ();

			resetScroll (categoryScrollBar);

			resetScrollRectTransfrom (categoryListContent, categoryScrollRect);
		} else {
			if (confirmUseSkillPointsPanel.gameObject.activeSelf) {
				confirmUseSkillPointsPanel.gameObject.SetActive (false);
			}

			eventOnSkillMenuClosed.Invoke ();
		}
	}

	public void checkSkillPressed (GameObject skillSlot)
	{
		for (int i = 0; i < skillUICategoryInfoList.Count; i++) {

			currentSkillUICategoryInfo = skillUICategoryInfoList [i];

			for (int k = 0; k < currentSkillUICategoryInfo.skillUIInfoList.Count; k++) {

				currentSkillUIInfo = currentSkillUICategoryInfo.skillUIInfoList [k];

				if (currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.skillSlot == skillSlot) {

					if (confirmUseSkillPointsPanel.gameObject.activeSelf) {
						confirmUseSkillPointsPanel.gameObject.SetActive (false);
					}

					if (previousSkillUIInfo != null) {
						if (previousSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotPressedIcon.activeSelf) {
							previousSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotPressedIcon.SetActive (false);
						}
					}

					updateCurrentSkillInfo ();

					bool activateUseOfSkillPoints = false;

					if (currentSkillUIInfo != previousSkillUIInfo) {
						previousSkillUIInfo = currentSkillUIInfo;
					} else {
						activateUseOfSkillPoints = true;

						if (showDebugPrint) {
							print ("different");
						}
					}

					if (showDebugPrint) {
						print (currentSkillUIInfo.Name);
					}

					if (!currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotPressedIcon.activeSelf) {
						currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotPressedIcon.SetActive (true);
					}

					currentSkillInfo = playerSkillsManager.getSkillInfoByIndex (currentSkillUIInfo.categorySkillIndex, currentSkillUIInfo.skillIndex);

					if (currentSkillInfo.skillComplete || !currentSkillInfo.skillUnlocked) {
						if (showDebugPrint) {
							print ("return");
						}

						return;
					}

					if (!useDoublePressToShowIncreaseSkillButtonEnabled) {
						activateUseOfSkillPoints = true;
					}

					if (activateUseOfSkillPoints) {
						if (!confirmUseSkillPointsPanel.gameObject.activeSelf) {
							confirmUseSkillPointsPanel.gameObject.SetActive (true);
						}

						confirmUseSkillPointsPanel.position = currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.confirmUseSkillPointsPanelPosition.position;
					}

					return;
				}
			}
		}
	}

	public void updateCurrentSkillInfo ()
	{
		bool isCheckLanguageActive = gameLanguageSelector.isCheckLanguageActive ();

		string currentSkillUICategoryInfoName = currentSkillUICategoryInfo.Name;

		string currentSkillUIInfoName = currentSkillUIInfo.Name;

		string lockedSkillNameToShowValue = lockedSkillNameToShow;

		string lockedSkillDescriptionToShowValue = lockedSkillDescriptionToShow;


		if (isCheckLanguageActive) {
			currentSkillUICategoryInfoName = skillsLocalizationManager.GetLocalizedValue (currentSkillUICategoryInfoName);
			currentSkillUIInfoName = skillsLocalizationManager.GetLocalizedValue (currentSkillUIInfoName);
			lockedSkillNameToShowValue = skillsLocalizationManager.GetLocalizedValue (lockedSkillNameToShowValue);
			lockedSkillDescriptionToShowValue = skillsLocalizationManager.GetLocalizedValue (lockedSkillDescriptionToShowValue);
		}


		currentSkillCategoryNameText.text = currentSkillUICategoryInfoName;

		currentSkillInfo = playerSkillsManager.getSkillInfoByIndex (currentSkillUIInfo.categorySkillIndex, currentSkillUIInfo.skillIndex);

		if (showLockedSkillName) {
			currentSkillNameText.text = currentSkillUIInfoName;
		} else {
			if (currentSkillInfo.skillUnlocked) {
				currentSkillNameText.text = currentSkillUIInfoName;
			} else {
				currentSkillNameText.text = lockedSkillNameToShowValue;
			}
		}

		bool showSkilLevelDescription = true;

		string currentSkillInfoSkillDescription = currentSkillInfo.skillDescription;

		if (isCheckLanguageActive) {
			currentSkillInfoSkillDescription = skillsLocalizationManager.GetLocalizedValue (currentSkillInfoSkillDescription);
		}

		if (showLockedSkillDescription) {
			currentSkillDescriptionText.text = currentSkillInfoSkillDescription;
		} else {
			if (currentSkillInfo.skillUnlocked) {
				currentSkillDescriptionText.text = currentSkillInfoSkillDescription;
			} else {
				currentSkillDescriptionText.text = lockedSkillDescriptionToShowValue;

				showSkilLevelDescription = false;
			}
		}

		if (showSkilLevelDescription) {
			if (currentSkillInfo.useSkillLevel) {
				if (currentSkillInfo.skillLevelInfoList.Count > currentSkillInfo.currentSkillLevel) {
					string skillLevelDescription = currentSkillInfo.skillLevelInfoList [currentSkillInfo.currentSkillLevel].skillLevelDescription;

					if (isCheckLanguageActive) {
						skillLevelDescription = skillsLocalizationManager.GetLocalizedValue (skillLevelDescription);
					}

					currentSkillLevelDescriptionText.text = skillLevelDescription;
				}
			} else {
				currentSkillLevelDescriptionText.text = "";
			}
		} else {
			if (currentSkillInfo.useSkillLevel && currentSkillInfo.skillLevelInfoList.Count > currentSkillInfo.currentSkillLevel &&
			    currentSkillInfo.skillLevelInfoList [currentSkillInfo.currentSkillLevel].skillLevelDescription != "") {
				currentSkillLevelDescriptionText.text = lockedSkillDescriptionToShowValue;
			} else {
				currentSkillLevelDescriptionText.text = "";
			}
		}

		currentSkillPointsText.text = playerExperienceManager.getSkillPointsAmount ().ToString ();

		if (currentSkillInfo.skillComplete) {
			requiredSkillPointsText.text = "";
		} else {
			if (currentSkillInfo.useSkillLevel) {
				if (currentSkillInfo.skillLevelInfoList.Count > currentSkillInfo.currentSkillLevel) {
					requiredSkillPointsText.text = currentSkillInfo.skillLevelInfoList [currentSkillInfo.currentSkillLevel].neededSkillPoints.ToString ();
				}
			} else {
				requiredSkillPointsText.text = currentSkillInfo.neededSkillPoints.ToString ();
			}
		}
	}

	public void confirmUseSkillPointsOnCurrentSkillSlot ()
	{
		if (currentSkillUIInfo != null) {

			int skillPointsUsed = playerSkillsManager.useSkillPoints (currentSkillUIInfo.categorySkillIndex, currentSkillUIInfo.skillIndex, playerExperienceManager.getSkillPointsAmount (), false);

			currentSkillInfo = playerSkillsManager.getSkillInfoByIndex (currentSkillUIInfo.categorySkillIndex, currentSkillUIInfo.skillIndex);

			if (showDebugPrint) {
				print (skillPointsUsed);
			}

			if (skillPointsUsed > 0) {
				playerExperienceManager.useSkillPoints (skillPointsUsed);

				currentSkillSlotPanelInfo = currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo;

				if (!currentSkillSlotPanelInfo.slotActive.activeSelf) {
					currentSkillSlotPanelInfo.slotActive.SetActive (true);
				}

				if (currentSkillSlotPanelInfo.slotUnlocked.activeSelf) {
					currentSkillSlotPanelInfo.slotUnlocked.SetActive (false);
				}

				if (currentSkillInfo.useSkillLevel && currentSkillInfo.skillActive) {
					if (!currentSkillSlotPanelInfo.slotSkillAmountText.gameObject.activeSelf) {
						currentSkillSlotPanelInfo.slotSkillAmountText.gameObject.SetActive (true);
					}

					if (currentSkillInfo.skillComplete) {
						currentSkillSlotPanelInfo.slotSkillAmountText.text = (currentSkillInfo.currentSkillLevel + 1) + "/" + currentSkillInfo.skillLevelInfoList.Count;
					} else {
						currentSkillSlotPanelInfo.slotSkillAmountText.text = currentSkillInfo.currentSkillLevel + "/" +	currentSkillInfo.skillLevelInfoList.Count;
					}
				}

				if (showDebugPrint) {
					print ("try unlock " + currentSkillUIInfo.unlockOtherSkillSlots);
				}

				if (currentSkillUIInfo.unlockOtherSkillSlots && (!currentSkillUIInfo.unlockWhenCurrentSlotIsComplete || currentSkillInfo.skillComplete)) {

					if (!currentSkillInfo.useSkillLevel || !currentSkillUIInfo.useMinSkillLevelToUnlock ||
					    (currentSkillUIInfo.useMinSkillLevelToUnlock && currentSkillInfo.currentSkillLevel >= currentSkillUIInfo.minSkillLevelToUnlock)) {		

						for (int i = 0; i < currentSkillUICategoryInfo.skillUIInfoList.Count; i++) {
							if (currentSkillUIInfo.skillNameListToUnlock.Contains (currentSkillUICategoryInfo.skillUIInfoList [i].Name)) {
								unlockSkillSlot (i);
							}
						}
					}
				}

				updateCurrentSkillInfo ();

				eventOnSkillPointsUsed.Invoke ();
			} else {
				eventOnNotEnoughSkillPoints.Invoke ();
			}

			if (confirmUseSkillPointsPanel.gameObject.activeSelf) {
				confirmUseSkillPointsPanel.gameObject.SetActive (false);
			}
		}
	}

	public void cancelUseSkillPointsOnCurrentSkillSlot ()
	{
		if (confirmUseSkillPointsPanel.gameObject.activeSelf) {
			confirmUseSkillPointsPanel.gameObject.SetActive (false);
		}
	}

	public void unlockSkillSlot (int skillSlotIndex)
	{
		if (showDebugPrint) {
			print ("unlock " + currentSkillUICategoryInfo.skillUIInfoList [skillSlotIndex].Name);
		}

		currentSkillInfo = playerSkillsManager.getSkillInfoByIndex (currentSkillUIInfo.categorySkillIndex, currentSkillUIInfo.skillIndex);

		currentSkillSlotPanelInfo = currentSkillUICategoryInfo.skillUIInfoList [skillSlotIndex].mainSkillSlotPanel.mainSkillSlotPanelInfo;

		if (currentSkillSlotPanelInfo.slotLocked.activeSelf) {
			currentSkillSlotPanelInfo.slotLocked.SetActive (false);
		}

		if (!currentSkillSlotPanelInfo.slotUnlocked.activeSelf) {
			currentSkillSlotPanelInfo.slotUnlocked.SetActive (true);
		}

		playerSkillsManager.unlockSkillSlotByName (currentSkillUICategoryInfo.skillUIInfoList [skillSlotIndex].Name);
	}

	public void checkSkillCategoryPressed (GameObject categorySlot)
	{
		previousSkillUIInfo = null;

		for (int i = 0; i < skillUICategoryInfoList.Count; i++) {
			if (skillUICategoryInfoList [i].categorySlot == categorySlot) {
				if (!skillUICategoryInfoList [i].categorySkillPanel.activeSelf) {
					skillUICategoryInfoList [i].categorySkillPanel.SetActive (true);
				}

				if (currentSkillUIInfo != null) {
					if (currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotPressedIcon.activeSelf) {
						currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.slotPressedIcon.SetActive (false);
					}
				}

				checkSkillPressed (skillUICategoryInfoList [i].skillUIInfoList [0].mainSkillSlotPanel.mainSkillSlotPanelInfo.skillSlot);
			} else {
				if (skillUICategoryInfoList [i].categorySkillPanel.activeSelf) {
					skillUICategoryInfoList [i].categorySkillPanel.SetActive (false);
				}
			}
		}

		if (confirmUseSkillPointsPanel.gameObject.activeSelf) {
			confirmUseSkillPointsPanel.gameObject.SetActive (false);
		}
	}

	public void resetScroll (Scrollbar scrollBarToReset)
	{
		StartCoroutine (resetScrollCoroutine (scrollBarToReset));
	}

	IEnumerator resetScrollCoroutine (Scrollbar scrollBarToReset)
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		scrollBarToReset.value = 1;
	}

	public void resetScrollRectTransfrom (RectTransform listContent, ScrollRect scrollRectToReset)
	{
		StartCoroutine (resetScrollRectTransfromCoroutine (listContent, scrollRectToReset));
	}

	IEnumerator resetScrollRectTransfromCoroutine (RectTransform listContent, ScrollRect scrollRectToReset)
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate (listContent);

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		scrollRectToReset.verticalNormalizedPosition = 1;
	}

	public void assignSkillsToSlots ()
	{
		for (int i = 0; i < skillUICategoryInfoList.Count; i++) {
			for (int k = 0; k < skillUICategoryInfoList [i].skillUIInfoList.Count; k++) {

				currentSkillUIInfo = skillUICategoryInfoList [i].skillUIInfoList [k];

				currentSkillUIInfo.categorySkillIndex = playerSkillsManager.getCategoryIndex (skillUICategoryInfoList [i].Name);
				currentSkillUIInfo.skillIndex = playerSkillsManager.getSkillIndex (skillUICategoryInfoList [i].Name, currentSkillUIInfo.Name);

				if (showDebugPrint) {
					if (currentSkillUIInfo.categorySkillIndex != -1 && currentSkillUIInfo.skillIndex != -1) {
						print ("Assigned skill " + currentSkillUIInfo.Name + " " + currentSkillUIInfo.categorySkillIndex + " " + currentSkillUIInfo.skillIndex);
					} else {
						print ("WARNING: the skill slot with the name " + currentSkillUIInfo.Name + " hasn't found the skill configured. Make sure it exists");
					}
				}
			}
		}

		if (showDebugPrint) {
			print ("All skills assigned to every slot");
		}

		updateComponent ();
	}

	public void assignSkillsNamesToSlots (int listIndex)
	{
		for (int k = 0; k < skillUICategoryInfoList [listIndex].skillUIInfoList.Count; k++) {

			currentSkillUIInfo = skillUICategoryInfoList [listIndex].skillUIInfoList [k];

			if (currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.skillSlot) {
				currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.skillSlot.name = skillSlotsName + currentSkillUIInfo.Name;
			}
		}

		if (showDebugPrint) {
			print ("All skills names assigned to every slot");
		}

		updateComponent ();
	}

	public void activateAllSkills (int listIndex)
	{
		checkSkillCategoryPressed (skillUICategoryInfoList [listIndex].categorySlot);

		for (int k = 0; k < skillUICategoryInfoList [listIndex].skillUIInfoList.Count; k++) {

			currentSkillUIInfo = skillUICategoryInfoList [listIndex].skillUIInfoList [k];

			checkSkillPressed (currentSkillUIInfo.mainSkillSlotPanel.mainSkillSlotPanelInfo.skillSlot);

			confirmUseSkillPointsOnCurrentSkillSlot ();
		}

		if (showDebugPrint) {
			print ("All skills on category " + skillUICategoryInfoList [listIndex].Name + " activated");
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class skillUICategoryInfo
	{
		public string Name;

		public GameObject categorySlot;

		public GameObject categorySkillPanel;

		public List<skillUIInfo> skillUIInfoList = new List<skillUIInfo> ();
	}

	[System.Serializable]
	public class skillUIInfo
	{
		public string Name;

		public skillSlotPanel mainSkillSlotPanel;

		public int categorySkillIndex;
		public int skillIndex;

		public bool unlockOtherSkillSlots;

		public bool unlockWhenCurrentSlotIsComplete;

		public bool useMinSkillLevelToUnlock;
		public int minSkillLevelToUnlock;

		public List<string> skillNameListToUnlock = new List<string> ();
	}
}
