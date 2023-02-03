using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class powersAndAbilitiesSystem : MonoBehaviour
{
	public List<powerInfo> powerInfoList = new List<powerInfo> ();

	public void disableGeneralPower (string powerName)
	{
		for (int i = 0; i < powerInfoList.Count; i++) {
			if (powerInfoList [i].Name.Equals (powerName)) {
				powerInfoList [i].eventToDisable.Invoke ();

				return;
			}
		}
	}

	public void enableGeneralPower (string powerName)
	{
		for (int i = 0; i < powerInfoList.Count; i++) {
			if (powerInfoList [i].Name.Equals (powerName)) {
				powerInfoList [i].eventToEnable.Invoke ();

				return;
			}
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	[System.Serializable]
	public class powerInfo
	{
		public string Name;
		public UnityEvent eventToEnable;
		public UnityEvent eventToDisable;
	}
}
