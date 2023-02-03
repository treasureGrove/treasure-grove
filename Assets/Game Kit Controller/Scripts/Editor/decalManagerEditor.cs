using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(decalManager))]
public class decalManagerEditor : Editor
{
	SerializedProperty disableFullDecallSystem;
	SerializedProperty decalsPlacedOnSurfacesEnabled;
	SerializedProperty particlesPlacedOnSurfaceEnabled;
	SerializedProperty soundsPlacedOnSurfaceEnabled;

	SerializedProperty fadeDecals;
	SerializedProperty fadeSpeed;
	SerializedProperty projectileImpactSoundPrefab;
	SerializedProperty impactListInfo;

	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		disableFullDecallSystem = serializedObject.FindProperty ("disableFullDecallSystem");
		decalsPlacedOnSurfacesEnabled = serializedObject.FindProperty ("decalsPlacedOnSurfacesEnabled");
		particlesPlacedOnSurfaceEnabled = serializedObject.FindProperty ("particlesPlacedOnSurfaceEnabled");
		soundsPlacedOnSurfaceEnabled = serializedObject.FindProperty ("soundsPlacedOnSurfaceEnabled");

		fadeDecals = serializedObject.FindProperty ("fadeDecals");
		fadeSpeed = serializedObject.FindProperty ("fadeSpeed");
		projectileImpactSoundPrefab = serializedObject.FindProperty ("projectileImpactSoundPrefab");
		impactListInfo = serializedObject.FindProperty ("impactListInfo");
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		GUILayout.BeginVertical (GUILayout.Height (30));

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (fadeDecals);
		EditorGUILayout.PropertyField (fadeSpeed);

		EditorGUILayout.PropertyField (projectileImpactSoundPrefab);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Decal System Enabled Settings", "window");
		EditorGUILayout.PropertyField (disableFullDecallSystem);
		EditorGUILayout.PropertyField (decalsPlacedOnSurfacesEnabled);
		EditorGUILayout.PropertyField (particlesPlacedOnSurfaceEnabled);
		EditorGUILayout.PropertyField (soundsPlacedOnSurfaceEnabled);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Impact Decal List", "window");
		showImpactListInfo (impactListInfo);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();

		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showImpactListInfo (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Number Of Decals: \t" + list.arraySize);
			EditorGUILayout.Space ();
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Decal")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();

				GUILayout.BeginHorizontal ("box");
				EditorGUILayout.Space ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
					if (list.GetArrayElementAtIndex (i).isExpanded) {
						expanded = true;
						showDecalInfo (list.GetArrayElementAtIndex (i));
					}
					EditorGUILayout.Space ();
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();

				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
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
				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showDecalInfo (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("name"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("decalEnabled"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("surfaceName"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useNoise"));
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Effect Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactSound"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("impactParticles"));
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("scorch"));
		if (list.FindPropertyRelative ("scorch").objectReferenceValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("scorchScale"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("fadeScorch"));
			if (list.FindPropertyRelative ("fadeScorch").boolValue) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("timeToFade"));
			}
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Terrain Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("checkTerrain"));
		if (list.FindPropertyRelative ("checkTerrain").boolValue) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("terrainTextureIndex"));
		}
		GUILayout.EndVertical ();

		GUILayout.EndVertical ();
	}
}
#endif