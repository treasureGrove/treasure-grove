using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class buildPlayer : MonoBehaviour
{
	public GameObject player;
	public GameObject grabPhysicalObject;
	public GameObject currentCharacterModel;

	public LayerMask layerToPlaceNPC;

	public bool buildPlayerType;
	public bool hasWeaponsEnabled;

	public bool assignNewModelManually;

	public Transform head;
	public Transform neck;
	public Transform chest;
	public Transform spine;

	public Transform hips;

	public Transform rightLowerArm;
	public Transform leftLowerArm;
	public Transform rightHand;
	public Transform leftHand;

	public Transform rightLowerLeg;
	public Transform leftLowerLeg;
	public Transform rightFoot;
	public Transform leftFoot;
	public Transform rightToes;
	public Transform leftToes;

	public GameObject newCharacterModel;

	public List<settingsInfoCategory> settingsInfoCategoryList = new List<settingsInfoCategory> ();
	public List<settingsInfo> temporalSettingsInfoList = new List<settingsInfo> ();

	public characterSettingsTemplate newCharacterSettingsTemplate;

	public string characterSettingToConfigureIfAIIsEnemy = "AI Is Enemy";

	public string characterSettingToConfigureIfAIIsNeutral = "AI Is Neutral";

	public string characterSettingToConfigureAIType = "AI Type";

	[TextArea (2, 3)] public string characterTemplateDataPath = "Assets/Game Kit Controller/Scriptable Objects/Character Templates Data";

	public string characterTemplateName = "New Template";

	public int characterTemplateID = 0;

	public bool applyCharacterSettingsOnManualBuild;

	[TextArea (5, 15)] public string explanation = "Drop on the scene the model prefab for the new player model you want to set on this character " +
	                                               "and press the button 'Build Character'. \n\nIMPORTANT: if your unity version uses nested prefabs, before do this, press in the main parent " +
	                                               "of the player prefab with the right mouse button and press 'Unpack Prefab', so this system can remove and replace objects without need" +
	                                               "to enter in the edit prefab mode";

	public List<characterSettingsTemplateInfo> characterSettingsTemplateInfoList = new List<characterSettingsTemplateInfo> ();

	public List<characterBodyElements> characterBodyElementsList = new List<characterBodyElements> ();

	public UnityEvent eventOnCreatePlayer;

	Transform playerCOM;
	Animator newModelAnimator;
	Vector3 IKHandPos;
	upperBodyRotationSystem upperBodyRotationManager;
	mapSystem mapSystemManager;
	GameObject playerCameraGameObject;

	playerController playerControllerManager;
	playerCamera playerCameraManager;

	ragdollBuilder ragdollBuilderManager;
	gravitySystem gravityManager;
	headTrack headTrackManager;
	IKSystem IKManager;
	playerWeaponsManager weaponsManager;
	ragdollActivator ragdollActivatorManager;
	IKFootSystem IKFootManager;

	bodyMountPointsSystem mainBodyMountPointsSystem;

	playerComponentsManager currentPlayerComponentsManager;

	SkinnedMeshRenderer currentSkinnedMeshRenderer;

	bool isInstantiated;

	settingsInfo currentSettingsInfo;
	settingsInfoCategory currentSettingsInfoCategory;

	string templateNameToApply;

	bool newCharacterIsAI;
	bool newCharacterIsEnemy;
	bool newCharacterIsNeutral;

	bool useTemplateList;


	bool characterHasBeenPreInstantiatedOnEditor;

	bool ignoreApplyCharacterSettings;


	public void buildCharacterByButton ()
	{
		if (!applyCharacterSettingsOnManualBuild) {
			ignoreApplyCharacterSettings = true;
		}

		buildCharacter ();

		ignoreApplyCharacterSettings = false;
	}

	public void setCharacterSettingsStatus (bool characterHasBeenPreInstantiatedOnEditorValue, bool characterIsAI, bool configuredAsEnemy,
	                                        bool configuredAsNeutral, string newTemplateNameToApply, bool useTemplateListValue)
	{
		characterHasBeenPreInstantiatedOnEditor = characterHasBeenPreInstantiatedOnEditorValue;

		newCharacterIsEnemy = configuredAsEnemy;

		newCharacterIsNeutral = configuredAsNeutral;

		templateNameToApply = newTemplateNameToApply;

		newCharacterIsAI = characterIsAI;

		useTemplateList = useTemplateListValue;
	}

	public void setCharacterVariables (bool characterIsPlayerType, bool hasWeapons, bool characterIsInstantiated, GameObject newModel)
	{
		buildPlayerType = characterIsPlayerType;
		hasWeaponsEnabled = hasWeapons;
		isInstantiated = characterIsInstantiated;

		newCharacterModel = newModel;
	}

	public void setCharacterBones (Transform newHead,
	                               Transform newNeck,
	                               Transform newChest,
	                               Transform newSpine,

	                               Transform newHips,
	                               Transform newRightLowerArm,
	                               Transform newLeftLowerArm,
	                               Transform newRightHand,
	                               Transform newLeftHand,

	                               Transform newRightLowerLeg,
	                               Transform newLeftLowerLeg,
	                               Transform newRightFoot,
	                               Transform newLeftFoot,
	                               Transform newRightToes,
	                               Transform newLeftToes)
	{

		head = newHead;
		neck = newNeck;
		chest = newChest;
		spine = newSpine;

		hips = newHips;

		rightLowerArm = newRightLowerArm;
		leftLowerArm = newLeftLowerArm;
		rightHand = newRightHand;
		leftHand = newLeftHand;

		rightLowerLeg = newRightLowerLeg;
		leftLowerLeg = newLeftLowerLeg;
		rightFoot = newRightFoot;
		leftFoot = newLeftFoot;
		rightToes = newRightToes;
		leftToes = newLeftToes;
	}

	public void getCharacterBones ()
	{
		if (newCharacterModel == null) {
			print ("WARNING: There is not new character model to use, assign it on the inspector or select a valid model on the character creator window");

			return;
		}

		newModelAnimator = newCharacterModel.GetComponent<Animator> ();

		if (newModelAnimator == null) {
			print ("WARNING: There is no animator configured in the model assigned, make sure to check that the model rig is configured as humanoid " +
			"and the model dropped in the scene has an animator component attached");

			return;
		}

		head = newModelAnimator.GetBoneTransform (HumanBodyBones.Head);
		neck = newModelAnimator.GetBoneTransform (HumanBodyBones.Neck);

		if (neck == null) {
			if (head != null) {
				neck = head.parent;
			} else {
				print ("WARNING: no head found, assign it manually to make sure all of them are configured correctly");
			}
		}	

		chest = newModelAnimator.GetBoneTransform (HumanBodyBones.Chest);
		spine = newModelAnimator.GetBoneTransform (HumanBodyBones.Spine);
	
		if (spine != null) {
			if (chest != null) {
				if (spine != chest.parent) {
					spine = chest.parent;
				}
			} else {
				print ("WARNING: no chest found, assign it manually to make sure all of them are configured correctly");
			}
		} else {
			print ("WARNING: no spine found, assign it manually to make sure all of them are configured correctly");
		}

		hips = newModelAnimator.GetBoneTransform (HumanBodyBones.Hips);

		rightLowerArm = newModelAnimator.GetBoneTransform (HumanBodyBones.RightLowerArm);
		leftLowerArm = newModelAnimator.GetBoneTransform (HumanBodyBones.LeftLowerArm);
		rightHand = newModelAnimator.GetBoneTransform (HumanBodyBones.RightHand);
		leftHand = newModelAnimator.GetBoneTransform (HumanBodyBones.LeftHand);

		rightLowerLeg = newModelAnimator.GetBoneTransform (HumanBodyBones.RightLowerLeg);
		leftLowerLeg = newModelAnimator.GetBoneTransform (HumanBodyBones.LeftLowerLeg);
		rightFoot = newModelAnimator.GetBoneTransform (HumanBodyBones.RightFoot);
		leftFoot = newModelAnimator.GetBoneTransform (HumanBodyBones.LeftFoot);
		rightToes = newModelAnimator.GetBoneTransform (HumanBodyBones.RightToes);
		leftToes = newModelAnimator.GetBoneTransform (HumanBodyBones.LeftToes);
	}

	public bool checkAllBonesFound ()
	{
		return 
			(head != null) &&
		(neck != null) &&
		(chest != null) &&
		(spine != null) &&
		(hips != null) &&
		(rightLowerArm != null) &&
		(leftLowerArm != null) &&
		(rightHand != null) &&
		(leftHand != null) &&
		(rightLowerLeg != null) &&
		(leftLowerLeg != null) &&
		(rightFoot != null) &&
		(leftFoot != null) &&
		(rightToes != null) &&
		(leftToes != null);
	}

	//set all the objects inside the character's body
	public void buildCharacter ()
	{
		//it only works in the editor mode, checking the game is not running
		if (!Application.isPlaying) {
			if (!assignNewModelManually) {
				getCharacterBones ();
			}

			if (!checkAllBonesFound ()) {
				print ("WARNING: not all bones necessary for the new player has been found, assign them manually to make sure all of them are configured correctly");
				return;
			}

			if (newCharacterModel == null) {
				print ("WARNING: Assign the New Character Model in the inspector");
				return;
			}

			print ("\n\n");
			print ("CREATING NEW CHARACTER");
			print ("\n\n");

			if (assignNewModelManually) {
				newModelAnimator = newCharacterModel.GetComponent<Animator> ();
				 
				newCharacterModel.transform.position = currentCharacterModel.transform.position;
				newCharacterModel.transform.rotation = currentCharacterModel.transform.rotation;
			}

			currentPlayerComponentsManager = player.GetComponent<playerComponentsManager> ();

			upperBodyRotationManager = player.GetComponent<upperBodyRotationSystem> ();

			mapSystemManager = currentPlayerComponentsManager.getMapSystem ();

			playerControllerManager = currentPlayerComponentsManager.getPlayerController ();

			playerCameraGameObject = playerControllerManager.getPlayerCameraGameObject ();

			playerCameraManager = currentPlayerComponentsManager.getPlayerCamera ();

			gravityManager = currentPlayerComponentsManager.getGravitySystem ();

			headTrackManager = currentPlayerComponentsManager.getHeadTrack ();

			weaponsManager = currentPlayerComponentsManager.getPlayerWeaponsManager ();

			ragdollActivatorManager = currentPlayerComponentsManager.getRagdollActivator ();

			IKFootManager = GetComponentInChildren<IKFootSystem> ();

			IKManager = currentPlayerComponentsManager.getIKSystem ();

			mainBodyMountPointsSystem = currentPlayerComponentsManager.getBodyMountPointsSystem ();

			setMainBodyMountPoints ();

			playerCOM = IKManager.getIKBodyCOM ();

			currentCharacterModel.transform.SetParent (null);

			newCharacterModel.transform.SetParent (playerCOM);
				
			//get and set the animator and avatar of the model
			player.GetComponent<Animator> ().avatar = newModelAnimator.avatar;

			currentSkinnedMeshRenderer = playerCOM.GetComponentInChildren<SkinnedMeshRenderer> ();

			//set the part of every arm in the otherpowers script
			setPowerSettings ();

			//set the animator in the ragdoll builder component
			setRagdollElements ();

			playerCameraManager.setHipsTransform (hips);

			setIKFootElements ();

			setWeapons ();

			setIKUpperBodyComponents ();

			setMapSystemComponents ();

			setHealthWeakSpots ();

			setHeadTrackInfo ();

			setPlayerControllerComponents ();

			setPlayerGrabPhysicalObjectComponents ();

			setPlayerCullingSystem ();

			setCharacterCustomizationManager ();

			if (buildPlayerType) {
				print ("\n\n");

				print ("Creating new player, adding all main managers into scene");

				print ("\n\n");

				GKC_Utils.addAllMainManagersToScene ();
			}

			if (ignoreApplyCharacterSettings) {
				print ("Character settings not applied due to use the manual build with the ignore option active");

			} else {
				adjustSettings ();
			}

			ignoreApplyCharacterSettings = false;

			DestroyImmediate (currentCharacterModel);

			currentCharacterModel = newCharacterModel;

			newModelAnimator.runtimeAnimatorController = null;

			if (isInstantiated) {
				placeCharacterInCameraPosition ();
			}

			head = null;
			neck = null;
			chest = null;
			spine = null;
			hips = null;
			rightLowerArm = null;
			leftLowerArm = null;
			rightHand = null;
			leftHand = null;
			rightLowerLeg = null;
			leftLowerLeg = null;
			rightFoot = null;
			leftFoot = null;
			rightToes = null;
			leftToes = null;

			assignNewModelManually = false;

			newCharacterModel = null;

			eventOnCreatePlayer.Invoke ();

			updateComponent ();

			print ("IMPORTANT: Character created successfully. Remember to check the component Upper Body Rotation System, in Player Controller gameObject. In that inspector, check the value " +
			"Chest Up Vector and adjust it if the player spine behaves strange when aiming a weapon in third person. You can find a better explanation in the documentation");
		}
	}

	void setMainBodyMountPoints ()
	{
		if (mainBodyMountPointsSystem != null) {
			mainBodyMountPointsSystem.setNewAnimator (newModelAnimator);

			mainBodyMountPointsSystem.setCharacterBodyMountPointsInfoList ();
		}
	}

	void setPlayerControllerComponents ()
	{
		if (currentSkinnedMeshRenderer != null) {
			playerControllerManager.setCharacterMeshGameObject (currentSkinnedMeshRenderer.gameObject);
		}
	}

	void setPowerSettings ()
	{
		otherPowers powers = currentPlayerComponentsManager.getOtherPowers ();

		powers.aimsettings.leftHand = leftHand.gameObject;
		powers.aimsettings.rightHand = rightHand.gameObject;

		if (currentSkinnedMeshRenderer != null) {
			int charactersMaterials = currentSkinnedMeshRenderer.sharedMaterials.Length;

			gravityManager.materialToChange = new List<bool> (charactersMaterials);

			for (int i = 0; i < gravityManager.materialToChange.Count; i++) {
				gravityManager.materialToChange [i] = true;
			}
		}

		gravityManager.setMeshCharacter (currentSkinnedMeshRenderer);
	}

	void setRagdollElements ()
	{
		ragdollBuilderManager = GetComponent<ragdollBuilder> ();

		if (ragdollBuilderManager != null) {
			ragdollBuilderManager.getAnimator (newModelAnimator);
			ragdollBuilderManager.setRagdollActivator (ragdollActivatorManager);
			ragdollBuilderManager.createRagdoll ();
		}

		if (ragdollActivatorManager != null) {
			ragdollActivatorManager.setCharacterBody (newCharacterModel, newModelAnimator);
		}
	}

	void setIKFootElements ()
	{
		if (IKFootManager != null) {
			IKFootManager.setLegsInfo (hips, rightLowerLeg, leftLowerLeg, rightFoot, leftFoot, rightToes, leftToes);
		}
	}

	void setIKUpperBodyComponents ()
	{
		upperBodyRotationManager.spineTransform = spine;
		upperBodyRotationManager.chestTransform = chest;

		Vector3 localDirection = chest.InverseTransformDirection (Vector3.up);

		localDirection = new Vector3 (Mathf.RoundToInt (localDirection.x), Mathf.RoundToInt (localDirection.y), Mathf.RoundToInt (localDirection.z));

		upperBodyRotationManager.setNewChestUpVectorValue (localDirection);
	}

	void setMapSystemComponents ()
	{
		if (mapSystemManager != null) {
			mapSystemManager.searchBuildingList ();
		}
	}

	void setHealthWeakSpots ()
	{
		if (!buildPlayerType) {
			currentPlayerComponentsManager.getHealth ().setHumanoidWeaKSpots ();
		}
	}

	void setHeadTrackInfo ()
	{
		if (headTrackManager != null) {
			headTrackManager.setHeadTransform (head);
		}
	}

	void setWeapons ()
	{
		weaponsManager.setThirdPersonParent (chest);

		weaponsManager.setRightHandTransform (rightHand);

		weaponsManager.setLeftHandTransform (leftHand);

		weaponsManager.setWeaponList ();

		if (!hasWeaponsEnabled) {
			weaponsManager.enableOrDisableWeaponsList (false);
		}
	}

	void setPlayerGrabPhysicalObjectComponents ()
	{
		if (grabPhysicalObject != null) {
			grabPhysicalObjectSystem currentgrabPhysicalObjectSystem = grabPhysicalObject.GetComponent<grabPhysicalObjectSystem> ();

			if (currentgrabPhysicalObjectSystem != null) {
				currentgrabPhysicalObjectSystem.setCharacter (player);
				currentgrabPhysicalObjectSystem.setCharacterBody (newCharacterModel.transform);

				currentgrabPhysicalObjectSystem.setObjectToGrabParent (hips.gameObject);
			}
		}
	}

	void setPlayerCullingSystem ()
	{
		playerCullingSystem currentPlayerCullingSystem = playerCameraGameObject.GetComponent<playerCullingSystem> ();

		if (currentPlayerCullingSystem != null && currentSkinnedMeshRenderer != null) {
			currentPlayerCullingSystem.playerGameObjectList.Clear ();

			currentPlayerCullingSystem.playerGameObjectList.Add (currentSkinnedMeshRenderer.gameObject);

			currentPlayerCullingSystem.storePlayerRendererFromEditor ();
		}
	}

	void setCharacterCustomizationManager ()
	{
		inventoryCharacterCustomizationSystem currentInventoryCharacterCustomizationSystem = currentPlayerComponentsManager.getInventoryCharacterCustomizationSystem ();
	
		if (currentInventoryCharacterCustomizationSystem != null) {
			characterCustomizationManager currentCharacterCustomizationManager = GetComponentInChildren<characterCustomizationManager> ();

			if (currentCharacterCustomizationManager != null) {
				currentInventoryCharacterCustomizationSystem.mainCharacterCustomizationManager = currentCharacterCustomizationManager;
			}
		}
	}

	public void setCharacterValuesAndAdjustSettingsExternally (bool characterHasBeenPreInstantiatedOnEditorValue, 
	                                                           bool characterIsAI, 
	                                                           bool configuredAsEnemy,
	                                                           bool configuredAsNeutral, 
	                                                           string newTemplateNameToApply, 
	                                                           bool useTemplateListValue, 
	                                                           bool characterIsPlayerType, 
	                                                           bool hasWeapons, 
	                                                           bool characterIsInstantiated)
	{
		setCharacterSettingsStatus (characterHasBeenPreInstantiatedOnEditorValue, 
			characterIsAI, 
			configuredAsEnemy,
			configuredAsNeutral, 
			newTemplateNameToApply, 
			useTemplateListValue);

		setCharacterVariables (characterIsPlayerType, 
			hasWeapons, 
			characterIsInstantiated, null);

		adjustSettings ();
	}

	public void adjustSettings ()
	{
		print ("\n\n");
		print ("APPLYING SETTINGS LIST TO NEW CHARACTER");
		print ("\n\n");

		if (newCharacterIsAI || useTemplateList) {

			if (useTemplateList) {
				if (newCharacterIsAI) {
					print ("Creating New AI Using Template " + templateNameToApply);
				} else {
					print ("Creating New Player Using Template " + templateNameToApply);
				}
			}

			if (characterHasBeenPreInstantiatedOnEditor && !useTemplateList) {
				for (int i = 0; i < settingsInfoCategoryList.Count; i++) { 
					currentSettingsInfoCategory = settingsInfoCategoryList [i];

					for (int j = 0; j < currentSettingsInfoCategory.settingsInfoList.Count; j++) { 
						currentSettingsInfo = currentSettingsInfoCategory.settingsInfoList [j];

						if (currentSettingsInfo.Name.Equals (characterSettingToConfigureIfAIIsEnemy)) {
							newCharacterIsEnemy = currentSettingsInfo.regularValue;
						}

						if (currentSettingsInfo.Name.Equals (characterSettingToConfigureAIType)) {
							templateNameToApply = currentSettingsInfo.stringValue;
						}

						if (currentSettingsInfo.Name.Equals (characterSettingToConfigureIfAIIsNeutral)) {
							newCharacterIsNeutral = currentSettingsInfo.regularValue;
						}
					}
				}
			} else {
				if (newCharacterIsAI) {
					if (newCharacterIsEnemy) {
						print ("New character is AI, adjusting its value as Enemy");
					} else {
						if (newCharacterIsNeutral) {
							print ("New character is AI, adjusting its value as Neutral");
						} else {
							print ("New character is AI, adjusting its value as Friend");
						}
					}
				}

				int templateIndex = characterSettingsTemplateInfoList.FindIndex (s => s.Name.ToLower () == templateNameToApply.ToLower ());

				if (templateIndex > -1) {
					setTemplateFromListAsCurrentToApply (templateIndex);
				}

				for (int i = 0; i < settingsInfoCategoryList.Count; i++) { 
					currentSettingsInfoCategory = settingsInfoCategoryList [i];

					for (int j = 0; j < currentSettingsInfoCategory.settingsInfoList.Count; j++) { 
						currentSettingsInfo = currentSettingsInfoCategory.settingsInfoList [j];

						if (currentSettingsInfo.Name.Equals (characterSettingToConfigureIfAIIsEnemy)) {
							currentSettingsInfo.regularValue = newCharacterIsEnemy;
						}

						if (currentSettingsInfo.Name.Equals (characterSettingToConfigureIfAIIsNeutral)) {
							currentSettingsInfo.regularValue = newCharacterIsNeutral;
						}
					}
				}

				characterHasBeenPreInstantiatedOnEditor = false;
			}

			if (newCharacterIsAI) {
				string AIType = "Friend";

				if (newCharacterIsEnemy) {
					AIType = "Enemy";
				} else {
					if (newCharacterIsNeutral) {
						AIType = "Neutral";
					}
				}

				gameObject.name = "AI " + AIType + " " + templateNameToApply;

				print ("Created new AI " + AIType + " of type " + templateNameToApply);
			}
		} 
			
		for (int i = 0; i < settingsInfoCategoryList.Count; i++) { 

			currentSettingsInfoCategory = settingsInfoCategoryList [i];

			adjustSettingsFromList (currentSettingsInfoCategory.settingsInfoList);
		}
	}

	public void adjustSettingsFromList (List<settingsInfo> settingsList)
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

	public void adjustSettingsFromEditor ()
	{
		for (int i = 0; i < settingsInfoCategoryList.Count; i++) { 
			currentSettingsInfoCategory = settingsInfoCategoryList [i];

			for (int j = 0; j < currentSettingsInfoCategory.settingsInfoList.Count; j++) { 
				currentSettingsInfo = currentSettingsInfoCategory.settingsInfoList [j];

				int currentIndex = temporalSettingsInfoList.FindIndex (s => s.Name.ToLower () == currentSettingsInfo.Name.ToLower ());

				if (currentIndex > -1) {

					settingsInfo temporalSettingsInfo = temporalSettingsInfoList [currentIndex];

					currentSettingsInfo.settingEnabled = temporalSettingsInfo.settingEnabled;

					currentSettingsInfo.useBoolState = temporalSettingsInfo.useBoolState;
					currentSettingsInfo.boolState = temporalSettingsInfo.boolState;

					currentSettingsInfo.useFloatValue = temporalSettingsInfo.useFloatValue;
					currentSettingsInfo.floatValue = temporalSettingsInfo.floatValue;

					currentSettingsInfo.useStringValue = temporalSettingsInfo.useStringValue;
					currentSettingsInfo.stringValue = temporalSettingsInfo.stringValue;

					currentSettingsInfo.useRegularValue = temporalSettingsInfo.useRegularValue;
					currentSettingsInfo.regularValue = temporalSettingsInfo.regularValue;
				}
			}
		}

		adjustSettingsFromList (temporalSettingsInfoList);

		updateComponent ();

		print ("Settings applied correctly");
	}

	public void adjustSingleSettingsromEditor (int currentIndex)
	{
		settingsInfo temporalSettingsInfo = temporalSettingsInfoList [currentIndex];

		currentSettingsInfo.settingEnabled = temporalSettingsInfo.settingEnabled;

		currentSettingsInfo.useBoolState = temporalSettingsInfo.useBoolState;
		currentSettingsInfo.boolState = temporalSettingsInfo.boolState;

		currentSettingsInfo.useFloatValue = temporalSettingsInfo.useFloatValue;
		currentSettingsInfo.floatValue = temporalSettingsInfo.floatValue;

		currentSettingsInfo.useStringValue = temporalSettingsInfo.useStringValue;
		currentSettingsInfo.stringValue = temporalSettingsInfo.stringValue;

		currentSettingsInfo.useRegularValue = temporalSettingsInfo.useRegularValue;
		currentSettingsInfo.regularValue = temporalSettingsInfo.regularValue;

		List<settingsInfo> currentTemporalSettingsInfoList = new List<settingsInfo> ();

		currentTemporalSettingsInfoList.Add (temporalSettingsInfo);

		adjustSettingsFromList (currentTemporalSettingsInfoList);

		updateComponent ();

		print ("Single Setting applied correctly");
	}

	public void setTemporalSettingsInfoList ()
	{
		temporalSettingsInfoList.Clear ();

		for (int i = 0; i < settingsInfoCategoryList.Count; i++) { 

			currentSettingsInfoCategory = settingsInfoCategoryList [i];

			for (int j = 0; j < currentSettingsInfoCategory.settingsInfoList.Count; j++) { 

				settingsInfo newSettingsInfo = new settingsInfo (currentSettingsInfoCategory.settingsInfoList [j]);

				temporalSettingsInfoList.Add (newSettingsInfo); 

			}
		}

		updateComponent ();

		GKC_Utils.updateDirtyScene ("Update Setting List", gameObject);
	}

	public void clearTemporalSettingsInfoList ()
	{
		temporalSettingsInfoList.Clear ();

		updateComponent ();

		GKC_Utils.updateDirtyScene ("Update Setting List", gameObject);
	}

	void placeCharacterInCameraPosition ()
	{
		Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

		if (currentCameraEditor != null) {
			Vector3 editorCameraPosition = currentCameraEditor.transform.position;
			Vector3 editorCameraForward = currentCameraEditor.transform.forward;

			RaycastHit hit;

			if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceNPC)) {
				if (!buildPlayerType) {
					transform.position = hit.point;
				} else {
					player.transform.position = hit.point;
					playerCameraGameObject.transform.position = player.transform.position;
				}
			}
		}
	}

	public void saveSettingsListToFile ()
	{
		if (newCharacterSettingsTemplate != null) {
			newCharacterSettingsTemplate.characterTemplateID = characterTemplateID;

			List<settingsInfoCategory> newSettingsInfoCategoryList = new List<settingsInfoCategory> ();

			for (int i = 0; i < settingsInfoCategoryList.Count; i++) { 

				currentSettingsInfoCategory = settingsInfoCategoryList [i];

				settingsInfoCategory newSettingsInfoCategory = new settingsInfoCategory ();

				newSettingsInfoCategory.Name = currentSettingsInfoCategory.Name;

				for (int j = 0; j < currentSettingsInfoCategory.settingsInfoList.Count; j++) { 

					currentSettingsInfo = currentSettingsInfoCategory.settingsInfoList [j];

					settingsInfo newSettingsInfo = new settingsInfo ();

					newSettingsInfo.Name = currentSettingsInfo.Name;

					newSettingsInfo.settingEnabled = currentSettingsInfo.settingEnabled;

					newSettingsInfo.useBoolState = currentSettingsInfo.useBoolState;
					newSettingsInfo.boolState = currentSettingsInfo.boolState;
				
					newSettingsInfo.useFloatValue = currentSettingsInfo.useFloatValue;
					newSettingsInfo.floatValue = currentSettingsInfo.floatValue;

					newSettingsInfo.useStringValue = currentSettingsInfo.useStringValue;
					newSettingsInfo.stringValue = currentSettingsInfo.stringValue;
					
					newSettingsInfo.useRegularValue = currentSettingsInfo.useRegularValue;
					newSettingsInfo.regularValue = currentSettingsInfo.regularValue;

					newSettingsInfoCategory.settingsInfoList.Add (newSettingsInfo);
				}

				newSettingsInfoCategoryList.Add (newSettingsInfoCategory);
			}

			newCharacterSettingsTemplate.settingsInfoCategoryList = newSettingsInfoCategoryList;

			print ("Settings list saved to file");
		} else {
			print ("WARNING: no character template has been assigned in the character settings template field"); 
		}
	}

	public void loadSettingsListFromFile ()
	{
		if (newCharacterSettingsTemplate != null) {
			if (newCharacterSettingsTemplate.characterTemplateID != characterTemplateID) {
				print ("WARNING: The character template id don't match, make sure to use a compatible template with this character ID, which is " + characterTemplateID);
			}

			List<settingsInfoCategory> temporalSettingsInfoCategoryList = new List<settingsInfoCategory> (newCharacterSettingsTemplate.settingsInfoCategoryList);

			for (int i = 0; i < settingsInfoCategoryList.Count; i++) { 
				currentSettingsInfoCategory = settingsInfoCategoryList [i];

				for (int j = 0; j < currentSettingsInfoCategory.settingsInfoList.Count; j++) { 

					currentSettingsInfo = currentSettingsInfoCategory.settingsInfoList [j];

					int currentCategoryIndex = temporalSettingsInfoCategoryList.FindIndex (s => s.Name.ToLower () == currentSettingsInfoCategory.Name.ToLower ());

					if (currentCategoryIndex > -1) {

						int currentIndex = temporalSettingsInfoCategoryList [currentCategoryIndex].settingsInfoList.FindIndex (s => s.Name.ToLower () == currentSettingsInfo.Name.ToLower ());

						if (currentIndex > -1) {
							
							settingsInfo temporalSettingsInfo = temporalSettingsInfoCategoryList [currentCategoryIndex].settingsInfoList [currentIndex];

							currentSettingsInfo.settingEnabled = temporalSettingsInfo.settingEnabled;

							currentSettingsInfo.useBoolState = temporalSettingsInfo.useBoolState;
							currentSettingsInfo.boolState = temporalSettingsInfo.boolState;

							currentSettingsInfo.useFloatValue = temporalSettingsInfo.useFloatValue;
							currentSettingsInfo.floatValue = temporalSettingsInfo.floatValue;
						
							currentSettingsInfo.useStringValue = temporalSettingsInfo.useStringValue;
							currentSettingsInfo.stringValue = temporalSettingsInfo.stringValue;
						
							currentSettingsInfo.useRegularValue = temporalSettingsInfo.useRegularValue;
							currentSettingsInfo.regularValue = temporalSettingsInfo.regularValue;
						}
					}
				}
			}

			updateComponent ();

			setTemporalSettingsInfoList ();

			print ("Settings list loaded from file");
		} else {
			print ("WARNING: no character template has been assigned in the character settings template field"); 
		}
	}

	public void createSettingsListTemplate ()
	{
		GKC_Utils.createSettingsListTemplate (characterTemplateDataPath, characterTemplateName, characterTemplateID, settingsInfoCategoryList);
	}

	public void setTemplateFromListAsCurrentToApply (int templateIndex)
	{
		newCharacterSettingsTemplate = characterSettingsTemplateInfoList [templateIndex].template;

		print ("Adjusting template on character as " + newCharacterSettingsTemplate.name);

		loadSettingsListFromFile ();

		updateComponent ();
	}

	public void setTemplateFromListAsCurrentToApplyOnCharacterCreatorEditor (int templateIndex)
	{
		setTemplateFromListAsCurrentToApply (templateIndex);
	}

	public void setTemplateFromListAsCurrentToApplyByName (string templateName)
	{
		int currentIndex = characterSettingsTemplateInfoList.FindIndex (s => s.Name.ToLower () == templateName.ToLower ());

		if (currentIndex > -1) {
			setTemplateFromListAsCurrentToApply (currentIndex);
		}
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class settingsInfoCategory
	{
		public string Name;
		public List<settingsInfo> settingsInfoList = new List<settingsInfo> ();
	}

	[System.Serializable]
	public class settingsInfo
	{
		public string Name;

		public bool settingEnabled = true;

		public bool useBoolState;
		public bool boolState;
		public eventParameters.eventToCallWithBool eventToSetBoolState;

		public bool useFloatValue;
		public float floatValue;
		public eventParameters.eventToCallWithAmount eventToSetFloatValue;

		public bool useStringValue;
		public string stringValue;
		public eventParameters.eventToCallWithString eventToSetStringValue;

		public bool useVector3Value;
		public Vector3 vector3Value;
		public eventParameters.eventToCallWithVector3 eventToSetVector3Value;

		public bool useRegularValue;
		public bool regularValue;
		public UnityEvent eventToEnableActiveValue;
		public UnityEvent eventToDisableActiveValue;

		public bool useFieldExplanation;
		[TextArea (5, 15)]public string fieldExplanation;

		public settingsInfo (settingsInfo newSettingsInfo)
		{
			Name = newSettingsInfo.Name;

			settingEnabled = newSettingsInfo.settingEnabled;

			useBoolState = newSettingsInfo.useBoolState;
			boolState = newSettingsInfo.boolState;
			eventToSetBoolState = newSettingsInfo.eventToSetBoolState;

			useFloatValue = newSettingsInfo.useFloatValue;
			floatValue = newSettingsInfo.floatValue;
			eventToSetFloatValue = newSettingsInfo.eventToSetFloatValue;

			useStringValue = newSettingsInfo.useStringValue;
			stringValue = newSettingsInfo.stringValue;
			eventToSetStringValue = newSettingsInfo.eventToSetStringValue;

			useVector3Value = newSettingsInfo.useVector3Value;
			vector3Value = newSettingsInfo.vector3Value;
			eventToSetVector3Value = newSettingsInfo.eventToSetVector3Value;

			useRegularValue = newSettingsInfo.useRegularValue;
			regularValue = newSettingsInfo.regularValue;
			eventToEnableActiveValue = newSettingsInfo.eventToEnableActiveValue;
			eventToDisableActiveValue = newSettingsInfo.eventToDisableActiveValue;

			useFieldExplanation = newSettingsInfo.useFieldExplanation;
			fieldExplanation = newSettingsInfo.fieldExplanation;
		}

		public settingsInfo ()
		{

		}
	}

	[System.Serializable]
	public class characterSettingsTemplateInfo
	{
		public string Name;
		public characterSettingsTemplate template;
	}

	[System.Serializable]
	public class characterBodyElements
	{
		public string Name;
		public GameObject elementGameObject;
		public HumanBodyBones mainBoneToPlaceElement;
		public HumanBodyBones alternativeBoneToPlaceElement;
	}
}