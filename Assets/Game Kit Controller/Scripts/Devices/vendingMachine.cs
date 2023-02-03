using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class vendingMachine : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float radiusToSpawn;

	public bool useSpawnLimitAmount;
	public int spawnLimitAmount;

	[Space]
	[Header ("Object To Spawn Settings")]
	[Space]

	public GameObject objectToSpawn;
	public Transform spawnPosition;

	public bool spawnObjectList;
	public List<GameObject> objectListToSpawn = new List<GameObject> ();

	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;

	int currentSpawnAmount;

	//simple script to spawn vehicles in the scene, or other objects
	public void getObject ()
	{
		if (spawnObjectList) {
			for (int i = 0; i < objectListToSpawn.Count; i++) {
				spawnObject (objectListToSpawn [i]);
			}

		} else {
			spawnObject (objectToSpawn);
		}
	}

	public void spawnObject (GameObject newObject)
	{
		if (newObject == null) {
			return;
		}

		if (useSpawnLimitAmount) {
			currentSpawnAmount++;

			if (currentSpawnAmount >= spawnLimitAmount) {
				return;
			}
		}

		Vector3 positionToSpawn = spawnPosition.position;

		if (radiusToSpawn > 0) {
			Vector2 circlePosition = Random.insideUnitCircle * radiusToSpawn;
			Vector3 newSpawnPosition = new Vector3 (circlePosition.x, 0, circlePosition.y);
			positionToSpawn += newSpawnPosition;
		}

		GameObject objectToSpawnClone = (GameObject)Instantiate (newObject, positionToSpawn, spawnPosition.rotation);

		objectToSpawnClone.name = objectToSpawn.name;

		if (!objectToSpawnClone.activeSelf) {
			objectToSpawnClone.SetActive (true);
		}
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
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (spawnPosition.position, radiusToSpawn);
		}
	}
}