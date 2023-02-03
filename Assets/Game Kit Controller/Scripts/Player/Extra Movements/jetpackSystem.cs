using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class jetpackSystem : externalControllerBehavior
{
	[Header ("Main Setting")]
	[Space]

	public bool jetpackEnabled;

	public float jetpackForce;
	public float jetpackAirSpeed;
	public float jetpackAirControl;

	public bool turboEnabled = true;
	public float jetpackTurboHorizontalSpeed = 5;
	public float jetpackTurboVerticalSpeed = 5;

	public bool useForceMode;

	public ForceMode forceMode;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool disableJetpackMeshWhenNotEquipped = true;

	public float jetpackHorizontalSpeedOnAimMultiplier = 0.5f;
	public float jetpackVerticalSpeedOnAimMultiplier = 0.5f;

	public string shakeCameraStateName = "Use Jetpack Turbo";

	public bool usedByAI;

	[Space]
	[Header ("Fuel Settings")]
	[Space]

	public float jetpackFuelAmount;
	public float jetpackFuelRate;
	public float regenerativeSpeed;
	public float timeToRegenerate;

	[Space]
	[Header ("Animation Settings")]
	[Space]

	public bool useJetpackAnimation = true;

	public string jetpackModeName = "Jetpack Mode";

	public int regularAirID = -1;
	public int jetpackID = 2;

	public bool useIKJetpack;

	public string animationName;

	[Space]
	[Header ("Particles Settings")]
	[Space]

	public List<ParticleSystem> thrustsParticles = new List<ParticleSystem> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool jetPackEquiped;
	public bool usingJetpack;
	public bool turboActive;

	public bool jetpackModePaused;

	[Space]
	[Header ("Event Setting")]
	[Space]

	public UnityEvent eventOnStartUsingJetpack;
	public UnityEvent eventOnStopUsingJetpack;

	public UnityEvent eventOnStateEnabled;
	public UnityEvent eventOnStateDisabled;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject jetpack;
	public GameObject jetpackHUDInfo;
	public Slider jetpackSlider;
	public Text fuelAmountText;

	public playerController mainPlayerController;
	public playerCamera mainPlayerCamera;
	public Animation jetPackAnimation;
	public IKSystem IKManager;
	public Transform playerTransform;

	public Rigidbody mainRigidbody;


	bool hudEnabled;
	float lastTimeUsed;

	Vector3 airMove;

	float currentHorizontalAimSpeedMultipler;
	float currentVerticalAimSpeedMultipler;

	Vector3 totalForce;

	bool playerIsAiming;

	bool turboActivePreviously;

	bool originalJetpackEnabled;

	externalControllerBehavior previousExternalControllerBehavior;


	void Start ()
	{
		if (usedByAI) {
			return;
		}
			
		if (jetpack == null) {
			jetpackEnabled = false;

			return;
		}

		changeThrustsParticlesState (false);

		if (jetpackFuelAmount > 0) {
			jetpackSlider.maxValue = jetpackFuelAmount;
			jetpackSlider.value = jetpackFuelAmount;

			hudEnabled = true;

			updateJetpackUIInfo ();
		}
			
		if (!jetpackEnabled || disableJetpackMeshWhenNotEquipped) {
			jetpack.SetActive (false);
		}

		originalJetpackEnabled = jetpackEnabled;
	}

	public override void updateControllerBehavior ()
	{
		if (usedByAI) {
			return;
		}

		if (jetPackEquiped) {
			//if the player is using the jetpack
			if (usingJetpack) {

				currentHorizontalAimSpeedMultipler = 1;
				currentVerticalAimSpeedMultipler = 1;

				playerIsAiming = mainPlayerController.isPlayerAiming ();

				if (!mainPlayerController.isPlayerOnFirstPerson () && playerIsAiming) {
					currentHorizontalAimSpeedMultipler = jetpackHorizontalSpeedOnAimMultiplier;
					currentVerticalAimSpeedMultipler = jetpackVerticalSpeedOnAimMultiplier;
				}

				airMove = mainPlayerController.getMoveInputDirection () * (jetpackAirSpeed * currentHorizontalAimSpeedMultipler);

				if (turboActive) {
					airMove *= jetpackTurboHorizontalSpeed;
				}

				airMove += playerTransform.InverseTransformDirection (mainPlayerController.currentVelocity).y * playerTransform.up;

				mainPlayerController.currentVelocity = Vector3.Lerp (mainPlayerController.currentVelocity, airMove, Time.fixedDeltaTime * jetpackAirControl);	

				totalForce = playerTransform.up * (-mainPlayerController.getGravityForce () * mainRigidbody.mass * jetpackForce * currentVerticalAimSpeedMultipler);

				if (turboActive) {
					totalForce *= jetpackTurboVerticalSpeed;
				}

				if (useForceMode) {
					mainRigidbody.AddForce (totalForce, forceMode);
				} else {
					mainRigidbody.AddForce (totalForce);
				}
					
				if (playerIsAiming) {
					if (turboActive) {
						if (!turboActivePreviously) {
							turboActivePreviously = true;

							enableOrDisableTurbo (false);
						}
					}
				} else {
					if (turboActivePreviously) {
						enableOrDisableTurbo (true);

						turboActivePreviously = false;
					}
				}
			}

			if (mainPlayerController.isPlayerOnFFOrZeroGravityModeOn ()) {
				enableOrDisableJetpack (false);
			}

			if (usingJetpack) {
				mainPlayerController.setPlayerOnGroundState (false);

				if (hudEnabled) {
					jetpackFuelAmount -= Time.deltaTime * jetpackFuelRate;

					updateJetpackUIInfo ();

					jetpackSlider.value = jetpackFuelAmount;
				}

				if (jetpackFuelAmount <= 0) {
					startOrStopJetpack (false);
				}

			} else {	
				if (regenerativeSpeed > 0 && lastTimeUsed != 0) {
					
					if (Time.time > lastTimeUsed + timeToRegenerate) {

						jetpackFuelAmount += regenerativeSpeed * Time.deltaTime;

						if (jetpackFuelAmount >= jetpackSlider.maxValue) {
							jetpackFuelAmount = jetpackSlider.maxValue;
							lastTimeUsed = 0;
						}

						updateJetpackUIInfo ();

						jetpackSlider.value = jetpackFuelAmount;
					}
				}
			} 
		}
	}

	void updateJetpackUIInfo ()
	{
		//jetpackSlider.maxValue.ToString ("0") + " / " + 
		fuelAmountText.text = jetpackFuelAmount.ToString ("0");
	}

	public bool canUseJetpack ()
	{
		bool value = false;

		if (jetpackFuelAmount > 0) {
			value = true;
		}

		if (jetpackModePaused) {
			value = false;
		}

		return value;
	}

	public void startOrStopJetpack (bool state)
	{
		if (usingJetpack != state) {
			usingJetpack = state;

			if (useJetpackAnimation) {
				if (usingJetpack) {
					jetPackAnimation [animationName].speed = 1; 
					jetPackAnimation.Play (animationName);
				} else {
					jetPackAnimation [animationName].speed = -1; 
					jetPackAnimation [animationName].time = jetPackAnimation [animationName].length;
					jetPackAnimation.Play (animationName);
				}
			}

			if (usingJetpack) {
				externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (currentExternalControllerBehavior != this) {
					if (previousExternalControllerBehavior == null && currentExternalControllerBehavior != null) {
						previousExternalControllerBehavior = currentExternalControllerBehavior;
					}

					mainPlayerController.setExternalControllerBehavior (this);
				}
			}
		
			mainPlayerController.setUsingJetpackState (usingJetpack);

			if (!usingJetpack) {
				mainPlayerController.setLastTimeFalling ();

				lastTimeUsed = Time.time;
			}

			changeThrustsParticlesState (usingJetpack);

			if (useIKJetpack) {
				IKManager.setIKBodyState (usingJetpack, jetpackModeName);
			}

			if (usingJetpack) {
				mainPlayerController.setCurrentAirIDValue (jetpackID);
			} else {
				mainPlayerController.setCurrentAirIDValue (regularAirID);
			}

			if (usingJetpack) {
				eventOnStartUsingJetpack.Invoke ();
			} else {
				eventOnStopUsingJetpack.Invoke ();
			}

			if (!usingJetpack) {
				if (turboActive) {
					enableOrDisableTurbo (false);
				}
			}

			turboActivePreviously = false;
		}
	}

	public void changeThrustsParticlesState (bool state)
	{
		for (int i = 0; i < thrustsParticles.Count; i++) {
			if (state) {
				if (!thrustsParticles [i].isPlaying) {
					thrustsParticles [i].gameObject.SetActive (true);
					thrustsParticles [i].Play ();

					var boostingParticlesMain = thrustsParticles [i].main;
					boostingParticlesMain.loop = true;
				}
			} else {
				if (thrustsParticles [i].isPlaying) {
					var boostingParticlesMain = thrustsParticles [i].main;
					boostingParticlesMain.loop = false;
				}
			}
		}
	}

	public void enableOrDisableJetpack (bool state)
	{
		if (!jetpackEnabled && state) {
			return;
		}

		if (jetPackEquiped == state) {
			return;
		}

		if (mainPlayerController.isUseExternalControllerBehaviorPaused ()) {
			return;
		}

		if (jetpackModePaused && state) {
			return;
		}

		if (state) {
			externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

			if (currentExternalControllerBehavior != null && currentExternalControllerBehavior != this) {
				if (canBeActivatedIfOthersBehaviorsActive && checkIfCanEnableBehavior (currentExternalControllerBehavior.behaviorName)) {
					currentExternalControllerBehavior.disableExternalControllerState ();
				} else {
					return;
				}
			}
		}

		bool jetpackEquipedPreviously = jetPackEquiped;

		jetPackEquiped = state;

		mainPlayerController.equipJetpack (state);

		setBehaviorCurrentlyActiveState (state);

		setCurrentPlayerActionSystemCustomActionCategoryID ();

		if (jetPackEquiped) {
			if (previousExternalControllerBehavior == null) {
				previousExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (previousExternalControllerBehavior == this) {
					previousExternalControllerBehavior = null;
				}
			}

			mainPlayerController.setExternalControllerBehavior (this);
		} else {
			if (jetpackEquipedPreviously) {
				externalControllerBehavior currentExternalControllerBehavior = mainPlayerController.getCurrentExternalControllerBehavior ();

				if (currentExternalControllerBehavior == null || currentExternalControllerBehavior == this) {
					mainPlayerController.setExternalControllerBehavior (null);
				}
			}
		}

		if (jetPackEquiped) {
			if (hudEnabled) {
				jetpackHUDInfo.SetActive (jetPackEquiped);
			}

			eventOnStateEnabled.Invoke ();
		} else {
			jetpackHUDInfo.SetActive (jetPackEquiped);

			eventOnStateDisabled.Invoke ();

			if (usingJetpack) {
				if (useJetpackAnimation) {
					jetPackAnimation [animationName].speed = -1; 
					jetPackAnimation [animationName].time = 0;

					jetPackAnimation.Rewind ();
				}

				startOrStopJetpack (false);
			}
		}

		if (disableJetpackMeshWhenNotEquipped) {
			if (jetpack != null) {
				jetpack.SetActive (jetPackEquiped);
			}
		}

		if (!state) {
			if (previousExternalControllerBehavior != null) {

				if (previousExternalControllerBehavior != this) {
					previousExternalControllerBehavior.checkIfResumeExternalControllerState ();
				}
			}
		}
	}

	public void getJetpackFuel (float amount)
	{
		jetpackFuelAmount += amount;

		if (jetpackFuelAmount > jetpackSlider.maxValue) {
			jetpackFuelAmount = jetpackSlider.maxValue;
		}
			
		updateJetpackUIInfo ();

		jetpackSlider.value = jetpackFuelAmount;
	}

	public float getJetpackFuelAmountToPick ()
	{
		return jetpackSlider.maxValue - jetpackFuelAmount;
	}

	public void enableOrDisableJetPackMesh (bool state)
	{
		if (!jetpackEnabled && state) {
			return;
		}

		bool jetpackMeshState = state;

		if (disableJetpackMeshWhenNotEquipped && !jetPackEquiped) {
			jetpackMeshState = false;
		}

		if (jetpack != null) {
			jetpack.SetActive (jetpackMeshState);
		}
	}

	public void removeJetpackMesh ()
	{
		if (jetpack != null) {
			DestroyImmediate (jetpack);
		}
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	public void enableOrDisableTurbo (bool state)
	{
		turboActive = state;

		mainPlayerCamera.changeCameraFov (turboActive);

		//when the player accelerates his movement in the air, the camera shakes
		if (turboActive) {
			mainPlayerCamera.setShakeCameraState (true, shakeCameraStateName);
		} else {
			mainPlayerCamera.setShakeCameraState (false, "");

			mainPlayerCamera.stopShakeCamera ();
		}

		if (turboActive) {
			mainPlayerController.setCurrentAirSpeedValue (2);
		} else {
			mainPlayerController.setCurrentAirSpeedValue (1);
		}
	}

	public void setJetpackEnabledState (bool state)
	{
		if (jetPackEquiped) {
			enableOrDisableJetpack (false);
		}

		jetpackEnabled = state;
	}

	public void setOriginalJetpackEnabledState ()
	{
		setJetpackEnabledState (originalJetpackEnabled);
	}

	public void setJetpackModePausedState (bool state)
	{
		if (state) {
			if (usingJetpack) {
				startOrStopJetpack (false);
			}
		}

		jetpackModePaused = state;
	}

	//Input Elements
	public void inputStartOrStopJetpack (bool state)
	{
		if (jetPackEquiped && !mainPlayerController.driving && canUseJetpack ()) {
			startOrStopJetpack (state);
		}
	}

	public void inputChangeTurboState (bool state)
	{
		if (jetPackEquiped && usingJetpack && turboEnabled) {
			if (state) {
				if (playerIsAiming) {
					turboActivePreviously = true;

					return;
				}
			} else {
				turboActivePreviously = false;
			}

			enableOrDisableTurbo (state);
		}
	}

	public override void disableExternalControllerState ()
	{
		enableOrDisableJetpack (false);
	}

	public override void setCurrentPlayerActionSystemCustomActionCategoryID ()
	{
		if (behaviorCurrentlyActive) {
			if (customActionCategoryID > -1) {
				mainPlayerController.setCurrentCustomActionCategoryID (customActionCategoryID);
			}
		} else {
			if (regularActionCategoryID > -1) {
				mainPlayerController.setCurrentCustomActionCategoryID (regularActionCategoryID);
			}
		}
	}
}