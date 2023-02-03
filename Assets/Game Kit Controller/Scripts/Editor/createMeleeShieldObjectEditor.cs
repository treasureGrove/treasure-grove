using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class createMeleeShieldObjectEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();

	Vector2 rectSize = new Vector2 (550, 600);

	bool objectCreated;

	float timeToBuild = 0.2f;
	float timer;

	GUIStyle style = new GUIStyle ();

	float windowHeightPercentage = 0.35f;

	Vector2 screenResolution;

	GameObject newObjectMesh;

	Vector2 scrollPos1;

	[Range (0.01f, 5)] float newObjectMeshScale = 1;

	static string editorTitle = "Create Melee Shield";

	static string editorDescription = "Create New Melee Shield";

	static string editorSecondaryTitle = "Create New Melee Shield";

	static string editorInstructions = "Select an object mesh to be used for the new melee shield.";

	bool addObjectToInventory = true;

	public string relativePathInventoryObject = "";

	public string relativePathMeleeShield = "";

	Texture iconTexture;

	Vector2 previousRectSize;

	float minHeight;

	string currentObjectName = "New Shield";

	string weaponInventoryDescription;

	[MenuItem ("Game Kit Controller/Create New Weapon/Create New Shield", false, 201)]
	public static void createPhysicalObjectToGrab ()
	{
		createMeleeShieldObjectEditor editorWindow = EditorWindow.GetWindow<createMeleeShieldObjectEditor> ();

		editorWindow.Init ();
	}

	public void Init ()
	{
		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		float totalHeight = screenResolution.y * windowHeightPercentage;

		if (totalHeight < 400) {
			totalHeight = 400;
		}

		minHeight = totalHeight;

		rectSize = new Vector2 (550, totalHeight);

		resetCreatorValues ();
	}

	void OnDisable ()
	{
		resetCreatorValues ();
	}

	void resetCreatorValues ()
	{
		if (objectCreated) {

		} else {

		}

		objectCreated = false;

		newObjectMeshScale = 1;

		currentObjectName = "New Shield";

		Debug.Log ("Object To Grab window closed");
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent (editorTitle, null, editorDescription);

		GUILayout.BeginVertical (editorSecondaryTitle, "window");

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

		EditorGUILayout.LabelField (editorInstructions, style);
		GUILayout.EndHorizontal ();

		GUILayout.FlexibleSpace ();

		EditorGUILayout.Space ();

		scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Shield Name", EditorStyles.boldLabel, GUILayout.MaxWidth (150));
		currentObjectName = (string)EditorGUILayout.TextField (currentObjectName); 
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("New Object Mesh", EditorStyles.boldLabel, GUILayout.MaxWidth (150));
		newObjectMesh = EditorGUILayout.ObjectField (newObjectMesh, typeof(GameObject), true, GUILayout.ExpandWidth (true)) as GameObject;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Object Mesh Scale", EditorStyles.boldLabel);
		newObjectMeshScale = EditorGUILayout.Slider (newObjectMeshScale, 0.01f, 5);
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Add Shield To\nInventory", EditorStyles.boldLabel);
		addObjectToInventory = (bool)EditorGUILayout.Toggle ("", addObjectToInventory);
		GUILayout.EndHorizontal ();

		if (addObjectToInventory) {
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Inventory Object Icon", EditorStyles.boldLabel, GUILayout.MaxWidth (150));
			iconTexture = EditorGUILayout.ObjectField (iconTexture, typeof(Texture), true, GUILayout.ExpandWidth (true)) as Texture;
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Weapon Description: ", EditorStyles.boldLabel, GUILayout.MaxWidth (150));
			weaponInventoryDescription = EditorGUILayout.TextArea (weaponInventoryDescription, EditorStyles.textArea, GUILayout.Height (40), GUILayout.ExpandWidth (true));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();

		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Window Height", EditorStyles.boldLabel, GUILayout.MaxWidth (150));

		if (previousRectSize != rectSize) {
			previousRectSize = rectSize;

			this.maxSize = rectSize;
		}

		rectSize.y = EditorGUILayout.Slider (rectSize.y, minHeight, screenResolution.y);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.EndScrollView ();

		EditorGUILayout.Space ();

		if (newObjectMesh != null) {
			if (GUILayout.Button ("Create Object")) {
				createObject ();
			}
		}

		if (GUILayout.Button ("Cancel")) {
			this.Close ();
		}

		GUILayout.EndVertical ();
	}

	void createObject ()
	{
		if (newObjectMesh != null) {
			GameObject newObjectCreated = new GameObject ();

			Transform newObjectCreatedTransform = newObjectCreated.transform;

			GameObject newShieldObject = (GameObject)Instantiate (newObjectMesh, Vector3.zero, Quaternion.identity, newObjectCreatedTransform);

			newShieldObject.transform.localPosition = Vector3.zero;
			newShieldObject.transform.localRotation = Quaternion.identity;

			newShieldObject.name = "Mesh";

			newObjectCreated.name = currentObjectName;

			objectCreated = true;

			newObjectCreated.transform.localScale = Vector3.one * newObjectMeshScale;

			relativePathMeleeShield = pathInfoValues.getMeleeShieldObjectPath ();

			GameObject newObjectCreatedPrefab = GKC_Utils.createPrefab (relativePathMeleeShield, currentObjectName, newObjectCreated);

			if (newObjectCreatedPrefab != null) {
				meleeWeaponsGrabbedManager[] meleeWeaponsGrabbedManagerList = FindObjectsOfType<meleeWeaponsGrabbedManager> ();

				foreach (meleeWeaponsGrabbedManager currentMeleeWeaponsGrabbedManager in meleeWeaponsGrabbedManagerList) {
					currentMeleeWeaponsGrabbedManager.addNewMeleeShieldPrefab (newObjectCreatedPrefab, currentObjectName);
				}
			}

			if (addObjectToInventory) {
				relativePathInventoryObject = pathInfoValues.getInventoryMeshShieldPath ();

				GKC_Utils.createInventoryObject (currentObjectName, "Melee Shields", newObjectCreated, weaponInventoryDescription, iconTexture, relativePathInventoryObject, true, true, false);
			}

			GKC_Utils.setActiveGameObjectInEditor (newObjectCreated);

			Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

			if (currentCameraEditor != null) {
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;

				RaycastHit hit;

				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity)) {
					newObjectCreated.transform.position = hit.point + Vector3.up * 0.2f;
				}
			}

			GKC_Utils.updateDirtyScene ("Create Shield", newObjectCreated);
		}
	}

	void Update ()
	{
		if (objectCreated) {
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
