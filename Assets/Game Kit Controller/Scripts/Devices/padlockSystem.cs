using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Reflection;

public class padlockSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string currentCodeOnWheels;
	public string correctCode;
	public float rotateWheelSpeed;
	public int numbersPerWheel;

	public float waitDelayAfterUnlock;

	[Space]
	[Header ("Interaction Settings")]
	[Space]

	public bool allowToUseKeyboard;
	public bool useSwipeToRotateWheels;
	public float minSwipeDist;
	public bool useSwipeAndClickForWheels;

	public bool allowKeyboardControls;

	public bool useMouseWheelToRotateActive = true;

	[Space]
	[Header ("Wheel Settings")]
	[Space]

	public List<wheelInfo> wheels = new List<wheelInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool usingDevice;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventsOnUnlock;
	public UnityEvent eventOnUnlock;

	public bool callEventsAfterActivateUnlock;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject resetButton;

	public GameObject resolveButton;

	public AudioClip corretPassSound;
	public AudioClip keyPressSound;

	public Transform currentWheelMark;
	public Vector3 currentWheelMarkOffset;

	public AudioSource audioSource;
	public electronicDevice deviceManager;
	public examineObjectSystem examineObjectManager;

	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();
	int length;
	bool unlocked;

	bool touchPlatform;
	RaycastHit hit;
	GameObject currentCaptured;
	Touch currentTouch;

	float anglePerWheel;

	bool touching;

	wheelInfo currentWheelInfo;
	int currentWheelIndex;

	Vector3 swipeStartPos;
	Vector3 swipeFinalPos;

	int currentWheelRotationDirection = 1;
	playerInputManager playerInput;
	bool pressingHorizontalRight;
	bool pressingHorizontalLeft;
	bool resetingPadlock;

	GameObject currentPlayer;

	Vector2 axisValues;

	padlockSystemPlayerManagement currentPadlockSystemPlayerManagement;

	void Start ()
	{
		anglePerWheel = 360 / numbersPerWheel;
		touchPlatform = touchJoystick.checkTouchPlatform ();

		if (audioSource == null) {
			audioSource = GetComponent<AudioSource> ();
		}

		if (deviceManager == null) {
			deviceManager = GetComponent<electronicDevice> ();
		}

		if (examineObjectManager == null) {
			examineObjectManager = GetComponent<examineObjectSystem> ();
		}
	}

	void Update ()
	{
		//if the terminal still locked, and the player is using it
		if (!unlocked && usingDevice && !playerInput.isGameManagerPaused ()) {
//			//use the center of the camera as mouse, checking also the touch input
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

				if (captureRaycastResults.Count > 0) {
					currentCaptured = captureRaycastResults [0].gameObject;
					//check the current number key pressed with the finger
					if (currentTouch.phase == TouchPhase.Began) {
						for (int k = 0; k < wheels.Count; k++) {
							if (wheels [k].wheelTransform == currentCaptured.transform) {
								
								touching = true;
								changeExamineObjectState (false);

								currentWheelInfo = wheels [k];

								swipeStartPos = currentTouch.position;
							}
						}

						if (resetButton) {
							if (currentCaptured == resetButton) {
								resetPadLock ();
							}
						}

						if (resolveButton) {
							if (currentCaptured == resolveButton) {
								setCorrectCode ();
							}
						}
					}
				}

				if (currentTouch.phase == TouchPhase.Ended) {
					if (touching && useSwipeToRotateWheels) {
						
						swipeFinalPos = currentTouch.position;

						Vector2 currentSwipe = new Vector2 (swipeFinalPos.x - swipeStartPos.x, swipeFinalPos.y - swipeStartPos.y);

						if (currentSwipe.magnitude > minSwipeDist) {
							currentSwipe.Normalize ();
							if (Vector2.Dot (currentSwipe, GKC_Utils.swipeDirections.up) > 0.906f) {
								//up swipe
								setNextNumber ();
							}
							if (Vector2.Dot (currentSwipe, GKC_Utils.swipeDirections.down) > 0.906f) {
								//down swipe
								setPreviousNumber ();
							}
						}

						if (useSwipeAndClickForWheels) {
							checkNumber (currentWheelInfo, -1);
						}
					}

					changeExamineObjectState (true);

					touching = false;

					currentWheelInfo = null;
				}

				if (touching && (currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved)) {
					if (!useSwipeToRotateWheels) {
						checkNumber (currentWheelInfo, -1);
					}
				}
			}

			if (allowToUseKeyboard) {
				for (int i = 0; i < 10; i++) {
					if (Input.GetKeyDown ("" + i)) {
						checkNumberByKeyboard (i);
					}
				}
			}

			if (allowKeyboardControls) {
				axisValues = playerInput.getPlayerMovementAxis ();

				if (axisValues.y < 0) {
					currentWheelInfo = wheels [currentWheelIndex];
					setNextNumber ();
				}

				if (axisValues.y > 0) {
					currentWheelInfo = wheels [currentWheelIndex];
					setPreviousNumber ();
				}

				if (!pressingHorizontalRight) {
					if (axisValues.x > 0) {
						setCurrentWheelIndex (1);
						pressingHorizontalRight = true;
					}
				}

				if (!pressingHorizontalLeft) {
					if (axisValues.x < 0) {
						setCurrentWheelIndex (-1);
						pressingHorizontalLeft = true;
					}
				} 

				if (axisValues.x == 0) {
					pressingHorizontalLeft = false;
					pressingHorizontalRight = false;
				}
			}
		}
	}

	public void setCurrentWheelIndex (int amount)
	{
		currentWheelIndex += amount;

		if (currentWheelIndex >= wheels.Count) {
			currentWheelIndex = 0;
		}

		if (currentWheelIndex < 0) {
			currentWheelIndex = wheels.Count - 1;
		}

		if (currentWheelMark) {
			currentWheelMark.localPosition = wheels [currentWheelIndex].wheelTransform.localPosition + currentWheelMarkOffset;
		}
	}

	public void setPreviousNumber ()
	{
		currentWheelRotationDirection = 1;

		checkNumber (currentWheelInfo, -1);
	}

	public void setNextNumber ()
	{
		currentWheelRotationDirection = -1;

		checkNumber (currentWheelInfo, -1);
	}

	public void activatePadlock ()
	{
		usingDevice = !usingDevice;

		if (usingDevice) {
			currentPlayer = deviceManager.getCurrentPlayer ();

			playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			playerInput = mainPlayerComponentsManager.getPlayerInputManager ();

			currentPadlockSystemPlayerManagement = mainPlayerComponentsManager.getPadlockSystemPlayerManagement ();

			currentPadlockSystemPlayerManagement.setCurrentPadlockSystem (this);
		}

		currentPadlockSystemPlayerManagement.setUsingPadlockState (usingDevice);
	}

	public void checkNumberByKeyboard (int number)
	{
		if (wheels [currentWheelIndex].wheelRotating) {
			return;
		}

		checkNumber (wheels [currentWheelIndex], number);

		setCurrentWheelIndex (1);
	}

	public void checkNumber (wheelInfo wheel, int wheelNumber)
	{
		if (wheel.wheelRotating && !resetingPadlock) {
			return;
		}

		audioSource.PlayOneShot (keyPressSound);

		if (wheelNumber > -1) {
			if (wheelNumber <= numbersPerWheel) {
				wheel.currentNumber = wheelNumber;

				if (resetingPadlock) {
					StartCoroutine (rotateCenter (currentWheelRotationDirection, wheel, wheelNumber));
				} else {
					checkRotateWheel (currentWheelRotationDirection, wheel, wheel.currentNumber);
				}
			}
		} else {
			wheel.currentNumber += currentWheelRotationDirection;

			if (wheel.currentNumber >= numbersPerWheel) {
				wheel.currentNumber = 0;
			}

			if (wheel.currentNumber < 0) {
				wheel.currentNumber = numbersPerWheel - 1;
			}

			checkRotateWheel (currentWheelRotationDirection, wheel, wheel.currentNumber);
		}

		currentCodeOnWheels = getCurrentCodeOnWheels ();

		if (currentCodeOnWheels == correctCode) {
			setLockedState (true);
		}
	}

	public string getCurrentCodeOnWheels ()
	{
		string code = "";

		for (int i = 0; i < wheels.Count; i++) {
			code += wheels [i].currentNumber.ToString ();
		}

		return code;
	}

	public void setLockedState (bool state)
	{
		if (state) {
			audioSource.PlayOneShot (corretPassSound);
			unlocked = true;

			GetComponent<Collider> ().enabled = false;

			if (callEventsAfterActivateUnlock) {
				checkStopUsingDevice ();
			}

			if (useEventsOnUnlock) {
				eventOnUnlock.Invoke ();
			}

			deviceManager.unlockObject ();

			if (!callEventsAfterActivateUnlock) {
				checkStopUsingDevice ();
			}
		}
	}

	void checkStopUsingDevice ()
	{
		if (waitDelayAfterUnlock > 0) {
			StartCoroutine (waitingAfterUnlock ());
		} else {
			deviceManager.stopUsindDevice ();
		}
	}

	IEnumerator waitingAfterUnlock ()
	{
		yield return new WaitForSeconds (waitDelayAfterUnlock);

		deviceManager.stopUsindDevice ();

		yield return null;
	}

	public void checkRotateWheel (int direction, wheelInfo wheel, int number)
	{
		StartCoroutine (rotateCenter (direction, wheel, number));
	}

	public IEnumerator rotateCenter (int direction, wheelInfo wheel, int number)
	{
		wheel.wheelRotating = true;

		Quaternion orgRotCenter = wheel.wheelTransform.localRotation;

		float rotationAmount = number * anglePerWheel * direction * (-currentWheelRotationDirection);
		Quaternion dstRotCenter = Quaternion.Euler (new Vector3 (orgRotCenter.eulerAngles.x, orgRotCenter.eulerAngles.y, rotationAmount));

		for (float t = 0; t < 1;) {
			t += Time.deltaTime * rotateWheelSpeed;
			wheel.wheelTransform.localRotation = Quaternion.Slerp (orgRotCenter, dstRotCenter, t);

			yield return null;
		}

		wheel.wheelRotating = false;

		currentWheelRotationDirection = 1;

		resetingPadlock = false;
	}

	public void resetPadLock ()
	{
		resetingPadlock = true;

		for (int i = 0; i < wheels.Count; i++) {
			currentWheelIndex = i;

			checkNumberByKeyboard (0);
		}

		currentWheelIndex = 0;

		setCurrentWheelIndex (0);
	}

	public void setCorrectCode ()
	{
		resetingPadlock = true;

		for (int i = 0; i < wheels.Count; i++) {
			currentWheelIndex = i;

			checkNumberByKeyboard (int.Parse (correctCode [i].ToString ()));
		}

		currentWheelIndex = 0;

		setCurrentWheelIndex (0);
	}

	public void changeExamineObjectState (bool state)
	{
		if (examineObjectManager != null) {
			examineObjectManager.setExamineDeviceState (state);
		}
	}

	public void inputRotateWheel (bool directionUp)
	{
		if (useMouseWheelToRotateActive) {
			if (directionUp) {
				currentWheelInfo = wheels [currentWheelIndex];

				setNextNumber ();
			} else {
				currentWheelInfo = wheels [currentWheelIndex];

				setPreviousNumber ();
			}
		}
	}

	[System.Serializable]
	public class wheelInfo
	{
		public string name;
		public int currentNumber;
		public Transform wheelTransform;
		public bool wheelRotating;
	}
}