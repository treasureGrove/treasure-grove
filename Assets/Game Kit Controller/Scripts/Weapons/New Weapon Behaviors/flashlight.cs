using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class flashlight : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool flashlightEnabled = true;

	public bool infiniteEnergy;
	public float useEnergyRate;
	public int amountEnergyUsed;

	public bool useHighIntentity;
	public float highIntensityAmount;

	public float lightRotationSpeed = 10;
	public bool usedThroughWeaponSystem = true;

	[Space]
	[Header ("Sound Settings")]
	[Space]

	public bool useSound;
	public AudioClip turnOnSound;
	public AudioClip turnOffSound;

	[Space]
	[Header ("UI Settings")]
	[Space]

	public bool useFlashlightIndicatorPanel;
	public Slider mainSlider;
	public GameObject flahslightIndicatorPanel;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool isActivated;

	public bool reloading;

	public bool usingFlashlight;

	public bool flashLightInputPausedState;

	[Space]
	[Header ("Components")]
	[Space]

	public playerWeaponsManager weaponsManager;
	public playerWeaponSystem weaponManager;
	public GameObject mainLight;
	public Light mainFlashlight;
	public AudioSource mainAudioSource;
	public GameObject flashlightMeshes;

	bool highIntensityActivated;

	float lastTimeUsed;
	Transform mainCameraTransform;
	float originalIntensity;
	Quaternion targetRotation;

	bool UIElementsLocated;

	void Start ()
	{
		if (mainAudioSource == null) {
			mainAudioSource = GetComponent<AudioSource> ();
		}

		if (weaponManager == null) {
			weaponManager = GetComponent<playerWeaponSystem> ();
		}

		if (mainFlashlight == null) {
			mainFlashlight = mainLight.GetComponent<Light> ();
		}

		originalIntensity = mainFlashlight.intensity;

		if (!flashlightEnabled) {
			enableOrDisableFlashlightMeshes (false);
		}
	}

	void Update ()
	{
		if (usedThroughWeaponSystem) {
			if (isActivated) {
				if (mainCameraTransform != null) {
					if (!weaponManager.weaponIsMoving () && (weaponManager.aimingInThirdPerson || weaponManager.carryingWeaponInFirstPerson)
					    && !weaponsManager.isEditinWeaponAttachments ()) {
						targetRotation = Quaternion.LookRotation (mainCameraTransform.forward);
						mainLight.transform.rotation = Quaternion.Slerp (mainLight.transform.rotation, targetRotation, Time.deltaTime * lightRotationSpeed);

						//mainLight.transform.rotation = targetRotation;
					} else {
						targetRotation = Quaternion.identity;
						mainLight.transform.localRotation = Quaternion.Slerp (mainLight.transform.localRotation, targetRotation, Time.deltaTime * lightRotationSpeed);

						//mainLight.transform.localRotation = targetRotation;
					}
				} else {
					mainCameraTransform = weaponManager.getMainCameraTransform ();
				}

				if (infiniteEnergy) {
					return;
				}

				if (Time.time > lastTimeUsed + useEnergyRate) {
					if (weaponManager.remainAmmoInClip () && !weaponManager.isWeaponReloading ()) {
						lastTimeUsed = Time.time;
						weaponManager.useAmmo (amountEnergyUsed);

						weaponManager.checkToUpdateInventoryWeaponAmmoTextByWeaponNumberKey ();
					}

					if (!weaponManager.remainAmmoInClip () || weaponManager.isWeaponReloading ()) {
						setFlashlightState (false);

						reloading = true;
					}
				}
			} else {
				if (reloading) {
					if (weaponManager.remainAmmoInClip () && weaponManager.carryingWeapon () && !weaponManager.isWeaponReloading ()) {
						setFlashlightState (true);

						reloading = false;
					}
				}
			}
		
			if (usingFlashlight) {
				if (useFlashlightIndicatorPanel) {
					if (!infiniteEnergy) {
						if (UIElementsLocated) {
							mainSlider.value = weaponManager.getWeaponClipSize ();
						}
					}
				}
			}
		}
	}

	public bool checkIfEnoughBattery ()
	{
		if (infiniteEnergy) {
			return true;
		}

		if (usedThroughWeaponSystem) {
			if (!weaponManager.remainAmmoInClip ()) {
				return false;
			}
		}

		return true;
	}

	public void changeFlashLightState ()
	{
		if (flashLightInputPausedState) {
			return;
		}

		setFlashlightState (!isActivated);
	}

	public void setFlashlightState (bool state)
	{
		if (state) {
			if (!checkIfEnoughBattery ()) {
				return;
			}

			if (!flashlightEnabled) {
				return;
			}
		}

		initializeComponents ();

		isActivated = state;

		playSound (isActivated);

		if (mainLight.activeSelf != isActivated) {
			mainLight.SetActive (isActivated);
		}
	}

	public void turnOn ()
	{
		if (!checkIfEnoughBattery ()) {
			return;
		}

		if (!flashlightEnabled) {
			return;
		}

		isActivated = true;

		playSound (isActivated);
	}

	public void turnOff ()
	{
		isActivated = false;

		playSound (isActivated);
	}

	public void playSound (bool state)
	{
		if (useSound) {
			GKC_Utils.checkAudioSourcePitch (mainAudioSource);

			if (state) {
				mainAudioSource.PlayOneShot (turnOnSound);
			} else {
				mainAudioSource.PlayOneShot (turnOffSound);
			}
		}
	}

	public void changeLightIntensity (bool state)
	{
		if (useHighIntentity) {
			highIntensityActivated = state;

			if (highIntensityActivated) {
				mainFlashlight.intensity = highIntensityAmount;
			} else {
				mainFlashlight.intensity = originalIntensity;
			}
		}
	}

	public void setHighIntensity ()
	{
		changeLightIntensity (true);
	}

	public void setOriginalIntensity ()
	{
		changeLightIntensity (false);
	}

	public void enableOrDisableFlashlightMeshes (bool state)
	{
		if (flashlightMeshes != null) {
			if (state) {
				if (!flashlightEnabled) {
					return;
				}
			}

			if (flashlightMeshes.activeSelf != state) {
				flashlightMeshes.SetActive (state);
			}
		}
	}

	public void setFlashLightInputPausedState (bool state)
	{
		flashLightInputPausedState = state;
	}

	public void enableOrDisableFlashlightIndicator (bool state)
	{
		usingFlashlight = state;

		if (useFlashlightIndicatorPanel) {
			if (flahslightIndicatorPanel != null) {
				if (flahslightIndicatorPanel.activeSelf != state) {
					flahslightIndicatorPanel.SetActive (state);
				}
			}

			if (mainSlider != null) {
				if (usedThroughWeaponSystem) {
					mainSlider.maxValue = weaponManager.getMagazineSize ();
				}

				UIElementsLocated = true;
			}
		}
	}

	bool componentsInitialized;

	void initializeComponents ()
	{
		if (componentsInitialized) {
			return;
		}

		if (usedThroughWeaponSystem) {
			if (weaponsManager == null) {
				weaponsManager = weaponManager.getPlayerWeaponsManger ();
			}
		}
	
		componentsInitialized = true;
	}
}
