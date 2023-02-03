using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

//a simple editor to add a button in the ragdollBuilder script inspector
[CustomEditor (typeof(ragdollBuilder))]
public class ragdollBuilderEditor : Editor
{
	public enum ragdollStateValues
	{
		NO,
		YES,
	}

	public ragdollStateValues ragdollCreated;
	bool checkState;
	ragdollBuilder player;

	void OnEnable ()
	{
		player = (ragdollBuilder)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Search Bones On New Character")) {
			if (!Application.isPlaying) {
				player.getCharacterBonesFromEditor ();
			}
		}

		//check if the current player has a ragdoll or not, to show it in the inspector
		if (!checkState) {
			if (player.ragdollAdded) {
				ragdollCreated = ragdollStateValues.YES;
			} else {
				ragdollCreated = ragdollStateValues.NO;
			}
			checkState = true;
		}

		EditorGUILayout.Space ();

		EditorGUILayout.Space ();
				
		GUILayout.Label ("RAGDOLL ADDED: " + ragdollCreated);
		if (GUILayout.Button ("Build Ragdoll")) {
			if (!Application.isPlaying) {
				if (player.createRagdoll ()) {
					ragdollCreated = ragdollStateValues.YES;
				}
			}
		}

		if (GUILayout.Button ("Remove Ragdoll")) {
			if (!Application.isPlaying) {
				if (player.removeRagdollFromEditor ()) {
					ragdollCreated = ragdollStateValues.NO;
				}
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Enable Ragdoll Colliders")) {
			if (!Application.isPlaying) {
				player.enableRagdollColliders ();
			}
		}
		if (GUILayout.Button ("Disable Ragdoll Colliders")) {
			if (!Application.isPlaying) {
				player.disableRagdollColliders ();
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Update Ragdoll Activator Parts")) {
			if (!Application.isPlaying) {
				player.updateRagdollActivatorParts ();
			}
		}
	}
}
#endif