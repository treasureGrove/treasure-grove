using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class WaypointCircuit : MonoBehaviour
{
	public List<Transform> waypointList = new List<Transform> ();
	public bool showGizmo;
	public Color gizmoLabelColor;
	public bool useHandleForVertex;
	public Color handleGizmoColor;
	public float gizmoRadius;
	public float handleRadius;

	public bool smoothRoute = true;
	int numPoints;
	Vector3[] points;
	float[] distances;

	[Range (1, 100)] public float editorVisualisationSubsteps = 100;

	public float Length { get; set; }

	//this being here will save GC allocs
	int p0n;
	int p1n;
	int p2n;
	int p3n;

	float i;
	Vector3 P0;
	Vector3 P1;
	Vector3 P2;
	Vector3 P3;

	// Use this for initialization
	void Awake ()
	{
		if (waypointList.Count > 1) {
			CachePositionsAndDistances ();
		}
	}

	public Vector3 getClosestPosition (Vector3 position)
	{
		float minDistance = Mathf.Infinity;

		int index = -1;

		for (int i = 0; i < waypointList.Count; ++i) {
			float distance = GKC_Utils.distance (position, waypointList [i].position);

			if (distance < minDistance) {
				minDistance = distance;

				index = i;
			}
		}

		if (index > -1) {
			return waypointList [index].position;
		} else {
			return Vector3.zero;
		}
	}

	public RoutePoint GetRoutePoint (float dist)
	{
		// position and direction
		Vector3 p1 = GetRoutePosition (dist);
		Vector3 p2 = GetRoutePosition (dist + 0.1f);
		Vector3 delta = p2 - p1;
		return new RoutePoint (p1, delta.normalized);
	}

	public Vector3 GetRoutePosition (float dist)
	{
		numPoints = waypointList.Count;

		int point = 0;

		if (Length == 0) {
			Length = distances [distances.Length - 1];
		}

		dist = Mathf.Repeat (dist, Length);

		while (distances [point] < dist) {
			++point;
		}


		// get nearest two points, ensuring points wrap-around start & end of circuit
		p1n = ((point - 1) + numPoints) % numPoints;
		p2n = point;

		// found point numbers, now find interpolation value between the two middle points

		i = Mathf.InverseLerp (distances [p1n], distances [p2n], dist);

		if (smoothRoute) {
			// smooth catmull-rom calculation between the two relevant points

			// get indices for the surrounding 2 points, because
			// four points are required by the catmull-rom function
			p0n = ((point - 2) + numPoints) % numPoints;
			p3n = (point + 1) % numPoints;

			// 2nd point may have been the 'last' point - a dupe of the first,
			// (to give a value of max track distance instead of zero)
			// but now it must be wrapped back to zero if that was the case.
			p2n = p2n % numPoints;

			P0 = points [p0n];
			P1 = points [p1n];
			P2 = points [p2n];
			P3 = points [p3n];

			return CatmullRom (P0, P1, P2, P3, i);
		} else {
			// simple linear lerp between the two points:

			p1n = ((point - 1) + numPoints) % numPoints;
			p2n = point;

			return Vector3.Lerp (points [p1n], points [p2n], i);
		}
	}

	Vector3 CatmullRom (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
	{
		// comments are no use here... it's the catmull-rom equation.
		// Un-magic this, lord vector!
		return 0.5f * (2 * p1 + (-p0 + p2) * i + (2 * p0 - 5 * p1 + 4 * p2 - p3) * (i * i) + (-p0 + 3 * p1 - 3 * p2 + p3) * (i * i * i));
	}

	void CachePositionsAndDistances ()
	{
		// transfer the position of each point and distances between points to arrays for
		// speed of lookup at runtime
		points = new Vector3[waypointList.Count + 1];
		distances = new float[waypointList.Count + 1];

		float accumulateDistance = 0;
		for (int i = 0; i < points.Length; ++i) {
			Transform t1 = waypointList [(i) % waypointList.Count];
			Transform t2 = waypointList [(i + 1) % waypointList.Count];
			if (t1 != null && t2 != null) {
				Vector3 p1 = t1.position;
				Vector3 p2 = t2.position;
				points [i] = waypointList [i % waypointList.Count].position;
				distances [i] = accumulateDistance;
				accumulateDistance += (p1 - p2).magnitude;
			}
		}
	}

	public void addNewWayPoint ()
	{
		Vector3 newPosition = transform.position;
		if (waypointList.Count > 0) {
			newPosition = waypointList [waypointList.Count - 1].position + waypointList [waypointList.Count - 1].forward;
		}
		GameObject newWayPoint = new GameObject ();
		newWayPoint.transform.SetParent (transform);
		newWayPoint.transform.position = newPosition;
		newWayPoint.name = (waypointList.Count + 1).ToString ();
		waypointList.Add (newWayPoint.transform);
		//Undo.RegisterCreatedObjectUndo (newWayPoint, "add new waypoint circuit");
		updateComponent ();
	}

	public void addNewWayPointAtIndex (int index)
	{
		addNewWayPoint ();

		GameObject currentWaypoint = waypointList [waypointList.Count - 1].gameObject;

		currentWaypoint.transform.position = waypointList [index].position + waypointList [index].right * 3;
		waypointList.Insert ((index + 1), currentWaypoint.transform);
		waypointList.RemoveAt (waypointList.Count - 1);
		renameWaypoints ();
	}

	public void renameWaypoints ()
	{
		for (int i = 0; i < waypointList.Count; ++i) {
			waypointList [i].name = (i + 1).ToString ();
			waypointList [i].SetParent (null);
			waypointList [i].SetParent (transform);
		}

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
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
			if (waypointList.Count > 1) {
				numPoints = waypointList.Count;

				CachePositionsAndDistances ();
				Length = distances [distances.Length - 1];

				Gizmos.color = gizmoLabelColor;
				Vector3 prev = waypointList [0].position;
				if (smoothRoute) {
					if (editorVisualisationSubsteps <= 0) {
						editorVisualisationSubsteps = 1;
					}
					for (float dist = 0; dist < Length; dist += Length / editorVisualisationSubsteps) {
						Vector3 next = GetRoutePosition (dist + 1);
						Gizmos.DrawLine (prev, next);
						prev = next;
					}
					Gizmos.DrawLine (prev, waypointList [0].position);
				} else {
					for (int n = 0; n < waypointList.Count; ++n) {
						Vector3 next = waypointList [(n + 1) % waypointList.Count].position;
						Gizmos.DrawLine (prev, next);
						prev = next;
					}
				}
			}
		}
	}

	public struct RoutePoint
	{
		public Vector3 position;
		public Vector3 direction;

		public RoutePoint (Vector3 position, Vector3 direction)
		{
			this.position = position;
			this.direction = direction;
		}
	}
}