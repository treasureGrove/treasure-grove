using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class armorClothSystemCreatorEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();
	Event currentEvent;

	Vector2 rectSize = new Vector2 (500, 600);

	float minHeight = 600f;

	bool setCreated;

	float timeToBuild = 0.2f;
	float timer;

	string armorClothPrefabPath = "";

	string fullSetsCharacterCreationTemplatesPrefabPath = "";

	string armorClothMeshInventoryPrefabsPath = "";

	public string fullArmorClothName;

	public List<armorClothSetPieceInfo> armorClothSetPieceInfoList = new List<armorClothSetPieceInfo> ();

	public armorClothPieceTemplateData mainArmorClothPieceTemplateData;

	public fullArmorClothTemplateData mainFullArmorClothTemplateData;

	public characterAspectCustomizationTemplateData mainCharacterAspectCustomizationTemplateData;

	public bool useCharacterCustomizationManagerInfo;

	public characterCustomizationManager currentCharacterCustomizationManager;

	public GameObject currentCharacterCustomizationManagerGameObject;


	GUIStyle style = new GUIStyle ();

	GUIStyle labelStyle = new GUIStyle ();

	float windowHeightPercentage = 0.6f;

	Vector2 screenResolution;

	Vector2 scrollPos1;

	float maxLayoutWidht = 220;

	armorClothSetPieceInfo currentArmorClothSetPieceInfo;

	armorClothStatsInfo currentArmorClothStatsInfo;

	Vector2 previousRectSize;

	[MenuItem ("Game Kit Controller/Create Armor-Cloth Set", false, 27)]
	public static void createArmorClothSystem ()
	{
		GetWindow<armorClothSystemCreatorEditor> ();
	}

	void OnEnable ()
	{
		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		//		Debug.Log (screenResolution + " " + partsHeight + " " + settingsHeight + " " + previewHeight);

		float totalHeight = screenResolution.y * windowHeightPercentage;

		totalHeight = Mathf.Clamp (totalHeight, minHeight, screenResolution.y);

		rectSize = new Vector2 (660, totalHeight);

		armorClothPrefabPath = pathInfoValues.getArmorClothPrefabPath ();

		fullSetsCharacterCreationTemplatesPrefabPath = pathInfoValues.getFullSetsCharacterCreationTemplatesPrefabPath ();

		armorClothMeshInventoryPrefabsPath = pathInfoValues.getArmorClothMeshInventoryPrefabsPath ();

		resetCreatorValues ();
	}

	void OnDisable ()
	{
		resetCreatorValues ();
	}

	void resetCreatorValues ()
	{
		if (setCreated) {

		} else {

		}

		Debug.Log ("Set Creator window closed");
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Armor Cloth System", null, "Create New Armor Cloth Set");

		GUILayout.BeginVertical ("Create New Armor Cloth Set", "window");

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

		EditorGUILayout.LabelField ("Configure the info of a new armor/cloth set, for each one of its pieces", style);

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
		EditorGUILayout.Space ();

		labelStyle.fontStyle = FontStyle.Bold;

		EditorGUILayout.LabelField ("ARMOR CLOTH SET INFO", labelStyle);

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Full Armor Cloth Set Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		fullArmorClothName = (string)EditorGUILayout.TextField ("", fullArmorClothName);
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

		EditorGUILayout.Space ();

		GUILayout.Label ("Number of Pieces " + (armorClothSetPieceInfoList.Count).ToString (), EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));

		EditorGUILayout.Space ();

		for (int i = 0; i < armorClothSetPieceInfoList.Count; i++) { 

			currentArmorClothSetPieceInfo = armorClothSetPieceInfoList [i];

			GUILayout.BeginHorizontal ("box");

			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Piece Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentArmorClothSetPieceInfo.Name = (string)EditorGUILayout.TextField ("", currentArmorClothSetPieceInfo.Name);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Category Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentArmorClothSetPieceInfo.categoryName = (string)EditorGUILayout.TextField ("", currentArmorClothSetPieceInfo.categoryName);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Piece Icon", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentArmorClothSetPieceInfo.pieceIcon = EditorGUILayout.ObjectField (currentArmorClothSetPieceInfo.pieceIcon, typeof(Texture), true, GUILayout.ExpandWidth (true)) as Texture;
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Piece Description", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentArmorClothSetPieceInfo.pieceDescription = (string)EditorGUILayout.TextField ("", currentArmorClothSetPieceInfo.pieceDescription);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Piece Mesh Prefab", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentArmorClothSetPieceInfo.pieceMeshPrefab = EditorGUILayout.ObjectField (currentArmorClothSetPieceInfo.pieceMeshPrefab, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Configure Stats", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentArmorClothSetPieceInfo.configureArmorClothStatsInfo = EditorGUILayout.Toggle ("", currentArmorClothSetPieceInfo.configureArmorClothStatsInfo);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("X")) {
				armorClothSetPieceInfoList.RemoveAt (i);
			}

			if (GUILayout.Button ("*")) {
				if (armorClothSetPieceInfoList.Count > 0) {
					armorClothSetPieceInfoList.Add (new armorClothSetPieceInfo (armorClothSetPieceInfoList [i]));
				}
			}

			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();

			if (currentArmorClothSetPieceInfo.configureArmorClothStatsInfo) {
				GUILayout.BeginVertical ("box");

				GUILayout.Label ("Armor Piece Values for " + currentArmorClothSetPieceInfo.Name, EditorStyles.boldLabel);

				EditorGUILayout.Space ();

				for (int j = 0; j < currentArmorClothSetPieceInfo.armorClothStatsInfoList.Count; j++) { 

					GUILayout.Label ("Main Settings", EditorStyles.boldLabel);

					currentArmorClothStatsInfo = currentArmorClothSetPieceInfo.armorClothStatsInfoList [j];

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					currentArmorClothStatsInfo.Name = (string)EditorGUILayout.TextField ("", currentArmorClothStatsInfo.Name);
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();

					GUILayout.Label ("Stats", EditorStyles.boldLabel);

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Stat is Amount", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					currentArmorClothStatsInfo.statIsAmount = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.statIsAmount);
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();

					if (currentArmorClothStatsInfo.statIsAmount) {
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Stat Amount", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentArmorClothStatsInfo.statAmount = (float)EditorGUILayout.FloatField (currentArmorClothStatsInfo.statAmount);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Use Stat Multiplier", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentArmorClothStatsInfo.useStatMultiplier = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.useStatMultiplier);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Use Random Range", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentArmorClothStatsInfo.useRandomRange = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.useRandomRange);
						GUILayout.EndHorizontal ();

						if (currentArmorClothStatsInfo.useRandomRange) {
							EditorGUILayout.Space ();

							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Random Range", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
							currentArmorClothStatsInfo.randomRange = (Vector2)EditorGUILayout.Vector2Field ("", currentArmorClothStatsInfo.randomRange);
							GUILayout.EndHorizontal ();
						}
					} else {
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Bool State", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentArmorClothStatsInfo.newBoolState = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.newBoolState);
						GUILayout.EndHorizontal ();
					}

					EditorGUILayout.Space ();

					GUILayout.Label ("Abilities and Skills", EditorStyles.boldLabel);

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Activate Ability", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					currentArmorClothStatsInfo.activateAbility = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.activateAbility);
					GUILayout.EndHorizontal ();

					if (currentArmorClothStatsInfo.activateAbility) {
						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Ability Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentArmorClothStatsInfo.abilityToActivateName = (string)EditorGUILayout.TextField ("", currentArmorClothStatsInfo.abilityToActivateName);
						GUILayout.EndHorizontal ();
					}

					EditorGUILayout.Space ();

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Unlock Skill", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					currentArmorClothStatsInfo.unlockSkill = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.unlockSkill);
					GUILayout.EndHorizontal ();

					if (currentArmorClothStatsInfo.unlockSkill) {
						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Skill Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentArmorClothStatsInfo.skillNameToUnlock = (string)EditorGUILayout.TextField ("", currentArmorClothStatsInfo.skillNameToUnlock);
						GUILayout.EndHorizontal ();
					}

					EditorGUILayout.Space ();

					GUILayout.Label ("Damage Resistance/Weakness Types", EditorStyles.boldLabel);

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Set Damage Type State", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					currentArmorClothStatsInfo.setDamageTypeState = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.setDamageTypeState);
					GUILayout.EndHorizontal ();

					if (currentArmorClothStatsInfo.setDamageTypeState) {
						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Damage Type Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentArmorClothStatsInfo.damageTypeName = (string)EditorGUILayout.TextField ("", currentArmorClothStatsInfo.damageTypeName);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Damage Type State", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentArmorClothStatsInfo.damageTypeState = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.damageTypeState);
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Increase Damage Type", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentArmorClothStatsInfo.increaseDamageType = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.increaseDamageType);
						GUILayout.EndHorizontal ();

						if (currentArmorClothStatsInfo.increaseDamageType) {
							EditorGUILayout.Space ();

							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Extra Damage Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
							currentArmorClothStatsInfo.extraDamageType = (float)EditorGUILayout.FloatField (currentArmorClothStatsInfo.extraDamageType);
							GUILayout.EndHorizontal ();
						}

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Set Obtain Health On Damage Type", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						currentArmorClothStatsInfo.setObtainHealthOnDamageType = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.setObtainHealthOnDamageType);
						GUILayout.EndHorizontal ();

						if (currentArmorClothStatsInfo.setObtainHealthOnDamageType) {
							EditorGUILayout.Space ();

							GUILayout.BeginHorizontal ();
							GUILayout.Label ("Obtain Health On Damage Type", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
							currentArmorClothStatsInfo.obtainHealthOnDamageType = EditorGUILayout.Toggle ("", currentArmorClothStatsInfo.obtainHealthOnDamageType);
							GUILayout.EndHorizontal ();
						}
					}

					EditorGUILayout.Space ();
				}

				GUILayout.EndVertical ();

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Clear Values")) {
					currentArmorClothSetPieceInfo.armorClothStatsInfoList.Clear ();
				}

				if (GUILayout.Button ("Add New Value")) {
					armorClothStatsInfo newArmorClothStatsInfo = new armorClothStatsInfo ();

					currentArmorClothSetPieceInfo.armorClothStatsInfoList.Add (newArmorClothStatsInfo);
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
			}


			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		}   

		EditorGUILayout.EndScrollView ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Clear Pieces")) {
			armorClothSetPieceInfoList.Clear ();
		}

		if (GUILayout.Button ("Add New Piece")) {
			armorClothSetPieceInfo newArmorClothSetPieceInfo = new armorClothSetPieceInfo ();

			newArmorClothSetPieceInfo.pieceDescription = "New description";

			armorClothSetPieceInfoList.Add (newArmorClothSetPieceInfo);
		}

		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Get Info From Character Object", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		useCharacterCustomizationManagerInfo = EditorGUILayout.Toggle ("", useCharacterCustomizationManagerInfo);
		GUILayout.EndHorizontal ();

		if (useCharacterCustomizationManagerInfo) {
			
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Character Object", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentCharacterCustomizationManagerGameObject = EditorGUILayout.ObjectField (currentCharacterCustomizationManagerGameObject, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (currentCharacterCustomizationManagerGameObject != null) {
				if (GUILayout.Button ("Set Pieces List From Character")) {

					armorClothSetPieceInfoList.Clear ();

					currentCharacterCustomizationManager = currentCharacterCustomizationManagerGameObject.GetComponentInChildren<characterCustomizationManager> ();

					if (currentCharacterCustomizationManager != null) {
						for (int i = 0; i < currentCharacterCustomizationManager.characterObjectTypeInfoList.Count; i++) {
							if (currentCharacterCustomizationManager.characterObjectTypeInfoList [i].Name.Equals (fullArmorClothName)) {
								
								for (int j = 0; j < currentCharacterCustomizationManager.characterObjectTypeInfoList [i].characterObjectInfoList.Count; j++) {
									armorClothSetPieceInfo newArmorClothSetPieceInfo = new armorClothSetPieceInfo ();

									newArmorClothSetPieceInfo.Name = currentCharacterCustomizationManager.characterObjectTypeInfoList [i].characterObjectInfoList [j].Name;

									newArmorClothSetPieceInfo.categoryName = currentCharacterCustomizationManager.characterObjectTypeInfoList [i].characterObjectInfoList [j].typeName;

									newArmorClothSetPieceInfo.pieceDescription = "New description";

									armorClothSetPieceInfoList.Add (newArmorClothSetPieceInfo);
								}
							}
						}
					} else {
						Debug.Log ("WARNING: no character customization manager component located, make sure to assign the object of your\n" +
						"new model with the character customization configured and sets info configured on it ");
					}
				}
			}
		}

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Armor Cloth Piece Template Data", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		mainArmorClothPieceTemplateData = EditorGUILayout.ObjectField (mainArmorClothPieceTemplateData, 
			typeof(armorClothPieceTemplateData), true, GUILayout.ExpandWidth (true)) as armorClothPieceTemplateData;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Full Armor Cloth Piece Template Data", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		mainFullArmorClothTemplateData = EditorGUILayout.ObjectField (mainFullArmorClothTemplateData, 
			typeof(fullArmorClothTemplateData), true, GUILayout.ExpandWidth (true)) as fullArmorClothTemplateData;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Character Aspect Customization Template Data", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		mainCharacterAspectCustomizationTemplateData = EditorGUILayout.ObjectField (mainCharacterAspectCustomizationTemplateData, 
			typeof(characterAspectCustomizationTemplateData), true, GUILayout.ExpandWidth (true)) as characterAspectCustomizationTemplateData;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();


	
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Create New Set")) {
			createNewSet ();
		}

		if (GUILayout.Button ("Cancel")) {
			this.Close ();
		}
		GUILayout.EndVertical ();
	}

	void createNewSet ()
	{
		bool checkInfoResult = true;

		if (fullArmorClothName == "" || armorClothSetPieceInfoList.Count == 0) {
			checkInfoResult = false;
		}

		for (int i = 0; i < armorClothSetPieceInfoList.Count; i++) { 
			if (armorClothSetPieceInfoList [i].Name == "") {
				Debug.Log ("There is an empty name");

				checkInfoResult = false;
			}

			if (armorClothSetPieceInfoList [i].categoryName == "") {
				Debug.Log ("There is an empty category");

				checkInfoResult = false;
			}

			if (armorClothSetPieceInfoList [i].pieceMeshPrefab == null) {
				Debug.Log ("There is an empty piece mesh");

				checkInfoResult = false;
			}

			if (armorClothSetPieceInfoList [i].pieceIcon == null) {
				Debug.Log ("There is an empty piece icon");

				checkInfoResult = false;
			}
		}

		if (mainArmorClothPieceTemplateData == null) {
			Debug.Log ("There is an empty piece template");

			checkInfoResult = false;
		}

		if (mainFullArmorClothTemplateData == null) {
			Debug.Log ("There is an empty full armor template");

			checkInfoResult = false;
		}

		if (mainCharacterAspectCustomizationTemplateData == null) {
			Debug.Log ("There is an empty character aspect customization template");

			checkInfoResult = false;
		}

		if (checkInfoResult) {
			List<string> fullArmorPiecesList = new List<string> ();

			for (int i = 0; i < armorClothSetPieceInfoList.Count; i++) { 

				string pieceName = armorClothSetPieceInfoList [i].Name;

				GKC_Utils.createInventoryArmorClothPiece (pieceName, 
					"Armor Cloth Pieces",
					armorClothSetPieceInfoList [i].categoryName,
					armorClothSetPieceInfoList [i].pieceMeshPrefab,
					armorClothSetPieceInfoList [i].pieceDescription, 
					armorClothSetPieceInfoList [i].pieceIcon, 
					armorClothMeshInventoryPrefabsPath);


				fullArmorPiecesList.Add (pieceName);

				armorClothPieceTemplate newArmorClothPieceTemplate = ScriptableObject.CreateInstance<armorClothPieceTemplate> ();

				newArmorClothPieceTemplate.Name = pieceName;
				newArmorClothPieceTemplate.fullSetName = fullArmorClothName;

				if (armorClothSetPieceInfoList [i].configureArmorClothStatsInfo) {
					newArmorClothPieceTemplate.armorClothStatsInfoList = armorClothSetPieceInfoList [i].armorClothStatsInfoList;
				}

				string pathNewArmorClothPieceTemplate = armorClothPrefabPath + pieceName + ".asset";
		
				AssetDatabase.CreateAsset (newArmorClothPieceTemplate, pathNewArmorClothPieceTemplate);

				AssetDatabase.SaveAssets ();

				AssetDatabase.Refresh ();

				EditorUtility.FocusProjectWindow ();


				//update scriptable objects info for pieces and full armor
				armorClothPieceTemplate currentArmorClothPieceTemplate = (armorClothPieceTemplate)AssetDatabase.LoadAssetAtPath (pathNewArmorClothPieceTemplate, typeof(armorClothPieceTemplate));

				if (currentArmorClothPieceTemplate != null) {
					mainArmorClothPieceTemplateData.armorClothPieceTemplateList.Add (currentArmorClothPieceTemplate);

					EditorUtility.SetDirty (mainArmorClothPieceTemplateData);
				}
			
				AssetDatabase.SaveAssets ();

				AssetDatabase.Refresh ();
			}



			fullArmorClothTemplate newFullArmorClothTemplate = ScriptableObject.CreateInstance<fullArmorClothTemplate> ();

			newFullArmorClothTemplate.Name = fullArmorClothName;

			newFullArmorClothTemplate.fullArmorPiecesList.AddRange (fullArmorPiecesList);

			string pathNewFullArmorClothTemplate = armorClothPrefabPath + fullArmorClothName + ".asset";

			AssetDatabase.CreateAsset (newFullArmorClothTemplate, pathNewFullArmorClothTemplate);

			AssetDatabase.SaveAssets ();

			AssetDatabase.Refresh ();

			EditorUtility.FocusProjectWindow ();



			characterAspectCustomizationTemplate newCharacterAspectCustomizationTemplate = ScriptableObject.CreateInstance<characterAspectCustomizationTemplate> ();

			newCharacterAspectCustomizationTemplate.Name = fullArmorClothName;

			characterCustomizationInfo newCharacterCustomizationInfo = new characterCustomizationInfo ();

			newCharacterCustomizationInfo.Name = fullArmorClothName;
			newCharacterCustomizationInfo.typeName = "Object";
			newCharacterCustomizationInfo.categoryName = "Full Body";

			newCharacterCustomizationInfo.boolValue = true;

			newCharacterCustomizationInfo.multipleElements = true;



			characterCustomizationTypeInfo newCharacterCustomizationTypeInfo = new characterCustomizationTypeInfo ();

			newCharacterCustomizationTypeInfo.Name = "Costume";

			newCharacterCustomizationTypeInfo.characterCustomizationInfoList.Add (newCharacterCustomizationInfo);


			newCharacterAspectCustomizationTemplate.characterCustomizationTypeInfoList.Add (newCharacterCustomizationTypeInfo);


			string pathNewCharacterAspectCustomizationTemplate = fullSetsCharacterCreationTemplatesPrefabPath + fullArmorClothName + ".asset";

			AssetDatabase.CreateAsset (newCharacterAspectCustomizationTemplate, pathNewCharacterAspectCustomizationTemplate);

			AssetDatabase.SaveAssets ();

			AssetDatabase.Refresh ();

			EditorUtility.FocusProjectWindow ();



			//updating the main character aspect customization template scriptable object
			characterAspectCustomizationTemplate currentCharacterAspectCustomizationTemplate = 
				(characterAspectCustomizationTemplate)AssetDatabase.LoadAssetAtPath (pathNewCharacterAspectCustomizationTemplate, typeof(characterAspectCustomizationTemplate));

			if (currentCharacterAspectCustomizationTemplate != null) {
				mainCharacterAspectCustomizationTemplateData.characterAspectCustomizationTemplateList.Add (currentCharacterAspectCustomizationTemplate);
			
				EditorUtility.SetDirty (mainCharacterAspectCustomizationTemplateData);
			}
	
			//update scriptable objects info for pieces and full armor
			fullArmorClothTemplate currentFullArmorClothTemplate = (fullArmorClothTemplate)AssetDatabase.LoadAssetAtPath (pathNewFullArmorClothTemplate, typeof(fullArmorClothTemplate));
		
			if (currentFullArmorClothTemplate != null) {
				mainFullArmorClothTemplateData.fullArmorClothTemplateList.Add (currentFullArmorClothTemplate);

				EditorUtility.SetDirty (mainFullArmorClothTemplateData);
			}

			AssetDatabase.SaveAssets ();

			AssetDatabase.Refresh ();

			Transform[] objectsOnScene = FindObjectsOfType<Transform> ();

			if (objectsOnScene.Length > 0) {
				GKC_Utils.updateDirtyScene ("Update Scene", objectsOnScene [0].gameObject);
			} else {
				GKC_Utils.updateDirtyScene ();
			}

			setCreated = true;

			armorClothSetPieceInfoList.Clear ();
		} else {
			Debug.Log ("WARNING: Make sure to set a full set name and create at least one piece for the set.");
		}
	}

	void Update ()
	{
		if (setCreated) {
			if (timer < timeToBuild) {
				timer += 0.01f;

				if (timer > timeToBuild) {
					timer = 0;

					this.Close ();
				}
			}
		}
	}

	[System.Serializable]
	public class armorClothSetPieceInfo
	{
		public string Name;

		public string categoryName;

		public Texture pieceIcon;

		public string pieceDescription;

		public GameObject pieceMeshPrefab;

		public bool configureArmorClothStatsInfo;

		public List<armorClothStatsInfo> armorClothStatsInfoList = new List<armorClothStatsInfo> ();


		public armorClothSetPieceInfo ()
		{
			
		}

		public armorClothSetPieceInfo (armorClothSetPieceInfo newInfo)
		{
			Name = newInfo.Name;
			categoryName = newInfo.categoryName;
			pieceIcon = newInfo.pieceIcon;
			pieceDescription = newInfo.pieceDescription;
			pieceMeshPrefab = newInfo.pieceMeshPrefab;

			configureArmorClothStatsInfo = newInfo.configureArmorClothStatsInfo;

			armorClothStatsInfoList = newInfo.armorClothStatsInfoList;
		}
	}
}
#endif