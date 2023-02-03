using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioClipBip : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float playTime;
	public float playRate;
	public float increasePitchSpeed;
	public float increasePlayRateSpeed;

	public bool playBipAtStart = true;

	public bool playBipOnEnable;

	public AudioClip soundClip;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public float lastTimePlayed;
	public float totalTimePlayed;
	public bool audioPlayed;

	public bool bipActivated;
	public float originalPlayRate;

	AudioSource mainAudioSource;

	bool initialized;

	void Start ()
	{
		if (playBipAtStart) {
			intializeBip ();
		}
	}

	void OnEnable ()
	{
		if (playBipOnEnable) {
			intializeBip ();
		}
	}

	void intializeBip ()
	{
		if (mainAudioSource == null) {
			mainAudioSource = GetComponent<AudioSource> ();
		}

		if (originalPlayRate == 0) {
			originalPlayRate = playRate;
		}

		totalTimePlayed = Time.time;

		resetBip ();

		if (showDebugPrint) {
			print ("Initialize bip");
		}

		initialized = true;
	}

	void FixedUpdate ()
	{
		if (!audioPlayed && (bipActivated || playBipAtStart || playBipOnEnable)) {
			if (Time.time > lastTimePlayed + playRate) {
				
				mainAudioSource.pitch += increasePitchSpeed;
				mainAudioSource.PlayOneShot (soundClip);

				lastTimePlayed = Time.time;
				playRate -= increasePlayRateSpeed;

				if (playRate <= 0) {
					playRate = 0.1f;
				}

				if (Time.time > (totalTimePlayed + playTime)) {
					audioPlayed = true;
				}
			}
		}
	}

	public void increasePlayTime (float extraTime)
	{
		if (!initialized || !playBipOnEnable) {
			intializeBip ();
		}

		totalTimePlayed = Time.time;

		playTime = extraTime;

		bipActivated = true;

		if (showDebugPrint) {
			print ("Activate bip");
		}
	}

	public void disableBip ()
	{
		bipActivated = false;

		mainAudioSource.pitch = 1;

		lastTimePlayed = 0;

		playRate = originalPlayRate;
	}

	void resetBip ()
	{
		lastTimePlayed = 0;

		playRate = originalPlayRate;

		totalTimePlayed = Time.time;

		audioPlayed = false;
		bipActivated = false;

		if (mainAudioSource != null) {
			mainAudioSource.pitch = 1;
		}
	}
}
