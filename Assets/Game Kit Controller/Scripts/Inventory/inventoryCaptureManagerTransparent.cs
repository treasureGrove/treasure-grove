using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class inventoryCaptureManagerTransparent : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string fileName = "New Capture";
	public string relativePathCaptures = "";

	public string extraName = "(Inventory Capture)";

	public Transform objectToCaptureParent;
	public bool useObjectName;

	[Space]
	[Header ("Others")]
	[Space]

	[TextArea (10, 15)] public string explanation;

	GameObject whiteCamGameObject;
	Camera whiteCam;
	GameObject blackCamGameObject;
	Camera blackCam;
	Camera mainCam;

	int screenWidth;
	int screenHeight;

	bool done = false;

	Texture2D textureBlack;
	Texture2D textureWhite;
	Texture2D textureTransparentBackground;

	[MenuItem ("Game Kit Controller/Go To Capture Inventory Transparent Icon Manager Scene", false, 23)]
	static void openInventoryCaptureManager ()
	{
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();

		string ransparentInventoryCaptureToolScenePath = pathInfoValues.getTransparentInventoryCaptureToolScenePath ();

		EditorSceneManager.OpenScene (ransparentInventoryCaptureToolScenePath);
	}

	public void takeCapture ()
	{
		UnityEditor.EditorApplication.isPlaying = true;
	}

	void Awake ()
	{
		if (useObjectName && objectToCaptureParent) {
			foreach (Transform child in objectToCaptureParent) {
				if (child.gameObject.activeSelf) {
					print (child.name);
					fileName = child.name;
				}
			}
		}

		mainCam = gameObject.GetComponent<Camera> ();

		CreateBlackAndWhiteCameras ();

		CacheAndInitialiseFields ();
	}

	void LateUpdate ()
	{
		if (!done) {
			StartCoroutine (CaptureFrame ());
		}
	}

	IEnumerator CaptureFrame ()
	{
		yield return new WaitForEndOfFrame ();

		RenderCamToTexture (blackCam, textureBlack);
		RenderCamToTexture (whiteCam, textureWhite);

		CalculateOutputTexture ();

		SavePng ();

		done = true;

		StopCoroutine ("CaptureFrame");

		GKC_Utils.refreshAssetDatabase ();

		UnityEditor.EditorApplication.isPlaying = false;

		yield return null;
	}

	void RenderCamToTexture (Camera cam, Texture2D tex)
	{
		cam.enabled = true;

		cam.Render ();

		WriteScreenImageToTexture (tex);

		cam.enabled = false;
	}

	void CreateBlackAndWhiteCameras ()
	{
		whiteCamGameObject = (GameObject)new GameObject ();

		whiteCamGameObject.name = "White Background Camera";

		whiteCam = whiteCamGameObject.AddComponent<Camera> ();

		whiteCam.CopyFrom (mainCam);

		whiteCam.backgroundColor = Color.white;

		whiteCamGameObject.transform.SetParent (gameObject.transform, true);

		blackCamGameObject = (GameObject)new GameObject ();

		blackCamGameObject.name = "Black Background Camera";

		blackCam = blackCamGameObject.AddComponent<Camera> ();

		blackCam.CopyFrom (mainCam);

		blackCam.backgroundColor = Color.black;

		blackCamGameObject.transform.SetParent (gameObject.transform, true);
	}

	void WriteScreenImageToTexture (Texture2D tex)
	{
		tex.ReadPixels (new Rect (0, 0, screenWidth, screenHeight), 0, 0);

		tex.Apply ();
	}

	void CalculateOutputTexture ()
	{
		Color color;

		for (int y = 0; y < textureTransparentBackground.height; ++y) {
			// each row
			for (int x = 0; x < textureTransparentBackground.width; ++x) {
				// each column
				float alpha = textureWhite.GetPixel (x, y).r - textureBlack.GetPixel (x, y).r;
				alpha = 1.0f - alpha;

				if (alpha == 0) {
					color = Color.clear;
				} else {
					color = textureBlack.GetPixel (x, y) / alpha;
				}

				color.a = alpha;

				textureTransparentBackground.SetPixel (x, y, color);
			}
		}
	}

	void SavePng ()
	{
		relativePathCaptures = pathInfoValues.getInventoryInfoCapturesPath (); 
			
		string name = string.Format ("{0}/{1:D04} ", relativePathCaptures, (fileName + " " + extraName + ".png"));

		var pngShot = textureTransparentBackground.EncodeToPNG ();

		if (Directory.Exists (relativePathCaptures)) {
			File.WriteAllBytes (name, pngShot);
		} else {
			print ("WARNING: The path configured for the capture doesn't exist, make sure the folder is created");
		}
	}

	void CacheAndInitialiseFields ()
	{
		screenWidth = Screen.width;
		screenHeight = Screen.height;

		textureBlack = new Texture2D (screenWidth, screenHeight, TextureFormat.RGB24, false);
		textureWhite = new Texture2D (screenWidth, screenHeight, TextureFormat.RGB24, false);
		textureTransparentBackground = new Texture2D (screenWidth, screenHeight, TextureFormat.ARGB32, false);
	}
}
#endif