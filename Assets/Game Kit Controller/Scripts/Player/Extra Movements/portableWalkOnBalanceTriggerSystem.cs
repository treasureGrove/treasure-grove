using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portableWalkOnBalanceTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask raycastLayermask;

	public float maxRaycastDistance;

	public bool adjustEndToAnyDistance;

	public bool activateOnStart;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool checkSurfaceLayer;
	public LayerMask layerSurface;
	public float raycastRadius;
	public bool destroyObjectIfSurfaceNotFound;

	public bool checkSurfaceInclination;
	public float minSurfaceInclination;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform pivotTransform;

	public Transform startTransform;
	public Transform endTransform;

	RaycastHit hit;

	void Start ()
	{
		if (activateOnStart) {
			activateWalkOnBalanceTrigger ();
		}
	}

	public void activateWalkOnBalanceTrigger ()
	{
		if (checkSurfaceLayer) {
			Collider[] hits = Physics.OverlapSphere (transform.position, raycastRadius);

			if (hits.Length <= 0) {
				if (destroyObjectIfSurfaceNotFound) {
					if (showDebugPrint) {
						print ("No surface found on portacle walk on balance trigger system, destroy object");
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

						if (Mathf.Abs (hitAngle) < minSurfaceInclination) {

							if (showDebugPrint) {
								print ("Surface angle not possible for portacle walk on balance trigger system, destroy object");
							}

							Destroy (gameObject);
						}
					}
				}
			}
		}
		Vector3 raycastPosition = transform.position;

		Vector3 raycastDirection = transform.forward;

		float raycastDistance = maxRaycastDistance;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, raycastDistance, raycastLayermask)) {
			transform.rotation = Quaternion.LookRotation (hit.normal);

			transform.position = hit.point + hit.normal * 0.1f;

			transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
		} else {
			Collider[] hits = Physics.OverlapSphere (transform.position, raycastRadius);

			bool surfaceLocated = false;

			Vector3 rayPosition = transform.position;

			for (int i = 0; i < hits.Length; i++) {
				if (!surfaceLocated) {
					Vector3 heading = hits [i].transform.position - rayPosition;

					float distance = heading.magnitude;

					Vector3 rayDirection = heading / distance;

					if (Physics.Raycast (rayPosition, rayDirection, out hit, distance + 1, raycastLayermask)) {
						transform.rotation = Quaternion.LookRotation (hit.normal);

						transform.position = hit.point + hit.normal * 0.1f;

						surfaceLocated = true;
					}
				}
			}
		}

		raycastDirection = transform.forward;

		raycastPosition = transform.position;

		if (adjustEndToAnyDistance) {
			raycastDistance = Mathf.Infinity;
		}

		Vector3 targetPosition = transform.position;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, raycastDistance, raycastLayermask)) {
			targetPosition = hit.point;
		} else {
			targetPosition += transform.forward * maxRaycastDistance;
		}
	
		startTransform.position = transform.position;

		endTransform.position = targetPosition;

		float scaleZ = GKC_Utils.distance (startTransform.position, endTransform.position);

		float finalScaleValue = scaleZ + (scaleZ * 0.01f) + 0.5f;

		pivotTransform.localScale = new Vector3 (1, 1, finalScaleValue);
	}
}