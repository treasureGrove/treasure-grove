using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

public class createMeleeWeaponEditor : EditorWindow
{
	static string prefabsPath = "";

	static string editorTitle = "Melee Weapon Objects";

	static string editorDescription = "Create New Melee Weapon";

	static string editorSecondaryTitle = "Create Melee Object Weapon";

	static string editorInstructions = "Select a Melee Weapon Type from the 'Object Type' list and press the button 'Create Object'. \n\n" +
	                                   "After that, make sure to adjust the collider size and shape for the new object created as you consider better.\n\n";

	[MenuItem ("Game Kit Controller/Create New Weapon/Create New Melee Weapon", false, 201)]
	public static void createPhysicalObjectToGrab ()
	{
		createPhysicalObjectToGrabEditor editorWindow = EditorWindow.GetWindow<createPhysicalObjectToGrabEditor> ();

		prefabsPath = pathInfoValues.getMeleeWeaponObjectPath ();

		editorWindow.setTemporalPrefabsPath (prefabsPath, editorTitle, editorDescription, editorSecondaryTitle, editorInstructions, true);
	}
}
#endif