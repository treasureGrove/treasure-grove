using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class meleeWeaponRangeAttacksSystem : MonoBehaviour
{
	public List<rangeWeaponInfo> rangeWeaponInfoList = new List<rangeWeaponInfo> ();

	[System.Serializable]
	public class rangeWeaponInfo
	{
		public string Name;

		public int rangeWeaponID;

		public List<Transform> projectilePosition = new List<Transform> ();

		public bool useEventOnRangeWeapon;
		public UnityEvent eventOnRangeWeapon;
	}
}
