using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class footStep : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool footStepEnabled = true;
	public LayerMask layer;
	public footType footSide;

	public float waitTimeBetweenWalkSteps = 0.4f;
	public float waitTimeBetwennRunSteps = 0.1f;

	[Space]
	[Header ("Component")]
	[Space]

	public footStepManager mainFootStepManager;
	public AudioSource audioSource;
	public playerController playerControllerManager;

	bool playerControllerManagerAssigned;

	bool touching;
	GameObject currentSurface;
	Vector2 volumeRange = new Vector2 (0.8f, 1.2f);

	float lastTimeStep;

	bool running;

	AudioClip soundEffect;

	public enum footType
	{
		left,
		right,
		center
	}

	Coroutine footStepStateDelayCoroutine;

	//check when the trigger hits a surface, and play one shoot of the audio clip according to the layer of the hitted collider
	void OnTriggerEnter (Collider col)
	{
		if (!footStepEnabled) {
			return;
		}

		if (!playerControllerManagerAssigned) {
			if (playerControllerManager != null) {
				playerControllerManagerAssigned = true;
			}
		}

		if (playerControllerManagerAssigned) {
			running = playerControllerManager.isPlayerRunning ();
		}

		//compare if the layer of the hitted object is not in the layer configured in the inspector
		if (mainFootStepManager.stepsEnabled && (1 << col.gameObject.layer & layer.value) == 1 << col.gameObject.layer &&
			
		    ((!running && Time.time > waitTimeBetweenWalkSteps + lastTimeStep) ||
		    (running && Time.time > waitTimeBetweenWalkSteps + waitTimeBetwennRunSteps))) {

			lastTimeStep = Time.time;

			touching = true;

			//get the gameObject touched by the foot trigger
			currentSurface = col.gameObject;

			//check the footstep frequency
			if (touching) {
				//get the audio clip according to the type of surface, mesh or terrain
				soundEffect = mainFootStepManager.getSound (transform.position, currentSurface, footSide);

				if (mainFootStepManager.soundsEnabled) {
					if (soundEffect != null) {
						playSound (soundEffect);
					}
				}
			}
		}
	}

	//play one shot of the audio
	void playSound (AudioClip clip)
	{
		audioSource.PlayOneShot (clip, Random.Range (volumeRange.x, volumeRange.y));
	}

	public void setStepVolumeRange (Vector2 newVolumeRange)
	{
		volumeRange = newVolumeRange;
	}

	public void setOriginalStepVolume ()
	{
		volumeRange = mainFootStepManager.feetVolumeRangeClamps;
	}

	public void enableOrDisableFootStep (bool state)
	{
		footStepEnabled = state;
	}

	public void setFooStepStateWithDelay (bool state, float delayAmount)
	{
		if (footStepStateDelayCoroutine != null) {
			StopCoroutine (footStepStateDelayCoroutine);
		}

		footStepStateDelayCoroutine = StartCoroutine (setFooStepStateWithDelayCoroutine (state, delayAmount));
	}

	IEnumerator setFooStepStateWithDelayCoroutine (bool state, float delayAmount)
	{
		yield return new WaitForSeconds (delayAmount);

		enableOrDisableFootStep (state);
	}
}