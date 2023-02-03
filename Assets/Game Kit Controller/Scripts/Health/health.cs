using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class health : healthManagement
{
	public float healthAmount = 100;
	public float maxHealthAmount = 100;

	public bool generalDamageMultiplerEnabled = true;
	public bool generalDamageMultiplerActive;
	public float generalDamageMultiplier = 1;
	float originalGeneralDamageMultiplier;

	public bool regenerateHealth;
	public bool constantRegenerate;
	public float regenerateSpeed = 0;
	public float regenerateTime;
	public float regenerateAmount;
	public bool invincible = false;

	bool temporalInvincibilityActive;

	bool checkEventsOnTemporalInvincibilityActive;

	float lastTimeCheckEventsOnTemporalInvincibilityActive;

	public bool useEventOnDamageReceivedWithTemporalInvincibility;
	public float maxDelayBetweenDamageReceivedAndInvincibilityStateActive;
	public UnityEvent eventOnDamageReceivedWithTemporalInvincibility;

	public bool useEventsOnInvincibleStateChange;
	public UnityEvent eventOnInvicibleOn;
	public UnityEvent eventOnInvicibleOff;

	public bool dead = false;
	public GameObject damagePrefab;
	public Transform placeToShoot;
	public bool placeToShootActive = true;
	public GameObject scorchMarkPrefab = null;

	public bool useEventOnDamageEnabled = true;
	public UnityEvent eventOnDamage = new UnityEvent ();

	public bool useEventOnDamageWithAmount;
	public eventParameters.eventToCallWithAmount eventOnDamageWithAmount;

	public bool useEventOnDamageWithAttacker;
	public eventParameters.eventToCallWithGameObject eventOnDamageWithAttacker;

	public bool useExtraDamageFunctions;
	public float delayInDamageFunctions;
	public List<damageFunctionInfo> extraDamageFunctionList = new List<damageFunctionInfo> ();

	public bool useEventOnDamageShield;
	public UnityEvent eventOnDamageShield;

	public bool useEventOnDamageShieldWithAttacker;
	public eventParameters.eventToCallWithGameObject eventOnDamageShieldWithAttacker;

	public UnityEvent deadFuncionCall = new UnityEvent ();
	public UnityEvent extraDeadFunctionCall = new UnityEvent ();

	public UnityEvent resurrectFunctionCall;

	public bool useExtraDeadFunctions;
	public float delayInExtraDeadFunctions;

	public bool useEventWithAttackerOnDeath;
	public eventParameters.eventToCallWithGameObject eventWithAttackerOnDeath;

	public bool useShield;
	public shieldSystem mainShieldSystem;
	bool originalUseShieldState;

	public playerStatsSystem playerStatsManager;
	public string healthStatName = "Current Health";
	public string maxHealthStatName = "Max Health";
	bool hasPlayerStatsManager;

	public bool setHealthBarAsNotVisibleAtStart;

	public enemySettings settings = new enemySettings ();
	public advancedSettingsClass advancedSettings = new advancedSettingsClass ();

	public bool showWeakSpotsInScannerMode;
	public GameObject weakSpotMesh;
	[Range (0, 1)] public float weakSpotMeshAlphaValue;

	public Slider healthSlider;
	public Text healthSliderText;

	public bool hideHealthSliderWhenNotDamageReceived;
	public float timeToHideHealthSliderAfterDamage;
	public RectTransform mainHealthSliderParent;
	public RectTransform hiddenHealthSliderParent;
	public RectTransform mainHealthSliderTransform;

	Coroutine hideHealthSliderCoroutine;

	float lastTimeHideHealthSliderChecked;


	public bool useHealthSlideInfoOnScreen;

	public string enemyTag = "enemy";
	public string friendTag = "friend";

	public bool canBeSedated = true;
	public bool awakeOnDamageIfSedated;
	public bool sedateActive;
	public bool sedateUntilReceiveDamageState;

	public bool useEventOnSedate;
	public UnityEvent sedateStartEvent;
	public UnityEvent sedateEndEvent;

	public bool showSettings;
	public bool showAdvancedSettings;
	public bool showDamageDeadSettings;

	public bool resurrectAfterDelayEnabled;
	public float resurrectDelay;
	public UnityEvent eventToResurrectAfterDelay;

	bool pauseResurrectAfterDelay;

	public bool showDamageReceivedDebugInfo;

	List<GameObject> damageFromAttackersReceivedList = new List<GameObject> ();
	List<characterDamageReceiver> damageReceiverList = new List<characterDamageReceiver> ();
	public List<characterDamageReceiver> customDamageReceiverList = new List<characterDamageReceiver> ();

	bool damageFromAttackersReceived;

	public bool useEventOnHealthValueList;
	public List<eventOnHealthValue> eventOnHealthValueList = new List<eventOnHealthValue> ();
	eventOnHealthValue currentEventOnHealthValue;

	public bool useDamageTypeCheck;
	public List<damageTypeInfo> damageTypeInfoList = new List<damageTypeInfo> ();

	public bool blockDamageActive;
	public float blockDamageProtectionAmount;

	bool useMaxBlockRangeAngle;
	float maxBlockRangeAngle;

	public bool useEventsOnDamageBlocked;
	public eventParameters.eventToCallWithAmount eventOnDamageBlocked;

	public bool useDamageHitReaction;
	public damageHitReactionSystem mainDamageHitReactionSystem;

	public Transform debugDamageSourceTransform;

	public eventParameters.eventToCallWithAmount eventToSendCurrentHealthAmount;

	public bool objectIsCharacter = true;

	public bool sendInfoToCharacterCustomizationOnDamageEnabled;
	public inventoryCharacterCustomizationSystem mainInventoryCharacterCustomizationSystem;

	bool damageCompletelyBlocked;
	bool damageBlocked;

	GameObject scorchMark;
	ParticleSystem damageEffect;

	float auxHealthAmount;

	float lastDamageTime = 0;

	RaycastHit hit;
	Vector3 originalPlaceToShootPosition;

	string characterName;

	public string mainDecalManagerName = "Decal Manager";

	decalManager impactDecalManager;
	public string[] impactDecalList;
	public int impactDecalIndex;
	public string impactDecalName;
	public bool useImpactSurface;

	public bool checkDamageReceiverOnChildrenTransform;
	public characterDamageReceiver mainDamageReceiver;

	public playerController playerControllerManager;
	public ragdollActivator ragdollManager;
	public damageInScreen damageInScreenManager;

	public float healthAmountToTakeOnEditor;
	public float healthAmountToGiveOnEditor;

	public string mainManagerName = "Health Bar Manager";

	bool damageInScreeLocated;

	int lastWeakSpotIndex = -1;

	Coroutine damageOverTimeCoroutine;

	healthBarManagementSystem healthBarManager;

	bool hasHealthSlider;

	bool healthSliderRemoved;

	bool useMainHealthSlider;

	bool useHealthbarManager;

	weakSpot currentWeakSpot;

	public int currentID;

	public float currentSedateDuration;

	public bool receiveDamageEvenDead;

	bool receivingDamageAfterDeath;

	float maxShieldAmount;

	float lastTimeDead;

	float currentCriticalDamageProbability;

	bool originalInvincibleValue;

	bool playerControllerManagerLocated;

	bool initialized;

	string originalTag;


	void Awake ()
	{
		originalUseShieldState = useShield;

		if (playerControllerManager != null) {
			playerControllerManagerLocated = true;
		}
	}

	void Start ()
	{
		if (maxHealthAmount == 0) {
			maxHealthAmount = healthAmount;
		}

		//get the initial health assigned
		auxHealthAmount = healthAmount;

		originalTag = gameObject.tag;

		if (damagePrefab != null) {
			//if damage prefab has been assigned, instantiate the damage effect
			GameObject effect = (GameObject)Instantiate (damagePrefab, Vector3.zero, Quaternion.identity);

			effect.transform.SetParent (transform);

			effect.transform.localPosition = Vector3.zero;

			damageEffect = effect.GetComponent<ParticleSystem> ();
		}

		if (scorchMarkPrefab != null) {
			scorchMarkPrefab.SetActive (false);
		}
		 
		setShieldState (useShield);

		//instantiate a health slider in the UI, used for the enemies and allies

		if (healthSlider != null) {
			hasHealthSlider = true;

			healthSlider.maxValue = maxHealthAmount;
			healthSlider.value = healthAmount;

			if (healthSliderText != null) {
				healthSliderText.text = maxHealthAmount.ToString ("0");
			}

			useMainHealthSlider = true;

			if (hideHealthSliderWhenNotDamageReceived) {
				checkSetHealthSliderParent (false);
			}
		}

		initializeHealthBar ();

		bool addSingleDamageReceiver = true;

		//get all the damage receivers in the character
		if (checkDamageReceiverOnChildrenTransform) {
			Component[] damageReceivers = GetComponentsInChildren (typeof(characterDamageReceiver));
			if (damageReceivers.Length > 0) {
				foreach (characterDamageReceiver newReceiver in damageReceivers) {
				
					newReceiver.setCharacter (gameObject, this);

					damageReceiverList.Add (newReceiver);

					if (showWeakSpotsInScannerMode) {
						if (newReceiver.gameObject.GetComponent<Collider> () != null) {
							Transform spot = newReceiver.transform;
							GameObject newWeakSpotMesh = (GameObject)Instantiate (weakSpotMesh, spot.position, spot.rotation);

							newWeakSpotMesh.transform.SetParent (spot);
							newWeakSpotMesh.transform.localScale = Vector3.one;

							for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
								if (advancedSettings.weakSpots [i].spotTransform == spot) {
									Renderer meshRenderer = newWeakSpotMesh.GetComponent<Renderer> ();

									for (int k = 0; k < meshRenderer.materials.Length; k++) {
										Color newColor = advancedSettings.weakSpots [i].weakSpotColor;
										newColor = new Vector4 (newColor.r, newColor.g, newColor.b, weakSpotMeshAlphaValue);
										meshRenderer.materials [k].color = newColor;
									}
								}
							}
						}
					}
				}

				addSingleDamageReceiver = false;
			}
		} 

		if (addSingleDamageReceiver) {
			if (mainDamageReceiver == null) {
				mainDamageReceiver = GetComponent<characterDamageReceiver> ();

				if (mainDamageReceiver == null) {
					mainDamageReceiver = gameObject.AddComponent<characterDamageReceiver> ();
				}

				mainDamageReceiver.setCharacter (gameObject, this);
			}
		}

		if (placeToShootActive) {
			if (placeToShoot == null) {
				GameObject newPlaceToShoot = new GameObject ();

				newPlaceToShoot.name = "Place To Shoot";

				placeToShoot = newPlaceToShoot.transform;

				placeToShoot.SetParent (transform);

				placeToShoot.localPosition = Vector3.zero;
			}

			if (placeToShoot != null) {
				originalPlaceToShootPosition = placeToShoot.localPosition;
			}
		}

		if (playerStatsManager != null) {
			hasPlayerStatsManager = true;
		}

		originalGeneralDamageMultiplier = generalDamageMultiplier;

		updateSlider (healthAmount);

		originalInvincibleValue = invincible;

		damageInScreeLocated = damageInScreenManager != null;
	}

	void Update ()
	{
		if (!initialized) {
			if (useEventOnHealthValueList) {
				checkEventOnHealthValueList (true);
			}

			initialized = true;
		}

		//clear the list which contains the projectiles received by the vehicle
		if (damageFromAttackersReceived && damageFromAttackersReceivedList.Count > 0 && Time.time > lastDamageTime + 0.3f) {
			damageFromAttackersReceivedList.Clear ();

			damageFromAttackersReceived = false;
		}

		//if the object can regenerate, add health after a while with no damage
		if (!dead) {
			if (regenerateHealth) {
				if (constantRegenerate) {
					if (regenerateSpeed > 0 && healthAmount < maxHealthAmount) {
						if (Time.time > lastDamageTime + regenerateTime) {
							getHealth (regenerateSpeed * Time.deltaTime);
						}
					}
				} else {
					if (healthAmount < maxHealthAmount) {
						if (Time.time > lastDamageTime + regenerateTime) {
							getHealth (regenerateAmount);

							lastDamageTime = Time.time;
						}
					}
				}
			}

			if (useShield) {
				mainShieldSystem.updateSliderState ();
			}
		} else {
			if (resurrectAfterDelayEnabled && !pauseResurrectAfterDelay) {
				if (Time.time > resurrectDelay + lastTimeDead) {
					eventToResurrectAfterDelay.Invoke ();
				}
			}
		}
	}

	public void setUseEventOnDamageEnabledState (bool state)
	{
		useEventOnDamageEnabled = state;
	}

	public void setShieldState (bool state)
	{
		useShield = state;

		if (useShield) {
			if (mainShieldSystem != null) {
				mainShieldSystem.startShieldComponents ();

				maxShieldAmount = mainShieldSystem.maxShieldAmount;

				mainShieldSystem.setUseShieldState (true);
			}
		} else {
			if (mainShieldSystem != null) {
				mainShieldSystem.disableShield ();
			}
		}
	}

	public void setOriginalUseShieldState ()
	{
		setShieldState (originalUseShieldState);
	}

	public void setUseShieldState (bool state)
	{
		useShield = state;
	}

	public override bool isUseShieldActive ()
	{
		return useShield;
	}

	public void addDamageReceiversToRagdoll ()
	{
		if (advancedSettings.haveRagdoll) {
			Component[] components = GetComponentsInChildren (typeof(Collider));
			foreach (Collider child in components) {

				if (!child.isTrigger) {
					if (!child.gameObject.GetComponent<characterDamageReceiver> ()) {
						characterDamageReceiver newReceiver = child.gameObject.AddComponent<characterDamageReceiver> ();
						newReceiver.setCharacter (gameObject, this);

						customDamageReceiverList.Add (newReceiver);
					}
				}
			}

			customDamageReceiverList.Clear ();

			Component[] damageReceivers = GetComponentsInChildren (typeof(characterDamageReceiver));
			foreach (characterDamageReceiver newReceiver in damageReceivers) {
				customDamageReceiverList.Add (newReceiver);
			}

			print ("Damage receivers added to ragdoll");

			updateComponent ();

			print (customDamageReceiverList.Count);
		}
	}

	public void removeDamageReceiversFromRagdoll ()
	{
		if (advancedSettings.haveRagdoll) {
			for (int i = 0; i < customDamageReceiverList.Count; i++) {
				if (customDamageReceiverList [i] != null) {
					DestroyImmediate (customDamageReceiverList [i]);
				}
			}
			customDamageReceiverList.Clear ();

			print ("Damage receivers removed from ragdoll");

			updateComponent ();
		}
	}

	//receive a certain amount of damage
	public void setDamage (float damageAmount, Vector3 fromDirection, Vector3 damagePosition, GameObject attacker, 
	                       GameObject projectile, bool damageConstant, bool searchClosestWeakSpot, bool ignoreShield, 
	                       bool ignoreDamageInScreen, bool damageCanBeBlocked, bool canActivateReactionSystemTemporally,
	                       int damageReactionID, int damageTypeID)
	{
		//if the objects is not dead, invincible or its health is zero, exit
		bool checkDamage = true;

		if (invincible && !receivingDamageOverTimeActive) {
			checkDamage = false;
		}

		if (dead && !receiveDamageEvenDead) {
			checkDamage = false;
		}

		if (damageAmount <= 0) {
			if (damageTypeID == -1 && checkDamage) {
				checkDamage = false;
			}
		}

		if (!checkDamage) {
			if (useEventOnDamageReceivedWithTemporalInvincibility) {
				if (!dead && invincible && temporalInvincibilityActive && checkEventsOnTemporalInvincibilityActive) {
					if (Time.time < maxDelayBetweenDamageReceivedAndInvincibilityStateActive + lastTimeCheckEventsOnTemporalInvincibilityActive) {
						eventOnDamageReceivedWithTemporalInvincibility.Invoke ();
					}
				}
			}

			return;
		}

		damageCompletelyBlocked = false;

		float originalDamageAmountReceived = damageAmount;

		if (blockDamageActive) {
			if (damageCanBeBlocked) {
				damageBlocked = false;

				if (blockDamageProtectionAmount < 1 || useMaxBlockRangeAngle) {
					if (useMaxBlockRangeAngle) {
						Vector3 damageDirection = Vector3.zero;

						float distanceBetweenAttackAndAttacker = 0;

						Vector3 attackerPosition = Vector3.zero;

						if (attacker != null) {
							attackerPosition = attacker.transform.position;
						} else {
							attackerPosition = transform.position + transform.forward;
						}

						distanceBetweenAttackAndAttacker = GKC_Utils.distance (damagePosition, attackerPosition);

						if (distanceBetweenAttackAndAttacker < 2.5f) {
							Vector3 directionFromAttacker = damagePosition - attackerPosition;
							directionFromAttacker = directionFromAttacker / directionFromAttacker.magnitude;

							damageDirection = (damagePosition - (directionFromAttacker * 1.2f)) - transform.position;
						} else {
							damageDirection = damagePosition - transform.position;
						}

						damageDirection = damageDirection / damageDirection.magnitude;

						float damageAngle = Vector3.SignedAngle (transform.forward, damageDirection, transform.up);

						float damageAngleAbs = Mathf.Abs (damageAngle);

//					print (distanceBetweenAttackAndAttacker + " " + damageAngle + " " + damageAngleAbs);

						if (damageAngleAbs < maxBlockRangeAngle / 2) {
							if (blockDamageProtectionAmount < 1) {
								damageAmount = damageAmount * (1 - blockDamageProtectionAmount);

								damageBlocked = true;
							} else {
								damageCompletelyBlocked = true;

								damageAmount = 0;
							}
						}
					} else {
						damageAmount = damageAmount * (1 - blockDamageProtectionAmount);

						damageBlocked = true;
					}
				} else {
					damageCompletelyBlocked = true;

					damageAmount = 0;
				}

				if (useDamageHitReaction) {
					mainDamageHitReactionSystem.checkDamageBlocked (damageAmount, originalDamageAmountReceived, damagePosition, damageBlocked, attacker);
				}

				if (damageCompletelyBlocked) {
					return;
				}
			} else {
				mainDamageHitReactionSystem.checkDamageReceivedUnblockable (damageAmount, damagePosition, attacker);
			}
		}

		if (dead && receiveDamageEvenDead) {
			receivingDamageAfterDeath = true;
		} else {
			receivingDamageAfterDeath = false;
		}

		if (!damageConstant) {
			//if the projectile is not a laser, store it in a list
			//this is done like this because you can add as many colliders (box or mesh) as you want (according to the vehicle meshes), 
			//which are used to check the damage received by every character, so like this the damage detection is really accurated. 
			//For example, if you shoot a grenade to a character, every collider will receive the explosion, but the character will only be damaged once, with the correct amount.
			//in this case the projectile has not produced damage yet, so it is stored in the list and in the below code the damage is applied. 
			//This is used for bullets for example, which make damage only in one position
			if (!damageFromAttackersReceivedList.Contains (projectile)) {
				damageFromAttackersReceivedList.Add (projectile);

				damageFromAttackersReceived = true;
			} 
			//in this case the projectile has been added to the list previously, it means that the projectile has already applied damage to the vehicle, 
			//so it can't damaged the vehicle twice. This is used for grenades for example, which make a damage inside a radius
			else {
				return;
			}
		}

		//if any elememnt in the list of current projectiles received no longer exits, remove it from the list
		for (int i = damageFromAttackersReceivedList.Count - 1; i >= 0; i--) {
			if (damageFromAttackersReceivedList [i] == null) {
				damageFromAttackersReceivedList.RemoveAt (i);
			}
		}

		if (damageFromAttackersReceivedList.Count == 0) {
			damageFromAttackersReceived = false;
		}

		if (playerControllerManagerLocated && playerControllerManager.isPlayerDriving () && !playerControllerManager.isDrivingRemotely ()) {
			return;
		}

		bool shieldTakesDamage = false;

		if (!ignoreShield) {
			if (useShield) {
				float shieldAmountBeforeDamage = mainShieldSystem.getCurrentShieldAmount ();

				shieldTakesDamage = mainShieldSystem.receiveDamage (damageAmount, attacker);

				if (shieldTakesDamage) {
					float shieldAmountAfterDamage = mainShieldSystem.getCurrentShieldAmount ();

					if (shieldAmountAfterDamage <= 0) {
						if (showDamageReceivedDebugInfo) {
							print ("the damage received has been higher " + damageAmount + " than the remaining shield amount " + shieldAmountBeforeDamage);
						}

						damageAmount -= shieldAmountBeforeDamage;

						shieldTakesDamage = false;
					}
				}
			}
		}

		currentCriticalDamageProbability = 0;

		bool disableDamageReactionOnDamageType = false;

		if (!shieldTakesDamage) {

			if (useDamageTypeCheck) {
				if (damageTypeID > 0) {
					int damageTypeIDIndex = damageTypeInfoList.FindIndex (s => s.damageTypeID == damageTypeID);

					if (damageTypeIDIndex > -1) {
						damageTypeInfo currentDamageTypeInfo = damageTypeInfoList [damageTypeIDIndex];

						if (currentDamageTypeInfo.damageTypeEnabled) {
							bool activateDamageType = true;

							if (currentDamageTypeInfo.avoidDamageTypeIfBlockDamageActive && damageBlocked) {
								activateDamageType = false;
							}

							if (showDamageReceivedDebugInfo) {
								print (currentDamageTypeInfo.Name + " " + activateDamageType);
							}

							if (activateDamageType) {
								float damageTypeResistance = currentDamageTypeInfo.damageTypeResistance;

								if (damageTypeResistance > 0) {
									damageAmount -= damageAmount * (1 / damageTypeResistance);

									if (damageAmount < 0) {
										damageAmount = 0;
									}
								} else if (damageTypeResistance < 0) {
									damageAmount += damageAmount * (1 + Mathf.Abs (damageTypeResistance));
								}

								if (currentDamageTypeInfo.useEventOnDamageType) {
									currentDamageTypeInfo.eventOnDamageType.Invoke ();
								}

								if (currentDamageTypeInfo.obtainHealthOnDamageType) {
									getHealth (currentDamageTypeInfo.healthMultiplierOnDamageType * damageAmount);

									if (currentDamageTypeInfo.useEventOnObtainHealthOnDamageType) {
										currentDamageTypeInfo.eventOnObtainHealthOnDamageType.Invoke ();
									}

									if (currentDamageTypeInfo.stopDamageCheckIfHealthObtained) {
										return;
									} else {
										damageAmount = 0;
									}
								}

								if (currentDamageTypeInfo.disableDamageReactionOnDamageType) {
									disableDamageReactionOnDamageType = true;
								}
							}
						}
					}
				}
			}

			if (showDamageReceivedDebugInfo) {
				print (gameObject.name + " receives " + damageAmount + " of initial damage by " + attacker.name);
			}

			currentWeakSpot = new weakSpot ();

			if (advancedSettings.useWeakSpots && searchClosestWeakSpot) {
				if (advancedSettings.weakSpots.Count > 0) {
					int weakSpotIndex = getClosesWeakSpotIndex (damagePosition);

					lastWeakSpotIndex = weakSpotIndex;

					if (showDamageReceivedDebugInfo) {
						print (advancedSettings.weakSpots [weakSpotIndex].name + " weak spot detected with a damage multiplier of " + advancedSettings.weakSpots [weakSpotIndex].damageMultiplier);
					}

					if (damageAmount < healthAmount || receivingDamageAfterDeath) {
						currentWeakSpot = advancedSettings.weakSpots [weakSpotIndex];

						if (currentWeakSpot.killedWithOneShoot) {
							if (currentWeakSpot.needMinValueToBeKilled) {
								if (currentWeakSpot.minValueToBeKilled < damageAmount) {
									damageAmount = healthAmount;
								}
							} else {
								damageAmount = healthAmount;
							}
						} else {
							if (!advancedSettings.notHuman) {
								//print (advancedSettings.weakSpots [weakSpotIndex].damageMultiplier + " " + amount);
								damageAmount *= currentWeakSpot.damageMultiplier;
							}
						}
					}

					if (currentWeakSpot.useCriticalDamageSpot) {
						currentCriticalDamageProbability = Random.Range (0, 101);

						if (showDamageReceivedDebugInfo) {
							print (currentCriticalDamageProbability);
						}

						if (currentCriticalDamageProbability > currentWeakSpot.criticalDamageProbability.x &&
						    currentCriticalDamageProbability < currentWeakSpot.criticalDamageProbability.y) {
							currentCriticalDamageProbability = 1;
						}
					}

					if (advancedSettings.useHealthAmountOnSpotEnabled) {
						if (currentWeakSpot.useHealthAmountOnSpot && !currentWeakSpot.healthAmountOnSpotEmpty) {
							currentWeakSpot.healhtAmountOnSpot -= damageAmount;

							if (currentCriticalDamageProbability == 1) {
								if (currentWeakSpot.removeAllHealthAmountOnSpotOnCritical) {
									currentWeakSpot.healhtAmountOnSpot = 0;
								}
							}

							if (currentWeakSpot.healhtAmountOnSpot <= 0) {
								currentWeakSpot.eventOnEmtpyHealthAmountOnSpot.Invoke ();

								currentWeakSpot.healthAmountOnSpotEmpty = true;

								if (currentWeakSpot.killCharacterOnEmtpyHealthAmountOnSpot) {
									damageAmount = healthAmount;
								}
							}
						}
					}

					if (currentCriticalDamageProbability == 1) {
						if (currentWeakSpot.killTargetOnCritical) {
							damageAmount = healthAmount;
						} else {
							if (currentWeakSpot.damageMultiplierOnCritical > 0) {
								damageAmount *= currentWeakSpot.damageMultiplierOnCritical;
							}
						}
					}

					if (sendInfoToCharacterCustomizationOnDamageEnabled) {
						if (currentWeakSpot.sendValueToArmorClothSystemOnDamage) {
							float newDurabilityValue = -damageAmount * currentWeakSpot.damageMultiplierOnArmorClothPiece;

							if (newDurabilityValue != 0) {
								mainInventoryCharacterCustomizationSystem.addOrRemoveDurabilityAmountToObjectByCategoryName (
									currentWeakSpot.armorClothCategoryName, newDurabilityValue, false);
							}
						}
					}
				}
			}

			if (generalDamageMultiplerEnabled) {
				if (generalDamageMultiplerActive) {
					if (generalDamageMultiplier != 1) {
						if (showDamageReceivedDebugInfo) {
							print (gameObject.name + " receives " + damageAmount + " but damage multiplier is " + generalDamageMultiplier +
							" ,so final damage is " + (damageAmount * generalDamageMultiplier));
						}

						damageAmount *= generalDamageMultiplier;
					}
				}
			}

			if (damageAmount > healthAmount) {
				damageAmount = healthAmount;
			}

			if (showDamageReceivedDebugInfo) {
				print (gameObject.name + " receives " + damageAmount + " of final damage by " + attacker.name);
			}

			//active the damage prefab, substract the health amount, and set the value in the slider
			healthAmount -= damageAmount;
			auxHealthAmount = healthAmount;

			updateSlider (healthAmount);
		}

		if (!receivingDamageAfterDeath && !shieldTakesDamage) {
			checkEventOnHealthValueList (true);
		}

		if (!receivingDamageAfterDeath) {
			if (damageInScreeLocated && !ignoreDamageInScreen) {
				if (damageAmount > 0) {
					damageInScreenManager.showScreenInfo (damageAmount, true, fromDirection, healthAmount, currentCriticalDamageProbability);
				}
			}

			lastDamageTime = Time.time;

			if (!shieldTakesDamage) {
				//call a function when the object receives damage
				if (useEventOnDamageEnabled) {
					eventOnDamage.Invoke ();
				}

				if (useEventOnDamageWithAttacker) {
					eventOnDamageWithAttacker.Invoke (attacker);
				}

				if (useEventOnDamageWithAmount) {
					eventOnDamageWithAmount.Invoke (damageAmount);
				}
				
				if (damageEffect != null) {
					//set the position of the damage in the position where the projectile hitted the object with the health component
					damageEffect.transform.position = damagePosition;
					damageEffect.transform.rotation = Quaternion.LookRotation (fromDirection, Vector3.up);
					damageEffect.Play ();
				}
		
				//if the health reachs 0, call the dead function
				if (healthAmount <= 0) {
					healthAmount = 0;

					if (useEventWithAttackerOnDeath) {
						eventWithAttackerOnDeath.Invoke (attacker);
					}

					removeHealthSlider ();

					dead = true;

					if (useShield) {
						mainShieldSystem.setDeadState (true);
					}

					lastTimeDead = Time.time;

					deadFuncionCall.Invoke ();

					if (advancedSettings.haveRagdoll && ragdollManager != null) {
						ragdollManager.deathDirection (-fromDirection);

						ragdollManager.die (damagePosition);
					}

					//check the map icon
					if (!gameObject.CompareTag ("Player")) {
						mapObjectInformation currentMapObjectInformation = GetComponent<mapObjectInformation> ();

						if (currentMapObjectInformation != null) {
							currentMapObjectInformation.removeMapObject ();
						}
					}

					if (useExtraDeadFunctions) {
						callExtraDeadFunctions ();
					}

					if (scorchMarkPrefab != null) {
						//if the object is an enemy, set an scorch below the enemy, using a raycast
						scorchMarkPrefab.SetActive (true);

						scorchMarkPrefab.transform.SetParent (null);

						RaycastHit hit;

						if (Physics.Raycast (transform.position, transform.up * (-1), out hit, 200, settings.layer)) {
							if ((1 << hit.collider.gameObject.layer & settings.layer.value) == 1 << hit.collider.gameObject.layer) {
								Vector3 scorchPosition = hit.point;

								scorchMarkPrefab.transform.position = scorchPosition + hit.normal * 0.03f;
							}
						}
					}

					if (invincible) {
						invincible = false;

						temporalInvincibilityActive = false;

						checkEventsOnTemporalInvincibilityActive = false;

						stopDamageOverTime ();
					}

					applyDamage.checkIfPlayerIsLookingAtDeadTarget (transform, placeToShoot);
				} else {
					if (advancedSettings.haveRagdoll && advancedSettings.activateRagdollOnDamageReceived) {
						if (damageAmount >= advancedSettings.minDamageToEnableRagdoll) {
							advancedSettings.ragdollEvent.Invoke ();
						}
					}

					if (useExtraDamageFunctions) {
						for (int i = 0; i < extraDamageFunctionList.Count; i++) {
							if (extraDamageFunctionList [i].damageRecived >= damageAmount) {
								extraDamageFunctionList [i].damageFunctionCall.Invoke ();
							}
						}
					}

					if (!disableDamageReactionOnDamageType) {
						if (useDamageHitReaction && !blockDamageActive) {
							if (damageReactionID > -1) {
								mainDamageHitReactionSystem.activateDamageReactionByID (damageAmount, damagePosition, attacker, damageReactionID);
							} else if (damageReactionID > -2) {
								if (canActivateReactionSystemTemporally) {

									mainDamageHitReactionSystem.checkDamageReceivedTemporally (damageAmount, damagePosition, attacker);

									mainDamageHitReactionSystem.checkDamageStateTemporally (originalDamageAmountReceived, false);
								} else {
									mainDamageHitReactionSystem.checkDamageReceived (damageAmount, damagePosition, attacker);
								}
							}
						}
					}

					if (blockDamageActive) {
						if (useEventsOnDamageBlocked) {
							eventOnDamageBlocked.Invoke (damageAmount);
						}
					}				
				}
			} else {
				if (useEventOnDamageShield) {
					eventOnDamageShield.Invoke ();

					if (useEventOnDamageShieldWithAttacker) {
						eventOnDamageShieldWithAttacker.Invoke (attacker);
					}
				}

				if (useDamageHitReaction && !blockDamageActive) {
					if (damageReactionID > -1) {
						mainDamageHitReactionSystem.activateDamageReactionByID (damageAmount, damagePosition, attacker, damageReactionID);
					} else if (damageReactionID > -2) {
						if (canActivateReactionSystemTemporally) {
						
							mainDamageHitReactionSystem.checkDamageStateTemporally (originalDamageAmountReceived, true);
						}
					}
				}
			}
		}

		if (!shieldTakesDamage) {
			//call functions from weak spots when they are damaged
			if (advancedSettings.useWeakSpots && lastWeakSpotIndex > -1 && searchClosestWeakSpot) {
				bool callFunction = false;

				currentWeakSpot = advancedSettings.weakSpots [lastWeakSpotIndex];

				if (currentWeakSpot.sendFunctionWhenDamage) {
					callFunction = true;
				}

				if (currentWeakSpot.sendFunctionWhenDie && dead) {
					callFunction = true;
				}

				//print (advancedSettings.weakSpots [lastWeakSpotIndex].name +" " +callFunction +" "+ dead);
				if (callFunction) {
					if (showDamageReceivedDebugInfo) {
						print ("call event on weak spot " + currentWeakSpot.name);
					}

					currentWeakSpot.damageFunction.Invoke ();
				}

				lastWeakSpotIndex = -1;
			}
		}

		if (!receivingDamageAfterDeath) {
			if (sedateActive && (awakeOnDamageIfSedated || sedateUntilReceiveDamageState)) {
				if (ragdollManager != null) {
					ragdollManager.stopSedateStateCoroutine ();
				}
			}
		}
	}

	public void setDamageReactionPausedState (bool state)
	{
		mainDamageHitReactionSystem.setDamageReactionPausedState (state);
	}

	public void checkEventOnHealthValueList (bool receivingDamage)
	{
		if (useEventOnHealthValueList) {
			for (int i = 0; i < eventOnHealthValueList.Count; i++) {

				currentEventOnHealthValue = eventOnHealthValueList [i];

				if (currentEventOnHealthValue.eventEnabled) {
					if ((receivingDamage && currentEventOnHealthValue.useEventOnDamageReceived) || (!receivingDamage && !currentEventOnHealthValue.useEventOnDamageReceived)) {
						if (!currentEventOnHealthValue.eventActivated || !currentEventOnHealthValue.callEventOnce) {

							bool canActivateState = false;

							if (receivingDamage) {
								if (currentEventOnHealthValue.useDamagePercentage) {
									float totalPercentage = (healthAmount / maxHealthAmount) * 100;

									if (totalPercentage < currentEventOnHealthValue.minDamageToReceive) {
										canActivateState = true;
									}
								} else {
									if (healthAmount < currentEventOnHealthValue.minDamageToReceive) {
										canActivateState = true;
									}
								}
							} else {
								if (currentEventOnHealthValue.useDamagePercentage) {
									float totalPercentage = (healthAmount / maxHealthAmount) * 100;

									if (totalPercentage >= currentEventOnHealthValue.minDamageToReceive) {
										canActivateState = true;
									}
								} else {
									if (healthAmount >= currentEventOnHealthValue.minDamageToReceive) {
										canActivateState = true;
									}
								}
							}

							if (canActivateState) {
								currentEventOnHealthValue.eventActivated = true;

								currentEventOnHealthValue.eventToCall.Invoke ();
							}
						}
					}
				}
			}
		}
	}

	public void setEventOnHealthValueEnabledState (bool state, string stateName)
	{
		for (int i = 0; i < eventOnHealthValueList.Count; i++) {
			if (eventOnHealthValueList [i].Name.Equals (stateName)) {
				eventOnHealthValueList [i].eventEnabled = state;

				return;
			}
		}
	}

	public void setEventOnHealthValueEnabledState (string stateName)
	{
		setEventOnHealthValueEnabledState (true, stateName);
	}

	public void setEventOnHealthValueDisabledState (string stateName)
	{
		setEventOnHealthValueEnabledState (false, stateName);
	}

	public void setEventOnHealthValueEnabledStateFromEditor (string stateName)
	{
		setEventOnHealthValueEnabledState (true, stateName);

		updateComponent ();
	}

	public void setEventOnHealthValueDisabledStateFromEditor (string stateName)
	{
		setEventOnHealthValueEnabledState (false, stateName);

		updateComponent ();
	}

	public float getLastDamageTime ()
	{
		return lastDamageTime;
	}

	public void damageSpot (int elementIndex, float damageAmount)
	{
		Vector3 damagePosition = Vector3.zero;

		if (advancedSettings.weakSpots [elementIndex].spotTransform != null) {
			damagePosition = advancedSettings.weakSpots [elementIndex].spotTransform.position;
		} else {
			damagePosition = transform.position;
		}

		setDamage (damageAmount, transform.forward, damagePosition, gameObject, gameObject, false, true, false, false, true, false, -1, -1);
	}

	public void checkIfEnabledStateCanChange (bool state)
	{
		if (!receiveDamageEvenDead) {
			enabled = state;
		}
	}

	public void setDamageTargetOverTimeToDeath (float damageOverTimeAmount)
	{
		setDamageTargetOverTimeState (0.5f, 1, damageOverTimeAmount, 0.4f, true, -1);
	}

	public void setDamageTargetOverTimeState (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{
		stopDamageOverTime ();

		damageOverTimeCoroutine = StartCoroutine (setDamageTargetOverTimeStateCoroutine (damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, damageOverTimeRate, damageOverTimeToDeath, damageTypeID));
	}

	bool receivingDamageOverTimeActive;

	IEnumerator setDamageTargetOverTimeStateCoroutine (float damageOverTimeDelay, float damageOverTimeDuration, float damageOverTimeAmount, 
	                                                   float damageOverTimeRate, bool damageOverTimeToDeath, int damageTypeID)
	{
		receivingDamageOverTimeActive = true;

		yield return new WaitForSeconds (damageOverTimeDelay);

		float lastTimeDamaged = Time.time;
		float lastTimeDuration = lastTimeDamaged;

		while (Time.time < lastTimeDuration + damageOverTimeDuration || (!dead && damageOverTimeToDeath)) {
			if (Time.time > lastTimeDamaged + damageOverTimeRate) {
				lastTimeDamaged = Time.time;

				setDamage (damageOverTimeAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, 
					gameObject, true, false, false, false, false, false, -1, damageTypeID);
			}

			yield return null;
		}

		receivingDamageOverTimeActive = false;
	}

	public void stopDamageOverTime ()
	{
		if (damageOverTimeCoroutine != null) {
			StopCoroutine (damageOverTimeCoroutine);
		}

		receivingDamageOverTimeActive = false;
	}

	public void sedateCharacter (Vector3 position, float sedateDelay, bool useWeakSpotToReduceDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{
		if (advancedSettings.haveRagdoll && ragdollManager != null && canBeSedated) {
			if (useWeakSpotToReduceDelay) {
				if (advancedSettings.weakSpots.Count > 0) {
					int weakSpotIndex = getClosesWeakSpotIndex (position);

					sedateDelay -= advancedSettings.weakSpots [weakSpotIndex].damageMultiplier;
					if (sedateDelay < 0) {
						sedateDelay = 0;
					}
				}
			}

			sedateUntilReceiveDamageState = sedateUntilReceiveDamage;

			if (sedateUntilReceiveDamage) {
				currentSedateDuration = 0;
			} else {
				currentSedateDuration = sedateDuration;
			}

			ragdollManager.sedateCharacter (sedateDelay, sedateUntilReceiveDamage, sedateDuration);
		}
	}

	public void sedateCharacter (float sedateDuration)
	{
		sedateCharacter (transform.position, 0, false, false, sedateDuration);
	}

	public void sedateCharacterUntilReceiveDamage (float sedateDuration)
	{
		sedateCharacter (transform.position, 0, false, true, sedateDuration);
	}

	public float getCurrentSedateDuration ()
	{
		return currentSedateDuration;
	}

	public void setSedateState (bool state)
	{
		sedateActive = state;

		if (dead) {
			return;
		}

		if (useEventOnSedate) {
			if (sedateActive) {
				sedateStartEvent.Invoke ();
			} else {
				sedateEndEvent.Invoke ();
			}
		}
	}

	public bool receiveDamageFromCollisionsEnabled;

	public float minTimeToReceiveDamageOnImpact;

	public float minVelocityToReceiveDamageOnImpact;

	public float receiveDamageOnImpactMultiplier;

	float lastTimeDamageOnImpact;

	public bool isReceiveDamageFromCollisionsEnabled ()
	{
		return receiveDamageFromCollisionsEnabled;
	}

	public void setImpactReceivedInfo (Vector3 impactVelocity)
	{
		if (receiveDamageFromCollisionsEnabled) {
			if (Time.time > lastTimeDamageOnImpact + minTimeToReceiveDamageOnImpact) {
				float currentImpactVelocity = impactVelocity.magnitude;

				float currentImpactVelocityABS = Mathf.Abs (impactVelocity.magnitude);

				if (currentImpactVelocityABS > minVelocityToReceiveDamageOnImpact) {
					takeConstantDamage (currentImpactVelocity * receiveDamageOnImpactMultiplier);

					lastTimeDamageOnImpact = Time.time;
				}
			}
		}
	}

	public void setRagdollImpactReceivedInfo (Vector3 impactVelocity, Collider impactCollider)
	{
		if (advancedSettings.haveRagdoll && ragdollManager != null) {
			ragdollManager.setImpactReceivedInfo (impactVelocity, impactCollider);
		}
	}

	public bool characterIsOnRagdollState ()
	{
		if (advancedSettings.haveRagdoll && ragdollManager != null) {
			return ragdollManager.isRagdollActive ();
		}

		return false;
	}

	public ragdollActivator getMainRagdollActivator ()
	{
		return ragdollManager;
	}

	public bool canRagdollReceiveDamageOnImpact ()
	{
		return advancedSettings.ragdollCanReceiveDamageOnImpact;
	}

	public void updateSlider (float value)
	{
		if (hasHealthSlider) {
			if (useMainHealthSlider) {
				healthSlider.value = value;

				if (healthSliderText != null) {
					healthSliderText.text = value.ToString ("0");
				}

				checkHideHealthSliderParent ();
			}

			if (useHealthbarManager) {
				healthBarManager.setSliderAmount (currentID, value); 
			}
		}

		if (hasPlayerStatsManager) {
			playerStatsManager.updateStatValue (healthStatName, healthAmount);
		}
	}

	public void updatePlayerStatsManagerValue ()
	{
		if (playerStatsManager != null) {
			playerStatsManager.updateStatValue (healthStatName, healthAmount);
		}
	}

	public void updatePlayerStatesManagerMaxHealthValue ()
	{
		if (playerStatsManager != null) {
			playerStatsManager.updateStatValue (maxHealthStatName, maxHealthAmount);
		}
	}

	public void updateSliderMaxValue (float newMaxValue)
	{
		if (hasHealthSlider) {
			if (useMainHealthSlider) {
				healthSlider.maxValue = newMaxValue;

				checkHideHealthSliderParent ();
			}

			if (useHealthbarManager) {
				healthBarManager.updateSliderMaxValue (currentID, newMaxValue); 
			}
		}
	}

	public void removeHealthSlider ()
	{
		if (hasHealthSlider && !healthSliderRemoved) {
			if (useHealthbarManager && settings.removeHealthBarSliderOnDeath) {
				healthBarManager.removeTargetSlider (currentID); 

				healthSliderRemoved = true;
			}
		}
	}

	public void checkHideHealthSliderParent ()
	{
		if (hideHealthSliderWhenNotDamageReceived) {
			stopCheckHideHealthSliderCoroutine ();

			lastTimeHideHealthSliderChecked = Time.time;

			hideHealthSliderCoroutine = StartCoroutine (checkHideHealthSliderCoroutine ());
		}
	}

	void stopCheckHideHealthSliderCoroutine ()
	{
		if (hideHealthSliderCoroutine != null) {
			StopCoroutine (hideHealthSliderCoroutine);
		}
	}

	IEnumerator checkHideHealthSliderCoroutine ()
	{
		checkSetHealthSliderParent (true);

		bool targetReached = false;

		while (!targetReached) {

			if (Time.time > timeToHideHealthSliderAfterDamage + lastTimeHideHealthSliderChecked) {
				targetReached = true;
			}

			if (isPlayerMenuActive ()) {
				targetReached = true;
			}

			yield return null;
		}

		checkSetHealthSliderParent (false);
	}

	public bool isPlayerMenuActive ()
	{
		if (playerControllerManager != null) {
			return playerControllerManager.isPlayerMenuActive ();
		}

		return false;
	}

	void checkSetHealthSliderParent (bool setOnMainParent)
	{
		if (hideHealthSliderWhenNotDamageReceived) {
			if (setOnMainParent) {
				mainHealthSliderTransform.transform.SetParent (mainHealthSliderParent);
			} else {
				mainHealthSliderTransform.transform.SetParent (hiddenHealthSliderParent);

			}

			mainHealthSliderTransform.transform.localPosition = Vector3.zero;
			mainHealthSliderTransform.transform.localRotation = Quaternion.identity;
		}
	}

	void OnDestroy ()
	{
		if (GKC_Utils.isApplicationPlaying () && Time.deltaTime > 0) {
//			print ("DESTROYYYYYY health checking if playing");

			if (!healthSliderRemoved) {
				removeHealthSlider ();
			}
		}
	}

	public void getHealth (float amount)
	{
		if (damageInScreeLocated) {
			damageInScreenManager.showScreenInfo (amount, false, Vector3.zero, healthAmount, 0);
		}

		addHealth (amount);
	}

	public void addHealth (float amount)
	{
		if (!dead) {
			healthAmount += amount;
			//check that the health amount is not higher that the health max value of the slider
			if (healthAmount >= maxHealthAmount) {
				healthAmount = maxHealthAmount;
			}

			updateSlider (healthAmount);

			checkEventOnHealthValueList (false);
		}

		auxHealthAmount = healthAmount;
	}

	public void setHealthAmountOnMaxValue ()
	{
		addHealth (maxHealthAmount - healthAmount);
	}

	public void clampHealthAmountToMaxHealth ()
	{
		healthAmount = Mathf.Clamp (healthAmount, 0, maxHealthAmount);

		updateSlider (healthAmount);
	}

	//if an enemy becomes an ally, set its name and its slider color
	public void setSliderInfo (string name, Color color)
	{
		if (hasHealthSlider) {
			healthBarManager.setSliderInfo (currentID, name, settings.nameTextColor, color); 

			characterName = name;
		}
	}

	public void updateNameWithAlly ()
	{
		setSliderInfo (settings.allyName, settings.allySliderColor);
	}

	public void updateNameWithEnemy ()
	{
		setSliderInfo (settings.enemyName, settings.enemySliderColor);
	}

	public void hacked ()
	{
		setSliderInfo (settings.allyName, settings.allySliderColor);
	}

	//restart the health component of the object
	public void resurrect ()
	{
		healthAmount = maxHealthAmount;

		dead = false;

		lastTimeDead = Time.time;

		updateSlider (healthAmount);

		resurrectFunctionCall.Invoke ();

		checkEventOnHealthValueList (false);

		initializeHealthBar ();
	
		if (useShield) {
			mainShieldSystem.setDeadState (false);
		}
	}

	public void setResurrectAfterDelayEnabledState (bool state)
	{
		resurrectAfterDelayEnabled = state;
	}

	public void setResurrectAfterDelayEnabledStateFromEditor (bool state)
	{
		resurrectAfterDelayEnabled = state;

		updateComponent ();
	}

	public void setPauseResurrectCheckState (bool state)
	{
		pauseResurrectAfterDelay = state;

		lastTimeDead = Time.time;
	}

	public void resurrectFromExternalCall ()
	{
		if (dead) {
			eventToResurrectAfterDelay.Invoke ();
		}
	}

	void initializeHealthBar ()
	{
		if (settings.useHealthSlider && settings.enemyHealthSlider != null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(healthBarManagementSystem));

			healthBarManager = FindObjectOfType<healthBarManagementSystem> ();

			if (healthBarManager != null) {
				Color sliderColor = settings.allySliderColor;

				if (originalTag.Equals (enemyTag)) {
					characterName = settings.enemyName;
					sliderColor = settings.enemySliderColor;
				} else {
					characterName = settings.allyName;
				}

				if (setHealthBarAsNotVisibleAtStart) {
					settings.healthBarSliderActiveOnStart = true;
				}

				currentID = healthBarManager.addNewTargetSlider (gameObject, settings.enemyHealthSlider, settings.sliderOffset, maxHealthAmount, maxShieldAmount, 
					characterName, settings.nameTextColor, sliderColor, settings.healthBarSliderActiveOnStart, useHealthSlideInfoOnScreen); 

				hasHealthSlider = true;

				useHealthbarManager = true;

				if (setHealthBarAsNotVisibleAtStart) {
					setSliderVisibleState (false);
				}

				healthSliderRemoved = false;
			}
		}
	}

	public void setSliderVisibleState (bool state)
	{
		if (hasHealthSlider) {
			if (useHealthbarManager) {
				healthBarManager.setSliderVisibleState (currentID, state); 
			}
		}
	}

	public void setSliderVisibleStateForPlayer (GameObject player, bool state)
	{
		if (hasHealthSlider) {
			if (useHealthbarManager) {
				healthBarManager.setSliderVisibleStateForPlayer (currentID, player, state); 
			}
		}
	}

	public void setSliderLocatedState (bool state)
	{
		if (hasHealthSlider) {
			if (useHealthbarManager) {
				healthBarManager.setSliderLocatedState (currentID, state); 
			}
		}
	}

	public void setSliderLocatedState (GameObject player, bool state)
	{
		if (hasHealthSlider) {
			if (useHealthbarManager) {
				healthBarManager.setSliderLocatedStateForPlayer (currentID, player, state); 
			}
		}
	}

	public int getHealthID ()
	{
		return currentID;
	}

	public int getClosesWeakSpotIndex (Vector3 collisionPosition)
	{
		float distance = Mathf.Infinity;

		int index = -1;

		Vector3 damagePosition = Vector3.zero;

		for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
			
			if (advancedSettings.weakSpots [i].spotTransform != null) {
				damagePosition = advancedSettings.weakSpots [i].spotTransform.position;
			} else {
				damagePosition = transform.position;
			}

			float currentDistance = GKC_Utils.distance (collisionPosition, damagePosition);

			if (currentDistance < distance) {
				distance = currentDistance;
				index = i;
			}
		}

		if (index > -1) {
			if (advancedSettings.showGizmo) {
				if (showDamageReceivedDebugInfo) {
					print (advancedSettings.weakSpots [index].name);
				}
			}
		}

		return index;
	}

	public bool checkIfWeakSpotListContainsTransform (Transform transformToCheck)
	{
		for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
			if (advancedSettings.weakSpots [i].spotTransform == transformToCheck) {
				return true;
			}
		}

		return false;
	}

	public void activateEventToSendCurrentHealthAmount ()
	{
		eventToSendCurrentHealthAmount.Invoke (healthAmount);
	}

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

	public void pushFullCharacter ()
	{
		if (ragdollManager != null) {
			if (ragdollManager.isRagdollActive ()) {
				print ("hsihdsihds"); // erm.... huh? o_O
				ragdollManager.pushHipsRigidobdy (transform.up * 80);
			} else {
				ragdollManager.pushFullCharacter ((transform.forward + transform.up) / 2);
			}
		}
	}

	public void pushCharacterWithoutForce ()
	{
		if (ragdollManager != null) {
			ragdollManager.pushCharacterWithoutForce ();
		}
	}

	public void TakeDamage (float damage)
	{
		takeHealth (damage);
	}

	public void killCharacter ()
	{
		killByButton ();
	}

	public void killByButton ()
	{
		setDamage (healthAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, 
			false, false, true, false, false, false, -1, -1);
	}

	public void killCharacter (Vector3 fromDirection, Vector3 damagePos, GameObject attacker, GameObject projectile, bool damageConstant)
	{
		setDamage (healthAmount, fromDirection, damagePos, attacker, projectile, damageConstant, false, true, false, false, false, -1, -1);
	}

	public void takeHealth (float damageAmount)
	{
		setDamage (damageAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, 
			false, false, true, false, false, false, -1, -1);
	}

	public void takeConstantDamage (float damageAmount)
	{
		setDamage (damageAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, 
			true, false, false, false, false, false, -1, -1);
	}

	public void takeConstantDamageWithoutShield (float damageAmount)
	{
		setDamage (damageAmount, transform.forward, transform.position + transform.up * 1.5f, gameObject, gameObject, 
			true, false, true, false, false, false, -1, -1);
	}

	public void takeDamageFromDebugDamageSourceTransform (float damageAmount)
	{
		if (debugDamageSourceTransform != null) {
			setDamage (damageAmount, debugDamageSourceTransform.forward, debugDamageSourceTransform.position, gameObject, gameObject, 
				false, false, true, false, false, false, -1, -1);
		}
	}

	public shieldSystem getShieldSystem ()
	{
		return mainShieldSystem;
	}

	public void callExtraDeadFunctions ()
	{
		StartCoroutine (callExtraDeadFunctionsnCoroutine ());
	}

	IEnumerator callExtraDeadFunctionsnCoroutine ()
	{
		yield return new WaitForSeconds (delayInExtraDeadFunctions);

		extraDeadFunctionCall.Invoke ();

		yield return null;			
	}

	public override void changePlaceToShootPosition (bool state)
	{
		if (state) {
			placeToShoot.localPosition = originalPlaceToShootPosition - placeToShoot.up;
		} else {
			placeToShoot.localPosition = originalPlaceToShootPosition;
		}
	}

	public override void setPlaceToShootPositionOffset (float newValue)
	{
		placeToShoot.localPosition = originalPlaceToShootPosition - placeToShoot.up * newValue;
	}

	public Transform getPlaceToShoot ()
	{
		return placeToShoot;
	}

	public Transform getTransformToAttachWeaponsByClosestPosition (Vector3 positionToCheck)
	{
		float distance = Mathf.Infinity;

		int weakSpotIndex = -1;

		Transform currentTransformToCheck = null;

		for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
			currentTransformToCheck = advancedSettings.weakSpots [i].transformToAttachWeapons;
				
			if (advancedSettings.weakSpots [i].useBoneTransformToAttachWeapons) {
				currentTransformToCheck = advancedSettings.weakSpots [i].spotTransform;
			}

			if (currentTransformToCheck != null) {
				float currentDistance = GKC_Utils.distance (positionToCheck, currentTransformToCheck.position);

				if (currentDistance < distance) {
					distance = currentDistance;
					weakSpotIndex = i;
				}
			}
		}

		if (weakSpotIndex > -1) {
			if (showDamageReceivedDebugInfo) {
				print (advancedSettings.weakSpots [weakSpotIndex].name);
			}

			if (advancedSettings.weakSpots [weakSpotIndex].useBoneTransformToAttachWeapons) {
				return advancedSettings.weakSpots [weakSpotIndex].spotTransform;
			} else {
				return advancedSettings.weakSpots [weakSpotIndex].transformToAttachWeapons;
			}
		}

		return null;
	}

	public void updateDamageReceivers ()
	{
		for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
			if (advancedSettings.weakSpots [i].spotTransform != null) {

				characterDamageReceiver currentDamageReceiverToCheck = advancedSettings.weakSpots [i].spotTransform.GetComponent<characterDamageReceiver> ();

				if (currentDamageReceiverToCheck != null) {
					currentDamageReceiverToCheck.setCharacter (gameObject, this);
				}
			}
		}

		updateComponent ();

		print ("Damage receivers of " + gameObject.name + " have been updated");
	}

	public void activateAllEventsOnEmptyHealthAmountOnWeakSpotsList ()
	{
		for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
			advancedSettings.weakSpots [i].eventOnEmtpyHealthAmountOnSpot.Invoke ();
		}
	}

	public void activateAllEventsOnDamageWeakSpotsList ()
	{
		for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
			advancedSettings.weakSpots [i].damageFunction.Invoke ();
		}
	}

	public override bool isDead ()
	{
		return dead;
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

	public string getCharacterName ()
	{
		return characterName;
	}

	public bool addDamageReceiversToCustomTransformList;
	public List<Transform> customTransformListDamageReceiver = new List<Transform> ();

	public void setCustomTransformListDamageReceiver ()
	{
		for (int i = 0; i < customTransformListDamageReceiver.Count; i++) {
			customTransformListDamageReceiver [i].gameObject.AddComponent <characterDamageReceiver> ().setCharacter (gameObject, this);
		}

		customTransformListDamageReceiver.Clear ();

		updateComponent ();
	}

	public void updateCharacterDamageReceiverOnObject ()
	{
		Component[] damageReceivers = GetComponentsInChildren (typeof(characterDamageReceiver));
		if (damageReceivers.Length > 0) {
			foreach (characterDamageReceiver newReceiver in damageReceivers) {

				newReceiver.setCharacter (gameObject, this);
			}
		}

		updateComponent ();
	}

	public void addDamageReceiverToObject ()
	{
		if (mainDamageReceiver == null) {
			mainDamageReceiver = GetComponent<characterDamageReceiver> ();

			if (mainDamageReceiver == null) {
				mainDamageReceiver = gameObject.AddComponent<characterDamageReceiver> ();
			}
		}

		mainDamageReceiver.setCharacter (gameObject, this);
	}

	//Set weak spots on humanoid npcs
	public void setHumanoidWeaKSpots ()
	{
		Animator animator = transform.GetChild (0).GetComponentInChildren<Animator> ();

		if (animator != null) {		
			advancedSettings.weakSpots.Clear ();

			Transform head = animator.GetBoneTransform (HumanBodyBones.Head);
			Transform upperBody = animator.GetBoneTransform (HumanBodyBones.Chest);
			Transform lowerBody = animator.GetBoneTransform (HumanBodyBones.Spine);
			Transform upperLeftLeg = animator.GetBoneTransform (HumanBodyBones.LeftUpperLeg);
			Transform lowerLeftLeg = animator.GetBoneTransform (HumanBodyBones.LeftLowerLeg);
			Transform upperRightLeg = animator.GetBoneTransform (HumanBodyBones.RightUpperLeg);
			Transform lowerRightLeg = animator.GetBoneTransform (HumanBodyBones.RightLowerLeg);
			Transform upperLeftArm = animator.GetBoneTransform (HumanBodyBones.LeftUpperArm);
			Transform lowerLeftArm = animator.GetBoneTransform (HumanBodyBones.LeftLowerArm);
			Transform upperRightArm = animator.GetBoneTransform (HumanBodyBones.RightUpperArm);
			Transform lowerRightArm = animator.GetBoneTransform (HumanBodyBones.RightLowerArm);

			if (head != null) {
				addWeakSpot (head, "Head", 1, false);
			}

			if (upperBody != null) {
				addWeakSpot (upperBody, "Upper Body", 2, false);
			}

			if (lowerBody != null) {
				addWeakSpot (lowerBody, "Lower Body", 2, false);
			}

			if (upperLeftLeg != null) {
				addWeakSpot (upperLeftLeg, "Upper Left Leg", 2, false);
			}

			if (lowerLeftLeg != null) {
				addWeakSpot (lowerLeftLeg, "Lower Left Leg", 0.5f, false);
			}

			if (upperRightLeg != null) {
				addWeakSpot (upperRightLeg, "Upper Right Leg", 2, false);
			}

			if (lowerRightLeg != null) {
				addWeakSpot (lowerRightLeg, "Lower Right Leg", 0.5f, false);
			}

			if (upperLeftArm != null) {
				addWeakSpot (upperLeftArm, "Upper Left Arm", 2, false);
			}

			if (lowerLeftArm != null) {
				addWeakSpot (lowerLeftArm, "Lower Left Arm", 0.5f, false);
			}

			if (upperRightArm != null) {
				addWeakSpot (upperRightArm, "Upper Right Arm", 2, false);
			}

			if (lowerRightArm != null) {
				addWeakSpot (lowerRightArm, "Lower Right Arm", 0.5f, false);
			}

			updateDamageReceivers ();
		}
	}

	public void resetWeakSpotDamageMultipliers ()
	{
		for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
			advancedSettings.weakSpots [i].damageMultiplier = 1;
		
			advancedSettings.weakSpots [i].killedWithOneShoot = false;
		}

		updateComponent ();
	}

	public void addWeakSpot (Transform spotTransform, string spotName, float damageMultiplier, bool killedInOneShot)
	{
		weakSpot newWeakSpot = new weakSpot ();

		newWeakSpot.name = spotName;
		newWeakSpot.spotTransform = spotTransform;
		newWeakSpot.damageMultiplier = damageMultiplier;
		newWeakSpot.killedWithOneShoot = killedInOneShot;

		advancedSettings.weakSpots.Add (newWeakSpot);
	}

	public void setInvincibleState (bool state)
	{
		invincible = state;

		if (!invincible) {
			temporalInvincibilityActive = false;

			checkEventsOnTemporalInvincibilityActive = false;
		}

		checkEventsOnInvicibleStateChange (invincible);
	}

	Coroutine invincibleDurationCoroutine;

	public void setInvincibleStateDuration (float invincibleDuration)
	{
		stopDamageOverTime ();

		stopSetInvincibleStateDurationCoroutine ();

		invincibleDurationCoroutine = StartCoroutine (setInvincibleStateDurationCoroutine (invincibleDuration));
	}

	public void setInvincibleStateDurationWithoutDisableDamageOverTime (float invincibleDuration)
	{
		stopSetInvincibleStateDurationCoroutine ();

		invincibleDurationCoroutine = StartCoroutine (setInvincibleStateDurationCoroutine (invincibleDuration));
	}

	IEnumerator setInvincibleStateDurationCoroutine (float invincibleDuration)
	{
		invincible = true; 

		temporalInvincibilityActive = true;

		checkEventsOnInvicibleStateChange (invincible);

		yield return new WaitForSeconds (invincibleDuration);

		invincible = originalInvincibleValue;

		temporalInvincibilityActive = false;

		checkEventsOnTemporalInvincibilityActive = false;

		checkEventsOnInvicibleStateChange (invincible);
	}

	public void setCheckEventsOnTemporalInvincibilityActiveState (bool state)
	{
		checkEventsOnTemporalInvincibilityActive = state;

		if (checkEventsOnTemporalInvincibilityActive) {
			lastTimeCheckEventsOnTemporalInvincibilityActive = Time.time;
		}
	}

	public void checkEventsOnInvicibleStateChange (bool state)
	{
		if (useEventsOnInvincibleStateChange) {
			if (state) {
				eventOnInvicibleOn.Invoke ();
			} else {
				eventOnInvicibleOff.Invoke ();
			}
		}
	}

	public void stopSetInvincibleStateDurationCoroutine ()
	{
		if (invincibleDurationCoroutine != null) {
			StopCoroutine (invincibleDurationCoroutine);
		}
	}

	public void initializeHealthAmount (float newValue)
	{
		healthAmount = newValue;
	}

	float lastIncreaseMaxHealthAmount = 0;

	public void checkLastIncreaseMaxHealthAmount ()
	{
		if (lastIncreaseMaxHealthAmount > 0) {
			setHealthAmountOnMaxValue ();
		} else {
			clampHealthAmountToMaxHealth ();
		}
	}

	public void increaseMaxHealthAmount (float newAmount)
	{
		maxHealthAmount += newAmount;

		lastIncreaseMaxHealthAmount = newAmount;

		updateSliderMaxValue (maxHealthAmount);
	}

	public void increaseMaxHealthAmountByMultiplier (float amountMultiplier)
	{
		maxHealthAmount *= amountMultiplier;

		updateSliderMaxValue (maxHealthAmount);
	}

	public void initializeMaxHealthAmount (float newValue)
	{
		maxHealthAmount = newValue;
	}

	public void setRegenerateHealthState (bool state)
	{
		regenerateHealth = state;
	}

	public void setConstantRegenerateState (bool state)
	{
		constantRegenerate = state;
	}

	public void initializeRegenerateAmount (float newValue)
	{
		regenerateAmount = newValue;
	}

	public void increaseRegenerateAmount (float newValue)
	{
		regenerateAmount += newValue;
	}

	public void increaseRegenerateAmountByMultiplier (float amountMultiplier)
	{
		regenerateAmount *= amountMultiplier;
	}

	public bool checkIfMaxHealth ()
	{
		return getCurrentHealthAmount () >= getMaxHealthAmount ();
	}

	public void setShield (float shieldAmount)
	{
		if (useShield) {
			mainShieldSystem.getShield (shieldAmount);
		}
	}

	public override float getCurrentShieldAmount ()
	{
		if (useShield) {
			return mainShieldSystem.getCurrentShieldAmount ();
		}

		return 0;
	}

	public float getHealthAmountToPick (float amount)
	{
		float totalAmmoAmountToAdd = 0;

		float amountToRefill = getHealthAmountToLimit ();

		if (amountToRefill > 0) {
			if (showDamageReceivedDebugInfo) {
				print ("amount to refill " + amountToRefill);
			}

			totalAmmoAmountToAdd = amount;

			if (amountToRefill < amount) {
				totalAmmoAmountToAdd = amountToRefill;
			}

			if (showDamageReceivedDebugInfo) {
				print (totalAmmoAmountToAdd);
			}

			addAuxHealthAmount (totalAmmoAmountToAdd);
		}

		return totalAmmoAmountToAdd;
	}

	public void setGeneralDamageMultiplerActiveState (bool state)
	{
		if (generalDamageMultiplerEnabled) {
			generalDamageMultiplerActive = state;
		}
	}

	public void setGeneralDamageMultiplierValue (float newValue)
	{
		generalDamageMultiplier = newValue;
	}

	public void setOriginalDamageMultiplierValue ()
	{
		setGeneralDamageMultiplierValue (originalGeneralDamageMultiplier);
	}

	public override void setBlockDamageActiveState (bool state)
	{
		blockDamageActive = state;
	}

	public override void setBlockDamageProtectionAmount (float newValue)
	{
		blockDamageProtectionAmount = newValue;

		if (blockDamageProtectionAmount > 1) {
			blockDamageProtectionAmount = 1;
		}
	}

	public override void setBlockDamageRangleAngleState (bool useMaxBlockRangeAngleValue, float maxBlockRangeAngleValue)
	{
		useMaxBlockRangeAngle = useMaxBlockRangeAngleValue;
		maxBlockRangeAngle = maxBlockRangeAngleValue;
	}

	public override void setHitReactionBlockIDValue (int newBlockID)
	{
		if (mainDamageHitReactionSystem != null) {
			mainDamageHitReactionSystem.setHitReactionBlockIDValue (newBlockID);
		}
	}

	//Override functions from Health Management
	public override void setDamageWithHealthManagement (float damageAmount, Vector3 fromDirection, Vector3 damagePos, GameObject attacker, 
	                                                    GameObject projectile, bool damageConstant, bool searchClosestWeakSpot, bool ignoreShield, 
	                                                    bool ignoreDamageInScreen, bool damageCanBeBlocked, bool canActivateReactionSystemTemporally,
	                                                    int damageReactionID, int damageTypeID)
	{
		setDamage (damageAmount, fromDirection, damagePos, attacker, projectile, damageConstant, searchClosestWeakSpot,
			ignoreShield, ignoreDamageInScreen, damageCanBeBlocked, canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
	}

	public override bool checkIfDeadWithHealthManagement ()
	{
		return isDead ();
	}

	public override bool checkIfMaxHealthWithHealthManagement ()
	{
		return checkIfMaxHealth ();
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

	public override void sedateCharacterithHealthManagement (Vector3 position, float sedateDelay, bool useWeakSpotToReduceDelay, bool sedateUntilReceiveDamage, float sedateDuration)
	{
		sedateCharacter (position, sedateDelay, useWeakSpotToReduceDelay, sedateUntilReceiveDamage, sedateDuration);
	}

	public override void setHealWithHealthManagement (float healAmount)
	{
		getHealth (healAmount);
	}

	public override void setShieldWithHealthManagement (float shieldAmount)
	{
		setShield (shieldAmount);
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

	public override void killCharacterWithHealthManagement (GameObject projectile, Vector3 direction, Vector3 position, GameObject attacker, bool damageConstant)
	{
		killCharacter (direction, position, attacker, projectile, damageConstant);
	}

	public override Transform getPlaceToShootWithHealthManagement ()
	{
		return placeToShoot;
	}

	public override GameObject getPlaceToShootGameObjectWithHealthManagement ()
	{
		return placeToShoot.gameObject;
	}

	public override bool isCharacterWithHealthManagement ()
	{
		return objectIsCharacter;
	}

	public override List<health.weakSpot> getCharacterWeakSpotListWithHealthManagement ()
	{
		return advancedSettings.weakSpots;
	}

	public override GameObject getCharacterWithHealthManagement ()
	{
		return gameObject;
	}

	public override GameObject getCharacterOrVehicleWithHealthManagement ()
	{
		return gameObject;
	}

	public override void killCharacterWithHealthManagement ()
	{
		killCharacter ();
	}

	public override bool checkIfWeakSpotListContainsTransformWithHealthManagement (Transform transformToCheck)
	{
		return checkIfWeakSpotListContainsTransform (transformToCheck);
	}

	public override int getDecalImpactIndexWithHealthManagement ()
	{
		return getDecalImpactIndex ();
	}

	public override bool isCharacterInRagdollState ()
	{
		return characterIsOnRagdollState ();
	}

	public override Transform getCharacterRootMotionTransform ()
	{
		if (ragdollManager != null) {
			return ragdollManager.getRootMotion ();
		}

		return null;
	}

	public override void updateSliderOffset (float value)
	{
		if (hasHealthSlider) {
			healthBarManager.updateSliderOffset (currentID, value); 
		}
	}

	public override void updateOriginalSliderOffset ()
	{
		updateSliderOffset (settings.sliderOffset.y);
	}

	public void setAllyNewNameIngame (string newName)
	{
		settings.allyName = newName;
	}

	public void setEnemyNewNameIngame (string newName)
	{
		settings.enemyName = newName;
	}

	public void enableOrDisableDamageTypeInfo (string damageTypeName, bool state)
	{
		for (int i = 0; i < damageTypeInfoList.Count; i++) {
			if (damageTypeInfoList [i].Name.Equals (damageTypeName)) {
				damageTypeInfoList [i].damageTypeEnabled = state;

				return;
			}
		}
	}

	public void increaseDamageTypeInfo (string damageTypeName, float extraValue)
	{
		for (int i = 0; i < damageTypeInfoList.Count; i++) {
			if (damageTypeInfoList [i].Name.Equals (damageTypeName)) {
				damageTypeInfoList [i].damageTypeResistance += extraValue;

				return;
			}
		}
	}

	public void setObtainHealthOnDamageTypeState (string damageTypeName, bool state)
	{
		for (int i = 0; i < damageTypeInfoList.Count; i++) {
			if (damageTypeInfoList [i].Name.Equals (damageTypeName)) {
				damageTypeInfoList [i].obtainHealthOnDamageType = state;

				return;
			}
		}
	}



	//EDITOR FUNCTIONS
	public void setAllyNewName (string newName)
	{
		settings.allyName = newName;

		if (!Application.isPlaying) {
			updateComponent ();
		}
	}

	public void setEnemyNewName (string newName)
	{
		settings.enemyName = newName;

		if (!Application.isPlaying) {
			updateComponent ();
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Updating health component " + gameObject.name, gameObject);
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
		// && !Application.isPlaying
		if (advancedSettings.showGizmo) {
			if (!Application.isPlaying) {
				for (int i = 0; i < advancedSettings.weakSpots.Count; i++) {
					if (advancedSettings.weakSpots [i].spotTransform != null) {
					
						float rValue = 0;
						float gValue = 0;
						float bValue = 0;

						if (!advancedSettings.weakSpots [i].killedWithOneShoot) {
							if (advancedSettings.weakSpots [i].damageMultiplier < 1) {
								bValue = advancedSettings.weakSpots [i].damageMultiplier / 0.1f;
							} else {
								rValue = advancedSettings.weakSpots [i].damageMultiplier / 20;
							}
						} else {
							rValue = 1;
							gValue = 1;
						}

						advancedSettings.weakSpots [i].weakSpotColor = new Vector4 (rValue, gValue, bValue, advancedSettings.alphaColor);

						Gizmos.color = advancedSettings.weakSpots [i].weakSpotColor;
						Gizmos.DrawSphere (advancedSettings.weakSpots [i].spotTransform.position, advancedSettings.gizmoRadius);

						if (advancedSettings.notHuman) {
							advancedSettings.weakSpots [i].spotTransform.GetComponent<characterDamageReceiver> ().damageMultiplier = advancedSettings.weakSpots [i].damageMultiplier;
						}
					}
				}
			}

			if (settings.useHealthSlider && settings.enemyHealthSlider != null) {
				Gizmos.color = advancedSettings.gizmoLabelColor;
				Gizmos.DrawSphere (transform.position + settings.sliderOffset, advancedSettings.gizmoRadius);
			}
		}
	}

	[System.Serializable]
	public class enemySettings
	{
		public bool useHealthSlider = true;
		public GameObject enemyHealthSlider;
		public Vector3 sliderOffset = new Vector3 (0, 1, 0);
		public bool useRaycastToCheckVisible;
		public LayerMask layer;
		public string enemyName;
		public string allyName;
		public Color enemySliderColor = Color.red;
		public Color allySliderColor = Color.green;
		public Color nameTextColor = Color.black;

		public bool removeHealthBarSliderOnDeath = true;

		public bool healthBarSliderActiveOnStart;
	}

	[System.Serializable]
	public class advancedSettingsClass
	{
		public bool notHuman;
		public bool useWeakSpots;
		public List<weakSpot> weakSpots = new List<weakSpot> ();

		public bool useHealthAmountOnSpotEnabled = true;

		public bool haveRagdoll;
		public bool allowPushCharacterOnExplosions = true;

		public bool activateRagdollOnDamageReceived = true;
		public float minDamageToEnableRagdoll;
		public bool ragdollCanReceiveDamageOnImpact;
		public UnityEvent ragdollEvent;

		public bool showGizmo;

		public int labelTextSize = 5;
		public Color gizmoLabelColor;
		[Range (0, 1)] public float alphaColor;
		[Range (0, 1)] public float gizmoRadius;
	}

	[System.Serializable]
	public class weakSpot
	{
		public string name;
		public Transform spotTransform;
		[Range (0.1f, 20)] public float damageMultiplier;

		public bool killedWithOneShoot;
		public bool needMinValueToBeKilled;
		public float minValueToBeKilled;

		public Color weakSpotColor;

		public bool sendFunctionWhenDamage;
		public bool sendFunctionWhenDie;
		public UnityEvent damageFunction;

		public bool useHealthAmountOnSpot;
		public float healhtAmountOnSpot;
		public bool killCharacterOnEmtpyHealthAmountOnSpot;
		public UnityEvent eventOnEmtpyHealthAmountOnSpot;
		public bool healthAmountOnSpotEmpty;

		public bool useCriticalDamageSpot;
		public Vector2 criticalDamageProbability;
		public float damageMultiplierOnCritical;
		public bool killTargetOnCritical;
		public bool removeAllHealthAmountOnSpotOnCritical;

		public Transform transformToAttachWeapons;

		public bool useBoneTransformToAttachWeapons;

		public bool sendValueToArmorClothSystemOnDamage;
		public string armorClothCategoryName;
		public float damageMultiplierOnArmorClothPiece = 1;
	}

	[System.Serializable]
	public class damageFunctionInfo
	{
		public string Name;
		public UnityEvent damageFunctionCall;
		public float damageRecived;
	}

	[System.Serializable]
	public class eventOnHealthValue
	{
		public string Name;

		public bool eventEnabled = true;
		public UnityEvent eventToCall;

		public bool useEventOnDamageReceived = true;
		public float minDamageToReceive;
		public bool callEventOnce;
		public bool eventActivated;

		public bool useDamagePercentage;
	}

	[System.Serializable]
	public class damageTypeInfo
	{
		public string Name;
		public int damageTypeID;

		public bool damageTypeEnabled = true;

		[Range (-1, 1)] public float damageTypeResistance = 1;

		public bool avoidDamageTypeIfBlockDamageActive;

		public bool obtainHealthOnDamageType;
		public float healthMultiplierOnDamageType;

		public bool disableDamageReactionOnDamageType;

		public bool stopDamageCheckIfHealthObtained;

		public bool useEventOnObtainHealthOnDamageType;
		public UnityEvent eventOnObtainHealthOnDamageType;

		public bool useEventOnDamageType;
		public UnityEvent eventOnDamageType;
	}
}