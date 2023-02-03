using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addPatrolSystemToAI : MonoBehaviour
{
	public GameObject AIWaypointPatrolPrefab;

	public void addPatrolSystem ()
	{
		if (AIWaypointPatrolPrefab == null) {
			print ("Patrol prefab not configured");

			return;
		}

		findObjectivesSystem currentFindObjectivesSystem = GetComponentInChildren<findObjectivesSystem> ();

		if (currentFindObjectivesSystem != null && currentFindObjectivesSystem.AIPatrolManager == null) {

			GameObject newAIWaypointPatrol = (GameObject)Instantiate (AIWaypointPatrolPrefab, currentFindObjectivesSystem.transform.position + Vector3.forward * 6, Quaternion.identity);
			newAIWaypointPatrol.name = "AI Waypoint Patrol";

			AIPatrolSystem currentAIPatrolSystem = newAIWaypointPatrol.GetComponent<AIPatrolSystem> ();

			currentAIPatrolSystem.AICharacter = currentFindObjectivesSystem.transform;

			AINavMesh mainAINavMesh = GetComponentInChildren<AINavMesh> ();

			currentAIPatrolSystem.mainAINavmesh = mainAINavMesh;

			currentFindObjectivesSystem.AIPatrolManager = currentAIPatrolSystem;

			GKC_Utils.updateComponent (currentFindObjectivesSystem);
			GKC_Utils.updateComponent (currentAIPatrolSystem);

			GKC_Utils.updateDirtyScene ("Adding patrol to AI", gameObject);

			print ("Patrol system added to AI");
		} else {
			print ("WARNING: patrol system already configured on this AI");
		}
	}
}
