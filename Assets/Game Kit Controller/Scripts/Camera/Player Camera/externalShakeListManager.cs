using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class externalShakeListManager : MonoBehaviour
{
	public List<externalShakeInfoListElement> externalShakeInfoList = new List<externalShakeInfoListElement> ();


	public void setShakeInManagerList (externalShakeInfoListElement element, int index)
	{
		externalShakeInfoList [index] = element;
	}

	public void udpateAllHeadbobShakeList ()
	{
		headBob[] headBobList = FindObjectsOfType<headBob> ();
		foreach (headBob bob in headBobList) {
			bob.updateExternalShakeInfoList (externalShakeInfoList);
		}

		print ("All head bob in the scene have been updated with the current shake list");
	}

	public void setExternalShakeStateByIndex (int index, bool isFirstPerson)
	{
		externalShakeInfoListElement newShake = externalShakeInfoList [index];

		headBob[] headBobList = FindObjectsOfType<headBob> ();
		foreach (headBob bob in headBobList) {
			if (isFirstPerson) {
				bob.setExternalShakeState (newShake.firstPersonDamageShake);

			} else {
				bob.setExternalShakeState (newShake.thirdPersonDamageShake);
			}
		}
	}
}