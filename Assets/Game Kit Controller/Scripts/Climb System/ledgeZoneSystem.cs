using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ledgeZoneSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool ledgeZoneActive = true;
	public Vector3 checkDownRaycastOffset;
	public float climbLedgeForwardRayDistance;
	public float climbLedgeDownRayDistance;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool ledgeZoneCanBeClimbed = true;

	public bool avoidPlayerGrabLedge;

	public bool canCheckForHangFromLedgeOnGround = true;

	public bool onlyHangFromLedgeIfPlayerIsNotMoving = true;

	public bool canGrabAnySurfaceOnAirActive = true;

	GameObject currentPlayer;
	climbLedgeSystem climbLedgeManager;


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
		if (!ledgeZoneActive) {
			return;
		}

		if (!col.gameObject.CompareTag (tagToCheck)) {
			return;
		}

		if (isEnter) {
			currentPlayer = col.gameObject;

			playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				climbLedgeManager = mainPlayerComponentsManager.getClimbLedgeSystem ();
			}

			if (climbLedgeManager == null) {
				return;
			}

			if (avoidPlayerGrabLedge) {
				climbLedgeManager.setAvoidPlayerGrabLedgeValue (true);

				return;
			}

			climbLedgeManager.setLedgeZoneFoundState (true);
			climbLedgeManager.setNewRaycastDistance (climbLedgeForwardRayDistance, climbLedgeDownRayDistance, checkDownRaycastOffset);

			climbLedgeManager.setCanClimbCurrentLedgeZoneState (ledgeZoneCanBeClimbed);

			climbLedgeManager.setCanCheckForHangFromLedgeOnGroundState (canCheckForHangFromLedgeOnGround);

			climbLedgeManager.setOnlyHangFromLedgeIfPlayerIsNotMovingValue (onlyHangFromLedgeIfPlayerIsNotMoving);

			climbLedgeManager.setCanGrabAnySurfaceOnAirActiveState (canGrabAnySurfaceOnAirActive);
		} else {
			currentPlayer = col.gameObject;

			playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				climbLedgeManager = mainPlayerComponentsManager.getClimbLedgeSystem ();
			}

			if (climbLedgeManager == null) {
				return;
			}

			if (avoidPlayerGrabLedge) {
				climbLedgeManager.setAvoidPlayerGrabLedgeValue (false);

				return;
			}

			climbLedgeManager.setLedgeZoneFoundState (false);
			climbLedgeManager.setOriginalRaycastDistance ();

			climbLedgeManager.setCanClimbCurrentLedgeZoneState (true);

			climbLedgeManager.setCanCheckForHangFromLedgeOnGroundState (true);

			climbLedgeManager.setOnlyHangFromLedgeIfPlayerIsNotMovingOriginalValue ();

			climbLedgeManager.setCanGrabAnySurfaceOnAirActiveState (true);
		}
	}
}
