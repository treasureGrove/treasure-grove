using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class infoPanelOnScreenSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool infoPanelEnabled = true;

	public bool showPanelInfoOnlyFirstTime;

	[Space]
	[Header ("Panel Detection Settings")]
	[Space]

	public List<string> tagsToCheck = new List<string> ();
	public LayerMask layermaskToCheck;

	public Transform objectToFollow;

	[Space]
	[Header ("Name Text Settings")]
	[Space]

	public bool usePanelNameText;
	public string panelNameText;

	[Space]
	[Header ("Image Settings")]
	[Space]

	public bool setImageOnPanel;
	public Texture imageOnPanel;

	[Space]
	[Header ("Panel Type Settings")]
	[Space]

	public string panelName = "Default";

	[TextArea (3, 10)] public string panelNameExplanation = "There are 4 types so far, but you can add any amount of panel info types: \n" +
	                                                        "Little, Default, Big, Large and Extra Large";

	[Space]
	[Header ("Panel Text Settings")]
	[Space]

	[TextArea (8, 15)] public string panelOnScreenText;

	public string includedActionNameOnText;

	[Space]
	[Space]

	[TextArea (3, 10)] public string explanation = "Write -ACTION NAME- in the field Panel On Screen Text where you want to place the" +
	                                               "key used for an action. For example, by default, Jump will show 'Space";

	[Space]
	[Header ("Panel Position Settings")]
	[Space]

	public Vector3 panelOffset;

	public bool useFixedPanelPosition;

	public bool useSeparatedTransformForEveryView;
	public Transform transformForThirdPerson;
	public Transform transformForFirstPerson;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoLabelColor = Color.green;
	public Color gizmoColor = Color.white;
	public float gizmoRadius = 0.3f;


	List<GameObject> playerFoundList = new List<GameObject> ();

	GameObject currentPlayer;


	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (!infoPanelEnabled) {
			return;
		}

		if ((1 << col.gameObject.layer & layermaskToCheck.value) == 1 << col.gameObject.layer) {

			if (isEnter) {
			
				if (tagsToCheck.Contains (col.tag)) {

					if (!playerFoundList.Contains (col.gameObject)) {
						playerFoundList.Add (col.gameObject);
					}

					currentPlayer = col.gameObject;

					enableOrdisablePanelInfoFromPlayer (currentPlayer, true);
				}
			} else {

				//if the player is leaving the trigger
				if (tagsToCheck.Contains (col.tag)) {
					//if the player is the same that was using the device, the device can be used again
					if (playerFoundList.Contains (col.gameObject)) {
						playerFoundList.Remove (col.gameObject);
					}

					enableOrdisablePanelInfoFromPlayer (col.gameObject, false);

					if (playerFoundList.Count == 0) {
						currentPlayer = null;

						if (showPanelInfoOnlyFirstTime) {
							disablePanelText ();
						}
					}
				}
			}
		}
	}

	public void disablePanelText ()
	{
		if (playerFoundList.Count > 0) {
			for (int i = 0; i < playerFoundList.Count; i++) {
				enableOrdisablePanelInfoFromPlayer (playerFoundList [i], false);
			}

			infoPanelEnabled = false;

			gameObject.SetActive (false);
		}
	}

	public void enableOrdisablePanelInfoFromPlayer (GameObject playerToCheck, bool enablePanel)
	{
		playerComponentsManager currentPlayerComponentsManager = playerToCheck.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {

			playerInfoPanelOnScreenSystem currentPlayerInfoPanelOnScreenSystem = currentPlayerComponentsManager.getPlayerInfoPanelOnScreenSystem ();

			if (currentPlayerInfoPanelOnScreenSystem != null) {

				if (objectToFollow == null) {
					objectToFollow = transform;
				}

				if (enablePanel) {
					currentPlayerInfoPanelOnScreenSystem.getNewPanelInfo (this);
				} else {
					currentPlayerInfoPanelOnScreenSystem.disablePanelInfo (objectToFollow);
				}
			}
		}
	}

	public void disablePanelInfoFromAllPlayersRemotely ()
	{
		if (playerFoundList.Count > 0) {
			for (int i = 0; i < playerFoundList.Count; i++) {
				enableOrdisablePanelInfoFromPlayer (playerFoundList [i], false);
			}
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
			Gizmos.color = gizmoColor;
			Vector3 gizmoPosition = transform.position + panelOffset;

			if (useSeparatedTransformForEveryView) {
				if (transformForThirdPerson != null) {
					Gizmos.color = Color.white;
					gizmoPosition = transformForThirdPerson.position;
					Gizmos.DrawSphere (gizmoPosition, gizmoRadius);
				}

				if (transformForFirstPerson != null) {
					Gizmos.color = Color.yellow;
					gizmoPosition = transformForFirstPerson.position;
					Gizmos.DrawSphere (gizmoPosition, gizmoRadius);
				}
			} else {
				if (objectToFollow != null) {
					gizmoPosition = objectToFollow.position;
				} else {
					gizmoPosition = transform.position;
				}

				Gizmos.DrawSphere (gizmoPosition, gizmoRadius);
			}
		}
	}
}
