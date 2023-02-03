using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headExplodeExample : MonoBehaviour
{
	public bool useTimeBullet = true;
	public float timeBulletDuration = 3;
	public float timeScale = 0.2f;

	public Transform headTransform;

	public void explode ()
	{
//		print ("function to explode head here");

		if (useTimeBullet) {
			GKC_Utils.activateTimeBulletXSeconds (timeBulletDuration, timeScale);
		}

		if (headTransform != null) {
			headTransform.localScale = Vector3.zero;
		}
	}

	public void explodeBodyPart (Transform objectToExplode)
	{
		if (objectToExplode != null) {
			objectToExplode.localScale = Vector3.zero;
		}
	}
}
