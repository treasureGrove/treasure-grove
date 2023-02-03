using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class searchMeleeSurfaceSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool searchSurfaceEnabled = true;

	public LayerMask layerToCheck;

	public float raycastDistance = 2;

	public Transform playerTransform;

	public string surfaceInfoOnMeleeAttackNameForSwingOnAir = "Swing On Air";

	public float maxDistanceToHiddenSurfaces = 3;

	[Space]
	[Header ("Extraction Settings")]
	[Space]

	public bool checkForMaterialsZoneToExtract;

	public float maxDetectionDistanceToExtract = 10;

	public bool useMaxMaterialZonesToExtractAtSameTime;
	public int maxMaterialZonesToExtractAtSameTime;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public GameObject lastSurfaceDetected;

	[Space]
	[Header ("Components")]
	[Space]

	public grabbedObjectMeleeAttackSystem mainGrabbedObjectMeleeAttackSystem;

	RaycastHit hit;

	Vector3 attackPosition;
	Vector3 attackNormal;

	List<materialsZoneSystem> materialsZoneSystemLocatedList = new List<materialsZoneSystem> ();


	public void checkSurfaceDown ()
	{
		checkSurfaceType (true);
	}

	public void checkHiddenSurfacesDown ()
	{
		checkSurfaceType (false);
	}

	void checkSurfaceType (bool checkByRaycast)
	{
		lastSurfaceDetected = null;

		attackPosition = Vector3.zero;
		attackNormal = Vector3.zero;

		if (checkByRaycast) {
			Vector3 raycastPosition = playerTransform.position + playerTransform.up;

			Vector3 raycastDirection = -playerTransform.up;

			if (Physics.Raycast (raycastPosition, raycastDirection, out hit, raycastDistance, layerToCheck)) {
				if (hit.collider.transform == playerTransform) {
					raycastPosition = hit.point;

					if (Physics.Raycast (raycastPosition, raycastDirection, out hit, raycastDistance, layerToCheck)) {
						lastSurfaceDetected = hit.collider.gameObject;

						attackPosition = hit.point;
						attackNormal = hit.normal;
					}
				} else {
					lastSurfaceDetected = hit.collider.gameObject;

					attackPosition = hit.point;
					attackNormal = hit.normal;
				}
			}

			checkSurface ();
		} else {
			hiddenMeleeSurface[] hiddenMeleeSurfaceList = FindObjectsOfType<hiddenMeleeSurface> ();

			foreach (hiddenMeleeSurface currentHiddenMeleeSurface in hiddenMeleeSurfaceList) {
				if (currentHiddenMeleeSurface.isSurfaceEnabled ()) {
					float currentDistance = GKC_Utils.distance (playerTransform.position, currentHiddenMeleeSurface.transform.position);

					if (currentDistance < maxDistanceToHiddenSurfaces) {
						lastSurfaceDetected = currentHiddenMeleeSurface.gameObject;

						Vector3 raycastPosition = playerTransform.position + playerTransform.up;

						Vector3 heading = lastSurfaceDetected.transform.position - raycastPosition;

						float distance = heading.magnitude;

						Vector3 raycastDirection = heading / distance;

						if (Physics.Raycast (raycastPosition, raycastDirection, out hit, raycastDistance, layerToCheck)) {

							attackPosition = hit.point;
							attackNormal = hit.normal;

							checkSurface ();
						}
					}
				}
			}
		}

		if (checkForMaterialsZoneToExtract) {
			materialsZoneSystemLocatedList.Clear ();

			materialsZoneSystem[] temporalMaterialsZoneSystem = FindObjectsOfType<materialsZoneSystem> ();

			foreach (materialsZoneSystem currentMaterialsZoneSystem in temporalMaterialsZoneSystem) {
				if (currentMaterialsZoneSystem.isMaterialsZoneEnabled ()) {
					bool addZoneResult = false;

					float currentDistance = GKC_Utils.distance (playerTransform.position, currentMaterialsZoneSystem.transform.position);

					if (currentDistance < maxDetectionDistanceToExtract) {
						addZoneResult = true;
					}

					if (useMaxMaterialZonesToExtractAtSameTime) {
						if (materialsZoneSystemLocatedList.Count >= maxMaterialZonesToExtractAtSameTime) {
							addZoneResult = false;
						}
					}

					if (addZoneResult) {
						materialsZoneSystemLocatedList.Add (currentMaterialsZoneSystem);
					}
				}
			}

			if (materialsZoneSystemLocatedList.Count > 0) {
				for (int i = 0; i < materialsZoneSystemLocatedList.Count; i++) {
					if (materialsZoneSystemLocatedList [i].isCanBeExtractedByExternalElementsEnabled () &&
					    !materialsZoneSystemLocatedList [i].isMaterialsZoneEmpty ()) {

						materialsZoneSystemLocatedList [i].checkMaterialZoneToExtractExternally ();

						materialsZoneSystemLocatedList [i].setMaterialsZoneFullState (false);
					}
				}
			}
		}
	}

	public void checkSurface ()
	{
		if (!searchSurfaceEnabled) {
			return;
		}

		if (!mainGrabbedObjectMeleeAttackSystem.grabbedObjectMeleeAttackActive) {
			return;
		}

		string surfaceName = surfaceInfoOnMeleeAttackNameForSwingOnAir;

		bool surfaceLocated = true;

		if (lastSurfaceDetected != null) {
			if (showDebugPrint) {
				print ("checking surface " + lastSurfaceDetected.name);
			}

			meleeAttackSurfaceInfo currentMeleeAttackSurfaceInfo = lastSurfaceDetected.GetComponent<meleeAttackSurfaceInfo> ();

			if (currentMeleeAttackSurfaceInfo != null) {
				if (!currentMeleeAttackSurfaceInfo.isSurfaceEnabled ()) {
					surfaceLocated = false;
				} else {

					surfaceName = currentMeleeAttackSurfaceInfo.getSurfaceName ();

					grabPhysicalObjectMeleeAttackSystem currentGrabPhysicalObjectMeleeAttackSystem = mainGrabbedObjectMeleeAttackSystem.getCurrentGrabPhysicalObjectMeleeAttackSystem ();

					if (currentMeleeAttackSurfaceInfo.useRemoteEventOnWeapon) {
						remoteEventSystem currentRemoteEventSystem = currentGrabPhysicalObjectMeleeAttackSystem.GetComponent<remoteEventSystem> ();

						if (currentRemoteEventSystem != null) {

							if (showDebugPrint) {
								print ("remote event on weapon detected");
							}

							for (int j = 0; j < currentMeleeAttackSurfaceInfo.remoteEventOnWeaponNameList.Count; j++) {
								currentRemoteEventSystem.callRemoteEvent (currentMeleeAttackSurfaceInfo.remoteEventOnWeaponNameList [j]);
							}
						}
					}

					if (currentGrabPhysicalObjectMeleeAttackSystem.useRemoteEventOnSurfacesDetected) {
						if (currentMeleeAttackSurfaceInfo.useRemoteEvent) {
							remoteEventSystem currentRemoteEventSystem = lastSurfaceDetected.GetComponent<remoteEventSystem> ();

							if (currentRemoteEventSystem != null) {

								if (showDebugPrint) {
									print ("remote event on object detected");
								}

								for (int j = 0; j < currentMeleeAttackSurfaceInfo.remoteEventNameList.Count; j++) {
									string currentRemoteEventName = currentMeleeAttackSurfaceInfo.remoteEventNameList [j];

									if (currentGrabPhysicalObjectMeleeAttackSystem.isRemoteEventIncluded (currentRemoteEventName)) {
										currentRemoteEventSystem.callRemoteEvent (currentRemoteEventName);
									}
								}
							}
						}
					}

					currentMeleeAttackSurfaceInfo.checkEventOnSurfaceDetected ();
				}
			} else {
				GameObject currentCharacter = applyDamage.getCharacterOrVehicle (lastSurfaceDetected);

				if (currentCharacter != null) {
					currentMeleeAttackSurfaceInfo = currentCharacter.GetComponent<meleeAttackSurfaceInfo> ();

					if (currentMeleeAttackSurfaceInfo != null) {
						if (!currentMeleeAttackSurfaceInfo.isSurfaceEnabled ()) {
							surfaceLocated = false;
						} else {
							surfaceName = currentMeleeAttackSurfaceInfo.getSurfaceName ();

							currentMeleeAttackSurfaceInfo.checkEventOnSurfaceDetected ();
						}
					}
				} else {
					surfaceLocated = false;
				}

				if (!surfaceLocated) {
					return;
				}
			}
		} else {
			if (showDebugPrint) {
				print ("SURFACE NOT FOUND BY TRIGGER!!!!!!!!!!");
			}
		}

		bool ignoreBounceEvent = false;

		bool ignoreSoundOnSurface = false;

		mainGrabbedObjectMeleeAttackSystem.checkSurfaceFoundOnAttackToProcess (surfaceName, surfaceLocated, attackPosition, attackNormal, ignoreBounceEvent, ignoreSoundOnSurface);
	}
}
