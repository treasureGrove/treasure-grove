using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(decalTypeInformation))]
[CanEditMultipleObjects]
public class decalTypeInformationEditor : Editor
{
	SerializedProperty mainManagerName;

	SerializedProperty impactDecalList;
	SerializedProperty impactDecalIndex;
	SerializedProperty impactDecalName;
	SerializedProperty decalSurfaceActive;
	SerializedProperty parentScorchOnThisObject;
	SerializedProperty showGizmo;
	SerializedProperty gizmoLabelColor;

	decalTypeInformation decalType;
	GUIStyle style = new GUIStyle ();

	void OnEnable ()
	{
		mainManagerName = serializedObject.FindProperty ("mainManagerName");

		impactDecalList = serializedObject.FindProperty ("impactDecalList");
		impactDecalIndex = serializedObject.FindProperty ("impactDecalIndex");
		impactDecalName = serializedObject.FindProperty ("impactDecalName");
		decalSurfaceActive = serializedObject.FindProperty ("decalSurfaceActive");
		parentScorchOnThisObject = serializedObject.FindProperty ("parentScorchOnThisObject");
		showGizmo = serializedObject.FindProperty ("showGizmo");
		gizmoLabelColor = serializedObject.FindProperty ("gizmoLabelColor");

		decalType = (decalTypeInformation)target;
	}

	void OnSceneGUI ()
	{   
		if (decalType.showGizmo) {
			style.normal.textColor = decalType.gizmoLabelColor;
			style.alignment = TextAnchor.MiddleCenter;

			Handles.Label (decalType.transform.position + decalType.transform.up, decalType.impactDecalName, style);	
		}
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Impact Surface Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (mainManagerName);

		EditorGUILayout.Space ();

		if (impactDecalList.arraySize > 0) {
			impactDecalIndex.intValue = EditorGUILayout.Popup ("Decal Impact Type", 
				impactDecalIndex.intValue, decalType.impactDecalList);
			impactDecalName.stringValue = decalType.impactDecalList [impactDecalIndex.intValue];
		}

		EditorGUILayout.PropertyField (decalSurfaceActive);
		EditorGUILayout.PropertyField (parentScorchOnThisObject);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Decal Impact List")) {
			decalType.getImpactListInfo ();
		}

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Gizmo Settings", "window", GUILayout.Height (30));
		EditorGUILayout.PropertyField (showGizmo);
		if (showGizmo.boolValue) {
			EditorGUILayout.PropertyField (gizmoLabelColor);
		}
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
#endif