using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System;

public static class CollapseInspectorEditor
{
	[MenuItem ("CONTEXT/Component/Collapse All")]
	private static void CollapseAll (MenuCommand command)
	{
		SetAllInspectorsExpanded ((command.context as Component).gameObject, false);
	}

	[MenuItem ("CONTEXT/Component/Expand All")]
	private static void ExpandAll (MenuCommand command)
	{
		SetAllInspectorsExpanded ((command.context as Component).gameObject, true);
	}

	public static void SetAllInspectorsExpanded (GameObject go, bool expanded)
	{
		Component[] components = go.GetComponents<Component> ();
		foreach (Component component in components) {
			if (component is Renderer) {
				var mats = ((Renderer)component).sharedMaterials;
				for (int i = 0; i < mats.Length; ++i) {
					InternalEditorUtility.SetIsInspectorExpanded (mats [i], expanded);
				}
			}
			InternalEditorUtility.SetIsInspectorExpanded (component, expanded);
		}
		ActiveEditorTracker.sharedTracker.ForceRebuild ();
	}

	public static void CollapseAllComponentsButOne (GameObject go, Type componentToCheck)
	{
		SetAllInspectorsExpanded (go, false);

		Component componentToSearch = go.GetComponent (componentToCheck);

		if (componentToSearch != null) {
			InternalEditorUtility.SetIsInspectorExpanded (componentToSearch, true);

			ActiveEditorTracker.sharedTracker.ForceRebuild ();
		}
	}
}
#endif