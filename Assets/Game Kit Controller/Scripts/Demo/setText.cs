using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class setText : MonoBehaviour
{
	public bool editingText;
	public bool showGizmo;
	TextMesh textMesh;
	Text text;
	MeshRenderer mainMeshRenderer;

	//a script to set the text of every cartel in the scene, and checking if the player is inside the trigger
	void Start ()
	{
		mainMeshRenderer = GetComponent<MeshRenderer> ();
		GetComponent<TextMesh> ().text = GetComponent<Text> ().text.Replace ("|", "\n");
		if (GetComponent<Collider> ()) {
			mainMeshRenderer.enabled = false;
		} else {
			mainMeshRenderer.enabled = true;
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.CompareTag ("Player")) {
			mainMeshRenderer.enabled = true;
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (col.gameObject.CompareTag ("Player")) {
			mainMeshRenderer.enabled = false;
		}
	}

	public void getmainMeshRendererComponent ()
	{
		if (mainMeshRenderer == null) {
			mainMeshRenderer = GetComponent<MeshRenderer> ();
		}
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			if (!Application.isPlaying) {
				if (mainMeshRenderer) {
					if (editingText) {
						if (!mainMeshRenderer.enabled) {
							mainMeshRenderer.enabled = true;
						}
						if (!textMesh || !text) {
							textMesh = GetComponent<TextMesh> ();
							text = GetComponent<Text> ();
						}
						textMesh.text = text.text.Replace ("|", "\n");
					} else {
						if (mainMeshRenderer.enabled) {
							mainMeshRenderer.enabled = false;
						}
					}
				} else {
					getmainMeshRendererComponent ();
				}
			}
		}
	}
}