using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class waypointPlatform : MonoBehaviour
{
	public bool platformActive = true;
	public Transform platformTransform;
	public List<Transform> wayPoints = new List<Transform> ();
	public Transform waypointsParent;
	public bool repeatWaypoints;
	public bool moveInCircles;
	public bool stopIfPlayerOutside;
	public float waitTimeBetweenPoints;
	public float movementSpeed;
	public bool movingForward = true;

	public string playerTag = "Player";
	public string vehicleTag = "vehicle";

	public bool useJustToMovePlatform;

	public bool showGizmo;
	public Color gizmoLabelColor = Color.black;
	public float gizmoRadius;
	public bool useHandleForVertex;
	public float handleRadius;
	public Color handleGizmoColor;

	public bool showVertexHandles;

	public List<string> tagToCheckToMove = new List<string> ();

	public List<string> tagToCheckBelow = new List<string> ();

	public List<GameObject> objectsDetectedBelowList = new List<GameObject> ();

	public bool mirrorPlatformMovement;
	public Vector3 mirrorMovementDirection = Vector3.one;
	public waypointPlatform platformToMirror;

	public bool useEventOnWaypointReached;
	public List<eventOnWaypointInfo> eventOnWaypointReachedList = new List<eventOnWaypointInfo> ();

	public List<passengersInfo> passengersInfoList = new List<passengersInfo> ();

	public List<vehiclesInfo> vehiclesInfoList = new List<vehiclesInfo> ();

	public deviceStringAction deviceStringActionManager;

	List<Transform> forwardPath = new List<Transform> ();
	List<Transform> inversePath = new List<Transform> ();
	List<Transform> currentPath = new List<Transform> ();
	Coroutine movement;
	Transform currentWaypoint;
	int currentPlatformIndex;
	int i;

	bool activateMovementForward;

	Vector3 currentPlatformPosition;
	bool mirroringPlatformActive;

	bool settingPositionOnMirrorPlatflorm;
	public float distanceToMirror;


	void Start ()
	{
		if (platformTransform == null) {
			platformTransform = transform;
		}

		forwardPath = new List<Transform> (wayPoints);
		inversePath = new List<Transform> (wayPoints);

		inversePath.Reverse ();

		if (!stopIfPlayerOutside && !useJustToMovePlatform && platformActive) {
			checkMovementCoroutine (true);
		}

		if (deviceStringActionManager == null) {
			deviceStringActionManager = GetComponent<deviceStringAction> ();
		}
	}

	void Update ()
	{
		if (mirroringPlatformActive) {
			platformTransform.position = currentPlatformPosition +
			new Vector3 (platformToMirror.distanceToMirror * mirrorMovementDirection.x,
				platformToMirror.distanceToMirror * mirrorMovementDirection.y,
				platformToMirror.distanceToMirror * mirrorMovementDirection.z);
		}

		if (settingPositionOnMirrorPlatflorm) {
			distanceToMirror = GKC_Utils.distance (currentPlatformPosition, platformTransform.position);
		}
	}

	public void setMirroringPlatformActiveState (bool state)
	{
		if (!mirrorPlatformMovement) {
			return;
		}

		mirroringPlatformActive = state;

		if (platformTransform != null) {
			currentPlatformPosition = platformTransform.position;
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (useJustToMovePlatform) {
			return;
		}

		//if the player enters inside the platform trigger, then
		if (col.gameObject.CompareTag (playerTag)) {
			//store him
			addPassenger (col.gameObject.transform);

			//if he is not driving, then attach the player and the camera inside the platform
			setPlayerParent (platformTransform, col.gameObject.transform);

			//if the platform stops when the player exits from it, then restart its movement
			if (stopIfPlayerOutside) {
				checkMovementCoroutine (true);
			}

		} else if (col.gameObject.CompareTag (vehicleTag)) {
			//store him
			addVehicle (col.gameObject.transform);

			//if he is not driving, then attach the player and the camera inside the platform
			setVehicleParent (platformTransform, col.gameObject.transform);

		} else if (tagToCheckToMove.Contains (col.gameObject.tag)) {
			col.gameObject.transform.SetParent (platformTransform);
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (useJustToMovePlatform) {
			return;
		}

		//if the player exits, then disattach the player
		if (col.gameObject.CompareTag (playerTag)) {
			setPlayerParent (null, col.gameObject.transform);

			removePassenger (col.gameObject.transform);

			//if the platform stops when the player exits from it, stop the platform
			if (stopIfPlayerOutside) {
				checkMovementCoroutine (false);
			}
		} else if (col.gameObject.CompareTag (vehicleTag)) {
			setVehicleParent (null, col.gameObject.transform);

			removeVehicle (col.gameObject.transform);

		} else if (tagToCheckToMove.Contains (col.gameObject.tag)) {
			col.gameObject.transform.SetParent (null);
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

	void setVehicleParent (Transform father, Transform newVehicle)
	{
		bool vehicleFound = false;

		vehiclesInfo newVehiclesInfo = new vehiclesInfo ();

		for (i = 0; i < vehiclesInfoList.Count; i++) {
			if (vehiclesInfoList [i].vehicleTransform == newVehicle && !vehicleFound) {
				newVehiclesInfo = vehiclesInfoList [i];
				vehicleFound = true;
			}
		}

		if (vehicleFound) {
			newVehiclesInfo.HUDManager.setVehicleAndCameraParent (father);
		}
	}

	void setAllVehiclesParent (Transform father)
	{
		for (i = 0; i < vehiclesInfoList.Count; i++) {
			vehiclesInfoList [i].HUDManager.setVehicleAndCameraParent (father);
		}
	}

	public void addVehicle (Transform newVehicle)
	{
		bool vehicleFound = false;
		for (i = 0; i < vehiclesInfoList.Count; i++) {
			if (vehiclesInfoList [i].vehicleTransform == newVehicle && !vehicleFound) {
				vehicleFound = true;
			}
		}

		if (!vehicleFound) {
			vehiclesInfo newVehiclesInfo = new vehiclesInfo ();
			newVehiclesInfo.vehicleTransform = newVehicle;

			newVehiclesInfo.HUDManager = newVehicle.GetComponent<vehicleHUDManager> ();

			vehiclesInfoList.Add (newVehiclesInfo);
		}
	}

	void removeVehicle (Transform newVehicle)
	{
		for (i = 0; i < vehiclesInfoList.Count; i++) {
			if (vehiclesInfoList [i].vehicleTransform == newVehicle) {
				vehiclesInfoList.RemoveAt (i);
			}
		}
	}

	public void disablePlatform ()
	{
		stopPlatformMovement ();

		platformActive = false;

		setAllPlayersParent (null);

		setAllVehiclesParent (null);

		GetComponent<Collider> ().enabled = false;

		this.enabled = false;
	}

	public void activatePlatformMovement ()
	{
		if (currentPath.Count == 0) {
			for (i = currentPlatformIndex; i < forwardPath.Count; i++) {
				currentPath.Add (forwardPath [i]);
			}
		}

		checkMovementCoroutine (true);
	}

	public void deactivatePlatformMovement ()
	{
		checkMovementCoroutine (false);
	}

	//stop the platform coroutine movement and play again
	public void checkMovementCoroutine (bool play)
	{
		stopPlatformMovement ();

		if (!platformActive) {
			return;
		}

		if (mirrorPlatformMovement) {
			platformToMirror.setMirroringPlatformActiveState (play);
			settingPositionOnMirrorPlatflorm = play;
			currentPlatformPosition = transform.position;
		}

		if (play) {
			movement = StartCoroutine (moveThroughWayPoints ());
		}
	}

	public void stopPlatformMovement ()
	{
		if (movement != null) {
			StopCoroutine (movement);
		}
	}

	IEnumerator moveThroughWayPoints ()
	{
		if (!useJustToMovePlatform) {
			currentPath.Clear ();

			//if the platform moves from waypoint to waypoint and it starts again, then
			if (moveInCircles) {
				//from the current waypoint to the last of them, add these waypoints
				for (i = currentPlatformIndex; i < forwardPath.Count; i++) {
					currentPath.Add (forwardPath [i]);
				}
			} else {
				//else, if only moves from the first waypoint to the last and then stop, then
				//if the platform moves between waypoins in the order list
				if (movingForward) {
					//from the current waypoint to the last of them, add these waypoints
					for (i = currentPlatformIndex; i < forwardPath.Count; i++) {
						currentPath.Add (forwardPath [i]);
					}
				} else {
					//from the current waypoint to the first of them, add these waypoints, making the reverse path
					for (i = currentPlatformIndex; i < inversePath.Count; i++) {
						currentPath.Add (inversePath [i]);
					}
				}
			}
		}
			
		//if the current path to move has waypoints, then
		if (currentPath.Count > 0) {
			//move between every waypoint
			foreach (Transform point in  currentPath) {
				//wait the amount of time configured
				yield return new WaitForSeconds (waitTimeBetweenPoints);

				Vector3 pos = point.position;
				Quaternion rot = point.rotation;

				currentWaypoint = point;

				//while the platform moves from the previous waypoint to the next, then displace it
				while (GKC_Utils.distance (platformTransform.position, pos) > .01f) {
					platformTransform.position = Vector3.MoveTowards (platformTransform.position, pos, Time.deltaTime * movementSpeed);
					platformTransform.rotation = Quaternion.Slerp (platformTransform.rotation, rot, Time.deltaTime * movementSpeed);

					yield return null;
				}

				//when the platform reaches the next waypoint
				currentPlatformIndex++;

				int currentIndex = currentPlatformIndex;

				if (currentPlatformIndex > wayPoints.Count - 1) {
					currentPlatformIndex = 0;
					movingForward = !movingForward;
				}

				if (useEventOnWaypointReached) {
					for (i = 0; i < eventOnWaypointReachedList.Count; i++) {
						if (currentIndex == (eventOnWaypointReachedList [i].waypointToReach)) {
							eventOnWaypointReachedList [i].eventOnWaypoint.Invoke ();
						}
					}
				}
			}

			//if the platform moves in every moment, then repeat the path
			if (repeatWaypoints) {
				checkMovementCoroutine (true);
			}
		} else {
			//else, stop the movement
			checkMovementCoroutine (false);
		}
	}

	public void activetMovementByButton ()
	{
		activateMovementForward = !activateMovementForward;

		currentPath.Clear ();

		if (activateMovementForward) {
			//from the current waypoint to the last of them, add these waypoints
			for (i = currentPlatformIndex; i < forwardPath.Count; i++) {
				currentPath.Add (forwardPath [i]);
			}
		} else {
			//from the current waypoint to the first of them, add these waypoints, making the reverse path
			for (i = currentPlatformIndex; i < inversePath.Count; i++) {
				currentPath.Add (inversePath [i]);
			}
		}

		setDeviceStringActionState (activateMovementForward);

		checkMovementCoroutine (true);
	}

	public void setDeviceStringActionState (bool state)
	{
		if (deviceStringActionManager != null) {
			deviceStringActionManager.changeActionName (state);
		}
	}

	public void addObjectDetectedBelow (GameObject objectToCheck)
	{
		if (!objectsDetectedBelowList.Contains (objectToCheck)) {
			if (tagToCheckBelow.Contains (objectToCheck.tag) || objectToCheck.GetComponent<Rigidbody> ()) {
				
				objectsDetectedBelowList.Add (objectToCheck);

				currentPlatformIndex = 0;

				checkMovementCoroutine (true);
			}
		}
	}

	public void removeObjectDetectedBelow (GameObject objectToCheck)
	{
		if (objectsDetectedBelowList.Contains (objectToCheck)) {
			objectsDetectedBelowList.Remove (objectToCheck);
		}
	}

	//add a new waypoint
	public void addNewWayPoint ()
	{
		Vector3 newPosition = Vector3.zero;

		if (platformTransform == null) {
			newPosition = transform.position;
		} else {
			newPosition = platformTransform.position;
		}

		if (wayPoints.Count > 0) {
			newPosition = wayPoints [wayPoints.Count - 1].position + wayPoints [wayPoints.Count - 1].forward;
		}

		GameObject newWayPoint = new GameObject ();
		newWayPoint.transform.SetParent (waypointsParent);
		newWayPoint.transform.position = newPosition;
		newWayPoint.name = (wayPoints.Count + 1).ToString ();
		wayPoints.Add (newWayPoint.transform);

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Waypoint Platform " + gameObject.name, gameObject);
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

	void DrawGizmos ()
	{
		if (showGizmo) {
			for (i = 0; i < wayPoints.Count; i++) {
				if (wayPoints [i] != null) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (wayPoints [i].position, gizmoRadius);

					if (i + 1 < wayPoints.Count) {
						Gizmos.color = Color.white;
						Gizmos.DrawLine (wayPoints [i].position, wayPoints [i + 1].position);
					}

					if (i == wayPoints.Count - 1 && moveInCircles) {
						Gizmos.color = Color.white;
						Gizmos.DrawLine (wayPoints [i].position, wayPoints [0].position);
					}

					if (currentWaypoint != null) {
						Gizmos.color = Color.red;
						Gizmos.DrawSphere (currentWaypoint.position, gizmoRadius);
					}
				}
			}
		}
	}

	[System.Serializable]
	public class eventOnWaypointInfo
	{
		public string Name;
		public int waypointToReach;
		public UnityEvent eventOnWaypoint;
	}

	[System.Serializable]
	public class passengersInfo
	{
		public Transform playerTransform;
		public playerController playerControllerManager;
	}

	[System.Serializable]
	public class vehiclesInfo
	{
		public Transform vehicleTransform;
		public vehicleHUDManager HUDManager;
	}
}