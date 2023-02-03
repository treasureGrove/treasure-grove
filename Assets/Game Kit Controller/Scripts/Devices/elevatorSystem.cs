using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class elevatorSystem : MonoBehaviour
{
	public bool elevatorSystemEnabled = true;

	public List<floorInfo> floors = new List<floorInfo> ();
	public int currentFloor;
	public float elevatorSpeed = 20;

	public bool hasInsideElevatorDoor;
	public GameObject insideElevatorDoor;
	public GameObject elevatorSwitchPrefab;
	public bool addSwitchInNewFloors;
	public GameObject elevatorDoorPrefab;
	public bool addDoorInNewFloors;
	public bool moving;
	public bool doorsClosed = true;

	public float floorHeight = 5;

	public bool changeIconFloorWhenMoving;

	public bool showGizmo;
	public Color gizmoLabelColor;

	public bool useEventsOnMoveStartAndEnd;
	public UnityEvent eventOnMoveStart;
	public UnityEvent eventOnMoveEnd;

	int currentDirectionToMove = 1;

	bool inside;
	int i;
	bool lockedElevator;
	bool closingDoors;
	Coroutine elevatorMovement;
	mapObjectInformation mapObjectInformationManager;

	public List<passengersInfo> passengersInfoList = new List<passengersInfo> ();

	int previousFloorIndex;

	doorSystem insideElevatorDoorSystem;

	bool allDoorsClosed;

	floorInfo currentFloorInfo;

	bool insideElevatorDoorLocated;

	void Start ()
	{
		if (mapObjectInformationManager == null) {
			mapObjectInformationManager = GetComponent<mapObjectInformation> ();
		}

		if (insideElevatorDoor != null) {
			insideElevatorDoorSystem = insideElevatorDoor.GetComponent<doorSystem> ();

			insideElevatorDoorLocated = insideElevatorDoorSystem != null;
		}
	}

	void Update ()
	{
		//check if there is doors in the elevator to close them and start the elevator movement when they are closed
		if (closingDoors) {
			if (insideElevatorDoorLocated || floors [previousFloorIndex].outsideElevatorDoorLocated) {
				allDoorsClosed = false;

				//print (!insideElevatorDoorSystem + " " + insideElevatorDoorSystem.isDoorClosed ());

//				print (!floors [previousFloorIndex].outsideElevatorDoorSystem + " " + floors [previousFloorIndex].outsideElevatorDoorSystem.isDoorClosed ());

				if ((!insideElevatorDoorLocated || insideElevatorDoorSystem.isDoorClosed ()) &&
				    (!floors [previousFloorIndex].outsideElevatorDoorLocated || floors [previousFloorIndex].outsideElevatorDoorSystem.isDoorClosed ())) {
					allDoorsClosed = true;
				}

				if (allDoorsClosed) {
					closingDoors = false;

					checkElevatorMovement ();
				}
			} else {
				closingDoors = false;

				checkElevatorMovement ();
			}
		}
	}

	//the player has press the button move up, so increase the current floor count
	public void nextFloor ()
	{
		if (!elevatorSystemEnabled) {
			return;
		}

		getFloorNumberToMove (1);
	}

	//the player has press the button move down, so decrease the current floor count
	public void previousFloor ()
	{
		if (!elevatorSystemEnabled) {
			return;
		}

		getFloorNumberToMove (-1);
	}

	public void moveBetweenTwoPositions ()
	{
		if (!elevatorSystemEnabled) {
			return;
		}

		getFloorNumberToMove (currentDirectionToMove);

		currentDirectionToMove *= -1;
	}

	//move to the floor, according to the direction selected by the player
	void getFloorNumberToMove (int direction)
	{
		//if the player is inside the elevator and it is not moving, then 
		if (inside && !moving) {
			//change the current floor to the next or the previous
			int floorIndex = currentFloor + direction;

			//check that the floor exists, and start to move the elevator to that floor position
			if (floorIndex < floors.Count && floorIndex >= 0) {
				openOrCloseElevatorDoors ();

				previousFloorIndex = currentFloor;

				currentFloor = floorIndex;

				checkIfInsideElevatorDoorSystem ();

				closingDoors = true;

				setAllPlayersParent (transform);
			}
		}
	}

	public void checkIfInsideElevatorDoorSystem ()
	{
		if (floors [currentFloor].outsideElevatorDoor != null) {
			if (!floors [currentFloor].outsideElevatorDoorLocated) {
				floors [currentFloor].outsideElevatorDoorSystem = floors [currentFloor].outsideElevatorDoor.GetComponent<doorSystem> ();

				floors [currentFloor].outsideElevatorDoorLocated = floors [currentFloor].outsideElevatorDoorSystem != null;
			}
		}
	}

	//move to the floor, according to the direction selected by the player
	public bool goToNumberFloor (int floorNumber)
	{
		if (!elevatorSystemEnabled) {
			return false;
		}

		bool canMoveToFloor = false;

		//if the player is inside the elevator and it is not moving, then 
		if (inside && !moving) {
			//check that the floor exists, and start to move the elevator to that floor position
			if (floorNumber < floors.Count && floorNumber >= 0 && floorNumber != currentFloor) {
				openOrCloseElevatorDoors ();

				previousFloorIndex = currentFloor;

				currentFloor = floorNumber;

				checkIfInsideElevatorDoorSystem ();

				closingDoors = true;

				setAllPlayersParent (transform);

				canMoveToFloor = true;
			}
		}

		return canMoveToFloor;
	}

	//when a elevator button is pressed, move the elevator to that floor
	public void callElevator (GameObject button)
	{
		if (!elevatorSystemEnabled) {
			return;
		}

		if (moving) {
			return;
		}

		for (i = 0; i < floors.Count; i++) {
			if (floors [i].floorButton == button) {
				
				lockedElevator = false;

				if (floors [currentFloor].outsideElevatorDoor != null) {
					checkIfInsideElevatorDoorSystem ();

					if (floors [currentFloor].outsideElevatorDoorSystem.locked) {
						lockedElevator = true;
					}
				}

				if (!lockedElevator) {
					if (currentFloor != i) {
						if (!doorsClosed) {
							openOrCloseElevatorDoors ();
						}

						previousFloorIndex = currentFloor;

						currentFloor = i;

						checkIfInsideElevatorDoorSystem ();

						closingDoors = true;
					} else {
						openOrCloseElevatorDoors ();
					}
				}
			}
		}
	}

	//open or close the inside and outside doors of the elevator if the elevator has every of this doors
	void openOrCloseElevatorDoors ()
	{
		if (insideElevatorDoor != null) {
			if (insideElevatorDoorSystem.doorState == doorSystem.doorCurrentState.closed) {
				doorsClosed = false;
			} else {
				doorsClosed = true;
			}

			insideElevatorDoorSystem.changeDoorsStateByButton ();
		}

		if (floors [currentFloor].outsideElevatorDoor != null) {
			checkIfInsideElevatorDoorSystem ();

			floors [currentFloor].outsideElevatorDoorSystem.openOrCloseElevatorDoor ();
		}
	}

	//stop the current elevator movement and start it again
	void checkElevatorMovement ()
	{
		if (elevatorMovement != null) {
			StopCoroutine (elevatorMovement);
		}

		elevatorMovement = StartCoroutine (moveElevator ());
	}

	IEnumerator moveElevator ()
	{
		moving = true;

		currentFloorInfo = floors [currentFloor];

		//move the elevator from its position to the currentfloor
		Vector3 currentElevatorPosition = transform.localPosition;

		Vector3 targetPosition = currentFloorInfo.floorPosition.localPosition;
		Quaternion targetRotation = currentFloorInfo.floorPosition.localRotation;

		bool rotateElevator = false;

		if (targetRotation != Quaternion.identity || transform.localRotation != Quaternion.identity) {
			rotateElevator = true;
		}

		float dist = GKC_Utils.distance (transform.position, targetPosition); 

		// calculate the movement duration
		float duration = dist / elevatorSpeed; 
		float t = 0;

		bool targetReached = false;

		float angleDifference = 0;

		float movementTimer = 0;

		float distanceToTarget = 0;

		checkEventsOnMove (true);

		if (currentFloorInfo.useEventsOnMoveStartAndEnd) {
			currentFloorInfo.eventOnMoveStart.Invoke ();
		}

		while (!targetReached) {
			t += Time.deltaTime / duration;

			transform.localPosition = Vector3.Lerp (currentElevatorPosition, targetPosition, t);

			if (rotateElevator) {
				transform.localRotation = Quaternion.Lerp (transform.localRotation, targetRotation, t);
			}

			angleDifference = Quaternion.Angle (transform.localRotation, targetRotation);

			movementTimer += Time.deltaTime;

			dist = GKC_Utils.distance (transform.localPosition, targetPosition); 

			distanceToTarget = GKC_Utils.distance (transform.localPosition, targetPosition);

			if ((dist < 0.02f && angleDifference < 0.02f && distanceToTarget < 0.02f) || movementTimer > (duration + 0.5f)) {
				targetReached = true;
			}

			yield return null;
		}

		//if the elevator reachs the correct floor, stop its movement, and deattach the player of its childs
		moving = false;

		setAllPlayersParent (null);

		openOrCloseElevatorDoors ();

		if (changeIconFloorWhenMoving) {
			if (mapObjectInformationManager != null) {
				mapObjectInformationManager.changeMapObjectIconFloorByPosition ();
			}
		}

		checkEventsOnMove (false);

		if (currentFloorInfo.useEventsOnMoveStartAndEnd) {
			currentFloorInfo.eventOnMoveEnd.Invoke ();
		}
	}

	void OnTriggerEnter (Collider col)
	{
		//the player has entered in the elevator trigger, stored it and set the evelator as his parent
		if (col.CompareTag ("Player")) {
			addPassenger (col.gameObject.transform);

			if (passengersInfoList.Count > 0) {
				inside = true;
			}

			setPlayerParent (transform, col.gameObject.transform);
		}
	}

	void OnTriggerExit (Collider col)
	{
		//the player has gone of the elevator trigger, remove the parent from the player
		if (col.CompareTag ("Player")) {

			setPlayerParent (null, col.gameObject.transform);

			removePassenger (col.gameObject.transform);

			if (passengersInfoList.Count == 0) {
				inside = false;
			}

			if (!doorsClosed) {
				openOrCloseElevatorDoors ();
			}
		}
	}

	//attach and disattch the player and the camera inside the elevator
	void setPlayerParent (Transform father, Transform newPassenger)
	{
		bool passengerFound = false;

		passengersInfo newPassengersInfo = new passengersInfo ();

		for (i = 0; i < passengersInfoList.Count; i++) {
			if (passengersInfoList [i].playerTransform == newPassenger && !passengerFound) {
				newPassengersInfo = passengersInfoList [i];
				passengerFound = true;
			}
		}

		if (passengerFound) {
			newPassengersInfo.playerControllerManager.setPlayerAndCameraParent (father);

			newPassengersInfo.playerControllerManager.setMovingOnPlatformActiveState (father != null);
		}
	}

	void setAllPlayersParent (Transform father)
	{
		for (i = 0; i < passengersInfoList.Count; i++) {
			passengersInfoList [i].playerControllerManager.setPlayerAndCameraParent (father);

			passengersInfoList [i].playerControllerManager.setMovingOnPlatformActiveState (father != null);
		}
	}

	public void addPassenger (Transform newPassenger)
	{
		bool passengerFound = false;
		for (i = 0; i < passengersInfoList.Count; i++) {
			if (passengersInfoList [i].playerTransform == newPassenger && !passengerFound) {
				passengerFound = true;
			}
		}

		if (!passengerFound) {
			passengersInfo newPassengersInfo = new passengersInfo ();

			newPassengersInfo.playerTransform = newPassenger;

			newPassengersInfo.playerControllerManager = newPassenger.GetComponent<playerController> ();

			passengersInfoList.Add (newPassengersInfo);
		}
	}

	void removePassenger (Transform newPassenger)
	{
		for (i = 0; i < passengersInfoList.Count; i++) {
			if (passengersInfoList [i].playerTransform == newPassenger) {
				passengersInfoList.RemoveAt (i);
			}
		}
	}

	public void setElevatorSystemEnabledState (bool state)
	{
		elevatorSystemEnabled = state;
	}

	//add a new floor, with a switch and a door, if they are enabled to add them
	public void addNewFloor ()
	{
		floorInfo newFloorInfo = new floorInfo ();

		GameObject newFloor = new GameObject ();

		newFloor.transform.SetParent (transform.parent);
		newFloor.transform.localRotation = Quaternion.identity;

		Vector3 newFloorLocalposition = Vector3.zero;

		if (floors.Count > 0) {
			newFloorLocalposition = floors [floors.Count - 1].floorPosition.position + floors [floors.Count - 1].floorPosition.up * floorHeight;
		}

		newFloor.transform.position = newFloorLocalposition;
		newFloor.name = "Floor " + floors.Count;

		newFloorInfo.name = newFloor.name;
		newFloorInfo.floorNumber = floors.Count;
		newFloorInfo.floorPosition = newFloor.transform;

		if (addSwitchInNewFloors) {
			newFloorInfo.hasFloorButton = true;
		}

		if (addDoorInNewFloors) {
			newFloorInfo.hasOutSideElevatorDoor = true;
		}

		//add a switch
		if (addSwitchInNewFloors) {
			GameObject newSwitch = (GameObject)Instantiate (elevatorSwitchPrefab, Vector3.zero, Quaternion.identity);
			newSwitch.transform.SetParent (transform.parent);
			newSwitch.transform.position = newFloorLocalposition + transform.forward * 6 - transform.right * 5;
			newSwitch.name = "Elevator Switch " + floors.Count;

			newFloorInfo.floorButton = newSwitch;
			newSwitch.transform.SetParent (newFloor.transform);

			simpleSwitch currentSimpleSwitch = newSwitch.GetComponent<simpleSwitch> ();
			currentSimpleSwitch.objectToActive = gameObject;

			GKC_Utils.updateComponent (currentSimpleSwitch);
		}

		//add a door
		if (addDoorInNewFloors) {
			GameObject newDoor = (GameObject)Instantiate (elevatorDoorPrefab, Vector3.zero, Quaternion.identity);
			newDoor.transform.SetParent (transform.parent);
			newDoor.transform.position = newFloorLocalposition + transform.forward * 5;

			newDoor.name = "Elevator Door " + floors.Count;
			newFloorInfo.outsideElevatorDoor = newDoor;
			newDoor.transform.SetParent (newFloor.transform);
		}

		floors.Add (newFloorInfo);

		updateComponent ();
	}

	public void removeFloor (int floorIndex)
	{
		if (floors [floorIndex].floorPosition != null) {
			DestroyImmediate (floors [floorIndex].floorPosition.gameObject);
		}

		floors.RemoveAt (floorIndex);

		updateComponent ();
	}

	public void removeAllFloors ()
	{
		for (i = 0; i < floors.Count; i++) {
			DestroyImmediate (floors [i].floorPosition.gameObject);
		}

		floors.Clear ();

		updateComponent ();
	}

	public void checkEventsOnMove (bool state)
	{
		if (useEventsOnMoveStartAndEnd) {
			if (state) {
				eventOnMoveStart.Invoke ();
			} else {
				eventOnMoveEnd.Invoke ();
			}
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	//draw every floor position and a line between floors
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
			if (!Application.isPlaying) {
				for (i = 0; i < floors.Count; i++) {
					if (floors [i].floorPosition != null) {
						Gizmos.color = Color.yellow;

						if (floors [i].floorNumber == currentFloor) {
							Gizmos.color = Color.red;
						}

						Gizmos.DrawSphere (floors [i].floorPosition.position, 0.6f);

						if (i + 1 < floors.Count && floors [i + 1].floorPosition != null) {
							Gizmos.color = Color.yellow;
							Gizmos.DrawLine (floors [i].floorPosition.position, floors [i + 1].floorPosition.position);
						}

						if (floors [i].floorButton != null) {
							Gizmos.color = Color.blue;
							Gizmos.DrawLine (floors [i].floorButton.transform.position, floors [i].floorPosition.position);

							Gizmos.color = Color.green;
							Gizmos.DrawSphere (floors [i].floorButton.transform.position, 0.3f);

							if (floors [i].outsideElevatorDoor) {
								Gizmos.color = Color.white;
								Gizmos.DrawLine (floors [i].floorButton.transform.position, floors [i].outsideElevatorDoor.transform.position);
							}
						}
					}
				}
			}
		}
	}

	[System.Serializable]
	public class floorInfo
	{
		public string name;
		public int floorNumber;
		public Transform floorPosition;
		public bool hasFloorButton;
		public GameObject floorButton;
		public bool hasOutSideElevatorDoor;
		public GameObject outsideElevatorDoor;

		public doorSystem outsideElevatorDoorSystem;

		public bool outsideElevatorDoorLocated;

		public bool useEventsOnMoveStartAndEnd;
		public UnityEvent eventOnMoveStart;
		public UnityEvent eventOnMoveEnd;
	}

	[System.Serializable]
	public class passengersInfo
	{
		public Transform playerTransform;
		public playerController playerControllerManager;
	}
}