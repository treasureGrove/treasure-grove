using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class cameraWaypointSystem : MonoBehaviour
{
	public Transform currentCameraTransform;
	public List<cameraWaypointInfo> waypointList = new List<cameraWaypointInfo> ();
	public float waitTimeBetweenPoints;
	public float movementSpeed;
	public float rotationSpeed;

	public Transform pointToLook;

	public bool useEventOnEnd;
	public UnityEvent eventOnEnd;

	public bool showGizmo;
	public Color gizmoLabelColor = Color.black;
	public float gizmoRadius;
	public bool useHandleForWaypoints;
	public float handleRadius;
	public Color handleGizmoColor;
	public bool showWaypointHandles;

	public float currentMovementSpeed;
	public float currentRotationSpeed;

	public bool useBezierCurve;
	public BezierSpline spline;
	public float bezierDuration = 10;
	public bool useExternalProgress;
	[NonSerialized]
	public Func<float> externalProgress;
	public bool snapCameraToFirstSplinePoint;

	public bool searchPlayerOnSceneIfNotAssigned = true;

	float currentWaitTime;
	Vector3 targetDirection;

	Coroutine movement;
	Transform currentWaypoint;
	int currentWaypointIndex;

	int i;

	List<Transform> currentPath = new List<Transform> ();
	cameraWaypointInfo currentCameraWaypointInfo;

	int previousWaypointIndex;

	Vector3 targetPosition;
	Quaternion targetRotation;

	public void setCurrentCameraTransform (GameObject cameraGameObject)
	{
		if (cameraGameObject == null) {
			return;
		}

		currentCameraTransform = cameraGameObject.transform;
	}

	public void findPlayerOnScene ()
	{
		if (searchPlayerOnSceneIfNotAssigned) {
			GameObject currentPlayer = GKC_Utils.findMainPlayerOnScene ();

			if (currentPlayer != null) {
				playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

				if (mainPlayerComponentsManager != null) {
					playerCamera playerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();

					setCurrentCameraTransform (playerCameraManager.getMainCamera ().gameObject);
				}
			}
		}
	}

	//stop the platform coroutine movement and play again
	public void checkMovementCoroutine (bool play)
	{
		stopMoveThroughWayPointsCoroutine ();

		if (play) {
			if (currentCameraTransform == null) {
				findPlayerOnScene ();

				if (currentCameraTransform == null) {
					print ("WARNING: no current camera transform has been assigned on the camera waypoint system." +
					" Make sure to use a trigger to activate the element or assign the player manually");

					return;
				}
			}

			movement = StartCoroutine (moveThroughWayPoints ());
		}
	}

	public void stopMoveThroughWayPointsCoroutine ()
	{
		if (movement != null) {
			StopCoroutine (movement);
		}
	}

	IEnumerator moveThroughWayPoints ()
	{
		currentWaypointIndex = 0;

		previousWaypointIndex = -1;

		//if the current path to move has waypoints, then
		if (currentPath.Count == 0) {
			for (i = 0; i < waypointList.Count; i++) {
				currentPath.Add (waypointList [i].waypointTransform);
			}
		}

		if (currentPath.Count > 0) {
			if (useBezierCurve) {
				yield return moveAlongBezierCurve ();
			} else {
				yield return moveAlongWaypoints ();
			}

			if (useEventOnEnd) {
				eventOnEnd.Invoke ();
			}
		} else {
			//else, stop the movement
			checkMovementCoroutine (false);
		}
	}

	private IEnumerator moveAlongWaypoints ()
	{
		//move between every waypoint
		foreach (Transform waypoint in currentPath) {
			currentWaypoint = waypoint;
			currentCameraWaypointInfo = waypointList [currentWaypointIndex];

			//wait the amount of time configured
			if (currentCameraWaypointInfo.useCustomWaitTimeBetweenPoint) {
				currentWaitTime = currentCameraWaypointInfo.waitTimeBetweenPoints;
			} else {
				currentWaitTime = waitTimeBetweenPoints;
			}

			targetPosition = currentWaypoint.position;
			targetRotation = currentWaypoint.rotation;

			yield return new WaitForSeconds (currentWaitTime);			

			if (currentCameraWaypointInfo.useCustomMovementSpeed) {
				currentMovementSpeed = currentCameraWaypointInfo.movementSpeed;
			} else {
				currentMovementSpeed = movementSpeed;
			}

			if (currentCameraWaypointInfo.useCustomRotationSpeed) {
				currentRotationSpeed = currentCameraWaypointInfo.rotationSpeed;
			} else {
				currentRotationSpeed = rotationSpeed;
			}

			if (currentCameraWaypointInfo.smoothTransitionToNextPoint) {
				//while the platform moves from the previous waypoint to the next, then displace it
				while (GKC_Utils.distance (currentCameraTransform.position, targetPosition) > .01f) {
					currentCameraTransform.position = Vector3.MoveTowards (currentCameraTransform.position, targetPosition, Time.deltaTime * currentMovementSpeed);

					if (currentCameraWaypointInfo.rotateCameraToNextWaypoint) {
						targetDirection = targetPosition - currentCameraTransform.position;
					} 

					if (currentCameraWaypointInfo.usePointToLook) {
						targetDirection = currentCameraWaypointInfo.pointToLook.position - currentCameraTransform.position;
					}

					if (targetDirection != Vector3.zero) {
						targetRotation = Quaternion.LookRotation (targetDirection);
						currentCameraTransform.rotation = Quaternion.Lerp (currentCameraTransform.rotation, targetRotation, Time.deltaTime * currentRotationSpeed);
					}

					yield return null;
				}
			} else {
				currentCameraTransform.position = targetPosition;

				if (currentCameraWaypointInfo.rotateCameraToNextWaypoint) {
					targetDirection = targetPosition - currentCameraTransform.position;
				} 

				if (currentCameraWaypointInfo.usePointToLook) {
					targetDirection = currentCameraWaypointInfo.pointToLook.position - currentCameraTransform.position;
				}

				if (!currentCameraWaypointInfo.rotateCameraToNextWaypoint && !currentCameraWaypointInfo.usePointToLook) {
					currentCameraTransform.rotation = currentCameraWaypointInfo.waypointTransform.rotation;
				} else {
					if (targetDirection != Vector3.zero) {
						currentCameraTransform.rotation = Quaternion.LookRotation (targetDirection);
					}
				}

				yield return new WaitForSeconds (currentCameraWaypointInfo.timeOnFixedPosition);
			}

			if (currentCameraWaypointInfo.useEventOnPointReached) {
				currentCameraWaypointInfo.eventOnPointReached.Invoke ();
			}

			//when the platform reaches the next waypoint
			currentWaypointIndex++;
		}
	}

	private IEnumerator moveAlongBezierCurve ()
	{
		if (!snapCameraToFirstSplinePoint) {
			spline.setInitialSplinePoint (currentCameraTransform.position);
		}

		float progress = 0;
		float progressTarget = 1;

		bool targetReached = false;

		while (!targetReached) {

			if (previousWaypointIndex != currentWaypointIndex) {

				if (previousWaypointIndex != -1) {
					if (currentCameraWaypointInfo.useEventOnPointReached) {
						currentCameraWaypointInfo.eventOnPointReached.Invoke ();
					}
				}

				previousWaypointIndex = currentWaypointIndex;

				currentCameraWaypointInfo = waypointList [currentWaypointIndex];

				currentWaypoint = currentCameraWaypointInfo.waypointTransform;

				//wait the amount of time configured
				if (currentCameraWaypointInfo.useCustomWaitTimeBetweenPoint) {
					currentWaitTime = currentCameraWaypointInfo.waitTimeBetweenPoints;
				} else {
					currentWaitTime = waitTimeBetweenPoints;
				}

				targetPosition = currentWaypoint.position;
				targetRotation = currentWaypoint.rotation;

				yield return new WaitForSeconds (currentWaitTime);			

				if (currentCameraWaypointInfo.useCustomMovementSpeed) {
					currentMovementSpeed = currentCameraWaypointInfo.movementSpeed;
				} else {
					currentMovementSpeed = movementSpeed;
				}

				if (currentCameraWaypointInfo.useCustomRotationSpeed) {
					currentRotationSpeed = currentCameraWaypointInfo.rotationSpeed;
				} else {
					currentRotationSpeed = rotationSpeed;
				}
			}

			currentWaypointIndex = spline.getPointIndex (progress);

			if (useExternalProgress) {
				if (externalProgress != null) {
					progress = externalProgress ();
				} else {
					Debug.LogError ("useExternalProgress is set but no externalProgress func is assigned");
				}
			} else {
				progress += Time.deltaTime / (bezierDuration * currentMovementSpeed);
			}

			Vector3 position = spline.GetPoint (progress);
			currentCameraTransform.position = position;

			if (currentCameraWaypointInfo.rotateCameraToNextWaypoint) {
				targetDirection = targetPosition - currentCameraTransform.position;
			} 

			if (currentCameraWaypointInfo.usePointToLook) {
				targetDirection = currentCameraWaypointInfo.pointToLook.position - currentCameraTransform.position;
			}

			if (targetDirection != Vector3.zero) {
				targetRotation = Quaternion.LookRotation (targetDirection);
				currentCameraTransform.rotation = Quaternion.Lerp (currentCameraTransform.rotation, targetRotation, Time.deltaTime * currentRotationSpeed);
			}			

			if (progress > progressTarget) {
				targetReached = true;
			}

			yield return null;
		}
	}

	//add a new waypoint
	public void addNewWayPoint ()
	{
		Vector3 newPosition = transform.position;

		if (waypointList.Count > 0) {
			newPosition = waypointList [waypointList.Count - 1].waypointTransform.position + waypointList [waypointList.Count - 1].waypointTransform.forward;
		}

		GameObject newWayPoint = new GameObject ();
		newWayPoint.transform.SetParent (transform);
		newWayPoint.transform.position = newPosition;
		newWayPoint.name = (waypointList.Count + 1).ToString ();

		cameraWaypointInfo newCameraWaypointInfo = new cameraWaypointInfo ();
		newCameraWaypointInfo.Name = newWayPoint.name;
		newCameraWaypointInfo.waypointTransform = newWayPoint.transform;
		newCameraWaypointInfo.rotateCameraToNextWaypoint = true;

		waypointList.Add (newCameraWaypointInfo);

		updateComponent ();
	}

	public void addNewWayPoint (int insertAtIndex)
	{
		GameObject newWayPoint = new GameObject ();
		newWayPoint.transform.SetParent (transform);
		newWayPoint.name = (waypointList.Count + 1).ToString ();

		cameraWaypointInfo newCameraWaypointInfo = new cameraWaypointInfo ();
		newCameraWaypointInfo.Name = newWayPoint.name;
		newCameraWaypointInfo.waypointTransform = newWayPoint.transform;
		newCameraWaypointInfo.rotateCameraToNextWaypoint = true;

		if (waypointList.Count > 0) {
			Vector3 lastPosition = waypointList [waypointList.Count - 1].waypointTransform.position + waypointList [waypointList.Count - 1].waypointTransform.forward;
			newWayPoint.transform.localPosition = lastPosition + waypointList [waypointList.Count - 1].waypointTransform.forward * 2;
		} else {
			newWayPoint.transform.localPosition = Vector3.zero;
		}

		if (insertAtIndex > -1) {
			if (waypointList.Count > 0) {
				newWayPoint.transform.localPosition = waypointList [insertAtIndex].waypointTransform.localPosition + waypointList [insertAtIndex].waypointTransform.forward * 2;
			}

			waypointList.Insert (insertAtIndex + 1, newCameraWaypointInfo);

			newWayPoint.transform.SetSiblingIndex (insertAtIndex + 1);

			renameAllWaypoints ();
		} else {
			waypointList.Add (newCameraWaypointInfo);
		}

		updateComponent ();
	}

	public void renameAllWaypoints ()
	{
		for (int i = 0; i < waypointList.Count; i++) {
			if (waypointList [i].waypointTransform != null) {
				waypointList [i].waypointTransform.name = (i + 1).ToString ("000");
				waypointList [i].Name = (i + 1).ToString ("000");
			}
		}

		updateComponent ();
	}

	public void removeWaypoint (int index)
	{
		if (waypointList [index].waypointTransform != null) {
			DestroyImmediate (waypointList [index].waypointTransform.gameObject);
		}

		waypointList.RemoveAt (index);

		updateComponent ();
	}

	public void removeAllWaypoints ()
	{
		for (int i = 0; i < waypointList.Count; i++) {
			if (waypointList [i].waypointTransform != null) {
				DestroyImmediate (waypointList [i].waypointTransform.gameObject);
			}
		}

		waypointList.Clear ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Camera Waypoin System", gameObject);
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
			if (waypointList.Count > 0) {
				if (waypointList [0].waypointTransform != null) {
					Gizmos.color = Color.white;
					Gizmos.DrawLine (waypointList [0].waypointTransform.position, transform.position);
				}
			}

			for (i = 0; i < waypointList.Count; i++) {
				if (waypointList [i].waypointTransform != null) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (waypointList [i].waypointTransform.position, gizmoRadius);

					if (i + 1 < waypointList.Count) {
						Gizmos.color = Color.white;
						Gizmos.DrawLine (waypointList [i].waypointTransform.position, waypointList [i + 1].waypointTransform.position);
					}

					if (currentWaypoint) {
						Gizmos.color = Color.red;
						Gizmos.DrawSphere (currentWaypoint.position, gizmoRadius);
					}

					if (waypointList [i].usePointToLook && waypointList [i].pointToLook != null) {
						Gizmos.color = Color.green;
						Gizmos.DrawLine (waypointList [i].waypointTransform.position, waypointList [i].pointToLook.position);
						Gizmos.color = Color.blue;
						Gizmos.DrawSphere (waypointList [i].pointToLook.position, gizmoRadius);
					}
				}
			}
		}
	}

	[System.Serializable]
	public class cameraWaypointInfo
	{
		public string Name;
		public Transform waypointTransform;
	
		public bool rotateCameraToNextWaypoint;
		public bool usePointToLook;
		public Transform pointToLook;

		public bool smoothTransitionToNextPoint = true;
		public bool useCustomMovementSpeed;
		public float movementSpeed;
		public bool useCustomRotationSpeed;
		public float rotationSpeed;

		public float timeOnFixedPosition;

		public bool useCustomWaitTimeBetweenPoint;
		public float waitTimeBetweenPoints;

		public bool useEventOnPointReached;
		public UnityEvent eventOnPointReached;
	}
}
