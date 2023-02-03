using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Linq;

public class cameraCaptureSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useScreenResolution;
	public Vector2 captureResolution;

	public Color disableButtonsColor;
	public Color originalColor;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool galleryOpened;
	public int currentCaptureIndex;

	[Space]
	[Header ("Components")]
	[Space]

	public ScrollRect captureListScrollRect;
	public GameObject captureSlotPrefab;
	public Scrollbar scrollBar;

	public GameObject expandedCaptureMenu;
	public RawImage expandedCaptureImage;

	public Image expandButton;
	public Image deleteButton;

	public Camera customCameraForEditorCaptures;

	bool useRelativePath;

	string captureFolderName;

	string captureFileName;

	string currentSaveDataPath;
	bool canDelete;
	bool canExpand;

	List<captureButtonInfo> captureList = new List<captureButtonInfo> ();
	int previousCaptureAmountInFolder;
	const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
	bool checkSettingsInitialized;

	gameManager gameSystemManager;

	void Start ()
	{
		captureSlotPrefab.SetActive (false);
		changeButtonsColor (false, false);
		expandedCaptureMenu.SetActive (false);

		setMainSettings ();
	}

	public void setMainSettings ()
	{
		if (gameSystemManager == null) {
			gameSystemManager = FindObjectOfType<gameManager> ();
		}

		useRelativePath = gameSystemManager.useRelativePath;

		captureFolderName = gameSystemManager.getSaveCaptureFolder ();

		captureFileName = gameSystemManager.getSaveCaptureFileName ();
	}

	public void checkSettings ()
	{
		if (!checkSettingsInitialized) {
			currentSaveDataPath = getDataPath ();
			checkSettingsInitialized = true;
		}
	}

	public void loadCaptures ()
	{
		checkSettings ();

		int numberOfFiles = 0;
		System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo (currentSaveDataPath);
		var fileInfo = dir.GetFiles ().OrderBy (p => p.CreationTime).ToArray ();

		//if the number of pictures has changed, reload these elements
		if (previousCaptureAmountInFolder != fileInfo.Length) {
			previousCaptureAmountInFolder = fileInfo.Length;
			for (int i = 0; i < captureList.Count; i++) {		
				Destroy (captureList [i].slotGameObject);
			}

			captureList.Clear ();
			foreach (FileInfo file in fileInfo) {
				#if !UNITY_WEBPLAYER
				if (File.Exists (currentSaveDataPath + file.Name)) {
					byte[] bytes = File.ReadAllBytes (currentSaveDataPath + file.Name);
					Texture2D texture = new Texture2D ((int)captureResolution.x, (int)captureResolution.y);
					texture.filterMode = FilterMode.Trilinear;
					texture.LoadImage (bytes);
					addCaptureSlot (numberOfFiles, file.Name);
					captureList [numberOfFiles].capture.texture = texture;
					numberOfFiles++;
				}
				#endif
			}
			captureListScrollRect.verticalNormalizedPosition = 0.5f;
			scrollBar.value = 1;
		}
	}

	public void getSaveButtonSelected (Button button)
	{
		currentCaptureIndex = -1;
		bool delete = false;
		bool expand = false;
		for (int i = 0; i < captureList.Count; i++) {		
			if (captureList [i].button == button) {
				currentCaptureIndex = i;	
				delete = true;
				expand = true;
			}
		}
		print (delete + " " + expand);
		changeButtonsColor (delete, expand);
	}

	public void addCaptureSlot (int index, string fileName)
	{
		GameObject newSlotPrefab = (GameObject)Instantiate (captureSlotPrefab, captureSlotPrefab.transform.position,
			                           captureSlotPrefab.transform.rotation, captureSlotPrefab.transform.parent);

		if (!newSlotPrefab.activeSelf) {
			newSlotPrefab.SetActive (true);
		}

		newSlotPrefab.transform.localScale = Vector3.one;
		newSlotPrefab.name = "Capture Slot " + (index + 1);
		captureSlot newCaptureSlot = newSlotPrefab.GetComponent<captureSlot> ();
		newCaptureSlot.captureInfo.fileName = fileName;
		captureList.Add (newCaptureSlot.captureInfo);
	}

	public void changeButtonsColor (bool delete, bool expand)
	{
		if (delete) {
			deleteButton.color = originalColor;
		} else {
			deleteButton.color = disableButtonsColor;
		}
	
		if (expand) {
			expandButton.color = originalColor;
		} else {
			expandButton.color = disableButtonsColor;
		}

		canDelete = delete;

		canExpand = expand;
	}

	public void openCaptureGallery ()
	{
		openOrCloseCapturesGallery (true);
	}

	public void closeCaptureGallery ()
	{
		openOrCloseCapturesGallery (false);
	}

	public void openOrCloseCapturesGallery (bool state)
	{
		galleryOpened = state;
		if (galleryOpened) {
			loadCaptures ();
		} else {
			changeButtonsColor (false, false);
			expandedCaptureMenu.SetActive (false);
		}
	}

	public string getDataPath ()
	{
		string dataPath = "";
		if (useRelativePath) {
			dataPath = captureFolderName;
		} else {
			dataPath = Application.persistentDataPath + "/" + captureFolderName;
		}

		if (!Directory.Exists (dataPath)) {
			Directory.CreateDirectory (dataPath);
		}

		dataPath += "/";

		return dataPath;
	}

	public void takeCapture (Camera currentCamera)
	{
		checkSettings ();

		// get the camera's render texture
		RenderTexture previousRenderTexture = currentCamera.targetTexture;

		Vector2 currentResolution = captureResolution;

		if (useScreenResolution) {
			currentResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);
		}

		currentCamera.targetTexture = new RenderTexture ((int)currentResolution.x, (int)currentResolution.y, 24);
		RenderTexture rendText = RenderTexture.active;
		RenderTexture.active = currentCamera.targetTexture;

		//render the texture
		currentCamera.Render ();
		//create a new Texture2D with the camera's texture, using its height and width
		Texture2D cameraImage = new Texture2D ((int)currentResolution.x, (int)currentResolution.y, TextureFormat.RGB24, false);
		cameraImage.ReadPixels (new Rect (0, 0, (int)currentResolution.x, (int)currentResolution.y), 0, 0);
		cameraImage.Apply ();
		RenderTexture.active = rendText;
		//store the texture into a .PNG file
		#if !UNITY_WEBPLAYER
		byte[] bytes = cameraImage.EncodeToPNG ();

		int numberOfFiles = 0;
		System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo (currentSaveDataPath);
		if (dir.Exists) {
			numberOfFiles = dir.GetFiles ().Length;
		}
			
		currentCamera.targetTexture = previousRenderTexture;
		RenderTexture.active = currentCamera.targetTexture;

		string randomString = "";

		int charAmount = UnityEngine.Random.Range (10, 20); //set those to the minimum and maximum length of your string
		for (int i = 0; i < charAmount; i++) {
			randomString += glyphs [UnityEngine.Random.Range (0, glyphs.Length)];
		}

		if (File.Exists (currentSaveDataPath + captureFileName + "_" + randomString + ".png")) {
			randomString += glyphs [UnityEngine.Random.Range (0, glyphs.Length)];
		}
		//save the encoded image to a file
		System.IO.File.WriteAllBytes (currentSaveDataPath + (captureFileName + "_" + randomString + ".png"), bytes);
		#endif
	}

	public void takeCaptureWithCameraEditor ()
	{
		setMainSettings ();

		takeCapture (customCameraForEditorCaptures);
	}

	public void deleteCapture ()
	{
		checkSettings ();

		if (currentCaptureIndex != -1 && canDelete) {

			string fileName = captureList [currentCaptureIndex].fileName;
			if (File.Exists (currentSaveDataPath + fileName)) {
				File.Delete (currentSaveDataPath + fileName);
			}

			destroyCaptureSlot (currentCaptureIndex);

			currentCaptureIndex = -1;

			changeButtonsColor (false, false);

			captureListScrollRect.verticalNormalizedPosition = 0.5f;

			scrollBar.value = 1;
		}
	}

	public void deleteExpandedCapture ()
	{
		checkSettings ();

		print (currentCaptureIndex);

		if (currentCaptureIndex != -1) {

			string fileName = captureList [currentCaptureIndex].fileName;
			if (File.Exists (currentSaveDataPath + fileName)) {
				File.Delete (currentSaveDataPath + fileName);
			}

			destroyCaptureSlot (currentCaptureIndex);
		
			if (captureList.Count > 0) {
				if ((currentCaptureIndex + 1) < captureList.Count) {
					expandNextCapture ();
				} else {
					expandPreviousCapture ();
				}
			} else {
				expandedCaptureMenu.SetActive (false);
				closeExpandCaptureMenu ();
			}

			captureListScrollRect.verticalNormalizedPosition = 0.5f;

			scrollBar.value = 1;
		}
	}

	public void expandCapture ()
	{
		if (currentCaptureIndex != -1 && canExpand) {
			expandedCaptureMenu.SetActive (true);

			expandedCaptureImage.texture = captureList [currentCaptureIndex].capture.texture;
		}
	}

	public void expandNextCapture ()
	{
		if (galleryOpened) {
			currentCaptureIndex++;

			if (currentCaptureIndex >= captureList.Count) {
				currentCaptureIndex = 0;
			}

			expandedCaptureImage.texture = captureList [currentCaptureIndex].capture.texture;
		}
	}

	public void expandPreviousCapture ()
	{
		if (galleryOpened) {
			currentCaptureIndex--;

			if (currentCaptureIndex < 0) {
				currentCaptureIndex = captureList.Count - 1;
			}

			expandedCaptureImage.texture = captureList [currentCaptureIndex].capture.texture;
		}
	}

	public void closeExpandCaptureMenu ()
	{
		changeButtonsColor (false, false);
	}

	public void destroyCaptureSlot (int slotIndex)
	{
		Destroy (captureList [slotIndex].slotGameObject);
		captureList.RemoveAt (slotIndex);
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class captureButtonInfo
	{
		public GameObject slotGameObject;
		public Button button;
		public RawImage capture;
		public string fileName;
	}
}
