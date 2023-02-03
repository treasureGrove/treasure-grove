using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class flamethrowerWeapon : MonoBehaviour
{
	[Space]
	[Header ("Main Settings")]
	[Space]

	public bool weaponEnabled;

	public float useEnergyRate;
	public int amountEnergyUsed;

	[Space]
	[Header ("Sound Settings")]
	[Space]

	public AudioClip soundEffect;
	public float playSoundRate;
	public float minDelayToPlaySound;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool reloading;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnWeaponStateChange;
	public UnityEvent eventOnWeaponEnabled;
	public UnityEvent eventOnWeaponDisabled;

	[Space]
	[Header ("Components")]
	[Space]

	public bool configuredOnWeapon = true;
	public playerWeaponSystem weaponManager;

	public ParticleSystem mainParticleSystem;

	public AudioSource mainAudioSource;

	//	float lastTimeWeaponActive;
	//	bool weaponActivePreviously;

	float lastTimeUsed;
	float lastTimeSoundPlayed;

	bool initialSoundWaitChecked;

	void Update ()
	{
		if (configuredOnWeapon) {
			if (reloading) {
				if (weaponManager.remainAmmoInClip () && weaponManager.carryingWeapon () && !weaponManager.isWeaponReloading ()) {
					reloading = false;
				} else {
					return;
				}
			}

			if (!weaponEnabled) {
				return;
			}

			if (Time.time > lastTimeUsed + useEnergyRate) {
				if (weaponManager.remainAmmoInClip () && !weaponManager.isWeaponReloading ()) {
					lastTimeUsed = Time.time;

					weaponManager.useAmmo (amountEnergyUsed);

					weaponManager.checkToUpdateInventoryWeaponAmmoTextFromWeaponSystem ();
				}

				if (!weaponManager.remainAmmoInClip () || weaponManager.isWeaponReloading ()) {
					setWeaponState (false);

					reloading = true;

					return;
				}				
			}
		}

		if (weaponEnabled) {
			if (Time.time > lastTimeSoundPlayed + playSoundRate) {
				if (initialSoundWaitChecked || Time.time > lastTimeSoundPlayed + minDelayToPlaySound) {
					lastTimeSoundPlayed = Time.time;

					playWeaponSoundEffect ();

					initialSoundWaitChecked = true;
				}
			}
		}
	}

	public void enableWeapon ()
	{
		setWeaponState (true);
	}

	public void disableWeapon ()
	{
		setWeaponState (false);
	}

	public void setWeaponState (bool state)
	{
		if (reloading) {
			weaponEnabled = false;

			return;
		}

		initializeComponents ();

		if (weaponEnabled == state) {
			return;
		}

		weaponEnabled = state;
//
//		if (weaponActivePreviously != weaponEnabled) {
//			if (weaponEnabled) {
//				lastTimeWeaponActive = Time.time;
//			}
//
//			weaponActivePreviously = weaponEnabled;
//		}

		if (mainParticleSystem != null) {
			if (weaponEnabled) {
				mainParticleSystem.Play ();
			} else {
				mainParticleSystem.Stop ();
			}
		}

		checkEventsOnStateChange (weaponEnabled);

		initialSoundWaitChecked = false;

		lastTimeSoundPlayed = 0;

		if (!weaponEnabled) {
			stopWeaponSoundEffect ();
		}
	}

	void playWeaponSoundEffect ()
	{
		if (mainAudioSource != null) {
			mainAudioSource.PlayOneShot (soundEffect);
		}
	}

	void stopWeaponSoundEffect ()
	{
		if (mainAudioSource != null) {
			mainAudioSource.Stop ();
		}
	}

	void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnWeaponStateChange) {
			if (state) {
				eventOnWeaponEnabled.Invoke ();
			} else {
				eventOnWeaponDisabled.Invoke ();
			}
		}
	}

	bool componentsInitialized;

	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}

		if (weaponManager != null) {

		}

		componentsInitialized = true;
	}
}