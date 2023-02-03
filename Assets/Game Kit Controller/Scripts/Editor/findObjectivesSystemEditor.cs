using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof(findObjectivesSystem))]
public class findObjectivesSystemEditor : Editor
{
	findObjectivesSystem manager;

	float visionRange;
	Vector3 rangePosition;
	string text;

	Transform mainTransform;

	void OnEnable ()
	{
		manager = (findObjectivesSystem)target;
	}

	void OnSceneGUI ()
	{
		if (manager.showGizmo) {
			Handles.color = Color.white;

			mainTransform = manager.transform;

			rangePosition = mainTransform.position + mainTransform.up * 2;

			visionRange = manager.visionRange;

			//Handles.DrawWireArc (rangePosition, -manager.transform.up, manager.transform.right, visionRange, 2);
			Handles.DrawWireArc (rangePosition, mainTransform.up, mainTransform.forward, (visionRange / 2), manager.visionRangeGizmoRadius);
			Handles.DrawWireArc (rangePosition, -mainTransform.up, mainTransform.forward, (visionRange / 2), manager.visionRangeGizmoRadius);
		
			Vector3 viewAngleA = DirFromAngle (-visionRange / 2);
			Vector3 viewAngleB = DirFromAngle (visionRange / 2);

			Handles.DrawLine (rangePosition, rangePosition + viewAngleA * manager.visionRangeGizmoRadius);
			Handles.DrawLine (rangePosition, rangePosition + viewAngleB * manager.visionRangeGizmoRadius);

			string text = "Vision Range " + visionRange;

			Handles.color = Color.red;
			Handles.Label (rangePosition, text);	
		}
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

	}

	public Vector3 DirFromAngle (float angleInDegrees)
	{
		angleInDegrees += manager.transform.eulerAngles.y;
		return new Vector3 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
	}
}
#endif