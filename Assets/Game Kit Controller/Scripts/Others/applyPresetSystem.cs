using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

#if UNITY_2019_1_OR_NEWER
using UnityEditor.Presets;
#endif

#endif

public class applyPresetSystem : MonoBehaviour
{
	public static void GKCapplyProjectSettings ()
	{
		#if UNITY_EDITOR

		print ("Apply tag manager and input manager presets");

		#if UNITY_2019_1_OR_NEWER

		Preset mainInputPreset = AssetDatabase.LoadAssetAtPath<Preset> ("Assets/Game Kit Controller/Presets/GKC InputManager.preset");

		#endif

		SerializedObject mainInputManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset") [0]);

		if (mainInputManager != null) {
		
			Object inputManagerTargetObject = mainInputManager.targetObject;

			#if UNITY_2019_1_OR_NEWER

			mainInputPreset.ApplyTo (inputManagerTargetObject);
			
			print ("Input manager preset applied");

			#endif

		}

		#if UNITY_2019_1_OR_NEWER

		Preset tagManagerPreset = AssetDatabase.LoadAssetAtPath<Preset> ("Assets/Game Kit Controller/Presets/GKC TagManager.preset");

		#endif

		SerializedObject tagManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset") [0]);

		if (tagManager != null) {
			Object tagManagerTargetObject = tagManager.targetObject;

			#if UNITY_2019_1_OR_NEWER

			tagManagerPreset.ApplyTo (tagManagerTargetObject);
			
			print ("Tag manager preset applied");

			#endif
		}

		#endif
	}
}