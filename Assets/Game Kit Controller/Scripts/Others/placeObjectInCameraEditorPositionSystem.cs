using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placeObjectInCameraEditorPositionSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public Vector3 positionOffset;

	public float rotationAmount = 90;

	public LayerMask layerToPlaceElements;

	[Space]
	[Header ("Snap To Ground Settings")]
	[Space]

	public bool useCustomObjectToSnap;
	public Transform customObjectToSnap;

	[Space]
	[Header ("Object Settings")]
	[Space]

	public GameObject objectToSelect;

	public List<Transform> objectsToMoveList = new List<Transform> ();

	RaycastHit hit;

	public void moveObjects ()
	{
		Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

		if (currentCameraEditor != null) {
			Vector3 editorCameraPosition = currentCameraEditor.transform.position;
			Vector3 editorCameraForward = currentCameraEditor.transform.forward;

			adjustToSurface (editorCameraPosition, editorCameraForward);
		}
	}

	public void snapToGrounSurface ()
	{
		Vector3 positionToSnap = Vector3.zero;
		Vector3 directionToSnap = Vector3.zero;

		if (useCustomObjectToSnap) {
			positionToSnap = customObjectToSnap.position + customObjectToSnap.up * 2;
			directionToSnap = -customObjectToSnap.up;
		} else {
			positionToSnap = transform.position + transform.up * 2;
			directionToSnap = -transform.up;
		}

		adjustToSurface (positionToSnap, directionToSnap);
	}

	void adjustToSurface (Vector3 raycastPosition, Vector3 raycastDirection)
	{
		RaycastHit hit;

		if (Physics.Raycast (raycastPosition, raycastDirection, out hit, Mathf.Infinity, layerToPlaceElements)) {

			if (objectsToMoveList.Contains (hit.collider.transform)) {
				RaycastHit newRaycastHit;

				Vector3 lastHitPosition = hit.point + raycastDirection * 0.08f;

				if (Physics.Raycast (lastHitPosition, raycastDirection, out newRaycastHit, Mathf.Infinity, layerToPlaceElements)) {
					hit = newRaycastHit;
				}
			} else {
				bool regularSurfaceDetected = false;

				int counter = 0;

				Transform objectDetected = hit.collider.transform;

				Vector3 lastHitPosition = hit.point + raycastDirection * 0.08f;

				while (!regularSurfaceDetected) {

					bool currentDetectedObjectNotChildOnList = true;

					for (int i = 0; i < objectsToMoveList.Count; i++) { 
						if (objectDetected.IsChildOf (objectsToMoveList [i])) {

							RaycastHit newRaycastHit;

							if (Physics.Raycast (lastHitPosition, raycastDirection, out newRaycastHit, Mathf.Infinity, layerToPlaceElements)) {
								lastHitPosition = newRaycastHit.point + raycastDirection * 0.08f;

								hit = newRaycastHit;

								objectDetected = hit.collider.transform;
							}

							currentDetectedObjectNotChildOnList = false;
						}
					}

					if (currentDetectedObjectNotChildOnList) {
						regularSurfaceDetected = true;
					}

					counter++;

					if (counter > 200) {
						regularSurfaceDetected = true;

						print (counter);
					}
				}
			}

			Vector3 positionToMove = hit.point + Vector3.right * positionOffset.x + Vector3.up * positionOffset.y + Vector3.forward * positionOffset.z;

			for (int i = 0; i < objectsToMoveList.Count; i++) { 
				objectsToMoveList [i].position = positionToMove;
			}
		}

		updateDirtyScene ();
	}

	public void rotateObject (int direction)
	{
		for (int i = 0; i < objectsToMoveList.Count; i++) { 
			objectsToMoveList [i].Rotate (0, direction * rotationAmount, 0);
		}

		updateDirtyScene ();
	}

	public void resetObjectRotation ()
	{
		for (int i = 0; i < objectsToMoveList.Count; i++) { 
			objectsToMoveList [i].rotation = Quaternion.identity;
		}

		updateDirtyScene ();
	}

	public void addObjectToList (GameObject newObject)
	{
		if (newObject == null) {
			return;
		}

		if (!objectsToMoveList.Contains (newObject.transform)) {
			objectsToMoveList.Add (newObject.transform);
		}

		updateDirtyScene ();
	}

	public void setDefaultLayermask ()
	{
		layerToPlaceElements |= (1 << LayerMask.NameToLayer ("Default"));
		
		layerToPlaceElements |= (1 << LayerMask.NameToLayer ("Terrain"));

		updateDirtyScene ();
	}

	public void selectObject ()
	{
		if (objectToSelect != null) {
			GKC_Utils.setActiveGameObjectInEditor (objectToSelect);
		}
	}

	public void setOffsetFromRendererBound ()
	{
		if (positionOffset.y != 0) {
			return;
		}

		float totalOffset = 0;

		for (int i = 0; i < objectsToMoveList.Count; i++) {
			if (objectsToMoveList [i] != null) {
				Renderer mainRenderer = objectsToMoveList [i].GetComponent<Renderer> ();

				if (mainRenderer == null) {
					mainRenderer = objectsToMoveList [i].GetComponentInChildren<Renderer> ();
				}

				if (mainRenderer != null) {
					totalOffset = mainRenderer.bounds.extents.y;
				}
			}
		}

		if (objectToSelect != null) {
			Renderer mainRenderer = objectToSelect.GetComponent<Renderer> ();

			if (mainRenderer == null) {
				mainRenderer = objectToSelect.GetComponentInChildren<Renderer> ();
			}

			if (mainRenderer != null) {
				totalOffset = mainRenderer.bounds.extents.y;

				updateDirtyScene ();
			}
		}

		if (totalOffset != 0) {
			positionOffset.y = totalOffset;

			updateDirtyScene ();
		}
	}

	void updateDirtyScene ()
	{
		GKC_Utils.updateDirtyScene ("Move Object on the scene", gameObject);
	}
}
