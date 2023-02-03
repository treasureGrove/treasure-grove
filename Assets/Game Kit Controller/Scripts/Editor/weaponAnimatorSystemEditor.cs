using UnityEngine;
using System.Collections;

using UnityEditor;

[CustomEditor (typeof(weaponAnimatorSystem))]
public class weaponAnimatorSystemEditor : Editor
{
	weaponAnimatorSystem manager;

	void OnEnable ()
	{
		manager = (weaponAnimatorSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable Weapon Animator")) {
			manager.enableOrDisableWeaponAnimator (true);
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Disable Weapon Animator")) {
			manager.enableOrDisableWeaponAnimator (false);
		}
	}
}