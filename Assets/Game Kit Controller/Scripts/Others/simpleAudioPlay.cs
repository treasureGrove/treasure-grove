using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleAudioPlay : MonoBehaviour
{
	public AudioSource mainAudioSource;

	void Start ()
	{
		mainAudioSource.Play ();
	}
}
