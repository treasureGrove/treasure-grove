using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class craftingBlueprintCreatorEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();
	Event currentEvent;

	Vector2 rectSize = new Vector2 (500, 600);

	float minHeight = 600f;

	bool elementCreated;

	float timeToBuild = 0.2f;
	float timer;

	string blueprintPrefabsPath = "";

	public string objectCategory;

	public string Name;

	public int ID = 0;

	public int amountObtained = 1;

	public bool useObjectToPlace;

	public GameObject objectToPlace;

	public Vector3 objectToPlacePositionOffset;

	public bool useCustomLayerMaskToPlaceObject;

	public LayerMask customLayerMaskToPlaceObject;

	public LayerMask layerMaskToAttachObject;

	public bool objectCanBeRotatedOnYAxis = true;
	public bool objectCanBeRotatedOnXAxis;

	public bool craftObjectInTime;
	public float timeToCraftObject;

	public List<craftingIngredientObjectInfo> craftingIngredientObjectInfoList = new List<craftingIngredientObjectInfo> ();

	public List<craftingIngredientObjectInfo> repairIngredientObjectInfoList = new List<craftingIngredientObjectInfo> ();

	public List<craftingIngredientObjectInfo> brokenIngredientObjectInfoList = new List<craftingIngredientObjectInfo> ();

	public List<craftingIngredientObjectInfo> disassembleIngredientObjectInfoList = new List<craftingIngredientObjectInfo> ();

	public bool checkStatsInfoToCraft;

	public List<craftingStatInfo> craftingStatInfoToCraftList = new List<craftingStatInfo> ();


	public craftingBlueprintInfoTemplateData mainCraftingBlueprintInfoTemplateData;


	GUIStyle style = new GUIStyle ();

	GUIStyle labelStyle = new GUIStyle ();

	float windowHeightPercentage = 0.6f;

	Vector2 screenResolution;

	Vector2 scrollPos1;

	float maxLayoutWidht = 220;

	Vector2 previousRectSize;

	[MenuItem ("Game Kit Controller/Create Crafting Blueprint", false, 28)]
	public static void createCraftingBlueSystem ()
	{
		GetWindow<craftingBlueprintCreatorEditor> ();
	}

	void OnEnable ()
	{
		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		float totalHeight = screenResolution.y * windowHeightPercentage;

		totalHeight = Mathf.Clamp (totalHeight, minHeight, screenResolution.y);

		rectSize = new Vector2 (660, totalHeight);

		blueprintPrefabsPath = pathInfoValues.getCraftingBlueprintPrefabsPath ();

		resetCreatorValues ();
	}

	void OnDisable ()
	{
		resetCreatorValues ();
	}

	void resetCreatorValues ()
	{
		if (elementCreated) {

		} else {

		}

		craftingIngredientObjectInfoList.Clear ();

		repairIngredientObjectInfoList.Clear ();

		brokenIngredientObjectInfoList.Clear ();

		disassembleIngredientObjectInfoList.Clear ();

		craftingStatInfoToCraftList.Clear ();

		Debug.Log ("Set Creator window closed");
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Crafting Blueprint System", null, "Create New Crafting Blueprint");

		GUILayout.BeginVertical ("Create New Crafting Blueprint", "window");

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		windowRect = GUILayoutUtility.GetLastRect ();

		windowRect.width = this.maxSize.x;

		GUILayout.BeginHorizontal ();

		EditorGUILayout.HelpBox ("", MessageType.Info);

		style = new GUIStyle (EditorStyles.helpBox);
		style.richText = true;

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 17;

		EditorGUILayout.LabelField ("Configure the info of a new crafting blueprint, for each one of its ingredientes and other info", style);

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

		EditorGUILayout.LabelField ("CRAFTING TEMPLATE INFO", labelStyle);

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Object Category", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		objectCategory = (string)EditorGUILayout.TextField ("", objectCategory);
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		Name = (string)EditorGUILayout.TextField ("", Name);
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Amount Obtained", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		amountObtained = EditorGUILayout.IntField ("", amountObtained);
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

		EditorGUILayout.Space ();

		GUILayout.Label ("Number of Ingredients " + (craftingIngredientObjectInfoList.Count).ToString (), EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));

		EditorGUILayout.Space ();

		for (int i = 0; i < craftingIngredientObjectInfoList.Count; i++) { 

			craftingIngredientObjectInfo currentInfo = craftingIngredientObjectInfoList [i];

			GUILayout.BeginHorizontal ("box");

			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentInfo.Name = (string)EditorGUILayout.TextField ("", currentInfo.Name);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Amount Required", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentInfo.amountRequired = EditorGUILayout.IntField ("", currentInfo.amountRequired);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		
			GUILayout.BeginVertical ();
			if (GUILayout.Button ("X")) {
				craftingIngredientObjectInfoList.RemoveAt (i);
			}

			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();
		}   

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Clear Values")) {
			craftingIngredientObjectInfoList.Clear ();
		}
			
		if (GUILayout.Button ("Add Ingredient")) {
			craftingIngredientObjectInfoList.Add (new craftingIngredientObjectInfo ());
		}
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();



		GUILayout.Label ("Number of Ingredients To Repair " + (repairIngredientObjectInfoList.Count).ToString (), EditorStyles.boldLabel, GUILayout.MaxWidth (300));

		EditorGUILayout.Space ();

		for (int i = 0; i < repairIngredientObjectInfoList.Count; i++) { 

			craftingIngredientObjectInfo currentInfo = repairIngredientObjectInfoList [i];

			GUILayout.BeginHorizontal ("box");

			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentInfo.Name = (string)EditorGUILayout.TextField ("", currentInfo.Name);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Amount Required", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentInfo.amountRequired = EditorGUILayout.IntField ("", currentInfo.amountRequired);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("X")) {
				repairIngredientObjectInfoList.RemoveAt (i);
			}

			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();
		}   

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Clear Values")) {
			repairIngredientObjectInfoList.Clear ();
		}

		if (GUILayout.Button ("Add Ingredient")) {
			repairIngredientObjectInfoList.Add (new craftingIngredientObjectInfo ());
		}
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();




		GUILayout.Label ("Number of Ingredients if Broken " + (brokenIngredientObjectInfoList.Count).ToString (), EditorStyles.boldLabel, GUILayout.MaxWidth (300));

		EditorGUILayout.Space ();

		for (int i = 0; i < brokenIngredientObjectInfoList.Count; i++) { 

			craftingIngredientObjectInfo currentInfo = brokenIngredientObjectInfoList [i];

			GUILayout.BeginHorizontal ("box");

			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentInfo.Name = (string)EditorGUILayout.TextField ("", currentInfo.Name);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Amount Required", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentInfo.amountRequired = EditorGUILayout.IntField ("", currentInfo.amountRequired);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("X")) {
				brokenIngredientObjectInfoList.RemoveAt (i);
			}
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
		}   

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Clear Values")) {
			brokenIngredientObjectInfoList.Clear ();
		}

		if (GUILayout.Button ("Add Ingredient")) {
			brokenIngredientObjectInfoList.Add (new craftingIngredientObjectInfo ());
		}
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();



		GUILayout.Label ("Number of Ingredients On Disassemble " + (disassembleIngredientObjectInfoList.Count).ToString (), EditorStyles.boldLabel, GUILayout.MaxWidth (300));

		EditorGUILayout.Space ();

		for (int i = 0; i < disassembleIngredientObjectInfoList.Count; i++) { 

			craftingIngredientObjectInfo currentInfo = disassembleIngredientObjectInfoList [i];

			GUILayout.BeginHorizontal ("box");

			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentInfo.Name = (string)EditorGUILayout.TextField ("", currentInfo.Name);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Amount Required", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			currentInfo.amountRequired = EditorGUILayout.IntField ("", currentInfo.amountRequired);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("X")) {
				disassembleIngredientObjectInfoList.RemoveAt (i);
			}
				
			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();
		}   

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Clear Values")) {
			disassembleIngredientObjectInfoList.Clear ();
		}

		if (GUILayout.Button ("Add Ingredient")) {
			disassembleIngredientObjectInfoList.Add (new craftingIngredientObjectInfo ());
		}
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();



		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Check Stats Info To Craft", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		checkStatsInfoToCraft = EditorGUILayout.Toggle ("", checkStatsInfoToCraft);
		GUILayout.EndHorizontal ();

		if (checkStatsInfoToCraft) {
			GUILayout.Label ("Number of Stats " + (craftingStatInfoToCraftList.Count).ToString (), EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));

			EditorGUILayout.Space ();

			for (int i = 0; i < craftingStatInfoToCraftList.Count; i++) { 

				craftingStatInfo currentInfo = craftingStatInfoToCraftList [i];

				GUILayout.BeginHorizontal ("box");

				GUILayout.BeginVertical ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Stat Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				currentInfo.statName = (string)EditorGUILayout.TextField ("", currentInfo.statName);
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Value Required", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				currentInfo.valueRequired = EditorGUILayout.IntField ("", currentInfo.valueRequired);
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Use Stat Value", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				currentInfo.useStatValue = EditorGUILayout.Toggle ("", currentInfo.useStatValue);
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Stat Value To Use", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
				currentInfo.statValueToUse = EditorGUILayout.IntField ("", currentInfo.statValueToUse);
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				GUILayout.EndVertical ();

				GUILayout.BeginVertical ();
				if (GUILayout.Button ("X")) {
					craftingStatInfoToCraftList.RemoveAt (i);
				}

				GUILayout.EndVertical ();

				GUILayout.EndHorizontal ();
			}   

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Clear Values")) {
				craftingStatInfoToCraftList.Clear ();
			}

			if (GUILayout.Button ("Add Stat")) {
				craftingStatInfoToCraftList.Add (new craftingStatInfo ());
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Use Object To Place", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		useObjectToPlace = EditorGUILayout.Toggle ("", useObjectToPlace);
		GUILayout.EndHorizontal ();

		if (useObjectToPlace) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Object To Place", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			objectToPlace = EditorGUILayout.ObjectField (objectToPlace, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Object To Place Position Offset", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			objectToPlacePositionOffset = EditorGUILayout.Vector3Field ("", objectToPlacePositionOffset);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

//			GUILayout.BeginHorizontal ();
//			GUILayout.Label ("use Custom LayerMask To Place Object", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
//			useCustomLayerMaskToPlaceObject = EditorGUILayout.Toggle ("", useCustomLayerMaskToPlaceObject);
//			GUILayout.EndHorizontal ();
//
//			if (useCustomLayerMaskToPlaceObject) {
//
//				GUILayout.BeginHorizontal ();
//				GUILayout.Label ("use Custom LayerMask To Place Object", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
//				customLayerMaskToPlaceObject = EditorGUILayout.ObjectField (customLayerMaskToPlaceObject, typeof(LayerMask), true, GUILayout.ExpandWidth (true)) as LayerMask;
//				GUILayout.EndHorizontal ();
//
//				GUILayout.BeginHorizontal ();
//				GUILayout.Label ("LayerMask To Attach Object", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
//				layerMaskToAttachObject = EditorGUILayout.ObjectField (layerMaskToAttachObject, typeof(LayerMask), true, GUILayout.ExpandWidth (true)) as LayerMask;
//				GUILayout.EndHorizontal ();
//			}
				
//			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Object Can Be Rotated On Y Axis", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			objectCanBeRotatedOnYAxis = EditorGUILayout.Toggle ("", objectCanBeRotatedOnYAxis);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Object Can Be Rotated On X Axis", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			objectCanBeRotatedOnXAxis = EditorGUILayout.Toggle ("", objectCanBeRotatedOnXAxis);
			GUILayout.EndHorizontal ();
		}

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Craft Object In Time", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		craftObjectInTime = EditorGUILayout.Toggle ("", craftObjectInTime);
		GUILayout.EndHorizontal ();

		if (craftObjectInTime) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Time To Craft Object", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			timeToCraftObject = EditorGUILayout.FloatField ("", timeToCraftObject);
			GUILayout.EndHorizontal ();
		}

		EditorGUILayout.EndScrollView ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
	
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Crafting Blueprint Info Template Data", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		mainCraftingBlueprintInfoTemplateData = EditorGUILayout.ObjectField (mainCraftingBlueprintInfoTemplateData, 
			typeof(craftingBlueprintInfoTemplateData), true, GUILayout.ExpandWidth (true)) as craftingBlueprintInfoTemplateData;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Cancel")) {
			this.Close ();
		}
		if (GUILayout.Button ("Create Blueprint")) {
			createNewBlueprint ();
		}
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
	}

	void createNewBlueprint ()
	{
		bool checkInfoResult = true;

		if (Name == "" || craftingIngredientObjectInfoList.Count == 0) {
			checkInfoResult = false;
		}
			
		if (mainCraftingBlueprintInfoTemplateData == null) {
			Debug.Log ("The blueprint template data is empty");

			checkInfoResult = false;
		}

		if (checkInfoResult) {
			craftingBlueprintInfoTemplate newCraftingBlueprintInfoTemplate = ScriptableObject.CreateInstance<craftingBlueprintInfoTemplate> ();

			newCraftingBlueprintInfoTemplate.Name = Name;

			newCraftingBlueprintInfoTemplate.amountObtained = amountObtained;


			newCraftingBlueprintInfoTemplate.craftingIngredientObjectInfoList.AddRange (craftingIngredientObjectInfoList);

			if (repairIngredientObjectInfoList.Count >= 0) {
				newCraftingBlueprintInfoTemplate.repairIngredientObjectInfoList.AddRange (repairIngredientObjectInfoList);
			}

			if (brokenIngredientObjectInfoList.Count >= 0) {
				newCraftingBlueprintInfoTemplate.brokenIngredientObjectInfoList.AddRange (brokenIngredientObjectInfoList);
			}

			if (disassembleIngredientObjectInfoList.Count >= 0) {
				newCraftingBlueprintInfoTemplate.disassembleIngredientObjectInfoList.AddRange (disassembleIngredientObjectInfoList);
			}


			string relativePath = blueprintPrefabsPath + "/" + objectCategory;

			if (!Directory.Exists (relativePath)) {

				Directory.CreateDirectory (relativePath);
			}

			string pathNewCraftingBlueprintInfoTemplate = relativePath + "/" + Name + ".asset";

			AssetDatabase.CreateAsset (newCraftingBlueprintInfoTemplate, pathNewCraftingBlueprintInfoTemplate);

			AssetDatabase.SaveAssets ();

			AssetDatabase.Refresh ();

//			EditorUtility.FocusProjectWindow ();
		
		
			craftingBlueprintInfoTemplate currentCraftingBlueprintInfoTemplate = 
				(craftingBlueprintInfoTemplate)AssetDatabase.LoadAssetAtPath (pathNewCraftingBlueprintInfoTemplate, typeof(craftingBlueprintInfoTemplate));

			if (currentCraftingBlueprintInfoTemplate != null) {
				mainCraftingBlueprintInfoTemplateData.craftingBlueprintInfoTemplateList.Add (currentCraftingBlueprintInfoTemplate);

				EditorUtility.SetDirty (mainCraftingBlueprintInfoTemplateData);
			}

			AssetDatabase.SaveAssets ();

			AssetDatabase.Refresh ();

			Transform[] objectsOnScene = FindObjectsOfType<Transform> ();

			if (objectsOnScene.Length > 0) {
				GKC_Utils.updateDirtyScene ("Update Scene", objectsOnScene [0].gameObject);
			} else {
				GKC_Utils.updateDirtyScene ();
			}

			elementCreated = true;
		} else {
			Debug.Log ("WARNING: Make sure to set a full name and create at least one ingredient element.");
		}
	}

	void Update ()
	{
		if (elementCreated) {
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