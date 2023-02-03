using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class hoverBoardWayPoints : MonoBehaviour
{
	public float movementSpeed;
	public bool moveInOneDirection;
	public float extraRotation;
	public float forceAtEnd;
	public float railsOffset;
	public float extraScale;
	public float triggerRadius;

	public string vehicleTag = "vehicle";

	public bool modifyMovementSpeedEnabled = true;
	public float maxMovementSpeed = 2;
	public float minMovementSpeed = 0.1f;
	public float modifyMovementSpeed = 5;

	public List<wayPointsInfo> wayPoints = new List<wayPointsInfo> ();

	public bool inside;

	public bool showGizmo;
	public Color gizmoLabelColor = Color.black;
	public float gizmoRadius;
	public bool useHandleForVertex;
	public float handleRadius;
	public Color handleGizmoColor;

	public bool showVertexHandles;

	public GameObject wayPointElement;

	int i;
	GameObject currentVehicle;
	GameObject currentvehicleCamera;
	Coroutine movement;
	bool moving;
	vehicleHUDManager currentVehicleHUDManager;

	hoverBoardController currentHoverBoardController;
	float currentVerticalDirection;
	float speedMultiplier = 1;
	float currentMovementSpeed;

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.CompareTag (vehicleTag) && !inside) {
			currentHoverBoardController = col.gameObject.GetComponent<hoverBoardController> ();

			if (currentHoverBoardController != null) {
				currentVehicle = col.gameObject;

				currentVehicleHUDManager = currentVehicle.GetComponent<vehicleHUDManager> ();

				if (currentVehicleHUDManager.isVehicleBeingDriven ()) {

					bool canActivateWaypoint = true;

					float lastTimeReleasedFromWaypoint = currentHoverBoardController.getLastTimeReleasedFromWaypoint ();

					if (!currentHoverBoardController.isUsingHoverBoardWaypoint () &&
					     lastTimeReleasedFromWaypoint > 0 &&
					     Time.time < lastTimeReleasedFromWaypoint + 0.7f) {
						canActivateWaypoint = false;
					}

					if (canActivateWaypoint) {
						currentvehicleCamera = currentVehicleHUDManager.getVehicleCameraController ().gameObject;

						pickOrReleaseVehicle (true, false);

						if (movement != null) {
							StopCoroutine (movement);
						}

						movement = StartCoroutine (moveThroughWayPoints ());
					}
				}
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (col.gameObject.CompareTag ("Player") && inside && !moving) {
			pickOrReleaseVehicle (false, false);
		}
	}

	public void pickOrReleaseVehicle (bool state, bool auto)
	{
		inside = state;

		currentVehicle.SendMessage ("enterOrExitFromWayPoint", inside);

		currentVehicle.SendMessage ("receiveWayPoints", this);

		currentvehicleCamera.SendMessage ("startOrStopFollowVehiclePosition", !inside);

		if (!inside) {
			if (movement != null) {
				StopCoroutine (movement);
			}

			if (auto) {
				currentVehicle.GetComponent<Rigidbody> ().AddForce (currentVehicle.transform.forward * (currentVehicle.GetComponent<Rigidbody> ().mass * forceAtEnd), ForceMode.Impulse);
			}

			currentVehicle = null;
			currentvehicleCamera = null;
		}
	}

	IEnumerator moveThroughWayPoints ()
	{
		moving = true;

		float closestDistance = Mathf.Infinity;

		int index = -1;

		for (i = 0; i < wayPoints.Count; i++) {
			float currentDistance = GKC_Utils.distance (wayPoints [i].wayPoint.position, currentVehicle.transform.position);

			if (currentDistance < closestDistance) {
				closestDistance = currentDistance;

				index = i;
			}
		}

		Vector3 heading = currentVehicle.transform.position - wayPoints [index].wayPoint.position;
		float distance = heading.magnitude;

		Vector3 directionToPoint = heading / distance;

		// ("player: "+directionToPoint + "-direction: "+wayPoints [index].direction.forward);
		//check if the vectors point in the same direction or not

		float angle = Vector3.Dot (directionToPoint, wayPoints [index].direction.forward);
		//print (angle);
//		if (angle < 0) {
//			print ("different direction");
//		}
		//if the vectors point in different directions, it means that the player is close to a waypoint in the opposite forward direction of the hoverboard waypoints,
		//so increase the index in 1 to move the player to the correct waypoint position, according to the forward direction used to the waypoints

		if (angle > 0) {
			//print ("same direcion");
			index++;

			if (index > wayPoints.Count - 1) {
				if (movement != null) {
					StopCoroutine (movement);
				}
			}
		}

		List<Transform> currentPath = new List<Transform> ();

		for (i = index; i < wayPoints.Count; i++) {
			currentPath.Add (wayPoints [i].direction);
		}

		if (index - 1 >= 0) {
			index--;
		} else {
			index = 0;
		}

		Vector3 extraYRotation = wayPoints [index].direction.eulerAngles + currentVehicle.transform.up * extraRotation;

		Quaternion rot = Quaternion.Euler (extraYRotation);

		foreach (Transform transformPath in  currentPath) {
			Vector3 pos = transformPath.transform.position;

			if (transformPath == currentPath [currentPath.Count - 1]) {
				pos += transformPath.forward * 2;
			}

			while (GKC_Utils.distance (currentVehicle.transform.position, pos) > .01f) {

				if (modifyMovementSpeedEnabled) {
					currentVerticalDirection = currentHoverBoardController.getVerticalAxis ();

					if (currentVerticalDirection > 0) {
						speedMultiplier = Mathf.Lerp (speedMultiplier, maxMovementSpeed, Time.deltaTime * modifyMovementSpeed);
					} else if (currentVerticalDirection < 0) {
						speedMultiplier = Mathf.Lerp (speedMultiplier, minMovementSpeed, Time.deltaTime * modifyMovementSpeed);
					} else {
						speedMultiplier = Mathf.Lerp (speedMultiplier, 1, Time.deltaTime * modifyMovementSpeed);
					}
				}

				currentMovementSpeed = speedMultiplier * movementSpeed;

				currentVehicle.transform.position = Vector3.MoveTowards (currentVehicle.transform.position, pos, Time.deltaTime * currentMovementSpeed);
				currentVehicle.transform.rotation = Quaternion.Slerp (currentVehicle.transform.rotation, rot, Time.deltaTime * currentMovementSpeed);
				currentvehicleCamera.transform.position = Vector3.MoveTowards (currentvehicleCamera.transform.position, pos, Time.deltaTime * currentMovementSpeed);

				yield return null;
			}

			extraYRotation = transformPath.eulerAngles + currentVehicle.transform.up * extraRotation;

			rot = Quaternion.Euler (extraYRotation);
		}

		moving = false;

		pickOrReleaseVehicle (false, true);
	}

	public void addNewWayPoint ()
	{
		Vector3 newPosition = transform.position;

		if (wayPoints.Count > 0) {
			newPosition = wayPoints [wayPoints.Count - 1].wayPoint.position + wayPoints [wayPoints.Count - 1].wayPoint.forward;
		}

		GameObject newWayPoint = (GameObject)Instantiate (wayPointElement, newPosition, Quaternion.identity);

		newWayPoint.transform.SetParent (transform);

		newWayPoint.name = (wayPoints.Count + 1).ToString ("000");

		wayPointsInfo newWayPointInfo = new wayPointsInfo ();

		newWayPointInfo.Name = newWayPoint.name;

		newWayPointInfo.wayPoint = newWayPoint.transform;

		newWayPointInfo.direction = newWayPoint.transform.GetChild (0);

		newWayPointInfo.trigger = newWayPoint.GetComponentInChildren<CapsuleCollider> ();

		newWayPointInfo.railMesh = newWayPoint.GetComponentInChildren<MeshRenderer> ().gameObject;

		wayPoints.Add (newWayPointInfo);

		updateComponent ();
	}

	public void removeWaypoint (int index)
	{
		wayPointsInfo currentWaypointInfo = wayPoints [index];

		DestroyImmediate (currentWaypointInfo.wayPoint);

		wayPoints.RemoveAt (index);

		updateComponent ();
	}

	public void addNewWayPointAtIndex (int index)
	{
		Vector3 newPosition = transform.position;

		if (wayPoints.Count > 0) {
			newPosition = wayPoints [index].wayPoint.position + wayPoints [index].wayPoint.forward;
		}

		GameObject newWayPoint = (GameObject)Instantiate (wayPointElement, newPosition, Quaternion.identity);

		newWayPoint.transform.SetParent (transform);

		newWayPoint.name = (index + 1).ToString ("000");

		wayPointsInfo newWayPointInfo = new wayPointsInfo ();

		newWayPointInfo.Name = newWayPoint.name;

		newWayPointInfo.wayPoint = newWayPoint.transform;

		newWayPointInfo.direction = newWayPoint.transform.GetChild (0);

		newWayPointInfo.trigger = newWayPoint.GetComponentInChildren<CapsuleCollider> ();

		newWayPointInfo.railMesh = newWayPoint.GetComponentInChildren<MeshRenderer> ().gameObject;

		wayPoints.Insert (index, newWayPointInfo);

		renameAllWaypoints ();

		updateComponent ();
	}

	public void renameAllWaypoints ()
	{
		for (int i = 0; i < wayPoints.Count; i++) {
			if (wayPoints [i].wayPoint != null) {
				wayPoints [i].Name = (i + 1).ToString ();

				wayPoints [i].wayPoint.name = (i + 1).ToString ("000");

				wayPoints [i].wayPoint.SetSiblingIndex (i);
			}
		}

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
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
		//&& !Application.isPlaying
		if (showGizmo) {
			for (i = 0; i < wayPoints.Count; i++) {
				if (wayPoints [i].wayPoint != null && wayPoints [i].direction != null) {
					
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (wayPoints [i].wayPoint.position, gizmoRadius);

					if (i + 1 < wayPoints.Count) {
						Gizmos.color = Color.white;

						Gizmos.DrawLine (wayPoints [i].wayPoint.position, wayPoints [i + 1].wayPoint.position);

						wayPoints [i].direction.LookAt (wayPoints [i + 1].wayPoint.position);
						float scaleZ = GKC_Utils.distance (wayPoints [i].wayPoint.position, wayPoints [i + 1].wayPoint.position);
						wayPoints [i].direction.localScale = new Vector3 (1, 1, scaleZ + scaleZ * extraScale);

						Gizmos.color = Color.green;
						Gizmos.DrawLine (wayPoints [i].wayPoint.position, wayPoints [i].wayPoint.position + wayPoints [i].direction.forward);
					}

					if (i == wayPoints.Count - 1 && (i - 1) >= 0 && i != 0) {
						wayPoints [i].direction.rotation = Quaternion.LookRotation (wayPoints [i].wayPoint.position - wayPoints [i - 1].wayPoint.position);

						Gizmos.color = Color.green;

						Gizmos.DrawLine (wayPoints [i].direction.position, wayPoints [i].direction.position + wayPoints [i].direction.forward);
					}

					if (i == wayPoints.Count - 1) {
						wayPoints [i].direction.localScale = Vector3.one;
					}

					wayPoints [i].trigger.radius = triggerRadius;

					wayPoints [i].railMesh.transform.localPosition = new Vector3 (wayPoints [i].railMesh.transform.localPosition.x, railsOffset, wayPoints [i].railMesh.transform.localPosition.z);
				}
			}
		}
	}

	[System.Serializable]
	public class wayPointsInfo
	{
		public string Name;
		public Transform wayPoint;
		public Transform direction;
		public CapsuleCollider trigger;
		public GameObject railMesh;
	}
}