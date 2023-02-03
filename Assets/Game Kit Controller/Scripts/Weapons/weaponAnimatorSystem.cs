using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class weaponAnimatorSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool weaponAnimatorEnabled = true;

	public string idleAnimatorName = "Idle";

	public string walkAnimatorName = "Walk";

	public string runAnimatorName = "Run";

	public string shootAnimatorName = "Shoot";

	public string shootAimAnimatorName = "Aim Shoot";

	public string reloadWithAmmoAnimatorName = "Reload With Ammo";

	public string reloadWithoutAmmoAnimatorName = "Reload Without Ammo";

	public string reloadProjectileCloseAnimatorName = "Reload Close";

	public string drawAnimatorname = "Draw";

	public string holsterAnimatorName = "Holster";

	public string aimInAnimatorName = "Aim In";
	public string aimOutAnimatorName = "Aim Out";

	public string meleeAttackAnimatorName = "Melee Attack";

	public string noAmmoAnimatorName = "No Ammo";

	public string shootLastProjectileAnimatorName = "Shoot Last Projectile";


	[Space]
	[Header ("Animation Duration Settings")]
	[Space]

	public float shootDuration = 0.1f;

	public float reloadWithAmmoDuration = 4.5f;
	public float reloadWithoutAmmoDuration = 4.5f;

	public float drawDuration = 0.8f;

	public float holsterDuration = 0.8f;

	[Space]
	[Header ("Melee Settings")]
	[Space]

	public bool useMeleeAttackAnimation;

	public float meleeAttackAnimationDuration = 1;

	public float meleeAttackStartDuration = 0.4f;

	public float meleeAttackMiddleDuration = 0.3f;

	public float meleeAttackEndDuration = 0.3f;

	public bool useMultipleMeleeAttacks;

	public List<weaponMeleeAttackInfo> meleeAttackAnimatorNameList = new List<weaponMeleeAttackInfo> ();


	[Space]
	[Header ("Reload Settings")]
	[Space]

	public bool useReloadPosition;
	public Transform reloadPosition;

	public bool reloadProjectilesOneByOne;

	public float reloadProjectileOpenDuration = 0.5f;

	public float reloadProjectileCloseDuration = 0.5f;

	public bool useReloadProjectileClose;

	public bool pauseRegularReloadSound;

	[Space]
	[Header ("Others Settings")]
	[Space]

	public bool useIdleAnimation = true;

	public bool useWalkAnimation = true;
	public bool useRunAnimation = true;

	public bool useAimInAnimation = true;
	public bool useAimOutAnimation = true;

	public bool useAimExtraMovement = true;

	public bool useWalkRunExtraMovement;

	public bool useAnimationForRecoilWithoutAim;
	public bool useAnimationForRecoilWithAim;

	public bool checkIfNoAmmoRemain;

	[Space]
	[Header ("Weapon Parts Settings")]
	[Space]

	public Transform mainWeaponMeshParent;

	public Vector3 weaponMeshOffsetAnimatorFirstPerson;

	public List<weaponMeshPartInfo> weaponMeshPartInfoList = new List<weaponMeshPartInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool idleCurrentlyActive;
	public bool walkCurrentlyActive;
	public bool runCurrentlyActive;

	public bool idleActive;

	public bool walkActive;

	public bool runActive;

	public bool shootActive;
	public bool reloadActive;

	public bool drawActive;
	public bool holsterActive;

	public bool aimActive;
	public bool aimInActive;
	public bool aimOutActive;

	public bool reloadWithoutAmmoActive;

	public bool aimingPreviously;

	public bool weaponInFirstPersonStateActive;

	public bool meleeAttackActive;

	public bool meleeAttackCurrentlyActive;

	public bool noAmmoActive;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnReloadProjectileOpen;
	public UnityEvent eventOnReloadProjectileOneByone;
	public UnityEvent eventOnReloadProjectileClose;

	[Space]
	[Header ("Components")]
	[Space]

	public IKWeaponSystem mainIKWeaponSystem;

	public Animator weaponAnimator;


	int idleAnimatorID;
	int walkAnimatorID;
	int runAnimatorID;

	int shootAnimatorID;
	int reloadWithAmmoAnimatorID;
	int reloadWithoutAmmoAnimatorID;

	int drawAnimatorID;
	int hoslterAnimatorID;

	int aimInAnimatorID;
	int aimOutAnimatorID;

	int shootAimAnimatorID;

	int reloadProjectileCloseAnimatorID;

	int meleeAttackAnimatorID;

	int noAmmoAnimatorID;

	int shootLastProjectileAnimatorID;

	Coroutine animatorCoroutine;

	weaponMeleeAttackInfo currentWeaponMeleeAttackInfo;

	void Start ()
	{
		idleAnimatorID = Animator.StringToHash (idleAnimatorName);
		walkAnimatorID = Animator.StringToHash (walkAnimatorName);
		runAnimatorID = Animator.StringToHash (runAnimatorName);

		shootAnimatorID = Animator.StringToHash (shootAnimatorName);
		shootAimAnimatorID = Animator.StringToHash (shootAimAnimatorName);

		reloadWithAmmoAnimatorID = Animator.StringToHash (reloadWithAmmoAnimatorName);
		reloadWithoutAmmoAnimatorID = Animator.StringToHash (reloadWithoutAmmoAnimatorName);

		drawAnimatorID = Animator.StringToHash (drawAnimatorname);
		hoslterAnimatorID = Animator.StringToHash (holsterAnimatorName);

		aimInAnimatorID = Animator.StringToHash (aimInAnimatorName);
		aimOutAnimatorID = Animator.StringToHash (aimOutAnimatorName);

		meleeAttackAnimatorID = Animator.StringToHash (meleeAttackAnimatorName);

		reloadProjectileCloseAnimatorID = Animator.StringToHash (reloadProjectileCloseAnimatorName);

		noAmmoAnimatorID = Animator.StringToHash (noAmmoAnimatorName);

		shootLastProjectileAnimatorID = Animator.StringToHash (shootLastProjectileAnimatorName);
			
		for (int i = 0; i < weaponMeshPartInfoList.Count; i++) {
			if (weaponMeshPartInfoList [i].weaponMeshTransform != null) {
				weaponMeshPartInfoList [i].originalWeaponMeshPosition = weaponMeshPartInfoList [i].weaponMeshTransform.localPosition;
				weaponMeshPartInfoList [i].originalWeaponMeshRotation = weaponMeshPartInfoList [i].weaponMeshTransform.localRotation;
			}
		}
	}

	public void setIdleState (bool state)
	{
		if (!weaponAnimatorEnabled) {
			return;
		}

		if (reloadActive || shootActive || meleeAttackCurrentlyActive) {
			return;
		}

		if (idleCurrentlyActive == state) {
			return;
		}

		idleCurrentlyActive = state;

		idleActive = state;

		if (showDebugPrint) {
			print ("Set Idle State " + idleActive);
		}

		if (idleActive) {
			walkActive = false;

			runActive = false;

			shootActive = false;

			reloadActive = false;

			walkCurrentlyActive = false;
			runCurrentlyActive = false;

			stopAnimatorCoroutine ();
		}

		updateAnimatorValues ();
	}

	public void setWalkState (bool state)
	{
		if (!weaponAnimatorEnabled) {
			return;
		}

		if (reloadActive || shootActive || meleeAttackCurrentlyActive) {
			return;
		}

		if (walkCurrentlyActive == state) {
			return;
		}

		walkCurrentlyActive = state;

		walkActive = state;

		if (showDebugPrint) {
			print ("Set Walk State " + walkActive);
		}

		if (walkActive) {
			idleActive = false;

			runActive = false;

			shootActive = false;

			reloadActive = false;

			idleCurrentlyActive = false;
			runCurrentlyActive = false;

			stopAnimatorCoroutine ();
		}

		updateAnimatorValues ();
	}

	public void setRunState (bool state)
	{
		if (!weaponAnimatorEnabled) {
			return;
		}

		if (reloadActive || shootActive || meleeAttackCurrentlyActive) {
			return;
		}

		if (runCurrentlyActive == state) {
			return;
		}

		runCurrentlyActive = state;

		runActive = state;

		if (showDebugPrint) {
			print ("Set Run State " + runActive);
		}

		if (runActive) {
			idleActive = false;

			walkActive = false;

			shootActive = false;

			reloadActive = false;

			idleCurrentlyActive = false;
			walkCurrentlyActive = false;

			stopAnimatorCoroutine ();
		}

		updateAnimatorValues ();
	}

	public void setShootState (bool state)
	{
		if (!weaponAnimatorEnabled) {
			return;
		}

		shootActive = state;

		if (showDebugPrint) {
			print ("Set Shoot State " + shootActive);
		}

		if (shootActive) {
			idleActive = false;

			walkActive = false;

			runActive = false;

			reloadActive = false;

			aimingPreviously = aimActive;

			idleCurrentlyActive = false;
			walkCurrentlyActive = false;
			runCurrentlyActive = false;

			stopAnimatorCoroutine ();

			bool noRemainAmmo = false;

			if (checkIfNoAmmoRemain) {
//				print (mainIKWeaponSystem.isCurrentMagazineEmpty () + " " + mainIKWeaponSystem.isRemainAmmoEmpty () + " " +
//				mainIKWeaponSystem.getProjectilesInMagazine ());

				if (mainIKWeaponSystem.isRemainAmmoEmpty ()) {
					if (mainIKWeaponSystem.isCurrentMagazineEmpty () || mainIKWeaponSystem.getProjectilesInMagazine () == 1) {
						noRemainAmmo = true;

//						print ("no more bullets");
					}
				}
			}

			if (noRemainAmmo) {
				noAmmoActive = true;
			}

			animatorCoroutine = StartCoroutine (setShootStateCoroutine (noRemainAmmo));
		}

		updateAnimatorValues ();
	}

	public void setReloadState (bool state)
	{
		if (!weaponAnimatorEnabled) {
			return;
		}

		reloadActive = state;

		if (showDebugPrint) {
			print ("Set Reload State " + reloadActive);
		}

		if (reloadActive) {
			idleActive = false;

			walkActive = false;

			runActive = false;

			shootActive = false;

			aimingPreviously = aimActive;

			aimActive = false;

			idleCurrentlyActive = false;
			walkCurrentlyActive = false;
			runCurrentlyActive = false;

			reloadWithoutAmmoActive = mainIKWeaponSystem.isCurrentMagazineEmpty ();

			stopAnimatorCoroutine ();

			animatorCoroutine = StartCoroutine (setReloadtStateCoroutine ());
		}

		updateAnimatorValues ();
	}

	public void setAimState (bool state)
	{
		if (!weaponAnimatorEnabled) {
			return;
		}

		aimActive = state;

		if (showDebugPrint) {
			print ("Set Aim State " + aimActive);
		}

		if (aimActive) {
			aimInActive = true;
			aimOutActive = false;
		} else {
			aimInActive = false;
			aimOutActive = true;
		}

		idleActive = false;

		walkActive = false;

		runActive = false;

		reloadActive = false;

		idleCurrentlyActive = false;
		walkCurrentlyActive = false;
		runCurrentlyActive = false;


		updateAnimatorValues ();
	}

	public void setDrawState (bool state)
	{
		if (!weaponAnimatorEnabled) {
			return;
		}

		drawActive = state;

		if (showDebugPrint) {
			print ("Set Draw State " + drawActive);
		}

		if (drawActive) {
			setWeaponInFirstPersonStateActiveState (true);
		}

		updateAnimatorValues ();
	}

	public void setHolsterState (bool state)
	{
		if (!weaponAnimatorEnabled) {
			return;
		}

		holsterActive = state;

		if (showDebugPrint) {
			print ("Set Holster State " + holsterActive);
		}

		updateAnimatorValues ();
	}

	public bool setMeleeAttackState (bool state)
	{
		if (!weaponAnimatorEnabled) {
			return false;
		}

		if (reloadActive) {
			return false;
		}

		if (meleeAttackCurrentlyActive == state) {
			return false;
		}

		meleeAttackActive = state;

		meleeAttackCurrentlyActive = state;

		if (showDebugPrint) {
			print ("Set Melee Attack State " + meleeAttackActive);
		}

		if (meleeAttackActive) {
			idleActive = false;

			walkActive = false;

			runActive = false;

			shootActive = false;

			aimActive = false;

			idleCurrentlyActive = false;
			walkCurrentlyActive = false;
			runCurrentlyActive = false;

			stopAnimatorCoroutine ();

			animatorCoroutine = StartCoroutine (setMeleeAttackStateCoroutine ());
		}

		updateAnimatorValues ();

		return true;
	}

	public void stopAnimatorCoroutine ()
	{
		if (animatorCoroutine != null) {
			StopCoroutine (animatorCoroutine);
		}
	}

	IEnumerator setShootStateCoroutine (bool noRemainAmmo)
	{
		if (noRemainAmmo) {
			yield return new WaitForSeconds (shootDuration / 2);
		} else {
			yield return new WaitForSeconds (shootDuration);
		}

		mainIKWeaponSystem.shootAnimatorStateComplete ();

		shootActive = false;

		if (aimingPreviously) {
			aimActive = true;

		} else {
			idleActive = true;

			idleCurrentlyActive = true;
		}

		if (noRemainAmmo) {
			noAmmoActive = true;
		}

		updateAnimatorValues ();
	}

	IEnumerator setReloadtStateCoroutine ()
	{
		if (reloadProjectilesOneByOne) {
			if (pauseRegularReloadSound) {
				mainIKWeaponSystem.pauseOrResumePlaySoundOnWeapon (true);
			}

			reloadWithoutAmmoActive = false;

			int weaponMagazine = mainIKWeaponSystem.getWeaponSystemManager ().getMagazineSize ();

			int remainProjectilesInMagazine = mainIKWeaponSystem.getProjectilesInMagazine ();

			int numberOfProjectilesToReload = weaponMagazine - remainProjectilesInMagazine;

			if (showDebugPrint) {
				print (numberOfProjectilesToReload);
			}

			mainIKWeaponSystem.setNewReloadTimeFirstPerson (
				(numberOfProjectilesToReload * reloadWithAmmoDuration) + reloadProjectileOpenDuration + reloadProjectileCloseDuration);

			eventOnReloadProjectileOpen.Invoke ();

			yield return new WaitForSeconds (reloadProjectileOpenDuration);

			while (numberOfProjectilesToReload > 0) {

				numberOfProjectilesToReload--;

				eventOnReloadProjectileOneByone.Invoke ();

				yield return new WaitForSeconds (reloadWithAmmoDuration);

				mainIKWeaponSystem.reloadSingleProjectile ();
			}

			if (useReloadProjectileClose) {
				weaponAnimator.CrossFadeInFixedTime (reloadProjectileCloseAnimatorID, 0.1f);
			}

			eventOnReloadProjectileClose.Invoke ();

			yield return new WaitForSeconds (reloadProjectileCloseDuration);

			if (pauseRegularReloadSound) {
				mainIKWeaponSystem.pauseOrResumePlaySoundOnWeapon (false);
			}
		} else {
			if (reloadWithoutAmmoActive) {
				yield return new WaitForSeconds (reloadWithoutAmmoDuration);
			} else {
				yield return new WaitForSeconds (reloadWithAmmoDuration);
			}
		}

		mainIKWeaponSystem.reloadAnimatorStateComplete ();

		reloadActive = false;

		if (aimingPreviously) {
			aimActive = true;

			aimInActive = true;
			aimOutActive = false;
		} else {
			idleActive = true;

			idleCurrentlyActive = true;
		}

		updateAnimatorValues ();
	}

	IEnumerator setMeleeAttackStateCoroutine ()
	{
		if (useMultipleMeleeAttacks) {
			yield return new WaitForSeconds (currentWeaponMeleeAttackInfo.meleeAttackAnimationDuration);
		} else {
			yield return new WaitForSeconds (meleeAttackAnimationDuration);
		}

		meleeAttackCurrentlyActive = false;

		idleActive = true;

		idleCurrentlyActive = true;

		updateAnimatorValues ();
	}


	public void updateAnimatorValues ()
	{
		if (idleActive) {
			if (useIdleAnimation) {
				if (noAmmoActive) {
					weaponAnimator.CrossFadeInFixedTime (noAmmoAnimatorID, 0.1f);
				} else {
					weaponAnimator.CrossFadeInFixedTime (idleAnimatorID, 0.1f);
				}
			}


//			print (noAmmoActive);

			noAmmoActive = false;

			idleActive = false;
		} else {
			if (walkActive) {
				if (useWalkAnimation) {
					weaponAnimator.CrossFadeInFixedTime (walkAnimatorID, 0.1f);
				}

				walkActive = false;
			} else if (runActive) {
				if (useRunAnimation) {
					weaponAnimator.CrossFadeInFixedTime (runAnimatorID, 0.1f);
				}

				runActive = false;
			}
		}

		if (drawActive) {
			weaponAnimator.CrossFadeInFixedTime (drawAnimatorID, 0.1f);

			drawActive = false;
		}

		if (holsterActive) {
			weaponAnimator.CrossFadeInFixedTime (hoslterAnimatorID, 0.1f);

			holsterActive = false;
		}

		if (aimActive) {
			if (aimInActive) {
				if (useAimInAnimation) {
					weaponAnimator.CrossFadeInFixedTime (aimInAnimatorID, 0.1f);
				}

				aimInActive = false;
			}
		} else {
			if (aimOutActive) {
				if (useAimOutAnimation) {
					weaponAnimator.CrossFadeInFixedTime (aimOutAnimatorID, 0.1f);
				}

				aimOutActive = false;
			}
		}

		if (shootActive) {

//			print (noAmmoActive);

			if (noAmmoActive) {
				weaponAnimator.CrossFadeInFixedTime (shootLastProjectileAnimatorID, 0.1f);

				noAmmoActive = false;
			} else {
				if (aimActive) {
					weaponAnimator.CrossFadeInFixedTime (shootAimAnimatorID, 0.1f);
				} else {
					weaponAnimator.CrossFadeInFixedTime (shootAnimatorID, 0.1f);
				}
			}
		} else {
			if (reloadActive) {
				if (reloadWithoutAmmoActive) {
					weaponAnimator.CrossFadeInFixedTime (reloadWithoutAmmoAnimatorID, 0.1f);
				} else {
					weaponAnimator.CrossFadeInFixedTime (reloadWithAmmoAnimatorID, 0.1f);
				}
			} else {
				if (meleeAttackActive) {
					weaponAnimator.CrossFadeInFixedTime (meleeAttackAnimatorID, 0.1f);

					meleeAttackActive = false;
				}
			}
		}
	}


	public void setWeaponInFirstPersonStateActiveState (bool state)
	{
		if (!weaponAnimatorEnabled) {
			return;
		}

		weaponInFirstPersonStateActive = state;

		weaponAnimator.enabled = weaponInFirstPersonStateActive;

		if (weaponInFirstPersonStateActive) {
			weaponAnimator.Rebind ();

			weaponAnimator.Update (0.1f);

			mainWeaponMeshParent.localPosition = weaponMeshOffsetAnimatorFirstPerson;
		} else {
			mainWeaponMeshParent.localPosition = Vector3.zero;

			for (int i = 0; i < weaponMeshPartInfoList.Count; i++) {
				if (weaponMeshPartInfoList [i].weaponMeshTransform != null) {
					weaponMeshPartInfoList [i].weaponMeshTransform.localPosition = weaponMeshPartInfoList [i].originalWeaponMeshPosition;
					weaponMeshPartInfoList [i].weaponMeshTransform.localRotation = weaponMeshPartInfoList [i].originalWeaponMeshRotation;
				}
			}

			stopAnimatorCoroutine ();

			resetActiveStates ();

		}
	}

	public void resetActiveStates ()
	{
		idleCurrentlyActive = false;
		walkCurrentlyActive = false;
		runCurrentlyActive = false;

		idleActive = false;

		walkActive = false;

		runActive = false;

		shootActive = false;
		reloadActive = false;

		drawActive = false;
		holsterActive = false;

		aimActive = false;

		aimInActive = false;
		aimOutActive = false;

		reloadWithoutAmmoActive = false;

		aimingPreviously = false;

		meleeAttackActive = false;

		meleeAttackCurrentlyActive = false;

		noAmmoActive = false;
	}

	public float getDrawDuration ()
	{
		return drawDuration;
	}

	public float getHolsterDuration ()
	{
		return holsterDuration;
	}

	public void setMeleeAttackInfo ()
	{
		if (useMultipleMeleeAttacks) {
			int randomIndex = Random.Range (0, meleeAttackAnimatorNameList.Count - 1);

			currentWeaponMeleeAttackInfo = meleeAttackAnimatorNameList [randomIndex];
		} else {
			currentWeaponMeleeAttackInfo = null;
		}
	}

	public float getMeleeAttackStartDuration ()
	{
		if (useMultipleMeleeAttacks) {
			return currentWeaponMeleeAttackInfo.meleeAttackStartDuration;
		} else {
			return meleeAttackStartDuration;
		}
	}

	public float getMeleeAttackMiddleDuration ()
	{
		if (useMultipleMeleeAttacks) {
			return currentWeaponMeleeAttackInfo.meleeAttackMiddleDuration;
		} else {
			return meleeAttackMiddleDuration;
		}
	}

	public float getMeleeAttackEndDuration ()
	{
		if (useMultipleMeleeAttacks) {
			return currentWeaponMeleeAttackInfo.meleeAttackEndDuration;
		} else {
			return meleeAttackEndDuration;
		}
	}

	public void enableOrDisableWeaponAnimator (bool state)
	{
		if (mainIKWeaponSystem) {
			weaponAnimatorEnabled = state;

			mainIKWeaponSystem.useWeaponAnimatorFirstPerson = weaponAnimatorEnabled;

			if (mainIKWeaponSystem.mainWeaponAnimatorSystem == null) {
				mainIKWeaponSystem.mainWeaponAnimatorSystem = this;
			}

			GKC_Utils.updateComponent (mainIKWeaponSystem);

			if (weaponAnimator == null) {
				weaponAnimator = GetComponentInChildren<Animator> ();
			}

			GKC_Utils.updateComponent (this);
		}
	}

	[System.Serializable]
	public class weaponMeshPartInfo
	{
		public Transform weaponMeshTransform;

		[HideInInspector] public Vector3 originalWeaponMeshPosition;
		[HideInInspector] public Quaternion originalWeaponMeshRotation;
	}

	[System.Serializable]
	public class weaponMeleeAttackInfo
	{
		public string meleeAttackAnimatorName = "Melee Attack";

		public float meleeAttackAnimationDuration = 1;

		public float meleeAttackStartDuration = 0.4f;

		public float meleeAttackMiddleDuration = 0.3f;

		public float meleeAttackEndDuration = 0.3f;
	}
}
