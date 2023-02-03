using UnityEngine;
using System.Collections;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;

using UnityEditor.SceneManagement;

public class inventoryCaptureManager : EditorWindow
{
	public Vector2 captureResolution = new Vector2 (512, 512);
	public Vector3 rotationOffset;
	public Vector3 positionOffset;
	public string fileName = "New Capture";

	public string inventoryLayerName = "inventory";

	public string mainInventoryManagerName = "Main Inventory Manager";

	bool checkCapturePath;
	string currentSaveDataPath;

	public inventoryListManager mainInventoryListManager;

	Camera inventoryCamera;
	static inventoryCaptureManager window;
	inventoryInfo currentInventoryInfo;
	GameObject currentInventoryObjectMesh;

	Rect positionRect;

	Rect renderRect;
	Transform inventoryLookObjectTransform;
	RenderTexture originalRenderTexture;
	Texture2D captureFile;

	GUISkin guiSkin;
	Rect windowRect = new Rect ();

	GameObject inventoryCameraParentGameObject;

	Color backGroundColor = Color.white;
	Color originalBackGroundColor;
	float cameraFov;
	float originalCameraFov;

	bool objectMeshLayerAssignedProperly;

	Vector2 rectSize = new Vector2 (550, 720);

	GUIStyle style = new GUIStyle ();

	Vector2 screenResolution;

	float windowHeightPercentage = 0.65f;

	float maxLayoutWidht = 180;

	Vector2 scrollPos1;

	static void ShowWindow ()
	{
		window = (inventoryCaptureManager)EditorWindow.GetWindow (typeof(inventoryCaptureManager));
		window.init ();
	}

	public void init ()
	{
		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		float windowHeight = screenResolution.y * windowHeightPercentage;

		windowHeight = Mathf.Clamp (windowHeight, 500, 800);

		rectSize = new Vector2 (550, windowHeight);

		GKC_Utils.instantiateMainManagerOnSceneWithType (mainInventoryManagerName, typeof(inventoryListManager));

		mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

		inventoryCamera = mainInventoryListManager.inventoryCamera;

		inventoryCamera.enabled = true;

		inventoryCameraParentGameObject = mainInventoryListManager.inventoryCameraParentGameObject;

		if (inventoryCameraParentGameObject != null) {
			inventoryCameraParentGameObject.SetActive (true);
		}

		inventoryLookObjectTransform = mainInventoryListManager.lookObjectsPosition;
		captureFile = null;
		checkCapturePath = false;

		originalBackGroundColor = Color.white;
		originalCameraFov = inventoryCamera.fieldOfView;
		cameraFov = originalCameraFov;

		mainInventoryListManager.setInventoryCaptureToolOpenState (true);
	}

	public void OnDisable ()
	{
		if (currentInventoryObjectMesh != null) {
			destroyAuxObjects ();
		}

		if (inventoryCamera != null) {
			inventoryCamera.backgroundColor = originalBackGroundColor;
			inventoryCamera.fieldOfView = originalCameraFov;

			inventoryCamera.enabled = false;
		}

		if (mainInventoryListManager != null) {
			mainInventoryListManager.setInventoryCaptureToolOpenState (false);
		}

		if (inventoryCameraParentGameObject != null) {
			inventoryCameraParentGameObject.SetActive (false);
		}
	}

	void OnGUI ()
	{
		if (window == null) {
			window = (inventoryCaptureManager)EditorWindow.GetWindow (typeof(inventoryCaptureManager));
		}

		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Inventory Object Capture Tool", null, "You can create inventory objects prefabs with this tool");

		GUILayout.BeginVertical ("Inventory Object Capture Tool", "window", GUILayout.Width (530));

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("box");

		scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Capture Resolution", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		captureResolution = EditorGUILayout.Vector2Field ("", captureResolution);
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("File Name", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		fileName = EditorGUILayout.TextField (fileName, GUILayout.ExpandWidth (true));
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.Label ("Rotation Offset", EditorStyles.boldLabel);
		rotationOffset.x = EditorGUILayout.Slider (rotationOffset.x, -360, 360);
		rotationOffset.y = EditorGUILayout.Slider (rotationOffset.y, -360, 360);
		rotationOffset.z = EditorGUILayout.Slider (rotationOffset.z, -360, 360);

		EditorGUILayout.Space ();

		GUILayout.Label ("Position Offset", EditorStyles.boldLabel);
		positionOffset.x = EditorGUILayout.Slider (positionOffset.x, -5, 5);
		positionOffset.y = EditorGUILayout.Slider (positionOffset.y, -5, 5);
		positionOffset.z = EditorGUILayout.Slider (positionOffset.z, -5, 5);


		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		backGroundColor = EditorGUILayout.ColorField ("Background Color", backGroundColor);
		inventoryCamera.backgroundColor = backGroundColor;

		cameraFov = EditorGUILayout.FloatField ("Camera FOV", cameraFov);
		inventoryCamera.fieldOfView = cameraFov;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Cancel")) {
			this.Close ();
		}

		if (GUILayout.Button ("Reset View")) {
			rotationOffset = Vector3.zero;
			positionOffset = Vector3.zero;

			cameraFov = originalCameraFov;
			backGroundColor = originalBackGroundColor;
		}

		if (GUILayout.Button ("Get Capture")) {
			getCapture ();
		}
		GUILayout.EndHorizontal ();

		EditorGUILayout.EndScrollView ();

		positionRect = position;

		windowRect = GUILayoutUtility.GetLastRect ();
//		windowRect.position = new Vector2 (0, windowRect.position.y);
		windowRect.width = this.maxSize.x;

		GUILayout.FlexibleSpace ();

		if (currentInventoryObjectMesh != null) {

			if (currentInventoryObjectMesh != null) {
				currentInventoryObjectMesh.transform.localRotation = Quaternion.Euler (rotationOffset);
				currentInventoryObjectMesh.transform.localPosition = positionOffset;
			}

			if (objectMeshLayerAssignedProperly) {
				if (inventoryCamera != null) {       
					inventoryCamera.Render ();

					renderRect = new Rect (positionRect.width / 4.5f, 370, positionRect.width / 1.9f, positionRect.width / 1.9f);

					GUI.DrawTexture (renderRect, inventoryCamera.targetTexture);       
				}
			} else {
				GUILayout.BeginHorizontal ();
				EditorGUILayout.HelpBox ("", MessageType.Info);

				style = new GUIStyle (EditorStyles.helpBox);
				style.richText = true;

				style.fontStyle = FontStyle.Bold;
				style.fontSize = 14;

				EditorGUILayout.LabelField ("WARNING: Object not visible on the icon preview.\n\n" +
				"The layer of the current object mesh selected is not configured as " + inventoryLayerName + ".\n\n" +
				"Close this window and make sure to apply that layer to the object mesh and confirm to set it to its childs when unity asks about it.", style);
				GUILayout.EndHorizontal ();

				if (GUILayout.Button ("Close Window")) {
					closeWindow ();
				}
			}
		}

		GUILayout.FlexibleSpace ();

		GUILayout.EndVertical ();
	
		GUILayout.EndVertical ();
	}

	public void getCapture ()
	{
		if (currentInventoryObjectMesh == null) {
			Debug.Log ("Please, close this window, assign the Inventory Object Mesh and open this window again to take the capture");
			return;
		}

		if (fileName.Equals ("")) {
			fileName = currentInventoryInfo.Name;
		}
			
		originalRenderTexture = inventoryCamera.targetTexture;
		inventoryCamera.targetTexture = new RenderTexture ((int)captureResolution.x, (int)captureResolution.y, 24);
		RenderTexture rendText = RenderTexture.active;
		RenderTexture.active = inventoryCamera.targetTexture;

		// render the texture
		inventoryCamera.Render ();

		// create a new Texture2D with the camera's texture, using its height and width
		Texture2D cameraImage = new Texture2D ((int)captureResolution.x, (int)captureResolution.y, TextureFormat.RGB24, false);
		cameraImage.ReadPixels (new Rect (0, 0, (int)captureResolution.x, (int)captureResolution.y), 0, 0);

		cameraImage.Apply ();

		RenderTexture.active = rendText;

		// store the texture into a .PNG file
		#if !UNITY_WEBPLAYER
		byte[] bytes = cameraImage.EncodeToPNG ();

		// save the encoded image to a file
		System.IO.File.WriteAllBytes (currentSaveDataPath + (fileName + " (Inventory Capture).png"), bytes);
		inventoryCamera.targetTexture = originalRenderTexture;
		#endif

		GKC_Utils.refreshAssetDatabase ();

		checkCapturePath = true;

		inventoryCamera.backgroundColor = originalBackGroundColor;
		inventoryCamera.fieldOfView = originalCameraFov;

		inventoryCamera.enabled = false;
	}

	void Update ()
	{
		if (checkCapturePath) {
			captureFile = (Texture2D)AssetDatabase.LoadAssetAtPath ((currentSaveDataPath + fileName + " (Inventory Capture).png"), typeof(Texture2D));

			if (captureFile != null) {
				mainInventoryListManager.setInventoryCaptureIcon (currentInventoryInfo, captureFile);

				checkCapturePath = false;

				closeWindow ();
			}
		}
	}

	public void closeWindow ()
	{
		if (currentInventoryObjectMesh != null) {
			destroyAuxObjects ();
		}

		window.Close ();
	}

	public void destroyAuxObjects ()
	{
		DestroyImmediate (currentInventoryObjectMesh);
	}

	public void setCurrentInventoryObjectInfo (inventoryInfo info, string savePath)
	{
		currentInventoryInfo = info;

		if (currentInventoryInfo.inventoryGameObject != null) {
			currentInventoryObjectMesh = (GameObject)Instantiate (info.inventoryGameObject, inventoryLookObjectTransform.position, Quaternion.identity);
			currentInventoryObjectMesh.transform.SetParent (inventoryLookObjectTransform);
			currentSaveDataPath = savePath;

			objectMeshLayerAssignedProperly = (LayerMask.NameToLayer (inventoryLayerName) == currentInventoryObjectMesh.layer);
		} else {
			Debug.Log ("Please, assign the Inventory Object Mesh to take the capture");
		}
	}
}
#endif