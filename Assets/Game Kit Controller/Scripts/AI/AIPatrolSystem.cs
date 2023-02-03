using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPatrolSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool paused;
	public float minDistanceToNextPoint = 0.6f;
	public float patrolSpeed = 0.2f;
	public float returnToPatrolSpeed = 1;

	[Space]
	[Header ("Patrol Time Settings")]
	[Space]

	public bool useGeneralWaitTime = true;
	public float generalWaitTimeBetweenPoints;
	public bool moveBetweenPatrolsInOrder = true;

	public float fixedTimeToChangeBetweenPatrols;

	[Space]
	[Header ("Random Patrol Settings")]
	[Space]

	public bool changeBetweenPointRandomly = true;
	[Range (0, 10)] public int changeRandomlyProbability = 1;
	public bool useTimeToChangeBetweenPointRandomly;

	public bool useRandomTimeToChangePatrol;
	public Vector2 randomTimeLimits;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool returningToPatrol;
	public bool AIIsDestroyed;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public float gizmoRadius;

	[Space]
	[Header ("Components")]
	[Space]

	[Tooltip ("An AIWayPointPatrol path in your scene that this AI should follow. To create one add a GameObject with AIWayPointPatrol component and edit the Way Points then drag the GameObject here.")]
	public AIWayPointPatrol patrolPath;

	public Transform AICharacter;
	public AINavMesh mainAINavmesh;

	bool patrolAssigned;

	Transform currentWayPoint;
	int currentPatrolIndex = 0;
	int currentWaypointIndex = 0;
	Coroutine movement;
	bool settingNextPoint;
	float lastTimeChanged;
	float distanceToPoint;

	Transform closestWaypointTransform;

	void Start ()
	{
		if (AICharacter == null) {
			AICharacter = transform;
		}

		if (mainAINavmesh == null) {
			mainAINavmesh = AICharacter.GetComponent<AINavMesh> ();
		}

		if (patrolPath) {
			setClosestWayPoint ();
		}
	}

	void Update ()
	{
		if (!paused && patrolAssigned && !settingNextPoint) {

			if (AIIsDestroyed) {
				enabled = false;

				return;
			}

			if (!mainAINavmesh.isPatrolPaused ()) {
				if (AICharacter == null) {
					AIIsDestroyed = true;

					return;
				}

				distanceToPoint = GKC_Utils.distance (AICharacter.position, currentWayPoint.position);

				if (distanceToPoint < minDistanceToNextPoint) {
					if (returningToPatrol) {
						mainAINavmesh.setPatrolSpeed (patrolSpeed);

						returningToPatrol = false;
					}
						
					bool setRandomWayPoint = false;

					if (changeBetweenPointRandomly) {
						int changeOrNotBool = Random.Range (0, (changeRandomlyProbability + 1));

						if (changeOrNotBool == 0) {
							setRandomWayPoint = true;
							//print ("random waypoint");
						}
					}

					if (setRandomWayPoint) {
						setNextRandomWaypoint ();
					} else {
						setNextWaypoint ();
						//print ("in order");
					}
				}

				if (changeBetweenPointRandomly) {
					if (useTimeToChangeBetweenPointRandomly) {
						if (useRandomTimeToChangePatrol) {

						} else {
							if (Time.time > fixedTimeToChangeBetweenPatrols + lastTimeChanged) {
								lastTimeChanged = Time.time;

								setNextRandomWaypoint ();
								//print ("random waypoint");
							}
						}
					}
				}
			}
		}
	}

	public void pauseOrPlayPatrol (bool state)
	{
//		print ("patrol paused");

		paused = state;
	}

	public bool isPatrolPaused ()
	{
		return paused;
	}

	public Transform closestWaypoint (Vector3 currentPosition)
	{
		float distance = Mathf.Infinity;

		for (int i = 0; i < patrolPath.patrolList.Count; i++) {
			for (int j = 0; j < patrolPath.patrolList [i].wayPoints.Count; j++) {
				float currentDistance = GKC_Utils.distance (currentPosition, patrolPath.patrolList [i].wayPoints [j].position);

				if (currentDistance < distance) {
					distance = currentDistance;
					currentPatrolIndex = i;
					currentWaypointIndex = j;
				}
			}
		}

		closestWaypointTransform = patrolPath.patrolList [currentPatrolIndex].wayPoints [currentWaypointIndex];

		return closestWaypointTransform;
	}

	public void setNextWaypoint ()
	{
		if (movement != null) {
			StopCoroutine (movement);
		}

		settingNextPoint = false;

		if (mainAINavmesh.isPatrolPaused ()) {
			return;
		}

		movement = StartCoroutine (setNextWayPointCoroutine ());
	}

	IEnumerator setNextWayPointCoroutine ()
	{
		mainAINavmesh.removeTarget ();

		settingNextPoint = true;

		if (useGeneralWaitTime) {
			yield return new WaitForSeconds (generalWaitTimeBetweenPoints);
		} else {
			yield return new WaitForSeconds (patrolPath.waitTimeBetweenPoints);
		}

		if (!mainAINavmesh.isPatrolPaused ()) {
			currentWaypointIndex++;

			if (currentWaypointIndex > patrolPath.patrolList [currentPatrolIndex].wayPoints.Count - 1) {
				currentWaypointIndex = 0;

				if (moveBetweenPatrolsInOrder) {
					currentPatrolIndex++;

					if (currentPatrolIndex > patrolPath.patrolList.Count - 1) {
						currentPatrolIndex = 0;
					}
				}
			}

			currentWayPoint = patrolPath.patrolList [currentPatrolIndex].wayPoints [currentWaypointIndex];

			setCurrentPatrolTarget (currentWayPoint);
		}

		settingNextPoint = false;
	}

	public void setNextRandomWaypoint ()
	{
		if (movement != null) {
			StopCoroutine (movement);
		}

		settingNextPoint = false;

		if (mainAINavmesh.isPatrolPaused ()) {
			return;
		}

		movement = StartCoroutine (setNextRandomWayPointCoroutine ());
	}

	IEnumerator setNextRandomWayPointCoroutine ()
	{
		mainAINavmesh.removeTarget ();

		settingNextPoint = true;

		if (useGeneralWaitTime) {
			yield return new WaitForSeconds (generalWaitTimeBetweenPoints);
		} else {
			yield return new WaitForSeconds (patrolPath.waitTimeBetweenPoints);
		}

		if (!mainAINavmesh.isPatrolPaused ()) {

			int currentWaypointIndexCopy = currentWaypointIndex;
			int currentPatrolIndexCopy = currentPatrolIndex;
			int checkBucle = 0;

			if (patrolPath.patrolList.Count > 1) {
				while (currentPatrolIndexCopy == currentPatrolIndex) {
					currentPatrolIndex = Random.Range (0, patrolPath.patrolList.Count);

					checkBucle++;

					if (checkBucle > 100) {
						//	print ("bucle error");
						break;
					}
				}
			}

			checkBucle = 0;

			while (currentWaypointIndexCopy == currentWaypointIndex) {
				currentWaypointIndex = Random.Range (0, patrolPath.patrolList [currentPatrolIndex].wayPoints.Count);

				checkBucle++;

				if (checkBucle > 100) {
					//print ("bucle error");
					break;
				}
			}

			//print ("Next patrol: " + (currentPatrolIndex+1) + " and next waypoint: " + (currentWaypointIndex+1));
			currentWayPoint = patrolPath.patrolList [currentPatrolIndex].wayPoints [currentWaypointIndex];

			setCurrentPatrolTarget (currentWayPoint);
		}

		settingNextPoint = false;
	}

	public void setClosestWayPoint ()
	{
		if (paused) {
			return;
		}

		patrolAssigned = true;

		currentWayPoint = closestWaypoint (AICharacter.position);

		setCurrentPatrolTarget (currentWayPoint);
	
		mainAINavmesh.setPatrolSpeed (patrolSpeed);
	}

	public void setCurrentPatrolTarget (Transform newTarget)
	{
		mainAINavmesh.setPatrolTarget (newTarget);

		mainAINavmesh.setPatrolState (true);
	}

	public void setReturningToPatrolState (bool state)
	{
		returningToPatrol = true;

		if (returningToPatrol) {
			mainAINavmesh.setPatrolSpeed (returnToPatrolSpeed);
		}
	}

	public void resumePatrolStateOnAI ()
	{
		pauseOrPlayPatrol (false);

		setClosestWayPoint ();
	}

	public void pausePatrolStateOnAI ()
	{
		pauseOrPlayPatrol (true);

		mainAINavmesh.setPatrolState (false);
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
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (patrolPath.patrolList [currentPatrolIndex].wayPoints [currentWaypointIndex].transform.position, gizmoRadius);
		}
	}
}