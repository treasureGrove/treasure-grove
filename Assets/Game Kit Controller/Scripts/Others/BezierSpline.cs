using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class BezierSpline : MonoBehaviour
{
	public List<splinePointInfo> splinePointList = new List<splinePointInfo> ();

	public List<Transform> lookDirectionList = new List<Transform> ();

	public int selectedIndex = -1;

	public float directionScale = 0.5f;

	public float newPointOffset = 0.2f;
	public float newPointBezierOffset = 0.2f;

	public float handleSize = 0.04f;
	public float pickSize = 0.06f;

	public bool showGizmo;
	public bool showLookDirectionGizmo;
	public float lookDirectionGizmoRadius = 0.05f;
	public float lookDirectionArrowLength = 1;

	public enum BezierControlPointMode
	{
		Aligned,
		Free,
		Mirrored
	}

	[SerializeField]
	public List<Vector3> points = new List<Vector3> ();

	[SerializeField]
	public List<BezierControlPointMode> modes = new List<BezierControlPointMode> ();

	[SerializeField]
	private bool loop;

	public bool Loop {
		get {
			return loop;
		}
		set {
			loop = value;
			if (value == true) {
				modes [modes.Count - 1] = modes [0];
				SetControlPoint (0, points [0]);
			}
		}
	}

	public int ControlPointCount {
		get {
			return points.Count;
		}
	}

	public Vector3 GetControlPoint (int index)
	{
		return points [index];
	}

	public void SetControlPoint (int index, Vector3 point)
	{
		if (index % 3 == 0) {
			Vector3 delta = point - points [index];
			if (loop) {
				if (index == 0) {
					points [1] += delta;
					points [points.Count - 2] += delta;
					points [points.Count - 1] = point;
				} else if (index == points.Count - 1) {
					points [0] = point;
					points [1] += delta;
					points [index - 1] += delta;
				} else {
					points [index - 1] += delta;
					points [index + 1] += delta;
				}
			} else {
				if (index > 0) {
					points [index - 1] += delta;
				}
				if (index + 1 < points.Count) {
					points [index + 1] += delta;
				}
			}
		}
		points [index] = point;
		EnforceMode (index);
	}

	public BezierControlPointMode GetControlPointMode (int index)
	{
		return modes [(index + 1) / 3];
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode)
	{
		int modeIndex = (index + 1) / 3;
		modes [modeIndex] = mode;
		if (loop) {
			if (modeIndex == 0) {
				modes [modes.Count - 1] = mode;
			} else if (modeIndex == modes.Count - 1) {
				modes [0] = mode;
			}
		}
		EnforceMode (index);
	}

	private void EnforceMode (int index)
	{
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes [modeIndex];
		if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Count - 1)) {
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			if (fixedIndex < 0) {
				fixedIndex = points.Count - 2;
			}
			enforcedIndex = middleIndex + 1;
			if (enforcedIndex >= points.Count) {
				enforcedIndex = 1;
			}
		} else {
			fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Count) {
				fixedIndex = 1;
			}
			enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0) {
				enforcedIndex = points.Count - 2;
			}
		}

		Vector3 middle = points [middleIndex];
		Vector3 enforcedTangent = middle - points [fixedIndex];
		if (mode == BezierControlPointMode.Aligned) {
			enforcedTangent = enforcedTangent.normalized * GKC_Utils.distance (middle, points [enforcedIndex]);
		}
		points [enforcedIndex] = middle + enforcedTangent;
	}

	public int CurveCount {
		get {
			return (points.Count - 1) / 3;
		}
	}

	public Vector3 GetPoint (float t)
	{
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Count - 4;
		} else {
			t = Mathf.Clamp01 (t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}

		return transform.TransformPoint (GetPoint (points [i], points [i + 1], points [i + 2], points [i + 3], t));
	}

	public Vector3 GetLookDirection (float t)
	{
		t = Mathf.Clamp01 (t) * CurveCount;

		int i = (int)t;

		//print (i);

		if (i >= lookDirectionList.Count) {
			i = lookDirectionList.Count - 1;
		}

		return lookDirectionList [i].localEulerAngles;
	}

	public Transform GetLookTransform (float t)
	{
		t = Mathf.Clamp01 (t) * CurveCount;
		int i = (int)t;

		//print (i);

		if (i >= lookDirectionList.Count) {
			i = lookDirectionList.Count - 1;
		}

		return lookDirectionList [i];
	}

	public int getPointIndex (float t)
	{
		t = Mathf.Clamp01 (t) * CurveCount;
		int i = (int)t;

		return i;
	}

	public Vector3 GetVelocity (float t)
	{
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Count - 4;
		} else {
			t = Mathf.Clamp01 (t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint (GetFirstDerivative (points [i], points [i + 1], points [i + 2], points [i + 3], t)) - transform.position;
	}

	public Vector3 GetDirection (float t)
	{
		return GetVelocity (t).normalized;
	}

	public void setInitialSplinePoint (Vector3 position)
	{
		points [0] = transform.InverseTransformPoint (position);
	}

	public void setFinalSplinePoint (Vector3 position)
	{
		points [points.Count - 1] = transform.InverseTransformPoint (position);
	}

	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		t = Mathf.Clamp01 (t);
		float oneMinusT = 1f - t;
		return oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2;
	}

	public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
	}

	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01 (t);
		float OneMinusT = 1f - t;
		return OneMinusT * OneMinusT * OneMinusT * p0 + 3f * OneMinusT * OneMinusT * t * p1 + 3f * OneMinusT * t * t * p2 +	t * t * t * p3;
	}

	public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01 (t);
		float oneMinusT = 1f - t;
		return 3f * oneMinusT * oneMinusT * (p1 - p0) +	6f * oneMinusT * t * (p2 - p1) + 3f * t * t * (p3 - p2);
	}

	public float AccuracyToStepSize (float accuracy)
	{
		if (accuracy <= 0f) {
			return 0.2f;
		}

		return Mathf.Clamp (1f / accuracy, 0.001f, 0.2f);
	}

	public Vector3 FindNearestPointTo (Vector3 worldPos, float accuracy = 100f, int secondPassIterations = 7, float secondPassExtents = 0.025f)
	{
		float normalizedT;

		return FindNearestPointTo (worldPos, out normalizedT, accuracy, secondPassIterations, secondPassExtents);
	}

	public Vector3 FindNearestPointTo (Vector3 worldPos, out float normalizedT, float accuracy = 100f, int secondPassIterations = 7, float secondPassExtents = 0.025f)
	{
		Vector3 result = Vector3.zero;

		normalizedT = -1f;

		float step = AccuracyToStepSize (accuracy);

		float minDistance = Mathf.Infinity;

		for (float i = 0f; i < 1f; i += step) {
			Vector3 thisPoint = GetPoint (i);
			float thisDistance = (worldPos - thisPoint).sqrMagnitude;

			if (thisDistance < minDistance) {
				minDistance = thisDistance;
				result = thisPoint;
				normalizedT = i;
			}
		}

		if (secondPassIterations > 0) {
			float minT = normalizedT - secondPassExtents;
			float maxT = normalizedT + secondPassExtents;

			for (int i = 0; i < secondPassIterations; i++) {
				float leftT = (minT + normalizedT) * 0.5f;
				float rightT = (maxT + normalizedT) * 0.5f;

				Vector3 leftPoint = GetPoint (leftT);
				Vector3 rightPoint = GetPoint (rightT);

				float leftDistance = (worldPos - leftPoint).sqrMagnitude;
				float rightDistance = (worldPos - rightPoint).sqrMagnitude;

				if (leftDistance < minDistance && leftDistance < rightDistance) {
					minDistance = leftDistance;
					result = leftPoint;
					maxT = normalizedT;
					normalizedT = leftT;
				} else if (rightDistance < minDistance && rightDistance < leftDistance) {
					minDistance = rightDistance;
					result = rightPoint;
					minT = normalizedT;
					normalizedT = rightT;
				} else {
					minT = leftT;
					maxT = rightT;
				}
			}
		}

		return result;
	}
		

	//EDITOR FUNCTIONS
	public void setSelectedIndex (int index)
	{
		selectedIndex = index;

		updateComponent ();
	}

	public void AddCurve (int index, bool addingAtTheEnd)
	{
		Vector3 point = points [index - 1];

		float currentOffset = newPointOffset;

		point.z += currentOffset;
		points.Insert (index, point);

		point.z += currentOffset;
		points.Insert (index + 1, point);

		point.z += currentOffset;
		points.Insert (index + 2, point);


		modes.Add (modes [modes.Count - 1]);
		EnforceMode (index - 4);

		if (loop) {
			points [index - 1] = points [0];
			modes [modes.Count - 1] = modes [0];
			EnforceMode (0);
		}

		if (addingAtTheEnd) {
			selectedIndex = points.Count - 1;
		} else {
			selectedIndex = index + 2;
		}

		updateComponent ();
	}

	public void removeCurve (int index)
	{
		if (index % 3 == 0) {
			if (points.Count > 4) {
				if (index == points.Count - 1) {
					points.RemoveAt (index);
					points.RemoveAt (index - 1);
					points.RemoveAt (index - 2);

					if (index == 3) {
						selectedIndex = 0;
					} else {
						selectedIndex = index - 3;
					}
				} else {
					if (index == 0) {
						points.RemoveAt (index + 2);
						points.RemoveAt (index + 1);
						points.RemoveAt (index);

						selectedIndex = 0;
					} else {
						points.RemoveAt (index + 1);
						points.RemoveAt (index);
						points.RemoveAt (index - 1);

						selectedIndex = index;
					}
				}

				modes.RemoveAt (index / 3);

				if (points.Count == 4) {
					Loop = false;
				}
			} else {
				print ("At least two points must be configured for the spline");
			}
		} else {
			print ("This point can't be removed");
		}

		updateComponent ();
	}

	public void Reset ()
	{
		points.Clear ();

		float currentOffset = newPointOffset;

		points.Add (new Vector3 (0f, 0f, currentOffset));
		points.Add (new Vector3 (0f, 0f, currentOffset * 2));
		points.Add (new Vector3 (0f, 0f, currentOffset * 3));
		points.Add (new Vector3 (0f, 0f, currentOffset * 4));

		modes.Clear ();

		modes.Add (BezierControlPointMode.Aligned);
		modes.Add (BezierControlPointMode.Aligned);

		selectedIndex = 0;

		updateComponent ();
	}

	public void alignLookDirection (bool adjustPositionAndRotation)
	{
		int pointIndex = 0;

		for (int j = 0; j < lookDirectionList.Count; j++) {
			Vector3 newPosition = Vector3.zero;

			if (pointIndex <= points.Count - 1) {
				newPosition = points [pointIndex];
			} 

			lookDirectionList [j].localPosition = newPosition;

			if (adjustPositionAndRotation) {
				lookDirectionList [j].localRotation = Quaternion.identity;
			}

			pointIndex += 3;
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
			if (showLookDirectionGizmo) {

				for (int i = 0; i < lookDirectionList.Count; i++) {
					if (lookDirectionList [i] != null) {
						Gizmos.color = Color.yellow;
						Gizmos.DrawSphere (lookDirectionList [i].position, lookDirectionGizmoRadius);

						GKC_Utils.drawGizmoArrow (lookDirectionList [i].position, lookDirectionList [i].forward * lookDirectionArrowLength, Color.yellow, 0.2f, 20);
					}
				}
			}
		}
	}

	[System.Serializable]
	public class splinePointInfo
	{
		public string Name;
		public Vector3 point;
		public BezierControlPointMode mode;
	}
}