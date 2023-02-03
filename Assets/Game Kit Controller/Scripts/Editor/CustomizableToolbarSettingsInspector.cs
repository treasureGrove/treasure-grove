using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor (typeof(CustomizableToolbarSettings))]
public class CustomizableToolbarSettingsInspector : Editor
{
	private SerializedProperty m_property;
	private ReorderableList m_reorderableList;

	SerializedProperty numberOfColumns;

	SerializedProperty windowsHeight;

	void OnEnable ()
	{
		m_property = serializedObject.FindProperty ("m_list");
		m_reorderableList = new ReorderableList (serializedObject, m_property) {
			elementHeight = 120,
			drawElementCallback = OnDrawElement
		};

		numberOfColumns = serializedObject.FindProperty ("numberOfColumns");

		windowsHeight = serializedObject.FindProperty ("windowsHeight");
	}

	private void OnDrawElement (Rect rect, int index, bool isActive, bool isFocused)
	{
		var element = m_property.GetArrayElementAtIndex (index);
		rect.height -= 4;
		rect.y += 2;
		EditorGUI.PropertyField (rect, element);
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();

		EditorGUILayout.PropertyField (numberOfColumns);

		EditorGUILayout.PropertyField (windowsHeight);

		EditorGUILayout.Space ();

		if (GUILayout.Button (new GUIContent ("Update Values"))) {
			CustomizableToolbar.updateToolBarValues ();
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		m_reorderableList.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();
	}
}