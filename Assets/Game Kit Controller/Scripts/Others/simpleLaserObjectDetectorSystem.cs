using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class simpleLaserObjectDetectorSystem : laser
{
	[Space]
	[Header ("Main Settings")]
	[Space]

	public LayerMask obstacleLayermask;

	public Transform laserTransform;

	RaycastHit hit;
	Ray ray;
	int nPoints;

	void Start ()
	{
		StartCoroutine (laserAnimation ());
	}

	void Update ()
	{
		animateLaser ();

		if (Physics.Raycast (laserTransform.position, laserTransform.forward, out hit, Mathf.Infinity, obstacleLayermask)) {
			//set the direction of the laser in the hit point direction
			transform.LookAt (hit.point);
		
			//get the hit position to set the particles of smoke and sparks
			laserDistance = hit.distance;
		
			lRenderer.positionCount = 2;
			lRenderer.SetPosition (0, transform.position);
			lRenderer.SetPosition (1, hit.point);
			//set the laser size 
		} else {

			laserDistance = 1000;

			lRenderer.positionCount = 2;
			lRenderer.SetPosition (0, transform.position);
			lRenderer.SetPosition (1, (laserDistance * transform.forward));
		}
	}
}