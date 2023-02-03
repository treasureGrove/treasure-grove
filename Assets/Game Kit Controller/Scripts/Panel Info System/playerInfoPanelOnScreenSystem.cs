using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerInfoPanelOnScreenSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]
		
	public bool panelOnScreenEnabled = true;

	public bool useFadePanel;
	public float fadeOnPanelSpeed;
	public float fadeOffPanelSpeed;

	public string actionNameField = "-ACTION NAME-";

	[Space]
	[Header ("Panel GameObject List")]
	[Space]

	public List<infoPanel> panelGameObjectList = new List<infoPanel> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public List<Transform> panelTransformFoundList = new List<Transform> ();
	public List<infoPanelOnScreenSystem> infoPanelOnScreenSystemList = new List<infoPanelOnScreenSystem> ();

	[Space]
	[Header ("Components")]
	[Space]

	public Camera mainCamera;

	public playerCamera mainPlayerCamera;

	public playerInputManager playerInput;


	Text panelText;

	RawImage currentPanelImage;

	Text currentPanelNameText;

	GameObject panelGameObject;

	RectTransform panelRectTransform;

	bool useFixedPanelPosition;

	Transform fixedPanelPosition;

	bool useSeparatedTransformForEveryView;
	Transform transformForThirdPerson;
	Transform transformForFirstPerson;

	Vector3 panelOffset;

	bool targetOnScreen;
	Vector2 mainCanvasSizeDelta;
	Vector2 halfMainCanvasSizeDelta;

	Vector2 iconPosition2d;
	bool usingScreenSpaceCamera;

	bool showPanelInfoActive;

	Vector3 screenPoint;

	Vector3 currentPanelPosition;

	Transform objectToFollow;

	infoPanel currentInfoPanel;

	float screenWidth;
	float screenHeight;

	void Start ()
	{	
		mainCanvasSizeDelta = mainPlayerCamera.getMainCanvasSizeDelta ();
		halfMainCanvasSizeDelta = mainCanvasSizeDelta * 0.5f;
		usingScreenSpaceCamera = mainPlayerCamera.isUsingScreenSpaceCamera ();
	}

	Coroutine updateCoroutine;

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForSecondsRealtime (0.0001f);

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}


	void updateSystem ()
	{
		if (!panelOnScreenEnabled) {
			return;
		}

		if (showPanelInfoActive) {
			if (!useFixedPanelPosition) {
				calculatePanelPosition ();

				if (targetOnScreen) {
					enableOrDisablePanel (true);
				} else {
					enableOrDisablePanel (false);
				}
			} else {
				if (objectToFollow == null) {
					disablePanelInfo (null);
				}
			}
		}
	}

	public void calculatePanelPosition ()
	{
		if (objectToFollow != null) {
			currentPanelPosition = objectToFollow.position;

			if (useSeparatedTransformForEveryView) {
				if (mainPlayerCamera.isFirstPersonActive ()) {
					currentPanelPosition = transformForFirstPerson.position;
				} else {
					currentPanelPosition = transformForThirdPerson.position;
				}
			}

			currentPanelPosition += panelOffset;

			if (!usingScreenSpaceCamera) {
				screenWidth = Screen.width;
				screenHeight = Screen.height;
			}

			if (usingScreenSpaceCamera) {
				screenPoint = mainCamera.WorldToViewportPoint (currentPanelPosition);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
			} else {
				screenPoint = mainCamera.WorldToScreenPoint (currentPanelPosition);
				targetOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < screenWidth && screenPoint.y > 0 && screenPoint.y < screenHeight;
			}
			
			if (usingScreenSpaceCamera) {
				iconPosition2d = new Vector2 ((screenPoint.x * mainCanvasSizeDelta.x) - halfMainCanvasSizeDelta.x, (screenPoint.y * mainCanvasSizeDelta.y) - halfMainCanvasSizeDelta.y);

				panelRectTransform.anchoredPosition = iconPosition2d;
			} else {
				panelGameObject.transform.position = new Vector3 (screenPoint.x, screenPoint.y, 0);
			}
		} else {
			disablePanelInfo (null);
		}
	}

	public void getNewPanelInfo (infoPanelOnScreenSystem newInfoPanelOnScreenSystem)
	{
		if (!panelOnScreenEnabled || newInfoPanelOnScreenSystem == null) {
			return;
		}
			
		if (mainCamera == null) {
			mainCamera = mainPlayerCamera.getMainCamera ();
		}

		useSeparatedTransformForEveryView = newInfoPanelOnScreenSystem.useSeparatedTransformForEveryView;
		transformForThirdPerson = newInfoPanelOnScreenSystem.transformForThirdPerson;
		transformForFirstPerson = newInfoPanelOnScreenSystem.transformForFirstPerson;

		objectToFollow = newInfoPanelOnScreenSystem.objectToFollow;

		string newPanelText = newInfoPanelOnScreenSystem.panelOnScreenText;

		if (newPanelText.Contains (actionNameField)) {
			string keyAction = playerInput.getButtonKey (newInfoPanelOnScreenSystem.includedActionNameOnText);
			newPanelText = newPanelText.Replace (actionNameField, keyAction);
		}

		if (!panelTransformFoundList.Contains (objectToFollow)) {
			panelTransformFoundList.Add (objectToFollow);

			infoPanelOnScreenSystemList.Add (newInfoPanelOnScreenSystem);
		}

		if (panelGameObject != null && panelGameObject.activeSelf) {
			panelGameObject.SetActive (false);
		}

		useFixedPanelPosition = newInfoPanelOnScreenSystem.useFixedPanelPosition;

		bool panelFound = false;

		for (int i = 0; i < panelGameObjectList.Count; i++) {
			if (panelGameObjectList [i].Name.Equals (newInfoPanelOnScreenSystem.panelName)) {
				panelText = panelGameObjectList [i].panelText;

				panelGameObject = panelGameObjectList [i].panelGameObject;

				panelRectTransform = panelGameObjectList [i].panelRectTransform;

				panelGameObjectList [i].isActive = true;

				fixedPanelPosition = panelGameObjectList [i].fixedPanelPosition;

				currentPanelImage = panelGameObjectList [i].panelImage;

				currentPanelNameText = panelGameObjectList [i].panelNameText;

				panelFound = true;

				currentInfoPanel = panelGameObjectList [i];
			} else {
				panelGameObjectList [i].isActive = false;
			}
		}

		if (!panelFound) {
			print ("WARNING: No panel with the name " + newInfoPanelOnScreenSystem.panelName + " has been found, make sure to configure a panel with that name");

			return;
		}

		panelText.text = newPanelText;

		panelOffset = newInfoPanelOnScreenSystem.panelOffset;

		if (currentPanelImage != null) {
			if (currentPanelImage.gameObject.activeSelf != newInfoPanelOnScreenSystem.setImageOnPanel) {
				currentPanelImage.gameObject.SetActive (newInfoPanelOnScreenSystem.setImageOnPanel);
			}

			if (newInfoPanelOnScreenSystem.setImageOnPanel) {
				currentPanelImage.texture = newInfoPanelOnScreenSystem.imageOnPanel;
			}
		}

		if (currentPanelNameText != null) {
			if (currentPanelNameText.gameObject.activeSelf != newInfoPanelOnScreenSystem.usePanelNameText) {
				currentPanelNameText.gameObject.SetActive (newInfoPanelOnScreenSystem.usePanelNameText);
			}

			if (newInfoPanelOnScreenSystem.usePanelNameText) {
				currentPanelNameText.text = newInfoPanelOnScreenSystem.panelNameText;
			}
		}

		if (useFixedPanelPosition) {
			panelGameObject.transform.position = fixedPanelPosition.position;

			if (useFadePanel) {
				fadePanel (currentInfoPanel, false);
			} 

			enableOrDisablePanel (true);
		} else {

			calculatePanelPosition ();

			LayoutRebuilder.ForceRebuildLayoutImmediate (panelRectTransform);
			LayoutRebuilder.MarkLayoutForRebuild (panelRectTransform);

			calculatePanelPosition ();

			panelGameObject.SetActive (true);
			panelGameObject.SetActive (false);

			if (useFadePanel) {
				fadePanel (currentInfoPanel, false);
			}
		}

		showPanelInfoActive = true;

		stopUpdateCoroutine ();

		updateCoroutine = StartCoroutine (updateSystemCoroutine ());
	}

	public void disablePanelInfo (Transform newObjectToFollow)
	{
		for (int i = panelTransformFoundList.Count - 1; i >= 0; i--) {
			if (panelTransformFoundList [i] == null) {
				panelTransformFoundList.RemoveAt (i);
			}
		}

		for (int i = infoPanelOnScreenSystemList.Count - 1; i >= 0; i--) {
			if (infoPanelOnScreenSystemList [i] == null) {
				infoPanelOnScreenSystemList.RemoveAt (i);
			}
		}

		if (newObjectToFollow != null) {
			if (panelTransformFoundList.Contains (newObjectToFollow)) {
				panelTransformFoundList.Remove (newObjectToFollow);

				int panelIndex = infoPanelOnScreenSystemList.FindIndex (s => s.objectToFollow == newObjectToFollow);

				if (panelIndex > -1) {
					infoPanelOnScreenSystemList.RemoveAt (panelIndex);
				}
			}
		}

		if (panelTransformFoundList.Count == 0) {
			showPanelInfoActive = false;

			stopUpdateCoroutine ();

			if (useFadePanel) {
				fadePanel (currentInfoPanel, true);
			} else {
				enableOrDisablePanel (false);
			}
		} else {
			getNewPanelInfo (infoPanelOnScreenSystemList [0]);
		}
	}

	public void enableOrDisablePanel (bool state)
	{
		if (panelGameObject != null && panelGameObject.activeSelf != state) {
			panelGameObject.SetActive (state);
		}
	}

	public void fadePanel (infoPanel newInfoPanel, bool fadingPanel)
	{
		if (newInfoPanel == null) {
			return;
		}

		if (newInfoPanel.fadePanelCoroutine != null) {
			StopCoroutine (newInfoPanel.fadePanelCoroutine);
		}

		newInfoPanel.fadePanelCoroutine = StartCoroutine (fadePanelCoroutine (newInfoPanel, fadingPanel));
	}

	IEnumerator fadePanelCoroutine (infoPanel newInfoPanel, bool fadingPanel)
	{
		float targetValue = 1;

		float fadeSpeed = fadeOffPanelSpeed;

		if (fadingPanel) {
			targetValue = 0;

			fadeSpeed = fadeOnPanelSpeed;
		}

		if (!fadingPanel) {
			if (currentInfoPanel.mainCanvasGroup.alpha == 1) {
				currentInfoPanel.mainCanvasGroup.alpha = 0;
			}
		}

		while (newInfoPanel.mainCanvasGroup.alpha != targetValue) {
			newInfoPanel.mainCanvasGroup.alpha = Mathf.MoveTowards (newInfoPanel.mainCanvasGroup.alpha, targetValue, Time.deltaTime * fadeSpeed);

			yield return null;
		}

		if (fadingPanel) {
			newInfoPanel.panelGameObject.SetActive (false);
		}
	}

	[System.Serializable]
	public class infoPanel
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;
		public GameObject panelGameObject;
		public RectTransform panelRectTransform;
		public Text panelText;

		public RawImage panelImage;

		public Text panelNameText;

		public Transform fixedPanelPosition;

		public CanvasGroup mainCanvasGroup;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool isActive;
	
		public Coroutine fadePanelCoroutine;
	}
}
