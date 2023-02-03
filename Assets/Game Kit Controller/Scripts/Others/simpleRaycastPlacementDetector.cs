using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleRaycastPlacementDetector : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask raycastLayermask;

	public bool placementEnabled = true;

	public float raycastDistance = 2;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform raycastDirection;

	public Transform mainTransform;

	RaycastHit hit;


	public void activePlacement ()
	{
		if (!placementEnabled) {
			return;
		}

		if (Physics.Raycast (raycastDirection.position, raycastDirection.forward, out hit, raycastDistance, raycastLayermask)) {
			mainTransform.rotation = Quaternion.LookRotation (hit.normal);

			mainTransform.position = hit.point + hit.normal * 0.1f;
		} 
	}
}
