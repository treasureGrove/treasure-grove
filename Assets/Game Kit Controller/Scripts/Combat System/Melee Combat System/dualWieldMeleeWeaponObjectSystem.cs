using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dualWieldMeleeWeaponObjectSystem : MonoBehaviour
{
	[Space]
	[Header ("Components")]
	[Space]

	public hitCombat mainHitCombat;

	public grabbedObjectMeleeAttackSystem currentGrabbedObjectMeleeAttackSystem;

	public Transform meleeWeaponObject;

	public Transform meleeWeaponParent;

	public Transform meleeWeaponHandTransformReference;

	public Transform meleeWeaponTransformReference;



	public Transform raycastCheckTransfrom;

	public Transform mainDamagePositionTransform;

	[Space]
	[Header ("Cutting Mode Settings")]
	[Space]

	public Transform cutPositionTransform;
	public Transform cutDirectionTransform;

	public Transform planeDefiner1;
	public Transform planeDefiner2;
	public Transform planeDefiner3;

	[Space]
	[Header ("Damage Detection Settings")]
	[Space]

	public List<Transform> raycastCheckTransfromList = new List<Transform> ();

	Vector3 originalHitCombatColliderSize;

	BoxCollider currentHitCombatBoxCollider;

	Vector3 originalHitCombatColliderCenter;

	Transform leftHandMountPoint;

	public void enableDualWieldMeleeweapobObject (grabbedObjectMeleeAttackSystem newGrabbedObjectMeleeAttackSystem, bool useEventsOnDamageDetected)
	{
		currentGrabbedObjectMeleeAttackSystem = newGrabbedObjectMeleeAttackSystem;

		if (currentGrabbedObjectMeleeAttackSystem.useCustomLayerToDetectSurfaces) {
			mainHitCombat.setCustomLayerMask (currentGrabbedObjectMeleeAttackSystem.customLayerToDetectSurfaces);
		}

		if (currentGrabbedObjectMeleeAttackSystem.useCustomIgnoreTags) {
			mainHitCombat.setCustomTagsToIgnore (currentGrabbedObjectMeleeAttackSystem.customTagsToIgnoreList);
		} else {
			mainHitCombat.setCustomTagsToIgnore (null);
		}

		mainHitCombat.setCustomDamageCanBeBlockedState (true);

		currentHitCombatBoxCollider = mainHitCombat.getMainCollider ().GetComponent<BoxCollider> ();

		originalHitCombatColliderCenter = currentHitCombatBoxCollider.center;

		originalHitCombatColliderSize = currentHitCombatBoxCollider.size;

		mainHitCombat.getOwner (currentGrabbedObjectMeleeAttackSystem.playerControllerGameObject);

		mainHitCombat.setMainColliderEnabledState (true);

		mainHitCombat.setSendMessageOnDamageDetectedState (useEventsOnDamageDetected);

		if (useEventsOnDamageDetected) {
			mainHitCombat.setCustomObjectToSendMessage (currentGrabbedObjectMeleeAttackSystem.gameObject);
		}

		leftHandMountPoint = currentGrabbedObjectMeleeAttackSystem.getLeftHandMountPoint ();

		meleeWeaponObject.SetParent (leftHandMountPoint);

		Vector3 localPosition = meleeWeaponHandTransformReference.localPosition;
		Quaternion localRotation = meleeWeaponHandTransformReference.localRotation;

		meleeWeaponObject.localPosition = localPosition;
		meleeWeaponObject.localRotation = localRotation;

		enableOrDisableDualWieldMeleeWeaponObject (true);
	}

	public void enableOrDisableDualWieldMeleeWeaponObject (bool state)
	{
		if (meleeWeaponObject.gameObject.activeSelf != state) {
			meleeWeaponObject.gameObject.SetActive (state);
		}
	}

	public void disableDualWieldMeleeweapobObject ()
	{
		mainHitCombat.setMainColliderEnabledState (false);

		enableOrDisableDualWieldMeleeWeaponObject (true);

		meleeWeaponObject.SetParent (meleeWeaponParent);

		Vector3 localPosition = meleeWeaponTransformReference.localPosition;
		Quaternion localRotation = meleeWeaponTransformReference.localRotation;

		meleeWeaponObject.localPosition = localPosition;
		meleeWeaponObject.localRotation = localRotation;
	}

	public void setHitCombatScale (Vector3 newScale)
	{
		if (currentHitCombatBoxCollider != null) {
			currentHitCombatBoxCollider.size = newScale;
		}
	}

	public void setHitCombatOffset (Vector3 newValue)
	{
		if (currentHitCombatBoxCollider != null) {
			currentHitCombatBoxCollider.center = newValue;
		}
	}

	public void setOriginalHitCombatScale ()
	{
		setHitCombatScale (originalHitCombatColliderSize);
	}

	public void setOriginalHitCombatOffset ()
	{
		setHitCombatOffset (originalHitCombatColliderCenter);
	}
}
