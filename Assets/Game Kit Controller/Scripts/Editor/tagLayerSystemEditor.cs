using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

//a simple editor to add a button in the ragdollBuilder script inspector
[CustomEditor (typeof(tagLayerSystem))]
public class tagLayerSystemEditor : Editor
{
	SerializedProperty newTag;
	SerializedProperty tagList;
	SerializedProperty newLayer;
	SerializedProperty layerList;

	SerializedProperty minLayerIndexToCheck;

	tagLayerSystem settingsManager;
	bool tagMenu;
	bool layerMenu;
	Color defBackgroundColor;

	private GUIStyle headerFoldout;
	bool tagListEnabled = true;
	bool layerListEnabled = true;

	void OnEnable ()
	{
		newTag = serializedObject.FindProperty ("newTag");
		tagList = serializedObject.FindProperty ("tagList");
		newLayer = serializedObject.FindProperty ("newLayer");
		layerList = serializedObject.FindProperty ("layerList");

		minLayerIndexToCheck = serializedObject.FindProperty ("minLayerIndexToCheck");

		settingsManager = (tagLayerSystem)target;
	}

	public override void OnInspectorGUI ()
	{
		if (headerFoldout == null) {
			headerFoldout = new GUIStyle(EditorStyles.foldout);
			headerFoldout.fontStyle = FontStyle.Bold;
		}

		EditorGUILayout.Space ();

		GUILayout.BeginVertical (GUILayout.Height (30));

		EditorGUILayout.Space ();

		GUI.color = Color.cyan;
		EditorGUILayout.HelpBox ("Manage all Tags/Layers with this editor", MessageType.None);
		GUI.color = Color.white;

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		defBackgroundColor = GUI.backgroundColor;
		EditorGUILayout.BeginHorizontal ();
		if (tagMenu) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Tag Menu")) {
			tagMenu = !tagMenu;
		}
		if (layerMenu) {
			GUI.backgroundColor = Color.gray;
		} else {
			GUI.backgroundColor = defBackgroundColor;
		}
		if (GUILayout.Button ("Layer Menu")) {
			layerMenu = !layerMenu;
		}
		GUI.backgroundColor = defBackgroundColor;
		EditorGUILayout.EndHorizontal ();
		if (tagMenu) {
			GUILayout.BeginVertical ("Tag Manager", "window");

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Add/remove tags one by one or in a list", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();

			GUILayout.Label ("ADD SINGLE TAG:");

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (newTag, new GUIContent ("New Tag To Add"), false);

			EditorGUILayout.Space ();

			if (newTag.stringValue.Length > 0) {
				if (GUILayout.Button ("Add Tag")) {
					settingsManager.addTag (newTag.stringValue);
				}
			}

			EditorGUILayout.Space ();

			tagListEnabled = GUILayout.Toggle (tagListEnabled, "Tag List", headerFoldout);

			showLowerList (tagList, false, true, tagListEnabled);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Get Tags From Editor")) {
				settingsManager.getTagList ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Add Tags To Editor")) {
				settingsManager.addCurrentTagList ();
			}

			GUILayout.EndVertical ();
		}

		if (layerMenu) {
			GUILayout.BeginVertical ("Layer Manager", "window");

			GUI.color = Color.cyan;
			EditorGUILayout.HelpBox ("Add layers one by one or in a list", MessageType.None);
			GUI.color = Color.white;

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (minLayerIndexToCheck, false);

			EditorGUILayout.Space ();

			GUILayout.Label ("ADD SINGLE LAYER:");

			EditorGUILayout.Space ();

			EditorGUILayout.PropertyField (newLayer, new GUIContent ("New Layer To Add"), false);

			EditorGUILayout.Space ();

			if (newLayer.stringValue.Length > 0) {
				if (GUILayout.Button ("Add Layer")) {
					settingsManager.addLayer (newLayer.stringValue);
				}
			}

			EditorGUILayout.Space ();

			layerListEnabled = GUILayout.Toggle (layerListEnabled, "Layer List", headerFoldout);

			showLowerList (layerList, true, true, layerListEnabled);

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Get Layers From Editor")) {
				settingsManager.getLayerList ();
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Replace Layers In Editor")) {
				settingsManager.addCurrentLayerList ();
			}

			GUILayout.EndVertical ();
		}

		GUILayout.EndVertical ();

		GUI.backgroundColor = defBackgroundColor;
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showLowerList (SerializedProperty list, bool managerList, bool isTag, bool listEnabled)
	{
		if (listEnabled) {
			if (managerList) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Add Element")) {
					list.arraySize++;
				}
				if (GUILayout.Button ("Clear List")) {
					list.arraySize = 0;
				}
				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("box");

			EditorGUILayout.Space ();

			if (!managerList) {
				GUILayout.Label (list.arraySize + " Elements");

				EditorGUILayout.Space ();
			}

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					if (isTag) {
						Debug.Log ("remove tag");
						string tagToRemove = list.GetArrayElementAtIndex (i).stringValue as string;
						settingsManager.removeTag (tagToRemove);
						list.DeleteArrayElementAtIndex (i);
					}

					if (!isTag) {
						list.DeleteArrayElementAtIndex (i);
					}
				}

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.Space ();

			GUILayout.EndVertical ();
		}       
	}
}
#endif
