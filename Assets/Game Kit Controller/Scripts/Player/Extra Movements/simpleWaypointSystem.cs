using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleWaypointSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public List<Transform> wayPoints = new List<Transform> ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoLabelColor = Color.black;
	public float gizmoRadius;

	public List<Transform> getWayPoints ()
	{
		return wayPoints;
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
			for (int i = 0; i < wayPoints.Count; i++) {
				if (wayPoints [i] != null) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (wayPoints [i].position, gizmoRadius);

					if (i + 1 < wayPoints.Count && wayPoints [i + 1] != null) {
						Gizmos.color = Color.white;
						Gizmos.DrawLine (wayPoints [i].position, wayPoints [i + 1].position);
					}

				}
			}
		}
	}
}
