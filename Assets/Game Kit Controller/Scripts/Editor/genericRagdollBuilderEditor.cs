using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(genericRagdollBuilder))]
public class genericRagdollBuilderEditor : Editor
{
	genericRagdollBuilder manager;

	Vector3 curretPositionHandle;
	Quaternion currentRotationHandle;

	void OnEnable ()
	{
		manager = (genericRagdollBuilder)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Build Ragdoll")) {
			manager.buildRagdoll ();
		}
		if (GUILayout.Button ("Remove Ragdoll")) {
			manager.removeRagdoll ();
		}
			
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Activate Ragdoll Elements")) {
			manager.enableRagdollElements ();
		}
		if (GUILayout.Button ("Deactivate Ragdoll Elements")) {
			manager.disableRagdollElements ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Send Bone List On Event")) {
			manager.activateEventToSendBonesList ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get Ragdoll Elements On Bone List Manually")) {
			manager.getRagdollElementsOnBoneListManually ();
		}

		EditorGUILayout.Space ();
	}
}
#endif