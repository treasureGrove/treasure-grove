using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class pressObjectsInOrderSystem : MonoBehaviour
{
	public List<positionInfo> positionInfoList = new List<positionInfo> ();

	public bool allPositionsPressed;
	public int correctPressedIndex;
	public bool useIncorrectOrderSound;
	public AudioClip incorrectOrderSound;

	public bool usingPressedObjectSystem;
	public LayerMask pressObjectsLayer;
	public bool pressObjectsWhileMousePressed;

	public UnityEvent allPositionsPressedEvent;

	public Camera mainCamera;
	public AudioSource mainAudioSource;


	bool touchPlatform;
	Touch currentTouch;
	bool touching;
	GameObject currentObjectPressed;
	GameObject previousObjectPressed;
	RaycastHit hit;

	bool mainCameraLocated;

	Ray ray;

	Coroutine updateCoroutine;

	IEnumerator mainUpdateCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			if (usingPressedObjectSystem && pressObjectsWhileMousePressed && mainCameraLocated) {
				//meter lo de touch aqui para llamar mientras se tenga pulsado el raton, distinguiendo que sea un nuevo objeto
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
						touching = true;

						ray = mainCamera.ScreenPointToRay (currentTouch.position);

						if (Physics.Raycast (ray, out hit, Mathf.Infinity, pressObjectsLayer)) {
							currentObjectPressed = hit.collider.gameObject;

							if (currentObjectPressed != previousObjectPressed) {
								previousObjectPressed = currentObjectPressed;
							}
						}
					}

					if ((currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved) && touching) {
						ray = mainCamera.ScreenPointToRay (currentTouch.position);

						if (Physics.Raycast (ray, out hit, Mathf.Infinity, pressObjectsLayer)) {
							currentObjectPressed = hit.collider.gameObject;

							if (currentObjectPressed != previousObjectPressed) {
								previousObjectPressed = currentObjectPressed;
								EventTrigger currentEventTrigger = currentObjectPressed.GetComponent<EventTrigger> ();

								if (currentEventTrigger != null) {
									var pointer = new PointerEventData (EventSystem.current);

									ExecuteEvents.Execute (currentObjectPressed, pointer, ExecuteEvents.pointerEnterHandler);
									ExecuteEvents.Execute (currentObjectPressed, pointer, ExecuteEvents.pointerDownHandler);
								}
							}
						}
					}

					if (currentTouch.phase == TouchPhase.Ended) {
						touching = false;
					}
				}
			}

			yield return waitTime;
		}
	}

	public void checkPressedPosition (string positionName)
	{
		if (!usingPressedObjectSystem) {
			return;
		}

		if (allPositionsPressed) {
			return;
		}

		if (correctPressedIndex < positionInfoList.Count) {
			positionInfo currentPosition = positionInfoList [correctPressedIndex];

			if (currentPosition.positionName.Equals (positionName)) {
				currentPosition.positionActive = true;

				if (currentPosition.usePositionEvent) {
					if (currentPosition.positionEvent.GetPersistentEventCount () > 0) {
						currentPosition.positionEvent.Invoke ();
					}
				}

				correctPressedIndex++;

				if (correctPressedIndex == positionInfoList.Count) {
					if (allPositionsPressedEvent.GetPersistentEventCount () > 0) {
						allPositionsPressedEvent.Invoke ();
					}

					allPositionsPressed = true;
				}
			} else {

				resetPressedObjects ();

				if (useIncorrectOrderSound) {
					if (mainAudioSource != null) {
						mainAudioSource.PlayOneShot (incorrectOrderSound);			
					}
				}
			}
		}
	}

	public void resetPressedObjects ()
	{
		correctPressedIndex = 0;

		for (int i = 0; i < positionInfoList.Count; i++) {	
			positionInfoList [i].positionActive = false;
		}
	}

	public void startOrStopPressedObjectSystem ()
	{
		setUsingPressedObjectSystemState (!usingPressedObjectSystem);
	}

	public void setUsingPressedObjectSystemState (bool state)
	{
		usingPressedObjectSystem = state;

		stopUpdateCoroutine ();

		if (usingPressedObjectSystem) {
			updateCoroutine = StartCoroutine (mainUpdateCoroutine ());

			if (mainAudioSource == null) {
				mainAudioSource = GetComponent<AudioSource> ();
			}

			touchPlatform = touchJoystick.checkTouchPlatform ();

			if (!mainCameraLocated) {
				
				mainCamera = GKC_Utils.findMainPlayerCameraComponentOnScene ();
		
				if (mainCamera != null) {
					mainCameraLocated = true;
				}
			}
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	[System.Serializable]
	public class positionInfo
	{
		public string positionName;
		public bool usePositionEvent;
		public UnityEvent positionEvent;
		public bool positionActive;
	}
}
