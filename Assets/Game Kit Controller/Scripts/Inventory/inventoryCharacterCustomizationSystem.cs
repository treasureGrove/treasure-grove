using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class inventoryCharacterCustomizationSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool characterCustomizationEnabled = true;

	public Vector3 cameraPositionOffset;
	public Vector3 cameraEulerOffset;

	public string characterCustomizationMenuName = "Character Aspect Customization Menu";

	public string regularInventoryPanelName = "Default";

	public string customizationInventoryPanelName = "Armor-Cloth Customization";

	public List<string> currentCategoryListToUseActive = new List<string> ();

	[Space]
	[Header ("Rotate Character Settings")]
	[Space]

	public bool rotateCharacterEnabled = true;

	public float characterRotationSpeed = 10;

	[Space]
	[Header ("Character Animation Settings")]
	[Space]

	public bool setUnscaledTimeOnAnimator;

	public bool setNewAnimatorIdle;

	public int animatorIdle;

	[Space]
	[Header ("Initial Armor/Cloth Pieces Settings")]
	[Space]

	public bool setInitialArmorClothPieceList;

	public string initialArmorClothPieceListName;

	public List<initialArmorClothInfo> initialArmorClothInfoList = new List<initialArmorClothInfo> ();

	[Space]
	[Space]

	public bool useCharacterAspectCustomizationTemplate;

	public characterAspectCustomizationTemplate initialCharacterAspectCustomizationTemplate;

	[Space]

	public bool useRandomCharacterAspectCustomizationTemplate;

	public List<characterAspectCustomizationTemplate> characterAspectCustomizationTemplateList = new List<characterAspectCustomizationTemplate> ();

	[Space]
	[Header ("Drop Pieces Settings")]
	[Space]

	public bool useInventoryManager = true;

	public bool dropArmorClothPiecesExternallyEnabled = true;

	public bool instantiatePiecesDroppedExternallyEnabled = true;

	public bool addForceToDroppedObject;

	public ForceMode forceModeForDroppedObject;

	public float forceAmountToDroppedObject;

	[Space]
	[Header ("Character Values Settings")]
	[Space]

	public bool upgradeAbilitiesEnabled = true;
	public bool upgradeStatsEnabled = true;
	public bool upgradeSkillsEnabled = true;
	public bool upgradeDamageResistanceValuesEnabled = true;

	public bool storeDurabilityValuesOfPiecesEnabled = true;

	public bool useEventOnDurabilityAffectedEnabled;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool menuOpened;

	public bool fullArmorClothActive;
	public string currentFullArmorClothActiveName;

	public bool mainCharacterAspectCustomizationUISystemAssigned;

	public bool mainCharacterCustomizationManagerAssigned;

	public bool rotationInputActive;

	public bool keepAllCharacterMeshesDisabledActive;

	[Space]
	[Header ("Pieces and Stats Debug")]
	[Space]

	public List<string> currentPiecesList = new List<string> ();

	[Space]
	[Space]

	public List<float> currentPiecesDurabilityList = new List<float> ();

	Dictionary<string, string> currentPiecesDictionaryList = new Dictionary<string, string> ();

	[Space]
	[Space]

	public List<temporalStatValue> temporalStatValueList = new List<temporalStatValue> ();

	[Space]
	[Header ("Armor Cloth Data Settings")]
	[Space]

	public armorClothPieceTemplateData mainArmorClothPieceTemplateData;

	[Space]
	[Space]

	public fullArmorClothTemplateData mainFullArmorClothTemplateData;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnOpenMenu;
	public UnityEvent eventOnCloseMenu;

	[Space]
	[Header ("Components")]
	[Space]

	public inventoryManager mainInventoryManager;

	public inventoryQuickAccessSlotsSystem mainInventoryQuickAccessSlotsSystem;

	public menuPause pauseManager;

	public Transform mainCameraTransform;

	public Transform mainCameraParent;

	public Transform playerTransform;

	public headTrack mainHeadTrack;

	public characterAspectCustomizationUISystem mainCharacterAspectCustomizationUISystem;

	public characterCustomizationManager mainCharacterCustomizationManager;

	public remoteEventSystem mainRemoteEventSystem;

	public playerStatsSystem mainPlayerStatsSystem;

	public playerSkillsSystem mainPlayerSkillsSystem;

	public playerAbilitiesSystem mainPlayerAbilitiesSystem;

	public health mainHealth;

	Coroutine menuCoroutine;

	Transform originalCameraParent;

	Vector2 axisValues;

	int previousIdleID = -1;

	fullArmorClothTemplate currentFullArmorClothTemplate;

	bool previousKeepAllCharacterMeshesDisabledActiveValue;

	void Start ()
	{
		if (!characterCustomizationEnabled) {
			return;
		}

		if (!mainCharacterAspectCustomizationUISystemAssigned) {
			checkAssignCharacterAspectCustomizationUISystem ();
		}

		if (!mainCharacterCustomizationManagerAssigned) {
			mainCharacterCustomizationManagerAssigned = mainCharacterCustomizationManager != null;
		}

		if (setInitialArmorClothPieceList) {
			StartCoroutine (setInitialArmorClothListCoroutine ());
		}

		if (keepAllCharacterMeshesDisabledActive && mainInventoryManager.isFirstPersonActive ()) {
			StartCoroutine (setInitialVisibleMeshesOnCharacter (true));
		}
	}

	IEnumerator setInitialArmorClothListCoroutine ()
	{
		yield return new WaitForSeconds (0.01f);

		setInitialArmorClothList ();
	}

	IEnumerator setInitialVisibleMeshesOnCharacter (bool state)
	{
		yield return new WaitForSeconds (0.01f);

		checkCameraViewToFirstOrThirdPerson (state);
	}

	public void stopMenuCoroutineUpdate ()
	{
		if (menuCoroutine != null) {
			StopCoroutine (menuCoroutine);
		}
	}

	public void inputSetRotationInputActive (bool state)
	{
		if (menuOpened) {
			rotationInputActive = state;
		}
	}

	IEnumerator menuCoroutineUpdate ()
	{
		var waitTime = new WaitForSecondsRealtime (0.0001f);

		while (true) {
			yield return waitTime;

			if (rotateCharacterEnabled) {
				if (rotationInputActive) {
					axisValues = mainInventoryManager.playerInput.getPlayerMouseAxis ();

					playerTransform.Rotate (playerTransform.up, -Mathf.Deg2Rad * characterRotationSpeed * axisValues.x, Space.World);
				}
			}

			if (!mainInventoryManager.isInventoryMenuOpened ()) {
				openOrCloseCustomizationMenu (false);
			} else {

			}
		}
	}

	void checkAssignCharacterAspectCustomizationUISystem ()
	{
		if (!mainCharacterAspectCustomizationUISystemAssigned) {
			if (useInventoryManager) {
				ingameMenuPanel currentIngameMenuPanel = pauseManager.getIngameMenuPanelByName (characterCustomizationMenuName);

				if (currentIngameMenuPanel == null) {
					pauseManager.checkcreateIngameMenuPanel (characterCustomizationMenuName);

					currentIngameMenuPanel = pauseManager.getIngameMenuPanelByName (characterCustomizationMenuName);
				}

				if (currentIngameMenuPanel != null) {
					mainCharacterAspectCustomizationUISystem = currentIngameMenuPanel.GetComponent<characterAspectCustomizationUISystem> ();

					mainCharacterAspectCustomizationUISystemAssigned = true;
				}

				if (mainCharacterCustomizationManager == null) {
					mainCharacterCustomizationManager = playerTransform.GetComponentInChildren<characterCustomizationManager> ();
				}

				mainCharacterCustomizationManagerAssigned = mainCharacterCustomizationManager != null;
			} else {
				if (mainCharacterCustomizationManager == null) {
					mainCharacterCustomizationManager = playerTransform.GetComponentInChildren<characterCustomizationManager> ();
				}

				if (mainCharacterCustomizationManager != null) {
					mainCharacterAspectCustomizationUISystemAssigned = true;

					mainCharacterCustomizationManagerAssigned = true;
				}
			}
		}
	}

	public void openOrCloseCustomizationMenu (bool state)
	{
		if (menuOpened == state) {
			return;
		}

		menuOpened = state;

		stopMenuCoroutineUpdate ();

		if (menuOpened) {
			if (originalCameraParent == null) {
				originalCameraParent = mainCameraTransform.parent;
			}

			if (!mainCharacterAspectCustomizationUISystemAssigned) {
				checkAssignCharacterAspectCustomizationUISystem ();
			}

			mainCameraTransform.SetParent (playerTransform);

			mainCameraTransform.localEulerAngles = cameraEulerOffset;
			mainCameraTransform.localPosition = cameraPositionOffset;

			mainCameraTransform.SetParent (mainCameraParent);

			menuCoroutine = StartCoroutine (menuCoroutineUpdate ());

		} else {
			if (originalCameraParent != null) {
				if (mainCameraTransform.parent != originalCameraParent) {
					mainCameraTransform.SetParent (originalCameraParent);
				}

				originalCameraParent = null;
			}

			mainCameraTransform.localPosition = Vector3.zero;
			mainCameraTransform.localEulerAngles = Vector3.zero;
		}

		checkEventOnStateChange (menuOpened);

		mainHeadTrack.setExternalHeadTrackPauseActiveState (menuOpened);

		mainInventoryQuickAccessSlotsSystem.setCustomizingCharacterActiveState (menuOpened);

		mainInventoryQuickAccessSlotsSystem.setCurrentCategoryListToUseActive (currentCategoryListToUseActive);

		mainInventoryQuickAccessSlotsSystem.setCheckObjectCategoriesToUseActiveState (menuOpened);

		rotationInputActive = false;

		axisValues = Vector3.zero;

		if (setUnscaledTimeOnAnimator) {
			pauseManager.setAnimatorUnscaledTimeState (state);
		}

		if (state) {
			if (setNewAnimatorIdle) {
				if (previousIdleID == -1) {
					previousIdleID = pauseManager.getCurrentIdleID ();
				}

				pauseManager.setCurrentIdleIDValue (animatorIdle);

				pauseManager.updateIdleIDOnAnimator ();
			}
		} else {
			if (previousIdleID != -1) {
				pauseManager.setCurrentIdleIDValue (previousIdleID);

				pauseManager.updateIdleIDOnAnimator ();

				previousIdleID = -1;
			}
		}

		if (mainInventoryManager.isFirstPersonActive ()) {
			if (state) {
				previousKeepAllCharacterMeshesDisabledActiveValue = mainCharacterAspectCustomizationUISystem.keepAllCharacterMeshesDisabledActive;

				if (previousKeepAllCharacterMeshesDisabledActiveValue) {
					checkCameraViewToFirstOrThirdPerson (false);

					mainInventoryManager.setPausePlayerRotateToCameraDirectionOnFirstPersonActiveState (true);
				}
			} else {
				if (previousKeepAllCharacterMeshesDisabledActiveValue) {
					checkCameraViewToFirstOrThirdPerson (true);

					mainInventoryManager.setPausePlayerRotateToCameraDirectionOnFirstPersonActiveState (false);

					previousKeepAllCharacterMeshesDisabledActiveValue = false;
				}
			}

			mainInventoryManager.setAnimatorState (menuOpened);

			playerTransform.localRotation = Quaternion.identity;
		} else {
			previousKeepAllCharacterMeshesDisabledActiveValue = false;
		}
	}

	public bool equipObject (string objectName, string categoryName)
	{
		return equipOrUnequipObject (true, objectName, categoryName);
	}

	public bool unequipObject (string objectName, string categoryName)
	{
		return equipOrUnequipObject (false, objectName, categoryName);
	}

	bool equipOrUnequipObject (bool state, string objectName, string categoryName)
	{
		if (showDebugPrint) {
			print ("equip or unequip object " + state + " " + objectName + " " + categoryName);
		}

		if (!mainCharacterAspectCustomizationUISystemAssigned) {
			checkAssignCharacterAspectCustomizationUISystem ();
		}

		if (showDebugPrint) {
			if (useInventoryManager) {
				print ("initializingInventory ------------------------------------------ " + mainInventoryManager.initializingInventory);
			}
		}

		bool checkIfObjectAlreadyOnCurrentPiecesListResult = false;

		if (useInventoryManager) {
			checkIfObjectAlreadyOnCurrentPiecesListResult = mainCharacterAspectCustomizationUISystem.checkIfObjectAlreadyOnCurrentPiecesList (objectName);
		} else {
			checkIfObjectAlreadyOnCurrentPiecesListResult = mainCharacterCustomizationManager.checkIfObjectAlreadyOnCurrentPiecesList (objectName);
		}

		bool objectAdjustedProperly = false;

		if (useInventoryManager) {
			objectAdjustedProperly = mainCharacterAspectCustomizationUISystem.equipOrUnequipObject (state, objectName, categoryName);
		} else {
			if (mainCharacterCustomizationManagerAssigned) {
				objectAdjustedProperly = mainCharacterCustomizationManager.setObjectState (state, objectName, true, categoryName, true);

				if (objectAdjustedProperly) {
					mainCharacterCustomizationManager.checkEquippedStateOnObject (state, objectName, categoryName);
				}
			}
		}

		bool checkArmorClothPieceResult = true;

		if (state && checkIfObjectAlreadyOnCurrentPiecesListResult) {
			checkArmorClothPieceResult = false;

			if (showDebugPrint) {
				print ("piece was already equipped, cancel to check stats values");
			}
		}
	
		if (objectAdjustedProperly) {
			checkArmorClothPieceStatsValues (state, objectName, checkArmorClothPieceResult, categoryName);
		} 

		return objectAdjustedProperly;
	}

	void checkArmorClothPieceStatsValues (bool state, string objectName, bool checkArmorClothPieceResult, string categoryName)
	{
		if (state) {
			if (!currentPiecesList.Contains (objectName)) {
				currentPiecesList.Add (objectName);

				currentPiecesDictionaryList.Add (categoryName, objectName);

				if (storeDurabilityValuesOfPiecesEnabled) {
					if (useInventoryManager) {
						float newDurabilityVale = mainInventoryManager.getDurabilityValueOnObjectByName (objectName);

						currentPiecesDurabilityList.Add (newDurabilityVale);
					}
				}
			}

			if (checkArmorClothPieceResult) {
				checkArmorClothPieceToUse (objectName, state);
			}

		} else {
			if (currentPiecesList.Contains (objectName)) {
				if (storeDurabilityValuesOfPiecesEnabled) {
					int durabilityIndex = currentPiecesList.IndexOf (objectName);

					if(durabilityIndex >=0 && currentPiecesDurabilityList.Count > durabilityIndex){
						currentPiecesDurabilityList.RemoveAt (durabilityIndex);
					}
				}

				currentPiecesList.Remove (objectName);

				currentPiecesDictionaryList.Remove (categoryName);
			}

			if (checkArmorClothPieceResult) {
				checkArmorClothPieceToUse (objectName, state);
			}
		}

		if (checkArmorClothPieceResult) {
			checkFullArmorClothState (objectName, state);
		}
	}


	void checkArmorClothPieceToUse (string objectName, bool state)
	{
		int currentIndex = -1;

		currentIndex = mainArmorClothPieceTemplateData.armorClothPieceTemplateList.FindIndex (s => s.Name == objectName);

		if (currentIndex > -1) {
			armorClothPieceTemplate currrentArmorClothPieceTemplate = mainArmorClothPieceTemplateData.armorClothPieceTemplateList [currentIndex];

			if (showDebugPrint) {
				print ("setting armor cloth stat " + currrentArmorClothPieceTemplate.Name);
			}

			configureStats (state, currrentArmorClothPieceTemplate.armorClothStatsInfoList, objectName, false);
		}
	}

	void checkFullArmorClothState (string objectName, bool state)
	{
		List<fullArmorClothTemplate> currentFullArmorClothTemplateList = mainFullArmorClothTemplateData.fullArmorClothTemplateList;

		if (state) {
			bool checkIfFullArmorComplete = false;

			int fullArmorClothTemplateIndex = -1;

			for (int i = 0; i < currentFullArmorClothTemplateList.Count; i++) {
				if (!checkIfFullArmorComplete) {
					currentFullArmorClothTemplate = currentFullArmorClothTemplateList [i];

					int currentArmorClothPieceAmountIndex = 0;

					int fullArmorPiecesListCount = currentFullArmorClothTemplate.fullArmorPiecesList.Count;

					if (currentPiecesList.Count >= fullArmorPiecesListCount) {

						if (showDebugPrint) {
							print ("checking if armor is full");
						}

						for (int k = 0; k < currentPiecesList.Count; k++) {

							if (currentFullArmorClothTemplate.fullArmorPiecesList.Contains (currentPiecesList [k])) {
								currentArmorClothPieceAmountIndex++;
							} else {
								if (showDebugPrint) {
									print ("armor piece " + currentPiecesList [k] + " not found on full armor " + currentFullArmorClothTemplate.Name);
								}
							}
						}

						if (currentArmorClothPieceAmountIndex >= fullArmorPiecesListCount) {
							checkIfFullArmorComplete = true;

							fullArmorClothTemplateIndex = i;
						}
					}
				}
			}

			if (checkIfFullArmorComplete) {
				if (showDebugPrint) {
					print ("full armor/cloth configured");
				}

				currentFullArmorClothTemplate = currentFullArmorClothTemplateList [fullArmorClothTemplateIndex];

				currentFullArmorClothActiveName = currentFullArmorClothTemplate.Name;

				fullArmorClothActive = true;

				configureStats (state, currentFullArmorClothTemplate.armorClothStatsInfoList, objectName, true);
			} else {
				fullArmorClothActive = false;
			}
		} else {
			if (fullArmorClothActive) {
				int currentIndex = currentFullArmorClothTemplateList.FindIndex (s => s.Name == currentFullArmorClothActiveName);

				if (showDebugPrint) {
					print ("checking to remove full armor " + currentFullArmorClothActiveName);
				}

				if (currentIndex > -1) {
					if (currentFullArmorClothTemplateList [currentIndex].fullArmorPiecesList.Contains (objectName)) {
				
						if (showDebugPrint) {
							print ("found");
						}

						configureStats (state, currentFullArmorClothTemplateList [currentIndex].armorClothStatsInfoList, objectName, true);

						fullArmorClothActive = false;

						currentFullArmorClothActiveName = "";
					}
				}
			}
		}
	}

	public void setNewArmorClothPieceTemplateData (armorClothPieceTemplateData newArmorClothPieceTemplateData)
	{
		mainArmorClothPieceTemplateData = newArmorClothPieceTemplateData;
	}

	public void setNewFullArmorClothTemplateData (fullArmorClothTemplateData newFullArmorClothTemplateData)
	{
		mainFullArmorClothTemplateData = newFullArmorClothTemplateData;
	}

	void configureStats (bool state, List<armorClothStatsInfo> armorClothStatsInfoList, string objectName, bool isFullSetActive)
	{
		if (showDebugPrint) {
			print ("configure stats " + objectName + " " + state);
		}

		int armorClothStatsInfoListCount = armorClothStatsInfoList.Count;

		for (int k = 0; k < armorClothStatsInfoListCount; k++) {
			armorClothStatsInfo currentArmorClothStatsInfo = armorClothStatsInfoList [k];

			if (currentArmorClothStatsInfo.useRemoteEvent) {
				if (state) {
					for (int j = 0; j < currentArmorClothStatsInfo.remoteEventNameListOnEquip.Count; j++) {
						mainRemoteEventSystem.callRemoteEvent (currentArmorClothStatsInfo.remoteEventNameListOnEquip [j]);
					}
				} else {
					for (int j = 0; j < currentArmorClothStatsInfo.remoteEventNameListOnUnequip.Count; j++) {
						mainRemoteEventSystem.callRemoteEvent (currentArmorClothStatsInfo.remoteEventNameListOnUnequip [j]);
					}
				}
			}

			if (upgradeStatsEnabled) {
				if (currentArmorClothStatsInfo.statIsAmount) {

					float extraValue = currentArmorClothStatsInfo.statAmount;

					if (state) {
						if (currentArmorClothStatsInfo.useRandomRange) {
							extraValue = Random.Range (currentArmorClothStatsInfo.randomRange.x, currentArmorClothStatsInfo.randomRange.y);

							extraValue = Mathf.RoundToInt (extraValue);
						}

						if (currentArmorClothStatsInfo.useStatMultiplier) {
							float currentStatValue = mainPlayerStatsSystem.getStatValue (currentArmorClothStatsInfo.Name);

							float increasedStatValue = currentStatValue * extraValue;

							float amountChangedResult = Mathf.Abs (currentStatValue - increasedStatValue);

							extraValue = amountChangedResult;
						}

						addOrRemoveTemporalStatValueList (true, currentArmorClothStatsInfo.Name, extraValue, objectName, isFullSetActive);
					} else {
						float temporalStateValue = getTemporalStatValue (currentArmorClothStatsInfo.Name, objectName, isFullSetActive);

						if (temporalStateValue > -1) {
							extraValue = -temporalStateValue;

							addOrRemoveTemporalStatValueList (false, currentArmorClothStatsInfo.Name, -1, objectName, isFullSetActive);
						} else {
							extraValue = -extraValue;
						}
					}

					if (showDebugPrint) {
						print (currentArmorClothStatsInfo.Name + " " + extraValue + " " + objectName + " " + isFullSetActive);
					}

					mainPlayerStatsSystem.increasePlayerStat (currentArmorClothStatsInfo.Name, extraValue);
				} else {
					if (state) {
						mainPlayerStatsSystem.enableOrDisableBoolPlayerStat (currentArmorClothStatsInfo.Name, currentArmorClothStatsInfo.newBoolState);
					} else {
						mainPlayerStatsSystem.enableOrDisableBoolPlayerStat (currentArmorClothStatsInfo.Name, !currentArmorClothStatsInfo.newBoolState);
					}
				}
			}

			if (upgradeSkillsEnabled) {
				if (currentArmorClothStatsInfo.unlockSkill) {
					if (state) {
						mainPlayerSkillsSystem.unlockSkillSlotByName (currentArmorClothStatsInfo.skillNameToUnlock);
					} 
//				else {
//					mainPlayerSkillsSystem.unlockSkillSlotByName (currentArmorClothStatsInfo.skillNameToUnlock);
//				}
				}
			}

			if (upgradeDamageResistanceValuesEnabled) {
				if (state) {
					if (currentArmorClothStatsInfo.setDamageTypeState) {
						mainHealth.enableOrDisableDamageTypeInfo (currentArmorClothStatsInfo.damageTypeName, currentArmorClothStatsInfo.damageTypeState);
					}

					if (currentArmorClothStatsInfo.increaseDamageType) {
						mainHealth.increaseDamageTypeInfo (currentArmorClothStatsInfo.damageTypeName, currentArmorClothStatsInfo.extraDamageType);
					}

					if (currentArmorClothStatsInfo.setObtainHealthOnDamageType) {
						mainHealth.setObtainHealthOnDamageTypeState (currentArmorClothStatsInfo.damageTypeName, currentArmorClothStatsInfo.obtainHealthOnDamageType);
					}
				} else {
					if (currentArmorClothStatsInfo.setDamageTypeState) {
						mainHealth.enableOrDisableDamageTypeInfo (currentArmorClothStatsInfo.damageTypeName, !currentArmorClothStatsInfo.damageTypeState);
					}

					if (currentArmorClothStatsInfo.increaseDamageType) {
						mainHealth.increaseDamageTypeInfo (currentArmorClothStatsInfo.damageTypeName, -currentArmorClothStatsInfo.extraDamageType);
					}

					if (currentArmorClothStatsInfo.setObtainHealthOnDamageType) {
						mainHealth.setObtainHealthOnDamageTypeState (currentArmorClothStatsInfo.damageTypeName, !currentArmorClothStatsInfo.obtainHealthOnDamageType);
					}
				}
			}

			if (upgradeAbilitiesEnabled) {
				if (currentArmorClothStatsInfo.activateAbility) {
					if (state) {
						mainPlayerAbilitiesSystem.enableAbilityByName (currentArmorClothStatsInfo.abilityToActivateName);
					} else {
						mainPlayerAbilitiesSystem.disableAbilityByName (currentArmorClothStatsInfo.abilityToActivateName);
					}
				}
			}
		}
	}

	void addOrRemoveTemporalStatValueList (bool state, string statName, float statValue, string objectName, bool isFullSetActive)
	{
		int currentIndex = -1;

		for (int i = 0; i < temporalStatValueList.Count; i++) {
			if (state) {
				if (temporalStatValueList [i].Name.Equals (statName) &&
				    temporalStatValueList [i].objectName.Equals (objectName) &&
				    temporalStatValueList [i].isFullSetActive == isFullSetActive) {

					currentIndex = i;
				}
			} else {
				if (temporalStatValueList [i].Name.Equals (statName)) {
					if (isFullSetActive) {
						if (temporalStatValueList [i].isFullSetActive) {
							currentIndex = i;
						}
					} else {
						if (temporalStatValueList [i].objectName.Equals (objectName) &&
						    temporalStatValueList [i].isFullSetActive == isFullSetActive) {

							currentIndex = i;
						}
					}
				}
			}
		}

		if (currentIndex > -1) {
			if (!state) {
				temporalStatValueList.RemoveAt (currentIndex);
			}
		} else {
			if (state) {
				temporalStatValue newTemporalStatValue = new temporalStatValue ();

				newTemporalStatValue.Name = statName;

				newTemporalStatValue.statValue = statValue;

				newTemporalStatValue.objectName = objectName;

				newTemporalStatValue.isFullSetActive = isFullSetActive;

				temporalStatValueList.Add (newTemporalStatValue);
			}
		}
	}

	float getTemporalStatValue (string statName, string objectName, bool isFullSetActive)
	{
		int currentIndex = -1;

		for (int i = 0; i < temporalStatValueList.Count; i++) {
			if (temporalStatValueList [i].Name.Equals (statName)) {
				if (isFullSetActive) {
					if (temporalStatValueList [i].isFullSetActive) {
						currentIndex = i;
					}
				} else {
					if (temporalStatValueList [i].objectName.Equals (objectName) &&
					    temporalStatValueList [i].isFullSetActive == isFullSetActive) {

						currentIndex = i;
					}
				}
			}
		}

		if (currentIndex > -1) {
			return temporalStatValueList [currentIndex].statValue;
		}

		return -1;
	}

	void checkEventOnStateChange (bool state)
	{
		if (state) {
			eventOnOpenMenu.Invoke ();
		} else {
			eventOnCloseMenu.Invoke ();
		}
	}

	public void checkCameraViewToFirstOrThirdPerson (bool state)
	{
		if (!characterCustomizationEnabled) {
			return;
		}

		if (mainCharacterAspectCustomizationUISystemAssigned) {
			mainCharacterAspectCustomizationUISystem.checkCameraViewToFirstOrThirdPerson (state);
		}
	}

	public void checkCameraViewToFirstOrThirdPersonOnEditor (bool state)
	{
		if (!characterCustomizationEnabled) {
			return;
		}

		if (mainCharacterAspectCustomizationUISystem != null) {
			mainCharacterAspectCustomizationUISystem.checkCameraViewToFirstOrThirdPersonOnEditor (state);
		}
	}

	public void setInitialArmorClothList ()
	{
		if (!characterCustomizationEnabled) {
			return;
		}

		if (!mainCharacterAspectCustomizationUISystemAssigned) {
			return;
		}

		if (useCharacterAspectCustomizationTemplate) {
			if (mainCharacterCustomizationManagerAssigned) {
				if (useRandomCharacterAspectCustomizationTemplate) {
					int randomIndex = Random.Range (0, characterAspectCustomizationTemplateList.Count);

					mainCharacterCustomizationManager.setCustomizationFromTemplate (characterAspectCustomizationTemplateList [randomIndex], true);
				} else {
					mainCharacterCustomizationManager.setCustomizationFromTemplate (initialCharacterAspectCustomizationTemplate, true);
				}

				for (int i = 0; i < mainCharacterCustomizationManager.currentPiecesList.Count; i++) {
					string objectName = mainCharacterCustomizationManager.currentPiecesList [i];

					currentPiecesList.Add (objectName);

					string categoryName = "";

					if (useInventoryManager) {
						categoryName = mainInventoryManager.getArmorClothPieceCategoryByName (objectName);
					} else {
						categoryName = mainCharacterCustomizationManager.getArmorClothCategoryByName (objectName);
					}

					checkArmorClothPieceStatsValues (true, objectName, true, categoryName);
				}

				return;
			}
		}

		int currentIndex = initialArmorClothInfoList.FindIndex (s => s.Name == initialArmorClothPieceListName);

		if (currentIndex > -1) {
			initialArmorClothInfo currentInitialArmorClothInfo = initialArmorClothInfoList [currentIndex];

			for (int i = 0; i < currentInitialArmorClothInfo.initialArmorClothPieceList.Count; i++) {
				string categoryName = "";

				if (useInventoryManager) {
					categoryName = mainCharacterAspectCustomizationUISystem.getArmorClothCategoryByName (currentInitialArmorClothInfo.initialArmorClothPieceList [i]);
				} else {
					if (mainCharacterCustomizationManagerAssigned) {
						categoryName = mainCharacterCustomizationManager.getArmorClothCategoryByName (currentInitialArmorClothInfo.initialArmorClothPieceList [i]);
					}
				}

				if (showDebugPrint) {
					print ("setting initial piece for " + currentInitialArmorClothInfo.initialArmorClothPieceList [i] + " " + categoryName);
				}

				if (categoryName != "") {
					equipObject (currentInitialArmorClothInfo.initialArmorClothPieceList [i], categoryName);
				}
			}
		}
	}

	public void dropArmorClothPieceByCategoryName (string categoryName)
	{
		if (!dropArmorClothPiecesExternallyEnabled) {
			return;
		}

		string pieceName = "";

		if (useInventoryManager) {
			pieceName = mainCharacterAspectCustomizationUISystem.getArmorClothPieceByName (categoryName);
		} else {
			if (mainCharacterCustomizationManagerAssigned) {
				pieceName = mainCharacterCustomizationManager.getArmorClothPieceByName (categoryName);
			}
		}

		if (showDebugPrint) {
			print ("piece name to drop " + pieceName);
		}

		dropArmorClothPieceByName (pieceName);
	}

	public void dropArmorClothPieceByName (string pieceName)
	{
		if (!dropArmorClothPiecesExternallyEnabled) {
			return;
		}

		if (pieceName == null || pieceName == "") {
			return;
		}

		if (!currentPiecesList.Contains (pieceName)) {
			return;
		}

		if (useInventoryManager) {
			if (instantiatePiecesDroppedExternallyEnabled) {
				mainInventoryManager.dropEquipByName (pieceName, 1, true, false);
			} else {
				string categoryName = mainInventoryManager.getArmorClothPieceCategoryByName (pieceName);

				if (categoryName != "") {
					equipOrUnequipObject (false, pieceName, categoryName);
				}

				mainInventoryManager.removeObjectAmountFromInventoryByName (pieceName, 1);
			}

			GameObject droppedObject = mainInventoryManager.getLastObjectDropped ();

			if (droppedObject != null) {
				setForceToDroppedObject (droppedObject);
			}
		} else {
			inventoryListManager mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

			if (mainInventoryListManager != null) {
				GameObject objectToDrop = mainInventoryListManager.getInventoryPrefabByName (pieceName);

				if (objectToDrop == null) {
					print ("WARNING: the object called " + pieceName + " wasn't found");

					return;
				}

				armorClothPickup currentPickupObject = objectToDrop.GetComponent<armorClothPickup> ();

				if (currentPickupObject != null) {
					if (instantiatePiecesDroppedExternallyEnabled) {
						Vector3 positionToSpawn = playerTransform.position + playerTransform.forward + playerTransform.up;

						//instantiate and drag the weapon object
						GameObject objectToSpawn = (GameObject)Instantiate (objectToDrop, positionToSpawn, playerTransform.rotation);

						setForceToDroppedObject (objectToSpawn);
					}
						
					equipOrUnequipObject (false, pieceName, currentPickupObject.categoryName);
				}
			}
		}
	}

	void setForceToDroppedObject (GameObject droppedObject)
	{
		if (addForceToDroppedObject) {

			Rigidbody objectToSpawnRigidbody = droppedObject.GetComponent<Rigidbody> ();

			if (objectToSpawnRigidbody != null) {
				objectToSpawnRigidbody.AddExplosionForce (forceAmountToDroppedObject, droppedObject.transform.position, 3, 1, forceModeForDroppedObject);

			} 
		}
	}

	public void addOrRemoveDurabilityAmountToObjectByCategoryName (string categoryName, float newAmount, bool setAmountAsCurrentValue)
	{
		if (storeDurabilityValuesOfPiecesEnabled) {
			string objectName = categoryName;

			currentPiecesDictionaryList.TryGetValue (categoryName, out objectName);

			int durabilityIndex = currentPiecesList.IndexOf (objectName);

			if (useEventOnDurabilityAffectedEnabled) {
				if (mainCharacterCustomizationManagerAssigned) {
					mainCharacterCustomizationManager.checkEventOnDurabilityAffected (categoryName);
				}
			}

			if (durabilityIndex >= 0) {

				if (currentPiecesDurabilityList.Count <= durabilityIndex || currentPiecesDurabilityList [durabilityIndex] == -1) {
					return;
				}

				if (setAmountAsCurrentValue) {
					currentPiecesDurabilityList [durabilityIndex] = newAmount;
				} else {
					currentPiecesDurabilityList [durabilityIndex] += newAmount;
				}

				if (currentPiecesDurabilityList [durabilityIndex] <= 0) {
					currentPiecesDurabilityList [durabilityIndex] = 0;

					if (useInventoryManager) {
						mainInventoryManager.addOrRemoveDurabilityAmountToObjectByName (objectName, currentPiecesDurabilityList [durabilityIndex], true);
					} else {

					}
				}
			}
		}
	}

	public void updateDurabilityAmountStateOnAllObjects ()
	{
		if (storeDurabilityValuesOfPiecesEnabled) {
			for (int i = 0; i < currentPiecesList.Count; i++) {
				if (currentPiecesDurabilityList.Count > i && currentPiecesDurabilityList [i] != -1) {
					
					if (useInventoryManager) {
						mainInventoryManager.addOrRemoveDurabilityAmountToObjectByName (currentPiecesList [i], currentPiecesDurabilityList [i], true);
					} else {

					}
				}
			}
		}
	}

	public void updateDurabilityAmountStateOnObjectByName (string objectName)
	{
		if (storeDurabilityValuesOfPiecesEnabled) {
			for (int i = 0; i < currentPiecesList.Count; i++) {
				if (currentPiecesList [i].Equals (objectName)) {
					if (currentPiecesDurabilityList.Count > i && currentPiecesDurabilityList [i] != -1) {
						if (useInventoryManager) {
							mainInventoryManager.addOrRemoveDurabilityAmountToObjectByName (currentPiecesList [i], currentPiecesDurabilityList [i], true);
						} else {

						}
					}

					return;
				}
			}
		}
	}

	public float getDurabilityAmountStateOnObjectByName (string objectName)
	{
		if (storeDurabilityValuesOfPiecesEnabled) {
			for (int i = 0; i < currentPiecesList.Count; i++) {
				if (currentPiecesList [i].Equals (objectName) && currentPiecesDurabilityList.Count > i) {
					return currentPiecesDurabilityList [i];
				}
			}
		}

		return -1;
	}

	public void initializeDurabilityValue (float newAmount, string objectName)
	{
		if (storeDurabilityValuesOfPiecesEnabled) {
			for (int i = 0; i < currentPiecesList.Count; i++) {
				if (currentPiecesList [i].Equals (objectName)) {
					if (currentPiecesDurabilityList.Count > i && currentPiecesDurabilityList [i] != -1) {
						currentPiecesDurabilityList [i] = newAmount;
					}

					return;
				}
			}
		}
	}

	public void repairObjectFully (string objectName)
	{
		if (storeDurabilityValuesOfPiecesEnabled) {
			for (int i = 0; i < currentPiecesList.Count; i++) {
				if (currentPiecesList [i].Equals (objectName)) {
					if (currentPiecesDurabilityList.Count > i && currentPiecesDurabilityList [i] != -1) {
						float newDurabilityVale = mainInventoryManager.getDurabilityValueOnObjectByName (objectName);

						currentPiecesDurabilityList [i] = newDurabilityVale;

						if (useInventoryManager) {
							mainInventoryManager.addOrRemoveDurabilityAmountToObjectByName (currentPiecesList [i], currentPiecesDurabilityList [i], true);
						} else {

						}
					}

					return;
				}
			}
		}
	}

	public void setKeepAllCharacterMeshesDisabledActiveStateOnEditor (bool state)
	{
		keepAllCharacterMeshesDisabledActive = state;

		updateComponent ();
	}

	public void setStoreDurabilityValuesOfPiecesEnabledValueOnEditor (bool state)
	{
		storeDurabilityValuesOfPiecesEnabled = state;

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Character Customization", gameObject);
	}


	[System.Serializable]
	public class elementSlotInfo
	{
		public string Name;

		public GameObject slotObject;
	}

	[System.Serializable]
	public class temporalStatValue
	{
		public string Name;

		public string objectName;

		public bool isFullSetActive;

		public float statValue;
	}

	[System.Serializable]
	public class initialArmorClothInfo
	{
		public string Name;

		public List<string> initialArmorClothPieceList = new List<string> ();
	}
}