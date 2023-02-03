using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSoundOnTriggerEnter : MonoBehaviour
{
	public AudioClip soundToPlay;
	public List<string> tagListToCheck = new List<string> ();
	public bool playingSound;
	public AudioSource mainAudioSource;
	public bool useEventTriggerSystem;

	void Start ()
	{
		
	}

	void Update ()
	{
		if (playingSound && !mainAudioSource.isPlaying) {
			playingSound = false;
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (useEventTriggerSystem) {
			return;
		}

		if (playingSound) {
			return;
		}

		if (tagListToCheck.Contains (col.gameObject.tag)) {
			playSound ();
		}
	}

	public void playSound ()
	{
		if (playingSound) {
			return;
		}

		mainAudioSource.PlayOneShot (soundToPlay);
		playingSound = true;
	}
}
