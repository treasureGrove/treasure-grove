using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(BezierSpline))]
public class BezierSplineEditor : Editor
{
	SerializedProperty directionScale;
	SerializedProperty newPointOffset;
	SerializedProperty newPointBezierOffset;
	SerializedProperty points;
	SerializedProperty modes;
	SerializedProperty lookDirectionList;
	SerializedProperty handleSize;
	SerializedProperty pickSize;
	SerializedProperty showGizmo;
	SerializedProperty showLookDirectionGizmo;
	SerializedProperty lookDirectionGizmoRadius;
	SerializedProperty lookDirectionArrowLength;

	GUIStyle buttonStyle = new GUIStyle ();

	private const int stepsPerCurve = 10;

	private static Color[] modeColors = {
		Color.green,
		Color.white,
		Color.cyan
	};

	Color mainPointColor = Color.red;

	private BezierSpline spline;
	private Transform handleTransform;
	private Quaternion handleRotation;


	void OnEnable ()
	{
		directionScale = serializedObject.FindProperty ("directionScale");
		newPointOffset = serializedObject.FindProperty ("newPointOffset");
		newPointBezierOffset = serializedObject.FindProperty ("newPointBezierOffset");
		points = serializedObject.FindProperty ("points");
		modes = serializedObject.FindProperty ("modes");
		lookDirectionList = serializedObject.FindProperty ("lookDirectionList");
		handleSize = serializedObject.FindProperty ("handleSize");
		pickSize = serializedObject.FindProperty ("pickSize");
		showGizmo = serializedObject.FindProperty("showGizmo");
		showLookDirectionGizmo = serializedObject.FindProperty ("showLookDirectionGizmo");
		lookDirectionGizmoRadius = serializedObject.FindProperty ("lookDirectionGizmoRadius");
		lookDirectionArrowLength = serializedObject.FindProperty ("lookDirectionArrowLength");

		spline = (BezierSpline)target;

		if (spline.points.Count == 0) {
			spline.Reset ();
		}
		//spline.Reset ();
	}

	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (directionScale);	
		EditorGUILayout.PropertyField (newPointOffset);
		EditorGUILayout.PropertyField (newPointBezierOffset);
		if (spline.points.Count > 4) {
			bool loop = EditorGUILayout.Toggle ("Loop", spline.Loop);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (spline, "Toggle Loop");
				EditorUtility.SetDirty (spline);
				spline.Loop = loop;
			}
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Spline State", "window");
		GUILayout.Label ("Selected Index\t " + spline.selectedIndex);
		GUILayout.Label ("Number Of Points\t " + ((Mathf.Round (spline.points.Count / 3)) + 1));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (spline.points.Count > 0 && spline.selectedIndex > -1) {
			GUILayout.BeginVertical ("Point State", "window");
			GUILayout.Label ("Selected Point");
			EditorGUI.BeginChangeCheck ();
			Vector3 point = EditorGUILayout.Vector3Field ("Position", spline.GetControlPoint (spline.selectedIndex));
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (spline, "Move Point");
				EditorUtility.SetDirty (spline);
				spline.SetControlPoint (spline.selectedIndex, point);
			}

			EditorGUI.BeginChangeCheck ();
			BezierSpline.BezierControlPointMode mode = (BezierSpline.BezierControlPointMode)EditorGUILayout.EnumPopup ("Mode", spline.GetControlPointMode (spline.selectedIndex));
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (spline, "Change Point Mode");
				spline.SetControlPointMode (spline.selectedIndex, mode);
				EditorUtility.SetDirty (spline);
			}
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();
		}

		if (GUILayout.Button ("Add Curve")) {
			spline.AddCurve (spline.points.Count, true);
		}

		if (GUILayout.Button ("x")) {
			spline.removeCurve (spline.selectedIndex);
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Point List", "window", GUILayout.Height (30));
		showPointList (points);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Mode List", "window", GUILayout.Height (30));
		showModeList (modes);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Look Direction List", "window", GUILayout.Height (30));
		showLookDirectionList (lookDirectionList);

		if (GUILayout.Button ("Align Look Direction With Spline Points (Pos and Rot)")) {
			spline.alignLookDirection (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Align Look Direction With Spline Points (Pos)")) {
			spline.alignLookDirection (false);
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window");
		EditorGUILayout.PropertyField (handleSize);
		EditorGUILayout.PropertyField (pickSize);
		EditorGUILayout.PropertyField (showGizmo);
		EditorGUILayout.PropertyField (showLookDirectionGizmo);
		EditorGUILayout.PropertyField (lookDirectionGizmoRadius);
		EditorGUILayout.PropertyField (lookDirectionArrowLength);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	private void OnSceneGUI ()
	{
		if (spline.showGizmo) {
			
			spline = target as BezierSpline;
			handleTransform = spline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			Vector3 p0 = ShowPoint (0);
			for (int i = 1; i < spline.ControlPointCount; i += 3) {
				Vector3 p1 = ShowPoint (i);
				Vector3 p2 = ShowPoint (i + 1);
				Vector3 p3 = ShowPoint (i + 2);

				Handles.color = Color.gray;
				Handles.DrawLine (p0, p1);
				Handles.DrawLine (p2, p3);

				Handles.DrawBezier (p0, p3, p1, p2, Color.white, null, 2f);
				p0 = p3;
			}

			ShowDirections ();
		}
	}

	private void ShowDirections ()
	{
		Handles.color = Color.green;
		Vector3 point = spline.GetPoint (0f);
		Handles.DrawLine (point, point + spline.GetDirection (0f) * spline.directionScale);
		int steps = stepsPerCurve * spline.CurveCount;

		for (int i = 1; i <= steps; i++) {
			point = spline.GetPoint (i / (float)steps);
			Handles.DrawLine (point, point + spline.GetDirection (i / (float)steps) * spline.directionScale);
		}
	}

	private Vector3 ShowPoint (int index)
	{
		Vector3 point = handleTransform.TransformPoint (spline.GetControlPoint (index));
		float size = HandleUtility.GetHandleSize (point);
		if (index == 0) {
			size *= 2f;
		}

		Handles.color = modeColors [(int)spline.GetControlPointMode (index)];

		if (index % 3 == 0) {
			Handles.color = mainPointColor;
		}
		if (Handles.Button (point, handleRotation, size * spline.handleSize, size * spline.pickSize, Handles.DotHandleCap)) {
			spline.selectedIndex = index;
			Repaint ();
		}

		if (spline.selectedIndex == index) {
			EditorGUI.BeginChangeCheck ();
			point = Handles.DoPositionHandle (point, handleRotation);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (spline, "Move Point");
				EditorUtility.SetDirty (spline);
				spline.SetControlPoint (index, handleTransform.InverseTransformPoint (point));
			}
		}

		return point;
	}

	void showPointList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Points: \t" + list.arraySize);
		
			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					GUILayout.Label ((i.ToString ()));
					GUILayout.Label ((list.GetArrayElementAtIndex (i).vector3Value).ToString ());
				}

				if (GUILayout.Button ("o")) {
					spline.setSelectedIndex (i);
				}

				if (i % 3 == 0) {
					if (GUILayout.Button ("x")) {
						spline.removeCurve (i);
					}
					if (GUILayout.Button ("+")) {
						spline.AddCurve (i + 1, false);
					}
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}
	}

	void showModeList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Modes: \t" + list.arraySize);

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

			}
			GUILayout.EndVertical ();
		}
	}


	void showLookDirectionList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Directions: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Direction")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear List")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();


			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ("box");
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndVertical ();
			}


			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}
}
#endif