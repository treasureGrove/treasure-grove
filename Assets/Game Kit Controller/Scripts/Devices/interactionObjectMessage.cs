using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class interactionObjectMessage : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	[TextArea (3, 10)] public string message;
	public bool usingDevice;
	public List<string> tagToDetect = new List<string> ();
	public bool pausePlayerWhileReading;
	public bool pressSecondTimeToStopReading;
	public float showMessageTime;
	public bool moveCameraToPosition;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool messageRemoved;
	public bool interactionUsed;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool callEventOnInteraction;
	public bool callEventOnEveryInteraction;
	public UnityEvent eventOnInteraction;
	public UnityEvent eventOnEndInteraction;

	[Space]
	[Header ("Components")]
	[Space]

	public Collider mainCollider;
	public moveCameraToDevice cameraMovementManager;

	GameObject currentPlayer;
	playerController currentPlayerControllerManager;
	menuPause pauseManager;
	usingDevicesSystem usingDevicesManager;
	float lastTimeUsed;

	playerComponentsManager mainPlayerComponentsManager;

	bool cameraMovementLocated;

	void Update ()
	{
		if (usingDevice) {
			if ((!pausePlayerWhileReading || !pressSecondTimeToStopReading) && Time.time > lastTimeUsed + showMessageTime) {
				activateDevice ();
			}
		}
	}

	public void activateDevice ()
	{
		if (messageRemoved) {
			return;
		}

		if (!pressSecondTimeToStopReading && usingDevice && showMessageTime == 0) {
			return;
		}

		usingDevice = !usingDevice;

		if (pausePlayerWhileReading) {
			setDeviceState (usingDevice);
		}

		checkCameraMovementLocated ();

		if (moveCameraToPosition && cameraMovementLocated) {
			cameraMovementManager.moveCamera (usingDevice);
		}

		if (usingDevice) {
			usingDevicesManager.checkShowObjectMessage (message, showMessageTime);

			lastTimeUsed = Time.time;
		} else {
			if (showMessageTime == 0) {
				usingDevicesManager.stopShowObjectMessage ();
			}
		}

		if (callEventOnInteraction) {
			if (callEventOnEveryInteraction) {
				if (usingDevice) {
					eventOnInteraction.Invoke ();
				} else {
					eventOnEndInteraction.Invoke ();
				}
			} else {
				if (!interactionUsed) {
					if (usingDevice) {
						eventOnInteraction.Invoke ();
					} else {
						eventOnEndInteraction.Invoke ();
					}
				}

				if (!usingDevice) {
					interactionUsed = true;
				}
			}
		}
	}

	void checkCameraMovementLocated ()
	{
		if (!cameraMovementLocated) {
			if (cameraMovementManager == null) {
				cameraMovementManager = GetComponent<moveCameraToDevice> ();

				cameraMovementLocated = cameraMovementManager != null;
			}
		}
	}

	//check when the player enters or exits of the trigger in the device
	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		//if the player is entering in the trigger
		if (isEnter) {
			//if the device is already being used, return
			if (usingDevice) {
				return;
			}

			if (tagToDetect.Contains (col.tag)) {
				currentPlayer = col.gameObject;

				mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

				currentPlayerControllerManager = mainPlayerComponentsManager.getPlayerController ();

				usingDevicesManager = mainPlayerComponentsManager.getUsingDevicesSystem ();
			
				pauseManager = mainPlayerComponentsManager.getPauseManager ();

				checkCameraMovementLocated ();

				if (cameraMovementLocated) {
					cameraMovementManager.setCurrentPlayer (currentPlayer);
				}
			}
		} else {
			//if the player is leaving the trigger
			if (tagToDetect.Contains (col.tag)) {
				
				//if the player is the same that was using the device, the device can be used again
				if (col.gameObject == currentPlayer) {
					currentPlayer = null;
				}
			}
		}
	}

	public void setDeviceState (bool state)
	{
		currentPlayerControllerManager.setUsingDeviceState (state);

		pauseManager.usingDeviceState (state);

		currentPlayerControllerManager.changeScriptState (!state);
	}

	public void removeMessage ()
	{
		messageRemoved = true;

		if (usingDevicesManager != null) {
			usingDevicesManager.removeDeviceFromListExternalCall (gameObject);
		}

		if (mainCollider != null) {
			mainCollider.enabled = false;
		}
	}
}
