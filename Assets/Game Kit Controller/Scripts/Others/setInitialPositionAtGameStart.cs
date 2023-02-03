using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setInitialPositionAtGameStart : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool setInitialPositionEnabled;

	public Vector3 positionOffset;

	public Vector3 rotationOffset;

	public bool useCustomTransform;

	public Transform customTransform;

	public bool adjustObjectToSurface;
	public LayerMask layerToPlaceElements;

	[Space]
	[Header ("Object Settings")]
	[Space]

	public List<Transform> objectsToMoveList = new List<Transform> ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.green;
	public float gizmoArrowLength = 1;
	public float gizmoArrowAngle = 20;
	public Color gizmoArrowColor = Color.white;
	public float arrowLength = 3;

	RaycastHit hit;

	void Start ()
	{
		if (setInitialPositionEnabled) {
			setInitialPosition ();
		}
	}

	public void setInitialPosition ()
	{
		Vector3 targetPoisition = positionOffset;
		Vector3 targetRotation = rotationOffset;

		if (useCustomTransform) {
			targetPoisition = customTransform.position;
			targetRotation = customTransform.eulerAngles;
		}

		if (adjustObjectToSurface) {
			RaycastHit hit;

			if (Physics.Raycast (targetPoisition, -Vector3.up, out hit, Mathf.Infinity, layerToPlaceElements)) {

				targetPoisition = hit.point + Vector3.up * 0.1f;
			}
		}

		for (int i = 0; i < objectsToMoveList.Count; i++) { 
			objectsToMoveList [i].position = targetPoisition;
			objectsToMoveList [i].eulerAngles = targetRotation;
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
			Vector3 targetPoisition = positionOffset;
			Vector3 targetRotation = rotationOffset;

			if (useCustomTransform) {
				targetPoisition = customTransform.position;
				targetRotation = customTransform.eulerAngles;
			}

			GKC_Utils.drawGizmoArrow (targetPoisition + Vector3.up * arrowLength, -Vector3.up * arrowLength, gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);

			Vector3 targetDirection = Vector3.forward; 

			if (targetRotation != Vector3.zero) {
				targetDirection = Quaternion.Euler (targetRotation) * Vector3.forward; 
			}

			GKC_Utils.drawGizmoArrow (positionOffset, targetDirection * arrowLength, gizmoArrowColor, gizmoArrowLength, gizmoArrowAngle);

			Gizmos.color = gizmoColor;

			Gizmos.DrawSphere (targetPoisition, 0.2f);
		}
	}
}
