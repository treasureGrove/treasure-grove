using UnityEngine;
using System.Collections;

[System.Serializable]
public class externalShakeInfoListElement
{
	public string name;
	public bool sameValueBothViews;
	public bool useDamageShakeInThirdPerson;
	public externalShakeInfo thirdPersonDamageShake;
	public bool useDamageShakeInFirstPerson;
	public externalShakeInfo firstPersonDamageShake;

	public externalShakeInfoListElement (externalShakeInfoListElement newExternalShake)
	{
		name = newExternalShake.name;
		sameValueBothViews = newExternalShake.sameValueBothViews;
		useDamageShakeInThirdPerson = newExternalShake.useDamageShakeInThirdPerson;
		thirdPersonDamageShake = newExternalShake.thirdPersonDamageShake;
		useDamageShakeInFirstPerson = newExternalShake.useDamageShakeInFirstPerson;
		firstPersonDamageShake = newExternalShake.firstPersonDamageShake;
	}
}
