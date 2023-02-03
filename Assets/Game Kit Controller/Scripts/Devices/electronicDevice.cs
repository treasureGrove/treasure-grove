using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class electronicDevice : MonoBehaviour
{
	public bool useOnlyForTrigger;
	public string functionToSetPlayer;

	public bool activateDeviceOnTriggerEnter;

	public bool useFreeInteraction;
	public bool useFreeInteractionEvent;
	public UnityEvent freeInteractionEvent;

	public bool useMoveCameraToDevice;
	public bool disableDeviceWhenStopUsing;
	public bool stopUsingDeviceWhenUnlock;
	public bool disableAndRemoveDeviceWhenUnlock;
	public UnityEvent functionToUseDevice = new UnityEvent ();
	public bool usingDevice;
	//public LayerMask layerForUsers;

	public UnityEvent unlockFunctionCall = new UnityEvent ();
	public UnityEvent lockFunctionCall = new UnityEvent ();
	public GameObject currentPlayer;

	public bool activateEventOnTriggerStay;
	public UnityEvent triggerStayEvent = new UnityEvent ();
	public float eventOnTriggerStayRate;
	public bool activateEventOnTriggerEnter;
	public UnityEvent triggerEnterEvent = new UnityEvent ();
	public bool activateEventOnTriggerExit;
	public UnityEvent triggerExitEvent = new UnityEvent ();

	public bool sendPlayerOnTriggerEnter;
	public eventParameters.eventToCallWithGameObject eventToSendPlayerOnTriggerEnter;
	public bool sendPlayerOnTriggerExit;
	public eventParameters.eventToCallWithGameObject eventToSendPlayerOnTriggerExit;

	public bool activateEventIfUnableToUseDevice;
	public UnityEvent unableToUseDeviceEvent = new UnityEvent ();

	public bool sendCurrentPlayerOnEvent;
	public eventParameters.eventToCallWithGameObject setCurrentPlayerEvent;

	public bool useEventOnStartUsingDevice;
	public bool useEventOnStopUsingDevice;
	public UnityEvent eventOnStartUsingDevice;
	public UnityEvent eventOnStopUsingDevice;

	public bool deviceCanBeUsed = true;

	public bool playerInside;

	float lastTimeEventOnTriggerStay;

	public moveCameraToDevice cameraMovementManager;
	public moveDeviceToCamera deviceMovementManager;

	public bool disableDeviceInteractionAfterUsingOnce;


	List<GameObject> playerFoundList = new List<GameObject> ();

	void Start ()
	{
		if (cameraMovementManager == null) {
			cameraMovementManager = GetComponent<moveCameraToDevice> ();
		}

		if (deviceMovementManager == null) {
			deviceMovementManager = GetComponent<moveDeviceToCamera> ();
		}

		//layerForUsers =  LayerMask.NameToLayer("player") << 0;
	}

	void Update ()
	{
		if (playerInside && activateEventOnTriggerStay) {
			if (Time.time > lastTimeEventOnTriggerStay + eventOnTriggerStayRate) {
				triggerStayEvent.Invoke ();
				lastTimeEventOnTriggerStay = Time.time;
			}
		}
	}

	public void activateDevice ()
	{
		if (!deviceCanBeUsed) {
			if (activateEventIfUnableToUseDevice) {
				if (unableToUseDeviceEvent.GetPersistentEventCount () > 0) {
					unableToUseDeviceEvent.Invoke ();
				}
			}

			return;
		}

		if (!useOnlyForTrigger) {
			if (usingDevice && !useMoveCameraToDevice && !useFreeInteraction) {
				return;
			}
		}

		if (useFreeInteraction && usingDevice) {
			if (useFreeInteractionEvent) {
				freeInteractionEvent.Invoke ();
			}
		} else {
			setDeviceState (!usingDevice);
		}
	}

	public void setDeviceState (bool state)
	{
		usingDevice = state;

		if (!useOnlyForTrigger) {
			if (useMoveCameraToDevice) {
				moveCamera (usingDevice);
			} 
		}
			
		functionToUseDevice.Invoke ();

		if (usingDevice) {
			if (useEventOnStartUsingDevice) {
				eventOnStartUsingDevice.Invoke ();
			}
		} else {
			if (useEventOnStopUsingDevice) {
				eventOnStopUsingDevice.Invoke ();
			}
		}

		if (!usingDevice) {
			if (disableDeviceInteractionAfterUsingOnce) {
				removeDeviceFromList ();
			}
		}
	}

	public void moveCamera (bool state)
	{
		if (cameraMovementManager != null) {
			cameraMovementManager.moveCamera (usingDevice);
		}

		if (deviceMovementManager != null) {
			deviceMovementManager.moveCamera (usingDevice);
		}
	}

	public void setCurrentUser (GameObject newPlayer)
	{
		if (!usingDevice) {
			currentPlayer = newPlayer;

			if (cameraMovementManager != null) {
				cameraMovementManager.setCurrentPlayer (currentPlayer);
			}

			if (deviceMovementManager != null) {
				deviceMovementManager.setCurrentPlayer (currentPlayer);
			}
		}
	}

	public void setPlayerManually (GameObject newPlayer)
	{
		if (newPlayer != null) {
			Collider newPlayerCollider = newPlayer.GetComponent<Collider> ();

			if (newPlayerCollider != null) {
				checkTriggerInfo (newPlayerCollider, true);
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
		//if ((1 << col.gameObject.layer & layerForUsers.value) == 1 << col.gameObject.layer) {
		//}
		//if the player is entering in the trigger
//		print (isEnter + " " + col.name);

		if (isEnter) {
			//if the device is already being used, return
			if (usingDevice) {
				return;
			}

			if (col.CompareTag ("Player")) {

				bool isNewPayerFound = false;

				if (!playerFoundList.Contains (col.gameObject)) {
					playerFoundList.Add (col.gameObject);

					isNewPayerFound = true;
				}

				currentPlayer = col.gameObject;

				if (useOnlyForTrigger || !useMoveCameraToDevice) {

					if (cameraMovementManager != null) {
						cameraMovementManager.setCurrentPlayer (currentPlayer);
					}

					if (deviceMovementManager != null) {
						deviceMovementManager.setCurrentPlayer (currentPlayer);
					}

					if (useOnlyForTrigger) {
						if (functionToSetPlayer != "") {
							SendMessage (functionToSetPlayer, currentPlayer, SendMessageOptions.DontRequireReceiver);
						}
					} else {
						if (!useMoveCameraToDevice) {
							setDeviceState (true);
						}
					}
				}

				if (sendCurrentPlayerOnEvent) {
					setCurrentPlayerEvent.Invoke (col.gameObject);
				}
					
				if (activateEventOnTriggerEnter) {
					triggerEnterEvent.Invoke ();
				}

				if (sendPlayerOnTriggerEnter) {
					eventToSendPlayerOnTriggerEnter.Invoke (col.gameObject);
				}

				playerInside = true;

				if (activateDeviceOnTriggerEnter && isNewPayerFound) {
					usingDevicesSystem currentUsingDevicesSystem = col.gameObject.GetComponent<usingDevicesSystem> ();

					if (currentUsingDevicesSystem != null) {
						currentUsingDevicesSystem.useCurrentDevice (gameObject);
					}
				}
			}
		} else {
			if ((usingDevice && !useFreeInteraction) || (useFreeInteraction && currentPlayer != col.gameObject)) {
				return;
			}

			//if the player is leaving the trigger
			if (col.CompareTag ("Player")) {
				//if the player is the same that was using the device, the device can be used again
				if (playerFoundList.Contains (col.gameObject)) {
					playerFoundList.Remove (col.gameObject);
				}
					
				if (playerFoundList.Count == 0) {
					currentPlayer = null;

					if (useOnlyForTrigger) {
						usingDevice = false;
					} else {
						if (!useMoveCameraToDevice || disableDeviceWhenStopUsing) {
							setDeviceState (false);
						}
					}

					if (activateEventOnTriggerExit) {
						triggerExitEvent.Invoke ();
					}

					if (sendPlayerOnTriggerExit) {
						eventToSendPlayerOnTriggerExit.Invoke (col.gameObject);
					}

					playerInside = false;

					lastTimeEventOnTriggerStay = 0;
				}
			}
		}
	}

	public void setUsingDeviceState (bool state)
	{
		usingDevice = state;
	}

	public GameObject getCurrentPlayer ()
	{
		return currentPlayer;
	}

	public void unlockObject ()
	{
		if (unlockFunctionCall.GetPersistentEventCount () > 0) {
			unlockFunctionCall.Invoke ();
		}

		if (disableAndRemoveDeviceWhenUnlock) {
			removeDeviceFromList ();
		}
	}

	public void lockObject ()
	{
		if (lockFunctionCall.GetPersistentEventCount () > 0) {
			lockFunctionCall.Invoke ();
		}
	}

	public void removeDeviceFromListWithoutDisablingDevice ()
	{
		if (playerFoundList.Count > 0) {
			for (int i = 0; i < playerFoundList.Count; i++) {
				playerFoundList [i].GetComponent<usingDevicesSystem> ().removeDeviceFromList (gameObject);
			}
		}
	}

	public void clearDeviceList ()
	{
		if (playerFoundList.Count > 0) {
			for (int i = 0; i < playerFoundList.Count; i++) {
				playerFoundList [i].GetComponent<usingDevicesSystem> ().clearDeviceList ();
			}
		}
	}

	public void addDeviceToList ()
	{
		if (playerFoundList.Count > 0) {
			for (int i = 0; i < playerFoundList.Count; i++) {
				playerFoundList [i].GetComponent<usingDevicesSystem> ().addDeviceToList (gameObject);
			}
		}
	}

	public void removeDeviceFromList ()
	{
		if (playerFoundList.Count > 0) {
			for (int i = 0; i < playerFoundList.Count; i++) {
				playerFoundList [i].GetComponent<usingDevicesSystem> ().removeDeviceFromList (gameObject);
			}

			gameObject.tag = "Untagged";
		}
	}

	public void removeDeviceFromListExternalCall ()
	{
		if (playerFoundList.Count > 0) {
			for (int i = 0; i < playerFoundList.Count; i++) {
				playerFoundList [i].GetComponent<usingDevicesSystem> ().removeDeviceFromListExternalCall (gameObject);
			}

			gameObject.tag = "Untagged";
		}
	}

	public void stopUsindDevice ()
	{
		if (stopUsingDeviceWhenUnlock) {
			setDeviceState (false);
		}
	}

	public void stopUseDeviceToPlayer ()
	{
		if (currentPlayer != null) {
			currentPlayer.GetComponent<usingDevicesSystem> ().useDevice ();
		}
	}

	public void reloadDeviceStringActionOnPlayer ()
	{
		if (playerFoundList.Count > 0) {
			for (int i = 0; i < playerFoundList.Count; i++) {
				playerFoundList [i].GetComponent<usingDevicesSystem> ().reloadDeviceStringActionOnPlayer (gameObject);
			}
		}
	}

	public void setDeviceCanBeUsedState (bool state)
	{
		deviceCanBeUsed = state;
	}

	public void cancelUseElectronicDevice ()
	{
		if (usingDevice) {
			activateDevice ();
		}
	}
}