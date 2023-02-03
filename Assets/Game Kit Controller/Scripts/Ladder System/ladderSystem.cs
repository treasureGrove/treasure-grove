using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ladderSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool ladderActive = true;

	public bool useLadderHorizontalMovement = true;

	public bool moveInLadderCenter;

	public bool useLocalMovementDirection;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsEnterExitLadder;
	public eventParameters.eventToCallWithGameObject eventToSendCurrentPlayerOnEnter;
	public eventParameters.eventToCallWithGameObject eventToSendCurrentPlayerOnExit;


	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo = true;
	public Color gizmoColor = Color.red;
	public float gizmoLength = 4;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform ladderDirectionTransform;
	public Transform ladderRaycastDirectionTransform;

	GameObject currentPlayer;
	playerComponentsManager mainPlayerComponentsManager;
	playerLadderSystem mainPlayerLadderSystem;

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
		if (!ladderActive) {
			return;
		}

		if (!col.gameObject.CompareTag (tagToCheck)) {
			return;
		}

		if (isEnter) {
			currentPlayer = col.gameObject;

			mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			mainPlayerLadderSystem = mainPlayerComponentsManager.getPlayerLadderSystem ();

			mainPlayerLadderSystem.setLadderFoundState (true, this);

			if (ladderDirectionTransform == null) {
				ladderDirectionTransform = transform;
			}

			if (ladderRaycastDirectionTransform == null) {
				ladderRaycastDirectionTransform = transform;
			}
				
			mainPlayerLadderSystem.setLadderDirectionTransform (ladderDirectionTransform, ladderRaycastDirectionTransform);

			mainPlayerLadderSystem.setLadderHorizontalMovementState (useLadderHorizontalMovement);

			mainPlayerLadderSystem.setMoveInLadderCenterState (moveInLadderCenter);

			mainPlayerLadderSystem.setUseLocalMovementDirectionState (useLocalMovementDirection);

			if (useEventsEnterExitLadder) {
				eventToSendCurrentPlayerOnEnter.Invoke (currentPlayer);
			}

		} else {
			currentPlayer = col.gameObject;

			mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			mainPlayerLadderSystem = mainPlayerComponentsManager.getPlayerLadderSystem ();

			mainPlayerLadderSystem.setLadderFoundState (false, this);

			if (useEventsEnterExitLadder) {
				eventToSendCurrentPlayerOnExit.Invoke (currentPlayer);
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
			if (ladderDirectionTransform != null) {
				GKC_Utils.drawGizmoArrow (transform.position - ladderDirectionTransform.forward * gizmoLength, ladderDirectionTransform.forward * gizmoLength, gizmoColor, 1, 20);

				GKC_Utils.drawGizmoArrow (transform.position, transform.up * gizmoLength, gizmoColor, 1, 20);
			}
		}
	}
}
