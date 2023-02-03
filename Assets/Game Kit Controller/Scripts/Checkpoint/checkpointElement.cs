using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class checkpointElement : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int checkpointID;

	public bool overwriteThisCheckpoint;

	public List<string> tagToSave = new List<string> ();
	public bool saveInEveryTriggerEnter;

	[Space]
	[Header ("Transform Settings")]
	[Space]

	public bool useCustomSaveTransform;
	public Transform customSaveTransform;

	public bool useCustomCameraTransform;
	public Transform customCameraTransform;
	public Transform customCameraPivotTransform;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool checkpointAlreadyFound;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnCheckpointActivated;
	public UnityEvent eventOnCheckpointActivated;

	[Space]
	[Header ("Components")]
	[Space]

	public checkpointSystem checkpointManager;
	public Collider mainCollider;


	void Awake ()
	{
		StartCoroutine (activateTriggers ());
	}

	IEnumerator activateTriggers ()
	{
		if (mainCollider != null) {
			mainCollider.enabled = false;

			yield return new WaitForSeconds (1);

			mainCollider.enabled = true;
		}
	}

	public void setCheckPointManager (checkpointSystem manager)
	{
		checkpointManager = manager;

		updateComponent ();
	}

	public void OnTriggerEnter (Collider col)
	{
		if ((!checkpointAlreadyFound || saveInEveryTriggerEnter) && tagToSave.Contains (col.tag)) {
			checkpointAlreadyFound = true;

			playerComponentsManager currentPlayerComponentsManager = col.gameObject.GetComponent<playerComponentsManager> (); 

			if (currentPlayerComponentsManager != null) {
				saveGameSystem currentSaveGameSystem = currentPlayerComponentsManager.getSaveGameSystem ();
			
				if (checkpointManager == null) {
					checkpointManager = FindObjectOfType<checkpointSystem> (); 
				}

				if (checkpointManager != null) {
					if (useCustomSaveTransform) {
						currentSaveGameSystem.saveGameCheckpoint (customSaveTransform, checkpointID, checkpointManager.checkpointSceneID, overwriteThisCheckpoint, false, true);
					} else {
						currentSaveGameSystem.saveGameCheckpoint (null, checkpointID, checkpointManager.checkpointSceneID, overwriteThisCheckpoint, false, true);
					}

					checkpointManager.setCurrentCheckpointElement (transform);

					if (useEventOnCheckpointActivated) {
						eventOnCheckpointActivated.Invoke ();
					}
				}
			}
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}