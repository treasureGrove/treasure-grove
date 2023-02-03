using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

using UnityEngine.Events;

public class vehicleHUDManager : healthManagement
{
	public string vehicleName;

	public float healthAmount;
	public float maxHealthAmount = 100;

	public float boostAmount;
	public float maxBoostAmount;

	public bool vehicleUseFuel;
	public float fuelAmount;
	public float maxFuelAmount = 100;

	public bool startWithGasTankEmpty;

	public bool destroyed;

	public bool regenerateHealth;
	public bool constantHealthRegenerate;
	public float regenerateHealthSpeed;
	public float regenerateHealthTime;
	public float regenerateHealthAmount;

	public bool regenerateBoost;
	public bool constantBoostRegenerate;
	public float regenerateBoostSpeed;
	public float regenerateBoostTime;
	public float regenerateBoostAmount;

	public bool vehicleUseBoost = true;

	public bool infiniteBoost;
	public float boostUseRate;

	public bool infiniteFuel;
	public float fuelUseRate;

	public bool regenerateFuel;
	public bool constantFuelRegenerate;
	public float regenerateFuelSpeed;
	public float regenerateFuelTime;
	public float regenerateFuelAmount;

	public GameObject gasTankGameObject;

	public bool invincible;
	public AudioClip destroyedSound;
	public AudioSource destroyedSource;
	public LayerMask layer;
	public LayerMask layerForPassengers;

	public bool useDamageParticles = true;
	public GameObject damageParticles;

	public bool useDestroyedParticles = true;
	public GameObject destroyedParticles;

	public float healthPercentageDamageParticles;


	public Transform placeToShoot;

	public bool removePiecesWhenDestroyed = true;

	public bool fadeVehiclePiecesOnDestroyed = true;
	public float timeToFadePieces = 3;

	public Shader destroyedMeshShader;

	public advancedSettingsClass advancedSettings = new advancedSettingsClass ();

	public bool damageObjectsOnCollision = true;
	[Range (1, 100)] public float damageMultiplierOnCollision = 1;
	public float minVehicleVelocityToDamage = 20;
	public float minCollisionVelocityToDamage = 20;
	public bool ignoreShieldOnCollision = true;

	public int damageTypeID = -1;

	public bool useRemoteEventOnObjectsFound;
	public List<string> remoteEventNameList = new List<string> ();

	public bool useWeakSpots;
	public Slider vehicleHealth;
	public Slider vehicleBoost;
	public Slider vehicleAmmo;
	public Slider vehicleFuel;

	public bool canSetTurnOnState;
	public bool autoTurnOnWhenGetOn;

	public bool launchDriverOnCollision;
	public float minCollisionVelocityToLaunch;
	public Vector3 launchDirectionOffset;
	public float extraCollisionForce = 1.2f;
	public bool ignoreRagdollCollider;
	public bool applyDamageToDriver;
	public bool useCollisionVelocityAsDamage;
	public float collisionDamageAmount;
	public float collisionDamageMultiplier;
	public bool ignoreShieldOnLaunch = true;

	public bool receiveDamageFromCollision;
	public float minVelocityToDamageCollision;
	public bool useCurrentVelocityAsDamage;
	public float defaultDamageCollision;

	public float vehicleRadius;

	public List<audioSourceInfo> audioSourceList = new List<audioSourceInfo> ();

	public bool usedByAI;

	public Transform passengersParent;

	public bool isBeingDriven;
	public bool passengersOnVehicle;

	public bool showVehicleHUD = true;
	public bool showVehicleSpeed = true;

	public bool canUseSelfDestruct;
	public bool canStopSelfDestruction;
	public float selfDestructDelay;
	public bool ejectPassengerOnSelfDestruct;
	public float ejectPassengerFoce;
	public bool getOffPassengersOnSelfDestruct;

	public bool canEjectFromVehicle;

	public bool useHornToCallFriends;
	public bool callOnlyFoundFriends;
	public float radiusToCallFriends;
	public bool useHornEvent;
	public UnityEvent hornEvent;

	public bool canUnlockCursor;
	public bool cursorUnlocked;

	public bool useEventsOnStateChanged;
	public UnityEvent eventOnGetOn;
	public UnityEvent eventOnGetOff;
	public UnityEvent eventOnDestroyed;

	public bool useJumpPlatformEvents = true;
	public eventParameters.eventToCallWithVector3 jumpPlatformEvent;
	public eventParameters.eventToCallWithVector3 jumpPlatformParableEvent;
	public eventParameters.eventToCallWithAmount setNewJumpPowerEvent;
	public UnityEvent setOriginalJumpPowerEvent;

	public UnityEvent passengerGettingOnOffEvent;
	public UnityEvent changeVehicleStateEvent;

	public bool useEventsToCheckIfVehicleUsedByAIOnPassengerEnterExit;
	public UnityEvent eventToCheckIfVehicleUsedByAIOnPassengerEnter;
	public UnityEvent eventToCheckIfVehicleUsedByAIOnPassengerExit;

	public bool useEventToSendPassenger;
	public eventParameters.eventToCallWithGameObject eventToSendPassengerOnGetOn;
	public eventParameters.eventToCallWithGameObject eventToSentPassengerOnGetOff;


	public float vehicleExplosionForce = 500;
	public float vehicleExplosionRadius = 50;
	public ForceMode vehicleExplosionForceMode = ForceMode.Impulse;

	public bool useCustomVehiclePartsToDestroy;
	public List<vehiclePartInfo> customVehiclePartsListToDestroy = new List<vehiclePartInfo> ();

	public bool useEventOnCustomVehicleParts;
	public UnityEvent eventOnCustomVehicleParts;

	public float customVehiclePartsExplosionForce = 500;
	public float customVehiclePartsExplosionRadius = 50;
	public ForceMode customVehiclePartsExplosionForceMode = ForceMode.Impulse;

	List<Material> rendererParts = new List<Material> ();

	bool projectilesListContainsElements;

	List<GameObject> projectilesReceived = new List<GameObject> ();

	public List<ParticleSystem> fireParticles = new List<ParticleSystem> ();

	public List<int> colliderLayerIndexParts = new List<int> ();

	public List<Collider> colliderParts = new List<Collider> ();

	public List<Collider> colliderWithoutMeshesParts = new List<Collider> ();

	public List<Collider> colliderWithoutMeshesPartsToIgnore = new List<Collider> ();

	public List<Collider> colliderWithMeshesParts = new List<Collider> ();

	public List<vehicleDamageReceiver> vehicleDamageReceiverList = new List<vehicleDamageReceiver> ();

	public List<GameObject> vehiclePartsToIgnore = new List<GameObject> ();

	public List<Renderer> vehicleRendererList = new List<Renderer> ();

	public List<vehiclePartInfo> vehiclePartInfoList = new List<vehiclePartInfo> ();

	List<GameObject> previousPassengerGameObjectList = new List<GameObject> ();

	public float debugLaunchCharacterSpeed;
	public Vector3 debugLaunchCharacterPosition;

	float lastDamageTime;
	float auxHealthAmount;

	float lastBoostTime;
	float auxPowerAmount;

	float lastFuelTime;
	float auxFuelAmount;

	bool vehicleDisabled;
	Text ammoAmountText;
	Text weaponNameText;
	Text currentSpeed;

	public IKDrivingSystem IKDrivingManager;
	public vehicleWeaponSystem weaponsManager;
	public Rigidbody mainRigidbody;
	public damageInScreen damageInScreenManager;

	public mapObjectInformation mapInformationManager;
	public vehicleCameraController vehicleCameraManager;
	public useInventoryObject gasTankManager;
	public vehicleGravityControl vehicleGravitymanager;
	public vehicleController mainVehicleController;

	bool mainVehicleControllerFound;
	bool mainVehicleGravityControlFound;

	Coroutine CollidersStateCoroutine;

	Coroutine damageOverTimeCoroutine;

	public string mainDecalManagerName = "Decal Manager";

	decalManager impactDecalManager;
	public string[] impactDecalList;
	public int impactDecalIndex;
	public string impactDecalName;
	public bool useImpactSurface;

	bool selfDestructionActivated;
	Coroutine selfDestructionCoroutine;

	public playerHUDManager.vehicleHUDElements currentHUDElements;
	playerHUDManager currentVehicleHUDInfo;

	audioClipBip selfDestructAudioClipBipManager;

	int lastWeakSpotIndex = -1;

	ContactPoint currentContact;

	public float healthAmountToTakeOnEditor;

	public bool showDebugLogCollisions;

	public string healthStatName = "Health";
	public string energyStatName = "Energy";
	public string fuelStatName = "Fuel";

	bool ignoreSettingPassengerStateActive;
	bool vehicleSetAsChildOfParent;

	//Inspector variables
	public bool showAllSettings;

	public bool showMainSettings;
	public bool showVehicleStats;
	public bool showAdvancedSettings;
	public bool showEventSettings;
	public bool showDebugSettings;
	public bool showOtherSettings;
	public bool showPhysicsSettings;

	public bool showVehicleElements;

	Material currentMaterial;

	bool avoidActivateSelfDestructBipActive;

	GameObject currentPassenger;


	void Start ()
	{
		//get the max amount of health and boost
		if (maxHealthAmount == 0) {
			maxHealthAmount = healthAmount;
		}

		if (maxBoostAmount == 0) {
			maxBoostAmount = boostAmount;
		}

		if (maxFuelAmount == 0) {
			maxFuelAmount = fuelAmount;
		}

		//like in the player, store the max amount of health and boost in two auxiliars varaibles, used by the pick ups to check if the vehicle uses one or more of them
		auxPowerAmount = maxBoostAmount;
		auxHealthAmount = maxHealthAmount;
		auxFuelAmount = maxFuelAmount;

		//get the damage particles of the vehicle
		if (useDamageParticles) {
			if (damageParticles != null) {
				for (int i = 0; i < fireParticles.Count; i++) {
					if (fireParticles [i].gameObject.activeSelf != false) {
						fireParticles [i].gameObject.SetActive (false);
					}
				}
			}
		}

		if (startWithGasTankEmpty) {
			fuelAmount = 0;
		}

		if (gasTankGameObject != null) {
			if (startWithGasTankEmpty) {
				setGasTankState (true);
			} else {
				setGasTankState (false);
			}
		}
			
		if (mainVehicleController != null) {
			mainVehicleControllerFound = true;
		}

		if (vehicleGravitymanager != null) {
			mainVehicleGravityControlFound = true;
		}

		if (usedByAI) {
			activaAIVehicle ();
		}

		if (healthAmount < maxHealthAmount) {
			if (useDamageParticles) {
				//increase the damage particles values
				changeDamageParticlesValue (true, healthAmount / 10);
			}
		}
	}

	void Update ()
	{
		if (mainVehicleController.callVehicleUpdateEachFrame) {
			mainVehicleController.vehicleUpdate ();
		}

		if (isBeingDriven) {
			//get the current values of health and boost of the vehicle, checking if they are regenerative or not
			if (healthAmount < maxHealthAmount) {
				manageBarInfo (healthStatName, regenerateHealth, constantHealthRegenerate, regenerateHealthSpeed, regenerateHealthTime, regenerateHealthAmount, 
					vehicleHealth, healthAmount, maxHealthAmount, lastDamageTime);
			}

			if (boostAmount < maxBoostAmount) {
				manageBarInfo (energyStatName, regenerateBoost, constantBoostRegenerate, regenerateBoostSpeed, regenerateBoostTime, regenerateBoostAmount,
					vehicleBoost, boostAmount, maxBoostAmount, lastBoostTime);
			}

			if (fuelAmount < maxFuelAmount) {
				manageBarInfo (fuelStatName, regenerateFuel, constantFuelRegenerate, regenerateFuelSpeed, regenerateFuelTime, regenerateFuelAmount,
					vehicleFuel, fuelAmount, maxFuelAmount, lastFuelTime);
			}

			IKDrivingManager.applySeatsForces ();
		}

		//clear the list which contains the projectiles received by the vehicle
		if (projectilesListContainsElements && Time.time > lastDamageTime + 0.4f) {
			projectilesReceived.Clear ();

			projectilesListContainsElements = false;
		}

		//if the vehicle is destroyed, when destroyed time reachs 0, all the renderer parts of the vehicle are vanished, setting their alpha color value to 0
		if (destroyed && !vehicleDisabled) {
			if (removePiecesWhenDestroyed) {
				if (timeToFadePieces > 0) {
					timeToFadePieces -= Time.deltaTime;
				}

				if (fadeVehiclePiecesOnDestroyed) {
					if (timeToFadePieces <= 0) {
						int piecesAmountFade = 0;

						int rendererPartsCount = rendererParts.Count;

						for (int i = 0; i < rendererPartsCount; i++) {
							currentMaterial = rendererParts [i];

							Color alpha = currentMaterial.color;
							alpha.a -= Time.deltaTime / 5;
							currentMaterial.color = alpha;

							if (alpha.a <= 0) {
								piecesAmountFade++;
							}
						}

						if (piecesAmountFade == rendererPartsCount) {
							IKDrivingManager.destroyVehicle ();

							vehicleDisabled = true;

							return;
						}
					}
				} else {
					if (timeToFadePieces <= 0) {
						IKDrivingManager.destroyVehicle ();
						vehicleDisabled = true;
					}
				}
			} else {
				vehicleDisabled = true;
			}
		}
	}

	public void setUnlockCursorState (bool state)
	{
		if (vehicleCameraManager.currentState.canUnlockCursor) {
			cursorUnlocked = state;

			if (!IKDrivingManager.setUnlockCursorState (cursorUnlocked)) {
				cursorUnlocked = false;
			}
		}
	}

	public void disableUnlockedCursor ()
	{
		if (canUnlockCursor) {
			cursorUnlocked = false;

			if (IKDrivingManager != null) {
				IKDrivingManager.setUnlockCursorState (cursorUnlocked);
			}
		}
	}

	bool interactionDisabledOnVehicle;

	public void setVehicleInteractionTriggerState (bool state)
	{
		Collider currentVehicleTrigger = getIKDrivingSystem ().getMainCollider ();

		if (currentVehicleTrigger != null) {
			currentVehicleTrigger.enabled = state;

			interactionDisabledOnVehicle = !state;
		}
	}

	public bool isInteractionDisabledOnVehicle ()
	{
		return interactionDisabledOnVehicle;
	}

	public void activateSelfDestructionOnVehicleExternally (float newSelfDestructDelayTime)
	{
		canUseSelfDestruct = false;

		selfDestructDelay = newSelfDestructDelayTime;

		avoidActivateSelfDestructBipActive = true;

		getOffPassengersOnSelfDestruct = false;

		if (!selfDestructionActivated) {
			activateSelfDestruction ();
		}
	}

	public void activateSelfDestruction ()
	{
		if (!selfDestructionActivated) {
			selfDestructionCoroutine = StartCoroutine (selfDestructVehicle ());
		} else {
			if (selfDestructionCoroutine != null) {
				StopCoroutine (selfDestructionCoroutine);

				selfDestructionActivated = false;

				if (selfDestructAudioClipBipManager != null) {
					selfDestructAudioClipBipManager.disableBip ();
				}

				if (ejectPassengerOnSelfDestruct) {

				}
			}
		}
	}

	IEnumerator selfDestructVehicle ()
	{
		selfDestructionActivated = true;

		if (selfDestructDelay > 0) {
			if (!avoidActivateSelfDestructBipActive) {
				selfDestructAudioClipBipManager = GetComponentInChildren<audioClipBip> ();

				if (selfDestructAudioClipBipManager != null) {
					selfDestructAudioClipBipManager.increasePlayTime (selfDestructDelay);
				}
			}
		}

		if (ejectPassengerOnSelfDestruct) {
			ejectFromVehicle ();

		} else {
			if (getOffPassengersOnSelfDestruct) {
				List<GameObject> passengerGameObjectList = IKDrivingManager.getPassengerGameObjectList ();

				for (int i = 0; i < passengerGameObjectList.Count; i++) {
					usingDevicesSystem usingDevicesManager = passengerGameObjectList [i].GetComponent<usingDevicesSystem> ();

					if (usingDevicesManager != null) {
						usingDevicesManager.useDevice ();
					}
				}
			}
		}

		yield return new WaitForSeconds (selfDestructDelay);

		destroyVehicle ();

		yield return null;
	}

	public void callEventOnGetOff ()
	{
		if (useEventsOnStateChanged) {
			if (eventOnGetOff.GetPersistentEventCount () > 0) {
				eventOnGetOff.Invoke ();
			}
		}
	}

	public void callEventOnGetOn ()
	{
		if (useEventsOnStateChanged) {
			if (eventOnGetOn.GetPersistentEventCount () > 0) {
				eventOnGetOn.Invoke ();
			}
		}
	}

	public void callEventOnDestroyed ()
	{
		if (useEventsOnStateChanged) {
			if (eventOnDestroyed.GetPersistentEventCount () > 0) {
				eventOnDestroyed.Invoke ();
			}
		}
	}

	public void ejectFromVehicle ()
	{
		callEventOnGetOff ();

		disableUnlockedCursor ();

		List<GameObject> passengerGameObjectList = IKDrivingManager.getPassengerGameObjectList ();

		for (int i = 0; i < passengerGameObjectList.Count; i++) {
			previousPassengerGameObjectList.Add (passengerGameObjectList [i]);
		}

		checkIgnorPassengersCollidersState ();

		IKDrivingManager.ejectVehiclePassengersOnSelfDestruct (ejectPassengerFoce);

		passengerGettingOnOffEvent.Invoke ();

		passengersOnVehicle = false;

		if (isBeingDriven) {
			isBeingDriven = false;

			changeVehicleStateEvent.Invoke ();

			mainVehicleController.changeVehicleState ();
		}
	}

	public void ejectFromVehicleWithFreeFloatingMode ()
	{
		IKDrivingManager.setActivateFreeFloatingModeOnEjectEnabledState (true);

		ejectFromVehicle ();
	}

	public void changeIgnorePassengerCollidersState (bool state)
	{
		if (previousPassengerGameObjectList.Count > 0) {
			List<Collider> passengerColliderList = new List<Collider> ();

			for (int j = 0; j < previousPassengerGameObjectList.Count; j++) {
				passengerColliderList.Add (previousPassengerGameObjectList [j].GetComponent<Collider> ());
			}

			foreach (Collider currentCollider in colliderParts) {
				//ignore collisions with the player
				for (int j = 0; j < passengerColliderList.Count; j++) {
					Physics.IgnoreCollision (passengerColliderList [j], currentCollider, state);
				}
			}
		}
	}

	public void ignoreCollisionWithVehicleColliderList (List<Collider> colliderList, bool state)
	{
		for (int i = 0; i < colliderWithMeshesParts.Count; i++) {
			for (int j = 0; j < colliderList.Count; j++) {
				Physics.IgnoreCollision (colliderWithMeshesParts [i], colliderList [j], state);
			}
		}
	}

	public void ignoreCollisionWithVehicleColliderList (Collider colliderToIgnore, bool state)
	{
		for (int i = 0; i < colliderWithMeshesParts.Count; i++) {
			Physics.IgnoreCollision (colliderWithMeshesParts [i], colliderToIgnore, state);
		}
	}

	public void ignoreCollisionWithSolidVehicleColliderList (Collider colliderToIgnore, bool state)
	{
		for (int i = 0; i < colliderParts.Count; i++) {
			Physics.IgnoreCollision (colliderParts [i], colliderToIgnore, state);
		}
	}

	public void checkIgnorPassengersCollidersState ()
	{
		if (CollidersStateCoroutine != null) {
			StopCoroutine (CollidersStateCoroutine);
		}

		CollidersStateCoroutine = StartCoroutine (changeIgnorePassengerCollidersStateCoroutine ());
	}

	IEnumerator changeIgnorePassengerCollidersStateCoroutine ()
	{
		changeIgnorePassengerCollidersState (true);

		yield return new WaitForSeconds (1);

		changeIgnorePassengerCollidersState (false);

		yield return null;
	}

	public void setVehiceColliderWithMeshListLayer (string newLayerName)
	{
		int newLayerIndex = LayerMask.NameToLayer (newLayerName); 

		for (int i = 0; i < colliderWithMeshesParts.Count; i++) {
			colliderWithMeshesParts [i].gameObject.layer = newLayerIndex;
		}
	}

	public void setVehiceColliderListLayer (string newLayerName)
	{
		int newLayerIndex = LayerMask.NameToLayer (newLayerName); 

		for (int i = 0; i < colliderParts.Count; i++) {
			colliderParts [i].gameObject.layer = newLayerIndex;
		}
	}

	public void setVehicleOriginalListLayer ()
	{
		for (int i = 0; i < colliderParts.Count; i++) {
			colliderParts [i].gameObject.layer = colliderLayerIndexParts [i];
		}
	}

	public Collider getVehicleCollider ()
	{
		if (colliderParts.Count > 0) {
			for (int i = 0; i < colliderParts.Count; i++) {
				if (colliderParts [i].GetComponent<vehicleDamageReceiver> () != null) {
					return colliderParts [i];
				}
			}
		}

		return null;
	}

	public void manualStartOrStopVehicle (bool state, GameObject passenger)
	{
		if (state) {
			callEventOnGetOn ();
		} else {
			callEventOnGetOff ();
		}

		disableUnlockedCursor ();

		//change the driving value
		Vector3 currentNormal = Vector3.up;

		if (mainVehicleGravityControlFound) {
			currentNormal = vehicleGravitymanager.getCurrentNormal ();
		}

		IKDrivingManager.startOrStopVehicle (passenger, passengersParent, currentNormal, passenger.transform.position);

		//send the message to the vehicle movement component, to enable or disable the driving state
		isBeingDriven = state;

		passengersOnVehicle = false;

		changeVehicleStateEvent.Invoke ();

		mainVehicleController.changeVehicleState ();
	}

	public void setCurrentPassenger (GameObject passenger)
	{
		currentPassenger = passenger;
	}

	//function called when the player press the use device button
	public void activateDevice ()
	{
		addOrRemovePassengerToVehicle ();
	}

	void addOrRemovePassengerToVehicle ()
	{
		if (IKDrivingManager.isVehicleFull () && !IKDrivingManager.getPassengerGameObjectList ().Contains (currentPassenger)) {
			return;
		}

		disableUnlockedCursor ();

		Vector3 nextPlayerPosition = Vector3.zero;

		//if the vehicle is being driven, check if the player can get off
		nextPlayerPosition = IKDrivingManager.getPassengerGetOffPosition (currentPassenger);

		//if the vehicle is not being driven (so the player is going to get on) or there is no obstacles to get off
		if (nextPlayerPosition != -Vector3.one) {
			//change the driving value

			if (usedByAI) {
				if (!passengersOnVehicle && isBeingDriven) {
					activaAIVehicle ();
				}
			}

			//print (currentPassenger.name);
			Vector3 currentNormal = Vector3.up;

			if (mainVehicleGravityControlFound) {
				currentNormal = vehicleGravitymanager.getCurrentNormal ();
			}

			bool passengerOnDriverSeat = IKDrivingManager.startOrStopVehicle (currentPassenger, passengersParent, currentNormal, nextPlayerPosition);

			if (!ignoreSettingPassengerStateActive) {
				setPassengerState (passengerOnDriverSeat);
			}

			ignoreSettingPassengerStateActive = false;

			if (usedByAI) {
				if (useEventsToCheckIfVehicleUsedByAIOnPassengerEnterExit) {
					if (passengersOnVehicle) {
						eventToCheckIfVehicleUsedByAIOnPassengerEnter.Invoke ();
					} else {
						eventToCheckIfVehicleUsedByAIOnPassengerExit.Invoke ();

						if (!isBeingDriven) {
							activaAIVehicle ();
						}
					}
				}
			}

			checkEventsToSendPassengerOnStateChange (currentPassenger, passengersOnVehicle);
		}
	}

	public void setIgnoreSettingPassengerStateActiveState (bool state)
	{
		ignoreSettingPassengerStateActive = state;
	}

	public void setPassengerState (bool passengerOnDriverSeat)
	{
//		print ("STATE: " + passengerOnDriverSeat + " " + IKDrivingManager.getPassengerGameObjectList ().Count);

		updatePassengersInVehicleCheckState ();

		passengerGettingOnOffEvent.Invoke ();

		if (passengerOnDriverSeat) {
			isBeingDriven = !isBeingDriven;

			//send the message to the vehicle movement component, like car controller or motorbike controller
			changeVehicleStateEvent.Invoke ();

			mainVehicleController.changeVehicleState ();
		}

		if (isBeingDriven) {
			callEventOnGetOn ();
		} else {
			callEventOnGetOff ();
		}

		if (!isBeingDriven) {
			if (mainVehicleController.vehicleControllerSettings.autoBrakeOnGetOff) {
				mainVehicleController.activateAutoBrakeOnGetOff ();
			}
		}
	}

	public void updatePassengersInVehicleCheckState ()
	{
		if (IKDrivingManager.getPassengerGameObjectList ().Count > 0) {
			passengersOnVehicle = true;
		} else {
			passengersOnVehicle = false;
		}
	}

	public void activaAIVehicle ()
	{
		isBeingDriven = !isBeingDriven;

		//send the message to the vehicle movement component, like car controller or motorbike controller
		changeVehicleStateEvent.Invoke ();

		mainVehicleController.changeVehicleState ();
	}

	public Vector3 currentVehicleVelocity ()
	{
		return mainRigidbody.velocity;
	}
		
	//if any collider in the vehicle collides, then
	void OnCollisionEnter (Collision collision)
	{
		currentContact = collision.contacts [0];

		if (mainVehicleControllerFound) {
			mainVehicleController.setCollisionDetected (collision);
		}

		if (mainVehicleGravityControlFound) {
			vehicleGravitymanager.setCollisionDetected (collision);
		}

		float collisionMagnitude = collision.relativeVelocity.magnitude;
		Vector3 contactPoint = currentContact.point;

		//check that the collision is not with the player
		if (passengersOnVehicle && launchDriverOnCollision) {
			if (collisionMagnitude > minCollisionVelocityToLaunch) {

				Vector3 collisionDirectionOffset = Vector3.zero;

				if (launchDirectionOffset != Vector3.zero) {
					collisionDirectionOffset = transform.right * collisionDirectionOffset.x +
					transform.up * collisionDirectionOffset.y +
					transform.forward * collisionDirectionOffset.z;
				}

				Vector3 collisionDirection = (contactPoint + collisionDirectionOffset - transform.position).normalized;

				if (extraCollisionForce > 1) {
					collisionDirection *= extraCollisionForce;
				}

				launchCharacterOnVehicleCollision (collisionDirection, collisionMagnitude);
			}
		}

		if (receiveDamageFromCollision) {
			if (collisionMagnitude > minVelocityToDamageCollision) {
				Vector3 collisionDirection = (contactPoint - transform.position).normalized;

				float collisionDamage = defaultDamageCollision;

				if (useCurrentVelocityAsDamage) {
					collisionDamage = collisionMagnitude;
				} 

				if (collisionDamageMultiplier > 0) {
					collisionDamage *= collisionDamageMultiplier;
				}

				if (showDebugLogCollisions) {
					print (collisionDamage);
				}

				applyDamage.checkHealth (gameObject, gameObject, collisionDamage, collisionDirection, contactPoint, gameObject, 
					false, true, false, false, false, -1, -1);
			}
		}

		if (damageObjectsOnCollision) {
			if (showDebugLogCollisions) {
				Debug.Log ("Collision Relative Velocity: " + collisionMagnitude);
				Debug.Log ("Vehicle Velocity: " + mainRigidbody.velocity.magnitude);
			}

			if (collisionMagnitude > minCollisionVelocityToDamage && mainRigidbody.velocity.magnitude > minVehicleVelocityToDamage) {
				if (showDebugLogCollisions) {
					Debug.Log ("Collision Damage To Apply: " + (collisionMagnitude * damageMultiplierOnCollision));
				}

				//if the vehicle hits another vehicle, apply damage to both of them according to the velocity at the impact
				applyDamage.checkHealth (gameObject, collision.collider.gameObject, collisionMagnitude * damageMultiplierOnCollision, 
					currentContact.normal, contactPoint, gameObject, false, true, ignoreShieldOnCollision, false, false, -1, damageTypeID);
			
				if (useRemoteEventOnObjectsFound) {
					remoteEventSystem currentRemoteEventSystem = collision.collider.gameObject.GetComponent<remoteEventSystem> ();
				
					if (currentRemoteEventSystem != null) {
						for (int i = 0; i < remoteEventNameList.Count; i++) {

							currentRemoteEventSystem.callRemoteEvent (remoteEventNameList [i]);
						}
					}
				}
			}
		}

		if (passengersOnVehicle) {
			if (IKDrivingManager.addCollisionForceDirectionToPassengers) {
				IKDrivingManager.setCollisionForceDirectionToPassengers ((contactPoint - transform.position).normalized * collisionMagnitude);
			}
		}
	}

	public void setNewVehicleGravityForce (float newVehicleGravityForce)
	{
		if (mainVehicleGravityControlFound) {
			vehicleGravitymanager.setNewGravityForce (newVehicleGravityForce);
		}
	}

	public void setOriginalGravityForce ()
	{
		if (mainVehicleGravityControlFound) {
			vehicleGravitymanager.setOriginalGravityForce ();
		}
	}

	public void setReducedVehicleSpeed (float newValue)
	{
		mainRigidbody.velocity *= newValue;
	}

	public void launchCharacterOnVehicleCollision (Vector3 collisionDirection, float collisionVelocity)
	{
		List<GameObject> passengerGameObjectList = IKDrivingManager.getPassengerGameObjectList ();

		GameObject currentDriver = IKDrivingManager.getcurrentDriver ();

		for (int i = 0; i < passengerGameObjectList.Count; i++) {
			GameObject currentPassengerToCheck = passengerGameObjectList [i];

			if (currentDriver != currentPassengerToCheck || (currentDriver == currentPassengerToCheck && !IKDrivingManager.getCanBeDrivenRemotelyValue ())) {
				manualStartOrStopVehicle (false, currentPassenger);

				if (currentPassenger != null) {
					currentPassenger.GetComponent<Collider> ().isTrigger = true;

					usingDevicesSystem currentUsingDevicesSystem = currentPassenger.GetComponent<usingDevicesSystem> ();
					currentUsingDevicesSystem.removeDeviceFromListExternalCall (gameObject);
					currentUsingDevicesSystem.removeCurrentVehicle (gameObject);
					currentUsingDevicesSystem.disableIcon ();

					if (ignoreRagdollCollider) {
						currentPassenger.SendMessage ("ignoreCollisionWithBodyColliderList", colliderParts, SendMessageOptions.DontRequireReceiver);
					}

					currentPassenger.SendMessage ("pushFullCharacter", collisionDirection, SendMessageOptions.DontRequireReceiver);

					if (applyDamageToDriver) {
						if (!useCollisionVelocityAsDamage) {
							collisionVelocity = collisionDamageAmount;
						}
					
						applyDamage.checkHealth (gameObject, currentPassenger, collisionVelocity, collisionDirection, currentPassenger.transform.position, 
							gameObject, false, true, ignoreShieldOnLaunch, false, false, -1, -1);
					}
				}
			}
		}
	}

	public bool checkIfDetectSurfaceBelongToVehicle (Collider surfaceFound)
	{
		if (colliderParts.Contains (surfaceFound)) {
			return true;
		} 

		return false;
	}

	//the player has used a pickup while he is driving, so the health is added in the vehicle
	public void getHealth (float amount)
	{
		if (!destroyed) {
			//increase the health amount
			healthAmount += amount;

			//check that the current health is not higher than the max value
			if (healthAmount >= maxHealthAmount) {
				healthAmount = maxHealthAmount;
			}

			//set the value in the slider of the HUD
			updateHealthSlider (healthAmount);

			//check the current health amount to stop or reduce the damage particles
			if (useDamageParticles) {
				changeDamageParticlesValue (false, amount);
			}

			if (damageInScreenManager != null) {
				damageInScreenManager.showScreenInfo (amount, false, Vector3.zero, healthAmount, 0);
			}

			auxHealthAmount = healthAmount;
		}
	}

	public void updateHealthSlider (float value)
	{
		if (vehicleHealth != null) {
			vehicleHealth.value = value;
		}
	}

	//the player has used a pickup while he is driving, so the boost is added in the vehicle
	public void getEnergy (float amount)
	{
		if (!destroyed) {
			//increase the boost amount
			boostAmount += amount;

			//check that the current boost is not higher than the max value
			if (boostAmount >= maxBoostAmount) {
				boostAmount = maxBoostAmount;
			}

			//set the value in the slider of the HUD
			updateEnergySlider (boostAmount);

			auxPowerAmount = boostAmount;
		}
	}

	public void removeEnergy (float amount)
	{
		//increase the boost amount
		boostAmount -= amount;

		//check that the current boost is not higher than the max value
		if (boostAmount < 0) {
			boostAmount = 0;
		}

		//set the value in the slider of the HUD
		updateEnergySlider (boostAmount);

		auxPowerAmount = boostAmount;
	}

	public void updateEnergySlider (float value)
	{
		if (vehicleBoost != null) {
			vehicleBoost.value = value;
		}
	}

	public void getFuel (float amount)
	{
		if (!destroyed) {
			fuelAmount += amount;

			if (fuelAmount > maxFuelAmount) {
				fuelAmount = maxFuelAmount;
				setGasTankState (false);
			}

			updateFuelSlider (fuelAmount);
			auxFuelAmount = fuelAmount;
		}
	}

	public void updateFuelSlider (float value)
	{
		if (vehicleFuel != null) {
			vehicleFuel.value = value;
		}
	}

	public void setGasTankState (bool state)
	{
		if (gasTankManager != null) {
			gasTankManager.enableOrDisableTrigger (state);
		}
	}

	public void refillFuelTank ()
	{
		if (gasTankManager != null) {
			int fuelAmountToRefill = gasTankManager.getCurrentAmountUsed ();

			getFuel (fuelAmountToRefill);
		}
	}

	//the player has used a pickup while he is driving, so the ammo is added in the vehicle
	public void getAmmo (string ammoName, int amount)
	{
		if (weaponsManager != null && weaponsManager.weaponsEnabled) {
			weaponsManager.getAmmo (ammoName, amount);
		}
	}

	//get the value of the current speed in the vehicle
	public void getSpeed (float speed, float maxSpeed)
	{
		if (currentSpeed != null) {
			currentSpeed.text = speed.ToString ("0") + " / " + maxSpeed;
		}
	}

	//if the health or the boost are regenerative, increase the values according to the last time damaged or used
	public void manageBarInfo (string sliderType, bool regenerate, bool constantRegenerate, float regenerateSpeed, float regenerateTime, float regenerateAmount,
	                           Slider bar, float barAmount, float maxAmount, float lastTime)
	{
		if (regenerate && !destroyed) {
			if (constantRegenerate) {
				if (regenerateSpeed > 0 && barAmount < maxAmount) {
					if (Time.time > lastTime + regenerateTime) {
						if (sliderType.Equals (healthStatName)) {
							getHealth (regenerateSpeed * Time.deltaTime);
						} else if (sliderType.Equals (energyStatName)) {
							getEnergy (regenerateSpeed * Time.deltaTime);
						} else if (sliderType.Equals (fuelStatName)) {
							getFuel (regenerateSpeed * Time.deltaTime);
						}
					}
				}
			} else {
				if (barAmount < maxAmount) {
					if (Time.time > lastTime + regenerateTime) {
						if (sliderType.Equals (healthStatName)) {
							getHealth (regenerateAmount);

							lastDamageTime = Time.time;
						} else if (sliderType.Equals (energyStatName)) {
							getEnergy (regenerateAmount);

							lastBoostTime = Time.time;
						} else if (sliderType.Equals (fuelStatName)) {
							getFuel (regenerateAmount);

							lastFuelTime = Time.time;
						}
					}
				}
			}
		}
	}

	//use the boost in the vehicle, checking the current amount of energy in it
	public bool useBoost (bool moving)
	{
		bool boostAvaliable = false;

		//the vehicle is moving so 
		if (moving) {
			if (infiniteBoost) {
				boostAmount = maxBoostAmount;

				boostAvaliable = true;
			} else if (boostAmount > 0) {
				//reduce the boost amount and return a true value
				boostAmount -= Time.deltaTime * boostUseRate;

				auxPowerAmount = boostAmount;

				boostAvaliable = true;
			}

			if (boostAmount < 0) {
				boostAmount = 0;
			}

			if (boostAvaliable) {
				updateEnergySlider (boostAmount);

				lastBoostTime = Time.time;
			}
		}

		return boostAvaliable;
	}

	public void setVehicleUseFuelState (bool state)
	{
		vehicleUseFuel = state;
	}

	public bool useFuel ()
	{
		if (!vehicleUseFuel) {
			return true;
		}

		bool fuelAvaliable = false;

		if (infiniteFuel) {
			fuelAmount = maxFuelAmount;

			fuelAvaliable = true;
		} else if (fuelAmount > 0) {
			fuelAmount -= Time.deltaTime * fuelUseRate;

			auxFuelAmount = fuelAmount;

			fuelAvaliable = true;

			if (fuelAmount < 0) {
				fuelAmount = 0;
			}

			if (fuelAmount < maxFuelAmount) {
				setGasTankState (true);
			}
		}

		if (fuelAvaliable) {
			updateFuelSlider (fuelAmount);

			lastFuelTime = Time.time;
		}

		return fuelAvaliable;
	}

	public void removeFuel (float amount)
	{
		if (!vehicleUseFuel) {
			return;
		}

		if (infiniteFuel) {
			fuelAmount = maxFuelAmount;

		} else if (fuelAmount > 0) {
			fuelAmount -= amount;

			auxFuelAmount = fuelAmount;

			if (fuelAmount < 0) {
				fuelAmount = 0;
			}

			if (fuelAmount < maxFuelAmount) {
				setGasTankState (true);
			}
		}

		updateFuelSlider (fuelAmount);
	}

	public void setRemainFuel (float newAmount)
	{
		fuelAmount = newAmount;
	}

	public eventParameters.eventToCallWithAmount eventToSendCurrentFuelAmount;

	public void activateEventToSendCurrentFuelAmount ()
	{
		eventToSendCurrentFuelAmount.Invoke (fuelAmount);
	}

	public eventParameters.eventToCallWithAmount eventToSendCurrentHealthAmount;

	public void activateEventToSendCurrentHealthAmount ()
	{
		eventToSendCurrentHealthAmount.Invoke (healthAmount);
	}

	public eventParameters.eventToCallWithAmount eventToSendCurrentBoostAmount;

	public void activateEventToSendCurrentBoostAmount ()
	{
		eventToSendCurrentBoostAmount.Invoke (boostAmount);
	}

	public void setRemainHealth (float newAmount)
	{
		healthAmount = newAmount;
	}

	public void setRemainBoost (float newAmount)
	{
		boostAmount = newAmount;
	}

	public void setFuelInfo ()
	{
		if (vehicleFuel != null) {
			vehicleFuel.maxValue = maxFuelAmount;
			vehicleFuel.value = fuelAmount;
		}
	}

	//when the current weapon is changed for another, get the current name, ammo per clip and clip size of that weapon
	public void setWeaponName (string name, int ammoPerClip, int clipSize)
	{
		if (weaponNameText != null) {
			weaponNameText.text = name;

			vehicleAmmo.maxValue = ammoPerClip;

			vehicleAmmo.value = clipSize;
		}
	}

	//the player is shooting while he is driving, so use ammo of the vehicle weapon
	public void useAmmo (int clipSize, int remainAmmo)
	{
		if (ammoAmountText != null) {
			if (remainAmmo == -1) {
				ammoAmountText.text = clipSize + "/Inf";
			} else {
				ammoAmountText.text = clipSize + "/" + remainAmmo;
			}

			vehicleAmmo.value = clipSize;
		}
	}

	//the vehicle is receiving damage, getting the current damage amount, the direction of the projectile, its hit position, the object that fired it and if the damage is applied only
	//one time, like a bullet, or constantly like a laser
	public void setDamage (float amount, Vector3 fromDirection, Vector3 damagePos, GameObject bulletOwner, GameObject projectile, 
	                       bool damageConstant, bool searchClosestWeakSpot, bool damageCanBeBlocked)
	{
		if (!damageConstant) {
			//if the projectile is not a laser, store it in a list
			//this is done like this because you can add as many colliders (box or mesh) as you want (according to the vehicle meshes), 
			//which are used to check the damage received by every vehicle, so like this the damage detection is really accurated. 
			//For example, if you shoot a grenade to a car, every collider will receive the explosion, but the vehicle will only be damaged once, with the correct amount.
			//in this case the projectile has not produced damage yet, so it is stored in the list and in the below code the damage is applied. 
			//This is used for bullets for example, which make damage only in one position
			if (!projectilesReceived.Contains (projectile)) {
				projectilesReceived.Add (projectile);

				projectilesListContainsElements = true;
			} 
			//in this case the projectile has been added to the list previously, it means that the projectile has already applied damage to the vehicle, 
			//so it can't damaged the vehicle twice. This is used for grenades for example, which make a damage inside a radius
			else {
				return;
			}
		}

		//if any elememnt in the list of current projectiles received is not longer exits, remove it from the list
		for (int i = projectilesReceived.Count - 1; i >= 0; i--) {
			if (projectilesReceived [i] == null) {
				projectilesReceived.RemoveAt (i);
			}
		}
	
		if (projectilesReceived.Count == 0) {
			projectilesListContainsElements = false;
		}

		//if the object is not dead, invincible or its health is zero, exit
		if (invincible || destroyed || amount <= 0) {
			return;
		}

		if (vehicleCameraManager.shakeSettings.useDamageShake && isBeingDriven) {
			vehicleCameraManager.setDamageCameraShake ();
		}

		damageReceiverInfo currentWeakSpot = new damageReceiverInfo ();

		if (useWeakSpots && searchClosestWeakSpot) {
			int weakSpotIndex = getClosesWeakSpotIndex (damagePos);

			lastWeakSpotIndex = weakSpotIndex;

			if (amount < healthAmount) {
				currentWeakSpot = advancedSettings.damageReceiverList [weakSpotIndex];

				if (advancedSettings.damageReceiverList [weakSpotIndex].killedWithOneShoot) {
					if (advancedSettings.damageReceiverList [weakSpotIndex].needMinValueToBeKilled) {
						if (advancedSettings.damageReceiverList [weakSpotIndex].minValueToBeKilled < amount) {
							amount = healthAmount;
						}
					} else {
						amount = healthAmount;
					}
				}
			}

			if (currentWeakSpot.useHealthAmountOnSpot && !currentWeakSpot.healthAmountOnSpotEmtpy) {
				currentWeakSpot.healhtAmountOnSpot -= amount;

				if (currentWeakSpot.healhtAmountOnSpot <= 0) {
					currentWeakSpot.eventOnEmtpyHealthAmountOnSpot.Invoke ();

					currentWeakSpot.healthAmountOnSpotEmtpy = true;

					if (currentWeakSpot.killCharacterOnEmtpyHealthAmountOnSpot) {
						amount = healthAmount;
					}
				}
			}
		}

		if (amount > healthAmount) {
			amount = healthAmount;
		}
			
		//decrease the health amount
		healthAmount -= amount;
		auxHealthAmount = healthAmount;

		//if the player is driving this vehicle, set the value in the slider
		if (isBeingDriven) {
			updateHealthSlider (healthAmount);
		}

		if (damageInScreenManager != null) {
			damageInScreenManager.showScreenInfo (amount, true, fromDirection, healthAmount, 0);
		}

		if (useDamageParticles) {
			//increase the damage particles values
			changeDamageParticlesValue (true, amount);
		}

		//set the last time damage
		lastDamageTime = Time.time;

		//if the health reachs 0, call the dead function
		if (healthAmount <= 0) {
			healthAmount = 0;
			destroyed = true;
			destroyVehicle (damagePos);
		}

		if (useWeakSpots && lastWeakSpotIndex > -1 && searchClosestWeakSpot) {
			bool callFunction = false;
			currentWeakSpot = advancedSettings.damageReceiverList [lastWeakSpotIndex];

			if (currentWeakSpot.sendFunctionWhenDamage) {
				callFunction = true;
			}

			if (currentWeakSpot.sendFunctionWhenDie && destroyed) {
				callFunction = true;
			}

			//print (advancedSettings.weakSpots [lastWeakSpotIndex].name +" " +callFunction +" "+ dead);
			if (callFunction) {
				if (currentWeakSpot.damageFunction.GetPersistentEventCount () > 0) {
					currentWeakSpot.damageFunction.Invoke ();
				}
			}

			lastWeakSpotIndex = -1;
		}
	}

	public void setDamageTargetOverTimeState (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{
		stopDamageOverTime ();

		damageOverTimeCoroutine = StartCoroutine (setDamageTargetOverTimeStateCoroutine (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath, damageTypeID));
	}

	IEnumerator setDamageTargetOverTimeStateCoroutine (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{
		yield return new WaitForSeconds (damageOverTimeDelay);

		float lastTimeDamaged = Time.time;

		float lastTimeDuration = lastTimeDamaged;

		while (Time.time < lastTimeDuration + damageOverTimeDuration || (!destroyed && damageOverTimeToDeath)) {
			if (Time.time > lastTimeDamaged + damageOverTimeRate) {
				lastTimeDamaged = Time.time;

				setDamage (damageOverTimeAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, true, false, false);
			}

			yield return null;
		}
	}

	public void stopDamageOverTime ()
	{
		if (damageOverTimeCoroutine != null) {
			StopCoroutine (damageOverTimeCoroutine);
		}
	}

	public int getClosesWeakSpotIndex (Vector3 collisionPosition)
	{
		float distance = Mathf.Infinity;

		int index = -1;

		for (int i = 0; i < advancedSettings.damageReceiverList.Count; i++) {
			if (advancedSettings.damageReceiverList [i].spotTransform != null) {
				float currentDistance = GKC_Utils.distance (collisionPosition, advancedSettings.damageReceiverList [i].spotTransform.position);
			
				if (currentDistance < distance) {
					distance = currentDistance;

					index = i;
				}
			} else {
				print ("WARNING: some damage spot is not configured properly in the vehicle " + vehicleName);
			}
		}

		if (index > -1) {
			if (advancedSettings.showGizmo) {
				print (advancedSettings.damageReceiverList [index].name);
			}
		}

		return index;
	}

	public bool setCollidersOnAllVehicleMeshParts = true;

	//the vehicle health is 0, so the vehicle is destroyed
	public void destroyVehicle (Vector3 damagePosition)
	{
		callEventOnDestroyed ();

		//instantiated an explosiotn particles
		if (useDestroyedParticles) {
			GameObject destroyedParticlesClone = (GameObject)Instantiate (destroyedParticles, transform.position, transform.rotation);

			destroyedParticlesClone.transform.SetParent (transform);
		}

		if (destroyedSound != null) {
			destroyedSource.PlayOneShot (destroyedSound);
		}

		placeToShoot.gameObject.SetActive (false);

		//set the velocity of the vehicle to zero
		mainRigidbody.velocity = Vector3.zero;

		mainRigidbody.isKinematic = true;

		List<GameObject> passengerGameObjectList = IKDrivingManager.getPassengerGameObjectList ();

		List<Collider> passengerColliderList = new List<Collider> ();

		for (int j = 0; j < passengerGameObjectList.Count; j++) {
			passengerColliderList.Add (passengerGameObjectList [j].GetComponent<Collider> ());
		}

		//any other object with a collider but with out renderer, is disabled
		if (colliderWithoutMeshesPartsToIgnore.Count > 0) {
			for (int i = 0; i < colliderWithoutMeshesParts.Count; i++) {
				if (!colliderWithoutMeshesPartsToIgnore.Contains (colliderWithoutMeshesParts [i])) {
					colliderWithoutMeshesParts [i].enabled = false;
				}
			}
		} else {
			for (int i = 0; i < colliderWithoutMeshesParts.Count; i++) {
				colliderWithoutMeshesParts [i].enabled = false;
			}
		}

		if (fadeVehiclePiecesOnDestroyed) {
			if (destroyedMeshShader == null) {
				destroyedMeshShader = Shader.Find ("Legacy Shaders/Transparent/Diffuse");
			}
		}

		Renderer currentRenderer;
		GameObject currentVehiclePiece;
		Rigidbody currentRigidbody;
		Collider currentCollider;

		int passengerColliderListCount = passengerColliderList.Count;

		int newLayerIndex = LayerMask.NameToLayer ("Ignore Raycast");

		//get every renderer component if the car
		int vehiclePartInfoListCount = vehiclePartInfoList.Count;

		for (int i = 0; i < vehiclePartInfoListCount; i++) {
			currentRenderer = vehiclePartInfoList [i].mainRenderer;

			if (currentRenderer != null) {
				currentVehiclePiece = currentRenderer.gameObject;
				
				if (currentRenderer.enabled) {

					if (fadeVehiclePiecesOnDestroyed && removePiecesWhenDestroyed) {
						//for every renderer object, change every shader in it for a transparent shader 
						int materialsLength = currentRenderer.materials.Length;

						for (int j = 0; j < materialsLength; j++) {
							Material currentMaterial = currentRenderer.materials [j];

							currentMaterial.shader = destroyedMeshShader;
							rendererParts.Add (currentMaterial);
						}
					}

					//set the layer ignore raycast to them
					currentVehiclePiece.layer = newLayerIndex;

					if (setCollidersOnAllVehicleMeshParts) {

						if (!useCustomVehiclePartsToDestroy) {
							currentRigidbody = vehiclePartInfoList [i].mainRigidbody;

							//add rigidbody and box collider to them
							if (currentRigidbody == null) {
								currentRigidbody = currentVehiclePiece.AddComponent<Rigidbody> ();
							}
							
							//apply explosion force
							currentRigidbody.AddExplosionForce (vehicleExplosionForce, damagePosition, vehicleExplosionRadius, 3, vehicleExplosionForceMode);
						}

						currentCollider = vehiclePartInfoList [i].mainCollider;

						if (currentCollider == null) {
							currentCollider = currentVehiclePiece.AddComponent<BoxCollider> ();
						} else {
							currentCollider.material = null;
						}

						colliderWithMeshesParts.Add (currentCollider);

						//ignore collisions with the player
						for (int j = 0; j < passengerColliderListCount; j++) {
							Physics.IgnoreCollision (passengerColliderList [j], currentCollider);
						}
					}
				}
			}
		}

		if (useCustomVehiclePartsToDestroy) {
			if (useEventOnCustomVehicleParts) {
				eventOnCustomVehicleParts.Invoke ();
			}

			int customVehiclePartsListToDestroyCount = customVehiclePartsListToDestroy.Count;

			for (int i = 0; i < customVehiclePartsListToDestroyCount; i++) {
				currentVehiclePiece = customVehiclePartsListToDestroy [i].vehiclePartGameObject;

				if (currentVehiclePiece != null) {
					currentRigidbody = customVehiclePartsListToDestroy [i].mainRigidbody;

					//add rigidbody and box collider to them
					if (currentRigidbody == null) {
						currentRigidbody = currentVehiclePiece.AddComponent<Rigidbody> ();
					}

					//apply explosion force
					if (currentRigidbody != null) {
						currentRigidbody.AddExplosionForce (customVehiclePartsExplosionForce, damagePosition, customVehiclePartsExplosionRadius, 3, customVehiclePartsExplosionForceMode);
					}

					if (customVehiclePartsListToDestroy [i].addCollider) {
						currentCollider = customVehiclePartsListToDestroy [i].mainCollider;

						if (currentCollider == null) {
							currentCollider = currentVehiclePiece.AddComponent<BoxCollider> ();
						} else {
							currentCollider.material = null;
						}

						colliderWithMeshesParts.Add (currentCollider);

						//ignore collisions with the player
						for (int j = 0; j < passengerColliderListCount; j++) {
							Physics.IgnoreCollision (passengerColliderList [j], currentCollider);
						}
					}
				}
			}
		}

		//stop the IK system in the player
		IKDrivingManager.disableVehicle ();

		if (mapInformationManager != null) {
			mapInformationManager.removeMapObject ();
		}
	}

	public void destroyVehicleAtOnce ()
	{
		Destroy (IKDrivingManager.gameObject);
		Destroy (vehicleCameraManager.gameObject);
		Destroy (gameObject);
	}

	//this function is called when the vehicle receives damage, to enable a fire and smoke particles system to show serious damage in the vehicle
	void changeDamageParticlesValue (bool damaging, float amount)
	{
		//if the vehicle has a damage particles object
		if (damageParticles != null) {
			bool activate = false;
			bool activeLoop;

			//if the health is 0, disable the damage particles
			if (healthAmount <= 0) {
				for (int i = 0; i < fireParticles.Count; i++) {
					if (fireParticles [i] != null) {
						if (fireParticles [i].gameObject.activeSelf) {
							fireParticles [i].gameObject.SetActive (false);
						}
					}
				}

				return;
			}

			//if the current vehicle health is lower than certain %, the damage particles are enabled 
			bool lowHealth = false;

			if (healthAmount <= maxHealthAmount / (100 / healthPercentageDamageParticles)) {
				activate = true;
				activeLoop = true;
				lowHealth = true;
			} else {
				activeLoop = false;
			}

			for (int i = 0; i < fireParticles.Count; i++) {
				//enable the particles
				if (activate) {
					if (fireParticles [i] != null) {
						if (!fireParticles [i].gameObject.activeSelf) {
							fireParticles [i].gameObject.SetActive (true);

							var fireParticlesMain = fireParticles [i].main;

							fireParticlesMain.loop = true;
						}

						if (!fireParticles [i].isPlaying) {
							fireParticles [i].Play ();
						}
					}
				}

				//enable or disable their loop, if the particles are enabled, and the health is higher that the above %, then the particles loop is disabled, because the car 
				//has a better health
				if (!activeLoop) {
					if (fireParticles [i].isPlaying) {
						var fireParticlesMain = fireParticles [i].main;

						fireParticlesMain.loop = false;
					}
				}

				//if the health Percentage Damage Particles is reached, then increase or decrease its size according to if the vehicle is being damaged or receiving health
				if (lowHealth) {
					var fireParticlesMain = fireParticles [i].main;

					if (damaging) { 
						fireParticlesMain.startSizeMultiplier += amount * 0.05f;
					} else {
						fireParticlesMain.startSizeMultiplier -= amount * 0.05f;
					}
				}
			}
		}
	}

	//when the player gets on the vehicle, the IK driving system sends every slider and text component of the vehicles HUD, to update every value and show them to the player
	public void getHUDBars (playerHUDManager newVehicleHUDInfo, playerHUDManager.vehicleHUDElements hudElements, bool drivingState)
	{
		currentVehicleHUDInfo = newVehicleHUDInfo;

		currentHUDElements = hudElements;

		vehicleHealth = currentHUDElements.vehicleHealth;
		vehicleBoost = currentHUDElements.vehicleBoost;
		vehicleFuel = currentHUDElements.vehicleFuel;
		vehicleAmmo = currentHUDElements.vehicleAmmo;
		vehicleHealth.value = healthAmount;
		vehicleBoost.value = boostAmount;
		ammoAmountText = currentHUDElements.ammoInfo;
		weaponNameText = currentHUDElements.weaponName;
		currentSpeed = currentHUDElements.currentSpeed;

		//check also if the vehicle has a weapon system attached, to enable or disable it
		//if the vehicle has not a weapon system, the weapon info of the HUD is disabled
		if (currentHUDElements.ammoContent != null && currentHUDElements.vehicleCursor != null) {
			bool hudElementState = false;

			if (weaponsManager != null && weaponsManager.weaponsEnabled) {
				weaponsManager.changeWeaponState (drivingState);

				hudElementState = true;
			}

			if (currentHUDElements.ammoContent.activeSelf != hudElementState) {
				currentHUDElements.ammoContent.SetActive (hudElementState);
			}

			if (currentHUDElements.vehicleCursor.activeSelf != hudElementState) {
				currentHUDElements.vehicleCursor.SetActive (hudElementState);
			}
		}

		if (currentHUDElements.healthContent != null) {
			bool hudElementState = false;

			if (!invincible) {
				hudElementState = true;
			}

			if (currentHUDElements.healthContent.activeSelf != hudElementState) {
				currentHUDElements.healthContent.SetActive (hudElementState);
			}
		}

		if (currentHUDElements.fuelContent != null) {
			bool hudElementState = false;

			if (vehicleUseFuel && !infiniteFuel) {
				hudElementState = true;

				setFuelInfo ();
			}

			if (currentHUDElements.fuelContent.activeSelf != hudElementState) {
				currentHUDElements.fuelContent.SetActive (hudElementState);
			}
		}

		if (currentHUDElements.energyContent != null) {
			bool hudElementState = false;

			if (vehicleUseBoost && !infiniteBoost) {
				hudElementState = true;
			}

			if (currentHUDElements.energyContent.activeSelf != hudElementState) {
				currentHUDElements.energyContent.SetActive (hudElementState);
			}
		}
			
		if (currentHUDElements.speedContent != null) {
			bool hudElementState = false;

			if (showVehicleSpeed) {
				hudElementState = true;
			}

			if (currentHUDElements.speedContent.activeSelf != hudElementState) {
				currentHUDElements.speedContent.SetActive (hudElementState);
			}
		}
	}

	public playerHUDManager.vehicleHUDElements getCurrentHUDElements ()
	{
		return currentHUDElements;
	}

	public playerHUDManager getCurrentVehicleHUDInfo ()
	{
		return currentVehicleHUDInfo;
	}

	public void setWeaponState (bool state)
	{
		if (weaponsManager != null && weaponsManager.weaponsEnabled) {
			weaponsManager.changeWeaponState (state);

			if (currentHUDElements != null) {
				if (currentHUDElements.ammoContent != null) {

					if (currentHUDElements.ammoContent.activeSelf != state) {
						currentHUDElements.ammoContent.SetActive (state);
					}

					if (currentHUDElements.vehicleCursor.activeSelf != state) {
						currentHUDElements.vehicleCursor.SetActive (state);
					}
				}
			}
		}
	}

	public bool showVehicleHUDActive ()
	{
		return showVehicleHUD;
	}
		
	//use a jump platform
	public void useJumpPlatform (Vector3 direction)
	{
		if (useJumpPlatformEvents) {
			jumpPlatformEvent.Invoke (direction);
		}
	}

	public void useJumpPlatformParable (Vector3 direction)
	{
		if (useJumpPlatformEvents) {
			jumpPlatformParableEvent.Invoke (direction);
		}
	}

	public void useJumpPlatformWithKeyButton (bool state, float newJumpPower)
	{
		if (useJumpPlatformEvents) {
			if (state) {
				setNewJumpPowerEvent.Invoke (newJumpPower);
			} else {
				setOriginalJumpPowerEvent.Invoke ();
			}
		}
	}

	void checkEventsToSendPassengerOnStateChange (GameObject currentPassegerGameObject, bool state)
	{
		if (useEventToSendPassenger) {
			if (state) {
				eventToSendPassengerOnGetOn.Invoke (currentPassegerGameObject);
			} else {
				eventToSentPassengerOnGetOff.Invoke (currentPassegerGameObject);
			}
		}
	}

	public void debugLaunchCharacterOnVehicleCollision ()
	{
		if (passengersOnVehicle && launchDriverOnCollision) {

			Vector3 collisionDirectionOffset = Vector3.zero;

			if (launchDirectionOffset != Vector3.zero) {
				collisionDirectionOffset = transform.right * collisionDirectionOffset.x +
				transform.up * collisionDirectionOffset.y +
				transform.forward * collisionDirectionOffset.z;
			}

			Vector3 collisionDirection = (debugLaunchCharacterPosition + collisionDirectionOffset).normalized;

			if (extraCollisionForce > 1) {
				collisionDirection *= extraCollisionForce;
			}

			launchCharacterOnVehicleCollision (collisionDirection, debugLaunchCharacterSpeed);
		}
	}

	public void setVehicleParts ()
	{
		setVehicleDamageReceivers ();

		setColliderList ();

		setVehicleRendererParts ();

		setVehicleParticleSystemList ();

		setVehiclePartsInfoList ();

		setCustomVehiclePartsListToDestroy ();

		setVehicleInput ();

		setVehicleWeaponsComponents ();

		getVehicleComponents ();

		updateComponent ();

		GKC_Utils.updateDirtyScene ("Update Vehicle Parts", gameObject);

		print ("Vehicle list for damage receivers, colliders, renderer,  particles, input and vehicle weapons have been updated");
	}

	public void setVehicleDamageReceivers ()
	{
		vehicleDamageReceiverList.Clear ();

		Component[] damageReceivers = IKDrivingManager.gameObject.GetComponentsInChildren (typeof(vehicleDamageReceiver));
		foreach (vehicleDamageReceiver child in damageReceivers) {
			child.setVehicle (gameObject, this);

			vehicleDamageReceiverList.Add (child);
		}

		updateComponent ();
	}

	public void setColliderList ()
	{
		colliderParts.Clear ();

		colliderLayerIndexParts.Clear ();

		Component[] colliderList = IKDrivingManager.gameObject.GetComponentsInChildren (typeof(Collider));
		foreach (Collider currentCollider in colliderList) {
			if (!currentCollider.isTrigger) {
				colliderParts.Add (currentCollider);

				colliderLayerIndexParts.Add (currentCollider.gameObject.layer);
			}
		}

		colliderWithoutMeshesParts.Clear ();

		for (int i = 0; i < colliderParts.Count; i++) {
			if (!colliderParts [i].GetComponent<Renderer> ()) {

				colliderWithoutMeshesParts.Add (colliderParts [i]);
			}
		}

		updateComponent ();
	}

	public void setColliderWithoutMeshesPartsToIgnore (List<Transform> newList)
	{
		colliderWithoutMeshesPartsToIgnore.Clear ();

		for (int i = 0; i < newList.Count; i++) {
			if (newList [i] != null) {
				Collider currentCollider = newList [i].GetComponent<Collider> ();

				if (currentCollider != null) {
					colliderWithoutMeshesPartsToIgnore.Add (currentCollider);
				}
			}
		}

		updateComponent ();
	}

	public void setVehicleRendererParts ()
	{
		vehicleRendererList.Clear ();

		Component[] components = IKDrivingManager.gameObject.GetComponentsInChildren (typeof(Renderer));
		foreach (Renderer child in components) {
			if (!vehiclePartsToIgnore.Contains (child.gameObject)) {
				vehicleRendererList.Add (child);
			}
		}

		updateComponent ();
	}

	public void setVehicleParticleSystemList ()
	{
		if (damageParticles != null) {
			fireParticles.Clear ();

			Component[] fireParticlesComponents = damageParticles.GetComponentsInChildren (typeof(ParticleSystem));
			foreach (ParticleSystem child in fireParticlesComponents) {
				fireParticles.Add (child);
			}

			updateComponent ();
		}
	}

	public void setVehiclePartsInfoList ()
	{
		vehiclePartInfoList.Clear ();

		for (int i = 0; i < vehicleRendererList.Count; i++) {
			vehiclePartInfo newVehiclePartInfo = new vehiclePartInfo ();

			newVehiclePartInfo.vehiclePartGameObject = vehicleRendererList [i].gameObject;
			newVehiclePartInfo.mainRenderer = vehicleRendererList [i];
			newVehiclePartInfo.mainRigidbody = vehicleRendererList [i].GetComponent<Rigidbody> ();
			newVehiclePartInfo.mainCollider = vehicleRendererList [i].GetComponent<Collider> ();

			vehiclePartInfoList.Add (newVehiclePartInfo);
		}

		updateComponent ();
	}

	public void setCustomVehiclePartsListToDestroy ()
	{
		if (useCustomVehiclePartsToDestroy) {

			for (int i = 0; i < customVehiclePartsListToDestroy.Count; i++) {
				if (customVehiclePartsListToDestroy [i].vehiclePartGameObject != null) {
					customVehiclePartsListToDestroy [i].mainRigidbody = customVehiclePartsListToDestroy [i].vehiclePartGameObject.GetComponent<Rigidbody> ();
					customVehiclePartsListToDestroy [i].mainCollider = customVehiclePartsListToDestroy [i].vehiclePartGameObject.GetComponent<Collider> ();
				}
			}

			updateComponent ();
		}
	}

	public void setVehicleInput ()
	{
		inputActionManager currentIKDrivingManager = IKDrivingManager.gameObject.GetComponent<inputActionManager> ();

		if (currentIKDrivingManager != null) {
			currentIKDrivingManager.setInputManagerOnEditor ();
		}
	}

	public void setVehicleWeaponsComponents ()
	{
		if (weaponsManager != null) {
			weaponsManager.setVehicleWeaponsComponents ();
		}
	}

	public void getVehicleComponents ()
	{
		mainVehicleController = GetComponent<vehicleController> ();

		mainVehicleController.getVehicleComponents (IKDrivingManager.gameObject);

		vehicleGravitymanager.mainVehicleController = mainVehicleController;

		GKC_Utils.updateComponent (vehicleGravitymanager);
	}

	public void getAllDamageReceivers ()
	{
		advancedSettings.damageReceiverList.Clear ();

		//get all the damage receivers in the vehicle
		Component[] damageReceivers = IKDrivingManager.gameObject.GetComponentsInChildren (typeof(vehicleDamageReceiver));
		foreach (vehicleDamageReceiver child in damageReceivers) {
			damageReceiverInfo newInfo = new damageReceiverInfo ();

			newInfo.name = "Spot " + (advancedSettings.damageReceiverList.Count + 1);
			newInfo.spotTransform = child.gameObject.transform;
			newInfo.vehicleDamageReceiverManager = child;
			newInfo.damageMultiplier = newInfo.vehicleDamageReceiverManager.damageMultiplier;

			advancedSettings.damageReceiverList.Add (newInfo);
		}

		updateComponent ();
	}

	//health management
	public override float getCurrentHealthAmount ()
	{
		return healthAmount;
	}

	public float getMaxHealthAmount ()
	{
		return maxHealthAmount;
	}

	public float getAuxHealthAmount ()
	{
		return auxHealthAmount;
	}

	public void addAuxHealthAmount (float amount)
	{
		auxHealthAmount += amount;
	}

	public float getHealthAmountToLimit ()
	{
		return maxHealthAmount - auxHealthAmount;
	}

	//energy management
	public float getCurrentEnergyAmount ()
	{
		return boostAmount;
	}

	public float getMaxEnergyAmount ()
	{
		return maxBoostAmount;
	}

	public float getAuxEnergyAmount ()
	{
		return auxPowerAmount;
	}

	public void addAuxEnergyAmount (float amount)
	{
		auxPowerAmount += amount;
	}

	public float getEnergyAmountToLimit ()
	{
		return maxBoostAmount - auxPowerAmount;
	}

	public bool checkIfMaxEnergy ()
	{
		return getCurrentEnergyAmount () >= getMaxEnergyAmount ();
	}

	//fuel management
	public float getCurrentFuelAmount ()
	{
		return fuelAmount;
	}

	public float getMaxFuelAmount ()
	{
		return maxFuelAmount;
	}

	public float getAuxFuelAmount ()
	{
		return auxFuelAmount;
	}

	public void addAuxFuelAmount (float amount)
	{
		auxFuelAmount += amount;
	}

	public float getFuelAmountToLimit ()
	{
		return maxFuelAmount - auxFuelAmount;
	}

	public bool hasFuel ()
	{
		return fuelAmount > 0 || !vehicleUseFuel;
	}

	public void killByButton ()
	{
		destroyVehicle ();
	}

	public void destroyVehicle ()
	{
		setDamage (healthAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, false, false, false);
	}

	public void takeHealth (float damageAmount)
	{
		damageVehicle (damageAmount);
	}

	public void damageVehicle (float damageAmount)
	{
		setDamage (damageAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, false, false, false);
	}

	public float getVehicleRadius ()
	{
		return vehicleRadius;
	}

	public void activateHorn ()
	{
		if (useHornEvent) {
			hornEvent.Invoke ();
		}

		callToFriends ();
	}

	public void callToFriends ()
	{
		if (useHornToCallFriends) {
			friendListManager currentFriendListManager = getCurrentDriver ().GetComponent<friendListManager> ();

			if (currentFriendListManager != null) {
				if (callOnlyFoundFriends) {
					currentFriendListManager.callToFriends ();
				} else {
					currentFriendListManager.findFriendsInRadius (radiusToCallFriends);
				}
			}
		}
	}

	public Transform getPlaceToShoot ()
	{
		return placeToShoot;
	}

	public GameObject getCurrentDriver ()
	{
		if (IKDrivingManager != null) {
			return IKDrivingManager.getcurrentDriver ();
		} 

		return null;
	}

	public string getVehicleName ()
	{
		return vehicleName;
	}

	public void setVehicleName (string newName)
	{
		vehicleName = newName;

		updateComponent ();
	}

	public void getImpactListInfo ()
	{
		if (impactDecalManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainDecalManagerName, typeof(decalManager));

			impactDecalManager = FindObjectOfType<decalManager> ();
		} 

		if (impactDecalManager != null) {
			impactDecalList = new string[impactDecalManager.impactListInfo.Count + 1];

			for (int i = 0; i < impactDecalManager.impactListInfo.Count; i++) {
				string name = impactDecalManager.impactListInfo [i].name;
				impactDecalList [i] = name;
			}

			updateComponent ();
		}
	}

	public int getDecalImpactIndex ()
	{
		return impactDecalIndex;
	}

	public void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	public void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (destroyed) {
			return;
		}

		if ((1 << col.gameObject.layer & layerForPassengers.value) == 1 << col.gameObject.layer) {
			IKDrivingManager.checkTriggerInfo (col, isEnter);
		}
	}

	public vehicleCameraController getVehicleCameraController ()
	{
		return vehicleCameraManager;
	}

	public AudioSource getAudioSourceElement (string name)
	{
		for (int i = 0; i < audioSourceList.Count; i++) {
			if (audioSourceList [i].audioSourceName.Equals (name)) {
				return audioSourceList [i].audioSource;
			}
		}

		return null;
	}

	public bool isVehicleBeingDriven ()
	{
		return isBeingDriven;
	}

	public bool isVehicleFull ()
	{
		return IKDrivingManager.isVehicleFull ();
	}

	public IKDrivingSystem getIKDrivingSystem ()
	{
		return IKDrivingManager;
	}

	public vehicleGravityControl getVehicleGravityControl ()
	{
		return vehicleGravitymanager;
	}

	//set the camera when the player is driving on locked camera
	public void setPlayerCameraParentAndPosition (Transform mainCameraTransform, playerCamera playerCameraManager)
	{
		IKDrivingManager.setPlayerCameraParentAndPosition (mainCameraTransform, playerCameraManager);

		if (weaponsManager != null && weaponsManager.weaponsEnabled) {
			weaponsManager.getCameraInfo (vehicleCameraManager.currentState.cameraTransform, vehicleCameraManager.currentState.useRotationInput);
		}
	}

	public bool vehicleIsDestroyed ()
	{
		return destroyed;
	}

	public void setInvencibleState (bool state)
	{
		invincible = state;
	}

	public void setInfiniteEnergyState (bool state)
	{
		infiniteBoost = state;
	}

	public void setInfiniteFuelState (bool state)
	{
		infiniteFuel = state;
	}

	public void setVehicleAndCameraParent (Transform newParent)
	{
		vehicleCameraManager.setVehicleAndCameraParent (newParent);

		if (newParent != null) {
			setVehicletAsChildOfParentState (true);
		} else {
			setVehicletAsChildOfParentState (false);
		}
	}

	public void setNewMainCameraTransform (Transform newTransform)
	{
		mainVehicleController.setNewMainCameraTransform (newTransform);
	}

	public void setNewPlayerCameraTransform (Transform newTransform)
	{
		mainVehicleController.setNewPlayerCameraTransform (newTransform);
	}

	public void setUseForwardDirectionForCameraDirectionState (bool state)
	{
		mainVehicleController.setUseForwardDirectionForCameraDirectionState (state);
	}

	public void setUseRightDirectionForCameraDirectionState (bool state)
	{
		mainVehicleController.setUseRightDirectionForCameraDirectionState (state);
	}

	public void setAddExtraRotationPausedState (bool state)
	{
		mainVehicleController.setAddExtraRotationPausedState (state);
	}

	public bool isVehicletAsChildOfParent ()
	{
		return vehicleSetAsChildOfParent;
	}

	public void setVehicletAsChildOfParentState (bool state)
	{
		vehicleSetAsChildOfParent = state;
	}

	public bool checkIfMaxHealth ()
	{
		return getCurrentHealthAmount () >= getMaxHealthAmount ();
	}

	public float getHealthAmountToPick (float amount)
	{
		float totalAmmoAmountToAdd = 0;

		float amountToRefill = getHealthAmountToLimit ();

		if (amountToRefill > 0) {
//			print ("amount to refill " + amountToRefill);

			totalAmmoAmountToAdd = amount;

			if (amountToRefill < amount) {
				totalAmmoAmountToAdd = amountToRefill;
			}

//			print (totalAmmoAmountToAdd);

			addAuxHealthAmount (totalAmmoAmountToAdd);
		}

		return totalAmmoAmountToAdd;
	}

	public bool checkIfMaxFuel ()
	{
		return getCurrentFuelAmount () >= getMaxFuelAmount ();
	}

	public void startBrakeVehicleToStopCompletely ()
	{
		mainVehicleController.startBrakeVehicleToStopCompletely ();
	}

	public void endBrakeVehicleToStopCompletely ()
	{
		mainVehicleController.endBrakeVehicleToStopCompletely ();
	}

	public float getCurrentSpeed ()
	{
		return mainVehicleController.getCurrentSpeed ();
	}

	public void changeVehicleState ()
	{
		mainVehicleController.changeVehicleState ();
	}

	public void passengerGettingOnOff ()
	{
		mainVehicleController.passengerGettingOnOff ();
	}

	//Vehicle controller INPUT FUNCTIONS

	//CALL INPUT FUNCTIONS
	public void inputShowControlsMenu ()
	{
		if (isBeingDriven) {
			IKDrivingManager.inputOpenOrCloseControlsMenu (!IKDrivingManager.controlsMenuOpened);
		}
	}

	public void inputSelfDestruct ()
	{
		if (isBeingDriven && canUseSelfDestruct) {
			activateSelfDestruction ();
		}
	}

	public void inputEject ()
	{
		if (isBeingDriven && canEjectFromVehicle) {
			ejectFromVehicle ();
		}
	}

	public void inputEjectWithFreeFloatingMode ()
	{
		if (isBeingDriven && canEjectFromVehicle) {
			ejectFromVehicleWithFreeFloatingMode ();
		}
	}

	public void inputUnLockCursor ()
	{
		if (isBeingDriven && canUnlockCursor) {
			setUnlockCursorState (!cursorUnlocked);
		}
	}

	public void inputHoldOrReleaseSecondaryButton (bool holdingButton)
	{
		if (isBeingDriven && canUnlockCursor && cursorUnlocked) {
			if (holdingButton) {
				vehicleCameraManager.pauseOrPlayVehicleCamera (false);
			} else {
				vehicleCameraManager.pauseOrPlayVehicleCamera (true);
			}
		}
	}

	public void inputGetOffFromVehicle ()
	{
		if (isBeingDriven) {
			IKDrivingManager.getOffFromVehicle ();
		}
	}

	public void inputJump ()
	{
		mainVehicleController.inputJump ();
	}

	public void inputHoldOrReleaseJump (bool holdingButton)
	{
		mainVehicleController.inputHoldOrReleaseJump (holdingButton);
	}

	public void inputHoldOrReleaseTurbo (bool holdingButton)
	{
		mainVehicleController.inputHoldOrReleaseTurbo (holdingButton);
	}

	public void inputSetTurnOnState ()
	{
		mainVehicleController.inputSetTurnOnState ();
	}

	public void inputHorn ()
	{
		mainVehicleController.inputHorn ();
	}

	public void inputHoldOrReleaseBrake (bool holdingButton)
	{
		mainVehicleController.inputHoldOrReleaseBrake (holdingButton);
	}


	//Override functions from Health Management
	public override void setDamageWithHealthManagement (float damageAmount, Vector3 fromDirection, Vector3 damagePos, GameObject attacker, 
	                                                    GameObject projectile, bool damageConstant, bool searchClosestWeakSpot, bool ignoreShield, 
	                                                    bool ignoreDamageInScreen, bool damageCanBeBlocked, bool canActivateReactionSystemTemporally,
	                                                    int damageReactionID, int damageTypeID)
	{
		setDamage (damageAmount, fromDirection, damagePos, attacker, projectile, damageConstant, searchClosestWeakSpot, damageCanBeBlocked);
	}

	public override bool checkIfDeadWithHealthManagement ()
	{
		return vehicleIsDestroyed ();
	}

	public override bool checkIfMaxHealthWithHealthManagement ()
	{
		return getCurrentHealthAmount () >= getMaxHealthAmount ();
	}

	public override void setDamageTargetOverTimeStateWithHealthManagement (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, 
	                                                                       float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{
		setDamageTargetOverTimeState (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath, damageTypeID);
	}

	public override void removeDamagetTargetOverTimeStateWithHealthManagement ()
	{
		stopDamageOverTime ();
	}

	public override void setHealWithHealthManagement (float healAmount)
	{
		getHealth (healAmount);
	}

	public void setHealthAmount (float newValue)
	{
		healthAmount = newValue;
	}

	public override float getCurrentHealthAmountWithHealthManagement ()
	{
		return healthAmount;
	}

	public override float getMaxHealthAmountWithHealthManagement ()
	{
		return getMaxHealthAmount ();
	}

	public override float getAuxHealthAmountWithHealthManagement ()
	{
		return getAuxHealthAmount ();
	}

	public override void addAuxHealthAmountWithHealthManagement (float amount)
	{
		addAuxHealthAmount (amount);
	}

	public override float getHealthAmountToPickWithHealthManagement (float amount)
	{
		return getHealthAmountToPick (amount);
	}

	public override void killCharacterWithHealthManagement ()
	{
		destroyVehicle ();
	}

	public override Transform getPlaceToShootWithHealthManagement ()
	{
		return placeToShoot;
	}

	public override GameObject getPlaceToShootGameObjectWithHealthManagement ()
	{
		return placeToShoot.gameObject;
	}

	public override bool isVehicleWithHealthManagement ()
	{
		return !vehicleIsDestroyed ();
	}

	public override GameObject getCharacterOrVehicleWithHealthManagement ()
	{
		return gameObject;
	}

	public override void setFuelWithHealthManagement (float fuelAmount)
	{
		getFuel (fuelAmount);
	}

	public override void removeFuelWithHealthManagement (float fuelAmount)
	{
		removeFuel (fuelAmount);
	}

	public override float getCurrentFuelAmountWithHealthManagement ()
	{
		return fuelAmount;
	}

	public override bool checkIfMaxFuelWithHealthManagement ()
	{
		return checkIfMaxFuel ();
	}

	public override GameObject getVehicleWithHealthManagement ()
	{
		return gameObject;
	}

	public override vehicleHUDManager getVehicleHUDManagerWithHealthManagement ()
	{
		return this;
	}

	public override GameObject getVehicleDriverWithHealthManagement ()
	{
		return getCurrentDriver ();
	}

	public override bool isVehicleBeingDrivenWithHealthManagement ()
	{
		return isVehicleBeingDriven ();
	}

	public override bool checkIfDetectSurfaceBelongToVehicleWithHealthManagement (Collider surfaceFound)
	{
		return checkIfDetectSurfaceBelongToVehicle (surfaceFound);
	}

	public override void setEnergyWithHealthManagement (float amount)
	{
		getEnergy (amount);
	}

	public override void removeEnergyWithHealthManagement (float amount)
	{
		removeEnergy (amount);
	}

	public override float getCurrentEnergyAmountWithHealthManagement ()
	{
		return getCurrentEnergyAmount ();
	}

	public override bool checkIfMaxEnergyWithHealthManagement ()
	{
		return checkIfMaxEnergy ();
	}

	public override int getDecalImpactIndexWithHealthManagement ()
	{
		return getDecalImpactIndex ();
	}

	public override bool isUseShieldActive ()
	{
		return false;
	}

	public override float getCurrentShieldAmount ()
	{
		return 0;
	}

	public override bool isDead ()
	{
		return destroyed;
	}

	//END OF HEALTH MANAGEMENT FUNCTIONS

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}

	void OnDrawGizmos ()
	{
		if (!advancedSettings.showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (advancedSettings.showGizmo && !Application.isPlaying) {
			//draw two spheres at both sides of the vehicles, to see where are launched two raycast to  
			//check if that side is not blocking by an object, so the player will get off in the other side, 
			//checking in the same way, so if both sides are blocked, the player won't get off
			//if there is not any obstacle, another ray is used to check the distance to the ground, so the player is placed at the side of the vehicle
			for (int i = 0; i < advancedSettings.damageReceiverList.Count; i++) {
				if (advancedSettings.damageReceiverList [i].spotTransform != null) {
					float rValue = 0;
					float gValue = 0;
					float bValue = 0;

					if (!advancedSettings.damageReceiverList [i].killedWithOneShoot) {
						rValue = advancedSettings.damageReceiverList [i].damageMultiplier / 10;
					} else {
						rValue = 1;
						gValue = 1;
					}

					Color gizmoColor = new Vector4 (rValue, gValue, bValue, advancedSettings.alphaColor);
					Gizmos.color = gizmoColor;
					Gizmos.DrawSphere (advancedSettings.damageReceiverList [i].spotTransform.position, advancedSettings.gizmoRadius);

					if (advancedSettings.damageReceiverList [i].vehicleDamageReceiverManager != null) {
						advancedSettings.damageReceiverList [i].vehicleDamageReceiverManager.damageMultiplier = advancedSettings.damageReceiverList [i].damageMultiplier;
					}
				}
			}

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere (transform.position, vehicleRadius);
		}
	}

	[System.Serializable]
	public class advancedSettingsClass
	{
		public List<damageReceiverInfo> damageReceiverList = new List<damageReceiverInfo> ();
		public bool showGizmo;
		public Color gizmoLabelColor;
		[Range (0, 1)] public float alphaColor;
		[Range (0, 1)] public float gizmoRadius;
	}

	[System.Serializable]
	public class damageReceiverInfo
	{
		public string name;
		public Transform spotTransform;
		[Range (1, 10)] public float damageMultiplier;

		public bool killedWithOneShoot;
		public bool needMinValueToBeKilled;
		public float minValueToBeKilled;

		public bool sendFunctionWhenDamage;
		public bool sendFunctionWhenDie;
		public UnityEvent damageFunction;

		public bool useHealthAmountOnSpot;
		public float healhtAmountOnSpot;
		public bool killCharacterOnEmtpyHealthAmountOnSpot;
		public UnityEvent eventOnEmtpyHealthAmountOnSpot;
		public bool healthAmountOnSpotEmtpy;

		public vehicleDamageReceiver vehicleDamageReceiverManager;
	}

	[System.Serializable]
	public class vehiclePartInfo
	{
		public GameObject vehiclePartGameObject;
		public Renderer mainRenderer;
		public Rigidbody mainRigidbody;
		public Collider mainCollider;

		public bool addCollider;
	}
}