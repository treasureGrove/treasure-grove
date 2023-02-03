using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Reflection;

public class accessTerminal : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool unlockThisTerminal;
	public bool disableCodeScreenAfterUnlock;

	public string code;
	public LayerMask layer;
	public Color unlockedColor;

	public bool allowToUseKeyboard;
	public bool useMoveCameraToDevice;
	public bool usingDevice;

	public float waitDelayAfterUnlock;

	public string backgroundPanelName = "terminal";

	[Space]
	[Header ("Sound Settings")]
	[Space]

	public AudioClip wrongPassSound;
	public AudioClip corretPassSound;
	public AudioClip keyPressSound;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnDeviceUnlocked;
	public UnityEvent eventOnDeviceUnlocked;

	[Space]
	[Header ("Components")]
	[Space]

	public RectTransform pointer;
	public GameObject keys;
	public Text currectCode;
	public Text stateText;

	public Image wallPaper;

	public GameObject hackPanel;
	public GameObject hackActiveButton;

	public GameObject codeScreen;
	public GameObject unlockedScreen;

	public AudioSource audioSource;
	public electronicDevice deviceManager;
	public hackTerminal hackTerminalManager;

	List<Image> keysList = new List<Image> ();
	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();
	int totalKeysPressed = 0;
	int length;
	bool unlocked;
	bool changedColor;
	bool checkPressedButton;
	bool touchPlatform;
	RaycastHit hit;
	GameObject currentCaptured;
	Touch currentTouch;

	GameObject currentPlayer;
	Camera mainCamera;
	gameManager mainGameManager;

	bool hackingTerminal;

	void Start ()
	{
		//get all the keys inside the keys gameobject, checking the name of every object, comparing if it is a number from 0 to 9
		Component[] components = keys.GetComponentsInChildren (typeof(Image));
		foreach (Image child in components) {
			int n;
			if (int.TryParse (child.name, out n)) {
				keysList.Add (child);
			}
		}

		//set the current code to 0 according to the length of the real code
		currectCode.text = "";
		length = code.Length;
		for (int i = 0; i < length; i++) {
			currectCode.text += "0";
		}

		touchPlatform = touchJoystick.checkTouchPlatform ();

		if (hackPanel != null) {
			if (hackTerminalManager == null) {
				hackTerminalManager = hackPanel.GetComponent<hackTerminal> ();
			}

			//hackActiveButton.SetActive (true);
			if (useMoveCameraToDevice) {
				hackTerminalManager.hasMoveCameraToDevice ();
			}
		}

		if (audioSource == null) {
			audioSource = GetComponent<AudioSource> ();
		}

		if (deviceManager == null) {
			deviceManager = GetComponent<electronicDevice> ();
		}

		if (mainGameManager == null) {
			mainGameManager = FindObjectOfType<gameManager> ();
		}
	}

	void Update ()
	{
		//if the terminal still locked, and the player is using it
		if (!unlocked && usingDevice && !mainGameManager.isGamePaused ()) {
			//use the center of the camera as mouse, checking also the touch input
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

				//get a list with all the objects under the center of the screen of the finger tap
				captureRaycastResults.Clear ();
				PointerEventData p = new PointerEventData (EventSystem.current);
				p.position = currentTouch.position;
				p.clickCount = i;
				p.dragging = false;
				EventSystem.current.RaycastAll (p, captureRaycastResults);

				foreach (RaycastResult r in captureRaycastResults) {
					currentCaptured = r.gameObject;
					//if the center of the camera is looking at the screen, move the cursor image inside it
					if (currentCaptured.name.Equals (backgroundPanelName)) {
						if (Physics.Raycast (mainCamera.ScreenPointToRay (currentTouch.position), out hit, Mathf.Infinity, layer)) {
							if (!hit.collider.isTrigger) {
								pointer.gameObject.transform.position = hit.point;
								Debug.DrawLine (mainCamera.transform.position, hit.point, Color.green, 2);
							}
						}
					}
					//check the current number key pressed with the finger
					if (currentTouch.phase == TouchPhase.Began) {
						checkButton (currentCaptured);
					}

					//check the current number key preesed with the interaction button in the keyboard
					if (checkPressedButton) {
						checkButton (currentCaptured);
					}
				}
					
				if (checkPressedButton) {
					checkPressedButton = false;
				}
			}

			if (allowToUseKeyboard) {
				for (int i = 0; i < 10; i++) {
					if (Input.GetKeyDown ("" + i)) {
						checkNumber (i.ToString ());
					}
				}
			}
		}

		//if the device is unlocked, change the color of the interface for the unlocked color
		if (unlocked) {
			if (!changedColor) {
				wallPaper.color = Vector4.MoveTowards (wallPaper.color, unlockedColor, Time.deltaTime * 3);
				stateText.color = Vector4.MoveTowards (stateText.color, unlockedColor, Time.deltaTime * 3);

				if (wallPaper.color == unlockedColor && stateText.color == unlockedColor) {
					changedColor = true;
				}
			}
		}
	}

	public void pressButtonOnScreen ()
	{
		checkPressedButton = true;
	}

	//this function is called when the interaction button in the keyboard is pressed, so in pc, the code is written by aiming the center of the camera to
	//every number and pressing the interaction button. In touch devices, the code is written by tapping with the finger every key number directly
	public void activateTerminal ()
	{
		usingDevice = !usingDevice;

		if (usingDevice) {
			getCurrentUser ();
		} else {
			if (hackingTerminal) {
				hackTerminalManager.stopHacking ();

				hackingTerminal = false;
			}
		}
	}

	public void enableTerminal ()
	{
		usingDevice = true;
	
		getCurrentUser ();
	}

	public void disableTerminal ()
	{
		usingDevice = false;

		if (hackingTerminal) {
			hackTerminalManager.stopHacking ();

			hackingTerminal = false;
		}
	}

	public void getCurrentUser ()
	{
		currentPlayer = deviceManager.getCurrentPlayer ();

		if (currentPlayer != null) {
			mainCamera = currentPlayer.GetComponent<playerController> ().getPlayerCameraManager ().getMainCamera ();
		}
	}

	//the currentCaptured is checked, to write the value of the number key in the screen device
	void checkButton (GameObject button)
	{
		Image currentImage = button.GetComponent<Image> ();

		if (currentImage != null) {
			//check if the currentCaptured is a key number
			if (keysList.Contains (currentImage)) {
				checkNumber (currentImage.name);
			} else {
				if (hackActiveButton) {
					if (button == hackActiveButton) {
						if (hackTerminalManager != null) {
							hackTerminalManager.activeHack ();

							hackingTerminal = true;
						}
					}
				}
			}
		}
	}

	public void checkNumber (string numberString)
	{
		//reset the code in the screen
		if (totalKeysPressed == 0) {
			currectCode.text = "";
		}	
		//add the current key number pressed to the code
		currectCode.text += numberString;
		totalKeysPressed++;

		//play the key press sound
		audioSource.PlayOneShot (keyPressSound);

		//if the player has pressed the an amount of key numbers equal to the lenght of the code, check if it is correct
		if (totalKeysPressed == length) {
			//if it is equal, then call the object to unlock, play the corret pass sound, and disable this terminal
			if (currectCode.text == code) {
				setLockedState (true);
			}
			//else, reset the terminal, and try again
			else {
				audioSource.PlayOneShot (wrongPassSound);

				totalKeysPressed = 0;
			}
		}
	}

	public void setLockedState (bool state)
	{
		if (state) {
			audioSource.PlayOneShot (corretPassSound);

			stateText.text = "Unlocked";

			unlocked = true;

			if (!unlockThisTerminal) {
				deviceManager.unlockObject ();

				StartCoroutine (waitingAfterUnlock ());
			}

			if (hackPanel != null) {
				hackTerminalManager.moveHackTerminal (false);
				hackingTerminal = false;
			}

			if (disableCodeScreenAfterUnlock) {
				codeScreen.SetActive (false);

				unlockedScreen.SetActive (true);
			}

			if (useEventOnDeviceUnlocked) {
				eventOnDeviceUnlocked.Invoke ();
			}
		}
	}

	public void setLockState (Scrollbar info)
	{
		if (info.value == 0) {
			deviceManager.lockObject ();

			if (info.value > 0) {
				info.value = 0;
			}
		} else if (info.value == 1) {
			deviceManager.unlockObject ();

			if (info.value < 1) {
				info.value = 1;
			}
		}
	}

	IEnumerator waitingAfterUnlock ()
	{
		yield return new WaitForSeconds (waitDelayAfterUnlock);

		deviceManager.stopUsindDevice ();

		yield return null;
	}

	public void showPasswordOnHackPanel ()
	{
		hackTerminalManager.setTextContent (code);
	}

	public void unlockAccessTerminal ()
	{
		currectCode.text = "";

		totalKeysPressed = code.Length - 1;

		checkNumber (code);
	}

	public void activateHackButton ()
	{
		if (hackActiveButton != null) {
			hackActiveButton.SetActive (true);
		}
	}
}