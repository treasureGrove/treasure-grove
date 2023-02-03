using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapPickup : pickupType
{
	public override bool checkIfCanBePicked ()
	{
		if (finderIsPlayer) {
			canPickCurrentObject = true;
		}

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (finderIsPlayer) {
			GetComponent<mapZoneUnlocker> ().unlockMapZone ();

			if (useCustomPickupMessage) {
				showPickupTakenMessage (amountTaken);
			} else {
				showPickupTakenMessage ("Map Zone Picked");
			}

			mainPickupObject.playPickupSound ();
		}

		mainPickupObject.removePickupFromLevel ();
	}
}