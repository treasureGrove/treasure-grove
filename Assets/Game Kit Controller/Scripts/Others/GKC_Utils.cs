using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class GKC_Utils : MonoBehaviour
{
	public static float getCurrentDeltaTime ()
	{
		float timeScale = Time.timeScale;

		if (timeScale > 0) {
			return 1 / timeScale;
		} else {
			return 1;
		}
	}

	public static float getCurrentScaleTime ()
	{
		if (Time.timeScale != 1) {
			return ((1f / Time.fixedDeltaTime) * 0.02f);
		}

		return 1;
	}

	public static void checkAudioSourcePitch (AudioSource audioSourceToCheck)
	{
		if (audioSourceToCheck != null) {
			audioSourceToCheck.pitch = Time.timeScale;
		}
	}

	public static float distance (Vector3 positionA, Vector3 positionB)
	{
		return Mathf.Sqrt ((positionA - positionB).sqrMagnitude);
	}

	//the four directions of a swipe
	public class swipeDirections
	{
		public static Vector2 up = new Vector2 (0, 1);
		public static Vector2 down = new Vector2 (0, -1);
		public static Vector2 right = new Vector2 (1, 0);
		public static Vector2 left = new Vector2 (-1, 0);
	}

	public static void ForGizmo (Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Gizmos.DrawRay (pos, direction);

		Vector3 right = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 + arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Vector3 left = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 - arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Gizmos.DrawRay (pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay (pos + direction, left * arrowHeadLength);
	}

	public static void drawGizmoArrow (Vector3 pos, Vector3 direction, Color color, float arrowHeadLength, float arrowHeadAngle)
	{
		Gizmos.color = color;
		Gizmos.DrawRay (pos, direction);

		Vector3 currentLookDirection = direction;
		Quaternion lookRotation = Quaternion.identity;

		if (currentLookDirection == Vector3.zero) {
			currentLookDirection = Vector3.forward;
		}

		lookRotation = Quaternion.LookRotation (currentLookDirection);

		Vector3 right = lookRotation * Quaternion.Euler (0, 180 + arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Vector3 left = lookRotation * Quaternion.Euler (0, 180 - arrowHeadAngle, 0) * new Vector3 (0, 0, 1);

		Gizmos.DrawRay (pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay (pos + direction, left * arrowHeadLength);
	}

	public static void ForDebug (Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay (pos, direction);

		Vector3 right = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 + arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Vector3 left = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 - arrowHeadAngle, 0) * new Vector3 (0, 0, 1);

		Debug.DrawRay (pos + direction, right * arrowHeadLength);
		Debug.DrawRay (pos + direction, left * arrowHeadLength);
	}

	public static void ForDebug (Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay (pos, direction, color);

		Vector3 right = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 + arrowHeadAngle, 0) * new Vector3 (0, 0, 1);
		Vector3 left = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 180 - arrowHeadAngle, 0) * new Vector3 (0, 0, 1);

		Debug.DrawRay (pos + direction, right * arrowHeadLength, color);
		Debug.DrawRay (pos + direction, left * arrowHeadLength, color);
	}

	public static void drawCapsuleGizmo (Vector3 point1, Vector3 point2, float capsuleCastRadius, Color sphereColor, Color cubeColor, Vector3 currentRayTargetPosition, Vector3 rayDirection, float distanceToTarget)
	{
		Gizmos.color = sphereColor;

		Gizmos.DrawSphere (point1, capsuleCastRadius);
		Gizmos.DrawSphere (point2, capsuleCastRadius);

		Gizmos.color = cubeColor;
		Vector3 scale = new Vector3 (capsuleCastRadius * 2, capsuleCastRadius * 2, distanceToTarget - capsuleCastRadius * 2);
		Matrix4x4 cubeTransform = Matrix4x4.TRS ((rayDirection * (distanceToTarget / 2)) + currentRayTargetPosition, Quaternion.LookRotation (rayDirection, point1 - point2), scale);
		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

		Gizmos.matrix *= cubeTransform;

		Gizmos.DrawCube (Vector3.zero, Vector3.one);

		Gizmos.matrix = oldGizmosMatrix;
	}

	public static void drawRectangleGizmo (Vector3 rectanglePosition, Quaternion rectangleRotation, Vector3 positionOffset, Vector3 rectangleScale, Color rectangleColor)
	{
		Gizmos.color = rectangleColor;

		Matrix4x4 cubeTransform = Matrix4x4.TRS (rectanglePosition + positionOffset, rectangleRotation, rectangleScale);
		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

		Gizmos.matrix *= cubeTransform;

		Gizmos.DrawCube (Vector3.zero, Vector3.one);

		Gizmos.matrix = oldGizmosMatrix;
	}

	public static void checkDropObject (GameObject objectToDrop)
	{
		grabbedObjectState currentGrabbedObjectState = objectToDrop.GetComponent<grabbedObjectState> ();

		if (currentGrabbedObjectState != null) {
			dropObject (currentGrabbedObjectState.getCurrentHolder (), objectToDrop);
		}
	}

	public static void dropObject (GameObject currentPlayer, GameObject objectToDrop)
	{
		if (currentPlayer == null) {
			return;
		}

		grabObjects grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();

		if (grabObjectsManager != null) {
			grabObjectsManager.checkIfDropObject (objectToDrop);
		}
	}

	public static bool checkIfObjectCanBePlaced (GameObject currentPlayer, GameObject objectToDrop)
	{
		if (currentPlayer == null) {
			return true;
		}

		grabObjects grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();

		if (grabObjectsManager != null) {
			return grabObjectsManager.checkIfObjectCanBePlaced (objectToDrop);
		}

		return true;
	}

	public static void dropObject (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return;
		}

		grabObjects grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();

		if (grabObjectsManager != null) {
			grabObjectsManager.checkIfDropObject ();
		}
	}

	public static void dropObjectIfNotGrabbedPhysically (GameObject currentPlayer, bool dropIfGrabbedPhysicallyWithIK)
	{
		if (currentPlayer == null) {
			return;
		}

		grabObjects grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();

		if (grabObjectsManager != null) {
			if (grabObjectsManager.isCarryingPhysicalObject ()) {
				if (grabObjectsManager.isIKSystemEnabledOnCurrentGrabbedObject () && dropIfGrabbedPhysicallyWithIK) {
					grabObjectsManager.checkIfDropObject ();
				}
			} else {
				grabObjectsManager.checkIfDropObject ();
			}
		}
	}

	public static void checkIfKeepGrabbedObjectDuringAction (GameObject currentPlayer, bool keepGrabbedObjectOnActionIfNotDropped, bool keepGrabbedObject)
	{
		if (currentPlayer == null) {
			return;
		}

		grabObjects grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();

		if (grabObjectsManager != null) {
			if (grabObjectsManager.isGrabbedObject () & keepGrabbedObjectOnActionIfNotDropped) {
				grabObjectsManager.keepOrCarryGrabbebObject (keepGrabbedObject);
			}	
		}
	}

	public static void disableKeepGrabbedObjectStateAfterAction (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return;
		}

		checkIfKeepGrabbedObjectDuringAction (currentPlayer, true, false);
	}

	public static void keepMeleeWeaponGrabbed (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return;
		}

		grabObjects grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();

		if (grabObjectsManager != null) {
			if (grabObjectsManager.isGrabbedObject ()) {
				grabObjectsManager.mainGrabbedObjectMeleeAttackSystem.drawOrKeepMeleeWeapon ();
			}

			if (grabObjectsManager.isGrabbedObject ()) {
				grabObjectsManager.checkIfDropObject ();
			}
		}
	}

	public static void drawMeleeWeaponGrabbed (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return;
		}

		grabObjects grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();

		if (grabObjectsManager != null) {
			if (!grabObjectsManager.isGrabbedObject ()) {
				grabObjectsManager.mainGrabbedObjectMeleeAttackSystem.drawOrKeepMeleeWeaponWithoutCheckingInputActive ();
			}
		}
	}

	public static void drawMeleeWeaponGrabbedCheckingAnimationDelay (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return;
		}

		grabObjects grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();

		if (grabObjectsManager != null) {
			if (!grabObjectsManager.isGrabbedObject ()) {
				grabObjectsManager.mainGrabbedObjectMeleeAttackSystem.drawMeleeWeaponGrabbedCheckingAnimationDelay ();
			}
		}
	}

	public static void grabPhysicalObjectExternally (GameObject currentPlayer, GameObject objectToGrab)
	{
		if (currentPlayer == null) {
			return;
		}

		grabObjects grabObjectsManager = currentPlayer.GetComponent<grabObjects> ();

		if (grabObjectsManager != null) {
			if (!grabObjectsManager.isGrabbedObject ()) {
				grabObjectsManager.grabPhysicalObjectExternally (objectToGrab);
			}
		}
	}

	public static void enableOrDisableMeleeWeaponMeshActiveState (GameObject currentPlayer, bool state)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();
		
		if (currentplayerComponentsManager != null) {
			grabbedObjectMeleeAttackSystem currentGrabbedObjectMeleeAttackSystem = currentplayerComponentsManager.getGrabbedObjectMeleeAttackSystem ();

			if (currentGrabbedObjectMeleeAttackSystem != null) {
				if (currentGrabbedObjectMeleeAttackSystem.isCarryingObject ()) {
					currentGrabbedObjectMeleeAttackSystem.enableOrDisableWeaponMeshActiveState (state);
				}
			}
		}
	}

	public static void enableOrDisableFireWeaponMeshActiveState (GameObject currentPlayer, bool state)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			playerWeaponsManager currentPlayerWeaponsManager = currentplayerComponentsManager.getPlayerWeaponsManager ();

			if (currentPlayerWeaponsManager != null) {
				if (currentPlayerWeaponsManager.isUsingWeapons ()) {
					currentPlayerWeaponsManager.enableOrDisableCurrentWeaponsMesh (state);
				}
			}
		}
	}

	public static bool enableOrDisableIKOnWeaponsDuringAction (GameObject currentPlayer, bool state)
	{
		if (currentPlayer == null) {
			return false;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			playerWeaponsManager currentPlayerWeaponsManager = currentplayerComponentsManager.getPlayerWeaponsManager ();

			if (currentPlayerWeaponsManager != null) {
				if (state) {
					currentPlayerWeaponsManager.enableOrDisableIKOnWeaponsDuringAction (true);
		
				} else {
					if (currentPlayerWeaponsManager.isPlayerCarringWeapon ()) {
						currentPlayerWeaponsManager.enableOrDisableIKOnWeaponsDuringAction (false);

						return true;
					} 
				}
			}
		}

		return false;
	}

	public static void useObjectExternally (GameObject currentPlayer, GameObject objectToUse)
	{
		usingDevicesSystem usingDevicesManager = currentPlayer.GetComponent<usingDevicesSystem> ();

		if (usingDevicesManager != null) {
			inventoryManager currentInventoryManager = currentPlayer.GetComponent<inventoryManager> ();

			bool examineObjectBeforeStoreEnabled = false;

			if (currentInventoryManager != null) {
				examineObjectBeforeStoreEnabled = currentInventoryManager.examineObjectBeforeStoreEnabled;

				currentInventoryManager.setExamineObjectBeforeStoreEnabledState (false);
			}

			bool useMinDistanceToUseDevices = usingDevicesManager.useMinDistanceToUseDevices;

			Transform objectToCheckParent = objectToUse.transform.parent;

			if (objectToCheckParent == null) {
				objectToCheckParent = objectToUse.transform;
			}

			pickUpObject currentPickUpObject = objectToCheckParent.GetComponentInChildren<pickUpObject> ();

			if (currentPickUpObject != null) {
				currentPickUpObject.checkTriggerInfoByGameObject (currentPlayer);
			}

			usingDevicesManager.setUseMinDistanceToUseDevicesState (false);

			usingDevicesManager.clearDeviceList ();

			usingDevicesManager.addDeviceToList (objectToUse);

			usingDevicesManager.updateClosestDeviceList ();

			usingDevicesManager.useCurrentDevice (objectToUse);

			usingDevicesManager.setUseMinDistanceToUseDevicesState (useMinDistanceToUseDevices);

			if (currentInventoryManager != null) {
				currentInventoryManager.setExamineObjectBeforeStoreEnabledState (examineObjectBeforeStoreEnabled);
			}
		}
	}

	public static Vector2 getScreenResolution ()
	{
		#if UNITY_EDITOR
		return new Vector2 (Screen.width, Screen.height);
		#else
		return new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);
		#endif
	}

	public static void createInventoryWeaponAmmo (string weaponName, string ammoName, GameObject weaponAmmoMesh, 
	                                              Texture weaponAmmoIconTexture, string inventoryAmmoCategoryName, int ammoAmountPerPickup,
	                                              string customAmmoDescription)
	{
		instantiateMainManagerOnSceneWithType ("Main Inventory Manager", typeof(inventoryListManager));

		inventoryListManager mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

		if (mainInventoryListManager != null) {
			bool ammoPickupFound = false;

			GameObject ammoPickupGameObject = mainInventoryListManager.getInventoryPrefabByName (ammoName);

			ammoPickupFound = ammoPickupGameObject != null;

			if (ammoPickupFound) {
				print ("Ammo inventory object " + ammoName + " already exists");
			} else {
				inventoryInfo currentWeaponInventoryInfo = mainInventoryListManager.getInventoryInfoFromCategoryListByName (weaponName);
				if (currentWeaponInventoryInfo != null) {

					int ammoInventoryCategoryIndex = mainInventoryListManager.getInventoryCategoryIndexByName (inventoryAmmoCategoryName);

					if (ammoInventoryCategoryIndex > -1) {
						print ("Category " + inventoryAmmoCategoryName + " found in inventory list manager");

						inventoryInfo ammoInventoryInfo = new inventoryInfo ();

						ammoInventoryInfo.Name = ammoName;
						ammoInventoryInfo.inventoryGameObject = weaponAmmoMesh;
						ammoInventoryInfo.icon = weaponAmmoIconTexture;

						ammoInventoryInfo.objectInfo = customAmmoDescription;

						ammoInventoryInfo.amountPerUnit = ammoAmountPerPickup;
						ammoInventoryInfo.storeTotalAmountPerUnit = true;

						ammoInventoryInfo.canBeDropped = true;
						ammoInventoryInfo.canBeDiscarded = true;
						ammoInventoryInfo.canBeCombined = true;

						ammoInventoryInfo.canBeExamined = true;

						ammoInventoryInfo.useNewBehaviorOnCombine = true;
						ammoInventoryInfo.useOneUnitOnNewBehaviourCombine = true;
						ammoInventoryInfo.newBehaviorOnCombineMessage = "-OBJECT- refilled with -AMOUNT- projectiles";
						ammoInventoryInfo.objectToCombine = currentWeaponInventoryInfo.inventoryGameObject;

						ammoInventoryInfo.canBeSold = true;
						ammoInventoryInfo.sellPrice = 1000;
						ammoInventoryInfo.vendorPrice = 500;

						ammoInventoryInfo.weight = 5;
					
						mainInventoryListManager.addNewInventoryObject (ammoInventoryCategoryIndex, ammoInventoryInfo);
					
						int inventoryObjectIndex = mainInventoryListManager.getInventoryInfoIndexByName (ammoName);

						if (inventoryObjectIndex > -1) {
							print ("Inventory info for the new ammo created " + ammoName + " found");
							mainInventoryListManager.createInventoryPrafab (ammoInventoryCategoryIndex, inventoryObjectIndex);

							ammoPickupGameObject = mainInventoryListManager.getInventoryPrefabByName (ammoName);

							if (ammoPickupGameObject != null) {
								print ("New ammo inventory object found, assigning to the weapon to combine the ammo" + ammoPickupGameObject.name);

								currentWeaponInventoryInfo.canBeCombined = true;
								currentWeaponInventoryInfo.objectToCombine = ammoPickupGameObject;

								mainInventoryListManager.updateInventoryList ();
							} else {
								print ("New ammo inventory object not found to assign");
							}
						} else {
							print ("Inventory info for the new ammo created " + ammoName + " not found");
						}
					} else {
						print ("Category " + inventoryAmmoCategoryName + " not found in inventory list manager");
					}
				} else {
					print ("WARNING: Weapon inventory prefab " + weaponName + " not found, make sure that weapon is configured in the Inventory List Manager");
				}
			}
		}
	}

	public static void createInventoryWeapon (string weaponName, string inventoryWeaponCategoryName, GameObject weaponMesh, string weaponDescription, 
	                                          Texture weaponIconTexture, string relativePathWeaponsMesh, bool isMeleeWeapon)
	{
		#if UNITY_EDITOR

		instantiateMainManagerOnSceneWithType ("Main Inventory Manager", typeof(inventoryListManager));

		inventoryListManager mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

		if (mainInventoryListManager != null) {
			bool weaponPickupFound = false;

			GameObject weaponPickupGameObject = mainInventoryListManager.getInventoryPrefabByName (weaponName);

			weaponPickupFound = weaponPickupGameObject != null;

			if (weaponPickupFound) {
				print ("Weapon inventory object " + weaponName + " already exists");
			} else {
				int weaponInventoryCategoryIndex = mainInventoryListManager.getInventoryCategoryIndexByName (inventoryWeaponCategoryName);

				if (weaponInventoryCategoryIndex > -1) {
					print ("Category " + inventoryWeaponCategoryName + " found in inventory list manager");

					GameObject weaponMeshCopy = Instantiate (weaponMesh);

					if (weaponMeshCopy != null) {
						if (!isMeleeWeapon) {
							weaponPartstToRemoveOnPickupCreation currentweaponPartstToRemoveOnPickupCreation = weaponMeshCopy.GetComponent<weaponPartstToRemoveOnPickupCreation> ();

							if (currentweaponPartstToRemoveOnPickupCreation != null) {
								currentweaponPartstToRemoveOnPickupCreation.removeWeaponObjects ();
							}

							weaponAttachmentSystem currentWeaponAttachmentSystem = weaponMeshCopy.GetComponentInChildren<weaponAttachmentSystem> ();

							if (currentWeaponAttachmentSystem != null) {
								print ("Removing weapon attachment system from pickup");
								DestroyImmediate (currentWeaponAttachmentSystem.gameObject);
							}
						}
					}

					GameObject newWeaponMesh = createPrefab (relativePathWeaponsMesh, (weaponName + " Mesh"), weaponMeshCopy);

					BoxCollider currentBoxCollider = newWeaponMesh.GetComponent<BoxCollider> ();

					if (currentBoxCollider == null) {
						newWeaponMesh.AddComponent<BoxCollider> ();
					}

					int newLayerIndex = LayerMask.NameToLayer ("inventory");

					Component[] components = newWeaponMesh.GetComponentsInChildren (typeof(Transform));
					foreach (Transform child in components) {
						child.gameObject.layer = newLayerIndex;
					}

					print ("Created weapon mesh prefab " + newWeaponMesh.name);

					inventoryInfo weaponInventoryInfo = new inventoryInfo ();
					
					weaponInventoryInfo.Name = weaponName;

					weaponInventoryInfo.objectInfo = weaponDescription;

					weaponInventoryInfo.inventoryGameObject = newWeaponMesh;
					weaponInventoryInfo.icon = weaponIconTexture;

					weaponInventoryInfo.canBeEquiped = true;
					weaponInventoryInfo.canBeDropped = true;

					weaponInventoryInfo.canBeSold = true;
					weaponInventoryInfo.sellPrice = 1000;
					weaponInventoryInfo.vendorPrice = 500;

					weaponInventoryInfo.isWeapon = true;
					weaponInventoryInfo.isMeleeWeapon = isMeleeWeapon;

					weaponInventoryInfo.canBePlaceOnQuickAccessSlot = true;

					weaponInventoryInfo.weight = 5;

					mainInventoryListManager.addNewInventoryObject (weaponInventoryCategoryIndex, weaponInventoryInfo);

					int inventoryObjectIndex = mainInventoryListManager.getInventoryInfoIndexByName (weaponName);

					if (inventoryObjectIndex > -1) {
						print ("Inventory info for the new weapon created " + weaponName + " found");
						mainInventoryListManager.createInventoryPrafab (weaponInventoryCategoryIndex, inventoryObjectIndex);

					} else {
						print ("Inventory info for the new weapon created " + weaponName + " not found");
					}

					print ("New weapon " + weaponName + " added to the inventory");

					if (weaponMeshCopy != null) {
						DestroyImmediate (weaponMeshCopy);
					}
				} else {
					print ("Category " + inventoryWeaponCategoryName + " not found in inventory list manager");
				}
			}
		}
		#endif
	}

	public static void createInventoryArmorClothPiece (string objectName, string inventoryObjectCategoryName, string categoryName,
	                                                   GameObject objectMeshPrefab, string objectDescription, Texture objectIconTexture,
	                                                   string relativePathObjectMesh)
	{
		#if UNITY_EDITOR

		instantiateMainManagerOnSceneWithType ("Main Inventory Manager", typeof(inventoryListManager));

		inventoryListManager mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

		if (mainInventoryListManager != null) {
			bool objectPickupFound = false;

			GameObject objectPickupGameObject = mainInventoryListManager.getInventoryPrefabByName (objectName);

			objectPickupFound = objectPickupGameObject != null;

			if (objectPickupFound) {
				print ("Inventory object " + objectName + " already exists");
			} else {
				int objectInventoryCategoryIndex = mainInventoryListManager.getInventoryCategoryIndexByName (inventoryObjectCategoryName);

				if (objectInventoryCategoryIndex > -1) {
					print ("Category " + inventoryObjectCategoryName + " found in inventory list manager");

					GameObject objectMeshCopy = Instantiate (objectMeshPrefab);

					GameObject newObjectMesh = createPrefab (relativePathObjectMesh, (objectName + " Mesh"), objectMeshCopy);

					BoxCollider currentBoxCollider = newObjectMesh.GetComponent<BoxCollider> ();

					if (currentBoxCollider == null) {
						newObjectMesh.AddComponent<BoxCollider> ();
					}
						
					int newLayerIndex = LayerMask.NameToLayer ("inventory");

					Component[] components = objectMeshCopy.GetComponentsInChildren (typeof(Transform));
					foreach (Transform child in components) {
						child.gameObject.layer = newLayerIndex;
					}

					print ("Created Object mesh prefab " + newObjectMesh.name);

					inventoryInfo objectInventoryInfo = new inventoryInfo ();

					objectInventoryInfo.Name = objectName;

					objectInventoryInfo.objectInfo = objectDescription;

					objectInventoryInfo.inventoryGameObject = newObjectMesh;
					objectInventoryInfo.icon = objectIconTexture;

					objectInventoryInfo.canBeEquiped = true;
					objectInventoryInfo.canBeDropped = true;

					objectInventoryInfo.canBeSold = true;
					objectInventoryInfo.sellPrice = 1000;
					objectInventoryInfo.vendorPrice = 500;

					objectInventoryInfo.isArmorClothAccessory = true;

					objectInventoryInfo.canBePlaceOnQuickAccessSlot = true;

					objectInventoryInfo.weight = 5;

					mainInventoryListManager.addNewInventoryObject (objectInventoryCategoryIndex, objectInventoryInfo);

					int inventoryObjectIndex = mainInventoryListManager.getInventoryInfoIndexByName (objectName);

					if (inventoryObjectIndex > -1) {
						print ("Inventory info for the new object created " + objectName + " found");
						mainInventoryListManager.createInventoryPrafab (objectInventoryCategoryIndex, inventoryObjectIndex);


						GameObject newObjectPrefab = mainInventoryListManager.getInventoryPrefabByName (objectName);

						if (newObjectPrefab != null) {
							armorClothPickup currentArmorClothPickup = newObjectPrefab.GetComponentInChildren<armorClothPickup> ();

							if (currentArmorClothPickup != null) {
								currentArmorClothPickup.categoryName = categoryName;
							}
						}

					} else {
						print ("Inventory info for the new object created " + objectName + " not found");
					}

					print ("New Armor Cloth Piece " + objectName + " added to the inventory");

					if (objectMeshCopy != null) {
						DestroyImmediate (objectMeshCopy);
					}
				} else {
					print ("Category " + inventoryObjectCategoryName + " not found in inventory list manager");
				}
			}
		}
		#endif
	}

	public static void createInventoryObject (string objectName, string inventoryCategoryName, GameObject objectMesh, string objectDescription, 
	                                          Texture iconTexture, string relativePathMesh, bool canBeEquipped, bool canBeDropped, bool canBeDiscarded)
	{
		#if UNITY_EDITOR

		instantiateMainManagerOnSceneWithType ("Main Inventory Manager", typeof(inventoryListManager));

		inventoryListManager mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

		if (mainInventoryListManager != null) {
			bool pickupFound = false;

			GameObject pickupGameObject = mainInventoryListManager.getInventoryPrefabByName (objectName);

			pickupFound = pickupGameObject != null;

			if (pickupFound) {
				print ("Inventory object " + objectName + " already exists");
			} else {
				int inventoryCategoryIndex = mainInventoryListManager.getInventoryCategoryIndexByName (inventoryCategoryName);

				if (inventoryCategoryIndex > -1) {
					print ("Category " + inventoryCategoryName + " found in inventory list manager");

					GameObject objectMeshCopy = Instantiate (objectMesh);

					GameObject newObjectMesh = createPrefab (relativePathMesh, (objectName + " Mesh"), objectMeshCopy);

					newObjectMesh.AddComponent<BoxCollider> ();

					int newLayerIndex = LayerMask.NameToLayer ("inventory");

					Component[] components = newObjectMesh.GetComponentsInChildren (typeof(Transform));
					foreach (Transform child in components) {
						child.gameObject.layer = newLayerIndex;
					}

					print ("Created object mesh prefab " + newObjectMesh.name);

					inventoryInfo inventoryInfo = new inventoryInfo ();

					inventoryInfo.Name = objectName;
					inventoryInfo.objectInfo = objectDescription;

					inventoryInfo.inventoryGameObject = newObjectMesh;
					inventoryInfo.icon = iconTexture;

					inventoryInfo.canBeEquiped = canBeEquipped;
					inventoryInfo.canBeDropped = canBeDropped;
					inventoryInfo.canBeDiscarded = canBeDiscarded;

					inventoryInfo.canBeSold = true;
					inventoryInfo.sellPrice = 1000;
					inventoryInfo.vendorPrice = 500;

					inventoryInfo.weight = 5;

					mainInventoryListManager.addNewInventoryObject (inventoryCategoryIndex, inventoryInfo);

					int inventoryObjectIndex = mainInventoryListManager.getInventoryInfoIndexByName (objectName);

					if (inventoryObjectIndex > -1) {
						print ("Inventory info for the new object created " + objectName + " found");
						mainInventoryListManager.createInventoryPrafab (inventoryCategoryIndex, inventoryObjectIndex);

					} else {
						print ("Inventory info for the new object created " + objectName + " not found");
					}

					print ("New Object " + objectName + " added to the inventory");

					if (objectMeshCopy != null) {
						DestroyImmediate (objectMeshCopy);
					}
				} else {
					print ("Category " + inventoryCategoryName + " not found in inventory list manager");
				}
			}
		}
		#endif
	}

	public static bool checkIfInventoryObjectNameExits (string objectName)
	{
		instantiateMainManagerOnSceneWithType ("Main Inventory Manager", typeof(inventoryListManager));

		inventoryListManager mainInventoryListManager = FindObjectOfType<inventoryListManager> ();

		if (mainInventoryListManager != null) {
			return mainInventoryListManager.existInventoryInfoFromName (objectName);
		}

		return false;
	}

	public static GameObject createPrefab (string prefabPath, string prefabName, GameObject prefabToCreate)
	{
		#if UNITY_EDITOR
		GameObject newPrefabGameObject = Instantiate (prefabToCreate);

		string relativePath = prefabPath;

		if (!Directory.Exists (relativePath)) {
			print ("Prefab folder " + relativePath + " doesn't exist, created a new one with that name");

			Directory.CreateDirectory (relativePath);
		}

		string prefabFilePath = relativePath + "/" + prefabName + ".prefab";

		bool prefabExists = false;
		if ((GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject)) != null) {
			prefabExists = true;
		}

		if (prefabExists) {
			UnityEngine.Object prefab = (GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject));
			PrefabUtility.ReplacePrefab (newPrefabGameObject, prefab, ReplacePrefabOptions.ReplaceNameBased);

			print ("Prefab already existed. Replacing prefab in path " + prefabFilePath);
		} else {
			UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab (prefabFilePath);
			PrefabUtility.ReplacePrefab (newPrefabGameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);

			print ("Prefab to create is new. Creating new prefab in path " + prefabFilePath);
		}

		DestroyImmediate (newPrefabGameObject);

		return (GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject));
		#else
		return null;
		#endif
	}

	public static GameObject instantiatePrefabInScene (string prefabPath, string prefabName, LayerMask layerToPlaceObjects)
	{
		#if UNITY_EDITOR
		string relativePath = prefabPath;

		if (Directory.Exists (relativePath)) {
			
			string prefabFilePath = relativePath + "/" + prefabName + ".prefab";

			bool prefabExists = false;

			if ((GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject)) != null) {
				prefabExists = true;
			}

			if (prefabExists) {
				GameObject prefabToInstantiate = (GameObject)AssetDatabase.LoadAssetAtPath (prefabFilePath, typeof(GameObject));

				if (prefabToInstantiate) {
					Vector3 positionToInstantiate = Vector3.zero;

					if (SceneView.lastActiveSceneView) {
						if (SceneView.lastActiveSceneView.camera) {
							Camera currentCameraEditor = SceneView.lastActiveSceneView.camera;
							Vector3 editorCameraPosition = currentCameraEditor.transform.position;
							Vector3 editorCameraForward = currentCameraEditor.transform.forward;
							RaycastHit hit;
							if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceObjects)) {
								positionToInstantiate = hit.point + Vector3.up * 0.2f;
							}
						}
					}

					GameObject newCreatedObject = (GameObject)Instantiate (prefabToInstantiate, positionToInstantiate, Quaternion.identity);
					newCreatedObject.name = prefabToInstantiate.name;

					print (prefabName + " prefab added to the scene");

					return newCreatedObject;
				} else {
					print ("Prefab in path " + relativePath + " not found");
				}
			} else {
				print ("Prefab in path " + relativePath + " not found");
			}
		}

		return null;
		#else
		return null;
		#endif
	}

	public static void createSettingsListTemplate (string characterTemplateDataPath, string characterTemplateName, int characterTemplateID, List<buildPlayer.settingsInfoCategory> settingsInfoCategoryList)
	{
		#if UNITY_EDITOR

		if (!Directory.Exists (characterTemplateDataPath)) {
			print ("Character Template Data folder " + characterTemplateDataPath + " doesn't exist, created a new one with that name");

			Directory.CreateDirectory (characterTemplateDataPath);
		}

		var obj = ScriptableObject.CreateInstance<characterSettingsTemplate> ();

		obj.characterTemplateID = characterTemplateID;

		List<buildPlayer.settingsInfoCategory> newSettingsInfoCategoryList = new List<buildPlayer.settingsInfoCategory> ();

		for (int i = 0; i < settingsInfoCategoryList.Count; i++) { 

			buildPlayer.settingsInfoCategory currentSettingsInfoCategory = settingsInfoCategoryList [i];

			buildPlayer.settingsInfoCategory newSettingsInfoCategory = new buildPlayer.settingsInfoCategory ();

			newSettingsInfoCategory.Name = currentSettingsInfoCategory.Name;

			for (int j = 0; j < currentSettingsInfoCategory.settingsInfoList.Count; j++) { 

				buildPlayer.settingsInfo currentSettingsInfo = currentSettingsInfoCategory.settingsInfoList [j];

				buildPlayer.settingsInfo newSettingsInfo = new buildPlayer.settingsInfo ();

				newSettingsInfo.Name = currentSettingsInfo.Name;

				newSettingsInfo.useBoolState = currentSettingsInfo.useBoolState;
				newSettingsInfo.boolState = currentSettingsInfo.boolState;

				newSettingsInfo.useFloatValue = currentSettingsInfo.useFloatValue;
				newSettingsInfo.floatValue = currentSettingsInfo.floatValue;

				newSettingsInfo.useStringValue = currentSettingsInfo.useStringValue;
				newSettingsInfo.stringValue = currentSettingsInfo.stringValue;

				newSettingsInfo.useRegularValue = currentSettingsInfo.useRegularValue;
				newSettingsInfo.regularValue = currentSettingsInfo.regularValue;

				newSettingsInfoCategory.settingsInfoList.Add (newSettingsInfo);
			}

			newSettingsInfoCategoryList.Add (newSettingsInfoCategory);
		}

		obj.settingsInfoCategoryList = newSettingsInfoCategoryList;

		string newPath = characterTemplateDataPath + "/" + characterTemplateName + ".asset";

		UnityEditor.AssetDatabase.CreateAsset (obj, newPath);
		UnityEditor.AssetDatabase.SaveAssets ();

		refreshAssetDatabase ();

		#endif
	}

	public static GameObject findMainPlayerOnScene ()
	{
		playerCharactersManager mainPlayerCharactersManager = FindObjectOfType<playerCharactersManager> ();

		if (mainPlayerCharactersManager != null) {
			return mainPlayerCharactersManager.getMainPlayerGameObject ();
		}

		return null;
	}

	public static Transform findMainPlayerTransformOnScene ()
	{
		playerCharactersManager mainPlayerCharactersManager = FindObjectOfType<playerCharactersManager> ();

		if (mainPlayerCharactersManager != null) {
			return mainPlayerCharactersManager.getMainPlayerTransform ();
		}

		return null;
	}

	public static playerCamera findMainPlayerCameraOnScene ()
	{
		playerCharactersManager mainPlayerCharactersManager = FindObjectOfType<playerCharactersManager> ();

		if (mainPlayerCharactersManager != null) {
			return mainPlayerCharactersManager.getMainPlayerCamera ();
		}

		return null;
	}

	public static Camera findMainPlayerCameraComponentOnScene ()
	{
		playerCharactersManager mainPlayerCharactersManager = FindObjectOfType<playerCharactersManager> ();

		if (mainPlayerCharactersManager != null) {
			return mainPlayerCharactersManager.getMainPlayerCameraComponent ();
		}

		return null;
	}

	public static Transform findMainPlayerCameraTransformOnScene ()
	{
		playerCharactersManager mainPlayerCharactersManager = FindObjectOfType<playerCharactersManager> ();

		if (mainPlayerCharactersManager != null) {
			return mainPlayerCharactersManager.getMainPlayerCameraTransform ();
		}

		return null;
	}

	public static void updateComponent (UnityEngine.Object componentToUpdate)
	{
		#if UNITY_EDITOR
		if (componentToUpdate != null) {
			EditorUtility.SetDirty (componentToUpdate);
		}
		#endif
	}

	public static void updateComponent (MonoBehaviour componentToUpdate)
	{
		#if UNITY_EDITOR
		if (componentToUpdate != null) {
			EditorUtility.SetDirty (componentToUpdate);
		}
		#endif
	}

	public static void setActiveGameObjectInEditor (GameObject objectToSelect)
	{
		#if UNITY_EDITOR

		if (objectToSelect == null) {
			return;
		}

		Selection.activeGameObject = objectToSelect;
		#endif
	}

	public static GameObject getActiveGameObjectInEditor ()
	{
		#if UNITY_EDITOR
		return Selection.activeGameObject;
		#else
		return null;
		#endif
	}

	public static bool isCurrentSelectionActiveGameObject (GameObject objectToSelect)
	{
		#if UNITY_EDITOR
		return Selection.activeGameObject != objectToSelect;
		#else
		return false;
		#endif
	}

	public static Camera getCameraEditor ()
	{
		#if UNITY_EDITOR
		if (SceneView.lastActiveSceneView) {
			if (SceneView.lastActiveSceneView.camera) {
				return SceneView.lastActiveSceneView.camera;
			}
		}
		#endif

		return null;
	}

	public static GameObject getLoadAssetAtPath (string objectPath)
	{
		#if UNITY_EDITOR
		GameObject newObject = (GameObject)AssetDatabase.LoadAssetAtPath (objectPath, typeof(GameObject));

		return newObject;
		#else

		return null;
		#endif
	}

	public static void alignViewToObject (Transform transformToUse)
	{
		#if UNITY_EDITOR
		SceneView sceneView = SceneView.lastActiveSceneView;

		if (sceneView) {
			sceneView.AlignViewToObject (transformToUse);
		}
		#endif
	}

	public static bool isApplicationPlaying ()
	{
		#if UNITY_EDITOR
		return Application.isPlaying;
		#else

		return true;
		#endif
	}

	public static void pauseOrResumeAIOnScene (bool state, int pauseCharacterPriority)
	{
		playerController[] playerControllerList = FindObjectsOfType<playerController> ();

		foreach (playerController currentPlayerController in playerControllerList) {
			if (currentPlayerController.usedByAI) {
				pauseOrResumeCharacter (state, currentPlayerController, pauseCharacterPriority);
			}
		}
	}

	public static void pauseOrResumeEnemyAIOnScene (bool state, string playerFactionName, int pauseCharacterPriority)
	{
		playerController[] playerControllerList = FindObjectsOfType<playerController> ();

		foreach (playerController currentPlayerController in playerControllerList) {
			if (currentPlayerController.usedByAI) {
				playerComponentsManager currentplayerComponentsManager = currentPlayerController.gameObject.GetComponent<playerComponentsManager> ();

				if (currentplayerComponentsManager != null) {
					characterFactionManager currentCharacterFactionManager = currentplayerComponentsManager.getCharacterFactionManager ();

					if (currentCharacterFactionManager != null) {
						if (currentCharacterFactionManager.isCharacterEnemy (playerFactionName)) {
							pauseOrResumeCharacter (state, currentPlayerController, pauseCharacterPriority);
						}
					}
				}
			}
		}
	}

	public static void pauseOrResumeCharacter (bool state, playerController currentPlayerController, int pauseCharacterPriority)
	{
		if (currentPlayerController.getPauseCharacterPriorityValue () > pauseCharacterPriority) {
			return;
		}

		if (state) {
			currentPlayerController.setPauseCharacterPriorityValue (pauseCharacterPriority);
		} else {
			currentPlayerController.setPauseCharacterPriorityValue (0);
		}

		currentPlayerController.setOverrideAnimationSpeedActiveState (state);

		if (state) {
			currentPlayerController.setReducedVelocity (0);
		} else {
			currentPlayerController.setNormalVelocity ();
		}

		currentPlayerController.setCanMoveAIState (!state);

		currentPlayerController.setAddExtraRotationPausedState (state);

		currentPlayerController.setIgnoreCameraDirectionOnStrafeMovementState (state);

		playerComponentsManager currentplayerComponentsManager = currentPlayerController.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {

			AINavMesh currentAINavMesh = currentPlayerController.GetComponent<AINavMesh> ();

			if (currentAINavMesh != null) {
				currentAINavMesh.pauseAI (state);

				if (!state) {
					findObjectivesSystem currentFindObjectivesSystem = currentPlayerController.GetComponent<findObjectivesSystem> ();

					if (currentFindObjectivesSystem != null) {
						currentFindObjectivesSystem.forceFovTriggerToDetectAnythingAround ();
					}
				}
			}
		}
	}

	public static void pauseOrResumeAllCharactersScene (bool state)
	{
		playerController[] playerControllerList = FindObjectsOfType<playerController> ();

		foreach (playerController currentPlayerController in playerControllerList) {
			currentPlayerController.setOverrideAnimationSpeedActiveState (state);

			if (state) {
				currentPlayerController.setReducedVelocity (0);
			} else {
				currentPlayerController.setNormalVelocity ();
			}

			currentPlayerController.setCanMoveAIState (!state);

			currentPlayerController.setIgnoreCameraDirectionOnStrafeMovementState (state);
		}
	}

	public static Vector3 ClampMagnitude (Vector3 v, float max, float min)
	{
		double sm = v.sqrMagnitude;

		if (sm > (double)max * (double)max) {
			return v.normalized * max;
		} else if (sm < (double)min * (double)min) {
			return v.normalized * min;
		}

		return v;
	}

	public static void setGravityValueOnObjectFromPlayerValues (artificialObjectGravity newArtificialObjectGravity, GameObject currentPlayer, float gravityForceForCircumnavigationOnProjectile)
	{
		gravitySystem currentGravitySystem = currentPlayer.GetComponent<gravitySystem> ();

		if (currentGravitySystem != null) {
			if (currentGravitySystem.isCurcumnavigating ()) {
				Transform currentSurfaceBelowPlayer = currentGravitySystem.getCurrentSurfaceBelowPlayer ();

				if (currentSurfaceBelowPlayer != null) {
					newArtificialObjectGravity.setUseCenterPointActiveState (true, currentSurfaceBelowPlayer);

					newArtificialObjectGravity.setGravityForceValue (false, gravityForceForCircumnavigationOnProjectile);
				}
			}
		}
	}

	public static weaponObjectInfo getMeleeWeaponObjectInfo (string weaponName, meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager)
	{
		return mainMeleeWeaponsGrabbedManager.getWeaponGrabbedByName (weaponName);
	}

	public static float Abs (float f)
	{
		return Math.Abs (f);
	}

	public static void addAllMainManagersToScene ()
	{
		mainManagerAdministrator currentMainManagerAdministrator = FindObjectOfType<mainManagerAdministrator> ();

		if (currentMainManagerAdministrator != null) {
			currentMainManagerAdministrator.addAllMainManagersToScene ();
		} else {
			print ("No Main Manager Administrator located, make sure to drop the player prefab on the scene or create a new player character");
		}
	}

	//	public static void instantiateMainManagerOnScene (string mainManagerName)
	//	{
	//		mainManagerAdministrator currentMainManagerAdministrator = FindObjectOfType<mainManagerAdministrator> ();
	//
	//		if (currentMainManagerAdministrator != null) {
	//			currentMainManagerAdministrator.addMainManagerToScene (mainManagerName);
	//		}
	//	}

	public static void instantiateMainManagerOnSceneWithType (string mainManagerName, Type typeToSearch)
	{
		mainManagerAdministrator currentMainManagerAdministrator = FindObjectOfType<mainManagerAdministrator> ();

		if (currentMainManagerAdministrator != null) {
			currentMainManagerAdministrator.addMainManagerToSceneWithType (mainManagerName, typeToSearch);
		}
	}

	public static void activateTimeBulletXSeconds (float timeBulletDuration, float timeScale)
	{
		timeBullet timeBulletManager = FindObjectOfType<timeBullet> ();
	
		if (timeBulletManager != null) {
			if (timeBulletDuration > 0) {
				timeBulletManager.activateTimeBulletXSeconds (timeBulletDuration, timeScale);
			} else {
				if (timeScale == 1) {
					timeBulletManager.setBulletTimeState (false, timeScale);
				} else {
					timeBulletManager.setBulletTimeState (true, timeScale);
				}
			}
		}
	}

	public static void loadScene (int newSceneIndex, bool useLoadScreen, int loadScreenScene, string sceneToLoadAsyncPrefsName, 
	                              bool useLastSceneIndexAsLoadScreen, bool checkLoadingScreenSceneConfigured, string loadingScreenSceneName)
	{
		int sceneLoadIndex = newSceneIndex;

		if (useLoadScreen) {
			bool loadingScreenLocatedResult = true;

			if (useLastSceneIndexAsLoadScreen) {
				int lastSceneIndex = SceneManager.sceneCountInBuildSettings;

				sceneLoadIndex = lastSceneIndex - 1;

				if (sceneLoadIndex < 0) {
					sceneLoadIndex = -1;
				} 
			} else {
				sceneLoadIndex = loadScreenScene;
			}

			if (checkLoadingScreenSceneConfigured) {
				string sceneNameToCheck = GKC_Utils.getSceneNameFromIndex (sceneLoadIndex);

				if (sceneNameToCheck != null && sceneNameToCheck != "") {
					if (!loadingScreenSceneName.Equals (sceneNameToCheck)) {
						sceneLoadIndex = newSceneIndex;
				
						loadingScreenLocatedResult = false;
					}
				}
			}

			if (loadingScreenLocatedResult) {
				PlayerPrefs.SetInt (sceneToLoadAsyncPrefsName, newSceneIndex);
			}
		}

		if (sceneLoadIndex >= 0) {
			SceneManager.LoadScene (sceneLoadIndex);
		}
	}

	public static string getSceneNameFromIndex (int sceneIndex)
	{
		string currentSceneName = "";

		string path = SceneUtility.GetScenePathByBuildIndex (sceneIndex);

		if (path != null && path != "") {
			int slash = path.LastIndexOf ('/');

			string name = path.Substring (slash + 1);

			int dot = name.LastIndexOf ('.');

			currentSceneName = name.Substring (0, dot);
		}

		return currentSceneName; 
	}

	public static void updateDirtyScene (string recordObjectName, GameObject gameObjectToRecord)
	{
		#if UNITY_EDITOR
		if (!Application.isPlaying) {
			Undo.RecordObject (gameObjectToRecord, recordObjectName);

			EditorSceneManager.MarkSceneDirty (menuPause.getCurrentActiveScene ());
		}
		#endif
	}

	public static void updateDirtyScene ()
	{
		#if UNITY_EDITOR
		if (!Application.isPlaying) {
			EditorSceneManager.MarkSceneDirty (menuPause.getCurrentActiveScene ());
		}
		#endif
	}

	public static void removeEnemiesFromNewFriendFaction (Transform characterTransform)
	{
		playerComponentsManager currentplayerComponentsManager = characterTransform.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			playerController currentPlayerController = currentplayerComponentsManager.getPlayerController ();

			findObjectivesSystem currentFindObjectivesSystem = characterTransform.GetComponent<findObjectivesSystem> ();

			currentFindObjectivesSystem.clearFullEnemiesList ();

			currentFindObjectivesSystem.removeCharacterAsTargetOnSameFaction ();

			currentFindObjectivesSystem.resetAITargets ();

			currentPlayerController.setMainColliderState (false);

			currentPlayerController.setMainColliderState (true);
		}
	}

	public static void eventOnPressingKeyboardInput (int controllerNumber)
	{
		playerCharactersManager.checkPanelsActiveOnGamepadOrKeyboard (true, controllerNumber);
	}

	public static void eventOnPressingGamepadInput (int controllerNumber)
	{
		playerCharactersManager.checkPanelsActiveOnGamepadOrKeyboard (false, controllerNumber);
	}

	public static void enableOrDisableAbilityGroupByName (Transform characterTransform, bool state, List<string> abilityNameList)
	{
		playerComponentsManager currentplayerComponentsManager = characterTransform.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			playerAbilitiesSystem currentPlayerAbilitiesSystem = currentplayerComponentsManager.getPlayerAbilitiesSystem ();

			if (currentPlayerAbilitiesSystem != null) {
				currentPlayerAbilitiesSystem.enableOrDisableAbilityGroupByName (abilityNameList, state);
			}
		}
	}

	public static void increaseStatsByList (Transform characterTransform, bool state, List<objectExperienceSystem.statInfo> statInfoList)
	{
		playerComponentsManager currentPlayerComponentsManager = characterTransform.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			playerStatsSystem currentPlayerStatsSystem = currentPlayerComponentsManager.getPlayerStatsSystem ();

			if (currentPlayerStatsSystem != null) {

				for (int k = 0; k < statInfoList.Count; k++) {

					if (statInfoList [k].statIsAmount) {
						float extraValue = statInfoList [k].statExtraValue;
						if (statInfoList [k].useRandomRange) {
							extraValue = UnityEngine.Random.Range (statInfoList [k].randomRange.x, statInfoList [k].randomRange.y);

							extraValue = Mathf.RoundToInt (extraValue);
						}

						currentPlayerStatsSystem.increasePlayerStat (statInfoList [k].Name, extraValue);
					} else {
						currentPlayerStatsSystem.enableOrDisableBoolPlayerStat (statInfoList [k].Name, statInfoList [k].newBoolState);
					}
				}
			}
		}
	}

	public static void activateAbilityByName (Transform characterTransform, string abilityName, bool abilityIsTemporallyActivated)
	{
		playerComponentsManager currentplayerComponentsManager = characterTransform.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			playerAbilitiesSystem currentPlayerAbilitiesSystem = currentplayerComponentsManager.getPlayerAbilitiesSystem ();

			if (currentPlayerAbilitiesSystem != null) {
				currentPlayerAbilitiesSystem.inputSelectAndPressDownNewAbilityTemporally (abilityName, abilityIsTemporallyActivated);
			}
		}
	}

	public static bool checkIfAbilitiesOnUseOrCooldown (Transform characterTransform, string abilityName)
	{
		playerComponentsManager currentplayerComponentsManager = characterTransform.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			playerAbilitiesSystem currentPlayerAbilitiesSystem = currentplayerComponentsManager.getPlayerAbilitiesSystem ();

			if (currentPlayerAbilitiesSystem != null) {
				return currentPlayerAbilitiesSystem.checkIfAbilitiesOnUseOrCooldown (abilityName);
			}
		}

		return false;
	}

	public static GameObject createSliceRagdollPrefab (GameObject characterMeshPrefab, string newPrefabsPath, Material newSliceMaterial, 
	                                                   bool setTagOnSkeletonRigidbodiesValue, string tagOnSkeletonRigidbodiesValue)
	{
		GameObject newCharacterMeshForRagdollPrefab = (GameObject)Instantiate (characterMeshPrefab, Vector3.zero, Quaternion.identity);

		string prefabName = characterMeshPrefab.name + " Ragdoll (With Slice System)";
		newCharacterMeshForRagdollPrefab.name = prefabName;

		surfaceToSlice currentSurfaceToSlice = newCharacterMeshForRagdollPrefab.GetComponent<surfaceToSlice> ();

		if (currentSurfaceToSlice == null) {
			genericRagdollBuilder currentGenericRagdollBuilder = newCharacterMeshForRagdollPrefab.GetComponent<genericRagdollBuilder> ();

			if (currentGenericRagdollBuilder == null) {
				ragdollBuilder currentRagdollBuilder = newCharacterMeshForRagdollPrefab.AddComponent<ragdollBuilder> ();

				Animator mainAnimator = newCharacterMeshForRagdollPrefab.GetComponent<Animator> ();
				currentRagdollBuilder.getAnimator (mainAnimator);
				currentRagdollBuilder.createRagdoll ();

				DestroyImmediate (currentRagdollBuilder);
			}

			currentSurfaceToSlice = newCharacterMeshForRagdollPrefab.AddComponent<surfaceToSlice> ();

			GameObject characterMesh = newCharacterMeshForRagdollPrefab;

			simpleSliceSystem currentSimpleSliceSystem = characterMesh.GetComponent<simpleSliceSystem> ();

			if (currentSimpleSliceSystem == null) {
				currentSimpleSliceSystem = characterMesh.AddComponent<simpleSliceSystem> ();
			}

			currentSimpleSliceSystem.searchBodyParts ();

			for (int i = 0; i < currentSimpleSliceSystem.severables.Length; i++) {
				//enable or disalbe colliders in the ragdoll
				if (currentSimpleSliceSystem.severables [i] != null) {
					Collider currentCollider = currentSimpleSliceSystem.severables [i].GetComponent<Collider> ();

					if (currentCollider != null) {
						currentCollider.enabled = true;
					}

					Rigidbody currentRigidbody = currentSimpleSliceSystem.severables [i].GetComponent<Rigidbody> ();

					if (currentRigidbody != null) {
						currentRigidbody.isKinematic = false;
					}
				}		
			}

			currentSimpleSliceSystem.mainSurfaceToSlice = currentSurfaceToSlice;
			currentSimpleSliceSystem.objectToSlice = characterMesh;
			currentSimpleSliceSystem.alternatePrefab = newCharacterMeshForRagdollPrefab;

			currentSurfaceToSlice.setMainSimpleSliceSystem (currentSimpleSliceSystem.gameObject);
			currentSurfaceToSlice.objectIsCharacter = true;

			currentSimpleSliceSystem.objectToSlice = characterMesh;

			currentSimpleSliceSystem.infillMaterial = newSliceMaterial;

			if (setTagOnSkeletonRigidbodiesValue) {
				currentSimpleSliceSystem.setTagOnBodyParts (tagOnSkeletonRigidbodiesValue);
			}

			GKC_Utils.updateComponent (currentSimpleSliceSystem);

			GKC_Utils.updateDirtyScene ("Set slice system info", currentSimpleSliceSystem.gameObject);

			Debug.Log ("Ragdoll prefab created ");
		} else {
			Debug.Log ("Ragdoll was already configured for this prefab");
		}

		GameObject newRagdollPrefab = GKC_Utils.createPrefab (newPrefabsPath, prefabName, newCharacterMeshForRagdollPrefab);

		GKC_Utils.updateDirtyScene ("Create Slice Ragdoll", newCharacterMeshForRagdollPrefab);

		DestroyImmediate (newCharacterMeshForRagdollPrefab);

		return newRagdollPrefab;
	}


	public static Transform getMountPointTransformByName (string mountPointName, Transform characterTransform)
	{
		playerComponentsManager currentplayerComponentsManager = characterTransform.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			bodyMountPointsSystem currentBodyMountPointsSystem = currentplayerComponentsManager.getBodyMountPointsSystem ();

			if (currentBodyMountPointsSystem != null) {
				return currentBodyMountPointsSystem.getMountPointTransformByName (mountPointName);
			}
		}

		return null;
	}

	public static void activateBrainWashOnCharacter (GameObject currentCharacter, string factionToConfigure, string newTag, bool setNewName, string newName, 
	                                                 bool AIIsFriend, bool followPartnerOnTriggerEnabled, bool setPlayerAsPartner, GameObject newPartner, 
	                                                 bool useRemoteEvents, List<string> remoteEventNameList)
	{
		if (currentCharacter == null) {
			return;
		}
			
		playerComponentsManager currentplayerComponentsManager = currentCharacter.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager == null) {
			return;
		}

		characterFactionManager currentCharacterFactionManager = currentplayerComponentsManager.getCharacterFactionManager ();

		if (currentCharacterFactionManager != null) {
			currentCharacterFactionManager.removeCharacterDeadFromFaction ();

			currentCharacterFactionManager.changeCharacterToFaction (factionToConfigure);

			currentCharacterFactionManager.addCharacterFromFaction ();

			currentCharacter.tag = newTag;


			playerController currentPlayerController = currentplayerComponentsManager.getPlayerController ();

			health currentHealth = currentplayerComponentsManager.getHealth ();

			if (setNewName) {
				if (AIIsFriend) {
					if (newName != "") {
						currentHealth.setAllyNewNameIngame (newName);
					}

					currentHealth.updateNameWithAlly ();
				} else {
					if (newName != "") {
						currentHealth.setEnemyNewNameIngame (newName);
					}

					currentHealth.updateNameWithEnemy ();
				}
			}

			AINavMesh currentAINavMesh = currentCharacter.GetComponent<AINavMesh> ();

			if (currentAINavMesh != null) {
				currentAINavMesh.pauseAI (true);

				currentAINavMesh.pauseAI (false);
			}


			findObjectivesSystem currentFindObjectivesSystem = currentCharacter.GetComponent<findObjectivesSystem> ();

			currentFindObjectivesSystem.clearFullEnemiesList ();

			currentFindObjectivesSystem.removeCharacterAsTargetOnSameFaction ();

			currentFindObjectivesSystem.resetAITargets ();

			currentFindObjectivesSystem.setFollowPartnerOnTriggerState (followPartnerOnTriggerEnabled);

			if (setPlayerAsPartner) {
				currentFindObjectivesSystem.addPlayerAsPartner (newPartner);
			}

			if (useRemoteEvents) {
				remoteEventSystem currentRemoteEventSystem = currentCharacter.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameList.Count; i++) {

						currentRemoteEventSystem.callRemoteEvent (remoteEventNameList [i]);
					}
				}
			}

			currentPlayerController.setMainColliderState (false);

			currentPlayerController.setMainColliderState (true);
		}
	}

	public static void activateRemoteEvent (string remoteEventName, GameObject objectToCheck)
	{
		remoteEventSystem currentRemoteEventSystem = objectToCheck.GetComponent<remoteEventSystem> ();

		if (currentRemoteEventSystem != null) {
			currentRemoteEventSystem.callRemoteEvent (remoteEventName);
		}
	}

	public static void activateRemoteEvents (List<string>  remoteEventNameList, GameObject objectToCheck)
	{
		remoteEventSystem currentRemoteEventSystem = objectToCheck.GetComponent<remoteEventSystem> ();

		if (currentRemoteEventSystem != null) {
			for (int i = 0; i < remoteEventNameList.Count; i++) {

				currentRemoteEventSystem.callRemoteEvent (remoteEventNameList [i]);
			}
		}
	}

	public static float getCharacterRadius (Transform characterToCheck)
	{
		if (characterToCheck != null) {
			playerController currentPlayerController = characterToCheck.GetComponent<playerController> ();

			if (currentPlayerController != null) {
				return currentPlayerController.getCharacterRadius ();
			}
		}

		return 0;
	}

	public static float getAngle (Vector3 v1, Vector3 v2)
	{
		return Mathf.Rad2Deg * Mathf.Asin (Vector3.Cross (v1.normalized, v2.normalized).magnitude);
	}

	public static string getCurrentLanguage ()
	{
		gameLanguageSelector currentGameLanguageSelector = FindObjectOfType<gameLanguageSelector> ();

		if (currentGameLanguageSelector != null) {
			return currentGameLanguageSelector.getCurrentLanguage ();
		}

		return "English";
	}

	public static void setCurrentLanguage (string newLanguage)
	{
		gameLanguageSelector currentGameLanguageSelector = FindObjectOfType<gameLanguageSelector> ();

		if (currentGameLanguageSelector != null) {
			currentGameLanguageSelector.setCurrentLanguage (newLanguage);
		}
	}

	public static bool isUpdateElementsOnLanguageChangeActive ()
	{
		gameLanguageSelector currentGameLanguageSelector = FindObjectOfType<gameLanguageSelector> ();

		if (currentGameLanguageSelector != null) {
			return currentGameLanguageSelector.updateElementsOnLanguageChange;
		}

		return false;
	}

	public static void removeNullGameObjectsFromList (List<GameObject> listToCheck)
	{
		for (int i = listToCheck.Count - 1; i >= 0; i--) {
			if (listToCheck [i] == null) {
				listToCheck.RemoveAt (i);
			}
		}
	}

	public static List<string> getCurrentLanguageList ()
	{
		gameLanguageSelector currentGameLanguageSelector = FindObjectOfType<gameLanguageSelector> ();

		if (currentGameLanguageSelector != null) {
			return currentGameLanguageSelector.getCurrentLanguageList ();
		}

		return null;
	}

	public static void addLanguage (string newName)
	{
		gameLanguageSelector currentGameLanguageSelector = FindObjectOfType<gameLanguageSelector> ();

		if (currentGameLanguageSelector != null) {
			currentGameLanguageSelector.addLanguage (newName);
		}
	}

	public static void removeLanguage (string newName)
	{
		gameLanguageSelector currentGameLanguageSelector = FindObjectOfType<gameLanguageSelector> ();

		if (currentGameLanguageSelector != null) {
			currentGameLanguageSelector.removeLanguage (newName);
		}
	}

	public static void refreshAssetDatabase ()
	{
		#if UNITY_EDITOR

		UnityEditor.AssetDatabase.Refresh ();

		print ("Refreshing Asset Database");

		#endif
	}

	public static List<T> FindObjectsOfTypeAll<T> ()
	{
		List<T> results = new List<T> ();
		var s = SceneManager.GetActiveScene ();

		if (s.isLoaded) {
			var allGameObjects = s.GetRootGameObjects ();

			for (int j = 0; j < allGameObjects.Length; j++) {
				var go = allGameObjects [j];
				results.AddRange (go.GetComponentsInChildren<T> (true));
			}
		}

		return results;
	}

	public static List<T> FindObjectOfTypeAll<T> ()
	{
		List<T> results = new List<T> ();

		var s = SceneManager.GetActiveScene ();

		if (s.isLoaded) {
			var allGameObjects = s.GetRootGameObjects ();

			for (int j = 0; j < allGameObjects.Length; j++) {
				var go = allGameObjects [j];
				results.Add ((go.GetComponentInChildren<T> (true)));

				if (results [results.Count - 1] != null) {
					return results;
				}
			}
		}

		return results;
	}

	public static float getStateValueByName (string statNameToShow, GameObject playerGameobject)
	{
		if (playerGameobject == null) {
			return -1;
		}

		playerComponentsManager currentplayerComponentsManager = playerGameobject.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			playerStatsSystem currentPlayerStatsSystem = currentplayerComponentsManager.getPlayerStatsSystem ();

			if (currentPlayerStatsSystem != null) {
				return currentPlayerStatsSystem.getStatValue (statNameToShow);
			}
		}

		return -1;
	}

	public static float getInventoryObjectAmountByName (string objectNameToShow, GameObject playerGameobject)
	{
		if (playerGameobject == null) {
			return -1;
		}

		playerComponentsManager currentplayerComponentsManager = playerGameobject.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			inventoryManager currentInventoryManager = currentplayerComponentsManager.getInventoryManager ();

			if (currentInventoryManager != null) {
				return currentInventoryManager.getInventoryObjectAmountByName (objectNameToShow);
			}
		}

		return -1;
	}

	public static GameObject spawnObjectFromPrefabsManager (string objectToSpawnName)
	{
		GKC_Utils.instantiateMainManagerOnSceneWithType ("Prefabs Manager", typeof(prefabsManager));

		prefabsManager mainPrefabsManager = FindObjectOfType<prefabsManager> ();

		if (mainPrefabsManager != null) {
			return mainPrefabsManager.getPrefabGameObject (objectToSpawnName);
		}

		return null;
	}

	public static prefabsManager addPrefabsManagerToScene ()
	{
		#if UNITY_EDITOR

		prefabsManager newPrefabsManager = FindObjectOfType<prefabsManager> ();

		if (newPrefabsManager == null) {
			string prefabsPath = pathInfoValues.getPrefabsManagerPrefabPath ();

			GameObject prefabsManagerPrefab = (GameObject)AssetDatabase.LoadAssetAtPath (prefabsPath, typeof(GameObject));

			if (prefabsManagerPrefab != null) {
				GameObject newPrefabsManagerGameObject = (GameObject)Instantiate (prefabsManagerPrefab, Vector3.zero, Quaternion.identity);

				newPrefabsManagerGameObject.name = "Prefabs Manager";

				newPrefabsManagerGameObject.transform.position = Vector3.zero;
				newPrefabsManagerGameObject.transform.rotation = Quaternion.identity;

				newPrefabsManager = newPrefabsManagerGameObject.GetComponent<prefabsManager> ();
			}
		}

		return newPrefabsManager;
		#else
		return null;
		#endif
	}

	public static void setSelectedGameObjectOnUI (bool ignoreCheckGamepad, bool isUsingGamepad, GameObject newGameObject, bool showDebugPrint)
	{
		EventSystem.current.SetSelectedGameObject (null);

		if (isUsingGamepad && !ignoreCheckGamepad) {
			if (newGameObject != null) {
				EventSystem.current.SetSelectedGameObject (newGameObject);

				if (showDebugPrint) {
					print ("enabling " + newGameObject.name);
				}
			}
		}

		if (showDebugPrint) {
			if (newGameObject != null) {
				print ("setting new UI element as selected " + newGameObject.name);
			} else {
				print ("removing UI element as selected");
			}
		}
	}

	public static string getMainDataPath ()
	{
		gameManager currentGameManager = FindObjectOfType<gameManager> ();

		if (currentGameManager != null) {
			return currentGameManager.getDataPath ();
		}

		return "";
	}

	public static void addElementToPlayerScreenObjectivesManager (GameObject currentPlayer, GameObject placeToShoot, string locatedEnemyIconName)
	{
		if (currentPlayer == null) {
			return;
		}

		if (placeToShoot == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			playerScreenObjectivesSystem currentPlayerScreenObjectivesSystem = currentplayerComponentsManager.getPlayerScreenObjectivesSystem ();

			if (currentPlayerScreenObjectivesSystem != null) {
				currentPlayerScreenObjectivesSystem.addElementToPlayerList (placeToShoot, false, false, 0, true, false, 
					false, false, locatedEnemyIconName, false, Color.white, true, -1, 0, false);
			}
		}
	}

	public static void removeElementToPlayerScreenObjectivesManager (GameObject currentPlayer, GameObject objectToRemove)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			playerScreenObjectivesSystem currentPlayerScreenObjectivesSystem = currentplayerComponentsManager.getPlayerScreenObjectivesSystem ();

			if (currentPlayerScreenObjectivesSystem != null) {
				currentPlayerScreenObjectivesSystem.removeElementFromListByPlayer (objectToRemove);
			}
		}
	}

	public static void removeElementListToPlayerScreenObjectivesManager (GameObject currentPlayer, List<Transform> objectToRemoveList)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			playerScreenObjectivesSystem currentPlayerScreenObjectivesSystem = currentplayerComponentsManager.getPlayerScreenObjectivesSystem ();

			if (currentPlayerScreenObjectivesSystem != null) {
				currentPlayerScreenObjectivesSystem.removeElementListFromListByPlayer (objectToRemoveList);
			}
		}
	}

	public static void removeTargetFromAIEnemyList (GameObject objectToRemove)
	{
		if (objectToRemove == null) {
			return;
		}

		playerController[] playerControllerList = FindObjectsOfType<playerController> ();

		foreach (playerController currentPlayerController in playerControllerList) {
			if (currentPlayerController.usedByAI) {
				playerComponentsManager currentplayerComponentsManager = currentPlayerController.gameObject.GetComponent<playerComponentsManager> ();

				if (currentplayerComponentsManager != null) {
					
					findObjectivesSystem currentFindObjectivesSystem = currentPlayerController.GetComponent<findObjectivesSystem> ();

					if (currentFindObjectivesSystem != null) {
						currentFindObjectivesSystem.setObjectOutOfRange (objectToRemove);
					}
				}
			}
		}
	}

	public static void createPieceMeshesObjectsFromSetByName (GameObject objectToCreate, string objectName)
	{
		if (objectToCreate == null) {
			return;
		}

		print (objectToCreate.name);

		GameObject objectToCreateClone = Instantiate (objectToCreate, objectToCreate.transform.position, objectToCreate.transform.rotation);

		objectToCreateClone.name = objectName;

		if (!objectToCreateClone.activeSelf) {
			objectToCreateClone.SetActive (true);
		}

		SkinnedMeshRenderer currentSkinnedMeshRenderer = objectToCreateClone.GetComponent<SkinnedMeshRenderer> ();

		if (currentSkinnedMeshRenderer == null) {
			return;
		}

		Mesh currentMesh = currentSkinnedMeshRenderer.sharedMesh;

		List<Material> materialList = new List<Material> ();
			
		int materialsLength = currentSkinnedMeshRenderer.sharedMaterials.Length;

		for (int j = 0; j < materialsLength; j++) {

			Material currentMaterial = currentSkinnedMeshRenderer.sharedMaterials [j];

			if (currentMaterial != null) {

				print (currentMaterial.name);

				materialList.Add (new Material (currentMaterial));

			}
		}

		MeshFilter currentMeshFilter = objectToCreateClone.AddComponent<MeshFilter> ();

		currentMeshFilter.mesh = currentMesh;

		MeshRenderer currentMeshRenderer = objectToCreateClone.AddComponent<MeshRenderer> ();


		currentMeshRenderer.sharedMaterials = new Material[materialList.Count];


		materialsLength = materialList.Count;

		Material[] allMats = currentMeshRenderer.sharedMaterials;

		for (int j = 0; j < materialsLength; j++) {
			allMats [j] = materialList [j];
		}

		currentMeshRenderer.sharedMaterials = allMats;

		if (currentSkinnedMeshRenderer != null) {
			DestroyImmediate (currentSkinnedMeshRenderer);
		}
	}

	public static void addNewBlueprintsUnlockedElement (GameObject currentPlayer, string newBlueprintsUnlockedElement)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				currentCraftingSystem.addNewBlueprintsUnlockedElement (newBlueprintsUnlockedElement);
			}
		}
	}

	public static void addNewBlueprintsUnlockedList (GameObject currentPlayer, List<string> craftingRecipesList)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				for (int j = 0; j < craftingRecipesList.Count; j++) {
					currentCraftingSystem.addNewBlueprintsUnlockedElement (craftingRecipesList [j]);
				}
			}
		}
	}

	public static void setBlueprintsUnlockedListValue (GameObject currentPlayer, List<string> craftingRecipesList)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				currentCraftingSystem.setBlueprintsUnlockedListValue (craftingRecipesList);
			}
		}
	}

	public static List<string> getBlueprintsUnlockedListValue (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return null;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				return currentCraftingSystem.getBlueprintsUnlockedListValue ();
			}
		}

		return null;
	}

	public static bool isUseOnlyBlueprintsUnlockedActive (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return false;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				return currentCraftingSystem.isUseOnlyBlueprintsUnlockedActive ();
			}
		}

		return false;
	}

	public static bool anyObjectToCraftInTimeActive (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return false;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				return currentCraftingSystem.anyObjectToCraftInTimeActive ();
			}
		}

		return false;
	}

	public static void setUseOnlyBlueprintsUnlockedState (GameObject currentPlayer, bool state)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				currentCraftingSystem.setUseOnlyBlueprintsUnlockedState (state);
			}
		}
	}

	public static List<craftObjectInTimeSimpleInfo> getCraftObjectInTimeInfoList (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return null;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				return currentCraftingSystem.getCraftObjectInTimeInfoList ();
			}
		}

		return null;
	}

	public static void setCraftObjectInTimeInfoList (GameObject currentPlayer, List<craftObjectInTimeSimpleInfo> newCraftObjectInTimeSimpleInfoList)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				currentCraftingSystem.setCraftObjectInTimeInfoList (newCraftObjectInTimeSimpleInfoList);
			}
		}
	}

	public static List<string> getObjectCategoriesToCraftAvailableAtAnyMomentValue (GameObject currentPlayer)
	{
		if (currentPlayer == null) {
			return null;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				return currentCraftingSystem.getObjectCategoriesToCraftAvailableAtAnyMomentValue ();
			}
		}

		return null;
	}

	public static void setObjectCategoriesToCraftAvailableAtAnyMomentValue (GameObject currentPlayer, List<string> newList)
	{
		if (currentPlayer == null) {
			return;
		}

		playerComponentsManager currentplayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentplayerComponentsManager != null) {
			craftingSystem currentCraftingSystem = currentplayerComponentsManager.getCraftingSystem ();

			if (currentCraftingSystem != null) {
				currentCraftingSystem.setObjectCategoriesToCraftAvailableAtAnyMomentValue (newList);
			}
		}
	}
}