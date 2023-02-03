using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class damageHitReactionSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool hitReactionsEnabled = true;

	public string defaultCategoryReaction = "Melee";

	[Space]
	[Header ("Reaction Probability Settings")]
	[Space]

	public float probabilityToActivateReactionMultiplier = 1;

	public bool canChangeProbabilityMultiplierExternally = true;

	[Space]
	[Header ("Hit Reaction Settings")]
	[Space]

	public List<hitReactionInfo> hitReactionInfoList = new List<hitReactionInfo> ();

	public List<damageStateInfo> damageStateInfoList = new List<damageStateInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool hitReactionActive;

	public int currentBlockID;

	public string currentReactionCategory;

	public bool damageReactionPaused;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useMainEventOnHitReactionFound;
	public UnityEvent mainEventOnHitReactionFound;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform playerControllerTransform;
	public playerActionSystem mainPlayerActionSystem;

	hitReactionInfo currentHitReactionInfo;

	Coroutine perfectBlockCoroutine;

	float lastTimeBlockActive;

	int currentBlockIDToCheck;

	hitDamageDirection currenthitDamageDirection;

	public void checkDamageReceived (float damageReceived, Vector3 damagePosition, GameObject objectBlocked)
	{
		if (!hitReactionsEnabled || !hitReactionActive) {
			return;
		}

		if (damageReactionPaused) {
			return;
		}

		checkHitReaction (damageReceived, 0, damagePosition, false, false, false, objectBlocked, -1);
	}

	public void checkDamageReceivedTemporally (float damageReceived, Vector3 damagePosition, GameObject objectBlocked)
	{
		if (!hitReactionsEnabled) {
			return;
		}

		if (damageReactionPaused) {
			return;
		}
			
		checkHitReaction (damageReceived, 0, damagePosition, false, false, false, objectBlocked, -1);
	}

	public void checkDamageBlocked (float damageReceived, float originalDamageAmountReceived, Vector3 damagePosition, bool damageBlocked, GameObject objectBlocked)
	{
		if (!hitReactionsEnabled || !hitReactionActive) {
			return;
		}

		if (damageReactionPaused) {
			return;
		}

		checkHitReaction (damageReceived, originalDamageAmountReceived, damagePosition, damageBlocked, true, false, objectBlocked, -1);
	}

	public void checkDamageReceivedUnblockable (float damageReceived, Vector3 damagePosition, GameObject objectBlocked)
	{
		if (!hitReactionsEnabled || !hitReactionActive) {
			return;
		}

		if (damageReactionPaused) {
			return;
		}

		checkHitReaction (damageReceived, 0, damagePosition, false, false, true, objectBlocked, -1);
	}

	public void checkReactionToTriggerExternally (float damageReceived, Vector3 damagePosition, GameObject objectBlocked)
	{
		if (!hitReactionsEnabled || !hitReactionActive) {
			return;
		}

		if (damageReactionPaused) {
			return;
		}

		checkHitReaction (damageReceived, 0, damagePosition, false, false, false, objectBlocked, -1);
	}

	public void activateDamageReactionByID (float damageReceived, Vector3 damagePosition, GameObject objectBlocked, int damageReactionID)
	{
		if (!hitReactionsEnabled) {
			return;
		}

		if (damageReactionPaused) {
			return;
		}

		checkHitReaction (damageReceived, 0, damagePosition, false, false, false, objectBlocked, damageReactionID);
	}

	void checkHitReaction (float damageReceived, float originalDamageAmountReceived, Vector3 damagePosition, bool damageBlocked,
	                       bool blockDamageActive, bool unblockableDamage, GameObject objectBlocked, int damageReactionID)
	{
		if (damageReactionID == -1) {
			Vector3 damageDirection = damagePosition - playerControllerTransform.position;

			damageDirection = damageDirection / damageDirection.magnitude;

			float damageAngle = Vector3.SignedAngle (playerControllerTransform.forward, damageDirection, playerControllerTransform.up);

			float damageAngleAbs = Mathf.Abs (damageAngle);

			currenthitDamageDirection = hitDamageDirection.front;

			if (damageAngleAbs > 45 && damageAngleAbs < 135) {
				if (damageAngle < 0) {
					currenthitDamageDirection = hitDamageDirection.left;
				} else {
					currenthitDamageDirection = hitDamageDirection.right;
				}
			} else if (damageAngleAbs > 135) {
				currenthitDamageDirection = hitDamageDirection.backward;
			}

			if (showDebugPrint) {
				print (currenthitDamageDirection);
			}
		}

		int hitReactionIndex = -1;
		float maxDamageFound = -1;

		bool damageStateFound = false;

		if (damageReceived > 0) {
			for (int i = 0; i < damageStateInfoList.Count; i++) {
				damageStateInfo currentDamageStateInfo = damageStateInfoList [i];
					
				if (!damageStateFound && currentDamageStateInfo.damageStateEnabled) {
					if (currentDamageStateInfo.damageStateRange.x <= damageReceived &&
					    currentDamageStateInfo.damageStateRange.y >= damageReceived) {

						if (showDebugPrint) {
							print (currentDamageStateInfo.Name);
						}

						currentDamageStateInfo.eventOnDamageState.Invoke ();

						damageStateFound = true;
					}
				}
			}
		}

		if (!damageStateFound) {
			for (int i = 0; i < damageStateInfoList.Count; i++) {
				damageStateInfo currentDamageStateInfo = damageStateInfoList [i];

				if (!damageStateFound && currentDamageStateInfo.damageStateEnabled) {
					if (currentDamageStateInfo.useAsDefaultState) {
						currentDamageStateInfo.eventOnDamageState.Invoke ();

						if (showDebugPrint) {
							print (currentDamageStateInfo.Name);
						}

						damageStateFound = true;
					}
				}
			}
		}

		bool canCheckReaction = false;

		if (currentReactionCategory == "") {
			currentReactionCategory = defaultCategoryReaction;
		}

		bool searchingReactionByID = damageReactionID > -1;

		bool canCheckRegularReaction = false;


		for (int i = 0; i < hitReactionInfoList.Count; i++) {

			currentHitReactionInfo = hitReactionInfoList [i];

			if (currentHitReactionInfo.reactionEnabled) {

				if (searchingReactionByID) {
					if (hitReactionIndex == -1) {
						if (damageReactionID == currentHitReactionInfo.damageReactionID) {
							canCheckRegularReaction = true;

							if (showDebugPrint) {
								print ("Damage reaction activated by ID, checking reaction" + currentHitReactionInfo.Name);
							}
						}
					}
				} else {
					canCheckRegularReaction = false;

					if (currentHitReactionInfo.reactionCategory == currentReactionCategory) {

						if (currentHitReactionInfo.reactionOnBlock == blockDamageActive) {

							if (!blockDamageActive || currentHitReactionInfo.blockID == currentBlockID) {
								canCheckRegularReaction = true;
							}
						}
					}
				}
				
				if (canCheckRegularReaction) {
					canCheckReaction = false;

					if (damageReceived >= currentHitReactionInfo.minAmountToReceiveDamage) {
						canCheckReaction = true;
					}

					if (blockDamageActive && (damageBlocked || damageReceived == 0)) {
						canCheckReaction = true;
					}

					if (showDebugPrint) {
						print (currentHitReactionInfo.Name + " " + damageReceived + " " + blockDamageActive + " " + damageBlocked + " " + canCheckReaction);
					}

					if (canCheckReaction) {
						if (currentHitReactionInfo.minAmountToReceiveDamage > maxDamageFound) {
							if (damageReactionID > -1 ||
							    (currentHitReactionInfo.damageDirection == hitDamageDirection.anyDirection ||
							    currentHitReactionInfo.damageDirection == currenthitDamageDirection)) {
								
								maxDamageFound = currentHitReactionInfo.minAmountToReceiveDamage;

								hitReactionIndex = i;
							}
						}
					}
				}
			}
		}

		if (hitReactionIndex > -1) {
			activateHitReactionByIndex (hitReactionIndex, damageReceived, originalDamageAmountReceived, unblockableDamage, objectBlocked);
		}
	}

	float lastTimeHitReaction;

	float currentReactionDelayUntilNextReaction = 0;

	void activateHitReactionByIndex (int hitReactionIndex, float damageReceived, float originalDamageAmountReceived, bool unblockableDamage, GameObject objectBlocked)
	{
		if (hitReactionIndex > -1) {
			currentHitReactionInfo = hitReactionInfoList [hitReactionIndex];

			if (currentHitReactionInfo.useProbabilityToActivateReaction) {
				float probabilityToActivateReaction = Random.Range (0, 100);

				float currentProbability = currentHitReactionInfo.probabilityToActivateReaction * probabilityToActivateReactionMultiplier;

				if (currentProbability < probabilityToActivateReaction) {
					return;
				}
			}

			if (!currentHitReactionInfo.ignoreReactionDelayFromPreviousReaction) {
				if (currentReactionDelayUntilNextReaction > 0 && Time.time < currentReactionDelayUntilNextReaction + lastTimeHitReaction) {
					if (showDebugPrint) {
						print ("\n reaction delay from previous reaction is active, disabling reaction activation \n");
					}

					return;
				}

				currentReactionDelayUntilNextReaction = 0;

				if (currentHitReactionInfo.setReactionDelayUntilNextReaction) {
					currentReactionDelayUntilNextReaction = currentHitReactionInfo.reactionDelayUntilNextReaction;
				}
			}

			lastTimeHitReaction = Time.time;

			if (Time.time > currentHitReactionInfo.minWaitTimeToPlayHitReactionAgain + currentHitReactionInfo.lastTimeHitReaction) {
				currentHitReactionInfo.lastTimeHitReaction = Time.time;

				bool canActivateHitReaction = true;

				bool perfectBlockActivated = false;

				if (!currentHitReactionInfo.activateHitReactionOnPerfectBlock && !unblockableDamage) {
					if (currentHitReactionInfo.reactionOnBlock && currentHitReactionInfo.canUsePerfectBlock) {
						if (lastTimeBlockActive != 0) {
							if (Time.time < lastTimeBlockActive + currentHitReactionInfo.maxTimeToPerfectBlock) {
								canActivateHitReaction = false;

								perfectBlockActivated = true;
							}
						}
					}
				}

				if (showDebugPrint) {
					print ("\n\n");
					print (currentHitReactionInfo.Name + " " + damageReceived + " " + playerControllerTransform.name + " " + perfectBlockActivated + " " + canActivateHitReaction);
				}

				if (canActivateHitReaction) {
					if (useMainEventOnHitReactionFound) {
						mainEventOnHitReactionFound.Invoke ();
					}

					if (currentHitReactionInfo.playHitAction) {
						mainPlayerActionSystem.activateCustomAction (currentHitReactionInfo.hitActionName);
					}
				} else {
					if (perfectBlockActivated) {
						if (currentHitReactionInfo.playHitAction) {
							mainPlayerActionSystem.activateCustomAction (currentHitReactionInfo.hitActionNameOnPerfectBlock);
						}
					}
				}

				if (currentHitReactionInfo.useStaminaOnBlock) {
					float currentDamageReceived = damageReceived;

					if (currentDamageReceived == 0) {
						currentDamageReceived = originalDamageAmountReceived;
					}

					float staminaAmountToUse = currentHitReactionInfo.staminaToUseMultiplier * currentDamageReceived;

					currentHitReactionInfo.eventToUseStamina.Invoke (staminaAmountToUse);
				}

				if (canActivateHitReaction) {
					currentHitReactionInfo.eventOnHit.Invoke ();
				} else {
					if (perfectBlockActivated) {
						currentHitReactionInfo.eventOnHitReactionOnPerfectBlock.Invoke ();

						if (currentHitReactionInfo.useRemoteEvent) {
							bool useRemoteEvents = false;

							if (objectBlocked != null) {
								if (currentHitReactionInfo.checkObjectsToUseRemoteEventsOnDamage) {
									if ((1 << objectBlocked.layer & currentHitReactionInfo.layerToUseRemoteEventsOnDamage.value) == 1 << objectBlocked.layer) {
										useRemoteEvents = true;
									}
								} else {
									useRemoteEvents = true;
								}

								if (useRemoteEvents) {
									remoteEventSystem currentRemoteEventSystem = objectBlocked.GetComponent<remoteEventSystem> ();

									if (currentRemoteEventSystem != null) {
										for (int j = 0; j < currentHitReactionInfo.remoteEventNameList.Count; j++) {
											currentRemoteEventSystem.callRemoteEvent (currentHitReactionInfo.remoteEventNameList [j]);
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public void checkDamageStateTemporally (float damageReceived, bool damageBlockedByShieldSystem)
	{
//		print (damageReceived);

		bool damageStateFound = false;

		if (damageReceived > 0) {

			if (damageBlockedByShieldSystem) {
				damageReceived /= 2;
			}

			for (int i = 0; i < damageStateInfoList.Count; i++) {
				damageStateInfo currentDamageStateInfo = damageStateInfoList [i];

				if (!damageStateFound && currentDamageStateInfo.damageStateEnabled) {
					if (currentDamageStateInfo.damageStateRange.x <= damageReceived &&
					    currentDamageStateInfo.damageStateRange.y >= damageReceived) {
						//						print (currentDamageStateInfoName);

						currentDamageStateInfo.eventOnDamageState.Invoke ();

						damageStateFound = true;
					}
				}
			}
		}

		if (!damageStateFound) {
			for (int i = 0; i < damageStateInfoList.Count; i++) {
				damageStateInfo currentDamageStateInfo = damageStateInfoList [i];

				if (!damageStateFound && currentDamageStateInfo.damageStateEnabled) {
					if (currentDamageStateInfo.useAsDefaultState) {
						currentDamageStateInfo.eventOnDamageState.Invoke ();

						//						print (currentDamageStateInfo.Name);

						damageStateFound = true;
					}
				}
			}
		}
	}

	public void setHitReactionActiveState (bool state)
	{
		hitReactionActive = state;
	}

	public bool getHitReactionActiveState ()
	{
		return hitReactionActive;
	}

	public void setHitReactionBlockIDValue (int newBlockID)
	{
		currentBlockID = newBlockID;

		if (currentBlockID != -1) {

			currentBlockIDToCheck = -1;

			for (int i = 0; i < hitReactionInfoList.Count; i++) {
				hitReactionInfo temporalReationInfo = hitReactionInfoList [i];

				if (currentBlockIDToCheck == -1 &&
				    temporalReationInfo.reactionOnBlock &&
				    temporalReationInfo.reactionCategory.Equals (currentReactionCategory) &&
				    temporalReationInfo.canUsePerfectBlock &&
				    temporalReationInfo.blockID == currentBlockID) {

					float probabilityForPerfectBlock = Random.Range (0, 100);

					if (temporalReationInfo.probabilityForPerfectBlock >= probabilityForPerfectBlock) {
						currentBlockIDToCheck = i;
					}
				}
			}

			if (currentBlockIDToCheck > -1) {

				lastTimeBlockActive = Time.time;

				setPerfectBlockDuration ();
			} else {
				lastTimeBlockActive = 0;
			}
		} else {
			lastTimeBlockActive = 0;

			stopSetPerfectBlockDurationCoroutine ();

			if (currentBlockIDToCheck > -1) {
				hitReactionInfoList [currentBlockIDToCheck].eventOnDisablePerfectBlock.Invoke ();

				currentBlockIDToCheck = -1;
			}
		}
	}

	public void setPerfectBlockDuration ()
	{
		stopSetPerfectBlockDurationCoroutine ();

		perfectBlockCoroutine = StartCoroutine (setPerfectBlockDurationCoroutine ());
	}

	void stopSetPerfectBlockDurationCoroutine ()
	{
		if (perfectBlockCoroutine != null) {
			StopCoroutine (perfectBlockCoroutine);
		}
	}

	IEnumerator setPerfectBlockDurationCoroutine ()
	{
		hitReactionInfo temporalReationInfo = hitReactionInfoList [currentBlockIDToCheck];

		temporalReationInfo.eventOnEnablePerfectBlock.Invoke ();

		float maxTimeToPerfectBlock = temporalReationInfo.maxTimeToPerfectBlock;

		var currentWaitTime = new WaitForSeconds (maxTimeToPerfectBlock);

		yield return currentWaitTime;

		temporalReationInfo.eventOnDisablePerfectBlock.Invoke ();
	}

	public void setCurrentReactionCategory (string newCategory)
	{
		currentReactionCategory = newCategory;
	}

	public void setNewProbabilityToActivateReactionMultiplierValue (float newValue)
	{
		if (!canChangeProbabilityMultiplierExternally) {
			return;
		}

		probabilityToActivateReactionMultiplier = newValue;
	}

	public void setDamageReactionPausedState (bool state)
	{
		damageReactionPaused = state;
	}

	[System.Serializable]
	public class hitReactionInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;
		public bool reactionEnabled = true;

		public int damageReactionID = -1;

		public string reactionCategory;
	
		public float minAmountToReceiveDamage;

		public hitDamageDirection damageDirection;

		public bool useProbabilityToActivateReaction = true;

		public float probabilityToActivateReaction = 100;

		public float minWaitTimeToPlayHitReactionAgain;

		[Space]
		[Header ("Pause Reaction Activation Settings")]
		[Space]

		public bool setReactionDelayUntilNextReaction;
		public float reactionDelayUntilNextReaction;

		public bool ignoreReactionDelayFromPreviousReaction;

		[Space]
		[Header ("Action Settings")]
		[Space]

		public bool playHitAction;
		public string hitActionName;

		public string hitActionNameOnPerfectBlock;

		[Space]
		[Header ("Hit Reaction On Block Settings")]
		[Space]

		public bool reactionOnBlock;
		public int blockID;

		public bool useStaminaOnBlock;
		public float staminaToUseMultiplier = 1;

		public eventParameters.eventToCallWithAmount eventToUseStamina;

		[Space]
		[Header ("Perfect Block Settings")]
		[Space]

		public bool canUsePerfectBlock;

		public bool activateHitReactionOnPerfectBlock;

		public float probabilityForPerfectBlock = 100;

		public float maxTimeToPerfectBlock = 0.5f;

		public UnityEvent eventOnEnablePerfectBlock;
		public UnityEvent eventOnDisablePerfectBlock;

		[Space]
		[Header ("Events Settings")]
		[Space]

		public UnityEvent eventOnHit;

		[HideInInspector] public float lastTimeHitReaction;

		public UnityEvent eventOnHitReactionOnPerfectBlock;

		[Space]
		[Header ("Remote Events Settings")]
		[Space]

		public bool checkObjectsToUseRemoteEventsOnDamage;
		public LayerMask layerToUseRemoteEventsOnDamage;

		public bool useRemoteEvent;
		public List<string> remoteEventNameList;
	}

	public enum hitDamageDirection
	{
		front,
		backward,
		right,
		left,
		anyDirection
	}

	[System.Serializable]
	public class damageStateInfo
	{
		public string Name;

		public bool damageStateEnabled;

		public Vector2 damageStateRange;

		public bool useAsDefaultState;

		public UnityEvent eventOnDamageState;
	}
}
