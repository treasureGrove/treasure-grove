using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fieldOfViewMeshSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]
	public bool fieldOfViewActive;

	public Color originalColor = Color.white;
	public Color alertColor = Color.red;

	public float viewRadius;
	[Range (0, 360)] public float viewAngle;

	public LayerMask obstacleMask;

	public float meshResolution;
	public int edgeResolveIterations;
	public float edgeDstThreshold;

	public float maskCutawayDst = .1f;

	public bool useObstacleRaycast;

	[Space]
	[Header ("Components")]
	[Space]

	public MeshFilter viewMeshFilter;
	public MeshRenderer mainMeshRenderer;

	Mesh viewMesh;
	int stepCount;
	float stepAngleSize;
	List<Vector3> viewPoints = new List<Vector3> ();
	viewCastInfo oldViewCast = new viewCastInfo ();
	edgeInfo edge;

	int vertexCount;
	Vector3[] vertices;
	int[] triangles;


	viewCastInfo newViewCast1;
	viewCastInfo newViewCast2;
	Vector3 minPoint;
	Vector3 maxPoint;
	RaycastHit hit;
	Vector3 viewCastDirection;

	void Start ()
	{
		if (!fieldOfViewActive) {
			enabled = false;

			if (gameObject.activeSelf) {
				gameObject.SetActive (false);
			}

			return;
		}

		viewMesh = new Mesh ();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;

		setMaterialColor (originalColor);
	}

	void LateUpdate ()
	{
		if (fieldOfViewActive) {
	
			stepCount = Mathf.RoundToInt (viewAngle * meshResolution);
			stepAngleSize = viewAngle / stepCount;

			viewPoints.Clear ();

			oldViewCast = new viewCastInfo ();

			for (int i = 0; i <= stepCount; i++) {
				float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
				newViewCast1 = ViewCast (angle);

				if (i > 0) {
					bool edgeDstThresholdExceeded = Mathf.Abs (oldViewCast.dst - newViewCast1.dst) > edgeDstThreshold;

					if (oldViewCast.hit != newViewCast1.hit || (oldViewCast.hit && newViewCast1.hit && edgeDstThresholdExceeded)) {
						edge = FindEdge (oldViewCast, newViewCast1);

						if (edge.pointA != Vector3.zero) {
							viewPoints.Add (edge.pointA);
						}
						if (edge.pointB != Vector3.zero) {
							viewPoints.Add (edge.pointB);
						}
					}

				}
				
				viewPoints.Add (newViewCast1.point);
				oldViewCast = newViewCast1;
			}

			vertexCount = viewPoints.Count + 1;
			vertices = new Vector3[vertexCount];
			triangles = new int[(vertexCount - 2) * 3];

			vertices [0] = Vector3.zero;

			for (int i = 0; i < vertexCount - 1; i++) {
				vertices [i + 1] = transform.InverseTransformPoint (viewPoints [i]) + Vector3.forward * maskCutawayDst;

				if (i < vertexCount - 2) {
					triangles [i * 3] = 0;
					triangles [i * 3 + 1] = i + 1;
					triangles [i * 3 + 2] = i + 2;
				}
			}

			viewMesh.Clear ();

			viewMesh.vertices = vertices;

			viewMesh.triangles = triangles;

			viewMesh.RecalculateNormals ();
		}
	}

	edgeInfo FindEdge (viewCastInfo minViewCast, viewCastInfo maxViewCast)
	{
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		minPoint = Vector3.zero;
		maxPoint = Vector3.zero;

		for (int i = 0; i < edgeResolveIterations; i++) {
			float angle = (minAngle + maxAngle) / 2;
			newViewCast2 = ViewCast (angle);

			bool edgeDstThresholdExceeded = Mathf.Abs (minViewCast.dst - newViewCast2.dst) > edgeDstThreshold;

			if (newViewCast2.hit == minViewCast.hit && !edgeDstThresholdExceeded) {
				minAngle = angle;
				minPoint = newViewCast2.point;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast2.point;
			}
		}

		return new edgeInfo (minPoint, maxPoint);
	}

	viewCastInfo ViewCast (float globalAngle)
	{
		viewCastDirection = new Vector3 (Mathf.Sin (globalAngle * Mathf.Deg2Rad), 0, Mathf.Cos (globalAngle * Mathf.Deg2Rad));

		if (Physics.Raycast (transform.position, viewCastDirection, out hit, viewRadius, obstacleMask) && useObstacleRaycast) {
			return new viewCastInfo (true, hit.point, hit.distance, globalAngle);
		} else {
			return new viewCastInfo (false, transform.position + viewCastDirection * viewRadius, viewRadius, globalAngle);
		}
	}

	public void setActiveState (bool state)
	{
		if (fieldOfViewActive) {
			if (gameObject.activeSelf != state) {
				gameObject.SetActive (state);
			}
		}
	}

	public struct viewCastInfo
	{
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public viewCastInfo (bool hitValue, Vector3 pointValue, float dstValue, float angleValue)
		{
			hit = hitValue;
			point = pointValue;
			dst = dstValue;
			angle = angleValue;
		}
	}

	public struct edgeInfo
	{
		public Vector3 pointA;
		public Vector3 pointB;

		public edgeInfo (Vector3 newPointA, Vector3 newPointB)
		{
			pointA = newPointA;
			pointB = newPointB;
		}
	}

	public void setMaterialColor (Color newColor)
	{
		mainMeshRenderer.material.color = newColor;
	}

	public void setRegularColor ()
	{
		setMaterialColor (originalColor);
	}

	public void setAlertColor ()
	{
		setMaterialColor (alertColor);
	}
}