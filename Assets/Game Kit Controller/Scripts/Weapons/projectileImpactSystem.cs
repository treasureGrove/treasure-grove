using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileImpactSystem : MonoBehaviour
{
	public AudioSource mainAudioSource;
	public destroyGameObject mainDestroyGameObject;

	public void activateImpactElements (Vector3 newPosition, AudioClip impactSoundEffect)
	{
//		print ("----Activate impact elements " + gameObject.activeSelf + " " + mainDestroyGameObject.destroyCoroutineActive);

		transform.position = newPosition;
		mainAudioSource.clip = impactSoundEffect;

//		print (impactSoundEffect.length);

		mainDestroyGameObject.setTimer (impactSoundEffect.length + 0.2f);

		if (gameObject.activeSelf) {

			mainDestroyGameObject.checkToDestroyObjectInTime (false);
		} else {
			gameObject.SetActive (true);
		}

		GKC_Utils.checkAudioSourcePitch (mainAudioSource);
		mainAudioSource.Play ();

		decalManager.setImpactSoundParent (transform);
	}

	public void changeDestroyForSetActiveFunction (bool state)
	{
		mainDestroyGameObject.changeDestroyForSetActiveFunction (state);
	}

	public void setSendObjectToPoolSystemToDisableState (bool state)
	{
		mainDestroyGameObject.setSendObjectToPoolSystemToDisableState (state);
	}
}
