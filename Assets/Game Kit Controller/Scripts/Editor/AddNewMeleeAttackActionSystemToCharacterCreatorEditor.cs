using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class AddNewMeleeAttackActionSystemToCharacterCreatorEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();
	Event currentEvent;

	Vector2 rectSize = new Vector2 (660, 600);

	bool actionAdded;

	float timeToBuild = 0.2f;
	float timer;

	float minHeight = 600f;

	string prefabsPath = "";

	playerActionSystem currentPlayerActionSystem;

	grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;

	public string actionSystemCategoryName = "Melee Combat";

	public string actionSystemName;
	public float actionSystemDuration;
	public float actionSystemSpeed = 1;
	public bool useActionSystemID;
	public int actionSystemID;
	public string actionSystemAnimationName;

	public string attackType = "Regular";

	public float attackDamage;

	public bool useCustomPrefabPath;
	public string customPrefabPath;

	public GameObject currentActionSystemPrefab;

	public GameObject mainGrabPhysicalObjectMeleeAttackSystemGameObject;

	public grabPhysicalObjectMeleeAttackSystem mainGrabPhysicalObjectMeleeAttackSystem;

	public meleeWeaponAttackInfo mainMeleeWeaponAttackInfo;

	public bool useMeleeWeaponAttackInfoTemplate;

	public bool addNewCustomMeleeAttack;

	public List<grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo> damageTriggerActiveInfoList = new List<grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo> ();

	grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo currentDamageTriggerActiveInfo;

	public bool editSingleMeleeAttackInfo;

	public string[] attackList;
	public int attackListIndex;

	bool attackListAssigned;

	bool attackInfoAssigned;

	string originalSingleAttackName;


	bool actionSystemAssigned;

	GUIStyle style = new GUIStyle ();

	GUIStyle labelStyle = new GUIStyle ();

	float windowHeightPercentage = 0.6f;

	Vector2 screenResolution;

	Vector2 scrollPos1;

	float maxLayoutWidht = 220;

	Vector2 previousRectSize;

	[MenuItem ("Game Kit Controller/Add Melee Attack Single or List To Character", false, 26)]
	public static void addMeleeAttackActionSystemToCharacter ()
	{
		GetWindow<AddNewMeleeAttackActionSystemToCharacterCreatorEditor> ();
	}

	void OnEnable ()
	{
		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		//		Debug.Log (screenResolution + " " + partsHeight + " " + settingsHeight + " " + previewHeight);

		float totalHeight = screenResolution.y * windowHeightPercentage;

		totalHeight = Mathf.Clamp (totalHeight, minHeight, screenResolution.y);

		rectSize = new Vector2 (660, totalHeight);


		prefabsPath = pathInfoValues.getMeleeActionSystemPrefabPath ();

		customPrefabPath = prefabsPath;

		resetCreatorValues ();

		checkCurrentObjectSelected (Selection.activeGameObject);
	}

	void checkCurrentObjectSelected (GameObject currentCharacterSelected)
	{
		if (currentCharacterSelected != null) {
			playerComponentsManager currentPlayerComponentsManager = currentCharacterSelected.GetComponentInChildren<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				currentPlayerActionSystem = currentPlayerComponentsManager.getPlayerActionSystem ();

				if (currentPlayerActionSystem != null) {
					actionSystemAssigned = true;

					currentActionSystemPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath (prefabsPath, typeof(GameObject)) as GameObject;
				}

				mainGrabbedObjectMeleeAttackSystem = currentPlayerComponentsManager.getGrabbedObjectMeleeAttackSystem ();
			}
		}
	}

	void OnDisable ()
	{
		resetCreatorValues ();
	}

	void resetCreatorValues ()
	{
		if (actionAdded) {

		} else {

		}

		currentPlayerActionSystem = null;

		actionAdded = false;

		actionSystemAssigned = false;

		actionSystemCategoryName = "Melee Combat";

		actionSystemName = "";
		actionSystemDuration = 0;
		actionSystemSpeed = 1;
		useActionSystemID = false;
		actionSystemID = 0;
		actionSystemAnimationName = "";

		useCustomPrefabPath = false;

		useMeleeWeaponAttackInfoTemplate = false;

		addNewCustomMeleeAttack = false;

		damageTriggerActiveInfoList.Clear ();

		editSingleMeleeAttackInfo = false;

		attackListAssigned = false;

		attackInfoAssigned = false;


		Debug.Log ("Action window closed");
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Melee", null, "Add Melee Attack List To Character");

		GUILayout.BeginVertical ("Add Melee Attack List To Character", "window");

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		windowRect = GUILayoutUtility.GetLastRect ();
//		windowRect.position = new Vector2 (0, windowRect.position.y);
		windowRect.width = this.maxSize.x;

		GUILayout.BeginHorizontal ();

		EditorGUILayout.HelpBox ("", MessageType.Info);

		style = new GUIStyle (EditorStyles.helpBox);
		style.richText = true;

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 17;

		EditorGUILayout.Space ();

		if (addNewCustomMeleeAttack) {
			EditorGUILayout.LabelField ("Configure the settings of the melee weapon, " +
			"with the info of a new melee attack that will be included on the weapon it self.\n" +
			"Remember to set the damage trigger values using the % value (from 0 to 1) of the attack animation to set the for each swing.\n\n" +
			"Once the weapon info is configured and created, remember to go to the character animator and configure the animation state on it, " +
			"along its transitions and conditions values.", style);
		} else {
			EditorGUILayout.LabelField ("Configure the settings of the melee weapon, " +
			"to use its attack list and press the button 'Add Melee Attack List To Character'. \n\n" +
			"If not character is selected in the hierarchy, select one and press the button 'Check Current Object Selected'.\n\n" +
			"Once the weapon info is configured and created, remember to go to the character animator and configure the animation state on it, " +
			"along its transitions and conditions values.", style);
		}

		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Window Height", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));

		if (previousRectSize != rectSize) {
			previousRectSize = rectSize;

			this.maxSize = rectSize;
		}

		rectSize.y = EditorGUILayout.Slider (rectSize.y, minHeight, screenResolution.y, GUILayout.ExpandWidth (true));

		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (currentPlayerActionSystem == null) {

			GUILayout.FlexibleSpace ();

			GUILayout.BeginHorizontal ();
			EditorGUILayout.HelpBox ("", MessageType.Warning);

			style = new GUIStyle (EditorStyles.helpBox);
			style.richText = true;

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 17;
			EditorGUILayout.LabelField ("WARNING: No Character was found, make sure to select the player or an " +
			"humanoid AI to add an action to it.", style);

			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Check Current Object Selected")) {
				checkCurrentObjectSelected (Selection.activeGameObject);
			}
		} else {
			if (actionSystemAssigned) {

				scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

				GUILayout.FlexibleSpace ();

				EditorGUILayout.Space ();

				labelStyle.fontStyle = FontStyle.Bold;

				EditorGUILayout.LabelField ("MELEE WEAPON ATTACK LIST NFO", labelStyle);

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Action Category Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				actionSystemCategoryName = (string)EditorGUILayout.TextField ("", actionSystemCategoryName);
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Add New Custom Melee Attack", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				addNewCustomMeleeAttack = (bool)EditorGUILayout.Toggle (addNewCustomMeleeAttack);
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Edit Single Melee Attack Info", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				editSingleMeleeAttackInfo = (bool)EditorGUILayout.Toggle (editSingleMeleeAttackInfo);
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				if (editSingleMeleeAttackInfo) {

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Melee Weapon Object", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					mainGrabPhysicalObjectMeleeAttackSystemGameObject = EditorGUILayout.ObjectField (mainGrabPhysicalObjectMeleeAttackSystemGameObject, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Get Melee Weapon Attack List")) {
						attackListAssigned = false;

						attackInfoAssigned = false;
					}

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Get Melee Weapon Attack Info")) {
						if (attackListAssigned) {
							grabPhysicalObjectMeleeAttackSystem.attackInfo currentAttackInfo = mainGrabPhysicalObjectMeleeAttackSystem.attackInfoList [attackListIndex];

							attackDamage = currentAttackInfo.attackDamage;

							attackType = currentAttackInfo.attackType;

							actionSystemAnimationName = currentAttackInfo.customActionName;

							actionSystemDuration = currentAttackInfo.attackDuration;

							actionSystemDuration = currentAttackInfo.attackDurationOnAir;

							actionSystemSpeed = currentAttackInfo.attackAnimationSpeed;

							actionSystemSpeed = currentAttackInfo.attackAnimationSpeedOnAir;

							originalSingleAttackName = actionSystemAnimationName;

							damageTriggerActiveInfoList.Clear ();

							for (int i = 0; i < currentAttackInfo.damageTriggerActiveInfoList.Count; i++) {
								grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo newTriggerInfo = new grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo ();

								newTriggerInfo.delayToActiveTrigger = currentAttackInfo.damageTriggerActiveInfoList [i].delayToActiveTrigger;
								newTriggerInfo.activateDamageTrigger = currentAttackInfo.damageTriggerActiveInfoList [i].activateDamageTrigger;

								damageTriggerActiveInfoList.Add (newTriggerInfo);
							}

							attackInfoAssigned = true;
						}
					}

					EditorGUILayout.Space ();

					if (!attackListAssigned) {
						if (mainGrabPhysicalObjectMeleeAttackSystemGameObject != null) {
							mainGrabPhysicalObjectMeleeAttackSystem = mainGrabPhysicalObjectMeleeAttackSystemGameObject.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

							if (mainGrabPhysicalObjectMeleeAttackSystem != null) {
					
								attackList = new string[mainGrabPhysicalObjectMeleeAttackSystem.attackInfoList.Count];

								int currentIndex = 0;

								for (int i = 0; i < mainGrabPhysicalObjectMeleeAttackSystem.attackInfoList.Count; i++) { 
									string currentAttackName = mainGrabPhysicalObjectMeleeAttackSystem.attackInfoList [i].customActionName;
									attackList [currentIndex] = currentAttackName;
									currentIndex++;
								}

								attackListAssigned = true;
							}
						}
					} else {
						if (attackList.Length > 0) {

							EditorGUILayout.Space ();

							if (attackListIndex < attackList.Length) {
								GUILayout.BeginHorizontal ();
								GUILayout.Label ("Attack Info", EditorStyles.boldLabel, GUILayout.MaxWidth (150));
								attackListIndex = EditorGUILayout.Popup (attackListIndex, attackList);
							  
								GUILayout.EndHorizontal ();

								EditorGUILayout.Space ();
							}
						}
					}

					if (attackInfoAssigned) {
						GUILayout.Label ("MELEE ATTACK INFO", EditorStyles.boldLabel);

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Attack Animation Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						actionSystemAnimationName = (string)EditorGUILayout.TextField ("", actionSystemAnimationName);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Attack Type", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						attackType = (string)EditorGUILayout.TextField ("", attackType);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Attack Damage", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						attackDamage = (float)EditorGUILayout.FloatField (attackDamage);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Attack Duration", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						actionSystemDuration = (float)EditorGUILayout.FloatField (actionSystemDuration);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Attack Speed", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						actionSystemSpeed = (float)EditorGUILayout.FloatField (actionSystemSpeed);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						EditorGUILayout.Space ();

						GUILayout.Label ("Number of Elements " + (damageTriggerActiveInfoList.Count).ToString (), EditorStyles.boldLabel);

						EditorGUILayout.Space ();

						for (int i = 0; i < damageTriggerActiveInfoList.Count; i++) { 

							currentDamageTriggerActiveInfo = damageTriggerActiveInfoList [i];

							GUILayout.BeginVertical ("box");

							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Delay To Activate Damage Trigger", EditorStyles.boldLabel);
							currentDamageTriggerActiveInfo.delayToActiveTrigger = (float)EditorGUILayout.FloatField (currentDamageTriggerActiveInfo.delayToActiveTrigger);
							GUILayout.EndHorizontal ();

							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Activate Damage Trigger State", EditorStyles.boldLabel);
							currentDamageTriggerActiveInfo.activateDamageTrigger = (bool)EditorGUILayout.Toggle (currentDamageTriggerActiveInfo.activateDamageTrigger);
							GUILayout.EndHorizontal ();

							GUILayout.EndVertical ();

							EditorGUILayout.Space ();
						}
					}
				} else {

					if (!addNewCustomMeleeAttack) {

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Use Attack List Template", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						useMeleeWeaponAttackInfoTemplate = (bool)EditorGUILayout.Toggle (useMeleeWeaponAttackInfoTemplate);
						GUILayout.EndHorizontal ();
				
						EditorGUILayout.Space ();
				
						if (useMeleeWeaponAttackInfoTemplate) {
							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Attack List Template", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
							mainMeleeWeaponAttackInfo = EditorGUILayout.ObjectField (mainMeleeWeaponAttackInfo, typeof(meleeWeaponAttackInfo), true, GUILayout.ExpandWidth (true)) as meleeWeaponAttackInfo;
							GUILayout.EndHorizontal ();
						} else {
							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Melee Weapon Object", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
							mainGrabPhysicalObjectMeleeAttackSystemGameObject = EditorGUILayout.ObjectField (mainGrabPhysicalObjectMeleeAttackSystemGameObject, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
							GUILayout.EndHorizontal ();

							EditorGUILayout.Space ();

							GUILayout.BeginHorizontal ();

							EditorGUILayout.HelpBox ("", MessageType.Info);

							style = new GUIStyle (EditorStyles.helpBox);
							style.richText = true;

							style.fontStyle = FontStyle.Bold;
							style.fontSize = 12;

							EditorGUILayout.LabelField ("The melee weapons object prefabs are on the folder: \n " +
							"Assets\\Game Kit Controller\\Prefabs\\Melee Combat System\\Melee Weapons\n", style);

							GUILayout.EndHorizontal ();
						}

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Action System Prefab", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentActionSystemPrefab = EditorGUILayout.ObjectField (currentActionSystemPrefab, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						EditorGUILayout.Space ();

						EditorGUILayout.Space ();

						showCustomPrefabPathOptions ();

					} else {
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Melee Weapon Object", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						mainGrabPhysicalObjectMeleeAttackSystemGameObject = EditorGUILayout.ObjectField (mainGrabPhysicalObjectMeleeAttackSystemGameObject, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();
						EditorGUILayout.Space ();

						GUILayout.Label ("MELEE ATTACK INFO", EditorStyles.boldLabel);

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Attack Animation Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						actionSystemAnimationName = (string)EditorGUILayout.TextField ("", actionSystemAnimationName);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Attack Type", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						attackType = (string)EditorGUILayout.TextField ("", attackType);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Attack Damage", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						attackDamage = (float)EditorGUILayout.FloatField (attackDamage);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Attack Duration", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						actionSystemDuration = (float)EditorGUILayout.FloatField (actionSystemDuration);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Attack Speed", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						actionSystemSpeed = (float)EditorGUILayout.FloatField (actionSystemSpeed);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						EditorGUILayout.Space ();

						GUILayout.Label ("Number of Elements " + (damageTriggerActiveInfoList.Count).ToString (), EditorStyles.boldLabel);

						EditorGUILayout.Space ();

						for (int i = 0; i < damageTriggerActiveInfoList.Count; i++) { 

							currentDamageTriggerActiveInfo = damageTriggerActiveInfoList [i];

							GUILayout.BeginVertical ("box");

							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Delay To Activate Damage Trigger", EditorStyles.boldLabel);
							currentDamageTriggerActiveInfo.delayToActiveTrigger = (float)EditorGUILayout.FloatField (currentDamageTriggerActiveInfo.delayToActiveTrigger);
							GUILayout.EndHorizontal ();

							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Activate Damage Trigger State", EditorStyles.boldLabel);
							currentDamageTriggerActiveInfo.activateDamageTrigger = (bool)EditorGUILayout.Toggle (currentDamageTriggerActiveInfo.activateDamageTrigger);
							GUILayout.EndHorizontal ();
					
							GUILayout.EndVertical ();

							EditorGUILayout.Space ();
						}

						GUILayout.BeginHorizontal ();
						if (GUILayout.Button ("Clear Values")) {
							damageTriggerActiveInfoList.Clear ();
						}

						if (GUILayout.Button ("Add New Value")) {
							grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo newDamageTriggerActiveInfo = new grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo ();

							damageTriggerActiveInfoList.Add (newDamageTriggerActiveInfo);
						}
						GUILayout.EndHorizontal ();
					}
				}

				EditorGUILayout.EndScrollView ();

				EditorGUILayout.Space ();

				if (editSingleMeleeAttackInfo) {
					if (GUILayout.Button ("Update Melee Attack Info")) {
						if (attackListAssigned && attackInfoAssigned) {
							updateMeleeAttackInfo ();
						}
					}
				} else {
					if (GUILayout.Button ("Add Melee Attack List To Character")) {
						addActionSystem ();
					}
				}

				if (GUILayout.Button ("Cancel")) {
					this.Close ();
				}
			}
		}

		GUILayout.EndVertical ();
	}

	void showCustomPrefabPathOptions ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Use Custom Prefab Path", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		useCustomPrefabPath = (bool)EditorGUILayout.Toggle ("", useCustomPrefabPath);
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (useCustomPrefabPath) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Custom Prefab Path", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			customPrefabPath = EditorGUILayout.TextField ("", customPrefabPath);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Check Prefabs On New Path")) {
				prefabsPath = customPrefabPath;

				checkCurrentObjectSelected (Selection.activeGameObject);
			}
		}
	}

	void addActionSystem ()
	{
		bool checkResult = true;

		if (currentActionSystemPrefab == null) {
			Debug.Log ("WARNING: no prefab found on path " + prefabsPath);

			checkResult = false;
		}

		if (mainGrabPhysicalObjectMeleeAttackSystemGameObject == null) {
			if (!useMeleeWeaponAttackInfoTemplate && !addNewCustomMeleeAttack) {
				Debug.Log ("WARNING: no melee weapon object assigned");

				checkResult = false;
			}
		}

		if (addNewCustomMeleeAttack && damageTriggerActiveInfoList.Count < 2) {
			Debug.Log ("WARNING: make sure to configure at least 2 damage trigger elements on the list for the damage detection");

			checkResult = false;
		}

		if (!checkResult) {
			return;
		}

		if (addNewCustomMeleeAttack) {
			grabPhysicalObjectMeleeAttackSystem.attackInfo newAttackInfo = new grabPhysicalObjectMeleeAttackSystem.attackInfo ();

			newAttackInfo.attackDamage = attackDamage;

			newAttackInfo.attackType = attackType;

			newAttackInfo.customActionName = actionSystemAnimationName;

			newAttackInfo.useCustomAction = true;

			newAttackInfo.attackDuration = actionSystemDuration;

			newAttackInfo.attackDurationOnAir = actionSystemDuration;

			newAttackInfo.attackAnimationSpeed = actionSystemSpeed;

			newAttackInfo.attackAnimationSpeedOnAir = actionSystemSpeed;

			newAttackInfo.minDelayBeforeNextAttack = 1;

			newAttackInfo.playerOnGroundToActivateAttack = false;


			for (int i = 0; i < damageTriggerActiveInfoList.Count; i++) {
				grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo newTriggerInfo = new grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo (damageTriggerActiveInfoList [i]);

				newTriggerInfo.checkSurfacesWithCapsuleRaycast = true;
				newTriggerInfo.checkSurfaceCapsuleRaycastRadius = 0.1f;

				newAttackInfo.damageTriggerActiveInfoList.Add (newTriggerInfo);
			}
			
			if (mainGrabPhysicalObjectMeleeAttackSystemGameObject != null) {
				mainGrabPhysicalObjectMeleeAttackSystem = mainGrabPhysicalObjectMeleeAttackSystemGameObject.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

				if (mainGrabPhysicalObjectMeleeAttackSystem != null) {
					mainGrabPhysicalObjectMeleeAttackSystem.attackInfoList.Add (newAttackInfo);

					GKC_Utils.updateComponent (mainGrabPhysicalObjectMeleeAttackSystem);

					GKC_Utils.updateDirtyScene ("Update Melee Weapon Info", mainGrabPhysicalObjectMeleeAttackSystemGameObject);

					if (mainGrabbedObjectMeleeAttackSystem != null) {
						mainGrabbedObjectMeleeAttackSystem.addEventOnDamageInfoList (mainGrabPhysicalObjectMeleeAttackSystem.weaponInfoName);

						GKC_Utils.updateComponent (mainGrabbedObjectMeleeAttackSystem);
					}
				}
			}

			addAttackInfoAsActionSystem (newAttackInfo);
		}

		if (!addNewCustomMeleeAttack) {
			List<grabPhysicalObjectMeleeAttackSystem.attackInfo> mainAttackInfoList = new List<grabPhysicalObjectMeleeAttackSystem.attackInfo> ();

			if (useMeleeWeaponAttackInfoTemplate) {
				mainAttackInfoList = mainMeleeWeaponAttackInfo.mainMeleeWeaponInfo.mainAttackInfoList;
			} else {
				if (mainGrabPhysicalObjectMeleeAttackSystemGameObject != null) {
					mainGrabPhysicalObjectMeleeAttackSystem = mainGrabPhysicalObjectMeleeAttackSystemGameObject.GetComponent<grabPhysicalObjectMeleeAttackSystem> ();

					if (mainGrabPhysicalObjectMeleeAttackSystem) {
						mainAttackInfoList = mainGrabPhysicalObjectMeleeAttackSystem.attackInfoList;
					}
				}
			}

			for (int i = 0; i < mainAttackInfoList.Count; i++) {
				grabPhysicalObjectMeleeAttackSystem.attackInfo currentAttackInfo = mainAttackInfoList [i];

				addAttackInfoAsActionSystem (currentAttackInfo);
			}
		}

		currentPlayerActionSystem.updateActionList (false);

		GKC_Utils.setActiveGameObjectInEditor (currentPlayerActionSystem.gameObject);

		actionAdded = true;
		
	}

	void addAttackInfoAsActionSystem (grabPhysicalObjectMeleeAttackSystem.attackInfo currentAttackInfo)
	{
		actionSystemName = currentAttackInfo.customActionName;

		if (!currentPlayerActionSystem.checkActionSystemAlreadyExists (actionSystemName)) {

			GameObject actionSystemObject = (GameObject)Instantiate (currentActionSystemPrefab, Vector3.zero, Quaternion.identity);

			actionSystemDuration = currentAttackInfo.attackDuration;

			actionSystemSpeed = currentAttackInfo.attackAnimationSpeed;

			useActionSystemID = false;

			actionSystemID = 0;

			actionSystemAnimationName = actionSystemName;

			actionSystemObject.name = actionSystemName;

			actionSystemObject.transform.SetParent (currentPlayerActionSystem.transform);
			actionSystemObject.transform.localPosition = Vector3.zero;
			actionSystemObject.transform.localRotation = Quaternion.identity;

			actionSystem currentActionSystem = actionSystemObject.GetComponent<actionSystem> ();

			if (currentActionSystem != null) {
				currentActionSystem.addNewActionFromEditor (actionSystemName, actionSystemDuration, actionSystemSpeed,
					useActionSystemID, actionSystemID, actionSystemAnimationName);

				bool addNewActionResult = currentPlayerActionSystem.addNewActionFromEditor (currentActionSystem, 
					                          actionSystemCategoryName, actionSystemName, false, "Sword 1 Hand");

				if (addNewActionResult) {
					Debug.Log ("Action System created and adjusted to character properly");
				} else {
					Debug.Log ("Action System not created properly, check the player action system component on the character selected");
				}
			}
		} else {
			Debug.Log (actionSystemName + " attack already configured, ignored in this case");	
		}
	}

	void updateMeleeAttackInfo ()
	{
		//update the attack info on the weapon attack list and in the action system of that attack
		grabPhysicalObjectMeleeAttackSystem.attackInfo newAttackInfo = mainGrabPhysicalObjectMeleeAttackSystem.attackInfoList [attackListIndex];

		newAttackInfo.attackDamage = attackDamage;

		newAttackInfo.attackType = attackType;

		newAttackInfo.customActionName = actionSystemAnimationName;

		newAttackInfo.attackDuration = actionSystemDuration;

		newAttackInfo.attackDurationOnAir = actionSystemDuration;

		newAttackInfo.attackAnimationSpeed = actionSystemSpeed;

		newAttackInfo.attackAnimationSpeedOnAir = actionSystemSpeed;

		newAttackInfo.damageTriggerActiveInfoList.Clear ();

		for (int i = 0; i < damageTriggerActiveInfoList.Count; i++) {
			grabPhysicalObjectMeleeAttackSystem.damageTriggerActiveInfo newTriggerInfo = damageTriggerActiveInfoList [i];

			newTriggerInfo.checkSurfacesWithCapsuleRaycast = true;
			newTriggerInfo.checkSurfaceCapsuleRaycastRadius = 0.1f;

			newAttackInfo.damageTriggerActiveInfoList.Add (newTriggerInfo);
		}

		GKC_Utils.updateComponent (mainGrabbedObjectMeleeAttackSystem);

		currentPlayerActionSystem.setNewInfoOnAction (actionSystemCategoryName, originalSingleAttackName, actionSystemAnimationName, actionSystemDuration, actionSystemSpeed);

		GKC_Utils.setActiveGameObjectInEditor (currentPlayerActionSystem.gameObject);

		GKC_Utils.updateDirtyScene ("Update Melee Weapon Info", mainGrabPhysicalObjectMeleeAttackSystemGameObject);

		actionAdded = true;
	}

	void Update ()
	{
		if (actionAdded) {
			if (timer < timeToBuild) {
				timer += 0.01f;

				if (timer > timeToBuild) {
					timer = 0;

					this.Close ();
				}
			}
		}
	}
}
#endif