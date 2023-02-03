using UnityEngine;
using System.Collections;

public class playSoundOnCollision : MonoBehaviour
{
	public AudioSource mainAudioSource;
	public bool useMinSpeedForSound;
	public float minSpeedForSound;

	void OnCollisionEnter (Collision col)
	{
		if (mainAudioSource == null) {
			mainAudioSource = GetComponent<AudioSource> ();
		}

		if (useMinSpeedForSound) {
			if (col.relativeVelocity.magnitude > minSpeedForSound) {
				playSound ();
			}
		} else {
			playSound ();
		}
	}

	public void playSound ()
	{
		if (mainAudioSource != null) {
			mainAudioSource.Play ();
		}
	}
}