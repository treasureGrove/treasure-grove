using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class meleeWeaponsGrabbedManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool meleeWeaponsGrabbedManagerEnabled = true;

	public bool storeEachGrabbedWeapon;

	public bool storeOnlyOneWeapon;

	public bool canSpawnAnyWeaponStoredInfiniteTimes;

	public bool storePickedWeaponsOnInventory;

	public bool drawWeaponAtStartIfFoundOnInitializingInventory;

	public bool numberKeysToChangeBetweenMeleeWeaponsEnabled = true;

	[Space]
	[Header ("Hide Weapon Meshes Settings")]
	[Space]

	public bool overrideOptionToHideWeaponMeshWhenNotUsed;
	public bool hideWeaponMeshWhenNotUsed;

	[Space]
	[Header ("Start Game With Melee Weapon Settings")]
	[Space]

	public bool startGameWithWeapon;
	public string weaponNameToStartGame;

	[Space]
	[Space]

	public bool startGameWithListOfWeapons;
	public List<string> listOfWeaponsToStart = new List<string> ();

	[Space]

	public bool startWithRandomWeapon;

	public bool startWithRandomWeaponFromListOfWeaponsToStart;

	public bool startWithWeaponOnlyIfAlreadyAvailable;

	public bool drawWeaponAtStartIfConfiguredExternally;

	[Space]
	[Header ("Start Game With Melee Shield Settings")]
	[Space]

	public bool startGameWithShield;
	public string shieldNameToStartGame;

	[Space]
	[Header ("Weapon List")]
	[Space]

	public List<meleeWeaponGrabbedInfo> meleeWeaponGrabbedInfoList = new List<meleeWeaponGrabbedInfo> ();

	public List<meleeWeaponPrefabInfo> meleeWeaponPrefabInfoList = new List<meleeWeaponPrefabInfo> ();

	[Space]
	[Header ("Shield List")]
	[Space]

	public List<shieldPrefabInfo> shieldPrefabInfoList = new List<shieldPrefabInfo> ();

	public List<shieldGrabbedInfo> shieldGrabbedInfoList = new List<shieldGrabbedInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStateChange;
	public UnityEvent eventOnStateActive;
	public UnityEvent eventOnStateDeactivate;

	public bool useEventsOnMeleeWeaponEquipped;
	public UnityEvent eventOnMeleeWeaponEquipped;

	[Space]
	[Header ("Stolen Weapon Settings")]
	[Space]

	public bool useEventOnWeaponStolen;
	public UnityEvent eventOnWeaponStolen;

	[Space]
	[Header ("Save Settings")]
	[Space]

	public bool saveCurrentMeleeWeaponListToSaveFile = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool meleeWeaponsGrabbedManagerActive;

	public int currentNumberOfWeaponsAvailable;

	public int currentWeaponIndex = 0;

	public bool equipMeleeWeaponPaused;

	public bool currentMeleeWeaponSheathedOrCarried;

	public bool isLoadingGame;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject playerGameObject;
	public playerInputManager playerInput;
	public grabObjects mainGrabObjects;
	public grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;
	public inventoryManager mainInventoryManager;

	bool startWithWeaponChecked;

	bool startWithShieldChecked;

	bool drawMeleeWeaponsPaused;

	shieldGrabbedInfo currentShieldGrabbedInfo;

	Coroutine weaponDrawKeepAnimationCoroutine;

	bool lastDroppeWeaponDurabilityStored;
	float lastDroppedWeaponDurabilityValue;
	string lastDroppedWeaponName;


	public void initializeMeleeManagerValues ()
	{

	}

	void Start ()
	{
		currentNumberOfWeaponsAvailable = meleeWeaponGrabbedInfoList.Count;

		if (playerGameObject == null) {
			playerGameObject = mainGrabObjects.gameObject;
		}
	}

	public void drawNextWeaponAvailable ()
	{
		getCurrentNumberOfWeaponsAvailable ();

		if (currentNumberOfWeaponsAvailable > 0) {
			
			checkMeleeWeaponToUse (meleeWeaponGrabbedInfoList [0].Name, startWithWeaponOnlyIfAlreadyAvailable);
		}
	}

	public void drawRandomWeaponFromPrefabList (string previousWeaponName)
	{
		int previousWeaponIndex = -1;

		if (previousWeaponName != "") {
			previousWeaponIndex = meleeWeaponPrefabInfoList.FindIndex (s => s.Name == previousWeaponName);
		}

		int newWeaponIndex = -1;

		if (previousWeaponIndex > -1) {
			
			bool exit = false;

			int loopCount = 0;

			while (!exit) {
				int randomWeaponIndex = Random.Range (0, meleeWeaponPrefabInfoList.Count - 1);

				if (randomWeaponIndex != previousWeaponIndex) {
					newWeaponIndex = randomWeaponIndex;

					exit = true;
				}

				loopCount++;

				if (loopCount > 100) {
					return;
				}
			}
		} else {
			newWeaponIndex = Random.Range (0, meleeWeaponPrefabInfoList.Count - 1);
		}

		if (newWeaponIndex > -1) {
			checkMeleeWeaponToUse (meleeWeaponPrefabInfoList [newWeaponIndex].Name, false);
		}
	}

	public void inputSetWeaponByIndex (int currentNumberInput)
	{
		if (currentNumberInput > -1) {
			if (currentNumberInput == 0) {
				currentNumberInput = 9;
			} else {
				currentNumberInput--;
			}

			if (meleeWeaponGrabbedInfoList.Count == 0 || currentNumberInput >= meleeWeaponGrabbedInfoList.Count) {
				return;
			}

			if (isGrabObjectsEnabled ()) {
				checkWeaponByNumber (currentNumberInput);
			}
		}
	}

	void Update ()
	{
		if (meleeWeaponsGrabbedManagerActive) {
			if (!storePickedWeaponsOnInventory) {
				if (numberKeysToChangeBetweenMeleeWeaponsEnabled) {
					if (!mainGrabbedObjectMeleeAttackSystem.isObjectThrownTravellingToTarget () &&
					    canUseInput ()) {

						int currentNumberInput = playerInput.checkNumberInput (currentNumberOfWeaponsAvailable + 1);

						if (currentNumberInput > -1) {
							inputSetWeaponByIndex (currentNumberInput);
						}
					}
				}
			}

			if (!startWithShieldChecked) {
				if (startGameWithShield && isGrabObjectsEnabled ()) {
					setShieldActiveState (true, shieldNameToStartGame);

					startWithShieldChecked = true;
				}
			}

			if (!startWithWeaponChecked) {

				if (startGameWithWeapon) {
					if (isGrabObjectsEnabled ()) {
						if (!mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
							if (startGameWithListOfWeapons && !isLoadingGame) {
								setInitialWeaponsList ();
							}

							if (startWithRandomWeapon) {
								if (startWithRandomWeaponFromListOfWeaponsToStart) {
									int randomWeaponIndex = Random.Range (0, meleeWeaponGrabbedInfoList.Count - 1);

									weaponNameToStartGame = meleeWeaponGrabbedInfoList [randomWeaponIndex].Name;
								} else {
									int randomWeaponIndex = Random.Range (0, meleeWeaponPrefabInfoList.Count - 1);

									weaponNameToStartGame = meleeWeaponPrefabInfoList [randomWeaponIndex].Name;
								}
							}

							if (weaponNameToStartGame != "") {
								checkMeleeWeaponToUse (weaponNameToStartGame, startWithWeaponOnlyIfAlreadyAvailable);
							}
						}

						startWithWeaponChecked = true;
					}
				} else {
					if (drawWeaponAtStartIfConfiguredExternally) {
						if (isGrabObjectsEnabled ()) {
							if (!mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
								if (meleeWeaponGrabbedInfoList.Count > 0) {
									checkMeleeWeaponToUse (meleeWeaponGrabbedInfoList [0].Name, false);

									if (showDebugPrint) {
										print ("checking to draw weapon when starting game and checking for grabbed object list");
									}
								}
							}
						}

						startWithWeaponChecked = true;
					}
				}
			}
		}
	}

	void setInitialWeaponsList ()
	{
		int listOfWeaponsToStartCount = listOfWeaponsToStart.Count;

		for (int i = 0; i < listOfWeaponsToStartCount; i++) {
			int weaponIndex = meleeWeaponPrefabInfoList.FindIndex (s => s.Name == listOfWeaponsToStart [i]);

			if (weaponIndex > -1) {
				meleeWeaponPrefabInfo currentMeleeWeaponPrefabInfo = meleeWeaponPrefabInfoList [weaponIndex];

				if (currentMeleeWeaponPrefabInfo != null) {
					meleeWeaponGrabbedInfo newMeleeWeaponGrabbedInfo = new meleeWeaponGrabbedInfo ();

					newMeleeWeaponGrabbedInfo.Name = currentMeleeWeaponPrefabInfo.Name;

					newMeleeWeaponGrabbedInfo.weaponPrefabIndex = currentMeleeWeaponPrefabInfo.weaponPrefabIndex;

					meleeWeaponGrabbedInfoList.Add (newMeleeWeaponGrabbedInfo);
				}
			}
		}

		if (!isGrabObjectsEnabled ()) {
			return;
		}

		int meleeWeaponGrabbedInfoListCount = meleeWeaponGrabbedInfoList.Count;

		for (int k = 0; k < meleeWeaponGrabbedInfoListCount; k++) {
			meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [k];

			currentMeleeWeaponGrabbedInfo.weaponInstantiated = true;

			meleeWeaponPrefabInfo currentMeleeWeaponPrefabInfo = getWeaponPrefabByName (currentMeleeWeaponGrabbedInfo.Name);

			if (currentMeleeWeaponPrefabInfo != null) {
				currentMeleeWeaponGrabbedInfo.weaponStored = (GameObject)Instantiate (currentMeleeWeaponPrefabInfo.weaponPrefab, Vector3.up * 1000, Quaternion.identity);

				currentMeleeWeaponGrabbedInfo.weaponPrefabIndex = currentMeleeWeaponPrefabInfo.weaponPrefabIndex;

				bool hideWeaponMeshResult = checkIfHideWeaponMeshWhenNotUsed (currentMeleeWeaponGrabbedInfo.hideWeaponMeshWhenNotUsed);

				checkObjectMeshToEnableOrDisable (!hideWeaponMeshResult, currentMeleeWeaponGrabbedInfo);

				if (currentMeleeWeaponGrabbedInfo.weaponStored.activeSelf) {
					currentMeleeWeaponGrabbedInfo.weaponStored.SetActive (false);
				}
			}
		}
	}

	bool checkIfHideWeaponMeshWhenNotUsed (bool currentValue)
	{
		if (overrideOptionToHideWeaponMeshWhenNotUsed) {
			return hideWeaponMeshWhenNotUsed;
		}

		return currentValue;
	}

	public bool checkMeleeWeaponToUse (string weaponNameToSearch, bool checkIfWeaponNotFound)
	{
		bool canSearchWeapon = true;

		if (showDebugPrint) {
			print (weaponNameToSearch);
		}

		int weaponIndex = meleeWeaponGrabbedInfoList.FindIndex (s => s.Name == weaponNameToSearch);

		if (checkIfWeaponNotFound) {
			if (weaponIndex == -1) {
				canSearchWeapon = false;
			}
		}

		if (canSearchWeapon) {
			
			if (weaponIndex > -1 && meleeWeaponGrabbedInfoList [weaponIndex].weaponInstantiated) {
				if (showDebugPrint) {
					print ("check weapon by number");
				}

				checkWeaponByNumber (weaponIndex);

				return true;
			} else {
				if (showDebugPrint) {
					print ("instantiate new weapon");
				}

				int weaponPrefabIndex = meleeWeaponPrefabInfoList.FindIndex (s => s.Name == weaponNameToSearch);

				if (weaponPrefabIndex > -1) {
					GameObject newWeaponToCarry = (GameObject)Instantiate (meleeWeaponPrefabInfoList [weaponPrefabIndex].weaponPrefab, Vector3.up * 1000, Quaternion.identity);

					grabPhysicalObjectSystem currentGrabPhysicalObjectSystem = newWeaponToCarry.GetComponent<grabPhysicalObjectSystem> ();

					currentGrabPhysicalObjectSystem.setCurrentPlayer (playerGameObject);

					mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (false);

					mainGrabObjects.grabPhysicalObjectExternally (newWeaponToCarry);

					mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (true);

					return true;
				}
			}
		}

		return false;
	}

	public void setCheckDrawKeepWeaponAnimationPauseState (bool state)
	{
		mainGrabbedObjectMeleeAttackSystem.setCheckDrawKeepWeaponAnimationPauseState (state);
	}

	public void checkIfDrawWeapon ()
	{
		if (meleeWeaponsGrabbedManagerActive) {
			checkWeaponByNumber (currentWeaponIndex);

			mainGrabbedObjectMeleeAttackSystem.checkEventWhenKeepingOrDrawingMeleeWeapon (false);
		}
	}
		
	//Check for the melee draw/keep actions if using animations for those actions
	void activateDrawkeepMeleeWeaponAnimation (meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo, bool useExtraDelay)
	{
		stopActivateDrawkeepMeleeWeaponAnimationCoroutine ();

		weaponDrawKeepAnimationCoroutine = StartCoroutine (activateDrawkeepMeleeWeaponAnimationCoroutine (currentMeleeWeaponGrabbedInfo, useExtraDelay));
	}

	public void stopActivateDrawkeepMeleeWeaponAnimationCoroutine ()
	{
		if (weaponDrawKeepAnimationCoroutine != null) {
			StopCoroutine (weaponDrawKeepAnimationCoroutine);
		}
	}

	IEnumerator activateDrawkeepMeleeWeaponAnimationCoroutine (meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo, bool useExtraDelay)
	{
		bool targetReached = false;

		if (useExtraDelay) {
			yield return new WaitForSeconds (0.3f);
		}

		while (!targetReached) {
			if (!mainGrabbedObjectMeleeAttackSystem.mainPlayerController.isActionActive ()) {
				targetReached = true;
			}


			yield return null;
		}

		confirmToDrawNextWeapon (false, currentMeleeWeaponGrabbedInfo);
	}

	void checkWeaponByNumber (int currentNumberInput)
	{
		if (currentNumberInput >= meleeWeaponGrabbedInfoList.Count || currentNumberInput < 0) {
			return;
		}

		meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [currentNumberInput];

		bool isCurrentWeapon = currentMeleeWeaponGrabbedInfo.isCurrentWeapon;

		bool previousWeaponIsKept = false;

		bool previousWeaponUseDrawKeepAnimations = false;

		if (!isCurrentWeapon) {

			currentMeleeWeaponGrabbedInfo.isCurrentWeapon = true;

			currentMeleeWeaponGrabbedInfo.carryingWeapon = true;

			currentMeleeWeaponSheathedOrCarried = true;

			currentWeaponIndex = currentNumberInput;

			bool weaponToKeepFound = false;

			for (int k = 0; k < meleeWeaponGrabbedInfoList.Count; k++) {
				if (k != currentNumberInput) {
					if (meleeWeaponGrabbedInfoList [k].isCurrentWeapon) {
						keepWeapon (k);

						weaponToKeepFound = true;

						previousWeaponIsKept = true;

						if (meleeWeaponGrabbedInfoList [k].weaponStored != null) {
							grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = meleeWeaponGrabbedInfoList [k].weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

							previousWeaponUseDrawKeepAnimations = currentGrabPhysicalObjectMeleeAttackSystem.useDrawKeepWeaponAnimation;
						}
					}

					meleeWeaponGrabbedInfoList [k].isCurrentWeapon = false;

					meleeWeaponGrabbedInfoList [k].carryingWeapon = false;
				} 
			}

			if (!weaponToKeepFound && !storeEachGrabbedWeapon) {
				if (mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
					mainGrabObjects.dropObject ();

					mainGrabObjects.clearPhysicalObjectToGrabFoundList ();
				}
			}
		}

		bool confirmToDrawNextWeaponResult = true;

		if (previousWeaponIsKept) {
			if (!isCurrentWeapon) {
				if (previousWeaponUseDrawKeepAnimations && !mainGrabbedObjectMeleeAttackSystem.ignoreUseDrawKeepWeaponAnimation) {
					confirmToDrawNextWeaponResult = false;

					activateDrawkeepMeleeWeaponAnimation (currentMeleeWeaponGrabbedInfo, true);
				}
			}
		}

		if (confirmToDrawNextWeaponResult) {
			confirmToDrawNextWeapon (isCurrentWeapon, currentMeleeWeaponGrabbedInfo);
		}
	}

	void confirmToDrawNextWeapon (bool isCurrentWeapon, meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo)
	{
		if (!isCurrentWeapon) {
			bool weaponInstantiated = false;

			if (!currentMeleeWeaponGrabbedInfo.weaponInstantiated || currentMeleeWeaponGrabbedInfo.canBeSpawnedInfiniteTimes || canSpawnAnyWeaponStoredInfiniteTimes) {
				if (currentMeleeWeaponGrabbedInfo.weaponStored == null) {

					currentMeleeWeaponGrabbedInfo.weaponInstantiated = true;

					meleeWeaponPrefabInfo currentMeleeWeaponPrefabInfo = getWeaponPrefabByName (currentMeleeWeaponGrabbedInfo.Name);

					if (currentMeleeWeaponPrefabInfo != null) {
						currentMeleeWeaponGrabbedInfo.weaponStored = (GameObject)Instantiate (currentMeleeWeaponPrefabInfo.weaponPrefab, Vector3.up * 1000, Quaternion.identity);
			
						currentMeleeWeaponGrabbedInfo.weaponPrefabIndex = currentMeleeWeaponPrefabInfo.weaponPrefabIndex;

						grabPhysicalObjectSystem currentGrabPhysicalObjectSystem = currentMeleeWeaponGrabbedInfo.weaponStored.GetComponent<grabPhysicalObjectSystem> ();

						currentGrabPhysicalObjectSystem.setCurrentPlayer (playerGameObject);

						mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (false);

						mainGrabObjects.grabPhysicalObjectExternally (currentMeleeWeaponGrabbedInfo.weaponStored);

						mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (true);

						weaponInstantiated = true;

						mainGrabbedObjectMeleeAttackSystem.activateStartDrawAnimation ();
					} else {
						print ("WARNING: melee weapon prefab with the name " + currentMeleeWeaponGrabbedInfo.Name + " not found, make sure to configure a weapon" +
						" with that info.");

						return;
					}
				}
			} 

			if (!weaponInstantiated) {
				if (!currentMeleeWeaponGrabbedInfo.weaponStored.activeSelf) {
					currentMeleeWeaponGrabbedInfo.weaponStored.SetActive (true);
				}

				bool hideWeaponMeshResult = checkIfHideWeaponMeshWhenNotUsed (currentMeleeWeaponGrabbedInfo.hideWeaponMeshWhenNotUsed);

				if (!hideWeaponMeshResult) {
					checkObjectMeshToEnableOrDisable (false, currentMeleeWeaponGrabbedInfo);
				}

				grabPhysicalObjectSystem currentGrabPhysicalObjectSystem = currentMeleeWeaponGrabbedInfo.weaponStored.GetComponent<grabPhysicalObjectSystem> ();

				currentGrabPhysicalObjectSystem.setCurrentPlayer (playerGameObject);

				mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (false);

				mainGrabObjects.grabPhysicalObjectExternally (currentMeleeWeaponGrabbedInfo.weaponStored);

				mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (true);

				mainGrabbedObjectMeleeAttackSystem.activateStartDrawAnimation ();
			}
		}

		mainGrabbedObjectMeleeAttackSystem.drawOrSheatheShield (true);
	}

	public void enableOrDisableMeleeWeaponsGrabbedManager (bool state)
	{
		if (!meleeWeaponsGrabbedManagerEnabled) {
			return;
		}
			
		if (meleeWeaponsGrabbedManagerActive == state) {
			return;
		}

		meleeWeaponsGrabbedManagerActive = state;

		if (meleeWeaponsGrabbedManagerActive) {
			if (!drawMeleeWeaponsPaused) {
				bool canDrawWeapon = true;

				if (startGameWithWeapon && !startWithWeaponChecked) {
					canDrawWeapon = false;
				}

				if (canDrawWeapon) {
					if (!mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
						checkWeaponByNumber (currentWeaponIndex);

						if (mainGrabbedObjectMeleeAttackSystem.isCarryingObject () && currentWeaponIndex >= 0 && currentWeaponIndex < meleeWeaponGrabbedInfoList.Count) {
							updateQuickAccesSlotOnInventory (meleeWeaponGrabbedInfoList [currentWeaponIndex].Name);
						}
					}
				}
			}
		} else {
			for (int k = 0; k < meleeWeaponGrabbedInfoList.Count; k++) {
				if (meleeWeaponGrabbedInfoList [k].isCurrentWeapon) {
					keepWeapon (k);
				}

				meleeWeaponGrabbedInfoList [k].isCurrentWeapon = false;
			} 
		}

		if (useEventsOnStateChange) {
			if (meleeWeaponsGrabbedManagerActive) {
				eventOnStateActive.Invoke ();
			} else {
				eventOnStateDeactivate.Invoke ();
			}
		}

		if (storePickedWeaponsOnInventory) {
			mainInventoryManager.checkToEnableOrDisableQuickAccessSlotsParentOutOfInventoryFromMeleeWeaponsMode (meleeWeaponsGrabbedManagerActive);
		}

		drawMeleeWeaponsPaused = false;

		if (!meleeWeaponsGrabbedManagerActive) {
			currentMeleeWeaponSheathedOrCarried = false; 
		}
	}

	public void setDrawMeleeWeaponsPausedState (bool state)
	{
		drawMeleeWeaponsPaused = state;
	}

	void keepWeapon (int weaponIndex)
	{
		if (!mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
			return;
		}

		if (meleeWeaponGrabbedInfoList.Count > weaponIndex) {

			meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [weaponIndex];

			if (currentMeleeWeaponGrabbedInfo.weaponStored != null) {
				mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (false);

				mainGrabbedObjectMeleeAttackSystem.checkEventWhenKeepingOrDrawingMeleeWeapon (true);

				mainGrabObjects.grabbed = true;

				mainGrabObjects.checkIfDropObject (currentMeleeWeaponGrabbedInfo.weaponStored);

				mainGrabObjects.removeCurrentPhysicalObjectToGrabFound (currentMeleeWeaponGrabbedInfo.weaponStored);

				if (currentMeleeWeaponGrabbedInfo.weaponStored.activeSelf) {
					currentMeleeWeaponGrabbedInfo.weaponStored.SetActive (false);
				}

				mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (true);

				bool hideWeaponMeshResult = checkIfHideWeaponMeshWhenNotUsed (currentMeleeWeaponGrabbedInfo.hideWeaponMeshWhenNotUsed);

				if (!hideWeaponMeshResult) {
					checkObjectMeshToEnableOrDisable (true, currentMeleeWeaponGrabbedInfo);
				}

				mainGrabbedObjectMeleeAttackSystem.drawOrSheatheShield (false);

				bool canUseWeaponKeepAnimation = true;

				if (mainGrabbedObjectMeleeAttackSystem.mainPlayerController.isCustomCharacterControllerActive () ||
				    mainGrabbedObjectMeleeAttackSystem.mainPlayerController.isPlayerDriving () ||
				    mainGrabbedObjectMeleeAttackSystem.mainPlayerController.isUsingDevice ()) {
					canUseWeaponKeepAnimation = false;
				} 
					
				if (canUseWeaponKeepAnimation) {
					mainGrabbedObjectMeleeAttackSystem.activateStartKeepAnimation ();
				} else {
					mainGrabbedObjectMeleeAttackSystem.removeDrawKeepWeaponAnimationInfo ();
				}
			}

			currentMeleeWeaponGrabbedInfo.isCurrentWeapon = false;
		}
	}

	public void disableCurrentMeleeWeapon (string weaponName)
	{
		if (!mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
			return;
		}

		int weaponIndex = meleeWeaponGrabbedInfoList.FindIndex (s => s.Name == weaponName);

		if (meleeWeaponGrabbedInfoList.Count > weaponIndex && weaponIndex > -1) {

			meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [weaponIndex];

			if (currentMeleeWeaponGrabbedInfo.isCurrentWeapon) {
				if (currentMeleeWeaponGrabbedInfo.weaponStored != null) {
					mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (false);

					mainGrabObjects.grabbed = true;

					mainGrabObjects.checkIfDropObject (currentMeleeWeaponGrabbedInfo.weaponStored);

					mainGrabObjects.removeCurrentPhysicalObjectToGrabFound (currentMeleeWeaponGrabbedInfo.weaponStored);

					if (currentMeleeWeaponGrabbedInfo.weaponStored.activeSelf) {
						currentMeleeWeaponGrabbedInfo.weaponStored.SetActive (false);
					}

					mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (true);

					bool hideWeaponMeshResult = checkIfHideWeaponMeshWhenNotUsed (currentMeleeWeaponGrabbedInfo.hideWeaponMeshWhenNotUsed);

					if (!hideWeaponMeshResult) {
						checkObjectMeshToEnableOrDisable (true, currentMeleeWeaponGrabbedInfo);
					}
				}

				currentMeleeWeaponGrabbedInfo.isCurrentWeapon = false;

				currentMeleeWeaponGrabbedInfo.carryingWeapon = false;
			}
		}
	}

	public void checkObjectMeshToEnableOrDisable (bool enableWeaponMesh, meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo)
	{
		grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = currentMeleeWeaponGrabbedInfo.weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

		if (currentMeleeWeaponGrabbedInfo.weaponMesh == null) {
			GameObject weaponMeshToInstantiate = currentGrabPhysicalObjectMeleeAttackSystem.weaponMesh;

			if (currentGrabPhysicalObjectMeleeAttackSystem.useCustomWeaponMeshToInstantiate) {
				weaponMeshToInstantiate = currentGrabPhysicalObjectMeleeAttackSystem.customWeaponMeshToInstantiate;
			}

			currentMeleeWeaponGrabbedInfo.weaponMesh = (GameObject)Instantiate (weaponMeshToInstantiate, Vector3.zero, Quaternion.identity);
		
			currentMeleeWeaponGrabbedInfo.weaponMesh.transform.localScale = currentGrabPhysicalObjectMeleeAttackSystem.weaponMesh.transform.localScale;
		}

		if (currentMeleeWeaponGrabbedInfo.weaponMesh != null) {
			if (currentMeleeWeaponGrabbedInfo.weaponMesh.activeSelf != enableWeaponMesh) {
				currentMeleeWeaponGrabbedInfo.weaponMesh.SetActive (enableWeaponMesh);

				if (showDebugPrint) {
					print (enableWeaponMesh);
				}
			}

			if (enableWeaponMesh) {
				grabPhysicalObjectSystem currentGrabPhysicalObjectSystem = currentMeleeWeaponGrabbedInfo.weaponStored.GetComponent<grabPhysicalObjectSystem> ();

				bool setWeaponMeshOnPhysicalWeaponPlace = false;

				if (enableWeaponMesh) {
					currentMeleeWeaponGrabbedInfo.objectThrown = currentGrabPhysicalObjectMeleeAttackSystem.isObjectThrown ();

					if (currentMeleeWeaponGrabbedInfo.objectThrown) {
						setWeaponMeshOnPhysicalWeaponPlace = true;
					}
				}

				Transform newParent = null;

				bool useMountPointToKeepObject = currentGrabPhysicalObjectSystem.useMountPointToKeepObject;

				if (useMountPointToKeepObject) {
					newParent = GKC_Utils.getMountPointTransformByName (currentGrabPhysicalObjectSystem.mountPointTokeepObjectName, playerGameObject.transform);
				} 

				if (!useMountPointToKeepObject || newParent == null) {
					newParent = mainGrabbedObjectMeleeAttackSystem.getCharacterHumanBone (currentGrabPhysicalObjectSystem.boneToKeepObject);
				}

				Vector3 targetPosition = currentGrabPhysicalObjectMeleeAttackSystem.referencePositionToKeepObjectMesh.localPosition;
				Quaternion targetRotation = currentGrabPhysicalObjectMeleeAttackSystem.referencePositionToKeepObjectMesh.localRotation;

				if (currentGrabPhysicalObjectMeleeAttackSystem.useCustomReferencePositionToKeepObjectMesh) {
					targetPosition = currentGrabPhysicalObjectMeleeAttackSystem.customReferencePositionToKeepObjectMesh.localPosition;
					targetRotation = currentGrabPhysicalObjectMeleeAttackSystem.customReferencePositionToKeepObjectMesh.localRotation;
				}

				if (setWeaponMeshOnPhysicalWeaponPlace) {
					Transform lastParentAssigned = currentGrabPhysicalObjectSystem.getLastParentAssigned ();

					if (lastParentAssigned != null) {
						currentMeleeWeaponGrabbedInfo.weaponStored.transform.SetParent (lastParentAssigned);
					}

					currentMeleeWeaponGrabbedInfo.weaponMesh.transform.SetParent (currentGrabPhysicalObjectMeleeAttackSystem.weaponMesh.transform);
				
					currentMeleeWeaponGrabbedInfo.weaponMesh.transform.localPosition = Vector3.zero;
					currentMeleeWeaponGrabbedInfo.weaponMesh.transform.localRotation = Quaternion.identity;

					newParent = currentGrabPhysicalObjectSystem.transform.parent;

					currentMeleeWeaponGrabbedInfo.weaponMesh.transform.SetParent (newParent);
				} else {

					currentMeleeWeaponGrabbedInfo.weaponMesh.transform.SetParent (newParent);

					currentMeleeWeaponGrabbedInfo.weaponMesh.transform.localPosition = targetPosition;
					currentMeleeWeaponGrabbedInfo.weaponMesh.transform.localRotation = targetRotation;
				}

				currentMeleeWeaponGrabbedInfo.weaponMesh.transform.localScale = Vector3.one;
			}
		}
	}

	public meleeWeaponPrefabInfo getWeaponPrefabByName (string weaponName)
	{
		for (int k = 0; k < meleeWeaponPrefabInfoList.Count; k++) {
			if (meleeWeaponPrefabInfoList [k].Name.Equals (weaponName)) {
				return meleeWeaponPrefabInfoList [k];
			}
		}

		return null;
	}

	public bool checkIfCanUseMeleeWeaponPrefabByName (string weaponName)
	{
		for (int k = 0; k < meleeWeaponPrefabInfoList.Count; k++) {
			if (meleeWeaponPrefabInfoList [k].Name.Equals (weaponName)) {
				return true;
			}
		}

		return false;
	}

	public grabPhysicalObjectMeleeAttackSystem getWeaponGrabbedByName (string weaponName)
	{
		for (int k = 0; k < meleeWeaponGrabbedInfoList.Count; k++) {
			if (meleeWeaponGrabbedInfoList [k].Name.Equals (weaponName)) {
				if (meleeWeaponGrabbedInfoList [k].weaponStored != null) {

					return meleeWeaponGrabbedInfoList [k].weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

				}
			}
		}

		return null;
	}

	public void checkWeaponToStore (string weaponName, GameObject weaponGameObject)
	{
		if (!storeEachGrabbedWeapon) {
			return;
		}

		int weaponIndex = meleeWeaponGrabbedInfoList.FindIndex (s => s.Name == weaponName);

		if (weaponIndex == -1) {
			meleeWeaponPrefabInfo currentMeleeWeaponPrefabInfo = getWeaponPrefabByName (weaponName);

			if (currentMeleeWeaponPrefabInfo != null) {
				meleeWeaponGrabbedInfo newMeleeWeaponGrabbedInfo = new meleeWeaponGrabbedInfo ();

				newMeleeWeaponGrabbedInfo.Name = weaponName;
				newMeleeWeaponGrabbedInfo.isCurrentWeapon = true;
				newMeleeWeaponGrabbedInfo.carryingWeapon = true;

				currentMeleeWeaponSheathedOrCarried = true;

				newMeleeWeaponGrabbedInfo.weaponPrefabIndex = currentMeleeWeaponPrefabInfo.weaponPrefabIndex;

				newMeleeWeaponGrabbedInfo.weaponStored = weaponGameObject;

				grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = newMeleeWeaponGrabbedInfo.weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

				bool hideWeaponMeshResult = checkIfHideWeaponMeshWhenNotUsed (currentGrabPhysicalObjectMeleeAttackSystem.hideWeaponMeshWhenNotUsed);

				newMeleeWeaponGrabbedInfo.hideWeaponMeshWhenNotUsed = hideWeaponMeshResult;

				newMeleeWeaponGrabbedInfo.weaponInstantiated = true;

				meleeWeaponGrabbedInfoList.Add (newMeleeWeaponGrabbedInfo);

				currentNumberOfWeaponsAvailable = meleeWeaponGrabbedInfoList.Count;

				currentWeaponIndex = meleeWeaponGrabbedInfoList.Count - 1;

				if (storeOnlyOneWeapon) {
					for (int i = meleeWeaponGrabbedInfoList.Count - 1; i >= 0; i--) {
						if (currentWeaponIndex != i) {
							meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [i];

							if (currentMeleeWeaponGrabbedInfo.weaponStored != null) {

								hideWeaponMeshResult = checkIfHideWeaponMeshWhenNotUsed (currentMeleeWeaponGrabbedInfo.hideWeaponMeshWhenNotUsed);

								if (!hideWeaponMeshResult) {
									checkObjectMeshToEnableOrDisable (false, currentMeleeWeaponGrabbedInfo);
								}

								currentMeleeWeaponGrabbedInfo.weaponStored.transform.position = mainGrabObjects.transform.position + mainGrabObjects.transform.up + mainGrabObjects.transform.forward;

								if (!currentMeleeWeaponGrabbedInfo.weaponStored.activeSelf) {
									currentMeleeWeaponGrabbedInfo.weaponStored.SetActive (true);
								}
							}

							meleeWeaponGrabbedInfoList.RemoveAt (i);
						} 
					} 

					currentNumberOfWeaponsAvailable = meleeWeaponGrabbedInfoList.Count;

					currentWeaponIndex = meleeWeaponGrabbedInfoList.Count - 1;
				}

				if (storePickedWeaponsOnInventory && !equipMeleeWeaponPaused) {
					if (showDebugPrint) {
						print ("ADD MELEE WEAPON TO INVENTORY " + weaponName);
					}

					mainInventoryManager.addObjectAmountToInventoryByName (weaponName, 1);

					if (!meleeWeaponsGrabbedManagerActive) {
						mainInventoryManager.checkQuickAccessSlotToSelectByName (weaponName);

//						if (useEventsOnMeleeWeaponEquipped) {
//							eventOnMeleeWeaponEquipped.Invoke ();
//						}
					}

					if (!mainGrabbedObjectMeleeAttackSystem.ignoreUseDrawKeepWeaponAnimation) {
						meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [currentWeaponIndex];

						activateDrawkeepMeleeWeaponAnimation (currentMeleeWeaponGrabbedInfo, false);
					}
				}

				equipMeleeWeaponPaused = false;
			} else {
				print ("WARNING: melee weapon prefab with the name " + weaponName + " not found, make sure to configure a weapon" +
				" with that info.");
			}
		} else {
			for (int k = 0; k < meleeWeaponGrabbedInfoList.Count; k++) {
				if (weaponIndex == k) {
					meleeWeaponGrabbedInfoList [k].isCurrentWeapon = true;
					meleeWeaponGrabbedInfoList [k].carryingWeapon = true;

					currentMeleeWeaponSheathedOrCarried = true;
				} else {
					meleeWeaponGrabbedInfoList [k].isCurrentWeapon = false;
					meleeWeaponGrabbedInfoList [k].carryingWeapon = false;
				}
			} 

			currentWeaponIndex = weaponIndex;

			meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [currentWeaponIndex];

			if (currentMeleeWeaponGrabbedInfo.weaponStored != weaponGameObject) {

				bool hideWeaponMeshResult = checkIfHideWeaponMeshWhenNotUsed (currentMeleeWeaponGrabbedInfo.hideWeaponMeshWhenNotUsed);

				if (!hideWeaponMeshResult) {
					checkObjectMeshToEnableOrDisable (false, currentMeleeWeaponGrabbedInfo);
				}

				currentMeleeWeaponGrabbedInfo.weaponStored.transform.position = mainGrabObjects.transform.position + mainGrabObjects.transform.up + mainGrabObjects.transform.forward;

				if (!currentMeleeWeaponGrabbedInfo.weaponStored.activeSelf) {
					currentMeleeWeaponGrabbedInfo.weaponStored.SetActive (true);
				}

				currentMeleeWeaponGrabbedInfo.weaponStored = weaponGameObject;
			}
		}
	}

	public void checkToDropWeaponFromList (string weaponName)
	{
		for (int k = 0; k < meleeWeaponGrabbedInfoList.Count; k++) {
			meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [k];

			if (currentMeleeWeaponGrabbedInfo.Name.Equals (weaponName)) {

				bool objectIsThrown = false;
				
				if (storePickedWeaponsOnInventory && mainInventoryManager != null) {
					if (currentMeleeWeaponGrabbedInfo.weaponStored != null) {
						grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = 
							currentMeleeWeaponGrabbedInfo.weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();
						
						if (currentGrabPhysicalObjectMeleeAttackSystem.isObjectThrown ()) {
							objectIsThrown = true;
						} else {
							if (currentMeleeWeaponGrabbedInfo.weaponStored.activeSelf) {
								currentMeleeWeaponGrabbedInfo.weaponStored.SetActive (false);
							}
						}
					}
				}

				if (currentMeleeWeaponGrabbedInfo.isCurrentWeapon) {
					currentMeleeWeaponSheathedOrCarried = false;
				}

				meleeWeaponGrabbedInfoList.RemoveAt (k);

				if (meleeWeaponGrabbedInfoList.Count == 0) {
					currentMeleeWeaponSheathedOrCarried = false;
				}

				if (storePickedWeaponsOnInventory && mainInventoryManager != null) {

					if (mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnAttackEnabled || mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnBlockEnabled) {
						if (currentMeleeWeaponGrabbedInfo.weaponStored != null) {
							grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = currentMeleeWeaponGrabbedInfo.weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

							if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
								lastDroppeWeaponDurabilityStored = true;

								lastDroppedWeaponName = currentMeleeWeaponGrabbedInfo.Name;

								lastDroppedWeaponDurabilityValue = currentGrabPhysicalObjectMeleeAttackSystem.getDurabilityAmount ();
							}
						}
					}

					if (objectIsThrown) {
						if (showDebugPrint) {
							print ("object is thrown, remove the weapon from inventory without instantiate a pickup");
						}

						mainInventoryManager.dropEquipByName (weaponName, 1, false, false);
					} else {
						if (showDebugPrint) {
							print ("object is not thrown, remove the weapon from inventory and instantiate a pickup");
						}

						bool objectLocatedOnInventory = false;

						if (showDebugPrint) {
							print (mainInventoryManager.getInventoryObjectAmountByName (weaponName));
						}

						int amountOfWeaponOnInventory = mainInventoryManager.getInventoryObjectAmountByName (weaponName);

						if (amountOfWeaponOnInventory >= 1) {
							objectLocatedOnInventory = true;
						}

						mainInventoryManager.dropEquipByName (weaponName, 1, true, false);

						if (!objectLocatedOnInventory) {
							if (!currentMeleeWeaponGrabbedInfo.weaponStored.activeSelf) {
								currentMeleeWeaponGrabbedInfo.weaponStored.SetActive (true);
							}
						}

						//Check here about unequipping melee weapons when carrying more of the same type
						if (amountOfWeaponOnInventory > 1) {
							mainInventoryManager.unEquipObjectByName (weaponName);
						}
					}
				}

				return;
			}
		} 
	}

	public void disableIsCurrentWeaponStateOnAllWeapons ()
	{
		for (int k = 0; k < meleeWeaponGrabbedInfoList.Count; k++) {
			meleeWeaponGrabbedInfoList [k].isCurrentWeapon = false;
		} 
	}

	public bool isGrabObjectsEnabled ()
	{
		return mainGrabObjects.isGrabObjectsEnabled ();
	}

	public void enableOrDisableMeleeWeaponMeshesOnCharacterBody (bool state)
	{
		if (state && !meleeWeaponsGrabbedManagerActive) {
			return;
		}

		for (int k = 0; k < meleeWeaponGrabbedInfoList.Count; k++) {
			if (!meleeWeaponGrabbedInfoList [k].objectThrown) {
				if (meleeWeaponGrabbedInfoList [k].weaponMesh != null) {
					if (meleeWeaponGrabbedInfoList [k].weaponMesh.activeSelf != state) {
						meleeWeaponGrabbedInfoList [k].weaponMesh.SetActive (state);

						if (showDebugPrint) {
							print (state);
						}
					}
				}
			}
		}
	}

	public void enableOrDisableMeleeWeaponMeshOnCharacterBodyByName (bool state, string weaponNameToSearch)
	{
		int weaponIndex = meleeWeaponGrabbedInfoList.FindIndex (s => s.Name == weaponNameToSearch);

		if (weaponIndex > -1) {
			if (meleeWeaponGrabbedInfoList [weaponIndex].weaponMesh != null) {
				if (meleeWeaponGrabbedInfoList [weaponIndex].weaponMesh.activeSelf != state) {
					meleeWeaponGrabbedInfoList [weaponIndex].weaponMesh.SetActive (state);

					if (showDebugPrint) {
						print (state);
					}
				}
			}
		}
	}

	public void enableOrDisableAllMeleeWeaponMeshesOnCharacterBody (bool state)
	{
		for (int k = 0; k < meleeWeaponGrabbedInfoList.Count; k++) {
			if (!meleeWeaponGrabbedInfoList [k].objectThrown) {
				if (meleeWeaponGrabbedInfoList [k].weaponMesh != null) {
					if (state) {
						if (!meleeWeaponGrabbedInfoList [k].isCurrentWeapon) {
							if (meleeWeaponGrabbedInfoList [k].weaponMesh.activeSelf != state) {
								meleeWeaponGrabbedInfoList [k].weaponMesh.SetActive (state);

								if (showDebugPrint) {
									print (state);
								}
							}
						}
					} else {
						if (meleeWeaponGrabbedInfoList [k].weaponMesh.activeSelf != state) {
							meleeWeaponGrabbedInfoList [k].weaponMesh.SetActive (state);

							if (showDebugPrint) {
								print (state);
							}
						}
					}
				}
			}
		}
	}

	public void enableOrDisableAllMeleeWeaponShieldMeshesOnCharacterBody (bool state)
	{
		for (int k = 0; k < shieldGrabbedInfoList.Count; k++) {
			if (shieldGrabbedInfoList [k].shieldStored != null) {
				if (state) {
					if (!shieldGrabbedInfoList [k].isCurrentShield) {
						if (shieldGrabbedInfoList [k].shieldStored.activeSelf != state) {
							shieldGrabbedInfoList [k].shieldStored.SetActive (state);

							if (showDebugPrint) {
								print (state);
							}
						}
					}
				} else {
					if (shieldGrabbedInfoList [k].shieldStored.activeSelf != state) {
						shieldGrabbedInfoList [k].shieldStored.SetActive (state);

						if (showDebugPrint) {
							print (state);
						}
					}
				}
			}
		}
	}

	public bool characterIsCarryingWeapon ()
	{
		return meleeWeaponsGrabbedManagerActive && mainGrabbedObjectMeleeAttackSystem.isCarryingObject ();
	}

	public float getLastTimeDrawMeleeWeapon ()
	{
		return mainGrabbedObjectMeleeAttackSystem.getLastTimeDrawMeleeWeapon ();
	}

	//INVENTORY FUNCTIONS
	public bool equipMeleeWeapon (string weaponNameToSearch, bool checkIfWeaponNotFound)
	{
		if (!mainGrabbedObjectMeleeAttackSystem.isGrabbedObjectMeleeAttackEnabled () || !mainGrabbedObjectMeleeAttackSystem.isCanGrabMeleeObjectsEnabled ()) {
			return false;
		}

		int weaponIndex = meleeWeaponGrabbedInfoList.FindIndex (s => s.Name == weaponNameToSearch);

		equipMeleeWeaponPaused = true;

		if (weaponIndex > -1) {
			meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [weaponIndex];

			if (currentMeleeWeaponGrabbedInfo.isCurrentWeapon) {
				return true;
			} else {
				if (mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {

				} else {
					checkWeaponByNumber (weaponIndex);

					if (!meleeWeaponsGrabbedManagerActive) {
//						mainInventoryManager.checkQuickAccessSlotToSelectByName (weaponNameToSearch);

//						if (useEventsOnMeleeWeaponEquipped) {
//							eventOnMeleeWeaponEquipped.Invoke ();
//						}
					}

					updateQuickAccesSlotOnInventory (weaponNameToSearch);

					return true;
				}
			}
		}

		bool weaponEquippedCorrectly = false;

		bool canSearchWeapon = true;

		if (showDebugPrint) {
			print (weaponNameToSearch);
		}

		weaponIndex = meleeWeaponGrabbedInfoList.FindIndex (s => s.Name == weaponNameToSearch);

		if (checkIfWeaponNotFound) {
			if (weaponIndex == -1) {
				canSearchWeapon = false;
			}
		}

		if (canSearchWeapon) {
			if (weaponIndex > -1 && meleeWeaponGrabbedInfoList [weaponIndex].weaponInstantiated) {
				if (showDebugPrint) {
					print ("check weapon by number");
				}

				weaponEquippedCorrectly = true;
			} else {
				if (showDebugPrint) {
					print ("instantiate new weapon");
				}

				int weaponPrefabIndex = meleeWeaponPrefabInfoList.FindIndex (s => s.Name == weaponNameToSearch);

				if (weaponPrefabIndex > -1) {
					meleeWeaponPrefabInfo currentMeleeWeaponPrefabInfo = meleeWeaponPrefabInfoList [weaponPrefabIndex];

					GameObject newWeaponToCarry = (GameObject)Instantiate (currentMeleeWeaponPrefabInfo.weaponPrefab, Vector3.up * 1000, Quaternion.identity);

					meleeWeaponGrabbedInfo newMeleeWeaponGrabbedInfo = new meleeWeaponGrabbedInfo ();
					newMeleeWeaponGrabbedInfo.Name = weaponNameToSearch;

					newMeleeWeaponGrabbedInfo.weaponPrefabIndex = currentMeleeWeaponPrefabInfo.weaponPrefabIndex;

					newMeleeWeaponGrabbedInfo.weaponStored = newWeaponToCarry;

					if (newMeleeWeaponGrabbedInfo.weaponStored.activeSelf) {
						newMeleeWeaponGrabbedInfo.weaponStored.SetActive (false);
					}

					newMeleeWeaponGrabbedInfo.weaponInstantiated = true;

					grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = newMeleeWeaponGrabbedInfo.weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

					bool hideWeaponMeshResult = checkIfHideWeaponMeshWhenNotUsed (currentGrabPhysicalObjectMeleeAttackSystem.hideWeaponMeshWhenNotUsed);

					newMeleeWeaponGrabbedInfo.hideWeaponMeshWhenNotUsed = hideWeaponMeshResult;

					meleeWeaponGrabbedInfoList.Add (newMeleeWeaponGrabbedInfo);

					weaponEquippedCorrectly = true;
				}
			}
		}
			

		equipMeleeWeaponPaused = false;

		if (weaponEquippedCorrectly) {

			updateQuickAccesSlotOnInventory (weaponNameToSearch);
		}

		return weaponEquippedCorrectly;
	}

	void updateQuickAccesSlotOnInventory (string weaponNameToSearch)
	{
		if (storePickedWeaponsOnInventory) {
			mainInventoryManager.showWeaponSlotsParentWhenWeaponSelectedByName (weaponNameToSearch);
		}
	}

	public bool unEquipMeleeWeapon (string weaponNameToSearch, bool dropWeaponObject)
	{
		int weaponIndex = meleeWeaponGrabbedInfoList.FindIndex (s => s.Name == weaponNameToSearch);

		if (weaponIndex > -1) {
			meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [weaponIndex];

			if (currentMeleeWeaponGrabbedInfo.weaponStored != null) {
				if (currentMeleeWeaponGrabbedInfo.isCurrentWeapon) {
					mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (false);

					mainGrabObjects.grabbed = true;

					mainGrabObjects.checkIfDropObject (currentMeleeWeaponGrabbedInfo.weaponStored);
				} else {
					currentMeleeWeaponGrabbedInfo.weaponStored.transform.position = mainGrabObjects.transform.position + mainGrabObjects.transform.up + mainGrabObjects.transform.forward;
				}

				bool hideWeaponMeshResult = checkIfHideWeaponMeshWhenNotUsed (currentMeleeWeaponGrabbedInfo.hideWeaponMeshWhenNotUsed);

				if (!hideWeaponMeshResult) {
					checkObjectMeshToEnableOrDisable (false, currentMeleeWeaponGrabbedInfo);
				}

				mainGrabObjects.removeCurrentPhysicalObjectToGrabFound (currentMeleeWeaponGrabbedInfo.weaponStored);

				if (currentMeleeWeaponGrabbedInfo.weaponStored.activeSelf != dropWeaponObject) {
					currentMeleeWeaponGrabbedInfo.weaponStored.SetActive (dropWeaponObject);
				}

				mainGrabbedObjectMeleeAttackSystem.setRemoveWeaponsFromManagerState (true);
			}

			currentMeleeWeaponGrabbedInfo.isCurrentWeapon = false;

			currentMeleeWeaponGrabbedInfo.carryingWeapon = false;

			currentMeleeWeaponSheathedOrCarried = false;

			meleeWeaponGrabbedInfoList.RemoveAt (weaponIndex);

			currentNumberOfWeaponsAvailable = meleeWeaponGrabbedInfoList.Count;

			currentWeaponIndex = meleeWeaponGrabbedInfoList.Count - 1;

			if (currentWeaponIndex < 0) {
				currentWeaponIndex = 0;
			}

			return true;
		}

		return false;
	}

	public bool checkWeaponToSelectOnQuickAccessSlots (string weaponName)
	{
		int weaponIndex = meleeWeaponGrabbedInfoList.FindIndex (s => s.Name == weaponName);

		if (weaponIndex > -1) {
			meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [weaponIndex];

			checkWeaponByNumber (weaponIndex);

			mainGrabbedObjectMeleeAttackSystem.checkEventWhenKeepingOrDrawingMeleeWeapon (false);

			if (currentMeleeWeaponGrabbedInfo.isCurrentWeapon) {
				return true;
			}
		}

		return false;
	}

	public bool isMeleeWeaponsGrabbedManagerActive ()
	{
		return meleeWeaponsGrabbedManagerActive;
	}

	public string getCurrentWeaponName ()
	{
		for (int k = 0; k < meleeWeaponGrabbedInfoList.Count; k++) {
			if (meleeWeaponGrabbedInfoList [k].isCurrentWeapon) {
				return meleeWeaponGrabbedInfoList [k].Name;
			}
		} 

		return "";
	}

	public GameObject getCurrentWeaponMeshByName (string weaponName)
	{
		for (int k = 0; k < meleeWeaponGrabbedInfoList.Count; k++) {
			if (meleeWeaponGrabbedInfoList [k].Name.Equals (weaponName)) {
				return meleeWeaponGrabbedInfoList [k].weaponMesh;
			}
		} 

		return null;
	}

	public bool isWeaponInGrabbedInfoList (string weaponName)
	{
		int weaponIndex = meleeWeaponGrabbedInfoList.FindIndex (s => s.Name == weaponName);

		if (weaponIndex > -1) {
			return true;
		}

		return false;
	}

	public string getCurrentWeaponActiveName ()
	{
		if (meleeWeaponGrabbedInfoList.Count > 0 && currentWeaponIndex < meleeWeaponGrabbedInfoList.Count) {
			return meleeWeaponGrabbedInfoList [currentWeaponIndex].Name;
		}

		return "";
	}

	public bool equipShield (string shieldName)
	{
		return setShieldActiveState (true, shieldName);
	}

	public bool unequipeShield (string shieldName)
	{
		bool unequipResult = setShieldActiveState (false, shieldName);

		int shieldInfoIndex = shieldGrabbedInfoList.FindIndex (s => s.Name == shieldName);

		if (shieldInfoIndex > -1) {
			if (shieldGrabbedInfoList [shieldInfoIndex].shieldStored != null) {
				Destroy (shieldGrabbedInfoList [shieldInfoIndex].shieldStored);
			}

			shieldGrabbedInfoList.RemoveAt (shieldInfoIndex);

			string emptyShieldWeaponName = mainGrabbedObjectMeleeAttackSystem.getEmptyWeaponToUseOnlyShield ();

			int weaponIndex = meleeWeaponGrabbedInfoList.FindIndex (s => s.Name == emptyShieldWeaponName);

			if (weaponIndex > -1) {
				if (meleeWeaponGrabbedInfoList [weaponIndex].weaponStored != null) {
					Destroy (meleeWeaponGrabbedInfoList [weaponIndex].weaponStored);
				}

				meleeWeaponGrabbedInfoList.RemoveAt (weaponIndex);
			}
		}

		return unequipResult;
	}
	//END INVENTORY FUNCTIONS


	//START SHIELD FUNCTIONS
	public bool setShieldActiveState (bool state, string shieldName)
	{
		print (shieldName + " " + state);

		int shieldInfoIndex = shieldGrabbedInfoList.FindIndex (s => s.Name == shieldName);

		int shieldPrefabIndex = shieldPrefabInfoList.FindIndex (s => s.Name == shieldName);

		if (shieldInfoIndex == -1) {
			if (shieldPrefabIndex > -1) {

				shieldPrefabInfo currentShieldPrefabInfo = shieldPrefabInfoList [shieldPrefabIndex];

				GameObject newWeaponToCarry = (GameObject)Instantiate (currentShieldPrefabInfo.shieldPrefab, Vector3.up * 1000, Quaternion.identity);

				shieldGrabbedInfo newShieldGrabbedInfo = new shieldGrabbedInfo ();
				newShieldGrabbedInfo.Name = shieldName;

				newShieldGrabbedInfo.shieldInstantiated = true;

				newShieldGrabbedInfo.shieldPrefabIndex = currentShieldPrefabInfo.shieldPrefabIndex;

				newShieldGrabbedInfo.shieldStored = newWeaponToCarry;

				newShieldGrabbedInfo.equipShieldWhenPickedIfNotShieldEquippedPrevioulsy = currentShieldPrefabInfo.equipShieldWhenPickedIfNotShieldEquippedPrevioulsy;

				shieldGrabbedInfoList.Add (newShieldGrabbedInfo);

				shieldInfoIndex = shieldGrabbedInfoList.Count - 1;
			}
		}

		if (shieldInfoIndex > -1) {
			for (int k = 0; k < shieldGrabbedInfoList.Count; k++) {
				if (k == shieldInfoIndex) {
					currentShieldGrabbedInfo = shieldGrabbedInfoList [k];

					if (!currentShieldGrabbedInfo.isCurrentShield) {
						if (currentShieldGrabbedInfo.useEventsOnEquipShieldChangeState) {
							if (state) {
								currentShieldGrabbedInfo.eventOnUnequippShield.Invoke ();
							} else {
								currentShieldGrabbedInfo.eventOnEquipShield.Invoke ();
							}
						}
					}

					currentShieldGrabbedInfo.isCurrentShield = true;

					if (currentShieldGrabbedInfo.shieldStored.activeSelf != state) {
						currentShieldGrabbedInfo.shieldStored.SetActive (state);
					}
				} else {
					if (shieldGrabbedInfoList [k].isCurrentShield) {
						if (shieldGrabbedInfoList [k].useEventsOnEquipShieldChangeState) {
							shieldGrabbedInfoList [k].eventOnUnequippShield.Invoke ();
						}
					}

					shieldGrabbedInfoList [k].isCurrentShield = false;

					if (shieldGrabbedInfoList [k].shieldStored.activeSelf) {
						shieldGrabbedInfoList [k].shieldStored.SetActive (false);
					}
				}
			}

			mainGrabbedObjectMeleeAttackSystem.setShieldInfo (currentShieldGrabbedInfo.Name, currentShieldGrabbedInfo.shieldStored, 
				shieldPrefabInfoList [shieldPrefabIndex].shieldHandMountPointTransformReference, shieldPrefabInfoList [shieldPrefabIndex].shieldBackMountPointTransformReference, state);

			mainGrabbedObjectMeleeAttackSystem.setShieldActiveState (state);

			return true;
		}

		return false;
	}

	public void checkEquipShieldIfNotCarryingPreviously ()
	{
		if (!mainGrabbedObjectMeleeAttackSystem.carryingShield &&
		    (meleeWeaponsGrabbedManagerActive || mainGrabbedObjectMeleeAttackSystem.shieldCanBeUsedWithoutMeleeWeapon)) {

			string lastInventoryObjectPickedName = mainInventoryManager.getLastInventoryObjectPickedName ();

			if (lastInventoryObjectPickedName != "") {
				for (int k = 0; k < shieldPrefabInfoList.Count; k++) {
					if (shieldPrefabInfoList [k].equipShieldWhenPickedIfNotShieldEquippedPrevioulsy &&
					    shieldPrefabInfoList [k].Name.Equals (lastInventoryObjectPickedName)) {

						mainInventoryManager.equipObjectByName (lastInventoryObjectPickedName);

						return;
					}
				}
			}
		}
	}

	public void toggleDrawOrSheatheShield (string shieldName)
	{
		int shieldInfoIndex = shieldGrabbedInfoList.FindIndex (s => s.Name == shieldName);

		bool shieldState = false;

		if (shieldInfoIndex > -1) {
//			if (meleeWeaponsGrabbedManagerActive) {
			if (shieldGrabbedInfoList [shieldInfoIndex].isCurrentShield) {
				shieldState = !mainGrabbedObjectMeleeAttackSystem.shieldActive;
			} else {
				shieldState = true;
			} 
//			} else {
//				print ("trying to draw the shield when not melee mode active, sending signal to quick access slots");
//
//				mainInventoryManager.changeToMeleeWeapons (mainGrabbedObjectMeleeAttackSystem.getEmptyWeaponToUseOnlyShield ());
//			}
		} else {
			shieldState = true;
		}

		if (shieldState) {
			currentMeleeWeaponSheathedOrCarried = false;
		}

		drawOrSheatheShield (shieldState, shieldName);

//		print (shieldState + " " + shieldName);
	}

	public void drawOrSheatheShield (bool state, string shieldName)
	{
		setShieldActiveState (true, shieldName);

		mainGrabbedObjectMeleeAttackSystem.drawOrSheatheShield (state);

		if (state) {
			if (!mainGrabbedObjectMeleeAttackSystem.shieldActive && mainGrabbedObjectMeleeAttackSystem.carryingShield) {
				mainGrabbedObjectMeleeAttackSystem.setShieldParentState (false);
			}
		} else {
			mainGrabbedObjectMeleeAttackSystem.setShieldActiveFieldValueDirectly (false);
		}
	}
	//END SHIELD FUNCTIONS

	public void checkToKeepWeapon ()
	{
		keepWeaponExternally (true);
	}

	public void checkToKeepWeaponWithoutCheckingInputActive ()
	{
		keepWeaponExternally (false);
	}

	void keepWeaponExternally (bool checkIfInputActive)
	{
		if (!meleeWeaponsGrabbedManagerActive) {
			return;
		}

		if (!isGrabObjectsEnabled ()) {
			return;
		}

		if ((mainGrabbedObjectMeleeAttackSystem.canUseWeaponsInput () || !checkIfInputActive) &&
		    !mainGrabbedObjectMeleeAttackSystem.isObjectThrownTravellingToTarget ()) {
			if (mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
				keepWeapon (currentWeaponIndex);
			}
		}
	}

	public void setStartGameWithWeaponState (bool state)
	{
		startGameWithWeapon = state;
	}

	public void setStartGameWithWeaponStateFromEditor (bool state)
	{
		setStartGameWithWeaponState (state);

		updateComponent ();
	}

	public void setWeaponNameToStartGame (string newName)
	{
		weaponNameToStartGame = newName;
	}

	public void updateDurabilityAmountStateOnAllObjects ()
	{
		if (mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnAttackEnabled || mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnBlockEnabled) {
			for (int i = 0; i < meleeWeaponGrabbedInfoList.Count; i++) {
				if (meleeWeaponGrabbedInfoList [i].weaponStored != null) {
					grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = meleeWeaponGrabbedInfoList [i].weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

					if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
						currentGrabPhysicalObjectMeleeAttackSystem.updateDurabilityAmountState ();
					}
				}
			}
		}
	}

	public void updateDurabilityAmountStateOnObjectByName (string weaponName)
	{
		if (mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnAttackEnabled || mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnBlockEnabled) {
			for (int i = 0; i < meleeWeaponGrabbedInfoList.Count; i++) {
				if (meleeWeaponGrabbedInfoList [i].Name.Equals (weaponName) && meleeWeaponGrabbedInfoList [i].weaponStored != null) {
					grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = meleeWeaponGrabbedInfoList [i].weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

					if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
						currentGrabPhysicalObjectMeleeAttackSystem.updateDurabilityAmountState ();
					}

					return;
				}
			}
		}
	}

	public float getDurabilityAmountStateOnObjectByName (string weaponName)
	{
		if (mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnAttackEnabled || mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnBlockEnabled) {
			if (lastDroppeWeaponDurabilityStored) {
				lastDroppeWeaponDurabilityStored = false;

				if (lastDroppedWeaponName.Equals (weaponName)) {
					return lastDroppedWeaponDurabilityValue;
				}
			}
			
			for (int i = 0; i < meleeWeaponGrabbedInfoList.Count; i++) {
				if (meleeWeaponGrabbedInfoList [i].Name.Equals (weaponName) && meleeWeaponGrabbedInfoList [i].weaponStored != null) {
					grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = meleeWeaponGrabbedInfoList [i].weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

					if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
						return currentGrabPhysicalObjectMeleeAttackSystem.getDurabilityAmount ();
					}
				}
			}
		}

		return -1;
	}

	public void initializeDurabilityValue (float newAmount, string weaponName)
	{
		if (mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnAttackEnabled || mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnBlockEnabled) {
			for (int i = 0; i < meleeWeaponGrabbedInfoList.Count; i++) {
				if (meleeWeaponGrabbedInfoList [i].Name.Equals (weaponName) && meleeWeaponGrabbedInfoList [i].weaponStored != null) {
					grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = meleeWeaponGrabbedInfoList [i].weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

					if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
						currentGrabPhysicalObjectMeleeAttackSystem.initializeDurabilityValue (newAmount);
					}

					return;
				}
			}
		}
	}

	public void repairObjectFully (string weaponName)
	{
		if (mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnAttackEnabled || mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnBlockEnabled) {
			for (int i = 0; i < meleeWeaponGrabbedInfoList.Count; i++) {
				if (meleeWeaponGrabbedInfoList [i].Name.Equals (weaponName) && meleeWeaponGrabbedInfoList [i].weaponStored != null) {
					grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = meleeWeaponGrabbedInfoList [i].weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

					if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
						currentGrabPhysicalObjectMeleeAttackSystem.repairObjectFully ();
					}

					return;
				}
			}
		}
	}

	public void breakFullDurabilityOnCurrentWeapon ()
	{
		if (!characterIsCarryingWeapon ()) {
			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnAttackEnabled || mainGrabbedObjectMeleeAttackSystem.checkDurabilityOnBlockEnabled) {
			grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = mainGrabbedObjectMeleeAttackSystem.getCurrentGrabPhysicalObjectMeleeAttackSystem ();

			if (currentGrabPhysicalObjectMeleeAttackSystem != null) {
				currentGrabPhysicalObjectMeleeAttackSystem.breakFullDurabilityOnCurrentWeapon ();
			}
		}
	}

	//EDITOR FUNCTIONS
	public void setWeaponNameToStartGameFromEditor (string newName)
	{
		setWeaponNameToStartGame (newName);

		updateComponent ();
	}

	public void addNewMeleeWeaponPrefab (GameObject newWeaponPrefab, string newWeaponName)
	{
		if (newWeaponPrefab != null) {
			int weaponPrefabIndex = meleeWeaponPrefabInfoList.FindIndex (s => s.Name == newWeaponName);

			if (weaponPrefabIndex < 0) {
				
				meleeWeaponPrefabInfo newMeleeWeaponPrefabInfo = new meleeWeaponPrefabInfo ();

				newMeleeWeaponPrefabInfo.Name = newWeaponName;

				newMeleeWeaponPrefabInfo.weaponPrefab = newWeaponPrefab;

				newMeleeWeaponPrefabInfo.weaponPrefabIndex = meleeWeaponPrefabInfoList.Count;

				meleeWeaponPrefabInfoList.Add (newMeleeWeaponPrefabInfo);

				updateComponent ();
			} else {
				meleeWeaponPrefabInfo newMeleeWeaponPrefabInfo = meleeWeaponPrefabInfoList [weaponPrefabIndex];

				if (newMeleeWeaponPrefabInfo.weaponPrefab == null) {
					newMeleeWeaponPrefabInfo.weaponPrefab = newWeaponPrefab;

					updateComponent ();
				}
			}
		}
	}

	public void addNewMeleeShieldPrefab (GameObject newShieldPrefab, string newShieldName)
	{
		if (newShieldPrefab != null) {
			shieldPrefabInfo newShieldPrefabInfo = new shieldPrefabInfo ();

			newShieldPrefabInfo.Name = newShieldName;

			newShieldPrefabInfo.shieldPrefab = newShieldPrefab;

			newShieldPrefabInfo.shieldPrefabIndex = shieldPrefabInfoList.Count + 1;

			if (shieldPrefabInfoList.Count > 0) {
				newShieldPrefabInfo.shieldBackMountPointTransformReference = shieldPrefabInfoList [0].shieldBackMountPointTransformReference;
				newShieldPrefabInfo.shieldHandMountPointTransformReference = shieldPrefabInfoList [0].shieldHandMountPointTransformReference;
			}

			shieldPrefabInfoList.Add (newShieldPrefabInfo);

			updateComponent ();
		}
	}


	//INPUT FUNCTIONS
	bool canUseInput ()
	{
		if (mainGrabbedObjectMeleeAttackSystem.mainPlayerController.iscloseCombatAttackInProcess ()) {
			return false;
		}

		if (!mainGrabbedObjectMeleeAttackSystem.mainPlayerController.isActionActive ()) {
			return false;
		}

		return true;
	}

	public void inputDrawOrKeepMeleeWeapon ()
	{
		drawOrKeepMeleeWeapon (true);
	}

	public void drawOrKeepMeleeWeaponWithoutCheckingInputActive ()
	{
		drawOrKeepMeleeWeapon (false);
	}

	public void drawMeleeWeaponGrabbedCheckingAnimationDelay ()
	{
		bool activateDrawCoroutineResult = false;

		if (currentWeaponIndex < meleeWeaponGrabbedInfoList.Count && currentWeaponIndex >= 0) {
			meleeWeaponGrabbedInfo currentMeleeWeaponGrabbedInfo = meleeWeaponGrabbedInfoList [currentWeaponIndex];

			if (currentMeleeWeaponGrabbedInfo.weaponStored != null) {
				grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = currentMeleeWeaponGrabbedInfo.weaponStored.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

				if (currentGrabPhysicalObjectMeleeAttackSystem.useDrawKeepWeaponAnimation) {
					StartCoroutine (drawMeleeWeaponGrabbedCheckingAnimationDelayCoroutine ());

					activateDrawCoroutineResult = true;
				}
			}
		}

		if (!activateDrawCoroutineResult) {
			drawOrKeepMeleeWeapon (false);
		}
	}

	IEnumerator drawMeleeWeaponGrabbedCheckingAnimationDelayCoroutine ()
	{
		yield return new WaitForSeconds (0.2f);

		drawOrKeepMeleeWeapon (false);
	}

	void drawOrKeepMeleeWeapon (bool checkIfInputActive)
	{
		if (!meleeWeaponsGrabbedManagerActive) {
			return;
		}

		if (!isGrabObjectsEnabled ()) {
			return;
		}

		if (mainGrabbedObjectMeleeAttackSystem.isAttackInProcess ()) {
			return;
		}

		if ((mainGrabbedObjectMeleeAttackSystem.canUseWeaponsInput () || !checkIfInputActive) &&
		    !mainGrabbedObjectMeleeAttackSystem.isObjectThrownTravellingToTarget ()) {
			if (mainGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
				keepWeapon (currentWeaponIndex);
			} else {
				checkIfDrawWeapon ();
			}
		}
	}

	public int getCurrentNumberOfWeaponsAvailable ()
	{
		currentNumberOfWeaponsAvailable = meleeWeaponGrabbedInfoList.Count;

		return currentNumberOfWeaponsAvailable;
	}

	public void checkEventOnWeaponStolen ()
	{
		if (useEventOnWeaponStolen) {
			eventOnWeaponStolen.Invoke ();
		}
	}

	//EDITOR FUNCTIONS
	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Add Shield", gameObject);
	}

	[System.Serializable]
	public class meleeWeaponGrabbedInfo
	{
		public string Name;
		public bool weaponInstantiated;
		public GameObject weaponStored;
		public bool canBeSpawnedInfiniteTimes;
		public bool isCurrentWeapon;
		public bool carryingWeapon;
		public int weaponPrefabIndex;
		public bool hideWeaponMeshWhenNotUsed;

		public bool objectThrown;

		public GameObject weaponMesh;
	}

	[System.Serializable]
	public class meleeWeaponPrefabInfo
	{
		public string Name;
		public GameObject weaponPrefab;
		public int weaponPrefabIndex;
	}

	[System.Serializable]
	public class shieldPrefabInfo
	{
		public string Name;
		public GameObject shieldPrefab;
		public Transform shieldHandMountPointTransformReference;
		public Transform shieldBackMountPointTransformReference;
		public int shieldPrefabIndex;

		public bool equipShieldWhenPickedIfNotShieldEquippedPrevioulsy;
	}

	[System.Serializable]
	public class shieldGrabbedInfo
	{
		public string Name;
		public bool shieldInstantiated;
		public GameObject shieldStored;

		public bool isCurrentShield;
		public int shieldPrefabIndex;
		public bool hideWeaponMeshWhenNotUsed;

		public bool equipShieldWhenPickedIfNotShieldEquippedPrevioulsy;

		public bool useEventsOnEquipShieldChangeState;
		public UnityEvent eventOnEquipShield;
		public UnityEvent eventOnUnequippShield;
	}
}
