using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(dialogManager))]
public class dialogManagerEditor : Editor
{
	dialogManager manager;

	void OnEnable ()
	{
		manager = (dialogManager)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Get All Dialog Content Systems On Level And Assign Info")) {
			manager.getAllDialogContentOnLevelAndAssignInfo ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Clear Dialog Content System List")) {
			manager.clearDialogContentList ();
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Instantiate Dialog Content System")) {
			manager.instantiateDialogContentSystem ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Instantiate External Dialog System")) {
			manager.instantiateExternalDialogSystem ();
		}

		EditorGUILayout.Space ();
	}
}
#endif