using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeWeaponTransformInfoSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool editWeaponTransformValuesIngame;

	[Space]

	public Vector3 weaponPosition;
	public Vector3 weaponRotation;

	[Space]

	public Transform currentWeaponObjectTransform;

	public GameObject currentWeaponMeshGameObject;

	[Space]
	[Header ("Melee Object Data Settings")]
	[Space]

	public bool showGizmo;

	[Space]
	[Header ("Melee Object Data Settings")]
	[Space]

	public grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;

	public meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager;

	public meleeWeaponTransformData mainMeleeWeaponHandTransformData;
	public meleeWeaponTransformData mainMeleeWeaponMeshTransformData;

	Coroutine updateCoroutine;


	public void toggleEditWeaponTransformValuesIngameStateState ()
	{
		setEditWeaponTransformValuesIngameState (!editWeaponTransformValuesIngame);
	}

	public void setEditWeaponTransformValuesIngameState (bool state)
	{
		editWeaponTransformValuesIngame = state;

		stopUpdateCoroutine ();

		if (editWeaponTransformValuesIngame) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		}

		currentWeaponObjectTransform = null;

		currentWeaponMeshGameObject = null;

		showGizmo = false;
	}

	public void selectCurrentWeaponObjectInGame ()
	{
		if (Application.isPlaying) {
			if (mainGrabbedObjectMeleeAttackSystem.carryingObject) {
				currentWeaponObjectTransform = mainGrabbedObjectMeleeAttackSystem.getCurrentGrabbedObjectTransform ();

				if (currentWeaponObjectTransform != null) {
					GKC_Utils.setActiveGameObjectInEditor (currentWeaponObjectTransform.gameObject);
				}
			} else {
				string meleeWeaponName = mainMeleeWeaponsGrabbedManager.getCurrentWeaponActiveName ();

				if (meleeWeaponName != "") {
					currentWeaponMeshGameObject = mainMeleeWeaponsGrabbedManager.getCurrentWeaponMeshByName (meleeWeaponName);

					if (currentWeaponMeshGameObject != null) {
						GKC_Utils.setActiveGameObjectInEditor (currentWeaponMeshGameObject);
					}
				}
			}
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (editWeaponTransformValuesIngame) {
			if (mainGrabbedObjectMeleeAttackSystem.carryingObject) {
				if (currentWeaponObjectTransform != null) {
					currentWeaponObjectTransform.localPosition = weaponPosition;

					currentWeaponObjectTransform.localEulerAngles = weaponRotation;
				} else {
					currentWeaponObjectTransform = mainGrabbedObjectMeleeAttackSystem.getCurrentGrabbedObjectTransform ();
					weaponPosition = currentWeaponObjectTransform.localPosition;

					weaponRotation = currentWeaponObjectTransform.localEulerAngles;
				}
			} else {
				string meleeWeaponName = mainMeleeWeaponsGrabbedManager.getCurrentWeaponActiveName ();

				if (meleeWeaponName != "") {
					if (currentWeaponMeshGameObject != null) {
						currentWeaponObjectTransform.localPosition = weaponPosition;

						currentWeaponObjectTransform.localEulerAngles = weaponRotation;
					} else {
						currentWeaponMeshGameObject = mainMeleeWeaponsGrabbedManager.getCurrentWeaponMeshByName (meleeWeaponName);

						currentWeaponObjectTransform = currentWeaponMeshGameObject.transform;

						weaponPosition = currentWeaponObjectTransform.localPosition;

						weaponRotation = currentWeaponObjectTransform.localEulerAngles;
					}
				}
			}
		}
	}

	public void copyTransformValuesToBuffer ()
	{
		if (mainGrabbedObjectMeleeAttackSystem.carryingObject) {
			objectTransformInfo newObjectTransformInfo = new objectTransformInfo ();

			Transform currentGrabbedObjectTransform = mainGrabbedObjectMeleeAttackSystem.getCurrentGrabbedObjectTransform ();

			if (currentGrabbedObjectTransform) {
				newObjectTransformInfo.objectPosition = currentGrabbedObjectTransform.localPosition;
				newObjectTransformInfo.objectRotation = currentGrabbedObjectTransform.localRotation;

				mainMeleeWeaponHandTransformData.meleeWeaponName = mainGrabbedObjectMeleeAttackSystem.getCurrentMeleeWeaponTypeName ();

				mainMeleeWeaponHandTransformData.mainObjectTransformInfo = newObjectTransformInfo;
			}
		}

		string meleeWeaponName = mainMeleeWeaponsGrabbedManager.getCurrentWeaponActiveName ();

		if (meleeWeaponName != "") {
			currentWeaponMeshGameObject = mainMeleeWeaponsGrabbedManager.getCurrentWeaponMeshByName (meleeWeaponName);
		
			if (currentWeaponMeshGameObject != null) {
				Transform currentWeaponMeshTransform = currentWeaponMeshGameObject.transform;

				objectTransformInfo newObjectTransformInfo = new objectTransformInfo ();

				newObjectTransformInfo.objectPosition = currentWeaponMeshTransform.localPosition;
				newObjectTransformInfo.objectRotation = currentWeaponMeshTransform.localRotation;

				mainMeleeWeaponMeshTransformData.meleeWeaponName = meleeWeaponName;

				mainMeleeWeaponMeshTransformData.mainObjectTransformInfo = newObjectTransformInfo;
			}
		}
	}

	public void pasteTransformValuesToBuffer ()
	{
		objectTransformInfo newWeaponHandTransformInfo = mainMeleeWeaponHandTransformData.mainObjectTransformInfo;

		objectTransformInfo newWeaponMeshTransformInfo = mainMeleeWeaponMeshTransformData.mainObjectTransformInfo;

		for (int i = 0; i < mainGrabbedObjectMeleeAttackSystem.grabbedWeaponInfoList.Count; i++) {
			if (mainGrabbedObjectMeleeAttackSystem.grabbedWeaponInfoList [i].Name.Equals (mainMeleeWeaponHandTransformData.meleeWeaponName)) {

				Transform customGrabbedWeaponReferencePosition = mainGrabbedObjectMeleeAttackSystem.grabbedWeaponInfoList [i].customGrabbedWeaponReferencePosition;

				if (customGrabbedWeaponReferencePosition != null) {
					customGrabbedWeaponReferencePosition.localPosition = newWeaponHandTransformInfo.objectPosition;
					customGrabbedWeaponReferencePosition.localRotation = newWeaponHandTransformInfo.objectRotation;
				}

				Transform customReferencePositionToKeepObjectMesh = mainGrabbedObjectMeleeAttackSystem.grabbedWeaponInfoList [i].customReferencePositionToKeepObjectMesh;

				if (customReferencePositionToKeepObjectMesh != null) {
					customReferencePositionToKeepObjectMesh.localPosition = newWeaponMeshTransformInfo.objectPosition;
					customReferencePositionToKeepObjectMesh.localRotation = newWeaponMeshTransformInfo.objectRotation;
				}
			}
		}

		GKC_Utils.updateDirtyScene ("Adjusting melee weapon positions", gameObject);
	}

	public void cleanPositionsOnScriptable ()
	{
		objectTransformInfo newObjectTransformInfo = new objectTransformInfo ();

		newObjectTransformInfo.objectPosition = Vector3.zero;
		newObjectTransformInfo.objectRotation = Quaternion.identity;

		mainMeleeWeaponHandTransformData.meleeWeaponName = "";
		mainMeleeWeaponHandTransformData.mainObjectTransformInfo = newObjectTransformInfo;

		mainMeleeWeaponMeshTransformData.meleeWeaponName = "";
		mainMeleeWeaponMeshTransformData.mainObjectTransformInfo = newObjectTransformInfo;
	}

	public void toggleShowHandleGizmo ()
	{
		if (Application.isPlaying) {

			editWeaponTransformValuesIngame = false;

			stopUpdateCoroutine ();

			showGizmo = !showGizmo;

			if (showGizmo) {
				if (mainGrabbedObjectMeleeAttackSystem.carryingObject) {
					currentWeaponObjectTransform = mainGrabbedObjectMeleeAttackSystem.getCurrentGrabbedObjectTransform ();
				} else {
					string meleeWeaponName = mainMeleeWeaponsGrabbedManager.getCurrentWeaponActiveName ();

					if (meleeWeaponName != "") {
						currentWeaponMeshGameObject = mainMeleeWeaponsGrabbedManager.getCurrentWeaponMeshByName (meleeWeaponName);
					}
				}
			} else {
				currentWeaponObjectTransform = null;

				currentWeaponMeshGameObject = null;
			}
		}
	}
}
