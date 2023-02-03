using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[System.Serializable]
//this script allows to edit the touch buttons to set their positions in run time with the mouse or finger in a touch screen, save those positions in a file, 
//and reload in the next execution, in case you want to load it
public class editControlPosition : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool loadButtonsPositionEnabled = false;

	public bool initializeTouchButtonsPositionEnabled = false;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<RectTransform> buttons = new List<RectTransform> ();

	public bool mainGameManagerLocated;

	[Space]
	[Header ("Components")]
	[Space]

	public inputManager input;
	public gameManager mainGameManager;

	[HideInInspector] public List<Vector2> buttonsPos = new List<Vector2> ();
	[HideInInspector] public List<Vector2> buttonPosDefault = new List<Vector2> ();
	[HideInInspector] public List<touchButtonListener> touchButtonList = new List<touchButtonListener> ();

	RectTransform buttonToMove;

	bool enableButtonEdit = false;
	bool touching;
	bool grab;

	int i = 0;
	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();
	Touch currentTouch;
	bool touchPlatform;
	string currentSaveDataPath;
	bool checkSettingsInitialized;

	bool useRelativePath;

	string touchControlsPositionFolderName;
	string touchControlsPositionFileName;

	Coroutine mainUpdateCoroutine;

	void Start ()
	{
		touchPlatform = touchJoystick.checkTouchPlatform ();

		checkMainManager ();

		if (mainGameManagerLocated) {
			touchControlsPositionFolderName = mainGameManager.getTouchControlsPositionFolderName ();

			touchControlsPositionFileName = mainGameManager.getTouchControlsPositionFileName ();
		}
	}

	void checkMainManager ()
	{
		if (mainGameManager == null) {
			mainGameManager = FindObjectOfType<gameManager> ();
		}

		mainGameManagerLocated = mainGameManager != null;
	}

	public void stopUpdateCoroutine ()
	{
		if (mainUpdateCoroutine != null) {
			StopCoroutine (mainUpdateCoroutine);
		}
	}

	IEnumerator updateCoroutine ()
	{
		var waitTime = new WaitForSecondsRealtime (0.00001f);

		while (true) {
			if (enableButtonEdit) {
				//check the mouse position in the screen if we are in the editor, or the finger position in a touch device
				int touchCount = Input.touchCount;
				if (!touchPlatform) {
					touchCount++;
				}

				for (int i = 0; i < touchCount; i++) {
					if (!touchPlatform) {
						currentTouch = touchJoystick.convertMouseIntoFinger ();
					} else {
						currentTouch = Input.GetTouch (i);
					}

					if (currentTouch.phase == TouchPhase.Began) {
						//if the finger/mouse position is above a button, it is selected to translate
						captureRaycastResults.Clear ();
						PointerEventData p = new PointerEventData (EventSystem.current);
						p.position = currentTouch.position;
						p.clickCount = i;
						p.dragging = false;
						EventSystem.current.RaycastAll (p, captureRaycastResults);
						foreach (RaycastResult r in captureRaycastResults) {
							if (buttons.Contains (r.gameObject.GetComponent<RectTransform> ())) {
								grab = true;
								buttonToMove = r.gameObject.GetComponent<RectTransform> ();
								touching = true;
							}
						}
					}

					//the button follows the mouse/finger position
					if ((currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved) && grab) {
						if (grab && buttonToMove != null) {
							buttonToMove.position = new Vector2 (currentTouch.position.x, currentTouch.position.y);
						}
					}

					//release the button 
					if (currentTouch.phase == TouchPhase.Ended && touching) {
						touching = false;

						if (grab) {
							grab = false;

							buttonToMove = null;
						}
					}
				}
			}

			yield return waitTime;
		}
	}

	public void checkSettings ()
	{
		checkMainManager ();

		if (!checkSettingsInitialized && mainGameManagerLocated) {

			useRelativePath = mainGameManager.useRelativePath;

			currentSaveDataPath = getDataPath ();

			checkSettingsInitialized = true;
		}
	}

	public string getDataPath ()
	{
		string dataPath = "";

		if (useRelativePath) {
			dataPath = touchControlsPositionFolderName;
		} else {
			dataPath = Application.persistentDataPath + "/" + touchControlsPositionFolderName;
		}

		if (!Directory.Exists (dataPath)) {
			Directory.CreateDirectory (dataPath);
		}

		dataPath += "/";

		return dataPath;
	}

	public void getTouchButtons ()
	{
		checkMainManager ();

		if (mainGameManagerLocated) {
			//load or not the previous positions of the buttons, saved in another execution
			if (mainGameManager.loadEnabled) {
				if (loadButtonsPositionEnabled) {
					Load ();

					loadButtonsPositionEnabled = false;
				}

				if (initializeTouchButtonsPositionEnabled) {
					for (i = 0; i < buttons.Count; i++) {
						if (buttons [i] != null) {
							buttonsPos.Add (buttons [i].position);
						}
					}
				}
			}
		}
	}

	public void getTouchButtonsComponents ()
	{
		buttons.Clear ();

		buttonPosDefault.Clear ();

		touchButtonList.Clear ();

		if (input == null) {
			input = FindObjectOfType<inputManager> ();
		}

		if (input == null) {
			return;
		}

		//save all touch buttons in a list and their positions
		for (i = 0; i < input.touchButtonList.Count; i++) {
			if (input.touchButtonList [i] != null) {
				RectTransform currentRectTransform = input.touchButtonList [i].GetComponent<RectTransform> ();

				if (currentRectTransform != null) {
					buttons.Add (currentRectTransform);

					buttonPosDefault.Add (currentRectTransform.position);

					touchButtonList.Add (input.touchButtonList [i]);
				}
			}
		}

		print ("Touch Buttons Components stored");

		updateComponent ();
	}

	//after edit the buttons, their positions are saved
	public void activateEdit ()
	{
		enableButtonEdit = !enableButtonEdit;

		stopUpdateCoroutine ();

		if (enableButtonEdit) {
			changeButtonsState (false);

			mainUpdateCoroutine = StartCoroutine (updateCoroutine ());

		} else {
			buttonsPos.Clear ();

			for (i = 0; i < buttons.Count; i++) {
				if (buttons [i] != null) {
					buttonsPos.Add (buttons [i].transform.position);
				}
			}

			Save ();
		}
	}

	//if we cancel the edition, the buttons back to their previous position, if any of them has been moved
	public void disableEdit ()
	{
		if (enableButtonEdit) {
			enableButtonEdit = false;

			for (i = 0; i < buttons.Count; i++) {
				if (buttons [i] != null) {
					buttons [i].transform.position = buttonsPos [i];
				}
			}
		}
	}

	//set the buttons with their default position
	public void changeToDefault ()
	{
		for (i = 0; i < buttons.Count; i++) {
			if (buttons [i] != null) {
				buttons [i].transform.position = buttonPosDefault [i];
			}
		}

		Save ();
	}

	//disable and enable the pointer click components in the buttons, to avoid activate them while the game is paused
	public void changeButtonsState (bool state)
	{
		for (i = 0; i < touchButtonList.Count; i++) {
			if (touchButtonList [i] != null && touchButtonList [i].enabled != state) {
				touchButtonList [i].enabled = state;
			}
		}
	}

	//save in a file the positions of the button, vector2 are not serializable if I am not wrong, so every axis is save in a list of float
	public void Save ()
	{
		checkSettings ();

		savedButtons sB = new savedButtons ();

		for (i = 0; i < buttons.Count; i++) {
			if (buttons [i] != null) {
				sB.saveButtonsPosX.Add (buttons [i].position.x);
				sB.saveButtonsPosY.Add (buttons [i].position.y);
			}
		}

		BinaryFormatter bf = new BinaryFormatter ();

		FileStream file = File.Create (currentSaveDataPath + touchControlsPositionFileName + ".txt");

		bf.Serialize (file, sB.saveButtonsPosX);

		bf.Serialize (file, sB.saveButtonsPosY);

		file.Close ();

		buttonsPos.Clear ();

		for (i = 0; i < buttons.Count; i++) {
			buttonsPos.Add (new Vector2 (sB.saveButtonsPosX [i], sB.saveButtonsPosY [i]));
		}
	}

	//load the positions of the buttons reading the file if it exists
	public void Load ()
	{
		checkSettings ();

		savedButtons sB = new savedButtons ();

		if (File.Exists (currentSaveDataPath + touchControlsPositionFileName + ".txt")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (currentSaveDataPath + touchControlsPositionFileName + ".txt", FileMode.Open);

			sB.saveButtonsPosX = (List<float>)bf.Deserialize (file);
			sB.saveButtonsPosY = (List<float>)bf.Deserialize (file);

			file.Close ();
		}

		//also check the lenght are equal in both list, to avoid exceptions
		if (sB.saveButtonsPosX.Count == buttons.Count) {
			for (i = 0; i < buttons.Count; i++) {
				if (buttons [i] != null) {
					buttons [i].position = new Vector2 (sB.saveButtonsPosX [i], sB.saveButtonsPosY [i]);
				}
			}
		}
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Touch Control Positions", gameObject);
	}
}

//the positions of the buttons need to be serializable to save them in a file
[Serializable]
class savedButtons
{
	public List<float> saveButtonsPosX = new List<float> ();
	public List<float> saveButtonsPosY = new List<float> ();
}