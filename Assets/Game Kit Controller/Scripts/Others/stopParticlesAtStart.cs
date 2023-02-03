using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stopParticlesAtStart : MonoBehaviour
{
	public bool disableParticlesAtStartEnabled = true;
	public bool checkIfParticlesComponentAssigned = true;

	public bool clearParticlesEnabled = true;

	public ParticleSystem mainParticleSystem;

	void Start ()
	{
		if (disableParticlesAtStartEnabled) {
			if (checkIfParticlesComponentAssigned) {
				if (mainParticleSystem == null) {
					mainParticleSystem = GetComponentInChildren<ParticleSystem> ();
				}
			}

			if (mainParticleSystem != null) {
				mainParticleSystem.Stop ();

				if (clearParticlesEnabled) {
					mainParticleSystem.Clear ();
				}
			}
		}

	}
}
