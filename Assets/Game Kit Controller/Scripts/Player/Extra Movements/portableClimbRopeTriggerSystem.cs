using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portableClimbRopeTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useDownRaycastDirection;

	public LayerMask raycastLayermask;

	public float maxRaycastDistance;

	public bool adjustRopeEndToAnyDistance;

	public bool activateOnStart;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool checkSurfaceLayer;
	public LayerMask layerSurface;
	public float raycastRadius;
	public bool destroyObjectIfSurfaceNotFound;

	public bool checkSurfaceInclination;
	public float maxSurfaceInclination;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Components")]
	[Space]

	public climbRopeTriggerSystem mainClimbRopeTriggerSystem;

	public Transform ropePivotTransform;

	public Transform customRaycastDirection;

	RaycastHit hit;

	void Start ()
	{
		if (activateOnStart) {
			activateClimbRopeTrigger ();
		}
	}

	public void activateClimbRopeTrigger ()
	{
		if (checkSurfaceLayer) {
			Collider[] hits = Physics.OverlapSphere (transform.position, raycastRadius);

			if (hits.Length <= 0) {
				if (destroyObjectIfSurfaceNotFound) {
					if (showDebugPrint) {
						print ("No surface found on portacle climb rope trigger system, destroy rope");
					}

					Destroy (gameObject);
				}
			} 

			if (checkSurfaceInclination) {
				Vector3 rayPosition = transform.position;

				for (int i = 0; i < hits.Length; i++) {
					Vector3 heading = hits [i].transform.position - rayPosition;

					float distance = heading.magnitude;

					Vector3 rayDirection = heading / distance;

					if (Physics.Raycast (rayPosition, rayDirection, out hit, distance + 1, raycastLayermask)) {
								
						float hitAngle = Vector3.Angle (Vector3.up, hit.normal);  

						if (showDebugPrint) {
							print ("Angle with surface found of " + hitAngle + " degrees");
						}
					
						if (Mathf.Abs (hitAngle) > maxSurfaceInclination) {

							if (showDebugPrint) {
								print ("Surface angle not possible for portacle climb rope trigger system, destroy rope");
							}

							Destroy (gameObject);
						}
					}
				}
			}
		}
			
		Vector3 raycastPosition = transform.position;

		Vector3 raycastDirection = Vector3.down;

		if (!useDownRaycastDirection) {
			raycastDirection = customRaycastDirection.forward;
		}

		float raycastDistance = maxRaycastDistance;

		if (adjustRopeEndToAnyDistance) {
			raycastDistance = Mathf.Infinity;
		}

		Vector3 targetPosition = transform.position;

		bool surfaceDetected = false;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, raycastDistance, raycastLayermask)) {
			targetPosition = hit.point;

			surfaceDetected = true;
		} else {
			targetPosition += Vector3.down * maxRaycastDistance;
		}
			
		Transform bottomTransform = mainClimbRopeTriggerSystem.bottomTransform;
		Transform topTransform = mainClimbRopeTriggerSystem.topTransform;

		topTransform.position = transform.position;

		bottomTransform.position = targetPosition;


		bool scaleDown = true;

		if (surfaceDetected) {
			if (hit.distance < 3) {
				raycastDirection = Vector3.up;

				targetPosition = transform.position;

				if (Physics.Raycast (raycastPosition, raycastDirection, out hit, raycastDistance, raycastLayermask)) {
					targetPosition = hit.point;
				} else {
					targetPosition += raycastDirection * maxRaycastDistance;
				}

				topTransform.position = targetPosition;

				scaleDown = false;
			}
		}


		float scaleZ = GKC_Utils.distance (topTransform.position, bottomTransform.position);

		float finalScaleValue = scaleZ + (scaleZ * 0.01f) + 0.5f;

		float scaleMultiplier = 1;

		if (!scaleDown) {
			scaleMultiplier = -1;
		}

		ropePivotTransform.localScale = new Vector3 (1, 1, finalScaleValue * scaleMultiplier);

		Vector3 direction = bottomTransform.position - topTransform.position;

		direction = direction / direction.magnitude;

		ropePivotTransform.rotation = Quaternion.LookRotation (direction);

		bottomTransform.position += Vector3.up * 0.5f;
		topTransform.position -= Vector3.up * 2;
	}
}
