using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class stealObjectFromCharacterSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool stealEnabled = true;

	[Range (0, 100)] public float probabilityToStealWeapon = 0;

	public bool stealWeaponOnlyIfNotPickupsLocated;

	public bool useMaxDistanceToStealWeapon;
	public float maxDistanceToGetStealWeapon;

	public bool useMaxDistanceToCameraCenter;
	public float maxDistanceToCameraCenter;

	public float minWaitTimeToActivateSteal = 2;

	[Space]
	[Header ("Weapon Settings")]
	[Space]

	public bool stealCurrentWeapon = true;

	public bool ignoreDrawOnStealWeapon = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventOnSteal;
	public UnityEvent eventOnStealWeapon;

	[Space]
	[Header ("Components")]
	[Space]

	public AIAroundManager mainAIAroundManager;
	public playerController mainPlayerController;

	public inventoryManager mainInventoryManager;
	public playerCamera mainPlayerCamera;
	public Camera mainCamera;


	Vector3 centerScreen;

	float lastTimeObjectStolen;


	public void activateStealFromCharacter ()
	{
		if (!stealEnabled) {
			return;
		}

		if (lastTimeObjectStolen > 0) {
			if (Time.time < minWaitTimeToActivateSteal + lastTimeObjectStolen) {
				return;
			}
		}

		if (stealCurrentWeapon) {
			if (checkIfStealWeaponFromCurrentTarget ()) {

				lastTimeObjectStolen = Time.time;
			}
		}
	}

	bool checkIfStealWeaponFromCurrentTarget ()
	{
		if (showDebugPrint) {
			print ("Checking probability to steal weapon");
		}

		float currentProbability = Random.Range (0, 100);

		if (currentProbability < probabilityToStealWeapon) {
			if (showDebugPrint) {
				print ("AI can check if steal object, checking target state");
			}

			Transform currentTarget = null;

			if (mainPlayerController.isPlayerLookingAtTarget ()) {
				currentTarget = mainPlayerController.getCurrentTargetToLook ();
			} 

			Vector3 currentPlayerPosition = mainPlayerController.transform.position;

			if (currentTarget == null) {

				List<Transform> charactersAround = mainAIAroundManager.getCharactersAround ();

				float currentDistance = 0;
				float maxDistanceToTarget = 10000;

				Vector3 characterPosition = Vector3.zero;

				Vector3 screenPoint = Vector3.zero;

				centerScreen = mainPlayerCamera.getScreenCenter ();

				float currentDistanceToScreenCenter = 0;

				for (int i = charactersAround.Count - 1; i >= 0; i--) {	
					if (charactersAround [i] != null) {
						if (!applyDamage.checkIfDead (charactersAround [i].gameObject)) {

							characterPosition = charactersAround [i].position;

							screenPoint = mainCamera.WorldToScreenPoint (characterPosition);
							currentDistanceToScreenCenter = GKC_Utils.distance (screenPoint, centerScreen);

							bool canBeChecked = false;

							if (useMaxDistanceToCameraCenter && mainPlayerCamera.isCameraTypeFree ()) {
								if (currentDistanceToScreenCenter < maxDistanceToCameraCenter) {
									canBeChecked = true;
								}
							} else {
								canBeChecked = true;
							}

							if (canBeChecked) {
								currentDistance = GKC_Utils.distance (characterPosition, currentPlayerPosition);

								if (currentDistance < maxDistanceToTarget) {
									maxDistanceToTarget = currentDistance;

									currentTarget = charactersAround [i];
								}
							}
						} else {
							charactersAround.RemoveAt (i);
						}
					} else {
						charactersAround.RemoveAt (i);
					}
				}
			}

			if (currentTarget != null) {
				if (useMaxDistanceToStealWeapon) {
					float distance = GKC_Utils.distance (currentTarget.transform.position, currentPlayerPosition);

					if (distance > maxDistanceToGetStealWeapon) {
						return false;
					}
				}

				playerComponentsManager currentPlayerComponentsManager = currentTarget.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					playerController currentPlayerController = currentPlayerComponentsManager.getPlayerController ();

					if (currentPlayerController.isPlayerUsingMeleeWeapons () || currentPlayerController.isPlayerUsingWeapons ()) {
						if (showDebugPrint) {
							print ("target is using weapons, checking weapon it self");
						}

						if (currentPlayerController.isPlayerUsingMeleeWeapons ()) {
							grabbedObjectMeleeAttackSystem currentGrabbedObjectMeleeAttackSystem = currentPlayerComponentsManager.getGrabbedObjectMeleeAttackSystem ();

							if (currentGrabbedObjectMeleeAttackSystem.canWeaponsBeStolenFromCharacter ()) {
								string currentWeaponName = currentGrabbedObjectMeleeAttackSystem.getCurrentMeleeWeaponName ();

								if (currentPlayerController.isCharacterUsedByAI ()) {
									currentGrabbedObjectMeleeAttackSystem.dropMeleeWeaponsExternallyWithoutResultAndDestroyIt ();
								} else {
									inventoryManager currentInventoryManager = currentPlayerComponentsManager.getInventoryManager ();

									if (currentInventoryManager != null) {
										currentInventoryManager.removeObjectAmountFromInventoryByName (currentWeaponName, 1);
									}
								}

								if (ignoreDrawOnStealWeapon) {
									mainInventoryManager.mainMeleeWeaponsGrabbedManager.setCheckDrawKeepWeaponAnimationPauseState (true);
								}

								mainInventoryManager.addObjectAmountToInventoryByName (currentWeaponName, 1);

								currentGrabbedObjectMeleeAttackSystem.mainMeleeWeaponsGrabbedManager.checkEventOnWeaponStolen ();

								if (showDebugPrint) {
									print ("weapon stolen from target");
								}

								if (useEventOnSteal) {
									eventOnStealWeapon.Invoke ();
								}

								return true;
							}
						} else {
							if (showDebugPrint) {
								print ("target is not using weapons");
							}
						}
					} else {
						if (currentPlayerController.isPlayerUsingWeapons ()) {
							playerWeaponsManager currentPlayerWeaponsManager = currentPlayerComponentsManager.getPlayerWeaponsManager ();

							if (currentPlayerWeaponsManager.canWeaponsBeStolenFromCharacter ()) {
								string currentWeaponName = currentPlayerWeaponsManager.getCurrentWeaponName ();

								if (showDebugPrint) {
									print (currentWeaponName + " is the weapon detected, checking if can be picked");
								}

								if (mainInventoryManager.weaponsManager.checkIfWeaponCanBePicked (currentWeaponName)) {
									if (currentPlayerController.isCharacterUsedByAI ()) {
										currentPlayerWeaponsManager.dropCurrentWeaponExternallyWithoutResultAndDestroyIt ();
									} else {
										inventoryManager currentInventoryManager = currentPlayerComponentsManager.getInventoryManager ();

										if (currentInventoryManager != null) {
											currentInventoryManager.removeObjectAmountFromInventoryByName (currentWeaponName, 1);
										}
									}

									mainInventoryManager.addObjectAmountToInventoryByName (currentWeaponName, 1);

									currentPlayerWeaponsManager.checkEventOnWeaponStolen ();

									if (showDebugPrint) {
										print ("weapon stolen from target");
									}

									if (useEventOnSteal) {
										eventOnStealWeapon.Invoke ();
									}

									return true;
								} else {
									if (showDebugPrint) {
										print ("can't use weapon from target");
									}
								}
							}
						}
					}
				}
			}
		}

		return false;
	}
}