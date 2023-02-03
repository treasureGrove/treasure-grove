using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPartstToRemoveOnPickupCreation : MonoBehaviour
{
	public List<GameObject> objectListToRemove = new List<GameObject> ();

	public void removeWeaponObjects ()
	{
		for (int i = 0; i < objectListToRemove.Count; i++) {
			if (objectListToRemove [i] != null) {
				print ("destroy " + objectListToRemove [i].name);
				DestroyImmediate (objectListToRemove [i]);
			}
		}

		objectListToRemove.Clear ();
	}
}
