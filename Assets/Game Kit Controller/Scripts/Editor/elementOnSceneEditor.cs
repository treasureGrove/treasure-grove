using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(elementOnScene))]
public class elementOnSceneEditor : Editor
{
	elementOnScene manager;

	void OnEnable ()
	{
		manager = (elementOnScene)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Add Single Element On Scene To Manager")) {
			manager.addSingleElementOnSceneToManager ();
		}

		EditorGUILayout.Space ();
//
//		if (GUILayout.Button ("Get All Elemets On Scene")) {
//			manager.getAllElementsOnSceneOnLevelByEditor ();
//		}
//
//		EditorGUILayout.Space ();
//
//		if (GUILayout.Button ("Clear Elements On Scene List")) {
//			manager.clearElementsOnSceneList ();
//		}
//
//		EditorGUILayout.Space ();
//
//		if (GUILayout.Button ("Enable State On All Elements On Scene")) {
//			manager.setEnableStateOnAllElementsOnScene (true);
//		}
//
//		EditorGUILayout.Space ();
//
//		if (GUILayout.Button ("Disable State On All Elements On Scene")) {
//			manager.setEnableStateOnAllElementsOnScene (false);
//		}
//
//		EditorGUILayout.Space ();
//		EditorGUILayout.Space ();
//
//		GUILayout.Label ("PREFABS ID BUTTONS", EditorStyles.boldLabel);
//
//		EditorGUILayout.Space ();
//
//		if (GUILayout.Button ("Set ID On Elements On Scene Prefabs")) {
//			manager.setIDOnElementsOnScenePrefabs ();
//		}
//
//		EditorGUILayout.Space ();
//
//		if (GUILayout.Button ("Set ID On Elements On Scene Prefabs Located On Scene")) {
//			manager.setIDOnElementsOnScenePrefabsLocatedOnScene ();
//		}
//
//		EditorGUILayout.Space ();
//
//		EditorGUILayout.Space ();
//
//		EditorGUILayout.Space ();
//
//		EditorGUILayout.Space ();
	}
}
#endif