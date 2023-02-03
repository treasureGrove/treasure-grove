using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(meleeWeaponAttackListInfoSocket))]
public class meleeWeaponAttackListInfoSocketEditor : Editor
{
	meleeWeaponAttackListInfoSocket manager;

	Vector3 curretPositionHandle;
	Quaternion currentRotationHandle;

	void OnEnable ()
	{
		manager = (meleeWeaponAttackListInfoSocket)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Copy Weapon Info To Template")) {
			manager.copyWeaponInfoToTemplate (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Copy Template To Weapon Attack Info")) {
			manager.copyTemplateToWeaponAttackInfo (true);
		}

		EditorGUILayout.Space ();
	}
}
#endif