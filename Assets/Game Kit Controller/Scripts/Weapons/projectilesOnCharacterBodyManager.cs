using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectilesOnCharacterBodyManager : MonoBehaviour
{
	public float timeToRemoveProjectiles = 5;

	public float waitTimeBetweenProjectileRemoving = 2;

	List<GameObject> projectilesGameObjectList = new List<GameObject> ();

	Coroutine removeCoroutine;

	float currentRemoveProjectileWaitTime;


	public void addProjectileToCharacterBody (GameObject newProjectileObject)
	{
		if (!projectilesGameObjectList.Contains (newProjectileObject)) {
			projectilesGameObjectList.Add (newProjectileObject);
		}

		currentRemoveProjectileWaitTime = timeToRemoveProjectiles;

		activateProjectileRemove ();
	}

	void activateProjectileRemove ()
	{
		stopRemoveProjectilesCoroutine ();

		removeCoroutine = StartCoroutine (removeProjectilesCoroutine ());
	}

	public void stopRemoveProjectilesCoroutine ()
	{
		if (removeCoroutine != null) {
			StopCoroutine (removeCoroutine);
		}
	}

	IEnumerator removeProjectilesCoroutine ()
	{
		yield return new WaitForSeconds (currentRemoveProjectileWaitTime);

		int projectileIndex = projectilesGameObjectList.Count - 1;

		if (projectilesGameObjectList [projectileIndex] != null) {
			Destroy (projectilesGameObjectList [projectileIndex]);
		}

		projectilesGameObjectList.RemoveAt (projectileIndex);

		if (projectilesGameObjectList.Count > 0) {

			currentRemoveProjectileWaitTime = waitTimeBetweenProjectileRemoving;

			activateProjectileRemove ();
		}
	}
}
