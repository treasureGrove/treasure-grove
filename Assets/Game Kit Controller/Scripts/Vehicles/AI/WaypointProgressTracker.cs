using System;
using UnityEngine;


public class WaypointProgressTracker : MonoBehaviour
{
	// This script can be used with any object that is supposed to follow a
	// route marked out by waypoints.

	// This script manages the amount to look ahead along the route,
	// and keeps track of progress and laps.

	[SerializeField]  WaypointCircuit circuit;
	// A reference to the waypoint-based route we should follow

	[SerializeField]  float lookAheadForTargetOffset = 5;
	// The offset ahead along the route that the we will aim for

	[SerializeField]  float lookAheadForTargetFactor = .1f;
	// A multiplier adding distance ahead along the route to aim for, based on current speed

	[SerializeField]  float lookAheadForSpeedOffset = 10;
	// The offset ahead only the route for speed adjustments (applied as the rotation of the waypoint target transform)

	[SerializeField]  float lookAheadForSpeedFactor = .2f;
	// A multiplier adding distance ahead along the route for speed adjustments

	[SerializeField]  ProgressStyle progressStyle = ProgressStyle.SmoothAlongRoute;
	// whether to update the position smoothly along the route (good for curved paths) or just when we reach each waypoint.

	[SerializeField]  float pointToPointThreshold = 4;
	// proximity to waypoint which must be reached to switch target to next waypoint : only used in PointToPoint mode.

	public enum ProgressStyle
	{
		SmoothAlongRoute,
		PointToPoint,
	}

	// these are public, readable by other objects - i.e. for an AI to know where to head!
	public WaypointCircuit.RoutePoint targetPoint { get; set; }

	public WaypointCircuit.RoutePoint speedPoint { get; set; }

	public WaypointCircuit.RoutePoint progressPoint { get; set; }

	public Transform target;

	public float progressDistance;
	// The progress round the route, used in smooth mode.
	public int progressNum;
	// the current waypoint number, used in point-to-point mode.
	Vector3 lastPosition;
	// Used to calculate current speed (since we may not have a rigidbody component)
	float speed;
	// current speed of this object (calculated from delta since last frame)

	vehicleAINavMesh vehicleAI;

	// setup script properties
	void Start ()
	{
		// we use a transform to represent the point to aim for, and the point which
		// is considered for upcoming changes-of-speed. This allows this component
		// to communicate this information to the AI without requiring further dependencies.

		// You can manually create a transform and assign it to this component *and* the AI,
		// then this component will update it, and the AI can read it.
		target = new GameObject (name + " Waypoint Target").transform;

		Reset ();

		vehicleAI = GetComponent<vehicleAINavMesh> ();
		if (vehicleAI) {
			vehicleAI.SetTarget (target);
		}
	}


	// reset the object to sensible values
	public void Reset ()
	{
		if (progressStyle == ProgressStyle.PointToPoint) {
			target.position = circuit.waypointList [progressNum].transform.position;
			target.rotation = circuit.waypointList [progressNum].transform.rotation;
		} else {
			target.position = transform.position;
			target.rotation = transform.rotation;

//			Vector3 currentPosition = circuit.getClosestPosition (transform.position);
//			progressDistance = currentPosition.magnitude * 0.5f;
		}
	}


	void Update ()
	{
		if (progressStyle == ProgressStyle.SmoothAlongRoute) {
			// determine the position we should currently be aiming for
			// (this is different to the current progress position, it is a a certain amount ahead along the route)
			// we use lerp as a simple way of smoothing out the speed over time.
			if (Time.deltaTime > 0) {
				speed = Mathf.Lerp (speed, (lastPosition - transform.position).magnitude / Time.deltaTime, Time.deltaTime);
			}
			target.position = circuit.GetRoutePoint (progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor * speed).position;
			target.rotation = Quaternion.LookRotation (circuit.GetRoutePoint (progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor * speed).direction);

			// get our current progress along the route
			progressPoint = circuit.GetRoutePoint (progressDistance);
			Vector3 progressDelta = progressPoint.position - transform.position;
			if (Vector3.Dot (progressDelta, progressPoint.direction) < 0) {
				progressDistance += progressDelta.magnitude * 0.5f;
			}

			lastPosition = transform.position;
		} else {
			// point to point mode. Just increase the waypoint if we're close enough:

			Vector3 targetDelta = target.position - transform.position;
			if (targetDelta.magnitude < pointToPointThreshold) {
				progressNum = (progressNum + 1) % circuit.waypointList.Count;
			}
	
			target.position = circuit.waypointList [progressNum].transform.position;
			target.rotation = circuit.waypointList [progressNum].transform.rotation;

			// get our current progress along the route
			progressPoint = circuit.GetRoutePoint (progressDistance);
			Vector3 progressDelta = progressPoint.position - transform.position;
			if (Vector3.Dot (progressDelta, progressPoint.direction) < 0) {
				progressDistance += progressDelta.magnitude;
			}
			lastPosition = transform.position;
		}
	}


	void OnDrawGizmos ()
	{
		if (Application.isPlaying) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine (transform.position, target.position);
			Gizmos.DrawWireSphere (circuit.GetRoutePosition (progressDistance), 1);
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine (target.position, target.position + target.forward);
		}
	}
}

