using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class doorSystem : MonoBehaviour
{
	public List<singleDoorInfo> doorsInfo = new List<singleDoorInfo> ();
	public List<string> tagListToOpen = new List<string> ();
	public doorMovementType movementType;
	public AudioClip openSound;
	public AudioClip closeSound;
	public doorType doorTypeInfo;
	public doorCurrentState doorState;

	public bool locked;
	public bool openDoorWhenUnlocked = true;
	public bool useSoundOnUnlock;
	public AudioClip unlockSound;
	public AudioSource unlockAudioSource;

	public float openSpeed;
	public GameObject hologram;
	public bool closeAfterTime;
	public float timeToClose;

	public bool showGizmo;
	public float gizmoArrowLength = 1;
	public float gizmoArrowLineLength = 2.5f;
	public float gizmoArrowAngle = 20;
	public Color gizmoArrowColor = Color.white;
	public bool rotateInBothDirections;

	public string animationName;

	public bool closeDoorOnTriggerExit = true;

	//set if the door is rotated or translated
	public enum doorMovementType
	{
		translate,
		rotate,
		animation
	}

	//set how the door is opened, using triggers, a button close to the door, using a hologram to press the interaction button close to the door
	//and by shooting the door
	public enum doorType
	{
		trigger,
		button,
		hologram,
		shoot
	}

	//set the initial state of the door, opened or closed
	public enum doorCurrentState
	{
		closed,
		opened
	}

	public Transform currentPlayerTransform;

	public bool useEventOnOpenAndClose;
	public UnityEvent openEvent;
	public UnityEvent closeEvent;

	public bool useEventOnUnlockDoor;
	public UnityEvent evenOnUnlockDoor;

	public bool useEventOnLockDoor;
	public UnityEvent eventOnLockDoor;

	public bool useEventOnDoorFound;
	public UnityEvent eventOnLockedDoorFound;
	public UnityEvent eventOnUnlockedDoorFound;

	public AudioSource soundSource;
	public mapObjectInformation mapObjectInformationManager;
	public Animation mainAnimation;
	public hologramDoor hologramDoorManager;
	public deviceStringAction deviceStringActionManager;

	public bool setMapIconsOnDoor = true;

	public bool moving;

	bool doorFound;

	bool enter;
	bool exit;

	int doorsNumber;
	int doorsInPosition = 0;

	int i;

	float lastTimeOpened;
	Coroutine lockStateCoroutine;

	bool disableDoorOpenCloseAction;

	float originalOpenSpeed;

	List<GameObject> characterList = new List<GameObject> ();

	bool charactersInside;

	void Start ()
	{
		if (movementType == doorMovementType.animation) {
			if (mainAnimation == null) {
				mainAnimation = GetComponent<Animation> ();
			}
		} else {
			//get the original rotation and position of every panel of the door
			for (i = 0; i < doorsInfo.Count; i++) {
				doorsInfo [i].originalPosition = doorsInfo [i].doorMesh.transform.localPosition;
				doorsInfo [i].originalRotation = doorsInfo [i].doorMesh.transform.localRotation;

				if (doorsInfo [i].openedPosition != null) {
					doorsInfo [i].openedPositionFound = true;
				}

				if (doorsInfo [i].rotatedPosition != null) {
					doorsInfo [i].rotatedPositionFound = true;
				}
			}

			//total number of panels
			doorsNumber = doorsInfo.Count;
		}

		if (hologram != null) {
			if (hologramDoorManager == null) {
				hologramDoorManager = hologram.GetComponent<hologramDoor> ();
			}
		}

		if (doorState == doorCurrentState.opened) {
			for (i = 0; i < doorsInfo.Count; i++) {
				if (movementType == doorMovementType.translate) {
					if (doorsInfo [i].openedPositionFound) {
						doorsInfo [i].doorMesh.transform.localPosition = doorsInfo [i].openedPosition.transform.localPosition;
					} 
				} else {
					if (doorsInfo [i].rotatedPositionFound) {
						doorsInfo [i].doorMesh.transform.localRotation = doorsInfo [i].rotatedPosition.transform.localRotation;
					}
				}
			}
		}

		if (soundSource == null) {
			soundSource = GetComponent<AudioSource> ();
		}

		if (mapObjectInformationManager == null) {
			mapObjectInformationManager = GetComponent<mapObjectInformation> ();
		}

		if (deviceStringActionManager == null) {
			deviceStringActionManager = GetComponent<deviceStringAction> ();
		}

		originalOpenSpeed = openSpeed;
	}

	void Update ()
	{
		//if the player enters or exits the door, move the door
		if ((enter || exit)) {
			moving = true;
		
			if (movementType == doorMovementType.animation) {
				if (!mainAnimation.IsPlaying (animationName)) {
					setDoorState ();
				}
			} else {
				//for every panel in the door
				doorsInPosition = 0;

				for (i = 0; i < doorsInfo.Count; i++) {
					//if the panels are translated, then
					if (movementType == doorMovementType.translate) {
						//if the curren position of the panel is different from the target position, then
						if (doorsInfo [i].doorMesh.transform.localPosition != doorsInfo [i].currentTargetPosition) {
							//translate the panel
							doorsInfo [i].doorMesh.transform.localPosition =
							Vector3.MoveTowards (doorsInfo [i].doorMesh.transform.localPosition, doorsInfo [i].currentTargetPosition, Time.deltaTime * openSpeed);
						} 
						//if the panel has reached its target position, then
						else {
							doorsInfo [i].doorMesh.transform.localPosition = doorsInfo [i].currentTargetPosition;

							//increase the number of panels that are in its target position
							doorsInPosition++;
						}
					} 
					//if the panels are rotated, then
					else {
						//if the curren rotation of the panel is different from the target rotation, then
						if (doorsInfo [i].doorMesh.transform.localRotation != doorsInfo [i].currentTargetRotation) {
							//rotate from its current rotation to the target rotation
							doorsInfo [i].doorMesh.transform.localRotation = Quaternion.RotateTowards (doorsInfo [i].doorMesh.transform.localRotation, 
								doorsInfo [i].currentTargetRotation, Time.deltaTime * openSpeed * 10);
						} 
						//if the panel has reached its target rotation, then
						else {
							//increase the number of panels that are in its target rotation
							doorsInPosition++;

							if (exit) {
								doorsInfo [i].doorMesh.transform.localRotation = Quaternion.identity;
							}
						}
					}
				}

				//if all the panels in the door are in its target position/rotation
				if (doorsInPosition == doorsNumber) {
					setDoorState ();
				}
			}
		}

		if (closeAfterTime) {
			if (doorState == doorCurrentState.opened && !exit && !enter && !moving) {
				if (Time.time > lastTimeOpened + timeToClose) {
					changeDoorsStateByButton ();
				}
			}
		}
	}

	public void setDoorState ()
	{
		//if the door was opening, then the door is opened
		if (enter) {
			doorState = doorCurrentState.opened;
			lastTimeOpened = Time.time;
		}

		//if the door was closing, then the door is closed
		if (exit) {
			doorState = doorCurrentState.closed;
		}

		//reset the parameters
		enter = false;
		exit = false;
		doorsInPosition = 0;
		moving = false;
	}

	//if the door was unlocked, locked it
	public void lockDoor ()
	{
		if (lockStateCoroutine != null) {
			StopCoroutine (lockStateCoroutine);
		}

		lockStateCoroutine = StartCoroutine (lockDoorCoroutine ());
	}

	IEnumerator lockDoorCoroutine ()
	{
		yield return new WaitForSeconds (0.01f);

		if (locked) {
			StopCoroutine (lockStateCoroutine);
		}

		if (doorState == doorCurrentState.opened || (doorState == doorCurrentState.closed && moving)) {
			closeDoors ();
		}

		//if the door is not a hologram type, then close the door
		if (doorTypeInfo != doorType.hologram && doorTypeInfo != doorType.button) {
			
		} else {
			//else, lock the hologram, so the door is closed
			if (hologramDoorManager != null) {
				hologramDoorManager.lockHologram ();
			}
		}

		if (useEventOnLockDoor) {
			eventOnLockDoor.Invoke ();
		}

		if (setMapIconsOnDoor && mapObjectInformationManager != null) {
			mapObjectInformationManager.addMapObject ("Locked Door");
		}

		locked = true;
	}

	//if the door was locked, unlocked it
	public void unlockDoor ()
	{
		if (lockStateCoroutine != null) {
			StopCoroutine (lockStateCoroutine);
		}

		lockStateCoroutine = StartCoroutine (unlockDoorCoroutine ());
	}

	IEnumerator unlockDoorCoroutine ()
	{
		yield return new WaitForSeconds (0.01f);
	
		if (!locked) {
			StopCoroutine (lockStateCoroutine);
		}

		locked = false;

		//if the door is not a hologram type, then open the door
		if (doorTypeInfo != doorType.hologram) {
			if (openDoorWhenUnlocked) {
				changeDoorsStateByButton ();
			}
		} else {
			//else, unlock the hologram, so the door can be opened when the hologram is used
			if (hologramDoorManager != null) {
				hologramDoorManager.unlockHologram ();
			}
		}

		if (setMapIconsOnDoor && mapObjectInformationManager) {
			mapObjectInformationManager.addMapObject ("Unlocked Door");
		}

		if (useSoundOnUnlock) {
			if (unlockAudioSource != null && unlockSound) {
				unlockAudioSource.PlayOneShot (unlockSound);
			}
		}

		if (useEventOnUnlockDoor) {
			evenOnUnlockDoor.Invoke ();
		}
	}

	public bool isDisableDoorOpenCloseActionActive ()
	{
		return disableDoorOpenCloseAction;
	}

	public void setEnableDisableDoorOpenCloseActionValue (bool state)
	{
		disableDoorOpenCloseAction = state;
	}

	public void openOrCloseElevatorDoor ()
	{
		if (hologramDoorManager != null) {
			hologramDoorManager.openHologramDoorByExternalInput ();
		} else {
			changeDoorsStateByButton ();
		}
	}

	//a button to open the door calls this function, so
	public void changeDoorsStateByButton ()
	{
		if (disableDoorOpenCloseAction) {
			return;
		}

		if (moving) {
			return;
		}

		//if the door is opened, close it
		// && !moving
		if (doorState == doorCurrentState.opened) {
			closeDoors ();
		} 

		//if the door is closed, open it
		if (doorState == doorCurrentState.closed) {
			openDoors ();
		}
	}

	public void openDoorsIfClosed ()
	{
		if (moving) {
			return;
		}

		if (doorState == doorCurrentState.closed) {
			openDoors ();
		}
	}

	public void closeDoorsIfOpened ()
	{
		if (moving) {
			return;
		}

		if (doorState == doorCurrentState.opened) {
			closeDoors ();
		} 
	}

	//open the doors
	public void openDoors ()
	{
		if (disableDoorOpenCloseAction) {
			return;
		}

		if (!locked) {

			enter = true;
			exit = false;

			setDeviceStringActionState (true);

			if (movementType == doorMovementType.animation) {
				playDoorAnimation (true);
			} else {
				bool rotateForward = true;

				if (currentPlayerTransform != null) {
					if (rotateInBothDirections) {
						float dot = Vector3.Dot (transform.forward, (currentPlayerTransform.position - transform.position).normalized);

						if (dot > 0) {
							rotateForward = false;
						}
					}
				}

				//for every panel in the door, set that their target rotation/position are their opened/rotated positions
				for (i = 0; i < doorsInfo.Count; i++) {
					if (movementType == doorMovementType.translate) {
						if (doorsInfo [i].openedPositionFound) {
							doorsInfo [i].currentTargetPosition = doorsInfo [i].openedPosition.transform.localPosition;
						}
					} else {
						if (doorsInfo [i].rotatedPositionFound) {
							if (rotateForward) {
								doorsInfo [i].currentTargetRotation = doorsInfo [i].rotatedPosition.transform.localRotation;
							} else {
								doorsInfo [i].currentTargetRotation = Quaternion.Euler ((-1) * doorsInfo [i].rotatedPosition.transform.localEulerAngles);
							}
						}
					}
				}
			}

			//play the open sound
			playSound (openSound);

			if (useEventOnOpenAndClose) {
				if (openEvent.GetPersistentEventCount () > 0) {
					openEvent.Invoke ();
				}
			}
		}
	}

	//close the doors
	public void closeDoors ()
	{
		if (disableDoorOpenCloseAction) {
			return;
		}

		if (!locked) {
			
			enter = false;
			exit = true;

			setDeviceStringActionState (false);

			if (movementType == doorMovementType.animation) {
				playDoorAnimation (false);
			} else {
				//for every panel in the door, set that their target rotation/position are their original positions/rotations
				for (i = 0; i < doorsInfo.Count; i++) {
					if (movementType == doorMovementType.translate) {
						doorsInfo [i].currentTargetPosition = doorsInfo [i].originalPosition;
					} else {
						doorsInfo [i].currentTargetRotation = doorsInfo [i].originalRotation;
					}
				}
			}

			//play the close sound
			playSound (closeSound);

			if (useEventOnOpenAndClose) {
				if (closeEvent.GetPersistentEventCount () > 0) {
					closeEvent.Invoke ();
				}
			}
		}
	}

	public void playSound (AudioClip clipSound)
	{
		if (soundSource != null) {
			soundSource.PlayOneShot (clipSound);
		}
	}

	public void setDeviceStringActionState (bool state)
	{
		if (deviceStringActionManager != null) {
			deviceStringActionManager.changeActionName (state);
		}
	}

	void OnTriggerEnter (Collider col)
	{
		//the player has entered in the door trigger, check if this door is a trigger door or a hologram door opened
		if (checkIfTagCanOpen (col.GetComponent<Collider> ().tag)) {
			
			currentPlayerTransform = col.gameObject.transform;

			if (!characterList.Contains (col.gameObject)) {
				characterList.Add (col.gameObject);

				GKC_Utils.removeNullGameObjectsFromList (characterList);

				if (characterList.Count > 0) {
					charactersInside = true;
				}
			}

			if ((doorTypeInfo == doorType.trigger && (doorState == doorCurrentState.closed || moving) ||
			    (doorTypeInfo == doorType.hologram && doorState == doorCurrentState.closed &&
			    hologramDoorManager != null && hologramDoorManager.openOnTrigger))) {
				
				openDoors ();
			}

			if (useEventOnDoorFound && !doorFound) {
				if (locked) {
					eventOnLockedDoorFound.Invoke ();
				} else {
					eventOnUnlockedDoorFound.Invoke ();
				}

				doorFound = true;
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		//the player has gone of the door trigger, check if this door is a trigger door, a shoot door, or a hologram door and it is opened, to close it
		if (checkIfTagCanOpen (col.GetComponent<Collider> ().tag)) {
			bool canCloseDoors = false;

			if (characterList.Contains (col.gameObject)) {
				characterList.Remove (col.gameObject);

				GKC_Utils.removeNullGameObjectsFromList (characterList);

				if (characterList.Count == 0) {
					charactersInside = false;
				}
			}

			if (!charactersInside) {
				if (doorTypeInfo == doorType.trigger && closeDoorOnTriggerExit) {
					canCloseDoors = true;
				}

				if (doorTypeInfo == doorType.shoot && doorState == doorCurrentState.opened) {
					canCloseDoors = true;
				}

				if (doorTypeInfo == doorType.hologram && doorState == doorCurrentState.opened && closeDoorOnTriggerExit) {
					canCloseDoors = true;
				}

				if (canCloseDoors) {
					closeDoors ();
				}
			}
		}
	}

	//the player has shooted this door, so
	public void doorsShooted (GameObject projectile)
	{
		//check if the object is a player's projectile
		if (projectile.GetComponent<projectileSystem> () != null) {
			//and if the door is closed and a shoot type
			if (doorTypeInfo == doorType.shoot) {
				if (doorState == doorCurrentState.closed && !moving) {
					//then, open the door
					openDoors ();
				} else if (doorState == doorCurrentState.opened) {
					if (moving) {
						//then, open the door
						openDoors ();
					} else {
						lastTimeOpened = Time.time;
					}
				}
			}
		}
	}

	public bool checkIfTagCanOpen (string tagToCheck)
	{
		if (tagListToOpen.Contains (tagToCheck)) {
			return true;
		}

		return false;
	}

	public bool doorIsMoving ()
	{
		return moving;
	}

	public void playDoorAnimation (bool playForward)
	{
		if (mainAnimation != null) {
			if (playForward) {
				mainAnimation [animationName].speed = 1;

				mainAnimation.CrossFade (animationName);
			} else {
				mainAnimation [animationName].speed = -1; 

				if (!mainAnimation.IsPlaying (animationName)) {
					mainAnimation [animationName].time = mainAnimation [animationName].length;
					mainAnimation.Play (animationName);
				} else {
					mainAnimation.CrossFade (animationName);
				}
			}
		}
	}

	public bool isDoorOpened ()
	{
		return 	doorState == doorCurrentState.opened && !moving;
	}

	public bool isDoorClosed ()
	{
		return 	doorState == doorCurrentState.closed && !moving;
	}

	public bool isDoorOpening ()
	{
		return 	doorState == doorCurrentState.opened && moving;
	}

	public bool isDoorClosing ()
	{
		return 	doorState == doorCurrentState.closed && moving;
	}

	public void setReducedVelocity (float newValue)
	{
		openSpeed = newValue;
	}

	public void setNormalVelocity ()
	{
		setReducedVelocity (originalOpenSpeed);
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo) {
			if (movementType != doorMovementType.animation) {
				for (i = 0; i < doorsInfo.Count; i++) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (doorsInfo [i].doorMesh.transform.position, 0.3f);

					if (movementType == doorMovementType.translate) {
						if (doorsInfo [i].openedPosition != null) {
							Gizmos.color = Color.green;

							Gizmos.DrawSphere (doorsInfo [i].openedPosition.transform.position, 0.3f);

							Gizmos.color = Color.white;
							Gizmos.DrawLine (doorsInfo [i].doorMesh.transform.position, doorsInfo [i].openedPosition.transform.position);
						}
					}

					if (movementType == doorMovementType.rotate) {
						if (doorsInfo [i].rotatedPosition != null) {
							Gizmos.color = Color.green;

							GKC_Utils.drawGizmoArrow (doorsInfo [i].rotatedPosition.transform.position, doorsInfo [i].rotatedPosition.transform.right * gizmoArrowLineLength, 
								gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);

							Gizmos.color = Color.white;
							Gizmos.DrawLine (doorsInfo [i].doorMesh.transform.position, doorsInfo [i].rotatedPosition.transform.position);
						}
					}
				}
			}
		}
	}

	//a clas to store every panel that make the door, the position to move when is opened or the object which has the rotation that the door has to make
	//and fields to store the current and original rotation and position
	[System.Serializable]
	public class singleDoorInfo
	{
		public GameObject doorMesh;
		public GameObject openedPosition;
		public bool openedPositionFound;

		public GameObject rotatedPosition;
		public bool rotatedPositionFound;

		public Vector3 originalPosition;
		public Quaternion originalRotation;
		public Vector3 currentTargetPosition;
		public Quaternion currentTargetRotation;
	}
}